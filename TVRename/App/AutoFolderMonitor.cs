using System;
using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    public sealed class AutoFolderMonitor :IDisposable 
    {
        private readonly TVDoc mDoc;
        private readonly UI mainForm;
        private readonly List<System.IO.FileSystemWatcher> watchers = new List<System.IO.FileSystemWatcher>();
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
                Stop();
                Start();
            }
            else
            {
                Stop();
            }
        }

        public void Start()
        {
            foreach (string efi in TVSettings.Instance.DownloadFolders)
            {
                if (!Directory.Exists(efi)) //Does not exist
                {
                    Logger.Warn($"Could not watch {efi} as it does not exist.");
                    continue;
                }

                if ((File.GetAttributes(efi) & System.IO.FileAttributes.Directory) != (System.IO.FileAttributes.Directory))  // not a folder
                {
                    Logger.Warn($"Could not watch {efi} as it is not a file.");
                    continue;
                }

                try
                {
                    System.IO.FileSystemWatcher watcher = new System.IO.FileSystemWatcher(efi);
                    watcher.Changed += watcher_Changed;
                    watcher.Created += watcher_Changed;
                    watcher.Renamed += watcher_Changed;
                    //watcher.Deleted += new FileSystemEventHandler(watcher_Changed);
                    //watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime;
                    watcher.IncludeSubdirectories = true;
                    watcher.EnableRaisingEvents = true;
                    watchers.Add(watcher);
                    Logger.Info("Starting FileSystemWatcher for {0}", efi);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, $"Failed to start logger for {efi}");
                }
            }
        }

        void watcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            Logger.Trace("Restarted delay timer");
            mScanDelayTimer.Stop();
            mScanDelayTimer.Start();
        }

        void mScanDelayTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            mScanDelayTimer.Stop();
            Stop();

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
            Start();
        }

        private void Stop()
        {
            foreach (System.IO.FileSystemWatcher watcher in watchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            watchers.Clear();
        }

        public void Dispose()
        {
            // ReSharper disable once UseNullPropagation
            if (mScanDelayTimer != null)
            {
                mScanDelayTimer.Dispose();
            }
        }
    }
}
