using System.Collections.Generic;

namespace TVRename
{
    public class ActionQueue
    {
        public readonly List<Action> Actions; // The contents of this queue
        public readonly int ParallelLimit; // Number of tasks in the queue than can be run at once
        public readonly string Name; // Name of this queue
        public int ActionPosition; // Position in the queue list of the next item to process

        public ActionQueue(string name, int parallelLimit)
        {
            Name = name;
            ParallelLimit = parallelLimit;
            Actions = new List<Action>();
            ActionPosition = 0;
        }
    }
}