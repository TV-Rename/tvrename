//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class DefaultUseDvdTvCheck(ShowConfiguration show, TVDoc doc) : DefaultTvShowCheck(show, doc)
{
    protected override string FieldName => "Use DVD Order Check";

    protected override bool Field => Show.DvdOrder;

    protected override bool Default => TVSettings.Instance.DefShowDVDOrder;

    protected override void FixInternal()
    {
        Show.DvdOrder = Default;
    }
}
