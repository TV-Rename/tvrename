namespace TVRename;

internal class DefaultSequentialMatchingTvCheck(ShowConfiguration show, TVDoc doc) : DefaultTvShowCheck(show, doc)
{
    protected override string FieldName => "Do Sequential Matching Check";

    protected override bool Field => Show.UseSequentialMatch;

    protected override bool Default => TVSettings.Instance.DefShowSequentialMatching;

    protected override void FixInternal()
    {
        Show.UseSequentialMatch = Default;
    }
}
