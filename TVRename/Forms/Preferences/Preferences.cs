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
using System.Globalization;
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
        private delegate void LoadLanguageDoneDel();

        private readonly TVDoc mDoc;
        private Thread loadLanguageThread;
        private string enterPreferredLanguage; // hold here until background language download task is done
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private CustomNameTagsFloatingWindow cntfw;
        private readonly Season sampleSeason;

        private readonly LoadLanguageDoneDel loadLanguageDone;

        public Preferences(TVDoc doc, bool goToScanOpts, Season s)
        {
            sampleSeason = s;
            InitializeComponent();
            loadLanguageDone += LoadLanguageDoneFunc;

            SetupRssGrid();
            SetupReplacementsGrid();

            mDoc = doc;
            cntfw = null;

            if (goToScanOpts)
                tpSearch.SelectedTab = tpScanOptions;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (!TVSettings.OKExtensionsString(txtEmptyIgnoreExtensions.Text))
            {
                MessageBox.Show(
                    "Extensions list must be separated by semicolons, and each extension must start with a dot.",
                    "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tpSearch.SelectedTab = tbFolderDeleting;
                txtEmptyIgnoreExtensions.Focus();
                return;
            }

            if (!TVSettings.OKExtensionsString(txtVideoExtensions.Text))
            {
                MessageBox.Show(
                    "Extensions list must be separated by semicolons, and each extension must start with a dot.",
                    "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tpSearch.SelectedTab = tbFilesAndFolders;
                txtVideoExtensions.Focus();
                return;
            }
            if (!TVSettings.OKExtensionsString(txtSubtitleExtensions.Text))
            {
                MessageBox.Show(
                    "Extensions list must be separated by semicolons, and each extension must start with a dot.",
                    "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tpSearch.SelectedTab = tpSubtitles;
                txtSubtitleExtensions.Focus();
                return;
            }
            if (!TVSettings.OKExtensionsString(txtOtherExtensions.Text))
            {
                MessageBox.Show(
                    "Extensions list must be separated by semicolons, and each extension must start with a dot.",
                    "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tpSearch.SelectedTab = tbFilesAndFolders;
                txtOtherExtensions.Focus();
                return;
            }
            if (!TVSettings.OKExtensionsString(txtKeepTogether.Text))
            {
                MessageBox.Show(
                    "Extensions list must be separated by semicolons, and each extension must start with a dot.",
                    "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tpSearch.SelectedTab = tbFilesAndFolders;
                txtKeepTogether.Focus();
                return;
            }

            TVSettings s = TVSettings.Instance;

            s.Replacements.Clear();
            for (int i = 1; i < ReplacementsGrid.RowsCount; i++)
            {
                string from = (string) (ReplacementsGrid[i, 0].Value);
                string to = (string) (ReplacementsGrid[i, 1].Value);
                bool ins = (bool) (ReplacementsGrid[i, 2].Value);
                if (!string.IsNullOrEmpty(from))
                    s.Replacements.Add(new Replacement(from, to, ins));
            }

            s.ExportWTWRSS = cbWTWRSS.Checked;
            s.ExportWTWRSSTo = txtWTWRSS.Text;
            s.ExportWTWXML = cbWTWXML.Checked;
            s.ExportWTWXMLTo = txtWTWXML.Text;
            s.ExportWTWICAL = cbWTWICAL.Checked;
            s.ExportWTWICALTo = txtWTWICAL.Text;
            s.ExportMissingXML = cbMissingXML.Checked;
            s.ExportMissingXMLTo = txtMissingXML.Text;
            s.ExportMissingCSV = cbMissingCSV.Checked;
            s.ExportMissingCSVTo = txtMissingCSV.Text;
            s.ExportRenamingXML = cbRenamingXML.Checked;
            s.ExportRenamingXMLTo = txtRenamingXML.Text;
            s.ExportFOXML = cbFOXML.Checked;
            s.ExportFOXMLTo = txtFOXML.Text;
            s.ExportShowsTXT = cbShowsTXT.Checked;
            s.ExportShowsTXTTo = txtShowsTXTTo.Text;
            s.ExportShowsHTML = cbShowsHTML.Checked;
            s.ExportShowsHTMLTo = txtShowsHTMLTo.Text;

            s.WTWRecentDays = Convert.ToInt32(txtWTWDays.Text);
            s.StartupTab = cbStartupTab.SelectedIndex;
            s.NotificationAreaIcon = cbNotificationIcon.Checked;
            s.VideoExtensionsString = txtVideoExtensions.Text;
            s.OtherExtensionsString = txtOtherExtensions.Text;
            s.subtitleExtensionsString = txtSubtitleExtensions.Text;
            s.ExportRSSMaxDays = Convert.ToInt32(txtExportRSSMaxDays.Text);
            s.ExportRSSMaxShows = Convert.ToInt32(txtExportRSSMaxShows.Text);
            s.ExportRSSDaysPast = Convert.ToInt32(txtExportRSSDaysPast.Text);
            s.KeepTogether = cbKeepTogether.Checked;
            s.LeadingZeroOnSeason = cbLeadingZero.Checked;
            s.ShowInTaskbar = chkShowInTaskbar.Checked;
            s.RenameTxtToSub = cbTxtToSub.Checked;
            s.ShowEpisodePictures = cbShowEpisodePictures.Checked;
            s.HideMyShowsSpoilers = chkHideMyShowsSpoilers.Checked;
            s.HideWtWSpoilers = chkHideWtWSpoilers.Checked;
            s.AutoSelectShowInMyShows = cbAutoSelInMyShows.Checked;
            s.AutoCreateFolders = cbAutoCreateFolders.Checked ;  
            s.SpecialsFolderName = txtSpecialsFolderName.Text;
            s.SeasonFolderFormat = txtSeasonFormat.Text;
            s.searchSeasonWordsString = tbSeasonSearchTerms.Text;
            s.preferredRSSSearchTermsString = tbPreferredRSSTerms.Text;
            s.defaultSeasonWord = txtSeasonFolderName.Text;
            s.keepTogetherExtensionsString = txtKeepTogether.Text;
            s.ForceLowercaseFilenames = cbForceLower.Checked;
            s.IgnoreSamples = cbIgnoreSamples.Checked;
            s.uTorrentPath = txtRSSuTorrentPath.Text;
            s.ResumeDatPath = txtUTResumeDatPath.Text;
            s.SABHostPort = txtSABHostPort.Text;
            s.SABAPIKey = txtSABAPIKey.Text;
            s.CheckSABnzbd = cbCheckSABnzbd.Checked;
            s.qBitTorrentHost= tbqBitTorrentHost.Text;
            s.qBitTorrentPort= tbqBitTorrentPort.Text;
            s.CheckqBitTorrent = cbCheckqBitTorrent.Checked;
            s.SearchRSS = cbSearchRSS.Checked;
            s.EpTBNs = cbEpTBNs.Checked;
            s.NFOShows = cbNFOShows.Checked;
            s.NFOEpisodes = cbNFOEpisodes.Checked;
            s.KODIImages = cbKODIImages.Checked;
            s.pyTivoMeta = cbMeta.Checked;
            s.pyTivoMetaSubFolder = cbMetaSubfolder.Checked;
            s.wdLiveTvMeta = cbWDLiveEpisodeFiles.Checked;
            s.FolderJpg = cbFolderJpg.Checked;
            s.RenameCheck = cbRenameCheck.Checked;
            s.PreventMove = chkPreventMove.Checked;
            s.MissingCheck = cbMissing.Checked;
            s.CorrectFileDates = cbxUpdateAirDate.Checked;
            s.SearchLocally = cbSearchLocally.Checked;
            s.AutoSearchForDownloadedFiles = chkAutoSearchForDownloadedFiles.Checked;
            s.LeaveOriginals = cbLeaveOriginals.Checked;
            s.CheckuTorrent = cbCheckuTorrent.Checked;
            s.LookForDateInFilename = cbLookForAirdate.Checked;
            s.AutoMergeDownloadEpisodes = chkAutoMergeDownloadEpisodes.Checked;
            s.AutoMergeLibraryEpisodes = chkAutoMergeLibraryEpisodes.Checked;
            s.RetainLanguageSpecificSubtitles = chkRetainLanguageSpecificSubtitles.Checked;
            s.ForceBulkAddToUseSettingsOnly = chkForceBulkAddToUseSettingsOnly.Checked;

            s.SearchJSON = cbSearchJSON.Checked;
            s.SearchJSONURL = tbJSONURL.Text;
            s.SearchJSONRootNode = tbJSONRootNode.Text;
            s.SearchJSONFilenameToken = tbJSONFilenameToken.Text;
            s.SearchJSONURLToken = tbJSONURLToken.Text;

            s.MonitorFolders = cbMonitorFolder.Checked;
            s.runStartupCheck = chkScanOnStartup.Checked;
            s.runPeriodicCheck = chkScheduledScan.Checked;
            s.periodCheckHours = int.Parse(domainUpDown1.SelectedItem?.ToString()??"1");
            s.RemoveDownloadDirectoriesFiles = cbCleanUpDownloadDir.Checked;

            s.EpJPGs = cbEpThumbJpg.Checked;
            s.SeriesJpg = cbSeriesJpg.Checked;
            s.Mede8erXML = cbXMLFiles.Checked;
            s.ShrinkLargeMede8erImages = cbShrinkLarge.Checked;
            s.FanArtJpg = cbFantArtJpg.Checked;

            s.Tidyup.DeleteEmpty = cbDeleteEmpty.Checked;
            s.Tidyup.DeleteEmptyIsRecycle = cbRecycleNotDelete.Checked;
            s.Tidyup.EmptyIgnoreWords = cbEmptyIgnoreWords.Checked;
            s.Tidyup.EmptyIgnoreWordList = txtEmptyIgnoreWords.Text;
            s.Tidyup.EmptyIgnoreExtensions = cbEmptyIgnoreExtensions.Checked;
            s.Tidyup.EmptyIgnoreExtensionList = txtEmptyIgnoreExtensions.Text;
            s.Tidyup.EmptyMaxSizeCheck = cbEmptyMaxSize.Checked;
            int.TryParse(txtEmptyMaxSize.Text, out s.Tidyup.EmptyMaxSizeMB);

            s.BulkAddCompareNoVideoFolders = cbIgnoreNoVideoFolders.Checked;
            s.BulkAddIgnoreRecycleBin = cbIgnoreRecycleBin.Checked;
            s.AutoAddIgnoreSuffixes = tbIgnoreSuffixes.Text;
            s.AutoAddMovieTerms = tbMovieTerms.Text;

            if (rbFolderFanArt.Checked)
                s.FolderJpgIs = TVSettings.FolderJpgIsType.FanArt;
            else if (rbFolderBanner.Checked)
                s.FolderJpgIs = TVSettings.FolderJpgIsType.Banner;
            else if (rbFolderSeasonPoster.Checked)
                s.FolderJpgIs = TVSettings.FolderJpgIsType.SeasonPoster;
            else
                s.FolderJpgIs = TVSettings.FolderJpgIsType.Poster;

            if (rdoQuickScan.Checked)
                s.MonitoredFoldersScanType = TVSettings.ScanType.Quick;
            else if (rdoRecentScan.Checked)
                s.MonitoredFoldersScanType = TVSettings.ScanType.Recent;
            else
                s.MonitoredFoldersScanType = TVSettings.ScanType.Full;

            s.mode = cbMode.Text == "Beta" ? TVSettings.BetaMode.BetaToo : TVSettings.BetaMode.ProductionOnly;

            if (cbKeepTogetherMode.Text == "All but these")
            {
                s.keepTogetherMode = TVSettings.KeepTogetherModes.AllBut;
            } else if (cbKeepTogetherMode.Text == "Just")

            {
                s.keepTogetherMode = TVSettings.KeepTogetherModes.Just;
            }
            else
                s.keepTogetherMode = TVSettings.KeepTogetherModes.All;


            TheTVDB.Instance.GetLock("Preferences-OK");
            foreach (Language l in TheTVDB.Instance.LanguageList)
            {
                if (l.Name == cbLanguages.Text)
                {
                    s.PreferredLanguageCode = l.Abbreviation;
                    break;
                }
            }
            s.WTWDoubleClick = rbWTWScan.Checked ? TVSettings.WTWDoubleClickAction.Scan : TVSettings.WTWDoubleClickAction.Search;

            TheTVDB.Instance.SaveCache();
            TheTVDB.Instance.Unlock("Preferences-OK");

            try
            {
                s.SampleFileMaxSizeMB = int.Parse(txtMaxSampleSize.Text);
            }
            catch
            {
                s.SampleFileMaxSizeMB = 50;
            }

            try
            {
                s.upgradeDirtyPercent = float.Parse(tbPercentDirty.Text);
            }
            catch
            {
                s.upgradeDirtyPercent = 20;
            }
            if (s.upgradeDirtyPercent < 1)
                s.upgradeDirtyPercent = 1;
            else if (s.upgradeDirtyPercent > 100)
                s.upgradeDirtyPercent = 100;


            try
            {
                s.ParallelDownloads = int.Parse(txtParallelDownloads.Text);
            }
            catch
            {
                s.ParallelDownloads = 4;
            }

            if (s.ParallelDownloads < 1)
                s.ParallelDownloads = 1;
            else if (s.ParallelDownloads > 8)
                s.ParallelDownloads = 8;

            // RSS URLs
            s.RSSURLs.Clear();
            for (int i = 1; i < RSSGrid.RowsCount; i++)
            {
                string url = (string) (RSSGrid[i, 0].Value);
                if (!string.IsNullOrEmpty(url))
                    s.RSSURLs.Add(url);
            }

            s.ShowStatusColors = new ShowStatusColoringTypeList();
            foreach (ListViewItem item in lvwDefinedColors.Items)
            {
                if (item.SubItems.Count > 1 && !string.IsNullOrEmpty(item.SubItems[1].Text) && item.Tag != null &&
                    item.Tag is ShowStatusColoringType type)
                {
                    s.ShowStatusColors.Add(type,ColorTranslator.FromHtml(item.SubItems[1].Text));
                }
            }

            mDoc.SetDirty();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Preferences_Load(object sender, EventArgs e)
        {
            SetupLanguages();

            TVSettings s = TVSettings.Instance;

            foreach (Replacement rep in s.Replacements)
            {
                AddNewReplacementRow(rep.This, rep.That, rep.CaseInsensitive);
            }

            txtMaxSampleSize.Text = s.SampleFileMaxSizeMB.ToString();

            cbWTWRSS.Checked = s.ExportWTWRSS;
            txtWTWRSS.Text = s.ExportWTWRSSTo;
            cbWTWICAL.Checked = s.ExportWTWICAL;
            txtWTWICAL.Text = s.ExportWTWICALTo;
            txtWTWDays.Text = s.WTWRecentDays.ToString();
            cbWTWXML.Checked = s.ExportWTWXML;
            txtWTWXML.Text = s.ExportWTWXMLTo;
            txtExportRSSMaxDays.Text = s.ExportRSSMaxDays.ToString();
            txtExportRSSMaxShows.Text = s.ExportRSSMaxShows.ToString();
            txtExportRSSDaysPast.Text = s.ExportRSSDaysPast.ToString();

            cbMissingXML.Checked = s.ExportMissingXML;
            txtMissingXML.Text = s.ExportMissingXMLTo;
            cbMissingCSV.Checked = s.ExportMissingCSV;
            txtMissingCSV.Text = s.ExportMissingCSVTo;

            cbShowsTXT.Checked = s.ExportShowsTXT ;
            txtShowsTXTTo.Text = s.ExportShowsTXTTo;
            cbShowsHTML.Checked = s.ExportShowsHTML;
            txtShowsHTMLTo.Text = s.ExportShowsHTMLTo;

            cbRenamingXML.Checked = s.ExportRenamingXML;
            txtRenamingXML.Text = s.ExportRenamingXMLTo;

            cbFOXML.Checked = s.ExportFOXML;
            txtFOXML.Text = s.ExportFOXMLTo;

            cbStartupTab.SelectedIndex = s.StartupTab;
            cbNotificationIcon.Checked = s.NotificationAreaIcon;
            txtVideoExtensions.Text = s.GetVideoExtensionsString();
            txtOtherExtensions.Text = s.GetOtherExtensionsString();
            txtSubtitleExtensions.Text = s.subtitleExtensionsString;
            txtKeepTogether.Text = s.GetKeepTogetherString();
            tbSeasonSearchTerms.Text = s.GetSeasonSearchTermsString();
            tbPreferredRSSTerms .Text = s.GetPreferredRSSSearchTermsString();

            cbKeepTogether.Checked = s.KeepTogether;
            cbKeepTogether_CheckedChanged(null, null);

            cbLeadingZero.Checked = s.LeadingZeroOnSeason;
            chkShowInTaskbar.Checked = s.ShowInTaskbar;
            cbTxtToSub.Checked = s.RenameTxtToSub;
            cbShowEpisodePictures.Checked = s.ShowEpisodePictures;
            chkHideMyShowsSpoilers.Checked = s.HideMyShowsSpoilers;
            chkHideWtWSpoilers.Checked = s.HideWtWSpoilers;
            cbAutoCreateFolders.Checked = s.AutoCreateFolders; 
            cbAutoSelInMyShows.Checked = s.AutoSelectShowInMyShows;
            txtSpecialsFolderName.Text = s.SpecialsFolderName;
            txtSeasonFormat.Text= s.SeasonFolderFormat ;
            cbForceLower.Checked = s.ForceLowercaseFilenames;
            cbIgnoreSamples.Checked = s.IgnoreSamples;
            txtRSSuTorrentPath.Text = s.uTorrentPath;
            txtUTResumeDatPath.Text = s.ResumeDatPath;
            txtSABHostPort.Text = s.SABHostPort;
            txtSABAPIKey.Text = s.SABAPIKey;
            tbqBitTorrentHost.Text = s.qBitTorrentHost;
            tbqBitTorrentPort.Text = s.qBitTorrentPort;
            cbCheckqBitTorrent.Checked= s.CheckqBitTorrent ;
            cbCheckSABnzbd.Checked = s.CheckSABnzbd;

            txtParallelDownloads.Text = s.ParallelDownloads.ToString();
            tbPercentDirty.Text = s.upgradeDirtyPercent.ToString(CultureInfo.InvariantCulture);

            cbSearchJSON.Checked = s.SearchJSON;
            tbJSONURL.Text = s.SearchJSONURL;
            tbJSONRootNode.Text = s.SearchJSONRootNode;
            tbJSONFilenameToken.Text = s.SearchJSONFilenameToken;
            tbJSONURLToken.Text = s.SearchJSONURLToken;

            cbSearchRSS.Checked = s.SearchRSS;
            cbEpTBNs.Checked = s.EpTBNs;
            cbWDLiveEpisodeFiles.Checked= s.wdLiveTvMeta ;
            cbNFOShows.Checked = s.NFOShows;
            cbNFOEpisodes.Checked = s.NFOEpisodes;
            cbKODIImages.Checked = s.KODIImages;
            cbMeta.Checked = s.pyTivoMeta;
            cbMetaSubfolder.Checked = s.pyTivoMetaSubFolder;
            cbFolderJpg.Checked = s.FolderJpg;
            cbRenameCheck.Checked = s.RenameCheck;
            chkPreventMove.Checked = s.PreventMove;
            cbCheckuTorrent.Checked = s.CheckuTorrent;
            cbLookForAirdate.Checked = s.LookForDateInFilename;
            chkRetainLanguageSpecificSubtitles.Checked = s.RetainLanguageSpecificSubtitles;
            chkForceBulkAddToUseSettingsOnly.Checked = s.ForceBulkAddToUseSettingsOnly;
            chkAutoMergeDownloadEpisodes.Checked = s.AutoMergeDownloadEpisodes;
            chkAutoMergeLibraryEpisodes.Checked = s.AutoMergeLibraryEpisodes;
            cbMonitorFolder.Checked = s.MonitorFolders;
            chkScheduledScan.Checked = s.RunPeriodicCheck();
            chkScanOnStartup.Checked = s.RunOnStartUp();
            domainUpDown1.SelectedItem = s.periodCheckHours;
            cbCleanUpDownloadDir.Checked = s.RemoveDownloadDirectoriesFiles;
            cbMissing.Checked = s.MissingCheck;
            cbxUpdateAirDate.Checked = s.CorrectFileDates;
            chkAutoSearchForDownloadedFiles.Checked = s.AutoSearchForDownloadedFiles;
            cbSearchLocally.Checked = s.SearchLocally;
            cbLeaveOriginals.Checked = s.LeaveOriginals;
            enterPreferredLanguage = s.PreferredLanguageCode;

            cbEpThumbJpg.Checked = s.EpJPGs;
            cbSeriesJpg.Checked = s.SeriesJpg;
            cbXMLFiles.Checked = s.Mede8erXML;
            cbShrinkLarge.Checked = s.ShrinkLargeMede8erImages;
            cbFantArtJpg.Checked = s.FanArtJpg;

#if DEBUG
            System.Diagnostics.Debug.Assert(s.Tidyup != null);
#endif
            cbDeleteEmpty.Checked = s.Tidyup.DeleteEmpty;
            cbRecycleNotDelete.Checked = s.Tidyup.DeleteEmptyIsRecycle;
            cbEmptyIgnoreWords.Checked = s.Tidyup.EmptyIgnoreWords;
            txtEmptyIgnoreWords.Text = s.Tidyup.EmptyIgnoreWordList;
            cbEmptyIgnoreExtensions.Checked = s.Tidyup.EmptyIgnoreExtensions;
            txtEmptyIgnoreExtensions.Text = s.Tidyup.EmptyIgnoreExtensionList;
            cbEmptyMaxSize.Checked = s.Tidyup.EmptyMaxSizeCheck;
            txtEmptyMaxSize.Text = s.Tidyup.EmptyMaxSizeMB.ToString();
            txtSeasonFolderName.Text = s.defaultSeasonWord;
            
            cbIgnoreRecycleBin.Checked = s.BulkAddIgnoreRecycleBin;
            cbIgnoreNoVideoFolders.Checked = s.BulkAddCompareNoVideoFolders;
            tbMovieTerms.Text = s.AutoAddMovieTerms;
            tbIgnoreSuffixes.Text = s.AutoAddIgnoreSuffixes;

            switch (s.WTWDoubleClick)
            {
                case TVSettings.WTWDoubleClickAction.Search:
                default:
                    rbWTWSearch.Checked = true;
                    break;
                case TVSettings.WTWDoubleClickAction.Scan:
                    rbWTWScan.Checked = true;
                    break;
            }
            switch(s.keepTogetherMode)
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

            switch (s.mode)
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

            foreach (string row in s.RSSURLs)
                AddNewRssRow(row);

            switch (s.FolderJpgIs)
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

            switch (s.MonitoredFoldersScanType)
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

            if (s.ShowStatusColors != null)
            {
                foreach (
                    System.Collections.Generic.KeyValuePair<ShowStatusColoringType, Color> showStatusColor in
                        s.ShowStatusColors)
                {
                    ListViewItem item = new ListViewItem
                    {
                        Text = showStatusColor.Key.Text,
                        Tag = showStatusColor.Key,
                        ForeColor = showStatusColor.Value
                    };
                    item.SubItems.Add(Helpers.TranslateColorToHtml(showStatusColor.Value));
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
                cboShowStatus.Items.Add(t);
            }
            // Seasons
            foreach (string status in Enum.GetNames(typeof(Season.SeasonStatus)))
            {
                ShowStatusColoringType t = new ShowStatusColoringType(true, false, status);
                cboShowStatus.Items.Add(t);
            }
            cboShowStatus.DisplayMember = "Text";
        }

        private void Browse(TextBox txt, string defaultExt, int filterIndex)
        {
            //rss =1, XML = 2, CSV = 3, TXT=4, HTML = 5
            saveFile.FileName = txt.Text;
            saveFile.DefaultExt = defaultExt;
            saveFile.FilterIndex = filterIndex;
            if (saveFile.ShowDialog() == DialogResult.OK)
                txt.Text = saveFile.FileName;
        }

        private void bnBrowseWTWXML_Click(object sender, EventArgs e)
        {
            Browse(txtWTWXML,"xml",2);
        }

        private void TxtNumberOnlyKeyPress(object sender, KeyPressEventArgs e)
        {
            // digits only
            if ((e.KeyChar >= 32) && (!char.IsDigit(e.KeyChar)))
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

            txtWTWICAL.Enabled = cbWTWICAL.Checked;
            bnBrowseWTWICAL.Enabled = cbWTWICAL.Checked;

            bool wtw;
            if ((cbWTWRSS.Checked) || (cbWTWXML.Checked) || (cbWTWICAL.Checked))
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
                    // ignored
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

            ColumnHeader h = new ColumnHeader("Search") {AutomaticSortEnabled = false};
            ReplacementsGrid[0, 0] = h;
            ReplacementsGrid[0, 0].View = titleModel;

            h = new ColumnHeader("Replace") {AutomaticSortEnabled = false};
            ReplacementsGrid[0, 1] = h;
            ReplacementsGrid[0, 1].View = titleModel;

            h = new ColumnHeader("Case Ins.") {AutomaticSortEnabled = false};
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
            if (!string.IsNullOrEmpty(from) && (TVSettings.CompulsoryReplacements().IndexOf(from, StringComparison.Ordinal) != -1))
            {
                ReplacementsGrid[r, 0].Editor.EnableEdit = false;
                ReplacementsGrid[r, 0].View = roModel;
            }
        }

        private void SetupRssGrid()
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
            ColumnHeader h = new ColumnHeader("URL") {AutomaticSortEnabled = false};
            RSSGrid[0, 0] = h;
            RSSGrid[0, 0].View = titleModel;
        }

        private void AddNewRssRow(string text)
        {
            int r = RSSGrid.RowsCount;
            RSSGrid.RowsCount = r + 1;
            RSSGrid[r, 0] = new SourceGrid.Cells.Cell(text, typeof(string));
        }

        private void bnRSSAdd_Click(object sender, EventArgs e)
        {
            AddNewRssRow(null);
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

            loadLanguageThread = new Thread(LoadLanguage);
            loadLanguageThread.Start();
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
                Logger.Fatal(e,"Unhandled Exception in LoadLanguages");
                aborted = true;
            }
            TheTVDB.Instance.Unlock("Preferences-LoadLanguages");
            if (!aborted)
                BeginInvoke(loadLanguageDone);
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

            string pref = "";
            foreach (Language l in TheTVDB.Instance.LanguageList)
            {
                cbLanguages.Items.Add(l.Name);

                if (enterPreferredLanguage == l.Abbreviation)
                    pref = l.Name;
            }
            cbLanguages.EndUpdate();
            cbLanguages.Text = pref;
            cbLanguages.Enabled = true;

            TheTVDB.Instance.Unlock("Preferences-FLL");
        }

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
                if (string.IsNullOrEmpty(from) || (TVSettings.CompulsoryReplacements().IndexOf(from, StringComparison.Ordinal) == -1))
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
                        ListViewItem item = lvwDefinedColors.FindItemWithText(ssct.Text);
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
                catch
                {
                    // ignored
                }
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
                txtShowStatusColor.Text =  Helpers.TranslateColorToHtml(colorDialog.Color);
                txtShowStatusColor.ForeColor = colorDialog.Color;
            }
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
            if (loadLanguageThread != null && loadLanguageThread.IsAlive)
            {
                loadLanguageThread.Abort();
                loadLanguageThread.Join(500); // milliseconds timeout
            }
        }

        private void cmDefaults_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (!(e.ClickedItem?.Tag is string) || !int.TryParse((string) e.ClickedItem.Tag, out int v))
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
                default:
                    System.Diagnostics.Debug.Fail("Unknown default selected.");
                    break;
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

        private void bnBrowseWTWICAL_Click(object sender, EventArgs e)
        {
            Browse(txtWTWICAL, "iCal", 6);
        }

        private void bnTags_Click(object sender, EventArgs e)
        {
            cntfw = new CustomNameTagsFloatingWindow(sampleSeason);
            cntfw.Show(this);
            Focus();
        }

        private void cbSearchRSS_CheckedChanged(object sender, EventArgs e)
        {
            gbRSS.Enabled = cbSearchRSS.Checked;
        }

        private void cbSearchJSON_CheckedChanged(object sender, EventArgs e)
        {
            gbJSON.Enabled = cbSearchJSON.Checked;
        }
    }
}
