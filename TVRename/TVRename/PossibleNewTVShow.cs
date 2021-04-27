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
        internal int ProviderCode;
        internal TVDoc.ProviderType Provider;
        public readonly bool HasSeasonFoldersGuess;
        public readonly string SeasonFolderFormat;
        public string? RefinedHint;

        public bool CodeKnown => !CodeUnknown;
        public bool CodeUnknown => ProviderCode == -1;

        public CachedSeriesInfo? CachedSeries =>
            Provider switch
            {
                TVDoc.ProviderType.TMDB => TMDB.LocalCache.Instance.GetSeries(ProviderCode),
                TVDoc.ProviderType.TheTVDB => TheTVDB.LocalCache.Instance.GetSeries(ProviderCode),
                TVDoc.ProviderType.TVmaze => TVmaze.LocalCache.Instance.GetSeries(ProviderCode),
                _ => null
            };

        public PossibleNewTvShow(DirectoryInfo directory, bool seasonFolders, string folderFormat)
        {
            Folder = directory;
            ProviderCode = -1;
            HasSeasonFoldersGuess = seasonFolders;
            SeasonFolderFormat = folderFormat;
        }

        public void SetId(int code, TVDoc.ProviderType provider)
        {
            ProviderCode = code;
            Provider = provider;
        }
    }
}
