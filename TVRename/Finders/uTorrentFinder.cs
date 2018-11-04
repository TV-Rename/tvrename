using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class uTorrentFinder: DownloadingFinder
    {
        public uTorrentFinder(TVDoc i) : base(i) { }
        public override bool Active() => TVSettings.Instance.CheckuTorrent;
        
        public override void Check(SetProgressDelegate prog, int startpct, int totPct)
        {
            // get list of files being downloaded by uTorrent
            string resDatFile = TVSettings.Instance.ResumeDatPath;
            if (string.IsNullOrEmpty(resDatFile) || !File.Exists(resDatFile))
                return;

            BTResume btr = new BTResume(prog, resDatFile);
            if (!btr.LoadResumeDat())
                return;

            List<TorrentEntry> downloading = btr.AllFilesBeingDownloaded();

            SearchForAppropriateDownloads(prog, startpct, totPct, downloading, DownloadApp.uTorrent);
        }

        internal static void StartTorrentDownload(string torrentFileName, string directoryName = "")
        {
            System.Diagnostics.Process.Start(TVSettings.Instance.uTorrentPath, "/directory \"" + (new FileInfo(directoryName).Directory.FullName) + "\" \"" + torrentFileName + "\"");
        }
    }
}
