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
    using System.IO;
    using System.Windows.Forms;

    public class ActionRSS : Item, Action, EpisodeRelated, ScanListItem
    {
        public RSSItem RSS;
        public string TheFileNoExt;

        public ActionRSS(RSSItem rss, string toWhereNoExt, ProcessedEpisode pe)
        {
            this.Episode = pe;
            this.RSS = rss;
            this.TheFileNoExt = toWhereNoExt;
        }

        public bool SameAs(Item o)
        {
            return (o is ActionRSS) && ((o as ActionRSS).RSS == this.RSS);
        }
        public bool Done { get; private set; }
        public bool Error { get; private set; }
        public string ErrorText { get; private set; }
        public string ProgressText { get { return this.RSS.Title; } }
        public int PercentDone { get { return Done ? 100 : 0; } }
        public long SizeOfWork { get { return 1; } }

        public bool Go(TVSettings settings)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            try
            {
                byte[] r = wc.DownloadData(this.RSS.URL);
                if ((r == null) || (r.Length == 0))
                {
                    this.Error = true;
                    this.ErrorText = "No data downloaded";
                }

                string saveTemp = Path.GetTempPath() + System.IO.Path.DirectorySeparatorChar + settings.FilenameFriendly(this.RSS.Title);
                if (new FileInfo(saveTemp).Extension.ToLower() != "torrent")
                    saveTemp += ".torrent";
                File.WriteAllBytes(saveTemp, r);

                System.Diagnostics.Process.Start(settings.uTorrentPath, "/directory \"" + (new FileInfo(this.TheFileNoExt).Directory.FullName) + "\" \"" + saveTemp + "\"");

                this.Error = false;
            }
            catch (Exception e)
            {
                this.ErrorText = e.Message;
                this.Error = true;
            }
            this.Done = true;

            return !this.Error;
        }

        public bool Stop()
        {
            return false;
        }

        public ProcessedEpisode Episode { get; private set; }
        public IgnoreItem Ignore
        {
            get
            {
                if (string.IsNullOrEmpty(this.TheFileNoExt))
                    return null;
                return new IgnoreItem(this.TheFileNoExt);
            }
        }
        public ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem {
                                                        Text = this.Episode.SI.ShowName()
                                                    };

                lvi.SubItems.Add(this.Episode.SeasonNumber.ToString());
                lvi.SubItems.Add(this.Episode.NumsAsString());
                DateTime? dt = this.Episode.GetAirDateDT(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                    lvi.SubItems.Add(dt.Value.ToShortDateString());
                else
                    lvi.SubItems.Add("");

                lvi.SubItems.Add(this.TheFileNoExt);
                lvi.SubItems.Add(this.RSS.Title);

                lvi.Tag = this;

                return lvi;
            }
        }
        string ScanListItem.TargetFolder
        {
            get
            {
                if (string.IsNullOrEmpty(this.TheFileNoExt))
                    return null;
                return new FileInfo(this.TheFileNoExt).DirectoryName;
            }
        }
        public int ScanListViewGroup { get { return 4; } }
        int ScanListItem.IconNumber { get { return 6; } }
        public int Compare(Item o)
        {
            ActionRSS rss = o as ActionRSS;
            return rss == null ? 0 : this.RSS.URL.CompareTo(rss.RSS.URL);
        }
    }
}