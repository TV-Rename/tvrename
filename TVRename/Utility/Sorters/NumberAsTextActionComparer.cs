//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using BrightIdeasSoftware;


namespace TVRename;

public class NumberAsTextActionComparer(int column) : ObjectListViewComparer<int>(column)
{
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
