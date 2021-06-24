using BrightIdeasSoftware;
using System.Collections;
using System.Collections.Generic;

namespace TVRename
{
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
        public int Compare(object x, object y) => Compare((OLVListItem)x, (OLVListItem)y);

        /// <summary>
        /// Compare two rows
        /// </summary>
        /// <param name="x">row1</param>
        /// <param name="y">row2</param>
        /// <returns>An ordering indication: -1, 0, 1</returns>
        public int Compare(OLVListItem x, OLVListItem y)
        {
            Item x1 = x.RowObject as Item;
            Item y1 = y.RowObject as Item;

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

            return new ActionItemSorter().Compare(x1, y1);
        }
    }
}
