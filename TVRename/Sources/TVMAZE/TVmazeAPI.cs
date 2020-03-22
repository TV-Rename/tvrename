using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using NLog;

namespace TVRename.TVmaze
{
    // ReSharper disable once InconsistentNaming
    internal static class API
    {
        // ReSharper disable once ConvertToConstant.Local
        private static readonly string WebsiteRoot = "https://tvmaze.com";
        // ReSharper disable once ConvertToConstant.Local
        // ReSharper disable once InconsistentNaming
        private static readonly string APIRoot = "http://api.tvmaze.com";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [NotNull]
        public static IEnumerable<KeyValuePair<string,long>> GetShowUpdates()
        {
            JObject updatesJson = HttpHelper.HttpGetRequestWithRetry(APIRoot + "/updates/shows", 3, 2);

            return updatesJson.Children<JProperty>().Select(t => new KeyValuePair<string, long>(t.Name, (long) t.Value));
        }

        private static int GetSeriesIdFromOtherCodes(int siTvdbCode,string imdb)
        {
            try
            {
                JObject r = HttpHelper.HttpGetRequestWithRetry(APIRoot + "/lookup/shows?thetvdb=" + siTvdbCode, 3,2);
                int tvMazeId = (int) r["id"];
                return tvMazeId;
            }
            catch (WebException wex)
            {
                if (wex.Is404())
                {
                    string tvdBimbd = TheTVDB.LocalCache.Instance.GetSeries(siTvdbCode)?.Imdb;
                    if (!imdb.HasValue() && !tvdBimbd.HasValue())
                    {
                        throw new SourceConsistencyException($"Please add show {siTvdbCode} to tvMaze", ShowItem.ProviderType.TVmaze);

                    }
                    string imdbCode = imdb ?? tvdBimbd;
                    try
                    {
                        JObject r = HttpHelper.HttpGetRequestWithRetry(APIRoot + "/lookup/shows?imdb=" + imdbCode, 3, 2);
                        int tvMazeId = (int)r["id"];
                        Logger.Fatal($"TVMaze Data issue: {tvMazeId} has the wrong TVDB Id based on {imdbCode}.");
                        return tvMazeId;
                    }
                    catch (WebException wex2)
                    {
                        if (wex2.Is404())
                        {
                            throw new SourceConsistencyException($"Please add show with imdb={imdbCode} and tvdb={siTvdbCode} to tvMaze", ShowItem.ProviderType.TVmaze);
                        }
                        throw new SourceConnectivityException($"Can't find TVmaze series for IMDB={imdbCode} and tvdb={siTvdbCode} {wex.Message}");
                    }

                }
                throw new SourceConnectivityException($"Can't find TVmaze series for {siTvdbCode} {wex.Message}");
            }
        }

        private static JObject GetSeriesDetails(int tvMazeId)
        {
            try
            {
                return HttpHelper.HttpGetRequestWithRetry($"{APIRoot}/shows/{tvMazeId}?specials=1&embed[]=cast&embed[]=episodes&embed[]=crew&embed[]=akas&embed[]=seasons&embed[]=images", 3,2);
            }
            catch (WebException wex)
            {
                if (wex.Is404())
                {
                    throw new SourceConsistencyException($"Please add show maze id {tvMazeId} to tvMaze", ShowItem.ProviderType.TVmaze);
                }
                throw new SourceConnectivityException($"Can't find TVmaze series for {tvMazeId} {wex.Message}");
            }
        }

        [NotNull]
        public static SeriesInfo GetSeriesDetails([NotNull] SeriesSpecifier ss)
        {
            JObject results =  ss.TvMazeSeriesId > 0
                ? GetSeriesDetails(ss.TvMazeSeriesId)
                : GetSeriesDetails(GetSeriesIdFromOtherCodes(ss.TvdbSeriesId,ss.ImdbCode));

            SeriesInfo downloadedSi = GenerateSeriesInfo(results);
            foreach (JToken akaJson in results["_embedded"]["akas"])
            {
                downloadedSi.AddAlias((string)akaJson["name"]);
            }

            List<string> writers = GetWriters(results["_embedded"]["crew"]);
            List<string> directors = GetDirectors(results["_embedded"]["crew"]);
            foreach (JToken epJson in results["_embedded"]["episodes"])
            {
                downloadedSi.AddEpisode(GenerateEpisode(ss.TvMazeSeriesId,writers,directors, (JObject)epJson));
            }

            foreach (JToken jsonSeason in results["_embedded"]["seasons"])
            {
                downloadedSi.AddSeason(GenerateSeason(ss.TvMazeSeriesId, jsonSeason));

                JToken imageNode = jsonSeason["image"];
                if (jsonSeason["image"].HasValues)
                {
                    downloadedSi.AddOrUpdateBanner(GenerateBanner(ss.TvMazeSeriesId,(int)jsonSeason["id"], (int)jsonSeason["number"], (string)imageNode["original"]));
                }
            }

            foreach (JToken imageJson in results["_embedded"]["images"].Where(imageJson => (string)imageJson["type"] == "background"))
            {
                downloadedSi.AddOrUpdateBanner(GenerateBanner(ss.TvMazeSeriesId, imageJson));
            }
            downloadedSi.BannersLoaded = true;

            downloadedSi.ClearActors();
            foreach (JToken jsonActor in results["_embedded"]["cast"])
            {
                downloadedSi.AddActor(GenerateActor(ss.TvMazeSeriesId, jsonActor));
            }
            
            return downloadedSi;
        }

        [NotNull]
        private static List<string> GetWriters(JToken crew)
        {
            return ((JArray) crew).Children<JToken>()
                .Where(token =>token["type"].ToString().EndsWith("Writer", StringComparison.InvariantCultureIgnoreCase))
                .Select(token => token["person"]["name"].ToString()).ToList();
        }
        [NotNull]
        private static List<string> GetDirectors(JToken crew)
        {
            return ((JArray)crew).Children<JToken>()
                .Where(token => token["type"].ToString().EndsWith("Director", StringComparison.InvariantCultureIgnoreCase))
                .Select(token => token["person"]["name"].ToString()).ToList();
        }

        [NotNull]
        private static Banner GenerateBanner(int seriesId, [NotNull] JToken imageJson)
        {
            Banner newBanner = new Banner(seriesId)
            {
                BannerPath = (string) imageJson["resolutions"]["original"]["url"],
                BannerId = (int) imageJson["id"],
                BannerType = "fanart",
                Rating = (bool) imageJson["main"] ? 10 : 1,
                RatingCount = 1
            };

            return newBanner;
        }

        [NotNull]
        private static Banner GenerateBanner(int seriesId,int seasonId, int seasonNumber,[NotNull] string url)
        {
            Banner newBanner = new Banner(seriesId)
            {
                BannerPath = url,
                BannerType = "season",
                SeasonId = seasonNumber,
                Rating = 10,
                RatingCount = 1
            };

            return newBanner;
        }

        [NotNull]
        private static Actor GenerateActor(int seriesId, [NotNull] JToken jsonActor)
        {
            JToken actorImageNode = jsonActor["person"]["image"];
            int actorId = (int) jsonActor["person"]["id"];
            string actorImage = actorImageNode.HasValues ? (string) actorImageNode["medium"] : null;
            string actorName = (string) jsonActor["person"]["name"];
            string actorRole = (string) jsonActor["character"]["name"];
            int actorSortOrder = (int) jsonActor["person"]["id"];
            return new Actor(actorId, actorImage, actorName, actorRole, seriesId,actorSortOrder);
        }

        [NotNull]
        private static Season GenerateSeason(int seriesId, [NotNull] JToken json)
        {
            int id = (int)json["id"];
            int number = (int)json["number"];
            string url = (string) json["url"];
            string name = (string)json["name"];
            string description = (string)json["summary"];
            JToken imageNode = json["image"];
            string imageUrl = imageNode.HasValues ? (string)imageNode["original"] : null; 
            return new Season(id,number,name,description,url,imageUrl,seriesId);
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

        [NotNull]
        private static SeriesInfo GenerateSeriesInfo([NotNull] JObject r)
        {
            string nw = GetKeySubKey(r,"network", "name"); 
            string wc = GetKeySubKey(r,"webChannel", "name");
            string days =  r["schedule"]["days"]?.Select(x => x.Value<string>()).ToCsv();
            int tvdb = (int) (r["externals"]["thetvdb"] ?? 0);
            int rage = r["externals"]["tvrage"].HasValues?(int) r["externals"]["tvrage"] : 0;

            SeriesInfo returnValue = new SeriesInfo
            {
                IsStub = false,
                AirsDay = days,
                AirsTime = ParseAirTime((string)r["schedule"]["time"]),
                FirstAired = ParseFirstAired((string)r["premiered"]),
                TvdbCode = tvdb,
                TvMazeCode = (int)(r["id"]??0),
                TvRageCode = rage,
                Imdb = (string)r["externals"]["imdb"],
                Network = nw??wc,
                WebUrl = ((string)r["url"])?.Trim(),
                PosterUrl = GetUrl(r,"original"),
                OfficialUrl = (string)r["officialSite"],
                ShowLanguage = (string)r["language"],
                Overview = System.Web.HttpUtility.HtmlDecode((string)r["summary"])?.Trim(),
                Runtime = ((string)r["runtime"])?.Trim(),
                Status = MapStatus((string)r["status"]),
                Type = (string)r["type"],
                SrvLastUpdated =
                    long.TryParse((string)r["updated"], out long updateTime)
                        ? updateTime
                        : 0,
            };

            if (r.ContainsKey("genres"))
            {
                returnValue.Genres = r["genres"]?.Select(x => x.Value<string>()).ToList();
            }

            List<string> typesToTransferToGenres = new List<string>{"Animation","Reality","Documentary","News","Sports"};
            foreach (string conversionType in typesToTransferToGenres.Where(s => s==returnValue.Type))
            {
                if (returnValue.Genres is null)
                {
                    returnValue.Genres=new List<string>();
                }
                returnValue.Genres.Add(conversionType);
            }

            if ((string)r["name"] != null)
            {
                returnValue.Name = System.Web.HttpUtility.HtmlDecode((string)r["name"])?.Trim();
            }

            string siteRatingString = ((string)r["rating"]["average"])?.Trim();
            float.TryParse(siteRatingString, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out float parsedSiteRating);
            returnValue.SiteRating = parsedSiteRating;

            string siteRatingVotesString = (string)r["weight"];
            int.TryParse(siteRatingVotesString, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out int parsedSiteRatingVotes);
            returnValue.SiteRatingVotes = parsedSiteRatingVotes;

            return returnValue;
        }

        private static string GetKeySubKey([CanBeNull] JObject r, string key, string firstSubKey)
        {
            if (r is null)
            {
                return null;
            }
            JToken keyVal = r[key];

            switch (keyVal)
            {
                case null:
                    return null;
                case JArray array:
                    return (string) array.First[firstSubKey];
                case JObject o:
                    return (string)o[firstSubKey];
            }
            return null;
        }

        private static string MapStatus(string s)
        {
            if (s == "Running")
            {
                return "Continuing";
            }

            return s;
        }

        private static DateTime? ParseAirTime([CanBeNull] string theTime)
        {
            try
            {
                if (!string.IsNullOrEmpty(theTime))
                {
                    if (DateTime.TryParse(theTime, out DateTime airsTime))
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

        [NotNull]
        private static Episode GenerateEpisode(int seriesId, [NotNull] List<string> writers, [NotNull] List<string> directors, [NotNull] JObject r)
        {
            //Somethign like {
                //"id":1,
                //"url":"http://www.tvmaze.com/episodes/1/under-the-dome-1x01-pilot",
                //"name":"Pilot",
                //"season":1,
                //"number":1,
                //"airdate":"2013-06-24",
                //"airtime":"22:00",
                //"airstamp":"2013-06-25T02:00:00+00:00",
                //"runtime":60,
                //"image":{
                    //"medium":"http://static.tvmaze.com/uploads/images/medium_landscape/1/4388.jpg",
                    //"original":"http://static.tvmaze.com/uploads/images/original_untouched/1/4388.jpg"},
                //"summary":"<p>When the residents of Chester's Mill find themselves trapped under a massive transparent dome with no way out, they struggle to survive as resources rapidly dwindle and panic quickly escalates.</p>",
                //"_links":{"self":{"href":"http://api.tvmaze.com/episodes/1"}}}

            Episode newEp =  new Episode(seriesId)
            {
                FirstAired = ((string)r["airdate"]).HasValue()? (DateTime?)r["airdate"]:null,
                AirTime = ParseAirTime((string?)r["airtime"]),
                AirStamp = r["airstamp"].HasValues? (DateTime?)r["airstamp"] : null,
                EpisodeId = (int)r["id"],
                LinkUrl = ((string)r["url"])?.Trim(),
                Overview = System.Web.HttpUtility.HtmlDecode((string)r["summary"])?.Trim(),
                Runtime = ((string)r["runtime"])?.Trim(),
                Name = ((string)r["name"])?.Trim() ?? string.Empty,
                AiredEpNum = (int)r["number"],
                SeasonId = (int)r["season"],
                AiredSeasonNumber = (int)r["season"],
                Filename = GetUrl(r, "medium")
            };

            newEp.SetWriters(writers);
            newEp.SetDirectors(directors);

            return newEp;
        }

        private static string GetUrl([NotNull] JObject r,string typeKey)
        {
            JToken x = r["image"];
            if (x.HasValues)
            {
                return (string)r["image"][typeKey];
            }

            return null;
        }
    }
}
