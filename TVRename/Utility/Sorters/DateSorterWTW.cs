//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Windows.Forms;

namespace TVRename;

public class DateSorterWtw : ListViewItemDateSorter
{
    public DateSorterWtw(int column) : base(column)
    {
    }

    protected override DateTime? GetDate(ListViewItem lvi)
    {
        try
        {
            return ((ProcessedEpisode)lvi.Tag).GetAirDateDt(true);
        }
        catch
        {
            return null;
        }
    }
}
