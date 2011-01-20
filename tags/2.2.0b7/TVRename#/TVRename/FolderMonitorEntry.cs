// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
namespace TVRename
{
    // "FolderMonitorItem" represents a folder found by doing a Check in the Folder monitor dialog
    public class FolderMonitorEntry
    {
        public string Folder;
        public int TVDBCode;
        public bool HasSeasonFoldersGuess;
        public string SeasonFolderName;

        public bool CodeKnown { get { return !this.CodeUnknown; } }
        public bool CodeUnknown { get { return TVDBCode == -1; } }

        public FolderMonitorEntry(string folder, bool seasonFolders, string seasonFolderName)
        {
            this.Folder = folder;
            this.TVDBCode = -1;
            this.HasSeasonFoldersGuess = seasonFolders;
            this.SeasonFolderName = seasonFolderName;
        }
    }

    public class FolderMonitorEntryList : System.Collections.Generic.List<FolderMonitorEntry>
    {
    }
}