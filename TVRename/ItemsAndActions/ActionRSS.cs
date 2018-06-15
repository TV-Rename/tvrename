// 
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
    using System.Windows.Forms;

    public class ActionRSS : ActionDownload
    {
        public RSSItem RSS;
        public string TheFileNoExt;

        public ActionRSS(RSSItem rss, string toWhereNoExt, ProcessedEpisode pe)
        {
            Episode = pe;
            RSS = rss;
            TheFileNoExt = toWhereNoExt;
        }

        #region Action Members

        public override  string ProgressText => RSS.Title;


        public override string Name => "Get Torrent";

        public override long SizeOfWork => 1000000;

        public override string Produces => RSS.URL;

        public override bool Go( ref bool pause, TVRenameStats stats)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            try
            {
                byte[] r = wc.DownloadData(RSS.URL);
                if ((r == null) || (r.Length == 0))
                {
                    Error = true;
                    ErrorText = "No data downloaded";
                    Done = true;
                    return false;
                }

                string saveTemp = Path.GetTempPath() + System.IO.Path.DirectorySeparatorChar + TVSettings.Instance.FilenameFriendly(RSS.Title);
                if (new FileInfo(saveTemp).Extension.ToLower() != "torrent")
                    saveTemp += ".torrent";
                File.WriteAllBytes(saveTemp, r);

                System.Diagnostics.Process.Start(TVSettings.Instance.uTorrentPath, "/directory \"" + (new FileInfo(TheFileNoExt).Directory.FullName) + "\" \"" + saveTemp + "\"");

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

        #endregion

        #region Item Members

        public override bool SameAs(Item o)
        {
            return (o is ActionRSS) && ((o as ActionRSS).RSS == RSS);
        }

        public override int Compare(Item o)
        {
            ActionRSS rss = o as ActionRSS;
            return rss == null ? 0 : RSS.URL.CompareTo(rss.RSS.URL);
        }

        #endregion

        #region Item Members

        public override IgnoreItem Ignore
        {
            get
            {
                if (string.IsNullOrEmpty(TheFileNoExt))
                    return null;
                return new IgnoreItem(TheFileNoExt);
            }
        }

        public override ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem {
                                                        Text = Episode.SI.ShowName
                                                    };

                lvi.SubItems.Add(Episode.AppropriateSeasonNumber.ToString());
                lvi.SubItems.Add(Episode.NumsAsString());
                DateTime? dt = Episode.GetAirDateDT(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                    lvi.SubItems.Add(dt.Value.ToShortDateString());
                else
                    lvi.SubItems.Add("");

                lvi.SubItems.Add(TheFileNoExt);
                lvi.SubItems.Add(RSS.Title);

                lvi.Tag = this;

                return lvi;
            }
        }

        public override string TargetFolder
        {
            get
            {
                if (string.IsNullOrEmpty(TheFileNoExt))
                    return null;
                return new FileInfo(TheFileNoExt).DirectoryName;
            }
        }

public override string ScanListViewGroup => "lvgActionDownloadRSS";

        public override int IconNumber => 6;

        #endregion
    }
}
