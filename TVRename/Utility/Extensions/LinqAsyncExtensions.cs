//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
namespace TVRename;

public static class LinqAsyncExtensions
{
    public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Task<TResult>> mapper)
    {
        // 1. Project items into tasks pairing the item with its async boolean outcome
        var evaluationTasks = source.Select(item => mapper(item));

        // 2. Await all conditions to resolve concurrently
        var evaluations = await Task.WhenAll(evaluationTasks);

        // 3. Return the results from completed tasks
        return evaluations.ToList();
    }

    public static async Task<IEnumerable<TSource>> WhereAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, Task<bool>> filter)
    {
        // 2. Await all conditions to resolve concurrently
        var evaluations = await Task.WhenAll(source.Select(p => filter(p)));

        // 3. Zip results with items and perform LINQ filter on the results
        return source.Zip(evaluations, (item, result) => new { item, result })
            .Where(x => x.result)
            .Select(x => x.item);
    }


    public static async Task<bool> AllAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, Task<bool>> filter)
    {
        // 1. Project items into tasks pairing the item with its async boolean outcome
        IEnumerable<Task<bool>> evaluationTasks = source.Select(item => filter(item));

        // 2. Await all conditions to resolve concurrently
        bool[] evaluations = await Task.WhenAll(evaluationTasks);

        // 3. Perform a standard synchronous LINQ filter on the results
        return evaluations.All(x => x);
    }

    public static async Task<bool> AnyAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, Task<bool>> filter)
    {
        // 1. Project items into tasks pairing the item with its async boolean outcome
        IEnumerable<Task<bool>> evaluationTasks = source.Select(item => filter(item));

        // 2. Await all conditions to resolve concurrently
        bool[] evaluations = await Task.WhenAll(evaluationTasks);

        // 3. Perform a standard synchronous LINQ filter on the results
        return evaluations.Any(x => x);
    }

    public static async Task<TSource> FirstAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, Task<bool>> filter)
    {
        // 2. Await all conditions to resolve concurrently
        var evaluations = await Task.WhenAll(source.Select(item => filter(item)));

        // 3. Zip results with items and return the first matching item
        return source.Zip(evaluations, (item, result) => new { item, result })
            .First(x => x.result)
            .item;
    }
}
