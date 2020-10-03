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
    /// <inheritdoc />
    /// <summary>
    /// Summary for TheTVDBCodeFinder
    /// </summary>
    public partial class TheTvdbCodeFinder : UserControl
    {
        private bool mInternal;

        public TheTvdbCodeFinder(string? initialHint)
        {
            mInternal = false;

            InitializeComponent();

            txtFindThis.Text = initialHint;

            if (string.IsNullOrEmpty(initialHint))
            {
                ListViewItem lvi = new ListViewItem("");
                lvi.SubItems.Add("Enter the show's name, and click \"Search\"");
                lvMatches.Items.Add(lvi);
            }
        }

        public event EventHandler? SelectionChanged;

        public void SetHint(string s)
        {
            mInternal = true;
            txtFindThis.Text = s;
            mInternal = false;
            Search(true);
            DoFind(true);
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

        public CachedSeriesInfo? SelectedShow()
        {
            try
            {
                if (lvMatches.SelectedItems.Count == 0)
                {
                    return null;
                }

                return (CachedSeriesInfo)lvMatches.SelectedItems[0].Tag;
            }
            catch
            {
                return null;
            }
        }
        private void txtFindThis_TextChanged(object sender, EventArgs e)
        {
            if (!mInternal)
            {
                DoFind(false);
            }
        }

        private void DoFind(bool chooseOnlyMatch)
        {
            if (mInternal)
            {
                return;
            }

            lvMatches.BeginUpdate();

            string what = txtFindThis.Text.RemoveDiacritics().RemoveDot().ToLower();

            lvMatches.Items.Clear();
            if (!string.IsNullOrEmpty(what))
            {
                bool numeric = int.TryParse(what, out int matchnum);

                lock (TheTVDB.LocalCache.Instance.SERIES_LOCK)
                {
                    foreach (KeyValuePair<int, CachedSeriesInfo> kvp in TheTVDB.LocalCache.Instance.GetSeriesDict())
                    {
                        int num = kvp.Key;
                        string show = kvp.Value.Name.RemoveDiacritics();

                        string s = num + " " + show.RemoveDot();
                        string simpleS = s.ToLower().CompareName();
                        bool textMatch = !numeric && simpleS.Contains(what.CompareName());

                        bool numberMatch = numeric && num == matchnum;
                        bool numberTextMatch = numeric && show.Contains(what);

                        if (numberMatch || textMatch ||numberTextMatch)
                        {
                            lvMatches.Items.Add(NewLvi(kvp.Value, num, show, numberMatch));
                        }
                    }
                }

                if (lvMatches.Items.Count == 1 && numeric)
                {
                    lvMatches.Items[0].Selected = true;
                }

                int n = lvMatches.Items.Count;
                txtSearchStatus.Text = "Found " + n + " show" + (n != 1 ? "s" : "");
            }
            else
            {
                txtSearchStatus.Text = string.Empty;
            }

            lvMatches.EndUpdate();

            if (lvMatches.Items.Count == 1 && chooseOnlyMatch)
            {
                lvMatches.Items[0].Selected = true;
            }
        }

        [NotNull]
        private static ListViewItem NewLvi([NotNull] CachedSeriesInfo si, int num, string show, bool numberMatch)
        {
            ListViewItem lvi = new ListViewItem {Text = num.ToString()};
            lvi.SubItems.Add(show);
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

        private void bnGoSearch_Click(object sender, EventArgs e)
        {
            Search(true);
        }

        private void Search(bool showErrorMsgBox)
        {
            // search on thetvdb.com site
            txtSearchStatus.Text = "Searching on TheTVDB.com";
            txtSearchStatus.Update();

            if (!string.IsNullOrEmpty(txtFindThis.Text))
            {
                TheTVDB.LocalCache.Instance.Search(txtFindThis.Text,showErrorMsgBox); //todo enable search on TV Maze too
                DoFind(true);
            }
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
