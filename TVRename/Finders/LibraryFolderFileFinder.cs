using System;
using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    internal class LibraryFolderFileFinder : FileFinder
    {
        public LibraryFolderFileFinder(TVDoc i) : base(i) { }

        public override bool Active() => TVSettings.Instance.RenameCheck && TVSettings.Instance.MissingCheck && TVSettings.Instance.MoveLibraryFiles;
        protected override string Checkname() => "Looked in the library for the missing files";


        protected override void Check(SetProgressDelegate prog, ICollection<ShowItem> showList, TVDoc.ScanSettings settings)
        {
            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();
            DirFilesCache dfc = new DirFilesCache();

            int currentItem = 0;
            int totalN = ActionList.MissingItems().Count() + 1;
            UpdateStatus(currentItem, totalN, "Starting searching through library looking for files");

            LOGGER.Info("Starting to look for missing items in the library");

            if (ActionList is null) return;

            foreach (ItemMissing me in ActionList.MissingItems())
            {
                if (settings.Token.IsCancellationRequested)
                    return;

                try
                {
                    if (me is null) return;

                    UpdateStatus(currentItem++, totalN, me.Filename);

                    ItemList thisRound = new ItemList();

                    if (me.Episode?.Show == null)
                    {
                        LOGGER.Info($"Not looking for {me.Filename} in the library as the show/episode is null");
                        continue;
                    }

                    string baseFolder = me.Episode.Show.AutoAddFolderBase;
                    LOGGER.Info($"Starting to look for {me.Filename} in the library: {baseFolder}");
                    
                    List<FileInfo> matchedFiles;

                    if (string.IsNullOrWhiteSpace(baseFolder))
                    {
                        matchedFiles = new List<FileInfo>();
                    }
                    else
                    {
                        FileInfo[] testFiles = dfc.GetFilesIncludeSubDirs(baseFolder);
                        matchedFiles = testFiles is null
                            ? new List<FileInfo>()
                            : testFiles.Where(testFile => ReviewFile(me, thisRound, testFile, settings, false, false, false)).ToList();
                    }

                    foreach (KeyValuePair<int, List<string>> seriesFolders in me.Episode.Show.AllFolderLocationsEpCheck(false))
                    {
                        if (seriesFolders.Value == null) continue;
                        if (me.Episode.AppropriateSeason.SeasonNumber != seriesFolders.Key) continue;

                        foreach (string folderName in seriesFolders.Value)
                        {
                            if (string.IsNullOrWhiteSpace(folderName)) continue; //No point looking here
                            if (folderName == baseFolder) continue; //Already looked here

                            LOGGER.Info($"Starting to look for {me.Filename} in the library folder: {folderName}");
                            FileInfo[] files = dfc.GetFiles(folderName);
                            if (files is null) continue;

                            foreach (FileInfo testFile in files)
                            {
                                if (!ReviewFile(me, thisRound, testFile, settings, false, false, false)) continue;

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
                catch (NullReferenceException nre)
                {
                    LOGGER.Error(nre,
                        $"NullReferenceException fired in LibraryFolderFileFinder.Check whilst looking for {me.Filename}");
                }
            }

            try
            {
                if (TVSettings.Instance.KeepTogether)
                    KeepTogether(newList, true);

                ReorganiseToLeaveOriginals(newList);

                ActionList.Replace(toRemove, newList);
            }
            catch (NullReferenceException nre)
            {
                LOGGER.Error(nre, "NullReferenceException fired in LibraryFolderFileFinder.Check whilst tidying up.");
            }
        }
    }
}
