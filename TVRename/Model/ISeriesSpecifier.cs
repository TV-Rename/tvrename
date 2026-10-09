//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

public interface ISeriesSpecifier
{
    TVDoc.ProviderType Provider { get; }
    int TvdbId { get; }
    string? Name { get; }
    MediaConfiguration.MediaType Media { get; }
    int TvMazeId { get; }
    int TmdbId { get; }
    string? ImdbCode { get; }

    Locale TargetLocale { get; }
    ProcessedSeason.SeasonType SeasonOrder { get; }

    void UpdateId(int id, TVDoc.ProviderType source);
}
