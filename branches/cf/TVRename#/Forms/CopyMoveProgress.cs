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
        private System.Collections.Generic.List<ActionItem> mToDo;

        public CopyMoveProgress(TVDoc doc, System.Collections.Generic.List<ActionItem> todo, TVRenameStats stats)
        {
            this.mDoc = doc;
            this.mToDo = todo;
            this.mStats = stats;
            this.InitializeComponent();
            this.copyTimer.Start();

            this.mCurrentNum = -1;
            //mLastPct = -1;
            //			mErrorText = "";
            //			ErrorFiles = gcnew RCList();

            this.CopyDone += this.CopyDoneFunc;
            this.Percent += this.SetPercentages;
            this.Filename += this.SetFilename;
        }

        //C++ TO C# CONVERTER NOTE: Embedded comments are not maintained by C++ to C# Converter
        //ORIGINAL LINE: void CopyDoneFunc(/*CopyMoveResult why, String ^errorText*/)
        public void CopyDoneFunc()
        {
            this.copyTimer.Stop();
            this.Close();
        }

        public event CopyDoneHandler CopyDone;
        public event PercentHandler Percent;
        public event FilenameHandler Filename;
        //            
        //			String ^ErrorText() 
        //			{ 
        //			String ^t = mErrorText;
        //
        //			if (t->Length > 0) // last char will be an extra \r we don't really want
        //			t->Remove(mErrorText->Length-1);
        //
        //			return t;
        //			}
        //			
        //			RCList ^ErrFiles() { return ErrorFiles; }

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
                ActionItem Action = this.mToDo[this.mCurrentNum];
                DirectoryInfo toWhere = null;

                if (Action.Type == ActionType.kCopyMoveRename)
                    toWhere = ((ActionCopyMoveRename) Action).To.Directory;
                else if (Action.Type == ActionType.kDownload)
                    toWhere = ((ActionDownload) (Action)).Destination.Directory;
                else if (Action.Type == ActionType.kRSS)
                    toWhere = new FileInfo(((ActionRSS) (Action)).TheFileNoExt).Directory;
                else if (Action.Type == ActionType.kNFO)
                    toWhere = ((ActionNFO) (Action)).Where.Directory;

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
                if (this.mToDo[i].Type == ActionType.kCopyMoveRename)
                    totalSize += ((ActionCopyMoveRename) this.mToDo[i]).FileSize();
                else if (this.mToDo[i].Type == ActionType.kNFO)
                    nfoCount++;
                else if (this.mToDo[i].Type == ActionType.kDownload)
                    downloadCount++;
                else if (this.mToDo[i].Type == ActionType.kRSS)
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

                ActionItem Action1 = this.mToDo[i];

                if ((Action1.Type == ActionType.kRSS) || (Action1.Type == ActionType.kRSS) || (Action1.Type == ActionType.kDownload))
                {
                    this.BeginInvoke(this.Filename, Action1.FilenameForProgress());

                    this.BeginInvoke(this.Percent, (extrasCount != 0) ? (1000 * extrasDone / extrasCount) : 0, (totalSize != 0) ? (int) (1000 * totalCopiedSoFar / totalSize) : 50, i);

                    Action1.Action(this.mDoc);
                    extrasDone++;
                    totalCopiedSoFar += sizePerExtra;
                }
                else if (Action1.Type == ActionType.kCopyMoveRename)
                {
                    ActionCopyMoveRename Action = (ActionCopyMoveRename) (Action1);
                    Action.HasError = false;
                    Action.ErrorText = "";

                    this.BeginInvoke(this.Filename, Action.FilenameForProgress());

                    long thisFileSize;
                    thisFileSize = Action.FileSize();

                    System.Security.AccessControl.FileSecurity security = null;
                    try
                    {
                        security = Action.From.GetAccessControl();
                    }
                    catch
                    {
                    }

                    if (Action.IsMoveRename() && (Action.From.Directory.Root.FullName.ToLower() == Action.To.Directory.Root.FullName.ToLower())) // same device ... TODO: UNC paths?
                    {
                        // ask the OS to do it for us, since it's easy and quick!
                        try
                        {
                            if (Helpers.Same(Action.From, Action.To))
                            {
                                // XP won't actually do a rename if its only a case difference
                                string tempName = TempFor(Action.To);
                                Action.From.MoveTo(tempName);
                                File.Move(tempName, Action.To.FullName);
                            }
                            else
                                Action.From.MoveTo(Action.To.FullName);

                            Action.Done = true;

                            System.Diagnostics.Debug.Assert((Action.Operation == ActionCopyMoveRename.Op.Move) || (Action.Operation == ActionCopyMoveRename.Op.Rename));
                            if (Action.Operation == ActionCopyMoveRename.Op.Move)
                                this.mStats.FilesMoved++;
                            else if (Action.Operation == ActionCopyMoveRename.Op.Rename)
                                this.mStats.FilesRenamed++;
                        }
                        catch (System.Exception e)
                        {
                            Action.Done = true;
                            Action.HasError = true;
                            Action.ErrorText = e.Message;
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

                            msr = new BinaryReader(new FileStream(Action.From.FullName, FileMode.Open));
                            string tempName = TempFor(Action.To);
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

                                this.BeginInvoke(this.Percent, (thisFileSize != 0) ? (int) (1000 * thisFileCopied / thisFileSize) : 50, (totalSize != 0) ? (int) (1000 * totalCopiedSoFar / totalSize) : 50, i);

                                while (this.cbPause.Checked)
                                    System.Threading.Thread.Sleep(100);
                            }
                            while (n != 0);

                            msr.Close();
                            msw.Close();

                            // rename temp version to final name
                            if (Action.To.Exists)
                                Action.To.Delete(); // outta ma way!
                            File.Move(tempName, Action.To.FullName);

                            // if that was a move/rename, delete the source
                            if (Action.IsMoveRename())
                                Action.From.Delete();

                            if (Action.Operation == ActionCopyMoveRename.Op.Move)
                                this.mStats.FilesMoved++;
                            else if (Action.Operation == ActionCopyMoveRename.Op.Rename)
                                this.mStats.FilesRenamed++;
                            else if (Action.Operation == ActionCopyMoveRename.Op.Copy)
                                this.mStats.FilesCopied++;

                            Action.Done = true;
                        } // try
                        catch (IOException e)
                        {
                            Action.Done = true;
                            Action.HasError = true;
                            Action.ErrorText = e.Message;

                            this.Result = CopyMoveResult.kAlreadyExists;
                            if (msw != null)
                                msw.Close();
                            if (msr != null)
                                msr.Close();

                            totalCopiedSoFar += thisFileSize;
                        }
                        catch (System.Threading.ThreadAbortException)
                        {
                            this.Result = CopyMoveResult.kUserCancelled;
                            //for (int k=i;k<mSources->Count;k++)
                            // ErrorFiles->Add(mSources[k]); // what was skipped

                            if (msw != null)
                            {
                                msw.Close();
                                string tempName = TempFor(Action.To);
                                if (File.Exists(tempName))
                                    File.Delete(tempName);
                            }
                            if (msr != null)
                                msr.Close();
                            this.BeginInvoke(this.CopyDone);
                            return;
                        }
                        catch (System.Exception e)
                        {
                            this.Result = CopyMoveResult.kFileError;
                            Action.HasError = true;
                            Action.ErrorText = e.Message;
                            if (msw != null)
                            {
                                msw.Close();
                                if (File.Exists(TempFor(Action.To)))
                                    File.Delete(TempFor(Action.To));
                            }
                            if (msr != null)
                                msr.Close();

                            totalCopiedSoFar += thisFileSize;
                        }
                    } // do it ourself
                    try
                    {
                        if (security != null)
                            Action.To.SetAccessControl(security);
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
            this.mCopyThread.Interrupt();
            //CopyDone(false);
        }

        private void CopyMoveProgress_Load(object sender, System.EventArgs e)
        {
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