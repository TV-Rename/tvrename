using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
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
        collectionMovies = new List<CollectionMember>();
        allAdded = new List<MovieConfiguration>();
        mDoc = doc;
        mainUi = main;
        Scan();
    }

    // ReSharper disable once InconsistentNaming
    private void UpdateUI()
    {
        if (chkRemoveCompleted.Checked && !chkRemoveFuture.Checked)
        {
            List<string> incompleteCollections = collectionMovies.GroupBy(member => member.CollectionName)
                .Where(members => members.Any(x => !x.IsInLibrary)).Select(members => members.Key).ToList();

            List<CollectionMember> incompleteCollectionMovies =
                collectionMovies.Where(member => incompleteCollections.Contains(member.CollectionName)).ToList();
            olvCollections.SetObjects(incompleteCollectionMovies,true);

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
                collectionMovies.Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value < DateTime.Now && m.MovieYear.HasValue);

            if (!chkRemoveCompleted.Checked)
            {
                olvCollections.SetObjects(historicCollectionMovies, true);
                return;
            }

            List<string> incompleteHistCollections = historicCollectionMovies.GroupBy(member => member.CollectionName)
                .Where(members => members.Any(x => !x.IsInLibrary)).Select(members => members.Key).ToList();

            List<CollectionMember> incompleteHistCollectionMovies =
                collectionMovies
                    .Where(member => incompleteHistCollections.Contains(member.CollectionName))
                    .Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value < DateTime.Now && m.MovieYear.HasValue)
                    .ToList();

            olvCollections.SetObjects(incompleteHistCollectionMovies, true);
        }
    }

    private void AddRcMenuItem(string label, EventHandler command)
    {
        ToolStripMenuItem tsi = new(label.ToUiVersion());
        tsi.Click += command;
        possibleMergedEpisodeRightClickMenu.Items.Add(tsi);
    }

    private void PossibleMergedEpisodeRightClickMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
        possibleMergedEpisodeRightClickMenu.Close();
    }

    private void BwScan_DoWork(object sender, DoWorkEventArgs e)
    {
        Thread.CurrentThread.Name ??= "CollectionView Scan Thread"; // Can only set it once
        BackgroundWorker bw = (BackgroundWorker)sender;

        List<(int, string)> collectionIds = mDoc.FilmLibrary.Collections;

        int total = collectionIds.Count;
        int current = 0;

        collectionMovies.Clear();
        foreach ((int collectionId, string collectionName) in collectionIds)
        {
            Dictionary<int, CachedMovieInfo> shows = TMDB.LocalCache.Instance.GetMovieIdsFromCollection(collectionId, TVSettings.Instance.TMDBLanguage.Abbreviation);
            foreach (KeyValuePair<int, CachedMovieInfo> neededShow in shows)
            {
                CollectionMember c = new( collectionName,neededShow.Value);

                c.IsInLibrary = mDoc.FilmLibrary.Movies.Any(configuration => configuration.TmdbCode == c.TmdbCode);
                collectionMovies.Add(c);
            }

            bw.ReportProgress(100 * current++ / total, collectionName);
        }
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

        possibleMergedEpisodeRightClickMenu.Items.Clear();

        if (mlastSelected.IsInLibrary)
        {
            TVDoc.ProviderType providerToUse = TVSettings.Instance.DefaultMovieProvider == TVDoc.ProviderType.TMDB ? TVSettings.Instance.DefaultMovieProvider : TVDoc.ProviderType.TMDB;
            MovieConfiguration? si = mDoc.FilmLibrary.GetMovie(mlastSelected.TmdbCode, providerToUse);
            if (si != null)
            {
                AddRcMenuItem("Force Refresh", (_, _) => mainUi.ForceMovieRefresh(si, false));
                AddRcMenuItem("Edit Movie", (_, _) => mainUi.EditMovie(si));
            }
        }
        else
        {
            AddRcMenuItem("Add to Library...", (_, _) => AddToLibrary(mlastSelected.Movie));
        }

        //possibleMergedEpisodeRightClickMenu.Items.Add(new ToolStripSeparator());
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

    private void CollectionsView_FormClosing(object sender, FormClosingEventArgs e)
    {
        mDoc.MoviesAddedOrEdited(true, false,false , mainUi, allAdded);
    }
}
