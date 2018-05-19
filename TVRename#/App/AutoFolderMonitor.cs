using System;
using System.Collections.Generic;
using System.IO;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;

namespace TVRename
{
    public sealed class AutoFolderMonitor :IDisposable 
    {
        private readonly TVDoc mDoc;
        private readonly UI mUI;
        private List<System.IO.FileSystemWatcher> Watchers = new List<System.IO.FileSystemWatcher>();
        private System.Timers.Timer mScanDelayTimer;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public AutoFolderMonitor(TVDoc Doc, UI ui)
        {
            mDoc = Doc;
            mUI = ui;

            mScanDelayTimer = new System.Timers.Timer(1000);
            mScanDelayTimer.Elapsed += new System.Timers.ElapsedEventHandler(mScanDelayTimer_Elapsed);
            mScanDelayTimer.Stop();
        }

        public void SettingsChanged(bool monitor)
        {
            if (monitor)
            {
                StopMonitor();
                StartMonitor();
            }
            else
                StopMonitor();
        }

        public void StartMonitor()
        {
            foreach (string efi in TVSettings.Instance.DownloadFolders )
            {
                if (!Directory.Exists(efi)) //Does not exist
                    continue;

                if ((File.GetAttributes(efi) & FileAttributes.Directory) != (FileAttributes.Directory))  // not a folder
                    continue;


                FileSystemWatcher watcher = new FileSystemWatcher(efi);
                watcher.Changed += new FileSystemEventHandler(watcher_Changed);
                watcher.Created += new FileSystemEventHandler(watcher_Changed);
                watcher.Renamed += new RenamedEventHandler(watcher_Changed);
                //watcher.Deleted += new FileSystemEventHandler(watcher_Changed);
                //watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime;
                watcher.IncludeSubdirectories = true;
                watcher.EnableRaisingEvents = true;
                Watchers.Add(watcher);
                logger.Trace("Starting logger for {0}", efi);
            }
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            logger.Trace("Restarted delay timer");
            mScanDelayTimer.Stop();
            mScanDelayTimer.Start();
        }

        void mScanDelayTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            mScanDelayTimer.Stop();
            this.StopMonitor();

            //We only wish to do a scan now if we are not already undertaking one
            if (!mDoc.CurrentlyBusy)
            {
                logger.Info("*******************************");
                logger.Info("Auto scan fired");
                if (mUI != null)
                {
                    switch (TVSettings.Instance.MonitoredFoldersScanType)
                    {
                        case TVRename.TVSettings.ScanType.Full:
                            mUI.Invoke(mUI.AFMFullScan);
                            break;
                        case TVRename.TVSettings.ScanType.Recent:
                            mUI.Invoke(mUI.AFMRecentScan);
                            break;
                        case TVRename.TVSettings.ScanType.Quick:
                            mUI.Invoke(mUI.AFMQuickScan);
                            break;
                    }

                    mUI.Invoke(mUI.AFMDoAll);

                }

            }
            else
            {
               logger.Info("Auto scan cancelled as the system is already busy");
            }
            this.StartMonitor();
        }

        public void StopMonitor()
        {
            foreach (FileSystemWatcher watcher in Watchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            Watchers.Clear();
        }

        public void Dispose()
        {
            // ReSharper disable once UseNullPropagation
            if (this.mScanDelayTimer != null) this.mScanDelayTimer.Dispose();
        }
    }
}
