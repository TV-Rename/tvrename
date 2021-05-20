//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename
{
    public class SearchSeriesSpecifier : ISeriesSpecifier
    {
        public int TvdbId { get; }
        public int TvMazeId { get; }
        public int TmdbId { get; }
        public string Name { get; }
        public string? ImdbCode { get; }
        public Locale TargetLocale { get; }

        public TVDoc.ProviderType Provider { get; }
        public MediaConfiguration.MediaType Type { get; }

        public SearchSeriesSpecifier(int tvdb, int tvmaze, int tmdb, Locale preferredLocale,
            string name, TVDoc.ProviderType p, string? imdb, MediaConfiguration.MediaType t)
        {
            TvdbId = tvdb;
            TvMazeId = tvmaze;
            Name = name;
            ImdbCode = imdb;
            Provider = p;
            Type = t;
            TmdbId = tmdb;
            TargetLocale = preferredLocale;
        }

        public override string ToString() => Type == MediaConfiguration.MediaType.tv
            ? $"{Name}//tvdb={TvdbId}//tvmaze={TvMazeId}//TMDB={TmdbId} {Provider} and lang = {this.LanguageToUse().EnglishName}."
            : $"{Name}//tvdb={TvdbId}//TMDB={TmdbId} {Provider} and lang = {this.LanguageToUse().EnglishName}.";
    }
}
