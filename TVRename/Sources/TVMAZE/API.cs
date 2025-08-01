using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace TVRename.TVmaze;

// ReSharper disable once InconsistentNaming
internal static class API
{
    // ReSharper disable once ConvertToConstant.Local
    // ReSharper disable once InconsistentNaming
    private static readonly string APIRoot = "https://api.tvmaze.com";

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    private static T HandleErrorsFrom<T>(string message, Func<T> handler)
    {
        string errorMessage = $"Could not {message} from TV Maze";
        try
        {
            return handler();
        }
        catch (WebException ex)
        {
            Logger.LogWebException(errorMessage, ex);
            throw new SourceConnectivityException(errorMessage, ex);
        }
        catch (HttpRequestException wex)
        {
            Logger.LogHttpRequestException(errorMessage, wex);
            throw new SourceConnectivityException(errorMessage, wex);
        }
        catch (System.IO.IOException iex)
        {
            Logger.Error(iex, errorMessage);
            throw new SourceConnectivityException(errorMessage, iex);
        }
        catch (JsonReaderException jre)
        {
            Logger.Error($"{errorMessage} due to {jre.ErrorText()}");
            throw new SourceConsistencyException($"{errorMessage} due to {jre.Message}", TVDoc.ProviderType.TVmaze,jre);
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException wex)
        {
            Logger.LogHttpRequestException(errorMessage, wex);
            // ReSharper disable once ThrowFromCatchWithNoInnerException
            throw new SourceConnectivityException(errorMessage, wex);
        }
        catch (System.Threading.Tasks.TaskCanceledException ex)
        {
            Logger.Warn($"{errorMessage} due to {ex.ErrorText()}");
            throw new SourceConnectivityException(errorMessage, ex);
        }
        catch (AggregateException aex) when (aex.InnerException is System.Threading.Tasks.TaskCanceledException ex)
        {
            Logger.Warn($"{errorMessage} due to {ex.ErrorText()}");
            // ReSharper disable once ThrowFromCatchWithNoInnerException
            throw new SourceConnectivityException(errorMessage, ex);
        }
    }

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    /// <exception cref="MediaNotFoundException">If the show/movie is not found</exception>
    private static T HandleErrorsFrom<T>(string message, Func<T> handler, string mediaNotFoundMessage,ISeriesSpecifier tvMazeId)
    {
        string errorMessage = $"Could not {message} from TV Maze";
        try
        {
            return handler();
        }
        catch (WebException ex)
        {
            if (ex.Is404() && TvMazeIsUp())
            {
                throw new MediaNotFoundException(tvMazeId, mediaNotFoundMessage, TVDoc.ProviderType.TVmaze, TVDoc.ProviderType.TVmaze, MediaConfiguration.MediaType.tv,ex);
            }

            Logger.LogWebException(errorMessage, ex);
            throw new SourceConnectivityException(errorMessage,ex);
        }
        catch (HttpRequestException wex)
        {
            if (wex.Is404() && TvMazeIsUp())
            {
                throw new MediaNotFoundException(tvMazeId, mediaNotFoundMessage, TVDoc.ProviderType.TVmaze, TVDoc.ProviderType.TVmaze, MediaConfiguration.MediaType.tv,wex);
            }
            Logger.LogHttpRequestException(errorMessage, wex);
            throw new SourceConnectivityException(errorMessage, wex);
        }
        catch (System.IO.IOException iex)
        {
            Logger.LogIoException(errorMessage, iex);
            throw new SourceConnectivityException(errorMessage, iex);
        }
        catch (JsonReaderException jre)
        {
            Logger.Error($"{errorMessage} due to {jre.ErrorText()}");
            throw new SourceConsistencyException($"{errorMessage} due to {jre.Message}", TVDoc.ProviderType.TVmaze,jre);
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException wex)
        {
            if (wex.Is404() && TvMazeIsUp())
            {
                // ReSharper disable once ThrowFromCatchWithNoInnerException
                throw new MediaNotFoundException(tvMazeId, mediaNotFoundMessage, TVDoc.ProviderType.TVmaze, TVDoc.ProviderType.TVmaze, MediaConfiguration.MediaType.tv,wex);
            }

            Logger.LogHttpRequestException(errorMessage, wex);
            // ReSharper disable once ThrowFromCatchWithNoInnerException
            throw new SourceConnectivityException(errorMessage,wex);
        }
        catch (System.Threading.Tasks.TaskCanceledException ex)
        {
            Logger.Warn($"{errorMessage} due to {ex.ErrorText()}");
            throw new SourceConnectivityException(errorMessage,ex);
        }
        catch (AggregateException aex) when (aex.InnerException is System.Threading.Tasks.TaskCanceledException ex)
        {
            Logger.Warn($"{errorMessage} due to {ex.ErrorText()}");
            // ReSharper disable once ThrowFromCatchWithNoInnerException
            throw new SourceConnectivityException(errorMessage, ex);
        }
    }

    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    public static IEnumerable<KeyValuePair<string, long>> GetUpdates()
    {
        string fullUrl = $"{APIRoot}/updates/shows";

        JObject updatesJson = HandleErrorsFrom("get updates", () => HttpHelper.HttpGetRequestWithRetry(fullUrl, 3, 2));

        return updatesJson.Children<JProperty>()
            .Select(t => new KeyValuePair<string, long>(t.Name, (long)t.Value));
    }

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    public static IEnumerable<CachedSeriesInfo> ShowSearch(string searchText)
    {
        string message = $"search TVmaze for show '{searchText}'";
        string fullUrl = $"{APIRoot}/search/shows?q={searchText}";

        JArray response = HandleErrorsFrom(message, () => HttpHelper.HttpGetArrayRequestWithRetry(fullUrl, 5, 2));

        return response.Children().Select(ConvertSearchResult).OfType<CachedSeriesInfo>();
    }

    private static CachedSeriesInfo? ConvertSearchResult(JToken token)
    {
        double? score = token["score"]?.Value<double?>();
        JObject? show = token["show"]?.Value<JObject?>();
        if (show is null)
        {
            return null;
        }
        CachedSeriesInfo downloadedSi = GenerateSeriesInfo(show);
        downloadedSi.Popularity = score;
        downloadedSi.IsSearchResultOnly = true;
        return downloadedSi;
    }

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    /// <exception cref="MediaNotFoundException">Condition.</exception>
    private static void GetSeriesIdFromOtherCodes(ISeriesSpecifier source)
    {
        try
        {
            string url = $"{APIRoot}/lookup/shows?thetvdb={source.TvdbId}";
            JObject r = HttpHelper.HttpGetRequestWithRetry(url, 3, 2);
            int tvMazeId = r.GetMandatoryInt("id", TVDoc.ProviderType.TVmaze);

            source.UpdateId(tvMazeId, TVDoc.ProviderType.TVmaze);
        }
        catch (System.IO.IOException wex)
        {
            throw new SourceConnectivityException($"Can't find TVmaze cachedSeries for {source} {wex.Message}",wex);
        }
        catch (AggregateException ex1) when (ex1.InnerException is HttpRequestException wex)
        {
            if (!wex.Is404())
            {
                // ReSharper disable once ThrowFromCatchWithNoInnerException
                throw new SourceConnectivityException($"Can't find TVmaze cachedSeries for {source} {wex.Message}",wex);
            }

            GetSeriesIdFromImdbCode(source, GuessImdbCode(source));
        }

        static string GuessImdbCode(ISeriesSpecifier seriesSpecifier)
        {
            if (seriesSpecifier.ImdbCode.HasValue())
            {
                return seriesSpecifier.ImdbCode;
            }
            string? tvdBimbd = TheTVDB.LocalCache.Instance.GetSeries(seriesSpecifier.TvdbId)?.Imdb;
            if (tvdBimbd.HasValue())
            {
                return tvdBimbd;
            }

            throw new MediaNotFoundException(seriesSpecifier,
                $"Cant find a show with TVDB Id {seriesSpecifier.TvdbId} on TV Maze, either add the show to TV Maze, find the show and update The TVDB Id or use TVDB for that show.",
                TVDoc.ProviderType.TheTVDB, TVDoc.ProviderType.TVmaze, MediaConfiguration.MediaType.tv);
        }
    }

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    /// <exception cref="MediaNotFoundException">Condition.</exception>
    private static void GetSeriesIdFromImdbCode(ISeriesSpecifier source, string s)
    {
        try
        {
            string url = $"{APIRoot}/lookup/shows?imdb={s}";
            JObject r = HttpHelper.HttpGetRequestWithRetry(url, 3, 2);

            int tvMazeId = r.GetMandatoryInt("id", TVDoc.ProviderType.TVmaze);
            JToken externalsToken = GetChild(r, "externals");
            JToken tvdbToken = GetChild(externalsToken, "thetvdb");
            int tvdb = tvdbToken.Type == JTokenType.Null ? -1 : (int)tvdbToken;

            if (source.TvdbId > 0)
            {
                Logger.Error(
                    $"TVMaze Data issue: {tvMazeId} has the wrong TVDB Id based on {s}. Should be {source.TvdbId}, currently is {tvdb}. [{source}]");
            }

            source.UpdateId(tvMazeId, TVDoc.ProviderType.TVmaze);
        }
        catch (HttpRequestException wex2)
        {
            RaiseException(wex2, s);
        }
        catch (AggregateException ex2) when (ex2.InnerException is HttpRequestException wex2)
        {
            RaiseException(wex2, s);
        }

        void RaiseException(HttpRequestException wex2, string? imdbCode)
        {
            if (wex2.Is404() && TvMazeIsUp())
            {
                throw new MediaNotFoundException(source,
                    $"Please add show with imdb={imdbCode} and tvdb={source.TvdbId} to tvMaze, or use TVDB as the source for that show.",
                    TVDoc.ProviderType.TheTVDB, TVDoc.ProviderType.TVmaze, MediaConfiguration.MediaType.tv);
            }

            throw new SourceConnectivityException(
                $"Can't find TVmaze cachedSeries for IMDB={imdbCode} and tvdb={source.TvdbId} {wex2.Message}",wex2);
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
        catch (HttpRequestException)
        {
            return false;
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException)
        {
            return false;
        }
        catch (System.IO.IOException)
        {
            return false;
        }
    }

    /// <exception cref="SourceConsistencyException">Condition.</exception>
    /// <exception cref="SourceConnectivityException">Condition.</exception>
    /// <exception cref="MediaNotFoundException">Condition.</exception>
    private static JObject GetSeriesDetailsWithMazeId(ISeriesSpecifier tvMazeId)
    {
        string errorMessage = $"Can't find TVmaze cachedSeries for {tvMazeId.TvMazeId}";
        string mediaNotFoundMessage = $"Please add show maze id {tvMazeId} to tvMaze";
        string fullUrl = $"{APIRoot}/shows/{tvMazeId.TvMazeId}?specials=1&embed[]=cast&embed[]=episodes&embed[]=crew&embed[]=akas&embed[]=seasons&embed[]=images";

        return HandleErrorsFrom(errorMessage, () => HttpHelper.HttpGetRequestWithRetry(fullUrl, 5, 2), mediaNotFoundMessage, tvMazeId);
    }

    /// <exception cref="SourceConsistencyException">Condition.</exception>
    /// <exception cref="MediaNotFoundException">Condition.</exception>
    /// <exception cref="SourceConnectivityException">Condition.</exception>
    public static CachedSeriesInfo GetSeriesDetails(ISeriesSpecifier ss)
    {
        if (ss.TvMazeId <= 0)
        {
            GetSeriesIdFromOtherCodes(ss);
        }
        JObject results = GetSeriesDetailsWithMazeId(ss);

        CachedSeriesInfo downloadedSi = GenerateSeriesInfo(results);
        JToken jToken = GetChild(results, "_embedded");

        foreach (string name in GetChild(jToken, "akas").Select(akaJson => (string?)akaJson["name"]).OfType<string>())
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
                string? child = (string?)GetChild(imageNode, "original");
                if (child != null)
                {
                    downloadedSi.AddOrUpdateImage(GenerateImage(jsonSeason.GetMandatoryInt("number", TVDoc.ProviderType.TVmaze), child));
                }
            }
        }

        foreach (JToken imageJson in GetChild(jToken, "images"))
        {
            downloadedSi.AddOrUpdateImage(GenerateImage(imageJson));
        }

        downloadedSi.ClearActors();
        foreach (JToken jsonActor in GetChild(jToken, "cast"))
        {
            downloadedSi.AddActor(GenerateActor(jsonActor));
        }

        return downloadedSi;
    }

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
                return (string?)personTokenToken["name"];
            })
            .OfType<string>()
            .ToList();
    }

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
                return (string?)personTokenToken["name"];
            })
            .OfType<string>()
            .ToList();
    }

    private static ShowImage GenerateImage(JToken imageJson)
    {
        ShowImage newBanner = new()
        {
            ImageUrl = (string?)GetChild(GetChild(GetChild(imageJson, "resolutions"), "original"), "url"),
            Id = imageJson.GetMandatoryInt("id", TVDoc.ProviderType.TVmaze),
            ImageStyle = MapImageType((string?)imageJson["type"]),
            Rating = (bool?)imageJson["main"] ?? false ? 10 : 1,
            RatingCount = 1,
            SeriesSource = TVDoc.ProviderType.TVmaze,
        };

        return newBanner;
    }

    private static MediaImage.ImageType MapImageType(string? s)
    {
        return s switch
        {
            null => MediaImage.ImageType.background,
            "background" => MediaImage.ImageType.background,
            "poster" => MediaImage.ImageType.poster,
            "banner" => MediaImage.ImageType.wideBanner,
            _ => MediaImage.ImageType.background
        };
    }

    private static ShowImage GenerateImage(int seasonNumber, string url) =>
        new()
        {
            ImageUrl = url,
            ImageStyle = MediaImage.ImageType.poster,
            Subject = MediaImage.ImageSubject.season,
            SeasonId = seasonNumber,
            Rating = 10,
            RatingCount = 1,
            SeriesSource = TVDoc.ProviderType.TVmaze,
        };

    private static Actor GenerateActor(JToken jsonActor)
    {
        JToken personToken = GetChild(jsonActor, "person");
        JToken actorImageNode = GetChild(personToken, "image");
        int actorId = personToken.GetMandatoryInt("id", TVDoc.ProviderType.TVmaze);
        string? actorImage = actorImageNode.HasValues ? (string?)actorImageNode["medium"] : null;
        string actorName = (string?)personToken["name"] ?? throw new SourceConsistencyException("No Actor Name", TVDoc.ProviderType.TVmaze);
        string? actorRole = (string?)GetChild(GetChild(jsonActor, "character"), "name");
        int? actorSortOrder = (int?)personToken["id"];
        return new Actor(actorId, actorImage, actorName, actorRole, actorSortOrder);
    }

    private static Season GenerateSeason(int seriesId, JToken json)
    {
        int id = json.GetMandatoryInt("id", TVDoc.ProviderType.TVmaze);
        int number = json.GetMandatoryInt("number", TVDoc.ProviderType.TVmaze);
        string? url = (string?)json["url"];
        string? name = (string?)json["name"];
        string? description = (string?)json["summary"];
        JToken imageNode = GetChild(json, "image");
        string? imageUrl = imageNode.HasValues ? (string?)imageNode["original"] : null;
        return new Season(id, number, name, StripPTags(description ?? string.Empty), url, imageUrl, seriesId);
    }

    private static string StripPTags(string description)
    {
        return description.TrimStartString("<p>").TrimEnd("</p>");
    }

    private static CachedSeriesInfo GenerateSeriesInfo(JObject r)
    {
        CachedSeriesInfo returnValue = GenerateCoreSeriesInfo(r);

        if (r.TryGetValue("genres", out JToken? genres))
        {
            returnValue.Genres = genres.Select(x => x.Value<string>()?.Trim()).OfType<string>().Distinct().ToSafeList();
        }

        List<string> typesToTransferToGenres = ["Animation", "Reality", "Documentary", "News", "Sports"];
        foreach (string conversionType in typesToTransferToGenres.Where(s => s == returnValue.SeriesType))
        {
            returnValue.Genres.Add(conversionType);
        }

        string? s1 = (string?)r["name"];
        if (s1 != null)
        {
            returnValue.Name = System.Web.HttpUtility.HtmlDecode(s1).Trim();
        }

        string? siteRatingString = ((string?)GetChild(r, "rating")["average"])?.Trim();
        float.TryParse(siteRatingString, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out float parsedSiteRating);
        returnValue.SiteRating = parsedSiteRating;

        string? siteRatingVotesString = (string?)r["weight"];
        int.TryParse(siteRatingVotesString, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.CreateSpecificCulture("en-US"), out int parsedSiteRatingVotes);
        returnValue.SiteRatingVotes = parsedSiteRatingVotes;

        return returnValue;
    }

    private static CachedSeriesInfo GenerateCoreSeriesInfo(JObject r)
    {
        string? nw = GetKeySubKey(r, "network", "name");
        string? wc = GetKeySubKey(r, "webChannel", "name");
        string? days = GetChild(r, "schedule")["days"]?.Select(x => x.Value<string>()).OfType<string>().ToCsv();
        JToken externalsToken = GetChild(r, "externals");
        int tvdb = GetChild(externalsToken, "thetvdb").Type == JTokenType.Null ? -1 : (int?)externalsToken["thetvdb"] ?? -1;
        int rage = GetChild(externalsToken, "tvrage").Type == JTokenType.Null ? -1 : (int?)externalsToken["tvrage"] ?? -1;

        return new CachedSeriesInfo(new Locale(), TVDoc.ProviderType.TVmaze)
        {
            IsSearchResultOnly = false,
            AirsDay = days,
            AirsTime = JsonHelper.ParseAirTime((string?)GetChild(r, "schedule")["time"]),
            FirstAired = JsonHelper.ParseFirstAired((string?)r["premiered"]),
            TvdbCode = tvdb,
            TvMazeCode = (int)(r["id"] ?? 0),
            TvRageCode = rage,
            Imdb = (string?)externalsToken["imdb"],
            Network = nw ?? wc,
            WebUrl = ((string?)r["url"])?.Trim(),
            PosterUrl = GetUrl(r, "original"),
            OfficialUrl = (string?)r["officialSite"],
            ShowLanguage = (string?)r["language"],
            Overview = System.Web.HttpUtility.HtmlDecode((string?)r["summary"])?.Trim(),
            Runtime = ((string?)r["runtime"])?.Trim(),
            Status = MapStatus((string?)r["status"] ?? throw new SourceConsistencyException("No Status", TVDoc.ProviderType.TVmaze)),
            SeriesType = (string?)r["type"],
            SrvLastUpdated =
                long.TryParse((string?)r["updated"], out long updateTime)
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
        JToken? keyVal = r[key];

        switch (keyVal)
        {
            case null:
                return null;

            case JArray array:
                if (array.First != null)
                {
                    return (string?)array.First[firstSubKey];
                }

                return null;

            case JObject o:
                return (string?)o[firstSubKey];

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

    private static Episode GenerateEpisode(int seriesId, List<string> writers, List<string> directors, JObject r, CachedSeriesInfo si)
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
            FirstAired = ((string?)r["airdate"]).HasValue() ? (DateTime?)r["airdate"] : null,
            AirTime = JsonHelper.ParseAirTime((string?)r["airtime"]),
            AirStamp = airstampToken.HasValues ? (DateTime?)airstampToken : null,
            EpisodeId = r.GetMandatoryInt("id", TVDoc.ProviderType.TheTVDB),
            LinkUrl = ((string?)r["url"])?.Trim(),
            Overview = System.Web.HttpUtility.HtmlDecode((string?)r["summary"])?.Trim(),
            Runtime = ((string?)r["runtime"])?.Trim(),
            Name = ((string?)r["name"])?.Trim() ?? string.Empty,
            AiredEpNum = r.GetMandatoryInt("number", TVDoc.ProviderType.TVmaze),
            SeasonId = r.GetMandatoryInt("season", TVDoc.ProviderType.TVmaze),
            AiredSeasonNumber = r.GetMandatoryInt("season", TVDoc.ProviderType.TVmaze),
            Filename = GetUrl(r, "medium"),
            ReadDvdSeasonNum = 0,
            DvdEpNum = 0
        };

        newEp.SetWriters(writers);
        newEp.SetDirectors(directors);

        return newEp;
    }

    private static JToken GetChild(JToken json, string key)
    {
        return json[key] ?? throw new SourceConsistencyException($"Could not get '{key}' element from {json}", TVDoc.ProviderType.TVmaze);
    }

    private static string? GetUrl(JObject r, string typeKey)
    {
        JToken x = r["image"]?? throw new SourceConsistencyException($"Could not get 'image' element from {r}", TVDoc.ProviderType.TVmaze);

        if (x.HasValues)
        {
            return (string?)x[typeKey];
        }

        return null;
    }
}
