using BrightIdeasSoftware;
using System;

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
