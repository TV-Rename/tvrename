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
    public class ItemSABnzbd : ItemInProgress
    {
        public SAB.queueSlotsSlot Entry;

        public ItemSABnzbd(SAB.queueSlotsSlot qss, ProcessedEpisode pe, string desiredLocationNoExt)
        {
            this.Episode = pe;
            this.DesiredLocationNoExt = desiredLocationNoExt;
            this.Entry = qss;
        }

        #region Item Members

        public override bool SameAs(Item o)
        {
            return (o is ItemSABnzbd bnzbd) && this.Entry == bnzbd.Entry;
        }

        public override int Compare(Item o)
        {
            if (!(o is ItemSABnzbd ut)){ return 0;}
            if (this.Episode == null){   return 1;}
            if (ut.Episode == null)  {    return -1;}

            return string.Compare((this.DesiredLocationNoExt), ut.DesiredLocationNoExt, StringComparison.Ordinal);
        }
        #endregion

        #region Item Members
        public override  int IconNumber => 8;
        #endregion

        public override string FileIdentifier => this.Entry.filename;
        public override string Destination => this.Entry.filename;
        public override string Remaining {
            get {
                string txt = this.Entry.status + ", " + (int)(0.5 + 100 - 100 * this.Entry.mbleft / this.Entry.mb) + "% Complete";
                if (this.Entry.status == "Downloading")
                { txt += ", " + this.Entry.timeleft + " left";}
                return txt;
            }
        }
    }
}
