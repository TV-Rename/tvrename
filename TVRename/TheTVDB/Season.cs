// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System;
using System.Collections.Generic;

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

        public System.Collections.Generic.Dictionary<int,Episode> Episodes;
        public int SeasonId;
        public int SeasonNumber;
        public SeriesInfo TheSeries;

        public Season(SeriesInfo theSeries, int number, int seasonid)
        {
            TheSeries = theSeries;
            SeasonNumber = number;
            SeasonId = seasonid;
            Episodes = new Dictionary<int, Episode>();
        }

        // ReSharper disable once InconsistentNaming
        public static string UISeasonWord(int season)
        {
            if (TVSettings.Instance.defaultSeasonWord.Length > 1)
            {
                return TVSettings.Instance.defaultSeasonWord + " " + season;
            }
            else
            {
                bool leadingZero = TVSettings.Instance.LeadingZeroOnSeason;
                if (leadingZero == true)
                {
                    return TVSettings.Instance.defaultSeasonWord + season.ToString("00");
                }
                else
                {
                    return TVSettings.Instance.defaultSeasonWord + season.ToString();
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        public static string UIFullSeasonWord(int snum)
        {
            return (snum == 0)
                ? TVSettings.Instance.SpecialsFolderName
                : UISeasonWord(snum);
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

        internal int MinYear()
        {
            int min = 9999;

            foreach (Episode e in Episodes.Values)
            {
                if (e.GetAirDateDt().HasValue)
                {
                    if (e.GetAirDateDt().Value.Year < min) min = e.GetAirDateDt().Value.Year;
                }
            }

            return min;
        }

        internal int MaxYear()
        {
            int max = 0;

            foreach (Episode e in Episodes.Values)
            {
                if (e.GetAirDateDt().HasValue)
                {
                    if (e.GetAirDateDt().Value.Year > max) max = e.GetAirDateDt().Value.Year;
                }
            }
            return max;
        }
        private bool HasEpisodes => Episodes != null && Episodes.Count > 0;

        private bool HasUnairedEpisodes(TimeZone tz)
        {
            if (HasEpisodes)
            {
                foreach (Episode e in Episodes.Values)
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
                foreach (Episode e in Episodes.Values)
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
            foreach (Episode a in Episodes.Values)
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
            if (Episodes.ContainsKey(newEpisode.EpisodeId))
            {
                Episodes[newEpisode.EpisodeId] = newEpisode;
            }
            else
            {
                Episodes.Add(newEpisode.EpisodeId,newEpisode);
            }
        }

        public bool ContainsEpisode(int episodeNumber, bool dvdOrder)
        {
            foreach (Episode ep in Episodes.Values)
            {
                if (dvdOrder && ep.DvdEpNum == episodeNumber) return true;
                if (!dvdOrder && ep.AiredEpNum == episodeNumber) return true;
            }

            return false;
        }

        public void RemoveEpisode(int episodeId)
        {
            if (Episodes.ContainsKey(episodeId))
            {
                Episodes.Remove(episodeId);
            }
        }

        public bool IsSpecial()
        {
            return (SeasonNumber == 0);
        }
    }
}
