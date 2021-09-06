namespace TVRename
{
    public interface ISeriesSpecifier
    {
        TVDoc.ProviderType Provider { get; }
        int TvdbId { get; }
        string Name { get; }
        MediaConfiguration.MediaType Media { get; }
        int TvMazeId { get; }
        int TmdbId { get; }
        string? ImdbCode { get; }

        Locale TargetLocale { get; }
        ProcessedSeason.SeasonType SeasonOrder { get; }

        void UpdateId(int id, TVDoc.ProviderType source);
    }
}
