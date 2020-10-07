using JetBrains.Annotations;

namespace TVRename
{
    class DefaultShowAirDateTvCheck : DefaultTvShowCheck
    {
        public DefaultShowAirDateTvCheck([NotNull] ShowConfiguration movie) : base(movie) { }

        protected override string FieldName => "Show Next AirDate Check";

        protected override bool Field => Show.ShowNextAirdate;

        protected override bool Default => TVSettings.Instance.DefShowNextAirdate;

        protected override void FixInternal()
        {
            Show.ShowNextAirdate = Default;
        }
    }
}