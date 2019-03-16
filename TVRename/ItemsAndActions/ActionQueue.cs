// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using System.Collections.Generic;
using System.Threading;

namespace TVRename
{
    public class ActionQueue
    {
        public readonly List<Action> Actions; // The contents of this queue
        public readonly int ParallelLimit; // Number of tasks in the queue than can be run at once
        public readonly string Name; // Name of this queue
        public readonly Semaphore Sem;

        // Position in the queue list of the next item to process
        public int ActionPosition { get; set; }

        public ActionQueue(string name, int parallelLimit)
        {
            Name = name;
            ParallelLimit = parallelLimit;
            Actions = new List<Action>();
            ActionPosition = 0;
            Sem =new Semaphore(parallelLimit,parallelLimit,Name); // allow up to numWorkers working at once
        }
    }
}
