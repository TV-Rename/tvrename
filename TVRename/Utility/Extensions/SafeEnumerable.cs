using System.Collections;
using System.Collections.Generic;

namespace TVRename;

/// <summary>
/// A thread-safe IEnumerable implementation
/// See: http://www.codeproject.com/KB/cs/safe_enumerable.aspx
/// </summary>
public class SafeEnumerable<T> : IEnumerable<T>
{
    private readonly IEnumerable<T> inner;
    private readonly object @lock;

    public SafeEnumerable(IEnumerable<T> inner, object @lock)
    {
        this.@lock = @lock;
        this.inner = inner;
    }

    public IEnumerator<T> GetEnumerator() => new SafeEnumerator<T>(inner.GetEnumerator(), @lock);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
