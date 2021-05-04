using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;

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
                ? media==MediaConfiguration.MediaType.movie
                    ? recs.Values.Where(x=> mDoc.FilmLibrary.Movies.All(configuration => configuration.TmdbCode != x.Key))
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
                    MovieConfiguration newMovie = new MovieConfiguration(mlastSelectedKey,TVDoc.ProviderType.TMDB);
                    tvDoc.Add(newMovie );
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
            string languageCode = TVSettings.Instance.TMDBLanguage;
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
                        Logger.Warn($"{rec.Key,-10} | {(rec.Value.TopRated?"Top":"   ")} | {(rec.Value.Trending ? "Trend":"    ")} | {rec.Value.Related.Count,5} | {rec.Value.Similar.Count,5} | {TMDB.LocalCache.Instance.GetMovie(rec.Key)?.IsSearchResultOnly} | {TMDB.LocalCache.Instance.GetMovie(rec.Key)?.Name}");
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
            RecommendationRow? rr = (e.Item as BrightIdeasSoftware.OLVListItem).RowObject as RecommendationRow;

            if (rr.cachedMovieInfo != null)
            {
                UI.SetHtmlBody(chrRecommendationPreview, rr.cachedMovieInfo.GetMovieHtmlOverview());
            } else if (rr.cachedSeriesInfo != null)
            {
                UI.SetHtmlBody(chrRecommendationPreview, rr.cachedSeriesInfo.GetShowHtmlOverview());
            }

        }
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class RecommendationRow
    {
        private readonly RecommendationResult result;
        private readonly MediaConfiguration.MediaType type;
        public readonly CachedSeriesInfo? cachedSeriesInfo;
        public readonly CachedMovieInfo? cachedMovieInfo;
        private readonly CachedMediaInfo? cachedMediaInfo;

        public RecommendationRow(RecommendationResult x, MediaConfiguration.MediaType t)
        {
            result = x;
            type = t;
            switch (t)
            {
                case MediaConfiguration.MediaType.tv:
                    cachedSeriesInfo = TMDB.LocalCache.Instance.GetSeries(x.Key);
                    cachedMediaInfo = cachedSeriesInfo;
                    break;
                case MediaConfiguration.MediaType.movie:
                    cachedMovieInfo = TMDB.LocalCache.Instance.GetMovie(x.Key);
                    cachedMediaInfo = cachedMovieInfo;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(t), t, null);
            }
        }

        public int Key => result.Key;
        public string? Name => cachedMediaInfo?.Name;
        public string? Overview => cachedMediaInfo?.Overview;
        public string? Year => type == MediaConfiguration.MediaType.movie
            ? cachedMovieInfo?.Year.ToString()
            : cachedSeriesInfo?.Year;

        public bool TopRated => result.TopRated;
        public bool Trending => result.Trending;


        //Star score is out of 5 stars, we produce a 'normlised' result by adding a top mark 10/10 and a bottom mark 1/10 and recalculating
        //this is to stop a show with one 10/10 vote looking too good, this normalises it back if the number of votes is small
        public float StarScore => (((cachedMediaInfo?.SiteRatingVotes ?? 1) * (cachedMediaInfo?.SiteRating ?? 5)) + 5) /
                                  ((cachedMediaInfo?.SiteRatingVotes ?? 1) + 1);

        public int RecommendationScore => result.GetScore(20, 20, 2, 1);

        public string Reason => result.Similar.Select(configuration => configuration.ShowName).ToCsv() + "-" + result.Related.Select(configuration => configuration.ShowName).ToCsv();
    }

    public class Recomendations : ConcurrentDictionary<int,RecommendationResult>
    {
       private RecommendationResult Enrich(int key)
        {
            if (TryGetValue(key, out RecommendationResult movieRec))
            {
                return movieRec;
            }

            RecommendationResult x = new RecommendationResult {Key = key};
            TryAdd(key, x);
            return x;
        }

       public void AddTopRated(int key)
       {
            Enrich(key).TopRated = true;
       }

       public void AddTrending(int key)
       {
            Enrich(key).Trending = true;
        }

       public void AddRelated(int key, MediaConfiguration sourceId)
       {
            Enrich(key).Related.Add(sourceId);
       }

       public void AddSimilar(int key, MediaConfiguration sourceId)
       {
            Enrich(key).Similar.Add(sourceId);
        }
    }

    public class RecommendationResult
    {
        internal int Key;
        internal bool Trending;
        internal bool TopRated;
        internal readonly List<MediaConfiguration> Related = new List<MediaConfiguration>();
        internal readonly List<MediaConfiguration> Similar = new List<MediaConfiguration>();

        public int GetScore(int trendingWeight, int topWeight, int relatedWeight, int similarWeight)
        {
            return (Trending ? trendingWeight : 0) + (TopRated ? topWeight : 0) + relatedWeight * Related.Count + similarWeight * Similar.Count;
        }
    }
}
