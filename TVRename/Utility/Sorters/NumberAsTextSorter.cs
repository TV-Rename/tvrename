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

        protected override int CompareListViewItem(ListViewItem x, ListViewItem y) => ParseAsInt(x.SubItems[Col].Text) - ParseAsInt(y.SubItems[Col].Text);

        private static int ParseAsInt([CanBeNull] string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return -1;
            }

            try
            {
                return Convert.ToInt32(text);
            }
            catch
            {
                return 0;
            }
        }
    }
}
