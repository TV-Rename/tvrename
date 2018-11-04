using System;
using System.Collections;
using System.Windows.Forms;

namespace TVRename
{
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
}