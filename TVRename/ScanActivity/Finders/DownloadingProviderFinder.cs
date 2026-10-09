//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

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
