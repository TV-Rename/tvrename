//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Threading;

#pragma warning disable CS0162

// Helpful functions and classes

namespace TVRename;

public class ThreadSafeCounter
{
    // High-performance atomic operations require a primitive field
    private int _value;

    /// <summary>
    /// Initializes a new instance of the counter starting at 0.
    /// </summary>
    public ThreadSafeCounter() : this(0) { }

    /// <summary>
    /// Initializes a new instance of the counter with a specific starting value.
    /// </summary>
    public ThreadSafeCounter(int initialValue)
    {
        _value = initialValue;
    }

    /// <summary>
    /// Gets the current value safely.
    /// </summary>
    public int Value => _value;

    /// <summary>
    /// Increments the counter by 1 and returns the new value.
    /// </summary>
    public int Increment()
    {
        return Interlocked.Increment(ref _value);
    }

    /// <summary>
    /// Resets the counter back to a specific value safely.
    /// </summary>
    public void Reset(int newValue = 0)
    {
        Interlocked.Exchange(ref _value, newValue);
    }

    internal void Increment(int additionalValue)
    {
        Interlocked.Add(ref _value, additionalValue);
    }
}
