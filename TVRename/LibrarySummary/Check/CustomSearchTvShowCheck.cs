using JetBrains.Annotations;

namespace TVRename
{
    internal class CustomSearchTvShowCheck : CustomTvShowCheck
    {
        public CustomSearchTvShowCheck([NotNull] ShowConfiguration show, TVDoc doc) : base(show, doc)
        {
        }

        protected override void FixInternal()
        {
            Show.UseCustomSearchUrl = false;
        }

        protected override string FieldName => "Use Custom Search";

        protected override bool Field => Show.UseCustomSearchUrl;
    }
}
