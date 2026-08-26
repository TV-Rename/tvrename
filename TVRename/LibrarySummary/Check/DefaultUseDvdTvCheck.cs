namespace TVRename;

internal class DefaultUseDvdTvCheck(ShowConfiguration show, TVDoc doc) : DefaultTvShowCheck(show, doc)
{
    protected override string FieldName => "Use DVD Order Check";

    protected override bool Field => Show.DvdOrder;

    protected override bool Default => TVSettings.Instance.DefShowDVDOrder;

    protected override void FixInternal()
    {
        Show.DvdOrder = Default;
    }
}
