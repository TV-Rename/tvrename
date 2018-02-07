// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;

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
            this.TheSeries = theSeries;
            this.SeasonNumber = number;
            this.SeasonID = seasonid;
            this.Episodes = new System.Collections.Generic.List<Episode>();
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
                return this.Episodes != null && this.Episodes.Count > 0;
            }
        }

        bool HasUnairedEpisodes
        {
            get
            {
                if (HasEpisodes)
                {
                    foreach (Episode e in this.Episodes)
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
                    foreach (Episode e in this.Episodes)
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

        
        public DateTime? LastAiredDate() {
            DateTime? returnValue = null;
            foreach (Episode a in Episodes)
            {
                if (DateTime.Compare(a.FirstAired.Value.ToUniversalTime(), DateTime.UtcNow) <= 0)
                {
                    //Date is before now()

                    //We don't have a best offer yet
                    if (returnValue == null) returnValue = a.FirstAired.Value.ToUniversalTime();
                    else
                    //the currently tested date is better than the current value
                    if (DateTime.Compare(a.FirstAired.Value.ToUniversalTime(), returnValue.Value) > 0)
                    {
                        returnValue = a.FirstAired.Value.ToUniversalTime();
                    }
                }
            }
            return returnValue;

        }

        public string GetBannerPath()
        {
            return this.TheSeries.GetSeasonBannerPath(this.SeasonNumber);
        }

        public string GetWideBannerPath()
        {
            return this.TheSeries.GetSeasonWideBannerPath(this.SeasonNumber);
        }

    }
}
