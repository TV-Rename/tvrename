using JetBrains.Annotations;

namespace TVRename
{
    internal class CustomRegionMovieCheck : CustomMovieCheck
    {
        public CustomRegionMovieCheck([NotNull] MovieConfiguration movie) : base(movie)
        {
        }

        protected override void FixInternal()
        {
            Movie.UseCustomRegion = false;
        }

        protected override string FieldName => "Use Custom Region";

        protected override bool Field => Movie.UseCustomRegion;
    }
}
