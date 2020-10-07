using JetBrains.Annotations;

namespace TVRename
{
    class CustomSearchTvShowCheck : CustomTvShowCheck
    {
        public CustomSearchTvShowCheck([NotNull] ShowConfiguration movie) : base(movie)
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