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
    public class SearchSeriesSpecifier : ISeriesSpecifier
    {
        public  int TvdbId { get; }
        public int TvMazeId { get; }
        public int TmdbId { get; }
        public  bool UseCustomLanguage { get;  }
        public  string CustomLanguageCode { get;  }
        public  string Name { get;  }
        public  string? ImdbCode { get;  }
        public  TVDoc.ProviderType Provider { get;  }
        public  MediaConfiguration.MediaType Type { get;  }

        public SearchSeriesSpecifier(int tvdb, int tvmaze, int tmdb, bool useCustomLanguage, string? customLanguageCode,
            string name, TVDoc.ProviderType p, string? imdb, MediaConfiguration.MediaType t )
        {
            TvdbId = tvdb;
            TvMazeId = tvmaze;
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
            ? $"{Name}//tvdb={TvdbId}//tvmaze={TvMazeId}//TMDB={TmdbId} {Provider} and lang = {LanguageCode}."
            : $"{Name}//tvdb={TvdbId}//TMDB={TmdbId} {Provider} and lang = {LanguageCode}.";

        public int IdFor(TVDoc.ProviderType provider)
        {
            return provider switch
            {
                TVDoc.ProviderType.TVmaze => TvMazeId,
                TVDoc.ProviderType.TheTVDB => TvdbId,
                TVDoc.ProviderType.TMDB => TmdbId,
                _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, null)
            };
        }

        public string LanguageCode =>UseCustomLanguage ? CustomLanguageCode : TVSettings.Instance.PreferredLanguageCode;
    }
}
