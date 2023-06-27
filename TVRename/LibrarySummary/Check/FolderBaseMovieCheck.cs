using System.Linq;

namespace TVRename;

internal class FolderBaseMovieCheck : MovieCheck
{
    public FolderBaseMovieCheck(MovieConfiguration movie, TVDoc doc) : base(movie, doc)
    {
    }

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
