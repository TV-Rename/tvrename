//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename;

public static class EnumerableExtensions
{
    /// <summary>
    /// Gets all items for an enum type.
    /// </summary>
    public static IEnumerable<T> GetAllItems<T>() where T : struct
    {
        foreach (object item in Enum.GetValues(typeof(T)))
        {
            yield return (T)item;
        }
    }

    public static void AddNullableRange<T>(this List<T> source, IEnumerable<T>? items)
    {
        if (items != null)
        {
            source.AddRange(items);
        }
    }
    public static void RemoveNullableRange<T>(this List<T> source, IEnumerable<T>? items)
    {
        if (items != null)
        {
            source.RemoveRange(items);
        }
    }

    public static void RemoveRange<T>(this List<T> source, IEnumerable<T> items)
    {
        foreach (T item in items)
        {
            source.Remove(item);
        }
    }

    public static IList<T> DeepClone<T>(this IEnumerable<T> listToClone) where T : ICloneable
    {
        return [.. listToClone.Select(item => (T)item.Clone())];
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (T item in source) { action(item); }
    }

    public static IEnumerable<int> Keys(this IEnumerable<KeyValuePair<int, List<ProcessedEpisode>>> source)
    {
        return [.. source.Select(pair => pair.Key)];
    }

    public static int MaxOrDefault<T>(this IEnumerable<T> enumeration, Func<T, int> selector, int defaultValue)
    {
        IEnumerable<T> enumerable = [.. enumeration];
        return enumerable.Any() ? enumerable.Max(selector) : defaultValue;
    }

    public static int MinOrDefault<T>(this IEnumerable<T> enumeration, Func<T, int> selector, int defaultValue)
    {
        IEnumerable<T> enumerable = [.. enumeration];
        return enumerable.Any() ? enumerable.Min(selector) : defaultValue;
    }

    public static TProp? MinOrNull<TItem, TProp>(this IEnumerable<TItem> @this, Func<TItem, TProp> selector) where TProp : struct
    {
        IEnumerable<TItem> list = [.. @this];

        return list.Any() ? list.Min(selector) : null;
    }
    public static TProp? MaxOrNull<TItem, TProp>(this IEnumerable<TItem> @this, Func<TItem, TProp> selector) where TProp : struct
    {
        IEnumerable<TItem> list = [.. @this];
        return list.Any() ? list.Max(selector) : null;
    }
}
