using System;

namespace TVRename
{
    public static class SeriesSpecifiedHelper
    {
        public static Language LanguageToUse(this ISeriesSpecifier ss) => ss.TargetLocale.LanguageToUse(ss.Provider);

        public static Region RegionToUse(this ISeriesSpecifier ss) => ss.TargetLocale.RegionToUse(ss.Provider);

        public static int IdFor(this ISeriesSpecifier ss, TVDoc.ProviderType provider)
        {
            return provider switch
            {
                TVDoc.ProviderType.libraryDefault => ss.IdFor(DefaultProviderFor(ss.Media)),
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