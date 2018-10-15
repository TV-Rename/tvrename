// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using Alphaleonis.Win32.Filesystem;
using System;
using System.Windows.Forms;

namespace TVRename
{
    public class ItemDownloading : Item
    {
        public DownloadInformation Entry;
        public string DesiredLocationNoExt;


        public ItemDownloading(DownloadInformation dl, ProcessedEpisode pe, string desiredLocationNoExt, TVRename.Finder.DownloadApp tApp)
        {
            Episode = pe;
            DesiredLocationNoExt = desiredLocationNoExt;
            Entry = dl;
            IconNumber = (tApp == TVRename.Finder.DownloadApp.uTorrent) ? 2 :
                         (tApp == TVRename.Finder.DownloadApp.SABnzbd)  ? 8 :
                         (tApp == TVRename.Finder.DownloadApp.qBitTorrent) ? 10 : 0;
        }

        public override IgnoreItem Ignore => GenerateIgnore(DesiredLocationNoExt);

        public override string ScanListViewGroup => "lvgDownloading";

        public override ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem { Text = Episode.Show.ShowName };
                lvi.SubItems.Add(Episode.AppropriateSeasonNumber.ToString());
                lvi.SubItems.Add(Episode.NumsAsString());
                lvi.SubItems.Add(Episode.GetAirDateDT(true).PrettyPrint());
                lvi.SubItems.Add(FileIdentifier);
                lvi.SubItems.Add(Destination);
                lvi.SubItems.Add(Remaining);
                lvi.Tag = this;
                return lvi;
            }
        }

        public override string TargetFolder
        {
            get
            {
                if (string.IsNullOrEmpty(Destination))
                    return null;

                return new FileInfo(Destination).DirectoryName;
            }
        }

        #region Item Members

        public override bool SameAs(Item o)
        {
            return (o is ItemDownloading torrenting) && Entry == torrenting.Entry;
        }

        public override int Compare(Item o)
        {
            if (!(o is ItemDownloading ut))
                { return 0;}
            if (Episode == null)
                { return 1;}
            if (ut.Episode == null)
                { return -1;}

            return string.Compare((DesiredLocationNoExt), ut.DesiredLocationNoExt, StringComparison.Ordinal);
        }
        #endregion

        #region Item Members
        public override int IconNumber { get; }
        #endregion

        protected string FileIdentifier => Entry.FileIdentifier;
        protected string Destination => Entry.Destination;
        protected string Remaining => Entry.RemainingText;
    }
}
