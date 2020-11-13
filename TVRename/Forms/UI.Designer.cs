//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace TVRename
{
    partial class UI
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (disposing && (this.mAutoFolderMonitor  != null))
            {
                mAutoFolderMonitor.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        public void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UI));
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Recently Aired", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Next 7 Days", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Future Episodes", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Later", System.Windows.Forms.HorizontalAlignment.Left);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.offlineOperationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundDownloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ignoreListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filenameTemplateEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.movieSearchEnginesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchEnginesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filenameProcessorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flushCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flushImageCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundDownloadNowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.folderMonitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bulkAddMoviesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.duplicateFinderLOGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickRenameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOrphanFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateMoviesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statisticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.showSummaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.movieCollectionSummaryLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.betaToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timezoneInconsistencyLOGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.episodeFileQualitySummaryLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.accuracyCheckLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tMDBAccuracyCheckLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickstartGuideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visitWebsiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visitSupportForumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bugReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.buyMeADrinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.checkForNewVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.thanksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tbMyMovies = new System.Windows.Forms.TabPage();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tbnAddMovie = new System.Windows.Forms.ToolStripButton();
            this.btnEditMovie = new System.Windows.Forms.ToolStripButton();
            this.btnMovieDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.btnMovieRefresh = new System.Windows.Forms.ToolStripButton();
            this.tsbScanMovies = new System.Windows.Forms.ToolStripButton();
            this.btnMovieFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbMyMoviesContextMenu = new System.Windows.Forms.ToolStripButton();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.movieTree = new System.Windows.Forms.TreeView();
            this.filterMoviesTextbox = new System.Windows.Forms.TextBox();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.webMovieInformation = new System.Windows.Forms.WebBrowser();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.webMovieImages = new System.Windows.Forms.WebBrowser();
            this.tbMyShows = new System.Windows.Forms.TabPage();
            this.tsMyShows = new System.Windows.Forms.ToolStrip();
            this.btnAddTVShow = new System.Windows.Forms.ToolStripButton();
            this.btnEditShow = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveShow = new System.Windows.Forms.ToolStripButton();
            this.btnHideHTMLPanel = new System.Windows.Forms.ToolStripButton();
            this.btnMyShowsCollapse = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.btnMyShowsRefresh = new System.Windows.Forms.ToolStripButton();
            this.btnFilterMyShows = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbMyShowsContextMenu = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.MyShowTree = new System.Windows.Forms.TreeView();
            this.filterTextBox = new System.Windows.Forms.TextBox();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tpInformation = new System.Windows.Forms.TabPage();
            this.webInformation = new System.Windows.Forms.WebBrowser();
            this.tpImages = new System.Windows.Forms.TabPage();
            this.webImages = new System.Windows.Forms.WebBrowser();
            this.tpSummary = new System.Windows.Forms.TabPage();
            this.webSummary = new System.Windows.Forms.WebBrowser();
            this.tbAllInOne = new System.Windows.Forms.TabPage();
            this.olvAction = new TVRename.ObjectListViewFlickerFree();
            this.olvShowColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvSeason = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvEpisode = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvDate = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvFolder = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvFilename = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvSource = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvErrors = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ilIcons = new System.Windows.Forms.ImageList(this.components);
            this.tsScanResults = new System.Windows.Forms.ToolStrip();
            this.btnScan = new System.Windows.Forms.ToolStripSplitButton();
            this.btnFullScan = new System.Windows.Forms.ToolStripMenuItem();
            this.recentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbFullScan = new System.Windows.Forms.ToolStripButton();
            this.tpRecentScan = new System.Windows.Forms.ToolStripButton();
            this.tbQuickScan = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.btnActionBTSearch = new System.Windows.Forms.ToolStripSplitButton();
            this.tbActionJackettSearch = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.btnIgnoreSelectedActions = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveSelActions = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.mcbAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mcbRename = new System.Windows.Forms.ToolStripMenuItem();
            this.mcbCopyMove = new System.Windows.Forms.ToolStripMenuItem();
            this.mcbDeleteFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.mcbSaveImages = new System.Windows.Forms.ToolStripMenuItem();
            this.mcbDownload = new System.Windows.Forms.ToolStripMenuItem();
            this.mcbWriteMetadata = new System.Windows.Forms.ToolStripMenuItem();
            this.mcbModifyMetadata = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.btnActionAction = new System.Windows.Forms.ToolStripButton();
            this.btnRevertView = new System.Windows.Forms.ToolStripButton();
            this.btnPreferences = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbScanContextMenu = new System.Windows.Forms.ToolStripButton();
            this.tbWTW = new System.Windows.Forms.TabPage();
            this.tsWtW = new System.Windows.Forms.ToolStrip();
            this.btnWhenToWatchCheck = new System.Windows.Forms.ToolStripButton();
            this.btnScheduleBTSearch = new System.Windows.Forms.ToolStripSplitButton();
            this.tsbScheduleJackettSearch = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.btnScheduleRightClick = new System.Windows.Forms.ToolStripButton();
            this.txtWhenToWatchSynopsis = new System.Windows.Forms.TextBox();
            this.calCalendar = new System.Windows.Forms.MonthCalendar();
            this.lvWhenToWatch = new TVRename.ListViewFlickerFree();
            this.columnHeader29 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader30 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader31 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader32 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader36 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader33 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader34 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader35 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ilNewIcons = new System.Windows.Forms.ImageList(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.pbProgressBarx = new System.Windows.Forms.ProgressBar();
            this.txtDLStatusLabel = new System.Windows.Forms.Label();
            this.tsNextShowTxt = new System.Windows.Forms.Label();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader25 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader26 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader27 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader28 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.openFile = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.refreshWTWTimer = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.showRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.statusTimer = new System.Windows.Forms.Timer(this.components);
            this.BGDownloadTimer = new System.Windows.Forms.Timer(this.components);
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.saveFile = new System.Windows.Forms.SaveFileDialog();
            this.tmrShowUpcomingPopup = new System.Windows.Forms.Timer(this.components);
            this.quickTimer = new System.Windows.Forms.Timer(this.components);
            this.tmrPeriodicScan = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnUpdateAvailable = new System.Windows.Forms.Button();
            this.bwSeasonHTMLGenerator = new System.ComponentModel.BackgroundWorker();
            this.bwUpdateSchedule = new System.ComponentModel.BackgroundWorker();
            this.bwShowHTMLGenerator = new System.ComponentModel.BackgroundWorker();
            this.bwShowSummaryHTMLGenerator = new System.ComponentModel.BackgroundWorker();
            this.bwSeasonSummaryHTMLGenerator = new System.ComponentModel.BackgroundWorker();
            this.bwMovieHTMLGenerator = new System.ComponentModel.BackgroundWorker();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.movieRecommendationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tvRecommendationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tbMyMovies.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControl3.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tbMyShows.SuspendLayout();
            this.tsMyShows.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tpInformation.SuspendLayout();
            this.tpImages.SuspendLayout();
            this.tpSummary.SuspendLayout();
            this.tbAllInOne.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvAction)).BeginInit();
            this.tsScanResults.SuspendLayout();
            this.tbWTW.SuspendLayout();
            this.tsWtW.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.betaToolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(884, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.exportToolStripMenuItem.Text = "&Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::TVRename.Properties.Resources.saveHS;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(145, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.offlineOperationToolStripMenuItem,
            this.backgroundDownloadToolStripMenuItem,
            this.toolStripSeparator2,
            this.preferencesToolStripMenuItem,
            this.ignoreListToolStripMenuItem,
            this.filenameTemplateEditorToolStripMenuItem,
            this.movieSearchEnginesToolStripMenuItem,
            this.searchEnginesToolStripMenuItem,
            this.filenameProcessorsToolStripMenuItem,
            this.toolStripSeparator20,
            this.settingsCheckToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // offlineOperationToolStripMenuItem
            // 
            this.offlineOperationToolStripMenuItem.Name = "offlineOperationToolStripMenuItem";
            this.offlineOperationToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.offlineOperationToolStripMenuItem.Text = "&Offline Operation";
            this.offlineOperationToolStripMenuItem.ToolTipText = "If you turn this on, TVRename will only use data it has locally, without download" +
    "ing anything.";
            this.offlineOperationToolStripMenuItem.Click += new System.EventHandler(this.offlineOperationToolStripMenuItem_Click);
            // 
            // backgroundDownloadToolStripMenuItem
            // 
            this.backgroundDownloadToolStripMenuItem.Image = global::TVRename.Properties.Resources.GetLatestVersion;
            this.backgroundDownloadToolStripMenuItem.Name = "backgroundDownloadToolStripMenuItem";
            this.backgroundDownloadToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.backgroundDownloadToolStripMenuItem.Text = "Automatic &Background Download";
            this.backgroundDownloadToolStripMenuItem.ToolTipText = "Turn this on to let TVRename automatically download thetvdb.com data in the backg" +
    "round";
            this.backgroundDownloadToolStripMenuItem.Click += new System.EventHandler(this.backgroundDownloadToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(251, 6);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Image = global::TVRename.Properties.Resources.EditInformationHS;
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.preferencesToolStripMenuItem.Text = "&Preferences";
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
            // 
            // ignoreListToolStripMenuItem
            // 
            this.ignoreListToolStripMenuItem.Name = "ignoreListToolStripMenuItem";
            this.ignoreListToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.ignoreListToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.ignoreListToolStripMenuItem.Text = "&Ignore List";
            this.ignoreListToolStripMenuItem.Click += new System.EventHandler(this.ignoreListToolStripMenuItem_Click);
            // 
            // filenameTemplateEditorToolStripMenuItem
            // 
            this.filenameTemplateEditorToolStripMenuItem.Image = global::TVRename.Properties.Resources.FormulaEvaluatorHS;
            this.filenameTemplateEditorToolStripMenuItem.Name = "filenameTemplateEditorToolStripMenuItem";
            this.filenameTemplateEditorToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.filenameTemplateEditorToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.filenameTemplateEditorToolStripMenuItem.Text = "&Filename Template Editor";
            this.filenameTemplateEditorToolStripMenuItem.Click += new System.EventHandler(this.filenameTemplateEditorToolStripMenuItem_Click);
            // 
            // movieSearchEnginesToolStripMenuItem
            // 
            this.movieSearchEnginesToolStripMenuItem.Image = global::TVRename.Properties.Resources.SearchWebHS;
            this.movieSearchEnginesToolStripMenuItem.Name = "movieSearchEnginesToolStripMenuItem";
            this.movieSearchEnginesToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.movieSearchEnginesToolStripMenuItem.Text = "Movie &Search Engines";
            this.movieSearchEnginesToolStripMenuItem.Click += new System.EventHandler(this.movieSearchEnginesToolStripMenuItem_Click);
            // 
            // searchEnginesToolStripMenuItem
            // 
            this.searchEnginesToolStripMenuItem.Image = global::TVRename.Properties.Resources.SearchWebHS;
            this.searchEnginesToolStripMenuItem.Name = "searchEnginesToolStripMenuItem";
            this.searchEnginesToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.searchEnginesToolStripMenuItem.Text = "TV &Search Engines";
            this.searchEnginesToolStripMenuItem.Click += new System.EventHandler(this.searchEnginesToolStripMenuItem_Click);
            // 
            // filenameProcessorsToolStripMenuItem
            // 
            this.filenameProcessorsToolStripMenuItem.Name = "filenameProcessorsToolStripMenuItem";
            this.filenameProcessorsToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.filenameProcessorsToolStripMenuItem.Text = "File&name Processors";
            this.filenameProcessorsToolStripMenuItem.Click += new System.EventHandler(this.filenameProcessorsToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.flushCacheToolStripMenuItem,
            this.flushImageCacheToolStripMenuItem,
            this.backgroundDownloadNowToolStripMenuItem,
            this.toolStripSeparator17,
            this.folderMonitorToolStripMenuItem,
            this.bulkAddMoviesToolStripMenuItem,
            this.toolStripSeparator3,
            this.duplicateFinderLOGToolStripMenuItem,
            this.quickRenameToolStripMenuItem,
            this.tsmiOrphanFiles,
            this.duplicateMoviesToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // flushCacheToolStripMenuItem
            // 
            this.flushCacheToolStripMenuItem.Name = "flushCacheToolStripMenuItem";
            this.flushCacheToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.flushCacheToolStripMenuItem.Text = "&Force Refesh All";
            this.flushCacheToolStripMenuItem.Click += new System.EventHandler(this.flushCacheToolStripMenuItem_Click);
            // 
            // flushImageCacheToolStripMenuItem
            // 
            this.flushImageCacheToolStripMenuItem.Name = "flushImageCacheToolStripMenuItem";
            this.flushImageCacheToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.flushImageCacheToolStripMenuItem.Text = "&Force Refesh All Images";
            this.flushImageCacheToolStripMenuItem.Click += new System.EventHandler(this.flushImageCacheToolStripMenuItem_Click);
            // 
            // backgroundDownloadNowToolStripMenuItem
            // 
            this.backgroundDownloadNowToolStripMenuItem.Image = global::TVRename.Properties.Resources.RefreshDocViewHS;
            this.backgroundDownloadNowToolStripMenuItem.Name = "backgroundDownloadNowToolStripMenuItem";
            this.backgroundDownloadNowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.backgroundDownloadNowToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.backgroundDownloadNowToolStripMenuItem.Text = "&Background Download Now";
            this.backgroundDownloadNowToolStripMenuItem.Click += new System.EventHandler(this.backgroundDownloadNowToolStripMenuItem_Click);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(261, 6);
            // 
            // folderMonitorToolStripMenuItem
            // 
            this.folderMonitorToolStripMenuItem.Image = global::TVRename.Properties.Resources.SearchFolderHS;
            this.folderMonitorToolStripMenuItem.Name = "folderMonitorToolStripMenuItem";
            this.folderMonitorToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.folderMonitorToolStripMenuItem.Text = "Bulk &Add Shows...";
            this.folderMonitorToolStripMenuItem.Click += new System.EventHandler(this.folderMonitorToolStripMenuItem_Click);
            // 
            // bulkAddMoviesToolStripMenuItem
            // 
            this.bulkAddMoviesToolStripMenuItem.Name = "bulkAddMoviesToolStripMenuItem";
            this.bulkAddMoviesToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.bulkAddMoviesToolStripMenuItem.Text = "Bulk Add Movies...";
            this.bulkAddMoviesToolStripMenuItem.Click += new System.EventHandler(this.bulkAddMoviesToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(261, 6);
            // 
            // duplicateFinderLOGToolStripMenuItem
            // 
            this.duplicateFinderLOGToolStripMenuItem.Name = "duplicateFinderLOGToolStripMenuItem";
            this.duplicateFinderLOGToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.duplicateFinderLOGToolStripMenuItem.Text = "Merged Episode Finder...";
            this.duplicateFinderLOGToolStripMenuItem.Click += new System.EventHandler(this.duplicateFinderLOGToolStripMenuItem_Click);
            // 
            // quickRenameToolStripMenuItem
            // 
            this.quickRenameToolStripMenuItem.Name = "quickRenameToolStripMenuItem";
            this.quickRenameToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.quickRenameToolStripMenuItem.Text = "Quick Rename...";
            this.quickRenameToolStripMenuItem.Click += new System.EventHandler(this.QuickRenameToolStripMenuItem_Click);
            // 
            // tsmiOrphanFiles
            // 
            this.tsmiOrphanFiles.Name = "tsmiOrphanFiles";
            this.tsmiOrphanFiles.Size = new System.Drawing.Size(264, 22);
            this.tsmiOrphanFiles.Text = "Orphan Media Files....";
            this.tsmiOrphanFiles.Click += new System.EventHandler(this.ToolStripMenuItem1_Click);
            // 
            // duplicateMoviesToolStripMenuItem
            // 
            this.duplicateMoviesToolStripMenuItem.Name = "duplicateMoviesToolStripMenuItem";
            this.duplicateMoviesToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.duplicateMoviesToolStripMenuItem.Text = "Duplicate Movies...";
            this.duplicateMoviesToolStripMenuItem.Click += new System.EventHandler(this.duplicateMoviesToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statisticsToolStripMenuItem,
            this.toolStripSeparator5,
            this.showSummaryToolStripMenuItem,
            this.movieCollectionSummaryLogToolStripMenuItem,
            this.actorsToolStripMenuItem,
            this.toolStripSeparator18});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // statisticsToolStripMenuItem
            // 
            this.statisticsToolStripMenuItem.Image = global::TVRename.Properties.Resources.graphhs;
            this.statisticsToolStripMenuItem.Name = "statisticsToolStripMenuItem";
            this.statisticsToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.statisticsToolStripMenuItem.Text = "&Statistics...";
            this.statisticsToolStripMenuItem.Click += new System.EventHandler(this.statisticsToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(224, 6);
            // 
            // showSummaryToolStripMenuItem
            // 
            this.showSummaryToolStripMenuItem.Name = "showSummaryToolStripMenuItem";
            this.showSummaryToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.showSummaryToolStripMenuItem.Text = "Show Summary...";
            this.showSummaryToolStripMenuItem.Click += new System.EventHandler(this.showSummaryToolStripMenuItem_Click);
            // 
            // movieCollectionSummaryLogToolStripMenuItem
            // 
            this.movieCollectionSummaryLogToolStripMenuItem.Name = "movieCollectionSummaryLogToolStripMenuItem";
            this.movieCollectionSummaryLogToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.movieCollectionSummaryLogToolStripMenuItem.Text = "Movie Collection Summary...";
            this.movieCollectionSummaryLogToolStripMenuItem.Click += new System.EventHandler(this.movieCollectionSummaryLogToolStripMenuItem_Click);
            // 
            // actorsToolStripMenuItem
            // 
            this.actorsToolStripMenuItem.Image = global::TVRename.Properties.Resources.TableHS;
            this.actorsToolStripMenuItem.Name = "actorsToolStripMenuItem";
            this.actorsToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.actorsToolStripMenuItem.Text = "TV &Actors Grid...";
            this.actorsToolStripMenuItem.Click += new System.EventHandler(this.actorsToolStripMenuItem_Click);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            this.toolStripSeparator18.Size = new System.Drawing.Size(224, 6);
            // 
            // betaToolsToolStripMenuItem
            // 
            this.betaToolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.timezoneInconsistencyLOGToolStripMenuItem,
            this.episodeFileQualitySummaryLogToolStripMenuItem,
            this.toolStripSeparator19,
            this.accuracyCheckLogToolStripMenuItem,
            this.tMDBAccuracyCheckLogToolStripMenuItem,
            this.toolStripSeparator21,
            this.movieRecommendationsToolStripMenuItem,
            this.tvRecommendationsToolStripMenuItem});
            this.betaToolsToolStripMenuItem.Name = "betaToolsToolStripMenuItem";
            this.betaToolsToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.betaToolsToolStripMenuItem.Text = "Beta";
            // 
            // timezoneInconsistencyLOGToolStripMenuItem
            // 
            this.timezoneInconsistencyLOGToolStripMenuItem.Name = "timezoneInconsistencyLOGToolStripMenuItem";
            this.timezoneInconsistencyLOGToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.timezoneInconsistencyLOGToolStripMenuItem.Text = "Timezone Inconsistency (Log)";
            this.timezoneInconsistencyLOGToolStripMenuItem.Click += new System.EventHandler(this.timezoneInconsistencyLOGToolStripMenuItem_Click);
            // 
            // episodeFileQualitySummaryLogToolStripMenuItem
            // 
            this.episodeFileQualitySummaryLogToolStripMenuItem.Name = "episodeFileQualitySummaryLogToolStripMenuItem";
            this.episodeFileQualitySummaryLogToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.episodeFileQualitySummaryLogToolStripMenuItem.Text = "Episode File Quality Summary (Log)";
            this.episodeFileQualitySummaryLogToolStripMenuItem.Click += new System.EventHandler(this.episodeFileQualitySummaryLogToolStripMenuItem_Click);
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            this.toolStripSeparator19.Size = new System.Drawing.Size(259, 6);
            // 
            // accuracyCheckLogToolStripMenuItem
            // 
            this.accuracyCheckLogToolStripMenuItem.Name = "accuracyCheckLogToolStripMenuItem";
            this.accuracyCheckLogToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.accuracyCheckLogToolStripMenuItem.Text = "TVDB Accuracy Check (Log)";
            this.accuracyCheckLogToolStripMenuItem.Click += new System.EventHandler(this.AccuracyCheckLogToolStripMenuItem_Click);
            // 
            // tMDBAccuracyCheckLogToolStripMenuItem
            // 
            this.tMDBAccuracyCheckLogToolStripMenuItem.Name = "tMDBAccuracyCheckLogToolStripMenuItem";
            this.tMDBAccuracyCheckLogToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.tMDBAccuracyCheckLogToolStripMenuItem.Text = "TMDB Accuracy Check (Log)";
            this.tMDBAccuracyCheckLogToolStripMenuItem.Click += new System.EventHandler(this.tMDBAccuracyCheckLogToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.quickstartGuideToolStripMenuItem,
            this.visitWebsiteToolStripMenuItem,
            this.visitSupportForumToolStripMenuItem,
            this.bugReportToolStripMenuItem,
            this.toolStripSeparator7,
            this.buyMeADrinkToolStripMenuItem,
            this.toolStripSeparator6,
            this.checkForNewVersionToolStripMenuItem,
            this.toolStripSeparator8,
            this.logToolStripMenuItem,
            this.thanksToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // quickstartGuideToolStripMenuItem
            // 
            this.quickstartGuideToolStripMenuItem.Image = global::TVRename.Properties.Resources.Help;
            this.quickstartGuideToolStripMenuItem.Name = "quickstartGuideToolStripMenuItem";
            this.quickstartGuideToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.quickstartGuideToolStripMenuItem.Text = "&Quickstart Guide";
            this.quickstartGuideToolStripMenuItem.Click += new System.EventHandler(this.quickstartGuideToolStripMenuItem_Click);
            // 
            // visitWebsiteToolStripMenuItem
            // 
            this.visitWebsiteToolStripMenuItem.Image = global::TVRename.Properties.Resources.Web;
            this.visitWebsiteToolStripMenuItem.Name = "visitWebsiteToolStripMenuItem";
            this.visitWebsiteToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.visitWebsiteToolStripMenuItem.Text = "&Visit Website";
            this.visitWebsiteToolStripMenuItem.Click += new System.EventHandler(this.visitWebsiteToolStripMenuItem_Click);
            // 
            // visitSupportForumToolStripMenuItem
            // 
            this.visitSupportForumToolStripMenuItem.Name = "visitSupportForumToolStripMenuItem";
            this.visitSupportForumToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.visitSupportForumToolStripMenuItem.Text = "Visit Support Forum";
            this.visitSupportForumToolStripMenuItem.Click += new System.EventHandler(this.visitSupportForumToolStripMenuItem_Click);
            // 
            // bugReportToolStripMenuItem
            // 
            this.bugReportToolStripMenuItem.Name = "bugReportToolStripMenuItem";
            this.bugReportToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.bugReportToolStripMenuItem.Text = "Bug &Report";
            this.bugReportToolStripMenuItem.Click += new System.EventHandler(this.bugReportToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(192, 6);
            // 
            // buyMeADrinkToolStripMenuItem
            // 
            this.buyMeADrinkToolStripMenuItem.Name = "buyMeADrinkToolStripMenuItem";
            this.buyMeADrinkToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.buyMeADrinkToolStripMenuItem.Text = "&Buy Me A Drink";
            this.buyMeADrinkToolStripMenuItem.Click += new System.EventHandler(this.buyMeADrinkToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(192, 6);
            // 
            // checkForNewVersionToolStripMenuItem
            // 
            this.checkForNewVersionToolStripMenuItem.Name = "checkForNewVersionToolStripMenuItem";
            this.checkForNewVersionToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.checkForNewVersionToolStripMenuItem.Text = "Check For New Version";
            this.checkForNewVersionToolStripMenuItem.Click += new System.EventHandler(this.checkForNewVersionToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(192, 6);
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.logToolStripMenuItem.Text = "Log";
            this.logToolStripMenuItem.Click += new System.EventHandler(this.logToolStripMenuItem_Click);
            // 
            // thanksToolStripMenuItem
            // 
            this.thanksToolStripMenuItem.Name = "thanksToolStripMenuItem";
            this.thanksToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.thanksToolStripMenuItem.Text = "Thanks";
            this.thanksToolStripMenuItem.Click += new System.EventHandler(this.ThanksToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tbMyMovies);
            this.tabControl1.Controls.Add(this.tbMyShows);
            this.tabControl1.Controls.Add(this.tbAllInOne);
            this.tabControl1.Controls.Add(this.tbWTW);
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.ImageList = this.ilNewIcons;
            this.tabControl1.ItemSize = new System.Drawing.Size(100, 100);
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(884, 483);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 0;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.TabControl1_DrawItem);
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.DoubleClick += new System.EventHandler(this.tabControl1_DoubleClick);
            // 
            // tbMyMovies
            // 
            this.tbMyMovies.Controls.Add(this.toolStrip1);
            this.tbMyMovies.Controls.Add(this.splitContainer2);
            this.tbMyMovies.ImageKey = "4632196-48.png";
            this.tbMyMovies.Location = new System.Drawing.Point(104, 4);
            this.tbMyMovies.Name = "tbMyMovies";
            this.tbMyMovies.Padding = new System.Windows.Forms.Padding(3);
            this.tbMyMovies.Size = new System.Drawing.Size(776, 475);
            this.tbMyMovies.TabIndex = 12;
            this.tbMyMovies.Text = "Movies";
            this.tbMyMovies.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbnAddMovie,
            this.btnEditMovie,
            this.btnMovieDelete,
            this.toolStripSeparator15,
            this.btnMovieRefresh,
            this.tsbScanMovies,
            this.btnMovieFilter,
            this.toolStripSeparator16,
            this.tsbMyMoviesContextMenu});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(770, 39);
            this.toolStrip1.TabIndex = 13;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tbnAddMovie
            // 
            this.tbnAddMovie.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbnAddMovie.Image = global::TVRename.Properties.Resources._226562_32;
            this.tbnAddMovie.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbnAddMovie.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbnAddMovie.Name = "tbnAddMovie";
            this.tbnAddMovie.Size = new System.Drawing.Size(68, 36);
            this.tbnAddMovie.Text = "&Add";
            this.tbnAddMovie.Click += new System.EventHandler(this.AddMovie_Click);
            // 
            // btnEditMovie
            // 
            this.btnEditMovie.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditMovie.Image = global::TVRename.Properties.Resources._314251_32;
            this.btnEditMovie.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnEditMovie.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditMovie.Name = "btnEditMovie";
            this.btnEditMovie.Size = new System.Drawing.Size(66, 36);
            this.btnEditMovie.Text = "&Edit";
            this.btnEditMovie.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // btnMovieDelete
            // 
            this.btnMovieDelete.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMovieDelete.Image = global::TVRename.Properties.Resources._616650_32;
            this.btnMovieDelete.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnMovieDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMovieDelete.Name = "btnMovieDelete";
            this.btnMovieDelete.Size = new System.Drawing.Size(81, 36);
            this.btnMovieDelete.Text = "&Delete";
            this.btnMovieDelete.Click += new System.EventHandler(this.btnMovieDelete_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(6, 39);
            // 
            // btnMovieRefresh
            // 
            this.btnMovieRefresh.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMovieRefresh.Image = global::TVRename.Properties.Resources._134221_32;
            this.btnMovieRefresh.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnMovieRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMovieRefresh.Name = "btnMovieRefresh";
            this.btnMovieRefresh.Size = new System.Drawing.Size(88, 36);
            this.btnMovieRefresh.Text = "&Refresh";
            this.btnMovieRefresh.Click += new System.EventHandler(this.btnMovieRefresh_Click);
            // 
            // tsbScanMovies
            // 
            this.tsbScanMovies.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.tsbScanMovies.Image = global::TVRename.Properties.Resources._322497_321;
            this.tsbScanMovies.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbScanMovies.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbScanMovies.Name = "tsbScanMovies";
            this.tsbScanMovies.Size = new System.Drawing.Size(117, 36);
            this.tsbScanMovies.Text = "Scan Movies";
            this.tsbScanMovies.Click += new System.EventHandler(this.toolStripButton1_Click_1);
            // 
            // btnMovieFilter
            // 
            this.btnMovieFilter.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMovieFilter.Image = global::TVRename.Properties.Resources._4781834_32;
            this.btnMovieFilter.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnMovieFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMovieFilter.Name = "btnMovieFilter";
            this.btnMovieFilter.Size = new System.Drawing.Size(72, 36);
            this.btnMovieFilter.Text = "&Filter";
            this.btnMovieFilter.Click += new System.EventHandler(this.btnMovieFilter_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(6, 39);
            // 
            // tsbMyMoviesContextMenu
            // 
            this.tsbMyMoviesContextMenu.Image = global::TVRename.Properties.Resources._314251_32;
            this.tsbMyMoviesContextMenu.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbMyMoviesContextMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbMyMoviesContextMenu.Name = "tsbMyMoviesContextMenu";
            this.tsbMyMoviesContextMenu.Size = new System.Drawing.Size(119, 36);
            this.tsbMyMoviesContextMenu.Text = "Context Menu";
            this.tsbMyMoviesContextMenu.Click += new System.EventHandler(this.tsbMyMoviesContextMenu_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(2, 43);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.movieTree);
            this.splitContainer2.Panel1.Controls.Add(this.filterMoviesTextbox);
            this.splitContainer2.Panel1MinSize = 100;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabControl3);
            this.splitContainer2.Panel2MinSize = 100;
            this.splitContainer2.Size = new System.Drawing.Size(773, 429);
            this.splitContainer2.SplitterDistance = 280;
            this.splitContainer2.TabIndex = 12;
            // 
            // movieTree
            // 
            this.movieTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.movieTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.movieTree.HideSelection = false;
            this.movieTree.Location = new System.Drawing.Point(0, 20);
            this.movieTree.Name = "movieTree";
            this.movieTree.Size = new System.Drawing.Size(276, 405);
            this.movieTree.TabIndex = 0;
            this.movieTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.MyMoviesTree_AfterSelect);
            this.movieTree.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MyMoviesTree_MouseClick);
            // 
            // filterMoviesTextbox
            // 
            this.filterMoviesTextbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.filterMoviesTextbox.Location = new System.Drawing.Point(0, 0);
            this.filterMoviesTextbox.Name = "filterMoviesTextbox";
            this.filterMoviesTextbox.Size = new System.Drawing.Size(276, 20);
            this.filterMoviesTextbox.TabIndex = 1;
            this.filterMoviesTextbox.SizeChanged += new System.EventHandler(this.filterMoviesTextBox_SizeChanged);
            this.filterMoviesTextbox.TextChanged += new System.EventHandler(this.filterMoviesTextBox_TextChanged);
            // 
            // tabControl3
            // 
            this.tabControl3.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl3.Controls.Add(this.tabPage1);
            this.tabControl3.Controls.Add(this.tabPage2);
            this.tabControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl3.Location = new System.Drawing.Point(0, 0);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.Size = new System.Drawing.Size(485, 425);
            this.tabControl3.TabIndex = 7;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.webMovieInformation);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(477, 396);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Information";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // webMovieInformation
            // 
            this.webMovieInformation.AllowWebBrowserDrop = false;
            this.webMovieInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webMovieInformation.Location = new System.Drawing.Point(3, 3);
            this.webMovieInformation.MinimumSize = new System.Drawing.Size(20, 20);
            this.webMovieInformation.Name = "webMovieInformation";
            this.webMovieInformation.Size = new System.Drawing.Size(471, 390);
            this.webMovieInformation.TabIndex = 0;
            this.webMovieInformation.WebBrowserShortcutsEnabled = false;
            this.webMovieInformation.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.NavigateTo);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.webMovieImages);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(477, 396);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Images";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // webMovieImages
            // 
            this.webMovieImages.AllowWebBrowserDrop = false;
            this.webMovieImages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webMovieImages.Location = new System.Drawing.Point(3, 3);
            this.webMovieImages.MinimumSize = new System.Drawing.Size(20, 20);
            this.webMovieImages.Name = "webMovieImages";
            this.webMovieImages.Size = new System.Drawing.Size(471, 390);
            this.webMovieImages.TabIndex = 0;
            this.webMovieImages.WebBrowserShortcutsEnabled = false;
            this.webMovieImages.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.NavigateTo);
            // 
            // tbMyShows
            // 
            this.tbMyShows.Controls.Add(this.tsMyShows);
            this.tbMyShows.Controls.Add(this.splitContainer1);
            this.tbMyShows.ImageKey = "3790574-48.png";
            this.tbMyShows.Location = new System.Drawing.Point(104, 4);
            this.tbMyShows.Name = "tbMyShows";
            this.tbMyShows.Padding = new System.Windows.Forms.Padding(3);
            this.tbMyShows.Size = new System.Drawing.Size(776, 475);
            this.tbMyShows.TabIndex = 9;
            this.tbMyShows.Text = "TV Shows";
            this.tbMyShows.UseVisualStyleBackColor = true;
            // 
            // tsMyShows
            // 
            this.tsMyShows.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsMyShows.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddTVShow,
            this.btnEditShow,
            this.btnRemoveShow,
            this.btnHideHTMLPanel,
            this.btnMyShowsCollapse,
            this.toolStripSeparator4,
            this.btnMyShowsRefresh,
            this.btnFilterMyShows,
            this.toolStripSeparator13,
            this.tsbMyShowsContextMenu});
            this.tsMyShows.Location = new System.Drawing.Point(3, 3);
            this.tsMyShows.Name = "tsMyShows";
            this.tsMyShows.Size = new System.Drawing.Size(770, 39);
            this.tsMyShows.TabIndex = 11;
            this.tsMyShows.Text = "toolStrip1";
            // 
            // btnAddTVShow
            // 
            this.btnAddTVShow.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddTVShow.Image = global::TVRename.Properties.Resources._226562_32;
            this.btnAddTVShow.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnAddTVShow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddTVShow.Name = "btnAddTVShow";
            this.btnAddTVShow.Size = new System.Drawing.Size(68, 36);
            this.btnAddTVShow.Text = "&Add";
            this.btnAddTVShow.Click += new System.EventHandler(this.bnMyShowsAdd_Click);
            // 
            // btnEditShow
            // 
            this.btnEditShow.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditShow.Image = global::TVRename.Properties.Resources._314251_32;
            this.btnEditShow.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnEditShow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditShow.Name = "btnEditShow";
            this.btnEditShow.Size = new System.Drawing.Size(66, 36);
            this.btnEditShow.Text = "&Edit";
            this.btnEditShow.Click += new System.EventHandler(this.bnMyShowsEdit_Click);
            // 
            // btnRemoveShow
            // 
            this.btnRemoveShow.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemoveShow.Image = global::TVRename.Properties.Resources._616650_32;
            this.btnRemoveShow.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnRemoveShow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemoveShow.Name = "btnRemoveShow";
            this.btnRemoveShow.Size = new System.Drawing.Size(81, 36);
            this.btnRemoveShow.Text = "&Delete";
            this.btnRemoveShow.Click += new System.EventHandler(this.bnMyShowsDelete_Click);
            // 
            // btnHideHTMLPanel
            // 
            this.btnHideHTMLPanel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnHideHTMLPanel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnHideHTMLPanel.Image = global::TVRename.Properties.Resources.FillRight;
            this.btnHideHTMLPanel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnHideHTMLPanel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnHideHTMLPanel.Name = "btnHideHTMLPanel";
            this.btnHideHTMLPanel.Size = new System.Drawing.Size(23, 36);
            this.btnHideHTMLPanel.Text = "Hide Details";
            this.btnHideHTMLPanel.Click += new System.EventHandler(this.ToolStripButton5_Click);
            // 
            // btnMyShowsCollapse
            // 
            this.btnMyShowsCollapse.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnMyShowsCollapse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnMyShowsCollapse.Image = global::TVRename.Properties.Resources.TreeView;
            this.btnMyShowsCollapse.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnMyShowsCollapse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMyShowsCollapse.Name = "btnMyShowsCollapse";
            this.btnMyShowsCollapse.Size = new System.Drawing.Size(23, 36);
            this.btnMyShowsCollapse.Text = "Collapse";
            this.btnMyShowsCollapse.Click += new System.EventHandler(this.BtnMyShowsCollapse_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 39);
            // 
            // btnMyShowsRefresh
            // 
            this.btnMyShowsRefresh.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMyShowsRefresh.Image = global::TVRename.Properties.Resources._134221_32;
            this.btnMyShowsRefresh.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnMyShowsRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMyShowsRefresh.Name = "btnMyShowsRefresh";
            this.btnMyShowsRefresh.Size = new System.Drawing.Size(88, 36);
            this.btnMyShowsRefresh.Text = "&Refresh";
            this.btnMyShowsRefresh.Click += new System.EventHandler(this.bnMyShowsRefresh_Click);
            // 
            // btnFilterMyShows
            // 
            this.btnFilterMyShows.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilterMyShows.Image = global::TVRename.Properties.Resources._4781834_32;
            this.btnFilterMyShows.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnFilterMyShows.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFilterMyShows.Name = "btnFilterMyShows";
            this.btnFilterMyShows.Size = new System.Drawing.Size(72, 36);
            this.btnFilterMyShows.Text = "&Filter";
            this.btnFilterMyShows.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(6, 39);
            // 
            // tsbMyShowsContextMenu
            // 
            this.tsbMyShowsContextMenu.Image = global::TVRename.Properties.Resources._314251_32;
            this.tsbMyShowsContextMenu.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbMyShowsContextMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbMyShowsContextMenu.Name = "tsbMyShowsContextMenu";
            this.tsbMyShowsContextMenu.Size = new System.Drawing.Size(119, 36);
            this.tsbMyShowsContextMenu.Text = "Context Menu";
            this.tsbMyShowsContextMenu.Click += new System.EventHandler(this.TsbMyShowsContextMenu_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(3, 43);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.MyShowTree);
            this.splitContainer1.Panel1.Controls.Add(this.filterTextBox);
            this.splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl2);
            this.splitContainer1.Panel2MinSize = 100;
            this.splitContainer1.Size = new System.Drawing.Size(773, 429);
            this.splitContainer1.SplitterDistance = 280;
            this.splitContainer1.TabIndex = 8;
            // 
            // MyShowTree
            // 
            this.MyShowTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.MyShowTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MyShowTree.HideSelection = false;
            this.MyShowTree.Location = new System.Drawing.Point(0, 20);
            this.MyShowTree.Name = "MyShowTree";
            this.MyShowTree.Size = new System.Drawing.Size(276, 405);
            this.MyShowTree.TabIndex = 0;
            this.MyShowTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.MyShowTree_AfterSelect);
            this.MyShowTree.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MyShowTree_MouseClick);
            // 
            // filterTextBox
            // 
            this.filterTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.filterTextBox.Location = new System.Drawing.Point(0, 0);
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new System.Drawing.Size(276, 20);
            this.filterTextBox.TabIndex = 1;
            this.filterTextBox.SizeChanged += new System.EventHandler(this.filterTextBox_SizeChanged);
            this.filterTextBox.TextChanged += new System.EventHandler(this.filterTextBox_TextChanged);
            // 
            // tabControl2
            // 
            this.tabControl2.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl2.Controls.Add(this.tpInformation);
            this.tabControl2.Controls.Add(this.tpImages);
            this.tabControl2.Controls.Add(this.tpSummary);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(485, 425);
            this.tabControl2.TabIndex = 7;
            // 
            // tpInformation
            // 
            this.tpInformation.Controls.Add(this.webInformation);
            this.tpInformation.Location = new System.Drawing.Point(4, 25);
            this.tpInformation.Name = "tpInformation";
            this.tpInformation.Padding = new System.Windows.Forms.Padding(3);
            this.tpInformation.Size = new System.Drawing.Size(477, 396);
            this.tpInformation.TabIndex = 0;
            this.tpInformation.Text = "Information";
            this.tpInformation.UseVisualStyleBackColor = true;
            // 
            // webInformation
            // 
            this.webInformation.AllowWebBrowserDrop = false;
            this.webInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webInformation.Location = new System.Drawing.Point(3, 3);
            this.webInformation.MinimumSize = new System.Drawing.Size(20, 20);
            this.webInformation.Name = "webInformation";
            this.webInformation.Size = new System.Drawing.Size(471, 390);
            this.webInformation.TabIndex = 0;
            this.webInformation.WebBrowserShortcutsEnabled = false;
            this.webInformation.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.NavigateTo);
            // 
            // tpImages
            // 
            this.tpImages.Controls.Add(this.webImages);
            this.tpImages.Location = new System.Drawing.Point(4, 25);
            this.tpImages.Name = "tpImages";
            this.tpImages.Padding = new System.Windows.Forms.Padding(3);
            this.tpImages.Size = new System.Drawing.Size(477, 396);
            this.tpImages.TabIndex = 1;
            this.tpImages.Text = "Images";
            this.tpImages.UseVisualStyleBackColor = true;
            // 
            // webImages
            // 
            this.webImages.AllowWebBrowserDrop = false;
            this.webImages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webImages.Location = new System.Drawing.Point(3, 3);
            this.webImages.MinimumSize = new System.Drawing.Size(20, 20);
            this.webImages.Name = "webImages";
            this.webImages.Size = new System.Drawing.Size(471, 390);
            this.webImages.TabIndex = 0;
            this.webImages.WebBrowserShortcutsEnabled = false;
            this.webImages.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.NavigateTo);
            // 
            // tpSummary
            // 
            this.tpSummary.Controls.Add(this.webSummary);
            this.tpSummary.Location = new System.Drawing.Point(4, 25);
            this.tpSummary.Name = "tpSummary";
            this.tpSummary.Padding = new System.Windows.Forms.Padding(3);
            this.tpSummary.Size = new System.Drawing.Size(477, 396);
            this.tpSummary.TabIndex = 2;
            this.tpSummary.Text = "Summary";
            this.tpSummary.UseVisualStyleBackColor = true;
            // 
            // webSummary
            // 
            this.webSummary.AllowWebBrowserDrop = false;
            this.webSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webSummary.Location = new System.Drawing.Point(3, 3);
            this.webSummary.MinimumSize = new System.Drawing.Size(20, 20);
            this.webSummary.Name = "webSummary";
            this.webSummary.Size = new System.Drawing.Size(471, 390);
            this.webSummary.TabIndex = 1;
            this.webSummary.WebBrowserShortcutsEnabled = false;
            this.webSummary.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.NavigateTo);
            // 
            // tbAllInOne
            // 
            this.tbAllInOne.Controls.Add(this.olvAction);
            this.tbAllInOne.Controls.Add(this.tsScanResults);
            this.tbAllInOne.ImageKey = "322497-48 (1).png";
            this.tbAllInOne.Location = new System.Drawing.Point(104, 4);
            this.tbAllInOne.Name = "tbAllInOne";
            this.tbAllInOne.Padding = new System.Windows.Forms.Padding(3);
            this.tbAllInOne.Size = new System.Drawing.Size(776, 475);
            this.tbAllInOne.TabIndex = 11;
            this.tbAllInOne.Text = "Scan";
            this.tbAllInOne.UseVisualStyleBackColor = true;
            // 
            // olvAction
            // 
            this.olvAction.AllColumns.Add(this.olvShowColumn);
            this.olvAction.AllColumns.Add(this.olvSeason);
            this.olvAction.AllColumns.Add(this.olvEpisode);
            this.olvAction.AllColumns.Add(this.olvDate);
            this.olvAction.AllColumns.Add(this.olvFolder);
            this.olvAction.AllColumns.Add(this.olvFilename);
            this.olvAction.AllColumns.Add(this.olvSource);
            this.olvAction.AllColumns.Add(this.olvErrors);
            this.olvAction.AllColumns.Add(this.olvType);
            this.olvAction.AllowColumnReorder = true;
            this.olvAction.CellEditUseWholeCell = false;
            this.olvAction.CheckBoxes = true;
            this.olvAction.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvShowColumn,
            this.olvSeason,
            this.olvEpisode,
            this.olvDate,
            this.olvFolder,
            this.olvFilename,
            this.olvSource,
            this.olvErrors});
            this.olvAction.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvAction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvAction.FullRowSelect = true;
            this.olvAction.GroupWithItemCountFormat = "{0} ({1} items)";
            this.olvAction.GroupWithItemCountSingularFormat = "{0} (1 Item)";
            this.olvAction.HideSelection = false;
            this.olvAction.IncludeColumnHeadersInCopy = true;
            this.olvAction.IsSimpleDropSink = true;
            this.olvAction.Location = new System.Drawing.Point(3, 48);
            this.olvAction.Name = "olvAction";
            this.olvAction.ShowCommandMenuOnRightClick = true;
            this.olvAction.ShowItemCountOnGroups = true;
            this.olvAction.ShowItemToolTips = true;
            this.olvAction.Size = new System.Drawing.Size(770, 424);
            this.olvAction.SmallImageList = this.ilIcons;
            this.olvAction.TabIndex = 0;
            this.olvAction.UseCompatibleStateImageBehavior = false;
            this.olvAction.UseFilterIndicator = true;
            this.olvAction.UseFiltering = true;
            this.olvAction.UseNotifyPropertyChanged = true;
            this.olvAction.View = System.Windows.Forms.View.Details;
            this.olvAction.BeforeCreatingGroups += new System.EventHandler<BrightIdeasSoftware.CreateGroupsEventArgs>(this.olvAction_BeforeCreatingGroups);
            this.olvAction.CanDrop += new System.EventHandler<BrightIdeasSoftware.OlvDropEventArgs>(this.OlvAction_CanDrop);
            this.olvAction.Dropped += new System.EventHandler<BrightIdeasSoftware.OlvDropEventArgs>(this.OlvAction_Dropped);
            this.olvAction.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.olv1_FormatRow);
            this.olvAction.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.olvAction_ItemCheck);
            this.olvAction.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvAction_ItemChecked);
            this.olvAction.SelectedIndexChanged += new System.EventHandler(this.lvAction_SelectedIndexChanged);
            this.olvAction.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvAction_KeyDown);
            this.olvAction.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvAction_MouseClick);
            this.olvAction.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvAction_MouseDoubleClick);
            // 
            // olvShowColumn
            // 
            this.olvShowColumn.AspectName = "SeriesName";
            this.olvShowColumn.GroupWithItemCountFormat = "{0} ({1} items)";
            this.olvShowColumn.GroupWithItemCountSingularFormat = "{0} (1 Item)";
            this.olvShowColumn.Hideable = false;
            this.olvShowColumn.MinimumWidth = 10;
            this.olvShowColumn.Text = "Show";
            // 
            // olvSeason
            // 
            this.olvSeason.AspectName = "SeasonNumber";
            this.olvSeason.GroupWithItemCountFormat = "{0} ({1} items)";
            this.olvSeason.GroupWithItemCountSingularFormat = "{0} (1 Item)";
            this.olvSeason.IsEditable = false;
            this.olvSeason.MinimumWidth = 10;
            this.olvSeason.Searchable = false;
            this.olvSeason.Text = "Season";
            // 
            // olvEpisode
            // 
            this.olvEpisode.AspectName = "EpisodeNumber";
            this.olvEpisode.Groupable = false;
            this.olvEpisode.GroupWithItemCountFormat = "{0} ({1} items)";
            this.olvEpisode.GroupWithItemCountSingularFormat = "{0} (1 Item)";
            this.olvEpisode.IsEditable = false;
            this.olvEpisode.MinimumWidth = 10;
            this.olvEpisode.Text = "Episode";
            // 
            // olvDate
            // 
            this.olvDate.AspectName = "AirDate";
            this.olvDate.AspectToStringFormat = "{0:d}";
            this.olvDate.GroupWithItemCountFormat = "{0} ({1} items)";
            this.olvDate.GroupWithItemCountSingularFormat = "{0} (1 Item)";
            this.olvDate.MinimumWidth = 10;
            this.olvDate.Text = "Date";
            // 
            // olvFolder
            // 
            this.olvFolder.AspectName = "DestinationFolder";
            this.olvFolder.GroupWithItemCountFormat = "{0} ({1} items)";
            this.olvFolder.GroupWithItemCountSingularFormat = "{0} (1 Item)";
            this.olvFolder.MinimumWidth = 10;
            this.olvFolder.Text = "Folder";
            // 
            // olvFilename
            // 
            this.olvFilename.AspectName = "DestinationFile";
            this.olvFilename.Groupable = false;
            this.olvFilename.GroupWithItemCountFormat = "{0} ({1} items)";
            this.olvFilename.GroupWithItemCountSingularFormat = "{0} (1 Item)";
            this.olvFilename.MinimumWidth = 10;
            this.olvFilename.Text = "Filename";
            // 
            // olvSource
            // 
            this.olvSource.AspectName = "SourceDetails";
            this.olvSource.Groupable = false;
            this.olvSource.GroupWithItemCountFormat = "{0} ({1} items)";
            this.olvSource.GroupWithItemCountSingularFormat = "{0} (1 Item)";
            this.olvSource.MinimumWidth = 10;
            this.olvSource.Text = "Source";
            // 
            // olvErrors
            // 
            this.olvErrors.AspectName = "ErrorText";
            this.olvErrors.Groupable = false;
            this.olvErrors.GroupWithItemCountFormat = "{0} ({1} items)";
            this.olvErrors.GroupWithItemCountSingularFormat = "{0} (1 Item)";
            this.olvErrors.MinimumWidth = 30;
            this.olvErrors.Text = "Errors";
            // 
            // olvType
            // 
            this.olvType.AspectName = "Name";
            this.olvType.DisplayIndex = 8;
            this.olvType.GroupWithItemCountFormat = "{0}";
            this.olvType.GroupWithItemCountSingularFormat = "{0}";
            this.olvType.IsVisible = false;
            this.olvType.Text = "Type";
            // 
            // ilIcons
            // 
            this.ilIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilIcons.ImageStream")));
            this.ilIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.ilIcons.Images.SetKeyName(0, "OnDisk.bmp");
            this.ilIcons.Images.SetKeyName(1, "MagGlass.bmp");
            this.ilIcons.Images.SetKeyName(2, "uTorrent.bmp");
            this.ilIcons.Images.SetKeyName(3, "copy.bmp");
            this.ilIcons.Images.SetKeyName(4, "move.bmp");
            this.ilIcons.Images.SetKeyName(5, "download.bmp");
            this.ilIcons.Images.SetKeyName(6, "RSS.bmp");
            this.ilIcons.Images.SetKeyName(7, "NFO.bmp");
            this.ilIcons.Images.SetKeyName(8, "sab.png");
            this.ilIcons.Images.SetKeyName(9, "tk1[1].png");
            this.ilIcons.Images.SetKeyName(10, "qBitTorrent.png");
            // 
            // tsScanResults
            // 
            this.tsScanResults.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsScanResults.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnScan,
            this.tbFullScan,
            this.tpRecentScan,
            this.tbQuickScan,
            this.toolStripSeparator11,
            this.btnActionBTSearch,
            this.tbActionJackettSearch,
            this.toolStripSeparator9,
            this.btnIgnoreSelectedActions,
            this.btnRemoveSelActions,
            this.toolStripDropDownButton1,
            this.toolStripSeparator10,
            this.btnActionAction,
            this.btnRevertView,
            this.btnPreferences,
            this.toolStripSeparator14,
            this.tsbScanContextMenu});
            this.tsScanResults.Location = new System.Drawing.Point(3, 3);
            this.tsScanResults.Name = "tsScanResults";
            this.tsScanResults.Size = new System.Drawing.Size(770, 45);
            this.tsScanResults.TabIndex = 13;
            this.tsScanResults.Text = "toolStrip1";
            // 
            // btnScan
            // 
            this.btnScan.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFullScan,
            this.recentToolStripMenuItem,
            this.quickToolStripMenuItem});
            this.btnScan.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScan.Image = global::TVRename.Properties.Resources._322497_32;
            this.btnScan.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnScan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(89, 42);
            this.btnScan.Text = "Scan";
            this.btnScan.ButtonClick += new System.EventHandler(this.BtnSearch_ButtonClick);
            // 
            // btnFullScan
            // 
            this.btnFullScan.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFullScan.Name = "btnFullScan";
            this.btnFullScan.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.btnFullScan.ShowShortcutKeys = false;
            this.btnFullScan.Size = new System.Drawing.Size(116, 22);
            this.btnFullScan.Text = "&Full";
            this.btnFullScan.ToolTipText = "Scan all shows";
            this.btnFullScan.Click += new System.EventHandler(this.FullToolStripMenuItem_Click);
            // 
            // recentToolStripMenuItem
            // 
            this.recentToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.recentToolStripMenuItem.Name = "recentToolStripMenuItem";
            this.recentToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.recentToolStripMenuItem.Text = "Recent";
            this.recentToolStripMenuItem.ToolTipText = "Scan shows with recent aired episodes";
            this.recentToolStripMenuItem.Click += new System.EventHandler(this.RecentToolStripMenuItem_Click);
            // 
            // quickToolStripMenuItem
            // 
            this.quickToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.quickToolStripMenuItem.Name = "quickToolStripMenuItem";
            this.quickToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.quickToolStripMenuItem.Text = "Quick";
            this.quickToolStripMenuItem.ToolTipText = "Scan shows with missing recent aired episodes and and shows that match files in t" +
    "he search folders";
            this.quickToolStripMenuItem.Click += new System.EventHandler(this.QuickToolStripMenuItem_Click);
            // 
            // tbFullScan
            // 
            this.tbFullScan.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbFullScan.Image = global::TVRename.Properties.Resources._322497_321;
            this.tbFullScan.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbFullScan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbFullScan.Name = "tbFullScan";
            this.tbFullScan.Padding = new System.Windows.Forms.Padding(3);
            this.tbFullScan.Size = new System.Drawing.Size(112, 42);
            this.tbFullScan.Text = "Full Scan";
            this.tbFullScan.Click += new System.EventHandler(this.TbFullScan_Click);
            // 
            // tpRecentScan
            // 
            this.tpRecentScan.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tpRecentScan.Image = global::TVRename.Properties.Resources._322497_321;
            this.tpRecentScan.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tpRecentScan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tpRecentScan.Name = "tpRecentScan";
            this.tpRecentScan.Size = new System.Drawing.Size(127, 42);
            this.tpRecentScan.Text = "Recent Scan";
            this.tpRecentScan.Click += new System.EventHandler(this.TpRecentScan_Click);
            // 
            // tbQuickScan
            // 
            this.tbQuickScan.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbQuickScan.Image = global::TVRename.Properties.Resources._322497_321;
            this.tbQuickScan.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbQuickScan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbQuickScan.Name = "tbQuickScan";
            this.tbQuickScan.Size = new System.Drawing.Size(120, 42);
            this.tbQuickScan.Text = "Quick Scan";
            this.tbQuickScan.Click += new System.EventHandler(this.TbQuickScan_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 45);
            // 
            // btnActionBTSearch
            // 
            this.btnActionBTSearch.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnActionBTSearch.Image = global::TVRename.Properties.Resources._1587498_32;
            this.btnActionBTSearch.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnActionBTSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnActionBTSearch.Name = "btnActionBTSearch";
            this.btnActionBTSearch.Size = new System.Drawing.Size(123, 42);
            this.btnActionBTSearch.Text = "BT Search";
            this.btnActionBTSearch.ButtonClick += new System.EventHandler(this.bnActionBTSearch_Click);
            this.btnActionBTSearch.DropDownOpening += new System.EventHandler(this.BTSearch_DropDownOpening);
            this.btnActionBTSearch.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuSearchSites_ItemClicked);
            // 
            // tbActionJackettSearch
            // 
            this.tbActionJackettSearch.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbActionJackettSearch.Image = global::TVRename.Properties.Resources._1587498_32;
            this.tbActionJackettSearch.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbActionJackettSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbActionJackettSearch.Name = "tbActionJackettSearch";
            this.tbActionJackettSearch.Size = new System.Drawing.Size(142, 42);
            this.tbActionJackettSearch.Text = "Jackett Search";
            this.tbActionJackettSearch.Click += new System.EventHandler(this.tbJackettSearch_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 45);
            // 
            // btnIgnoreSelectedActions
            // 
            this.btnIgnoreSelectedActions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnIgnoreSelectedActions.Image = ((System.Drawing.Image)(resources.GetObject("btnIgnoreSelectedActions.Image")));
            this.btnIgnoreSelectedActions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnIgnoreSelectedActions.Name = "btnIgnoreSelectedActions";
            this.btnIgnoreSelectedActions.Size = new System.Drawing.Size(63, 19);
            this.btnIgnoreSelectedActions.Text = "&Ignore Sel";
            this.btnIgnoreSelectedActions.Click += new System.EventHandler(this.cbActionIgnore_Click);
            // 
            // btnRemoveSelActions
            // 
            this.btnRemoveSelActions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRemoveSelActions.Image = ((System.Drawing.Image)(resources.GetObject("btnRemoveSelActions.Image")));
            this.btnRemoveSelActions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemoveSelActions.Name = "btnRemoveSelActions";
            this.btnRemoveSelActions.Size = new System.Drawing.Size(72, 19);
            this.btnRemoveSelActions.Text = "&Remove Sel";
            this.btnRemoveSelActions.Click += new System.EventHandler(this.bnRemoveSel_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mcbAll,
            this.mcbRename,
            this.mcbCopyMove,
            this.mcbDeleteFiles,
            this.mcbSaveImages,
            this.mcbDownload,
            this.mcbWriteMetadata,
            this.mcbModifyMetadata});
            this.toolStripDropDownButton1.Image = global::TVRename.Properties.Resources._353430_32;
            this.toolStripDropDownButton1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(85, 36);
            this.toolStripDropDownButton1.Text = "Check";
            // 
            // mcbAll
            // 
            this.mcbAll.Checked = true;
            this.mcbAll.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.mcbAll.Name = "mcbAll";
            this.mcbAll.Size = new System.Drawing.Size(219, 22);
            this.mcbAll.Text = "Show All";
            this.mcbAll.Click += new System.EventHandler(this.McbAll_Click);
            // 
            // mcbRename
            // 
            this.mcbRename.Checked = true;
            this.mcbRename.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.mcbRename.Name = "mcbRename";
            this.mcbRename.Size = new System.Drawing.Size(219, 22);
            this.mcbRename.Text = "Rename Files";
            this.mcbRename.Click += new System.EventHandler(this.McbRename_Click);
            // 
            // mcbCopyMove
            // 
            this.mcbCopyMove.Checked = true;
            this.mcbCopyMove.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.mcbCopyMove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mcbCopyMove.Name = "mcbCopyMove";
            this.mcbCopyMove.Size = new System.Drawing.Size(219, 22);
            this.mcbCopyMove.Text = "Copy / Move";
            this.mcbCopyMove.Click += new System.EventHandler(this.McbCopyMove_Click);
            // 
            // mcbDeleteFiles
            // 
            this.mcbDeleteFiles.Checked = true;
            this.mcbDeleteFiles.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.mcbDeleteFiles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mcbDeleteFiles.Name = "mcbDeleteFiles";
            this.mcbDeleteFiles.Size = new System.Drawing.Size(219, 22);
            this.mcbDeleteFiles.Text = "Delete Files";
            this.mcbDeleteFiles.Click += new System.EventHandler(this.McbDeleteFiles_Click);
            // 
            // mcbSaveImages
            // 
            this.mcbSaveImages.Checked = true;
            this.mcbSaveImages.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.mcbSaveImages.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mcbSaveImages.Name = "mcbSaveImages";
            this.mcbSaveImages.Size = new System.Drawing.Size(219, 22);
            this.mcbSaveImages.Text = "Save Images";
            this.mcbSaveImages.Click += new System.EventHandler(this.McbSaveImages_Click);
            // 
            // mcbDownload
            // 
            this.mcbDownload.Checked = true;
            this.mcbDownload.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.mcbDownload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mcbDownload.Name = "mcbDownload";
            this.mcbDownload.Size = new System.Drawing.Size(219, 22);
            this.mcbDownload.Text = "Download";
            this.mcbDownload.Click += new System.EventHandler(this.McbDownload_Click);
            // 
            // mcbWriteMetadata
            // 
            this.mcbWriteMetadata.Checked = true;
            this.mcbWriteMetadata.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.mcbWriteMetadata.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mcbWriteMetadata.Name = "mcbWriteMetadata";
            this.mcbWriteMetadata.Size = new System.Drawing.Size(219, 22);
            this.mcbWriteMetadata.Text = "Write Metadata Files";
            this.mcbWriteMetadata.Click += new System.EventHandler(this.McbWriteMetadata_Click);
            // 
            // mcbModifyMetadata
            // 
            this.mcbModifyMetadata.Checked = true;
            this.mcbModifyMetadata.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.mcbModifyMetadata.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mcbModifyMetadata.Name = "mcbModifyMetadata";
            this.mcbModifyMetadata.Size = new System.Drawing.Size(219, 22);
            this.mcbModifyMetadata.Text = "Update Video File Metadata";
            this.mcbModifyMetadata.Click += new System.EventHandler(this.McbModifyMetadata_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 45);
            // 
            // btnActionAction
            // 
            this.btnActionAction.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnActionAction.Image = global::TVRename.Properties.Resources._5172493_32;
            this.btnActionAction.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnActionAction.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnActionAction.Name = "btnActionAction";
            this.btnActionAction.Size = new System.Drawing.Size(127, 36);
            this.btnActionAction.Text = "&Do Checked";
            this.btnActionAction.Click += new System.EventHandler(this.bnActionAction_Click);
            // 
            // btnRevertView
            // 
            this.btnRevertView.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnRevertView.Image = ((System.Drawing.Image)(resources.GetObject("btnRevertView.Image")));
            this.btnRevertView.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnRevertView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRevertView.Name = "btnRevertView";
            this.btnRevertView.Size = new System.Drawing.Size(88, 20);
            this.btnRevertView.Text = "Revert View";
            this.btnRevertView.Click += new System.EventHandler(this.BtnRevertView_Click);
            // 
            // btnPreferences
            // 
            this.btnPreferences.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnPreferences.Image = global::TVRename.Properties.Resources._2738302_32;
            this.btnPreferences.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnPreferences.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPreferences.Name = "btnPreferences";
            this.btnPreferences.Size = new System.Drawing.Size(113, 36);
            this.btnPreferences.Text = "&Preferences...";
            this.btnPreferences.Click += new System.EventHandler(this.bnActionOptions_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(6, 45);
            // 
            // tsbScanContextMenu
            // 
            this.tsbScanContextMenu.Image = global::TVRename.Properties.Resources._314251_32;
            this.tsbScanContextMenu.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbScanContextMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbScanContextMenu.Name = "tsbScanContextMenu";
            this.tsbScanContextMenu.Size = new System.Drawing.Size(119, 36);
            this.tsbScanContextMenu.Text = "Context Menu";
            this.tsbScanContextMenu.Click += new System.EventHandler(this.TsbScanContextMenu_Click);
            // 
            // tbWTW
            // 
            this.tbWTW.Controls.Add(this.tsWtW);
            this.tbWTW.Controls.Add(this.txtWhenToWatchSynopsis);
            this.tbWTW.Controls.Add(this.calCalendar);
            this.tbWTW.Controls.Add(this.lvWhenToWatch);
            this.tbWTW.ImageKey = "115762-48.png";
            this.tbWTW.Location = new System.Drawing.Point(104, 4);
            this.tbWTW.Name = "tbWTW";
            this.tbWTW.Size = new System.Drawing.Size(776, 475);
            this.tbWTW.TabIndex = 4;
            this.tbWTW.Text = "Schedule";
            this.tbWTW.UseVisualStyleBackColor = true;
            // 
            // tsWtW
            // 
            this.tsWtW.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsWtW.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnWhenToWatchCheck,
            this.btnScheduleBTSearch,
            this.tsbScheduleJackettSearch,
            this.toolStripSeparator12,
            this.btnScheduleRightClick});
            this.tsWtW.Location = new System.Drawing.Point(0, 0);
            this.tsWtW.Name = "tsWtW";
            this.tsWtW.Size = new System.Drawing.Size(776, 39);
            this.tsWtW.TabIndex = 6;
            this.tsWtW.Text = "toolStrip1";
            // 
            // btnWhenToWatchCheck
            // 
            this.btnWhenToWatchCheck.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWhenToWatchCheck.Image = global::TVRename.Properties.Resources._134221_32;
            this.btnWhenToWatchCheck.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnWhenToWatchCheck.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnWhenToWatchCheck.Name = "btnWhenToWatchCheck";
            this.btnWhenToWatchCheck.Size = new System.Drawing.Size(96, 36);
            this.btnWhenToWatchCheck.Text = "&Refresh";
            this.btnWhenToWatchCheck.Click += new System.EventHandler(this.bnWhenToWatchCheck_Click);
            // 
            // btnScheduleBTSearch
            // 
            this.btnScheduleBTSearch.DropDownButtonWidth = 20;
            this.btnScheduleBTSearch.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScheduleBTSearch.Image = global::TVRename.Properties.Resources._1587498_32;
            this.btnScheduleBTSearch.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnScheduleBTSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScheduleBTSearch.Name = "btnScheduleBTSearch";
            this.btnScheduleBTSearch.Size = new System.Drawing.Size(132, 36);
            this.btnScheduleBTSearch.Text = "BT Search";
            this.btnScheduleBTSearch.ButtonClick += new System.EventHandler(this.bnWTWBTSearch_Click);
            this.btnScheduleBTSearch.DropDownOpening += new System.EventHandler(this.BTSearch_DropDownOpening);
            this.btnScheduleBTSearch.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuSearchSites_ItemClicked);
            // 
            // tsbScheduleJackettSearch
            // 
            this.tsbScheduleJackettSearch.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsbScheduleJackettSearch.Image = global::TVRename.Properties.Resources._1587498_32;
            this.tsbScheduleJackettSearch.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsbScheduleJackettSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbScheduleJackettSearch.Name = "tsbScheduleJackettSearch";
            this.tsbScheduleJackettSearch.Size = new System.Drawing.Size(142, 36);
            this.tsbScheduleJackettSearch.Text = "Jackett Search";
            this.tsbScheduleJackettSearch.Click += new System.EventHandler(this.tsbScheduleJackettSearch_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 39);
            // 
            // btnScheduleRightClick
            // 
            this.btnScheduleRightClick.Image = global::TVRename.Properties.Resources._314251_32;
            this.btnScheduleRightClick.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnScheduleRightClick.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScheduleRightClick.Name = "btnScheduleRightClick";
            this.btnScheduleRightClick.Size = new System.Drawing.Size(119, 36);
            this.btnScheduleRightClick.Text = "Context Menu";
            this.btnScheduleRightClick.Click += new System.EventHandler(this.ToolStripButton1_Click);
            // 
            // txtWhenToWatchSynopsis
            // 
            this.txtWhenToWatchSynopsis.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWhenToWatchSynopsis.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWhenToWatchSynopsis.Location = new System.Drawing.Point(3, 311);
            this.txtWhenToWatchSynopsis.Multiline = true;
            this.txtWhenToWatchSynopsis.Name = "txtWhenToWatchSynopsis";
            this.txtWhenToWatchSynopsis.ReadOnly = true;
            this.txtWhenToWatchSynopsis.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtWhenToWatchSynopsis.Size = new System.Drawing.Size(542, 161);
            this.txtWhenToWatchSynopsis.TabIndex = 4;
            // 
            // calCalendar
            // 
            this.calCalendar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.calCalendar.Location = new System.Drawing.Point(546, 310);
            this.calCalendar.MaxSelectionCount = 1;
            this.calCalendar.Name = "calCalendar";
            this.calCalendar.TabIndex = 5;
            this.calCalendar.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.calCalendar_DateSelected);
            // 
            // lvWhenToWatch
            // 
            this.lvWhenToWatch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvWhenToWatch.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader29,
            this.columnHeader30,
            this.columnHeader31,
            this.columnHeader32,
            this.columnHeader36,
            this.columnHeader33,
            this.columnHeader34,
            this.columnHeader1,
            this.columnHeader35});
            this.lvWhenToWatch.FullRowSelect = true;
            listViewGroup1.Header = "Recently Aired";
            listViewGroup1.Name = "justPassed";
            listViewGroup2.Header = "Next 7 Days";
            listViewGroup2.Name = "next7days";
            listViewGroup2.Tag = "1";
            listViewGroup3.Header = "Future Episodes";
            listViewGroup3.Name = "futureEps";
            listViewGroup4.Header = "Later";
            listViewGroup4.Name = "later";
            listViewGroup4.Tag = "2";
            this.lvWhenToWatch.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4});
            this.lvWhenToWatch.HideSelection = false;
            this.lvWhenToWatch.Location = new System.Drawing.Point(0, 40);
            this.lvWhenToWatch.Name = "lvWhenToWatch";
            this.lvWhenToWatch.ShowItemToolTips = true;
            this.lvWhenToWatch.Size = new System.Drawing.Size(773, 265);
            this.lvWhenToWatch.SmallImageList = this.ilIcons;
            this.lvWhenToWatch.TabIndex = 3;
            this.lvWhenToWatch.UseCompatibleStateImageBehavior = false;
            this.lvWhenToWatch.View = System.Windows.Forms.View.Details;
            this.lvWhenToWatch.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvWhenToWatch_ColumnClick);
            this.lvWhenToWatch.SelectedIndexChanged += new System.EventHandler(this.lvWhenToWatch_Click);
            this.lvWhenToWatch.DoubleClick += new System.EventHandler(this.lvWhenToWatch_DoubleClick);
            this.lvWhenToWatch.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvWhenToWatch_MouseClick);
            // 
            // columnHeader29
            // 
            this.columnHeader29.Text = "Show";
            this.columnHeader29.Width = 187;
            // 
            // columnHeader30
            // 
            this.columnHeader30.Text = "Season";
            this.columnHeader30.Width = 51;
            // 
            // columnHeader31
            // 
            this.columnHeader31.Text = "Episode";
            this.columnHeader31.Width = 55;
            // 
            // columnHeader32
            // 
            this.columnHeader32.Text = "Air Date";
            this.columnHeader32.Width = 81;
            // 
            // columnHeader36
            // 
            this.columnHeader36.Text = "Time";
            // 
            // columnHeader33
            // 
            this.columnHeader33.Text = "Day";
            this.columnHeader33.Width = 42;
            // 
            // columnHeader34
            // 
            this.columnHeader34.Text = "How Long";
            this.columnHeader34.Width = 69;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Network";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader35
            // 
            this.columnHeader35.Text = "Episode Name";
            this.columnHeader35.Width = 360;
            // 
            // ilNewIcons
            // 
            this.ilNewIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilNewIcons.ImageStream")));
            this.ilNewIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.ilNewIcons.Images.SetKeyName(0, "322497-48 (1).png");
            this.ilNewIcons.Images.SetKeyName(1, "353430-48.png");
            this.ilNewIcons.Images.SetKeyName(2, "1587498-48.png");
            this.ilNewIcons.Images.SetKeyName(3, "115762-48.png");
            this.ilNewIcons.Images.SetKeyName(4, "3790574-48.png");
            this.ilNewIcons.Images.SetKeyName(5, "4632196-48.png");
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Magenta;
            this.imageList1.Images.SetKeyName(0, "clock.bmp");
            this.imageList1.Images.SetKeyName(1, "Calendar_schedule.bmp");
            this.imageList1.Images.SetKeyName(2, "Save.bmp");
            this.imageList1.Images.SetKeyName(3, "Refresh.bmp");
            this.imageList1.Images.SetKeyName(4, "Control_TreeView.bmp");
            this.imageList1.Images.SetKeyName(5, "Zoom.bmp");
            this.imageList1.Images.SetKeyName(6, "delete.bmp");
            this.imageList1.Images.SetKeyName(7, "EditInformation.bmp");
            this.imageList1.Images.SetKeyName(8, "FormRun.bmp");
            this.imageList1.Images.SetKeyName(9, "GetLatestVersion.bmp");
            this.imageList1.Images.SetKeyName(10, "OpenFolder.bmp");
            this.imageList1.Images.SetKeyName(11, "SearchWeb.bmp");
            this.imageList1.Images.SetKeyName(12, "PublishToWeb.bmp");
            this.imageList1.Images.SetKeyName(13, "Options.bmp");
            this.imageList1.Images.SetKeyName(14, "NewCard.bmp");
            this.imageList1.Images.SetKeyName(15, "TVOff.bmp");
            this.imageList1.Images.SetKeyName(16, "FillLeft.bmp");
            this.imageList1.Images.SetKeyName(17, "FillRight.bmp");
            this.imageList1.Images.SetKeyName(18, "filtre.png");
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel2.Controls.Add(this.pbProgressBarx, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtDLStatusLabel, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.tsNextShowTxt, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel2.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 510);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(884, 19);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // pbProgressBarx
            // 
            this.pbProgressBarx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgressBarx.Location = new System.Drawing.Point(753, 3);
            this.pbProgressBarx.Name = "pbProgressBarx";
            this.pbProgressBarx.Size = new System.Drawing.Size(128, 13);
            this.pbProgressBarx.Step = 1;
            this.pbProgressBarx.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbProgressBarx.TabIndex = 0;
            // 
            // txtDLStatusLabel
            // 
            this.txtDLStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDLStatusLabel.Location = new System.Drawing.Point(400, 6);
            this.txtDLStatusLabel.Name = "txtDLStatusLabel";
            this.txtDLStatusLabel.Size = new System.Drawing.Size(347, 13);
            this.txtDLStatusLabel.TabIndex = 1;
            this.txtDLStatusLabel.Text = "Background Download: ---";
            this.txtDLStatusLabel.Visible = false;
            // 
            // tsNextShowTxt
            // 
            this.tsNextShowTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tsNextShowTxt.Location = new System.Drawing.Point(3, 6);
            this.tsNextShowTxt.Name = "tsNextShowTxt";
            this.tsNextShowTxt.Size = new System.Drawing.Size(391, 13);
            this.tsNextShowTxt.TabIndex = 1;
            this.tsNextShowTxt.Text = "---";
            this.tsNextShowTxt.UseMnemonic = false;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Show";
            this.columnHeader5.Width = 211;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Season";
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "thetvdb code";
            this.columnHeader7.Width = 82;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Show next airdate";
            this.columnHeader8.Width = 115;
            // 
            // columnHeader25
            // 
            this.columnHeader25.Text = "From Folder";
            this.columnHeader25.Width = 187;
            // 
            // columnHeader26
            // 
            this.columnHeader26.Text = "From Name";
            this.columnHeader26.Width = 163;
            // 
            // columnHeader27
            // 
            this.columnHeader27.Text = "To Folder";
            this.columnHeader27.Width = 172;
            // 
            // columnHeader28
            // 
            this.columnHeader28.Text = "To Name";
            this.columnHeader28.Width = 165;
            // 
            // openFile
            // 
            this.openFile.Filter = "Torrent files (*.torrent)|*.torrent|All files (*.*)|*.*";
            // 
            // folderBrowser
            // 
            this.folderBrowser.ShowNewFolderButton = false;
            // 
            // refreshWTWTimer
            // 
            this.refreshWTWTimer.Enabled = true;
            this.refreshWTWTimer.Interval = 600000;
            this.refreshWTWTimer.Tick += new System.EventHandler(this.refreshWTWTimer_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipText = "TV Rename is t3h r0x0r";
            this.notifyIcon1.BalloonTipTitle = "TV Rename";
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "TV Rename";
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_Click);
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_DoubleClick);
            // 
            // showRightClickMenu
            // 
            this.showRightClickMenu.Name = "menuSearchSites";
            this.showRightClickMenu.ShowImageMargin = false;
            this.showRightClickMenu.Size = new System.Drawing.Size(36, 4);
            this.showRightClickMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.showRightClickMenu_ItemClicked);
            // 
            // statusTimer
            // 
            this.statusTimer.Enabled = true;
            this.statusTimer.Interval = 250;
            this.statusTimer.Tick += new System.EventHandler(this.statusTimer_Tick);
            // 
            // BGDownloadTimer
            // 
            this.BGDownloadTimer.Enabled = true;
            this.BGDownloadTimer.Interval = 10000;
            this.BGDownloadTimer.Tick += new System.EventHandler(this.BGDownloadTimer_Tick);
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Enabled = true;
            this.UpdateTimer.Interval = 1000;
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
            // 
            // tmrShowUpcomingPopup
            // 
            this.tmrShowUpcomingPopup.Interval = 250;
            this.tmrShowUpcomingPopup.Tick += new System.EventHandler(this.tmrShowUpcomingPopup_Tick);
            // 
            // quickTimer
            // 
            this.quickTimer.Interval = 1;
            this.quickTimer.Tick += new System.EventHandler(this.quickTimer_Tick);
            // 
            // tmrPeriodicScan
            // 
            this.tmrPeriodicScan.Enabled = true;
            this.tmrPeriodicScan.Tick += new System.EventHandler(this.tmrPeriodicScan_Tick);
            // 
            // btnUpdateAvailable
            // 
            this.btnUpdateAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdateAvailable.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnUpdateAvailable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdateAvailable.Location = new System.Drawing.Point(761, 0);
            this.btnUpdateAvailable.Name = "btnUpdateAvailable";
            this.btnUpdateAvailable.Size = new System.Drawing.Size(116, 23);
            this.btnUpdateAvailable.TabIndex = 10;
            this.btnUpdateAvailable.Text = "Update Available...";
            this.btnUpdateAvailable.UseVisualStyleBackColor = false;
            this.btnUpdateAvailable.Visible = false;
            this.btnUpdateAvailable.Click += new System.EventHandler(this.btnUpdateAvailable_Click);
            // 
            // bwSeasonHTMLGenerator
            // 
            this.bwSeasonHTMLGenerator.WorkerSupportsCancellation = true;
            this.bwSeasonHTMLGenerator.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BwSeasonHTMLGenerator_DoWork);
            this.bwSeasonHTMLGenerator.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.UpdateWebInformation);
            // 
            // bwUpdateSchedule
            // 
            this.bwUpdateSchedule.WorkerSupportsCancellation = true;
            this.bwUpdateSchedule.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BwUpdateSchedule_DoWork);
            this.bwUpdateSchedule.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BwUpdateSchedule_RunWorkerCompleted);
            // 
            // bwShowHTMLGenerator
            // 
            this.bwShowHTMLGenerator.WorkerSupportsCancellation = true;
            this.bwShowHTMLGenerator.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BwShowHTMLGenerator_DoWork);
            this.bwShowHTMLGenerator.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.UpdateWebInformation);
            // 
            // bwShowSummaryHTMLGenerator
            // 
            this.bwShowSummaryHTMLGenerator.WorkerSupportsCancellation = true;
            this.bwShowSummaryHTMLGenerator.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BwShowSummaryHTMLGenerator_DoWork);
            this.bwShowSummaryHTMLGenerator.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.UpdateWebSummary);
            // 
            // bwSeasonSummaryHTMLGenerator
            // 
            this.bwSeasonSummaryHTMLGenerator.WorkerSupportsCancellation = true;
            this.bwSeasonSummaryHTMLGenerator.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BwSeasonSummaryHTMLGenerator_DoWork);
            this.bwSeasonSummaryHTMLGenerator.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.UpdateWebSummary);
            // 
            // bwMovieHTMLGenerator
            // 
            this.bwMovieHTMLGenerator.WorkerSupportsCancellation = true;
            this.bwMovieHTMLGenerator.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwMovieHTMLGenerator_DoWork);
            this.bwMovieHTMLGenerator.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.UpdateMovieInformation);
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            this.toolStripSeparator20.Size = new System.Drawing.Size(251, 6);
            // 
            // settingsCheckToolStripMenuItem
            // 
            this.settingsCheckToolStripMenuItem.Name = "settingsCheckToolStripMenuItem";
            this.settingsCheckToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.settingsCheckToolStripMenuItem.Text = "Settings Check";
            // 
            // toolStripSeparator21
            // 
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            this.toolStripSeparator21.Size = new System.Drawing.Size(259, 6);
            // 
            // movieRecommendationsToolStripMenuItem
            // 
            this.movieRecommendationsToolStripMenuItem.Name = "movieRecommendationsToolStripMenuItem";
            this.movieRecommendationsToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.movieRecommendationsToolStripMenuItem.Text = "Movie Recommendations...";
            // 
            // tvRecommendationsToolStripMenuItem
            // 
            this.tvRecommendationsToolStripMenuItem.Name = "tvRecommendationsToolStripMenuItem";
            this.tvRecommendationsToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.tvRecommendationsToolStripMenuItem.Text = "TV Recommendations...";
            // 
            // UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 529);
            this.Controls.Add(this.btnUpdateAvailable);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.tabControl1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "UI";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "TV Rename";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UI_FormClosing);
            this.Load += new System.EventHandler(this.UI_Load);
            this.LocationChanged += new System.EventHandler(this.UI_LocationChanged);
            this.SizeChanged += new System.EventHandler(this.UI_SizeChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UI_KeyDown);
            this.Resize += new System.EventHandler(this.UI_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tbMyMovies.ResumeLayout(false);
            this.tbMyMovies.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabControl3.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tbMyShows.ResumeLayout(false);
            this.tbMyShows.PerformLayout();
            this.tsMyShows.ResumeLayout(false);
            this.tsMyShows.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tpInformation.ResumeLayout(false);
            this.tpImages.ResumeLayout(false);
            this.tpSummary.ResumeLayout(false);
            this.tbAllInOne.ResumeLayout(false);
            this.tbAllInOne.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvAction)).EndInit();
            this.tsScanResults.ResumeLayout(false);
            this.tsScanResults.PerformLayout();
            this.tbWTW.ResumeLayout(false);
            this.tbWTW.PerformLayout();
            this.tsWtW.ResumeLayout(false);
            this.tsWtW.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private System.Windows.Forms.TabPage tbAllInOne;

        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem folderMonitorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ignoreListToolStripMenuItem;

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem visitWebsiteToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tbWTW;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private ListViewFlickerFree lvWhenToWatch;
        private System.Windows.Forms.ColumnHeader columnHeader29;
        private System.Windows.Forms.ColumnHeader columnHeader30;
        private System.Windows.Forms.ColumnHeader columnHeader31;
        private System.Windows.Forms.ColumnHeader columnHeader32;
        private System.Windows.Forms.ColumnHeader columnHeader33;
        private System.Windows.Forms.ColumnHeader columnHeader34;
        private System.Windows.Forms.ColumnHeader columnHeader35;
        private System.Windows.Forms.ColumnHeader columnHeader25;
        private System.Windows.Forms.ColumnHeader columnHeader26;
        private System.Windows.Forms.ColumnHeader columnHeader27;
        private System.Windows.Forms.ColumnHeader columnHeader28;
        private System.Windows.Forms.TextBox txtWhenToWatchSynopsis;
        private System.Windows.Forms.MonthCalendar calCalendar;
        private System.Windows.Forms.OpenFileDialog openFile;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
        private System.Windows.Forms.Timer refreshWTWTimer;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ColumnHeader columnHeader36;
        private System.Windows.Forms.ToolStripMenuItem buyMeADrinkToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip showRightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Timer statusTimer;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backgroundDownloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem offlineOperationToolStripMenuItem;
        private System.Windows.Forms.Label tsNextShowTxt;
        private System.Windows.Forms.Label txtDLStatusLabel;
        private System.Windows.Forms.ProgressBar pbProgressBarx;
        private System.Windows.Forms.Timer BGDownloadTimer;
        private System.Windows.Forms.Timer UpdateTimer;
        private System.Windows.Forms.ToolStripMenuItem bugReportToolStripMenuItem;
        private System.Windows.Forms.ImageList ilIcons;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFile;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        
        private System.Windows.Forms.ToolStripMenuItem flushCacheToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flushImageCacheToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backgroundDownloadNowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statisticsToolStripMenuItem;
        private System.Windows.Forms.TabPage tbMyShows;
        private System.Windows.Forms.TreeView MyShowTree;
        private System.Windows.Forms.ToolStripMenuItem quickstartGuideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filenameTemplateEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchEnginesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filenameProcessorsToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;

        private System.Windows.Forms.Timer tmrShowUpcomingPopup;
        private System.Windows.Forms.ToolStripMenuItem actorsToolStripMenuItem;
        private System.Windows.Forms.Timer quickTimer;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tpInformation;
        private System.Windows.Forms.TabPage tpImages;
        private System.Windows.Forms.TextBox filterTextBox;
        private System.Windows.Forms.ToolStripMenuItem visitSupportForumToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem betaToolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSummaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForNewVersionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem duplicateFinderLOGToolStripMenuItem;
        private System.Windows.Forms.Timer tmrPeriodicScan;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem timezoneInconsistencyLOGToolStripMenuItem;
        private WebBrowser webInformation;
        private WebBrowser webImages;
        private Button btnUpdateAvailable;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem logToolStripMenuItem;
        private ToolStripMenuItem episodeFileQualitySummaryLogToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ColumnHeader columnHeader1;
        private ToolStripMenuItem quickRenameToolStripMenuItem;
        private ToolStripMenuItem accuracyCheckLogToolStripMenuItem;
        private ToolStrip tsMyShows;
        private ToolStripButton btnMyShowsRefresh;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton btnAddTVShow;
        private ToolStripButton btnEditShow;
        private ToolStripButton btnRemoveShow;
        private ToolStripButton btnHideHTMLPanel;
        private ToolStripButton btnMyShowsCollapse;
        private ToolStrip tsWtW;
        private ToolStripButton btnWhenToWatchCheck;
        private ToolStripSplitButton btnScheduleBTSearch;
        private ToolStripButton btnFilterMyShows;
        private ToolStrip tsScanResults;
        private ToolStripSplitButton btnScan;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripSplitButton btnActionBTSearch;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripButton btnIgnoreSelectedActions;
        private ToolStripButton btnRemoveSelActions;
        private ToolStripDropDownButton toolStripDropDownButton1;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripButton btnActionAction;
        private ToolStripButton btnPreferences;
        private ToolStripMenuItem btnFullScan;
        private ToolStripMenuItem recentToolStripMenuItem;
        private ToolStripMenuItem quickToolStripMenuItem;
        private ToolStripMenuItem mcbAll;
        private ToolStripMenuItem mcbRename;
        private ToolStripMenuItem mcbCopyMove;
        private ToolStripMenuItem mcbDeleteFiles;
        private ToolStripMenuItem mcbSaveImages;
        private ToolStripMenuItem mcbDownload;
        private ToolStripMenuItem mcbWriteMetadata;
        private ToolStripMenuItem mcbModifyMetadata;
        private ToolStripMenuItem thanksToolStripMenuItem;
        private ImageList ilNewIcons;
        private System.ComponentModel.BackgroundWorker bwSeasonHTMLGenerator;
        private System.ComponentModel.BackgroundWorker bwUpdateSchedule;
        private System.ComponentModel.BackgroundWorker bwShowHTMLGenerator;
        private ToolStripButton tbFullScan;
        private ToolStripButton tpRecentScan;
        private ToolStripButton tbQuickScan;
        private OLVColumn olvShowColumn;
        private OLVColumn olvSeason;
        private OLVColumn olvEpisode;
        private OLVColumn olvDate;
        private OLVColumn olvFolder;
        private OLVColumn olvFilename;
        private OLVColumn olvSource;
        private OLVColumn olvErrors;
        private OLVColumn olvType;
        private ToolStripButton btnRevertView;
        private TabPage tpSummary;
        private WebBrowser webSummary;
        private System.ComponentModel.BackgroundWorker bwShowSummaryHTMLGenerator;
        private System.ComponentModel.BackgroundWorker bwSeasonSummaryHTMLGenerator;
        private ToolStripButton btnScheduleRightClick;
        private ToolStripSeparator toolStripSeparator12;
        private ToolStripSeparator toolStripSeparator13;
        private ToolStripButton tsbMyShowsContextMenu;
        private ToolStripSeparator toolStripSeparator14;
        private ToolStripButton tsbScanContextMenu;
        private ToolStripMenuItem tsmiOrphanFiles;
        private ObjectListViewFlickerFree olvAction;
        private ToolStripButton tbActionJackettSearch;
        private ToolStripButton tsbScheduleJackettSearch;
        private TabPage tbMyMovies;
        private ToolStrip toolStrip1;
        private ToolStripButton tbnAddMovie;
        private ToolStripButton btnEditMovie;
        private ToolStripButton btnMovieDelete;
        private ToolStripSeparator toolStripSeparator15;
        private ToolStripButton btnMovieRefresh;
        private ToolStripButton btnMovieFilter;
        private ToolStripSeparator toolStripSeparator16;
        private ToolStripButton tsbMyMoviesContextMenu;
        private SplitContainer splitContainer2;
        private TreeView movieTree;
        private TextBox filterMoviesTextbox;
        private TabControl tabControl3;
        private TabPage tabPage1;
        private WebBrowser webMovieInformation;
        private TabPage tabPage2;
        private WebBrowser webMovieImages;
        private System.ComponentModel.BackgroundWorker bwMovieHTMLGenerator;
        private ToolStripSeparator toolStripSeparator17;
        private ToolStripMenuItem bulkAddMoviesToolStripMenuItem;
        private ToolStripMenuItem tMDBAccuracyCheckLogToolStripMenuItem;
        private ToolStripMenuItem duplicateMoviesToolStripMenuItem;
        private ToolStripMenuItem movieCollectionSummaryLogToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator18;
        private ToolStripSeparator toolStripSeparator19;
        private ToolStripButton tsbScanMovies;
        private ToolStripMenuItem movieSearchEnginesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator20;
        private ToolStripMenuItem settingsCheckToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator21;
        private ToolStripMenuItem movieRecommendationsToolStripMenuItem;
        private ToolStripMenuItem tvRecommendationsToolStripMenuItem;
    }
}
