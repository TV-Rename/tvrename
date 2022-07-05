using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using TVRename.Forms.ShowPreferences;
using TVRename.Forms.Tools;

namespace TVRename.Forms;

public partial class RecommendationView : Form
{
    private Recomendations recs;
    private readonly TVDoc mDoc;
    private readonly UI mainUi;
    private readonly MediaConfiguration.MediaType media;
    private readonly IEnumerable<ShowConfiguration> tvShows;
    private readonly IEnumerable<MovieConfiguration> movies;
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly List<ShowConfiguration> addedShows;
    private readonly List<MovieConfiguration> addedMovies;
    private int trendingWeight=100;
    private int topWeight = 100;
    private int relatedWeight = 100;
    private int similarWeight = 100;

    private RecommendationView(TVDoc doc, UI main)
    {
        InitializeComponent();
        recs = new Recomendations();
        tvShows = new List<ShowConfiguration>();
        movies = new List<MovieConfiguration>();
        addedShows = new List<ShowConfiguration>();
        addedMovies = new List<MovieConfiguration>();

        mDoc = doc;
        mainUi = main;

        olvScore.MakeGroupies(new[] {0.1, 0.25, 0.5, 0.75 }, new[] { "0-10%","10-25%", "25-50%", "50-75%", "75%+" });

        olvRating.GroupKeyGetter = rowObject => (int) Math.Floor(((RecommendationRow) rowObject).StarScore);
        olvRating.GroupKeyToTitleConverter = key => $"{(int)key}/10 Rating";
    }

    public RecommendationView(TVDoc doc, UI main, MediaConfiguration.MediaType type) : this(doc, main)
    {
        media = type;
        switch (type)
        {
            case MediaConfiguration.MediaType.tv:
                tvShows = doc.TvLibrary.Shows;
                break;

            case MediaConfiguration.MediaType.movie:
                movies = doc.FilmLibrary.Movies;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
        Scan();
    }

    // ReSharper disable once UnusedMember.Global
    public RecommendationView(TVDoc doc, UI main, IEnumerable<MovieConfiguration> m) : this(doc, main)
    {
        media = MediaConfiguration.MediaType.movie;
        movies = m;
        Scan();
    }

    // ReSharper disable once UnusedMember.Global
    public RecommendationView(TVDoc doc, UI main, IEnumerable<ShowConfiguration> s) : this(doc, main)
    {
        media = MediaConfiguration.MediaType.tv;
        tvShows = s;
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
        List<RecommendationResult> recommendationRows = chkRemoveExisting.Checked
            ? media == MediaConfiguration.MediaType.movie
                ? recs.Values.Where(x => mDoc.FilmLibrary.Movies.All(configuration => configuration.TmdbCode != x.Key)).ToList()
                : recs.Values.Where(x => mDoc.TvLibrary.Shows.All(configuration => configuration.TmdbCode != x.Key)).ToList()
            : recs.Values.ToList();

        int maxRelated = recommendationRows.MaxOrDefault(x => x.Related.Count,0);
        int maxSimilar = recommendationRows.MaxOrDefault(x => x.Similar.Count, 0);

        lvRecommendations.SetObjects(recommendationRows.Select(x => new RecommendationRow(x, media, trendingWeight, topWeight, relatedWeight, similarWeight,maxRelated,maxSimilar)),true);
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

    private void AddTvToLibrary(int id, CachedSeriesInfo cache)
    {
        string name = TVSettings.Instance.DefaultTVShowFolder(cache);

        ShowConfiguration newShow = new(id, TVDoc.ProviderType.TMDB);

        if (newShow.ConfigurationProvider == TVSettings.Instance.DefaultProvider)
        {
            newShow.ConfigurationProvider = TVDoc.ProviderType.libraryDefault;
        }

        if (mDoc.AlreadyContains(newShow))
        {
            Logger.Info($"Not adding {id}:{name} as it already exists in the library");
            return;
        }

        QuickLocateForm f = new(name, MediaConfiguration.MediaType.tv);

        if (f.ShowDialog(this) != DialogResult.OK)
        {
            Logger.Info($"Not adding {id}:{name} as user cancelled addition");
            return;
        }

        newShow.AutoAddFolderBase = f.DirectoryFullPath;

        mDoc.Add(newShow.AsList(), true);
        addedShows.Add(newShow);
    }

    private void AddMovieToLibrary(int id, string? name)
    {
        // need to add a new showitem
        MovieConfiguration found = new(id, TVDoc.ProviderType.TMDB);

        if (found.ConfigurationProvider == TVSettings.Instance.DefaultMovieProvider)
        {
            found.ConfigurationProvider = TVDoc.ProviderType.libraryDefault;
        }

        if (mDoc.AlreadyContains(found))
        {
            Logger.Info($"Not adding {id}:{name} as it already exists in the library");
            return;
        }

        QuickLocateForm f = new(name, MediaConfiguration.MediaType.movie);

        if (f.ShowDialog(this) != DialogResult.OK)
        {
            Logger.Info($"Not adding {id}:{name} as the user cancelled addition");
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
        System.Threading.Thread.CurrentThread.Name ??= "Recommendations Scan Thread"; // Can only set it once
        string languageCode = TVSettings.Instance.TMDBLanguage.Abbreviation;
        switch (media)
        {
            case MediaConfiguration.MediaType.tv:
                recs = TMDB.LocalCache.Instance.GetRecommendations((BackgroundWorker)sender, tvShows.ToList(), languageCode).Result;
                foreach (KeyValuePair<int, RecommendationResult> rec in recs)
                {
                    Logger.Warn($"{rec.Key,-10} | {(rec.Value.TopRated ? "Top" : "   ")} | {(rec.Value.Trending ? "Trend" : "    ")} | {rec.Value.Related.Count,5} | {rec.Value.Similar.Count,5} | {mDoc.TvLibrary.Shows.All(configuration => configuration.TmdbCode != rec.Key)} | {TMDB.LocalCache.Instance.GetSeries(rec.Key)?.Name}");
                }
                break;

            case MediaConfiguration.MediaType.movie:
                recs = TMDB.LocalCache.Instance.GetRecommendations((BackgroundWorker)sender, movies.ToList(), languageCode).Result;
                foreach (KeyValuePair<int, RecommendationResult> rec in recs)
                {
                    Logger.Warn($"{rec.Key,-10} | {(rec.Value.TopRated ? "Top" : "   ")} | {(rec.Value.Trending ? "Trend" : "    ")} | {rec.Value.Related.Count,5} | {rec.Value.Similar.Count,5} | {TMDB.LocalCache.Instance.GetMovie(rec.Key)?.IsSearchResultOnly} | {TMDB.LocalCache.Instance.GetMovie(rec.Key)?.Name}");
                }
                break;

            default:
                throw new ArgumentOutOfRangeException();
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

        RecommendationRow lastSelected = (RecommendationRow)e.Model;

        possibleMergedEpisodeRightClickMenu.Items.Clear();

        switch (media)
        {
            case MediaConfiguration.MediaType.movie:
                AddRcMenuItem("Add Movie to Library", (_, _) => AddMovieToLibrary(lastSelected.Key, lastSelected.Name));
                break;
            case MediaConfiguration.MediaType.tv:
                if (lastSelected.Series != null)
                {
                    AddRcMenuItem("Add TV show to Library",
                        (_, _) => AddTvToLibrary(lastSelected.Key, lastSelected.Series));
                }
                break;
        }
    }

    private void lvRecommendations_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
    {
        if ((e.Item as BrightIdeasSoftware.OLVListItem)?.RowObject is not RecommendationRow rr)
        {
            return;
        }
        if (rr.Movie != null)
        {
            UI.SetHtmlBody(chrRecommendationPreview, rr.Movie.GetMovieHtmlOverview(rr));
        }
        else if (rr.Series != null)
        {
            UI.SetHtmlBody(chrRecommendationPreview, rr.Series.GetShowHtmlOverview(rr));
        }
    }
    private void this_FormClosing(object sender, FormClosingEventArgs e)
    {
        mDoc.MoviesAddedOrEdited(true, false, false, mainUi, addedMovies);
        mDoc.TvAddedOrEdited(true, false, false, mainUi, addedShows);
    }

    private void btnPreferences_Click(object sender, EventArgs e)
    {
        RecommendationViewPreferences prefs = new(trendingWeight, topWeight, relatedWeight, similarWeight);
        if (prefs.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        trendingWeight = prefs.TrendingWeight;
        topWeight = prefs.TopWeight;
        relatedWeight = prefs.RelatedWeight;
        similarWeight = prefs.SimilarWeight;

        PopulateGrid();
    }
}
