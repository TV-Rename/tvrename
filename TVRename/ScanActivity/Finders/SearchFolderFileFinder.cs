using Alphaleonis.Win32.Filesystem;
using System.Collections.Generic;
using System.Linq;

namespace TVRename;

internal class SearchFolderFileFinder : FileFinder
{
    public SearchFolderFileFinder(TVDoc doc, TVDoc.ScanSettings settings) : base(doc, settings)
    {
    }

    public override bool Active() => TVSettings.Instance.SearchLocally;

    protected override string CheckName() => "Looked in the search folders for the missing files";

    protected override void DoCheck(SetProgressDelegate progress)
    {
        ItemList newList = new();
        ItemList toRemove = new();

        int fileCount = CountFilesInDownloadDirs();

        DirCache dirCache = new();
        foreach (string s in TVSettings.Instance.DownloadFolders.ToList())
        {
            if (Settings.Token.IsCancellationRequested)
            {
                return;
            }

            dirCache.AddFolder(progress, 0, fileCount, s, true);
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

            Dictionary<FileInfo, ItemList> thisRound = new();

            if (action is ShowItemMissing showMissingAction)
            {
                List<FileInfo> matchedFiles = FindMatchedFiles(dirCache, showMissingAction, thisRound);

                ProcessMissingItem(newList, toRemove, showMissingAction, thisRound, matchedFiles,
                    TVSettings.Instance.UseFullPathNameToMatchSearchFolders);
            }
            else if (action is MovieItemMissing movieMissingAction)
            {
                List<FileInfo> matchedFiles = FindMatchedFiles(dirCache, movieMissingAction, thisRound);

                ProcessMissingItem(newList, toRemove, movieMissingAction, thisRound, matchedFiles,
                    TVSettings.Instance.UseFullPathNameToMatchSearchFolders);
            }
        }

        if (TVSettings.Instance.KeepTogether)
        {
            KeepTogether(newList, false);
        }
        if (TVSettings.Instance.CopySubsFolders)
        {
            CopySubsFolders(newList);
        }
        if (!TVSettings.Instance.LeaveOriginals)
        {
            ReorganiseToLeaveOriginals(newList);
        }

        ActionList.Replace(toRemove, newList);
    }

    private List<FileInfo> FindMatchedFiles(DirCache dirCache, MovieItemMissing movieMissingAction, Dictionary<FileInfo, ItemList> thisRound)
    {
        List<FileInfo> matchedFiles = new();

        foreach (DirCacheEntry dce in dirCache)
        {
            ItemList actionsForThisFile = new();

            if (thisRound.ContainsKey(dce.TheFile))
            {
                continue;
            }
            if (!ReviewFile(movieMissingAction, actionsForThisFile, dce.TheFile, TVSettings.Instance.PreventMove, true,
                    TVSettings.Instance.UseFullPathNameToMatchSearchFolders))
            {
                continue;
            }

            matchedFiles.Add(dce.TheFile);
            thisRound.Add(dce.TheFile, actionsForThisFile);
        }

        return matchedFiles;
    }

    private List<FileInfo> FindMatchedFiles(DirCache dirCache, ShowItemMissing me, Dictionary<FileInfo,ItemList> thisRound)
    {
        List<FileInfo> matchedFiles = new();

        foreach (DirCacheEntry dce in dirCache)
        {
            if (thisRound.ContainsKey(dce.TheFile))
            {
                continue;
            }
            ItemList actionsForThisFile = new();
            if (!ReviewFile(me, actionsForThisFile, dce.TheFile, TVSettings.Instance.AutoMergeDownloadEpisodes, TVSettings.Instance.PreventMove, true, TVSettings.Instance.UseFullPathNameToMatchSearchFolders))
            {
                continue;
            }

            matchedFiles.Add(dce.TheFile);
            thisRound.Add(dce.TheFile,actionsForThisFile);
        }

        return matchedFiles;
    }

    private static int CountFilesInDownloadDirs()
    {
        return TVSettings.Instance.DownloadFolders.ToArray().Select(x => x.Trim()).Sum(s => DirCache.CountFiles(s, true));
    }
}
