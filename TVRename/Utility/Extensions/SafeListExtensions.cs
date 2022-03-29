using System.Collections.Generic;
using JetBrains.Annotations;

namespace TVRename
{
    public static class SafeListExtensions
    {
        [NotNull]
        public static SafeList<T> ToSafeList<T>(this IEnumerable<T> source)
        {
            SafeList<T> retValue = new();
            retValue.AddNullableRange(source);
            return retValue;
        }
    }
}