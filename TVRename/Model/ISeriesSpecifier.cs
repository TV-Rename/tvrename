using System;

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

        Locale TargetLocale { get; }
    }

    public static class SeriesSpecifiedHelper
    {
        public static Language LanguageToUse(this ISeriesSpecifier ss) => ss.TargetLocale.LanguageToUse(ss.Provider);
        public static Region RegionToUse(this ISeriesSpecifier ss) => ss.TargetLocale.RegionToUse(ss.Provider);

        public static int IdFor(this ISeriesSpecifier ss,TVDoc.ProviderType provider)
        {
            return provider switch
            {
                TVDoc.ProviderType.libraryDefault => throw new ArgumentOutOfRangeException(),
                TVDoc.ProviderType.TVmaze => ss.TvMazeId,
                TVDoc.ProviderType.TheTVDB => ss.TvdbId,
                TVDoc.ProviderType.TMDB => ss.TmdbId,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

    }
}
