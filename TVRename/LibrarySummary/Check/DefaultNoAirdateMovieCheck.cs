namespace TVRename
{
    internal class DefaultNoAirdateMovieCheck : DefaultMovieCheck
    {
        public DefaultNoAirdateMovieCheck(MovieConfiguration show, TVDoc doc) : base(show, doc)
        {
        }

        protected override string FieldName => "No Airdate Movie Check";

        protected override bool Field => Movie.ForceCheckNoAirdate;

        protected override bool Default => TVSettings.Instance.DefMovieCheckNoDatedMovies;

        protected override void FixInternal()
        {
            Movie.ForceCheckNoAirdate = Default;
        }
    }
}
