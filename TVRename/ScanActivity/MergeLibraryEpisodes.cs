// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    class MergeLibraryEpisodes : ScanShowActivity
    {
        public MergeLibraryEpisodes(TVDoc doc) : base(doc) {}
        protected override string Checkname() => "Created Merge Rules for episodes in the library";

        protected override void Check(ShowItem si, DirFilesCache dfc, TVDoc.ScanSettings settings)
        {
            if (settings.Token.IsCancellationRequested)
                throw new TVRenameOperationInteruptedException();

            if (!TVSettings.Instance.AutoMergeLibraryEpisodes) return;

            Dictionary<int, List<string>> allFolders = si.AllExistngFolderLocations();

            if (allFolders.Count == 0) // no folders defined for this show
                return; // so, nothing to do.

            int[] numbers = new int[si.SeasonEpisodes.Keys.Count];
            si.SeasonEpisodes.Keys.CopyTo(numbers, 0);

            // process each folder for each season...
            foreach (int snum in numbers)
            {
                if (settings.Token.IsCancellationRequested)
                    throw new TVRenameOperationInteruptedException();

                if ((si.IgnoreSeasons.Contains(snum)) || (!allFolders.ContainsKey(snum)))
                    continue; // ignore/skip this season

                if ((snum == 0) && (si.CountSpecials))
                    continue; // don't process the specials season, as they're merged into the seasons themselves

                if ((snum == 0) && TVSettings.Instance.IgnoreAllSpecials)
                    continue;

                // all the folders for this particular season
                List<string> folders = allFolders[snum];

                List<ProcessedEpisode> eps = si.SeasonEpisodes[snum];

                List<ShowRule> rulesToAdd = new List<ShowRule>();

                foreach (string folder in folders)
                {
                    if (settings.Token.IsCancellationRequested)
                        throw new TVRenameOperationInteruptedException();

                    FileInfo[] files = dfc.GetFiles(folder);
                    if (files == null)
                        continue;

                    foreach (FileInfo fi in files)
                    {
                        if (settings.Token.IsCancellationRequested)
                        {
                            throw new TVRenameOperationInteruptedException();
                        }

                        if (!fi.IsMovieFile())
                        {
                            continue; //not a video file, so ignore
                        }

                        if (!FinderHelper.FindSeasEp(fi, out int seasNum, out int epNum, out int maxEp, si,
                            out TVSettings.FilenameProcessorRE _))
                        {
                            continue; // can't find season & episode, so this file is of no interest to us
                        }

                        if (seasNum == -1)
                        {
                            seasNum = snum;
                        }

                        int epIdx = eps.FindIndex(x =>
                            ((x.AppropriateEpNum == epNum) && (x.AppropriateSeasonNumber == seasNum)));

                        if (epIdx == -1)
                        {
                            continue; // season+episode number don't correspond to any episode we know of from thetvdb
                        }

                        ProcessedEpisode ep = eps[epIdx];

                        if (ep.Type != ProcessedEpisode.ProcessedEpisodeType.merged && maxEp != -1)
                        {
                            LOGGER.Info(
                                $"Looking at {ep.Show.ShowName} and have identified that episode {epNum} and {maxEp} of season {seasNum} should be merged into one file {fi.FullName}");

                            ShowRule sr = new ShowRule
                            {
                                DoWhatNow = RuleAction.kMerge,
                                First = epNum,
                                Second = maxEp
                            };

                            rulesToAdd.Add(sr);
                        }
                    } // foreach file in folder
                } // for each folder for this season of this show

                foreach (ShowRule sr in rulesToAdd)
                {
                    si.AddSeasonRule(snum, sr);
                    LOGGER.Info($"Added new rule automatically for {sr}");

                    //Regenerate the episodes with the new rule added
                    ShowLibrary.GenerateEpisodeDict(si);
                }
            } // for each season of this show
        }

        protected override bool Active() => TVSettings.Instance.AutoMergeLibraryEpisodes;
    }
}
