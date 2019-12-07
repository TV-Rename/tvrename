using System;
using System.Collections.Generic;
using JetBrains.Annotations;

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

        public enum ProcessedEpisodeType
        {
            single,
            split,
            merged
        }

        public ProcessedEpisode(SeriesInfo ser, Season airseas, Season dvdseas, [NotNull] ShowItem si)
            : base(ser, airseas, dvdseas)
        {
            NextToAir = false;
            OverallNumber = -1;
            Ignore = false;
            EpNum2 = si.DvdOrder ? DvdEpNum : AiredEpNum;
            Show = si;
            Type = ProcessedEpisodeType.single;
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
        }

        public ProcessedEpisode([NotNull] Episode e, [NotNull] ShowItem si)
            : base(e)
        {
            OverallNumber = -1;
            NextToAir = false;
            EpNum2 = si.DvdOrder ? DvdEpNum : AiredEpNum;
            Ignore = false;
            Show = si;
            Type = ProcessedEpisodeType.single;
        }

        public ProcessedEpisode([NotNull] Episode e, [NotNull] ShowItem si, ProcessedEpisodeType t)
            : base(e)
        {
            OverallNumber = -1;
            NextToAir = false;
            EpNum2 = si.DvdOrder ? DvdEpNum : AiredEpNum;
            Ignore = false;
            Show = si;
            Type = t;
        }

        public ProcessedEpisode([NotNull] Episode e, [NotNull] ShowItem si, List<Episode> episodes)
            : base(e)
        {
            OverallNumber = -1;
            NextToAir = false;
            EpNum2 = si.DvdOrder ? DvdEpNum : AiredEpNum;
            Ignore = false;
            Show = si;
            SourceEpisodes = episodes;
            Type = ProcessedEpisodeType.merged;
        }

        public int AppropriateSeasonNumber => Show.DvdOrder ? DvdSeasonNumber : AiredSeasonNumber;
        public int AppropriateSeasonIndex => Show.DvdOrder ? DvdSeasonIndex : AiredSeasonIndex;

        public Season AppropriateSeason => Show.DvdOrder ? TheDvdSeason : TheAiredSeason;

        public int AppropriateEpNum
        {
            get => Show.DvdOrder ? DvdEpNum : AiredEpNum;
            set
            {
                if (Show.DvdOrder)
                {
                    DvdEpNum = value;
                }
                else
                {
                    AiredEpNum = value;
                }
            }
        }

        public bool PreviouslySeen => TVSettings.Instance.PreviouslySeenEpisodes.Contains(EpisodeId);

        [NotNull]
        public string SeasonNumberAsText => AppropriateSeasonNumber != 0 ? AppropriateSeasonNumber.ToString() : "Special";

        [NotNull]
        public string NumsAsString()
        {
            if (AppropriateEpNum == EpNum2)
            {
                return AppropriateEpNum.ToString();
            }
            else
            {
                return AppropriateEpNum + "-" + EpNum2;
            }
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
            if (!inLocalTime)
            {
                return GetAirDateDt();
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
            else
            {
                int h = ts.Hours;
                if (ts.TotalHours >= 1)
                {
                    if (ts.Minutes >= 30)
                    {
                        h += 1;
                    }

                    return ts.Days + "d " + h + "h";
                }
                else
                {
                    return Math.Round(ts.TotalMinutes) + "min";
                }
            }
        }

        [NotNull]
        public string DayOfWeek()
        {
            DateTime? dt = GetAirDateDt(true);
            return (dt != null) ? dt.Value.ToString("ddd") : "-";
        }

        [NotNull]
        public string TimeOfDay()
        {
            DateTime? dt = GetAirDateDt(true);
            return (dt != null) ? dt.Value.ToString("t") : "-";
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
            if ((dt is null) || (dt.Value.CompareTo(DateTime.MaxValue) == 0))
            {
                return false;
            }

            TimeSpan ts = dt.Value.Subtract(DateTime.Now);
            return (ts.TotalHours >= (-24 * days)) && (ts.TotalHours <= 0);
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
    }
}
