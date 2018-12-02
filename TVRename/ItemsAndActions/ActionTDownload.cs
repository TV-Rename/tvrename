// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
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

        public ActionTDownload(RSSItem rss, string theFileNoExt, ProcessedEpisode pe, ItemMissing me)
        {
            SourceName = rss.Title;
            url = rss.URL;
            this.theFileNoExt = theFileNoExt;
            Episode = pe;
            UndoItemMissing = me;
        }

        #region Action Members

        public override string ProgressText => SourceName;
        public override string Name => "Get Torrent";
        public override long SizeOfWork => 1000000;
        public override string Produces => url;

        public override bool Go( ref bool pause, TVRenameStats stats)
        {
            try
            {
                if (!(TVSettings.Instance.CheckuTorrent || TVSettings.Instance.CheckqBitTorrent))
                {
                    Error = true;
                    ErrorText = "No torrent clients enabled to download RSS";
                    Done = true;
                    return false;
                }

                if (TVSettings.Instance.CheckuTorrent)
                {
                    byte[] r = new System.Net.WebClient().DownloadData(url);
                    if ((r == null) || (r.Length == 0))
                    {
                        Error = true;
                        ErrorText = "No data downloaded";
                        Done = true;
                        return false;
                    }

                    string saveTemp = SaveDownloadedData(r, SourceName);
                    uTorrentFinder.StartTorrentDownload(saveTemp, theFileNoExt);
                }
                
                if (TVSettings.Instance.CheckqBitTorrent)
                    qBitTorrentFinder.StartTorrentDownload(url);

                Done = true;
                return true;
            }
            catch (Exception e)
            {
                ErrorText = e.Message;
                Error = true;
                Done = true;
                return false;
            }
        }

        private static string SaveDownloadedData(byte[] r,string name)
        {
            string saveTemp = Path.GetTempPath() + Path.DirectorySeparatorChar + TVSettings.Instance.FilenameFriendly(name);
            if (new FileInfo(saveTemp).Extension.ToLower() != "torrent")
                saveTemp += ".torrent";
            File.WriteAllBytes(saveTemp, r);
            return saveTemp;
        }

        #endregion

        #region Item Members

        public override bool SameAs(Item o)
        {
            return (o is ActionTDownload rss) && (rss.url == url);
        }

        public override int Compare(Item o)
        {
            return !(o is ActionTDownload rss) ? 0 : string.Compare(url, rss.url, StringComparison.Ordinal);
        }

        #endregion

        #region Item Members

        public override IgnoreItem Ignore => GenerateIgnore(theFileNoExt);

        protected override string DestinationFolder => TargetFolder;
        protected override string DestinationFile => TargetFilename;
        protected override string SourceDetails => $"{SourceName} ({(sizeBytes < 0 ? "N/A" : sizeBytes.GBMB())})";

        public override string TargetFolder => string.IsNullOrEmpty(theFileNoExt) ? null : new FileInfo(theFileNoExt).DirectoryName;

        private string TargetFilename => string.IsNullOrEmpty(theFileNoExt) ? null : new FileInfo(theFileNoExt).Name;

        public override string ScanListViewGroup => "lvgActionDownloadRSS";

        public override int IconNumber => 6;

        #endregion
    }
}
