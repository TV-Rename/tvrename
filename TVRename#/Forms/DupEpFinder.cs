using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TVRename.Forms
{
    public partial class frmDupEpFinder : Form
    {
        private List<PossibleDuplicateEpisode> dupEps;
        private TVDoc mDoc;
        private PossibleDuplicateEpisode mlastSelected;
        private UI mainUI;
        private ListViewItem mlastClicked;

        public enum RightClickCommands
        {
            None = 0,
            kEpisodeGuideForShow = 1,
            kForceRefreshSeries,
            kEditShow,
            kEditSeason,
            kAddRule,
            kOpenFolderBase = 2000
        }

        public frmDupEpFinder(List<PossibleDuplicateEpisode> x, TVDoc doc, UI main)
        {
            InitializeComponent();
            this.dupEps = x;
            this.mDoc = doc;
            this.mainUI = main;
            updateUI();
        }

        private void updateUI()
        {
            // Save where the list is currently scrolled too
            //int currentTop = this.lvDuplicates.GetScrollVerticalPos();

            this.lvDuplicates.BeginUpdate();
            this.lvDuplicates.Items.Clear();

            foreach (PossibleDuplicateEpisode item in this.dupEps)
            {
                ListViewItem possible = item.PresentationView;

                bool passesAirDateTest = (this.chkAirDateTest.Checked == false || item.AirDatesMatch);
                bool passesNameTest = (this.chkNameTest.Checked == false || item.SimilarNames);
                bool passesMissingTest = (this.chkMIssingTest.Checked == false || item.OneFound);
                bool passesSizeTest = (this.chkFilesizeTest.Checked == false || item.LargeFileSize);

                if (passesSizeTest && passesAirDateTest && passesNameTest && passesMissingTest)
                    this.lvDuplicates.Items.Add(possible);
            }

            this.lvDuplicates.EndUpdate();

            // Restore the scrolled to position
            //this.lvDuplicates.SetScrollVerticalPos(currentTop);
        }

        private void btnRescan_Click(object sender, EventArgs e)
        {
            this.dupEps = this.mDoc.findDoubleEps();
            updateUI();
        }

        private void chkAirDateTest_CheckedChanged(object sender, EventArgs e)
        {
            this.chkAirDateTest.Checked = true;
            updateUI();
        }

        private void UpdateCheckboxes()
        {
            if (this.chkFilesizeTest.Checked) this.chkMIssingTest.Checked = true;
            if (this.chkMIssingTest.Checked) this.chkNameTest.Checked = true;
            if (this.chkNameTest.Checked == false) this.chkMIssingTest.Checked = false;
            if (this.chkMIssingTest.Checked == false) this.chkFilesizeTest.Checked = false;

            updateUI();
        }

        private void chkNameTest_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.chkNameTest.Checked)
            {
                this.chkFilesizeTest.Checked = false;
                this.chkMIssingTest.Checked = false;
            }

            updateUI();
        }

        private void chkMIssingTest_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.chkMIssingTest.Checked)
                this.chkFilesizeTest.Checked = false;
            if (this.chkMIssingTest.Checked)
                this.chkNameTest.Checked = true;

            updateUI();
        }

        private void chkFilesizeTest_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkFilesizeTest.Checked)
            {
                this.chkNameTest.Checked = true;
                this.chkMIssingTest.Checked = true;
                }

            updateUI();
        }

        private void lvDuplicates_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            if (this.lvDuplicates.SelectedItems.Count == 0)
                return;

            this.mlastSelected = (PossibleDuplicateEpisode) this.lvDuplicates.SelectedItems[0].Tag;
            this.mlastClicked = this.lvDuplicates.SelectedItems[0];
            Point pt = this.lvDuplicates.PointToScreen(new Point(e.X, e.Y));

            this.duplicateRightClickMenu.Items.Clear();

            //kEpisodeGuideForShow = 1,
            ToolStripMenuItem tsi;
            tsi = new ToolStripMenuItem("Episode Guide") {Tag = (int) RightClickCommands.kEpisodeGuideForShow};
            this.duplicateRightClickMenu.Items.Add(tsi);


            //kForceRefreshSeries,
            tsi = new ToolStripMenuItem("Force Refresh") {Tag = (int) RightClickCommands.kForceRefreshSeries};
            this.duplicateRightClickMenu.Items.Add(tsi);

            //kEditShow,
            tsi = new ToolStripMenuItem("Edit Show") {Tag = (int) RightClickCommands.kEditShow};
            this.duplicateRightClickMenu.Items.Add(tsi);

            //kEditSeason,
            tsi = new ToolStripMenuItem("Edit " + (this.mlastSelected.SeasonNumber == 0
                                            ? TVSettings.Instance.SpecialsFolderName
                                            : TVSettings.Instance.defaultSeasonWord + " " + this.mlastSelected.SeasonNumber));
            tsi.Tag = (int) RightClickCommands.kEditSeason;
            this.duplicateRightClickMenu.Items.Add(tsi);

            this.duplicateRightClickMenu.Items.Add(new ToolStripSeparator());

            //kAddRule,
            tsi = new ToolStripMenuItem("Add Rule") {Tag = (int) RightClickCommands.kAddRule};
            this.duplicateRightClickMenu.Items.Add(tsi);

            this.duplicateRightClickMenu.Show(pt);
        }


        public void duplicateRightClickMenu_ItemClicked(object sender,
            ToolStripItemClickedEventArgs e)
        {
            this.duplicateRightClickMenu.Close();

            if (e.ClickedItem.Tag != null)
            {

                RightClickCommands n = (RightClickCommands) e.ClickedItem.Tag;

                ShowItem si = this.mlastSelected?.ShowItem;

                switch (n)
                {
                    case RightClickCommands.kEpisodeGuideForShow: // epguide
                        if (this.mlastSelected != null)
                            this.mainUI.GotoEpguideFor(this.mlastSelected.Episode, true);
                        else
                        {
                            if (si != null)
                                this.mainUI.GotoEpguideFor(si, true);
                        }
                        Close();
                        break;


                    case RightClickCommands.kForceRefreshSeries:
                        if (si != null)
                            this.mainUI.ForceRefresh(new List<ShowItem> {this.mlastSelected.ShowItem});
                        Close();
                        break;
                    case RightClickCommands.kEditShow:
                        if (si != null)
                            this.mainUI.EditShow(si);
                        break;

                    case RightClickCommands.kEditSeason:
                        if (si != null)
                            this.mainUI.EditSeason(si, this.mlastSelected.SeasonNumber);
                        break;
                    case RightClickCommands.kAddRule:
                        ShowRule sr = new ShowRule();
                        sr.DoWhatNow = RuleAction.kMerge;
                        sr.First = this.mlastSelected.episodeOne.AppropriateEpNum;
                        sr.Second  = this.mlastSelected.episodeTwo.AppropriateEpNum;
                        if (!si.SeasonRules.ContainsKey(this.mlastSelected.SeasonNumber)) si.SeasonRules[this.mlastSelected.SeasonNumber] = new List<ShowRule>();

                        si.SeasonRules[this.mlastSelected.SeasonNumber].Add(sr);
                        this.lvDuplicates.Items.Remove(this.mlastClicked);
                        this.dupEps.Remove(this.mlastSelected);
                        break;
                    default:
                    {
/*                        if ((n >= RightClickCommands.kWatchBase) && (n < RightClickCommands.kOpenFolderBase))
                        {
                            int wn = n - RightClickCommands.kWatchBase;
                            if ((this.mLastFL != null) && (wn >= 0) && (wn < this.mLastFL.Count))
                                Helpers.SysOpen(this.mLastFL[wn].FullName);
                        }
                        else if (n >= RightClickCommands.kOpenFolderBase)
                        {
                            int fnum = n - RightClickCommands.kOpenFolderBase;

                            if (fnum < this.mFoldersToOpen.Count)
                            {
                                string folder = this.mFoldersToOpen[fnum];

                                if (Directory.Exists(folder))
                                    Helpers.SysOpen(folder);
                            }

                            return;
                        }
                        else*/
                            System.Diagnostics.Debug.Fail("Unknown right-click action " + n);

                            break;


                }
                }
            }

            this.mlastSelected = null;
        }

    }
}

