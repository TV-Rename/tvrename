//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//


namespace TVRename.Forms
{
    partial class Preferences
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            this.cntfw?.Close();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Preferences));
            OKButton = new Button();
            bnCancel = new Button();
            saveFile = new SaveFileDialog();
            folderBrowser = new FolderBrowserDialog();
            openFile = new OpenFileDialog();
            toolTip1 = new ToolTip(components);
            cbMonitorFolder = new CheckBox();
            txtEmptyIgnoreExtensions = new TextBox();
            txtEmptyIgnoreWords = new TextBox();
            lbSearchFolders = new ListBox();
            lstFMMonitorFolders = new ListBox();
            tbIgnoreSuffixes = new TextBox();
            tbMovieTerms = new TextBox();
            txtKeepTogether = new TextBox();
            txtOtherExtensions = new TextBox();
            cbCopyFutureDatedEps = new CheckBox();
            label40 = new Label();
            domainUpDown2 = new DomainUpDown();
            tbSeasonSearchTerms = new TextBox();
            chkForceBulkAddToUseSettingsOnly = new CheckBox();
            cbIgnoreRecycleBin = new CheckBox();
            cbIgnoreNoVideoFolders = new CheckBox();
            label1 = new Label();
            upDownScanHours = new DomainUpDown();
            chkScheduledScan = new CheckBox();
            chkScanOnStartup = new CheckBox();
            chkIgnoreAllSpecials = new CheckBox();
            cbAutoSaveOnExit = new CheckBox();
            label84 = new Label();
            lstMovieMonitorFolders = new ListBox();
            chkIncludeMoviesQuickRecent = new CheckBox();
            tbCleanUpDownloadDirMoviesLength = new TextBox();
            label98 = new Label();
            upDownScanSeconds = new DomainUpDown();
            colorDialog = new ColorDialog();
            cmDefaults = new ContextMenuStrip(components);
            KODIToolStripMenuItem = new ToolStripMenuItem();
            pyTivoToolStripMenuItem = new ToolStripMenuItem();
            mede8erToolStripMenuItem = new ToolStripMenuItem();
            noneToolStripMenuItem = new ToolStripMenuItem();
            tpDisplay = new TabPage();
            chkShowAccessibilityOptions = new CheckBox();
            cbUseColoursOnWtw = new CheckBox();
            chkBasicShowDetails = new CheckBox();
            chkPostpendThe = new CheckBox();
            groupBox11 = new GroupBox();
            label7 = new Label();
            cboShowStatus = new ComboBox();
            label5 = new Label();
            txtShowStatusColor = new TextBox();
            btnSelectColor = new Button();
            bnRemoveDefinedColor = new Button();
            btnAddShowStatusColoring = new Button();
            lvwDefinedColors = new ListView();
            colShowStatus = new ColumnHeader();
            colColor = new ColumnHeader();
            label61 = new Label();
            cbLeadingZero = new CheckBox();
            txtSeasonFolderName = new TextBox();
            label35 = new Label();
            chkHideWtWSpoilers = new CheckBox();
            chkHideMyShowsSpoilers = new CheckBox();
            rbWTWScan = new RadioButton();
            rbWTWSearch = new RadioButton();
            cbStartupTab = new ComboBox();
            cbAutoSelInMyShows = new CheckBox();
            cbShowEpisodePictures = new CheckBox();
            label11 = new Label();
            label6 = new Label();
            chkShowInTaskbar = new CheckBox();
            cbNotificationIcon = new CheckBox();
            pbDisplay = new PictureBox();
            tpRSSJSONSearch = new TabPage();
            pbRSSJSONSearch = new PictureBox();
            label59 = new Label();
            cbSearchJSON = new CheckBox();
            cbSearchRSS = new CheckBox();
            gbJSON = new GroupBox();
            label78 = new Label();
            tbJSONSeedersToken = new TextBox();
            cbJSONCloudflareProtection = new CheckBox();
            cbSearchJSONManualScanOnly = new CheckBox();
            label55 = new Label();
            tbJSONFilesizeToken = new TextBox();
            label51 = new Label();
            tbJSONFilenameToken = new TextBox();
            label50 = new Label();
            tbJSONURLToken = new TextBox();
            label49 = new Label();
            tbJSONRootNode = new TextBox();
            label48 = new Label();
            tbJSONURL = new TextBox();
            gbRSS = new GroupBox();
            cbRSSCloudflareProtection = new CheckBox();
            cbSearchRSSManualScanOnly = new CheckBox();
            RSSGrid = new SourceGrid.Grid();
            label25 = new Label();
            bnRSSRemove = new Button();
            bnRSSGo = new Button();
            bnRSSAdd = new Button();
            tpLibraryFolders = new TabPage();
            groupBox23 = new GroupBox();
            button3 = new Button();
            txtMovieFilenameFormat = new TextBox();
            label90 = new Label();
            button2 = new Button();
            txtMovieFolderFormat = new TextBox();
            label85 = new Label();
            label87 = new Label();
            bnOpenMovieMonFolder = new Button();
            bnAddMovieMonFolder = new Button();
            bnRemoveMovieMonFolder = new Button();
            groupBox6 = new GroupBox();
            button4 = new Button();
            txtShowFolderFormat = new TextBox();
            label96 = new Label();
            button1 = new Button();
            txtSeasonFormat = new TextBox();
            txtSpecialsFolderName = new TextBox();
            label47 = new Label();
            label13 = new Label();
            label65 = new Label();
            label56 = new Label();
            bnOpenMonFolder = new Button();
            bnAddMonFolder = new Button();
            bnRemoveMonFolder = new Button();
            pbLibraryFolders = new PictureBox();
            tpTorrentNZB = new TabPage();
            pbuTorrentNZB = new PictureBox();
            label58 = new Label();
            cbCheckqBitTorrent = new CheckBox();
            cbCheckSABnzbd = new CheckBox();
            cbCheckuTorrent = new CheckBox();
            qBitTorrent = new GroupBox();
            chkBitTorrentUseHTTPS = new CheckBox();
            chkRemoveCompletedTorrents = new CheckBox();
            llqBitTorrentLink = new LinkLabel();
            label79 = new Label();
            rdoqBitTorrentAPIVersionv2 = new RadioButton();
            rdoqBitTorrentAPIVersionv1 = new RadioButton();
            rdoqBitTorrentAPIVersionv0 = new RadioButton();
            label29 = new Label();
            cbDownloadTorrentBeforeDownloading = new CheckBox();
            tbqBitTorrentHost = new TextBox();
            tbqBitTorrentPort = new TextBox();
            label41 = new Label();
            label42 = new Label();
            gbSAB = new GroupBox();
            txtSABHostPort = new TextBox();
            txtSABAPIKey = new TextBox();
            label8 = new Label();
            label9 = new Label();
            gbuTorrent = new GroupBox();
            bnUTBrowseResumeDat = new Button();
            txtUTResumeDatPath = new TextBox();
            bnRSSBrowseuTorrent = new Button();
            label27 = new Label();
            label26 = new Label();
            txtRSSuTorrentPath = new TextBox();
            tbSearchFolders = new TabPage();
            chkUseSearchFullPathWhenMatchingShows = new CheckBox();
            groupBox8 = new GroupBox();
            cbMovieHigherQuality = new CheckBox();
            label53 = new Label();
            label54 = new Label();
            tbPercentBetter = new TextBox();
            tbPriorityOverrideTerms = new TextBox();
            label52 = new Label();
            cbHigherQuality = new CheckBox();
            label67 = new Label();
            gbAutoAdd = new GroupBox();
            cbAutomateAutoAddWhenOneMovieFound = new CheckBox();
            cbAutomateAutoAddWhenOneShowFound = new CheckBox();
            chkAutoSearchForDownloadedFiles = new CheckBox();
            label43 = new Label();
            label44 = new Label();
            cbLeaveOriginals = new CheckBox();
            cbSearchLocally = new CheckBox();
            chkAutoMergeDownloadEpisodes = new CheckBox();
            bnOpenSearchFolder = new Button();
            bnRemoveSearchFolder = new Button();
            bnAddSearchFolder = new Button();
            pbSearchFolders = new PictureBox();
            label23 = new Label();
            tbMediaCenter = new TabPage();
            groupBox16 = new GroupBox();
            cbWDLiveEpisodeFiles = new CheckBox();
            groupBox13 = new GroupBox();
            cbXMLFiles = new CheckBox();
            cbSeriesJpg = new CheckBox();
            cbShrinkLarge = new CheckBox();
            groupBox14 = new GroupBox();
            cbMeta = new CheckBox();
            cbMetaSubfolder = new CheckBox();
            groupBox15 = new GroupBox();
            cbNFOMovies = new CheckBox();
            cbEpTBNs = new CheckBox();
            cbNFOShows = new CheckBox();
            cbKODIImages = new CheckBox();
            cbNFOEpisodes = new CheckBox();
            groupBox12 = new GroupBox();
            cbFantArtJpg = new CheckBox();
            cbFolderJpg = new CheckBox();
            cbEpThumbJpg = new CheckBox();
            panel1 = new Panel();
            rbFolderBanner = new RadioButton();
            rbFolderPoster = new RadioButton();
            rbFolderFanArt = new RadioButton();
            rbFolderSeasonPoster = new RadioButton();
            label64 = new Label();
            bnMCPresets = new Button();
            pbMediaCenter = new PictureBox();
            tbFolderDeleting = new TabPage();
            cbDeleteMovieFromDisk = new CheckBox();
            groupBox28 = new GroupBox();
            cbCleanUpDownloadDirMoviesLength = new CheckBox();
            cbCleanUpDownloadDirMovies = new CheckBox();
            cbCleanUpDownloadDir = new CheckBox();
            label69 = new Label();
            cbDeleteShowFromDisk = new CheckBox();
            label32 = new Label();
            label30 = new Label();
            txtEmptyMaxSize = new TextBox();
            label31 = new Label();
            cbRecycleNotDelete = new CheckBox();
            cbEmptyMaxSize = new CheckBox();
            cbEmptyIgnoreWords = new CheckBox();
            cbEmptyIgnoreExtensions = new CheckBox();
            cbDeleteEmpty = new CheckBox();
            pbFolderDeleting = new PictureBox();
            tbAutoExport = new TabPage();
            pbuExportEpisodes = new PictureBox();
            label88 = new Label();
            groupBox10 = new GroupBox();
            bnBrowseWPL = new Button();
            txtWPL = new TextBox();
            cbWPL = new CheckBox();
            bnBrowseASX = new Button();
            txtASX = new TextBox();
            cbASX = new CheckBox();
            bnBrowseM3U = new Button();
            txtM3U = new TextBox();
            cbM3U = new CheckBox();
            bnBrowseXSPF = new Button();
            txtXSPF = new TextBox();
            cbXSPF = new CheckBox();
            groupBox5 = new GroupBox();
            bnBrowseFOXML = new Button();
            cbFOXML = new CheckBox();
            txtFOXML = new TextBox();
            groupBox4 = new GroupBox();
            bnBrowseRenamingXML = new Button();
            cbRenamingXML = new CheckBox();
            txtRenamingXML = new TextBox();
            groupBox2 = new GroupBox();
            bnBrowseWTWTXT = new Button();
            txtWTWTXT = new TextBox();
            cbWTWTXT = new CheckBox();
            bnBrowseWTWICAL = new Button();
            txtWTWICAL = new TextBox();
            cbWTWICAL = new CheckBox();
            label4 = new Label();
            txtExportRSSDaysPast = new TextBox();
            bnBrowseWTWXML = new Button();
            txtWTWXML = new TextBox();
            cbWTWXML = new CheckBox();
            bnBrowseWTWRSS = new Button();
            txtWTWRSS = new TextBox();
            cbWTWRSS = new CheckBox();
            label17 = new Label();
            label16 = new Label();
            label15 = new Label();
            txtExportRSSMaxDays = new TextBox();
            txtExportRSSMaxShows = new TextBox();
            tbFilesAndFolders = new TabPage();
            chkUnArchiveFilesInDownloadDirectory = new CheckBox();
            cbFileNameCaseSensitiveMatch = new CheckBox();
            chkUseLibraryFullPathWhenMatchingShows = new CheckBox();
            label66 = new Label();
            txtMaxSampleSize = new TextBox();
            txtVideoExtensions = new TextBox();
            label39 = new Label();
            cbKeepTogetherMode = new ComboBox();
            bnReplaceRemove = new Button();
            bnReplaceAdd = new Button();
            label3 = new Label();
            ReplacementsGrid = new SourceGrid.Grid();
            label19 = new Label();
            label22 = new Label();
            label14 = new Label();
            cbKeepTogether = new CheckBox();
            cbForceLower = new CheckBox();
            cbIgnoreSamples = new CheckBox();
            pbFilesAndFolders = new PictureBox();
            tbGeneral = new TabPage();
            chkAutoAddAsPartOfQuickRename = new CheckBox();
            chkShareCriticalLogs = new CheckBox();
            label60 = new Label();
            pbGeneral = new PictureBox();
            txtWTWDays = new TextBox();
            cbMode = new ComboBox();
            label34 = new Label();
            label2 = new Label();
            tcTabs = new TabControl();
            tpDataSources = new TabPage();
            panel4 = new Panel();
            rdoGlobalReleaseDates = new RadioButton();
            rdoRegionalReleaseDates = new RadioButton();
            panel3 = new Panel();
            rdoMovieTMDB = new RadioButton();
            rdoMovieTheTVDB = new RadioButton();
            label100 = new Label();
            panel2 = new Panel();
            rdoTVTMDB = new RadioButton();
            rdoTVTVMaze = new RadioButton();
            rdoTVTVDB = new RadioButton();
            label83 = new Label();
            gbTMDB = new GroupBox();
            cbTMDBRegions = new ComboBox();
            label80 = new Label();
            label81 = new Label();
            tbTMDBPercentDirty = new TextBox();
            label82 = new Label();
            cbTMDBLanguages = new ComboBox();
            label63 = new Label();
            label57 = new Label();
            txtParallelDownloads = new TextBox();
            label21 = new Label();
            label20 = new Label();
            groupBox21 = new GroupBox();
            groupBox20 = new GroupBox();
            label91 = new Label();
            cbTVDBVersion = new ComboBox();
            label37 = new Label();
            label38 = new Label();
            tbPercentDirty = new TextBox();
            label10 = new Label();
            cbTVDBLanguages = new ComboBox();
            label33 = new Label();
            pbSources = new PictureBox();
            tpMovieDefaults = new TabPage();
            label86 = new Label();
            groupBox24 = new GroupBox();
            cmbDefMovieFolderFormat = new ComboBox();
            label95 = new Label();
            cmbDefMovieLocation = new ComboBox();
            cbDefMovieUseDefLocation = new CheckBox();
            cbDefMovieAutoFolders = new CheckBox();
            groupBox25 = new GroupBox();
            cbDefMovieIncludeNoAirdate = new CheckBox();
            cbDefMovieIncludeFuture = new CheckBox();
            cbDefMovieDoMissing = new CheckBox();
            cbDefMovieDoRenaming = new CheckBox();
            pbMovieDefaults = new PictureBox();
            tpShowDefaults = new TabPage();
            label18 = new Label();
            groupBox19 = new GroupBox();
            label24 = new Label();
            cbTimeZone = new ComboBox();
            rbDefShowUseSubFolders = new RadioButton();
            rbDefShowUseBase = new RadioButton();
            label12 = new Label();
            cmbDefShowLocation = new ComboBox();
            cbDefShowUseDefLocation = new CheckBox();
            cbDefShowAutoFolders = new CheckBox();
            groupBox18 = new GroupBox();
            cbDefShowAlternateOrder = new CheckBox();
            cbDefShowEpNameMatching = new CheckBox();
            label68 = new Label();
            cbDefShowAirdateMatching = new CheckBox();
            cbDefShowSpecialsCount = new CheckBox();
            cbDefShowSequentialMatching = new CheckBox();
            cbDefShowIncludeNoAirdate = new CheckBox();
            cbDefShowDoMissingCheck = new CheckBox();
            cbDefShowDVDOrder = new CheckBox();
            cbDefShowIncludeFuture = new CheckBox();
            cbDefShowDoRenaming = new CheckBox();
            cbDefShowNextAirdate = new CheckBox();
            pictureBox1 = new PictureBox();
            tpScanSettings = new TabPage();
            chkGroupMissingEpisodesIntoSeasons = new CheckBox();
            groupBox17 = new GroupBox();
            cbIgnorePreviouslySeenMovies = new CheckBox();
            chkMoveLibraryFiles = new CheckBox();
            lblScanAction = new Label();
            rdoQuickScan = new RadioButton();
            rdoRecentScan = new RadioButton();
            rdoFullScan = new RadioButton();
            cbIgnorePreviouslySeen = new CheckBox();
            chkPreventMove = new CheckBox();
            label28 = new Label();
            cbRenameCheck = new CheckBox();
            cbMissing = new CheckBox();
            groupBox1 = new GroupBox();
            chkChooseWhenMultipleEpisodesMatch = new CheckBox();
            cbxUpdateAirDate = new CheckBox();
            cbAutoCreateFolders = new CheckBox();
            chkAutoMergeLibraryEpisodes = new CheckBox();
            cbScanIncludesBulkAdd = new CheckBox();
            gbBulkAdd = new GroupBox();
            label36 = new Label();
            label62 = new Label();
            pbScanOptions = new PictureBox();
            tpSubtitles = new TabPage();
            groupBox29 = new GroupBox();
            label94 = new Label();
            txtSubtitleFolderNames = new TextBox();
            cbCopySubsFolders = new CheckBox();
            label93 = new Label();
            pictureBox2 = new PictureBox();
            groupBox9 = new GroupBox();
            cbTxtToSub = new CheckBox();
            label46 = new Label();
            txtSubtitleExtensions = new TextBox();
            chkRetainLanguageSpecificSubtitles = new CheckBox();
            tpJackett = new TabPage();
            label99 = new Label();
            txtMinRSSSeeders = new TextBox();
            label97 = new Label();
            tbUnwantedRSSTerms = new TextBox();
            chkSearchJackettButton = new CheckBox();
            cmbSupervisedDuplicateAction = new ComboBox();
            label77 = new Label();
            cmbUnattendedDuplicateAction = new ComboBox();
            label76 = new Label();
            cbDetailedRSSJSONLogging = new CheckBox();
            pbuJackett = new PictureBox();
            label70 = new Label();
            cbSearchJackett = new CheckBox();
            groupBox22 = new GroupBox();
            chkUseJackettTextSearch = new CheckBox();
            chkSkipJackettFullScans = new CheckBox();
            llJackettLink = new LinkLabel();
            label71 = new Label();
            cbSearchJackettOnManualScansOnly = new CheckBox();
            label72 = new Label();
            txtJackettIndexer = new TextBox();
            label73 = new Label();
            txtJackettAPIKey = new TextBox();
            label74 = new Label();
            txtJackettPort = new TextBox();
            label75 = new Label();
            txtJackettServer = new TextBox();
            label45 = new Label();
            tbPreferredRSSTerms = new TextBox();
            tpAutoExportLibrary = new TabPage();
            chkRestrictMissingExportsToFullScans = new CheckBox();
            pbuShowExport = new PictureBox();
            label89 = new Label();
            groupBox26 = new GroupBox();
            bnBrowseMoviesHTML = new Button();
            cbMoviesHTML = new CheckBox();
            txtMoviesHTMLTo = new TextBox();
            bnBrowseMoviesTXT = new Button();
            cbMoviesTXT = new CheckBox();
            txtMoviesTXTTo = new TextBox();
            groupBox7 = new GroupBox();
            bnBrowseShowsHTML = new Button();
            cbShowsHTML = new CheckBox();
            txtShowsHTMLTo = new TextBox();
            bnBrowseShowsTXT = new Button();
            cbShowsTXT = new CheckBox();
            txtShowsTXTTo = new TextBox();
            groupBox27 = new GroupBox();
            bnBrowseMissingMoviesCSV = new Button();
            bnBrowseMissingMoviesXML = new Button();
            txtMissingMoviesCSV = new TextBox();
            cbMissingMoviesXML = new CheckBox();
            cbMissingMoviesCSV = new CheckBox();
            txtMissingMoviesXML = new TextBox();
            groupBox3 = new GroupBox();
            bnBrowseMissingCSV = new Button();
            bnBrowseMissingXML = new Button();
            txtMissingCSV = new TextBox();
            cbMissingXML = new CheckBox();
            cbMissingCSV = new CheckBox();
            txtMissingXML = new TextBox();
            tbAppUpdate = new TabPage();
            pbuUpdates = new PictureBox();
            chkUpdateCheckEnabled = new CheckBox();
            label92 = new Label();
            grpUpdateIntervalOption = new GroupBox();
            chkNoPopupOnUpdate = new CheckBox();
            cboUpdateCheckInterval = new ComboBox();
            optUpdateCheckInterval = new RadioButton();
            optUpdateCheckAlways = new RadioButton();
            cmDefaults.SuspendLayout();
            tpDisplay.SuspendLayout();
            groupBox11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbDisplay).BeginInit();
            tpRSSJSONSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbRSSJSONSearch).BeginInit();
            gbJSON.SuspendLayout();
            gbRSS.SuspendLayout();
            tpLibraryFolders.SuspendLayout();
            groupBox23.SuspendLayout();
            groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbLibraryFolders).BeginInit();
            tpTorrentNZB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbuTorrentNZB).BeginInit();
            qBitTorrent.SuspendLayout();
            gbSAB.SuspendLayout();
            gbuTorrent.SuspendLayout();
            tbSearchFolders.SuspendLayout();
            groupBox8.SuspendLayout();
            gbAutoAdd.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbSearchFolders).BeginInit();
            tbMediaCenter.SuspendLayout();
            groupBox16.SuspendLayout();
            groupBox13.SuspendLayout();
            groupBox14.SuspendLayout();
            groupBox15.SuspendLayout();
            groupBox12.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbMediaCenter).BeginInit();
            tbFolderDeleting.SuspendLayout();
            groupBox28.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbFolderDeleting).BeginInit();
            tbAutoExport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbuExportEpisodes).BeginInit();
            groupBox10.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox2.SuspendLayout();
            tbFilesAndFolders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbFilesAndFolders).BeginInit();
            tbGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbGeneral).BeginInit();
            tcTabs.SuspendLayout();
            tpDataSources.SuspendLayout();
            panel4.SuspendLayout();
            panel3.SuspendLayout();
            panel2.SuspendLayout();
            gbTMDB.SuspendLayout();
            groupBox20.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbSources).BeginInit();
            tpMovieDefaults.SuspendLayout();
            groupBox24.SuspendLayout();
            groupBox25.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbMovieDefaults).BeginInit();
            tpShowDefaults.SuspendLayout();
            groupBox19.SuspendLayout();
            groupBox18.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            tpScanSettings.SuspendLayout();
            groupBox17.SuspendLayout();
            groupBox1.SuspendLayout();
            gbBulkAdd.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbScanOptions).BeginInit();
            tpSubtitles.SuspendLayout();
            groupBox29.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            groupBox9.SuspendLayout();
            tpJackett.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbuJackett).BeginInit();
            groupBox22.SuspendLayout();
            tpAutoExportLibrary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbuShowExport).BeginInit();
            groupBox26.SuspendLayout();
            groupBox7.SuspendLayout();
            groupBox27.SuspendLayout();
            groupBox3.SuspendLayout();
            tbAppUpdate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbuUpdates).BeginInit();
            grpUpdateIntervalOption.SuspendLayout();
            SuspendLayout();
            // 
            // OKButton
            // 
            OKButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            OKButton.Location = new Point(481, 713);
            OKButton.Margin = new Padding(4, 3, 4, 3);
            OKButton.Name = "OKButton";
            OKButton.Size = new Size(88, 27);
            OKButton.TabIndex = 0;
            OKButton.Text = "OK";
            OKButton.UseVisualStyleBackColor = true;
            OKButton.Click += OKButton_Click;
            // 
            // bnCancel
            // 
            bnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            bnCancel.DialogResult = DialogResult.Cancel;
            bnCancel.Location = new Point(580, 713);
            bnCancel.Margin = new Padding(4, 3, 4, 3);
            bnCancel.Name = "bnCancel";
            bnCancel.Size = new Size(88, 27);
            bnCancel.TabIndex = 1;
            bnCancel.Text = "Cancel";
            bnCancel.UseVisualStyleBackColor = true;
            bnCancel.Click += CancelButton_Click;
            // 
            // saveFile
            // 
            saveFile.Filter = resources.GetString("saveFile.Filter");
            // 
            // folderBrowser
            // 
            folderBrowser.ShowNewFolderButton = false;
            // 
            // openFile
            // 
            openFile.Filter = "Torrent files (*.torrent)|*.torrent|All files (*.*)|*.*";
            // 
            // cbMonitorFolder
            // 
            cbMonitorFolder.AutoSize = true;
            cbMonitorFolder.Location = new Point(7, 147);
            cbMonitorFolder.Margin = new Padding(4, 3, 4, 3);
            cbMonitorFolder.Name = "cbMonitorFolder";
            cbMonitorFolder.Size = new Size(338, 19);
            cbMonitorFolder.TabIndex = 5;
            cbMonitorFolder.Text = "&Monitor Search Folders (run a scan when files change) after";
            toolTip1.SetToolTip(cbMonitorFolder, "If the contents of any of these folder change, then automatically do a \"Scan\" and \"Do\".");
            cbMonitorFolder.UseVisualStyleBackColor = true;
            // 
            // txtEmptyIgnoreExtensions
            // 
            txtEmptyIgnoreExtensions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtEmptyIgnoreExtensions.Location = new Point(111, 186);
            txtEmptyIgnoreExtensions.Margin = new Padding(4, 3, 4, 3);
            txtEmptyIgnoreExtensions.Name = "txtEmptyIgnoreExtensions";
            txtEmptyIgnoreExtensions.Size = new Size(359, 23);
            txtEmptyIgnoreExtensions.TabIndex = 5;
            toolTip1.SetToolTip(txtEmptyIgnoreExtensions, "For example \".par2;.nzb;.nfo\"");
            // 
            // txtEmptyIgnoreWords
            // 
            txtEmptyIgnoreWords.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtEmptyIgnoreWords.Location = new Point(111, 128);
            txtEmptyIgnoreWords.Margin = new Padding(4, 3, 4, 3);
            txtEmptyIgnoreWords.Name = "txtEmptyIgnoreWords";
            txtEmptyIgnoreWords.Size = new Size(359, 23);
            txtEmptyIgnoreWords.TabIndex = 3;
            toolTip1.SetToolTip(txtEmptyIgnoreWords, "For example \"sample\"");
            // 
            // lbSearchFolders
            // 
            lbSearchFolders.AllowDrop = true;
            lbSearchFolders.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lbSearchFolders.FormattingEnabled = true;
            lbSearchFolders.Location = new Point(6, 239);
            lbSearchFolders.Margin = new Padding(4, 3, 4, 3);
            lbSearchFolders.Name = "lbSearchFolders";
            lbSearchFolders.ScrollAlwaysVisible = true;
            lbSearchFolders.Size = new Size(460, 79);
            lbSearchFolders.TabIndex = 1;
            toolTip1.SetToolTip(lbSearchFolders, "Search Folders\r\n\r\nThe Search Folders are where new, unsorted\r\nfiles are to be found. The scan will look for\r\nmissing episodes in this location. It should not\r\noverlap with the 'Library Folders' above.");
            lbSearchFolders.SelectedIndexChanged += lbSearchFolders_SelectedIndexChanged;
            lbSearchFolders.DragDrop += lbSearchFolders_DragDrop;
            lbSearchFolders.DragOver += lbSearchFolders_DragOver;
            lbSearchFolders.KeyDown += lbSearchFolders_KeyDown;
            // 
            // lstFMMonitorFolders
            // 
            lstFMMonitorFolders.AllowDrop = true;
            lstFMMonitorFolders.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstFMMonitorFolders.FormattingEnabled = true;
            lstFMMonitorFolders.IntegralHeight = false;
            lstFMMonitorFolders.Location = new Point(7, 80);
            lstFMMonitorFolders.Margin = new Padding(4, 3, 4, 3);
            lstFMMonitorFolders.Name = "lstFMMonitorFolders";
            lstFMMonitorFolders.ScrollAlwaysVisible = true;
            lstFMMonitorFolders.SelectionMode = SelectionMode.MultiExtended;
            lstFMMonitorFolders.Size = new Size(464, 111);
            lstFMMonitorFolders.TabIndex = 32;
            toolTip1.SetToolTip(lstFMMonitorFolders, resources.GetString("lstFMMonitorFolders.ToolTip"));
            lstFMMonitorFolders.SelectedIndexChanged += lstFMMonitorFolders_SelectedIndexChanged;
            lstFMMonitorFolders.DragDrop += lstFMMonitorFolders_DragDrop;
            lstFMMonitorFolders.DragOver += FileIcon_DragOver;
            lstFMMonitorFolders.KeyDown += lstFMMonitorFolders_KeyDown;
            // 
            // tbIgnoreSuffixes
            // 
            tbIgnoreSuffixes.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbIgnoreSuffixes.Location = new Point(117, 117);
            tbIgnoreSuffixes.Margin = new Padding(4, 3, 4, 3);
            tbIgnoreSuffixes.Name = "tbIgnoreSuffixes";
            tbIgnoreSuffixes.Size = new Size(344, 23);
            tbIgnoreSuffixes.TabIndex = 15;
            toolTip1.SetToolTip(tbIgnoreSuffixes, "These terms and any text after them will be ignored when\r\nsearching on TVDB for the show title based on the filename.");
            // 
            // tbMovieTerms
            // 
            tbMovieTerms.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbMovieTerms.Location = new Point(117, 87);
            tbMovieTerms.Margin = new Padding(4, 3, 4, 3);
            tbMovieTerms.Name = "tbMovieTerms";
            tbMovieTerms.Size = new Size(344, 23);
            tbMovieTerms.TabIndex = 13;
            toolTip1.SetToolTip(tbMovieTerms, "If a filename contains any of these terms then it is assumed\r\nthat it is a Film and not a TV Show. Hence 'Auto Add' is not\r\ninvoked for this file.");
            // 
            // txtKeepTogether
            // 
            txtKeepTogether.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtKeepTogether.Location = new Point(238, 359);
            txtKeepTogether.Margin = new Padding(4, 3, 4, 3);
            txtKeepTogether.Name = "txtKeepTogether";
            txtKeepTogether.Size = new Size(226, 23);
            txtKeepTogether.TabIndex = 23;
            toolTip1.SetToolTip(txtKeepTogether, "Which file extensions should be copied from the Search\r\nFolders into the library?");
            // 
            // txtOtherExtensions
            // 
            txtOtherExtensions.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtOtherExtensions.Location = new Point(115, 299);
            txtOtherExtensions.Margin = new Padding(4, 3, 4, 3);
            txtOtherExtensions.Name = "txtOtherExtensions";
            txtOtherExtensions.Size = new Size(348, 23);
            txtOtherExtensions.TabIndex = 7;
            toolTip1.SetToolTip(txtOtherExtensions, "Which file extensions in the library should be renamed along\r\nwith the video files?");
            // 
            // cbCopyFutureDatedEps
            // 
            cbCopyFutureDatedEps.AutoSize = true;
            cbCopyFutureDatedEps.Location = new Point(7, 173);
            cbCopyFutureDatedEps.Margin = new Padding(4, 3, 4, 3);
            cbCopyFutureDatedEps.Name = "cbCopyFutureDatedEps";
            cbCopyFutureDatedEps.Size = new Size(298, 19);
            cbCopyFutureDatedEps.TabIndex = 41;
            cbCopyFutureDatedEps.Text = "Copy future dated episodes found in Search Folders";
            toolTip1.SetToolTip(cbCopyFutureDatedEps, "If set then any episodes in the search folders will be copied into the library, even if they are yet to officially air.");
            cbCopyFutureDatedEps.UseVisualStyleBackColor = true;
            // 
            // label40
            // 
            label40.AutoSize = true;
            label40.Location = new Point(188, 173);
            label40.Margin = new Padding(4, 0, 4, 0);
            label40.Name = "label40";
            label40.Size = new Size(37, 15);
            label40.TabIndex = 54;
            label40.Text = "hours";
            toolTip1.SetToolTip(label40, "If checked the system will automatically scan and complete actions on a periodic schedule");
            // 
            // domainUpDown2
            // 
            domainUpDown2.Items.Add("96");
            domainUpDown2.Items.Add("48");
            domainUpDown2.Items.Add("24");
            domainUpDown2.Items.Add("12");
            domainUpDown2.Items.Add("8");
            domainUpDown2.Items.Add("6");
            domainUpDown2.Items.Add("5");
            domainUpDown2.Items.Add("4");
            domainUpDown2.Items.Add("3");
            domainUpDown2.Items.Add("2");
            domainUpDown2.Items.Add("1");
            domainUpDown2.Location = new Point(136, 169);
            domainUpDown2.Margin = new Padding(4, 3, 4, 3);
            domainUpDown2.Name = "domainUpDown2";
            domainUpDown2.Size = new Size(47, 23);
            domainUpDown2.TabIndex = 53;
            toolTip1.SetToolTip(domainUpDown2, "How often should TV Rename update itself from upstream data sources?");
            domainUpDown2.KeyDown += SuppressKeyPress;
            // 
            // tbSeasonSearchTerms
            // 
            tbSeasonSearchTerms.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbSeasonSearchTerms.Location = new Point(147, 98);
            tbSeasonSearchTerms.Margin = new Padding(4, 3, 4, 3);
            tbSeasonSearchTerms.Name = "tbSeasonSearchTerms";
            tbSeasonSearchTerms.Size = new Size(300, 23);
            tbSeasonSearchTerms.TabIndex = 22;
            toolTip1.SetToolTip(tbSeasonSearchTerms, "Which terms should the system look for in directory\r\nnames that indicate that the folder contains a season's\r\nworth of episodes for a show.\r\nThey should be separated by a semi-colon - ; ");
            // 
            // chkForceBulkAddToUseSettingsOnly
            // 
            chkForceBulkAddToUseSettingsOnly.AutoSize = true;
            chkForceBulkAddToUseSettingsOnly.Location = new Point(7, 75);
            chkForceBulkAddToUseSettingsOnly.Margin = new Padding(4, 3, 4, 3);
            chkForceBulkAddToUseSettingsOnly.Name = "chkForceBulkAddToUseSettingsOnly";
            chkForceBulkAddToUseSettingsOnly.Size = new Size(270, 19);
            chkForceBulkAddToUseSettingsOnly.TabIndex = 15;
            chkForceBulkAddToUseSettingsOnly.Text = "Force to Use Season Words from Settings Only";
            toolTip1.SetToolTip(chkForceBulkAddToUseSettingsOnly, "If set then Bulk Add just uses the season words from settings. If not set (recommended) then Bulk Add finds addition season words from each show's configuration.");
            chkForceBulkAddToUseSettingsOnly.UseVisualStyleBackColor = true;
            // 
            // cbIgnoreRecycleBin
            // 
            cbIgnoreRecycleBin.AutoSize = true;
            cbIgnoreRecycleBin.Location = new Point(7, 48);
            cbIgnoreRecycleBin.Margin = new Padding(4, 3, 4, 3);
            cbIgnoreRecycleBin.Name = "cbIgnoreRecycleBin";
            cbIgnoreRecycleBin.Size = new Size(123, 19);
            cbIgnoreRecycleBin.TabIndex = 14;
            cbIgnoreRecycleBin.Text = "Ignore &Recycle Bin";
            toolTip1.SetToolTip(cbIgnoreRecycleBin, "If set then Bulk Add ignores all files in the Recycle Bin");
            cbIgnoreRecycleBin.UseVisualStyleBackColor = true;
            // 
            // cbIgnoreNoVideoFolders
            // 
            cbIgnoreNoVideoFolders.AutoSize = true;
            cbIgnoreNoVideoFolders.Location = new Point(7, 22);
            cbIgnoreNoVideoFolders.Margin = new Padding(4, 3, 4, 3);
            cbIgnoreNoVideoFolders.Name = "cbIgnoreNoVideoFolders";
            cbIgnoreNoVideoFolders.Size = new Size(251, 19);
            cbIgnoreNoVideoFolders.TabIndex = 13;
            cbIgnoreNoVideoFolders.Text = "&Only Include Folders containing Video files";
            toolTip1.SetToolTip(cbIgnoreNoVideoFolders, "If set then only folders that contain video files are considered for the 'Bulk Add' feature");
            cbIgnoreNoVideoFolders.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(216, 96);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(37, 15);
            label1.TabIndex = 47;
            label1.Text = "hours";
            toolTip1.SetToolTip(label1, "If checked the system will automatically scan and complete actions on a periodic schedule");
            // 
            // upDownScanHours
            // 
            upDownScanHours.Items.Add("96");
            upDownScanHours.Items.Add("48");
            upDownScanHours.Items.Add("24");
            upDownScanHours.Items.Add("12");
            upDownScanHours.Items.Add("8");
            upDownScanHours.Items.Add("6");
            upDownScanHours.Items.Add("5");
            upDownScanHours.Items.Add("4");
            upDownScanHours.Items.Add("3");
            upDownScanHours.Items.Add("2");
            upDownScanHours.Items.Add("1");
            upDownScanHours.Location = new Point(164, 93);
            upDownScanHours.Margin = new Padding(4, 3, 4, 3);
            upDownScanHours.Name = "upDownScanHours";
            upDownScanHours.Size = new Size(47, 23);
            upDownScanHours.TabIndex = 46;
            toolTip1.SetToolTip(upDownScanHours, "If checked the system will automatically scan and complete actions on a periodic schedule");
            // 
            // chkScheduledScan
            // 
            chkScheduledScan.AutoSize = true;
            chkScheduledScan.Location = new Point(12, 95);
            chkScheduledScan.Margin = new Padding(4, 3, 4, 3);
            chkScheduledScan.Name = "chkScheduledScan";
            chkScheduledScan.Size = new Size(142, 19);
            chkScheduledScan.TabIndex = 45;
            chkScheduledScan.Text = "Sc&heduled scan every ";
            toolTip1.SetToolTip(chkScheduledScan, "If checked the system will automatically scan and complete actions on a periodic schedule");
            chkScheduledScan.UseVisualStyleBackColor = true;
            // 
            // chkScanOnStartup
            // 
            chkScanOnStartup.AutoSize = true;
            chkScanOnStartup.Location = new Point(12, 68);
            chkScanOnStartup.Margin = new Padding(4, 3, 4, 3);
            chkScanOnStartup.Name = "chkScanOnStartup";
            chkScanOnStartup.Size = new Size(109, 19);
            chkScanOnStartup.TabIndex = 44;
            chkScanOnStartup.Text = "&Scan on Startup";
            toolTip1.SetToolTip(chkScanOnStartup, "If checked the system will automatically scan and complete actions on startup");
            chkScanOnStartup.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreAllSpecials
            // 
            chkIgnoreAllSpecials.AutoSize = true;
            chkIgnoreAllSpecials.Location = new Point(34, 242);
            chkIgnoreAllSpecials.Margin = new Padding(4, 3, 4, 3);
            chkIgnoreAllSpecials.Name = "chkIgnoreAllSpecials";
            chkIgnoreAllSpecials.Size = new Size(192, 19);
            chkIgnoreAllSpecials.TabIndex = 49;
            chkIgnoreAllSpecials.Text = "Ignore Specials for all TV Shows";
            toolTip1.SetToolTip(chkIgnoreAllSpecials, "Ignores 'specials' season for all TV shows");
            chkIgnoreAllSpecials.UseVisualStyleBackColor = true;
            // 
            // cbAutoSaveOnExit
            // 
            cbAutoSaveOnExit.AutoSize = true;
            cbAutoSaveOnExit.Location = new Point(15, 173);
            cbAutoSaveOnExit.Margin = new Padding(4, 3, 4, 3);
            cbAutoSaveOnExit.Name = "cbAutoSaveOnExit";
            cbAutoSaveOnExit.Size = new Size(116, 19);
            cbAutoSaveOnExit.TabIndex = 44;
            cbAutoSaveOnExit.Text = "Auto save on Exit";
            toolTip1.SetToolTip(cbAutoSaveOnExit, "Should the system ask the user or always save when the application is shutdown?");
            cbAutoSaveOnExit.UseVisualStyleBackColor = true;
            // 
            // label84
            // 
            label84.AutoSize = true;
            label84.Location = new Point(12, 55);
            label84.Margin = new Padding(4, 0, 4, 0);
            label84.Name = "label84";
            label84.Size = new Size(95, 15);
            label84.TabIndex = 26;
            label84.Text = "&Preferred region:";
            toolTip1.SetToolTip(label84, "TMDB will return release dates and certifications based on your location");
            // 
            // lstMovieMonitorFolders
            // 
            lstMovieMonitorFolders.AllowDrop = true;
            lstMovieMonitorFolders.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstMovieMonitorFolders.FormattingEnabled = true;
            lstMovieMonitorFolders.IntegralHeight = false;
            lstMovieMonitorFolders.Location = new Point(7, 373);
            lstMovieMonitorFolders.Margin = new Padding(4, 3, 4, 3);
            lstMovieMonitorFolders.Name = "lstMovieMonitorFolders";
            lstMovieMonitorFolders.ScrollAlwaysVisible = true;
            lstMovieMonitorFolders.SelectionMode = SelectionMode.MultiExtended;
            lstMovieMonitorFolders.Size = new Size(464, 111);
            lstMovieMonitorFolders.TabIndex = 49;
            toolTip1.SetToolTip(lstMovieMonitorFolders, resources.GetString("lstMovieMonitorFolders.ToolTip"));
            lstMovieMonitorFolders.SelectedIndexChanged += lstMovieMonitorFolders_SelectedIndexChanged;
            lstMovieMonitorFolders.DragDrop += lstMovieMonitorFolders_DragDrop;
            lstMovieMonitorFolders.DragOver += FileIcon_DragOver;
            lstMovieMonitorFolders.KeyDown += lstMovieMonitorFolders_KeyDown;
            // 
            // chkIncludeMoviesQuickRecent
            // 
            chkIncludeMoviesQuickRecent.AutoSize = true;
            chkIncludeMoviesQuickRecent.Location = new Point(264, 22);
            chkIncludeMoviesQuickRecent.Margin = new Padding(4, 3, 4, 3);
            chkIncludeMoviesQuickRecent.Name = "chkIncludeMoviesQuickRecent";
            chkIncludeMoviesQuickRecent.Size = new Size(194, 19);
            chkIncludeMoviesQuickRecent.TabIndex = 50;
            chkIncludeMoviesQuickRecent.Text = "&Include Movies in Quick/Recent";
            toolTip1.SetToolTip(chkIncludeMoviesQuickRecent, "If checked the system will automatically scan and complete actions on startup");
            chkIncludeMoviesQuickRecent.UseVisualStyleBackColor = true;
            // 
            // tbCleanUpDownloadDirMoviesLength
            // 
            tbCleanUpDownloadDirMoviesLength.Location = new Point(252, 75);
            tbCleanUpDownloadDirMoviesLength.Margin = new Padding(4, 3, 4, 3);
            tbCleanUpDownloadDirMoviesLength.Name = "tbCleanUpDownloadDirMoviesLength";
            tbCleanUpDownloadDirMoviesLength.Size = new Size(63, 23);
            tbCleanUpDownloadDirMoviesLength.TabIndex = 14;
            toolTip1.SetToolTip(tbCleanUpDownloadDirMoviesLength, "Number of letters that the name of the movie must be. To prevent 'Up' ");
            // 
            // label98
            // 
            label98.AutoSize = true;
            label98.Location = new Point(414, 148);
            label98.Margin = new Padding(4, 0, 4, 0);
            label98.Name = "label98";
            label98.Size = new Size(50, 15);
            label98.TabIndex = 49;
            label98.Text = "seconds";
            toolTip1.SetToolTip(label98, "If checked the system will automatically scan and complete actions on a periodic schedule");
            // 
            // upDownScanSeconds
            // 
            upDownScanSeconds.Items.Add("120");
            upDownScanSeconds.Items.Add("60");
            upDownScanSeconds.Items.Add("30");
            upDownScanSeconds.Items.Add("15");
            upDownScanSeconds.Items.Add("10");
            upDownScanSeconds.Items.Add("5");
            upDownScanSeconds.Items.Add("4");
            upDownScanSeconds.Items.Add("3");
            upDownScanSeconds.Items.Add("2");
            upDownScanSeconds.Items.Add("1");
            upDownScanSeconds.Location = new Point(363, 145);
            upDownScanSeconds.Margin = new Padding(4, 3, 4, 3);
            upDownScanSeconds.Name = "upDownScanSeconds";
            upDownScanSeconds.Size = new Size(47, 23);
            upDownScanSeconds.TabIndex = 48;
            toolTip1.SetToolTip(upDownScanSeconds, "If checked the system will automatically scan and complete actions on a periodic schedule");
            // 
            // cmDefaults
            // 
            cmDefaults.Items.AddRange(new ToolStripItem[] { KODIToolStripMenuItem, pyTivoToolStripMenuItem, mede8erToolStripMenuItem, noneToolStripMenuItem });
            cmDefaults.Name = "cmDefaults";
            cmDefaults.Size = new Size(121, 92);
            cmDefaults.ItemClicked += cmDefaults_ItemClicked;
            // 
            // KODIToolStripMenuItem
            // 
            KODIToolStripMenuItem.Name = "KODIToolStripMenuItem";
            KODIToolStripMenuItem.Size = new Size(120, 22);
            KODIToolStripMenuItem.Tag = "1";
            KODIToolStripMenuItem.Text = "&KODI";
            // 
            // pyTivoToolStripMenuItem
            // 
            pyTivoToolStripMenuItem.Name = "pyTivoToolStripMenuItem";
            pyTivoToolStripMenuItem.Size = new Size(120, 22);
            pyTivoToolStripMenuItem.Tag = "2";
            pyTivoToolStripMenuItem.Text = "&pyTivo";
            // 
            // mede8erToolStripMenuItem
            // 
            mede8erToolStripMenuItem.Name = "mede8erToolStripMenuItem";
            mede8erToolStripMenuItem.Size = new Size(120, 22);
            mede8erToolStripMenuItem.Tag = "3";
            mede8erToolStripMenuItem.Text = "&Mede8er";
            // 
            // noneToolStripMenuItem
            // 
            noneToolStripMenuItem.Name = "noneToolStripMenuItem";
            noneToolStripMenuItem.Size = new Size(120, 22);
            noneToolStripMenuItem.Tag = "4";
            noneToolStripMenuItem.Text = "&None";
            // 
            // tpDisplay
            // 
            tpDisplay.Controls.Add(chkShowAccessibilityOptions);
            tpDisplay.Controls.Add(cbUseColoursOnWtw);
            tpDisplay.Controls.Add(chkBasicShowDetails);
            tpDisplay.Controls.Add(chkPostpendThe);
            tpDisplay.Controls.Add(groupBox11);
            tpDisplay.Controls.Add(label61);
            tpDisplay.Controls.Add(cbLeadingZero);
            tpDisplay.Controls.Add(txtSeasonFolderName);
            tpDisplay.Controls.Add(label35);
            tpDisplay.Controls.Add(chkHideWtWSpoilers);
            tpDisplay.Controls.Add(chkHideMyShowsSpoilers);
            tpDisplay.Controls.Add(rbWTWScan);
            tpDisplay.Controls.Add(rbWTWSearch);
            tpDisplay.Controls.Add(cbStartupTab);
            tpDisplay.Controls.Add(cbAutoSelInMyShows);
            tpDisplay.Controls.Add(cbShowEpisodePictures);
            tpDisplay.Controls.Add(label11);
            tpDisplay.Controls.Add(label6);
            tpDisplay.Controls.Add(chkShowInTaskbar);
            tpDisplay.Controls.Add(cbNotificationIcon);
            tpDisplay.Controls.Add(pbDisplay);
            tpDisplay.Location = new Point(149, 4);
            tpDisplay.Margin = new Padding(4, 3, 4, 3);
            tpDisplay.Name = "tpDisplay";
            tpDisplay.Padding = new Padding(4, 3, 4, 3);
            tpDisplay.Size = new Size(500, 684);
            tpDisplay.TabIndex = 13;
            tpDisplay.Text = "Display";
            tpDisplay.UseVisualStyleBackColor = true;
            // 
            // chkShowAccessibilityOptions
            // 
            chkShowAccessibilityOptions.AutoSize = true;
            chkShowAccessibilityOptions.Location = new Point(257, 332);
            chkShowAccessibilityOptions.Margin = new Padding(4, 3, 4, 3);
            chkShowAccessibilityOptions.Name = "chkShowAccessibilityOptions";
            chkShowAccessibilityOptions.Size = new Size(165, 19);
            chkShowAccessibilityOptions.TabIndex = 45;
            chkShowAccessibilityOptions.Text = "Show Accessibility Options";
            chkShowAccessibilityOptions.UseVisualStyleBackColor = true;
            // 
            // cbUseColoursOnWtw
            // 
            cbUseColoursOnWtw.AutoSize = true;
            cbUseColoursOnWtw.Location = new Point(257, 359);
            cbUseColoursOnWtw.Margin = new Padding(4, 3, 4, 3);
            cbUseColoursOnWtw.Name = "cbUseColoursOnWtw";
            cbUseColoursOnWtw.Size = new Size(157, 19);
            cbUseColoursOnWtw.TabIndex = 44;
            cbUseColoursOnWtw.Text = "Use Colours on Schedule";
            cbUseColoursOnWtw.UseVisualStyleBackColor = true;
            // 
            // chkBasicShowDetails
            // 
            chkBasicShowDetails.AutoSize = true;
            chkBasicShowDetails.Location = new Point(12, 359);
            chkBasicShowDetails.Margin = new Padding(4, 3, 4, 3);
            chkBasicShowDetails.Name = "chkBasicShowDetails";
            chkBasicShowDetails.Size = new Size(155, 19);
            chkBasicShowDetails.TabIndex = 43;
            chkBasicShowDetails.Text = "Show Basic Show Details";
            chkBasicShowDetails.UseVisualStyleBackColor = true;
            // 
            // chkPostpendThe
            // 
            chkPostpendThe.AutoSize = true;
            chkPostpendThe.Location = new Point(12, 249);
            chkPostpendThe.Margin = new Padding(4, 3, 4, 3);
            chkPostpendThe.Name = "chkPostpendThe";
            chkPostpendThe.Size = new Size(276, 19);
            chkPostpendThe.TabIndex = 42;
            chkPostpendThe.Text = "Move 'The' to the end of tv show/movie names";
            chkPostpendThe.UseVisualStyleBackColor = true;
            // 
            // groupBox11
            // 
            groupBox11.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox11.Controls.Add(label7);
            groupBox11.Controls.Add(cboShowStatus);
            groupBox11.Controls.Add(label5);
            groupBox11.Controls.Add(txtShowStatusColor);
            groupBox11.Controls.Add(btnSelectColor);
            groupBox11.Controls.Add(bnRemoveDefinedColor);
            groupBox11.Controls.Add(btnAddShowStatusColoring);
            groupBox11.Controls.Add(lvwDefinedColors);
            groupBox11.Location = new Point(7, 385);
            groupBox11.Margin = new Padding(4, 3, 4, 3);
            groupBox11.Name = "groupBox11";
            groupBox11.Padding = new Padding(4, 3, 4, 3);
            groupBox11.Size = new Size(457, 255);
            groupBox11.TabIndex = 41;
            groupBox11.TabStop = false;
            groupBox11.Text = "Show Colouring";
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label7.AutoSize = true;
            label7.Location = new Point(2, 195);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(42, 15);
            label7.TabIndex = 16;
            label7.Text = "&Status:";
            // 
            // cboShowStatus
            // 
            cboShowStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cboShowStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cboShowStatus.FormattingEnabled = true;
            cboShowStatus.Location = new Point(58, 192);
            cboShowStatus.Margin = new Padding(4, 3, 4, 3);
            cboShowStatus.Name = "cboShowStatus";
            cboShowStatus.Size = new Size(403, 23);
            cboShowStatus.TabIndex = 15;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label5.AutoSize = true;
            label5.Location = new Point(4, 231);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(63, 15);
            label5.TabIndex = 14;
            label5.Text = "&Text Color:";
            // 
            // txtShowStatusColor
            // 
            txtShowStatusColor.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtShowStatusColor.Location = new Point(78, 223);
            txtShowStatusColor.Margin = new Padding(4, 3, 4, 3);
            txtShowStatusColor.Name = "txtShowStatusColor";
            txtShowStatusColor.Size = new Size(116, 23);
            txtShowStatusColor.TabIndex = 13;
            txtShowStatusColor.TextChanged += txtShowStatusColor_TextChanged;
            // 
            // btnSelectColor
            // 
            btnSelectColor.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSelectColor.Location = new Point(202, 222);
            btnSelectColor.Margin = new Padding(4, 3, 4, 3);
            btnSelectColor.Name = "btnSelectColor";
            btnSelectColor.Size = new Size(88, 27);
            btnSelectColor.TabIndex = 12;
            btnSelectColor.Text = "Select &Color";
            btnSelectColor.UseVisualStyleBackColor = true;
            btnSelectColor.Click += btnSelectColor_Click;
            // 
            // bnRemoveDefinedColor
            // 
            bnRemoveDefinedColor.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnRemoveDefinedColor.Enabled = false;
            bnRemoveDefinedColor.Location = new Point(4, 158);
            bnRemoveDefinedColor.Margin = new Padding(4, 3, 4, 3);
            bnRemoveDefinedColor.Name = "bnRemoveDefinedColor";
            bnRemoveDefinedColor.Size = new Size(88, 27);
            bnRemoveDefinedColor.TabIndex = 10;
            bnRemoveDefinedColor.Text = "&Remove";
            bnRemoveDefinedColor.UseVisualStyleBackColor = true;
            bnRemoveDefinedColor.Click += bnRemoveDefinedColor_Click;
            // 
            // btnAddShowStatusColoring
            // 
            btnAddShowStatusColoring.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnAddShowStatusColoring.Location = new Point(363, 222);
            btnAddShowStatusColoring.Margin = new Padding(4, 3, 4, 3);
            btnAddShowStatusColoring.Name = "btnAddShowStatusColoring";
            btnAddShowStatusColoring.Size = new Size(88, 27);
            btnAddShowStatusColoring.TabIndex = 11;
            btnAddShowStatusColoring.Text = "&Add";
            btnAddShowStatusColoring.UseVisualStyleBackColor = true;
            btnAddShowStatusColoring.Click += btnAddShowStatusColoring_Click;
            // 
            // lvwDefinedColors
            // 
            lvwDefinedColors.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvwDefinedColors.Columns.AddRange(new ColumnHeader[] { colShowStatus, colColor });
            lvwDefinedColors.GridLines = true;
            lvwDefinedColors.Location = new Point(7, 22);
            lvwDefinedColors.Margin = new Padding(4, 3, 4, 3);
            lvwDefinedColors.MultiSelect = false;
            lvwDefinedColors.Name = "lvwDefinedColors";
            lvwDefinedColors.Size = new Size(443, 129);
            lvwDefinedColors.TabIndex = 9;
            lvwDefinedColors.UseCompatibleStateImageBehavior = false;
            lvwDefinedColors.View = View.Details;
            lvwDefinedColors.SelectedIndexChanged += EnableDisable;
            lvwDefinedColors.DoubleClick += lvwDefinedColors_DoubleClick;
            // 
            // colShowStatus
            // 
            colShowStatus.Text = "Show Status";
            colShowStatus.Width = 297;
            // 
            // colColor
            // 
            colColor.Text = "Color";
            colColor.Width = 92;
            // 
            // label61
            // 
            label61.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label61.AutoSize = true;
            label61.Location = new Point(8, 8);
            label61.Margin = new Padding(4, 0, 4, 0);
            label61.Name = "label61";
            label61.Size = new Size(326, 45);
            label61.TabIndex = 40;
            label61.Text = "Settings that control the way that TV Rename looks. These do\r\nnot have any impact on the main scanning, just on the way \r\nthe interface looks.";
            // 
            // cbLeadingZero
            // 
            cbLeadingZero.AutoSize = true;
            cbLeadingZero.Location = new Point(12, 332);
            cbLeadingZero.Margin = new Padding(4, 3, 4, 3);
            cbLeadingZero.Name = "cbLeadingZero";
            cbLeadingZero.Size = new Size(184, 19);
            cbLeadingZero.TabIndex = 38;
            cbLeadingZero.Text = "&Leading 0 on Season numbers";
            cbLeadingZero.UseVisualStyleBackColor = true;
            // 
            // txtSeasonFolderName
            // 
            txtSeasonFolderName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSeasonFolderName.Location = new Point(133, 302);
            txtSeasonFolderName.Margin = new Padding(4, 3, 4, 3);
            txtSeasonFolderName.Name = "txtSeasonFolderName";
            txtSeasonFolderName.Size = new Size(331, 23);
            txtSeasonFolderName.TabIndex = 37;
            // 
            // label35
            // 
            label35.AutoSize = true;
            label35.Location = new Point(8, 306);
            label35.Margin = new Padding(4, 0, 4, 0);
            label35.Name = "label35";
            label35.Size = new Size(85, 15);
            label35.TabIndex = 36;
            label35.Text = "&Seasons name:";
            // 
            // chkHideWtWSpoilers
            // 
            chkHideWtWSpoilers.AutoSize = true;
            chkHideWtWSpoilers.Location = new Point(257, 223);
            chkHideWtWSpoilers.Margin = new Padding(4, 3, 4, 3);
            chkHideWtWSpoilers.Name = "chkHideWtWSpoilers";
            chkHideWtWSpoilers.Size = new Size(159, 19);
            chkHideWtWSpoilers.TabIndex = 35;
            chkHideWtWSpoilers.Text = "Hide Spoilers in Schedule";
            chkHideWtWSpoilers.UseVisualStyleBackColor = true;
            // 
            // chkHideMyShowsSpoilers
            // 
            chkHideMyShowsSpoilers.AutoSize = true;
            chkHideMyShowsSpoilers.Location = new Point(12, 223);
            chkHideMyShowsSpoilers.Margin = new Padding(4, 3, 4, 3);
            chkHideMyShowsSpoilers.Name = "chkHideMyShowsSpoilers";
            chkHideMyShowsSpoilers.Size = new Size(168, 19);
            chkHideMyShowsSpoilers.TabIndex = 34;
            chkHideMyShowsSpoilers.Text = "Hide Spoilers in 'TV Shows'";
            chkHideMyShowsSpoilers.UseVisualStyleBackColor = true;
            // 
            // rbWTWScan
            // 
            rbWTWScan.AutoSize = true;
            rbWTWScan.Location = new Point(33, 113);
            rbWTWScan.Margin = new Padding(4, 3, 4, 3);
            rbWTWScan.Name = "rbWTWScan";
            rbWTWScan.Size = new Size(50, 19);
            rbWTWScan.TabIndex = 27;
            rbWTWScan.Text = "S&can";
            rbWTWScan.UseVisualStyleBackColor = true;
            // 
            // rbWTWSearch
            // 
            rbWTWSearch.AutoSize = true;
            rbWTWSearch.Checked = true;
            rbWTWSearch.Location = new Point(33, 91);
            rbWTWSearch.Margin = new Padding(4, 3, 4, 3);
            rbWTWSearch.Name = "rbWTWSearch";
            rbWTWSearch.Size = new Size(60, 19);
            rbWTWSearch.TabIndex = 26;
            rbWTWSearch.TabStop = true;
            rbWTWSearch.Text = "S&earch";
            rbWTWSearch.UseVisualStyleBackColor = true;
            // 
            // cbStartupTab
            // 
            cbStartupTab.DropDownStyle = ComboBoxStyle.DropDownList;
            cbStartupTab.FormattingEnabled = true;
            cbStartupTab.Items.AddRange(new object[] { "Movies", "TV Shows", "Scan", "Schedule" });
            cbStartupTab.Location = new Point(88, 138);
            cbStartupTab.Margin = new Padding(4, 3, 4, 3);
            cbStartupTab.Name = "cbStartupTab";
            cbStartupTab.Size = new Size(157, 23);
            cbStartupTab.TabIndex = 29;
            // 
            // cbAutoSelInMyShows
            // 
            cbAutoSelInMyShows.AutoSize = true;
            cbAutoSelInMyShows.Location = new Point(12, 276);
            cbAutoSelInMyShows.Margin = new Padding(4, 3, 4, 3);
            cbAutoSelInMyShows.Name = "cbAutoSelInMyShows";
            cbAutoSelInMyShows.Size = new Size(299, 19);
            cbAutoSelInMyShows.TabIndex = 33;
            cbAutoSelInMyShows.Text = "&Automatically select show and season in 'TV Shows'";
            cbAutoSelInMyShows.UseVisualStyleBackColor = true;
            // 
            // cbShowEpisodePictures
            // 
            cbShowEpisodePictures.AutoSize = true;
            cbShowEpisodePictures.Location = new Point(12, 196);
            cbShowEpisodePictures.Margin = new Padding(4, 3, 4, 3);
            cbShowEpisodePictures.Name = "cbShowEpisodePictures";
            cbShowEpisodePictures.Size = new Size(239, 19);
            cbShowEpisodePictures.TabIndex = 32;
            cbShowEpisodePictures.Text = "S&how episode pictures in episode guides";
            cbShowEpisodePictures.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(8, 74);
            label11.Margin = new Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new Size(169, 15);
            label11.TabIndex = 25;
            label11.Text = "Double-click in Schedule does:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(8, 142);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(68, 15);
            label6.TabIndex = 28;
            label6.Text = "&Startup tab:";
            // 
            // chkShowInTaskbar
            // 
            chkShowInTaskbar.AutoSize = true;
            chkShowInTaskbar.Location = new Point(257, 170);
            chkShowInTaskbar.Margin = new Padding(4, 3, 4, 3);
            chkShowInTaskbar.Name = "chkShowInTaskbar";
            chkShowInTaskbar.Size = new Size(109, 19);
            chkShowInTaskbar.TabIndex = 31;
            chkShowInTaskbar.Text = "Show in &taskbar";
            chkShowInTaskbar.UseVisualStyleBackColor = true;
            chkShowInTaskbar.CheckedChanged += EnableDisable;
            // 
            // cbNotificationIcon
            // 
            cbNotificationIcon.AutoSize = true;
            cbNotificationIcon.Location = new Point(12, 170);
            cbNotificationIcon.Margin = new Padding(4, 3, 4, 3);
            cbNotificationIcon.Name = "cbNotificationIcon";
            cbNotificationIcon.Size = new Size(170, 19);
            cbNotificationIcon.TabIndex = 30;
            cbNotificationIcon.Text = "Show &notification area icon";
            cbNotificationIcon.UseVisualStyleBackColor = true;
            cbNotificationIcon.CheckedChanged += EnableDisable;
            // 
            // pbDisplay
            // 
            pbDisplay.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbDisplay.Cursor = Cursors.Hand;
            pbDisplay.Image = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbDisplay.InitialImage = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbDisplay.Location = new Point(418, 7);
            pbDisplay.Margin = new Padding(4, 3, 4, 3);
            pbDisplay.Name = "pbDisplay";
            pbDisplay.Size = new Size(50, 46);
            pbDisplay.SizeMode = PictureBoxSizeMode.CenterImage;
            pbDisplay.TabIndex = 39;
            pbDisplay.TabStop = false;
            pbDisplay.Click += pbDisplay_Click;
            // 
            // tpRSSJSONSearch
            // 
            tpRSSJSONSearch.Controls.Add(pbRSSJSONSearch);
            tpRSSJSONSearch.Controls.Add(label59);
            tpRSSJSONSearch.Controls.Add(cbSearchJSON);
            tpRSSJSONSearch.Controls.Add(cbSearchRSS);
            tpRSSJSONSearch.Controls.Add(gbJSON);
            tpRSSJSONSearch.Controls.Add(gbRSS);
            tpRSSJSONSearch.Location = new Point(149, 4);
            tpRSSJSONSearch.Margin = new Padding(4, 3, 4, 3);
            tpRSSJSONSearch.Name = "tpRSSJSONSearch";
            tpRSSJSONSearch.Padding = new Padding(4, 3, 4, 3);
            tpRSSJSONSearch.Size = new Size(500, 684);
            tpRSSJSONSearch.TabIndex = 12;
            tpRSSJSONSearch.Text = "RSS/JSON Search";
            tpRSSJSONSearch.UseVisualStyleBackColor = true;
            // 
            // pbRSSJSONSearch
            // 
            pbRSSJSONSearch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbRSSJSONSearch.Cursor = Cursors.Hand;
            pbRSSJSONSearch.Image = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbRSSJSONSearch.InitialImage = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbRSSJSONSearch.Location = new Point(418, 7);
            pbRSSJSONSearch.Margin = new Padding(4, 3, 4, 3);
            pbRSSJSONSearch.Name = "pbRSSJSONSearch";
            pbRSSJSONSearch.Size = new Size(50, 46);
            pbRSSJSONSearch.SizeMode = PictureBoxSizeMode.CenterImage;
            pbRSSJSONSearch.TabIndex = 38;
            pbRSSJSONSearch.TabStop = false;
            pbRSSJSONSearch.Click += pbRSSJSONSearch_Click;
            // 
            // label59
            // 
            label59.AutoSize = true;
            label59.Location = new Point(4, 15);
            label59.Margin = new Padding(4, 0, 4, 0);
            label59.Name = "label59";
            label59.Size = new Size(379, 45);
            label59.TabIndex = 37;
            label59.Text = "If an episode is missing from your library, TV Rename will look in the \r\nfollowing URLs for appropriate files to download. It will use the torrent \r\nhandlers to download the file(s)";
            // 
            // cbSearchJSON
            // 
            cbSearchJSON.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cbSearchJSON.AutoSize = true;
            cbSearchJSON.Location = new Point(9, 370);
            cbSearchJSON.Margin = new Padding(4, 3, 4, 3);
            cbSearchJSON.Name = "cbSearchJSON";
            cbSearchJSON.Size = new Size(178, 19);
            cbSearchJSON.TabIndex = 36;
            cbSearchJSON.Text = "Search &JSON for missing files";
            cbSearchJSON.UseVisualStyleBackColor = true;
            cbSearchJSON.CheckedChanged += EnableDisable;
            // 
            // cbSearchRSS
            // 
            cbSearchRSS.AutoSize = true;
            cbSearchRSS.Location = new Point(7, 76);
            cbSearchRSS.Margin = new Padding(4, 3, 4, 3);
            cbSearchRSS.Name = "cbSearchRSS";
            cbSearchRSS.Size = new Size(169, 19);
            cbSearchRSS.TabIndex = 35;
            cbSearchRSS.Text = "&Search RSS for missing files";
            cbSearchRSS.UseVisualStyleBackColor = true;
            cbSearchRSS.CheckedChanged += EnableDisable;
            // 
            // gbJSON
            // 
            gbJSON.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gbJSON.Controls.Add(label78);
            gbJSON.Controls.Add(tbJSONSeedersToken);
            gbJSON.Controls.Add(cbJSONCloudflareProtection);
            gbJSON.Controls.Add(cbSearchJSONManualScanOnly);
            gbJSON.Controls.Add(label55);
            gbJSON.Controls.Add(tbJSONFilesizeToken);
            gbJSON.Controls.Add(label51);
            gbJSON.Controls.Add(tbJSONFilenameToken);
            gbJSON.Controls.Add(label50);
            gbJSON.Controls.Add(tbJSONURLToken);
            gbJSON.Controls.Add(label49);
            gbJSON.Controls.Add(tbJSONRootNode);
            gbJSON.Controls.Add(label48);
            gbJSON.Controls.Add(tbJSONURL);
            gbJSON.Location = new Point(7, 396);
            gbJSON.Margin = new Padding(4, 3, 4, 3);
            gbJSON.Name = "gbJSON";
            gbJSON.Padding = new Padding(4, 3, 4, 3);
            gbJSON.Size = new Size(464, 230);
            gbJSON.TabIndex = 33;
            gbJSON.TabStop = false;
            gbJSON.Text = "JSON Search";
            // 
            // label78
            // 
            label78.AutoSize = true;
            label78.Location = new Point(8, 202);
            label78.Margin = new Padding(4, 0, 4, 0);
            label78.Name = "label78";
            label78.Size = new Size(85, 15);
            label78.TabIndex = 42;
            label78.Text = "Seeders Token:";
            // 
            // tbJSONSeedersToken
            // 
            tbJSONSeedersToken.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbJSONSeedersToken.Location = new Point(114, 198);
            tbJSONSeedersToken.Margin = new Padding(4, 3, 4, 3);
            tbJSONSeedersToken.Name = "tbJSONSeedersToken";
            tbJSONSeedersToken.Size = new Size(344, 23);
            tbJSONSeedersToken.TabIndex = 41;
            // 
            // cbJSONCloudflareProtection
            // 
            cbJSONCloudflareProtection.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbJSONCloudflareProtection.AutoSize = true;
            cbJSONCloudflareProtection.Location = new Point(292, 22);
            cbJSONCloudflareProtection.Margin = new Padding(4, 3, 4, 3);
            cbJSONCloudflareProtection.Name = "cbJSONCloudflareProtection";
            cbJSONCloudflareProtection.Size = new Size(161, 19);
            cbJSONCloudflareProtection.TabIndex = 40;
            cbJSONCloudflareProtection.Text = "Use Cloudflare protection";
            cbJSONCloudflareProtection.UseVisualStyleBackColor = true;
            // 
            // cbSearchJSONManualScanOnly
            // 
            cbSearchJSONManualScanOnly.AutoSize = true;
            cbSearchJSONManualScanOnly.Location = new Point(10, 22);
            cbSearchJSONManualScanOnly.Margin = new Padding(4, 3, 4, 3);
            cbSearchJSONManualScanOnly.Name = "cbSearchJSONManualScanOnly";
            cbSearchJSONManualScanOnly.Size = new Size(143, 19);
            cbSearchJSONManualScanOnly.TabIndex = 38;
            cbSearchJSONManualScanOnly.Text = "Only on manual scans";
            cbSearchJSONManualScanOnly.UseVisualStyleBackColor = true;
            // 
            // label55
            // 
            label55.AutoSize = true;
            label55.Location = new Point(7, 172);
            label55.Margin = new Padding(4, 0, 4, 0);
            label55.Name = "label55";
            label55.Size = new Size(65, 15);
            label55.TabIndex = 39;
            label55.Text = "Size Token:";
            // 
            // tbJSONFilesizeToken
            // 
            tbJSONFilesizeToken.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbJSONFilesizeToken.Location = new Point(113, 168);
            tbJSONFilesizeToken.Margin = new Padding(4, 3, 4, 3);
            tbJSONFilesizeToken.Name = "tbJSONFilesizeToken";
            tbJSONFilesizeToken.Size = new Size(344, 23);
            tbJSONFilesizeToken.TabIndex = 38;
            // 
            // label51
            // 
            label51.AutoSize = true;
            label51.Location = new Point(7, 112);
            label51.Margin = new Padding(4, 0, 4, 0);
            label51.Name = "label51";
            label51.Size = new Size(93, 15);
            label51.TabIndex = 37;
            label51.Text = "Filename Token:";
            // 
            // tbJSONFilenameToken
            // 
            tbJSONFilenameToken.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbJSONFilenameToken.Location = new Point(114, 108);
            tbJSONFilenameToken.Margin = new Padding(4, 3, 4, 3);
            tbJSONFilenameToken.Name = "tbJSONFilenameToken";
            tbJSONFilenameToken.Size = new Size(344, 23);
            tbJSONFilenameToken.TabIndex = 36;
            // 
            // label50
            // 
            label50.AutoSize = true;
            label50.Location = new Point(7, 142);
            label50.Margin = new Padding(4, 0, 4, 0);
            label50.Name = "label50";
            label50.Size = new Size(66, 15);
            label50.TabIndex = 35;
            label50.Text = "URL Token:";
            // 
            // tbJSONURLToken
            // 
            tbJSONURLToken.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbJSONURLToken.Location = new Point(114, 138);
            tbJSONURLToken.Margin = new Padding(4, 3, 4, 3);
            tbJSONURLToken.Name = "tbJSONURLToken";
            tbJSONURLToken.Size = new Size(344, 23);
            tbJSONURLToken.TabIndex = 34;
            // 
            // label49
            // 
            label49.AutoSize = true;
            label49.Location = new Point(7, 82);
            label49.Margin = new Padding(4, 0, 4, 0);
            label49.Name = "label49";
            label49.Size = new Size(67, 15);
            label49.TabIndex = 33;
            label49.Text = "Root Node:";
            // 
            // tbJSONRootNode
            // 
            tbJSONRootNode.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbJSONRootNode.Location = new Point(114, 78);
            tbJSONRootNode.Margin = new Padding(4, 3, 4, 3);
            tbJSONRootNode.Name = "tbJSONRootNode";
            tbJSONRootNode.Size = new Size(344, 23);
            tbJSONRootNode.TabIndex = 32;
            // 
            // label48
            // 
            label48.AutoSize = true;
            label48.Location = new Point(7, 52);
            label48.Margin = new Padding(4, 0, 4, 0);
            label48.Name = "label48";
            label48.Size = new Size(31, 15);
            label48.TabIndex = 31;
            label48.Text = "URL:";
            // 
            // tbJSONURL
            // 
            tbJSONURL.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbJSONURL.Location = new Point(113, 48);
            tbJSONURL.Margin = new Padding(4, 3, 4, 3);
            tbJSONURL.Name = "tbJSONURL";
            tbJSONURL.Size = new Size(344, 23);
            tbJSONURL.TabIndex = 30;
            // 
            // gbRSS
            // 
            gbRSS.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gbRSS.Controls.Add(cbRSSCloudflareProtection);
            gbRSS.Controls.Add(cbSearchRSSManualScanOnly);
            gbRSS.Controls.Add(RSSGrid);
            gbRSS.Controls.Add(label25);
            gbRSS.Controls.Add(bnRSSRemove);
            gbRSS.Controls.Add(bnRSSGo);
            gbRSS.Controls.Add(bnRSSAdd);
            gbRSS.Location = new Point(4, 99);
            gbRSS.Margin = new Padding(4, 3, 4, 3);
            gbRSS.Name = "gbRSS";
            gbRSS.Padding = new Padding(4, 3, 4, 3);
            gbRSS.Size = new Size(463, 263);
            gbRSS.TabIndex = 32;
            gbRSS.TabStop = false;
            gbRSS.Text = "RSS Search";
            // 
            // cbRSSCloudflareProtection
            // 
            cbRSSCloudflareProtection.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbRSSCloudflareProtection.AutoSize = true;
            cbRSSCloudflareProtection.Location = new Point(295, 22);
            cbRSSCloudflareProtection.Margin = new Padding(4, 3, 4, 3);
            cbRSSCloudflareProtection.Name = "cbRSSCloudflareProtection";
            cbRSSCloudflareProtection.Size = new Size(161, 19);
            cbRSSCloudflareProtection.TabIndex = 41;
            cbRSSCloudflareProtection.Text = "Use Cloudflare protection";
            cbRSSCloudflareProtection.UseVisualStyleBackColor = true;
            // 
            // cbSearchRSSManualScanOnly
            // 
            cbSearchRSSManualScanOnly.AutoSize = true;
            cbSearchRSSManualScanOnly.Location = new Point(7, 22);
            cbSearchRSSManualScanOnly.Margin = new Padding(4, 3, 4, 3);
            cbSearchRSSManualScanOnly.Name = "cbSearchRSSManualScanOnly";
            cbSearchRSSManualScanOnly.Size = new Size(143, 19);
            cbSearchRSSManualScanOnly.TabIndex = 37;
            cbSearchRSSManualScanOnly.Text = "Only on manual scans";
            cbSearchRSSManualScanOnly.UseVisualStyleBackColor = true;
            // 
            // RSSGrid
            // 
            RSSGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            RSSGrid.BackColor = SystemColors.Window;
            RSSGrid.EnableSort = true;
            RSSGrid.Location = new Point(7, 63);
            RSSGrid.Margin = new Padding(4, 3, 4, 3);
            RSSGrid.Name = "RSSGrid";
            RSSGrid.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            RSSGrid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            RSSGrid.Size = new Size(449, 155);
            RSSGrid.TabIndex = 26;
            RSSGrid.TabStop = true;
            RSSGrid.ToolTipText = "";
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Location = new Point(4, 45);
            label25.Margin = new Padding(4, 0, 4, 0);
            label25.Name = "label25";
            label25.Size = new Size(99, 15);
            label25.TabIndex = 25;
            label25.Text = "Torrent RSS URLs:";
            // 
            // bnRSSRemove
            // 
            bnRSSRemove.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnRSSRemove.Location = new Point(102, 228);
            bnRSSRemove.Margin = new Padding(4, 3, 4, 3);
            bnRSSRemove.Name = "bnRSSRemove";
            bnRSSRemove.Size = new Size(88, 27);
            bnRSSRemove.TabIndex = 28;
            bnRSSRemove.Text = "&Remove";
            bnRSSRemove.UseVisualStyleBackColor = true;
            bnRSSRemove.Click += bnRSSRemove_Click;
            // 
            // bnRSSGo
            // 
            bnRSSGo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnRSSGo.Location = new Point(196, 228);
            bnRSSGo.Margin = new Padding(4, 3, 4, 3);
            bnRSSGo.Name = "bnRSSGo";
            bnRSSGo.Size = new Size(88, 27);
            bnRSSGo.TabIndex = 29;
            bnRSSGo.Text = "&Open";
            bnRSSGo.UseVisualStyleBackColor = true;
            bnRSSGo.Click += bnRSSGo_Click;
            // 
            // bnRSSAdd
            // 
            bnRSSAdd.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnRSSAdd.Location = new Point(7, 228);
            bnRSSAdd.Margin = new Padding(4, 3, 4, 3);
            bnRSSAdd.Name = "bnRSSAdd";
            bnRSSAdd.Size = new Size(88, 27);
            bnRSSAdd.TabIndex = 27;
            bnRSSAdd.Text = "&Add";
            bnRSSAdd.UseVisualStyleBackColor = true;
            bnRSSAdd.Click += bnRSSAdd_Click;
            // 
            // tpLibraryFolders
            // 
            tpLibraryFolders.Controls.Add(groupBox23);
            tpLibraryFolders.Controls.Add(label87);
            tpLibraryFolders.Controls.Add(bnOpenMovieMonFolder);
            tpLibraryFolders.Controls.Add(bnAddMovieMonFolder);
            tpLibraryFolders.Controls.Add(bnRemoveMovieMonFolder);
            tpLibraryFolders.Controls.Add(lstMovieMonitorFolders);
            tpLibraryFolders.Controls.Add(groupBox6);
            tpLibraryFolders.Controls.Add(label65);
            tpLibraryFolders.Controls.Add(label56);
            tpLibraryFolders.Controls.Add(bnOpenMonFolder);
            tpLibraryFolders.Controls.Add(bnAddMonFolder);
            tpLibraryFolders.Controls.Add(bnRemoveMonFolder);
            tpLibraryFolders.Controls.Add(pbLibraryFolders);
            tpLibraryFolders.Controls.Add(lstFMMonitorFolders);
            tpLibraryFolders.Location = new Point(149, 4);
            tpLibraryFolders.Margin = new Padding(4, 3, 4, 3);
            tpLibraryFolders.Name = "tpLibraryFolders";
            tpLibraryFolders.Padding = new Padding(4, 3, 4, 3);
            tpLibraryFolders.Size = new Size(500, 684);
            tpLibraryFolders.TabIndex = 10;
            tpLibraryFolders.Text = "Library Folders";
            tpLibraryFolders.UseVisualStyleBackColor = true;
            // 
            // groupBox23
            // 
            groupBox23.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox23.Controls.Add(button3);
            groupBox23.Controls.Add(txtMovieFilenameFormat);
            groupBox23.Controls.Add(label90);
            groupBox23.Controls.Add(button2);
            groupBox23.Controls.Add(txtMovieFolderFormat);
            groupBox23.Controls.Add(label85);
            groupBox23.Location = new Point(10, 525);
            groupBox23.Margin = new Padding(4, 3, 4, 3);
            groupBox23.Name = "groupBox23";
            groupBox23.Padding = new Padding(4, 3, 4, 3);
            groupBox23.Size = new Size(456, 99);
            groupBox23.TabIndex = 54;
            groupBox23.TabStop = false;
            groupBox23.Text = "Default Movie Library Folder Format";
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button3.Location = new Point(377, 59);
            button3.Margin = new Padding(4, 3, 4, 3);
            button3.Name = "button3";
            button3.Size = new Size(72, 27);
            button3.TabIndex = 34;
            button3.Text = "Tags...";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // txtMovieFilenameFormat
            // 
            txtMovieFilenameFormat.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtMovieFilenameFormat.Location = new Point(145, 60);
            txtMovieFilenameFormat.Margin = new Padding(4, 3, 4, 3);
            txtMovieFilenameFormat.Name = "txtMovieFilenameFormat";
            txtMovieFilenameFormat.Size = new Size(224, 23);
            txtMovieFilenameFormat.TabIndex = 33;
            // 
            // label90
            // 
            label90.AutoSize = true;
            label90.Location = new Point(7, 65);
            label90.Margin = new Padding(4, 0, 4, 0);
            label90.Name = "label90";
            label90.Size = new Size(97, 15);
            label90.TabIndex = 32;
            label90.Text = "Filename format:";
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.Location = new Point(377, 25);
            button2.Margin = new Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new Size(72, 27);
            button2.TabIndex = 31;
            button2.Text = "Tags...";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // txtMovieFolderFormat
            // 
            txtMovieFolderFormat.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtMovieFolderFormat.Location = new Point(145, 27);
            txtMovieFolderFormat.Margin = new Padding(4, 3, 4, 3);
            txtMovieFolderFormat.Name = "txtMovieFolderFormat";
            txtMovieFolderFormat.Size = new Size(224, 23);
            txtMovieFolderFormat.TabIndex = 30;
            // 
            // label85
            // 
            label85.AutoSize = true;
            label85.Location = new Point(7, 31);
            label85.Margin = new Padding(4, 0, 4, 0);
            label85.Name = "label85";
            label85.Size = new Size(82, 15);
            label85.TabIndex = 29;
            label85.Text = "&Folder format:";
            // 
            // label87
            // 
            label87.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label87.AutoSize = true;
            label87.Location = new Point(7, 354);
            label87.Margin = new Padding(4, 0, 4, 0);
            label87.Name = "label87";
            label87.Size = new Size(120, 15);
            label87.TabIndex = 53;
            label87.Text = "Movie &Library Folders";
            // 
            // bnOpenMovieMonFolder
            // 
            bnOpenMovieMonFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnOpenMovieMonFolder.Enabled = false;
            bnOpenMovieMonFolder.Location = new Point(196, 492);
            bnOpenMovieMonFolder.Margin = new Padding(4, 3, 4, 3);
            bnOpenMovieMonFolder.Name = "bnOpenMovieMonFolder";
            bnOpenMovieMonFolder.Size = new Size(88, 27);
            bnOpenMovieMonFolder.TabIndex = 52;
            bnOpenMovieMonFolder.Text = "&Open";
            bnOpenMovieMonFolder.UseVisualStyleBackColor = true;
            bnOpenMovieMonFolder.Click += bnOpenMovieMonFolder_Click;
            // 
            // bnAddMovieMonFolder
            // 
            bnAddMovieMonFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnAddMovieMonFolder.Location = new Point(7, 492);
            bnAddMovieMonFolder.Margin = new Padding(4, 3, 4, 3);
            bnAddMovieMonFolder.Name = "bnAddMovieMonFolder";
            bnAddMovieMonFolder.Size = new Size(88, 27);
            bnAddMovieMonFolder.TabIndex = 50;
            bnAddMovieMonFolder.Text = "&Add";
            bnAddMovieMonFolder.UseVisualStyleBackColor = true;
            bnAddMovieMonFolder.Click += bnAddMovieMonFolder_Click;
            // 
            // bnRemoveMovieMonFolder
            // 
            bnRemoveMovieMonFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnRemoveMovieMonFolder.Enabled = false;
            bnRemoveMovieMonFolder.Location = new Point(102, 492);
            bnRemoveMovieMonFolder.Margin = new Padding(4, 3, 4, 3);
            bnRemoveMovieMonFolder.Name = "bnRemoveMovieMonFolder";
            bnRemoveMovieMonFolder.Size = new Size(88, 27);
            bnRemoveMovieMonFolder.TabIndex = 51;
            bnRemoveMovieMonFolder.Text = "&Remove";
            bnRemoveMovieMonFolder.UseVisualStyleBackColor = true;
            bnRemoveMovieMonFolder.Click += bnRemoveMovieMonFolder_Click;
            // 
            // groupBox6
            // 
            groupBox6.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox6.Controls.Add(button4);
            groupBox6.Controls.Add(txtShowFolderFormat);
            groupBox6.Controls.Add(label96);
            groupBox6.Controls.Add(button1);
            groupBox6.Controls.Add(txtSeasonFormat);
            groupBox6.Controls.Add(txtSpecialsFolderName);
            groupBox6.Controls.Add(label47);
            groupBox6.Controls.Add(label13);
            groupBox6.Location = new Point(10, 232);
            groupBox6.Margin = new Padding(4, 3, 4, 3);
            groupBox6.Name = "groupBox6";
            groupBox6.Padding = new Padding(4, 3, 4, 3);
            groupBox6.Size = new Size(456, 115);
            groupBox6.TabIndex = 48;
            groupBox6.TabStop = false;
            groupBox6.Text = "Default TV Show Library Folder Format";
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button4.Location = new Point(377, 83);
            button4.Margin = new Padding(4, 3, 4, 3);
            button4.Name = "button4";
            button4.Size = new Size(72, 27);
            button4.TabIndex = 34;
            button4.Text = "Tags...";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // txtShowFolderFormat
            // 
            txtShowFolderFormat.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtShowFolderFormat.Location = new Point(145, 84);
            txtShowFolderFormat.Margin = new Padding(4, 3, 4, 3);
            txtShowFolderFormat.Name = "txtShowFolderFormat";
            txtShowFolderFormat.Size = new Size(224, 23);
            txtShowFolderFormat.TabIndex = 33;
            // 
            // label96
            // 
            label96.AutoSize = true;
            label96.Location = new Point(7, 89);
            label96.Margin = new Padding(4, 0, 4, 0);
            label96.Name = "label96";
            label96.Size = new Size(112, 15);
            label96.TabIndex = 32;
            label96.Text = "Show folder format:";
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.Location = new Point(377, 53);
            button1.Margin = new Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new Size(72, 27);
            button1.TabIndex = 31;
            button1.Text = "Tags...";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // txtSeasonFormat
            // 
            txtSeasonFormat.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSeasonFormat.Location = new Point(145, 54);
            txtSeasonFormat.Margin = new Padding(4, 3, 4, 3);
            txtSeasonFormat.Name = "txtSeasonFormat";
            txtSeasonFormat.Size = new Size(224, 23);
            txtSeasonFormat.TabIndex = 30;
            // 
            // txtSpecialsFolderName
            // 
            txtSpecialsFolderName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSpecialsFolderName.Location = new Point(145, 24);
            txtSpecialsFolderName.Margin = new Padding(4, 3, 4, 3);
            txtSpecialsFolderName.Name = "txtSpecialsFolderName";
            txtSpecialsFolderName.Size = new Size(304, 23);
            txtSpecialsFolderName.TabIndex = 28;
            // 
            // label47
            // 
            label47.AutoSize = true;
            label47.Location = new Point(7, 59);
            label47.Margin = new Padding(4, 0, 4, 0);
            label47.Name = "label47";
            label47.Size = new Size(125, 15);
            label47.TabIndex = 29;
            label47.Text = "&Seasons folder format:";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(7, 28);
            label13.Margin = new Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new Size(119, 15);
            label13.TabIndex = 27;
            label13.Text = "&Specials folder name:";
            // 
            // label65
            // 
            label65.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label65.AutoSize = true;
            label65.Location = new Point(7, 7);
            label65.Margin = new Padding(4, 0, 4, 0);
            label65.Name = "label65";
            label65.Size = new Size(327, 30);
            label65.TabIndex = 44;
            label65.Text = "TV Rename considers 2 sets of folders. Library Folders are the\r\nbase folders for a sorted collection of files";
            // 
            // label56
            // 
            label56.AutoSize = true;
            label56.Location = new Point(4, 61);
            label56.Margin = new Padding(4, 0, 4, 0);
            label56.Name = "label56";
            label56.Size = new Size(101, 15);
            label56.TabIndex = 36;
            label56.Text = "TV &Library Folders";
            // 
            // bnOpenMonFolder
            // 
            bnOpenMonFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnOpenMonFolder.Enabled = false;
            bnOpenMonFolder.Location = new Point(196, 198);
            bnOpenMonFolder.Margin = new Padding(4, 3, 4, 3);
            bnOpenMonFolder.Name = "bnOpenMonFolder";
            bnOpenMonFolder.Size = new Size(88, 27);
            bnOpenMonFolder.TabIndex = 35;
            bnOpenMonFolder.Text = "&Open";
            bnOpenMonFolder.UseVisualStyleBackColor = true;
            bnOpenMonFolder.Click += bnOpenMonFolder_Click;
            // 
            // bnAddMonFolder
            // 
            bnAddMonFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnAddMonFolder.Location = new Point(7, 198);
            bnAddMonFolder.Margin = new Padding(4, 3, 4, 3);
            bnAddMonFolder.Name = "bnAddMonFolder";
            bnAddMonFolder.Size = new Size(88, 27);
            bnAddMonFolder.TabIndex = 33;
            bnAddMonFolder.Text = "&Add";
            bnAddMonFolder.UseVisualStyleBackColor = true;
            bnAddMonFolder.Click += bnAddMonFolder_Click;
            // 
            // bnRemoveMonFolder
            // 
            bnRemoveMonFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnRemoveMonFolder.Enabled = false;
            bnRemoveMonFolder.Location = new Point(102, 198);
            bnRemoveMonFolder.Margin = new Padding(4, 3, 4, 3);
            bnRemoveMonFolder.Name = "bnRemoveMonFolder";
            bnRemoveMonFolder.Size = new Size(88, 27);
            bnRemoveMonFolder.TabIndex = 34;
            bnRemoveMonFolder.Text = "&Remove";
            bnRemoveMonFolder.UseVisualStyleBackColor = true;
            bnRemoveMonFolder.Click += bnRemoveMonFolder_Click;
            // 
            // pbLibraryFolders
            // 
            pbLibraryFolders.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbLibraryFolders.Cursor = Cursors.Hand;
            pbLibraryFolders.Image = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbLibraryFolders.InitialImage = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbLibraryFolders.Location = new Point(418, 7);
            pbLibraryFolders.Margin = new Padding(4, 3, 4, 3);
            pbLibraryFolders.Name = "pbLibraryFolders";
            pbLibraryFolders.Size = new Size(50, 46);
            pbLibraryFolders.SizeMode = PictureBoxSizeMode.CenterImage;
            pbLibraryFolders.TabIndex = 43;
            pbLibraryFolders.TabStop = false;
            pbLibraryFolders.Click += pbLibraryFolders_Click;
            // 
            // tpTorrentNZB
            // 
            tpTorrentNZB.Controls.Add(pbuTorrentNZB);
            tpTorrentNZB.Controls.Add(label58);
            tpTorrentNZB.Controls.Add(cbCheckqBitTorrent);
            tpTorrentNZB.Controls.Add(cbCheckSABnzbd);
            tpTorrentNZB.Controls.Add(cbCheckuTorrent);
            tpTorrentNZB.Controls.Add(qBitTorrent);
            tpTorrentNZB.Controls.Add(gbSAB);
            tpTorrentNZB.Controls.Add(gbuTorrent);
            tpTorrentNZB.Location = new Point(149, 4);
            tpTorrentNZB.Margin = new Padding(4, 3, 4, 3);
            tpTorrentNZB.Name = "tpTorrentNZB";
            tpTorrentNZB.Padding = new Padding(4, 3, 4, 3);
            tpTorrentNZB.Size = new Size(500, 684);
            tpTorrentNZB.TabIndex = 4;
            tpTorrentNZB.Text = "Torrents / NZB";
            tpTorrentNZB.UseVisualStyleBackColor = true;
            // 
            // pbuTorrentNZB
            // 
            pbuTorrentNZB.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbuTorrentNZB.Cursor = Cursors.Hand;
            pbuTorrentNZB.Image = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbuTorrentNZB.InitialImage = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbuTorrentNZB.Location = new Point(418, 7);
            pbuTorrentNZB.Margin = new Padding(4, 3, 4, 3);
            pbuTorrentNZB.Name = "pbuTorrentNZB";
            pbuTorrentNZB.Size = new Size(50, 46);
            pbuTorrentNZB.SizeMode = PictureBoxSizeMode.CenterImage;
            pbuTorrentNZB.TabIndex = 21;
            pbuTorrentNZB.TabStop = false;
            pbuTorrentNZB.Click += torrents_nzb_Click;
            // 
            // label58
            // 
            label58.AutoSize = true;
            label58.Location = new Point(4, 7);
            label58.Margin = new Padding(4, 0, 4, 0);
            label58.Name = "label58";
            label58.Size = new Size(370, 30);
            label58.TabIndex = 20;
            label58.Text = "If an episode is missing from your library, TV Rename will look in the \r\nfollowing locations to see whether it is already being downloaded.";
            // 
            // cbCheckqBitTorrent
            // 
            cbCheckqBitTorrent.AutoSize = true;
            cbCheckqBitTorrent.Location = new Point(7, 329);
            cbCheckqBitTorrent.Margin = new Padding(4, 3, 4, 3);
            cbCheckqBitTorrent.Name = "cbCheckqBitTorrent";
            cbCheckqBitTorrent.Size = new Size(157, 19);
            cbCheckqBitTorrent.TabIndex = 19;
            cbCheckqBitTorrent.Text = "Check &qBitTorrent queue";
            cbCheckqBitTorrent.UseVisualStyleBackColor = true;
            cbCheckqBitTorrent.CheckedChanged += EnableDisable;
            // 
            // cbCheckSABnzbd
            // 
            cbCheckSABnzbd.AutoSize = true;
            cbCheckSABnzbd.Location = new Point(7, 76);
            cbCheckSABnzbd.Margin = new Padding(4, 3, 4, 3);
            cbCheckSABnzbd.Name = "cbCheckSABnzbd";
            cbCheckSABnzbd.Size = new Size(145, 19);
            cbCheckSABnzbd.TabIndex = 18;
            cbCheckSABnzbd.Text = "Check SA&Bnzbd queue";
            cbCheckSABnzbd.UseVisualStyleBackColor = true;
            cbCheckSABnzbd.CheckedChanged += EnableDisable;
            // 
            // cbCheckuTorrent
            // 
            cbCheckuTorrent.AutoSize = true;
            cbCheckuTorrent.Location = new Point(7, 203);
            cbCheckuTorrent.Margin = new Padding(4, 3, 4, 3);
            cbCheckuTorrent.Name = "cbCheckuTorrent";
            cbCheckuTorrent.Size = new Size(143, 19);
            cbCheckuTorrent.TabIndex = 17;
            cbCheckuTorrent.Text = "C&heck µTorrent queue";
            cbCheckuTorrent.UseVisualStyleBackColor = true;
            cbCheckuTorrent.CheckedChanged += EnableDisable;
            // 
            // qBitTorrent
            // 
            qBitTorrent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            qBitTorrent.Controls.Add(chkBitTorrentUseHTTPS);
            qBitTorrent.Controls.Add(chkRemoveCompletedTorrents);
            qBitTorrent.Controls.Add(llqBitTorrentLink);
            qBitTorrent.Controls.Add(label79);
            qBitTorrent.Controls.Add(rdoqBitTorrentAPIVersionv2);
            qBitTorrent.Controls.Add(rdoqBitTorrentAPIVersionv1);
            qBitTorrent.Controls.Add(rdoqBitTorrentAPIVersionv0);
            qBitTorrent.Controls.Add(label29);
            qBitTorrent.Controls.Add(cbDownloadTorrentBeforeDownloading);
            qBitTorrent.Controls.Add(tbqBitTorrentHost);
            qBitTorrent.Controls.Add(tbqBitTorrentPort);
            qBitTorrent.Controls.Add(label41);
            qBitTorrent.Controls.Add(label42);
            qBitTorrent.Location = new Point(7, 355);
            qBitTorrent.Margin = new Padding(4, 3, 4, 3);
            qBitTorrent.Name = "qBitTorrent";
            qBitTorrent.Padding = new Padding(4, 3, 4, 3);
            qBitTorrent.Size = new Size(461, 203);
            qBitTorrent.TabIndex = 7;
            qBitTorrent.TabStop = false;
            qBitTorrent.Text = "qBitTorrent";
            // 
            // chkBitTorrentUseHTTPS
            // 
            chkBitTorrentUseHTTPS.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkBitTorrentUseHTTPS.AutoSize = true;
            chkBitTorrentUseHTTPS.Location = new Point(370, 59);
            chkBitTorrentUseHTTPS.Margin = new Padding(4, 3, 4, 3);
            chkBitTorrentUseHTTPS.Name = "chkBitTorrentUseHTTPS";
            chkBitTorrentUseHTTPS.Size = new Size(84, 19);
            chkBitTorrentUseHTTPS.TabIndex = 43;
            chkBitTorrentUseHTTPS.Text = "Use HTTPS";
            chkBitTorrentUseHTTPS.UseVisualStyleBackColor = true;
            chkBitTorrentUseHTTPS.CheckedChanged += chkBitTorrentUseHTTPS_CheckedChanged;
            // 
            // chkRemoveCompletedTorrents
            // 
            chkRemoveCompletedTorrents.AutoSize = true;
            chkRemoveCompletedTorrents.Location = new Point(88, 144);
            chkRemoveCompletedTorrents.Margin = new Padding(4, 3, 4, 3);
            chkRemoveCompletedTorrents.Name = "chkRemoveCompletedTorrents";
            chkRemoveCompletedTorrents.Size = new Size(208, 19);
            chkRemoveCompletedTorrents.TabIndex = 22;
            chkRemoveCompletedTorrents.Text = "Automatically Remove Completed";
            chkRemoveCompletedTorrents.UseVisualStyleBackColor = true;
            // 
            // llqBitTorrentLink
            // 
            llqBitTorrentLink.AutoSize = true;
            llqBitTorrentLink.Location = new Point(89, 168);
            llqBitTorrentLink.Margin = new Padding(4, 0, 4, 0);
            llqBitTorrentLink.Name = "llqBitTorrentLink";
            llqBitTorrentLink.Size = new Size(94, 15);
            llqBitTorrentLink.TabIndex = 42;
            llqBitTorrentLink.TabStop = true;
            llqBitTorrentLink.Text = "llqBitTorrentLink";
            llqBitTorrentLink.LinkClicked += LinkLabel1_LinkClicked;
            // 
            // label79
            // 
            label79.AutoSize = true;
            label79.Location = new Point(9, 167);
            label79.Margin = new Padding(4, 0, 4, 0);
            label79.Name = "label79";
            label79.Size = new Size(21, 15);
            label79.TabIndex = 41;
            label79.Text = "UI:";
            // 
            // rdoqBitTorrentAPIVersionv2
            // 
            rdoqBitTorrentAPIVersionv2.AutoSize = true;
            rdoqBitTorrentAPIVersionv2.Location = new Point(262, 87);
            rdoqBitTorrentAPIVersionv2.Margin = new Padding(4, 3, 4, 3);
            rdoqBitTorrentAPIVersionv2.Name = "rdoqBitTorrentAPIVersionv2";
            rdoqBitTorrentAPIVersionv2.Size = new Size(54, 19);
            rdoqBitTorrentAPIVersionv2.TabIndex = 24;
            rdoqBitTorrentAPIVersionv2.TabStop = true;
            rdoqBitTorrentAPIVersionv2.Text = "v4.1+";
            rdoqBitTorrentAPIVersionv2.UseVisualStyleBackColor = true;
            // 
            // rdoqBitTorrentAPIVersionv1
            // 
            rdoqBitTorrentAPIVersionv1.AutoSize = true;
            rdoqBitTorrentAPIVersionv1.Location = new Point(160, 87);
            rdoqBitTorrentAPIVersionv1.Margin = new Padding(4, 3, 4, 3);
            rdoqBitTorrentAPIVersionv1.Name = "rdoqBitTorrentAPIVersionv1";
            rdoqBitTorrentAPIVersionv1.Size = new Size(84, 19);
            rdoqBitTorrentAPIVersionv1.TabIndex = 23;
            rdoqBitTorrentAPIVersionv1.TabStop = true;
            rdoqBitTorrentAPIVersionv1.Text = "v3.2 to v4.0";
            rdoqBitTorrentAPIVersionv1.UseVisualStyleBackColor = true;
            // 
            // rdoqBitTorrentAPIVersionv0
            // 
            rdoqBitTorrentAPIVersionv0.AutoSize = true;
            rdoqBitTorrentAPIVersionv0.Location = new Point(89, 87);
            rdoqBitTorrentAPIVersionv0.Margin = new Padding(4, 3, 4, 3);
            rdoqBitTorrentAPIVersionv0.Name = "rdoqBitTorrentAPIVersionv0";
            rdoqBitTorrentAPIVersionv0.Size = new Size(57, 19);
            rdoqBitTorrentAPIVersionv0.TabIndex = 22;
            rdoqBitTorrentAPIVersionv0.TabStop = true;
            rdoqBitTorrentAPIVersionv0.Text = "< v3.1";
            rdoqBitTorrentAPIVersionv0.UseVisualStyleBackColor = true;
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Location = new Point(13, 89);
            label29.Margin = new Padding(4, 0, 4, 0);
            label29.Name = "label29";
            label29.Size = new Size(48, 15);
            label29.TabIndex = 21;
            label29.Text = "Version:";
            // 
            // cbDownloadTorrentBeforeDownloading
            // 
            cbDownloadTorrentBeforeDownloading.AutoSize = true;
            cbDownloadTorrentBeforeDownloading.Location = new Point(88, 118);
            cbDownloadTorrentBeforeDownloading.Margin = new Padding(4, 3, 4, 3);
            cbDownloadTorrentBeforeDownloading.Name = "cbDownloadTorrentBeforeDownloading";
            cbDownloadTorrentBeforeDownloading.Size = new Size(256, 19);
            cbDownloadTorrentBeforeDownloading.TabIndex = 20;
            cbDownloadTorrentBeforeDownloading.Text = "Download .torrent files before downloading";
            cbDownloadTorrentBeforeDownloading.UseVisualStyleBackColor = true;
            // 
            // tbqBitTorrentHost
            // 
            tbqBitTorrentHost.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbqBitTorrentHost.Location = new Point(88, 22);
            tbqBitTorrentHost.Margin = new Padding(4, 3, 4, 3);
            tbqBitTorrentHost.Name = "tbqBitTorrentHost";
            tbqBitTorrentHost.Size = new Size(366, 23);
            tbqBitTorrentHost.TabIndex = 1;
            tbqBitTorrentHost.TextChanged += QBitDetailsChanged;
            // 
            // tbqBitTorrentPort
            // 
            tbqBitTorrentPort.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbqBitTorrentPort.Location = new Point(88, 55);
            tbqBitTorrentPort.Margin = new Padding(4, 3, 4, 3);
            tbqBitTorrentPort.Name = "tbqBitTorrentPort";
            tbqBitTorrentPort.Size = new Size(252, 23);
            tbqBitTorrentPort.TabIndex = 4;
            tbqBitTorrentPort.TextChanged += QBitDetailsChanged;
            // 
            // label41
            // 
            label41.AutoSize = true;
            label41.Location = new Point(13, 59);
            label41.Margin = new Padding(4, 0, 4, 0);
            label41.Name = "label41";
            label41.Size = new Size(32, 15);
            label41.TabIndex = 3;
            label41.Text = "Port:";
            // 
            // label42
            // 
            label42.AutoSize = true;
            label42.Location = new Point(13, 25);
            label42.Margin = new Padding(4, 0, 4, 0);
            label42.Name = "label42";
            label42.Size = new Size(35, 15);
            label42.TabIndex = 0;
            label42.Text = "Host:";
            // 
            // gbSAB
            // 
            gbSAB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbSAB.Controls.Add(txtSABHostPort);
            gbSAB.Controls.Add(txtSABAPIKey);
            gbSAB.Controls.Add(label8);
            gbSAB.Controls.Add(label9);
            gbSAB.Location = new Point(7, 103);
            gbSAB.Margin = new Padding(4, 3, 4, 3);
            gbSAB.Name = "gbSAB";
            gbSAB.Padding = new Padding(4, 3, 4, 3);
            gbSAB.Size = new Size(461, 93);
            gbSAB.TabIndex = 6;
            gbSAB.TabStop = false;
            gbSAB.Text = "SABnzbd";
            // 
            // txtSABHostPort
            // 
            txtSABHostPort.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSABHostPort.Location = new Point(88, 22);
            txtSABHostPort.Margin = new Padding(4, 3, 4, 3);
            txtSABHostPort.Name = "txtSABHostPort";
            txtSABHostPort.Size = new Size(366, 23);
            txtSABHostPort.TabIndex = 1;
            // 
            // txtSABAPIKey
            // 
            txtSABAPIKey.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSABAPIKey.Location = new Point(88, 55);
            txtSABAPIKey.Margin = new Padding(4, 3, 4, 3);
            txtSABAPIKey.Name = "txtSABAPIKey";
            txtSABAPIKey.Size = new Size(366, 23);
            txtSABAPIKey.TabIndex = 4;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(13, 59);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(50, 15);
            label8.TabIndex = 3;
            label8.Text = "API Key:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(13, 25);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(57, 15);
            label9.TabIndex = 0;
            label9.Text = "Host:Port";
            // 
            // gbuTorrent
            // 
            gbuTorrent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbuTorrent.Controls.Add(bnUTBrowseResumeDat);
            gbuTorrent.Controls.Add(txtUTResumeDatPath);
            gbuTorrent.Controls.Add(bnRSSBrowseuTorrent);
            gbuTorrent.Controls.Add(label27);
            gbuTorrent.Controls.Add(label26);
            gbuTorrent.Controls.Add(txtRSSuTorrentPath);
            gbuTorrent.Location = new Point(7, 230);
            gbuTorrent.Margin = new Padding(4, 3, 4, 3);
            gbuTorrent.Name = "gbuTorrent";
            gbuTorrent.Padding = new Padding(4, 3, 4, 3);
            gbuTorrent.Size = new Size(461, 92);
            gbuTorrent.TabIndex = 5;
            gbuTorrent.TabStop = false;
            gbuTorrent.Text = "µTorrent";
            // 
            // bnUTBrowseResumeDat
            // 
            bnUTBrowseResumeDat.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnUTBrowseResumeDat.Location = new Point(366, 53);
            bnUTBrowseResumeDat.Margin = new Padding(4, 3, 4, 3);
            bnUTBrowseResumeDat.Name = "bnUTBrowseResumeDat";
            bnUTBrowseResumeDat.Size = new Size(88, 27);
            bnUTBrowseResumeDat.TabIndex = 5;
            bnUTBrowseResumeDat.Text = "Bro&wse...";
            bnUTBrowseResumeDat.UseVisualStyleBackColor = true;
            bnUTBrowseResumeDat.Click += bnUTBrowseResumeDat_Click;
            // 
            // txtUTResumeDatPath
            // 
            txtUTResumeDatPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUTResumeDatPath.Location = new Point(88, 55);
            txtUTResumeDatPath.Margin = new Padding(4, 3, 4, 3);
            txtUTResumeDatPath.Name = "txtUTResumeDatPath";
            txtUTResumeDatPath.Size = new Size(271, 23);
            txtUTResumeDatPath.TabIndex = 4;
            // 
            // bnRSSBrowseuTorrent
            // 
            bnRSSBrowseuTorrent.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnRSSBrowseuTorrent.Location = new Point(366, 18);
            bnRSSBrowseuTorrent.Margin = new Padding(4, 3, 4, 3);
            bnRSSBrowseuTorrent.Name = "bnRSSBrowseuTorrent";
            bnRSSBrowseuTorrent.Size = new Size(88, 27);
            bnRSSBrowseuTorrent.TabIndex = 2;
            bnRSSBrowseuTorrent.Text = "&Browse...";
            bnRSSBrowseuTorrent.UseVisualStyleBackColor = true;
            bnRSSBrowseuTorrent.Click += bnRSSBrowseuTorrent_Click;
            // 
            // label27
            // 
            label27.AutoSize = true;
            label27.Location = new Point(8, 25);
            label27.Margin = new Padding(4, 0, 4, 0);
            label27.Name = "label27";
            label27.Size = new Size(71, 15);
            label27.TabIndex = 0;
            label27.Text = "A&pplication:";
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Location = new Point(8, 59);
            label26.Margin = new Padding(4, 0, 4, 0);
            label26.Name = "label26";
            label26.Size = new Size(69, 15);
            label26.TabIndex = 3;
            label26.Text = "resume.&dat:";
            // 
            // txtRSSuTorrentPath
            // 
            txtRSSuTorrentPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtRSSuTorrentPath.Location = new Point(88, 22);
            txtRSSuTorrentPath.Margin = new Padding(4, 3, 4, 3);
            txtRSSuTorrentPath.Name = "txtRSSuTorrentPath";
            txtRSSuTorrentPath.Size = new Size(271, 23);
            txtRSSuTorrentPath.TabIndex = 1;
            // 
            // tbSearchFolders
            // 
            tbSearchFolders.Controls.Add(label98);
            tbSearchFolders.Controls.Add(upDownScanSeconds);
            tbSearchFolders.Controls.Add(chkUseSearchFullPathWhenMatchingShows);
            tbSearchFolders.Controls.Add(cbCopyFutureDatedEps);
            tbSearchFolders.Controls.Add(groupBox8);
            tbSearchFolders.Controls.Add(label67);
            tbSearchFolders.Controls.Add(gbAutoAdd);
            tbSearchFolders.Controls.Add(cbLeaveOriginals);
            tbSearchFolders.Controls.Add(cbSearchLocally);
            tbSearchFolders.Controls.Add(chkAutoMergeDownloadEpisodes);
            tbSearchFolders.Controls.Add(cbMonitorFolder);
            tbSearchFolders.Controls.Add(bnOpenSearchFolder);
            tbSearchFolders.Controls.Add(bnRemoveSearchFolder);
            tbSearchFolders.Controls.Add(bnAddSearchFolder);
            tbSearchFolders.Controls.Add(pbSearchFolders);
            tbSearchFolders.Controls.Add(lbSearchFolders);
            tbSearchFolders.Controls.Add(label23);
            tbSearchFolders.Location = new Point(149, 4);
            tbSearchFolders.Margin = new Padding(4, 3, 4, 3);
            tbSearchFolders.Name = "tbSearchFolders";
            tbSearchFolders.Size = new Size(500, 684);
            tbSearchFolders.TabIndex = 3;
            tbSearchFolders.Text = "Search Folders";
            tbSearchFolders.UseVisualStyleBackColor = true;
            // 
            // chkUseSearchFullPathWhenMatchingShows
            // 
            chkUseSearchFullPathWhenMatchingShows.AutoSize = true;
            chkUseSearchFullPathWhenMatchingShows.Location = new Point(7, 197);
            chkUseSearchFullPathWhenMatchingShows.Margin = new Padding(4, 3, 4, 3);
            chkUseSearchFullPathWhenMatchingShows.Name = "chkUseSearchFullPathWhenMatchingShows";
            chkUseSearchFullPathWhenMatchingShows.Size = new Size(451, 19);
            chkUseSearchFullPathWhenMatchingShows.TabIndex = 42;
            chkUseSearchFullPathWhenMatchingShows.Text = "Use name of Search Folder when searching for a match between a file and media";
            chkUseSearchFullPathWhenMatchingShows.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            groupBox8.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox8.Controls.Add(cbMovieHigherQuality);
            groupBox8.Controls.Add(label53);
            groupBox8.Controls.Add(label54);
            groupBox8.Controls.Add(tbPercentBetter);
            groupBox8.Controls.Add(tbPriorityOverrideTerms);
            groupBox8.Controls.Add(label52);
            groupBox8.Controls.Add(cbHigherQuality);
            groupBox8.Location = new Point(7, 510);
            groupBox8.Margin = new Padding(4, 3, 4, 3);
            groupBox8.Name = "groupBox8";
            groupBox8.Padding = new Padding(4, 3, 4, 3);
            groupBox8.Size = new Size(462, 133);
            groupBox8.TabIndex = 40;
            groupBox8.TabStop = false;
            groupBox8.Text = "Upgrade media when better quality files are found";
            // 
            // cbMovieHigherQuality
            // 
            cbMovieHigherQuality.AutoSize = true;
            cbMovieHigherQuality.Location = new Point(7, 42);
            cbMovieHigherQuality.Margin = new Padding(4, 3, 4, 3);
            cbMovieHigherQuality.Name = "cbMovieHigherQuality";
            cbMovieHigherQuality.Size = new Size(370, 19);
            cbMovieHigherQuality.TabIndex = 39;
            cbMovieHigherQuality.Text = "Update movies when higher-quality ones found in Search Folders";
            cbMovieHigherQuality.UseVisualStyleBackColor = true;
            // 
            // label53
            // 
            label53.AutoSize = true;
            label53.Location = new Point(8, 103);
            label53.Margin = new Padding(4, 0, 4, 0);
            label53.Name = "label53";
            label53.Size = new Size(147, 15);
            label53.TabIndex = 36;
            label53.Text = "Consider a file better if it is";
            // 
            // label54
            // 
            label54.AutoSize = true;
            label54.Location = new Point(202, 102);
            label54.Margin = new Padding(4, 0, 4, 0);
            label54.Name = "label54";
            label54.Size = new Size(149, 15);
            label54.TabIndex = 38;
            label54.Text = "% higher resolution/longer";
            // 
            // tbPercentBetter
            // 
            tbPercentBetter.Location = new Point(164, 98);
            tbPercentBetter.Margin = new Padding(4, 3, 4, 3);
            tbPercentBetter.Name = "tbPercentBetter";
            tbPercentBetter.Size = new Size(32, 23);
            tbPercentBetter.TabIndex = 37;
            tbPercentBetter.TextChanged += EnsureInteger;
            tbPercentBetter.KeyPress += TxtNumberOnlyKeyPress;
            // 
            // tbPriorityOverrideTerms
            // 
            tbPriorityOverrideTerms.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbPriorityOverrideTerms.Location = new Point(164, 68);
            tbPriorityOverrideTerms.Margin = new Padding(4, 3, 4, 3);
            tbPriorityOverrideTerms.Name = "tbPriorityOverrideTerms";
            tbPriorityOverrideTerms.Size = new Size(294, 23);
            tbPriorityOverrideTerms.TabIndex = 35;
            // 
            // label52
            // 
            label52.AutoSize = true;
            label52.Location = new Point(9, 72);
            label52.Margin = new Padding(4, 0, 4, 0);
            label52.Name = "label52";
            label52.Size = new Size(127, 15);
            label52.TabIndex = 34;
            label52.Text = "Priority override terms:";
            // 
            // cbHigherQuality
            // 
            cbHigherQuality.AutoSize = true;
            cbHigherQuality.Location = new Point(7, 22);
            cbHigherQuality.Margin = new Padding(4, 3, 4, 3);
            cbHigherQuality.Name = "cbHigherQuality";
            cbHigherQuality.Size = new Size(378, 19);
            cbHigherQuality.TabIndex = 33;
            cbHigherQuality.Text = "Update episodes when higher-quality ones found in Search Folders";
            cbHigherQuality.UseVisualStyleBackColor = true;
            // 
            // label67
            // 
            label67.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label67.AutoEllipsis = true;
            label67.AutoSize = true;
            label67.Location = new Point(7, 7);
            label67.Margin = new Padding(4, 0, 4, 0);
            label67.Name = "label67";
            label67.Size = new Size(370, 45);
            label67.TabIndex = 39;
            label67.Text = resources.GetString("label67.Text");
            // 
            // gbAutoAdd
            // 
            gbAutoAdd.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gbAutoAdd.Controls.Add(cbAutomateAutoAddWhenOneMovieFound);
            gbAutoAdd.Controls.Add(cbAutomateAutoAddWhenOneShowFound);
            gbAutoAdd.Controls.Add(chkAutoSearchForDownloadedFiles);
            gbAutoAdd.Controls.Add(label43);
            gbAutoAdd.Controls.Add(label44);
            gbAutoAdd.Controls.Add(tbIgnoreSuffixes);
            gbAutoAdd.Controls.Add(tbMovieTerms);
            gbAutoAdd.Location = new Point(7, 359);
            gbAutoAdd.Margin = new Padding(4, 3, 4, 3);
            gbAutoAdd.Name = "gbAutoAdd";
            gbAutoAdd.Padding = new Padding(4, 3, 4, 3);
            gbAutoAdd.Size = new Size(463, 144);
            gbAutoAdd.TabIndex = 36;
            gbAutoAdd.TabStop = false;
            gbAutoAdd.Text = "Auto Add Movies & TV Shows from Search Folders";
            // 
            // cbAutomateAutoAddWhenOneMovieFound
            // 
            cbAutomateAutoAddWhenOneMovieFound.AutoSize = true;
            cbAutomateAutoAddWhenOneMovieFound.Location = new Point(23, 67);
            cbAutomateAutoAddWhenOneMovieFound.Margin = new Padding(4, 3, 4, 3);
            cbAutomateAutoAddWhenOneMovieFound.Name = "cbAutomateAutoAddWhenOneMovieFound";
            cbAutomateAutoAddWhenOneMovieFound.Size = new Size(229, 19);
            cbAutomateAutoAddWhenOneMovieFound.TabIndex = 18;
            cbAutomateAutoAddWhenOneMovieFound.Text = "Auto Add when only one movie found";
            cbAutomateAutoAddWhenOneMovieFound.UseVisualStyleBackColor = true;
            // 
            // cbAutomateAutoAddWhenOneShowFound
            // 
            cbAutomateAutoAddWhenOneShowFound.AutoSize = true;
            cbAutomateAutoAddWhenOneShowFound.Location = new Point(23, 45);
            cbAutomateAutoAddWhenOneShowFound.Margin = new Padding(4, 3, 4, 3);
            cbAutomateAutoAddWhenOneShowFound.Name = "cbAutomateAutoAddWhenOneShowFound";
            cbAutomateAutoAddWhenOneShowFound.Size = new Size(237, 19);
            cbAutomateAutoAddWhenOneShowFound.TabIndex = 17;
            cbAutomateAutoAddWhenOneShowFound.Text = "Auto Add when only one tv show found";
            cbAutomateAutoAddWhenOneShowFound.UseVisualStyleBackColor = true;
            // 
            // chkAutoSearchForDownloadedFiles
            // 
            chkAutoSearchForDownloadedFiles.AutoSize = true;
            chkAutoSearchForDownloadedFiles.Location = new Point(7, 22);
            chkAutoSearchForDownloadedFiles.Margin = new Padding(4, 3, 4, 3);
            chkAutoSearchForDownloadedFiles.Name = "chkAutoSearchForDownloadedFiles";
            chkAutoSearchForDownloadedFiles.Size = new Size(198, 19);
            chkAutoSearchForDownloadedFiles.TabIndex = 16;
            chkAutoSearchForDownloadedFiles.Text = "Notify when new media is found";
            chkAutoSearchForDownloadedFiles.UseVisualStyleBackColor = true;
            // 
            // label43
            // 
            label43.AutoSize = true;
            label43.Location = new Point(5, 120);
            label43.Margin = new Padding(4, 0, 4, 0);
            label43.Name = "label43";
            label43.Size = new Size(86, 15);
            label43.TabIndex = 14;
            label43.Text = "&Ignore suffixes:";
            // 
            // label44
            // 
            label44.AutoSize = true;
            label44.Location = new Point(5, 90);
            label44.Margin = new Padding(4, 0, 4, 0);
            label44.Name = "label44";
            label44.Size = new Size(78, 15);
            label44.TabIndex = 12;
            label44.Text = "&Movie Terms:";
            // 
            // cbLeaveOriginals
            // 
            cbLeaveOriginals.AutoSize = true;
            cbLeaveOriginals.Location = new Point(20, 93);
            cbLeaveOriginals.Margin = new Padding(4, 3, 4, 3);
            cbLeaveOriginals.Name = "cbLeaveOriginals";
            cbLeaveOriginals.Size = new Size(145, 19);
            cbLeaveOriginals.TabIndex = 35;
            cbLeaveOriginals.Text = "&Copy files, don't move";
            cbLeaveOriginals.UseVisualStyleBackColor = true;
            // 
            // cbSearchLocally
            // 
            cbSearchLocally.AutoSize = true;
            cbSearchLocally.Checked = true;
            cbSearchLocally.CheckState = CheckState.Checked;
            cbSearchLocally.Location = new Point(7, 67);
            cbSearchLocally.Margin = new Padding(4, 3, 4, 3);
            cbSearchLocally.Name = "cbSearchLocally";
            cbSearchLocally.Size = new Size(240, 19);
            cbSearchLocally.TabIndex = 34;
            cbSearchLocally.Text = "&Look in \"Search Folders\" for missing files";
            cbSearchLocally.UseVisualStyleBackColor = true;
            cbSearchLocally.CheckedChanged += EnableDisable;
            // 
            // chkAutoMergeDownloadEpisodes
            // 
            chkAutoMergeDownloadEpisodes.AutoSize = true;
            chkAutoMergeDownloadEpisodes.Location = new Point(7, 120);
            chkAutoMergeDownloadEpisodes.Margin = new Padding(4, 3, 4, 3);
            chkAutoMergeDownloadEpisodes.Name = "chkAutoMergeDownloadEpisodes";
            chkAutoMergeDownloadEpisodes.Size = new Size(367, 19);
            chkAutoMergeDownloadEpisodes.TabIndex = 32;
            chkAutoMergeDownloadEpisodes.Text = "Automatically create merge rules based on files in Search Folders";
            chkAutoMergeDownloadEpisodes.UseVisualStyleBackColor = true;
            // 
            // bnOpenSearchFolder
            // 
            bnOpenSearchFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnOpenSearchFolder.Enabled = false;
            bnOpenSearchFolder.Location = new Point(196, 325);
            bnOpenSearchFolder.Margin = new Padding(4, 3, 4, 3);
            bnOpenSearchFolder.Name = "bnOpenSearchFolder";
            bnOpenSearchFolder.Size = new Size(88, 27);
            bnOpenSearchFolder.TabIndex = 4;
            bnOpenSearchFolder.Text = "&Open";
            bnOpenSearchFolder.UseVisualStyleBackColor = true;
            bnOpenSearchFolder.Click += bnOpenSearchFolder_Click;
            // 
            // bnRemoveSearchFolder
            // 
            bnRemoveSearchFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnRemoveSearchFolder.Enabled = false;
            bnRemoveSearchFolder.Location = new Point(102, 325);
            bnRemoveSearchFolder.Margin = new Padding(4, 3, 4, 3);
            bnRemoveSearchFolder.Name = "bnRemoveSearchFolder";
            bnRemoveSearchFolder.Size = new Size(88, 27);
            bnRemoveSearchFolder.TabIndex = 3;
            bnRemoveSearchFolder.Text = "&Remove";
            bnRemoveSearchFolder.UseVisualStyleBackColor = true;
            bnRemoveSearchFolder.Click += bnRemoveSearchFolder_Click;
            // 
            // bnAddSearchFolder
            // 
            bnAddSearchFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnAddSearchFolder.Location = new Point(7, 325);
            bnAddSearchFolder.Margin = new Padding(4, 3, 4, 3);
            bnAddSearchFolder.Name = "bnAddSearchFolder";
            bnAddSearchFolder.Size = new Size(88, 27);
            bnAddSearchFolder.TabIndex = 2;
            bnAddSearchFolder.Text = "&Add";
            bnAddSearchFolder.UseVisualStyleBackColor = true;
            bnAddSearchFolder.Click += bnAddSearchFolder_Click;
            // 
            // pbSearchFolders
            // 
            pbSearchFolders.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbSearchFolders.Cursor = Cursors.Hand;
            pbSearchFolders.Image = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbSearchFolders.InitialImage = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbSearchFolders.Location = new Point(418, 7);
            pbSearchFolders.Margin = new Padding(4, 3, 4, 3);
            pbSearchFolders.Name = "pbSearchFolders";
            pbSearchFolders.Size = new Size(50, 46);
            pbSearchFolders.SizeMode = PictureBoxSizeMode.CenterImage;
            pbSearchFolders.TabIndex = 37;
            pbSearchFolders.TabStop = false;
            pbSearchFolders.Click += pbSearchFolders_Click;
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Location = new Point(7, 220);
            label23.Margin = new Padding(4, 0, 4, 0);
            label23.Name = "label23";
            label23.Size = new Size(83, 15);
            label23.TabIndex = 0;
            label23.Text = "&Search Folders";
            // 
            // tbMediaCenter
            // 
            tbMediaCenter.Controls.Add(groupBox16);
            tbMediaCenter.Controls.Add(groupBox13);
            tbMediaCenter.Controls.Add(groupBox14);
            tbMediaCenter.Controls.Add(groupBox15);
            tbMediaCenter.Controls.Add(groupBox12);
            tbMediaCenter.Controls.Add(label64);
            tbMediaCenter.Controls.Add(bnMCPresets);
            tbMediaCenter.Controls.Add(pbMediaCenter);
            tbMediaCenter.Location = new Point(149, 4);
            tbMediaCenter.Margin = new Padding(4, 3, 4, 3);
            tbMediaCenter.Name = "tbMediaCenter";
            tbMediaCenter.Padding = new Padding(4, 3, 4, 3);
            tbMediaCenter.Size = new Size(500, 684);
            tbMediaCenter.TabIndex = 8;
            tbMediaCenter.Text = "Media Centres";
            tbMediaCenter.UseVisualStyleBackColor = true;
            // 
            // groupBox16
            // 
            groupBox16.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox16.Controls.Add(cbWDLiveEpisodeFiles);
            groupBox16.Location = new Point(10, 410);
            groupBox16.Margin = new Padding(4, 3, 4, 3);
            groupBox16.Name = "groupBox16";
            groupBox16.Padding = new Padding(4, 3, 4, 3);
            groupBox16.Size = new Size(457, 58);
            groupBox16.TabIndex = 41;
            groupBox16.TabStop = false;
            groupBox16.Text = "WD TV Live Hub";
            // 
            // cbWDLiveEpisodeFiles
            // 
            cbWDLiveEpisodeFiles.AutoSize = true;
            cbWDLiveEpisodeFiles.Location = new Point(7, 27);
            cbWDLiveEpisodeFiles.Margin = new Padding(4, 3, 4, 3);
            cbWDLiveEpisodeFiles.Name = "cbWDLiveEpisodeFiles";
            cbWDLiveEpisodeFiles.Size = new Size(215, 19);
            cbWDLiveEpisodeFiles.TabIndex = 25;
            cbWDLiveEpisodeFiles.Text = "WD TV Live Hub Episode Files (.xml)";
            cbWDLiveEpisodeFiles.UseVisualStyleBackColor = true;
            // 
            // groupBox13
            // 
            groupBox13.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox13.Controls.Add(cbXMLFiles);
            groupBox13.Controls.Add(cbSeriesJpg);
            groupBox13.Controls.Add(cbShrinkLarge);
            groupBox13.Location = new Point(10, 287);
            groupBox13.Margin = new Padding(4, 3, 4, 3);
            groupBox13.Name = "groupBox13";
            groupBox13.Padding = new Padding(4, 3, 4, 3);
            groupBox13.Size = new Size(457, 115);
            groupBox13.TabIndex = 40;
            groupBox13.TabStop = false;
            groupBox13.Text = "Mede8er";
            // 
            // cbXMLFiles
            // 
            cbXMLFiles.AutoSize = true;
            cbXMLFiles.Location = new Point(7, 48);
            cbXMLFiles.Margin = new Padding(4, 3, 4, 3);
            cbXMLFiles.Name = "cbXMLFiles";
            cbXMLFiles.Size = new Size(200, 19);
            cbXMLFiles.TabIndex = 8;
            cbXMLFiles.Text = "&XML files for shows and episodes";
            cbXMLFiles.UseVisualStyleBackColor = true;
            // 
            // cbSeriesJpg
            // 
            cbSeriesJpg.AutoSize = true;
            cbSeriesJpg.Location = new Point(7, 22);
            cbSeriesJpg.Margin = new Padding(4, 3, 4, 3);
            cbSeriesJpg.Name = "cbSeriesJpg";
            cbSeriesJpg.Size = new Size(266, 19);
            cbSeriesJpg.TabIndex = 7;
            cbSeriesJpg.Text = "&Create series poster (Series.jpg)";
            cbSeriesJpg.UseVisualStyleBackColor = true;
            // 
            // cbShrinkLarge
            // 
            cbShrinkLarge.AutoSize = true;
            cbShrinkLarge.Location = new Point(7, 75);
            cbShrinkLarge.Margin = new Padding(4, 3, 4, 3);
            cbShrinkLarge.Name = "cbShrinkLarge";
            cbShrinkLarge.Size = new Size(363, 19);
            cbShrinkLarge.TabIndex = 9;
            cbShrinkLarge.Text = "S&hrink large series and episode images to 156 x 232 pixels";
            cbShrinkLarge.UseVisualStyleBackColor = true;
            // 
            // groupBox14
            // 
            groupBox14.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox14.Controls.Add(cbMeta);
            groupBox14.Controls.Add(cbMetaSubfolder);
            groupBox14.Location = new Point(10, 205);
            groupBox14.Margin = new Padding(4, 3, 4, 3);
            groupBox14.Name = "groupBox14";
            groupBox14.Padding = new Padding(4, 3, 4, 3);
            groupBox14.Size = new Size(457, 75);
            groupBox14.TabIndex = 40;
            groupBox14.TabStop = false;
            groupBox14.Text = "pyTivo";
            // 
            // cbMeta
            // 
            cbMeta.AutoSize = true;
            cbMeta.Location = new Point(7, 22);
            cbMeta.Margin = new Padding(4, 3, 4, 3);
            cbMeta.Name = "cbMeta";
            cbMeta.Size = new Size(171, 19);
            cbMeta.TabIndex = 4;
            cbMeta.Text = "&Meta files for episodes (.txt)";
            cbMeta.UseVisualStyleBackColor = true;
            cbMeta.CheckedChanged += EnableDisable;
            // 
            // cbMetaSubfolder
            // 
            cbMetaSubfolder.AutoSize = true;
            cbMetaSubfolder.Location = new Point(7, 48);
            cbMetaSubfolder.Margin = new Padding(4, 3, 4, 3);
            cbMetaSubfolder.Name = "cbMetaSubfolder";
            cbMetaSubfolder.Size = new Size(207, 19);
            cbMetaSubfolder.TabIndex = 5;
            cbMetaSubfolder.Text = "Pl&ace Meta files in .meta subfolder";
            cbMetaSubfolder.UseVisualStyleBackColor = true;
            // 
            // groupBox15
            // 
            groupBox15.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox15.Controls.Add(cbNFOMovies);
            groupBox15.Controls.Add(cbEpTBNs);
            groupBox15.Controls.Add(cbNFOShows);
            groupBox15.Controls.Add(cbKODIImages);
            groupBox15.Controls.Add(cbNFOEpisodes);
            groupBox15.Location = new Point(10, 83);
            groupBox15.Margin = new Padding(4, 3, 4, 3);
            groupBox15.Name = "groupBox15";
            groupBox15.Padding = new Padding(4, 3, 4, 3);
            groupBox15.Size = new Size(457, 115);
            groupBox15.TabIndex = 40;
            groupBox15.TabStop = false;
            groupBox15.Text = "Kodi";
            // 
            // cbNFOMovies
            // 
            cbNFOMovies.AutoSize = true;
            cbNFOMovies.Location = new Point(7, 67);
            cbNFOMovies.Margin = new Padding(4, 3, 4, 3);
            cbNFOMovies.Name = "cbNFOMovies";
            cbNFOMovies.Size = new Size(133, 19);
            cbNFOMovies.TabIndex = 25;
            cbNFOMovies.Text = "&NFO files for movies";
            cbNFOMovies.UseVisualStyleBackColor = true;
            // 
            // cbEpTBNs
            // 
            cbEpTBNs.AutoSize = true;
            cbEpTBNs.Location = new Point(7, 22);
            cbEpTBNs.Margin = new Padding(4, 3, 4, 3);
            cbEpTBNs.Name = "cbEpTBNs";
            cbEpTBNs.Size = new Size(205, 19);
            cbEpTBNs.TabIndex = 1;
            cbEpTBNs.Text = "&Episode Thumbnails (-thumb.jpg)";
            cbEpTBNs.UseVisualStyleBackColor = true;
            // 
            // cbNFOShows
            // 
            cbNFOShows.AutoSize = true;
            cbNFOShows.Location = new Point(7, 46);
            cbNFOShows.Margin = new Padding(4, 3, 4, 3);
            cbNFOShows.Name = "cbNFOShows";
            cbNFOShows.Size = new Size(128, 19);
            cbNFOShows.TabIndex = 2;
            cbNFOShows.Text = "&NFO files for shows";
            cbNFOShows.UseVisualStyleBackColor = true;
            // 
            // cbKODIImages
            // 
            cbKODIImages.AutoSize = true;
            cbKODIImages.Location = new Point(7, 90);
            cbKODIImages.Margin = new Padding(4, 3, 4, 3);
            cbKODIImages.Name = "cbKODIImages";
            cbKODIImages.Size = new Size(265, 19);
            cbKODIImages.TabIndex = 17;
            cbKODIImages.Text = "Download &Images (fanart, poster, banner.jpg)";
            cbKODIImages.UseVisualStyleBackColor = true;
            // 
            // cbNFOEpisodes
            // 
            cbNFOEpisodes.AutoSize = true;
            cbNFOEpisodes.Location = new Point(176, 46);
            cbNFOEpisodes.Margin = new Padding(4, 3, 4, 3);
            cbNFOEpisodes.Name = "cbNFOEpisodes";
            cbNFOEpisodes.Size = new Size(141, 19);
            cbNFOEpisodes.TabIndex = 24;
            cbNFOEpisodes.Text = "&NFO files for episodes";
            cbNFOEpisodes.UseVisualStyleBackColor = true;
            // 
            // groupBox12
            // 
            groupBox12.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox12.Controls.Add(cbFantArtJpg);
            groupBox12.Controls.Add(cbFolderJpg);
            groupBox12.Controls.Add(cbEpThumbJpg);
            groupBox12.Controls.Add(panel1);
            groupBox12.Location = new Point(10, 474);
            groupBox12.Margin = new Padding(4, 3, 4, 3);
            groupBox12.Name = "groupBox12";
            groupBox12.Padding = new Padding(4, 3, 4, 3);
            groupBox12.Size = new Size(457, 129);
            groupBox12.TabIndex = 39;
            groupBox12.TabStop = false;
            groupBox12.Text = "General";
            // 
            // cbFantArtJpg
            // 
            cbFantArtJpg.AutoSize = true;
            cbFantArtJpg.Location = new Point(10, 75);
            cbFantArtJpg.Margin = new Padding(4, 3, 4, 3);
            cbFantArtJpg.Name = "cbFantArtJpg";
            cbFantArtJpg.Size = new Size(157, 19);
            cbFantArtJpg.TabIndex = 15;
            cbFantArtJpg.Text = "Fanar&t Image (fanart.jpg)";
            cbFantArtJpg.UseVisualStyleBackColor = true;
            // 
            // cbFolderJpg
            // 
            cbFolderJpg.AutoSize = true;
            cbFolderJpg.Location = new Point(10, 25);
            cbFolderJpg.Margin = new Padding(4, 3, 4, 3);
            cbFolderJpg.Name = "cbFolderJpg";
            cbFolderJpg.Size = new Size(157, 19);
            cbFolderJpg.TabIndex = 11;
            cbFolderJpg.Text = "&Folder image (folder.jpg)";
            cbFolderJpg.UseVisualStyleBackColor = true;
            // 
            // cbEpThumbJpg
            // 
            cbEpThumbJpg.AutoSize = true;
            cbEpThumbJpg.Location = new Point(10, 102);
            cbEpThumbJpg.Margin = new Padding(4, 3, 4, 3);
            cbEpThumbJpg.Name = "cbEpThumbJpg";
            cbEpThumbJpg.Size = new Size(164, 19);
            cbEpThumbJpg.TabIndex = 16;
            cbEpThumbJpg.Text = "Episode Thumbnails (.&jpg)";
            cbEpThumbJpg.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(rbFolderBanner);
            panel1.Controls.Add(rbFolderPoster);
            panel1.Controls.Add(rbFolderFanArt);
            panel1.Controls.Add(rbFolderSeasonPoster);
            panel1.Location = new Point(35, 40);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(327, 28);
            panel1.TabIndex = 22;
            // 
            // rbFolderBanner
            // 
            rbFolderBanner.AutoSize = true;
            rbFolderBanner.Location = new Point(0, 3);
            rbFolderBanner.Margin = new Padding(4, 3, 4, 3);
            rbFolderBanner.Name = "rbFolderBanner";
            rbFolderBanner.Size = new Size(62, 19);
            rbFolderBanner.TabIndex = 12;
            rbFolderBanner.TabStop = true;
            rbFolderBanner.Text = "&Banner";
            rbFolderBanner.UseVisualStyleBackColor = true;
            // 
            // rbFolderPoster
            // 
            rbFolderPoster.AutoSize = true;
            rbFolderPoster.Location = new Point(70, 3);
            rbFolderPoster.Margin = new Padding(4, 3, 4, 3);
            rbFolderPoster.Name = "rbFolderPoster";
            rbFolderPoster.Size = new Size(58, 19);
            rbFolderPoster.TabIndex = 13;
            rbFolderPoster.TabStop = true;
            rbFolderPoster.Text = "&Poster";
            rbFolderPoster.UseVisualStyleBackColor = true;
            // 
            // rbFolderFanArt
            // 
            rbFolderFanArt.AutoSize = true;
            rbFolderFanArt.Location = new Point(141, 3);
            rbFolderFanArt.Margin = new Padding(4, 3, 4, 3);
            rbFolderFanArt.Name = "rbFolderFanArt";
            rbFolderFanArt.Size = new Size(63, 19);
            rbFolderFanArt.TabIndex = 14;
            rbFolderFanArt.TabStop = true;
            rbFolderFanArt.Text = "Fan A&rt";
            rbFolderFanArt.UseVisualStyleBackColor = true;
            // 
            // rbFolderSeasonPoster
            // 
            rbFolderSeasonPoster.AutoSize = true;
            rbFolderSeasonPoster.Location = new Point(217, 3);
            rbFolderSeasonPoster.Margin = new Padding(4, 3, 4, 3);
            rbFolderSeasonPoster.Name = "rbFolderSeasonPoster";
            rbFolderSeasonPoster.Size = new Size(98, 19);
            rbFolderSeasonPoster.TabIndex = 16;
            rbFolderSeasonPoster.TabStop = true;
            rbFolderSeasonPoster.Text = "Seaso&n Poster";
            rbFolderSeasonPoster.UseVisualStyleBackColor = true;
            // 
            // label64
            // 
            label64.AutoSize = true;
            label64.Location = new Point(7, 8);
            label64.Margin = new Padding(4, 0, 4, 0);
            label64.Name = "label64";
            label64.Size = new Size(335, 45);
            label64.TabIndex = 38;
            label64.Text = "While scanning your library folders TV Rename can create and\r\ndownload additional files to help video playing applications to\r\nunderstand what is in your library and display it in a nicer way.";
            // 
            // bnMCPresets
            // 
            bnMCPresets.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            bnMCPresets.Location = new Point(380, 615);
            bnMCPresets.Margin = new Padding(4, 3, 4, 3);
            bnMCPresets.Name = "bnMCPresets";
            bnMCPresets.Size = new Size(88, 27);
            bnMCPresets.TabIndex = 16;
            bnMCPresets.Text = "Pre&sets...";
            bnMCPresets.UseVisualStyleBackColor = true;
            bnMCPresets.Click += bnMCPresets_Click;
            // 
            // pbMediaCenter
            // 
            pbMediaCenter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbMediaCenter.Cursor = Cursors.Hand;
            pbMediaCenter.Image = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbMediaCenter.InitialImage = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbMediaCenter.Location = new Point(418, 7);
            pbMediaCenter.Margin = new Padding(4, 3, 4, 3);
            pbMediaCenter.Name = "pbMediaCenter";
            pbMediaCenter.Size = new Size(50, 46);
            pbMediaCenter.SizeMode = PictureBoxSizeMode.CenterImage;
            pbMediaCenter.TabIndex = 27;
            pbMediaCenter.TabStop = false;
            pbMediaCenter.Click += pictureBox7_Click;
            // 
            // tbFolderDeleting
            // 
            tbFolderDeleting.Controls.Add(cbDeleteMovieFromDisk);
            tbFolderDeleting.Controls.Add(groupBox28);
            tbFolderDeleting.Controls.Add(label69);
            tbFolderDeleting.Controls.Add(cbDeleteShowFromDisk);
            tbFolderDeleting.Controls.Add(label32);
            tbFolderDeleting.Controls.Add(label30);
            tbFolderDeleting.Controls.Add(txtEmptyMaxSize);
            tbFolderDeleting.Controls.Add(txtEmptyIgnoreWords);
            tbFolderDeleting.Controls.Add(txtEmptyIgnoreExtensions);
            tbFolderDeleting.Controls.Add(label31);
            tbFolderDeleting.Controls.Add(cbRecycleNotDelete);
            tbFolderDeleting.Controls.Add(cbEmptyMaxSize);
            tbFolderDeleting.Controls.Add(cbEmptyIgnoreWords);
            tbFolderDeleting.Controls.Add(cbEmptyIgnoreExtensions);
            tbFolderDeleting.Controls.Add(cbDeleteEmpty);
            tbFolderDeleting.Controls.Add(pbFolderDeleting);
            tbFolderDeleting.Location = new Point(149, 4);
            tbFolderDeleting.Margin = new Padding(4, 3, 4, 3);
            tbFolderDeleting.Name = "tbFolderDeleting";
            tbFolderDeleting.Padding = new Padding(4, 3, 4, 3);
            tbFolderDeleting.Size = new Size(500, 684);
            tbFolderDeleting.TabIndex = 9;
            tbFolderDeleting.Text = "Folder Deleting";
            tbFolderDeleting.UseVisualStyleBackColor = true;
            // 
            // cbDeleteMovieFromDisk
            // 
            cbDeleteMovieFromDisk.AutoSize = true;
            cbDeleteMovieFromDisk.Location = new Point(19, 428);
            cbDeleteMovieFromDisk.Margin = new Padding(4, 3, 4, 3);
            cbDeleteMovieFromDisk.Name = "cbDeleteMovieFromDisk";
            cbDeleteMovieFromDisk.Size = new Size(340, 19);
            cbDeleteMovieFromDisk.TabIndex = 42;
            cbDeleteMovieFromDisk.Text = "Ask to delete from disk when deleting movie from database";
            cbDeleteMovieFromDisk.UseVisualStyleBackColor = true;
            // 
            // groupBox28
            // 
            groupBox28.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox28.Controls.Add(tbCleanUpDownloadDirMoviesLength);
            groupBox28.Controls.Add(cbCleanUpDownloadDirMoviesLength);
            groupBox28.Controls.Add(cbCleanUpDownloadDirMovies);
            groupBox28.Controls.Add(cbCleanUpDownloadDir);
            groupBox28.Location = new Point(19, 279);
            groupBox28.Margin = new Padding(4, 3, 4, 3);
            groupBox28.Name = "groupBox28";
            groupBox28.Padding = new Padding(4, 3, 4, 3);
            groupBox28.Size = new Size(449, 115);
            groupBox28.TabIndex = 41;
            groupBox28.TabStop = false;
            groupBox28.Text = "Clean Up Search Folders";
            // 
            // cbCleanUpDownloadDirMoviesLength
            // 
            cbCleanUpDownloadDirMoviesLength.AutoSize = true;
            cbCleanUpDownloadDirMoviesLength.Location = new Point(52, 75);
            cbCleanUpDownloadDirMoviesLength.Margin = new Padding(4, 3, 4, 3);
            cbCleanUpDownloadDirMoviesLength.Name = "cbCleanUpDownloadDirMoviesLength";
            cbCleanUpDownloadDirMoviesLength.Size = new Size(198, 19);
            cbCleanUpDownloadDirMoviesLength.TabIndex = 13;
            cbCleanUpDownloadDirMoviesLength.Text = "Only include movies longer than";
            cbCleanUpDownloadDirMoviesLength.UseVisualStyleBackColor = true;
            // 
            // cbCleanUpDownloadDirMovies
            // 
            cbCleanUpDownloadDirMovies.AutoSize = true;
            cbCleanUpDownloadDirMovies.Location = new Point(7, 48);
            cbCleanUpDownloadDirMovies.Margin = new Padding(4, 3, 4, 3);
            cbCleanUpDownloadDirMovies.Name = "cbCleanUpDownloadDirMovies";
            cbCleanUpDownloadDirMovies.Size = new Size(318, 19);
            cbCleanUpDownloadDirMovies.TabIndex = 12;
            cbCleanUpDownloadDirMovies.Text = "Clean up already copied movie files from search folders";
            cbCleanUpDownloadDirMovies.UseVisualStyleBackColor = true;
            // 
            // cbCleanUpDownloadDir
            // 
            cbCleanUpDownloadDir.AutoSize = true;
            cbCleanUpDownloadDir.Location = new Point(7, 22);
            cbCleanUpDownloadDir.Margin = new Padding(4, 3, 4, 3);
            cbCleanUpDownloadDir.Name = "cbCleanUpDownloadDir";
            cbCleanUpDownloadDir.Size = new Size(326, 19);
            cbCleanUpDownloadDir.TabIndex = 11;
            cbCleanUpDownloadDir.Text = "Clean up already copied episode files from search folders";
            cbCleanUpDownloadDir.UseVisualStyleBackColor = true;
            // 
            // label69
            // 
            label69.AutoSize = true;
            label69.Location = new Point(7, 7);
            label69.Margin = new Padding(4, 0, 4, 0);
            label69.Name = "label69";
            label69.Size = new Size(306, 30);
            label69.TabIndex = 40;
            label69.Text = "TV Rename can clean up the search folders to keep them\r\nclear of unused files and duplicate downloads.";
            // 
            // cbDeleteShowFromDisk
            // 
            cbDeleteShowFromDisk.AutoSize = true;
            cbDeleteShowFromDisk.Location = new Point(19, 402);
            cbDeleteShowFromDisk.Margin = new Padding(4, 3, 4, 3);
            cbDeleteShowFromDisk.Name = "cbDeleteShowFromDisk";
            cbDeleteShowFromDisk.Size = new Size(348, 19);
            cbDeleteShowFromDisk.TabIndex = 13;
            cbDeleteShowFromDisk.Text = "Ask to delete from disk when deleting tv show from database";
            cbDeleteShowFromDisk.UseVisualStyleBackColor = true;
            // 
            // label32
            // 
            label32.AutoSize = true;
            label32.Location = new Point(15, 194);
            label32.Margin = new Padding(4, 0, 4, 0);
            label32.Name = "label32";
            label32.Size = new Size(0, 15);
            label32.TabIndex = 6;
            // 
            // label30
            // 
            label30.AutoSize = true;
            label30.Location = new Point(15, 50);
            label30.Margin = new Padding(4, 0, 4, 0);
            label30.Name = "label30";
            label30.Size = new Size(0, 15);
            label30.TabIndex = 1;
            // 
            // txtEmptyMaxSize
            // 
            txtEmptyMaxSize.Location = new Point(254, 215);
            txtEmptyMaxSize.Margin = new Padding(4, 3, 4, 3);
            txtEmptyMaxSize.Name = "txtEmptyMaxSize";
            txtEmptyMaxSize.Size = new Size(63, 23);
            txtEmptyMaxSize.TabIndex = 8;
            // 
            // label31
            // 
            label31.AutoSize = true;
            label31.Location = new Point(328, 223);
            label31.Margin = new Padding(4, 0, 4, 0);
            label31.Name = "label31";
            label31.Size = new Size(0, 15);
            label31.TabIndex = 9;
            // 
            // cbRecycleNotDelete
            // 
            cbRecycleNotDelete.AutoSize = true;
            cbRecycleNotDelete.Location = new Point(19, 253);
            cbRecycleNotDelete.Margin = new Padding(4, 3, 4, 3);
            cbRecycleNotDelete.Name = "cbRecycleNotDelete";
            cbRecycleNotDelete.Size = new Size(333, 19);
            cbRecycleNotDelete.TabIndex = 10;
            cbRecycleNotDelete.Text = "Folders with files are moved to the &recycle bin, not deleted";
            cbRecycleNotDelete.UseVisualStyleBackColor = true;
            // 
            // cbEmptyMaxSize
            // 
            cbEmptyMaxSize.AutoSize = true;
            cbEmptyMaxSize.Location = new Point(41, 217);
            cbEmptyMaxSize.Margin = new Padding(4, 3, 4, 3);
            cbEmptyMaxSize.Name = "cbEmptyMaxSize";
            cbEmptyMaxSize.Size = new Size(200, 19);
            cbEmptyMaxSize.TabIndex = 7;
            cbEmptyMaxSize.Text = "&Maximum total file size to delete:";
            cbEmptyMaxSize.UseVisualStyleBackColor = true;
            // 
            // cbEmptyIgnoreWords
            // 
            cbEmptyIgnoreWords.AutoSize = true;
            cbEmptyIgnoreWords.Location = new Point(41, 102);
            cbEmptyIgnoreWords.Margin = new Padding(4, 3, 4, 3);
            cbEmptyIgnoreWords.Name = "cbEmptyIgnoreWords";
            cbEmptyIgnoreWords.Size = new Size(412, 19);
            cbEmptyIgnoreWords.TabIndex = 2;
            cbEmptyIgnoreWords.Text = "Ignore any files with these &words in their name: (semicolon separated list)";
            cbEmptyIgnoreWords.UseVisualStyleBackColor = true;
            // 
            // cbEmptyIgnoreExtensions
            // 
            cbEmptyIgnoreExtensions.AutoSize = true;
            cbEmptyIgnoreExtensions.Location = new Point(41, 159);
            cbEmptyIgnoreExtensions.Margin = new Padding(4, 3, 4, 3);
            cbEmptyIgnoreExtensions.Name = "cbEmptyIgnoreExtensions";
            cbEmptyIgnoreExtensions.Size = new Size(340, 19);
            cbEmptyIgnoreExtensions.TabIndex = 4;
            cbEmptyIgnoreExtensions.Text = "&Ignore files with these extensions: (semicolon separated list)";
            cbEmptyIgnoreExtensions.UseVisualStyleBackColor = true;
            // 
            // cbDeleteEmpty
            // 
            cbDeleteEmpty.AutoSize = true;
            cbDeleteEmpty.Location = new Point(19, 75);
            cbDeleteEmpty.Margin = new Padding(4, 3, 4, 3);
            cbDeleteEmpty.Name = "cbDeleteEmpty";
            cbDeleteEmpty.Size = new Size(346, 19);
            cbDeleteEmpty.TabIndex = 0;
            cbDeleteEmpty.Text = "&Delete empty folders after moving files (from Search Folders)";
            cbDeleteEmpty.UseVisualStyleBackColor = true;
            // 
            // pbFolderDeleting
            // 
            pbFolderDeleting.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbFolderDeleting.Cursor = Cursors.Hand;
            pbFolderDeleting.Image = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbFolderDeleting.InitialImage = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbFolderDeleting.Location = new Point(418, 7);
            pbFolderDeleting.Margin = new Padding(4, 3, 4, 3);
            pbFolderDeleting.Name = "pbFolderDeleting";
            pbFolderDeleting.Size = new Size(50, 46);
            pbFolderDeleting.SizeMode = PictureBoxSizeMode.CenterImage;
            pbFolderDeleting.TabIndex = 24;
            pbFolderDeleting.TabStop = false;
            pbFolderDeleting.Click += pbFolderDeleting_Click;
            // 
            // tbAutoExport
            // 
            tbAutoExport.Controls.Add(pbuExportEpisodes);
            tbAutoExport.Controls.Add(label88);
            tbAutoExport.Controls.Add(groupBox10);
            tbAutoExport.Controls.Add(groupBox5);
            tbAutoExport.Controls.Add(groupBox4);
            tbAutoExport.Controls.Add(groupBox2);
            tbAutoExport.Location = new Point(149, 4);
            tbAutoExport.Margin = new Padding(4, 3, 4, 3);
            tbAutoExport.Name = "tbAutoExport";
            tbAutoExport.Padding = new Padding(4, 3, 4, 3);
            tbAutoExport.Size = new Size(500, 684);
            tbAutoExport.TabIndex = 2;
            tbAutoExport.Text = "Episode Export";
            tbAutoExport.UseVisualStyleBackColor = true;
            // 
            // pbuExportEpisodes
            // 
            pbuExportEpisodes.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbuExportEpisodes.Cursor = Cursors.Hand;
            pbuExportEpisodes.Image = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbuExportEpisodes.InitialImage = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbuExportEpisodes.Location = new Point(416, 9);
            pbuExportEpisodes.Margin = new Padding(4, 3, 4, 3);
            pbuExportEpisodes.Name = "pbuExportEpisodes";
            pbuExportEpisodes.Size = new Size(50, 46);
            pbuExportEpisodes.SizeMode = PictureBoxSizeMode.CenterImage;
            pbuExportEpisodes.TabIndex = 44;
            pbuExportEpisodes.TabStop = false;
            pbuExportEpisodes.Click += pbuExportEpisodes_Click;
            // 
            // label88
            // 
            label88.AutoSize = true;
            label88.Location = new Point(4, 3);
            label88.Margin = new Padding(4, 0, 4, 0);
            label88.Name = "label88";
            label88.Size = new Size(294, 45);
            label88.TabIndex = 43;
            label88.Text = "TV Rename can export information about episodes\r\nin various formats. Some focus on upcoming episodes\r\nand others are based on recently aired.";
            // 
            // groupBox10
            // 
            groupBox10.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox10.Controls.Add(bnBrowseWPL);
            groupBox10.Controls.Add(txtWPL);
            groupBox10.Controls.Add(cbWPL);
            groupBox10.Controls.Add(bnBrowseASX);
            groupBox10.Controls.Add(txtASX);
            groupBox10.Controls.Add(cbASX);
            groupBox10.Controls.Add(bnBrowseM3U);
            groupBox10.Controls.Add(txtM3U);
            groupBox10.Controls.Add(cbM3U);
            groupBox10.Controls.Add(bnBrowseXSPF);
            groupBox10.Controls.Add(txtXSPF);
            groupBox10.Controls.Add(cbXSPF);
            groupBox10.Location = new Point(8, 396);
            groupBox10.Margin = new Padding(4, 3, 4, 3);
            groupBox10.Name = "groupBox10";
            groupBox10.Padding = new Padding(4, 3, 4, 3);
            groupBox10.Size = new Size(461, 156);
            groupBox10.TabIndex = 5;
            groupBox10.TabStop = false;
            groupBox10.Text = "Recent Playlist";
            // 
            // bnBrowseWPL
            // 
            bnBrowseWPL.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnBrowseWPL.Location = new Point(364, 120);
            bnBrowseWPL.Margin = new Padding(4, 3, 4, 3);
            bnBrowseWPL.Name = "bnBrowseWPL";
            bnBrowseWPL.Size = new Size(88, 27);
            bnBrowseWPL.TabIndex = 27;
            bnBrowseWPL.Text = "Browse...";
            bnBrowseWPL.UseVisualStyleBackColor = true;
            bnBrowseWPL.Click += bnBrowseWPL_Click;
            // 
            // txtWPL
            // 
            txtWPL.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtWPL.Location = new Point(76, 122);
            txtWPL.Margin = new Padding(4, 3, 4, 3);
            txtWPL.Name = "txtWPL";
            txtWPL.Size = new Size(279, 23);
            txtWPL.TabIndex = 26;
            // 
            // cbWPL
            // 
            cbWPL.AutoSize = true;
            cbWPL.Location = new Point(9, 125);
            cbWPL.Margin = new Padding(4, 3, 4, 3);
            cbWPL.Name = "cbWPL";
            cbWPL.Size = new Size(50, 19);
            cbWPL.TabIndex = 25;
            cbWPL.Text = "WPL";
            cbWPL.UseVisualStyleBackColor = true;
            cbWPL.CheckedChanged += EnableDisable;
            // 
            // bnBrowseASX
            // 
            bnBrowseASX.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnBrowseASX.Location = new Point(364, 88);
            bnBrowseASX.Margin = new Padding(4, 3, 4, 3);
            bnBrowseASX.Name = "bnBrowseASX";
            bnBrowseASX.Size = new Size(88, 27);
            bnBrowseASX.TabIndex = 24;
            bnBrowseASX.Text = "Browse...";
            bnBrowseASX.UseVisualStyleBackColor = true;
            bnBrowseASX.Click += bnBrowseASX_Click;
            // 
            // txtASX
            // 
            txtASX.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtASX.Location = new Point(76, 90);
            txtASX.Margin = new Padding(4, 3, 4, 3);
            txtASX.Name = "txtASX";
            txtASX.Size = new Size(279, 23);
            txtASX.TabIndex = 23;
            // 
            // cbASX
            // 
            cbASX.AutoSize = true;
            cbASX.Location = new Point(9, 92);
            cbASX.Margin = new Padding(4, 3, 4, 3);
            cbASX.Name = "cbASX";
            cbASX.Size = new Size(47, 19);
            cbASX.TabIndex = 22;
            cbASX.Text = "ASX";
            cbASX.UseVisualStyleBackColor = true;
            cbASX.CheckedChanged += EnableDisable;
            // 
            // bnBrowseM3U
            // 
            bnBrowseM3U.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnBrowseM3U.Location = new Point(363, 53);
            bnBrowseM3U.Margin = new Padding(4, 3, 4, 3);
            bnBrowseM3U.Name = "bnBrowseM3U";
            bnBrowseM3U.Size = new Size(88, 27);
            bnBrowseM3U.TabIndex = 19;
            bnBrowseM3U.Text = "Browse...";
            bnBrowseM3U.UseVisualStyleBackColor = true;
            bnBrowseM3U.Click += bnBrowseM3U_Click;
            // 
            // txtM3U
            // 
            txtM3U.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtM3U.Location = new Point(76, 57);
            txtM3U.Margin = new Padding(4, 3, 4, 3);
            txtM3U.Name = "txtM3U";
            txtM3U.Size = new Size(279, 23);
            txtM3U.TabIndex = 18;
            // 
            // cbM3U
            // 
            cbM3U.AutoSize = true;
            cbM3U.Location = new Point(9, 59);
            cbM3U.Margin = new Padding(4, 3, 4, 3);
            cbM3U.Name = "cbM3U";
            cbM3U.Size = new Size(62, 19);
            cbM3U.TabIndex = 17;
            cbM3U.Text = "M3U/8";
            cbM3U.UseVisualStyleBackColor = true;
            cbM3U.CheckedChanged += EnableDisable;
            // 
            // bnBrowseXSPF
            // 
            bnBrowseXSPF.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnBrowseXSPF.Location = new Point(364, 21);
            bnBrowseXSPF.Margin = new Padding(4, 3, 4, 3);
            bnBrowseXSPF.Name = "bnBrowseXSPF";
            bnBrowseXSPF.Size = new Size(88, 27);
            bnBrowseXSPF.TabIndex = 2;
            bnBrowseXSPF.Text = "Browse...";
            bnBrowseXSPF.UseVisualStyleBackColor = true;
            bnBrowseXSPF.Click += bnBrowseXSPF_Click;
            // 
            // txtXSPF
            // 
            txtXSPF.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtXSPF.Location = new Point(75, 23);
            txtXSPF.Margin = new Padding(4, 3, 4, 3);
            txtXSPF.Name = "txtXSPF";
            txtXSPF.Size = new Size(282, 23);
            txtXSPF.TabIndex = 1;
            // 
            // cbXSPF
            // 
            cbXSPF.AutoSize = true;
            cbXSPF.Location = new Point(9, 25);
            cbXSPF.Margin = new Padding(4, 3, 4, 3);
            cbXSPF.Name = "cbXSPF";
            cbXSPF.Size = new Size(52, 19);
            cbXSPF.TabIndex = 0;
            cbXSPF.Text = "XSPF";
            cbXSPF.UseVisualStyleBackColor = true;
            cbXSPF.CheckedChanged += EnableDisable;
            // 
            // groupBox5
            // 
            groupBox5.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox5.Controls.Add(bnBrowseFOXML);
            groupBox5.Controls.Add(cbFOXML);
            groupBox5.Controls.Add(txtFOXML);
            groupBox5.Location = new Point(10, 325);
            groupBox5.Margin = new Padding(4, 3, 4, 3);
            groupBox5.Name = "groupBox5";
            groupBox5.Padding = new Padding(4, 3, 4, 3);
            groupBox5.Size = new Size(461, 63);
            groupBox5.TabIndex = 3;
            groupBox5.TabStop = false;
            groupBox5.Text = "Finding and Organising";
            // 
            // bnBrowseFOXML
            // 
            bnBrowseFOXML.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnBrowseFOXML.Location = new Point(363, 22);
            bnBrowseFOXML.Margin = new Padding(4, 3, 4, 3);
            bnBrowseFOXML.Name = "bnBrowseFOXML";
            bnBrowseFOXML.Size = new Size(88, 27);
            bnBrowseFOXML.TabIndex = 2;
            bnBrowseFOXML.Text = "Browse...";
            bnBrowseFOXML.UseVisualStyleBackColor = true;
            bnBrowseFOXML.Click += bnBrowseFOXML_Click;
            // 
            // cbFOXML
            // 
            cbFOXML.AutoSize = true;
            cbFOXML.Location = new Point(9, 27);
            cbFOXML.Margin = new Padding(4, 3, 4, 3);
            cbFOXML.Name = "cbFOXML";
            cbFOXML.Size = new Size(50, 19);
            cbFOXML.TabIndex = 0;
            cbFOXML.Text = "XML";
            cbFOXML.UseVisualStyleBackColor = true;
            cbFOXML.CheckedChanged += EnableDisable;
            // 
            // txtFOXML
            // 
            txtFOXML.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFOXML.Location = new Point(75, 24);
            txtFOXML.Margin = new Padding(4, 3, 4, 3);
            txtFOXML.Name = "txtFOXML";
            txtFOXML.Size = new Size(280, 23);
            txtFOXML.TabIndex = 1;
            // 
            // groupBox4
            // 
            groupBox4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox4.Controls.Add(bnBrowseRenamingXML);
            groupBox4.Controls.Add(cbRenamingXML);
            groupBox4.Controls.Add(txtRenamingXML);
            groupBox4.Location = new Point(10, 253);
            groupBox4.Margin = new Padding(4, 3, 4, 3);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new Padding(4, 3, 4, 3);
            groupBox4.Size = new Size(461, 66);
            groupBox4.TabIndex = 2;
            groupBox4.TabStop = false;
            groupBox4.Text = "Renaming";
            // 
            // bnBrowseRenamingXML
            // 
            bnBrowseRenamingXML.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnBrowseRenamingXML.Location = new Point(363, 22);
            bnBrowseRenamingXML.Margin = new Padding(4, 3, 4, 3);
            bnBrowseRenamingXML.Name = "bnBrowseRenamingXML";
            bnBrowseRenamingXML.Size = new Size(88, 27);
            bnBrowseRenamingXML.TabIndex = 2;
            bnBrowseRenamingXML.Text = "Browse...";
            bnBrowseRenamingXML.UseVisualStyleBackColor = true;
            bnBrowseRenamingXML.Click += bnBrowseRenamingXML_Click;
            // 
            // cbRenamingXML
            // 
            cbRenamingXML.AutoSize = true;
            cbRenamingXML.Location = new Point(9, 27);
            cbRenamingXML.Margin = new Padding(4, 3, 4, 3);
            cbRenamingXML.Name = "cbRenamingXML";
            cbRenamingXML.Size = new Size(50, 19);
            cbRenamingXML.TabIndex = 0;
            cbRenamingXML.Text = "XML";
            cbRenamingXML.UseVisualStyleBackColor = true;
            cbRenamingXML.CheckedChanged += EnableDisable;
            // 
            // txtRenamingXML
            // 
            txtRenamingXML.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtRenamingXML.Location = new Point(75, 24);
            txtRenamingXML.Margin = new Padding(4, 3, 4, 3);
            txtRenamingXML.Name = "txtRenamingXML";
            txtRenamingXML.Size = new Size(280, 23);
            txtRenamingXML.TabIndex = 1;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(bnBrowseWTWTXT);
            groupBox2.Controls.Add(txtWTWTXT);
            groupBox2.Controls.Add(cbWTWTXT);
            groupBox2.Controls.Add(bnBrowseWTWICAL);
            groupBox2.Controls.Add(txtWTWICAL);
            groupBox2.Controls.Add(cbWTWICAL);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(txtExportRSSDaysPast);
            groupBox2.Controls.Add(bnBrowseWTWXML);
            groupBox2.Controls.Add(txtWTWXML);
            groupBox2.Controls.Add(cbWTWXML);
            groupBox2.Controls.Add(bnBrowseWTWRSS);
            groupBox2.Controls.Add(txtWTWRSS);
            groupBox2.Controls.Add(cbWTWRSS);
            groupBox2.Controls.Add(label17);
            groupBox2.Controls.Add(label16);
            groupBox2.Controls.Add(label15);
            groupBox2.Controls.Add(txtExportRSSMaxDays);
            groupBox2.Controls.Add(txtExportRSSMaxShows);
            groupBox2.Location = new Point(10, 62);
            groupBox2.Margin = new Padding(4, 3, 4, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4, 3, 4, 3);
            groupBox2.Size = new Size(461, 183);
            groupBox2.TabIndex = 0;
            groupBox2.TabStop = false;
            groupBox2.Text = "Schedule";
            // 
            // bnBrowseWTWTXT
            // 
            bnBrowseWTWTXT.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnBrowseWTWTXT.Location = new Point(364, 118);
            bnBrowseWTWTXT.Margin = new Padding(4, 3, 4, 3);
            bnBrowseWTWTXT.Name = "bnBrowseWTWTXT";
            bnBrowseWTWTXT.Size = new Size(88, 27);
            bnBrowseWTWTXT.TabIndex = 27;
            bnBrowseWTWTXT.Text = "Browse...";
            bnBrowseWTWTXT.UseVisualStyleBackColor = true;
            bnBrowseWTWTXT.Click += bnBrowseWTWTXT_Click;
            // 
            // txtWTWTXT
            // 
            txtWTWTXT.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtWTWTXT.Location = new Point(76, 120);
            txtWTWTXT.Margin = new Padding(4, 3, 4, 3);
            txtWTWTXT.Name = "txtWTWTXT";
            txtWTWTXT.Size = new Size(279, 23);
            txtWTWTXT.TabIndex = 26;
            // 
            // cbWTWTXT
            // 
            cbWTWTXT.AutoSize = true;
            cbWTWTXT.Location = new Point(9, 122);
            cbWTWTXT.Margin = new Padding(4, 3, 4, 3);
            cbWTWTXT.Name = "cbWTWTXT";
            cbWTWTXT.Size = new Size(47, 19);
            cbWTWTXT.TabIndex = 25;
            cbWTWTXT.Text = "TXT";
            cbWTWTXT.UseVisualStyleBackColor = true;
            cbWTWTXT.CheckedChanged += EnableDisable;
            // 
            // bnBrowseWTWICAL
            // 
            bnBrowseWTWICAL.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnBrowseWTWICAL.Location = new Point(364, 88);
            bnBrowseWTWICAL.Margin = new Padding(4, 3, 4, 3);
            bnBrowseWTWICAL.Name = "bnBrowseWTWICAL";
            bnBrowseWTWICAL.Size = new Size(88, 27);
            bnBrowseWTWICAL.TabIndex = 24;
            bnBrowseWTWICAL.Text = "Browse...";
            bnBrowseWTWICAL.UseVisualStyleBackColor = true;
            bnBrowseWTWICAL.Click += bnBrowseWTWICAL_Click;
            // 
            // txtWTWICAL
            // 
            txtWTWICAL.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtWTWICAL.Location = new Point(76, 90);
            txtWTWICAL.Margin = new Padding(4, 3, 4, 3);
            txtWTWICAL.Name = "txtWTWICAL";
            txtWTWICAL.Size = new Size(279, 23);
            txtWTWICAL.TabIndex = 23;
            // 
            // cbWTWICAL
            // 
            cbWTWICAL.AutoSize = true;
            cbWTWICAL.Location = new Point(9, 92);
            cbWTWICAL.Margin = new Padding(4, 3, 4, 3);
            cbWTWICAL.Name = "cbWTWICAL";
            cbWTWICAL.Size = new Size(46, 19);
            cbWTWICAL.TabIndex = 22;
            cbWTWICAL.Text = "iCal";
            cbWTWICAL.UseVisualStyleBackColor = true;
            cbWTWICAL.CheckedChanged += EnableDisable;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(382, 153);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(65, 15);
            label4.TabIndex = 21;
            label4.Text = "in the past.";
            // 
            // txtExportRSSDaysPast
            // 
            txtExportRSSDaysPast.Location = new Point(340, 150);
            txtExportRSSDaysPast.Margin = new Padding(4, 3, 4, 3);
            txtExportRSSDaysPast.Name = "txtExportRSSDaysPast";
            txtExportRSSDaysPast.Size = new Size(32, 23);
            txtExportRSSDaysPast.TabIndex = 20;
            txtExportRSSDaysPast.TextChanged += EnsureInteger;
            txtExportRSSDaysPast.KeyPress += TxtNumberOnlyKeyPress;
            // 
            // bnBrowseWTWXML
            // 
            bnBrowseWTWXML.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnBrowseWTWXML.Location = new Point(363, 53);
            bnBrowseWTWXML.Margin = new Padding(4, 3, 4, 3);
            bnBrowseWTWXML.Name = "bnBrowseWTWXML";
            bnBrowseWTWXML.Size = new Size(88, 27);
            bnBrowseWTWXML.TabIndex = 19;
            bnBrowseWTWXML.Text = "Browse...";
            bnBrowseWTWXML.UseVisualStyleBackColor = true;
            bnBrowseWTWXML.Click += bnBrowseWTWXML_Click;
            // 
            // txtWTWXML
            // 
            txtWTWXML.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtWTWXML.Location = new Point(76, 57);
            txtWTWXML.Margin = new Padding(4, 3, 4, 3);
            txtWTWXML.Name = "txtWTWXML";
            txtWTWXML.Size = new Size(279, 23);
            txtWTWXML.TabIndex = 18;
            // 
            // cbWTWXML
            // 
            cbWTWXML.AutoSize = true;
            cbWTWXML.Location = new Point(9, 59);
            cbWTWXML.Margin = new Padding(4, 3, 4, 3);
            cbWTWXML.Name = "cbWTWXML";
            cbWTWXML.Size = new Size(50, 19);
            cbWTWXML.TabIndex = 17;
            cbWTWXML.Text = "XML";
            cbWTWXML.UseVisualStyleBackColor = true;
            cbWTWXML.CheckedChanged += EnableDisable;
            // 
            // bnBrowseWTWRSS
            // 
            bnBrowseWTWRSS.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnBrowseWTWRSS.Location = new Point(364, 21);
            bnBrowseWTWRSS.Margin = new Padding(4, 3, 4, 3);
            bnBrowseWTWRSS.Name = "bnBrowseWTWRSS";
            bnBrowseWTWRSS.Size = new Size(88, 27);
            bnBrowseWTWRSS.TabIndex = 2;
            bnBrowseWTWRSS.Text = "Browse...";
            bnBrowseWTWRSS.UseVisualStyleBackColor = true;
            bnBrowseWTWRSS.Click += bnBrowseWTWRSS_Click;
            // 
            // txtWTWRSS
            // 
            txtWTWRSS.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtWTWRSS.Location = new Point(75, 23);
            txtWTWRSS.Margin = new Padding(4, 3, 4, 3);
            txtWTWRSS.Name = "txtWTWRSS";
            txtWTWRSS.Size = new Size(282, 23);
            txtWTWRSS.TabIndex = 1;
            // 
            // cbWTWRSS
            // 
            cbWTWRSS.AutoSize = true;
            cbWTWRSS.Location = new Point(9, 25);
            cbWTWRSS.Margin = new Padding(4, 3, 4, 3);
            cbWTWRSS.Name = "cbWTWRSS";
            cbWTWRSS.Size = new Size(45, 19);
            cbWTWRSS.TabIndex = 0;
            cbWTWRSS.Text = "RSS";
            cbWTWRSS.UseVisualStyleBackColor = true;
            cbWTWRSS.CheckedChanged += EnableDisable;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(244, 153);
            label17.Margin = new Padding(4, 0, 4, 0);
            label17.Name = "label17";
            label17.Size = new Size(88, 15);
            label17.TabIndex = 7;
            label17.Text = "days worth and";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(136, 153);
            label16.Margin = new Padding(4, 0, 4, 0);
            label16.Name = "label16";
            label16.Size = new Size(57, 15);
            label16.TabIndex = 5;
            label16.Text = "shows, or";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(7, 153);
            label15.Margin = new Padding(4, 0, 4, 0);
            label15.Name = "label15";
            label15.Size = new Size(81, 15);
            label15.TabIndex = 3;
            label15.Text = "No more than";
            // 
            // txtExportRSSMaxDays
            // 
            txtExportRSSMaxDays.Location = new Point(204, 150);
            txtExportRSSMaxDays.Margin = new Padding(4, 3, 4, 3);
            txtExportRSSMaxDays.Name = "txtExportRSSMaxDays";
            txtExportRSSMaxDays.Size = new Size(32, 23);
            txtExportRSSMaxDays.TabIndex = 6;
            txtExportRSSMaxDays.TextChanged += EnsureInteger;
            txtExportRSSMaxDays.KeyPress += TxtNumberOnlyKeyPress;
            // 
            // txtExportRSSMaxShows
            // 
            txtExportRSSMaxShows.Location = new Point(97, 150);
            txtExportRSSMaxShows.Margin = new Padding(4, 3, 4, 3);
            txtExportRSSMaxShows.Name = "txtExportRSSMaxShows";
            txtExportRSSMaxShows.Size = new Size(32, 23);
            txtExportRSSMaxShows.TabIndex = 4;
            txtExportRSSMaxShows.TextChanged += EnsureInteger;
            txtExportRSSMaxShows.KeyPress += TxtNumberOnlyKeyPress;
            // 
            // tbFilesAndFolders
            // 
            tbFilesAndFolders.Controls.Add(chkUnArchiveFilesInDownloadDirectory);
            tbFilesAndFolders.Controls.Add(cbFileNameCaseSensitiveMatch);
            tbFilesAndFolders.Controls.Add(chkUseLibraryFullPathWhenMatchingShows);
            tbFilesAndFolders.Controls.Add(label66);
            tbFilesAndFolders.Controls.Add(txtKeepTogether);
            tbFilesAndFolders.Controls.Add(txtMaxSampleSize);
            tbFilesAndFolders.Controls.Add(txtOtherExtensions);
            tbFilesAndFolders.Controls.Add(txtVideoExtensions);
            tbFilesAndFolders.Controls.Add(label39);
            tbFilesAndFolders.Controls.Add(cbKeepTogetherMode);
            tbFilesAndFolders.Controls.Add(bnReplaceRemove);
            tbFilesAndFolders.Controls.Add(bnReplaceAdd);
            tbFilesAndFolders.Controls.Add(label3);
            tbFilesAndFolders.Controls.Add(ReplacementsGrid);
            tbFilesAndFolders.Controls.Add(label19);
            tbFilesAndFolders.Controls.Add(label22);
            tbFilesAndFolders.Controls.Add(label14);
            tbFilesAndFolders.Controls.Add(cbKeepTogether);
            tbFilesAndFolders.Controls.Add(cbForceLower);
            tbFilesAndFolders.Controls.Add(cbIgnoreSamples);
            tbFilesAndFolders.Controls.Add(pbFilesAndFolders);
            tbFilesAndFolders.Location = new Point(149, 4);
            tbFilesAndFolders.Margin = new Padding(4, 3, 4, 3);
            tbFilesAndFolders.Name = "tbFilesAndFolders";
            tbFilesAndFolders.Padding = new Padding(4, 3, 4, 3);
            tbFilesAndFolders.Size = new Size(500, 684);
            tbFilesAndFolders.TabIndex = 1;
            tbFilesAndFolders.Text = "Files and Folders";
            tbFilesAndFolders.UseVisualStyleBackColor = true;
            // 
            // chkUnArchiveFilesInDownloadDirectory
            // 
            chkUnArchiveFilesInDownloadDirectory.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chkUnArchiveFilesInDownloadDirectory.AutoSize = true;
            chkUnArchiveFilesInDownloadDirectory.Location = new Point(4, 508);
            chkUnArchiveFilesInDownloadDirectory.Margin = new Padding(4, 3, 4, 3);
            chkUnArchiveFilesInDownloadDirectory.Name = "chkUnArchiveFilesInDownloadDirectory";
            chkUnArchiveFilesInDownloadDirectory.Size = new Size(236, 19);
            chkUnArchiveFilesInDownloadDirectory.TabIndex = 43;
            chkUnArchiveFilesInDownloadDirectory.Text = "Extract Archives found in Search Folders";
            chkUnArchiveFilesInDownloadDirectory.UseVisualStyleBackColor = true;
            // 
            // cbFileNameCaseSensitiveMatch
            // 
            cbFileNameCaseSensitiveMatch.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cbFileNameCaseSensitiveMatch.AutoSize = true;
            cbFileNameCaseSensitiveMatch.Location = new Point(4, 482);
            cbFileNameCaseSensitiveMatch.Margin = new Padding(4, 3, 4, 3);
            cbFileNameCaseSensitiveMatch.Name = "cbFileNameCaseSensitiveMatch";
            cbFileNameCaseSensitiveMatch.Size = new Size(211, 19);
            cbFileNameCaseSensitiveMatch.TabIndex = 42;
            cbFileNameCaseSensitiveMatch.Text = "Case Sensitive Match for Filenames";
            cbFileNameCaseSensitiveMatch.UseVisualStyleBackColor = true;
            // 
            // chkUseLibraryFullPathWhenMatchingShows
            // 
            chkUseLibraryFullPathWhenMatchingShows.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chkUseLibraryFullPathWhenMatchingShows.AutoSize = true;
            chkUseLibraryFullPathWhenMatchingShows.Location = new Point(4, 455);
            chkUseLibraryFullPathWhenMatchingShows.Margin = new Padding(4, 3, 4, 3);
            chkUseLibraryFullPathWhenMatchingShows.Name = "chkUseLibraryFullPathWhenMatchingShows";
            chkUseLibraryFullPathWhenMatchingShows.Size = new Size(469, 19);
            chkUseLibraryFullPathWhenMatchingShows.TabIndex = 41;
            chkUseLibraryFullPathWhenMatchingShows.Text = "Use name of Library Folder when searching for a match between a file and a tv show";
            chkUseLibraryFullPathWhenMatchingShows.UseVisualStyleBackColor = true;
            // 
            // label66
            // 
            label66.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label66.AutoSize = true;
            label66.Location = new Point(7, 8);
            label66.Margin = new Padding(4, 0, 4, 0);
            label66.Name = "label66";
            label66.Size = new Size(358, 45);
            label66.TabIndex = 39;
            label66.Text = "These preferences control how TV Rename copies files across from\r\nyour search folders to your library. Often there are other files that\r\nyou'd like copied as well.";
            // 
            // txtMaxSampleSize
            // 
            txtMaxSampleSize.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtMaxSampleSize.Location = new Point(197, 399);
            txtMaxSampleSize.Margin = new Padding(4, 3, 4, 3);
            txtMaxSampleSize.Name = "txtMaxSampleSize";
            txtMaxSampleSize.Size = new Size(61, 23);
            txtMaxSampleSize.TabIndex = 14;
            txtMaxSampleSize.TextChanged += EnsureInteger;
            txtMaxSampleSize.KeyPress += TxtNumberOnlyKeyPress;
            // 
            // txtVideoExtensions
            // 
            txtVideoExtensions.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtVideoExtensions.Location = new Point(115, 269);
            txtVideoExtensions.Margin = new Padding(4, 3, 4, 3);
            txtVideoExtensions.Name = "txtVideoExtensions";
            txtVideoExtensions.Size = new Size(348, 23);
            txtVideoExtensions.TabIndex = 5;
            // 
            // label39
            // 
            label39.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label39.AutoSize = true;
            label39.Location = new Point(29, 363);
            label39.Margin = new Padding(4, 0, 4, 0);
            label39.Name = "label39";
            label39.Size = new Size(22, 15);
            label39.TabIndex = 22;
            label39.Text = "Do";
            // 
            // cbKeepTogetherMode
            // 
            cbKeepTogetherMode.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cbKeepTogetherMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cbKeepTogetherMode.FormattingEnabled = true;
            cbKeepTogetherMode.Items.AddRange(new object[] { "All", "All but these", "Just" });
            cbKeepTogetherMode.Location = new Point(61, 360);
            cbKeepTogetherMode.Margin = new Padding(4, 3, 4, 3);
            cbKeepTogetherMode.Name = "cbKeepTogetherMode";
            cbKeepTogetherMode.Size = new Size(170, 23);
            cbKeepTogetherMode.Sorted = true;
            cbKeepTogetherMode.TabIndex = 21;
            cbKeepTogetherMode.SelectedIndexChanged += EnableDisable;
            // 
            // bnReplaceRemove
            // 
            bnReplaceRemove.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnReplaceRemove.Location = new Point(105, 226);
            bnReplaceRemove.Margin = new Padding(4, 3, 4, 3);
            bnReplaceRemove.Name = "bnReplaceRemove";
            bnReplaceRemove.Size = new Size(88, 27);
            bnReplaceRemove.TabIndex = 3;
            bnReplaceRemove.Text = "&Remove";
            bnReplaceRemove.UseVisualStyleBackColor = true;
            bnReplaceRemove.Click += bnReplaceRemove_Click;
            // 
            // bnReplaceAdd
            // 
            bnReplaceAdd.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnReplaceAdd.Location = new Point(10, 226);
            bnReplaceAdd.Margin = new Padding(4, 3, 4, 3);
            bnReplaceAdd.Name = "bnReplaceAdd";
            bnReplaceAdd.Size = new Size(88, 27);
            bnReplaceAdd.TabIndex = 2;
            bnReplaceAdd.Text = "&Add";
            bnReplaceAdd.UseVisualStyleBackColor = true;
            bnReplaceAdd.Click += bnReplaceAdd_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(7, 69);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(132, 15);
            label3.TabIndex = 0;
            label3.Text = "Filename Replacements";
            // 
            // ReplacementsGrid
            // 
            ReplacementsGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ReplacementsGrid.BackColor = SystemColors.Window;
            ReplacementsGrid.EnableSort = true;
            ReplacementsGrid.Location = new Point(7, 88);
            ReplacementsGrid.Margin = new Padding(4, 3, 4, 3);
            ReplacementsGrid.Name = "ReplacementsGrid";
            ReplacementsGrid.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            ReplacementsGrid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            ReplacementsGrid.Size = new Size(457, 132);
            ReplacementsGrid.TabIndex = 1;
            ReplacementsGrid.TabStop = true;
            ReplacementsGrid.ToolTipText = "";
            // 
            // label19
            // 
            label19.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label19.AutoSize = true;
            label19.Location = new Point(262, 403);
            label19.Margin = new Padding(4, 0, 4, 0);
            label19.Name = "label19";
            label19.Size = new Size(60, 15);
            label19.TabIndex = 15;
            label19.Text = "MB in size";
            // 
            // label22
            // 
            label22.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label22.AutoSize = true;
            label22.Location = new Point(4, 302);
            label22.Margin = new Padding(4, 0, 4, 0);
            label22.Name = "label22";
            label22.Size = new Size(98, 15);
            label22.TabIndex = 6;
            label22.Text = "&Other extensions:";
            // 
            // label14
            // 
            label14.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label14.AutoSize = true;
            label14.Location = new Point(4, 272);
            label14.Margin = new Padding(4, 0, 4, 0);
            label14.Name = "label14";
            label14.Size = new Size(98, 15);
            label14.TabIndex = 4;
            label14.Text = "&Video extensions:";
            // 
            // cbKeepTogether
            // 
            cbKeepTogether.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cbKeepTogether.AutoSize = true;
            cbKeepTogether.Location = new Point(7, 330);
            cbKeepTogether.Margin = new Padding(4, 3, 4, 3);
            cbKeepTogether.Name = "cbKeepTogether";
            cbKeepTogether.Size = new Size(384, 19);
            cbKeepTogether.TabIndex = 8;
            cbKeepTogether.Text = "&Copy/Move files with same base name as video from Search Folders";
            cbKeepTogether.UseVisualStyleBackColor = true;
            cbKeepTogether.CheckedChanged += EnableDisable;
            // 
            // cbForceLower
            // 
            cbForceLower.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cbForceLower.AutoSize = true;
            cbForceLower.Location = new Point(4, 429);
            cbForceLower.Margin = new Padding(4, 3, 4, 3);
            cbForceLower.Name = "cbForceLower";
            cbForceLower.Size = new Size(182, 19);
            cbForceLower.TabIndex = 16;
            cbForceLower.Text = "&Make all filenames lower case";
            cbForceLower.UseVisualStyleBackColor = true;
            // 
            // cbIgnoreSamples
            // 
            cbIgnoreSamples.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cbIgnoreSamples.AutoSize = true;
            cbIgnoreSamples.Location = new Point(4, 402);
            cbIgnoreSamples.Margin = new Padding(4, 3, 4, 3);
            cbIgnoreSamples.Name = "cbIgnoreSamples";
            cbIgnoreSamples.Size = new Size(182, 19);
            cbIgnoreSamples.TabIndex = 13;
            cbIgnoreSamples.Text = "&Ignore \"sample\" videos, up to";
            cbIgnoreSamples.UseVisualStyleBackColor = true;
            // 
            // pbFilesAndFolders
            // 
            pbFilesAndFolders.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbFilesAndFolders.Cursor = Cursors.Hand;
            pbFilesAndFolders.Image = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbFilesAndFolders.InitialImage = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbFilesAndFolders.Location = new Point(418, 7);
            pbFilesAndFolders.Margin = new Padding(4, 3, 4, 3);
            pbFilesAndFolders.Name = "pbFilesAndFolders";
            pbFilesAndFolders.Size = new Size(50, 46);
            pbFilesAndFolders.SizeMode = PictureBoxSizeMode.CenterImage;
            pbFilesAndFolders.TabIndex = 33;
            pbFilesAndFolders.TabStop = false;
            pbFilesAndFolders.Click += pbFilesAndFolders_Click;
            // 
            // tbGeneral
            // 
            tbGeneral.Controls.Add(cbAutoSaveOnExit);
            tbGeneral.Controls.Add(chkAutoAddAsPartOfQuickRename);
            tbGeneral.Controls.Add(chkShareCriticalLogs);
            tbGeneral.Controls.Add(label60);
            tbGeneral.Controls.Add(pbGeneral);
            tbGeneral.Controls.Add(txtWTWDays);
            tbGeneral.Controls.Add(cbMode);
            tbGeneral.Controls.Add(label34);
            tbGeneral.Controls.Add(label2);
            tbGeneral.Location = new Point(149, 4);
            tbGeneral.Margin = new Padding(4, 3, 4, 3);
            tbGeneral.Name = "tbGeneral";
            tbGeneral.Padding = new Padding(4, 3, 4, 3);
            tbGeneral.Size = new Size(500, 684);
            tbGeneral.TabIndex = 0;
            tbGeneral.Text = "General";
            tbGeneral.UseVisualStyleBackColor = true;
            // 
            // chkAutoAddAsPartOfQuickRename
            // 
            chkAutoAddAsPartOfQuickRename.AutoSize = true;
            chkAutoAddAsPartOfQuickRename.Location = new Point(15, 147);
            chkAutoAddAsPartOfQuickRename.Margin = new Padding(4, 3, 4, 3);
            chkAutoAddAsPartOfQuickRename.Name = "chkAutoAddAsPartOfQuickRename";
            chkAutoAddAsPartOfQuickRename.Size = new Size(211, 19);
            chkAutoAddAsPartOfQuickRename.TabIndex = 43;
            chkAutoAddAsPartOfQuickRename.Text = "Auto-Add as part of Quick Rename";
            chkAutoAddAsPartOfQuickRename.UseVisualStyleBackColor = true;
            // 
            // chkShareCriticalLogs
            // 
            chkShareCriticalLogs.AutoSize = true;
            chkShareCriticalLogs.Location = new Point(15, 120);
            chkShareCriticalLogs.Margin = new Padding(4, 3, 4, 3);
            chkShareCriticalLogs.Name = "chkShareCriticalLogs";
            chkShareCriticalLogs.Size = new Size(231, 19);
            chkShareCriticalLogs.TabIndex = 42;
            chkShareCriticalLogs.Text = "Share critical errors to help defeat bugs";
            chkShareCriticalLogs.UseVisualStyleBackColor = true;
            // 
            // label60
            // 
            label60.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label60.AutoSize = true;
            label60.Location = new Point(7, 7);
            label60.Margin = new Padding(4, 0, 4, 0);
            label60.Name = "label60";
            label60.Size = new Size(323, 30);
            label60.TabIndex = 38;
            label60.Text = "General settings to control TV Rename's scan and download\r\nbehaviour";
            // 
            // pbGeneral
            // 
            pbGeneral.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbGeneral.Cursor = Cursors.Hand;
            pbGeneral.Image = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbGeneral.InitialImage = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbGeneral.Location = new Point(418, 7);
            pbGeneral.Margin = new Padding(4, 3, 4, 3);
            pbGeneral.Name = "pbGeneral";
            pbGeneral.Size = new Size(50, 46);
            pbGeneral.SizeMode = PictureBoxSizeMode.CenterImage;
            pbGeneral.TabIndex = 23;
            pbGeneral.TabStop = false;
            pbGeneral.Click += pbGeneral_Click;
            // 
            // txtWTWDays
            // 
            txtWTWDays.Location = new Point(15, 59);
            txtWTWDays.Margin = new Padding(4, 3, 4, 3);
            txtWTWDays.Name = "txtWTWDays";
            txtWTWDays.Size = new Size(32, 23);
            txtWTWDays.TabIndex = 1;
            txtWTWDays.TextChanged += EnsureInteger;
            txtWTWDays.KeyPress += TxtNumberOnlyKeyPress;
            // 
            // cbMode
            // 
            cbMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMode.FormattingEnabled = true;
            cbMode.Items.AddRange(new object[] { "Beta", "Production" });
            cbMode.Location = new Point(136, 89);
            cbMode.Margin = new Padding(4, 3, 4, 3);
            cbMode.Name = "cbMode";
            cbMode.Size = new Size(170, 23);
            cbMode.Sorted = true;
            cbMode.TabIndex = 19;
            // 
            // label34
            // 
            label34.AutoSize = true;
            label34.Location = new Point(13, 92);
            label34.Margin = new Padding(4, 0, 4, 0);
            label34.Name = "label34";
            label34.Size = new Size(41, 15);
            label34.TabIndex = 18;
            label34.Text = "&Mode:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(55, 62);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(120, 15);
            label2.TabIndex = 2;
            label2.Text = "days counts as recent";
            // 
            // tcTabs
            // 
            tcTabs.Alignment = TabAlignment.Left;
            tcTabs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tcTabs.Controls.Add(tbGeneral);
            tcTabs.Controls.Add(tpDisplay);
            tcTabs.Controls.Add(tpDataSources);
            tcTabs.Controls.Add(tpLibraryFolders);
            tcTabs.Controls.Add(tpMovieDefaults);
            tcTabs.Controls.Add(tpShowDefaults);
            tcTabs.Controls.Add(tpScanSettings);
            tcTabs.Controls.Add(tbFilesAndFolders);
            tcTabs.Controls.Add(tpSubtitles);
            tcTabs.Controls.Add(tbSearchFolders);
            tcTabs.Controls.Add(tbFolderDeleting);
            tcTabs.Controls.Add(tbMediaCenter);
            tcTabs.Controls.Add(tpTorrentNZB);
            tcTabs.Controls.Add(tpRSSJSONSearch);
            tcTabs.Controls.Add(tpJackett);
            tcTabs.Controls.Add(tbAutoExport);
            tcTabs.Controls.Add(tpAutoExportLibrary);
            tcTabs.Controls.Add(tbAppUpdate);
            tcTabs.DrawMode = TabDrawMode.OwnerDrawFixed;
            tcTabs.ItemSize = new Size(30, 145);
            tcTabs.Location = new Point(14, 14);
            tcTabs.Margin = new Padding(4, 3, 4, 3);
            tcTabs.Multiline = true;
            tcTabs.Name = "tcTabs";
            tcTabs.SelectedIndex = 0;
            tcTabs.Size = new Size(653, 692);
            tcTabs.SizeMode = TabSizeMode.Fixed;
            tcTabs.TabIndex = 0;
            tcTabs.DrawItem += tpSearch_DrawItem;
            // 
            // tpDataSources
            // 
            tpDataSources.Controls.Add(panel4);
            tpDataSources.Controls.Add(panel3);
            tpDataSources.Controls.Add(label100);
            tpDataSources.Controls.Add(panel2);
            tpDataSources.Controls.Add(label83);
            tpDataSources.Controls.Add(gbTMDB);
            tpDataSources.Controls.Add(label63);
            tpDataSources.Controls.Add(label57);
            tpDataSources.Controls.Add(label40);
            tpDataSources.Controls.Add(domainUpDown2);
            tpDataSources.Controls.Add(txtParallelDownloads);
            tpDataSources.Controls.Add(label21);
            tpDataSources.Controls.Add(label20);
            tpDataSources.Controls.Add(groupBox21);
            tpDataSources.Controls.Add(groupBox20);
            tpDataSources.Controls.Add(label33);
            tpDataSources.Controls.Add(pbSources);
            tpDataSources.Location = new Point(149, 4);
            tpDataSources.Margin = new Padding(4, 3, 4, 3);
            tpDataSources.Name = "tpDataSources";
            tpDataSources.Size = new Size(500, 684);
            tpDataSources.TabIndex = 15;
            tpDataSources.Text = "Data Sources";
            tpDataSources.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            panel4.Controls.Add(rdoGlobalReleaseDates);
            panel4.Controls.Add(rdoRegionalReleaseDates);
            panel4.Location = new Point(150, 199);
            panel4.Margin = new Padding(4, 3, 4, 3);
            panel4.Name = "panel4";
            panel4.Size = new Size(303, 33);
            panel4.TabIndex = 65;
            // 
            // rdoGlobalReleaseDates
            // 
            rdoGlobalReleaseDates.AutoSize = true;
            rdoGlobalReleaseDates.Location = new Point(83, 5);
            rdoGlobalReleaseDates.Margin = new Padding(4, 3, 4, 3);
            rdoGlobalReleaseDates.Name = "rdoGlobalReleaseDates";
            rdoGlobalReleaseDates.Size = new Size(59, 19);
            rdoGlobalReleaseDates.TabIndex = 60;
            rdoGlobalReleaseDates.TabStop = true;
            rdoGlobalReleaseDates.Text = "Global";
            rdoGlobalReleaseDates.UseVisualStyleBackColor = true;
            // 
            // rdoRegionalReleaseDates
            // 
            rdoRegionalReleaseDates.AutoSize = true;
            rdoRegionalReleaseDates.Location = new Point(4, 5);
            rdoRegionalReleaseDates.Margin = new Padding(4, 3, 4, 3);
            rdoRegionalReleaseDates.Name = "rdoRegionalReleaseDates";
            rdoRegionalReleaseDates.Size = new Size(71, 19);
            rdoRegionalReleaseDates.TabIndex = 59;
            rdoRegionalReleaseDates.TabStop = true;
            rdoRegionalReleaseDates.Text = "Regional";
            rdoRegionalReleaseDates.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            panel3.Controls.Add(rdoMovieTMDB);
            panel3.Controls.Add(rdoMovieTheTVDB);
            panel3.Location = new Point(150, 104);
            panel3.Margin = new Padding(4, 3, 4, 3);
            panel3.Name = "panel3";
            panel3.Size = new Size(303, 33);
            panel3.TabIndex = 63;
            // 
            // rdoMovieTMDB
            // 
            rdoMovieTMDB.AutoSize = true;
            rdoMovieTMDB.Location = new Point(202, 5);
            rdoMovieTMDB.Margin = new Padding(4, 3, 4, 3);
            rdoMovieTMDB.Name = "rdoMovieTMDB";
            rdoMovieTMDB.Size = new Size(58, 19);
            rdoMovieTMDB.TabIndex = 60;
            rdoMovieTMDB.TabStop = true;
            rdoMovieTMDB.Text = "TMDB";
            rdoMovieTMDB.UseVisualStyleBackColor = true;
            // 
            // rdoMovieTheTVDB
            // 
            rdoMovieTheTVDB.AutoSize = true;
            rdoMovieTheTVDB.Location = new Point(4, 5);
            rdoMovieTheTVDB.Margin = new Padding(4, 3, 4, 3);
            rdoMovieTheTVDB.Name = "rdoMovieTheTVDB";
            rdoMovieTheTVDB.Size = new Size(77, 19);
            rdoMovieTheTVDB.TabIndex = 59;
            rdoMovieTheTVDB.TabStop = true;
            rdoMovieTheTVDB.Text = "The TVDB";
            rdoMovieTheTVDB.UseVisualStyleBackColor = true;
            // 
            // label100
            // 
            label100.AutoSize = true;
            label100.Location = new Point(10, 203);
            label100.Margin = new Padding(4, 0, 4, 0);
            label100.Name = "label100";
            label100.Size = new Size(104, 15);
            label100.TabIndex = 64;
            label100.Text = "Release Date Type:";
            label100.TextAlign = ContentAlignment.TopRight;
            // 
            // panel2
            // 
            panel2.Controls.Add(rdoTVTMDB);
            panel2.Controls.Add(rdoTVTVMaze);
            panel2.Controls.Add(rdoTVTVDB);
            panel2.Location = new Point(150, 68);
            panel2.Margin = new Padding(4, 3, 4, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(303, 33);
            panel2.TabIndex = 62;
            // 
            // rdoTVTMDB
            // 
            rdoTVTMDB.AutoSize = true;
            rdoTVTMDB.Location = new Point(202, 5);
            rdoTVTMDB.Margin = new Padding(4, 3, 4, 3);
            rdoTVTMDB.Name = "rdoTVTMDB";
            rdoTVTMDB.Size = new Size(58, 19);
            rdoTVTMDB.TabIndex = 61;
            rdoTVTMDB.TabStop = true;
            rdoTVTMDB.Text = "TMDB";
            rdoTVTMDB.UseVisualStyleBackColor = true;
            // 
            // rdoTVTVMaze
            // 
            rdoTVTVMaze.AutoSize = true;
            rdoTVTVMaze.Location = new Point(105, 3);
            rdoTVTVMaze.Margin = new Padding(4, 3, 4, 3);
            rdoTVTVMaze.Name = "rdoTVTVMaze";
            rdoTVTVMaze.Size = new Size(70, 19);
            rdoTVTVMaze.TabIndex = 46;
            rdoTVTVMaze.TabStop = true;
            rdoTVTVMaze.Text = "TV Maze";
            rdoTVTVMaze.UseVisualStyleBackColor = true;
            // 
            // rdoTVTVDB
            // 
            rdoTVTVDB.AutoSize = true;
            rdoTVTVDB.Location = new Point(4, 3);
            rdoTVTVDB.Margin = new Padding(4, 3, 4, 3);
            rdoTVTVDB.Name = "rdoTVTVDB";
            rdoTVTVDB.Size = new Size(77, 19);
            rdoTVTVDB.TabIndex = 45;
            rdoTVTVDB.TabStop = true;
            rdoTVTVDB.Text = "The TVDB";
            rdoTVTVDB.UseVisualStyleBackColor = true;
            // 
            // label83
            // 
            label83.AutoSize = true;
            label83.Location = new Point(10, 108);
            label83.Margin = new Padding(4, 0, 4, 0);
            label83.Name = "label83";
            label83.Size = new Size(123, 15);
            label83.TabIndex = 61;
            label83.Text = "Default Movie Source:";
            label83.TextAlign = ContentAlignment.TopRight;
            // 
            // gbTMDB
            // 
            gbTMDB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbTMDB.Controls.Add(label84);
            gbTMDB.Controls.Add(cbTMDBRegions);
            gbTMDB.Controls.Add(label80);
            gbTMDB.Controls.Add(label81);
            gbTMDB.Controls.Add(tbTMDBPercentDirty);
            gbTMDB.Controls.Add(label82);
            gbTMDB.Controls.Add(cbTMDBLanguages);
            gbTMDB.Location = new Point(14, 479);
            gbTMDB.Margin = new Padding(4, 3, 4, 3);
            gbTMDB.Name = "gbTMDB";
            gbTMDB.Padding = new Padding(4, 3, 4, 3);
            gbTMDB.Size = new Size(448, 115);
            gbTMDB.TabIndex = 58;
            gbTMDB.TabStop = false;
            gbTMDB.Text = "TMDB";
            // 
            // cbTMDBRegions
            // 
            cbTMDBRegions.DropDownStyle = ComboBoxStyle.DropDownList;
            cbTMDBRegions.FormattingEnabled = true;
            cbTMDBRegions.Location = new Point(135, 52);
            cbTMDBRegions.Margin = new Padding(4, 3, 4, 3);
            cbTMDBRegions.Name = "cbTMDBRegions";
            cbTMDBRegions.Size = new Size(170, 23);
            cbTMDBRegions.Sorted = true;
            cbTMDBRegions.TabIndex = 27;
            // 
            // label80
            // 
            label80.AutoSize = true;
            label80.Location = new Point(12, 87);
            label80.Margin = new Padding(4, 0, 4, 0);
            label80.Name = "label80";
            label80.Size = new Size(124, 15);
            label80.TabIndex = 23;
            label80.Text = "Refresh entire series  if";
            // 
            // label81
            // 
            label81.AutoSize = true;
            label81.Location = new Point(189, 87);
            label81.Margin = new Padding(4, 0, 4, 0);
            label81.Name = "label81";
            label81.Size = new Size(146, 15);
            label81.TabIndex = 25;
            label81.Text = "% of episodes are updated";
            // 
            // tbTMDBPercentDirty
            // 
            tbTMDBPercentDirty.Location = new Point(148, 83);
            tbTMDBPercentDirty.Margin = new Padding(4, 3, 4, 3);
            tbTMDBPercentDirty.Name = "tbTMDBPercentDirty";
            tbTMDBPercentDirty.Size = new Size(32, 23);
            tbTMDBPercentDirty.TabIndex = 24;
            // 
            // label82
            // 
            label82.AutoSize = true;
            label82.Location = new Point(13, 25);
            label82.Margin = new Padding(4, 0, 4, 0);
            label82.Name = "label82";
            label82.Size = new Size(110, 15);
            label82.TabIndex = 18;
            label82.Text = "&Preferred language:";
            // 
            // cbTMDBLanguages
            // 
            cbTMDBLanguages.DropDownStyle = ComboBoxStyle.DropDownList;
            cbTMDBLanguages.FormattingEnabled = true;
            cbTMDBLanguages.Location = new Point(136, 22);
            cbTMDBLanguages.Margin = new Padding(4, 3, 4, 3);
            cbTMDBLanguages.Name = "cbTMDBLanguages";
            cbTMDBLanguages.Size = new Size(170, 23);
            cbTMDBLanguages.Sorted = true;
            cbTMDBLanguages.TabIndex = 19;
            // 
            // label63
            // 
            label63.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label63.AutoSize = true;
            label63.Location = new Point(10, 3);
            label63.Margin = new Padding(4, 0, 4, 0);
            label63.Name = "label63";
            label63.Size = new Size(396, 30);
            label63.TabIndex = 57;
            label63.Text = "TV Rename downloads information from upstream sources to understand\r\nwhich shows have episodes";
            // 
            // label57
            // 
            label57.AutoSize = true;
            label57.Location = new Point(10, 172);
            label57.Margin = new Padding(4, 0, 4, 0);
            label57.Name = "label57";
            label57.Size = new Size(110, 15);
            label57.TabIndex = 55;
            label57.Text = "Update cache every";
            // 
            // txtParallelDownloads
            // 
            txtParallelDownloads.Location = new Point(111, 137);
            txtParallelDownloads.Margin = new Padding(4, 3, 4, 3);
            txtParallelDownloads.Name = "txtParallelDownloads";
            txtParallelDownloads.Size = new Size(32, 23);
            txtParallelDownloads.TabIndex = 51;
            txtParallelDownloads.TextChanged += EnsureInteger;
            txtParallelDownloads.KeyPress += TxtNumberOnlyKeyPress;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new Point(10, 141);
            label21.Margin = new Padding(4, 0, 4, 0);
            label21.Name = "label21";
            label21.Size = new Size(92, 15);
            label21.TabIndex = 50;
            label21.Text = "&Download up to";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new Point(150, 141);
            label20.Margin = new Padding(4, 0, 4, 0);
            label20.Name = "label20";
            label20.Size = new Size(166, 15);
            label20.TabIndex = 52;
            label20.Text = "shows/images simultaneously";
            // 
            // groupBox21
            // 
            groupBox21.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox21.Location = new Point(14, 357);
            groupBox21.Margin = new Padding(4, 3, 4, 3);
            groupBox21.Name = "groupBox21";
            groupBox21.Padding = new Padding(4, 3, 4, 3);
            groupBox21.Size = new Size(448, 115);
            groupBox21.TabIndex = 49;
            groupBox21.TabStop = false;
            groupBox21.Text = "TV Maze";
            // 
            // groupBox20
            // 
            groupBox20.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox20.Controls.Add(label91);
            groupBox20.Controls.Add(cbTVDBVersion);
            groupBox20.Controls.Add(label37);
            groupBox20.Controls.Add(label38);
            groupBox20.Controls.Add(tbPercentDirty);
            groupBox20.Controls.Add(label10);
            groupBox20.Controls.Add(cbTVDBLanguages);
            groupBox20.Location = new Point(14, 234);
            groupBox20.Margin = new Padding(4, 3, 4, 3);
            groupBox20.Name = "groupBox20";
            groupBox20.Padding = new Padding(4, 3, 4, 3);
            groupBox20.Size = new Size(448, 115);
            groupBox20.TabIndex = 48;
            groupBox20.TabStop = false;
            groupBox20.Text = "TheTVDB";
            // 
            // label91
            // 
            label91.AutoSize = true;
            label91.Location = new Point(12, 87);
            label91.Margin = new Padding(4, 0, 4, 0);
            label91.Name = "label91";
            label91.Size = new Size(48, 15);
            label91.TabIndex = 26;
            label91.Text = "Version:";
            // 
            // cbTVDBVersion
            // 
            cbTVDBVersion.DropDownStyle = ComboBoxStyle.DropDownList;
            cbTVDBVersion.FormattingEnabled = true;
            cbTVDBVersion.Items.AddRange(new object[] { "v4" });
            cbTVDBVersion.Location = new Point(135, 83);
            cbTVDBVersion.Margin = new Padding(4, 3, 4, 3);
            cbTVDBVersion.Name = "cbTVDBVersion";
            cbTVDBVersion.Size = new Size(170, 23);
            cbTVDBVersion.Sorted = true;
            cbTVDBVersion.TabIndex = 27;
            // 
            // label37
            // 
            label37.AutoSize = true;
            label37.Location = new Point(12, 57);
            label37.Margin = new Padding(4, 0, 4, 0);
            label37.Name = "label37";
            label37.Size = new Size(124, 15);
            label37.TabIndex = 23;
            label37.Text = "Refresh entire series  if";
            // 
            // label38
            // 
            label38.AutoSize = true;
            label38.Location = new Point(189, 57);
            label38.Margin = new Padding(4, 0, 4, 0);
            label38.Name = "label38";
            label38.Size = new Size(146, 15);
            label38.TabIndex = 25;
            label38.Text = "% of episodes are updated";
            // 
            // tbPercentDirty
            // 
            tbPercentDirty.Location = new Point(148, 53);
            tbPercentDirty.Margin = new Padding(4, 3, 4, 3);
            tbPercentDirty.Name = "tbPercentDirty";
            tbPercentDirty.Size = new Size(32, 23);
            tbPercentDirty.TabIndex = 24;
            tbPercentDirty.TextChanged += EnsureInteger;
            tbPercentDirty.KeyPress += TxtNumberOnlyKeyPress;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(13, 25);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(110, 15);
            label10.TabIndex = 18;
            label10.Text = "&Preferred language:";
            // 
            // cbTVDBLanguages
            // 
            cbTVDBLanguages.DropDownStyle = ComboBoxStyle.DropDownList;
            cbTVDBLanguages.FormattingEnabled = true;
            cbTVDBLanguages.Items.AddRange(new object[] { "My Shows", "Scan", "Schedule" });
            cbTVDBLanguages.Location = new Point(136, 22);
            cbTVDBLanguages.Margin = new Padding(4, 3, 4, 3);
            cbTVDBLanguages.Name = "cbTVDBLanguages";
            cbTVDBLanguages.Size = new Size(170, 23);
            cbTVDBLanguages.Sorted = true;
            cbTVDBLanguages.TabIndex = 19;
            // 
            // label33
            // 
            label33.AutoSize = true;
            label33.Location = new Point(10, 75);
            label33.Margin = new Padding(4, 0, 4, 0);
            label33.Name = "label33";
            label33.Size = new Size(136, 15);
            label33.TabIndex = 47;
            label33.Text = "Default TV Show Source:";
            label33.TextAlign = ContentAlignment.TopRight;
            // 
            // pbSources
            // 
            pbSources.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbSources.Cursor = Cursors.Hand;
            pbSources.Image = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbSources.InitialImage = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbSources.Location = new Point(421, 3);
            pbSources.Margin = new Padding(4, 3, 4, 3);
            pbSources.Name = "pbSources";
            pbSources.Size = new Size(50, 46);
            pbSources.SizeMode = PictureBoxSizeMode.CenterImage;
            pbSources.TabIndex = 56;
            pbSources.TabStop = false;
            pbSources.Click += pbSources_Click;
            // 
            // tpMovieDefaults
            // 
            tpMovieDefaults.Controls.Add(label86);
            tpMovieDefaults.Controls.Add(groupBox24);
            tpMovieDefaults.Controls.Add(groupBox25);
            tpMovieDefaults.Controls.Add(pbMovieDefaults);
            tpMovieDefaults.Location = new Point(149, 4);
            tpMovieDefaults.Margin = new Padding(4, 3, 4, 3);
            tpMovieDefaults.Name = "tpMovieDefaults";
            tpMovieDefaults.Padding = new Padding(4, 3, 4, 3);
            tpMovieDefaults.Size = new Size(500, 684);
            tpMovieDefaults.TabIndex = 18;
            tpMovieDefaults.Text = "Movies Defaults";
            tpMovieDefaults.UseVisualStyleBackColor = true;
            // 
            // label86
            // 
            label86.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label86.AutoSize = true;
            label86.Location = new Point(8, 13);
            label86.Margin = new Padding(4, 0, 4, 0);
            label86.Name = "label86";
            label86.Size = new Size(360, 45);
            label86.TabIndex = 63;
            label86.Text = "These settings control the defaults used to add a new movie to the \r\nsystem. Once the show has been created, you will need to modify\r\nits configuration directly.";
            // 
            // groupBox24
            // 
            groupBox24.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox24.Controls.Add(cmbDefMovieFolderFormat);
            groupBox24.Controls.Add(label95);
            groupBox24.Controls.Add(cmbDefMovieLocation);
            groupBox24.Controls.Add(cbDefMovieUseDefLocation);
            groupBox24.Controls.Add(cbDefMovieAutoFolders);
            groupBox24.Location = new Point(7, 68);
            groupBox24.Margin = new Padding(4, 3, 4, 3);
            groupBox24.Name = "groupBox24";
            groupBox24.Padding = new Padding(4, 3, 4, 3);
            groupBox24.Size = new Size(456, 155);
            groupBox24.TabIndex = 61;
            groupBox24.TabStop = false;
            groupBox24.Text = "Default Movie Settings";
            // 
            // cmbDefMovieFolderFormat
            // 
            cmbDefMovieFolderFormat.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbDefMovieFolderFormat.FormattingEnabled = true;
            cmbDefMovieFolderFormat.Location = new Point(162, 114);
            cmbDefMovieFolderFormat.Margin = new Padding(4, 3, 4, 3);
            cmbDefMovieFolderFormat.Name = "cmbDefMovieFolderFormat";
            cmbDefMovieFolderFormat.Size = new Size(286, 23);
            cmbDefMovieFolderFormat.TabIndex = 7;
            // 
            // label95
            // 
            label95.AutoSize = true;
            label95.Location = new Point(7, 118);
            label95.Margin = new Padding(4, 0, 4, 0);
            label95.Name = "label95";
            label95.Size = new Size(144, 15);
            label95.TabIndex = 6;
            label95.Text = "Default movie folder type:";
            // 
            // cmbDefMovieLocation
            // 
            cmbDefMovieLocation.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbDefMovieLocation.FormattingEnabled = true;
            cmbDefMovieLocation.Location = new Point(34, 78);
            cmbDefMovieLocation.Margin = new Padding(4, 3, 4, 3);
            cmbDefMovieLocation.Name = "cmbDefMovieLocation";
            cmbDefMovieLocation.Size = new Size(415, 23);
            cmbDefMovieLocation.TabIndex = 2;
            // 
            // cbDefMovieUseDefLocation
            // 
            cbDefMovieUseDefLocation.AutoSize = true;
            cbDefMovieUseDefLocation.Location = new Point(10, 50);
            cbDefMovieUseDefLocation.Margin = new Padding(4, 3, 4, 3);
            cbDefMovieUseDefLocation.Name = "cbDefMovieUseDefLocation";
            cbDefMovieUseDefLocation.Size = new Size(135, 19);
            cbDefMovieUseDefLocation.TabIndex = 1;
            cbDefMovieUseDefLocation.Text = "Use Default Location";
            cbDefMovieUseDefLocation.UseVisualStyleBackColor = true;
            // 
            // cbDefMovieAutoFolders
            // 
            cbDefMovieAutoFolders.AutoSize = true;
            cbDefMovieAutoFolders.Location = new Point(10, 23);
            cbDefMovieAutoFolders.Margin = new Padding(4, 3, 4, 3);
            cbDefMovieAutoFolders.Name = "cbDefMovieAutoFolders";
            cbDefMovieAutoFolders.Size = new Size(145, 19);
            cbDefMovieAutoFolders.TabIndex = 0;
            cbDefMovieAutoFolders.Text = "Use Automatic Folders";
            cbDefMovieAutoFolders.UseVisualStyleBackColor = true;
            // 
            // groupBox25
            // 
            groupBox25.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox25.Controls.Add(cbDefMovieIncludeNoAirdate);
            groupBox25.Controls.Add(cbDefMovieIncludeFuture);
            groupBox25.Controls.Add(cbDefMovieDoMissing);
            groupBox25.Controls.Add(cbDefMovieDoRenaming);
            groupBox25.Location = new Point(7, 230);
            groupBox25.Margin = new Padding(4, 3, 4, 3);
            groupBox25.Name = "groupBox25";
            groupBox25.Padding = new Padding(4, 3, 4, 3);
            groupBox25.Size = new Size(456, 134);
            groupBox25.TabIndex = 60;
            groupBox25.TabStop = false;
            groupBox25.Text = "Default Advanced Settings";
            // 
            // cbDefMovieIncludeNoAirdate
            // 
            cbDefMovieIncludeNoAirdate.AutoSize = true;
            cbDefMovieIncludeNoAirdate.Location = new Point(29, 102);
            cbDefMovieIncludeNoAirdate.Margin = new Padding(4, 3, 4, 3);
            cbDefMovieIncludeNoAirdate.Name = "cbDefMovieIncludeNoAirdate";
            cbDefMovieIncludeNoAirdate.Size = new Size(270, 19);
            cbDefMovieIncludeNoAirdate.TabIndex = 53;
            cbDefMovieIncludeNoAirdate.Text = "Include check when movie has no release date";
            cbDefMovieIncludeNoAirdate.UseVisualStyleBackColor = true;
            // 
            // cbDefMovieIncludeFuture
            // 
            cbDefMovieIncludeFuture.AutoSize = true;
            cbDefMovieIncludeFuture.Location = new Point(29, 75);
            cbDefMovieIncludeFuture.Margin = new Padding(4, 3, 4, 3);
            cbDefMovieIncludeFuture.Name = "cbDefMovieIncludeFuture";
            cbDefMovieIncludeFuture.Size = new Size(297, 19);
            cbDefMovieIncludeFuture.TabIndex = 54;
            cbDefMovieIncludeFuture.Text = "Include check when movie has a future release date";
            cbDefMovieIncludeFuture.UseVisualStyleBackColor = true;
            // 
            // cbDefMovieDoMissing
            // 
            cbDefMovieDoMissing.AutoSize = true;
            cbDefMovieDoMissing.Location = new Point(7, 48);
            cbDefMovieDoMissing.Margin = new Padding(4, 3, 4, 3);
            cbDefMovieDoMissing.Name = "cbDefMovieDoMissing";
            cbDefMovieDoMissing.Size = new Size(119, 19);
            cbDefMovieDoMissing.TabIndex = 52;
            cbDefMovieDoMissing.Text = "Do &missing check";
            cbDefMovieDoMissing.UseVisualStyleBackColor = true;
            // 
            // cbDefMovieDoRenaming
            // 
            cbDefMovieDoRenaming.AutoSize = true;
            cbDefMovieDoRenaming.Location = new Point(7, 22);
            cbDefMovieDoRenaming.Margin = new Padding(4, 3, 4, 3);
            cbDefMovieDoRenaming.Name = "cbDefMovieDoRenaming";
            cbDefMovieDoRenaming.Size = new Size(95, 19);
            cbDefMovieDoRenaming.TabIndex = 51;
            cbDefMovieDoRenaming.Text = "Do &renaming";
            cbDefMovieDoRenaming.UseVisualStyleBackColor = true;
            // 
            // pbMovieDefaults
            // 
            pbMovieDefaults.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbMovieDefaults.Cursor = Cursors.Hand;
            pbMovieDefaults.Image = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbMovieDefaults.InitialImage = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbMovieDefaults.Location = new Point(418, 12);
            pbMovieDefaults.Margin = new Padding(4, 3, 4, 3);
            pbMovieDefaults.Name = "pbMovieDefaults";
            pbMovieDefaults.Size = new Size(50, 46);
            pbMovieDefaults.SizeMode = PictureBoxSizeMode.CenterImage;
            pbMovieDefaults.TabIndex = 62;
            pbMovieDefaults.TabStop = false;
            pbMovieDefaults.Click += pbMovieDefaults_Click;
            // 
            // tpShowDefaults
            // 
            tpShowDefaults.Controls.Add(label18);
            tpShowDefaults.Controls.Add(groupBox19);
            tpShowDefaults.Controls.Add(groupBox18);
            tpShowDefaults.Controls.Add(pictureBox1);
            tpShowDefaults.Location = new Point(149, 4);
            tpShowDefaults.Margin = new Padding(4, 3, 4, 3);
            tpShowDefaults.Name = "tpShowDefaults";
            tpShowDefaults.Padding = new Padding(4, 3, 4, 3);
            tpShowDefaults.Size = new Size(500, 684);
            tpShowDefaults.TabIndex = 14;
            tpShowDefaults.Text = "TV Show Defaults";
            tpShowDefaults.UseVisualStyleBackColor = true;
            // 
            // label18
            // 
            label18.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label18.AutoSize = true;
            label18.Location = new Point(8, 8);
            label18.Margin = new Padding(4, 0, 4, 0);
            label18.Name = "label18";
            label18.Size = new Size(355, 45);
            label18.TabIndex = 59;
            label18.Text = "These settings control the defaults used to add a new show to the \r\nsystem. Once the show has been created, you will need to modify\r\nits configuration directly.";
            // 
            // groupBox19
            // 
            groupBox19.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox19.Controls.Add(label24);
            groupBox19.Controls.Add(cbTimeZone);
            groupBox19.Controls.Add(rbDefShowUseSubFolders);
            groupBox19.Controls.Add(rbDefShowUseBase);
            groupBox19.Controls.Add(label12);
            groupBox19.Controls.Add(cmbDefShowLocation);
            groupBox19.Controls.Add(cbDefShowUseDefLocation);
            groupBox19.Controls.Add(cbDefShowAutoFolders);
            groupBox19.Location = new Point(7, 63);
            groupBox19.Margin = new Padding(4, 3, 4, 3);
            groupBox19.Name = "groupBox19";
            groupBox19.Padding = new Padding(4, 3, 4, 3);
            groupBox19.Size = new Size(456, 203);
            groupBox19.TabIndex = 57;
            groupBox19.TabStop = false;
            groupBox19.Text = "Default TV Show Settings";
            // 
            // label24
            // 
            label24.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label24.AutoSize = true;
            label24.Location = new Point(10, 172);
            label24.Margin = new Padding(4, 0, 4, 0);
            label24.Name = "label24";
            label24.Size = new Size(98, 15);
            label24.TabIndex = 6;
            label24.Text = "Airs in &Timezone:";
            label24.TextAlign = ContentAlignment.TopRight;
            // 
            // cbTimeZone
            // 
            cbTimeZone.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbTimeZone.DropDownStyle = ComboBoxStyle.DropDownList;
            cbTimeZone.FormattingEnabled = true;
            cbTimeZone.Location = new Point(119, 163);
            cbTimeZone.Margin = new Padding(4, 3, 4, 3);
            cbTimeZone.Name = "cbTimeZone";
            cbTimeZone.Size = new Size(330, 23);
            cbTimeZone.TabIndex = 7;
            // 
            // rbDefShowUseSubFolders
            // 
            rbDefShowUseSubFolders.AutoSize = true;
            rbDefShowUseSubFolders.Location = new Point(204, 130);
            rbDefShowUseSubFolders.Margin = new Padding(4, 3, 4, 3);
            rbDefShowUseSubFolders.Name = "rbDefShowUseSubFolders";
            rbDefShowUseSubFolders.Size = new Size(112, 19);
            rbDefShowUseSubFolders.TabIndex = 5;
            rbDefShowUseSubFolders.TabStop = true;
            rbDefShowUseSubFolders.Text = "in subdirectories";
            rbDefShowUseSubFolders.UseVisualStyleBackColor = true;
            // 
            // rbDefShowUseBase
            // 
            rbDefShowUseBase.AutoSize = true;
            rbDefShowUseBase.Location = new Point(34, 130);
            rbDefShowUseBase.Margin = new Padding(4, 3, 4, 3);
            rbDefShowUseBase.Name = "rbDefShowUseBase";
            rbDefShowUseBase.Size = new Size(96, 19);
            rbDefShowUseBase.TabIndex = 4;
            rbDefShowUseBase.TabStop = true;
            rbDefShowUseBase.Text = "in base folder";
            rbDefShowUseBase.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(10, 111);
            label12.Margin = new Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new Size(133, 15);
            label12.TabIndex = 3;
            label12.Text = "Default season location:";
            // 
            // cmbDefShowLocation
            // 
            cmbDefShowLocation.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbDefShowLocation.FormattingEnabled = true;
            cmbDefShowLocation.Location = new Point(34, 78);
            cmbDefShowLocation.Margin = new Padding(4, 3, 4, 3);
            cmbDefShowLocation.Name = "cmbDefShowLocation";
            cmbDefShowLocation.Size = new Size(415, 23);
            cmbDefShowLocation.TabIndex = 2;
            // 
            // cbDefShowUseDefLocation
            // 
            cbDefShowUseDefLocation.AutoSize = true;
            cbDefShowUseDefLocation.Location = new Point(10, 50);
            cbDefShowUseDefLocation.Margin = new Padding(4, 3, 4, 3);
            cbDefShowUseDefLocation.Name = "cbDefShowUseDefLocation";
            cbDefShowUseDefLocation.Size = new Size(135, 19);
            cbDefShowUseDefLocation.TabIndex = 1;
            cbDefShowUseDefLocation.Text = "Use Default Location";
            cbDefShowUseDefLocation.UseVisualStyleBackColor = true;
            cbDefShowUseDefLocation.CheckedChanged += CbDefShowUseDefLocation_CheckedChanged;
            // 
            // cbDefShowAutoFolders
            // 
            cbDefShowAutoFolders.AutoSize = true;
            cbDefShowAutoFolders.Location = new Point(10, 23);
            cbDefShowAutoFolders.Margin = new Padding(4, 3, 4, 3);
            cbDefShowAutoFolders.Name = "cbDefShowAutoFolders";
            cbDefShowAutoFolders.Size = new Size(185, 19);
            cbDefShowAutoFolders.TabIndex = 0;
            cbDefShowAutoFolders.Text = "Use Automatic Season Folders";
            cbDefShowAutoFolders.UseVisualStyleBackColor = true;
            // 
            // groupBox18
            // 
            groupBox18.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox18.Controls.Add(cbDefShowAlternateOrder);
            groupBox18.Controls.Add(cbDefShowEpNameMatching);
            groupBox18.Controls.Add(label68);
            groupBox18.Controls.Add(cbDefShowAirdateMatching);
            groupBox18.Controls.Add(cbDefShowSpecialsCount);
            groupBox18.Controls.Add(cbDefShowSequentialMatching);
            groupBox18.Controls.Add(cbDefShowIncludeNoAirdate);
            groupBox18.Controls.Add(cbDefShowDoMissingCheck);
            groupBox18.Controls.Add(cbDefShowDVDOrder);
            groupBox18.Controls.Add(cbDefShowIncludeFuture);
            groupBox18.Controls.Add(cbDefShowDoRenaming);
            groupBox18.Controls.Add(cbDefShowNextAirdate);
            groupBox18.Location = new Point(7, 273);
            groupBox18.Margin = new Padding(4, 3, 4, 3);
            groupBox18.Name = "groupBox18";
            groupBox18.Padding = new Padding(4, 3, 4, 3);
            groupBox18.Size = new Size(456, 298);
            groupBox18.TabIndex = 56;
            groupBox18.TabStop = false;
            groupBox18.Text = "Default Advanced Settings";
            // 
            // cbDefShowAlternateOrder
            // 
            cbDefShowAlternateOrder.AutoSize = true;
            cbDefShowAlternateOrder.Location = new Point(134, 32);
            cbDefShowAlternateOrder.Margin = new Padding(4, 3, 4, 3);
            cbDefShowAlternateOrder.Name = "cbDefShowAlternateOrder";
            cbDefShowAlternateOrder.Size = new Size(129, 19);
            cbDefShowAlternateOrder.TabIndex = 59;
            cbDefShowAlternateOrder.Text = "Use Alternate Order";
            cbDefShowAlternateOrder.UseVisualStyleBackColor = true;
            cbDefShowAlternateOrder.CheckedChanged += cbDefShowAlternateOrder_CheckedChanged;
            // 
            // cbDefShowEpNameMatching
            // 
            cbDefShowEpNameMatching.AutoSize = true;
            cbDefShowEpNameMatching.Location = new Point(34, 267);
            cbDefShowEpNameMatching.Margin = new Padding(4, 3, 4, 3);
            cbDefShowEpNameMatching.Name = "cbDefShowEpNameMatching";
            cbDefShowEpNameMatching.Size = new Size(204, 19);
            cbDefShowEpNameMatching.TabIndex = 58;
            cbDefShowEpNameMatching.Text = "Look for episode title in filenames";
            cbDefShowEpNameMatching.UseVisualStyleBackColor = true;
            // 
            // label68
            // 
            label68.AutoSize = true;
            label68.Location = new Point(10, 190);
            label68.Margin = new Padding(4, 0, 4, 0);
            label68.Name = "label68";
            label68.Size = new Size(206, 15);
            label68.TabIndex = 57;
            label68.Text = "When finding missing episodes (only)";
            // 
            // cbDefShowAirdateMatching
            // 
            cbDefShowAirdateMatching.AutoSize = true;
            cbDefShowAirdateMatching.Location = new Point(34, 240);
            cbDefShowAirdateMatching.Margin = new Padding(4, 3, 4, 3);
            cbDefShowAirdateMatching.Name = "cbDefShowAirdateMatching";
            cbDefShowAirdateMatching.Size = new Size(176, 19);
            cbDefShowAirdateMatching.TabIndex = 56;
            cbDefShowAirdateMatching.Text = "&Look for airdate in filenames";
            cbDefShowAirdateMatching.UseVisualStyleBackColor = true;
            // 
            // cbDefShowSpecialsCount
            // 
            cbDefShowSpecialsCount.AutoSize = true;
            cbDefShowSpecialsCount.Location = new Point(10, 85);
            cbDefShowSpecialsCount.Margin = new Padding(4, 3, 4, 3);
            cbDefShowSpecialsCount.Name = "cbDefShowSpecialsCount";
            cbDefShowSpecialsCount.Size = new Size(165, 19);
            cbDefShowSpecialsCount.TabIndex = 50;
            cbDefShowSpecialsCount.Text = "S&pecials count as episodes";
            cbDefShowSpecialsCount.UseVisualStyleBackColor = true;
            // 
            // cbDefShowSequentialMatching
            // 
            cbDefShowSequentialMatching.AutoSize = true;
            cbDefShowSequentialMatching.Location = new Point(34, 213);
            cbDefShowSequentialMatching.Margin = new Padding(4, 3, 4, 3);
            cbDefShowSequentialMatching.Name = "cbDefShowSequentialMatching";
            cbDefShowSequentialMatching.Size = new Size(201, 19);
            cbDefShowSequentialMatching.TabIndex = 53;
            cbDefShowSequentialMatching.Text = "Use sequential number matching";
            cbDefShowSequentialMatching.UseVisualStyleBackColor = true;
            // 
            // cbDefShowIncludeNoAirdate
            // 
            cbDefShowIncludeNoAirdate.AutoSize = true;
            cbDefShowIncludeNoAirdate.Location = new Point(204, 165);
            cbDefShowIncludeNoAirdate.Margin = new Padding(4, 3, 4, 3);
            cbDefShowIncludeNoAirdate.Name = "cbDefShowIncludeNoAirdate";
            cbDefShowIncludeNoAirdate.Size = new Size(121, 19);
            cbDefShowIncludeNoAirdate.TabIndex = 54;
            cbDefShowIncludeNoAirdate.Text = "Include no airdate";
            cbDefShowIncludeNoAirdate.UseVisualStyleBackColor = true;
            // 
            // cbDefShowDoMissingCheck
            // 
            cbDefShowDoMissingCheck.AutoSize = true;
            cbDefShowDoMissingCheck.Location = new Point(10, 138);
            cbDefShowDoMissingCheck.Margin = new Padding(4, 3, 4, 3);
            cbDefShowDoMissingCheck.Name = "cbDefShowDoMissingCheck";
            cbDefShowDoMissingCheck.Size = new Size(119, 19);
            cbDefShowDoMissingCheck.TabIndex = 52;
            cbDefShowDoMissingCheck.Text = "Do &missing check";
            cbDefShowDoMissingCheck.UseVisualStyleBackColor = true;
            // 
            // cbDefShowDVDOrder
            // 
            cbDefShowDVDOrder.AutoSize = true;
            cbDefShowDVDOrder.Location = new Point(10, 32);
            cbDefShowDVDOrder.Margin = new Padding(4, 3, 4, 3);
            cbDefShowDVDOrder.Name = "cbDefShowDVDOrder";
            cbDefShowDVDOrder.Size = new Size(104, 19);
            cbDefShowDVDOrder.TabIndex = 48;
            cbDefShowDVDOrder.Text = "&Use DVD Order";
            cbDefShowDVDOrder.UseVisualStyleBackColor = true;
            cbDefShowDVDOrder.CheckedChanged += cbDefShowDVDOrder_CheckedChanged;
            // 
            // cbDefShowIncludeFuture
            // 
            cbDefShowIncludeFuture.AutoSize = true;
            cbDefShowIncludeFuture.Location = new Point(34, 165);
            cbDefShowIncludeFuture.Margin = new Padding(4, 3, 4, 3);
            cbDefShowIncludeFuture.Name = "cbDefShowIncludeFuture";
            cbDefShowIncludeFuture.Size = new Size(149, 19);
            cbDefShowIncludeFuture.TabIndex = 55;
            cbDefShowIncludeFuture.Text = "Include future episodes";
            cbDefShowIncludeFuture.UseVisualStyleBackColor = true;
            // 
            // cbDefShowDoRenaming
            // 
            cbDefShowDoRenaming.AutoSize = true;
            cbDefShowDoRenaming.Location = new Point(10, 112);
            cbDefShowDoRenaming.Margin = new Padding(4, 3, 4, 3);
            cbDefShowDoRenaming.Name = "cbDefShowDoRenaming";
            cbDefShowDoRenaming.Size = new Size(95, 19);
            cbDefShowDoRenaming.TabIndex = 51;
            cbDefShowDoRenaming.Text = "Do &renaming";
            cbDefShowDoRenaming.UseVisualStyleBackColor = true;
            // 
            // cbDefShowNextAirdate
            // 
            cbDefShowNextAirdate.AutoSize = true;
            cbDefShowNextAirdate.Location = new Point(10, 59);
            cbDefShowNextAirdate.Margin = new Padding(4, 3, 4, 3);
            cbDefShowNextAirdate.Name = "cbDefShowNextAirdate";
            cbDefShowNextAirdate.Size = new Size(189, 19);
            cbDefShowNextAirdate.TabIndex = 49;
            cbDefShowNextAirdate.Text = "Show &next airdate in 'Schedule'";
            cbDefShowNextAirdate.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureBox1.Cursor = Cursors.Hand;
            pictureBox1.Image = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pictureBox1.InitialImage = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pictureBox1.Location = new Point(418, 7);
            pictureBox1.Margin = new Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(50, 46);
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox1.TabIndex = 58;
            pictureBox1.TabStop = false;
            pictureBox1.Click += tv_show_Defaults_Click;
            // 
            // tpScanSettings
            // 
            tpScanSettings.Controls.Add(chkGroupMissingEpisodesIntoSeasons);
            tpScanSettings.Controls.Add(groupBox17);
            tpScanSettings.Controls.Add(groupBox1);
            tpScanSettings.Controls.Add(cbScanIncludesBulkAdd);
            tpScanSettings.Controls.Add(gbBulkAdd);
            tpScanSettings.Controls.Add(label62);
            tpScanSettings.Controls.Add(pbScanOptions);
            tpScanSettings.Location = new Point(149, 4);
            tpScanSettings.Margin = new Padding(4, 3, 4, 3);
            tpScanSettings.Name = "tpScanSettings";
            tpScanSettings.Padding = new Padding(4, 3, 4, 3);
            tpScanSettings.Size = new Size(500, 684);
            tpScanSettings.TabIndex = 16;
            tpScanSettings.Text = "Scan Settings";
            tpScanSettings.UseVisualStyleBackColor = true;
            // 
            // chkGroupMissingEpisodesIntoSeasons
            // 
            chkGroupMissingEpisodesIntoSeasons.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            chkGroupMissingEpisodesIntoSeasons.AutoSize = true;
            chkGroupMissingEpisodesIntoSeasons.Location = new Point(8, 647);
            chkGroupMissingEpisodesIntoSeasons.Margin = new Padding(4, 3, 4, 3);
            chkGroupMissingEpisodesIntoSeasons.Name = "chkGroupMissingEpisodesIntoSeasons";
            chkGroupMissingEpisodesIntoSeasons.Size = new Size(244, 19);
            chkGroupMissingEpisodesIntoSeasons.TabIndex = 50;
            chkGroupMissingEpisodesIntoSeasons.Text = "Group Entire Seasons of Missing Episodes";
            chkGroupMissingEpisodesIntoSeasons.UseVisualStyleBackColor = true;
            // 
            // groupBox17
            // 
            groupBox17.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox17.Controls.Add(cbIgnorePreviouslySeenMovies);
            groupBox17.Controls.Add(chkIncludeMoviesQuickRecent);
            groupBox17.Controls.Add(chkIgnoreAllSpecials);
            groupBox17.Controls.Add(chkMoveLibraryFiles);
            groupBox17.Controls.Add(label1);
            groupBox17.Controls.Add(upDownScanHours);
            groupBox17.Controls.Add(chkScheduledScan);
            groupBox17.Controls.Add(chkScanOnStartup);
            groupBox17.Controls.Add(lblScanAction);
            groupBox17.Controls.Add(rdoQuickScan);
            groupBox17.Controls.Add(rdoRecentScan);
            groupBox17.Controls.Add(rdoFullScan);
            groupBox17.Controls.Add(cbIgnorePreviouslySeen);
            groupBox17.Controls.Add(chkPreventMove);
            groupBox17.Controls.Add(label28);
            groupBox17.Controls.Add(cbRenameCheck);
            groupBox17.Controls.Add(cbMissing);
            groupBox17.Location = new Point(8, 57);
            groupBox17.Margin = new Padding(4, 3, 4, 3);
            groupBox17.Name = "groupBox17";
            groupBox17.Padding = new Padding(4, 3, 4, 3);
            groupBox17.Size = new Size(461, 284);
            groupBox17.TabIndex = 49;
            groupBox17.TabStop = false;
            groupBox17.Text = "Scan Options";
            // 
            // cbIgnorePreviouslySeenMovies
            // 
            cbIgnorePreviouslySeenMovies.AutoSize = true;
            cbIgnorePreviouslySeenMovies.Location = new Point(248, 216);
            cbIgnorePreviouslySeenMovies.Margin = new Padding(4, 3, 4, 3);
            cbIgnorePreviouslySeenMovies.Name = "cbIgnorePreviouslySeenMovies";
            cbIgnorePreviouslySeenMovies.Size = new Size(186, 19);
            cbIgnorePreviouslySeenMovies.TabIndex = 51;
            cbIgnorePreviouslySeenMovies.Text = "Ignore Movies Previously Seen";
            cbIgnorePreviouslySeenMovies.UseVisualStyleBackColor = true;
            // 
            // chkMoveLibraryFiles
            // 
            chkMoveLibraryFiles.AutoSize = true;
            chkMoveLibraryFiles.Checked = true;
            chkMoveLibraryFiles.CheckState = CheckState.Checked;
            chkMoveLibraryFiles.Location = new Point(12, 264);
            chkMoveLibraryFiles.Margin = new Padding(4, 3, 4, 3);
            chkMoveLibraryFiles.Name = "chkMoveLibraryFiles";
            chkMoveLibraryFiles.Size = new Size(236, 19);
            chkMoveLibraryFiles.TabIndex = 48;
            chkMoveLibraryFiles.Text = "Move Files within Library to Keep it Tidy";
            chkMoveLibraryFiles.UseVisualStyleBackColor = true;
            // 
            // lblScanAction
            // 
            lblScanAction.AutoSize = true;
            lblScanAction.Location = new Point(10, 22);
            lblScanAction.Margin = new Padding(4, 0, 4, 0);
            lblScanAction.Name = "lblScanAction";
            lblScanAction.Size = new Size(130, 15);
            lblScanAction.TabIndex = 43;
            lblScanAction.Text = "Default Auto &Scan Type";
            // 
            // rdoQuickScan
            // 
            rdoQuickScan.AutoSize = true;
            rdoQuickScan.Location = new Point(164, 42);
            rdoQuickScan.Margin = new Padding(4, 3, 4, 3);
            rdoQuickScan.Name = "rdoQuickScan";
            rdoQuickScan.Size = new Size(56, 19);
            rdoQuickScan.TabIndex = 42;
            rdoQuickScan.TabStop = true;
            rdoQuickScan.Text = "&Quick";
            rdoQuickScan.UseVisualStyleBackColor = true;
            // 
            // rdoRecentScan
            // 
            rdoRecentScan.AutoSize = true;
            rdoRecentScan.Location = new Point(88, 42);
            rdoRecentScan.Margin = new Padding(4, 3, 4, 3);
            rdoRecentScan.Name = "rdoRecentScan";
            rdoRecentScan.Size = new Size(61, 19);
            rdoRecentScan.TabIndex = 41;
            rdoRecentScan.TabStop = true;
            rdoRecentScan.Text = "&Recent";
            rdoRecentScan.UseVisualStyleBackColor = true;
            // 
            // rdoFullScan
            // 
            rdoFullScan.AutoSize = true;
            rdoFullScan.Location = new Point(33, 42);
            rdoFullScan.Margin = new Padding(4, 3, 4, 3);
            rdoFullScan.Name = "rdoFullScan";
            rdoFullScan.Size = new Size(44, 19);
            rdoFullScan.TabIndex = 40;
            rdoFullScan.TabStop = true;
            rdoFullScan.Text = "&Full";
            rdoFullScan.UseVisualStyleBackColor = true;
            // 
            // cbIgnorePreviouslySeen
            // 
            cbIgnorePreviouslySeen.AutoSize = true;
            cbIgnorePreviouslySeen.Location = new Point(35, 216);
            cbIgnorePreviouslySeen.Margin = new Padding(4, 3, 4, 3);
            cbIgnorePreviouslySeen.Name = "cbIgnorePreviouslySeen";
            cbIgnorePreviouslySeen.Size = new Size(194, 19);
            cbIgnorePreviouslySeen.TabIndex = 39;
            cbIgnorePreviouslySeen.Text = "Ignore Episodes Previously Seen";
            cbIgnorePreviouslySeen.UseVisualStyleBackColor = true;
            // 
            // chkPreventMove
            // 
            chkPreventMove.AutoSize = true;
            chkPreventMove.Location = new Point(35, 167);
            chkPreventMove.Margin = new Padding(4, 3, 4, 3);
            chkPreventMove.Name = "chkPreventMove";
            chkPreventMove.Size = new Size(210, 19);
            chkPreventMove.TabIndex = 38;
            chkPreventMove.Text = "Pre&vent move of files (just rename)";
            chkPreventMove.UseVisualStyleBackColor = true;
            // 
            // label28
            // 
            label28.AutoSize = true;
            label28.Location = new Point(10, 122);
            label28.Margin = new Padding(4, 0, 4, 0);
            label28.Name = "label28";
            label28.Size = new Size(148, 15);
            label28.TabIndex = 35;
            label28.Text = "\"Scan\" checks and actions:";
            // 
            // cbRenameCheck
            // 
            cbRenameCheck.AutoSize = true;
            cbRenameCheck.Checked = true;
            cbRenameCheck.CheckState = CheckState.Checked;
            cbRenameCheck.Location = new Point(12, 141);
            cbRenameCheck.Margin = new Padding(4, 3, 4, 3);
            cbRenameCheck.Name = "cbRenameCheck";
            cbRenameCheck.Size = new Size(105, 19);
            cbRenameCheck.TabIndex = 36;
            cbRenameCheck.Text = "&Rename Check";
            cbRenameCheck.UseVisualStyleBackColor = true;
            // 
            // cbMissing
            // 
            cbMissing.AutoSize = true;
            cbMissing.Checked = true;
            cbMissing.CheckState = CheckState.Checked;
            cbMissing.Location = new Point(12, 194);
            cbMissing.Margin = new Padding(4, 3, 4, 3);
            cbMissing.Name = "cbMissing";
            cbMissing.Size = new Size(103, 19);
            cbMissing.TabIndex = 37;
            cbMissing.Text = "&Missing Check";
            cbMissing.UseVisualStyleBackColor = true;
            cbMissing.CheckedChanged += EnableDisable;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(chkChooseWhenMultipleEpisodesMatch);
            groupBox1.Controls.Add(cbxUpdateAirDate);
            groupBox1.Controls.Add(cbAutoCreateFolders);
            groupBox1.Controls.Add(chkAutoMergeLibraryEpisodes);
            groupBox1.Location = new Point(7, 347);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(460, 133);
            groupBox1.TabIndex = 48;
            groupBox1.TabStop = false;
            groupBox1.Text = "Additional Scan Options";
            // 
            // chkChooseWhenMultipleEpisodesMatch
            // 
            chkChooseWhenMultipleEpisodesMatch.AutoSize = true;
            chkChooseWhenMultipleEpisodesMatch.Location = new Point(7, 102);
            chkChooseWhenMultipleEpisodesMatch.Margin = new Padding(4, 3, 4, 3);
            chkChooseWhenMultipleEpisodesMatch.Name = "chkChooseWhenMultipleEpisodesMatch";
            chkChooseWhenMultipleEpisodesMatch.Size = new Size(328, 19);
            chkChooseWhenMultipleEpisodesMatch.TabIndex = 42;
            chkChooseWhenMultipleEpisodesMatch.Text = "Choose between episodes in library when multiple match";
            chkChooseWhenMultipleEpisodesMatch.UseVisualStyleBackColor = true;
            // 
            // cbxUpdateAirDate
            // 
            cbxUpdateAirDate.AutoSize = true;
            cbxUpdateAirDate.Location = new Point(7, 22);
            cbxUpdateAirDate.Margin = new Padding(4, 3, 4, 3);
            cbxUpdateAirDate.Name = "cbxUpdateAirDate";
            cbxUpdateAirDate.Size = new Size(218, 19);
            cbxUpdateAirDate.TabIndex = 39;
            cbxUpdateAirDate.Text = "Update files and folders with air date";
            cbxUpdateAirDate.UseVisualStyleBackColor = true;
            // 
            // cbAutoCreateFolders
            // 
            cbAutoCreateFolders.AutoSize = true;
            cbAutoCreateFolders.Location = new Point(7, 75);
            cbAutoCreateFolders.Margin = new Padding(4, 3, 4, 3);
            cbAutoCreateFolders.Name = "cbAutoCreateFolders";
            cbAutoCreateFolders.Size = new Size(218, 19);
            cbAutoCreateFolders.TabIndex = 37;
            cbAutoCreateFolders.Text = "&Automatically create missing folders";
            cbAutoCreateFolders.UseVisualStyleBackColor = true;
            // 
            // chkAutoMergeLibraryEpisodes
            // 
            chkAutoMergeLibraryEpisodes.AutoSize = true;
            chkAutoMergeLibraryEpisodes.Location = new Point(7, 48);
            chkAutoMergeLibraryEpisodes.Margin = new Padding(4, 3, 4, 3);
            chkAutoMergeLibraryEpisodes.Name = "chkAutoMergeLibraryEpisodes";
            chkAutoMergeLibraryEpisodes.Size = new Size(347, 19);
            chkAutoMergeLibraryEpisodes.TabIndex = 41;
            chkAutoMergeLibraryEpisodes.Text = "Automatically create merge rules for merged library episodes";
            chkAutoMergeLibraryEpisodes.UseVisualStyleBackColor = true;
            // 
            // cbScanIncludesBulkAdd
            // 
            cbScanIncludesBulkAdd.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbScanIncludesBulkAdd.AutoSize = true;
            cbScanIncludesBulkAdd.Location = new Point(8, 487);
            cbScanIncludesBulkAdd.Margin = new Padding(4, 3, 4, 3);
            cbScanIncludesBulkAdd.Name = "cbScanIncludesBulkAdd";
            cbScanIncludesBulkAdd.Size = new Size(173, 19);
            cbScanIncludesBulkAdd.TabIndex = 47;
            cbScanIncludesBulkAdd.Text = "Do Bulk-Add as part of scan";
            cbScanIncludesBulkAdd.UseVisualStyleBackColor = true;
            // 
            // gbBulkAdd
            // 
            gbBulkAdd.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbBulkAdd.Controls.Add(tbSeasonSearchTerms);
            gbBulkAdd.Controls.Add(label36);
            gbBulkAdd.Controls.Add(chkForceBulkAddToUseSettingsOnly);
            gbBulkAdd.Controls.Add(cbIgnoreRecycleBin);
            gbBulkAdd.Controls.Add(cbIgnoreNoVideoFolders);
            gbBulkAdd.Location = new Point(8, 513);
            gbBulkAdd.Margin = new Padding(4, 3, 4, 3);
            gbBulkAdd.Name = "gbBulkAdd";
            gbBulkAdd.Padding = new Padding(4, 3, 4, 3);
            gbBulkAdd.Size = new Size(460, 128);
            gbBulkAdd.TabIndex = 46;
            gbBulkAdd.TabStop = false;
            gbBulkAdd.Text = "Bulk Add TV Shows from Library Folders";
            // 
            // label36
            // 
            label36.AutoSize = true;
            label36.Location = new Point(7, 102);
            label36.Margin = new Padding(4, 0, 4, 0);
            label36.Name = "label36";
            label36.Size = new Size(117, 15);
            label36.TabIndex = 21;
            label36.Text = "Season search terms:";
            // 
            // label62
            // 
            label62.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label62.AutoSize = true;
            label62.Location = new Point(7, 3);
            label62.Margin = new Padding(4, 0, 4, 0);
            label62.Name = "label62";
            label62.Size = new Size(323, 30);
            label62.TabIndex = 40;
            label62.Text = "General settings to control TV Rename's scan and download\r\nbehaviour";
            // 
            // pbScanOptions
            // 
            pbScanOptions.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbScanOptions.Cursor = Cursors.Hand;
            pbScanOptions.Image = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbScanOptions.InitialImage = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbScanOptions.Location = new Point(418, 3);
            pbScanOptions.Margin = new Padding(4, 3, 4, 3);
            pbScanOptions.Name = "pbScanOptions";
            pbScanOptions.Size = new Size(50, 46);
            pbScanOptions.SizeMode = PictureBoxSizeMode.CenterImage;
            pbScanOptions.TabIndex = 39;
            pbScanOptions.TabStop = false;
            pbScanOptions.Click += pbScanOptions_Click;
            // 
            // tpSubtitles
            // 
            tpSubtitles.Controls.Add(groupBox29);
            tpSubtitles.Controls.Add(label93);
            tpSubtitles.Controls.Add(pictureBox2);
            tpSubtitles.Controls.Add(groupBox9);
            tpSubtitles.Location = new Point(149, 4);
            tpSubtitles.Margin = new Padding(4, 3, 4, 3);
            tpSubtitles.Name = "tpSubtitles";
            tpSubtitles.Padding = new Padding(4, 3, 4, 3);
            tpSubtitles.Size = new Size(500, 684);
            tpSubtitles.TabIndex = 21;
            tpSubtitles.Text = "Subtitles";
            tpSubtitles.UseVisualStyleBackColor = true;
            // 
            // groupBox29
            // 
            groupBox29.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox29.Controls.Add(label94);
            groupBox29.Controls.Add(txtSubtitleFolderNames);
            groupBox29.Controls.Add(cbCopySubsFolders);
            groupBox29.Location = new Point(7, 185);
            groupBox29.Margin = new Padding(4, 3, 4, 3);
            groupBox29.Name = "groupBox29";
            groupBox29.Padding = new Padding(4, 3, 4, 3);
            groupBox29.Size = new Size(464, 90);
            groupBox29.TabIndex = 44;
            groupBox29.TabStop = false;
            groupBox29.Text = "Subtitle Folders";
            // 
            // label94
            // 
            label94.AutoSize = true;
            label94.Location = new Point(4, 52);
            label94.Margin = new Padding(4, 0, 4, 0);
            label94.Name = "label94";
            label94.Size = new Size(126, 15);
            label94.TabIndex = 30;
            label94.Text = "&Subtitle Folder Names:";
            // 
            // txtSubtitleFolderNames
            // 
            txtSubtitleFolderNames.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSubtitleFolderNames.Location = new Point(142, 48);
            txtSubtitleFolderNames.Margin = new Padding(4, 3, 4, 3);
            txtSubtitleFolderNames.Name = "txtSubtitleFolderNames";
            txtSubtitleFolderNames.Size = new Size(314, 23);
            txtSubtitleFolderNames.TabIndex = 31;
            // 
            // cbCopySubsFolders
            // 
            cbCopySubsFolders.AutoSize = true;
            cbCopySubsFolders.Location = new Point(7, 22);
            cbCopySubsFolders.Margin = new Padding(4, 3, 4, 3);
            cbCopySubsFolders.Name = "cbCopySubsFolders";
            cbCopySubsFolders.Size = new Size(135, 19);
            cbCopySubsFolders.TabIndex = 29;
            cbCopySubsFolders.Text = "Copy subtitle folders";
            cbCopySubsFolders.UseVisualStyleBackColor = true;
            // 
            // label93
            // 
            label93.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label93.AutoSize = true;
            label93.Location = new Point(4, 3);
            label93.Margin = new Padding(4, 0, 4, 0);
            label93.Name = "label93";
            label93.Size = new Size(322, 30);
            label93.TabIndex = 43;
            label93.Text = "These preferences control how TV Rename finds and retains\r\nsubtitles and subtitle files.";
            // 
            // pictureBox2
            // 
            pictureBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureBox2.Cursor = Cursors.Hand;
            pictureBox2.Image = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pictureBox2.InitialImage = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pictureBox2.Location = new Point(414, 2);
            pictureBox2.Margin = new Padding(4, 3, 4, 3);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(50, 46);
            pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox2.TabIndex = 42;
            pictureBox2.TabStop = false;
            pictureBox2.Click += subtitles_Click;
            // 
            // groupBox9
            // 
            groupBox9.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox9.Controls.Add(cbTxtToSub);
            groupBox9.Controls.Add(label46);
            groupBox9.Controls.Add(txtSubtitleExtensions);
            groupBox9.Controls.Add(chkRetainLanguageSpecificSubtitles);
            groupBox9.Location = new Point(7, 55);
            groupBox9.Margin = new Padding(4, 3, 4, 3);
            groupBox9.Name = "groupBox9";
            groupBox9.Padding = new Padding(4, 3, 4, 3);
            groupBox9.Size = new Size(464, 122);
            groupBox9.TabIndex = 41;
            groupBox9.TabStop = false;
            groupBox9.Text = "Subtitles";
            // 
            // cbTxtToSub
            // 
            cbTxtToSub.AutoSize = true;
            cbTxtToSub.Location = new Point(7, 48);
            cbTxtToSub.Margin = new Padding(4, 3, 4, 3);
            cbTxtToSub.Name = "cbTxtToSub";
            cbTxtToSub.Size = new Size(127, 19);
            cbTxtToSub.TabIndex = 32;
            cbTxtToSub.Text = "&Rename .txt to .sub";
            cbTxtToSub.UseVisualStyleBackColor = true;
            // 
            // label46
            // 
            label46.AutoSize = true;
            label46.Location = new Point(4, 81);
            label46.Margin = new Padding(4, 0, 4, 0);
            label46.Name = "label46";
            label46.Size = new Size(108, 15);
            label46.TabIndex = 30;
            label46.Text = "&Subtitle extensions:";
            // 
            // txtSubtitleExtensions
            // 
            txtSubtitleExtensions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSubtitleExtensions.Location = new Point(125, 77);
            txtSubtitleExtensions.Margin = new Padding(4, 3, 4, 3);
            txtSubtitleExtensions.Name = "txtSubtitleExtensions";
            txtSubtitleExtensions.Size = new Size(332, 23);
            txtSubtitleExtensions.TabIndex = 31;
            // 
            // chkRetainLanguageSpecificSubtitles
            // 
            chkRetainLanguageSpecificSubtitles.AutoSize = true;
            chkRetainLanguageSpecificSubtitles.Location = new Point(7, 22);
            chkRetainLanguageSpecificSubtitles.Margin = new Padding(4, 3, 4, 3);
            chkRetainLanguageSpecificSubtitles.Name = "chkRetainLanguageSpecificSubtitles";
            chkRetainLanguageSpecificSubtitles.Size = new Size(206, 19);
            chkRetainLanguageSpecificSubtitles.TabIndex = 29;
            chkRetainLanguageSpecificSubtitles.Text = "Retain &Language Specific Subtitles";
            chkRetainLanguageSpecificSubtitles.UseVisualStyleBackColor = true;
            // 
            // tpJackett
            // 
            tpJackett.Controls.Add(label99);
            tpJackett.Controls.Add(txtMinRSSSeeders);
            tpJackett.Controls.Add(label97);
            tpJackett.Controls.Add(tbUnwantedRSSTerms);
            tpJackett.Controls.Add(chkSearchJackettButton);
            tpJackett.Controls.Add(cmbSupervisedDuplicateAction);
            tpJackett.Controls.Add(label77);
            tpJackett.Controls.Add(cmbUnattendedDuplicateAction);
            tpJackett.Controls.Add(label76);
            tpJackett.Controls.Add(cbDetailedRSSJSONLogging);
            tpJackett.Controls.Add(pbuJackett);
            tpJackett.Controls.Add(label70);
            tpJackett.Controls.Add(cbSearchJackett);
            tpJackett.Controls.Add(groupBox22);
            tpJackett.Controls.Add(label45);
            tpJackett.Controls.Add(tbPreferredRSSTerms);
            tpJackett.Location = new Point(149, 4);
            tpJackett.Margin = new Padding(4, 3, 4, 3);
            tpJackett.Name = "tpJackett";
            tpJackett.Padding = new Padding(4, 3, 4, 3);
            tpJackett.Size = new Size(500, 684);
            tpJackett.TabIndex = 17;
            tpJackett.Text = "Jackett Search";
            tpJackett.UseVisualStyleBackColor = true;
            // 
            // label99
            // 
            label99.AutoSize = true;
            label99.Location = new Point(9, 175);
            label99.Margin = new Padding(4, 0, 4, 0);
            label99.Name = "label99";
            label99.Size = new Size(74, 15);
            label99.TabIndex = 51;
            label99.Text = "Min Seeders:";
            // 
            // txtMinRSSSeeders
            // 
            txtMinRSSSeeders.Location = new Point(420, 171);
            txtMinRSSSeeders.Margin = new Padding(4, 3, 4, 3);
            txtMinRSSSeeders.Name = "txtMinRSSSeeders";
            txtMinRSSSeeders.Size = new Size(32, 23);
            txtMinRSSSeeders.TabIndex = 52;
            txtMinRSSSeeders.TextChanged += EnsureInteger;
            txtMinRSSSeeders.KeyPress += TxtNumberOnlyKeyPress;
            // 
            // label97
            // 
            label97.AutoSize = true;
            label97.Location = new Point(7, 147);
            label97.Margin = new Padding(4, 0, 4, 0);
            label97.Name = "label97";
            label97.Size = new Size(99, 15);
            label97.TabIndex = 50;
            label97.Text = "Unwanted Terms:";
            // 
            // tbUnwantedRSSTerms
            // 
            tbUnwantedRSSTerms.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbUnwantedRSSTerms.Location = new Point(120, 143);
            tbUnwantedRSSTerms.Margin = new Padding(4, 3, 4, 3);
            tbUnwantedRSSTerms.Name = "tbUnwantedRSSTerms";
            tbUnwantedRSSTerms.Size = new Size(332, 23);
            tbUnwantedRSSTerms.TabIndex = 49;
            // 
            // chkSearchJackettButton
            // 
            chkSearchJackettButton.AutoSize = true;
            chkSearchJackettButton.Location = new Point(265, 282);
            chkSearchJackettButton.Margin = new Padding(4, 3, 4, 3);
            chkSearchJackettButton.Name = "chkSearchJackettButton";
            chkSearchJackettButton.Size = new Size(177, 19);
            chkSearchJackettButton.TabIndex = 48;
            chkSearchJackettButton.Text = "Show 'Search &Jackett' button";
            chkSearchJackettButton.UseVisualStyleBackColor = true;
            // 
            // cmbSupervisedDuplicateAction
            // 
            cmbSupervisedDuplicateAction.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbSupervisedDuplicateAction.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSupervisedDuplicateAction.FormattingEnabled = true;
            cmbSupervisedDuplicateAction.Items.AddRange(new object[] { "Ask User", "Choose Largest File", "Choose Most Popular", "Download All", "Ignore", "Use First" });
            cmbSupervisedDuplicateAction.Location = new Point(190, 231);
            cmbSupervisedDuplicateAction.Margin = new Padding(4, 3, 4, 3);
            cmbSupervisedDuplicateAction.Name = "cmbSupervisedDuplicateAction";
            cmbSupervisedDuplicateAction.Size = new Size(262, 23);
            cmbSupervisedDuplicateAction.Sorted = true;
            cmbSupervisedDuplicateAction.TabIndex = 47;
            // 
            // label77
            // 
            label77.AutoSize = true;
            label77.Location = new Point(8, 234);
            label77.Margin = new Padding(4, 0, 4, 0);
            label77.Name = "label77";
            label77.Size = new Size(158, 15);
            label77.TabIndex = 46;
            label77.Text = "Supervised Duplicate Action:";
            // 
            // cmbUnattendedDuplicateAction
            // 
            cmbUnattendedDuplicateAction.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbUnattendedDuplicateAction.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbUnattendedDuplicateAction.FormattingEnabled = true;
            cmbUnattendedDuplicateAction.Items.AddRange(new object[] { "Ask User", "Choose Largest File", "Choose Most Popular", "Download All", "Ignore", "Use First" });
            cmbUnattendedDuplicateAction.Location = new Point(190, 200);
            cmbUnattendedDuplicateAction.Margin = new Padding(4, 3, 4, 3);
            cmbUnattendedDuplicateAction.Name = "cmbUnattendedDuplicateAction";
            cmbUnattendedDuplicateAction.Size = new Size(262, 23);
            cmbUnattendedDuplicateAction.Sorted = true;
            cmbUnattendedDuplicateAction.TabIndex = 45;
            // 
            // label76
            // 
            label76.AutoSize = true;
            label76.Location = new Point(8, 203);
            label76.Margin = new Padding(4, 0, 4, 0);
            label76.Name = "label76";
            label76.Size = new Size(163, 15);
            label76.TabIndex = 44;
            label76.Text = "Unattended Duplicate Action:";
            // 
            // cbDetailedRSSJSONLogging
            // 
            cbDetailedRSSJSONLogging.AutoSize = true;
            cbDetailedRSSJSONLogging.Location = new Point(9, 87);
            cbDetailedRSSJSONLogging.Margin = new Padding(4, 3, 4, 3);
            cbDetailedRSSJSONLogging.Name = "cbDetailedRSSJSONLogging";
            cbDetailedRSSJSONLogging.Size = new Size(332, 19);
            cbDetailedRSSJSONLogging.TabIndex = 43;
            cbDetailedRSSJSONLogging.Text = "Detailed logging (useful when setting up RSS/JSON Feeds)";
            cbDetailedRSSJSONLogging.UseVisualStyleBackColor = false;
            // 
            // pbuJackett
            // 
            pbuJackett.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbuJackett.Cursor = Cursors.Hand;
            pbuJackett.Image = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbuJackett.InitialImage = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbuJackett.Location = new Point(418, 14);
            pbuJackett.Margin = new Padding(4, 3, 4, 3);
            pbuJackett.Name = "pbuJackett";
            pbuJackett.Size = new Size(50, 46);
            pbuJackett.SizeMode = PictureBoxSizeMode.CenterImage;
            pbuJackett.TabIndex = 42;
            pbuJackett.TabStop = false;
            pbuJackett.Click += pbuJackett_Click;
            // 
            // label70
            // 
            label70.AutoSize = true;
            label70.Location = new Point(5, 8);
            label70.Margin = new Padding(4, 0, 4, 0);
            label70.Name = "label70";
            label70.Size = new Size(353, 45);
            label70.TabIndex = 41;
            label70.Text = "If an episode is missing from your library, TV Rename will talk to a\r\nrunning Jackett instance for appropriate files to download. It will \r\nuse the torrent handlers to download the file(s)";
            // 
            // cbSearchJackett
            // 
            cbSearchJackett.AutoSize = true;
            cbSearchJackett.Location = new Point(9, 282);
            cbSearchJackett.Margin = new Padding(4, 3, 4, 3);
            cbSearchJackett.Name = "cbSearchJackett";
            cbSearchJackett.Size = new Size(186, 19);
            cbSearchJackett.TabIndex = 40;
            cbSearchJackett.Text = "Search &Jackett for missing files";
            cbSearchJackett.UseVisualStyleBackColor = true;
            cbSearchJackett.CheckedChanged += EnableDisable;
            // 
            // groupBox22
            // 
            groupBox22.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox22.Controls.Add(chkUseJackettTextSearch);
            groupBox22.Controls.Add(chkSkipJackettFullScans);
            groupBox22.Controls.Add(llJackettLink);
            groupBox22.Controls.Add(label71);
            groupBox22.Controls.Add(cbSearchJackettOnManualScansOnly);
            groupBox22.Controls.Add(label72);
            groupBox22.Controls.Add(txtJackettIndexer);
            groupBox22.Controls.Add(label73);
            groupBox22.Controls.Add(txtJackettAPIKey);
            groupBox22.Controls.Add(label74);
            groupBox22.Controls.Add(txtJackettPort);
            groupBox22.Controls.Add(label75);
            groupBox22.Controls.Add(txtJackettServer);
            groupBox22.Location = new Point(7, 308);
            groupBox22.Margin = new Padding(4, 3, 4, 3);
            groupBox22.Name = "groupBox22";
            groupBox22.Padding = new Padding(4, 3, 4, 3);
            groupBox22.Size = new Size(464, 232);
            groupBox22.TabIndex = 39;
            groupBox22.TabStop = false;
            groupBox22.Text = "Jackett Search";
            // 
            // chkUseJackettTextSearch
            // 
            chkUseJackettTextSearch.AutoSize = true;
            chkUseJackettTextSearch.Location = new Point(114, 168);
            chkUseJackettTextSearch.Margin = new Padding(4, 3, 4, 3);
            chkUseJackettTextSearch.Name = "chkUseJackettTextSearch";
            chkUseJackettTextSearch.Size = new Size(104, 19);
            chkUseJackettTextSearch.TabIndex = 42;
            chkUseJackettTextSearch.Text = "Use text search";
            chkUseJackettTextSearch.UseVisualStyleBackColor = true;
            // 
            // chkSkipJackettFullScans
            // 
            chkSkipJackettFullScans.AutoSize = true;
            chkSkipJackettFullScans.Location = new Point(289, 22);
            chkSkipJackettFullScans.Margin = new Padding(4, 3, 4, 3);
            chkSkipJackettFullScans.Name = "chkSkipJackettFullScans";
            chkSkipJackettFullScans.Size = new Size(161, 19);
            chkSkipJackettFullScans.TabIndex = 41;
            chkSkipJackettFullScans.Text = "Skip Jackett On Full Scans";
            chkSkipJackettFullScans.UseVisualStyleBackColor = true;
            // 
            // llJackettLink
            // 
            llJackettLink.AutoSize = true;
            llJackettLink.Location = new Point(114, 196);
            llJackettLink.Margin = new Padding(4, 0, 4, 0);
            llJackettLink.Name = "llJackettLink";
            llJackettLink.Size = new Size(60, 15);
            llJackettLink.TabIndex = 40;
            llJackettLink.TabStop = true;
            llJackettLink.Text = "linkLabel1";
            llJackettLink.LinkClicked += LlJackettLink_LinkClicked;
            // 
            // label71
            // 
            label71.AutoSize = true;
            label71.Location = new Point(7, 196);
            label71.Margin = new Padding(4, 0, 4, 0);
            label71.Name = "label71";
            label71.Size = new Size(84, 15);
            label71.TabIndex = 39;
            label71.Text = "Configuration:";
            // 
            // cbSearchJackettOnManualScansOnly
            // 
            cbSearchJackettOnManualScansOnly.AutoSize = true;
            cbSearchJackettOnManualScansOnly.Location = new Point(10, 22);
            cbSearchJackettOnManualScansOnly.Margin = new Padding(4, 3, 4, 3);
            cbSearchJackettOnManualScansOnly.Name = "cbSearchJackettOnManualScansOnly";
            cbSearchJackettOnManualScansOnly.Size = new Size(143, 19);
            cbSearchJackettOnManualScansOnly.TabIndex = 38;
            cbSearchJackettOnManualScansOnly.Text = "Only on manual scans";
            cbSearchJackettOnManualScansOnly.UseVisualStyleBackColor = true;
            // 
            // label72
            // 
            label72.AutoSize = true;
            label72.Location = new Point(7, 112);
            label72.Margin = new Padding(4, 0, 4, 0);
            label72.Name = "label72";
            label72.Size = new Size(75, 15);
            label72.TabIndex = 37;
            label72.Text = "Indexer Path:";
            // 
            // txtJackettIndexer
            // 
            txtJackettIndexer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtJackettIndexer.Location = new Point(114, 108);
            txtJackettIndexer.Margin = new Padding(4, 3, 4, 3);
            txtJackettIndexer.Name = "txtJackettIndexer";
            txtJackettIndexer.Size = new Size(344, 23);
            txtJackettIndexer.TabIndex = 36;
            // 
            // label73
            // 
            label73.AutoSize = true;
            label73.Location = new Point(7, 142);
            label73.Margin = new Padding(4, 0, 4, 0);
            label73.Name = "label73";
            label73.Size = new Size(50, 15);
            label73.TabIndex = 35;
            label73.Text = "API Key:";
            // 
            // txtJackettAPIKey
            // 
            txtJackettAPIKey.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtJackettAPIKey.Location = new Point(114, 138);
            txtJackettAPIKey.Margin = new Padding(4, 3, 4, 3);
            txtJackettAPIKey.Name = "txtJackettAPIKey";
            txtJackettAPIKey.Size = new Size(344, 23);
            txtJackettAPIKey.TabIndex = 34;
            // 
            // label74
            // 
            label74.AutoSize = true;
            label74.Location = new Point(7, 82);
            label74.Margin = new Padding(4, 0, 4, 0);
            label74.Name = "label74";
            label74.Size = new Size(32, 15);
            label74.TabIndex = 33;
            label74.Text = "Port:";
            // 
            // txtJackettPort
            // 
            txtJackettPort.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtJackettPort.Location = new Point(114, 78);
            txtJackettPort.Margin = new Padding(4, 3, 4, 3);
            txtJackettPort.Name = "txtJackettPort";
            txtJackettPort.Size = new Size(344, 23);
            txtJackettPort.TabIndex = 32;
            txtJackettPort.TextChanged += JackettDetailsUpdate;
            // 
            // label75
            // 
            label75.AutoSize = true;
            label75.Location = new Point(7, 52);
            label75.Margin = new Padding(4, 0, 4, 0);
            label75.Name = "label75";
            label75.Size = new Size(42, 15);
            label75.TabIndex = 31;
            label75.Text = "Server:";
            // 
            // txtJackettServer
            // 
            txtJackettServer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtJackettServer.Location = new Point(113, 48);
            txtJackettServer.Margin = new Padding(4, 3, 4, 3);
            txtJackettServer.Name = "txtJackettServer";
            txtJackettServer.Size = new Size(344, 23);
            txtJackettServer.TabIndex = 30;
            txtJackettServer.TextChanged += JackettDetailsUpdate;
            // 
            // label45
            // 
            label45.AutoSize = true;
            label45.Location = new Point(7, 117);
            label45.Margin = new Padding(4, 0, 4, 0);
            label45.Name = "label45";
            label45.Size = new Size(93, 15);
            label45.TabIndex = 33;
            label45.Text = "Preferred Terms:";
            // 
            // tbPreferredRSSTerms
            // 
            tbPreferredRSSTerms.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbPreferredRSSTerms.Location = new Point(120, 113);
            tbPreferredRSSTerms.Margin = new Padding(4, 3, 4, 3);
            tbPreferredRSSTerms.Name = "tbPreferredRSSTerms";
            tbPreferredRSSTerms.Size = new Size(332, 23);
            tbPreferredRSSTerms.TabIndex = 32;
            // 
            // tpAutoExportLibrary
            // 
            tpAutoExportLibrary.Controls.Add(chkRestrictMissingExportsToFullScans);
            tpAutoExportLibrary.Controls.Add(pbuShowExport);
            tpAutoExportLibrary.Controls.Add(label89);
            tpAutoExportLibrary.Controls.Add(groupBox26);
            tpAutoExportLibrary.Controls.Add(groupBox7);
            tpAutoExportLibrary.Controls.Add(groupBox27);
            tpAutoExportLibrary.Controls.Add(groupBox3);
            tpAutoExportLibrary.Location = new Point(149, 4);
            tpAutoExportLibrary.Margin = new Padding(4, 3, 4, 3);
            tpAutoExportLibrary.Name = "tpAutoExportLibrary";
            tpAutoExportLibrary.Padding = new Padding(4, 3, 4, 3);
            tpAutoExportLibrary.Size = new Size(500, 684);
            tpAutoExportLibrary.TabIndex = 19;
            tpAutoExportLibrary.Text = "Library Export";
            tpAutoExportLibrary.UseVisualStyleBackColor = true;
            // 
            // chkRestrictMissingExportsToFullScans
            // 
            chkRestrictMissingExportsToFullScans.AutoSize = true;
            chkRestrictMissingExportsToFullScans.Location = new Point(10, 67);
            chkRestrictMissingExportsToFullScans.Name = "chkRestrictMissingExportsToFullScans";
            chkRestrictMissingExportsToFullScans.Size = new Size(219, 19);
            chkRestrictMissingExportsToFullScans.TabIndex = 45;
            chkRestrictMissingExportsToFullScans.Text = "Restrict Missing Exports to Full Scans";
            chkRestrictMissingExportsToFullScans.UseVisualStyleBackColor = true;
            // 
            // pbuShowExport
            // 
            pbuShowExport.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbuShowExport.Cursor = Cursors.Hand;
            pbuShowExport.Image = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbuShowExport.InitialImage = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbuShowExport.Location = new Point(418, 18);
            pbuShowExport.Margin = new Padding(4, 3, 4, 3);
            pbuShowExport.Name = "pbuShowExport";
            pbuShowExport.Size = new Size(50, 46);
            pbuShowExport.SizeMode = PictureBoxSizeMode.CenterImage;
            pbuShowExport.TabIndex = 44;
            pbuShowExport.TabStop = false;
            pbuShowExport.Click += pbuShowExport_Click;
            // 
            // label89
            // 
            label89.AutoSize = true;
            label89.Location = new Point(5, 13);
            label89.Margin = new Padding(4, 0, 4, 0);
            label89.Name = "label89";
            label89.Size = new Size(257, 30);
            label89.TabIndex = 43;
            label89.Text = "TV Rename can export information about\r\nshows, movies and episodes in various formats.";
            // 
            // groupBox26
            // 
            groupBox26.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox26.Controls.Add(bnBrowseMoviesHTML);
            groupBox26.Controls.Add(cbMoviesHTML);
            groupBox26.Controls.Add(txtMoviesHTMLTo);
            groupBox26.Controls.Add(bnBrowseMoviesTXT);
            groupBox26.Controls.Add(cbMoviesTXT);
            groupBox26.Controls.Add(txtMoviesTXTTo);
            groupBox26.Location = new Point(12, 366);
            groupBox26.Margin = new Padding(4, 3, 4, 3);
            groupBox26.Name = "groupBox26";
            groupBox26.Padding = new Padding(4, 3, 4, 3);
            groupBox26.Size = new Size(460, 83);
            groupBox26.TabIndex = 10;
            groupBox26.TabStop = false;
            groupBox26.Text = "All Movies";
            // 
            // bnBrowseMoviesHTML
            // 
            bnBrowseMoviesHTML.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnBrowseMoviesHTML.Location = new Point(363, 52);
            bnBrowseMoviesHTML.Margin = new Padding(4, 3, 4, 3);
            bnBrowseMoviesHTML.Name = "bnBrowseMoviesHTML";
            bnBrowseMoviesHTML.Size = new Size(88, 27);
            bnBrowseMoviesHTML.TabIndex = 8;
            bnBrowseMoviesHTML.Text = "Browse...";
            bnBrowseMoviesHTML.UseVisualStyleBackColor = true;
            bnBrowseMoviesHTML.Click += bnBrowseMoviesHTML_Click;
            // 
            // cbMoviesHTML
            // 
            cbMoviesHTML.AutoSize = true;
            cbMoviesHTML.Location = new Point(9, 57);
            cbMoviesHTML.Margin = new Padding(4, 3, 4, 3);
            cbMoviesHTML.Name = "cbMoviesHTML";
            cbMoviesHTML.Size = new Size(59, 19);
            cbMoviesHTML.TabIndex = 6;
            cbMoviesHTML.Text = "HTML";
            cbMoviesHTML.UseVisualStyleBackColor = true;
            cbMoviesHTML.CheckedChanged += EnableDisable;
            // 
            // txtMoviesHTMLTo
            // 
            txtMoviesHTMLTo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtMoviesHTMLTo.Location = new Point(75, 54);
            txtMoviesHTMLTo.Margin = new Padding(4, 3, 4, 3);
            txtMoviesHTMLTo.Name = "txtMoviesHTMLTo";
            txtMoviesHTMLTo.Size = new Size(280, 23);
            txtMoviesHTMLTo.TabIndex = 7;
            // 
            // bnBrowseMoviesTXT
            // 
            bnBrowseMoviesTXT.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnBrowseMoviesTXT.Location = new Point(363, 24);
            bnBrowseMoviesTXT.Margin = new Padding(4, 3, 4, 3);
            bnBrowseMoviesTXT.Name = "bnBrowseMoviesTXT";
            bnBrowseMoviesTXT.Size = new Size(88, 27);
            bnBrowseMoviesTXT.TabIndex = 5;
            bnBrowseMoviesTXT.Text = "Browse...";
            bnBrowseMoviesTXT.UseVisualStyleBackColor = true;
            bnBrowseMoviesTXT.Click += bnBrowseMoviesTXT_Click;
            // 
            // cbMoviesTXT
            // 
            cbMoviesTXT.AutoSize = true;
            cbMoviesTXT.Location = new Point(9, 29);
            cbMoviesTXT.Margin = new Padding(4, 3, 4, 3);
            cbMoviesTXT.Name = "cbMoviesTXT";
            cbMoviesTXT.Size = new Size(47, 19);
            cbMoviesTXT.TabIndex = 3;
            cbMoviesTXT.Text = "TXT";
            cbMoviesTXT.UseVisualStyleBackColor = true;
            cbMoviesTXT.CheckedChanged += EnableDisable;
            // 
            // txtMoviesTXTTo
            // 
            txtMoviesTXTTo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtMoviesTXTTo.Location = new Point(75, 27);
            txtMoviesTXTTo.Margin = new Padding(4, 3, 4, 3);
            txtMoviesTXTTo.Name = "txtMoviesTXTTo";
            txtMoviesTXTTo.Size = new Size(279, 23);
            txtMoviesTXTTo.TabIndex = 4;
            // 
            // groupBox7
            // 
            groupBox7.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox7.Controls.Add(bnBrowseShowsHTML);
            groupBox7.Controls.Add(cbShowsHTML);
            groupBox7.Controls.Add(txtShowsHTMLTo);
            groupBox7.Controls.Add(bnBrowseShowsTXT);
            groupBox7.Controls.Add(cbShowsTXT);
            groupBox7.Controls.Add(txtShowsTXTTo);
            groupBox7.Location = new Point(12, 184);
            groupBox7.Margin = new Padding(4, 3, 4, 3);
            groupBox7.Name = "groupBox7";
            groupBox7.Padding = new Padding(4, 3, 4, 3);
            groupBox7.Size = new Size(460, 83);
            groupBox7.TabIndex = 6;
            groupBox7.TabStop = false;
            groupBox7.Text = "All TV Shows";
            // 
            // bnBrowseShowsHTML
            // 
            bnBrowseShowsHTML.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnBrowseShowsHTML.Location = new Point(363, 52);
            bnBrowseShowsHTML.Margin = new Padding(4, 3, 4, 3);
            bnBrowseShowsHTML.Name = "bnBrowseShowsHTML";
            bnBrowseShowsHTML.Size = new Size(88, 27);
            bnBrowseShowsHTML.TabIndex = 8;
            bnBrowseShowsHTML.Text = "Browse...";
            bnBrowseShowsHTML.UseVisualStyleBackColor = true;
            bnBrowseShowsHTML.Click += bnBrowseShowsHTML_Click;
            // 
            // cbShowsHTML
            // 
            cbShowsHTML.AutoSize = true;
            cbShowsHTML.Location = new Point(9, 57);
            cbShowsHTML.Margin = new Padding(4, 3, 4, 3);
            cbShowsHTML.Name = "cbShowsHTML";
            cbShowsHTML.Size = new Size(59, 19);
            cbShowsHTML.TabIndex = 6;
            cbShowsHTML.Text = "HTML";
            cbShowsHTML.UseVisualStyleBackColor = true;
            cbShowsHTML.CheckedChanged += EnableDisable;
            // 
            // txtShowsHTMLTo
            // 
            txtShowsHTMLTo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtShowsHTMLTo.Location = new Point(75, 54);
            txtShowsHTMLTo.Margin = new Padding(4, 3, 4, 3);
            txtShowsHTMLTo.Name = "txtShowsHTMLTo";
            txtShowsHTMLTo.Size = new Size(280, 23);
            txtShowsHTMLTo.TabIndex = 7;
            // 
            // bnBrowseShowsTXT
            // 
            bnBrowseShowsTXT.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnBrowseShowsTXT.Location = new Point(363, 24);
            bnBrowseShowsTXT.Margin = new Padding(4, 3, 4, 3);
            bnBrowseShowsTXT.Name = "bnBrowseShowsTXT";
            bnBrowseShowsTXT.Size = new Size(88, 27);
            bnBrowseShowsTXT.TabIndex = 5;
            bnBrowseShowsTXT.Text = "Browse...";
            bnBrowseShowsTXT.UseVisualStyleBackColor = true;
            bnBrowseShowsTXT.Click += bnBrowseShowsTXT_Click;
            // 
            // cbShowsTXT
            // 
            cbShowsTXT.AutoSize = true;
            cbShowsTXT.Location = new Point(9, 29);
            cbShowsTXT.Margin = new Padding(4, 3, 4, 3);
            cbShowsTXT.Name = "cbShowsTXT";
            cbShowsTXT.Size = new Size(47, 19);
            cbShowsTXT.TabIndex = 3;
            cbShowsTXT.Text = "TXT";
            cbShowsTXT.UseVisualStyleBackColor = true;
            cbShowsTXT.CheckedChanged += EnableDisable;
            // 
            // txtShowsTXTTo
            // 
            txtShowsTXTTo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtShowsTXTTo.Location = new Point(75, 27);
            txtShowsTXTTo.Margin = new Padding(4, 3, 4, 3);
            txtShowsTXTTo.Name = "txtShowsTXTTo";
            txtShowsTXTTo.Size = new Size(279, 23);
            txtShowsTXTTo.TabIndex = 4;
            // 
            // groupBox27
            // 
            groupBox27.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox27.Controls.Add(bnBrowseMissingMoviesCSV);
            groupBox27.Controls.Add(bnBrowseMissingMoviesXML);
            groupBox27.Controls.Add(txtMissingMoviesCSV);
            groupBox27.Controls.Add(cbMissingMoviesXML);
            groupBox27.Controls.Add(cbMissingMoviesCSV);
            groupBox27.Controls.Add(txtMissingMoviesXML);
            groupBox27.Location = new Point(10, 274);
            groupBox27.Margin = new Padding(4, 3, 4, 3);
            groupBox27.Name = "groupBox27";
            groupBox27.Padding = new Padding(4, 3, 4, 3);
            groupBox27.Size = new Size(461, 91);
            groupBox27.TabIndex = 9;
            groupBox27.TabStop = false;
            groupBox27.Text = "Missing Movies";
            // 
            // bnBrowseMissingMoviesCSV
            // 
            bnBrowseMissingMoviesCSV.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnBrowseMissingMoviesCSV.Location = new Point(363, 54);
            bnBrowseMissingMoviesCSV.Margin = new Padding(4, 3, 4, 3);
            bnBrowseMissingMoviesCSV.Name = "bnBrowseMissingMoviesCSV";
            bnBrowseMissingMoviesCSV.Size = new Size(88, 27);
            bnBrowseMissingMoviesCSV.TabIndex = 2;
            bnBrowseMissingMoviesCSV.Text = "Browse...";
            bnBrowseMissingMoviesCSV.UseVisualStyleBackColor = true;
            bnBrowseMissingMoviesCSV.Click += bnBrowseMissingMoviesCSV_Click;
            // 
            // bnBrowseMissingMoviesXML
            // 
            bnBrowseMissingMoviesXML.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnBrowseMissingMoviesXML.Location = new Point(364, 22);
            bnBrowseMissingMoviesXML.Margin = new Padding(4, 3, 4, 3);
            bnBrowseMissingMoviesXML.Name = "bnBrowseMissingMoviesXML";
            bnBrowseMissingMoviesXML.Size = new Size(88, 27);
            bnBrowseMissingMoviesXML.TabIndex = 5;
            bnBrowseMissingMoviesXML.Text = "Browse...";
            bnBrowseMissingMoviesXML.UseVisualStyleBackColor = true;
            bnBrowseMissingMoviesXML.Click += bnBrowseMissingMoviesXML_Click;
            // 
            // txtMissingMoviesCSV
            // 
            txtMissingMoviesCSV.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtMissingMoviesCSV.Location = new Point(75, 55);
            txtMissingMoviesCSV.Margin = new Padding(4, 3, 4, 3);
            txtMissingMoviesCSV.Name = "txtMissingMoviesCSV";
            txtMissingMoviesCSV.Size = new Size(280, 23);
            txtMissingMoviesCSV.TabIndex = 1;
            // 
            // cbMissingMoviesXML
            // 
            cbMissingMoviesXML.AutoSize = true;
            cbMissingMoviesXML.Location = new Point(9, 27);
            cbMissingMoviesXML.Margin = new Padding(4, 3, 4, 3);
            cbMissingMoviesXML.Name = "cbMissingMoviesXML";
            cbMissingMoviesXML.Size = new Size(50, 19);
            cbMissingMoviesXML.TabIndex = 3;
            cbMissingMoviesXML.Text = "XML";
            cbMissingMoviesXML.UseVisualStyleBackColor = true;
            cbMissingMoviesXML.CheckedChanged += EnableDisable;
            // 
            // cbMissingMoviesCSV
            // 
            cbMissingMoviesCSV.AutoSize = true;
            cbMissingMoviesCSV.Location = new Point(10, 55);
            cbMissingMoviesCSV.Margin = new Padding(4, 3, 4, 3);
            cbMissingMoviesCSV.Name = "cbMissingMoviesCSV";
            cbMissingMoviesCSV.Size = new Size(47, 19);
            cbMissingMoviesCSV.TabIndex = 0;
            cbMissingMoviesCSV.Text = "CSV";
            cbMissingMoviesCSV.UseVisualStyleBackColor = true;
            cbMissingMoviesCSV.CheckedChanged += EnableDisable;
            // 
            // txtMissingMoviesXML
            // 
            txtMissingMoviesXML.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtMissingMoviesXML.Location = new Point(75, 24);
            txtMissingMoviesXML.Margin = new Padding(4, 3, 4, 3);
            txtMissingMoviesXML.Name = "txtMissingMoviesXML";
            txtMissingMoviesXML.Size = new Size(280, 23);
            txtMissingMoviesXML.TabIndex = 4;
            // 
            // groupBox3
            // 
            groupBox3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox3.Controls.Add(bnBrowseMissingCSV);
            groupBox3.Controls.Add(bnBrowseMissingXML);
            groupBox3.Controls.Add(txtMissingCSV);
            groupBox3.Controls.Add(cbMissingXML);
            groupBox3.Controls.Add(cbMissingCSV);
            groupBox3.Controls.Add(txtMissingXML);
            groupBox3.Location = new Point(10, 92);
            groupBox3.Margin = new Padding(4, 3, 4, 3);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(4, 3, 4, 3);
            groupBox3.Size = new Size(461, 91);
            groupBox3.TabIndex = 5;
            groupBox3.TabStop = false;
            groupBox3.Text = "Missing Episodes";
            // 
            // bnBrowseMissingCSV
            // 
            bnBrowseMissingCSV.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnBrowseMissingCSV.Location = new Point(363, 54);
            bnBrowseMissingCSV.Margin = new Padding(4, 3, 4, 3);
            bnBrowseMissingCSV.Name = "bnBrowseMissingCSV";
            bnBrowseMissingCSV.Size = new Size(88, 27);
            bnBrowseMissingCSV.TabIndex = 2;
            bnBrowseMissingCSV.Text = "Browse...";
            bnBrowseMissingCSV.UseVisualStyleBackColor = true;
            bnBrowseMissingCSV.Click += bnBrowseMissingCSV_Click;
            // 
            // bnBrowseMissingXML
            // 
            bnBrowseMissingXML.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnBrowseMissingXML.Location = new Point(364, 22);
            bnBrowseMissingXML.Margin = new Padding(4, 3, 4, 3);
            bnBrowseMissingXML.Name = "bnBrowseMissingXML";
            bnBrowseMissingXML.Size = new Size(88, 27);
            bnBrowseMissingXML.TabIndex = 5;
            bnBrowseMissingXML.Text = "Browse...";
            bnBrowseMissingXML.UseVisualStyleBackColor = true;
            bnBrowseMissingXML.Click += bnBrowseMissingXML_Click;
            // 
            // txtMissingCSV
            // 
            txtMissingCSV.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtMissingCSV.Location = new Point(75, 55);
            txtMissingCSV.Margin = new Padding(4, 3, 4, 3);
            txtMissingCSV.Name = "txtMissingCSV";
            txtMissingCSV.Size = new Size(280, 23);
            txtMissingCSV.TabIndex = 1;
            // 
            // cbMissingXML
            // 
            cbMissingXML.AutoSize = true;
            cbMissingXML.Location = new Point(9, 27);
            cbMissingXML.Margin = new Padding(4, 3, 4, 3);
            cbMissingXML.Name = "cbMissingXML";
            cbMissingXML.Size = new Size(50, 19);
            cbMissingXML.TabIndex = 3;
            cbMissingXML.Text = "XML";
            cbMissingXML.UseVisualStyleBackColor = true;
            cbMissingXML.CheckedChanged += EnableDisable;
            // 
            // cbMissingCSV
            // 
            cbMissingCSV.AutoSize = true;
            cbMissingCSV.Location = new Point(10, 55);
            cbMissingCSV.Margin = new Padding(4, 3, 4, 3);
            cbMissingCSV.Name = "cbMissingCSV";
            cbMissingCSV.Size = new Size(47, 19);
            cbMissingCSV.TabIndex = 0;
            cbMissingCSV.Text = "CSV";
            cbMissingCSV.UseVisualStyleBackColor = true;
            cbMissingCSV.CheckedChanged += EnableDisable;
            // 
            // txtMissingXML
            // 
            txtMissingXML.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtMissingXML.Location = new Point(75, 24);
            txtMissingXML.Margin = new Padding(4, 3, 4, 3);
            txtMissingXML.Name = "txtMissingXML";
            txtMissingXML.Size = new Size(280, 23);
            txtMissingXML.TabIndex = 4;
            // 
            // tbAppUpdate
            // 
            tbAppUpdate.Controls.Add(pbuUpdates);
            tbAppUpdate.Controls.Add(chkUpdateCheckEnabled);
            tbAppUpdate.Controls.Add(label92);
            tbAppUpdate.Controls.Add(grpUpdateIntervalOption);
            tbAppUpdate.Location = new Point(149, 4);
            tbAppUpdate.Margin = new Padding(4, 3, 4, 3);
            tbAppUpdate.Name = "tbAppUpdate";
            tbAppUpdate.Size = new Size(500, 684);
            tbAppUpdate.TabIndex = 20;
            tbAppUpdate.Text = "App Updates";
            tbAppUpdate.UseVisualStyleBackColor = true;
            // 
            // pbuUpdates
            // 
            pbuUpdates.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbuUpdates.Cursor = Cursors.Hand;
            pbuUpdates.Image = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbuUpdates.InitialImage = Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823;
            pbuUpdates.Location = new Point(416, 16);
            pbuUpdates.Margin = new Padding(4, 3, 4, 3);
            pbuUpdates.Name = "pbuUpdates";
            pbuUpdates.Size = new Size(50, 46);
            pbuUpdates.SizeMode = PictureBoxSizeMode.CenterImage;
            pbuUpdates.TabIndex = 46;
            pbuUpdates.TabStop = false;
            pbuUpdates.Click += pbuUpdates_Click;
            // 
            // chkUpdateCheckEnabled
            // 
            chkUpdateCheckEnabled.AutoSize = true;
            chkUpdateCheckEnabled.BackColor = Color.White;
            chkUpdateCheckEnabled.Location = new Point(13, 70);
            chkUpdateCheckEnabled.Margin = new Padding(4, 3, 4, 3);
            chkUpdateCheckEnabled.Name = "chkUpdateCheckEnabled";
            chkUpdateCheckEnabled.Size = new Size(123, 19);
            chkUpdateCheckEnabled.TabIndex = 0;
            chkUpdateCheckEnabled.Text = "Check for Updates";
            chkUpdateCheckEnabled.UseVisualStyleBackColor = false;
            chkUpdateCheckEnabled.CheckedChanged += chkUpdateCheckEnabled_CheckedChanged;
            // 
            // label92
            // 
            label92.AutoSize = true;
            label92.Location = new Point(4, 10);
            label92.Margin = new Padding(4, 0, 4, 0);
            label92.Name = "label92";
            label92.Size = new Size(250, 30);
            label92.TabIndex = 45;
            label92.Text = "Define how TV Rename alerts users about new\r\nversions being available";
            // 
            // grpUpdateIntervalOption
            // 
            grpUpdateIntervalOption.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpUpdateIntervalOption.Controls.Add(chkNoPopupOnUpdate);
            grpUpdateIntervalOption.Controls.Add(cboUpdateCheckInterval);
            grpUpdateIntervalOption.Controls.Add(optUpdateCheckInterval);
            grpUpdateIntervalOption.Controls.Add(optUpdateCheckAlways);
            grpUpdateIntervalOption.Location = new Point(13, 82);
            grpUpdateIntervalOption.Margin = new Padding(4, 3, 4, 3);
            grpUpdateIntervalOption.Name = "grpUpdateIntervalOption";
            grpUpdateIntervalOption.Padding = new Padding(4, 3, 4, 3);
            grpUpdateIntervalOption.Size = new Size(454, 143);
            grpUpdateIntervalOption.TabIndex = 1;
            grpUpdateIntervalOption.TabStop = false;
            // 
            // chkNoPopupOnUpdate
            // 
            chkNoPopupOnUpdate.AutoSize = true;
            chkNoPopupOnUpdate.Location = new Point(8, 107);
            chkNoPopupOnUpdate.Margin = new Padding(4, 3, 4, 3);
            chkNoPopupOnUpdate.Name = "chkNoPopupOnUpdate";
            chkNoPopupOnUpdate.Size = new Size(226, 19);
            chkNoPopupOnUpdate.TabIndex = 2;
            chkNoPopupOnUpdate.Text = "No dialog when an update is available";
            chkNoPopupOnUpdate.UseVisualStyleBackColor = true;
            // 
            // cboUpdateCheckInterval
            // 
            cboUpdateCheckInterval.DropDownStyle = ComboBoxStyle.DropDownList;
            cboUpdateCheckInterval.FormattingEnabled = true;
            cboUpdateCheckInterval.Location = new Point(8, 68);
            cboUpdateCheckInterval.Margin = new Padding(4, 3, 4, 3);
            cboUpdateCheckInterval.Name = "cboUpdateCheckInterval";
            cboUpdateCheckInterval.Size = new Size(193, 23);
            cboUpdateCheckInterval.TabIndex = 2;
            // 
            // optUpdateCheckInterval
            // 
            optUpdateCheckInterval.AutoSize = true;
            optUpdateCheckInterval.Location = new Point(8, 40);
            optUpdateCheckInterval.Margin = new Padding(4, 3, 4, 3);
            optUpdateCheckInterval.Name = "optUpdateCheckInterval";
            optUpdateCheckInterval.Size = new Size(121, 19);
            optUpdateCheckInterval.TabIndex = 1;
            optUpdateCheckInterval.TabStop = true;
            optUpdateCheckInterval.Text = "at certain intervals";
            optUpdateCheckInterval.UseVisualStyleBackColor = true;
            optUpdateCheckInterval.CheckedChanged += updateCheckOption_CheckedChanged;
            // 
            // optUpdateCheckAlways
            // 
            optUpdateCheckAlways.AutoSize = true;
            optUpdateCheckAlways.Location = new Point(8, 13);
            optUpdateCheckAlways.Margin = new Padding(4, 3, 4, 3);
            optUpdateCheckAlways.Name = "optUpdateCheckAlways";
            optUpdateCheckAlways.Size = new Size(96, 19);
            optUpdateCheckAlways.TabIndex = 0;
            optUpdateCheckAlways.TabStop = true;
            optUpdateCheckAlways.Text = "on every start";
            optUpdateCheckAlways.UseVisualStyleBackColor = true;
            optUpdateCheckAlways.CheckedChanged += updateCheckOption_CheckedChanged;
            // 
            // Preferences
            // 
            AcceptButton = OKButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = bnCancel;
            ClientSize = new Size(681, 743);
            ControlBox = false;
            Controls.Add(tcTabs);
            Controls.Add(bnCancel);
            Controls.Add(OKButton);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(697, 753);
            Name = "Preferences";
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Preferences";
            FormClosing += Preferences_FormClosing;
            Load += Preferences_Load;
            cmDefaults.ResumeLayout(false);
            tpDisplay.ResumeLayout(false);
            tpDisplay.PerformLayout();
            groupBox11.ResumeLayout(false);
            groupBox11.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbDisplay).EndInit();
            tpRSSJSONSearch.ResumeLayout(false);
            tpRSSJSONSearch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbRSSJSONSearch).EndInit();
            gbJSON.ResumeLayout(false);
            gbJSON.PerformLayout();
            gbRSS.ResumeLayout(false);
            gbRSS.PerformLayout();
            tpLibraryFolders.ResumeLayout(false);
            tpLibraryFolders.PerformLayout();
            groupBox23.ResumeLayout(false);
            groupBox23.PerformLayout();
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbLibraryFolders).EndInit();
            tpTorrentNZB.ResumeLayout(false);
            tpTorrentNZB.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbuTorrentNZB).EndInit();
            qBitTorrent.ResumeLayout(false);
            qBitTorrent.PerformLayout();
            gbSAB.ResumeLayout(false);
            gbSAB.PerformLayout();
            gbuTorrent.ResumeLayout(false);
            gbuTorrent.PerformLayout();
            tbSearchFolders.ResumeLayout(false);
            tbSearchFolders.PerformLayout();
            groupBox8.ResumeLayout(false);
            groupBox8.PerformLayout();
            gbAutoAdd.ResumeLayout(false);
            gbAutoAdd.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbSearchFolders).EndInit();
            tbMediaCenter.ResumeLayout(false);
            tbMediaCenter.PerformLayout();
            groupBox16.ResumeLayout(false);
            groupBox16.PerformLayout();
            groupBox13.ResumeLayout(false);
            groupBox13.PerformLayout();
            groupBox14.ResumeLayout(false);
            groupBox14.PerformLayout();
            groupBox15.ResumeLayout(false);
            groupBox15.PerformLayout();
            groupBox12.ResumeLayout(false);
            groupBox12.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbMediaCenter).EndInit();
            tbFolderDeleting.ResumeLayout(false);
            tbFolderDeleting.PerformLayout();
            groupBox28.ResumeLayout(false);
            groupBox28.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbFolderDeleting).EndInit();
            tbAutoExport.ResumeLayout(false);
            tbAutoExport.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbuExportEpisodes).EndInit();
            groupBox10.ResumeLayout(false);
            groupBox10.PerformLayout();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            tbFilesAndFolders.ResumeLayout(false);
            tbFilesAndFolders.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbFilesAndFolders).EndInit();
            tbGeneral.ResumeLayout(false);
            tbGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbGeneral).EndInit();
            tcTabs.ResumeLayout(false);
            tpDataSources.ResumeLayout(false);
            tpDataSources.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            gbTMDB.ResumeLayout(false);
            gbTMDB.PerformLayout();
            groupBox20.ResumeLayout(false);
            groupBox20.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbSources).EndInit();
            tpMovieDefaults.ResumeLayout(false);
            tpMovieDefaults.PerformLayout();
            groupBox24.ResumeLayout(false);
            groupBox24.PerformLayout();
            groupBox25.ResumeLayout(false);
            groupBox25.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbMovieDefaults).EndInit();
            tpShowDefaults.ResumeLayout(false);
            tpShowDefaults.PerformLayout();
            groupBox19.ResumeLayout(false);
            groupBox19.PerformLayout();
            groupBox18.ResumeLayout(false);
            groupBox18.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            tpScanSettings.ResumeLayout(false);
            tpScanSettings.PerformLayout();
            groupBox17.ResumeLayout(false);
            groupBox17.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            gbBulkAdd.ResumeLayout(false);
            gbBulkAdd.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbScanOptions).EndInit();
            tpSubtitles.ResumeLayout(false);
            tpSubtitles.PerformLayout();
            groupBox29.ResumeLayout(false);
            groupBox29.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            groupBox9.ResumeLayout(false);
            groupBox9.PerformLayout();
            tpJackett.ResumeLayout(false);
            tpJackett.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbuJackett).EndInit();
            groupBox22.ResumeLayout(false);
            groupBox22.PerformLayout();
            tpAutoExportLibrary.ResumeLayout(false);
            tpAutoExportLibrary.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbuShowExport).EndInit();
            groupBox26.ResumeLayout(false);
            groupBox26.PerformLayout();
            groupBox7.ResumeLayout(false);
            groupBox7.PerformLayout();
            groupBox27.ResumeLayout(false);
            groupBox27.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            tbAppUpdate.ResumeLayout(false);
            tbAppUpdate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbuUpdates).EndInit();
            grpUpdateIntervalOption.ResumeLayout(false);
            grpUpdateIntervalOption.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.SaveFileDialog saveFile;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
        private System.Windows.Forms.OpenFileDialog openFile;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.ContextMenuStrip cmDefaults;
        private System.Windows.Forms.ToolStripMenuItem KODIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pyTivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mede8erToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem;
        private System.Windows.Forms.TabPage tpDisplay;
        private System.Windows.Forms.CheckBox chkHideWtWSpoilers;
        private System.Windows.Forms.CheckBox chkHideMyShowsSpoilers;
        private System.Windows.Forms.RadioButton rbWTWScan;
        private System.Windows.Forms.RadioButton rbWTWSearch;
        private System.Windows.Forms.ComboBox cbStartupTab;
        private System.Windows.Forms.CheckBox cbAutoSelInMyShows;
        private System.Windows.Forms.CheckBox cbShowEpisodePictures;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkShowInTaskbar;
        private System.Windows.Forms.CheckBox cbNotificationIcon;
        private System.Windows.Forms.TabPage tpRSSJSONSearch;
        private System.Windows.Forms.GroupBox gbJSON;
        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.TextBox tbJSONFilenameToken;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.TextBox tbJSONURLToken;
        private System.Windows.Forms.Label label49;
        private System.Windows.Forms.TextBox tbJSONRootNode;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.TextBox tbJSONURL;
        private System.Windows.Forms.GroupBox gbRSS;
        private SourceGrid.Grid RSSGrid;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Button bnRSSRemove;
        private System.Windows.Forms.Button bnRSSGo;
        private System.Windows.Forms.Button bnRSSAdd;
        private System.Windows.Forms.TabPage tpLibraryFolders;
        private System.Windows.Forms.TabPage tpTorrentNZB;
        private System.Windows.Forms.GroupBox qBitTorrent;
        private System.Windows.Forms.TextBox tbqBitTorrentHost;
        private System.Windows.Forms.TextBox tbqBitTorrentPort;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.GroupBox gbSAB;
        private System.Windows.Forms.TextBox txtSABHostPort;
        private System.Windows.Forms.TextBox txtSABAPIKey;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox gbuTorrent;
        private System.Windows.Forms.Button bnUTBrowseResumeDat;
        private System.Windows.Forms.TextBox txtUTResumeDatPath;
        private System.Windows.Forms.Button bnRSSBrowseuTorrent;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox txtRSSuTorrentPath;
        private System.Windows.Forms.TabPage tbSearchFolders;
        private System.Windows.Forms.CheckBox cbMonitorFolder;
        private System.Windows.Forms.Button bnOpenSearchFolder;
        private System.Windows.Forms.Button bnRemoveSearchFolder;
        private System.Windows.Forms.Button bnAddSearchFolder;
        private System.Windows.Forms.ListBox lbSearchFolders;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TabPage tbMediaCenter;
        private System.Windows.Forms.CheckBox cbWDLiveEpisodeFiles;
        private System.Windows.Forms.CheckBox cbNFOEpisodes;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbFolderBanner;
        private System.Windows.Forms.RadioButton rbFolderPoster;
        private System.Windows.Forms.RadioButton rbFolderFanArt;
        private System.Windows.Forms.RadioButton rbFolderSeasonPoster;
        private System.Windows.Forms.CheckBox cbKODIImages;
        private System.Windows.Forms.Button bnMCPresets;
        private System.Windows.Forms.CheckBox cbShrinkLarge;
        private System.Windows.Forms.CheckBox cbEpThumbJpg;
        private System.Windows.Forms.CheckBox cbMetaSubfolder;
        private System.Windows.Forms.CheckBox cbMeta;
        private System.Windows.Forms.CheckBox cbEpTBNs;
        private System.Windows.Forms.CheckBox cbSeriesJpg;
        private System.Windows.Forms.CheckBox cbXMLFiles;
        private System.Windows.Forms.CheckBox cbNFOShows;
        private System.Windows.Forms.CheckBox cbFantArtJpg;
        private System.Windows.Forms.CheckBox cbFolderJpg;
        private System.Windows.Forms.TabPage tbFolderDeleting;
        private System.Windows.Forms.CheckBox cbCleanUpDownloadDir;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.TextBox txtEmptyMaxSize;
        private System.Windows.Forms.TextBox txtEmptyIgnoreWords;
        private System.Windows.Forms.TextBox txtEmptyIgnoreExtensions;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.CheckBox cbRecycleNotDelete;
        private System.Windows.Forms.CheckBox cbEmptyMaxSize;
        private System.Windows.Forms.CheckBox cbEmptyIgnoreWords;
        private System.Windows.Forms.CheckBox cbEmptyIgnoreExtensions;
        private System.Windows.Forms.CheckBox cbDeleteEmpty;
        private System.Windows.Forms.TabPage tbAutoExport;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button bnBrowseFOXML;
        private System.Windows.Forms.CheckBox cbFOXML;
        private System.Windows.Forms.TextBox txtFOXML;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button bnBrowseRenamingXML;
        private System.Windows.Forms.CheckBox cbRenamingXML;
        private System.Windows.Forms.TextBox txtRenamingXML;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button bnBrowseWTWICAL;
        private System.Windows.Forms.TextBox txtWTWICAL;
        private System.Windows.Forms.CheckBox cbWTWICAL;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtExportRSSDaysPast;
        private System.Windows.Forms.Button bnBrowseWTWXML;
        private System.Windows.Forms.TextBox txtWTWXML;
        private System.Windows.Forms.CheckBox cbWTWXML;
        private System.Windows.Forms.Button bnBrowseWTWRSS;
        private System.Windows.Forms.TextBox txtWTWRSS;
        private System.Windows.Forms.CheckBox cbWTWRSS;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtExportRSSMaxDays;
        private System.Windows.Forms.TextBox txtExportRSSMaxShows;
        private System.Windows.Forms.TabPage tbFilesAndFolders;
        private System.Windows.Forms.TextBox txtKeepTogether;
        private System.Windows.Forms.TextBox txtMaxSampleSize;
        private System.Windows.Forms.TextBox txtOtherExtensions;
        private System.Windows.Forms.TextBox txtVideoExtensions;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.ComboBox cbKeepTogetherMode;
        private System.Windows.Forms.Button bnReplaceRemove;
        private System.Windows.Forms.Button bnReplaceAdd;
        private System.Windows.Forms.Label label3;
        private SourceGrid.Grid ReplacementsGrid;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox cbKeepTogether;
        private System.Windows.Forms.CheckBox cbForceLower;
        private System.Windows.Forms.CheckBox cbIgnoreSamples;
        private System.Windows.Forms.TabPage tbGeneral;
        private System.Windows.Forms.TextBox txtWTWDays;
        private System.Windows.Forms.ComboBox cbMode;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl tcTabs;
        private System.Windows.Forms.TextBox txtSeasonFolderName;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.CheckBox cbLeadingZero;
        private System.Windows.Forms.CheckBox cbDeleteShowFromDisk;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.Button bnBrowseASX;
        private System.Windows.Forms.TextBox txtASX;
        private System.Windows.Forms.CheckBox cbASX;
        private System.Windows.Forms.Button bnBrowseM3U;
        private System.Windows.Forms.TextBox txtM3U;
        private System.Windows.Forms.CheckBox cbM3U;
        private System.Windows.Forms.Button bnBrowseXSPF;
        private System.Windows.Forms.TextBox txtXSPF;
        private System.Windows.Forms.CheckBox cbXSPF;
        private System.Windows.Forms.Button bnBrowseWPL;
        private System.Windows.Forms.TextBox txtWPL;
        private System.Windows.Forms.CheckBox cbWPL;
        private System.Windows.Forms.Label label55;
        private System.Windows.Forms.TextBox tbJSONFilesizeToken;
        private System.Windows.Forms.CheckBox cbSearchRSSManualScanOnly;
        private System.Windows.Forms.CheckBox cbSearchJSON;
        private System.Windows.Forms.CheckBox cbSearchRSS;
        private System.Windows.Forms.CheckBox cbSearchJSONManualScanOnly;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.CheckBox cbCheckqBitTorrent;
        private System.Windows.Forms.CheckBox cbCheckSABnzbd;
        private System.Windows.Forms.CheckBox cbCheckuTorrent;
        private System.Windows.Forms.Label label59;
        private System.Windows.Forms.CheckBox cbHigherQuality;
        private System.Windows.Forms.CheckBox chkAutoMergeDownloadEpisodes;
        private System.Windows.Forms.Label label56;
        private System.Windows.Forms.Button bnOpenMonFolder;
        private System.Windows.Forms.Button bnAddMonFolder;
        private System.Windows.Forms.Button bnRemoveMonFolder;
        private System.Windows.Forms.ListBox lstFMMonitorFolders;
        private System.Windows.Forms.GroupBox gbAutoAdd;
        private System.Windows.Forms.CheckBox chkAutoSearchForDownloadedFiles;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.TextBox tbIgnoreSuffixes;
        private System.Windows.Forms.TextBox tbMovieTerms;
        private System.Windows.Forms.CheckBox cbLeaveOriginals;
        private System.Windows.Forms.CheckBox cbSearchLocally;
        private System.Windows.Forms.PictureBox pbuTorrentNZB;
        private System.Windows.Forms.PictureBox pbGeneral;
        private System.Windows.Forms.Label label61;
        private System.Windows.Forms.PictureBox pbDisplay;
        private System.Windows.Forms.PictureBox pbRSSJSONSearch;
        private System.Windows.Forms.PictureBox pbLibraryFolders;
        private System.Windows.Forms.PictureBox pbSearchFolders;
        private System.Windows.Forms.Label label64;
        private System.Windows.Forms.PictureBox pbMediaCenter;
        private System.Windows.Forms.PictureBox pbFolderDeleting;
        private System.Windows.Forms.PictureBox pbFilesAndFolders;
        private System.Windows.Forms.Label label60;
        private System.Windows.Forms.Label label65;
        private System.Windows.Forms.Label label67;
        private System.Windows.Forms.Label label66;
        private System.Windows.Forms.Label label69;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.Label label54;
        private System.Windows.Forms.TextBox tbPercentBetter;
        private System.Windows.Forms.TextBox tbPriorityOverrideTerms;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cboShowStatus;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtShowStatusColor;
        private System.Windows.Forms.Button btnSelectColor;
        private System.Windows.Forms.Button bnRemoveDefinedColor;
        private System.Windows.Forms.Button btnAddShowStatusColoring;
        private System.Windows.Forms.ListView lvwDefinedColors;
        private System.Windows.Forms.ColumnHeader colShowStatus;
        private System.Windows.Forms.ColumnHeader colColor;
        private System.Windows.Forms.GroupBox groupBox16;
        private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.GroupBox groupBox14;
        private System.Windows.Forms.GroupBox groupBox15;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.CheckBox cbCopyFutureDatedEps;
        private System.Windows.Forms.CheckBox chkShareCriticalLogs;
        private System.Windows.Forms.CheckBox chkPostpendThe;
        private System.Windows.Forms.CheckBox chkBasicShowDetails;
        private System.Windows.Forms.CheckBox chkUseSearchFullPathWhenMatchingShows;
        private System.Windows.Forms.CheckBox chkUseLibraryFullPathWhenMatchingShows;
        private System.Windows.Forms.CheckBox chkAutoAddAsPartOfQuickRename;
        private System.Windows.Forms.CheckBox cbJSONCloudflareProtection;
        private System.Windows.Forms.CheckBox cbDownloadTorrentBeforeDownloading;
        private System.Windows.Forms.CheckBox cbRSSCloudflareProtection;
        private System.Windows.Forms.TabPage tpShowDefaults;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox19;
        private System.Windows.Forms.RadioButton rbDefShowUseSubFolders;
        private System.Windows.Forms.RadioButton rbDefShowUseBase;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cmbDefShowLocation;
        private System.Windows.Forms.CheckBox cbDefShowUseDefLocation;
        private System.Windows.Forms.CheckBox cbDefShowAutoFolders;
        private System.Windows.Forms.GroupBox groupBox18;
        private System.Windows.Forms.CheckBox cbDefShowSpecialsCount;
        private System.Windows.Forms.CheckBox cbDefShowSequentialMatching;
        private System.Windows.Forms.CheckBox cbDefShowIncludeNoAirdate;
        private System.Windows.Forms.CheckBox cbDefShowDoMissingCheck;
        private System.Windows.Forms.CheckBox cbDefShowIncludeFuture;
        private System.Windows.Forms.CheckBox cbDefShowDoRenaming;
        private System.Windows.Forms.CheckBox cbDefShowNextAirdate;
        private System.Windows.Forms.CheckBox cbDefShowDVDOrder;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.ComboBox cbTimeZone;
        private System.Windows.Forms.CheckBox cbUseColoursOnWtw;
        private System.Windows.Forms.RadioButton rdoqBitTorrentAPIVersionv1;
        private System.Windows.Forms.RadioButton rdoqBitTorrentAPIVersionv0;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.RadioButton rdoqBitTorrentAPIVersionv2;
        private System.Windows.Forms.TabPage tpDataSources;
        private System.Windows.Forms.GroupBox groupBox21;
        private System.Windows.Forms.GroupBox groupBox20;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cbTVDBLanguages;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.RadioButton rdoTVTVMaze;
        private System.Windows.Forms.RadioButton rdoTVTVDB;
        private System.Windows.Forms.TextBox txtParallelDownloads;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.TextBox tbPercentDirty;
        private System.Windows.Forms.Label label57;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.DomainUpDown domainUpDown2;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtSeasonFormat;
        private System.Windows.Forms.TextBox txtSpecialsFolderName;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label63;
        private System.Windows.Forms.PictureBox pbSources;
        private System.Windows.Forms.TabPage tpScanSettings;
        private System.Windows.Forms.GroupBox groupBox17;
        private System.Windows.Forms.CheckBox chkIgnoreAllSpecials;
        private System.Windows.Forms.CheckBox chkMoveLibraryFiles;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DomainUpDown upDownScanHours;
        private System.Windows.Forms.CheckBox chkScheduledScan;
        private System.Windows.Forms.CheckBox chkScanOnStartup;
        private System.Windows.Forms.Label lblScanAction;
        private System.Windows.Forms.RadioButton rdoQuickScan;
        private System.Windows.Forms.RadioButton rdoRecentScan;
        private System.Windows.Forms.RadioButton rdoFullScan;
        private System.Windows.Forms.CheckBox cbIgnorePreviouslySeen;
        private System.Windows.Forms.CheckBox chkPreventMove;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.CheckBox cbRenameCheck;
        private System.Windows.Forms.CheckBox cbMissing;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkChooseWhenMultipleEpisodesMatch;
        private System.Windows.Forms.CheckBox cbxUpdateAirDate;
        private System.Windows.Forms.CheckBox cbAutoCreateFolders;
        private System.Windows.Forms.CheckBox chkAutoMergeLibraryEpisodes;
        private System.Windows.Forms.CheckBox cbScanIncludesBulkAdd;
        private System.Windows.Forms.GroupBox gbBulkAdd;
        private System.Windows.Forms.TextBox tbSeasonSearchTerms;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.CheckBox chkForceBulkAddToUseSettingsOnly;
        private System.Windows.Forms.CheckBox cbIgnoreRecycleBin;
        private System.Windows.Forms.CheckBox cbIgnoreNoVideoFolders;
        private System.Windows.Forms.Label label62;
        private System.Windows.Forms.PictureBox pbScanOptions;
        private System.Windows.Forms.CheckBox cbDefShowEpNameMatching;
        private System.Windows.Forms.Label label68;
        private System.Windows.Forms.CheckBox cbDefShowAirdateMatching;
        private System.Windows.Forms.CheckBox chkShowAccessibilityOptions;
        private System.Windows.Forms.TabPage tpJackett;
        private System.Windows.Forms.Label label77;
        private System.Windows.Forms.ComboBox cmbUnattendedDuplicateAction;
        private System.Windows.Forms.Label label76;
        private System.Windows.Forms.CheckBox cbDetailedRSSJSONLogging;
        private System.Windows.Forms.PictureBox pbuJackett;
        private System.Windows.Forms.Label label70;
        private System.Windows.Forms.CheckBox cbSearchJackett;
        private System.Windows.Forms.GroupBox groupBox22;
        private System.Windows.Forms.CheckBox cbSearchJackettOnManualScansOnly;
        private System.Windows.Forms.Label label72;
        private System.Windows.Forms.TextBox txtJackettIndexer;
        private System.Windows.Forms.Label label73;
        private System.Windows.Forms.TextBox txtJackettAPIKey;
        private System.Windows.Forms.Label label74;
        private System.Windows.Forms.TextBox txtJackettPort;
        private System.Windows.Forms.Label label75;
        private System.Windows.Forms.TextBox txtJackettServer;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.TextBox tbPreferredRSSTerms;
        private System.Windows.Forms.ComboBox cmbSupervisedDuplicateAction;
        private System.Windows.Forms.Label label71;
        private System.Windows.Forms.LinkLabel llJackettLink;
        private System.Windows.Forms.Label label78;
        private System.Windows.Forms.TextBox tbJSONSeedersToken;
        private System.Windows.Forms.CheckBox chkRemoveCompletedTorrents;
        private System.Windows.Forms.LinkLabel llqBitTorrentLink;
        private System.Windows.Forms.Label label79;
        private System.Windows.Forms.CheckBox chkSearchJackettButton;
        private System.Windows.Forms.CheckBox chkSkipJackettFullScans;
        private System.Windows.Forms.CheckBox cbAutoSaveOnExit;
        private System.Windows.Forms.CheckBox chkUseJackettTextSearch;
        private System.Windows.Forms.Label label83;
        private System.Windows.Forms.RadioButton rdoMovieTMDB;
        private System.Windows.Forms.RadioButton rdoMovieTheTVDB;
        private System.Windows.Forms.GroupBox gbTMDB;
        private System.Windows.Forms.Label label80;
        private System.Windows.Forms.Label label81;
        private System.Windows.Forms.TextBox tbTMDBPercentDirty;
        private System.Windows.Forms.Label label82;
        private System.Windows.Forms.ComboBox cbTMDBLanguages;
        private System.Windows.Forms.TabPage tpMovieDefaults;
        private System.Windows.Forms.Label label84;
        private System.Windows.Forms.ComboBox cbTMDBRegions;
        private System.Windows.Forms.GroupBox groupBox23;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtMovieFolderFormat;
        private System.Windows.Forms.Label label85;
        private System.Windows.Forms.Label label87;
        private System.Windows.Forms.Button bnOpenMovieMonFolder;
        private System.Windows.Forms.Button bnAddMovieMonFolder;
        private System.Windows.Forms.Button bnRemoveMovieMonFolder;
        private System.Windows.Forms.ListBox lstMovieMonitorFolders;
        private System.Windows.Forms.CheckBox cbNFOMovies;
        private System.Windows.Forms.PictureBox pbuExportEpisodes;
        private System.Windows.Forms.Label label88;
        private System.Windows.Forms.Label label86;
        private System.Windows.Forms.GroupBox groupBox24;
        private System.Windows.Forms.ComboBox cmbDefMovieLocation;
        private System.Windows.Forms.CheckBox cbDefMovieUseDefLocation;
        private System.Windows.Forms.CheckBox cbDefMovieAutoFolders;
        private System.Windows.Forms.GroupBox groupBox25;
        private System.Windows.Forms.CheckBox cbDefMovieDoMissing;
        private System.Windows.Forms.CheckBox cbDefMovieDoRenaming;
        private System.Windows.Forms.PictureBox pbMovieDefaults;
        private System.Windows.Forms.CheckBox chkIncludeMoviesQuickRecent;
        private System.Windows.Forms.TabPage tpAutoExportLibrary;
        private System.Windows.Forms.PictureBox pbuShowExport;
        private System.Windows.Forms.Label label89;
        private System.Windows.Forms.GroupBox groupBox26;
        private System.Windows.Forms.Button bnBrowseMoviesHTML;
        private System.Windows.Forms.CheckBox cbMoviesHTML;
        private System.Windows.Forms.TextBox txtMoviesHTMLTo;
        private System.Windows.Forms.Button bnBrowseMoviesTXT;
        private System.Windows.Forms.CheckBox cbMoviesTXT;
        private System.Windows.Forms.TextBox txtMoviesTXTTo;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button bnBrowseShowsHTML;
        private System.Windows.Forms.CheckBox cbShowsHTML;
        private System.Windows.Forms.TextBox txtShowsHTMLTo;
        private System.Windows.Forms.Button bnBrowseShowsTXT;
        private System.Windows.Forms.CheckBox cbShowsTXT;
        private System.Windows.Forms.TextBox txtShowsTXTTo;
        private System.Windows.Forms.GroupBox groupBox27;
        private System.Windows.Forms.Button bnBrowseMissingMoviesCSV;
        private System.Windows.Forms.Button bnBrowseMissingMoviesXML;
        private System.Windows.Forms.TextBox txtMissingMoviesCSV;
        private System.Windows.Forms.CheckBox cbMissingMoviesXML;
        private System.Windows.Forms.CheckBox cbMissingMoviesCSV;
        private System.Windows.Forms.TextBox txtMissingMoviesXML;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button bnBrowseMissingCSV;
        private System.Windows.Forms.Button bnBrowseMissingXML;
        private System.Windows.Forms.TextBox txtMissingCSV;
        private System.Windows.Forms.CheckBox cbMissingXML;
        private System.Windows.Forms.CheckBox cbMissingCSV;
        private System.Windows.Forms.TextBox txtMissingXML;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox txtMovieFilenameFormat;
        private System.Windows.Forms.Label label90;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox28;
        private System.Windows.Forms.TextBox tbCleanUpDownloadDirMoviesLength;
        private System.Windows.Forms.CheckBox cbCleanUpDownloadDirMoviesLength;
        private System.Windows.Forms.CheckBox cbCleanUpDownloadDirMovies;
        private System.Windows.Forms.TabPage tbAppUpdate;
        private System.Windows.Forms.GroupBox grpUpdateIntervalOption;
        private System.Windows.Forms.CheckBox chkNoPopupOnUpdate;
        private System.Windows.Forms.ComboBox cboUpdateCheckInterval;
        private System.Windows.Forms.RadioButton optUpdateCheckInterval;
        private System.Windows.Forms.RadioButton optUpdateCheckAlways;
        private System.Windows.Forms.CheckBox chkUpdateCheckEnabled;
        private System.Windows.Forms.Button bnBrowseWTWTXT;
        private System.Windows.Forms.TextBox txtWTWTXT;
        private System.Windows.Forms.CheckBox cbWTWTXT;
        private System.Windows.Forms.RadioButton rdoTVTMDB;
        private System.Windows.Forms.CheckBox cbMovieHigherQuality;
        private System.Windows.Forms.Label label91;
        private System.Windows.Forms.ComboBox cbTVDBVersion;
        private System.Windows.Forms.PictureBox pbuUpdates;
        private System.Windows.Forms.Label label92;
        private System.Windows.Forms.CheckBox cbDefShowAlternateOrder;
        private System.Windows.Forms.CheckBox cbDeleteMovieFromDisk;
        private System.Windows.Forms.CheckBox cbIgnorePreviouslySeenMovies;
        private System.Windows.Forms.CheckBox cbDefMovieIncludeNoAirdate;
        private System.Windows.Forms.CheckBox cbDefMovieIncludeFuture;
        private System.Windows.Forms.CheckBox cbFileNameCaseSensitiveMatch;
        private System.Windows.Forms.TabPage tpSubtitles;
        private System.Windows.Forms.GroupBox groupBox29;
        private System.Windows.Forms.Label label94;
        private System.Windows.Forms.TextBox txtSubtitleFolderNames;
        private System.Windows.Forms.CheckBox cbCopySubsFolders;
        private System.Windows.Forms.Label label93;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.CheckBox cbTxtToSub;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.TextBox txtSubtitleExtensions;
        private System.Windows.Forms.CheckBox chkRetainLanguageSpecificSubtitles;
        private System.Windows.Forms.CheckBox cbAutomateAutoAddWhenOneMovieFound;
        private System.Windows.Forms.CheckBox cbAutomateAutoAddWhenOneShowFound;
        private System.Windows.Forms.Label label95;
        private System.Windows.Forms.ComboBox cmbDefMovieFolderFormat;
        private System.Windows.Forms.CheckBox chkUnArchiveFilesInDownloadDirectory;
        private System.Windows.Forms.CheckBox chkBitTorrentUseHTTPS;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox txtShowFolderFormat;
        private System.Windows.Forms.Label label96;
        private System.Windows.Forms.Label label97;
        private System.Windows.Forms.TextBox tbUnwantedRSSTerms;
        private System.Windows.Forms.Label label98;
        private System.Windows.Forms.DomainUpDown upDownScanSeconds;
        private System.Windows.Forms.CheckBox chkRestrictMissingExportsToFullScans;
        private System.Windows.Forms.CheckBox chkGroupMissingEpisodesIntoSeasons;
        private System.Windows.Forms.Label label99;
        private System.Windows.Forms.TextBox txtMinRSSSeeders;
        private global::System.Windows.Forms.Panel panel4;
        private global::System.Windows.Forms.RadioButton rdoGlobalReleaseDates;
        private global::System.Windows.Forms.RadioButton rdoRegionalReleaseDates;
        private global::System.Windows.Forms.Label label100;
    }
}
