using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using NLog;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    static class TheTVDBAPI
    {
        // ReSharper disable once ConvertToConstant.Local
        private static readonly string WebsiteRoot = "https://thetvdb.com";
        // ReSharper disable once ConvertToConstant.Local
        private static readonly string WebsiteImageRoot = "https://artworks.thetvdb.com";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // ReSharper disable once InconsistentNaming
        private static readonly TvDbTokenProvider tvDbTokenProvider = new TvDbTokenProvider();

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

        private static JObject JsonHttpGetRequest(string url, Dictionary<string, string> parameters, TvDbTokenProvider authToken, bool retry) =>
            JsonHttpGetRequest(url, parameters, authToken, String.Empty, retry);

        private static JObject JsonHttpGetRequest(string url, Dictionary<string, string> parameters, TvDbTokenProvider authToken, string lang, bool retry)
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
            return JsonHttpGetRequest(TvDbTokenProvider.TVDB_API_URL + "/languages", null, tvDbTokenProvider, true);
        }

        [NotNull]
        private static string HttpRequest([NotNull] string method, [NotNull] string url, string json, string contentType,
            [CanBeNull] TvDbTokenProvider authToken, string lang = "")
            => HttpHelper.HttpRequest(method, url, json, contentType, authToken?.GetToken(), lang);

        public static JObject GetUpdatesSince(long time, string lang)
        {
            string uri = TvDbTokenProvider.TVDB_API_URL + "/updated/query";

            return JsonHttpGetRequest(uri,
                new Dictionary<string, string> { { "fromTime", time.ToString() } },
                tvDbTokenProvider, lang, true);
        }

        public static JObject GetSeriesEpisodes(int seriesId, string languageCode, int pageNumber=0)
        {
            string episodeUri = $"{TvDbTokenProvider.TVDB_API_URL}/series/{seriesId}/episodes";
            return JsonHttpGetRequest(episodeUri,
                new Dictionary<string, string> { { "page", pageNumber.ToString() } },
                tvDbTokenProvider, languageCode, true);

        }

        public static JObject GetSeriesActors(int seriesId)
        {
            return JsonHttpGetRequest($"{TvDbTokenProvider.TVDB_API_URL}/series/{seriesId}/actors",
                null, tvDbTokenProvider, false);
        }

        [NotNull]
        internal static string BuildUrl(int apiKey, string lang)
            //would rather make this private to hide api key from outside world
            //https://forum.kodi.tv/showthread.php?tid=323588
            //says that we need a format like this:
            //https://api.thetvdb.com/login?{&quot;apikey&quot;:&quot;((API-KEY))&quot;,&quot;id&quot;:((ID))}|Content-Type=application/json
        {
            return $"{TvDbTokenProvider.TVDB_API_URL}/login?"
                   + "{&quot;apikey&quot;:&quot;" + TvDbTokenProvider.TVDB_API_KEY + "&quot;,&quot;id&quot;:" + apiKey + "}"
                   + "|Content-Type=application/json";
        }

        [NotNull]
        public static IEnumerable<string> GetImageTypes(int code, string requestedLanguageCode)
        {
            string uriImages = TvDbTokenProvider.TVDB_API_URL + "/series/" + code + "/images";

            List<string> imageTypes = new List<string>();
            try
            {
                JObject jsonEpisodeSearchResponse = JsonHttpGetRequest(
                    uriImages, null, tvDbTokenProvider,
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
            string uri = TvDbTokenProvider.TVDB_API_URL + "/search/series";
            return JsonHttpGetRequest(uri, new Dictionary<string, string> { { "name", text } }, tvDbTokenProvider, defaultLanguageCode, false);
        }

        public static bool TvdbIsUp()
        {
            JObject jsonResponse;
            try
            {
                //Deliberately send no authToken, so that it should fail if it's up
                jsonResponse = JsonHttpGetRequest(TvDbTokenProvider.TVDB_API_URL, null, null, false);
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
            string uriImagesQuery = TvDbTokenProvider.TVDB_API_URL + "/series/" + code + "/images/query";
            List<JObject> returnList = new List<JObject>();
            foreach (string imageType in imageTypes)
            {
                try
                {
                    JObject jsonImageResponse = JsonHttpGetRequest(
                        uriImagesQuery,
                        new Dictionary<string, string> { { "keyType", imageType } }, tvDbTokenProvider,
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
            string uri = TvDbTokenProvider.TVDB_API_URL + "/series/" + code;
            return JsonHttpGetRequest(uri, null, tvDbTokenProvider, requestedLanguageCode, true);
        }

        public static JObject GetEpisode(int episodeId, string requestLangCode)
        {
            string uri = $"{TvDbTokenProvider.TVDB_API_URL}/episodes/{episodeId}";
            return JsonHttpGetRequest(uri, null, tvDbTokenProvider, requestLangCode, true);
        }
    }

    class TvDbTokenProvider
    {
        [NotNull]
        // ReSharper disable once InconsistentNaming
        public static string TVDB_API_URL
        {
            get
            {
                switch (TheTVDB.VERS)
                {
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    case ApiVersion.v2:
                        return "https://api.thetvdb.com";

                    // ReSharper disable once HeuristicUnreachableCode
                    // ReSharper disable once HeuristicUnreachableCode
                    case ApiVersion.v3:
                        // ReSharper disable once HeuristicUnreachableCode
                        return "https://api-dev.thetvdb.com";

                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public static readonly string TVDB_API_KEY = "5FEC454623154441";

        private string lastKnownToken = string.Empty;
        private DateTime lastRefreshTime = DateTime.MinValue;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public string GetToken()
        {
            //If we have not logged on at all then logon
            if (!IsTokenAquired())
            {
                AcquireToken();
            }
            //If we have logged in but the token has expired so logon again
            if (!TokenIsValid())
            {
                AcquireToken();
            }
            //If we have logged on and have a valid token that is nearing its use-by date then refresh
            if (ShouldRefreshToken())
            {
                RefreshToken();
            }

            return lastKnownToken;
        }

        public void EnsureValid()
        {
            GetToken();
        }

        private void AcquireToken()
        {
            Logger.Info("Acquire a TheTVDB token... ");
            JObject request = new JObject(new JProperty("apikey", TVDB_API_KEY));
            JObject jsonResponse = HttpHelper.JsonHttpPostRequest($"{TVDB_API_URL}/login", request, true);

            UpdateToken((string)jsonResponse["token"]);
            Logger.Info("Performed login at " + DateTime.UtcNow);
            Logger.Info("New Token " + lastKnownToken);
        }

        private void RefreshToken()
        {
            Logger.Info("Refreshing TheTVDB token... ");
            JObject jsonResponse = HttpHelper.JsonHttpGetRequest($"{TVDB_API_URL}/refresh_token", lastKnownToken);

            UpdateToken((string)jsonResponse["token"]);
            Logger.Info("refreshed token at " + DateTime.UtcNow);
            Logger.Info("New Token " + lastKnownToken);
        }

        private void UpdateToken(string token)
        {
            lastKnownToken = token;
            lastRefreshTime = DateTime.Now;
        }

        private bool ShouldRefreshToken() => DateTime.Now - lastRefreshTime >= TimeSpan.FromHours(23);

        private bool TokenIsValid() => DateTime.Now - lastRefreshTime < TimeSpan.FromDays(1) - TimeSpan.FromMinutes(1);

        private bool IsTokenAquired() => lastKnownToken != string.Empty;
    }
}
