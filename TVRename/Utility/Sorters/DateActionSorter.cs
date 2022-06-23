//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace TVRename;

public class DateActionSorter : ListViewItemDateSorter
{
    public DateActionSorter(int column) : base(column)
    {
    }

    protected override DateTime? GetDate(ListViewItem lvi)
    {
        try
        {
            if (lvi is OLVListItem olvi)
            {
                return ((Item)olvi.RowObject).AirDate ?? DateTime.Now;
            }
            return DateTime.Now;
        }
        catch
        {
            return DateTime.Now;
        }
    }
}
