//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class CustomNameTvShowCheck(ShowConfiguration show, TVDoc doc) : CustomTvShowCheck(show, doc)
{
    protected override void FixInternal()
    {
        Show.UseCustomShowName = false;
    }

    protected override string FieldName => "Use Custom TV Show Name";
    protected override bool Field => Show.UseCustomShowName;
    protected override string? CustomFieldValue => Show.CustomShowName;

    protected override string? DefaultFieldValue => Show.CachedShow?.Name;
}
