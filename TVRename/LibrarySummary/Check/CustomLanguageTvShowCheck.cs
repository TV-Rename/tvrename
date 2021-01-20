using JetBrains.Annotations;

namespace TVRename
{
    internal class CustomLanguageTvShowCheck : CustomTvShowCheck
    {
        public CustomLanguageTvShowCheck([NotNull] ShowConfiguration movie) : base(movie)
        {
        }

        protected override void FixInternal()
        {
            Show.UseCustomLanguage = false;
        }

        protected override string FieldName => "Use Custom Language";

        protected override bool Field => Show.UseCustomLanguage;
    }
}
