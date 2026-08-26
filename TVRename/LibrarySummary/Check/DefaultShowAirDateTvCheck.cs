namespace TVRename;

internal class DefaultShowAirDateTvCheck(ShowConfiguration show, TVDoc doc) : DefaultTvShowCheck(show, doc)
{
    protected override string FieldName => "Show Next AirDate Check";

    protected override bool Field => Show.ShowNextAirdate;

    protected override bool Default => TVSettings.Instance.DefShowNextAirdate;

    protected override void FixInternal()
    {
        Show.ShowNextAirdate = Default;
    }
}
