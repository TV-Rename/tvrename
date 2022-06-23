//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Windows.Forms;

// Sorting IComparer classes used by the ListViews in UI.cs

namespace TVRename;

public sealed class TextSorter : ListViewItemSorter
{
    public TextSorter(int column) : base(column)
    {
    }

    protected override int CompareListViewItem(ListViewItem x, ListViewItem y) =>
        string.Compare(x.SubItems[Col].Text, y.SubItems[Col].Text, StringComparison.OrdinalIgnoreCase);
}
