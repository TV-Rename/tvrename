// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System;
using System.Collections.Generic;
using JetBrains.Annotations;

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

        public enum SeasonType
        {
            dvd,
            aired
        }

        public readonly Dictionary<int,Episode> Episodes;
        public readonly int SeasonId;
        public readonly int SeasonNumber;
        public readonly SeriesInfo TheSeries;
        private readonly SeasonType type;

        public Season(SeriesInfo theSeries, int number, int seasonid, SeasonType t)
        {
            TheSeries = theSeries;
            SeasonNumber = number;
            SeasonId = seasonid;
            Episodes = new Dictionary<int, Episode>();
            type = t;
        }

        // ReSharper disable once InconsistentNaming
        [NotNull]
        public static string UISeasonWord(int season)
        {
            if ((TVSettings.Instance.defaultSeasonWord.Length > 1) && (TVSettings.Instance.LeadingZeroOnSeason))
            {
                return TVSettings.Instance.defaultSeasonWord + " " + season.ToString("00");
            }

            if (TVSettings.Instance.defaultSeasonWord.Length > 1)
            {
                return TVSettings.Instance.defaultSeasonWord + " " + season;
            }

            if (TVSettings.Instance.LeadingZeroOnSeason)
            {
                return TVSettings.Instance.defaultSeasonWord + season.ToString("00");
            }

            return TVSettings.Instance.defaultSeasonWord + season;
        }

        // ReSharper disable once InconsistentNaming
        public static string UIFullSeasonWord(int snum)
        {
            return (snum == 0)
                ? TVSettings.Instance.SpecialsFolderName
                : UISeasonWord(snum);
        }

        public SeasonStatus Status(TimeZoneInfo tz)
        {
            if (!HasEpisodes)
            {
                return SeasonStatus.noEpisodes;
            }

            if (HasAiredEpisodes(tz) && !HasUnairedEpisodes(tz))
            {
                return SeasonStatus.aired;
            }

            if (HasAiredEpisodes(tz) && HasUnairedEpisodes(tz))
            {
                return SeasonStatus.partiallyAired;
            }

            if (!HasAiredEpisodes(tz) && HasUnairedEpisodes(tz))
            {
                return SeasonStatus.noneAired;
            }

            // Can happen if a Season has Episodes WITHOUT Airdates. 
            return SeasonStatus.noEpisodes;
        }

        internal int MinYear()
        {
            int min = 9999;

            foreach (Episode e in Episodes.Values)
            {
                DateTime? adt = e.GetAirDateDt();
                if (!adt.HasValue)
                {
                    continue;
                }

                DateTime airDateTime = adt.Value;
                if (airDateTime.Year < min)
                {
                    min = airDateTime.Year;
                }
            }

            return min;
        }

        internal int MaxYear()
        {
            int max = 0;

            foreach (Episode e in Episodes.Values)
            {
                DateTime? adt = e.GetAirDateDt();
                if (!adt.HasValue)
                {
                    continue;
                }

                DateTime airDateTime = adt.Value;
                if (airDateTime.Year > max)
                {
                    max = airDateTime.Year;
                }
            }
            return max;
        }

        private bool HasEpisodes => Episodes != null && Episodes.Count > 0;

        public int SeasonIndex => TheSeries.GetSeasonIndex(SeasonNumber,type);

        private bool HasUnairedEpisodes(TimeZoneInfo tz)
        {
            if (!HasEpisodes)
            {
                return false;
            }

            foreach (Episode e in Episodes.Values)
            {
                DateTime? adt = e.GetAirDateDt(tz);
                if (!adt.HasValue)
                {
                    continue;
                }

                DateTime airDateTime = adt.Value;
                if (airDateTime > DateTime.Now)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasAiredEpisodes(TimeZoneInfo tz)
        {
            if (!HasEpisodes)
            {
                return false;
            }

            foreach (Episode e in Episodes.Values)
            {
                DateTime? adt = e.GetAirDateDt(tz);
                if (!adt.HasValue)
                {
                    continue;
                }

                DateTime airDateTime = adt.Value;
                if (airDateTime < DateTime.Now)
                {
                    return true;
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
                if (!episodeAirDate.HasValue)
                {
                    continue;
                }

                //ignore episode if it's in the future
                if (DateTime.Compare(episodeAirDate.Value.ToUniversalTime(), DateTime.UtcNow) > 0)
                {
                    continue;
                }

                //If we don't have a best offer yet
                if (!returnValue.HasValue)
                {
                    returnValue = episodeAirDate.Value;
                }
                //else the currently tested date is better than the current value
                else if (DateTime.Compare(episodeAirDate.Value, returnValue.Value) > 0)
                {
                    returnValue = episodeAirDate.Value;
                }
            }

            return returnValue;
        }

        public string GetBannerPath() => TheSeries.GetSeasonBannerPath(SeasonNumber);

        public string GetWideBannerPath() => TheSeries.GetSeasonWideBannerPath(SeasonNumber);

        public void AddUpdateEpisode([NotNull] Episode newEpisode)
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
                if (dvdOrder && ep.DvdEpNum == episodeNumber)
                {
                    return true;
                }

                if (!dvdOrder && ep.AiredEpNum == episodeNumber)
                {
                    return true;
                }
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

        public bool IsSpecial() => (SeasonNumber == 0);
    }
}
