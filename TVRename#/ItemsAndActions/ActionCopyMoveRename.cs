using System;
using System.CodeDom;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace TVRename
{
    public class ActionCopyMoveRename : Item, Action, ScanListItem
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
        private double _percent;
        private readonly TidySettings _tidyup;

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

        public bool Done { get; set; }
        public bool Error { get; set; }
        public string ErrorText { get; set; }

        public string Name => IsMoveRename() ? "Move" : "Copy";

        public string ProgressText => To.Name;

        public double PercentDone
        {
            get { return Done ? 100.0 : _percent; }
            set { _percent = value; }
        }

        // 0.0 to 100.0
        public long SizeOfWork => QuickOperation() ? 10000 : SourceFileSize();

        public bool Go(ref bool pause, TVRenameStats stats)
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

            if (QuickOperation())
                OSMoveRename(stats); // ask the OS to do it for us, since it's easy and quick!
            else
                CopyItOurself(ref pause, stats); // do it ourself!

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
                DoTidyup();
            }

            return !Error;
        }

        private void DeleteOrRecycleFolder()
        {
            DirectoryInfo di = From.Directory;
            if (di == null) return;
            if (_tidyup.DeleteEmptyIsRecycle)
                FileSystem.DeleteDirectory(di.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            else
                di.Delete();
        }


        private void DoTidyup()
        {
#if DEBUG
            Debug.Assert(this._tidyup != null);
            Debug.Assert(this._tidyup.DeleteEmpty);
#else
            if (_tidyup == null || !_tidyup.DeleteEmpty)
                return;
#endif
            // See if we should now delete the folder we just moved that file from.
            DirectoryInfo di = From.Directory;
            if (di == null)
                return;

            //if there are sub-directories then we shouldn't remove this one
            if (di.GetDirectories().Length > 0)
                return;


            FileInfo[] files = di.GetFiles();
            if (files.Length == 0)
            {
                // its empty, so just delete it
                di.Delete();
                return;
            }


            if (_tidyup.EmptyIgnoreExtensions && !_tidyup.EmptyIgnoreWords)
                return; // nope

            foreach (FileInfo fi in files)
            {
                bool okToDelete = _tidyup.EmptyIgnoreExtensions &&
                                 Array.FindIndex(_tidyup.EmptyIgnoreExtensionsArray, x => x == fi.Extension) != -1;

                if (okToDelete)
                    continue; // onto the next file

                // look in the filename
                if (_tidyup.EmptyIgnoreWordsArray.Any(word => fi.Name.Contains(word)))
                    okToDelete = true;

                if (!okToDelete)
                    return;
            }

            if (_tidyup.EmptyMaxSizeCheck)
            {
                // how many MB are we deleting?
                long totalBytes = files.Sum(fi => fi.Length);

                if (totalBytes/(1024*1024) > _tidyup.EmptyMaxSizeMB)
                    return; // too much
            }
            DeleteOrRecycleFolder();
        }

        #endregion

        public string produces => To.FullName;

        #region Item Members

        public bool SameAs(Item o)
        {
            ActionCopyMoveRename cmr = o as ActionCopyMoveRename;

            return (cmr != null) && (Operation == cmr.Operation) && FileHelper.Same(From, cmr.From) &&
                   FileHelper.Same(To, cmr.To);
        }

        public int Compare(Item o)
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

        public int IconNumber => IsMoveRename() ? 4 : 3;

        public ProcessedEpisode Episode { get; set; }

        public IgnoreItem Ignore => To == null ? null : new IgnoreItem(To.FullName);

        public ListViewItem ScanListViewItem
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

        public string ScanListViewGroup
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

        public string TargetFolder => To?.DirectoryName;

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
                if (FileHelper.Same(From, To))
                {
                    // XP won't actually do a rename if its only a case difference
                    string tempName = TempFor(To);
                    From.MoveTo(tempName);
                    File.Move(tempName, To.FullName);
                }
                else
                    From.MoveTo(To.FullName);

                KeepTimestamps(From, To);

                Done = true;

                Debug.Assert((Operation == Op.Move) || (Operation == Op.Rename));

                switch (Operation)
                {
                    case Op.Move:
                        stats.FilesMoved++;
                        break;
                    case Op.Rename:
                        stats.FilesRenamed++;
                        break;
                    case Op.Copy:
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
                    msr = new BinaryReader(new FileStream(this.From.FullName, FileMode.Open, FileAccess.Read));
                    msw = new BinaryWriter(new FileStream(tempName, FileMode.CreateNew));
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