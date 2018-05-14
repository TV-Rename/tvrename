using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace TVRename
{
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

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static NLog.Logger threadslogger = NLog.LogManager.GetLogger("threads");

        public CacheUpdater()
        {
            this.DownloadDone = true;
            this.DownloadOK = true;
        }

        public void StartBGDownloadThread(bool stopOnError, ICollection<int> shows)
        {
            if (!this.DownloadDone)
                return;
            this.DownloadStopOnError = stopOnError;
            this.DownloadPct = 0;
            this.DownloadDone = false;
            this.DownloadOK = true;

            this.downloadIds = shows;
            this.mDownloaderThread = new Thread(this.Downloader) { Name = "Downloader" };
            this.mDownloaderThread.Start();
        }

        public bool DoDownloadsFG(bool showProgress, bool showErrorMsgBox, ICollection<int> shows)
        {
            if (TVSettings.Instance.OfflineMode)
                return true; // don't do internet in offline mode!
            logger.Info("Doing downloads in the foreground...");

            this.StartBGDownloadThread(true, shows);

            const int delayStep = 100;
            int count = 1000 / delayStep; // one second
            while ((count-- > 0) && (!this.DownloadDone))
                System.Threading.Thread.Sleep(delayStep);

            if (!this.DownloadDone && showProgress) // downloading still going on, so time to show the dialog if we're not in /hide mode
            {
                DownloadProgress dp = new DownloadProgress(this);
                dp.ShowDialog();
                dp.Update();
            }

            this.WaitForBGDownloadDone();

            TheTVDB.Instance.SaveCache();

            if (!this.DownloadOK)
            {
                logger.Warn(TheTVDB.Instance.LastError);
                if (showErrorMsgBox)
                    MessageBox.Show(TheTVDB.Instance.LastError, "Error while downloading", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TheTVDB.Instance.LastError = "";
            }

            return this.DownloadOK;
        }

        public void StopBGDownloadThread()
        {
            if (this.mDownloaderThread != null)
            {
                this.DownloadDone = true;
                this.mDownloaderThread.Join();

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
                this.mDownloaderThread = null;
            }
        }

        private void GetThread(Object codeIn)
        {
            System.Diagnostics.Debug.Assert(this.WorkerSemaphore != null);

            try
            {
                this.WorkerSemaphore.WaitOne(); // don't start until we're allowed to

                int code = (int)(codeIn);

                bool bannersToo = TVSettings.Instance.NeedToDownloadBannerFile();

                threadslogger.Trace("  Downloading " + code);
                bool r = TheTVDB.Instance.EnsureUpdated(code, bannersToo);
                threadslogger.Trace("  Finished " + code);
                if (!r)
                {
                    this.DownloadOK = false;
                    if (this.DownloadStopOnError)
                        this.DownloadDone = true;
                }

                this.WorkerSemaphore.Release(1);
            }
            catch (Exception e)
            {
                logger.Fatal(e, "Unhandled Exception in GetThread");
                return;
            }
        }

        private void WaitForAllThreadsAndTidyUp()
        {
            if (this.Workers != null)
            {
                foreach (Thread t in this.Workers)
                {
                    if (t.IsAlive)
                        t.Join();
                }
            }

            this.Workers = null;
            this.WorkerSemaphore = null;
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
                    this.DownloadDone = true;
                    this.DownloadOK = true;
                    return;
                }

                if (!TheTVDB.Instance.GetUpdates())
                {
                    this.DownloadDone = true;
                    this.DownloadOK = false;
                    return;
                }

                // for eachs of the ShowItems, make sure we've got downloaded data for it

                int totalItems = downloadIds.Count;
                int n = 0;

                int numWorkers = TVSettings.Instance.ParallelDownloads;
                logger.Info("Setting up {0} threads to download information from TVDB.com", numWorkers);
                this.Workers = new List<Thread>();

                this.WorkerSemaphore = new Semaphore(numWorkers, numWorkers); // allow up to numWorkers working at once

                foreach (int code in downloadIds)
                {
                    this.DownloadPct = 100 * (n + 1) / (totalItems + 1);
                    this.DownloadsRemaining = totalItems - n;
                    n++;

                    this.WorkerSemaphore.WaitOne(); // blocks until there is an available slot
                    Thread t = new Thread(this.GetThread);
                    this.Workers.Add(t);
                    t.Name = "GetThread:" + code;
                    t.Start(code); // will grab the semaphore as soon as we make it available
                    int nfr = this.WorkerSemaphore
                        .Release(1); // release our hold on the semaphore, so that worker can grab it
                    threadslogger.Trace("Started " + code + " pool has " + nfr + " free");
                    Thread.Sleep(1); // allow the other thread a chance to run and grab

                    // tidy up any finished workers
                    for (int i = this.Workers.Count - 1; i >= 0; i--)
                    {
                        if (!this.Workers[i].IsAlive)
                            this.Workers.RemoveAt(i); // remove dead worker
                    }

                    if (this.DownloadDone)
                        break;
                }

                this.WaitForAllThreadsAndTidyUp();

                TheTVDB.Instance.UpdatesDoneOK();
                this.DownloadDone = true;
                this.DownloadOK = true;
                return;
            }
            catch (ThreadAbortException taa)
            {
                this.DownloadDone = true;
                this.DownloadOK = false;
                logger.Error(taa);
                return;
            }
            catch (Exception e)
            {
                this.DownloadDone = true;
                this.DownloadOK = false;
                logger.Fatal(e, "UNHANDLED EXCEPTION IN DOWNLOAD THREAD");
                return;
            }
            finally
            {
                this.Workers = null;
                this.WorkerSemaphore = null;
            }
        }

        private void WaitForBGDownloadDone()
        {
            if ((this.mDownloaderThread != null) && (this.mDownloaderThread.IsAlive))
                this.mDownloaderThread.Join();
            this.mDownloaderThread = null;
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                // ReSharper disable once UseNullPropagation
                if (this.WorkerSemaphore != null) this.WorkerSemaphore.Dispose();
            }
        }
        private void ReleaseUnmanagedResources()
        {
            this.StopBGDownloadThread();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
