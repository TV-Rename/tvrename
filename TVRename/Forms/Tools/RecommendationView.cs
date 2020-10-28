using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
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
        private MediaConfiguration.MediaType media;
        private IEnumerable<ShowConfiguration> tvShows;
        private IEnumerable<MovieConfiguration> movies;
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
                    movies = new List<MovieConfiguration>();
                    break;
                case MediaConfiguration.MediaType.movie:
                    movies = doc.FilmLibrary.Movies;
                    tvShows = new List<ShowConfiguration>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            Scan();
        }

        public RecommendationView([NotNull] TVDoc doc, UI main, IEnumerable<MovieConfiguration> m) : this(doc, main)
        {
            media = MediaConfiguration.MediaType.movie;
            tvShows = new List<ShowConfiguration>();
            movies = m;

            Scan();
        }

        public RecommendationView([NotNull] TVDoc doc, UI main, IEnumerable<ShowConfiguration> s) : this(doc, main)
        {
            media = MediaConfiguration.MediaType.tv;
            tvShows = s;
            movies = new List<MovieConfiguration>();
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
                    ? recs.Values.Where(x=> !mDoc.FilmLibrary.ContainsKey(x.Key))
                    : recs.Values.Where(x => !mDoc.TvLibrary.ContainsKey(x.Key))
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
                    var show = new ShowConfiguration(mlastSelectedKey, TVDoc.ProviderType.TMDB);
                    tvDoc.Add(show);
                    break;
                case MediaConfiguration.MediaType.movie:
                    var newMovie = new MovieConfiguration(mlastSelectedKey,TVDoc.ProviderType.TMDB);
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
            switch (media)
            {
                case MediaConfiguration.MediaType.tv:
                    recs = TMDB.LocalCache.Instance.GetRecommendations(mDoc, (BackgroundWorker)sender, tvShows.ToList()).Result;
                    foreach (KeyValuePair<int, RecommendationResult> rec in recs)
                    {
                        Logger.Warn($"{rec.Key,-10} | {(rec.Value.TopRated ? "Top" : "   ")} | {(rec.Value.Trending ? "Trend" : "    ")} | {rec.Value.Related.Count,5} | {rec.Value.Similar.Count,5} | {mDoc.TvLibrary.ContainsKey(rec.Key)} | {TMDB.LocalCache.Instance.GetSeries(rec.Key)?.Name}");
                    }
                    break;
                case MediaConfiguration.MediaType.movie:
                    recs = TMDB.LocalCache.Instance.GetRecommendations(mDoc, (BackgroundWorker)sender, movies.ToList()).Result;
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
            //AddRcMenuItem("Force Refresh", (o, args) => mainUi.ForceRefresh(new List<ShowConfiguration> {si}, false));
            //AddRcMenuItem("Edit Show", (o, args) => mainUi.EditShow(si));
        }
    }

    public class RecommendationRow
    {
        RecommendationResult result;
        private MediaConfiguration.MediaType type;
        private CachedSeriesInfo? cachedSeriesInfo;
        private CachedMovieInfo? cachedMovieInfo;
        private CachedMediaInfo? cachedMediaInfo;

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
        public string? Year => type == MediaConfiguration.MediaType.movie
            ? cachedMovieInfo?.Year.ToString()
            : cachedSeriesInfo?.Year;

        public bool TopRated => result.TopRated;
        public bool Trending => result.Trending;

        public float StarScore => (cachedMediaInfo?.SiteRatingVotes ?? 1 * cachedMediaInfo?.SiteRating ?? 1 + 3) /
                                  (cachedMediaInfo?.SiteRatingVotes ?? 1 + 1);

        public int RecommendationScore => result.GetScore(20, 20, 2, 1);

        public string Reason => result.Similar.Select(configuration => configuration.ShowName).ToCsv() + "-" + result.Related.Select(configuration => configuration.ShowName).ToCsv();

        //lvi.SubItems.Add(rec.Value.TopRated? "Top" : "   ");
        //lvi.SubItems.Add(rec.Value.Trending? "Trend" : "    ");
        //lvi.SubItems.Add(rec.Value.Related.Count.ToString());
        //lvi.SubItems.Add(rec.Value.Similar.Count.ToString());
        //lvi.SubItems.Add(mDoc.TvLibrary.ContainsKey(rec.Key).ToString());
        //lvi.SubItems.Add(cachedMovieInfo?.IsSearchResultOnly.ToString());
    }

    public class Recomendations : ConcurrentDictionary<int,RecommendationResult>
    {
       private RecommendationResult Enrich(int key)
        {
            if (TryGetValue(key, out RecommendationResult movieRec))
            {
                return movieRec;
            }

            RecommendationResult x = new RecommendationResult();
            x.Key = key;
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
        internal List<MediaConfiguration> Related = new List<MediaConfiguration>();
        internal List<MediaConfiguration> Similar = new List<MediaConfiguration>();

        public int GetScore(int trendingWeight, int topWeight, int relatedWeight, int similarWeight)
        {
            return (Trending ? trendingWeight : 0) + (TopRated ? topWeight : 0) + relatedWeight * Related.Count + similarWeight * Similar.Count;
        }
    }
}
