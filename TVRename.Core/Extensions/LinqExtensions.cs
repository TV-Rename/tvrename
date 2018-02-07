using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TVRename.Core.Extensions
{
    /// <summary>
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
        /// <param name="continueOnCapturedContext"><see langword="true" /> to attempt to marshal the continuation back to the original context captured; otherwise, <see langword="false" />.</param>
        /// <returns></returns>
        public static Task ForEachAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, Task> action, int maxDegreeOfParallelism = 5, bool continueOnCapturedContext = false)
        {
            SemaphoreSlim throttler = new SemaphoreSlim(maxDegreeOfParallelism, maxDegreeOfParallelism);

            IEnumerable<Task> tasks = source.Select(async input =>
            {
                await throttler.WaitAsync().ConfigureAwait(continueOnCapturedContext);

                try
                {
                    await action(input).ConfigureAwait(continueOnCapturedContext);
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
