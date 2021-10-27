using JetBrains.Annotations;

namespace TVRename
{
    internal class CustomRegionMovieCheck : CustomMovieCheck
    {
        public CustomRegionMovieCheck([NotNull] MovieConfiguration movie, TVDoc doc) : base(movie, doc)
        {
        }

        protected override void FixInternal()
        {
            Movie.UseCustomRegion = false;
        }

        [NotNull]
        protected override string FieldName => "Use Custom Region";

        protected override bool Field => Movie.UseCustomRegion;
    }
}
