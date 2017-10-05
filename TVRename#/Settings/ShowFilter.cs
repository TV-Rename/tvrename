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
            if (ShowName != null && !show.ShowName.Contains(ShowName , StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            //Filter on show status
            if (ShowStatus != null && !show.ShowStatus.Equals(ShowStatus))
            {
                return false;
            }

            //Filter on show status
            if (ShowNetwork != null && !show.TheSeries().getNetwork().Equals(ShowNetwork))
            {
                return false;
            }

            //Filter on show status
            if (ShowRating != null) 
                if ( !show.TheSeries().GetRating().Equals(ShowRating))
                {
                    return false;
                }

            //Filter on show genres
            if (Genres.Count != 0)
            {
                if (show.Genres == null)
                {
                    return false;
                }
                List<String> showGenres = new List<String>(show.Genres);
                foreach (String filterGenre in Genres)
                {
                    if (!showGenres.Contains(filterGenre))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}

