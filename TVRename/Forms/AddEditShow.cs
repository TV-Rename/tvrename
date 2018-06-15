// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
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
        private readonly ShowItem selectedShow;
        private readonly TheTvdbCodeFinder codeFinderForm;

        public AddEditShow(ShowItem si)
        {
            selectedShow = si;
            InitializeComponent();

            cbTimeZone.BeginUpdate();
            cbTimeZone.Items.Clear();

            foreach (string s in TimeZone.ZoneNames())
                cbTimeZone.Items.Add(s);

            cbTimeZone.EndUpdate();
            cbTimeZone.Text = si.ShowTimeZone;

            codeFinderForm = new TheTvdbCodeFinder(si.TVDBCode != -1 ? si.TVDBCode.ToString() : "");
            codeFinderForm.Dock = DockStyle.Fill;

            pnlCF.SuspendLayout();
            pnlCF.Controls.Add(codeFinderForm);
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
                    ListViewItem lvi = new ListViewItem {Text = kvp.Key.ToString()};
                    lvi.SubItems.Add(s);

                    lvSeasonFolders.Items.Add(lvi);
                }
            }
            lvSeasonFolders.Sort();

            txtSeasonNumber_TextChanged(null, null);
            txtFolder_TextChanged();

            ActiveControl = codeFinderForm; // set initial focus to the code entry/show finder control

            foreach (string aliasName in selectedShow.AliasNames)
            {
                lbShowAlias.Items.Add(aliasName);
            }

            StringBuilder tl = new StringBuilder();

            foreach (string s in CustomName.Tags)
            {
                tl.AppendLine(s);
            }
            txtTagList.Text = tl.ToString();

            cbUseCustomSearch.Checked = si.UseCustomSearchURL && !string.IsNullOrWhiteSpace(si.CustomSearchURL);
            txtSearchURL.Text = si.CustomSearchURL ?? "";
            EnableDisableCustomSearch();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (!OkToClose())
            {
                DialogResult = DialogResult.None;
                return;
            }

            SetShow();
            DialogResult = DialogResult.OK;
            Close();
        }

        private bool OkToClose()
        {
            if (!TheTVDB.Instance.HasSeries(codeFinderForm.SelectedCode()))
            {
                DialogResult dr = MessageBox.Show("tvdb code unknown, close anyway?", "TVRename Add/Edit Show",
                                                  MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.No)
                    return false;
            }

            return true;
        }

        private void SetShow()
        {
            int code = codeFinderForm.SelectedCode();


            selectedShow.CustomShowName = txtCustomShowName.Text;
            selectedShow.UseCustomShowName = chkCustomShowName.Checked;
            selectedShow.ShowTimeZone = cbTimeZone.SelectedItem?.ToString() ?? TimeZone.DefaultTimeZone();
            selectedShow.ShowNextAirdate = chkShowNextAirdate.Checked;
            selectedShow.PadSeasonToTwoDigits = chkPadTwoDigits.Checked;
            selectedShow.TVDBCode = code;
            selectedShow.CountSpecials = chkSpecialsCount.Checked;
            selectedShow.DoRename = cbDoRenaming.Checked;
            selectedShow.DoMissingCheck = cbDoMissingCheck.Checked;
            selectedShow.AutoAddNewSeasons = chkAutoFolders.Checked;
            selectedShow.AutoAdd_FolderPerSeason = chkFolderPerSeason.Checked;
            selectedShow.AutoAdd_SeasonFolderName = txtSeasonFolderName.Text;
            selectedShow.AutoAdd_FolderBase = txtBaseFolder.Text;

            selectedShow.DVDOrder = chkDVDOrder.Checked;
            selectedShow.ForceCheckFuture = cbIncludeFuture.Checked;
            selectedShow.ForceCheckNoAirdate = cbIncludeNoAirdate.Checked;
            selectedShow.UseCustomSearchURL = cbUseCustomSearch.Checked;
            selectedShow.CustomSearchURL = txtSearchURL.Text;

            selectedShow.UseSequentialMatch = cbSequentialMatching.Checked;

            string slist = txtIgnoreSeasons.Text;
            selectedShow.IgnoreSeasons.Clear();
            foreach (Match match in Regex.Matches(slist, "\\b[0-9]+\\b"))
                selectedShow.IgnoreSeasons.Add(int.Parse(match.Value));

            selectedShow.ManualFolderLocations.Clear();
            foreach (ListViewItem lvi in lvSeasonFolders.Items)
            {
                try
                {
                    int seas = int.Parse(lvi.Text);
                    if (!selectedShow.ManualFolderLocations.ContainsKey(seas))
                        selectedShow.ManualFolderLocations.Add(seas, new List<string>());

                    selectedShow.ManualFolderLocations[seas].Add(lvi.SubItems[1].Text);
                }
                catch
                {
                    // ignored
                }
            }

            selectedShow.AliasNames.Clear();
            foreach (string showAlias in lbShowAlias.Items)
            {
                if (!selectedShow.AliasNames.Contains(showAlias))
                {
                    selectedShow.AliasNames.Add(showAlias);
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

        private void txtFolder_TextChanged()
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
                    // ignored
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

        private void tbShowAlias_TextChanged(object sender, EventArgs e)
        {
          bnAddAlias.Enabled = tbShowAlias.Text.Length > 0;
        }

        private void lbShowAlias_SelectedIndexChanged(object sender, EventArgs e)
        {
            bnRemoveAlias.Enabled = lbShowAlias.SelectedItems.Count > 0;
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}
