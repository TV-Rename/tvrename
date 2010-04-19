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
        private ActionQueue[] mToDo;

        public CopyMoveProgress(TVDoc doc, ActionQueue[] todo, TVRenameStats stats)
        {
            this.mDoc = doc;
            this.mToDo = todo;
            this.mStats = stats;
            this.InitializeComponent();
            this.copyTimer.Start();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        ~CopyMoveProgress()
        {
            this.copyTimer.Stop();
        }

        private void SetPercentages(double file, double group)
        {
            if (file > 100)
                file = 100;
            if (group > 100)
                group = 100;
            if (file < 0)
                file = 0;
            if (group < 0)
                group = 0;

            this.txtFile.Text = ((int) Math.Round(file)) + "% Done";
            this.txtTotal.Text = ((int) Math.Round(group)) + "% Done";

            // progress bars go 0 to 1000
            this.pbFile.Value = (int) (10.0 * file);
            this.pbGroup.Value = (int) (10.0 * group);
            this.pbFile.Update();
            this.pbGroup.Update();
            this.txtFile.Update();
            this.txtTotal.Update();
            this.Update();
        }

        private bool UpdateNewStyle() // return true if all tasks are done
        {
            // update each listview item, for non-empty queues
            bool allDone = true;

            this.lvProgress.BeginUpdate();
            int top = this.lvProgress.TopItem != null ? this.lvProgress.TopItem.Index : 0;
            ActionCopyMoveRename activeCMAction = null;
            long workDone = 0;
            long totalWork = 0;
            this.lvProgress.Items.Clear();
            foreach (ActionQueue aq in this.mToDo)
            {
                if (aq.Actions.Count == 0)
                    continue;

                foreach (Action action in aq.Actions)
                {
                    if (!action.Done)
                        allDone = false;

                    long size = action.SizeOfWork;
                    workDone += (long) (size * action.PercentDone / 100);
                    totalWork += action.SizeOfWork;

                    if (!action.Done)
                    {
                        if ((action is ActionCopyMoveRename) && (action.PercentDone > 0))
                            activeCMAction = action as ActionCopyMoveRename;

                        ListViewItem lvi = new ListViewItem();

                        lvi.Text = action.Name;
                        lvi.SubItems.Add(action.ProgressText);

                        this.lvProgress.Items.Add(lvi);
                    }
                }
            }

            if (top >= this.lvProgress.Items.Count)
                top = this.lvProgress.Items.Count - 1;
            if (top >= 0)
                this.lvProgress.TopItem = this.lvProgress.Items[top];
            this.lvProgress.EndUpdate();

            int diskValue = 0;
            string diskText = "--- GB free";
            string fileText = "";

            if (activeCMAction != null)
            {
                string folder = activeCMAction.TargetFolder;
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
                        diskValue = 1000 - pct;
                        diskText = ((int) (di.TotalFreeSpace / 1024.0 / 1024.0 / 1024.0 + 0.5)) + " GB free";
                    }

                    fileText = activeCMAction.ProgressText;
                }

                this.txtFilename.Text = fileText;
                this.pbDiskSpace.Value = diskValue;
                this.txtDiskSpace.Text = diskText;

                if (totalWork != 0.0)
                    workDone = workDone * 100 / totalWork;

                this.SetPercentages(activeCMAction.PercentDone, workDone);
            }

            return allDone;
        }

        private void copyTimer_Tick(object sender, System.EventArgs e)
        {
            this.copyTimer.Stop();

            //this.UpdateOldStyle();
            bool allDone = this.UpdateNewStyle();

            if (allDone)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
                this.copyTimer.Start();
        }

        private void bnCancel_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cbPause_CheckedChanged(object sender, System.EventArgs e)
        {
            this.mDoc.ActionPause = this.cbPause.Checked;

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