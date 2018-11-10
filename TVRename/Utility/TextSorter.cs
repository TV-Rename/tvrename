// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections;
using System.Windows.Forms;

// Sorting IComparer classes used by the ListViews in UI.cs

namespace TVRename
{
    public class TextSorter : IComparer
    {
        private readonly int col;

        public TextSorter(int column)
        {
            col = column;
        }

        #region IComparer Members

        public virtual int Compare(object x, object y)
        {
            ListViewItem lvi1 = x as ListViewItem;
            ListViewItem lvi2 = y as ListViewItem;
            return string.CompareOrdinal( lvi1.SubItems[col].Text, lvi2.SubItems[col].Text);
        }
        #endregion
    }
}
