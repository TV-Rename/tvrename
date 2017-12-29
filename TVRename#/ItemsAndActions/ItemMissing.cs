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

    public class ItemMissing : Item, ScanListItem
    {
        public string TheFileNoExt;

        public ItemMissing(ProcessedEpisode pe, string whereItShouldBeNoExt)
        {
            Episode = pe;
            TheFileNoExt = whereItShouldBeNoExt;
        }

        #region Item Members

        public bool SameAs(Item o)
        {
            return (o is ItemMissing) && (string.Compare((o as ItemMissing).TheFileNoExt, TheFileNoExt) == 0);
        }

        public int Compare(Item o)
        {
            ItemMissing miss = o as ItemMissing;
            //return (o == null || miss == null) ? 0 : (this.TheFileNoExt + this.Episode.Name).CompareTo(miss.TheFileNoExt + miss.Episode.Name);
            if (o == null || miss == null)
            {
                return 0;
            }

            if (!Episode.SI.ShowName.Equals(miss.Episode.SI.ShowName))
            {
                return Episode.SI.ShowName.CompareTo(miss.Episode.SI.ShowName);
            }

            if (!Episode.SeasonNumber.Equals(miss.Episode.SeasonNumber))
            {
                int compare = Episode.SeasonNumber.CompareTo(miss.Episode.SeasonNumber);
                return compare;
            }

            return Episode.EpNum.CompareTo(miss.Episode.EpNum);
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
                                                        Text = Episode.SI.ShowName
                                                    };

                lvi.SubItems.Add(Episode.SeasonNumber.ToString());
                lvi.SubItems.Add(Episode.NumsAsString());

                DateTime? dt = Episode.GetAirDateDT(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue)) != 0)
                    lvi.SubItems.Add(dt.Value.ToShortDateString());
                else
                    lvi.SubItems.Add("");

                FileInfo fi = new FileInfo(TheFileNoExt);
                lvi.SubItems.Add(fi.DirectoryName);
                lvi.SubItems.Add(fi.Name);

                lvi.Tag = this;

                return lvi;
            }
        }

        public string ScanListViewGroup
        {
            get { return "lvgActionMissing"; }
        }

        public string TargetFolder
        {
            get
            {
                if (string.IsNullOrEmpty(TheFileNoExt))
                    return null;
                return new FileInfo(TheFileNoExt).DirectoryName;
            }
        }

        public int IconNumber
        {
            get { return 1; }
        }

        #endregion
    }
}
