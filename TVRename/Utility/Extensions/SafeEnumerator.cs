using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace TVRename;

/// <summary>
/// A thread-safe IEnumerator implementation.
/// See: http://www.codeproject.com/KB/cs/safe_enumerable.aspx
/// </summary>
public class SafeEnumerator<T> : IEnumerator<T>
{
    // this is the (thread-unsafe)
    // enumerator of the underlying collection
    private readonly IEnumerator<T> inner;

    // this is the object we shall lock on.
    private readonly object @lock;

    public SafeEnumerator(IEnumerator<T> inner, object @lock)
    {
        this.inner = inner;
        this.@lock = @lock;

        // entering lock in constructor
        Monitor.Enter(this.@lock);
    }

    public T Current => inner.Current;

    object? IEnumerator.Current => Current;

    public void Dispose()
    {
        // .. and exiting lock on Dispose()
        // This will be called when foreach loop finishes
        Monitor.Exit(@lock);
    }

    /// <remarks>
    /// we just delegate actual implementation
    /// to the inner enumerator, that actually iterates
    /// over some collection
    /// </remarks>
    public bool MoveNext() => inner.MoveNext();

    public void Reset() => inner.Reset();
}
