using Alphaleonis.Win32.Filesystem;
using System.Collections.Generic;

namespace TVRename;

public interface IDownloadProvider
{
    void RemoveCompletedDownload(TorrentEntry torrent);

    string Name();

    List<TorrentEntry>? GetTorrentDownloads();

    void StartUrlDownload(string torrentUrl);

    void StartTorrentDownload(FileInfo torrentFile);
    DownloadingFinder.DownloadApp Application { get; }
}
