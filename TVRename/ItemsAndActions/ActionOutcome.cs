using System;

namespace TVRename;

public class ActionOutcome
{
    public static ActionOutcome NoOutcomeYet() => new() {Done = false, Error = false };

    public static ActionOutcome Success() => new() {Done = true, Error = false };

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

    public static ActionOutcome CompleteFail() => new("Complete Fail");
}
