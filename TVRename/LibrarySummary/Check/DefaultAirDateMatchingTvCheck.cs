using JetBrains.Annotations;

namespace TVRename
{
    class DefaultAirDateMatchingTvCheck : DefaultTvShowCheck
    {
        public DefaultAirDateMatchingTvCheck([NotNull] ShowConfiguration movie) : base(movie) { }

        protected override string FieldName => "Do Airdate matching Check";

        protected override bool Field => Show.UseAirDateMatch;

        protected override bool Default => TVSettings.Instance.DefShowAirDateMatching;

        protected override void FixInternal()
        {
            Show.UseAirDateMatch = Default;
        }
    }
}