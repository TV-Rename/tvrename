//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

// Control for searching for a source provider code, checking against local cache and
// searching on various providers

namespace TVRename
{
    public partial class CombinedCodeFinder : UserControl
    {
        private MediaConfiguration.MediaType Type { get; }
        internal TVDoc.ProviderType Source { get; private set; }
        private bool mInternal;
        private readonly ListViewColumnSorter lvwCodeFinderColumnSorter;

        public CachedSeriesInfo? TvShowInitialFound { get; private set; }
        public CachedMovieInfo? MovieInitialFound { get; private set; }
        public int? TvShowInitialFoundCode => TvShowInitialFound?.IdCode(Source);

        private const string DEFAULT_MESSAGE = "Enter the show's name, and click \"Search\"";
        public int? MovieInitialFoundCode => MovieInitialFound?.IdCode(Source);

        public CombinedCodeFinder(string? initialHint, MediaConfiguration.MediaType type, TVDoc.ProviderType source)
        {
            Type = type;
            Source = source;
            mInternal = false;

            InitializeComponent();

            txtFindThis.Text = initialHint;

            SetupColumns();

            if (!initialHint.HasValue())
            {
                ListViewItem lvi = new ListViewItem(string.Empty);
                lvi.SubItems.Add(DEFAULT_MESSAGE);
                lvMatches.Items.Add(lvi);
            }

            lvwCodeFinderColumnSorter = new ListViewColumnSorter(new DoubleAsTextSorter(5))
            {
                Order = SortOrder.Descending
            };

            lvMatches.ListViewItemSorter = lvwCodeFinderColumnSorter;

            label3.Text = GetPromptLabel(Source);
        }

        public void SetSource(TVDoc.ProviderType source) => SetSource(source, null);

        public void SetSource(TVDoc.ProviderType source, MediaConfiguration? mi)
        {
            UpdateSource(source);
            if (txtFindThis.Text.IsNumeric() && mi != null && mi.IdFor(source) > 0)
            {
                mInternal = true;
                txtFindThis.Text = GenerateNewHintForProvider(mi);
                mInternal = false;
                DoFind(false);
            }
            else if (txtFindThis.Text.IsNumeric() && mi != null && mi.Name.HasValue())
            {
                mInternal = true;
                txtFindThis.Text = GenerateNewHintForProvider(mi);
                mInternal = false;
                DoFind(true);
            }
        }

        private void UpdateSource(TVDoc.ProviderType source)
        {
            if (source == TVDoc.ProviderType.libraryDefault)
            {
                Source = Type == MediaConfiguration.MediaType.movie
                    ? TVSettings.Instance.DefaultMovieProvider
                    : TVSettings.Instance.DefaultProvider;
            }
            else
            {
                Source = source;
            }

            label3.Text = GetPromptLabel(Source);
        }

        private string GenerateNewHintForProvider(MediaConfiguration mi)
        {
            if (mi.IdFor(Source) > 0) return mi.IdFor(Source).ToString();
            return mi.ShowName;
        }

        private void SetupColumns()
        {
            lvMatches.Columns.Clear();
            (int width, string name)[] cols;
            switch (Type)
            {
                case MediaConfiguration.MediaType.movie:
                    cols = new (int width, string name)[]
                    {
                        (44, "Code"),
                        (184, "Movie Name"),
                        (39, "Year"),
                        (45, "Rating"),
                        (40, "Language"),
                        (40, "Pop."),
                    };

                    break;

                case MediaConfiguration.MediaType.tv:
                    cols = new (int width, string name)[]
                    {
                        (44, "Code"),
                        (188, "Show Name"),
                        (39, "Year"),
                        (52, "Network"),
                        (58, "Status"),
                        (40, "Pop."),
                    };

                    break;

                case MediaConfiguration.MediaType.both:
                    throw new ArgumentOutOfRangeException();
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach ((int width, var name) in cols)
            {
                lvMatches.Columns.Add(new ColumnHeader { Text = name, Width = width });
            }
        }

        public event EventHandler<EventArgs>? SelectionChanged;

        public bool SetHint(string s, TVDoc.ProviderType provider)
        {
            mInternal = true;
            txtFindThis.Text = s;
            SetSource(provider);
            mInternal = false;
            Search(true);
            return DoFind(true);
        }

        public CachedMovieInfo? SelectedMovie() => (CachedMovieInfo)SelectedObject();

        public CachedSeriesInfo? SelectedShow() => (CachedSeriesInfo)SelectedObject();

        private object? SelectedObject()
        {
            try
            {
                if (lvMatches.SelectedItems.Count == 0)
                {
                    return null;
                }

                return lvMatches.SelectedItems[0].Tag;
            }
            catch
            {
                return null;
            }
        }

        public int SelectedCode()
        {
            try
            {
                return lvMatches.SelectedItems.Count == 0
                    ? int.Parse(txtFindThis.Text)
                    : int.Parse(lvMatches.SelectedItems[0].SubItems[0].Text);
            }
            catch
            {
                return -1;
            }
        }

        private void txtFindThis_TextChanged(object sender, EventArgs e)
        {
            if (!mInternal && txtFindThis.Text.Length > 2)
            {
                DoFind(false);
            }
        }

        private bool DoFind(bool chooseOnlyMatch)
        {
            if (mInternal)
            {
                return false;
            }

            try
            {
                mInternal = true;
                lvMatches.BeginUpdate();

                string what = txtFindThis.Text.CompareName();
                int matchedMovies = 0;
                int matchedTvShows = 0;

                if (!txtFindThis.Text.HasValue() && lvMatches.Items.Count == 1 && lvMatches.Items[0].SubItems[1].Text == DEFAULT_MESSAGE)
                {
                    //we have no further information
                    return false;
                }

                lvMatches.Items.Clear();
                if (!string.IsNullOrEmpty(what))
                {
                    bool numeric = int.TryParse(what, out int matchnum);
                    MediaCache cache = TVDoc.GetMediaCache(Source);

                    if (Type == MediaConfiguration.MediaType.tv)
                    {
                        lock (cache.SERIES_LOCK)
                        {
                            foreach (KeyValuePair<int, CachedSeriesInfo> kvp in cache.CachedShowData.Where(kvp => matches(kvp.Key, kvp.Value, numeric, what, matchnum)))
                            {
                                lvMatches.Items.Add(NewLvi(kvp.Value, kvp.Key, numeric && kvp.Key == matchnum));
                                matchedTvShows++;
                                TvShowInitialFound = kvp.Value;
                            }
                        }
                        txtSearchStatus.Text = "Found " + matchedTvShows + " show" + (matchedTvShows != 1 ? "s" : "");
                    }
                    else if (Type == MediaConfiguration.MediaType.movie)
                    {
                        lock (cache.MOVIE_LOCK)
                        {
                            foreach (KeyValuePair<int, CachedMovieInfo> kvp in cache.CachedMovieData.Where(kvp => matches(kvp.Key, kvp.Value, numeric, what, matchnum)).OrderByDescending(m => m.Value.Popularity))
                            {
                                lvMatches.Items.Add(NewLvi(kvp.Value, kvp.Key, numeric && kvp.Key == matchnum));
                                matchedMovies++;
                                MovieInitialFound = kvp.Value;
                            }
                        }
                        txtSearchStatus.Text = "Found " + matchedMovies + " movie" + (matchedMovies != 1 ? "s" : "");
                    }

                    if (lvMatches.Items.Count == 1 && numeric)
                    {
                        lvMatches.Items[0].Selected = true;
                    }
                }
                else
                {
                    txtSearchStatus.Text = string.Empty;
                }

                if (matchedMovies == 1 && chooseOnlyMatch)
                {
                    lvMatches.Items[0].Selected = true;
                    return true;
                }

                if (matchedTvShows == 1 && chooseOnlyMatch)
                {
                    lvMatches.Items[0].Selected = true;
                    return true;
                }

                return false;
            }
            finally
            {
                lvMatches.EndUpdate();
                mInternal = false;
            }
        }

        private bool matches(int num, CachedMediaInfo kvp, bool numeric, string what, int matchnum)
        {
            string show = kvp.Name.CompareName();

            string s = num + " " + show;
            string simpleWhat = what.CompareName();
            bool textMatch = !numeric && s.Contains(simpleWhat);

            bool numberMatch = numeric && num == matchnum;
            bool numberTextMatch = numeric && show.Contains(what);

            return (numberMatch || textMatch || numberTextMatch);
        }

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

        private void bnGoSearch_Click(object sender, EventArgs e)
        {
            Search(true);
        }

        private void Search(bool showErrorMsgBox)
        {
            // search on site
            txtSearchStatus.Text = GetLabel(Source);
            txtSearchStatus.Update();

            //TODO - make search multi language and use custom language specified

            if (!string.IsNullOrEmpty(txtFindThis.Text))
            {
                GetSourceInstance(Source).Search(txtFindThis.Text, showErrorMsgBox, Type, new Locale());
                DoFind(true);
            }
        }

        private string GetLabel(TVDoc.ProviderType source)
        {
            return source switch
            {
                TVDoc.ProviderType.TMDB => "Searching on TMDB.com",
                TVDoc.ProviderType.TheTVDB => "Searching on TheTVDB.com",
                TVDoc.ProviderType.TVmaze => "Searching on TVmaze.com",
                TVDoc.ProviderType.libraryDefault => GetLabel(DefaultType),
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };
        }

        private TVDoc.ProviderType DefaultType => MediaConfiguration.MediaType.movie == Type
            ? TVSettings.Instance.DefaultMovieProvider
            : TVSettings.Instance.DefaultProvider;

        private string GetPromptLabel(TVDoc.ProviderType source)
        {
            return source switch
            {
                TVDoc.ProviderType.TMDB => "TMDB &code:",
                TVDoc.ProviderType.TheTVDB => "TheTVDB &code:",
                TVDoc.ProviderType.TVmaze => "TVmaze &code:",
                TVDoc.ProviderType.libraryDefault => GetPromptLabel(DefaultType),
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };
        }

        private MediaCache GetSourceInstance(TVDoc.ProviderType source)
        {
            return source switch
            {
                TVDoc.ProviderType.TMDB => TMDB.LocalCache.Instance,
                TVDoc.ProviderType.TheTVDB => TheTVDB.LocalCache.Instance,
                TVDoc.ProviderType.TVmaze => TVmaze.LocalCache.Instance,
                TVDoc.ProviderType.libraryDefault => GetSourceInstance(DefaultType),
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };
        }

        private void lvMatches_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(sender, e);
        }

        public void TakeFocus()
        {
            Focus();
            txtFindThis.Focus();
        }

        private void txtFindThis_KeyDown(object sender, [NotNull] KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                Search(true);
                e.Handled = true;
            }
        }

        private void lvMatches_ColumnClick(object sender, [NotNull] ColumnClickEventArgs e)
        {
            lvwCodeFinderColumnSorter.ClickedOn(e.Column);

            if (e.Column == 0 || e.Column == 2) // code or year
            {
                lvwCodeFinderColumnSorter.ListViewItemSorter = new NumberAsTextSorter(e.Column);
            }
            else if (e.Column == 5) //  popularity
            {
                lvwCodeFinderColumnSorter.ListViewItemSorter = new DoubleAsTextSorter(e.Column);
            }
            else
            {
                lvwCodeFinderColumnSorter.ListViewItemSorter = new TextSorter(e.Column);
            }

            lvMatches.Sort();
        }
    }
}
