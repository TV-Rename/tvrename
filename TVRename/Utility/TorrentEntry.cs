//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

public class TorrentEntry(string? torrentFile, string to, int percent, bool finished, string? key) : IDownloadInformation // represents a torrent downloading in a downloader(Torrent)
{
    public readonly string DownloadingTo = to;
    public readonly int PercentDone = percent;
    public readonly string? TorrentFile = torrentFile;
    public readonly bool Finished = finished;
    public readonly string? Key = key;

    string? IDownloadInformation.FileIdentifier => TorrentFile;

    string IDownloadInformation.Destination => DownloadingTo;

    string IDownloadInformation.RemainingText => PercentDone == -1 ? "" : PercentDone + "% Complete";
}
