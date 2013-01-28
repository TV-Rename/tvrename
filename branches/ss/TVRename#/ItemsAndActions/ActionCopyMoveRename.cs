// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html

using Microsoft.VisualBasic.FileIO;

namespace TVRename
{
    using System;
    using System.IO;
    using System.Windows.Forms;

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
        private double _Percent;
        private TidySettings Tidyup;

        public ActionCopyMoveRename(Op operation, FileInfo from, FileInfo to, ProcessedEpisode ep, TidySettings tidyup)
        {
            this.Tidyup = tidyup;
            this.PercentDone = 0;
            this.Episode = ep;
            this.Operation = operation;
            this.From = from;
            this.To = to;
        }

        #region Action Members

        public bool Done { get; set; }
        public bool Error { get; set; }
        public string ErrorText { get; set; }

        public string Name
        {
            get { return this.IsMoveRename() ? "Move" : "Copy"; }
        }

        public string ProgressText
        {
            get { return this.To.Name; }
        }

        public double PercentDone
        {
            get { return this.Done ? 100.0 : this._Percent; }
            set { this._Percent = value; }
        }

        // 0.0 to 100.0
        public long SizeOfWork
        {
            get { return this.QuickOperation() ? 10000 : this.SourceFileSize(); }
        }

        public bool Go(TVSettings settings, ref bool pause, TVRenameStats stats)
        {
            // read NTFS permissions (if any)
            System.Security.AccessControl.FileSecurity security = null;
            try
            {
                security = this.From.GetAccessControl();
            }
            catch
            {
            }

            if (this.QuickOperation())
            {
                this.OSMoveRename(stats); // ask the OS to do it for us, since it's easy and quick!
            }
            else
                this.CopyItOurself(ref pause, stats); // do it ourself!

            // set NTFS permissions
            try
            {
                if (security != null)
                    this.To.SetAccessControl(security);
            }
            catch
            {
            }

            if (this.Operation == Op.Move && this.Tidyup != null && this.Tidyup.DeleteEmpty)
            {
                DoTidyup();
            }

            return !this.Error;
        }

        private void DeleteOrRecycleFolder()
        {
            DirectoryInfo di = this.From.Directory;
            if (Tidyup.DeleteEmptyIsRecycle)
            {
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(di.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
            else
            {
                di.Delete();
            }
        }


        private void DoTidyup()
        {
#if DEBUG
            System.Diagnostics.Debug.Assert(this.Tidyup != null);
            System.Diagnostics.Debug.Assert(this.Tidyup.DeleteEmpty);
#else
            if (this.Tidyup == null || !this.Tidyup.DeleteEmpty)
            {
                return;
            }
#endif
            // See if we should now delete the folder we just moved that file from.
            DirectoryInfo di = this.From.Directory;
            if (di == null)
                return;

            FileInfo[] files = di.GetFiles();
            if (files.Length == 0)
            {
                // its empty, so just delete it
                di.Delete();
                return;
            }

            if (Tidyup.EmptyIgnoreExtensions && !Tidyup.EmptyIgnoreWords)
                return; // nope

            foreach (FileInfo fi in files)
            {
                bool okToDelete = Tidyup.EmptyIgnoreExtensions &&
                                  Array.FindIndex(Tidyup.EmptyIgnoreExtensionsArray, x => x == fi.Extension) != -1;

                if (okToDelete)
                    continue; // onto the next file

                // look in the filename
                foreach (string word in Tidyup.EmptyIgnoreWordsArray)
                {
                    if (fi.Name.Contains(word))
                    {
                        okToDelete = true;
                        break;
                    }
                }

                if (!okToDelete)
                    return;
            }

            if (Tidyup.EmptyMaxSizeCheck)
            {
                // how many MB are we deleting?
                long totalBytes = 0;
                foreach (FileInfo fi in files)
                {
                    totalBytes += fi.Length;
                }
                if (totalBytes / (1024 * 1024) > Tidyup.EmptyMaxSizeMB)
                    return; // too much
            }
            DeleteOrRecycleFolder();
        }

        #endregion

        #region Item Members

        public bool SameAs(Item o)
        {
            ActionCopyMoveRename cmr = o as ActionCopyMoveRename;

            return ((cmr != null) && (this.Operation == cmr.Operation) && Helpers.Same(this.From, cmr.From) && Helpers.Same(this.To, cmr.To));
        }

        public int Compare(Item o)
        {
            ActionCopyMoveRename cmr = o as ActionCopyMoveRename;

            if (cmr == null || this.From.Directory == null || this.To.Directory == null || cmr.From.Directory==null || cmr.To.Directory==null)
                return 0;

            string s1 = this.From.FullName + (this.From.Directory.Root.FullName != this.To.Directory.Root.FullName ? "0" : "1");
            string s2 = cmr.From.FullName + (cmr.From.Directory.Root.FullName != cmr.To.Directory.Root.FullName ? "0" : "1");

            return s1.CompareTo(s2);
        }

        #endregion

        #region ScanListItem Members

        public int IconNumber
        {
            get { return this.IsMoveRename() ? 4 : 3; }
        }

        public ProcessedEpisode Episode { get; set; }

        public IgnoreItem Ignore
        {
            get
            {
                if (this.To == null)
                    return null;
                return new IgnoreItem(this.To.FullName);
            }
        }

        public ListViewItem ScanListViewItem
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

        public string ScanListViewGroup
        {
            get
            {
                if (this.Operation == Op.Rename)
                    return "lvgActionRename";
                if (this.Operation == Op.Copy)
                    return "lvgActionCopy";
                if (this.Operation == Op.Move)
                    return "lvgActionMove";
                return "lvgActionCopy";
            }
        }

        public string TargetFolder
        {
            get
            {
                if (this.To == null)
                    return null;
                return this.To.DirectoryName;
            }
        }

        #endregion

        private static string TempFor(FileInfo f)
        {
            return f.FullName + ".tvrenametemp";
        }

        private void NicelyStopAndCleanUp_Win32(Win32FileIO.WinFileIO copier)
        {
            copier.Close();
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

        public bool QuickOperation()
        {
            if ((this.From == null) || (this.To == null) || (this.From.Directory == null) || (this.To.Directory == null))
                return false;

            return (this.IsMoveRename() && (this.From.Directory.Root.FullName.ToLower() == this.To.Directory.Root.FullName.ToLower())); // same device ... TODO: UNC paths?
        }

        private static void KeepTimestamps(FileInfo from, FileInfo to)
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
                if (Helpers.Same(this.From, this.To))
                {
                    // XP won't actually do a rename if its only a case difference
                    string tempName = TempFor(this.To);
                    this.From.MoveTo(tempName);
                    File.Move(tempName, this.To.FullName);
                }
                else
                    this.From.MoveTo(this.To.FullName);

                KeepTimestamps(this.From, this.To);

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
                    int n = useWin32 ? copier.ReadBlocks(kArrayLength) : msr.Read(dataArray, 0, kArrayLength);
                    if (n == 0)
                        break;

                    if (useWin32)
                    {
                        copier.WriteBlocks(n);
                    }
                    else
                    {
                        msw.Write(dataArray, 0, n);
                    }
                    thisFileCopied += n;

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

                if (this.Operation == ActionCopyMoveRename.Op.Move)
                    stats.FilesMoved++;
                else if (this.Operation == ActionCopyMoveRename.Op.Rename)
                    stats.FilesRenamed++;
                else if (this.Operation == ActionCopyMoveRename.Op.Copy)
                    stats.FilesCopied++;

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


        // --------------------------------------------------------------------------------------------------------

        public bool IsMoveRename() // same thing to the OS
        {
            return ((this.Operation == Op.Move) || (this.Operation == Op.Rename));
        }

        public bool SameSource(ActionCopyMoveRename o)
        {
            return (Helpers.Same(this.From, o.From));
        }

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