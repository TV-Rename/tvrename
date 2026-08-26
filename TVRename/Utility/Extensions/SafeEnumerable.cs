using System.Collections;
using System.Collections.Generic;

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
