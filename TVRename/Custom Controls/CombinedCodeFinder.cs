// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using JetBrains.Annotations;

// Control for searching for a tvdb code, checking against local cache and
// searching on thetvdb

namespace TVRename
{
    public partial class CombinedCodeFinder : UserControl
    {
        private MediaConfiguration.MediaType Type { get; }
        private TVDoc.ProviderType Source { get; }
        private bool mInternal;

        public CachedSeriesInfo tvShowInitialFound { get; private set; }
        public CachedMovieInfo movieInitialFound{ get; private set; }
        public CombinedCodeFinder(string? initialHint, MediaConfiguration.MediaType type, TVDoc.ProviderType source)
        {
            Type = type;
            Source = source;
            mInternal = false;

            InitializeComponent();

            txtFindThis.Text = initialHint;

            if (string.IsNullOrEmpty(initialHint))
            {
                ListViewItem lvi = new ListViewItem("");
                lvi.SubItems.Add("Enter the show's name, and click \"Search\"");
                lvMatches.Items.Add(lvi);
            }

            SetupColumns();
            label3.Text = GetPromptLabel(Source);
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
                    };

                    break;
                case MediaConfiguration.MediaType.both:
                    throw new ArgumentOutOfRangeException();
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach ((int width, string name) col in cols)
            {
                lvMatches.Columns.Add(new ColumnHeader {Text = col.name, Width = col.width});
            }
        }

        public  event EventHandler<EventArgs>? SelectionChanged;

        public  bool SetHint(string s)
        {
            mInternal = true;
            txtFindThis.Text = s;
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

                return  lvMatches.SelectedItems[0].Tag;
            }
            catch
            {
                return null;
            }
        }

        public  int SelectedCode()
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
            if (!mInternal)
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

            lvMatches.BeginUpdate();

            string what = txtFindThis.Text.CompareName();
            int matchedMovies = 0;
            int matchedTVShows = 0;

            lvMatches.Items.Clear();
            if (!string.IsNullOrEmpty(what))
            {
                bool numeric = int.TryParse(what, out int matchnum);

                if (Type == MediaConfiguration.MediaType.tv)
                {
                    lock (TheTVDB.LocalCache.Instance.SERIES_LOCK)
                    {
                        foreach (KeyValuePair<int, CachedSeriesInfo> kvp in TheTVDB.LocalCache.Instance.GetSeriesDict())
                        {
                            if (matches(kvp.Key, kvp.Value, numeric, what, matchnum))
                            {
                                lvMatches.Items.Add(NewLvi((CachedSeriesInfo)(kvp.Value), kvp.Key, numeric && kvp.Key == matchnum));
                                matchedTVShows++;
                                tvShowInitialFound = kvp.Value;
                            }
                        }
                    }
                    txtSearchStatus.Text = "Found " + matchedTVShows + " show" + (matchedTVShows != 1 ? "s" : "");
                }
                else if (Type == MediaConfiguration.MediaType.movie)
                {
                    lock (TMDB.LocalCache.Instance.MOVIE_LOCK)
                    {
                        foreach (KeyValuePair<int, CachedMovieInfo> kvp in TMDB.LocalCache.Instance.CachedMovieData)
                        {
                            if (matches(kvp.Key, kvp.Value, numeric, what, matchnum))
                            {
                                lvMatches.Items.Add(NewLvi((CachedMovieInfo)(kvp.Value), kvp.Key, numeric && kvp.Key == matchnum));
                                matchedMovies++;
                                movieInitialFound = kvp.Value;
                            }
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

            lvMatches.EndUpdate();

            if (matchedMovies == 1 && chooseOnlyMatch)
            {
                lvMatches.Items[0].Selected = true;
                
                return true;
            }

            if (matchedTVShows == 1 && chooseOnlyMatch)
            {
                lvMatches.Items[0].Selected = true;

                return true;
            }
            return false;
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
            ListViewItem lvi = new ListViewItem {Text = num.ToString()};
            lvi.SubItems.Add(si.Name);
            lvi.SubItems.Add(si.Year);
            lvi.SubItems.Add(si.Network ?? string.Empty);
            lvi.SubItems.Add(si.Status);

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

            if (!string.IsNullOrEmpty(txtFindThis.Text))
            {
                GetSourceInstance(Source).Search(txtFindThis.Text,showErrorMsgBox);
                DoFind(true);
            }
        }

        private static string GetLabel(TVDoc.ProviderType source)
        {
            return source switch
            {
                TVDoc.ProviderType.TMDB => "Searching on TMDB.com",
                TVDoc.ProviderType.TheTVDB => "Searching on TheTVDB.com",
                TVDoc.ProviderType.TVmaze => "Searching on TVmaze.com",
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };
        }
        private static string GetPromptLabel(TVDoc.ProviderType source)
        {
            return source switch
            {
                TVDoc.ProviderType.TMDB => "TMDB &code:",
                TVDoc.ProviderType.TheTVDB => "TheTVDB &code:",
                TVDoc.ProviderType.TVmaze => "TVmaze &code:",
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };
        }
        private static MediaCache GetSourceInstance(TVDoc.ProviderType source)
        {
            return source switch
            {
                TVDoc.ProviderType.TMDB => TMDB.LocalCache.Instance,
                TVDoc.ProviderType.TheTVDB => TheTVDB.LocalCache.Instance,
                TVDoc.ProviderType.TVmaze => TVmaze.LocalCache.Instance,
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
            if (e.Column == 0 || e.Column == 2) // code or year
            {
                lvMatches.ListViewItemSorter = new NumberAsTextSorter(e.Column);
            }
            else
            {
                lvMatches.ListViewItemSorter = new TextSorter(e.Column);
            }

            lvMatches.Sort();
        }
    }
}
