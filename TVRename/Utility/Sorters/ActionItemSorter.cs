//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;

namespace TVRename;

public abstract class ListSorter<T> : System.Collections.Generic.IComparer<T>
{
    public int Compare(T? x, T? y)
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

    protected abstract int CompareItems(T item, T item1);
}
public class DefaultActionItemSorter: ListSorter<Item>
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
            UpdateMediaFileTitle=>20,
            UpdateMediaFileDescription => 21,
            UpdateMediaFileComment => 22,
            UpdateMediaFileSubtitle => 23,
            UpdateMediaFileYear => 24,
            UpdateMediaFileGenres => 25,
            ActionTRemove => 30,
            ActionUnArchive => 31,
            ActionChangeLibraryRemoveMovie =>32,
            ActionChangeLibraryRemoveShow =>33,
            _ => throw new NotSupportedException()
        };
    }
}

public abstract class ActionItemStringSorter<T> : ListSorter<T>
{
    protected override int CompareItems(T x, T y) => string.Compare(GetString(x), GetString(y), StringComparison.CurrentCultureIgnoreCase);

    protected abstract string GetString(T x);
}

public class ActionItemNameSorter : ActionItemStringSorter<Item>
{
    protected override string GetString(Item x) => x.SeriesName;
}
public class ActionItemDateSorter : ListSorter<Item>
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
public class ActionItemFilenameSorter : ActionItemStringSorter<Item>
{
    protected override string GetString(Item x) => x.DestinationFile ?? string.Empty;
}
public class ActionItemFolderSorter : ActionItemStringSorter<Item>
{
    protected override string GetString(Item x) => x.DestinationFolder ?? string.Empty;
}
public class ActionItemSourceSorter : ActionItemStringSorter<Item>
{
    protected override string GetString(Item x) => x.SourceDetails;
}
public class ActionItemErrorsSorter : ActionItemStringSorter<Item>
{
    protected override string GetString(Item x) =>x.ErrorText ?? string.Empty;
}
public class ActionItemSeasonSorter : ListSorter<Item>
{
    protected override int CompareItems(Item x, Item y) => GetValue(x) - GetValue(y);
    private static int GetValue(Item x) => x.SeasonNumberAsInt ?? 0;
}
public class ActionItemEpisodeSorter : ListSorter<Item>
{
    protected override int CompareItems(Item x, Item y) => GetValue(x) - GetValue(y);
    private static int GetValue(Item x) => x.EpisodeNumber ?? 0;
}

public class DefaultProcessedEpisodeSorter : ListSorter<ProcessedEpisode>
{
    #region IComparer<ProcessedEpisode> Members

    protected override int CompareItems(ProcessedEpisode x, ProcessedEpisode y)
    {
        DateTime? XairDate = x.AirDate;
        DateTime? YairDate = y.AirDate;
        if (XairDate == null && YairDate == null)
        {
            return x.SeriesId.CompareTo(y.SeriesId);
        }
        if (XairDate == null)
        {
            return -1;
        }
        if (YairDate == null)
        {
            return 1;
        }   
        return DateTime.Compare(XairDate.Value, YairDate.Value);
    }

    #endregion IComparer<ProcessedEpisode> Members
}


public class EpisodeNetworkSorter : EpisodeStringSorter
{
    protected override string GetString(ProcessedEpisode x) => x.Network;
}

public class EpisodeSeriesSorter : EpisodeStringSorter
{
    protected override string GetString(ProcessedEpisode x) => x.SeriesName;
}
public class EpisodeNameSorter : EpisodeStringSorter
{
    protected override string GetString(ProcessedEpisode x) => x.Name;
}

public class EpisodeLengthSorter : EpisodeStringSorter
{
    protected override string GetString(ProcessedEpisode x) => x.Length;
}
public class EpisodeDateSorter : ListSorter<ProcessedEpisode>
{
    protected override int CompareItems(ProcessedEpisode x, ProcessedEpisode y)
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

public class SeasonNumberSorter : ListSorter<ProcessedEpisode>
{
    protected override int CompareItems(ProcessedEpisode x, ProcessedEpisode y) => GetValue(x) - GetValue(y);
    private static int GetValue(ProcessedEpisode x) => x.AppropriateSeasonNumber;
}
public class EpisodeNumberSorter : ListSorter<ProcessedEpisode>
{
    protected override int CompareItems(ProcessedEpisode x, ProcessedEpisode y) => GetValue(x) - GetValue(y);
    private static int GetValue(ProcessedEpisode x) => x.AppropriateEpNum;
}
public abstract class EpisodeStringSorter : ListSorter<ProcessedEpisode>
{
    protected override int CompareItems(ProcessedEpisode x, ProcessedEpisode y) => string.Compare(GetString(x), GetString(y), StringComparison.CurrentCultureIgnoreCase);

    protected abstract string GetString(ProcessedEpisode x);
}
