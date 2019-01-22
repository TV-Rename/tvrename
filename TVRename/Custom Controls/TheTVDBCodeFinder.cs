// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System;
using System.Windows.Forms;
using System.Collections.Generic;

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

        public TheTvdbCodeFinder(string initialHint)
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

        public event EventHandler SelectionChanged;

        public void SetHint(string s)
        {
            mInternal = true;
            txtFindThis.Text = s;
            mInternal = false;
            Search();
            DoFind(true);
        }

        public int SelectedCode()
        {
            try
            {
                if (lvMatches.SelectedItems.Count == 0)
                    return int.Parse(txtFindThis.Text);

                return (int.Parse(lvMatches.SelectedItems[0].SubItems[0].Text));
            }
            catch
            {
                return -1;
            }
        }

        public SeriesInfo SelectedShow()
        {
            try
            {
                if (lvMatches.SelectedItems.Count == 0) return null;

                return ((SeriesInfo)(lvMatches.SelectedItems[0].Tag));
            }
            catch
            {
                return null;
            }
        }
        private void txtFindThis_TextChanged(object sender, EventArgs e)
        {
            if (!mInternal)
                DoFind(false);
        }

        private void DoFind(bool chooseOnlyMatch)
        {
            if (mInternal)
                return;

            lvMatches.BeginUpdate();

            string what = txtFindThis.Text.RemoveDiacritics().RemoveDot().ToLower();

            lvMatches.Items.Clear();
            if (!string.IsNullOrEmpty(what))
            {
                bool numeric = int.TryParse(what, out int matchnum);

                lock (TheTVDB.SERIES_LOCK)
                {
                    foreach (KeyValuePair<int, SeriesInfo> kvp in TheTVDB.Instance.GetSeriesDict())
                    {
                        int num = kvp.Key;
                        string show = kvp.Value.Name.RemoveDiacritics();
                        string s = num + " " + show.RemoveDot();

                        string simpleS = s.ToLower().CompareName();

                        bool numberMatch = numeric && num == matchnum;
                        string searchTerm = what.CompareName();

                        if (numberMatch || (!numeric && (simpleS.Contains(searchTerm))) ||
                            (numeric && show.Contains(what)))
                        {
                            lvMatches.Items.Add(NewLvi(kvp.Value, num, show, numberMatch));
                        }
                    }
                }

                if ((lvMatches.Items.Count == 1) && numeric)
                    lvMatches.Items[0].Selected = true;

                int n = lvMatches.Items.Count;
                txtSearchStatus.Text = "Found " + n + " show" + ((n != 1) ? "s" : "");
            }
            else
                txtSearchStatus.Text = "";

            lvMatches.EndUpdate();

            if ((lvMatches.Items.Count == 1) && chooseOnlyMatch)
                lvMatches.Items[0].Selected = true;
        }

        private static ListViewItem NewLvi(SeriesInfo si, int num, string show, bool numberMatch)
        {
            ListViewItem lvi = new ListViewItem {Text = num.ToString()};
            lvi.SubItems.Add(show);
            lvi.SubItems.Add(si.FirstAired != null ? si.FirstAired.Value.Year.ToString() : "");

            lvi.Tag = si;
            if (numberMatch)
                lvi.Selected = true;
            return lvi;
        }

        private void bnGoSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {
            // search on thetvdb.com site
            txtSearchStatus.Text = "Searching on TheTVDB.com";
            txtSearchStatus.Update();

            if (!string.IsNullOrEmpty(txtFindThis.Text))
            {
                TheTVDB.Instance.Search(txtFindThis.Text);
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

        private void txtFindThis_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
            {
                bnGoSearch_Click(null, null);
                e.Handled = true;
            }
        }

        private void lvMatches_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == 0 || e.Column == 2) // code or year
                lvMatches.ListViewItemSorter = new NumberAsTextSorter(e.Column);
            else
                lvMatches.ListViewItemSorter = new TextSorter(e.Column);
            lvMatches.Sort();
        }
    }
}
