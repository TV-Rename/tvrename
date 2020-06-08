using System;
using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;
using IOException = System.IO.IOException;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class uTorrentFinder: DownloadingFinder
    {
        public uTorrentFinder(TVDoc i) : base(i) { }
        public override bool Active() => TVSettings.Instance.CheckuTorrent;
        protected override string CheckName() => "Looked in the uTorrent queue for the missing files";

        protected override void DoCheck(SetProgressDelegate prog, ICollection<ShowItem> showList,TVDoc.ScanSettings settings)
        {
            // get list of files being downloaded by uTorrent
            string resDatFile = TVSettings.Instance.ResumeDatPath;
            if (string.IsNullOrEmpty(resDatFile) || !File.Exists(resDatFile))
            {
                return;
            }

            try
            {
                BTResume btr = new BTResume(prog, resDatFile);
                if (!btr.LoadResumeDat())
                {
                    return;
                }

                List<TorrentEntry> downloading = btr.AllFilesBeingDownloaded();

                SearchForAppropriateDownloads(downloading, DownloadApp.uTorrent, settings);
            }
            catch (FormatException fex)
            {
                LOGGER.Warn($"Could not parse contents of uTorrent resource file: Got a format exception accessing {resDatFile}, message: {fex.Message}");
            }
        }

        internal static IEnumerable<TorrentEntry> GetTorrentDownloads()
        {
            try
            {
                // get list of files being downloaded by uTorrent
                string resDatFile = TVSettings.Instance.ResumeDatPath;
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
                LOGGER.Warn($"Could not get downloads from uTorrent: {i.Message}");
                return null;
            }
        }

        internal static void StartTorrentDownload(string torrentFileName, string directoryName = "")
        {
            System.Diagnostics.Process.Start(TVSettings.Instance.uTorrentPath, "/directory \"" + new FileInfo(directoryName).Directory.FullName + "\" \"" + torrentFileName + "\"");
        }
    }
}
