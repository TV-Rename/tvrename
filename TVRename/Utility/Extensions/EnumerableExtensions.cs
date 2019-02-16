// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;

namespace TVRename
{
    public static class EnumerableExtensions
    {
        public static void AddNullableRange<T>(this List<T> source, IEnumerable<T> items)
        {
            if (items != null) source.AddRange(items);
        }
    }
}
