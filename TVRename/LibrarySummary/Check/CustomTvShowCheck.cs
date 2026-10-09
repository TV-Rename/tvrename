//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal abstract class CustomTvShowCheck(ShowConfiguration show, TVDoc doc) : TvShowCheck(show, doc)
{
    public override string CheckName => "[TV] " + FieldName;
    protected abstract string FieldName { get; }
    protected abstract bool Field { get; }
    protected abstract string? CustomFieldValue { get; }
    protected abstract string? DefaultFieldValue { get; }

    public override bool Check() => Field;

    public override string Explain() => $"{FieldName} is enabled for this TV Show [{CustomFieldValue}], by default it is not [{DefaultFieldValue}].";
}
