using Alphaleonis.Win32.Filesystem;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TVRename;

public interface IDownloadProvider
{
    /// <exception cref="WebException">Condition.</exception>
    /// <exception cref="HttpRequestException">Condition.</exception>
    /// <exception cref="TaskCanceledException">.NET Core and .NET 5.0 and later only: The request failed due to timeout.</exception>
    void RemoveCompletedDownload(TorrentEntry torrent);

    string Name();

    List<TorrentEntry>? GetTorrentDownloads();

    void StartUrlDownload(string torrentUrl);

    void StartTorrentDownload(FileInfo torrentFile);
    DownloadingFinder.DownloadApp Application { get; }
}
