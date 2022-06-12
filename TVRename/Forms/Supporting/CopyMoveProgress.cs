//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using JetBrains.Annotations;
using Alphaleonis.Win32.Filesystem;
using System;
using System.Windows.Forms;

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
        private const int MAX_PROGRESS_BAR = 1000;
        private readonly ActionEngine mDoc;
        private readonly ItemList mToDo;
        private readonly System.Action mDoOnClose;

        public CopyMoveProgress([NotNull] TVDoc engine, [NotNull] TVDoc.ActionSettings settings, System.Action doOnClose)
        {
            mDoc = engine.ActionManager;
            mToDo =settings.DoAll ? engine.TheActionList: settings.Lvr;
            mDoOnClose = doOnClose;
            InitializeComponent();
            copyTimer.Start();
            diskSpaceTimer.Start();
        }

        private static int Normalise(double x)
        {
            if (x < 0)
            {
                return 0;
            }

            if (x > 100)
            {
                return 100;
            }

            return (int)Math.Round(x);
        }

        private void SetPercentages(double file, double group)
        {
            txtFile.Text = Normalise(file) + "% Done";
            txtTotal.Text = Normalise(group) + "% Done";

            // progress bars go 0 to MAX_PROGRESS_BAR
            pbFile.Value = MAX_PROGRESS_BAR / 100 * Normalise(file);
            pbGroup.Value = MAX_PROGRESS_BAR / 100 * Normalise(group);
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

            foreach (Action action in mToDo.Actions)
            {
                if (!action.Outcome.Done)
                {
                    allDone = false;
                }

                long size = action.SizeOfWork;
                workDone += (long)(size * action.PercentDone / 100);
                totalWork += action.SizeOfWork;

                if (!action.Outcome.Done)
                {
                    ListViewItem lvi = new(action.Name);
                    lvi.SubItems.Add(action.ProgressText);

                    lvProgress.Items.Add(lvi);
                }
            }

            if (top >= lvProgress.Items.Count)
            {
                top = lvProgress.Items.Count - 1;
            }

            if (top >= 0)
            {
                try
                {
                    lvProgress.TopItem = lvProgress.Items[top];
                }
                catch (NullReferenceException)
                {
                    //Ignore this as we're done
                }
            }

            lvProgress.EndUpdate();

            if (activeCmAction != null)
            {
                txtFilename.Text = activeCmAction.ProgressText.ToUiVersion();
                SetPercentages(activeCmAction.PercentDone, totalWork == 0 ? 0.0 : workDone * 100.0 / totalWork);
            }

            return allDone;
        }

        private void copyTimer_Tick(object sender, EventArgs e)
        {
            copyTimer.Stop();

            bool allDone = UpdateCopyProgress();

            if (allDone)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                copyTimer.Start();
            }
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

            if (activeCmAction is null)
            {
                return;
            }

            string folder = activeCmAction.TargetFolder;
            DirectoryInfo toRoot = !string.IsNullOrEmpty(folder) && !folder.StartsWith("\\\\", StringComparison.Ordinal) ? new DirectoryInfo(folder).Root : null;

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

                try
                {
                    if (di != null)
                    {
                        (diskValue, diskText) = DiskValue(di.TotalFreeSpace, di.TotalSize);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (System.IO.IOException)
                {
                }
            }

            DirectoryInfo toUncRoot = !string.IsNullOrEmpty(folder) && folder.StartsWith("\\\\", StringComparison.Ordinal) ? new DirectoryInfo(folder).Root : null;
            if (toUncRoot != null)
            {
                FileSystemProperties driveStats = FileHelper.GetProperties(toUncRoot.ToString());
                long? availableBytes = driveStats.AvailableBytes;
                long? totalBytes = driveStats.TotalBytes;
                if (availableBytes.HasValue && totalBytes.HasValue)
                {
                    (diskValue, diskText) = DiskValue(availableBytes.Value, totalBytes.Value);
                }
            }

            pbDiskSpace.Value = diskValue;
            txtDiskSpace.Text = diskText;
        }

        private static (int value, string diskText) DiskValue(long diTotalFreeSpace, long totalSize)
        {
            int pct = (int)(MAX_PROGRESS_BAR * diTotalFreeSpace / totalSize);
            int diskValue = MAX_PROGRESS_BAR - pct;
            string diskText = diTotalFreeSpace.GBMB(1) + " free";
            return (diskValue, diskText);
        }

        private ActionCopyMoveRename? GetActiveCmAction()
        {
            foreach (Action action in mToDo.Actions)
            {
                if (!action.Outcome.Done && action.PercentDone > 0 && action is ActionCopyMoveRename cmAction)
                {
                    return cmAction;
                }
            }

            return null;
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
            mDoOnClose();
        }

        private void cbPause_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPause.Checked)
            {
                mDoc.Pause();
                cbPause.Text = "Resume";
            }
            else
            {
                mDoc.Resume();
                cbPause.Text = "Pause";
            }

            bool en = !cbPause.Checked;
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

        private void CopyMoveProgress_SizeChanged(object sender, EventArgs e)
        {
            if (sender is not Form {WindowState: FormWindowState.Minimized} childWindow)
            {
                return;
            }

            Form parentWindow = childWindow.Owner;
            if (parentWindow != null)
            {
                parentWindow.WindowState = FormWindowState.Minimized;
            }
        }
    }
}
