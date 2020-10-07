using JetBrains.Annotations;

namespace TVRename
{
    class CustomLanguageMovieCheck : CustomMovieCheck
    {
        public CustomLanguageMovieCheck([NotNull] MovieConfiguration movie) : base(movie)
        {
        }

        protected override void FixInternal()
        {
            Movie.UseCustomLanguage = false;
        }

        protected override string FieldName => "Use Custom Language";

        protected override bool Field => Movie.UseCustomLanguage;
    }
}