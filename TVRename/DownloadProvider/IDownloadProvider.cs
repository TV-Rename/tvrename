using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    public interface IDownloadProvider
    {
        void RemoveCompletedDownload(TorrentEntry torrent);
        string Name();
        List<TorrentEntry>? GetTorrentDownloads();
        void StartUrlDownload(string torrentUrl);
        void StartTorrentDownload(FileInfo torrentFile);
    }
}
