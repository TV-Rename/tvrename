using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

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
        private Semaphore workerSemaphore;
        private List<Thread> workers;
        private Thread mDownloaderThread;
        private ICollection<SeriesSpecifier> downloadIds;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly NLog.Logger Threadslogger = NLog.LogManager.GetLogger("threads");

        public CacheUpdater()
        {
            DownloadDone = true;
            downloadOk = true;
        }

        public void StartBgDownloadThread(bool stopOnError, ICollection<SeriesSpecifier> shows)
        {
            if (!DownloadDone)
                return;
            downloadStopOnError = stopOnError;
            DownloadPct = 0;
            DownloadDone = false;
            downloadOk = true;

            downloadIds = shows;
            mDownloaderThread = new Thread(Downloader) { Name = "Downloader" };
            mDownloaderThread.Start();
        }

        public bool DoDownloadsFg(bool showProgress, bool showErrorMsgBox, ICollection<SeriesSpecifier> shows)
        {
            if (TVSettings.Instance.OfflineMode)
                return true; // don't do internet in offline mode!
            Logger.Info("Doing downloads in the foreground...");

            StartBgDownloadThread(true, shows);

            const int DELAY_STEP = 100;
            int count = 1000 / DELAY_STEP; // one second
            while ((count-- > 0) && (!DownloadDone))
                Thread.Sleep(DELAY_STEP);

            if (!DownloadDone && showProgress) // downloading still going on, so time to show the dialog if we're not in /hide mode
            {
                DownloadProgress dp = new DownloadProgress(this);
                dp.ShowDialog();
                dp.Update();
            }

            WaitForBgDownloadDone();

            TheTVDB.Instance.SaveCache();

            if (!downloadOk)
            {
                Logger.Warn(TheTVDB.Instance.LastError);
                if (showErrorMsgBox)
                    MessageBox.Show(TheTVDB.Instance.LastError, "Error while downloading", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TheTVDB.Instance.LastError = "";
            }

            return downloadOk;
        }

        public void StopBgDownloadThread()
        {
            if (mDownloaderThread == null) return;

            DownloadDone = true;
            mDownloaderThread.Join();
            mDownloaderThread = null;
        }

        private void GetThread(object codeIn)
        {
            System.Diagnostics.Debug.Assert(workerSemaphore != null);

            try
            {
                workerSemaphore.WaitOne(); // don't start until we're allowed to

                SeriesSpecifier series = (SeriesSpecifier)(codeIn);

                bool bannersToo = TVSettings.Instance.NeedToDownloadBannerFile();

                Threadslogger.Trace("  Downloading " + series.SeriesId);
                bool r = TheTVDB.Instance.EnsureUpdated(series.SeriesId, bannersToo,series.UseCustomLanguage,series.CustomLanguageCode);
                Threadslogger.Trace("  Finished " + series.SeriesId);
                if (!r)
                {
                    downloadOk = false;
                    if (downloadStopOnError)
                        DownloadDone = true;
                }

                workerSemaphore.Release(1);
            }
            catch (Exception e)
            {
                Logger.Fatal(e, "Unhandled Exception in GetThread");
            }
        }

        private void WaitForAllThreadsAndTidyUp()
        {
            if (workers != null)
            {
                foreach (Thread t in workers)
                {
                    if (t.IsAlive)
                        t.Join();
                }
            }

            workers = null;
            workerSemaphore = null;
        }

        private void Downloader()
        {
            // do background downloads of webpages
            Logger.Info("*******************************");
            Logger.Info("Starting Background Download...");
            try
            {
                if (downloadIds.Count == 0)
                {
                    DownloadDone = true;
                    downloadOk = true;
                    return;
                }

                if (!TheTVDB.Instance.GetUpdates())
                {
                    DownloadDone = true;
                    downloadOk = false;
                    return;
                }

                // for eachs of the ShowItems, make sure we've got downloaded data for it

                int totalItems = downloadIds.Count;
                int n = 0;

                int numWorkers = TVSettings.Instance.ParallelDownloads;
                Logger.Info("Setting up {0} threads to download information from TVDB.com", numWorkers);
                workers = new List<Thread>();

                workerSemaphore = new Semaphore(numWorkers, numWorkers); // allow up to numWorkers working at once

                foreach (SeriesSpecifier code in downloadIds)
                {
                    DownloadPct = 100 * (n + 1) / (totalItems + 1);
                    DownloadsRemaining = totalItems - n;
                    n++;

                    workerSemaphore.WaitOne(); // blocks until there is an available slot
                    Thread t = new Thread(GetThread);
                    workers.Add(t);
                    t.Name = "GetThread:" + code;
                    t.Start(code); // will grab the semaphore as soon as we make it available
                    int nfr = workerSemaphore
                        .Release(1); // release our hold on the semaphore, so that worker can grab it
                    Threadslogger.Trace("Started " + code + " pool has " + nfr + " free");
                    Thread.Sleep(1); // allow the other thread a chance to run and grab

                    // tidy up any finished workers
                    for (int i = workers.Count - 1; i >= 0; i--)
                    {
                        if (!workers[i].IsAlive)
                            workers.RemoveAt(i); // remove dead worker
                    }

                    if (DownloadDone)
                        break;
                }

                WaitForAllThreadsAndTidyUp();

                TheTVDB.Instance.UpdatesDoneOk();
                DownloadDone = true;
                downloadOk = true;
            }
            catch (ThreadAbortException taa)
            {
                DownloadDone = true;
                downloadOk = false;
                Logger.Error(taa);
            }
            catch (Exception e)
            {
                DownloadDone = true;
                downloadOk = false;
                Logger.Fatal(e, "UNHANDLED EXCEPTION IN DOWNLOAD THREAD");
            }
            finally
            {
                workers = null;
                workerSemaphore = null;
            }
        }

        private void WaitForBgDownloadDone()
        {
            if ((mDownloaderThread != null) && (mDownloaderThread.IsAlive))
                mDownloaderThread.Join();
            mDownloaderThread = null;
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                // ReSharper disable once UseNullPropagation
                if (workerSemaphore != null) workerSemaphore.Dispose();
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
    }
}
