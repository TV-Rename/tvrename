using JetBrains.Annotations;

namespace TVRename
{
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