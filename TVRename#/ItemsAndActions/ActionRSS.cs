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
    public class ActionRss : ITem, IAction, IScanListItem
    {
        public RssItem Rss;
        public string TheFileNoExt;

        public ActionRss(RssItem rss, string toWhereNoExt, ProcessedEpisode pe)
        {
            Episode = pe;
            Rss = rss;
            TheFileNoExt = toWhereNoExt;
        }

        #region Action Members

        public bool Done { get; private set; }
        public bool Error { get; private set; }
        public string ErrorText { get; private set; }

        public string ProgressText
        {
            get { return Rss.Title; }
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

        public string Produces
        {
            get { return Rss.Url; }
        }

        public bool Go( ref bool pause, TVRenameStats stats)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            try
            {
                byte[] r = wc.DownloadData(Rss.Url);
                if ((r == null) || (r.Length == 0))
                {
                    Error = true;
                    ErrorText = "No data downloaded";
                    Done = true;
                    return false;
                }

                string saveTemp = Path.GetTempPath() + System.IO.Path.DirectorySeparatorChar + TVSettings.Instance.FilenameFriendly(Rss.Title);
                if (new FileInfo(saveTemp).Extension.ToLower() != "torrent")
                    saveTemp += ".torrent";
                File.WriteAllBytes(saveTemp, r);

                System.Diagnostics.Process.Start(TVSettings.Instance.UTorrentPath, "/directory \"" + (new FileInfo(TheFileNoExt).Directory.FullName) + "\" \"" + saveTemp + "\"");

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

        public bool SameAs(ITem o)
        {
            return (o is ActionRss) && ((o as ActionRss).Rss == Rss);
        }

        public int Compare(ITem o)
        {
            ActionRss rss = o as ActionRss;
            return rss == null ? 0 : Rss.Url.CompareTo(rss.Rss.Url);
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
                                                        Text = Episode.Si.ShowName
                                                    };

                lvi.SubItems.Add(Episode.SeasonNumber.ToString());
                lvi.SubItems.Add(Episode.NumsAsString());
                DateTime? dt = Episode.GetAirDateDt(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                    lvi.SubItems.Add(dt.Value.ToShortDateString());
                else
                    lvi.SubItems.Add("");

                lvi.SubItems.Add(TheFileNoExt);
                lvi.SubItems.Add(Rss.Title);

                lvi.Tag = this;

                return lvi;
            }
        }

        string IScanListItem.TargetFolder
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

        int IScanListItem.IconNumber
        {
            get { return 6; }
        }

        #endregion
    }
}
