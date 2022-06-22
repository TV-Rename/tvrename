namespace TVRename
{
    internal class DefaultNoAirdateTvCheck : DefaultTvShowCheck
    {
        public DefaultNoAirdateTvCheck(ShowConfiguration show, TVDoc doc) : base(show, doc)
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
}
