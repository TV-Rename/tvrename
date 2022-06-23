namespace TVRename;

// ReSharper disable once InconsistentNaming
internal class uTorrentFinder : DownloadingProviderFinder
{
    public uTorrentFinder(TVDoc doc, TVDoc.ScanSettings settings) : base(doc, new uTorrent(), settings)
    {
    }

    public override bool Active() => TVSettings.Instance.CheckuTorrent;
}
