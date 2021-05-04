using System;
using System.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    internal class FolderBaseMovieCheck : MovieCheck
    {
        public FolderBaseMovieCheck([NotNull] MovieConfiguration movie, TVDoc doc) : base(movie, doc) { }

        public override bool Check() => Movie.UseAutomaticFolders && !Movie.AutomaticFolderRoot.HasValue();

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

        public override string CheckName => "[Movie] Use Default folder supplied";
    }

    internal class FolderBaseTvCheck : TvShowCheck
    {
        public FolderBaseTvCheck([NotNull] ShowConfiguration show, TVDoc doc) : base(show, doc) { }

        public override bool Check() => !Show.AutoAddFolderBase.HasValue() && Show.AutoAddNewSeasons();

        public override string Explain() => "This TV show does not have an automatic folder base specified.";

        protected override void FixInternal()
        {
            throw new NotImplementedException();
        }

        public override string CheckName => "[TV] Has an automatic base folder supplied";
    }
}
