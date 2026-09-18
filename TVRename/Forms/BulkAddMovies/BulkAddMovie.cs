//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;
using DaveChambers.FolderBrowserDialogEx;
using SharpCompress.Common;
using System;
using System.ComponentModel;
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
    //public CancellationTokenSource TokenSource;
    private readonly TVDoc mDoc;

    private readonly BulkAddMovieManager engine;
    private readonly UI mainUi;

    //Thread safe counters to work out the progress
    //For auto id
    private static readonly ThreadSafeCounter VolatileCounter = new();

    public class ProgressReport
    {
        public int NumberComplete { get; set; }
        public int Total { get; set; }
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
    }

    private void bnClose_Click(object sender, System.EventArgs e)
    {
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

        if (UiHelpers.ShowDialogAndOk(searchFolderBrowser,this))
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

        if (UiHelpers.ShowDialogAndOk(ignoreFolderBrowser,this))
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

    private void bnCheck_Click(object sender, System.EventArgs e)
    {
        DoCheck();
    }

    private void DoCheck()
    {
        tbResults.Parent = tabControl1;

        tabControl1.SelectedTab = tbResults;
        tabControl1.Update();

        bnFullAuto.Enabled = false;
        pbProgress.Visible = true;
        lblStatusLabel.Visible = true;

        bwRescan.RunWorkerAsync();
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
        if (files == null)
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

        bnFullAuto.Enabled = false;
        pbProgress.Visible = true;
        lblStatusLabel.Visible = true;

        CancellationTokenSource cts = new();

        pbProgress.SetProgress(0);
        pbProgress.Maximum = engine.AddItems.Count;

        var progressHandler = new Progress<ProgressReport>(report =>
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

        await Parallel.ForEachAsync(
            engine.AddItems,
            options,
            async (movie,token) =>
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

                var report = new ProgressReport
                {
                    NumberComplete = VolatileCounter.Increment(),
                    LatestItemProcessed = movie
                };

                ((IProgress<ProgressReport>)progressHandler).Report(report);
            }
            );

        cts.Cancel();

        olvFMNewShows.UpdateObjects(engine.AddItems );
        olvFMNewShows.Update();
        bnFullAuto.Enabled = true;
        pbProgress.Visible = false;
        lblStatusLabel.Visible = false;
    }

    private void bnRemoveNewFolder_Click(object _, System.EventArgs e)
    {
        RemoveNewFolder();
    }

    private void RemoveNewFolder()
    {
        if (olvFMNewShows.SelectedObjects.Count == 0)
        {
            return;
        }

        foreach (PossibleNewMovie ai in olvFMNewShows.SelectedObjects.OfType<PossibleNewMovie>())
        {
            engine.AddItems.Remove(ai);
            olvFMNewShows.RemoveObject(ai);
        }
    }

    private void bnIgnoreNewFolder_Click(object _, System.EventArgs e)
    {
        if (olvFMNewShows.SelectedObjects.Count == 0)
        {
            return;
        }

        DialogResult res = MessageBox.Show("Add selected folders to the 'Bulk Add Movies' ignore folders list?", "Bulk Add Movies", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (res != DialogResult.Yes)
        {
            return;
        }

        foreach (PossibleNewMovie ai in olvFMNewShows.SelectedObjects.OfType<PossibleNewMovie>())
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
        if (olvFMNewShows.SelectedObjects.Count == 0)
        {
            return;
        }

        if (olvFMNewShows.SelectedObjects.OfType<PossibleNewMovie>().FirstOrDefault() is PossibleNewMovie ai)
        {
            ai.Directory.FullName.OpenFolder();
        }
    }

    private void PopulateShowList()
    {
        olvFMNewShows.SetObjects(engine.AddItems);
    }

    private void UpdateListItem(PossibleNewMovie? ai, bool makevis)
    {
        if (ai is null)
        {
            return;
        }
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
        if (olvFMNewShows.SelectedObjects.Count == 0)
        {
            return;
        }

        if (olvFMNewShows.SelectedObjects.OfType<PossibleNewMovie>().FirstOrDefault() is not PossibleNewMovie fme)
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

    private void bnCheck2_Click(object sender, System.EventArgs e)
    {
        DoCheck();
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
        if (olvFMNewShows.SelectedObjects.Count == 0)
        {
            return;
        }

        if (olvFMNewShows.SelectedObjects.OfType<PossibleNewMovie>().FirstOrDefault() is PossibleNewMovie fme)
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

        fme.SetId(ed.Code, ed.Provider);
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
        bool somethingSelected = olvFMNewShows.SelectedObjects.Count > 0;
        bnEditEntry.Enabled = somethingSelected;
        bnRemoveNewFolder.Enabled = somethingSelected;
        bnIgnoreNewFolder.Enabled = somethingSelected;
        bnVisitTVcom.Enabled = somethingSelected;
        bnNewFolderOpen.Enabled = somethingSelected;
    }

    private void bwRescan_DoWork(object sender, DoWorkEventArgs e)
    {
        Thread.CurrentThread.Name ??= "BulkAddMovie Scan Thread"; // Can only set it once

        CancellationTokenSource cts = new();
        engine.CheckFoldersAsync((BackgroundWorker)sender, true, true, cts.Token).GetAwaiter().GetResult();
        cts.Cancel();
    }

    private void UpdateShowList()
    {
        //Unclear what we need to do here
    }

    private void bwIdentify_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        bnFullAuto.Enabled = true;
        pbProgress.Visible = false;
        lblStatusLabel.Visible = false;
    }

    private void bwRescan_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        pbProgress.SetProgress(e.ProgressPercentage);

        lblStatusLabel.Text = e.UserState?.ToString()?.ToUiVersion();
    }

    private void bwRescan_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        bnFullAuto.Enabled = true;
        pbProgress.Visible = false;
        lblStatusLabel.Visible = false;
        PopulateShowList();
    }
}
