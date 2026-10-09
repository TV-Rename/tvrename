//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class DefaultSpecialsAsEpisodesTvCheck(ShowConfiguration show, TVDoc doc) : DefaultTvShowCheck(show, doc)
{
    protected override string FieldName => "Count Specials As Episodes Check";

    protected override bool Field => Show.CountSpecials;

    protected override bool Default => TVSettings.Instance.DefShowSpecialsCount;

    protected override void FixInternal()
    {
        Show.CountSpecials = Default;
    }
}
