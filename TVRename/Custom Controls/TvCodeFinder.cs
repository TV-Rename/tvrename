using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace TVRename
{
    public class TvCodeFinder : CodeFinder
    {
        public TvCodeFinder([CanBeNull] string? initialHint, TVDoc.ProviderType source) : base(initialHint, MediaConfiguration.MediaType.tv, source)
        {
            lvMatches.Columns.Clear();
            lvMatches.Columns.Add(new ColumnHeader { Text = "Code", Width = 44 });
            lvMatches.Columns.Add(new ColumnHeader { Text = "Show Name", Width = 188 });
            lvMatches.Columns.Add(new ColumnHeader { Text = "Year", Width = 39 });
            lvMatches.Columns.Add(new ColumnHeader { Text = "Network", Width = 52 });
            lvMatches.Columns.Add(new ColumnHeader { Text = "Status", Width = 58 });
            lvMatches.Columns.Add(new ColumnHeader { Text = "Pop.", Width = 40 });
        }
        public CachedSeriesInfo? TvShowInitialFound { get; private set; }
        public int? TvShowInitialFoundCode => TvShowInitialFound?.IdCode(Source);

        [NotNull]
        private static ListViewItem NewLvi([NotNull] CachedSeriesInfo si, int num, bool numberMatch)
        {
            ListViewItem lvi = new ListViewItem { Text = num.ToString() };
            lvi.SubItems.Add(si.Name);
            lvi.SubItems.Add(si.Year);
            lvi.SubItems.Add(si.Network ?? string.Empty);
            lvi.SubItems.Add(si.Status);
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
            List<KeyValuePair<int, CachedSeriesInfo>> lvis;
            lock (cache.SERIES_LOCK)
            {
                lvis = cache.CachedShowData
                    .Where(kvp => Matches(kvp.Key, kvp.Value, numeric, what, matchnum))
                    .ToList();
            }
            foreach (var lvi in lvis.Select(kvp => NewLvi(kvp.Value, kvp.Key, numeric && kvp.Key == matchnum)))
            {
                lvMatches.Items.Add(lvi);
            }

            int matchedTvShows = lvis.Count;
            TvShowInitialFound = lvis.FirstOrDefault().Value;
            txtSearchStatus.Text = "Found " + matchedTvShows + " show" + (matchedTvShows != 1 ? "s" : "");
            return matchedTvShows;
        }
    }
}
