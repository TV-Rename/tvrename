//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using Alphaleonis.Win32.Filesystem;
using DaveChambers.FolderBrowserDialogEx;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using TimeZoneConverter;
using ColumnHeader = SourceGrid.Cells.ColumnHeader;
using Control = System.Windows.Forms.Control;

namespace TVRename.Forms;

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
    private readonly TVDoc mDoc;
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private CustomNameTagsFloatingWindow? cntfw;
    private readonly ProcessedSeason? sampleProcessedSeason;

    private class FailedValidationException : Exception
    {
    }

    public Preferences(TVDoc doc, bool goToScanOpts, ProcessedSeason? s)
    {
        sampleProcessedSeason = s;
        InitializeComponent();

        FillLanguageList();
        SetupTimezoneDropdown();
        SetupTmdbDropDowns();
        SetupRssGrid();
        SetupReplacementsGrid();
        FillFolderStringLists();
        FillMovieFolderStringLists();
        PopulateFolderTypes(MovieConfiguration.MovieFolderFormat.singleDirectorySingleFile);
        mDoc = doc;
        cntfw = null;

        if (goToScanOpts)
        {
            tcTabs.SelectedTab = tpScanSettings;
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

    private void SetupTmdbDropDowns()
    {
        cbTMDBLanguages.BeginUpdate();
        cbTMDBLanguages.Items.Clear();
        foreach (Language language in Languages.Instance)
        {
            cbTMDBLanguages.Items.Add(language.LocalName);
        }
        cbTMDBLanguages.EndUpdate();

        cbTMDBRegions.BeginUpdate();
        cbTMDBRegions.Items.Clear();
        foreach (Region region in Regions.Instance.Where(region => region.EnglishName.HasValue()))
        {
            cbTMDBRegions.Items.Add(region.EnglishName!);
        }
        cbTMDBRegions.EndUpdate();
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
        ValidateFilePath(txtSpecialsFolderName, tpLibraryFolders, true);
        ValidateFilePath(txtSeasonFormat, tpLibraryFolders, true);
        ValidateFilePath(txtShowFolderFormat, tpLibraryFolders, true);
        ValidateFilePath(txtMovieFolderFormat, tpLibraryFolders, true);
        ValidateFilePath(txtMovieFilenameFormat, tpLibraryFolders, true);
        if (cbCheckuTorrent.Checked)
        {
            ValidateFilePath(txtUTResumeDatPath, tpTorrentNZB, false);
            ValidateFilePath(txtRSSuTorrentPath, tpTorrentNZB, false);
        }
    }

    private void ValidateFilePath(TextBox validationField, TabPage errorPage, bool emptyOk)
    {
        if (TVSettings.OKPath(validationField.Text, emptyOk))
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
        ValidateExtensionsWithoutDot(txtOtherExtensions, tbFilesAndFolders);
        ValidateExtensions(txtKeepTogether, tbFilesAndFolders);
    }

    private void ValidateExporterLocations()
    {
        ValidateExporterLocation(cbWTWRSS, txtWTWRSS);
        ValidateExporterLocation(cbWTWXML, txtWTWXML);
        ValidateExporterLocation(cbWTWICAL, txtWTWICAL);
        ValidateExporterLocation(cbWTWTXT, txtWTWTXT);

        ValidateExporterLocation(cbMissingXML, txtMissingXML);
        ValidateExporterLocation(cbMissingCSV, txtMissingCSV);

        ValidateExporterLocation(cbShowsTXT, txtShowsTXTTo);
        ValidateExporterLocation(cbShowsHTML, txtShowsHTMLTo);

        ValidateExporterLocation(cbMissingMoviesXML, txtMissingMoviesXML);
        ValidateExporterLocation(cbMissingMoviesCSV, txtMissingMoviesCSV);

        ValidateExporterLocation(cbMoviesTXT, txtMoviesTXTTo);
        ValidateExporterLocation(cbMoviesHTML, txtMoviesHTMLTo);

        ValidateExporterLocation(cbRenamingXML, txtRenamingXML);
        ValidateExporterLocation(cbFOXML, txtFOXML);

        ValidateExporterLocation(cbXSPF, txtXSPF);
        ValidateExporterLocation(cbM3U, txtM3U);
        ValidateExporterLocation(cbASX, txtASX);
        ValidateExporterLocation(cbWPL, txtWPL);
    }

    private void ValidateExporterLocation(CheckBox controlCheckbox, TextBox validationField)
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

    private void ValidateExtensions(Control validateField, TabPage focusTabPage)
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

    private void ValidateExtensionsWithoutDot(Control validateField, TabPage focusTabPage)
    {
        if (TVSettings.OKExtensionsStringNoDotCheck(validateField.Text))
        {
            return;
        }

        MessageBox.Show(
            "Extensions list must be separated by semicolons.",
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

        s.AutoSaveOnExit = cbAutoSaveOnExit.Checked;
        s.RSSUseCloudflare = cbRSSCloudflareProtection.Checked;
        s.SearchJSONUseCloudflare = cbJSONCloudflareProtection.Checked;
        s.qBitTorrentDownloadFilesFirst = cbDownloadTorrentBeforeDownloading.Checked;
        s.ShowBasicShowDetails = chkBasicShowDetails.Checked;
        s.DetailedRSSJSONLogging = cbDetailedRSSJSONLogging.Checked;
        s.ExportWTWRSS = cbWTWRSS.Checked;
        s.ExportWTWRSSTo = txtWTWRSS.Text;
        s.ExportWTWXML = cbWTWXML.Checked;
        s.ExportWTWXMLTo = txtWTWXML.Text;
        s.ExportWTWTXT = cbWTWTXT.Checked;
        s.ExportWTWTXTTo = txtWTWTXT.Text;
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

        s.ExportMoviesTXT = cbMoviesTXT.Checked;
        s.ExportMoviesTXTTo = txtMoviesTXTTo.Text;
        s.ExportMoviesHTML = cbMoviesHTML.Checked;
        s.ExportMoviesHTMLTo = txtMoviesHTMLTo.Text;

        s.ExportMissingMoviesXML = cbMissingMoviesXML.Checked;
        s.ExportMissingMoviesXMLTo = txtMissingMoviesXML.Text;
        s.ExportMissingMoviesCSV = cbMissingMoviesCSV.Checked;
        s.ExportMissingMoviesCSVTo = txtMissingMoviesCSV.Text;

        s.WTWRecentDays = Convert.ToInt32(txtWTWDays.Text);
        s.StartupTab = cbStartupTab.SelectedIndex;
        s.NotificationAreaIcon = cbNotificationIcon.Checked;
        s.VideoExtensionsString = txtVideoExtensions.Text;
        s.OtherExtensionsString = txtOtherExtensions.Text;
        s.subtitleExtensionsString = txtSubtitleExtensions.Text;
        s.ExportRSSMaxDays = txtExportRSSMaxDays.Text.ToInt(7);
        s.ExportRSSMaxShows = txtExportRSSMaxShows.Text.ToInt(10);
        s.ExportRSSDaysPast = txtExportRSSDaysPast.Text.ToInt(0);
        s.MinRSSSeeders = txtMinRSSSeeders.Text.ToInt(10);
        s.KeepTogether = cbKeepTogether.Checked;
        s.LeadingZeroOnSeason = cbLeadingZero.Checked;
        s.ShowInTaskbar = chkShowInTaskbar.Checked;
        s.ShowAccessibilityOptions = chkShowAccessibilityOptions.Checked;
        s.RenameTxtToSub = cbTxtToSub.Checked;
        s.ShowEpisodePictures = cbShowEpisodePictures.Checked;
        s.ReplaceWithBetterQuality = cbHigherQuality.Checked;
        s.ReplaceMoviesWithBetterQuality = cbMovieHigherQuality.Checked;
        s.HideMyShowsSpoilers = chkHideMyShowsSpoilers.Checked;
        s.HideWtWSpoilers = chkHideWtWSpoilers.Checked;
        s.AutoSelectShowInMyShows = cbAutoSelInMyShows.Checked;
        s.AutoCreateFolders = cbAutoCreateFolders.Checked;
        s.SpecialsFolderName = txtSpecialsFolderName.Text;
        s.SeasonFolderFormat = txtSeasonFormat.Text;
        s.DefaultTvShowFolderFormat = txtShowFolderFormat.Text;
        s.MovieFolderFormat = txtMovieFolderFormat.Text;
        s.MovieFilenameFormat = txtMovieFilenameFormat.Text;
        s.searchSeasonWordsString = tbSeasonSearchTerms.Text;
        s.SubsFolderNamesString = txtSubtitleFolderNames.Text;
        s.preferredRSSSearchTermsString = tbPreferredRSSTerms.Text;
        s.unwantedRSSSearchTermsString = tbUnwantedRSSTerms.Text;
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
        s.qBitTorrentUseHTTPS = chkBitTorrentUseHTTPS.Checked;
        s.CheckqBitTorrent = cbCheckqBitTorrent.Checked;
        s.RemoveCompletedTorrents = chkRemoveCompletedTorrents.Checked;
        s.SearchRSS = cbSearchRSS.Checked;
        s.EpThumbnails = cbEpTBNs.Checked;
        s.NFOShows = cbNFOShows.Checked;
        s.NFOEpisodes = cbNFOEpisodes.Checked;
        s.NFOMovies = cbNFOMovies.Checked;
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
        s.IgnorePreviouslySeenMovies = cbIgnorePreviouslySeenMovies.Checked;
        s.AutoSearchForDownloadedFiles = chkAutoSearchForDownloadedFiles.Checked;
        s.LeaveOriginals = cbLeaveOriginals.Checked;
        s.CheckuTorrent = cbCheckuTorrent.Checked;
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
        s.AutomateAutoAddWhenOneMovieFound = cbAutomateAutoAddWhenOneMovieFound.Checked;
        s.AutomateAutoAddWhenOneShowFound = cbAutomateAutoAddWhenOneShowFound.Checked;
        s.ChooseWhenMultipleEpisodesMatch = chkChooseWhenMultipleEpisodesMatch.Checked;

        s.SearchJSON = cbSearchJSON.Checked;
        s.SearchJSONManualScanOnly = cbSearchJSONManualScanOnly.Checked;
        s.SearchJSONURL = tbJSONURL.Text;
        s.SearchJSONRootNode = tbJSONRootNode.Text;
        s.SearchJSONFilenameToken = tbJSONFilenameToken.Text;
        s.SearchJSONURLToken = tbJSONURLToken.Text;
        s.SearchJSONFileSizeToken = tbJSONFilesizeToken.Text;
        s.SearchJSONSeedersToken = tbJSONSeedersToken.Text;
        s.SearchRSSManualScanOnly = cbSearchRSSManualScanOnly.Checked;
        s.MonitorFolders = cbMonitorFolder.Checked;
        s.runStartupCheck = chkScanOnStartup.Checked;
        s.runPeriodicCheck = chkScheduledScan.Checked;
        s.periodCheckHours = int.Parse(upDownScanHours.SelectedItem?.ToString() ?? "1");
        s.periodUpdateCacheHours = int.Parse(domainUpDown2.SelectedItem?.ToString() ?? "1");
        s.FolderMonitorDelaySeconds = int.Parse(upDownScanSeconds.SelectedItem?.ToString() ?? "1");

        s.UnattendedMultiActionOutcome = ConvertToDupActEnum(cmbUnattendedDuplicateAction);
        s.UserMultiActionOutcome = ConvertToDupActEnum(cmbSupervisedDuplicateAction);
        s.DefMovieFolderFormat = ConvertToMovieFormat(cmbDefMovieFolderFormat);

        s.SearchJackett = cbSearchJackett.Checked;
        s.UseJackettTextSearch = chkUseJackettTextSearch.Checked;
        s.SearchJackettManualScanOnly = cbSearchJackettOnManualScansOnly.Checked;
        s.SearchJackettButton = chkSearchJackettButton.Checked;
        s.StopJackettSearchOnFullScan = chkSkipJackettFullScans.Checked;
        s.JackettServer = txtJackettServer.Text;
        s.JackettPort = txtJackettPort.Text;
        s.JackettIndexer = txtJackettIndexer.Text;
        s.JackettAPIKey = txtJackettAPIKey.Text;

        s.RemoveDownloadDirectoriesFiles = cbCleanUpDownloadDir.Checked;
        s.RemoveDownloadDirectoriesFilesMatchMovies = cbCleanUpDownloadDirMovies.Checked;
        s.RemoveDownloadDirectoriesFilesMatchMoviesLengthCheck = cbCleanUpDownloadDirMoviesLength.Checked;
        s.RemoveDownloadDirectoriesFilesMatchMoviesLengthCheckLength = tbCleanUpDownloadDirMoviesLength.Text.ToInt(8);

        s.DeleteShowFromDisk = cbDeleteShowFromDisk.Checked;
        s.DeleteMovieFromDisk = cbDeleteMovieFromDisk.Checked;
        s.DoBulkAddInScan = cbScanIncludesBulkAdd.Checked;
        s.IgnoreAllSpecials = chkIgnoreAllSpecials.Checked;
        s.FileNameCaseSensitiveMatch = cbFileNameCaseSensitiveMatch.Checked;
        s.CopySubsFolders = cbCopySubsFolders.Checked;

        s.EpJPGs = cbEpThumbJpg.Checked;
        s.SeriesJpg = cbSeriesJpg.Checked;
        s.Mede8erXML = cbXMLFiles.Checked;
        s.ShrinkLargeMede8erImages = cbShrinkLarge.Checked;
        s.FanArtJpg = cbFantArtJpg.Checked;
        s.GroupMissingEpisodesIntoSeasons = chkGroupMissingEpisodesIntoSeasons.Checked;

        s.Tidyup.DeleteEmpty = cbDeleteEmpty.Checked;
        s.Tidyup.DeleteEmptyIsRecycle = cbRecycleNotDelete.Checked;
        s.Tidyup.EmptyIgnoreWords = cbEmptyIgnoreWords.Checked;
        s.Tidyup.EmptyIgnoreWordList = txtEmptyIgnoreWords.Text;
        s.Tidyup.EmptyIgnoreExtensions = cbEmptyIgnoreExtensions.Checked;
        s.Tidyup.EmptyIgnoreExtensionList = txtEmptyIgnoreExtensions.Text;
        s.Tidyup.EmptyMaxSizeCheck = cbEmptyMaxSize.Checked;
        s.Tidyup.EmptyMaxSizeMB = txtEmptyMaxSize.Text.ToInt(100);

        s.BulkAddCompareNoVideoFolders = cbIgnoreNoVideoFolders.Checked;
        s.BulkAddIgnoreRecycleBin = cbIgnoreRecycleBin.Checked;
        s.AutoAddIgnoreSuffixes = tbIgnoreSuffixes.Text;
        s.AutoAddMovieTerms = tbMovieTerms.Text;
        s.PriorityReplaceTerms = tbPriorityOverrideTerms.Text;

        s.FolderJpgIs = FolderJpgMode();
        s.MonitoredFoldersScanType = ScanTypeMode();
        s.qBitTorrentAPIVersion = qBitTorrentAPIVersionMode();
        s.DefaultProvider = ProviderMode();

        s.mode = cbMode.Text == @"Beta" ? TVSettings.BetaMode.BetaToo : TVSettings.BetaMode.ProductionOnly;

        s.keepTogetherMode = KeepTogetherMode();

        s.PreferredTVDBLanguage = Languages.Instance.GetLanguageFromLocalName(cbTVDBLanguages.Text) ?? s.PreferredTVDBLanguage;

        s.WTWDoubleClick = rbWTWScan.Checked
            ? TVSettings.WTWDoubleClickAction.Scan
            : TVSettings.WTWDoubleClickAction.Search;

        s.UseGlobalReleaseDate = rdoGlobalReleaseDates.Checked;

        s.SampleFileMaxSizeMB = txtMaxSampleSize.Text.ToInt(50);
        s.upgradeDirtyPercent = tbPercentDirty.Text.ToPercent(20);
        s.replaceMargin = tbPercentBetter.Text.ToPercent(10);
        s.ParallelDownloads = txtParallelDownloads.Text.ToInt(1, 4, 8);

        UpdateRSSURLs(s);

        s.ShowStatusColors = GetShowStatusColouring();

        s.DefShowIncludeFuture = cbDefShowIncludeFuture.Checked;
        s.DefShowIncludeNoAirdate = cbDefShowIncludeNoAirdate.Checked;
        s.DefShowNextAirdate = cbDefShowNextAirdate.Checked;
        s.DefShowDoMissingCheck = cbDefShowDoMissingCheck.Checked;
        s.DefShowDoRenaming = cbDefShowDoRenaming.Checked;
        s.DefShowDVDOrder = cbDefShowDVDOrder.Checked;
        s.DefShowAlternateOrder = cbDefShowAlternateOrder.Checked;
        s.DefShowAutoFolders = cbDefShowAutoFolders.Checked;
        s.DefShowLocation = (string)cmbDefShowLocation.SelectedItem;
        s.DefaultShowTimezoneName = cbTimeZone.Text;
        s.DefShowSequentialMatching = cbDefShowSequentialMatching.Checked;
        s.DefShowAirDateMatching = cbDefShowAirdateMatching.Checked;
        s.DefShowEpNameMatching = cbDefShowEpNameMatching.Checked;
        s.DefShowSpecialsCount = cbDefShowSpecialsCount.Checked;
        s.DefShowUseBase = rbDefShowUseBase.Checked;
        s.DefShowUseDefLocation = cbDefShowUseDefLocation.Checked;
        s.DefShowUseSubFolders = rbDefShowUseSubFolders.Checked;

        s.DefMovieDoRenaming = cbDefMovieDoRenaming.Checked;
        s.DefMovieDoMissingCheck = cbDefMovieDoMissing.Checked;
        s.DefMovieCheckFutureDatedMovies = cbDefMovieIncludeFuture.Checked;
        s.DefMovieCheckNoDatedMovies = cbDefMovieIncludeNoAirdate.Checked;
        s.DefMovieUseAutomaticFolders = cbDefMovieAutoFolders.Checked;
        s.DefMovieUseDefaultLocation = cbDefMovieUseDefLocation.Checked;
        s.DefMovieDefaultLocation = (string)cmbDefMovieLocation.SelectedItem;
        s.DefaultMovieProvider = MovieProviderMode();

        s.TMDBLanguage = Languages.Instance.GetLanguageFromLocalName(cbTMDBLanguages.SelectedItem?.ToString()) ?? s.TMDBLanguage;
        s.TMDBRegion = Regions.Instance.RegionFromName(cbTMDBRegions.SelectedItem?.ToString()) ?? s.TMDBRegion;
        s.TMDBPercentDirty = tbTMDBPercentDirty.Text.ToPercent(20);
        s.IncludeMoviesQuickRecent = chkIncludeMoviesQuickRecent.Checked;
        s.UnArchiveFilesInDownloadDirectory = chkUnArchiveFilesInDownloadDirectory.Checked;

        UpdateAppUpdateSettings(s);
    }

    private void UpdateAppUpdateSettings(TVSettings s)
    {
        s.UpdateCheckType = GetUpdateCheckTypeFromUi();
        if (cboUpdateCheckInterval.SelectedValue != null)
        {
            s.UpdateCheckInterval = (TimeSpan)cboUpdateCheckInterval.SelectedValue;
        }
        s.SuppressUpdateAvailablePopup = chkNoPopupOnUpdate.Checked;
    }

    private TVSettings.UpdateCheckMode GetUpdateCheckTypeFromUi()
    {
        if (chkUpdateCheckEnabled.Checked)
        {
            return optUpdateCheckAlways.Checked
                ? TVSettings.UpdateCheckMode.Everytime
                : TVSettings.UpdateCheckMode.Interval;
        }

        return TVSettings.UpdateCheckMode.Off;
    }

    private static TVSettings.DuplicateActionOutcome ConvertToDupActEnum(ComboBox p0)
    {
        return p0.Text switch
        {
            "Ask User" => TVSettings.DuplicateActionOutcome.Ask,
            "Choose Largest File" => TVSettings.DuplicateActionOutcome.Largest,
            "Use First" => TVSettings.DuplicateActionOutcome.ChooseFirst,
            "Download All" => TVSettings.DuplicateActionOutcome.DoAll,
            "Ignore" => TVSettings.DuplicateActionOutcome.IgnoreAll,
            "Choose Most Popular" => TVSettings.DuplicateActionOutcome.MostSeeders,
            _ => throw new ArgumentOutOfRangeException(nameof(p0), $"ConvertToDupActEnum: p0 has invalid Text {p0.Text}")
        };
    }

    private static MovieConfiguration.MovieFolderFormat ConvertToMovieFormat(ComboBox p0)
    {
        return p0.Text switch
        {
            "Single Movie per Folder" => MovieConfiguration.MovieFolderFormat.singleDirectorySingleFile,
            "Many Movies per Folder" => MovieConfiguration.MovieFolderFormat.multiPerDirectory,
            "Bluray format" => MovieConfiguration.MovieFolderFormat.bluray,
            "DVD format" => MovieConfiguration.MovieFolderFormat.dvd,
            _ => throw new ArgumentOutOfRangeException(nameof(p0), $"ConvertToMovieFormat: p0 has invalid Text {p0.Text}")
        };
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

    // ReSharper disable once InconsistentNaming
    private qBitTorrent.qBitTorrentAPIVersion qBitTorrentAPIVersionMode()
    {
        if (rdoqBitTorrentAPIVersionv0.Checked)
        {
            return global::TVRename.qBitTorrent.qBitTorrentAPIVersion.v0;
        }
        if (rdoqBitTorrentAPIVersionv1.Checked)
        {
            return global::TVRename.qBitTorrent.qBitTorrentAPIVersion.v1;
        }

        return global::TVRename.qBitTorrent.qBitTorrentAPIVersion.v2;
    }

    private TVDoc.ProviderType ProviderMode()
    {
        if (rdoTVTVMaze.Checked)
        {
            return TVDoc.ProviderType.TVmaze;
        }
        if (rdoTVTMDB.Checked)
        {
            return TVDoc.ProviderType.TMDB;
        }
        return TVDoc.ProviderType.TheTVDB;
    }

    private TVDoc.ProviderType MovieProviderMode()
    {
        if (rdoMovieTheTVDB.Checked)
        {
            return TVDoc.ProviderType.TheTVDB;
        }
        return TVDoc.ProviderType.TMDB;
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

    private TVSettings.ShowStatusColoringTypeList GetShowStatusColouring()
    {
        TVSettings.ShowStatusColoringTypeList returnValue = [];
        foreach (ListViewItem item in lvwDefinedColors.Items)
        {
            if (item.SubItems.Count > 1 && !string.IsNullOrEmpty(item.SubItems[1].Text) && item.Tag is TVSettings.ColouringRule type)
            {
                returnValue.Add(type, ColorTranslator.FromHtml(item.SubItems[1].Text));
            }
        }

        return returnValue;
    }

    private TVSettings.KeepTogetherModes KeepTogetherMode()
    {
        return cbKeepTogetherMode.Text switch
        {
            "All but these" => TVSettings.KeepTogetherModes.AllBut,
            "Just" => TVSettings.KeepTogetherModes.Just,
            _ => TVSettings.KeepTogetherModes.All
        };
    }

    #endregion Update Settings

    #region Update Form

    private void FillLanguageList()
    {
        cbTVDBLanguages.BeginUpdate();
        cbTVDBLanguages.Items.Clear();
        foreach (Language l in Languages.Instance)
        {
            cbTVDBLanguages.Items.Add(l.LocalName);
        }
        cbTVDBLanguages.EndUpdate();
        cbTVDBLanguages.Enabled = true;
    }

    private void SetupReplacementsGrid()
    {
        SourceGrid.Cells.Views.Cell titleModel = new()
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

        ReplacementsGrid.Columns[0].Width -= 8; // allow for scrollbar
        ReplacementsGrid.Columns[1].Width -= 8;

        //////////////////////////////////////////////////////////////////////
        // header row

        ColumnHeader h = new("Search") { AutomaticSortEnabled = false };
        ReplacementsGrid[0, 0] = h;
        ReplacementsGrid[0, 0].View = titleModel;

        h = new ColumnHeader("Replace") { AutomaticSortEnabled = false };
        ReplacementsGrid[0, 1] = h;
        ReplacementsGrid[0, 1].View = titleModel;

        h = new ColumnHeader("Case Ins.") { AutomaticSortEnabled = false };
        ReplacementsGrid[0, 2] = h;
        ReplacementsGrid[0, 2].View = titleModel;
    }

    private void AddNewReplacementRow(string? from, string? to, bool ins)
    {
        SourceGrid.Cells.Views.Cell roModel = new() { ForeColor = Color.Gray };

        int r = ReplacementsGrid.RowsCount;
        ReplacementsGrid.RowsCount = r + 1;
        ReplacementsGrid[r, 0] = new SourceGrid.Cells.Cell(from, typeof(string));
        ReplacementsGrid[r, 1] = new SourceGrid.Cells.Cell(to, typeof(string));
        ReplacementsGrid[r, 2] = new SourceGrid.Cells.CheckBox(null, ins);
        if (!string.IsNullOrEmpty(from) &&
            TVSettings.CompulsoryReplacements().Contains(from))
        {
            ReplacementsGrid[r, 0].Editor.EnableEdit = false;
            ReplacementsGrid[r, 0].View = roModel;
        }
    }

    private void SetupRssGrid()
    {
        SourceGrid.Cells.Views.Cell titleModel = new()
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
        ColumnHeader h = new("URL") { AutomaticSortEnabled = false };
        RSSGrid[0, 0] = h;
        RSSGrid[0, 0].View = titleModel;
    }

    private void AddNewRssRow(string? text)
    {
        int r = RSSGrid.RowsCount;
        RSSGrid.RowsCount = r + 1;
        RSSGrid[r, 0] = new SourceGrid.Cells.Cell(text, typeof(string));
    }

    // ReSharper disable once InconsistentNaming
    private void UpdateRSSURLs(TVSettings s)
    {
        // RSS URLs
        s.RSSURLs.Clear();
        for (int i = 1; i < RSSGrid.RowsCount; i++)
        {
            string url = (string)RSSGrid[i, 0].Value;
            if (!string.IsNullOrEmpty(url))
            {
                s.RSSURLs.Add(url);
            }
        }
    }

    private void UpdateReplacement(TVSettings s)
    {
        s.Replacements.Clear();
        for (int i = 1; i < ReplacementsGrid.RowsCount; i++)
        {
            string from = (string)ReplacementsGrid[i, 0].Value;
            string to = (string)ReplacementsGrid[i, 1].Value;
            bool ins = (bool)ReplacementsGrid[i, 2].Value;
            if (!string.IsNullOrEmpty(from))
            {
                s.Replacements.Add(new TVSettings.Replacement(from, to, ins));
            }
        }
    }

    // ReSharper disable once FunctionComplexityOverflow
    private void Preferences_Load(object sender, EventArgs e)
    {
        TVSettings s = TVSettings.Instance;

        PopulateReplacements(s);

        txtMaxSampleSize.Text = s.SampleFileMaxSizeMB.ToString();

        cbAutoSaveOnExit.Checked = s.AutoSaveOnExit;
        cbRSSCloudflareProtection.Checked = s.RSSUseCloudflare;
        cbJSONCloudflareProtection.Checked = s.SearchJSONUseCloudflare;
        cbDownloadTorrentBeforeDownloading.Checked = s.qBitTorrentDownloadFilesFirst;
        chkBasicShowDetails.Checked = s.ShowBasicShowDetails;
        cbDetailedRSSJSONLogging.Checked = s.DetailedRSSJSONLogging;
        cbWTWRSS.Checked = s.ExportWTWRSS;
        txtWTWRSS.Text = s.ExportWTWRSSTo;
        cbWTWICAL.Checked = s.ExportWTWICAL;
        txtWTWICAL.Text = s.ExportWTWICALTo;
        txtWTWDays.Text = s.WTWRecentDays.ToString();
        cbWTWXML.Checked = s.ExportWTWXML;
        txtWTWXML.Text = s.ExportWTWXMLTo;
        cbWTWTXT.Checked = s.ExportWTWTXT;
        txtWTWTXT.Text = s.ExportWTWTXTTo;
        txtExportRSSMaxDays.Text = s.ExportRSSMaxDays.ToString();
        txtExportRSSMaxShows.Text = s.ExportRSSMaxShows.ToString();
        txtExportRSSDaysPast.Text = s.ExportRSSDaysPast.ToString();
        txtMinRSSSeeders.Text = s.MinRSSSeeders.ToString();
        cbUseColoursOnWtw.Checked = s.UseColoursOnWtw;

        cbTimeZone.Text = s.DefaultShowTimezoneName;
        if (cbTimeZone.Text == string.Empty && !string.IsNullOrWhiteSpace(s.DefaultShowTimezoneName))
        {
            try
            {
                Logger.Info($"Could not work out what timezone is the default. In the settings it uses '{s.DefaultShowTimezoneName}', Testing to see whether it needs to be upgraded.");
                cbTimeZone.Text = TZConvert.WindowsToIana(s.DefaultShowTimezoneName);
            }
            catch (Exception)
            {
                Logger.Warn($"Could not work out what timezone is the default. In the settings it uses '{s.DefaultShowTimezoneName}', but could not work out what this is.");
            }
        }

        cbMissingXML.Checked = s.ExportMissingXML;
        txtMissingXML.Text = s.ExportMissingXMLTo;
        cbMissingCSV.Checked = s.ExportMissingCSV;
        txtMissingCSV.Text = s.ExportMissingCSVTo;

        chkRestrictMissingExportsToFullScans.Checked = s.RestrictMissingExportsToFullScans;

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

        cbMoviesTXT.Checked = s.ExportMoviesTXT;
        txtMoviesTXTTo.Text = s.ExportMoviesTXTTo;
        cbMoviesHTML.Checked = s.ExportMoviesHTML;
        txtMoviesHTMLTo.Text = s.ExportMoviesHTMLTo;

        cbMissingMoviesXML.Checked = s.ExportMissingMoviesXML;
        txtMissingMoviesXML.Text = s.ExportMissingMoviesXMLTo;
        cbMissingMoviesCSV.Checked = s.ExportMissingMoviesCSV;
        txtMissingMoviesCSV.Text = s.ExportMissingMoviesCSVTo;

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
        txtSubtitleFolderNames.Text = s.SubsFolderNamesString;
        tbPreferredRSSTerms.Text = s.GetPreferredRSSSearchTermsString();
        tbUnwantedRSSTerms.Text = s.GetUnwantedRSSSearchTermsString();

        cbKeepTogether.Checked = s.KeepTogether;
        cbLeadingZero.Checked = s.LeadingZeroOnSeason;
        chkShowAccessibilityOptions.Checked = s.ShowAccessibilityOptions;
        chkShowInTaskbar.Checked = s.ShowInTaskbar;
        cbTxtToSub.Checked = s.RenameTxtToSub;
        cbShowEpisodePictures.Checked = s.ShowEpisodePictures;
        chkHideMyShowsSpoilers.Checked = s.HideMyShowsSpoilers;
        chkHideWtWSpoilers.Checked = s.HideWtWSpoilers;
        cbAutoCreateFolders.Checked = s.AutoCreateFolders;
        cbAutoSelInMyShows.Checked = s.AutoSelectShowInMyShows;
        txtSpecialsFolderName.Text = s.SpecialsFolderName;
        txtSeasonFormat.Text = s.SeasonFolderFormat;
        txtShowFolderFormat.Text = s.DefaultTvShowFolderFormat;
        txtMovieFolderFormat.Text = s.MovieFolderFormat;
        txtMovieFilenameFormat.Text = s.MovieFilenameFormat;
        cbForceLower.Checked = s.ForceLowercaseFilenames;
        cbIgnoreSamples.Checked = s.IgnoreSamples;
        txtRSSuTorrentPath.Text = s.uTorrentPath;
        txtUTResumeDatPath.Text = s.ResumeDatPath;
        txtSABHostPort.Text = s.SABHostPort;
        txtSABAPIKey.Text = s.SABAPIKey;
        tbqBitTorrentHost.Text = s.qBitTorrentHost;
        tbqBitTorrentPort.Text = s.qBitTorrentPort;
        chkBitTorrentUseHTTPS.Checked = s.qBitTorrentUseHTTPS;
        cbCheckqBitTorrent.Checked = s.CheckqBitTorrent;
        chkRemoveCompletedTorrents.Checked = s.RemoveCompletedTorrents;
        cbCheckSABnzbd.Checked = s.CheckSABnzbd;
        cbHigherQuality.Checked = s.ReplaceWithBetterQuality;
        cbMovieHigherQuality.Checked = s.ReplaceMoviesWithBetterQuality;

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
        tbJSONSeedersToken.Text = s.SearchJSONSeedersToken;

        cbSearchRSSManualScanOnly.Checked = s.SearchRSSManualScanOnly;
        cbSearchRSS.Checked = s.SearchRSS;
        cbEpTBNs.Checked = s.EpThumbnails;
        cbWDLiveEpisodeFiles.Checked = s.wdLiveTvMeta;
        cbNFOShows.Checked = s.NFOShows;
        cbNFOEpisodes.Checked = s.NFOEpisodes;
        cbNFOMovies.Checked = s.NFOMovies;
        cbKODIImages.Checked = s.KODIImages;
        cbMeta.Checked = s.pyTivoMeta;
        cbMetaSubfolder.Checked = s.pyTivoMetaSubFolder;
        cbFolderJpg.Checked = s.FolderJpg;
        cbRenameCheck.Checked = s.RenameCheck;
        chkPreventMove.Checked = s.PreventMove;
        cbCheckuTorrent.Checked = s.CheckuTorrent;
        chkRetainLanguageSpecificSubtitles.Checked = s.RetainLanguageSpecificSubtitles;
        chkForceBulkAddToUseSettingsOnly.Checked = s.ForceBulkAddToUseSettingsOnly;
        chkAutoMergeDownloadEpisodes.Checked = s.AutoMergeDownloadEpisodes;
        chkAutoMergeLibraryEpisodes.Checked = s.AutoMergeLibraryEpisodes;
        cbMonitorFolder.Checked = s.MonitorFolders;
        chkScheduledScan.Checked = s.RunPeriodicCheck();
        chkScanOnStartup.Checked = s.RunOnStartUp();
        SetDropdownValue(upDownScanHours, s.periodCheckHours);
        SetDropdownValue(domainUpDown2, s.periodUpdateCacheHours);
        SetDropdownValue(upDownScanSeconds, s.FolderMonitorDelaySeconds);
        cbCleanUpDownloadDir.Checked = s.RemoveDownloadDirectoriesFiles;
        cbCleanUpDownloadDirMovies.Checked = s.RemoveDownloadDirectoriesFilesMatchMovies;
        cbCleanUpDownloadDirMoviesLength.Checked = s.RemoveDownloadDirectoriesFilesMatchMoviesLengthCheck;
        tbCleanUpDownloadDirMoviesLength.Text = s.RemoveDownloadDirectoriesFilesMatchMoviesLengthCheckLength.ToString();
        cbDeleteShowFromDisk.Checked = s.DeleteShowFromDisk;
        cbDeleteMovieFromDisk.Checked = s.DeleteMovieFromDisk;
        cbFileNameCaseSensitiveMatch.Checked = s.FileNameCaseSensitiveMatch;
        cbCopySubsFolders.Checked = s.CopySubsFolders;
        cbCopyFutureDatedEps.Checked = s.CopyFutureDatedEpsFromSearchFolders;
        chkShareCriticalLogs.Checked = s.ShareLogs;
        chkPostpendThe.Checked = s.PostpendThe;
        chkUseLibraryFullPathWhenMatchingShows.Checked = s.UseFullPathNameToMatchLibraryFolders;
        chkUseSearchFullPathWhenMatchingShows.Checked = s.UseFullPathNameToMatchSearchFolders;
        chkAutoAddAsPartOfQuickRename.Checked = s.AutoAddAsPartOfQuickRename;
        cbAutomateAutoAddWhenOneMovieFound.Checked = s.AutomateAutoAddWhenOneMovieFound;
        cbAutomateAutoAddWhenOneShowFound.Checked = s.AutomateAutoAddWhenOneShowFound;
        chkChooseWhenMultipleEpisodesMatch.Checked = s.ChooseWhenMultipleEpisodesMatch;
        cmbUnattendedDuplicateAction.Text = ConvertEnum(s.UnattendedMultiActionOutcome);
        cmbSupervisedDuplicateAction.Text = ConvertEnum(s.UserMultiActionOutcome);
        cmbDefMovieFolderFormat.Text = ConvertEnum(s.DefMovieFolderFormat);
        cbSearchJackett.Checked = s.SearchJackett;
        chkUseJackettTextSearch.Checked = s.UseJackettTextSearch;
        cbSearchJackettOnManualScansOnly.Checked = s.SearchJackettManualScanOnly;
        chkSearchJackettButton.Checked = s.SearchJackettButton;
        chkSkipJackettFullScans.Checked = s.StopJackettSearchOnFullScan;
        txtJackettServer.Text = s.JackettServer;
        txtJackettPort.Text = s.JackettPort;
        txtJackettIndexer.Text = s.JackettIndexer;
        txtJackettAPIKey.Text = s.JackettAPIKey;
        cbMissing.Checked = s.MissingCheck;
        chkMoveLibraryFiles.Checked = s.MoveLibraryFiles;
        cbxUpdateAirDate.Checked = s.CorrectFileDates;
        chkAutoSearchForDownloadedFiles.Checked = s.AutoSearchForDownloadedFiles;
        cbSearchLocally.Checked = s.SearchLocally;
        cbIgnorePreviouslySeen.Checked = s.IgnorePreviouslySeen;
        cbIgnorePreviouslySeenMovies.Checked = s.IgnorePreviouslySeenMovies;
        cbLeaveOriginals.Checked = s.LeaveOriginals;
        cbTVDBLanguages.Text = s.PreferredTVDBLanguage.LocalName;
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
        chkGroupMissingEpisodesIntoSeasons.Checked = s.GroupMissingEpisodesIntoSeasons;

        cbIgnoreRecycleBin.Checked = s.BulkAddIgnoreRecycleBin;
        cbIgnoreNoVideoFolders.Checked = s.BulkAddCompareNoVideoFolders;
        tbMovieTerms.Text = s.AutoAddMovieTerms;
        tbIgnoreSuffixes.Text = s.AutoAddIgnoreSuffixes;

        cbDefShowIncludeFuture.Checked = s.DefShowIncludeFuture;
        cbDefShowIncludeNoAirdate.Checked = s.DefShowIncludeNoAirdate;
        cbDefShowNextAirdate.Checked = s.DefShowNextAirdate;
        cbDefShowDoMissingCheck.Checked = s.DefShowDoMissingCheck;
        cbDefShowDoRenaming.Checked = s.DefShowDoRenaming;
        cbDefShowDVDOrder.Checked = s.DefShowDVDOrder;
        cbDefShowAlternateOrder.Checked = s.DefShowAlternateOrder;
        cbDefShowAutoFolders.Checked = s.DefShowAutoFolders;
        cbDefShowSequentialMatching.Checked = s.DefShowSequentialMatching;
        cbDefShowAirdateMatching.Checked = s.DefShowAirDateMatching;
        cbDefShowEpNameMatching.Checked = s.DefShowEpNameMatching;
        cbDefShowSpecialsCount.Checked = s.DefShowSpecialsCount;
        rbDefShowUseBase.Checked = s.DefShowUseBase;
        cbDefShowUseDefLocation.Checked = s.DefShowUseDefLocation;
        rbDefShowUseSubFolders.Checked = s.DefShowUseSubFolders;

        cbDefMovieDoRenaming.Checked = s.DefMovieDoRenaming;
        cbDefMovieDoMissing.Checked = s.DefMovieDoMissingCheck;
        cbDefMovieAutoFolders.Checked = s.DefMovieUseAutomaticFolders;
        cbDefMovieUseDefLocation.Checked = s.DefMovieUseDefaultLocation;
        cbDefMovieIncludeFuture.Checked = s.DefMovieCheckFutureDatedMovies;
        cbDefMovieIncludeNoAirdate.Checked = s.DefMovieCheckNoDatedMovies;

        s.RestrictMissingExportsToFullScans = chkRestrictMissingExportsToFullScans.Checked;

        cbTMDBLanguages.Text = s.TMDBLanguage.LocalName;
        cbTMDBRegions.Text = s.TMDBRegion.EnglishName ?? Regions.Instance.FallbackRegion.EnglishName;

        tbTMDBPercentDirty.Text = s.upgradeDirtyPercent.ToString(CultureInfo.InvariantCulture);
        chkIncludeMoviesQuickRecent.Checked = s.IncludeMoviesQuickRecent;
        chkUnArchiveFilesInDownloadDirectory.Checked = s.UnArchiveFilesInDownloadDirectory;

        tbPriorityOverrideTerms.Text = s.PriorityReplaceTerms;

        PopulateFromEnums(s);

        FillSearchFolderList();
        FillFolderStringLists();
        FillMovieFolderStringLists();
        PopulateAndSetDefShowLocation(s.DefShowLocation);
        PopulateAndSetDefMovieLocation(s.DefMovieDefaultLocation);

        foreach (string row in s.RSSURLs)
        {
            AddNewRssRow(row);
        }

        PopulateShowStatusColours(s);

        FillTreeViewColoringShowStatusTypeCombobox();

        SetupAppUpdateTabPageContent(s);

        EnableDisable();
    }

    private void SetupAppUpdateTabPageContent(TVSettings settings)
    {
        FillUpdateIntervals();
        TriggerControlEventsForInitialState();

        cboUpdateCheckInterval.SelectedValue = settings.UpdateCheckInterval;
        chkNoPopupOnUpdate.Checked = settings.SuppressUpdateAvailablePopup;
        SetUpdateCheckToTypeToUi(settings.UpdateCheckType);

        void TriggerControlEventsForInitialState()
        {
            // These seem redundant but they are there to cycle the control events and get the controls into the correct enabled state
            // The order of the statements additionally serves the purpose to set the default UI state
            optUpdateCheckInterval.Checked = true;
            optUpdateCheckAlways.Checked = true;
            chkUpdateCheckEnabled.Checked = false;
            chkUpdateCheckEnabled.Checked = true;
        }
    }

    private void SetUpdateCheckToTypeToUi(TVSettings.UpdateCheckMode updateCheckMode)
    {
        switch (updateCheckMode)
        {
            case TVSettings.UpdateCheckMode.Off:
                chkUpdateCheckEnabled.Checked = false;
                break;

            case TVSettings.UpdateCheckMode.Everytime:
                chkUpdateCheckEnabled.Checked = true;
                optUpdateCheckAlways.Checked = true;
                break;

            case TVSettings.UpdateCheckMode.Interval:
                chkUpdateCheckEnabled.Checked = true;
                optUpdateCheckInterval.Checked = true;
                break;
        }
    }

    private void FillUpdateIntervals()
    {
        cboUpdateCheckInterval.DisplayMember = nameof(UpdateCheckInterval.Text);
        cboUpdateCheckInterval.ValueMember = nameof(UpdateCheckInterval.Interval);
        cboUpdateCheckInterval.DataSource = new[]
        {
            new UpdateCheckInterval( "1 Hour",  1.Hours()),
            new UpdateCheckInterval( "12 Hours",  12.Hours()),
            new UpdateCheckInterval( "1 Day",  1.Days()),
            new UpdateCheckInterval( "1 Week", 1.Weeks()),
            new UpdateCheckInterval( "2 Week",  2.Weeks()),
            new UpdateCheckInterval( "30 Days",  30.Days()),
            new UpdateCheckInterval( "90 Days",  90.Days()),
        };
    }

    private class UpdateCheckInterval(string text, TimeSpan interval)
    {
        public string Text { get; set; } = text;

        public TimeSpan Interval { get; set; } = interval;
    }

    private void PopulateFromEnums(TVSettings s)
    {
        cbKeepTogetherMode.Text = ConvertEnum(s.keepTogetherMode);
        cbMode.Text = ConvertEnum(s.mode);
        cbTVDBVersion.Text = ConvertEnum(s.TvdbVersion);

        ChooseRadioButton(s.WTWDoubleClick).Checked = true;
        ChooseRadioButton(s.FolderJpgIs).Checked = true;
        ChooseRadioButton(s.qBitTorrentAPIVersion).Checked = true;
        ChooseRadioButton(s.MonitoredFoldersScanType).Checked = true;
        ChooseTvRadioButton(s.DefaultProvider).Checked = true;
        ChooseMovieRadioButton(s.DefaultMovieProvider).Checked = true;

        if (s.UseGlobalReleaseDate)
        {
            rdoGlobalReleaseDates.Checked = true;
        }
        else
        {
            rdoRegionalReleaseDates.Checked = true;
        }
    }

    private RadioButton ChooseRadioButton(TVSettings.ScanType enumType)
    {
        return enumType switch
        {
            TVSettings.ScanType.Quick => rdoQuickScan,
            TVSettings.ScanType.Recent => rdoRecentScan,
            TVSettings.ScanType.Full => rdoFullScan,
            TVSettings.ScanType.SingleShow => throw new InvalidOperationException("Unexpected value s.MonitoredFoldersScanType = SingleShow"),
            _ => throw new InvalidOperationException("Unexpected value s.MonitoredFoldersScanType = " + enumType)
        };
    }

    private RadioButton ChooseTvRadioButton(TVDoc.ProviderType enumType)
    {
        return enumType switch
        {
            TVDoc.ProviderType.libraryDefault => throw new InvalidOperationException("Unexpected value s.DefaultProvider = " + enumType),
            TVDoc.ProviderType.TVmaze => rdoTVTVMaze,
            TVDoc.ProviderType.TheTVDB => rdoTVTVDB,
            TVDoc.ProviderType.TMDB => rdoTVTMDB,
            _ => throw new InvalidOperationException("Unexpected value s.DefaultProvider = " + enumType)
        };
    }

    private RadioButton ChooseMovieRadioButton(TVDoc.ProviderType enumType)
    {
        return enumType switch
        {
            TVDoc.ProviderType.libraryDefault => throw new InvalidOperationException("Unexpected value s.DefaultMovieProvider = " + enumType),
            TVDoc.ProviderType.TVmaze => throw new InvalidOperationException("Unexpected value s.DefaultMovieProvider = " + enumType),
            TVDoc.ProviderType.TheTVDB => rdoMovieTheTVDB,
            TVDoc.ProviderType.TMDB => rdoMovieTMDB,
            _ => throw new InvalidOperationException("Unexpected value s.DefaultProvider = " + enumType)
        };
    }

    private RadioButton ChooseRadioButton(qBitTorrent.qBitTorrentAPIVersion sQBitTorrentApiVersion)
    {
        return sQBitTorrentApiVersion switch
        {
            global::TVRename.qBitTorrent.qBitTorrentAPIVersion.v0 => rdoqBitTorrentAPIVersionv0,
            global::TVRename.qBitTorrent.qBitTorrentAPIVersion.v1 => rdoqBitTorrentAPIVersionv1,
            global::TVRename.qBitTorrent.qBitTorrentAPIVersion.v2 => rdoqBitTorrentAPIVersionv2,
            _ => throw new InvalidOperationException("Unexpected value s.qBitTorrentAPIVersion = " +
                                                     sQBitTorrentApiVersion)
        };
    }

    private RadioButton ChooseRadioButton(TVSettings.FolderJpgIsType enumTyp)
    {
        return enumTyp switch
        {
            TVSettings.FolderJpgIsType.FanArt => rbFolderFanArt,
            TVSettings.FolderJpgIsType.Banner => rbFolderBanner,
            TVSettings.FolderJpgIsType.SeasonPoster => rbFolderSeasonPoster,
            TVSettings.FolderJpgIsType.Poster => rbFolderPoster,
            _ => throw new InvalidOperationException("Unexpected value s.FolderJpgIs = " + enumTyp)
        };
    }

    private RadioButton ChooseRadioButton(TVSettings.WTWDoubleClickAction sWtwDoubleClick)
    {
        return sWtwDoubleClick switch
        {
            TVSettings.WTWDoubleClickAction.Search => rbWTWSearch,
            TVSettings.WTWDoubleClickAction.Scan => rbWTWScan,
            _ => throw new InvalidOperationException("Unexpected value s.WTWDoubleClick = " + sWtwDoubleClick)
        };
    }

    private static string ConvertEnum(TVSettings.BetaMode mode)
    {
        return mode switch
        {
            TVSettings.BetaMode.ProductionOnly => "Production",
            TVSettings.BetaMode.BetaToo => "Beta",
            _ => throw new InvalidOperationException("Unexpected value s.mode = " + mode)
        };
    }

    private static string ConvertEnum(TheTVDB.ApiVersion mode)
    {
        return mode switch
        {
            TheTVDB.ApiVersion.v2 => "v2",
            TheTVDB.ApiVersion.v3 => "v3",
            TheTVDB.ApiVersion.v4 => "v4",
            _ => throw new InvalidOperationException("Unexpected value s.mode = " + mode)
        };
    }

    private static string ConvertEnum(TVSettings.KeepTogetherModes sKeepTogetherMode)
    {
        return sKeepTogetherMode switch
        {
            TVSettings.KeepTogetherModes.All => "All",
            TVSettings.KeepTogetherModes.AllBut => "All but these",
            TVSettings.KeepTogetherModes.Just => "Just",
            _ => throw new InvalidOperationException("Unexpected value s.keepTogetherMode = " + sKeepTogetherMode)
        };
    }

    private static string ConvertEnum(TVSettings.DuplicateActionOutcome outcome)
    {
        return outcome switch
        {
            TVSettings.DuplicateActionOutcome.IgnoreAll => "Ignore",
            TVSettings.DuplicateActionOutcome.ChooseFirst => "Use First",
            TVSettings.DuplicateActionOutcome.Ask => "Ask User",
            TVSettings.DuplicateActionOutcome.DoAll => "Download All",
            TVSettings.DuplicateActionOutcome.MostSeeders => "Choose Most Popular",
            TVSettings.DuplicateActionOutcome.Largest => "Choose Largest File",
            _ => throw new InvalidOperationException("Unexpected value s.outcome = " + outcome)
        };
    }

    private static string ConvertEnum(MovieConfiguration.MovieFolderFormat outcome) => outcome.PrettyPrint();

    private void FillSearchFolderList()
    {
        lbSearchFolders.Items.Clear();
        TVSettings.Instance.DownloadFolders.Sort();
        foreach (string efi in TVSettings.Instance.DownloadFolders)
        {
            lbSearchFolders.Items.Add(efi);
        }
    }

    private void FillMovieFolderStringLists()
    {
        TVSettings.Instance.MovieLibraryFolders.Sort();

        lstMovieMonitorFolders.BeginUpdate();
        lstMovieMonitorFolders.Items.Clear();

        foreach (string folder in TVSettings.Instance.MovieLibraryFolders)
        {
            lstMovieMonitorFolders.Items.Add(folder);
        }

        lstMovieMonitorFolders.EndUpdate();
    }

    private void PopulateFolderTypes(MovieConfiguration.MovieFolderFormat selectedShowFormat)
    {
        cmbDefMovieFolderFormat.SuspendLayout();
        cmbDefMovieFolderFormat.Items.Clear();
        cmbDefMovieFolderFormat.Items.AddRange(Enum.GetValues(typeof(MovieConfiguration.MovieFolderFormat))
            .OfType<MovieConfiguration.MovieFolderFormat>()
            .Select(x => x.PrettyPrint())
            .ToArray<object>());
        cmbDefMovieFolderFormat.ResumeLayout();
        cmbDefMovieFolderFormat.Text = selectedShowFormat.PrettyPrint();
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
        string oldValue = (string)cmbDefShowLocation.SelectedItem;
        PopulateAndSetDefShowLocation(oldValue);
    }

    private void UpdateDefMovieLocation()
    {
        string oldValue = (string)cmbDefMovieLocation.SelectedItem;
        PopulateAndSetDefMovieLocation(oldValue);
    }

    private void PopulateAndSetDefMovieLocation(string? path)
    {
        TVSettings.Instance.MovieLibraryFolders.Sort();

        cmbDefMovieLocation.BeginUpdate();
        cmbDefMovieLocation.Items.Clear();

        cmbDefMovieLocation.Items.Add(path ?? string.Empty);

        foreach (string folder in TVSettings.Instance.MovieLibraryFolders)
        {
            if (folder == path)
            {
                continue;
            }
            cmbDefMovieLocation.Items.Add(folder);
        }

        cmbDefMovieLocation.Text = path;

        cmbDefMovieLocation.EndUpdate();
    }

    private void PopulateAndSetDefShowLocation(string? path)
    {
        TVSettings.Instance.LibraryFolders.Sort();

        cmbDefShowLocation.BeginUpdate();
        cmbDefShowLocation.Items.Clear();

        cmbDefShowLocation.Items.Add(path ?? string.Empty);

        foreach (string folder in TVSettings.Instance.LibraryFolders)
        {
            if (folder == path)
            {
                continue;
            }
            cmbDefShowLocation.Items.Add(folder);
        }

        cmbDefShowLocation.Text = path;

        cmbDefShowLocation.EndUpdate();
    }

    private void PopulateReplacements(TVSettings s)
    {
        foreach (TVSettings.Replacement rep in s.Replacements)
        {
            AddNewReplacementRow(rep.This, rep.That, rep.CaseInsensitive);
        }
    }

    private void PopulateShowStatusColours(TVSettings s)
    {
        foreach (
            KeyValuePair<TVSettings.ColouringRule, Color> showStatusColor in
            s.ShowStatusColors)
        {
            ListViewItem item = new()
            {
                Text = showStatusColor.Key.Text,
                Tag = showStatusColor.Key,
                ForeColor = showStatusColor.Value
            };

            item.SubItems.Add(showStatusColor.Value.TranslateColorToHtml());
            lvwDefinedColors.Items.Add(item);
        }
    }

    private static void SetDropdownValue(DomainUpDown control, int sPeriodCheckHours)
    {
        foreach (object item in control.Items.Cast<object>().Where(item => item.ToString() == sPeriodCheckHours.ToString()))
        {
            control.SelectedItem = item;
        }
    }

    private static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>() => Enum.GetValues(typeof(T)).OfType<T>();
    }

    private void FillTreeViewColoringShowStatusTypeCombobox()
    {
        // Shows
        foreach (ShowConfiguration.ShowAirStatus x in EnumUtil.GetValues<ShowConfiguration.ShowAirStatus>())
        {
            cboShowStatus.Items.Add(new TVSettings.ShowAirStatusColouringRule(x));
        }

        foreach (string status in mDoc.TvLibrary.ShowStatuses)
        {
            cboShowStatus.Items.Add(new TVSettings.ShowStatusColouringRule(status));
        }

        // Seasons
        foreach (ProcessedSeason.SeasonStatus t in EnumUtil.GetValues<ProcessedSeason.SeasonStatus>())
        {
            cboShowStatus.Items.Add(new TVSettings.SeasonStatusColouringRule(t));
        }

        cboShowStatus.DisplayMember = "Text";
    }

    #endregion Update Form

    private void TxtNumberOnlyKeyPress(object sender, KeyPressEventArgs e)
    {
        // digits only
        if (e.KeyChar >= 32 && !char.IsDigit(e.KeyChar))
        {
            e.Handled = true;
        }
    }

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
            ((string)RSSGrid[rowsIndex[0], 0].Value).OpenUrlInBrowser();
        }
    }

    #endregion RSS OnClick Functionality

    #region enable and disable settings as appropriate

    private void EnableDisable()
    {
        bnRemoveDefinedColor.Enabled = lvwDefinedColors.SelectedItems.Count == 1;
        txtKeepTogether.Enabled = cbKeepTogether.Checked && cbKeepTogetherMode.Text != "All";
        gbRSS.Enabled = cbSearchRSS.Checked;
        gbJSON.Enabled = cbSearchJSON.Checked;
        groupBox22.Enabled = cbSearchJackett.Checked || chkSearchJackettButton.Checked;

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

        qBitTorrent.Enabled = cbCheckqBitTorrent.Checked;
        gbSAB.Enabled = cbCheckSABnzbd.Checked;
        gbuTorrent.Enabled = cbCheckuTorrent.Checked;
    }

    private void ExportersOptEnableDisable()
    {
        bool wtw = cbWTWRSS.Checked || cbWTWXML.Checked || cbWTWICAL.Checked || cbWTWTXT.Checked;
        label4.Enabled = wtw;
        label15.Enabled = wtw;
        label16.Enabled = wtw;
        label17.Enabled = wtw;
        txtExportRSSMaxDays.Enabled = wtw;
        txtExportRSSMaxShows.Enabled = wtw;
        txtExportRSSDaysPast.Enabled = wtw;

        SetEnabled(cbWTWRSS, txtWTWRSS, bnBrowseWTWRSS);
        SetEnabled(cbWTWXML, txtWTWXML, bnBrowseWTWXML);
        SetEnabled(cbWTWICAL, txtWTWICAL, bnBrowseWTWICAL);
        SetEnabled(cbWTWTXT, txtWTWTXT, bnBrowseWTWTXT);
        SetEnabled(cbM3U, txtM3U, bnBrowseM3U);
        SetEnabled(cbXSPF, txtXSPF, bnBrowseXSPF);
        SetEnabled(cbASX, txtASX, bnBrowseASX);
        SetEnabled(cbWPL, txtWPL, bnBrowseWPL);
        SetEnabled(cbFOXML, txtFOXML, bnBrowseFOXML);
        SetEnabled(cbShowsTXT, txtShowsTXTTo, bnBrowseShowsTXT);
        SetEnabled(cbShowsHTML, txtShowsHTMLTo, bnBrowseShowsHTML);
        SetEnabled(cbRenamingXML, txtRenamingXML, bnBrowseRenamingXML);
        SetEnabled(cbMissingXML, txtMissingXML, bnBrowseMissingXML);
        SetEnabled(cbMissingCSV, txtMissingCSV, bnBrowseMissingCSV);
        SetEnabled(cbMoviesTXT, txtMoviesTXTTo, bnBrowseMoviesTXT);
        SetEnabled(cbMoviesHTML, txtMoviesHTMLTo, bnBrowseMoviesHTML);
        SetEnabled(cbMissingMoviesXML, txtMissingMoviesXML, bnBrowseMissingMoviesXML);
        SetEnabled(cbMissingMoviesCSV, txtMissingMoviesCSV, bnBrowseMissingMoviesCSV);
    }

    private static void SetEnabled(CheckBox checkBox, TextBox txtMissingCsv, Button bnBrowseMissingCsv)
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
        cbSearchJackett.Enabled = e;

        cbSearchJSONManualScanOnly.Enabled = cbSearchJSON.Checked && e;
        cbSearchRSSManualScanOnly.Enabled = cbSearchRSS.Checked && e;
        cbSearchJackettOnManualScansOnly.Enabled = cbSearchJackett.Checked && e;
        chkSkipJackettFullScans.Enabled = cbSearchJackett.Checked && e;

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
            string from = (string)ReplacementsGrid[n, 0].Value;
            if (string.IsNullOrEmpty(from) ||
                !TVSettings.CompulsoryReplacements().Contains(from))
            {
                ReplacementsGrid.Rows.Remove(n);
            }
        }
    }

    private void btnAddShowStatusColoring_Click(object sender, EventArgs e)
    {
        if (cboShowStatus.SelectedItem == null || string.IsNullOrEmpty(txtShowStatusColor.Text))
        {
            return;
        }

        try
        {
            if (ColorTranslator.FromHtml(txtShowStatusColor.Text).IsEmpty ||
                cboShowStatus.SelectedItem is not TVSettings.ColouringRule ssct)
            {
                return;
            }

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
        catch
        {
            // ignored
        }
    }

    #endregion Replacement Rows OnClick Functionality

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

        if (UiHelpers.ShowDialogAndOk(colorDialog, this))
        {
            txtShowStatusColor.Text = colorDialog.Color.TranslateColorToHtml();
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

    #endregion ColourSelection OnClick Functionality

    private void Preferences_FormClosing(object sender, FormClosingEventArgs e)
    {
    }
    protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
    {
        base.ScaleControl(factor, specified);
        lvwDefinedColors.ScaleListViewColumns(factor);
    }

    private void cmDefaults_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
        if (e.ClickedItem?.Tag is not string tag || !int.TryParse(tag, out int v))
        {
            return;
        }

        switch (v)
        {
            case 1: // KODI
                cbEpTBNs.Checked = true;
                cbNFOShows.Checked = true;
                cbNFOEpisodes.Checked = true;
                cbNFOMovies.Checked = true;
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

    private void SuppressKeyPress(object _, KeyEventArgs e)
    {
        e.SuppressKeyPress = true;
    }

    private void tpSearch_DrawItem(object sender, DrawItemEventArgs e)
    {
        //Follow this advice https://docs.microsoft.com/en-us/dotnet/framework/winforms/controls/how-to-display-side-aligned-tabs-with-tabcontrol

        Graphics g = e.Graphics;

        using SolidBrush backColor = new (tcTabs.BackColor);
        g.FillRectangle(e.State == DrawItemState.Selected ? Brushes.White : backColor, e.Bounds);

        // Get the item from the collection.
        TabPage tabPage = tcTabs.TabPages[e.Index];

        // Get the real bounds for the tab rectangle.
        Rectangle tabBounds = tcTabs.GetTabRect(e.Index);

        // Draw string. Center the text.
        using StringFormat stringFlags = new();

        stringFlags.Alignment = StringAlignment.Near;
        stringFlags.LineAlignment = StringAlignment.Center;

        using SolidBrush fore = new (tcTabs.ForeColor);
        g.DrawString(tabPage.Text, tcTabs.Font, fore, tabBounds, stringFlags);
    }

    #region Folder Add & Remove

    private void bnAddSearchFolder_Click(object sender, EventArgs e)
    {
        //Setup the UI
        FolderBrowserDialogEx searchFolderBrowser = new()
        {
            SelectedPath = string.Empty,
            Title = "Add New Search Folder...",
            ShowEditbox = true,
            StartPosition = FormStartPosition.CenterParent
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

        TVSettings.Instance.DownloadFolders.Add(searchFolderBrowser.SelectedPath.Trim());
        mDoc.SetDirty();
        FillSearchFolderList();
    }

    private void bnRemoveSearchFolder_Click(object sender, EventArgs e)
    {
        RemoveSelectedSearchFolder();
    }

    private void RemoveSelectedSearchFolder()
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

        TVSettings.Instance.DownloadFolders[n].Trim().OpenFolder();
    }

    private void lbSearchFolders_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete)
        {
            RemoveSelectedSearchFolder();
        }
    }

    private void lbSearchFolders_DragOver(object sender, DragEventArgs e)
    {
        e.Effect = e.Data is not null && !e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.None : DragDropEffects.Copy;
    }

    private void lbSearchFolders_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data is not null)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string path in files)
            {
                try
                {
                    DirectoryInfo di = new(path);
                    if (di.Exists)
                    {
                        TVSettings.Instance.DownloadFolders.Add(path.ToLower().Trim());
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        mDoc.SetDirty();
        FillSearchFolderList();
    }

    private void bnRemoveMonFolder_Click(object sender, EventArgs e)
    {
        RemoveSelectedMonitorFolder();
    }

    private void RemoveSelectedMonitorFolder()
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

    private void RemoveSelectedMovieMonitorFolder()
    {
        for (int i = lstMovieMonitorFolders.SelectedIndices.Count - 1; i >= 0; i--)
        {
            int n = lstMovieMonitorFolders.SelectedIndices[i];
            TVSettings.Instance.MovieLibraryFolders.RemoveAt(n);
        }

        mDoc.SetDirty();
        FillMovieFolderStringLists();
        UpdateDefMovieLocation();
    }

    private void bnAddMonFolder_Click(object sender, EventArgs e)
    {
        FolderBrowserDialogEx searchFolderBrowser = new()
        {
            SelectedPath = string.Empty,
            Title = "Add New Library Folder...",
            ShowEditbox = true,
            StartPosition = FormStartPosition.CenterParent
        };

        if (lstFMMonitorFolders.SelectedIndex != -1)
        {
            int n = lstFMMonitorFolders.SelectedIndex;
            searchFolderBrowser.SelectedPath = TVSettings.Instance.LibraryFolders[n];
        }

        if (UiHelpers.ShowDialogAndOk(searchFolderBrowser, this) && Directory.Exists(searchFolderBrowser.SelectedPath))
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
            TVSettings.Instance.LibraryFolders[lstFMMonitorFolders.SelectedIndex].OpenFolder();
        }
    }

    private void bnOpenMovieMonFolder_Click(object sender, EventArgs e)
    {
        if (lstMovieMonitorFolders.SelectedIndex != -1)
        {
            TVSettings.Instance.MovieLibraryFolders[lstMovieMonitorFolders.SelectedIndex].OpenFolder();
        }
    }

    private void lstFMMonitorFolders_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete)
        {
            RemoveSelectedMonitorFolder();
        }
    }

    private void lstMovieMonitorFolders_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete)
        {
            RemoveSelectedMovieMonitorFolder();
        }
    }

    private void FileIcon_DragOver(object sender, DragEventArgs e)
    {
        e.Effect = e.Data is not null && e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void lstMovieMonitorFolders_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data is not null)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string path in files)
            {
                try
                {
                    DirectoryInfo di = new(path);
                    if (di.Exists)
                    {
                        TVSettings.Instance.MovieLibraryFolders.Add(path.ToLower());
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        mDoc.SetDirty();
        FillMovieFolderStringLists();
    }

    private void lstFMMonitorFolders_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data is not null)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string path in files)
            {
                try
                {
                    DirectoryInfo di = new(path);
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
        }

        mDoc.SetDirty();
        FillSearchFolderList();
    }

    private void lstFMMonitorFolders_SelectedIndexChanged(object sender, EventArgs e)
    {
        bnRemoveMonFolder.Enabled = lstFMMonitorFolders.SelectedIndices.Count > 0;
        bnOpenMonFolder.Enabled = lstFMMonitorFolders.SelectedIndices.Count > 0;
    }

    private void lstMovieMonitorFolders_SelectedIndexChanged(object sender, EventArgs e)
    {
        bnRemoveMovieMonFolder.Enabled = lstMovieMonitorFolders.SelectedIndices.Count > 0;
        bnOpenMovieMonFolder.Enabled = lstMovieMonitorFolders.SelectedIndices.Count > 0;
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
    private void pbFilesAndFolders_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-files-and-folders-tab");
    private void pbFolderDeleting_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-folder-deleting-tab");
    private void pbLibraryFolders_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-library-folders-tab");
    private void pictureBox1_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-torrents--nzb-tab");
    private void PictureBox1_Click_1(object sender, EventArgs e) => OpenInfoWindow("/#the-folder-defaults-tab");
    private void pictureBox7_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-media-center-tab");
    private void pbuUpdates_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-updates-tab");
    private void pbuExportEpisodes_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-episode-exporters-tab");
    private void pbuJackett_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-jackett-tab");
    private void pbuShowExport_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-show-exporters-tab");
    private void pbMovieDefaults_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-movie-defaults-tab");
    private void pbSources_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-data-sources-tab");
    private void pbScanOptions_Click(object sender, EventArgs e) => OpenInfoWindow("/#the-general-tab");
    private static void OpenInfoWindow(string page)
    {
        $"https://www.tvrename.com/manual/options{page}".OpenUrlInBrowser();
    }

    #endregion HelpWindows

    private void button1_Click(object sender, EventArgs e)
    {
        cntfw = new CustomNameTagsFloatingWindow(sampleProcessedSeason);
        cntfw.Show(this);
        Focus();
    }

    private void CbDefShowUseDefLocation_CheckedChanged(object sender, EventArgs e)
    {
        cmbDefShowLocation.Enabled = cbDefShowUseDefLocation.Checked;
    }

    private void LlJackettLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        llJackettLink.Text.OpenUrlInBrowser();
    }

    private void UpdateJackettLink()
    {
        llJackettLink.Text = $"http://{txtJackettServer.Text}:{txtJackettPort.Text}/UI/Dashboard";
    }

    private void EnableDisable(object sender, EventArgs e)
    {
        EnableDisable();
    }

    private void JackettDetailsUpdate(object sender, EventArgs e)
    {
        UpdateJackettLink();
    }

    private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        llqBitTorrentLink.Text.OpenUrlInBrowser();
    }

    private void QBitDetailsChanged(object sender, EventArgs e)
    {
        UpdateQBitTorrentLink();
    }

    private void UpdateQBitTorrentLink()
    {
        string protocol = chkBitTorrentUseHTTPS.Checked ? "https" : "http";
        llqBitTorrentLink.Text = $"{protocol}://{tbqBitTorrentHost.Text}:{tbqBitTorrentPort.Text}/";
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

    private void bnBrowseMissingMoviesXML_Click(object sender, EventArgs e) => Browse(txtMissingMoviesXML, "xml", 2);

    private void bnBrowseMissingMoviesCSV_Click(object sender, EventArgs e) => Browse(txtMissingMoviesCSV, "csv", 3);

    private void bnBrowseMoviesTXT_Click(object sender, EventArgs e) => Browse(txtMoviesTXTTo, "txt", 4);

    private void bnBrowseMoviesHTML_Click(object sender, EventArgs e) => Browse(txtMoviesHTMLTo, "html", 5);

    private void bnBrowseWTWTXT_Click(object sender, EventArgs e) => Browse(txtWTWTXT, "txt", 4);

    private void Browse(Control txt, string defaultExt, int filterIndex)
    {
        //rss =1, XML = 2, CSV = 3, TXT=4, HTML = 5
        saveFile.FileName = txt.Text;
        saveFile.DefaultExt = defaultExt;
        saveFile.FilterIndex = filterIndex;
        if (UiHelpers.ShowDialogAndOk(saveFile, this))
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

    private void Browse(TextBox txt, string filter)
    {
        openFile.FileName = txt.Text;
        openFile.Filter = filter;
        if (UiHelpers.ShowDialogAndOk(openFile, this))
        {
            txt.Text = openFile.FileName;
        }
    }

    #endregion PopupBrowseDialog

    private void bnAddMovieMonFolder_Click(object sender, EventArgs e)
    {
        FolderBrowserDialogEx searchFolderBrowser = new()
        {
            SelectedPath = string.Empty,
            Title = "Add New Library Folder...",
            ShowEditbox = true,
            StartPosition = FormStartPosition.CenterParent
        };

        if (lstMovieMonitorFolders.SelectedIndex != -1)
        {
            int n = lstMovieMonitorFolders.SelectedIndex;
            searchFolderBrowser.SelectedPath = TVSettings.Instance.MovieLibraryFolders[n];
        }

        if (UiHelpers.ShowDialogAndOk(searchFolderBrowser, this) && Directory.Exists(searchFolderBrowser.SelectedPath))
        {
            TVSettings.Instance.MovieLibraryFolders.Add(searchFolderBrowser.SelectedPath);
            mDoc.SetDirty();
            FillMovieFolderStringLists();
            UpdateDefMovieLocation();
        }
    }

    private void bnRemoveMovieMonFolder_Click(object sender, EventArgs e)
    {
        RemoveSelectedMovieMonitorFolder();
    }

    private void button2_Click(object sender, EventArgs e) => OpenMovieTags();

    private void OpenMovieTags()
    {
        MovieConfiguration? t = null;
        // ReSharper disable once ExpressionIsAlwaysNull
        cntfw = new CustomNameTagsFloatingWindow(t);
        cntfw.Show(this);
        Focus();
    }

    private void button3_Click(object sender, EventArgs e) => OpenMovieTags();

    private void chkUpdateCheckEnabled_CheckedChanged(object sender, EventArgs e)
    {
        grpUpdateIntervalOption.Enabled = chkUpdateCheckEnabled.Checked;
    }

    private void updateCheckOption_CheckedChanged(object sender, EventArgs e)
    {
        cboUpdateCheckInterval.Enabled = optUpdateCheckInterval.Checked;
    }

    private void cbDefShowDVDOrder_CheckedChanged(object sender, EventArgs e)
    {
        if (cbDefShowDVDOrder.Checked)
        {
            cbDefShowAlternateOrder.Checked = false;
        }
    }

    private void cbDefShowAlternateOrder_CheckedChanged(object sender, EventArgs e)
    {
        if (cbDefShowAlternateOrder.Checked)
        {
            cbDefShowDVDOrder.Checked = false;
        }
    }

    private void chkBitTorrentUseHTTPS_CheckedChanged(object sender, EventArgs e)
    {
        UpdateQBitTorrentLink();
    }

    private void button4_Click(object sender, EventArgs e)
    {
        ShowConfiguration? t = null;
        // ReSharper disable once ExpressionIsAlwaysNull
        cntfw = new CustomNameTagsFloatingWindow(t);
        cntfw.Show(this);
        Focus();
    }

    private void EnsureInteger(object sender, EventArgs e)
    {
        if (sender is not TextBox t)
        {
            return;
        }

        t.Text = t.Text.IntegerCharactersOnly();
    }
}
