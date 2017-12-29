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

    public class ItemSABnzbd : Item, ScanListItem
    {
        public string DesiredLocationNoExt;
        public SAB.queueSlotsSlot Entry;

        public ItemSABnzbd(SAB.queueSlotsSlot qss, ProcessedEpisode pe, string desiredLocationNoExt)
        {
            Episode = pe;
            DesiredLocationNoExt = desiredLocationNoExt;
            Entry = qss;
        }

        #region Item Members

        public bool SameAs(Item o)
        {
            return (o is ItemSABnzbd) && Entry == (o as ItemSABnzbd).Entry;
        }

        public int Compare(Item o)
        {
            ItemSABnzbd ut = o as ItemSABnzbd;
            if (ut == null)
                return 0;

            if (Episode == null)
                return 1;
            if (ut.Episode == null)
                return -1;

            return (DesiredLocationNoExt).CompareTo(ut.DesiredLocationNoExt);
        }

        #endregion

        #region ScanListItem Members

        public string TargetFolder
        {
            get
            {
                if (string.IsNullOrEmpty(Entry.filename))
                    return null;
                return new FileInfo(Entry.filename).DirectoryName;
            }
        }

        public ProcessedEpisode Episode { get; private set; }

        public IgnoreItem Ignore
        {
            get
            {
                if (string.IsNullOrEmpty(DesiredLocationNoExt))
                    return null;
                return new IgnoreItem(DesiredLocationNoExt);
            }
        }

        public ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = Episode.SI.ShowName;
                lvi.SubItems.Add(Episode.SeasonNumber.ToString());
                lvi.SubItems.Add(Episode.NumsAsString());
                DateTime? dt = Episode.GetAirDateDT(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                    lvi.SubItems.Add(dt.Value.ToShortDateString());
                else
                    lvi.SubItems.Add("");

                lvi.SubItems.Add(Entry.filename);
                String txt = Entry.status + ", " + (int) (0.5 + 100 - 100 * Entry.mbleft / Entry.mb) + "% Complete";
                if (Entry.status == "Downloading")
                    txt += ", " + Entry.timeleft + " left";
                
                lvi.SubItems.Add(txt);

                lvi.SubItems.Add("");

                lvi.Tag = this;

                return lvi;
            }
        }

        public string ScanListViewGroup
        {
            get { return "lvgDownloading"; }
        }

        int ScanListItem.IconNumber
        {
            get { return 8; }
        }

        #endregion
    }
}
