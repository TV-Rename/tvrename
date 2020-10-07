using JetBrains.Annotations;

namespace TVRename
{
    class CustomNameTvShowCheck : CustomTvShowCheck
    {
        public CustomNameTvShowCheck([NotNull] ShowConfiguration show) : base(show)  { }

        protected override void FixInternal()
        {
            Show.UseCustomShowName = false;
        }
        protected override string FieldName => "Use Custom TV Show Name";
        protected override bool Field => Show.UseCustomShowName;
    }
}