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

    public class ItemuTorrenting : Item, ScanListItem
    {
        public string DesiredLocationNoExt;
        public TorrentEntry Entry;

        public ItemuTorrenting(TorrentEntry te, ProcessedEpisode pe, string desiredLocationNoExt)
        {
            this.Episode = pe;
            this.DesiredLocationNoExt = desiredLocationNoExt;
            this.Entry = te;
        }
        public string TargetFolder
        {
            get
            {
                if (string.IsNullOrEmpty(this.Entry.DownloadingTo))
                    return null;
                return new FileInfo(this.Entry.DownloadingTo).DirectoryName;
            }
        }

        public bool SameAs(Item o)
        {
            return (o is ItemuTorrenting)&& this.Entry == (o as ItemuTorrenting).Entry;
        }

        public ProcessedEpisode Episode { get; private set; }
        public IgnoreItem Ignore
        {
            get
            {
                if (string.IsNullOrEmpty(this.DesiredLocationNoExt))
                    return null;
                return new IgnoreItem(this.DesiredLocationNoExt);
            }
        }
        public ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = this.Episode.SI.ShowName();
                lvi.SubItems.Add(this.Episode.SeasonNumber.ToString());
                lvi.SubItems.Add(this.Episode.NumsAsString());
                DateTime? dt = this.Episode.GetAirDateDT(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                    lvi.SubItems.Add(dt.Value.ToShortDateString());
                else
                    lvi.SubItems.Add("");

                lvi.SubItems.Add(this.Entry.TorrentFile);
                lvi.SubItems.Add(this.Entry.DownloadingTo);
                int p = this.Entry.PercentDone;
                lvi.SubItems.Add(p == -1 ? "" : this.Entry.PercentDone + "% Complete");

                lvi.Tag = this;

                return lvi;
            }
        }

        public int ScanListViewGroup { get { return 7; } }
        int ScanListItem.IconNumber { get { return 2; } }

        public int Compare(Item o)
        {
            ItemuTorrenting ut = o as ItemuTorrenting;
            if (ut == null)
                return 0;

            if (this.Episode == null)
                return 1;
            if (ut.Episode == null)
                return -1;
            
            return (this.DesiredLocationNoExt).CompareTo(ut.DesiredLocationNoExt);
        }
    }
}