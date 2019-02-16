// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections;
using System.Windows.Forms;

// Sorting IComparer classes used by the ListViews in UI.cs

namespace TVRename
{
    public sealed class TextSorter : IComparer
    {
        private readonly int col;

        public TextSorter(int column)
        {
            col = column;
        }

        #region IComparer Members

        public int Compare(object x, object y)
        {
            if (!(x is ListViewItem lvix)) throw new InvalidOperationException();
            if (!(y is ListViewItem lviy)) throw new InvalidOperationException();
            return string.CompareOrdinal( lvix.SubItems[col].Text, lviy.SubItems[col].Text);
        }
        #endregion
    }
}
