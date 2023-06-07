//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Threading;

namespace TVRename;

public abstract class Action : Item // Something we can do
{
    private bool checkedItemValue;
    public sealed override bool CheckedItem
    {
        get => checkedItemValue;
        set
        {
            checkedItemValue = value;
            NotifyPropertyChanged();
        }
    }
    protected Action()
    {
        CheckedItem = true;
    }

    public virtual int Order => 0;
    public abstract string ProgressText { get; } // shortish text to display to user while task is running

    private double percent;

    public ActionOutcome Outcome
    {
        get => internalOutcome ?? ActionOutcome.NoOutcomeYet();
        set => internalOutcome = value;
    }

    private ActionOutcome? internalOutcome;

    public double PercentDone // 0.0 to 100.0
    {
        get => Outcome.Done ? 100.0 : percent;
        protected set => percent = value;
    }

    public abstract long SizeOfWork
    {
        get;
    } // for file copy/move, number of bytes in file.  for simple tasks, 1, or something proportional to how slow it is to copy files around.

    public abstract ActionOutcome Go(TVRenameStats stats, CancellationToken cancellationToken); // action the action.  do not return until done.  will be run in a dedicated thread.  if pause is set to true, stop working until it goes back to false

    public abstract string Produces { get; } //What does this action produce? typically a filename

    public virtual Item? Becomes() => null;

    public void ResetOutcome()
    {
        internalOutcome = null;
    }

    public abstract QueueName Queue();

    public enum QueueName
    {
        slowFileOperation,
        quickFileOperation,
        writeMetadata,
        download
    }
}
