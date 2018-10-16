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
        private readonly UI mainForm;
        private readonly List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
        private readonly System.Timers.Timer mScanDelayTimer;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public AutoFolderMonitor(TVDoc doc, UI ui)
        {
            mDoc = doc;
            mainForm = ui;

            mScanDelayTimer = new System.Timers.Timer(1000);
            mScanDelayTimer.Elapsed += mScanDelayTimer_Elapsed;
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
                watcher.Changed += watcher_Changed;
                watcher.Created += watcher_Changed;
                watcher.Renamed += watcher_Changed;
                //watcher.Deleted += new FileSystemEventHandler(watcher_Changed);
                //watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime;
                watcher.IncludeSubdirectories = true;
                watcher.EnableRaisingEvents = true;
                watchers.Add(watcher);
                Logger.Trace("Starting logger for {0}", efi);
            }
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Logger.Trace("Restarted delay timer");
            mScanDelayTimer.Stop();
            mScanDelayTimer.Start();
        }

        void mScanDelayTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            mScanDelayTimer.Stop();
            StopMonitor();

            //We only wish to do a scan now if we are not already undertaking one
            if (mDoc.AutoScanCanRun())
            {
                Logger.Info("*******************************");
                Logger.Info("Auto scan fired");
                if (mainForm != null)
                {
                    switch (TVSettings.Instance.MonitoredFoldersScanType)
                    {
                        case TVSettings.ScanType.Full:
                            mainForm.Invoke(mainForm.AfmFullScan);
                            break;
                        case TVSettings.ScanType.Recent:
                            mainForm.Invoke(mainForm.AfmRecentScan);
                            break;
                        case TVSettings.ScanType.Quick:
                            mainForm.Invoke(mainForm.AfmQuickScan);
                            break;
                        case TVSettings.ScanType.SingleShow:
                        default:
                            throw new ArgumentException("Inappropriate action for auto-scan " + TVSettings.Instance.MonitoredFoldersScanType);
                    }

                    mainForm.Invoke(mainForm.AfmDoAll);

                }

            }
            else
            {
               Logger.Info("Auto scan cancelled as the system is already busy");
            }
            StartMonitor();
        }

        public void StopMonitor()
        {
            foreach (FileSystemWatcher watcher in watchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            watchers.Clear();
        }

        public void Dispose()
        {
            // ReSharper disable once UseNullPropagation
            if (mScanDelayTimer != null) mScanDelayTimer.Dispose();
        }
    }
}
