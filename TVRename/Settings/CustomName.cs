//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

public static class CustomName
{
    public static string ReplaceYear(this string source, MediaConfiguration? config)
        => ReplaceYear(source, config?.Name ?? string.Empty);

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
