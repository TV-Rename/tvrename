using System;
using System.Windows.Forms;

namespace TVRename
{
    public sealed class DoubleAsTextSorter : ListViewItemSorter
    {
        public DoubleAsTextSorter(int column) : base(column)
        {
        }

        protected override int CompareListViewItem(ListViewItem x, ListViewItem y) => (int)(1000 * (ParseAsDouble(x) - ParseAsDouble(y)));

        private double ParseAsDouble(ListViewItem cellItem)
        {
            string value = cellItem.SubItems[Col].Text;

            if (!value.HasValue())
            {
                return -1;
            }

            if (value == TVSettings.SpecialsListViewName)
            {
                return 0;
            }

            try
            {
                return Convert.ToDouble(value);
            }
            catch
            {
                return 0;
            }
        }
    }
}
