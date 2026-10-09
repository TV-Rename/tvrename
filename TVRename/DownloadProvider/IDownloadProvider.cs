//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//



using System.Net;
using System.Net.Http;


namespace TVRename;

public interface IDownloadProvider
{
    /// <exception cref="WebException">Condition.</exception>
    /// <exception cref="HttpRequestException">Condition.</exception>
    /// <exception cref="TaskCanceledException">.NET Core and .NET 5.0 and later only: The request failed due to timeout.</exception>
    Task RemoveCompletedDownloadAsync(TorrentEntry torrent);

    string Name();

    Task<List<TorrentEntry>?> GetTorrentDownloadsAsync();

    Task StartUrlDownloadAsync(string torrentUrl);

    Task StartTorrentDownloadAsync(FileInfo torrentFile);
    DownloadingFinder.DownloadApp Application { get; }
}
