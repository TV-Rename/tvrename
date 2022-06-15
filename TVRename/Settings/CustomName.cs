using System;
using JetBrains.Annotations;

namespace TVRename
{
    public static class CustomName
    {
        [NotNull]
        public static string ReplaceYear(this string source, [CanBeNull] MediaConfiguration config)
            => ReplaceYear(source, config?.Name??string.Empty);

        [NotNull]
        public static string ReplaceYear(this string source, string showName)
        {
            if (!source.HasValue())
            {
                return string.Empty;
            }

            const string TAG = "{ShowNameNoYear}";

            return source.Contains(TAG, StringComparison.OrdinalIgnoreCase)
                ? source.ReplaceInsensitive(TAG, showName.RemoveBracketedYearFromEnd())
                : source;
        }
    }
}
