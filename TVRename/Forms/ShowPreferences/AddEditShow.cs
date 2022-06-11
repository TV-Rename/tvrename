//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NLog;
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
        private readonly ShowConfiguration selectedShow;
        private readonly CodeFinder codeFinderForm;
        private CustomNameTagsFloatingWindow? cntfw;
        internal bool HasChanged;
        private readonly ProcessedSeason? sampleProcessedSeason;
        private readonly ProcessedEpisode? sampleEpisode;
        private readonly bool addingNewShow;
        private readonly TVDoc mDoc;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public AddEditShow([NotNull] ShowConfiguration si, TVDoc doc)
        {
            selectedShow = si;
            mDoc = doc;
            sampleProcessedSeason = si.GetFirstAvailableSeason();
            sampleEpisode = si.GetFirstAvailableEpisode();
            addingNewShow = si.TvdbCode == -1 && si.TmdbCode == -1 && si.TVmazeCode == -1;
            InitializeComponent();
            HasChanged = false;

            lblSeasonWordPreview.Text = SeasonPreviewText();
            lblSeasonWordPreview.ForeColor = Color.DarkGray;

            SetupDropDowns(si);

            codeFinderForm = new TvCodeFinder(si.Code != -1 ? si.Code.ToString() : si.LastName, si.Provider) { Dock = DockStyle.Fill };

            codeFinderForm.SelectionChanged += MTCCF_SelectionChanged;

            pnlCF.SuspendLayout();
            pnlCF.Controls.Add(codeFinderForm);
            pnlCF.ResumeLayout();

            cntfw = null;
            chkCustomShowName.Checked = si.UseCustomShowName;
            if (chkCustomShowName.Checked)
            {
                txtCustomShowName.Text = si.CustomShowName;
            }

            UpdateCustomShowNameEnabled();

            SetupLanguages(si);
            SetupRegions(si);

            cbSequentialMatching.Checked = si.UseSequentialMatch;
            cbAirdateMatching.Checked = si.UseAirDateMatch;
            cbEpNameMatching.Checked = si.UseEpNameMatch;

            chkShowNextAirdate.Checked = si.ShowNextAirdate;
            chkSpecialsCount.Checked = si.CountSpecials;
            txtBaseFolder.Text = si.AutoAddFolderBase;

            cbDoRenaming.Checked = si.DoRename;
            cbDoMissingCheck.Checked = si.DoMissingCheck;
            cbDoMissingCheck_CheckedChanged(null, null);

            SetAutoAdd(si);
            SetProvider(si);

            txtSeasonFormat.Text = si.AutoAddCustomFolderFormat;

            chkDVDOrder.Checked = si.DvdOrder;
            chkAlternateOrder.Checked = si.AlternateOrder;
            cbIncludeFuture.Checked = si.ForceCheckFuture;
            cbIncludeNoAirdate.Checked = si.ForceCheckNoAirdate;
            chkReplaceAutoFolders.Checked = si.ManualFoldersReplaceAutomatic;

            SetIgnoreSeasons(si);

            SetManualFolders(si);

            CheckToEnableAddButton();
            txtFolder_TextChanged();

            ActiveControl = codeFinderForm; // set initial focus to the code entry/show finder control

            PopulateAliasses();
            SetTagListText();

            cbUseCustomSearch.Checked = si.UseCustomSearchUrl && !string.IsNullOrWhiteSpace(si.CustomSearchUrl);
            cbUseCustomNamingFormat.Checked = si.UseCustomNamingFormat && !string.IsNullOrWhiteSpace(si.CustomNamingFormat);

            txtSearchURL.Text = si.CustomSearchUrl;
            txtCustomEpisodeNamingFormat.Text = si.CustomNamingFormat;

            EnableDisableCustomSearch();
            EnableDisableCustomNaming();
            UpdateIgnore();
        }
        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);
            lvSeasonFolders.ScaleListViewColumns(factor);
        }
        private string SeasonPreviewText()
        {
            if (sampleProcessedSeason == null)
            {
                return TVSettings.Instance.SeasonFolderFormat;
            }
            return
                $"{TVSettings.Instance.SeasonFolderFormat}-({CustomSeasonName.NameFor(sampleProcessedSeason, TVSettings.Instance.SeasonFolderFormat)})";
        }

        private void PopulateAliasses()
        {
            foreach (string aliasName in selectedShow.AliasNames)
            {
                lbShowAlias.Items.Add(aliasName);
            }

            if (selectedShow.CachedShow != null)
            {
                foreach (string aliasName in selectedShow.CachedShow.GetAliases())
                {
                    lbSourceAliases.Items.Add(aliasName);
                }
            }
        }

        private void SetupLanguages([NotNull] ShowConfiguration si)
        {
            chkCustomLanguage.Checked = si.UseCustomLanguage;
            if (chkCustomLanguage.Checked)
            {
                Language languageFromCode = Languages.Instance.GetLanguageFromCode(si.CustomLanguageCode);

                if (languageFromCode != null)
                {
                    cbLanguage.Text = languageFromCode.LocalName;
                }
            }

            cbLanguage.Enabled = chkCustomLanguage.Checked;
        }

        private void SetupRegions([NotNull] ShowConfiguration si)
        {
            chkCustomRegion.Checked = si.UseCustomRegion;
            if (chkCustomLanguage.Checked && si.CustomRegionCode.HasValue())
            {
                Region r = Regions.Instance.RegionFromCode(si.CustomRegionCode!);

                if (r != null)
                {
                    cbRegion.Text = r.EnglishName;
                }
            }

            cbRegion.Enabled = chkCustomRegion.Checked;
        }

        private void SetTagListText()
        {
            StringBuilder tl = new();

            foreach (string s in CustomEpisodeName.TAGS)
            {
                tl.AppendLine(s + (sampleEpisode != null
                                  ? " - " + CustomEpisodeName.NameForNoExt(sampleEpisode, s)
                                  : string.Empty));
            }

            txtTagList.Text = tl.ToString();
            txtTagList2.Text = tl.ToString();
        }

        private void SetIgnoreSeasons([NotNull] ShowConfiguration si)
        {
            bool first = true;
            si.IgnoreSeasons.Sort();
            foreach (int i in si.IgnoreSeasons.Distinct())
            {
                if (!first)
                {
                    txtIgnoreSeasons.Text += " ";
                }

                txtIgnoreSeasons.Text += i.ToString();
                first = false;
            }
        }

        private void SetAutoAdd([NotNull] ShowConfiguration si)
        {
            switch (si.AutoAddType)
            {
                case ShowConfiguration.AutomaticFolderType.none:
                    chkAutoFolders.Checked = false;
                    break;

                case ShowConfiguration.AutomaticFolderType.baseOnly:
                    chkAutoFolders.Checked = true;
                    rdoFolderBaseOnly.Checked = true;
                    break;

                case ShowConfiguration.AutomaticFolderType.customFolderFormat:
                    chkAutoFolders.Checked = true;
                    rdoFolderCustom.Checked = true;
                    break;

                case ShowConfiguration.AutomaticFolderType.libraryDefaultFolderFormat:
                    chkAutoFolders.Checked = true;
                    rdoFolderLibraryDefault.Checked = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetProvider([NotNull] ShowConfiguration si)
        {
            switch (si.ConfigurationProvider)
            {
                case TVDoc.ProviderType.libraryDefault:
                    rdoDefault.Checked = true;
                    break;

                case TVDoc.ProviderType.TVmaze:
                    rdoTVMaze.Checked = true;
                    break;

                case TVDoc.ProviderType.TheTVDB:
                    rdoTVDB.Checked = true;
                    break;

                case TVDoc.ProviderType.TMDB:
                    rdoTMDB.Checked = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetManualFolders([NotNull] ShowConfiguration si)
        {
            foreach (KeyValuePair<int, List<string>> kvp in si.ManualFolderLocations)
            {
                foreach (string s in kvp.Value)
                {
                    ListViewItem lvi = new() { Text = kvp.Key.ToString() };
                    lvi.SubItems.Add(s);

                    lvSeasonFolders.Items.Add(lvi);
                }
            }

            lvSeasonFolders.Sort();
        }

        private void SetupDropDowns([NotNull] ShowConfiguration si)
        {
            cbTimeZone.BeginUpdate();
            cbTimeZone.Items.Clear();
            foreach (string s in TimeZoneHelper.ZoneNames())
            {
                cbTimeZone.Items.Add(s);
            }

            cbTimeZone.EndUpdate();
            cbTimeZone.Text = si.ShowTimeZone;

            string pref = string.Empty;
            cbLanguage.BeginUpdate();
            cbLanguage.Items.Clear();
            foreach (Language l in Languages.Instance)
            {
                cbLanguage.Items.Add(l.LocalName);

                if (si.CustomLanguageCode == l.Abbreviation)
                {
                    pref = l.LocalName;
                }
            }
            cbLanguage.EndUpdate();
            cbLanguage.Text = pref;

            string rpref = string.Empty;
            cbRegion.BeginUpdate();
            cbRegion.Items.Clear();
            foreach (Region r in Regions.Instance)
            {
                if (r.EnglishName.HasValue())
                {
                    cbRegion.Items.Add(r.EnglishName!);

                    if (si.CustomRegionCode == r.Abbreviation)
                    {
                        rpref = r.EnglishName;
                    }
                }
            }
            cbRegion.EndUpdate();
            cbRegion.Text = rpref;
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
            if (!TVDoc.GetMediaCache(GetProviderTypeInUse()).HasSeries(codeFinderForm.SelectedCode()))
            {
                DialogResult dr = MessageBox.Show($"{GetConfigurationProviderType().PrettyPrint()} code unknown, close anyway?", "TV Rename Add/Edit TV Show",
                                                  MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.No)
                {
                    return false;
                }
            }
            if (chkCustomLanguage.Checked && string.IsNullOrWhiteSpace(cbLanguage.SelectedItem?.ToString()))
            {
                MessageBox.Show("Please enter language for the show or accept the default preferred language", "TV Rename Add/Edit TV Show",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }
            if (chkCustomRegion.Checked && string.IsNullOrWhiteSpace(cbRegion.SelectedItem?.ToString()))
            {
                MessageBox.Show("Please enter region for the show or accept the default region", "TV Rename Add/Edit TV Show",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }
            if (chkCustomShowName.Checked && string.IsNullOrWhiteSpace(txtCustomShowName.Text))
            {
                MessageBox.Show("Please enter custom for the show or remove custom naming", "TV Rename Add/Edit TV Show",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }
            if (chkAutoFolders.Checked && string.IsNullOrWhiteSpace(txtBaseFolder.Text))
            {
                MessageBox.Show("Please enter base folder for this show or turn off automatic season folders", "TV Rename Add/Edit TV Show",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                Folders.SelectedTab = tabPage5;
                txtBaseFolder.Focus();

                return false;
            }
            if (chkAutoFolders.Checked && !TVSettings.OKPath(txtBaseFolder.Text, false))
            {
                MessageBox.Show("Please check the base folder is a valid one and has no invalid characters"
                    , "TV Rename Add/Edit TV Show",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                Folders.SelectedTab = tabPage5;
                txtBaseFolder.Focus();

                return false;
            }

            if (chkAutoFolders.Checked && rdoFolderCustom.Checked && !txtSeasonFormat.Text.IsValidDirectory())
            {
                MessageBox.Show("Please check the custom subdirectory is a valid one and has no invalid characters"
                    , "TV Rename Add/Edit TV Show",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                Folders.SelectedTab = tabPage5;
                txtSeasonFormat.Focus();

                return false;
            }

            return true;
        }

        #region HelpWindows

        private void pbBasics_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-basics-tab");

        private void pbAdvanced_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-advanced-tab");

        private void pbSearch_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-search-tab");

        private void pbAliases_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-show-aliases-tab");

        private void pbFolders_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-folders-tab");

        private static void OpenInfoWindow(string page)
        {
            Helpers.OpenUrl($"https://www.tvrename.com/manual/user{page}");
        }

        #endregion HelpWindows

        private void SetShow()
        {
            selectedShow.SetId(GetProviderTypeInUse(), codeFinderForm.SelectedCode());
            selectedShow.CustomShowName = txtCustomShowName.Text;
            selectedShow.UseCustomShowName = chkCustomShowName.Checked;
            selectedShow.UseCustomLanguage = chkCustomLanguage.Checked;
            selectedShow.UseCustomRegion = chkCustomRegion.Checked;
            SetupOverrides();

            selectedShow.ShowTimeZone = cbTimeZone.SelectedItem?.ToString() ?? TVSettings.Instance.DefaultShowTimezoneName ?? TimeZoneHelper.DefaultTimeZone();
            selectedShow.ShowNextAirdate = chkShowNextAirdate.Checked;

            selectedShow.CountSpecials = chkSpecialsCount.Checked;
            selectedShow.DoRename = cbDoRenaming.Checked;
            selectedShow.DoMissingCheck = cbDoMissingCheck.Checked;
            selectedShow.AutoAddCustomFolderFormat = txtSeasonFormat.Text;
            selectedShow.AutoAddFolderBase = txtBaseFolder.Text;

            selectedShow.AutoAddType = GetAutoAddType();
            selectedShow.ConfigurationProvider = GetConfigurationProviderType();

            selectedShow.DvdOrder = chkDVDOrder.Checked;
            selectedShow.AlternateOrder = chkAlternateOrder.Checked;
            selectedShow.ForceCheckFuture = cbIncludeFuture.Checked;
            selectedShow.ForceCheckNoAirdate = cbIncludeNoAirdate.Checked;
            selectedShow.UseCustomSearchUrl = cbUseCustomSearch.Checked;
            selectedShow.CustomSearchUrl = txtSearchURL.Text;
            selectedShow.UseCustomNamingFormat = cbUseCustomNamingFormat.Checked;
            selectedShow.CustomNamingFormat = txtCustomEpisodeNamingFormat.Text;
            selectedShow.ManualFoldersReplaceAutomatic = chkReplaceAutoFolders.Checked;

            selectedShow.UseSequentialMatch = cbSequentialMatching.Checked;
            selectedShow.UseAirDateMatch = cbAirdateMatching.Checked;
            selectedShow.UseEpNameMatch = cbEpNameMatching.Checked;

            SetupDropDowns();
        }

        private void SetupOverrides()
        {
            if (selectedShow.UseCustomLanguage)
            {
                selectedShow.CustomLanguageCode = (Languages.Instance.GetLanguageFromLocalName(cbLanguage.SelectedItem?.ToString()) ?? TVSettings.Instance.PreferredTVDBLanguage).Abbreviation;
            }

            if (selectedShow.UseCustomRegion)
            {
                selectedShow.CustomRegionCode = Regions.Instance.RegionFromName(cbLanguage.SelectedItem?.ToString())?.ThreeAbbreviation ?? TVSettings.Instance.TMDBRegion.ThreeAbbreviation;
            }
        }

        private void SetupDropDowns()
        {
            string slist = txtIgnoreSeasons.Text;
            selectedShow.IgnoreSeasons.Clear();
            foreach (Match match in Regex.Matches(slist, "\\b[0-9]+\\b"))
            {
                selectedShow.IgnoreSeasons.Add(int.Parse(match.Value));
            }

            selectedShow.ManualFolderLocations.Clear();
            foreach (ListViewItem lvi in lvSeasonFolders.Items)
            {
                try
                {
                    int seas = int.Parse(lvi.Text);
                    selectedShow.ManualFolderLocations.TryAdd(seas, new List<string>());
                    selectedShow.ManualFolderLocations[seas].Add(lvi.SubItems[1].Text);
                }
                catch
                {
                    // ignored
                }
            }

            selectedShow.AliasNames.Clear();
            selectedShow.AliasNames.AddNullableRange(lbShowAlias.Items.Cast<string>().Distinct());
        }

        private ShowConfiguration.AutomaticFolderType GetAutoAddType()
        {
            if (!chkAutoFolders.Checked)
            {
                return ShowConfiguration.AutomaticFolderType.none;
            }
            if (rdoFolderCustom.Checked)
            {
                return ShowConfiguration.AutomaticFolderType.customFolderFormat;
            }
            if (rdoFolderBaseOnly.Checked)
            {
                return ShowConfiguration.AutomaticFolderType.baseOnly;
            }
            return ShowConfiguration.AutomaticFolderType.libraryDefaultFolderFormat;
        }

        private TVDoc.ProviderType GetConfigurationProviderType()
        {
            if (rdoTVMaze.Checked)
            {
                return TVDoc.ProviderType.TVmaze;
            }
            if (rdoDefault.Checked)
            {
                return TVDoc.ProviderType.libraryDefault;
            }
            if (rdoTVDB.Checked)
            {
                return TVDoc.ProviderType.TheTVDB;
            }
            if (rdoTMDB.Checked)
            {
                return TVDoc.ProviderType.TMDB;
            }
            return TVDoc.ProviderType.TheTVDB;
        }

        private TVDoc.ProviderType GetProviderTypeInUse()
        {
            if (GetConfigurationProviderType() == TVDoc.ProviderType.libraryDefault)
            {
                return TVSettings.Instance.DefaultProvider;
            }

            return GetConfigurationProviderType();
        }

        private void bnCancel_Click(object sender, EventArgs e) => Close();

        private void bnBrowse_Click(object sender, EventArgs e)
        {
            folderBrowser.ShowNewFolderButton = true;

            if (!string.IsNullOrEmpty(txtBaseFolder.Text))
            {
                folderBrowser.SelectedPath = txtBaseFolder.Text;
            }

            try
            {
                if (folderBrowser.ShowDialog(this) == DialogResult.OK)
                {
                    txtBaseFolder.Text = folderBrowser.SelectedPath;
                }
            }
            catch (SEHException ex)
            {
                Logger.Error(ex,"Could not load Folder Selection Dialog:");
            }
        }

        private void cbDoMissingCheck_CheckedChanged(object? sender, EventArgs? e)
        {
            cbIncludeNoAirdate.Enabled = cbDoMissingCheck.Checked;
            cbIncludeFuture.Enabled = cbDoMissingCheck.Checked;
        }

        private void bnRemove_Click(object sender, EventArgs e)
        {
            if (lvSeasonFolders.SelectedItems.Count > 0)
            {
                foreach (ListViewItem lvi in lvSeasonFolders.SelectedItems)
                {
                    lvSeasonFolders.Items.Remove(lvi);
                }
            }
        }

        private void bnAdd_Click(object sender, EventArgs e)
        {
            ListViewItem lvi = new() { Text = txtSeasonNumber.Text };
            lvi.SubItems.Add(txtFolder.Text);

            lvSeasonFolders.Items.Add(lvi);

            txtSeasonNumber.Text = string.Empty;
            txtFolder.Text = string.Empty;

            lvSeasonFolders.Sort();
        }

        private void bnBrowseFolder_Click(object sender, EventArgs e)
        {
            folderBrowser.ShowNewFolderButton = true;

            if (!string.IsNullOrEmpty(txtFolder.Text))
            {
                folderBrowser.SelectedPath = txtFolder.Text;
            }

            if (string.IsNullOrWhiteSpace(folderBrowser.SelectedPath) && !string.IsNullOrWhiteSpace(txtBaseFolder.Text))
            {
                folderBrowser.SelectedPath = txtBaseFolder.Text;
            }

            if (folderBrowser.ShowDialog(this) == DialogResult.OK)
            {
                txtFolder.Text = folderBrowser.SelectedPath;
            }
        }

        private void txtSeasonNumber_TextChanged(object sender, EventArgs e) => CheckToEnableAddButton();

        private void CheckToEnableAddButton()
        {
            bool isNumber = Regex.Match(txtSeasonNumber.Text, "^[0-9]+$").Success;
            bnAdd.Enabled = isNumber && !string.IsNullOrEmpty(txtSeasonNumber.Text);
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
            UpdateCustomShowNameEnabled();
        }

        private void UpdateCustomShowNameEnabled()
        {
            txtCustomShowName.Enabled = chkCustomShowName.Checked;
        }

        private void chkAutoFolders_CheckedChanged(object sender, EventArgs e)
        {
            gbAutoFolders.Enabled = chkAutoFolders.Checked;
        }

        private void bnAddAlias_Click(object sender, EventArgs e)
        {
            AddAlias();
        }

        private void AddAlias()
        {
            string aliasName = tbShowAlias.Text;

            if (string.IsNullOrEmpty(aliasName))
            {
                return;
            }

            if (lbShowAlias.FindStringExact(aliasName) == -1)
            {
                lbShowAlias.Items.Add(aliasName);
            }

            tbShowAlias.Text = string.Empty;
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

        private void tbShowAlias_KeyDown(object sender, [NotNull] KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddAlias();
            }
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

        private void EnableDisableCustomNaming()
        {
            bool en = cbUseCustomNamingFormat.Checked;

            lbLibraryDefaultNaming.Enabled = en;
            txtCustomEpisodeNamingFormat.Enabled = en;
            lbAvailableTags.Enabled = en;
            txtTagList2.Enabled = en;
            lbLibraryDefaultNaming.Enabled = en;
            label19.Enabled = en;
            lbNamingExample.Enabled = en;
            llCustomName.Enabled = en;
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
            cntfw = new CustomNameTagsFloatingWindow(sampleProcessedSeason);
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
            HasChanged = true;
            cbLanguage.Enabled = chkCustomLanguage.Checked;
        }

        private void bnQuickLocate_Click(object sender, EventArgs e)
        {
            //If there are no LibraryFolders then we cant use the simplified UI
            if (TVSettings.Instance.LibraryFolders.Count == 0)
            {
                MessageBox.Show(
                    "Please add some library folders in the Preferences to use this.",
                    "Can't Auto Add TV Show", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            string showName = codeFinderForm.SelectedShow() is null
                ? txtCustomShowName.Text ?? "New Folder"
                : TVSettings.Instance.DefaultTVShowFolder(codeFinderForm.SelectedShow());

            QuickLocateForm f = new(showName, MediaConfiguration.MediaType.tv);

            if (f.ShowDialog(this) == DialogResult.OK)
            {
                txtBaseFolder.Text = f.DirectoryFullPath;
            }
        }

        private void txtSearchURL_TextChanged(object sender, EventArgs e)
        {
            if (sampleEpisode != null)
            {
                llCustomSearchPreview.Text = CustomEpisodeName.NameForNoExt(sampleEpisode, txtSearchURL.Text, true);
            }
        }

        private void llCustomSearchPreview_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Helpers.OpenUrl(llCustomSearchPreview.Text);
        }

        private void MTCCF_SelectionChanged(object sender, EventArgs e)
        {
            HasChanged = true;
            if (addingNewShow && TVSettings.Instance.DefShowAutoFolders && TVSettings.Instance.DefShowUseDefLocation)
            {
                txtBaseFolder.Text =
                    TVSettings.Instance.DefShowLocation.EnsureEndsWithSeparator()
                    + TVSettings.Instance.DefaultTVShowFolder(codeFinderForm.SelectedShow());
            }
        }

        private void BtnIgnoreList_Click(object sender, EventArgs e)
        {
            IgnoreEdit ie = new(mDoc, txtBaseFolder.Text);
            ie.ShowDialog(this);
            UpdateIgnore();
        }

        private void TxtBaseFolder_TextChanged(object sender, EventArgs e)
        {
            UpdateIgnore();
        }

        private void UpdateIgnore()
        {
            bool someIgnoredEps = txtBaseFolder.Text.HasValue() && TVSettings.Instance.Ignore.Any(item => item.FileAndPath.StartsWith(txtBaseFolder.Text, StringComparison.CurrentCultureIgnoreCase));

            txtIgnoreList.Visible = someIgnoredEps;
            btnIgnoreList.Visible = someIgnoredEps;
        }

        private void CbUseCustomNamingFormat_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableCustomNaming();
        }

        private void TxtCustomEpisodeNamingFormat_TextChanged(object sender, EventArgs e)
        {
            if (sampleEpisode != null)
            {
                llCustomName.Text =
                    CustomEpisodeName.NameForNoExt(sampleEpisode, txtCustomEpisodeNamingFormat.Text, false);

                llLibraryDefaultFormat.Text = TVSettings.Instance.NamingStyle.NameFor(sampleEpisode);
            }
        }

        private void txtSeasonFormat_TextChanged(object sender, EventArgs e)
        {
        }

        private void rdoProvider_CheckedChanged(object sender, EventArgs e)
        {
            HasChanged = true;
            codeFinderForm.SetSource(GetConfigurationProviderType(), selectedShow);
        }

        private void chkCustomRegion_CheckedChanged(object sender, EventArgs e)
        {
            HasChanged = true;
            cbRegion.Enabled = chkCustomRegion.Checked;
        }

        private void cbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            HasChanged = true;
        }

        private void cbRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            HasChanged = true;
        }

        private void chkDVDOrder_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDVDOrder.Checked)
            {
                chkAlternateOrder.Checked = false;
            }
        }

        private void chkAlternateOrder_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAlternateOrder.Checked)
            {
                chkDVDOrder.Checked = false;
            }
        }
    }
}
