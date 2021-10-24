using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace TVRename
{
    internal static class LinqHelper
    {
        public static List<T> AsList<T>([CanBeNull] this T item) => new() { item };

        public static Task ParallelForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> funcBody, int maxDoP = 4)
        {
            async Task AwaitPartition(IEnumerator<T> partition)
            {
                using (partition)
                {
                    while (partition.MoveNext())
                    {
                        await Task.Yield(); // prevents a sync/hot thread hangup
                        await funcBody(partition.Current);
                    }
                }
            }

            return Task.WhenAll(
                Partitioner
                    .Create(source)
                    .GetPartitions(maxDoP)
                    .AsParallel()
                    .Select(AwaitPartition));
        }
    }
}
