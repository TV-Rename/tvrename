//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;
using DaveChambers.FolderBrowserDialogEx;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TVRename.Forms;

namespace TVRename;

/// <summary>
/// Summary for FolderMonitor
///
/// WARNING: If you change the name of this class, you will need to change the
///          'Resource File Name' property for the managed resource compiler tool
///          associated with all .resx files this class depends on.  Otherwise,
///          the designers will not be able to interact properly with localized
///          resources associated with this form.
/// </summary>
public partial class BulkAddMovie : Form
{
    private readonly TVDoc mDoc;
    private readonly BulkAddMovieManager engine;
    private readonly UI mainUi;
    CancellationTokenSource cts = new();


    //Thread safe counters to work out the progress
    //For auto id
    private static readonly ThreadSafeCounter VolatileCounter = new();

    public class NewMovieProgressReport
    {
        public int NumberComplete { get; set; }
        public PossibleNewMovie? LatestItemProcessed { get; set; }
    }

    public BulkAddMovie(TVDoc doc, BulkAddMovieManager bam, UI mainUi)
    {
        mDoc = doc;
        engine = bam;
        this.mainUi = mainUi;
        InitializeComponent();
        FillFolderStringLists();
        tbResults.Parent = null;
        olvFMNewShows.ShowGroups = false;
        ShowHideUpdateControls(false);
    }

    private void bnClose_Click(object sender, System.EventArgs e)
    {
        cts.Cancel();

        if (!CanClose())
        {
            if (DialogResult.OK != MessageBox.Show("Close without adding identified shows to \"My Movies\"?", "Bulk Add Movies", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
            {
                return;
            }
        }

        Close();
    }

    private bool CanClose()
    {
        return engine.AddItems.All(fme => !fme.CodeKnown);
    }

    private void FillFolderStringLists()
    {
        TVSettings.Instance.MovieLibraryFolders.Sort();
        TVSettings.Instance.IgnoreFolders.Sort();

        lstFMMonitorFolders.BeginUpdate();
        lstFMMonitorFolders.Items.Clear();

        foreach (string folder in TVSettings.Instance.MovieLibraryFolders)
        {
            lstFMMonitorFolders.Items.Add(folder);
        }

        lstFMMonitorFolders.EndUpdate();

        lstFMIgnoreFolders.BeginUpdate();
        lstFMIgnoreFolders.Items.Clear();

        foreach (string folder in TVSettings.Instance.IgnoreFolders)
        {
            lstFMIgnoreFolders.Items.Add(folder);
        }

        lstFMIgnoreFolders.EndUpdate();
    }

    private void bnRemoveMonFolder_Click(object sender, System.EventArgs e)
    {
        DeleteSelectedFolder(lstFMMonitorFolders, TVSettings.Instance.MovieLibraryFolders);
    }

    private void DeleteSelectedFolder(ListBox lb, SafeList<string> folders)
    {
        for (int i = lb.SelectedIndices.Count - 1; i >= 0; i--)
        {
            int n = lb.SelectedIndices[i];
            folders.RemoveAt(n);
        }

        mDoc.SetDirty();
        FillFolderStringLists();
    }

    private void bnRemoveIgFolder_Click(object sender, System.EventArgs e)
    {
        DeleteSelectedFolder(lstFMIgnoreFolders, TVSettings.Instance.IgnoreFolders);
    }

    private void bnAddMonFolder_Click(object sender, System.EventArgs e)
    {
        FolderBrowserDialogEx searchFolderBrowser = new()
        {
            SelectedPath = string.Empty,
            Title = "Add New Movie Base Folder...",
            ShowEditbox = true,
            StartPosition = FormStartPosition.CenterParent
        };

        if (lstFMMonitorFolders.SelectedIndex != -1)
        {
            int n = lstFMMonitorFolders.SelectedIndex;
            searchFolderBrowser.SelectedPath = TVSettings.Instance.MovieLibraryFolders[n];
        }

        if (UiHelpers.ShowDialogAndOk(searchFolderBrowser, this))
        {
            if (Directory.Exists(searchFolderBrowser.SelectedPath))
            {
                TVSettings.Instance.MovieLibraryFolders.Add(searchFolderBrowser.SelectedPath);
                mDoc.SetDirty();
                FillFolderStringLists();
            }
        }
    }
    private void bnAddIgFolder_Click(object sender, System.EventArgs e)
    {
        FolderBrowserDialogEx ignoreFolderBrowser = new()
        {
            SelectedPath = string.Empty,
            Title = "Add New Ignore Folder...",
            ShowEditbox = true,
            StartPosition = FormStartPosition.CenterParent
        };

        if (lstFMIgnoreFolders.SelectedIndex != -1)
        {
            ignoreFolderBrowser.SelectedPath = TVSettings.Instance.IgnoreFolders[lstFMIgnoreFolders.SelectedIndex];
        }

        if (UiHelpers.ShowDialogAndOk(ignoreFolderBrowser, this))
        {
            TVSettings.Instance.IgnoreFolders.Add(ignoreFolderBrowser.SelectedPath.ToLower());
            mDoc.SetDirty();
            FillFolderStringLists();
        }
    }
    private void bnOpenMonFolder_Click(object sender, System.EventArgs e)
    {
        OpenSelectedFolder();
    }

    private void OpenSelectedFolder()
    {
        if (lstFMMonitorFolders.SelectedIndex != -1)
        {
            TVSettings.Instance.MovieLibraryFolders[lstFMMonitorFolders.SelectedIndex].OpenFolder();
        }
    }

    private void bnOpenIgFolder_Click(object sender, System.EventArgs e)
    {
        if (lstFMIgnoreFolders.SelectedIndex != -1)
        {
            TVSettings.Instance.IgnoreFolders[lstFMIgnoreFolders.SelectedIndex].OpenFolder();
        }
    }

    private void lstFMMonitorFolders_DoubleClick(object sender, System.EventArgs e)
    {
        OpenSelectedFolder();
    }

    private async void bnCheck_Click(object sender, System.EventArgs e)
    {
        await DoCheckAsync();
    }

    private async Task DoCheckAsync()
    {
        tbResults.Parent = tabControl1;
        tabControl1.SelectedTab = tbResults;
        tabControl1.Update();

        ShowHideUpdateControls(true);

        cts = new();

        pbProgress.SetProgress(0);
        pbProgress.Maximum = 100;
        lblStatusLabel.Text = "Checking folders";

        var progressHandler = new Progress<ScanProgressReport>(scanReport =>
        {
            // This body executes safely on the main thread
            pbProgress.SetProgress(scanReport.ProgressPercentage);
            lblStatusLabel.Text = scanReport.UpdateText.ToUiVersion();
        });

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 4, // Limit concurrent tasks
            CancellationToken = cts.Token // Pass token to the loop mechanism
        };

        VolatileCounter.Reset();

        try
        {
            await engine.CheckFoldersAsync(options, VolatileCounter, progressHandler, true, true, cts.Token);
        }
        catch (OperationCanceledException)
        {
            //OK
        }

        olvFMNewShows.UpdateObjects(engine.AddItems);
        olvFMNewShows.Update();

        ShowHideUpdateControls(false);
    }

    private void ShowHideUpdateControls(bool show)
    {
        bnFullAuto.Enabled = !show;
        pbProgress.Visible = show;
        lblStatusLabel.Visible = show;
        btnStopScan.Enabled = show;
        btnStopScan.Visible = show;
    }

    private void lstFMMonitorFolders_DragOver(object _, DragEventArgs e)
    {
        e.Effect = GetDragEffect(e);
    }

    private static DragDropEffects GetDragEffect(DragEventArgs e) => e.Data?.GetDataPresent(DataFormats.FileDrop) ?? false ? DragDropEffects.Copy : DragDropEffects.None;

    private void lstFMIgnoreFolders_DragOver(object _, DragEventArgs e)
    {
        e.Effect = GetDragEffect(e);
    }

    private void lstFMMonitorFolders_DragDrop(object _, DragEventArgs e)
    {
        AddDraggedFiles(e, TVSettings.Instance.MovieLibraryFolders);
    }

    private async void lvFMNewShows_DragDrop(object _, DragEventArgs e)
    {
        if (e.Data is null)
        {
            return;
        }

        string[]? files = (string[]?)e.Data.GetData(DataFormats.FileDrop);
        if (files == null || files.Length == 0)
        {
            return;
        }

        foreach (string path in files)
        {
            try
            {
                DirectoryInfo di = new(path);
                if (di.Exists)
                {
                    await engine.CheckFolderForMoviesAsync(di, true, true, true);
                }
            }
            catch
            {
                // ignored
            }
        }
        PopulateShowList();
    }

    private void lstFMIgnoreFolders_DragDrop(object _, DragEventArgs e)
    {
        AddDraggedFiles(e, TVSettings.Instance.IgnoreFolders);
    }

    private void AddDraggedFiles(DragEventArgs e, SafeList<string> strings)
    {
        if (e.Data is not null)
        {
            string[]? files = (string[]?)e.Data.GetData(DataFormats.FileDrop);
            if (files != null)
                foreach (string path in files)
                {
                    try
                    {
                        DirectoryInfo di = new(path);
                        if (di.Exists)
                        {
                            strings.Add(path.ToLower());
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
        }

        mDoc.SetDirty();
        FillFolderStringLists();
    }

    private void lstFMMonitorFolders_KeyDown(object _, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete)
        {
            DeleteSelectedFolder(lstFMMonitorFolders, TVSettings.Instance.MovieLibraryFolders);
        }
    }

    private void lstFMIgnoreFolders_KeyDown(object _, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete)
        {
            DeleteSelectedFolder(lstFMIgnoreFolders, TVSettings.Instance.IgnoreFolders);
        }
    }

    private async void bnFullAuto_Click(object sender, System.EventArgs e)
    {
        if (engine.AddItems.Count == 0)
        {
            return;
        }

        ShowHideUpdateControls(true);

        cts = new();

        pbProgress.SetProgress(0);
        pbProgress.Maximum = engine.AddItems.Count;
        lblStatusLabel.Text = "Identifying Movies...";

        var progressHandler = new Progress<NewMovieProgressReport>(report =>
        {
            // This body executes safely on the main thread
            pbProgress.SetProgress(report.NumberComplete);
            if (report.LatestItemProcessed is null)
            {
                return;
            }
            lblStatusLabel.Text = report.LatestItemProcessed.Movie?.ToUiVersion();
        });

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 4, // Limit concurrent tasks
            CancellationToken = cts.Token // Pass token to the loop mechanism
        };

        VolatileCounter.Reset();

        try
        {
            await Parallel.ForEachAsync(
                engine.AddItems,
                options,
                async (movie, token) =>
                {
                    if (cts.IsCancellationRequested)
                    {
                        return;
                    }

                    if (movie.CodeKnown)
                    {
                        return;
                    }

                    await movie.GuessMovieAsync(true);

                    var report = new NewMovieProgressReport
                    {
                        NumberComplete = VolatileCounter.Increment(),
                        LatestItemProcessed = movie
                    };

                    ((IProgress<NewMovieProgressReport>)progressHandler).Report(report);
                }
                );
        }
        catch (OperationCanceledException)
        {
            //OK
        }


        olvFMNewShows.UpdateObjects(engine.AddItems);
        olvFMNewShows.Update();
        ShowHideUpdateControls(false);
    }

    private void bnRemoveNewFolder_Click(object _, System.EventArgs e)
    {
        RemoveNewFolder();
    }

    private void RemoveNewFolder()
    {
        if (NothingSelected())
        {
            return;
        }

        foreach (PossibleNewMovie ai in olvFMNewShows.Selected())
        {
            engine.AddItems.Remove(ai);
            olvFMNewShows.RemoveObject(ai);
        }
    }

    private void bnIgnoreNewFolder_Click(object _, System.EventArgs e)
    {
        if (NothingSelected())
        {
            return;
        }

        DialogResult res = MessageBox.Show("Add selected folders to the 'Bulk Add Movies' ignore folders list?", "Bulk Add Movies", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (res != DialogResult.Yes)
        {
            return;
        }

        foreach (PossibleNewMovie ai in olvFMNewShows.Selected())
        {
            TVSettings.Instance.IgnoreFolders.Add(ai.Directory.FullName.ToLower());
            engine.AddItems.Remove(ai);
            olvFMNewShows.RemoveObject(ai);
        }
        mDoc.SetDirty();
        FillFolderStringLists();
    }

    private void lvFMNewShows_DragOver(object _, DragEventArgs e)
    {
        e.Effect = GetDragEffect(e);
    }

    private void lvFMNewShows_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete)
        {
            RemoveNewFolder();
        }
    }

    private void bnNewFolderOpen_Click(object sender, System.EventArgs e)
    {
        if (NothingSelected())
        {
            return;
        }

        if (olvFMNewShows.FirstSelected() is PossibleNewMovie ai)
        {
            ai.Directory.FullName.OpenFolder();
        }
    }

    private bool NothingSelected()
    {
        return !olvFMNewShows.AnySelected();
    }

    private void PopulateShowList()
    {
        olvFMNewShows.SetObjects(engine.AddItems);
    }

    private void UpdateListItem(PossibleNewMovie ai, bool makevis)
    {
        olvFMNewShows.UpdateObject(ai);

        if (makevis)
        {
            olvFMNewShows.EnsureModelVisible(ai);
        }
    }

    private async void bnFolderMonitorDone_Click(object sender, System.EventArgs e)
    {
        int numberToAdd = engine.AddItems.Count(ai => ai.CodeKnown);
        if (numberToAdd > 0)
        {
            DialogResult res = MessageBox.Show($"Add {numberToAdd} identified movies to \"My Movies\"?", "Bulk Add Movies", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res != DialogResult.Yes)
            {
                return;
            }

            await engine.AddAllToMyMoviesAsync(mainUi);
        }

        Close();
    }

    private void bnVisitTVcom_Click(object sender, System.EventArgs e)
    {
        if (NothingSelected())
        {
            return;
        }

        if (olvFMNewShows.FirstSelected() is not PossibleNewMovie fme)
        {
            return;
        }

        if (fme.CodeKnown)
        {
            switch (fme.SourceProvider)
            {
                case TVDoc.ProviderType.TheTVDB:
                    TheTVDB.API.WebsiteMovieUrl(fme.ProviderCode).OpenUrlInBrowser();
                    break;

                case TVDoc.ProviderType.TMDB:
                    TMDB.API.WebsiteMovieUrl(fme.ProviderCode).OpenUrlInBrowser();
                    break;
            }
        }
    }

    private async void bnCheck2_Click(object sender, System.EventArgs e)
    {
        await DoCheckAsync();
    }

    private async void lvFMNewShows_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        await EditEntryAsync();
    }

    private async void bnEditEntry_Click(object sender, System.EventArgs e)
    {
        await EditEntryAsync();
    }

    private async Task EditEntryAsync()
    {
        if (NothingSelected())
        {
            return;
        }

        if (olvFMNewShows.FirstSelected() is PossibleNewMovie fme)
        {
            await EditEntryAsync(fme);
            UpdateListItem(fme, true);
            olvFMNewShows.RefreshObject(fme);
        }
    }

    private async Task EditEntryAsync(PossibleNewMovie fme)
    {
        BulkAddEditMovie ed = new();
        await ed.SetHintAsync(fme);
        if (ed.ShowDialog(this) != DialogResult.OK || ed.Code == -1)
        {
            return;
        }

        fme.UpdateId(ed.Code, ed.Provider);
    }

    private void lstFMMonitorFolders_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        bnRemoveMonFolder.Enabled = lstFMMonitorFolders.SelectedIndices.Count > 0;
        bnOpenMonFolder.Enabled = lstFMMonitorFolders.SelectedIndices.Count > 0;
    }

    private void lstFMIgnoreFolders_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        bnRemoveIgFolder.Enabled = lstFMIgnoreFolders.SelectedIndices.Count > 0;
        bnOpenIgFolder.Enabled = lstFMIgnoreFolders.SelectedIndices.Count > 0;
    }

    private void lvFMNewShows_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        bool somethingSelected = olvFMNewShows.AnySelected();
        bnEditEntry.Enabled = somethingSelected;
        bnRemoveNewFolder.Enabled = somethingSelected;
        bnIgnoreNewFolder.Enabled = somethingSelected;
        bnVisitTVcom.Enabled = somethingSelected;
        bnNewFolderOpen.Enabled = somethingSelected;
    }

    private void btnStopScan_Click(object sender, EventArgs e)
    {
        cts.Cancel();
    }
}
