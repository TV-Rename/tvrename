namespace TVRename;

// ReSharper disable once InconsistentNaming
internal class uTorrentFinder(TVDoc doc, TVDoc.ScanSettings settings) : DownloadingProviderFinder(doc, new uTorrent(), settings)
{
    public override bool Active() => TVSettings.Instance.CheckuTorrent;
}
