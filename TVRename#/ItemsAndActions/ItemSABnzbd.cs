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
using TVRename.SAB;

namespace TVRename
{
    public class ItemSaBnzbd : Item, IScanListItem
    {
        public readonly string DesiredLocationNoExt;
        private readonly QueueSlotsSlot _entry;

        public ItemSaBnzbd(QueueSlotsSlot qss, ProcessedEpisode pe, string desiredLocationNoExt)
        {
            Episode = pe;
            DesiredLocationNoExt = desiredLocationNoExt;
            _entry = qss;
        }

        #region Item Members

        public bool SameAs(Item o)
        {
            return (o is ItemSaBnzbd bnzbd) && _entry == bnzbd._entry;
        }

        public int Compare(Item o)
        {
            if (!(o is ItemSaBnzbd ut))
                return 0;

            if (Episode == null)
                return 1;
            if (ut.Episode == null)
                return -1;

            return String.Compare((DesiredLocationNoExt), ut.DesiredLocationNoExt, StringComparison.Ordinal);
        }

        #endregion

        #region ScanListItem Members

        public string TargetFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_entry.Filename))
                    return null;
                return new FileInfo(_entry.Filename).DirectoryName;
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

                lvi.SubItems.Add(_entry.Filename);
                String txt = _entry.Status + ", " + (int) (0.5 + 100 - 100 * _entry.Mbleft / _entry.Mb) + "% Complete";
                if (_entry.Status == "Downloading")
                    txt += ", " + _entry.Timeleft + " left";
                
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
