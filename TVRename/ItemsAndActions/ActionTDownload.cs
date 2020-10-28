// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using JetBrains.Annotations;

namespace TVRename
{
    using System;
    using Alphaleonis.Win32.Filesystem;

    // ReSharper disable once InconsistentNaming
    public class ActionTDownload : ActionDownload
    {
        // ReSharper disable once InconsistentNaming
        private readonly string theFileNoExt;
        public readonly string SourceName;
        private readonly string url;
        public readonly long sizeBytes;
        public readonly int Seeders;
        // ReSharper disable once NotAccessedField.Global - Used as a property in the Choose Download Grid
        public readonly string UpstreamSource;

        public ActionTDownload(string name, long sizeBytes,int seeders, string url, string toWhereNoExt, ProcessedEpisode? pe,ItemMissing me, string upstreamSource)
        {
            Episode = pe;
            SourceName = name;
            this.url = url;
            theFileNoExt = toWhereNoExt;
            UpstreamSource = upstreamSource;
            UndoItemMissing = me;
            this.sizeBytes = sizeBytes;
            Seeders = seeders;
        }

        public ActionTDownload([NotNull] RSSItem rss, string theFileNoExt, ItemMissing me)
        {
            SourceName = rss.Title;
            url = rss.URL;
            this.theFileNoExt = theFileNoExt;
            UpstreamSource = rss.UpstreamSource;
            Episode = me.Episode;
            Movie = me.Movie;
            UndoItemMissing = me;
            Seeders = rss.Seeders;
            sizeBytes = rss.Bytes;
            UpstreamSource = rss.UpstreamSource;
        }

        #region Action Members

        public override string ProgressText => SourceName;
        public override string Name => "Get Torrent";
        public override long SizeOfWork => 1000000;
        public override string Produces => url;

        public override ActionOutcome Go( TVRenameStats stats)
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
                catch (DownloadFailedException e)
                {
                    //Don't worry about this error, we'll retry below
                }

                if (Helpers.OpenUrl(url))
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
            FileInfo downloadedFile = new FileInfo(theFileNoExt + saveTemp);
            return downloadedFile;
        }

        private class DownloadFailedException : Exception
        {
        }

        [NotNull]
        private static string SaveDownloadedData(byte[] r,string name)
        {
            string saveTemp = Path.GetTempPath().EnsureEndsWithSeparator() + TVSettings.Instance.FilenameFriendly(name);
            if (new FileInfo(saveTemp).Extension.ToLower() != "torrent")
            {
                saveTemp += ".torrent";
            }

            File.WriteAllBytes(saveTemp, r);
            return saveTemp;
        }

        #endregion

        #region Item Members

        public override bool SameAs(Item o) => o is ActionTDownload rss && rss.url == url;

        public override int CompareTo(object o)
        {
            return !(o is ActionTDownload rss) ? -1 : string.Compare(url, rss.url, StringComparison.Ordinal);
        }

        public override IgnoreItem? Ignore => GenerateIgnore(theFileNoExt);

        public override string? DestinationFolder => TargetFolder;
        public override string? DestinationFile => TargetFilename;

        public override string SourceDetails => $"{SourceName} ({(sizeBytes < 0 ? "N/A" : sizeBytes.GBMB())}) [{Seeders} Seeds]";

        [NotNull]
        public string SizePretty => $"{(sizeBytes < 0 ? "N/A" : sizeBytes.GBMB())}";

        public override string? TargetFolder => string.IsNullOrEmpty(theFileNoExt) ? null : new FileInfo(theFileNoExt).DirectoryName;

        private string? TargetFilename => string.IsNullOrEmpty(theFileNoExt) ? null : new FileInfo(theFileNoExt).Name;

        public override string ScanListViewGroup => "lvgActionDownloadRSS";

        public override int IconNumber => 6;

        #endregion
    }
}
