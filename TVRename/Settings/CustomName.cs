using System;

namespace TVRename;

public static class CustomName
{
    public static string ReplaceYear(this string source, MediaConfiguration? config)
        => ReplaceYear(source, config?.Name??string.Empty);

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
