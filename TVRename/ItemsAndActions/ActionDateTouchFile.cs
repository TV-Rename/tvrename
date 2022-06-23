using Alphaleonis.Win32.Filesystem;
using System;

namespace TVRename;

internal abstract class ActionDateTouchFile : ActionDateTouch
{
    protected ActionDateTouchFile(FileInfo f, DateTime date) : base(date)
    {
        WhereFile = f;
    }

    protected readonly FileInfo WhereFile;
    public override string Produces => WhereFile.FullName;
    public override string ProgressText => WhereFile.Name;
    public override IgnoreItem Ignore => new(WhereFile.FullName);
    public override string? DestinationFolder => WhereFile.DirectoryName;
    public override string? DestinationFile => WhereFile.Name;
    public override string? TargetFolder => WhereFile.DirectoryName;

    public override ActionOutcome Go(TVRenameStats stats)
    {
        try
        {
            ProcessFile(WhereFile, UpdateTime);
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

    private static void ProcessFile(FileInfo whereFile, DateTime updateTime)
    {
        bool priorFileReadonly = whereFile.IsReadOnly;
        if (priorFileReadonly)
        {
            whereFile.IsReadOnly = false;
        }

        File.SetLastWriteTimeUtc(whereFile.FullName, updateTime);
        if (priorFileReadonly)
        {
            whereFile.IsReadOnly = true;
        }
    }
}
