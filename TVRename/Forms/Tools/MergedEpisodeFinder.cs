using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TVRename.Forms;

public partial class MergedEpisodeFinder : Form
{
    private List<PossibleMergedEpisode> dupEps;
    private readonly TVDoc mDoc;
    private readonly UI mainUi;

    public MergedEpisodeFinder(TVDoc doc, UI main)
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

    private void lvDuplicates_MouseClick(object sender, MouseEventArgs e)
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

        rightClickMenu.Items.Clear();

        rightClickMenu.Add("Episode Guide", (_, _) => GotoEpGuide(si, mlastSelected));
        rightClickMenu.Add("Force Refresh", (_, _) => mainUi.ForceRefresh(si, false));
        rightClickMenu.Add("Edit TV Show", (_, _) => mainUi.EditShow(si));

        rightClickMenu.Add("Edit " + ProcessedSeason.UIFullSeasonWord(mlastSelected!.SeasonNumber),
            (_, _) => mainUi.EditSeason(si, mlastSelected.SeasonNumber));

        rightClickMenu.AddSeparator();
        rightClickMenu.Add("Add Rule", (_, _) => AddRule(mlastSelected, si, mLastClicked));

        rightClickMenu.Show(pt);
    }

    private void AddRule(PossibleMergedEpisode selected, ShowConfiguration si, ListViewItem lastClicked)
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

    private void rightClickMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
        rightClickMenu.Close();
    }

    private void BwScan_DoWork(object sender, DoWorkEventArgs e)
    {
        System.Threading.Thread.CurrentThread.Name ??= "MergedEpisode Scan Thread"; // Can only set it once
        dupEps = MergedEpisodeFinderController.FindDoubleEps(mDoc, (BackgroundWorker)sender);
    }

    private void BwScan_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        pbProgress.Value = e.ProgressPercentage.Between(0, 100);
        lblStatus.Text = e.UserState?.ToString().ToUiVersion();
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
