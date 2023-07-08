using BrightIdeasSoftware;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TVRename;

/// <summary>
/// ColumnComparer is the workhorse for all comparison between two values of a particular column.
/// If the column has a specific comparer, use that to compare the values. Otherwise, do
/// a case insensitive string compare of the string representations of the values.
/// </summary>
/// <remarks><para>This class inherits from both IComparer and its generic counterpart
/// so that it can be used on untyped and typed collections.</para>
/// <para>This is used by normal (non-virtual) ObjectListViews. Virtual lists use
/// ModelObjectComparer</para>
/// </remarks>
public class ListViewActionItemSorter : IComparer, IComparer<OLVListItem>
{
    /// <summary>
    /// Compare two rows
    /// </summary>
    /// <param name="x">row1</param>
    /// <param name="y">row2</param>
    /// <returns>An ordering indication: -1, 0, 1</returns>
    public int Compare(object? x, object? y) => Compare(x as OLVListItem, y as OLVListItem);

    /// <summary>
    /// Compare two rows
    /// </summary>
    /// <param name="x">row1</param>
    /// <param name="y">row2</param>
    /// <returns>An ordering indication: -1, 0, 1</returns>
    public int Compare(OLVListItem? x, OLVListItem? y)
    {
        Item? x1 = x?.RowObject as Item;
        Item? y1 = y?.RowObject as Item;

        // Handle nulls. Null values come last
        bool xIsNull = x1 == null;
        bool yIsNull = y1 == null;
        if (xIsNull || yIsNull)
        {
            if (xIsNull && yIsNull)
            {
                return 0;
            }

            return xIsNull ? -1 : 1;
        }

        return Polarity() * Sorter.Compare(x1, y1);
    }

    /// <summary>
    /// Class constructor.  Initializes various elements
    /// </summary>
    public ListViewActionItemSorter()
    {
        // Initialize the column to '0'
        SortColumn = 0;

        // Initialize the sort order to 'none'
        Order = SortOrder.None;

        // Initialize the CaseInsensitiveComparer object
        Sorter = new DefaultActionItemSorter();
    }

    private int Polarity()
    {
        return Order switch
        {
            // Calculate correct return value based on object comparison
            SortOrder.Ascending =>
                // Ascending sort is selected, return normal result of compare operation
                1,
            SortOrder.Descending =>
                // Descending sort is selected, return negative result of compare operation
                -1,
            SortOrder.None =>
                // Return '0' to indicate they are equal
                0,
            _ => 0
        };
    }

    /// <summary>
    /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
    /// </summary>
    public int SortColumn { set; get; }

    /// <summary>
    /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
    /// </summary>
    public SortOrder Order { set; get; }

    /// <summary>
    /// Case insensitive comparer object
    /// </summary>
    public ActionItemSorter Sorter { get; set; }

    public void ClickedOn(int col, ActionItemSorter sorter)
    {
        Sorter = sorter;

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

internal class OlvActionGroupComparer : IComparer<OLVListItem>
{
    /// <summary>
    /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
    /// </summary>
    private SortOrder Order { set; get; }

    /// <summary>
    /// Case insensitive comparer object
    /// </summary>
    private ActionItemSorter Sorter { get; set; }

    /// <summary>
    /// Class constructor.  Initializes various elements
    /// </summary>
    public OlvActionGroupComparer(ActionItemSorter sorter, SortOrder order)
    {
        // Initialize the sort order to 'none'
        Order = order;

        // Initialize the CaseInsensitiveComparer object
        Sorter = sorter;
    }

    /// <summary>
    /// Compare two rows
    /// </summary>
    /// <param name="x">row1</param>
    /// <param name="y">row2</param>
    /// <returns>An ordering indication: -1, 0, 1</returns>
    public int Compare(object? x, object? y) => Compare(x as OLVListItem, y as OLVListItem);

    /// <summary>
    /// Compare two rows
    /// </summary>
    /// <param name="x">row1</param>
    /// <param name="y">row2</param>
    /// <returns>An ordering indication: -1, 0, 1</returns>
    public int Compare(OLVListItem? x, OLVListItem? y)
    {
        Item? x1 = x?.RowObject as Item;
        Item? y1 = y?.RowObject as Item;

        // Handle nulls. Null values come last
        bool xIsNull = x1 == null;
        bool yIsNull = y1 == null;

        if (!xIsNull && !yIsNull)
        {
            return Polarity() * Sorter.Compare(x1, y1);
        }

        if (xIsNull && yIsNull)
        {
            return 0;
        }

        return xIsNull ? -1 : 1;
    }

    private int Polarity()
    {
        return Order switch
        {
            // Calculate correct return value based on object comparison
            SortOrder.Ascending =>
                // Ascending sort is selected, return normal result of compare operation
                1,
            SortOrder.Descending =>
                // Descending sort is selected, return negative result of compare operation
                -1,
            SortOrder.None =>
                // Return '0' to indicate they are equal
                0,
            _ => 0
        };
    }
}
