using JetBrains.Annotations;

namespace TVRename
{
    internal class MovieFolderTypeCheck : MovieCheck
    {
        public MovieFolderTypeCheck(MovieConfiguration movie, TVDoc doc) : base(movie, doc)
        {
        }

        public override bool Check() => Movie.Format != TVSettings.Instance.DefMovieFolderFormat;

        [NotNull]
        public override string Explain() => $"The default format for movies is {TVSettings.Instance.DefMovieFolderFormat.PrettyPrint()}, this movie uses {Movie.Format.PrettyPrint()}.";

        protected override void FixInternal()
        {
            Movie.Format = TVSettings.Instance.DefMovieFolderFormat;
        }

        [NotNull]
        protected override string MovieCheckName => "Movie Folder Format";
    }
}
