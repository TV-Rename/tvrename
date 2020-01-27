using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    internal class CleanDownloadDirectory:ScanActivity
    {
        public CleanDownloadDirectory(TVDoc doc) : base(doc)
        {
        }

        private List<FileInfo> filesThatMayBeNeeded;
        private readonly DirFilesCache dfc = new DirFilesCache();
        private ICollection<ShowItem> showList;
        private TVDoc.ScanSettings currentSettings;
        private ItemList returnActions;

        public override bool Active() => TVSettings.Instance.RemoveDownloadDirectoriesFiles ||
                                         TVSettings.Instance.ReplaceWithBetterQuality ||
                                         TVSettings.Instance.CopyFutureDatedEpsFromSearchFolders;
        [NotNull]
        protected override string Checkname() => "Cleaned up and files in download directory that are not needed";

        protected override void DoCheck(SetProgressDelegate prog, ICollection<ShowItem> shows, TVDoc.ScanSettings settings)
        {
            returnActions = new ItemList();
            showList = MDoc.Library.GetShowItems(); //We ignore the current set of shows being scanned to be secrure that no files are deleted for unscanned shows
            currentSettings = settings;

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

                if (!Directory.Exists(dirPath) || currentSettings.Token.IsCancellationRequested)
                {
                    continue;
                }

                filesThatMayBeNeeded = new List<FileInfo>();

                ReviewFilesInDownloadDirectory(dirPath);
                ReviewDirsInDownloadDirectory(dirPath);
            }

            ItemList removeActions = new ItemList();
            //Remove any missing items we are planning to resolve
            foreach (ActionCopyMoveRename acmr in returnActions.OfType<ActionCopyMoveRename>())
            {
                foreach (ItemMissing missingItem in MDoc.TheActionList.MissingItems())
                {
                    if (missingItem.Episode == acmr.Episode)
                    {
                        removeActions.Add(missingItem);
                    }
                }
            }

            MDoc.TheActionList.Replace(removeActions,returnActions);
        }

        private void ReviewDirsInDownloadDirectory(string dirPath)
        {
            try
            {
                foreach (string subDirPath in Directory.GetDirectories(dirPath, "*",
                    System.IO.SearchOption.AllDirectories).Where(Directory.Exists))
                {
                    if (currentSettings.Token.IsCancellationRequested)
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
            if (!TVSettings.Instance.RemoveDownloadDirectoriesFiles)
            {
                return;
            }

            if (!Directory.Exists(subDirPath))
            {
                return;
            }

            DirectoryInfo di = new DirectoryInfo(subDirPath);

            FileInfo neededFile = filesThatMayBeNeeded.FirstOrDefault(info => info.DirectoryName.Contains(di.FullName));
            if (neededFile != null)
            {
                LOGGER.Info($"Not removing {di.FullName} as it may be needed for {neededFile.FullName}");
                return;
            }

            List<ShowItem> matchingShows = showList.Where(si => si.NameMatch(di,TVSettings.Instance.UseFullPathNameToMatchSearchFolders)).ToList();
            if (!matchingShows.Any())
            {
                return;
            }

            ShowItem firstMatchingShow = matchingShows.FirstOrDefault(si => FinderHelper.FileNeeded(di, si, dfc));
            if (firstMatchingShow != null)
            {
                LOGGER.Info($"Not removing {di.FullName} as it may be needed for {firstMatchingShow.ShowName}");
                return;
            }

            returnActions.Add(SetupDirectoryRemoval(di, matchingShows));
        }

        [NotNull]
        private static Action SetupDirectoryRemoval([NotNull] DirectoryInfo di, [NotNull] IReadOnlyList<ShowItem> matchingShows)
        {
            ShowItem si = matchingShows[0]; //Choose the first series
            FinderHelper.FindSeasEp(di, out int seasF, out int epF, si, out TVSettings.FilenameProcessorRE _);
            SeriesInfo s = si.TheSeries();
            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            Episode ep = s.GetEpisode(seasF, epF, si.DvdOrder);
            ProcessedEpisode pep = new ProcessedEpisode(ep, si);
            LOGGER.Info(
                $"Removing {di.FullName} as it matches {matchingShows[0].ShowName} and no episodes are needed");

            return new ActionDeleteDirectory(di, pep, TVSettings.Instance.Tidyup);
        }

        private void ReviewFilesInDownloadDirectory(string dirPath)
        {
            try
            {
                foreach (string filePath in Directory.GetFiles(dirPath, "*", System.IO.SearchOption.AllDirectories).Where(File.Exists))
                {
                    if (currentSettings.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    FileInfo fi = new FileInfo(filePath);

                    if (fi.IgnoreFile())
                    {
                        continue;
                    }

                    List<ShowItem> matchingShows = showList.Where(si => si.NameMatch(fi, TVSettings.Instance.UseFullPathNameToMatchSearchFolders)).ToList();

                    if (matchingShows.Any())
                    {
                        ReviewFileInDownloadDirectory(currentSettings.Unattended,  fi,matchingShows);
                    }
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

        private void ReviewFileInDownloadDirectory(bool unattended, FileInfo fi, [NotNull] List<ShowItem> matchingShows)
        {
            bool fileCanBeDeleted = TVSettings.Instance.RemoveDownloadDirectoriesFiles;
            ProcessedEpisode firstMatchingPep = null;

            foreach (ShowItem si in matchingShows)
            {
                FinderHelper.FindSeasEp(fi, out int seasF, out int epF, out int _, si, out TVSettings.FilenameProcessorRE re);

                if (!si.SeasonEpisodes.ContainsKey(seasF))
                {
                    LOGGER.Info($"Can't find the right season for {fi.FullName} coming out as S{seasF}E{epF} using rule '{re?.Notes}'");
                    fileCanBeDeleted = false;
                    continue;
                }

                ProcessedEpisode pep = si.SeasonEpisodes[seasF].FirstOrDefault(ep => ep.AppropriateEpNum == epF);

                if (pep == null)
                {
                    LOGGER.Info($"Can't find the right episode for {fi.FullName} coming out as S{seasF}E{epF} using rule '{re?.Notes}'");
                    fileCanBeDeleted = false;
                    continue;
                }

                firstMatchingPep = pep;
                List<FileInfo> encumbants = dfc.FindEpOnDisk(pep, false);

                if (encumbants.Count == 0)
                {
                    //File is needed as there are no files for that series/episode
                    fileCanBeDeleted = false;

                    CopyFutureDatedFile(fi, pep, MDoc);
                }
                else
                {
                    foreach (FileInfo existingFile in encumbants)
                    {
                        if (existingFile.FullName.Equals(fi.FullName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            //the user has put the search folder and the download folder in the same place - DO NOT DELETE
                            fileCanBeDeleted = false;
                            continue;
                        }

                        bool? deleteFile = ReviewFile(unattended, fi, matchingShows, existingFile, pep);
                        if (deleteFile.HasValue && deleteFile.Value==false)
                        {
                            fileCanBeDeleted = false;
                        }
                    }
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
        }

        private bool? ReviewFile(bool unattended, [NotNull] FileInfo newFile, [NotNull] List<ShowItem> matchingShows, [NotNull] FileInfo existingFile,[NotNull] ProcessedEpisode pep)
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
                                $"Keeping {newFile.FullName}. Although it is better quality than {existingFile.FullName}, there are other shows ({string.Join(", ", matchingShows.Select(item => item.ShowName))}) that match.");
                        }
                        else
                        {
                            UpgradeFile(newFile, pep, existingFile);
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
                            return AskUserAboutFileReplacement(newFile, existingFile, pep);
                        }

                        LOGGER.Warn(
                            $"Keeping {newFile.FullName}. Although it is better quality than {existingFile.FullName}, there are other shows ({string.Join(", ", matchingShows.Select(item => item.ShowName))}) that match.");

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

        private bool IsNotClearCut(FileHelper.VideoComparison result)
        {
            switch (result)
            {
                case FileHelper.VideoComparison.cantTell:
                    return true;
                case FileHelper.VideoComparison.same:
                    return true;
                case FileHelper.VideoComparison.similar:
                    return true;
                case FileHelper.VideoComparison.firstFileBetter:
                    return false;
                case FileHelper.VideoComparison.secondFileBetter:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }

        private bool? AskUserAboutFileReplacement([NotNull] FileInfo newFile, [NotNull] FileInfo existingFile, [NotNull] ProcessedEpisode pep)
        {
            try
            {
                ChooseFile question = new ChooseFile(existingFile, newFile);
                question.ShowDialog();

                switch (question.Answer)
                {
                    case ChooseFile.ChooseFileDialogResult.ignore:
                        LOGGER.Info(
                            $"Keeping {newFile.FullName} as it might be better quality than {existingFile.FullName}");
                        return false;
                    case ChooseFile.ChooseFileDialogResult.left:
                        LOGGER.Info(
                            $"User has elected to remove {newFile.FullName} as it is not as good quality than {existingFile.FullName}");

                        break;
                    case ChooseFile.ChooseFileDialogResult.right:
                        UpgradeFile(newFile, pep, existingFile);
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return null;
            }
            catch (System.IO.FileNotFoundException)
            {
                return false;
            }
        }

        private void CopyFutureDatedFile(FileInfo fi, [NotNull] ProcessedEpisode pep,TVDoc d)
        {
            ShowItem si = pep.Show;
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

            Dictionary<int, List<string>> foldersLocations = si.AllProposedFolderLocations();
            if (!foldersLocations.ContainsKey(seasF))
            {
                LOGGER.Info(
                    $"Identified that {fi.FullName} matches S{seasF}E{epF} of show {si.ShowName}, but can't tell where to copy it. Not copying across.");
                return;
            }

            LOGGER.Info(
                $"Identified that {fi.FullName} matches S{seasF}E{epF} of show {si.ShowName}, that it's not already present and airs in the future. Copying across.");

            List<string> folders = si.AllProposedFolderLocations()[seasF];

            foreach (string folder in folders)
            {
                if (!Directory.Exists(folder))
                {
                    if (TVSettings.Instance.AutoCreateFolders) // Also check || Doc.Args.MissingFolder == CommandLineArgs.MissingFolderBehavior.create
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
                string filename = TVSettings.Instance.FilenameFriendly(TVSettings.Instance.NamingStyle.NameFor(pep,fi.Extension,folder.Length));

                FileInfo targetFile = new FileInfo(folder + Path.DirectorySeparatorChar + filename);

                if (fi.FullName == targetFile.FullName)
                {
                    continue;
                }

                returnActions.Add(new ActionCopyMoveRename(fi, targetFile, pep,d));

                // if we're copying/moving a file across, we might also want to make a thumbnail or NFO for it
                returnActions.AddRange(new DownloadIdentifiersController().ProcessEpisode(pep, targetFile));
            }
        }

        private void UpgradeFile([NotNull] FileInfo fi, ProcessedEpisode pep, [NotNull] FileInfo existingFile)
        {
            if (existingFile.Extension != fi.Extension)
            {
                returnActions.Add(new ActionDeleteFile(existingFile, pep, null));
                returnActions.Add(new ActionCopyMoveRename(fi, existingFile.WithExtension(fi.Extension), pep,MDoc));
            }
            else
            {
                returnActions.Add(new ActionCopyMoveRename(fi, existingFile, pep, MDoc));
            }

            LOGGER.Info(
                $"Using {fi.FullName} to replace {existingFile.FullName} as it is better quality");
        }
    }
}
