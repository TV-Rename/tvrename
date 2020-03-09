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
        private readonly long sizeBytes;

        public ActionTDownload(string name, long sizeBytes, string url, string toWhereNoExt, ProcessedEpisode pe,ItemMissing me)
        {
            Episode = pe;
            SourceName = name;
            this.url = url;
            theFileNoExt = toWhereNoExt;
            UndoItemMissing = me;
            this.sizeBytes = sizeBytes;
        }

        public ActionTDownload([NotNull] RSSItem rss, string theFileNoExt, ProcessedEpisode pe, ItemMissing me)
        {
            SourceName = rss.Title;
            url = rss.URL;
            this.theFileNoExt = theFileNoExt;
            Episode = pe;
            UndoItemMissing = me;
        }

        #region Action Members

        public override string ProgressText => SourceName;
        [NotNull]
        public override string Name => "Get Torrent";
        public override long SizeOfWork => 1000000;
        public override string Produces => url;

        [NotNull]
        public override ActionOutcome Go( TVRenameStats stats)
        {
            try
            {
                if (!(TVSettings.Instance.CheckuTorrent || TVSettings.Instance.CheckqBitTorrent))
                {
                    return new ActionOutcome("No torrent clients enabled to download RSS");
                }

                if (!TVSettings.Instance.qBitTorrentDownloadFilesFirst && TVSettings.Instance.CheckqBitTorrent)
                {
                    qBitTorrentFinder.StartTorrentDownload(url, null, false);
                    return ActionOutcome.Success();
                }

                byte[] r = HttpHelper.GetUrlBytes(url,true);

                if (r is null || r.Length == 0)
                {
                    return new ActionOutcome($"No data downloaded from {url}");
                }

                string saveTemp = SaveDownloadedData(r, SourceName);

                if (TVSettings.Instance.CheckuTorrent)
                {
                    uTorrentFinder.StartTorrentDownload(saveTemp, theFileNoExt);
                }
                
                if (TVSettings.Instance.CheckqBitTorrent)
                {
                    qBitTorrentFinder.StartTorrentDownload(url,saveTemp, TVSettings.Instance.qBitTorrentDownloadFilesFirst);
                }

                return ActionOutcome.Success();
            }
            catch (Exception e)
            {
                return new ActionOutcome(e);
            }
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

        public override int Compare(Item o)
        {
            return !(o is ActionTDownload rss) ? 0 : string.Compare(url, rss.url, StringComparison.Ordinal);
        }

        #endregion

        #region Item Members

        [CanBeNull]
        public override IgnoreItem Ignore => GenerateIgnore(theFileNoExt);

        [CanBeNull]
        protected override string DestinationFolder => TargetFolder;
        [CanBeNull]
        protected override string DestinationFile => TargetFilename;

        protected override string SourceDetails => $"{SourceName} ({(sizeBytes < 0 ? "N/A" : sizeBytes.GBMB())})";

        [CanBeNull]
        public override string TargetFolder => string.IsNullOrEmpty(theFileNoExt) ? null : new FileInfo(theFileNoExt).DirectoryName;

        [CanBeNull]
        private string TargetFilename => string.IsNullOrEmpty(theFileNoExt) ? null : new FileInfo(theFileNoExt).Name;

        [NotNull]
        public override string ScanListViewGroup => "lvgActionDownloadRSS";

        public override int IconNumber => 6;

        #endregion
    }
}
