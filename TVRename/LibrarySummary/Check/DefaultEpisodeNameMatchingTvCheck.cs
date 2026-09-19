namespace TVRename;

internal class DefaultEpisodeNameMatchingTvCheck(ShowConfiguration show, TVDoc doc) : DefaultTvShowCheck(show, doc)
{
    protected override string FieldName => "Do EpisodeName Matching Check";

    protected override bool Field => Show.UseEpNameMatch;

    protected override bool Default => TVSettings.Instance.DefShowEpNameMatching;

    protected override void FixInternal()
    {
        Show.UseEpNameMatch = Default;
    }
}
