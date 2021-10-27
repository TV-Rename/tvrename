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
    }
}
