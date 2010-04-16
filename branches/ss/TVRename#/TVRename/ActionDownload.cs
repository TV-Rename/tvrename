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

    public class ActionDownload : Item, Action, EpisodeRelated, ScanListItem
    {
        readonly string BannerPath;
        readonly FileInfo Destination;
        readonly ShowItem SI;

        public bool Done { get; set; }
        public bool Error { get; set; }
        public string ErrorText { get; set; }
        public int IconNumber { get { return 5; } }
        public string ProgressText
        {
            get { return this.Destination.Name; }
        }
        public ProcessedEpisode Episode { get; set; }
        public IgnoreItem Ignore
        {
            get
            {
                if (this.Destination == null)
                    return null;
                return new IgnoreItem(this.Destination.FullName);
            }
        }
        public ListViewItem ScanListViewItem 
        { 
            get
            {
                ListViewItem lvi = new ListViewItem {
                                                        Text = this.Episode.SI.ShowName()
                                                    };

                lvi.SubItems.Add(this.Episode != null ? this.Episode.SeasonNumber.ToString() : "");
                lvi.SubItems.Add(this.Episode != null ? this.Episode.NumsAsString() : "");

                if (this.Episode != null)
                {
                    DateTime? dt = this.Episode.GetAirDateDT(true);
                    if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                        lvi.SubItems.Add(dt.Value.ToShortDateString());
                    else
                        lvi.SubItems.Add("");
                }
                else
                    lvi.SubItems.Add("");

                lvi.SubItems.Add(this.Destination.DirectoryName);
                lvi.SubItems.Add(this.BannerPath);

                if (string.IsNullOrEmpty(this.BannerPath))
                    lvi.BackColor = Helpers.WarningColor();

                lvi.SubItems.Add(this.Destination.Name);

                lvi.Tag = this;

                return lvi;
            }
        }
        public int ScanListViewGroup { get { return 5; } }
        public string TargetFolder
        {
            get
            {
                if (this.Destination == null)
                    return null;
                return this.Destination.DirectoryName;
            }
        }

        public int PercentDone { get { return Done ? 100 : 0; } } // 0 to 100
        public long SizeOfWork { get { return 1; } } // for file copy/move, number of bytes in file.  for simple tasks, 1.
        public bool SameAs(Item o)
        {
            return (o is ActionDownload) && ((o as ActionDownload).Destination == this.Destination);
        }
        public bool Go(TVSettings settings)
        {
            byte[] theData = this.SI.TVDB.GetPage(this.BannerPath, false, typeMaskBits.tmBanner, false);
            if (theData == null)
            {
                this.ErrorText = "Unable to download " + this.BannerPath;
                this.Error = true;
                return false;
            }

            FileStream fs = new FileStream(this.Destination.FullName, FileMode.Create);
            fs.Write(theData, 0, theData.Length);
            fs.Close();

            this.Done = true;
            return true;
        }
        public bool Stop()
        {
            return false;
        }

        public ActionDownload(ShowItem si, ProcessedEpisode pe, FileInfo dest, string bannerPath)
        {
            this.Episode = pe;
            this.SI = si;
            this.Destination = dest;
            this.BannerPath = bannerPath;
        }
        public int Compare(Item o)
        {
            ActionDownload dl = o as ActionDownload;
            return dl == null ? 0 : this.Destination.FullName.CompareTo(dl.Destination.FullName);
        }
    }
}