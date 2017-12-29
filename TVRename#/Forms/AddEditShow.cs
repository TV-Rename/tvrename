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
            mSI = si;
            InitializeComponent();

            cbTimeZone.BeginUpdate();
            cbTimeZone.Items.Clear();

            foreach (string s in TimeZone.ZoneNames())
                cbTimeZone.Items.Add(s);

            cbTimeZone.EndUpdate();

            mTCCF = new TheTVDBCodeFinder(si.TVDBCode != -1 ? si.TVDBCode.ToString() : "");
            mTCCF.Dock = DockStyle.Fill;
            //mTCCF->SelectionChanged += gcnew System::EventHandler(this, &AddEditShow::lvMatches_ItemSelectionChanged);

            pnlCF.SuspendLayout();
            pnlCF.Controls.Add(mTCCF);
            pnlCF.ResumeLayout();

            chkCustomShowName.Checked = si.UseCustomShowName;
            if (chkCustomShowName.Checked)
                txtCustomShowName.Text = si.CustomShowName;
            chkCustomShowName_CheckedChanged(null, null);

            cbSequentialMatching.Checked = si.UseSequentialMatch;
            chkShowNextAirdate.Checked = si.ShowNextAirdate;
            chkSpecialsCount.Checked = si.CountSpecials;
            chkFolderPerSeason.Checked = si.AutoAdd_FolderPerSeason;
            txtSeasonFolderName.Text = si.AutoAdd_SeasonFolderName;
            txtBaseFolder.Text = si.AutoAdd_FolderBase;
            chkAutoFolders.Checked = si.AutoAddNewSeasons;
            chkFolderPerSeason_CheckedChanged(null, null);

            cbDoRenaming.Checked = si.DoRename;
            cbDoMissingCheck.Checked = si.DoMissingCheck;
            cbDoMissingCheck_CheckedChanged(null, null);

            chkPadTwoDigits.Checked = si.PadSeasonToTwoDigits;

            ShowTimeZone = ((si == null) || (si.TheSeries() == null))
                                    ? TimeZone.DefaultTimeZone()
                                    : si.TheSeries().ShowTimeZone;

            cbTimeZone.Text = ShowTimeZone;
            chkDVDOrder.Checked = si.DVDOrder;
            cbIncludeFuture.Checked = si.ForceCheckFuture;
            cbIncludeNoAirdate.Checked = si.ForceCheckNoAirdate;

            bool first = true;
            si.IgnoreSeasons.Sort();
            foreach (int i in si.IgnoreSeasons)
            {
                if (!first)
                    txtIgnoreSeasons.Text += " ";
                txtIgnoreSeasons.Text += i.ToString();
                first = false;
            }

            foreach (KeyValuePair<int, List<string>> kvp in si.ManualFolderLocations)
            {
                foreach (string s in kvp.Value)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = kvp.Key.ToString();
                    lvi.SubItems.Add(s);

                    lvSeasonFolders.Items.Add(lvi);
                }
            }
            lvSeasonFolders.Sort();

            txtSeasonNumber_TextChanged(null, null);
            txtFolder_TextChanged(null, null);

            ActiveControl = mTCCF; // set initial focus to the code entry/show finder control

            foreach (string aliasName in mSI.AliasNames)
            {
                lbShowAlias.Items.Add(aliasName);
            }

            StringBuilder tl = new StringBuilder();

            foreach (string s in CustomName.Tags)
            {
                tl.AppendLine(s);
            }
            txtTagList.Text = tl.ToString();

            cbUseCustomSearch.Checked = !String.IsNullOrEmpty(si.CustomSearchURL);
            txtSearchURL.Text = si.CustomSearchURL ?? "";
            EnableDisableCustomSearch();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (!OKToClose())
            {
                DialogResult = DialogResult.None;
                return;
            }

            SetmSI();
            DialogResult = DialogResult.OK;
            Close();
        }

        private bool OKToClose()
        {
            if (!TheTVDB.Instance.HasSeries(mTCCF.SelectedCode()))
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
            int code = mTCCF.SelectedCode();

            string tz = (cbTimeZone.SelectedItem != null) ? cbTimeZone.SelectedItem.ToString() : "";

            mSI.CustomShowName = txtCustomShowName.Text;
            mSI.UseCustomShowName = chkCustomShowName.Checked;
            ShowTimeZone = tz; //TODO: move to somewhere else. make timezone manager for tvdb?
            mSI.ShowNextAirdate = chkShowNextAirdate.Checked;
            mSI.PadSeasonToTwoDigits = chkPadTwoDigits.Checked;
            mSI.TVDBCode = code;
            //todo mSI->SeasonNumber = seasnum;
            mSI.CountSpecials = chkSpecialsCount.Checked;
            //                                 mSI->Rules = mWorkingRuleSet;  // TODO
            mSI.DoRename = cbDoRenaming.Checked;
            mSI.DoMissingCheck = cbDoMissingCheck.Checked;

            mSI.AutoAddNewSeasons = chkAutoFolders.Checked;
            mSI.AutoAdd_FolderPerSeason = chkFolderPerSeason.Checked;
            mSI.AutoAdd_SeasonFolderName = txtSeasonFolderName.Text;
            mSI.AutoAdd_FolderBase = txtBaseFolder.Text;

            mSI.DVDOrder = chkDVDOrder.Checked;
            mSI.ForceCheckFuture = cbIncludeFuture.Checked;
            mSI.ForceCheckNoAirdate = cbIncludeNoAirdate.Checked;
            mSI.CustomSearchURL = txtSearchURL.Text;

            mSI.UseSequentialMatch = cbSequentialMatching.Checked;

            string slist = txtIgnoreSeasons.Text;
            mSI.IgnoreSeasons.Clear();
            foreach (Match match in Regex.Matches(slist, "\\b[0-9]+\\b"))
                mSI.IgnoreSeasons.Add(int.Parse(match.Value));

            mSI.ManualFolderLocations.Clear();
            foreach (ListViewItem lvi in lvSeasonFolders.Items)
            {
                try
                {
                    int seas = int.Parse(lvi.Text);
                    if (!mSI.ManualFolderLocations.ContainsKey(seas))
                        mSI.ManualFolderLocations.Add(seas, new List<String>());

                    mSI.ManualFolderLocations[seas].Add(lvi.SubItems[1].Text);
                }
                catch
                {
                }
            }

            mSI.AliasNames.Clear();
            foreach (string showAlias in lbShowAlias.Items)
            {
                if (!mSI.AliasNames.Contains(showAlias))
                {
                    mSI.AliasNames.Add(showAlias);
                }
            }
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void chkFolderPerSeason_CheckedChanged(object sender, EventArgs e)
        {
            txtSeasonFolderName.Enabled = chkFolderPerSeason.Checked;
        }

        private void bnBrowse_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBaseFolder.Text))
                folderBrowser.SelectedPath = txtBaseFolder.Text;

            if (folderBrowser.ShowDialog() == DialogResult.OK)
                txtBaseFolder.Text = folderBrowser.SelectedPath;
        }

        private void cbDoMissingCheck_CheckedChanged(object sender, EventArgs e)
        {
            cbIncludeNoAirdate.Enabled = cbDoMissingCheck.Checked;
            cbIncludeFuture.Enabled = cbDoMissingCheck.Checked;
        }

        private void bnRemove_Click(object sender, EventArgs e)
        {
            if (lvSeasonFolders.SelectedItems.Count > 0)
            {
                foreach (ListViewItem lvi in lvSeasonFolders.SelectedItems)
                    lvSeasonFolders.Items.Remove(lvi);
            }
        }

        private void bnAdd_Click(object sender, EventArgs e)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = txtSeasonNumber.Text;
            lvi.SubItems.Add(txtFolder.Text);

            lvSeasonFolders.Items.Add(lvi);

            txtSeasonNumber.Text = "";
            txtFolder.Text = "";

            lvSeasonFolders.Sort();
        }

        private void bnBrowseFolder_Click(object sender, EventArgs e)
        {
            folderBrowser.SelectedPath = txtFolder.Text;
            if (folderBrowser.ShowDialog() == DialogResult.OK)
                txtFolder.Text = folderBrowser.SelectedPath;
        }

        private void txtSeasonNumber_TextChanged(object sender, EventArgs e)
        {
            bool isNumber = Regex.Match(txtSeasonNumber.Text, "^[0-9]+$").Success;
            bnAdd.Enabled = isNumber && (!string.IsNullOrEmpty(txtSeasonNumber.Text));
        }

        private void txtFolder_TextChanged(object sender, EventArgs e)
        {
            bool ok = true;
            if (!string.IsNullOrEmpty(txtFolder.Text))
            {
                try
                {
                    ok = System.IO.Directory.Exists(txtFolder.Text);
                }
                catch
                {
                }
            }
            txtFolder.BackColor = ok ? System.Drawing.SystemColors.Window : Helpers.WarningColor();
        }

        private void chkCustomShowName_CheckedChanged(object sender, EventArgs e)
        {
            txtCustomShowName.Enabled = chkCustomShowName.Checked;
        }

        private void chkAutoFolders_CheckedChanged(object sender, EventArgs e)
        {
            gbAutoFolders.Enabled = chkAutoFolders.Checked;
        }

        private void bnAddAlias_Click(object sender, EventArgs e)
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

        private void bnRemoveAlias_Click(object sender, EventArgs e)
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
                bnAddAlias_Click(null, null);
        }

        private void cbUseCustomSearch_CheckedChanged(object sender, EventArgs e)
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
