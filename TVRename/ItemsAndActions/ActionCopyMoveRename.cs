using JetBrains.Annotations;
using System.Diagnostics;
using System.Security.AccessControl;

namespace TVRename
{
    using Alphaleonis.Win32.Filesystem;
    using System;

    public class ActionCopyMoveRename : ActionFileOperation
    {
        #region Op enum

        public enum Op
        {
            copy,
            move,
            rename
        }

        #endregion Op enum

        public readonly FileInfo From;
        public Op Operation;
        public readonly FileInfo To;
        private readonly TVDoc doc;

        public ActionCopyMoveRename(Op operation, FileInfo from, FileInfo to, ProcessedEpisode? ep, bool doTidyup, ItemMissing? undoItem, TVDoc tvDoc)
        {
            Tidyup = doTidyup ? TVSettings.Instance.Tidyup : null;
            PercentDone = 0;
            Episode = ep;
            Operation = operation;
            From = from;
            To = to;
            UndoItemMissing = undoItem;
            doc = tvDoc;
        }

        public ActionCopyMoveRename(FileInfo from, FileInfo to, ProcessedEpisode? ep, TVDoc tvDoc) :
            this(TVSettings.Instance.LeaveOriginals ? Op.copy : Op.move, from, to, ep, true, null, tvDoc)
        { }

        public ActionCopyMoveRename(FileInfo from, FileInfo to, MovieConfiguration ep, TVDoc tvDoc) :
    this(TVSettings.Instance.LeaveOriginals ? Op.copy : Op.move, from, to, ep, true, null, tvDoc)
        { }

        public ActionCopyMoveRename(Op operation, FileInfo from, FileInfo to, MovieConfiguration mc, bool doTidyup, ItemMissing? undoItem, TVDoc tvDoc)
        {
            Tidyup = doTidyup ? TVSettings.Instance.Tidyup : null;
            PercentDone = 0;
            Movie = mc;
            Operation = operation;
            From = from;
            To = to;
            UndoItemMissing = undoItem;
            doc = tvDoc;
        }

        #region Action Members

        public override string Name => Operation == Op.rename ? "Rename" : IsMoveRename() ? "Move" : "Copy";

        public override string ProgressText => To.Name;

        // 0.0 to 100.0
        public override long SizeOfWork => QuickOperation() ? 10000 : SourceFileSize();

        public override ActionOutcome Go(TVRenameStats stats)
        {
            // read NTFS permissions (if any)
            FileSecurity security = null;
            try
            {
                security = From.GetAccessControl();
            }
            catch
            {
                // ignored
            }

            try
            {
                //we use a temp name just in case we are interrupted or some other problem occurs
                string tempName = TempFor(To);

                if (!Directory.Exists(To.Directory.FullName))
                {
                    Directory.CreateDirectory(To.Directory.FullName);
                }

                // If both full filenames are the same then we want to move it away and back
                //This deals with an issue on some systems (XP?) that case insensitive moves did not occur
                if (IsMoveRename() || FileHelper.Same(From, To))
                {
                    // This step could be slow, so report progress
                    CopyMoveResult moveResult = File.Move(From.FullName, tempName, MoveOptions.CopyAllowed | MoveOptions.ReplaceExisting, CopyProgressCallback, null);
                    if (moveResult.ErrorCode != 0)
                    {
                        throw new ActionFailedException(moveResult.ErrorMessage);
                    }
                }
                else
                {
                    //we are copying
                    Debug.Assert(Operation == Op.copy);

                    // This step could be slow, so report progress
                    CopyMoveResult copyResult = File.Copy(From.FullName, tempName, CopyOptions.None, true, CopyProgressCallback, null);
                    if (copyResult.ErrorCode != 0)
                    {
                        throw new ActionFailedException(copyResult.ErrorMessage);
                    }
                }

                // Copying the temp file into the correct name is very quick, so no progress reporting
                File.Move(tempName, To.FullName, MoveOptions.ReplaceExisting);
                LOGGER.Info($"{Name} completed: {From.FullName} to {To.FullName } ");

                UpdateStats(stats);

                if (To.IsMovieFile())
                {
                    //File is correct name
                    LOGGER.Debug($"Just copied {To.FullName} to the right place. Marking it as 'seen'.");

                    if (Episode != null)
                    {
                        //Record this episode as seen
                        TVSettings.Instance.PreviouslySeenEpisodes.EnsureAdded(SourceEpisode);

                        if (TVSettings.Instance.IgnorePreviouslySeen)
                        {
                            doc.SetDirty();
                        }
                    }

                    if (Movie != null)
                    {
                        //Record this movie as seen
                        TVSettings.Instance.PreviouslySeenMovies.EnsureAdded(Movie);

                        if (TVSettings.Instance.IgnorePreviouslySeenMovies)
                        {
                            doc.SetDirty();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LOGGER.Warn(e, $"Error occurred while {Name}: {From.FullName} to {To.FullName } ");
                return new ActionOutcome(e);
            }

            // set NTFS permissions
            try
            {
                if (security != null)
                {
                    To.SetAccessControl(security);
                }
            }
            catch
            {
                // ignored
            }

            try
            {
                if (Operation == Op.move && Tidyup != null && Tidyup.DeleteEmpty)
                {
                    LOGGER.Info($"Testing {From.Directory.FullName} to see whether it should be tidied up");
                    DoTidyUp(From.Directory);
                }
            }
            catch (Exception e)
            {
                return new ActionOutcome(e);
            }

            return ActionOutcome.Success();
        }

        private void UpdateStats([NotNull] TVRenameStats stats)
        {
            switch (Operation)
            {
                case Op.move:
                    stats.FilesMoved++;
                    break;

                case Op.rename:
                    stats.FilesRenamed++;
                    break;

                case Op.copy:
                    stats.FilesCopied++;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string Produces => To.FullName;

        public override bool SameAs(Item o)
        {
            return o is ActionCopyMoveRename cmr
                   && Operation == cmr.Operation
                   && FileHelper.Same(From, cmr.From)
                   && FileHelper.Same(To, cmr.To);
        }

        public override int CompareTo(Item o)
        {
            if (!(o is ActionCopyMoveRename cmr))
            {
                return -1;
            }

            if (From.Directory is null
                || To.Directory is null
                || cmr.From.Directory is null
                || cmr.To.Directory is null)
            {
                return 0;
            }

            string s1 = From.FullName + (From.Directory.Root.FullName != To.Directory.Root.FullName ? "0" : "1");
            string s2 = cmr.From.FullName +
                     (cmr.From.Directory.Root.FullName != cmr.To.Directory.Root.FullName ? "0" : "1");

            return string.Compare(s1, s2, StringComparison.Ordinal);
        }

        public override int IconNumber => IsMoveRename() ? 4 : 3;

        #endregion Action Members

        public ProcessedEpisode SourceEpisode => Episode ?? throw new InvalidOperationException();

        #region Item Members

        public override IgnoreItem Ignore => new IgnoreItem(To.FullName);

        public override string ScanListViewGroup
        {
            get
            {
                return Operation switch
                {
                    Op.rename => "lvgActionRename",
                    Op.copy => "lvgActionCopy",
                    Op.move => "lvgActionMove",
                    _ => "lvgActionCopy"
                };
            }
        }

        public override string TargetFolder => To.DirectoryName;

        #endregion Item Members

        [NotNull]
        private static string TempFor([NotNull] FileSystemInfo f) => f.FullName + ".tvrenametemp";

        public bool QuickOperation()
        {
            if (From.Directory is null || To.Directory is null)
            {
                return false;
            }

            return IsMoveRename() &&
                   string.Equals(From.Directory.Root.FullName, To.Directory.Root.FullName,
                       StringComparison.InvariantCultureIgnoreCase); // same device ... TODO: UNC paths?
        }

        private static void KeepTimestamps([NotNull] FileSystemInfo from, [NotNull] FileSystemInfo to)
        {
            to.CreationTime = from.CreationTime;
            to.CreationTimeUtc = from.CreationTimeUtc;
            to.LastAccessTime = from.LastAccessTime;
            to.LastAccessTimeUtc = from.LastAccessTimeUtc;
            to.LastWriteTime = from.LastWriteTime;
            to.LastWriteTimeUtc = from.LastWriteTimeUtc;
        }

        private CopyMoveProgressResult CopyProgressCallback(long totalFileSize, long totalBytesTransferred, long streamSize, long streamBytesTransferred, int streamNumber, CopyMoveProgressCallbackReason callbackReason, object userData)
        {
            double pct = totalBytesTransferred * 100.0 / totalFileSize;
            PercentDone = pct > 100.0 ? 100.0 : pct;
            return CopyMoveProgressResult.Continue;
        }

        private bool IsMoveRename() // same thing to the OS
            => Operation == Op.move || Operation == Op.rename;

        public bool SameSource([NotNull] ActionCopyMoveRename o) => FileHelper.Same(From, o.From);

        private long SourceFileSize()
        {
            try
            {
                return From.Length;
            }
            catch
            {
                return 1;
            }
        }

        public override string DestinationFolder => To.DirectoryName;
        public override string DestinationFile => To.Name;
        public override string SourceDetails => From.FullName;

        public DirectoryInfo SourceDirectory => From.Directory;
        public string DestinationBaseName=> To.FileNameNoExt();
    }
}
