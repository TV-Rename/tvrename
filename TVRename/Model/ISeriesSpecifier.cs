namespace TVRename
{
    public interface ISeriesSpecifier
    {
        TVDoc.ProviderType Provider { get; }
        int TvdbId { get; }
        string  Name { get; }
        MediaConfiguration.MediaType Type { get; }
        int TvMazeId { get; }
        int TmdbId { get; }
        string? ImdbCode { get; }
        string LanguageCode { get; }
        bool UseCustomLanguage { get; }
        string CustomLanguageCode { get; }
        int IdFor(TVDoc.ProviderType provider);
    }
}