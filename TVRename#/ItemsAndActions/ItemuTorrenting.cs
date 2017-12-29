// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    public class ItemuTorrenting : ITem, IScanListItem
    {
        public string DesiredLocationNoExt;
        public TorrentEntry Entry;

        public ItemuTorrenting(TorrentEntry te, ProcessedEpisode pe, string desiredLocationNoExt)
        {
            Episode = pe;
            DesiredLocationNoExt = desiredLocationNoExt;
            Entry = te;
        }

        #region Item Members

        public bool SameAs(ITem o)
        {
            return (o is ItemuTorrenting) && Entry == (o as ItemuTorrenting).Entry;
        }

        public int Compare(ITem o)
        {
            ItemuTorrenting ut = o as ItemuTorrenting;
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
                if (string.IsNullOrEmpty(Entry.DownloadingTo))
                    return null;
                return new FileInfo(Entry.DownloadingTo).DirectoryName;
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

                lvi.SubItems.Add(Entry.TorrentFile);
                lvi.SubItems.Add(Entry.DownloadingTo);
                int p = Entry.PercentDone;
                lvi.SubItems.Add(p == -1 ? "" : Entry.PercentDone + "% Complete");

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
            get { return 2; }
        }

        #endregion
    }
}
