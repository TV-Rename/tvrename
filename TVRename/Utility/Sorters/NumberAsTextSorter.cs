// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using System;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace TVRename
{
    public sealed class NumberAsTextSorter : ListViewItemSorter
    {
        public NumberAsTextSorter(int column) : base(column) {}

        protected override int CompareListViewItem([NotNull] ListViewItem x, [NotNull] ListViewItem y) => ParseAsInt(x) - ParseAsInt(y);

        private int ParseAsInt( [NotNull] ListViewItem cellItem)
        {
            if (string.IsNullOrEmpty(cellItem.SubItems[Col].Text))
            {
                return -1;
            }

            try
            {
                return Convert.ToInt32(cellItem.SubItems[Col].Text);
            }
            catch
            {
                return 0;
            }
        }
    }
}
