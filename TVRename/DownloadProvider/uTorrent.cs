using System;
using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;

namespace TVRename;

// ReSharper disable once InconsistentNaming
internal class uTorrent : IDownloadProvider
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public void RemoveCompletedDownload(TorrentEntry name)
    {
        throw new NotSupportedException();
    }

    public string Name() => "uTorrent";

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

    public void StartTorrentDownload(FileInfo torrentFile)
    {
        System.Diagnostics.Process.Start(TVSettings.Instance.uTorrentPath, "\""+torrentFile.FullName+ "\"");
    }
}
