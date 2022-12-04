using BrightIdeasSoftware;
using System;

namespace TVRename;

public class NumberAsTextActionComparer : ObjectListViewComparer<int>
{
    public NumberAsTextActionComparer(int column) : base(column)
    {
    }

    protected override int GetValue(OLVListItem x, int columnId)
    {
        string value = x.SubItems[columnId].Text;

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
            return Convert.ToInt32(value);
        }
        catch
        {
            return 0;
        }
    }
}
