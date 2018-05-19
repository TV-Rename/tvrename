// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
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

        public SeasonStatus Status(TimeZone tz)
        {
            if (HasEpisodes)
            {
                if (HasAiredEpisodes(tz) && !HasUnairedEpisodes(tz))
                {
                    return SeasonStatus.Aired;
                }
                else if (HasAiredEpisodes(tz) && HasUnairedEpisodes(tz))
                {
                    return SeasonStatus.PartiallyAired;
                }
                else if (!HasAiredEpisodes(tz) && HasUnairedEpisodes(tz))
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

        private bool HasEpisodes => this.Episodes != null && this.Episodes.Count > 0;

        private bool HasUnairedEpisodes(TimeZone tz)
        {
            if (HasEpisodes)
            {
                foreach (Episode e in this.Episodes)
                {
                    if (e.GetAirDateDT(tz).HasValue)
                    {
                        if (e.GetAirDateDT(tz).Value > System.DateTime.Now)
                            return true;
                    }
                }
            }

            return false;
        }

        private bool HasAiredEpisodes(TimeZone tz)
        {
            if (HasEpisodes)
            {
                foreach (Episode e in this.Episodes)
                {
                    if (e.GetAirDateDT(tz).HasValue)
                    {
                        if (e.GetAirDateDT(tz).Value < System.DateTime.Now)
                            return true;
                    }
                }
            }

            return false;
        }

        public DateTime? LastAiredDate()
        {
            DateTime? returnValue = null;
            foreach (Episode a in this.Episodes)
            {
                DateTime? episodeAirDate = a.FirstAired;

                //ignore episode if has no date
                if (!episodeAirDate.HasValue) continue;

                //ignore episode if it's in the future
                if (DateTime.Compare(episodeAirDate.Value.ToUniversalTime(), DateTime.UtcNow) > 0) continue;

                //If we don't have a best offer yet
                if (!returnValue.HasValue) returnValue = episodeAirDate.Value;
                //else the currently tested date is better than the current value
                else if (DateTime.Compare(episodeAirDate.Value, returnValue.Value) > 0)
                    returnValue = episodeAirDate.Value;
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

        public void AddUpdateEpisode(Episode newEpisode)
        {
            bool added = false;
            for (int i = 0; i < this.Episodes.Count; i++)
            {
                Episode ep = this.Episodes[i];
                if (ep.EpisodeID == newEpisode.EpisodeID)
                {
                    this.Episodes[i] = newEpisode;
                    added = true;
                    break;
                }
            }

            if (!added)
                this.Episodes.Add(newEpisode);
        }

        public bool ContainsEpisode(int episodeNumber, bool dvdOrder)
        {
            foreach (Episode ep in this.Episodes)
            {
                if (dvdOrder && ep.DVDEpNum == episodeNumber) return true;
                if (!dvdOrder && ep.AiredEpNum == episodeNumber) return true;
            }

            return false;
        }

        public void RemoveEpisode(int episodeId)
        {
            Episode ep = GetEpisode(episodeId);
            if (ep != null) this.Episodes.Remove(ep);
        }

        public Episode GetEpisode(int episodeId)
        {
            foreach (Episode ep in this.Episodes)
            {
                if (ep.EpisodeID == episodeId) return ep;
            }

            return null;
        }
    }
}
