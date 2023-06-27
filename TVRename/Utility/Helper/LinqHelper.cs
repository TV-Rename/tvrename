using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace TVRename;

internal static class LinqHelper
{
    public static bool In<T>(this T? item, params T[] items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        return items.Contains(item);
    }
    public static List<T> AsList<T>(this T? item) => item is null ? new List<T>() : new List<T> { item };

    public static Task ParallelForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> funcBody, int maxDoP = 4)
    {
        async Task AwaitPartition(IEnumerator<T> partition)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    await Task.Yield(); // prevents a sync/hot thread hangup
                    await funcBody(partition.Current).ConfigureAwait(false);
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

    public static bool HasAny<T>([NotNullWhen(true)] this IEnumerable<T>? source)
        => source is not null && source.Any();

    public static IEnumerable<T> WithMax<T>(this IEnumerable<T>? source, Func<T, int> countFunction)
    {
        return WithTarget(source, countFunction, x => x.Max(), (y, z) => y == z);
    }

    private static IEnumerable<T> WithTarget<T, TX>(this IEnumerable<T>? source, Func<T, TX> countFunction, Func<IEnumerable<TX>, TX> groupFunction, Func<TX, TX, bool> comparisionOperator)
    {
        if (source is null)
        {
            return new List<T>();
        }
        IEnumerable<T> enumerable = source as T[] ?? source.ToArray();
        if (!enumerable.Any())
        {
            return new List<T>();
        }
        TX targetValue = groupFunction(enumerable.Select(countFunction));
        return enumerable.Where(x => comparisionOperator(countFunction(x), targetValue));
    }

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
