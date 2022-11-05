using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TVRename;

internal static class LinqHelper
{
    public static List<T> AsList<T>(this T? item) => item is null ? new List<T>() :new List<T> { item };

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

    public static bool HasAny<T>(this IEnumerable<T>? source)
        => source is not null && source.Any();

    public static void Swap<T>(
        this IList<T> list,
        int firstIndex,
        int secondIndex
    )
    {
        if (firstIndex == secondIndex)
        {
            return;
        }

        (list[firstIndex], list[secondIndex]) = (list[secondIndex], list[firstIndex]);
    }
}
