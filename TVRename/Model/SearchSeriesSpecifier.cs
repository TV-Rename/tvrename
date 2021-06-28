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
    public class SearchSpecifier : ISeriesSpecifier
    {
        public int TvdbId { get; private set; }
        public int TvMazeId { get; private set; }
        public int TmdbId { get; private set; }
        public string Name { get; }
        public string? ImdbCode { get; }
        public Locale TargetLocale { get; }

        public void UpdateId(int id, TVDoc.ProviderType source)
        {
            switch (source)
            {
                case TVDoc.ProviderType.TVmaze:
                    TvMazeId = id;
                    break;

                case TVDoc.ProviderType.TheTVDB:
                    TvdbId = id;
                    break;

                case TVDoc.ProviderType.TMDB:
                    TmdbId = id;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        public TVDoc.ProviderType Provider { get; }
        public MediaConfiguration.MediaType Media { get; }

        public SearchSpecifier(int tvdb, int tvmaze, int tmdb, Locale preferredLocale,
            string name, TVDoc.ProviderType p, string? imdb, MediaConfiguration.MediaType t)
        {
            TvdbId = tvdb;
            TvMazeId = tvmaze;
            Name = name;
            ImdbCode = imdb;
            Provider = p;
            Media = t;
            TmdbId = tmdb;
            TargetLocale = preferredLocale;
        }

        public SearchSpecifier(int id, Locale preferredLocale, TVDoc.ProviderType source, MediaConfiguration.MediaType t)
        {
            TvdbId = -1;
            TvMazeId = -1;
            TmdbId = -1;

            UpdateId(id, source);
            Media = t;
            TargetLocale = preferredLocale;

            Name = string.Empty;
        }

        public override string ToString() => Media == MediaConfiguration.MediaType.tv
            ? $"{Name}//tvdb={TvdbId}//tvmaze={TvMazeId}//TMDB={TmdbId} {Provider} and lang = {this.LanguageToUse().EnglishName}."
            : $"{Name}//tvdb={TvdbId}//TMDB={TmdbId} {Provider} and lang = {this.LanguageToUse().EnglishName}.";
    }
}
