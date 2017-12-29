// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
namespace TVRename
{
    public class Season
    {
        public enum SeasonStatus
        {
            Aired, // Season completely aired ... no further shows in this season scheduled to date
            PartiallyAired, // Season partially aired ... there are further shows in this season which are unaired to date
            NoneAired, // Season completely unaired ... no show of this season as aired yet
            NoEpisodes,
        }

        public System.Collections.Generic.List<Episode> Episodes;
        public int SeasonID;
        public int SeasonNumber;
        public SeriesInfo TheSeries;

        public Season(SeriesInfo theSeries, int number, int seasonid)
        {
            TheSeries = theSeries;
            SeasonNumber = number;
            SeasonID = seasonid;
            Episodes = new System.Collections.Generic.List<Episode>();
        }

        public SeasonStatus Status
        {
            get
            {
                if (HasEpisodes)
                {
                    if (HasAiredEpisodes && !HasUnairedEpisodes)
                    {
                        return SeasonStatus.Aired;
                    }
                    else if (HasAiredEpisodes && HasUnairedEpisodes)
                    {
                        return SeasonStatus.PartiallyAired;
                    }
                    else if (!HasAiredEpisodes && HasUnairedEpisodes)
                    {
                        return SeasonStatus.NoneAired;
                    }
                    else
                    {
                        // Can happen if a Season has Episodes WITHOUT Airdates. 
                        //System.Diagnostics.Debug.Assert(false, string.Format("That is weird ... we have 'episodes' in '{0}' Season {1}, but none are aired, nor unaired. That case shouldn't actually occur !", this.TheSeries.Name,SeasonNumber));
                        return SeasonStatus.NoEpisodes;
                    }
                }
                else
                {
                    return SeasonStatus.NoEpisodes;
                }
            }
        }

        bool HasEpisodes
        {
            get
            {
                return Episodes != null && Episodes.Count > 0;
            }
        }

        bool HasUnairedEpisodes
        {
            get
            {
                if (HasEpisodes)
                {
                    foreach (Episode e in Episodes)
                    {
                        if (e.GetAirDateDT(true).HasValue)
                        {
                            if (e.GetAirDateDT(true).Value > System.DateTime.Now)
                                return true;
                        }
                    }
                }
                return false;
            }
        }

        bool HasAiredEpisodes
        {
            get
            {
                if (HasEpisodes)
                {
                    foreach (Episode e in Episodes)
                    {
                        if (e.GetAirDateDT(true).HasValue)
                        {
                            if (e.GetAirDateDT(true).Value < System.DateTime.Now)
                                return true;
                        }
                    }
                }
                return false;
            }
        }

        public string GetBannerPath()
        {
            return TheSeries.GetSeasonBannerPath(SeasonNumber);
        }

        public string GetWideBannerPath()
        {
            return TheSeries.GetSeasonWideBannerPath(SeasonNumber);
        }

    }
}
