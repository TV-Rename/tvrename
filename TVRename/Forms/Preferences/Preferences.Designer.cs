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
            this.components = (new global::System.ComponentModel.Container());
            global::System.ComponentModel.ComponentResourceManager resources = new global::System.ComponentModel.ComponentResourceManager(typeof(global::TVRename.Forms.Preferences));
            this.OKButton = (new global::System.Windows.Forms.Button());
            this.bnCancel = (new global::System.Windows.Forms.Button());
            this.saveFile = (new global::System.Windows.Forms.SaveFileDialog());
            this.folderBrowser = (new global::System.Windows.Forms.FolderBrowserDialog());
            this.openFile = (new global::System.Windows.Forms.OpenFileDialog());
            this.toolTip1 = (new global::System.Windows.Forms.ToolTip(this.components));
            this.cbMonitorFolder = (new global::System.Windows.Forms.CheckBox());
            this.txtEmptyIgnoreExtensions = (new global::System.Windows.Forms.TextBox());
            this.txtEmptyIgnoreWords = (new global::System.Windows.Forms.TextBox());
            this.lbSearchFolders = (new global::System.Windows.Forms.ListBox());
            this.lstFMMonitorFolders = (new global::System.Windows.Forms.ListBox());
            this.tbIgnoreSuffixes = (new global::System.Windows.Forms.TextBox());
            this.tbMovieTerms = (new global::System.Windows.Forms.TextBox());
            this.txtKeepTogether = (new global::System.Windows.Forms.TextBox());
            this.txtOtherExtensions = (new global::System.Windows.Forms.TextBox());
            this.cbCopyFutureDatedEps = (new global::System.Windows.Forms.CheckBox());
            this.label40 = (new global::System.Windows.Forms.Label());
            this.domainUpDown2 = (new global::System.Windows.Forms.DomainUpDown());
            this.tbSeasonSearchTerms = (new global::System.Windows.Forms.TextBox());
            this.chkForceBulkAddToUseSettingsOnly = (new global::System.Windows.Forms.CheckBox());
            this.cbIgnoreRecycleBin = (new global::System.Windows.Forms.CheckBox());
            this.cbIgnoreNoVideoFolders = (new global::System.Windows.Forms.CheckBox());
            this.label1 = (new global::System.Windows.Forms.Label());
            this.upDownScanHours = (new global::System.Windows.Forms.DomainUpDown());
            this.chkScheduledScan = (new global::System.Windows.Forms.CheckBox());
            this.chkScanOnStartup = (new global::System.Windows.Forms.CheckBox());
            this.chkIgnoreAllSpecials = (new global::System.Windows.Forms.CheckBox());
            this.cbAutoSaveOnExit = (new global::System.Windows.Forms.CheckBox());
            this.label84 = (new global::System.Windows.Forms.Label());
            this.lstMovieMonitorFolders = (new global::System.Windows.Forms.ListBox());
            this.chkIncludeMoviesQuickRecent = (new global::System.Windows.Forms.CheckBox());
            this.tbCleanUpDownloadDirMoviesLength = (new global::System.Windows.Forms.TextBox());
            this.label98 = (new global::System.Windows.Forms.Label());
            this.upDownScanSeconds = (new global::System.Windows.Forms.DomainUpDown());
            this.colorDialog = (new global::System.Windows.Forms.ColorDialog());
            this.cmDefaults = (new global::System.Windows.Forms.ContextMenuStrip(this.components));
            this.KODIToolStripMenuItem = (new global::System.Windows.Forms.ToolStripMenuItem());
            this.pyTivoToolStripMenuItem = (new global::System.Windows.Forms.ToolStripMenuItem());
            this.mede8erToolStripMenuItem = (new global::System.Windows.Forms.ToolStripMenuItem());
            this.noneToolStripMenuItem = (new global::System.Windows.Forms.ToolStripMenuItem());
            this.tpDisplay = (new global::System.Windows.Forms.TabPage());
            this.chkShowAccessibilityOptions = (new global::System.Windows.Forms.CheckBox());
            this.cbUseColoursOnWtw = (new global::System.Windows.Forms.CheckBox());
            this.chkBasicShowDetails = (new global::System.Windows.Forms.CheckBox());
            this.chkPostpendThe = (new global::System.Windows.Forms.CheckBox());
            this.groupBox11 = (new global::System.Windows.Forms.GroupBox());
            this.label7 = (new global::System.Windows.Forms.Label());
            this.cboShowStatus = (new global::System.Windows.Forms.ComboBox());
            this.label5 = (new global::System.Windows.Forms.Label());
            this.txtShowStatusColor = (new global::System.Windows.Forms.TextBox());
            this.btnSelectColor = (new global::System.Windows.Forms.Button());
            this.bnRemoveDefinedColor = (new global::System.Windows.Forms.Button());
            this.btnAddShowStatusColoring = (new global::System.Windows.Forms.Button());
            this.lvwDefinedColors = (new global::System.Windows.Forms.ListView());
            this.colShowStatus = (new global::System.Windows.Forms.ColumnHeader());
            this.colColor = (new global::System.Windows.Forms.ColumnHeader());
            this.label61 = (new global::System.Windows.Forms.Label());
            this.cbLeadingZero = (new global::System.Windows.Forms.CheckBox());
            this.txtSeasonFolderName = (new global::System.Windows.Forms.TextBox());
            this.label35 = (new global::System.Windows.Forms.Label());
            this.chkHideWtWSpoilers = (new global::System.Windows.Forms.CheckBox());
            this.chkHideMyShowsSpoilers = (new global::System.Windows.Forms.CheckBox());
            this.rbWTWScan = (new global::System.Windows.Forms.RadioButton());
            this.rbWTWSearch = (new global::System.Windows.Forms.RadioButton());
            this.cbStartupTab = (new global::System.Windows.Forms.ComboBox());
            this.cbAutoSelInMyShows = (new global::System.Windows.Forms.CheckBox());
            this.cbShowEpisodePictures = (new global::System.Windows.Forms.CheckBox());
            this.label11 = (new global::System.Windows.Forms.Label());
            this.label6 = (new global::System.Windows.Forms.Label());
            this.chkShowInTaskbar = (new global::System.Windows.Forms.CheckBox());
            this.cbNotificationIcon = (new global::System.Windows.Forms.CheckBox());
            this.pbDisplay = (new global::System.Windows.Forms.PictureBox());
            this.tpRSSJSONSearch = (new global::System.Windows.Forms.TabPage());
            this.pbRSSJSONSearch = (new global::System.Windows.Forms.PictureBox());
            this.label59 = (new global::System.Windows.Forms.Label());
            this.cbSearchJSON = (new global::System.Windows.Forms.CheckBox());
            this.cbSearchRSS = (new global::System.Windows.Forms.CheckBox());
            this.gbJSON = (new global::System.Windows.Forms.GroupBox());
            this.label78 = (new global::System.Windows.Forms.Label());
            this.tbJSONSeedersToken = (new global::System.Windows.Forms.TextBox());
            this.cbJSONCloudflareProtection = (new global::System.Windows.Forms.CheckBox());
            this.cbSearchJSONManualScanOnly = (new global::System.Windows.Forms.CheckBox());
            this.label55 = (new global::System.Windows.Forms.Label());
            this.tbJSONFilesizeToken = (new global::System.Windows.Forms.TextBox());
            this.label51 = (new global::System.Windows.Forms.Label());
            this.tbJSONFilenameToken = (new global::System.Windows.Forms.TextBox());
            this.label50 = (new global::System.Windows.Forms.Label());
            this.tbJSONURLToken = (new global::System.Windows.Forms.TextBox());
            this.label49 = (new global::System.Windows.Forms.Label());
            this.tbJSONRootNode = (new global::System.Windows.Forms.TextBox());
            this.label48 = (new global::System.Windows.Forms.Label());
            this.tbJSONURL = (new global::System.Windows.Forms.TextBox());
            this.gbRSS = (new global::System.Windows.Forms.GroupBox());
            this.cbRSSCloudflareProtection = (new global::System.Windows.Forms.CheckBox());
            this.cbSearchRSSManualScanOnly = (new global::System.Windows.Forms.CheckBox());
            this.RSSGrid = (new global::SourceGrid.Grid());
            this.label25 = (new global::System.Windows.Forms.Label());
            this.bnRSSRemove = (new global::System.Windows.Forms.Button());
            this.bnRSSGo = (new global::System.Windows.Forms.Button());
            this.bnRSSAdd = (new global::System.Windows.Forms.Button());
            this.tpLibraryFolders = (new global::System.Windows.Forms.TabPage());
            this.groupBox23 = (new global::System.Windows.Forms.GroupBox());
            this.button3 = (new global::System.Windows.Forms.Button());
            this.txtMovieFilenameFormat = (new global::System.Windows.Forms.TextBox());
            this.label90 = (new global::System.Windows.Forms.Label());
            this.button2 = (new global::System.Windows.Forms.Button());
            this.txtMovieFolderFormat = (new global::System.Windows.Forms.TextBox());
            this.label85 = (new global::System.Windows.Forms.Label());
            this.label87 = (new global::System.Windows.Forms.Label());
            this.bnOpenMovieMonFolder = (new global::System.Windows.Forms.Button());
            this.bnAddMovieMonFolder = (new global::System.Windows.Forms.Button());
            this.bnRemoveMovieMonFolder = (new global::System.Windows.Forms.Button());
            this.groupBox6 = (new global::System.Windows.Forms.GroupBox());
            this.button4 = (new global::System.Windows.Forms.Button());
            this.txtShowFolderFormat = (new global::System.Windows.Forms.TextBox());
            this.label96 = (new global::System.Windows.Forms.Label());
            this.button1 = (new global::System.Windows.Forms.Button());
            this.txtSeasonFormat = (new global::System.Windows.Forms.TextBox());
            this.txtSpecialsFolderName = (new global::System.Windows.Forms.TextBox());
            this.label47 = (new global::System.Windows.Forms.Label());
            this.label13 = (new global::System.Windows.Forms.Label());
            this.label65 = (new global::System.Windows.Forms.Label());
            this.label56 = (new global::System.Windows.Forms.Label());
            this.bnOpenMonFolder = (new global::System.Windows.Forms.Button());
            this.bnAddMonFolder = (new global::System.Windows.Forms.Button());
            this.bnRemoveMonFolder = (new global::System.Windows.Forms.Button());
            this.pbLibraryFolders = (new global::System.Windows.Forms.PictureBox());
            this.tpTorrentNZB = (new global::System.Windows.Forms.TabPage());
            this.pbuTorrentNZB = (new global::System.Windows.Forms.PictureBox());
            this.label58 = (new global::System.Windows.Forms.Label());
            this.cbCheckqBitTorrent = (new global::System.Windows.Forms.CheckBox());
            this.cbCheckSABnzbd = (new global::System.Windows.Forms.CheckBox());
            this.cbCheckuTorrent = (new global::System.Windows.Forms.CheckBox());
            this.qBitTorrent = (new global::System.Windows.Forms.GroupBox());
            this.chkBitTorrentUseHTTPS = (new global::System.Windows.Forms.CheckBox());
            this.chkRemoveCompletedTorrents = (new global::System.Windows.Forms.CheckBox());
            this.llqBitTorrentLink = (new global::System.Windows.Forms.LinkLabel());
            this.label79 = (new global::System.Windows.Forms.Label());
            this.rdoqBitTorrentAPIVersionv2 = (new global::System.Windows.Forms.RadioButton());
            this.rdoqBitTorrentAPIVersionv1 = (new global::System.Windows.Forms.RadioButton());
            this.rdoqBitTorrentAPIVersionv0 = (new global::System.Windows.Forms.RadioButton());
            this.label29 = (new global::System.Windows.Forms.Label());
            this.cbDownloadTorrentBeforeDownloading = (new global::System.Windows.Forms.CheckBox());
            this.tbqBitTorrentHost = (new global::System.Windows.Forms.TextBox());
            this.tbqBitTorrentPort = (new global::System.Windows.Forms.TextBox());
            this.label41 = (new global::System.Windows.Forms.Label());
            this.label42 = (new global::System.Windows.Forms.Label());
            this.gbSAB = (new global::System.Windows.Forms.GroupBox());
            this.txtSABHostPort = (new global::System.Windows.Forms.TextBox());
            this.txtSABAPIKey = (new global::System.Windows.Forms.TextBox());
            this.label8 = (new global::System.Windows.Forms.Label());
            this.label9 = (new global::System.Windows.Forms.Label());
            this.gbuTorrent = (new global::System.Windows.Forms.GroupBox());
            this.bnUTBrowseResumeDat = (new global::System.Windows.Forms.Button());
            this.txtUTResumeDatPath = (new global::System.Windows.Forms.TextBox());
            this.bnRSSBrowseuTorrent = (new global::System.Windows.Forms.Button());
            this.label27 = (new global::System.Windows.Forms.Label());
            this.label26 = (new global::System.Windows.Forms.Label());
            this.txtRSSuTorrentPath = (new global::System.Windows.Forms.TextBox());
            this.tbSearchFolders = (new global::System.Windows.Forms.TabPage());
            this.chkUseSearchFullPathWhenMatchingShows = (new global::System.Windows.Forms.CheckBox());
            this.groupBox8 = (new global::System.Windows.Forms.GroupBox());
            this.cbMovieHigherQuality = (new global::System.Windows.Forms.CheckBox());
            this.label53 = (new global::System.Windows.Forms.Label());
            this.label54 = (new global::System.Windows.Forms.Label());
            this.tbPercentBetter = (new global::System.Windows.Forms.TextBox());
            this.tbPriorityOverrideTerms = (new global::System.Windows.Forms.TextBox());
            this.label52 = (new global::System.Windows.Forms.Label());
            this.cbHigherQuality = (new global::System.Windows.Forms.CheckBox());
            this.label67 = (new global::System.Windows.Forms.Label());
            this.gbAutoAdd = (new global::System.Windows.Forms.GroupBox());
            this.cbAutomateAutoAddWhenOneMovieFound = (new global::System.Windows.Forms.CheckBox());
            this.cbAutomateAutoAddWhenOneShowFound = (new global::System.Windows.Forms.CheckBox());
            this.chkAutoSearchForDownloadedFiles = (new global::System.Windows.Forms.CheckBox());
            this.label43 = (new global::System.Windows.Forms.Label());
            this.label44 = (new global::System.Windows.Forms.Label());
            this.cbLeaveOriginals = (new global::System.Windows.Forms.CheckBox());
            this.cbSearchLocally = (new global::System.Windows.Forms.CheckBox());
            this.chkAutoMergeDownloadEpisodes = (new global::System.Windows.Forms.CheckBox());
            this.bnOpenSearchFolder = (new global::System.Windows.Forms.Button());
            this.bnRemoveSearchFolder = (new global::System.Windows.Forms.Button());
            this.bnAddSearchFolder = (new global::System.Windows.Forms.Button());
            this.pbSearchFolders = (new global::System.Windows.Forms.PictureBox());
            this.label23 = (new global::System.Windows.Forms.Label());
            this.tbMediaCenter = (new global::System.Windows.Forms.TabPage());
            this.groupBox16 = (new global::System.Windows.Forms.GroupBox());
            this.cbWDLiveEpisodeFiles = (new global::System.Windows.Forms.CheckBox());
            this.groupBox13 = (new global::System.Windows.Forms.GroupBox());
            this.cbXMLFiles = (new global::System.Windows.Forms.CheckBox());
            this.cbSeriesJpg = (new global::System.Windows.Forms.CheckBox());
            this.cbShrinkLarge = (new global::System.Windows.Forms.CheckBox());
            this.groupBox14 = (new global::System.Windows.Forms.GroupBox());
            this.cbMeta = (new global::System.Windows.Forms.CheckBox());
            this.cbMetaSubfolder = (new global::System.Windows.Forms.CheckBox());
            this.groupBox15 = (new global::System.Windows.Forms.GroupBox());
            this.cbNFOMovies = (new global::System.Windows.Forms.CheckBox());
            this.cbEpTBNs = (new global::System.Windows.Forms.CheckBox());
            this.cbNFOShows = (new global::System.Windows.Forms.CheckBox());
            this.cbKODIImages = (new global::System.Windows.Forms.CheckBox());
            this.cbNFOEpisodes = (new global::System.Windows.Forms.CheckBox());
            this.groupBox12 = (new global::System.Windows.Forms.GroupBox());
            this.cbFantArtJpg = (new global::System.Windows.Forms.CheckBox());
            this.cbFolderJpg = (new global::System.Windows.Forms.CheckBox());
            this.cbEpThumbJpg = (new global::System.Windows.Forms.CheckBox());
            this.panel1 = (new global::System.Windows.Forms.Panel());
            this.rbFolderBanner = (new global::System.Windows.Forms.RadioButton());
            this.rbFolderPoster = (new global::System.Windows.Forms.RadioButton());
            this.rbFolderFanArt = (new global::System.Windows.Forms.RadioButton());
            this.rbFolderSeasonPoster = (new global::System.Windows.Forms.RadioButton());
            this.label64 = (new global::System.Windows.Forms.Label());
            this.bnMCPresets = (new global::System.Windows.Forms.Button());
            this.pbMediaCenter = (new global::System.Windows.Forms.PictureBox());
            this.tbFolderDeleting = (new global::System.Windows.Forms.TabPage());
            this.cbDeleteMovieFromDisk = (new global::System.Windows.Forms.CheckBox());
            this.groupBox28 = (new global::System.Windows.Forms.GroupBox());
            this.cbCleanUpDownloadDirMoviesLength = (new global::System.Windows.Forms.CheckBox());
            this.cbCleanUpDownloadDirMovies = (new global::System.Windows.Forms.CheckBox());
            this.cbCleanUpDownloadDir = (new global::System.Windows.Forms.CheckBox());
            this.label69 = (new global::System.Windows.Forms.Label());
            this.cbDeleteShowFromDisk = (new global::System.Windows.Forms.CheckBox());
            this.label32 = (new global::System.Windows.Forms.Label());
            this.label30 = (new global::System.Windows.Forms.Label());
            this.txtEmptyMaxSize = (new global::System.Windows.Forms.TextBox());
            this.label31 = (new global::System.Windows.Forms.Label());
            this.cbRecycleNotDelete = (new global::System.Windows.Forms.CheckBox());
            this.cbEmptyMaxSize = (new global::System.Windows.Forms.CheckBox());
            this.cbEmptyIgnoreWords = (new global::System.Windows.Forms.CheckBox());
            this.cbEmptyIgnoreExtensions = (new global::System.Windows.Forms.CheckBox());
            this.cbDeleteEmpty = (new global::System.Windows.Forms.CheckBox());
            this.pbFolderDeleting = (new global::System.Windows.Forms.PictureBox());
            this.tbAutoExport = (new global::System.Windows.Forms.TabPage());
            this.pbuExportEpisodes = (new global::System.Windows.Forms.PictureBox());
            this.label88 = (new global::System.Windows.Forms.Label());
            this.groupBox10 = (new global::System.Windows.Forms.GroupBox());
            this.bnBrowseWPL = (new global::System.Windows.Forms.Button());
            this.txtWPL = (new global::System.Windows.Forms.TextBox());
            this.cbWPL = (new global::System.Windows.Forms.CheckBox());
            this.bnBrowseASX = (new global::System.Windows.Forms.Button());
            this.txtASX = (new global::System.Windows.Forms.TextBox());
            this.cbASX = (new global::System.Windows.Forms.CheckBox());
            this.bnBrowseM3U = (new global::System.Windows.Forms.Button());
            this.txtM3U = (new global::System.Windows.Forms.TextBox());
            this.cbM3U = (new global::System.Windows.Forms.CheckBox());
            this.bnBrowseXSPF = (new global::System.Windows.Forms.Button());
            this.txtXSPF = (new global::System.Windows.Forms.TextBox());
            this.cbXSPF = (new global::System.Windows.Forms.CheckBox());
            this.groupBox5 = (new global::System.Windows.Forms.GroupBox());
            this.bnBrowseFOXML = (new global::System.Windows.Forms.Button());
            this.cbFOXML = (new global::System.Windows.Forms.CheckBox());
            this.txtFOXML = (new global::System.Windows.Forms.TextBox());
            this.groupBox4 = (new global::System.Windows.Forms.GroupBox());
            this.bnBrowseRenamingXML = (new global::System.Windows.Forms.Button());
            this.cbRenamingXML = (new global::System.Windows.Forms.CheckBox());
            this.txtRenamingXML = (new global::System.Windows.Forms.TextBox());
            this.groupBox2 = (new global::System.Windows.Forms.GroupBox());
            this.bnBrowseWTWTXT = (new global::System.Windows.Forms.Button());
            this.txtWTWTXT = (new global::System.Windows.Forms.TextBox());
            this.cbWTWTXT = (new global::System.Windows.Forms.CheckBox());
            this.bnBrowseWTWICAL = (new global::System.Windows.Forms.Button());
            this.txtWTWICAL = (new global::System.Windows.Forms.TextBox());
            this.cbWTWICAL = (new global::System.Windows.Forms.CheckBox());
            this.label4 = (new global::System.Windows.Forms.Label());
            this.txtExportRSSDaysPast = (new global::System.Windows.Forms.TextBox());
            this.bnBrowseWTWXML = (new global::System.Windows.Forms.Button());
            this.txtWTWXML = (new global::System.Windows.Forms.TextBox());
            this.cbWTWXML = (new global::System.Windows.Forms.CheckBox());
            this.bnBrowseWTWRSS = (new global::System.Windows.Forms.Button());
            this.txtWTWRSS = (new global::System.Windows.Forms.TextBox());
            this.cbWTWRSS = (new global::System.Windows.Forms.CheckBox());
            this.label17 = (new global::System.Windows.Forms.Label());
            this.label16 = (new global::System.Windows.Forms.Label());
            this.label15 = (new global::System.Windows.Forms.Label());
            this.txtExportRSSMaxDays = (new global::System.Windows.Forms.TextBox());
            this.txtExportRSSMaxShows = (new global::System.Windows.Forms.TextBox());
            this.tbFilesAndFolders = (new global::System.Windows.Forms.TabPage());
            this.chkUnArchiveFilesInDownloadDirectory = (new global::System.Windows.Forms.CheckBox());
            this.cbFileNameCaseSensitiveMatch = (new global::System.Windows.Forms.CheckBox());
            this.chkUseLibraryFullPathWhenMatchingShows = (new global::System.Windows.Forms.CheckBox());
            this.label66 = (new global::System.Windows.Forms.Label());
            this.txtMaxSampleSize = (new global::System.Windows.Forms.TextBox());
            this.txtVideoExtensions = (new global::System.Windows.Forms.TextBox());
            this.label39 = (new global::System.Windows.Forms.Label());
            this.cbKeepTogetherMode = (new global::System.Windows.Forms.ComboBox());
            this.bnReplaceRemove = (new global::System.Windows.Forms.Button());
            this.bnReplaceAdd = (new global::System.Windows.Forms.Button());
            this.label3 = (new global::System.Windows.Forms.Label());
            this.ReplacementsGrid = (new global::SourceGrid.Grid());
            this.label19 = (new global::System.Windows.Forms.Label());
            this.label22 = (new global::System.Windows.Forms.Label());
            this.label14 = (new global::System.Windows.Forms.Label());
            this.cbKeepTogether = (new global::System.Windows.Forms.CheckBox());
            this.cbForceLower = (new global::System.Windows.Forms.CheckBox());
            this.cbIgnoreSamples = (new global::System.Windows.Forms.CheckBox());
            this.pbFilesAndFolders = (new global::System.Windows.Forms.PictureBox());
            this.tbGeneral = (new global::System.Windows.Forms.TabPage());
            this.chkAutoAddAsPartOfQuickRename = (new global::System.Windows.Forms.CheckBox());
            this.chkShareCriticalLogs = (new global::System.Windows.Forms.CheckBox());
            this.label60 = (new global::System.Windows.Forms.Label());
            this.pbGeneral = (new global::System.Windows.Forms.PictureBox());
            this.txtWTWDays = (new global::System.Windows.Forms.TextBox());
            this.cbMode = (new global::System.Windows.Forms.ComboBox());
            this.label34 = (new global::System.Windows.Forms.Label());
            this.label2 = (new global::System.Windows.Forms.Label());
            this.tcTabs = (new global::System.Windows.Forms.TabControl());
            this.tpDataSources = (new global::System.Windows.Forms.TabPage());
            this.panel3 = (new global::System.Windows.Forms.Panel());
            this.rdoMovieTMDB = (new global::System.Windows.Forms.RadioButton());
            this.rdoMovieTheTVDB = (new global::System.Windows.Forms.RadioButton());
            this.panel2 = (new global::System.Windows.Forms.Panel());
            this.rdoTVTMDB = (new global::System.Windows.Forms.RadioButton());
            this.rdoTVTVMaze = (new global::System.Windows.Forms.RadioButton());
            this.rdoTVTVDB = (new global::System.Windows.Forms.RadioButton());
            this.label83 = (new global::System.Windows.Forms.Label());
            this.gbTMDB = (new global::System.Windows.Forms.GroupBox());
            this.cbTMDBRegions = (new global::System.Windows.Forms.ComboBox());
            this.label80 = (new global::System.Windows.Forms.Label());
            this.label81 = (new global::System.Windows.Forms.Label());
            this.tbTMDBPercentDirty = (new global::System.Windows.Forms.TextBox());
            this.label82 = (new global::System.Windows.Forms.Label());
            this.cbTMDBLanguages = (new global::System.Windows.Forms.ComboBox());
            this.label63 = (new global::System.Windows.Forms.Label());
            this.label57 = (new global::System.Windows.Forms.Label());
            this.txtParallelDownloads = (new global::System.Windows.Forms.TextBox());
            this.label21 = (new global::System.Windows.Forms.Label());
            this.label20 = (new global::System.Windows.Forms.Label());
            this.groupBox21 = (new global::System.Windows.Forms.GroupBox());
            this.groupBox20 = (new global::System.Windows.Forms.GroupBox());
            this.label91 = (new global::System.Windows.Forms.Label());
            this.cbTVDBVersion = (new global::System.Windows.Forms.ComboBox());
            this.label37 = (new global::System.Windows.Forms.Label());
            this.label38 = (new global::System.Windows.Forms.Label());
            this.tbPercentDirty = (new global::System.Windows.Forms.TextBox());
            this.label10 = (new global::System.Windows.Forms.Label());
            this.cbTVDBLanguages = (new global::System.Windows.Forms.ComboBox());
            this.label33 = (new global::System.Windows.Forms.Label());
            this.pbSources = (new global::System.Windows.Forms.PictureBox());
            this.tpMovieDefaults = (new global::System.Windows.Forms.TabPage());
            this.label86 = (new global::System.Windows.Forms.Label());
            this.groupBox24 = (new global::System.Windows.Forms.GroupBox());
            this.cmbDefMovieFolderFormat = (new global::System.Windows.Forms.ComboBox());
            this.label95 = (new global::System.Windows.Forms.Label());
            this.cmbDefMovieLocation = (new global::System.Windows.Forms.ComboBox());
            this.cbDefMovieUseDefLocation = (new global::System.Windows.Forms.CheckBox());
            this.cbDefMovieAutoFolders = (new global::System.Windows.Forms.CheckBox());
            this.groupBox25 = (new global::System.Windows.Forms.GroupBox());
            this.cbDefMovieIncludeNoAirdate = (new global::System.Windows.Forms.CheckBox());
            this.cbDefMovieIncludeFuture = (new global::System.Windows.Forms.CheckBox());
            this.cbDefMovieDoMissing = (new global::System.Windows.Forms.CheckBox());
            this.cbDefMovieDoRenaming = (new global::System.Windows.Forms.CheckBox());
            this.pbMovieDefaults = (new global::System.Windows.Forms.PictureBox());
            this.tpShowDefaults = (new global::System.Windows.Forms.TabPage());
            this.label18 = (new global::System.Windows.Forms.Label());
            this.groupBox19 = (new global::System.Windows.Forms.GroupBox());
            this.label24 = (new global::System.Windows.Forms.Label());
            this.cbTimeZone = (new global::System.Windows.Forms.ComboBox());
            this.rbDefShowUseSubFolders = (new global::System.Windows.Forms.RadioButton());
            this.rbDefShowUseBase = (new global::System.Windows.Forms.RadioButton());
            this.label12 = (new global::System.Windows.Forms.Label());
            this.cmbDefShowLocation = (new global::System.Windows.Forms.ComboBox());
            this.cbDefShowUseDefLocation = (new global::System.Windows.Forms.CheckBox());
            this.cbDefShowAutoFolders = (new global::System.Windows.Forms.CheckBox());
            this.groupBox18 = (new global::System.Windows.Forms.GroupBox());
            this.cbDefShowAlternateOrder = (new global::System.Windows.Forms.CheckBox());
            this.cbDefShowEpNameMatching = (new global::System.Windows.Forms.CheckBox());
            this.label68 = (new global::System.Windows.Forms.Label());
            this.cbDefShowAirdateMatching = (new global::System.Windows.Forms.CheckBox());
            this.cbDefShowSpecialsCount = (new global::System.Windows.Forms.CheckBox());
            this.cbDefShowSequentialMatching = (new global::System.Windows.Forms.CheckBox());
            this.cbDefShowIncludeNoAirdate = (new global::System.Windows.Forms.CheckBox());
            this.cbDefShowDoMissingCheck = (new global::System.Windows.Forms.CheckBox());
            this.cbDefShowDVDOrder = (new global::System.Windows.Forms.CheckBox());
            this.cbDefShowIncludeFuture = (new global::System.Windows.Forms.CheckBox());
            this.cbDefShowDoRenaming = (new global::System.Windows.Forms.CheckBox());
            this.cbDefShowNextAirdate = (new global::System.Windows.Forms.CheckBox());
            this.pictureBox1 = (new global::System.Windows.Forms.PictureBox());
            this.tpScanSettings = (new global::System.Windows.Forms.TabPage());
            this.chkGroupMissingEpisodesIntoSeasons = (new global::System.Windows.Forms.CheckBox());
            this.groupBox17 = (new global::System.Windows.Forms.GroupBox());
            this.cbIgnorePreviouslySeenMovies = (new global::System.Windows.Forms.CheckBox());
            this.chkMoveLibraryFiles = (new global::System.Windows.Forms.CheckBox());
            this.lblScanAction = (new global::System.Windows.Forms.Label());
            this.rdoQuickScan = (new global::System.Windows.Forms.RadioButton());
            this.rdoRecentScan = (new global::System.Windows.Forms.RadioButton());
            this.rdoFullScan = (new global::System.Windows.Forms.RadioButton());
            this.cbIgnorePreviouslySeen = (new global::System.Windows.Forms.CheckBox());
            this.chkPreventMove = (new global::System.Windows.Forms.CheckBox());
            this.label28 = (new global::System.Windows.Forms.Label());
            this.cbRenameCheck = (new global::System.Windows.Forms.CheckBox());
            this.cbMissing = (new global::System.Windows.Forms.CheckBox());
            this.groupBox1 = (new global::System.Windows.Forms.GroupBox());
            this.chkChooseWhenMultipleEpisodesMatch = (new global::System.Windows.Forms.CheckBox());
            this.cbxUpdateAirDate = (new global::System.Windows.Forms.CheckBox());
            this.cbAutoCreateFolders = (new global::System.Windows.Forms.CheckBox());
            this.chkAutoMergeLibraryEpisodes = (new global::System.Windows.Forms.CheckBox());
            this.cbScanIncludesBulkAdd = (new global::System.Windows.Forms.CheckBox());
            this.gbBulkAdd = (new global::System.Windows.Forms.GroupBox());
            this.label36 = (new global::System.Windows.Forms.Label());
            this.label62 = (new global::System.Windows.Forms.Label());
            this.pbScanOptions = (new global::System.Windows.Forms.PictureBox());
            this.tpSubtitles = (new global::System.Windows.Forms.TabPage());
            this.groupBox29 = (new global::System.Windows.Forms.GroupBox());
            this.label94 = (new global::System.Windows.Forms.Label());
            this.txtSubtitleFolderNames = (new global::System.Windows.Forms.TextBox());
            this.cbCopySubsFolders = (new global::System.Windows.Forms.CheckBox());
            this.label93 = (new global::System.Windows.Forms.Label());
            this.pictureBox2 = (new global::System.Windows.Forms.PictureBox());
            this.groupBox9 = (new global::System.Windows.Forms.GroupBox());
            this.cbTxtToSub = (new global::System.Windows.Forms.CheckBox());
            this.label46 = (new global::System.Windows.Forms.Label());
            this.txtSubtitleExtensions = (new global::System.Windows.Forms.TextBox());
            this.chkRetainLanguageSpecificSubtitles = (new global::System.Windows.Forms.CheckBox());
            this.tpJackett = (new global::System.Windows.Forms.TabPage());
            this.label99 = (new global::System.Windows.Forms.Label());
            this.txtMinRSSSeeders = (new global::System.Windows.Forms.TextBox());
            this.label97 = (new global::System.Windows.Forms.Label());
            this.tbUnwantedRSSTerms = (new global::System.Windows.Forms.TextBox());
            this.chkSearchJackettButton = (new global::System.Windows.Forms.CheckBox());
            this.cmbSupervisedDuplicateAction = (new global::System.Windows.Forms.ComboBox());
            this.label77 = (new global::System.Windows.Forms.Label());
            this.cmbUnattendedDuplicateAction = (new global::System.Windows.Forms.ComboBox());
            this.label76 = (new global::System.Windows.Forms.Label());
            this.cbDetailedRSSJSONLogging = (new global::System.Windows.Forms.CheckBox());
            this.pbuJackett = (new global::System.Windows.Forms.PictureBox());
            this.label70 = (new global::System.Windows.Forms.Label());
            this.cbSearchJackett = (new global::System.Windows.Forms.CheckBox());
            this.groupBox22 = (new global::System.Windows.Forms.GroupBox());
            this.chkUseJackettTextSearch = (new global::System.Windows.Forms.CheckBox());
            this.chkSkipJackettFullScans = (new global::System.Windows.Forms.CheckBox());
            this.llJackettLink = (new global::System.Windows.Forms.LinkLabel());
            this.label71 = (new global::System.Windows.Forms.Label());
            this.cbSearchJackettOnManualScansOnly = (new global::System.Windows.Forms.CheckBox());
            this.label72 = (new global::System.Windows.Forms.Label());
            this.txtJackettIndexer = (new global::System.Windows.Forms.TextBox());
            this.label73 = (new global::System.Windows.Forms.Label());
            this.txtJackettAPIKey = (new global::System.Windows.Forms.TextBox());
            this.label74 = (new global::System.Windows.Forms.Label());
            this.txtJackettPort = (new global::System.Windows.Forms.TextBox());
            this.label75 = (new global::System.Windows.Forms.Label());
            this.txtJackettServer = (new global::System.Windows.Forms.TextBox());
            this.label45 = (new global::System.Windows.Forms.Label());
            this.tbPreferredRSSTerms = (new global::System.Windows.Forms.TextBox());
            this.tpAutoExportLibrary = (new global::System.Windows.Forms.TabPage());
            this.chkRestrictMissingExportsToFullScans = (new global::System.Windows.Forms.CheckBox());
            this.pbuShowExport = (new global::System.Windows.Forms.PictureBox());
            this.label89 = (new global::System.Windows.Forms.Label());
            this.groupBox26 = (new global::System.Windows.Forms.GroupBox());
            this.bnBrowseMoviesHTML = (new global::System.Windows.Forms.Button());
            this.cbMoviesHTML = (new global::System.Windows.Forms.CheckBox());
            this.txtMoviesHTMLTo = (new global::System.Windows.Forms.TextBox());
            this.bnBrowseMoviesTXT = (new global::System.Windows.Forms.Button());
            this.cbMoviesTXT = (new global::System.Windows.Forms.CheckBox());
            this.txtMoviesTXTTo = (new global::System.Windows.Forms.TextBox());
            this.groupBox7 = (new global::System.Windows.Forms.GroupBox());
            this.bnBrowseShowsHTML = (new global::System.Windows.Forms.Button());
            this.cbShowsHTML = (new global::System.Windows.Forms.CheckBox());
            this.txtShowsHTMLTo = (new global::System.Windows.Forms.TextBox());
            this.bnBrowseShowsTXT = (new global::System.Windows.Forms.Button());
            this.cbShowsTXT = (new global::System.Windows.Forms.CheckBox());
            this.txtShowsTXTTo = (new global::System.Windows.Forms.TextBox());
            this.groupBox27 = (new global::System.Windows.Forms.GroupBox());
            this.bnBrowseMissingMoviesCSV = (new global::System.Windows.Forms.Button());
            this.bnBrowseMissingMoviesXML = (new global::System.Windows.Forms.Button());
            this.txtMissingMoviesCSV = (new global::System.Windows.Forms.TextBox());
            this.cbMissingMoviesXML = (new global::System.Windows.Forms.CheckBox());
            this.cbMissingMoviesCSV = (new global::System.Windows.Forms.CheckBox());
            this.txtMissingMoviesXML = (new global::System.Windows.Forms.TextBox());
            this.groupBox3 = (new global::System.Windows.Forms.GroupBox());
            this.bnBrowseMissingCSV = (new global::System.Windows.Forms.Button());
            this.bnBrowseMissingXML = (new global::System.Windows.Forms.Button());
            this.txtMissingCSV = (new global::System.Windows.Forms.TextBox());
            this.cbMissingXML = (new global::System.Windows.Forms.CheckBox());
            this.cbMissingCSV = (new global::System.Windows.Forms.CheckBox());
            this.txtMissingXML = (new global::System.Windows.Forms.TextBox());
            this.tbAppUpdate = (new global::System.Windows.Forms.TabPage());
            this.pbuUpdates = (new global::System.Windows.Forms.PictureBox());
            this.chkUpdateCheckEnabled = (new global::System.Windows.Forms.CheckBox());
            this.label92 = (new global::System.Windows.Forms.Label());
            this.grpUpdateIntervalOption = (new global::System.Windows.Forms.GroupBox());
            this.chkNoPopupOnUpdate = (new global::System.Windows.Forms.CheckBox());
            this.cboUpdateCheckInterval = (new global::System.Windows.Forms.ComboBox());
            this.optUpdateCheckInterval = (new global::System.Windows.Forms.RadioButton());
            this.optUpdateCheckAlways = (new global::System.Windows.Forms.RadioButton());
            this.panel4 = (new global::System.Windows.Forms.Panel());
            this.rdoGlobalReleaseDates = (new global::System.Windows.Forms.RadioButton());
            this.rdoRegionalReleaseDates = (new global::System.Windows.Forms.RadioButton());
            this.label100 = (new global::System.Windows.Forms.Label());
            this.cmDefaults.SuspendLayout();
            this.tpDisplay.SuspendLayout();
            this.groupBox11.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbDisplay)).BeginInit();
            this.tpRSSJSONSearch.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbRSSJSONSearch)).BeginInit();
            this.gbJSON.SuspendLayout();
            this.gbRSS.SuspendLayout();
            this.tpLibraryFolders.SuspendLayout();
            this.groupBox23.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbLibraryFolders)).BeginInit();
            this.tpTorrentNZB.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbuTorrentNZB)).BeginInit();
            this.qBitTorrent.SuspendLayout();
            this.gbSAB.SuspendLayout();
            this.gbuTorrent.SuspendLayout();
            this.tbSearchFolders.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.gbAutoAdd.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbSearchFolders)).BeginInit();
            this.tbMediaCenter.SuspendLayout();
            this.groupBox16.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.groupBox15.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.panel1.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbMediaCenter)).BeginInit();
            this.tbFolderDeleting.SuspendLayout();
            this.groupBox28.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbFolderDeleting)).BeginInit();
            this.tbAutoExport.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbuExportEpisodes)).BeginInit();
            this.groupBox10.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tbFilesAndFolders.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbFilesAndFolders)).BeginInit();
            this.tbGeneral.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbGeneral)).BeginInit();
            this.tcTabs.SuspendLayout();
            this.tpDataSources.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.gbTMDB.SuspendLayout();
            this.groupBox20.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbSources)).BeginInit();
            this.tpMovieDefaults.SuspendLayout();
            this.groupBox24.SuspendLayout();
            this.groupBox25.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbMovieDefaults)).BeginInit();
            this.tpShowDefaults.SuspendLayout();
            this.groupBox19.SuspendLayout();
            this.groupBox18.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tpScanSettings.SuspendLayout();
            this.groupBox17.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbBulkAdd.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbScanOptions)).BeginInit();
            this.tpSubtitles.SuspendLayout();
            this.groupBox29.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBox9.SuspendLayout();
            this.tpJackett.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbuJackett)).BeginInit();
            this.groupBox22.SuspendLayout();
            this.tpAutoExportLibrary.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbuShowExport)).BeginInit();
            this.groupBox26.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox27.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tbAppUpdate.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbuUpdates)).BeginInit();
            this.grpUpdateIntervalOption.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.Location = (new global::System.Drawing.Point(481, 713));
            this.OKButton.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.OKButton.Name = ("OKButton");
            this.OKButton.Size = (new global::System.Drawing.Size(88, 27));
            this.OKButton.TabIndex = (0);
            this.OKButton.Text = ("OK");
            this.OKButton.UseVisualStyleBackColor = (true);
            this.OKButton.Click += (this.OKButton_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = (global::System.Windows.Forms.DialogResult.Cancel);
            this.bnCancel.Location = (new global::System.Drawing.Point(580, 713));
            this.bnCancel.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnCancel.Name = ("bnCancel");
            this.bnCancel.Size = (new global::System.Drawing.Size(88, 27));
            this.bnCancel.TabIndex = (1);
            this.bnCancel.Text = ("Cancel");
            this.bnCancel.UseVisualStyleBackColor = (true);
            this.bnCancel.Click += (this.CancelButton_Click);
            // 
            // saveFile
            // 
            this.saveFile.Filter = (resources.GetString("saveFile.Filter"));
            // 
            // folderBrowser
            // 
            this.folderBrowser.ShowNewFolderButton = (false);
            // 
            // openFile
            // 
            this.openFile.Filter = ("Torrent files (*.torrent)|*.torrent|All files (*.*)|*.*");
            // 
            // cbMonitorFolder
            // 
            this.cbMonitorFolder.AutoSize = (true);
            this.cbMonitorFolder.Location = (new global::System.Drawing.Point(7, 147));
            this.cbMonitorFolder.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbMonitorFolder.Name = ("cbMonitorFolder");
            this.cbMonitorFolder.Size = (new global::System.Drawing.Size(338, 19));
            this.cbMonitorFolder.TabIndex = (5);
            this.cbMonitorFolder.Text = ("&Monitor Search Folders (run a scan when files change) after");
            this.toolTip1.SetToolTip(this.cbMonitorFolder, "If the contents of any of these folder change, then automatically do a \"Scan\" and \"Do\".");
            this.cbMonitorFolder.UseVisualStyleBackColor = (true);
            // 
            // txtEmptyIgnoreExtensions
            // 
            this.txtEmptyIgnoreExtensions.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmptyIgnoreExtensions.Location = (new global::System.Drawing.Point(111, 186));
            this.txtEmptyIgnoreExtensions.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtEmptyIgnoreExtensions.Name = ("txtEmptyIgnoreExtensions");
            this.txtEmptyIgnoreExtensions.Size = (new global::System.Drawing.Size(359, 23));
            this.txtEmptyIgnoreExtensions.TabIndex = (5);
            this.toolTip1.SetToolTip(this.txtEmptyIgnoreExtensions, "For example \".par2;.nzb;.nfo\"");
            // 
            // txtEmptyIgnoreWords
            // 
            this.txtEmptyIgnoreWords.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmptyIgnoreWords.Location = (new global::System.Drawing.Point(111, 128));
            this.txtEmptyIgnoreWords.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtEmptyIgnoreWords.Name = ("txtEmptyIgnoreWords");
            this.txtEmptyIgnoreWords.Size = (new global::System.Drawing.Size(359, 23));
            this.txtEmptyIgnoreWords.TabIndex = (3);
            this.toolTip1.SetToolTip(this.txtEmptyIgnoreWords, "For example \"sample\"");
            // 
            // lbSearchFolders
            // 
            this.lbSearchFolders.AllowDrop = (true);
            this.lbSearchFolders.Anchor = ((global::System.Windows.Forms.AnchorStyles)((((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Bottom)) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.lbSearchFolders.FormattingEnabled = (true);
            this.lbSearchFolders.ItemHeight = (15);
            this.lbSearchFolders.Location = (new global::System.Drawing.Point(6, 239));
            this.lbSearchFolders.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.lbSearchFolders.Name = ("lbSearchFolders");
            this.lbSearchFolders.ScrollAlwaysVisible = (true);
            this.lbSearchFolders.Size = (new global::System.Drawing.Size(460, 79));
            this.lbSearchFolders.TabIndex = (1);
            this.toolTip1.SetToolTip(this.lbSearchFolders, "Search Folders\r\n\r\nThe Search Folders are where new, unsorted\r\nfiles are to be found. The scan will look for\r\nmissing episodes in this location. It should not\r\noverlap with the 'Library Folders' above.");
            this.lbSearchFolders.SelectedIndexChanged += (this.lbSearchFolders_SelectedIndexChanged);
            this.lbSearchFolders.DragDrop += (this.lbSearchFolders_DragDrop);
            this.lbSearchFolders.DragOver += (this.lbSearchFolders_DragOver);
            this.lbSearchFolders.KeyDown += (this.lbSearchFolders_KeyDown);
            // 
            // lstFMMonitorFolders
            // 
            this.lstFMMonitorFolders.AllowDrop = (true);
            this.lstFMMonitorFolders.Anchor = ((global::System.Windows.Forms.AnchorStyles)((((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Bottom)) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.lstFMMonitorFolders.FormattingEnabled = (true);
            this.lstFMMonitorFolders.IntegralHeight = (false);
            this.lstFMMonitorFolders.ItemHeight = (15);
            this.lstFMMonitorFolders.Location = (new global::System.Drawing.Point(7, 80));
            this.lstFMMonitorFolders.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.lstFMMonitorFolders.Name = ("lstFMMonitorFolders");
            this.lstFMMonitorFolders.ScrollAlwaysVisible = (true);
            this.lstFMMonitorFolders.SelectionMode = (global::System.Windows.Forms.SelectionMode.MultiExtended);
            this.lstFMMonitorFolders.Size = (new global::System.Drawing.Size(464, 111));
            this.lstFMMonitorFolders.TabIndex = (32);
            this.toolTip1.SetToolTip(this.lstFMMonitorFolders, resources.GetString("lstFMMonitorFolders.ToolTip"));
            this.lstFMMonitorFolders.SelectedIndexChanged += (this.lstFMMonitorFolders_SelectedIndexChanged);
            this.lstFMMonitorFolders.DragDrop += (this.lstFMMonitorFolders_DragDrop);
            this.lstFMMonitorFolders.DragOver += (this.FileIcon_DragOver);
            this.lstFMMonitorFolders.KeyDown += (this.lstFMMonitorFolders_KeyDown);
            // 
            // tbIgnoreSuffixes
            // 
            this.tbIgnoreSuffixes.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.tbIgnoreSuffixes.Location = (new global::System.Drawing.Point(117, 117));
            this.tbIgnoreSuffixes.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbIgnoreSuffixes.Name = ("tbIgnoreSuffixes");
            this.tbIgnoreSuffixes.Size = (new global::System.Drawing.Size(344, 23));
            this.tbIgnoreSuffixes.TabIndex = (15);
            this.toolTip1.SetToolTip(this.tbIgnoreSuffixes, "These terms and any text after them will be ignored when\r\nsearching on TVDB for the show title based on the filename.");
            // 
            // tbMovieTerms
            // 
            this.tbMovieTerms.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.tbMovieTerms.Location = (new global::System.Drawing.Point(117, 87));
            this.tbMovieTerms.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbMovieTerms.Name = ("tbMovieTerms");
            this.tbMovieTerms.Size = (new global::System.Drawing.Size(344, 23));
            this.tbMovieTerms.TabIndex = (13);
            this.toolTip1.SetToolTip(this.tbMovieTerms, "If a filename contains any of these terms then it is assumed\r\nthat it is a Film and not a TV Show. Hence 'Auto Add' is not\r\ninvoked for this file.");
            // 
            // txtKeepTogether
            // 
            this.txtKeepTogether.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtKeepTogether.Location = (new global::System.Drawing.Point(238, 359));
            this.txtKeepTogether.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtKeepTogether.Name = ("txtKeepTogether");
            this.txtKeepTogether.Size = (new global::System.Drawing.Size(226, 23));
            this.txtKeepTogether.TabIndex = (23);
            this.toolTip1.SetToolTip(this.txtKeepTogether, "Which file extensions should be copied from the Search\r\nFolders into the library?");
            // 
            // txtOtherExtensions
            // 
            this.txtOtherExtensions.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtOtherExtensions.Location = (new global::System.Drawing.Point(115, 299));
            this.txtOtherExtensions.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtOtherExtensions.Name = ("txtOtherExtensions");
            this.txtOtherExtensions.Size = (new global::System.Drawing.Size(348, 23));
            this.txtOtherExtensions.TabIndex = (7);
            this.toolTip1.SetToolTip(this.txtOtherExtensions, "Which file extensions in the library should be renamed along\r\nwith the video files?");
            // 
            // cbCopyFutureDatedEps
            // 
            this.cbCopyFutureDatedEps.AutoSize = (true);
            this.cbCopyFutureDatedEps.Location = (new global::System.Drawing.Point(7, 173));
            this.cbCopyFutureDatedEps.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbCopyFutureDatedEps.Name = ("cbCopyFutureDatedEps");
            this.cbCopyFutureDatedEps.Size = (new global::System.Drawing.Size(298, 19));
            this.cbCopyFutureDatedEps.TabIndex = (41);
            this.cbCopyFutureDatedEps.Text = ("Copy future dated episodes found in Search Folders");
            this.toolTip1.SetToolTip(this.cbCopyFutureDatedEps, "If set then any episodes in the search folders will be copied into the library, even if they are yet to officially air.");
            this.cbCopyFutureDatedEps.UseVisualStyleBackColor = (true);
            // 
            // label40
            // 
            this.label40.AutoSize = (true);
            this.label40.Location = (new global::System.Drawing.Point(188, 173));
            this.label40.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label40.Name = ("label40");
            this.label40.Size = (new global::System.Drawing.Size(37, 15));
            this.label40.TabIndex = (54);
            this.label40.Text = ("hours");
            this.toolTip1.SetToolTip(this.label40, "If checked the system will automatically scan and complete actions on a periodic schedule");
            // 
            // domainUpDown2
            // 
            this.domainUpDown2.Items.Add("96");
            this.domainUpDown2.Items.Add("48");
            this.domainUpDown2.Items.Add("24");
            this.domainUpDown2.Items.Add("12");
            this.domainUpDown2.Items.Add("8");
            this.domainUpDown2.Items.Add("6");
            this.domainUpDown2.Items.Add("5");
            this.domainUpDown2.Items.Add("4");
            this.domainUpDown2.Items.Add("3");
            this.domainUpDown2.Items.Add("2");
            this.domainUpDown2.Items.Add("1");
            this.domainUpDown2.Location = (new global::System.Drawing.Point(136, 169));
            this.domainUpDown2.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.domainUpDown2.Name = ("domainUpDown2");
            this.domainUpDown2.Size = (new global::System.Drawing.Size(47, 23));
            this.domainUpDown2.TabIndex = (53);
            this.toolTip1.SetToolTip(this.domainUpDown2, "How often should TV Rename update itself from upstream data sources?");
            this.domainUpDown2.KeyDown += (this.SuppressKeyPress);
            // 
            // tbSeasonSearchTerms
            // 
            this.tbSeasonSearchTerms.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.tbSeasonSearchTerms.Location = (new global::System.Drawing.Point(147, 98));
            this.tbSeasonSearchTerms.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbSeasonSearchTerms.Name = ("tbSeasonSearchTerms");
            this.tbSeasonSearchTerms.Size = (new global::System.Drawing.Size(300, 23));
            this.tbSeasonSearchTerms.TabIndex = (22);
            this.toolTip1.SetToolTip(this.tbSeasonSearchTerms, "Which terms should the system look for in directory\r\nnames that indicate that the folder contains a season's\r\nworth of episodes for a show.\r\nThey should be separated by a semi-colon - ; ");
            // 
            // chkForceBulkAddToUseSettingsOnly
            // 
            this.chkForceBulkAddToUseSettingsOnly.AutoSize = (true);
            this.chkForceBulkAddToUseSettingsOnly.Location = (new global::System.Drawing.Point(7, 75));
            this.chkForceBulkAddToUseSettingsOnly.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkForceBulkAddToUseSettingsOnly.Name = ("chkForceBulkAddToUseSettingsOnly");
            this.chkForceBulkAddToUseSettingsOnly.Size = (new global::System.Drawing.Size(270, 19));
            this.chkForceBulkAddToUseSettingsOnly.TabIndex = (15);
            this.chkForceBulkAddToUseSettingsOnly.Text = ("Force to Use Season Words from Settings Only");
            this.toolTip1.SetToolTip(this.chkForceBulkAddToUseSettingsOnly, "If set then Bulk Add just uses the season words from settings. If not set (recommended) then Bulk Add finds addition season words from each show's configuration.");
            this.chkForceBulkAddToUseSettingsOnly.UseVisualStyleBackColor = (true);
            // 
            // cbIgnoreRecycleBin
            // 
            this.cbIgnoreRecycleBin.AutoSize = (true);
            this.cbIgnoreRecycleBin.Location = (new global::System.Drawing.Point(7, 48));
            this.cbIgnoreRecycleBin.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbIgnoreRecycleBin.Name = ("cbIgnoreRecycleBin");
            this.cbIgnoreRecycleBin.Size = (new global::System.Drawing.Size(123, 19));
            this.cbIgnoreRecycleBin.TabIndex = (14);
            this.cbIgnoreRecycleBin.Text = ("Ignore &Recycle Bin");
            this.toolTip1.SetToolTip(this.cbIgnoreRecycleBin, "If set then Bulk Add ignores all files in the Recycle Bin");
            this.cbIgnoreRecycleBin.UseVisualStyleBackColor = (true);
            // 
            // cbIgnoreNoVideoFolders
            // 
            this.cbIgnoreNoVideoFolders.AutoSize = (true);
            this.cbIgnoreNoVideoFolders.Location = (new global::System.Drawing.Point(7, 22));
            this.cbIgnoreNoVideoFolders.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbIgnoreNoVideoFolders.Name = ("cbIgnoreNoVideoFolders");
            this.cbIgnoreNoVideoFolders.Size = (new global::System.Drawing.Size(251, 19));
            this.cbIgnoreNoVideoFolders.TabIndex = (13);
            this.cbIgnoreNoVideoFolders.Text = ("&Only Include Folders containing Video files");
            this.toolTip1.SetToolTip(this.cbIgnoreNoVideoFolders, "If set then only folders that contain video files are considered for the 'Bulk Add' feature");
            this.cbIgnoreNoVideoFolders.UseVisualStyleBackColor = (true);
            // 
            // label1
            // 
            this.label1.AutoSize = (true);
            this.label1.Location = (new global::System.Drawing.Point(216, 96));
            this.label1.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label1.Name = ("label1");
            this.label1.Size = (new global::System.Drawing.Size(37, 15));
            this.label1.TabIndex = (47);
            this.label1.Text = ("hours");
            this.toolTip1.SetToolTip(this.label1, "If checked the system will automatically scan and complete actions on a periodic schedule");
            // 
            // upDownScanHours
            // 
            this.upDownScanHours.Items.Add("96");
            this.upDownScanHours.Items.Add("48");
            this.upDownScanHours.Items.Add("24");
            this.upDownScanHours.Items.Add("12");
            this.upDownScanHours.Items.Add("8");
            this.upDownScanHours.Items.Add("6");
            this.upDownScanHours.Items.Add("5");
            this.upDownScanHours.Items.Add("4");
            this.upDownScanHours.Items.Add("3");
            this.upDownScanHours.Items.Add("2");
            this.upDownScanHours.Items.Add("1");
            this.upDownScanHours.Location = (new global::System.Drawing.Point(164, 93));
            this.upDownScanHours.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.upDownScanHours.Name = ("upDownScanHours");
            this.upDownScanHours.Size = (new global::System.Drawing.Size(47, 23));
            this.upDownScanHours.TabIndex = (46);
            this.toolTip1.SetToolTip(this.upDownScanHours, "If checked the system will automatically scan and complete actions on a periodic schedule");
            // 
            // chkScheduledScan
            // 
            this.chkScheduledScan.AutoSize = (true);
            this.chkScheduledScan.Location = (new global::System.Drawing.Point(12, 95));
            this.chkScheduledScan.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkScheduledScan.Name = ("chkScheduledScan");
            this.chkScheduledScan.Size = (new global::System.Drawing.Size(142, 19));
            this.chkScheduledScan.TabIndex = (45);
            this.chkScheduledScan.Text = ("Sc&heduled scan every ");
            this.toolTip1.SetToolTip(this.chkScheduledScan, "If checked the system will automatically scan and complete actions on a periodic schedule");
            this.chkScheduledScan.UseVisualStyleBackColor = (true);
            // 
            // chkScanOnStartup
            // 
            this.chkScanOnStartup.AutoSize = (true);
            this.chkScanOnStartup.Location = (new global::System.Drawing.Point(12, 68));
            this.chkScanOnStartup.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkScanOnStartup.Name = ("chkScanOnStartup");
            this.chkScanOnStartup.Size = (new global::System.Drawing.Size(109, 19));
            this.chkScanOnStartup.TabIndex = (44);
            this.chkScanOnStartup.Text = ("&Scan on Startup");
            this.toolTip1.SetToolTip(this.chkScanOnStartup, "If checked the system will automatically scan and complete actions on startup");
            this.chkScanOnStartup.UseVisualStyleBackColor = (true);
            // 
            // chkIgnoreAllSpecials
            // 
            this.chkIgnoreAllSpecials.AutoSize = (true);
            this.chkIgnoreAllSpecials.Location = (new global::System.Drawing.Point(34, 242));
            this.chkIgnoreAllSpecials.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkIgnoreAllSpecials.Name = ("chkIgnoreAllSpecials");
            this.chkIgnoreAllSpecials.Size = (new global::System.Drawing.Size(191, 19));
            this.chkIgnoreAllSpecials.TabIndex = (49);
            this.chkIgnoreAllSpecials.Text = ("Ignore Specials for all TV Shows");
            this.toolTip1.SetToolTip(this.chkIgnoreAllSpecials, "Ignores 'specials' season for all TV shows");
            this.chkIgnoreAllSpecials.UseVisualStyleBackColor = (true);
            // 
            // cbAutoSaveOnExit
            // 
            this.cbAutoSaveOnExit.AutoSize = (true);
            this.cbAutoSaveOnExit.Location = (new global::System.Drawing.Point(15, 173));
            this.cbAutoSaveOnExit.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbAutoSaveOnExit.Name = ("cbAutoSaveOnExit");
            this.cbAutoSaveOnExit.Size = (new global::System.Drawing.Size(117, 19));
            this.cbAutoSaveOnExit.TabIndex = (44);
            this.cbAutoSaveOnExit.Text = ("Auto save on Exit");
            this.toolTip1.SetToolTip(this.cbAutoSaveOnExit, "Should the system ask the user or always save when the application is shutdown?");
            this.cbAutoSaveOnExit.UseVisualStyleBackColor = (true);
            // 
            // label84
            // 
            this.label84.AutoSize = (true);
            this.label84.Location = (new global::System.Drawing.Point(12, 55));
            this.label84.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label84.Name = ("label84");
            this.label84.Size = (new global::System.Drawing.Size(95, 15));
            this.label84.TabIndex = (26);
            this.label84.Text = ("&Preferred region:");
            this.toolTip1.SetToolTip(this.label84, "TMDB will return release dates and certifications based on your location");
            // 
            // lstMovieMonitorFolders
            // 
            this.lstMovieMonitorFolders.AllowDrop = (true);
            this.lstMovieMonitorFolders.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.lstMovieMonitorFolders.FormattingEnabled = (true);
            this.lstMovieMonitorFolders.IntegralHeight = (false);
            this.lstMovieMonitorFolders.ItemHeight = (15);
            this.lstMovieMonitorFolders.Location = (new global::System.Drawing.Point(7, 373));
            this.lstMovieMonitorFolders.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.lstMovieMonitorFolders.Name = ("lstMovieMonitorFolders");
            this.lstMovieMonitorFolders.ScrollAlwaysVisible = (true);
            this.lstMovieMonitorFolders.SelectionMode = (global::System.Windows.Forms.SelectionMode.MultiExtended);
            this.lstMovieMonitorFolders.Size = (new global::System.Drawing.Size(464, 111));
            this.lstMovieMonitorFolders.TabIndex = (49);
            this.toolTip1.SetToolTip(this.lstMovieMonitorFolders, resources.GetString("lstMovieMonitorFolders.ToolTip"));
            this.lstMovieMonitorFolders.SelectedIndexChanged += (this.lstMovieMonitorFolders_SelectedIndexChanged);
            this.lstMovieMonitorFolders.DragDrop += (this.lstMovieMonitorFolders_DragDrop);
            this.lstMovieMonitorFolders.DragOver += (this.FileIcon_DragOver);
            this.lstMovieMonitorFolders.KeyDown += (this.lstMovieMonitorFolders_KeyDown);
            // 
            // chkIncludeMoviesQuickRecent
            // 
            this.chkIncludeMoviesQuickRecent.AutoSize = (true);
            this.chkIncludeMoviesQuickRecent.Location = (new global::System.Drawing.Point(264, 22));
            this.chkIncludeMoviesQuickRecent.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkIncludeMoviesQuickRecent.Name = ("chkIncludeMoviesQuickRecent");
            this.chkIncludeMoviesQuickRecent.Size = (new global::System.Drawing.Size(194, 19));
            this.chkIncludeMoviesQuickRecent.TabIndex = (50);
            this.chkIncludeMoviesQuickRecent.Text = ("&Include Movies in Quick/Recent");
            this.toolTip1.SetToolTip(this.chkIncludeMoviesQuickRecent, "If checked the system will automatically scan and complete actions on startup");
            this.chkIncludeMoviesQuickRecent.UseVisualStyleBackColor = (true);
            // 
            // tbCleanUpDownloadDirMoviesLength
            // 
            this.tbCleanUpDownloadDirMoviesLength.Location = (new global::System.Drawing.Point(252, 75));
            this.tbCleanUpDownloadDirMoviesLength.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbCleanUpDownloadDirMoviesLength.Name = ("tbCleanUpDownloadDirMoviesLength");
            this.tbCleanUpDownloadDirMoviesLength.Size = (new global::System.Drawing.Size(63, 23));
            this.tbCleanUpDownloadDirMoviesLength.TabIndex = (14);
            this.toolTip1.SetToolTip(this.tbCleanUpDownloadDirMoviesLength, "Number of letters that the name of the movie must be. To prevent 'Up' ");
            // 
            // label98
            // 
            this.label98.AutoSize = (true);
            this.label98.Location = (new global::System.Drawing.Point(414, 148));
            this.label98.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label98.Name = ("label98");
            this.label98.Size = (new global::System.Drawing.Size(50, 15));
            this.label98.TabIndex = (49);
            this.label98.Text = ("seconds");
            this.toolTip1.SetToolTip(this.label98, "If checked the system will automatically scan and complete actions on a periodic schedule");
            // 
            // upDownScanSeconds
            // 
            this.upDownScanSeconds.Items.Add("120");
            this.upDownScanSeconds.Items.Add("60");
            this.upDownScanSeconds.Items.Add("30");
            this.upDownScanSeconds.Items.Add("15");
            this.upDownScanSeconds.Items.Add("10");
            this.upDownScanSeconds.Items.Add("5");
            this.upDownScanSeconds.Items.Add("4");
            this.upDownScanSeconds.Items.Add("3");
            this.upDownScanSeconds.Items.Add("2");
            this.upDownScanSeconds.Items.Add("1");
            this.upDownScanSeconds.Location = (new global::System.Drawing.Point(363, 145));
            this.upDownScanSeconds.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.upDownScanSeconds.Name = ("upDownScanSeconds");
            this.upDownScanSeconds.Size = (new global::System.Drawing.Size(47, 23));
            this.upDownScanSeconds.TabIndex = (48);
            this.toolTip1.SetToolTip(this.upDownScanSeconds, "If checked the system will automatically scan and complete actions on a periodic schedule");
            // 
            // cmDefaults
            // 
            this.cmDefaults.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[] { this.KODIToolStripMenuItem, this.pyTivoToolStripMenuItem, this.mede8erToolStripMenuItem, this.noneToolStripMenuItem });
            this.cmDefaults.Name = ("cmDefaults");
            this.cmDefaults.Size = (new global::System.Drawing.Size(121, 92));
            this.cmDefaults.ItemClicked += (this.cmDefaults_ItemClicked);
            // 
            // KODIToolStripMenuItem
            // 
            this.KODIToolStripMenuItem.Name = ("KODIToolStripMenuItem");
            this.KODIToolStripMenuItem.Size = (new global::System.Drawing.Size(120, 22));
            this.KODIToolStripMenuItem.Tag = ("1");
            this.KODIToolStripMenuItem.Text = ("&KODI");
            // 
            // pyTivoToolStripMenuItem
            // 
            this.pyTivoToolStripMenuItem.Name = ("pyTivoToolStripMenuItem");
            this.pyTivoToolStripMenuItem.Size = (new global::System.Drawing.Size(120, 22));
            this.pyTivoToolStripMenuItem.Tag = ("2");
            this.pyTivoToolStripMenuItem.Text = ("&pyTivo");
            // 
            // mede8erToolStripMenuItem
            // 
            this.mede8erToolStripMenuItem.Name = ("mede8erToolStripMenuItem");
            this.mede8erToolStripMenuItem.Size = (new global::System.Drawing.Size(120, 22));
            this.mede8erToolStripMenuItem.Tag = ("3");
            this.mede8erToolStripMenuItem.Text = ("&Mede8er");
            // 
            // noneToolStripMenuItem
            // 
            this.noneToolStripMenuItem.Name = ("noneToolStripMenuItem");
            this.noneToolStripMenuItem.Size = (new global::System.Drawing.Size(120, 22));
            this.noneToolStripMenuItem.Tag = ("4");
            this.noneToolStripMenuItem.Text = ("&None");
            // 
            // tpDisplay
            // 
            this.tpDisplay.Controls.Add(this.chkShowAccessibilityOptions);
            this.tpDisplay.Controls.Add(this.cbUseColoursOnWtw);
            this.tpDisplay.Controls.Add(this.chkBasicShowDetails);
            this.tpDisplay.Controls.Add(this.chkPostpendThe);
            this.tpDisplay.Controls.Add(this.groupBox11);
            this.tpDisplay.Controls.Add(this.label61);
            this.tpDisplay.Controls.Add(this.cbLeadingZero);
            this.tpDisplay.Controls.Add(this.txtSeasonFolderName);
            this.tpDisplay.Controls.Add(this.label35);
            this.tpDisplay.Controls.Add(this.chkHideWtWSpoilers);
            this.tpDisplay.Controls.Add(this.chkHideMyShowsSpoilers);
            this.tpDisplay.Controls.Add(this.rbWTWScan);
            this.tpDisplay.Controls.Add(this.rbWTWSearch);
            this.tpDisplay.Controls.Add(this.cbStartupTab);
            this.tpDisplay.Controls.Add(this.cbAutoSelInMyShows);
            this.tpDisplay.Controls.Add(this.cbShowEpisodePictures);
            this.tpDisplay.Controls.Add(this.label11);
            this.tpDisplay.Controls.Add(this.label6);
            this.tpDisplay.Controls.Add(this.chkShowInTaskbar);
            this.tpDisplay.Controls.Add(this.cbNotificationIcon);
            this.tpDisplay.Controls.Add(this.pbDisplay);
            this.tpDisplay.Location = (new global::System.Drawing.Point(149, 4));
            this.tpDisplay.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpDisplay.Name = ("tpDisplay");
            this.tpDisplay.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpDisplay.Size = (new global::System.Drawing.Size(500, 684));
            this.tpDisplay.TabIndex = (13);
            this.tpDisplay.Text = ("Display");
            this.tpDisplay.UseVisualStyleBackColor = (true);
            // 
            // chkShowAccessibilityOptions
            // 
            this.chkShowAccessibilityOptions.AutoSize = (true);
            this.chkShowAccessibilityOptions.Location = (new global::System.Drawing.Point(257, 332));
            this.chkShowAccessibilityOptions.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkShowAccessibilityOptions.Name = ("chkShowAccessibilityOptions");
            this.chkShowAccessibilityOptions.Size = (new global::System.Drawing.Size(165, 19));
            this.chkShowAccessibilityOptions.TabIndex = (45);
            this.chkShowAccessibilityOptions.Text = ("Show Accessibilty Options");
            this.chkShowAccessibilityOptions.UseVisualStyleBackColor = (true);
            // 
            // cbUseColoursOnWtw
            // 
            this.cbUseColoursOnWtw.AutoSize = (true);
            this.cbUseColoursOnWtw.Location = (new global::System.Drawing.Point(257, 359));
            this.cbUseColoursOnWtw.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbUseColoursOnWtw.Name = ("cbUseColoursOnWtw");
            this.cbUseColoursOnWtw.Size = (new global::System.Drawing.Size(157, 19));
            this.cbUseColoursOnWtw.TabIndex = (44);
            this.cbUseColoursOnWtw.Text = ("Use Colours on Schedule");
            this.cbUseColoursOnWtw.UseVisualStyleBackColor = (true);
            // 
            // chkBasicShowDetails
            // 
            this.chkBasicShowDetails.AutoSize = (true);
            this.chkBasicShowDetails.Location = (new global::System.Drawing.Point(12, 359));
            this.chkBasicShowDetails.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkBasicShowDetails.Name = ("chkBasicShowDetails");
            this.chkBasicShowDetails.Size = (new global::System.Drawing.Size(155, 19));
            this.chkBasicShowDetails.TabIndex = (43);
            this.chkBasicShowDetails.Text = ("Show Basic Show Details");
            this.chkBasicShowDetails.UseVisualStyleBackColor = (true);
            // 
            // chkPostpendThe
            // 
            this.chkPostpendThe.AutoSize = (true);
            this.chkPostpendThe.Location = (new global::System.Drawing.Point(12, 249));
            this.chkPostpendThe.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkPostpendThe.Name = ("chkPostpendThe");
            this.chkPostpendThe.Size = (new global::System.Drawing.Size(275, 19));
            this.chkPostpendThe.TabIndex = (42);
            this.chkPostpendThe.Text = ("Move 'The' to the end of tv show/movie names");
            this.chkPostpendThe.UseVisualStyleBackColor = (true);
            // 
            // groupBox11
            // 
            this.groupBox11.Anchor = ((global::System.Windows.Forms.AnchorStyles)((((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Bottom)) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox11.Controls.Add(this.label7);
            this.groupBox11.Controls.Add(this.cboShowStatus);
            this.groupBox11.Controls.Add(this.label5);
            this.groupBox11.Controls.Add(this.txtShowStatusColor);
            this.groupBox11.Controls.Add(this.btnSelectColor);
            this.groupBox11.Controls.Add(this.bnRemoveDefinedColor);
            this.groupBox11.Controls.Add(this.btnAddShowStatusColoring);
            this.groupBox11.Controls.Add(this.lvwDefinedColors);
            this.groupBox11.Location = (new global::System.Drawing.Point(7, 385));
            this.groupBox11.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox11.Name = ("groupBox11");
            this.groupBox11.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox11.Size = (new global::System.Drawing.Size(457, 255));
            this.groupBox11.TabIndex = (41);
            this.groupBox11.TabStop = (false);
            this.groupBox11.Text = ("Show Colouring");
            // 
            // label7
            // 
            this.label7.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = (true);
            this.label7.Location = (new global::System.Drawing.Point(2, 195));
            this.label7.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label7.Name = ("label7");
            this.label7.Size = (new global::System.Drawing.Size(42, 15));
            this.label7.TabIndex = (16);
            this.label7.Text = ("&Status:");
            // 
            // cboShowStatus
            // 
            this.cboShowStatus.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.cboShowStatus.DropDownStyle = (global::System.Windows.Forms.ComboBoxStyle.DropDownList);
            this.cboShowStatus.FormattingEnabled = (true);
            this.cboShowStatus.Location = (new global::System.Drawing.Point(58, 192));
            this.cboShowStatus.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cboShowStatus.Name = ("cboShowStatus");
            this.cboShowStatus.Size = (new global::System.Drawing.Size(403, 23));
            this.cboShowStatus.TabIndex = (15);
            // 
            // label5
            // 
            this.label5.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = (true);
            this.label5.Location = (new global::System.Drawing.Point(4, 231));
            this.label5.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label5.Name = ("label5");
            this.label5.Size = (new global::System.Drawing.Size(63, 15));
            this.label5.TabIndex = (14);
            this.label5.Text = ("&Text Color:");
            // 
            // txtShowStatusColor
            // 
            this.txtShowStatusColor.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.txtShowStatusColor.Location = (new global::System.Drawing.Point(78, 223));
            this.txtShowStatusColor.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtShowStatusColor.Name = ("txtShowStatusColor");
            this.txtShowStatusColor.Size = (new global::System.Drawing.Size(116, 23));
            this.txtShowStatusColor.TabIndex = (13);
            this.txtShowStatusColor.TextChanged += (this.txtShowStatusColor_TextChanged);
            // 
            // btnSelectColor
            // 
            this.btnSelectColor.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectColor.Location = (new global::System.Drawing.Point(202, 222));
            this.btnSelectColor.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.btnSelectColor.Name = ("btnSelectColor");
            this.btnSelectColor.Size = (new global::System.Drawing.Size(88, 27));
            this.btnSelectColor.TabIndex = (12);
            this.btnSelectColor.Text = ("Select &Color");
            this.btnSelectColor.UseVisualStyleBackColor = (true);
            this.btnSelectColor.Click += (this.btnSelectColor_Click);
            // 
            // bnRemoveDefinedColor
            // 
            this.bnRemoveDefinedColor.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.bnRemoveDefinedColor.Enabled = (false);
            this.bnRemoveDefinedColor.Location = (new global::System.Drawing.Point(4, 158));
            this.bnRemoveDefinedColor.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnRemoveDefinedColor.Name = ("bnRemoveDefinedColor");
            this.bnRemoveDefinedColor.Size = (new global::System.Drawing.Size(88, 27));
            this.bnRemoveDefinedColor.TabIndex = (10);
            this.bnRemoveDefinedColor.Text = ("&Remove");
            this.bnRemoveDefinedColor.UseVisualStyleBackColor = (true);
            this.bnRemoveDefinedColor.Click += (this.bnRemoveDefinedColor_Click);
            // 
            // btnAddShowStatusColoring
            // 
            this.btnAddShowStatusColoring.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddShowStatusColoring.Location = (new global::System.Drawing.Point(363, 222));
            this.btnAddShowStatusColoring.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.btnAddShowStatusColoring.Name = ("btnAddShowStatusColoring");
            this.btnAddShowStatusColoring.Size = (new global::System.Drawing.Size(88, 27));
            this.btnAddShowStatusColoring.TabIndex = (11);
            this.btnAddShowStatusColoring.Text = ("&Add");
            this.btnAddShowStatusColoring.UseVisualStyleBackColor = (true);
            this.btnAddShowStatusColoring.Click += (this.btnAddShowStatusColoring_Click);
            // 
            // lvwDefinedColors
            // 
            this.lvwDefinedColors.Anchor = ((global::System.Windows.Forms.AnchorStyles)((((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Bottom)) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.lvwDefinedColors.Columns.AddRange(new global::System.Windows.Forms.ColumnHeader[] { this.colShowStatus, this.colColor });
            this.lvwDefinedColors.GridLines = (true);
            this.lvwDefinedColors.Location = (new global::System.Drawing.Point(7, 22));
            this.lvwDefinedColors.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.lvwDefinedColors.MultiSelect = (false);
            this.lvwDefinedColors.Name = ("lvwDefinedColors");
            this.lvwDefinedColors.Size = (new global::System.Drawing.Size(443, 129));
            this.lvwDefinedColors.TabIndex = (9);
            this.lvwDefinedColors.UseCompatibleStateImageBehavior = (false);
            this.lvwDefinedColors.View = (global::System.Windows.Forms.View.Details);
            this.lvwDefinedColors.SelectedIndexChanged += (this.EnableDisable);
            this.lvwDefinedColors.DoubleClick += (this.lvwDefinedColors_DoubleClick);
            // 
            // colShowStatus
            // 
            this.colShowStatus.Text = ("Show Status");
            this.colShowStatus.Width = (297);
            // 
            // colColor
            // 
            this.colColor.Text = ("Color");
            this.colColor.Width = (92);
            // 
            // label61
            // 
            this.label61.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.label61.AutoSize = (true);
            this.label61.Location = (new global::System.Drawing.Point(8, 8));
            this.label61.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label61.Name = ("label61");
            this.label61.Size = (new global::System.Drawing.Size(324, 45));
            this.label61.TabIndex = (40);
            this.label61.Text = ("Settings that contol the way that TV Rename looks. These do\r\nnot have any impact on the main scanning, just on the way \r\nthe interface looks.");
            // 
            // cbLeadingZero
            // 
            this.cbLeadingZero.AutoSize = (true);
            this.cbLeadingZero.Location = (new global::System.Drawing.Point(12, 332));
            this.cbLeadingZero.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbLeadingZero.Name = ("cbLeadingZero");
            this.cbLeadingZero.Size = (new global::System.Drawing.Size(184, 19));
            this.cbLeadingZero.TabIndex = (38);
            this.cbLeadingZero.Text = ("&Leading 0 on Season numbers");
            this.cbLeadingZero.UseVisualStyleBackColor = (true);
            // 
            // txtSeasonFolderName
            // 
            this.txtSeasonFolderName.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtSeasonFolderName.Location = (new global::System.Drawing.Point(133, 302));
            this.txtSeasonFolderName.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtSeasonFolderName.Name = ("txtSeasonFolderName");
            this.txtSeasonFolderName.Size = (new global::System.Drawing.Size(331, 23));
            this.txtSeasonFolderName.TabIndex = (37);
            // 
            // label35
            // 
            this.label35.AutoSize = (true);
            this.label35.Location = (new global::System.Drawing.Point(8, 306));
            this.label35.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label35.Name = ("label35");
            this.label35.Size = (new global::System.Drawing.Size(85, 15));
            this.label35.TabIndex = (36);
            this.label35.Text = ("&Seasons name:");
            // 
            // chkHideWtWSpoilers
            // 
            this.chkHideWtWSpoilers.AutoSize = (true);
            this.chkHideWtWSpoilers.Location = (new global::System.Drawing.Point(257, 223));
            this.chkHideWtWSpoilers.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkHideWtWSpoilers.Name = ("chkHideWtWSpoilers");
            this.chkHideWtWSpoilers.Size = (new global::System.Drawing.Size(159, 19));
            this.chkHideWtWSpoilers.TabIndex = (35);
            this.chkHideWtWSpoilers.Text = ("Hide Spoilers in Schedule");
            this.chkHideWtWSpoilers.UseVisualStyleBackColor = (true);
            // 
            // chkHideMyShowsSpoilers
            // 
            this.chkHideMyShowsSpoilers.AutoSize = (true);
            this.chkHideMyShowsSpoilers.Location = (new global::System.Drawing.Point(12, 223));
            this.chkHideMyShowsSpoilers.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkHideMyShowsSpoilers.Name = ("chkHideMyShowsSpoilers");
            this.chkHideMyShowsSpoilers.Size = (new global::System.Drawing.Size(167, 19));
            this.chkHideMyShowsSpoilers.TabIndex = (34);
            this.chkHideMyShowsSpoilers.Text = ("Hide Spoilers in 'TV Shows'");
            this.chkHideMyShowsSpoilers.UseVisualStyleBackColor = (true);
            // 
            // rbWTWScan
            // 
            this.rbWTWScan.AutoSize = (true);
            this.rbWTWScan.Location = (new global::System.Drawing.Point(33, 113));
            this.rbWTWScan.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rbWTWScan.Name = ("rbWTWScan");
            this.rbWTWScan.Size = (new global::System.Drawing.Size(50, 19));
            this.rbWTWScan.TabIndex = (27);
            this.rbWTWScan.Text = ("S&can");
            this.rbWTWScan.UseVisualStyleBackColor = (true);
            // 
            // rbWTWSearch
            // 
            this.rbWTWSearch.AutoSize = (true);
            this.rbWTWSearch.Checked = (true);
            this.rbWTWSearch.Location = (new global::System.Drawing.Point(33, 91));
            this.rbWTWSearch.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rbWTWSearch.Name = ("rbWTWSearch");
            this.rbWTWSearch.Size = (new global::System.Drawing.Size(60, 19));
            this.rbWTWSearch.TabIndex = (26);
            this.rbWTWSearch.TabStop = (true);
            this.rbWTWSearch.Text = ("S&earch");
            this.rbWTWSearch.UseVisualStyleBackColor = (true);
            // 
            // cbStartupTab
            // 
            this.cbStartupTab.DropDownStyle = (global::System.Windows.Forms.ComboBoxStyle.DropDownList);
            this.cbStartupTab.FormattingEnabled = (true);
            this.cbStartupTab.Items.AddRange(new global::System.Object[] { "Movies", "TV Shows", "Scan", "Schedule" });
            this.cbStartupTab.Location = (new global::System.Drawing.Point(88, 138));
            this.cbStartupTab.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbStartupTab.Name = ("cbStartupTab");
            this.cbStartupTab.Size = (new global::System.Drawing.Size(157, 23));
            this.cbStartupTab.TabIndex = (29);
            // 
            // cbAutoSelInMyShows
            // 
            this.cbAutoSelInMyShows.AutoSize = (true);
            this.cbAutoSelInMyShows.Location = (new global::System.Drawing.Point(12, 276));
            this.cbAutoSelInMyShows.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbAutoSelInMyShows.Name = ("cbAutoSelInMyShows");
            this.cbAutoSelInMyShows.Size = (new global::System.Drawing.Size(298, 19));
            this.cbAutoSelInMyShows.TabIndex = (33);
            this.cbAutoSelInMyShows.Text = ("&Automatically select show and season in 'TV Shows'");
            this.cbAutoSelInMyShows.UseVisualStyleBackColor = (true);
            // 
            // cbShowEpisodePictures
            // 
            this.cbShowEpisodePictures.AutoSize = (true);
            this.cbShowEpisodePictures.Location = (new global::System.Drawing.Point(12, 196));
            this.cbShowEpisodePictures.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbShowEpisodePictures.Name = ("cbShowEpisodePictures");
            this.cbShowEpisodePictures.Size = (new global::System.Drawing.Size(239, 19));
            this.cbShowEpisodePictures.TabIndex = (32);
            this.cbShowEpisodePictures.Text = ("S&how episode pictures in episode guides");
            this.cbShowEpisodePictures.UseVisualStyleBackColor = (true);
            // 
            // label11
            // 
            this.label11.AutoSize = (true);
            this.label11.Location = (new global::System.Drawing.Point(8, 74));
            this.label11.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label11.Name = ("label11");
            this.label11.Size = (new global::System.Drawing.Size(169, 15));
            this.label11.TabIndex = (25);
            this.label11.Text = ("Double-click in Schedule does:");
            // 
            // label6
            // 
            this.label6.AutoSize = (true);
            this.label6.Location = (new global::System.Drawing.Point(8, 142));
            this.label6.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label6.Name = ("label6");
            this.label6.Size = (new global::System.Drawing.Size(68, 15));
            this.label6.TabIndex = (28);
            this.label6.Text = ("&Startup tab:");
            // 
            // chkShowInTaskbar
            // 
            this.chkShowInTaskbar.AutoSize = (true);
            this.chkShowInTaskbar.Location = (new global::System.Drawing.Point(257, 170));
            this.chkShowInTaskbar.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkShowInTaskbar.Name = ("chkShowInTaskbar");
            this.chkShowInTaskbar.Size = (new global::System.Drawing.Size(109, 19));
            this.chkShowInTaskbar.TabIndex = (31);
            this.chkShowInTaskbar.Text = ("Show in &taskbar");
            this.chkShowInTaskbar.UseVisualStyleBackColor = (true);
            this.chkShowInTaskbar.CheckedChanged += (this.EnableDisable);
            // 
            // cbNotificationIcon
            // 
            this.cbNotificationIcon.AutoSize = (true);
            this.cbNotificationIcon.Location = (new global::System.Drawing.Point(12, 170));
            this.cbNotificationIcon.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbNotificationIcon.Name = ("cbNotificationIcon");
            this.cbNotificationIcon.Size = (new global::System.Drawing.Size(170, 19));
            this.cbNotificationIcon.TabIndex = (30);
            this.cbNotificationIcon.Text = ("Show &notification area icon");
            this.cbNotificationIcon.UseVisualStyleBackColor = (true);
            this.cbNotificationIcon.CheckedChanged += (this.EnableDisable);
            // 
            // pbDisplay
            // 
            this.pbDisplay.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.pbDisplay.Cursor = (global::System.Windows.Forms.Cursors.Hand);
            this.pbDisplay.Image = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbDisplay.InitialImage = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbDisplay.Location = (new global::System.Drawing.Point(418, 7));
            this.pbDisplay.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.pbDisplay.Name = ("pbDisplay");
            this.pbDisplay.Size = (new global::System.Drawing.Size(50, 46));
            this.pbDisplay.SizeMode = (global::System.Windows.Forms.PictureBoxSizeMode.CenterImage);
            this.pbDisplay.TabIndex = (39);
            this.pbDisplay.TabStop = (false);
            this.pbDisplay.Click += (this.pbDisplay_Click);
            // 
            // tpRSSJSONSearch
            // 
            this.tpRSSJSONSearch.Controls.Add(this.pbRSSJSONSearch);
            this.tpRSSJSONSearch.Controls.Add(this.label59);
            this.tpRSSJSONSearch.Controls.Add(this.cbSearchJSON);
            this.tpRSSJSONSearch.Controls.Add(this.cbSearchRSS);
            this.tpRSSJSONSearch.Controls.Add(this.gbJSON);
            this.tpRSSJSONSearch.Controls.Add(this.gbRSS);
            this.tpRSSJSONSearch.Location = (new global::System.Drawing.Point(149, 4));
            this.tpRSSJSONSearch.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpRSSJSONSearch.Name = ("tpRSSJSONSearch");
            this.tpRSSJSONSearch.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpRSSJSONSearch.Size = (new global::System.Drawing.Size(500, 684));
            this.tpRSSJSONSearch.TabIndex = (12);
            this.tpRSSJSONSearch.Text = ("RSS/JSON Search");
            this.tpRSSJSONSearch.UseVisualStyleBackColor = (true);
            // 
            // pbRSSJSONSearch
            // 
            this.pbRSSJSONSearch.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.pbRSSJSONSearch.Cursor = (global::System.Windows.Forms.Cursors.Hand);
            this.pbRSSJSONSearch.Image = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbRSSJSONSearch.InitialImage = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbRSSJSONSearch.Location = (new global::System.Drawing.Point(418, 7));
            this.pbRSSJSONSearch.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.pbRSSJSONSearch.Name = ("pbRSSJSONSearch");
            this.pbRSSJSONSearch.Size = (new global::System.Drawing.Size(50, 46));
            this.pbRSSJSONSearch.SizeMode = (global::System.Windows.Forms.PictureBoxSizeMode.CenterImage);
            this.pbRSSJSONSearch.TabIndex = (38);
            this.pbRSSJSONSearch.TabStop = (false);
            this.pbRSSJSONSearch.Click += (this.pbRSSJSONSearch_Click);
            // 
            // label59
            // 
            this.label59.AutoSize = (true);
            this.label59.Location = (new global::System.Drawing.Point(4, 15));
            this.label59.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label59.Name = ("label59");
            this.label59.Size = (new global::System.Drawing.Size(379, 45));
            this.label59.TabIndex = (37);
            this.label59.Text = ("If an episode is missing from your library, TV Rename will look in the \r\nfollowing URLs for appropriate files to download. It will use the torrent \r\nhandlers to download the file(s)");
            // 
            // cbSearchJSON
            // 
            this.cbSearchJSON.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.cbSearchJSON.AutoSize = (true);
            this.cbSearchJSON.Location = (new global::System.Drawing.Point(9, 370));
            this.cbSearchJSON.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbSearchJSON.Name = ("cbSearchJSON");
            this.cbSearchJSON.Size = (new global::System.Drawing.Size(178, 19));
            this.cbSearchJSON.TabIndex = (36);
            this.cbSearchJSON.Text = ("Search &JSON for missing files");
            this.cbSearchJSON.UseVisualStyleBackColor = (true);
            this.cbSearchJSON.CheckedChanged += (this.EnableDisable);
            // 
            // cbSearchRSS
            // 
            this.cbSearchRSS.AutoSize = (true);
            this.cbSearchRSS.Location = (new global::System.Drawing.Point(7, 76));
            this.cbSearchRSS.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbSearchRSS.Name = ("cbSearchRSS");
            this.cbSearchRSS.Size = (new global::System.Drawing.Size(169, 19));
            this.cbSearchRSS.TabIndex = (35);
            this.cbSearchRSS.Text = ("&Search RSS for missing files");
            this.cbSearchRSS.UseVisualStyleBackColor = (true);
            this.cbSearchRSS.CheckedChanged += (this.EnableDisable);
            // 
            // gbJSON
            // 
            this.gbJSON.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.gbJSON.Controls.Add(this.label78);
            this.gbJSON.Controls.Add(this.tbJSONSeedersToken);
            this.gbJSON.Controls.Add(this.cbJSONCloudflareProtection);
            this.gbJSON.Controls.Add(this.cbSearchJSONManualScanOnly);
            this.gbJSON.Controls.Add(this.label55);
            this.gbJSON.Controls.Add(this.tbJSONFilesizeToken);
            this.gbJSON.Controls.Add(this.label51);
            this.gbJSON.Controls.Add(this.tbJSONFilenameToken);
            this.gbJSON.Controls.Add(this.label50);
            this.gbJSON.Controls.Add(this.tbJSONURLToken);
            this.gbJSON.Controls.Add(this.label49);
            this.gbJSON.Controls.Add(this.tbJSONRootNode);
            this.gbJSON.Controls.Add(this.label48);
            this.gbJSON.Controls.Add(this.tbJSONURL);
            this.gbJSON.Location = (new global::System.Drawing.Point(7, 396));
            this.gbJSON.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.gbJSON.Name = ("gbJSON");
            this.gbJSON.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.gbJSON.Size = (new global::System.Drawing.Size(464, 230));
            this.gbJSON.TabIndex = (33);
            this.gbJSON.TabStop = (false);
            this.gbJSON.Text = ("JSON Search");
            // 
            // label78
            // 
            this.label78.AutoSize = (true);
            this.label78.Location = (new global::System.Drawing.Point(8, 202));
            this.label78.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label78.Name = ("label78");
            this.label78.Size = (new global::System.Drawing.Size(84, 15));
            this.label78.TabIndex = (42);
            this.label78.Text = ("Seeders Token:");
            // 
            // tbJSONSeedersToken
            // 
            this.tbJSONSeedersToken.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.tbJSONSeedersToken.Location = (new global::System.Drawing.Point(114, 198));
            this.tbJSONSeedersToken.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbJSONSeedersToken.Name = ("tbJSONSeedersToken");
            this.tbJSONSeedersToken.Size = (new global::System.Drawing.Size(344, 23));
            this.tbJSONSeedersToken.TabIndex = (41);
            // 
            // cbJSONCloudflareProtection
            // 
            this.cbJSONCloudflareProtection.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.cbJSONCloudflareProtection.AutoSize = (true);
            this.cbJSONCloudflareProtection.Location = (new global::System.Drawing.Point(292, 22));
            this.cbJSONCloudflareProtection.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbJSONCloudflareProtection.Name = ("cbJSONCloudflareProtection");
            this.cbJSONCloudflareProtection.Size = (new global::System.Drawing.Size(161, 19));
            this.cbJSONCloudflareProtection.TabIndex = (40);
            this.cbJSONCloudflareProtection.Text = ("Use Cloudflare protection");
            this.cbJSONCloudflareProtection.UseVisualStyleBackColor = (true);
            // 
            // cbSearchJSONManualScanOnly
            // 
            this.cbSearchJSONManualScanOnly.AutoSize = (true);
            this.cbSearchJSONManualScanOnly.Location = (new global::System.Drawing.Point(10, 22));
            this.cbSearchJSONManualScanOnly.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbSearchJSONManualScanOnly.Name = ("cbSearchJSONManualScanOnly");
            this.cbSearchJSONManualScanOnly.Size = (new global::System.Drawing.Size(143, 19));
            this.cbSearchJSONManualScanOnly.TabIndex = (38);
            this.cbSearchJSONManualScanOnly.Text = ("Only on manual scans");
            this.cbSearchJSONManualScanOnly.UseVisualStyleBackColor = (true);
            // 
            // label55
            // 
            this.label55.AutoSize = (true);
            this.label55.Location = (new global::System.Drawing.Point(7, 172));
            this.label55.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label55.Name = ("label55");
            this.label55.Size = (new global::System.Drawing.Size(64, 15));
            this.label55.TabIndex = (39);
            this.label55.Text = ("Size Token:");
            // 
            // tbJSONFilesizeToken
            // 
            this.tbJSONFilesizeToken.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.tbJSONFilesizeToken.Location = (new global::System.Drawing.Point(113, 168));
            this.tbJSONFilesizeToken.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbJSONFilesizeToken.Name = ("tbJSONFilesizeToken");
            this.tbJSONFilesizeToken.Size = (new global::System.Drawing.Size(344, 23));
            this.tbJSONFilesizeToken.TabIndex = (38);
            // 
            // label51
            // 
            this.label51.AutoSize = (true);
            this.label51.Location = (new global::System.Drawing.Point(7, 112));
            this.label51.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label51.Name = ("label51");
            this.label51.Size = (new global::System.Drawing.Size(92, 15));
            this.label51.TabIndex = (37);
            this.label51.Text = ("Filename Token:");
            // 
            // tbJSONFilenameToken
            // 
            this.tbJSONFilenameToken.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.tbJSONFilenameToken.Location = (new global::System.Drawing.Point(114, 108));
            this.tbJSONFilenameToken.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbJSONFilenameToken.Name = ("tbJSONFilenameToken");
            this.tbJSONFilenameToken.Size = (new global::System.Drawing.Size(344, 23));
            this.tbJSONFilenameToken.TabIndex = (36);
            // 
            // label50
            // 
            this.label50.AutoSize = (true);
            this.label50.Location = (new global::System.Drawing.Point(7, 142));
            this.label50.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label50.Name = ("label50");
            this.label50.Size = (new global::System.Drawing.Size(65, 15));
            this.label50.TabIndex = (35);
            this.label50.Text = ("URL Token:");
            // 
            // tbJSONURLToken
            // 
            this.tbJSONURLToken.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.tbJSONURLToken.Location = (new global::System.Drawing.Point(114, 138));
            this.tbJSONURLToken.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbJSONURLToken.Name = ("tbJSONURLToken");
            this.tbJSONURLToken.Size = (new global::System.Drawing.Size(344, 23));
            this.tbJSONURLToken.TabIndex = (34);
            // 
            // label49
            // 
            this.label49.AutoSize = (true);
            this.label49.Location = (new global::System.Drawing.Point(7, 82));
            this.label49.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label49.Name = ("label49");
            this.label49.Size = (new global::System.Drawing.Size(67, 15));
            this.label49.TabIndex = (33);
            this.label49.Text = ("Root Node:");
            // 
            // tbJSONRootNode
            // 
            this.tbJSONRootNode.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.tbJSONRootNode.Location = (new global::System.Drawing.Point(114, 78));
            this.tbJSONRootNode.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbJSONRootNode.Name = ("tbJSONRootNode");
            this.tbJSONRootNode.Size = (new global::System.Drawing.Size(344, 23));
            this.tbJSONRootNode.TabIndex = (32);
            // 
            // label48
            // 
            this.label48.AutoSize = (true);
            this.label48.Location = (new global::System.Drawing.Point(7, 52));
            this.label48.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label48.Name = ("label48");
            this.label48.Size = (new global::System.Drawing.Size(31, 15));
            this.label48.TabIndex = (31);
            this.label48.Text = ("URL:");
            // 
            // tbJSONURL
            // 
            this.tbJSONURL.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.tbJSONURL.Location = (new global::System.Drawing.Point(113, 48));
            this.tbJSONURL.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbJSONURL.Name = ("tbJSONURL");
            this.tbJSONURL.Size = (new global::System.Drawing.Size(344, 23));
            this.tbJSONURL.TabIndex = (30);
            // 
            // gbRSS
            // 
            this.gbRSS.Anchor = ((global::System.Windows.Forms.AnchorStyles)((((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Bottom)) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.gbRSS.Controls.Add(this.cbRSSCloudflareProtection);
            this.gbRSS.Controls.Add(this.cbSearchRSSManualScanOnly);
            this.gbRSS.Controls.Add(this.RSSGrid);
            this.gbRSS.Controls.Add(this.label25);
            this.gbRSS.Controls.Add(this.bnRSSRemove);
            this.gbRSS.Controls.Add(this.bnRSSGo);
            this.gbRSS.Controls.Add(this.bnRSSAdd);
            this.gbRSS.Location = (new global::System.Drawing.Point(4, 99));
            this.gbRSS.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.gbRSS.Name = ("gbRSS");
            this.gbRSS.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.gbRSS.Size = (new global::System.Drawing.Size(463, 263));
            this.gbRSS.TabIndex = (32);
            this.gbRSS.TabStop = (false);
            this.gbRSS.Text = ("RSS Search");
            // 
            // cbRSSCloudflareProtection
            // 
            this.cbRSSCloudflareProtection.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.cbRSSCloudflareProtection.AutoSize = (true);
            this.cbRSSCloudflareProtection.Location = (new global::System.Drawing.Point(295, 22));
            this.cbRSSCloudflareProtection.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbRSSCloudflareProtection.Name = ("cbRSSCloudflareProtection");
            this.cbRSSCloudflareProtection.Size = (new global::System.Drawing.Size(161, 19));
            this.cbRSSCloudflareProtection.TabIndex = (41);
            this.cbRSSCloudflareProtection.Text = ("Use Cloudflare protection");
            this.cbRSSCloudflareProtection.UseVisualStyleBackColor = (true);
            // 
            // cbSearchRSSManualScanOnly
            // 
            this.cbSearchRSSManualScanOnly.AutoSize = (true);
            this.cbSearchRSSManualScanOnly.Location = (new global::System.Drawing.Point(7, 22));
            this.cbSearchRSSManualScanOnly.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbSearchRSSManualScanOnly.Name = ("cbSearchRSSManualScanOnly");
            this.cbSearchRSSManualScanOnly.Size = (new global::System.Drawing.Size(143, 19));
            this.cbSearchRSSManualScanOnly.TabIndex = (37);
            this.cbSearchRSSManualScanOnly.Text = ("Only on manual scans");
            this.cbSearchRSSManualScanOnly.UseVisualStyleBackColor = (true);
            // 
            // RSSGrid
            // 
            this.RSSGrid.Anchor = ((global::System.Windows.Forms.AnchorStyles)((((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Bottom)) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.RSSGrid.BackColor = (global::System.Drawing.SystemColors.Window);
            this.RSSGrid.EnableSort = (true);
            this.RSSGrid.Location = (new global::System.Drawing.Point(7, 63));
            this.RSSGrid.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.RSSGrid.Name = ("RSSGrid");
            this.RSSGrid.OptimizeMode = (global::SourceGrid.CellOptimizeMode.ForRows);
            this.RSSGrid.SelectionMode = (global::SourceGrid.GridSelectionMode.Cell);
            this.RSSGrid.Size = (new global::System.Drawing.Size(449, 155));
            this.RSSGrid.TabIndex = (26);
            this.RSSGrid.TabStop = (true);
            this.RSSGrid.ToolTipText = ("");
            // 
            // label25
            // 
            this.label25.AutoSize = (true);
            this.label25.Location = (new global::System.Drawing.Point(4, 45));
            this.label25.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label25.Name = ("label25");
            this.label25.Size = (new global::System.Drawing.Size(98, 15));
            this.label25.TabIndex = (25);
            this.label25.Text = ("Torrent RSS URLs:");
            // 
            // bnRSSRemove
            // 
            this.bnRSSRemove.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.bnRSSRemove.Location = (new global::System.Drawing.Point(102, 228));
            this.bnRSSRemove.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnRSSRemove.Name = ("bnRSSRemove");
            this.bnRSSRemove.Size = (new global::System.Drawing.Size(88, 27));
            this.bnRSSRemove.TabIndex = (28);
            this.bnRSSRemove.Text = ("&Remove");
            this.bnRSSRemove.UseVisualStyleBackColor = (true);
            this.bnRSSRemove.Click += (this.bnRSSRemove_Click);
            // 
            // bnRSSGo
            // 
            this.bnRSSGo.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.bnRSSGo.Location = (new global::System.Drawing.Point(196, 228));
            this.bnRSSGo.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnRSSGo.Name = ("bnRSSGo");
            this.bnRSSGo.Size = (new global::System.Drawing.Size(88, 27));
            this.bnRSSGo.TabIndex = (29);
            this.bnRSSGo.Text = ("&Open");
            this.bnRSSGo.UseVisualStyleBackColor = (true);
            this.bnRSSGo.Click += (this.bnRSSGo_Click);
            // 
            // bnRSSAdd
            // 
            this.bnRSSAdd.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.bnRSSAdd.Location = (new global::System.Drawing.Point(7, 228));
            this.bnRSSAdd.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnRSSAdd.Name = ("bnRSSAdd");
            this.bnRSSAdd.Size = (new global::System.Drawing.Size(88, 27));
            this.bnRSSAdd.TabIndex = (27);
            this.bnRSSAdd.Text = ("&Add");
            this.bnRSSAdd.UseVisualStyleBackColor = (true);
            this.bnRSSAdd.Click += (this.bnRSSAdd_Click);
            // 
            // tpLibraryFolders
            // 
            this.tpLibraryFolders.Controls.Add(this.groupBox23);
            this.tpLibraryFolders.Controls.Add(this.label87);
            this.tpLibraryFolders.Controls.Add(this.bnOpenMovieMonFolder);
            this.tpLibraryFolders.Controls.Add(this.bnAddMovieMonFolder);
            this.tpLibraryFolders.Controls.Add(this.bnRemoveMovieMonFolder);
            this.tpLibraryFolders.Controls.Add(this.lstMovieMonitorFolders);
            this.tpLibraryFolders.Controls.Add(this.groupBox6);
            this.tpLibraryFolders.Controls.Add(this.label65);
            this.tpLibraryFolders.Controls.Add(this.label56);
            this.tpLibraryFolders.Controls.Add(this.bnOpenMonFolder);
            this.tpLibraryFolders.Controls.Add(this.bnAddMonFolder);
            this.tpLibraryFolders.Controls.Add(this.bnRemoveMonFolder);
            this.tpLibraryFolders.Controls.Add(this.pbLibraryFolders);
            this.tpLibraryFolders.Controls.Add(this.lstFMMonitorFolders);
            this.tpLibraryFolders.Location = (new global::System.Drawing.Point(149, 4));
            this.tpLibraryFolders.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpLibraryFolders.Name = ("tpLibraryFolders");
            this.tpLibraryFolders.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpLibraryFolders.Size = (new global::System.Drawing.Size(500, 684));
            this.tpLibraryFolders.TabIndex = (10);
            this.tpLibraryFolders.Text = ("Library Folders");
            this.tpLibraryFolders.UseVisualStyleBackColor = (true);
            // 
            // groupBox23
            // 
            this.groupBox23.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox23.Controls.Add(this.button3);
            this.groupBox23.Controls.Add(this.txtMovieFilenameFormat);
            this.groupBox23.Controls.Add(this.label90);
            this.groupBox23.Controls.Add(this.button2);
            this.groupBox23.Controls.Add(this.txtMovieFolderFormat);
            this.groupBox23.Controls.Add(this.label85);
            this.groupBox23.Location = (new global::System.Drawing.Point(10, 525));
            this.groupBox23.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox23.Name = ("groupBox23");
            this.groupBox23.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox23.Size = (new global::System.Drawing.Size(456, 99));
            this.groupBox23.TabIndex = (54);
            this.groupBox23.TabStop = (false);
            this.groupBox23.Text = ("Default Movie Library Folder Format");
            // 
            // button3
            // 
            this.button3.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = (new global::System.Drawing.Point(377, 59));
            this.button3.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.button3.Name = ("button3");
            this.button3.Size = (new global::System.Drawing.Size(72, 27));
            this.button3.TabIndex = (34);
            this.button3.Text = ("Tags...");
            this.button3.UseVisualStyleBackColor = (true);
            this.button3.Click += (this.button3_Click);
            // 
            // txtMovieFilenameFormat
            // 
            this.txtMovieFilenameFormat.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtMovieFilenameFormat.Location = (new global::System.Drawing.Point(145, 60));
            this.txtMovieFilenameFormat.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtMovieFilenameFormat.Name = ("txtMovieFilenameFormat");
            this.txtMovieFilenameFormat.Size = (new global::System.Drawing.Size(224, 23));
            this.txtMovieFilenameFormat.TabIndex = (33);
            // 
            // label90
            // 
            this.label90.AutoSize = (true);
            this.label90.Location = (new global::System.Drawing.Point(7, 65));
            this.label90.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label90.Name = ("label90");
            this.label90.Size = (new global::System.Drawing.Size(97, 15));
            this.label90.TabIndex = (32);
            this.label90.Text = ("Filename format:");
            // 
            // button2
            // 
            this.button2.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = (new global::System.Drawing.Point(377, 25));
            this.button2.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.button2.Name = ("button2");
            this.button2.Size = (new global::System.Drawing.Size(72, 27));
            this.button2.TabIndex = (31);
            this.button2.Text = ("Tags...");
            this.button2.UseVisualStyleBackColor = (true);
            this.button2.Click += (this.button2_Click);
            // 
            // txtMovieFolderFormat
            // 
            this.txtMovieFolderFormat.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtMovieFolderFormat.Location = (new global::System.Drawing.Point(145, 27));
            this.txtMovieFolderFormat.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtMovieFolderFormat.Name = ("txtMovieFolderFormat");
            this.txtMovieFolderFormat.Size = (new global::System.Drawing.Size(224, 23));
            this.txtMovieFolderFormat.TabIndex = (30);
            // 
            // label85
            // 
            this.label85.AutoSize = (true);
            this.label85.Location = (new global::System.Drawing.Point(7, 31));
            this.label85.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label85.Name = ("label85");
            this.label85.Size = (new global::System.Drawing.Size(82, 15));
            this.label85.TabIndex = (29);
            this.label85.Text = ("&Folder format:");
            // 
            // label87
            // 
            this.label87.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.label87.AutoSize = (true);
            this.label87.Location = (new global::System.Drawing.Point(7, 354));
            this.label87.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label87.Name = ("label87");
            this.label87.Size = (new global::System.Drawing.Size(120, 15));
            this.label87.TabIndex = (53);
            this.label87.Text = ("Movie &Library Folders");
            // 
            // bnOpenMovieMonFolder
            // 
            this.bnOpenMovieMonFolder.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.bnOpenMovieMonFolder.Enabled = (false);
            this.bnOpenMovieMonFolder.Location = (new global::System.Drawing.Point(196, 492));
            this.bnOpenMovieMonFolder.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnOpenMovieMonFolder.Name = ("bnOpenMovieMonFolder");
            this.bnOpenMovieMonFolder.Size = (new global::System.Drawing.Size(88, 27));
            this.bnOpenMovieMonFolder.TabIndex = (52);
            this.bnOpenMovieMonFolder.Text = ("&Open");
            this.bnOpenMovieMonFolder.UseVisualStyleBackColor = (true);
            this.bnOpenMovieMonFolder.Click += (this.bnOpenMovieMonFolder_Click);
            // 
            // bnAddMovieMonFolder
            // 
            this.bnAddMovieMonFolder.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.bnAddMovieMonFolder.Location = (new global::System.Drawing.Point(7, 492));
            this.bnAddMovieMonFolder.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnAddMovieMonFolder.Name = ("bnAddMovieMonFolder");
            this.bnAddMovieMonFolder.Size = (new global::System.Drawing.Size(88, 27));
            this.bnAddMovieMonFolder.TabIndex = (50);
            this.bnAddMovieMonFolder.Text = ("&Add");
            this.bnAddMovieMonFolder.UseVisualStyleBackColor = (true);
            this.bnAddMovieMonFolder.Click += (this.bnAddMovieMonFolder_Click);
            // 
            // bnRemoveMovieMonFolder
            // 
            this.bnRemoveMovieMonFolder.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.bnRemoveMovieMonFolder.Enabled = (false);
            this.bnRemoveMovieMonFolder.Location = (new global::System.Drawing.Point(102, 492));
            this.bnRemoveMovieMonFolder.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnRemoveMovieMonFolder.Name = ("bnRemoveMovieMonFolder");
            this.bnRemoveMovieMonFolder.Size = (new global::System.Drawing.Size(88, 27));
            this.bnRemoveMovieMonFolder.TabIndex = (51);
            this.bnRemoveMovieMonFolder.Text = ("&Remove");
            this.bnRemoveMovieMonFolder.UseVisualStyleBackColor = (true);
            this.bnRemoveMovieMonFolder.Click += (this.bnRemoveMovieMonFolder_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox6.Controls.Add(this.button4);
            this.groupBox6.Controls.Add(this.txtShowFolderFormat);
            this.groupBox6.Controls.Add(this.label96);
            this.groupBox6.Controls.Add(this.button1);
            this.groupBox6.Controls.Add(this.txtSeasonFormat);
            this.groupBox6.Controls.Add(this.txtSpecialsFolderName);
            this.groupBox6.Controls.Add(this.label47);
            this.groupBox6.Controls.Add(this.label13);
            this.groupBox6.Location = (new global::System.Drawing.Point(10, 232));
            this.groupBox6.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox6.Name = ("groupBox6");
            this.groupBox6.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox6.Size = (new global::System.Drawing.Size(456, 115));
            this.groupBox6.TabIndex = (48);
            this.groupBox6.TabStop = (false);
            this.groupBox6.Text = ("Default TV Show Library Folder Format");
            // 
            // button4
            // 
            this.button4.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = (new global::System.Drawing.Point(377, 83));
            this.button4.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.button4.Name = ("button4");
            this.button4.Size = (new global::System.Drawing.Size(72, 27));
            this.button4.TabIndex = (34);
            this.button4.Text = ("Tags...");
            this.button4.UseVisualStyleBackColor = (true);
            this.button4.Click += (this.button4_Click);
            // 
            // txtShowFolderFormat
            // 
            this.txtShowFolderFormat.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtShowFolderFormat.Location = (new global::System.Drawing.Point(145, 84));
            this.txtShowFolderFormat.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtShowFolderFormat.Name = ("txtShowFolderFormat");
            this.txtShowFolderFormat.Size = (new global::System.Drawing.Size(224, 23));
            this.txtShowFolderFormat.TabIndex = (33);
            // 
            // label96
            // 
            this.label96.AutoSize = (true);
            this.label96.Location = (new global::System.Drawing.Point(7, 89));
            this.label96.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label96.Name = ("label96");
            this.label96.Size = (new global::System.Drawing.Size(112, 15));
            this.label96.TabIndex = (32);
            this.label96.Text = ("Show folder format:");
            // 
            // button1
            // 
            this.button1.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = (new global::System.Drawing.Point(377, 53));
            this.button1.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.button1.Name = ("button1");
            this.button1.Size = (new global::System.Drawing.Size(72, 27));
            this.button1.TabIndex = (31);
            this.button1.Text = ("Tags...");
            this.button1.UseVisualStyleBackColor = (true);
            this.button1.Click += (this.button1_Click);
            // 
            // txtSeasonFormat
            // 
            this.txtSeasonFormat.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtSeasonFormat.Location = (new global::System.Drawing.Point(145, 54));
            this.txtSeasonFormat.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtSeasonFormat.Name = ("txtSeasonFormat");
            this.txtSeasonFormat.Size = (new global::System.Drawing.Size(224, 23));
            this.txtSeasonFormat.TabIndex = (30);
            // 
            // txtSpecialsFolderName
            // 
            this.txtSpecialsFolderName.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtSpecialsFolderName.Location = (new global::System.Drawing.Point(145, 24));
            this.txtSpecialsFolderName.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtSpecialsFolderName.Name = ("txtSpecialsFolderName");
            this.txtSpecialsFolderName.Size = (new global::System.Drawing.Size(304, 23));
            this.txtSpecialsFolderName.TabIndex = (28);
            // 
            // label47
            // 
            this.label47.AutoSize = (true);
            this.label47.Location = (new global::System.Drawing.Point(7, 59));
            this.label47.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label47.Name = ("label47");
            this.label47.Size = (new global::System.Drawing.Size(125, 15));
            this.label47.TabIndex = (29);
            this.label47.Text = ("&Seasons folder format:");
            // 
            // label13
            // 
            this.label13.AutoSize = (true);
            this.label13.Location = (new global::System.Drawing.Point(7, 28));
            this.label13.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label13.Name = ("label13");
            this.label13.Size = (new global::System.Drawing.Size(119, 15));
            this.label13.TabIndex = (27);
            this.label13.Text = ("&Specials folder name:");
            // 
            // label65
            // 
            this.label65.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.label65.AutoSize = (true);
            this.label65.Location = (new global::System.Drawing.Point(7, 7));
            this.label65.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label65.Name = ("label65");
            this.label65.Size = (new global::System.Drawing.Size(326, 30));
            this.label65.TabIndex = (44);
            this.label65.Text = ("TV Rename considers 2 sets of folders. Library Folders are the\r\nbase folders for a sorted collection of files");
            // 
            // label56
            // 
            this.label56.AutoSize = (true);
            this.label56.Location = (new global::System.Drawing.Point(4, 61));
            this.label56.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label56.Name = ("label56");
            this.label56.Size = (new global::System.Drawing.Size(100, 15));
            this.label56.TabIndex = (36);
            this.label56.Text = ("TV &Library Folders");
            // 
            // bnOpenMonFolder
            // 
            this.bnOpenMonFolder.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.bnOpenMonFolder.Enabled = (false);
            this.bnOpenMonFolder.Location = (new global::System.Drawing.Point(196, 198));
            this.bnOpenMonFolder.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnOpenMonFolder.Name = ("bnOpenMonFolder");
            this.bnOpenMonFolder.Size = (new global::System.Drawing.Size(88, 27));
            this.bnOpenMonFolder.TabIndex = (35);
            this.bnOpenMonFolder.Text = ("&Open");
            this.bnOpenMonFolder.UseVisualStyleBackColor = (true);
            this.bnOpenMonFolder.Click += (this.bnOpenMonFolder_Click);
            // 
            // bnAddMonFolder
            // 
            this.bnAddMonFolder.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.bnAddMonFolder.Location = (new global::System.Drawing.Point(7, 198));
            this.bnAddMonFolder.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnAddMonFolder.Name = ("bnAddMonFolder");
            this.bnAddMonFolder.Size = (new global::System.Drawing.Size(88, 27));
            this.bnAddMonFolder.TabIndex = (33);
            this.bnAddMonFolder.Text = ("&Add");
            this.bnAddMonFolder.UseVisualStyleBackColor = (true);
            this.bnAddMonFolder.Click += (this.bnAddMonFolder_Click);
            // 
            // bnRemoveMonFolder
            // 
            this.bnRemoveMonFolder.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.bnRemoveMonFolder.Enabled = (false);
            this.bnRemoveMonFolder.Location = (new global::System.Drawing.Point(102, 198));
            this.bnRemoveMonFolder.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnRemoveMonFolder.Name = ("bnRemoveMonFolder");
            this.bnRemoveMonFolder.Size = (new global::System.Drawing.Size(88, 27));
            this.bnRemoveMonFolder.TabIndex = (34);
            this.bnRemoveMonFolder.Text = ("&Remove");
            this.bnRemoveMonFolder.UseVisualStyleBackColor = (true);
            this.bnRemoveMonFolder.Click += (this.bnRemoveMonFolder_Click);
            // 
            // pbLibraryFolders
            // 
            this.pbLibraryFolders.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.pbLibraryFolders.Cursor = (global::System.Windows.Forms.Cursors.Hand);
            this.pbLibraryFolders.Image = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbLibraryFolders.InitialImage = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbLibraryFolders.Location = (new global::System.Drawing.Point(418, 7));
            this.pbLibraryFolders.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.pbLibraryFolders.Name = ("pbLibraryFolders");
            this.pbLibraryFolders.Size = (new global::System.Drawing.Size(50, 46));
            this.pbLibraryFolders.SizeMode = (global::System.Windows.Forms.PictureBoxSizeMode.CenterImage);
            this.pbLibraryFolders.TabIndex = (43);
            this.pbLibraryFolders.TabStop = (false);
            this.pbLibraryFolders.Click += (this.pbLibraryFolders_Click);
            // 
            // tpTorrentNZB
            // 
            this.tpTorrentNZB.Controls.Add(this.pbuTorrentNZB);
            this.tpTorrentNZB.Controls.Add(this.label58);
            this.tpTorrentNZB.Controls.Add(this.cbCheckqBitTorrent);
            this.tpTorrentNZB.Controls.Add(this.cbCheckSABnzbd);
            this.tpTorrentNZB.Controls.Add(this.cbCheckuTorrent);
            this.tpTorrentNZB.Controls.Add(this.qBitTorrent);
            this.tpTorrentNZB.Controls.Add(this.gbSAB);
            this.tpTorrentNZB.Controls.Add(this.gbuTorrent);
            this.tpTorrentNZB.Location = (new global::System.Drawing.Point(149, 4));
            this.tpTorrentNZB.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpTorrentNZB.Name = ("tpTorrentNZB");
            this.tpTorrentNZB.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpTorrentNZB.Size = (new global::System.Drawing.Size(500, 684));
            this.tpTorrentNZB.TabIndex = (4);
            this.tpTorrentNZB.Text = ("Torrents / NZB");
            this.tpTorrentNZB.UseVisualStyleBackColor = (true);
            // 
            // pbuTorrentNZB
            // 
            this.pbuTorrentNZB.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.pbuTorrentNZB.Cursor = (global::System.Windows.Forms.Cursors.Hand);
            this.pbuTorrentNZB.Image = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbuTorrentNZB.InitialImage = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbuTorrentNZB.Location = (new global::System.Drawing.Point(418, 7));
            this.pbuTorrentNZB.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.pbuTorrentNZB.Name = ("pbuTorrentNZB");
            this.pbuTorrentNZB.Size = (new global::System.Drawing.Size(50, 46));
            this.pbuTorrentNZB.SizeMode = (global::System.Windows.Forms.PictureBoxSizeMode.CenterImage);
            this.pbuTorrentNZB.TabIndex = (21);
            this.pbuTorrentNZB.TabStop = (false);
            this.pbuTorrentNZB.Click += (this.pictureBox1_Click);
            // 
            // label58
            // 
            this.label58.AutoSize = (true);
            this.label58.Location = (new global::System.Drawing.Point(4, 7));
            this.label58.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label58.Name = ("label58");
            this.label58.Size = (new global::System.Drawing.Size(369, 30));
            this.label58.TabIndex = (20);
            this.label58.Text = ("If an episode is missing from your library, TV Rename will look in the \r\nfollowing locations to see whether it is already being downloaded.");
            // 
            // cbCheckqBitTorrent
            // 
            this.cbCheckqBitTorrent.AutoSize = (true);
            this.cbCheckqBitTorrent.Location = (new global::System.Drawing.Point(7, 329));
            this.cbCheckqBitTorrent.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbCheckqBitTorrent.Name = ("cbCheckqBitTorrent");
            this.cbCheckqBitTorrent.Size = (new global::System.Drawing.Size(156, 19));
            this.cbCheckqBitTorrent.TabIndex = (19);
            this.cbCheckqBitTorrent.Text = ("Check &qBitTorrent queue");
            this.cbCheckqBitTorrent.UseVisualStyleBackColor = (true);
            this.cbCheckqBitTorrent.CheckedChanged += (this.EnableDisable);
            // 
            // cbCheckSABnzbd
            // 
            this.cbCheckSABnzbd.AutoSize = (true);
            this.cbCheckSABnzbd.Location = (new global::System.Drawing.Point(7, 76));
            this.cbCheckSABnzbd.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbCheckSABnzbd.Name = ("cbCheckSABnzbd");
            this.cbCheckSABnzbd.Size = (new global::System.Drawing.Size(145, 19));
            this.cbCheckSABnzbd.TabIndex = (18);
            this.cbCheckSABnzbd.Text = ("Check SA&Bnzbd queue");
            this.cbCheckSABnzbd.UseVisualStyleBackColor = (true);
            this.cbCheckSABnzbd.CheckedChanged += (this.EnableDisable);
            // 
            // cbCheckuTorrent
            // 
            this.cbCheckuTorrent.AutoSize = (true);
            this.cbCheckuTorrent.Location = (new global::System.Drawing.Point(7, 203));
            this.cbCheckuTorrent.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbCheckuTorrent.Name = ("cbCheckuTorrent");
            this.cbCheckuTorrent.Size = (new global::System.Drawing.Size(142, 19));
            this.cbCheckuTorrent.TabIndex = (17);
            this.cbCheckuTorrent.Text = ("C&heck µTorrent queue");
            this.cbCheckuTorrent.UseVisualStyleBackColor = (true);
            this.cbCheckuTorrent.CheckedChanged += (this.EnableDisable);
            // 
            // qBitTorrent
            // 
            this.qBitTorrent.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.qBitTorrent.Controls.Add(this.chkBitTorrentUseHTTPS);
            this.qBitTorrent.Controls.Add(this.chkRemoveCompletedTorrents);
            this.qBitTorrent.Controls.Add(this.llqBitTorrentLink);
            this.qBitTorrent.Controls.Add(this.label79);
            this.qBitTorrent.Controls.Add(this.rdoqBitTorrentAPIVersionv2);
            this.qBitTorrent.Controls.Add(this.rdoqBitTorrentAPIVersionv1);
            this.qBitTorrent.Controls.Add(this.rdoqBitTorrentAPIVersionv0);
            this.qBitTorrent.Controls.Add(this.label29);
            this.qBitTorrent.Controls.Add(this.cbDownloadTorrentBeforeDownloading);
            this.qBitTorrent.Controls.Add(this.tbqBitTorrentHost);
            this.qBitTorrent.Controls.Add(this.tbqBitTorrentPort);
            this.qBitTorrent.Controls.Add(this.label41);
            this.qBitTorrent.Controls.Add(this.label42);
            this.qBitTorrent.Location = (new global::System.Drawing.Point(7, 355));
            this.qBitTorrent.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.qBitTorrent.Name = ("qBitTorrent");
            this.qBitTorrent.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.qBitTorrent.Size = (new global::System.Drawing.Size(461, 203));
            this.qBitTorrent.TabIndex = (7);
            this.qBitTorrent.TabStop = (false);
            this.qBitTorrent.Text = ("qBitTorrent");
            // 
            // chkBitTorrentUseHTTPS
            // 
            this.chkBitTorrentUseHTTPS.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.chkBitTorrentUseHTTPS.AutoSize = (true);
            this.chkBitTorrentUseHTTPS.Location = (new global::System.Drawing.Point(372, 59));
            this.chkBitTorrentUseHTTPS.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkBitTorrentUseHTTPS.Name = ("chkBitTorrentUseHTTPS");
            this.chkBitTorrentUseHTTPS.Size = (new global::System.Drawing.Size(82, 19));
            this.chkBitTorrentUseHTTPS.TabIndex = (43);
            this.chkBitTorrentUseHTTPS.Text = ("Use HTTPS");
            this.chkBitTorrentUseHTTPS.UseVisualStyleBackColor = (true);
            this.chkBitTorrentUseHTTPS.CheckedChanged += (this.chkBitTorrentUseHTTPS_CheckedChanged);
            // 
            // chkRemoveCompletedTorrents
            // 
            this.chkRemoveCompletedTorrents.AutoSize = (true);
            this.chkRemoveCompletedTorrents.Location = (new global::System.Drawing.Point(88, 144));
            this.chkRemoveCompletedTorrents.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkRemoveCompletedTorrents.Name = ("chkRemoveCompletedTorrents");
            this.chkRemoveCompletedTorrents.Size = (new global::System.Drawing.Size(208, 19));
            this.chkRemoveCompletedTorrents.TabIndex = (22);
            this.chkRemoveCompletedTorrents.Text = ("Automatically Remove Completed");
            this.chkRemoveCompletedTorrents.UseVisualStyleBackColor = (true);
            // 
            // llqBitTorrentLink
            // 
            this.llqBitTorrentLink.AutoSize = (true);
            this.llqBitTorrentLink.Location = (new global::System.Drawing.Point(89, 168));
            this.llqBitTorrentLink.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.llqBitTorrentLink.Name = ("llqBitTorrentLink");
            this.llqBitTorrentLink.Size = (new global::System.Drawing.Size(93, 15));
            this.llqBitTorrentLink.TabIndex = (42);
            this.llqBitTorrentLink.TabStop = (true);
            this.llqBitTorrentLink.Text = ("llqBitTorrentLink");
            this.llqBitTorrentLink.LinkClicked += (this.LinkLabel1_LinkClicked);
            // 
            // label79
            // 
            this.label79.AutoSize = (true);
            this.label79.Location = (new global::System.Drawing.Point(9, 167));
            this.label79.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label79.Name = ("label79");
            this.label79.Size = (new global::System.Drawing.Size(21, 15));
            this.label79.TabIndex = (41);
            this.label79.Text = ("UI:");
            // 
            // rdoqBitTorrentAPIVersionv2
            // 
            this.rdoqBitTorrentAPIVersionv2.AutoSize = (true);
            this.rdoqBitTorrentAPIVersionv2.Location = (new global::System.Drawing.Point(262, 87));
            this.rdoqBitTorrentAPIVersionv2.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rdoqBitTorrentAPIVersionv2.Name = ("rdoqBitTorrentAPIVersionv2");
            this.rdoqBitTorrentAPIVersionv2.Size = (new global::System.Drawing.Size(54, 19));
            this.rdoqBitTorrentAPIVersionv2.TabIndex = (24);
            this.rdoqBitTorrentAPIVersionv2.TabStop = (true);
            this.rdoqBitTorrentAPIVersionv2.Text = ("v4.1+");
            this.rdoqBitTorrentAPIVersionv2.UseVisualStyleBackColor = (true);
            // 
            // rdoqBitTorrentAPIVersionv1
            // 
            this.rdoqBitTorrentAPIVersionv1.AutoSize = (true);
            this.rdoqBitTorrentAPIVersionv1.Location = (new global::System.Drawing.Point(160, 87));
            this.rdoqBitTorrentAPIVersionv1.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rdoqBitTorrentAPIVersionv1.Name = ("rdoqBitTorrentAPIVersionv1");
            this.rdoqBitTorrentAPIVersionv1.Size = (new global::System.Drawing.Size(84, 19));
            this.rdoqBitTorrentAPIVersionv1.TabIndex = (23);
            this.rdoqBitTorrentAPIVersionv1.TabStop = (true);
            this.rdoqBitTorrentAPIVersionv1.Text = ("v3.2 to v4.0");
            this.rdoqBitTorrentAPIVersionv1.UseVisualStyleBackColor = (true);
            // 
            // rdoqBitTorrentAPIVersionv0
            // 
            this.rdoqBitTorrentAPIVersionv0.AutoSize = (true);
            this.rdoqBitTorrentAPIVersionv0.Location = (new global::System.Drawing.Point(89, 87));
            this.rdoqBitTorrentAPIVersionv0.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rdoqBitTorrentAPIVersionv0.Name = ("rdoqBitTorrentAPIVersionv0");
            this.rdoqBitTorrentAPIVersionv0.Size = (new global::System.Drawing.Size(57, 19));
            this.rdoqBitTorrentAPIVersionv0.TabIndex = (22);
            this.rdoqBitTorrentAPIVersionv0.TabStop = (true);
            this.rdoqBitTorrentAPIVersionv0.Text = ("< v3.1");
            this.rdoqBitTorrentAPIVersionv0.UseVisualStyleBackColor = (true);
            // 
            // label29
            // 
            this.label29.AutoSize = (true);
            this.label29.Location = (new global::System.Drawing.Point(13, 89));
            this.label29.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label29.Name = ("label29");
            this.label29.Size = (new global::System.Drawing.Size(48, 15));
            this.label29.TabIndex = (21);
            this.label29.Text = ("Version:");
            // 
            // cbDownloadTorrentBeforeDownloading
            // 
            this.cbDownloadTorrentBeforeDownloading.AutoSize = (true);
            this.cbDownloadTorrentBeforeDownloading.Location = (new global::System.Drawing.Point(88, 118));
            this.cbDownloadTorrentBeforeDownloading.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDownloadTorrentBeforeDownloading.Name = ("cbDownloadTorrentBeforeDownloading");
            this.cbDownloadTorrentBeforeDownloading.Size = (new global::System.Drawing.Size(256, 19));
            this.cbDownloadTorrentBeforeDownloading.TabIndex = (20);
            this.cbDownloadTorrentBeforeDownloading.Text = ("Download .torrent files before downloading");
            this.cbDownloadTorrentBeforeDownloading.UseVisualStyleBackColor = (true);
            // 
            // tbqBitTorrentHost
            // 
            this.tbqBitTorrentHost.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.tbqBitTorrentHost.Location = (new global::System.Drawing.Point(88, 22));
            this.tbqBitTorrentHost.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbqBitTorrentHost.Name = ("tbqBitTorrentHost");
            this.tbqBitTorrentHost.Size = (new global::System.Drawing.Size(366, 23));
            this.tbqBitTorrentHost.TabIndex = (1);
            this.tbqBitTorrentHost.TextChanged += (this.QBitDetailsChanged);
            // 
            // tbqBitTorrentPort
            // 
            this.tbqBitTorrentPort.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.tbqBitTorrentPort.Location = (new global::System.Drawing.Point(88, 55));
            this.tbqBitTorrentPort.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbqBitTorrentPort.Name = ("tbqBitTorrentPort");
            this.tbqBitTorrentPort.Size = (new global::System.Drawing.Size(252, 23));
            this.tbqBitTorrentPort.TabIndex = (4);
            this.tbqBitTorrentPort.TextChanged += (this.QBitDetailsChanged);
            // 
            // label41
            // 
            this.label41.AutoSize = (true);
            this.label41.Location = (new global::System.Drawing.Point(13, 59));
            this.label41.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label41.Name = ("label41");
            this.label41.Size = (new global::System.Drawing.Size(32, 15));
            this.label41.TabIndex = (3);
            this.label41.Text = ("Port:");
            // 
            // label42
            // 
            this.label42.AutoSize = (true);
            this.label42.Location = (new global::System.Drawing.Point(13, 25));
            this.label42.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label42.Name = ("label42");
            this.label42.Size = (new global::System.Drawing.Size(35, 15));
            this.label42.TabIndex = (0);
            this.label42.Text = ("Host:");
            // 
            // gbSAB
            // 
            this.gbSAB.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.gbSAB.Controls.Add(this.txtSABHostPort);
            this.gbSAB.Controls.Add(this.txtSABAPIKey);
            this.gbSAB.Controls.Add(this.label8);
            this.gbSAB.Controls.Add(this.label9);
            this.gbSAB.Location = (new global::System.Drawing.Point(7, 103));
            this.gbSAB.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.gbSAB.Name = ("gbSAB");
            this.gbSAB.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.gbSAB.Size = (new global::System.Drawing.Size(461, 93));
            this.gbSAB.TabIndex = (6);
            this.gbSAB.TabStop = (false);
            this.gbSAB.Text = ("SABnzbd");
            // 
            // txtSABHostPort
            // 
            this.txtSABHostPort.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtSABHostPort.Location = (new global::System.Drawing.Point(88, 22));
            this.txtSABHostPort.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtSABHostPort.Name = ("txtSABHostPort");
            this.txtSABHostPort.Size = (new global::System.Drawing.Size(366, 23));
            this.txtSABHostPort.TabIndex = (1);
            // 
            // txtSABAPIKey
            // 
            this.txtSABAPIKey.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtSABAPIKey.Location = (new global::System.Drawing.Point(88, 55));
            this.txtSABAPIKey.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtSABAPIKey.Name = ("txtSABAPIKey");
            this.txtSABAPIKey.Size = (new global::System.Drawing.Size(366, 23));
            this.txtSABAPIKey.TabIndex = (4);
            // 
            // label8
            // 
            this.label8.AutoSize = (true);
            this.label8.Location = (new global::System.Drawing.Point(13, 59));
            this.label8.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label8.Name = ("label8");
            this.label8.Size = (new global::System.Drawing.Size(50, 15));
            this.label8.TabIndex = (3);
            this.label8.Text = ("API Key:");
            // 
            // label9
            // 
            this.label9.AutoSize = (true);
            this.label9.Location = (new global::System.Drawing.Point(13, 25));
            this.label9.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label9.Name = ("label9");
            this.label9.Size = (new global::System.Drawing.Size(57, 15));
            this.label9.TabIndex = (0);
            this.label9.Text = ("Host:Port");
            // 
            // gbuTorrent
            // 
            this.gbuTorrent.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.gbuTorrent.Controls.Add(this.bnUTBrowseResumeDat);
            this.gbuTorrent.Controls.Add(this.txtUTResumeDatPath);
            this.gbuTorrent.Controls.Add(this.bnRSSBrowseuTorrent);
            this.gbuTorrent.Controls.Add(this.label27);
            this.gbuTorrent.Controls.Add(this.label26);
            this.gbuTorrent.Controls.Add(this.txtRSSuTorrentPath);
            this.gbuTorrent.Location = (new global::System.Drawing.Point(7, 230));
            this.gbuTorrent.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.gbuTorrent.Name = ("gbuTorrent");
            this.gbuTorrent.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.gbuTorrent.Size = (new global::System.Drawing.Size(461, 92));
            this.gbuTorrent.TabIndex = (5);
            this.gbuTorrent.TabStop = (false);
            this.gbuTorrent.Text = ("µTorrent");
            // 
            // bnUTBrowseResumeDat
            // 
            this.bnUTBrowseResumeDat.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnUTBrowseResumeDat.Location = (new global::System.Drawing.Point(366, 53));
            this.bnUTBrowseResumeDat.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnUTBrowseResumeDat.Name = ("bnUTBrowseResumeDat");
            this.bnUTBrowseResumeDat.Size = (new global::System.Drawing.Size(88, 27));
            this.bnUTBrowseResumeDat.TabIndex = (5);
            this.bnUTBrowseResumeDat.Text = ("Bro&wse...");
            this.bnUTBrowseResumeDat.UseVisualStyleBackColor = (true);
            this.bnUTBrowseResumeDat.Click += (this.bnUTBrowseResumeDat_Click);
            // 
            // txtUTResumeDatPath
            // 
            this.txtUTResumeDatPath.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtUTResumeDatPath.Location = (new global::System.Drawing.Point(88, 55));
            this.txtUTResumeDatPath.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtUTResumeDatPath.Name = ("txtUTResumeDatPath");
            this.txtUTResumeDatPath.Size = (new global::System.Drawing.Size(271, 23));
            this.txtUTResumeDatPath.TabIndex = (4);
            // 
            // bnRSSBrowseuTorrent
            // 
            this.bnRSSBrowseuTorrent.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnRSSBrowseuTorrent.Location = (new global::System.Drawing.Point(366, 18));
            this.bnRSSBrowseuTorrent.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnRSSBrowseuTorrent.Name = ("bnRSSBrowseuTorrent");
            this.bnRSSBrowseuTorrent.Size = (new global::System.Drawing.Size(88, 27));
            this.bnRSSBrowseuTorrent.TabIndex = (2);
            this.bnRSSBrowseuTorrent.Text = ("&Browse...");
            this.bnRSSBrowseuTorrent.UseVisualStyleBackColor = (true);
            this.bnRSSBrowseuTorrent.Click += (this.bnRSSBrowseuTorrent_Click);
            // 
            // label27
            // 
            this.label27.AutoSize = (true);
            this.label27.Location = (new global::System.Drawing.Point(8, 25));
            this.label27.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label27.Name = ("label27");
            this.label27.Size = (new global::System.Drawing.Size(71, 15));
            this.label27.TabIndex = (0);
            this.label27.Text = ("A&pplication:");
            // 
            // label26
            // 
            this.label26.AutoSize = (true);
            this.label26.Location = (new global::System.Drawing.Point(8, 59));
            this.label26.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label26.Name = ("label26");
            this.label26.Size = (new global::System.Drawing.Size(69, 15));
            this.label26.TabIndex = (3);
            this.label26.Text = ("resume.&dat:");
            // 
            // txtRSSuTorrentPath
            // 
            this.txtRSSuTorrentPath.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtRSSuTorrentPath.Location = (new global::System.Drawing.Point(88, 22));
            this.txtRSSuTorrentPath.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtRSSuTorrentPath.Name = ("txtRSSuTorrentPath");
            this.txtRSSuTorrentPath.Size = (new global::System.Drawing.Size(271, 23));
            this.txtRSSuTorrentPath.TabIndex = (1);
            // 
            // tbSearchFolders
            // 
            this.tbSearchFolders.Controls.Add(this.label98);
            this.tbSearchFolders.Controls.Add(this.upDownScanSeconds);
            this.tbSearchFolders.Controls.Add(this.chkUseSearchFullPathWhenMatchingShows);
            this.tbSearchFolders.Controls.Add(this.cbCopyFutureDatedEps);
            this.tbSearchFolders.Controls.Add(this.groupBox8);
            this.tbSearchFolders.Controls.Add(this.label67);
            this.tbSearchFolders.Controls.Add(this.gbAutoAdd);
            this.tbSearchFolders.Controls.Add(this.cbLeaveOriginals);
            this.tbSearchFolders.Controls.Add(this.cbSearchLocally);
            this.tbSearchFolders.Controls.Add(this.chkAutoMergeDownloadEpisodes);
            this.tbSearchFolders.Controls.Add(this.cbMonitorFolder);
            this.tbSearchFolders.Controls.Add(this.bnOpenSearchFolder);
            this.tbSearchFolders.Controls.Add(this.bnRemoveSearchFolder);
            this.tbSearchFolders.Controls.Add(this.bnAddSearchFolder);
            this.tbSearchFolders.Controls.Add(this.pbSearchFolders);
            this.tbSearchFolders.Controls.Add(this.lbSearchFolders);
            this.tbSearchFolders.Controls.Add(this.label23);
            this.tbSearchFolders.Location = (new global::System.Drawing.Point(149, 4));
            this.tbSearchFolders.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbSearchFolders.Name = ("tbSearchFolders");
            this.tbSearchFolders.Size = (new global::System.Drawing.Size(500, 684));
            this.tbSearchFolders.TabIndex = (3);
            this.tbSearchFolders.Text = ("Search Folders");
            this.tbSearchFolders.UseVisualStyleBackColor = (true);
            // 
            // chkUseSearchFullPathWhenMatchingShows
            // 
            this.chkUseSearchFullPathWhenMatchingShows.AutoSize = (true);
            this.chkUseSearchFullPathWhenMatchingShows.Location = (new global::System.Drawing.Point(7, 197));
            this.chkUseSearchFullPathWhenMatchingShows.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkUseSearchFullPathWhenMatchingShows.Name = ("chkUseSearchFullPathWhenMatchingShows");
            this.chkUseSearchFullPathWhenMatchingShows.Size = (new global::System.Drawing.Size(451, 19));
            this.chkUseSearchFullPathWhenMatchingShows.TabIndex = (42);
            this.chkUseSearchFullPathWhenMatchingShows.Text = ("Use name of Search Folder when searching for a match between a file and media");
            this.chkUseSearchFullPathWhenMatchingShows.UseVisualStyleBackColor = (true);
            // 
            // groupBox8
            // 
            this.groupBox8.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox8.Controls.Add(this.cbMovieHigherQuality);
            this.groupBox8.Controls.Add(this.label53);
            this.groupBox8.Controls.Add(this.label54);
            this.groupBox8.Controls.Add(this.tbPercentBetter);
            this.groupBox8.Controls.Add(this.tbPriorityOverrideTerms);
            this.groupBox8.Controls.Add(this.label52);
            this.groupBox8.Controls.Add(this.cbHigherQuality);
            this.groupBox8.Location = (new global::System.Drawing.Point(7, 510));
            this.groupBox8.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox8.Name = ("groupBox8");
            this.groupBox8.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox8.Size = (new global::System.Drawing.Size(462, 133));
            this.groupBox8.TabIndex = (40);
            this.groupBox8.TabStop = (false);
            this.groupBox8.Text = ("Upgrade media when better quality files are found");
            // 
            // cbMovieHigherQuality
            // 
            this.cbMovieHigherQuality.AutoSize = (true);
            this.cbMovieHigherQuality.Location = (new global::System.Drawing.Point(7, 42));
            this.cbMovieHigherQuality.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbMovieHigherQuality.Name = ("cbMovieHigherQuality");
            this.cbMovieHigherQuality.Size = (new global::System.Drawing.Size(370, 19));
            this.cbMovieHigherQuality.TabIndex = (39);
            this.cbMovieHigherQuality.Text = ("Update movies when higher-quality ones found in Search Folders");
            this.cbMovieHigherQuality.UseVisualStyleBackColor = (true);
            // 
            // label53
            // 
            this.label53.AutoSize = (true);
            this.label53.Location = (new global::System.Drawing.Point(8, 103));
            this.label53.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label53.Name = ("label53");
            this.label53.Size = (new global::System.Drawing.Size(147, 15));
            this.label53.TabIndex = (36);
            this.label53.Text = ("Consider a file better if it is");
            // 
            // label54
            // 
            this.label54.AutoSize = (true);
            this.label54.Location = (new global::System.Drawing.Point(202, 102));
            this.label54.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label54.Name = ("label54");
            this.label54.Size = (new global::System.Drawing.Size(149, 15));
            this.label54.TabIndex = (38);
            this.label54.Text = ("% higher resolution/longer");
            // 
            // tbPercentBetter
            // 
            this.tbPercentBetter.Location = (new global::System.Drawing.Point(164, 98));
            this.tbPercentBetter.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbPercentBetter.Name = ("tbPercentBetter");
            this.tbPercentBetter.Size = (new global::System.Drawing.Size(32, 23));
            this.tbPercentBetter.TabIndex = (37);
            this.tbPercentBetter.TextChanged += (this.EnsureInteger);
            this.tbPercentBetter.KeyPress += (this.TxtNumberOnlyKeyPress);
            // 
            // tbPriorityOverrideTerms
            // 
            this.tbPriorityOverrideTerms.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.tbPriorityOverrideTerms.Location = (new global::System.Drawing.Point(164, 68));
            this.tbPriorityOverrideTerms.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbPriorityOverrideTerms.Name = ("tbPriorityOverrideTerms");
            this.tbPriorityOverrideTerms.Size = (new global::System.Drawing.Size(294, 23));
            this.tbPriorityOverrideTerms.TabIndex = (35);
            // 
            // label52
            // 
            this.label52.AutoSize = (true);
            this.label52.Location = (new global::System.Drawing.Point(9, 72));
            this.label52.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label52.Name = ("label52");
            this.label52.Size = (new global::System.Drawing.Size(127, 15));
            this.label52.TabIndex = (34);
            this.label52.Text = ("Priority override terms:");
            // 
            // cbHigherQuality
            // 
            this.cbHigherQuality.AutoSize = (true);
            this.cbHigherQuality.Location = (new global::System.Drawing.Point(7, 22));
            this.cbHigherQuality.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbHigherQuality.Name = ("cbHigherQuality");
            this.cbHigherQuality.Size = (new global::System.Drawing.Size(378, 19));
            this.cbHigherQuality.TabIndex = (33);
            this.cbHigherQuality.Text = ("Update episodes when higher-quality ones found in Search Folders");
            this.cbHigherQuality.UseVisualStyleBackColor = (true);
            // 
            // label67
            // 
            this.label67.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.label67.AutoEllipsis = (true);
            this.label67.AutoSize = (true);
            this.label67.Location = (new global::System.Drawing.Point(7, 7));
            this.label67.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label67.Name = ("label67");
            this.label67.Size = (new global::System.Drawing.Size(370, 45));
            this.label67.TabIndex = (39);
            this.label67.Text = (resources.GetString("label67.Text"));
            // 
            // gbAutoAdd
            // 
            this.gbAutoAdd.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.gbAutoAdd.Controls.Add(this.cbAutomateAutoAddWhenOneMovieFound);
            this.gbAutoAdd.Controls.Add(this.cbAutomateAutoAddWhenOneShowFound);
            this.gbAutoAdd.Controls.Add(this.chkAutoSearchForDownloadedFiles);
            this.gbAutoAdd.Controls.Add(this.label43);
            this.gbAutoAdd.Controls.Add(this.label44);
            this.gbAutoAdd.Controls.Add(this.tbIgnoreSuffixes);
            this.gbAutoAdd.Controls.Add(this.tbMovieTerms);
            this.gbAutoAdd.Location = (new global::System.Drawing.Point(7, 359));
            this.gbAutoAdd.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.gbAutoAdd.Name = ("gbAutoAdd");
            this.gbAutoAdd.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.gbAutoAdd.Size = (new global::System.Drawing.Size(463, 144));
            this.gbAutoAdd.TabIndex = (36);
            this.gbAutoAdd.TabStop = (false);
            this.gbAutoAdd.Text = ("Auto Add Movies & TV Shows from Search Folders");
            // 
            // cbAutomateAutoAddWhenOneMovieFound
            // 
            this.cbAutomateAutoAddWhenOneMovieFound.AutoSize = (true);
            this.cbAutomateAutoAddWhenOneMovieFound.Location = (new global::System.Drawing.Point(23, 67));
            this.cbAutomateAutoAddWhenOneMovieFound.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbAutomateAutoAddWhenOneMovieFound.Name = ("cbAutomateAutoAddWhenOneMovieFound");
            this.cbAutomateAutoAddWhenOneMovieFound.Size = (new global::System.Drawing.Size(229, 19));
            this.cbAutomateAutoAddWhenOneMovieFound.TabIndex = (18);
            this.cbAutomateAutoAddWhenOneMovieFound.Text = ("Auto Add when only one movie found");
            this.cbAutomateAutoAddWhenOneMovieFound.UseVisualStyleBackColor = (true);
            // 
            // cbAutomateAutoAddWhenOneShowFound
            // 
            this.cbAutomateAutoAddWhenOneShowFound.AutoSize = (true);
            this.cbAutomateAutoAddWhenOneShowFound.Location = (new global::System.Drawing.Point(23, 45));
            this.cbAutomateAutoAddWhenOneShowFound.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbAutomateAutoAddWhenOneShowFound.Name = ("cbAutomateAutoAddWhenOneShowFound");
            this.cbAutomateAutoAddWhenOneShowFound.Size = (new global::System.Drawing.Size(237, 19));
            this.cbAutomateAutoAddWhenOneShowFound.TabIndex = (17);
            this.cbAutomateAutoAddWhenOneShowFound.Text = ("Auto Add when only one tv show found");
            this.cbAutomateAutoAddWhenOneShowFound.UseVisualStyleBackColor = (true);
            // 
            // chkAutoSearchForDownloadedFiles
            // 
            this.chkAutoSearchForDownloadedFiles.AutoSize = (true);
            this.chkAutoSearchForDownloadedFiles.Location = (new global::System.Drawing.Point(7, 22));
            this.chkAutoSearchForDownloadedFiles.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkAutoSearchForDownloadedFiles.Name = ("chkAutoSearchForDownloadedFiles");
            this.chkAutoSearchForDownloadedFiles.Size = (new global::System.Drawing.Size(198, 19));
            this.chkAutoSearchForDownloadedFiles.TabIndex = (16);
            this.chkAutoSearchForDownloadedFiles.Text = ("Notify when new media is found");
            this.chkAutoSearchForDownloadedFiles.UseVisualStyleBackColor = (true);
            // 
            // label43
            // 
            this.label43.AutoSize = (true);
            this.label43.Location = (new global::System.Drawing.Point(5, 120));
            this.label43.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label43.Name = ("label43");
            this.label43.Size = (new global::System.Drawing.Size(87, 15));
            this.label43.TabIndex = (14);
            this.label43.Text = ("&Ignore suffixes:");
            // 
            // label44
            // 
            this.label44.AutoSize = (true);
            this.label44.Location = (new global::System.Drawing.Point(5, 90));
            this.label44.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label44.Name = ("label44");
            this.label44.Size = (new global::System.Drawing.Size(77, 15));
            this.label44.TabIndex = (12);
            this.label44.Text = ("&Movie Terms:");
            // 
            // cbLeaveOriginals
            // 
            this.cbLeaveOriginals.AutoSize = (true);
            this.cbLeaveOriginals.Location = (new global::System.Drawing.Point(20, 93));
            this.cbLeaveOriginals.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbLeaveOriginals.Name = ("cbLeaveOriginals");
            this.cbLeaveOriginals.Size = (new global::System.Drawing.Size(145, 19));
            this.cbLeaveOriginals.TabIndex = (35);
            this.cbLeaveOriginals.Text = ("&Copy files, don't move");
            this.cbLeaveOriginals.UseVisualStyleBackColor = (true);
            // 
            // cbSearchLocally
            // 
            this.cbSearchLocally.AutoSize = (true);
            this.cbSearchLocally.Checked = (true);
            this.cbSearchLocally.CheckState = (global::System.Windows.Forms.CheckState.Checked);
            this.cbSearchLocally.Location = (new global::System.Drawing.Point(7, 67));
            this.cbSearchLocally.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbSearchLocally.Name = ("cbSearchLocally");
            this.cbSearchLocally.Size = (new global::System.Drawing.Size(240, 19));
            this.cbSearchLocally.TabIndex = (34);
            this.cbSearchLocally.Text = ("&Look in \"Search Folders\" for missing files");
            this.cbSearchLocally.UseVisualStyleBackColor = (true);
            this.cbSearchLocally.CheckedChanged += (this.EnableDisable);
            // 
            // chkAutoMergeDownloadEpisodes
            // 
            this.chkAutoMergeDownloadEpisodes.AutoSize = (true);
            this.chkAutoMergeDownloadEpisodes.Location = (new global::System.Drawing.Point(7, 120));
            this.chkAutoMergeDownloadEpisodes.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkAutoMergeDownloadEpisodes.Name = ("chkAutoMergeDownloadEpisodes");
            this.chkAutoMergeDownloadEpisodes.Size = (new global::System.Drawing.Size(367, 19));
            this.chkAutoMergeDownloadEpisodes.TabIndex = (32);
            this.chkAutoMergeDownloadEpisodes.Text = ("Automatically create merge rules based on files in Search Folders");
            this.chkAutoMergeDownloadEpisodes.UseVisualStyleBackColor = (true);
            // 
            // bnOpenSearchFolder
            // 
            this.bnOpenSearchFolder.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.bnOpenSearchFolder.Enabled = (false);
            this.bnOpenSearchFolder.Location = (new global::System.Drawing.Point(196, 325));
            this.bnOpenSearchFolder.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnOpenSearchFolder.Name = ("bnOpenSearchFolder");
            this.bnOpenSearchFolder.Size = (new global::System.Drawing.Size(88, 27));
            this.bnOpenSearchFolder.TabIndex = (4);
            this.bnOpenSearchFolder.Text = ("&Open");
            this.bnOpenSearchFolder.UseVisualStyleBackColor = (true);
            this.bnOpenSearchFolder.Click += (this.bnOpenSearchFolder_Click);
            // 
            // bnRemoveSearchFolder
            // 
            this.bnRemoveSearchFolder.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.bnRemoveSearchFolder.Enabled = (false);
            this.bnRemoveSearchFolder.Location = (new global::System.Drawing.Point(102, 325));
            this.bnRemoveSearchFolder.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnRemoveSearchFolder.Name = ("bnRemoveSearchFolder");
            this.bnRemoveSearchFolder.Size = (new global::System.Drawing.Size(88, 27));
            this.bnRemoveSearchFolder.TabIndex = (3);
            this.bnRemoveSearchFolder.Text = ("&Remove");
            this.bnRemoveSearchFolder.UseVisualStyleBackColor = (true);
            this.bnRemoveSearchFolder.Click += (this.bnRemoveSearchFolder_Click);
            // 
            // bnAddSearchFolder
            // 
            this.bnAddSearchFolder.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.bnAddSearchFolder.Location = (new global::System.Drawing.Point(7, 325));
            this.bnAddSearchFolder.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnAddSearchFolder.Name = ("bnAddSearchFolder");
            this.bnAddSearchFolder.Size = (new global::System.Drawing.Size(88, 27));
            this.bnAddSearchFolder.TabIndex = (2);
            this.bnAddSearchFolder.Text = ("&Add");
            this.bnAddSearchFolder.UseVisualStyleBackColor = (true);
            this.bnAddSearchFolder.Click += (this.bnAddSearchFolder_Click);
            // 
            // pbSearchFolders
            // 
            this.pbSearchFolders.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.pbSearchFolders.Cursor = (global::System.Windows.Forms.Cursors.Hand);
            this.pbSearchFolders.Image = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbSearchFolders.InitialImage = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbSearchFolders.Location = (new global::System.Drawing.Point(418, 7));
            this.pbSearchFolders.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.pbSearchFolders.Name = ("pbSearchFolders");
            this.pbSearchFolders.Size = (new global::System.Drawing.Size(50, 46));
            this.pbSearchFolders.SizeMode = (global::System.Windows.Forms.PictureBoxSizeMode.CenterImage);
            this.pbSearchFolders.TabIndex = (37);
            this.pbSearchFolders.TabStop = (false);
            this.pbSearchFolders.Click += (this.pbSearchFolders_Click);
            // 
            // label23
            // 
            this.label23.AutoSize = (true);
            this.label23.Location = (new global::System.Drawing.Point(7, 220));
            this.label23.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label23.Name = ("label23");
            this.label23.Size = (new global::System.Drawing.Size(83, 15));
            this.label23.TabIndex = (0);
            this.label23.Text = ("&Search Folders");
            // 
            // tbMediaCenter
            // 
            this.tbMediaCenter.Controls.Add(this.groupBox16);
            this.tbMediaCenter.Controls.Add(this.groupBox13);
            this.tbMediaCenter.Controls.Add(this.groupBox14);
            this.tbMediaCenter.Controls.Add(this.groupBox15);
            this.tbMediaCenter.Controls.Add(this.groupBox12);
            this.tbMediaCenter.Controls.Add(this.label64);
            this.tbMediaCenter.Controls.Add(this.bnMCPresets);
            this.tbMediaCenter.Controls.Add(this.pbMediaCenter);
            this.tbMediaCenter.Location = (new global::System.Drawing.Point(149, 4));
            this.tbMediaCenter.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbMediaCenter.Name = ("tbMediaCenter");
            this.tbMediaCenter.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbMediaCenter.Size = (new global::System.Drawing.Size(500, 684));
            this.tbMediaCenter.TabIndex = (8);
            this.tbMediaCenter.Text = ("Media Centres");
            this.tbMediaCenter.UseVisualStyleBackColor = (true);
            // 
            // groupBox16
            // 
            this.groupBox16.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox16.Controls.Add(this.cbWDLiveEpisodeFiles);
            this.groupBox16.Location = (new global::System.Drawing.Point(10, 410));
            this.groupBox16.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox16.Name = ("groupBox16");
            this.groupBox16.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox16.Size = (new global::System.Drawing.Size(457, 58));
            this.groupBox16.TabIndex = (41);
            this.groupBox16.TabStop = (false);
            this.groupBox16.Text = ("WD TV Live Hub");
            // 
            // cbWDLiveEpisodeFiles
            // 
            this.cbWDLiveEpisodeFiles.AutoSize = (true);
            this.cbWDLiveEpisodeFiles.Location = (new global::System.Drawing.Point(7, 27));
            this.cbWDLiveEpisodeFiles.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbWDLiveEpisodeFiles.Name = ("cbWDLiveEpisodeFiles");
            this.cbWDLiveEpisodeFiles.Size = (new global::System.Drawing.Size(215, 19));
            this.cbWDLiveEpisodeFiles.TabIndex = (25);
            this.cbWDLiveEpisodeFiles.Text = ("WD TV Live Hub Episode Files (.xml)");
            this.cbWDLiveEpisodeFiles.UseVisualStyleBackColor = (true);
            // 
            // groupBox13
            // 
            this.groupBox13.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox13.Controls.Add(this.cbXMLFiles);
            this.groupBox13.Controls.Add(this.cbSeriesJpg);
            this.groupBox13.Controls.Add(this.cbShrinkLarge);
            this.groupBox13.Location = (new global::System.Drawing.Point(10, 287));
            this.groupBox13.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox13.Name = ("groupBox13");
            this.groupBox13.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox13.Size = (new global::System.Drawing.Size(457, 115));
            this.groupBox13.TabIndex = (40);
            this.groupBox13.TabStop = (false);
            this.groupBox13.Text = ("Mede8er");
            // 
            // cbXMLFiles
            // 
            this.cbXMLFiles.AutoSize = (true);
            this.cbXMLFiles.Location = (new global::System.Drawing.Point(7, 48));
            this.cbXMLFiles.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbXMLFiles.Name = ("cbXMLFiles");
            this.cbXMLFiles.Size = (new global::System.Drawing.Size(200, 19));
            this.cbXMLFiles.TabIndex = (8);
            this.cbXMLFiles.Text = ("&XML files for shows and episodes");
            this.cbXMLFiles.UseVisualStyleBackColor = (true);
            // 
            // cbSeriesJpg
            // 
            this.cbSeriesJpg.AutoSize = (true);
            this.cbSeriesJpg.Location = (new global::System.Drawing.Point(7, 22));
            this.cbSeriesJpg.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbSeriesJpg.Name = ("cbSeriesJpg");
            this.cbSeriesJpg.Size = (new global::System.Drawing.Size(266, 19));
            this.cbSeriesJpg.TabIndex = (7);
            this.cbSeriesJpg.Text = ("&Create cachedSeries poster (cachedSeries.jpg)");
            this.cbSeriesJpg.UseVisualStyleBackColor = (true);
            // 
            // cbShrinkLarge
            // 
            this.cbShrinkLarge.AutoSize = (true);
            this.cbShrinkLarge.Location = (new global::System.Drawing.Point(7, 75));
            this.cbShrinkLarge.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbShrinkLarge.Name = ("cbShrinkLarge");
            this.cbShrinkLarge.Size = (new global::System.Drawing.Size(365, 19));
            this.cbShrinkLarge.TabIndex = (9);
            this.cbShrinkLarge.Text = ("S&hrink large cachedSeries and episode images to 156 x 232 pixels");
            this.cbShrinkLarge.UseVisualStyleBackColor = (true);
            // 
            // groupBox14
            // 
            this.groupBox14.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox14.Controls.Add(this.cbMeta);
            this.groupBox14.Controls.Add(this.cbMetaSubfolder);
            this.groupBox14.Location = (new global::System.Drawing.Point(10, 205));
            this.groupBox14.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox14.Name = ("groupBox14");
            this.groupBox14.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox14.Size = (new global::System.Drawing.Size(457, 75));
            this.groupBox14.TabIndex = (40);
            this.groupBox14.TabStop = (false);
            this.groupBox14.Text = ("pyTivo");
            // 
            // cbMeta
            // 
            this.cbMeta.AutoSize = (true);
            this.cbMeta.Location = (new global::System.Drawing.Point(7, 22));
            this.cbMeta.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbMeta.Name = ("cbMeta");
            this.cbMeta.Size = (new global::System.Drawing.Size(172, 19));
            this.cbMeta.TabIndex = (4);
            this.cbMeta.Text = ("&Meta files for episodes (.txt)");
            this.cbMeta.UseVisualStyleBackColor = (true);
            this.cbMeta.CheckedChanged += (this.EnableDisable);
            // 
            // cbMetaSubfolder
            // 
            this.cbMetaSubfolder.AutoSize = (true);
            this.cbMetaSubfolder.Location = (new global::System.Drawing.Point(7, 48));
            this.cbMetaSubfolder.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbMetaSubfolder.Name = ("cbMetaSubfolder");
            this.cbMetaSubfolder.Size = (new global::System.Drawing.Size(207, 19));
            this.cbMetaSubfolder.TabIndex = (5);
            this.cbMetaSubfolder.Text = ("Pl&ace Meta files in .meta subfolder");
            this.cbMetaSubfolder.UseVisualStyleBackColor = (true);
            // 
            // groupBox15
            // 
            this.groupBox15.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox15.Controls.Add(this.cbNFOMovies);
            this.groupBox15.Controls.Add(this.cbEpTBNs);
            this.groupBox15.Controls.Add(this.cbNFOShows);
            this.groupBox15.Controls.Add(this.cbKODIImages);
            this.groupBox15.Controls.Add(this.cbNFOEpisodes);
            this.groupBox15.Location = (new global::System.Drawing.Point(10, 83));
            this.groupBox15.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox15.Name = ("groupBox15");
            this.groupBox15.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox15.Size = (new global::System.Drawing.Size(457, 115));
            this.groupBox15.TabIndex = (40);
            this.groupBox15.TabStop = (false);
            this.groupBox15.Text = ("Kodi");
            // 
            // cbNFOMovies
            // 
            this.cbNFOMovies.AutoSize = (true);
            this.cbNFOMovies.Location = (new global::System.Drawing.Point(7, 67));
            this.cbNFOMovies.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbNFOMovies.Name = ("cbNFOMovies");
            this.cbNFOMovies.Size = (new global::System.Drawing.Size(133, 19));
            this.cbNFOMovies.TabIndex = (25);
            this.cbNFOMovies.Text = ("&NFO files for movies");
            this.cbNFOMovies.UseVisualStyleBackColor = (true);
            // 
            // cbEpTBNs
            // 
            this.cbEpTBNs.AutoSize = (true);
            this.cbEpTBNs.Location = (new global::System.Drawing.Point(7, 22));
            this.cbEpTBNs.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbEpTBNs.Name = ("cbEpTBNs");
            this.cbEpTBNs.Size = (new global::System.Drawing.Size(204, 19));
            this.cbEpTBNs.TabIndex = (1);
            this.cbEpTBNs.Text = ("&Episode Thumbnails (-thumb.jpg)");
            this.cbEpTBNs.UseVisualStyleBackColor = (true);
            // 
            // cbNFOShows
            // 
            this.cbNFOShows.AutoSize = (true);
            this.cbNFOShows.Location = (new global::System.Drawing.Point(7, 46));
            this.cbNFOShows.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbNFOShows.Name = ("cbNFOShows");
            this.cbNFOShows.Size = (new global::System.Drawing.Size(128, 19));
            this.cbNFOShows.TabIndex = (2);
            this.cbNFOShows.Text = ("&NFO files for shows");
            this.cbNFOShows.UseVisualStyleBackColor = (true);
            // 
            // cbKODIImages
            // 
            this.cbKODIImages.AutoSize = (true);
            this.cbKODIImages.Location = (new global::System.Drawing.Point(7, 90));
            this.cbKODIImages.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbKODIImages.Name = ("cbKODIImages");
            this.cbKODIImages.Size = (new global::System.Drawing.Size(265, 19));
            this.cbKODIImages.TabIndex = (17);
            this.cbKODIImages.Text = ("Download &Images (fanart, poster, banner.jpg)");
            this.cbKODIImages.UseVisualStyleBackColor = (true);
            // 
            // cbNFOEpisodes
            // 
            this.cbNFOEpisodes.AutoSize = (true);
            this.cbNFOEpisodes.Location = (new global::System.Drawing.Point(176, 46));
            this.cbNFOEpisodes.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbNFOEpisodes.Name = ("cbNFOEpisodes");
            this.cbNFOEpisodes.Size = (new global::System.Drawing.Size(141, 19));
            this.cbNFOEpisodes.TabIndex = (24);
            this.cbNFOEpisodes.Text = ("&NFO files for episodes");
            this.cbNFOEpisodes.UseVisualStyleBackColor = (true);
            // 
            // groupBox12
            // 
            this.groupBox12.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox12.Controls.Add(this.cbFantArtJpg);
            this.groupBox12.Controls.Add(this.cbFolderJpg);
            this.groupBox12.Controls.Add(this.cbEpThumbJpg);
            this.groupBox12.Controls.Add(this.panel1);
            this.groupBox12.Location = (new global::System.Drawing.Point(10, 474));
            this.groupBox12.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox12.Name = ("groupBox12");
            this.groupBox12.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox12.Size = (new global::System.Drawing.Size(457, 129));
            this.groupBox12.TabIndex = (39);
            this.groupBox12.TabStop = (false);
            this.groupBox12.Text = ("General");
            // 
            // cbFantArtJpg
            // 
            this.cbFantArtJpg.AutoSize = (true);
            this.cbFantArtJpg.Location = (new global::System.Drawing.Point(10, 75));
            this.cbFantArtJpg.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbFantArtJpg.Name = ("cbFantArtJpg");
            this.cbFantArtJpg.Size = (new global::System.Drawing.Size(157, 19));
            this.cbFantArtJpg.TabIndex = (15);
            this.cbFantArtJpg.Text = ("Fanar&t Image (fanart.jpg)");
            this.cbFantArtJpg.UseVisualStyleBackColor = (true);
            // 
            // cbFolderJpg
            // 
            this.cbFolderJpg.AutoSize = (true);
            this.cbFolderJpg.Location = (new global::System.Drawing.Point(10, 25));
            this.cbFolderJpg.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbFolderJpg.Name = ("cbFolderJpg");
            this.cbFolderJpg.Size = (new global::System.Drawing.Size(157, 19));
            this.cbFolderJpg.TabIndex = (11);
            this.cbFolderJpg.Text = ("&Folder image (folder.jpg)");
            this.cbFolderJpg.UseVisualStyleBackColor = (true);
            // 
            // cbEpThumbJpg
            // 
            this.cbEpThumbJpg.AutoSize = (true);
            this.cbEpThumbJpg.Location = (new global::System.Drawing.Point(10, 102));
            this.cbEpThumbJpg.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbEpThumbJpg.Name = ("cbEpThumbJpg");
            this.cbEpThumbJpg.Size = (new global::System.Drawing.Size(163, 19));
            this.cbEpThumbJpg.TabIndex = (16);
            this.cbEpThumbJpg.Text = ("Episode Thumbnails (.&jpg)");
            this.cbEpThumbJpg.UseVisualStyleBackColor = (true);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbFolderBanner);
            this.panel1.Controls.Add(this.rbFolderPoster);
            this.panel1.Controls.Add(this.rbFolderFanArt);
            this.panel1.Controls.Add(this.rbFolderSeasonPoster);
            this.panel1.Location = (new global::System.Drawing.Point(35, 40));
            this.panel1.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.panel1.Name = ("panel1");
            this.panel1.Size = (new global::System.Drawing.Size(327, 28));
            this.panel1.TabIndex = (22);
            // 
            // rbFolderBanner
            // 
            this.rbFolderBanner.AutoSize = (true);
            this.rbFolderBanner.Location = (new global::System.Drawing.Point(0, 3));
            this.rbFolderBanner.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rbFolderBanner.Name = ("rbFolderBanner");
            this.rbFolderBanner.Size = (new global::System.Drawing.Size(62, 19));
            this.rbFolderBanner.TabIndex = (12);
            this.rbFolderBanner.TabStop = (true);
            this.rbFolderBanner.Text = ("&Banner");
            this.rbFolderBanner.UseVisualStyleBackColor = (true);
            // 
            // rbFolderPoster
            // 
            this.rbFolderPoster.AutoSize = (true);
            this.rbFolderPoster.Location = (new global::System.Drawing.Point(70, 3));
            this.rbFolderPoster.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rbFolderPoster.Name = ("rbFolderPoster");
            this.rbFolderPoster.Size = (new global::System.Drawing.Size(58, 19));
            this.rbFolderPoster.TabIndex = (13);
            this.rbFolderPoster.TabStop = (true);
            this.rbFolderPoster.Text = ("&Poster");
            this.rbFolderPoster.UseVisualStyleBackColor = (true);
            // 
            // rbFolderFanArt
            // 
            this.rbFolderFanArt.AutoSize = (true);
            this.rbFolderFanArt.Location = (new global::System.Drawing.Point(141, 3));
            this.rbFolderFanArt.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rbFolderFanArt.Name = ("rbFolderFanArt");
            this.rbFolderFanArt.Size = (new global::System.Drawing.Size(63, 19));
            this.rbFolderFanArt.TabIndex = (14);
            this.rbFolderFanArt.TabStop = (true);
            this.rbFolderFanArt.Text = ("Fan A&rt");
            this.rbFolderFanArt.UseVisualStyleBackColor = (true);
            // 
            // rbFolderSeasonPoster
            // 
            this.rbFolderSeasonPoster.AutoSize = (true);
            this.rbFolderSeasonPoster.Location = (new global::System.Drawing.Point(217, 3));
            this.rbFolderSeasonPoster.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rbFolderSeasonPoster.Name = ("rbFolderSeasonPoster");
            this.rbFolderSeasonPoster.Size = (new global::System.Drawing.Size(98, 19));
            this.rbFolderSeasonPoster.TabIndex = (16);
            this.rbFolderSeasonPoster.TabStop = (true);
            this.rbFolderSeasonPoster.Text = ("Seaso&n Poster");
            this.rbFolderSeasonPoster.UseVisualStyleBackColor = (true);
            // 
            // label64
            // 
            this.label64.AutoSize = (true);
            this.label64.Location = (new global::System.Drawing.Point(7, 8));
            this.label64.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label64.Name = ("label64");
            this.label64.Size = (new global::System.Drawing.Size(335, 45));
            this.label64.TabIndex = (38);
            this.label64.Text = ("While scanning your library folders TV Rename can create and\r\ndownload additional files to help video playing applications to\r\nunderstand what is in your library and display it in a nicer way.");
            // 
            // bnMCPresets
            // 
            this.bnMCPresets.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnMCPresets.Location = (new global::System.Drawing.Point(380, 615));
            this.bnMCPresets.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnMCPresets.Name = ("bnMCPresets");
            this.bnMCPresets.Size = (new global::System.Drawing.Size(88, 27));
            this.bnMCPresets.TabIndex = (16);
            this.bnMCPresets.Text = ("Pre&sets...");
            this.bnMCPresets.UseVisualStyleBackColor = (true);
            this.bnMCPresets.Click += (this.bnMCPresets_Click);
            // 
            // pbMediaCenter
            // 
            this.pbMediaCenter.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.pbMediaCenter.Cursor = (global::System.Windows.Forms.Cursors.Hand);
            this.pbMediaCenter.Image = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbMediaCenter.InitialImage = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbMediaCenter.Location = (new global::System.Drawing.Point(418, 7));
            this.pbMediaCenter.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.pbMediaCenter.Name = ("pbMediaCenter");
            this.pbMediaCenter.Size = (new global::System.Drawing.Size(50, 46));
            this.pbMediaCenter.SizeMode = (global::System.Windows.Forms.PictureBoxSizeMode.CenterImage);
            this.pbMediaCenter.TabIndex = (27);
            this.pbMediaCenter.TabStop = (false);
            this.pbMediaCenter.Click += (this.pictureBox7_Click);
            // 
            // tbFolderDeleting
            // 
            this.tbFolderDeleting.Controls.Add(this.cbDeleteMovieFromDisk);
            this.tbFolderDeleting.Controls.Add(this.groupBox28);
            this.tbFolderDeleting.Controls.Add(this.label69);
            this.tbFolderDeleting.Controls.Add(this.cbDeleteShowFromDisk);
            this.tbFolderDeleting.Controls.Add(this.label32);
            this.tbFolderDeleting.Controls.Add(this.label30);
            this.tbFolderDeleting.Controls.Add(this.txtEmptyMaxSize);
            this.tbFolderDeleting.Controls.Add(this.txtEmptyIgnoreWords);
            this.tbFolderDeleting.Controls.Add(this.txtEmptyIgnoreExtensions);
            this.tbFolderDeleting.Controls.Add(this.label31);
            this.tbFolderDeleting.Controls.Add(this.cbRecycleNotDelete);
            this.tbFolderDeleting.Controls.Add(this.cbEmptyMaxSize);
            this.tbFolderDeleting.Controls.Add(this.cbEmptyIgnoreWords);
            this.tbFolderDeleting.Controls.Add(this.cbEmptyIgnoreExtensions);
            this.tbFolderDeleting.Controls.Add(this.cbDeleteEmpty);
            this.tbFolderDeleting.Controls.Add(this.pbFolderDeleting);
            this.tbFolderDeleting.Location = (new global::System.Drawing.Point(149, 4));
            this.tbFolderDeleting.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbFolderDeleting.Name = ("tbFolderDeleting");
            this.tbFolderDeleting.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbFolderDeleting.Size = (new global::System.Drawing.Size(500, 684));
            this.tbFolderDeleting.TabIndex = (9);
            this.tbFolderDeleting.Text = ("Folder Deleting");
            this.tbFolderDeleting.UseVisualStyleBackColor = (true);
            // 
            // cbDeleteMovieFromDisk
            // 
            this.cbDeleteMovieFromDisk.AutoSize = (true);
            this.cbDeleteMovieFromDisk.Location = (new global::System.Drawing.Point(19, 428));
            this.cbDeleteMovieFromDisk.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDeleteMovieFromDisk.Name = ("cbDeleteMovieFromDisk");
            this.cbDeleteMovieFromDisk.Size = (new global::System.Drawing.Size(340, 19));
            this.cbDeleteMovieFromDisk.TabIndex = (42);
            this.cbDeleteMovieFromDisk.Text = ("Ask to delete from disk when deleting movie from database");
            this.cbDeleteMovieFromDisk.UseVisualStyleBackColor = (true);
            // 
            // groupBox28
            // 
            this.groupBox28.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox28.Controls.Add(this.tbCleanUpDownloadDirMoviesLength);
            this.groupBox28.Controls.Add(this.cbCleanUpDownloadDirMoviesLength);
            this.groupBox28.Controls.Add(this.cbCleanUpDownloadDirMovies);
            this.groupBox28.Controls.Add(this.cbCleanUpDownloadDir);
            this.groupBox28.Location = (new global::System.Drawing.Point(19, 279));
            this.groupBox28.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox28.Name = ("groupBox28");
            this.groupBox28.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox28.Size = (new global::System.Drawing.Size(449, 115));
            this.groupBox28.TabIndex = (41);
            this.groupBox28.TabStop = (false);
            this.groupBox28.Text = ("Clean Up Search Folders");
            // 
            // cbCleanUpDownloadDirMoviesLength
            // 
            this.cbCleanUpDownloadDirMoviesLength.AutoSize = (true);
            this.cbCleanUpDownloadDirMoviesLength.Location = (new global::System.Drawing.Point(52, 75));
            this.cbCleanUpDownloadDirMoviesLength.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbCleanUpDownloadDirMoviesLength.Name = ("cbCleanUpDownloadDirMoviesLength");
            this.cbCleanUpDownloadDirMoviesLength.Size = (new global::System.Drawing.Size(198, 19));
            this.cbCleanUpDownloadDirMoviesLength.TabIndex = (13);
            this.cbCleanUpDownloadDirMoviesLength.Text = ("Only include movies longer than");
            this.cbCleanUpDownloadDirMoviesLength.UseVisualStyleBackColor = (true);
            // 
            // cbCleanUpDownloadDirMovies
            // 
            this.cbCleanUpDownloadDirMovies.AutoSize = (true);
            this.cbCleanUpDownloadDirMovies.Location = (new global::System.Drawing.Point(7, 48));
            this.cbCleanUpDownloadDirMovies.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbCleanUpDownloadDirMovies.Name = ("cbCleanUpDownloadDirMovies");
            this.cbCleanUpDownloadDirMovies.Size = (new global::System.Drawing.Size(318, 19));
            this.cbCleanUpDownloadDirMovies.TabIndex = (12);
            this.cbCleanUpDownloadDirMovies.Text = ("Clean up already copied movie files from search folders");
            this.cbCleanUpDownloadDirMovies.UseVisualStyleBackColor = (true);
            // 
            // cbCleanUpDownloadDir
            // 
            this.cbCleanUpDownloadDir.AutoSize = (true);
            this.cbCleanUpDownloadDir.Location = (new global::System.Drawing.Point(7, 22));
            this.cbCleanUpDownloadDir.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbCleanUpDownloadDir.Name = ("cbCleanUpDownloadDir");
            this.cbCleanUpDownloadDir.Size = (new global::System.Drawing.Size(326, 19));
            this.cbCleanUpDownloadDir.TabIndex = (11);
            this.cbCleanUpDownloadDir.Text = ("Clean up already copied episode files from search folders");
            this.cbCleanUpDownloadDir.UseVisualStyleBackColor = (true);
            // 
            // label69
            // 
            this.label69.AutoSize = (true);
            this.label69.Location = (new global::System.Drawing.Point(7, 7));
            this.label69.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label69.Name = ("label69");
            this.label69.Size = (new global::System.Drawing.Size(305, 30));
            this.label69.TabIndex = (40);
            this.label69.Text = ("TV Rename can clean up the search folders to keep them\r\nclear of unused files and duplicate downloads.");
            // 
            // cbDeleteShowFromDisk
            // 
            this.cbDeleteShowFromDisk.AutoSize = (true);
            this.cbDeleteShowFromDisk.Location = (new global::System.Drawing.Point(19, 402));
            this.cbDeleteShowFromDisk.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDeleteShowFromDisk.Name = ("cbDeleteShowFromDisk");
            this.cbDeleteShowFromDisk.Size = (new global::System.Drawing.Size(348, 19));
            this.cbDeleteShowFromDisk.TabIndex = (13);
            this.cbDeleteShowFromDisk.Text = ("Ask to delete from disk when deleting tv show from database");
            this.cbDeleteShowFromDisk.UseVisualStyleBackColor = (true);
            // 
            // label32
            // 
            this.label32.AutoSize = (true);
            this.label32.Location = (new global::System.Drawing.Point(15, 194));
            this.label32.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label32.Name = ("label32");
            this.label32.Size = (new global::System.Drawing.Size(0, 15));
            this.label32.TabIndex = (6);
            // 
            // label30
            // 
            this.label30.AutoSize = (true);
            this.label30.Location = (new global::System.Drawing.Point(15, 50));
            this.label30.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label30.Name = ("label30");
            this.label30.Size = (new global::System.Drawing.Size(0, 15));
            this.label30.TabIndex = (1);
            // 
            // txtEmptyMaxSize
            // 
            this.txtEmptyMaxSize.Location = (new global::System.Drawing.Point(254, 215));
            this.txtEmptyMaxSize.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtEmptyMaxSize.Name = ("txtEmptyMaxSize");
            this.txtEmptyMaxSize.Size = (new global::System.Drawing.Size(63, 23));
            this.txtEmptyMaxSize.TabIndex = (8);
            // 
            // label31
            // 
            this.label31.AutoSize = (true);
            this.label31.Location = (new global::System.Drawing.Point(328, 223));
            this.label31.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label31.Name = ("label31");
            this.label31.Size = (new global::System.Drawing.Size(0, 15));
            this.label31.TabIndex = (9);
            // 
            // cbRecycleNotDelete
            // 
            this.cbRecycleNotDelete.AutoSize = (true);
            this.cbRecycleNotDelete.Location = (new global::System.Drawing.Point(19, 253));
            this.cbRecycleNotDelete.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbRecycleNotDelete.Name = ("cbRecycleNotDelete");
            this.cbRecycleNotDelete.Size = (new global::System.Drawing.Size(333, 19));
            this.cbRecycleNotDelete.TabIndex = (10);
            this.cbRecycleNotDelete.Text = ("Folders with files are moved to the &recycle bin, not deleted");
            this.cbRecycleNotDelete.UseVisualStyleBackColor = (true);
            // 
            // cbEmptyMaxSize
            // 
            this.cbEmptyMaxSize.AutoSize = (true);
            this.cbEmptyMaxSize.Location = (new global::System.Drawing.Point(41, 217));
            this.cbEmptyMaxSize.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbEmptyMaxSize.Name = ("cbEmptyMaxSize");
            this.cbEmptyMaxSize.Size = (new global::System.Drawing.Size(201, 19));
            this.cbEmptyMaxSize.TabIndex = (7);
            this.cbEmptyMaxSize.Text = ("&Maximum total file size to delete:");
            this.cbEmptyMaxSize.UseVisualStyleBackColor = (true);
            // 
            // cbEmptyIgnoreWords
            // 
            this.cbEmptyIgnoreWords.AutoSize = (true);
            this.cbEmptyIgnoreWords.Location = (new global::System.Drawing.Point(41, 102));
            this.cbEmptyIgnoreWords.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbEmptyIgnoreWords.Name = ("cbEmptyIgnoreWords");
            this.cbEmptyIgnoreWords.Size = (new global::System.Drawing.Size(412, 19));
            this.cbEmptyIgnoreWords.TabIndex = (2);
            this.cbEmptyIgnoreWords.Text = ("Ignore any files with these &words in their name: (semicolon separated list)");
            this.cbEmptyIgnoreWords.UseVisualStyleBackColor = (true);
            // 
            // cbEmptyIgnoreExtensions
            // 
            this.cbEmptyIgnoreExtensions.AutoSize = (true);
            this.cbEmptyIgnoreExtensions.Location = (new global::System.Drawing.Point(41, 159));
            this.cbEmptyIgnoreExtensions.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbEmptyIgnoreExtensions.Name = ("cbEmptyIgnoreExtensions");
            this.cbEmptyIgnoreExtensions.Size = (new global::System.Drawing.Size(341, 19));
            this.cbEmptyIgnoreExtensions.TabIndex = (4);
            this.cbEmptyIgnoreExtensions.Text = ("&Ignore files with these extensions: (semicolon separated list)");
            this.cbEmptyIgnoreExtensions.UseVisualStyleBackColor = (true);
            // 
            // cbDeleteEmpty
            // 
            this.cbDeleteEmpty.AutoSize = (true);
            this.cbDeleteEmpty.Location = (new global::System.Drawing.Point(19, 75));
            this.cbDeleteEmpty.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDeleteEmpty.Name = ("cbDeleteEmpty");
            this.cbDeleteEmpty.Size = (new global::System.Drawing.Size(346, 19));
            this.cbDeleteEmpty.TabIndex = (0);
            this.cbDeleteEmpty.Text = ("&Delete empty folders after moving files (from Search Folders)");
            this.cbDeleteEmpty.UseVisualStyleBackColor = (true);
            // 
            // pbFolderDeleting
            // 
            this.pbFolderDeleting.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.pbFolderDeleting.Cursor = (global::System.Windows.Forms.Cursors.Hand);
            this.pbFolderDeleting.Image = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbFolderDeleting.InitialImage = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbFolderDeleting.Location = (new global::System.Drawing.Point(418, 7));
            this.pbFolderDeleting.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.pbFolderDeleting.Name = ("pbFolderDeleting");
            this.pbFolderDeleting.Size = (new global::System.Drawing.Size(50, 46));
            this.pbFolderDeleting.SizeMode = (global::System.Windows.Forms.PictureBoxSizeMode.CenterImage);
            this.pbFolderDeleting.TabIndex = (24);
            this.pbFolderDeleting.TabStop = (false);
            this.pbFolderDeleting.Click += (this.pbFolderDeleting_Click);
            // 
            // tbAutoExport
            // 
            this.tbAutoExport.Controls.Add(this.pbuExportEpisodes);
            this.tbAutoExport.Controls.Add(this.label88);
            this.tbAutoExport.Controls.Add(this.groupBox10);
            this.tbAutoExport.Controls.Add(this.groupBox5);
            this.tbAutoExport.Controls.Add(this.groupBox4);
            this.tbAutoExport.Controls.Add(this.groupBox2);
            this.tbAutoExport.Location = (new global::System.Drawing.Point(149, 4));
            this.tbAutoExport.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbAutoExport.Name = ("tbAutoExport");
            this.tbAutoExport.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbAutoExport.Size = (new global::System.Drawing.Size(500, 684));
            this.tbAutoExport.TabIndex = (2);
            this.tbAutoExport.Text = ("Episode Export");
            this.tbAutoExport.UseVisualStyleBackColor = (true);
            // 
            // pbuExportEpisodes
            // 
            this.pbuExportEpisodes.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.pbuExportEpisodes.Cursor = (global::System.Windows.Forms.Cursors.Hand);
            this.pbuExportEpisodes.Image = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbuExportEpisodes.InitialImage = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbuExportEpisodes.Location = (new global::System.Drawing.Point(416, 9));
            this.pbuExportEpisodes.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.pbuExportEpisodes.Name = ("pbuExportEpisodes");
            this.pbuExportEpisodes.Size = (new global::System.Drawing.Size(50, 46));
            this.pbuExportEpisodes.SizeMode = (global::System.Windows.Forms.PictureBoxSizeMode.CenterImage);
            this.pbuExportEpisodes.TabIndex = (44);
            this.pbuExportEpisodes.TabStop = (false);
            this.pbuExportEpisodes.Click += (this.pbuExportEpisodes_Click);
            // 
            // label88
            // 
            this.label88.AutoSize = (true);
            this.label88.Location = (new global::System.Drawing.Point(4, 3));
            this.label88.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label88.Name = ("label88");
            this.label88.Size = (new global::System.Drawing.Size(294, 45));
            this.label88.TabIndex = (43);
            this.label88.Text = ("TV Rename can export information about episodes\r\nin various formats. Some focus on upcoming episodes\r\nand others are based on recently aired.");
            // 
            // groupBox10
            // 
            this.groupBox10.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox10.Controls.Add(this.bnBrowseWPL);
            this.groupBox10.Controls.Add(this.txtWPL);
            this.groupBox10.Controls.Add(this.cbWPL);
            this.groupBox10.Controls.Add(this.bnBrowseASX);
            this.groupBox10.Controls.Add(this.txtASX);
            this.groupBox10.Controls.Add(this.cbASX);
            this.groupBox10.Controls.Add(this.bnBrowseM3U);
            this.groupBox10.Controls.Add(this.txtM3U);
            this.groupBox10.Controls.Add(this.cbM3U);
            this.groupBox10.Controls.Add(this.bnBrowseXSPF);
            this.groupBox10.Controls.Add(this.txtXSPF);
            this.groupBox10.Controls.Add(this.cbXSPF);
            this.groupBox10.Location = (new global::System.Drawing.Point(8, 396));
            this.groupBox10.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox10.Name = ("groupBox10");
            this.groupBox10.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox10.Size = (new global::System.Drawing.Size(461, 156));
            this.groupBox10.TabIndex = (5);
            this.groupBox10.TabStop = (false);
            this.groupBox10.Text = ("Recent Playlist");
            // 
            // bnBrowseWPL
            // 
            this.bnBrowseWPL.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseWPL.Location = (new global::System.Drawing.Point(364, 120));
            this.bnBrowseWPL.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnBrowseWPL.Name = ("bnBrowseWPL");
            this.bnBrowseWPL.Size = (new global::System.Drawing.Size(88, 27));
            this.bnBrowseWPL.TabIndex = (27);
            this.bnBrowseWPL.Text = ("Browse...");
            this.bnBrowseWPL.UseVisualStyleBackColor = (true);
            this.bnBrowseWPL.Click += (this.bnBrowseWPL_Click);
            // 
            // txtWPL
            // 
            this.txtWPL.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtWPL.Location = (new global::System.Drawing.Point(76, 122));
            this.txtWPL.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtWPL.Name = ("txtWPL");
            this.txtWPL.Size = (new global::System.Drawing.Size(279, 23));
            this.txtWPL.TabIndex = (26);
            // 
            // cbWPL
            // 
            this.cbWPL.AutoSize = (true);
            this.cbWPL.Location = (new global::System.Drawing.Point(9, 125));
            this.cbWPL.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbWPL.Name = ("cbWPL");
            this.cbWPL.Size = (new global::System.Drawing.Size(50, 19));
            this.cbWPL.TabIndex = (25);
            this.cbWPL.Text = ("WPL");
            this.cbWPL.UseVisualStyleBackColor = (true);
            this.cbWPL.CheckedChanged += (this.EnableDisable);
            // 
            // bnBrowseASX
            // 
            this.bnBrowseASX.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseASX.Location = (new global::System.Drawing.Point(364, 88));
            this.bnBrowseASX.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnBrowseASX.Name = ("bnBrowseASX");
            this.bnBrowseASX.Size = (new global::System.Drawing.Size(88, 27));
            this.bnBrowseASX.TabIndex = (24);
            this.bnBrowseASX.Text = ("Browse...");
            this.bnBrowseASX.UseVisualStyleBackColor = (true);
            this.bnBrowseASX.Click += (this.bnBrowseASX_Click);
            // 
            // txtASX
            // 
            this.txtASX.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtASX.Location = (new global::System.Drawing.Point(76, 90));
            this.txtASX.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtASX.Name = ("txtASX");
            this.txtASX.Size = (new global::System.Drawing.Size(279, 23));
            this.txtASX.TabIndex = (23);
            // 
            // cbASX
            // 
            this.cbASX.AutoSize = (true);
            this.cbASX.Location = (new global::System.Drawing.Point(9, 92));
            this.cbASX.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbASX.Name = ("cbASX");
            this.cbASX.Size = (new global::System.Drawing.Size(47, 19));
            this.cbASX.TabIndex = (22);
            this.cbASX.Text = ("ASX");
            this.cbASX.UseVisualStyleBackColor = (true);
            this.cbASX.CheckedChanged += (this.EnableDisable);
            // 
            // bnBrowseM3U
            // 
            this.bnBrowseM3U.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseM3U.Location = (new global::System.Drawing.Point(363, 53));
            this.bnBrowseM3U.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnBrowseM3U.Name = ("bnBrowseM3U");
            this.bnBrowseM3U.Size = (new global::System.Drawing.Size(88, 27));
            this.bnBrowseM3U.TabIndex = (19);
            this.bnBrowseM3U.Text = ("Browse...");
            this.bnBrowseM3U.UseVisualStyleBackColor = (true);
            this.bnBrowseM3U.Click += (this.bnBrowseM3U_Click);
            // 
            // txtM3U
            // 
            this.txtM3U.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtM3U.Location = (new global::System.Drawing.Point(76, 57));
            this.txtM3U.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtM3U.Name = ("txtM3U");
            this.txtM3U.Size = (new global::System.Drawing.Size(279, 23));
            this.txtM3U.TabIndex = (18);
            // 
            // cbM3U
            // 
            this.cbM3U.AutoSize = (true);
            this.cbM3U.Location = (new global::System.Drawing.Point(9, 59));
            this.cbM3U.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbM3U.Name = ("cbM3U");
            this.cbM3U.Size = (new global::System.Drawing.Size(62, 19));
            this.cbM3U.TabIndex = (17);
            this.cbM3U.Text = ("M3U/8");
            this.cbM3U.UseVisualStyleBackColor = (true);
            this.cbM3U.CheckedChanged += (this.EnableDisable);
            // 
            // bnBrowseXSPF
            // 
            this.bnBrowseXSPF.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseXSPF.Location = (new global::System.Drawing.Point(364, 21));
            this.bnBrowseXSPF.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnBrowseXSPF.Name = ("bnBrowseXSPF");
            this.bnBrowseXSPF.Size = (new global::System.Drawing.Size(88, 27));
            this.bnBrowseXSPF.TabIndex = (2);
            this.bnBrowseXSPF.Text = ("Browse...");
            this.bnBrowseXSPF.UseVisualStyleBackColor = (true);
            this.bnBrowseXSPF.Click += (this.bnBrowseXSPF_Click);
            // 
            // txtXSPF
            // 
            this.txtXSPF.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtXSPF.Location = (new global::System.Drawing.Point(75, 23));
            this.txtXSPF.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtXSPF.Name = ("txtXSPF");
            this.txtXSPF.Size = (new global::System.Drawing.Size(282, 23));
            this.txtXSPF.TabIndex = (1);
            // 
            // cbXSPF
            // 
            this.cbXSPF.AutoSize = (true);
            this.cbXSPF.Location = (new global::System.Drawing.Point(9, 25));
            this.cbXSPF.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbXSPF.Name = ("cbXSPF");
            this.cbXSPF.Size = (new global::System.Drawing.Size(52, 19));
            this.cbXSPF.TabIndex = (0);
            this.cbXSPF.Text = ("XSPF");
            this.cbXSPF.UseVisualStyleBackColor = (true);
            this.cbXSPF.CheckedChanged += (this.EnableDisable);
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.bnBrowseFOXML);
            this.groupBox5.Controls.Add(this.cbFOXML);
            this.groupBox5.Controls.Add(this.txtFOXML);
            this.groupBox5.Location = (new global::System.Drawing.Point(10, 325));
            this.groupBox5.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox5.Name = ("groupBox5");
            this.groupBox5.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox5.Size = (new global::System.Drawing.Size(461, 63));
            this.groupBox5.TabIndex = (3);
            this.groupBox5.TabStop = (false);
            this.groupBox5.Text = ("Finding and Organising");
            // 
            // bnBrowseFOXML
            // 
            this.bnBrowseFOXML.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseFOXML.Location = (new global::System.Drawing.Point(363, 22));
            this.bnBrowseFOXML.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnBrowseFOXML.Name = ("bnBrowseFOXML");
            this.bnBrowseFOXML.Size = (new global::System.Drawing.Size(88, 27));
            this.bnBrowseFOXML.TabIndex = (2);
            this.bnBrowseFOXML.Text = ("Browse...");
            this.bnBrowseFOXML.UseVisualStyleBackColor = (true);
            this.bnBrowseFOXML.Click += (this.bnBrowseFOXML_Click);
            // 
            // cbFOXML
            // 
            this.cbFOXML.AutoSize = (true);
            this.cbFOXML.Location = (new global::System.Drawing.Point(9, 27));
            this.cbFOXML.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbFOXML.Name = ("cbFOXML");
            this.cbFOXML.Size = (new global::System.Drawing.Size(50, 19));
            this.cbFOXML.TabIndex = (0);
            this.cbFOXML.Text = ("XML");
            this.cbFOXML.UseVisualStyleBackColor = (true);
            this.cbFOXML.CheckedChanged += (this.EnableDisable);
            // 
            // txtFOXML
            // 
            this.txtFOXML.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtFOXML.Location = (new global::System.Drawing.Point(75, 24));
            this.txtFOXML.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtFOXML.Name = ("txtFOXML");
            this.txtFOXML.Size = (new global::System.Drawing.Size(280, 23));
            this.txtFOXML.TabIndex = (1);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.bnBrowseRenamingXML);
            this.groupBox4.Controls.Add(this.cbRenamingXML);
            this.groupBox4.Controls.Add(this.txtRenamingXML);
            this.groupBox4.Location = (new global::System.Drawing.Point(10, 253));
            this.groupBox4.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox4.Name = ("groupBox4");
            this.groupBox4.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox4.Size = (new global::System.Drawing.Size(461, 66));
            this.groupBox4.TabIndex = (2);
            this.groupBox4.TabStop = (false);
            this.groupBox4.Text = ("Renaming");
            // 
            // bnBrowseRenamingXML
            // 
            this.bnBrowseRenamingXML.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseRenamingXML.Location = (new global::System.Drawing.Point(363, 22));
            this.bnBrowseRenamingXML.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnBrowseRenamingXML.Name = ("bnBrowseRenamingXML");
            this.bnBrowseRenamingXML.Size = (new global::System.Drawing.Size(88, 27));
            this.bnBrowseRenamingXML.TabIndex = (2);
            this.bnBrowseRenamingXML.Text = ("Browse...");
            this.bnBrowseRenamingXML.UseVisualStyleBackColor = (true);
            this.bnBrowseRenamingXML.Click += (this.bnBrowseRenamingXML_Click);
            // 
            // cbRenamingXML
            // 
            this.cbRenamingXML.AutoSize = (true);
            this.cbRenamingXML.Location = (new global::System.Drawing.Point(9, 27));
            this.cbRenamingXML.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbRenamingXML.Name = ("cbRenamingXML");
            this.cbRenamingXML.Size = (new global::System.Drawing.Size(50, 19));
            this.cbRenamingXML.TabIndex = (0);
            this.cbRenamingXML.Text = ("XML");
            this.cbRenamingXML.UseVisualStyleBackColor = (true);
            this.cbRenamingXML.CheckedChanged += (this.EnableDisable);
            // 
            // txtRenamingXML
            // 
            this.txtRenamingXML.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtRenamingXML.Location = (new global::System.Drawing.Point(75, 24));
            this.txtRenamingXML.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtRenamingXML.Name = ("txtRenamingXML");
            this.txtRenamingXML.Size = (new global::System.Drawing.Size(280, 23));
            this.txtRenamingXML.TabIndex = (1);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.bnBrowseWTWTXT);
            this.groupBox2.Controls.Add(this.txtWTWTXT);
            this.groupBox2.Controls.Add(this.cbWTWTXT);
            this.groupBox2.Controls.Add(this.bnBrowseWTWICAL);
            this.groupBox2.Controls.Add(this.txtWTWICAL);
            this.groupBox2.Controls.Add(this.cbWTWICAL);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtExportRSSDaysPast);
            this.groupBox2.Controls.Add(this.bnBrowseWTWXML);
            this.groupBox2.Controls.Add(this.txtWTWXML);
            this.groupBox2.Controls.Add(this.cbWTWXML);
            this.groupBox2.Controls.Add(this.bnBrowseWTWRSS);
            this.groupBox2.Controls.Add(this.txtWTWRSS);
            this.groupBox2.Controls.Add(this.cbWTWRSS);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.txtExportRSSMaxDays);
            this.groupBox2.Controls.Add(this.txtExportRSSMaxShows);
            this.groupBox2.Location = (new global::System.Drawing.Point(10, 62));
            this.groupBox2.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox2.Name = ("groupBox2");
            this.groupBox2.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox2.Size = (new global::System.Drawing.Size(461, 183));
            this.groupBox2.TabIndex = (0);
            this.groupBox2.TabStop = (false);
            this.groupBox2.Text = ("Schedule");
            // 
            // bnBrowseWTWTXT
            // 
            this.bnBrowseWTWTXT.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseWTWTXT.Location = (new global::System.Drawing.Point(364, 118));
            this.bnBrowseWTWTXT.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnBrowseWTWTXT.Name = ("bnBrowseWTWTXT");
            this.bnBrowseWTWTXT.Size = (new global::System.Drawing.Size(88, 27));
            this.bnBrowseWTWTXT.TabIndex = (27);
            this.bnBrowseWTWTXT.Text = ("Browse...");
            this.bnBrowseWTWTXT.UseVisualStyleBackColor = (true);
            this.bnBrowseWTWTXT.Click += (this.bnBrowseWTWTXT_Click);
            // 
            // txtWTWTXT
            // 
            this.txtWTWTXT.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtWTWTXT.Location = (new global::System.Drawing.Point(76, 120));
            this.txtWTWTXT.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtWTWTXT.Name = ("txtWTWTXT");
            this.txtWTWTXT.Size = (new global::System.Drawing.Size(279, 23));
            this.txtWTWTXT.TabIndex = (26);
            // 
            // cbWTWTXT
            // 
            this.cbWTWTXT.AutoSize = (true);
            this.cbWTWTXT.Location = (new global::System.Drawing.Point(9, 122));
            this.cbWTWTXT.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbWTWTXT.Name = ("cbWTWTXT");
            this.cbWTWTXT.Size = (new global::System.Drawing.Size(45, 19));
            this.cbWTWTXT.TabIndex = (25);
            this.cbWTWTXT.Text = ("TXT");
            this.cbWTWTXT.UseVisualStyleBackColor = (true);
            this.cbWTWTXT.CheckedChanged += (this.EnableDisable);
            // 
            // bnBrowseWTWICAL
            // 
            this.bnBrowseWTWICAL.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseWTWICAL.Location = (new global::System.Drawing.Point(364, 88));
            this.bnBrowseWTWICAL.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnBrowseWTWICAL.Name = ("bnBrowseWTWICAL");
            this.bnBrowseWTWICAL.Size = (new global::System.Drawing.Size(88, 27));
            this.bnBrowseWTWICAL.TabIndex = (24);
            this.bnBrowseWTWICAL.Text = ("Browse...");
            this.bnBrowseWTWICAL.UseVisualStyleBackColor = (true);
            this.bnBrowseWTWICAL.Click += (this.bnBrowseWTWICAL_Click);
            // 
            // txtWTWICAL
            // 
            this.txtWTWICAL.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtWTWICAL.Location = (new global::System.Drawing.Point(76, 90));
            this.txtWTWICAL.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtWTWICAL.Name = ("txtWTWICAL");
            this.txtWTWICAL.Size = (new global::System.Drawing.Size(279, 23));
            this.txtWTWICAL.TabIndex = (23);
            // 
            // cbWTWICAL
            // 
            this.cbWTWICAL.AutoSize = (true);
            this.cbWTWICAL.Location = (new global::System.Drawing.Point(9, 92));
            this.cbWTWICAL.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbWTWICAL.Name = ("cbWTWICAL");
            this.cbWTWICAL.Size = (new global::System.Drawing.Size(46, 19));
            this.cbWTWICAL.TabIndex = (22);
            this.cbWTWICAL.Text = ("iCal");
            this.cbWTWICAL.UseVisualStyleBackColor = (true);
            this.cbWTWICAL.CheckedChanged += (this.EnableDisable);
            // 
            // label4
            // 
            this.label4.AutoSize = (true);
            this.label4.Location = (new global::System.Drawing.Point(382, 153));
            this.label4.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label4.Name = ("label4");
            this.label4.Size = (new global::System.Drawing.Size(65, 15));
            this.label4.TabIndex = (21);
            this.label4.Text = ("in the past.");
            // 
            // txtExportRSSDaysPast
            // 
            this.txtExportRSSDaysPast.Location = (new global::System.Drawing.Point(340, 150));
            this.txtExportRSSDaysPast.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtExportRSSDaysPast.Name = ("txtExportRSSDaysPast");
            this.txtExportRSSDaysPast.Size = (new global::System.Drawing.Size(32, 23));
            this.txtExportRSSDaysPast.TabIndex = (20);
            this.txtExportRSSDaysPast.TextChanged += (this.EnsureInteger);
            this.txtExportRSSDaysPast.KeyPress += (this.TxtNumberOnlyKeyPress);
            // 
            // bnBrowseWTWXML
            // 
            this.bnBrowseWTWXML.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseWTWXML.Location = (new global::System.Drawing.Point(363, 53));
            this.bnBrowseWTWXML.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnBrowseWTWXML.Name = ("bnBrowseWTWXML");
            this.bnBrowseWTWXML.Size = (new global::System.Drawing.Size(88, 27));
            this.bnBrowseWTWXML.TabIndex = (19);
            this.bnBrowseWTWXML.Text = ("Browse...");
            this.bnBrowseWTWXML.UseVisualStyleBackColor = (true);
            this.bnBrowseWTWXML.Click += (this.bnBrowseWTWXML_Click);
            // 
            // txtWTWXML
            // 
            this.txtWTWXML.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtWTWXML.Location = (new global::System.Drawing.Point(76, 57));
            this.txtWTWXML.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtWTWXML.Name = ("txtWTWXML");
            this.txtWTWXML.Size = (new global::System.Drawing.Size(279, 23));
            this.txtWTWXML.TabIndex = (18);
            // 
            // cbWTWXML
            // 
            this.cbWTWXML.AutoSize = (true);
            this.cbWTWXML.Location = (new global::System.Drawing.Point(9, 59));
            this.cbWTWXML.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbWTWXML.Name = ("cbWTWXML");
            this.cbWTWXML.Size = (new global::System.Drawing.Size(50, 19));
            this.cbWTWXML.TabIndex = (17);
            this.cbWTWXML.Text = ("XML");
            this.cbWTWXML.UseVisualStyleBackColor = (true);
            this.cbWTWXML.CheckedChanged += (this.EnableDisable);
            // 
            // bnBrowseWTWRSS
            // 
            this.bnBrowseWTWRSS.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseWTWRSS.Location = (new global::System.Drawing.Point(364, 21));
            this.bnBrowseWTWRSS.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnBrowseWTWRSS.Name = ("bnBrowseWTWRSS");
            this.bnBrowseWTWRSS.Size = (new global::System.Drawing.Size(88, 27));
            this.bnBrowseWTWRSS.TabIndex = (2);
            this.bnBrowseWTWRSS.Text = ("Browse...");
            this.bnBrowseWTWRSS.UseVisualStyleBackColor = (true);
            this.bnBrowseWTWRSS.Click += (this.bnBrowseWTWRSS_Click);
            // 
            // txtWTWRSS
            // 
            this.txtWTWRSS.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtWTWRSS.Location = (new global::System.Drawing.Point(75, 23));
            this.txtWTWRSS.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtWTWRSS.Name = ("txtWTWRSS");
            this.txtWTWRSS.Size = (new global::System.Drawing.Size(282, 23));
            this.txtWTWRSS.TabIndex = (1);
            // 
            // cbWTWRSS
            // 
            this.cbWTWRSS.AutoSize = (true);
            this.cbWTWRSS.Location = (new global::System.Drawing.Point(9, 25));
            this.cbWTWRSS.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbWTWRSS.Name = ("cbWTWRSS");
            this.cbWTWRSS.Size = (new global::System.Drawing.Size(45, 19));
            this.cbWTWRSS.TabIndex = (0);
            this.cbWTWRSS.Text = ("RSS");
            this.cbWTWRSS.UseVisualStyleBackColor = (true);
            this.cbWTWRSS.CheckedChanged += (this.EnableDisable);
            // 
            // label17
            // 
            this.label17.AutoSize = (true);
            this.label17.Location = (new global::System.Drawing.Point(244, 153));
            this.label17.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label17.Name = ("label17");
            this.label17.Size = (new global::System.Drawing.Size(88, 15));
            this.label17.TabIndex = (7);
            this.label17.Text = ("days worth and");
            // 
            // label16
            // 
            this.label16.AutoSize = (true);
            this.label16.Location = (new global::System.Drawing.Point(136, 153));
            this.label16.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label16.Name = ("label16");
            this.label16.Size = (new global::System.Drawing.Size(57, 15));
            this.label16.TabIndex = (5);
            this.label16.Text = ("shows, or");
            // 
            // label15
            // 
            this.label15.AutoSize = (true);
            this.label15.Location = (new global::System.Drawing.Point(7, 153));
            this.label15.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label15.Name = ("label15");
            this.label15.Size = (new global::System.Drawing.Size(81, 15));
            this.label15.TabIndex = (3);
            this.label15.Text = ("No more than");
            // 
            // txtExportRSSMaxDays
            // 
            this.txtExportRSSMaxDays.Location = (new global::System.Drawing.Point(204, 150));
            this.txtExportRSSMaxDays.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtExportRSSMaxDays.Name = ("txtExportRSSMaxDays");
            this.txtExportRSSMaxDays.Size = (new global::System.Drawing.Size(32, 23));
            this.txtExportRSSMaxDays.TabIndex = (6);
            this.txtExportRSSMaxDays.TextChanged += (this.EnsureInteger);
            this.txtExportRSSMaxDays.KeyPress += (this.TxtNumberOnlyKeyPress);
            // 
            // txtExportRSSMaxShows
            // 
            this.txtExportRSSMaxShows.Location = (new global::System.Drawing.Point(97, 150));
            this.txtExportRSSMaxShows.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtExportRSSMaxShows.Name = ("txtExportRSSMaxShows");
            this.txtExportRSSMaxShows.Size = (new global::System.Drawing.Size(32, 23));
            this.txtExportRSSMaxShows.TabIndex = (4);
            this.txtExportRSSMaxShows.TextChanged += (this.EnsureInteger);
            this.txtExportRSSMaxShows.KeyPress += (this.TxtNumberOnlyKeyPress);
            // 
            // tbFilesAndFolders
            // 
            this.tbFilesAndFolders.Controls.Add(this.chkUnArchiveFilesInDownloadDirectory);
            this.tbFilesAndFolders.Controls.Add(this.cbFileNameCaseSensitiveMatch);
            this.tbFilesAndFolders.Controls.Add(this.chkUseLibraryFullPathWhenMatchingShows);
            this.tbFilesAndFolders.Controls.Add(this.label66);
            this.tbFilesAndFolders.Controls.Add(this.txtKeepTogether);
            this.tbFilesAndFolders.Controls.Add(this.txtMaxSampleSize);
            this.tbFilesAndFolders.Controls.Add(this.txtOtherExtensions);
            this.tbFilesAndFolders.Controls.Add(this.txtVideoExtensions);
            this.tbFilesAndFolders.Controls.Add(this.label39);
            this.tbFilesAndFolders.Controls.Add(this.cbKeepTogetherMode);
            this.tbFilesAndFolders.Controls.Add(this.bnReplaceRemove);
            this.tbFilesAndFolders.Controls.Add(this.bnReplaceAdd);
            this.tbFilesAndFolders.Controls.Add(this.label3);
            this.tbFilesAndFolders.Controls.Add(this.ReplacementsGrid);
            this.tbFilesAndFolders.Controls.Add(this.label19);
            this.tbFilesAndFolders.Controls.Add(this.label22);
            this.tbFilesAndFolders.Controls.Add(this.label14);
            this.tbFilesAndFolders.Controls.Add(this.cbKeepTogether);
            this.tbFilesAndFolders.Controls.Add(this.cbForceLower);
            this.tbFilesAndFolders.Controls.Add(this.cbIgnoreSamples);
            this.tbFilesAndFolders.Controls.Add(this.pbFilesAndFolders);
            this.tbFilesAndFolders.Location = (new global::System.Drawing.Point(149, 4));
            this.tbFilesAndFolders.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbFilesAndFolders.Name = ("tbFilesAndFolders");
            this.tbFilesAndFolders.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbFilesAndFolders.Size = (new global::System.Drawing.Size(500, 684));
            this.tbFilesAndFolders.TabIndex = (1);
            this.tbFilesAndFolders.Text = ("Files and Folders");
            this.tbFilesAndFolders.UseVisualStyleBackColor = (true);
            // 
            // chkUnArchiveFilesInDownloadDirectory
            // 
            this.chkUnArchiveFilesInDownloadDirectory.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.chkUnArchiveFilesInDownloadDirectory.AutoSize = (true);
            this.chkUnArchiveFilesInDownloadDirectory.Location = (new global::System.Drawing.Point(4, 508));
            this.chkUnArchiveFilesInDownloadDirectory.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkUnArchiveFilesInDownloadDirectory.Name = ("chkUnArchiveFilesInDownloadDirectory");
            this.chkUnArchiveFilesInDownloadDirectory.Size = (new global::System.Drawing.Size(237, 19));
            this.chkUnArchiveFilesInDownloadDirectory.TabIndex = (43);
            this.chkUnArchiveFilesInDownloadDirectory.Text = ("Extract Archives found in Search Folders");
            this.chkUnArchiveFilesInDownloadDirectory.UseVisualStyleBackColor = (true);
            // 
            // cbFileNameCaseSensitiveMatch
            // 
            this.cbFileNameCaseSensitiveMatch.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.cbFileNameCaseSensitiveMatch.AutoSize = (true);
            this.cbFileNameCaseSensitiveMatch.Location = (new global::System.Drawing.Point(4, 482));
            this.cbFileNameCaseSensitiveMatch.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbFileNameCaseSensitiveMatch.Name = ("cbFileNameCaseSensitiveMatch");
            this.cbFileNameCaseSensitiveMatch.Size = (new global::System.Drawing.Size(211, 19));
            this.cbFileNameCaseSensitiveMatch.TabIndex = (42);
            this.cbFileNameCaseSensitiveMatch.Text = ("Case Sensitive Match for Filenames");
            this.cbFileNameCaseSensitiveMatch.UseVisualStyleBackColor = (true);
            // 
            // chkUseLibraryFullPathWhenMatchingShows
            // 
            this.chkUseLibraryFullPathWhenMatchingShows.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.chkUseLibraryFullPathWhenMatchingShows.AutoSize = (true);
            this.chkUseLibraryFullPathWhenMatchingShows.Location = (new global::System.Drawing.Point(4, 455));
            this.chkUseLibraryFullPathWhenMatchingShows.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkUseLibraryFullPathWhenMatchingShows.Name = ("chkUseLibraryFullPathWhenMatchingShows");
            this.chkUseLibraryFullPathWhenMatchingShows.Size = (new global::System.Drawing.Size(469, 19));
            this.chkUseLibraryFullPathWhenMatchingShows.TabIndex = (41);
            this.chkUseLibraryFullPathWhenMatchingShows.Text = ("Use name of Library Folder when searching for a match between a file and a tv show");
            this.chkUseLibraryFullPathWhenMatchingShows.UseVisualStyleBackColor = (true);
            // 
            // label66
            // 
            this.label66.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.label66.AutoSize = (true);
            this.label66.Location = (new global::System.Drawing.Point(7, 8));
            this.label66.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label66.Name = ("label66");
            this.label66.Size = (new global::System.Drawing.Size(356, 45));
            this.label66.TabIndex = (39);
            this.label66.Text = ("These preferences control how TV Rename copies files across from\r\nyour search folders to your library. Often there are other files that\r\nyou'd like copied as well.");
            // 
            // txtMaxSampleSize
            // 
            this.txtMaxSampleSize.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.txtMaxSampleSize.Location = (new global::System.Drawing.Point(197, 399));
            this.txtMaxSampleSize.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtMaxSampleSize.Name = ("txtMaxSampleSize");
            this.txtMaxSampleSize.Size = (new global::System.Drawing.Size(61, 23));
            this.txtMaxSampleSize.TabIndex = (14);
            this.txtMaxSampleSize.TextChanged += (this.EnsureInteger);
            this.txtMaxSampleSize.KeyPress += (this.TxtNumberOnlyKeyPress);
            // 
            // txtVideoExtensions
            // 
            this.txtVideoExtensions.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtVideoExtensions.Location = (new global::System.Drawing.Point(115, 269));
            this.txtVideoExtensions.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtVideoExtensions.Name = ("txtVideoExtensions");
            this.txtVideoExtensions.Size = (new global::System.Drawing.Size(348, 23));
            this.txtVideoExtensions.TabIndex = (5);
            // 
            // label39
            // 
            this.label39.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.label39.AutoSize = (true);
            this.label39.Location = (new global::System.Drawing.Point(29, 363));
            this.label39.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label39.Name = ("label39");
            this.label39.Size = (new global::System.Drawing.Size(22, 15));
            this.label39.TabIndex = (22);
            this.label39.Text = ("Do");
            // 
            // cbKeepTogetherMode
            // 
            this.cbKeepTogetherMode.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.cbKeepTogetherMode.DropDownStyle = (global::System.Windows.Forms.ComboBoxStyle.DropDownList);
            this.cbKeepTogetherMode.FormattingEnabled = (true);
            this.cbKeepTogetherMode.Items.AddRange(new global::System.Object[] { "All", "All but these", "Just" });
            this.cbKeepTogetherMode.Location = (new global::System.Drawing.Point(61, 360));
            this.cbKeepTogetherMode.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbKeepTogetherMode.Name = ("cbKeepTogetherMode");
            this.cbKeepTogetherMode.Size = (new global::System.Drawing.Size(170, 23));
            this.cbKeepTogetherMode.Sorted = (true);
            this.cbKeepTogetherMode.TabIndex = (21);
            this.cbKeepTogetherMode.SelectedIndexChanged += (this.EnableDisable);
            // 
            // bnReplaceRemove
            // 
            this.bnReplaceRemove.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.bnReplaceRemove.Location = (new global::System.Drawing.Point(105, 226));
            this.bnReplaceRemove.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnReplaceRemove.Name = ("bnReplaceRemove");
            this.bnReplaceRemove.Size = (new global::System.Drawing.Size(88, 27));
            this.bnReplaceRemove.TabIndex = (3);
            this.bnReplaceRemove.Text = ("&Remove");
            this.bnReplaceRemove.UseVisualStyleBackColor = (true);
            this.bnReplaceRemove.Click += (this.bnReplaceRemove_Click);
            // 
            // bnReplaceAdd
            // 
            this.bnReplaceAdd.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.bnReplaceAdd.Location = (new global::System.Drawing.Point(10, 226));
            this.bnReplaceAdd.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnReplaceAdd.Name = ("bnReplaceAdd");
            this.bnReplaceAdd.Size = (new global::System.Drawing.Size(88, 27));
            this.bnReplaceAdd.TabIndex = (2);
            this.bnReplaceAdd.Text = ("&Add");
            this.bnReplaceAdd.UseVisualStyleBackColor = (true);
            this.bnReplaceAdd.Click += (this.bnReplaceAdd_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = (true);
            this.label3.Location = (new global::System.Drawing.Point(7, 69));
            this.label3.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label3.Name = ("label3");
            this.label3.Size = (new global::System.Drawing.Size(132, 15));
            this.label3.TabIndex = (0);
            this.label3.Text = ("Filename Replacements");
            // 
            // ReplacementsGrid
            // 
            this.ReplacementsGrid.Anchor = ((global::System.Windows.Forms.AnchorStyles)((((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Bottom)) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.ReplacementsGrid.BackColor = (global::System.Drawing.SystemColors.Window);
            this.ReplacementsGrid.EnableSort = (true);
            this.ReplacementsGrid.Location = (new global::System.Drawing.Point(7, 88));
            this.ReplacementsGrid.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.ReplacementsGrid.Name = ("ReplacementsGrid");
            this.ReplacementsGrid.OptimizeMode = (global::SourceGrid.CellOptimizeMode.ForRows);
            this.ReplacementsGrid.SelectionMode = (global::SourceGrid.GridSelectionMode.Cell);
            this.ReplacementsGrid.Size = (new global::System.Drawing.Size(457, 132));
            this.ReplacementsGrid.TabIndex = (1);
            this.ReplacementsGrid.TabStop = (true);
            this.ReplacementsGrid.ToolTipText = ("");
            // 
            // label19
            // 
            this.label19.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.label19.AutoSize = (true);
            this.label19.Location = (new global::System.Drawing.Point(262, 403));
            this.label19.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label19.Name = ("label19");
            this.label19.Size = (new global::System.Drawing.Size(60, 15));
            this.label19.TabIndex = (15);
            this.label19.Text = ("MB in size");
            // 
            // label22
            // 
            this.label22.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.label22.AutoSize = (true);
            this.label22.Location = (new global::System.Drawing.Point(4, 302));
            this.label22.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label22.Name = ("label22");
            this.label22.Size = (new global::System.Drawing.Size(99, 15));
            this.label22.TabIndex = (6);
            this.label22.Text = ("&Other extensions:");
            // 
            // label14
            // 
            this.label14.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.label14.AutoSize = (true);
            this.label14.Location = (new global::System.Drawing.Point(4, 272));
            this.label14.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label14.Name = ("label14");
            this.label14.Size = (new global::System.Drawing.Size(99, 15));
            this.label14.TabIndex = (4);
            this.label14.Text = ("&Video extensions:");
            // 
            // cbKeepTogether
            // 
            this.cbKeepTogether.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.cbKeepTogether.AutoSize = (true);
            this.cbKeepTogether.Location = (new global::System.Drawing.Point(7, 330));
            this.cbKeepTogether.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbKeepTogether.Name = ("cbKeepTogether");
            this.cbKeepTogether.Size = (new global::System.Drawing.Size(384, 19));
            this.cbKeepTogether.TabIndex = (8);
            this.cbKeepTogether.Text = ("&Copy/Move files with same base name as video from Search Folders");
            this.cbKeepTogether.UseVisualStyleBackColor = (true);
            this.cbKeepTogether.CheckedChanged += (this.EnableDisable);
            // 
            // cbForceLower
            // 
            this.cbForceLower.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.cbForceLower.AutoSize = (true);
            this.cbForceLower.Location = (new global::System.Drawing.Point(4, 429));
            this.cbForceLower.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbForceLower.Name = ("cbForceLower");
            this.cbForceLower.Size = (new global::System.Drawing.Size(182, 19));
            this.cbForceLower.TabIndex = (16);
            this.cbForceLower.Text = ("&Make all filenames lower case");
            this.cbForceLower.UseVisualStyleBackColor = (true);
            // 
            // cbIgnoreSamples
            // 
            this.cbIgnoreSamples.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.cbIgnoreSamples.AutoSize = (true);
            this.cbIgnoreSamples.Location = (new global::System.Drawing.Point(4, 402));
            this.cbIgnoreSamples.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbIgnoreSamples.Name = ("cbIgnoreSamples");
            this.cbIgnoreSamples.Size = (new global::System.Drawing.Size(182, 19));
            this.cbIgnoreSamples.TabIndex = (13);
            this.cbIgnoreSamples.Text = ("&Ignore \"sample\" videos, up to");
            this.cbIgnoreSamples.UseVisualStyleBackColor = (true);
            // 
            // pbFilesAndFolders
            // 
            this.pbFilesAndFolders.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.pbFilesAndFolders.Cursor = (global::System.Windows.Forms.Cursors.Hand);
            this.pbFilesAndFolders.Image = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbFilesAndFolders.InitialImage = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbFilesAndFolders.Location = (new global::System.Drawing.Point(418, 7));
            this.pbFilesAndFolders.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.pbFilesAndFolders.Name = ("pbFilesAndFolders");
            this.pbFilesAndFolders.Size = (new global::System.Drawing.Size(50, 46));
            this.pbFilesAndFolders.SizeMode = (global::System.Windows.Forms.PictureBoxSizeMode.CenterImage);
            this.pbFilesAndFolders.TabIndex = (33);
            this.pbFilesAndFolders.TabStop = (false);
            this.pbFilesAndFolders.Click += (this.pbFilesAndFolders_Click);
            // 
            // tbGeneral
            // 
            this.tbGeneral.Controls.Add(this.cbAutoSaveOnExit);
            this.tbGeneral.Controls.Add(this.chkAutoAddAsPartOfQuickRename);
            this.tbGeneral.Controls.Add(this.chkShareCriticalLogs);
            this.tbGeneral.Controls.Add(this.label60);
            this.tbGeneral.Controls.Add(this.pbGeneral);
            this.tbGeneral.Controls.Add(this.txtWTWDays);
            this.tbGeneral.Controls.Add(this.cbMode);
            this.tbGeneral.Controls.Add(this.label34);
            this.tbGeneral.Controls.Add(this.label2);
            this.tbGeneral.Location = (new global::System.Drawing.Point(149, 4));
            this.tbGeneral.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbGeneral.Name = ("tbGeneral");
            this.tbGeneral.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbGeneral.Size = (new global::System.Drawing.Size(500, 684));
            this.tbGeneral.TabIndex = (0);
            this.tbGeneral.Text = ("General");
            this.tbGeneral.UseVisualStyleBackColor = (true);
            // 
            // chkAutoAddAsPartOfQuickRename
            // 
            this.chkAutoAddAsPartOfQuickRename.AutoSize = (true);
            this.chkAutoAddAsPartOfQuickRename.Location = (new global::System.Drawing.Point(15, 147));
            this.chkAutoAddAsPartOfQuickRename.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkAutoAddAsPartOfQuickRename.Name = ("chkAutoAddAsPartOfQuickRename");
            this.chkAutoAddAsPartOfQuickRename.Size = (new global::System.Drawing.Size(211, 19));
            this.chkAutoAddAsPartOfQuickRename.TabIndex = (43);
            this.chkAutoAddAsPartOfQuickRename.Text = ("Auto-Add as part of Quick Rename");
            this.chkAutoAddAsPartOfQuickRename.UseVisualStyleBackColor = (true);
            // 
            // chkShareCriticalLogs
            // 
            this.chkShareCriticalLogs.AutoSize = (true);
            this.chkShareCriticalLogs.Location = (new global::System.Drawing.Point(15, 120));
            this.chkShareCriticalLogs.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkShareCriticalLogs.Name = ("chkShareCriticalLogs");
            this.chkShareCriticalLogs.Size = (new global::System.Drawing.Size(231, 19));
            this.chkShareCriticalLogs.TabIndex = (42);
            this.chkShareCriticalLogs.Text = ("Share critical errors to help defeat bugs");
            this.chkShareCriticalLogs.UseVisualStyleBackColor = (true);
            // 
            // label60
            // 
            this.label60.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.label60.AutoSize = (true);
            this.label60.Location = (new global::System.Drawing.Point(7, 7));
            this.label60.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label60.Name = ("label60");
            this.label60.Size = (new global::System.Drawing.Size(322, 30));
            this.label60.TabIndex = (38);
            this.label60.Text = ("General settings to control TV Rename's scan and download\r\nbehaviour");
            // 
            // pbGeneral
            // 
            this.pbGeneral.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.pbGeneral.Cursor = (global::System.Windows.Forms.Cursors.Hand);
            this.pbGeneral.Image = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbGeneral.InitialImage = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbGeneral.Location = (new global::System.Drawing.Point(418, 7));
            this.pbGeneral.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.pbGeneral.Name = ("pbGeneral");
            this.pbGeneral.Size = (new global::System.Drawing.Size(50, 46));
            this.pbGeneral.SizeMode = (global::System.Windows.Forms.PictureBoxSizeMode.CenterImage);
            this.pbGeneral.TabIndex = (23);
            this.pbGeneral.TabStop = (false);
            this.pbGeneral.Click += (this.pbGeneral_Click);
            // 
            // txtWTWDays
            // 
            this.txtWTWDays.Location = (new global::System.Drawing.Point(15, 59));
            this.txtWTWDays.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtWTWDays.Name = ("txtWTWDays");
            this.txtWTWDays.Size = (new global::System.Drawing.Size(32, 23));
            this.txtWTWDays.TabIndex = (1);
            this.txtWTWDays.TextChanged += (this.EnsureInteger);
            this.txtWTWDays.KeyPress += (this.TxtNumberOnlyKeyPress);
            // 
            // cbMode
            // 
            this.cbMode.DropDownStyle = (global::System.Windows.Forms.ComboBoxStyle.DropDownList);
            this.cbMode.FormattingEnabled = (true);
            this.cbMode.Items.AddRange(new global::System.Object[] { "Beta", "Production" });
            this.cbMode.Location = (new global::System.Drawing.Point(136, 89));
            this.cbMode.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbMode.Name = ("cbMode");
            this.cbMode.Size = (new global::System.Drawing.Size(170, 23));
            this.cbMode.Sorted = (true);
            this.cbMode.TabIndex = (19);
            // 
            // label34
            // 
            this.label34.AutoSize = (true);
            this.label34.Location = (new global::System.Drawing.Point(13, 92));
            this.label34.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label34.Name = ("label34");
            this.label34.Size = (new global::System.Drawing.Size(41, 15));
            this.label34.TabIndex = (18);
            this.label34.Text = ("&Mode:");
            // 
            // label2
            // 
            this.label2.AutoSize = (true);
            this.label2.Location = (new global::System.Drawing.Point(55, 62));
            this.label2.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label2.Name = ("label2");
            this.label2.Size = (new global::System.Drawing.Size(120, 15));
            this.label2.TabIndex = (2);
            this.label2.Text = ("days counts as recent");
            // 
            // tcTabs
            // 
            this.tcTabs.Alignment = (global::System.Windows.Forms.TabAlignment.Left);
            this.tcTabs.Anchor = ((global::System.Windows.Forms.AnchorStyles)((((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Bottom)) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.tcTabs.Controls.Add(this.tbGeneral);
            this.tcTabs.Controls.Add(this.tpDisplay);
            this.tcTabs.Controls.Add(this.tpDataSources);
            this.tcTabs.Controls.Add(this.tpLibraryFolders);
            this.tcTabs.Controls.Add(this.tpMovieDefaults);
            this.tcTabs.Controls.Add(this.tpShowDefaults);
            this.tcTabs.Controls.Add(this.tpScanSettings);
            this.tcTabs.Controls.Add(this.tbFilesAndFolders);
            this.tcTabs.Controls.Add(this.tpSubtitles);
            this.tcTabs.Controls.Add(this.tbSearchFolders);
            this.tcTabs.Controls.Add(this.tbFolderDeleting);
            this.tcTabs.Controls.Add(this.tbMediaCenter);
            this.tcTabs.Controls.Add(this.tpTorrentNZB);
            this.tcTabs.Controls.Add(this.tpRSSJSONSearch);
            this.tcTabs.Controls.Add(this.tpJackett);
            this.tcTabs.Controls.Add(this.tbAutoExport);
            this.tcTabs.Controls.Add(this.tpAutoExportLibrary);
            this.tcTabs.Controls.Add(this.tbAppUpdate);
            this.tcTabs.DrawMode = (global::System.Windows.Forms.TabDrawMode.OwnerDrawFixed);
            this.tcTabs.ItemSize = (new global::System.Drawing.Size(30, 145));
            this.tcTabs.Location = (new global::System.Drawing.Point(14, 14));
            this.tcTabs.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tcTabs.Multiline = (true);
            this.tcTabs.Name = ("tcTabs");
            this.tcTabs.SelectedIndex = (0);
            this.tcTabs.Size = (new global::System.Drawing.Size(653, 692));
            this.tcTabs.SizeMode = (global::System.Windows.Forms.TabSizeMode.Fixed);
            this.tcTabs.TabIndex = (0);
            this.tcTabs.DrawItem += (this.tpSearch_DrawItem);
            // 
            // tpDataSources
            // 
            this.tpDataSources.Controls.Add(this.panel4);
            this.tpDataSources.Controls.Add(this.panel3);
            this.tpDataSources.Controls.Add(this.label100);
            this.tpDataSources.Controls.Add(this.panel2);
            this.tpDataSources.Controls.Add(this.label83);
            this.tpDataSources.Controls.Add(this.gbTMDB);
            this.tpDataSources.Controls.Add(this.label63);
            this.tpDataSources.Controls.Add(this.label57);
            this.tpDataSources.Controls.Add(this.label40);
            this.tpDataSources.Controls.Add(this.domainUpDown2);
            this.tpDataSources.Controls.Add(this.txtParallelDownloads);
            this.tpDataSources.Controls.Add(this.label21);
            this.tpDataSources.Controls.Add(this.label20);
            this.tpDataSources.Controls.Add(this.groupBox21);
            this.tpDataSources.Controls.Add(this.groupBox20);
            this.tpDataSources.Controls.Add(this.label33);
            this.tpDataSources.Controls.Add(this.pbSources);
            this.tpDataSources.Location = (new global::System.Drawing.Point(149, 4));
            this.tpDataSources.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpDataSources.Name = ("tpDataSources");
            this.tpDataSources.Size = (new global::System.Drawing.Size(500, 684));
            this.tpDataSources.TabIndex = (15);
            this.tpDataSources.Text = ("Data Sources");
            this.tpDataSources.UseVisualStyleBackColor = (true);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.rdoMovieTMDB);
            this.panel3.Controls.Add(this.rdoMovieTheTVDB);
            this.panel3.Location = (new global::System.Drawing.Point(150, 104));
            this.panel3.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.panel3.Name = ("panel3");
            this.panel3.Size = (new global::System.Drawing.Size(303, 33));
            this.panel3.TabIndex = (63);
            // 
            // rdoMovieTMDB
            // 
            this.rdoMovieTMDB.AutoSize = (true);
            this.rdoMovieTMDB.Location = (new global::System.Drawing.Point(202, 5));
            this.rdoMovieTMDB.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rdoMovieTMDB.Name = ("rdoMovieTMDB");
            this.rdoMovieTMDB.Size = (new global::System.Drawing.Size(57, 19));
            this.rdoMovieTMDB.TabIndex = (60);
            this.rdoMovieTMDB.TabStop = (true);
            this.rdoMovieTMDB.Text = ("TMDB");
            this.rdoMovieTMDB.UseVisualStyleBackColor = (true);
            // 
            // rdoMovieTheTVDB
            // 
            this.rdoMovieTheTVDB.AutoSize = (true);
            this.rdoMovieTheTVDB.Location = (new global::System.Drawing.Point(4, 5));
            this.rdoMovieTheTVDB.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rdoMovieTheTVDB.Name = ("rdoMovieTheTVDB");
            this.rdoMovieTheTVDB.Size = (new global::System.Drawing.Size(75, 19));
            this.rdoMovieTheTVDB.TabIndex = (59);
            this.rdoMovieTheTVDB.TabStop = (true);
            this.rdoMovieTheTVDB.Text = ("The TVDB");
            this.rdoMovieTheTVDB.UseVisualStyleBackColor = (true);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rdoTVTMDB);
            this.panel2.Controls.Add(this.rdoTVTVMaze);
            this.panel2.Controls.Add(this.rdoTVTVDB);
            this.panel2.Location = (new global::System.Drawing.Point(150, 68));
            this.panel2.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.panel2.Name = ("panel2");
            this.panel2.Size = (new global::System.Drawing.Size(303, 33));
            this.panel2.TabIndex = (62);
            // 
            // rdoTVTMDB
            // 
            this.rdoTVTMDB.AutoSize = (true);
            this.rdoTVTMDB.Location = (new global::System.Drawing.Point(202, 5));
            this.rdoTVTMDB.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rdoTVTMDB.Name = ("rdoTVTMDB");
            this.rdoTVTMDB.Size = (new global::System.Drawing.Size(57, 19));
            this.rdoTVTMDB.TabIndex = (61);
            this.rdoTVTMDB.TabStop = (true);
            this.rdoTVTMDB.Text = ("TMDB");
            this.rdoTVTMDB.UseVisualStyleBackColor = (true);
            // 
            // rdoTVTVMaze
            // 
            this.rdoTVTVMaze.AutoSize = (true);
            this.rdoTVTVMaze.Location = (new global::System.Drawing.Point(105, 3));
            this.rdoTVTVMaze.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rdoTVTVMaze.Name = ("rdoTVTVMaze");
            this.rdoTVTVMaze.Size = (new global::System.Drawing.Size(69, 19));
            this.rdoTVTVMaze.TabIndex = (46);
            this.rdoTVTVMaze.TabStop = (true);
            this.rdoTVTVMaze.Text = ("TV Maze");
            this.rdoTVTVMaze.UseVisualStyleBackColor = (true);
            // 
            // rdoTVTVDB
            // 
            this.rdoTVTVDB.AutoSize = (true);
            this.rdoTVTVDB.Location = (new global::System.Drawing.Point(4, 3));
            this.rdoTVTVDB.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rdoTVTVDB.Name = ("rdoTVTVDB");
            this.rdoTVTVDB.Size = (new global::System.Drawing.Size(75, 19));
            this.rdoTVTVDB.TabIndex = (45);
            this.rdoTVTVDB.TabStop = (true);
            this.rdoTVTVDB.Text = ("The TVDB");
            this.rdoTVTVDB.UseVisualStyleBackColor = (true);
            // 
            // label83
            // 
            this.label83.AutoSize = (true);
            this.label83.Location = (new global::System.Drawing.Point(10, 108));
            this.label83.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label83.Name = ("label83");
            this.label83.Size = (new global::System.Drawing.Size(123, 15));
            this.label83.TabIndex = (61);
            this.label83.Text = ("Default Movie Source:");
            this.label83.TextAlign = (global::System.Drawing.ContentAlignment.TopRight);
            // 
            // gbTMDB
            // 
            this.gbTMDB.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.gbTMDB.Controls.Add(this.label84);
            this.gbTMDB.Controls.Add(this.cbTMDBRegions);
            this.gbTMDB.Controls.Add(this.label80);
            this.gbTMDB.Controls.Add(this.label81);
            this.gbTMDB.Controls.Add(this.tbTMDBPercentDirty);
            this.gbTMDB.Controls.Add(this.label82);
            this.gbTMDB.Controls.Add(this.cbTMDBLanguages);
            this.gbTMDB.Location = (new global::System.Drawing.Point(14, 479));
            this.gbTMDB.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.gbTMDB.Name = ("gbTMDB");
            this.gbTMDB.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.gbTMDB.Size = (new global::System.Drawing.Size(448, 115));
            this.gbTMDB.TabIndex = (58);
            this.gbTMDB.TabStop = (false);
            this.gbTMDB.Text = ("TMDB");
            // 
            // cbTMDBRegions
            // 
            this.cbTMDBRegions.DropDownStyle = (global::System.Windows.Forms.ComboBoxStyle.DropDownList);
            this.cbTMDBRegions.FormattingEnabled = (true);
            this.cbTMDBRegions.Location = (new global::System.Drawing.Point(135, 52));
            this.cbTMDBRegions.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbTMDBRegions.Name = ("cbTMDBRegions");
            this.cbTMDBRegions.Size = (new global::System.Drawing.Size(170, 23));
            this.cbTMDBRegions.Sorted = (true);
            this.cbTMDBRegions.TabIndex = (27);
            // 
            // label80
            // 
            this.label80.AutoSize = (true);
            this.label80.Location = (new global::System.Drawing.Point(12, 87));
            this.label80.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label80.Name = ("label80");
            this.label80.Size = (new global::System.Drawing.Size(124, 15));
            this.label80.TabIndex = (23);
            this.label80.Text = ("Refresh entire series  if");
            // 
            // label81
            // 
            this.label81.AutoSize = (true);
            this.label81.Location = (new global::System.Drawing.Point(189, 87));
            this.label81.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label81.Name = ("label81");
            this.label81.Size = (new global::System.Drawing.Size(146, 15));
            this.label81.TabIndex = (25);
            this.label81.Text = ("% of episodes are updated");
            // 
            // tbTMDBPercentDirty
            // 
            this.tbTMDBPercentDirty.Location = (new global::System.Drawing.Point(148, 83));
            this.tbTMDBPercentDirty.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbTMDBPercentDirty.Name = ("tbTMDBPercentDirty");
            this.tbTMDBPercentDirty.Size = (new global::System.Drawing.Size(32, 23));
            this.tbTMDBPercentDirty.TabIndex = (24);
            // 
            // label82
            // 
            this.label82.AutoSize = (true);
            this.label82.Location = (new global::System.Drawing.Point(13, 25));
            this.label82.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label82.Name = ("label82");
            this.label82.Size = (new global::System.Drawing.Size(110, 15));
            this.label82.TabIndex = (18);
            this.label82.Text = ("&Preferred language:");
            // 
            // cbTMDBLanguages
            // 
            this.cbTMDBLanguages.DropDownStyle = (global::System.Windows.Forms.ComboBoxStyle.DropDownList);
            this.cbTMDBLanguages.FormattingEnabled = (true);
            this.cbTMDBLanguages.Location = (new global::System.Drawing.Point(136, 22));
            this.cbTMDBLanguages.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbTMDBLanguages.Name = ("cbTMDBLanguages");
            this.cbTMDBLanguages.Size = (new global::System.Drawing.Size(170, 23));
            this.cbTMDBLanguages.Sorted = (true);
            this.cbTMDBLanguages.TabIndex = (19);
            // 
            // label63
            // 
            this.label63.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.label63.AutoSize = (true);
            this.label63.Location = (new global::System.Drawing.Point(10, 3));
            this.label63.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label63.Name = ("label63");
            this.label63.Size = (new global::System.Drawing.Size(395, 30));
            this.label63.TabIndex = (57);
            this.label63.Text = ("TV Rename downloads information from upstream sources to understand\r\nwhich shows have epiosdes");
            // 
            // label57
            // 
            this.label57.AutoSize = (true);
            this.label57.Location = (new global::System.Drawing.Point(10, 172));
            this.label57.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label57.Name = ("label57");
            this.label57.Size = (new global::System.Drawing.Size(110, 15));
            this.label57.TabIndex = (55);
            this.label57.Text = ("Update cache every");
            // 
            // txtParallelDownloads
            // 
            this.txtParallelDownloads.Location = (new global::System.Drawing.Point(111, 137));
            this.txtParallelDownloads.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtParallelDownloads.Name = ("txtParallelDownloads");
            this.txtParallelDownloads.Size = (new global::System.Drawing.Size(32, 23));
            this.txtParallelDownloads.TabIndex = (51);
            this.txtParallelDownloads.TextChanged += (this.EnsureInteger);
            this.txtParallelDownloads.KeyPress += (this.TxtNumberOnlyKeyPress);
            // 
            // label21
            // 
            this.label21.AutoSize = (true);
            this.label21.Location = (new global::System.Drawing.Point(10, 141));
            this.label21.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label21.Name = ("label21");
            this.label21.Size = (new global::System.Drawing.Size(92, 15));
            this.label21.TabIndex = (50);
            this.label21.Text = ("&Download up to");
            // 
            // label20
            // 
            this.label20.AutoSize = (true);
            this.label20.Location = (new global::System.Drawing.Point(150, 141));
            this.label20.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label20.Name = ("label20");
            this.label20.Size = (new global::System.Drawing.Size(166, 15));
            this.label20.TabIndex = (52);
            this.label20.Text = ("shows/images simultaneously");
            // 
            // groupBox21
            // 
            this.groupBox21.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox21.Location = (new global::System.Drawing.Point(14, 357));
            this.groupBox21.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox21.Name = ("groupBox21");
            this.groupBox21.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox21.Size = (new global::System.Drawing.Size(448, 115));
            this.groupBox21.TabIndex = (49);
            this.groupBox21.TabStop = (false);
            this.groupBox21.Text = ("TV Maze");
            // 
            // groupBox20
            // 
            this.groupBox20.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox20.Controls.Add(this.label91);
            this.groupBox20.Controls.Add(this.cbTVDBVersion);
            this.groupBox20.Controls.Add(this.label37);
            this.groupBox20.Controls.Add(this.label38);
            this.groupBox20.Controls.Add(this.tbPercentDirty);
            this.groupBox20.Controls.Add(this.label10);
            this.groupBox20.Controls.Add(this.cbTVDBLanguages);
            this.groupBox20.Location = (new global::System.Drawing.Point(14, 234));
            this.groupBox20.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox20.Name = ("groupBox20");
            this.groupBox20.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox20.Size = (new global::System.Drawing.Size(448, 115));
            this.groupBox20.TabIndex = (48);
            this.groupBox20.TabStop = (false);
            this.groupBox20.Text = ("TheTVDB");
            // 
            // label91
            // 
            this.label91.AutoSize = (true);
            this.label91.Location = (new global::System.Drawing.Point(12, 87));
            this.label91.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label91.Name = ("label91");
            this.label91.Size = (new global::System.Drawing.Size(48, 15));
            this.label91.TabIndex = (26);
            this.label91.Text = ("Version:");
            // 
            // cbTVDBVersion
            // 
            this.cbTVDBVersion.DropDownStyle = (global::System.Windows.Forms.ComboBoxStyle.DropDownList);
            this.cbTVDBVersion.FormattingEnabled = (true);
            this.cbTVDBVersion.Items.AddRange(new global::System.Object[] { "v4" });
            this.cbTVDBVersion.Location = (new global::System.Drawing.Point(135, 83));
            this.cbTVDBVersion.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbTVDBVersion.Name = ("cbTVDBVersion");
            this.cbTVDBVersion.Size = (new global::System.Drawing.Size(170, 23));
            this.cbTVDBVersion.Sorted = (true);
            this.cbTVDBVersion.TabIndex = (27);
            // 
            // label37
            // 
            this.label37.AutoSize = (true);
            this.label37.Location = (new global::System.Drawing.Point(12, 57));
            this.label37.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label37.Name = ("label37");
            this.label37.Size = (new global::System.Drawing.Size(124, 15));
            this.label37.TabIndex = (23);
            this.label37.Text = ("Refresh entire series  if");
            // 
            // label38
            // 
            this.label38.AutoSize = (true);
            this.label38.Location = (new global::System.Drawing.Point(189, 57));
            this.label38.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label38.Name = ("label38");
            this.label38.Size = (new global::System.Drawing.Size(146, 15));
            this.label38.TabIndex = (25);
            this.label38.Text = ("% of episodes are updated");
            // 
            // tbPercentDirty
            // 
            this.tbPercentDirty.Location = (new global::System.Drawing.Point(148, 53));
            this.tbPercentDirty.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbPercentDirty.Name = ("tbPercentDirty");
            this.tbPercentDirty.Size = (new global::System.Drawing.Size(32, 23));
            this.tbPercentDirty.TabIndex = (24);
            this.tbPercentDirty.TextChanged += (this.EnsureInteger);
            this.tbPercentDirty.KeyPress += (this.TxtNumberOnlyKeyPress);
            // 
            // label10
            // 
            this.label10.AutoSize = (true);
            this.label10.Location = (new global::System.Drawing.Point(13, 25));
            this.label10.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label10.Name = ("label10");
            this.label10.Size = (new global::System.Drawing.Size(110, 15));
            this.label10.TabIndex = (18);
            this.label10.Text = ("&Preferred language:");
            // 
            // cbTVDBLanguages
            // 
            this.cbTVDBLanguages.DropDownStyle = (global::System.Windows.Forms.ComboBoxStyle.DropDownList);
            this.cbTVDBLanguages.FormattingEnabled = (true);
            this.cbTVDBLanguages.Items.AddRange(new global::System.Object[] { "My Shows", "Scan", "Schedule" });
            this.cbTVDBLanguages.Location = (new global::System.Drawing.Point(136, 22));
            this.cbTVDBLanguages.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbTVDBLanguages.Name = ("cbTVDBLanguages");
            this.cbTVDBLanguages.Size = (new global::System.Drawing.Size(170, 23));
            this.cbTVDBLanguages.Sorted = (true);
            this.cbTVDBLanguages.TabIndex = (19);
            // 
            // label33
            // 
            this.label33.AutoSize = (true);
            this.label33.Location = (new global::System.Drawing.Point(10, 75));
            this.label33.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label33.Name = ("label33");
            this.label33.Size = (new global::System.Drawing.Size(135, 15));
            this.label33.TabIndex = (47);
            this.label33.Text = ("Default TV Show Source:");
            this.label33.TextAlign = (global::System.Drawing.ContentAlignment.TopRight);
            // 
            // pbSources
            // 
            this.pbSources.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.pbSources.Cursor = (global::System.Windows.Forms.Cursors.Hand);
            this.pbSources.Image = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbSources.InitialImage = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbSources.Location = (new global::System.Drawing.Point(421, 3));
            this.pbSources.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.pbSources.Name = ("pbSources");
            this.pbSources.Size = (new global::System.Drawing.Size(50, 46));
            this.pbSources.SizeMode = (global::System.Windows.Forms.PictureBoxSizeMode.CenterImage);
            this.pbSources.TabIndex = (56);
            this.pbSources.TabStop = (false);
            this.pbSources.Click += (this.pbSources_Click);
            // 
            // tpMovieDefaults
            // 
            this.tpMovieDefaults.Controls.Add(this.label86);
            this.tpMovieDefaults.Controls.Add(this.groupBox24);
            this.tpMovieDefaults.Controls.Add(this.groupBox25);
            this.tpMovieDefaults.Controls.Add(this.pbMovieDefaults);
            this.tpMovieDefaults.Location = (new global::System.Drawing.Point(149, 4));
            this.tpMovieDefaults.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpMovieDefaults.Name = ("tpMovieDefaults");
            this.tpMovieDefaults.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpMovieDefaults.Size = (new global::System.Drawing.Size(500, 684));
            this.tpMovieDefaults.TabIndex = (18);
            this.tpMovieDefaults.Text = ("Movies Defaults");
            this.tpMovieDefaults.UseVisualStyleBackColor = (true);
            // 
            // label86
            // 
            this.label86.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.label86.AutoSize = (true);
            this.label86.Location = (new global::System.Drawing.Point(8, 13));
            this.label86.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label86.Name = ("label86");
            this.label86.Size = (new global::System.Drawing.Size(359, 45));
            this.label86.TabIndex = (63);
            this.label86.Text = ("These settings control the defaults used to add a new movie to the \r\nsystem. Once the show has been created, you will need to modify\r\nits configuration directly.");
            // 
            // groupBox24
            // 
            this.groupBox24.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox24.Controls.Add(this.cmbDefMovieFolderFormat);
            this.groupBox24.Controls.Add(this.label95);
            this.groupBox24.Controls.Add(this.cmbDefMovieLocation);
            this.groupBox24.Controls.Add(this.cbDefMovieUseDefLocation);
            this.groupBox24.Controls.Add(this.cbDefMovieAutoFolders);
            this.groupBox24.Location = (new global::System.Drawing.Point(7, 68));
            this.groupBox24.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox24.Name = ("groupBox24");
            this.groupBox24.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox24.Size = (new global::System.Drawing.Size(456, 155));
            this.groupBox24.TabIndex = (61);
            this.groupBox24.TabStop = (false);
            this.groupBox24.Text = ("Default Movie Settings");
            // 
            // cmbDefMovieFolderFormat
            // 
            this.cmbDefMovieFolderFormat.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDefMovieFolderFormat.FormattingEnabled = (true);
            this.cmbDefMovieFolderFormat.Location = (new global::System.Drawing.Point(162, 114));
            this.cmbDefMovieFolderFormat.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cmbDefMovieFolderFormat.Name = ("cmbDefMovieFolderFormat");
            this.cmbDefMovieFolderFormat.Size = (new global::System.Drawing.Size(286, 23));
            this.cmbDefMovieFolderFormat.TabIndex = (7);
            // 
            // label95
            // 
            this.label95.AutoSize = (true);
            this.label95.Location = (new global::System.Drawing.Point(7, 118));
            this.label95.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label95.Name = ("label95");
            this.label95.Size = (new global::System.Drawing.Size(144, 15));
            this.label95.TabIndex = (6);
            this.label95.Text = ("Default movie folder type:");
            // 
            // cmbDefMovieLocation
            // 
            this.cmbDefMovieLocation.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDefMovieLocation.FormattingEnabled = (true);
            this.cmbDefMovieLocation.Location = (new global::System.Drawing.Point(34, 78));
            this.cmbDefMovieLocation.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cmbDefMovieLocation.Name = ("cmbDefMovieLocation");
            this.cmbDefMovieLocation.Size = (new global::System.Drawing.Size(415, 23));
            this.cmbDefMovieLocation.TabIndex = (2);
            // 
            // cbDefMovieUseDefLocation
            // 
            this.cbDefMovieUseDefLocation.AutoSize = (true);
            this.cbDefMovieUseDefLocation.Location = (new global::System.Drawing.Point(10, 50));
            this.cbDefMovieUseDefLocation.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDefMovieUseDefLocation.Name = ("cbDefMovieUseDefLocation");
            this.cbDefMovieUseDefLocation.Size = (new global::System.Drawing.Size(135, 19));
            this.cbDefMovieUseDefLocation.TabIndex = (1);
            this.cbDefMovieUseDefLocation.Text = ("Use Default Location");
            this.cbDefMovieUseDefLocation.UseVisualStyleBackColor = (true);
            // 
            // cbDefMovieAutoFolders
            // 
            this.cbDefMovieAutoFolders.AutoSize = (true);
            this.cbDefMovieAutoFolders.Location = (new global::System.Drawing.Point(10, 23));
            this.cbDefMovieAutoFolders.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDefMovieAutoFolders.Name = ("cbDefMovieAutoFolders");
            this.cbDefMovieAutoFolders.Size = (new global::System.Drawing.Size(145, 19));
            this.cbDefMovieAutoFolders.TabIndex = (0);
            this.cbDefMovieAutoFolders.Text = ("Use Automatic Folders");
            this.cbDefMovieAutoFolders.UseVisualStyleBackColor = (true);
            // 
            // groupBox25
            // 
            this.groupBox25.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox25.Controls.Add(this.cbDefMovieIncludeNoAirdate);
            this.groupBox25.Controls.Add(this.cbDefMovieIncludeFuture);
            this.groupBox25.Controls.Add(this.cbDefMovieDoMissing);
            this.groupBox25.Controls.Add(this.cbDefMovieDoRenaming);
            this.groupBox25.Location = (new global::System.Drawing.Point(7, 230));
            this.groupBox25.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox25.Name = ("groupBox25");
            this.groupBox25.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox25.Size = (new global::System.Drawing.Size(456, 134));
            this.groupBox25.TabIndex = (60);
            this.groupBox25.TabStop = (false);
            this.groupBox25.Text = ("Default Advanced Settings");
            // 
            // cbDefMovieIncludeNoAirdate
            // 
            this.cbDefMovieIncludeNoAirdate.AutoSize = (true);
            this.cbDefMovieIncludeNoAirdate.Location = (new global::System.Drawing.Point(29, 102));
            this.cbDefMovieIncludeNoAirdate.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDefMovieIncludeNoAirdate.Name = ("cbDefMovieIncludeNoAirdate");
            this.cbDefMovieIncludeNoAirdate.Size = (new global::System.Drawing.Size(270, 19));
            this.cbDefMovieIncludeNoAirdate.TabIndex = (53);
            this.cbDefMovieIncludeNoAirdate.Text = ("Include check when movie has no release date");
            this.cbDefMovieIncludeNoAirdate.UseVisualStyleBackColor = (true);
            // 
            // cbDefMovieIncludeFuture
            // 
            this.cbDefMovieIncludeFuture.AutoSize = (true);
            this.cbDefMovieIncludeFuture.Location = (new global::System.Drawing.Point(29, 75));
            this.cbDefMovieIncludeFuture.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDefMovieIncludeFuture.Name = ("cbDefMovieIncludeFuture");
            this.cbDefMovieIncludeFuture.Size = (new global::System.Drawing.Size(297, 19));
            this.cbDefMovieIncludeFuture.TabIndex = (54);
            this.cbDefMovieIncludeFuture.Text = ("Include check when movie has a future release date");
            this.cbDefMovieIncludeFuture.UseVisualStyleBackColor = (true);
            // 
            // cbDefMovieDoMissing
            // 
            this.cbDefMovieDoMissing.AutoSize = (true);
            this.cbDefMovieDoMissing.Location = (new global::System.Drawing.Point(7, 48));
            this.cbDefMovieDoMissing.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDefMovieDoMissing.Name = ("cbDefMovieDoMissing");
            this.cbDefMovieDoMissing.Size = (new global::System.Drawing.Size(119, 19));
            this.cbDefMovieDoMissing.TabIndex = (52);
            this.cbDefMovieDoMissing.Text = ("Do &missing check");
            this.cbDefMovieDoMissing.UseVisualStyleBackColor = (true);
            // 
            // cbDefMovieDoRenaming
            // 
            this.cbDefMovieDoRenaming.AutoSize = (true);
            this.cbDefMovieDoRenaming.Location = (new global::System.Drawing.Point(7, 22));
            this.cbDefMovieDoRenaming.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDefMovieDoRenaming.Name = ("cbDefMovieDoRenaming");
            this.cbDefMovieDoRenaming.Size = (new global::System.Drawing.Size(95, 19));
            this.cbDefMovieDoRenaming.TabIndex = (51);
            this.cbDefMovieDoRenaming.Text = ("Do &renaming");
            this.cbDefMovieDoRenaming.UseVisualStyleBackColor = (true);
            // 
            // pbMovieDefaults
            // 
            this.pbMovieDefaults.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.pbMovieDefaults.Cursor = (global::System.Windows.Forms.Cursors.Hand);
            this.pbMovieDefaults.Image = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbMovieDefaults.InitialImage = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbMovieDefaults.Location = (new global::System.Drawing.Point(418, 12));
            this.pbMovieDefaults.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.pbMovieDefaults.Name = ("pbMovieDefaults");
            this.pbMovieDefaults.Size = (new global::System.Drawing.Size(50, 46));
            this.pbMovieDefaults.SizeMode = (global::System.Windows.Forms.PictureBoxSizeMode.CenterImage);
            this.pbMovieDefaults.TabIndex = (62);
            this.pbMovieDefaults.TabStop = (false);
            this.pbMovieDefaults.Click += (this.pbMovieDefaults_Click);
            // 
            // tpShowDefaults
            // 
            this.tpShowDefaults.Controls.Add(this.label18);
            this.tpShowDefaults.Controls.Add(this.groupBox19);
            this.tpShowDefaults.Controls.Add(this.groupBox18);
            this.tpShowDefaults.Controls.Add(this.pictureBox1);
            this.tpShowDefaults.Location = (new global::System.Drawing.Point(149, 4));
            this.tpShowDefaults.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpShowDefaults.Name = ("tpShowDefaults");
            this.tpShowDefaults.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpShowDefaults.Size = (new global::System.Drawing.Size(500, 684));
            this.tpShowDefaults.TabIndex = (14);
            this.tpShowDefaults.Text = ("TV Show Defaults");
            this.tpShowDefaults.UseVisualStyleBackColor = (true);
            // 
            // label18
            // 
            this.label18.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.label18.AutoSize = (true);
            this.label18.Location = (new global::System.Drawing.Point(8, 8));
            this.label18.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label18.Name = ("label18");
            this.label18.Size = (new global::System.Drawing.Size(354, 45));
            this.label18.TabIndex = (59);
            this.label18.Text = ("These settings control the defaults used to add a new show to the \r\nsystem. Once the show has been created, you will need to modify\r\nits configuration directly.");
            // 
            // groupBox19
            // 
            this.groupBox19.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox19.Controls.Add(this.label24);
            this.groupBox19.Controls.Add(this.cbTimeZone);
            this.groupBox19.Controls.Add(this.rbDefShowUseSubFolders);
            this.groupBox19.Controls.Add(this.rbDefShowUseBase);
            this.groupBox19.Controls.Add(this.label12);
            this.groupBox19.Controls.Add(this.cmbDefShowLocation);
            this.groupBox19.Controls.Add(this.cbDefShowUseDefLocation);
            this.groupBox19.Controls.Add(this.cbDefShowAutoFolders);
            this.groupBox19.Location = (new global::System.Drawing.Point(7, 63));
            this.groupBox19.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox19.Name = ("groupBox19");
            this.groupBox19.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox19.Size = (new global::System.Drawing.Size(456, 203));
            this.groupBox19.TabIndex = (57);
            this.groupBox19.TabStop = (false);
            this.groupBox19.Text = ("Default TV Show Settings");
            // 
            // label24
            // 
            this.label24.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Bottom) | (global::System.Windows.Forms.AnchorStyles.Left)));
            this.label24.AutoSize = (true);
            this.label24.Location = (new global::System.Drawing.Point(10, 172));
            this.label24.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label24.Name = ("label24");
            this.label24.Size = (new global::System.Drawing.Size(97, 15));
            this.label24.TabIndex = (6);
            this.label24.Text = ("Airs in &Timezone:");
            this.label24.TextAlign = (global::System.Drawing.ContentAlignment.TopRight);
            // 
            // cbTimeZone
            // 
            this.cbTimeZone.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.cbTimeZone.DropDownStyle = (global::System.Windows.Forms.ComboBoxStyle.DropDownList);
            this.cbTimeZone.FormattingEnabled = (true);
            this.cbTimeZone.Location = (new global::System.Drawing.Point(119, 163));
            this.cbTimeZone.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbTimeZone.Name = ("cbTimeZone");
            this.cbTimeZone.Size = (new global::System.Drawing.Size(330, 23));
            this.cbTimeZone.TabIndex = (7);
            // 
            // rbDefShowUseSubFolders
            // 
            this.rbDefShowUseSubFolders.AutoSize = (true);
            this.rbDefShowUseSubFolders.Location = (new global::System.Drawing.Point(204, 130));
            this.rbDefShowUseSubFolders.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rbDefShowUseSubFolders.Name = ("rbDefShowUseSubFolders");
            this.rbDefShowUseSubFolders.Size = (new global::System.Drawing.Size(112, 19));
            this.rbDefShowUseSubFolders.TabIndex = (5);
            this.rbDefShowUseSubFolders.TabStop = (true);
            this.rbDefShowUseSubFolders.Text = ("in subdirectories");
            this.rbDefShowUseSubFolders.UseVisualStyleBackColor = (true);
            // 
            // rbDefShowUseBase
            // 
            this.rbDefShowUseBase.AutoSize = (true);
            this.rbDefShowUseBase.Location = (new global::System.Drawing.Point(34, 130));
            this.rbDefShowUseBase.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rbDefShowUseBase.Name = ("rbDefShowUseBase");
            this.rbDefShowUseBase.Size = (new global::System.Drawing.Size(96, 19));
            this.rbDefShowUseBase.TabIndex = (4);
            this.rbDefShowUseBase.TabStop = (true);
            this.rbDefShowUseBase.Text = ("in base folder");
            this.rbDefShowUseBase.UseVisualStyleBackColor = (true);
            // 
            // label12
            // 
            this.label12.AutoSize = (true);
            this.label12.Location = (new global::System.Drawing.Point(10, 111));
            this.label12.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label12.Name = ("label12");
            this.label12.Size = (new global::System.Drawing.Size(133, 15));
            this.label12.TabIndex = (3);
            this.label12.Text = ("Default season location:");
            // 
            // cmbDefShowLocation
            // 
            this.cmbDefShowLocation.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDefShowLocation.FormattingEnabled = (true);
            this.cmbDefShowLocation.Location = (new global::System.Drawing.Point(34, 78));
            this.cmbDefShowLocation.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cmbDefShowLocation.Name = ("cmbDefShowLocation");
            this.cmbDefShowLocation.Size = (new global::System.Drawing.Size(415, 23));
            this.cmbDefShowLocation.TabIndex = (2);
            // 
            // cbDefShowUseDefLocation
            // 
            this.cbDefShowUseDefLocation.AutoSize = (true);
            this.cbDefShowUseDefLocation.Location = (new global::System.Drawing.Point(10, 50));
            this.cbDefShowUseDefLocation.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDefShowUseDefLocation.Name = ("cbDefShowUseDefLocation");
            this.cbDefShowUseDefLocation.Size = (new global::System.Drawing.Size(135, 19));
            this.cbDefShowUseDefLocation.TabIndex = (1);
            this.cbDefShowUseDefLocation.Text = ("Use Default Location");
            this.cbDefShowUseDefLocation.UseVisualStyleBackColor = (true);
            this.cbDefShowUseDefLocation.CheckedChanged += (this.CbDefShowUseDefLocation_CheckedChanged);
            // 
            // cbDefShowAutoFolders
            // 
            this.cbDefShowAutoFolders.AutoSize = (true);
            this.cbDefShowAutoFolders.Location = (new global::System.Drawing.Point(10, 23));
            this.cbDefShowAutoFolders.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDefShowAutoFolders.Name = ("cbDefShowAutoFolders");
            this.cbDefShowAutoFolders.Size = (new global::System.Drawing.Size(185, 19));
            this.cbDefShowAutoFolders.TabIndex = (0);
            this.cbDefShowAutoFolders.Text = ("Use Automatic Season Folders");
            this.cbDefShowAutoFolders.UseVisualStyleBackColor = (true);
            // 
            // groupBox18
            // 
            this.groupBox18.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox18.Controls.Add(this.cbDefShowAlternateOrder);
            this.groupBox18.Controls.Add(this.cbDefShowEpNameMatching);
            this.groupBox18.Controls.Add(this.label68);
            this.groupBox18.Controls.Add(this.cbDefShowAirdateMatching);
            this.groupBox18.Controls.Add(this.cbDefShowSpecialsCount);
            this.groupBox18.Controls.Add(this.cbDefShowSequentialMatching);
            this.groupBox18.Controls.Add(this.cbDefShowIncludeNoAirdate);
            this.groupBox18.Controls.Add(this.cbDefShowDoMissingCheck);
            this.groupBox18.Controls.Add(this.cbDefShowDVDOrder);
            this.groupBox18.Controls.Add(this.cbDefShowIncludeFuture);
            this.groupBox18.Controls.Add(this.cbDefShowDoRenaming);
            this.groupBox18.Controls.Add(this.cbDefShowNextAirdate);
            this.groupBox18.Location = (new global::System.Drawing.Point(7, 273));
            this.groupBox18.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox18.Name = ("groupBox18");
            this.groupBox18.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox18.Size = (new global::System.Drawing.Size(456, 298));
            this.groupBox18.TabIndex = (56);
            this.groupBox18.TabStop = (false);
            this.groupBox18.Text = ("Default Advanced Settings");
            // 
            // cbDefShowAlternateOrder
            // 
            this.cbDefShowAlternateOrder.AutoSize = (true);
            this.cbDefShowAlternateOrder.Location = (new global::System.Drawing.Point(134, 32));
            this.cbDefShowAlternateOrder.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDefShowAlternateOrder.Name = ("cbDefShowAlternateOrder");
            this.cbDefShowAlternateOrder.Size = (new global::System.Drawing.Size(129, 19));
            this.cbDefShowAlternateOrder.TabIndex = (59);
            this.cbDefShowAlternateOrder.Text = ("Use Alternate Order");
            this.cbDefShowAlternateOrder.UseVisualStyleBackColor = (true);
            this.cbDefShowAlternateOrder.CheckedChanged += (this.cbDefShowAlternateOrder_CheckedChanged);
            // 
            // cbDefShowEpNameMatching
            // 
            this.cbDefShowEpNameMatching.AutoSize = (true);
            this.cbDefShowEpNameMatching.Location = (new global::System.Drawing.Point(34, 267));
            this.cbDefShowEpNameMatching.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDefShowEpNameMatching.Name = ("cbDefShowEpNameMatching");
            this.cbDefShowEpNameMatching.Size = (new global::System.Drawing.Size(204, 19));
            this.cbDefShowEpNameMatching.TabIndex = (58);
            this.cbDefShowEpNameMatching.Text = ("Look for episode title in filenames");
            this.cbDefShowEpNameMatching.UseVisualStyleBackColor = (true);
            // 
            // label68
            // 
            this.label68.AutoSize = (true);
            this.label68.Location = (new global::System.Drawing.Point(10, 190));
            this.label68.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label68.Name = ("label68");
            this.label68.Size = (new global::System.Drawing.Size(206, 15));
            this.label68.TabIndex = (57);
            this.label68.Text = ("When finding missing episodes (only)");
            // 
            // cbDefShowAirdateMatching
            // 
            this.cbDefShowAirdateMatching.AutoSize = (true);
            this.cbDefShowAirdateMatching.Location = (new global::System.Drawing.Point(34, 240));
            this.cbDefShowAirdateMatching.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDefShowAirdateMatching.Name = ("cbDefShowAirdateMatching");
            this.cbDefShowAirdateMatching.Size = (new global::System.Drawing.Size(176, 19));
            this.cbDefShowAirdateMatching.TabIndex = (56);
            this.cbDefShowAirdateMatching.Text = ("&Look for airdate in filenames");
            this.cbDefShowAirdateMatching.UseVisualStyleBackColor = (true);
            // 
            // cbDefShowSpecialsCount
            // 
            this.cbDefShowSpecialsCount.AutoSize = (true);
            this.cbDefShowSpecialsCount.Location = (new global::System.Drawing.Point(10, 85));
            this.cbDefShowSpecialsCount.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDefShowSpecialsCount.Name = ("cbDefShowSpecialsCount");
            this.cbDefShowSpecialsCount.Size = (new global::System.Drawing.Size(165, 19));
            this.cbDefShowSpecialsCount.TabIndex = (50);
            this.cbDefShowSpecialsCount.Text = ("S&pecials count as episodes");
            this.cbDefShowSpecialsCount.UseVisualStyleBackColor = (true);
            // 
            // cbDefShowSequentialMatching
            // 
            this.cbDefShowSequentialMatching.AutoSize = (true);
            this.cbDefShowSequentialMatching.Location = (new global::System.Drawing.Point(34, 213));
            this.cbDefShowSequentialMatching.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDefShowSequentialMatching.Name = ("cbDefShowSequentialMatching");
            this.cbDefShowSequentialMatching.Size = (new global::System.Drawing.Size(201, 19));
            this.cbDefShowSequentialMatching.TabIndex = (53);
            this.cbDefShowSequentialMatching.Text = ("Use sequential number matching");
            this.cbDefShowSequentialMatching.UseVisualStyleBackColor = (true);
            // 
            // cbDefShowIncludeNoAirdate
            // 
            this.cbDefShowIncludeNoAirdate.AutoSize = (true);
            this.cbDefShowIncludeNoAirdate.Location = (new global::System.Drawing.Point(204, 165));
            this.cbDefShowIncludeNoAirdate.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDefShowIncludeNoAirdate.Name = ("cbDefShowIncludeNoAirdate");
            this.cbDefShowIncludeNoAirdate.Size = (new global::System.Drawing.Size(121, 19));
            this.cbDefShowIncludeNoAirdate.TabIndex = (54);
            this.cbDefShowIncludeNoAirdate.Text = ("Include no airdate");
            this.cbDefShowIncludeNoAirdate.UseVisualStyleBackColor = (true);
            // 
            // cbDefShowDoMissingCheck
            // 
            this.cbDefShowDoMissingCheck.AutoSize = (true);
            this.cbDefShowDoMissingCheck.Location = (new global::System.Drawing.Point(10, 138));
            this.cbDefShowDoMissingCheck.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDefShowDoMissingCheck.Name = ("cbDefShowDoMissingCheck");
            this.cbDefShowDoMissingCheck.Size = (new global::System.Drawing.Size(119, 19));
            this.cbDefShowDoMissingCheck.TabIndex = (52);
            this.cbDefShowDoMissingCheck.Text = ("Do &missing check");
            this.cbDefShowDoMissingCheck.UseVisualStyleBackColor = (true);
            // 
            // cbDefShowDVDOrder
            // 
            this.cbDefShowDVDOrder.AutoSize = (true);
            this.cbDefShowDVDOrder.Location = (new global::System.Drawing.Point(10, 32));
            this.cbDefShowDVDOrder.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDefShowDVDOrder.Name = ("cbDefShowDVDOrder");
            this.cbDefShowDVDOrder.Size = (new global::System.Drawing.Size(104, 19));
            this.cbDefShowDVDOrder.TabIndex = (48);
            this.cbDefShowDVDOrder.Text = ("&Use DVD Order");
            this.cbDefShowDVDOrder.UseVisualStyleBackColor = (true);
            this.cbDefShowDVDOrder.CheckedChanged += (this.cbDefShowDVDOrder_CheckedChanged);
            // 
            // cbDefShowIncludeFuture
            // 
            this.cbDefShowIncludeFuture.AutoSize = (true);
            this.cbDefShowIncludeFuture.Location = (new global::System.Drawing.Point(34, 165));
            this.cbDefShowIncludeFuture.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDefShowIncludeFuture.Name = ("cbDefShowIncludeFuture");
            this.cbDefShowIncludeFuture.Size = (new global::System.Drawing.Size(149, 19));
            this.cbDefShowIncludeFuture.TabIndex = (55);
            this.cbDefShowIncludeFuture.Text = ("Include future episodes");
            this.cbDefShowIncludeFuture.UseVisualStyleBackColor = (true);
            // 
            // cbDefShowDoRenaming
            // 
            this.cbDefShowDoRenaming.AutoSize = (true);
            this.cbDefShowDoRenaming.Location = (new global::System.Drawing.Point(10, 112));
            this.cbDefShowDoRenaming.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDefShowDoRenaming.Name = ("cbDefShowDoRenaming");
            this.cbDefShowDoRenaming.Size = (new global::System.Drawing.Size(95, 19));
            this.cbDefShowDoRenaming.TabIndex = (51);
            this.cbDefShowDoRenaming.Text = ("Do &renaming");
            this.cbDefShowDoRenaming.UseVisualStyleBackColor = (true);
            // 
            // cbDefShowNextAirdate
            // 
            this.cbDefShowNextAirdate.AutoSize = (true);
            this.cbDefShowNextAirdate.Location = (new global::System.Drawing.Point(10, 59));
            this.cbDefShowNextAirdate.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDefShowNextAirdate.Name = ("cbDefShowNextAirdate");
            this.cbDefShowNextAirdate.Size = (new global::System.Drawing.Size(190, 19));
            this.cbDefShowNextAirdate.TabIndex = (49);
            this.cbDefShowNextAirdate.Text = ("Show &next airdate in 'Schedule'");
            this.cbDefShowNextAirdate.UseVisualStyleBackColor = (true);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Cursor = (global::System.Windows.Forms.Cursors.Hand);
            this.pictureBox1.Image = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pictureBox1.InitialImage = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pictureBox1.Location = (new global::System.Drawing.Point(418, 7));
            this.pictureBox1.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.pictureBox1.Name = ("pictureBox1");
            this.pictureBox1.Size = (new global::System.Drawing.Size(50, 46));
            this.pictureBox1.SizeMode = (global::System.Windows.Forms.PictureBoxSizeMode.CenterImage);
            this.pictureBox1.TabIndex = (58);
            this.pictureBox1.TabStop = (false);
            this.pictureBox1.Click += (this.PictureBox1_Click_1);
            // 
            // tpScanSettings
            // 
            this.tpScanSettings.Controls.Add(this.chkGroupMissingEpisodesIntoSeasons);
            this.tpScanSettings.Controls.Add(this.groupBox17);
            this.tpScanSettings.Controls.Add(this.groupBox1);
            this.tpScanSettings.Controls.Add(this.cbScanIncludesBulkAdd);
            this.tpScanSettings.Controls.Add(this.gbBulkAdd);
            this.tpScanSettings.Controls.Add(this.label62);
            this.tpScanSettings.Controls.Add(this.pbScanOptions);
            this.tpScanSettings.Location = (new global::System.Drawing.Point(149, 4));
            this.tpScanSettings.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpScanSettings.Name = ("tpScanSettings");
            this.tpScanSettings.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpScanSettings.Size = (new global::System.Drawing.Size(500, 684));
            this.tpScanSettings.TabIndex = (16);
            this.tpScanSettings.Text = ("Scan Settings");
            this.tpScanSettings.UseVisualStyleBackColor = (true);
            // 
            // chkGroupMissingEpisodesIntoSeasons
            // 
            this.chkGroupMissingEpisodesIntoSeasons.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.chkGroupMissingEpisodesIntoSeasons.AutoSize = (true);
            this.chkGroupMissingEpisodesIntoSeasons.Location = (new global::System.Drawing.Point(8, 647));
            this.chkGroupMissingEpisodesIntoSeasons.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkGroupMissingEpisodesIntoSeasons.Name = ("chkGroupMissingEpisodesIntoSeasons");
            this.chkGroupMissingEpisodesIntoSeasons.Size = (new global::System.Drawing.Size(244, 19));
            this.chkGroupMissingEpisodesIntoSeasons.TabIndex = (50);
            this.chkGroupMissingEpisodesIntoSeasons.Text = ("Group Entire Seasons of Missing Episodes");
            this.chkGroupMissingEpisodesIntoSeasons.UseVisualStyleBackColor = (true);
            // 
            // groupBox17
            // 
            this.groupBox17.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox17.Controls.Add(this.cbIgnorePreviouslySeenMovies);
            this.groupBox17.Controls.Add(this.chkIncludeMoviesQuickRecent);
            this.groupBox17.Controls.Add(this.chkIgnoreAllSpecials);
            this.groupBox17.Controls.Add(this.chkMoveLibraryFiles);
            this.groupBox17.Controls.Add(this.label1);
            this.groupBox17.Controls.Add(this.upDownScanHours);
            this.groupBox17.Controls.Add(this.chkScheduledScan);
            this.groupBox17.Controls.Add(this.chkScanOnStartup);
            this.groupBox17.Controls.Add(this.lblScanAction);
            this.groupBox17.Controls.Add(this.rdoQuickScan);
            this.groupBox17.Controls.Add(this.rdoRecentScan);
            this.groupBox17.Controls.Add(this.rdoFullScan);
            this.groupBox17.Controls.Add(this.cbIgnorePreviouslySeen);
            this.groupBox17.Controls.Add(this.chkPreventMove);
            this.groupBox17.Controls.Add(this.label28);
            this.groupBox17.Controls.Add(this.cbRenameCheck);
            this.groupBox17.Controls.Add(this.cbMissing);
            this.groupBox17.Location = (new global::System.Drawing.Point(8, 57));
            this.groupBox17.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox17.Name = ("groupBox17");
            this.groupBox17.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox17.Size = (new global::System.Drawing.Size(461, 284));
            this.groupBox17.TabIndex = (49);
            this.groupBox17.TabStop = (false);
            this.groupBox17.Text = ("Scan Options");
            // 
            // cbIgnorePreviouslySeenMovies
            // 
            this.cbIgnorePreviouslySeenMovies.AutoSize = (true);
            this.cbIgnorePreviouslySeenMovies.Location = (new global::System.Drawing.Point(248, 216));
            this.cbIgnorePreviouslySeenMovies.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbIgnorePreviouslySeenMovies.Name = ("cbIgnorePreviouslySeenMovies");
            this.cbIgnorePreviouslySeenMovies.Size = (new global::System.Drawing.Size(186, 19));
            this.cbIgnorePreviouslySeenMovies.TabIndex = (51);
            this.cbIgnorePreviouslySeenMovies.Text = ("Ignore Movies Previously Seen");
            this.cbIgnorePreviouslySeenMovies.UseVisualStyleBackColor = (true);
            // 
            // chkMoveLibraryFiles
            // 
            this.chkMoveLibraryFiles.AutoSize = (true);
            this.chkMoveLibraryFiles.Checked = (true);
            this.chkMoveLibraryFiles.CheckState = (global::System.Windows.Forms.CheckState.Checked);
            this.chkMoveLibraryFiles.Location = (new global::System.Drawing.Point(12, 264));
            this.chkMoveLibraryFiles.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkMoveLibraryFiles.Name = ("chkMoveLibraryFiles");
            this.chkMoveLibraryFiles.Size = (new global::System.Drawing.Size(235, 19));
            this.chkMoveLibraryFiles.TabIndex = (48);
            this.chkMoveLibraryFiles.Text = ("Move Files within Library to Keep it Tidy");
            this.chkMoveLibraryFiles.UseVisualStyleBackColor = (true);
            // 
            // lblScanAction
            // 
            this.lblScanAction.AutoSize = (true);
            this.lblScanAction.Location = (new global::System.Drawing.Point(10, 22));
            this.lblScanAction.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.lblScanAction.Name = ("lblScanAction");
            this.lblScanAction.Size = (new global::System.Drawing.Size(129, 15));
            this.lblScanAction.TabIndex = (43);
            this.lblScanAction.Text = ("Default Auto &Scan Type");
            // 
            // rdoQuickScan
            // 
            this.rdoQuickScan.AutoSize = (true);
            this.rdoQuickScan.Location = (new global::System.Drawing.Point(164, 42));
            this.rdoQuickScan.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rdoQuickScan.Name = ("rdoQuickScan");
            this.rdoQuickScan.Size = (new global::System.Drawing.Size(56, 19));
            this.rdoQuickScan.TabIndex = (42);
            this.rdoQuickScan.TabStop = (true);
            this.rdoQuickScan.Text = ("&Quick");
            this.rdoQuickScan.UseVisualStyleBackColor = (true);
            // 
            // rdoRecentScan
            // 
            this.rdoRecentScan.AutoSize = (true);
            this.rdoRecentScan.Location = (new global::System.Drawing.Point(88, 42));
            this.rdoRecentScan.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rdoRecentScan.Name = ("rdoRecentScan");
            this.rdoRecentScan.Size = (new global::System.Drawing.Size(61, 19));
            this.rdoRecentScan.TabIndex = (41);
            this.rdoRecentScan.TabStop = (true);
            this.rdoRecentScan.Text = ("&Recent");
            this.rdoRecentScan.UseVisualStyleBackColor = (true);
            // 
            // rdoFullScan
            // 
            this.rdoFullScan.AutoSize = (true);
            this.rdoFullScan.Location = (new global::System.Drawing.Point(33, 42));
            this.rdoFullScan.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rdoFullScan.Name = ("rdoFullScan");
            this.rdoFullScan.Size = (new global::System.Drawing.Size(44, 19));
            this.rdoFullScan.TabIndex = (40);
            this.rdoFullScan.TabStop = (true);
            this.rdoFullScan.Text = ("&Full");
            this.rdoFullScan.UseVisualStyleBackColor = (true);
            // 
            // cbIgnorePreviouslySeen
            // 
            this.cbIgnorePreviouslySeen.AutoSize = (true);
            this.cbIgnorePreviouslySeen.Location = (new global::System.Drawing.Point(35, 216));
            this.cbIgnorePreviouslySeen.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbIgnorePreviouslySeen.Name = ("cbIgnorePreviouslySeen");
            this.cbIgnorePreviouslySeen.Size = (new global::System.Drawing.Size(194, 19));
            this.cbIgnorePreviouslySeen.TabIndex = (39);
            this.cbIgnorePreviouslySeen.Text = ("Ignore Episodes Previously Seen");
            this.cbIgnorePreviouslySeen.UseVisualStyleBackColor = (true);
            // 
            // chkPreventMove
            // 
            this.chkPreventMove.AutoSize = (true);
            this.chkPreventMove.Location = (new global::System.Drawing.Point(35, 167));
            this.chkPreventMove.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkPreventMove.Name = ("chkPreventMove");
            this.chkPreventMove.Size = (new global::System.Drawing.Size(210, 19));
            this.chkPreventMove.TabIndex = (38);
            this.chkPreventMove.Text = ("Pre&vent move of files (just rename)");
            this.chkPreventMove.UseVisualStyleBackColor = (true);
            // 
            // label28
            // 
            this.label28.AutoSize = (true);
            this.label28.Location = (new global::System.Drawing.Point(10, 122));
            this.label28.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label28.Name = ("label28");
            this.label28.Size = (new global::System.Drawing.Size(148, 15));
            this.label28.TabIndex = (35);
            this.label28.Text = ("\"Scan\" checks and actions:");
            // 
            // cbRenameCheck
            // 
            this.cbRenameCheck.AutoSize = (true);
            this.cbRenameCheck.Checked = (true);
            this.cbRenameCheck.CheckState = (global::System.Windows.Forms.CheckState.Checked);
            this.cbRenameCheck.Location = (new global::System.Drawing.Point(12, 141));
            this.cbRenameCheck.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbRenameCheck.Name = ("cbRenameCheck");
            this.cbRenameCheck.Size = (new global::System.Drawing.Size(105, 19));
            this.cbRenameCheck.TabIndex = (36);
            this.cbRenameCheck.Text = ("&Rename Check");
            this.cbRenameCheck.UseVisualStyleBackColor = (true);
            // 
            // cbMissing
            // 
            this.cbMissing.AutoSize = (true);
            this.cbMissing.Checked = (true);
            this.cbMissing.CheckState = (global::System.Windows.Forms.CheckState.Checked);
            this.cbMissing.Location = (new global::System.Drawing.Point(12, 194));
            this.cbMissing.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbMissing.Name = ("cbMissing");
            this.cbMissing.Size = (new global::System.Drawing.Size(103, 19));
            this.cbMissing.TabIndex = (37);
            this.cbMissing.Text = ("&Missing Check");
            this.cbMissing.UseVisualStyleBackColor = (true);
            this.cbMissing.CheckedChanged += (this.EnableDisable);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chkChooseWhenMultipleEpisodesMatch);
            this.groupBox1.Controls.Add(this.cbxUpdateAirDate);
            this.groupBox1.Controls.Add(this.cbAutoCreateFolders);
            this.groupBox1.Controls.Add(this.chkAutoMergeLibraryEpisodes);
            this.groupBox1.Location = (new global::System.Drawing.Point(7, 347));
            this.groupBox1.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox1.Name = ("groupBox1");
            this.groupBox1.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox1.Size = (new global::System.Drawing.Size(460, 133));
            this.groupBox1.TabIndex = (48);
            this.groupBox1.TabStop = (false);
            this.groupBox1.Text = ("Additional Scan Options");
            // 
            // chkChooseWhenMultipleEpisodesMatch
            // 
            this.chkChooseWhenMultipleEpisodesMatch.AutoSize = (true);
            this.chkChooseWhenMultipleEpisodesMatch.Location = (new global::System.Drawing.Point(7, 102));
            this.chkChooseWhenMultipleEpisodesMatch.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkChooseWhenMultipleEpisodesMatch.Name = ("chkChooseWhenMultipleEpisodesMatch");
            this.chkChooseWhenMultipleEpisodesMatch.Size = (new global::System.Drawing.Size(328, 19));
            this.chkChooseWhenMultipleEpisodesMatch.TabIndex = (42);
            this.chkChooseWhenMultipleEpisodesMatch.Text = ("Choose between episodes in library when multiple match");
            this.chkChooseWhenMultipleEpisodesMatch.UseVisualStyleBackColor = (true);
            // 
            // cbxUpdateAirDate
            // 
            this.cbxUpdateAirDate.AutoSize = (true);
            this.cbxUpdateAirDate.Location = (new global::System.Drawing.Point(7, 22));
            this.cbxUpdateAirDate.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbxUpdateAirDate.Name = ("cbxUpdateAirDate");
            this.cbxUpdateAirDate.Size = (new global::System.Drawing.Size(218, 19));
            this.cbxUpdateAirDate.TabIndex = (39);
            this.cbxUpdateAirDate.Text = ("Update files and folders with air date");
            this.cbxUpdateAirDate.UseVisualStyleBackColor = (true);
            // 
            // cbAutoCreateFolders
            // 
            this.cbAutoCreateFolders.AutoSize = (true);
            this.cbAutoCreateFolders.Location = (new global::System.Drawing.Point(7, 75));
            this.cbAutoCreateFolders.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbAutoCreateFolders.Name = ("cbAutoCreateFolders");
            this.cbAutoCreateFolders.Size = (new global::System.Drawing.Size(218, 19));
            this.cbAutoCreateFolders.TabIndex = (37);
            this.cbAutoCreateFolders.Text = ("&Automatically create missing folders");
            this.cbAutoCreateFolders.UseVisualStyleBackColor = (true);
            // 
            // chkAutoMergeLibraryEpisodes
            // 
            this.chkAutoMergeLibraryEpisodes.AutoSize = (true);
            this.chkAutoMergeLibraryEpisodes.Location = (new global::System.Drawing.Point(7, 48));
            this.chkAutoMergeLibraryEpisodes.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkAutoMergeLibraryEpisodes.Name = ("chkAutoMergeLibraryEpisodes");
            this.chkAutoMergeLibraryEpisodes.Size = (new global::System.Drawing.Size(347, 19));
            this.chkAutoMergeLibraryEpisodes.TabIndex = (41);
            this.chkAutoMergeLibraryEpisodes.Text = ("Automatically create merge rules for merged library episodes");
            this.chkAutoMergeLibraryEpisodes.UseVisualStyleBackColor = (true);
            // 
            // cbScanIncludesBulkAdd
            // 
            this.cbScanIncludesBulkAdd.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.cbScanIncludesBulkAdd.AutoSize = (true);
            this.cbScanIncludesBulkAdd.Location = (new global::System.Drawing.Point(8, 487));
            this.cbScanIncludesBulkAdd.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbScanIncludesBulkAdd.Name = ("cbScanIncludesBulkAdd");
            this.cbScanIncludesBulkAdd.Size = (new global::System.Drawing.Size(173, 19));
            this.cbScanIncludesBulkAdd.TabIndex = (47);
            this.cbScanIncludesBulkAdd.Text = ("Do Bulk-Add as part of scan");
            this.cbScanIncludesBulkAdd.UseVisualStyleBackColor = (true);
            // 
            // gbBulkAdd
            // 
            this.gbBulkAdd.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.gbBulkAdd.Controls.Add(this.tbSeasonSearchTerms);
            this.gbBulkAdd.Controls.Add(this.label36);
            this.gbBulkAdd.Controls.Add(this.chkForceBulkAddToUseSettingsOnly);
            this.gbBulkAdd.Controls.Add(this.cbIgnoreRecycleBin);
            this.gbBulkAdd.Controls.Add(this.cbIgnoreNoVideoFolders);
            this.gbBulkAdd.Location = (new global::System.Drawing.Point(8, 513));
            this.gbBulkAdd.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.gbBulkAdd.Name = ("gbBulkAdd");
            this.gbBulkAdd.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.gbBulkAdd.Size = (new global::System.Drawing.Size(460, 128));
            this.gbBulkAdd.TabIndex = (46);
            this.gbBulkAdd.TabStop = (false);
            this.gbBulkAdd.Text = ("Bulk Add TV Shows from Library Folders");
            // 
            // label36
            // 
            this.label36.AutoSize = (true);
            this.label36.Location = (new global::System.Drawing.Point(7, 102));
            this.label36.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label36.Name = ("label36");
            this.label36.Size = (new global::System.Drawing.Size(117, 15));
            this.label36.TabIndex = (21);
            this.label36.Text = ("Season search terms:");
            // 
            // label62
            // 
            this.label62.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.label62.AutoSize = (true);
            this.label62.Location = (new global::System.Drawing.Point(7, 3));
            this.label62.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label62.Name = ("label62");
            this.label62.Size = (new global::System.Drawing.Size(322, 30));
            this.label62.TabIndex = (40);
            this.label62.Text = ("General settings to control TV Rename's scan and download\r\nbehaviour");
            // 
            // pbScanOptions
            // 
            this.pbScanOptions.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.pbScanOptions.Cursor = (global::System.Windows.Forms.Cursors.Hand);
            this.pbScanOptions.Image = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbScanOptions.InitialImage = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbScanOptions.Location = (new global::System.Drawing.Point(418, 3));
            this.pbScanOptions.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.pbScanOptions.Name = ("pbScanOptions");
            this.pbScanOptions.Size = (new global::System.Drawing.Size(50, 46));
            this.pbScanOptions.SizeMode = (global::System.Windows.Forms.PictureBoxSizeMode.CenterImage);
            this.pbScanOptions.TabIndex = (39);
            this.pbScanOptions.TabStop = (false);
            this.pbScanOptions.Click += (this.pbScanOptions_Click);
            // 
            // tpSubtitles
            // 
            this.tpSubtitles.Controls.Add(this.groupBox29);
            this.tpSubtitles.Controls.Add(this.label93);
            this.tpSubtitles.Controls.Add(this.pictureBox2);
            this.tpSubtitles.Controls.Add(this.groupBox9);
            this.tpSubtitles.Location = (new global::System.Drawing.Point(149, 4));
            this.tpSubtitles.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpSubtitles.Name = ("tpSubtitles");
            this.tpSubtitles.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpSubtitles.Size = (new global::System.Drawing.Size(500, 684));
            this.tpSubtitles.TabIndex = (21);
            this.tpSubtitles.Text = ("Subtitles");
            this.tpSubtitles.UseVisualStyleBackColor = (true);
            // 
            // groupBox29
            // 
            this.groupBox29.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox29.Controls.Add(this.label94);
            this.groupBox29.Controls.Add(this.txtSubtitleFolderNames);
            this.groupBox29.Controls.Add(this.cbCopySubsFolders);
            this.groupBox29.Location = (new global::System.Drawing.Point(7, 185));
            this.groupBox29.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox29.Name = ("groupBox29");
            this.groupBox29.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox29.Size = (new global::System.Drawing.Size(464, 90));
            this.groupBox29.TabIndex = (44);
            this.groupBox29.TabStop = (false);
            this.groupBox29.Text = ("Subtitle Folders");
            // 
            // label94
            // 
            this.label94.AutoSize = (true);
            this.label94.Location = (new global::System.Drawing.Point(4, 52));
            this.label94.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label94.Name = ("label94");
            this.label94.Size = (new global::System.Drawing.Size(126, 15));
            this.label94.TabIndex = (30);
            this.label94.Text = ("&Subtitle Folder Names:");
            // 
            // txtSubtitleFolderNames
            // 
            this.txtSubtitleFolderNames.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubtitleFolderNames.Location = (new global::System.Drawing.Point(142, 48));
            this.txtSubtitleFolderNames.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtSubtitleFolderNames.Name = ("txtSubtitleFolderNames");
            this.txtSubtitleFolderNames.Size = (new global::System.Drawing.Size(314, 23));
            this.txtSubtitleFolderNames.TabIndex = (31);
            // 
            // cbCopySubsFolders
            // 
            this.cbCopySubsFolders.AutoSize = (true);
            this.cbCopySubsFolders.Location = (new global::System.Drawing.Point(7, 22));
            this.cbCopySubsFolders.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbCopySubsFolders.Name = ("cbCopySubsFolders");
            this.cbCopySubsFolders.Size = (new global::System.Drawing.Size(135, 19));
            this.cbCopySubsFolders.TabIndex = (29);
            this.cbCopySubsFolders.Text = ("Copy subtitle folders");
            this.cbCopySubsFolders.UseVisualStyleBackColor = (true);
            // 
            // label93
            // 
            this.label93.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.label93.AutoSize = (true);
            this.label93.Location = (new global::System.Drawing.Point(4, 3));
            this.label93.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label93.Name = ("label93");
            this.label93.Size = (new global::System.Drawing.Size(320, 30));
            this.label93.TabIndex = (43);
            this.label93.Text = ("These preferences control how TV Rename finds and retains\r\nsubtitles and subtitle files.");
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.Cursor = (global::System.Windows.Forms.Cursors.Hand);
            this.pictureBox2.Image = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pictureBox2.InitialImage = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pictureBox2.Location = (new global::System.Drawing.Point(414, 2));
            this.pictureBox2.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.pictureBox2.Name = ("pictureBox2");
            this.pictureBox2.Size = (new global::System.Drawing.Size(50, 46));
            this.pictureBox2.SizeMode = (global::System.Windows.Forms.PictureBoxSizeMode.CenterImage);
            this.pictureBox2.TabIndex = (42);
            this.pictureBox2.TabStop = (false);
            // 
            // groupBox9
            // 
            this.groupBox9.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox9.Controls.Add(this.cbTxtToSub);
            this.groupBox9.Controls.Add(this.label46);
            this.groupBox9.Controls.Add(this.txtSubtitleExtensions);
            this.groupBox9.Controls.Add(this.chkRetainLanguageSpecificSubtitles);
            this.groupBox9.Location = (new global::System.Drawing.Point(7, 55));
            this.groupBox9.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox9.Name = ("groupBox9");
            this.groupBox9.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox9.Size = (new global::System.Drawing.Size(464, 122));
            this.groupBox9.TabIndex = (41);
            this.groupBox9.TabStop = (false);
            this.groupBox9.Text = ("Subtitles");
            // 
            // cbTxtToSub
            // 
            this.cbTxtToSub.AutoSize = (true);
            this.cbTxtToSub.Location = (new global::System.Drawing.Point(7, 48));
            this.cbTxtToSub.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbTxtToSub.Name = ("cbTxtToSub");
            this.cbTxtToSub.Size = (new global::System.Drawing.Size(128, 19));
            this.cbTxtToSub.TabIndex = (32);
            this.cbTxtToSub.Text = ("&Rename .txt to .sub");
            this.cbTxtToSub.UseVisualStyleBackColor = (true);
            // 
            // label46
            // 
            this.label46.AutoSize = (true);
            this.label46.Location = (new global::System.Drawing.Point(4, 81));
            this.label46.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label46.Name = ("label46");
            this.label46.Size = (new global::System.Drawing.Size(109, 15));
            this.label46.TabIndex = (30);
            this.label46.Text = ("&Subtitle extensions:");
            // 
            // txtSubtitleExtensions
            // 
            this.txtSubtitleExtensions.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubtitleExtensions.Location = (new global::System.Drawing.Point(125, 77));
            this.txtSubtitleExtensions.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtSubtitleExtensions.Name = ("txtSubtitleExtensions");
            this.txtSubtitleExtensions.Size = (new global::System.Drawing.Size(332, 23));
            this.txtSubtitleExtensions.TabIndex = (31);
            // 
            // chkRetainLanguageSpecificSubtitles
            // 
            this.chkRetainLanguageSpecificSubtitles.AutoSize = (true);
            this.chkRetainLanguageSpecificSubtitles.Location = (new global::System.Drawing.Point(7, 22));
            this.chkRetainLanguageSpecificSubtitles.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkRetainLanguageSpecificSubtitles.Name = ("chkRetainLanguageSpecificSubtitles");
            this.chkRetainLanguageSpecificSubtitles.Size = (new global::System.Drawing.Size(206, 19));
            this.chkRetainLanguageSpecificSubtitles.TabIndex = (29);
            this.chkRetainLanguageSpecificSubtitles.Text = ("Retain &Language Specific Subtitles");
            this.chkRetainLanguageSpecificSubtitles.UseVisualStyleBackColor = (true);
            // 
            // tpJackett
            // 
            this.tpJackett.Controls.Add(this.label99);
            this.tpJackett.Controls.Add(this.txtMinRSSSeeders);
            this.tpJackett.Controls.Add(this.label97);
            this.tpJackett.Controls.Add(this.tbUnwantedRSSTerms);
            this.tpJackett.Controls.Add(this.chkSearchJackettButton);
            this.tpJackett.Controls.Add(this.cmbSupervisedDuplicateAction);
            this.tpJackett.Controls.Add(this.label77);
            this.tpJackett.Controls.Add(this.cmbUnattendedDuplicateAction);
            this.tpJackett.Controls.Add(this.label76);
            this.tpJackett.Controls.Add(this.cbDetailedRSSJSONLogging);
            this.tpJackett.Controls.Add(this.pbuJackett);
            this.tpJackett.Controls.Add(this.label70);
            this.tpJackett.Controls.Add(this.cbSearchJackett);
            this.tpJackett.Controls.Add(this.groupBox22);
            this.tpJackett.Controls.Add(this.label45);
            this.tpJackett.Controls.Add(this.tbPreferredRSSTerms);
            this.tpJackett.Location = (new global::System.Drawing.Point(149, 4));
            this.tpJackett.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpJackett.Name = ("tpJackett");
            this.tpJackett.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpJackett.Size = (new global::System.Drawing.Size(500, 684));
            this.tpJackett.TabIndex = (17);
            this.tpJackett.Text = ("Jackett Search");
            this.tpJackett.UseVisualStyleBackColor = (true);
            // 
            // label99
            // 
            this.label99.AutoSize = (true);
            this.label99.Location = (new global::System.Drawing.Point(9, 175));
            this.label99.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label99.Name = ("label99");
            this.label99.Size = (new global::System.Drawing.Size(74, 15));
            this.label99.TabIndex = (51);
            this.label99.Text = ("Min Seeders:");
            // 
            // txtMinRSSSeeders
            // 
            this.txtMinRSSSeeders.Location = (new global::System.Drawing.Point(420, 171));
            this.txtMinRSSSeeders.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtMinRSSSeeders.Name = ("txtMinRSSSeeders");
            this.txtMinRSSSeeders.Size = (new global::System.Drawing.Size(32, 23));
            this.txtMinRSSSeeders.TabIndex = (52);
            this.txtMinRSSSeeders.TextChanged += (this.EnsureInteger);
            this.txtMinRSSSeeders.KeyPress += (this.TxtNumberOnlyKeyPress);
            // 
            // label97
            // 
            this.label97.AutoSize = (true);
            this.label97.Location = (new global::System.Drawing.Point(7, 147));
            this.label97.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label97.Name = ("label97");
            this.label97.Size = (new global::System.Drawing.Size(98, 15));
            this.label97.TabIndex = (50);
            this.label97.Text = ("Unwanted Terms:");
            // 
            // tbUnwantedRSSTerms
            // 
            this.tbUnwantedRSSTerms.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.tbUnwantedRSSTerms.Location = (new global::System.Drawing.Point(120, 143));
            this.tbUnwantedRSSTerms.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbUnwantedRSSTerms.Name = ("tbUnwantedRSSTerms");
            this.tbUnwantedRSSTerms.Size = (new global::System.Drawing.Size(332, 23));
            this.tbUnwantedRSSTerms.TabIndex = (49);
            // 
            // chkSearchJackettButton
            // 
            this.chkSearchJackettButton.AutoSize = (true);
            this.chkSearchJackettButton.Location = (new global::System.Drawing.Point(265, 282));
            this.chkSearchJackettButton.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkSearchJackettButton.Name = ("chkSearchJackettButton");
            this.chkSearchJackettButton.Size = (new global::System.Drawing.Size(177, 19));
            this.chkSearchJackettButton.TabIndex = (48);
            this.chkSearchJackettButton.Text = ("Show 'Search &Jackett' button");
            this.chkSearchJackettButton.UseVisualStyleBackColor = (true);
            // 
            // cmbSupervisedDuplicateAction
            // 
            this.cmbSupervisedDuplicateAction.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSupervisedDuplicateAction.DropDownStyle = (global::System.Windows.Forms.ComboBoxStyle.DropDownList);
            this.cmbSupervisedDuplicateAction.FormattingEnabled = (true);
            this.cmbSupervisedDuplicateAction.Items.AddRange(new global::System.Object[] { "Ask User", "Choose Largest File", "Choose Most Popular", "Download All", "Ignore", "Use First" });
            this.cmbSupervisedDuplicateAction.Location = (new global::System.Drawing.Point(190, 231));
            this.cmbSupervisedDuplicateAction.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cmbSupervisedDuplicateAction.Name = ("cmbSupervisedDuplicateAction");
            this.cmbSupervisedDuplicateAction.Size = (new global::System.Drawing.Size(262, 23));
            this.cmbSupervisedDuplicateAction.Sorted = (true);
            this.cmbSupervisedDuplicateAction.TabIndex = (47);
            // 
            // label77
            // 
            this.label77.AutoSize = (true);
            this.label77.Location = (new global::System.Drawing.Point(8, 234));
            this.label77.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label77.Name = ("label77");
            this.label77.Size = (new global::System.Drawing.Size(158, 15));
            this.label77.TabIndex = (46);
            this.label77.Text = ("Supervised Duplicate Action:");
            // 
            // cmbUnattendedDuplicateAction
            // 
            this.cmbUnattendedDuplicateAction.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.cmbUnattendedDuplicateAction.DropDownStyle = (global::System.Windows.Forms.ComboBoxStyle.DropDownList);
            this.cmbUnattendedDuplicateAction.FormattingEnabled = (true);
            this.cmbUnattendedDuplicateAction.Items.AddRange(new global::System.Object[] { "Ask User", "Choose Largest File", "Choose Most Popular", "Download All", "Ignore", "Use First" });
            this.cmbUnattendedDuplicateAction.Location = (new global::System.Drawing.Point(190, 200));
            this.cmbUnattendedDuplicateAction.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cmbUnattendedDuplicateAction.Name = ("cmbUnattendedDuplicateAction");
            this.cmbUnattendedDuplicateAction.Size = (new global::System.Drawing.Size(262, 23));
            this.cmbUnattendedDuplicateAction.Sorted = (true);
            this.cmbUnattendedDuplicateAction.TabIndex = (45);
            // 
            // label76
            // 
            this.label76.AutoSize = (true);
            this.label76.Location = (new global::System.Drawing.Point(8, 203));
            this.label76.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label76.Name = ("label76");
            this.label76.Size = (new global::System.Drawing.Size(163, 15));
            this.label76.TabIndex = (44);
            this.label76.Text = ("Unattended Duplicate Action:");
            // 
            // cbDetailedRSSJSONLogging
            // 
            this.cbDetailedRSSJSONLogging.AutoSize = (true);
            this.cbDetailedRSSJSONLogging.Location = (new global::System.Drawing.Point(9, 87));
            this.cbDetailedRSSJSONLogging.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbDetailedRSSJSONLogging.Name = ("cbDetailedRSSJSONLogging");
            this.cbDetailedRSSJSONLogging.Size = (new global::System.Drawing.Size(332, 19));
            this.cbDetailedRSSJSONLogging.TabIndex = (43);
            this.cbDetailedRSSJSONLogging.Text = ("Detailed logging (useful when setting up RSS/JSON Feeds)");
            this.cbDetailedRSSJSONLogging.UseVisualStyleBackColor = (false);
            // 
            // pbuJackett
            // 
            this.pbuJackett.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.pbuJackett.Cursor = (global::System.Windows.Forms.Cursors.Hand);
            this.pbuJackett.Image = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbuJackett.InitialImage = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbuJackett.Location = (new global::System.Drawing.Point(418, 14));
            this.pbuJackett.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.pbuJackett.Name = ("pbuJackett");
            this.pbuJackett.Size = (new global::System.Drawing.Size(50, 46));
            this.pbuJackett.SizeMode = (global::System.Windows.Forms.PictureBoxSizeMode.CenterImage);
            this.pbuJackett.TabIndex = (42);
            this.pbuJackett.TabStop = (false);
            this.pbuJackett.Click += (this.pbuJackett_Click);
            // 
            // label70
            // 
            this.label70.AutoSize = (true);
            this.label70.Location = (new global::System.Drawing.Point(5, 8));
            this.label70.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label70.Name = ("label70");
            this.label70.Size = (new global::System.Drawing.Size(352, 45));
            this.label70.TabIndex = (41);
            this.label70.Text = ("If an episode is missing from your library, TV Rename will talk to a\r\nrunning Jackett instance for appropriate files to download. It will \r\nuse the torrent handlers to download the file(s)");
            // 
            // cbSearchJackett
            // 
            this.cbSearchJackett.AutoSize = (true);
            this.cbSearchJackett.Location = (new global::System.Drawing.Point(9, 282));
            this.cbSearchJackett.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbSearchJackett.Name = ("cbSearchJackett");
            this.cbSearchJackett.Size = (new global::System.Drawing.Size(186, 19));
            this.cbSearchJackett.TabIndex = (40);
            this.cbSearchJackett.Text = ("Search &Jackett for missing files");
            this.cbSearchJackett.UseVisualStyleBackColor = (true);
            this.cbSearchJackett.CheckedChanged += (this.EnableDisable);
            // 
            // groupBox22
            // 
            this.groupBox22.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox22.Controls.Add(this.chkUseJackettTextSearch);
            this.groupBox22.Controls.Add(this.chkSkipJackettFullScans);
            this.groupBox22.Controls.Add(this.llJackettLink);
            this.groupBox22.Controls.Add(this.label71);
            this.groupBox22.Controls.Add(this.cbSearchJackettOnManualScansOnly);
            this.groupBox22.Controls.Add(this.label72);
            this.groupBox22.Controls.Add(this.txtJackettIndexer);
            this.groupBox22.Controls.Add(this.label73);
            this.groupBox22.Controls.Add(this.txtJackettAPIKey);
            this.groupBox22.Controls.Add(this.label74);
            this.groupBox22.Controls.Add(this.txtJackettPort);
            this.groupBox22.Controls.Add(this.label75);
            this.groupBox22.Controls.Add(this.txtJackettServer);
            this.groupBox22.Location = (new global::System.Drawing.Point(7, 308));
            this.groupBox22.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox22.Name = ("groupBox22");
            this.groupBox22.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox22.Size = (new global::System.Drawing.Size(464, 232));
            this.groupBox22.TabIndex = (39);
            this.groupBox22.TabStop = (false);
            this.groupBox22.Text = ("Jackett Search");
            // 
            // chkUseJackettTextSearch
            // 
            this.chkUseJackettTextSearch.AutoSize = (true);
            this.chkUseJackettTextSearch.Location = (new global::System.Drawing.Point(114, 168));
            this.chkUseJackettTextSearch.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkUseJackettTextSearch.Name = ("chkUseJackettTextSearch");
            this.chkUseJackettTextSearch.Size = (new global::System.Drawing.Size(105, 19));
            this.chkUseJackettTextSearch.TabIndex = (42);
            this.chkUseJackettTextSearch.Text = ("Use text search");
            this.chkUseJackettTextSearch.UseVisualStyleBackColor = (true);
            // 
            // chkSkipJackettFullScans
            // 
            this.chkSkipJackettFullScans.AutoSize = (true);
            this.chkSkipJackettFullScans.Location = (new global::System.Drawing.Point(289, 22));
            this.chkSkipJackettFullScans.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkSkipJackettFullScans.Name = ("chkSkipJackettFullScans");
            this.chkSkipJackettFullScans.Size = (new global::System.Drawing.Size(161, 19));
            this.chkSkipJackettFullScans.TabIndex = (41);
            this.chkSkipJackettFullScans.Text = ("Skip Jackett On Full Scans");
            this.chkSkipJackettFullScans.UseVisualStyleBackColor = (true);
            // 
            // llJackettLink
            // 
            this.llJackettLink.AutoSize = (true);
            this.llJackettLink.Location = (new global::System.Drawing.Point(114, 196));
            this.llJackettLink.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.llJackettLink.Name = ("llJackettLink");
            this.llJackettLink.Size = (new global::System.Drawing.Size(60, 15));
            this.llJackettLink.TabIndex = (40);
            this.llJackettLink.TabStop = (true);
            this.llJackettLink.Text = ("linkLabel1");
            this.llJackettLink.LinkClicked += (this.LlJackettLink_LinkClicked);
            // 
            // label71
            // 
            this.label71.AutoSize = (true);
            this.label71.Location = (new global::System.Drawing.Point(7, 196));
            this.label71.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label71.Name = ("label71");
            this.label71.Size = (new global::System.Drawing.Size(84, 15));
            this.label71.TabIndex = (39);
            this.label71.Text = ("Configuration:");
            // 
            // cbSearchJackettOnManualScansOnly
            // 
            this.cbSearchJackettOnManualScansOnly.AutoSize = (true);
            this.cbSearchJackettOnManualScansOnly.Location = (new global::System.Drawing.Point(10, 22));
            this.cbSearchJackettOnManualScansOnly.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbSearchJackettOnManualScansOnly.Name = ("cbSearchJackettOnManualScansOnly");
            this.cbSearchJackettOnManualScansOnly.Size = (new global::System.Drawing.Size(143, 19));
            this.cbSearchJackettOnManualScansOnly.TabIndex = (38);
            this.cbSearchJackettOnManualScansOnly.Text = ("Only on manual scans");
            this.cbSearchJackettOnManualScansOnly.UseVisualStyleBackColor = (true);
            // 
            // label72
            // 
            this.label72.AutoSize = (true);
            this.label72.Location = (new global::System.Drawing.Point(7, 112));
            this.label72.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label72.Name = ("label72");
            this.label72.Size = (new global::System.Drawing.Size(76, 15));
            this.label72.TabIndex = (37);
            this.label72.Text = ("Indexer Path:");
            // 
            // txtJackettIndexer
            // 
            this.txtJackettIndexer.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtJackettIndexer.Location = (new global::System.Drawing.Point(114, 108));
            this.txtJackettIndexer.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtJackettIndexer.Name = ("txtJackettIndexer");
            this.txtJackettIndexer.Size = (new global::System.Drawing.Size(344, 23));
            this.txtJackettIndexer.TabIndex = (36);
            // 
            // label73
            // 
            this.label73.AutoSize = (true);
            this.label73.Location = (new global::System.Drawing.Point(7, 142));
            this.label73.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label73.Name = ("label73");
            this.label73.Size = (new global::System.Drawing.Size(50, 15));
            this.label73.TabIndex = (35);
            this.label73.Text = ("API Key:");
            // 
            // txtJackettAPIKey
            // 
            this.txtJackettAPIKey.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtJackettAPIKey.Location = (new global::System.Drawing.Point(114, 138));
            this.txtJackettAPIKey.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtJackettAPIKey.Name = ("txtJackettAPIKey");
            this.txtJackettAPIKey.Size = (new global::System.Drawing.Size(344, 23));
            this.txtJackettAPIKey.TabIndex = (34);
            // 
            // label74
            // 
            this.label74.AutoSize = (true);
            this.label74.Location = (new global::System.Drawing.Point(7, 82));
            this.label74.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label74.Name = ("label74");
            this.label74.Size = (new global::System.Drawing.Size(32, 15));
            this.label74.TabIndex = (33);
            this.label74.Text = ("Port:");
            // 
            // txtJackettPort
            // 
            this.txtJackettPort.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtJackettPort.Location = (new global::System.Drawing.Point(114, 78));
            this.txtJackettPort.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtJackettPort.Name = ("txtJackettPort");
            this.txtJackettPort.Size = (new global::System.Drawing.Size(344, 23));
            this.txtJackettPort.TabIndex = (32);
            this.txtJackettPort.TextChanged += (this.JackettDetailsUpdate);
            // 
            // label75
            // 
            this.label75.AutoSize = (true);
            this.label75.Location = (new global::System.Drawing.Point(7, 52));
            this.label75.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label75.Name = ("label75");
            this.label75.Size = (new global::System.Drawing.Size(42, 15));
            this.label75.TabIndex = (31);
            this.label75.Text = ("Server:");
            // 
            // txtJackettServer
            // 
            this.txtJackettServer.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtJackettServer.Location = (new global::System.Drawing.Point(113, 48));
            this.txtJackettServer.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtJackettServer.Name = ("txtJackettServer");
            this.txtJackettServer.Size = (new global::System.Drawing.Size(344, 23));
            this.txtJackettServer.TabIndex = (30);
            this.txtJackettServer.TextChanged += (this.JackettDetailsUpdate);
            // 
            // label45
            // 
            this.label45.AutoSize = (true);
            this.label45.Location = (new global::System.Drawing.Point(7, 117));
            this.label45.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label45.Name = ("label45");
            this.label45.Size = (new global::System.Drawing.Size(92, 15));
            this.label45.TabIndex = (33);
            this.label45.Text = ("Preferred Terms:");
            // 
            // tbPreferredRSSTerms
            // 
            this.tbPreferredRSSTerms.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.tbPreferredRSSTerms.Location = (new global::System.Drawing.Point(120, 113));
            this.tbPreferredRSSTerms.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbPreferredRSSTerms.Name = ("tbPreferredRSSTerms");
            this.tbPreferredRSSTerms.Size = (new global::System.Drawing.Size(332, 23));
            this.tbPreferredRSSTerms.TabIndex = (32);
            // 
            // tpAutoExportLibrary
            // 
            this.tpAutoExportLibrary.Controls.Add(this.chkRestrictMissingExportsToFullScans);
            this.tpAutoExportLibrary.Controls.Add(this.pbuShowExport);
            this.tpAutoExportLibrary.Controls.Add(this.label89);
            this.tpAutoExportLibrary.Controls.Add(this.groupBox26);
            this.tpAutoExportLibrary.Controls.Add(this.groupBox7);
            this.tpAutoExportLibrary.Controls.Add(this.groupBox27);
            this.tpAutoExportLibrary.Controls.Add(this.groupBox3);
            this.tpAutoExportLibrary.Location = (new global::System.Drawing.Point(149, 4));
            this.tpAutoExportLibrary.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpAutoExportLibrary.Name = ("tpAutoExportLibrary");
            this.tpAutoExportLibrary.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tpAutoExportLibrary.Size = (new global::System.Drawing.Size(500, 684));
            this.tpAutoExportLibrary.TabIndex = (19);
            this.tpAutoExportLibrary.Text = ("Library Export");
            this.tpAutoExportLibrary.UseVisualStyleBackColor = (true);
            // 
            // chkRestrictMissingExportsToFullScans
            // 
            this.chkRestrictMissingExportsToFullScans.AutoSize = (true);
            this.chkRestrictMissingExportsToFullScans.Location = (new global::System.Drawing.Point(10, 67));
            this.chkRestrictMissingExportsToFullScans.Name = ("chkRestrictMissingExportsToFullScans");
            this.chkRestrictMissingExportsToFullScans.Size = (new global::System.Drawing.Size(220, 19));
            this.chkRestrictMissingExportsToFullScans.TabIndex = (45);
            this.chkRestrictMissingExportsToFullScans.Text = ("Restrict Missing Exports to Full Scans");
            this.chkRestrictMissingExportsToFullScans.UseVisualStyleBackColor = (true);
            // 
            // pbuShowExport
            // 
            this.pbuShowExport.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.pbuShowExport.Cursor = (global::System.Windows.Forms.Cursors.Hand);
            this.pbuShowExport.Image = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbuShowExport.InitialImage = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbuShowExport.Location = (new global::System.Drawing.Point(418, 18));
            this.pbuShowExport.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.pbuShowExport.Name = ("pbuShowExport");
            this.pbuShowExport.Size = (new global::System.Drawing.Size(50, 46));
            this.pbuShowExport.SizeMode = (global::System.Windows.Forms.PictureBoxSizeMode.CenterImage);
            this.pbuShowExport.TabIndex = (44);
            this.pbuShowExport.TabStop = (false);
            this.pbuShowExport.Click += (this.pbuShowExport_Click);
            // 
            // label89
            // 
            this.label89.AutoSize = (true);
            this.label89.Location = (new global::System.Drawing.Point(5, 13));
            this.label89.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label89.Name = ("label89");
            this.label89.Size = (new global::System.Drawing.Size(257, 30));
            this.label89.TabIndex = (43);
            this.label89.Text = ("TV Rename can export information about\r\nshows, movies and episodes in various formats.");
            // 
            // groupBox26
            // 
            this.groupBox26.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox26.Controls.Add(this.bnBrowseMoviesHTML);
            this.groupBox26.Controls.Add(this.cbMoviesHTML);
            this.groupBox26.Controls.Add(this.txtMoviesHTMLTo);
            this.groupBox26.Controls.Add(this.bnBrowseMoviesTXT);
            this.groupBox26.Controls.Add(this.cbMoviesTXT);
            this.groupBox26.Controls.Add(this.txtMoviesTXTTo);
            this.groupBox26.Location = (new global::System.Drawing.Point(12, 366));
            this.groupBox26.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox26.Name = ("groupBox26");
            this.groupBox26.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox26.Size = (new global::System.Drawing.Size(460, 83));
            this.groupBox26.TabIndex = (10);
            this.groupBox26.TabStop = (false);
            this.groupBox26.Text = ("All Movies");
            // 
            // bnBrowseMoviesHTML
            // 
            this.bnBrowseMoviesHTML.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseMoviesHTML.Location = (new global::System.Drawing.Point(363, 52));
            this.bnBrowseMoviesHTML.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnBrowseMoviesHTML.Name = ("bnBrowseMoviesHTML");
            this.bnBrowseMoviesHTML.Size = (new global::System.Drawing.Size(88, 27));
            this.bnBrowseMoviesHTML.TabIndex = (8);
            this.bnBrowseMoviesHTML.Text = ("Browse...");
            this.bnBrowseMoviesHTML.UseVisualStyleBackColor = (true);
            this.bnBrowseMoviesHTML.Click += (this.bnBrowseMoviesHTML_Click);
            // 
            // cbMoviesHTML
            // 
            this.cbMoviesHTML.AutoSize = (true);
            this.cbMoviesHTML.Location = (new global::System.Drawing.Point(9, 57));
            this.cbMoviesHTML.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbMoviesHTML.Name = ("cbMoviesHTML");
            this.cbMoviesHTML.Size = (new global::System.Drawing.Size(58, 19));
            this.cbMoviesHTML.TabIndex = (6);
            this.cbMoviesHTML.Text = ("HTML");
            this.cbMoviesHTML.UseVisualStyleBackColor = (true);
            this.cbMoviesHTML.CheckedChanged += (this.EnableDisable);
            // 
            // txtMoviesHTMLTo
            // 
            this.txtMoviesHTMLTo.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtMoviesHTMLTo.Location = (new global::System.Drawing.Point(75, 54));
            this.txtMoviesHTMLTo.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtMoviesHTMLTo.Name = ("txtMoviesHTMLTo");
            this.txtMoviesHTMLTo.Size = (new global::System.Drawing.Size(280, 23));
            this.txtMoviesHTMLTo.TabIndex = (7);
            // 
            // bnBrowseMoviesTXT
            // 
            this.bnBrowseMoviesTXT.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseMoviesTXT.Location = (new global::System.Drawing.Point(363, 24));
            this.bnBrowseMoviesTXT.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnBrowseMoviesTXT.Name = ("bnBrowseMoviesTXT");
            this.bnBrowseMoviesTXT.Size = (new global::System.Drawing.Size(88, 27));
            this.bnBrowseMoviesTXT.TabIndex = (5);
            this.bnBrowseMoviesTXT.Text = ("Browse...");
            this.bnBrowseMoviesTXT.UseVisualStyleBackColor = (true);
            this.bnBrowseMoviesTXT.Click += (this.bnBrowseMoviesTXT_Click);
            // 
            // cbMoviesTXT
            // 
            this.cbMoviesTXT.AutoSize = (true);
            this.cbMoviesTXT.Location = (new global::System.Drawing.Point(9, 29));
            this.cbMoviesTXT.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbMoviesTXT.Name = ("cbMoviesTXT");
            this.cbMoviesTXT.Size = (new global::System.Drawing.Size(45, 19));
            this.cbMoviesTXT.TabIndex = (3);
            this.cbMoviesTXT.Text = ("TXT");
            this.cbMoviesTXT.UseVisualStyleBackColor = (true);
            this.cbMoviesTXT.CheckedChanged += (this.EnableDisable);
            // 
            // txtMoviesTXTTo
            // 
            this.txtMoviesTXTTo.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtMoviesTXTTo.Location = (new global::System.Drawing.Point(75, 27));
            this.txtMoviesTXTTo.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtMoviesTXTTo.Name = ("txtMoviesTXTTo");
            this.txtMoviesTXTTo.Size = (new global::System.Drawing.Size(279, 23));
            this.txtMoviesTXTTo.TabIndex = (4);
            // 
            // groupBox7
            // 
            this.groupBox7.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox7.Controls.Add(this.bnBrowseShowsHTML);
            this.groupBox7.Controls.Add(this.cbShowsHTML);
            this.groupBox7.Controls.Add(this.txtShowsHTMLTo);
            this.groupBox7.Controls.Add(this.bnBrowseShowsTXT);
            this.groupBox7.Controls.Add(this.cbShowsTXT);
            this.groupBox7.Controls.Add(this.txtShowsTXTTo);
            this.groupBox7.Location = (new global::System.Drawing.Point(12, 184));
            this.groupBox7.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox7.Name = ("groupBox7");
            this.groupBox7.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox7.Size = (new global::System.Drawing.Size(460, 83));
            this.groupBox7.TabIndex = (6);
            this.groupBox7.TabStop = (false);
            this.groupBox7.Text = ("All TV Shows");
            // 
            // bnBrowseShowsHTML
            // 
            this.bnBrowseShowsHTML.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseShowsHTML.Location = (new global::System.Drawing.Point(363, 52));
            this.bnBrowseShowsHTML.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnBrowseShowsHTML.Name = ("bnBrowseShowsHTML");
            this.bnBrowseShowsHTML.Size = (new global::System.Drawing.Size(88, 27));
            this.bnBrowseShowsHTML.TabIndex = (8);
            this.bnBrowseShowsHTML.Text = ("Browse...");
            this.bnBrowseShowsHTML.UseVisualStyleBackColor = (true);
            this.bnBrowseShowsHTML.Click += (this.bnBrowseShowsHTML_Click);
            // 
            // cbShowsHTML
            // 
            this.cbShowsHTML.AutoSize = (true);
            this.cbShowsHTML.Location = (new global::System.Drawing.Point(9, 57));
            this.cbShowsHTML.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbShowsHTML.Name = ("cbShowsHTML");
            this.cbShowsHTML.Size = (new global::System.Drawing.Size(58, 19));
            this.cbShowsHTML.TabIndex = (6);
            this.cbShowsHTML.Text = ("HTML");
            this.cbShowsHTML.UseVisualStyleBackColor = (true);
            this.cbShowsHTML.CheckedChanged += (this.EnableDisable);
            // 
            // txtShowsHTMLTo
            // 
            this.txtShowsHTMLTo.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtShowsHTMLTo.Location = (new global::System.Drawing.Point(75, 54));
            this.txtShowsHTMLTo.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtShowsHTMLTo.Name = ("txtShowsHTMLTo");
            this.txtShowsHTMLTo.Size = (new global::System.Drawing.Size(280, 23));
            this.txtShowsHTMLTo.TabIndex = (7);
            // 
            // bnBrowseShowsTXT
            // 
            this.bnBrowseShowsTXT.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseShowsTXT.Location = (new global::System.Drawing.Point(363, 24));
            this.bnBrowseShowsTXT.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnBrowseShowsTXT.Name = ("bnBrowseShowsTXT");
            this.bnBrowseShowsTXT.Size = (new global::System.Drawing.Size(88, 27));
            this.bnBrowseShowsTXT.TabIndex = (5);
            this.bnBrowseShowsTXT.Text = ("Browse...");
            this.bnBrowseShowsTXT.UseVisualStyleBackColor = (true);
            this.bnBrowseShowsTXT.Click += (this.bnBrowseShowsTXT_Click);
            // 
            // cbShowsTXT
            // 
            this.cbShowsTXT.AutoSize = (true);
            this.cbShowsTXT.Location = (new global::System.Drawing.Point(9, 29));
            this.cbShowsTXT.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbShowsTXT.Name = ("cbShowsTXT");
            this.cbShowsTXT.Size = (new global::System.Drawing.Size(45, 19));
            this.cbShowsTXT.TabIndex = (3);
            this.cbShowsTXT.Text = ("TXT");
            this.cbShowsTXT.UseVisualStyleBackColor = (true);
            this.cbShowsTXT.CheckedChanged += (this.EnableDisable);
            // 
            // txtShowsTXTTo
            // 
            this.txtShowsTXTTo.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtShowsTXTTo.Location = (new global::System.Drawing.Point(75, 27));
            this.txtShowsTXTTo.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtShowsTXTTo.Name = ("txtShowsTXTTo");
            this.txtShowsTXTTo.Size = (new global::System.Drawing.Size(279, 23));
            this.txtShowsTXTTo.TabIndex = (4);
            // 
            // groupBox27
            // 
            this.groupBox27.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox27.Controls.Add(this.bnBrowseMissingMoviesCSV);
            this.groupBox27.Controls.Add(this.bnBrowseMissingMoviesXML);
            this.groupBox27.Controls.Add(this.txtMissingMoviesCSV);
            this.groupBox27.Controls.Add(this.cbMissingMoviesXML);
            this.groupBox27.Controls.Add(this.cbMissingMoviesCSV);
            this.groupBox27.Controls.Add(this.txtMissingMoviesXML);
            this.groupBox27.Location = (new global::System.Drawing.Point(10, 274));
            this.groupBox27.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox27.Name = ("groupBox27");
            this.groupBox27.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox27.Size = (new global::System.Drawing.Size(461, 91));
            this.groupBox27.TabIndex = (9);
            this.groupBox27.TabStop = (false);
            this.groupBox27.Text = ("Missing Movies");
            // 
            // bnBrowseMissingMoviesCSV
            // 
            this.bnBrowseMissingMoviesCSV.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseMissingMoviesCSV.Location = (new global::System.Drawing.Point(363, 54));
            this.bnBrowseMissingMoviesCSV.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnBrowseMissingMoviesCSV.Name = ("bnBrowseMissingMoviesCSV");
            this.bnBrowseMissingMoviesCSV.Size = (new global::System.Drawing.Size(88, 27));
            this.bnBrowseMissingMoviesCSV.TabIndex = (2);
            this.bnBrowseMissingMoviesCSV.Text = ("Browse...");
            this.bnBrowseMissingMoviesCSV.UseVisualStyleBackColor = (true);
            this.bnBrowseMissingMoviesCSV.Click += (this.bnBrowseMissingMoviesCSV_Click);
            // 
            // bnBrowseMissingMoviesXML
            // 
            this.bnBrowseMissingMoviesXML.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseMissingMoviesXML.Location = (new global::System.Drawing.Point(364, 22));
            this.bnBrowseMissingMoviesXML.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnBrowseMissingMoviesXML.Name = ("bnBrowseMissingMoviesXML");
            this.bnBrowseMissingMoviesXML.Size = (new global::System.Drawing.Size(88, 27));
            this.bnBrowseMissingMoviesXML.TabIndex = (5);
            this.bnBrowseMissingMoviesXML.Text = ("Browse...");
            this.bnBrowseMissingMoviesXML.UseVisualStyleBackColor = (true);
            this.bnBrowseMissingMoviesXML.Click += (this.bnBrowseMissingMoviesXML_Click);
            // 
            // txtMissingMoviesCSV
            // 
            this.txtMissingMoviesCSV.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtMissingMoviesCSV.Location = (new global::System.Drawing.Point(75, 55));
            this.txtMissingMoviesCSV.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtMissingMoviesCSV.Name = ("txtMissingMoviesCSV");
            this.txtMissingMoviesCSV.Size = (new global::System.Drawing.Size(280, 23));
            this.txtMissingMoviesCSV.TabIndex = (1);
            // 
            // cbMissingMoviesXML
            // 
            this.cbMissingMoviesXML.AutoSize = (true);
            this.cbMissingMoviesXML.Location = (new global::System.Drawing.Point(9, 27));
            this.cbMissingMoviesXML.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbMissingMoviesXML.Name = ("cbMissingMoviesXML");
            this.cbMissingMoviesXML.Size = (new global::System.Drawing.Size(50, 19));
            this.cbMissingMoviesXML.TabIndex = (3);
            this.cbMissingMoviesXML.Text = ("XML");
            this.cbMissingMoviesXML.UseVisualStyleBackColor = (true);
            this.cbMissingMoviesXML.CheckedChanged += (this.EnableDisable);
            // 
            // cbMissingMoviesCSV
            // 
            this.cbMissingMoviesCSV.AutoSize = (true);
            this.cbMissingMoviesCSV.Location = (new global::System.Drawing.Point(10, 55));
            this.cbMissingMoviesCSV.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbMissingMoviesCSV.Name = ("cbMissingMoviesCSV");
            this.cbMissingMoviesCSV.Size = (new global::System.Drawing.Size(47, 19));
            this.cbMissingMoviesCSV.TabIndex = (0);
            this.cbMissingMoviesCSV.Text = ("CSV");
            this.cbMissingMoviesCSV.UseVisualStyleBackColor = (true);
            this.cbMissingMoviesCSV.CheckedChanged += (this.EnableDisable);
            // 
            // txtMissingMoviesXML
            // 
            this.txtMissingMoviesXML.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtMissingMoviesXML.Location = (new global::System.Drawing.Point(75, 24));
            this.txtMissingMoviesXML.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtMissingMoviesXML.Name = ("txtMissingMoviesXML");
            this.txtMissingMoviesXML.Size = (new global::System.Drawing.Size(280, 23));
            this.txtMissingMoviesXML.TabIndex = (4);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.bnBrowseMissingCSV);
            this.groupBox3.Controls.Add(this.bnBrowseMissingXML);
            this.groupBox3.Controls.Add(this.txtMissingCSV);
            this.groupBox3.Controls.Add(this.cbMissingXML);
            this.groupBox3.Controls.Add(this.cbMissingCSV);
            this.groupBox3.Controls.Add(this.txtMissingXML);
            this.groupBox3.Location = (new global::System.Drawing.Point(10, 92));
            this.groupBox3.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox3.Name = ("groupBox3");
            this.groupBox3.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.groupBox3.Size = (new global::System.Drawing.Size(461, 91));
            this.groupBox3.TabIndex = (5);
            this.groupBox3.TabStop = (false);
            this.groupBox3.Text = ("Missing Episodes");
            // 
            // bnBrowseMissingCSV
            // 
            this.bnBrowseMissingCSV.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseMissingCSV.Location = (new global::System.Drawing.Point(363, 54));
            this.bnBrowseMissingCSV.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnBrowseMissingCSV.Name = ("bnBrowseMissingCSV");
            this.bnBrowseMissingCSV.Size = (new global::System.Drawing.Size(88, 27));
            this.bnBrowseMissingCSV.TabIndex = (2);
            this.bnBrowseMissingCSV.Text = ("Browse...");
            this.bnBrowseMissingCSV.UseVisualStyleBackColor = (true);
            this.bnBrowseMissingCSV.Click += (this.bnBrowseMissingCSV_Click);
            // 
            // bnBrowseMissingXML
            // 
            this.bnBrowseMissingXML.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseMissingXML.Location = (new global::System.Drawing.Point(364, 22));
            this.bnBrowseMissingXML.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.bnBrowseMissingXML.Name = ("bnBrowseMissingXML");
            this.bnBrowseMissingXML.Size = (new global::System.Drawing.Size(88, 27));
            this.bnBrowseMissingXML.TabIndex = (5);
            this.bnBrowseMissingXML.Text = ("Browse...");
            this.bnBrowseMissingXML.UseVisualStyleBackColor = (true);
            this.bnBrowseMissingXML.Click += (this.bnBrowseMissingXML_Click);
            // 
            // txtMissingCSV
            // 
            this.txtMissingCSV.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtMissingCSV.Location = (new global::System.Drawing.Point(75, 55));
            this.txtMissingCSV.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtMissingCSV.Name = ("txtMissingCSV");
            this.txtMissingCSV.Size = (new global::System.Drawing.Size(280, 23));
            this.txtMissingCSV.TabIndex = (1);
            // 
            // cbMissingXML
            // 
            this.cbMissingXML.AutoSize = (true);
            this.cbMissingXML.Location = (new global::System.Drawing.Point(9, 27));
            this.cbMissingXML.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbMissingXML.Name = ("cbMissingXML");
            this.cbMissingXML.Size = (new global::System.Drawing.Size(50, 19));
            this.cbMissingXML.TabIndex = (3);
            this.cbMissingXML.Text = ("XML");
            this.cbMissingXML.UseVisualStyleBackColor = (true);
            this.cbMissingXML.CheckedChanged += (this.EnableDisable);
            // 
            // cbMissingCSV
            // 
            this.cbMissingCSV.AutoSize = (true);
            this.cbMissingCSV.Location = (new global::System.Drawing.Point(10, 55));
            this.cbMissingCSV.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cbMissingCSV.Name = ("cbMissingCSV");
            this.cbMissingCSV.Size = (new global::System.Drawing.Size(47, 19));
            this.cbMissingCSV.TabIndex = (0);
            this.cbMissingCSV.Text = ("CSV");
            this.cbMissingCSV.UseVisualStyleBackColor = (true);
            this.cbMissingCSV.CheckedChanged += (this.EnableDisable);
            // 
            // txtMissingXML
            // 
            this.txtMissingXML.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.txtMissingXML.Location = (new global::System.Drawing.Point(75, 24));
            this.txtMissingXML.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.txtMissingXML.Name = ("txtMissingXML");
            this.txtMissingXML.Size = (new global::System.Drawing.Size(280, 23));
            this.txtMissingXML.TabIndex = (4);
            // 
            // tbAppUpdate
            // 
            this.tbAppUpdate.Controls.Add(this.pbuUpdates);
            this.tbAppUpdate.Controls.Add(this.chkUpdateCheckEnabled);
            this.tbAppUpdate.Controls.Add(this.label92);
            this.tbAppUpdate.Controls.Add(this.grpUpdateIntervalOption);
            this.tbAppUpdate.Location = (new global::System.Drawing.Point(149, 4));
            this.tbAppUpdate.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.tbAppUpdate.Name = ("tbAppUpdate");
            this.tbAppUpdate.Size = (new global::System.Drawing.Size(500, 684));
            this.tbAppUpdate.TabIndex = (20);
            this.tbAppUpdate.Text = ("App Updates");
            this.tbAppUpdate.UseVisualStyleBackColor = (true);
            // 
            // pbuUpdates
            // 
            this.pbuUpdates.Anchor = ((global::System.Windows.Forms.AnchorStyles)((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.pbuUpdates.Cursor = (global::System.Windows.Forms.Cursors.Hand);
            this.pbuUpdates.Image = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbuUpdates.InitialImage = (global::TVRename.Properties.Resources.iconfinder_Info_Circle_Symbol_Information_Letter_1396823);
            this.pbuUpdates.Location = (new global::System.Drawing.Point(416, 16));
            this.pbuUpdates.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.pbuUpdates.Name = ("pbuUpdates");
            this.pbuUpdates.Size = (new global::System.Drawing.Size(50, 46));
            this.pbuUpdates.SizeMode = (global::System.Windows.Forms.PictureBoxSizeMode.CenterImage);
            this.pbuUpdates.TabIndex = (46);
            this.pbuUpdates.TabStop = (false);
            this.pbuUpdates.Click += (this.pbuUpdates_Click);
            // 
            // chkUpdateCheckEnabled
            // 
            this.chkUpdateCheckEnabled.AutoSize = (true);
            this.chkUpdateCheckEnabled.BackColor = (global::System.Drawing.Color.White);
            this.chkUpdateCheckEnabled.Location = (new global::System.Drawing.Point(13, 70));
            this.chkUpdateCheckEnabled.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkUpdateCheckEnabled.Name = ("chkUpdateCheckEnabled");
            this.chkUpdateCheckEnabled.Size = (new global::System.Drawing.Size(123, 19));
            this.chkUpdateCheckEnabled.TabIndex = (0);
            this.chkUpdateCheckEnabled.Text = ("Check for Updates");
            this.chkUpdateCheckEnabled.UseVisualStyleBackColor = (false);
            this.chkUpdateCheckEnabled.CheckedChanged += (this.chkUpdateCheckEnabled_CheckedChanged);
            // 
            // label92
            // 
            this.label92.AutoSize = (true);
            this.label92.Location = (new global::System.Drawing.Point(4, 10));
            this.label92.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label92.Name = ("label92");
            this.label92.Size = (new global::System.Drawing.Size(249, 30));
            this.label92.TabIndex = (45);
            this.label92.Text = ("Define how TV Rename alerts users about new\r\nversions being available");
            // 
            // grpUpdateIntervalOption
            // 
            this.grpUpdateIntervalOption.Anchor = ((global::System.Windows.Forms.AnchorStyles)(((global::System.Windows.Forms.AnchorStyles.Top) | (global::System.Windows.Forms.AnchorStyles.Left)) | (global::System.Windows.Forms.AnchorStyles.Right)));
            this.grpUpdateIntervalOption.Controls.Add(this.chkNoPopupOnUpdate);
            this.grpUpdateIntervalOption.Controls.Add(this.cboUpdateCheckInterval);
            this.grpUpdateIntervalOption.Controls.Add(this.optUpdateCheckInterval);
            this.grpUpdateIntervalOption.Controls.Add(this.optUpdateCheckAlways);
            this.grpUpdateIntervalOption.Location = (new global::System.Drawing.Point(13, 82));
            this.grpUpdateIntervalOption.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.grpUpdateIntervalOption.Name = ("grpUpdateIntervalOption");
            this.grpUpdateIntervalOption.Padding = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.grpUpdateIntervalOption.Size = (new global::System.Drawing.Size(454, 143));
            this.grpUpdateIntervalOption.TabIndex = (1);
            this.grpUpdateIntervalOption.TabStop = (false);
            // 
            // chkNoPopupOnUpdate
            // 
            this.chkNoPopupOnUpdate.AutoSize = (true);
            this.chkNoPopupOnUpdate.Location = (new global::System.Drawing.Point(8, 107));
            this.chkNoPopupOnUpdate.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.chkNoPopupOnUpdate.Name = ("chkNoPopupOnUpdate");
            this.chkNoPopupOnUpdate.Size = (new global::System.Drawing.Size(226, 19));
            this.chkNoPopupOnUpdate.TabIndex = (2);
            this.chkNoPopupOnUpdate.Text = ("No dialog when an update is available");
            this.chkNoPopupOnUpdate.UseVisualStyleBackColor = (true);
            // 
            // cboUpdateCheckInterval
            // 
            this.cboUpdateCheckInterval.DropDownStyle = (global::System.Windows.Forms.ComboBoxStyle.DropDownList);
            this.cboUpdateCheckInterval.FormattingEnabled = (true);
            this.cboUpdateCheckInterval.Location = (new global::System.Drawing.Point(8, 68));
            this.cboUpdateCheckInterval.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.cboUpdateCheckInterval.Name = ("cboUpdateCheckInterval");
            this.cboUpdateCheckInterval.Size = (new global::System.Drawing.Size(193, 23));
            this.cboUpdateCheckInterval.TabIndex = (2);
            // 
            // optUpdateCheckInterval
            // 
            this.optUpdateCheckInterval.AutoSize = (true);
            this.optUpdateCheckInterval.Location = (new global::System.Drawing.Point(8, 40));
            this.optUpdateCheckInterval.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.optUpdateCheckInterval.Name = ("optUpdateCheckInterval");
            this.optUpdateCheckInterval.Size = (new global::System.Drawing.Size(121, 19));
            this.optUpdateCheckInterval.TabIndex = (1);
            this.optUpdateCheckInterval.TabStop = (true);
            this.optUpdateCheckInterval.Text = ("at certain intervals");
            this.optUpdateCheckInterval.UseVisualStyleBackColor = (true);
            this.optUpdateCheckInterval.CheckedChanged += (this.updateCheckOption_CheckedChanged);
            // 
            // optUpdateCheckAlways
            // 
            this.optUpdateCheckAlways.AutoSize = (true);
            this.optUpdateCheckAlways.Location = (new global::System.Drawing.Point(8, 13));
            this.optUpdateCheckAlways.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.optUpdateCheckAlways.Name = ("optUpdateCheckAlways");
            this.optUpdateCheckAlways.Size = (new global::System.Drawing.Size(96, 19));
            this.optUpdateCheckAlways.TabIndex = (0);
            this.optUpdateCheckAlways.TabStop = (true);
            this.optUpdateCheckAlways.Text = ("on every start");
            this.optUpdateCheckAlways.UseVisualStyleBackColor = (true);
            this.optUpdateCheckAlways.CheckedChanged += (this.updateCheckOption_CheckedChanged);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.rdoGlobalReleaseDates);
            this.panel4.Controls.Add(this.rdoRegionalReleaseDates);
            this.panel4.Location = (new global::System.Drawing.Point(150, 199));
            this.panel4.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.panel4.Name = ("panel4");
            this.panel4.Size = (new global::System.Drawing.Size(303, 33));
            this.panel4.TabIndex = (65);
            // 
            // rdoGlobalReleaseDates
            // 
            this.rdoGlobalReleaseDates.AutoSize = (true);
            this.rdoGlobalReleaseDates.Location = (new global::System.Drawing.Point(83, 5));
            this.rdoGlobalReleaseDates.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rdoGlobalReleaseDates.Name = ("rdoGlobalReleaseDates");
            this.rdoGlobalReleaseDates.Size = (new global::System.Drawing.Size(59, 19));
            this.rdoGlobalReleaseDates.TabIndex = (60);
            this.rdoGlobalReleaseDates.TabStop = (true);
            this.rdoGlobalReleaseDates.Text = ("Global");
            this.rdoGlobalReleaseDates.UseVisualStyleBackColor = (true);
            // 
            // rdoRegionalReleaseDates
            // 
            this.rdoRegionalReleaseDates.AutoSize = (true);
            this.rdoRegionalReleaseDates.Location = (new global::System.Drawing.Point(4, 5));
            this.rdoRegionalReleaseDates.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.rdoRegionalReleaseDates.Name = ("rdoRegionalReleaseDates");
            this.rdoRegionalReleaseDates.Size = (new global::System.Drawing.Size(71, 19));
            this.rdoRegionalReleaseDates.TabIndex = (59);
            this.rdoRegionalReleaseDates.TabStop = (true);
            this.rdoRegionalReleaseDates.Text = ("Regional");
            this.rdoRegionalReleaseDates.UseVisualStyleBackColor = (true);
            // 
            // label100
            // 
            this.label100.AutoSize = (true);
            this.label100.Location = (new global::System.Drawing.Point(10, 203));
            this.label100.Margin = (new global::System.Windows.Forms.Padding(4, 0, 4, 0));
            this.label100.Name = ("label100");
            this.label100.Size = (new global::System.Drawing.Size(103, 15));
            this.label100.TabIndex = (64);
            this.label100.Text = ("Release Date Type:");
            this.label100.TextAlign = (global::System.Drawing.ContentAlignment.TopRight);
            // 
            // Preferences
            // 
            this.AcceptButton = (this.OKButton);
            this.AutoScaleDimensions = (new global::System.Drawing.SizeF(7F, 15F));
            this.AutoScaleMode = (global::System.Windows.Forms.AutoScaleMode.Font);
            this.CancelButton = (this.bnCancel);
            this.ClientSize = (new global::System.Drawing.Size(681, 743));
            this.ControlBox = (false);
            this.Controls.Add(this.tcTabs);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.OKButton);
            this.Icon = ((global::System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = (new global::System.Windows.Forms.Padding(4, 3, 4, 3));
            this.MaximizeBox = (false);
            this.MinimizeBox = (false);
            this.MinimumSize = (new global::System.Drawing.Size(697, 753));
            this.Name = ("Preferences");
            this.ShowInTaskbar = (false);
            this.SizeGripStyle = (global::System.Windows.Forms.SizeGripStyle.Show);
            this.StartPosition = (global::System.Windows.Forms.FormStartPosition.CenterParent);
            this.Text = ("Preferences");
            this.FormClosing += (this.Preferences_FormClosing);
            this.Load += (this.Preferences_Load);
            this.cmDefaults.ResumeLayout(false);
            this.tpDisplay.ResumeLayout(false);
            this.tpDisplay.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbDisplay)).EndInit();
            this.tpRSSJSONSearch.ResumeLayout(false);
            this.tpRSSJSONSearch.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbRSSJSONSearch)).EndInit();
            this.gbJSON.ResumeLayout(false);
            this.gbJSON.PerformLayout();
            this.gbRSS.ResumeLayout(false);
            this.gbRSS.PerformLayout();
            this.tpLibraryFolders.ResumeLayout(false);
            this.tpLibraryFolders.PerformLayout();
            this.groupBox23.ResumeLayout(false);
            this.groupBox23.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbLibraryFolders)).EndInit();
            this.tpTorrentNZB.ResumeLayout(false);
            this.tpTorrentNZB.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbuTorrentNZB)).EndInit();
            this.qBitTorrent.ResumeLayout(false);
            this.qBitTorrent.PerformLayout();
            this.gbSAB.ResumeLayout(false);
            this.gbSAB.PerformLayout();
            this.gbuTorrent.ResumeLayout(false);
            this.gbuTorrent.PerformLayout();
            this.tbSearchFolders.ResumeLayout(false);
            this.tbSearchFolders.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.gbAutoAdd.ResumeLayout(false);
            this.gbAutoAdd.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbSearchFolders)).EndInit();
            this.tbMediaCenter.ResumeLayout(false);
            this.tbMediaCenter.PerformLayout();
            this.groupBox16.ResumeLayout(false);
            this.groupBox16.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            this.groupBox15.ResumeLayout(false);
            this.groupBox15.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbMediaCenter)).EndInit();
            this.tbFolderDeleting.ResumeLayout(false);
            this.tbFolderDeleting.PerformLayout();
            this.groupBox28.ResumeLayout(false);
            this.groupBox28.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbFolderDeleting)).EndInit();
            this.tbAutoExport.ResumeLayout(false);
            this.tbAutoExport.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbuExportEpisodes)).EndInit();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tbFilesAndFolders.ResumeLayout(false);
            this.tbFilesAndFolders.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbFilesAndFolders)).EndInit();
            this.tbGeneral.ResumeLayout(false);
            this.tbGeneral.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbGeneral)).EndInit();
            this.tcTabs.ResumeLayout(false);
            this.tpDataSources.ResumeLayout(false);
            this.tpDataSources.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.gbTMDB.ResumeLayout(false);
            this.gbTMDB.PerformLayout();
            this.groupBox20.ResumeLayout(false);
            this.groupBox20.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbSources)).EndInit();
            this.tpMovieDefaults.ResumeLayout(false);
            this.tpMovieDefaults.PerformLayout();
            this.groupBox24.ResumeLayout(false);
            this.groupBox24.PerformLayout();
            this.groupBox25.ResumeLayout(false);
            this.groupBox25.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbMovieDefaults)).EndInit();
            this.tpShowDefaults.ResumeLayout(false);
            this.tpShowDefaults.PerformLayout();
            this.groupBox19.ResumeLayout(false);
            this.groupBox19.PerformLayout();
            this.groupBox18.ResumeLayout(false);
            this.groupBox18.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tpScanSettings.ResumeLayout(false);
            this.tpScanSettings.PerformLayout();
            this.groupBox17.ResumeLayout(false);
            this.groupBox17.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbBulkAdd.ResumeLayout(false);
            this.gbBulkAdd.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbScanOptions)).EndInit();
            this.tpSubtitles.ResumeLayout(false);
            this.tpSubtitles.PerformLayout();
            this.groupBox29.ResumeLayout(false);
            this.groupBox29.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.tpJackett.ResumeLayout(false);
            this.tpJackett.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbuJackett)).EndInit();
            this.groupBox22.ResumeLayout(false);
            this.groupBox22.PerformLayout();
            this.tpAutoExportLibrary.ResumeLayout(false);
            this.tpAutoExportLibrary.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbuShowExport)).EndInit();
            this.groupBox26.ResumeLayout(false);
            this.groupBox26.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox27.ResumeLayout(false);
            this.groupBox27.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tbAppUpdate.ResumeLayout(false);
            this.tbAppUpdate.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)(this.pbuUpdates)).EndInit();
            this.grpUpdateIntervalOption.ResumeLayout(false);
            this.grpUpdateIntervalOption.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);
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
