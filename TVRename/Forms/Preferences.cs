// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using ColumnHeader = SourceGrid.Cells.ColumnHeader;

namespace TVRename
{
    /// <summary>
    /// Summary for Preferences
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class Preferences : Form
    {
        delegate void LoadLanguageDoneDel();

        private TVDoc mDoc;
        private Thread LoadLanguageThread;
        private String EnterPreferredLanguage; // hold here until background language download task is done
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private LoadLanguageDoneDel LoadLanguageDone;

        public Preferences(TVDoc doc, bool goToScanOpts)
        {
            InitializeComponent();
            LoadLanguageDone += LoadLanguageDoneFunc;

            SetupRSSGrid();
            SetupReplacementsGrid();

            mDoc = doc;

            if (goToScanOpts)
                tabControl1.SelectedTab = tpScanOptions;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (!TVSettings.OKExtensionsString(txtEmptyIgnoreExtensions.Text))
            {
                MessageBox.Show(
                    "Extensions list must be separated by semicolons, and each extension must start with a dot.",
                    "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl1.SelectedTab = tbFolderDeleting;
                txtVideoExtensions.Focus();
                return;
            }

            if (!TVSettings.OKExtensionsString(txtVideoExtensions.Text))
            {
                MessageBox.Show(
                    "Extensions list must be separated by semicolons, and each extension must start with a dot.",
                    "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl1.SelectedTab = tbFilesAndFolders;
                txtVideoExtensions.Focus();
                return;
            }
            if (!TVSettings.OKExtensionsString(txtVideoExtensions.Text))
            {
                MessageBox.Show(
                    "Extensions list must be separated by semicolons, and each extension must start with a dot.",
                    "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl1.SelectedTab = tbFilesAndFolders;
                txtVideoExtensions.Focus();
                return;
            }
            if (!TVSettings.OKExtensionsString(txtOtherExtensions.Text))
            {
                MessageBox.Show(
                    "Extensions list must be separated by semicolons, and each extension must start with a dot.",
                    "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl1.SelectedTab = tbFilesAndFolders;
                txtOtherExtensions.Focus();
                return;
            }
            if (!TVSettings.OKExtensionsString(txtKeepTogether.Text))
            {
                MessageBox.Show(
                    "Extensions list must be separated by semicolons, and each extension must start with a dot.",
                    "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl1.SelectedTab = tbFilesAndFolders;
                txtKeepTogether.Focus();
                return;
            }

            TVSettings S = TVSettings.Instance;

            S.Replacements.Clear();
            for (int i = 1; i < ReplacementsGrid.RowsCount; i++)
            {
                string from = (string) (ReplacementsGrid[i, 0].Value);
                string to = (string) (ReplacementsGrid[i, 1].Value);
                bool ins = (bool) (ReplacementsGrid[i, 2].Value);
                if (!string.IsNullOrEmpty(from))
                    S.Replacements.Add(new Replacement(from, to, ins));
            }

            S.ExportWTWRSS = cbWTWRSS.Checked;
            S.ExportWTWRSSTo = txtWTWRSS.Text;
            S.ExportWTWXML = cbWTWXML.Checked;
            S.ExportWTWXMLTo = txtWTWXML.Text;
            S.ExportMissingXML = cbMissingXML.Checked;
            S.ExportMissingXMLTo = txtMissingXML.Text;
            S.ExportMissingCSV = cbMissingCSV.Checked;
            S.ExportMissingCSVTo = txtMissingCSV.Text;
            S.ExportRenamingXML = cbRenamingXML.Checked;
            S.ExportRenamingXMLTo = txtRenamingXML.Text;
            S.ExportFOXML = cbFOXML.Checked;
            S.ExportFOXMLTo = txtFOXML.Text;
            S.ExportShowsTXT = cbShowsTXT.Checked;
            S.ExportShowsTXTTo = txtShowsTXTTo.Text;
            S.ExportShowsHTML = cbShowsHTML.Checked;
            S.ExportShowsHTMLTo = txtShowsHTMLTo.Text;

            S.WTWRecentDays = Convert.ToInt32(txtWTWDays.Text);
            S.StartupTab = cbStartupTab.SelectedIndex;
            S.NotificationAreaIcon = cbNotificationIcon.Checked;
            S.VideoExtensionsString = txtVideoExtensions.Text;
            S.OtherExtensionsString = txtOtherExtensions.Text;
            S.ExportRSSMaxDays = Convert.ToInt32(txtExportRSSMaxDays.Text);
            S.ExportRSSMaxShows = Convert.ToInt32(txtExportRSSMaxShows.Text);
            S.ExportRSSDaysPast = Convert.ToInt32(txtExportRSSDaysPast.Text);
            S.KeepTogether = cbKeepTogether.Checked;
            S.LeadingZeroOnSeason = cbLeadingZero.Checked;
            S.ShowInTaskbar = chkShowInTaskbar.Checked;
            S.RenameTxtToSub = cbTxtToSub.Checked;
            S.ShowEpisodePictures = cbShowEpisodePictures.Checked;
            S.HideMyShowsSpoilers = chkHideMyShowsSpoilers.Checked;
            S.HideWtWSpoilers = chkHideWtWSpoilers.Checked;
            S.AutoSelectShowInMyShows = cbAutoSelInMyShows.Checked;
            S.AutoCreateFolders = cbAutoCreateFolders.Checked ;  
            S.SpecialsFolderName = txtSpecialsFolderName.Text;
            S.searchSeasonWordsString = tbSeasonSearchTerms.Text;
            S.preferredRSSSearchTermsString = tbPreferredRSSTerms.Text;
            

            S.defaultSeasonWord = txtSeasonFolderName.Text;
            S.keepTogetherExtensionsString = txtKeepTogether.Text;

            S.ForceLowercaseFilenames = cbForceLower.Checked;
            S.IgnoreSamples = cbIgnoreSamples.Checked;

            S.uTorrentPath = txtRSSuTorrentPath.Text;
            S.ResumeDatPath = txtUTResumeDatPath.Text;
            S.SABHostPort = txtSABHostPort.Text;
            S.SABAPIKey = txtSABAPIKey.Text;
            S.CheckSABnzbd = cbCheckSABnzbd.Checked;

            S.SearchRSS = cbSearchRSS.Checked;
            S.EpTBNs = cbEpTBNs.Checked;
            S.NFOShows = cbNFOShows.Checked;
            S.NFOEpisodes = cbNFOEpisodes.Checked;
            S.KODIImages = cbKODIImages.Checked;
            S.pyTivoMeta = cbMeta.Checked;
            S.pyTivoMetaSubFolder = cbMetaSubfolder.Checked;
            S.FolderJpg = cbFolderJpg.Checked;
            S.RenameCheck = cbRenameCheck.Checked;
            S.PreventMove = chkPreventMove.Checked;
            S.MissingCheck = cbMissing.Checked;
            S.CorrectFileDates = cbxUpdateAirDate.Checked;
            S.SearchLocally = cbSearchLocally.Checked;
            S.AutoSearchForDownloadedFiles = chkAutoSearchForDownloadedFiles.Checked;
            S.LeaveOriginals = cbLeaveOriginals.Checked;
            S.CheckuTorrent = cbCheckuTorrent.Checked;
            S.LookForDateInFilename = cbLookForAirdate.Checked;
            S.AutoMergeDownloadEpisodes = chkAutoMergeDownloadEpisodes.Checked;
            S.AutoMergeLibraryEpisodes = chkAutoMergeLibraryEpisodes.Checked;
            S.RetainLanguageSpecificSubtitles = chkRetainLanguageSpecificSubtitles.Checked;
            S.ForceBulkAddToUseSettingsOnly = chkForceBulkAddToUseSettingsOnly.Checked;

            S.MonitorFolders = cbMonitorFolder.Checked;
            S.runStartupCheck = chkScanOnStartup.Checked;
            S.runPeriodicCheck = chkScheduledScan.Checked;
            S.periodCheckHours = int.Parse(domainUpDown1.SelectedItem?.ToString()??"1");
            S.RemoveDownloadDirectoriesFiles = cbCleanUpDownloadDir.Checked;

            S.EpJPGs = cbEpThumbJpg.Checked;
            S.SeriesJpg = cbSeriesJpg.Checked;
            S.Mede8erXML = cbXMLFiles.Checked;
            S.ShrinkLargeMede8erImages = cbShrinkLarge.Checked;
            S.FanArtJpg = cbFantArtJpg.Checked;

            S.Tidyup.DeleteEmpty = cbDeleteEmpty.Checked;
            S.Tidyup.DeleteEmptyIsRecycle = cbRecycleNotDelete.Checked;
            S.Tidyup.EmptyIgnoreWords = cbEmptyIgnoreWords.Checked;
            S.Tidyup.EmptyIgnoreWordList = txtEmptyIgnoreWords.Text;
            S.Tidyup.EmptyIgnoreExtensions = cbEmptyIgnoreExtensions.Checked;
            S.Tidyup.EmptyIgnoreExtensionList = txtEmptyIgnoreExtensions.Text;
            S.Tidyup.EmptyMaxSizeCheck = cbEmptyMaxSize.Checked;
            int.TryParse(txtEmptyMaxSize.Text, out S.Tidyup.EmptyMaxSizeMB);

            S.BulkAddCompareNoVideoFolders = cbIgnoreNoVideoFolders.Checked;
            S.BulkAddIgnoreRecycleBin = cbIgnoreRecycleBin.Checked;
            S.AutoAddIgnoreSuffixes = tbIgnoreSuffixes.Text;
            S.AutoAddMovieTerms = tbMovieTerms.Text;

            if (rbFolderFanArt.Checked)
                S.FolderJpgIs = TVSettings.FolderJpgIsType.FanArt;
            else if (rbFolderBanner.Checked)
                S.FolderJpgIs = TVSettings.FolderJpgIsType.Banner;
            else if (rbFolderSeasonPoster.Checked)
                S.FolderJpgIs = TVSettings.FolderJpgIsType.SeasonPoster;
            else
                S.FolderJpgIs = TVSettings.FolderJpgIsType.Poster;

            if (rdoQuickScan.Checked)
                S.MonitoredFoldersScanType = TVSettings.ScanType.Quick;
            else if (rdoRecentScan.Checked)
                S.MonitoredFoldersScanType = TVSettings.ScanType.Recent;
            else
                S.MonitoredFoldersScanType = TVSettings.ScanType.Full;

            if (rdEden.Checked)
                S.SelectedKODIType= TVSettings.KODIType.Eden;
            else if (rdFrodo.Checked)
                S.SelectedKODIType = TVSettings.KODIType.Frodo;
            else
                S.SelectedKODIType = TVSettings.KODIType.Both;

            if (cbMode.Text == "Beta")
            {
                S.mode = TVSettings.BetaMode.BetaToo;
            }
            else
            {
                S.mode = TVSettings.BetaMode.ProductionOnly;
            }

            if (cbKeepTogetherMode.Text == "All but these")
            {
                S.keepTogetherMode = TVSettings.KeepTogetherModes.AllBut;
            } else if (cbKeepTogetherMode.Text == "Just")

            {
                S.keepTogetherMode = TVSettings.KeepTogetherModes.Just;
            }
            else
                S.keepTogetherMode = TVSettings.KeepTogetherModes.All;


            TheTVDB.Instance.GetLock("Preferences-OK");
            foreach (TheTVDB.Language l in TheTVDB.Instance.LanguageList)
            {
                if (l.name == cbLanguages.Text)
                {
                    S.PreferredLanguage = l.abbreviation;
                    break;
                }
            }
            S.WTWDoubleClick = rbWTWScan.Checked ? TVSettings.WTWDoubleClickAction.Scan : TVSettings.WTWDoubleClickAction.Search;

            TheTVDB.Instance.SaveCache();
            TheTVDB.Instance.Unlock("Preferences-OK");

            try
            {
                S.SampleFileMaxSizeMB = int.Parse(txtMaxSampleSize.Text);
            }
            catch
            {
                S.SampleFileMaxSizeMB = 50;
            }

            try
            {
                S.upgradeDirtyPercent = float.Parse(tbPercentDirty.Text);
            }
            catch
            {
                S.upgradeDirtyPercent = 20;
            }
            if (S.upgradeDirtyPercent < 1)
                S.upgradeDirtyPercent = 1;
            else if (S.upgradeDirtyPercent > 100)
                S.upgradeDirtyPercent = 100;


            try
            {
                S.ParallelDownloads = int.Parse(txtParallelDownloads.Text);
            }
            catch
            {
                S.ParallelDownloads = 4;
            }

            if (S.ParallelDownloads < 1)
                S.ParallelDownloads = 1;
            else if (S.ParallelDownloads > 8)
                S.ParallelDownloads = 8;

            // RSS URLs
            S.RSSURLs.Clear();
            for (int i = 1; i < RSSGrid.RowsCount; i++)
            {
                string url = (string) (RSSGrid[i, 0].Value);
                if (!string.IsNullOrEmpty(url))
                    S.RSSURLs.Add(url);
            }

            S.ShowStatusColors = new ShowStatusColoringTypeList();
            foreach (ListViewItem item in lvwDefinedColors.Items)
            {
                if (item.SubItems.Count > 1 && !string.IsNullOrEmpty(item.SubItems[1].Text) && item.Tag != null &&
                    item.Tag is ShowStatusColoringType)
                {
                    S.ShowStatusColors.Add(item.Tag as ShowStatusColoringType,
                                           ColorTranslator.FromHtml(item.SubItems[1].Text));
                }
            }

            mDoc.SetDirty();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Preferences_Load(object sender, EventArgs e)
        {
            SetupLanguages();

            TVSettings S = TVSettings.Instance;

            foreach (Replacement R in S.Replacements)
            {
                AddNewReplacementRow(R.This, R.That, R.CaseInsensitive);
            }

            txtMaxSampleSize.Text = S.SampleFileMaxSizeMB.ToString();

            cbWTWRSS.Checked = S.ExportWTWRSS;
            txtWTWRSS.Text = S.ExportWTWRSSTo;
            txtWTWDays.Text = S.WTWRecentDays.ToString();
            cbWTWXML.Checked = S.ExportWTWXML;
            txtWTWXML.Text = S.ExportWTWXMLTo;
            txtExportRSSMaxDays.Text = S.ExportRSSMaxDays.ToString();
            txtExportRSSMaxShows.Text = S.ExportRSSMaxShows.ToString();
            txtExportRSSDaysPast.Text = S.ExportRSSDaysPast.ToString();

            cbMissingXML.Checked = S.ExportMissingXML;
            txtMissingXML.Text = S.ExportMissingXMLTo;
            cbMissingCSV.Checked = S.ExportMissingCSV;
            txtMissingCSV.Text = S.ExportMissingCSVTo;

            
            cbShowsTXT.Checked = S.ExportShowsTXT ;
            txtShowsTXTTo.Text = S.ExportShowsTXTTo;
            cbShowsHTML.Checked = S.ExportShowsHTML;
            txtShowsHTMLTo.Text = S.ExportShowsHTMLTo;


            cbRenamingXML.Checked = S.ExportRenamingXML;
            txtRenamingXML.Text = S.ExportRenamingXMLTo;

            cbFOXML.Checked = S.ExportFOXML;
            txtFOXML.Text = S.ExportFOXMLTo;

            cbStartupTab.SelectedIndex = S.StartupTab;
            cbNotificationIcon.Checked = S.NotificationAreaIcon;
            txtVideoExtensions.Text = S.GetVideoExtensionsString();
            txtOtherExtensions.Text = S.GetOtherExtensionsString();
            txtKeepTogether.Text = S.GetKeepTogetherString();
            tbSeasonSearchTerms.Text = S.GetSeasonSearchTermsString();
            tbPreferredRSSTerms .Text = S.GetPreferredRSSSearchTermsString();

            cbKeepTogether.Checked = S.KeepTogether;
            cbKeepTogether_CheckedChanged(null, null);

            cbLeadingZero.Checked = S.LeadingZeroOnSeason;
            chkShowInTaskbar.Checked = S.ShowInTaskbar;
            cbTxtToSub.Checked = S.RenameTxtToSub;
            cbShowEpisodePictures.Checked = S.ShowEpisodePictures;
            chkHideMyShowsSpoilers.Checked = S.HideMyShowsSpoilers;
            chkHideWtWSpoilers.Checked = S.HideWtWSpoilers;
            cbAutoCreateFolders.Checked = S.AutoCreateFolders; 
            cbAutoSelInMyShows.Checked = S.AutoSelectShowInMyShows;
            txtSpecialsFolderName.Text = S.SpecialsFolderName;
            cbForceLower.Checked = S.ForceLowercaseFilenames;
            cbIgnoreSamples.Checked = S.IgnoreSamples;
            txtRSSuTorrentPath.Text = S.uTorrentPath;
            txtUTResumeDatPath.Text = S.ResumeDatPath;
            txtSABHostPort.Text = S.SABHostPort;
            txtSABAPIKey.Text = S.SABAPIKey;
            cbCheckSABnzbd.Checked = S.CheckSABnzbd;

            txtParallelDownloads.Text = S.ParallelDownloads.ToString();
            tbPercentDirty.Text = S.upgradeDirtyPercent.ToString();

            cbSearchRSS.Checked = S.SearchRSS;
            cbEpTBNs.Checked = S.EpTBNs;
            cbNFOShows.Checked = S.NFOShows;
            cbNFOEpisodes.Checked = S.NFOEpisodes;
            cbKODIImages.Checked = S.KODIImages;
            cbMeta.Checked = S.pyTivoMeta;
            cbMetaSubfolder.Checked = S.pyTivoMetaSubFolder;
            cbFolderJpg.Checked = S.FolderJpg;
            cbRenameCheck.Checked = S.RenameCheck;
            chkPreventMove.Checked = S.PreventMove;
            cbCheckuTorrent.Checked = S.CheckuTorrent;
            cbLookForAirdate.Checked = S.LookForDateInFilename;
            chkRetainLanguageSpecificSubtitles.Checked = S.RetainLanguageSpecificSubtitles;
            chkForceBulkAddToUseSettingsOnly.Checked = S.ForceBulkAddToUseSettingsOnly;
            chkAutoMergeDownloadEpisodes.Checked = S.AutoMergeDownloadEpisodes;
            chkAutoMergeLibraryEpisodes.Checked = S.AutoMergeLibraryEpisodes;
            cbMonitorFolder.Checked = S.MonitorFolders;
            chkScheduledScan.Checked = S.RunPeriodicCheck();
            chkScanOnStartup.Checked = S.RunOnStartUp();
            domainUpDown1.SelectedItem = S.periodCheckHours;
            cbCleanUpDownloadDir.Checked = S.RemoveDownloadDirectoriesFiles;
            cbMissing.Checked = S.MissingCheck;
            cbxUpdateAirDate.Checked = S.CorrectFileDates;
            chkAutoSearchForDownloadedFiles.Checked = S.AutoSearchForDownloadedFiles;
            cbSearchLocally.Checked = S.SearchLocally;
            cbLeaveOriginals.Checked = S.LeaveOriginals;
            EnterPreferredLanguage = S.PreferredLanguage;

            cbEpThumbJpg.Checked = S.EpJPGs;
            cbSeriesJpg.Checked = S.SeriesJpg;
            cbXMLFiles.Checked = S.Mede8erXML;
            cbShrinkLarge.Checked = S.ShrinkLargeMede8erImages;
            cbFantArtJpg.Checked = S.FanArtJpg;


#if DEBUG
            System.Diagnostics.Debug.Assert(S.Tidyup != null);
#endif
            cbDeleteEmpty.Checked = S.Tidyup.DeleteEmpty;
            cbRecycleNotDelete.Checked = S.Tidyup.DeleteEmptyIsRecycle;
            cbEmptyIgnoreWords.Checked = S.Tidyup.EmptyIgnoreWords;
            txtEmptyIgnoreWords.Text = S.Tidyup.EmptyIgnoreWordList;
            cbEmptyIgnoreExtensions.Checked = S.Tidyup.EmptyIgnoreExtensions;
            txtEmptyIgnoreExtensions.Text = S.Tidyup.EmptyIgnoreExtensionList;
            cbEmptyMaxSize.Checked = S.Tidyup.EmptyMaxSizeCheck;
            txtEmptyMaxSize.Text = S.Tidyup.EmptyMaxSizeMB.ToString();
            txtSeasonFolderName.Text = S.defaultSeasonWord;

            
            cbIgnoreRecycleBin.Checked = S.BulkAddIgnoreRecycleBin;
            cbIgnoreNoVideoFolders.Checked = S.BulkAddCompareNoVideoFolders;
            tbMovieTerms.Text = S.AutoAddMovieTerms;
            tbIgnoreSuffixes.Text = S.AutoAddIgnoreSuffixes;

            switch (S.WTWDoubleClick)
            {
                case TVSettings.WTWDoubleClickAction.Search:
                default:
                    rbWTWSearch.Checked = true;
                    break;
                case TVSettings.WTWDoubleClickAction.Scan:
                    rbWTWScan.Checked = true;
                    break;
            }
            switch(S.keepTogetherMode)
            {
                case TVSettings.KeepTogetherModes.All:
                default:
                    cbKeepTogetherMode.Text = "All";
                    break;
                case TVSettings.KeepTogetherModes.AllBut:
                    cbKeepTogetherMode.Text = "All but these";
                    break;
                case TVSettings.KeepTogetherModes.Just:
                    cbKeepTogetherMode.Text = "Just";
                    break;
            }

            switch (S.mode)
            {
                case TVSettings.BetaMode.ProductionOnly:
                default:
                    cbMode.Text = "Production";
                    break;
                case TVSettings.BetaMode.BetaToo:
                    cbMode.Text = "Beta";
                    break;

            }

            EnableDisable(null, null);
            ScanOptEnableDisable();

            FillSearchFolderList();

            foreach (string s in S.RSSURLs)
                AddNewRSSRow(s);

            switch (S.FolderJpgIs)
            {
                case TVSettings.FolderJpgIsType.FanArt:
                    rbFolderFanArt.Checked = true;
                    break;
                case TVSettings.FolderJpgIsType.Banner:
                    rbFolderBanner.Checked = true;
                    break;
                case TVSettings.FolderJpgIsType.SeasonPoster:
                    rbFolderSeasonPoster.Checked = true;
                    break;
                default:
                    rbFolderPoster.Checked = true;
                    break;
            }

            switch (S.MonitoredFoldersScanType)
            {
                case TVSettings.ScanType.Quick:
                    rdoQuickScan.Checked = true;
                    break;
                case TVSettings.ScanType.Recent:
                    rdoRecentScan.Checked = true;
                    break;
                default:
                    rdoFullScan.Checked = true;
                    break;
            }

            switch (S.SelectedKODIType)
            {
                case TVSettings.KODIType.Eden:
                    rdEden.Checked = true;
                    break;
                case TVSettings.KODIType.Frodo:
                    rdFrodo.Checked = true;
                    break;
                default:
                    rdBoth.Checked = true;
                    break;
            }

            if (S.ShowStatusColors != null)
            {
                foreach (
                    System.Collections.Generic.KeyValuePair<ShowStatusColoringType, Color> showStatusColor in
                        S.ShowStatusColors)
                {
                    ListViewItem item = new ListViewItem
                    {
                        Text = showStatusColor.Key.Text,
                        Tag = showStatusColor.Key,
                        ForeColor = showStatusColor.Value
                    };
                    item.SubItems.Add(TranslateColorToHtml(showStatusColor.Value));
                    lvwDefinedColors.Items.Add(item);
                }
            }

            FillTreeViewColoringShowStatusTypeCombobox();
        }



        private void FillTreeViewColoringShowStatusTypeCombobox()
        {
            //System.Collections.Generic.KeyValuePair<string, object> item = new System.Collections.Generic.KeyValuePair<string, object>();
            // Shows
            foreach (string status in Enum.GetNames(typeof(ShowItem.ShowAirStatus)))
            {
                ShowStatusColoringType t = new ShowStatusColoringType(true, true, status);
                //System.Collections.Generic.KeyValuePair<string, object> item = new System.Collections.Generic.KeyValuePair<string, object>("Show Seasons Status: " + status, new ShowStatusColoringType(true, true, status));
                cboShowStatus.Items.Add(t);
                //this.cboShowStatus.Items.Add("Show Seasons Status: " + status);
            }
            System.Collections.Generic.List<string> showStatusList = new System.Collections.Generic.List<string>();
            foreach (ShowItem show in mDoc.Library.GetShowItems())
            {
                if(!showStatusList.Contains(show.ShowStatus))
                    showStatusList.Add(show.ShowStatus);
            }
            foreach (string status in showStatusList)
            {
                ShowStatusColoringType t = new ShowStatusColoringType(false, true, status);
                //System.Collections.Generic.KeyValuePair<string, object> item = new System.Collections.Generic.KeyValuePair<string, object>("Show  Status: " + status, new ShowStatusColoringType(false, true, status));
                cboShowStatus.Items.Add(t);
            }
            //this.cboShowStatus.Items.Add(new System.Collections.Generic.KeyValuePair<string, object>("Show Seasons Status: Custom", null));
            // Seasons
            foreach (string status in Enum.GetNames(typeof(Season.SeasonStatus)))
            {
                ShowStatusColoringType t = new ShowStatusColoringType(true, false, status);
                //System.Collections.Generic.KeyValuePair<string, object> item = new System.Collections.Generic.KeyValuePair<string, object>("Seasons Status: " + status, new ShowStatusColoringType(true, false, status));
                cboShowStatus.Items.Add(t);
                //this.cboShowStatus.Items.Add("Seasons Status: " + status);
            }
            cboShowStatus.DisplayMember = "Text";
            //this.cboShowStatus.ValueMember = ";
        }

        private void Browse(TextBox txt, string DefaultExt, int FilterIndex)
        {
            //rss =1, XML = 2, CSV = 3, TXT=4, HTML = 5
            saveFile.FileName = txt.Text;
            saveFile.DefaultExt = DefaultExt;
            saveFile.FilterIndex = FilterIndex;
            if (saveFile.ShowDialog() == DialogResult.OK)
                txt.Text = saveFile.FileName;
        }


        private void bnBrowseWTWXML_Click(object sender, EventArgs e)
        {
            Browse(txtWTWXML,"xml",2);
        }

        private void txtNumberOnlyKeyPress(object sender, KeyPressEventArgs e)
        {
            // digits only
            if ((e.KeyChar >= 32) && (!Char.IsDigit(e.KeyChar)))
                e.Handled = true;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cbNotificationIcon_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbNotificationIcon.Checked)
                chkShowInTaskbar.Checked = true;
        }

        private void chkShowInTaskbar_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkShowInTaskbar.Checked)
                cbNotificationIcon.Checked = true;
        }

        private void cbKeepTogether_CheckedChanged(object sender, EventArgs e)
        {
            cbTxtToSub.Enabled = cbKeepTogether.Checked;
            txtKeepTogether.Enabled = (cbKeepTogether.Checked && cbKeepTogetherMode.Text != "All");
            cbKeepTogetherMode.Enabled = cbKeepTogether.Checked;
            label39.Enabled = cbKeepTogether.Checked;
        }

        private void bnBrowseMissingCSV_Click(object sender, EventArgs e)
        {
            Browse(txtMissingCSV,"csv",3);
        }


        private void bnBrowseWTWRSS_Click(object sender, EventArgs e)
        {
            Browse(txtWTWRSS, "rss", 1);
        }

        private void bnBrowseMissingXML_Click(object sender, EventArgs e)
        {
            Browse(txtMissingXML,"xml",2);
        }

        private void bnBrowseShowsTXT_Click(object sender, EventArgs e)
        {
            Browse(txtShowsTXTTo, "txt", 4);
        }

        private void bnBrowseShowsHTML_Click(object sender, EventArgs e)
        {
            Browse(txtShowsHTMLTo, "html", 5);
        }

        private void bnBrowseRenamingXML_Click(object sender, EventArgs e)
        {
            Browse(txtRenamingXML,"xml",2);
        }

        private void bnBrowseFOXML_Click(object sender, EventArgs e)
        {
            Browse(txtFOXML,"xml",2);
        }

        private void EnableDisable(object sender, EventArgs e)
        {
            txtWTWRSS.Enabled = cbWTWRSS.Checked;
            bnBrowseWTWRSS.Enabled = cbWTWRSS.Checked;

            txtWTWXML.Enabled = cbWTWXML.Checked;
            bnBrowseWTWXML.Enabled = cbWTWXML.Checked;

            bool wtw;
            if ((cbWTWRSS.Checked) || (cbWTWXML.Checked))
                wtw = true;
            else
                wtw = false;

            label4.Enabled = wtw;
            label15.Enabled = wtw;
            label16.Enabled = wtw;
            label17.Enabled = wtw;
            txtExportRSSMaxDays.Enabled = wtw;
            txtExportRSSMaxShows.Enabled = wtw;
            txtExportRSSDaysPast.Enabled = wtw;

            bool fo = cbFOXML.Checked;
            txtFOXML.Enabled = fo;
            bnBrowseFOXML.Enabled = fo;

            bool stxt = cbShowsTXT.Checked;
            txtShowsTXTTo.Enabled = stxt;
            bnBrowseShowsTXT.Enabled = stxt;

            bool shtml = cbShowsHTML.Checked;
            txtShowsHTMLTo.Enabled = shtml;
            bnBrowseShowsHTML.Enabled = shtml;

            bool ren = cbRenamingXML.Checked;
            txtRenamingXML.Enabled = ren;
            bnBrowseRenamingXML.Enabled = ren;

            bool misx = cbMissingXML.Checked;
            txtMissingXML.Enabled = misx;
            bnBrowseMissingXML.Enabled = misx;

            bool misc = cbMissingCSV.Checked;
            txtMissingCSV.Enabled = misc;
            bnBrowseMissingCSV.Enabled = misc;
        }

        private void bnAddSearchFolder_Click(object sender, EventArgs e)
        {
            int n = lbSearchFolders.SelectedIndex;
            folderBrowser.SelectedPath = n != -1 ? TVSettings.Instance.DownloadFolders[n] : "";

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                TVSettings.Instance.DownloadFolders.Add(folderBrowser.SelectedPath);
                mDoc.SetDirty();
            }

            FillSearchFolderList();
        }

        private void bnRemoveSearchFolder_Click(object sender, EventArgs e)
        {
            int n = lbSearchFolders.SelectedIndex;
            if (n == -1)
                return;

            TVSettings.Instance.DownloadFolders.RemoveAt(n);
            mDoc.SetDirty();

            FillSearchFolderList();
        }

        private void bnOpenSearchFolder_Click(object sender, EventArgs e)
        {
            int n = lbSearchFolders.SelectedIndex;
            if (n == -1)
                return;
            Helpers.SysOpen(TVSettings.Instance.DownloadFolders[n]);
        }

        private void FillSearchFolderList()
        {
            lbSearchFolders.Items.Clear();
            TVSettings.Instance.DownloadFolders.Sort();
            foreach (string efi in TVSettings.Instance.DownloadFolders)
                lbSearchFolders.Items.Add(efi);
        }

        private void lbSearchFolders_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                bnRemoveSearchFolder_Click(null, null);
        }

        private void lbSearchFolders_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = !e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.None : DragDropEffects.Copy;
        }

        private void lbSearchFolders_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[]) (e.Data.GetData(DataFormats.FileDrop));
            foreach (string path in files)
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    if (di.Exists)
                        TVSettings.Instance.DownloadFolders.Add(path.ToLower());
                }
                catch
                {
                }
            }
            mDoc.SetDirty();
            FillSearchFolderList();
        }

        private void bnRSSBrowseuTorrent_Click(object sender, EventArgs e)
        {
            openFile.FileName = txtRSSuTorrentPath.Text;
            openFile.Filter = "utorrent.exe|utorrent.exe|All Files (*.*)|*.*";
            if (openFile.ShowDialog() == DialogResult.OK)
                txtRSSuTorrentPath.Text = openFile.FileName;
        }

        private void bnUTBrowseResumeDat_Click(object sender, EventArgs e)
        {
            openFile.FileName = txtUTResumeDatPath.Text;
            openFile.Filter = "resume.dat|resume.dat|All Files (*.*)|*.*";
            if (openFile.ShowDialog() == DialogResult.OK)
                txtUTResumeDatPath.Text = openFile.FileName;
        }

        private void SetupReplacementsGrid()
        {
            SourceGrid.Cells.Views.Cell titleModel = new SourceGrid.Cells.Views.Cell
                                                         {
                                                             BackColor = Color.SteelBlue,
                                                             ForeColor = Color.White,
                                                             TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft
                                                         };

            ReplacementsGrid.Columns.Clear();
            ReplacementsGrid.Rows.Clear();

            ReplacementsGrid.RowsCount = 1;
            ReplacementsGrid.ColumnsCount = 3;
            ReplacementsGrid.FixedRows = 1;
            ReplacementsGrid.FixedColumns = 0;
            ReplacementsGrid.Selection.EnableMultiSelection = false;

            ReplacementsGrid.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.EnableStretch | SourceGrid.AutoSizeMode.EnableAutoSize;
            ReplacementsGrid.Columns[1].AutoSizeMode = SourceGrid.AutoSizeMode.EnableStretch | SourceGrid.AutoSizeMode.EnableAutoSize;
            ReplacementsGrid.Columns[2].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;

            ReplacementsGrid.Columns[2].Width = 80;

            ReplacementsGrid.AutoStretchColumnsToFitWidth = true;
            ReplacementsGrid.Columns.StretchToFit();

            ReplacementsGrid.Columns[0].Width = ReplacementsGrid.Columns[0].Width - 8; // allow for scrollbar
            ReplacementsGrid.Columns[1].Width = ReplacementsGrid.Columns[1].Width - 8;

            //////////////////////////////////////////////////////////////////////
            // header row

            ColumnHeader h;
            h = new ColumnHeader("Search");
            h.AutomaticSortEnabled = false;
            ReplacementsGrid[0, 0] = h;
            ReplacementsGrid[0, 0].View = titleModel;

            h = new ColumnHeader("Replace");
            h.AutomaticSortEnabled = false;
            ReplacementsGrid[0, 1] = h;
            ReplacementsGrid[0, 1].View = titleModel;

            h = new ColumnHeader("Case Ins.");
            h.AutomaticSortEnabled = false;
            ReplacementsGrid[0, 2] = h;
            ReplacementsGrid[0, 2].View = titleModel;
        }

        private void AddNewReplacementRow(string from, string to, bool ins)
        {
            SourceGrid.Cells.Views.Cell roModel = new SourceGrid.Cells.Views.Cell {ForeColor = Color.Gray};

            int r = ReplacementsGrid.RowsCount;
            ReplacementsGrid.RowsCount = r + 1;
            ReplacementsGrid[r, 0] = new SourceGrid.Cells.Cell(from, typeof(string));
            ReplacementsGrid[r, 1] = new SourceGrid.Cells.Cell(to, typeof(string));
            ReplacementsGrid[r, 2] = new SourceGrid.Cells.CheckBox(null, ins);
            if (!string.IsNullOrEmpty(from) && (TVSettings.CompulsoryReplacements().IndexOf(from) != -1))
            {
                ReplacementsGrid[r, 0].Editor.EnableEdit = false;
                ReplacementsGrid[r, 0].View = roModel;
            }
        }

        private void SetupRSSGrid()
        {
            SourceGrid.Cells.Views.Cell titleModel = new SourceGrid.Cells.Views.Cell
                                                         {
                                                             BackColor = Color.SteelBlue,
                                                             ForeColor = Color.White,
                                                             TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft
                                                         };

            RSSGrid.Columns.Clear();
            RSSGrid.Rows.Clear();

            RSSGrid.RowsCount = 1;
            RSSGrid.ColumnsCount = 1;
            RSSGrid.FixedRows = 1;
            RSSGrid.FixedColumns = 0;
            RSSGrid.Selection.EnableMultiSelection = false;

            RSSGrid.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;

            RSSGrid.AutoStretchColumnsToFitWidth = true;
            RSSGrid.Columns.StretchToFit();

            //////////////////////////////////////////////////////////////////////
            // header row

            ColumnHeader h = new ColumnHeader("URL");
            h.AutomaticSortEnabled = false;
            RSSGrid[0, 0] = h;
            RSSGrid[0, 0].View = titleModel;
        }

        private void AddNewRSSRow(string text)
        {
            int r = RSSGrid.RowsCount;
            RSSGrid.RowsCount = r + 1;
            RSSGrid[r, 0] = new SourceGrid.Cells.Cell(text, typeof(string));
        }

        private void bnRSSAdd_Click(object sender, EventArgs e)
        {
            AddNewRSSRow(null);
        }

        private void bnRSSRemove_Click(object sender, EventArgs e)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = RSSGrid.Selection.GetSelectionRegion().GetRowsIndex();
            if (rowsIndex.Length > 0)
                RSSGrid.Rows.Remove(rowsIndex[0]);
        }

        private void bnRSSGo_Click(object sender, EventArgs e)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = RSSGrid.Selection.GetSelectionRegion().GetRowsIndex();

            if (rowsIndex.Length > 0)
                Helpers.SysOpen((string) (RSSGrid[rowsIndex[0], 0].Value));
        }

        private void SetupLanguages()
        {
            cbLanguages.Items.Clear();
            cbLanguages.Items.Add("Please wait...");
            cbLanguages.SelectedIndex = 0;
            cbLanguages.Update();
            cbLanguages.Enabled = false;

            LoadLanguageThread = new Thread(LoadLanguage);
            LoadLanguageThread.Start();
        }

        private void LoadLanguage()
        {
            TheTVDB.Instance.GetLock ("Preferences-LoadLanguages");
            bool aborted = false;
            try
            {
                if (!TheTVDB.Instance.Connected)
                {
                    TheTVDB.Instance.Connect();
                }
            }
            catch (ThreadAbortException)
            {
                aborted = true;
            }
            catch (Exception e)
            {
                logger.Fatal(e,"Unhandled Exception in LoadLanguages");
                aborted = true;
            }
            TheTVDB.Instance.Unlock("Preferences-LoadLanguages");
            if (!aborted)
                BeginInvoke(LoadLanguageDone);
        }

        private void LoadLanguageDoneFunc()
        {
            FillLanguageList();
        }


        private void FillLanguageList()
        {
            TheTVDB.Instance.GetLock( "Preferences-FLL");
            cbLanguages.BeginUpdate();
            cbLanguages.Items.Clear();

            String pref = "";
            foreach (TheTVDB.Language l in TheTVDB.Instance.LanguageList)
            {
                cbLanguages.Items.Add(l.name);

                if (EnterPreferredLanguage == l.abbreviation)
                    pref = l.name;
            }
            cbLanguages.EndUpdate();
            cbLanguages.Text = pref;
            cbLanguages.Enabled = true;

            TheTVDB.Instance.Unlock("Preferences-FLL");
        }
        /*
        private void bnLangDown_Click(object sender, System.EventArgs e)
        {
            int n = this.lbLangs.SelectedIndex;
            if (n == -1)
                return;

            if (n < (this.LangList.Count - 1))
            {
                this.LangList.Reverse(n, 2);
                this.FillLanguageList();
                this.lbLangs.SelectedIndex = n + 1;
            }
        }

        private void bnLangUp_Click(object sender, System.EventArgs e)
        {
            int n = this.lbLangs.SelectedIndex;
            if (n == -1)
                return;
            if (n > 0)
            {
                this.LangList.Reverse(n - 1, 2);
                this.FillLanguageList();
                this.lbLangs.SelectedIndex = n - 1;
            }
        }*/

        private void cbMissing_CheckedChanged(object sender, EventArgs e)
        {
            ScanOptEnableDisable();
        }

        private void ScanOptEnableDisable()
        {
            bool e = cbMissing.Checked;
            tbMediaCenter.Enabled = e;

            cbSearchRSS.Enabled = e;
            cbSearchLocally.Enabled = e;
            cbCheckuTorrent.Enabled = e;

            bool e2 = e && cbSearchLocally.Checked;
            cbLeaveOriginals.Enabled = e2;
            cbCheckSABnzbd.Enabled = e2;
        }

        private void cbSearchLocally_CheckedChanged(object sender, EventArgs e)
        {
            ScanOptEnableDisable();
        }

        private void cbMeta_CheckedChanged(object sender, EventArgs e)
        {
            ScanOptEnableDisable();
        }

        private void bnReplaceAdd_Click(object sender, EventArgs e)
        {
            AddNewReplacementRow(null, null, false);
        }

        private void bnReplaceRemove_Click(object sender, EventArgs e)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = ReplacementsGrid.Selection.GetSelectionRegion().GetRowsIndex();
            if (rowsIndex.Length > 0)
            {
                // don't delete compulsory items
                int n = rowsIndex[0];
                string from = (string) (ReplacementsGrid[n, 0].Value);
                if (string.IsNullOrEmpty(from) || (TVSettings.CompulsoryReplacements().IndexOf(from) == -1))
                    ReplacementsGrid.Rows.Remove(n);
            }
        }

        private void btnAddShowStatusColoring_Click(object sender, EventArgs e)
        {
            if (cboShowStatus.SelectedItem != null && !string.IsNullOrEmpty(txtShowStatusColor.Text))
            {
                try
                {
                    ShowStatusColoringType ssct = cboShowStatus.SelectedItem as ShowStatusColoringType;
                    if (!ColorTranslator.FromHtml(txtShowStatusColor.Text).IsEmpty && ssct != null)
                    {
                        ListViewItem item = null;
                        item = lvwDefinedColors.FindItemWithText(ssct.Text);
                        if (item == null)
                        {
                            item = new ListViewItem();
                            item.SubItems.Add(txtShowStatusColor.Text);
                            lvwDefinedColors.Items.Add(item);
                        }

                        item.Text = ssct.Text;
                        item.SubItems[1].Text = txtShowStatusColor.Text;
                        item.ForeColor = ColorTranslator.FromHtml(txtShowStatusColor.Text);
                        item.Tag = ssct;
                        txtShowStatusColor.Text = string.Empty;
                        txtShowStatusColor.ForeColor = Color.Black;
                    }
                }
                catch { }
            }
        }

        private void btnSelectColor_Click(object sender, EventArgs e)
        {
            try
            {
                colorDialog.Color = ColorTranslator.FromHtml(txtShowStatusColor.Text);
            }
            catch
            {
                colorDialog.Color = Color.Black;
            }
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
            {
                txtShowStatusColor.Text =  TranslateColorToHtml(colorDialog.Color);
                txtShowStatusColor.ForeColor = colorDialog.Color;
            }
        }

        string TranslateColorToHtml(Color c)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B);
        }

        private void lvwDefinedColors_DoubleClick(object sender, EventArgs e)
        {
            RemoveSelectedDefinedColor();
        }

        private void bnRemoveDefinedColor_Click(object sender, EventArgs e)
        {
            RemoveSelectedDefinedColor();
        }

        private void lvwDefinedColors_SelectedIndexChanged(object sender, EventArgs e)
        {
            bnRemoveDefinedColor.Enabled = lvwDefinedColors.SelectedItems.Count == 1;
        }

        private void RemoveSelectedDefinedColor()
        {
            if (lvwDefinedColors.SelectedItems.Count == 1)
            {
                lvwDefinedColors.Items.Remove(lvwDefinedColors.SelectedItems[0]);
            }
        }

        private void txtShowStatusColor_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtShowStatusColor.ForeColor = ColorTranslator.FromHtml(txtShowStatusColor.Text);
            }
            catch
            {
                txtShowStatusColor.ForeColor = Color.Black;
            }
        }

        private void Preferences_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (LoadLanguageThread != null && LoadLanguageThread.IsAlive)
            {
                LoadLanguageThread.Abort();
                LoadLanguageThread.Join(500); // milliseconds timeout
            }
        }

        private void cmDefaults_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int v;
            if (e.ClickedItem == null || !(e.ClickedItem.Tag is String) || !int.TryParse(e.ClickedItem.Tag as String, out v))
                return;

            switch (v)
            {
                case 1: // KODI
                    cbEpTBNs.Checked = true;
                    cbNFOShows.Checked = true;
                    cbNFOEpisodes.Checked = true;
                    cbMeta.Checked = false;
                    cbMetaSubfolder.Checked = false;
                    cbSeriesJpg.Checked = false;
                    cbXMLFiles.Checked = false;
                    cbShrinkLarge.Checked = false;
                    cbFolderJpg.Checked = true;
                    rbFolderSeasonPoster.Checked = true;
                    cbEpThumbJpg.Checked = false;
                    cbFantArtJpg.Checked = false;
                    cbKODIImages.Checked = true;
                    rdBoth.Checked = true;
                    break;
                case 2: // pytivo
                    cbEpTBNs.Checked = false;
                    cbNFOShows.Checked = false;
                    cbNFOEpisodes.Checked = false;
                    cbMeta.Checked = true;
                    cbMetaSubfolder.Checked = true;
                    cbSeriesJpg.Checked = false;
                    cbXMLFiles.Checked = false;
                    cbShrinkLarge.Checked = false;
                    cbFolderJpg.Checked = true;
                    rbFolderPoster.Checked = true;
                    cbEpThumbJpg.Checked = false;
                    cbFantArtJpg.Checked = false;
                    cbKODIImages.Checked = false;
                    break;
                case 3: // mede8er
                    cbEpTBNs.Checked = false;
                    cbNFOShows.Checked = false;
                    cbNFOEpisodes.Checked = false;
                    cbMeta.Checked = false;
                    cbMetaSubfolder.Checked = false;
                    cbSeriesJpg.Checked = true;
                    cbXMLFiles.Checked = true;
                    cbShrinkLarge.Checked = true;
                    cbFolderJpg.Checked = true;
                    rbFolderSeasonPoster.Checked = true;
                    cbEpThumbJpg.Checked = false;
                    cbFantArtJpg.Checked = true;
                    cbKODIImages.Checked = false;
                    break;
                case 4: // none
                    cbEpTBNs.Checked = false;
                    cbNFOShows.Checked = false;
                    cbNFOEpisodes.Checked = false;
                    cbMeta.Checked = false;
                    cbMetaSubfolder.Checked = false;
                    cbSeriesJpg.Checked = false;
                    cbXMLFiles.Checked = false;
                    cbShrinkLarge.Checked = false;
                    cbFolderJpg.Checked = false;
                    rbFolderPoster.Checked = false;
                    cbEpThumbJpg.Checked = false;
                    cbFantArtJpg.Checked = false;
                    cbKODIImages.Checked = false;
                    break;
#if DEBUG
                default:
                    System.Diagnostics.Debug.Fail("Unknown default selected.");
                    break;
#endif
            }
        }

        private void bnMCPresets_Click(object sender, EventArgs e)
        {
            Point pt = PointToScreen(bnMCPresets.Location);
            cmDefaults.Show(pt);
        }

        private void cbKeepTogetherMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtKeepTogether.Enabled = (cbKeepTogether.Checked && cbKeepTogetherMode.Text != "All");
        }

        private void domainUpDown1_KeyDown(object sender, KeyEventArgs e)
        {
                e.SuppressKeyPress = true;
            
        }
    }
}
