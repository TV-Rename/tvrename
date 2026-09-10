using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TVRename.Forms;
using TVRename.Forms.Utilities;

namespace TVRename;

/// <inheritdoc />
/// <summary>
/// Updates Provider cache in another thread and reports back progress to UI
/// Handles the update happening in the background and also presenting a UI and bringing the update into the
/// foreground
/// </summary>
public class CacheUpdater : IDisposable, IAsyncDisposable
{
    public int DownloadPct;
    private bool downloadOk;
    private bool downloadStopOnError;
    private bool showErrorMsgBox;
    private Task? mDownloaderThread;
    private ICollection<ISeriesSpecifier> downloadIds;
    public ConcurrentBag<MediaNotFoundException> Problems { get; }

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private static readonly NLog.Logger Threadslogger = NLog.LogManager.GetLogger("threads");

    public CacheUpdater()
    {
        downloadOk = true;
        Problems = [];
        downloadIds = [];
    }

    public void StartBackgroundDownloadAsync(bool stopOnError, ICollection<ISeriesSpecifier> shows, bool showMsgBox, DownloadProgressStatus? p,
        CancellationToken ctsToken)
    {
        if (!DownloadIsHappening())
        {
            downloadStopOnError = stopOnError;
            showErrorMsgBox = showMsgBox;
            DownloadPct = 0;
            downloadOk = true;

            downloadIds = shows;

            ClearProblematicSeriesIds();

            mDownloaderThread = DownloadAsync(p, ctsToken);
        }
    }

    public async Task<bool> DoDownloadsFgAsync(bool showProgress, bool showMsgBox, ICollection<ISeriesSpecifier> shows, UI owner)
    {
        if (TVSettings.Instance.OfflineMode)
        {
            Logger.Info("Cancelling downloads... We're in offline mode");
            return true; // don't do internet in offline mode!
        }

        Logger.Info("Doing downloads in the foreground...");

        CancellationTokenSource cts = new();
        StartBackgroundDownloadAsync(true, shows, showMsgBox, null, cts.Token); //todo hook up progress dialog

        if (DownloadIsHappening() && showProgress) // downloading still going on, so time to show the dialog if we're not in /hide mode
        {
            owner.ShowFgDownloadProgress(this, cts);
        }

        if (mDownloaderThread is not null)
        {
            await mDownloaderThread;
        }

        if (downloadOk)
        {
            return true;
        }

        if (cts.IsCancellationRequested)
        {
            return false;
        }

        string message = TheTVDB.LocalCache.Instance.LastErrorMessage + " " + TVmaze.LocalCache.Instance.LastErrorMessage + " " + TMDB.LocalCache.Instance.LastErrorMessage;
        Logger.Warn(message);
        if (showErrorMsgBox)
        {
            using CannotConnectForm ccform = new("Error while downloading", message, FindProviderWithError());

            owner.ShowChildDialog(ccform);
            DialogResult ccresult = ccform.DialogResult;
            switch (ccresult)
            {
                case DialogResult.Retry:
                    await TheTVDB.LocalCache.Instance.ReConnectAsync(false);
                    await TVmaze.LocalCache.Instance.ReConnectAsync(false);
                    await TMDB.LocalCache.Instance.ReConnectAsync(false);
                    break;
                case DialogResult.Abort:
                    TVSettings.Instance.OfflineMode = true;
                    downloadOk = true;
                    break;
            }
        }

        TheTVDB.LocalCache.Instance.LastErrorMessage = string.Empty;
        TVmaze.LocalCache.Instance.LastErrorMessage = string.Empty;
        TMDB.LocalCache.Instance.LastErrorMessage = string.Empty;

        return downloadOk;
    }

    private static TVDoc.ProviderType FindProviderWithError()
    {
        if (TheTVDB.LocalCache.Instance.LastErrorMessage.HasValue())
        {
            return TVDoc.ProviderType.TheTVDB;
        }
        if (TMDB.LocalCache.Instance.LastErrorMessage.HasValue())
        {
            return TVDoc.ProviderType.TMDB;
        }
        if (TVmaze.LocalCache.Instance.LastErrorMessage.HasValue())
        {
            return TVDoc.ProviderType.TVmaze;
        }

        //Should never get here, but just in case
        return TVSettings.Instance.DefaultProvider;
    }

    public async Task DownloadThreadAsync()
    {
        if (mDownloaderThread is null)
        {
            return;
        }

        await mDownloaderThread;
        mDownloaderThread = null;
    }

    private async Task GetThreadAsync(ISeriesSpecifier series, SemaphoreSlim semaphore, IProgress<DownloadProgressReport>? p, CancellationToken cts)
    {
        await semaphore.WaitAsync(cts).ConfigureAwait(false); // blocks until there is an available slot

        try
        {
            if (cts.IsCancellationRequested) return;

            p?.Report(new DownloadProgressReport
            {
                Provider = series.Provider,
                Message = series.Name ?? "Unknown Show",
                UpdateType = DownloadProgressReport.Type.EpisodeDownload    
            });


            bool bannersToo = TVSettings.Instance.NeedToDownloadBannerFile();

            Threadslogger.Trace("  Downloading " + series.Name);
            if (await TVDoc.GetMediaCache(series.Provider).EnsureUpdatedAsync(series, bannersToo, true))
            {
                return;
            }
        }
        catch (MediaNotFoundException snfe)
        {
            Problems.Add(snfe);
            //We don't want this to stop all other threads
            return;
        }
        catch (SourceConsistencyException sce)
        {
            Logger.Error(sce, sce.ErrorText());
        }
        catch (SourceConnectivityException sce)
        {
            Logger.Warn(sce.ErrorText());
        }
        catch (TaskCanceledException tce)
        {
            Logger.Warn(tce.ErrorText());
        }
        catch (Exception e)
        {
            Logger.Fatal(e, $"Unhandled Exception in GetThread for {series}");
        }
        finally
        {
            Threadslogger.Trace("  Finished " + series);
            semaphore.Release();
        }

        //If we get to here the download failed
        downloadOk = false;
    }

    private async Task DownloadAsync(DownloadProgressStatus? p, CancellationToken cts)
    {
        // do background downloads of webpages
        Logger.Info("*******************************");
        Logger.Info("Starting Background Download...");

        try
        {
            if (downloadIds.Count == 0)
            {
                downloadOk = true;
                return;
            }

            Task<bool> tvmazeTask = await GetUpdates(TVDoc.ProviderType.TVmaze, p, cts).ConfigureAwait(false);
            Task<bool> tvdbTask = await GetUpdates(TVDoc.ProviderType.TheTVDB, p, cts).ConfigureAwait(false);
            Task<bool> tmdbTask = await GetUpdates(TVDoc.ProviderType.TMDB, p, cts).ConfigureAwait(false);

            Task.WaitAll(tvdbTask, tmdbTask, tvmazeTask);

            if (tvdbTask.Result == false || tmdbTask.Result == false || tvmazeTask.Result == false) //one of the downloads that was needed failed, so we can't continue
            {
                downloadOk = false;
                return;
            }

            p?.UpdateFromSource(TVDoc.ProviderType.TVmaze, downloadIds.Count(s => s.Provider == TVDoc.ProviderType.TVmaze));
            p?.UpdateFromSource(TVDoc.ProviderType.TheTVDB, downloadIds.Count(s => s.Provider == TVDoc.ProviderType.TheTVDB));
            p?.UpdateFromSource(TVDoc.ProviderType.TMDB, downloadIds.Count(s => s.Provider == TVDoc.ProviderType.TMDB));

            // for each of the ShowItems, make sure we've got downloaded data for it


            int numWorkers = TVSettings.Instance.ParallelDownloads;
            Logger.Info($"Setting up {numWorkers} threads to download information from TheTVDB, TMDB and TVMaze");
            Logger.Info($"Working on {CountIdsFrom(TVDoc.ProviderType.TheTVDB, MediaConfiguration.MediaType.tv)} TVDB, {CountIdsFrom(TVDoc.ProviderType.TMDB, MediaConfiguration.MediaType.tv)} TMDB and {CountIdsFrom(TVDoc.ProviderType.TVmaze, MediaConfiguration.MediaType.tv)} TV Maze shows.");
            Logger.Info($"Working on {CountIdsFrom(TVDoc.ProviderType.TheTVDB, MediaConfiguration.MediaType.movie)} TVDB and {CountIdsFrom(TVDoc.ProviderType.TMDB, MediaConfiguration.MediaType.movie)} TMDB Movies.");
            Logger.Info($"Identified that {CountDirtyIdsFrom(TVDoc.ProviderType.TheTVDB, MediaConfiguration.MediaType.tv)} TVDB, {CountDirtyIdsFrom(TVDoc.ProviderType.TMDB, MediaConfiguration.MediaType.tv)} TMDB and {CountDirtyIdsFrom(TVDoc.ProviderType.TVmaze, MediaConfiguration.MediaType.tv)} TV Maze shows need to be updated");
            Logger.Info($"Identified that {CountDirtyIdsFrom(TVDoc.ProviderType.TheTVDB, MediaConfiguration.MediaType.movie)} TVDB and {CountDirtyIdsFrom(TVDoc.ProviderType.TMDB, MediaConfiguration.MediaType.movie)} TMDB movies need to be updated");

            using (var semaphore = new SemaphoreSlim(numWorkers, numWorkers))
            {
                var tasks = downloadIds.Select(code => GetThreadAsync(code, semaphore, p, cts)).ToArray();
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }

            if (!cts.IsCancellationRequested)
            {
                TheTVDB.LocalCache.Instance.UpdatesDoneOk();
                TVmaze.LocalCache.Instance.UpdatesDoneOk();
                TMDB.LocalCache.Instance.UpdatesDoneOk();
            }
            downloadOk = !cts.IsCancellationRequested;
        }
        catch (ThreadAbortException taa)
        {
            downloadOk = false;
            Logger.Error(taa);
        }
        catch (TaskCanceledException tce)
        {
            downloadOk = false;
            Logger.Warn(tce);
        }
        catch (Exception e)
        {
            downloadOk = false;
            Logger.Fatal(e, "UNHANDLED EXCEPTION IN DOWNLOAD THREAD");
        }
        finally
        {
            if (p != null)
            {
                var x = new DownloadProgressReport
                {
                    UpdateType = DownloadProgressReport.Type.Final,
                    Message = "Cleaning Up"
                };

                ((IProgress<DownloadProgressReport>)p).Report(x);
            }
        }

        async Task<Task<bool>> GetUpdates(TVDoc.ProviderType provider, DownloadProgressStatus? p, CancellationToken cts)
        {
            Task<bool> task = Task.FromResult(true);
            if (downloadIds.Any(s => s.Provider == provider))
            {
                if (p != null)
                {
                    var x = new DownloadProgressReport
                    {
                        UpdateType = DownloadProgressReport.Type.ProviderUpdates,
                        Provider = provider
                    };

                    ((IProgress<DownloadProgressReport>)p).Report(x);
                }

                task = TVDoc.GetMediaCache(provider).GetUpdatesAsync([.. downloadIds.Where(specifier => specifier.Provider == provider)], showErrorMsgBox, cts);
                await task.ConfigureAwait(false);
            }

            return task;
        }
    }

    private int CountIdsFrom(TVDoc.ProviderType provider, MediaConfiguration.MediaType type)
    {
        return downloadIds.Count(s => s.Provider == provider && s.Media == type);
    }

    private int CountDirtyIdsFrom(TVDoc.ProviderType provider, MediaConfiguration.MediaType type)
    {
        return type == MediaConfiguration.MediaType.tv
            ? downloadIds.Count(s => s.Provider == provider && s.Media == type && (TVDoc.GetMediaCache(provider).GetSeries(s.IdFor(provider))?.Dirty ?? true))
            : downloadIds.Count(s => s.Provider == provider && s.Media == type && (TVDoc.GetMediaCache(provider).GetMovie(s.IdFor(provider))?.Dirty ?? true));
    }

    
    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await DownloadThreadAsync();
        Dispose(true);
    }
    private void Dispose(bool disposing)
    {
        Dispose();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public void ClearProblems()
    {
        List<ISeriesSpecifier> toRemove = [];

        foreach (MediaNotFoundException sid in Problems)
        {
            foreach (ISeriesSpecifier ss in downloadIds)
            {
                if (ss.TvdbId == sid.Media.TvdbId && sid.ShowIdProvider == TVDoc.ProviderType.TheTVDB)
                {
                    toRemove.Add(ss);
                }
                if (ss.TvMazeId == sid.Media.TvMazeId && sid.ShowIdProvider == TVDoc.ProviderType.TVmaze)
                {
                    toRemove.Add(ss);
                }
                if (ss.TmdbId == sid.Media.TmdbId && sid.ShowIdProvider == TVDoc.ProviderType.TMDB)
                {
                    toRemove.Add(ss);
                }
            }
        }

        foreach (ISeriesSpecifier s in toRemove)
        {
            downloadIds.Remove(s);
        }

        ClearProblematicSeriesIds();
    }

    private void ClearProblematicSeriesIds()
    {
        while (!Problems.IsEmpty)
        {
            Problems.TryTake(out _);
        }
    }

    internal bool DownloadIsHappening()
    {
        return mDownloaderThread != null && !mDownloaderThread.IsCompleted;
    }
}
