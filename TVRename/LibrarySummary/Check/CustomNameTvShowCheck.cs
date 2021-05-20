using JetBrains.Annotations;

namespace TVRename
{
    internal class CustomNameTvShowCheck : CustomTvShowCheck
    {
        public CustomNameTvShowCheck([NotNull] ShowConfiguration show, TVDoc doc) : base(show, doc)
        {
        }

        protected override void FixInternal()
        {
            Show.UseCustomShowName = false;
        }

        protected override string FieldName => "Use Custom TV Show Name";
        protected override bool Field => Show.UseCustomShowName;
    }
}
