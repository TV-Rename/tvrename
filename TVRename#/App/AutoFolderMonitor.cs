using System.Collections.Generic;
using System.IO;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;

namespace TVRename
{
    public class AutoFolderMonitor
    {
        private TVDoc _mDoc;
        private Ui _mUi;
        private List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();
        private System.Timers.Timer _mScanDelayTimer;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public AutoFolderMonitor(TVDoc doc, Ui ui)
        {
            _mDoc = doc;
            _mUi = ui;

            _mScanDelayTimer = new System.Timers.Timer(1000);
            _mScanDelayTimer.Elapsed += new System.Timers.ElapsedEventHandler(mScanDelayTimer_Elapsed);
            _mScanDelayTimer.Stop();
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
            foreach (string efi in _mDoc.SearchFolders)
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
                _watchers.Add(watcher);
                _logger.Trace("Starting logger for {0}", efi);
            }
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            _logger.Trace("Restarted delay timer");
            _mScanDelayTimer.Stop();
            _mScanDelayTimer.Start();
        }

        void mScanDelayTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _mScanDelayTimer.Stop();
            StopMonitor();

            //We only wish to do a scan now if we are not already undertaking one
            if (!_mDoc.CurrentlyBusy)
            {
                _logger.Info("*******************************");
                _logger.Info("Auto scan fired");
                if (_mUi != null)
                {
                    switch (TVSettings.Instance.MonitoredFoldersScanType)
                    {
                        case TVSettings.ScanType.Full:
                            _mUi.Invoke(_mUi.AfmFullScan);
                            break;
                        case TVSettings.ScanType.Recent:
                            _mUi.Invoke(_mUi.AfmRecentScan);
                            break;
                        case TVSettings.ScanType.Quick:
                            _mUi.Invoke(_mUi.AfmQuickScan);
                            break;
                    }

                    _mUi.Invoke(_mUi.AfmDoAll);

                    if (TVSettings.Instance.MonitoredFoldersScanType == TVSettings.ScanType.Full)
                        _mDoc.ExportMissingXML(); // Export Missing episodes to XML if we scanned all

                }

            }
            else
            {
               _logger.Info("Auto scan cancelled as the system is already busy");
            }
            StartMonitor();
        }

        public void StopMonitor()
        {
            foreach (FileSystemWatcher watcher in _watchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            _watchers.Clear();
        }
    }
}
