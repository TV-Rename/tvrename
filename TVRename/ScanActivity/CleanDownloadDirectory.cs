using System;
using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

namespace TVRename;

internal class CleanDownloadDirectory : ScanActivity
{
    public CleanDownloadDirectory(TVDoc doc, TVDoc.ScanSettings settings) : base(doc, settings)
    {
        filesThatMayBeNeeded = new List<FileInfo>();
        returnActions = new ItemList();
        showList = new List<ShowConfiguration>();
        movieList = new List<MovieConfiguration>();
    }

    private List<FileInfo> filesThatMayBeNeeded;
    private readonly DirFilesCache dfc = new();
    private ICollection<ShowConfiguration> showList;
    private ICollection<MovieConfiguration> movieList;
    private readonly ItemList returnActions;

    public override bool Active() => TVSettings.Instance.RemoveDownloadDirectoriesFiles ||
                                     TVSettings.Instance.RemoveDownloadDirectoriesFilesMatchMovies ||
                                     TVSettings.Instance.ReplaceWithBetterQuality ||
                                     TVSettings.Instance.ReplaceMoviesWithBetterQuality ||
                                     TVSettings.Instance.CopyFutureDatedEpsFromSearchFolders;

    protected override string CheckName() => "Cleaned up and files in download directory that are not needed";

    protected override void DoCheck(SetProgressDelegate progress)
    {
        returnActions.Clear();
        showList = MDoc.TvLibrary.GetSortedShowItems(); //We ignore the current set of shows being scanned to be secrure that no files are deleted for unscanned shows
        movieList = MDoc.FilmLibrary.GetSortedMovies();

        //for each directory in settings directory
        //for each file in directory
        //for each saved show (order by recent)
        //is file already available?
        //if so add show to list of files to be removed

        int totalDownloadFolders = TVSettings.Instance.DownloadFolders.Count;
        int c = 0;

        foreach (string dirPath in TVSettings.Instance.DownloadFolders.ToList())
        {
            UpdateStatus(c++, totalDownloadFolders, dirPath);

            if (!Directory.Exists(dirPath) || Settings.Token.IsCancellationRequested)
            {
                continue;
            }

            filesThatMayBeNeeded = new List<FileInfo>();

            ReviewFilesInDownloadDirectory(dirPath, Settings.Owner);
            ReviewDirsInDownloadDirectory(dirPath);
        }

        ItemList removeActions = new();
        //Remove any missing items we are planning to resolve
        foreach (ActionCopyMoveRename acmr in returnActions.OfType<ActionCopyMoveRename>())
        {
            removeActions.AddRange(MDoc.TheActionList.MissingEpisodes.Where(missingItem => missingItem.Episode == acmr.Episode));
            removeActions.AddRange(MDoc.TheActionList.MissingMovies.Where(missingItem => missingItem.Movie == acmr.Movie));
        }

        MDoc.TheActionList.Replace(removeActions, returnActions);
    }

    private void ReviewDirsInDownloadDirectory(string dirPath)
    {
        try
        {
            foreach (string subDirPath in Directory.GetDirectories(dirPath, "*",
                         System.IO.SearchOption.AllDirectories).Where(Directory.Exists))
            {
                if (Settings.Token.IsCancellationRequested)
                {
                    return;
                }

                ReviewDirInDownloadDirectory(subDirPath);
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            LOGGER.Warn(ex, $"Could not access subdirectories of {dirPath}");
        }
        catch (System.IO.DirectoryNotFoundException ex)
        {
            LOGGER.Warn(ex, $"Could not access subdirectories of {dirPath}");
        }
        catch (System.IO.IOException ex)
        {
            LOGGER.Warn(ex, $"Could not access subdirectories of {dirPath}");
        }
    }

    private void ReviewDirInDownloadDirectory(string subDirPath)
    {
        //we are not checking for any file updates, so can return
        if (!TVSettings.Instance.RemoveDownloadDirectoriesFiles && !TVSettings.Instance.RemoveDownloadDirectoriesFilesMatchMovies)
        {
            return;
        }

        if (!Directory.Exists(subDirPath))
        {
            return;
        }

        DirectoryInfo di = new(subDirPath);

        FileInfo? neededFile = filesThatMayBeNeeded.FirstOrDefault(info => info.DirectoryName.Contains(di.FullName));
        if (neededFile != null)
        {
            LOGGER.Info($"Not removing {di.FullName} as it contains {neededFile.FullName} which may be needed.");
            return;
        }

        List<MovieConfiguration> matchingMovies = movieList.Where(mi => mi.NameMatch(di, TVSettings.Instance.UseFullPathNameToMatchSearchFolders)).ToList();

        List<ShowConfiguration> matchingShows = showList.Where(si => si.NameMatch(di, TVSettings.Instance.UseFullPathNameToMatchSearchFolders)).ToList();

        if (!matchingShows.Any() && !matchingMovies.Any())
        {
            return; // Some sort of random file - ignore
        }

        List<ShowConfiguration> neededMatchingShows = matchingShows.Where(si => FinderHelper.FileNeeded(di, si, dfc)).ToList();
        if (neededMatchingShows.Any())
        {
            LOGGER.Info($"Not removing {di.FullName} as it may be needed for {neededMatchingShows.Select(x => x.ShowName).ToCsv()}");
            return;
        }

        List<MovieConfiguration> neededMatchingMovie = matchingMovies.Where(si => FinderHelper.FileNeeded(di, si, dfc)).ToList();
        if (neededMatchingMovie.Any())
        {
            LOGGER.Info($"Not removing {di.FullName} as it may be needed for {neededMatchingMovie.Select(x => x.ShowName).ToCsv()}");
            return;
        }

        if (matchingShows.Any() && TVSettings.Instance.RemoveDownloadDirectoriesFiles)
        {
            returnActions.Add(SetupDirectoryRemoval(di, matchingShows));
        }
        if (matchingMovies.Any() && TVSettings.Instance.RemoveDownloadDirectoriesFilesMatchMovies)
        {
            returnActions.Add(SetupDirectoryRemoval(di, matchingMovies));
        }
    }

    private static Action SetupDirectoryRemoval(DirectoryInfo di,
        IReadOnlyList<ShowConfiguration> matchingShows)
    {
        ShowConfiguration si = matchingShows[0]; //Choose the first cachedSeries
        FinderHelper.FindSeasEp(di, out int seasF, out int epF, si, out TVSettings.FilenameProcessorRE? _);
        CachedSeriesInfo? s = si.CachedShow;
        if (s is null)
        {
            throw new ArgumentNullException(nameof(s));
        }

        ProcessedEpisode pep = si.GetEpisode(seasF, epF);
        LOGGER.Info(
            $"Removing {di.FullName} as it matches {si.ShowName} and no episodes are needed");

        return new ActionDeleteDirectory(di, pep, TVSettings.Instance.Tidyup);
    }

    private static Action SetupDirectoryRemoval(DirectoryInfo di,
        IReadOnlyList<MovieConfiguration> matchingMovies)
    {
        MovieConfiguration si = matchingMovies[0]; //Choose the first cachedSeries
        LOGGER.Info($"Removing {di.FullName} as it matches {si.ShowName} and no files are needed");

        return new ActionDeleteDirectory(di, si, TVSettings.Instance.Tidyup);
    }

    private void ReviewFilesInDownloadDirectory(string dirPath, IDialogParent owner)
    {
        try
        {
            foreach (string filePath in Directory.GetFiles(dirPath, "*", System.IO.SearchOption.AllDirectories).Where(File.Exists))
            {
                if (Settings.Token.IsCancellationRequested)
                {
                    return;
                }

                FileInfo fi = new(filePath);

                if (fi.IgnoreFile())
                {
                    continue;
                }

                ReviewFileInDownloadDirectory(Settings.Unattended, fi, owner);
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            LOGGER.Warn(ex, $"Could not access files in {dirPath}");
        }
        catch (System.IO.DirectoryNotFoundException ex)
        {
            LOGGER.Warn(ex, $"Could not access files in {dirPath}");
        }
        catch (System.IO.IOException ex)
        {
            LOGGER.Warn(ex, $"Could not access files in {dirPath}");
        }
    }

    private void ReviewFileInDownloadDirectory(bool unattended, FileInfo fi,  IDialogParent owner)
    {
        List<ShowConfiguration> matchingShowsAll = showList.Where(si => si.NameMatch(fi, TVSettings.Instance.UseFullPathNameToMatchSearchFolders)).ToList();
        List<ShowConfiguration> matchingShows = FinderHelper.RemoveShortShows(matchingShowsAll);
        List<MovieConfiguration> matchingMoviesAll = movieList.Where(mi => mi.NameMatch(fi, TVSettings.Instance.UseFullPathNameToMatchSearchFolders)).ToList();
        List<MovieConfiguration> matchingMovies = FinderHelper.RemoveShortShows(matchingMoviesAll);

        List<MovieConfiguration> matchingMoviesNoShows =
            FinderHelper.RemoveShortMedia(matchingMovies, matchingShows);
        //List<ShowConfiguration> matchingShowsNoMovies = FinderHelper.RemoveShortMedia(matchingShows, matchingMovies);

        if (!matchingMovies.Any() && !matchingShows.Any())
        {
            // Some sort of random file - ignore
            return;
        }

        bool showCheck = TVSettings.Instance.RemoveDownloadDirectoriesFiles && matchingShows.Any();
        bool movieCheck = TVSettings.Instance.RemoveDownloadDirectoriesFilesMatchMovies && matchingMovies.Any();
        bool fileCanBeDeleted = showCheck || movieCheck;

        ProcessedEpisode? firstMatchingPep = null;

        foreach (ShowConfiguration si in matchingShows)
        {
            (bool? x,ProcessedEpisode? matchingEpisode) = CanFileBeDeletedForShow(unattended, fi, owner, si, matchingShows);
            if (x is false)
            {
                fileCanBeDeleted = false;
            }
            else if (matchingEpisode != null)
            {
                firstMatchingPep = matchingEpisode;
            }
        }

        List<MovieConfiguration> neededMatchingMovie = matchingMovies.Where(si => FinderHelper.FileNeeded(fi, si, dfc)).ToList();
        if (neededMatchingMovie.Any())
        {
            LOGGER.Info($"Not removing {fi.FullName} as it may be needed for {neededMatchingMovie.Select(x => x.ShowName).ToCsv()}");

            fileCanBeDeleted = false;
        }
        else
        {
            if (TVSettings.Instance.RemoveDownloadDirectoriesFilesMatchMoviesLengthCheck && matchingMovies.Any() && matchingMovies.Max(c => c.ShowName.Length) <= TVSettings.Instance.RemoveDownloadDirectoriesFilesMatchMoviesLengthCheckLength)
            {
                LOGGER.Info($"Not removing {fi.FullName} as it may be needed for {matchingMovies.Select(x => x.ShowName).ToCsv()} and they are all too short");

                fileCanBeDeleted = false;
            }

            if (TVSettings.Instance.ReplaceMoviesWithBetterQuality)
            {
                bool? x = ReviewFileAgainstExistingMovies(unattended, fi, owner, matchingMoviesNoShows);
                if (x is false)
                {
                    fileCanBeDeleted = false;
                }
            }
        }

        if (fileCanBeDeleted)
        {
            RemoveFileAndReport(fi, matchingMovies, matchingShows, firstMatchingPep);
        }
        else
        {
            filesThatMayBeNeeded.Add(fi);
        }
    }

    private (bool?, ProcessedEpisode?) CanFileBeDeletedForShow(bool unattended, FileInfo fi, IDialogParent owner, ShowConfiguration si,
        List<ShowConfiguration> matchingShows)
    {
        FinderHelper.FindSeasEp(fi, out int seasF, out int epF, out int _, si, out TVSettings.FilenameProcessorRE? re);

        if (!si.SeasonEpisodes.ContainsKey(seasF))
        {
            LogError(fi, seasF, epF, re, si, "season");
            return (false,null);
        }

        ProcessedEpisode? firstMatchingEpisode = si.SeasonEpisodes[seasF].FirstOrDefault(ep => ep.AppropriateEpNum == epF);

        if (firstMatchingEpisode == null)
        {
            LogError(fi, seasF, epF, re, si, "episode");
            return (false,null);
        }

        List<FileInfo> encumbants = dfc.FindEpOnDisk(firstMatchingEpisode, false);

        if (encumbants.Count == 0)
        {
            //File is needed as there are no files for that cachedSeries/episode
            CopyFutureDatedFile(fi, firstMatchingEpisode, MDoc);
            return (false,firstMatchingEpisode);
        }
        else
        {
            bool? fileCanBeDeleted = null;
            foreach (FileInfo existingFile in encumbants)
            {
                if (existingFile.FullName.Equals(fi.FullName, StringComparison.InvariantCultureIgnoreCase))
                {
                    //the user has put the search folder and the download folder in the same place - DO NOT DELETE
                    fileCanBeDeleted = false;
                    continue;
                }

                bool? deleteFile = ReviewFile(unattended, fi, matchingShows, existingFile, firstMatchingEpisode, owner);
                if (deleteFile is false)
                {
                    fileCanBeDeleted = false;
                }
            }
            return (fileCanBeDeleted,firstMatchingEpisode);
        }
    }

    private bool? ReviewFileAgainstExistingMovies(bool unattended, FileInfo fi, IDialogParent owner, List<MovieConfiguration> matchingMovies)
    {
        bool? fileCanBeDeleted = null;

        foreach (MovieConfiguration testMovie in matchingMovies)
        {
            List<FileInfo> encumbants = dfc.FindMovieOnDisk(testMovie).ToList();

            foreach (FileInfo existingFile in encumbants)
            {
                if (existingFile.FullName.Equals(fi.FullName, StringComparison.InvariantCultureIgnoreCase))
                {
                    //the user has put the search folder and the download folder in the same place - DO NOT DELETE
                    fileCanBeDeleted = false;
                    continue;
                }

                bool? deleteFile = ReviewFile(unattended, fi, matchingMovies, existingFile, testMovie, owner);
                if (deleteFile is false)
                {
                    fileCanBeDeleted = false;
                }
            }
        }

        return fileCanBeDeleted;
    }

    private void RemoveFileAndReport(FileInfo fi, List<MovieConfiguration> matchingMovies, List<ShowConfiguration> matchingShows,
        ProcessedEpisode? firstMatchingPep)
    {
        if (matchingMovies.Any())
        {
            LOGGER.Info(
                $"Removing {fi.FullName} as it matches {matchingMovies.Select(s => s.ShowName).ToCsv()} and no files are needed for those movies");

            returnActions.Add(new ActionDeleteFile(fi, matchingMovies.LongestShowName(), TVSettings.Instance.Tidyup));
        }

        if (matchingShows.Any())
        {
            LOGGER.Info(
                $"Removing {fi.FullName} as it matches {matchingShows.Select(s => s.ShowName).ToCsv()} and no episodes are needed");

            if (!matchingMovies.Any())
            {
                returnActions.Add(new ActionDeleteFile(fi, firstMatchingPep, TVSettings.Instance.Tidyup));
            }
        }
    }

    private static void LogError(FileInfo fi, int seasF, int epF, TVSettings.FilenameProcessorRE? re, ShowConfiguration si, string typeMissing)
    {
        if (seasF == -1)
        {
            LOGGER.Info(
                $"Can't find the right {typeMissing} for {fi.FullName} coming out as S{seasF}E{epF} using rule '{re?.Notes}' for show {si.ShowName}:{si.Code}");
        }
        else
        {
            LOGGER.Warn(
                $"Can't find the right {typeMissing} for {fi.FullName} coming out as S{seasF}E{epF} using rule '{re?.Notes}' for show {si.ShowName}:{si.Code}");
        }
    }

    private bool? ReviewFile(bool unattended, FileInfo newFile, IReadOnlyCollection<ShowConfiguration> matchingShows, FileInfo existingFile, ProcessedEpisode pep, IDialogParent owner)
    {
        FileHelper.VideoComparison result = FileHelper.BetterQualityFile(existingFile, newFile);

        FileHelper.VideoComparison newResult = result;

        if (TVSettings.Instance.ReplaceWithBetterQuality && TVSettings.Instance.ForceSystemToDecideOnUpgradedFiles && IsNotClearCut(result))
        {
            //User has asked us to make a call
            newResult = existingFile.Length >= newFile.Length ? FileHelper.VideoComparison.firstFileBetter : FileHelper.VideoComparison.secondFileBetter;
        }

        switch (newResult)
        {
            case FileHelper.VideoComparison.secondFileBetter:
                if (TVSettings.Instance.ReplaceWithBetterQuality)
                {
                    if (matchingShows.Count > 1)
                    {
                        LOGGER.Warn(
                            $"Keeping {newFile.FullName}. Although it is better quality than {existingFile.FullName}, there are other shows ({matchingShows.Select(item => item.ShowName).ToCsv()}) that match.");
                    }
                    else
                    {
                        ScanHelper.UpgradeFile(newFile, pep, existingFile,MDoc,returnActions);
                    }
                }
                else
                {
                    LOGGER.Warn(
                        $"Keeping {newFile.FullName} as it is better quality than some of the current files for that show (Auto Replace with better quality files is turned off)");
                }
                return false;

            case FileHelper.VideoComparison.cantTell:
            case FileHelper.VideoComparison.similar:
                if (unattended)
                {
                    LOGGER.Info(
                        $"Keeping {newFile.FullName} as it might be better quality than {existingFile.FullName}");

                    return false;
                }
                else
                {
                    if (matchingShows.Count <= 1)
                    {
                        return ScanHelper.AskUserAboutFileReplacement(newFile, existingFile, pep, owner,MDoc,returnActions);
                    }

                    LOGGER.Warn(
                        $"Keeping {newFile.FullName}. Although it is better quality than {existingFile.FullName}, there are other shows ({matchingShows.Select(item => item.ShowName).ToCsv()}) that match.");

                    return false;
                }

            //the other cases of the files being the same or the existing file being better are not enough to save the file
            case FileHelper.VideoComparison.firstFileBetter:
                break;

            case FileHelper.VideoComparison.same:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
        return null;
    }

    private bool? ReviewFile(bool unattended, FileInfo newFile, IReadOnlyCollection<MovieConfiguration> matchingShows, FileInfo existingFile, MovieConfiguration pep, IDialogParent owner)
    {
        FileHelper.VideoComparison result = FileHelper.BetterQualityFile(existingFile, newFile);

        FileHelper.VideoComparison newResult = result;

        if (TVSettings.Instance.ReplaceMoviesWithBetterQuality && TVSettings.Instance.ForceSystemToDecideOnUpgradedFiles && IsNotClearCut(result))
        {
            //User has asked us to make a call
            newResult = existingFile.Length >= newFile.Length ? FileHelper.VideoComparison.firstFileBetter : FileHelper.VideoComparison.secondFileBetter;
        }

        switch (newResult)
        {
            case FileHelper.VideoComparison.secondFileBetter:
                if (TVSettings.Instance.ReplaceMoviesWithBetterQuality)
                {
                    if (matchingShows.Count > 1)
                    {
                        LOGGER.Warn(
                            $"Keeping {newFile.FullName}. Although it is better quality than {existingFile.FullName}, there are other shows ({matchingShows.Select(item => item.ShowName).ToCsv()}) that match.");
                    }
                    else
                    {
                        ScanHelper.UpgradeFile(newFile, pep, existingFile, MDoc, returnActions);
                    }
                }
                else
                {
                    LOGGER.Warn(
                        $"Keeping {newFile.FullName} as it is better quality than some of the current files for that show (Auto Replace with better quality files is turned off)");
                }
                return false;

            case FileHelper.VideoComparison.cantTell:
            case FileHelper.VideoComparison.similar:
                if (unattended)
                {
                    LOGGER.Info(
                        $"Keeping {newFile.FullName} as it might be better quality than {existingFile.FullName}");

                    return false;
                }
                else
                {
                    if (matchingShows.Count <= 1)
                    {
                        return ScanHelper.AskUserAboutFileReplacement(newFile, existingFile, pep, owner, MDoc, returnActions);
                    }

                    LOGGER.Warn(
                        $"Keeping {newFile.FullName}. Although it is better quality than {existingFile.FullName}, there are other shows ({matchingShows.Select(item => item.ShowName).ToCsv()}) that match.");

                    return false;
                }

            //the other cases of the files being the same or the existing file being better are not enough to save the file
            case FileHelper.VideoComparison.firstFileBetter:
                break;

            case FileHelper.VideoComparison.same:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
        return null;
    }

    private static bool IsNotClearCut(FileHelper.VideoComparison result)
    {
        return result switch
        {
            FileHelper.VideoComparison.cantTell => true,
            FileHelper.VideoComparison.same => true,
            FileHelper.VideoComparison.similar => true,
            FileHelper.VideoComparison.firstFileBetter => false,
            FileHelper.VideoComparison.secondFileBetter => false,
            _ => throw new ArgumentOutOfRangeException(nameof(result), result, null)
        };
    }

    private void CopyFutureDatedFile(FileInfo fi, ProcessedEpisode pep, TVDoc d)
    {
        ShowConfiguration si = pep.Show;
        int seasF = pep.AppropriateSeasonNumber;
        int epF = pep.AppropriateEpNum;

        //This episode may be a future dated one - process it now if the settings request that we do and it wont be picked up in a full scan
        if (!TVSettings.Instance.CopyFutureDatedEpsFromSearchFolders || si.ForceCheckFuture || !si.DoMissingCheck ||
            !TVSettings.Instance.MissingCheck || TVSettings.Instance.PreventMove)
        {
            return;
        }

        if (!pep.IsInFuture(true))
        {
            return;
        }

        Dictionary<int, SafeList<string>> foldersLocations = si.AllProposedFolderLocations();
        if (!foldersLocations.ContainsKey(seasF))
        {
            LOGGER.Info(
                $"Identified that {fi.FullName} matches S{seasF}E{epF} of show {si.ShowName}, but can't tell where to copy it. Not copying across.");
            return;
        }

        LOGGER.Info(
            $"Identified that {fi.FullName} matches S{seasF}E{epF} of show {si.ShowName}, that it's not already present and airs in the future. Copying across.");

        SafeList<string> folders = si.AllProposedFolderLocations()[seasF];

        foreach (string folder in folders)
        {
            if (!Directory.Exists(folder))
            {
                if (TVSettings.Instance.AutoCreateFolders || MDoc.Args.MissingFolder == CommandLineArgs.MissingFolderBehavior.create)
                {
                    LOGGER.Info($"Want to copy {fi.FullName} to {folder}, but it doesn't exist yet, so creating it");
                    Directory.CreateDirectory(folder);
                }
                else
                {
                    LOGGER.Warn($"Want to copy {fi.FullName} to {folder}, but it doesn't exist yet");
                    continue;
                }
            }
            FileInfo targetFile = FinderHelper.GenerateTargetName(folder, pep, fi);

            if (fi.FullName == targetFile.FullName)
            {
                continue;
            }

            returnActions.Add(new ActionCopyMoveRename(fi, targetFile, pep, d));

            // if we're copying/moving a file across, we might also want to make a thumbnail or NFO for it
            returnActions.AddNullableRange(new DownloadIdentifiersController().ProcessEpisode(pep, targetFile));
        }
    }
}
