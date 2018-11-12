// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename
{
    public class ShowFilter
    {
        public List<string> Genres { get; } = new List<string>();
        public string ShowName { get; set; } 
        public string ShowStatus { get; set; }
        public string ShowNetwork { get; set; }
        public string ShowRating { get; set; }

        public bool Filter(ShowItem show)
        {
            //Filter on show name
            bool isNameOk = (ShowName == null) || show.ShowName.Contains(ShowName, StringComparison.OrdinalIgnoreCase);

            //Filter on show status
            bool isStatusOk = (ShowStatus == null) || show.ShowStatus.Equals(ShowStatus);

            //Filter on show network
            bool isNetworkOk = (ShowNetwork == null) || (show.TheSeries() == null) || show.TheSeries().Network.Equals(ShowNetwork);

            //Filter on show rating
            bool isRatingOk = (ShowRating == null) || (show.TheSeries() == null) || show.TheSeries().ContentRating.Equals(ShowRating);

            //Filter on show genres
            bool areGenresIgnored = (Genres.Count == 0);
            bool doAnyGenresMatch = FindMatchingGenres(show);

            return isNameOk && isStatusOk && isNetworkOk && isRatingOk && (areGenresIgnored || doAnyGenresMatch);
        }

        private bool FindMatchingGenres(ShowItem show)
        {
            if (show.Genres == null) return false;

            return show.Genres.Any(showGenre => Genres.Contains(showGenre));
        }
    }
}
