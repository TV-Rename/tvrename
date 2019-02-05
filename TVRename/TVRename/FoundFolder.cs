// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    // "FoundFolder" represents a folder found by doing a Check in the 'Bulk Add Shows' dialog
    public class FoundFolder
    {
        public readonly DirectoryInfo Folder;
        // ReSharper disable once InconsistentNaming
        public int TVDBCode;
        public readonly bool HasSeasonFoldersGuess;
        public readonly string SeasonFolderFormat;
        public string RefinedHint;

        public bool CodeKnown => !CodeUnknown;
        public bool CodeUnknown => TVDBCode == -1;
        
        public FoundFolder(DirectoryInfo directory, bool seasonFolders, string folderFormat)
        {
            Folder = directory;
            TVDBCode = -1;
            HasSeasonFoldersGuess = seasonFolders;
            SeasonFolderFormat = folderFormat;
        }
    }
}
