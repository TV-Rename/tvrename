// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using System.Collections;
using System.Windows.Forms;

// Sorting IComparer classes used by the ListViews in UI.cs

namespace TVRename
{
    public class TextSorter : IComparer
    {
        private int col;

        public TextSorter()
        {
            col = 0;
        }

        public TextSorter(int column)
        {
            col = column;
        }

        #region IComparer Members

        public virtual int Compare(Object x, Object y)
        {
            ListViewItem lvi1 = x as ListViewItem;
            ListViewItem lvi2 = y as ListViewItem;
            return string.Compare(lvi1.SubItems[col].Text, lvi2.SubItems[col].Text);
        }

        #endregion
    }

    public class DateSorterWTW : IComparer
    {
        #region IComparer Members

        public virtual int Compare(Object x, Object y)
        {
            DateTime? d1;
            DateTime? d2;

            try
            {
                d1 = ((Episode) ((x as ListViewItem).Tag)).GetAirDateDT(true);
            }
            catch
            {
                d1 = DateTime.Now;
            }

            try
            {
                d2 = ((Episode) ((y as ListViewItem).Tag)).GetAirDateDT(true);
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

    public class DaySorter : IComparer
    {
        private int col;

        public DaySorter()
        {
            col = 0;
        }

        public DaySorter(int column)
        {
            col = column;
        }

        #region IComparer Members

        public virtual int Compare(Object x, Object y)
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
            }

            return d1 - d2;
        }

        #endregion
    }

    public class NumberAsTextSorter : IComparer
    {
        private int col;

        public NumberAsTextSorter()
        {
            col = 0;
        }

        public NumberAsTextSorter(int column)
        {
            col = column;
        }

        #region IComparer Members

        public virtual int Compare(Object x, Object y)
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
