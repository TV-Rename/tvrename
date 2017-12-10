using System;
using System.CodeDom;
using System.Diagnostics;
using Alphaleonis.Win32.Filesystem;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace TVRename
{
    using System;
    using Alphaleonis.Win32.Filesystem;
    using System.Windows.Forms;
    using System.IO;

    using File = Alphaleonis.Win32.Filesystem.File;		
    using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;		
    using FileSystemInfo = Alphaleonis.Win32.Filesystem.FileSystemInfo;		
    using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;


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

            //if (QuickOperation())
                this.OSMoveRename(stats); // ask the OS to do it for us, since it's easy and quick!
            //else
                //CopyItOurself(ref pause, stats); // do it ourself!

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

        private void OSMoveRename(TVRenameStats stats)
        {
            try
            {
                if (FileHelper.Same(this.From, this.To))
                {
                    // XP won't actually do a rename if its only a case difference
                    string tempName = TempFor(this.To);

                    //From.MoveTo(tempName);
                    //File.Move(tempName, To.FullName);

                    // This step could be slow, so report progress
                    CopyMoveResult moveResult = Alphaleonis.Win32.Filesystem.File.Move(this.From.FullName, this.To.FullName, MoveOptions.CopyAllowed | MoveOptions.ReplaceExisting, CopyProgressCallback, null);
                    if (moveResult.ErrorCode != 0)
                    {
                        throw new Exception(moveResult.ErrorMessage);
                    }

                    // This step very quick, so no progress reporting		
                    Alphaleonis.Win32.Filesystem.File.Move(tempName, this.To.FullName);


                }
                else
                {
                    //From.MoveTo(To.FullName);
                    CopyMoveResult moveResult = Alphaleonis.Win32.Filesystem.File.Move(this.From.FullName, this.To.FullName, MoveOptions.CopyAllowed | MoveOptions.ReplaceExisting, CopyProgressCallback, null);
                    if (moveResult.ErrorCode != 0)
                    {
                        throw new Exception(moveResult.ErrorMessage);
                    }
                }

                // AlphaFS doesn't reset file time stamps
                //KeepTimestamps(this.From, this.To);

                this.Done = true;

                System.Diagnostics.Debug.Assert((this.Operation == ActionCopyMoveRename.Op.Move) || (this.Operation == ActionCopyMoveRename.Op.Rename));

                if (this.Operation == ActionCopyMoveRename.Op.Move)
                    stats.FilesMoved++;
                else if (this.Operation == ActionCopyMoveRename.Op.Rename)
                    stats.FilesRenamed++;
            }
            catch (System.Exception e)
            {
                this.Done = true;
                this.Error = true;
                this.ErrorText = e.Message;
            }
        }

        private CopyMoveProgressResult CopyProgressCallback(long TotalFileSize, long TotalBytesTransferred, long StreamSize, long StreamBytesTransferred, int StreamNumber, CopyMoveProgressCallbackReason CallbackReason, Object UserData)
        {
            double pct = TotalBytesTransferred * 100.0 / TotalFileSize;
            this.PercentDone = pct > 100.0 ? 100.0 : pct;
            return CopyMoveProgressResult.Continue;
        }


        private void CopyItOurself(ref bool pause, TVRenameStats stats)

        {
            const int kArrayLength = 1 * 1024 * 1024;
            Byte[] dataArray = new Byte[kArrayLength];

            bool useWin32 = Version.OnWindows() && !Version.OnMono();

            Win32FileIO.WinFileIO copier = null;

            BinaryReader msr = null;
            BinaryWriter msw = null;

            try
            {
                long thisFileCopied = 0;
                long thisFileSize = this.SourceFileSize();

                string tempName = TempFor(this.To);
                if (File.Exists(tempName))
                    File.Delete(tempName);

                if (useWin32)
                {
                    copier = new Win32FileIO.WinFileIO(dataArray);
                    copier.OpenForReading(this.From.FullName);
                    copier.OpenForWriting(tempName);
                }
                else
                {
                    msr = new BinaryReader(new FileStream(this.From.FullName, System.IO.FileMode.Open, FileAccess.Read));
                    msw = new BinaryWriter(new FileStream(tempName, System.IO.FileMode.CreateNew));
                }

                for (;;)
                {
                    int bytesRead = useWin32 ? copier.ReadBlocks(kArrayLength) : msr.Read(dataArray, 0, kArrayLength);
                    if (bytesRead == 0)
                        break;

                    if (useWin32)
                    {
                        copier.WriteBlocks(bytesRead);
                    }
                    else
                    {
                        msw.Write(dataArray, 0, bytesRead);
                    }
                    thisFileCopied += bytesRead;

                    double pct = (thisFileSize != 0) ? (100.0 * thisFileCopied / thisFileSize) : this.Done ? 100 : 0;
                    if (pct > 100.0)
                        pct = 100.0;
                    this.PercentDone = pct;

                    while (pause)
                        System.Threading.Thread.Sleep(100);
                }

                if (useWin32)
                {
                    copier.Close();
                }
                else
                {
                    msr.Close();
                    msw.Close();
                }

                // rename temp version to final name
                if (this.To.Exists)
                    this.To.Delete(); // outta ma way!
                File.Move(tempName, this.To.FullName);

                KeepTimestamps(this.From, this.To);

                // if that was a move/rename, delete the source
                if (this.IsMoveRename())
                    this.From.Delete();

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

                this.Done = true;
            } // try
            catch (System.Threading.ThreadAbortException)
            {
                if (useWin32)
                {
                    this.NicelyStopAndCleanUp_Win32(copier);
                }
                else
                {
                    this.NicelyStopAndCleanUp_Streams(msr, msw);
                }
                return;
            }
            catch (Exception ex)
            {
                // handle any exception type
                this.Error = true;
                this.Done = true;
                this.ErrorText = ex.Message;
                if (useWin32)
                {
                    this.NicelyStopAndCleanUp_Win32(copier);
                }
                else
                {
                    this.NicelyStopAndCleanUp_Streams(msr, msw);
                }
            }
        }


        private void NicelyStopAndCleanUp_Win32(Win32FileIO.WinFileIO copier)
        {
            if (copier != null) copier.Close();
            string tempName = TempFor(this.To);
            if (File.Exists(tempName))
                File.Delete(tempName);
        }

        private void NicelyStopAndCleanUp_Streams(BinaryReader msr, BinaryWriter msw)
        {
            if (msw != null)
            {
                msw.Close();
                string tempName = TempFor(this.To);
                if (File.Exists(tempName))
                    File.Delete(tempName);
            }
            if (msr != null)
                msr.Close();
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