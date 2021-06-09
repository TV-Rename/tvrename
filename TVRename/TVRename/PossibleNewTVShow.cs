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
    public class PossibleNewTvShow : ISeriesSpecifier
    {
        public readonly DirectoryInfo Folder;

        // ReSharper disable once InconsistentNaming
        internal int ProviderCode;

        internal TVDoc.ProviderType SourceProvider;
        public readonly bool HasSeasonFoldersGuess;
        public readonly string SeasonFolderFormat;
        public string? RefinedHint;

        public bool CodeKnown => !CodeUnknown;
        public bool CodeUnknown => ProviderCode == -1;

        public CachedSeriesInfo? CachedSeries => TVDoc.GetTVCache(Provider).GetSeries(ProviderCode);

        public PossibleNewTvShow(DirectoryInfo directory, bool seasonFolders, string folderFormat)
        {
            Folder = directory;
            ProviderCode = -1;
            HasSeasonFoldersGuess = seasonFolders;
            SeasonFolderFormat = folderFormat;
        }

        public void UpdateId(int id, TVDoc.ProviderType source)
        {
            SourceProvider = source;
            ProviderCode = id;
        }

        public TVDoc.ProviderType Provider => SourceProvider;

        public int TvdbId => Provider == TVDoc.ProviderType.TheTVDB && CodeKnown ? ProviderCode : -1;

        public string Name => RefinedHint ?? string.Empty;

        public MediaConfiguration.MediaType Type => MediaConfiguration.MediaType.tv;

        public int TvMazeId => Provider == TVDoc.ProviderType.TVmaze && CodeKnown ? ProviderCode : -1;

        public int TmdbId => Provider == TVDoc.ProviderType.TMDB && CodeKnown ? ProviderCode : -1;

        public string? ImdbCode => null;

        public Locale TargetLocale => new Locale();
    }
}
