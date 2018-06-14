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

// Sorting IComparer classes used by the ListViews in UI.cs

namespace TVRename
{
    public class TextSorter : IComparer
    {
        private readonly int col;

        public TextSorter()
        {
            col = 0;
        }

        public TextSorter(int column)
        {
            col = column;
        }

        #region IComparer Members

        public virtual int Compare(object x, object y)
        {
            ListViewItem lvi1 = x as ListViewItem;
            ListViewItem lvi2 = y as ListViewItem;
            return string.CompareOrdinal( lvi1.SubItems[col].Text, lvi2.SubItems[col].Text);
        }

        #endregion
    }

    public sealed class DateSorterWTW : IComparer
    {
        #region IComparer Members

        public int Compare(object x, object y)
        {
            DateTime? d1;
            DateTime? d2;

            try
            {
                d1 = ((ProcessedEpisode) ((x as ListViewItem).Tag)).GetAirDateDT(true);
            }
            catch
            {
                d1 = DateTime.Now;
            }

            try
            {
                d2 = ((ProcessedEpisode) ((y as ListViewItem).Tag)).GetAirDateDT(true);
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

    public sealed class DaySorter : IComparer
    {
        private readonly int col;

        public DaySorter()
        {
            col = 0;
        }

        public DaySorter(int column)
        {
            col = column;
        }

        #region IComparer Members

        public int Compare(object x, object y)
        {
            int d1 = 8;
            int d2 = 8;

            try
            {
                string t1 = (x as ListViewItem).SubItems[col].Text;
                string t2 = (y as ListViewItem).SubItems[col].Text;

                DateTime now = DateTime.Now;

                for (int i = 0; i < 7; i++)
                {
                    if ((now + new TimeSpan(i, 0, 0, 0)).ToString("ddd") == t1)
                        d1 = i;
                    if ((now + new TimeSpan(i, 0, 0, 0)).ToString("ddd") == t2)
                        d2 = i;
                }
            }
            catch
            {
                // ignored
            }

            return d1 - d2;
        }

        #endregion
    }

    public sealed class NumberAsTextSorter : IComparer
    {
        private readonly int col;

        public NumberAsTextSorter()
        {
            col = 0;
        }

        public NumberAsTextSorter(int column)
        {
            col = column;
        }

        #region IComparer Members

        public int Compare(object x, object y)
        {
            int one;
            int two;
            string s1 = ((x as ListViewItem).SubItems)[col].Text;
            string s2 = ((y as ListViewItem).SubItems)[col].Text;
            if (string.IsNullOrEmpty(s1))
                s1 = "-1";
            if (string.IsNullOrEmpty(s2))
                s2 = "-1";

            try
            {
                one = Convert.ToInt32(s1);
            }
            catch
            {
                one = 0;
            }
            try
            {
                two = Convert.ToInt32(s2);
            }
            catch
            {
                two = 0;
            }

            return one - two;
        }

        #endregion
    }
}
