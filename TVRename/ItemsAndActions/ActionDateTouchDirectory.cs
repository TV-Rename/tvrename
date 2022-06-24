using Alphaleonis.Win32.Filesystem;
using System;

namespace TVRename;

internal abstract class ActionDateTouchDirectory : ActionDateTouch
{
    private readonly DirectoryInfo whereDirectory;

    protected ActionDateTouchDirectory(DirectoryInfo dir, DateTime date) : base(date)
    {
        whereDirectory = dir;
    }

    public override string Produces => whereDirectory.FullName;
    public override string ProgressText => whereDirectory.Name;

    public override bool SameAs(Item o)
    {
        return o is ActionDateTouchDirectory touch && touch.whereDirectory == whereDirectory;
    }

    public override ActionOutcome Go(TVRenameStats stats)
    {
        try
        {
            System.IO.Directory.SetLastWriteTimeUtc(whereDirectory.FullName, UpdateTime);
        }
        catch (UnauthorizedAccessException uae)
        {
            return new ActionOutcome(uae);
        }
        catch (Exception e)
        {
            return new ActionOutcome(e);
        }

        return ActionOutcome.Success();
    }

    public override IgnoreItem? Ignore => null;

    public override string? DestinationFolder => whereDirectory.FullName;
    public override string? DestinationFile => whereDirectory.Name;
    public override string? TargetFolder => whereDirectory.Name;

    public override int CompareTo(Item? o)
    {
        ActionDateTouchDirectory? nfo = o as ActionDateTouchDirectory;

        if (nfo?.whereDirectory is null)
        {
            return -1;
        }

        return string.Compare(whereDirectory.FullName, nfo.whereDirectory.FullName, StringComparison.Ordinal);
    }
}
