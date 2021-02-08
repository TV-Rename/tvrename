using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace TVRename.TheTVDB
{
    // ReSharper disable once InconsistentNaming
    internal static class API
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
        public static string GetImageURL(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }

            string mirr = WebsiteImageRoot;

            if (url.StartsWith("/", StringComparison.Ordinal))
            {
                url = url.RemoveFirstCharacter();
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
        public static string WebsiteShowUrl([NotNull] ShowConfiguration si)
        {
            string? value = si.CachedShow?.Slug;
            return string.IsNullOrWhiteSpace(value) ? WebsiteShowUrl(si.TvdbCode) : WebsiteShowUrl(value);
        }

        [NotNull]
        public static string WebsiteShowUrl(int seriesId)
        {
            //return $"{WebsiteRoot}/series/{seriesId}";
            return $"{WebsiteRoot}/?tab=cachedSeries&id={seriesId}";
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
            return string.IsNullOrWhiteSpace(ep.TheCachedSeries.Slug)
                ? WebsiteEpisodeUrl(ep.TheCachedSeries.TvdbCode, ep.EpisodeId)
                : WebsiteEpisodeUrl(ep.TheCachedSeries.Slug, ep.EpisodeId);
        }

        [NotNull]
        public static string WebsiteSeasonUrl([NotNull] ProcessedSeason s)
        {
            string? value = s.Show.CachedShow?.Slug;
            return string.IsNullOrWhiteSpace(value)
                ? WebsiteSeasonUrl(s.Show.TvdbCode, s.Show.Order, s.SeasonNumber)
                : WebsiteSeasonUrl(value, s.Show.Order, s.SeasonNumber);
        }

        [NotNull]
        // ReSharper disable once MemberCanBePrivate.Global
        public static string WebsiteSeasonUrl(int seriesId, ProcessedSeason.SeasonType type, int seasonNumber)
        {
            //format: return $"{WebsiteRoot}/?tab=season&seriesid={seriesId}&seasonid={seasonId}";
            switch (type)
            {
                case ProcessedSeason.SeasonType.dvd:
                    return $"{WebsiteRoot}/series/{seriesId}/seasons/dvd/{seasonNumber}";
                case ProcessedSeason.SeasonType.aired:
                    return $"{WebsiteRoot}/series/{seriesId}/seasons/official/{seasonNumber}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        [NotNull]
        // ReSharper disable once MemberCanBePrivate.Global
        public static string WebsiteSeasonUrl(string slug, ProcessedSeason.SeasonType type, int seasonNumber)
        {
            //format: https://thetvdb.com/series/the-terror/seasons/official/2
            switch (type)
            {
                case ProcessedSeason.SeasonType.dvd:
                    return $"{WebsiteRoot}/series/{slug}/seasons/dvd/{seasonNumber}";
                case ProcessedSeason.SeasonType.aired:
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
            return episodeId>0?$"{WebsiteRoot}/series/{seriesId}/episodes/{episodeId}":string.Empty;
        }

        [NotNull]
        // ReSharper disable once MemberCanBePrivate.Global
        public static string WebsiteEpisodeUrl(string slug, int episodeId)
        {
            return episodeId > 0 ? $"{WebsiteRoot}/series/{slug}/episodes/{episodeId}":string.Empty;
        }

        [NotNull]
        private static JObject JsonHttpGetRequest(string url, Dictionary<string, string>? parameters, TokenProvider? authToken, bool retry) =>
            JsonHttpGetRequest(url, parameters, authToken, string.Empty, retry);

        [NotNull]
        private static JObject JsonHttpGetRequest(string url, Dictionary<string, string>? parameters, TokenProvider? authToken, string lang, bool retry)
        {
            TimeSpan pauseBetweenFailures = TimeSpan.FromSeconds(2);
            string fullUrl = url + HttpHelper.GetHttpParameters(parameters);

            string response = null;

            if (retry)
            {
                if (authToken !=null)
                {
                    HttpHelper.RetryOnException(3, pauseBetweenFailures, fullUrl,
                    exception => true,
                    () => { response = HttpRequest("GET", fullUrl, null, "application/json", authToken, lang); },
                    authToken.EnsureValid);

                }
                else
                {
                    HttpHelper.RetryOnException(3, pauseBetweenFailures, fullUrl,
                        exception => true,
                        () => { response = HttpRequest("GET", fullUrl, null, "application/json", null, lang); },
                        () => { });
                }
            }
            else
            {
                response = HttpRequest("GET", fullUrl, null, "application/json", authToken, lang);
            }

            try
            {
                return JObject.Parse(response);
            }
            catch (JsonReaderException)
            {
                const string ERROR_ON_END = @"{""Error"":""Not authorized""}";
                if (response.EndsWith(ERROR_ON_END, StringComparison.Ordinal) && response.Length>ERROR_ON_END.Length)
                {
                    return JObject.Parse(response.TrimEnd(ERROR_ON_END));
                }
                throw;
            }
        }

        [NotNull]
        public static JObject GetLanguages()
        {
            return JsonHttpGetRequest(TokenProvider.TVDB_API_URL + "/languages", null, TokenProvider, true);
        }

        [NotNull]
        private static string HttpRequest([NotNull] string method, [NotNull] string url, string? json, string contentType,
            TokenProvider? authToken, string lang = "")
            => HttpHelper.HttpRequest(method, url, json, contentType, authToken?.GetToken(), lang);

        [NotNull]
        public static JObject GetUpdatesSince(long time, string lang)
        {
            string uri = LocalCache.VERS==ApiVersion.v4
                ? TokenProvider.TVDB_API_URL + "/updates"
                : TokenProvider.TVDB_API_URL + "/updated/query";

            string keyName = LocalCache.VERS == ApiVersion.v4
                ? "since"
                : "fromTime;";

            return JsonHttpGetRequest(uri,
                new Dictionary<string, string> { { keyName, time.ToString() } },
                TokenProvider, lang, true);
        }

        [NotNull]
        public static JObject GetSeriesEpisodes(int seriesId, string languageCode, int pageNumber=0)
        {
            string episodeUri = $"{TokenProvider.TVDB_API_URL}/series/{seriesId}/episodes";
            return JsonHttpGetRequest(episodeUri,
                new Dictionary<string, string> { { "page", pageNumber.ToString() } },
                TokenProvider, languageCode, true);
        }

        [NotNull]
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
            string uriImages = $"{TokenProvider.TVDB_API_URL}/series/{code}/images";
            
            try
            {
                JObject jsonEpisodeSearchResponse = JsonHttpGetRequest(
                    uriImages, null, TokenProvider,
                    requestedLanguageCode, false);

                JObject a = (JObject)jsonEpisodeSearchResponse["data"];

                if (a != null)
                {
                    List<string> imageTypes = new List<string>();
                    foreach (KeyValuePair<string, JToken> imageType in a)
                    {
                        if ((int) imageType.Value > 0)
                        {
                            imageTypes.Add(imageType.Key);
                        }
                    }
                    return imageTypes;
                }
            }
            catch (WebException)
            {
                //no images for chosen language
                Logger.Warn($"Looking for images, but none found for seriesId {code} via {uriImages} in language {requestedLanguageCode}");
            }
            catch (IOException)
            {
                //no images for chosen language
                Logger.Warn($"Looking for images, but none found for seriesId {code} via {uriImages} in language {requestedLanguageCode}");
            }
            return new List<string>();
        }

        [NotNull]
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
                    return resp.StatusCode switch
                    {
                        HttpStatusCode.Unauthorized => true,
                        HttpStatusCode.Forbidden => true,
                        HttpStatusCode.NotFound => false,
                        HttpStatusCode.OK => true,
                        _ => false
                    };
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
                    Logger.LogWebException($"Looking for {imageType} images (in {languageCode}), but none found for seriesId {code}:",webEx);
                }
                catch (IOException ioe)
                {
                    Logger.Warn($"Looking for {imageType} images (in {languageCode}), but none found for seriesId {code}: {ioe.LoggableDetails()}");
                }
            }

            return returnList;
        }

        [NotNull]
        public static JObject GetSeries(int code, string requestedLanguageCode)
        {
            string uri = TokenProvider.TVDB_API_URL + "/series/" + code;
            return JsonHttpGetRequest(uri, null, TokenProvider, requestedLanguageCode, true);
        }

        [NotNull]
        public static JObject GetEpisode(int episodeId, string requestLangCode)
        {
            string uri = $"{TokenProvider.TVDB_API_URL}/episodes/{episodeId}";
            return JsonHttpGetRequest(uri, null, TokenProvider, requestLangCode, true);
        }
    }
}
