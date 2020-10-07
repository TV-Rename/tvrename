using System.Collections.Generic;

namespace TVRename
{
    abstract class DownloadingProviderFinder : DownloadingFinder
    {
        private readonly IDownloadProvider source;
        protected DownloadingProviderFinder(TVDoc doc, IDownloadProvider source) : base(doc)
        {
            this.source = source;
        }
        protected override string CheckName() => $"Looked in {source.Name()} for the missing files to see if they are being downloaded";

        protected override void DoCheck(SetProgressDelegate prog, TVDoc.ScanSettings settings)
        {
            List<TorrentEntry>? downloading = source.GetTorrentDownloads();
            if (downloading is null)
            {
                LOGGER.Warn($"Failed to get current downloads from {source.Name()}");
                return;
            }
            SearchForAppropriateDownloads(downloading, DownloadApp.qBitTorrent, settings);
        }
    }
}
