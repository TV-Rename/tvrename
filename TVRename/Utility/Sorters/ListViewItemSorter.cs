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

namespace TVRename
{
    public abstract class ListViewItemSorter : IComparer
    {
        protected readonly int Col;

        protected ListViewItemSorter(int column)
        {
            Col = column;
        }
        public int Compare(object x, object y)
        {
            if (!(x is ListViewItem lvix))
            {
                throw new InvalidOperationException();
            }

            if (!(y is ListViewItem lviy))
            {
                throw new InvalidOperationException();
            }

            return CompareListViewItem(lvix, lviy);
        }

        protected abstract int CompareListViewItem(ListViewItem x, ListViewItem y);
    }
}
