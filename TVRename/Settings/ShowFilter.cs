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
    public class ShowFilter
    {
        public List<string> Genres { get; } = new List<string>();
        public string ShowName { get; set; } 
        public string ShowStatus { get; set; }
        public bool ShowStatusInclude { get; set; }
        public string ShowNetwork { get; set; }
        public bool ShowNetworkInclude { get; set; }
        public string ShowRating { get; set; }
        public bool ShowRatingInclude { get; set; }

        public bool Filter([NotNull] ShowItem show)
        {
            bool IsNetworkOk(ShowItem showItem)
            {
                SeriesInfo seriesInfo = showItem.TheSeries();
                if (seriesInfo is null)
                {
                    return true;
                }
                return ShowNetworkInclude
                    ? seriesInfo.Network.Equals(ShowNetwork)
                    : !seriesInfo.Network.Equals(ShowNetwork);
            }

            bool IsRatingOk(ShowItem showItem)
            {
                SeriesInfo seriesInfo = showItem.TheSeries();
                if (seriesInfo is null)
                {
                    return true;
                }

                return ShowRatingInclude
                    ? seriesInfo.ContentRating.Equals(ShowRating)
                    : !seriesInfo.ContentRating.Equals(ShowRating);
            }

            bool IsStatusOk(ShowItem showItem)
            {
                return ShowStatusInclude
                    ? showItem.ShowStatus.Equals(ShowStatus)
                    :!showItem.ShowStatus.Equals(ShowStatus);
            }

            //Filter on show name
            bool isNameOk = ShowName is null || show.ShowName.Contains(ShowName, StringComparison.OrdinalIgnoreCase);

            //Filter on show status
            bool isStatusOk = ShowStatus is null || IsStatusOk(show);

            //Filter on show network
            bool isNetworkOk = ShowNetwork is null || IsNetworkOk(show);

            //Filter on show rating
            bool isRatingOk = ShowRating is null || IsRatingOk(show);

            //Filter on show genres
            bool areGenresIgnored = Genres.Count == 0;
            bool doAnyGenresMatch = FindMatchingGenres(show);

            return isNameOk && isStatusOk && isNetworkOk && isRatingOk && (areGenresIgnored || doAnyGenresMatch);
        }

        private bool FindMatchingGenres([NotNull] ShowItem show)
        {
            return show.Genres.Any(showGenre => Genres.Contains(showGenre));
        }
    }
}
