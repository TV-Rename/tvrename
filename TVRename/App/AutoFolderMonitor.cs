using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using TVRename.Forms;

namespace TVRename;

public sealed class AutoFolderMonitor : IDisposable
{
    private readonly TVDoc mDoc;
    private readonly UI mainForm;
    private readonly List<System.IO.FileSystemWatcher> watchers = new();
    private readonly System.Timers.Timer mScanDelayTimer;
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public AutoFolderMonitor(TVDoc doc, UI ui, int intervalSeconds)
    {
        mDoc = doc;
        mainForm = ui;
        double interval = (double)intervalSeconds * 1000;

        mScanDelayTimer = new System.Timers.Timer(interval);
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
            try
            {
                if (!Directory.Exists(efi)) //Does not exist
                {
                    Logger.Warn($"Could not watch {efi} as it does not exist.");
                    continue;
                }

                if ((File.GetAttributes(efi) & System.IO.FileAttributes.Directory) != System.IO.FileAttributes.Directory)  // not a folder
                {
                    Logger.Warn($"Could not watch {efi} as it is not a file.");
                    continue;
                }

                if (!efi.IsValidDirectory())  // not a valid folder
                {
                    Logger.Error($"Could not watch {efi} as it is a path with invalid characters.");
                    continue;
                }

                System.IO.FileSystemWatcher watcher = new(efi)
                {
                    IncludeSubdirectories = true,
                    EnableRaisingEvents = true
                };

                watcher.Changed += watcher_Changed;
                watcher.Created += watcher_Changed;
                watcher.Renamed += watcher_Changed;
                watchers.Add(watcher);
                Logger.Info("Starting FileSystemWatcher for " + efi);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, $"Failed to start logger for {efi}");
            }
        }
    }

    private void watcher_Changed(object sender, System.IO.FileSystemEventArgs e)
    {
        Logger.Trace("Restarted delay timer");
        try
        {
            mScanDelayTimer.Stop();
            mScanDelayTimer.Start();
        }
        catch (ObjectDisposedException)
        {
            //Ignored as we assume we are shutting down
        }
    }

    private void mScanDelayTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        mScanDelayTimer.Stop();
        Stop();

        //We only wish to do a scan now if we are not already undertaking one
        if (mDoc.AutoScanCanRun())
        {
            Logger.Info("*******************************");
            Logger.Info("Auto scan fired");
            if (TVSettings.Instance.MonitoredFoldersScanType == TVSettings.ScanType.SingleShow)
            {
                throw new ArgumentException("Inappropriate action for auto-scan " + TVSettings.Instance.MonitoredFoldersScanType);
            }
            mainForm.BeginInvoke(mainForm.ScanAndDo, TVSettings.Instance.MonitoredFoldersScanType);
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
        mScanDelayTimer.Dispose();
    }
}
