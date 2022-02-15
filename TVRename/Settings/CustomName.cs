using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace TVRename
{
    public static class CustomName
    {
        [NotNull]
        public static string ReplaceYear(this string source, [CanBeNull] MediaConfiguration config)
        {
            return ReplaceYear(source, config?.Name);
        }

        [NotNull]
        private static string RemoveYear(this string source)
        {
            if (!source.HasValue())
            {
                return string.Empty;
            }
            const string PATTERN = @".*\ \(\d{4}\)";
            return Regex.IsMatch(source, PATTERN)
                ? source.Substring(0,source.Length-7)
                : source;
        }

        [NotNull]
        public static string ReplaceYear(this string source, string? showName)
        {
            if (!source.HasValue())
            {
                return string.Empty;
            }

            const string TAG = "{ShowNameNoYear}";

            return source.Contains(TAG, StringComparison.OrdinalIgnoreCase)
                ? source.ReplaceInsensitive(TAG, showName.RemoveYear())
                : source;
        }
    }
}
