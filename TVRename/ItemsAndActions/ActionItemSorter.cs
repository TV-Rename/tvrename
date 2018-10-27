// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
namespace TVRename
{
    public class ActionItemSorter : System.Collections.Generic.IComparer<Item>
    {
        #region IComparer<Item> Members

        public int Compare(Item x, Item y)
        {
            if (x is null) return -1;
            if (y is null) return 1;
            return (x.GetType() == y.GetType()) ? x.Compare(y) : (TypeNumber(x) - TypeNumber(y));
        }

        #endregion

        private static int TypeNumber(Item a)
        {
            if (a is ItemMissing)
                return 1;
            if (a is ActionCopyMoveRename)
                return 2;
            if (a is ActionTDownload)
                return 3;
            if (a is ActionDownloadImage)
                return 4;
            if (a is ActionNfo)
                return 5;
            if (a is ItemDownloading)
                return 6;
            return 7;
        }
    }
}
