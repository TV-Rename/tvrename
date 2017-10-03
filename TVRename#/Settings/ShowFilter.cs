using System;
using System.Collections.Generic;

namespace TVRename
{
    public class ShowFilter
    {
        private String showStatus = null;
        private List<String> genres = new List<String>();
        private String showName = null;

        public ShowFilter()
        {
        }

        public String ShowStatus
        {
            set
            {
                this.showStatus = value;
            }
            get
            {
                return showStatus;
            }
        }

        public List<String> Genres
        {
            get
            {
                return genres;
            }
        }

        public String ShowName
        {
            get
            {
                return showName;
            }
            set
            {
                this.showName = value;
            }
        }

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

            //Filter on show genres
            if (genres.Count != 0)
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
