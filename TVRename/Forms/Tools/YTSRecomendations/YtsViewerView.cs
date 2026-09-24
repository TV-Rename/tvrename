using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TVRename.Forms.ShowPreferences;

namespace TVRename.Forms;

public partial class YtsViewerView : Form
{
    private List<YtsViewerRow> recs;
    private readonly TVDoc mDoc;
    private readonly UI mainUi;
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly List<MovieConfiguration> addedMovies;
    private readonly string quality;
    private readonly int minRating;
    private DateTime scanStartTime;

    private CancellationTokenSource cts = new();
    private Task? scanTask;

    public YtsViewerView(TVDoc doc, UI main)
    {
        InitializeComponent();
        recs = [];
        addedMovies = [];

        mDoc = doc;
        mainUi = main;
        quality = "1080p";
        minRating = 5;
        chrRecommendationPreview.RequestHandler = new BrowserRequestHandler();

        olvRating.GroupKeyGetter = rowObject => (int)Math.Floor(((YtsViewerRow)rowObject).StarScore);
        olvRating.GroupKeyToTitleConverter = key => $"{(int)key}/10 Rating";

        StartScan();
    }

    // ReSharper disable once InconsistentNaming
    private void UpdateUI()
    {
        ClearGrid();
        PopulateGrid();
    }

    private void PopulateGrid()
    {
        List<YtsViewerRow> recommendationRows = chkRemoveExisting.Checked
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

    private async Task AddMovieToLibraryAsync(YtsViewerRow addedMovie)
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

    private async void BtnRefresh_Click_1(object sender, EventArgs e)
    {
        StartScan();
    }

    void StartScan()
    {
        cts = new();
        scanTask = Scan();
    }

    private async Task Scan()
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

        scanStartTime = TimeHelpers.LocalNow();
        try
        {
            recs = (await YTS.API.GetMoviesAsync(progressHandler, quality, minRating,cts.Token ))
                    .Select(x => new YtsViewerRow(x, mDoc))
                    .ToList();
        }
        catch (TaskCanceledException)
        {
            Logger.Warn("Error obtinaing recommendations from YTS - Task Cancelled");
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

    private void lvRecommendations_CellRightClick(object sender, BrightIdeasSoftware.CellRightClickEventArgs e)
    {
        if (e.Model is null)
        {
            return;
        }

        YtsViewerRow lastSelected = (YtsViewerRow)e.Model;

        rightClickMenu.Items.Clear();

        rightClickMenu.Add("Add Movie to Library and Download", async (_, _) =>
        {
            await AddMovieToLibraryAsync(lastSelected);
            Download(lastSelected, quality);
        });

        rightClickMenu.Add("Add Movie to Library", async (_, _) => await AddMovieToLibraryAsync(lastSelected));
        rightClickMenu.Add("Download Movie", (_, _) => Download(lastSelected, quality));
    }

    private static void Download(YtsViewerRow lastSelected, string qualityToDownload)
    {
        string? url = lastSelected.Downloads.FirstOrDefault(d => d.Quality == qualityToDownload)?.Url
                     ?? lastSelected.Downloads.FirstOrDefault()?.Url;

        url?.OpenUrlInBrowser();
    }

    private async void lvRecommendations_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
    {
        object? rowObject = (e.Item as BrightIdeasSoftware.OLVListItem)?.RowObject;
        if (rowObject is YtsViewerRow rr)
        {
            chrRecommendationPreview.SetHtmlBody(rr.Movie != null
                    ? await rr.Movie.GetMovieHtmlOverviewAsync(false)
                    : rr.YtsMovie.GetMovieHtmlOverview());
        }
    }
    private async void this_FormClosing(object sender, FormClosingEventArgs e)
    {
        await cts.CancelAsync();
        if (scanTask is not null) await scanTask;
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

