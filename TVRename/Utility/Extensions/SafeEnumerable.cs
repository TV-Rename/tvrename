//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

/// <summary>
/// A thread-safe IEnumerable implementation
/// See: http://www.codeproject.com/KB/cs/safe_enumerable.aspx
/// </summary>
public class SafeEnumerable<T>(IEnumerable<T> inner, object @lock) : IEnumerable<T>
{
    private readonly IEnumerable<T> inner = inner;
    private readonly object @lock = @lock;

    public IEnumerator<T> GetEnumerator() => new SafeEnumerator<T>(inner.GetEnumerator(), @lock);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
