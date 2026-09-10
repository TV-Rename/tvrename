using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TVRename.Forms.ShowPreferences;

namespace TVRename.Forms;

public partial class CollectionsView : Form
{
    private readonly List<CollectionMember> collectionMovies;
    private readonly TVDoc mDoc;
    private readonly UI mainUi;
    private readonly List<MovieConfiguration> allAdded;

    public CollectionsView(TVDoc doc, UI main)
    {
        InitializeComponent();
        collectionMovies = [];
        allAdded = [];
        mDoc = doc;
        mainUi = main;
        Scan();
    }

    // ReSharper disable once InconsistentNaming
    private void UpdateUI()
    {
        if (chkRemoveCompleted.Checked && !chkRemoveFuture.Checked)
        {
            List<string> incompleteCollections = [.. collectionMovies.GroupBy(member => member.CollectionName)
                .Where(members => members.Any(x => !x.IsInLibrary)).Select(members => members.Key)];

            List<CollectionMember> incompleteCollectionMovies =
                [.. collectionMovies.Where(member => incompleteCollections.Contains(member.CollectionName))];
            olvCollections.SetObjects(incompleteCollectionMovies, true);

            return;
        }

        if (!chkRemoveCompleted.Checked && !chkRemoveFuture.Checked)
        {
            olvCollections.SetObjects(collectionMovies, true);
            return;
        }

        if (chkRemoveFuture.Checked)
        {
            IEnumerable<CollectionMember> historicCollectionMovies =
                collectionMovies.Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value < TimeHelpers.LocalNow() && m.MovieYear.HasValue);

            if (!chkRemoveCompleted.Checked)
            {
                olvCollections.SetObjects(historicCollectionMovies, true);
                return;
            }

            List<string> incompleteHistCollections = [.. historicCollectionMovies.GroupBy(member => member.CollectionName)
                .Where(members => members.Any(x => !x.IsInLibrary)).Select(members => members.Key)];

            List<CollectionMember> incompleteHistCollectionMovies =
                [.. collectionMovies
                    .Where(member => incompleteHistCollections.Contains(member.CollectionName))
                    .Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value < TimeHelpers.LocalNow() && m.MovieYear.HasValue)];

            olvCollections.SetObjects(incompleteHistCollectionMovies, true);
        }
    }

    private void rightClickMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
        rightClickMenu.Close();
    }

    private void BwScan_DoWork(object sender, DoWorkEventArgs e)
    {
        Thread.CurrentThread.Name ??= "CollectionView Scan Thread"; // Can only set it once
        BackgroundWorker bw = (BackgroundWorker)sender;

        List<(int, string)> collectionIds = mDoc.FilmLibrary.Collections;

        int total = collectionIds.Count;
        ThreadSafeCounter current =new();

        collectionMovies.Clear();

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 8 // Limit to 8 concurrent downloads at a time
        };

        Parallel.ForEach(collectionIds, options, (collection) =>
        {
            Dictionary<int, CachedMovieInfo> shows =  TMDB.LocalCache.Instance.GetMovieIdsFromCollectionAsync(collection.Item1, TVSettings.Instance.TMDBLanguage.Abbreviation).GetAwaiter().GetResult();
            foreach (KeyValuePair<int, CachedMovieInfo> neededShow in shows)
            {
                CollectionMember c = new(collection.Item2, neededShow.Value);

                c.IsInLibrary = mDoc.FilmLibrary.Movies.Any(configuration => configuration.TmdbCode == c.TmdbCode);
                collectionMovies.Add(c);
            }

            bw.ReportProgress(100 * current.Increment() / total, collection.Item2);
        });
    }

    private void BwScan_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        pbProgress.Value = e.ProgressPercentage.Between(0, 100);
        lblStatus.Text = e.UserState?.ToString()?.ToUiVersion();
    }

    private void BwScan_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        btnRefresh.Visible = true;
        pbProgress.Visible = false;
        lblStatus.Visible = false;
        if (olvCollections.IsDisposed)
        {
            return;
        }

        UpdateUI();
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

    private void olvDuplicates_CellRightClick(object sender, BrightIdeasSoftware.CellRightClickEventArgs e)
    {
        if (e.Model is null)
        {
            return;
        }

        CollectionMember mlastSelected = (CollectionMember)e.Model;

        rightClickMenu.Items.Clear();

        if (mlastSelected.IsInLibrary)
        {
            TVDoc.ProviderType providerToUse = TVSettings.Instance.DefaultMovieProvider == TVDoc.ProviderType.TMDB ? TVSettings.Instance.DefaultMovieProvider : TVDoc.ProviderType.TMDB;
            MovieConfiguration? si = mDoc.FilmLibrary.GetMovie(mlastSelected.TmdbCode, providerToUse);
            if (si != null)
            {
                rightClickMenu.Add("Force Refresh", async (_, _) => await mainUi.ForceMovieRefreshAsync(si, false));
                rightClickMenu.Add("Edit Movie", async (_, _) => await mainUi.EditMovieAsync(si));
            }
        }
        else
        {
            rightClickMenu.Add("Add to Library...", (_, _) => AddToLibrary(mlastSelected.Movie));
        }
    }

    private void AddToLibrary(CachedMovieInfo si)
    {
        // need to add a new showitem
        MovieConfiguration found = new(si.TmdbCode, TVDoc.ProviderType.TMDB);
        QuickLocateForm f = new(si.Name, MediaConfiguration.MediaType.movie);

        if (f.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (found.ConfigurationProvider == TVSettings.Instance.DefaultMovieProvider)
        {
            found.ConfigurationProvider = TVDoc.ProviderType.libraryDefault;
        }

        if (f.FolderNameChanged)
        {
            found.UseAutomaticFolders = false;
            found.UseManualLocations = true;
            found.ManualLocations.Add(f.DirectoryFullPath);
        }
        else if (f.RootDirectory.HasValue())
        {
            found.AutomaticFolderRoot = f.RootDirectory;
            found.UseAutomaticFolders = true;
        }

        mDoc.Add(found.AsList(), true);
        allAdded.Add(found);
    }

    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
        UpdateUI();
    }

    private void chkRemoveFuture_CheckedChanged(object sender, EventArgs e)
    {
        UpdateUI();
    }

    private async void CollectionsView_FormClosing(object sender, FormClosingEventArgs e)
    {
        await mDoc.MoviesAddedOrEditedAsync(true, false, false, mainUi, allAdded);
    }
}
