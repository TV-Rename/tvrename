using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;

namespace TVRename
{
    internal class SearchFolderFileFinder : FileFinder
    {
        public SearchFolderFileFinder(TVDoc doc, TVDoc.ScanSettings settings) : base(doc, settings)
        {
        }

        public override bool Active() => TVSettings.Instance.SearchLocally;

        protected override string CheckName() => "Looked in the search folders for the missing files";

        protected override void DoCheck(SetProgressDelegate prog)
        {
            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();

            int fileCount = CountFilesInDownloadDirs();

            DirCache dirCache = new DirCache();
            foreach (string s in TVSettings.Instance.DownloadFolders.ToList())
            {
                if (Settings.Token.IsCancellationRequested)
                {
                    return;
                }

                dirCache.AddFolder(prog, 0, fileCount, s, true);
            }

            int currentItem = 0;
            int totalN = ActionList.Missing.Count + 1;
            UpdateStatus(currentItem, totalN, "Starting searching through files");

            foreach (ItemMissing? action in ActionList.Missing.ToList())
            {
                if (Settings.Token.IsCancellationRequested)
                {
                    return;
                }

                UpdateStatus(currentItem++, totalN, action.Filename);

                ItemList thisRound = new ItemList();

                if (action is ShowItemMissing showMissingAction)
                {
                    List<FileInfo> matchedFiles = FindMatchedFiles(dirCache, showMissingAction, thisRound);

                    ProcessMissingItem(newList, toRemove, showMissingAction, thisRound, matchedFiles,
                        TVSettings.Instance.UseFullPathNameToMatchSearchFolders);
                }
                else if (action is MovieItemMissing movieMissingAction)
                {
                    List<FileInfo> matchedFiles = new List<FileInfo>();

                    foreach (DirCacheEntry dce in dirCache)
                    {
                        if (!ReviewFile(movieMissingAction, thisRound, dce.TheFile, TVSettings.Instance.PreventMove, true, TVSettings.Instance.UseFullPathNameToMatchSearchFolders))
                        {
                            continue;
                        }

                        matchedFiles.Add(dce.TheFile);
                    }

                    ProcessMissingItem(newList, toRemove, movieMissingAction, thisRound, matchedFiles,
                        TVSettings.Instance.UseFullPathNameToMatchSearchFolders);
                }
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
        private List<FileInfo> FindMatchedFiles([NotNull] DirCache dirCache, ShowItemMissing me, ItemList thisRound)
        {
            List<FileInfo> matchedFiles = new List<FileInfo>();

            foreach (DirCacheEntry dce in dirCache)
            {
                if (!ReviewFile(me, thisRound, dce.TheFile, TVSettings.Instance.AutoMergeDownloadEpisodes, TVSettings.Instance.PreventMove, true, TVSettings.Instance.UseFullPathNameToMatchSearchFolders))
                {
                    continue;
                }

                matchedFiles.Add(dce.TheFile);
            }

            return matchedFiles;
        }

        private static int CountFilesInDownloadDirs()
        {
            return TVSettings.Instance.DownloadFolders.ToArray().Select(x => x.Trim()).Sum(s => DirCache.CountFiles(s, true));
        }
    }
}
