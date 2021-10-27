using JetBrains.Annotations;

namespace TVRename
{
    internal class CustomLanguageTvShowCheck : CustomTvShowCheck
    {
        public CustomLanguageTvShowCheck([NotNull] ShowConfiguration show, TVDoc doc) : base(show, doc)
        {
        }

        protected override void FixInternal()
        {
            Show.UseCustomLanguage = false;
        }

        [NotNull]
        protected override string FieldName => "Use Custom Language";

        protected override bool Field => Show.UseCustomLanguage;
    }
}
