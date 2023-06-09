using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using TVRename.Forms.ShowPreferences;
using TVRename.YTS;

namespace TVRename.Forms;

public partial class YtsRecommendationView : Form
{
    private List<YtsRecommendationRow> recs;
    private readonly TVDoc mDoc;
    private readonly UI mainUi;
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly List<MovieConfiguration> addedMovies;
    string quality;
    private DateTime scanStartTime;

    public YtsRecommendationView(TVDoc doc, UI main, string defaultQuality)
    {
        InitializeComponent();
        recs = new List<YtsRecommendationRow>();
        addedMovies = new List<MovieConfiguration>();

        mDoc = doc;
        mainUi = main;
        quality = defaultQuality;

        olvRating.GroupKeyGetter = rowObject => (int)Math.Floor(((YtsRecommendationRow)rowObject).StarScore);
        olvRating.GroupKeyToTitleConverter = key => $"{(int)key}/10 Rating";

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
        List<YtsRecommendationRow> recommendationRows = chkRemoveExisting.Checked
            ? recs.Where(x => mDoc.FilmLibrary.Movies.All(configuration => configuration.ImdbCode != x.ImdbCode)).ToList()
            : recs.ToList();

        lvRecommendations.SetObjects(recommendationRows, true);
    }

    private void ClearGrid()
    {
        lvRecommendations.BeginUpdate();
        lvRecommendations.Items.Clear();
        lvRecommendations.EndUpdate();
    }

    private void chkAirDateTest_CheckedChanged(object sender, EventArgs e)
    {
        UpdateUI();
    }

    private void AddMovieToLibrary(YtsRecommendationRow addedMovie)
    {
        string imdbCode = addedMovie.ImdbCode;
        string name = addedMovie.Name;

        CachedMovieInfo? movie = TMDB.LocalCache.Instance.LookupMovieByImdb(imdbCode, new Locale());
        if (movie is null)
        {
            Logger.Info($"Not adding {imdbCode}:{name} as the IMDB code is not found on TMDB");
            return;
        }

        // need to add a new showitem
        MovieConfiguration found = new(movie.TmdbCode, TVDoc.ProviderType.TMDB);

        if (found.ConfigurationProvider == TVSettings.Instance.DefaultMovieProvider)
        {
            found.ConfigurationProvider = TVDoc.ProviderType.libraryDefault;
        }

        if (mDoc.AlreadyContains(found))
        {
            Logger.Info($"Not adding {imdbCode}:{name} as it already exists in the library");
            return;
        }

        QuickLocateForm f = new(name, MediaConfiguration.MediaType.movie);

        if (f.ShowDialog(this) != DialogResult.OK)
        {
            Logger.Info($"Not adding {imdbCode}:{name} as the user cancelled addition");
            return;
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
        addedMovies.Add(found);
        addedMovie.SetShow(found);
    }

    private void rightClickMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
        rightClickMenu.Close();
    }

    private void BwScan_DoWork(object sender, DoWorkEventArgs e)
    {
        System.Threading.Thread.CurrentThread.Name ??= "Recommendations Scan Thread"; // Can only set it once

        RecommendationMovieStructure source = new();
        int page = 0;
        List<MovieConfiguration> inputMovies = mDoc.FilmLibrary.Movies.Where(m => m.ImdbCode != null && !m.ImdbCode.IsNullOrWhitespace()).ToList();
        scanStartTime = DateTime.Now;

        try
        {
            foreach (MovieConfiguration existingMovie in inputMovies)
            {
                page++;

                API.YtsMovie? ytsMovie = API.GetMovieByImdb(existingMovie.ImdbCode);
                if (ytsMovie is null || ytsMovie.Id==0)
                {
                    continue;
                }

                IEnumerable<API.YtsMovie>? relatedMovies = API.GetRelatedMovies(ytsMovie.Id);
                if (relatedMovies is null)
                {
                    continue;
                }

                //File these away
                foreach (API.YtsMovie relatedMovie in relatedMovies)
                {
                    source.Add(relatedMovie,existingMovie, ytsMovie);
                }

                ((BackgroundWorker)sender).ReportProgress(100 * page / inputMovies.Count,existingMovie.Name);
            }

            recs = source.AsRecommendationRows(mDoc);
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex, "UNHANDLED error obtinaing recommendations from YTS");
        }
    }

    private class RecommendationMovieStructure : Dictionary<int, Tuple<API.YtsMovie, List<Tuple<API.YtsMovie, MovieConfiguration>>>>
    {
        internal void Add(API.YtsMovie relatedMovie, MovieConfiguration existingMovie, API.YtsMovie ytsMovie)
        {
            if (ContainsKey(relatedMovie.Id))
            {
                this[relatedMovie.Id].Item2.Add(Tuple.Create(ytsMovie, existingMovie));
            }
            else
            {
                Add(relatedMovie.Id, new Tuple<API.YtsMovie, List<Tuple<API.YtsMovie, MovieConfiguration>>>(relatedMovie, new(new List<Tuple<API.YtsMovie, MovieConfiguration>> { new(ytsMovie, existingMovie) })));
            }
        }

        public List<YtsRecommendationRow> AsRecommendationRows(TVDoc mDoc)
        {
            return this.Select(m => new YtsRecommendationRow(m.Value.Item1, m.Value.Item2, mDoc)).ToList();
        }
    }

    private void BwScan_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        pbProgress.Value = e.ProgressPercentage.Between(0, 100);
        DateTime completionDateTime = scanStartTime.Add((DateTime.Now - scanStartTime) / (pbProgress.Value+1) * 100) ;
        lblStatus.Text = $"ETC={completionDateTime} {e.UserState?.ToString()?.ToUiVersion()}";
    }

    private void BwScan_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        btnRefresh.Visible = true;
        pbProgress.Visible = false;
        lblStatus.Visible = false;
        if (lvRecommendations.IsDisposed)
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

    private void lvRecommendations_CellRightClick(object sender, BrightIdeasSoftware.CellRightClickEventArgs e)
    {
        if (e.Model is null)
        {
            return;
        }

        YtsRecommendationRow lastSelected = (YtsRecommendationRow)e.Model;

        rightClickMenu.Items.Clear();

        rightClickMenu.Add("Add Movie to Library and Download", (_, _) =>
        {
            AddMovieToLibrary(lastSelected);
            Download(lastSelected, quality);
        });

        rightClickMenu.Add("Add Movie to Library", (_, _) => AddMovieToLibrary(lastSelected));
        rightClickMenu.Add("Download Movie", (_, _) => Download(lastSelected, quality));
    }

    private static void Download(YtsRecommendationRow lastSelected, string qualityToDownload)
    {
        string? url = lastSelected.Downloads.FirstOrDefault(d => d.Quality == qualityToDownload)?.Url
                     ?? lastSelected.Downloads.FirstOrDefault()?.Url;

        url?.OpenUrlInBrowser();
    }

    private void lvRecommendations_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
    {
        if ((e.Item as BrightIdeasSoftware.OLVListItem)?.RowObject is not YtsRecommendationRow rr)
        {
            return;
        }

        UI.SetHtmlBody(chrRecommendationPreview,
            rr.Movie != null
                ? rr.Movie.GetMovieHtmlOverview(false)
                : rr.YtsMovie.GetMovieHtmlOverview());
    }
    private void this_FormClosing(object sender, FormClosingEventArgs e)
    {
        mDoc.MoviesAddedOrEdited(true, false, false, mainUi, addedMovies);
    }

    private void btnPreferences_Click(object sender, EventArgs e)
    {
        //YTSRecommendationViewPreferences prefs = new();
        //if (prefs.ShowDialog(this) != DialogResult.OK)
        //{
        //    return;
        //}

        PopulateGrid();
    }
}

