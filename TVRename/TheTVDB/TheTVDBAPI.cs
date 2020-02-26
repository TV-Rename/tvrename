using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using NLog;

namespace TVRename.TheTVDB
{
    // ReSharper disable once InconsistentNaming
    static class API
    {
        // ReSharper disable once ConvertToConstant.Local
        private static readonly string WebsiteRoot = "https://thetvdb.com";
        // ReSharper disable once ConvertToConstant.Local
        private static readonly string WebsiteImageRoot = "https://artworks.thetvdb.com";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // ReSharper disable once InconsistentNaming
        private static readonly TokenProvider TokenProvider = new TokenProvider();

        // ReSharper disable once InconsistentNaming
        [NotNull]
        public static string GetImageURL(string url)
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                return String.Empty;
            }

            string mirr = WebsiteImageRoot;

            if (url.StartsWith("/", StringComparison.Ordinal))
            {
                url = url.Substring(1);
            }

            if (!mirr.EndsWith("/", StringComparison.Ordinal))
            {
                mirr += "/";
            }

            return url.StartsWith("banners/", StringComparison.Ordinal) ? mirr + url : mirr + "banners/" + url;
        }
     
        public static byte[] GetTvdbDownload([NotNull] string url, bool forceReload)
        {
            string theUrl = GetImageURL(url);

            WebClient wc = new WebClient();

            if (forceReload)
            {
                wc.CachePolicy = new RequestCachePolicy(RequestCacheLevel.Reload);
            }

            byte[] r = wc.DownloadData(theUrl);

            if (!url.EndsWith(".zip", StringComparison.Ordinal))
            {
                Logger.Info("Downloaded " + theUrl + ", " + r.Length + " bytes");
            }

            return r;
        }

        [NotNull]
        public static string WebsiteShowUrl([NotNull] ShowItem si)
        {
            return String.IsNullOrWhiteSpace(si.TheSeries()?.Slug) ? WebsiteShowUrl(si.TvdbCode) : WebsiteShowUrl(si.TheSeries()?.Slug);
        }

        [NotNull]
        public static string WebsiteShowUrl(int seriesId)
        {
            //return $"{WebsiteRoot}/series/{seriesId}";
            return $"{WebsiteRoot}/?tab=series&id={seriesId}";
        }

        [NotNull]
        // ReSharper disable once MemberCanBePrivate.Global
        public static string WebsiteShowUrl(string slug)
        {
            return $"{WebsiteRoot}/series/{slug}";
        }

        [NotNull]
        public static string WebsiteEpisodeUrl([NotNull] Episode ep)
        {
            if (ep.TheSeries != null)
            {
                return String.IsNullOrWhiteSpace(ep.TheSeries?.Slug)
                    ? WebsiteEpisodeUrl(ep.TheSeries.TvdbCode, ep.EpisodeId)
                    : WebsiteEpisodeUrl(ep.TheSeries.Slug, ep.EpisodeId);
            }

            return String.Empty;
        }

        [NotNull]
        public static string WebsiteSeasonUrl([NotNull] Season s)
        {
            return String.IsNullOrWhiteSpace(s.Show.TheSeries()?.Slug)
                ? WebsiteSeasonUrl(s.Show.TvdbCode, s.Show.Order, s.SeasonNumber)
                : WebsiteSeasonUrl(s.Show.TheSeries()?.Slug, s.Show.Order, s.SeasonNumber);
        }

        [NotNull]
        // ReSharper disable once MemberCanBePrivate.Global
        public static string WebsiteSeasonUrl(int seriesId, Season.SeasonType type, int seasonNumber)
        {
            //format: return $"{WebsiteRoot}/?tab=season&seriesid={seriesId}&seasonid={seasonId}";
            switch (type)
            {
                case Season.SeasonType.dvd:
                    return $"{WebsiteRoot}/series/{seriesId}/seasons/dvd/{seasonNumber}";
                case Season.SeasonType.aired:
                    return $"{WebsiteRoot}/series/{seriesId}/seasons/official/{seasonNumber}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        [NotNull]
        // ReSharper disable once MemberCanBePrivate.Global
        public static string WebsiteSeasonUrl(string slug, Season.SeasonType type, int seasonNumber)
        {
            //format: https://thetvdb.com/series/the-terror/seasons/official/2
            switch (type)
            {
                case Season.SeasonType.dvd:
                    return $"{WebsiteRoot}/series/{slug}/seasons/dvd/{seasonNumber}";
                case Season.SeasonType.aired:
                    return $"{WebsiteRoot}/series/{slug}/seasons/official/{seasonNumber}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        [NotNull]
        // ReSharper disable once MemberCanBePrivate.Global
        public static string WebsiteEpisodeUrl(int seriesId, int episodeId)
        {
            // http://www.thetvdb.com/?tab=episode&seriesid=73141&seasonid=5356&id=108303&lid=7
            //return $"{WebsiteRoot}/?tab=episode&seriesid={seriesId}&seasonid={seasonId}&id={episodeId}";

            //New format: https://thetvdb.com/series/the-terror/episodes/7124969
            return $"{WebsiteRoot}/series/{seriesId}/episodes/{episodeId}";
        }

        [NotNull]
        // ReSharper disable once MemberCanBePrivate.Global
        public static string WebsiteEpisodeUrl(string slug, int episodeId)
        {
            return $"{WebsiteRoot}/series/{slug}/episodes/{episodeId}";
        }

        private static JObject JsonHttpGetRequest(string url, Dictionary<string, string> parameters, TokenProvider authToken, bool retry) =>
            JsonHttpGetRequest(url, parameters, authToken, String.Empty, retry);

        private static JObject JsonHttpGetRequest(string url, Dictionary<string, string> parameters, TokenProvider authToken, string lang, bool retry)
        {
            TimeSpan pauseBetweenFailures = TimeSpan.FromSeconds(2);
            string fullUrl = url + HttpHelper.GetHttpParameters(parameters);

            string response = null;

            if (retry)
            {
                HttpHelper.RetryOnException(3, pauseBetweenFailures, fullUrl,
                    () => { response = HttpRequest("GET", fullUrl, null, "application/json", authToken, lang); },
                    authToken.EnsureValid);
            }
            else
            {
                response = HttpRequest("GET", fullUrl, null, "application/json", authToken, lang);
            }

            return JObject.Parse(response);
        }

        public static JObject GetLanguages()
        {
            return JsonHttpGetRequest(TokenProvider.TVDB_API_URL + "/languages", null, TokenProvider, true);
        }

        [NotNull]
        private static string HttpRequest([NotNull] string method, [NotNull] string url, string json, string contentType,
            [CanBeNull] TokenProvider authToken, string lang = "")
            => HttpHelper.HttpRequest(method, url, json, contentType, authToken?.GetToken(), lang);

        public static JObject GetUpdatesSince(long time, string lang)
        {
            string uri = TokenProvider.TVDB_API_URL + "/updated/query";

            return JsonHttpGetRequest(uri,
                new Dictionary<string, string> { { "fromTime", time.ToString() } },
                TokenProvider, lang, true);
        }

        public static JObject GetSeriesEpisodes(int seriesId, string languageCode, int pageNumber=0)
        {
            string episodeUri = $"{TokenProvider.TVDB_API_URL}/series/{seriesId}/episodes";
            return JsonHttpGetRequest(episodeUri,
                new Dictionary<string, string> { { "page", pageNumber.ToString() } },
                TokenProvider, languageCode, true);
        }

        public static JObject GetSeriesActors(int seriesId)
        {
            return JsonHttpGetRequest($"{TokenProvider.TVDB_API_URL}/series/{seriesId}/actors",
                null, TokenProvider, false);
        }

        [NotNull]
        internal static string BuildUrl(int apiKey, string lang)
            //would rather make this private to hide api key from outside world
            //https://forum.kodi.tv/showthread.php?tid=323588
            //says that we need a format like this:
            //https://api.thetvdb.com/login?{&quot;apikey&quot;:&quot;((API-KEY))&quot;,&quot;id&quot;:((ID))}|Content-Type=application/json
        {
            return $"{TokenProvider.TVDB_API_URL}/login?"
                   + "{'apikey':'" + TokenProvider.TVDB_API_KEY + "','id':" + apiKey + "}"
                   + "|Content-Type=application/json";
        }

        [NotNull]
        public static IEnumerable<string> GetImageTypes(int code, string requestedLanguageCode)
        {
            string uriImages = TokenProvider.TVDB_API_URL + "/series/" + code + "/images";

            List<string> imageTypes = new List<string>();
            try
            {
                JObject jsonEpisodeSearchResponse = JsonHttpGetRequest(
                    uriImages, null, TokenProvider,
                    requestedLanguageCode, false);

                JObject a = (JObject)jsonEpisodeSearchResponse["data"];

                foreach (KeyValuePair<string, JToken> imageType in a)
                {
                    if ((int)imageType.Value > 0)
                    {
                        imageTypes.Add(imageType.Key);
                    }
                }
            }
            catch (WebException)
            {
                //no images for chosen language
                Logger.Warn($"Looking for images, but none found for seriesId {code} via {uriImages} in language {requestedLanguageCode}");
            }

            return imageTypes;
        }

        public static JObject Search(string text, string defaultLanguageCode)
        {
            string uri = TokenProvider.TVDB_API_URL + "/search/series";
            return JsonHttpGetRequest(uri, new Dictionary<string, string> { { "name", text } }, TokenProvider, defaultLanguageCode, false);
        }

        public static bool TvdbIsUp()
        {
            JObject jsonResponse;
            try
            {
                //Deliberately send no authToken, so that it should fail if it's up
                jsonResponse = JsonHttpGetRequest(TokenProvider.TVDB_API_URL, null, null, false);
            }
            catch (WebException ex)
            {
                //we expect an Unauthorised response - so we know the site is up

                if (ex.Status == WebExceptionStatus.ProtocolError && !(ex.Response is null) && ex.Response is HttpWebResponse resp)
                {
                    switch (resp.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            return true;
                        case HttpStatusCode.Forbidden:
                            return true;
                        case HttpStatusCode.NotFound:
                            return false;
                        case HttpStatusCode.OK:
                            return true;
                        default:
                            return false;
                    }
                }

                return false;
            }

            return jsonResponse.HasValues;
        }

        [NotNull]
        public static List<JObject> GetImages(int code, string languageCode, [NotNull] IEnumerable<string> imageTypes)
        {
            string uriImagesQuery = TokenProvider.TVDB_API_URL + "/series/" + code + "/images/query";
            List<JObject> returnList = new List<JObject>();
            foreach (string imageType in imageTypes)
            {
                try
                {
                    JObject jsonImageResponse = JsonHttpGetRequest(
                        uriImagesQuery,
                        new Dictionary<string, string> { { "keyType", imageType } }, TokenProvider,
                        languageCode, false);

                    returnList.Add(jsonImageResponse);
                }
                catch (WebException webEx)
                {
                    if (webEx.IsUnimportant())
                    {
                        Logger.Info(
                            $"Looking for {imageType} images (in {languageCode}), but none found for seriesId {code}: {webEx.LoggableDetails()}");
                    }
                    else
                    {
                        Logger.Warn(
                            $"Looking for {imageType} images (in {languageCode}), but none found for seriesId {code}: {webEx.LoggableDetails()}");
                    }
                }
                catch (IOException ioe)
                {
                    Logger.Error(ioe,
                        $"Looking for {imageType} images (in {languageCode}), but none found for seriesId {code}");
                }
            }

            return returnList;
        }

        public static JObject GetSeries(int code, string requestedLanguageCode)
        {
            string uri = TokenProvider.TVDB_API_URL + "/series/" + code;
            return JsonHttpGetRequest(uri, null, TokenProvider, requestedLanguageCode, true);
        }

        public static JObject GetEpisode(int episodeId, string requestLangCode)
        {
            string uri = $"{TokenProvider.TVDB_API_URL}/episodes/{episodeId}";
            return JsonHttpGetRequest(uri, null, TokenProvider, requestLangCode, true);
        }
    }
}
