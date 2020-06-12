using System;
using System.Collections.Generic;
using System.IO;
using File = Alphaleonis.Win32.Filesystem.File;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    class uTorrent : IDownloadProvider
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

                BTResume btr = new BTResume((percent, message) => { }, resDatFile);
                if (!btr.LoadResumeDat())
                {
                    return null;
                }

                return btr.AllFilesBeingDownloaded();
            }
            catch (IOException i)
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
            string torrentFileName = torrentFile.Name;
            System.Diagnostics.Process.Start(TVSettings.Instance.uTorrentPath, "/directory \"" + torrentFile.Directory.FullName + "\" \"" + torrentFileName + "\"");
        }
    }
}
