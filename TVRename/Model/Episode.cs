//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename
{
    public class Episode
    {
        public bool Dirty;
        public int AiredEpNum;
        public int DvdEpNum;
        public int? DvdChapter;
        public string? DvdDiscId;
        public int EpisodeId;
        public DateTime? FirstAired;
        public string? Overview;
        public string? LinkUrl;
        public string? Runtime;
        public string? EpisodeRating;
        public string? EpisodeGuestStars;
        public string? EpisodeDirector;
        public string? Writer;
        public int? AirsBeforeSeason;
        public int? AirsBeforeEpisode;
        public int? AirsAfterSeason;
        public int? SiteRatingCount;
        public int? AbsoluteNumber;

        public string? ProductionCode;
        public string? ImdbCode;
        public string? ShowUrl;
        public string? Filename;

        protected internal int ReadAiredSeasonNum; // only use after loading to attach to the correct season!
        public int ReadDvdSeasonNum; // only use after loading to attach to the correct season!
        public int SeasonId;
        public int SeriesId;
        public long SrvLastUpdated;

        private CachedSeriesInfo? internalSeries;

        internal string? MName;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public DateTime? AirStamp;
        public DateTime? AirTime;

        // ReSharper disable once FunctionComplexityOverflow
        protected Episode([NotNull] Episode o)
        {
            EpisodeId = o.EpisodeId;
            SeriesId = o.SeriesId;
            AiredEpNum = o.AiredEpNum;
            DvdEpNum = o.DvdEpNum;
            FirstAired = o.FirstAired;
            SrvLastUpdated = o.SrvLastUpdated;
            Overview = o.Overview;
            Runtime = o.Runtime;
            LinkUrl = o.LinkUrl;
            EpisodeRating = o.EpisodeRating;
            EpisodeGuestStars = o.EpisodeGuestStars;
            EpisodeDirector = o.EpisodeDirector;
            Writer = o.Writer;
            MName = o.MName;
            internalSeries = o.TheCachedSeries;
            SeasonId = o.SeasonId;
            Dirty = o.Dirty;
            DvdChapter = o.DvdChapter;
            DvdDiscId = o.DvdDiscId;
            AirsBeforeEpisode = o.AirsBeforeEpisode;
            AirsBeforeSeason = o.AirsBeforeSeason;
            AirsAfterSeason = o.AirsAfterSeason;
            SiteRatingCount = o.SiteRatingCount;
            AbsoluteNumber = o.AbsoluteNumber;
            ProductionCode = o.ProductionCode;
            ImdbCode = o.ImdbCode;
            ShowUrl = o.ShowUrl;
            Filename = o.Filename;
            ReadAiredSeasonNum = o.ReadAiredSeasonNum;
            ReadDvdSeasonNum = o.ReadDvdSeasonNum;
            AirStamp = o.AirStamp;
            AirTime = o.AirTime;
        }

        public LocalDateTime? GetAirDateDt()
        {
            if (FirstAired is null || internalSeries is null)
            {
                return null;
            }

            DateTime fa = (DateTime)FirstAired;
            DateTime? airs = internalSeries.AirsTime;

            return new LocalDateTime(fa.Year, fa.Month, fa.Day, airs?.Hour ?? 20, airs?.Minute ?? 0);
        }

        public DateTime? GetAirDateDt(DateTimeZone tz)
        {
            LocalDateTime? dt = GetAirDateDt();
            if (dt is null)
            {
                return null;
            }

            return TimeZoneHelper.AdjustTzTimeToLocalTime(dt.Value, tz);
        }

        public Episode(int seriesId, JObject? bestLanguageR, [NotNull] JObject jsonInDefaultLang, CachedSeriesInfo si) : this(seriesId, si)
        {
            if (bestLanguageR is null)
            {
                LoadJson(jsonInDefaultLang);
            }
            else
            {
                //Here we have two pieces of JSON. One in local language and one in the default language (English).
                //We will populate with the best language first and then fill in any gaps with the backup Language
                LoadJson(bestLanguageR);

                //backupLanguageR should be a cachedSeries of name/value pairs (ie a JArray of JProperties)
                //TVDB asserts that name and overview are the fields that are localised

                string? epName = (string)jsonInDefaultLang["episodeName"];
                if (string.IsNullOrWhiteSpace(MName) && epName != null)
                {
                    MName = System.Web.HttpUtility.HtmlDecode(epName).Trim();
                }

                string overviewFromJson = (string)jsonInDefaultLang["overview"];
                if (string.IsNullOrWhiteSpace(Overview) && overviewFromJson != null)
                {
                    Overview = System.Web.HttpUtility.HtmlDecode(overviewFromJson).Trim();
                }
            }
        }

        public Episode(int seriesId, [NotNull] JObject r, CachedSeriesInfo si) : this(seriesId, si)
        {
            // <Episode>
            //  <id>...</id>
            //  blah blah
            // </Episode>

            LoadJson(r);
        }

        public Episode(int seriesId, CachedSeriesInfo si) :this()
        {
            internalSeries = si;
            SeriesId = seriesId;
        }

        public Episode()
        {
            Overview = string.Empty;
            EpisodeRating = string.Empty;
            EpisodeGuestStars = string.Empty;
            EpisodeDirector = string.Empty;
            Writer = string.Empty;
            MName = string.Empty;
            EpisodeId = -1;
            ReadAiredSeasonNum = -1;
            ReadDvdSeasonNum = -1;
            AiredEpNum = -1;
            DvdEpNum = -1;
            FirstAired = null;
            SrvLastUpdated = -1;
            Dirty = false;
        }

        private void LoadJson([NotNull] JObject r)
        {
            try
            {
                EpisodeId = (int)r["id"];

                if (EpisodeId == 0)
                {
                    return;
                }

                if ((string)r["airedSeasonID"] != null)
                {
                    SeasonId = (int)r["airedSeasonID"];
                }
                else
                {
                    Logger.Error("Issue with episode (loadJSON) " + EpisodeId + " no airedSeasonID ");
                    Logger.Error(r.ToString());
                }

                AiredEpNum = (int)r["airedEpisodeNumber"];

                SrvLastUpdated = (long)r["lastUpdated"];
                Overview = System.Web.HttpUtility.HtmlDecode((string)r["overview"])?.Trim();
                EpisodeRating = GetString(r, "siteRating");
                MName = System.Web.HttpUtility.HtmlDecode((string)r["episodeName"]);

                AirsBeforeEpisode = (int?)r["airsBeforeEpisode"];
                AirsBeforeSeason = (int?)r["airsBeforeSeason"];
                AirsAfterSeason = (int?)r["airsAfterSeason"];
                SiteRatingCount = (int?)r["siteRatingCount"];
                AbsoluteNumber = (int?)r["absoluteNumber"];
                Filename = GetString(r, "filename");
                ImdbCode = GetString(r, "imdbId");
                ShowUrl = GetString(r, "showUrl");
                ProductionCode = GetString(r, "productionCode");
                DvdChapter = (int?)r["dvdChapter"];
                DvdDiscId = GetString(r, "dvdDiscid");

                string sn = (string)r["airedSeason"];
                if (sn is null)
                {
                    Logger.Error("Issue with episode " + EpisodeId + " airedSeason = null");
                    Logger.Error(r.ToString());
                }
                else
                {
                    int.TryParse(sn, out ReadAiredSeasonNum);
                }

                DvdEpNum = r.ExtractStringToInt("dvdEpisodeNumber");
                ReadDvdSeasonNum = r.ExtractStringToInt("dvdSeason");

                EpisodeGuestStars = r["guestStars"].Flatten("|");
                EpisodeDirector = r["directors"].Flatten("|");
                Writer = r["writers"].Flatten("|");

                FirstAired = JsonHelper.ParseFirstAired((string)r["firstAired"]);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to parse : {r}");
            }
        }

        private static string? GetString([NotNull] JObject jObject, [NotNull] string key) => ((string)jObject[key])?.Trim();

        [NotNull]
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(MName))
                {
                    return "Aired Episode " + AiredEpNum;
                }

                return MName;
            }
            set => MName = System.Web.HttpUtility.HtmlDecode(value);
        }

        public int AiredSeasonNumber
        {
            get => ReadAiredSeasonNum;
            set => ReadAiredSeasonNum = value;
        }

        public int DvdSeasonNumber => ReadDvdSeasonNum;

        public bool SameAs([NotNull] Episode o) => EpisodeId == o.EpisodeId;

        [NotNull]
        public IEnumerable<string> GuestStars => string.IsNullOrEmpty(EpisodeGuestStars) ? new string[] { } : EpisodeGuestStars.Split('|').Where(s => s.HasValue());

        [NotNull]
        public IEnumerable<string> Writers => string.IsNullOrEmpty(Writer) ? new string[] { } : Writer.Split('|').Where(s => s.HasValue());

        [NotNull]
        public IEnumerable<string> Directors => string.IsNullOrEmpty(EpisodeDirector) ? new string[] { } : EpisodeDirector.Split('|').Where(s => s.HasValue());

        [NotNull]
        public CachedSeriesInfo TheCachedSeries
        {
            get
            {
                if (internalSeries != null)
                {
                    return internalSeries;
                }

                throw new InvalidOperationException("Attempt to access Series for an Episode that has yet to be set");
            }
        }

        public bool Ok()
        {
            bool returnVal = EpisodeId != -1 && AiredEpNum != -1 && SeasonId != -1 && ReadAiredSeasonNum != -1;

            if (!returnVal)
            {
                Logger.Error("Issue with episode " + EpisodeId + " for cachedSeries " + SeriesId + " for EpNum " + AiredEpNum +
                            " for SeasonID " + SeasonId + " for ReadSeasonNum " + ReadAiredSeasonNum +
                            " for DVDSeasonNum " + ReadDvdSeasonNum);
            }

            return returnVal;
        }

        public void SetSeriesSeason(CachedSeriesInfo ser)
        {
            internalSeries = ser;
        }

        public int GetSeasonNumber(ProcessedSeason.SeasonType order)
        {
            return order switch
            {
                ProcessedSeason.SeasonType.dvd => DvdSeasonNumber,
                ProcessedSeason.SeasonType.aired => AiredSeasonNumber,
                ProcessedSeason.SeasonType.alternate => AiredSeasonNumber,
                _ => throw new ArgumentOutOfRangeException(nameof(order), order, null)
            };
        }

        public int GetEpisodeNumber(ProcessedSeason.SeasonType order)
        {
            return order switch
            {
                ProcessedSeason.SeasonType.dvd => DvdEpNum,
                ProcessedSeason.SeasonType.aired => AiredEpNum,
                ProcessedSeason.SeasonType.alternate => AiredEpNum,
                _ => throw new ArgumentOutOfRangeException(nameof(order), order, null)
            };
        }

        [NotNull]
        public IEnumerable<Actor> AllActors([NotNull] CachedSeriesInfo si)
        {
            List<Actor> returnValue = si.GetActors().ToList();
            foreach (string star in GuestStars)
            {
                if (si.GetActors().All(actor => actor.ActorName != star))
                {
                    returnValue.Add(new Actor(star));
                }
            }

            return returnValue;
        }

        public void SetWriters([NotNull] List<string> writers)
        {
            if (writers.HasAny())
            {
                Writer = string.Join("|", writers);
            }
        }

        public void SetDirectors([NotNull] List<string> directors)
        {
            if (directors.HasAny())
            {
                EpisodeDirector = string.Join("|", directors);
            }
        }

        public bool HasAired()
        {
            LocalDateTime? dateTime = GetAirDateDt();
            return dateTime.HasValue && dateTime.Value.InUtc().ToInstant().CompareTo(SystemClock.Instance.GetCurrentInstant()) < 0;
        }

        public bool IsSpecial(ProcessedSeason.SeasonType seasonOrderType)
        {
            switch (seasonOrderType)
            {
                case ProcessedSeason.SeasonType.dvd:
                    return 0 == DvdSeasonNumber;
                case ProcessedSeason.SeasonType.aired:
                case ProcessedSeason.SeasonType.absolute:
                case ProcessedSeason.SeasonType.alternate:
                    return 0 == AiredSeasonNumber;
                default:
                    throw new ArgumentOutOfRangeException(nameof(seasonOrderType), seasonOrderType, null);
            }
        }
    }
}
