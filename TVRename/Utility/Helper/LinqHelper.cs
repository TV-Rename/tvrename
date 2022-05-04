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
        [NotNull]
        public static List<T> AsList<T>([CanBeNull] this T item) => new() { item };

        [NotNull]
        public static Task ParallelForEachAsync<T>([NotNull] this IEnumerable<T> source, Func<T, Task> funcBody, int maxDoP = 4)
        {
            async Task AwaitPartition([NotNull] IEnumerator<T> partition)
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

        public static bool HasAny<T>([CanBeNull] this IEnumerable<T> source)
            => source is not null && source.Any();
    }
}
