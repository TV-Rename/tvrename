using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TVRename.ItemsAndActions;

namespace TVRename.Forms
{
    public partial class DupEpFinder : Form
    {
        private List<PossibleDuplicateEpisode> dupEps;
        private readonly TVDoc mDoc;
        private PossibleDuplicateEpisode mlastSelected;
        private readonly UI mainUI;
        private ListViewItem mlastClicked;

        public enum RightClickCommands
        {
            none = 0,
            kEpisodeGuideForShow = 1,
            kForceRefreshSeries,
            kEditShow,
            kEditSeason,
            kAddRule,
            kOpenFolderBase = 2000
        }

        public DupEpFinder(List<PossibleDuplicateEpisode> x, TVDoc doc, UI main)
        {
            InitializeComponent();
            dupEps = x;
            mDoc = doc;
            mainUI = main;
            UpdateUI();
        }

        // ReSharper disable once InconsistentNaming
        private void UpdateUI()
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
            UpdateUI();
        }

        private void chkAirDateTest_CheckedChanged(object sender, EventArgs e)
        {
            chkAirDateTest.Checked = true;
            UpdateUI();
        }

        private void UpdateCheckboxes()
        {
            if (chkFilesizeTest.Checked) chkMIssingTest.Checked = true;
            if (chkMIssingTest.Checked) chkNameTest.Checked = true;
            if (chkNameTest.Checked == false) chkMIssingTest.Checked = false;
            if (chkMIssingTest.Checked == false) chkFilesizeTest.Checked = false;

            UpdateUI();
        }

        private void chkNameTest_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkNameTest.Checked)
            {
                chkFilesizeTest.Checked = false;
                chkMIssingTest.Checked = false;
            }

            UpdateUI();
        }

        private void chkMIssingTest_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkMIssingTest.Checked)
                chkFilesizeTest.Checked = false;
            if (chkMIssingTest.Checked)
                chkNameTest.Checked = true;

            UpdateUI();
        }

        private void chkFilesizeTest_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFilesizeTest.Checked)
            {
                chkNameTest.Checked = true;
                chkMIssingTest.Checked = true;
                }

            UpdateUI();
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
                                            : new ActionBuildRightSeasonWord(mlastSelected.SeasonNumber).SeasonName));

            tsi.Tag = (int) RightClickCommands.kEditSeason;
            duplicateRightClickMenu.Items.Add(tsi);

            duplicateRightClickMenu.Items.Add(new ToolStripSeparator());

            //kAddRule,
            tsi = new ToolStripMenuItem("Add Rule") {Tag = (int) RightClickCommands.kAddRule};
            duplicateRightClickMenu.Items.Add(tsi);

            duplicateRightClickMenu.Show(pt);
        }

        private void duplicateRightClickMenu_ItemClicked(object sender,
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
                            mainUI.ForceRefresh(new List<ShowItem> {si});
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
                        if (mlastSelected != null)
                        {
                            ShowRule sr = mlastSelected.GenerateRule();

                            si?.AddSeasonRule(mlastSelected.SeasonNumber, sr);

                            lvDuplicates.Items.Remove(mlastClicked);
                            dupEps.Remove(mlastSelected);
                        }
                        break;
                    default:
                    {
                            System.Diagnostics.Debug.Fail("Unknown right-click action " + n);
                            break;
                    }
                }
            }
            mlastSelected = null;
        }
    }
}

