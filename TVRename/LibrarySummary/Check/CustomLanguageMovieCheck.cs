//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class CustomLanguageMovieCheck(MovieConfiguration movie, TVDoc doc) : CustomMovieCheck(movie, doc)
{
    protected override void FixInternal()
    {
        Movie.UseCustomLanguage = false;
    }

    protected override string FieldName => "Use Custom Language";

    protected override bool Field => Movie.UseCustomLanguage;

    protected override string? CustomFieldValue => Movie.CustomLanguageCode;

    protected override string DefaultFieldValue => Movie.Provider == TVDoc.ProviderType.TMDB ? TVSettings.Instance.TMDBLanguage.ThreeAbbreviation : TVSettings.Instance.PreferredTVDBLanguage.ThreeAbbreviation;
}
