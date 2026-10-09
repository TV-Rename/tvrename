//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class DefaultAirDateMatchingTvCheck(ShowConfiguration show, TVDoc doc) : DefaultTvShowCheck(show, doc)
{
    protected override string FieldName => "Do Airdate matching Check";

    protected override bool Field => Show.UseAirDateMatch;

    protected override bool Default => TVSettings.Instance.DefShowAirDateMatching;

    protected override void FixInternal()
    {
        Show.UseAirDateMatch = Default;
    }
}
