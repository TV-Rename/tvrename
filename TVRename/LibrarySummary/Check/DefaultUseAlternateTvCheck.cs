namespace TVRename;

internal class DefaultUseAlternateTvCheck : DefaultTvShowCheck
{
    public DefaultUseAlternateTvCheck(ShowConfiguration show, TVDoc doc) : base(show, doc)
    {
    }

    protected override string FieldName => "Use Alternate Order Check";

    protected override bool Field => Show.AlternateOrder;

    protected override bool Default => TVSettings.Instance.DefShowAlternateOrder;

    protected override void FixInternal()
    {
        Show.AlternateOrder = Default;
    }
}
