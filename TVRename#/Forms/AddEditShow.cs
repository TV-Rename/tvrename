// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

//            this->txtCustomShowName->TextChanged += gcnew System::EventHandler(this, &AddEditShow::txtCustomShowName_TextChanged);

namespace TVRename
{
    /// <summary>
    /// Summary for AddEditShow
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class AddEditShow : Form
    {
        public string ShowTimeZone;
        private ShowItem mSI;
        private TheTVDBCodeFinder mTCCF;

        public AddEditShow(ShowItem si)
        {
            this.mSI = si;
            this.InitializeComponent();

            this.cbTimeZone.BeginUpdate();
            this.cbTimeZone.Items.Clear();

            foreach (string s in TimeZone.ZoneNames())
                this.cbTimeZone.Items.Add(s);

            this.cbTimeZone.EndUpdate();

            this.mTCCF = new TheTVDBCodeFinder(si.TVDBCode != -1 ? si.TVDBCode.ToString() : "");
            this.mTCCF.Dock = DockStyle.Fill;
            //mTCCF->SelectionChanged += gcnew System::EventHandler(this, &AddEditShow::lvMatches_ItemSelectionChanged);

            this.pnlCF.SuspendLayout();
            this.pnlCF.Controls.Add(this.mTCCF);
            this.pnlCF.ResumeLayout();

            this.chkCustomShowName.Checked = si.UseCustomShowName;
            if (this.chkCustomShowName.Checked)
                this.txtCustomShowName.Text = si.CustomShowName;
            this.chkCustomShowName_CheckedChanged(null, null);

            this.cbSequentialMatching.Checked = si.UseSequentialMatch;
            this.chkShowNextAirdate.Checked = si.ShowNextAirdate;
            this.chkSpecialsCount.Checked = si.CountSpecials;
            this.chkFolderPerSeason.Checked = si.AutoAdd_FolderPerSeason;
            this.txtSeasonFolderName.Text = si.AutoAdd_SeasonFolderName;
            this.txtBaseFolder.Text = si.AutoAdd_FolderBase;
            this.chkAutoFolders.Checked = si.AutoAddNewSeasons;
            this.chkFolderPerSeason_CheckedChanged(null, null);

            this.cbDoRenaming.Checked = si.DoRename;
            this.cbDoMissingCheck.Checked = si.DoMissingCheck;
            this.cbDoMissingCheck_CheckedChanged(null, null);

            this.chkPadTwoDigits.Checked = si.PadSeasonToTwoDigits;

            this.ShowTimeZone = ((si == null) || (si.TheSeries() == null))
                                    ? TimeZone.DefaultTimeZone()
                                    : si.TheSeries().ShowTimeZone;

            this.cbTimeZone.Text = this.ShowTimeZone;
            this.chkDVDOrder.Checked = si.DVDOrder;
            this.cbIncludeFuture.Checked = si.ForceCheckFuture;
            this.cbIncludeNoAirdate.Checked = si.ForceCheckNoAirdate;

            bool first = true;
            si.IgnoreSeasons.Sort();
            foreach (int i in si.IgnoreSeasons)
            {
                if (!first)
                    this.txtIgnoreSeasons.Text += " ";
                this.txtIgnoreSeasons.Text += i.ToString();
                first = false;
            }

            foreach (System.Collections.Generic.KeyValuePair<int, List<string>> kvp in si.ManualFolderLocations)
            {
                foreach (string s in kvp.Value)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = kvp.Key.ToString();
                    lvi.SubItems.Add(s);

                    this.lvSeasonFolders.Items.Add(lvi);
                }
            }
            this.lvSeasonFolders.Sort();

            this.txtSeasonNumber_TextChanged(null, null);
            this.txtFolder_TextChanged(null, null);

            this.ActiveControl = mTCCF; // set initial focus to the code entry/show finder control

            foreach (string aliasName in this.mSI.AliasNames)
            {
                lbShowAlias.Items.Add(aliasName);
            }

            StringBuilder tl = new StringBuilder();

            foreach (string s in CustomName.Tags)
            {
                tl.AppendLine(s);
            }
            this.txtTagList.Text = tl.ToString();

            cbUseCustomSearch.Checked = !String.IsNullOrEmpty(si.CustomSearchURL);
            txtSearchURL.Text = si.CustomSearchURL ?? "";
            EnableDisableCustomSearch();
        }

        private void buttonOK_Click(object sender, System.EventArgs e)
        {
            if (!this.OKToClose())
            {
                this.DialogResult = DialogResult.None;
                return;
            }

            this.SetmSI();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private bool OKToClose()
        {
            if (!TheTVDB.Instance.HasSeries(this.mTCCF.SelectedCode()))
            {
                DialogResult dr = MessageBox.Show("tvdb code unknown, close anyway?", "TVRename Add/Edit Show",
                                                  MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.No)
                    return false;
            }

            return true;
        }

        private void SetmSI()
        {
            int code = this.mTCCF.SelectedCode();

            string tz = (this.cbTimeZone.SelectedItem != null) ? this.cbTimeZone.SelectedItem.ToString() : "";

            this.mSI.CustomShowName = this.txtCustomShowName.Text;
            this.mSI.UseCustomShowName = this.chkCustomShowName.Checked;
            this.ShowTimeZone = tz; //TODO: move to somewhere else. make timezone manager for tvdb?
            this.mSI.ShowNextAirdate = this.chkShowNextAirdate.Checked;
            this.mSI.PadSeasonToTwoDigits = this.chkPadTwoDigits.Checked;
            this.mSI.TVDBCode = code;
            //todo mSI->SeasonNumber = seasnum;
            this.mSI.CountSpecials = this.chkSpecialsCount.Checked;
            //                                 mSI->Rules = mWorkingRuleSet;  // TODO
            this.mSI.DoRename = this.cbDoRenaming.Checked;
            this.mSI.DoMissingCheck = this.cbDoMissingCheck.Checked;

            this.mSI.AutoAddNewSeasons = this.chkAutoFolders.Checked;
            this.mSI.AutoAdd_FolderPerSeason = this.chkFolderPerSeason.Checked;
            this.mSI.AutoAdd_SeasonFolderName = this.txtSeasonFolderName.Text;
            this.mSI.AutoAdd_FolderBase = this.txtBaseFolder.Text;

            this.mSI.DVDOrder = this.chkDVDOrder.Checked;
            this.mSI.ForceCheckFuture = this.cbIncludeFuture.Checked;
            this.mSI.ForceCheckNoAirdate = this.cbIncludeNoAirdate.Checked;
            this.mSI.CustomSearchURL = this.txtSearchURL.Text;

            this.mSI.UseSequentialMatch = this.cbSequentialMatching.Checked;

            string slist = this.txtIgnoreSeasons.Text;
            this.mSI.IgnoreSeasons.Clear();
            foreach (Match match in Regex.Matches(slist, "\\b[0-9]+\\b"))
                this.mSI.IgnoreSeasons.Add(int.Parse(match.Value));

            this.mSI.ManualFolderLocations.Clear();
            foreach (ListViewItem lvi in this.lvSeasonFolders.Items)
            {
                try
                {
                    int seas = int.Parse(lvi.Text);
                    if (!this.mSI.ManualFolderLocations.ContainsKey(seas))
                        this.mSI.ManualFolderLocations.Add(seas, new List<String>());

                    this.mSI.ManualFolderLocations[seas].Add(lvi.SubItems[1].Text);
                }
                catch
                {
                }
            }

            this.mSI.AliasNames.Clear();
            foreach (string showAlias in this.lbShowAlias.Items)
            {
                if (!this.mSI.AliasNames.Contains(showAlias))
                {
                    this.mSI.AliasNames.Add(showAlias);
                }
            }
        }

        private void bnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void chkFolderPerSeason_CheckedChanged(object sender, System.EventArgs e)
        {
            this.txtSeasonFolderName.Enabled = this.chkFolderPerSeason.Checked;
        }

        private void bnBrowse_Click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtBaseFolder.Text))
                this.folderBrowser.SelectedPath = this.txtBaseFolder.Text;

            if (this.folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txtBaseFolder.Text = this.folderBrowser.SelectedPath;
        }

        private void cbDoMissingCheck_CheckedChanged(object sender, System.EventArgs e)
        {
            this.cbIncludeNoAirdate.Enabled = this.cbDoMissingCheck.Checked;
            this.cbIncludeFuture.Enabled = this.cbDoMissingCheck.Checked;
        }

        private void bnRemove_Click(object sender, System.EventArgs e)
        {
            if (this.lvSeasonFolders.SelectedItems.Count > 0)
            {
                foreach (ListViewItem lvi in this.lvSeasonFolders.SelectedItems)
                    this.lvSeasonFolders.Items.Remove(lvi);
            }
        }

        private void bnAdd_Click(object sender, System.EventArgs e)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = this.txtSeasonNumber.Text;
            lvi.SubItems.Add(this.txtFolder.Text);

            this.lvSeasonFolders.Items.Add(lvi);

            this.txtSeasonNumber.Text = "";
            this.txtFolder.Text = "";

            this.lvSeasonFolders.Sort();
        }

        private void bnBrowseFolder_Click(object sender, System.EventArgs e)
        {
            this.folderBrowser.SelectedPath = this.txtFolder.Text;
            if (this.folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txtFolder.Text = this.folderBrowser.SelectedPath;
        }

        private void txtSeasonNumber_TextChanged(object sender, System.EventArgs e)
        {
            bool isNumber = Regex.Match(this.txtSeasonNumber.Text, "^[0-9]+$").Success;
            this.bnAdd.Enabled = isNumber && (!string.IsNullOrEmpty(this.txtSeasonNumber.Text));
        }

        private void txtFolder_TextChanged(object sender, System.EventArgs e)
        {
            bool ok = true;
            if (!string.IsNullOrEmpty(this.txtFolder.Text))
            {
                try
                {
                    ok = System.IO.Directory.Exists(this.txtFolder.Text);
                }
                catch
                {
                }
            }
            this.txtFolder.BackColor = ok ? System.Drawing.SystemColors.Window : Helpers.WarningColor();
        }

        private void chkCustomShowName_CheckedChanged(object sender, System.EventArgs e)
        {
            this.txtCustomShowName.Enabled = this.chkCustomShowName.Checked;
        }

        private void chkAutoFolders_CheckedChanged(object sender, System.EventArgs e)
        {
            gbAutoFolders.Enabled = chkAutoFolders.Checked;
        }

        private void bnAddAlias_Click(object sender, System.EventArgs e)
        {
            string aliasName = tbShowAlias.Text;

            if (!string.IsNullOrEmpty(aliasName))
            {
                if (lbShowAlias.FindStringExact(aliasName) == -1)
                {
                    lbShowAlias.Items.Add(aliasName);
                }
                tbShowAlias.Text = "";
            }
        }

        private void bnRemoveAlias_Click(object sender, System.EventArgs e)
        {
            if (lbShowAlias.SelectedItems.Count > 0)
            {
                foreach (int i in lbShowAlias.SelectedIndices)
                {
                    lbShowAlias.Items.RemoveAt(i);
                }
            }
        }

        private void tbShowAlias_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.bnAddAlias_Click(null, null);
        }

        private void cbUseCustomSearch_CheckedChanged(object sender, System.EventArgs e)
        {
            EnableDisableCustomSearch();
        }

        private void EnableDisableCustomSearch()
        {
            bool en = cbUseCustomSearch.Checked;

            lbSearchURL.Enabled = en;
            txtSearchURL.Enabled = en;
            lbTags.Enabled = en;
            txtTagList.Enabled = en;
        }

    }
}
