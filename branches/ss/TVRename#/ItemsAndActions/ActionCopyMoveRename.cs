// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
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

        public ActionCopyMoveRename(Op operation, FileInfo from, FileInfo to, ProcessedEpisode ep)
        {
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
            get { return this.SourceFileSize(); }
        }

        public bool Go(TVSettings settings, ref bool pause)
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
                this.OSMoveRename(); // ask the OS to do it for us, since it's easy and quick!
            else
                this.CopyItOurself(ref pause); // do it ourself!

            // set NTFS permissions
            try
            {
                if (security != null)
                    this.To.SetAccessControl(security);
            }
            catch
            {
            }

            return !this.Error;
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

            if (cmr == null)
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

        public int ScanListViewGroup
        {
            get
            {
                if (this.Operation == Op.Rename)
                    return 1;
                if (this.Operation == Op.Copy)
                    return 2;
                if (this.Operation == Op.Move)
                    return 3;
                return 2;
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

        private void NicelyStopAndCleanUp(BinaryReader msr, BinaryWriter msw)
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
            if ((this.From == null) || (this.To == null))
                return false;

            return (this.IsMoveRename() && (this.From.Directory.Root.FullName.ToLower() == this.To.Directory.Root.FullName.ToLower())); // same device ... TODO: UNC paths?
        }

        private void OSMoveRename()
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

                this.Done = true;

                System.Diagnostics.Debug.Assert((this.Operation == ActionCopyMoveRename.Op.Move) || (this.Operation == ActionCopyMoveRename.Op.Rename));

                //TODO: Statistics
                //if (this.Operation == ActionCopyMoveRename.Op.Move)
                //    this.mStats.FilesMoved++;
                //else if (this.Operation == ActionCopyMoveRename.Op.Rename)
                //    this.mStats.FilesRenamed++;
            }
            catch (System.Exception e)
            {
                this.Done = true;
                this.Error = true;
                this.ErrorText = e.Message;
            }
        }

        private void CopyItOurself(ref bool pause)
        {
            const int kArrayLength = 256 * 1024;
            Byte[] dataArray = new Byte[kArrayLength];
            BinaryReader msr = null;
            BinaryWriter msw = null;

            try
            {
                long thisFileCopied = 0;
                long thisFileSize = this.SourceFileSize();

                msr = new BinaryReader(new FileStream(this.From.FullName, FileMode.Open, FileAccess.Read));
                string tempName = TempFor(this.To);
                if (File.Exists(tempName))
                    File.Delete(tempName);

                msw = new BinaryWriter(new FileStream(tempName, FileMode.CreateNew));

                int n = 0;

                do
                {
                    n = msr.Read(dataArray, 0, kArrayLength);
                    if (n != 0)
                        msw.Write(dataArray, 0, n);
                    thisFileCopied += n;

                    double pct = (thisFileSize != 0) ? (100.0 * thisFileCopied / thisFileSize) : this.Done ? 100 : 0;
                    if (pct > 100.0)
                        pct = 100.0;
                    this.PercentDone = pct;

                    while (pause)
                        System.Threading.Thread.Sleep(100);
                }
                while (n != 0);

                msr.Close();
                msw.Close();

                // rename temp version to final name
                if (this.To.Exists)
                    this.To.Delete(); // outta ma way!
                File.Move(tempName, this.To.FullName);

                // if that was a move/rename, delete the source
                if (this.IsMoveRename())
                    this.From.Delete();

                // TODO: Stats
                //if (this.Operation == ActionCopyMoveRename.Op.Move)
                //    this.mStats.FilesMoved++;
                //else if (this.Operation == ActionCopyMoveRename.Op.Rename)
                //    this.mStats.FilesRenamed++;
                //else if (this.Operation == ActionCopyMoveRename.Op.Copy)
                //    this.mStats.FilesCopied++;

                this.Done = true;
            } // try
            catch (System.Threading.ThreadAbortException)
            {
                this.NicelyStopAndCleanUp(msr, msw);
                return;
            }
            catch (IOException e)
            {
                this.Done = true;
                this.Error = true;
                this.ErrorText = e.Message;

                if (msw != null)
                    msw.Close();
                if (msr != null)
                    msr.Close();
            }
            catch (System.Exception ex)
            {
                // handle any other exception type
                this.Error = true;
                this.Done = true;
                this.ErrorText = ex.Message;
                this.NicelyStopAndCleanUp(msr, msw);
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