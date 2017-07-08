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
            var di = From.Directory;
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
            var di = From.Directory;
            if (di == null)
                return;

            //if there are sub-directories then we shouldn't remove this one
            if (di.GetDirectories().Length > 0)
                return;


            var files = di.GetFiles();
            if (files.Length == 0)
            {
                // its empty, so just delete it
                di.Delete();
                return;
            }


            if (_tidyup.EmptyIgnoreExtensions && !_tidyup.EmptyIgnoreWords)
                return; // nope

            foreach (var fi in files)
            {
                var okToDelete = _tidyup.EmptyIgnoreExtensions &&
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
                var totalBytes = files.Sum(fi => fi.Length);

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
            var cmr = o as ActionCopyMoveRename;

            return (cmr != null) && (Operation == cmr.Operation) && FileHelper.Same(From, cmr.From) &&
                   FileHelper.Same(To, cmr.To);
        }

        public int Compare(Item o)
        {
            var cmr = o as ActionCopyMoveRename;

            if (cmr == null || From.Directory == null || To.Directory == null || cmr.From.Directory == null ||
                cmr.To.Directory == null)
                return 0;

            var s1 = From.FullName + (From.Directory.Root.FullName != To.Directory.Root.FullName ? "0" : "1");
            var s2 = cmr.From.FullName +
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
                var lvi = new ListViewItem();

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
		            var dt = Episode.GetAirDateDT(true);
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
                    var tempName = TempFor(To);
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
            const int kArrayLength = 4 * 1024 * 1024;
            var dataArray = new byte[kArrayLength];

            var useWin32 = Version.OnWindows() && !Version.OnMono();

            try
            {

                var tempName = TempFor(To);
                if (File.Exists(tempName))
                    File.Delete(tempName);

                using (var inputMemoryMappedFile = useWin32 ? MemoryMappedFile.CreateFromFile(From.FullName) : null)
                using (var inputStream = useWin32 ? (Stream)inputMemoryMappedFile.CreateViewStream() : new FileStream(From.FullName, FileMode.Open, FileAccess.Read))
                using (var outputStream = new FileStream(tempName, FileMode.CreateNew, FileAccess.Write, FileShare.Read, 4096, FileOptions.WriteThrough))
                {
                    long bytesCopied = 0;
                    // MemoryMappedViewStream.Length reflects MemoryMappedView.Size which does not reflect file size
                    var srcFileSize = useWin32 || inputStream.Length == 0 ? SourceFileSize() : inputStream.Length;

                    outputStream.SetLength(srcFileSize);
                    outputStream.Position = 0;

                    var remainingBytes = srcFileSize;
                    while (srcFileSize == 0 || remainingBytes > 0)
                    {
                        var bytesRead = inputStream.Read(dataArray, 0, kArrayLength);
                        if (bytesRead == 0)
                            break;
                        if (srcFileSize != 0 && bytesRead > remainingBytes)
                            bytesRead = (int)remainingBytes;

                        outputStream.Write(dataArray, 0, bytesRead);
                        bytesCopied += bytesRead;
                        remainingBytes -= bytesRead;

                        var pct = srcFileSize != 0 ? 100.0 * bytesCopied / srcFileSize : (Done ? 100 : 0);
                        PercentDone = Math.Min(pct, 100.0);

                        while (pause)
                            Thread.Sleep(100);
                    }
                }

                // rename temp version to final name
                if (To.Exists)
                    To.Delete(); // outta ma way!
                File.Move(tempName, To.FullName);

                KeepTimestamps(From, To);

                // if that was a move/rename, delete the source
                if (IsMoveRename())
                    From.Delete();

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

                Done = true;
            } // try
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                // handle any exception type
                Error = true;
                Done = true;
                ErrorText = ex.Message;
            }
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