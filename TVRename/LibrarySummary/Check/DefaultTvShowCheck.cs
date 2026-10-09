//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal abstract class DefaultTvShowCheck(ShowConfiguration show, TVDoc doc) : TvShowCheck(show, doc)
{
    public override string CheckName => "[TV] " + FieldName;
    protected abstract string FieldName { get; }
    protected abstract bool Field { get; }
    protected abstract bool Default { get; }

    public override bool Check() => Field != Default;

    public override string Explain() => $"Default value for '{FieldName}' is {Default}. For this TV Show it is {Field}.";
}
