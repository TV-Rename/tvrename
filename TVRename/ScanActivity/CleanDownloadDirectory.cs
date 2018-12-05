using System;
using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    internal class CleanDownloadDirectory:ScanActivity
    {
        // ReSharper disable once InconsistentNaming
        private IEnumerable<Action> Go(ICollection<ShowItem> showList, bool unattended)
        {
            //for each directory in settings directory
            //for each file in directory
            //for each saved show (order by recent)
            //is file already available? 
            //if so add show to list of files to be removed

            DirFilesCache dfc = new DirFilesCache();
            List<Action> returnActions =new List<Action>();

            int totalDownloadFolders = TVSettings.Instance.DownloadFolders.Count;
            int c = 1;

            foreach (string dirPath in TVSettings.Instance.DownloadFolders)
            {
                UpdateStatus(c++ , totalDownloadFolders,dirPath);

                if (!Directory.Exists(dirPath)) continue;

                List<FileInfo> filesThatMayBeNeeded = new List<FileInfo>();
                returnActions.AddNullableRange(ReviewFilesInDownloadDirectory(showList, unattended, dfc, dirPath, filesThatMayBeNeeded));

                returnActions.AddNullableRange(ReviewDirsInDownloadDirectory(showList, dfc, dirPath, filesThatMayBeNeeded));
            }

            return returnActions;
        }

        private static IEnumerable<Action> ReviewDirsInDownloadDirectory(ICollection<ShowItem> showList, DirFilesCache dfc, string dirPath, List<FileInfo> filesThatMayBeNeeded)
        {
            try
            {
                foreach (string subDirPath in Directory.GetDirectories(dirPath, "*",
                System.IO.SearchOption.AllDirectories))
                {
                    if (!Directory.Exists(subDirPath)) continue;

                    return ReviewDirInDownloadDirectory(showList, dfc, filesThatMayBeNeeded, subDirPath);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                LOGGER.Warn(ex, $"Could not access subdirectories of {dirPath}");
            }

            return null;
        }

        private static IEnumerable<Action> ReviewDirInDownloadDirectory(ICollection<ShowItem> showList, DirFilesCache dfc, List<FileInfo> filesThatMayBeNeeded, string subDirPath)
        {
            List<Action> returnActions = new List<Action>();

            //we are not checking for any file updates, so can return
            if (!TVSettings.Instance.RemoveDownloadDirectoriesFiles) return null;

            if (!Directory.Exists(subDirPath)) return null;

            DirectoryInfo di = new DirectoryInfo(subDirPath);

            List<ShowItem> matchingShows = showList.Where(si => si.NameMatch(di)).ToList();

            if (matchingShows.Any())
            {
                bool dirCanBeRemoved = TVSettings.Instance.RemoveDownloadDirectoriesFiles;

                foreach (ShowItem si in matchingShows)
                {
                    if (FinderHelper.FileNeeded(di, si, dfc))
                    {
                        LOGGER.Info($"Not removing {di.FullName} as it may be needed for {si.ShowName}");
                        dirCanBeRemoved = false;
                    }
                }

                foreach (FileInfo neededFile in filesThatMayBeNeeded)
                {
                    if (di.FullName.Contains(neededFile.DirectoryName))
                    {
                        dirCanBeRemoved = false;
                    }
                }

                if (dirCanBeRemoved)
                {
                    returnActions.Add(SetupDirectoryRemoval(di, matchingShows));
                }
            }

            return returnActions;
        }

        private static Action SetupDirectoryRemoval(DirectoryInfo di, List<ShowItem> matchingShows)
        {
            ShowItem si = matchingShows[0]; //Choose the first series
            FinderHelper.FindSeasEp(di, out int seasF, out int epF, si, out TVSettings.FilenameProcessorRE _);
            SeriesInfo s = si.TheSeries();
            Episode ep = s.GetEpisode(seasF, epF, si.DvdOrder);
            ProcessedEpisode pep = new ProcessedEpisode(ep, si);
            LOGGER.Info(
                $"Removing {di.FullName} as it matches {matchingShows[0].ShowName} and no episodes are needed");

            return new ActionDeleteDirectory(di, pep, TVSettings.Instance.Tidyup);
        }

        private static IEnumerable<Action> ReviewFilesInDownloadDirectory(ICollection<ShowItem> showList, bool unattended, DirFilesCache dfc, string dirPath, List<FileInfo> filesThatMayBeNeeded)
        {
            List<Action> returnActions = new List<Action>();
            try
            {
                foreach (string filePath in Directory.GetFiles(dirPath, "*", System.IO.SearchOption.AllDirectories))
                {
                    if (!File.Exists(filePath)) continue;

                    FileInfo fi = new FileInfo(filePath);

                    if (fi.IgnoreFile()) continue;

                    List<ShowItem> matchingShows = showList.Where(si => si.NameMatch(fi)).ToList();

                    if (matchingShows.Any())
                    {
                        returnActions.AddNullableRange(ReviewFileInDownloadDirectory(unattended, dfc, filesThatMayBeNeeded, fi,
                            matchingShows));
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                LOGGER.Warn(ex, $"Could not access files in {dirPath}");
            }

            return returnActions;
        }

        private static List<Action> ReviewFileInDownloadDirectory(bool unattended, DirFilesCache dfc, ICollection<FileInfo> filesThatMayBeNeeded, FileInfo fi, List<ShowItem> matchingShows)
        {
            bool fileCanBeDeleted = TVSettings.Instance.RemoveDownloadDirectoriesFiles;
            List<Action> returnActions = new List<Action>();
            ProcessedEpisode firstMatchingPep = null;

            foreach (ShowItem si in matchingShows)
            {
                FinderHelper.FindSeasEp(fi, out int seasF, out int epF, out int _, si,
                        out TVSettings.FilenameProcessorRE re);

                SeriesInfo s = si.TheSeries();
                try
                {
                    Episode ep = s.GetEpisode(seasF, epF, si.DvdOrder);
                    ProcessedEpisode pep = new ProcessedEpisode(ep, si);
                    firstMatchingPep = pep;
                    List<FileInfo> encumbants = dfc.FindEpOnDisk(pep, false);

                    if (encumbants.Count == 0)
                    {
                        //File is needed as there are no files for that series/episode
                        fileCanBeDeleted = false;
                    }
                    else
                    {
                        foreach (FileInfo existingFile in encumbants)
                        {
                            if (existingFile.FullName.Equals(fi.FullName,StringComparison.InvariantCultureIgnoreCase))
                            {
                                //the user has put the search folder and the download folder in the same place - DO NOT DELETE
                                fileCanBeDeleted = false;
                                continue;
                            }

                            FileHelper.VideoComparison result = FileHelper.BetterQualityFile(existingFile, fi);
                            switch (result)
                            {
                                case FileHelper.VideoComparison.SecondFileBetter:
                                    fileCanBeDeleted = false;

                                    if (TVSettings.Instance.ReplaceWithBetterQuality)
                                    {
                                        if (matchingShows.Count > 1)
                                        {
                                            LOGGER.Warn(
                                                $"Keeping {fi.FullName}. Although it is better quality than {existingFile.FullName}, there are other shows ({string.Join(", ", matchingShows.Select(item => item.ShowName))}) that match.");
                                        }
                                        else
                                        {
                                            returnActions.AddNullableRange(UpgradeFile(fi, pep, existingFile));
                                        }
                                    }
                                    else
                                    {
                                        LOGGER.Warn(
                                            $"Keeping {fi.FullName} as it is better quality than some of the current files for that show (Auto Replace with better quality files is turned off)");
                                    }

                                    break;
                                case FileHelper.VideoComparison.CantTell:
                                case FileHelper.VideoComparison.Similar:
                                    if (unattended)
                                    {
                                        fileCanBeDeleted = false;
                                        LOGGER.Info(
                                            $"Keeping {fi.FullName} as it might be better quality than {existingFile.FullName}");
                                    }
                                    else
                                    {
                                        if (matchingShows.Count > 1)
                                        {
                                            LOGGER.Warn(
                                                $"Keeping {fi.FullName}. Although it is better quality than {existingFile.FullName}, there are other shows ({string.Join(", ", matchingShows.Select(item => item.ShowName))}) that match.");
                                            fileCanBeDeleted = false;
                                        }
                                        else
                                        {
                                            ChooseFile question = new ChooseFile(existingFile, fi);
                                            question.ShowDialog();

                                            switch (question.Answer)
                                            {
                                                case ChooseFile.ChooseFileDialogResult.ignore:
                                                    fileCanBeDeleted = false;
                                                    LOGGER.Info(
                                                        $"Keeping {fi.FullName} as it might be better quality than {existingFile.FullName}");

                                                    break;
                                                case ChooseFile.ChooseFileDialogResult.left:
                                                    LOGGER.Info(
                                                        $"User has elected to remove {fi.FullName} as it is not as good quality than {existingFile.FullName}");

                                                    break;
                                                case ChooseFile.ChooseFileDialogResult.right:
                                                    returnActions.AddNullableRange(UpgradeFile(fi, pep, existingFile));
                                                    fileCanBeDeleted = false;
                                                    break;
                                                default:
                                                    throw new ArgumentOutOfRangeException();
                                            }
                                        }
                                    }

                                    break;
                                //the other cases of the files being the same or the existing file being better are not enough to save the file
                                case FileHelper.VideoComparison.FirstFileBetter:
                                    break;
                                case FileHelper.VideoComparison.Same:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }
                }
                catch (SeriesInfo.EpisodeNotFoundException)
                {
                    LOGGER.Info($"Can't find the right episode for {fi.FullName} coming out as S{seasF}E{epF} using rule '{re?.Notes}'");
                    fileCanBeDeleted = false;
                }
            }

            if (fileCanBeDeleted)
            {
                LOGGER.Info(
                    $"Removing {fi.FullName} as it matches {string.Join(", ", matchingShows.Select(s => s.ShowName))} and no episodes are needed");

                returnActions.Add(new ActionDeleteFile(fi, firstMatchingPep, TVSettings.Instance.Tidyup));
            }
            else
            {
                filesThatMayBeNeeded.Add(fi);
            }

            return returnActions;
        }

        private static IEnumerable<Action> UpgradeFile(FileInfo fi, ProcessedEpisode pep, FileInfo existingFile)
        {
            List<Action> returnActions = new List<Action>();

            if (existingFile.Extension != fi.Extension)
            {
                returnActions.Add(new ActionDeleteFile(existingFile, pep, null));
                returnActions.Add(new ActionCopyMoveRename(fi, existingFile.WithExtension(fi.Extension), pep));
            }
            else
            {
                returnActions.Add(new ActionCopyMoveRename(fi, existingFile, pep));
            }

            LOGGER.Info(
                $"Using {fi.FullName} to replace {existingFile.FullName} as it is better quality");

            return returnActions;
        }

        public CleanDownloadDirectory(TVDoc doc) : base(doc)
        {
        }

        protected override void Check(SetProgressDelegate prog, ICollection<ShowItem> showList, TVDoc.ScanSettings settings)
        {
            MDoc.TheActionList.AddNullableRange(Go(MDoc.Library.GetShowItems(),settings.Unattended));
        }

        public override bool Active() => TVSettings.Instance.RemoveDownloadDirectoriesFiles ||
                                         TVSettings.Instance.ReplaceWithBetterQuality;
    }
}
