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
            dupEps = x;
            mDoc = doc;
            mainUI = main;
            updateUI();
        }

        private void updateUI()
        {
            // Save where the list is currently scrolled too
            //int currentTop = this.lvDuplicates.GetScrollVerticalPos();

            lvDuplicates.BeginUpdate();
            lvDuplicates.Items.Clear();

            foreach (PossibleDuplicateEpisode item in dupEps)
            {
                ListViewItem possible = item.PresentationView;

                bool passesAirDateTest = (chkAirDateTest.Checked == false || item.AirDatesMatch);
                bool passesNameTest = (chkNameTest.Checked == false || item.SimilarNames);
                bool passesMissingTest = (chkMIssingTest.Checked == false || item.OneFound);
                bool passesSizeTest = (chkFilesizeTest.Checked == false || item.LargeFileSize);

                if (passesSizeTest && passesAirDateTest && passesNameTest && passesMissingTest)
                    lvDuplicates.Items.Add(possible);
            }

            lvDuplicates.EndUpdate();

            // Restore the scrolled to position
            //this.lvDuplicates.SetScrollVerticalPos(currentTop);
        }

        private void btnRescan_Click(object sender, EventArgs e)
        {
            dupEps = mDoc.FindDoubleEps();
            updateUI();
        }

        private void chkAirDateTest_CheckedChanged(object sender, EventArgs e)
        {
            chkAirDateTest.Checked = true;
            updateUI();
        }

        private void UpdateCheckboxes()
        {
            if (chkFilesizeTest.Checked) chkMIssingTest.Checked = true;
            if (chkMIssingTest.Checked) chkNameTest.Checked = true;
            if (chkNameTest.Checked == false) chkMIssingTest.Checked = false;
            if (chkMIssingTest.Checked == false) chkFilesizeTest.Checked = false;

            updateUI();
        }

        private void chkNameTest_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkNameTest.Checked)
            {
                chkFilesizeTest.Checked = false;
                chkMIssingTest.Checked = false;
            }

            updateUI();
        }

        private void chkMIssingTest_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkMIssingTest.Checked)
                chkFilesizeTest.Checked = false;
            if (chkMIssingTest.Checked)
                chkNameTest.Checked = true;

            updateUI();
        }

        private void chkFilesizeTest_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFilesizeTest.Checked)
            {
                chkNameTest.Checked = true;
                chkMIssingTest.Checked = true;
                }

            updateUI();
        }

        private void lvDuplicates_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            if (lvDuplicates.SelectedItems.Count == 0)
                return;

            mlastSelected = (PossibleDuplicateEpisode) lvDuplicates.SelectedItems[0].Tag;
            mlastClicked = lvDuplicates.SelectedItems[0];
            Point pt = lvDuplicates.PointToScreen(new Point(e.X, e.Y));

            duplicateRightClickMenu.Items.Clear();

            //kEpisodeGuideForShow = 1,
            ToolStripMenuItem tsi;
            tsi = new ToolStripMenuItem("Episode Guide") {Tag = (int) RightClickCommands.kEpisodeGuideForShow};
            duplicateRightClickMenu.Items.Add(tsi);


            //kForceRefreshSeries,
            tsi = new ToolStripMenuItem("Force Refresh") {Tag = (int) RightClickCommands.kForceRefreshSeries};
            duplicateRightClickMenu.Items.Add(tsi);

            //kEditShow,
            tsi = new ToolStripMenuItem("Edit Show") {Tag = (int) RightClickCommands.kEditShow};
            duplicateRightClickMenu.Items.Add(tsi);

            //kEditSeason,
            tsi = new ToolStripMenuItem("Edit " + (mlastSelected.SeasonNumber == 0
                                            ? TVSettings.Instance.SpecialsFolderName
                                            : TVSettings.Instance.defaultSeasonWord + " " + mlastSelected.SeasonNumber));
            tsi.Tag = (int) RightClickCommands.kEditSeason;
            duplicateRightClickMenu.Items.Add(tsi);

            duplicateRightClickMenu.Items.Add(new ToolStripSeparator());

            //kAddRule,
            tsi = new ToolStripMenuItem("Add Rule") {Tag = (int) RightClickCommands.kAddRule};
            duplicateRightClickMenu.Items.Add(tsi);

            duplicateRightClickMenu.Show(pt);
        }


        public void duplicateRightClickMenu_ItemClicked(object sender,
            ToolStripItemClickedEventArgs e)
        {
            duplicateRightClickMenu.Close();

            if (e.ClickedItem.Tag != null)
            {

                RightClickCommands n = (RightClickCommands) e.ClickedItem.Tag;

                ShowItem si = mlastSelected?.ShowItem;

                switch (n)
                {
                    case RightClickCommands.kEpisodeGuideForShow: // epguide
                        if (mlastSelected != null)
                            mainUI.GotoEpguideFor(mlastSelected.Episode, true);
                        else
                        {
                            if (si != null)
                                mainUI.GotoEpguideFor(si, true);
                        }
                        Close();
                        break;


                    case RightClickCommands.kForceRefreshSeries:
                        if (si != null)
                            mainUI.ForceRefresh(new List<ShowItem> {mlastSelected.ShowItem});
                        Close();
                        break;
                    case RightClickCommands.kEditShow:
                        if (si != null)
                            mainUI.EditShow(si);
                        break;

                    case RightClickCommands.kEditSeason:
                        if (si != null)
                            mainUI.EditSeason(si, mlastSelected.SeasonNumber);
                        break;
                    case RightClickCommands.kAddRule:
                        ShowRule sr = new ShowRule();
                        sr.DoWhatNow = RuleAction.kMerge;
                        sr.First = mlastSelected.episodeOne.AppropriateEpNum;
                        sr.Second  = mlastSelected.episodeTwo.AppropriateEpNum;

                        si?.AddSeasonRule(mlastSelected.SeasonNumber,sr);

                        lvDuplicates.Items.Remove(mlastClicked);
                        dupEps.Remove(mlastSelected);
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

            mlastSelected = null;
        }

    }
}

