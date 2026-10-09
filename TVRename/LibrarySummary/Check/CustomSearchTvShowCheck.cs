//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class CustomSearchTvShowCheck(ShowConfiguration show, TVDoc doc) : CustomTvShowCheck(show, doc)
{
    protected override void FixInternal()
    {
        Show.UseCustomSearchUrl = false;
    }

    protected override string FieldName => "Use Custom Search";

    protected override bool Field => Show.UseCustomSearchUrl;

    protected override string CustomFieldValue => Show.CustomSearchUrl;

    protected override string DefaultFieldValue => TVSettings.Instance.TheSearchers.CurrentSearch.Url;
}
