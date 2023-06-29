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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TVRename.Forms.Utilities;
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

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    /// <exception cref="MediaNotFoundException">If the show/movie is not found</exception>
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
        TvdbAccuracyCheck check = new();

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
        TvdbAccuracyCheck check = new();

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

    private bool DownloadEpisodeNow(ISeriesSpecifier series, int episodeId, Locale locale, ProcessedSeason.SeasonType order)
    {
        if (episodeId == 0)
        {
            LOGGER.Warn($"Asked to download episodeId = 0 for cachedSeries {series.Name}:{series.TvdbId}");
            SayNothing();
            return true;
        }

        if (!Series.TryGetValue(series.TvdbId, out CachedSeriesInfo? cachedSeriesInfo))
        {
            return false; // shouldn't happen
        }

        Episode? ep = FindEpisodeById(episodeId);
        string eptxt = ep == null
            ? "New Episode Id = " + episodeId
            : order == ProcessedSeason.SeasonType.dvd
                ? $"S{ep.DvdSeasonNumber:00}E{ep.DvdEpNum:00}"
                : $"S{ep.AiredSeasonNumber:00}E{ep.AiredEpNum:00}";

        Say($"{cachedSeriesInfo.Name} ({eptxt}) in {locale.LanguageToUse(TVDoc.ProviderType.TheTVDB).EnglishName}");
        try
        {
            API.DownloadEpisodeNow(cachedSeriesInfo, episodeId, locale,order);
        }
        catch (SourceConnectivityException e)
        {
            cachedSeriesInfo.Dirty = true;
            HandleConnectionIssue(showConnectionIssues, e);
            return false;
        }
        catch (SourceConsistencyException ex)
        {
            LOGGER.Error(ex);
            cachedSeriesInfo.Dirty = true;
            return false;
        }
        finally
        {
            SayNothing();
        }

        return true;
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

    public bool Connect(bool showErrorMsgBox)
    {
        Say("TheTVDB Login");
        try
        {
            return TvdbWebApi.TVDBLogin();
        }
        catch (SourceConnectivityException e)
        {
            LastErrorMessage = $"Failed to login to TVDB: {e.Message}: {e.InnerException?.Message}";
            HandleConnectionIssue(showErrorMsgBox, e);
        }
        finally
        {
            SayNothing();
        }

        return false;
    }

    private void HandleConnectionIssue(bool showErrorMsgBox, Exception e)
    {
        if (!showErrorMsgBox)
        {
            return;
        }

        CannotConnectForm form = new("Error while obtaining token from TVDB",
            LastErrorMessage??e.Message, TVDoc.ProviderType.TheTVDB);

        DialogResult result = form.ShowDialog();
        if (result != DialogResult.Abort)
        {
            return;
        }

        TVSettings.Instance.OfflineMode = true;
        LastErrorMessage = string.Empty;
    }

    public override void ReConnect(bool showErrorMsgBox)
    {
        Say("TheTVDB Reconnect");
        try
        {
            TvdbWebApi.ReConnect();
        }
        catch (SourceConnectivityException e)
        {
            LastErrorMessage = $"Failed to Reconnect to TVDB: {e.Message}: {e.InnerException?.Message}";
            HandleConnectionIssue(showErrorMsgBox, e);
        }
        finally
        {
            SayNothing();
        }
    }
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

        try
        {
            TvdbWebApi.ReConnect();
        }
        catch (SourceConnectivityException ex)
        {
            HandleConnectionIssue(true,ex);
        } 
        SaveCache();

        //All cachedSeries will be forgotten and will be fully refreshed, so we'll only need updates after this point
        LatestUpdateTime.Reset();
        LOGGER.Info($"Forget everything, so we assume we have updates until {LatestUpdateTime}");
    }

    public override bool GetUpdates(List<ISeriesSpecifier> ss, bool showErrorMsgBox, CancellationToken cts)
    {
        Say("Validating TheTVDB cache");
        AddPlaceholders(ss);

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
            API.TvdbUpdateResponse updatesResponse = API.GetUpdates(updateFromEpochTime, showConnectionIssues, cts);

            long maxUpdateTime = updatesResponse.LatestTime;

            if (maxUpdateTime > 0)
            {
                LatestUpdateTime.RegisterServerUpdate(maxUpdateTime);
                LOGGER.Info(
                    $"Obtained updates - since (local) {updateFromEpochTime.GetRequestedTime().ToLocalTime()} - to (local) {LatestUpdateTime}");
            }

            Say("Processing Updates from TVDB");
            Parallel.ForEach(updatesResponse.Updates,
                new ParallelOptions { MaxDegreeOfParallelism = TVSettings.Instance.ParallelDownloads }, o =>
                {
                    Thread.CurrentThread.Name ??= "Recent Updates"; // Can only set it once
                    ProcessUpdate(o);
                });
        }
        catch (TooManyCallsException e)
        {
            string errorMessage = e.Message +
                                  $"The system will need to check again once this set of updates have been processed.{Environment.NewLine} Last Updated time was {LatestUpdateTime.LastSuccessfulServerUpdateDateTime()}{Environment.NewLine}New Last Updated time is {LatestUpdateTime.ProposedServerUpdateDateTime()}{Environment.NewLine}{Environment.NewLine}If the dates keep getting more recent then let the system keep getting updates, otherwise consider a 'Force Refresh All'";
            LOGGER.Error(errorMessage);
            if (showConnectionIssues && Environment.UserInteractive)
            {
                MessageBox.Show(errorMessage, "Long Running Update", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            LastErrorMessage = errorMessage;
            SayNothing();
            return false;
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
            HandleConnectionIssue(showErrorMsgBox, conex);
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

    private void ProcessUpdate(API.UpdateRecord updateRecord)
    {
        int id = updateRecord.Id;
        long time = updateRecord.Time;
        string entityType = updateRecord.Type.PrettyPrint();

        switch (updateRecord.Type)
        {
            case API.UpdateRecord.UpdateType.series:
            {
                CachedSeriesInfo? selectedCachedSeriesInfo = GetSeries(id);
                if (selectedCachedSeriesInfo != null)
                {
                    MarkDirty(selectedCachedSeriesInfo, time,
                        $"as it({id}-{entityType}) has been updated at {time.FromUnixTime().ToLocalTime()} ({time})",
                        true);
                }

                return;
            }
            case API.UpdateRecord.UpdateType.movie:
            {
                CachedMovieInfo? selectedMovieCachedData = GetMovie(id);
                if (selectedMovieCachedData != null)
                {
                    MarkDirty(selectedMovieCachedData, time,
                        $"as it({id}-{entityType}) has been updated at {time.FromUnixTime().ToLocalTime()} ({time})",
                        true);
                }

                return;
            }
            case API.UpdateRecord.UpdateType.episode:
            {
                List<CachedSeriesInfo> matchingShows =
                    Series.Values.Where(y => y.Episodes.Any(e => e.EpisodeId == id)).ToList();

                if (!matchingShows.Any())
                {
                    return;
                }

                foreach (CachedSeriesInfo? selectedCachedSeriesInfo in matchingShows)
                {
                    MarkDirty(selectedCachedSeriesInfo, time,
                        $"({selectedCachedSeriesInfo.Id()}) as episodes({id}-{entityType}) have been updated at {time.FromUnixTime().ToLocalTime()} ({time})",
                        false);
                }

                foreach (Episode updatedEpisode in Series.Values.SelectMany(s => s.Episodes)
                             .Where(e => e.EpisodeId == id))
                {
                    updatedEpisode.Dirty = true;
                }

                return;
            }
            case API.UpdateRecord.UpdateType.season:
            {
                List<CachedSeriesInfo> matchingShows =
                    Series.Values.Where(y => y.Seasons.Any(e => e.SeasonId == id)).ToList();

                if (!matchingShows.Any())
                {
                    return;
                }

                foreach (CachedSeriesInfo? selectedCachedSeriesInfo in matchingShows)
                {
                    MarkDirty(selectedCachedSeriesInfo, time,
                        $"({selectedCachedSeriesInfo.Id()}) as seasons({id}) have been updated at {time.FromUnixTime().ToLocalTime()} ({time})",
                        false);
                }

                return;
            }
            default:
                LOGGER.Error($"Found update record for '{entityType}' = {id}");
                return;
        }
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

    private void UpgradeDirtyLocks()
    {
        // if more than x% of a show's episodes are marked as dirty, just download the entire show again
        foreach (KeyValuePair<int, CachedSeriesInfo> kvp in Series)
        {
            int totaleps = kvp.Value.Episodes.Count;
            int totaldirty = kvp.Value.Episodes.Count(episode => episode.Dirty);

            float percentDirty = totaleps > 0
                ? (float)totaldirty * 100 / totaleps
                : 100; 

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

    private static void MarkDirty(CachedMediaInfo selectedCachedSeriesInfo, long time, string message, bool notifyTimeDiscrepancy)
    {
        if (time > selectedCachedSeriesInfo.SrvLastUpdated) // newer version on the server
        {
            LOGGER.Info($"Updating {selectedCachedSeriesInfo.Name} {message} - Marking as dirty");
            selectedCachedSeriesInfo.Dirty = true; // mark as dirty, so it'll be fetched again later
        }
        else if (time < selectedCachedSeriesInfo.SrvLastUpdated && notifyTimeDiscrepancy)
        {
            LOGGER.Warn(
                $"{selectedCachedSeriesInfo.Name} has a lastupdated of {selectedCachedSeriesInfo.SrvLastUpdated.FromUnixTime().ToLocalTime()} server says {time.FromUnixTime().ToLocalTime()} {message}");
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

    /// <exception cref="SourceConsistencyException">Episode's Series Id is not found.</exception>
    public void AddOrUpdateEpisode(Episode e)
    {
        lock (SERIES_LOCK)
        {
            if (!Series.TryGetValue(e.SeriesId, out CachedSeriesInfo? ser))
            {
                throw new SourceConsistencyException(
                    $"Can't find the cachedSeries to add the episode to (TheTVDB). EpId:{e.EpisodeId} SeriesId:{e.SeriesId} {e.Name}",
                    TVDoc.ProviderType.TheTVDB);
            }

            ser.AddEpisode(e);
        }
    }

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    /// <exception cref="MediaNotFoundException">If the show/movie is not found</exception>
    private CachedSeriesInfo? DownloadSeriesNow(ISeriesSpecifier deets, bool episodesToo, bool bannersToo,
        bool showErrorMsgBox) =>
        DownloadSeriesNow(deets, episodesToo, bannersToo, deets.TargetLocale, showErrorMsgBox);

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    /// <exception cref="MediaNotFoundException">If the show/movie is not found</exception>
    private CachedSeriesInfo? DownloadSeriesNow(ISeriesSpecifier code, bool episodesToo, bool bannersToo, Locale locale,
        bool showErrorMsgBox)
    {
        if (code.TvdbId == 0)
        {
            SayNothing();
            return null;
        }
        if (!Connect(showErrorMsgBox))
        {
            Say("Failed to Connect to TVDB");
            SayNothing();
        }

        bool forceReload = DoWeForceReloadFor(code.TvdbId);

        Say(GenerateMessage(code.TvdbId, episodesToo, bannersToo));

        try
        {
            CachedSeriesInfo? si = API.DownloadSeriesInfo(code, locale);
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
                    API.ReloadEpisodes(code, locale, si, st);
                }
                else
                {
                    //The Series has changed, so we need to check for any new episodes
                    API.CheckForNewEpisodes(code, locale, si, st);
                }
            }

            HaveReloaded(code.TvdbId);
        }
        catch (SourceConnectivityException ex)
        {
            LOGGER.Warn(ex);
            LastErrorMessage = ex.Message;
            HandleConnectionIssue(showErrorMsgBox, ex);
            SayNothing();
            return null;
        }
        
        Series.TryGetValue(code.TvdbId, out CachedSeriesInfo? returnValue);
        SayNothing();
        return returnValue;
    }

    private string GenerateMessage(int code, bool episodesToo, bool bannersToo)
    {
        string txt;
        if (Series.TryGetValue(code, out CachedSeriesInfo? si))
        {
            txt = si.Name.HasValue() ? si.Name : "Code " + code;
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

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    /// <exception cref="MediaNotFoundException">If the show/movie is not found</exception>
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

    /// <exception cref="MediaNotFoundException">If the show/movie is not found</exception>
    private bool EnsureMovieUpdated(ISeriesSpecifier id, bool showErrorMsgBox)
    {
        lock (MOVIE_LOCK)
        {
            if (Movies.TryGetValue(id.TvdbId, out CachedMovieInfo? movie) && !movie.Dirty)
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
            CachedMovieInfo? downloadedSi = DownloadMovieNow(id, id.TargetLocale,showErrorMsgBox);

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
            HandleConnectionIssue(showErrorMsgBox, conex);
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

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    /// <exception cref="MediaNotFoundException">If the show/movie is not found</exception>
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

        foreach (Episode e in Series[code].Episodes.Where(e => e is { Dirty: true, EpisodeId: > 0 }))
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
                            DownloadMovieNow(ss, locale,showErrorMsgBox);
                            break;
                    }
                }
                catch (MediaNotFoundException)
                {
                    //not really an issue so we can continue
                }
                catch (SourceConnectivityException ex)
                {
                    LOGGER.Warn($"Searcing for {text} may be compromised as got an error from the API", ex);
                    HandleConnectionIssue(showErrorMsgBox, ex);
                }
                catch (SourceConsistencyException ex)
                {
                    LOGGER.Error($"Searcing for {text} may be compromised as got an error from the API", ex);
                }
            }
        }

        try
        {
            TvdbSearchResult sr = API.Search(text, locale, type);

            foreach (CachedSeriesInfo si in sr.TvShows)
            {
                this.AddSeriesToCache(si);
            }

            foreach (CachedMovieInfo si in sr.Movies)
            {
                this.AddMovieToCache(si);
            }
        }
        catch (SourceConnectivityException ex)
        {
            LOGGER.Warn($"Searcing for {text} may be compromised as got an error from the API",ex);
            HandleConnectionIssue(showErrorMsgBox, ex);
        }
        catch (SourceConsistencyException ex)
        {
            LOGGER.Error($"Searcing for {text} may be compromised as got an error from the API", ex);
        }
    }

    public override int PrimaryKey(ISeriesSpecifier ss) => ss.TvdbId;

    public override string CacheSourceName() => "TVDB";

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

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    /// <exception cref="MediaNotFoundException">If the show/movie is not found</exception>
    public CachedMovieInfo? GetMovieAndDownload(ISeriesSpecifier id, Locale locale, bool showErrorMsgBox) => HasMovie(id.TvdbId)
        ? CachedMovieData[id.TvdbId]
        : DownloadMovieNow(id, locale,showErrorMsgBox);

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    /// <exception cref="MediaNotFoundException">If the show/movie is not found</exception>
    private CachedMovieInfo? DownloadMovieNow(ISeriesSpecifier tvdbId, Locale locale, bool showErrroMsgBox)
    {
        if (tvdbId.TvdbId == 0)
        {
            SayNothing();
            return null;
        }

        Say($"Movie with id {tvdbId} from TheTVDB");

        try
        {
            CachedMovieInfo si = API.DownloadMovieInfo(tvdbId, locale);
            this.AddMovieToCache(si);
        }
        catch (SourceConnectivityException e)
        {
            if (!showErrroMsgBox)
            {
                SayNothing();
                return null;
            }

            HandleConnectionIssue(showErrroMsgBox, e);
        }

        lock (MOVIE_LOCK)
        {
            Movies.TryGetValue(tvdbId.TvdbId, out CachedMovieInfo? returnValue);
            SayNothing();
            return returnValue;
        }
    }

    TVDoc.ProviderType iMovieSource.SourceProvider() => Provider();
}

public class TvdbSearchResult
{
    public readonly List<CachedSeriesInfo> TvShows = new();
    public readonly List<CachedMovieInfo> Movies=new();
}

public class UpdateCancelledException : Exception
{
}
