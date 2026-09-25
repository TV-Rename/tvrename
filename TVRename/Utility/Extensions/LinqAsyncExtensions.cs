//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TVRename;

public static class LinqAsyncExtensions
{
    public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Task<TResult>> mapper)
    {
        // 1. Project items into tasks pairing the item with its async boolean outcome
        var evaluationTasks = source.Select(async item => mapper(item));

        // 2. Await all conditions to resolve concurrently
        var evaluations = await Task.WhenAll(evaluationTasks);

        // 3. Perform a standard synchronous LINQ filter on the results
        return evaluations.Select(x => x.Result).ToList();
    }

    public static async Task<IEnumerable<TSource>> WhereAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, Task<bool>> filter)
    {
        // 1. Project items into tasks pairing the item with its async boolean outcome
        var evaluationTasks = source.Select(item => new
        {
            Item = item,
            FilterTask = filter(item)
        });

        // 2. Await all conditions to resolve concurrently
        var evaluations = await Task.WhenAll(evaluationTasks.Select(p => p.FilterTask));

        // 3. Perform a standard synchronous LINQ filter on the results
        return evaluationTasks.Where(x => x.FilterTask.Result).Select(x => x.Item);
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
        // 1. Project items into tasks pairing the item with its async boolean outcome
        var evaluationTasks = source.Select(item => new
        {
            Item = item,
            FilterTask = filter(item)
        });

        // 2. Await all conditions to resolve concurrently
        var evaluations = await Task.WhenAll(evaluationTasks.Select(p => p.FilterTask));

        // 3. Perform a standard synchronous LINQ filter on the results
        return evaluationTasks.First(x => x.FilterTask.Result).Item;
    }
}
