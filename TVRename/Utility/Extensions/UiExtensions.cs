using System;
using System.Drawing;
using System.Windows.Forms;

namespace TVRename
{
    public static class UiExtensions
    {
        public static void ScaleListViewColumns(this ListView listview, SizeF factor)
        {
            foreach (ColumnHeader column in listview.Columns)
            {
                column.Width = (int)Math.Round(column.Width * factor.Width);
            }
        }

        public static string ToUiVersion(this string source)
            => source.Replace("&", "&&");
    }
}
