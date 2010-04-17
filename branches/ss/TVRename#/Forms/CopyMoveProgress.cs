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

        private TVDoc mDoc;
        private TVRenameStats mStats;
        private ScanListItemList mToDo;

        public CopyMoveProgress(TVDoc doc, ScanListItemList todo, TVRenameStats stats)
        {
            this.mDoc = doc;
            this.mToDo = todo;
            this.mStats = stats;
            this.InitializeComponent();
            this.copyTimer.Start();
        }

        public void SetPercentages(double file, double group)
        {
            if (file > 100)
                file = 100;
            if (group > 100)
                group = 100;
            if (file < 0)
                file = 0;
            if (group < 0)
                group = 0;

            this.txtFile.Text = ((int)Math.Round(file)) + "% Done";
            this.txtTotal.Text = ((int)Math.Round(group)) + "% Done";

            // progress bars go 0 to 1000
            this.pbFile.Value = (int)(10.0 * file);
            this.pbGroup.Value = (int)(10.0 * group);
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
            this.copyTimer.Stop();

            double workDone = 0;
            double totalWork = 0;
            double workingPercent = 0;
            double workingTotal = 0;

            bool allDone = true;

            foreach (ScanListItem sli in mToDo)
            {
                Action action = sli as Action;
                if (action == null)
                    continue;

                if (!action.Done)
                    allDone = false;

                workDone += action.PercentDone;
                totalWork += 100.0;

                if ((action.PercentDone > 0.0) && (!action.Done))
                {
                    workingPercent += action.PercentDone;
                    workingTotal += 100.0;
                }
            }

            if (totalWork != 0.0)
                workDone = workDone * 100.0 / totalWork;
            if (workingTotal != 0.0)
                workingPercent = workingPercent * 100.0 / workingTotal;

            this.SetPercentages(workingPercent, workDone);

            if (allDone)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
                this.copyTimer.Start();
            /* 
             if (this.mCurrentNum == -1)
             {
                 this.pbDiskSpace.Value = 0;
                 this.txtDiskSpace.Text = "--- GB free";
             }
             else
             {
                 bool ok = false;
                 //Action action = this.mToDo[this.mCurrentNum] as Action;
                 ScanListItem sli = this.mToDo[this.mCurrentNum];

                 string folder = sli.TargetFolder;
                 DirectoryInfo toRoot = (!string.IsNullOrEmpty(folder) && !folder.StartsWith("\\\\")) ? new DirectoryInfo(folder).Root : null;

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
             */
        }

        private void bnCancel_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cbPause_CheckedChanged(object sender, System.EventArgs e)
        {
            throw new NotImplementedException();

#pragma warning disable 162
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
#pragma warning restore 162
        }
    }
}