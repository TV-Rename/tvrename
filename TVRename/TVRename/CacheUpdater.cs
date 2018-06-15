using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace TVRename
{
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

        private bool DownloadOK;
        private bool DownloadStopOnError;
        private Semaphore WorkerSemaphore;
        private List<Thread> Workers;
        private Thread mDownloaderThread;
        private ICollection<int> downloadIds;

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly NLog.Logger threadslogger = NLog.LogManager.GetLogger("threads");

        public CacheUpdater()
        {
            DownloadDone = true;
            DownloadOK = true;
        }

        public void StartBGDownloadThread(bool stopOnError, ICollection<int> shows)
        {
            if (!DownloadDone)
                return;
            DownloadStopOnError = stopOnError;
            DownloadPct = 0;
            DownloadDone = false;
            DownloadOK = true;

            downloadIds = shows;
            mDownloaderThread = new Thread(Downloader) { Name = "Downloader" };
            mDownloaderThread.Start();
        }

        public bool DoDownloadsFG(bool showProgress, bool showErrorMsgBox, ICollection<int> shows)
        {
            if (TVSettings.Instance.OfflineMode)
                return true; // don't do internet in offline mode!
            logger.Info("Doing downloads in the foreground...");

            StartBGDownloadThread(true, shows);

            const int delayStep = 100;
            int count = 1000 / delayStep; // one second
            while ((count-- > 0) && (!DownloadDone))
                Thread.Sleep(delayStep);

            if (!DownloadDone && showProgress) // downloading still going on, so time to show the dialog if we're not in /hide mode
            {
                DownloadProgress dp = new DownloadProgress(this);
                dp.ShowDialog();
                dp.Update();
            }

            WaitForBGDownloadDone();

            TheTVDB.Instance.SaveCache();

            if (!DownloadOK)
            {
                logger.Warn(TheTVDB.Instance.LastError);
                if (showErrorMsgBox)
                    MessageBox.Show(TheTVDB.Instance.LastError, "Error while downloading", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TheTVDB.Instance.LastError = "";
            }

            return DownloadOK;
        }

        public void StopBGDownloadThread()
        {
            if (mDownloaderThread != null)
            {
                DownloadDone = true;
                mDownloaderThread.Join();

                /*if (Workers != null)
                {
                    foreach (Thread t in Workers)
                        t.Interrupt();
                }

                WaitForAllThreadsAndTidyUp();
                if (mDownloaderThread.IsAlive)
                {
                    mDownloaderThread.Interrupt();
                    mDownloaderThread = null;
                }
                */
                mDownloaderThread = null;
            }
        }

        private void GetThread(object codeIn)
        {
            System.Diagnostics.Debug.Assert(WorkerSemaphore != null);

            try
            {
                WorkerSemaphore.WaitOne(); // don't start until we're allowed to

                int code = (int)(codeIn);

                bool bannersToo = TVSettings.Instance.NeedToDownloadBannerFile();

                threadslogger.Trace("  Downloading " + code);
                bool r = TheTVDB.Instance.EnsureUpdated(code, bannersToo);
                threadslogger.Trace("  Finished " + code);
                if (!r)
                {
                    DownloadOK = false;
                    if (DownloadStopOnError)
                        DownloadDone = true;
                }

                WorkerSemaphore.Release(1);
            }
            catch (Exception e)
            {
                logger.Fatal(e, "Unhandled Exception in GetThread");
                return;
            }
        }

        private void WaitForAllThreadsAndTidyUp()
        {
            if (Workers != null)
            {
                foreach (Thread t in Workers)
                {
                    if (t.IsAlive)
                        t.Join();
                }
            }

            Workers = null;
            WorkerSemaphore = null;
        }

        private void Downloader()
        {
            // do background downloads of webpages
            logger.Info("*******************************");
            logger.Info("Starting Background Download...");
            try
            {
                if (downloadIds.Count == 0)
                {
                    DownloadDone = true;
                    DownloadOK = true;
                    return;
                }

                if (!TheTVDB.Instance.GetUpdates())
                {
                    DownloadDone = true;
                    DownloadOK = false;
                    return;
                }

                // for eachs of the ShowItems, make sure we've got downloaded data for it

                int totalItems = downloadIds.Count;
                int n = 0;

                int numWorkers = TVSettings.Instance.ParallelDownloads;
                logger.Info("Setting up {0} threads to download information from TVDB.com", numWorkers);
                Workers = new List<Thread>();

                WorkerSemaphore = new Semaphore(numWorkers, numWorkers); // allow up to numWorkers working at once

                foreach (int code in downloadIds)
                {
                    DownloadPct = 100 * (n + 1) / (totalItems + 1);
                    DownloadsRemaining = totalItems - n;
                    n++;

                    WorkerSemaphore.WaitOne(); // blocks until there is an available slot
                    Thread t = new Thread(GetThread);
                    Workers.Add(t);
                    t.Name = "GetThread:" + code;
                    t.Start(code); // will grab the semaphore as soon as we make it available
                    int nfr = WorkerSemaphore
                        .Release(1); // release our hold on the semaphore, so that worker can grab it
                    threadslogger.Trace("Started " + code + " pool has " + nfr + " free");
                    Thread.Sleep(1); // allow the other thread a chance to run and grab

                    // tidy up any finished workers
                    for (int i = Workers.Count - 1; i >= 0; i--)
                    {
                        if (!Workers[i].IsAlive)
                            Workers.RemoveAt(i); // remove dead worker
                    }

                    if (DownloadDone)
                        break;
                }

                WaitForAllThreadsAndTidyUp();

                TheTVDB.Instance.UpdatesDoneOk();
                DownloadDone = true;
                DownloadOK = true;
                return;
            }
            catch (ThreadAbortException taa)
            {
                DownloadDone = true;
                DownloadOK = false;
                logger.Error(taa);
                return;
            }
            catch (Exception e)
            {
                DownloadDone = true;
                DownloadOK = false;
                logger.Fatal(e, "UNHANDLED EXCEPTION IN DOWNLOAD THREAD");
                return;
            }
            finally
            {
                Workers = null;
                WorkerSemaphore = null;
            }
        }

        private void WaitForBGDownloadDone()
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
                if (WorkerSemaphore != null) WorkerSemaphore.Dispose();
            }
        }
        private void ReleaseUnmanagedResources()
        {
            StopBGDownloadThread();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
