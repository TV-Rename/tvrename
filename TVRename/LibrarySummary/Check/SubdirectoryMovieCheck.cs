using System;
using JetBrains.Annotations;

namespace TVRename
{
    class SubdirectoryMovieCheck : MovieCheck
    {
        public SubdirectoryMovieCheck([NotNull] MovieConfiguration movie) : base(movie) { }

        public override bool Check() => Movie.UseCustomFolderNameFormat;

        public override string Explain() => $"This Movie does not use the standard Folder naming format {TVSettings.Instance.MovieFolderFormat}, it uses {Movie.CustomFolderNameFormat}";

        protected override void FixInternal()
        {
            //todo - move files
            throw new NotImplementedException();
        }

        public override string CheckName => "[Movie] Use Custom Folder Name Format";

    }

    class FolderBaseMovieCheck : MovieCheck
    {
        public FolderBaseMovieCheck([NotNull] MovieConfiguration movie) : base(movie) { }

        public override bool Check() => Movie.UseAutomaticFolders && !Movie.AutomaticFolderRoot.HasValue();

        public override string Explain() => $"This Movie does not have an automatic folder base specified.";

        protected override void FixInternal()
        {
            Movie.AutomaticFolderRoot = TVSettings.Instance.MovieLibraryFolders[1];
            return;
            throw new NotImplementedException();
        }

        public override string CheckName => "[Movie] Use Default folder supplied";
    }
}
