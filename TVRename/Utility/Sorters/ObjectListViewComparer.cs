//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using BrightIdeasSoftware;
using System;
using System.Collections.Generic;

namespace TVRename;

public abstract class ObjectListViewComparer<T> : IComparer<OLVListItem> where T : IComparable<T>
{
    private readonly int col;

    protected ObjectListViewComparer(int column)
    {
        col = column;
    }

    public int Compare(OLVListItem? x, OLVListItem? y)
    {
        if (col == -1)
        {
            return 0;
        }
        T? d1 = x != null ? GetValue(x, col) : default;
        T? d2 = y != null ? GetValue(y, col) : default;

        if (d1 is null && d2 is null)
        {
            return 0;
        }

        if (d1 is null)
        {
            return -1;
        }

        if (d2 is null)
        {
            return 1;
        }

        int primary = d1.CompareTo(d2);
        return primary == 0 ?
            string.Compare(GetDefault(x!), GetDefault(y!), StringComparison.Ordinal)
            : primary;
    }

    private static string GetDefault(OLVListItem p) => ((Item)p.RowObject).OrderKey;

    protected abstract T? GetValue(OLVListItem x, int columnId);
}
