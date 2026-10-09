//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class DefaultEpisodeNameMatchingTvCheck(ShowConfiguration show, TVDoc doc) : DefaultTvShowCheck(show, doc)
{
    protected override string FieldName => "Do EpisodeName Matching Check";

    protected override bool Field => Show.UseEpNameMatch;

    protected override bool Default => TVSettings.Instance.DefShowEpNameMatching;

    protected override void FixInternal()
    {
        Show.UseEpNameMatch = Default;
    }
}
