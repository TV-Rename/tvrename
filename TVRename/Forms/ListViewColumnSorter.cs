using System.Collections;
using System.Windows.Forms;

/// <summary>
/// This class is an implementation of the 'IComparer' interface.
/// </summary>
namespace TVRename
{
    public class ListViewColumnSorter : IComparer
    {
        /// <summary>
        /// Specifies the column to be sorted
        /// </summary>
        private int columnToSort;

        /// <summary>
        /// Specifies the order in which to sort (i.e. 'Ascending').
        /// </summary>
        private SortOrder orderOfSort;

        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        private ListViewItemSorter sorter;

        /// <summary>
        /// Class constructor.  Initializes various elements
        /// </summary>
        public ListViewColumnSorter(ListViewItemSorter s)
        {
            // Initialize the column to '0'
            columnToSort = 0;

            // Initialize the sort order to 'none'
            orderOfSort = SortOrder.None;

            // Initialize the CaseInsensitiveComparer object
            ListViewItemSorter = s;
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {
            // Cast the objects to be compared to ListViewItem objects
            ListViewItem listviewX = (ListViewItem) x;
            ListViewItem listviewY = (ListViewItem) y;

            // Compare the two items
            int compareResult = ListViewItemSorter.Compare(listviewX,listviewY);

            switch (orderOfSort)
            {
                // Calculate correct return value based on object comparison
                case SortOrder.Ascending:
                    // Ascending sort is selected, return normal result of compare operation
                    return compareResult;

                case SortOrder.Descending:
                    // Descending sort is selected, return negative result of compare operation
                    return (-compareResult);

                case SortOrder.None:
                default:
                    // Return '0' to indicate they are equal
                    return 0;
            }
        }

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn
        {
            set { columnToSort = value; }
            get { return columnToSort; }
        }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order
        {
            set { orderOfSort = value; }
            get { return orderOfSort; }
        }

        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        public ListViewItemSorter ListViewItemSorter
        {
            get { return sorter; }
            set { sorter = value; }
        }

        public void ClickedOn(int col)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (col == SortColumn)
            {
                // Reverse the current sort direction for this column.
                Order = Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                SortColumn = col;
                Order = SortOrder.Ascending;
            }
        }
    }
}
