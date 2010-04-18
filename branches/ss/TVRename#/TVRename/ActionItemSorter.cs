// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
namespace TVRename
{
    public class ActionItemSorter : System.Collections.Generic.IComparer<Item>
    {
        #region IComparer<Item> Members

        public virtual int Compare(Item x, Item y)
        {
            return (x.GetType() == y.GetType()) ? x.Compare(y) : (TypeNumber(x) - TypeNumber(y));
        }

        static int TypeNumber(Item a)
        {
            if (a is ItemMissing) return 1;
            if (a is ActionCopyMoveRename) return 2;
            if (a is ActionRSS) return 3;
            if (a is ActionDownload) return 4;
            if (a is ActionNFO) return 5;
            if (a is ItemuTorrenting) return 6;
            return 7;
        }

        #endregion
    }
}