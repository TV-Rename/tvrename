// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Windows.Forms;
using JetBrains.Annotations;

// Sorting IComparer classes used by the ListViews in UI.cs

namespace TVRename
{
    public sealed class TextSorter : ListViewItemSorter
    {
        public TextSorter(int column) : base(column) { }

        protected override int CompareListViewItem([NotNull] ListViewItem x, [NotNull] ListViewItem y) =>
            string.Compare(x.SubItems[Col].Text, y.SubItems[Col].Text);
    }
}
