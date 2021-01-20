using JetBrains.Annotations;

namespace TVRename
{
    internal class CustomRegionTvShowCheck : CustomTvShowCheck
    {
        public CustomRegionTvShowCheck([NotNull] ShowConfiguration show) : base(show) { }

        protected override void FixInternal()
        {
            Show.UseCustomRegion = false;
        }
        protected override string FieldName => "Use Custom Region";
        protected override bool Field => Show.UseCustomRegion;
    }
}
