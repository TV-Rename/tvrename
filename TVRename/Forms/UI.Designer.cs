//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Windows.Forms;

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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Missing", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Rename", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Copy", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Move", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Remove", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("Download RSS", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup7 = new System.Windows.Forms.ListViewGroup("Download", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup8 = new System.Windows.Forms.ListViewGroup("Media Center Metadata", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup9 = new System.Windows.Forms.ListViewGroup("Update File Metadata", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup10 = new System.Windows.Forms.ListViewGroup("Downloading", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup11 = new System.Windows.Forms.ListViewGroup("Recently Aired", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup12 = new System.Windows.Forms.ListViewGroup("Next 7 Days", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup13 = new System.Windows.Forms.ListViewGroup("Future Episodes", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup14 = new System.Windows.Forms.ListViewGroup("Later", System.Windows.Forms.HorizontalAlignment.Left);
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
            this.searchEnginesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filenameProcessorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flushCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flushImageCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundDownloadNowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.folderMonitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateFinderLOGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickRenameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statisticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.showSummaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.betaToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timezoneInconsistencyLOGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.episodeFileQualitySummaryLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.accuracyCheckLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.tbMyShows = new System.Windows.Forms.TabPage();
            this.tsMyShows = new System.Windows.Forms.ToolStrip();
            this.btnAddShow = new System.Windows.Forms.ToolStripButton();
            this.btnEditShow = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveShow = new System.Windows.Forms.ToolStripButton();
            this.btnHideHTMLPanel = new System.Windows.Forms.ToolStripButton();
            this.btnMyShowsCollapse = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.btnMyShowsRefresh = new System.Windows.Forms.ToolStripButton();
            this.btnFilterMyShows = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.MyShowTree = new System.Windows.Forms.TreeView();
            this.filterTextBox = new System.Windows.Forms.TextBox();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.webInformation = new System.Windows.Forms.WebBrowser();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.webImages = new System.Windows.Forms.WebBrowser();
            this.tbAllInOne = new System.Windows.Forms.TabPage();
            this.tsScanResults = new System.Windows.Forms.ToolStrip();
            this.btnScan = new System.Windows.Forms.ToolStripSplitButton();
            this.btnFullScan = new System.Windows.Forms.ToolStripMenuItem();
            this.recentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.btnActionBTSearch = new System.Windows.Forms.ToolStripSplitButton();
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
            this.btnPreferences = new System.Windows.Forms.ToolStripButton();
            this.lvAction = new MyListView();
            this.columnHeader48 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader49 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader51 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader52 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader53 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader54 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader55 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader58 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ilIcons = new System.Windows.Forms.ImageList(this.components);
            this.tbWTW = new System.Windows.Forms.TabPage();
            this.tsWtW = new System.Windows.Forms.ToolStrip();
            this.btnWhenToWatchCheck = new System.Windows.Forms.ToolStripButton();
            this.btnWTWBTSearch = new System.Windows.Forms.ToolStripSplitButton();
            this.txtWhenToWatchSynopsis = new System.Windows.Forms.TextBox();
            this.calCalendar = new System.Windows.Forms.MonthCalendar();
            this.lvWhenToWatch = new ListViewFlickerFree();
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
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tbMyShows.SuspendLayout();
            this.tsMyShows.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tbAllInOne.SuspendLayout();
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
            this.menuStrip1.Size = new System.Drawing.Size(1039, 24);
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
            this.searchEnginesToolStripMenuItem,
            this.filenameProcessorsToolStripMenuItem});
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
            // searchEnginesToolStripMenuItem
            // 
            this.searchEnginesToolStripMenuItem.Image = global::TVRename.Properties.Resources.SearchWebHS;
            this.searchEnginesToolStripMenuItem.Name = "searchEnginesToolStripMenuItem";
            this.searchEnginesToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.searchEnginesToolStripMenuItem.Text = "&Search Engines";
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
            this.toolStripSeparator3,
            this.folderMonitorToolStripMenuItem,
            this.duplicateFinderLOGToolStripMenuItem,
            this.quickRenameToolStripMenuItem});
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
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(261, 6);
            // 
            // folderMonitorToolStripMenuItem
            // 
            this.folderMonitorToolStripMenuItem.Image = global::TVRename.Properties.Resources.SearchFolderHS;
            this.folderMonitorToolStripMenuItem.Name = "folderMonitorToolStripMenuItem";
            this.folderMonitorToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.folderMonitorToolStripMenuItem.Text = "Bulk &Add Shows...";
            this.folderMonitorToolStripMenuItem.Click += new System.EventHandler(this.folderMonitorToolStripMenuItem_Click);
            // 
            // duplicateFinderLOGToolStripMenuItem
            // 
            this.duplicateFinderLOGToolStripMenuItem.Name = "duplicateFinderLOGToolStripMenuItem";
            this.duplicateFinderLOGToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.duplicateFinderLOGToolStripMenuItem.Text = "Duplicate Episode Finder...";
            this.duplicateFinderLOGToolStripMenuItem.Click += new System.EventHandler(this.duplicateFinderLOGToolStripMenuItem_Click);
            // 
            // quickRenameToolStripMenuItem
            // 
            this.quickRenameToolStripMenuItem.Name = "quickRenameToolStripMenuItem";
            this.quickRenameToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.quickRenameToolStripMenuItem.Text = "Quick Rename...";
            this.quickRenameToolStripMenuItem.Click += new System.EventHandler(this.QuickRenameToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statisticsToolStripMenuItem,
            this.toolStripSeparator5,
            this.showSummaryToolStripMenuItem,
            this.actorsToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // statisticsToolStripMenuItem
            // 
            this.statisticsToolStripMenuItem.Image = global::TVRename.Properties.Resources.graphhs;
            this.statisticsToolStripMenuItem.Name = "statisticsToolStripMenuItem";
            this.statisticsToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.statisticsToolStripMenuItem.Text = "&Statistics...";
            this.statisticsToolStripMenuItem.Click += new System.EventHandler(this.statisticsToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(163, 6);
            // 
            // showSummaryToolStripMenuItem
            // 
            this.showSummaryToolStripMenuItem.Name = "showSummaryToolStripMenuItem";
            this.showSummaryToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.showSummaryToolStripMenuItem.Text = "Show Summary...";
            this.showSummaryToolStripMenuItem.Click += new System.EventHandler(this.showSummaryToolStripMenuItem_Click);
            // 
            // actorsToolStripMenuItem
            // 
            this.actorsToolStripMenuItem.Image = global::TVRename.Properties.Resources.TableHS;
            this.actorsToolStripMenuItem.Name = "actorsToolStripMenuItem";
            this.actorsToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.actorsToolStripMenuItem.Text = "&Actors Grid...";
            this.actorsToolStripMenuItem.Click += new System.EventHandler(this.actorsToolStripMenuItem_Click);
            // 
            // betaToolsToolStripMenuItem
            // 
            this.betaToolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.timezoneInconsistencyLOGToolStripMenuItem,
            this.episodeFileQualitySummaryLogToolStripMenuItem,
            this.accuracyCheckLogToolStripMenuItem});
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
            // accuracyCheckLogToolStripMenuItem
            // 
            this.accuracyCheckLogToolStripMenuItem.Name = "accuracyCheckLogToolStripMenuItem";
            this.accuracyCheckLogToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.accuracyCheckLogToolStripMenuItem.Text = "Accuracy Check (Log)";
            this.accuracyCheckLogToolStripMenuItem.Click += new System.EventHandler(this.AccuracyCheckLogToolStripMenuItem_Click);
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
            this.tabControl1.Size = new System.Drawing.Size(1039, 667);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 0;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.TabControl1_DrawItem);
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.DoubleClick += new System.EventHandler(this.tabControl1_DoubleClick);
            // 
            // tbMyShows
            // 
            this.tbMyShows.Controls.Add(this.tsMyShows);
            this.tbMyShows.Controls.Add(this.splitContainer1);
            this.tbMyShows.ImageKey = "3790574-48.png";
            this.tbMyShows.Location = new System.Drawing.Point(104, 4);
            this.tbMyShows.Name = "tbMyShows";
            this.tbMyShows.Padding = new System.Windows.Forms.Padding(3);
            this.tbMyShows.Size = new System.Drawing.Size(931, 659);
            this.tbMyShows.TabIndex = 9;
            this.tbMyShows.Text = "My Shows";
            this.tbMyShows.UseVisualStyleBackColor = true;
            // 
            // tsMyShows
            // 
            this.tsMyShows.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddShow,
            this.btnEditShow,
            this.btnRemoveShow,
            this.btnHideHTMLPanel,
            this.btnMyShowsCollapse,
            this.toolStripSeparator4,
            this.btnMyShowsRefresh,
            this.btnFilterMyShows});
            this.tsMyShows.Location = new System.Drawing.Point(3, 3);
            this.tsMyShows.Name = "tsMyShows";
            this.tsMyShows.Size = new System.Drawing.Size(925, 72);
            this.tsMyShows.TabIndex = 11;
            this.tsMyShows.Text = "toolStrip1";
            // 
            // btnAddShow
            // 
            this.btnAddShow.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddShow.Image = global::TVRename.Properties.Resources.AddSmall;
            this.btnAddShow.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnAddShow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddShow.Name = "btnAddShow";
            this.btnAddShow.Size = new System.Drawing.Size(52, 69);
            this.btnAddShow.Text = "&Add";
            this.btnAddShow.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnAddShow.Click += new System.EventHandler(this.bnMyShowsAdd_Click);
            // 
            // btnEditShow
            // 
            this.btnEditShow.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditShow.Image = global::TVRename.Properties.Resources.EditShowSmall;
            this.btnEditShow.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnEditShow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditShow.Name = "btnEditShow";
            this.btnEditShow.Size = new System.Drawing.Size(52, 69);
            this.btnEditShow.Text = "&Edit";
            this.btnEditShow.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnEditShow.Click += new System.EventHandler(this.bnMyShowsEdit_Click);
            // 
            // btnRemoveShow
            // 
            this.btnRemoveShow.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemoveShow.Image = global::TVRename.Properties.Resources.DeleteShowSmall;
            this.btnRemoveShow.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnRemoveShow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemoveShow.Name = "btnRemoveShow";
            this.btnRemoveShow.Size = new System.Drawing.Size(52, 69);
            this.btnRemoveShow.Text = "&Delete";
            this.btnRemoveShow.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
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
            this.btnHideHTMLPanel.Size = new System.Drawing.Size(23, 69);
            this.btnHideHTMLPanel.Text = "Hide Detals";
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
            this.btnMyShowsCollapse.Size = new System.Drawing.Size(23, 69);
            this.btnMyShowsCollapse.Text = "Collapse";
            this.btnMyShowsCollapse.Click += new System.EventHandler(this.BtnMyShowsCollapse_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 72);
            // 
            // btnMyShowsRefresh
            // 
            this.btnMyShowsRefresh.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMyShowsRefresh.Image = global::TVRename.Properties.Resources.RefreshSmall;
            this.btnMyShowsRefresh.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnMyShowsRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMyShowsRefresh.Name = "btnMyShowsRefresh";
            this.btnMyShowsRefresh.Size = new System.Drawing.Size(56, 69);
            this.btnMyShowsRefresh.Text = "&Refresh";
            this.btnMyShowsRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnMyShowsRefresh.Click += new System.EventHandler(this.bnMyShowsRefresh_Click);
            // 
            // btnFilterMyShows
            // 
            this.btnFilterMyShows.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilterMyShows.Image = global::TVRename.Properties.Resources.FilterSmall;
            this.btnFilterMyShows.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnFilterMyShows.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFilterMyShows.Name = "btnFilterMyShows";
            this.btnFilterMyShows.Size = new System.Drawing.Size(52, 69);
            this.btnFilterMyShows.Text = "&Filter";
            this.btnFilterMyShows.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnFilterMyShows.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 76);
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
            this.splitContainer1.Size = new System.Drawing.Size(931, 583);
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
            this.MyShowTree.Size = new System.Drawing.Size(276, 559);
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
            this.tabControl2.Controls.Add(this.tabPage1);
            this.tabControl2.Controls.Add(this.tabPage2);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(643, 579);
            this.tabControl2.TabIndex = 7;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.webInformation);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(635, 550);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Information";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // webInformation
            // 
            this.webInformation.AllowWebBrowserDrop = false;
            this.webInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webInformation.Location = new System.Drawing.Point(3, 3);
            this.webInformation.MinimumSize = new System.Drawing.Size(20, 20);
            this.webInformation.Name = "webInformation";
            this.webInformation.Size = new System.Drawing.Size(629, 544);
            this.webInformation.TabIndex = 0;
            this.webInformation.WebBrowserShortcutsEnabled = false;
            this.webInformation.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.NavigateTo);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.webImages);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(635, 550);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Images";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // webImages
            // 
            this.webImages.AllowWebBrowserDrop = false;
            this.webImages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webImages.Location = new System.Drawing.Point(3, 3);
            this.webImages.MinimumSize = new System.Drawing.Size(20, 20);
            this.webImages.Name = "webImages";
            this.webImages.Size = new System.Drawing.Size(629, 544);
            this.webImages.TabIndex = 0;
            this.webImages.WebBrowserShortcutsEnabled = false;
            this.webImages.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.NavigateTo);
            // 
            // tbAllInOne
            // 
            this.tbAllInOne.Controls.Add(this.tsScanResults);
            this.tbAllInOne.Controls.Add(this.lvAction);
            this.tbAllInOne.ImageKey = "322497-48 (1).png";
            this.tbAllInOne.Location = new System.Drawing.Point(104, 4);
            this.tbAllInOne.Name = "tbAllInOne";
            this.tbAllInOne.Padding = new System.Windows.Forms.Padding(3);
            this.tbAllInOne.Size = new System.Drawing.Size(931, 659);
            this.tbAllInOne.TabIndex = 11;
            this.tbAllInOne.Text = "Scan";
            this.tbAllInOne.UseVisualStyleBackColor = true;
            // 
            // tsScanResults
            // 
            this.tsScanResults.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnScan,
            this.toolStripSeparator11,
            this.btnActionBTSearch,
            this.toolStripSeparator9,
            this.btnIgnoreSelectedActions,
            this.btnRemoveSelActions,
            this.toolStripDropDownButton1,
            this.toolStripSeparator10,
            this.btnActionAction,
            this.btnPreferences});
            this.tsScanResults.Location = new System.Drawing.Point(3, 3);
            this.tsScanResults.Name = "tsScanResults";
            this.tsScanResults.Size = new System.Drawing.Size(925, 70);
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
            this.btnScan.Image = global::TVRename.Properties.Resources.ScanSmall;
            this.btnScan.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnScan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(105, 67);
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
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 70);
            // 
            // btnActionBTSearch
            // 
            this.btnActionBTSearch.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnActionBTSearch.Image = global::TVRename.Properties.Resources.SearchWebSmall;
            this.btnActionBTSearch.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnActionBTSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnActionBTSearch.Name = "btnActionBTSearch";
            this.btnActionBTSearch.Size = new System.Drawing.Size(139, 67);
            this.btnActionBTSearch.Text = "BT Search";
            this.btnActionBTSearch.ButtonClick += new System.EventHandler(this.bnActionBTSearch_Click);
            this.btnActionBTSearch.DropDownOpening += new System.EventHandler(this.BtnActionBTSearch_DropDownOpening);
            this.btnActionBTSearch.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuSearchSites_ItemClicked);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 70);
            // 
            // btnIgnoreSelectedActions
            // 
            this.btnIgnoreSelectedActions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnIgnoreSelectedActions.Image = ((System.Drawing.Image)(resources.GetObject("btnIgnoreSelectedActions.Image")));
            this.btnIgnoreSelectedActions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnIgnoreSelectedActions.Name = "btnIgnoreSelectedActions";
            this.btnIgnoreSelectedActions.Size = new System.Drawing.Size(63, 67);
            this.btnIgnoreSelectedActions.Text = "&Ignore Sel";
            this.btnIgnoreSelectedActions.Click += new System.EventHandler(this.cbActionIgnore_Click);
            // 
            // btnRemoveSelActions
            // 
            this.btnRemoveSelActions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRemoveSelActions.Image = ((System.Drawing.Image)(resources.GetObject("btnRemoveSelActions.Image")));
            this.btnRemoveSelActions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemoveSelActions.Name = "btnRemoveSelActions";
            this.btnRemoveSelActions.Size = new System.Drawing.Size(72, 67);
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
            this.toolStripDropDownButton1.Image = global::TVRename.Properties.Resources.CheckBoxSmall;
            this.toolStripDropDownButton1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(101, 67);
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
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 70);
            // 
            // btnActionAction
            // 
            this.btnActionAction.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnActionAction.Image = global::TVRename.Properties.Resources.DoSmall;
            this.btnActionAction.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnActionAction.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnActionAction.Name = "btnActionAction";
            this.btnActionAction.Size = new System.Drawing.Size(143, 67);
            this.btnActionAction.Text = "&Do Checked";
            this.btnActionAction.Click += new System.EventHandler(this.bnActionAction_Click);
            // 
            // btnPreferences
            // 
            this.btnPreferences.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnPreferences.Image = global::TVRename.Properties.Resources.PreferencesSmall;
            this.btnPreferences.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnPreferences.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPreferences.Name = "btnPreferences";
            this.btnPreferences.Size = new System.Drawing.Size(81, 67);
            this.btnPreferences.Text = "&Preferences...";
            this.btnPreferences.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPreferences.Click += new System.EventHandler(this.bnActionOptions_Click);
            // 
            // lvAction
            // 
            this.lvAction.AllowDrop = true;
            this.lvAction.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvAction.CheckBoxes = true;
            this.lvAction.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader48,
            this.columnHeader49,
            this.columnHeader51,
            this.columnHeader52,
            this.columnHeader53,
            this.columnHeader54,
            this.columnHeader55,
            this.columnHeader58});
            this.lvAction.FullRowSelect = true;
            listViewGroup1.Header = "Missing";
            listViewGroup1.Name = "lvgActionMissing";
            listViewGroup2.Header = "Rename";
            listViewGroup2.Name = "lvgActionRename";
            listViewGroup3.Header = "Copy";
            listViewGroup3.Name = "lvgActionCopy";
            listViewGroup4.Header = "Move";
            listViewGroup4.Name = "lvgActionMove";
            listViewGroup5.Header = "Remove";
            listViewGroup5.Name = "lvgActionDelete";
            listViewGroup6.Header = "Download RSS";
            listViewGroup6.Name = "lvgActionDownloadRSS";
            listViewGroup7.Header = "Download";
            listViewGroup7.Name = "lvgActionDownload";
            listViewGroup8.Header = "Media Center Metadata";
            listViewGroup8.Name = "lvgActionMeta";
            listViewGroup9.Header = "Update File Metadata";
            listViewGroup9.Name = "lvgUpdateFileDates";
            listViewGroup10.Header = "Downloading";
            listViewGroup10.Name = "lvgDownloading";
            this.lvAction.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4,
            listViewGroup5,
            listViewGroup6,
            listViewGroup7,
            listViewGroup8,
            listViewGroup9,
            listViewGroup10});
            this.lvAction.HideSelection = false;
            this.lvAction.Location = new System.Drawing.Point(3, 76);
            this.lvAction.Name = "lvAction";
            this.lvAction.ShowItemToolTips = true;
            this.lvAction.Size = new System.Drawing.Size(925, 583);
            this.lvAction.SmallImageList = this.ilIcons;
            this.lvAction.TabIndex = 2;
            this.lvAction.UseCompatibleStateImageBehavior = false;
            this.lvAction.View = System.Windows.Forms.View.Details;
            this.lvAction.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.LvAction_ColumnClick);
            this.lvAction.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lvAction_ItemCheck);
            this.lvAction.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvAction_ItemChecked);
            this.lvAction.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lvAction_RetrieveVirtualItem);
            this.lvAction.SelectedIndexChanged += new System.EventHandler(this.lvAction_SelectedIndexChanged);
            this.lvAction.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvAction_DragDrop);
            this.lvAction.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvAction_DragEnter);
            this.lvAction.DragOver += new System.Windows.Forms.DragEventHandler(this.lvAction_DragEnter);
            this.lvAction.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvAction_KeyDown);
            this.lvAction.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvAction_MouseClick);
            this.lvAction.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvAction_MouseDoubleClick);
            // 
            // columnHeader48
            // 
            this.columnHeader48.Text = "Show";
            this.columnHeader48.Width = 155;
            // 
            // columnHeader49
            // 
            this.columnHeader49.Text = "Season";
            this.columnHeader49.Width = 50;
            // 
            // columnHeader51
            // 
            this.columnHeader51.Text = "Episode";
            this.columnHeader51.Width = 50;
            // 
            // columnHeader52
            // 
            this.columnHeader52.Text = "Date";
            this.columnHeader52.Width = 70;
            // 
            // columnHeader53
            // 
            this.columnHeader53.Text = "Folder";
            this.columnHeader53.Width = 180;
            // 
            // columnHeader54
            // 
            this.columnHeader54.Text = "Filename";
            this.columnHeader54.Width = 180;
            // 
            // columnHeader55
            // 
            this.columnHeader55.Text = "Source";
            this.columnHeader55.Width = 180;
            // 
            // columnHeader58
            // 
            this.columnHeader58.Text = "Errors";
            this.columnHeader58.Width = 180;
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
            // tbWTW
            // 
            this.tbWTW.Controls.Add(this.tsWtW);
            this.tbWTW.Controls.Add(this.txtWhenToWatchSynopsis);
            this.tbWTW.Controls.Add(this.calCalendar);
            this.tbWTW.Controls.Add(this.lvWhenToWatch);
            this.tbWTW.ImageKey = "115762-48.png";
            this.tbWTW.Location = new System.Drawing.Point(104, 4);
            this.tbWTW.Name = "tbWTW";
            this.tbWTW.Size = new System.Drawing.Size(931, 659);
            this.tbWTW.TabIndex = 4;
            this.tbWTW.Text = "When to Watch";
            this.tbWTW.UseVisualStyleBackColor = true;
            // 
            // tsWtW
            // 
            this.tsWtW.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnWhenToWatchCheck,
            this.btnWTWBTSearch});
            this.tsWtW.Location = new System.Drawing.Point(0, 0);
            this.tsWtW.Name = "tsWtW";
            this.tsWtW.Size = new System.Drawing.Size(931, 55);
            this.tsWtW.TabIndex = 6;
            this.tsWtW.Text = "toolStrip1";
            // 
            // btnWhenToWatchCheck
            // 
            this.btnWhenToWatchCheck.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWhenToWatchCheck.Image = global::TVRename.Properties.Resources.RefreshSmall;
            this.btnWhenToWatchCheck.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnWhenToWatchCheck.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnWhenToWatchCheck.Name = "btnWhenToWatchCheck";
            this.btnWhenToWatchCheck.Size = new System.Drawing.Size(112, 52);
            this.btnWhenToWatchCheck.Text = "&Refresh";
            this.btnWhenToWatchCheck.Click += new System.EventHandler(this.bnWhenToWatchCheck_Click);
            // 
            // btnWTWBTSearch
            // 
            this.btnWTWBTSearch.DropDownButtonWidth = 20;
            this.btnWTWBTSearch.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWTWBTSearch.Image = global::TVRename.Properties.Resources.SearchWebSmall;
            this.btnWTWBTSearch.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnWTWBTSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnWTWBTSearch.Name = "btnWTWBTSearch";
            this.btnWTWBTSearch.Size = new System.Drawing.Size(148, 52);
            this.btnWTWBTSearch.Text = "BT Search";
            this.btnWTWBTSearch.ButtonClick += new System.EventHandler(this.bnWTWBTSearch_Click);
            this.btnWTWBTSearch.DropDownOpening += new System.EventHandler(this.ToolStripSplitButton1_DropDownOpening);
            this.btnWTWBTSearch.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuSearchSites_ItemClicked);
            // 
            // txtWhenToWatchSynopsis
            // 
            this.txtWhenToWatchSynopsis.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWhenToWatchSynopsis.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWhenToWatchSynopsis.Location = new System.Drawing.Point(3, 495);
            this.txtWhenToWatchSynopsis.Multiline = true;
            this.txtWhenToWatchSynopsis.Name = "txtWhenToWatchSynopsis";
            this.txtWhenToWatchSynopsis.ReadOnly = true;
            this.txtWhenToWatchSynopsis.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtWhenToWatchSynopsis.Size = new System.Drawing.Size(697, 161);
            this.txtWhenToWatchSynopsis.TabIndex = 4;
            // 
            // calCalendar
            // 
            this.calCalendar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.calCalendar.Location = new System.Drawing.Point(701, 494);
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
            listViewGroup11.Header = "Recently Aired";
            listViewGroup11.Name = "justPassed";
            listViewGroup12.Header = "Next 7 Days";
            listViewGroup12.Name = "next7days";
            listViewGroup12.Tag = "1";
            listViewGroup13.Header = "Future Episodes";
            listViewGroup13.Name = "futureEps";
            listViewGroup14.Header = "Later";
            listViewGroup14.Name = "later";
            listViewGroup14.Tag = "2";
            this.lvWhenToWatch.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup11,
            listViewGroup12,
            listViewGroup13,
            listViewGroup14});
            this.lvWhenToWatch.HideSelection = false;
            this.lvWhenToWatch.Location = new System.Drawing.Point(0, 58);
            this.lvWhenToWatch.Name = "lvWhenToWatch";
            this.lvWhenToWatch.ShowItemToolTips = true;
            this.lvWhenToWatch.Size = new System.Drawing.Size(928, 431);
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
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel2.Controls.Add(this.pbProgressBarx, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtDLStatusLabel, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.tsNextShowTxt, 0, 0);
            this.tableLayoutPanel2.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 693);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1027, 19);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // pbProgressBarx
            // 
            this.pbProgressBarx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgressBarx.Location = new System.Drawing.Point(875, 3);
            this.pbProgressBarx.Name = "pbProgressBarx";
            this.pbProgressBarx.Size = new System.Drawing.Size(149, 13);
            this.pbProgressBarx.Step = 1;
            this.pbProgressBarx.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbProgressBarx.TabIndex = 0;
            // 
            // txtDLStatusLabel
            // 
            this.txtDLStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDLStatusLabel.Location = new System.Drawing.Point(465, 6);
            this.txtDLStatusLabel.Name = "txtDLStatusLabel";
            this.txtDLStatusLabel.Size = new System.Drawing.Size(404, 13);
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
            this.tsNextShowTxt.Size = new System.Drawing.Size(456, 13);
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
            this.btnUpdateAvailable.Location = new System.Drawing.Point(916, 0);
            this.btnUpdateAvailable.Name = "btnUpdateAvailable";
            this.btnUpdateAvailable.Size = new System.Drawing.Size(116, 23);
            this.btnUpdateAvailable.TabIndex = 10;
            this.btnUpdateAvailable.Text = "Update Available...";
            this.btnUpdateAvailable.UseVisualStyleBackColor = false;
            this.btnUpdateAvailable.Visible = false;
            this.btnUpdateAvailable.Click += new System.EventHandler(this.btnUpdateAvailable_Click);
            // 
            // UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1039, 716);
            this.Controls.Add(this.btnUpdateAvailable);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.tabControl1);
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
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
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
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tbAllInOne.ResumeLayout(false);
            this.tbAllInOne.PerformLayout();
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
        private System.Windows.Forms.ColumnHeader columnHeader48;
        private System.Windows.Forms.ColumnHeader columnHeader49;

        private System.Windows.Forms.ColumnHeader columnHeader51;
        private System.Windows.Forms.ColumnHeader columnHeader52;
        private System.Windows.Forms.ColumnHeader columnHeader53;
        private System.Windows.Forms.ColumnHeader columnHeader54;
        private System.Windows.Forms.ColumnHeader columnHeader55;

        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem folderMonitorToolStripMenuItem;

        private System.Windows.Forms.ColumnHeader columnHeader58;
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
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
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
        private ToolStripButton btnAddShow;
        private ToolStripButton btnEditShow;
        private ToolStripButton btnRemoveShow;
        private ToolStripButton btnHideHTMLPanel;
        private ToolStripButton btnMyShowsCollapse;
        private ToolStrip tsWtW;
        private ToolStripButton btnWhenToWatchCheck;
        private ToolStripSplitButton btnWTWBTSearch;
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
    }
}
