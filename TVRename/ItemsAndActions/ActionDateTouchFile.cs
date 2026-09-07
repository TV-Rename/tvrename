using Alphaleonis.Win32.Filesystem;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TVRename;

internal abstract class ActionDateTouchFile(FileInfo f, DateTime date) : ActionDateTouch(date)
{
    protected readonly FileInfo WhereFile = f;
    public override string Produces => WhereFile.FullName;
    public override string ProgressText => WhereFile.Name;
    public override IgnoreItem Ignore => new(WhereFile.FullName);
    public override string? DestinationFolder => WhereFile.DirectoryName;
    public override string? DestinationFile => WhereFile.Name;
    public override string? TargetFolder => WhereFile.DirectoryName;

    public override async Task<ActionOutcome> GoAsync(TVRenameStats stats, CancellationToken cancellationToken)
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
