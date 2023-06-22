using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Humanizer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace TVRename.TheTVDB;

// ReSharper disable once InconsistentNaming
public static class API
{
    //V4 Doco: https://app.swaggerhub.com/apis/tvdb/tvdb-api-v4/4.0.1#
    //         https://app.swaggerhub.com/apis-docs/thetvdb/tvdb-api_v_4/4.0.0#

    // ReSharper disable once ConvertToConstant.Local
    private static readonly string WebsiteRoot = "https://thetvdb.com";

    // ReSharper disable once ConvertToConstant.Local
    private static readonly string WebsiteImageRoot = "https://artworks.thetvdb.com";

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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

    public static byte[]? GetTvdbDownload(string url)
    {
        try
        {
            System.Net.Http.HttpClient wc = new();
            return Task.Run(async () => await wc.GetByteArrayAsync(url)).Result;
        }
        catch (Exception e)
        {
            Logger.Warn($"Could not download {url}.", e);
            return null;
        }
    }

    public class UpdateRecord
    {
        internal UpdateRecord(UpdateType t, int id, long time)
        {
            Time = time;
            Id = id;
            Type = t;
        }

        public long Time
        {
            get;
        }

        public int Id
        {
            get;
        }

        public UpdateType Type
        {
            get;
        }

        public enum UpdateType
        {
            series,
            movie,
            season,
            episode
        }
    }

    private static void PersistResponse(JObject response, long updateFromEpochTime, int i)
    {
        //open file stream
        using StreamWriter file = File.CreateText(PathManager.AuditLogFile($"-{updateFromEpochTime}-{i}"));
        JsonSerializer serializer = new();
        //serialize object directly into file stream
        serializer.Serialize(file, response);
    }

    private static bool MoreFrom(JObject jsonUpdateResponse)
    {
        JToken? x = jsonUpdateResponse["links"]?["next"];
        return x is { Type: JTokenType.String };
    }

    private static int? GetNumResponses(JObject jsonUpdateResponse, DateTime requestedTime, bool showConnectionIssues)
    {
        try
        {
            JToken? dataToken = jsonUpdateResponse["data"];
            if (dataToken is null)
            {
                return 0;
            }

            return !dataToken.HasValues ? 0 : ((JArray)dataToken).Count;
        }
        catch (InvalidCastException ex)
        {
            string msg = "Unable to get latest updates from TVDB " + Environment.NewLine +
                         "Trying to get updates since " + requestedTime.ToLocalTime() +
                         Environment.NewLine + Environment.NewLine +
                         "If the date is very old, please consider a full refresh";

            Logger.Warn($"Error obtaining lastupdated query -since(local) {requestedTime.ToLocalTime()}");

            Logger.Warn(ex, msg);

            if (showConnectionIssues && Environment.UserInteractive)
            {
                MessageBox.Show(msg, "Error obtaining updates from TVDB", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            return null;
        }
    }

    public static void ReloadEpisodesV4(ISeriesSpecifier code, Locale locale, CachedSeriesInfo si, ProcessedSeason.SeasonType order)
    {
        Parallel.ForEach(si.Seasons, new ParallelOptions { MaxDegreeOfParallelism = TVSettings.Instance.ParallelDownloads }, s =>
        {
            Thread.CurrentThread.Name ??= $"Download Season {s.SeasonNumber} for {si.Name}"; // Can only set it once
            try
            {
                ReloadEpisode(code, locale, si, order, s);
            }
            catch (SourceConsistencyException sce)
            {
                Logger.Error(sce);
            }
            catch (SourceConnectivityException sce)
            {
                Logger.Warn(sce.Message);
            }
            catch (MediaNotFoundException mnfe)
            {
                Logger.Error($"Season Issue: {mnfe.Message}");
            }
        });
    }

    private static void ReloadEpisode(ISeriesSpecifier code, Locale locale, CachedSeriesInfo si, ProcessedSeason.SeasonType order, Season s)
    {
        JObject seasonInfo = TvdbWebApi.GetSeasonEpisodes(si,
            locale.LanguageToUse(TVDoc.ProviderType.TheTVDB), s.SeasonId);

        JToken? episodeData = seasonInfo["data"]?["episodes"];

        if (episodeData != null)
        {
            Parallel.ForEach(episodeData,
                new ParallelOptions { MaxDegreeOfParallelism = TVSettings.Instance.ParallelDownloads }, x =>
                {
                    int? epNumber = x["number"]?.ToObject<int>();
                    Thread.CurrentThread.Name ??=
                        $"Creating S{s.SeasonNumber}E{epNumber} Episode for {si.Name}"; // Can only set it once

                    GenerateAddEpisodeV4(code, locale, si, x, order);
                });
        }

        JToken? imageData = seasonInfo["data"]?["artwork"];
        if (imageData != null)
        {
            foreach (ShowImage newImage in imageData.Select(API.ConvertJsonToImage))
            {
                newImage.SeasonId = s.SeasonId;
                newImage.SeasonNumber = s.SeasonNumber;
                si.AddOrUpdateImage(newImage);
            }
        }
    }

    internal static CachedMovieInfo DownloadMovieInfo(ISeriesSpecifier code, Locale locale)
    {
        (CachedMovieInfo si, Language? languageCode) = GenerateMovieInfoV4(TvdbWebApi.DownloadMovie(code, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB)), locale);
        if (languageCode != null)
        {
            si.AddTranslations(TvdbWebApi.DownloadMovieTranslations(code, languageCode));
        }

        return si;
    }
    internal static CachedSeriesInfo DownloadSeriesInfo(ISeriesSpecifier code, Locale locale)
    {
        ProcessedSeason.SeasonType st = code is ShowConfiguration showConfig
            ? showConfig.Order
            : ProcessedSeason.SeasonType.aired;

        (CachedSeriesInfo si, Language? languageCodeToUse) = GenerateSeriesInfoV4(TvdbWebApi.DownloadSeries(code, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB)), locale, st);
        if (languageCodeToUse != null)
        {
            si.AddTranslations(TvdbWebApi.DownloadSeriesTranslations(code, languageCodeToUse));
        }

        return si;
    }

    internal static void CheckForNewEpisodes(ISeriesSpecifier code, Locale locale, CachedSeriesInfo si, ProcessedSeason.SeasonType st)
    {
        try
        {
            JObject episodeInfo = TvdbWebApi.GetSeriesEpisodesOfType(si,
                locale.LanguageToUse(TVDoc.ProviderType.TheTVDB), st);

            JToken? episodeData = episodeInfo["data"]?["episodes"];

            if (episodeData == null)
            {
                return;
            }

            IEnumerable<(int? id, JToken jsonData)> availableEpisodes =
                episodeData.Select(x => (x["id"]?.ToObject<int>(), x)).Where(x => x.Item1.HasValue);

            List<(int? id, JToken jsonData)> neededEpisodes =
                availableEpisodes
                    .Where(x => x.id.HasValue && si.Episodes.All(e => e.EpisodeId != x.id))
                    .ToList();

            if (!neededEpisodes.Any())
            {
                return;
            }

            Parallel.ForEach(neededEpisodes,
                new ParallelOptions { MaxDegreeOfParallelism = TVSettings.Instance.ParallelDownloads }, x =>
                {
                    int? epNumber = x.jsonData["number"]?.ToObject<int>();
                    Thread.CurrentThread.Name ??=
                        $"Creating SE{epNumber} Episode for {si.Name}"; // Can only set it once

                    GenerateAddEpisodeV4(code, locale, si, x.jsonData, st);
                });
        }
        catch (SourceConsistencyException sce)
        {
            Logger.Error(sce);
        }
        catch (SourceConnectivityException sce)
        {
            Logger.Warn(sce.Message);
        }
        catch (MediaNotFoundException mnfe)
        {
            Logger.Error($"Season Issue: {mnfe.Message}");
        }
    }

    private static void GenerateAddEpisodeV4(ISeriesSpecifier code, Locale locale, CachedSeriesInfo si, JToken x, ProcessedSeason.SeasonType order)
    {
        try
        {
            (Episode newEp, Language? bestLanguage) = API.GenerateCoreEpisodeV4(x, code.TvdbId, si, locale, order);
            if (bestLanguage != null)
            {
                newEp.AddTranslations(TvdbWebApi.GetEpisodeTranslations(code, bestLanguage, newEp.EpisodeId));
            }

            si.AddEpisode(newEp);
        }
        catch (MediaNotFoundException mnfe)
        {
            Logger.Error($"Episode (+ Translations) claimed to exist, but got a 404 when searching for them. Ignoring Episode, but might be worth a full refresh of the show and contacting TVDB if it does not get resolved. {mnfe.Message}");
            si.Dirty = true;
        }
        catch (SourceConnectivityException sce1)
        {
            Logger.Warn(sce1.Message);
            si.Dirty = true;
        }
        catch (SourceConsistencyException sce1)
        {
            Logger.Error(sce1);
            si.Dirty = true;
        }
    }

    private static long GetUpdateTime(JObject jsonUpdateResponse)
    {
        try
        {
            const string KEY_NAME = "timeStamp";
            IEnumerable<long>? updateTimes = jsonUpdateResponse["data"]?.Select(a => a.GetMandatoryLong(KEY_NAME, TVDoc.ProviderType.TheTVDB));
            long maxUpdateTime = updateTimes?.DefaultIfEmpty(0).Max() ?? 0;

            long nowTime = TimeHelpers.UnixUtcNow();
            if (maxUpdateTime > nowTime)
            {
                int buffer = 10.Seconds().Seconds;
                string message = $"Assuming up to date: Update time from TVDB API is greater than current time for {maxUpdateTime} > {nowTime} ({maxUpdateTime.FromUnixTime().ToLocalTime()} > {nowTime.FromUnixTime().ToLocalTime()})";
                if (maxUpdateTime > nowTime + buffer)
                {
                    Logger.Error(message);
                }
                else
                {
                    Logger.Warn(message);
                }

                return nowTime;
            }

            return maxUpdateTime;
        }
        catch (Exception e)
        {
            Logger.Error(e, jsonUpdateResponse.ToString());
            return 0;
        }
    }
    internal static TvdbUpdateResponse GetUpdatesV4(long updateFromEpochTime,bool showConnectionIssues, CancellationToken cts)
    {
        //We need to ask for a number of pages
        //We'll keep asking until we get to a page until there is no next pages

        bool moreUpdates = true;
        int pageNumber = 0;
        const int MAX_NUMBER_OF_CALLS = 1000;
        const int OFFSET = 0;
        bool auditUpdates = Helpers.InDebug();
        long fromEpochTime = updateFromEpochTime - OFFSET;
        List<JObject> updatesResponses = new();
        TvdbUpdateResponse result = new();

        while (moreUpdates)
        {
            if (cts.IsCancellationRequested)
            {
                throw new UpdateCancelledException();
            }

            //TODO - get these in parallel;
            JObject jsonUpdateResponse = TvdbWebApi.GetUpdates(fromEpochTime, pageNumber)
                                         ?? throw new SourceConsistencyException("Could not get updates from TVDB", TVDoc.ProviderType.TheTVDB);

            int numberOfResponses = GetNumResponses(jsonUpdateResponse, fromEpochTime.GetRequestedTime(),showConnectionIssues) ?? throw new SourceConsistencyException($"NumberOfResponses is null: {fromEpochTime}:{pageNumber}:{jsonUpdateResponse}", TVDoc.ProviderType.TheTVDB);

            updatesResponses.Add(jsonUpdateResponse);
            pageNumber++;
            long maxTime = GetUpdateTime(jsonUpdateResponse);
            Logger.Info($"Obtained {numberOfResponses} responses from lastupdated query({fromEpochTime.FromUnixTime().ToLocalTime()}) #{pageNumber} - until {maxTime.FromUnixTime().ToLocalTime()} ({maxTime})");

            if (!MoreFrom(jsonUpdateResponse))
            {
                moreUpdates = false;
            }

            //As a safety measure we check that no more than MAX_NUMBER_OF_CALLS calls are made
            if (pageNumber > MAX_NUMBER_OF_CALLS)
            {
                throw new TooManyCallsException($"We have got {pageNumber} pages of updates and we are not up to date.");
            }
        }

        result.LatestTime = updatesResponses.Max(API.GetUpdateTime);

        Parallel.ForEach(updatesResponses, new ParallelOptions { MaxDegreeOfParallelism = TVSettings.Instance.ParallelDownloads }, o =>
        {
            Thread.CurrentThread.Name ??= "Recent Updates"; // Can only set it once
            result.AddRange(ProcessUpdate(o));
        });

        if (auditUpdates && updatesResponses.Any())
        {
            int n = 0;
            foreach (JObject response in updatesResponses)
            {
                PersistResponse(response, updateFromEpochTime, n++);
            }
        }

        return result;
    }

    private static IEnumerable<UpdateRecord> ProcessUpdate(JObject jsonResponse)
    {
        // if updatetime > localtime for item, then remove it, so it will be downloaded later
        JToken jToken = jsonResponse["data"] ?? throw new SourceConsistencyException($"Could not get data element from {jsonResponse}", TVDoc.ProviderType.TheTVDB);

        try
        {
            return jToken.Cast<JObject>()
                .Select(MapUpdate)
                .OfType<UpdateRecord>();
        }
        catch (InvalidCastException ex)
        {
            Logger.Error("Did not receive the expected format of json from lastupdated query.");
            Logger.Error(ex);
            Logger.Error(jToken.ToString());
        }
        catch (OverflowException ex)
        {
            Logger.Error("Could not parse the json from lastupdated query.");
            Logger.Error(ex);
            Logger.Error(jToken.ToString());
        }

        return new List<UpdateRecord>();
    }

    private static UpdateRecord? MapUpdate(JObject seriesResponse)
    {
        int id = seriesResponse.GetMandatoryInt("recordId", TVDoc.ProviderType.TheTVDB);
        long time = seriesResponse.GetMandatoryLong("timeStamp", TVDoc.ProviderType.TheTVDB);
        string? entityType = (string?)seriesResponse["entityType"];

        switch (entityType)
        {
            case "series":
            case "translatedseries":
            case "seriespeople":
            {
                return new UpdateRecord(UpdateRecord.UpdateType.series, id, time);
                }
            case "movies":
            case "translatedmovies":
            case "movie-genres":
                {
                    return new UpdateRecord(UpdateRecord.UpdateType.movie , id, time);
                }
            case "episodes":
            case "translatedepisodes":
                {
                    return new UpdateRecord(UpdateRecord.UpdateType.episode , id, time);
                }
            case "seasons":
            case "translatedseasons":
                {
                    return new UpdateRecord(UpdateRecord.UpdateType.season, id, time);
                }
            case "artwork":
            case "artworktypes":
            case "people":
            case "characters":
            case "award-nominees":
            case "award_categories":
            case "companies":
            case "awards":
            case "company_types":
            case "movie_status":
            case "content_ratings":
            case "countries":
            case "entity_types":
            case "genres":
            case "languages":
            case "peopletypes":
            case "seasontypes":
            case "sourcetypes":
            case "translatedpeople":
            case "translatedcharacters":
            case "lists":
            case "translatedlists":
            case "translatedcompanies":
            case "tags":
            case "tag-options":
            case "award-categories":

                return null;
        }
        return null;
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

    private static void AddTranslations(this Episode newEp, JObject downloadSeriesTranslationsJsonV4)
    {
        string? transName = downloadSeriesTranslationsJsonV4["data"]?["name"]?.ToString();
        newEp.Name = Translate(newEp.Name, transName);

        string? transOverview = downloadSeriesTranslationsJsonV4["data"]?["overview"]?.ToString();
        newEp.Overview = Translate(newEp.Overview, transOverview);
        //Set a language code on the SI?? si.lan ==downloadSeriesTranslationsJsonV4["data"]["language"].ToString();
    }

    private static long GetUpdateTicks(string? lastUpdateString)
    {
        const long DEFLT = 0; //equates to  1970/1/1

        try
        {
            if (!lastUpdateString.HasValue())
            {
                return DEFLT;
            }
            //"lastUpdated": "2021-06-11 12:42:28",
            DateTime rawDateTime = DateTime.ParseExact(lastUpdateString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            return DateTime.SpecifyKind(rawDateTime, DateTimeKind.Utc).ToUnixTime();
        }
        catch
        {
            Logger.Error($"Failed to parse Epsiode update date {lastUpdateString}");
            return DEFLT;
        }
    }

    private static DateTime? GetEpisodeAiredDate(JToken episodeJson)
    {
        string? date = episodeJson["aired"]?.ToString();
        try
        {
            if (!date.HasValue())
            {
                return null;
            }

            return DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        catch
        {
            return null;
        }
    }

    private static (Episode, Language?) GenerateCoreEpisodeV4(JToken episodeJson, int code, CachedSeriesInfo si, Locale locale, ProcessedSeason.SeasonType order)
    {
        Episode x = new(code, si)
        {
            EpisodeId = episodeJson.GetMandatoryInt("id", TVDoc.ProviderType.TheTVDB),
            SeriesId = (int?)episodeJson["seriesId"] ?? -1,
            Name = (string?)episodeJson["name"] ?? string.Empty,
            FirstAired = GetEpisodeAiredDate(episodeJson),
            Runtime = (string?)episodeJson["runtime"],
            Filename = (string?)episodeJson["image"],
            SrvLastUpdated = GetUpdateTicks((string?)episodeJson["lastUpdated"])
        };

        if (order == ProcessedSeason.SeasonType.dvd)
        {
            x.ReadDvdSeasonNum = episodeJson["seasonNumber"]?.ToObject<int?>() ?? 0;
            x.DvdEpNum = episodeJson["number"]?.ToObject<int?>() ?? 0;
        }
        else
        {
            x.AiredSeasonNumber = episodeJson["seasonNumber"]?.ToObject<int?>() ?? 0;
            x.AiredEpNum = episodeJson["number"]?.ToObject<int?>() ?? 0;
        }

        return (x, GetAppropriateLanguage(episodeJson["nameTranslations"], locale));
    }
    private static string Translate(string? originalName, string? transName)
    {
        //https://github.com/thetvdb/v4-api/issues/30

        if (transName.HasValue() && !transName.IsPlaceholderName())
        {
            return transName;
        }

        if (originalName.HasValue() && !originalName.IsPlaceholderName())
        {
            return originalName;
        }

        if (transName.HasValue() && transName.IsPlaceholderName() && originalName.HasValue() && !originalName.IsPlaceholderName())
        {
            //issue
            return originalName;
        }

        return transName ?? originalName ?? string.Empty;
    }

    public static string WebsiteMovieUrl(string? serSlug)
    {
        return $"https://www.thetvdb.com/movies/{serSlug}";
    }

    private static void AddTranslations(this CachedSeriesInfo si, JObject downloadSeriesTranslationsJsonV4)
    {
        si.Name = downloadSeriesTranslationsJsonV4["data"]?["name"]?.ToString() ?? si.Name;
        si.Overview = downloadSeriesTranslationsJsonV4["data"]?["overview"]?.ToString() ?? si.Overview;
        //Set a language code on the SI?? si.lan ==downloadSeriesTranslationsJsonV4["data"]["language"].ToString();
        IEnumerable<string>? aliases = downloadSeriesTranslationsJsonV4["data"]?["aliases"]?.Select(x => x.ToString());
        if (aliases == null)
        {
            return;
        }

        foreach (string alias in aliases)
        {
            si.AddAlias(alias);
        }
    }

    private static void AddTranslations(this CachedMovieInfo si, JObject downloadSeriesTranslationsJsonV4)
    {
        si.Name = downloadSeriesTranslationsJsonV4["data"]?["name"]?.ToString() ?? si.Name;
        si.Overview = downloadSeriesTranslationsJsonV4["data"]?["overview"]?.ToString() ?? si.Overview;

        IEnumerable<string>? aliases = downloadSeriesTranslationsJsonV4["data"]?["aliases"]?.Select(x => x.ToString());
        if (aliases == null)
        {
            return;
        }

        foreach (string alias in aliases)
        {
            si.AddAlias(alias);
        }
    }

    private static int ParseIdFromObjectId(JToken? jToken)
    {
        string? baseValue = jToken?.ToString();
        string[]? splitString = baseValue?.Split('-');
        if (splitString?.Length == 2)
        {
            int? i = splitString[1].ToInt();
            if (i.HasValue)
            {
                return i.Value;
            }
        }

        return 0;
    }

    private static CachedSeriesInfo GenerateSeriesV4(JObject r, Locale locale, bool searchResult, ProcessedSeason.SeasonType st)
    {
        CachedSeriesInfo si = new(locale, TVDoc.ProviderType.TheTVDB)
        {
            TvdbCode = ParseIdFromObjectId(r["objectID"]),
            Slug = ((string?)r["slug"])?.Trim(),
            PosterUrl = (string?)r["image_url"],
            Network = (string?)r["network"],
            Status = (string?)r["status"],
            IsSearchResultOnly = searchResult,
            SeasonOrderType = st,
            ShowLanguage = (string?)r["primary_language"],
            Country = (string?)r["country"],
            FirstAired = ParseDate((string?)r["first_air_time"]) ?? GenerateFirstAiredDate(r),
            Name = FindTranslation(r, locale, "translations") ?? Decode(r, "name") ?? Decode(r, "extended_title") ?? string.Empty,
            Overview = FindTranslation(r, locale, "overviews") ?? Decode(r, "overview") ?? string.Empty,

            Imdb = GetExternalIdSearchResultV4(r, "IMDB"),
            OfficialUrl = GetExternalIdSearchResultV4(r, "Official Website"),
            FacebookId = GetExternalIdSearchResultV4(r, "Facebook"),
            InstagramId = GetExternalIdSearchResultV4(r, "Instagram"),
            TwitterId = GetExternalIdSearchResultV4(r, "Twitter"),
            TmdbCode = GetExternalIdSearchResultV4(r, "TheMovieDB.com")?.ToInt() ?? -1,
            SeriesId = GetExternalIdSearchResultV4(r, "TV.com"),
        };
        AddAliases(r, si); //todo check whether this needs to be V4 version?

        ValidateNewMedia(r, searchResult, si, "Series");

        return si;
    }

    private static CachedMovieInfo GenerateMovieV4(JObject r, Locale locale, bool searchResult)
    {
        CachedMovieInfo si = new(locale, TVDoc.ProviderType.TheTVDB)
        {
            TvdbCode = ParseIdFromObjectId(r["objectID"]),
            Slug = ((string?)r["slug"])?.Trim(),
            PosterUrl = (string?)r["image_url"],
            Status = (string?)r["status"],
            IsSearchResultOnly = searchResult,
            ShowLanguage = (string?)r["primary_language"],
            FirstAired = ParseDate((string?)r["first_air_time"]) ?? GenerateFirstAiredDate(r),
            Name = FindTranslation(r, locale, "translations") ?? Decode(r, "name") ?? Decode(r, "extended_title") ?? string.Empty,
            Overview = FindTranslation(r, locale, "overviews") ?? Decode(r, "overview") ?? string.Empty,
            Country = (string?)r["country"],
            Imdb = GetExternalIdSearchResultV4(r, "IMDB"),
            OfficialUrl = GetExternalIdSearchResultV4(r, "Official Website"),
            FacebookId = GetExternalIdSearchResultV4(r, "Facebook"),
            InstagramId = GetExternalIdSearchResultV4(r, "Instagram"),
            TwitterId = GetExternalIdSearchResultV4(r, "Twitter"),
            TmdbCode = GetExternalIdSearchResultV4(r, "TheMovieDB.com")?.ToInt() ?? -1,
            Genres = r["genres"]?.ToObject<string[]>()?.ToSafeList() ?? new(),
            Network = r["studios"]?.ToObject<string[]>()?.ToPsv() ?? string.Empty,
        };

        AddDirector(r, si);
        AddAliases(r, si);

        ValidateNewMedia(r, searchResult, si, "Movie");

        return si;
    }

    private static IEnumerable<CachedSeriesInfo> GetEnumSeriesV4(JToken jToken, Locale locale, bool b)
    {
        JArray ja = (JArray)jToken;
        List<CachedSeriesInfo> ses = new();

        foreach (JToken jt in ja.Children())
        {
            JObject showJson = (JObject)jt;
            if (jt["type"]?.ToString() == "movie")
            {
                continue;
            }
            ses.Add(GenerateSeriesV4(showJson, locale, b, ProcessedSeason.SeasonType.aired)); //Assume Aired for Search Results
        }

        return ses;
    }

    private static IEnumerable<CachedMovieInfo> GetEnumMoviesV4(JToken jToken, Locale locale, bool b)
    {
        JArray ja = (JArray)jToken;
        List<CachedMovieInfo> ses = new();

        foreach (JToken jt in ja.Children())
        {
            JObject showJson = (JObject)jt;
            if (jt["type"]?.ToString() == "movie")
            {
                ses.Add(GenerateMovieV4(showJson, locale, b));
            }
        }

        return ses;
    }

    private static void ValidateNewMedia(JObject r, bool searchResult, CachedMediaInfo si, string type)
    {
        if (string.IsNullOrEmpty(si.Name))
        {
            Logger.Warn($"Issue with {type} {si}");
            Logger.Warn(r.ToString());
        }

        if (si.TvdbCode == 0)
        {
            Logger.Error($"Issue with {type} (No Id) {si}");
            Logger.Error(r.ToString());
        }

        if (si.SrvLastUpdated == 0 && !searchResult)
        {
            Logger.Warn($"Issue with {type} (update time is 0) {si}");
            Logger.Warn(r.ToString());
            si.SrvLastUpdated = 100;
        }
    }

    private static void AddAliases(JObject r, CachedMediaInfo si)
    {
        JToken? al = r["aliases"];
        if (al != null)
        {
            foreach (JValue a in ((JArray)al).Cast<JValue>())
            {
                si.AddAlias(a.ToObject<string>());
            }
        }
    }

    private static void AddDirector(JObject r, CachedMovieInfo si)
    {
        string? directorName = (string?)r["director"];
        if (directorName.HasValue())
        {
            si.AddCrew(new Crew(1, null, directorName, "Director", "Directing", null));
        }
    }

    private static string? FindTranslation(JObject r, Locale locale, string tag)
    {
        JToken? languagesArray = r[tag];
        if (languagesArray == null || !languagesArray.HasValues || languagesArray is not JObject)
        {
            return null;
        }

        string languageCode = locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).TVDBV4Code();
        JToken? languageValue = languagesArray[languageCode];
        if (languageValue is not { Type: JTokenType.String })
        {
            return null;
        }

        return (string?)languageValue;
    }

    private static string? Decode(JObject r, string tag)
    {
        string? s = (string?)r[tag];
        return s.HasValue() ? System.Web.HttpUtility.HtmlDecode(s).Trim() : null;
    }

    private static DateTime? GenerateFirstAiredDate(JObject r)
    {
        int? year = r.Value<int?>("year");
        if (year > 0)
        {
            try
            {
                return new DateTime(year.Value, 1, 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                Logger.Error($"Could not parse TVDB Series year from {r}");
            }
        }

        return null;
    }

    private static string? GetArtworkV4(JObject json, int type)
    {
        return json["data"]?["artworks"]
            ?.OrderByDescending(x => x["score"]?.ToObject<int>())
            .FirstOrDefault(x => (int?)x["type"] == type)
            ?["image"]
            ?.ToString();
    }

    private static string? GetExternalIdV4(JObject json, string source)
    {
        return json["data"]?["remoteIds"]?.FirstOrDefault(x => x["sourceName"]?.ToString() == source)?["id"]
            ?.ToString();
    }

    private static string? GetExternalIdSearchResultV4(JObject json, string source)
    {
        return json["remote_ids"]?.FirstOrDefault(x => x["sourceName"]?.ToString() == source)?["id"]
            ?.ToString();
    }

    private static string? GetContentRatingV4(JObject json, string country)
    {
        return json["data"]?["contentRatings"]?.FirstOrDefault(x => x["country"]?.ToString() == country)?["name"]
            ?.ToString();
    }

    private static DateTime? ParseDate(string? date)
    {
        try
        {
            if (!date.HasValue())
            {
                return null;
            }

            return DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        catch
        {
            return null;
        }
    }

    private static DateTime? GetFirstReleaseDateV4(JObject json)
    {
        List<string?>? dates = json["data"]?["releases"]?.Select(x => x["date"]?.ToString()).ToList();

        try
        {
            if (!dates.HasAny())
            {
                return null;
            }

            return dates.Where(x => x.HasValue()).MinOrNull(x => DateTime.ParseExact(x!, "yyyy-MM-dd", CultureInfo.InvariantCulture));
        }
        catch
        {
            return null;
        }
    }

    private static DateTime? GetReleaseDateV4(JObject json, string? region)
    {
        if (region is null)
        {
            return GetFirstReleaseDateV4(json);
        }

        string? date = json["data"]?["releases"]?.FirstOrDefault(x => x["country"]?.ToString() == region)?["date"]?.ToString();

        try
        {
            if (!date.HasValue())
            {
                return null;
            }

            return DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        catch
        {
            return null;
        }
    }

    private static (CachedMovieInfo, Language?) GenerateMovieInfoV4(JObject r, Locale locale)
    {
        CachedMovieInfo si = GenerateCoreMovieInfoV4(r, locale);
        AddAliasesV4(r, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB), si);
        AddCastAndCrew(r, si);
        AddMovieImagesV4(r, si);

        return (si, GetAppropriateLanguage(r["data"]?["nameTranslations"], locale));
    }
    private static void AddMovieImagesV4(JObject r, CachedMovieInfo si)
    {
        JToken? jToken = r["data"]?["artworks"];
        if (jToken is not null)
        {
            foreach (MovieImage mi in jToken
                         .Where(imageJson => !IgnoreableImageJson(imageJson))
                         .Select(imageJson => GenerateMovieImage(si, imageJson)))
            {
                si.AddOrUpdateImage(mi);
            }
        }
    }

    private static MovieImage GenerateMovieImage(CachedMovieInfo si, JToken imageJson) =>
        new()
        {
            Id = imageJson.GetMandatoryInt("id", TVDoc.ProviderType.TheTVDB),
            ImageUrl = GetImageURL((string?)imageJson["image"]),
            ThumbnailUrl = GetImageURL((string?)imageJson["thumbnail"]),
            LanguageCode = (string?)imageJson["language"],
            Rating = imageJson.GetMandatoryInt("score", TVDoc.ProviderType.TheTVDB),
            MovieId = si.TvdbCode,
            ImageStyle = MapBannerTvdbV4ApiCode(imageJson.GetMandatoryInt("type", TVDoc.ProviderType.TheTVDB)),
            MovieSource = TVDoc.ProviderType.TheTVDB,
            RatingCount = 1
        };

    private static bool IgnoreableImageJson(JToken imageJson) => imageJson.GetMandatoryInt("type", TVDoc.ProviderType.TheTVDB) == 13; //Person Snapshot

    private static void AddShowImagesV4(JObject r, CachedSeriesInfo si)
    {
        //JObject x = API.ImageTypesV4();
        if (r["data"]?["artworks"] is null)
        {
            return;
        }

        foreach (ShowImage mi in r["data"]?["artworks"]!.Select(ConvertJsonToImage)!)
        {
            si.AddOrUpdateImage(mi);
        }
    }

    private static ShowImage ConvertJsonToImage(JToken imageJson)
    {
        int imageCodeType = imageJson.GetMandatoryInt("type", TVDoc.ProviderType.TheTVDB);

        return new ShowImage
        {
            Id = imageJson.GetMandatoryInt("id", TVDoc.ProviderType.TheTVDB),
            ImageUrl = GetImageURL((string?)imageJson["image"]),
            ThumbnailUrl = GetImageURL((string?)imageJson["thumbnail"]),
            LanguageCode = (string?)imageJson["language"],
            Rating = imageJson.GetMandatoryInt("score", TVDoc.ProviderType.TheTVDB),
            ImageStyle = MapBannerTvdbV4ApiCode(imageCodeType),
            Subject = MapSubjectTvdbv4ApiCode(imageCodeType),
            SeriesSource = TVDoc.ProviderType.TheTVDB,
            RatingCount = 1
        };
    }

    private static MediaImage.ImageType MapBannerTvdbV4ApiCode(int v)
    {
        // from call to API.ImageTypesV4()
        return v switch
        {
            14 => MediaImage.ImageType.poster,
            2 => MediaImage.ImageType.poster,
            7 => MediaImage.ImageType.poster,

            1 => MediaImage.ImageType.wideBanner,
            6 => MediaImage.ImageType.wideBanner,
            16 => MediaImage.ImageType.wideBanner,

            15 => MediaImage.ImageType.background,
            3 => MediaImage.ImageType.background,
            8 => MediaImage.ImageType.background,

            5 => MediaImage.ImageType.icon,
            10 => MediaImage.ImageType.icon,
            18 => MediaImage.ImageType.icon,
            19 => MediaImage.ImageType.icon,

            11 => MediaImage.ImageType.thumbs,
            12 => MediaImage.ImageType.thumbs,

            25 => MediaImage.ImageType.clearLogo,
            24 => MediaImage.ImageType.clearArt,
            22 => MediaImage.ImageType.clearArt,
            23 => MediaImage.ImageType.clearLogo,

            _ => MediaImage.ImageType.poster
        };
    }

    private static MediaImage.ImageSubject MapSubjectTvdbv4ApiCode(int v)
    {
        // from call to API.ImageTypesV4()
        return v switch
        {
            1 => MediaImage.ImageSubject.show,
            2 => MediaImage.ImageSubject.show,
            3 => MediaImage.ImageSubject.show,
            5 => MediaImage.ImageSubject.show,
            20 => MediaImage.ImageSubject.show,
            22 => MediaImage.ImageSubject.show,
            23 => MediaImage.ImageSubject.show,

            6 => MediaImage.ImageSubject.season,
            7 => MediaImage.ImageSubject.season,
            8 => MediaImage.ImageSubject.season,
            10 => MediaImage.ImageSubject.season,

            11 => MediaImage.ImageSubject.episode,
            12 => MediaImage.ImageSubject.episode,

            14 => MediaImage.ImageSubject.movie,
            15 => MediaImage.ImageSubject.movie,
            16 => MediaImage.ImageSubject.movie,
            18 => MediaImage.ImageSubject.movie,
            21 => MediaImage.ImageSubject.movie,
            24 => MediaImage.ImageSubject.movie,
            25 => MediaImage.ImageSubject.movie,

            _ => MediaImage.ImageSubject.show
        };
    }

    private static CachedMovieInfo GenerateCoreMovieInfoV4(JObject r, Locale locale)
    {
        JToken dataNode = r["data"] ?? throw new SourceConsistencyException($"Data element not found in {r}", TVDoc.ProviderType.TheTVDB);
        JToken? collectionNode = GetCollectionNodeV4(dataNode);
        DateTime? firstAired = TVSettings.Instance.UseGlobalReleaseDate
        ? GetFirstReleaseDateV4(r)
          ?? GetReleaseDateV4(r, "global")
        : GetReleaseDateV4(r, locale);

        return new CachedMovieInfo(locale, TVDoc.ProviderType.TheTVDB)
        {
            FirstAired = firstAired,
            TvdbCode = dataNode.GetMandatoryInt("id", TVDoc.ProviderType.TheTVDB),
            Slug = ((string?)dataNode["slug"])?.Trim(),
            Imdb = GetExternalIdV4(r, "IMDB"),
            Runtime = ((string?)dataNode["runtime"])?.Trim(),
            Name = dataNode["name"]?.ToString() ?? string.Empty,
            TrailerUrl = GetTrailerUrl(r, locale),
            IsSearchResultOnly = false,
            PosterUrl = GetImageURL(GetArtworkV4(r, 14)),
            FanartUrl = GetImageURL(GetArtworkV4(r, 15)),
            OfficialUrl = GetExternalIdV4(r, "Official Website"),
            FacebookId = GetExternalIdV4(r, "Facebook"),
            InstagramId = GetExternalIdV4(r, "Instagram"),
            TwitterId = GetExternalIdV4(r, "Twitter"),
            Dirty = false,
            Network = dataNode["studios"]?.Select(x => x["name"]?.ToString()).OfType<string>().ToPsv(),
            ShowLanguage = dataNode["audioLanguages"]?.ToString(),
            Country = dataNode["originalCountry"]?.ToString(),
            ContentRating = GetContentRatingV4(r, locale),
            Status = dataNode["status"]?["name"]?.ToString(),
            SrvLastUpdated = GetUnixTime(dataNode, "lastUpdated"),
            Genres = GetGenresV4(r),

            CollectionId = collectionNode?["id"]?.ToString().ToInt(),
            CollectionName = collectionNode?["name"]?.ToString(),
        };
    }
    private static long GetUnixTime(JToken dataNode, string key)
    {
        JToken updated = dataNode[key] ?? throw new SourceConsistencyException($"Data element {key} not found in {dataNode}", TVDoc.ProviderType.TheTVDB);
        DateTime dt = DateTime.SpecifyKind((DateTime)updated, DateTimeKind.Utc);
        return dt.ToUnixTime();
    }

    private static JToken? GetCollectionNodeV4(JToken? r) =>
        r?["lists"]?.FirstOrDefault(x => (bool?)x["isOfficial"] is true);

    private static DateTime? GetReleaseDateV4(JObject r, Locale locale) =>
        GetReleaseDateV4(r, locale.RegionToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation)
        ?? GetReleaseDateV4(r, "global")
        ?? GetReleaseDateV4(r, (string?)null);

    private static string? GetContentRatingV4(JObject r, Locale locale) =>
        GetContentRatingV4(r, locale.RegionToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation)
        ?? GetContentRatingV4(r, Regions.Instance.FallbackRegion.ThreeAbbreviation)
        ?? GetContentRatingV4(r, "usa");

    private static string? GetTrailerUrl(JObject r, Locale locale)
    {
        JToken? trailersNode = r["data"]?["trailers"];
        return
            TrailerUrl(trailersNode, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB)) ??
            TrailerUrl(trailersNode, TVSettings.Instance.PreferredTVDBLanguage) ??
            TrailerUrl(trailersNode, Languages.Instance.FallbackLanguage) ??
            trailersNode?.FirstOrDefault()?["url"]?.ToString();
    }

    private static string? TrailerUrl(JToken? trailersNode, Language language)
    {
        return trailersNode
            ?.FirstOrDefault(x =>
                x["language"]?.ToString() == language.TVDBV4Code())
            ?["url"]
            ?.ToString();
    }

    // ReSharper disable once InconsistentNaming
    internal static string TVDBV4Code(this Language l) =>
        l.ISODialectAbbreviation == "pt-BR" ? "pt" :
        l.ISODialectAbbreviation == "zh-TW" ? "zhtw" :
        l.ISODialectAbbreviation == "zh-YU" ? "yue" :
        l.ThreeAbbreviation;

    private static void AddAliasesV4(JObject r, Language lang, CachedMediaInfo si)
    {
        JToken? aliasNode = r["data"]?["aliases"];
        if (aliasNode is null || !aliasNode.HasValues)
        {
            return;
        }

        List<JToken> languageNodes = aliasNode.Where(x => x["language"]?.ToString() == lang.TVDBV4Code()).ToList();
        if (languageNodes.Any())
        {
            foreach (JToken? x in languageNodes)
            {
                si.AddAlias(x["name"]?.ToString());
            }
            return;
        }

        languageNodes = aliasNode.Where(x => x["language"]?.ToString() == TVSettings.Instance.PreferredTVDBLanguage.TVDBV4Code()).ToList();
        if (languageNodes.Any())
        {
            foreach (JToken? x in languageNodes)
            {
                si.AddAlias(x["name"]?.ToString());
            }
            // ReSharper disable once RedundantJumpStatement
            return;
        }
    }

    private static void AddCastAndCrew(JObject r, CachedMovieInfo si)
    {
        JToken? chactersToken = r["data"]?["characters"];
        if (chactersToken is not null)
        {
            foreach (JToken? actorJson in chactersToken.Where(x => x["peopleType"]?.ToString() == "Actor"))
            {
                int id = int.Parse(actorJson["id"]?.ToString() ?? "0");
                string name = actorJson["personName"]?.ToString() ?? string.Empty;
                string image = GetImageURL(actorJson["image"]?.ToObject<string?>());
                string? role = actorJson["name"]?.ToString();
                int? sort = actorJson["sort"]?.ToString().ToInt();
                si.AddActor(new Actor(id, image, name, role, sort));
            }

            foreach (JToken? actorJson in chactersToken.Where(x => x["peopleType"]?.ToString() != "Actor"))
            {
                int id = int.Parse(actorJson["id"]?.ToString() ?? "0");
                string name = actorJson["personName"]?.ToString() ?? string.Empty;
                string? role = actorJson["peopleType"]?.ToString();
                string? sort = actorJson["sort"]?.ToString();
                si.AddCrew(new Crew(id, string.Empty, name, role, string.Empty, sort));
            }
        }
    }

    private static (CachedSeriesInfo, Language?) GenerateSeriesInfoV4(JObject r, Locale locale,
        ProcessedSeason.SeasonType seasonType)
    {
        CachedSeriesInfo si = GenerateCoreSeriesInfoV4(r, locale, seasonType);

        AddAliasesV4(r, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB), si);
        AddCastAndCrewV4(r, si);
        AddShowImagesV4(r, si);
        AddSeasonsV4(r, seasonType, si);

        return (si, GetAppropriateLanguage(r["data"]?["nameTranslations"], locale));
    }

    private static CachedSeriesInfo GenerateCoreSeriesInfoV4(JObject r, Locale locale, ProcessedSeason.SeasonType st)
    {
        JToken jToken = r["data"] ?? throw new SourceConsistencyException($"Data element not found in {r}", TVDoc.ProviderType.TheTVDB);
        Logger.Info($"Update obtained for {GetName(r)} at {GetUnixTime(jToken, "lastUpdated")} based on {jToken["lastUpdated"]}");

        return new CachedSeriesInfo(locale, TVDoc.ProviderType.TheTVDB)
        {
            Name = GetName(r),
            AirsTime = GetAirsTimeV4(r),
            TvdbCode = jToken.GetMandatoryInt("id", TVDoc.ProviderType.TheTVDB),
            IsSearchResultOnly = false,
            Dirty = false,
            Slug = ((string?)jToken["slug"])?.Trim(),
            Genres = GetGenresV4(r),
            ShowLanguage = jToken["originalLanguage"]?.ToString(),
            Country = jToken["originalCountry"]?.ToString(),
            TrailerUrl = GetTrailerUrl(r, locale),
            SrvLastUpdated = GetUnixTime(jToken, "lastUpdated"),
            Status = (string?)jToken["status"]?["name"],
            FirstAired = JsonHelper.ParseFirstAired((string?)jToken["firstAired"]),
            AirsDay = GetAirsDayV4(r),
            Network = GetNetworks(r),
            Imdb = GetExternalIdV4(r, "IMDB"),
            OfficialUrl = GetExternalIdV4(r, "Official Website"),
            FacebookId = GetExternalIdV4(r, "Facebook"),
            InstagramId = GetExternalIdV4(r, "Instagram"),
            TwitterId = GetExternalIdV4(r, "Twitter"),
            TmdbCode = GetExternalIdV4(r, "TheMovieDB.com")?.ToInt() ?? -1,
            SeriesId = GetExternalIdV4(r, "TV.com"),
            PosterUrl = GetArtworkV4(r, 2),
            FanartUrl = GetArtworkV4(r, 3),
            SeasonOrderType = st,
        };
    }

    private static string GetName(JObject r) => System.Web.HttpUtility.HtmlDecode((string?)r["data"]?["name"] ?? string.Empty).Trim();

    private static string? GetNetworks(JObject r)
    {
        JToken? jToken = r["data"]?["companies"];
        return jToken
            ?.Where(x => x["companyType"]?["companyTypeName"]?.ToString().Equals("Network", StringComparison.OrdinalIgnoreCase) ?? false)
            .Select(x => x["name"]?.ToString())
            .OfType<string>()
            .ToPsv();
    }
    private static SafeList<string> GetGenresV4(JObject r)
    {
        return r["data"]?["genres"]?.Select(x => x["name"]?.ToString()).OfType<string>().ToSafeList() ?? new SafeList<string>();
    }

    private static void AddCastAndCrewV4(JObject r, CachedSeriesInfo si)
    {
        JToken? characterNode = r["data"]?["characters"];
        if (characterNode is not null)
        {
            foreach (JToken? actorJson in characterNode.Where(x => x["peopleType"]?.ToString() == "Actor"))
            {
                int id = int.Parse(actorJson["id"]?.ToString() ?? "0");
                string name = actorJson["personName"]?.ToString() ?? string.Empty;
                string image = GetImageURL(actorJson["image"]?.ToObject<string?>());
                string? role = actorJson["name"]?.ToString();
                int? sort = actorJson["sort"]?.ToString().ToInt();
                si.AddActor(new Actor(id, image, name, role, sort));
            }

            foreach (JToken? actorJson in characterNode.Where(x => x["peopleType"]?.ToString() != "Actor"))
            {
                int id = int.Parse(actorJson["id"]?.ToString() ?? "0");
                string name = actorJson["personName"]?.ToString() ?? string.Empty;
                string? role = actorJson["peopleType"]?.ToString();
                string? sort = actorJson["sort"]?.ToString();
                si.AddCrew(new Crew(id, string.Empty, name, role, string.Empty, sort));
            }
        }
    }

    private static void AddSeasonsV4(JObject r, ProcessedSeason.SeasonType seasonType, CachedSeriesInfo si)
    {
        JToken? seasonJsons = r["data"]?["seasons"];
        if (seasonJsons is null)
        {
            return;
        }

        foreach (JToken seasonJson in seasonJsons.Where(x => seasonType == GetSeasonType(x)))
        {
            int seasonId = seasonJson.GetMandatoryInt("id", TVDoc.ProviderType.TheTVDB);
            string? seasonName = (string?)seasonJson["name"];
            int seasonSeriesId = seasonJson.GetMandatoryInt("seriesId", TVDoc.ProviderType.TheTVDB);
            int seasonNumber = seasonJson.GetMandatoryInt("number", TVDoc.ProviderType.TheTVDB);
            string seasonDescription = string.Empty;
            string? imageUrl = (string?)seasonJson["image"];
            string url = string.Empty;

            si.AddSeason(new Season(seasonId, seasonNumber
                , seasonName, seasonDescription, url, imageUrl, seasonSeriesId));
        }
    }

    private static ProcessedSeason.SeasonType GetSeasonType(JToken seasonJson)
    {
        return seasonJson["type"]?["type"]?.ToString() switch
        {
            "official" => ProcessedSeason.SeasonType.aired,
            "dvd" => ProcessedSeason.SeasonType.dvd,
            "absolute" => ProcessedSeason.SeasonType.absolute,
            "alternate" => ProcessedSeason.SeasonType.alternate,
            _ => ProcessedSeason.SeasonType.absolute
        };
    }

    private static Language? GetAppropriateLanguage(JToken? languageOptions, Locale preferredLocale)
    {
        if (languageOptions == null)
        {
            return Languages.Instance.FallbackLanguage;
        }

        if (((JArray)languageOptions).ContainsTyped(preferredLocale.LanguageToUse(TVDoc.ProviderType.TheTVDB)
                .TVDBV4Code()))
        {
            return preferredLocale.LanguageToUse(TVDoc.ProviderType.TheTVDB);
        }

        if (((JArray)languageOptions).Count == 1)
        {
            string languageCode = ((JArray)languageOptions).Single().ToString();

            if (languageCode == "pt")
            {
                return Languages.Instance.GetLanguageFromDialectCode("pt-BR");
            }
            if (languageCode == "zhtw")
            {
                return Languages.Instance.GetLanguageFromDialectCode("zh-TW");
            }
            if (languageCode == "yue")
            {
                return Languages.Instance.GetLanguageFromDialectCode("zh-YU");
            }
            Language? onlyLanguage = Languages.Instance.GetLanguageFromThreeCode(languageCode)
                                     ?? Languages.Instance.GetLanguageFromCode(languageCode);

            if (onlyLanguage != null)
            {
                return onlyLanguage;
            }
            Logger.Error($"Could not parse {languageCode} into a language - assuming {Languages.Instance.FallbackLanguage} (which will probably fail)");
            return Languages.Instance.FallbackLanguage;
        }

        if (((JArray)languageOptions).ContainsTyped(TVSettings.Instance.PreferredTVDBLanguage.TVDBV4Code()))
        {
            return TVSettings.Instance.PreferredTVDBLanguage;
        }

        if (((JArray)languageOptions).Count == 0)
        {
            return null;
        }

        if (((JArray)languageOptions).ContainsTyped(Languages.Instance.FallbackLanguage.TVDBV4Code()))
        {
            return Languages.Instance.FallbackLanguage;
        }

        foreach (Language? y in ((JArray)languageOptions).Select(x => x.ToString())
                 .Where(langCode => langCode.HasValue())
                 .Select(langCode => Languages.Instance.GetLanguageFromThreeCode(langCode))
                 .Where(y => y != null))
        {
            return y;
        }

        return Languages.Instance.FallbackLanguage;
    }

    private static DateTime? GetAirsTimeV4(JObject r)
    {
        string? airsTimeString = (string?)r["data"]?["airsTime"];
        return JsonHelper.ParseAirTime(airsTimeString);
    }

    private static string? GetAirsDayV4(JObject r)
    {
        JToken? jTokens = r["data"]?["airsDays"];
        IEnumerable<JToken>? days = jTokens?.Children().Where(token => (bool)token.Values().First());
        return days?.Select(ConvertDayName).ToCsv();
    }

    private static string ConvertDayName(JToken t)
    {
        JProperty p = (JProperty)t;
        return p.Name.UppercaseFirst();
    }

    public static TvdbSearchResult Search(string text, Locale locale, MediaConfiguration.MediaType type)
    {
        // but, the number could also be a name, so continue searching as usual
        //text = text.Replace(".", " ");
        TvdbSearchResult result = new();

        JObject? jsonSearchResponse = TvdbWebApi.SearchResponse(text, type, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB));

        if (jsonSearchResponse != null)
        {
            ProcessSearchResult(result,jsonSearchResponse, locale);
        }

        Locale defaultLocale = new(TVSettings.Instance.PreferredTVDBLanguage);
        string defaultLanguage = defaultLocale.LanguageToUse(TVDoc.ProviderType.TheTVDB).TVDBV4Code();
        string language = locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).TVDBV4Code();
        if (language == defaultLanguage)
        {
            return result;
        }

        //we also want to search for search terms that match in default language
        JObject? jsonSearchDefaultLangResponse = TvdbWebApi.SearchResponse(text, type, defaultLocale.LanguageToUse(TVDoc.ProviderType.TheTVDB));
        if (jsonSearchDefaultLangResponse != null)
        {
            ProcessSearchResult(result, jsonSearchDefaultLangResponse,defaultLocale);
        }

        return result;
    }

    private static void ProcessSearchResult(TvdbSearchResult result, JObject jsonResponse, Locale locale)
    {
        JToken jToken = jsonResponse["data"] ?? throw new SourceConsistencyException($"Could not get data element from {jsonResponse}",
            TVDoc.ProviderType.TheTVDB);
        try
        {
            IEnumerable<CachedSeriesInfo> cachedSeriesInfos = API.GetEnumSeriesV4(jToken, locale, true);

            foreach (CachedSeriesInfo si in cachedSeriesInfos)
            {
                result.TvShows.Add(si);
            }

            IEnumerable<CachedMovieInfo> cachedMovieInfos = API.GetEnumMoviesV4(jToken, locale, true);

            foreach (CachedMovieInfo si in cachedMovieInfos)
            {
                result.Movies.Add(si);
            }
        }
        catch (InvalidCastException ex)
        {
            Logger.Error("<TVDB ISSUE?>: Did not receive the expected format of json from search results.");
            Logger.Error(ex);
            Logger.Error(jToken.ToString());
        }
    }

    public static void DownloadEpisodeNow(CachedSeriesInfo cachedSeriesInfo, int episodeId, Locale locale,
        ProcessedSeason.SeasonType order)
    {
        JObject? jsonEpisodeResponse = TvdbWebApi.DownloadEpisode(cachedSeriesInfo.Name, episodeId, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB));

        JObject jsonResponseData = (JObject?)jsonEpisodeResponse?["data"] ??
                                   throw new SourceConsistencyException("No Data in Ep Response",
                                       TVDoc.ProviderType.TheTVDB);

        GenerateAddEpisodeV4(cachedSeriesInfo, locale, cachedSeriesInfo, jsonResponseData, order);
    }

    public class TvdbUpdateResponse
    {
        public long LatestTime;
        private readonly SafeList<UpdateRecord> updates = new();

        public IEnumerable<UpdateRecord> Updates => updates;

        public void Add(UpdateRecord update)
        {
            updates.Add(update);
        }

        public void AddRange(IEnumerable<UpdateRecord> update)
        {
            updates.AddRange(update);
        }
    }
}
public class TooManyCallsException : Exception
{
    public TooManyCallsException(string s) :base(s)
    {
    }
}
