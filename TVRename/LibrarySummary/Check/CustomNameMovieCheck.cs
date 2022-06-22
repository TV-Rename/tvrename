namespace TVRename
{
    internal class CustomNameMovieCheck : CustomMovieCheck
    {
        public CustomNameMovieCheck(MovieConfiguration movie, TVDoc doc) : base(movie, doc)
        {
        }

        protected override void FixInternal()
        {
            Movie.UseCustomShowName = false;
        }

        protected override string FieldName => "Use Custom Name";

        protected override bool Field => Movie.UseCustomShowName;

        protected override string CustomFieldValue => Movie.CustomShowName;

        protected override string DefaultFieldValue => Movie.CachedMovie?.Name??string.Empty;
    }
}
