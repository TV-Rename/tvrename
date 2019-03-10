using System;
using System.Windows.Forms;

namespace TVRename
{
    public sealed class DaySorter : ListViewItemSorter
    {
        public DaySorter(int column) : base(column) { }

        protected override int CompareListViewItem(ListViewItem x, ListViewItem y)
        {
            int d1 = 8;
            int d2 = 8;

            try
            {
                string t1 = x.SubItems[Col].Text;
                string t2 = y.SubItems[Col].Text;

                DateTime now = DateTime.Now;

                for (int i = 0; i < 7; i++)
                {
                    if ((now + new TimeSpan(i, 0, 0, 0)).ToString("ddd") == t1)
                    {
                        d1 = i;
                    }

                    if ((now + new TimeSpan(i, 0, 0, 0)).ToString("ddd") == t2)
                    {
                        d2 = i;
                    }
                }
            }
            catch
            {
                // ignored
            }

            return d1 - d2;
        }
    }
}
