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
