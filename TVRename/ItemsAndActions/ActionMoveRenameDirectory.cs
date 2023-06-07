using Alphaleonis.Win32.Filesystem;
using System;
using System.Linq;
using System.Threading;

namespace TVRename;

public class ActionMoveRenameDirectory : ActionFileOperation
{
    private readonly string sourceFolder;
    private readonly string targetFolder;

    public ActionMoveRenameDirectory(string sourceFolder, string targetFolder, MovieConfiguration movie)
    {
        this.sourceFolder = sourceFolder;
        this.targetFolder = targetFolder;
        Movie = movie;
    }
    public ActionMoveRenameDirectory(string sourceFolder, string targetFolder, ProcessedEpisode episode)
    {
        this.sourceFolder = sourceFolder;
        this.targetFolder = targetFolder;
        Episode = episode;
    }
    public override string TargetFolder => targetFolder;

    public override string ScanListViewGroup => "lvgActionMove";

    public override int IconNumber => 4;
    public override QueueName Queue() => QueueName.quickFileOperation;

    public override IgnoreItem? Ignore => null;

    public override int CompareTo(Item? o)
    {
        if (o is not ActionMoveRenameDirectory cmr)
        {
            return -1;
        }

        if (targetFolder == cmr.targetFolder)
        {
            return string.Compare(sourceFolder, cmr.sourceFolder, StringComparison.Ordinal);
        }

        return string.Compare(targetFolder, cmr.targetFolder, StringComparison.Ordinal);
    }

    public override bool SameAs(Item o) => o is ActionMoveRenameDirectory amd && amd.targetFolder == targetFolder && amd.sourceFolder == sourceFolder;

    public override string Name => "Rename Directory";

    public override string DestinationFolder => targetFolder;

    public override string DestinationFile => targetFolder;

    public override string ProgressText => sourceFolder;

    public override long SizeOfWork => 10;

    public override ActionOutcome Go(TVRenameStats stats, CancellationToken cancellationToken)
    {
        DirectoryInfo source = new(sourceFolder);
        DirectoryInfo target = new(targetFolder);

        if (target.Exists && source.Exists && !target.EnumerateFiles().Any())
        {
            try
            {
                target.Delete();
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                LOGGER.Info($"Testing {target.FullName} but it has already been removed - Job Done!");
            }
            catch (Exception e)
            {
                return new ActionOutcome(e);
            }

            try
            {
                source.MoveTo(targetFolder, MoveOptions.CopyAllowed | MoveOptions.ReplaceExisting);
                LOGGER.Info($"Moved whole directory {sourceFolder} to {targetFolder}");

                return ActionOutcome.Success();
            }
            catch (Exception e)
            {
                return new ActionOutcome(e);
            }
        }

        try
        {
            if (target.Exists)
            {
                if (target.GetFiles().Any(x => x.IsMovieFile()))
                {
                    throw new ActionFailedException("Target location has movie files - not copying just in case");
                }

                //Copy files
                foreach (FileInfo file in source.EnumerateFiles())
                {
                    string destFile = Path.Combine(targetFolder, file.Name);
                    if (!File.Exists(destFile))
                    {
                        file.MoveTo(destFile, MoveOptions.CopyAllowed);

                        LOGGER.Info($"Moved {file.FullName} to {destFile}");
                    }
                }

                if (Directory.IsEmpty(source.FullName))
                {
                    source.Delete(false);
                    LOGGER.Info($"Deleted empty directory {source.FullName}");
                }
                return ActionOutcome.Success();
            }

            source.MoveTo(targetFolder, MoveOptions.CopyAllowed);
            LOGGER.Info($"Moved whole directory {sourceFolder} to {targetFolder}");
            return ActionOutcome.Success();
        }
        catch (Exception e)
        {
            return new ActionOutcome(e);
        }
    }

    public override string Produces => targetFolder;

    public override string SourceDetails => sourceFolder;
}
