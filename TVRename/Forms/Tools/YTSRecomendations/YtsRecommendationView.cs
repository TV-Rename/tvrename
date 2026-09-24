using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    private readonly string quality;
    private DateTime scanStartTime;
    private Task? scanTask;


    public YtsRecommendationView(TVDoc doc, UI main, string defaultQuality)
    {
        InitializeComponent();
        recs = [];
        addedMovies = [];

        mDoc = doc;
        mainUi = main;
        quality = defaultQuality;
        chrRecommendationPreview.RequestHandler = new BrowserRequestHandler();

        olvRating.GroupKeyGetter = rowObject => (int)Math.Floor(((YtsRecommendationRow)rowObject).StarScore);
        olvRating.GroupKeyToTitleConverter = key => $"{(int)key}/10 Rating";

        StartScan();
    }
    void StartScan()
    {
        scanTask = ScanAsync();
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
            ? [.. recs.Where(x => mDoc.FilmLibrary.Movies.All(configuration => configuration.ImdbCode != x.ImdbCode))]
            : [.. recs];

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

    private async Task AddMovieToLibraryAsync(YtsRecommendationRow addedMovie)
    {
        string imdbCode = addedMovie.ImdbCode;
        string name = addedMovie.Name;

        CachedMovieInfo? movie = await TMDB.LocalCache.Instance.LookupMovieByImdbAsync(imdbCode, new Locale());
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

        await mDoc.AddAsync(found.AsList(), true);
        addedMovies.Add(found);
        addedMovie.SetShow(found);
    }

    private void rightClickMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
        rightClickMenu.Close();
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
                Add(relatedMovie.Id, new Tuple<API.YtsMovie, List<Tuple<API.YtsMovie, MovieConfiguration>>>(relatedMovie, [new(ytsMovie, existingMovie)]));
            }
        }

        public List<YtsRecommendationRow> AsRecommendationRows(TVDoc mDoc)
        {
            return [.. this.Select(m => new YtsRecommendationRow(m.Value.Item1, m.Value.Item2, mDoc))];
        }
    }

    private async void BtnRefresh_Click_1(object sender, EventArgs e)
    {
        StartScan();
    }

    public async Task ScanAsync()
    {
        var progressHandler = new Progress<ProgressReport>(scanReport =>
        {
            // This body executes safely on the main thread
            pbProgress.SetProgress(scanReport.ProgressPercentage);

            DateTime completionDateTime = scanStartTime.Add((TimeHelpers.LocalNow() - scanStartTime) / (pbProgress.Value + 1) * 100);
            lblStatus.Text = $"ETC={completionDateTime} {scanReport.UpdateText.ToUiVersion()}";
        });

        btnRefresh.Visible = false;
        pbProgress.Visible = true;
        lblStatus.Visible = true;

        RecommendationMovieStructure source = new();
        ThreadSafeCounter page = new();

        List<MovieConfiguration> inputMovies = [.. mDoc.FilmLibrary.Movies.Where(m => m.ImdbCode != null && !m.ImdbCode.IsNullOrWhitespace())];
        scanStartTime = TimeHelpers.LocalNow();

        try
        {
            CancellationTokenSource cts = new();

            await Parallel.ForEachAsync(
                inputMovies,
                new ParallelOptions {
                    MaxDegreeOfParallelism = 2* TVSettings.Instance.ParallelDownloads,
                    CancellationToken = cts.Token },
                async (existingMovie, token) =>
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    ((IProgress<ProgressReport>)progressHandler).Report(new ProgressReport()
                    {
                        ProgressPercentage = 100 * page.Increment() / inputMovies.Count,
                        UpdateText = existingMovie.Name ?? string.Empty
                    });

                    await ScanMovie(source, existingMovie);
                });


            recs = source.AsRecommendationRows(mDoc);
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex, "UNHANDLED error obtinaing recommendations from YTS");
        }

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

    private static async Task<bool> ScanMovie(RecommendationMovieStructure source, MovieConfiguration existingMovie)
    {
        API.YtsMovie? ytsMovie = await API.GetMovieByImdbAsync(existingMovie.ImdbCode);
        if (ytsMovie is null || ytsMovie.Id == 0)
        {
            return false;
        }

        IEnumerable<API.YtsMovie>? relatedMovies = await API.GetRelatedMoviesAsync(ytsMovie.Id);
        if (relatedMovies is null)
        {
            return false;
        }

        //File these away
        foreach (API.YtsMovie relatedMovie in relatedMovies)
        {
            source.Add(relatedMovie, existingMovie, ytsMovie);
        }

        return true;
    }

    private void lvRecommendations_CellRightClick(object sender, BrightIdeasSoftware.CellRightClickEventArgs e)
    {
        if (e.Model is null)
        {
            return;
        }

        YtsRecommendationRow lastSelected = (YtsRecommendationRow)e.Model;

        rightClickMenu.Items.Clear();

        rightClickMenu.Add("Add Movie to Library and Download", async (_, _) =>
        {
            await AddMovieToLibraryAsync(lastSelected);
            Download(lastSelected, quality);
        });

        rightClickMenu.Add("Add Movie to Library", async (_, _) =>  await AddMovieToLibraryAsync(lastSelected));
        rightClickMenu.Add("Download Movie", (_, _) => Download(lastSelected, quality));
    }

    private static void Download(YtsRecommendationRow lastSelected, string qualityToDownload)
    {
        string? url = lastSelected.Downloads.FirstOrDefault(d => d.Quality == qualityToDownload)?.Url
                     ?? lastSelected.Downloads.FirstOrDefault()?.Url;

        url?.OpenUrlInBrowser();
    }

    private async void lvRecommendations_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
    {
        if (e.Item is BrightIdeasSoftware.OLVListItem { RowObject: YtsRecommendationRow rr })
        {
            chrRecommendationPreview.SetHtmlBody(rr.Movie != null
                    ? await rr.Movie.GetMovieHtmlOverviewAsync(false)
                    : rr.YtsMovie.GetMovieHtmlOverview());
        }
    }
    private async void this_FormClosing(object sender, FormClosingEventArgs e)
    {
        await mDoc.MoviesAddedOrEditedAsync(true, false, false, mainUi, addedMovies);
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

