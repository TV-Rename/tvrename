//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;
using System;

namespace TVRename;

public class ItemDownloading : Item
{
    private readonly IDownloadInformation entry;
    private readonly string desiredLocationNoExt;

    public override IgnoreItem? Ignore => GenerateIgnore(desiredLocationNoExt);
    public override string ScanListViewGroup => "lvgDownloading";

    public override string? DestinationFolder => TargetFolder;
    public override string? DestinationFile => entry.FileIdentifier;
    public override string SourceDetails => entry.RemainingText ?? string.Empty;

    private readonly ShowConfiguration? internalShow;
    private readonly int? internalSeasonNumber;

    public override int IconNumber { get; }
    public override string? TargetFolder => string.IsNullOrEmpty(entry.Destination) ? null : new FileInfo(entry.Destination).DirectoryName;

    private ItemDownloading(IDownloadInformation dl, string desiredLocationNoExt, DownloadingFinder.DownloadApp tApp, ItemMissing undoItem)
    {
        this.desiredLocationNoExt = desiredLocationNoExt;
        entry = dl;
        UndoItemMissing = undoItem;
        IconNumber = tApp switch
        {
            DownloadingFinder.DownloadApp.uTorrent => 2,
            DownloadingFinder.DownloadApp.SABnzbd => 8,
            DownloadingFinder.DownloadApp.qBitTorrent => 10,
            _ => 0
        };
    }

    public ItemDownloading(IDownloadInformation dl, ProcessedEpisode pe, string desiredLocationNoExt, DownloadingFinder.DownloadApp tApp, ItemMissing me) : this(dl, desiredLocationNoExt, tApp, me)
    {
        Episode = pe;
    }

    public ItemDownloading(IDownloadInformation dl, MovieConfiguration mc, string desiredLocationNoExt, DownloadingFinder.DownloadApp tApp, ItemMissing me) : this(dl, desiredLocationNoExt, tApp, me)
    {
        Episode = null;
        Movie = mc;
    }

    public ItemDownloading(IDownloadInformation dl, ShowConfiguration series, int? seasonNumberAsInt, string desiredLocationNoExt, DownloadingFinder.DownloadApp tApp, ItemMissing me)
        : this(dl, desiredLocationNoExt, tApp, me)
    {
        Episode=null;
        Movie = null;
        internalShow = series;
        internalSeasonNumber = seasonNumberAsInt;
    }

    public override string SeriesName => internalShow?.Name ?? base.SeriesName;
    public override string SeasonNumber => internalSeasonNumber is null
        ? base.SeasonNumber
        : TVSettings.SeasonNameFor(internalSeasonNumber.Value);
    public override int? SeasonNumberAsInt => internalSeasonNumber ?? base.SeasonNumberAsInt;
    public override ShowConfiguration? Series => internalShow ?? base.Series;

    #region Item Members

    public override bool SameAs(Item o) => o is ItemDownloading torrent && entry == torrent.entry;

    public override string Name => "Already Downloading";

    public override bool CheckedItem { get => false; set { } }

    public override int CompareTo(Item? o)
    {
        if (o is not ItemDownloading ut)
        {
            return -1;
        }

        return string.Compare(DestinationFile, ut.DestinationFile, StringComparison.Ordinal);
    }

    #endregion Item Members
}
