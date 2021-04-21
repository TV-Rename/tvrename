using JetBrains.Annotations;

namespace TVRename
{
    internal class CustomLanguageMovieCheck : CustomMovieCheck
    {
        public CustomLanguageMovieCheck([NotNull] MovieConfiguration movie, TVDoc doc) : base(movie, doc)
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
