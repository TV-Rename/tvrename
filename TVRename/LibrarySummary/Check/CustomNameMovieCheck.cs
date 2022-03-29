using JetBrains.Annotations;

namespace TVRename
{
    internal class CustomNameMovieCheck : CustomMovieCheck
    {
        public CustomNameMovieCheck([NotNull] MovieConfiguration movie, TVDoc doc) : base(movie, doc)
        {
        }

        protected override void FixInternal()
        {
            Movie.UseCustomShowName = false;
        }

        [NotNull]
        protected override string FieldName => "Use Custom Name";

        protected override bool Field => Movie.UseCustomShowName;

        protected override string CustomFieldValue => Movie.CustomShowName;

        [NotNull]
        protected override string DefaultFieldValue => Movie.CachedMovie?.Name??string.Empty;
    }
}
