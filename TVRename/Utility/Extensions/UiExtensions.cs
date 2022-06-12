using System;
using System.Drawing;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace TVRename
{
    public static class UiExtensions
    {
        public static void ScaleListViewColumns([NotNull] this ListView listview, SizeF factor)
        {
            foreach (ColumnHeader column in listview.Columns)
            {
                column.Width = (int)Math.Round(column.Width * factor.Width);
            }
        }

        [NotNull]
        public static string ToUiVersion([NotNull] this string source)
            => source.ToUiVersion();
    }
}
