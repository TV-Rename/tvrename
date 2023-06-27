using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TVRename;

// ReSharper disable once InconsistentNaming
internal class uTorrent : IDownloadProvider
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    /// <exception cref="NotSupportedException">Condition.</exception>
    public void RemoveCompletedDownload(TorrentEntry name)
    {
        throw new NotSupportedException();
    }

    public string Name() => "uTorrent";

    /// <exception cref="NotSupportedException">Condition.</exception>
    public void StartUrlDownload(string torrentUrl)
    {
        throw new NotSupportedException();
    }

    public List<TorrentEntry>? GetTorrentDownloads()
    {
        string resDatFile = TVSettings.Instance.ResumeDatPath;
        try
        {
            // get list of files being downloaded by uTorrent
            if (string.IsNullOrEmpty(resDatFile) || !File.Exists(resDatFile))
            {
                return null;
            }

            BTResume btr = new(resDatFile);
            if (!btr.LoadResumeDat())
            {
                return null;
            }

            return btr.AllFilesBeingDownloaded();
        }
        catch (System.IO.IOException i)
        {
            Logger.Warn($"Could not get downloads from uTorrent: {i.Message}");
            return null;
        }
        catch (FormatException fex)
        {
            Logger.Warn($"Could not parse contents of uTorrent resource file: Got a format exception accessing {resDatFile}, message: {fex.Message}");
            return null;
        }
    }

    /// <exception cref="InvalidOperationException">The file is null.</exception>
    /// <exception cref="Win32Exception">An error occurred when opening the associated file</exception>
    /// <exception cref="ObjectDisposedException">The process object has already been disposed.</exception>
    /// <exception cref="System.IO.FileNotFoundException">The PATH environment variable has a string containing quotes.</exception>
    public void StartTorrentDownload(FileInfo torrentFile)
    {
        System.Diagnostics.Process.Start(TVSettings.Instance.uTorrentPath, torrentFile.FullName.InDoubleQuotes());
    }

    public DownloadingFinder.DownloadApp Application => DownloadingFinder.DownloadApp.uTorrent;
}
