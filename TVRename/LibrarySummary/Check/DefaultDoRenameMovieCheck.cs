//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class DefaultDoRenameMovieCheck(MovieConfiguration movie, TVDoc doc) : DefaultMovieCheck(movie, doc)
{
    protected override string FieldName => "Rename Check";

    protected override bool Field => Movie.DoRename;

    protected override bool Default => TVSettings.Instance.DefMovieDoRenaming;

    protected override void FixInternal()
    {
        Movie.DoRename = Default;
    }
}
