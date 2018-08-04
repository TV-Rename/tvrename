// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
namespace TVRename
{
    // "FolderMonitorItem" represents a folder found by doing a Check in the 'Bulk Add Shows' dialog
    public class FolderMonitorEntry
    {
        public readonly string Folder;
        // ReSharper disable once InconsistentNaming
        public int TVDBCode;
        public readonly bool HasSeasonFoldersGuess;
        public readonly string SeasonFolderFormat;

        public bool CodeKnown => !CodeUnknown;
        public bool CodeUnknown => TVDBCode == -1;

        public FolderMonitorEntry(string folder, bool seasonFolders, string folderFormat)
        {
            Folder = folder;
            TVDBCode = -1;
            HasSeasonFoldersGuess = seasonFolders;
            SeasonFolderFormat = folderFormat;
        }
    }

    public class FolderMonitorEntryList : System.Collections.Generic.List<FolderMonitorEntry>
    {
    }
}
