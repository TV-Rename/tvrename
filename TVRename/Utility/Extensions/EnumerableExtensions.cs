//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename
{
    public static class EnumerableExtensions
    {
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

        public static void RemoveRange<T>(this List<T> source, [NotNull] IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                source.Remove(item);
            }
        }

        [NotNull]
        public static IList<T> DeepClone<T>([NotNull] this IEnumerable<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        [NotNull]
        public static IList<T> Clone<T>([NotNull] this IEnumerable<T> listToClone)
        {
            return listToClone.Select(item => item).ToList();
        }

        public static void ForEach<T>([NotNull] this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source) { action(item); }
        }

        [NotNull]
        public static IEnumerable<int> Keys([NotNull] this IEnumerable<KeyValuePair<int, List<ProcessedEpisode>>> source)
        {
            return source.Select(pair => pair.Key).ToList();
        }

        public static int MaxOrDefault<T>([NotNull] this IEnumerable<T> enumeration, Func<T, int> selector, int defaultValue)
        {
            IEnumerable<T> enumerable = enumeration.ToList();
            return enumerable.Any() ? enumerable.Max(selector) : defaultValue;
        }

        public static int MinOrDefault<T>([NotNull] this IEnumerable<T> enumeration, Func<T, int> selector, int defaultValue)
        {
            IEnumerable<T> enumerable = enumeration.ToList();
            return enumerable.Any() ? enumerable.Min(selector) : defaultValue;
        }
    }
}
