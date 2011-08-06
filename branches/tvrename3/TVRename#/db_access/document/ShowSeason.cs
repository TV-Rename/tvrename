using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TVRename.db_access.document
{
    class ShowSeason
    {
        public enum SeasonStatus
        {
            Aired, // Season completely aired ... no further shows in this season scheduled to date
            PartiallyAired, // Season partially aired ... there are further shows in this season which are unaired to date
            NoneAired, // Season completely unaired ... no show of this season as aired yet
            NoEpisodes,
        }

        public List<ShowEpisode> Episodes { get; set; }
        public int SeasonID { get; set; }
        public int SeasonNumber { get; set; }
    }
}
