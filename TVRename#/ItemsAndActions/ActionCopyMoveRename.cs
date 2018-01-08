using Alphaleonis.Win32.Filesystem;
using System.Diagnostics;
using System.Security.AccessControl;

namespace TVRename
{
    using Alphaleonis.Win32.Filesystem;
    using System;
    using System.Windows.Forms;
    using File = File;		
    using FileInfo = FileInfo;		
    using FileSystemInfo = FileSystemInfo;		


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
            this._tidyup = tidyup;
            this.PercentDone = 0;
            this.Episode = ep;
            this.Operation = operation;
            this.From = from;
            this.To = to;
        }

        #region Action Members



        public override string Name => IsMoveRename() ? "Move" : "Copy";

        public override string ProgressText => this.To.Name;


        // 0.0 to 100.0
        public override long SizeOfWork => QuickOperation() ? 10000 : SourceFileSize();

        public override bool Go(ref bool pause, TVRenameStats stats)
        {
            // read NTFS permissions (if any)
            FileSecurity security = null;
            try
            {
                security = this.From.GetAccessControl();
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
                    CopyMoveResult moveResult = File.Move(this.From.FullName, tempName, MoveOptions.CopyAllowed | MoveOptions.ReplaceExisting, CopyProgressCallback, null);
                    if (moveResult.ErrorCode != 0) throw new Exception(moveResult.ErrorMessage);
                }
                else
                {
                    //we are copying
                    Debug.Assert(this.Operation == Op.Copy);

                    // This step could be slow, so report progress
                    CopyMoveResult copyResult = File.Copy(this.From.FullName, tempName, CopyOptions.None, true, CopyProgressCallback, null);
                    if (copyResult.ErrorCode != 0) throw new Exception(copyResult.ErrorMessage);
                }

                // Copying the temp file into the correct name is very quick, so no progress reporting		
                File.Move(tempName, this.To.FullName, MoveOptions.ReplaceExisting);

                this.Done = true;

                switch (this.Operation)
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
                this.Done = true;
                this.Error = true;
                this.ErrorText = e.Message;
            }

            // set NTFS permissions
            try
            {
                if (security != null) this.To.SetAccessControl(security);
            }
            catch
            {
                // ignored
            }

            if (this.Operation == Op.Move && this._tidyup != null && this._tidyup.DeleteEmpty)
            {
                DoTidyup(this.From.Directory  );
            }

            return !this.Error;
        }



        public override string produces => this.To.FullName;

        #region Item Members

        public override bool SameAs(Item o)
        {
            ActionCopyMoveRename cmr = o as ActionCopyMoveRename;

            return (cmr != null) && (this.Operation == cmr.Operation) && FileHelper.Same(this.From, cmr.From) &&
                   FileHelper.Same(this.To, cmr.To);
        }

        public override int Compare(Item o)
        {
            ActionCopyMoveRename cmr = o as ActionCopyMoveRename;

            if (cmr == null || this.From.Directory == null || this.To.Directory == null || cmr.From.Directory == null ||
                cmr.To.Directory == null)
                return 0;

            string s1 = this.From.FullName + (this.From.Directory.Root.FullName != this.To.Directory.Root.FullName ? "0" : "1");
            string s2 = cmr.From.FullName +
                     (cmr.From.Directory.Root.FullName != cmr.To.Directory.Root.FullName ? "0" : "1");

            return string.Compare(s1, s2, StringComparison.Ordinal);
        }

        #endregion

        #region ScanListItem Members

        public override int IconNumber => IsMoveRename() ? 4 : 3;



        public override IgnoreItem Ignore => this.To == null ? null : new IgnoreItem(this.To.FullName);
        #endregion

        public override ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem();

	            if (this.Episode == null)
	            {
		            lvi.Text = "";
		            lvi.SubItems.Add("");
		            lvi.SubItems.Add("");
		            lvi.SubItems.Add("");
		            lvi.SubItems.Add("");
	            }
	            else
	            {
		            lvi.Text = this.Episode.TheSeries.Name;
		            lvi.SubItems.Add(this.Episode.SeasonNumber.ToString());
		            lvi.SubItems.Add(this.Episode.NumsAsString());
		            DateTime? dt = this.Episode.GetAirDateDT(true);
		            if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
			            lvi.SubItems.Add(dt.Value.ToShortDateString());
		            else
			            lvi.SubItems.Add("");
	            }

	            lvi.SubItems.Add(this.From.DirectoryName);
                lvi.SubItems.Add(this.From.Name);
                lvi.SubItems.Add(this.To.DirectoryName);
                lvi.SubItems.Add(this.To.Name);

                return lvi;
            }
        }

        public override string ScanListViewGroup
        {
            get
            {
                switch (this.Operation)
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

        public override string TargetFolder => this.To?.DirectoryName;

        #endregion

        private static string TempFor(FileSystemInfo f) => f.FullName + ".tvrenametemp";

        public bool QuickOperation()
        {
            if ((this.From == null) || (this.To == null) || (this.From.Directory == null) || (this.To.Directory == null))
                return false;

            return IsMoveRename() &&
                   string.Equals(this.From.Directory.Root.FullName, this.To.Directory.Root.FullName,
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
            => (this.Operation == Op.Move) || (this.Operation == Op.Rename);

        public bool SameSource(ActionCopyMoveRename o) => FileHelper.Same(this.From, o.From);

        // ========================================================================================================

        private long SourceFileSize()
        {
            try
            {
                return this.From.Length;
            }
            catch
            {
                return 1;
            }
        }
    }
}
