using Alphaleonis.Win32.Filesystem;
using System;
using System.CodeDom;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace TVRename
{
    using Alphaleonis.Win32.Filesystem;
    using System;
    using System.IO;
    using System.Windows.Forms;
    using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
    using File = Alphaleonis.Win32.Filesystem.File;		
    using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;		
    using FileSystemInfo = Alphaleonis.Win32.Filesystem.FileSystemInfo;		


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


        public ActionCopyMoveRename(Op operation, FileInfo from, FileInfo to, ProcessedEpisode ep, TidySettings tidyup)
        {
            _tidyup = tidyup;
            PercentDone = 0;
            Episode = ep;
            Operation = operation;
            From = from;
            To = to;
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
                string tempName = TempFor(this.To);

                // If both full filenames are the same then we want to move it away and back
                //This deals with an issue on some systems (XP?) that case insensitive moves did not occur
                if (IsMoveRename() || FileHelper.Same(this.From, this.To)) 
                {
                    // This step could be slow, so report progress
                    CopyMoveResult moveResult = Alphaleonis.Win32.Filesystem.File.Move(this.From.FullName, tempName, MoveOptions.CopyAllowed | MoveOptions.ReplaceExisting, CopyProgressCallback, null);
                    if (moveResult.ErrorCode != 0) throw new Exception(moveResult.ErrorMessage);
                }
                else
                {
                    //we are copying
                    System.Diagnostics.Debug.Assert(this.Operation == Op.Copy);

                    // This step could be slow, so report progress
                    CopyMoveResult copyResult = Alphaleonis.Win32.Filesystem.File.Copy(this.From.FullName, tempName, CopyOptions.None, true, CopyProgressCallback, null);
                    if (copyResult.ErrorCode != 0) throw new Exception(copyResult.ErrorMessage);
                }

                // Copying the temp file into the correct name is very quick, so no progress reporting		
                Alphaleonis.Win32.Filesystem.File.Move(tempName, this.To.FullName, MoveOptions.ReplaceExisting);

                this.Done = true;

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
            catch (System.Exception e)
            {
                this.Done = true;
                this.Error = true;
                this.ErrorText = e.Message;
            }

            // set NTFS permissions
            try
            {
                if (security != null)
                    To.SetAccessControl(security);
            }
            catch
            {
                // ignored
            }

            if (Operation == Op.Move && _tidyup != null && _tidyup.DeleteEmpty)
            {
                DoTidyup(From.Directory  );
            }

            return !Error;
        }



        public override string produces => To.FullName;

        #region Item Members

        public override bool SameAs(Item o)
        {
            ActionCopyMoveRename cmr = o as ActionCopyMoveRename;

            return (cmr != null) && (Operation == cmr.Operation) && FileHelper.Same(From, cmr.From) &&
                   FileHelper.Same(To, cmr.To);
        }

        public override int Compare(Item o)
        {
            ActionCopyMoveRename cmr = o as ActionCopyMoveRename;

            if (cmr == null || From.Directory == null || To.Directory == null || cmr.From.Directory == null ||
                cmr.To.Directory == null)
                return 0;

            string s1 = From.FullName + (From.Directory.Root.FullName != To.Directory.Root.FullName ? "0" : "1");
            string s2 = cmr.From.FullName +
                     (cmr.From.Directory.Root.FullName != cmr.To.Directory.Root.FullName ? "0" : "1");

            return string.Compare(s1, s2, StringComparison.Ordinal);
        }

        #endregion

        #region ScanListItem Members

        public override int IconNumber => IsMoveRename() ? 4 : 3;



        public override IgnoreItem Ignore => To == null ? null : new IgnoreItem(To.FullName);
        #endregion

        public override ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem();

	            if (Episode == null)
	            {
		            lvi.Text = "";
		            lvi.SubItems.Add("");
		            lvi.SubItems.Add("");
		            lvi.SubItems.Add("");
		            lvi.SubItems.Add("");
	            }
	            else
	            {
		            lvi.Text = Episode.TheSeries.Name;
		            lvi.SubItems.Add(Episode.SeasonNumber.ToString());
		            lvi.SubItems.Add(Episode.NumsAsString());
		            DateTime? dt = Episode.GetAirDateDT(true);
		            if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
			            lvi.SubItems.Add(dt.Value.ToShortDateString());
		            else
			            lvi.SubItems.Add("");
	            }

	            lvi.SubItems.Add(From.DirectoryName);
                lvi.SubItems.Add(From.Name);
                lvi.SubItems.Add(To.DirectoryName);
                lvi.SubItems.Add(To.Name);

                return lvi;
            }
        }

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

        public override string TargetFolder => To?.DirectoryName;

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

        private CopyMoveProgressResult CopyProgressCallback(long TotalFileSize, long TotalBytesTransferred, long StreamSize, long StreamBytesTransferred, int StreamNumber, CopyMoveProgressCallbackReason CallbackReason, Object UserData)
        {
            double pct = TotalBytesTransferred * 100.0 / TotalFileSize;
            this.PercentDone = pct > 100.0 ? 100.0 : pct;
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
    }
}
