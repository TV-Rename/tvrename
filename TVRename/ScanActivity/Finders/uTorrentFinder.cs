namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class uTorrentFinder : DownloadingProviderFinder
    {
        public uTorrentFinder(TVDoc i) : base(i, new uTorrent())
        {
        }

        public override bool Active() => TVSettings.Instance.CheckuTorrent;
    }
}