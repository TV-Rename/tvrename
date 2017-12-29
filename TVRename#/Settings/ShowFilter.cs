using System;
using System.Collections.Generic;

namespace TVRename
{
    public class ShowFilter
    {
        public List<String> Genres { get; } = new List<String>();

        public String ShowName { get; set; } 

        public String ShowStatus { get; set; }
        public String ShowNetwork { get; set; }
        public String ShowRating { get; set; }


        public Boolean Filter(ShowItem show)
        {

            //Filter on show name
            Boolean isNameOk = (ShowName == null) || show.ShowName.Contains(ShowName, StringComparison.OrdinalIgnoreCase);

            //Filter on show status
            Boolean isStatusOk = (ShowStatus == null) || show.ShowStatus.Equals(ShowStatus);

            //Filter on show network
            Boolean isNetworkOk = (ShowNetwork == null) || (show.TheSeries() == null ) || show.TheSeries().GetNetwork().Equals(ShowNetwork);

            //Filter on show rating
            Boolean isRatingOk = (ShowRating == null) || (show.TheSeries() == null) || show.TheSeries().GetRating().Equals(ShowRating);

            //Filter on show genres
            Boolean areGenresIgnored = (Genres.Count == 0);

            Boolean doAnyGenresMatch = false; //assume false
            if (!areGenresIgnored )
                {
                if (show.Genres != null)
                    foreach (String showGenre in show.Genres)
                    {
                        foreach (String filterGenre in Genres)
                            if (showGenre == filterGenre) doAnyGenresMatch = true;
                    }
            }

            return isNameOk && isStatusOk && isNetworkOk && isRatingOk && (areGenresIgnored || doAnyGenresMatch );
        }
    }
}

