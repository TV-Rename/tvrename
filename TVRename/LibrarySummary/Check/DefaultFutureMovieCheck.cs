//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class DefaultFutureMovieCheck(MovieConfiguration show, TVDoc doc) : DefaultMovieCheck(show, doc)
{
    protected override string FieldName => "Do Future Movies Check";

    protected override bool Field => Movie.ForceCheckFuture;

    protected override bool Default => TVSettings.Instance.DefMovieCheckFutureDatedMovies;

    protected override void FixInternal()
    {
        Movie.ForceCheckFuture = Default;
    }
}
