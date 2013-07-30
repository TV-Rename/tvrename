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
    using System.Windows.Forms;
    using System.IO;
    using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

    public class ActionDownload : Item, Action, ScanListItem
    {
        private readonly string BannerPath;
        private readonly FileInfo Destination;
        private readonly ShowItem SI;

        public ActionDownload(ShowItem si, ProcessedEpisode pe, FileInfo dest, string bannerPath)
        {
            this.Episode = pe;
            this.SI = si;
            this.Destination = dest;
            this.BannerPath = bannerPath;
        }

        #region Action Members

        public bool Done { get; set; }
        public bool Error { get; set; }
        public string ErrorText { get; set; }

        public string Name
        {
            get { return "Download"; }
        }

        public string ProgressText
        {
            get { return this.Destination.Name; }
        }

        public double PercentDone
        {
            get { return this.Done ? 100 : 0; }
        }

        // 0 to 100
        public long SizeOfWork
        {
            get { return 1000000; }
        }

        public bool Go(TVSettings settings, ref bool pause, TVRenameStats stats)
        {
            byte[] theData = this.SI.TVDB.GetPage(this.BannerPath, false, typeMaskBits.tmBanner, false);
            if ((theData == null) || (theData.Length == 0))
            {
                this.ErrorText = "Unable to download " + this.BannerPath;
                this.Error = true;
                this.Done = true;
                return false;
            }

            try
            {
                FileStream fs = new FileStream(this.Destination.FullName, FileMode.Create);
                fs.Write(theData, 0, theData.Length);
                fs.Close();
            }
            catch (Exception e)
            {
                this.ErrorText = e.Message;
                this.Error = true;
                this.Done = true;
                return false;
            }
                

            this.Done = true;
            return true;
        }

        #endregion

        #region Item Members

        public bool SameAs(Item o)
        {
            return (o is ActionDownload) && ((o as ActionDownload).Destination == this.Destination);
        }

        public int Compare(Item o)
        {
            ActionDownload dl = o as ActionDownload;
            return dl == null ? 0 : this.Destination.FullName.CompareTo(dl.Destination.FullName);
        }

        #endregion

        #region ScanListItem Members

        public int IconNumber
        {
            get { return 5; }
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
                                                        Text = (this.Episode != null) ? this.Episode.SI.ShowName : ((this.SI != null) ? this.SI.ShowName : "")
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

        public int ScanListViewGroup
        {
            get { return 5; }
        }

        public string TargetFolder
        {
            get
            {
                if (this.Destination == null)
                    return null;
                return this.Destination.DirectoryName;
            }
        }

        #endregion
    }
}