using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace TVRename
{
    public class MovieCodeFinder : CodeFinder
    {
        public MovieCodeFinder([CanBeNull] string? initialHint, TVDoc.ProviderType source) : base(initialHint, MediaConfiguration.MediaType.tv, source)
        {
            lvMatches.Columns.Clear();
            lvMatches.Columns.Add(new ColumnHeader { Text = "Code", Width = 44 });
            lvMatches.Columns.Add(new ColumnHeader { Text = "Show Name", Width = 188 });
            lvMatches.Columns.Add(new ColumnHeader { Text = "Year", Width = 39 });
            lvMatches.Columns.Add(new ColumnHeader { Text = "Network", Width = 52 });
            lvMatches.Columns.Add(new ColumnHeader { Text = "Status", Width = 58 });
            lvMatches.Columns.Add(new ColumnHeader { Text = "Pop.", Width = 40 });
        }
        public CachedMovieInfo? MovieInitialFound { get; private set; }
        public int? MovieInitialFoundCode => MovieInitialFound?.IdCode(Source);
        [NotNull]
        private static ListViewItem NewLvi([NotNull] CachedMovieInfo si, int num, bool numberMatch)
        {
            ListViewItem lvi = new ListViewItem { Text = num.ToString() };
            lvi.SubItems.Add(si.Name);
            lvi.SubItems.Add(si.FirstAired.HasValue ? si.FirstAired.Value.Year.ToString() : string.Empty);
            lvi.SubItems.Add(si.ContentRating);
            lvi.SubItems.Add(si.ShowLanguage);
            lvi.SubItems.Add(si.Popularity.HasValue ? si.Popularity.Value.ToString("0.##") : string.Empty);
            lvi.ToolTipText = si.Overview;
            lvi.Tag = si;
            if (numberMatch)
            {
                lvi.Selected = true;
            }
            return lvi;
        }

        protected override int FindMedia(MediaCache cache, bool numeric, int matchnum, string what)
        {
            List<KeyValuePair<int, CachedMovieInfo>> lvis;
            lock (cache.MOVIE_LOCK)
            {
                lvis = cache.CachedMovieData
                    .Where(kvp => Matches(kvp.Key, kvp.Value, numeric, what, matchnum))
                    .OrderByDescending(m => m.Value.Popularity)
                    .ToList();
            }
            foreach (var lvi in lvis.Select(kvp => NewLvi(kvp.Value, kvp.Key, numeric && kvp.Key == matchnum)))
            {
                lvMatches.Items.Add(lvi);
            }

            int matchedMovies = lvis.Count;
            MovieInitialFound = lvis.FirstOrDefault().Value;
            txtSearchStatus.Text = "Found " + matchedMovies + " movie" + (matchedMovies != 1 ? "s" : "");
            return matchedMovies;
        }
    }
}
