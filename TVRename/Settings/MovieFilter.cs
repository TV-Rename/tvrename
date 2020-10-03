// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    public class MovieFilter
    {
        public List<string> Genres { get; } = new List<string>();
        public string? ShowName { get; set; } 
        public string? ShowStatus { get; set; }
        public bool ShowStatusInclude { get; set; }
        public string? ShowNetwork { get; set; }
        public bool ShowNetworkInclude { get; set; }
        public string? ShowRating { get; set; }
        public bool ShowRatingInclude { get; set; }
        public string? ShowYear { get; set; }
        public bool ShowYearInclude { get; set; }

        public bool Filter([NotNull] MovieConfiguration show)
        {
            bool IsNetworkOk(MovieConfiguration showItem)
            {
                string? seriesInfoNetwork = showItem.CachedMovie?.Network;
                if (seriesInfoNetwork is null)
                {
                    return true;
                }

                return ShowNetworkInclude
                    ? seriesInfoNetwork.Equals(ShowNetwork)
                    : !seriesInfoNetwork.Equals(ShowNetwork);
            }

            bool IsRatingOk(MovieConfiguration showItem)
            {
                string? seriesInfoContentRating = showItem.CachedMovie?.ContentRating;
                if (seriesInfoContentRating is null)
                {
                    return true;
                }

                return ShowRatingInclude
                    ? seriesInfoContentRating.Equals(ShowRating)
                    : !seriesInfoContentRating.Equals(ShowRating);
            }

            bool IsStatusOk(MovieConfiguration showItem)
            {
                return ShowStatusInclude
                    ? showItem.CachedMovie?.Status?.Equals(ShowStatus) ?? true
                    :!showItem.CachedMovie?.Status?.Equals(ShowStatus) ?? true;
            }
            bool IsYearOk(MovieConfiguration showItem)
            {
                return ShowYearInclude
                    ? showItem.CachedMovie?.Year?.ToString().Equals(ShowYear) ?? true
                    : !showItem.CachedMovie?.Year?.ToString().Equals(ShowYear) ?? true;
            }

            //Filter on show name
            bool isNameOk = ShowName is null || show.ShowName.Contains(ShowName, StringComparison.OrdinalIgnoreCase);

            //Filter on show status
            bool isStatusOk = ShowStatus is null || IsStatusOk(show);

            //Filter on show network
            bool isNetworkOk = ShowNetwork is null || IsNetworkOk(show);

            //Filter on show rating
            bool isRatingOk = ShowRating is null || IsRatingOk(show);

            //Filter on show rating
            bool isYearOk = ShowYear is null || IsYearOk(show);

            //Filter on show genres
            bool areGenresIgnored = Genres.Count == 0;
            bool doAnyGenresMatch = FindMatchingGenres(show);

            return isNameOk && isStatusOk && isNetworkOk && isRatingOk && isYearOk && (areGenresIgnored || doAnyGenresMatch);
        }

        private bool FindMatchingGenres([NotNull] MovieConfiguration show)
        {
            return show.Genres.Any(showGenre => Genres.Contains(showGenre));
        }
    }
}
