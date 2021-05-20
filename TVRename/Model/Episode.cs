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
using System.Xml;
using System.Xml.Linq;

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

        protected int ReadAiredSeasonNum; // only use after loading to attach to the correct season!
        public int ReadDvdSeasonNum; // only use after loading to attach to the correct season!
        public int SeasonId;
        public int SeriesId;
        public long SrvLastUpdated;

        private CachedSeriesInfo? internalSeries;

        private string? mName;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public DateTime? AirStamp;
        public DateTime? AirTime;

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
            mName = o.mName;
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

        public Episode([NotNull] XElement r)
        {
            // <Episode>
            //  <id>...</id>
            //  blah blah
            // </Episode>
            SeriesId = r.ExtractInt("seriesid", -1); // key to the cachedSeries

            EpisodeId = r.ExtractInt("id", -1);
            SeasonId = r.ExtractInt("airedSeasonID") ?? r.ExtractInt("seasonid", -1);
            AiredEpNum = r.ExtractInt("airedEpisodeNumber") ?? r.ExtractInt("EpisodeNumber", -1);
            DvdEpNum = ExtractAndParse(r, "dvdEpisodeNumber");
            ReadAiredSeasonNum = ExtractAndParse(r, "SeasonNumber");
            ReadDvdSeasonNum = ExtractAndParse(r, "dvdSeason");
            SrvLastUpdated = r.ExtractLong("lastupdated", -1);
            Overview = System.Web.HttpUtility.HtmlDecode(XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("Overview")));
            LinkUrl = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("LinkURL"));
            Runtime = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("Runtime"));
            EpisodeRating = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("Rating"));
            EpisodeGuestStars = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("GuestStars"));
            EpisodeDirector = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("EpisodeDirector"));
            Writer = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("Writer"));
            mName = System.Web.HttpUtility.HtmlDecode(XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("EpisodeName")));

            AirStamp = r.ExtractDateTime("AirStamp");
            AirTime = JsonHelper.ParseAirTime(r.ExtractString("Airs_Time"));

            DvdDiscId = r.ExtractString("DvdDiscId");
            Filename = r.ExtractStringOrNull("Filename") ?? r.ExtractString("filename");
            ShowUrl = r.ExtractStringOrNull("ShowUrl") ?? r.ExtractString("showUrl");
            ImdbCode = r.ExtractStringOrNull("ImdbCode") ?? r.ExtractStringOrNull("IMDB_ID") ?? r.ExtractString("imdbId");
            ProductionCode = r.ExtractStringOrNull("ProductionCode") ?? r.ExtractString("productionCode");

            DvdChapter = r.ExtractInt("DvdChapter") ?? r.ExtractInt("dvdChapter");
            AirsBeforeSeason = r.ExtractInt("AirsBeforeSeason") ?? r.ExtractInt("airsBeforeSeason") ?? r.ExtractInt("airsbefore_season");
            AirsBeforeEpisode = r.ExtractInt("AirsBeforeEpisode") ?? r.ExtractInt("airsBeforeEpisode") ?? r.ExtractInt("airsbefore_episode");
            AirsAfterSeason = r.ExtractInt("AirsAfterSeason");
            SiteRatingCount = r.ExtractInt("SiteRatingCount") ?? r.ExtractInt("siteRatingCount");
            AbsoluteNumber = r.ExtractInt("AbsoluteNumber");
            FirstAired = JsonHelper.ParseFirstAired(r.ExtractString("FirstAired"));
        }

        private static int ExtractAndParse([NotNull] XElement r, string key)
        {
            string value = r.ExtractString(key);
            int.TryParse(value, out int intValue);
            return intValue;
        }

        public Episode(int seriesId, JObject? bestLanguageR, JObject jsonInDefaultLang, CachedSeriesInfo si) : this(seriesId, si)
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
                if (string.IsNullOrWhiteSpace(mName) && epName != null)
                {
                    mName = System.Web.HttpUtility.HtmlDecode(epName).Trim();
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

        public Episode(int seriesId, CachedSeriesInfo si)
        {
            internalSeries = si;
            SeriesId = seriesId;

            Overview = string.Empty;
            EpisodeRating = string.Empty;
            EpisodeGuestStars = string.Empty;
            EpisodeDirector = string.Empty;
            Writer = string.Empty;
            mName = string.Empty;
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
                mName = System.Web.HttpUtility.HtmlDecode((string)r["episodeName"]);

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
                if (string.IsNullOrEmpty(mName))
                {
                    return "Aired Episode " + AiredEpNum;
                }

                return mName;
            }
            set => mName = System.Web.HttpUtility.HtmlDecode(value);
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

        public void WriteXml([NotNull] XmlWriter writer)
        {
            writer.WriteStartElement("Episode");

            writer.WriteElement("id", EpisodeId);
            writer.WriteElement("seriesid", SeriesId);
            writer.WriteElement("airedSeasonID", SeasonId);
            writer.WriteElement("airedEpisodeNumber", AiredEpNum);
            writer.WriteElement("SeasonNumber", AiredSeasonNumber);
            writer.WriteElement("dvdEpisodeNumber", DvdEpNum, true);
            writer.WriteElement("dvdSeason", DvdSeasonNumber, true);
            writer.WriteElement("lastupdated", SrvLastUpdated);
            writer.WriteElement("Overview", Overview?.Trim());
            writer.WriteElement("LinkURL", LinkUrl?.Trim());
            writer.WriteElement("Runtime", Runtime?.Trim());
            writer.WriteElement("Rating", EpisodeRating);
            writer.WriteElement("GuestStars", EpisodeGuestStars, true);
            writer.WriteElement("EpisodeDirector", EpisodeDirector, true);
            writer.WriteElement("Writer", Writer, true);
            writer.WriteElement("EpisodeName", mName, true);

            writer.WriteElement("FirstAired", FirstAired?.ToString("yyyy-MM-dd"), true);
            writer.WriteElement("AirTime", AirTime?.ToString("HH:mm"), true);
            writer.WriteElement("AirTime", AirStamp);

            writer.WriteElement("DvdChapter", DvdChapter);
            writer.WriteElement("DvdDiscId", DvdDiscId, true);
            writer.WriteElement("AirsBeforeSeason", AirsBeforeSeason);
            writer.WriteElement("AirsBeforeEpisode", AirsBeforeEpisode);
            writer.WriteElement("AirsAfterSeason", AirsAfterSeason);
            writer.WriteElement("SiteRatingCount", SiteRatingCount);
            writer.WriteElement("AbsoluteNumber", AbsoluteNumber);
            writer.WriteElement("ProductionCode", ProductionCode, true);
            writer.WriteElement("ImdbCode", ImdbCode, true);
            writer.WriteElement("ShowUrl", ShowUrl, true);
            writer.WriteElement("Filename", Filename, true);

            writer.WriteEndElement(); //Episode
        }

        public int GetSeasonNumber(ProcessedSeason.SeasonType order)
        {
            return order switch
            {
                ProcessedSeason.SeasonType.dvd => DvdSeasonNumber,
                ProcessedSeason.SeasonType.aired => AiredSeasonNumber,
                _ => throw new ArgumentOutOfRangeException(nameof(order), order, null)
            };
        }

        public int GetEpisodeNumber(ProcessedSeason.SeasonType order)
        {
            return order switch
            {
                ProcessedSeason.SeasonType.dvd => DvdEpNum,
                ProcessedSeason.SeasonType.aired => AiredEpNum,
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
            if (writers.Any())
            {
                Writer = string.Join("|", writers);
            }
        }

        public void SetDirectors([NotNull] List<string> directors)
        {
            if (directors.Any())
            {
                EpisodeDirector = string.Join("|", directors);
            }
        }
    }
}
