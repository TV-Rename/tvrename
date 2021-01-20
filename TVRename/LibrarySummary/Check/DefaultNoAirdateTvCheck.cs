using JetBrains.Annotations;

namespace TVRename
{
    internal class DefaultNoAirdateTvCheck : DefaultTvShowCheck
    {
        public DefaultNoAirdateTvCheck([NotNull] ShowConfiguration movie) : base(movie) { }

        protected override string FieldName => "No Airdate Check";

        protected override bool Field => Show.ForceCheckNoAirdate;

        protected override bool Default => TVSettings.Instance.DefShowIncludeNoAirdate;

        protected override void FixInternal()
        {
            Show.ForceCheckNoAirdate = Default;
        }
    }
}
