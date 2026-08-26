namespace TVRename;

internal class DefaultFutureEpisodesTvCheck(ShowConfiguration show, TVDoc doc) : DefaultTvShowCheck(show, doc)
{
    protected override string FieldName => "Do Future Episodes Check";

    protected override bool Field => Show.ForceCheckFuture;

    protected override bool Default => TVSettings.Instance.DefShowIncludeFuture;

    protected override void FixInternal()
    {
        Show.ForceCheckFuture = Default;
    }
}
