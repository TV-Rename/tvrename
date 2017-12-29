// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System.Collections.Generic;

namespace TVRename
{
    // "FolderMonitorItem" represents a folder found by doing a Check in the Folder monitor dialog
    public class FolderMonitorEntry
    {
        public string Folder;
        public int TVDBCode;
        public bool HasSeasonFoldersGuess;
        public string SeasonFolderName;

        public bool CodeKnown { get { return !CodeUnknown; } }
        public bool CodeUnknown { get { return TVDBCode == -1; } }

        public FolderMonitorEntry(string folder, bool seasonFolders, string seasonFolderName)
        {
            Folder = folder;
            TVDBCode = -1;
            HasSeasonFoldersGuess = seasonFolders;
            SeasonFolderName = seasonFolderName;
        }
    }

    public class FolderMonitorEntryList : List<FolderMonitorEntry>
    {
    }
}
