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
            return (o is ItemuTorrenting torrenting) && this.Entry == torrenting.Entry;
        }

        public override int Compare(Item o)
        {
            if (!(o is ItemuTorrenting ut)){ return 0;}
            if (this.Episode == null){ return 1;}
            if (ut.Episode == null){ return -1;}

            return string.Compare((this.DesiredLocationNoExt), ut.DesiredLocationNoExt, StringComparison.Ordinal);
        }
        #endregion

        #region Item Members
        public override int IconNumber => 2;
        #endregion

        public override string FileIdentifier => this.Entry.TorrentFile;
        public override string Destination => this.Entry.DownloadingTo;
        public override string Remaining {
            get {
                int p = this.Entry.PercentDone;
                return p == -1 ? "" : this.Entry.PercentDone + "% Complete";
            }
        }
    }
}
