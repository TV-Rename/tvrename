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

    public class ItemSABnzbd : Item, ScanListItem
    {
        public string DesiredLocationNoExt;
        public SAB.queueSlotsSlot Entry;

        public ItemSABnzbd(SAB.queueSlotsSlot qss, ProcessedEpisode pe, string desiredLocationNoExt)
        {
            this.Episode = pe;
            this.DesiredLocationNoExt = desiredLocationNoExt;
            this.Entry = qss;
        }

        #region Item Members

        public bool SameAs(Item o)
        {
            return (o is ItemSABnzbd) && this.Entry == (o as ItemSABnzbd).Entry;
        }

        public int Compare(Item o)
        {
            ItemSABnzbd ut = o as ItemSABnzbd;
            if (ut == null)
                return 0;

            if (this.Episode == null)
                return 1;
            if (ut.Episode == null)
                return -1;

            return (this.DesiredLocationNoExt).CompareTo(ut.DesiredLocationNoExt);
        }

        #endregion

        #region ScanListItem Members

        public string TargetFolder
        {
            get
            {
                if (string.IsNullOrEmpty(this.Entry.filename))
                    return null;
                return new FileInfo(this.Entry.filename).DirectoryName;
            }
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

                lvi.Text = this.Episode.SI.ShowName;
                lvi.SubItems.Add(this.Episode.SeasonNumber.ToString());
                lvi.SubItems.Add(this.Episode.NumsAsString());
                DateTime? dt = this.Episode.GetAirDateDT(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                    lvi.SubItems.Add(dt.Value.ToShortDateString());
                else
                    lvi.SubItems.Add("");

                lvi.SubItems.Add(this.Entry.filename);
                String txt = this.Entry.status + ", " + (int) (0.5 + 100 - 100 * Entry.mbleft / Entry.mb) + "% Complete";
                if (this.Entry.status == "Downloading")
                    txt += ", " + this.Entry.timeleft + " left";
                
                lvi.SubItems.Add(txt);

                lvi.SubItems.Add("");

                lvi.Tag = this;

                return lvi;
            }
        }

        public int ScanListViewGroup
        {
            get { return 8; }
        }

        int ScanListItem.IconNumber
        {
            get { return 8; }
        }

        #endregion
    }
}