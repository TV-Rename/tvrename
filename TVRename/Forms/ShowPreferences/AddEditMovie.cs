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
using System.Windows.Forms;

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
    public partial class AddEditMovie : Form
    {
        private readonly MovieConfiguration selectedShow;
        private readonly CodeFinder codeFinderForm;
        private CustomNameTagsFloatingWindow? cntfw;
        internal bool HasChanged;
        private readonly TVDoc mDoc;

        public AddEditMovie([NotNull] MovieConfiguration si, TVDoc doc)
        {
            selectedShow = si;
            mDoc = doc;
            InitializeComponent();
            HasChanged = false;

            SetupDropDowns(si);

            lblSeasonWordPreview.Text = TVSettings.Instance.MovieFolderFormat + "-(" +
                                        CustomMovieName.NameFor(si,
                                            TVSettings.Instance.MovieFolderFormat) + ")";

            lblSeasonWordPreview.ForeColor = Color.DarkGray;

            codeFinderForm =
                new MovieCodeFinder(si.Code != -1 ? si.Code.ToString() : si.LastName, si.Provider) { Dock = DockStyle.Fill };

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

            chkCustomRegion.Checked = selectedShow.UseCustomRegion;
            cbRegion.Text = selectedShow.CustomRegionCode;

            UpdateCustomShowNameEnabled();

            SetupLanguages(si);
            SetupRegions(si);

            cbDoRenaming.Checked = si.DoRename;
            cbDoMissingCheck.Checked = si.DoMissingCheck;

            SetProvider(si);
            chkManualFolders.Checked = selectedShow.UseManualLocations;
            chkAutoFolders.Checked = selectedShow.UseAutomaticFolders;
            PopulateRootDirectories(selectedShow.AutomaticFolderRoot);
            PopulateFolderTypes(selectedShow.Format);
            txtFolderNameFormat.Text = selectedShow.CustomFolderNameFormat;
            txtCustomMovieFileNamingFormat.Text = selectedShow.CustomNamingFormat;
            cbUseCustomNamingFormat.Checked = selectedShow.UseCustomNamingFormat;

            ActiveControl = codeFinderForm; // set initial focus to the code entry/show finder control

            foreach (string folder in selectedShow.ManualLocations)
            {
                lvManualFolders.Items.Add(folder);
            }

            PopulateAliasses();
            SetTagListText();
            EnableDisableCustomNaming();
            UpdateIgnore();
            SetMovieFolderType(si);
        }

        private void PopulateFolderTypes(MovieConfiguration.MovieFolderFormat selectedShowFormat)
        {
            cbFolderType.SuspendLayout();
            cbFolderType.Items.Clear();
            cbFolderType.Items.AddRange(Enum.GetValues(typeof(MovieConfiguration.MovieFolderFormat))
                .OfType<MovieConfiguration.MovieFolderFormat>()
                .Select(x => x.PrettyPrint())
                .ToArray<object>());
            cbFolderType.ResumeLayout();
            cbFolderType.Text = selectedShowFormat.PrettyPrint();
        }
        private MovieConfiguration.MovieFolderFormat? GetFolderFormat()
        {
            return Enum.GetValues(typeof(MovieConfiguration.MovieFolderFormat))
                .Cast<MovieConfiguration.MovieFolderFormat>()
                .FirstOrDefault(format => format.PrettyPrint().Equals(cbFolderType.Text));
        }

        private void SetMovieFolderType([NotNull] MovieConfiguration si)
        {
            if (si.UseAutomaticFolders)
            {
                if (si.UseCustomFolderNameFormat)
                {
                    rdoFolderCustom.Checked = true;
                }
                else
                {
                    rdoFolderLibraryDefault.Checked = true;
                }
            }
        }

        private void SetTagListText()
        {
            System.Text.StringBuilder tl = new();

            foreach (string s in CustomMovieName.TAGS)
            {
                tl.AppendLine($"{s} - {CustomMovieName.NameFor(selectedShow, s)}");
            }

            txtTagList2.Text = tl.ToString();
        }

        private void PopulateRootDirectories(string chosenValue)
        {
            cbDirectory.SuspendLayout();
            cbDirectory.Items.Clear();
            foreach (string folder in TVSettings.Instance.MovieLibraryFolders)
            {
                cbDirectory.Items.Add(folder.EnsureEndsWithSeparator());
            }

            if (TVSettings.Instance.MovieLibraryFolders.Any())
            {
                cbDirectory.SelectedIndex = 0;
            }
            cbDirectory.ResumeLayout();
            cbDirectory.Text = chosenValue.EnsureEndsWithSeparator();
        }

        private void PopulateAliasses()
        {
            foreach (string aliasName in selectedShow.AliasNames)
            {
                lbShowAlias.Items.Add(aliasName);
            }

            if (selectedShow.CachedData != null)
            {
                foreach (string aliasName in selectedShow.CachedData?.GetAliases())
                {
                    lbSourceAliases.Items.Add(aliasName);
                }
            }
        }

        private void SetupLanguages([NotNull] MovieConfiguration si)
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

        private void SetupRegions([NotNull] MovieConfiguration si)
        {
            chkCustomRegion.Checked = si.UseCustomRegion;
            if (chkCustomRegion.Checked)
            {
                Region? r = si.CustomRegionCode.HasValue() ?
                    Regions.Instance.RegionFromCode(si.CustomRegionCode!) : Regions.Instance.FallbackRegion;

                if (r != null)
                {
                    cbRegion.Text = r.EnglishName;
                }
            }

            cbRegion.Enabled = chkCustomRegion.Checked;
        }

        private void SetProvider([NotNull] MovieConfiguration si)
        {
            switch (si.ConfigurationProvider)
            {
                case TVDoc.ProviderType.libraryDefault:
                case TVDoc.ProviderType.TVmaze:
                    rdoDefault.Checked = true;
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

        private void SetupDropDowns([NotNull] MovieConfiguration si)
        {
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

        private TVDoc.ProviderType GetProviderTypeInUse()
        {
            if (GetProviderType() == TVDoc.ProviderType.libraryDefault)
            {
                return TVSettings.Instance.DefaultMovieProvider;
            }

            return GetProviderType();
        }

        private bool OkToClose()
        {
            if (!TVDoc.GetMediaCache(GetProviderTypeInUse()).HasMovie(codeFinderForm.SelectedCode()))
            {
                DialogResult dr = MessageBox.Show($"{GetProviderType().PrettyPrint()} code unknown, close anyway?", "TVRename Add/Edit Movie",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.No)
                {
                    return false;
                }
            }
            if (chkCustomLanguage.Checked && string.IsNullOrWhiteSpace(cbLanguage.SelectedItem?.ToString()))
            {
                MessageBox.Show("Please enter language for the show or accept the default preferred language", "TVRename Add/Edit Movie",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }
            if (chkCustomRegion.Checked && string.IsNullOrWhiteSpace(cbRegion.SelectedItem?.ToString()))
            {
                MessageBox.Show("Please enter region for the show or accept the default region", "TVRename Add/Edit Movie",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }
            if (chkCustomShowName.Checked && string.IsNullOrWhiteSpace(txtCustomShowName.Text))
            {
                MessageBox.Show("Please enter custom for the show or remove custom naming", "TVRename Add/Edit Movie",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }
            if (chkAutoFolders.Checked && string.IsNullOrWhiteSpace(cbDirectory.SelectedItem?.ToString()))
            {
                MessageBox.Show("Please enter base folder for this show or turn off automatic folders", "TVRename Add/Edit Movie",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                Folders.SelectedTab = tabPage5;
                cbDirectory.Focus();

                return false;
            }
            if (chkAutoFolders.Checked && !TVSettings.OKPath(cbDirectory.SelectedItem?.ToString(), false))
            {
                MessageBox.Show("Please check the base folder is a valid one and has no invalid characters"
                    , "TVRename Add/Edit Movie",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                Folders.SelectedTab = tabPage5;
                cbDirectory.Focus();

                return false;
            }

            if (chkAutoFolders.Checked && rdoFolderCustom.Checked && !txtFolderNameFormat.Text.IsValidDirectory())
            {
                MessageBox.Show("Please check the custom subdirectory is a valid one and has no invalid characters"
                    , "TVRename Add/Edit Show",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                Folders.SelectedTab = tabPage5;
                txtFolderNameFormat.Focus();

                return false;
            }

            return true;
        }

        #region HelpWindows

        private void pbBasics_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-basics-tab");

        private void pbAdvanced_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-advanced-tab");

        private void pbAliases_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-show-aliases-tab");

        private void pbFolders_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-folders-tab");

        private static void OpenInfoWindow(string page)
        {
            Helpers.OpenUrl($"https://www.tvrename.com/manual/user{page}");
        }

        #endregion HelpWindows

        private void SetShow()
        {
            int code = codeFinderForm.SelectedCode();

            selectedShow.CustomShowName = txtCustomShowName.Text;
            selectedShow.UseCustomShowName = chkCustomShowName.Checked;
            selectedShow.UseCustomLanguage = chkCustomLanguage.Checked;
            if (selectedShow.UseCustomLanguage)
            {
                selectedShow.CustomLanguageCode = (Languages.Instance.GetLanguageFromLocalName(cbLanguage.SelectedItem?.ToString()) ?? TVSettings.Instance.PreferredTVDBLanguage).Abbreviation;
            }

            selectedShow.UseCustomRegion = chkCustomRegion.Checked;
            selectedShow.CustomRegionCode = cbRegion.Text;
            selectedShow.SetId(GetProviderTypeInUse(), code);
            selectedShow.DoRename = cbDoRenaming.Checked;
            selectedShow.DoMissingCheck = cbDoMissingCheck.Checked;
            selectedShow.ConfigurationProvider = GetProviderType();
            selectedShow.AliasNames.Clear();
            selectedShow.AliasNames.AddRange(lbShowAlias.Items.OfType<string>().Distinct());

            selectedShow.ManualLocations.Clear();
            selectedShow.ManualLocations.AddRange(GetFolders());

            selectedShow.UseManualLocations = chkManualFolders.Checked;
            selectedShow.UseAutomaticFolders = chkAutoFolders.Checked;
            selectedShow.AutomaticFolderRoot = cbDirectory.Text;
            selectedShow.UseCustomFolderNameFormat = rdoFolderCustom.Checked;
            selectedShow.CustomFolderNameFormat = txtFolderNameFormat.Text;
            selectedShow.CustomNamingFormat = txtCustomMovieFileNamingFormat.Text;
            selectedShow.UseCustomNamingFormat = cbUseCustomNamingFormat.Checked;
            selectedShow.Format = GetFolderFormat() ?? MovieConfiguration.MovieFolderFormat.singleDirectorySingleFile;
        }

        private IEnumerable<string> GetFolders()
        {
            List<string> folders = new();
            foreach (ListViewItem item in lvManualFolders.Items.OfType<ListViewItem>())
            {
                folders.Add(item.Text);
            }
            return folders.Distinct();
        }

        private TVDoc.ProviderType GetProviderType()
        {
            if (rdoTMDB.Checked)
            {
                return TVDoc.ProviderType.TMDB;
            }
            if (rdoDefault.Checked)
            {
                return TVDoc.ProviderType.libraryDefault;
            }
            if (rdoTVDB.Checked)
            {
                return TVDoc.ProviderType.TheTVDB;
            }
            return TVDoc.ProviderType.TMDB;
        }

        private void bnCancel_Click(object sender, EventArgs e) => Close();

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
            cntfw = new CustomNameTagsFloatingWindow(selectedShow);
            cntfw.Show(this);
            Focus();
        }

        private void chkCustomLanguage_CheckedChanged(object sender, EventArgs e)
        {
            HasChanged = true;
            cbLanguage.Enabled = chkCustomLanguage.Checked;
        }

        private void MTCCF_SelectionChanged(object sender, EventArgs e)
        {
            HasChanged = true;
        }

        private void bnBrowseFolder_Click_1(object sender, EventArgs e)
        {
            folderBrowser.ShowNewFolderButton = true;

            if (!string.IsNullOrEmpty(txtFolder.Text))
            {
                folderBrowser.SelectedPath = txtFolder.Text;
            }

            if (string.IsNullOrWhiteSpace(folderBrowser.SelectedPath) && !string.IsNullOrWhiteSpace(cbDirectory.SelectedText))
            {
                folderBrowser.SelectedPath = cbDirectory.Text;
            }

            if (folderBrowser.ShowDialog(this) == DialogResult.OK)
            {
                txtFolder.Text = folderBrowser.SelectedPath;
            }
        }

        private void bnAdd_Click_1(object sender, EventArgs e)
        {
            ListViewItem lvi = new() { Text = txtFolder.Text };

            lvManualFolders.Items.Add(lvi);

            txtFolder.Text = string.Empty;

            lvManualFolders.Sort();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (lvManualFolders.SelectedItems.Count > 0)
            {
                foreach (ListViewItem lvi in lvManualFolders.SelectedItems)
                {
                    Helpers.OpenFolder(lvi.Text);
                }
            }
        }

        private void bnRemove_Click_1(object sender, EventArgs e)
        {
            if (lvManualFolders.SelectedItems.Count > 0)
            {
                foreach (ListViewItem lvi in lvManualFolders.SelectedItems)
                {
                    lvManualFolders.Items.Remove(lvi);
                }
            }
        }

        private void btnIgnoreList_Click_1(object sender, EventArgs e)
        {
            IgnoreEdit ie = new(mDoc, cbDirectory.SelectedText);
            ie.ShowDialog(this);
            UpdateIgnore();
        }

        private void UpdateIgnore()
        {
            bool someIgnoredEps = cbDirectory.SelectedText.HasValue() && TVSettings.Instance.Ignore.Any(item => item.FileAndPath.StartsWith(cbDirectory.SelectedText, StringComparison.CurrentCultureIgnoreCase));

            txtIgnoreList.Visible = someIgnoredEps;
            btnIgnoreList.Visible = someIgnoredEps;
        }

        private void cbUseCustomNamingFormat_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableCustomNaming();
        }

        private void EnableDisableCustomNaming()
        {
            bool en = cbUseCustomNamingFormat.Checked;

            lbLibraryDefaultNaming.Enabled = en;
            txtCustomMovieFileNamingFormat.Enabled = en;
            lbAvailableTags.Enabled = en;
            txtTagList2.Enabled = en;
            lbLibraryDefaultNaming.Enabled = en;
            label19.Enabled = en;
            lbNamingExample.Enabled = en;
            llCustomName.Enabled = en;
        }

        private void txtCustomMovieFileNamingFormat_TextChanged(object sender, EventArgs e)
        {
            llCustomName.Text =
                CustomMovieName.NameFor(selectedShow, txtCustomMovieFileNamingFormat.Text);

            llFilenameDefaultFormat.Text = selectedShow.ProposedFilename;
        }

        private void txtFolderNameFormat_TextChanged(object sender, EventArgs e)
        {
        }

        private void rdoProvider_CheckedChanged(object sender, EventArgs e)
        {
            HasChanged = true;
            codeFinderForm.SetSource(GetProviderType(), selectedShow);
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
    }
}
