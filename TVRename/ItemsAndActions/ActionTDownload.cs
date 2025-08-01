// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Threading;

namespace TVRename;

using Alphaleonis.Win32.Filesystem;
using System;

// ReSharper disable once InconsistentNaming
public class ActionTDownload : ActionDownload
{
    // ReSharper disable once InconsistentNaming
    private readonly string theFileNoExt;

    public readonly string SourceName;
    private readonly string url;
    public readonly long sizeBytes;
    public readonly int? Seeders;
    public ItemList AlsoAvailable = [];

    // ReSharper disable once NotAccessedField.Global - Used as a property in the Choose Download Grid
    public readonly string UpstreamSource;

    private readonly ItemDownloading becomes;
    public override QueueName Queue() => QueueName.download;
    public ActionTDownload(string name, long sizeBytes, int? seeders, string url, string toWhereNoExt, ProcessedEpisode? pe, ItemMissing me, string upstreamSource, ItemDownloading becomes)
    {
        Episode = pe;
        SourceName = name;
        this.url = url;
        theFileNoExt = toWhereNoExt;
        UpstreamSource = upstreamSource;
        UndoItemMissing = me;
        this.sizeBytes = sizeBytes;
        Seeders = seeders;
        this.becomes = becomes;
    }

    public ActionTDownload(RSSItem rss, ItemMissing me, ItemDownloading becomes)
    {
        SourceName = rss.Title;
        url = rss.URL;
        theFileNoExt = me.TheFileNoExt;
        UpstreamSource = rss.UpstreamSource;
        Episode = me.Episode;
        Movie = me.Movie;
        UndoItemMissing = me;
        Seeders = rss.Seeders;
        sizeBytes = rss.Bytes;
        UpstreamSource = rss.UpstreamSource;
        this.becomes = becomes;
    }

    #region Action Members

    public override string ProgressText => SourceName;
    public override string Name => "Get Torrent";
    public override long SizeOfWork => 1000000;
    public override string Produces => url;

    public override Item Becomes() => becomes;

    public override ActionOutcome Go(TVRenameStats stats, CancellationToken cancellationToken)
    {
        bool isDownloadable = url.IsWebLink();
        try
        {
            try
            {
                if (TVSettings.Instance.CheckuTorrent && isDownloadable)
                {
                    FileInfo downloadedFile = DownloadFile();
                    new uTorrent().StartTorrentDownload(downloadedFile);
                    return ActionOutcome.Success();
                }

                if (TVSettings.Instance.CheckqBitTorrent)
                {
                    if (isDownloadable && TVSettings.Instance.qBitTorrentDownloadFilesFirst)
                    {
                        FileInfo downloadedFile = DownloadFile();
                        new qBitTorrent().StartTorrentDownload(downloadedFile);
                        return ActionOutcome.Success();
                    }
                    else
                    {
                        new qBitTorrent().StartUrlDownload(url);
                        return ActionOutcome.Success();
                    }
                }
            }
            catch (DownloadFailedException)
            {
                //Don't worry about this error, we'll retry below
            }

            if (url.OpenUrlInBrowser())
            {
                return ActionOutcome.Success();
            }

            return new ActionOutcome("No torrent clients enabled to download RSS - Tried to use system open, but failed");
        }
        catch (Exception e)
        {
            return new ActionOutcome(e);
        }
    }

    private FileInfo DownloadFile()
    {
        byte[] r = HttpHelper.GetUrlBytes(url, true);

        if (r.Length == 0)
        {
            throw new DownloadFailedException();
        }

        string saveTemp = SaveDownloadedData(r, SourceName);
        FileInfo downloadedFile = new(saveTemp);
        return downloadedFile;
    }

    private class DownloadFailedException : Exception
    {
    }

    private static string SaveDownloadedData(byte[] r, string name)
    {
        string saveTemp = Path.GetTempPath().EnsureEndsWithSeparator() + TVSettings.Instance.FilenameFriendly(name);
        if (new FileInfo(saveTemp).Extension.ToLower() != "torrent")
        {
            saveTemp += ".torrent";
        }

        File.WriteAllBytes(saveTemp, r);
        return saveTemp;
    }

    #endregion Action Members

    #region Item Members

    public override bool SameAs(Item o) => o is ActionTDownload rss && rss.url == url;

    public override int CompareTo(Item? o)
    {
        return o is not ActionTDownload rss ? -1 : string.Compare(url, rss.url, StringComparison.Ordinal);
    }

    public override IgnoreItem? Ignore => GenerateIgnore(theFileNoExt);

    public override string? DestinationFolder => TargetFolder;
    public override string? DestinationFile => TargetFilename;

    public override string SourceDetails => $"{SourceName} ({(sizeBytes < 0 ? "N/A" : sizeBytes.GBMB())}) [{Seeders} Seeds]";

    public string SizePretty => $"{(sizeBytes < 0 ? "N/A" : sizeBytes.GBMB())}";

    public override string? TargetFolder => string.IsNullOrEmpty(theFileNoExt) ? null : new FileInfo(theFileNoExt).DirectoryName;

    private string? TargetFilename => string.IsNullOrEmpty(theFileNoExt) ? null : new FileInfo(theFileNoExt).Name;

    public override string ScanListViewGroup => "lvgActionDownloadRSS";

    public override int IconNumber => 6;

    #endregion Item Members
}
