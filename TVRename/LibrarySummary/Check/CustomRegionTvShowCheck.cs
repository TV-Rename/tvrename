//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class CustomRegionTvShowCheck(ShowConfiguration show, TVDoc doc) : CustomTvShowCheck(show, doc)
{
    protected override void FixInternal()
    {
        Show.UseCustomRegion = false;
    }

    protected override string FieldName => "Use Custom Region";
    protected override bool Field => Show.UseCustomRegion;
    protected override string CustomFieldValue => Show.CustomRegionCode ?? string.Empty;

    protected override string DefaultFieldValue => Show.Provider == TVDoc.ProviderType.TMDB ? TVSettings.Instance.TMDBRegion.ThreeAbbreviation : string.Empty;
}
