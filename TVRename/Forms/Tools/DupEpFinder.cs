using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace TVRename.Forms
{
    public partial class DupEpFinder : Form
    {
        private List<PossibleDuplicateEpisode> dupEps;
        private readonly TVDoc mDoc;
        private readonly UI mainUi;

        public DupEpFinder([NotNull] TVDoc doc, UI main)
        {
            InitializeComponent();
            dupEps = Beta.FindDoubleEps(doc);
            mDoc = doc;
            mainUi = main;
            UpdateUI();
        }

        // ReSharper disable once InconsistentNaming
        private void UpdateUI()
        {
            lvDuplicates.BeginUpdate();
            lvDuplicates.Items.Clear();

            foreach (PossibleDuplicateEpisode item in dupEps)
            {
                ListViewItem possible = item.PresentationView;

                bool passesAirDateTest = chkAirDateTest.Checked == false || item.AirDatesMatch;
                bool passesNameTest = chkNameTest.Checked == false || item.SimilarNames;
                bool passesMissingTest = chkMIssingTest.Checked == false || item.OneFound;
                bool passesSizeTest = chkFilesizeTest.Checked == false || item.LargeFileSize;

                if (passesSizeTest && passesAirDateTest && passesNameTest && passesMissingTest)
                {
                    lvDuplicates.Items.Add(possible);
                }
            }

            lvDuplicates.EndUpdate();
        }

        private void btnRescan_Click(object sender, EventArgs e)
        {
            dupEps = Beta.FindDoubleEps(mDoc);
            UpdateUI();
        }

        private void chkAirDateTest_CheckedChanged(object sender, EventArgs e)
        {
            chkAirDateTest.Checked = true;
            UpdateUI();
        }

        private void UpdateCheckboxes()
        {
            if (chkFilesizeTest.Checked)
            {
                chkMIssingTest.Checked = true;
            }

            if (chkMIssingTest.Checked)
            {
                chkNameTest.Checked = true;
            }

            if (chkNameTest.Checked == false)
            {
                chkMIssingTest.Checked = false;
            }

            if (chkMIssingTest.Checked == false)
            {
                chkFilesizeTest.Checked = false;
            }

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
            {
                chkFilesizeTest.Checked = false;
            }

            if (chkMIssingTest.Checked)
            {
                chkNameTest.Checked = true;
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

            if (lvDuplicates.SelectedItems.Count == 0)
            {
                return;
            }

            PossibleDuplicateEpisode? mlastSelected = (PossibleDuplicateEpisode) lvDuplicates.SelectedItems[0].Tag;
            ListViewItem? mlastClicked = lvDuplicates.SelectedItems[0];
            ShowItem? si = mlastSelected?.ShowItem;

            Point pt = lvDuplicates.PointToScreen(new Point(e.X, e.Y));

            duplicateRightClickMenu.Items.Clear();

            if (si != null)
            {
                AddRcMenuItem("Episode Guide", (o, args) => GotoEpGuide(si,mlastSelected));
                AddRcMenuItem("Force Refresh", (o, args) => mainUi.ForceRefresh(new List<ShowItem> {si}, false));
                AddRcMenuItem("Edit Show", (o, args) => mainUi.EditShow(si));

                AddRcMenuItem("Edit " + ProcessedSeason.UIFullSeasonWord(mlastSelected.SeasonNumber),
                        (o, args) => mainUi.EditSeason(si, mlastSelected.SeasonNumber));

                duplicateRightClickMenu.Items.Add(new ToolStripSeparator());
                AddRcMenuItem("Add Rule", (o, args) => AddRule(mlastSelected, si, mlastClicked));
            }
            duplicateRightClickMenu.Show(pt);

        }

        private void AddRcMenuItem(string label, EventHandler command)
        {
            ToolStripMenuItem tsi = new ToolStripMenuItem(label);
            tsi.Click += command;
            duplicateRightClickMenu.Items.Add(tsi);
        }

        private void duplicateRightClickMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            duplicateRightClickMenu.Close();
        }

        private void AddRule(PossibleDuplicateEpisode selected,ShowItem si, ListViewItem lastClicked)
        {
            ShowRule sr = selected.GenerateRule();

            si.AddSeasonRule(selected.SeasonNumber, sr);

            lvDuplicates.Items.Remove(lastClicked);
            dupEps.Remove(selected);
        }

        private void GotoEpGuide(ShowItem? si, PossibleDuplicateEpisode? mlastSelected)
        {
            if (mlastSelected != null)
            {
                mainUi.GotoEpguideFor(mlastSelected.Episode, true);
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
    }
}
