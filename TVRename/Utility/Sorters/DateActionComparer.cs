using System;
using BrightIdeasSoftware;

namespace TVRename;

public class DateActionComparer : ObjectListViewComparer<DateTime>
{
    public DateActionComparer(int column) : base(column)
    {
    }

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
