// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
namespace TVRename
{
    using System;
    using Alphaleonis.Win32.Filesystem;
    using System.Windows.Forms;

    // MS_TODO: derive this from ActionDownload?
    public class ActionRSS : Item, Action, ScanListItem
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

        public bool Done { get; private set; }
        public bool Error { get; private set; }
        public string ErrorText { get; private set; }

        public string ProgressText
        {
            get { return RSS.Title; }
        }

        public double PercentDone
        {
            get { return Done ? 100 : 0; }
        }

        public string Name
        {
            get { return "Get Torrent"; }
        }

        public long SizeOfWork
        {
            get { return 1000000; }
        }

        public string produces
        {
            get { return RSS.URL; }
        }

        public bool Go( ref bool pause, TVRenameStats stats)
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

        public bool SameAs(Item o)
        {
            return (o is ActionRSS) && ((o as ActionRSS).RSS == RSS);
        }

        public int Compare(Item o)
        {
            ActionRSS rss = o as ActionRSS;
            return rss == null ? 0 : RSS.URL.CompareTo(rss.RSS.URL);
        }

        #endregion

        #region ScanListItem Members

        public ProcessedEpisode Episode { get; private set; }

        public IgnoreItem Ignore
        {
            get
            {
                if (string.IsNullOrEmpty(TheFileNoExt))
                    return null;
                return new IgnoreItem(TheFileNoExt);
            }
        }

        public ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem {
                                                        Text = Episode.SI.ShowName
                                                    };

                lvi.SubItems.Add(Episode.SeasonNumber.ToString());
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

        string ScanListItem.TargetFolder
        {
            get
            {
                if (string.IsNullOrEmpty(TheFileNoExt))
                    return null;
                return new FileInfo(TheFileNoExt).DirectoryName;
            }
        }

        public string ScanListViewGroup
        {
            get { return "lvgActionDownloadRSS"; }
        }

        int ScanListItem.IconNumber
        {
            get { return 6; }
        }

        #endregion
    }
}
