namespace TVRename
{
    public class TorrentEntry : IDownloadInformation // represents a torrent downloading in a doewloader(Torrent)
    {
        public readonly string DownloadingTo;
        public readonly int PercentDone;
        public readonly string TorrentFile;
        public readonly bool Finished;
        public readonly string Key;

        public TorrentEntry(string torrentFile, string to, int percent, bool finished, string key)
        {
            TorrentFile = torrentFile;
            DownloadingTo = to;
            PercentDone = percent;
            Finished = finished;
            Key = key;
        }

        string IDownloadInformation.FileIdentifier => TorrentFile;

        string IDownloadInformation.Destination => DownloadingTo;

        string IDownloadInformation.RemainingText
        {
            get
            {
                int p = PercentDone;
                return p == -1 ? "" : PercentDone + "% Complete";
            }
        }
    }
}
