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
        private ScanListItemList[] mToDo;

        public CopyMoveProgress(TVDoc doc, ScanListItemList[] todo, TVRenameStats stats)
        {
            this.mDoc = doc;
            this.mToDo = todo;
            this.mStats = stats;
            this.InitializeComponent();
            this.copyTimer.Start();

            // set up list view items, one for each non-empty queue
            foreach (ScanListItemList slil in this.mToDo)
            {
                if (slil.Count == 0)
                    continue;

                ListViewItem lvi = new ListViewItem("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                this.lvProgress.Items.Add(lvi);
            }
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
        private void UpdateOldStyle()
        {
            double workDone = 0;
            double totalWork = 0;
            double workingPercent = 0;
            double workingTotal = 0;

            bool allDone = true;

            string filename = "";

            foreach (ScanListItemList slil in this.mToDo)
            {
                foreach (ScanListItem sli in slil)
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

                        filename = action.ProgressText;
                    }
                }
            }

            this.txtFilename.Text = filename;

            if (totalWork != 0.0)
                workDone = workDone * 100.0 / totalWork;
            if (workingTotal != 0.0)
                workingPercent = workingPercent * 100.0 / workingTotal;

            this.SetPercentages(workingPercent, workDone);
        }

        private bool UpdateNewStyle() // return true if all tasks are done
        {
            // update each listview item, for non-empty queues
            int itemNum = 0;
            bool allDone = true;

            lvProgress.BeginUpdate();
            foreach (ScanListItemList slil in this.mToDo)
            {
                if (slil.Count == 0)
                    continue;

                ListViewItem lvi = lvProgress.Items[itemNum++];
                double workDone = 0;
                double totalWork = 0;
                double workingPercent = 0;
                double workingTotal = 0;
                string filename = "";
                string actname = "";
                foreach (ScanListItem sli in slil)
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

                        filename = action.ProgressText;
                        actname = action.Name;
                    }
                }

                if (totalWork != 0.0)
                    workDone = workDone * 100.0 / totalWork;
                if (workingTotal != 0.0)
                    workingPercent = workingPercent * 100.0 / workingTotal;


                lvi.Text = actname;
                lvi.SubItems[1].Text = filename;
                lvi.SubItems[2].Text = ((int)(Math.Round(workingPercent))).ToString();
                lvi.SubItems[3].Text = ((int)(Math.Round(workDone))).ToString();
            }
            lvProgress.EndUpdate();

            return allDone;
        }

        private void copyTimer_Tick(object sender, System.EventArgs e)
        {
            this.copyTimer.Stop();

            this.UpdateOldStyle();
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
            mDoc.ActionPause = cbPause.Checked;

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