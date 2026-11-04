//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
namespace TVRename;

internal class MergeLibraryEpisodes(TVDoc doc) : ScanShowActivity(doc)
{
    protected override string ActivityName() => "Created Merge Rules for episodes in the library";

    /// <exception cref="TVRenameOperationInterruptedException">Condition.</exception>
    protected override async Task CheckAsync(ShowConfiguration si, DirFilesCache dfc, TVDoc.ScanSettings settings)
    {
        if (settings.Token.IsCancellationRequested)
        {
            throw new TVRenameOperationInterruptedException();
        }

        if (!Active())
        {
            return;
        }

        Dictionary<int, List<string>> allFolders = await si.AllExistngFolderLocationsAsync();

        if (allFolders.Count == 0) // no folders defined for this show
        {
            return; // so, nothing to do.
        }

        // process each folder for each season...
        foreach (int snum in si.GetSeasonKeys())
        {
            if (settings.Token.IsCancellationRequested)
            {
                throw new TVRenameOperationInterruptedException();
            }

            if (si.IgnoreSeasons.Contains(snum) || !allFolders.TryGetValue(snum, out List<string>? folders))
            {
                continue;
            }

            // all the folders for this particular season
            MergeShowEpisodes(si, dfc, snum, folders, settings.Token);
        } // for each season of this show
    }

    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    private static void MergeShowEpisodes(ShowConfiguration si, DirFilesCache dfc, int snum, IEnumerable<string> folders, CancellationToken token)
    {
        if (snum == 0 && si.CountSpecials)
        {
            return;
        }

        if (snum == 0 && TVSettings.Instance.IgnoreAllSpecials)
        {
            return;
        }

        List<ProcessedEpisode> eps = si.AllEpisodesForSeason(snum);

        List<ShowRule> rulesToAdd = [];

        foreach (string folder in folders)
        {
            if (token.IsCancellationRequested)
            {
                throw new TVRenameOperationInterruptedException();
            }

            FileInfo[] files = dfc.GetFiles(folder);

            foreach (FileInfo fi in files)
            {
                if (token.IsCancellationRequested)
                {
                    throw new TVRenameOperationInterruptedException();
                }

                if (!fi.IsMovieFile())
                {
                    continue; //not a video file, so ignore
                }

                var x = new Utility.FileMatcher().Match(fi,si);
                if (!x.IsMatch || !x.FoundEpisode.HasValue)
                {
                    continue; // can't find season & episode, so this file is of no interest to us
                }

                int seasNum = x.FoundSeason ?? snum;

                int epIdx = eps.FindIndex(ep => ep.AppropriateEpNum == x.FoundEpisode && ep.AppropriateSeasonNumber == seasNum);

                if (epIdx == -1)
                {
                    continue; // season+episode number don't correspond to any episode we know of
                }

                ProcessedEpisode ep = eps[epIdx];

                if (ep.Type == ProcessedEpisode.ProcessedEpisodeType.merged || !x.FoundMaxEpisode.HasValue)
                {
                    continue;
                }

                LOGGER.Info(
                    $"Looking at {ep.Show.ShowName} and have identified that episode {x.FoundEpisode} and {x.FoundMaxEpisode} of season {seasNum} should be merged into one file {fi.FullName}");

                ShowRule sr = new()
                {
                    DoWhatNow = RuleAction.kMerge,
                    First = x.FoundEpisode,
                    Second = x.FoundMaxEpisode  
                };

                rulesToAdd.Add(sr);
            } // foreach file in folder
        } // for each folder for this season of this show

        foreach (ShowRule sr in rulesToAdd)
        {
            si.AddSeasonRule(snum, sr);
            LOGGER.Info($"Added new rule automatically for {sr}");
        }

        if (rulesToAdd.IsAny())
        {
            //Regenerate the episodes with the new rule added
            si.UpdateEpisodeCaches();
        }
    }

    protected override bool Active() => TVSettings.Instance.AutoMergeLibraryEpisodes;
}
