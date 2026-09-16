namespace TVRename;

internal class DefaultAirDateMatchingTvCheck(ShowConfiguration show, TVDoc doc) : DefaultTvShowCheck(show, doc)
{
    protected override string FieldName => "Do Airdate matching Check";

    protected override bool Field => Show.UseAirDateMatch;

    protected override bool Default => TVSettings.Instance.DefShowAirDateMatching;

    protected override void FixInternal()
    {
        Show.UseAirDateMatch = Default;
    }
}
