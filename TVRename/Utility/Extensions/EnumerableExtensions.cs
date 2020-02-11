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
using JetBrains.Annotations;

namespace TVRename
{
    public static class EnumerableExtensions
    {
        public static void AddNullableRange<T>(this List<T> source, [CanBeNull] IEnumerable<T> items)
        {
            if (items != null)
            {
                source.AddRange(items);
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
    }
}
