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
            aired, // Season completely aired ... no further shows in this season scheduled to date
            partiallyAired, // Season partially aired ... there are further shows in this season which are unaired to date
            noneAired, // Season completely unaired ... no show of this season as aired yet
            noEpisodes,
        }

        public System.Collections.Generic.List<Episode> Episodes;
        public int SeasonId;
        public int SeasonNumber;
        public SeriesInfo TheSeries;

        public Season(SeriesInfo theSeries, int number, int seasonid)
        {
            TheSeries = theSeries;
            SeasonNumber = number;
            SeasonId = seasonid;
            Episodes = new System.Collections.Generic.List<Episode>();
        }

        public SeasonStatus Status(TimeZone tz)
        {
            if (HasEpisodes)
            {
                if (HasAiredEpisodes(tz) && !HasUnairedEpisodes(tz))
                {
                    return SeasonStatus.aired;
                }
                else if (HasAiredEpisodes(tz) && HasUnairedEpisodes(tz))
                {
                    return SeasonStatus.partiallyAired;
                }
                else if (!HasAiredEpisodes(tz) && HasUnairedEpisodes(tz))
                {
                    return SeasonStatus.noneAired;
                }
                else
                {
                    // Can happen if a Season has Episodes WITHOUT Airdates. 
                    //System.Diagnostics.Debug.Assert(false, string.Format("That is weird ... we have 'episodes' in '{0}' Season {1}, but none are aired, nor unaired. That case shouldn't actually occur !", this.TheSeries.Name,SeasonNumber));
                    return SeasonStatus.noEpisodes;
                }
            }
            else
            {
                return SeasonStatus.noEpisodes;
            }
        }

        private bool HasEpisodes => Episodes != null && Episodes.Count > 0;

        private bool HasUnairedEpisodes(TimeZone tz)
        {
            if (HasEpisodes)
            {
                foreach (Episode e in Episodes)
                {
                    if (e.GetAirDateDt(tz).HasValue)
                    {
                        if (e.GetAirDateDt(tz).Value > DateTime.Now)
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
                foreach (Episode e in Episodes)
                {
                    if (e.GetAirDateDt(tz).HasValue)
                    {
                        if (e.GetAirDateDt(tz).Value < DateTime.Now)
                            return true;
                    }
                }
            }

            return false;
        }

        public DateTime? LastAiredDate()
        {
            DateTime? returnValue = null;
            foreach (Episode a in Episodes)
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
            return TheSeries.GetSeasonBannerPath(SeasonNumber);
        }

        public string GetWideBannerPath()
        {
            return TheSeries.GetSeasonWideBannerPath(SeasonNumber);
        }

        public void AddUpdateEpisode(Episode newEpisode)
        {
            bool added = false;
            for (int i = 0; i < Episodes.Count; i++)
            {
                Episode ep = Episodes[i];
                if (ep.EpisodeId == newEpisode.EpisodeId)
                {
                    Episodes[i] = newEpisode;
                    added = true;
                    break;
                }
            }

            if (!added)
                Episodes.Add(newEpisode);
        }

        public bool ContainsEpisode(int episodeNumber, bool dvdOrder)
        {
            foreach (Episode ep in Episodes)
            {
                if (dvdOrder && ep.DvdEpNum == episodeNumber) return true;
                if (!dvdOrder && ep.AiredEpNum == episodeNumber) return true;
            }

            return false;
        }

        public void RemoveEpisode(int episodeId)
        {
            Episode ep = GetEpisode(episodeId);
            if (ep != null) Episodes.Remove(ep);
        }

        public Episode GetEpisode(int episodeId)
        {
            foreach (Episode ep in Episodes)
            {
                if (ep.EpisodeId == episodeId) return ep;
            }

            return null;
        }

        public bool IsSpecial()
        {
            return (SeasonNumber == 0);
        }
    }
}
