// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    // "PossibleNewTVShow" represents a folder found by doing a Check in the 'Bulk Add Shows' dialog
    public class PossibleNewTvShow
    {
        public readonly DirectoryInfo Folder;
        // ReSharper disable once InconsistentNaming
        public int TVDBCode;
        public readonly bool HasSeasonFoldersGuess;
        public readonly string SeasonFolderFormat;
        public string? RefinedHint;

        public bool CodeKnown => !CodeUnknown;
        public bool CodeUnknown => TVDBCode == -1;
        
        public PossibleNewTvShow(DirectoryInfo directory, bool seasonFolders, string folderFormat)
        {
            Folder = directory;
            TVDBCode = -1;
            HasSeasonFoldersGuess = seasonFolders;
            SeasonFolderFormat = folderFormat;
        }
    }
}
