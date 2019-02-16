// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

namespace TVRename
{
    public abstract class Action : Item // Something we can do
    {
        public ItemMissing UndoItemMissing; //Item to revert to if we have to cancel this action

        public abstract string Name { get; } // Name of this action, e.g. "Copy", "Move", "Download"

        public bool
            Done
        {
            get;
            protected set;
        } // All work has been completed for this item, and can be removed from to-do list.  set to true on completion, even on error.

        public bool Error { get; protected set; } // Error state, after trying to do work?
        public abstract string ProgressText { get; } // shortish text to display to user while task is running

        private double percent;

        public double PercentDone // 0.0 to 100.0
        {
            get => Done ? 100.0 : percent;
            protected set => percent = value;
        }

        public abstract long
            SizeOfWork
        {
            get;
        } // for file copy/move, number of bytes in file.  for simple tasks, 1, or something proportional to how slow it is to copy files around.

        public abstract bool
            Go(ref bool pause,
                TVRenameStats stats); // action the action.  do not return until done.  will be run in a dedicated thread.  if pause is set to true, stop working until it goes back to false        

        public abstract string Produces { get; } //What does this action produce? typically a filename
    }
}
