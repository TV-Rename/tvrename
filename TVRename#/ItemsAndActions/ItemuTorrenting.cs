// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
namespace TVRename
{
    using System;
    using Alphaleonis.Win32.Filesystem;
    using System.Windows.Forms;

    public class ItemuTorrenting : ItemInProgress
    {
        public TorrentEntry Entry;

        public ItemuTorrenting(TorrentEntry te, ProcessedEpisode pe, string desiredLocationNoExt)
        {
            this.Episode = pe;
            this.DesiredLocationNoExt = desiredLocationNoExt;
            this.Entry = te;
        }

        #region Item Members

        public override bool SameAs(Item o)
        {
            return (o is ItemuTorrenting) && this.Entry == (o as ItemuTorrenting).Entry;
        }

        public override int Compare(Item o)
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

        #endregion

        #region Item Members

        public override string TargetFolder
        {
            get
            {
                if (string.IsNullOrEmpty(this.Entry.DownloadingTo))
                    return null;
                return new FileInfo(this.Entry.DownloadingTo).DirectoryName;
            }
        }

        public override ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = this.Episode.SI.ShowName;
                lvi.SubItems.Add(this.Episode.AppropriateSeasonNumber.ToString());
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

        public override int IconNumber
        {
            get { return 2; }
        }

        #endregion
    }
}
