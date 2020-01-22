// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;
using NodaTime;

namespace TVRename
{
    public class Episode
    {
        public bool Dirty;
        public int AiredEpNum;
        public int DvdEpNum;
        public int? DvdChapter;
        public string DvdDiscId;
        public int EpisodeId;
        public DateTime? FirstAired;
        public string Overview;
        public string EpisodeRating;
        public string EpisodeGuestStars;
        public string EpisodeDirector;
        public string Writer;
        public int? AirsBeforeSeason;
        public int? AirsBeforeEpisode;
        public int? AirsAfterSeason;
        public int? SiteRatingCount;
        public int? AbsoluteNumber;

        public string ProductionCode;
        public string ImdbCode;
        public string ShowUrl;
        public string Filename;

        public int ReadAiredSeasonNum; // only use after loading to attach to the correct season!
        public int ReadDvdSeasonNum; // only use after loading to attach to the correct season!
        public int SeasonId;
        public int SeriesId;
        public long SrvLastUpdated;

        public Season TheAiredSeason;
        public Season TheDvdSeason;
        public SeriesInfo TheSeries;
        private string mName;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        protected Episode([NotNull] Episode o)
        {
            EpisodeId = o.EpisodeId;
            SeriesId = o.SeriesId;
            AiredEpNum = o.AiredEpNum;
            DvdEpNum = o.DvdEpNum;
            FirstAired = o.FirstAired;
            SrvLastUpdated = o.SrvLastUpdated;
            Overview = o.Overview;
            EpisodeRating = o.EpisodeRating;
            EpisodeGuestStars = o.EpisodeGuestStars;
            EpisodeDirector = o.EpisodeDirector;
            Writer = o.Writer;
            mName = o.mName;
            TheAiredSeason = o.TheAiredSeason;
            TheDvdSeason = o.TheDvdSeason;
            TheSeries = o.TheSeries;
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
        }

        protected Episode(SeriesInfo ser, Season airSeason, Season dvdSeason)
        {
            SetDefaults(ser, airSeason, dvdSeason);
        }

        [CanBeNull]
        public LocalDateTime? GetAirDateDt()
        {
            if (FirstAired is null || TheSeries is null)
            {
                return null;
            }

            DateTime fa = (DateTime) FirstAired;
            DateTime? airs = TheSeries.AirsTime;

            return new LocalDateTime(fa.Year, fa.Month, fa.Day, airs?.Hour ?? 20, airs?.Minute ?? 0);
        }

        [CanBeNull]
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

            SetDefaults(null, null, null);
            EpisodeId = r.ExtractInt("id",-1);
            SeriesId = r.ExtractInt("seriesid",-1); // thetvdb series id
            SeasonId = r.ExtractInt("airedSeasonID") ?? r.ExtractInt("seasonid",-1);
            AiredEpNum = r.ExtractInt("airedEpisodeNumber") ?? r.ExtractInt("EpisodeNumber",-1);
            DvdEpNum = ExtractAndParse(r, "dvdEpisodeNumber");
            ReadAiredSeasonNum = ExtractAndParse(r, "SeasonNumber");
            ReadDvdSeasonNum = ExtractAndParse(r,"dvdSeason");
            SrvLastUpdated = r.ExtractLong("lastupdated",-1);
            Overview = System.Web.HttpUtility.HtmlDecode(XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("Overview")));
            EpisodeRating = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("Rating"));
            EpisodeGuestStars = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("GuestStars"));
            EpisodeDirector = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("EpisodeDirector"));
            Writer = XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("Writer"));
            mName = System.Web.HttpUtility.HtmlDecode(XmlHelper.ReadStringFixQuotesAndSpaces(r.ExtractString("EpisodeName")));

            DvdDiscId = r.ExtractString("DvdDiscId");
            Filename = r.ExtractString("Filename") ?? r.ExtractString("filename");
            ShowUrl = r.ExtractString("ShowUrl") ?? r.ExtractString("showUrl");
            ImdbCode = r.ExtractString("ImdbCode") ?? r.ExtractString("IMDB_ID") ?? r.ExtractString("imdbId");
            ProductionCode = r.ExtractString("ProductionCode") ?? r.ExtractString("productionCode");

            DvdChapter = r.ExtractInt("DvdChapter") ?? r.ExtractInt("dvdChapter");
            AirsBeforeSeason = r.ExtractInt("AirsBeforeSeason") ?? r.ExtractInt("airsBeforeSeason") ?? r.ExtractInt("airsbefore_season");
            AirsBeforeEpisode = r.ExtractInt("AirsBeforeEpisode") ?? r.ExtractInt("airsBeforeEpisode") ?? r.ExtractInt("airsbefore_episode");
            AirsAfterSeason = r.ExtractInt("AirsAfterSeason");
            SiteRatingCount = r.ExtractInt("SiteRatingCount") ?? r.ExtractInt("siteRatingCount");
            AbsoluteNumber = r.ExtractInt("AbsoluteNumber");
            FirstAired = ParseAired(r.ExtractString("FirstAired"));
        }

        private static int ExtractAndParse([NotNull] XElement r, string key)
        {
            string value = r.ExtractString(key);
            int.TryParse(value, out int intValue);
            return intValue;
        }

        private static DateTime? ParseAired([CanBeNull] string contents)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(contents))
                {
                    return null;
                }
                return DateTime.ParseExact(contents, "yyyy-MM-dd",new System.Globalization.CultureInfo(""));
            }
            catch
            {
                return null;
            }
        }

        public Episode(int seriesId, [NotNull] JObject json, JObject jsonInDefaultLang)
        {
            SetDefaults(null, null, null);
            LoadJson(seriesId, json, jsonInDefaultLang);
        }

        public Episode(int seriesId, [NotNull] JObject r)
        {
            // <Episode>
            //  <id>...</id>
            //  blah blah
            // </Episode>

            SetDefaults(null, null, null);

            LoadJson(seriesId, r);
        }

        private void LoadJson(int seriesId, [NotNull] JObject bestLanguageR, JObject backupLanguageR)
        {
            //Here we have two pieces of JSON. One in local language and one in the default language (English). 
            //We will populate with the best language first and then fill in any gaps with the backup Language
            LoadJson(seriesId, bestLanguageR);

            //backupLanguageR should be a series of name/value pairs (ie a JArray of JProperties)
            //TVDB asserts that name and overview are the fields that are localised

            if (string.IsNullOrWhiteSpace(mName) && (string) backupLanguageR["episodeName"] != null)
            {
                mName = System.Web.HttpUtility.HtmlDecode((string) backupLanguageR["episodeName"])?.Trim();
            }

            if (string.IsNullOrWhiteSpace(Overview) && (string) backupLanguageR["overview"] != null)
            {
                Overview = System.Web.HttpUtility.HtmlDecode((string)backupLanguageR["overview"])?.Trim();
            }
        }

        private void LoadJson(int seriesId, [NotNull] JObject r)
        {
            SeriesId = seriesId;
            try
            {
                EpisodeId = (int) r["id"];

                if(EpisodeId==0)
                {
                    return;
                }

                if ((string) r["airedSeasonID"] != null)
                {
                    SeasonId = (int) r["airedSeasonID"];
                }
                else
                {
                    Logger.Error("Issue with episode (loadJSON) " + EpisodeId + " for series " + seriesId + " no airedSeasonID ");
                    Logger.Error(r.ToString());
                }

                AiredEpNum = (int) r["airedEpisodeNumber"];

                SrvLastUpdated = (long) r["lastUpdated"];
                Overview = System.Web.HttpUtility.HtmlDecode((string)r["overview"])?.Trim();
                EpisodeRating = ((string) r["siteRating"]).Trim();
                mName = System.Web.HttpUtility.HtmlDecode((string)r["episodeName"]);

                AirsBeforeEpisode = (int?)r["airsBeforeEpisode"];
                AirsBeforeSeason = (int?)r["airsBeforeSeason"];
                AirsAfterSeason = (int?)r["airsAfterSeason"];
                SiteRatingCount = (int?)r["siteRatingCount"];
                AbsoluteNumber = (int?)r["absoluteNumber"];
                Filename = ((string)r["filename"]).Trim();
                ImdbCode = ((string)r["imdbId"]).Trim();
                ShowUrl = ((string)r["showUrl"]).Trim();
                ProductionCode = ((string)r["productionCode"]).Trim();
                DvdChapter = (int?)r["dvdChapter"];
                DvdDiscId = ((string)r["dvdDiscid"]).Trim();

                string sn = (string) r["airedSeason"];
                if (sn is null)
                {
                    Logger.Error("Issue with episode " + EpisodeId + " for series " + seriesId + " airedSeason = null");
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

                FirstAired = GetFirstAired(r);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to parse : {r}");
            }
        }

        private static DateTime? GetFirstAired(JObject r)
        {
            try
            {
                string contents = (string) r["firstAired"];
                if (string.IsNullOrEmpty(contents))
                {
                    return null;
                }

                return DateTime.ParseExact(contents, "yyyy-MM-dd",
                    new System.Globalization.CultureInfo(""));
            }
            catch (Exception e)
            {
                Logger.Debug(e, "Failed to parse firstAired");
                return null;
            }
        }

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

        public int AiredSeasonNumber => TheAiredSeason?.SeasonNumber ?? -1;

        public int DvdSeasonNumber => TheDvdSeason?.SeasonNumber ?? -1;

        public int AiredSeasonIndex => TheAiredSeason?.SeasonIndex ?? -1;

        public int DvdSeasonIndex => TheDvdSeason?.SeasonIndex ?? -1;

        public bool SameAs([NotNull] Episode o) => EpisodeId == o.EpisodeId;

        [NotNull]
        public IEnumerable<string> GuestStars => string.IsNullOrEmpty(EpisodeGuestStars) ? new string[] { } : EpisodeGuestStars.Split('|');

        [NotNull]
        public IEnumerable<string> Writers => string.IsNullOrEmpty(Writer) ? new string[] { } : Writer.Split('|');

        [NotNull]
        public IEnumerable<string> Directors => string.IsNullOrEmpty(EpisodeDirector) ? new string[] { } : EpisodeDirector.Split('|');

        public bool Ok()
        {
            bool returnVal = SeriesId != -1 && EpisodeId != -1 && AiredEpNum != -1 && SeasonId != -1 &&
                             ReadAiredSeasonNum != -1;

            if (!returnVal)
            {
                Logger.Error("Issue with episode " + EpisodeId + " for series " + SeriesId + " for EpNum " + AiredEpNum +
                            " for SeasonID " + SeasonId + " for ReadSeasonNum " + ReadAiredSeasonNum +
                            " for DVDSeasonNum " + ReadDvdSeasonNum);
            }

            return returnVal;
        }

        private void SetDefaults(SeriesInfo ser, Season airSeas, Season dvdSeason)
        {
            TheAiredSeason = airSeas;
            TheDvdSeason = dvdSeason;

            TheSeries = ser;

            Overview = "";
            EpisodeRating = "";
            EpisodeGuestStars = "";
            EpisodeDirector = "";
            Writer = "";
            mName = "";
            EpisodeId = -1;
            SeriesId = -1;
            ReadAiredSeasonNum = -1;
            ReadDvdSeasonNum = -1;
            AiredEpNum = -1;
            DvdEpNum = -1;
            FirstAired = null;
            SrvLastUpdated = -1;
            Dirty = false;
        }

        public void SetSeriesSeason(SeriesInfo ser, Season airedSeas, Season dvdSeason)
        {
            TheAiredSeason = airedSeas;
            TheDvdSeason = dvdSeason;
            TheSeries = ser;
        }

        public void WriteXml([NotNull] XmlWriter writer)
        {
            writer.WriteStartElement("Episode");

            writer.WriteElement("id", EpisodeId);
            writer.WriteElement("seriesid", SeriesId);
            writer.WriteElement("airedSeasonID", SeasonId);
            writer.WriteElement("airedEpisodeNumber", AiredEpNum);
            writer.WriteElement("SeasonNumber", AiredSeasonNumber);
            writer.WriteElement("dvdEpisodeNumber", DvdEpNum,true);
            writer.WriteElement("dvdSeason", DvdSeasonNumber, true);
            writer.WriteElement("lastupdated", SrvLastUpdated);
            writer.WriteElement("Overview", Overview?.Trim());
            writer.WriteElement("Rating", EpisodeRating);
            writer.WriteElement("GuestStars", EpisodeGuestStars, true);
            writer.WriteElement("EpisodeDirector", EpisodeDirector, true);
            writer.WriteElement("Writer", Writer, true);
            writer.WriteElement("EpisodeName", mName, true);

            if (FirstAired != null)
            {
                writer.WriteElement("FirstAired", FirstAired.Value.ToString("yyyy-MM-dd"));
            }

            writer.WriteElement("DvdChapter", DvdChapter);
            writer.WriteElement("DvdDiscId", DvdDiscId,true);
            writer.WriteElement("AirsBeforeSeason", AirsBeforeSeason);
            writer.WriteElement("AirsBeforeEpisode", AirsBeforeEpisode);
            writer.WriteElement("AirsAfterSeason", AirsAfterSeason);
            writer.WriteElement("SiteRatingCount", SiteRatingCount);
            writer.WriteElement("AbsoluteNumber", AbsoluteNumber);
            writer.WriteElement("ProductionCode", ProductionCode, true);
            writer.WriteElement("ImdbCode", ImdbCode,true);
            writer.WriteElement("ShowUrl", ShowUrl,true);
            writer.WriteElement("Filename", Filename, true);

            writer.WriteEndElement(); //Episode
        }
    }
}
