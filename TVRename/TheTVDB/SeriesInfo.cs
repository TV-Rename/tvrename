// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace TVRename
{
    public class SeriesInfo
    {
        public DateTime? AirsTime;
        public bool Dirty; // set to true if local info is known to be older than whats on the server
        public DateTime? FirstAired;
        public readonly string TargetLanguageCode; //The Language Code we'd like the Series in ; null if we want to use the system setting
        public int LanguageId; //The actual language obtained
        public string Name;
        public string Status="Unkonwn";
        private string airsTimeString;
        public string AirsDay;
        public string Network;
        public string Overview;
        public string Runtime;
        public string ContentRating;
        public float SiteRating;
        public int SiteRatingVotes;
        public string Imdb;
        public string Year;
        private string firstAiredString;
        public string SeriesId;
        public string BannerString;
        public bool BannersLoaded;
        public long SrvLastUpdated;
        public int TvdbCode;
        public string TempTimeZone;

        private List<Actor> actors;
        private List<string> genres;
        private List<string> aliases;

        public Dictionary<int, Season> AiredSeasons;
        public Dictionary<int, Season> DvdSeasons;
        private SeriesBanners banners;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public bool UseCustomLanguage => TargetLanguageCode != null;

        public IEnumerable<KeyValuePair<int, Banner>> AllBanners => banners.AllBanners;

        public int MinYear
        {
            get
            {
                int min = 9999;
                foreach (Season s in AiredSeasons.Values)
                {
                    min = Math.Min(min, s.MinYear());
                }
                return min;
            }
        }

        public int MaxYear
        {
            get
            {
                int max = 0;
                foreach (Season s in AiredSeasons.Values)
                {
                    max = Math.Max(max, s.MaxYear());
                }
                return max;
            }
        }

        public DateTime? LastAiredDate
        {
            get
            {
                DateTime? returnValue = null;
                foreach (Season s in AiredSeasons.Values) //We can use AiredSeasons as it does not matter which order we do this in Aired or DVD
                {
                    DateTime? seasonLastAirDate = s.LastAiredDate();

                    if (!seasonLastAirDate.HasValue) continue;

                    if (!returnValue.HasValue) returnValue = seasonLastAirDate.Value;
                    else if (DateTime.Compare(seasonLastAirDate.Value, returnValue.Value) > 0) returnValue = seasonLastAirDate.Value;
                }
                return returnValue;
            }
        }

        // note: "SeriesID" in a <Series> is the tv.com code,
        // "seriesid" in an <Episode> is the tvdb code!

        public SeriesInfo(string name, int id)
        {
            SetToDefauts();
            Name = name;
            TvdbCode = id;
        }

        public SeriesInfo(string name, int id, string langCode)
        {
            SetToDefauts();
            TargetLanguageCode = langCode;
            Name = name;
            TvdbCode = id;
        }

        public SeriesInfo(XElement seriesXml)
        {
            SetToDefauts();
            LoadXml(seriesXml);
        }

        public SeriesInfo(JObject json,int langId)
        {
            SetToDefauts();
            LanguageId = langId;
            LoadJson(json);

            if (string.IsNullOrEmpty(Name))
            {
               Logger.Warn("Issue with series " + TvdbCode );
               Logger.Warn(json.ToString());
            }
        }

        public SeriesInfo(JObject json, JObject jsonInDefaultLang, int langId)
        {
            SetToDefauts();
            LanguageId = langId;
            LoadJson(json,jsonInDefaultLang);
            if (string.IsNullOrEmpty(Name)            ){
               Logger.Warn("Issue with series " + TvdbCode );
               Logger.Warn(json.ToString());
               Logger.Info(jsonInDefaultLang .ToString());
            }
        }

        public IEnumerable<Actor> GetActors() => actors;

        public IEnumerable<string> Aliases() => aliases;

        public IEnumerable<string> GetActorNames() => GetActors().Select(x => x.ActorName);

        private void SetToDefauts()
        {
            AiredSeasons = new Dictionary<int, Season>();
            DvdSeasons = new Dictionary<int, Season>();
            actors=new List<Actor>();
            aliases = new List<string>();
            genres = new List<string>();
            Dirty = false;
            Name = "";
            AirsTime = null;
            TvdbCode = -1;
            LanguageId = -1;
            Status = "Unknown";
            banners = new SeriesBanners(this);
            banners.ResetBanners();
            BannersLoaded = false;
        }

        public void Merge(SeriesInfo o, int preferredLanguageId)
        {
            if (o.TvdbCode != TvdbCode)
                return; // that's not us!
            if (o.SrvLastUpdated != 0 && o.SrvLastUpdated < SrvLastUpdated)
                return; // older!?

            bool currentLanguageNotSet = o.LanguageId == -1;
            bool newLanguageBetter = o.LanguageId == preferredLanguageId && LanguageId != preferredLanguageId;
            bool betterLanguage = currentLanguageNotSet || newLanguageBetter;

            SrvLastUpdated = o.SrvLastUpdated;

            // take the best bits of "o"
            // "o" is always newer/better than us, if there is a choice
            if ((!string.IsNullOrEmpty(o.Name)) && betterLanguage)
                Name = o.Name;

            AirsDay = ChooseBetter(AirsDay, betterLanguage, o.AirsDay);
            Imdb = ChooseBetter(Imdb, betterLanguage, o.Imdb);
            Overview = ChooseBetter(Overview, betterLanguage, o.Overview);
            BannerString = ChooseBetter(BannerString, betterLanguage, o.BannerString);
            Network = ChooseBetter(Network, betterLanguage, o.Network);
            Runtime = ChooseBetter(Runtime, betterLanguage, o.Runtime);
            SeriesId = ChooseBetter(SeriesId, betterLanguage, o.SeriesId);
            Status = ChooseBetter(Status, betterLanguage, o.Status);
            ContentRating = ChooseBetter(ContentRating, betterLanguage, o.ContentRating);

            if (betterLanguage && o.FirstAired.HasValue)
                FirstAired = o.FirstAired;

            if (!FirstAired.HasValue && o.FirstAired.HasValue)
                FirstAired = o.FirstAired;

            if (betterLanguage && o.SiteRating > 0)
                SiteRating = o.SiteRating;

            if (betterLanguage && o.SiteRatingVotes > 0)
                SiteRatingVotes = o.SiteRatingVotes;

            if (!aliases.Any() || (o.aliases.Any() && betterLanguage))
                aliases = o.aliases;

            if (!genres.Any() || (o.genres.Any() && betterLanguage))
                genres = o.genres;

            if (o.AirsTime != null)
                AirsTime = o.AirsTime;

            if ((o.AiredSeasons != null) && (o.AiredSeasons.Count != 0))
                AiredSeasons = o.AiredSeasons;
            if ((o.DvdSeasons != null) && (o.DvdSeasons.Count != 0))
                DvdSeasons = o.DvdSeasons;

            banners.MergeBanners(o.banners);

            if (betterLanguage)
                LanguageId = o.LanguageId;

            Dirty = o.Dirty;
        }

        private static string ChooseBetter(string encumbant, bool betterLanguage, string newValue)
        {
            if (string.IsNullOrEmpty(encumbant)) return newValue;
            if (string.IsNullOrEmpty(newValue)) return encumbant;
            return betterLanguage?newValue:encumbant;
        }

        private void LoadXml(XElement seriesXml)
        {
            //<Data>
            // <Series>
            //  <id>...</id>
            //  etc.
            // </Series>
            // <Episode>
            //  <id>...</id>
            //  blah blah
            // </Episode>
            // <Episode>
            //  <id>...</id>
            //  blah blah
            // </Episode>
            // ...
            //</Data>

            try
            {
                TvdbCode = seriesXml.ExtractInt("id")?? throw new TheTVDB.TVDBException("Error Extracting Id for Series");
                Name = System.Web.HttpUtility.HtmlDecode(
                    XmlHelper.ReadStringFixQuotesAndSpaces(seriesXml.ExtractString("SeriesName") ?? seriesXml.ExtractString("seriesName")));

                SrvLastUpdated = seriesXml.ExtractLong("lastupdated")??seriesXml.ExtractLong("lastUpdated") ??0;
                LanguageId = seriesXml.ExtractInt("LanguageId") ?? seriesXml.ExtractInt("languageId") ?? throw new TheTVDB.TVDBException("Error Extracting Language for Series");
                TempTimeZone = seriesXml.ExtractString("TimeZone");

                string theTime = seriesXml.ExtractString("Airs_Time")?? seriesXml.ExtractString("airsTime");
                AirsTime = ParseAirTime(theTime);

                AirsDay = seriesXml.ExtractString("airsDayOfWeek") ?? seriesXml.ExtractString("Airs_DayOfWeek");
                BannerString = seriesXml.ExtractString("BannerString") ?? seriesXml.ExtractString("bannerString");
                Imdb = seriesXml.ExtractString("imdbId") ?? seriesXml.ExtractString("IMDB_ID");
                Network = seriesXml.ExtractString("network") ?? seriesXml.ExtractString("Network");
                Overview = seriesXml.ExtractString("overview") ?? seriesXml.ExtractString("Overview");
                ContentRating = seriesXml.ExtractString("rating") ?? seriesXml.ExtractString("Rating");
                Runtime = seriesXml.ExtractString("runtime") ?? seriesXml.ExtractString("Runtime");
                SeriesId = seriesXml.ExtractString("seriesId") ?? seriesXml.ExtractString("SeriesID");
                Status = seriesXml.ExtractString("status") ?? seriesXml.ExtractString("Status");
                string siteRatingString = seriesXml.ExtractString("siteRating") ?? seriesXml.ExtractString("SiteRating");
                float.TryParse(siteRatingString, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out SiteRating);
                SiteRatingVotes = seriesXml.ExtractInt("siteRatingCount") ?? seriesXml.ExtractInt("SiteRatingCount")??0;

                string theDate = seriesXml.ExtractString("FirstAired")?? seriesXml.ExtractString("firstAired");
                if (!string.IsNullOrWhiteSpace(theDate))
                {
                    try
                    {
                        FirstAired = DateTime.ParseExact(theDate, "yyyy-MM-dd",
                            new CultureInfo(""));

                        Year = FirstAired.Value.ToString("yyyy");
                    }
                    catch
                    {
                        Logger.Trace("Failed to parse date: {0} ", theDate);
                        FirstAired = null;
                        Year = "";
                    }
                }
                else
                {
                    FirstAired = null;
                    Year = "";
                }

                ClearActors();
                foreach (XElement actorXml in seriesXml.Descendants("Actors").Descendants("Actor"))
                {
                    Actor a = new Actor(actorXml);
                    AddActor(a);
                }

                aliases = new List<string>();
                foreach (XElement aliasXml in seriesXml.Descendants("Actors").Descendants("Actor"))
                {
                    aliases.Add(aliasXml.Value);
                }

                genres = new List<string>();
                foreach (XElement g in seriesXml.Descendants("Genres").Descendants("Genre"))
                {
                    genres.Add(g.Value);
                }
            }
            catch (TheTVDB.TVDBException e)
            {
                string message = "Error processing data from TheTVDB for a show.";
                if (TvdbCode != -1)
                    message += "\r\nTheTVDB Code: " + TvdbCode;
                if (!string.IsNullOrEmpty(Name))
                    message += "\r\nName: " + Name;

                message += "\r\nLanguage: \"" + LanguageId + "\"";

                message += "\r\n" + e.Message;

                Logger.Error(e, message);

                throw new TheTVDB.TVDBException(e.Message);
            }
        }

        private static DateTime? ParseAirTime(string theTime)
        {
            try
            {
                if (!string.IsNullOrEmpty(theTime))
                {
                    if (DateTime.TryParse(theTime, out DateTime airsTime) ||
                        DateTime.TryParse(theTime.Replace('.', ':'), out airsTime))
                        return airsTime;
                }
            }
            catch (FormatException)
            {
                Logger.Info("Failed to parse time: {0} ", theTime);
            }
            return DateTime.Parse("20:00");
        }

        private void LoadJson(JObject r)
        {
            AirsDay = (string)r["airsDayOfWeek"];
            AirsTime = ParseAirTime((string)r["airsTime"]);
            aliases = r["aliases"].Select(x => x.Value<string>()).ToList();
            BannerString = (string)r["banner"];

            string theDate = (string)r["firstAired"];
            try
            {
                if (!string.IsNullOrEmpty(theDate))
                {
                    FirstAired = DateTime.ParseExact(theDate, "yyyy-MM-dd", new System.Globalization.CultureInfo(""));
                    Year = FirstAired.Value.ToString("yyyy");
                }
                else
                {
                    FirstAired = null;
                    Year = "";
                }
            }
            catch
            {
                FirstAired = null;
                Year = "";
            }

            if (r.ContainsKey("genre"))
                genres = r["genre"]?.Select(x => x.Value<string>()).ToList();

            TvdbCode = (int)r["id"];
            Imdb = (string)r["imdbId"];
            Network =  (string) r["network"];
            Overview = System.Web.HttpUtility.HtmlDecode((string)r["overview"]);
            ContentRating = (string) r["rating"];
            Runtime = (string) r["runtime"];
            SeriesId = (string) r["seriesId"];
            if ((string)r["seriesName"] != null)
            {
                Name = System.Web.HttpUtility.HtmlDecode((string)r["seriesName"]);
            }
            Status = (string) r["status"];

            if (long.TryParse((string)r["lastUpdated"], out long updateTime) )
                SrvLastUpdated = updateTime;
            else
                SrvLastUpdated = 0;

            string siteRatingString = (string) r["siteRating"];
            float.TryParse(siteRatingString, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out SiteRating);

            string siteRatingVotesString =  (string) r["siteRatingCount"];
            int.TryParse(siteRatingVotesString, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out SiteRatingVotes);
        }

        private void LoadJson(JObject bestLanguageR, JObject backupLanguageR)
        {
            //Here we have two pieces of JSON. One in local language and one in the default language (English). 
            //We will populate with the best language frst and then fill in any gaps with the backup Language
            LoadJson(bestLanguageR);

            //backupLanguageR should be a series of name/value pairs (ie a JArray of JPropertes)
            //TVDB asserts that name and overview are the fields that are localised

            if ((string.IsNullOrWhiteSpace(Name) && ((string)backupLanguageR["seriesName"] != null)) ){
                Name = System.Web.HttpUtility.HtmlDecode((string)backupLanguageR["seriesName"]);
            }

            if ((string.IsNullOrWhiteSpace(Overview) && ((string)backupLanguageR["overview"] != null)) ){
                Overview = System.Web.HttpUtility.HtmlDecode((string)backupLanguageR["overview"]);
            }

            //Looking at the data then the aliases, banner and runtime are also different by language

            if (!aliases.Any())
            {
                aliases = backupLanguageR["aliases"].Select(x => x.Value<string>()).ToList();
            }

            if ((string.IsNullOrWhiteSpace(Runtime)))
            {
                Runtime = (string)backupLanguageR["runtime"];
            }

            if ((string.IsNullOrWhiteSpace(BannerString)))
            {
                BannerString = (string)backupLanguageR["banner"];
            }
        }

        public IEnumerable<string> Genres() => genres;

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Series");

            XmlHelper.WriteElementToXml(writer, "id", TvdbCode);
            XmlHelper.WriteElementToXml(writer, "SeriesName", Name);
            XmlHelper.WriteElementToXml(writer, "lastupdated", SrvLastUpdated);
            XmlHelper.WriteElementToXml(writer, "LanguageId", LanguageId);
            XmlHelper.WriteElementToXml(writer, "airsDayOfWeek", AirsDay);
            XmlHelper.WriteElementToXml(writer, "Airs_Time", AirsTime );
            XmlHelper.WriteElementToXml(writer, "banner", BannerString);
            XmlHelper.WriteElementToXml(writer, "imdbId", Imdb);
            XmlHelper.WriteElementToXml(writer, "network", Network);
            XmlHelper.WriteElementToXml(writer, "overview", Overview);
            XmlHelper.WriteElementToXml(writer, "rating", ContentRating);
            XmlHelper.WriteElementToXml(writer, "runtime", Runtime);
            XmlHelper.WriteElementToXml(writer, "seriesId", SeriesId);
            XmlHelper.WriteElementToXml(writer, "status", Status);
            XmlHelper.WriteElementToXml(writer, "siteRating", SiteRating);
            XmlHelper.WriteElementToXml(writer, "siteRatingCount", SiteRatingVotes);

            if (FirstAired != null)
            {
                XmlHelper.WriteElementToXml(writer, "FirstAired", FirstAired.Value.ToString("yyyy-MM-dd"));
            }

            writer.WriteStartElement("Actors");
            foreach (Actor aa in GetActors())
            {
                aa.WriteXml(writer);
            }
            writer.WriteEndElement(); //Actors

            writer.WriteStartElement("Aliases");
            foreach (string a in aliases)
            {
                XmlHelper.WriteElementToXml(writer, "Alias", a);
            }
            writer.WriteEndElement(); //Aliases

            writer.WriteStartElement("Genres");
            foreach (string a in genres)
            {
                XmlHelper.WriteElementToXml(writer, "Genre", a);
            }
            writer.WriteEndElement(); //Genres

            writer.WriteEndElement(); // series
        }

        public Season GetOrAddAiredSeason(int num, int seasonId)
        {
            if (AiredSeasons.ContainsKey(num))
                return AiredSeasons[num];

            Season s = new Season(this, num, seasonId,Season.SeasonType.aired);
            AiredSeasons[num] = s;

            return s;
        }

        public Season GetOrAddDvdSeason(int num, int seasonId)
        {
            if (DvdSeasons.ContainsKey(num))
                return DvdSeasons[num];

            Season s = new Season(this, num, seasonId,Season.SeasonType.dvd);
            DvdSeasons[num] = s;

            return s;
        }

        internal Episode GetEpisode(int seasF, int epF,bool dvdOrder)
        {
            if (dvdOrder)
            {
                foreach (Season s in DvdSeasons.Values)
                {
                    if (s.SeasonNumber == seasF)
                    {
                        foreach (Episode pe in s.Episodes.Values)
                        {
                            if (pe.DvdEpNum == epF) return pe;
                        }
                    }
                }
            }
            else
            {
                foreach (Season s in AiredSeasons.Values)
                {
                    if (s.SeasonNumber == seasF)
                    {
                        foreach (Episode pe in s.Episodes.Values)
                        {
                            if (pe.AiredEpNum == epF) return pe;
                        }
                    }
                }
            }
            throw new EpisodeNotFoundException();
        }

        [Serializable]
        public class EpisodeNotFoundException : Exception
        {
        }

        internal void RemoveEpisode(int episodeId)
        {
            //Remove from Aired and DVD Seasons
            foreach (Season s in DvdSeasons.Values)
            {
                s.RemoveEpisode(episodeId);
            }
            foreach (Season s in AiredSeasons.Values)
            {
                s.RemoveEpisode(episodeId);
            }
        }

        public void ClearActors()
        {
            actors=new List<Actor>();
        }

        public void AddActor(Actor actor)
        {
            actors.Add(actor);
        }

        public string GetImdbNumber()
        {
            return Imdb.StartsWith("tt") ? Imdb.Substring(2): Imdb;
        }

        public int GetSeasonIndex(int seasonNumber, Season.SeasonType type)
        {
            Dictionary<int, Season> appropriateSeasons = type == Season.SeasonType.aired ? AiredSeasons : DvdSeasons;

            List<int> seasonNumbers = new List<int>();
            foreach (KeyValuePair<int, Season> sn in appropriateSeasons)
            {
                if (sn.Value.IsSpecial()) continue;

                seasonNumbers.Add(sn.Value.SeasonNumber);
            }

            seasonNumbers.Sort();

            return seasonNumbers.IndexOf(seasonNumber) +1;
        }

        public string GetSeriesFanartPath() => banners.GetSeriesFanartPath();
        public string GetSeriesPosterPath() => banners.GetSeriesPosterPath();
        public string GetImage(TVSettings.FolderJpgIsType itemForFolderJpg) => banners.GetImage(itemForFolderJpg);
        public string GetSeasonBannerPath(int snum) => banners.GetSeasonBannerPath(snum);
        public string GetSeriesWideBannerPath() => banners.GetSeriesWideBannerPath();
        public string GetSeasonWideBannerPath(int snum) => banners.GetSeasonWideBannerPath(snum);
        public void AddOrUpdateBanner(Banner banner) => banners.AddOrUpdateBanner(banner);
    }
}
