// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using System.Windows.Forms;
using System.IO;

namespace TVRename
{
    public enum CopyMoveResult
    {
        kCopyMoveOk,
        kUserCancelled,
        kFileError,
        kAlreadyExists
    }

    /// <summary>
    /// Summary for CopyMoveProgress
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class CopyMoveProgress : Form
    {
        #region Delegates

        public delegate void CopyDoneHandler();

        public delegate void FilenameHandler(string newName);

        public delegate void PercentHandler(int one, int two, int num);

        #endregion

        public const int kArrayLength = 256 * 1024;

        public CopyMoveResult Result;
        // String ^mErrorText;
        private System.Threading.Thread mCopyThread;
        private int mCurrentNum;
        private TVDoc mDoc;
        private TVRenameStats mStats;
        private ItemList mToDo;
        private bool Stop;

        public CopyMoveProgress(TVDoc doc, ItemList todo, TVRenameStats stats)
        {
            this.mDoc = doc;
            this.mToDo = todo;
            this.mStats = stats;
            this.InitializeComponent();
            this.copyTimer.Start();

            this.mCurrentNum = -1;

            this.CopyDone += this.CopyDoneFunc;
            this.Percent += this.SetPercentages;
            this.Filename += this.SetFilename;
        }

        public void CopyDoneFunc()
        {
            this.copyTimer.Stop();
            this.Close();
        }

        public event CopyDoneHandler CopyDone;
        public event PercentHandler Percent;
        public event FilenameHandler Filename;

        public void SetFilename(string filename)
        {
            this.txtFilename.Text = filename;
        }

        public void SetPercentages(int file, int group, int currentNum)
        {
            this.mCurrentNum = currentNum;
            //mPct = group;

            if (file > 1000)
                file = 1000;
            if (group > 1000)
                group = 1000;
            if (file < 0)
                file = 0;
            if (group < 0)
                group = 0;

            this.txtFile.Text = (file / 10) + "% Done";
            this.txtTotal.Text = (group / 10) + "% Done";

            this.pbFile.Value = file;
            this.pbGroup.Value = group;
            this.pbFile.Update();
            this.pbGroup.Update();
            this.txtFile.Update();
            this.txtTotal.Update();
            this.Update();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        ~CopyMoveProgress()
        {
            this.copyTimer.Stop();
        }

        private void copyTimer_Tick(object sender, System.EventArgs e)
        {
            if (this.mCurrentNum == -1)
            {
                this.pbDiskSpace.Value = 0;
                this.txtDiskSpace.Text = "--- GB free";
            }
            else
            {
                bool ok = false;
                Item action = this.mToDo[this.mCurrentNum];
                DirectoryInfo toWhere = null;

                if (action is ActionCopyMoveRename)
                    toWhere = ((ActionCopyMoveRename) action).To.Directory;
                else if (action is ActionDownload)
                    toWhere = ((ActionDownload) (action)).Destination.Directory;
                else if (action is ActionRSS)
                    toWhere = new FileInfo(((ActionRSS) (action)).TheFileNoExt).Directory;
                else if (action is ActionNFO)
                    toWhere = ((ActionNFO) (action)).Where.Directory;

                DirectoryInfo toRoot = null;
                if (toWhere.Name.StartsWith("\\\\"))
                    toRoot = null;
                else
                    toRoot = toWhere.Root;

                if (toRoot != null)
                {
                    System.IO.DriveInfo di;
                    try
                    {
                        // try to get root of drive
                        di = new System.IO.DriveInfo(toRoot.ToString());
                    }
                    catch (System.ArgumentException)
                    {
                        di = null;
                    }

                    if (di != null)
                    {
                        int pct = (int) ((1000 * di.TotalFreeSpace) / di.TotalSize);
                        this.pbDiskSpace.Value = 1000 - pct;
                        this.txtDiskSpace.Text = ((int) (di.TotalFreeSpace / 1024.0 / 1024.0 / 1024.0 + 0.5)) + " GB free";
                        ok = true;
                    }
                }

                if (!ok)
                {
                    this.txtDiskSpace.Text = "Unknown";
                    this.pbDiskSpace.Value = 0;
                }
            }

            this.pbDiskSpace.Update();
            this.txtDiskSpace.Update();
        }

        private static string TempFor(FileInfo f)
        {
            return f.FullName + ".tvrenametemp";
        }
        private void NicelyStopAndCleanUp(BinaryReader msr, BinaryWriter msw, ActionCopyMoveRename action)
        {
            if (msw != null)
            {
                msw.Close();
                string tempName = TempFor(action.To);
                if (File.Exists(tempName))
                    File.Delete(tempName);
            }
            if (msr != null)
                msr.Close();
        }

        private void CopyMachine()
        {
            Byte[] dataArray = new Byte[kArrayLength];
            BinaryReader msr = null;
            BinaryWriter msw = null;

            long totalSize = 0;
            long totalCopiedSoFar = 0;

            totalSize = 0;
            totalCopiedSoFar = 0;

            int nfoCount = 0;
            int downloadCount = 0;

            for (int i = 0; i < this.mToDo.Count; i++)
            {
                if (this.mToDo[i] is ActionCopyMoveRename)
                    totalSize += ((ActionCopyMoveRename) this.mToDo[i]).FileSize();
                else if (this.mToDo[i] is ActionNFO)
                    nfoCount++;
                else if (this.mToDo[i] is ActionDownload)
                    downloadCount++;
                else if (this.mToDo[i] is ActionRSS)
                    downloadCount++;
            }

            int extrasCount = nfoCount + downloadCount;
            long sizePerExtra = 1;
            if ((extrasCount > 0) && (totalSize != 0))
                sizePerExtra = totalSize / (10 * extrasCount);
            if (sizePerExtra == 0)
                sizePerExtra = 1;
            totalSize += sizePerExtra * extrasCount;

            int extrasDone = 0;

            for (int i = 0; i < this.mToDo.Count; i++)
            {
                while (this.cbPause.Checked)
                    System.Threading.Thread.Sleep(100);

                Item action1 = this.mToDo[i];

                if ((action1 is ActionRSS) || (action1 is ActionRSS) || (action1 is ActionDownload))
                {
                    this.BeginInvoke(this.Filename, action1.FilenameForProgress());

                    this.BeginInvoke(this.Percent, (extrasCount != 0) ? (1000 * extrasDone / extrasCount) : 0, (totalSize != 0) ? (int) (1000 * totalCopiedSoFar / totalSize) : 50, i);

                    action1.Action(this.mDoc);
                    extrasDone++;
                    totalCopiedSoFar += sizePerExtra;
                }
                else if (action1 is ActionCopyMoveRename)
                {
                    ActionCopyMoveRename action = (ActionCopyMoveRename) (action1);
                    action.Error = false;
                    action.ErrorText = "";

                    this.BeginInvoke(this.Filename, action.FilenameForProgress());

                    long thisFileSize = action.FileSize();

                    System.Security.AccessControl.FileSecurity security = null;
                    try
                    {
                        security = action.From.GetAccessControl();
                    }
                    catch
                    {
                    }

                    if (action.IsMoveRename() && (action.From.Directory.Root.FullName.ToLower() == action.To.Directory.Root.FullName.ToLower())) // same device ... TODO: UNC paths?
                    {
                        // ask the OS to do it for us, since it's easy and quick!
                        try
                        {
                            if (Helpers.Same(action.From, action.To))
                            {
                                // XP won't actually do a rename if its only a case difference
                                string tempName = TempFor(action.To);
                                action.From.MoveTo(tempName);
                                File.Move(tempName, action.To.FullName);
                            }
                            else
                                action.From.MoveTo(action.To.FullName);

                            action.Done = true;

                            System.Diagnostics.Debug.Assert((action.Operation == ActionCopyMoveRename.Op.Move) || (action.Operation == ActionCopyMoveRename.Op.Rename));
                            if (action.Operation == ActionCopyMoveRename.Op.Move)
                                this.mStats.FilesMoved++;
                            else if (action.Operation == ActionCopyMoveRename.Op.Rename)
                                this.mStats.FilesRenamed++;
                        }
                        catch (System.Exception e)
                        {
                            action.Done = true;
                            action.Error = true;
                            action.ErrorText = e.Message;
                            totalCopiedSoFar += thisFileSize;
                        }
                    }
                    else
                    {
                        // do it ourself!
                        try
                        {
                            long thisFileCopied = 0;
                            msr = null;
                            msw = null;

                            msr = new BinaryReader(new FileStream(action.From.FullName, FileMode.Open, FileAccess.Read));
                            string tempName = TempFor(action.To);
                            if (File.Exists(tempName))
                                File.Delete(tempName);

                            msw = new BinaryWriter(new FileStream(tempName, FileMode.CreateNew));

                            int n = 0;

                            do
                            {
                                n = msr.Read(dataArray, 0, kArrayLength);
                                if (n != 0)
                                    msw.Write(dataArray, 0, n);
                                totalCopiedSoFar += n;
                                thisFileCopied += n;

                                this.BeginInvoke(this.Percent, (thisFileSize != 0) ? (int)(1000 * thisFileCopied / thisFileSize) : 50, (totalSize != 0) ? (int)(1000 * totalCopiedSoFar / totalSize) : 50, i);

                                while (this.cbPause.Checked)
                                    System.Threading.Thread.Sleep(100);
                                if (this.Stop)
                                {
                                    this.NicelyStopAndCleanUp(msr,msw,action);
                                    this.BeginInvoke(this.CopyDone);
                                    return;
                                }
                            }
                            while (n != 0);

                            msr.Close();
                            msw.Close();

                            // rename temp version to final name
                            if (action.To.Exists)
                                action.To.Delete(); // outta ma way!
                            File.Move(tempName, action.To.FullName);

                            // if that was a move/rename, delete the source
                            if (action.IsMoveRename())
                                action.From.Delete();

                            if (action.Operation == ActionCopyMoveRename.Op.Move)
                                this.mStats.FilesMoved++;
                            else if (action.Operation == ActionCopyMoveRename.Op.Rename)
                                this.mStats.FilesRenamed++;
                            else if (action.Operation == ActionCopyMoveRename.Op.Copy)
                                this.mStats.FilesCopied++;

                            action.Done = true;
                        } // try
                        catch (IOException e)
                        {
                            action.Done = true;
                            action.Error = true;
                            action.ErrorText = e.Message;

                            this.Result = CopyMoveResult.kAlreadyExists;
                            if (msw != null)
                                msw.Close();
                            if (msr != null)
                                msr.Close();

                            totalCopiedSoFar += thisFileSize;
                        }
                        catch (System.Exception ex)
                        {
                            // handle any other exception type
                            this.Result = CopyMoveResult.kFileError;
                            action.Error = true;
                            action.ErrorText = ex.Message;
                            this.NicelyStopAndCleanUp(msr,msw,action);

                            totalCopiedSoFar += thisFileSize;
                        }
                    } // do it ourself
                    try
                    {
                        if (security != null)
                            action.To.SetAccessControl(security);
                    }
                    catch
                    {
                    }
                } // if copymoverename
            } // for each source

            this.Result = CopyMoveResult.kCopyMoveOk;
            this.BeginInvoke(this.CopyDone);
        }

        // CopyMachine

        private void button1_Click(object sender, System.EventArgs e)
        {
            this.Stop = true;
            this.Result = CopyMoveResult.kUserCancelled;
        }

        private void CopyMoveProgress_Load(object sender, System.EventArgs e)
        {
            this.Stop = false;
            this.mCopyThread = new System.Threading.Thread(this.CopyMachine);
            this.mCopyThread.Name = "Copy Thread";
            this.mCopyThread.Start();
        }

        private void cbPause_CheckedChanged(object sender, System.EventArgs e)
        {
            bool en = !(this.cbPause.Checked);
            this.pbFile.Enabled = en;
            this.pbGroup.Enabled = en;
            this.pbDiskSpace.Enabled = en;
            this.txtFile.Enabled = en;
            this.txtTotal.Enabled = en;
            this.txtDiskSpace.Enabled = en;
            this.label1.Enabled = en;
            this.label2.Enabled = en;
            this.label4.Enabled = en;
            this.label3.Enabled = en;
            this.txtFilename.Enabled = en;
        }
    }
}