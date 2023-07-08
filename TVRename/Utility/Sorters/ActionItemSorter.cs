//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;

namespace TVRename;

public abstract class ActionItemSorter : System.Collections.Generic.IComparer<Item>
{
    public int Compare(Item? x, Item? y)
    {
        if (x is null)
        {
            return -1;
        }

        if (y is null)
        {
            return 1;
        }

        return CompareItems(x,y);
    }

    protected abstract int CompareItems(Item item, Item item1);
}
public class DefaultActionItemSorter:ActionItemSorter
{
    #region IComparer<Item> Members

    protected override int CompareItems(Item x, Item y)
        => TypeNumber(x) == TypeNumber(y) ? x.CompareTo(y) : TypeNumber(x) - TypeNumber(y);

    #endregion IComparer<Item> Members

    private static int TypeNumber(Item a)
    {
        return a switch
        {
            ShowItemMissing => 1,
            MovieItemMissing => 2,
            ShowSeasonMissing => 3,
            ActionCopyMoveRename => 4,
            ActionMoveRenameDirectory => 5,
            ActionTDownload => 6,
            ActionDownloadImage => 7,
            ActionMede8erViewXML => 8,
            ActionMede8erXML => 9,
            ActionNfo => 10,
            ActionPyTivoMeta => 11,
            ActionWdtvMeta => 12,
            ItemDownloading => 13,
            ActionDeleteFile => 14,
            ActionDeleteDirectory => 15,
            ActionDateTouchEpisode => 16,
            ActionDateTouchSeason => 17,
            ActionDateTouchMedia => 18,
            ActionDateTouchMovie => 19,
            ActionTRemove => 20,
            ActionUnArchive => 21,
            ActionChangeLibraryRemoveMovie =>22,
            ActionChangeLibraryRemoveShow =>23,
            _ => throw new NotSupportedException()
        };
    }
}

public abstract class ActionItemStringSorter : ActionItemSorter
{
    protected override int CompareItems(Item x, Item y) => string.Compare(GetString(x),GetString(y),StringComparison.CurrentCultureIgnoreCase);

    protected abstract string GetString(Item x);
}

public class ActionItemNameSorter : ActionItemStringSorter
{
    protected override string GetString(Item x) => x.SeriesName;
}
public class ActionItemDateSorter : ActionItemSorter
{
    protected override int CompareItems(Item x, Item y)
    {
        DateTime? x1 = x.AirDate;
        DateTime? y1 = y.AirDate;

        // Handle nulls. Null values come last
        bool xIsNull = x1 == null;
        bool yIsNull = y1 == null;

        if (x1 != null && y1 != null)
        {
            return DateTime.Compare(x1.Value, y1.Value);
        }

        if (xIsNull && yIsNull)
        {
            return 0;
        }

        return xIsNull ? -1 : 1;
    }
}
public class ActionItemFilenameSorter : ActionItemStringSorter
{
    protected override string GetString(Item x) => x.DestinationFile ?? string.Empty;
}
public class ActionItemFolderSorter : ActionItemStringSorter
{
    protected override string GetString(Item x) => x.DestinationFolder ?? string.Empty;
}
public class ActionItemSourceSorter : ActionItemStringSorter
{
    protected override string GetString(Item x) => x.SourceDetails;
}
public class ActionItemErrorsSorter : ActionItemStringSorter
{
    protected override string GetString(Item x) =>x.ErrorText ?? string.Empty;
}
public class ActionItemSeasonSorter : ActionItemSorter
{
    protected override int CompareItems(Item x, Item y) => GetValue(x) - GetValue(y);
    private static int GetValue(Item x) => x.SeasonNumberAsInt ?? 0;
}
public class ActionItemEpisodeSorter : ActionItemSorter
{
    protected override int CompareItems(Item x, Item y) => GetValue(x) - GetValue(y);
    private static int GetValue(Item x) => x.EpisodeNumber ?? 0;
}
