namespace TVRename;

internal class DefaultEpisodeNameMatchingTvCheck : DefaultTvShowCheck
{
    public DefaultEpisodeNameMatchingTvCheck(ShowConfiguration show, TVDoc doc) : base(show, doc)
    {
    }

    protected override string FieldName => "Do EpisodeName Matching Check";

    protected override bool Field => Show.UseEpNameMatch;

    protected override bool Default => TVSettings.Instance.DefShowEpNameMatching;

    protected override void FixInternal()
    {
        Show.UseEpNameMatch = Default;
    }
}
