namespace TVRename
{
    internal class FilenameMovieCheck : MovieCheck
    {
        public FilenameMovieCheck(MovieConfiguration movie, TVDoc doc) : base(movie, doc)
        {
        }

        public override bool Check() => Movie.UseCustomNamingFormat;

        public override string Explain() => $"This movie does not use the standard file naming format '{TVSettings.Instance.MovieFilenameFormat}', it uses '{Movie.CustomNamingFormat}'";

        protected override void FixInternal()
        {
            Movie.UseCustomNamingFormat = false;
        }

        protected override string MovieCheckName => "Use Custom File Name Format";
    }
}
