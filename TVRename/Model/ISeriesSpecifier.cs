using System;

namespace TVRename
{
    public interface ISeriesSpecifier
    {
        TVDoc.ProviderType Provider { get; }
        int TvdbId { get; }
        string Name { get; }
        MediaConfiguration.MediaType Type { get; }
        int TvMazeId { get; }
        int TmdbId { get; }
        string? ImdbCode { get; }

        Locale TargetLocale { get; }

        void UpdateId(int id, TVDoc.ProviderType source);
    }

    public static class SeriesSpecifiedHelper
    {
        public static Language LanguageToUse(this ISeriesSpecifier ss) => ss.TargetLocale.LanguageToUse(ss.Provider);

        public static Region RegionToUse(this ISeriesSpecifier ss) => ss.TargetLocale.RegionToUse(ss.Provider);

        public static int IdFor(this ISeriesSpecifier ss, TVDoc.ProviderType provider)
        {
            return provider switch
            {
                TVDoc.ProviderType.libraryDefault => ss.IdFor(DefaultProviderFor(ss.Type)),
                TVDoc.ProviderType.TVmaze => ss.TvMazeId,
                TVDoc.ProviderType.TheTVDB => ss.TvdbId,
                TVDoc.ProviderType.TMDB => ss.TmdbId,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static int Id(this ISeriesSpecifier ss) => ss.IdFor(ss.Provider);

        private static TVDoc.ProviderType DefaultProviderFor(MediaConfiguration.MediaType type)
        {
            return type switch
            {
                MediaConfiguration.MediaType.tv => TVSettings.Instance.DefaultProvider,
                MediaConfiguration.MediaType.movie => TVSettings.Instance.DefaultMovieProvider,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
