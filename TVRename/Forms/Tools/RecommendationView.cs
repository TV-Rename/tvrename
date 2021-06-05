using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace TVRename.Forms
{
    public partial class RecommendationView : Form
    {
        private Recomendations recs;
        private readonly TVDoc mDoc;
        private readonly UI mainUi;
        private readonly MediaConfiguration.MediaType media;
        private readonly IEnumerable<ShowConfiguration> tvShows;
        private readonly IEnumerable<MovieConfiguration> movies;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private RecommendationView([NotNull] TVDoc doc, UI main)
        {
            InitializeComponent();
            recs = new Recomendations();
            tvShows = new List<ShowConfiguration>();
            movies = new List<MovieConfiguration>();
            mDoc = doc;
            mainUi = main;

            olvScore.MakeGroupies(new[] { 5, 10, 20 }, new[] { "0-5", "5-10", "10-20", "20+" });

            //olvRating.MakeGroupies(new[] { 2, 4, 6, 8 }, new[] { "*", "**", "***", "****","*****" });
        }

        public RecommendationView([NotNull] TVDoc doc, UI main, MediaConfiguration.MediaType type) : this(doc, main)
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

        public RecommendationView([NotNull] TVDoc doc, UI main, IEnumerable<MovieConfiguration> m) : this(doc, main)
        {
            media = MediaConfiguration.MediaType.movie;
            movies = m;
            Scan();
        }

        public RecommendationView([NotNull] TVDoc doc, UI main, IEnumerable<ShowConfiguration> s) : this(doc, main)
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
            IEnumerable<RecommendationResult> recommendationRows = chkRemoveExisting.Checked
                ? media == MediaConfiguration.MediaType.movie
                    ? recs.Values.Where(x => mDoc.FilmLibrary.Movies.All(configuration => configuration.TmdbCode != x.Key))
                    : recs.Values.Where(x => mDoc.TvLibrary.Shows.All(configuration => configuration.TmdbCode != x.Key))
                : recs.Values;

            lvRecommendations.SetObjects(recommendationRows.Select(x => new RecommendationRow(x, media)));
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

        private void AddToLibrary(int mlastSelectedKey, TVDoc tvDoc)
        {
            switch (media)
            {
                case MediaConfiguration.MediaType.tv:
                    ShowConfiguration show = new ShowConfiguration(mlastSelectedKey, TVDoc.ProviderType.TMDB);
                    tvDoc.Add(show);
                    break;

                case MediaConfiguration.MediaType.movie:
                    MovieConfiguration newMovie = new MovieConfiguration(mlastSelectedKey, TVDoc.ProviderType.TMDB);
                    tvDoc.Add(newMovie);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AddRcMenuItem(string label, EventHandler command)
        {
            ToolStripMenuItem tsi = new ToolStripMenuItem(label);
            tsi.Click += command;
            possibleMergedEpisodeRightClickMenu.Items.Add(tsi);
        }

        private void PossibleMergedEpisodeRightClickMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            possibleMergedEpisodeRightClickMenu.Close();
        }

        private void BwScan_DoWork(object sender, DoWorkEventArgs e)
        {
            string languageCode = TVSettings.Instance.TMDBLanguage.Abbreviation;
            switch (media)
            {
                case MediaConfiguration.MediaType.tv:
                    recs = TMDB.LocalCache.Instance.GetRecommendations(mDoc, (BackgroundWorker)sender, tvShows.ToList(), languageCode).Result;
                    foreach (KeyValuePair<int, RecommendationResult> rec in recs)
                    {
                        Logger.Warn($"{rec.Key,-10} | {(rec.Value.TopRated ? "Top" : "   ")} | {(rec.Value.Trending ? "Trend" : "    ")} | {rec.Value.Related.Count,5} | {rec.Value.Similar.Count,5} | {mDoc.TvLibrary.Shows.All(configuration => configuration.TmdbCode != rec.Key)} | {TMDB.LocalCache.Instance.GetSeries(rec.Key)?.Name}");
                    }
                    break;

                case MediaConfiguration.MediaType.movie:
                    recs = TMDB.LocalCache.Instance.GetRecommendations(mDoc, (BackgroundWorker)sender, movies.ToList(), languageCode).Result;
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
            pbProgress.Value = e.ProgressPercentage;
            lblStatus.Text = e.UserState.ToString();
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

            RecommendationRow mlastSelected = (RecommendationRow)e.Model;

            possibleMergedEpisodeRightClickMenu.Items.Clear();

            AddRcMenuItem("Add to Library", (o, args) => AddToLibrary(mlastSelected.Key, mDoc));
        }

        private void lvRecommendations_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            RecommendationRow? rr = (e.Item as BrightIdeasSoftware.OLVListItem)?.RowObject as RecommendationRow;
            if (rr == null)
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
    }
}
