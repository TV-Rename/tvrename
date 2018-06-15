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
    // ReSharper disable once InconsistentNaming
    public class ItemSABnzbd : ItemInProgress
    {
        public SAB.queueSlotsSlot Entry;

        public ItemSABnzbd(SAB.queueSlotsSlot qss, ProcessedEpisode pe, string desiredLocationNoExt)
        {
            Episode = pe;
            DesiredLocationNoExt = desiredLocationNoExt;
            Entry = qss;
        }

        #region Item Members

        public override bool SameAs(Item o)
        {
            return (o is ItemSABnzbd bnzbd) && Entry == bnzbd.Entry;
        }

        public override int Compare(Item o)
        {
            if (!(o is ItemSABnzbd ut))
                { return 0;}
            if (Episode == null)
                { return 1;}
            if (ut.Episode == null)
                { return -1;}

            return string.Compare((DesiredLocationNoExt), ut.DesiredLocationNoExt, StringComparison.Ordinal);
        }
        #endregion

        #region Item Members
        public override  int IconNumber => 8;
        #endregion

        public override string FileIdentifier => Entry.filename;
        public override string Destination => Entry.filename;
        public override string Remaining {
            get {
                string txt = Entry.status + ", " + (int)(0.5 + 100 - 100 * Entry.mbleft / Entry.mb) + "% Complete";
                if (Entry.status == "Downloading")
                { txt += ", " + Entry.timeleft + " left";}
                return txt;
            }
        }
    }
}
