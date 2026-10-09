//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class CustomRegionMovieCheck(MovieConfiguration movie, TVDoc doc) : CustomMovieCheck(movie, doc)
{
    protected override void FixInternal()
    {
        Movie.UseCustomRegion = false;
    }

    protected override string FieldName => "Use Custom Region";

    protected override bool Field => Movie.UseCustomRegion;

    protected override string CustomFieldValue => Movie.CustomRegionCode ?? string.Empty;

    protected override string DefaultFieldValue => Movie.Provider == TVDoc.ProviderType.TMDB ? TVSettings.Instance.TMDBRegion.ThreeAbbreviation : string.Empty;
}
