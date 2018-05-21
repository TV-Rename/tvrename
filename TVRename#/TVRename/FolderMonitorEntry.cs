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
        public string Folder;
        public int TVDBCode;
        public bool HasSeasonFoldersGuess;
        public string SeasonFolderName;
        public bool PadSeasonToTwoDigits;

        public bool CodeKnown { get { return !this.CodeUnknown; } }
        public bool CodeUnknown { get { return TVDBCode == -1; } }

        public FolderMonitorEntry(string folder, bool seasonFolders, string seasonFolderName,bool padSeasonToTwoDigits)
        {
            this.Folder = folder;
            this.TVDBCode = -1;
            this.HasSeasonFoldersGuess = seasonFolders;
            this.SeasonFolderName = seasonFolderName;
            this.PadSeasonToTwoDigits = padSeasonToTwoDigits;
        }
    }

    public class FolderMonitorEntryList : System.Collections.Generic.List<FolderMonitorEntry>
    {
    }
}
