//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//



namespace TVRename;

public static class SafeListExtensions
{
    public static SafeList<T> ToSafeList<T>(this IEnumerable<T> source)
    {
        SafeList<T> retValue = [];
        retValue.AddNullableRange(source);
        return retValue;
    }
}
