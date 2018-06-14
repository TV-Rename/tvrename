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
            Copy,
            Move,
            Rename
        }

        #endregion

        public FileInfo From;
        public Op Operation;
        public FileInfo To;
        public ItemMissing UndoItemMissing;


        public ActionCopyMoveRename(Op operation, FileInfo from, FileInfo to, ProcessedEpisode ep, TidySettings tidyup,ItemMissing undoItem)
        {
            Tidyup = tidyup;
            PercentDone = 0;
            Episode = ep;
            Operation = operation;
            From = from;
            To = to;
            UndoItemMissing = undoItem;
        }

        #region Action Members

        public override string Name => IsMoveRename() ? "Move" : "Copy";

        public override string ProgressText => To.Name;


        // 0.0 to 100.0
        public override long SizeOfWork => QuickOperation() ? 10000 : SourceFileSize();

        public override bool Go(ref bool pause, TVRenameStats stats)
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
                //we use a temp name just in case we are interruted or some other problem occurs
                string tempName = TempFor(To);

                // If both full filenames are the same then we want to move it away and back
                //This deals with an issue on some systems (XP?) that case insensitive moves did not occur
                if (IsMoveRename() || FileHelper.Same(From, To)) 
                {
                    // This step could be slow, so report progress
                    CopyMoveResult moveResult = File.Move(From.FullName, tempName, MoveOptions.CopyAllowed | MoveOptions.ReplaceExisting, CopyProgressCallback, null);
                    if (moveResult.ErrorCode != 0) throw new Exception(moveResult.ErrorMessage);
                }
                else
                {
                    //we are copying
                    Debug.Assert(Operation == Op.Copy);

                    // This step could be slow, so report progress
                    CopyMoveResult copyResult = File.Copy(From.FullName, tempName, CopyOptions.None, true, CopyProgressCallback, null);
                    if (copyResult.ErrorCode != 0) throw new Exception(copyResult.ErrorMessage);
                }

                // Copying the temp file into the correct name is very quick, so no progress reporting		
                File.Move(tempName, To.FullName, MoveOptions.ReplaceExisting);
                logger.Info($"{Name} completed: {From.FullName} to {To.FullName } ");

                Done = true;

                switch (Operation)
                {
                    case Op.Move:
                        stats.FilesMoved++;
                        break;
                    case Op.Rename:
                        stats.FilesRenamed++;
                        break;
                    case Op.Copy:
                        stats.FilesCopied++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
            catch (Exception e)
            {
                Done = true;
                Error = true;
                ErrorText = e.Message;
            }

            // set NTFS permissions
            try
            {
                if (security != null) To.SetAccessControl(security);
            }
            catch
            {
                // ignored
            }

            try
            {
                if (Operation == Op.Move && Tidyup != null && Tidyup.DeleteEmpty)
                {
                    logger.Info($"Testing {From.Directory.FullName} to see whether it should be tidied up");
                    DoTidyup(From.Directory);
                }
            }
            catch (Exception e)
            {
                Done = true;
                Error = true;
                ErrorText = e.Message;
            }

            return !Error;
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
                || From.Directory == null
                || To.Directory == null
                || cmr.From.Directory == null
                ||cmr.To.Directory == null)
                return 0;

            string s1 = From.FullName + (From.Directory.Root.FullName != To.Directory.Root.FullName ? "0" : "1");
            string s2 = cmr.From.FullName +
                     (cmr.From.Directory.Root.FullName != cmr.To.Directory.Root.FullName ? "0" : "1");

            return string.Compare(s1, s2, StringComparison.Ordinal);
        }



        public override int IconNumber => IsMoveRename() ? 4 : 3;
        #endregion


        #region Item Members
        public override IgnoreItem Ignore => To == null ? null : new IgnoreItem(To.FullName);

        public override string ScanListViewGroup
        {
            get
            {
                switch (Operation)
                {
                    case Op.Rename:
                        return "lvgActionRename";
                    case Op.Copy:
                        return "lvgActionCopy";
                    case Op.Move:
                        return "lvgActionMove";
                    default:
                        return "lvgActionCopy";
                }
            }
        }

        public override  string TargetFolder => To?.DirectoryName;

        #endregion

        private static string TempFor(FileSystemInfo f) => f.FullName + ".tvrenametemp";

        public bool QuickOperation()
        {
            if ((From == null) || (To == null) || (From.Directory == null) || (To.Directory == null))
                return false;

            return IsMoveRename() &&
                   string.Equals(From.Directory.Root.FullName, To.Directory.Root.FullName,
                       StringComparison.InvariantCultureIgnoreCase); // same device ... TODO: UNC paths?
        }

        private static void KeepTimestamps(FileSystemInfo from, FileSystemInfo to)
        {
            to.CreationTime = from.CreationTime;
            to.CreationTimeUtc = from.CreationTimeUtc;
            to.LastAccessTime = from.LastAccessTime;
            to.LastAccessTimeUtc = from.LastAccessTimeUtc;
            to.LastWriteTime = from.LastWriteTime;
            to.LastWriteTimeUtc = from.LastWriteTimeUtc;
        }

        private CopyMoveProgressResult CopyProgressCallback(long totalFileSize, long totalBytesTransferred, long StreamSize, long StreamBytesTransferred, int StreamNumber, CopyMoveProgressCallbackReason CallbackReason, Object UserData)
        {
            double pct = totalBytesTransferred * 100.0 / totalFileSize;
            PercentDone = pct > 100.0 ? 100.0 : pct;
            return CopyMoveProgressResult.Continue;
        }



        // --------------------------------------------------------------------------------------------------------

        public bool IsMoveRename() // same thing to the OS
            => (Operation == Op.Move) || (Operation == Op.Rename);

        public bool SameSource(ActionCopyMoveRename o) => FileHelper.Same(From, o.From);

        // ========================================================================================================

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

        public override string FileInfo1 => From.DirectoryName;
        public override string FileInfo2 => From.Name;
        public override string FileInfo3 => To.DirectoryName;
        public override string FileInfo4 => To.Name;
    }
}
