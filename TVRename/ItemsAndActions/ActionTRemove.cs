using System;

namespace TVRename
{
    class ActionTRemove : Action
    {
        private readonly IDownloadProvider client;
        private readonly TorrentEntry name;

        public ActionTRemove(IDownloadProvider source, TorrentEntry lastFoundEntry, ProcessedEpisode episode)
        {
            Episode = episode;
            client = source;
            name = lastFoundEntry;
        }

        public override string TargetFolder => client.Name();

        public override string ScanListViewGroup => "lvgActionDownloadRSS";
        public override int IconNumber => 6;

        public override IgnoreItem? Ignore => null;

        public override bool SameAs(Item o) => o is ActionTRemove other && other.client == client && other.name == name;

        public override int CompareTo(object o)
        {
            return !(o is ActionTRemove rss) ? 0 : string.Compare(name.TorrentFile, rss.name.TorrentFile, StringComparison.Ordinal);
        }

        public override string Name => "Remove Completed Torrent";

        public override string DestinationFolder => name.DownloadingTo;

        public override string DestinationFile => name.TorrentFile;

        public override string ProgressText => name.TorrentFile;

        public override long SizeOfWork => 1000000;

        public override ActionOutcome Go(TVRenameStats stats)
        {
            try
            {
                client.RemoveCompletedDownload(name);

                return ActionOutcome.Success();
            }
            catch (Exception e)
            {
                return new ActionOutcome(e);
            }
        }

        public override string Produces => name.TorrentFile;
    }
}
