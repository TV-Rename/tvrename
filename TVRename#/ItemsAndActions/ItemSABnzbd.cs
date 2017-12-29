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

    public class ItemSaBnzbd : ITem, IScanListItem
    {
        public string DesiredLocationNoExt;
        public SAB.QueueSlotsSlot Entry;

        public ItemSaBnzbd(SAB.QueueSlotsSlot qss, ProcessedEpisode pe, string desiredLocationNoExt)
        {
            Episode = pe;
            DesiredLocationNoExt = desiredLocationNoExt;
            Entry = qss;
        }

        #region Item Members

        public bool SameAs(ITem o)
        {
            return (o is ItemSaBnzbd) && Entry == (o as ItemSaBnzbd).Entry;
        }

        public int Compare(ITem o)
        {
            ItemSaBnzbd ut = o as ItemSaBnzbd;
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
                if (string.IsNullOrEmpty(Entry.Filename))
                    return null;
                return new FileInfo(Entry.Filename).DirectoryName;
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

                lvi.Text = Episode.Si.ShowName;
                lvi.SubItems.Add(Episode.SeasonNumber.ToString());
                lvi.SubItems.Add(Episode.NumsAsString());
                DateTime? dt = Episode.GetAirDateDt(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                    lvi.SubItems.Add(dt.Value.ToShortDateString());
                else
                    lvi.SubItems.Add("");

                lvi.SubItems.Add(Entry.Filename);
                String txt = Entry.Status + ", " + (int) (0.5 + 100 - 100 * Entry.Mbleft / Entry.Mb) + "% Complete";
                if (Entry.Status == "Downloading")
                    txt += ", " + Entry.Timeleft + " left";
                
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

        int IScanListItem.IconNumber
        {
            get { return 8; }
        }

        #endregion
    }
}
