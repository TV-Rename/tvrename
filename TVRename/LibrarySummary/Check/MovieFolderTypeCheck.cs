//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class MovieFolderTypeCheck(MovieConfiguration movie, TVDoc doc) : MovieCheck(movie, doc)
{
    public override bool Check() => Movie.Format != TVSettings.Instance.DefMovieFolderFormat;

    public override string Explain() => $"The default format for movies is {TVSettings.Instance.DefMovieFolderFormat.PrettyPrint()}, this movie uses {Movie.Format.PrettyPrint()}.";

    protected override void FixInternal()
    {
        Movie.Format = TVSettings.Instance.DefMovieFolderFormat;
    }

    protected override string MovieCheckName => "Movie Folder Format";
}
