//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class DefaultNoAirdateMovieCheck(MovieConfiguration show, TVDoc doc) : DefaultMovieCheck(show, doc)
{
    protected override string FieldName => "No Airdate Movie Check";

    protected override bool Field => Movie.ForceCheckNoAirdate;

    protected override bool Default => TVSettings.Instance.DefMovieCheckNoDatedMovies;

    protected override void FixInternal()
    {
        Movie.ForceCheckNoAirdate = Default;
    }
}
