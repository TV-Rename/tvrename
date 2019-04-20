// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Windows.Forms;

namespace TVRename
{
    public sealed class DateSorterWtw : ListViewItemSorter
    {
        public DateSorterWtw(int column) : base(column) { }

        protected override int CompareListViewItem(ListViewItem x, ListViewItem y)
        {
            DateTime? d1 = GetDate(x);
            DateTime? d2 = GetDate(y);

            if ((d1 is null) && (d2 is null))
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

            return d1.Value.CompareTo(d2.Value);
        }

        private static DateTime? GetDate(ListViewItem lvi)
        {
            try
            {
                return ((ProcessedEpisode)(lvi.Tag)).GetAirDateDt(true);
            }
            catch
            {
                return DateTime.Now;
            }
        }
    }
}
