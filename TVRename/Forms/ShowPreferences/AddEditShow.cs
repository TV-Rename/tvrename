// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TVRename.Forms.ShowPreferences;

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
        private CustomNameTagsFloatingWindow cntfw;
        private readonly Season sampleSeason;
        private readonly ProcessedEpisode sampleEpisode;

        public AddEditShow(ShowItem si)
        {
            selectedShow = si;
            sampleSeason = si.GetFirstAvailableSeason();
            sampleEpisode = si.GetFirstAvailableEpisode();
            InitializeComponent();

            lblSeasonWordPreview.Text = TVSettings.Instance.SeasonFolderFormat + "-(" + CustomSeasonName.NameFor(si.GetFirstAvailableSeason(), TVSettings.Instance.SeasonFolderFormat) + ")";
            lblSeasonWordPreview.ForeColor = Color.DarkGray;

            SetupDropDowns(si);

            codeFinderForm =
                new TheTvdbCodeFinder(si.TvdbCode != -1 ? si.TvdbCode.ToString() : "") { Dock = DockStyle.Fill };

            pnlCF.SuspendLayout();
            pnlCF.Controls.Add(codeFinderForm);
            pnlCF.ResumeLayout();

            cntfw = null;
            chkCustomShowName.Checked = si.UseCustomShowName;
            if (chkCustomShowName.Checked)
                txtCustomShowName.Text = si.CustomShowName;
            chkCustomShowName_CheckedChanged(null, null);

            chkCustomLanguage.Checked = si.UseCustomLanguage;
            if (chkCustomLanguage.Checked)
                cbLanguage.Text = TheTVDB.Instance.LanguageList.GetLanguageFromCode(si.CustomLanguageCode).Name;
            chkCustomLanguage_CheckedChanged(null, null);

            cbSequentialMatching.Checked = si.UseSequentialMatch;
            chkShowNextAirdate.Checked = si.ShowNextAirdate;
            chkSpecialsCount.Checked = si.CountSpecials;
            txtBaseFolder.Text = si.AutoAddFolderBase;

            cbDoRenaming.Checked = si.DoRename;
            cbDoMissingCheck.Checked = si.DoMissingCheck;
            cbDoMissingCheck_CheckedChanged(null, null);

            switch (si.AutoAddType)
            {
                case ShowItem.AutomaticFolderType.none:
                    chkAutoFolders.Checked = false;
                    break;
                case ShowItem.AutomaticFolderType.baseOnly:
                    chkAutoFolders.Checked = true;
                    rdoFolderBaseOnly.Checked = true;
                    break;
                case ShowItem.AutomaticFolderType.custom:
                    chkAutoFolders.Checked = true;
                    rdoFolderCustom.Checked = true;
                    break;
                case ShowItem.AutomaticFolderType.libraryDefault:
                default:
                    chkAutoFolders.Checked = true;
                    rdoFolderLibraryDefault.Checked = true;
                    break;
            }

            txtSeasonFormat.Text = si.AutoAddCustomFolderFormat;

            chkDVDOrder.Checked = si.DvdOrder;
            cbIncludeFuture.Checked = si.ForceCheckFuture;
            cbIncludeNoAirdate.Checked = si.ForceCheckNoAirdate;
            chkReplaceAutoFolders.Checked = si.ManualFoldersReplaceAutomatic;

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
                    ListViewItem lvi = new ListViewItem { Text = kvp.Key.ToString() };
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

            foreach (string s in CustomEpisodeName.TAGS)
            {
                tl.AppendLine(s + (sampleEpisode != null ? " - " + CustomEpisodeName.NameForNoExt(sampleEpisode, s): string.Empty));
            }

            txtTagList.Text = tl.ToString();

            cbUseCustomSearch.Checked = si.UseCustomSearchUrl && !string.IsNullOrWhiteSpace(si.CustomSearchUrl);
            txtSearchURL.Text = si.CustomSearchUrl ?? "";
            EnableDisableCustomSearch();
        }

        private void SetupDropDowns(ShowItem si)
        {
            cbTimeZone.BeginUpdate();
            cbTimeZone.Items.Clear();
            foreach (string s in TimeZoneHelper.ZoneNames())
                cbTimeZone.Items.Add(s);
            cbTimeZone.EndUpdate();
            cbTimeZone.Text = si.ShowTimeZone;

            string pref = "";
            cbLanguage.BeginUpdate();
            cbLanguage.Items.Clear();
            foreach (Language l in TheTVDB.Instance.LanguageList)
            {
                cbLanguage.Items.Add(l.Name);

                if (si.CustomLanguageCode == l.Abbreviation)
                    pref = l.Name;
            }
            cbLanguage.EndUpdate();
            cbLanguage.Text = pref;
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
            if (chkCustomLanguage.Checked && string.IsNullOrWhiteSpace(cbLanguage.SelectedItem?.ToString()))
            {
                MessageBox.Show("Please enter language for the show or accept the default preferred language", "TVRename Add/Edit Show",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                return false;
            }
            if (chkAutoFolders.Checked && string.IsNullOrWhiteSpace(txtBaseFolder.Text))
            {
                MessageBox.Show("Please enter base folder for this show or turn off automatic folders", "TVRename Add/Edit Show",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                Folders.SelectedTab = tabPage5;
                txtBaseFolder.Focus();

                return false;
            }
            if (chkAutoFolders.Checked && !TVSettings.OKPath(txtBaseFolder.Text))
            {
                MessageBox.Show("Please check the base folder is a valid one and has no invalid characters"
                    , "TVRename Add/Edit Show",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                Folders.SelectedTab = tabPage5;
                txtBaseFolder.Focus();

                return false;
            }
            return true;
        }

        private void SetShow()
        {
            int code = codeFinderForm.SelectedCode();

            selectedShow.CustomShowName = txtCustomShowName.Text;
            selectedShow.UseCustomShowName = chkCustomShowName.Checked;
            selectedShow.UseCustomLanguage = chkCustomLanguage.Checked;
            if (selectedShow.UseCustomLanguage)
            {
                selectedShow.CustomLanguageCode = TheTVDB.Instance.LanguageList
                    .GetLanguageFromLocalName(cbLanguage.SelectedItem?.ToString())?.Abbreviation ??TVSettings.Instance.PreferredLanguageCode;
            }
            selectedShow.ShowTimeZone = cbTimeZone.SelectedItem?.ToString() ?? TimeZoneHelper.DefaultTimeZone();
            selectedShow.ShowNextAirdate = chkShowNextAirdate.Checked;
            selectedShow.TvdbCode = code;
            selectedShow.CountSpecials = chkSpecialsCount.Checked;
            selectedShow.DoRename = cbDoRenaming.Checked;
            selectedShow.DoMissingCheck = cbDoMissingCheck.Checked;
            selectedShow.AutoAddCustomFolderFormat = txtSeasonFormat.Text;
            selectedShow.AutoAddFolderBase = txtBaseFolder.Text;

            if (chkAutoFolders.Checked){
                if (rdoFolderCustom.Checked)
                    selectedShow.AutoAddType = ShowItem.AutomaticFolderType.custom;
                else if (rdoFolderBaseOnly.Checked)
                    selectedShow.AutoAddType = ShowItem.AutomaticFolderType.baseOnly;
                else if (rdoFolderLibraryDefault.Checked)
                    selectedShow.AutoAddType = ShowItem.AutomaticFolderType.libraryDefault;
            }
            else
                selectedShow.AutoAddType = ShowItem.AutomaticFolderType.none;

            selectedShow.DvdOrder = chkDVDOrder.Checked;
            selectedShow.ForceCheckFuture = cbIncludeFuture.Checked;
            selectedShow.ForceCheckNoAirdate = cbIncludeNoAirdate.Checked;
            selectedShow.UseCustomSearchUrl = cbUseCustomSearch.Checked;
            selectedShow.CustomSearchUrl = txtSearchURL.Text;
            selectedShow.ManualFoldersReplaceAutomatic = chkReplaceAutoFolders.Checked;

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

        private void bnCancel_Click(object sender, EventArgs e) => Close();

        private void bnBrowse_Click(object sender, EventArgs e)
        {
            folderBrowser.ShowNewFolderButton = true;

            if (!string.IsNullOrEmpty(txtBaseFolder.Text))
                folderBrowser.SelectedPath = txtBaseFolder.Text;

            if (folderBrowser.ShowDialog(this) == DialogResult.OK)
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
            ListViewItem lvi = new ListViewItem {Text = txtSeasonNumber.Text};
            lvi.SubItems.Add(txtFolder.Text);

            lvSeasonFolders.Items.Add(lvi);

            txtSeasonNumber.Text = "";
            txtFolder.Text = "";

            lvSeasonFolders.Sort();
        }

        private void bnBrowseFolder_Click(object sender, EventArgs e)
        {
            //folderBrowser.Title = "Add Folder...";
            //folderBrowser.ShowEditbox = true;
            //folderBrowser.StartPosition = FormStartPosition.CenterParent;
            folderBrowser.ShowNewFolderButton = true;

            if (!string.IsNullOrEmpty(txtFolder.Text))
                folderBrowser.SelectedPath = txtFolder.Text;

            if(string.IsNullOrWhiteSpace(folderBrowser.SelectedPath) && !string.IsNullOrWhiteSpace(txtBaseFolder.Text))
                folderBrowser.SelectedPath = txtBaseFolder.Text;

            if (folderBrowser.ShowDialog(this) == DialogResult.OK)
                txtFolder.Text = folderBrowser.SelectedPath;
        }

        private void txtSeasonNumber_TextChanged(object sender, EventArgs e) => CheckToEnableAddButton();

        private void CheckToEnableAddButton()
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
            txtFolder.BackColor = ok ? SystemColors.Window : Helpers.WarningColor();
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

            if (string.IsNullOrEmpty(aliasName)) return;

            if (lbShowAlias.FindStringExact(aliasName) == -1)
            {
                lbShowAlias.Items.Add(aliasName);
            }
            tbShowAlias.Text = "";
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
            llCustomSearchPreview.Enabled = en;
            lbSearchExample.Enabled = en;
        }

        private void tbShowAlias_TextChanged(object sender, EventArgs e)
        {
          bnAddAlias.Enabled = tbShowAlias.Text.Length > 0;
        }

        private void lbShowAlias_SelectedIndexChanged(object sender, EventArgs e)
        {
            bnRemoveAlias.Enabled = lbShowAlias.SelectedItems.Count > 0;
        }

        private void bnTags_Click(object sender, EventArgs e)
        {
                cntfw = new CustomNameTagsFloatingWindow(sampleSeason);
                cntfw.Show(this);
                Focus();
        }

        private void txtFolder_TextChanged(object sender, EventArgs e) => CheckToEnableAddButton();

        private void lvSeasonFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            bnRemove.Enabled = lvSeasonFolders.SelectedItems.Count > 0;
        }

        private void chkCustomLanguage_CheckedChanged(object sender, EventArgs e)
        {
            cbLanguage.Enabled = chkCustomLanguage.Checked;
        }

        private void bnQuickLocate_Click(object sender, EventArgs e)
        {
            //If there are no LibraryFolders then we cant use the simplified UI
            if (TVSettings.Instance.LibraryFolders.Count == 0)
            {
                MessageBox.Show(
                    "Please add some library folders in the Preferences to use this.",
                    "Can't Auto Add Show", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                return;
            }

            string showName = codeFinderForm.SelectedShow()?.Name ?? txtCustomShowName.Text ?? "New Folder";
            QuickLocateForm f = new QuickLocateForm(showName);

            if (f.ShowDialog() == DialogResult.OK)
            {
                txtBaseFolder.Text = f.DirectoryFullPath;
            }
        }

        private void txtSearchURL_TextChanged(object sender, EventArgs e)
        {
            llCustomSearchPreview.Text = CustomEpisodeName.NameForNoExt(sampleEpisode, txtSearchURL.Text, true);
        }

        private void llCustomSearchPreview_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Helpers.SysOpen(llCustomSearchPreview.Text);
        }
    }
}
