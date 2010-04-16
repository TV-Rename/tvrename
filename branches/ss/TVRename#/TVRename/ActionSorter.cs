// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
namespace TVRename
{
    public class ActionSorter : System.Collections.Generic.IComparer<Item>
    {
        #region IComparer<ActionItem> Members

        public virtual int Compare(Item x, Item y)
        {
            return (x.GetType() == y.GetType()) ? x.Compare(y) : (TypeNumber(x) - TypeNumber(y));
        }

        static int TypeNumber(Item a)
        {
            if (a is ActionMissing) return 1;
            if (a is ActionCopyMoveRename) return 2;
            if (a is ActionRSS) return 3;
            if (a is ActionDownload) return 4;
            if (a is ActionNFO) return 5;
            if (a is ActionuTorrenting) return 6;
            return 7;
        }

        #endregion
    }
}