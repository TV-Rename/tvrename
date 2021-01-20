using JetBrains.Annotations;

namespace TVRename
{
    internal class CustomNameMovieCheck : CustomMovieCheck
    {
        public CustomNameMovieCheck([NotNull] MovieConfiguration movie) : base(movie)
        {
        }

        protected override void FixInternal()
        {
            Movie.UseCustomShowName = false;
        }

        protected override string FieldName => "Use Custom Name";

        protected override bool Field => Movie.UseCustomShowName;
    }
}
