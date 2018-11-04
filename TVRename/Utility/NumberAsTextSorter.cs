using System;
using System.Collections;
using System.Windows.Forms;

namespace TVRename
{
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