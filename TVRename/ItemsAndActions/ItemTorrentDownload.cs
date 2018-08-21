// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;

namespace TVRename
{
    public class ItemTorrentDownload : ItemInProgress
    {
        public TorrentEntry Entry;

        public ItemTorrentDownload(TorrentEntry te, ProcessedEpisode pe, string desiredLocationNoExt, TorrentApp tApp)
        {
            Episode = pe;
            DesiredLocationNoExt = desiredLocationNoExt;
            Entry = te;
            IconNumber = (tApp == TorrentApp.uTorrent) ? 2 : 10;
        }

        #region Item Members

        public override bool SameAs(Item o)
        {
            return (o is ItemTorrentDownload torrenting) && Entry == torrenting.Entry;
        }

        public override int Compare(Item o)
        {
            if (!(o is ItemTorrentDownload ut))
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

        protected override string FileIdentifier => Entry.TorrentFile;
        protected override string Destination => Entry.DownloadingTo;

        protected override string Remaining {
            get {
                int p = Entry.PercentDone;
                return p == -1 ? "" : Entry.PercentDone + "% Complete";
            }
        }
    }
}
