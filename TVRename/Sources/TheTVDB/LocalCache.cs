//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Humanizer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using TVRename.Forms.Utilities;
using File = Alphaleonis.Win32.Filesystem.File;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

// Talk to the TheTVDB web API, and get tv cachedSeries info

// Hierarchy is:
//   TheTVDB -> Series (class CachedSeriesInfo) -> Seasons (class Season) -> Episodes (class Episode)

namespace TVRename.TheTVDB;

// ReSharper disable once InconsistentNaming
public class LocalCache : MediaCache, iTVSource, iMovieSource
{
    private readonly ConcurrentDictionary<int, ExtraEp>
        extraEpisodes; // IDs of extra episodes to grab and merge in on next update

    private readonly UpdateTimeTracker LatestUpdateTime;

    private bool showConnectionIssues;

    //We are using the singleton design pattern
    //http://msdn.microsoft.com/en-au/library/ff650316.aspx

    private static volatile LocalCache? InternalInstance;
    private static readonly object SyncRoot = new();

    private LocalCache()
    {
        LastErrorMessage = string.Empty;
        IsConnected = false;
        extraEpisodes = new ConcurrentDictionary<int, ExtraEp>();
        //assume that the data is up to date (this will be overridden by the value in the XML if we have a prior install)
        //If we have no prior install then the app has no shows and is by definition up-to-date
        LatestUpdateTime = new UpdateTimeTracker();
    }
    public static LocalCache Instance
    {
        get
        {
            if (InternalInstance is null)
            {
                lock (SyncRoot)
                {
                    // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
                    if (InternalInstance is null)
                    {
                        InternalInstance = new LocalCache();
                    }
                }
            }

            return InternalInstance;
        }
    }

    public TVDoc.ProviderType SourceProvider() => TVDoc.ProviderType.TheTVDB;

    public override TVDoc.ProviderType Provider() => TVDoc.ProviderType.TheTVDB;

    public void Setup(FileInfo? loadFrom, FileInfo cache, bool showIssues)
    {
        showConnectionIssues = showIssues;

        System.Diagnostics.Debug.Assert(cache != null);
        CacheFile = cache;

        if (loadFrom is null)
        {
            LoadOk = true;
            return;
        }

        bool mOk = CachePersistor.LoadMovieCache(loadFrom, this);
        bool tvOk = CachePersistor.LoadTvCache(loadFrom, this);
        LoadOk = mOk && tvOk;
        LOGGER.Info($"Assumed we have updates until {LatestUpdateTime}");
    }

    public byte[]? GetTvdbDownload(string url)
    {
        try
        {
            return API.GetTvdbDownload(url);
        }
        catch (WebException e)
        {
            LOGGER.Warn(CurrentDLTask + " : " + e.LoggableDetails() + " : " + url);
            LastErrorMessage = CurrentDLTask + " : " + e.LoggableDetails();
            return null;
        }
        catch (HttpRequestException e)
        {
            LOGGER.Warn(CurrentDLTask + " : " + e.LoggableDetails() + " : " + url);
            LastErrorMessage = CurrentDLTask + " : " + e.LoggableDetails();
            return null;
        }
        catch (System.IO.IOException e)
        {
            LOGGER.Warn(CurrentDLTask + " : " + e.LoggableDetails() + " : " + url);
            LastErrorMessage = CurrentDLTask + " : " + e.LoggableDetails();
            return null;
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException e)
        {
            LOGGER.Warn(CurrentDLTask + " : " + e.LoggableDetails() + " : " + url);
            LastErrorMessage = CurrentDLTask + " : " + e.LoggableDetails();
            return null;
        }
    }

    public CachedSeriesInfo? GetSeriesOrDownload(ISeriesSpecifier id, bool showErrorMsgBox) => HasSeries(id.TvdbId)
        ? Series[id.TvdbId]
        : DownloadSeriesNow(id, false, false, new Locale(TVSettings.Instance.PreferredTVDBLanguage),
            showErrorMsgBox);

    public void UpdatesDoneOk()
    {
        // call when all downloading and updating is done.  updates local Srv_Time with the tentative
        // new_srv_time value.
        LatestUpdateTime.RecordSuccessfulUpdate();
    }

    public CachedMovieInfo? GetMovie(PossibleNewMovie show, Locale preferredLocale, bool showErrorMsgBox) => this.GetMovie(show.RefinedHint, show.PossibleYear, preferredLocale, showErrorMsgBox, false);

    internal IEnumerable<CachedSeriesInfo> ServerTvAccuracyCheck()
    {
        TvdbAccuracyCheck check = new(this);

        Say($"TVDB Accuracy Check (TV) running for {FullShows().Count} shows.");

        Parallel.ForEach(FullShows(),
            new ParallelOptions { MaxDegreeOfParallelism = TVSettings.Instance.ParallelDownloads },
            si =>
            {
                Thread.CurrentThread.Name ??= $"TVDB Consistency Check: {si.Name}"; // Can only set it once
                check.ServerAccuracyCheck(si);
            });

        foreach (string issue in check.Issues)
        {
            LOGGER.Warn(issue);
        }

        SayNothing();
        return check.ShowsToUpdate;
    }
    internal IEnumerable<CachedMovieInfo> ServerMovieAccuracyCheck()
    {
        TvdbAccuracyCheck check = new(this);

        Say($"TVDB Accuracy Check (Movies) running {FullMovies().Count} shows.");

        Parallel.ForEach(FullMovies(),
            new ParallelOptions { MaxDegreeOfParallelism = TVSettings.Instance.ParallelDownloads },
            si =>
            {
                Thread.CurrentThread.Name ??= $"TVDB Consistency Check: {si.Name}"; // Can only set it once
                check.ServerAccuracyCheck(si);
            });

        foreach (string issue in check.Issues)
        {
            LOGGER.Warn(issue);
        }

        SayNothing();
        return check.MoviesToUpdate;
    }

    private Episode? FindEpisodeById(int id)
    {
        lock (SERIES_LOCK)
        {
            return Series
                .Values
                .SelectMany(s => s.Episodes)
                .FirstOrDefault(e => e.EpisodeId == id);
        }
    }

    public bool Connect(bool showErrorMsgBox) => TVDBLogin(showErrorMsgBox);

    public void ForgetEverything()
    {
        lock (MOVIE_LOCK)
        {
            Movies.Clear();
        }
        lock (SERIES_LOCK)
        {
            Series.Clear();
        }

        IsConnected = false;
        SaveCache();

        //All cachedSeries will be forgotten and will be fully refreshed, so we'll only need updates after this point
        LatestUpdateTime.Reset();
        LOGGER.Info($"Forget everything, so we assume we have updates until {LatestUpdateTime}");
    }

    // ReSharper disable once InconsistentNaming
    private bool TVDBLogin(bool showErrorMsgBox)
    {
        Say("TheTVDB Login");
        try
        {
            API.Login(false);
            IsConnected = true;
            return true;
        }
        catch (WebException ex)
        {
            HandleConnectionProblem(showErrorMsgBox, ex);
            return false;
        }
        catch (HttpRequestException wex)
        {
            HandleConnectionProblem(showErrorMsgBox, wex);
            return false;
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException wex)
        {
            HandleConnectionProblem(showErrorMsgBox, wex);
            return false;
        }
        catch (System.IO.IOException ex)
        {
            HandleConnectionProblem(showErrorMsgBox, ex);
            return false;
        }
        finally
        {
            SayNothing();
        }
    }

    private void HandleConnectionProblem(bool showErrorMsgBox, Exception ex)
    {
        Say("Could not connect to TVDB");

        if (ex is WebException wex)
        {
            LOGGER.LogWebException("Error obtaining token from TVDB", wex);
            LastErrorMessage = wex.LoggableDetails();
        }
        else if (ex is HttpRequestException lex)
        {
            LOGGER.LogHttpRequestException("Error obtaining token from TVDB", lex);
            LastErrorMessage = lex.LoggableDetails();
        }
        else if (ex is System.IO.IOException iex)
        {
            LOGGER.LogIoException("Error obtaining token from TVDB", iex);
            LastErrorMessage = iex.LoggableDetails();
        }
        else if (ex is AggregateException { InnerException: HttpRequestException hex })
        {
            LOGGER.LogHttpRequestException("Error obtaining token from TVDB", hex);
            LastErrorMessage = hex.LoggableDetails();
        }
        else
        {
            LOGGER.Error(ex, "Error obtaining token from TVDB");
            LastErrorMessage = ex.Message;
        }

        if (showErrorMsgBox)
        {
            CannotConnectForm form = ex is WebException e
                    ? new CannotConnectForm("Error while obtaining token from TVDB", e.LoggableDetails(), TVDoc.ProviderType.TheTVDB)
                    : new CannotConnectForm("Error while obtaining token from TVDB", ex.Message, TVDoc.ProviderType.TheTVDB)
                ;

            DialogResult result = form.ShowDialog();
            if (result == DialogResult.Abort)
            {
                TVSettings.Instance.OfflineMode = true;
                LastErrorMessage = string.Empty;
            }
        }
    }

    public override bool GetUpdates(List<ISeriesSpecifier> ss, bool showErrorMsgBox, CancellationToken cts)
    {
        Say("Validating TheTVDB cache");
        AddPlaceholders(ss);
        bool auditUpdates = Helpers.InDebug();

        if (!IsConnected && !Connect(showErrorMsgBox))
        {
            SayNothing();
            return false;
        }

        Say("Calculating updates needed from TVDB");
        long updateFromEpochTime = LatestUpdateTime.LastSuccessfulServerUpdateTimecode();

        if (updateFromEpochTime == 0)
        {
            updateFromEpochTime = GetUpdateTimeFromShows();
        }

        this.MarkPlaceholdersDirty();

        if (updateFromEpochTime == 0 && Series.Values.Any(info => !info.IsSearchResultOnly))
        {
            SayNothing();
            LOGGER.Warn(
                $"Not updating as update time is 0. Need to do a Full Refresh on {Series.Values.Count(info => !info.IsSearchResultOnly)} shows. {LatestUpdateTime}");

            return GetUpdatesManually(); 
        }

        if (updateFromEpochTime == 0)
        {
            SayNothing();
            LOGGER.Info("We have no shows yet to get TVDB updates for. Not getting latest updates.");
            return true; // that's it for now
        }

        if (DateTime.UtcNow - updateFromEpochTime.FromUnixTime() > 10.Weeks())
        {
            SayNothing();
            LOGGER.Warn("Last update from TVDB was more than 10 weeks ago, so doing a full refresh.");
            return GetUpdatesManually(); 
        }

        try
        {
            Say("Getting updates list from TVDB");
            List<JObject> updatesResponses = GetUpdatesV4(updateFromEpochTime, cts);

            Say("Processing Updates from TVDB");
            Parallel.ForEach(updatesResponses, new ParallelOptions { MaxDegreeOfParallelism = TVSettings.Instance.ParallelDownloads }, o =>
            {
                Thread.CurrentThread.Name ??= "Recent Updates"; // Can only set it once
                ProcessUpdate(o, cts);
            });

            if (auditUpdates && updatesResponses.Any())
            {
                Say("Recording Updates");
                int n = 0;
                foreach (JObject response in updatesResponses)
                {
                    PersistResponse(response, updateFromEpochTime, n++);
                }
            }
        }
        catch (UpdateCancelledException)
        {
            SayNothing();
            return false;
        }
        catch (SourceConnectivityException conex)
        {
            LOGGER.Warn(conex.Message);
            LastErrorMessage = conex.Message;
            SayNothing();
            return false;
        }
        catch (SourceConsistencyException sce)
        {
            LOGGER.Error(sce);
            LastErrorMessage = sce.Message;
            SayNothing();
            return false;
        }

        Say("Upgrading dirty locks");

        UpgradeDirtyLocks();

        SayNothing();

        return true;
    }

    private bool GetUpdatesManually()
    {
        long time = DateTime.UtcNow.ToUnixTime();
        IEnumerable<CachedSeriesInfo> seriesToUpdate = ServerTvAccuracyCheck();
        foreach (CachedSeriesInfo s in seriesToUpdate)
        {
            this.ForgetShow(s);
            s.Dirty = true;
        }
       
        IEnumerable<CachedMovieInfo> moviesToUpdate = Instance.ServerMovieAccuracyCheck();
        foreach (CachedMovieInfo s in moviesToUpdate)
        {
            this.ForgetMovie(s);
            s.Dirty = true;
        }
        LatestUpdateTime.RegisterServerUpdate(time);
        return true;
    }

    private static void PersistResponse(JObject response, long updateFromEpochTime, int i)
    {
        //open file stream
        using System.IO.StreamWriter file = File.CreateText(PathManager.AuditLogFile($"-{updateFromEpochTime}-{i}"));
        Newtonsoft.Json.JsonSerializer serializer = new();
        //serialize object directly into file stream
        serializer.Serialize(file, response);
    }

    private List<JObject> GetUpdatesV4(long updateFromEpochTime, CancellationToken cts)
    {
        //We need to ask for a number of pages
        //We'll keep asking until we get to a page until there is no next pages

        bool moreUpdates = true;
        int pageNumber = 0;
        const int MAX_NUMBER_OF_CALLS = 1000;
        const int OFFSET = 0;
        long fromEpochTime = updateFromEpochTime - OFFSET;
        List<JObject> updatesResponses = new();

        while (moreUpdates)
        {
            if (cts.IsCancellationRequested)
            {
                throw new UpdateCancelledException();
            }

            JObject jsonUpdateResponse = GetUpdatesJson(fromEpochTime, pageNumber) ?? throw new SourceConnectivityException( $"No Updates available from TVDB: {fromEpochTime}:{pageNumber} ({LastErrorMessage})");

            int numberOfResponses = GetNumResponses(jsonUpdateResponse, GetRequestedTime(fromEpochTime))?? throw new SourceConsistencyException(                    $"NumberOfResponses is null: {fromEpochTime}:{pageNumber}:{jsonUpdateResponse}",                    TVDoc.ProviderType.TheTVDB);

            updatesResponses.Add(jsonUpdateResponse);
            pageNumber++;
            long maxTime = GetUpdateTime(jsonUpdateResponse);
            LOGGER.Info($"Obtained {numberOfResponses} responses from lastupdated query({fromEpochTime.FromUnixTime().ToLocalTime()}) #{pageNumber} - until {maxTime.FromUnixTime().ToLocalTime()} ({maxTime})");

            if (!MoreFrom(jsonUpdateResponse))
            {
                moreUpdates = false;
            }

            //As a safety measure we check that no more than MAX_NUMBER_OF_CALLS calls are made
            if (pageNumber > MAX_NUMBER_OF_CALLS)
            {
                moreUpdates = false;
                string errorMessage =
                    $"We have got {pageNumber} pages of updates and we are not up to date.  The system will need to check again once this set of updates have been processed.{Environment.NewLine}Last Updated time was {LatestUpdateTime.LastSuccessfulServerUpdateDateTime()}{Environment.NewLine}New Last Updated time is {LatestUpdateTime.ProposedServerUpdateDateTime()}{Environment.NewLine}{Environment.NewLine}If the dates keep getting more recent then let the system keep getting {MAX_NUMBER_OF_CALLS} week blocks of updates, otherwise consider a 'Force Refresh All'";

                LOGGER.Error(errorMessage);
                if (showConnectionIssues && Environment.UserInteractive)
                {
                    MessageBox.Show(errorMessage, "Long Running Update", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
        }

        long maxUpdateTime = updatesResponses.Max(GetUpdateTime);

        if (maxUpdateTime > 0)
        {
            LatestUpdateTime.RegisterServerUpdate(maxUpdateTime);
            LOGGER.Info(
                $"Obtained {pageNumber} pages of updates - since (local) {GetRequestedTime(fromEpochTime).ToLocalTime()} - to (local) {LatestUpdateTime}");
        }

        return updatesResponses;
    }

    private void AddPlaceholders(IEnumerable<ISeriesSpecifier> ss)
    {
        IEnumerable<ISeriesSpecifier> seriesSpecifiers = ss.ToList();
        foreach (ISeriesSpecifier downloadShow in seriesSpecifiers.Where(downloadShow =>
                     downloadShow.Media == MediaConfiguration.MediaType.tv && !HasSeries(downloadShow.TvdbId)))
        {
            this.AddPlaceholderSeries(downloadShow);
        }

        foreach (ISeriesSpecifier downloadShow in seriesSpecifiers.Where(downloadShow =>
                     downloadShow.Media == MediaConfiguration.MediaType.movie && !HasMovie(downloadShow.TvdbId)))
        {
            this.AddPlaceholderMovie(downloadShow);
        }
    }

    private static bool MoreFrom(JObject jsonUpdateResponse)
    {
        JToken? x = jsonUpdateResponse["links"]?["next"];
        return x is { Type: JTokenType.String };
    }

    private int? GetNumResponses(JObject jsonUpdateResponse, DateTime requestedTime)
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
            SayNothing();
            LastErrorMessage = ex.Message;

            string msg = "Unable to get latest updates from TVDB " + Environment.NewLine +
                         "Trying to get updates since " + requestedTime.ToLocalTime() +
                         Environment.NewLine + Environment.NewLine +
                         "If the date is very old, please consider a full refresh";

            LOGGER.Warn($"Error obtaining lastupdated query -since(local) {requestedTime.ToLocalTime()}");

            LOGGER.Warn(ex, msg);

            if (showConnectionIssues && Environment.UserInteractive)
            {
                MessageBox.Show(msg, "Error obtaining updates from TVDB", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            return null;
        }
    }

    internal JObject? GetUpdatesJson(long updateFromEpochTime, int page)
    {
        try
        {
            return API.GetShowUpdatesSince(updateFromEpochTime, TVSettings.Instance.PreferredTVDBLanguage.Abbreviation, page);
        }
        catch (System.IO.IOException iex)
        {
            LOGGER.LogIoException(
                $"Error obtaining lastupdated query since (local) {updateFromEpochTime}: Message is", iex);

            SayNothing();
            LastErrorMessage = iex.LoggableDetails();
            return null;
        }
        catch (WebException ex)
        {
            LOGGER.LogWebException(
                $"Error obtaining lastupdated query since (local) {updateFromEpochTime}: Message is", ex);

            SayNothing();
            LastErrorMessage = ex.LoggableDetails();
            return null;
        }
        catch (HttpRequestException ex)
        {
            LOGGER.LogHttpRequestException(
                $"Error obtaining lastupdated query since (local) {updateFromEpochTime}: Message is", ex);

            SayNothing();
            LastErrorMessage = ex.LoggableDetails();
            return null;
        }
        catch (AggregateException aex) when (aex.InnerException is WebException ex)
        {
            LOGGER.LogWebException(
                $"Error obtaining lastupdated query since (local) {updateFromEpochTime}: Message is", ex);

            SayNothing();
            LastErrorMessage = ex.LoggableDetails();
            return null;
        }
        catch (AggregateException aex) when (aex.InnerException is HttpRequestException ex)
        {
            LOGGER.LogHttpRequestException(
                $"Error obtaining lastupdated query since (local) {updateFromEpochTime}: Message is", ex);

            SayNothing();
            LastErrorMessage = ex.LoggableDetails();
            return null;
        }
    }

    private static DateTime GetRequestedTime(long updateFromEpochTime)
    {
        try
        {
            return updateFromEpochTime.FromUnixTime().ToUniversalTime();
        }
        catch (Exception ex)
        {
            LOGGER.Error(ex,
                $"Could not convert {updateFromEpochTime} to DateTime.");
        }

        //Have to do something!!
        return DateTime.UnixEpoch.ToUniversalTime();
    }

    private void UpgradeDirtyLocks()
    {
        // if more than x% of a show's episodes are marked as dirty, just download the entire show again
        foreach (KeyValuePair<int, CachedSeriesInfo> kvp in Series)
        {
            int totaleps = kvp.Value.Episodes.Count;
            int totaldirty = kvp.Value.Episodes.Count(episode => episode.Dirty);

            float percentDirty = 100;
            if (totaldirty > 0 || totaleps > 0)
            {
                percentDirty = 100 * totaldirty / (float)totaleps;
            }

            if (totaleps > 0 && percentDirty >= TVSettings.Instance.PercentDirtyUpgrade()) // 10%
            {
                kvp.Value.Dirty = true;
                kvp.Value.ClearEpisodes();
                LOGGER.Info(
                    $"Planning to download all of {kvp.Value.Name} as {percentDirty}% of the episodes need to be updated");
            }
            else
            {
                LOGGER.Trace(
                    $"Not planning to download all of {kvp.Value.Name} as {percentDirty}% of the episodes need to be updated and that's less than the 10% limit to upgrade.");
            }
        }
    }

    private void ProcessUpdate(JObject jsonResponse, CancellationToken cts)
    {
        // if updatetime > localtime for item, then remove it, so it will be downloaded later
        JToken jToken = jsonResponse["data"] ?? throw new SourceConsistencyException($"Could not get data element from {jsonResponse}", TVDoc.ProviderType.TheTVDB);

        try
        {
            foreach (JObject seriesResponse in jToken.Cast<JObject>())
            {
                if (!cts.IsCancellationRequested)
                {
                    ProcessSeriesUpdateV4(seriesResponse);
                }
            }
        }
        catch (InvalidCastException ex)
        {
            LOGGER.Error("Did not receive the expected format of json from lastupdated query.");
            LOGGER.Error(ex);
            LOGGER.Error(jToken.ToString());
        }
        catch (OverflowException ex)
        {
            LOGGER.Error("Could not parse the json from lastupdated query.");
            LOGGER.Error(ex);
            LOGGER.Error(jToken.ToString());
        }
    }

    private void ProcessSeriesUpdateV4(JObject seriesResponse)
    {
        int id = seriesResponse.GetMandatoryInt("recordId", TVDoc.ProviderType.TheTVDB);
        long time = seriesResponse.GetMandatoryLong("timeStamp", TVDoc.ProviderType.TheTVDB);
        string? entityType = (string?)seriesResponse["entityType"];
        //string method = (string)seriesResponse["method"];

        switch (entityType)
        {
            case "series":
            case "translatedseries":
            case "seriespeople":
                {
                    CachedSeriesInfo? selectedCachedSeriesInfo = GetSeries(id);
                    if (selectedCachedSeriesInfo != null)
                    {
                        ProcessUpdate(selectedCachedSeriesInfo, time, $"as it({id}-{entityType}) has been updated at {time.FromUnixTime().ToLocalTime()} ({time})", true);
                    }
                    return;
                }
            case "movies":
            case "translatedmovies":
            case "movie-genres":
                {
                    CachedMovieInfo? selectedMovieCachedData = GetMovie(id);
                    if (selectedMovieCachedData != null)
                    {
                        ProcessUpdate(selectedMovieCachedData, time, $"as it({id}-{entityType}) has been updated at {time.FromUnixTime().ToLocalTime()} ({time})", true);
                    }

                    return;
                }
            case "episodes":
            case "translatedepisodes":
                {
                    List<CachedSeriesInfo> matchingShows = Series.Values.Where(y => y.Episodes.Any(e => e.EpisodeId == id)).ToList();
                    if (!matchingShows.Any())
                    {
                        return;
                    }

                    foreach (CachedSeriesInfo? selectedCachedSeriesInfo in matchingShows)
                    {
                        ProcessUpdate(selectedCachedSeriesInfo, time, $"({selectedCachedSeriesInfo.Id()}) as episodes({id}-{entityType}) have been updated at {time.FromUnixTime().ToLocalTime()} ({time})", false);
                    }

                    foreach (Episode updatedEpisode in Series.Values.SelectMany(s => s.Episodes).Where(e => e.EpisodeId == id))
                    {
                        updatedEpisode.Dirty = true;
                    }
                    return;
                }
            case "seasons":
            case "translatedseasons":
                {
                    List<CachedSeriesInfo> matchingShows = Series.Values.Where(y => y.Seasons.Any(e => e.SeasonId == id)).ToList();
                    if (!matchingShows.Any())
                    {
                        return;
                    }
                    foreach (CachedSeriesInfo? selectedCachedSeriesInfo in matchingShows)
                    {
                        ProcessUpdate(selectedCachedSeriesInfo, time, $"({selectedCachedSeriesInfo.Id()}) as seasons({id}) have been updated at {time.FromUnixTime().ToLocalTime()} ({time})", false);
                    }
                    return;
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

                return;

            default:
                LOGGER.Error($"Found update record for '{entityType}' = {id}");
                return;
        }
    }

    private static void ProcessUpdate(CachedMediaInfo selectedCachedSeriesInfo, long time, string message, bool notifyTimeDiscrepancy)
    {
        if (time > selectedCachedSeriesInfo.SrvLastUpdated) // newer version on the server
        {
            LOGGER.Info($"Updating {selectedCachedSeriesInfo.Name} {message} - Marking as dirty");
            selectedCachedSeriesInfo.Dirty = true; // mark as dirty, so it'll be fetched again later
        }
        else if (time < selectedCachedSeriesInfo.SrvLastUpdated && notifyTimeDiscrepancy)
        {
            LOGGER.Error(
                $"{selectedCachedSeriesInfo.Name} has a lastupdated of {selectedCachedSeriesInfo.SrvLastUpdated.FromUnixTime().ToLocalTime()} server says {time.FromUnixTime().ToLocalTime()} {message}");
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
                    LOGGER.Error(message);
                }
                else
                {
                    LOGGER.Warn(message);
                }

                return nowTime;
            }

            return maxUpdateTime;
        }
        catch (Exception e)
        {
            LOGGER.Error(e, jsonUpdateResponse.ToString());
            return 0;
        }
    }

    private long GetUpdateTimeFromShows()
    {
        // we can use the oldest thing we have locally.  It isn't safe to use the newest thing.
        // This will only happen the first time we do an update, so a false _all update isn't too bad.
        return Series.Values
            .Where(info => !info.IsSearchResultOnly)
            .Select(info => info.SrvLastUpdated)
            .Where(i => i > 0)
            .DefaultIfEmpty(0).Min();
    }

    public enum PagingMethod
    {
        proper, // uses the links/next method
        brute //keeps asking until we get a 0 length response
    }
    
    public void AddOrUpdateEpisode(Episode e)
    {
        lock (SERIES_LOCK)
        {
            if (!Series.ContainsKey(e.SeriesId))
            {
                throw new SourceConsistencyException(
                    $"Can't find the cachedSeries to add the episode to (TheTVDB). EpId:{e.EpisodeId} SeriesId:{e.SeriesId} {e.Name}",
                    TVDoc.ProviderType.TheTVDB);
            }

            CachedSeriesInfo ser = Series[e.SeriesId];

            ser.AddEpisode(e);
        }
    }

    private CachedSeriesInfo? DownloadSeriesNow(ISeriesSpecifier deets, bool episodesToo, bool bannersToo,
        bool showErrorMsgBox) =>
        DownloadSeriesNow(deets, episodesToo, bannersToo, deets.TargetLocale, showErrorMsgBox);

    private CachedSeriesInfo? DownloadSeriesNow(ISeriesSpecifier code, bool episodesToo, bool bannersToo, Locale locale,
        bool showErrorMsgBox)
    {
        if (code.TvdbId == 0)
        {
            SayNothing();
            return null;
        }

        bool forceReload = DoWeForceReloadFor(code.TvdbId);

        Say(GenerateMessage(code.TvdbId, episodesToo, bannersToo));

        try
        {
            CachedSeriesInfo? si = DownloadSeriesInfo(code, locale, showErrorMsgBox);
            this.AddSeriesToCache(si);
            lock (SERIES_LOCK)
            {
                si = GetSeries(code.TvdbId);
            }

            //Now deal with obtaining any episodes for the cachedSeries (we then group them into seasons)
            //tvDB only gives us responses in blocks of 100, so we need to iterate over the pages until we get one with <100 rows
            //We push the results into a bag to use later
            //If there is a problem with the while method then we can be proactive by using /series/{id}/episodes/summary to get the total

            if (si != null)
            {
                ProcessedSeason.SeasonType st = code is ShowConfiguration sc
                    ? sc.Order
                    : ProcessedSeason.SeasonType.aired;

                if (episodesToo || forceReload)
                {
                    ReloadEpisodesV4(code, locale, si, st);
                }
                else
                {
                    //The Series has changed, so we need to check for any new episodes
                    CheckForNewEpisodes(code, locale, si, st);
                }
            }

            HaveReloaded(code.TvdbId);
        }
        catch (SourceConnectivityException)
        {
            SayNothing();
            return null;
        }
        
        Series.TryGetValue(code.TvdbId, out CachedSeriesInfo? returnValue);
        SayNothing();
        return returnValue;
    }

    private static void CheckForNewEpisodes(ISeriesSpecifier code, Locale locale, CachedSeriesInfo si, ProcessedSeason.SeasonType st)
    {
        try
        {
            JObject episodeInfo = API.GetSeriesEpisodesV4(si,
                locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).ThreeAbbreviation, st);

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
            LOGGER.Error(sce);
        }
        catch (SourceConnectivityException sce)
        {
            LOGGER.Warn(sce.Message);
        }
        catch (MediaNotFoundException mnfe)
        {
            LOGGER.Error($"Season Issue: {mnfe.Message}");
        }
    }

    internal CachedSeriesInfo DownloadSeriesInfo(ISeriesSpecifier code, Locale locale, bool showErrorMsgBox)
    {
        if (!IsConnected && !Connect(showErrorMsgBox))
        {
            Say("Failed to Connect to TVDB");
            SayNothing();
            throw new SourceConnectivityException();
        }

        CachedSeriesInfo si;
        if (true)
        {
            ProcessedSeason.SeasonType st = code is ShowConfiguration showConfig
                ? showConfig.Order
                : ProcessedSeason.SeasonType.aired;

            (si, Language? languageCodeToUse) = API.GenerateSeriesInfoV4(DownloadSeriesJson(code, locale), locale, st);
            if (languageCodeToUse != null)
            {
                si.AddTranslations(DownloadSeriesTranslationsJsonV4(code, new Locale(languageCodeToUse), showErrorMsgBox));
            }
        }

        if (si is null)
        {
            LOGGER.Error(
                $"Error obtaining cachedSeries {code} - no cound not generate a cachedSeries from the responses");

            SayNothing();
            throw new SourceConnectivityException();
        }

        return si;
    }
    
    private CachedMovieInfo DownloadMovieInfo(ISeriesSpecifier code, Locale locale, bool showErrorMsgBox)
    {
        if (!IsConnected && !Connect(showErrorMsgBox))
        {
            Say("Failed to Connect to TVDB");
            SayNothing();
            throw new SourceConnectivityException();
        }

        (CachedMovieInfo si, Language? languageCode) = API.GenerateMovieInfoV4(DownloadMovieJson(code, locale), locale);
        if (languageCode != null)
        {
            si.AddTranslations(DownloadMovieTranslationsJsonV4(code, new Locale(languageCode), showErrorMsgBox));
        }

        return si;
    }

    private JObject DownloadMovieJson(ISeriesSpecifier code, Locale locale)
    {
        try
        {
            return API.GetMovieV4(code, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).TVDBV4Code());
        }
        catch (System.IO.IOException ioex)
        {
            LOGGER.LogIoException(
                $"Error obtaining movie {code} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}:",
                ioex);

            SayNothing();
            LastErrorMessage = ioex.LoggableDetails();
            throw new SourceConnectivityException();
        }
        catch (HttpRequestException ex)
        {
            if (ex.Is404())
            {
                LOGGER.Warn($"Movie with Id {code} is no longer available from TVDB (got a 404).");
                SayNothing();

                if (API.TvdbIsUp() && !CanFindEpisodesFor(code, locale)
                   ) //todo - Check whether this is right? will be no episodes for a movie
                {
                    LastErrorMessage = ex.LoggableDetails();
                    string msg = $"Movie with TVDB Id {code} is no longer found on TVDB. Please Update";
                    throw new MediaNotFoundException(code, msg, TVDoc.ProviderType.TheTVDB,
                        TVDoc.ProviderType.TheTVDB, MediaConfiguration.MediaType.movie);
                }
            }

            LOGGER.LogHttpRequestException(
                $"Error obtaining movie {code} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}:",
                ex);

            SayNothing();
            LastErrorMessage = ex.LoggableDetails();
            throw new SourceConnectivityException();
        }
        catch (WebException ex)
        {
            if (ex.Is404())
            {
                LOGGER.Warn($"Movie with Id {code} is no longer available from TVDB (got a 404).");
                SayNothing();

                if (API.TvdbIsUp() && !CanFindEpisodesFor(code, locale)
                   ) //todo - Check whether this is right? will be no episodes for a movie
                {
                    LastErrorMessage = ex.LoggableDetails();
                    string msg = $"Movie with TVDB Id {code} is no longer found on TVDB. Please Update";
                    throw new MediaNotFoundException(code, msg, TVDoc.ProviderType.TheTVDB,
                        TVDoc.ProviderType.TheTVDB, MediaConfiguration.MediaType.movie);
                }
            }

            LOGGER.LogWebException(
                $"Error obtaining movie {code} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}:",
                ex);

            SayNothing();
            LastErrorMessage = ex.LoggableDetails();
            throw new SourceConnectivityException();
        }
    }
    
    private JObject DownloadSeriesJson(ISeriesSpecifier code, Locale locale)
    {
        try
        {
            return API.GetSeriesV4(code, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).TVDBV4Code());
        }
        catch (System.IO.IOException e)
        {
            LOGGER.LogIoException(
                $"Error obtaining cachedSeries {code} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}:",
                e);

            SayNothing();
            LastErrorMessage = e.LoggableDetails();
            throw new SourceConnectivityException();
        }
        catch (WebException ex)
        {
            if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse
                { StatusCode: HttpStatusCode.NotFound })
            {
                LOGGER.Warn($"Show with Id {code} is no longer available from TVDB (got a 404).");
                SayNothing();

                if (API.TvdbIsUp() && !CanFindEpisodesFor(code, locale))
                {
                    LastErrorMessage = ex.LoggableDetails();
                    string msg = $"Show with TVDB Id {code} is no longer found on TVDB. Please Update. ";
                    throw new MediaNotFoundException(code, msg, TVDoc.ProviderType.TheTVDB,
                        TVDoc.ProviderType.TheTVDB, MediaConfiguration.MediaType.tv);
                }
            }

            LOGGER.LogWebException(
                $"Error obtaining cachedSeries {code} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}:",
                ex);

            SayNothing();
            LastErrorMessage = ex.LoggableDetails();
            throw new SourceConnectivityException();
        }
        catch (HttpRequestException wex)
        {
            if (wex.Is404())
            {
                LOGGER.Warn($"Show with Id {code.TvdbId} is no longer available from TVDB (got a 404).");

                if (API.TvdbIsUp() && code.TvdbId > 0)
                {
                    string msg = $"Show with TVDB Id {code.TvdbId} is no longer found on TVDB. Please Update";
                    throw new MediaNotFoundException(code, msg, TVDoc.ProviderType.TheTVDB,
                        TVDoc.ProviderType.TheTVDB, MediaConfiguration.MediaType.tv);
                }
            }

            LOGGER.LogHttpRequestException(
                $"Error obtaining cachedSeries {code} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}:",
                wex);

            SayNothing();
            LastErrorMessage = wex.LoggableDetails();
            throw new SourceConnectivityException();
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException wex)
        {
            if (wex.Is404())
            {
                LOGGER.Warn($"Show with Id {code.TvdbId} is no longer available from TVDB (got a 404).");

                if (API.TvdbIsUp() && code.TvdbId >0)
                {
                    string msg = $"Show with TVDB Id {code.TvdbId} is no longer found on TVDB. Please Update";
                    throw new MediaNotFoundException(code, msg, TVDoc.ProviderType.TheTVDB,
                        TVDoc.ProviderType.TheTVDB, MediaConfiguration.MediaType.tv);
                }
            }

            LOGGER.LogHttpRequestException(
                $"Error obtaining cachedSeries {code} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}:",
                wex);

            SayNothing();
            LastErrorMessage = wex.LoggableDetails();
            throw new SourceConnectivityException();
        }
    }

    private JObject DownloadMovieTranslationsJsonV4(ISeriesSpecifier code, Locale locale, bool showErrorMsgBox)
    {
        return HandleWebErrorsFor(
            () => API.GetMovieTranslationsV4(code, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).TVDBV4Code()),
        $"obtaining translations for {code} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}", showErrorMsgBox);
    }

    private JObject DownloadSeriesTranslationsJsonV4(ISeriesSpecifier code, Locale locale, bool showErrorMsgBox)
    {
        return HandleWebErrorsFor(
            () => API.GetSeriesTranslationsV4(code, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).TVDBV4Code()),
            $"obtaining translations for {code.TvdbId} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}", showErrorMsgBox);
    }

    private T HandleWebErrorsFor<T>(Func<T> webCall, string errorMessage, bool showErrorMsgBox)
    {
        if (!IsConnected && !Connect(showErrorMsgBox))
        {
            Say("Failed to Connect to TVDB");
            SayNothing();
            throw new SourceConnectivityException();
        }
        try
        {
            return webCall();
        }
        catch (System.IO.IOException ioex)
        {
            LOGGER.LogIoException($"Error {errorMessage}:", ioex);

            SayNothing();
            LastErrorMessage = ioex.LoggableDetails();
            throw new SourceConnectivityException();
        }
        catch (WebException ex)
        {
            LOGGER.LogWebException($"Error {errorMessage}:", ex);

            SayNothing();
            LastErrorMessage = ex.LoggableDetails();
            throw new SourceConnectivityException();
        }
        catch (AggregateException ex) when (ex.InnerException is WebException wex)
        {
            LOGGER.LogWebException($"Error {errorMessage}:", wex);

            SayNothing();
            LastErrorMessage = wex.LoggableDetails();
            throw new SourceConnectivityException();
        }
        catch (HttpRequestException ex)
        {
            LOGGER.LogHttpRequestException($"Error {errorMessage}:", ex);

            SayNothing();
            LastErrorMessage = ex.LoggableDetails();
            throw new SourceConnectivityException();
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException wex)
        {
            LOGGER.LogHttpRequestException($"Error {errorMessage}:", wex);

            SayNothing();
            LastErrorMessage = wex.LoggableDetails();
            throw new SourceConnectivityException();
        }
        catch (TaskCanceledException ex)
        {
            LOGGER.LogTaskCanceledException($"Error {errorMessage}:", ex);

            SayNothing();
            LastErrorMessage = ex.LoggableDetails();
            throw new SourceConnectivityException();
        }
        catch (AggregateException ex) when (ex.InnerException is TaskCanceledException wex)
        {
            LOGGER.LogTaskCanceledException($"Error {errorMessage}:", wex);

            SayNothing();
            LastErrorMessage = wex.LoggableDetails();
            throw new SourceConnectivityException();
        }
    }

    private static bool CanFindEpisodesFor(ISeriesSpecifier code, Locale locale)
    {
        try
        {
            API.GetSeriesEpisodes(code.TvdbId, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).Abbreviation);
        }
        catch (System.IO.IOException)
        {
            return true;
        }
        catch (WebException ex)
        {
            if (ex is { Status: WebExceptionStatus.ProtocolError, Response: HttpWebResponse
                    { StatusCode: HttpStatusCode.NotFound }
                })
            {
                return false;
            }
        }
        catch (HttpRequestException wex)
        {
            if (wex.StatusCode is HttpStatusCode.NotFound)
            {
                return false;
            }
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException wex)
        {
            if (wex.StatusCode is HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        return true;
    }
    
    private string GenerateMessage(int code, bool episodesToo, bool bannersToo)
    {
        string txt;
        if (Series.ContainsKey(code))
        {
            txt = Series[code].Name.HasValue() ? Series[code].Name : "Code " + code;
        }
        else
        {
            txt = "Code " + code;
        }

        if (episodesToo)
        {
            txt += " (Everything)";
        }
        else
        {
            txt += " Overview";
        }

        if (bannersToo)
        {
            txt += " plus banners";
        }

        return txt;
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
                LOGGER.Error(sce);
            }
            catch (SourceConnectivityException sce)
            {
                LOGGER.Warn(sce.Message);
            }
            catch (MediaNotFoundException mnfe)
            {
                LOGGER.Error($"Season Issue: {mnfe.Message}");
            }
        });
    }

    private static void ReloadEpisode(ISeriesSpecifier code, Locale locale, CachedSeriesInfo si, ProcessedSeason.SeasonType order, Season s)
    {
        JObject seasonInfo = API.GetSeasonEpisodesV4(si, s.SeasonId,
            locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).TVDBV4Code());

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

    private static void GenerateAddEpisodeV4(ISeriesSpecifier code, Locale locale, CachedSeriesInfo si, JToken x, ProcessedSeason.SeasonType order)
    {
        try
        {
            (Episode newEp, Language? bestLanguage) = API.GenerateCoreEpisodeV4(x, code.TvdbId, si, locale, order);
            if (bestLanguage != null)
            {
                newEp.AddTranslations(API.GetEpisodeTranslationsV4(code, newEp.EpisodeId, bestLanguage.TVDBV4Code()));
            }

            si.AddEpisode(newEp);
        }
        catch (MediaNotFoundException mnfe)
        {
            LOGGER.Error($"Episode (+ Translations) claimed to exist, but got a 404 when searching for them. Ignoring Episode, but might be worth a full refresh of the show and contacting TVDB if it does not get resolved. {mnfe.Message}");
            si.Dirty = true;
        }
        catch (SourceConnectivityException sce1)
        {
            LOGGER.Warn(sce1.Message);
            si.Dirty = true;
        }
        catch (SourceConsistencyException sce1)
        {
            LOGGER.Error(sce1);
            si.Dirty = true;
        }
    }

    private bool DownloadEpisodeNow(ISeriesSpecifier series, int episodeId, Locale locale, ProcessedSeason.SeasonType order)
    {
        if (episodeId == 0)
        {
            LOGGER.Warn($"Asked to download episodeId = 0 for cachedSeries {series.Name}:{series.TvdbId}");
            SayNothing();
            return true;
        }

        if (!Series.ContainsKey(series.TvdbId))
        {
            return false; // shouldn't happen
        }

        Episode? ep = FindEpisodeById(episodeId);
        string eptxt = EpisodeDescription(order, episodeId, ep);

        CachedSeriesInfo cachedSeriesInfo = Series[series.TvdbId];
        Say($"{cachedSeriesInfo.Name} ({eptxt}) in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}");

        JObject jsonEpisodeResponse;
        string errorMessage = $"Error obtaining {cachedSeriesInfo.Name} episode[{episodeId}]:";

        try
        {
            jsonEpisodeResponse =
                API.GetEpisode(episodeId, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).Abbreviation) ?? throw new SourceConnectivityException();
        }
        catch (System.IO.IOException ex)
        {
            LOGGER.LogIoException(errorMessage, ex);
            LastErrorMessage = ex.LoggableDetails();
            cachedSeriesInfo.Dirty = true;
            return false;
        }
        catch (WebException ex)
        {
            LOGGER.LogWebException(errorMessage, ex);
            LastErrorMessage = ex.LoggableDetails();
            cachedSeriesInfo.Dirty = true;
            return false;
        }
        catch (HttpRequestException wex)
        {
            LOGGER.LogHttpRequestException(errorMessage, wex);
            LastErrorMessage = wex.LoggableDetails();
            cachedSeriesInfo.Dirty = true;
            return false;
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException wex)
        {
            LOGGER.LogHttpRequestException(errorMessage, wex);
            LastErrorMessage = wex.LoggableDetails();
            cachedSeriesInfo.Dirty = true;
            return false;
        }
        finally
        {
            SayNothing();
        }

        JObject jsonResponseData = (JObject?)jsonEpisodeResponse["data"] ??
                                   throw new SourceConsistencyException("No Data in Ep Response",
                                       TVDoc.ProviderType.TheTVDB);

        GenerateAddEpisodeV4(cachedSeriesInfo, locale, cachedSeriesInfo, jsonResponseData, order);
        return true;
    }

    private static string EpisodeDescription(ProcessedSeason.SeasonType order, int episodeId, Episode? ep)
    {
        if (ep == null)
        {
            return "New Episode Id = " + episodeId;
        }

        return order == ProcessedSeason.SeasonType.dvd
            ? $"S{ep.DvdSeasonNumber:00}E{ep.DvdEpNum:00}"
            : $"S{ep.AiredSeasonNumber:00}E{ep.AiredEpNum:00}";
    }

    public override bool EnsureUpdated(ISeriesSpecifier s, bool bannersToo, bool showErrorMsgBox)
    {
        if (s.Provider != TVDoc.ProviderType.TheTVDB)
        {
            throw new SourceConsistencyException(
                $"Asked to update {s.Name} from The TVDB, but the Id is not for The TVDB.",
                TVDoc.ProviderType.TheTVDB);
        }

        if (s.Media == MediaConfiguration.MediaType.movie)
        {
            return EnsureMovieUpdated(s, showErrorMsgBox);
        }

        return EnsureSeriesUpdated(s, bannersToo, showErrorMsgBox);
    }

    private bool EnsureMovieUpdated(ISeriesSpecifier id, bool showErrorMsgBox)
    {
        lock (MOVIE_LOCK)
        {
            if (Movies.ContainsKey(id.TvdbId) && !Movies[id.TvdbId].Dirty)
            {
                return true;
            }
        }

        if (id.TvdbId == -1)
        {
            //We don't have access to search by other Ids
            throw new MediaNotFoundException(id, $"Please add movie {id.Name} to TVDB, or use another source for that show.", TVDoc.ProviderType.TheTVDB, TVDoc.ProviderType.TheTVDB, MediaConfiguration.MediaType.movie);
        }

        Say($"Movie: {id.Name} from The TVDB");
        try
        {
            CachedMovieInfo? downloadedSi = DownloadMovieNow(id, id.TargetLocale, showErrorMsgBox);

            if (downloadedSi is null)
            {
                return false;
            }

            if (downloadedSi.TvdbCode != id.TvdbId && id.TvdbId == -1)
            {
                lock (MOVIE_LOCK)
                {
                    Movies.TryRemove(-1, out _);
                }
            }

            this.AddMovieToCache(downloadedSi);

            return true;
        }
        catch (SourceConnectivityException conex)
        {
            LastErrorMessage = conex.Message;
            LOGGER.Warn(LastErrorMessage);
            return false;
        }
        catch (SourceConsistencyException sce)
        {
            LOGGER.Error(sce.Message);
            LastErrorMessage = sce.Message;
            return false;
        }
        finally
        {
            SayNothing();
        }
    }

    private bool EnsureSeriesUpdated(ISeriesSpecifier seriesd, bool bannersToo, bool showErrorMsgBox)
    {
        int code = seriesd.TvdbId;

        if (seriesd.TvdbId == -1)
        {
            //We don't have access to search by other Ids
            throw new MediaNotFoundException(seriesd, $"Please add movie {seriesd.Name} to TVDB, or use another source for that show.", TVDoc.ProviderType.TheTVDB, TVDoc.ProviderType.TheTVDB, MediaConfiguration.MediaType.tv);
        }

        if (DoWeForceReloadFor(code) || Series[code].Episodes.Count == 0)
        {
            return DownloadSeriesNow(seriesd, true, bannersToo, showErrorMsgBox) != null; // the whole lot!
        }

        bool ok = true;

        bool seriesNeedsUpdating = Series[code].Dirty;

        if (seriesNeedsUpdating)
        {
            ok = DownloadSeriesNow(seriesd, false, bannersToo, showErrorMsgBox) != null;
        }

        foreach (Episode e in Series[code].Episodes.Where(e => e.Dirty && e.EpisodeId > 0))
        {
            extraEpisodes.TryAdd(e.EpisodeId, new ExtraEp(e.SeriesId, e.EpisodeId, seriesd.SeasonOrder));
            extraEpisodes[e.EpisodeId].Done = false;
        }

        Parallel.ForEach(extraEpisodes.Where(e => e.Value.SeriesId == code && !e.Value.Done), new ParallelOptions { MaxDegreeOfParallelism = TVSettings.Instance.ParallelDownloads }, ee =>
        {
            if (ee.Value.Done)
            {
                return;
            }
            Thread.CurrentThread.Name ??= $"Download Episode {ee.Value.EpisodeId}"; // Can only set it once

            ok = DownloadEpisodeNow(seriesd, ee.Key, seriesd.TargetLocale, ee.Value.Order) && ok;
            ee.Value.Done = true;
        });

        HaveReloaded(code);

        return ok;
    }

    public override void Search(string text, bool showErrorMsgBox, MediaConfiguration.MediaType type, Locale locale)
    {
        if (!IsConnected && !Connect(showErrorMsgBox))
        {
            Say("Failed to Connect to TVDB");
            return;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            Say("Please Search for a Show Name");
            return;
        }

        text = text.RemoveDiacritics(); // API doesn't like accented characters

        if (text.IsNumeric())
        {
            if (int.TryParse(text, out int textAsInt))
            {
                SearchSpecifier ss = new(textAsInt, locale, TVDoc.ProviderType.TheTVDB, type);
                try
                {
                    switch (type)
                    {
                        case MediaConfiguration.MediaType.tv:
                            DownloadSeriesNow(ss, false, false, locale, showErrorMsgBox);
                            break;

                        case MediaConfiguration.MediaType.movie:
                            DownloadMovieNow(ss, locale, showErrorMsgBox);
                            break;
                    }
                }
                catch (MediaNotFoundException)
                {
                    //not really an issue so we can continue
                }
            }
        }

        // but, the number could also be a name, so continue searching as usual
        //text = text.Replace(".", " ");

        JObject? jsonSearchResponse = null;
        JObject? jsonSearchDefaultLangResponse = null;
        try
        {
            jsonSearchResponse = API.SearchV4(text, locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).TVDBV4Code(),
                type);
        }
        catch (WebException ex)
        {
            if (ex.Response is null) //probably a timeout
            {
                LOGGER.LogWebException($"Error obtaining results for search term '{text}':", ex);
                LastErrorMessage = ex.LoggableDetails();
                SayNothing();
            }
            else if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
            {
                LOGGER.Info(
                    $"Could not find any search results for {text} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}");
            }
            else
            {
                LOGGER.LogWebException($"Error obtaining results for search term '{text}':", ex);
                LastErrorMessage = ex.LoggableDetails();
                SayNothing();
            }
        }
        catch (HttpRequestException wex)
        {
            if (wex.StatusCode is HttpStatusCode.NotFound)
            {
                LOGGER.Info(
                    $"Could not find any search results for {text} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}");
            }
            else
            {
                LOGGER.LogHttpRequestException($"Error obtaining results for search term '{text}':", wex);
                LastErrorMessage = wex.LoggableDetails();
                SayNothing();
            }
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException wex)
        {
            if (wex.StatusCode is HttpStatusCode.NotFound)
            {
                LOGGER.Info(
                    $"Could not find any search results for {text} in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}");
            }
            else
            {
                LOGGER.LogHttpRequestException($"Error obtaining results for search term '{text}':", wex);
                LastErrorMessage = wex.LoggableDetails();
                SayNothing();
            }
        }

        if (jsonSearchResponse != null)
        {
            ProcessSearchResult(jsonSearchResponse, locale);
        }

        if (jsonSearchDefaultLangResponse != null)
        //we also want to search for search terms that match in default language
        {
            ProcessSearchResult(jsonSearchDefaultLangResponse,
                new Locale(TVSettings.Instance.PreferredTVDBLanguage));
        }
    }

    public override int PrimaryKey(ISeriesSpecifier ss) => ss.TvdbId;

    public override string CacheSourceName() => "TVDB";

    private void ProcessSearchResult(JObject jsonResponse, Locale locale)
    {
        JToken jToken = jsonResponse["data"] ?? throw new SourceConsistencyException($"Could not get data element from {jsonResponse}",
                TVDoc.ProviderType.TheTVDB);
        try
        {
            IEnumerable<CachedSeriesInfo> cachedSeriesInfos = API.GetEnumSeriesV4(jToken, locale, true);

            foreach (CachedSeriesInfo si in cachedSeriesInfos)
            {
                this.AddSeriesToCache(si);
            }

            IEnumerable<CachedMovieInfo> cachedMovieInfos = API.GetEnumMoviesV4(jToken, locale, true);

            foreach (CachedMovieInfo si in cachedMovieInfos)
            {
                this.AddMovieToCache(si);
            }
        }
        catch (InvalidCastException ex)
        {
            LOGGER.Error("<TVDB ISSUE?>: Did not receive the expected format of json from search results.");
            LOGGER.Error(ex);
            LOGGER.Error(jToken.ToString());
        }
    }

    public void SaveCache()
    {
        lock (MOVIE_LOCK)
        {
            lock (SERIES_LOCK)
            {
                CachePersistor.SaveCache(Series, Movies, CacheFile!,
                    LatestUpdateTime.LastSuccessfulServerUpdateTimecode());
            }
        }
    }

    public void LatestUpdateTimeIs(string time)
    {
        LatestUpdateTime.Load(time);
        LOGGER.Info($"Loaded file with updates until {LatestUpdateTime.LastSuccessfulServerUpdateDateTime()}");
    }

    public CachedMovieInfo? GetMovieAndDownload(ISeriesSpecifier id, Locale locale, bool showErrorMsgBox) => HasMovie(id.TvdbId)
        ? CachedMovieData[id.TvdbId]
        : DownloadMovieNow(id, locale, showErrorMsgBox);

    internal CachedMovieInfo? DownloadMovieNow(ISeriesSpecifier tvdbId, Locale locale, bool showErrorMsgBox)
    {
        if (tvdbId.TvdbId == 0)
        {
            SayNothing();
            return null;
        }

        Say($"Movie with id {tvdbId} from TheTVDB");

        try
        {
            CachedMovieInfo si = DownloadMovieInfo(tvdbId, locale, showErrorMsgBox);
            this.AddMovieToCache(si);
        }
        catch (SourceConnectivityException)
        {
            SayNothing();
            return null;
        }

        lock (MOVIE_LOCK)
        {
            Movies.TryGetValue(tvdbId.TvdbId, out CachedMovieInfo? returnValue);
            SayNothing();
            return returnValue;
        }
    }

    public override void ReConnect(bool showErrorMsgBox)
    {
        Say("TheTVDB Login");
        try
        {
            API.Login(true);
            IsConnected = true;
        }
        catch (System.IO.IOException ex)
        {
            HandleConnectionProblem(showErrorMsgBox, ex);
        }
        catch (WebException ex)
        {
            HandleConnectionProblem(showErrorMsgBox, ex);
        }
        catch (HttpRequestException ex)
        {
            HandleConnectionProblem(showErrorMsgBox, ex);
        }
        finally
        {
            SayNothing();
        }
    }

    TVDoc.ProviderType iMovieSource.SourceProvider() => Provider();
}

public class UpdateCancelledException : Exception
{
}
