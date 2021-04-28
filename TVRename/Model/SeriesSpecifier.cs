// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;

namespace TVRename
{
    public class SeriesSpecifier
    {
        public readonly int TvdbSeriesId;
        public readonly int TvMazeSeriesId;
        public readonly int TmdbId;
        public readonly bool UseCustomLanguage;
        public readonly string CustomLanguageCode;
        public readonly string Name;
        public readonly string? ImdbCode;
        public readonly TVDoc.ProviderType Provider;
        public readonly MediaConfiguration.MediaType Type;

        public SeriesSpecifier(int tvdb, int tvmaze, int tmdb, bool useCustomLanguage, string? customLanguageCode,
            string name, TVDoc.ProviderType p, string? imdb, MediaConfiguration.MediaType t )
        {
            TvdbSeriesId = tvdb;
            TvMazeSeriesId = tvmaze;
            Name = name;
            ImdbCode = imdb;
            Provider = p;
            Type = t;
            TmdbId = tmdb;

            if (string.IsNullOrWhiteSpace(customLanguageCode))
            {
                UseCustomLanguage = false;
                CustomLanguageCode = TVSettings.Instance.PreferredLanguageCode;
            }
            else
            {
                UseCustomLanguage = useCustomLanguage;
                CustomLanguageCode = customLanguageCode;
            }
        }

        public override string ToString() => Type == MediaConfiguration.MediaType.tv
            ? $"{Name}//tvdb={TvdbSeriesId}//tvmaze={TvMazeSeriesId}//TMDB={TmdbId} {Provider} and lang = {CustomLanguageCode}."
            : $"{Name}//tvdb={TvdbSeriesId}//TMDB={TmdbId} {Provider} and lang = {CustomLanguageCode}.";

        public int IdFor(TVDoc.ProviderType provider)
        {
            return provider switch
            {
                TVDoc.ProviderType.TVmaze => TvMazeSeriesId,
                TVDoc.ProviderType.TheTVDB => TvdbSeriesId,
                TVDoc.ProviderType.TMDB => TmdbId,
                _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, null)
            };
        }
    }
}
