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
    public enum CopyMoveResult : int
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
        public const int kArrayLength = 256 * 1024;

        public CopyMoveResult Result;
        // String ^mErrorText;
        private System.Collections.Generic.List<ActionItem> mToDo;
        //RCList ^mSources, ^ErrorFiles;
        //MissingEpisodeList ^MissingList;
        private int mCurrentNum;
        private TVRenameStats mStats;
        private System.Threading.Thread mCopyThread;
        private TVDoc mDoc;

        public delegate void CopyDoneHandler();
        public delegate void PercentHandler(int one, int two, int num);
        public delegate void FilenameHandler(string newName);

        //C++ TO C# CONVERTER NOTE: Embedded comments are not maintained by C++ to C# Converter
        //ORIGINAL LINE: void CopyDoneFunc(/*CopyMoveResult why, String ^errorText*/)
        public void CopyDoneFunc()
        {
            copyTimer.Stop();
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

        public CopyMoveProgress(TVDoc doc, System.Collections.Generic.List<ActionItem> todo, TVRenameStats stats)
        {
            mDoc = doc;
            mToDo = todo;
            mStats = stats;
            InitializeComponent();
            copyTimer.Start();

            mCurrentNum = -1;
            //mLastPct = -1;
            //			mErrorText = "";
            //			ErrorFiles = gcnew RCList();

            this.CopyDone += CopyDoneFunc;
            this.Percent += new PercentHandler(SetPercentages);
            this.Filename += new FilenameHandler(SetFilename);
        }

        public void SetFilename(string filename)
        {
            txtFilename.Text = filename;
        }
        public void SetPercentages(int file, int group, int currentNum)
        {
            mCurrentNum = currentNum;
            //mPct = group;

            if (file > 1000)
                file = 1000;
            if (group > 1000)
                group = 1000;
            if (file < 0)
                file = 0;
            if (group < 0)
                group = 0;

            txtFile.Text = (file / 10).ToString() + "% Done";
            txtTotal.Text = (group / 10).ToString() + "% Done";

            pbFile.Value = file;
            pbGroup.Value = group;
            pbFile.Update();
            pbGroup.Update();
            txtFile.Update();
            txtTotal.Update();
            Update();
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        ~CopyMoveProgress()
        {
            copyTimer.Stop();
        }

        private void copyTimer_Tick(object sender, System.EventArgs e)
        {
            if (mCurrentNum == -1)
            {
                pbDiskSpace.Value = 0;
                txtDiskSpace.Text = "--- GB free";
            }
            else
            {
                bool ok = false;
                ActionItem Action = mToDo[mCurrentNum];
                DirectoryInfo toWhere = null;

                if (Action.Type == ActionType.kCopyMoveRename)
                    toWhere = ((ActionCopyMoveRename)Action).To.Directory;
                else if (Action.Type == ActionType.kDownload)
                    toWhere = ((ActionDownload)(Action)).Destination.Directory;
                else if (Action.Type == ActionType.kRSS)
                    toWhere = new FileInfo(((ActionRSS)(Action)).TheFileNoExt).Directory;
                else if (Action.Type == ActionType.kNFO)
                    toWhere = ((ActionNFO)(Action)).Where.Directory;

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
                        int pct = (int)((1000 * di.TotalFreeSpace) / di.TotalSize);
                        pbDiskSpace.Value = 1000 - pct;
                        txtDiskSpace.Text = ((int)((double)di.TotalFreeSpace / 1024.0 / 1024.0 / 1024.0 + 0.5)).ToString() + " GB free";
                        ok = true;
                    }
                }

                if (!ok)
                {
                    txtDiskSpace.Text = "Unknown";
                    pbDiskSpace.Value = 0;
                }

            }

            pbDiskSpace.Update();
            txtDiskSpace.Update();
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

            for (int i = 0; i < mToDo.Count; i++)
            {
                if (mToDo[i].Type == ActionType.kCopyMoveRename)
                    totalSize += ((ActionCopyMoveRename)mToDo[i]).FileSize();
                else if (mToDo[i].Type == ActionType.kNFO)
                    nfoCount++;
                else if (mToDo[i].Type == ActionType.kDownload)
                    downloadCount++;
                else if (mToDo[i].Type == ActionType.kRSS)
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

            for (int i = 0; i < mToDo.Count; i++)
            {
                while (cbPause.Checked)
                    System.Threading.Thread.Sleep(100);

                ActionItem Action1 = mToDo[i];

                if ((Action1.Type == ActionType.kRSS) || (Action1.Type == ActionType.kRSS) || (Action1.Type == ActionType.kDownload))
                {
                    this.BeginInvoke(this.Filename, Action1.FilenameForProgress());

                    this.BeginInvoke(this.Percent,
                        (extrasCount != 0) ? (int)(1000 * extrasDone / extrasCount) : 0,
                        (totalSize != 0) ? (int)(1000 * totalCopiedSoFar / totalSize) : 50,
                        i);

                    Action1.Action(mDoc);
                    extrasDone++;
                    totalCopiedSoFar += sizePerExtra;
                }
                else if (Action1.Type == ActionType.kCopyMoveRename)
                {
                    ActionCopyMoveRename Action = (ActionCopyMoveRename)(Action1);
                    Action.HasError = false;
                    Action.ErrorText = "";

                    this.BeginInvoke(Filename, Action.FilenameForProgress());

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
                                mStats.FilesMoved++;
                            else if (Action.Operation == ActionCopyMoveRename.Op.Rename)
                                mStats.FilesRenamed++;
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

                                this.BeginInvoke(Percent,
                                    (thisFileSize != 0) ? (int)(1000 * thisFileCopied / thisFileSize) : 50,
                                    (totalSize != 0) ? (int)(1000 * totalCopiedSoFar / totalSize) : 50,
                                    i);

                                while (cbPause.Checked)
                                    System.Threading.Thread.Sleep(100);
                            } while (n != 0);

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
                                mStats.FilesMoved++;
                            else if (Action.Operation == ActionCopyMoveRename.Op.Rename)
                                mStats.FilesRenamed++;
                            else if (Action.Operation == ActionCopyMoveRename.Op.Copy)
                                mStats.FilesCopied++;

                            Action.Done = true;
                        } // try
                        catch (IOException e)
                        {
                            Action.Done = true;
                            Action.HasError = true;
                            Action.ErrorText = e.Message;

                            Result = CopyMoveResult.kAlreadyExists;
                            if (msw != null)
                                msw.Close();
                            if (msr != null)
                                msr.Close();

                            totalCopiedSoFar += thisFileSize;
                        }
                        catch (System.Threading.ThreadAbortException)
                        {
                            Result = CopyMoveResult.kUserCancelled;
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
                            this.BeginInvoke(CopyDone);
                            return;
                        }
                        catch (System.Exception e)
                        {
                            Result = CopyMoveResult.kFileError;
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

            Result = CopyMoveResult.kCopyMoveOk;
            this.BeginInvoke(CopyDone);
        } // CopyMachine

        private void button1_Click(object sender, System.EventArgs e)
        {
            mCopyThread.Interrupt();
            //CopyDone(false);

        }
        private void CopyMoveProgress_Load(object sender, System.EventArgs e)
        {

            mCopyThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.CopyMachine));
            mCopyThread.Name = "Copy Thread";
            mCopyThread.Start();
        }

        private void cbPause_CheckedChanged(object sender, System.EventArgs e)
        {
            bool en = !(cbPause.Checked);
            pbFile.Enabled = en;
            pbGroup.Enabled = en;
            pbDiskSpace.Enabled = en;
            txtFile.Enabled = en;
            txtTotal.Enabled = en;
            txtDiskSpace.Enabled = en;
            label1.Enabled = en;
            label2.Enabled = en;
            label4.Enabled = en;
            label3.Enabled = en;
            txtFilename.Enabled = en;
        }
    } // class
} // namespace