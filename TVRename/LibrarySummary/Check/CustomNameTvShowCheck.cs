namespace TVRename;

internal class CustomNameTvShowCheck(ShowConfiguration show, TVDoc doc) : CustomTvShowCheck(show, doc)
{
    protected override void FixInternal()
    {
        Show.UseCustomShowName = false;
    }

    protected override string FieldName => "Use Custom TV Show Name";
    protected override bool Field => Show.UseCustomShowName;
    protected override string? CustomFieldValue => Show.CustomShowName;

    protected override string? DefaultFieldValue => Show.CachedShow?.Name;
}
