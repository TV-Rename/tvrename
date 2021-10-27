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
            if (TVSettings.Instance.MovieLibraryFolders.Count > 1)
            {
                throw new FixCheckException("Can't fix movie as multiple Movie Library Folders are specified");
            }

            if (TVSettings.Instance.MovieLibraryFolders.Count == 0)
            {
                throw new FixCheckException("Can't fix movie as no Movie Library Folders are specified");
            }

            Movie.AutomaticFolderRoot = TVSettings.Instance.MovieLibraryFolders.First();
        }

        [NotNull]
        public override string CheckName => "[Movie] Use Default folder supplied";
    }
}
