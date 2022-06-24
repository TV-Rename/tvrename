namespace TVRename;

internal class CustomLanguageTvShowCheck : CustomTvShowCheck
{
    public CustomLanguageTvShowCheck(ShowConfiguration show, TVDoc doc) : base(show, doc)
    {
    }

    protected override void FixInternal()
    {
        Show.UseCustomLanguage = false;
    }

    protected override string FieldName => "Use Custom Language";

    protected override bool Field => Show.UseCustomLanguage;

    protected override string? CustomFieldValue => Show.CustomLanguageCode;

    protected override string DefaultFieldValue => Show.Provider == TVDoc.ProviderType.TMDB
        ? TVSettings.Instance.TMDBLanguage.ThreeAbbreviation
        : TVSettings.Instance.PreferredTVDBLanguage.ThreeAbbreviation;
}
