using JetBrains.Annotations;
using System.Linq;

namespace TVRename
{
    internal class FolderBaseMovieCheck : MovieCheck
    {
        public FolderBaseMovieCheck([NotNull] MovieConfiguration movie, TVDoc doc) : base(movie, doc)
        {
        }

        public override bool Check() => Movie.UseAutomaticFolders && !Movie.AutomaticFolderRoot.HasValue();

        [NotNull]
        public override string Explain() => "This Movie does not have an automatic folder base specified.";

        protected override void FixInternal()
        {
            Movie.AutomaticFolderRoot = TVSettings.Instance.MovieLibraryFolders.Count switch
            {
                > 1 => throw new FixCheckException("Can't fix movie as multiple Movie Library Folders are specified"),
                0 => throw new FixCheckException("Can't fix movie as no Movie Library Folders are specified"),
                _ => TVSettings.Instance.MovieLibraryFolders.First()
            };
        }

        [NotNull]
        public override string CheckName => "[Movie] Use Default folder supplied";
    }
}
