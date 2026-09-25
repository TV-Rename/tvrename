using BrightIdeasSoftware;
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

/// <summary>
/// Class constructor.  Initializes various elements
/// </summary>
internal class OlvGroupComparer<T>(ListSorter<T> sorter, SortOrder order) : IComparer<OLVListItem> where T : class
{
    /// <summary>
    /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
    /// </summary>
    private SortOrder Order { set; get; } = order;

    /// <summary>
    /// Case insensitive comparer object
    /// </summary>
    private ListSorter<T> Sorter { get; set; } = sorter;

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
        T? x1 = x?.RowObject as T;
        T? y1 = y?.RowObject as T;

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



