using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Humanizer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace TVRename.TheTVDB;

public static class TvdbWebApi
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    #region Login
    // ReSharper disable once InconsistentNaming
    private static readonly TokenProvider TokenProvider = new();
    private static void Login(bool forceReconnect)
    {
        if (forceReconnect)
        {
            TokenProvider.Reset();
        }
        TokenProvider.EnsureValid();
    }

    // ReSharper disable once InconsistentNaming
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    public static bool TVDBLogin()
    {
        const string ERROR_MESSAGE = "Failed to obtain token from TVDB";
        try
        {
            Login(false);
            return true;
        }
        catch (WebException ex)
        {
            Logger.LogWebException(ERROR_MESSAGE, ex);
            throw new SourceConnectivityException(ERROR_MESSAGE, ex);
        }
        catch (HttpRequestException wex)
        {
            Logger.LogHttpRequestException(ERROR_MESSAGE, wex);
            throw new SourceConnectivityException(ERROR_MESSAGE, wex);
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException wex)
        {
            Logger.LogHttpRequestException(ERROR_MESSAGE, wex);
            // ReSharper disable once ThrowFromCatchWithNoInnerException
            throw new SourceConnectivityException(ERROR_MESSAGE, wex);
        }
        catch (IOException ex)
        {
            Logger.LogIoException(ERROR_MESSAGE, ex);
            throw new SourceConnectivityException(ERROR_MESSAGE, ex);
        }
    }

    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    public static bool ReConnect()
    {
        const string ERROR_MESSAGE = "Failed to renew token from TVDB";
        try
        {
            Login(true);
            return true;
        }
        catch (WebException ex)
        {
            Logger.LogWebException(ERROR_MESSAGE, ex);
            throw new SourceConnectivityException(ERROR_MESSAGE, ex);
        }
        catch (HttpRequestException wex)
        {
            Logger.LogHttpRequestException(ERROR_MESSAGE, wex);
            throw new SourceConnectivityException(ERROR_MESSAGE, wex);
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException wex)
        {
            Logger.LogHttpRequestException(ERROR_MESSAGE, wex);
            // ReSharper disable once ThrowFromCatchWithNoInnerException
            throw new SourceConnectivityException(ERROR_MESSAGE, wex);
        }
        catch (IOException ex)
        {
            Logger.LogIoException(ERROR_MESSAGE, ex);
            throw new SourceConnectivityException(ERROR_MESSAGE, ex);
        }
    }
    #endregion

    #region DB access

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    /// <exception cref="MediaNotFoundException">If the show/movie is not found</exception>
    // ReSharper disable once UnusedMember.Local
    private static JObject GetUrlWithErrorHandling(ISeriesSpecifier? code, string uri, Language language)
    {
        return HandleWebErrorsWithNotFoundFor(
            () => {
                Logger.Trace($"   Downloading {uri} in {language.EnglishName} ({language.TVDBCode()}");
                string message = $"Error obtaining {uri} in {language.EnglishName} ({language.TVDBCode()})";
                return JsonHttpGetRequest(uri, null, TokenProvider, language.TVDBCode(), true)
                       ?? throw new SourceConsistencyException(message, TVDoc.ProviderType.TheTVDB);
            },
            $"Error obtaining {code?.Media.PrettyPrint()} with id={code} Looking for {uri} (in {language}), but got an error:",
            code,
            $"{code?.Media.PrettyPrint()} with TVDB Id {code} is no longer found on TVDB. ({uri}) Please Update"
        );
    }

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    private static JObject GetUrl(string uri, Language language)
    {
        return HandleWebErrorsFor(
            () => {
                Logger.Trace($"   Downloading {uri} in {language.EnglishName} ({language.TVDBCode()}");
                string message = $"Error obtaining {uri} in {language.EnglishName} ({language.TVDBCode()})";

                return JsonHttpGetRequest(uri, null, TokenProvider, language.TVDBCode(), true)
                       ?? throw new SourceConsistencyException(message, TVDoc.ProviderType.TheTVDB);
            },
            $"Error obtaining {uri} (in {language}), but got an error:"
        );
    }

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="WebException">Regular WebExceptions</exception>
    private static JObject GetUrl(string uri, Language language,string errorMessage)
    => JsonHttpGetRequest(uri, null, TokenProvider, language.TVDBCode(), true)
       ?? throw new SourceConsistencyException(errorMessage, TVDoc.ProviderType.TheTVDB);
    private static JObject? JsonHttpGetRequest(string url, Dictionary<string, string?>? parameters, TokenProvider? authToken, bool retry) =>
        JsonHttpGetRequest(url, parameters, authToken, string.Empty, retry);
    private static JObject? JsonHttpGetRequest(string url, Dictionary<string, string?>? parameters, TokenProvider? authToken, string lang, bool retry)
    {
        string fullUrl = url + HttpHelper.GetHttpParameters(parameters);

        string? response = null;

        void Operation()
        {
            response = HttpRequest("GET", fullUrl, "application/json", authToken, lang);
        }

        if (retry)
        {
            HttpHelper.RetryOnException(3, 2.Seconds(), fullUrl, _ => true, Operation, () => { authToken?.EnsureValid(); });
        }
        else
        {
            Operation();
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
    private static string HttpRequest(string method, string url, string contentType, TokenProvider? authToken, string lang)
        => HttpHelper.HttpRequest(method, url, null, contentType, authToken?.GetToken(), lang);

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    private static T HandleWebErrorsFor<T>(Func<T> webCall, string errorMessage)
    {
        try
        {
            return webCall();
        }
        catch (IOException ioex)
        {
            Logger.LogIoException($"Error {errorMessage}:", ioex);
            throw new SourceConnectivityException(errorMessage + " - " + ioex.LoggableDetails(), ioex);
        }
        catch (WebException ex)
        {
            Logger.LogWebException($"Error {errorMessage}:", ex);
            throw new SourceConnectivityException(errorMessage + " - " + ex.LoggableDetails(), ex);
        }
        catch (AggregateException ex) when (ex.InnerException is WebException wex)
        {
            Logger.LogWebException($"Error {errorMessage}:", wex);
            // ReSharper disable once ThrowFromCatchWithNoInnerException
            throw new SourceConnectivityException(errorMessage + " - " + wex.LoggableDetails(), wex);
        }
        catch (HttpRequestException ex)
        {
            Logger.LogHttpRequestException($"Error {errorMessage}:", ex);
            throw new SourceConnectivityException(errorMessage + " - " + ex.LoggableDetails(), ex);
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException wex)
        {
            Logger.LogHttpRequestException($"Error {errorMessage}:", wex);
            // ReSharper disable once ThrowFromCatchWithNoInnerException
            throw new SourceConnectivityException(errorMessage + " - " + wex.LoggableDetails(), wex);
        }
        catch (TaskCanceledException ex)
        {
            Logger.LogTaskCanceledException($"Error {errorMessage}:", ex);
            throw new SourceConnectivityException(errorMessage + " - " + ex.LoggableDetails(), ex);
        }
        catch (AggregateException ex) when (ex.InnerException is TaskCanceledException wex)
        {
            Logger.LogTaskCanceledException($"Error {errorMessage}:", wex);
            // ReSharper disable once ThrowFromCatchWithNoInnerException
            throw new SourceConnectivityException(errorMessage + "- " + wex.LoggableDetails(), wex);
        }
    }

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    /// <exception cref="MediaNotFoundException">If the show/movie is not found</exception>
    private static T HandleWebErrorsWithNotFoundFor<T>(Func<T> webCall, string errorMessage, ISeriesSpecifier? code, string? notFoundMessage)
    {
        try
        {
            return webCall();
        }
        catch (IOException ioex)
        {
            Logger.LogIoException($"Error {errorMessage}:", ioex);
            throw new SourceConnectivityException(errorMessage + " - " + ioex.LoggableDetails(), ioex);
        }
        catch (TaskCanceledException ex)
        {
            Logger.LogTaskCanceledException($"Error {errorMessage}:", ex);
            throw new SourceConnectivityException(errorMessage + " - " + ex.LoggableDetails(), ex);
        }
        catch (AggregateException ex) when (ex.InnerException is TaskCanceledException wex)
        {
            Logger.LogTaskCanceledException($"Error {errorMessage}:", wex);
            // ReSharper disable once ThrowFromCatchWithNoInnerException
            throw new SourceConnectivityException(errorMessage + "- " + wex.LoggableDetails(), wex);
        }
        catch (WebException ex)
        {
            ProcessWebException(ex,errorMessage,code,notFoundMessage);
        }
        catch (AggregateException ex) when (ex.InnerException is WebException wex)
        {
            ProcessWebException(wex, errorMessage, code, notFoundMessage);
        }
        catch (HttpRequestException ex)
        {
            ProcessHttpRequestException(ex, errorMessage, code, notFoundMessage);
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException wex)
        {
            ProcessHttpRequestException(wex, errorMessage, code, notFoundMessage);
        }

        throw new SourceConnectivityException("Failed to execute");//should never happen - just to appease compiler

        static void ProcessWebException(WebException ex, string errorMessage, ISeriesSpecifier? code, string? notFoundMessage)
        {
            if (ex.Is404() && code != null)
            {
                RaiseNotFoundIfNeeded(notFoundMessage ?? errorMessage, code);
            }
            Logger.LogWebException($"Error {errorMessage}:", ex);
            throw new SourceConnectivityException(errorMessage + " - " + ex.LoggableDetails(), ex);
        }
        static void ProcessHttpRequestException(HttpRequestException ex, string errorMessage, ISeriesSpecifier? code, string? notFoundMessage)
        {
            if (ex.Is404() && code != null)
            {
                RaiseNotFoundIfNeeded(notFoundMessage ?? errorMessage, code);
            }
            Logger.LogHttpRequestException($"Error {errorMessage}:", ex);
            throw new SourceConnectivityException(errorMessage + " - " + ex.LoggableDetails(), ex);
        }
        static void RaiseNotFoundIfNeeded(string message, ISeriesSpecifier code)
        {
            if (TvdbIsUp())
            {
                Logger.Warn(message);
                throw new MediaNotFoundException(code, message, TVDoc.ProviderType.TheTVDB, TVDoc.ProviderType.TheTVDB, code.Media);
            }
        }
    }
    private static bool TvdbIsUp()
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
            if (ex is { Status: WebExceptionStatus.ProtocolError, Response: HttpWebResponse resp })
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
        catch (HttpRequestException hex)
        {
            //we expect an Unauthorised response - so we know the site is up

            return hex.StatusCode switch
            {
                HttpStatusCode.Unauthorized => true,
                HttpStatusCode.Forbidden => true,
                HttpStatusCode.NotFound => false,
                HttpStatusCode.OK => true,
                _ => false
            };
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
    #endregion

    #region Simple Queries
    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    internal static JObject DownloadMovieTranslations(ISeriesSpecifier code, Language language)
    {
        string uri = $"{TokenProvider.TVDB_API_URL}/movies/{code.TvdbId}/translations/{language.TVDBCode()}";
        string errorMessage = $"obtaining translations for {code} in {language.EnglishName}";

        return HandleWebErrorsFor(() => GetUrl( uri, language, errorMessage),errorMessage);
    }
    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    internal static JObject DownloadSeriesTranslations(ISeriesSpecifier code, Language language)
    {
        string uri = $"{TokenProvider.TVDB_API_URL}/series/{code.TvdbId}/translations/{language.TVDBCode()}";
        string errorMessage = $"obtaining translations for {code.TvdbId} in {language.EnglishName}";

        return HandleWebErrorsFor(() => GetUrl(uri, language, errorMessage), errorMessage);
    }

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    internal static JObject? DownloadEpisode(string showName, int episodeId, Language language)
    {
        string errorMessage = $"Error obtaining {showName} episode[{episodeId}]:";
        string uri = $"{TokenProvider.TVDB_API_URL}/episodes/{episodeId}";
        string requestLangCode = language.Abbreviation; //TODO - check this is right - should be TVDBv4() ?

        return HandleWebErrorsFor(
            () => JsonHttpGetRequest(uri, null, TokenProvider, requestLangCode, true),
            errorMessage);
    }
    // ReSharper disable once UnusedMember.Local

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    internal static JObject ImageTypes()
    {
        string uri = $"{TokenProvider.TVDB_API_URL}/artwork/types";
        return GetUrl( uri, Languages.Instance.FallbackLanguage);
    }
    // ReSharper disable once UnusedMember.Local
    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    internal static JObject GetSeason(ISeriesSpecifier code, Language language, int seasonId)
    {
        string errorMessage = $"Error obtaining season {seasonId} in {language.EnglishName} episodes for [{code}]:";
        string uri = $"{TokenProvider.TVDB_API_URL}/seasons/{seasonId}/extended";

        return HandleWebErrorsFor(() => GetUrl(uri, language, errorMessage), errorMessage);
    }

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    internal static JObject GetSeriesEpisodesOfType(ISeriesSpecifier code, Language language, ProcessedSeason.SeasonType type)
    {
        string uri = $"{TokenProvider.TVDB_API_URL}/series/{code.TvdbId}/episodes/{type.PrettyPrint()}";
        string errorMessage = $"Error obtaining {type.PrettyPrint()} episodes for [{code}] in {language.EnglishName}:";

        return HandleWebErrorsFor(() => GetUrl(uri, language, errorMessage), errorMessage);
    }

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    internal static JObject GetSeasonEpisodes(ISeriesSpecifier code, Language language, int seasonId)
    {
        string errorMessage = $"Error obtaining episodes for [{code}] in {language.TVDBCode()} for season [{seasonId}]:";
        string uri = $"{TokenProvider.TVDB_API_URL}/seasons/{seasonId}/extended";

        return HandleWebErrorsFor(() => GetUrl(uri, language, errorMessage), errorMessage);
    }
    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    internal static JObject GetEpisodeTranslations(ISeriesSpecifier code, Language language, int episodeId)
    {
        string errorMessage = $"Error obtaining episode [{episodeId}] for [{code}] in {language.EnglishName}:";
        string uri = $"{TokenProvider.TVDB_API_URL}/episodes/{episodeId}/translations/{language.TVDBCode()}";

        return HandleWebErrorsFor(() => GetUrl(uri, language, errorMessage), errorMessage);
    }
    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    /// <exception cref="MediaNotFoundException">If the show/movie is not found</exception>
    internal static JObject DownloadMovie(ISeriesSpecifier code, Language language)
    {
        string errorMessage = $"Error obtaining movie {code} in {language.EnglishName}:";
        string notFoundMessage = $"Movie with Id {code} is no longer available from TVDB (got a 404), please update.";
        string uri = $"{TokenProvider.TVDB_API_URL}/movies/{code.TvdbId}/extended";

        return HandleWebErrorsWithNotFoundFor(() => GetUrl(uri, language,errorMessage), errorMessage, code, notFoundMessage);
    }
    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    /// <exception cref="MediaNotFoundException">If the show/movie is not found</exception>
    internal static JObject DownloadSeries(ISeriesSpecifier code, Language language)
    {
        string errorMessage = $"Error obtaining TV series {code} in {language.EnglishName}:";
        string notFoundMessage = $"Show with Id {code} is no longer available from TVDB (got a 404), please update.";
        string uri = $"{TokenProvider.TVDB_API_URL}/series/{code.TvdbId}/extended";

        return HandleWebErrorsWithNotFoundFor(() => GetUrl(uri, language, errorMessage), errorMessage, code, notFoundMessage);
    }

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    internal static JObject? GetUpdates(long updateFromEpochTime, int pageNumber)
    {
        string errorMessage = $"Error obtaining lastupdated query since (local) {updateFromEpochTime}:{pageNumber}";
        string url = $"{TokenProvider.TVDB_API_URL}/updates";
        Dictionary<string, string?> parameters = new() { { "since", updateFromEpochTime.ToString() }, { "page", pageNumber.ToString() } };

        return HandleWebErrorsFor(
            () => JsonHttpGetRequest(url, parameters, TokenProvider, TVSettings.Instance.PreferredTVDBLanguage.Abbreviation, true)
            , errorMessage);
    }
    #endregion

    #region Custom Search Query
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    internal static JObject? SearchResponse(string text, MediaConfiguration.MediaType type, Language language)
    {
        //This is separate from the main queries as we want to handle 404s differently

        string errorMessage = $"Error obtaining results for search term '{text}':";
        string notFoundMessage = $"Could not find any search results for {text} in {language.TVDBCode()}";
        string uri = $"{TokenProvider.TVDB_API_URL}/search";

        try
        {
            return JsonHttpGetRequest(uri,GenerateParmeters(text,type),TokenProvider,language.TVDBCode(), true);
        }
        catch (WebException ex)
        {
            if (ex.Is404())
            {
                Logger.Info(notFoundMessage);
            }
            else
            {
                Logger.LogWebException(errorMessage, ex);
                throw new SourceConnectivityException(errorMessage + "-" + ex.LoggableDetails(), ex);
            }
        }
        catch (HttpRequestException wex)
        {
            if (wex.Is404())
            {
                Logger.Info(notFoundMessage);
            }
            else
            {
                Logger.LogHttpRequestException(errorMessage, wex);
                throw new SourceConnectivityException(errorMessage + "-" + wex.LoggableDetails(), wex);
            }
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException wex)
        {
            if (wex.Is404())
            {
                Logger.Info(notFoundMessage);
            }
            else
            {
                Logger.LogHttpRequestException(errorMessage, wex);
                throw new SourceConnectivityException(errorMessage + "-" + wex.LoggableDetails(), ex);
            }
        }
        catch (IOException ioex)
        {
            Logger.LogIoException($"Error {errorMessage}:", ioex);
            throw new SourceConnectivityException(errorMessage + " - " + ioex.LoggableDetails(), ioex);
        }
        catch (TaskCanceledException ex)
        {
            Logger.LogTaskCanceledException($"Error {errorMessage}:", ex);
            throw new SourceConnectivityException(errorMessage + " - " + ex.LoggableDetails(), ex);
        }
        catch (AggregateException ex) when (ex.InnerException is TaskCanceledException wex)
        {
            Logger.LogTaskCanceledException($"Error {errorMessage}:", wex);
            // ReSharper disable once ThrowFromCatchWithNoInnerException
            throw new SourceConnectivityException(errorMessage + "- " + wex.LoggableDetails(), wex);
        }
        return null;

        static Dictionary<string, string?> GenerateParmeters(string text, MediaConfiguration.MediaType media)
        {
            return media switch
            {
                MediaConfiguration.MediaType.tv => new Dictionary<string, string?> { { "q", text }, { "type", "series" } },
                MediaConfiguration.MediaType.movie => new Dictionary<string, string?> { { "q", text }, { "type", "movie" } },
                MediaConfiguration.MediaType.both => new Dictionary<string, string?> { { "q", text } },
                _ => throw new ArgumentOutOfRangeException(nameof(media), media, null)
            };
        }
    }
    #endregion
}
