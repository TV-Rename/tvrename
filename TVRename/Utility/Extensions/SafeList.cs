using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace TVRename
{
    /// <summary>
    /// A thread-safe IList implementation using the custom SafeEnumerator class
    /// See: http://www.codeproject.com/KB/cs/safe_enumerable.aspx
    /// </summary>
    public class SafeList<T> : IList<T>
    {
        // the (thread-unsafe) collection that actually stores everything
        private readonly List<T> inner;

        // this is the object we shall lock on.
        private readonly object @lock = new object();

        public SafeList()
        {
            inner = new List<T>();
        }

        public int Count
        {
            get
            {
                lock (@lock)
                {
                    return inner.Count;
                }
            }
        }

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get
            {
                lock (@lock)
                {
                    return inner[index];
                }
            }
            set
            {
                lock (@lock)
                {
                    inner[index] = value;
                }
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            lock (@lock)
            {
                // instead of returning an usafe enumerator,
                // we wrap it into our thread-safe class
                return new SafeEnumerator<T>(inner.GetEnumerator(), @lock);
            }
        }

        /// <remarks>
        /// To be actually thread-safe, our collection must be locked on all other operations
        /// </remarks>
        public void Add(T item)
        {
            lock (@lock)
            {
                inner.Add(item);
            }
        }

        public void Clear()
        {
            lock (@lock)
            {
                inner.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (@lock)
            {
                return inner.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (@lock)
            {
                inner.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            lock (@lock)
            {
                return inner.Remove(item);
            }
        }

        public IEnumerator GetEnumerator()
        {
            lock (@lock)
            {
                return new SafeEnumerator<T>(inner.GetEnumerator(), @lock);
            }
        }

        public int IndexOf(T item)
        {
            lock (@lock)
            {
                return inner.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (@lock)
            {
                inner.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (@lock)
            {
                inner.RemoveAt(index);
            }
        }

        public ReadOnlyCollection<T> AsReadOnly()
        {
            lock (@lock)
            {
                return new ReadOnlyCollection<T>(this);
            }
        }

        public void ForEach([NotNull] Action<T> action)
        {
            lock (@lock)
            {
                foreach (T item in inner)
                {
                    action(item);
                }
            }
        }

        public bool Exists([NotNull] Predicate<T> match)
        {
            lock (@lock)
            {
                foreach (T item in inner)
                {
                    if (match(item))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
