using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TVRename
{
    public class AutoFolderMonitor
    {
        private TVDoc mDoc;
        private UI mUI;
        private List<System.IO.FileSystemWatcher> Watchers = new List<System.IO.FileSystemWatcher>();
        private System.Timers.Timer mScanDelayTimer;

        public AutoFolderMonitor(TVDoc Doc,UI ui)
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
            foreach (string efi in this.mDoc.SearchFolders)
            {
                if (!File.Exists(efi) ||  // doesn't exist
                    ((File.GetAttributes(efi) & FileAttributes.Directory) != (FileAttributes.Directory)) ) // not a folder
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
            }
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            mScanDelayTimer.Stop();
            mScanDelayTimer.Start();
        }

        void mScanDelayTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            mScanDelayTimer.Stop();
            this.StopMonitor();
            if (mUI != null)
            {
                mUI.Invoke(mUI.AFMScan);
                mUI.Invoke(mUI.AFMDoAll);
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
    }
}
