using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    internal class LibraryFolderFileFinder : FileFinder
    {
        public LibraryFolderFileFinder(TVDoc i) : base(i) { }

        public override bool Active() => TVSettings.Instance.RenameCheck && TVSettings.Instance.MissingCheck && TVSettings.Instance.MoveLibraryFiles;
        protected override string CheckName() => "Looked in the library for the missing files";

        protected override void DoCheck(SetProgressDelegate prog, ICollection<ShowItem> showList, TVDoc.ScanSettings settings)
        {
            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();
            DirFilesCache dfc = new DirFilesCache();

            int currentItem = 0;
            int totalN = ActionList.Missing.Count + 1;
            UpdateStatus(currentItem, totalN, "Starting searching through library looking for files");

            LOGGER.Info("Starting to look for missing items in the library");

            foreach (ItemMissing me in ActionList.Missing.ToList())
            {
                if (settings.Token.IsCancellationRequested)
                {
                    return;
                }

                UpdateStatus(currentItem++, totalN, me.Filename);

                ItemList thisRound = new ItemList();

                if (me.Episode?.Show is null)
                {
                    LOGGER.Info($"Not looking for {me.Filename} in the library as the show/episode is null");
                    continue;
                }

                string baseFolder = me.Episode.Show.AutoAddFolderBase;
                LOGGER.Info($"Starting to look for {me.Filename} in the library: {baseFolder}");
                
                List<FileInfo> matchedFiles = GetMatchingFilesFromFolder(baseFolder, dfc, me, settings, thisRound);

                foreach (string folderName in me.Episode.Show.AllFolderLocationsEpCheck(false)
                    .Where(folders => folders.Value!=null)
                    .Where(folders => folders.Key==me.Episode.AppropriateProcessedSeason.SeasonNumber)
                    .SelectMany(seriesFolders => seriesFolders.Value
                        .Where(f => !string.IsNullOrWhiteSpace(f)) //No point looking here
                        .Where(f=> f!=baseFolder)))
                {
                    ProcessFolder(settings, me, folderName, dfc, thisRound, matchedFiles);
                }

                ProcessMissingItem(settings, newList, toRemove, me, thisRound, matchedFiles,TVSettings.Instance.UseFullPathNameToMatchLibraryFolders);
            }

            if (TVSettings.Instance.KeepTogether)
            {
                KeepTogether(newList, true);
            }

            ReorganiseToLeaveOriginals(newList);

            ActionList.Replace(toRemove, newList);
        }

        [NotNull]
        private List<FileInfo> GetMatchingFilesFromFolder(string? baseFolder, DirFilesCache dfc, ItemMissing me, TVDoc.ScanSettings settings,
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

        private void ProcessFolder(TVDoc.ScanSettings settings, [NotNull] ItemMissing me, [NotNull] string folderName, [NotNull] DirFilesCache dfc,
            ItemList thisRound, [NotNull] List<FileInfo>  matchedFiles)
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
