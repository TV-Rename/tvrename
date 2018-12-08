using System;
using System.Collections;
using System.Windows.Forms;

namespace TVRename
{
    public sealed class DateSorterWTW : IComparer
    {
        #region IComparer Members

        public int Compare(object x, object y)
        {
            DateTime? d1;
            DateTime? d2;

            try
            {
                d1 = ((ProcessedEpisode) ((x as ListViewItem).Tag)).GetAirDateDt(true);
            }
            catch
            {
                d1 = DateTime.Now;
            }

            try
            {
                d2 = ((ProcessedEpisode) ((y as ListViewItem).Tag)).GetAirDateDt(true);
            }
            catch
            {
                d2 = DateTime.Now;
            }

            if ((d1 == null) && (d2 == null))
                return 0;
            if (d1 == null)
                return -1;
            if (d2 == null)
                return 1;
            return d1.Value.CompareTo(d2.Value);
        }

        #endregion
    }
}
