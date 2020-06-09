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
using TVRename.TheTVDB;

namespace TVRename
{
    public class SeriesInfo
    {
        public DateTime? AirsTime;
        private string? airsTimeString; //The raw value we obtain from TVDB
        public bool Dirty; // set to true if local info is known to be older than whats on the server
        public DateTime? FirstAired;
        public readonly string? TargetLanguageCode; //The Language Code we'd like the Series in ; null if we want to use the system setting
        public int LanguageId; //The actual language obtained
        public string Name;
        public string? AirsDay;
        public string? Network;
        public string? Overview;
        public string? Runtime;
        public string? ContentRating;
        public float SiteRating;
        public int SiteRatingVotes;
        public string? Imdb;
        public int TvdbCode;
        public int TvMazeCode;
        public int TvRageCode;
        public string? SeriesId;
        public string? WebUrl;
        public string? OfficialUrl;
        public string? Type;
        public string? ShowLanguage;
        public string? BannerString;
        public string? PosterUrl;
        public bool BannersLoaded;
        public long SrvLastUpdated;
        
        public bool IsSearchResultOnly;
        public string? Slug;

        private List<Actor> actors;
        public List<string> Genres;
        private List<string> aliases;

        private ConcurrentDictionary<int, Episode> sourceEpisodes;

        [NotNull]
        public ICollection<Episode> Episodes => sourceEpisodes.Values;

        public void ClearEpisodes()
        {
            sourceEpisodes.Clear();
        }

        private readonly SeriesBanners banners;

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

      public string? Status { get; set; }

      // note: "SeriesID" in a <Series> is the tv.com code,
        // "seriesid" in an <Episode> is the tvdb code!

        public SeriesInfo()
        {
            sourceEpisodes = new ConcurrentDictionary<int, Episode>();
            actors = new List<Actor>();
            aliases = new List<string>();
            Genres = new List<string>();
            Dirty = false;
            Name = string.Empty;
            AirsTime = null;
            TvdbCode = -1;
            TvMazeCode = -1;
            TvRageCode = 0;
            LanguageId = -1;
            Status = "Unknown";
            banners = new SeriesBanners(this);
            banners.ResetBanners();
            BannersLoaded = false;
        }

        public SeriesInfo(int tvdb, int tvmaze):this()
        {
            IsSearchResultOnly = false;
            TvMazeCode = tvmaze;
            TvdbCode = tvdb;
        }

        public SeriesInfo( int tvdb, int tvmaze, string langCode) :this(tvdb,tvmaze)
        {
            TargetLanguageCode = langCode;
        }

        public SeriesInfo([NotNull] XElement seriesXml):this()
        {
            LoadXml(seriesXml);
            IsSearchResultOnly = false;
        }

        public SeriesInfo([NotNull] JObject json,int langId,bool searchResult) : this()
        {
            LanguageId = langId;
            LoadJson(json);
            IsSearchResultOnly = searchResult;

            if (string.IsNullOrEmpty(Name))
            {
               Logger.Warn("Issue with series " + this );
               Logger.Warn(json.ToString());
            }

            if (SrvLastUpdated==0 && !searchResult)
            {
                Logger.Warn("Issue with series (update time is 0) " + this);
                Logger.Warn(json.ToString());
                SrvLastUpdated = 100;
            }
        }

        public SeriesInfo([NotNull] JObject json, JObject jsonInDefaultLang, int langId):this()
        {
            LanguageId = langId;
            LoadJson(json,jsonInDefaultLang);
            IsSearchResultOnly = false;
            if (string.IsNullOrEmpty(Name)            ){
               Logger.Warn("Issue with series " + this );
               Logger.Warn(json.ToString());
               Logger.Info(jsonInDefaultLang .ToString());
            }

            if (SrvLastUpdated == 0)
            {
                Logger.Warn("Issue with series (update time is 0) " + this);
                Logger.Warn(json.ToString());
                Logger.Info(jsonInDefaultLang.ToString());
                SrvLastUpdated = 100;
            }
        }

        public IEnumerable<Actor> GetActors() => actors;

        public IEnumerable<string> Aliases() => aliases;

        [NotNull]
        public IEnumerable<string> GetActorNames() => GetActors().Select(x => x.ActorName);

        // ReSharper disable once FunctionComplexityOverflow
        public void Merge([NotNull] SeriesInfo o)
        {
            if (o.TvdbCode != TvdbCode && o.TvMazeCode !=TvMazeCode) 
            {
                return; // that's not us!
            }

            if (o.TvMazeCode !=-1 && TvMazeCode != o.TvMazeCode)
            {
                TvMazeCode = o.TvMazeCode;
            }

            if (o.TvdbCode != -1 && TvdbCode != o.TvdbCode)
            {
                TvdbCode = o.TvdbCode;
            }

            if (o.SrvLastUpdated != 0 && o.SrvLastUpdated < SrvLastUpdated)
            {
                return; // older!?
            }

            if (!o.IsSearchResultOnly)
            {
                IsSearchResultOnly = false;
            }
            bool currentLanguageNotSet = LanguageId == -1;
            string bestLanguageCode= TargetLanguageCode ?? TVSettings.Instance.PreferredLanguageCode;
            Language optimaLanguage = LocalCache.Instance.GetLanguageFromCode(bestLanguageCode);
            bool newLanguageOptimal = !(optimaLanguage is null) && o.LanguageId == optimaLanguage.Id;
            bool useNewDataOverOld = currentLanguageNotSet || newLanguageOptimal;

            SrvLastUpdated = o.SrvLastUpdated;

            // take the best bits of "o"
            // "o" is always newer/better than us, if there is a choice
            Name = ChooseBetter(Name, useNewDataOverOld, o.Name);
            AirsDay = ChooseBetter(AirsDay, useNewDataOverOld, o.AirsDay);
            Imdb = ChooseBetter(Imdb, useNewDataOverOld, o.Imdb);
            WebUrl= ChooseBetter(WebUrl, useNewDataOverOld, o.WebUrl);
            OfficialUrl = ChooseBetter(OfficialUrl, useNewDataOverOld, o.OfficialUrl);
            ShowLanguage = ChooseBetter(ShowLanguage, useNewDataOverOld, o.ShowLanguage);
            Type = ChooseBetter(Type, useNewDataOverOld, o.Type);
            Overview = ChooseBetter(Overview, useNewDataOverOld, o.Overview);
            BannerString = ChooseBetter(BannerString, useNewDataOverOld, o.BannerString);
            PosterUrl = ChooseBetter(PosterUrl, useNewDataOverOld, o.PosterUrl);
            Network = ChooseBetter(Network, useNewDataOverOld, o.Network);
            Runtime = ChooseBetter(Runtime, useNewDataOverOld, o.Runtime);
            SeriesId = ChooseBetter(SeriesId, useNewDataOverOld, o.SeriesId);
            Status = ChooseBetterStatus(Status, useNewDataOverOld, o.Status);
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

            bool useNewGenres = o.Genres.Any() && useNewDataOverOld;
            if (!Genres.Any() || useNewGenres)
            {
                Genres = o.Genres;
            }

            bool useNewSeasons = o.seasons.Any() && useNewDataOverOld;
            if (!seasons.Any() || useNewSeasons)
            {
                seasons = o.seasons;
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
            BannersLoaded = o.BannersLoaded;

            if (useNewDataOverOld)
            {
                LanguageId = o.LanguageId;
            }

            Dirty = o.Dirty;
        }

        [NotNull]
        private static string ChooseBetter(string? encumbant, bool betterLanguage, string? newValue)
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

        [NotNull]
        private static string ChooseBetterStatus(string? encumbant, bool betterLanguage, string? newValue)
        {
            if (string.IsNullOrEmpty(encumbant) || encumbant.Equals("Unknown")) 
            {
                return newValue?.Trim() ?? string.Empty;
            }

            if (string.IsNullOrEmpty(newValue) || newValue.Equals("Unknown"))
            {
                return encumbant.Trim();
            }

            return betterLanguage ? newValue.Trim() : encumbant.Trim();
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
                TvdbCode = seriesXml.ExtractInt("id")?? throw new SourceConsistencyException("Error Extracting Id for Series",ShowItem.ProviderType.TheTVDB);
                TvMazeCode = seriesXml.ExtractInt("mazeid") ?? -1;

                Name = System.Web.HttpUtility.HtmlDecode(
                    XmlHelper.ReadStringFixQuotesAndSpaces(seriesXml.ExtractStringOrNull("SeriesName") ?? seriesXml.ExtractString("seriesName")));

                SrvLastUpdated = seriesXml.ExtractLong("lastupdated")??seriesXml.ExtractLong("lastUpdated",0);
                LanguageId = seriesXml.ExtractInt("LanguageId") ?? seriesXml.ExtractInt("languageId") ?? throw new SourceConsistencyException("Error Extracting Language for Series",ShowItem.ProviderType.TheTVDB);

                airsTimeString = seriesXml.ExtractStringOrNull("Airs_Time")?? seriesXml.ExtractString("airsTime");
                AirsTime = JsonHelper.ParseAirTime(airsTimeString);

                AirsDay = seriesXml.ExtractStringOrNull("airsDayOfWeek") ?? seriesXml.ExtractString("Airs_DayOfWeek");
                BannerString = seriesXml.ExtractStringOrNull("banner") ?? seriesXml.ExtractString("Banner");
                PosterUrl = seriesXml.ExtractString("posterURL");
                Imdb = seriesXml.ExtractStringOrNull("imdbId") ?? seriesXml.ExtractString("IMDB_ID");
                WebUrl = seriesXml.ExtractString("WebURL");
                OfficialUrl = seriesXml.ExtractString("OfficialUrl");
                Type = seriesXml.ExtractString("Type");
                ShowLanguage = seriesXml.ExtractString("ShowLanguage");
                TvRageCode = seriesXml.ExtractInt("rageid") ?? 0;
                Network = seriesXml.ExtractStringOrNull("network") ?? seriesXml.ExtractString("Network");
                Overview = seriesXml.ExtractStringOrNull("overview") ?? seriesXml.ExtractString("Overview");
                ContentRating = seriesXml.ExtractStringOrNull("rating") ?? seriesXml.ExtractString("Rating");
                Runtime = seriesXml.ExtractStringOrNull("runtime") ?? seriesXml.ExtractString("Runtime");
                SeriesId = seriesXml.ExtractStringOrNull("seriesId") ?? seriesXml.ExtractString("SeriesID");
                Status = seriesXml.ExtractStringOrNull("status") ?? seriesXml.ExtractString("Status");
                SiteRatingVotes = seriesXml.ExtractInt("siteRatingCount") ?? seriesXml.ExtractInt("SiteRatingCount",0);
                Slug = seriesXml.ExtractString("slug");

                SiteRating = GetSiteRating(seriesXml);
                FirstAired = JsonHelper.ParseFirstAired(seriesXml.ExtractStringOrNull("FirstAired") ?? seriesXml.ExtractString("firstAired"));

                LoadActors(seriesXml);
                LoadAliases(seriesXml);
                LoadGenres(seriesXml);
                LoadSeasons(seriesXml);
            }
            catch (SourceConsistencyException e)
            {
                Logger.Error(e, GenerateErrorMessage());
                // ReSharper disable once PossibleIntendedRethrow
                throw e;
            }
        }

        private static float GetSiteRating([NotNull] XElement seriesXml)
        {
            string siteRatingString = seriesXml.ExtractStringOrNull("siteRating") ?? seriesXml.ExtractString("SiteRating");
            float.TryParse(siteRatingString,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
                CultureInfo.CreateSpecificCulture("en-US"), out float x);

            return x;
        }

        [NotNull]
        private string GenerateErrorMessage() => "Error processing data from TheTVDB for a show. " + this + "\r\nLanguage: \"" + LanguageId + "\"";

        private void LoadActors([NotNull] XElement seriesXml)
        {
            ClearActors();
            foreach (Actor a in seriesXml.Descendants("Actors").Descendants("Actor").Select(actorXml => new Actor(actorXml)))
            {
                AddActor(a);
            }
        }

        private void LoadSeasons([NotNull] XElement seriesXml)
        {
            seasons = new List<Season>();
            foreach (Season s in seriesXml.Descendants("Seasons").Descendants("Season").Select(xml => new Season(xml)))
            {
                seasons.Add(s);
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
            Genres = new List<string>();
            foreach (XElement g in seriesXml.Descendants("Genres").Descendants("Genre"))
            {
                Genres.Add(g.Value);
            }
        }

        private void LoadJson([NotNull] JObject r)
        {
            AirsDay = ((string)r["airsDayOfWeek"])?.Trim();
            airsTimeString = (string) r["airsTime"];
            AirsTime = JsonHelper.ParseAirTime(airsTimeString);
            aliases = (r["aliases"] ?? throw new SourceConsistencyException($"Can't find aliases in Series JSON: {r}",ShowItem.ProviderType.TheTVDB)).Select(x => x.Value<string>()).ToList();
            BannerString = (string)r["banner"];
            FirstAired = JsonHelper.ParseFirstAired((string)r["firstAired"]);

            if (r.ContainsKey("genre"))
            {
                Genres = r["genre"]?.Select(x => x.Value<string>()).ToList() ??new List<string>();
            }

            TvdbCode = (int)r["id"];
            Imdb = ((string)r["imdbId"])?.Trim();
            Network =  ((string) r["network"])?.Trim();
            Slug = ((string)r["slug"])?.Trim();
            Overview = System.Web.HttpUtility.HtmlDecode((string)r["overview"])?.Trim();
            ContentRating = ((string) r["rating"])?.Trim();
            Runtime = ((string) r["runtime"])?.Trim();
            SeriesId = (string) r["seriesId"];
            string s = (string)r["seriesName"];
            if (s != null)
            {
                Name = System.Web.HttpUtility.HtmlDecode(s).Trim();
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

        private void LoadJson([NotNull] JObject bestLanguageR, JObject backupLanguageR)
        {
            //Here we have two pieces of JSON. One in local language and one in the default language (English). 
            //We will populate with the best language frst and then fill in any gaps with the backup Language
            LoadJson(bestLanguageR);

            //backupLanguageR should be a series of name/value pairs (ie a JArray of JPropertes)
            //TVDB asserts that name and overview are the fields that are localised

            string s = (string)backupLanguageR["seriesName"];
            if (string.IsNullOrWhiteSpace(Name) && s != null ){
                Name = System.Web.HttpUtility.HtmlDecode(s);
            }

            string o = (string)backupLanguageR["overview"];
            if (string.IsNullOrWhiteSpace(Overview) && o != null ){
                Overview = System.Web.HttpUtility.HtmlDecode(o);
            }

            //Looking at the data then the aliases, banner and runtime are also different by language

            if (!aliases.Any())
            {
                JToken? aliasesToken = backupLanguageR["aliases"];
                if (aliasesToken is null)
                {
                    throw new SourceConsistencyException($"Can not find aliases in {backupLanguageR}",ShowItem.ProviderType.TheTVDB);
                }
                aliases = aliasesToken.Select(x => x.Value<string>()).ToList();
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

        public void WriteXml([NotNull] XmlWriter writer)
        {
            writer.WriteStartElement("Series");

            writer.WriteElement("id", TvdbCode);
            writer.WriteElement("mazeid",TvMazeCode);
            writer.WriteElement("SeriesName", Name);
            writer.WriteElement("lastupdated", SrvLastUpdated);
            writer.WriteElement("LanguageId", LanguageId);
            writer.WriteElement("airsDayOfWeek", AirsDay);
            writer.WriteElement("Airs_Time", AirsTime?.ToString("HH:mm"),true);
            writer.WriteElement("banner", BannerString);
            writer.WriteElement("posterURL", PosterUrl);
            writer.WriteElement("WebURL", WebUrl);
            writer.WriteElement("OfficialUrl", OfficialUrl);
            writer.WriteElement("ShowLanguage", ShowLanguage);
            writer.WriteElement("Type", Type);
            writer.WriteElement("imdbId", Imdb);
            writer.WriteElement("rageid",TvRageCode);
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

            writer.WriteStartElement("Seasons");
            foreach (Season a in seasons)
            {
                a.WriteXml(writer);
            }
            writer.WriteEndElement(); //Actors

            writer.WriteStartElement("Aliases");
            foreach (string a in aliases)
            {
                writer.WriteElement("Alias", a);
            }
            writer.WriteEndElement(); //Aliases

            writer.WriteStartElement("Genres");
            foreach (string a in Genres)
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
        public string GetImdbNumber() =>
              Imdb is null? string.Empty
            : Imdb.StartsWith("tt", StringComparison.Ordinal) ? Imdb.RemoveFirst(2)
            : Imdb;

        [NotNull]
        public string GetSeriesFanartPath() => banners.GetSeriesFanartPath();
        [NotNull]
        public string GetSeriesPosterPath() => banners.GetSeriesPosterPath();
        public string? GetImage(TVSettings.FolderJpgIsType itemForFolderJpg) => banners.GetImage(itemForFolderJpg);
        public string? GetSeasonBannerPath(int snum) => banners.GetSeasonBannerPath(snum);
        public string? GetSeriesWideBannerPath() => banners.GetSeriesWideBannerPath();
        public string? GetSeasonWideBannerPath(int snum) => banners.GetSeasonWideBannerPath(snum);
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

        public override string ToString() => $"TVDB:{TvdbCode}/Maze:{TvMazeCode}/{Name}";

        private List<Season> seasons = new List<Season>();
        public void AddSeason(Season generateSeason)
        {
            seasons.Add(generateSeason);
        }

        public void AddAlias(string s)
        {
            aliases.Add(s);
        }

        public Season? Season(int sSeasonNumber)
        {
            return seasons.FirstOrDefault(season => season.SeasonNumber== sSeasonNumber);
        }
    }
}
