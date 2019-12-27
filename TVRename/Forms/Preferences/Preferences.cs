// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using Alphaleonis.Win32.Filesystem;
using DaveChambers.FolderBrowserDialogEx;
using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using JetBrains.Annotations;
using ColumnHeader = SourceGrid.Cells.ColumnHeader;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;

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

        private class FailedValidationException : Exception
        {
        }

        public Preferences(TVDoc doc, bool goToScanOpts, Season s)
        {
            sampleSeason = s;
            InitializeComponent();
            loadLanguageDone += LoadLanguageDoneFunc;

            SetupTimezoneDropdown();
            SetupRssGrid();
            SetupReplacementsGrid();
            FillFolderStringLists();

            mDoc = doc;
            cntfw = null;

            if (goToScanOpts)
            {
                tcTabs.SelectedTab = tbGeneral;
            }
        }

        private void SetupTimezoneDropdown()
        {
            cbTimeZone.BeginUpdate();
            cbTimeZone.Items.Clear();
            foreach (string s in TimeZoneHelper.ZoneNames())
            {
                cbTimeZone.Items.Add(s);
            }

            cbTimeZone.EndUpdate();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            try
            {
                ValidateForm();
            }
            catch (FailedValidationException)
            {
                return;
            }

            UpdateSettings();

            mDoc.SetDirty();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ValidateForm()
        {
            ValidateFileExtensions();
            ValidateExporterLocations();
            ValidateFilePaths();
        }

        private void ValidateFilePaths()
        {
            ValidateFilePath(txtSpecialsFolderName, tpLibraryFolders,true);
            ValidateFilePath(txtSeasonFormat, tpLibraryFolders, true);
            if (cbCheckuTorrent.Checked)
            {
                ValidateFilePath(txtUTResumeDatPath, tbuTorrentNZB,false);
                ValidateFilePath(txtRSSuTorrentPath, tbuTorrentNZB,false);
            }
        }

        private void ValidateFilePath([NotNull] TextBox validationField,TabPage errorPage,bool emptyOk)
        {
            if (TVSettings.OKPath(validationField.Text,emptyOk))
            {
                return;
            }

            MessageBox.Show(
                "Please check that the proposed location/path is a valid one and has no invalid characters",
                "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            tcTabs.SelectedTab = errorPage;
            validationField.Focus();
            throw new FailedValidationException();
        }

        private void ValidateFileExtensions()
        {
            ValidateExtensions(txtEmptyIgnoreExtensions, tbFolderDeleting);
            ValidateExtensions(txtVideoExtensions, tbFilesAndFolders);
            ValidateExtensions(txtSubtitleExtensions, tbFilesAndFolders);
            ValidateExtensions(txtOtherExtensions, tbFilesAndFolders);
            ValidateExtensions(txtKeepTogether, tbFilesAndFolders);
        }

        private void ValidateExporterLocations()
        {
            ValidateExporterLocation(cbWTWRSS, txtWTWRSS);
            ValidateExporterLocation(cbWTWXML, txtWTWXML);
            ValidateExporterLocation(cbWTWICAL, txtWTWICAL);

            ValidateExporterLocation(cbMissingXML , txtMissingXML);
            ValidateExporterLocation(cbMissingCSV, txtMissingCSV);

            ValidateExporterLocation(cbShowsTXT, txtShowsTXTTo);
            ValidateExporterLocation(cbShowsHTML, txtShowsHTMLTo);

            ValidateExporterLocation(cbRenamingXML, txtRenamingXML);
            ValidateExporterLocation(cbFOXML, txtFOXML);

            ValidateExporterLocation(cbXSPF, txtXSPF);
            ValidateExporterLocation(cbM3U, txtM3U);
            ValidateExporterLocation(cbASX, txtASX);
            ValidateExporterLocation(cbWPL, txtWPL);
        }

        private void ValidateExporterLocation([NotNull] CheckBox controlCheckbox,TextBox validationField)
        {
            if (!controlCheckbox.Checked)
            {
                return;
            }

            if (TVSettings.OKExporterLocation(validationField.Text))
            {
                return;
            }

            MessageBox.Show(
                "Exporters can only export to the local filesystem and must be a valid file/folder name",
                "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            tcTabs.SelectedTab = tbAutoExport;
            validationField.Focus();
            throw new FailedValidationException();
        }

        private void ValidateExtensions([NotNull] Control validateField, TabPage focusTabPage)
        {
            if (TVSettings.OKExtensionsString(validateField.Text))
            {
                return;
            }

            MessageBox.Show(
                "Extensions list must be separated by semicolons, and each extension must start with a dot.",
                "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            tcTabs.SelectedTab = focusTabPage;
            validateField.Focus();
            throw new FailedValidationException();
        }

        #region Update Settings

        // ReSharper disable once FunctionComplexityOverflow
        private void UpdateSettings()
        {
            TVSettings s = TVSettings.Instance;

            UpdateReplacement(s);

            s.RSSUseCloudflare = cbRSSCloudflareProtection.Checked;
            s.SearchJSONUseCloudflare = cbJSONCloudflareProtection.Checked;
            s.qBitTorrentDownloadFilesFirst = cbDownloadTorrentBeforeDownloading.Checked;
            s.ShowBasicShowDetails = chkBasicShowDetails.Checked;
            s.DetailedRSSJSONLogging = cbDetailedRSSJSONLogging.Checked;
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
            s.UseColoursOnWtw = cbUseColoursOnWtw.Checked;

            s.ExportRecentM3U = cbM3U.Checked;
            s.ExportRecentM3UTo = txtM3U.Text;
            s.ExportRecentASX = cbASX.Checked;
            s.ExportRecentASXTo = txtASX.Text;
            s.ExportRecentXSPF = cbXSPF.Checked;
            s.ExportRecentXSPFTo = txtXSPF.Text;
            s.ExportRecentWPL = cbWPL.Checked;
            s.ExportRecentWPLTo = txtWPL.Text;

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
            s.ReplaceWithBetterQuality = cbHigherQuality.Checked;
            s.HideMyShowsSpoilers = chkHideMyShowsSpoilers.Checked;
            s.HideWtWSpoilers = chkHideWtWSpoilers.Checked;
            s.AutoSelectShowInMyShows = cbAutoSelInMyShows.Checked;
            s.AutoCreateFolders = cbAutoCreateFolders.Checked;
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
            s.qBitTorrentHost = tbqBitTorrentHost.Text;
            s.qBitTorrentPort = tbqBitTorrentPort.Text;
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
            s.MoveLibraryFiles = chkMoveLibraryFiles.Checked;
            s.CorrectFileDates = cbxUpdateAirDate.Checked;
            s.SearchLocally = cbSearchLocally.Checked;
            s.IgnorePreviouslySeen = cbIgnorePreviouslySeen.Checked;
            s.AutoSearchForDownloadedFiles = chkAutoSearchForDownloadedFiles.Checked;
            s.LeaveOriginals = cbLeaveOriginals.Checked;
            s.CheckuTorrent = cbCheckuTorrent.Checked;
            s.LookForDateInFilename = cbLookForAirdate.Checked;
            s.AutoMergeDownloadEpisodes = chkAutoMergeDownloadEpisodes.Checked;
            s.AutoMergeLibraryEpisodes = chkAutoMergeLibraryEpisodes.Checked;
            s.RetainLanguageSpecificSubtitles = chkRetainLanguageSpecificSubtitles.Checked;
            s.ForceBulkAddToUseSettingsOnly = chkForceBulkAddToUseSettingsOnly.Checked;
            s.CopyFutureDatedEpsFromSearchFolders = cbCopyFutureDatedEps.Checked;
            s.ShareLogs = chkShareCriticalLogs.Checked;
            s.PostpendThe = chkPostpendThe.Checked;
            s.UseFullPathNameToMatchLibraryFolders = chkUseLibraryFullPathWhenMatchingShows.Checked;
            s.UseFullPathNameToMatchSearchFolders = chkUseSearchFullPathWhenMatchingShows.Checked;
            s.AutoAddAsPartOfQuickRename = chkAutoAddAsPartOfQuickRename.Checked;
            s.CleanLibraryAfterActions = chkCleanLibraryAfterActions.Checked;

            s.SearchJSON = cbSearchJSON.Checked;
            s.SearchJSONManualScanOnly = cbSearchJSONManualScanOnly.Checked;
            s.SearchJSONURL = tbJSONURL.Text;
            s.SearchJSONRootNode = tbJSONRootNode.Text;
            s.SearchJSONFilenameToken = tbJSONFilenameToken.Text;
            s.SearchJSONURLToken = tbJSONURLToken.Text;
            s.SearchJSONFileSizeToken = tbJSONFilesizeToken.Text;
            s.SearchRSSManualScanOnly = cbSearchRSSManualScanOnly.Checked;
            s.MonitorFolders = cbMonitorFolder.Checked;
            s.runStartupCheck = chkScanOnStartup.Checked;
            s.runPeriodicCheck = chkScheduledScan.Checked;
            s.periodCheckHours = int.Parse(domainUpDown1.SelectedItem?.ToString() ?? "1");
            s.RemoveDownloadDirectoriesFiles = cbCleanUpDownloadDir.Checked;
            s.DeleteShowFromDisk = cbDeleteShowFromDisk.Checked;
            s.DoBulkAddInScan = cbScanIncludesBulkAdd.Checked;
            s.IgnoreAllSpecials = chkIgnoreAllSpecials.Checked;

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
            s.PriorityReplaceTerms = tbPriorityOverrideTerms.Text;

            s.FolderJpgIs = FolderJpgMode();
            s.MonitoredFoldersScanType = ScanTypeMode();

            s.mode = cbMode.Text == "Beta" ? TVSettings.BetaMode.BetaToo : TVSettings.BetaMode.ProductionOnly;

            s.keepTogetherMode = KeepTogetherMode();

            s.PreferredLanguageCode =
                TheTVDB.Instance.LanguageList.First(l => l.Name == cbLanguages.Text)?.Abbreviation ??
                s.PreferredLanguageCode;

            s.WTWDoubleClick = rbWTWScan.Checked
                ? TVSettings.WTWDoubleClickAction.Scan
                : TVSettings.WTWDoubleClickAction.Search;

            s.SampleFileMaxSizeMB = txtMaxSampleSize.Text.ToInt(50);
            s.upgradeDirtyPercent = tbPercentDirty.Text.ToPercent(20);
            s.replaceMargin = tbPercentBetter.Text.ToPercent(10);
            s.ParallelDownloads = txtParallelDownloads.Text.ToInt( 1, 4, 8);

            UpdateRSSURLs(s);

            s.ShowStatusColors = GetShowStatusColouring();

            s.DefShowIncludeFuture = cbDefShowIncludeFuture.Checked;
            s.DefShowIncludeNoAirdate = cbDefShowIncludeNoAirdate.Checked;
            s.DefShowNextAirdate = cbDefShowNextAirdate.Checked;
            s.DefShowDoMissingCheck = cbDefShowDoMissingCheck.Checked;
            s.DefShowDoRenaming = cbDefShowDoRenaming.Checked;
            s.DefShowDVDOrder = cbDefShowDVDOrder.Checked;
            s.DefShowAutoFolders = cbDefShowAutoFolders.Checked;
            s.DefShowLocation = (string)cmbDefShowLocation.SelectedItem;
            s.DefaultShowTimezoneName = cbTimeZone.Text;
            s.DefShowSequentialMatching = cbDefShowSequentialMatching.Checked;
            s.DefShowSpecialsCount = cbDefShowSpecialsCount.Checked;
            s.DefShowUseBase = rbDefShowUseBase.Checked;
            s.DefShowUseDefLocation = cbDefShowUseDefLocation.Checked;
            s.DefShowUseSubFolders = rbDefShowUseSubFolders.Checked;
        }

        private TVSettings.ScanType ScanTypeMode()
        {
            if (rdoQuickScan.Checked)
            {
                return TVSettings.ScanType.Quick;
            }

            if (rdoRecentScan.Checked)
            {
                return TVSettings.ScanType.Recent;
            }

            return TVSettings.ScanType.Full;
        }

        private TVSettings.FolderJpgIsType FolderJpgMode()
        {
            if (rbFolderFanArt.Checked)
            {
                return TVSettings.FolderJpgIsType.FanArt;
            }

            if (rbFolderBanner.Checked)
            {
                return TVSettings.FolderJpgIsType.Banner;
            }

            if (rbFolderSeasonPoster.Checked)
            {
                return TVSettings.FolderJpgIsType.SeasonPoster;
            }

            return TVSettings.FolderJpgIsType.Poster;
        }

        [NotNull]
        private TVSettings.ShowStatusColoringTypeList GetShowStatusColouring()
        {
            TVSettings.ShowStatusColoringTypeList returnValue = new TVSettings.ShowStatusColoringTypeList();
            foreach (ListViewItem item in lvwDefinedColors.Items)
            {
                if (item.SubItems.Count > 1 && !string.IsNullOrEmpty(item.SubItems[1].Text) && item.Tag != null &&
                    item.Tag is TVSettings.ShowStatusColoringType type)
                {
                    returnValue.Add(type, ColorTranslator.FromHtml(item.SubItems[1].Text));
                }
            }

            return returnValue;
        }

        private TVSettings.KeepTogetherModes KeepTogetherMode()
        {
            switch (cbKeepTogetherMode.Text)
            {
                case "All but these":
                    return TVSettings.KeepTogetherModes.AllBut;
                case "Just":
                    return TVSettings.KeepTogetherModes.Just;
                default:
                    return TVSettings.KeepTogetherModes.All;
            }
        }

        #endregion

        #region Update Form

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
            bool aborted = false;
            lock (TheTVDB.LANGUAGE_LOCK)
            {
                try
                {
                    if (!TheTVDB.Instance.Connected)
                    {
                        TheTVDB.Instance.Connect(true);
                    }
                }
                catch (ThreadAbortException)
                {
                    aborted = true;
                }
                catch (Exception e)
                {
                    Logger.Fatal(e, "Unhandled Exception in LoadLanguages");
                    aborted = true;
                }
            }

            if (!aborted)
            {
                BeginInvoke(loadLanguageDone);
            }
        }

        private void LoadLanguageDoneFunc()
        {
            FillLanguageList();
        }

        private void FillLanguageList()
        {
            cbLanguages.BeginUpdate();
            cbLanguages.Items.Clear();

            string pref = "";
            lock(TheTVDB.LANGUAGE_LOCK)
            {
                foreach (Language l in TheTVDB.Instance.LanguageList)
                {
                    cbLanguages.Items.Add(l.Name);

                    if (enterPreferredLanguage == l.Abbreviation)
                    {
                        pref = l.Name;
                    }
                }
            }
            cbLanguages.EndUpdate();
            cbLanguages.Text = pref;
            cbLanguages.Enabled = true;
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

            ReplacementsGrid.Columns[0].AutoSizeMode =
                SourceGrid.AutoSizeMode.EnableStretch | SourceGrid.AutoSizeMode.EnableAutoSize;

            ReplacementsGrid.Columns[1].AutoSizeMode =
                SourceGrid.AutoSizeMode.EnableStretch | SourceGrid.AutoSizeMode.EnableAutoSize;

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

        private void AddNewReplacementRow([CanBeNull] string from, string to, bool ins)
        {
            SourceGrid.Cells.Views.Cell roModel = new SourceGrid.Cells.Views.Cell {ForeColor = Color.Gray};

            int r = ReplacementsGrid.RowsCount;
            ReplacementsGrid.RowsCount = r + 1;
            ReplacementsGrid[r, 0] = new SourceGrid.Cells.Cell(from, typeof(string));
            ReplacementsGrid[r, 1] = new SourceGrid.Cells.Cell(to, typeof(string));
            ReplacementsGrid[r, 2] = new SourceGrid.Cells.CheckBox(null, ins);
            if (!string.IsNullOrEmpty(from) &&
                TVSettings.CompulsoryReplacements().IndexOf(from, StringComparison.Ordinal) != -1)
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

            RSSGrid.Columns[0].AutoSizeMode =
                SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;

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

        // ReSharper disable once InconsistentNaming
        private void UpdateRSSURLs([NotNull] TVSettings s)
        {
            // RSS URLs
            s.RSSURLs.Clear();
            for (int i = 1; i < RSSGrid.RowsCount; i++)
            {
                string url = (string) RSSGrid[i, 0].Value;
                if (!string.IsNullOrEmpty(url))
                {
                    s.RSSURLs.Add(url);
                }
            }
        }

        private void UpdateReplacement([NotNull] TVSettings s)
        {
            s.Replacements.Clear();
            for (int i = 1; i < ReplacementsGrid.RowsCount; i++)
            {
                string from = (string) ReplacementsGrid[i, 0].Value;
                string to = (string) ReplacementsGrid[i, 1].Value;
                bool ins = (bool) ReplacementsGrid[i, 2].Value;
                if (!string.IsNullOrEmpty(from))
                {
                    s.Replacements.Add(new TVSettings.Replacement(from, to, ins));
                }
            }
        }

        // ReSharper disable once FunctionComplexityOverflow
        private void Preferences_Load(object sender, EventArgs e)
        {
            SetupLanguages();

            TVSettings s = TVSettings.Instance;

            PopulateReplacements(s);

            txtMaxSampleSize.Text = s.SampleFileMaxSizeMB.ToString();

            cbRSSCloudflareProtection.Checked= s.RSSUseCloudflare;
            cbJSONCloudflareProtection.Checked= s.SearchJSONUseCloudflare;
            cbDownloadTorrentBeforeDownloading.Checked= s.qBitTorrentDownloadFilesFirst;
            chkBasicShowDetails.Checked = s.ShowBasicShowDetails;
            cbDetailedRSSJSONLogging.Checked = s.DetailedRSSJSONLogging;
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
            cbUseColoursOnWtw.Checked= s.UseColoursOnWtw ;

            cbTimeZone.Text = s.DefaultShowTimezoneName;

            cbMissingXML.Checked = s.ExportMissingXML;
            txtMissingXML.Text = s.ExportMissingXMLTo;
            cbMissingCSV.Checked = s.ExportMissingCSV;
            txtMissingCSV.Text = s.ExportMissingCSVTo;

            cbXSPF.Checked = s.ExportRecentXSPF;
            txtXSPF.Text = s.ExportRecentXSPFTo;
            cbM3U.Checked = s.ExportRecentM3U;
            txtM3U.Text = s.ExportRecentM3UTo;
            cbASX.Checked = s.ExportRecentASX;
            txtASX.Text = s.ExportRecentASXTo;
            cbWPL.Checked = s.ExportRecentWPL;
            txtWPL.Text = s.ExportRecentWPLTo;

            cbShowsTXT.Checked = s.ExportShowsTXT;
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
            tbPreferredRSSTerms.Text = s.GetPreferredRSSSearchTermsString();

            cbKeepTogether.Checked = s.KeepTogether;
            cbLeadingZero.Checked = s.LeadingZeroOnSeason;
            chkShowInTaskbar.Checked = s.ShowInTaskbar;
            cbTxtToSub.Checked = s.RenameTxtToSub;
            cbShowEpisodePictures.Checked = s.ShowEpisodePictures;
            chkHideMyShowsSpoilers.Checked = s.HideMyShowsSpoilers;
            chkHideWtWSpoilers.Checked = s.HideWtWSpoilers;
            cbAutoCreateFolders.Checked = s.AutoCreateFolders;
            cbAutoSelInMyShows.Checked = s.AutoSelectShowInMyShows;
            txtSpecialsFolderName.Text = s.SpecialsFolderName;
            txtSeasonFormat.Text = s.SeasonFolderFormat;
            cbForceLower.Checked = s.ForceLowercaseFilenames;
            cbIgnoreSamples.Checked = s.IgnoreSamples;
            txtRSSuTorrentPath.Text = s.uTorrentPath;
            txtUTResumeDatPath.Text = s.ResumeDatPath;
            txtSABHostPort.Text = s.SABHostPort;
            txtSABAPIKey.Text = s.SABAPIKey;
            tbqBitTorrentHost.Text = s.qBitTorrentHost;
            tbqBitTorrentPort.Text = s.qBitTorrentPort;
            cbCheckqBitTorrent.Checked = s.CheckqBitTorrent;
            cbCheckSABnzbd.Checked = s.CheckSABnzbd;
            cbHigherQuality.Checked = s.ReplaceWithBetterQuality;

            txtParallelDownloads.Text = s.ParallelDownloads.ToString();
            tbPercentDirty.Text = s.upgradeDirtyPercent.ToString(CultureInfo.InvariantCulture);
            tbPercentBetter.Text = s.replaceMargin.ToString(CultureInfo.InvariantCulture);

            cbSearchJSON.Checked = s.SearchJSON;
            cbSearchJSONManualScanOnly.Checked = s.SearchJSONManualScanOnly;
            tbJSONURL.Text = s.SearchJSONURL;
            tbJSONRootNode.Text = s.SearchJSONRootNode;
            tbJSONFilenameToken.Text = s.SearchJSONFilenameToken;
            tbJSONURLToken.Text = s.SearchJSONURLToken;
            tbJSONFilesizeToken.Text = s.SearchJSONFileSizeToken;

            cbSearchRSSManualScanOnly.Checked = s.SearchRSSManualScanOnly;
            cbSearchRSS.Checked = s.SearchRSS;
            cbEpTBNs.Checked = s.EpTBNs;
            cbWDLiveEpisodeFiles.Checked = s.wdLiveTvMeta;
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
            SetDropdownValue(domainUpDown1, s.periodCheckHours);
            cbCleanUpDownloadDir.Checked = s.RemoveDownloadDirectoriesFiles;
            cbDeleteShowFromDisk.Checked = s.DeleteShowFromDisk;
            cbCopyFutureDatedEps.Checked = s.CopyFutureDatedEpsFromSearchFolders;
            chkShareCriticalLogs.Checked = s.ShareLogs;
            chkPostpendThe.Checked = s.PostpendThe;
            chkUseLibraryFullPathWhenMatchingShows.Checked = s.UseFullPathNameToMatchLibraryFolders;
            chkUseSearchFullPathWhenMatchingShows.Checked= s.UseFullPathNameToMatchSearchFolders;
            chkAutoAddAsPartOfQuickRename.Checked = s.AutoAddAsPartOfQuickRename;
            chkCleanLibraryAfterActions.Checked = s.CleanLibraryAfterActions;

            cbMissing.Checked = s.MissingCheck;
            chkMoveLibraryFiles.Checked = s.MoveLibraryFiles;
            cbxUpdateAirDate.Checked = s.CorrectFileDates;
            chkAutoSearchForDownloadedFiles.Checked = s.AutoSearchForDownloadedFiles;
            cbSearchLocally.Checked = s.SearchLocally;
            cbIgnorePreviouslySeen.Checked = s.IgnorePreviouslySeen;
            cbLeaveOriginals.Checked = s.LeaveOriginals;
            enterPreferredLanguage = s.PreferredLanguageCode;
            cbScanIncludesBulkAdd.Checked = s.DoBulkAddInScan;
            chkIgnoreAllSpecials.Checked = s.IgnoreAllSpecials;

            cbEpThumbJpg.Checked = s.EpJPGs;
            cbSeriesJpg.Checked = s.SeriesJpg;
            cbXMLFiles.Checked = s.Mede8erXML;
            cbShrinkLarge.Checked = s.ShrinkLargeMede8erImages;
            cbFantArtJpg.Checked = s.FanArtJpg;

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

            cbDefShowIncludeFuture.Checked= s.DefShowIncludeFuture;
            cbDefShowIncludeNoAirdate.Checked= s.DefShowIncludeNoAirdate;
            cbDefShowNextAirdate.Checked= s.DefShowNextAirdate;
            cbDefShowDoMissingCheck.Checked= s.DefShowDoMissingCheck;
            cbDefShowDoRenaming.Checked= s.DefShowDoRenaming;
            cbDefShowDVDOrder.Checked= s.DefShowDVDOrder;
            cbDefShowAutoFolders.Checked= s.DefShowAutoFolders;
            cmbDefShowLocation.SelectedText= s.DefShowLocation;
            cbDefShowSequentialMatching.Checked= s.DefShowSequentialMatching;
            cbDefShowSpecialsCount.Checked= s.DefShowSpecialsCount;
            rbDefShowUseBase.Checked= s.DefShowUseBase;
            cbDefShowUseDefLocation.Checked= s.DefShowUseDefLocation;
            rbDefShowUseSubFolders.Checked= s.DefShowUseSubFolders;

            tbPriorityOverrideTerms.Text = s.PriorityReplaceTerms;

            PopulateFromEnums(s);

            FillSearchFolderList();
            FillFolderStringLists();
            PopulateAndSetDefShowLocation(s.DefShowLocation);

            foreach (string row in s.RSSURLs)
            {
                AddNewRssRow(row);
            }

            PopulateShowStatusColours(s);

            FillTreeViewColoringShowStatusTypeCombobox();

            EnableDisable(null, null);
        }

        private void PopulateFromEnums([NotNull] TVSettings s)
        {
            switch (s.WTWDoubleClick)
            {
                case TVSettings.WTWDoubleClickAction.Search:
                    rbWTWSearch.Checked = true;
                    break;
                case TVSettings.WTWDoubleClickAction.Scan:
                    rbWTWScan.Checked = true;
                    break;
                default:
                    throw new InvalidOperationException("Unexpected value s.WTWDoubleClick = " + s.WTWDoubleClick);
            }

            switch (s.keepTogetherMode)
            {
                case TVSettings.KeepTogetherModes.All:
                    cbKeepTogetherMode.Text = "All";
                    break;
                case TVSettings.KeepTogetherModes.AllBut:
                    cbKeepTogetherMode.Text = "All but these";
                    break;
                case TVSettings.KeepTogetherModes.Just:
                    cbKeepTogetherMode.Text = "Just";
                    break;
                default:
                    throw new InvalidOperationException("Unexpected value s.keepTogetherMode = " + s.keepTogetherMode);
            }

            switch (s.mode)
            {
                case TVSettings.BetaMode.ProductionOnly:
                    cbMode.Text = "Production";
                    break;
                case TVSettings.BetaMode.BetaToo:
                    cbMode.Text = "Beta";
                    break;
                default:
                    throw new InvalidOperationException("Unexpected value s.mode = " + s.mode);
            }

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
                case TVSettings.FolderJpgIsType.Poster:
                    rbFolderPoster.Checked = true;
                    break;
                default:
                    throw new InvalidOperationException("Unexpected value s.FolderJpgIs = " + s.FolderJpgIs);
            }

            switch (s.MonitoredFoldersScanType)
            {
                case TVSettings.ScanType.Quick:
                    rdoQuickScan.Checked = true;
                    break;
                case TVSettings.ScanType.Recent:
                    rdoRecentScan.Checked = true;
                    break;
                case TVSettings.ScanType.Full:
                    rdoFullScan.Checked = true;
                    break;

                case TVSettings.ScanType.SingleShow:
                    throw new InvalidOperationException("Unexpected value s.MonitoredFoldersScanType = SingleShow");
                    break;
                default:
                    throw new InvalidOperationException("Unexpected value s.MonitoredFoldersScanType = " + s.MonitoredFoldersScanType);
                    break;
            }
        }

        private void FillSearchFolderList()
        {
            lbSearchFolders.Items.Clear();
            TVSettings.Instance.DownloadFolders.Sort();
            foreach (string efi in TVSettings.Instance.DownloadFolders)
            {
                lbSearchFolders.Items.Add(efi);
            }
        }

        private void FillFolderStringLists()
        {
            TVSettings.Instance.LibraryFolders.Sort();

            lstFMMonitorFolders.BeginUpdate();
            lstFMMonitorFolders.Items.Clear();

            foreach (string folder in TVSettings.Instance.LibraryFolders)
            {
                lstFMMonitorFolders.Items.Add(folder);
            }

            lstFMMonitorFolders.EndUpdate();
        }

        private void UpdateDefShowLocation()
        {
            string oldValue = (string) cmbDefShowLocation.SelectedItem;
            PopulateAndSetDefShowLocation(oldValue);
        }

        private void PopulateAndSetDefShowLocation([CanBeNull] string path)
        {
            TVSettings.Instance.LibraryFolders.Sort();

            cmbDefShowLocation.BeginUpdate();
            cmbDefShowLocation.Items.Clear();

            cmbDefShowLocation.Items.Add(path??string.Empty);

            foreach (string folder in TVSettings.Instance.LibraryFolders)
            {
                if (folder == path)
                {
                    continue;
                }
                cmbDefShowLocation.Items.Add(folder);
            }

            cmbDefShowLocation.SelectedItem = path;

            cmbDefShowLocation.EndUpdate();
        }
        private void PopulateReplacements([NotNull] TVSettings s)
        {
            foreach (TVSettings.Replacement rep in s.Replacements)
            {
                AddNewReplacementRow(rep.This, rep.That, rep.CaseInsensitive);
            }
        }

        private void PopulateShowStatusColours([NotNull] TVSettings s)
        {
            if (s.ShowStatusColors is null)
            {
                return;
            }

            foreach (
                System.Collections.Generic.KeyValuePair<TVSettings.ShowStatusColoringType, Color> showStatusColor in
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

        private static void SetDropdownValue([NotNull] DomainUpDown control, int sPeriodCheckHours)
        {
            foreach (object item in control.Items)
            {
                if (item.ToString() == sPeriodCheckHours.ToString())
                {
                    control.SelectedItem = item;
                }
            }
        }

        private void FillTreeViewColoringShowStatusTypeCombobox()
        {
            //System.Collections.Generic.KeyValuePair<string, object> item = new System.Collections.Generic.KeyValuePair<string, object>();
            // Shows
            foreach (string status in Enum.GetNames(typeof(ShowItem.ShowAirStatus)))
            {
                TVSettings.ShowStatusColoringType t = new TVSettings.ShowStatusColoringType(true, true, status);
                //System.Collections.Generic.KeyValuePair<string, object> item = new System.Collections.Generic.KeyValuePair<string, object>("Show Seasons Status: " + status, new ShowStatusColoringType(true, true, status));
                cboShowStatus.Items.Add(t);
                //this.cboShowStatus.Items.Add("Show Seasons Status: " + status);
            }

            System.Collections.Generic.List<string> showStatusList = new System.Collections.Generic.List<string>();
            foreach (ShowItem show in mDoc.Library.GetShowItems())
            {
                if (!showStatusList.Contains(show.ShowStatus))
                {
                    showStatusList.Add(show.ShowStatus);
                }
            }

            foreach (string status in showStatusList)
            {
                TVSettings.ShowStatusColoringType t = new TVSettings.ShowStatusColoringType(false, true, status);
                cboShowStatus.Items.Add(t);
            }

            // Seasons
            foreach (TVSettings.ShowStatusColoringType t in Enum.GetNames(typeof(Season.SeasonStatus))
                .Select(status => new TVSettings.ShowStatusColoringType(true, false, status)))
            {
                cboShowStatus.Items.Add(t);
            }

            cboShowStatus.DisplayMember = "Text";
        }

        #endregion

        private void TxtNumberOnlyKeyPress(object sender, [NotNull] KeyPressEventArgs e)
        {
            // digits only
            if (e.KeyChar >= 32 && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        #region PopupBrowseDialog

        private void bnBrowseWTWRSS_Click(object sender, EventArgs e) => Browse(txtWTWRSS, "rss", 1);
        private void bnBrowseMissingXML_Click(object sender, EventArgs e) => Browse(txtMissingXML, "xml", 2);
        private void bnBrowseRenamingXML_Click(object sender, EventArgs e) => Browse(txtRenamingXML, "xml", 2);
        private void bnBrowseFOXML_Click(object sender, EventArgs e) => Browse(txtFOXML, "xml", 2);
        private void bnBrowseWTWXML_Click(object sender, EventArgs e) => Browse(txtWTWXML, "xml", 2);
        private void bnBrowseMissingCSV_Click(object sender, EventArgs e) => Browse(txtMissingCSV, "csv", 3);
        private void bnBrowseShowsTXT_Click(object sender, EventArgs e) => Browse(txtShowsTXTTo, "txt", 4);
        private void bnBrowseShowsHTML_Click(object sender, EventArgs e) => Browse(txtShowsHTMLTo, "html", 5);
        private void bnBrowseWTWICAL_Click(object sender, EventArgs e) => Browse(txtWTWICAL, "iCal", 6);
        private void bnBrowseXSPF_Click(object sender, EventArgs e) => Browse(txtXSPF, "xspf", 7);
        private void bnBrowseM3U_Click(object sender, EventArgs e) => Browse(txtM3U, "m3u8", 8);
        private void bnBrowseASX_Click(object sender, EventArgs e) => Browse(txtASX, "asx", 9);
        private void bnBrowseWPL_Click(object sender, EventArgs e) => Browse(txtWPL, "wpl", 10);

        private void Browse([NotNull] Control txt, string defaultExt, int filterIndex)
        {
            //rss =1, XML = 2, CSV = 3, TXT=4, HTML = 5
            saveFile.FileName = txt.Text;
            saveFile.DefaultExt = defaultExt;
            saveFile.FilterIndex = filterIndex;
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                txt.Text = saveFile.FileName;
            }
        }

        private void bnRSSBrowseuTorrent_Click(object sender, EventArgs e)
        {
            Browse(txtRSSuTorrentPath, "utorrent.exe|utorrent.exe|All Files (*.*)|*.*");
        }

        private void bnUTBrowseResumeDat_Click(object sender, EventArgs e)
        {
            Browse(txtUTResumeDatPath, "resume.dat|resume.dat|All Files (*.*)|*.*");
        }

        private void Browse([NotNull] TextBox txt, string filter)
        {
            openFile.FileName = txt.Text;
            openFile.Filter = filter;
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                txt.Text = openFile.FileName;
            }
        }

        #endregion PopupBrowseDialog

        #region RSS OnClick Functionality

        private void bnRSSAdd_Click(object sender, EventArgs e)
        {
            AddNewRssRow(null);
        }

        private void bnRSSRemove_Click(object sender, EventArgs e)
        {
            // multi-selection is off, so we can cheat...
            int[] rowsIndex = RSSGrid.Selection.GetSelectionRegion().GetRowsIndex();
            if (rowsIndex.Length > 0)
            {
                RSSGrid.Rows.Remove(rowsIndex[0]);
            }
        }

        private void bnRSSGo_Click(object sender, EventArgs e)
        {
            // multi-selection is off, so we can cheat...
            int[] rowsIndex = RSSGrid.Selection.GetSelectionRegion().GetRowsIndex();

            if (rowsIndex.Length > 0)
            {
                Helpers.SysOpen((string) RSSGrid[rowsIndex[0], 0].Value);
            }
        }

        #endregion

        #region enable and disable settings as appropriate

        private void cbNotificationIcon_CheckedChanged(object sender, EventArgs e) => EnableDisable(sender, e);
        private void chkShowInTaskbar_CheckedChanged(object sender, EventArgs e) => EnableDisable(sender, e);
        private void cbKeepTogether_CheckedChanged(object sender, EventArgs e) => EnableDisable(sender, e);
        private void cbMissing_CheckedChanged(object sender, EventArgs e) => EnableDisable(sender, e);
        private void cbSearchLocally_CheckedChanged(object sender, EventArgs e) => EnableDisable(sender, e);
        private void cbMeta_CheckedChanged(object sender, EventArgs e) => EnableDisable(sender, e);
        private void cbKeepTogetherMode_SelectedIndexChanged(object sender, EventArgs e) => EnableDisable(sender, e);
        private void cbSearchRSS_CheckedChanged(object sender, EventArgs e) => EnableDisable(sender, e);
        private void cbSearchJSON_CheckedChanged(object sender, EventArgs e) => EnableDisable(sender, e);
        private void lvwDefinedColors_SelectedIndexChanged(object sender, EventArgs e) => EnableDisable(sender, e);

        private void EnableDisable(object sender, EventArgs e)
        {
            bnRemoveDefinedColor.Enabled = lvwDefinedColors.SelectedItems.Count == 1;
            txtKeepTogether.Enabled = cbKeepTogether.Checked && cbKeepTogetherMode.Text != "All";
            gbRSS.Enabled = cbSearchRSS.Checked;
            gbJSON.Enabled = cbSearchJSON.Checked;

            if (!cbNotificationIcon.Checked)
            {
                chkShowInTaskbar.Checked = true;
            }

            if (!chkShowInTaskbar.Checked)
            {
                cbNotificationIcon.Checked = true;
            }

            cbTxtToSub.Enabled = cbKeepTogether.Checked;
            txtKeepTogether.Enabled = cbKeepTogether.Checked && cbKeepTogetherMode.Text != "All";
            cbKeepTogetherMode.Enabled = cbKeepTogether.Checked;
            label39.Enabled = cbKeepTogether.Checked;

            ExportersOptEnableDisable();

            ScanOptEnableDisable();
        }

        private void ExportersOptEnableDisable()
        {
            txtWTWRSS.Enabled = cbWTWRSS.Checked;
            bnBrowseWTWRSS.Enabled = cbWTWRSS.Checked;

            txtWTWXML.Enabled = cbWTWXML.Checked;
            bnBrowseWTWXML.Enabled = cbWTWXML.Checked;

            txtWTWICAL.Enabled = cbWTWICAL.Checked;
            bnBrowseWTWICAL.Enabled = cbWTWICAL.Checked;

            txtM3U.Enabled = cbM3U.Checked;
            bnBrowseM3U.Enabled = cbM3U.Checked;
            txtXSPF.Enabled = cbXSPF.Checked;
            bnBrowseXSPF.Enabled = cbXSPF.Checked;
            txtASX.Enabled = cbASX.Checked;
            bnBrowseASX.Enabled = cbASX.Checked;
            txtWPL.Enabled = cbWPL.Checked;
            bnBrowseWPL.Enabled = cbWPL.Checked;

            bool wtw = cbWTWRSS.Checked || cbWTWXML.Checked || cbWTWICAL.Checked;
            label4.Enabled = wtw;
            label15.Enabled = wtw;
            label16.Enabled = wtw;
            label17.Enabled = wtw;
            txtExportRSSMaxDays.Enabled = wtw;
            txtExportRSSMaxShows.Enabled = wtw;
            txtExportRSSDaysPast.Enabled = wtw;

            SetEnabled(cbFOXML, txtFOXML, bnBrowseFOXML);
            SetEnabled(cbShowsTXT, txtShowsTXTTo, bnBrowseShowsTXT);
            SetEnabled(cbShowsHTML, txtShowsHTMLTo, bnBrowseShowsHTML);
            SetEnabled(cbRenamingXML, txtRenamingXML, bnBrowseRenamingXML);
            SetEnabled(cbMissingXML, txtMissingXML, bnBrowseMissingXML);
            SetEnabled(cbMissingCSV, txtMissingCSV, bnBrowseMissingCSV);
        }

        private static void SetEnabled([NotNull] CheckBox checkBox, [NotNull] TextBox txtMissingCsv, [NotNull] Button bnBrowseMissingCsv)
        {
            bool status = checkBox.Checked;
            txtMissingCsv.Enabled = status;
            bnBrowseMissingCsv.Enabled = status;
        }

        private void ScanOptEnableDisable()
        {
            bool e = cbMissing.Checked;

            cbIgnorePreviouslySeen.Enabled = e;
            cbSearchRSS.Enabled = e;
            cbSearchLocally.Enabled = e;
            cbCheckuTorrent.Enabled = e;
            cbCheckSABnzbd.Enabled = e;
            cbCheckqBitTorrent.Enabled = e;
            cbSearchJSON.Enabled = e;

            cbSearchJSONManualScanOnly.Enabled = cbSearchJSON.Checked && e;
            cbSearchRSSManualScanOnly.Enabled = cbSearchRSS.Checked && e;

            cbLeaveOriginals.Enabled = e && cbSearchLocally.Checked;
        }

        #endregion enable and disable settings as appropriate

        #region Replacement Rows OnClick Functionality

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
                string from = (string) ReplacementsGrid[n, 0].Value;
                if (string.IsNullOrEmpty(from) ||
                    TVSettings.CompulsoryReplacements().IndexOf(from, StringComparison.Ordinal) == -1)
                {
                    ReplacementsGrid.Rows.Remove(n);
                }
            }
        }

        private void btnAddShowStatusColoring_Click(object sender, EventArgs e)
        {
            if (cboShowStatus.SelectedItem != null && !string.IsNullOrEmpty(txtShowStatusColor.Text))
            {
                try
                {
                    TVSettings.ShowStatusColoringType ssct =
                        cboShowStatus.SelectedItem as TVSettings.ShowStatusColoringType;

                    if (!ColorTranslator.FromHtml(txtShowStatusColor.Text).IsEmpty && ssct != null)
                    {
                        ListViewItem item = lvwDefinedColors.FindItemWithText(ssct.Text);
                        if (item is null)
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

        #endregion

        #region ColourSelection OnClick Functionality

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
                txtShowStatusColor.Text = Helpers.TranslateColorToHtml(colorDialog.Color);
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

        #endregion

        private void Preferences_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (loadLanguageThread != null && loadLanguageThread.IsAlive)
            {
                loadLanguageThread.Abort();
                loadLanguageThread.Join(500); // milliseconds timeout
            }
        }

        private void cmDefaults_ItemClicked(object sender, [NotNull] ToolStripItemClickedEventArgs e)
        {
            if (!(e.ClickedItem?.Tag is string) || !int.TryParse((string) e.ClickedItem.Tag, out int v))
            {
                return;
            }

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

        private void domainUpDown1_KeyDown(object sender, [NotNull] KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        private void bnTags_Click(object sender, EventArgs e)
        {
            cntfw = new CustomNameTagsFloatingWindow(sampleSeason);
            cntfw.Show(this);
            Focus();
        }

        private void tpSearch_DrawItem(object sender, [NotNull] DrawItemEventArgs e)
        {
            //Follow this advice https://docs.microsoft.com/en-us/dotnet/framework/winforms/controls/how-to-display-side-aligned-tabs-with-tabcontrol

            Graphics g = e.Graphics;

            g.FillRectangle(e.State == DrawItemState.Selected ? Brushes.White : new SolidBrush(BackColor),
                e.Bounds);

            // Get the item from the collection.
            TabPage tabPage = tcTabs.TabPages[e.Index];

            // Get the real bounds for the tab rectangle.
            Rectangle tabBounds = tcTabs.GetTabRect(e.Index);

            // Draw string. Center the text.
            StringFormat stringFlags = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString(tabPage.Text, tcTabs.Font, Brushes.Black, tabBounds, new StringFormat(stringFlags));
        }

        #region Folder Add & Remove

        private void bnAddSearchFolder_Click(object sender, EventArgs e)
        {
            //Setup the UI
            FolderBrowserDialogEx searchFolderBrowser = new FolderBrowserDialogEx
            {
                SelectedPath = "",
                Title = "Add New Search Folder...",
                ShowEditbox = true,
                StartPosition = FormStartPosition.CenterScreen
            };

            //Populate the popup with the right path
            if (lbSearchFolders.SelectedIndex != -1)
            {
                int n = lbSearchFolders.SelectedIndex;
                searchFolderBrowser.SelectedPath = TVSettings.Instance.DownloadFolders[n];
            }

            //Show dialog
            if (searchFolderBrowser.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            //exit if nothing is selected
            if (!Directory.Exists(searchFolderBrowser.SelectedPath))
            {
                return;
            }

            TVSettings.Instance.DownloadFolders.Add(searchFolderBrowser.SelectedPath);
            mDoc.SetDirty();
            FillSearchFolderList();
        }

        private void bnRemoveSearchFolder_Click(object sender, EventArgs e)
        {
            int n = lbSearchFolders.SelectedIndex;
            if (n == -1)
            {
                return;
            }

            TVSettings.Instance.DownloadFolders.RemoveAt(n);
            mDoc.SetDirty();

            FillSearchFolderList();
        }

        private void bnOpenSearchFolder_Click(object sender, EventArgs e)
        {
            int n = lbSearchFolders.SelectedIndex;
            if (n == -1)
            {
                return;
            }

            Helpers.SysOpen(TVSettings.Instance.DownloadFolders[n]);
        }

        private void lbSearchFolders_KeyDown(object sender, [NotNull] KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                bnRemoveSearchFolder_Click(null, null);
            }
        }

        private void lbSearchFolders_DragOver(object sender, [NotNull] DragEventArgs e)
        {
            e.Effect = !e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.None : DragDropEffects.Copy;
        }

        private void lbSearchFolders_DragDrop(object sender, [NotNull] DragEventArgs e)
        {
            string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
            foreach (string path in files)
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    if (di.Exists)
                    {
                        TVSettings.Instance.DownloadFolders.Add(path.ToLower());
                    }
                }
                catch
                {
                    // ignored
                }
            }

            mDoc.SetDirty();
            FillSearchFolderList();
        }

        private void bnRemoveMonFolder_Click(object sender, EventArgs e)
        {
            for (int i = lstFMMonitorFolders.SelectedIndices.Count - 1; i >= 0; i--)
            {
                int n = lstFMMonitorFolders.SelectedIndices[i];
                TVSettings.Instance.LibraryFolders.RemoveAt(n);
            }

            mDoc.SetDirty();
            FillFolderStringLists();
            UpdateDefShowLocation();
        }

        private void bnAddMonFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialogEx searchFolderBrowser = new FolderBrowserDialogEx
            {
                SelectedPath = "",
                Title = "Add New Library Folder...",
                ShowEditbox = true,
                StartPosition = FormStartPosition.CenterScreen
            };

            if (lstFMMonitorFolders.SelectedIndex != -1)
            {
                int n = lstFMMonitorFolders.SelectedIndex;
                searchFolderBrowser.SelectedPath = TVSettings.Instance.LibraryFolders[n];
            }

            if (searchFolderBrowser.ShowDialog(this) == DialogResult.OK && Directory.Exists(searchFolderBrowser.SelectedPath))
            {
                TVSettings.Instance.LibraryFolders.Add(searchFolderBrowser.SelectedPath);
                mDoc.SetDirty();
                FillFolderStringLists();
                UpdateDefShowLocation();
            }
        }

        private void bnOpenMonFolder_Click(object sender, EventArgs e)
        {
            if (lstFMMonitorFolders.SelectedIndex != -1)
            {
                Helpers.SysOpen(TVSettings.Instance.LibraryFolders[lstFMMonitorFolders.SelectedIndex]);
            }
        }

        private void lstFMMonitorFolders_KeyDown(object sender, [NotNull] KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                bnRemoveMonFolder_Click(null, null);
            }
        }

        private void lstFMMonitorFolders_DragOver(object sender, [NotNull] DragEventArgs e)
        {
            e.Effect = !e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.None : DragDropEffects.Copy;
        }

        private void lstFMMonitorFolders_DragDrop(object sender, [NotNull] DragEventArgs e)
        {
            string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
            foreach (string path in files)
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    if (di.Exists)
                    {
                        TVSettings.Instance.LibraryFolders.Add(path.ToLower());
                    }
                }
                catch
                {
                    // ignored
                }
            }

            mDoc.SetDirty();
            FillSearchFolderList();
        }

        private void lstFMMonitorFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            bnRemoveMonFolder.Enabled = lstFMMonitorFolders.SelectedIndices.Count > 0;
            bnOpenMonFolder.Enabled = lstFMMonitorFolders.SelectedIndices.Count > 0;
        }

        private void lbSearchFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            bnRemoveSearchFolder.Enabled = lbSearchFolders.SelectedIndices.Count > 0;
            bnOpenSearchFolder.Enabled = lbSearchFolders.SelectedIndices.Count > 0;
        }

        #endregion Folder Add & Remove

        #region HelpWindows

        private void pbGeneral_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-general-tab");
        private void pbDisplay_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-display-tab");
        private void pbSearchFolders_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-search-folders-tab");
        private void pbRSSJSONSearch_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-rss--json-search-tab");

        private void pbFilesAndFolders_Click(object sender, EventArgs e) =>
            OpenInfoWindow("/#the-files-and-folders-tab");

        private void pbFolderDeleting_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-folder-deleting-tab");
        private void pictureBox7_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-media-center-tab");
        private void pictureBox1_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-torrents--nzb-tab");
        private void pbLibraryFolders_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-library-folders-tab");
        private void PictureBox1_Click_1(object sender, EventArgs e) => OpenInfoWindow("/#the-folder-defaults-tab");

        private static void OpenInfoWindow(string page)
        {
            Helpers.SysOpen($"https://www.tvrename.com/manual/options{page}");
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            Season t = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            cntfw = new CustomNameTagsFloatingWindow(t);
            cntfw.Show(this);
            Focus();
        }

        private void CbDefShowUseDefLocation_CheckedChanged(object sender, EventArgs e)
        {
            cmbDefShowLocation.Enabled = cbDefShowUseDefLocation.Checked;
        }
    }
}
