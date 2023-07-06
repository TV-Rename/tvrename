using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace TVRename.Forms;

/// <summary>
/// This comparer sort list view specifically for sesaons so that they appear in the season order
/// OLVGroups have a "SortValue" property,
/// which is used if present. Otherwise, the titles of the groups will be compared.
/// </summary>
public class SeasonGroupComparer : IComparer<OLVGroup>
{
    /// <summary>
    /// Create a group comparer
    /// </summary>
    /// <param name="order">The ordering for column values</param>
    public SeasonGroupComparer(SortOrder order)
    {
        sortOrder = order;
    }

    /// <summary>
    /// Compare the two groups. OLVGroups have a "SortValue" property,
    /// which is used if present. Otherwise, the titles of the groups will be compared.
    /// </summary>
    /// <param name="x">group1</param>
    /// <param name="y">group2</param>
    /// <returns>An ordering indication: -1, 0, 1</returns>
    public int Compare(OLVGroup? x, OLVGroup? y)
    {
        if (x is null || y is null)
        {
            return 0;
        }

        // If we can compare the sort values, do that.
        // Otherwise do a case insensitive compare on the group header.
        int result;
        if (x.Items.Any() && y.Items.Any())
        {
            result = CompareValue(x).CompareTo(CompareValue(y));
        }
        else if (x.SortValue != null && y.SortValue != null)
        {
            result = x.SortValue.CompareTo(y.SortValue);
        }
        else
        {
            result = string.Compare(x.Header, y.Header, StringComparison.CurrentCultureIgnoreCase);
        }

        if (sortOrder == SortOrder.Descending)
        {
            return 0 - result;
        }

        return result;
    }

    private static int CompareValue(OLVGroup x) => ((Item)x.Items.First().RowObject).SeasonNumberAsInt ?? 0;

    private readonly SortOrder sortOrder;
}
