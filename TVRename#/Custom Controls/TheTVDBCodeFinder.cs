// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

// Control for searching for a tvdb code, checking against local cache and
// searching on thetvdb

namespace TVRename
{
    /// <summary>
    /// Summary for TheTVDBCodeFinder
    /// </summary>
    public partial class TheTVDBCodeFinder : UserControl
    {
        private bool _mInternal;

        public TheTVDBCodeFinder(string initialHint)
        {
            _mInternal = false;

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
            _mInternal = true;
            txtFindThis.Text = s;
            _mInternal = false;
            DoFind(true);
        }

        public int SelectedCode()
        {
            try
            {
                if (lvMatches.SelectedItems.Count == 0)
                    return int.Parse(txtFindThis.Text);

                return (int) (lvMatches.SelectedItems[0].Tag);
            }
            catch
            {
                return -1;
            }
        }

        private void txtFindThis_TextChanged(object sender, EventArgs e)
        {
            if (!_mInternal)
                DoFind(false);
        }

        private void DoFind(bool chooseOnlyMatch)
        {
            if (_mInternal)
                return;

            lvMatches.BeginUpdate();

            string what = txtFindThis.Text;
            what = Helpers.RemoveDiacritics(what);
            what = what.Replace(".", " ");

            lvMatches.Items.Clear();
            if (!string.IsNullOrEmpty(what))
            {
                what = what.ToLower();

                bool numeric = Regex.Match(what, "^[0-9]+$").Success;
                int matchnum = 0;
                try
                {
                    matchnum = numeric ? int.Parse(what) : -1;
                }
                catch (OverflowException)
                {
                }

                what = Helpers.RemoveDiacritics(what);

                if (!TheTVDB.Instance.GetLock("DoFind"))
                    return;

                foreach (KeyValuePair<int, SeriesInfo> kvp in TheTVDB.Instance.GetSeriesDict())
                {
                    int num = kvp.Key;
                    string show = kvp.Value.Name;
                    show = Helpers.RemoveDiacritics(show);
                    string s = num + " " + show;

                    string simpleS = Regex.Replace(s.ToLower(), "[^\\w ]", "");

                    bool numberMatch = numeric && num == matchnum;

                    if (numberMatch || (!numeric && (simpleS.Contains(Regex.Replace(what, "[^\\w ]", "")))) || (numeric && show.Contains(what)))
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = num.ToString();
                        lvi.SubItems.Add(show);
                        lvi.SubItems.Add(kvp.Value.FirstAired != null ? kvp.Value.FirstAired.Value.Year.ToString() : "");

                        lvi.Tag = num;
                        if (numberMatch)
                            lvi.Selected = true;
                        lvMatches.Items.Add(lvi);
                    }
                }
                TheTVDB.Instance.Unlock("DoFind");

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

        private void bnGoSearch_Click(object sender, EventArgs e)
        {
            // search on thetvdb.com site
            txtSearchStatus.Text = "Searching on TheTVDB.com";
            txtSearchStatus.Update();

            //String ^url = "http://www.tv.com/search.php?stype=program&qs="+txtFindThis->Text+"&type=11&stype=search&tag=search%3Bbutton";

            if (!String.IsNullOrEmpty(txtFindThis.Text))
            {
                TheTVDB.Instance.Search(txtFindThis.Text);
                DoFind(true);
            }
        }

        private void lvMatches_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(sender, e);
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
