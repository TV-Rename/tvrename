using System;
using System.Collections.Generic;

namespace TVRename
{
    public class ShowFilter
    {
        public ShowFilter() { }

        public List<string> Genres { get; } = new List<string>();

        public string ShowName { get; set; } 

        public string ShowStatus { get; set; }
        public string ShowNetwork { get; set; }
        public string ShowRating { get; set; }


        public bool filter(ShowItem show)
        {

            //Filter on show name
            bool isNameOK = (ShowName == null) || show.ShowName.Contains(ShowName, StringComparison.OrdinalIgnoreCase);

            //Filter on show status
            bool isStatusOK = (ShowStatus == null) || show.ShowStatus.Equals(ShowStatus);

            //Filter on show network
            bool isNetworkOK = (ShowNetwork == null) || (show.TheSeries() == null ) || show.TheSeries().GetNetwork().Equals(ShowNetwork);

            //Filter on show rating
            bool isRatingOK = (ShowRating == null) || (show.TheSeries() == null) || show.TheSeries().GetContentRating().Equals(ShowRating);

            //Filter on show genres
            bool areGenresIgnored = (Genres.Count == 0);

            bool doAnyGenresMatch = false; //assume false
            if (!areGenresIgnored )
                {
                if (show.Genres == null)
                {
                    doAnyGenresMatch = false;
                } else                 foreach(string showGenre in show.Genres)
                {
                    foreach (string filterGenre in Genres)
                        if (showGenre == filterGenre) doAnyGenresMatch = true;
                }
            }

            return isNameOK && isStatusOK && isNetworkOK && isRatingOK && (areGenresIgnored || doAnyGenresMatch );
        }
    }
}

