using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename;

internal class RenameAndMissingMovieCheck : ScanMovieActivity
{
    private readonly DownloadIdentifiersController downloadIdentifiers;

    protected override string ActivityName() => "Rename & Missing Movie Check";

    public RenameAndMissingMovieCheck(TVDoc doc) : base(doc)
    {
        downloadIdentifiers = new DownloadIdentifiersController();
    }

    protected override void Check(MovieConfiguration si, DirFilesCache dfc, TVDoc.ScanSettings settings)
    {
        List<string> allFolders = si.Locations.ToList();
        if (allFolders.Count == 0) // no folders defined for this show
        {
            LOGGER.Warn($"No Folders defined for {si.Name}, please review the configuration for that movie.");
            return; // so, nothing to do.
        }

        // process each folder for show...
        foreach (string folder in allFolders)
        {
            if (settings.Token.IsCancellationRequested)
            {
                return;
            }

            CheckMovieFolder(si, dfc, settings, folder);
        }
    }

    private void CheckMovieFolder(MovieConfiguration si, DirFilesCache dfc, TVDoc.ScanSettings settings, string folder)
    {
        if (settings.Token.IsCancellationRequested)
        {
            return;
        }

        bool renCheck = TVSettings.Instance.RenameCheck && si.DoRename && Directory.Exists(folder); // renaming check needs the folder to exist

        bool missCheck = TVSettings.Instance.MissingCheck && si.DoMissingCheck;

        if (!renCheck && !missCheck)
        {
            return;
        }

        switch (si.Format)
        {
            case MovieConfiguration.MovieFolderFormat.multiPerDirectory:
                CheckMultiPartMovieFolder(si, settings, folder, dfc, renCheck, missCheck);
                return;
            case MovieConfiguration.MovieFolderFormat.singleDirectorySingleFile:
                CheckSingleMovieFolder(si, settings, folder, dfc, renCheck);
                return;
            case MovieConfiguration.MovieFolderFormat.dvd:
            case MovieConfiguration.MovieFolderFormat.bluray:
                CheckDvdBluRayMovieFolder(si, folder);
                return;
            default:
                LOGGER.Error($"Unclear how to check {si.Name} with format {si.Format.PrettyPrint()}.");
                return;
        }
    }

    private void CheckSingleMovieFolder(MovieConfiguration si, TVDoc.ScanSettings settings, string folder, DirFilesCache dfc, bool renCheck)
    {
        FileInfo[] files = dfc.GetFiles(folder);
        FileInfo[] movieFiles = files.Where(f => f.IsMovieFile()).Where(f => !f.IsSampleFile()).ToArray();
        List<string> bases = movieFiles.Select(fi => fi.MovieFileNameBase()).Distinct().ToList();
        string newBase = TVSettings.Instance.FilenameFriendly(si.ProposedFilename);

        if (movieFiles.Length == 0)
        {
            FileIsMissing(si, folder);
            return;
        }

        if (bases.Count == 1 && si.NameMatch(bases[0]))
        {
            string baseString = bases[0];
            //All Seems OK
            FileInfo matchingMovieFile = movieFiles.First(m => si.NameMatch(m, false));
            FileInfo newFile = matchingMovieFile;

            if (renCheck && !baseString.Equals(newBase, StringComparison.OrdinalIgnoreCase))
            {
                //Do a tweak to filename (case insensitive ones are dealt with below; this is for changes that are around punctuation
                PlanToRenameFilesInFolder(si, settings, folder, files, baseString, newBase);
                string newName = baseString.HasValue() ? matchingMovieFile.Name.Replace(baseString, newBase) : newBase + matchingMovieFile.Extension;
                newFile = FileHelper.FileInFolder(folder, newName); // rename updates the filename
            }

            //This is the code that will iterate over the DownloadIdentifiers and ask each to ensure that
            //it has all the required files for that show
            Doc.TheActionList.Add(downloadIdentifiers.ProcessMovie(si, newFile));
            FileIsCorrect(si, movieFiles.First().FullName);
            return;
        }

        if (renCheck && bases.Select(b => b.ToLower()).Distinct().Count() == 1 && bases.All(si.NameMatch))
        {
            //We have a case sensitive issue and have been asked to rename
            if (TVSettings.Instance.FileNameCaseSensitiveMatch)
            {
                foreach (string baseString in bases)
                {
                    PlanToRenameFilesInFolder(si, settings, folder, files, baseString, newBase);
                }
            }
            Doc.TheActionList.Add(downloadIdentifiers.ProcessMovie(si, movieFiles.FirstOrDefault(m => m.Name.StartsWith(newBase.RemoveBracketedYearFromEnd(), StringComparison.OrdinalIgnoreCase)) ?? movieFiles.First()));
            return;
        }

        //Check for changing of the year
        if (renCheck && bases.Select(b => b.RemoveBracketedYearFromEnd().ToLower()).Distinct().Count() == 1 &&
            bases.All(b => b.RemoveBracketedYearFromEnd().Equals(newBase.RemoveBracketedYearFromEnd(), StringComparison.CurrentCultureIgnoreCase)))
        {
            foreach (string baseString in bases)
            {
                PlanToRenameFilesInFolder(si, settings, folder, files, baseString, newBase);
            }
            return;
        }

        //Check for random crap at the end
        if (renCheck
            && bases.Select(b => b.RemoveBracketedYearFromEnd().ToLower()).Distinct().Count() == 1
            && bases.All(b => b.StartsWith(newBase.RemoveBracketedYearFromEnd(), StringComparison.CurrentCultureIgnoreCase))
            && newBase.RemoveBracketedYearFromEnd().Length > 6)
        {
            foreach (string baseString in bases)
            {
                PlanToRenameFilesInFolder(si, settings, folder, files, baseString, newBase);
            }
            return;
        }

        if (bases.Any(si.NameMatch))
        {
            FileInfo? matchingFile = movieFiles
                .FirstOrDefault(m => si.NameMatch(m, false));

            if (matchingFile != null)
            {
                Doc.TheActionList.Add(downloadIdentifiers.ProcessMovie(si, matchingFile));
                FileIsCorrect(si, matchingFile.FullName);
                LOGGER.Warn($"{matchingFile.Name} matches {newBase}, but other files [{movieFiles.Select(f => f.Name).ToCsv()}] are present; please review.");
                return;
            }

            LOGGER.Error($"Bases match {bases.ToCsv()}, but no files do {movieFiles.ToCsv()}");
        }

        LOGGER.Warn($"Unclear what to do with '{bases.ToCsv()}' for {si} with {newBase}. Marking as missing.");
        FileIsMissing(si, folder);
    }

    private void CheckMultiPartMovieFolder(MovieConfiguration si, TVDoc.ScanSettings settings, string folder, DirFilesCache dfc,
        bool renCheck, bool missCheck)
    {
        FileInfo[] files = dfc.GetFiles(folder);
        FileInfo[] movieFiles = files.Where(f => f.IsMovieFile()).ToArray();
        List<string> bases = movieFiles.Select(fi => fi.MovieFileNameBase()).Distinct().ToList();
        string newBase = TVSettings.Instance.FilenameFriendly(si.ProposedFilename);

        if (movieFiles.Length == 0)
        {
            FileIsMissing(si, folder);
            return;
        }

        //we have 3 options - matching file / close file that needs rename / else it's missing
        if (bases.Any(x => x.Equals(newBase)))
        {
            Doc.TheActionList.Add(downloadIdentifiers.ProcessMovie(si,
                movieFiles.First(m => m.Name.StartsWith(newBase, StringComparison.Ordinal))));

            return;
        }

        if (renCheck)
        {
            //This section deals with files that have had a 1 year rename
            List<string> matchingBases = bases.Where(x => IsClose(x, si)).ToList();
            if (matchingBases.Any())
            {
                foreach (string baseString in matchingBases)
                {
                    //rename all files with this base
                    PlanToRenameFilesInFolder(si, settings, folder, files, baseString, newBase);
                }

                return;
            }

            List<string> matchingBases2 = bases.Where(x => MatchesBase(x, newBase)).ToList();
            if (matchingBases2.Any())
            {
                foreach (string baseString in matchingBases2)
                {
                    //rename all files with this base
                    PlanToRenameFilesInFolder(si, settings, folder, files, baseString, newBase);
                }

                return;
            }
        }

        if (missCheck)
        {
            FileIsMissing(si, folder);
        }
    }

    private void CheckDvdBluRayMovieFolder(MovieConfiguration si, string folder)
    {
        string targetFile = si.Format == MovieConfiguration.MovieFolderFormat.bluray
            ? Path.Combine(folder, "BDMV", "index.bdmv")
            : Path.Combine(folder, "VIDEO_TS", "VIDEO_TS.IFO");

        if (File.Exists(targetFile))
        {
            Doc.TheActionList.Add(downloadIdentifiers.ProcessMovie(si, new FileInfo(targetFile)));
            FileIsCorrect(si, targetFile);
        }
        else
        {
            FileIsMissing(si, folder);
        }
    }

    private static bool MatchesBase(string baseFileName, string newBase)
    {
        if (baseFileName.CompareName().StartsWith(newBase.CompareName(), StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private void PlanToRenameFilesInFolder(MovieConfiguration si, TVDoc.ScanSettings settings, string folder, FileInfo[] files,
        string baseString, string newBase)
    {
        foreach (FileInfo fi in files)
        {
            if (settings.Token.IsCancellationRequested)
            {
                return;
            }

            if (fi.Name.StartsWith(baseString, StringComparison.CurrentCultureIgnoreCase))
            {
                string newName = baseString.HasValue() ? fi.Name.Replace(baseString, newBase) : newBase + fi.Extension;
                FileInfo newFile = FileHelper.FileInFolder(folder, newName); // rename updates the filename

                if (newFile.IsMovieFile())
                {
                    //This is the code that will iterate over the DownloadIdentifiers and ask each to ensure that
                    //it has all the required files for that show
                    Doc.TheActionList.Add(downloadIdentifiers.ProcessMovie(si, newFile));
                }

                if (newFile.FullName != fi.FullName)
                {
                    //Check that the file does not already exist
                    //if (FileHelper.FileExistsCaseSensitive(newFile.FullName))
                    if (FileHelper.FileExistsCaseSensitive(files, newFile))
                    {
                        LOGGER.Warn(
                            $"Identified that {fi.FullName} should be renamed to {newName}, but it already exists.");
                    }
                    else
                    {
                        LOGGER.Info($"Identified that {fi.FullName} should be renamed to {newName}.");
                        Doc.TheActionList.Add(new ActionCopyMoveRename(ActionCopyMoveRename.Op.rename, fi,
                            newFile, si, false, null, Doc));

                        //The following section informs the DownloadIdentifers that we already plan to
                        //copy a file in the appropriate place and they do not need to worry about downloading
                        //one for that purpose
                        downloadIdentifiers.NotifyComplete(newFile);
                    }
                }
                else
                {
                    if (fi.IsMovieFile())
                    {
                        FileIsCorrect(si, fi.FullName);
                    }
                }
            }
        } // foreach file in folder
    }

    private void FileIsCorrect(MovieConfiguration si, string fi)
    {
        //File is correct name
        LOGGER.Debug($"Identified that {fi} is in the right place. Marking it as 'seen'.");
        //Record this movie as seen

        TVSettings.Instance.PreviouslySeenMovies.EnsureAdded(si);
        if (TVSettings.Instance.IgnorePreviouslySeenMovies)
        {
            Doc.SetDirty();
        }
    }

    private static bool IsClose(string baseFileName, MovieConfiguration config)
    {
        (string targetFolder, string targetFolderEarlier, string targetFolderLater) = config.NeighbouringFolderNames();
        return baseFileName.Equals(targetFolderEarlier) || baseFileName.Equals(targetFolderLater) || baseFileName.Equals(targetFolder);
    }

    private void FileIsMissing(MovieConfiguration si, string folder)
    {
        // second part of missing check is to see what is missing!
        if (TVSettings.Instance.MissingCheck && si.DoMissingCheck)
        {
            DateTime? dt = si.CachedMovie?.FirstAired;

            bool inPast = dt.HasValue && dt.Value.CompareTo(DateTime.Now) < 0;
            bool shouldCheckFutureDated = si.ForceCheckFuture && dt.HasValue;
            bool shouldCheckNoDatedMovies = si.ForceCheckNoAirdate && !dt.HasValue;

            if (inPast || shouldCheckFutureDated || shouldCheckNoDatedMovies)
            {
                // then add it as officially missing
                Doc.TheActionList.Add(new MovieItemMissing(si, folder));
                LOGGER.Info($"Identified that {si.Name} is missing from {folder}.");
            }
            else
            {
                LOGGER.Info($"{si.Name} not considered missing as it {(dt.HasValue ? $"is in the future ({dt.Value.ToString("d", System.Globalization.DateTimeFormatInfo.CurrentInfo)})" : "has no airdate")} (and the settings)");
            }
        } // if doing missing check
    }

    protected override bool Active() => true;
}
