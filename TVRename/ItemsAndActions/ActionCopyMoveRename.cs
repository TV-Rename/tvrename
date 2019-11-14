using System.Diagnostics;
using System.Security.AccessControl;
using JetBrains.Annotations;

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

        #endregion

        public readonly FileInfo From;
        public Op Operation;
        public readonly FileInfo To;
        private readonly TVDoc doc;

        public ActionCopyMoveRename(Op operation, FileInfo from, FileInfo to, ProcessedEpisode ep, bool doTidyup,ItemMissing undoItem,TVDoc tvDoc)
        {
            Tidyup = doTidyup? TVSettings.Instance.Tidyup:null;
            PercentDone = 0;
            Episode = ep;
            Operation = operation;
            From = from;
            To = to;
            UndoItemMissing = undoItem;
            doc = tvDoc;
        }

        public ActionCopyMoveRename(FileInfo fi, FileInfo existingFile, ProcessedEpisode pep,TVDoc d): 
            this(TVSettings.Instance.LeaveOriginals ? Op.copy : Op.move, fi,
                existingFile,
                pep, true, null,d)
            {}

        #region Action Members

        [NotNull]
        public override string Name => IsMoveRename() ? "Move" : "Copy";

        public override string ProgressText => To.Name;

        // 0.0 to 100.0
        public override long SizeOfWork => QuickOperation() ? 10000 : SourceFileSize();

        public override bool Go(TVRenameStats stats)
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

            DoCopyRename(stats);

            if (To.IsMovieFile())
            {
                //File is correct name
                LOGGER.Debug($"Just copied {To.FullName} to the right place. Marking it as 'seen'.");
                //Record this episode as seen
                TVSettings.Instance.PreviouslySeenEpisodes.EnsureAdded(Episode);
                doc.SetDirty();
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

            TidyUpIfNeeded();

            return !Error;
        }

        private void DoCopyRename(TVRenameStats stats)
        {
            try
            {
                //we use a temp name just in case we are interrupted or some other problem occurs
                string tempName = TempFor(To);

                // If both full filenames are the same then we want to move it away and back
                //This deals with an issue on some systems (XP?) that case insensitive moves did not occur
                if (IsMoveRename() || FileHelper.Same(From, To))
                {
                    // This step could be slow, so report progress
                    CopyMoveResult moveResult = File.Move(From.FullName, tempName, MoveOptions.CopyAllowed | MoveOptions.ReplaceExisting, CopyProgressCallback, null);
                    if (moveResult.ErrorCode != 0)
                    {
                        throw new TheTVDB.TVDBException(moveResult.ErrorMessage);
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
                        throw new TheTVDB.TVDBException(copyResult.ErrorMessage);
                    }
                }

                // Copying the temp file into the correct name is very quick, so no progress reporting		
                File.Move(tempName, To.FullName, MoveOptions.ReplaceExisting);
                LOGGER.Info($"{Name} completed: {From.FullName} to {To.FullName } ");

                Done = true;

                UpdateStats(stats);
            }
            catch (Exception e)
            {
                LOGGER.Warn(e,$"Error occurred while {Name}: {From.FullName} to {To.FullName } ");
                Done = true;
                Error = true;
                ErrorText = e.Message;
                LastError = e;
            }
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

        private void TidyUpIfNeeded()
        {
            try
            {
                if (Operation == Op.move && Tidyup != null && Tidyup.DeleteEmpty)
                {
                    LOGGER.Info($"Testing {From.Directory.FullName} to see whether it should be tidied up");
                    DoTidyup(From.Directory);
                }
            }
            catch (Exception e)
            {
                Done = true;
                Error = true;
                ErrorText = e.Message;
                LastError = e;
            }
        }

        public override string Produces => To.FullName;

        public override bool SameAs(Item o)
        {
            return (o is ActionCopyMoveRename cmr)
                   && (Operation == cmr.Operation)
                   && FileHelper.Same(From, cmr.From)
                   && FileHelper.Same(To, cmr.To);
        }

        public override int Compare(Item o)
        {
            if (!(o is ActionCopyMoveRename cmr)
                || From.Directory is null
                || To.Directory is null
                || cmr.From.Directory is null
                ||cmr.To.Directory is null)
            {
                return 0;
            }

            string s1 = From.FullName + (From.Directory.Root.FullName != To.Directory.Root.FullName ? "0" : "1");
            string s2 = cmr.From.FullName +
                     (cmr.From.Directory.Root.FullName != cmr.To.Directory.Root.FullName ? "0" : "1");

            return string.Compare(s1, s2, StringComparison.Ordinal);
        }

        public override int IconNumber => IsMoveRename() ? 4 : 3;
        #endregion

        #region Item Members
        [CanBeNull]
        public override IgnoreItem Ignore => To is null ? null : new IgnoreItem(To.FullName);

        [NotNull]
        public override string ScanListViewGroup
        {
            get
            {
                switch (Operation)
                {
                    case Op.rename:
                        return "lvgActionRename";
                    case Op.copy:
                        return "lvgActionCopy";
                    case Op.move:
                        return "lvgActionMove";
                    default:
                        return "lvgActionCopy";
                }
            }
        }

        [CanBeNull]
        public override  string TargetFolder => To?.DirectoryName;

        #endregion

        [NotNull]
        private static string TempFor([NotNull] FileSystemInfo f) => f.FullName + ".tvrenametemp";

        public bool QuickOperation()
        {
            if ((From is null) || (To is null) || (From.Directory is null) || (To.Directory is null))
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
            => (Operation == Op.move) || (Operation == Op.rename);

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

        protected override string DestinationFolder => To.DirectoryName;
        protected override string DestinationFile => To.Name;
        protected override string SourceDetails => From.FullName;
    }
}
