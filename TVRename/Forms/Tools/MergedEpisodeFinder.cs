using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TVRename.Forms
{
    public partial class MergedEpisodeFinder : Form
    {
        private List<PossibleMergedEpisode> dupEps;
        private readonly TVDoc mDoc;
        private readonly UI mainUi;

        public MergedEpisodeFinder([NotNull] TVDoc doc, UI main)
        {
            InitializeComponent();
            dupEps = new List<PossibleMergedEpisode>();
            mDoc = doc;
            mainUi = main;
            Scan();
        }

        // ReSharper disable once InconsistentNaming
        private void UpdateUI()
        {
            ClearGrid();
            PopulateGrid();
        }

        private void PopulateGrid()
        {
            lvMergedEpisodes.BeginUpdate();
            foreach (PossibleMergedEpisode item in dupEps)
            {
                ListViewItem possible = item.PresentationView;

                bool passesAirDateTest = chkAirDateTest.Checked == false || item.AirDatesMatch;
                bool passesNameTest = chkNameTest.Checked == false || item.SimilarNames;
                bool passesMissingTest = chkMIssingTest.Checked == false || item.OneFound;
                bool passesSizeTest = chkFilesizeTest.Checked == false || item.LargeFileSize;

                if (passesSizeTest && passesAirDateTest && passesNameTest && passesMissingTest)
                {
                    lvMergedEpisodes.Items.Add(possible);
                }
            }

            lvMergedEpisodes.EndUpdate();
        }

        private void ClearGrid()
        {
            lvMergedEpisodes.BeginUpdate();
            lvMergedEpisodes.Items.Clear();
            lvMergedEpisodes.EndUpdate();
        }

        private void chkAirDateTest_CheckedChanged(object sender, EventArgs e)
        {
            chkAirDateTest.Checked = true;
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

        private void chkMissingTest_CheckedChanged(object sender, EventArgs e)
        {
            switch (chkMIssingTest.Checked)
            {
                case false:
                    chkFilesizeTest.Checked = false;
                    break;
                case true:
                    chkNameTest.Checked = true;
                    break;
            }

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

        private void lvDuplicates_MouseClick(object sender, [NotNull] MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            if (lvMergedEpisodes.SelectedItems.Count == 0)
            {
                return;
            }

            PossibleMergedEpisode? mlastSelected = (PossibleMergedEpisode)lvMergedEpisodes.SelectedItems[0].Tag;
            ListViewItem? mLastClicked = lvMergedEpisodes.SelectedItems[0];
            ShowConfiguration? si = mlastSelected?.ShowConfiguration;

            if (si == null)
            {
                return;
            }

            Point pt = lvMergedEpisodes.PointToScreen(new Point(e.X, e.Y));

            possibleMergedEpisodeRightClickMenu.Items.Clear();

            AddRcMenuItem("Episode Guide", (_, _) => GotoEpGuide(si, mlastSelected));
            AddRcMenuItem("Force Refresh", (_, _) => mainUi.ForceRefresh(si, false));
            AddRcMenuItem("Edit TV Show", (_, _) => mainUi.EditShow(si));

            AddRcMenuItem("Edit " + ProcessedSeason.UIFullSeasonWord(mlastSelected.SeasonNumber),
                (_, _) => mainUi.EditSeason(si, mlastSelected.SeasonNumber));

            possibleMergedEpisodeRightClickMenu.Items.Add(new ToolStripSeparator());
            AddRcMenuItem("Add Rule", (_, _) => AddRule(mlastSelected, si, mLastClicked));

            possibleMergedEpisodeRightClickMenu.Show(pt);
        }

        private void AddRcMenuItem([NotNull] string label, EventHandler command)
        {
            ToolStripMenuItem tsi = new(label.Replace("&", "&&"));
            tsi.Click += command;
            possibleMergedEpisodeRightClickMenu.Items.Add(tsi);
        }

        private void AddRule([NotNull] PossibleMergedEpisode selected, [NotNull] ShowConfiguration si, [NotNull] ListViewItem lastClicked)
        {
            ShowRule sr = selected.GenerateRule();

            si.AddSeasonRule(selected.SeasonNumber, sr);

            lvMergedEpisodes.Items.Remove(lastClicked);
            dupEps.Remove(selected);
        }

        private void GotoEpGuide(ShowConfiguration? si, PossibleMergedEpisode? mLastSelected)
        {
            if (mLastSelected != null)
            {
                mainUi.GotoEpguideFor(mLastSelected.Episode, true);
            }
            else
            {
                if (si != null)
                {
                    mainUi.GotoEpguideFor(si, true);
                }
            }

            Close();
        }

        private void PossibleMergedEpisodeRightClickMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            possibleMergedEpisodeRightClickMenu.Close();
        }

        private void BwScan_DoWork([NotNull] object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.CurrentThread.Name ??= "MergedEpisode Scan Thread"; // Can only set it once
            dupEps = MergedEpisodeFinderController.FindDoubleEps(mDoc, (BackgroundWorker)sender);
        }

        private void BwScan_ProgressChanged(object sender, [NotNull] ProgressChangedEventArgs e)
        {
            pbProgress.Value = e.ProgressPercentage.Between(0, 100);
            lblStatus.Text = e.UserState.ToString();
        }

        private void BwScan_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnRefresh.Visible = true;
            pbProgress.Visible = false;
            lblStatus.Visible = false;
            if (lvMergedEpisodes.IsDisposed)
            {
                return;
            }
            ClearGrid();
            PopulateGrid();
        }

        private void BtnRefresh_Click_1(object sender, EventArgs e)
        {
            Scan();
        }

        private void Scan()
        {
            btnRefresh.Visible = false;
            pbProgress.Visible = true;
            lblStatus.Visible = true;
            bwScan.RunWorkerAsync();
        }
    }
}
