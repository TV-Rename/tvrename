// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    public class SeriesInfo
    {
        public DateTime? AirsTime;
        private string airsTimeString; //The raw value we obtain from TVDB
        public bool Dirty; // set to true if local info is known to be older than whats on the server
        public DateTime? FirstAired;
        public readonly string TargetLanguageCode; //The Language Code we'd like the Series in ; null if we want to use the system setting
        public int LanguageId; //The actual language obtained
        public string Name;
        public string AirsDay;
        public string Network;
        public string Overview;
        public string Runtime;
        public string ContentRating;
        public float SiteRating;
        public int SiteRatingVotes;
        public string Imdb;
        public string SeriesId;
        public string BannerString;
        public bool BannersLoaded;
        public long SrvLastUpdated;
        public int TvdbCode;
        public bool IsStub;
        public string Slug;

        private List<Actor> actors;
        private List<string> genres;
        private List<string> aliases;

        private ConcurrentDictionary<int, Episode> sourceEpisodes;

        [NotNull]
        public ICollection<Episode> Episodes => sourceEpisodes.Values;

        public void ClearEpisodes()
        {
            sourceEpisodes.Clear();
        }

        private SeriesBanners banners;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public bool UseCustomLanguage => TargetLanguageCode != null;

        public IEnumerable<KeyValuePair<int, Banner>> AllBanners => banners.AllBanners;

        public int? MinYear
        {
            get
            {
                return Episodes.Select(e => e.GetAirDateDt())
                        .Where(adt => adt.HasValue)
                        .Select(adt => adt.Value)
                        .Min(airDateTime => (int?) airDateTime.Year);
            }
        }

        public int? MaxYear
        {
            get
            {
                return Episodes.Select(e => e.GetAirDateDt())
                    .Where(adt => adt.HasValue)
                    .Select(adt => adt.Value)
                    .Max(airDateTime => (int?)airDateTime.Year);
            }
        }

      [NotNull]
      public string Year => FirstAired?.ToString("yyyy") ?? $"{MinYear}";

      public string Status { get; private set; } = "Unkonwn";

      // note: "SeriesID" in a <Series> is the tv.com code,
        // "seriesid" in an <Episode> is the tvdb code!

        public SeriesInfo(string name, int id)
        {
            SetToDefaults();
            Name = name;
            TvdbCode = id;
            IsStub = false;
        }

        public SeriesInfo(string name, int id, string langCode):this(name,id)
        {
            TargetLanguageCode = langCode;
        }

        public SeriesInfo([NotNull] XElement seriesXml)
        {
            SetToDefaults();
            LoadXml(seriesXml);
            IsStub = false;
        }

        public SeriesInfo([NotNull] JObject json,int langId,bool miniSeriesData)
        {
            SetToDefaults();
            LanguageId = langId;
            LoadJson(json);
            IsStub = miniSeriesData;

            if (string.IsNullOrEmpty(Name))
            {
               Logger.Warn("Issue with series " + TvdbCode );
               Logger.Warn(json.ToString());
            }

            if (SrvLastUpdated==0 && !miniSeriesData)
            {
                Logger.Warn("Issue with series (update time is 0) " + TvdbCode);
                Logger.Warn(json.ToString());
                SrvLastUpdated = 100;
            }
        }

        public SeriesInfo([NotNull] JObject json, JObject jsonInDefaultLang, int langId)
        {
            SetToDefaults();
            LanguageId = langId;
            LoadJson(json,jsonInDefaultLang);
            IsStub = false;
            if (string.IsNullOrEmpty(Name)            ){
               Logger.Warn("Issue with series " + TvdbCode );
               Logger.Warn(json.ToString());
               Logger.Info(jsonInDefaultLang .ToString());
            }

            if (SrvLastUpdated == 0)
            {
                Logger.Warn("Issue with series (update time is 0) " + TvdbCode);
                Logger.Warn(json.ToString());
                Logger.Info(jsonInDefaultLang.ToString());
                SrvLastUpdated = 100;
            }
        }

        public IEnumerable<Actor> GetActors() => actors;

        public IEnumerable<string> Aliases() => aliases;

        [NotNull]
        public IEnumerable<string> GetActorNames() => GetActors().Select(x => x.ActorName);

        private void SetToDefaults()
        {
            sourceEpisodes = new ConcurrentDictionary<int, Episode>();
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

        // ReSharper disable once FunctionComplexityOverflow
        public void Merge([NotNull] SeriesInfo o, int preferredLanguageId)
        {
            if (o.TvdbCode != TvdbCode)
            {
                return; // that's not us!
            }

            if (o.SrvLastUpdated != 0 && o.SrvLastUpdated < SrvLastUpdated)
            {
                return; // older!?
            }

            if (!o.IsStub)
            {
                IsStub = false;
            }
            bool currentLanguageNotSet = o.LanguageId == -1;
            bool newLanguageBetter = o.LanguageId == preferredLanguageId && LanguageId != preferredLanguageId;
            bool newLanguageOptimal = o.LanguageId == preferredLanguageId;
            bool useNewDataOverOld = currentLanguageNotSet || newLanguageBetter ||newLanguageOptimal;

            SrvLastUpdated = o.SrvLastUpdated;

            // take the best bits of "o"
            // "o" is always newer/better than us, if there is a choice
            Name = ChooseBetter(Name, useNewDataOverOld, o.Name);
            AirsDay = ChooseBetter(AirsDay, useNewDataOverOld, o.AirsDay);
            Imdb = ChooseBetter(Imdb, useNewDataOverOld, o.Imdb);
            Overview = ChooseBetter(Overview, useNewDataOverOld, o.Overview);
            BannerString = ChooseBetter(BannerString, useNewDataOverOld, o.BannerString);
            Network = ChooseBetter(Network, useNewDataOverOld, o.Network);
            Runtime = ChooseBetter(Runtime, useNewDataOverOld, o.Runtime);
            SeriesId = ChooseBetter(SeriesId, useNewDataOverOld, o.SeriesId);
            Status = ChooseBetter(Status, useNewDataOverOld, o.Status);
            ContentRating = ChooseBetter(ContentRating, useNewDataOverOld, o.ContentRating);
            Slug = ChooseBetter(Slug, useNewDataOverOld, o.Slug);

            if ( o.FirstAired.HasValue &&(useNewDataOverOld || !FirstAired.HasValue))
            {
                FirstAired = o.FirstAired;
            }

            if (useNewDataOverOld && o.SiteRating > 0)
            {
                SiteRating = o.SiteRating;
            }

            if (useNewDataOverOld && o.SiteRatingVotes > 0)
            {
                SiteRatingVotes = o.SiteRatingVotes;
            }

            bool useNewAliases = o.aliases.Any() && useNewDataOverOld;
            if (!aliases.Any() || useNewAliases)
            {
                aliases = o.aliases;
            }

            bool useNewGenres = o.genres.Any() && useNewDataOverOld;
            if (!genres.Any() || useNewGenres)
            {
                genres = o.genres;
            }

            if (o.AirsTime != null)
            {
                AirsTime = o.AirsTime;
            }

            if (o.sourceEpisodes != null && o.sourceEpisodes.Count != 0)
            {
                sourceEpisodes = o.sourceEpisodes;
            }

            banners.MergeBanners(o.banners);

            if (useNewDataOverOld)
            {
                LanguageId = o.LanguageId;
            }

            Dirty = o.Dirty;
        }

        [NotNull]
        private static string ChooseBetter([CanBeNull] string encumbant, bool betterLanguage, [CanBeNull] string newValue)
        {
            if (string.IsNullOrEmpty(encumbant))
            {
                return newValue?.Trim()??string.Empty;
            }

            if (string.IsNullOrEmpty(newValue))
            {
                return encumbant.Trim();
            }

            return betterLanguage?newValue.Trim():encumbant.Trim();
        }

        private void LoadXml([NotNull] XElement seriesXml)
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

                SrvLastUpdated = seriesXml.ExtractLong("lastupdated")??seriesXml.ExtractLong("lastUpdated",0);
                LanguageId = seriesXml.ExtractInt("LanguageId") ?? seriesXml.ExtractInt("languageId") ?? throw new TheTVDB.TVDBException("Error Extracting Language for Series");

                airsTimeString = seriesXml.ExtractString("Airs_Time")?? seriesXml.ExtractString("airsTime");
                AirsTime = ParseAirTime(airsTimeString);

                AirsDay = seriesXml.ExtractString("airsDayOfWeek") ?? seriesXml.ExtractString("Airs_DayOfWeek");
                BannerString = seriesXml.ExtractString("banner") ?? seriesXml.ExtractString("Banner");
                Imdb = seriesXml.ExtractString("imdbId") ?? seriesXml.ExtractString("IMDB_ID");
                Network = seriesXml.ExtractString("network") ?? seriesXml.ExtractString("Network");
                Overview = seriesXml.ExtractString("overview") ?? seriesXml.ExtractString("Overview");
                ContentRating = seriesXml.ExtractString("rating") ?? seriesXml.ExtractString("Rating");
                Runtime = seriesXml.ExtractString("runtime") ?? seriesXml.ExtractString("Runtime");
                SeriesId = seriesXml.ExtractString("seriesId") ?? seriesXml.ExtractString("SeriesID");
                Status = seriesXml.ExtractString("status") ?? seriesXml.ExtractString("Status");
                SiteRatingVotes = seriesXml.ExtractInt("siteRatingCount") ?? seriesXml.ExtractInt("SiteRatingCount",0);
                Slug = seriesXml.ExtractString("slug");

                SiteRating = GetSiteRating(seriesXml);
                FirstAired = ExtractFirstAired(seriesXml);

                LoadActors(seriesXml);
                LoadAliases(seriesXml);
                LoadGenres(seriesXml);
            }
            catch (TheTVDB.TVDBException e)
            {
                Logger.Error(e, GenerateErrorMessage());
                throw new TheTVDB.TVDBException(e.Message);
            }
        }

        private static float GetSiteRating([NotNull] XElement seriesXml)
        {
            string siteRatingString = seriesXml.ExtractString("siteRating") ?? seriesXml.ExtractString("SiteRating");
            float.TryParse(siteRatingString,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
                CultureInfo.CreateSpecificCulture("en-US"), out float x);

            return x;
        }

        [NotNull]
        private string GenerateErrorMessage()
        {
            string message = "Error processing data from TheTVDB for a show.";
            if (TvdbCode != -1)
            {
                message += "\r\nTheTVDB Code: " + TvdbCode;
            }

            if (!string.IsNullOrEmpty(Name))
            {
                message += "\r\nName: " + Name;
            }

            message += "\r\nLanguage: \"" + LanguageId + "\"";
            return message;
        }

        private static DateTime? ExtractFirstAired([NotNull] XElement seriesXml)
        {
            string theDate = seriesXml.ExtractString("FirstAired") ?? seriesXml.ExtractString("firstAired");
            if (!string.IsNullOrWhiteSpace(theDate))
            {
                try
                {
                    return DateTime.ParseExact(theDate, "yyyy-MM-dd", new CultureInfo(""));
                }
                catch
                {
                    Logger.Trace("Failed to parse date: {0} ", theDate);
                }
            }

            return null;
        }

        private void LoadActors([NotNull] XElement seriesXml)
        {
            ClearActors();
            foreach (Actor a in seriesXml.Descendants("Actors").Descendants("Actor").Select(actorXml => new Actor(actorXml)))
            {
                AddActor(a);
            }
        }

        private void LoadAliases([NotNull] XElement seriesXml)
        {
            aliases = new List<string>();
            foreach (XElement aliasXml in seriesXml.Descendants("Aliases").Descendants("Alias"))
            {
                aliases.Add(aliasXml.Value);
            }
        }

        private void LoadGenres([NotNull] XElement seriesXml)
        {
            genres = new List<string>();
            foreach (XElement g in seriesXml.Descendants("Genres").Descendants("Genre"))
            {
                genres.Add(g.Value);
            }
        }

        private static DateTime? ParseAirTime([CanBeNull] string theTime)
        {
            try
            {
                if (!string.IsNullOrEmpty(theTime))
                {
                    if (DateTime.TryParse(theTime, out DateTime airsTime) )
                    {
                        return airsTime;
                    }

                    if (DateTime.TryParse(theTime.Replace('.', ':'), out airsTime))
                    {
                        return airsTime;
                    }
                }
            }
            catch (FormatException)
            {
                Logger.Info("Failed to parse time: {0} ", theTime);
            }
            return DateTime.Parse("20:00");
        }

        private void LoadJson([NotNull] JObject r)
        {
            AirsDay = ((string)r["airsDayOfWeek"])?.Trim();
            airsTimeString = (string) r["airsTime"];
            AirsTime = ParseAirTime(airsTimeString);
            aliases = r["aliases"].Select(x => x.Value<string>()).ToList();
            BannerString = (string)r["banner"];
            FirstAired = ParseFirstAired((string)r["firstAired"]);

            if (r.ContainsKey("genre"))
            {
                genres = r["genre"]?.Select(x => x.Value<string>()).ToList();
            }

            TvdbCode = (int)r["id"];
            Imdb = ((string)r["imdbId"])?.Trim();
            Network =  ((string) r["network"])?.Trim();
            Slug = ((string)r["slug"])?.Trim();
            Overview = System.Web.HttpUtility.HtmlDecode((string)r["overview"])?.Trim();
            ContentRating = ((string) r["rating"])?.Trim();
            Runtime = ((string) r["runtime"])?.Trim();
            SeriesId = (string) r["seriesId"];
            if ((string)r["seriesName"] != null)
            {
                Name = System.Web.HttpUtility.HtmlDecode((string)r["seriesName"])?.Trim();
            }
            Status = (string) r["status"];

            SrvLastUpdated = long.TryParse((string)r["lastUpdated"], out long updateTime) ? updateTime : 0;

            string siteRatingString = (string) r["siteRating"];
            float.TryParse(siteRatingString, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out SiteRating);

            string siteRatingVotesString =  (string) r["siteRatingCount"];
            int.TryParse(siteRatingVotesString, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out SiteRatingVotes);
        }

        [NotNull]
        internal Episode GetEpisode(int epId)
        {
            if (sourceEpisodes.TryGetValue(epId, out Episode returnValue))
            {
                return returnValue;
            }
            throw new ShowItem.EpisodeNotFoundException();
        }

        private static DateTime? ParseFirstAired([CanBeNull] string theDate)
        {
            try
            {
                if (!string.IsNullOrEmpty(theDate))
                {
                    return DateTime.ParseExact(theDate, "yyyy-MM-dd", new CultureInfo(""));
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private void LoadJson([NotNull] JObject bestLanguageR, JObject backupLanguageR)
        {
            //Here we have two pieces of JSON. One in local language and one in the default language (English). 
            //We will populate with the best language frst and then fill in any gaps with the backup Language
            LoadJson(bestLanguageR);

            //backupLanguageR should be a series of name/value pairs (ie a JArray of JPropertes)
            //TVDB asserts that name and overview are the fields that are localised

            if (string.IsNullOrWhiteSpace(Name) && (string)backupLanguageR["seriesName"] != null ){
                Name = System.Web.HttpUtility.HtmlDecode((string)backupLanguageR["seriesName"]);
            }

            if (string.IsNullOrWhiteSpace(Overview) && (string)backupLanguageR["overview"] != null ){
                Overview = System.Web.HttpUtility.HtmlDecode((string)backupLanguageR["overview"]);
            }

            //Looking at the data then the aliases, banner and runtime are also different by language

            if (!aliases.Any())
            {
                aliases = backupLanguageR["aliases"].Select(x => x.Value<string>()).ToList();
            }

            if (string.IsNullOrWhiteSpace(Runtime))
            {
                Runtime = (string)backupLanguageR["runtime"];
            }

            if (string.IsNullOrWhiteSpace(BannerString))
            {
                BannerString = (string)backupLanguageR["banner"];
            }
        }

        public IEnumerable<string> Genres() => genres;

        public void WriteXml([NotNull] XmlWriter writer)
        {
            writer.WriteStartElement("Series");

            writer.WriteElement("id", TvdbCode);
            writer.WriteElement("SeriesName", Name);
            writer.WriteElement("lastupdated", SrvLastUpdated);
            writer.WriteElement("LanguageId", LanguageId);
            writer.WriteElement("airsDayOfWeek", AirsDay);
            writer.WriteElement("Airs_Time", AirsTime?.ToString("HH:mm"),true);
            writer.WriteElement("banner", BannerString);
            writer.WriteElement("imdbId", Imdb);
            writer.WriteElement("network", Network);
            writer.WriteElement("overview", Overview);
            writer.WriteElement("rating", ContentRating);
            writer.WriteElement("runtime", Runtime);
            writer.WriteElement("seriesId", SeriesId);
            writer.WriteElement("status", Status);
            writer.WriteElement("siteRating", SiteRating,"0.##");
            writer.WriteElement("siteRatingCount", SiteRatingVotes);
            writer.WriteElement("slug",Slug);

            if (FirstAired != null)
            {
                writer.WriteElement("FirstAired", FirstAired.Value.ToString("yyyy-MM-dd"));
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
                writer.WriteElement("Alias", a);
            }
            writer.WriteEndElement(); //Aliases

            writer.WriteStartElement("Genres");
            foreach (string a in genres)
            {
                writer.WriteElement("Genre", a);
            }
            writer.WriteEndElement(); //Genres

            writer.WriteEndElement(); // series
        }

        public void ClearActors()
        {
            actors=new List<Actor>();
        }

        public void AddActor(Actor actor)
        {
            actors.Add(actor);
        }

        [NotNull]
        public string GetImdbNumber() => Imdb is null? string.Empty: Imdb.StartsWith("tt", StringComparison.Ordinal) ? Imdb?.Substring(2): Imdb;

        [NotNull]
        public string GetSeriesFanartPath() => banners.GetSeriesFanartPath();
        [NotNull]
        public string GetSeriesPosterPath() => banners.GetSeriesPosterPath();
        public string GetImage(TVSettings.FolderJpgIsType itemForFolderJpg) => banners.GetImage(itemForFolderJpg);
        public string GetSeasonBannerPath(int snum) => banners.GetSeasonBannerPath(snum);
        public string GetSeriesWideBannerPath() => banners.GetSeriesWideBannerPath();
        public string GetSeasonWideBannerPath(int snum) => banners.GetSeasonWideBannerPath(snum);
        public void AddOrUpdateBanner([NotNull] Banner banner) => banners.AddOrUpdateBanner(banner);

        public void UpdateBanners(List<int> latestBannerIds)
        {
            List<int> bannersToRemove =new List<int>();
            foreach (KeyValuePair<int, Banner> bannerKeyValuePair in AllBanners)
            {
                if (latestBannerIds.Contains(bannerKeyValuePair.Key))
                {
                    continue;
                }

                bannersToRemove.Add(bannerKeyValuePair.Key);
            }

            foreach (int removeBanner in bannersToRemove)
            {
                banners.Remove(removeBanner);
            }
        }

        public void AddEpisode([NotNull] Episode episode)
        {
            sourceEpisodes.TryAdd(episode.EpisodeId,episode);
            episode.SetSeriesSeason(this);
        }

        public void RemoveEpisode(int episodeId)
        {
            sourceEpisodes.TryRemove(episodeId, out Episode _);
        }
    }
}
