namespace TVRename;

internal class CustomSearchTvShowCheck(ShowConfiguration show, TVDoc doc) : CustomTvShowCheck(show, doc)
{
    protected override void FixInternal()
    {
        Show.UseCustomSearchUrl = false;
    }

    protected override string FieldName => "Use Custom Search";

    protected override bool Field => Show.UseCustomSearchUrl;

    protected override string CustomFieldValue => Show.CustomSearchUrl;

    protected override string DefaultFieldValue => TVSettings.Instance.TheSearchers.CurrentSearch.Url;
}
