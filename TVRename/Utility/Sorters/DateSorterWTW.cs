// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections;
using System.Windows.Forms;

namespace TVRename
{
    public sealed class DateSorterWtw : IComparer
    {
        #region IComparer Members

        public int Compare(object x, object y)
        {
            if (!(x is ListViewItem lvix)) throw new InvalidOperationException();
            if (!(y is ListViewItem lviy)) throw new InvalidOperationException();

            DateTime? d1 = GetDate(lvix);
            DateTime? d2 = GetDate(lviy);

            if ((d1 == null) && (d2 == null))
                return 0;
            if (d1 == null)
                return -1;
            if (d2 == null)
                return 1;
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

        #endregion
    }
}
