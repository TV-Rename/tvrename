using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NodaTime;

namespace TVRename
{
    public class ProcessedEpisode : Episode
    {
        public int
            EpNum2; // if we are a concatenation of episodes, this is the last one in the series. Otherwise, same as EpNum

        public bool Ignore;
        public bool NextToAir;
        public int OverallNumber;
        public readonly ShowItem Show;
        public readonly ProcessedEpisodeType Type;
        public readonly List<Episode> SourceEpisodes;
        public Season TheAiredSeason;
        public Season TheDvdSeason;

        public enum ProcessedEpisodeType
        {
            single,
            split,
            merged
        }

        public ProcessedEpisode([NotNull] ProcessedEpisode o)
            : base(o)
        {
            NextToAir = o.NextToAir;
            EpNum2 = o.EpNum2;
            Ignore = o.Ignore;
            Show = o.Show;
            OverallNumber = o.OverallNumber;
            Type = o.Type;
            TheAiredSeason = o.TheAiredSeason;
            TheDvdSeason = o.TheDvdSeason;
        }

        public ProcessedEpisode([NotNull] Episode e, [NotNull] ShowItem si)
            : base(e)
        {
            OverallNumber = -1;
            NextToAir = false;
            Ignore = false;
            Show = si;
            EpNum2 = Show.Order == Season.SeasonType.dvd ? DvdEpNum : AiredEpNum;
            Type = ProcessedEpisodeType.single;
            TheAiredSeason=si.GetOrAddAiredSeason(e.AiredSeasonNumber,e.SeasonId);
            TheDvdSeason = si.GetOrAddDvdSeason(e.DvdSeasonNumber, e.SeasonId);
        }

        public ProcessedEpisode([NotNull] ProcessedEpisode e, [NotNull] ShowItem si, ProcessedEpisodeType t)
            : base(e)
        {
            OverallNumber = -1;
            NextToAir = false;
            Show = si;
            EpNum2 = Show.Order == Season.SeasonType.dvd ? DvdEpNum : AiredEpNum;
            Ignore = false;
            Type = t;
            TheAiredSeason = e.TheAiredSeason;
            TheDvdSeason = e.TheDvdSeason;
        }

        public ProcessedEpisode([NotNull] ProcessedEpisode e, [NotNull] ShowItem si, List<Episode> episodes)
            : base(e)
        {
            OverallNumber = -1;
            NextToAir = false;
            Show = si;
            EpNum2 = Show.Order == Season.SeasonType.dvd ? DvdEpNum : AiredEpNum;
            Ignore = false;
            SourceEpisodes = episodes;
            Type = ProcessedEpisodeType.merged;
            TheAiredSeason = e.TheAiredSeason;
            TheDvdSeason = e.TheDvdSeason;
        }

        public ProcessedEpisode([NotNull] ProcessedEpisode pe, ShowItem si, [NotNull] string name, int airedEpNum, int dvdEpNum, int epNum2) : base(pe.TheSeries)
        {
            NextToAir = false;
            OverallNumber = -1;
            Ignore = false;
            Name = name;
            AiredEpNum = airedEpNum;
            DvdEpNum = dvdEpNum;
            EpNum2 = epNum2;
            Show = si;
            Type = ProcessedEpisodeType.single;
            TheAiredSeason = pe.TheAiredSeason;
            TheDvdSeason = pe.TheDvdSeason;
            ReadDvdSeasonNum = TheDvdSeason.SeasonNumber;
            ReadAiredSeasonNum = TheDvdSeason.SeasonNumber;
        }

        public int AppropriateSeasonNumber => Show.Order==Season.SeasonType.dvd ? DvdSeasonNumber : AiredSeasonNumber;
        public int AppropriateSeasonIndex => Show.GetSeasonIndex(AppropriateSeasonNumber);
        public Season AppropriateSeason => Show.Order == Season.SeasonType.dvd ? TheDvdSeason : TheAiredSeason;
        public int AppropriateEpNum => Show.Order == Season.SeasonType.dvd ? DvdEpNum : AiredEpNum;

        public bool PreviouslySeen => TVSettings.Instance.PreviouslySeenEpisodes.Contains(EpisodeId);

        [NotNull]
        public string SeasonNumberAsText => AppropriateSeasonNumber != 0 ? AppropriateSeasonNumber.ToString() : "Special";

        [NotNull]
        public string EpNumsAsString()
        {
            if (AppropriateEpNum == EpNum2)
            {
                return AppropriateEpNum.ToString();
            }

            return AppropriateEpNum + "-" + EpNum2;
        }

        public static int EpNumberSorter([NotNull] ProcessedEpisode e1, [NotNull] ProcessedEpisode e2)
        {
            int ep1 = e1.AiredEpNum;
            int ep2 = e2.AiredEpNum;

            return ep1 - ep2;
        }

        // ReSharper disable once InconsistentNaming
        public static int DVDOrderSorter([NotNull] ProcessedEpisode e1, [NotNull] ProcessedEpisode e2)
        {
            int ep1 = e1.DvdEpNum;
            int ep2 = e2.DvdEpNum;

            return ep1 - ep2;
        }

        public DateTime? GetAirDateDt(bool inLocalTime)
        {
            LocalDateTime? x = GetAirDateDt();

            if (!inLocalTime && x.HasValue)
            {
                return x.Value.ToDateTimeUnspecified();
            }

            if (!inLocalTime)
            {
                return null;
            }

            // do timezone adjustment
            return GetAirDateDt(Show.GetTimeZone());
        }

        [NotNull]
        public string HowLong()
        {
            DateTime? airsdt = GetAirDateDt(true);
            if (airsdt is null)
            {
                return "";
            }

            DateTime dt = (DateTime) airsdt;

            TimeSpan ts = dt.Subtract(DateTime.Now); // how long...
            if (ts.TotalHours < 0)
            {
                return "Aired";
            }

            int h = ts.Hours;
            if (ts.TotalHours >= 1)
            {
                if (ts.Minutes >= 30)
                {
                    h += 1;
                }

                return ts.Days + "d " + h + "h";
            }

            return Math.Round(ts.TotalMinutes) + "min";
        }

        [NotNull]
        public string DayOfWeek()
        {
            DateTime? dt = GetAirDateDt(true);
            return dt != null ? dt.Value.ToString("ddd") : "-";
        }

        [NotNull]
        public string TimeOfDay()
        {
            DateTime? dt = GetAirDateDt(true);
            return dt != null ? dt.Value.ToString("t") : "-";
        }

        public bool HasAired()
        {
            DateTime? airsdt = GetAirDateDt(true);
            if (airsdt is null)
            {
                return false;
            }

            DateTime dt = (DateTime) airsdt;

            TimeSpan ts = dt.Subtract(DateTime.Now); // how long...
            return ts.TotalHours < 0;
        }

        public bool WithinDays(int days)
        {
            DateTime? dt = GetAirDateDt(true);
            if (dt is null || dt.Value.CompareTo(DateTime.MaxValue) == 0)
            {
                return false;
            }

            TimeSpan ts = dt.Value.Subtract(DateTime.Now);
            return ts.TotalHours >= -24 * days && ts.TotalHours <= 0;
        }

        public bool IsInFuture(bool def)
        {
            DateTime? airsdt = GetAirDateDt(true);
            if (airsdt is null)
            {
                return def;
            }

            DateTime dt = (DateTime)airsdt;

            TimeSpan ts = dt.Subtract(DateTime.Now); // how long...
            return ts.TotalHours > 0;
        }

        public override string ToString() => $"S{AppropriateSeasonNumber:D2}E{AppropriateEpNum:D2} - {Name}";

        public void SetEpisodeNumbers(int startEpisodeNum, int endEpisodeNum)
        {
            if (Show.Order==Season.SeasonType.dvd)
            {
                DvdEpNum = startEpisodeNum;
            }
            else
            {
                AiredEpNum = startEpisodeNum;
            }

            EpNum2 = endEpisodeNum;
        }

        public bool NotOnDvd()
        {
            return DvdEpNum == 0 && DvdChapter is null && string.IsNullOrWhiteSpace(DvdDiscId) && DvdSeasonNumber == 0;
        }
    }
}
