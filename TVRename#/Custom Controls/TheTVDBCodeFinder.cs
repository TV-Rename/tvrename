// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections.Generic;

// Control for searching for a tvdb code, checking against local cache and
// searching on thetvdb

namespace TVRename
{
    /// <summary>
    /// Summary for TheTVDBCodeFinder
    /// </summary>
    public partial class TheTVDBCodeFinder : UserControl
    {
        private bool mInternal;

        public TheTVDBCodeFinder(string initialHint)
        {
            this.mInternal = false;

            this.InitializeComponent();

            this.txtFindThis.Text = initialHint;

            if (string.IsNullOrEmpty(initialHint))
            {
                ListViewItem lvi = new ListViewItem("");
                lvi.SubItems.Add("Enter the show's name, and click \"Search\"");
                this.lvMatches.Items.Add(lvi);
            }
        }

        public event EventHandler SelectionChanged;

        public void SetHint(string s)
        {
            this.mInternal = true;
            this.txtFindThis.Text = s;
            this.mInternal = false;
            this.DoFind(true);
        }

        public int SelectedCode()
        {
            try
            {
                if (this.lvMatches.SelectedItems.Count == 0)
                    return int.Parse(this.txtFindThis.Text);

                return (int) (this.lvMatches.SelectedItems[0].Tag);
            }
            catch
            {
                return -1;
            }
        }

        private void txtFindThis_TextChanged(object sender, EventArgs e)
        {
            if (!this.mInternal)
                this.DoFind(false);
        }

        private void DoFind(bool chooseOnlyMatch)
        {
            if (this.mInternal)
                return;

            this.lvMatches.BeginUpdate();

            string what = this.txtFindThis.Text;
            what = Helpers.RemoveDiacritics(what);
            what = what.Replace(".", " ");

            this.lvMatches.Items.Clear();
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
                    string s = num + " " + show.Replace(".", " "); 

                    string simpleS = Regex.Replace(s.ToLower(), "[^\\w ]", "");

                    bool numberMatch = numeric && num == matchnum;
                    string searchTerm = Regex.Replace(what, "[^\\w ]", "");
                    if (numberMatch || (!numeric && (simpleS.Contains(searchTerm))) || (numeric && show.Contains(what)))
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = num.ToString();
                        lvi.SubItems.Add(show);
                        lvi.SubItems.Add(kvp.Value.FirstAired != null ? kvp.Value.FirstAired.Value.Year.ToString() : "");

                        lvi.Tag = num;
                        if (numberMatch)
                            lvi.Selected = true;
                        this.lvMatches.Items.Add(lvi);
                    }
                }
                TheTVDB.Instance.Unlock("DoFind");

                if ((this.lvMatches.Items.Count == 1) && numeric)
                    this.lvMatches.Items[0].Selected = true;

                int n = this.lvMatches.Items.Count;
                this.txtSearchStatus.Text = "Found " + n + " show" + ((n != 1) ? "s" : "");
            }
            else
                this.txtSearchStatus.Text = "";

            this.lvMatches.EndUpdate();

            if ((this.lvMatches.Items.Count == 1) && chooseOnlyMatch)
                this.lvMatches.Items[0].Selected = true;
        }

        private void bnGoSearch_Click(object sender, EventArgs e)
        {
            // search on thetvdb.com site
            this.txtSearchStatus.Text = "Searching on TheTVDB.com";
            this.txtSearchStatus.Update();

            //String ^url = "http://www.tv.com/search.php?stype=program&qs="+txtFindThis->Text+"&type=11&stype=search&tag=search%3Bbutton";

            if (!String.IsNullOrEmpty(this.txtFindThis.Text))
            {
                TheTVDB.Instance.Search(this.txtFindThis.Text);
                this.DoFind(true);
            }
        }

        private void lvMatches_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SelectionChanged != null)
                this.SelectionChanged(sender, e);
        }

        public void TakeFocus()
        {
            this.Focus();
            this.txtFindThis.Focus();
        }

        private void txtFindThis_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
            {
                this.bnGoSearch_Click(null, null);
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