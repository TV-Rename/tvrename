//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace TVRename;

public class ActionQueue
{
    private readonly List<Action> actions; // The contents of this queue
    public readonly Semaphore Sem;

    private readonly int parallelThreadLimit; // Number of tasks in the queue than can be run at once
    private readonly string queueName; // Name of this queue
    private int actionPosition; // Position in the queue list of the next item to process

    public bool HasCapacity => actionPosition < actions.Count;

    public Action NextAction() => actions[actionPosition++];

    /// <exception cref="ArgumentException"><paramref name="parallelLimit" /> is greater than <paramref name="parallelLimit" />.
    /// -or-
    /// .NET Framework only: <paramref name="name" /> is longer than MAX_PATH (260 characters).</exception>
    /// <exception cref="IOException"><paramref name="name" /> is invalid. This can be for various reasons, including some restrictions that may be placed by the operating system, such as an unknown prefix or invalid characters. Note that the name and common prefixes "Global" and "Local" are case-sensitive.
    /// -or-
    /// There was some other error. The HResult property may provide more information.</exception>
    /// <exception cref="DirectoryNotFoundException">Windows only: <paramref name="name" /> specified an unknown namespace. See Object Names for more information.</exception>
    /// <exception cref="UnauthorizedAccessException">The named semaphore exists and has access control security, and the user does not have <see cref="System.Security.AccessControl.SemaphoreRights.FullControl" />.</exception>
    /// <exception cref="WaitHandleCannotBeOpenedException">A synchronization object with the provided <paramref name="name" /> cannot be created. A synchronization object of a different type might have the same name.</exception>
    public ActionQueue(string name, int parallelLimit, IEnumerable<Action> actions)
    {
        queueName = name;
        parallelThreadLimit = parallelLimit;
        actionPosition = 0;
        Sem = new Semaphore(parallelLimit, parallelLimit, name); // allow up to numWorkers working at once
        this.actions = [.. actions.OrderBy(a=>a.Order)];
    }

    public override string ToString() => $"'{queueName}' worker, with {parallelThreadLimit} threads.";
}
