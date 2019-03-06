using System;
using System.Collections;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace TVRename
{
    public sealed class NumberAsTextSorter : IComparer
    {
        private readonly int col;

        public NumberAsTextSorter(int column)
        {
            col = column;
        }

        #region IComparer Members

        public int Compare(object x, object y)
        {
            if (!(x is ListViewItem lvix))
            {
                throw new InvalidOperationException();
            }

            if (!(y is ListViewItem lviy))
            {
                throw new InvalidOperationException();
            }

            return ParseAsInt(lvix.SubItems[col].Text) - ParseAsInt(lviy.SubItems[col].Text);
        }

        private static int ParseAsInt([CanBeNull] string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return -1;
            }

            try
            {
                return Convert.ToInt32(text);
            }
            catch
            {
                return 0;
            }
        }
        #endregion
    }
}
