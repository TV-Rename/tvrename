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
public class CacheUpdater : IDisposable
{
    public bool DownloadDone;
    public int DownloadsRemaining;
    public int DownloadPct;

    private bool downloadOk;
    private bool downloadStopOnError;
    private bool showErrorMsgBox;
    private Semaphore? workerSemaphore;
    private List<Thread> workers;
    private Thread? mDownloaderThread;
    private ICollection<ISeriesSpecifier> downloadIds;
    public ConcurrentBag<MediaNotFoundException> Problems { get; }

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private static readonly NLog.Logger Threadslogger = NLog.LogManager.GetLogger("threads");

    public CacheUpdater()
    {
        DownloadDone = true;
        downloadOk = true;
        Problems = new ConcurrentBag<MediaNotFoundException>();
        workers = new List<Thread>();
        downloadIds = new List<ISeriesSpecifier>();
    }

    public void StartBgDownloadThread(bool stopOnError, ICollection<ISeriesSpecifier> shows, bool showMsgBox,
        CancellationToken ctsToken)
    {
        if (!DownloadDone)
        {
            return;
        }

        downloadStopOnError = stopOnError;
        showErrorMsgBox = showMsgBox;
        DownloadPct = 0;
        DownloadDone = false;
        downloadOk = true;

        downloadIds = shows;

        ClearProblematicSeriesIds();

        mDownloaderThread = new Thread(Downloader) { Name = "Download Thread" };
        mDownloaderThread.SetApartmentState(ApartmentState.STA);
        mDownloaderThread.Start(ctsToken);
    }

    public bool DoDownloadsFg(bool showProgress, bool showMsgBox, ICollection<ISeriesSpecifier> shows, UI owner)
    {
        if (TVSettings.Instance.OfflineMode)
        {
            Logger.Info("Cancelling downloads... We're in offline mode");
            return true; // don't do internet in offline mode!
        }

        Logger.Info("Doing downloads in the foreground...");

        CancellationTokenSource cts = new();
        StartBgDownloadThread(true, shows, showMsgBox, cts.Token);

        const int DELAY_STEP = 100;
        int count = 1000 / DELAY_STEP; // one second
        while (count-- > 0 && !DownloadDone)
        {
            Thread.Sleep(DELAY_STEP);
        }

        if (!DownloadDone && showProgress) // downloading still going on, so time to show the dialog if we're not in /hide mode
        {
            owner.ShowFgDownloadProgress(this, cts);
        }

        WaitForBgDownloadDone();

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
            CannotConnectForm ccform = new("Error while downloading", message, FindProviderWithError());

            owner.ShowChildDialog(ccform);
            DialogResult ccresult = ccform.DialogResult;
            ccform.Dispose();
            switch (ccresult)
            {
                case DialogResult.Retry:
                    TheTVDB.LocalCache.Instance.ReConnect(false);
                    TVmaze.LocalCache.Instance.ReConnect(false);
                    TMDB.LocalCache.Instance.ReConnect(false);
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

    public void StopBgDownloadThread()
    {
        if (mDownloaderThread is null)
        {
            return;
        }

        DownloadDone = true;
        mDownloaderThread.Join();
        mDownloaderThread = null;
    }

    private void GetThread(object? codeIn)
    {
        System.Diagnostics.Debug.Assert(workerSemaphore != null);

        if (workerSemaphore is null)
        {
            return;
        }

        ISeriesSpecifier series = codeIn switch
        {
            ISeriesSpecifier ss => ss,
            _ => throw new ArgumentException("GetThread started with invalid parameter")
        };

        try
        {
            workerSemaphore.WaitOne(); // don't start until we're allowed to

            bool bannersToo = TVSettings.Instance.NeedToDownloadBannerFile();

            Threadslogger.Trace("  Downloading " + series.Name);
            if (TVDoc.GetMediaCache(series.Provider).EnsureUpdated(series, bannersToo, true))
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
            Logger.Error(sce, sce.Message);
        }
        catch (TaskCanceledException tce)
        {
            Logger.Warn(tce.Message);
        }
        catch (Exception e)
        {
            Logger.Fatal(e, $"Unhandled Exception in GetThread for {series}");
        }
        finally
        {
            Threadslogger.Trace("  Finished " + series);
            workerSemaphore.Release(1);
        }

        //If we get to here the download failed
        downloadOk = false;
        if (downloadStopOnError)
        {
            DownloadDone = true;
        }
    }

    private void WaitForAllThreadsAndTidyUp()
    {
        foreach (Thread t in workers.Where(t => t.IsAlive))
        {
            t.Join();
        }

        workers.Clear();
        workerSemaphore = null;
    }

    private void Downloader(object? token)
    {
        // do background downloads of webpages
        Logger.Info("*******************************");
        Logger.Info("Starting Background Download...");

        CancellationToken cts = (CancellationToken)(token ?? throw new ArgumentNullException(nameof(token)));
        try
        {
            if (downloadIds.Count == 0)
            {
                DownloadDone = true;
                downloadOk = true;
                return;
            }

            if (downloadIds.Any(s => s.Provider == TVDoc.ProviderType.TVmaze))
            {
                if (!TVmaze.LocalCache.Instance.GetUpdates(downloadIds.Where(specifier => specifier.Provider == TVDoc.ProviderType.TVmaze).ToList(), showErrorMsgBox,
                        cts))
                {
                    DownloadDone = true;
                    downloadOk = false;
                    return;
                }
            }

            if (downloadIds.Any(s => s.Provider == TVDoc.ProviderType.TheTVDB))
            {
                if (!TheTVDB.LocalCache.Instance.GetUpdates(downloadIds.Where(specifier => specifier.Provider == TVDoc.ProviderType.TheTVDB).ToList(), showErrorMsgBox,
                        cts))
                {
                    DownloadDone = true;
                    downloadOk = false;
                    return;
                }
            }

            if (downloadIds.Any(s => s.Provider == TVDoc.ProviderType.TMDB))
            {
                if (!TMDB.LocalCache.Instance.GetUpdates(downloadIds.Where(specifier => specifier.Provider == TVDoc.ProviderType.TMDB).ToList(), showErrorMsgBox,
                        cts))
                {
                    DownloadDone = true;
                    downloadOk = false;
                    return;
                }
            }

            // for each of the ShowItems, make sure we've got downloaded data for it

            int totalItems = downloadIds.Count;
            int n = 0;

            int numWorkers = TVSettings.Instance.ParallelDownloads;
            Logger.Info($"Setting up {numWorkers} threads to download information from TheTVDB, TMDB and TVMaze");
            Logger.Info($"Working on {CountIdsFrom(TVDoc.ProviderType.TheTVDB, MediaConfiguration.MediaType.tv)} TVDB, {CountIdsFrom(TVDoc.ProviderType.TMDB, MediaConfiguration.MediaType.tv)} TMDB and {CountIdsFrom(TVDoc.ProviderType.TVmaze, MediaConfiguration.MediaType.tv)} TV Maze shows.");
            Logger.Info($"Working on {CountIdsFrom(TVDoc.ProviderType.TheTVDB, MediaConfiguration.MediaType.movie)} TVDB and {CountIdsFrom(TVDoc.ProviderType.TMDB, MediaConfiguration.MediaType.movie)} TMDB Movies.");
            Logger.Info($"Identified that {CountDirtyIdsFrom(TVDoc.ProviderType.TheTVDB, MediaConfiguration.MediaType.tv)} TVDB, {CountDirtyIdsFrom(TVDoc.ProviderType.TMDB, MediaConfiguration.MediaType.tv)} TMDB and {CountDirtyIdsFrom(TVDoc.ProviderType.TVmaze, MediaConfiguration.MediaType.tv)} TV Maze shows need to be updated");
            Logger.Info($"Identified that {CountDirtyIdsFrom(TVDoc.ProviderType.TheTVDB, MediaConfiguration.MediaType.movie)} TVDB and {CountDirtyIdsFrom(TVDoc.ProviderType.TMDB, MediaConfiguration.MediaType.movie)} TMDB movies need to be updated");
            workers = new List<Thread>();

            Semaphore newSemaphore = new(numWorkers, numWorkers); // allow up to numWorkers working at once
            workerSemaphore = newSemaphore;

            foreach (ISeriesSpecifier code in downloadIds)
            {
                if (cts.IsCancellationRequested)
                {
                    break;
                }
                DownloadPct = 100 * (n + 1) / (totalItems + 1);
                DownloadsRemaining = totalItems - n;
                n++;

                newSemaphore.WaitOne(); // blocks until there is an available slot
                Thread t = new(GetThread);
                workers.Add(t);
                t.Name = "GetThread:" + code.Name;
                t.Start(code); // will grab the semaphore as soon as we make it available
                int nfr = newSemaphore.Release(1); // release our hold on the semaphore, so that worker can grab it
                Threadslogger.Trace("Started " + code + " pool has " + nfr + " free");
                Thread.Sleep(1); // allow the other thread a chance to run and grab

                // tidy up any finished workers
                for (int i = workers.Count - 1; i >= 0; i--)
                {
                    if (!workers[i].IsAlive)
                    {
                        workers.RemoveAt(i); // remove dead worker
                    }
                }

                if (DownloadDone)
                {
                    break;
                }
            }

            WaitForAllThreadsAndTidyUp();

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
            workers.Clear();
            workerSemaphore = null;
            DownloadDone = true;
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

    private void WaitForBgDownloadDone()
    {
        if (mDownloaderThread is { IsAlive: true })
        {
            mDownloaderThread.Join();
        }

        mDownloaderThread = null;
    }

    private void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();
        if (disposing)
        {
            workerSemaphore?.Dispose();
        }
    }

    private void ReleaseUnmanagedResources()
    {
        StopBgDownloadThread();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void ClearProblems()
    {
        List<ISeriesSpecifier> toRemove = new();

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
}
