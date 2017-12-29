// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using DriveInfo = System.IO.DriveInfo;

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

        private readonly TVDoc _mDoc;
        private readonly ActionQueue[] _mToDo;

        public CopyMoveProgress(TVDoc doc, ActionQueue[] todo)
        {
            _mDoc = doc;
            _mToDo = todo;
            InitializeComponent();
            copyTimer.Start();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        ~CopyMoveProgress()
        {
            copyTimer.Stop();
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

        private bool UpdateNewStyle() // return true if all tasks are done
        {
            // update each listview item, for non-empty queues
            bool allDone = true;

            lvProgress.BeginUpdate();
            int top = lvProgress.TopItem != null ? lvProgress.TopItem.Index : 0;
            ActionCopyMoveRename activeCmAction = null;
            long workDone = 0;
            long totalWork = 0;
            lvProgress.Items.Clear();
            foreach (ActionQueue aq in _mToDo)
            {
                if (aq.Actions.Count == 0)
                    continue;

                foreach (IAction action in aq.Actions)
                {
                    if (!action.Done)
                        allDone = false;

                    long size = action.SizeOfWork;
                    workDone += (long) (size * action.PercentDone / 100);
                    totalWork += action.SizeOfWork;

                    if (!action.Done)
                    {
                        if ((action is ActionCopyMoveRename) && (action.PercentDone > 0))
                            activeCmAction = action as ActionCopyMoveRename;

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

            int diskValue = 0;
            string diskText = "--- GB free";
            string fileText = "";

            if (activeCmAction != null)
            {
                string folder = activeCmAction.TargetFolder;
                DirectoryInfo toRoot = (!string.IsNullOrEmpty(folder) && !folder.StartsWith("\\\\")) ? new DirectoryInfo(folder).Root : null;

                if (toRoot != null)
                {
                    DriveInfo di;
                    try
                    {
                        // try to get root of drive
                        di = new DriveInfo(toRoot.ToString());
                    }
                    catch (ArgumentException)
                    {
                        di = null;
                    }

                    if (di != null)
                    {
                        int pct = (int) ((1000 * di.TotalFreeSpace) / di.TotalSize);
                        diskValue = 1000 - pct;
                        diskText = ((int) (di.TotalFreeSpace / 1024.0 / 1024.0 / 1024.0 + 0.5)) + " GB free";
                    }

                    fileText = activeCmAction.ProgressText;
                }

                txtFilename.Text = fileText;
                pbDiskSpace.Value = diskValue;
                txtDiskSpace.Text = diskText;

                SetPercentages(activeCmAction.PercentDone, totalWork == 0 ? 0.0 : (workDone * 100.0 / totalWork));
            }

            return allDone;
        }

        private void copyTimer_Tick(object sender, EventArgs e)
        {
            copyTimer.Stop();

            //this.UpdateOldStyle();
            bool allDone = UpdateNewStyle();

            if (allDone)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
                copyTimer.Start();
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void cbPause_CheckedChanged(object sender, EventArgs e)
        {
            _mDoc.ActionPause = cbPause.Checked;

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
