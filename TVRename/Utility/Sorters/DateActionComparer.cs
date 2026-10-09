//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using BrightIdeasSoftware;


namespace TVRename;

public class DateActionComparer(int column) : ObjectListViewComparer<DateTime>(column)
{
    protected override DateTime GetValue(OLVListItem x, int columnId)
    {
        try
        {
            return ((Item)x.RowObject).AirDate ?? DateTime.MinValue;
        }
        catch
        {
            return DateTime.MinValue;
        }
    }
}
