// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
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
        private TheTVDB mTVDB;

        public TheTVDBCodeFinder(string initialHint, TheTVDB db)
        {
            this.mInternal = false;
            this.mTVDB = db;

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

                this.mTVDB.GetLock("DoFind");
                foreach (KeyValuePair<int, SeriesInfo> kvp in this.mTVDB.GetSeriesDict())
                {
                    int num = kvp.Key;
                    string show = kvp.Value.Name;
                    string s = num + " " + show;

                    string simpleS = Regex.Replace(s.ToLower(), "[^\\w ]", "");

                    bool numberMatch = numeric && num == matchnum;

                    if (numberMatch || (!numeric && (simpleS.Contains(Regex.Replace(what, "[^\\w ]", "")))) || (numeric && show.Contains(what)))
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = num.ToString();
                        lvi.SubItems.Add(show);
                        if (kvp.Value.FirstAired != null)
                            lvi.SubItems.Add(kvp.Value.FirstAired.Value.Year.ToString());
                        else
                            lvi.SubItems.Add("");

                        lvi.Tag = num;
                        if (numberMatch)
                            lvi.Selected = true;
                        this.lvMatches.Items.Add(lvi);
                    }
                }
                this.mTVDB.Unlock("DoFind");

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
                this.mTVDB.Search(this.txtFindThis.Text);

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
    }
}