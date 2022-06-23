//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using System.Collections.Generic;
using System.Threading;

namespace TVRename;

public class ActionQueue
{
    public readonly List<Action> Actions = new(); // The contents of this queue
    public readonly Semaphore Sem;

    private readonly int parallelThreadLimit; // Number of tasks in the queue than can be run at once
    private readonly string queueName; // Name of this queue
    private int actionPosition; // Position in the queue list of the next item to process

    public bool HasCapacity => actionPosition < Actions.Count;

    public Action NextAction() => Actions[actionPosition++];

    public ActionQueue(string name, int parallelLimit)
    {
        queueName = name;
        parallelThreadLimit = parallelLimit;
        actionPosition = 0;
        Sem = new Semaphore(parallelLimit, parallelLimit, name); // allow up to numWorkers working at once
    }

    public override string ToString() => $"'{queueName}' worker, with {parallelThreadLimit} threads.";
}
