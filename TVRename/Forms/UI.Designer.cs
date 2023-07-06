//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Windows.Forms;
using BrightIdeasSoftware;

namespace TVRename.Forms
{
    public partial class UI
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
            if (disposing && (this.mAutoFolderMonitor != null))
            {
                mAutoFolderMonitor.Dispose();
            }
            if (disposing && (scanProgDlg != null))
            {
                scanProgDlg.Dispose();
            }

            try
            {
                base.Dispose(disposing);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Logger.Warn(ex);
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        public void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UI));
            ListViewGroup listViewGroup1 = new ListViewGroup("Recently Aired", HorizontalAlignment.Left);
            ListViewGroup listViewGroup2 = new ListViewGroup("Next 7 Days", HorizontalAlignment.Left);
            ListViewGroup listViewGroup3 = new ListViewGroup("Returning Series", HorizontalAlignment.Left);
            ListViewGroup listViewGroup4 = new ListViewGroup("Other Planned Episodes", HorizontalAlignment.Left);
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            exportToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            offlineOperationToolStripMenuItem = new ToolStripMenuItem();
            backgroundDownloadToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            preferencesToolStripMenuItem = new ToolStripMenuItem();
            ignoreListToolStripMenuItem = new ToolStripMenuItem();
            filenameTemplateEditorToolStripMenuItem = new ToolStripMenuItem();
            movieSearchEnginesToolStripMenuItem = new ToolStripMenuItem();
            searchEnginesToolStripMenuItem = new ToolStripMenuItem();
            filenameProcessorsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator20 = new ToolStripSeparator();
            settingsCheckToolStripMenuItem = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            flushCacheToolStripMenuItem = new ToolStripMenuItem();
            flushImageCacheToolStripMenuItem = new ToolStripMenuItem();
            backgroundDownloadNowToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator17 = new ToolStripSeparator();
            folderMonitorToolStripMenuItem = new ToolStripMenuItem();
            bulkAddMoviesToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            tsmiOrphanFiles = new ToolStripMenuItem();
            duplicateFinderLOGToolStripMenuItem = new ToolStripMenuItem();
            duplicateMoviesToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator18 = new ToolStripSeparator();
            quickRenameToolStripMenuItem = new ToolStripMenuItem();
            scanMovieFolderToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator21 = new ToolStripSeparator();
            movieRecommendationsToolStripMenuItem = new ToolStripMenuItem();
            tvRecommendationsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator121 = new ToolStripSeparator();
            cleanLibraryFoldersToolStripMenuItem = new ToolStripMenuItem();
            forceRefreshKodiTVShowNFOFIlesToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            statisticsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            showSummaryToolStripMenuItem = new ToolStripMenuItem();
            movieCollectionSummaryLogToolStripMenuItem = new ToolStripMenuItem();
            actorsToolStripMenuItem = new ToolStripMenuItem();
            betaToolsToolStripMenuItem = new ToolStripMenuItem();
            timezoneInconsistencyLOGToolStripMenuItem = new ToolStripMenuItem();
            episodeFileQualitySummaryLogToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator19 = new ToolStripSeparator();
            accuracyCheckLogToolStripMenuItem = new ToolStripMenuItem();
            tMDBAccuracyCheckLogToolStripMenuItem = new ToolStripMenuItem();
            tVDBUPdateCheckerLogToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator24 = new ToolStripSeparator();
            yTSMoviePreviewToolStripMenuItem = new ToolStripMenuItem();
            yTSMovieRecommendationsToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            quickstartGuideToolStripMenuItem = new ToolStripMenuItem();
            visitWebsiteToolStripMenuItem = new ToolStripMenuItem();
            visitSupportForumToolStripMenuItem = new ToolStripMenuItem();
            bugReportToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator7 = new ToolStripSeparator();
            requestANewFeatureToolStripMenuItem = new ToolStripMenuItem();
            buyMeADrinkToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator6 = new ToolStripSeparator();
            browserTestToolStripMenuItem = new ToolStripMenuItem();
            checkForNewVersionToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator8 = new ToolStripSeparator();
            logToolStripMenuItem = new ToolStripMenuItem();
            thanksToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator23 = new ToolStripSeparator();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator22 = new ToolStripSeparator();
            webTestToolStripMenuItem = new ToolStripMenuItem();
            downloadInstallerToolStripMenuItem = new ToolStripMenuItem();
            tabControl1 = new TabControl();
            tbMyMovies = new TabPage();
            toolStripContainer1 = new ToolStripContainer();
            splitContainer2 = new SplitContainer();
            movieTree = new TreeView();
            filterMoviesTextbox = new TextBox();
            tabControl3 = new TabControl();
            tabPage1 = new TabPage();
            chrMovieInformation = new CefSharp.WinForms.ChromiumWebBrowser();
            tabPage2 = new TabPage();
            chrMovieImages = new CefSharp.WinForms.ChromiumWebBrowser();
            tpMovieTrailer = new TabPage();
            chrMovieTrailer = new CefSharp.WinForms.ChromiumWebBrowser();
            toolStrip1 = new ToolStrip();
            tbnAddMovie = new ToolStripButton();
            btnEditMovie = new ToolStripButton();
            btnMovieDelete = new ToolStripButton();
            toolStripSeparator15 = new ToolStripSeparator();
            btnMovieRefresh = new ToolStripButton();
            tsbScanMovies = new ToolStripButton();
            btnMovieFilter = new ToolStripButton();
            toolStripSeparator16 = new ToolStripSeparator();
            tsbMyMoviesContextMenu = new ToolStripButton();
            tbMyShows = new TabPage();
            toolStripContainer2 = new ToolStripContainer();
            splitContainer1 = new SplitContainer();
            MyShowTree = new TreeView();
            filterTextBox = new TextBox();
            tabControl2 = new TabControl();
            tpInformation = new TabPage();
            chrInformation = new CefSharp.WinForms.ChromiumWebBrowser();
            tpImages = new TabPage();
            chrImages = new CefSharp.WinForms.ChromiumWebBrowser();
            tpSummary = new TabPage();
            chrSummary = new CefSharp.WinForms.ChromiumWebBrowser();
            tabPage3 = new TabPage();
            chrTvTrailer = new CefSharp.WinForms.ChromiumWebBrowser();
            tsMyShows = new ToolStrip();
            btnAddTVShow = new ToolStripButton();
            btnEditShow = new ToolStripButton();
            btnRemoveShow = new ToolStripButton();
            btnHideHTMLPanel = new ToolStripButton();
            btnMyShowsCollapse = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            btnMyShowsRefresh = new ToolStripButton();
            tsbScanTV = new ToolStripButton();
            btnFilterMyShows = new ToolStripButton();
            toolStripSeparator13 = new ToolStripSeparator();
            tsbMyShowsContextMenu = new ToolStripButton();
            tbAllInOne = new TabPage();
            olvAction = new ObjectListViewFlickerFree();
            olvShowColumn = new OLVColumn();
            olvSeason = new OLVColumn();
            olvEpisode = new OLVColumn();
            olvDate = new OLVColumn();
            olvFolder = new OLVColumn();
            olvFilename = new OLVColumn();
            olvSource = new OLVColumn();
            olvErrors = new OLVColumn();
            olvType = new OLVColumn();
            ilIcons = new ImageList(components);
            tsScanResults = new ToolStrip();
            btnScan = new ToolStripSplitButton();
            fullToolStripMenuItem = new ToolStripMenuItem();
            recentToolStripMenuItem = new ToolStripMenuItem();
            quickToolStripMenuItem = new ToolStripMenuItem();
            tbFullScan = new ToolStripButton();
            tpRecentScan = new ToolStripButton();
            tbQuickScan = new ToolStripButton();
            toolStripSeparator11 = new ToolStripSeparator();
            btnActionBTSearch = new ToolStripSplitButton();
            tbActionJackettSearch = new ToolStripButton();
            toolStripSeparator9 = new ToolStripSeparator();
            btnIgnoreSelectedActions = new ToolStripButton();
            btnRemoveSelActions = new ToolStripButton();
            toolStripDropDownButton1 = new ToolStripDropDownButton();
            mcbAll = new ToolStripMenuItem();
            mcbRename = new ToolStripMenuItem();
            mcbCopyMove = new ToolStripMenuItem();
            mcbDeleteFiles = new ToolStripMenuItem();
            mcbSaveImages = new ToolStripMenuItem();
            mcbDownload = new ToolStripMenuItem();
            mcbWriteMetadata = new ToolStripMenuItem();
            mcbModifyMetadata = new ToolStripMenuItem();
            toolStripSeparator10 = new ToolStripSeparator();
            btnActionAction = new ToolStripButton();
            btnRevertView = new ToolStripButton();
            btnPreferences = new ToolStripButton();
            toolStripSeparator14 = new ToolStripSeparator();
            tsbScanContextMenu = new ToolStripButton();
            tbWTW = new TabPage();
            tableLayoutPanel4 = new TableLayoutPanel();
            tableLayoutPanel1 = new TableLayoutPanel();
            calCalendar = new MonthCalendar();
            txtWhenToWatchSynopsis = new TextBox();
            toolStripContainer3 = new ToolStripContainer();
            lvWhenToWatch = new ListViewFlickerFree();
            columnHeader29 = new ColumnHeader();
            columnHeader30 = new ColumnHeader();
            columnHeader31 = new ColumnHeader();
            columnHeader32 = new ColumnHeader();
            columnHeader36 = new ColumnHeader();
            columnHeader33 = new ColumnHeader();
            columnHeader34 = new ColumnHeader();
            columnHeader1 = new ColumnHeader();
            columnHeader35 = new ColumnHeader();
            tsWtW = new ToolStrip();
            btnWhenToWatchCheck = new ToolStripButton();
            btnScheduleBTSearch = new ToolStripSplitButton();
            tsbScheduleJackettSearch = new ToolStripButton();
            toolStripSeparator12 = new ToolStripSeparator();
            btnScheduleRightClick = new ToolStripButton();
            ilNewIcons = new ImageList(components);
            imageList1 = new ImageList(components);
            tableLayoutPanel2 = new TableLayoutPanel();
            pbProgressBarx = new ProgressBar();
            txtDLStatusLabel = new Label();
            tsNextShowTxt = new Label();
            columnHeader5 = new ColumnHeader();
            columnHeader6 = new ColumnHeader();
            columnHeader7 = new ColumnHeader();
            columnHeader8 = new ColumnHeader();
            columnHeader25 = new ColumnHeader();
            columnHeader26 = new ColumnHeader();
            columnHeader27 = new ColumnHeader();
            columnHeader28 = new ColumnHeader();
            openFile = new OpenFileDialog();
            folderBrowser = new FolderBrowserDialog();
            refreshWTWTimer = new Timer(components);
            notifyIcon1 = new NotifyIcon(components);
            showRightClickMenu = new ContextMenuStrip(components);
            statusTimer = new Timer(components);
            BGDownloadTimer = new Timer(components);
            UpdateTimer = new Timer(components);
            saveFile = new SaveFileDialog();
            tmrShowUpcomingPopup = new Timer(components);
            quickTimer = new Timer(components);
            tmrPeriodicScan = new Timer(components);
            toolTip1 = new ToolTip(components);
            btnUpdateAvailable = new Button();
            bwSeasonHTMLGenerator = new System.ComponentModel.BackgroundWorker();
            bwUpdateSchedule = new System.ComponentModel.BackgroundWorker();
            bwShowHTMLGenerator = new System.ComponentModel.BackgroundWorker();
            bwShowSummaryHTMLGenerator = new System.ComponentModel.BackgroundWorker();
            bwSeasonSummaryHTMLGenerator = new System.ComponentModel.BackgroundWorker();
            bwMovieHTMLGenerator = new System.ComponentModel.BackgroundWorker();
            bwScan = new System.ComponentModel.BackgroundWorker();
            bwAction = new System.ComponentModel.BackgroundWorker();
            tableLayoutPanel3 = new TableLayoutPanel();
            panel1 = new Panel();
            removeShowsWithNoFoldersToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            tabControl1.SuspendLayout();
            tbMyMovies.SuspendLayout();
            toolStripContainer1.ContentPanel.SuspendLayout();
            toolStripContainer1.TopToolStripPanel.SuspendLayout();
            toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            tabControl3.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tpMovieTrailer.SuspendLayout();
            toolStrip1.SuspendLayout();
            tbMyShows.SuspendLayout();
            toolStripContainer2.ContentPanel.SuspendLayout();
            toolStripContainer2.TopToolStripPanel.SuspendLayout();
            toolStripContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabControl2.SuspendLayout();
            tpInformation.SuspendLayout();
            tpImages.SuspendLayout();
            tpSummary.SuspendLayout();
            tabPage3.SuspendLayout();
            tsMyShows.SuspendLayout();
            tbAllInOne.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)olvAction).BeginInit();
            tsScanResults.SuspendLayout();
            tbWTW.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            toolStripContainer3.ContentPanel.SuspendLayout();
            toolStripContainer3.TopToolStripPanel.SuspendLayout();
            toolStripContainer3.SuspendLayout();
            tsWtW.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, optionsToolStripMenuItem, toolsToolStripMenuItem, viewToolStripMenuItem, betaToolsToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(7, 2, 0, 2);
            menuStrip1.Size = new System.Drawing.Size(1102, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportToolStripMenuItem, saveToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // exportToolStripMenuItem
            // 
            exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            exportToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.E;
            exportToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            exportToolStripMenuItem.Text = "&Export";
            exportToolStripMenuItem.Click += exportToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("saveToolStripMenuItem.Image");
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            saveToolStripMenuItem.Text = "&Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(145, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
            exitToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            exitToolStripMenuItem.Text = "E&xit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { offlineOperationToolStripMenuItem, backgroundDownloadToolStripMenuItem, toolStripSeparator2, preferencesToolStripMenuItem, ignoreListToolStripMenuItem, filenameTemplateEditorToolStripMenuItem, movieSearchEnginesToolStripMenuItem, searchEnginesToolStripMenuItem, filenameProcessorsToolStripMenuItem, toolStripSeparator20, settingsCheckToolStripMenuItem });
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            optionsToolStripMenuItem.Text = "&Options";
            // 
            // offlineOperationToolStripMenuItem
            // 
            offlineOperationToolStripMenuItem.Name = "offlineOperationToolStripMenuItem";
            offlineOperationToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            offlineOperationToolStripMenuItem.Text = "&Offline Operation";
            offlineOperationToolStripMenuItem.ToolTipText = "If you turn this on, TVRename will only use data it has locally, without downloading anything.";
            offlineOperationToolStripMenuItem.Click += offlineOperationToolStripMenuItem_Click;
            // 
            // backgroundDownloadToolStripMenuItem
            // 
            backgroundDownloadToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("backgroundDownloadToolStripMenuItem.Image");
            backgroundDownloadToolStripMenuItem.Name = "backgroundDownloadToolStripMenuItem";
            backgroundDownloadToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            backgroundDownloadToolStripMenuItem.Text = "Automatic &Background Download";
            backgroundDownloadToolStripMenuItem.ToolTipText = "Turn this on to let TVRename automatically download thetvdb.com data in the background";
            backgroundDownloadToolStripMenuItem.Click += backgroundDownloadToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(251, 6);
            // 
            // preferencesToolStripMenuItem
            // 
            preferencesToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("preferencesToolStripMenuItem.Image");
            preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            preferencesToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.P;
            preferencesToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            preferencesToolStripMenuItem.Text = "&Preferences";
            preferencesToolStripMenuItem.Click += preferencesToolStripMenuItem_Click;
            // 
            // ignoreListToolStripMenuItem
            // 
            ignoreListToolStripMenuItem.Name = "ignoreListToolStripMenuItem";
            ignoreListToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.I;
            ignoreListToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            ignoreListToolStripMenuItem.Text = "&Ignore List";
            ignoreListToolStripMenuItem.Click += ignoreListToolStripMenuItem_Click;
            // 
            // filenameTemplateEditorToolStripMenuItem
            // 
            filenameTemplateEditorToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("filenameTemplateEditorToolStripMenuItem.Image");
            filenameTemplateEditorToolStripMenuItem.Name = "filenameTemplateEditorToolStripMenuItem";
            filenameTemplateEditorToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.T;
            filenameTemplateEditorToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            filenameTemplateEditorToolStripMenuItem.Text = "&Filename Template Editor";
            filenameTemplateEditorToolStripMenuItem.Click += filenameTemplateEditorToolStripMenuItem_Click;
            // 
            // movieSearchEnginesToolStripMenuItem
            // 
            movieSearchEnginesToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("movieSearchEnginesToolStripMenuItem.Image");
            movieSearchEnginesToolStripMenuItem.Name = "movieSearchEnginesToolStripMenuItem";
            movieSearchEnginesToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            movieSearchEnginesToolStripMenuItem.Text = "Movie &Search Engines";
            movieSearchEnginesToolStripMenuItem.Click += movieSearchEnginesToolStripMenuItem_Click;
            // 
            // searchEnginesToolStripMenuItem
            // 
            searchEnginesToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("searchEnginesToolStripMenuItem.Image");
            searchEnginesToolStripMenuItem.Name = "searchEnginesToolStripMenuItem";
            searchEnginesToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            searchEnginesToolStripMenuItem.Text = "TV Show &Search Engines";
            searchEnginesToolStripMenuItem.Click += searchEnginesToolStripMenuItem_Click;
            // 
            // filenameProcessorsToolStripMenuItem
            // 
            filenameProcessorsToolStripMenuItem.Name = "filenameProcessorsToolStripMenuItem";
            filenameProcessorsToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            filenameProcessorsToolStripMenuItem.Text = "File&name Processors";
            filenameProcessorsToolStripMenuItem.Click += filenameProcessorsToolStripMenuItem_Click;
            // 
            // toolStripSeparator20
            // 
            toolStripSeparator20.Name = "toolStripSeparator20";
            toolStripSeparator20.Size = new System.Drawing.Size(251, 6);
            // 
            // settingsCheckToolStripMenuItem
            // 
            settingsCheckToolStripMenuItem.Name = "settingsCheckToolStripMenuItem";
            settingsCheckToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            settingsCheckToolStripMenuItem.Text = "Settings Check";
            settingsCheckToolStripMenuItem.Click += settingsCheckToolStripMenuItem_Click;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { flushCacheToolStripMenuItem, flushImageCacheToolStripMenuItem, backgroundDownloadNowToolStripMenuItem, toolStripSeparator17, folderMonitorToolStripMenuItem, bulkAddMoviesToolStripMenuItem, toolStripSeparator3, tsmiOrphanFiles, duplicateFinderLOGToolStripMenuItem, duplicateMoviesToolStripMenuItem, toolStripSeparator18, quickRenameToolStripMenuItem, scanMovieFolderToolStripMenuItem, toolStripSeparator21, movieRecommendationsToolStripMenuItem, tvRecommendationsToolStripMenuItem, toolStripSeparator121, cleanLibraryFoldersToolStripMenuItem, forceRefreshKodiTVShowNFOFIlesToolStripMenuItem, removeShowsWithNoFoldersToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            toolsToolStripMenuItem.Text = "&Tools";
            // 
            // flushCacheToolStripMenuItem
            // 
            flushCacheToolStripMenuItem.Name = "flushCacheToolStripMenuItem";
            flushCacheToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            flushCacheToolStripMenuItem.Text = "&Force Refesh All";
            flushCacheToolStripMenuItem.Click += flushCacheToolStripMenuItem_Click;
            // 
            // flushImageCacheToolStripMenuItem
            // 
            flushImageCacheToolStripMenuItem.Name = "flushImageCacheToolStripMenuItem";
            flushImageCacheToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            flushImageCacheToolStripMenuItem.Text = "&Force Refesh All Images";
            flushImageCacheToolStripMenuItem.Click += flushImageCacheToolStripMenuItem_Click;
            // 
            // backgroundDownloadNowToolStripMenuItem
            // 
            backgroundDownloadNowToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("backgroundDownloadNowToolStripMenuItem.Image");
            backgroundDownloadNowToolStripMenuItem.Name = "backgroundDownloadNowToolStripMenuItem";
            backgroundDownloadNowToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.B;
            backgroundDownloadNowToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            backgroundDownloadNowToolStripMenuItem.Text = "&Background Download Now";
            backgroundDownloadNowToolStripMenuItem.Click += backgroundDownloadNowToolStripMenuItem_Click;
            // 
            // toolStripSeparator17
            // 
            toolStripSeparator17.Name = "toolStripSeparator17";
            toolStripSeparator17.Size = new System.Drawing.Size(274, 6);
            // 
            // folderMonitorToolStripMenuItem
            // 
            folderMonitorToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("folderMonitorToolStripMenuItem.Image");
            folderMonitorToolStripMenuItem.Name = "folderMonitorToolStripMenuItem";
            folderMonitorToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            folderMonitorToolStripMenuItem.Text = "Bulk &Add TV Shows...";
            folderMonitorToolStripMenuItem.Click += folderMonitorToolStripMenuItem_Click;
            // 
            // bulkAddMoviesToolStripMenuItem
            // 
            bulkAddMoviesToolStripMenuItem.Name = "bulkAddMoviesToolStripMenuItem";
            bulkAddMoviesToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            bulkAddMoviesToolStripMenuItem.Text = "Bulk Add Movies...";
            bulkAddMoviesToolStripMenuItem.Click += bulkAddMoviesToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(274, 6);
            // 
            // tsmiOrphanFiles
            // 
            tsmiOrphanFiles.Name = "tsmiOrphanFiles";
            tsmiOrphanFiles.Size = new System.Drawing.Size(277, 26);
            tsmiOrphanFiles.Text = "Find Orphan Media Files....";
            tsmiOrphanFiles.Click += ToolStripMenuItem1_Click;
            // 
            // duplicateFinderLOGToolStripMenuItem
            // 
            duplicateFinderLOGToolStripMenuItem.Name = "duplicateFinderLOGToolStripMenuItem";
            duplicateFinderLOGToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            duplicateFinderLOGToolStripMenuItem.Text = "Find Merged Episodes...";
            duplicateFinderLOGToolStripMenuItem.Click += duplicateFinderLOGToolStripMenuItem_Click;
            // 
            // duplicateMoviesToolStripMenuItem
            // 
            duplicateMoviesToolStripMenuItem.Name = "duplicateMoviesToolStripMenuItem";
            duplicateMoviesToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            duplicateMoviesToolStripMenuItem.Text = "Find Duplicate Movies...";
            duplicateMoviesToolStripMenuItem.Click += duplicateMoviesToolStripMenuItem_Click;
            // 
            // toolStripSeparator18
            // 
            toolStripSeparator18.Name = "toolStripSeparator18";
            toolStripSeparator18.Size = new System.Drawing.Size(274, 6);
            // 
            // quickRenameToolStripMenuItem
            // 
            quickRenameToolStripMenuItem.Name = "quickRenameToolStripMenuItem";
            quickRenameToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            quickRenameToolStripMenuItem.Text = "Quick Rename TV Files...";
            quickRenameToolStripMenuItem.Click += QuickRenameToolStripMenuItem_Click;
            // 
            // scanMovieFolderToolStripMenuItem
            // 
            scanMovieFolderToolStripMenuItem.Name = "scanMovieFolderToolStripMenuItem";
            scanMovieFolderToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            scanMovieFolderToolStripMenuItem.Text = "Move Movies From...";
            scanMovieFolderToolStripMenuItem.Click += scanMovieFolderToolStripMenuItem_Click;
            // 
            // toolStripSeparator21
            // 
            toolStripSeparator21.Name = "toolStripSeparator21";
            toolStripSeparator21.Size = new System.Drawing.Size(274, 6);
            // 
            // movieRecommendationsToolStripMenuItem
            // 
            movieRecommendationsToolStripMenuItem.Name = "movieRecommendationsToolStripMenuItem";
            movieRecommendationsToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            movieRecommendationsToolStripMenuItem.Text = "Movie Recommendations...";
            movieRecommendationsToolStripMenuItem.Click += movieRecommendationsToolStripMenuItem_Click;
            // 
            // tvRecommendationsToolStripMenuItem
            // 
            tvRecommendationsToolStripMenuItem.Name = "tvRecommendationsToolStripMenuItem";
            tvRecommendationsToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            tvRecommendationsToolStripMenuItem.Text = "TV Show Recommendations...";
            tvRecommendationsToolStripMenuItem.Click += recommendationsToolStripMenuItem_Click;
            // 
            // toolStripSeparator121
            // 
            toolStripSeparator121.Name = "toolStripSeparator121";
            toolStripSeparator121.Size = new System.Drawing.Size(274, 6);
            // 
            // cleanLibraryFoldersToolStripMenuItem
            // 
            cleanLibraryFoldersToolStripMenuItem.Name = "cleanLibraryFoldersToolStripMenuItem";
            cleanLibraryFoldersToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            cleanLibraryFoldersToolStripMenuItem.Text = "Clean Empty Library Folders...";
            cleanLibraryFoldersToolStripMenuItem.Click += cleanLibraryFoldersToolStripMenuItem_Click;
            // 
            // forceRefreshKodiTVShowNFOFIlesToolStripMenuItem
            // 
            forceRefreshKodiTVShowNFOFIlesToolStripMenuItem.Name = "forceRefreshKodiTVShowNFOFIlesToolStripMenuItem";
            forceRefreshKodiTVShowNFOFIlesToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            forceRefreshKodiTVShowNFOFIlesToolStripMenuItem.Text = "Force Refresh Kodi TV Show NFO FIles";
            forceRefreshKodiTVShowNFOFIlesToolStripMenuItem.Click += forceRefreshKodiTVShowNFOFIlesToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { statisticsToolStripMenuItem, toolStripSeparator5, showSummaryToolStripMenuItem, movieCollectionSummaryLogToolStripMenuItem, actorsToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            viewToolStripMenuItem.Text = "View";
            // 
            // statisticsToolStripMenuItem
            // 
            statisticsToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("statisticsToolStripMenuItem.Image");
            statisticsToolStripMenuItem.Name = "statisticsToolStripMenuItem";
            statisticsToolStripMenuItem.Size = new System.Drawing.Size(231, 26);
            statisticsToolStripMenuItem.Text = "&Statistics...";
            statisticsToolStripMenuItem.Click += statisticsToolStripMenuItem_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new System.Drawing.Size(228, 6);
            // 
            // showSummaryToolStripMenuItem
            // 
            showSummaryToolStripMenuItem.Name = "showSummaryToolStripMenuItem";
            showSummaryToolStripMenuItem.Size = new System.Drawing.Size(231, 26);
            showSummaryToolStripMenuItem.Text = "TV Show Summary...";
            showSummaryToolStripMenuItem.Click += showSummaryToolStripMenuItem_Click;
            // 
            // movieCollectionSummaryLogToolStripMenuItem
            // 
            movieCollectionSummaryLogToolStripMenuItem.Name = "movieCollectionSummaryLogToolStripMenuItem";
            movieCollectionSummaryLogToolStripMenuItem.Size = new System.Drawing.Size(231, 26);
            movieCollectionSummaryLogToolStripMenuItem.Text = "Movie Collection Summary...";
            movieCollectionSummaryLogToolStripMenuItem.Click += movieCollectionSummaryLogToolStripMenuItem_Click;
            // 
            // actorsToolStripMenuItem
            // 
            actorsToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("actorsToolStripMenuItem.Image");
            actorsToolStripMenuItem.Name = "actorsToolStripMenuItem";
            actorsToolStripMenuItem.Size = new System.Drawing.Size(231, 26);
            actorsToolStripMenuItem.Text = "TV Show &Actors Grid...";
            actorsToolStripMenuItem.Click += actorsToolStripMenuItem_Click;
            // 
            // betaToolsToolStripMenuItem
            // 
            betaToolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { timezoneInconsistencyLOGToolStripMenuItem, episodeFileQualitySummaryLogToolStripMenuItem, toolStripSeparator19, accuracyCheckLogToolStripMenuItem, tMDBAccuracyCheckLogToolStripMenuItem, tVDBUPdateCheckerLogToolStripMenuItem, toolStripSeparator24, yTSMoviePreviewToolStripMenuItem, yTSMovieRecommendationsToolStripMenuItem });
            betaToolsToolStripMenuItem.Name = "betaToolsToolStripMenuItem";
            betaToolsToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            betaToolsToolStripMenuItem.Text = "Beta";
            // 
            // timezoneInconsistencyLOGToolStripMenuItem
            // 
            timezoneInconsistencyLOGToolStripMenuItem.Name = "timezoneInconsistencyLOGToolStripMenuItem";
            timezoneInconsistencyLOGToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            timezoneInconsistencyLOGToolStripMenuItem.Text = "Timezone Inconsistency (Log)";
            timezoneInconsistencyLOGToolStripMenuItem.Click += timezoneInconsistencyLOGToolStripMenuItem_Click;
            // 
            // episodeFileQualitySummaryLogToolStripMenuItem
            // 
            episodeFileQualitySummaryLogToolStripMenuItem.Name = "episodeFileQualitySummaryLogToolStripMenuItem";
            episodeFileQualitySummaryLogToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            episodeFileQualitySummaryLogToolStripMenuItem.Text = "Episode File Quality Summary (Log)";
            episodeFileQualitySummaryLogToolStripMenuItem.Click += episodeFileQualitySummaryLogToolStripMenuItem_Click;
            // 
            // toolStripSeparator19
            // 
            toolStripSeparator19.Name = "toolStripSeparator19";
            toolStripSeparator19.Size = new System.Drawing.Size(259, 6);
            // 
            // accuracyCheckLogToolStripMenuItem
            // 
            accuracyCheckLogToolStripMenuItem.Name = "accuracyCheckLogToolStripMenuItem";
            accuracyCheckLogToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            accuracyCheckLogToolStripMenuItem.Text = "TVDB Accuracy Check (Log)";
            accuracyCheckLogToolStripMenuItem.Click += AccuracyCheckLogToolStripMenuItem_Click;
            // 
            // tMDBAccuracyCheckLogToolStripMenuItem
            // 
            tMDBAccuracyCheckLogToolStripMenuItem.Name = "tMDBAccuracyCheckLogToolStripMenuItem";
            tMDBAccuracyCheckLogToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            tMDBAccuracyCheckLogToolStripMenuItem.Text = "TMDB Accuracy Check (Log)";
            tMDBAccuracyCheckLogToolStripMenuItem.Click += tMDBAccuracyCheckLogToolStripMenuItem_Click;
            // 
            // tVDBUPdateCheckerLogToolStripMenuItem
            // 
            tVDBUPdateCheckerLogToolStripMenuItem.Name = "tVDBUPdateCheckerLogToolStripMenuItem";
            tVDBUPdateCheckerLogToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            tVDBUPdateCheckerLogToolStripMenuItem.Text = "TVDB Update Checker (Log)";
            tVDBUPdateCheckerLogToolStripMenuItem.Click += tVDBUPdateCheckerLogToolStripMenuItem_Click;
            // 
            // toolStripSeparator24
            // 
            toolStripSeparator24.Name = "toolStripSeparator24";
            toolStripSeparator24.Size = new System.Drawing.Size(259, 6);
            // 
            // yTSMoviePreviewToolStripMenuItem
            // 
            yTSMoviePreviewToolStripMenuItem.Name = "yTSMoviePreviewToolStripMenuItem";
            yTSMoviePreviewToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            yTSMoviePreviewToolStripMenuItem.Text = "YTS Movie Preview";
            yTSMoviePreviewToolStripMenuItem.Click += yTSMoviePreviewToolStripMenuItem_Click;
            // 
            // yTSMovieRecommendationsToolStripMenuItem
            // 
            yTSMovieRecommendationsToolStripMenuItem.Name = "yTSMovieRecommendationsToolStripMenuItem";
            yTSMovieRecommendationsToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            yTSMovieRecommendationsToolStripMenuItem.Text = "YTS Movie Recommendations";
            yTSMovieRecommendationsToolStripMenuItem.Click += yTSMovieRecommendationsToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { quickstartGuideToolStripMenuItem, visitWebsiteToolStripMenuItem, visitSupportForumToolStripMenuItem, bugReportToolStripMenuItem, toolStripSeparator7, requestANewFeatureToolStripMenuItem, buyMeADrinkToolStripMenuItem, toolStripSeparator6, browserTestToolStripMenuItem, checkForNewVersionToolStripMenuItem, toolStripSeparator8, logToolStripMenuItem, thanksToolStripMenuItem, toolStripSeparator23, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            helpToolStripMenuItem.Text = "&Help";
            // 
            // quickstartGuideToolStripMenuItem
            // 
            quickstartGuideToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("quickstartGuideToolStripMenuItem.Image");
            quickstartGuideToolStripMenuItem.Name = "quickstartGuideToolStripMenuItem";
            quickstartGuideToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            quickstartGuideToolStripMenuItem.Text = "&Quickstart Guide";
            quickstartGuideToolStripMenuItem.Click += quickstartGuideToolStripMenuItem_Click;
            // 
            // visitWebsiteToolStripMenuItem
            // 
            visitWebsiteToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("visitWebsiteToolStripMenuItem.Image");
            visitWebsiteToolStripMenuItem.Name = "visitWebsiteToolStripMenuItem";
            visitWebsiteToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            visitWebsiteToolStripMenuItem.Text = "&Visit Website";
            visitWebsiteToolStripMenuItem.Click += visitWebsiteToolStripMenuItem_Click;
            // 
            // visitSupportForumToolStripMenuItem
            // 
            visitSupportForumToolStripMenuItem.Name = "visitSupportForumToolStripMenuItem";
            visitSupportForumToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            visitSupportForumToolStripMenuItem.Text = "Visit Support Forum";
            visitSupportForumToolStripMenuItem.Click += visitSupportForumToolStripMenuItem_Click;
            // 
            // bugReportToolStripMenuItem
            // 
            bugReportToolStripMenuItem.Name = "bugReportToolStripMenuItem";
            bugReportToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            bugReportToolStripMenuItem.Text = "Bug &Report";
            bugReportToolStripMenuItem.Click += bugReportToolStripMenuItem_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new System.Drawing.Size(193, 6);
            // 
            // requestANewFeatureToolStripMenuItem
            // 
            requestANewFeatureToolStripMenuItem.Name = "requestANewFeatureToolStripMenuItem";
            requestANewFeatureToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            requestANewFeatureToolStripMenuItem.Text = "Request A New Feature";
            requestANewFeatureToolStripMenuItem.Click += requestANewFeatureToolStripMenuItem_Click;
            // 
            // buyMeADrinkToolStripMenuItem
            // 
            buyMeADrinkToolStripMenuItem.Name = "buyMeADrinkToolStripMenuItem";
            buyMeADrinkToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            buyMeADrinkToolStripMenuItem.Text = "&Buy Me A Drink";
            buyMeADrinkToolStripMenuItem.Click += buyMeADrinkToolStripMenuItem_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new System.Drawing.Size(193, 6);
            // 
            // browserTestToolStripMenuItem
            // 
            browserTestToolStripMenuItem.Name = "browserTestToolStripMenuItem";
            browserTestToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            browserTestToolStripMenuItem.Text = "Browser Test";
            browserTestToolStripMenuItem.Click += browserTestToolStripMenuItem_Click;
            // 
            // checkForNewVersionToolStripMenuItem
            // 
            checkForNewVersionToolStripMenuItem.Name = "checkForNewVersionToolStripMenuItem";
            checkForNewVersionToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            checkForNewVersionToolStripMenuItem.Text = "Check For New Version";
            checkForNewVersionToolStripMenuItem.Click += checkForNewVersionToolStripMenuItem_Click;
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new System.Drawing.Size(193, 6);
            // 
            // logToolStripMenuItem
            // 
            logToolStripMenuItem.Name = "logToolStripMenuItem";
            logToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            logToolStripMenuItem.Text = "Log";
            logToolStripMenuItem.Click += logToolStripMenuItem_Click;
            // 
            // thanksToolStripMenuItem
            // 
            thanksToolStripMenuItem.Name = "thanksToolStripMenuItem";
            thanksToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            thanksToolStripMenuItem.Text = "Thanks";
            thanksToolStripMenuItem.Click += ThanksToolStripMenuItem_Click;
            // 
            // toolStripSeparator23
            // 
            toolStripSeparator23.Name = "toolStripSeparator23";
            toolStripSeparator23.Size = new System.Drawing.Size(193, 6);
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // toolStripSeparator22
            // 
            toolStripSeparator22.Name = "toolStripSeparator22";
            toolStripSeparator22.Size = new System.Drawing.Size(6, 6);
            // 
            // webTestToolStripMenuItem
            // 
            webTestToolStripMenuItem.Name = "webTestToolStripMenuItem";
            webTestToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // downloadInstallerToolStripMenuItem
            // 
            downloadInstallerToolStripMenuItem.Name = "downloadInstallerToolStripMenuItem";
            downloadInstallerToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // tabControl1
            // 
            tabControl1.Alignment = TabAlignment.Left;
            tabControl1.Controls.Add(tbMyMovies);
            tabControl1.Controls.Add(tbMyShows);
            tabControl1.Controls.Add(tbAllInOne);
            tabControl1.Controls.Add(tbWTW);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.ImageList = ilNewIcons;
            tabControl1.ItemSize = new System.Drawing.Size(100, 100);
            tabControl1.Location = new System.Drawing.Point(0, 0);
            tabControl1.Margin = new Padding(4, 3, 4, 3);
            tabControl1.Multiline = true;
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(1102, 690);
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.TabIndex = 0;
            tabControl1.DrawItem += TabControl1_DrawItem;
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            tabControl1.DoubleClick += tabControl1_DoubleClick;
            // 
            // tbMyMovies
            // 
            tbMyMovies.Controls.Add(toolStripContainer1);
            tbMyMovies.ImageKey = "4632196-48.png";
            tbMyMovies.Location = new System.Drawing.Point(104, 4);
            tbMyMovies.Margin = new Padding(4, 3, 4, 3);
            tbMyMovies.Name = "tbMyMovies";
            tbMyMovies.Size = new System.Drawing.Size(994, 682);
            tbMyMovies.TabIndex = 12;
            tbMyMovies.Text = "Movies";
            tbMyMovies.UseVisualStyleBackColor = true;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            toolStripContainer1.ContentPanel.Controls.Add(splitContainer2);
            toolStripContainer1.ContentPanel.Margin = new Padding(2);
            toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(994, 643);
            toolStripContainer1.Dock = DockStyle.Fill;
            toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            toolStripContainer1.Margin = new Padding(2);
            toolStripContainer1.Name = "toolStripContainer1";
            toolStripContainer1.Size = new System.Drawing.Size(994, 682);
            toolStripContainer1.TabIndex = 11;
            toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            toolStripContainer1.TopToolStripPanel.Controls.Add(toolStrip1);
            // 
            // splitContainer2
            // 
            splitContainer2.BorderStyle = BorderStyle.Fixed3D;
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.FixedPanel = FixedPanel.Panel1;
            splitContainer2.Location = new System.Drawing.Point(0, 0);
            splitContainer2.Margin = new Padding(4, 3, 4, 3);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(movieTree);
            splitContainer2.Panel1.Controls.Add(filterMoviesTextbox);
            splitContainer2.Panel1MinSize = 100;
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(tabControl3);
            splitContainer2.Panel2MinSize = 100;
            splitContainer2.Size = new System.Drawing.Size(994, 643);
            splitContainer2.SplitterDistance = 362;
            splitContainer2.SplitterWidth = 5;
            splitContainer2.TabIndex = 12;
            // 
            // movieTree
            // 
            movieTree.BorderStyle = BorderStyle.None;
            movieTree.Dock = DockStyle.Fill;
            movieTree.HideSelection = false;
            movieTree.Location = new System.Drawing.Point(0, 23);
            movieTree.Margin = new Padding(4, 3, 4, 3);
            movieTree.Name = "movieTree";
            movieTree.Size = new System.Drawing.Size(358, 616);
            movieTree.TabIndex = 0;
            movieTree.AfterSelect += MyMoviesTree_AfterSelect;
            movieTree.MouseClick += MyMoviesTree_MouseClick;
            // 
            // filterMoviesTextbox
            // 
            filterMoviesTextbox.Dock = DockStyle.Top;
            filterMoviesTextbox.Location = new System.Drawing.Point(0, 0);
            filterMoviesTextbox.Margin = new Padding(4, 3, 4, 3);
            filterMoviesTextbox.Name = "filterMoviesTextbox";
            filterMoviesTextbox.Size = new System.Drawing.Size(358, 23);
            filterMoviesTextbox.TabIndex = 1;
            filterMoviesTextbox.SizeChanged += filterMoviesTextBox_SizeChanged;
            filterMoviesTextbox.TextChanged += filterMoviesTextBox_TextChanged;
            // 
            // tabControl3
            // 
            tabControl3.Appearance = TabAppearance.FlatButtons;
            tabControl3.Controls.Add(tabPage1);
            tabControl3.Controls.Add(tabPage2);
            tabControl3.Controls.Add(tpMovieTrailer);
            tabControl3.Dock = DockStyle.Fill;
            tabControl3.Location = new System.Drawing.Point(0, 0);
            tabControl3.Margin = new Padding(4, 3, 4, 3);
            tabControl3.Name = "tabControl3";
            tabControl3.SelectedIndex = 0;
            tabControl3.Size = new System.Drawing.Size(623, 639);
            tabControl3.TabIndex = 7;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(chrMovieInformation);
            tabPage1.Location = new System.Drawing.Point(4, 27);
            tabPage1.Margin = new Padding(4, 3, 4, 3);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(4, 3, 4, 3);
            tabPage1.Size = new System.Drawing.Size(615, 608);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Information";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // chrMovieInformation
            // 
            chrMovieInformation.ActivateBrowserOnCreation = false;
            chrMovieInformation.Dock = DockStyle.Fill;
            chrMovieInformation.Location = new System.Drawing.Point(4, 3);
            chrMovieInformation.Margin = new Padding(4, 3, 4, 3);
            chrMovieInformation.Name = "chrMovieInformation";
            chrMovieInformation.Size = new System.Drawing.Size(607, 602);
            chrMovieInformation.TabIndex = 1;
            chrMovieInformation.Visible = false;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(chrMovieImages);
            tabPage2.Location = new System.Drawing.Point(4, 27);
            tabPage2.Margin = new Padding(4, 3, 4, 3);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(4, 3, 4, 3);
            tabPage2.Size = new System.Drawing.Size(615, 608);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Images";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // chrMovieImages
            // 
            chrMovieImages.ActivateBrowserOnCreation = false;
            chrMovieImages.Dock = DockStyle.Fill;
            chrMovieImages.Location = new System.Drawing.Point(4, 3);
            chrMovieImages.Margin = new Padding(4, 3, 4, 3);
            chrMovieImages.Name = "chrMovieImages";
            chrMovieImages.Size = new System.Drawing.Size(607, 602);
            chrMovieImages.TabIndex = 1;
            chrMovieImages.Visible = false;
            // 
            // tpMovieTrailer
            // 
            tpMovieTrailer.Controls.Add(chrMovieTrailer);
            tpMovieTrailer.Location = new System.Drawing.Point(4, 27);
            tpMovieTrailer.Margin = new Padding(4, 3, 4, 3);
            tpMovieTrailer.Name = "tpMovieTrailer";
            tpMovieTrailer.Padding = new Padding(4, 3, 4, 3);
            tpMovieTrailer.Size = new System.Drawing.Size(615, 608);
            tpMovieTrailer.TabIndex = 2;
            tpMovieTrailer.Text = "Trailer";
            tpMovieTrailer.UseVisualStyleBackColor = true;
            // 
            // chrMovieTrailer
            // 
            chrMovieTrailer.ActivateBrowserOnCreation = false;
            chrMovieTrailer.Dock = DockStyle.Fill;
            chrMovieTrailer.Location = new System.Drawing.Point(4, 3);
            chrMovieTrailer.Margin = new Padding(4, 3, 4, 3);
            chrMovieTrailer.Name = "chrMovieTrailer";
            chrMovieTrailer.Size = new System.Drawing.Size(607, 602);
            chrMovieTrailer.TabIndex = 2;
            chrMovieTrailer.Visible = false;
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.None;
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.ImageScalingSize = new System.Drawing.Size(28, 28);
            toolStrip1.Items.AddRange(new ToolStripItem[] { tbnAddMovie, btnEditMovie, btnMovieDelete, toolStripSeparator15, btnMovieRefresh, tsbScanMovies, btnMovieFilter, toolStripSeparator16, tsbMyMoviesContextMenu });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Padding = new Padding(0, 0, 4, 0);
            toolStrip1.Size = new System.Drawing.Size(994, 39);
            toolStrip1.Stretch = true;
            toolStrip1.TabIndex = 13;
            toolStrip1.Text = "toolStrip1";
            // 
            // tbnAddMovie
            // 
            tbnAddMovie.Image = (System.Drawing.Image)resources.GetObject("tbnAddMovie.Image");
            tbnAddMovie.ImageScaling = ToolStripItemImageScaling.None;
            tbnAddMovie.Name = "tbnAddMovie";
            tbnAddMovie.Size = new System.Drawing.Size(65, 36);
            tbnAddMovie.Text = "&Add";
            tbnAddMovie.Click += AddMovie_Click;
            // 
            // btnEditMovie
            // 
            btnEditMovie.Image = (System.Drawing.Image)resources.GetObject("btnEditMovie.Image");
            btnEditMovie.ImageScaling = ToolStripItemImageScaling.None;
            btnEditMovie.Name = "btnEditMovie";
            btnEditMovie.Size = new System.Drawing.Size(63, 36);
            btnEditMovie.Text = "&Edit";
            btnEditMovie.Click += toolStripButton2_Click;
            // 
            // btnMovieDelete
            // 
            btnMovieDelete.Image = (System.Drawing.Image)resources.GetObject("btnMovieDelete.Image");
            btnMovieDelete.ImageScaling = ToolStripItemImageScaling.None;
            btnMovieDelete.Name = "btnMovieDelete";
            btnMovieDelete.Size = new System.Drawing.Size(76, 36);
            btnMovieDelete.Text = "&Delete";
            btnMovieDelete.Click += btnMovieDelete_Click;
            // 
            // toolStripSeparator15
            // 
            toolStripSeparator15.Name = "toolStripSeparator15";
            toolStripSeparator15.Size = new System.Drawing.Size(6, 39);
            // 
            // btnMovieRefresh
            // 
            btnMovieRefresh.Image = (System.Drawing.Image)resources.GetObject("btnMovieRefresh.Image");
            btnMovieRefresh.ImageScaling = ToolStripItemImageScaling.None;
            btnMovieRefresh.Name = "btnMovieRefresh";
            btnMovieRefresh.Size = new System.Drawing.Size(82, 36);
            btnMovieRefresh.Text = "&Refresh";
            btnMovieRefresh.Click += btnMovieRefresh_Click;
            // 
            // tsbScanMovies
            // 
            tsbScanMovies.Image = (System.Drawing.Image)resources.GetObject("tsbScanMovies.Image");
            tsbScanMovies.ImageScaling = ToolStripItemImageScaling.None;
            tsbScanMovies.Name = "tsbScanMovies";
            tsbScanMovies.Size = new System.Drawing.Size(109, 36);
            tsbScanMovies.Text = "Scan Movies";
            tsbScanMovies.Click += toolStripButton1_Click_1;
            // 
            // btnMovieFilter
            // 
            btnMovieFilter.BackColor = System.Drawing.Color.Transparent;
            btnMovieFilter.Image = (System.Drawing.Image)resources.GetObject("btnMovieFilter.Image");
            btnMovieFilter.ImageScaling = ToolStripItemImageScaling.None;
            btnMovieFilter.Name = "btnMovieFilter";
            btnMovieFilter.Size = new System.Drawing.Size(69, 36);
            btnMovieFilter.Text = "&Filter";
            btnMovieFilter.Click += btnMovieFilter_Click;
            // 
            // toolStripSeparator16
            // 
            toolStripSeparator16.Name = "toolStripSeparator16";
            toolStripSeparator16.Size = new System.Drawing.Size(6, 39);
            // 
            // tsbMyMoviesContextMenu
            // 
            tsbMyMoviesContextMenu.Image = (System.Drawing.Image)resources.GetObject("tsbMyMoviesContextMenu.Image");
            tsbMyMoviesContextMenu.ImageScaling = ToolStripItemImageScaling.None;
            tsbMyMoviesContextMenu.Name = "tsbMyMoviesContextMenu";
            tsbMyMoviesContextMenu.Size = new System.Drawing.Size(119, 36);
            tsbMyMoviesContextMenu.Text = "Context Menu";
            tsbMyMoviesContextMenu.Click += tsbMyMoviesContextMenu_Click;
            // 
            // tbMyShows
            // 
            tbMyShows.Controls.Add(toolStripContainer2);
            tbMyShows.ImageKey = "3790574-48.png";
            tbMyShows.Location = new System.Drawing.Point(104, 4);
            tbMyShows.Margin = new Padding(4, 3, 4, 3);
            tbMyShows.Name = "tbMyShows";
            tbMyShows.Size = new System.Drawing.Size(994, 682);
            tbMyShows.TabIndex = 9;
            tbMyShows.Text = "TV Shows";
            tbMyShows.UseVisualStyleBackColor = true;
            // 
            // toolStripContainer2
            // 
            // 
            // toolStripContainer2.ContentPanel
            // 
            toolStripContainer2.ContentPanel.Controls.Add(splitContainer1);
            toolStripContainer2.ContentPanel.Margin = new Padding(2);
            toolStripContainer2.ContentPanel.Size = new System.Drawing.Size(994, 643);
            toolStripContainer2.Dock = DockStyle.Fill;
            toolStripContainer2.Location = new System.Drawing.Point(0, 0);
            toolStripContainer2.Margin = new Padding(2);
            toolStripContainer2.Name = "toolStripContainer2";
            toolStripContainer2.Size = new System.Drawing.Size(994, 682);
            toolStripContainer2.TabIndex = 11;
            toolStripContainer2.Text = "toolStripContainer2";
            // 
            // toolStripContainer2.TopToolStripPanel
            // 
            toolStripContainer2.TopToolStripPanel.Controls.Add(tsMyShows);
            // 
            // splitContainer1
            // 
            splitContainer1.BorderStyle = BorderStyle.Fixed3D;
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Margin = new Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(MyShowTree);
            splitContainer1.Panel1.Controls.Add(filterTextBox);
            splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tabControl2);
            splitContainer1.Panel2MinSize = 100;
            splitContainer1.Size = new System.Drawing.Size(994, 643);
            splitContainer1.SplitterDistance = 337;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 8;
            // 
            // MyShowTree
            // 
            MyShowTree.BorderStyle = BorderStyle.None;
            MyShowTree.Dock = DockStyle.Fill;
            MyShowTree.HideSelection = false;
            MyShowTree.Location = new System.Drawing.Point(0, 23);
            MyShowTree.Margin = new Padding(4, 3, 4, 3);
            MyShowTree.Name = "MyShowTree";
            MyShowTree.Size = new System.Drawing.Size(333, 616);
            MyShowTree.TabIndex = 0;
            MyShowTree.AfterSelect += MyShowTree_AfterSelect;
            MyShowTree.MouseClick += MyShowTree_MouseClick;
            // 
            // filterTextBox
            // 
            filterTextBox.Dock = DockStyle.Top;
            filterTextBox.Location = new System.Drawing.Point(0, 0);
            filterTextBox.Margin = new Padding(4, 3, 4, 3);
            filterTextBox.Name = "filterTextBox";
            filterTextBox.Size = new System.Drawing.Size(333, 23);
            filterTextBox.TabIndex = 1;
            filterTextBox.SizeChanged += filterTextBox_SizeChanged;
            filterTextBox.TextChanged += filterTextBox_TextChanged;
            // 
            // tabControl2
            // 
            tabControl2.Appearance = TabAppearance.FlatButtons;
            tabControl2.Controls.Add(tpInformation);
            tabControl2.Controls.Add(tpImages);
            tabControl2.Controls.Add(tpSummary);
            tabControl2.Controls.Add(tabPage3);
            tabControl2.Dock = DockStyle.Fill;
            tabControl2.Location = new System.Drawing.Point(0, 0);
            tabControl2.Margin = new Padding(4, 3, 4, 3);
            tabControl2.Name = "tabControl2";
            tabControl2.SelectedIndex = 0;
            tabControl2.Size = new System.Drawing.Size(648, 639);
            tabControl2.TabIndex = 7;
            // 
            // tpInformation
            // 
            tpInformation.Controls.Add(chrInformation);
            tpInformation.Location = new System.Drawing.Point(4, 27);
            tpInformation.Margin = new Padding(4, 3, 4, 3);
            tpInformation.Name = "tpInformation";
            tpInformation.Padding = new Padding(4, 3, 4, 3);
            tpInformation.Size = new System.Drawing.Size(640, 608);
            tpInformation.TabIndex = 0;
            tpInformation.Text = "Information";
            tpInformation.UseVisualStyleBackColor = true;
            // 
            // chrInformation
            // 
            chrInformation.ActivateBrowserOnCreation = false;
            chrInformation.Dock = DockStyle.Fill;
            chrInformation.Location = new System.Drawing.Point(4, 3);
            chrInformation.Margin = new Padding(4, 3, 4, 3);
            chrInformation.Name = "chrInformation";
            chrInformation.Size = new System.Drawing.Size(632, 602);
            chrInformation.TabIndex = 1;
            chrInformation.Visible = false;
            // 
            // tpImages
            // 
            tpImages.Controls.Add(chrImages);
            tpImages.Location = new System.Drawing.Point(4, 27);
            tpImages.Margin = new Padding(4, 3, 4, 3);
            tpImages.Name = "tpImages";
            tpImages.Padding = new Padding(4, 3, 4, 3);
            tpImages.Size = new System.Drawing.Size(640, 608);
            tpImages.TabIndex = 1;
            tpImages.Text = "Images";
            tpImages.UseVisualStyleBackColor = true;
            // 
            // chrImages
            // 
            chrImages.ActivateBrowserOnCreation = false;
            chrImages.Dock = DockStyle.Fill;
            chrImages.Location = new System.Drawing.Point(4, 3);
            chrImages.Margin = new Padding(4, 3, 4, 3);
            chrImages.Name = "chrImages";
            chrImages.Size = new System.Drawing.Size(632, 602);
            chrImages.TabIndex = 1;
            chrImages.Visible = false;
            // 
            // tpSummary
            // 
            tpSummary.Controls.Add(chrSummary);
            tpSummary.Location = new System.Drawing.Point(4, 27);
            tpSummary.Margin = new Padding(4, 3, 4, 3);
            tpSummary.Name = "tpSummary";
            tpSummary.Padding = new Padding(4, 3, 4, 3);
            tpSummary.Size = new System.Drawing.Size(640, 608);
            tpSummary.TabIndex = 2;
            tpSummary.Text = "Summary";
            tpSummary.UseVisualStyleBackColor = true;
            // 
            // chrSummary
            // 
            chrSummary.ActivateBrowserOnCreation = false;
            chrSummary.Dock = DockStyle.Fill;
            chrSummary.Location = new System.Drawing.Point(4, 3);
            chrSummary.Margin = new Padding(4, 3, 4, 3);
            chrSummary.Name = "chrSummary";
            chrSummary.Size = new System.Drawing.Size(632, 602);
            chrSummary.TabIndex = 2;
            chrSummary.Visible = false;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(chrTvTrailer);
            tabPage3.Location = new System.Drawing.Point(4, 27);
            tabPage3.Margin = new Padding(4, 3, 4, 3);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(4, 3, 4, 3);
            tabPage3.Size = new System.Drawing.Size(640, 608);
            tabPage3.TabIndex = 3;
            tabPage3.Text = "Trailer";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // chrTvTrailer
            // 
            chrTvTrailer.ActivateBrowserOnCreation = false;
            chrTvTrailer.Dock = DockStyle.Fill;
            chrTvTrailer.Location = new System.Drawing.Point(4, 3);
            chrTvTrailer.Margin = new Padding(4, 3, 4, 3);
            chrTvTrailer.Name = "chrTvTrailer";
            chrTvTrailer.Size = new System.Drawing.Size(632, 602);
            chrTvTrailer.TabIndex = 2;
            chrTvTrailer.Visible = false;
            // 
            // tsMyShows
            // 
            tsMyShows.Dock = DockStyle.None;
            tsMyShows.GripStyle = ToolStripGripStyle.Hidden;
            tsMyShows.ImageScalingSize = new System.Drawing.Size(28, 28);
            tsMyShows.Items.AddRange(new ToolStripItem[] { btnAddTVShow, btnEditShow, btnRemoveShow, btnHideHTMLPanel, btnMyShowsCollapse, toolStripSeparator4, btnMyShowsRefresh, tsbScanTV, btnFilterMyShows, toolStripSeparator13, tsbMyShowsContextMenu });
            tsMyShows.Location = new System.Drawing.Point(0, 0);
            tsMyShows.Name = "tsMyShows";
            tsMyShows.Padding = new Padding(0, 0, 4, 0);
            tsMyShows.Size = new System.Drawing.Size(994, 39);
            tsMyShows.Stretch = true;
            tsMyShows.TabIndex = 11;
            tsMyShows.Text = "toolStrip1";
            // 
            // btnAddTVShow
            // 
            btnAddTVShow.Image = (System.Drawing.Image)resources.GetObject("btnAddTVShow.Image");
            btnAddTVShow.ImageScaling = ToolStripItemImageScaling.None;
            btnAddTVShow.Name = "btnAddTVShow";
            btnAddTVShow.Size = new System.Drawing.Size(65, 36);
            btnAddTVShow.Text = "&Add";
            btnAddTVShow.Click += bnMyShowsAdd_Click;
            // 
            // btnEditShow
            // 
            btnEditShow.Image = (System.Drawing.Image)resources.GetObject("btnEditShow.Image");
            btnEditShow.ImageScaling = ToolStripItemImageScaling.None;
            btnEditShow.Name = "btnEditShow";
            btnEditShow.Size = new System.Drawing.Size(63, 36);
            btnEditShow.Text = "&Edit";
            btnEditShow.Click += bnMyShowsEdit_Click;
            // 
            // btnRemoveShow
            // 
            btnRemoveShow.Image = (System.Drawing.Image)resources.GetObject("btnRemoveShow.Image");
            btnRemoveShow.ImageScaling = ToolStripItemImageScaling.None;
            btnRemoveShow.Name = "btnRemoveShow";
            btnRemoveShow.Size = new System.Drawing.Size(76, 36);
            btnRemoveShow.Text = "&Delete";
            btnRemoveShow.Click += bnMyShowsDelete_Click;
            // 
            // btnHideHTMLPanel
            // 
            btnHideHTMLPanel.Alignment = ToolStripItemAlignment.Right;
            btnHideHTMLPanel.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnHideHTMLPanel.Image = (System.Drawing.Image)resources.GetObject("btnHideHTMLPanel.Image");
            btnHideHTMLPanel.ImageScaling = ToolStripItemImageScaling.None;
            btnHideHTMLPanel.Name = "btnHideHTMLPanel";
            btnHideHTMLPanel.Size = new System.Drawing.Size(23, 36);
            btnHideHTMLPanel.Text = "Hide Details";
            btnHideHTMLPanel.Click += ToolStripButton5_Click;
            // 
            // btnMyShowsCollapse
            // 
            btnMyShowsCollapse.Alignment = ToolStripItemAlignment.Right;
            btnMyShowsCollapse.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnMyShowsCollapse.Image = (System.Drawing.Image)resources.GetObject("btnMyShowsCollapse.Image");
            btnMyShowsCollapse.ImageScaling = ToolStripItemImageScaling.None;
            btnMyShowsCollapse.Name = "btnMyShowsCollapse";
            btnMyShowsCollapse.Size = new System.Drawing.Size(23, 36);
            btnMyShowsCollapse.Text = "Collapse";
            btnMyShowsCollapse.Click += BtnMyShowsCollapse_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(6, 39);
            // 
            // btnMyShowsRefresh
            // 
            btnMyShowsRefresh.Image = (System.Drawing.Image)resources.GetObject("btnMyShowsRefresh.Image");
            btnMyShowsRefresh.ImageScaling = ToolStripItemImageScaling.None;
            btnMyShowsRefresh.Name = "btnMyShowsRefresh";
            btnMyShowsRefresh.Size = new System.Drawing.Size(82, 36);
            btnMyShowsRefresh.Text = "&Refresh";
            btnMyShowsRefresh.Click += bnMyShowsRefresh_Click;
            // 
            // tsbScanTV
            // 
            tsbScanTV.Image = (System.Drawing.Image)resources.GetObject("tsbScanTV.Image");
            tsbScanTV.ImageScaling = ToolStripItemImageScaling.None;
            tsbScanTV.Name = "tsbScanTV";
            tsbScanTV.Size = new System.Drawing.Size(84, 36);
            tsbScanTV.Text = "Scan TV";
            tsbScanTV.Click += tsbScanTV_Click;
            // 
            // btnFilterMyShows
            // 
            btnFilterMyShows.Image = (System.Drawing.Image)resources.GetObject("btnFilterMyShows.Image");
            btnFilterMyShows.ImageScaling = ToolStripItemImageScaling.None;
            btnFilterMyShows.Name = "btnFilterMyShows";
            btnFilterMyShows.Size = new System.Drawing.Size(69, 36);
            btnFilterMyShows.Text = "&Filter";
            btnFilterMyShows.Click += btnFilter_Click;
            // 
            // toolStripSeparator13
            // 
            toolStripSeparator13.Name = "toolStripSeparator13";
            toolStripSeparator13.Size = new System.Drawing.Size(6, 39);
            // 
            // tsbMyShowsContextMenu
            // 
            tsbMyShowsContextMenu.Image = (System.Drawing.Image)resources.GetObject("tsbMyShowsContextMenu.Image");
            tsbMyShowsContextMenu.ImageScaling = ToolStripItemImageScaling.None;
            tsbMyShowsContextMenu.Name = "tsbMyShowsContextMenu";
            tsbMyShowsContextMenu.Size = new System.Drawing.Size(119, 36);
            tsbMyShowsContextMenu.Text = "Context Menu";
            tsbMyShowsContextMenu.Click += TsbMyShowsContextMenu_Click;
            // 
            // tbAllInOne
            // 
            tbAllInOne.Controls.Add(olvAction);
            tbAllInOne.Controls.Add(tsScanResults);
            tbAllInOne.ImageKey = "322497-48 (1).png";
            tbAllInOne.Location = new System.Drawing.Point(104, 4);
            tbAllInOne.Margin = new Padding(4, 3, 4, 3);
            tbAllInOne.Name = "tbAllInOne";
            tbAllInOne.Size = new System.Drawing.Size(994, 682);
            tbAllInOne.TabIndex = 11;
            tbAllInOne.Text = "Scan";
            tbAllInOne.UseVisualStyleBackColor = true;
            // 
            // olvAction
            // 
            olvAction.AllColumns.Add(olvShowColumn);
            olvAction.AllColumns.Add(olvSeason);
            olvAction.AllColumns.Add(olvEpisode);
            olvAction.AllColumns.Add(olvDate);
            olvAction.AllColumns.Add(olvFolder);
            olvAction.AllColumns.Add(olvFilename);
            olvAction.AllColumns.Add(olvSource);
            olvAction.AllColumns.Add(olvErrors);
            olvAction.AllColumns.Add(olvType);
            olvAction.AllowColumnReorder = true;
            olvAction.CellEditUseWholeCell = false;
            olvAction.CheckBoxes = true;
            olvAction.CheckedAspectName = "CheckedItem";
            olvAction.Columns.AddRange(new ColumnHeader[] { olvShowColumn, olvSeason, olvEpisode, olvDate, olvFolder, olvFilename, olvSource, olvErrors });
            olvAction.Dock = DockStyle.Fill;
            olvAction.FullRowSelect = true;
            olvAction.GroupWithItemCountFormat = "{0} ({1} items)";
            olvAction.GroupWithItemCountSingularFormat = "{0} (1 Item)";
            olvAction.IncludeColumnHeadersInCopy = true;
            olvAction.IsSimpleDropSink = true;
            olvAction.Location = new System.Drawing.Point(0, 45);
            olvAction.Margin = new Padding(4, 3, 4, 3);
            olvAction.Name = "olvAction";
            olvAction.ShowCommandMenuOnRightClick = true;
            olvAction.ShowGroups = false;
            olvAction.ShowImagesOnSubItems = true;
            olvAction.ShowItemCountOnGroups = true;
            olvAction.ShowItemToolTips = true;
            olvAction.Size = new System.Drawing.Size(994, 637);
            olvAction.SmallImageList = ilIcons;
            olvAction.TabIndex = 0;
            olvAction.UseCompatibleStateImageBehavior = false;
            olvAction.UseFilterIndicator = true;
            olvAction.UseFiltering = true;
            olvAction.UseNotifyPropertyChanged = true;
            olvAction.View = View.Details;
            olvAction.BeforeCreatingGroups += olvAction_BeforeCreatingGroups;
            olvAction.CanDrop += OlvAction_CanDrop;
            olvAction.Dropped += OlvAction_Dropped;
            olvAction.FormatRow += olv1_FormatRow;
            olvAction.ItemChecked += lvAction_ItemChecked;
            olvAction.SelectedIndexChanged += lvAction_SelectedIndexChanged;
            olvAction.KeyDown += lvAction_KeyDown;
            olvAction.MouseClick += lvAction_MouseClick;
            olvAction.MouseDoubleClick += lvAction_MouseDoubleClick;
            // 
            // olvShowColumn
            // 
            olvShowColumn.AspectName = "SeriesName";
            olvShowColumn.GroupWithItemCountFormat = "{0} ({1} items)";
            olvShowColumn.GroupWithItemCountSingularFormat = "{0} (1 Item)";
            olvShowColumn.Hideable = false;
            olvShowColumn.MinimumWidth = 10;
            olvShowColumn.Text = "Show";
            olvShowColumn.Width = 70;
            // 
            // olvSeason
            // 
            olvSeason.AspectName = "SeasonNumber";
            olvSeason.GroupWithItemCountFormat = "{0} ({1} items)";
            olvSeason.GroupWithItemCountSingularFormat = "{0} (1 Item)";
            olvSeason.IsEditable = false;
            olvSeason.MinimumWidth = 10;
            olvSeason.Searchable = false;
            olvSeason.Text = "Season";
            olvSeason.Width = 70;
            // 
            // olvEpisode
            // 
            olvEpisode.AspectName = "EpisodeString";
            olvEpisode.Groupable = false;
            olvEpisode.GroupWithItemCountFormat = "{0} ({1} items)";
            olvEpisode.GroupWithItemCountSingularFormat = "{0} (1 Item)";
            olvEpisode.IsEditable = false;
            olvEpisode.MinimumWidth = 10;
            olvEpisode.Text = "Episode";
            olvEpisode.Width = 70;
            // 
            // olvDate
            // 
            olvDate.AspectName = "AirDateString";
            olvDate.AspectToStringFormat = "";
            olvDate.GroupWithItemCountFormat = "{0} ({1} items)";
            olvDate.GroupWithItemCountSingularFormat = "{0} (1 Item)";
            olvDate.MinimumWidth = 10;
            olvDate.Text = "Date";
            olvDate.Width = 70;
            // 
            // olvFolder
            // 
            olvFolder.AspectName = "DestinationFolder";
            olvFolder.GroupWithItemCountFormat = "{0} ({1} items)";
            olvFolder.GroupWithItemCountSingularFormat = "{0} (1 Item)";
            olvFolder.MinimumWidth = 10;
            olvFolder.Text = "Folder";
            olvFolder.Width = 70;
            // 
            // olvFilename
            // 
            olvFilename.AspectName = "DestinationFile";
            olvFilename.Groupable = false;
            olvFilename.GroupWithItemCountFormat = "{0} ({1} items)";
            olvFilename.GroupWithItemCountSingularFormat = "{0} (1 Item)";
            olvFilename.MinimumWidth = 10;
            olvFilename.Text = "Filename";
            olvFilename.Width = 70;
            // 
            // olvSource
            // 
            olvSource.AspectName = "SourceDetails";
            olvSource.Groupable = false;
            olvSource.GroupWithItemCountFormat = "{0} ({1} items)";
            olvSource.GroupWithItemCountSingularFormat = "{0} (1 Item)";
            olvSource.MinimumWidth = 10;
            olvSource.Text = "Source";
            olvSource.Width = 70;
            // 
            // olvErrors
            // 
            olvErrors.AspectName = "ErrorText";
            olvErrors.Groupable = false;
            olvErrors.GroupWithItemCountFormat = "{0} ({1} items)";
            olvErrors.GroupWithItemCountSingularFormat = "{0} (1 Item)";
            olvErrors.MinimumWidth = 30;
            olvErrors.Text = "Errors";
            olvErrors.Width = 70;
            // 
            // olvType
            // 
            olvType.AspectName = "Name";
            olvType.DisplayIndex = 8;
            olvType.GroupWithItemCountFormat = "{0}";
            olvType.GroupWithItemCountSingularFormat = "{0}";
            olvType.IsVisible = false;
            olvType.Text = "Type";
            // 
            // ilIcons
            // 
            ilIcons.ColorDepth = ColorDepth.Depth24Bit;
            ilIcons.ImageStream = (ImageListStreamer)resources.GetObject("ilIcons.ImageStream");
            ilIcons.TransparentColor = System.Drawing.Color.Transparent;
            ilIcons.Images.SetKeyName(0, "OnDisk.bmp");
            ilIcons.Images.SetKeyName(1, "MagGlass.bmp");
            ilIcons.Images.SetKeyName(2, "uTorrent.bmp");
            ilIcons.Images.SetKeyName(3, "copy.bmp");
            ilIcons.Images.SetKeyName(4, "move.bmp");
            ilIcons.Images.SetKeyName(5, "download.bmp");
            ilIcons.Images.SetKeyName(6, "RSS.bmp");
            ilIcons.Images.SetKeyName(7, "NFO.bmp");
            ilIcons.Images.SetKeyName(8, "sab.png");
            ilIcons.Images.SetKeyName(9, "tk1[1].png");
            ilIcons.Images.SetKeyName(10, "qBitTorrent.png");
            ilIcons.Images.SetKeyName(11, "zip.png");
            // 
            // tsScanResults
            // 
            tsScanResults.GripStyle = ToolStripGripStyle.Hidden;
            tsScanResults.ImageScalingSize = new System.Drawing.Size(14, 14);
            tsScanResults.Items.AddRange(new ToolStripItem[] { btnScan, tbFullScan, tpRecentScan, tbQuickScan, toolStripSeparator11, btnActionBTSearch, tbActionJackettSearch, toolStripSeparator9, btnIgnoreSelectedActions, btnRemoveSelActions, toolStripDropDownButton1, toolStripSeparator10, btnActionAction, btnRevertView, btnPreferences, toolStripSeparator14, tsbScanContextMenu });
            tsScanResults.Location = new System.Drawing.Point(0, 0);
            tsScanResults.Name = "tsScanResults";
            tsScanResults.Padding = new Padding(0, 0, 2, 0);
            tsScanResults.Size = new System.Drawing.Size(994, 45);
            tsScanResults.TabIndex = 13;
            tsScanResults.Text = "toolStrip1";
            // 
            // btnScan
            // 
            btnScan.DropDownItems.AddRange(new ToolStripItem[] { fullToolStripMenuItem, recentToolStripMenuItem, quickToolStripMenuItem });
            btnScan.Image = (System.Drawing.Image)resources.GetObject("btnScan.Image");
            btnScan.ImageScaling = ToolStripItemImageScaling.None;
            btnScan.Name = "btnScan";
            btnScan.Size = new System.Drawing.Size(80, 42);
            btnScan.Text = "Scan";
            btnScan.ButtonClick += BtnSearch_ButtonClick;
            // 
            // fullToolStripMenuItem
            // 
            fullToolStripMenuItem.Name = "fullToolStripMenuItem";
            fullToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            fullToolStripMenuItem.Text = "&Full";
            fullToolStripMenuItem.ToolTipText = "Scan all Movies and TV Shows";
            fullToolStripMenuItem.Click += FullToolStripMenuItem_Click;
            // 
            // recentToolStripMenuItem
            // 
            recentToolStripMenuItem.Name = "recentToolStripMenuItem";
            recentToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            recentToolStripMenuItem.Text = "Recent";
            recentToolStripMenuItem.ToolTipText = "Scan TV Shows with recent aired episodes";
            recentToolStripMenuItem.Click += RecentToolStripMenuItem_Click;
            // 
            // quickToolStripMenuItem
            // 
            quickToolStripMenuItem.Name = "quickToolStripMenuItem";
            quickToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            quickToolStripMenuItem.Text = "Quick";
            quickToolStripMenuItem.ToolTipText = "Scan shows with missing recent aired episodes and and shows that match files in the search folders";
            quickToolStripMenuItem.Click += QuickToolStripMenuItem_Click;
            // 
            // tbFullScan
            // 
            tbFullScan.AccessibleDescription = "Full Scan Button - Starts a full scan";
            tbFullScan.AccessibleName = "Full Scan";
            tbFullScan.AccessibleRole = AccessibleRole.ButtonMenu;
            tbFullScan.Image = (System.Drawing.Image)resources.GetObject("tbFullScan.Image");
            tbFullScan.ImageScaling = ToolStripItemImageScaling.None;
            tbFullScan.Name = "tbFullScan";
            tbFullScan.Padding = new Padding(3);
            tbFullScan.Size = new System.Drawing.Size(96, 42);
            tbFullScan.Text = "&Full Scan";
            tbFullScan.Click += TbFullScan_Click;
            // 
            // tpRecentScan
            // 
            tpRecentScan.Image = (System.Drawing.Image)resources.GetObject("tpRecentScan.Image");
            tpRecentScan.ImageScaling = ToolStripItemImageScaling.None;
            tpRecentScan.Name = "tpRecentScan";
            tpRecentScan.Size = new System.Drawing.Size(107, 42);
            tpRecentScan.Text = "&Recent Scan";
            tpRecentScan.Click += TpRecentScan_Click;
            // 
            // tbQuickScan
            // 
            tbQuickScan.Image = (System.Drawing.Image)resources.GetObject("tbQuickScan.Image");
            tbQuickScan.ImageScaling = ToolStripItemImageScaling.None;
            tbQuickScan.Name = "tbQuickScan";
            tbQuickScan.Size = new System.Drawing.Size(102, 42);
            tbQuickScan.Text = "&Quick Scan";
            tbQuickScan.Click += TbQuickScan_Click;
            // 
            // toolStripSeparator11
            // 
            toolStripSeparator11.Name = "toolStripSeparator11";
            toolStripSeparator11.Size = new System.Drawing.Size(6, 45);
            // 
            // btnActionBTSearch
            // 
            btnActionBTSearch.Image = (System.Drawing.Image)resources.GetObject("btnActionBTSearch.Image");
            btnActionBTSearch.ImageScaling = ToolStripItemImageScaling.None;
            btnActionBTSearch.Name = "btnActionBTSearch";
            btnActionBTSearch.Size = new System.Drawing.Size(105, 42);
            btnActionBTSearch.Text = "BT Search";
            btnActionBTSearch.ButtonClick += bnActionBTSearch_Click;
            btnActionBTSearch.DropDownOpening += BTSearch_DropDownOpening;
            btnActionBTSearch.DropDownItemClicked += menuSearchSites_ItemClicked;
            // 
            // tbActionJackettSearch
            // 
            tbActionJackettSearch.Image = (System.Drawing.Image)resources.GetObject("tbActionJackettSearch.Image");
            tbActionJackettSearch.ImageScaling = ToolStripItemImageScaling.None;
            tbActionJackettSearch.Name = "tbActionJackettSearch";
            tbActionJackettSearch.Size = new System.Drawing.Size(117, 42);
            tbActionJackettSearch.Text = "Jackett Search";
            tbActionJackettSearch.Click += tbJackettSearch_Click;
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new System.Drawing.Size(6, 45);
            // 
            // btnIgnoreSelectedActions
            // 
            btnIgnoreSelectedActions.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnIgnoreSelectedActions.Image = (System.Drawing.Image)resources.GetObject("btnIgnoreSelectedActions.Image");
            btnIgnoreSelectedActions.ImageTransparentColor = System.Drawing.Color.Magenta;
            btnIgnoreSelectedActions.Name = "btnIgnoreSelectedActions";
            btnIgnoreSelectedActions.Size = new System.Drawing.Size(63, 42);
            btnIgnoreSelectedActions.Text = "Ignore Sel";
            btnIgnoreSelectedActions.Click += cbActionIgnore_Click;
            // 
            // btnRemoveSelActions
            // 
            btnRemoveSelActions.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnRemoveSelActions.Image = (System.Drawing.Image)resources.GetObject("btnRemoveSelActions.Image");
            btnRemoveSelActions.ImageTransparentColor = System.Drawing.Color.Magenta;
            btnRemoveSelActions.Name = "btnRemoveSelActions";
            btnRemoveSelActions.Size = new System.Drawing.Size(72, 42);
            btnRemoveSelActions.Text = "Remove Sel";
            btnRemoveSelActions.Click += bnRemoveSel_Click;
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.DropDownItems.AddRange(new ToolStripItem[] { mcbAll, mcbRename, mcbCopyMove, mcbDeleteFiles, mcbSaveImages, mcbDownload, mcbWriteMetadata, mcbModifyMetadata });
            toolStripDropDownButton1.Image = (System.Drawing.Image)resources.GetObject("toolStripDropDownButton1.Image");
            toolStripDropDownButton1.ImageScaling = ToolStripItemImageScaling.None;
            toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new System.Drawing.Size(85, 42);
            toolStripDropDownButton1.Text = "Check";
            // 
            // mcbAll
            // 
            mcbAll.Checked = true;
            mcbAll.CheckState = CheckState.Indeterminate;
            mcbAll.Name = "mcbAll";
            mcbAll.Size = new System.Drawing.Size(219, 22);
            mcbAll.Text = "Show All";
            mcbAll.Click += McbAll_Click;
            // 
            // mcbRename
            // 
            mcbRename.Checked = true;
            mcbRename.CheckState = CheckState.Indeterminate;
            mcbRename.Name = "mcbRename";
            mcbRename.Size = new System.Drawing.Size(219, 22);
            mcbRename.Text = "Rename Files";
            mcbRename.Click += McbRename_Click;
            // 
            // mcbCopyMove
            // 
            mcbCopyMove.Checked = true;
            mcbCopyMove.CheckState = CheckState.Indeterminate;
            mcbCopyMove.DisplayStyle = ToolStripItemDisplayStyle.Text;
            mcbCopyMove.Name = "mcbCopyMove";
            mcbCopyMove.Size = new System.Drawing.Size(219, 22);
            mcbCopyMove.Text = "Copy / Move";
            mcbCopyMove.Click += McbCopyMove_Click;
            // 
            // mcbDeleteFiles
            // 
            mcbDeleteFiles.Checked = true;
            mcbDeleteFiles.CheckState = CheckState.Indeterminate;
            mcbDeleteFiles.DisplayStyle = ToolStripItemDisplayStyle.Text;
            mcbDeleteFiles.Name = "mcbDeleteFiles";
            mcbDeleteFiles.Size = new System.Drawing.Size(219, 22);
            mcbDeleteFiles.Text = "Delete Files";
            mcbDeleteFiles.Click += McbDeleteFiles_Click;
            // 
            // mcbSaveImages
            // 
            mcbSaveImages.Checked = true;
            mcbSaveImages.CheckState = CheckState.Indeterminate;
            mcbSaveImages.DisplayStyle = ToolStripItemDisplayStyle.Text;
            mcbSaveImages.Name = "mcbSaveImages";
            mcbSaveImages.Size = new System.Drawing.Size(219, 22);
            mcbSaveImages.Text = "Save Images";
            mcbSaveImages.Click += McbSaveImages_Click;
            // 
            // mcbDownload
            // 
            mcbDownload.Checked = true;
            mcbDownload.CheckState = CheckState.Indeterminate;
            mcbDownload.DisplayStyle = ToolStripItemDisplayStyle.Text;
            mcbDownload.Name = "mcbDownload";
            mcbDownload.Size = new System.Drawing.Size(219, 22);
            mcbDownload.Text = "Download";
            mcbDownload.Click += McbDownload_Click;
            // 
            // mcbWriteMetadata
            // 
            mcbWriteMetadata.Checked = true;
            mcbWriteMetadata.CheckState = CheckState.Indeterminate;
            mcbWriteMetadata.DisplayStyle = ToolStripItemDisplayStyle.Text;
            mcbWriteMetadata.Name = "mcbWriteMetadata";
            mcbWriteMetadata.Size = new System.Drawing.Size(219, 22);
            mcbWriteMetadata.Text = "Write Metadata Files";
            mcbWriteMetadata.Click += McbWriteMetadata_Click;
            // 
            // mcbModifyMetadata
            // 
            mcbModifyMetadata.Checked = true;
            mcbModifyMetadata.CheckState = CheckState.Indeterminate;
            mcbModifyMetadata.DisplayStyle = ToolStripItemDisplayStyle.Text;
            mcbModifyMetadata.Name = "mcbModifyMetadata";
            mcbModifyMetadata.Size = new System.Drawing.Size(219, 22);
            mcbModifyMetadata.Text = "Update Video File Metadata";
            mcbModifyMetadata.Click += McbModifyMetadata_Click;
            // 
            // toolStripSeparator10
            // 
            toolStripSeparator10.Name = "toolStripSeparator10";
            toolStripSeparator10.Size = new System.Drawing.Size(6, 45);
            // 
            // btnActionAction
            // 
            btnActionAction.Image = (System.Drawing.Image)resources.GetObject("btnActionAction.Image");
            btnActionAction.ImageScaling = ToolStripItemImageScaling.None;
            btnActionAction.Name = "btnActionAction";
            btnActionAction.Size = new System.Drawing.Size(107, 42);
            btnActionAction.Text = "&Do Checked";
            btnActionAction.Click += bnActionAction_Click;
            // 
            // btnRevertView
            // 
            btnRevertView.Alignment = ToolStripItemAlignment.Right;
            btnRevertView.Image = (System.Drawing.Image)resources.GetObject("btnRevertView.Image");
            btnRevertView.ImageScaling = ToolStripItemImageScaling.None;
            btnRevertView.Name = "btnRevertView";
            btnRevertView.Size = new System.Drawing.Size(88, 20);
            btnRevertView.Text = "Revert View";
            btnRevertView.Click += BtnRevertView_Click;
            // 
            // btnPreferences
            // 
            btnPreferences.Alignment = ToolStripItemAlignment.Right;
            btnPreferences.Image = (System.Drawing.Image)resources.GetObject("btnPreferences.Image");
            btnPreferences.ImageScaling = ToolStripItemImageScaling.None;
            btnPreferences.Name = "btnPreferences";
            btnPreferences.Size = new System.Drawing.Size(113, 36);
            btnPreferences.Text = "&Preferences...";
            btnPreferences.Click += bnActionOptions_Click;
            // 
            // toolStripSeparator14
            // 
            toolStripSeparator14.Name = "toolStripSeparator14";
            toolStripSeparator14.Size = new System.Drawing.Size(6, 45);
            // 
            // tsbScanContextMenu
            // 
            tsbScanContextMenu.Image = (System.Drawing.Image)resources.GetObject("tsbScanContextMenu.Image");
            tsbScanContextMenu.ImageScaling = ToolStripItemImageScaling.None;
            tsbScanContextMenu.Name = "tsbScanContextMenu";
            tsbScanContextMenu.Size = new System.Drawing.Size(119, 36);
            tsbScanContextMenu.Text = "Context Menu";
            tsbScanContextMenu.Click += TsbScanContextMenu_Click;
            // 
            // tbWTW
            // 
            tbWTW.Controls.Add(tableLayoutPanel4);
            tbWTW.ImageKey = "115762-48.png";
            tbWTW.Location = new System.Drawing.Point(104, 4);
            tbWTW.Margin = new Padding(4, 3, 4, 3);
            tbWTW.Name = "tbWTW";
            tbWTW.Size = new System.Drawing.Size(994, 682);
            tbWTW.TabIndex = 4;
            tbWTW.Text = "Schedule";
            tbWTW.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 1;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.Controls.Add(tableLayoutPanel1, 0, 1);
            tableLayoutPanel4.Controls.Add(toolStripContainer3, 0, 0);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel4.Margin = new Padding(4, 3, 4, 3);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 2;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 162F));
            tableLayoutPanel4.Size = new System.Drawing.Size(994, 682);
            tableLayoutPanel4.TabIndex = 13;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 227F));
            tableLayoutPanel1.Controls.Add(calCalendar, 1, 0);
            tableLayoutPanel1.Controls.Add(txtWhenToWatchSynopsis, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanel1.Location = new System.Drawing.Point(2, 522);
            tableLayoutPanel1.Margin = new Padding(2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new System.Drawing.Size(990, 158);
            tableLayoutPanel1.TabIndex = 4;
            // 
            // calCalendar
            // 
            calCalendar.Dock = DockStyle.Fill;
            calCalendar.Location = new System.Drawing.Point(763, 0);
            calCalendar.Margin = new Padding(0);
            calCalendar.MaxSelectionCount = 1;
            calCalendar.Name = "calCalendar";
            calCalendar.TabIndex = 5;
            calCalendar.DateSelected += calCalendar_DateSelected;
            // 
            // txtWhenToWatchSynopsis
            // 
            txtWhenToWatchSynopsis.Dock = DockStyle.Fill;
            txtWhenToWatchSynopsis.Location = new System.Drawing.Point(0, 0);
            txtWhenToWatchSynopsis.Margin = new Padding(0);
            txtWhenToWatchSynopsis.Multiline = true;
            txtWhenToWatchSynopsis.Name = "txtWhenToWatchSynopsis";
            txtWhenToWatchSynopsis.ReadOnly = true;
            txtWhenToWatchSynopsis.ScrollBars = ScrollBars.Vertical;
            txtWhenToWatchSynopsis.Size = new System.Drawing.Size(763, 158);
            txtWhenToWatchSynopsis.TabIndex = 4;
            // 
            // toolStripContainer3
            // 
            // 
            // toolStripContainer3.ContentPanel
            // 
            toolStripContainer3.ContentPanel.Controls.Add(lvWhenToWatch);
            toolStripContainer3.ContentPanel.Margin = new Padding(2);
            toolStripContainer3.ContentPanel.Size = new System.Drawing.Size(990, 477);
            toolStripContainer3.Dock = DockStyle.Fill;
            toolStripContainer3.Location = new System.Drawing.Point(2, 2);
            toolStripContainer3.Margin = new Padding(2);
            toolStripContainer3.Name = "toolStripContainer3";
            toolStripContainer3.Size = new System.Drawing.Size(990, 516);
            toolStripContainer3.TabIndex = 11;
            toolStripContainer3.Text = "toolStripContainer3";
            // 
            // toolStripContainer3.TopToolStripPanel
            // 
            toolStripContainer3.TopToolStripPanel.Controls.Add(tsWtW);
            // 
            // lvWhenToWatch
            // 
            lvWhenToWatch.Columns.AddRange(new ColumnHeader[] { columnHeader29, columnHeader30, columnHeader31, columnHeader32, columnHeader36, columnHeader33, columnHeader34, columnHeader1, columnHeader35 });
            lvWhenToWatch.Dock = DockStyle.Fill;
            lvWhenToWatch.FullRowSelect = true;
            listViewGroup1.Header = "Recently Aired";
            listViewGroup1.Name = "justPassed";
            listViewGroup2.Header = "Next 7 Days";
            listViewGroup2.Name = "next7days";
            listViewGroup2.Tag = "1";
            listViewGroup3.Header = "Returning Series";
            listViewGroup3.Name = "futureEps";
            listViewGroup4.Header = "Other Planned Episodes";
            listViewGroup4.Name = "later";
            listViewGroup4.Tag = "2";
            lvWhenToWatch.Groups.AddRange(new ListViewGroup[] { listViewGroup1, listViewGroup2, listViewGroup3, listViewGroup4 });
            lvWhenToWatch.Location = new System.Drawing.Point(0, 0);
            lvWhenToWatch.Margin = new Padding(4, 3, 4, 3);
            lvWhenToWatch.Name = "lvWhenToWatch";
            lvWhenToWatch.ShowItemToolTips = true;
            lvWhenToWatch.Size = new System.Drawing.Size(990, 477);
            lvWhenToWatch.SmallImageList = ilIcons;
            lvWhenToWatch.TabIndex = 3;
            lvWhenToWatch.UseCompatibleStateImageBehavior = false;
            lvWhenToWatch.View = View.Details;
            lvWhenToWatch.ColumnClick += lvWhenToWatch_ColumnClick;
            lvWhenToWatch.SelectedIndexChanged += lvWhenToWatch_Click;
            lvWhenToWatch.DoubleClick += lvWhenToWatch_DoubleClick;
            lvWhenToWatch.MouseClick += lvWhenToWatch_MouseClick;
            // 
            // columnHeader29
            // 
            columnHeader29.Text = "Show";
            columnHeader29.Width = 218;
            // 
            // columnHeader30
            // 
            columnHeader30.Text = "Season";
            columnHeader30.Width = 59;
            // 
            // columnHeader31
            // 
            columnHeader31.Text = "Episode";
            columnHeader31.Width = 64;
            // 
            // columnHeader32
            // 
            columnHeader32.Text = "Air Date";
            columnHeader32.Width = 94;
            // 
            // columnHeader36
            // 
            columnHeader36.Text = "Time";
            columnHeader36.Width = 70;
            // 
            // columnHeader33
            // 
            columnHeader33.Text = "Day";
            columnHeader33.Width = 49;
            // 
            // columnHeader34
            // 
            columnHeader34.Text = "How Long";
            columnHeader34.Width = 80;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Network";
            columnHeader1.Width = 117;
            // 
            // columnHeader35
            // 
            columnHeader35.Text = "Episode Name";
            columnHeader35.Width = 420;
            // 
            // tsWtW
            // 
            tsWtW.Dock = DockStyle.None;
            tsWtW.GripStyle = ToolStripGripStyle.Hidden;
            tsWtW.ImageScalingSize = new System.Drawing.Size(28, 28);
            tsWtW.Items.AddRange(new ToolStripItem[] { btnWhenToWatchCheck, btnScheduleBTSearch, tsbScheduleJackettSearch, toolStripSeparator12, btnScheduleRightClick });
            tsWtW.Location = new System.Drawing.Point(0, 0);
            tsWtW.Name = "tsWtW";
            tsWtW.Padding = new Padding(0, 0, 4, 0);
            tsWtW.Size = new System.Drawing.Size(990, 39);
            tsWtW.Stretch = true;
            tsWtW.TabIndex = 6;
            tsWtW.Text = "toolStrip1";
            // 
            // btnWhenToWatchCheck
            // 
            btnWhenToWatchCheck.Image = (System.Drawing.Image)resources.GetObject("btnWhenToWatchCheck.Image");
            btnWhenToWatchCheck.ImageScaling = ToolStripItemImageScaling.None;
            btnWhenToWatchCheck.Name = "btnWhenToWatchCheck";
            btnWhenToWatchCheck.Size = new System.Drawing.Size(82, 36);
            btnWhenToWatchCheck.Text = "&Refresh";
            btnWhenToWatchCheck.Click += bnWhenToWatchCheck_Click;
            // 
            // btnScheduleBTSearch
            // 
            btnScheduleBTSearch.DropDownButtonWidth = 20;
            btnScheduleBTSearch.Image = (System.Drawing.Image)resources.GetObject("btnScheduleBTSearch.Image");
            btnScheduleBTSearch.ImageScaling = ToolStripItemImageScaling.None;
            btnScheduleBTSearch.Name = "btnScheduleBTSearch";
            btnScheduleBTSearch.Size = new System.Drawing.Size(114, 36);
            btnScheduleBTSearch.Text = "BT Search";
            btnScheduleBTSearch.ButtonClick += bnWTWBTSearch_Click;
            btnScheduleBTSearch.DropDownOpening += BTSearch_DropDownOpening;
            btnScheduleBTSearch.DropDownItemClicked += menuSearchSites_ItemClicked;
            // 
            // tsbScheduleJackettSearch
            // 
            tsbScheduleJackettSearch.Image = (System.Drawing.Image)resources.GetObject("tsbScheduleJackettSearch.Image");
            tsbScheduleJackettSearch.ImageScaling = ToolStripItemImageScaling.None;
            tsbScheduleJackettSearch.Name = "tsbScheduleJackettSearch";
            tsbScheduleJackettSearch.Size = new System.Drawing.Size(117, 36);
            tsbScheduleJackettSearch.Text = "Jackett Search";
            tsbScheduleJackettSearch.Click += tsbScheduleJackettSearch_Click;
            // 
            // toolStripSeparator12
            // 
            toolStripSeparator12.Name = "toolStripSeparator12";
            toolStripSeparator12.Size = new System.Drawing.Size(6, 39);
            // 
            // btnScheduleRightClick
            // 
            btnScheduleRightClick.Image = (System.Drawing.Image)resources.GetObject("btnScheduleRightClick.Image");
            btnScheduleRightClick.ImageScaling = ToolStripItemImageScaling.None;
            btnScheduleRightClick.Name = "btnScheduleRightClick";
            btnScheduleRightClick.Size = new System.Drawing.Size(119, 36);
            btnScheduleRightClick.Text = "Context Menu";
            btnScheduleRightClick.Click += ToolStripButton1_Click;
            // 
            // ilNewIcons
            // 
            ilNewIcons.ColorDepth = ColorDepth.Depth32Bit;
            ilNewIcons.ImageStream = (ImageListStreamer)resources.GetObject("ilNewIcons.ImageStream");
            ilNewIcons.TransparentColor = System.Drawing.Color.Transparent;
            ilNewIcons.Images.SetKeyName(0, "322497-48 (1).png");
            ilNewIcons.Images.SetKeyName(1, "353430-48.png");
            ilNewIcons.Images.SetKeyName(2, "1587498-48.png");
            ilNewIcons.Images.SetKeyName(3, "115762-48.png");
            ilNewIcons.Images.SetKeyName(4, "3790574-48.png");
            ilNewIcons.Images.SetKeyName(5, "4632196-48.png");
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth8Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = System.Drawing.Color.Magenta;
            imageList1.Images.SetKeyName(0, "clock.bmp");
            imageList1.Images.SetKeyName(1, "Calendar_schedule.bmp");
            imageList1.Images.SetKeyName(2, "Save.bmp");
            imageList1.Images.SetKeyName(3, "Refresh.bmp");
            imageList1.Images.SetKeyName(4, "Control_TreeView.bmp");
            imageList1.Images.SetKeyName(5, "Zoom.bmp");
            imageList1.Images.SetKeyName(6, "delete.bmp");
            imageList1.Images.SetKeyName(7, "EditInformation.bmp");
            imageList1.Images.SetKeyName(8, "FormRun.bmp");
            imageList1.Images.SetKeyName(9, "GetLatestVersion.bmp");
            imageList1.Images.SetKeyName(10, "OpenFolder.bmp");
            imageList1.Images.SetKeyName(11, "SearchWeb.bmp");
            imageList1.Images.SetKeyName(12, "PublishToWeb.bmp");
            imageList1.Images.SetKeyName(13, "Options.bmp");
            imageList1.Images.SetKeyName(14, "NewCard.bmp");
            imageList1.Images.SetKeyName(15, "TVOff.bmp");
            imageList1.Images.SetKeyName(16, "FillLeft.bmp");
            imageList1.Images.SetKeyName(17, "FillRight.bmp");
            imageList1.Images.SetKeyName(18, "filtre.png");
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            tableLayoutPanel2.Controls.Add(pbProgressBarx, 2, 0);
            tableLayoutPanel2.Controls.Add(txtDLStatusLabel, 1, 0);
            tableLayoutPanel2.Controls.Add(tsNextShowTxt, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Bottom;
            tableLayoutPanel2.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanel2.Location = new System.Drawing.Point(0, 714);
            tableLayoutPanel2.Margin = new Padding(4, 3, 4, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new System.Drawing.Size(1102, 29);
            tableLayoutPanel2.TabIndex = 9;
            // 
            // pbProgressBarx
            // 
            pbProgressBarx.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pbProgressBarx.Location = new System.Drawing.Point(939, 12);
            pbProgressBarx.Margin = new Padding(4, 3, 4, 3);
            pbProgressBarx.Name = "pbProgressBarx";
            pbProgressBarx.Size = new System.Drawing.Size(159, 14);
            pbProgressBarx.Step = 1;
            pbProgressBarx.Style = ProgressBarStyle.Continuous;
            pbProgressBarx.TabIndex = 0;
            // 
            // txtDLStatusLabel
            // 
            txtDLStatusLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtDLStatusLabel.Location = new System.Drawing.Point(499, 14);
            txtDLStatusLabel.Margin = new Padding(4, 0, 4, 0);
            txtDLStatusLabel.Name = "txtDLStatusLabel";
            txtDLStatusLabel.Size = new System.Drawing.Size(432, 15);
            txtDLStatusLabel.TabIndex = 1;
            txtDLStatusLabel.Text = "Background Download: ---";
            txtDLStatusLabel.Visible = false;
            // 
            // tsNextShowTxt
            // 
            tsNextShowTxt.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tsNextShowTxt.Location = new System.Drawing.Point(4, 14);
            tsNextShowTxt.Margin = new Padding(4, 0, 4, 0);
            tsNextShowTxt.Name = "tsNextShowTxt";
            tsNextShowTxt.Size = new System.Drawing.Size(487, 15);
            tsNextShowTxt.TabIndex = 1;
            tsNextShowTxt.Text = "---";
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "Show";
            columnHeader5.Width = 211;
            // 
            // columnHeader6
            // 
            columnHeader6.Text = "Season";
            // 
            // columnHeader7
            // 
            columnHeader7.Text = "thetvdb code";
            columnHeader7.Width = 82;
            // 
            // columnHeader8
            // 
            columnHeader8.Text = "Show next airdate";
            columnHeader8.Width = 115;
            // 
            // columnHeader25
            // 
            columnHeader25.Text = "From Folder";
            columnHeader25.Width = 187;
            // 
            // columnHeader26
            // 
            columnHeader26.Text = "From Name";
            columnHeader26.Width = 163;
            // 
            // columnHeader27
            // 
            columnHeader27.Text = "To Folder";
            columnHeader27.Width = 172;
            // 
            // columnHeader28
            // 
            columnHeader28.Text = "To Name";
            columnHeader28.Width = 165;
            // 
            // openFile
            // 
            openFile.Filter = "Torrent files (*.torrent)|*.torrent|All files (*.*)|*.*";
            // 
            // folderBrowser
            // 
            folderBrowser.ShowNewFolderButton = false;
            // 
            // refreshWTWTimer
            // 
            refreshWTWTimer.Enabled = true;
            refreshWTWTimer.Interval = 600000;
            refreshWTWTimer.Tick += refreshWTWTimer_Tick;
            // 
            // notifyIcon1
            // 
            notifyIcon1.BalloonTipText = "TV Rename is t3h r0x0r";
            notifyIcon1.BalloonTipTitle = "TV Rename";
            notifyIcon1.Icon = (System.Drawing.Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "TV Rename";
            notifyIcon1.MouseClick += notifyIcon1_Click;
            notifyIcon1.MouseDoubleClick += notifyIcon1_DoubleClick;
            // 
            // showRightClickMenu
            // 
            showRightClickMenu.ImageScalingSize = new System.Drawing.Size(28, 28);
            showRightClickMenu.Name = "menuSearchSites";
            showRightClickMenu.ShowImageMargin = false;
            showRightClickMenu.Size = new System.Drawing.Size(36, 4);
            showRightClickMenu.ItemClicked += showRightClickMenu_ItemClicked;
            // 
            // statusTimer
            // 
            statusTimer.Enabled = true;
            statusTimer.Interval = 250;
            statusTimer.Tick += statusTimer_Tick;
            // 
            // BGDownloadTimer
            // 
            BGDownloadTimer.Enabled = true;
            BGDownloadTimer.Interval = 10000;
            BGDownloadTimer.Tick += BGDownloadTimer_Tick;
            // 
            // UpdateTimer
            // 
            UpdateTimer.Enabled = true;
            UpdateTimer.Interval = 1000;
            UpdateTimer.Tick += UpdateTimer_Tick;
            // 
            // tmrShowUpcomingPopup
            // 
            tmrShowUpcomingPopup.Interval = 250;
            tmrShowUpcomingPopup.Tick += tmrShowUpcomingPopup_Tick;
            // 
            // quickTimer
            // 
            quickTimer.Interval = 1;
            quickTimer.Tick += quickTimer_Tick;
            // 
            // tmrPeriodicScan
            // 
            tmrPeriodicScan.Enabled = true;
            tmrPeriodicScan.Tick += tmrPeriodicScan_Tick;
            // 
            // btnUpdateAvailable
            // 
            btnUpdateAvailable.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUpdateAvailable.BackColor = System.Drawing.SystemColors.ActiveCaption;
            btnUpdateAvailable.Location = new System.Drawing.Point(958, 0);
            btnUpdateAvailable.Margin = new Padding(4, 3, 4, 3);
            btnUpdateAvailable.Name = "btnUpdateAvailable";
            btnUpdateAvailable.Size = new System.Drawing.Size(135, 27);
            btnUpdateAvailable.TabIndex = 10;
            btnUpdateAvailable.Text = "Update Available...";
            btnUpdateAvailable.UseVisualStyleBackColor = false;
            btnUpdateAvailable.Visible = false;
            btnUpdateAvailable.Click += btnUpdateAvailable_Click;
            // 
            // bwSeasonHTMLGenerator
            // 
            bwSeasonHTMLGenerator.WorkerSupportsCancellation = true;
            bwSeasonHTMLGenerator.DoWork += BwSeasonHTMLGenerator_DoWork;
            bwSeasonHTMLGenerator.RunWorkerCompleted += UpdateWeb;
            // 
            // bwUpdateSchedule
            // 
            bwUpdateSchedule.WorkerSupportsCancellation = true;
            bwUpdateSchedule.DoWork += BwUpdateSchedule_DoWork;
            bwUpdateSchedule.RunWorkerCompleted += BwUpdateSchedule_RunWorkerCompleted;
            // 
            // bwShowHTMLGenerator
            // 
            bwShowHTMLGenerator.WorkerSupportsCancellation = true;
            bwShowHTMLGenerator.DoWork += BwShowHTMLGenerator_DoWork;
            bwShowHTMLGenerator.RunWorkerCompleted += UpdateWeb;
            // 
            // bwShowSummaryHTMLGenerator
            // 
            bwShowSummaryHTMLGenerator.WorkerSupportsCancellation = true;
            bwShowSummaryHTMLGenerator.DoWork += BwShowSummaryHTMLGenerator_DoWork;
            bwShowSummaryHTMLGenerator.RunWorkerCompleted += UpdateWeb;
            // 
            // bwSeasonSummaryHTMLGenerator
            // 
            bwSeasonSummaryHTMLGenerator.WorkerSupportsCancellation = true;
            bwSeasonSummaryHTMLGenerator.DoWork += BwSeasonSummaryHTMLGenerator_DoWork;
            bwSeasonSummaryHTMLGenerator.RunWorkerCompleted += UpdateWeb;
            // 
            // bwMovieHTMLGenerator
            // 
            bwMovieHTMLGenerator.WorkerSupportsCancellation = true;
            bwMovieHTMLGenerator.DoWork += bwMovieHTMLGenerator_DoWork;
            bwMovieHTMLGenerator.RunWorkerCompleted += UpdateWeb;
            // 
            // bwScan
            // 
            bwScan.WorkerSupportsCancellation = true;
            bwScan.DoWork += bwScan_DoWork;
            bwScan.ProgressChanged += bwScan_ProgressChanged;
            bwScan.RunWorkerCompleted += bwScan_RunWorkerCompleted;
            // 
            // bwAction
            // 
            bwAction.WorkerSupportsCancellation = true;
            bwAction.DoWork += bwAction_DoWork;
            bwAction.RunWorkerCompleted += bwAction_RunWorkerCompleted;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Location = new System.Drawing.Point(52, 351);
            tableLayoutPanel3.Margin = new Padding(4, 3, 4, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Size = new System.Drawing.Size(233, 115);
            tableLayoutPanel3.TabIndex = 11;
            // 
            // panel1
            // 
            panel1.Controls.Add(tabControl1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 24);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(1102, 690);
            panel1.TabIndex = 12;
            // 
            // removeShowsWithNoFoldersToolStripMenuItem
            // 
            removeShowsWithNoFoldersToolStripMenuItem.Name = "removeShowsWithNoFoldersToolStripMenuItem";
            removeShowsWithNoFoldersToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            removeShowsWithNoFoldersToolStripMenuItem.Text = "Remove Shows With No Folders...";
            removeShowsWithNoFoldersToolStripMenuItem.Click += removeShowsWithNoFoldersToolStripMenuItem_Click;
            // 
            // UI
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1102, 743);
            Controls.Add(panel1);
            Controls.Add(tableLayoutPanel3);
            Controls.Add(btnUpdateAvailable);
            Controls.Add(tableLayoutPanel2);
            Controls.Add(menuStrip1);
            DoubleBuffered = true;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4, 3, 4, 3);
            Name = "UI";
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.Manual;
            Text = "TV Rename";
            FormClosing += UI_FormClosing;
            Load += UI_Load;
            LocationChanged += UI_LocationChanged;
            SizeChanged += UI_SizeChanged;
            KeyDown += UI_KeyDown;
            Resize += UI_Resize;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            tabControl1.ResumeLayout(false);
            tbMyMovies.ResumeLayout(false);
            toolStripContainer1.ContentPanel.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.PerformLayout();
            toolStripContainer1.ResumeLayout(false);
            toolStripContainer1.PerformLayout();
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel1.PerformLayout();
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            tabControl3.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tpMovieTrailer.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            tbMyShows.ResumeLayout(false);
            toolStripContainer2.ContentPanel.ResumeLayout(false);
            toolStripContainer2.TopToolStripPanel.ResumeLayout(false);
            toolStripContainer2.TopToolStripPanel.PerformLayout();
            toolStripContainer2.ResumeLayout(false);
            toolStripContainer2.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tabControl2.ResumeLayout(false);
            tpInformation.ResumeLayout(false);
            tpImages.ResumeLayout(false);
            tpSummary.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            tsMyShows.ResumeLayout(false);
            tsMyShows.PerformLayout();
            tbAllInOne.ResumeLayout(false);
            tbAllInOne.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)olvAction).EndInit();
            tsScanResults.ResumeLayout(false);
            tsScanResults.PerformLayout();
            tbWTW.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            toolStripContainer3.ContentPanel.ResumeLayout(false);
            toolStripContainer3.TopToolStripPanel.ResumeLayout(false);
            toolStripContainer3.TopToolStripPanel.PerformLayout();
            toolStripContainer3.ResumeLayout(false);
            toolStripContainer3.PerformLayout();
            tsWtW.ResumeLayout(false);
            tsWtW.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
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
        private ToolStripMenuItem fullToolStripMenuItem;
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
        private TabPage tabPage2;
        private System.ComponentModel.BackgroundWorker bwMovieHTMLGenerator;
        private ToolStripSeparator toolStripSeparator17;
        private ToolStripMenuItem bulkAddMoviesToolStripMenuItem;
        private ToolStripMenuItem tMDBAccuracyCheckLogToolStripMenuItem;
        private ToolStripMenuItem duplicateMoviesToolStripMenuItem;
        private ToolStripMenuItem movieCollectionSummaryLogToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator19;
        private ToolStripButton tsbScanMovies;
        private ToolStripMenuItem movieSearchEnginesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator20;
        private ToolStripMenuItem settingsCheckToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator21;
        private ToolStripSeparator toolStripSeparator121;
        private ToolStripMenuItem movieRecommendationsToolStripMenuItem;
        private ToolStripMenuItem tvRecommendationsToolStripMenuItem;
        private ToolStripMenuItem scanMovieFolderToolStripMenuItem;
        private CefSharp.WinForms.ChromiumWebBrowser chrMovieInformation;
        private CefSharp.WinForms.ChromiumWebBrowser chrInformation;
        private CefSharp.WinForms.ChromiumWebBrowser chrImages;
        private CefSharp.WinForms.ChromiumWebBrowser chrSummary;
        private CefSharp.WinForms.ChromiumWebBrowser chrMovieImages;
        private System.ComponentModel.BackgroundWorker bwScan;
        private TabPage tpMovieTrailer;
        private CefSharp.WinForms.ChromiumWebBrowser chrMovieTrailer;
        private TabPage tabPage3;
        private CefSharp.WinForms.ChromiumWebBrowser chrTvTrailer;
        private ToolStripMenuItem browserTestToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker bwAction;
        private ToolStripButton tsbScanTV;
        private ToolStripMenuItem cleanLibraryFoldersToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator18;
        private ToolStripSeparator toolStripSeparator22;
        private ToolStripMenuItem webTestToolStripMenuItem;
        private ToolStripMenuItem downloadInstallerToolStripMenuItem;
        private ToolStripContainer toolStripContainer1;
        private ToolStripContainer toolStripContainer2;
        private ToolStripContainer toolStripContainer3;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel3;
        private Panel panel1;
        private TableLayoutPanel tableLayoutPanel4;
        private ToolStripMenuItem tVDBUPdateCheckerLogToolStripMenuItem;
        private ToolStripMenuItem requestANewFeatureToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator23;
        private ToolStripSeparator toolStripSeparator24;
        private ToolStripMenuItem yTSMoviePreviewToolStripMenuItem;
        private ToolStripMenuItem yTSMovieRecommendationsToolStripMenuItem;
        private ToolStripMenuItem forceRefreshKodiTVShowNFOFIlesToolStripMenuItem;
        private ToolStripMenuItem removeShowsWithNoFoldersToolStripMenuItem;
    }
}
