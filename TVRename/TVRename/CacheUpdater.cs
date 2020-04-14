using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using JetBrains.Annotations;
using TVRename.Forms.Utilities;

namespace TVRename
{
    /// <inheritdoc />
    /// <summary>
    /// Updates TVDB cache in another thread and reports back progress to UI
    /// Handles the update happening in the background and also presenting a UI and bringing the update into the
    /// foreground
    /// </summary>
    public class CacheUpdater:IDisposable
    {
        public bool DownloadDone;
        public int DownloadsRemaining;
        public int DownloadPct;

        private bool downloadOk;
        private bool downloadStopOnError;
        private bool showErrorMsgBox;
        private Semaphore workerSemaphore;
        private List<Thread> workers;
        private Thread mDownloaderThread;
        private ICollection<SeriesSpecifier> downloadIds;
        public ConcurrentBag<ShowNotFoundException> Problems { get; }
        
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly NLog.Logger Threadslogger = NLog.LogManager.GetLogger("threads");

        public CacheUpdater()
        {
            DownloadDone = true;
            downloadOk = true;
            Problems = new ConcurrentBag<ShowNotFoundException>();
        }

        public void StartBgDownloadThread(bool stopOnError, ICollection<SeriesSpecifier> shows, bool showMsgBox,
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

            mDownloaderThread = new Thread(Downloader) { Name = "Downloader" };
            mDownloaderThread.Start(ctsToken);
        }

        public bool DoDownloadsFg(bool showProgress, bool showMsgBox, ICollection<SeriesSpecifier> shows)
        {
            if (TVSettings.Instance.OfflineMode)
            {
                return true; // don't do internet in offline mode!
            }

            Logger.Info("Doing downloads in the foreground...");

            CancellationTokenSource cts = new CancellationTokenSource();
            StartBgDownloadThread(true, shows,showMsgBox,cts.Token);

            const int DELAY_STEP = 100;
            int count = 1000 / DELAY_STEP; // one second
            while (count-- > 0 && !DownloadDone)
            {
                Thread.Sleep(DELAY_STEP);
            }

            if (!DownloadDone && showProgress) // downloading still going on, so time to show the dialog if we're not in /hide mode
            {
                DownloadProgress dp = new DownloadProgress(this);
                DialogResult result = dp.ShowDialog();

                if (result == DialogResult.Abort)
                {
                    cts.Cancel();
                }
            }

            WaitForBgDownloadDone();

            if (!downloadOk)
            {
                Logger.Warn(TheTVDB.LocalCache.Instance.LastErrorMessage +" "+ TVmaze.LocalCache.Instance.LastErrorMessage);
                if (showErrorMsgBox)
                {
                    CannotConnectForm ccform = new CannotConnectForm("Error while downloading", TheTVDB.LocalCache.Instance.LastErrorMessage + " " + TVmaze.LocalCache.Instance.LastErrorMessage);
                    DialogResult ccresult = ccform.ShowDialog();
                    if (ccresult == DialogResult.Abort)
                    {
                        TVSettings.Instance.OfflineMode = true;
                    }
                }

                TheTVDB.LocalCache.Instance.LastErrorMessage = string.Empty;
                TVmaze.LocalCache.Instance.LastErrorMessage = string.Empty;
            }

            return downloadOk;
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
        
        private void GetThread([NotNull] object codeIn)
        {
            System.Diagnostics.Debug.Assert(workerSemaphore != null);

            SeriesSpecifier series;

            switch (codeIn)
            {
                case SeriesSpecifier ss:
                    series = ss;
                    break;

                default:
                    throw new Exception("GetThread started with invalid parameter");
            }

            try
            {
                workerSemaphore.WaitOne(); // don't start until we're allowed to

                bool bannersToo = TVSettings.Instance.NeedToDownloadBannerFile();

                Threadslogger.Trace("  Downloading " + series.Name);
                switch (series.Provider)
                {
                    case ShowItem.ProviderType.TVmaze:
                        if (TVmaze.LocalCache.Instance.EnsureUpdated(series, bannersToo))
                        {
                            return;
                        }

                        break;

                    case ShowItem.ProviderType.TheTVDB:
                        if (TheTVDB.LocalCache.Instance.EnsureUpdated(series, bannersToo))
                        {
                            return;
                        }

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (ShowNotFoundException snfe)
            {
                Problems.Add(snfe);
                //We don't want this to stop all other threads
                return;
            }
            catch (SourceConsistencyException sce)
            {
                Logger.Error(sce.Message);
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
            if (workers != null)
            {
                foreach (Thread t in workers)
                {
                    if (t.IsAlive)
                    {
                        t.Join();
                    }
                }
            }

            workers = null;
            workerSemaphore = null;
        }

        private void Downloader([NotNull] object token)
        {
            // do background downloads of webpages
            Logger.Info("*******************************");
            Logger.Info("Starting Background Download...");

            CancellationToken cts = (CancellationToken)token;
            try
            {
                if (downloadIds.Count == 0)
                {
                    DownloadDone = true;
                    downloadOk = true;
                    return;
                }

                if (downloadIds.Any(s => s.Provider == ShowItem.ProviderType.TVmaze))
                {
                    if (!TVmaze.LocalCache.Instance.GetUpdates(showErrorMsgBox, cts,
                        downloadIds.Where(specifier => specifier.Provider == ShowItem.ProviderType.TVmaze)))
                    {
                        DownloadDone = true;
                        downloadOk = false;
                        return;
                    }
                }

                if (downloadIds.Any(s => s.Provider == ShowItem.ProviderType.TheTVDB))
                {
                    if (!TheTVDB.LocalCache.Instance.GetUpdates(showErrorMsgBox, cts,
                        downloadIds.Where(specifier => specifier.Provider == ShowItem.ProviderType.TheTVDB)))
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
                Logger.Info("Setting up {0} threads to download information from TheTVDB.com and MVMaze.com", numWorkers);
                Logger.Info($"Working on {downloadIds.Count(s => s.Provider == ShowItem.ProviderType.TheTVDB)} TVDB and {downloadIds.Count(s => s.Provider == ShowItem.ProviderType.TVmaze)} TV Maze shows.");
                Logger.Info($"Identified that {downloadIds.Count(s => s.Provider==ShowItem.ProviderType.TheTVDB && (TheTVDB.LocalCache.Instance.GetSeries(s.TvdbSeriesId)?.Dirty??true))} TVDB and {downloadIds.Count(s => s.Provider == ShowItem.ProviderType.TVmaze && (TVmaze.LocalCache.Instance.GetSeries(s.TvMazeSeriesId)?.Dirty ?? true))} TV Maze shows need to be updated");
                workers = new List<Thread>();

                workerSemaphore = new Semaphore(numWorkers, numWorkers); // allow up to numWorkers working at once

                foreach (SeriesSpecifier code in downloadIds)
                {
                    if (cts.IsCancellationRequested)
                    {
                        break;
                    }
                    DownloadPct = 100 * (n + 1) / (totalItems + 1);
                    DownloadsRemaining = totalItems - n;
                    n++;

                    workerSemaphore.WaitOne(); // blocks until there is an available slot
                    Thread t = new Thread(GetThread);
                    workers.Add(t);
                    t.Name = "GetThread:" + code.Name;
                    t.Start(code); // will grab the semaphore as soon as we make it available
                    int nfr = workerSemaphore
                        .Release(1); // release our hold on the semaphore, so that worker can grab it
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
                }
                downloadOk = true;
            }
            catch (ThreadAbortException taa)
            {
                downloadOk = false;
                Logger.Error(taa);
            }
            catch (Exception e)
            {
                downloadOk = false;
                Logger.Fatal(e, "UNHANDLED EXCEPTION IN DOWNLOAD THREAD");
            }
            finally
            {
                workers = null;
                workerSemaphore = null;
                DownloadDone = true;
            }
        }

        private void WaitForBgDownloadDone()
        {
            if (mDownloaderThread != null && mDownloaderThread.IsAlive)
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
            List<SeriesSpecifier> toRemove = new List<SeriesSpecifier>();

            foreach (ShowNotFoundException sid in Problems)
            {
                foreach (SeriesSpecifier ss in downloadIds)
                {
                    if (ss.TvdbSeriesId == sid.ShowId && sid.ShowIdProvider == ShowItem.ProviderType.TheTVDB)
                    {
                        toRemove.Add(ss);
                    }
                    if (ss.TvMazeSeriesId == sid.ShowId && sid.ShowIdProvider == ShowItem.ProviderType.TVmaze)
                    {
                        toRemove.Add(ss);
                    }
                }
            }

            foreach (SeriesSpecifier s in toRemove)
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
}
