using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace TVRename.TVmaze
{
    // ReSharper disable once InconsistentNaming
    internal static class API
    {
        // ReSharper disable once ConvertToConstant.Local
        // ReSharper disable once InconsistentNaming
        private static readonly string APIRoot = "https://api.tvmaze.com";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [NotNull]
        public static IEnumerable<KeyValuePair<string, long>> GetUpdates()
        {
            try
            {
                JObject updatesJson = HttpHelper.HttpGetRequestWithRetry(APIRoot + "/updates/shows", 3, 2);

                return updatesJson.Children<JProperty>()
                    .Select(t => new KeyValuePair<string, long>(t.Name, (long)t.Value));
            }
            catch (WebException ex)
            {
                Logger.LogWebException("Could not get updates from TV Maze due to", ex);
                throw new SourceConnectivityException(ex.Message);
            }
            catch (IOException iex)
            {
                Logger.Error($"Could not get updates from TV Maze due to {iex.Message}");
                throw new SourceConnectivityException(iex.Message);
            }
            catch (JsonReaderException jre)
            {
                Logger.Error($"Could not get updates from TV Maze due to {jre.Message}");
                throw new SourceConnectivityException(jre.Message);
            }
        }

        [NotNull]
        public static IEnumerable<CachedSeriesInfo> ShowSearch(string searchText)
        {
            JArray response;
            try
            {
                string fullUrl = $"{APIRoot}/search/shows?q={searchText}";
                response = HttpHelper.HttpGetArrayRequestWithRetry(fullUrl, 5, 2);
            }
            catch (WebException wex)
            {
                Logger.LogWebException($"Could not search for show '{searchText}' from TV Maze due to", wex);
                throw new SourceConnectivityException($"Can't search TVmaze  for {searchText} {wex.Message}");
            }
            catch (IOException wex)
            {
                Logger.LogIoException($"Could not search for show '{searchText}' from TV Maze due to", wex);
                throw new SourceConnectivityException($"Can't search TVmaze  for {searchText} {wex.Message}");
            }
            return response.Children().Select(ConvertSearchResult);
        }

        private static CachedSeriesInfo? ConvertSearchResult([NotNull] JToken token)
        {
            double score = token["score"].Value<double>();
            JObject show = token["show"].Value<JObject>();
            if (show is null)
            {
                return null;
            }
            CachedSeriesInfo downloadedSi = GenerateSeriesInfo(show);
            downloadedSi.Popularity = score;
            downloadedSi.IsSearchResultOnly = true;
            return downloadedSi;
        }

        private static void GetSeriesIdFromOtherCodes([NotNull] ISeriesSpecifier source)
        {
            try
            {
                JObject r = HttpHelper.HttpGetRequestWithRetry(APIRoot + "/lookup/shows?thetvdb=" + source.TvdbId, 3, 2);
                int tvMazeId = (int)r["id"];

                source.UpdateId(tvMazeId, TVDoc.ProviderType.TVmaze);
            }
            catch (IOException wex)
            {
                throw new SourceConnectivityException($"Can't find TVmaze cachedSeries for {source} {wex.Message}");
            }
            catch (WebException wex)
            {
                if (wex.Is404())
                {
                    string tvdBimbd = TheTVDB.LocalCache.Instance.GetSeries(source.TvdbId)?.Imdb;
                    if (!source.ImdbCode.HasValue() && !tvdBimbd.HasValue())
                    {
                        throw new MediaNotFoundException(source, $"Cant find a show with TVDB Id {source.TvdbId} on TV Maze, either add the show to TV Maze, find the show and update The TVDB Id or use TVDB for that show.", TVDoc.ProviderType.TheTVDB, TVDoc.ProviderType.TVmaze, MediaConfiguration.MediaType.tv);
                    }
                    string imdbCode = source.ImdbCode ?? tvdBimbd;
                    try
                    {
                        JObject r = HttpHelper.HttpGetRequestWithRetry(APIRoot + "/lookup/shows?imdb=" + imdbCode, 3, 2);
                        int tvMazeId = (int)r["id"];
                        JToken externalsToken = GetChild(r, "externals");
                        JToken tvdbToken = GetChild(externalsToken, "thetvdb");
                        int tvdb = tvdbToken.Type == JTokenType.Null ? -1 : (int)tvdbToken;
                        Logger.Error($"TVMaze Data issue: {tvMazeId} has the wrong TVDB Id based on {imdbCode}. Should be {source}, currently is {tvdb}.");

                        source.UpdateId(tvMazeId, TVDoc.ProviderType.TVmaze);
                        return;
                    }
                    catch (WebException wex2)
                    {
                        if (wex2.Is404() && TvMazeIsUp())
                        {
                            throw new MediaNotFoundException(source, $"Please add show with imdb={imdbCode} and tvdb={source.TvdbId} to tvMaze, or use TVDB as the source for that show.", TVDoc.ProviderType.TheTVDB, TVDoc.ProviderType.TVmaze, MediaConfiguration.MediaType.tv);
                        }
                        throw new SourceConnectivityException($"Can't find TVmaze cachedSeries for IMDB={imdbCode} and tvdb={source.TvdbId} {wex.Message}");
                    }
                }
                throw new SourceConnectivityException($"Can't find TVmaze cachedSeries for {source} {wex.Message}");
            }
        }

        private static bool TvMazeIsUp()
        {
            try
            {
                return HttpHelper.HttpGetRequestWithRetry(APIRoot + "/singlesearch/shows?q=girls", 5, 1).HasValues;
            }
            catch (WebException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
        }

        private static JObject GetSeriesDetailsWithMazeId([NotNull] ISeriesSpecifier tvMazeId)
        {
            try
            {
                return HttpHelper.HttpGetRequestWithRetry($"{APIRoot}/shows/{tvMazeId.TvMazeId}?specials=1&embed[]=cast&embed[]=episodes&embed[]=crew&embed[]=akas&embed[]=seasons&embed[]=images", 5, 2);
            }
            catch (WebException wex)
            {
                if (wex.Is404() && TvMazeIsUp())
                {
                    throw new MediaNotFoundException(tvMazeId, $"Please add show maze id {tvMazeId} to tvMaze", TVDoc.ProviderType.TVmaze, TVDoc.ProviderType.TVmaze, MediaConfiguration.MediaType.tv);
                }

                Logger.LogWebException($"Could not get show with id {tvMazeId.TvMazeId} from TV Maze due to", wex);
                throw new SourceConnectivityException($"Can't find TVmaze cachedSeries for {tvMazeId.TvMazeId} {wex.Message}");
            }
            catch (IOException ioe)
            {
                Logger.LogIoException($"Could not get show with id {tvMazeId.TvMazeId} from TV Maze due to", ioe);
                throw new SourceConnectivityException($"Can't find TVmaze cachedSeries for {tvMazeId.TvMazeId} {ioe.Message}");
            }
        }

        [NotNull]
        public static CachedSeriesInfo GetSeriesDetails([NotNull] ISeriesSpecifier ss)
        {
            if (ss.TvMazeId <= 0)
            {
                GetSeriesIdFromOtherCodes(ss);
            }
            JObject results = GetSeriesDetailsWithMazeId(ss);

            CachedSeriesInfo downloadedSi = GenerateSeriesInfo(results);
            JToken jToken = GetChild(results, "_embedded");

            foreach (string name in GetChild(jToken, "akas").Select(akaJson => (string)akaJson["name"]).Where(name => name != null))
            {
                downloadedSi.AddAlias(name);
            }

            List<string> writers = GetWriters(GetChild(jToken, "crew"));
            List<string> directors = GetDirectors(GetChild(jToken, "crew"));
            foreach (JToken epJson in GetChild(jToken, "episodes"))
            {
                downloadedSi.AddEpisode(GenerateEpisode(ss.TvMazeId, writers, directors, (JObject)epJson, downloadedSi));
            }

            foreach (JToken jsonSeason in GetChild(jToken, "seasons"))
            {
                downloadedSi.AddSeason(GenerateSeason(ss.TvMazeId, jsonSeason));

                JToken imageNode = GetChild(jsonSeason, "image");
                if (imageNode.HasValues)
                {
                    string child = (string)GetChild(imageNode, "original");
                    if (child != null)
                    {
                        downloadedSi.AddOrUpdateImage(GenerateImage(ss.TvMazeId, (int)jsonSeason["number"], child));
                    }
                }
            }

            foreach (JToken imageJson in GetChild(jToken, "images"))
            {
                downloadedSi.AddOrUpdateImage(GenerateImage(ss.TvMazeId, imageJson));
            }

            downloadedSi.ClearActors();
            foreach (JToken jsonActor in GetChild(jToken, "cast"))
            {
                downloadedSi.AddActor(GenerateActor(ss.TvMazeId, jsonActor));
            }

            return downloadedSi;
        }

        [NotNull]
        private static List<string> GetWriters(JToken crew)
        {
            return ((JArray)crew).Children<JToken>()
                .Where(token =>
                {
                    JToken typeToken = GetChild(token, "type");
                    return typeToken.ToString().EndsWith("Writer", StringComparison.InvariantCultureIgnoreCase);
                })
                .Select(token =>
                {
                    JToken personTokenToken = GetChild(token, "person");
                    return (string)personTokenToken["name"];
                }).ToList();
        }

        [NotNull]
        private static List<string> GetDirectors(JToken crew)
        {
            return ((JArray)crew).Children<JToken>()
                .Where(token =>
                {
                    JToken typeToken = GetChild(token, "type");
                    return typeToken.ToString().EndsWith("Director", StringComparison.InvariantCultureIgnoreCase);
                })
                .Select(token =>
                {
                    JToken personTokenToken = GetChild(token, "person");
                    return (string)personTokenToken["name"];
                }).ToList();
        }

        [NotNull]
        private static ShowImage GenerateImage(int seriesId, [NotNull] JToken imageJson)
        {
            ShowImage newBanner = new()
            {
                SeriesId = seriesId,
                ImageUrl = (string)GetChild(GetChild(GetChild(imageJson, "resolutions"), "original"), "url"),
                Id = (int)imageJson["id"],
                ImageStyle = MapImageType((string)imageJson["type"]),
                Rating = (bool)imageJson["main"] ? 10 : 1,
                RatingCount = 1,
                SeriesSource = TVDoc.ProviderType.TVmaze,
            };

            return newBanner;
        }

        private static MediaImage.ImageType MapImageType(string? s)
        {
            switch (s)
            {
                case null:
                case "background":
                    return MediaImage.ImageType.background;
                case "poster":
                    return MediaImage.ImageType.poster;
                case "banner":
                    return MediaImage.ImageType.wideBanner;
                default:
                    return MediaImage.ImageType.background;
            }
        }

        [NotNull]
        private static ShowImage GenerateImage(int seriesId, int seasonNumber, [NotNull] string url)
        {
            ShowImage newBanner = new()
            {
                SeriesId = seriesId,
                ImageUrl = url,
                ImageStyle = MediaImage.ImageType.poster,
                Subject = MediaImage.ImageSubject.season,
                SeasonId = seasonNumber,
                Rating = 10,
                RatingCount = 1,
                SeriesSource = TVDoc.ProviderType.TVmaze,
            };

            return newBanner;
        }

        [NotNull]
        private static Actor GenerateActor(int seriesId, [NotNull] JToken jsonActor)
        {
            JToken personToken = GetChild(jsonActor, "person");
            JToken actorImageNode = GetChild(personToken, "image");
            int actorId = (int)personToken["id"];
            string? actorImage = actorImageNode.HasValues ? (string)actorImageNode["medium"] : null;
            string actorName = (string)personToken["name"] ?? throw new SourceConsistencyException("No Actor Name", TVDoc.ProviderType.TVmaze);
            string actorRole = (string)GetChild(GetChild(jsonActor, "character"), "name");
            int actorSortOrder = (int)personToken["id"];
            return new Actor(actorId, actorImage, actorName, actorRole, seriesId, actorSortOrder);
        }

        [NotNull]
        private static Season GenerateSeason(int seriesId, [NotNull] JToken json)
        {
            int id = (int)json["id"];
            int number = (int)json["number"];
            string url = (string)json["url"];
            string name = (string)json["name"];
            string description = (string)json["summary"];
            JToken imageNode = GetChild(json, "image");
            string imageUrl = imageNode.HasValues ? (string)imageNode["original"] : null;
            return new Season(id, number, name, StripPTags(description ?? string.Empty), url, imageUrl, seriesId);
        }

        [NotNull]
        private static string StripPTags([NotNull] string description)
        {
            return description.TrimStartString("<p>").TrimEnd("</p>");
        }

        [NotNull]
        private static CachedSeriesInfo GenerateSeriesInfo([NotNull] JObject r)
        {
            CachedSeriesInfo returnValue = GenerateCoreSeriesInfo(r);

            if (r.ContainsKey("genres"))
            {
                returnValue.Genres = r["genres"]?.Select(x => x.Value<string>()?.Trim()).Distinct().ToSafeList() ?? new SafeList<string>();
            }

            List<string> typesToTransferToGenres = new() { "Animation", "Reality", "Documentary", "News", "Sports" };
            foreach (string conversionType in typesToTransferToGenres.Where(s => s == returnValue.SeriesType))
            {
                returnValue.Genres.Add(conversionType);
            }

            string s1 = (string)r["name"];
            if (s1 != null)
            {
                returnValue.Name = System.Web.HttpUtility.HtmlDecode(s1).Trim();
            }

            string siteRatingString = ((string)GetChild(r, "rating")["average"])?.Trim();
            float.TryParse(siteRatingString, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out float parsedSiteRating);
            returnValue.SiteRating = parsedSiteRating;

            string siteRatingVotesString = (string)r["weight"];
            int.TryParse(siteRatingVotesString, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out int parsedSiteRatingVotes);
            returnValue.SiteRatingVotes = parsedSiteRatingVotes;

            return returnValue;
        }

        [NotNull]
        private static CachedSeriesInfo GenerateCoreSeriesInfo([NotNull] JObject r)
        {
            string nw = GetKeySubKey(r, "network", "name");
            string wc = GetKeySubKey(r, "webChannel", "name");
            string days = GetChild(r, "schedule")["days"]?.Select(x => x.Value<string>()).ToCsv();
            JToken externalsToken = GetChild(r, "externals");
            int tvdb = GetChild(externalsToken, "thetvdb").Type == JTokenType.Null ? -1 : (int)externalsToken["thetvdb"];
            int rage = GetChild(externalsToken, "tvrage").Type == JTokenType.Null ? -1 : (int)externalsToken["tvrage"];
            
            return new CachedSeriesInfo(new Locale(), TVDoc.ProviderType.TVmaze)
            {
                IsSearchResultOnly = false,
                AirsDay = days,
                AirsTime = JsonHelper.ParseAirTime((string)GetChild(r, "schedule")["time"]),
                FirstAired = JsonHelper.ParseFirstAired((string)r["premiered"]),
                TvdbCode = tvdb,
                TvMazeCode = (int)(r["id"] ?? 0),
                TvRageCode = rage,
                Imdb = (string)externalsToken["imdb"],
                Network = nw ?? wc,
                WebUrl = ((string)r["url"])?.Trim(),
                PosterUrl = GetUrl(r, "original"),
                OfficialUrl = (string)r["officialSite"],
                ShowLanguage = (string)r["language"],
                Overview = System.Web.HttpUtility.HtmlDecode((string)r["summary"])?.Trim(),
                Runtime = ((string)r["runtime"])?.Trim(),
                Status = MapStatus((string)r["status"] ?? throw new SourceConsistencyException("No Status", TVDoc.ProviderType.TVmaze)),
                SeriesType = (string)r["type"],
                SrvLastUpdated =
                    long.TryParse((string)r["updated"], out long updateTime)
                        ? updateTime
                        : 0,
                Dirty = false,
                SeasonOrderType = ProcessedSeason.SeasonType.aired,
            };
        }

        private static string? GetKeySubKey(JObject? r, string key, string firstSubKey)
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
                    if (array.First != null)
                    {
                        return (string)array.First[firstSubKey];
                    }

                    return null;

                case JObject o:
                    return (string)o[firstSubKey];

                default:
                    return null;
            }
        }

        private static string MapStatus(string s)
        {
            if (s == "Running")
            {
                return "Continuing";
            }

            return s;
        }

        [NotNull]
        private static Episode GenerateEpisode(int seriesId, [NotNull] List<string> writers, [NotNull] List<string> directors, [NotNull] JObject r, CachedSeriesInfo si)
        {
            //Something like {
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

            JToken airstampToken = GetChild(r, "airstamp");

            Episode newEp = new(seriesId, si)
            {
                FirstAired = ((string)r["airdate"]).HasValue() ? (DateTime?)r["airdate"] : null,
                AirTime = JsonHelper.ParseAirTime((string)r["airtime"]),
                AirStamp = airstampToken.HasValues ? (DateTime?)airstampToken : null,
                EpisodeId = (int)r["id"],
                LinkUrl = ((string)r["url"])?.Trim(),
                Overview = System.Web.HttpUtility.HtmlDecode((string)r["summary"])?.Trim(),
                Runtime = ((string)r["runtime"])?.Trim(),
                Name = ((string)r["name"])?.Trim() ?? string.Empty,
                AiredEpNum = (int)r["number"],
                SeasonId = (int)r["season"],
                AiredSeasonNumber = (int)r["season"],
                Filename = GetUrl(r, "medium"),
                ReadDvdSeasonNum = 0,
                DvdEpNum = 0
            };

            newEp.SetWriters(writers);
            newEp.SetDirectors(directors);

            return newEp;
        }

        [NotNull]
        private static JToken GetChild([NotNull] JToken json, [NotNull] string key)
        {
            JToken? token = json[key];
            if (token is null)
            {
                throw new SourceConsistencyException($"Could not get '{key}' element from {json}", TVDoc.ProviderType.TVmaze);
            }

            return token;
        }

        private static string? GetUrl([NotNull] JObject r, string typeKey)
        {
            JToken x = r["image"];
            if (x is null)
            {
                throw new SourceConsistencyException($"Could not get 'image' element from {r}", TVDoc.ProviderType.TVmaze);
            }

            if (x.HasValues)
            {
                return (string)x[typeKey];
            }

            return null;
        }
    }
}
