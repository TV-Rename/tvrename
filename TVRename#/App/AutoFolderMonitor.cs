using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;

namespace TVRename.App
{
    /// <summary>
    /// Monitors search folders, triggering form actions on file change.
    /// </summary>
    public class AutoFolderMonitor
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly TVDoc doc;
        private readonly UI ui;
        private readonly Timer timer;
        private readonly List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

        /// <summary>
        /// Sets a value indicating whether this <see cref="AutoFolderMonitor"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled
        {
            set
            {
                Stop();

                if (value) Start();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoFolderMonitor"/> class.
        /// </summary>
        /// <param name="doc">The settings document.</param>
        /// <param name="ui">The form to trigger.</param>
        public AutoFolderMonitor(TVDoc doc, UI ui)
        {
            this.doc = doc;
            this.ui = ui;

            this.timer = new Timer(1000);
            this.timer.Elapsed += timer_Elapsed;
        }

        /// <summary>
        /// Starts the folder montioring.
        /// </summary>
        public void Start()
        {
            foreach (string dir in this.doc.SearchFolders)
            {
                if (!Directory.Exists(dir)) continue; // Does not exist
                if ((File.GetAttributes(dir) & FileAttributes.Directory) != FileAttributes.Directory) continue; // Not a directory

                FileSystemWatcher watcher = new FileSystemWatcher(dir)
                {
                    IncludeSubdirectories = true,
                    EnableRaisingEvents = true
                };

                watcher.Changed += watcher_Updated;
                watcher.Created += watcher_Updated;
                watcher.Renamed += watcher_Updated;

                this.watchers.Add(watcher);

                Logger.Trace($"Starting logger for {dir}");
            }
        }

        /// <summary>
        /// Stops the folder montioring.
        /// </summary>
        public void Stop()
        {
            foreach (FileSystemWatcher watcher in this.watchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }

            this.watchers.Clear();
        }

        private void watcher_Updated(object sender, FileSystemEventArgs e)
        {
            Logger.Trace("Restarted delay timer");

            this.timer.Stop();
            this.timer.Start();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.timer.Stop();
            Stop();

            // Only start a scan if not already busy
            if (!this.doc.CurrentlyBusy)
            {
                Logger.Info("*******************************");
                Logger.Info("Auto scan fired");

                if (this.ui != null)
                {
                    switch (TVSettings.Instance.MonitoredFoldersScanType)
                    {
                        case TVSettings.ScanType.Full:
                            this.ui.Invoke(this.ui.AFMFullScan);
                            break;
                        case TVSettings.ScanType.Recent:
                            this.ui.Invoke(this.ui.AFMRecentScan);
                            break;
                        case TVSettings.ScanType.Quick:
                            this.ui.Invoke(this.ui.AFMQuickScan);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    this.ui.Invoke(this.ui.AFMDoAll);

                    if (TVSettings.Instance.MonitoredFoldersScanType == TVSettings.ScanType.Full) this.doc.ExportMissingXML(); // Export missing episodes if everything was scanned
                }
            }
            else
            {
                Logger.Info("Auto scan cancelled as the system is already busy");
            }

            Start();
        }
    }
}
