using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TVRename.Core.Extensions
{    /// <summary>
     /// Linq and enumerable type extention methods.
     /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Performs the specified action on each element of the <see cref="IEnumerable{T}"/> asynchronously, limiting parallelism.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="action">The action to perform asynchronously on each element.</param>
        /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism.</param>
        /// <returns></returns>
        public static Task ForEachAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, Task> action, int maxDegreeOfParallelism = 5)
        {
            SemaphoreSlim throttler = new SemaphoreSlim(maxDegreeOfParallelism, maxDegreeOfParallelism);

            IEnumerable<Task> tasks = source.Select(async input =>
            {
                await throttler.WaitAsync().ConfigureAwait(false);

                try
                {
                    await action(input).ConfigureAwait(false);
                }
                finally
                {
                    throttler.Release();
                }
            });

            return Task.WhenAll(tasks);
        }
    }
}
