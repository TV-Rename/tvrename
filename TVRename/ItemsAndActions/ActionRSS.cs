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
    public class ActionRSS : ActionDownload
    {
        // ReSharper disable once InconsistentNaming
        public RSSItem RSS;
        private readonly string theFileNoExt;

        public ActionRSS(RSSItem rss, string toWhereNoExt, ProcessedEpisode pe)
        {
            Episode = pe;
            RSS = rss;
            theFileNoExt = toWhereNoExt;
        }

        #region Action Members

        public override string ProgressText => RSS.Title;
        public override string Name => "Get Torrent";
        public override long SizeOfWork => 1000000;
        public override string Produces => RSS.URL;

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
                    byte[] r = new System.Net.WebClient().DownloadData(RSS.URL);
                    if ((r == null) || (r.Length == 0))
                    {
                        Error = true;
                        ErrorText = "No data downloaded";
                        Done = true;
                        return false;
                    }

                    string saveTemp = SaveDownloadedData(r, RSS.Title);
                    uTorrentFinder.StartTorrentDownload(saveTemp, theFileNoExt);
                }
                
                if (TVSettings.Instance.CheckqBitTorrent)
                    qBitTorrentFinder.StartTorrentDownload(RSS.URL);

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
            return (o is ActionRSS rss) && (rss.RSS == RSS);
        }

        public override int Compare(Item o)
        {
            ActionRSS rss = o as ActionRSS;
            return rss == null ? 0 : string.Compare(RSS.URL, rss.RSS.URL, StringComparison.Ordinal);
        }

        #endregion

        #region Item Members

        public override IgnoreItem Ignore => GenerateIgnore(theFileNoExt);

        protected override string SeriesName => Episode.Show.ShowName;
        protected override string SeasonNumber => Episode.AppropriateSeasonNumber.ToString();
        protected override string EpisodeNumber => Episode.NumsAsString();
        protected override string AirDate => Episode.GetAirDateDT(true).PrettyPrint();
        protected override string DestinationFolder => TargetFolder;
        protected override string DestinationFile => theFileNoExt;
        protected override string SourceDetails => RSS.Title;

        public override string TargetFolder
        {
            get
            {
                if (string.IsNullOrEmpty(theFileNoExt))
                    return null;
                return new FileInfo(theFileNoExt).DirectoryName;
            }
        }

        public override string ScanListViewGroup => "lvgActionDownloadRSS";

        public override int IconNumber => 6;

        #endregion
    }
}
