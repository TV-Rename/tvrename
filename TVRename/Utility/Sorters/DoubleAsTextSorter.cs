//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

public sealed class DoubleAsTextSorter(int column) : ListViewItemSorter(column)
{
    protected override int CompareListViewItem(ListViewItem x, ListViewItem y) => (int)(1000 * (ParseAsDouble(x) - ParseAsDouble(y)));

    private double ParseAsDouble(ListViewItem cellItem)
    {
        string value = cellItem.SubItems[Col].Text;

        if (!value.HasValue())
        {
            return -1;
        }

        if (value == TVSettings.SpecialsListViewName)
        {
            return 0;
        }

        try
        {
            return Convert.ToDouble(value);
        }
        catch
        {
            return 0;
        }
    }
}
