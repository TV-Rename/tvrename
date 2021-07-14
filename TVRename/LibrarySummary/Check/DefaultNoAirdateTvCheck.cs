using JetBrains.Annotations;

namespace TVRename
{
    internal class DefaultNoAirdateTvCheck : DefaultTvShowCheck
    {
        public DefaultNoAirdateTvCheck([NotNull] ShowConfiguration show, TVDoc doc) : base(show, doc)
        {
        }

        protected override string FieldName => "No Airdate Check";

        protected override bool Field => Show.ForceCheckNoAirdate;

        protected override bool Default => TVSettings.Instance.DefShowIncludeNoAirdate;

        protected override void FixInternal()
        {
            Show.ForceCheckNoAirdate = Default;
        }
    }
    internal class DefaultNoAirdateMovieCheck : DefaultMovieCheck
    {
        public DefaultNoAirdateMovieCheck([NotNull] MovieConfiguration show, TVDoc doc) : base(show, doc)
        {
        }

        protected override string FieldName => "No Airdate Movie Check";

        protected override bool Field => Movie.ForceCheckNoAirdate;

        protected override bool Default => TVSettings.Instance.CheckNoDatedMovies;

        protected override void FixInternal()
        {
            Movie.ForceCheckNoAirdate = Default;
        }
    }
}
