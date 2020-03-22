// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using JetBrains.Annotations;

namespace TVRename
{
    public class SeriesSpecifier
    {
        public readonly int TvdbSeriesId;
        public readonly int TvMazeSeriesId;
        public readonly bool UseCustomLanguage;
        public readonly string CustomLanguageCode;
        public readonly string Name;
        public readonly string ImdbCode;
        public readonly ShowItem.ProviderType Provider;

        public SeriesSpecifier(int tvdb, int tvmaze, bool useCustomLanguage, [CanBeNull] string customLanguageCode,
            string name, ShowItem.ProviderType p, string imdb)
        {
            TvdbSeriesId = tvdb;
            TvMazeSeriesId = tvmaze;
            Name = name;
            ImdbCode = imdb;
            Provider = p;

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

        public override string ToString() =>
            $"{Name}//tvdb={TvdbSeriesId}//tvmaze={TvMazeSeriesId} {Provider} and lang = {CustomLanguageCode}.";
    }
}
