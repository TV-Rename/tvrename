using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    internal class SearchFolderFileFinder:FileFinder
    {
        public SearchFolderFileFinder(TVDoc i) : base(i) { }

        public override bool Active() => TVSettings.Instance.SearchLocally;
        [NotNull]
        protected override string CheckName() => "Looked in the search folders for the missing files";

        protected override void DoCheck(SetProgressDelegate prog, ICollection<ShowItem> showList,
            TVDoc.ScanSettings settings)
        {
            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();

            int fileCount = CountFilesInDownloadDirs();

            DirCache dirCache = new DirCache();
            foreach (string s in TVSettings.Instance.DownloadFolders.ToList())
            {
                if (settings.Token.IsCancellationRequested)
                {
                    return;
                }

                dirCache.AddFolder(prog, 0, fileCount, s, true);
            }

            int currentItem = 0;
            int totalN = ActionList.Missing.Count + 1;
            UpdateStatus(currentItem, totalN, "Starting searching through files");

            foreach (ItemMissing me in ActionList.Missing.ToList())
            {
                if (settings.Token.IsCancellationRequested)
                {
                    return;
                }

                UpdateStatus(currentItem++, totalN, me.Filename);

                ItemList thisRound = new ItemList();
                List<FileInfo> matchedFiles = FindMatchedFiles(settings, dirCache, me, thisRound);

                ProcessMissingItem(settings, newList, toRemove, me, thisRound, matchedFiles,TVSettings.Instance.UseFullPathNameToMatchSearchFolders);
            }

            if (TVSettings.Instance.KeepTogether)
            {
                KeepTogether(newList, false);
            }

            if (!TVSettings.Instance.LeaveOriginals)
            {
                ReorganiseToLeaveOriginals(newList);
            }

            ActionList.Replace(toRemove, newList);
        }

        [NotNull]
        private List<FileInfo> FindMatchedFiles(TVDoc.ScanSettings settings, [NotNull] DirCache dirCache, ItemMissing me, ItemList thisRound)
        {
            List<FileInfo> matchedFiles = new List<FileInfo>();

            foreach (DirCacheEntry dce in dirCache)
            {
                if (!ReviewFile(me, thisRound, dce.TheFile, settings, TVSettings.Instance.AutoMergeDownloadEpisodes, TVSettings.Instance.PreventMove,true, TVSettings.Instance.UseFullPathNameToMatchSearchFolders))
                {
                    continue;
                }

                matchedFiles.Add(dce.TheFile);
            }

            return matchedFiles;
        }

        private static int CountFilesInDownloadDirs()
        {
            return TVSettings.Instance.DownloadFolders.ToArray().Sum(s => DirCache.CountFiles(s, true));
        }
    }
}
