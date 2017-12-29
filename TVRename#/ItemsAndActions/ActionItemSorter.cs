// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
namespace TVRename
{
    public class ActionItemSorter : System.Collections.Generic.IComparer<ITem>
    {
        #region IComparer<Item> Members

        public virtual int Compare(ITem x, ITem y)
        {
            return (x.GetType() == y.GetType()) ? x.Compare(y) : (TypeNumber(x) - TypeNumber(y));
        }

        #endregion

        private static int TypeNumber(ITem a)
        {
            if (a is ItemMissing)
                return 1;
            if (a is ActionCopyMoveRename)
                return 2;
            if (a is ActionRss)
                return 3;
            if (a is ActionDownload)
                return 4;
            if (a is ActionNfo)
                return 5;
            if (a is ItemuTorrenting || a is ItemSaBnzbd)
                return 6;
            return 7;
        }
    }
}