//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace TVRename
{
    public class CachedSeriesInfo : CachedMediaInfo
    {
        public DateTime? AirsTime;
        public string? AirsDay;
        public string? Type;

        private ConcurrentDictionary<int, Episode> sourceEpisodes;

        [NotNull]
        public ICollection<Episode> Episodes => sourceEpisodes.Values;

        public void ClearEpisodes() => sourceEpisodes.Clear();

        private ShowImages images = new ShowImages();

        public int? MinYear =>
            Episodes.Select(e => e.GetAirDateDt())
                .Where(adt => adt.HasValue)
                .Select(adt => adt.Value)
                .Min(airDateTime => (int?)airDateTime.Year);

        public int? MaxYear =>
            Episodes.Select(e => e.GetAirDateDt())
                .Where(adt => adt.HasValue)
                .Select(adt => adt.Value)
                .Max(airDateTime => (int?)airDateTime.Year);

        [NotNull]
        public string Year => FirstAired?.ToString("yyyy") ?? $"{MinYear}";

        public IEnumerable<Season> Seasons => seasons;

        // note: "SeriesID" in a <Series> is the tv.com code,
        // "seriesid" in an <Episode> is the tvdb code!

        private CachedSeriesInfo(TVDoc.ProviderType source) : base(source)
        {
            sourceEpisodes = new ConcurrentDictionary<int, Episode>();
            AirsTime = null;
        }

        public CachedSeriesInfo(int tvdb, int tvmaze, int tmdb, Locale langCode, TVDoc.ProviderType source) : base(tvdb, tvmaze, tmdb, langCode, source)
        {
            sourceEpisodes = new ConcurrentDictionary<int, Episode>();
            AirsTime = null;
        }

        public CachedSeriesInfo(Locale locale, TVDoc.ProviderType source) : base(locale, source)
        {
            sourceEpisodes = new ConcurrentDictionary<int, Episode>();
            AirsTime = null;
        }

        public CachedSeriesInfo([NotNull] XElement seriesXml, TVDoc.ProviderType source) : this(source)
        {
            LoadXml(seriesXml);
            IsSearchResultOnly = false;
        }

        public CachedSeriesInfo([NotNull] JObject json, Locale locale, bool searchResult, TVDoc.ProviderType source) : this(locale, source)
        {
            LoadJson(json);
            IsSearchResultOnly = searchResult;

            if (string.IsNullOrEmpty(Name))
            {
                LOGGER.Warn("Issue with cachedSeries " + this);
                LOGGER.Warn(json.ToString());
            }

            if (SrvLastUpdated == 0 && !searchResult)
            {
                LOGGER.Warn("Issue with cachedSeries (update time is 0) " + this);
                LOGGER.Warn(json.ToString());
                SrvLastUpdated = 100;
            }
        }

        public CachedSeriesInfo([NotNull] JObject json, JObject jsonInDefaultLang, Locale locale, TVDoc.ProviderType source) : this(locale, source)
        {
            LoadJson(json, jsonInDefaultLang);
            IsSearchResultOnly = false;
            if (string.IsNullOrEmpty(Name))
            {
                LOGGER.Warn("Issue with cachedSeries " + this);
                LOGGER.Warn(json.ToString());
                LOGGER.Info(jsonInDefaultLang.ToString());
            }

            if (SrvLastUpdated == 0)
            {
                LOGGER.Warn("Issue with cachedSeries (update time is 0) " + this);
                LOGGER.Warn(json.ToString());
                LOGGER.Info(jsonInDefaultLang.ToString());
                SrvLastUpdated = 100;
            }
        }

        // ReSharper disable once FunctionComplexityOverflow
        public void Merge([NotNull] CachedSeriesInfo o)
        {
            if (o.IsSearchResultOnly && !IsSearchResultOnly)
            {
                return;
            }

            if (o.TvdbCode != TvdbCode && o.TvMazeCode != TvMazeCode && o.TmdbCode != TmdbCode)
            {
                return; // that's not us!
            }

            if (o.TvMazeCode != -1 && TvMazeCode != o.TvMazeCode)
            {
                TvMazeCode = o.TvMazeCode;
            }
            if (o.TmdbCode != -1 && TmdbCode != o.TmdbCode)
            {
                TmdbCode = o.TmdbCode;
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

            bool currentLanguageNotSet = ActualLocale is null;
            //            Language optimaLanguage = config o.ActualLocale ?? TVSettings.Instance.PreferredTVDBLanguage;
            // bool newLanguageOptimal = o.ActualLocale.PreferredLanguage == optimaLanguage;
            bool useNewDataOverOld = currentLanguageNotSet || o.SrvLastUpdated >= SrvLastUpdated; //TODO - work out cached language and see what's best || newLanguageOptimal;

            SrvLastUpdated = o.SrvLastUpdated;

            // take the best bits of "o"
            // "o" is always newer/better than us, if there is a choice
            Name = ChooseBetter(Name, useNewDataOverOld, o.Name);
            AirsDay = ChooseBetter(AirsDay, useNewDataOverOld, o.AirsDay);
            Imdb = ChooseBetter(Imdb, useNewDataOverOld, o.Imdb);
            WebUrl = ChooseBetter(WebUrl, useNewDataOverOld, o.WebUrl);
            OfficialUrl = ChooseBetter(OfficialUrl, useNewDataOverOld, o.OfficialUrl);
            ShowLanguage = ChooseBetter(ShowLanguage, useNewDataOverOld, o.ShowLanguage);
            Type = ChooseBetter(Type, useNewDataOverOld, o.Type);
            Overview = ChooseBetter(Overview, useNewDataOverOld, o.Overview);
            BannerString = ChooseBetter(BannerString, useNewDataOverOld, o.BannerString);
            PosterUrl = ChooseBetter(PosterUrl, useNewDataOverOld, o.PosterUrl);
            FanartUrl = ChooseBetter(FanartUrl, useNewDataOverOld, o.FanartUrl);
            TrailerUrl = ChooseBetter(TrailerUrl, useNewDataOverOld, o.TrailerUrl);
            Network = ChooseBetter(Network, useNewDataOverOld, o.Network);
            Runtime = ChooseBetter(Runtime, useNewDataOverOld, o.Runtime);
            SeriesId = ChooseBetter(SeriesId, useNewDataOverOld, o.SeriesId);
            Status = ChooseBetterStatus(Status, useNewDataOverOld, o.Status);
            ContentRating = ChooseBetter(ContentRating, useNewDataOverOld, o.ContentRating);
            Slug = ChooseBetter(Slug, useNewDataOverOld, o.Slug);
            TwitterId = ChooseBetter(TwitterId, useNewDataOverOld, o.TwitterId);
            InstagramId = ChooseBetter(InstagramId, useNewDataOverOld, o.InstagramId);
            FacebookId = ChooseBetter(FacebookId, useNewDataOverOld, o.FacebookId);
            TagLine = ChooseBetter(TagLine, useNewDataOverOld, o.TagLine);
            Country = ChooseBetter(Country, useNewDataOverOld, o.Country);

            if (o.FirstAired.HasValue && (useNewDataOverOld || !FirstAired.HasValue))
            {
                FirstAired = o.FirstAired;
            }

            if (useNewDataOverOld && o.SiteRating > 0)
            {
                SiteRating = o.SiteRating;
            }

            if (useNewDataOverOld && o.Popularity > 0)
            {
                Popularity = o.Popularity;
            }

            if (useNewDataOverOld && o.SiteRatingVotes > 0)
            {
                SiteRatingVotes = o.SiteRatingVotes;
            }

            bool useNewAliases = o.Aliases.Any() && useNewDataOverOld;
            if (!Aliases.Any() || useNewAliases)
            {
                Aliases = o.Aliases;
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

            bool useNewCrew = o.Crew.Any() && useNewDataOverOld;
            if (!Crew.Any() || useNewCrew)
            {
                Crew = o.Crew;
            }

            bool useNewActors = o.Actors.Any() && useNewDataOverOld;
            if (!Actors.Any() || useNewActors)
            {
                Actors = o.Actors;
            }

            if (o.AirsTime != null)
            {
                AirsTime = o.AirsTime;
            }

            if (o.sourceEpisodes.Count != 0)
            {
                sourceEpisodes = o.sourceEpisodes;
            }

            images.MergeImages(o.images);

            if (useNewDataOverOld)
            {
                ActualLocale = o.ActualLocale;
            }

            Dirty = o.Dirty;
            IsSearchResultOnly = o.IsSearchResultOnly;
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
                TvdbCode = seriesXml.ExtractInt("id") ?? throw new SourceConsistencyException("Error Extracting Id for Series", TVDoc.ProviderType.TheTVDB);
                TvMazeCode = seriesXml.ExtractInt("mazeid") ?? -1;
                TmdbCode = seriesXml.ExtractInt("TMDBCode") ?? -1;

                Name = System.Web.HttpUtility.HtmlDecode(
                    XmlHelper.ReadStringFixQuotesAndSpaces(seriesXml.ExtractStringOrNull("SeriesName") ?? seriesXml.ExtractString("seriesName")));

                SrvLastUpdated = seriesXml.ExtractLong("lastupdated") ?? seriesXml.ExtractLong("lastUpdated", 0);
                int? languageId = seriesXml.ExtractInt("LanguageId") ?? seriesXml.ExtractInt("languageId");
                string regionCode = seriesXml.ExtractString("RegionCode");
                ActualLocale = GetLocale(languageId, regionCode);
                string airsTimeString = seriesXml.ExtractStringOrNull("Airs_Time") ?? seriesXml.ExtractString("airsTime");
                AirsTime = JsonHelper.ParseAirTime(airsTimeString);

                AirsDay = seriesXml.ExtractStringOrNull("airsDayOfWeek") ?? seriesXml.ExtractString("Airs_DayOfWeek");
                BannerString = seriesXml.ExtractStringOrNull("banner") ?? seriesXml.ExtractString("Banner");
                Popularity = seriesXml.ExtractDouble("Popularity") ?? 0;
                TwitterId = seriesXml.ExtractStringOrNull("TwitterId");
                InstagramId = seriesXml.ExtractStringOrNull("InstagramId");
                FacebookId = seriesXml.ExtractStringOrNull("FacebookId");
                TagLine = seriesXml.ExtractStringOrNull("TagLine");
                Country = seriesXml.ExtractStringOrNull("Country");

                PosterUrl = seriesXml.ExtractString("posterURL");
                TrailerUrl = seriesXml.ExtractString("TrailerUrl");
                FanartUrl = seriesXml.ExtractString("FanartUrl");
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
                SiteRatingVotes = seriesXml.ExtractInt("siteRatingCount") ?? seriesXml.ExtractInt("SiteRatingCount", 0);
                Slug = seriesXml.ExtractString("slug");
                Popularity = seriesXml.ExtractDouble("Popularity") ?? 0;

                SiteRating = GetSiteRating(seriesXml);
                FirstAired = JsonHelper.ParseFirstAired(seriesXml.ExtractStringOrNull("FirstAired") ?? seriesXml.ExtractString("firstAired"));

                LoadActors(seriesXml);
                LoadCrew(seriesXml);
                LoadAliases(seriesXml);
                LoadGenres(seriesXml);
                LoadSeasons(seriesXml);
                LoadImages(seriesXml);
            }
            catch (SourceConsistencyException e)
            {
                LOGGER.Error(e, GenerateErrorMessage());
                // ReSharper disable once PossibleIntendedRethrow
                throw e;
            }
        }

        internal void AddBanners(int seriesId, IEnumerable<ShowImage> enumerable)
        {
            foreach (ShowImage s in enumerable)
            {
                images.Add(s);
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

        private void LoadImages([NotNull] XElement seriesXml)
        {
            images = new ShowImages();
            foreach (ShowImage s in seriesXml.Descendants("Images").Descendants("ShowImage").Select(xml => new ShowImage(IdCode(Source),Source, xml)))
            {
                images.Add(s);
            }
        }

        private void LoadJson([NotNull] JObject r)
        {
            AirsDay = ((string)r["airsDayOfWeek"])?.Trim();
            string airsTimeString = (string)r["airsTime"];
            AirsTime = JsonHelper.ParseAirTime(airsTimeString);
            Aliases = (r["aliases"] ?? throw new SourceConsistencyException($"Can't find aliases in Series JSON: {r}", TVDoc.ProviderType.TheTVDB)).Select(x => x.Value<string>()).ToList();
            BannerString = (string)r["banner"];
            FirstAired = JsonHelper.ParseFirstAired((string)r["firstAired"]);

            if (r.ContainsKey("genre"))
            {
                Genres = r["genre"]?.Select(x => x.Value<string>()?.Trim()).Distinct().ToList() ?? new List<string>();
            }

            TvdbCode = (int)r["id"];
            Imdb = ((string)r["imdbId"])?.Trim();
            Network = ((string)r["network"])?.Trim();
            Slug = ((string)r["slug"])?.Trim();
            Overview = System.Web.HttpUtility.HtmlDecode((string)r["overview"])?.Trim();
            ContentRating = ((string)r["rating"])?.Trim();
            Runtime = ((string)r["runtime"])?.Trim();
            SeriesId = (string)r["seriesId"];
            string s = (string)r["seriesName"];
            if (s != null)
            {
                Name = System.Web.HttpUtility.HtmlDecode(s).Trim();
            }
            Status = (string)r["status"];

            SrvLastUpdated = long.TryParse((string)r["lastUpdated"], out long updateTime) ? updateTime : 0;

            string siteRatingString = (string)r["siteRating"];
            float.TryParse(siteRatingString, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out SiteRating);

            string siteRatingVotesString = (string)r["siteRatingCount"];
            int.TryParse(siteRatingVotesString, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out SiteRatingVotes);
        }

        [NotNull]
        internal Episode GetEpisode(int epId)
        {
            if (sourceEpisodes.TryGetValue(epId, out Episode returnValue))
            {
                return returnValue;
            }
            throw new ShowConfiguration.EpisodeNotFoundException();
        }

        private void LoadJson([NotNull] JObject bestLanguageR, JObject backupLanguageR)
        {
            //Here we have two pieces of JSON. One in local language and one in the default language (English).
            //We will populate with the best language frst and then fill in any gaps with the backup Language
            LoadJson(bestLanguageR);

            //backupLanguageR should be a cachedSeries of name/value pairs (ie a JArray of JPropertes)
            //TVDB asserts that name and overview are the fields that are localised

            string s = (string)backupLanguageR["seriesName"];
            if (string.IsNullOrWhiteSpace(Name) && s != null)
            {
                Name = System.Web.HttpUtility.HtmlDecode(s);
            }

            string o = (string)backupLanguageR["overview"];
            if (string.IsNullOrWhiteSpace(Overview) && o != null)
            {
                Overview = System.Web.HttpUtility.HtmlDecode(o);
            }

            //Looking at the data then the aliases, banner and runtime are also different by language

            if (!Aliases.Any())
            {
                JToken? aliasesToken = backupLanguageR["aliases"];
                if (aliasesToken is null)
                {
                    throw new SourceConsistencyException($"Can not find aliases in {backupLanguageR}", TVDoc.ProviderType.TheTVDB);
                }
                Aliases = aliasesToken.Select(x => x.Value<string>()).ToList();
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
            writer.WriteElement("mazeid", TvMazeCode);
            writer.WriteElement("TMDBCode", TmdbCode);
            writer.WriteElement("SeriesName", Name);
            writer.WriteElement("lastupdated", SrvLastUpdated);
            writer.WriteElement("LanguageId", ActualLocale?.PreferredLanguage?.TvdbId);
            writer.WriteElement("RegionCode", ActualLocale?.PreferredRegion?.Abbreviation);
            writer.WriteElement("airsDayOfWeek", AirsDay);
            writer.WriteElement("Airs_Time", AirsTime?.ToString("HH:mm"), true);
            writer.WriteElement("banner", BannerString, true);
            writer.WriteElement("TwitterId", TwitterId, true);
            writer.WriteElement("InstagramId", InstagramId, true);
            writer.WriteElement("FacebookId", FacebookId, true);
            writer.WriteElement("TagLine", TagLine, true);
            writer.WriteElement("Country", Country, true);
            writer.WriteElement("posterURL", PosterUrl);
            writer.WriteElement("FanartUrl", FanartUrl);
            writer.WriteElement("TrailerUrl", TrailerUrl, true);
            writer.WriteElement("WebURL", WebUrl,true);
            writer.WriteElement("OfficialUrl", OfficialUrl, true);
            writer.WriteElement("ShowLanguage", ShowLanguage);
            writer.WriteElement("Type", Type, true);
            writer.WriteElement("imdbId", Imdb, true);
            writer.WriteElement("rageid", TvRageCode, true);
            writer.WriteElement("network", Network, true);
            writer.WriteElement("overview", Overview, true);
            writer.WriteElement("rating", ContentRating);
            writer.WriteElement("runtime", Runtime, true);
            writer.WriteElement("seriesId", SeriesId, true);
            writer.WriteElement("status", Status, true);
            writer.WriteElement("siteRating", SiteRating, "0.##");
            writer.WriteElement("siteRatingCount", SiteRatingVotes);
            writer.WriteElement("Popularity", Popularity, "0.##");
            writer.WriteElement("slug", Slug, true);

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

            writer.WriteStartElement("Crew");
            foreach (Crew aa in Crew)
            {
                aa.WriteXml(writer);
            }
            writer.WriteEndElement(); //Crew

            writer.WriteStartElement("Seasons");
            foreach (Season a in seasons)
            {
                a.WriteXml(writer);
            }
            writer.WriteEndElement(); //Actors

            writer.WriteStartElement("Aliases");
            foreach (string a in Aliases)
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

            writer.WriteStartElement("Episodes");
            foreach (Episode e in Episodes)
            {
                e.WriteXml(writer);
            }
            writer.WriteEndElement(); //Episodes

            writer.WriteStartElement("Images");
            foreach (ShowImage i in images)
            {
                i.WriteXml(writer);
            }
            writer.WriteEndElement(); //Images

            writer.WriteEndElement(); // cachedSeries
        }

        public string? GetSeriesFanartPath() => FanartUrl.HasValue() ? FanartUrl:  images.GetShowImage(TargetLocale.LanguageToUse(Source), MediaImage.ImageType.background)?.ImageUrl;

        public string? GetSeriesPosterPath() => PosterUrl.HasValue() ? PosterUrl :  images.GetShowImage(TargetLocale.LanguageToUse(Source), MediaImage.ImageType.poster)?.ImageUrl;

        public string? GetImage(TVSettings.FolderJpgIsType itemForFolderJpg) => images.GetImage(itemForFolderJpg, TargetLocale.LanguageToUse(Source))?.ImageUrl;

        public string? GetSeasonBannerPath(int snum) => images.GetSeasonBanner(snum, TargetLocale.LanguageToUse(Source))?.ImageUrl;

        public string? GetSeriesWideBannerPath() => images.GetShowImage(TargetLocale.LanguageToUse(Source), MediaImage.ImageType.wideBanner)?.ImageUrl;

        public string? GetSeasonWideBannerPath(int snum) => images.GetSeasonWideBanner(snum, TargetLocale.LanguageToUse(Source))?.ImageUrl;

        public void UpdateBanners(List<int> latestBannerIds)
        {
            List<int> bannersToRemove = new List<int>();
            foreach (ShowImage currentImage in images)
            {
                if (latestBannerIds.Contains(currentImage.Id))
                {
                    continue;
                }

                bannersToRemove.Add(currentImage.Id);
            }

            foreach (int removeBanner in bannersToRemove)
            {
                images.RemoveAll(x => x.Id == removeBanner);
            }
        }

        public void AddEpisode([NotNull] Episode episode)
        {
            sourceEpisodes.AddOrUpdate(episode.EpisodeId, episode,(i, episode1) => episode);
            episode.SetSeriesSeason(this);
        }

        public void RemoveEpisode(int episodeId)
        {
            sourceEpisodes.TryRemove(episodeId, out Episode _);
        }

        protected override MediaConfiguration.MediaType MediaType() => MediaConfiguration.MediaType.tv;

        public override string ToString() => $"TVDB:{TvdbCode}/Maze:{TvMazeCode}/TMDB:{TmdbCode}/{Name}";

        private List<Season> seasons = new List<Season>();

        public void AddSeason(Season generateSeason)
        {
            seasons.Add(generateSeason);
        }

        public Season? Season(int sSeasonNumber)
        {
            return seasons.FirstOrDefault(season => season.SeasonNumber == sSeasonNumber);
        }

        public bool IsCacheFor(ShowConfiguration show) => show.TmdbCode == TmdbCode || show.TvdbId == TvdbCode || show.TvMazeId == TvMazeCode;

        public void AddOrUpdateImage(ShowImage showImage)
        {
            images.RemoveAll(s => s.Id == showImage.Id);
            images.Add(showImage);
        }

        public IEnumerable<ShowImage> Images(MediaImage.ImageType type)
        {
            IEnumerable<ShowImage> y = images.Where(x => x.ImageStyle == type);
            return FilterLanguages(y, TargetLocale.LanguageToUse(Source));
        }

        private IEnumerable<ShowImage> FilterLanguages(IEnumerable<ShowImage> showImages, Language languageToUse)
        {
            return showImages.Where(x =>
                x.LanguageCode is null ||
                x.LanguageCode.Equals(languageToUse.ThreeAbbreviation,StringComparison.CurrentCultureIgnoreCase) ||
                x.LanguageCode.Equals(languageToUse.Abbreviation, StringComparison.CurrentCultureIgnoreCase)
                );
        }

        public IEnumerable<ShowImage> Images(MediaImage.ImageType type, MediaImage.ImageSubject subject)
        {
            IEnumerable<ShowImage> y = images.Where(x => x.ImageStyle == type && x.Subject == subject);
            return FilterLanguages(y, TargetLocale.LanguageToUse(Source));
        }
        internal IEnumerable<ShowImage> Images(MediaImage.ImageType type, MediaImage.ImageSubject subject, int seasonNumber)
        {
            IEnumerable<ShowImage> y = images.Where(x => x.ImageStyle == type && x.Subject == subject && x.SeasonNumber == seasonNumber);
            return FilterLanguages(y, TargetLocale.LanguageToUse(Source));
        }
    }
}
