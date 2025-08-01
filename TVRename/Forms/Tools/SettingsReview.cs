using BrightIdeasSoftware;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TVRename.Forms.Tools;

namespace TVRename.Forms;

public partial class SettingsReview : Form
{
    private readonly List<SettingsCheck> set;
    private readonly TVDoc mDoc;
    private readonly UI mainUi;

    public SettingsReview(TVDoc doc, UI main)
    {
        InitializeComponent();
        set = [];
        mDoc = doc;
        mainUi = main;
        Scan();
    }

    // ReSharper disable once InconsistentNaming
    private void UpdateUI()
    {
        olvDuplicates.SetObjects(set.Where(check => check.Check()));
        olvDuplicates.Sort(olvCheck);
    }

    private void rightClickMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
        rightClickMenu.Close();
    }

    private void BwScan_DoWork(object sender, DoWorkEventArgs e)
    {
        Thread.CurrentThread.Name ??= "SettingsReview Scan Thread"; // Can only set it once
        BackgroundWorker bw = (BackgroundWorker)sender;
        int total = mDoc.FilmLibrary.Movies.Count() + mDoc.TvLibrary.Shows.Count();
        int current = 0;

        set.Clear();

        foreach (MovieConfiguration movie in mDoc.FilmLibrary.GetSortedMovies())
        {
            set.Add(new CustomLanguageMovieCheck(movie, mDoc));
            set.Add(new CustomNameMovieCheck(movie, mDoc));
            set.Add(new CustomRegionMovieCheck(movie, mDoc));
            set.Add(new ManualFoldersMovieCheck(movie, mDoc));
            set.Add(new MovieCheckEmptyManualFolders(movie, mDoc));
            set.Add(new MovieManualFoldersMirrorAutomaticCheck(movie, mDoc));
            set.Add(new MovieFolderCheck(movie, mDoc));

            set.Add(new DefaultDoMissingMovieCheck(movie, mDoc));
            set.Add(new DefaultDoRenameMovieCheck(movie, mDoc));
            set.Add(new DefaultFutureMovieCheck(movie, mDoc));
            set.Add(new DefaultNoAirdateMovieCheck(movie, mDoc));

            set.Add(new FilenameMovieCheck(movie, mDoc));
            set.Add(new MovieProviderCheck(movie, mDoc));
            set.Add(new SubdirectoryMovieCheck(movie, mDoc));
            set.Add(new FolderBaseMovieCheck(movie, mDoc));
            set.Add(new MovieFolderTypeCheck(movie, mDoc));

            bw.ReportProgress(100 * current++ / total, movie.ShowName);
        }

        foreach (ShowConfiguration show in mDoc.TvLibrary.GetSortedShowItems())
        {
            set.Add(new CustomLanguageTvShowCheck(show, mDoc));
            set.Add(new CustomNameTvShowCheck(show, mDoc));
            set.Add(new CustomRegionTvShowCheck(show, mDoc));
            set.Add(new CustomSearchTvShowCheck(show, mDoc));
            set.Add(new UseManualFoldersTvShowCheck(show, mDoc));

            set.Add(new DefaultAirDateMatchingTvCheck(show, mDoc));
            set.Add(new DefaultDoMissingTvCheck(show, mDoc));
            set.Add(new DefaultDoRenameTvCheck(show, mDoc));
            set.Add(new DefaultEpisodeNameMatchingTvCheck(show, mDoc));
            set.Add(new DefaultFutureEpisodesTvCheck(show, mDoc));
            set.Add(new DefaultNoAirdateTvCheck(show, mDoc));
            set.Add(new DefaultSequentialMatchingTvCheck(show, mDoc));
            set.Add(new DefaultShowAirDateTvCheck(show, mDoc));
            set.Add(new DefaultSpecialsAsEpisodesTvCheck(show, mDoc));
            set.Add(new DefaultUseDvdTvCheck(show, mDoc));
            set.Add(new DefaultUseAlternateTvCheck(show, mDoc));

            set.Add(new TvShowEpisodeNameCheck(show, mDoc));
            set.Add(new TvShowProviderCheck(show, mDoc));
            set.Add(new TvShowSeasonFormatCheck(show, mDoc));
            set.Add(new FolderBaseTvCheck(show, mDoc));
            set.Add(new FolderBaseLibraryDefaultTvCheck(show, mDoc));
            set.Add(new TvShowSubdiretoryFormatCheck(show, mDoc));

            bw.ReportProgress(100 * current++ / total, show.ShowName);
        }
    }

    private void BwScan_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        pbProgress.Value = e.ProgressPercentage.Between(0, 100);
        lblStatus.Text = e.UserState?.ToString()?.ToUiVersion();

        UiHelpers.SetProgress(e.ProgressPercentage.Between(0, 100), mainUi.Handle);
    }

    private void BwScan_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        UiHelpers.SetProgressStateNone(mainUi.Handle);
        btnRefresh.Visible = true;
        pbProgress.Visible = false;
        lblStatus.Visible = false;
        if (olvDuplicates.IsDisposed)
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
        UiHelpers.SetProgressStateNormal(mainUi.Handle);
        bwScan.RunWorkerAsync();
    }

    private void olvDuplicates_CellRightClick(object sender, CellRightClickEventArgs e)
    {
        if (e.Model is null)
        {
            return;
        }

        rightClickMenu.Items.Clear();
        if (olvDuplicates.SelectedObjects.Count == 1)
        {
            SettingsCheck mlastSelected = (SettingsCheck)e.Model;

            if (mlastSelected is MovieCheck mcheck)
            {
                MovieConfiguration si = mcheck.Movie;
                rightClickMenu.Add("Force Refresh",
                    (_, _) => mainUi.ForceMovieRefresh(new List<MovieConfiguration> { si }, false));

                rightClickMenu.Add("Edit Movie", (_, _) => mainUi.EditMovie(si));

                rightClickMenu.AddSeparator();
                foreach (string? f in si.Locations)
                {
                    rightClickMenu.Add("Visit " + f, (_, _) => f.OpenFolder());
                }
            }
            else if (mlastSelected is TvShowCheck tcheck)
            {
                ShowConfiguration si = tcheck.Show;
                rightClickMenu.Add("Force Refresh",
                    (_, _) => mainUi.ForceRefresh(new List<ShowConfiguration> { si }, false));

                rightClickMenu.Add("Edit TV Show", (_, _) => mainUi.EditShow(si));
            }

            rightClickMenu.AddSeparator();
            rightClickMenu.Add("Fix Issue", (_, _) => Remedy(mlastSelected.AsList()));
        }
        else
        {
            rightClickMenu.Add("Fix Issues", (_, _) =>
            {
                Remedy(olvDuplicates.SelectedObjects.OfType<SettingsCheck>());
                mDoc.SetDirty();
            });
        }
    }

    private void Remedy(IEnumerable<SettingsCheck> selectedItems)
    {
        FixIssuesNotifier form = new(new RemedySettings(selectedItems, this));
        form.ShowDialog();
    }

    internal void Remove(SettingsCheck selected)
    {
        olvDuplicates.RemoveObject(selected);
        set.Remove(selected);
    }
}
