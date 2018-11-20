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
        private ProcessedEpisode pe;
        private readonly int sizeBytes;

        public ActionTDownload(string name, int sizeBytes, string url, string toWhereNoExt, ProcessedEpisode pe, ItemMissing me)
        {
            Episode = pe;
            this.sizeBytes = sizeBytes;
            SourceName = name;
            this.url = url;
            theFileNoExt = toWhereNoExt;
            UndoItemMissing = me;

            //append file size
            if (sizeBytes < 0) SourceName = " (N/A)";
            else SourceName += " (" + SizeSuffix(sizeBytes) + ")";
        }

        public ActionTDownload(RSSItem rss, string theFileNoExt, ProcessedEpisode pe, ItemMissing me)
        {
            SourceName = rss.Title;
            url = rss.URL;
            this.theFileNoExt = theFileNoExt;
            this.pe = pe;
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
            string saveTemp = Path.GetTempPath() + System.IO.Path.DirectorySeparatorChar + TVSettings.Instance.FilenameFriendly(name);
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
        protected override string SourceDetails => SourceName;

        public override string TargetFolder => string.IsNullOrEmpty(theFileNoExt) ? null : new FileInfo(theFileNoExt).DirectoryName;

        private string TargetFilename => string.IsNullOrEmpty(theFileNoExt) ? null : new FileInfo(theFileNoExt).Name;

        public override string ScanListViewGroup => "lvgActionDownloadRSS";

        public override int IconNumber => 6;

        #endregion


        //Source: https://stackoverflow.com/a/14488941/4471649
        private static readonly string[] SizeSuffixes =
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        private static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }
    }
}
