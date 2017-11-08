using System;
using System.Collections.Generic;

namespace TVRename
{
    public class ShowFilter
    {
        public ShowFilter() { }

        public List<String> Genres { get; } = new List<String>();

        public String ShowName { get; set; } 

        public String ShowStatus { get; set; }
        public String ShowNetwork { get; set; }
        public String ShowRating { get; set; }


        public Boolean filter(ShowItem show)
        {

            //Filter on show name
            Boolean isNameOK = (ShowName == null) || show.ShowName.Contains(ShowName, StringComparison.OrdinalIgnoreCase);

            //Filter on show status
            Boolean isStatusOK = (ShowStatus == null) || show.ShowStatus.Equals(ShowStatus);

            //Filter on show network
            Boolean isNetworkOK = (ShowNetwork == null) || show.TheSeries().getNetwork().Equals(ShowNetwork);

            //Filter on show rating
            Boolean isRatingOK = (ShowRating == null) || show.TheSeries().GetRating().Equals(ShowRating);

            //Filter on show genres
            Boolean areGenresIgnored = (Genres.Count == 0);

            Boolean doAnyGenresMatch = false; //assume false
            if (!areGenresIgnored )
                {
                foreach (String showGenre in show.Genres)
                {
                    foreach (String filterGenre in this.Genres)
                        if (showGenre == filterGenre) doAnyGenresMatch = true;
                }
            }

            return isNameOK && isStatusOK && isNetworkOK && isRatingOK && (areGenresIgnored || doAnyGenresMatch );
        }
    }
}

