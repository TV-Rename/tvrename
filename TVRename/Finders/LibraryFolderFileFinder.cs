using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    internal class LibraryFolderFileFinder : FileFinder
    {
        public LibraryFolderFileFinder(TVDoc i) : base(i) { }

        public override bool Active() => TVSettings.Instance.RenameCheck && TVSettings.Instance.MissingCheck && TVSettings.Instance.MoveLibraryFiles; 

        protected override void Check(SetProgressDelegate prog, ICollection<ShowItem> showList, TVDoc.ScanSettings settings)
        {
            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();
            DirFilesCache dfc = new DirFilesCache();

            int currentItem = 0;
            int totalN = ActionList.MissingItems().Count() + 1;
            UpdateStatus(currentItem, totalN, "Starting searching through library looking for files");

            LOGGER.Info("Starting to look for missing items in the library");

            foreach (ItemMissing me in ActionList.MissingItems())
            {
                if (settings.Token.IsCancellationRequested)
                    return;

                UpdateStatus(currentItem++, totalN, me.Filename);

                ItemList thisRound = new ItemList();

                if (me.Episode?.Show == null)
                {
                    LOGGER.Info($"Not looking for {me.Filename} in the library as the show/episode is null");
                    continue;
                }

                string baseFolder = me.Episode.Show.AutoAddFolderBase;
                LOGGER.Info($"Starting to look for {me.Filename} in the library: {baseFolder}");

                List<FileInfo> matchedFiles = dfc.GetFilesIncludeSubDirs(baseFolder).Where(testFile => ReviewFile(me, thisRound, testFile, settings,false,false,false)).ToList();

                foreach (KeyValuePair<int, List<string>> seriesFolders in me.Episode.Show.AllFolderLocationsEpCheck(false))
                {
                    foreach (string folderName in seriesFolders.Value)
                    {
                        LOGGER.Info($"Starting to look for {me.Filename} in the library folder: {folderName}");
                        foreach (FileInfo testFile in dfc.GetFilesIncludeSubDirs(folderName))
                         {
                            if (!ReviewFile(me, thisRound, testFile, settings,false,false,false)) continue;

                            if (!matchedFiles.Contains(testFile))
                            {
                                matchedFiles.Add(testFile);
                                LOGGER.Info($"Found {me.Filename} at: {testFile}");
                            }
                        }
                    }
                }

                ProcessMissingItem(settings, newList, toRemove, me, thisRound, matchedFiles);
            }

            if (TVSettings.Instance.KeepTogether)
                KeepTogether(newList,true);

            ReorganiseToLeaveOriginals(newList);

            ActionList.Replace(toRemove,newList);
        }
    }
}
