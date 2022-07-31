
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TVRename.TheTVDB;

// ReSharper disable once InconsistentNaming
internal static class API
{
    //V4 Doco: https://app.swaggerhub.com/apis/tvdb/tvdb-api-v4/4.0.1#
    //         https://app.swaggerhub.com/apis-docs/thetvdb/tvdb-api_v_4/4.0.0#

    // ReSharper disable once ConvertToConstant.Local
    private static readonly string WebsiteRoot = "https://thetvdb.com";

    // ReSharper disable once ConvertToConstant.Local
    private static readonly string WebsiteImageRoot = "https://artworks.thetvdb.com";

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    // ReSharper disable once InconsistentNaming
    private static readonly TokenProvider TokenProvider = new();

    // ReSharper disable once InconsistentNaming
    public static string GetImageURL(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return string.Empty;
        }

        string mirr = WebsiteImageRoot;

        if (url.StartsWith(mirr, StringComparison.OrdinalIgnoreCase))
        {
            return url;
        }

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

    public static byte[] GetTvdbDownload(string url, bool forceReload)
    {
        string theUrl = GetImageURL(url);

        HttpClient wc = new();

        if (forceReload)
        {
            //TODO wc. = new HttpRequestCachePolicy(HttpRequestCacheLevel.Reload);
        }
        Task<byte[]> task = Task.Run(async () => await wc.GetByteArrayAsync(url));

        byte[] r = task.Result;

        if (!url.EndsWith(".zip", StringComparison.Ordinal))
        {
            Logger.Info("Downloaded " + theUrl + ", " + r.Length + " bytes");
        }

        return r;
    }

    public static string WebsiteShowUrl(ShowConfiguration si)
    {
        string? value = si.CachedShow?.Slug;
        return string.IsNullOrWhiteSpace(value) ? WebsiteShowUrl(si.TvdbCode) : WebsiteShowUrl(value);
    }

    public static string WebsiteShowUrl(CachedSeriesInfo si)
    {
        string? value = si.Slug;
        return string.IsNullOrWhiteSpace(value) ? WebsiteShowUrl(si.TvdbCode) : WebsiteShowUrl(value);
    }

    public static string WebsiteShowUrl(int seriesId)
    {
        //return $"{WebsiteRoot}/series/{seriesId}";
        return $"{WebsiteRoot}/?tab=series&id={seriesId}";
    }

    public static string WebsiteMovieUrl(int id)
    {
        return $"{WebsiteRoot}/?tab=movie&id={id}";
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static string WebsiteShowUrl(string slug)
    {
        return $"{WebsiteRoot}/series/{slug}";
    }

    public static string WebsiteEpisodeUrl(Episode ep)
    {
        return string.IsNullOrWhiteSpace(ep.TheCachedSeries.Slug)
            ? WebsiteEpisodeUrl(ep.TheCachedSeries.TvdbCode, ep.EpisodeId)
            : WebsiteEpisodeUrl(ep.TheCachedSeries.Slug, ep.EpisodeId);
    }

    public static string WebsiteSeasonUrl(ProcessedSeason s)
    {
        string? value = s.Show.CachedShow?.Slug;
        return string.IsNullOrWhiteSpace(value)
            ? WebsiteSeasonUrl(s.Show.TvdbCode, s.Show.Order, s.SeasonNumber)
            : WebsiteSeasonUrl(value, s.Show.Order, s.SeasonNumber);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static string WebsiteSeasonUrl(int seriesId, ProcessedSeason.SeasonType type, int seasonNumber)
    {
        //format: return $"{WebsiteRoot}/?tab=season&seriesid={seriesId}&seasonid={seasonId}";
        return type switch
        {
            ProcessedSeason.SeasonType.dvd => $"{WebsiteRoot}/series/{seriesId}/seasons/dvd/{seasonNumber}",
            ProcessedSeason.SeasonType.aired => $"{WebsiteRoot}/series/{seriesId}/seasons/official/{seasonNumber}",
            ProcessedSeason.SeasonType.alternate => $"{WebsiteRoot}/series/{seriesId}/seasons/alternate/{seasonNumber}",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static string WebsiteSeasonUrl(string slug, ProcessedSeason.SeasonType type, int seasonNumber)
    {
        //format: https://thetvdb.com/series/the-terror/seasons/official/2
        return type switch
        {
            ProcessedSeason.SeasonType.dvd => $"{WebsiteRoot}/series/{slug}/seasons/dvd/{seasonNumber}",
            ProcessedSeason.SeasonType.aired => $"{WebsiteRoot}/series/{slug}/seasons/official/{seasonNumber}",
            ProcessedSeason.SeasonType.alternate => $"{WebsiteRoot}/series/{slug}/seasons/alternate/{seasonNumber}",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static string WebsiteEpisodeUrl(int seriesId, int episodeId)
    {
        // http://www.thetvdb.com/?tab=episode&seriesid=73141&seasonid=5356&id=108303&lid=7
        //return $"{WebsiteRoot}/?tab=episode&seriesid={seriesId}&seasonid={seasonId}&id={episodeId}";

        //New format: https://thetvdb.com/series/the-terror/episodes/7124969
        return episodeId > 0 ? $"{WebsiteRoot}/series/{seriesId}/episodes/{episodeId}" : string.Empty;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static string WebsiteEpisodeUrl(string slug, int episodeId)
    {
        return episodeId > 0 ? $"{WebsiteRoot}/series/{slug}/episodes/{episodeId}" : string.Empty;
    }

    private static JObject? JsonHttpGetRequest(string url, Dictionary<string, string?>? parameters, TokenProvider? authToken, bool retry) =>
        JsonHttpGetRequest(url, parameters, authToken, string.Empty, retry);

    private static JObject? JsonHttpGetRequest(string url, Dictionary<string, string?>? parameters, TokenProvider? authToken, string lang, bool retry)
    {
        TimeSpan pauseBetweenFailures = TimeSpan.FromSeconds(2);
        string fullUrl = url + HttpHelper.GetHttpParameters(parameters);

        string? response = null;

        if (retry)
        {
            if (authToken != null)
            {
                HttpHelper.RetryOnException(3, pauseBetweenFailures, fullUrl,
                    _ => true,
                    () => { response = HttpRequest("GET", fullUrl, "application/json", authToken, lang); },
                    authToken.EnsureValid);
            }
            else
            {
                HttpHelper.RetryOnException(3, pauseBetweenFailures, fullUrl,
                    _ => true,
                    () => { response = HttpRequest("GET", fullUrl, "application/json", null, lang); },
                    () => { });
            }
        }
        else
        {
            response = HttpRequest("GET", fullUrl, "application/json", authToken, lang);
        }

        try
        {
            return response.HasValue() ? JObject.Parse(response) : null;
        }
        catch (JsonReaderException)
        {
            const string ERROR_ON_END = @"{""Error"":""Not authorized""}";
            if (response.HasValue() && response.EndsWith(ERROR_ON_END, StringComparison.Ordinal) && response.Length > ERROR_ON_END.Length)
            {
                return JObject.Parse(response.TrimEnd(ERROR_ON_END));
            }
            throw;
        }
    }

    private static string HttpRequest(string method, string url, string contentType,
        TokenProvider? authToken, string lang = "")
        => HttpHelper.HttpRequest(method, url, null, contentType, authToken?.GetToken(), lang);

    public static JObject? GetShowUpdatesSince(long time, string lang, int page)
    {
        if (TVSettings.Instance.TvdbVersion == ApiVersion.v4)
        {
            return GetShowUpdatesSinceV4(time,lang,page);
        }
        else
        {
            return GetShowUpdatesSinceV3(time, lang);
        }
    }

    public static JObject? GetShowUpdatesSinceV3(long time, string lang)
    {
            return JsonHttpGetRequest(TokenProvider.TVDB_API_URL + "/updated/query",
                new Dictionary<string, string?> { { "fromTime", time.ToString() } },
                TokenProvider, lang, true);
    }
    public static JObject? GetShowUpdatesSinceV4(long time, string lang, int page)
    {
            return JsonHttpGetRequest(TokenProvider.TVDB_API_URL + "/updates",
                new Dictionary<string, string?> { { "since", time.ToString() }, { "page", page.ToString() } },
                TokenProvider, lang, true);
    }

    public static JObject? GetSeriesEpisodes(int seriesId, string languageCode, int pageNumber = 0)
    {
        string episodeUri = $"{TokenProvider.TVDB_API_URL}/series/{seriesId}/episodes";
        return JsonHttpGetRequest(episodeUri,
            new Dictionary<string, string?> { { "page", pageNumber.ToString() } },
            TokenProvider, languageCode, true);
    }

    public static JObject? GetSeriesActors(int seriesId)
    {
        return JsonHttpGetRequest($"{TokenProvider.TVDB_API_URL}/series/{seriesId}/actors",
            null, TokenProvider, false);
    }

    // ReSharper disable once UnusedParameter.Global
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

    public static IEnumerable<string> GetImageTypes(int code, string requestedLanguageCode)
    {
        string uriImages = $"{TokenProvider.TVDB_API_URL}/series/{code}/images";

        try
        {
            JObject? jsonEpisodeSearchResponse = JsonHttpGetRequest(
                uriImages, null, TokenProvider,
                requestedLanguageCode, false);

            if (jsonEpisodeSearchResponse?["data"] is JObject a)
            {
                List<string> imageTypes = new();
                foreach (KeyValuePair<string, JToken?> imageType in a)
                {
                    if ((int?)imageType.Value > 0)
                    {
                        imageTypes.Add(imageType.Key);
                    }
                }
                return imageTypes;
            }
        }
        catch (WebException wex)
        {
            //no images for chosen language
            Logger.LogWebException($"Looking for images, but none found for seriesId {code} via {uriImages} in language {requestedLanguageCode}",wex);
        }
        catch (System.IO.IOException iox)
        {
            //no images for chosen language
            Logger.LogIoException($"Looking for images, but none found for seriesId {code} via {uriImages} in language {requestedLanguageCode}",iox);
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException wex)
        {
            //no images for chosen language
            Logger.LogHttpRequestException($"Looking for images, but none found for seriesId {code} via {uriImages} in language {requestedLanguageCode}",wex);
        }
        return new List<string>();
    }

    public static JObject? SearchTvShow(string text, string defaultLanguageCode)
    {
        string uri = TokenProvider.TVDB_API_URL + "/search/series";
        return JsonHttpGetRequest(uri, new Dictionary<string, string?> { { "name", text } }, TokenProvider, defaultLanguageCode, false);
    }

    public static JObject? SearchV4(string text, string defaultLanguageCode, MediaConfiguration.MediaType media)
    {
        string uri = TokenProvider.TVDB_API_URL + "/search";
        return media switch
        {
            MediaConfiguration.MediaType.tv => JsonHttpGetRequest(uri,
                new Dictionary<string, string?> { { "q", text }, { "type", "series" } }, TokenProvider,
                defaultLanguageCode, false),
            MediaConfiguration.MediaType.movie => JsonHttpGetRequest(uri,
                new Dictionary<string, string?> { { "q", text }, { "type", "movie" } }, TokenProvider, defaultLanguageCode,
                false),
            MediaConfiguration.MediaType.both => JsonHttpGetRequest(uri,
                new Dictionary<string, string?> { { "q", text } }, TokenProvider, defaultLanguageCode, false),
            _ => throw new ArgumentOutOfRangeException(nameof(media), media, null)
        };
    }

    public static bool TvdbIsUp()
    {
        JObject? jsonResponse;
        try
        {
            //Deliberately send no authToken, so that it should fail if it's up
            jsonResponse = JsonHttpGetRequest(TokenProvider.TVDB_API_URL, null, null, false);
        }
        catch (WebException ex)
        {
            //we expect an Unauthorised response - so we know the site is up

            if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse resp)
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
        catch (AggregateException aex) when (aex.InnerException is HttpRequestException ex)
        {
            //we expect an Unauthorised response - so we know the site is up

            return ex.StatusCode switch
            {
                HttpStatusCode.Unauthorized => true,
                HttpStatusCode.Forbidden => true,
                HttpStatusCode.NotFound => false,
                HttpStatusCode.OK => true,
                _ => false
            };
        }
        return jsonResponse?.HasValues ?? false;
    }

    public static List<JObject> GetImages(int code, string languageCode, IEnumerable<string> imageTypes)
    {
        string uriImagesQuery = TokenProvider.TVDB_API_URL + "/series/" + code + "/images/query";
        List<JObject> returnList = new();
        foreach (string imageType in imageTypes)
        {
            try
            {
                JObject? jsonImageResponse = JsonHttpGetRequest(
                    uriImagesQuery,
                    new Dictionary<string, string?> { { "keyType", imageType } }, TokenProvider,
                    languageCode, false);

                if (jsonImageResponse != null)
                {
                    returnList.Add(jsonImageResponse);
                }
            }
            catch (WebException webEx)
            {
                Logger.LogWebException($"Looking for {imageType} images (in {languageCode}), but none found for seriesId {code}:", webEx);
            }
            catch (System.IO.IOException ioe)
            {
                Logger.LogIoException($"Looking for {imageType} images (in {languageCode}), but none found for seriesId {code}: {ioe.LoggableDetails()}",ioe);
            }
            catch (AggregateException ex) when (ex.InnerException is HttpRequestException wex)
            {
                Logger.LogHttpRequestException($"Looking for {imageType} images (in {languageCode}), but none found for seriesId {code}:", wex);
            }
        }

        return returnList;
    }

    public static JObject? GetSeries(int code, string requestedLanguageCode)
    {
        string uri = TokenProvider.TVDB_API_URL + "/series/" + code;
        return JsonHttpGetRequest(uri, null, TokenProvider, requestedLanguageCode, true);
    }

    public static JObject GetSeriesV4(ISeriesSpecifier code, string requestedLanguageCode)
    {
        string uri = $"{TokenProvider.TVDB_API_URL}/series/{code.TvdbId}/extended";
        return GetUrl(code, uri, requestedLanguageCode,MediaConfiguration.MediaType.tv);
    }

    public static JObject GetSeasonV4(ISeriesSpecifier code, int seasonId, string requestLangCode)
    {
        string uri = $"{TokenProvider.TVDB_API_URL}/series/{seasonId}/extended";
        return GetUrl(code,uri, requestLangCode,MediaConfiguration.MediaType.tv);
    }
    public static JObject? GetEpisode(int episodeId, string requestLangCode)
    {
        string uri = $"{TokenProvider.TVDB_API_URL}/episodes/{episodeId}";
        return JsonHttpGetRequest(uri, null, TokenProvider, requestLangCode, true);
    }

    public static void Login(bool forceReconnect)
    {
        if (forceReconnect)
        {
            TokenProvider.Reset();
        }
        TokenProvider.EnsureValid();
    }

    public static JObject? GetMovie(int code, string requestedLanguageCode)
    {
        string uri = TokenProvider.TVDB_API_URL + "/movies/" + code;
        return JsonHttpGetRequest(uri, null, TokenProvider, requestedLanguageCode, true);
    }

    public static JObject GetMovieV4(ISeriesSpecifier code, string requestedLanguageCode)
    {
        string uri = $"{TokenProvider.TVDB_API_URL}/movies/{code.TvdbId}/extended";
        return GetUrl(code, uri, requestedLanguageCode, MediaConfiguration.MediaType.movie);
    }

    public static JObject GetSeasonEpisodesV4(ISeriesSpecifier id, int seasonId, string requestedLanguageCode)
    {
        string uri = $"{TokenProvider.TVDB_API_URL}/seasons/{seasonId}/extended";
        return GetUrl(id, uri, requestedLanguageCode, MediaConfiguration.MediaType.tv);
    }

    public static JObject GetSeriesTranslationsV4(ISeriesSpecifier code, string requestedLanguageCode)
    {
        string uri = $"{TokenProvider.TVDB_API_URL}/series/{code.TvdbId}/translations/{requestedLanguageCode}";
        return GetUrl(code, uri, requestedLanguageCode, MediaConfiguration.MediaType.tv);
    }

    public static JObject GetEpisodeTranslationsV4(ISeriesSpecifier id, int episodeId, string requestedLanguageCode)
    {
        string uri = $"{TokenProvider.TVDB_API_URL}/episodes/{episodeId}/translations/{requestedLanguageCode}";
        return GetUrl(id, uri, requestedLanguageCode,MediaConfiguration.MediaType.tv);
    }

    public static JObject GetMovieTranslationsV4(ISeriesSpecifier code, string requestedLanguageCode)
    {
        string uri = $"{TokenProvider.TVDB_API_URL}/movies/{code.TvdbId}/translations/{requestedLanguageCode}";
        return GetUrl(code, uri, requestedLanguageCode, MediaConfiguration.MediaType.movie);
    }

    private static JObject GetUrl(ISeriesSpecifier? code, string uri, string requestedLanguageCode, MediaConfiguration.MediaType type)
    {
        try
        {
            Logger.Trace($"   Downloading {uri}");
            return JsonHttpGetRequest(uri, null, TokenProvider, requestedLanguageCode, true) ?? throw new SourceConnectivityException($"Looking for {uri} (in {requestedLanguageCode})");
        }
        catch (WebException webEx)
        {
            if (webEx.Status == WebExceptionStatus.ProtocolError && webEx.Response is HttpWebResponse
                    { StatusCode: HttpStatusCode.NotFound })
            {
                Logger.Warn($"Show with Id {code?.TvdbId} is no longer available from TVDB (got a 404).");

                if (TvdbIsUp() && code!=null)
                {
                    string msg = $"Show with TVDB Id {code.TvdbId} is no longer found on TVDB. Please Update";
                    throw new MediaNotFoundException(code, msg, TVDoc.ProviderType.TheTVDB,
                        TVDoc.ProviderType.TheTVDB, type);
                }
            }

            Logger.LogWebException($"Id={code} Looking for {uri} (in {requestedLanguageCode}), but got WebException:", webEx);
            throw new SourceConnectivityException($"Id={code?.TvdbId} Looking for {uri} (in {requestedLanguageCode}) {webEx.Message}");
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException wex)
        {
            if (wex.StatusCode is HttpStatusCode.NotFound)
            {
                Logger.Warn($"Show with Id {code?.TvdbId} is no longer available from TVDB (got a 404) via {uri}.");

                if (TvdbIsUp() && code!=null)
                {
                    string msg = $"Show with TVDB Id {code.TvdbId} is no longer found on TVDB via {uri}. Please Update";
                    throw new MediaNotFoundException(code, msg, TVDoc.ProviderType.TheTVDB,
                        TVDoc.ProviderType.TheTVDB, type);
                }
            }

            Logger.LogHttpRequestException($"Id={code} Looking for {uri} (in {requestedLanguageCode}), but got WebException:", wex);
            throw new SourceConnectivityException($"Id={code?.TvdbId} Looking for {uri} (in {requestedLanguageCode}) {wex.Message}");
        }
        catch (System.IO.IOException ioe)
        {
            Logger.LogIoException($"Id={code} Looking for {uri} (in {requestedLanguageCode}), but got: {ioe.LoggableDetails()}",ioe);
            throw new SourceConnectivityException($"Id={code?.TvdbId} Looking for {uri} (in {requestedLanguageCode}) {ioe.Message}");
        }
    }

    public static JObject ImageTypesV4()
    {
        return GetUrl(null,"https://api4.thetvdb.com/v4/artwork/types", "en",MediaConfiguration.MediaType.both);
    }

    public static string WebsiteMovieUrl(string? serSlug)
    {
        return $"https://www.thetvdb.com/movies/{serSlug}";
    }
}
