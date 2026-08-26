namespace TVRename;

internal class DefaultNoAirdateTvCheck(ShowConfiguration show, TVDoc doc) : DefaultTvShowCheck(show, doc)
{
    protected override string FieldName => "No Airdate Check";

    protected override bool Field => Show.ForceCheckNoAirdate;

    protected override bool Default => TVSettings.Instance.DefShowIncludeNoAirdate;

    protected override void FixInternal()
    {
        Show.ForceCheckNoAirdate = Default;
    }
}
