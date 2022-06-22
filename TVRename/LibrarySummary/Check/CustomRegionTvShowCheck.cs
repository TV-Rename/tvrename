namespace TVRename
{
    internal class CustomRegionTvShowCheck : CustomTvShowCheck
    {
        public CustomRegionTvShowCheck(ShowConfiguration show, TVDoc doc) : base(show, doc)
        {
        }

        protected override void FixInternal()
        {
            Show.UseCustomRegion = false;
        }

        protected override string FieldName => "Use Custom Region";
        protected override bool Field => Show.UseCustomRegion;
        protected override string CustomFieldValue => Show.CustomRegionCode ?? string.Empty;

        protected override string DefaultFieldValue => Show.Provider == TVDoc.ProviderType.TMDB ? TVSettings.Instance.TMDBRegion.ThreeAbbreviation : string.Empty;
    }
}
