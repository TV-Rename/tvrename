//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class CustomLanguageTvShowCheck(ShowConfiguration show, TVDoc doc) : CustomTvShowCheck(show, doc)
{
    protected override void FixInternal()
    {
        Show.UseCustomLanguage = false;
    }

    protected override string FieldName => "Use Custom Language";

    protected override bool Field => Show.UseCustomLanguage;

    protected override string? CustomFieldValue => Show.CustomLanguageCode;

    protected override string DefaultFieldValue => Show.Provider == TVDoc.ProviderType.TMDB
        ? TVSettings.Instance.TMDBLanguage.ThreeAbbreviation
        : TVSettings.Instance.PreferredTVDBLanguage.ThreeAbbreviation;
}
