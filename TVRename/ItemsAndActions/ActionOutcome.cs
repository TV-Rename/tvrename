using JetBrains.Annotations;
using System;

namespace TVRename
{
    public class ActionOutcome
    {
        [NotNull]
        public static ActionOutcome NoOutcomeYet() => new ActionOutcome {Done = false, Error = false };

        [NotNull]
        public static ActionOutcome Success() => new ActionOutcome {Done = true, Error = false };

        private ActionOutcome()
        {
        }

        public ActionOutcome(Exception e)
        {
            LastError = e;
            Error = true;
            Done = true;
        }

        public ActionOutcome(string errorText) : this(new Exception(errorText))
        {
        }

        public bool Error { get; private set; } // Error state, after trying to do work?

        public bool Done
        {
            get;
            private set;
        } // All work has been completed for this item, and can be removed from to-do list.  set to true on completion, even on error.

        public Exception? LastError { get; }

        [NotNull]
        public static ActionOutcome CompleteFail() => new ActionOutcome("Complete Fail");
    }
}
