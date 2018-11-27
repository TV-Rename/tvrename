// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;

namespace TVRename
{
    public static class IEnumerableExtensions
    {
        public static void AddNullableRange<T>(this List<T> source, IEnumerable<T> items)
        {
            if (items != null) source.AddRange(items);
        }
    }
}
