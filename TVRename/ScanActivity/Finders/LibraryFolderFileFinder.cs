using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;

namespace TVRename
{
    internal class LibraryFolderFileFinder : FileFinder
    {
        public LibraryFolderFileFinder(TVDoc i) : base(i)
        {
        }

        public override bool Active() => TVSettings.Instance.RenameCheck && TVSettings.Instance.MissingCheck && TVSettings.Instance.MoveLibraryFiles;

        protected override string CheckName() => "Looked in the library for the missing files";

        protected override void DoCheck(SetProgressDelegate prog, TVDoc.ScanSettings settings)
        {
            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();
            DirFilesCache dfc = new DirFilesCache();

            int currentItem = 0;
            int totalN = ActionList.Missing.Count + 1;
            UpdateStatus(currentItem, totalN, "Starting searching through library looking for files");

            LOGGER.Info("Starting to look for missing items in the library");

            foreach (ItemMissing? me in ActionList.Missing.ToList())
            {
                if (settings.Token.IsCancellationRequested)
                {
                    return;
                }

                UpdateStatus(currentItem++, totalN, me.Filename);

                if (me is ShowItemMissing sim)
                {
                    if (me.Episode?.Show is null)
                    {
                        LOGGER.Info($"Not looking for {me.Filename} in the library as the show/episode is null");
                        continue;
                    }
                    FindEpisode(settings, sim, dfc, newList, toRemove);
                }
                else if (me is MovieItemMissing mim)
                {
                    FindMovie(settings, mim, dfc, newList, toRemove);
                }
            }

            if (TVSettings.Instance.KeepTogether)
            {
                KeepTogether(newList, true);
            }

            if (!TVSettings.Instance.LeaveOriginals)
            {
                ReorganiseToLeaveOriginals(newList);
            }

            ActionList.Replace(toRemove, newList);
        }

        private void FindMovie(TVDoc.ScanSettings settings, MovieItemMissing mim, DirFilesCache dfc, ItemList newList, ItemList toRemove)
        {
            if (!mim.MovieConfig.UseAutomaticFolders)
            {
                return;
            }
            (string targetFolder, string targetFolderEarlier, string targetFolderLater) = mim.MovieConfig.NeighbouringFolderNames();

            TestShouldMove(targetFolderEarlier, targetFolder, dfc, newList, toRemove, mim);
            TestShouldMove(targetFolderLater, targetFolder, dfc, newList, toRemove, mim);
        }

        private void TestShouldMove(string sourceFolder, string targetFolder, DirFilesCache dfc, ItemList newList, ItemList toRemove, MovieItemMissing mim)
        {
            if (sourceFolder == targetFolder)
            {
                return;
            }

            if (dfc.GetFiles(targetFolder).Length > 0)
            {
                //do not want to copy any files over new location
                return;
            }

            if (!Directory.Exists(sourceFolder) || dfc.GetFiles(sourceFolder).Length == 0)
            {
                return;
            }
            LOGGER.Info($"Have identified that {sourceFolder} can be copied to {targetFolder}");

            toRemove.Add(mim);
            newList.Add(new ActionMoveRenameDirectory(sourceFolder, targetFolder, mim.MovieConfig));
        }

        private void FindEpisode(TVDoc.ScanSettings settings, ShowItemMissing me, DirFilesCache dfc, ItemList newList,
            ItemList toRemove)
        {
            ItemList thisRound = new ItemList();

            string baseFolder = me.Episode.Show.AutoAddFolderBase;
            LOGGER.Info($"Starting to look for {me.Filename} in the library: {baseFolder}");

            List<FileInfo> matchedFiles = GetMatchingFilesFromFolder(baseFolder, dfc, me, settings, thisRound);

            foreach (string folderName in me.Episode.Show.AllFolderLocationsEpCheck(false)
                .Where(folders => folders.Value != null)
                .Where(folders => folders.Key == me.Episode.AppropriateProcessedSeason.SeasonNumber)
                .SelectMany(seriesFolders => seriesFolders.Value
                    .Where(f => !string.IsNullOrWhiteSpace(f)) //No point looking here
                    .Where(f => f != baseFolder)))
            {
                ProcessFolder(settings, me, folderName, dfc, thisRound, matchedFiles);
            }

            ProcessMissingItem(settings, newList, toRemove, me, thisRound, matchedFiles,
                TVSettings.Instance.UseFullPathNameToMatchLibraryFolders);
        }

        [NotNull]
        private List<FileInfo> GetMatchingFilesFromFolder(string? baseFolder, DirFilesCache dfc, ShowItemMissing me, TVDoc.ScanSettings settings,
            ItemList thisRound)
        {
            List<FileInfo> matchedFiles;

            if (string.IsNullOrWhiteSpace(baseFolder))
            {
                matchedFiles = new List<FileInfo>();
            }
            else
            {
                IEnumerable<FileInfo> testFiles = dfc.GetFilesIncludeSubDirs(baseFolder);
                matchedFiles = testFiles.Where(testFile => ReviewFile(me, thisRound, testFile, settings, false, false, false,
                    TVSettings.Instance.UseFullPathNameToMatchLibraryFolders)).ToList();
            }

            return matchedFiles;
        }

        private void ProcessFolder(TVDoc.ScanSettings settings, [NotNull] ShowItemMissing me, [NotNull] string folderName, [NotNull] DirFilesCache dfc,
            ItemList thisRound, [NotNull] List<FileInfo> matchedFiles)
        {
            LOGGER.Info($"Starting to look for {me.Filename} in the library folder: {folderName}");
            FileInfo[] files = dfc.GetFiles(folderName);

            foreach (FileInfo testFile in files)
            {
                if (!ReviewFile(me, thisRound, testFile, settings, false, false, false,
                    TVSettings.Instance.UseFullPathNameToMatchLibraryFolders))
                {
                    continue;
                }

                if (matchedFiles.Contains(testFile))
                {
                    continue;
                }

                matchedFiles.Add(testFile);
                LOGGER.Info($"Found {me.Filename} at: {testFile}");
            }
        }
    }
}
