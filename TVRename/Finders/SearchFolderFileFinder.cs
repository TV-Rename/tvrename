using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    internal class SearchFolderFileFinder:FileFinder
    {
        public SearchFolderFileFinder(TVDoc i) : base(i) { }

        public override bool Active() => TVSettings.Instance.SearchLocally;

        protected override void Check(SetProgressDelegate prog, ICollection<ShowItem> showList,
            TVDoc.ScanSettings settings)
        {
            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();

            int fileCount = CountFilesInDownloadDirs();

            DirCache dirCache = new DirCache();
            foreach (string s in TVSettings.Instance.DownloadFolders)
            {
                if (settings.Token.IsCancellationRequested)
                    return;

                dirCache.AddFolder(prog, 0, fileCount, s, true);
            }

            int currentItem = 0;
            int totalN = ActionList.Count + 1;
            UpdateStatus(currentItem, totalN, "Starting searching through files");

            foreach (ItemMissing me in ActionList.MissingItems())
            {
                if (settings.Token.IsCancellationRequested)
                    return;

                UpdateStatus(currentItem++, totalN, me.Filename);

                ItemList thisRound = new ItemList();
                List<FileInfo> matchedFiles = FindMatchedFiles(settings, dirCache, me, thisRound);

                ProcessMissingItem(settings, newList, toRemove, me, thisRound, matchedFiles);
            }

            if (TVSettings.Instance.KeepTogether)
            {
                KeepTogether(newList, false);
            }

            if (!TVSettings.Instance.LeaveOriginals)
            {
                ReorganiseToLeaveOriginals(newList);
            }

            foreach (Item i in toRemove)
            {

            ActionList.Remove(i);
            }

            foreach (Item i in newList)
            {
                ActionList.Add(i);
            }
        }

        private List<FileInfo> FindMatchedFiles(TVDoc.ScanSettings settings, DirCache dirCache, ItemMissing me, ItemList thisRound)
        {
            List<FileInfo> matchedFiles = new List<FileInfo>();

            foreach (DirCacheEntry dce in dirCache)
            {
                if (!ReviewFile(me, thisRound, dce.TheFile, settings, TVSettings.Instance.AutoMergeDownloadEpisodes, TVSettings.Instance.PreventMove,true)) continue;

                matchedFiles.Add(dce.TheFile);
            }

            return matchedFiles;
        }

        private static int CountFilesInDownloadDirs()
        {
            int fileCount = 0;
            foreach (string s in TVSettings.Instance.DownloadFolders)
                fileCount += DirCache.CountFiles(s, true);
            return fileCount;
        }
    }
}
