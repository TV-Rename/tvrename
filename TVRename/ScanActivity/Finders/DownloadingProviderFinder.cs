using System.Collections.Generic;
using System.Threading.Tasks;

namespace TVRename;

internal abstract class DownloadingProviderFinder(TVDoc doc, IDownloadProvider source, TVDoc.ScanSettings settings) : DownloadingFinder(doc, settings)
{
    private readonly IDownloadProvider source = source;

    protected override string CheckName() => $"Looked in {source.Name()} for the missing files to see if they are being downloaded";

    protected override async Task DoCheckAsync(SetProgressDelegate progress)
    {
        List<TorrentEntry>? downloading = await source.GetTorrentDownloadsAsync();
        if (downloading is null)
        {
            LOGGER.Warn($"Failed to get current downloads from {source.Name()}");
            return;
        }

        SearchForAppropriateDownloads(downloading, source.Application);
    }
}
