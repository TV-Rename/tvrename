//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

public static class UiExtensions
{
    public static void ScaleListViewColumns(this ListView listview, SizeF factor)
    {
        foreach (ColumnHeader column in listview.Columns)
        {
            column.Width = (int)Math.Round(column.Width * factor.Width);
        }
    }

    public static string ToUiVersion(this string? source)
        => source.HasValue() ? source.Replace("&", "&&") : string.Empty;
}
