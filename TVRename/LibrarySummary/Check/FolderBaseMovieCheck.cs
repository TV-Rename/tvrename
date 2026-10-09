//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal class FolderBaseMovieCheck(MovieConfiguration movie, TVDoc doc) : MovieCheck(movie, doc)
{
    public override bool Check() => Movie.UseAutomaticFolders && !Movie.AutomaticFolderRoot.HasValue();

    public override string Explain() => "This Movie does not have an automatic folder base specified.";

    /// <exception cref="FixCheckException">Can't fix movie as multiple Movie Library Folders are specified</exception>
    protected override void FixInternal()
    {
        Movie.AutomaticFolderRoot = TVSettings.Instance.MovieLibraryFolders.Count switch
        {
            > 1 => throw new FixCheckException("Can't fix movie as multiple Movie Library Folders are specified"),
            0 => throw new FixCheckException("Can't fix movie as no Movie Library Folders are specified"),
            _ => TVSettings.Instance.MovieLibraryFolders.First()
        };
    }

    protected override string MovieCheckName => "Use Default folder supplied";
}
