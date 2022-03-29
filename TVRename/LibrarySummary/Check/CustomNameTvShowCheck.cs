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

        [NotNull]
        protected override string FieldName => "Use Custom TV Show Name";
        protected override bool Field => Show.UseCustomShowName;
        protected override string CustomFieldValue => Show.CustomShowName;

        [NotNull]
        protected override string DefaultFieldValue => Show.CachedShow?.Name ?? string.Empty;
    }
}
