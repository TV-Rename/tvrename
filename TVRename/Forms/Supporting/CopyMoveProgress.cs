// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

namespace TVRename
{
    using System;
    using Alphaleonis.Win32.Filesystem;
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
        private readonly ActionEngine mDoc;
        private readonly ActionQueue[] mToDo;

        public CopyMoveProgress(ActionEngine doc, ActionQueue[] todo)
        {
            mDoc = doc;
            mToDo = todo;
            InitializeComponent();
            copyTimer.Start();
            diskSpaceTimer.Start();
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

            txtFile.Text = ((int) Math.Round(file)) + "% Done";
            txtTotal.Text = ((int) Math.Round(group)) + "% Done";

            // progress bars go 0 to 1000
            pbFile.Value = (int) (10.0 * file);
            pbGroup.Value = (int) (10.0 * group);
            pbFile.Update();
            pbGroup.Update();
            txtFile.Update();
            txtTotal.Update();
            Update();
            BringToFront();
        }

        private bool UpdateCopyProgress() // return true if all tasks are done
        {
            // update each listview item, for non-empty queues
            bool allDone = true;

            lvProgress.BeginUpdate();
            int top = lvProgress.TopItem?.Index ?? 0;
            ActionCopyMoveRename activeCmAction = GetActiveCmAction();
            long workDone = 0;
            long totalWork = 0;
            lvProgress.Items.Clear();

            foreach (ActionQueue aq in mToDo)
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
                        ListViewItem lvi = new ListViewItem(action.Name);
                        lvi.SubItems.Add(action.ProgressText);

                        lvProgress.Items.Add(lvi);
                    }
                }
            }

            if (top >= lvProgress.Items.Count)
                top = lvProgress.Items.Count - 1;
            if (top >= 0)
                lvProgress.TopItem = lvProgress.Items[top];
            lvProgress.EndUpdate();

            if (activeCmAction != null)
            {
                txtFilename.Text = activeCmAction.ProgressText;
                SetPercentages(activeCmAction.PercentDone, totalWork == 0 ? 0.0 : (workDone * 100.0 / totalWork));
            }

            return allDone;
        }

        private void copyTimer_Tick(object sender, EventArgs e)
        {
            copyTimer.Stop();

            //this.UpdateOldStyle();
            bool allDone = UpdateCopyProgress();

            if (allDone)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
                copyTimer.Start();
        }

        private void diskSpaceTimer_Tick(object sender, EventArgs e)
        {
            diskSpaceTimer.Stop();
            UpdateDiskSpace();
            diskSpaceTimer.Start();
        }

        private void UpdateDiskSpace()
        {
            int diskValue = 0;
            string diskText = "--- GB free";

            ActionCopyMoveRename activeCmAction = GetActiveCmAction();

            if (activeCmAction is null) return;

            string folder = activeCmAction.TargetFolder;
            DirectoryInfo toRoot = (!string.IsNullOrEmpty(folder) && !folder.StartsWith("\\\\")) ? new DirectoryInfo(folder).Root : null;

            if (toRoot != null)
            {
                System.IO.DriveInfo di;
                try
                {
                    // try to get root of drive
                    di = new System.IO.DriveInfo(toRoot.ToString());
                }
                catch (ArgumentException)
                {
                    di = null;
                }

                if (di != null)
                {
                    int pct = (int)((1000 * di.TotalFreeSpace) / di.TotalSize);
                    diskValue = 1000 - pct;
                    diskText = di.TotalFreeSpace.GBMB(1) + " free";
                }
            }

            DirectoryInfo toUncRoot = (!string.IsNullOrEmpty(folder) && folder.StartsWith("\\\\")) ? new DirectoryInfo(folder).Root : null;
            if (toUncRoot != null)
            {
                FileSystemProperties driveStats = FileHelper.GetProperties(toUncRoot.ToString());
                if (driveStats?.AvailableBytes != null && driveStats.TotalBytes.HasValue)
                {
                    int pct = (int)((1000 * driveStats.AvailableBytes) / driveStats.TotalBytes);
                    diskValue = 1000 - pct;
                    diskText = (driveStats.AvailableBytes??0).GBMB(1) + " free";
                }
            }
            
            pbDiskSpace.Value = diskValue;
            txtDiskSpace.Text = diskText;
        }

        private ActionCopyMoveRename GetActiveCmAction()
        {
            foreach (ActionQueue aq in mToDo)
            {
                if (aq.Actions.Count == 0)
                    continue;

                foreach (Action action in aq.Actions)
                {
                    if ((!action.Done) && (action.PercentDone > 0))
                    {
                        if (action is ActionCopyMoveRename cmAction )
                            return cmAction;
                    }
                }
            }

            return null;
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void cbPause_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPause.Checked)
                mDoc.Pause();
            else
                mDoc.Unpause();

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
    }
}
