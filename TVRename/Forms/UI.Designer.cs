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
            System.Windows.Forms.ListViewGroup listViewGroup15 = new System.Windows.Forms.ListViewGroup("Missing", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup16 = new System.Windows.Forms.ListViewGroup("Rename", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup17 = new System.Windows.Forms.ListViewGroup("Copy", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup18 = new System.Windows.Forms.ListViewGroup("Move", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup19 = new System.Windows.Forms.ListViewGroup("Remove", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup20 = new System.Windows.Forms.ListViewGroup("Download RSS", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup21 = new System.Windows.Forms.ListViewGroup("Download", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup22 = new System.Windows.Forms.ListViewGroup("Media Center Metadata", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup23 = new System.Windows.Forms.ListViewGroup("Update File Metadata", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup24 = new System.Windows.Forms.ListViewGroup("Downloading", System.Windows.Forms.HorizontalAlignment.Left);
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
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tbMyShows = new System.Windows.Forms.TabPage();
            this.btnFilter = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.bnHideHTMLPanel = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.MyShowTree = new System.Windows.Forms.TreeView();
            this.filterTextBox = new System.Windows.Forms.TextBox();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.webInformation = new System.Windows.Forms.WebBrowser();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.webImages = new System.Windows.Forms.WebBrowser();
            this.bnMyShowsCollapse = new System.Windows.Forms.Button();
            this.bnMyShowsRefresh = new System.Windows.Forms.Button();
            this.bnMyShowsDelete = new System.Windows.Forms.Button();
            this.bnMyShowsEdit = new System.Windows.Forms.Button();
            this.bnMyShowsAdd = new System.Windows.Forms.Button();
            this.tbAllInOne = new System.Windows.Forms.TabPage();
            this.cbDeleteFiles = new System.Windows.Forms.CheckBox();
            this.btnActionQuickScan = new System.Windows.Forms.Button();
            this.cbModifyMetadata = new System.Windows.Forms.CheckBox();
            this.cbWriteMetadata = new System.Windows.Forms.CheckBox();
            this.cbSaveImages = new System.Windows.Forms.CheckBox();
            this.cbDownload = new System.Windows.Forms.CheckBox();
            this.cbCopyMove = new System.Windows.Forms.CheckBox();
            this.cbRename = new System.Windows.Forms.CheckBox();
            this.cbAll = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bnRemoveSel = new System.Windows.Forms.Button();
            this.bnActionIgnore = new System.Windows.Forms.Button();
            this.bnActionOptions = new System.Windows.Forms.Button();
            this.bnActionWhichSearch = new System.Windows.Forms.Button();
            this.bnActionBTSearch = new System.Windows.Forms.Button();
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
            this.bnActionAction = new System.Windows.Forms.Button();
            this.bnActionRecentCheck = new System.Windows.Forms.Button();
            this.bnActionCheck = new System.Windows.Forms.Button();
            this.tbWTW = new System.Windows.Forms.TabPage();
            this.bnWTWChooseSite = new System.Windows.Forms.Button();
            this.bnWTWBTSearch = new System.Windows.Forms.Button();
            this.bnWhenToWatchCheck = new System.Windows.Forms.Button();
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
            this.menuSearchSites = new System.Windows.Forms.ContextMenuStrip(this.components);
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
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tbAllInOne.SuspendLayout();
            this.tbWTW.SuspendLayout();
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
            this.menuStrip1.Size = new System.Drawing.Size(975, 24);
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
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tbMyShows);
            this.tabControl1.Controls.Add(this.tbAllInOne);
            this.tabControl1.Controls.Add(this.tbWTW);
            this.tabControl1.ImageList = this.imageList1;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(975, 568);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.DoubleClick += new System.EventHandler(this.tabControl1_DoubleClick);
            // 
            // tbMyShows
            // 
            this.tbMyShows.Controls.Add(this.btnFilter);
            this.tbMyShows.Controls.Add(this.bnHideHTMLPanel);
            this.tbMyShows.Controls.Add(this.splitContainer1);
            this.tbMyShows.Controls.Add(this.bnMyShowsCollapse);
            this.tbMyShows.Controls.Add(this.bnMyShowsRefresh);
            this.tbMyShows.Controls.Add(this.bnMyShowsDelete);
            this.tbMyShows.Controls.Add(this.bnMyShowsEdit);
            this.tbMyShows.Controls.Add(this.bnMyShowsAdd);
            this.tbMyShows.ImageKey = "TVOff.bmp";
            this.tbMyShows.Location = new System.Drawing.Point(4, 23);
            this.tbMyShows.Name = "tbMyShows";
            this.tbMyShows.Padding = new System.Windows.Forms.Padding(3);
            this.tbMyShows.Size = new System.Drawing.Size(967, 541);
            this.tbMyShows.TabIndex = 9;
            this.tbMyShows.Text = "My Shows";
            this.tbMyShows.UseVisualStyleBackColor = true;
            // 
            // btnFilter
            // 
            this.btnFilter.CausesValidation = false;
            this.btnFilter.ImageKey = "filtre.png";
            this.btnFilter.ImageList = this.imageList1;
            this.btnFilter.Location = new System.Drawing.Point(330, 6);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(75, 25);
            this.btnFilter.TabIndex = 10;
            this.btnFilter.Text = "&Filter";
            this.btnFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
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
            // bnHideHTMLPanel
            // 
            this.bnHideHTMLPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnHideHTMLPanel.ImageKey = "FillRight.bmp";
            this.bnHideHTMLPanel.ImageList = this.imageList1;
            this.bnHideHTMLPanel.Location = new System.Drawing.Point(936, 6);
            this.bnHideHTMLPanel.Name = "bnHideHTMLPanel";
            this.bnHideHTMLPanel.Size = new System.Drawing.Size(25, 25);
            this.bnHideHTMLPanel.TabIndex = 9;
            this.bnHideHTMLPanel.UseVisualStyleBackColor = true;
            this.bnHideHTMLPanel.Click += new System.EventHandler(this.bnHideHTMLPanel_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 37);
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
            this.splitContainer1.Size = new System.Drawing.Size(967, 500);
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
            this.MyShowTree.Size = new System.Drawing.Size(276, 476);
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
            this.tabControl2.Size = new System.Drawing.Size(679, 496);
            this.tabControl2.TabIndex = 7;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.webInformation);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(671, 467);
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
            this.webInformation.Size = new System.Drawing.Size(665, 461);
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
            this.tabPage2.Size = new System.Drawing.Size(671, 464);
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
            this.webImages.Size = new System.Drawing.Size(665, 458);
            this.webImages.TabIndex = 0;
            this.webImages.WebBrowserShortcutsEnabled = false;
            this.webImages.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.NavigateTo);
            // 
            // bnMyShowsCollapse
            // 
            this.bnMyShowsCollapse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnMyShowsCollapse.ImageKey = "Control_TreeView.bmp";
            this.bnMyShowsCollapse.ImageList = this.imageList1;
            this.bnMyShowsCollapse.Location = new System.Drawing.Point(908, 6);
            this.bnMyShowsCollapse.Name = "bnMyShowsCollapse";
            this.bnMyShowsCollapse.Size = new System.Drawing.Size(25, 25);
            this.bnMyShowsCollapse.TabIndex = 4;
            this.bnMyShowsCollapse.UseVisualStyleBackColor = true;
            this.bnMyShowsCollapse.Click += new System.EventHandler(this.bnMyShowsCollapse_Click);
            // 
            // bnMyShowsRefresh
            // 
            this.bnMyShowsRefresh.ImageKey = "Refresh.bmp";
            this.bnMyShowsRefresh.ImageList = this.imageList1;
            this.bnMyShowsRefresh.Location = new System.Drawing.Point(6, 6);
            this.bnMyShowsRefresh.Name = "bnMyShowsRefresh";
            this.bnMyShowsRefresh.Size = new System.Drawing.Size(75, 25);
            this.bnMyShowsRefresh.TabIndex = 5;
            this.bnMyShowsRefresh.Text = "&Refresh";
            this.bnMyShowsRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bnMyShowsRefresh.UseVisualStyleBackColor = true;
            this.bnMyShowsRefresh.Click += new System.EventHandler(this.bnMyShowsRefresh_Click);
            // 
            // bnMyShowsDelete
            // 
            this.bnMyShowsDelete.ImageKey = "delete.bmp";
            this.bnMyShowsDelete.ImageList = this.imageList1;
            this.bnMyShowsDelete.Location = new System.Drawing.Point(249, 6);
            this.bnMyShowsDelete.Name = "bnMyShowsDelete";
            this.bnMyShowsDelete.Size = new System.Drawing.Size(75, 25);
            this.bnMyShowsDelete.TabIndex = 3;
            this.bnMyShowsDelete.Text = "&Delete";
            this.bnMyShowsDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bnMyShowsDelete.UseVisualStyleBackColor = true;
            this.bnMyShowsDelete.Click += new System.EventHandler(this.bnMyShowsDelete_Click);
            // 
            // bnMyShowsEdit
            // 
            this.bnMyShowsEdit.ImageKey = "EditInformation.bmp";
            this.bnMyShowsEdit.ImageList = this.imageList1;
            this.bnMyShowsEdit.Location = new System.Drawing.Point(168, 6);
            this.bnMyShowsEdit.Name = "bnMyShowsEdit";
            this.bnMyShowsEdit.Size = new System.Drawing.Size(75, 25);
            this.bnMyShowsEdit.TabIndex = 2;
            this.bnMyShowsEdit.Text = "&Edit";
            this.bnMyShowsEdit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bnMyShowsEdit.UseVisualStyleBackColor = true;
            this.bnMyShowsEdit.Click += new System.EventHandler(this.bnMyShowsEdit_Click);
            // 
            // bnMyShowsAdd
            // 
            this.bnMyShowsAdd.ImageKey = "NewCard.bmp";
            this.bnMyShowsAdd.ImageList = this.imageList1;
            this.bnMyShowsAdd.Location = new System.Drawing.Point(87, 6);
            this.bnMyShowsAdd.Name = "bnMyShowsAdd";
            this.bnMyShowsAdd.Size = new System.Drawing.Size(75, 25);
            this.bnMyShowsAdd.TabIndex = 1;
            this.bnMyShowsAdd.Text = "&Add";
            this.bnMyShowsAdd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bnMyShowsAdd.UseVisualStyleBackColor = true;
            this.bnMyShowsAdd.Click += new System.EventHandler(this.bnMyShowsAdd_Click);
            // 
            // tbAllInOne
            // 
            this.tbAllInOne.Controls.Add(this.cbDeleteFiles);
            this.tbAllInOne.Controls.Add(this.btnActionQuickScan);
            this.tbAllInOne.Controls.Add(this.cbModifyMetadata);
            this.tbAllInOne.Controls.Add(this.cbWriteMetadata);
            this.tbAllInOne.Controls.Add(this.cbSaveImages);
            this.tbAllInOne.Controls.Add(this.cbDownload);
            this.tbAllInOne.Controls.Add(this.cbCopyMove);
            this.tbAllInOne.Controls.Add(this.cbRename);
            this.tbAllInOne.Controls.Add(this.cbAll);
            this.tbAllInOne.Controls.Add(this.label1);
            this.tbAllInOne.Controls.Add(this.bnRemoveSel);
            this.tbAllInOne.Controls.Add(this.bnActionIgnore);
            this.tbAllInOne.Controls.Add(this.bnActionOptions);
            this.tbAllInOne.Controls.Add(this.bnActionWhichSearch);
            this.tbAllInOne.Controls.Add(this.bnActionBTSearch);
            this.tbAllInOne.Controls.Add(this.lvAction);
            this.tbAllInOne.Controls.Add(this.bnActionAction);
            this.tbAllInOne.Controls.Add(this.bnActionRecentCheck);
            this.tbAllInOne.Controls.Add(this.bnActionCheck);
            this.tbAllInOne.ImageKey = "Zoom.bmp";
            this.tbAllInOne.Location = new System.Drawing.Point(4, 23);
            this.tbAllInOne.Name = "tbAllInOne";
            this.tbAllInOne.Padding = new System.Windows.Forms.Padding(3);
            this.tbAllInOne.Size = new System.Drawing.Size(967, 541);
            this.tbAllInOne.TabIndex = 11;
            this.tbAllInOne.Text = "Scan";
            this.tbAllInOne.UseVisualStyleBackColor = true;
            // 
            // cbDeleteFiles
            // 
            this.cbDeleteFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDeleteFiles.AutoSize = true;
            this.cbDeleteFiles.Location = new System.Drawing.Point(514, 515);
            this.cbDeleteFiles.Name = "cbDeleteFiles";
            this.cbDeleteFiles.Size = new System.Drawing.Size(57, 17);
            this.cbDeleteFiles.TabIndex = 12;
            this.cbDeleteFiles.Text = "Delete";
            this.cbDeleteFiles.ThreeState = true;
            this.cbDeleteFiles.UseVisualStyleBackColor = true;
            this.cbDeleteFiles.Click += new System.EventHandler(this.cbDeletes_Click);
            // 
            // btnActionQuickScan
            // 
            this.btnActionQuickScan.ImageKey = "Zoom.bmp";
            this.btnActionQuickScan.ImageList = this.imageList1;
            this.btnActionQuickScan.Location = new System.Drawing.Point(170, 6);
            this.btnActionQuickScan.Name = "btnActionQuickScan";
            this.btnActionQuickScan.Size = new System.Drawing.Size(86, 25);
            this.btnActionQuickScan.TabIndex = 11;
            this.btnActionQuickScan.Text = "&Quick";
            this.btnActionQuickScan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnActionQuickScan.UseVisualStyleBackColor = true;
            this.btnActionQuickScan.Click += new System.EventHandler(this.btnActionQuickScan_Click);
            // 
            // cbModifyMetadata
            // 
            this.cbModifyMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbModifyMetadata.AutoSize = true;
            this.cbModifyMetadata.Location = new System.Drawing.Point(856, 515);
            this.cbModifyMetadata.Name = "cbModifyMetadata";
            this.cbModifyMetadata.Size = new System.Drawing.Size(105, 17);
            this.cbModifyMetadata.TabIndex = 10;
            this.cbModifyMetadata.Text = "Modify Metadata";
            this.cbModifyMetadata.ThreeState = true;
            this.cbModifyMetadata.UseVisualStyleBackColor = true;
            this.cbModifyMetadata.Click += new System.EventHandler(this.cbActionModifyMetaData_Click);
            // 
            // cbWriteMetadata
            // 
            this.cbWriteMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbWriteMetadata.AutoSize = true;
            this.cbWriteMetadata.Location = new System.Drawing.Point(751, 515);
            this.cbWriteMetadata.Name = "cbWriteMetadata";
            this.cbWriteMetadata.Size = new System.Drawing.Size(99, 17);
            this.cbWriteMetadata.TabIndex = 9;
            this.cbWriteMetadata.Text = "Write Metadata";
            this.cbWriteMetadata.ThreeState = true;
            this.cbWriteMetadata.UseVisualStyleBackColor = true;
            this.cbWriteMetadata.Click += new System.EventHandler(this.cbActionNFO_Click);
            // 
            // cbSaveImages
            // 
            this.cbSaveImages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSaveImages.AutoSize = true;
            this.cbSaveImages.Location = new System.Drawing.Point(577, 515);
            this.cbSaveImages.Name = "cbSaveImages";
            this.cbSaveImages.Size = new System.Drawing.Size(88, 17);
            this.cbSaveImages.TabIndex = 9;
            this.cbSaveImages.Text = "Save Images";
            this.cbSaveImages.ThreeState = true;
            this.cbSaveImages.UseVisualStyleBackColor = true;
            this.cbSaveImages.Click += new System.EventHandler(this.cbActionDownloads_Click);
            // 
            // cbDownload
            // 
            this.cbDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDownload.AutoSize = true;
            this.cbDownload.Location = new System.Drawing.Point(671, 515);
            this.cbDownload.Name = "cbDownload";
            this.cbDownload.Size = new System.Drawing.Size(74, 17);
            this.cbDownload.TabIndex = 9;
            this.cbDownload.Text = "Download";
            this.cbDownload.ThreeState = true;
            this.cbDownload.UseVisualStyleBackColor = true;
            this.cbDownload.Click += new System.EventHandler(this.cbActionRSS_Click);
            // 
            // cbCopyMove
            // 
            this.cbCopyMove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbCopyMove.AutoSize = true;
            this.cbCopyMove.Location = new System.Drawing.Point(426, 515);
            this.cbCopyMove.Name = "cbCopyMove";
            this.cbCopyMove.Size = new System.Drawing.Size(82, 17);
            this.cbCopyMove.TabIndex = 9;
            this.cbCopyMove.Text = "Copy/Move";
            this.cbCopyMove.ThreeState = true;
            this.cbCopyMove.UseVisualStyleBackColor = true;
            this.cbCopyMove.Click += new System.EventHandler(this.cbActionCopyMove_Click);
            // 
            // cbRename
            // 
            this.cbRename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbRename.AutoSize = true;
            this.cbRename.Location = new System.Drawing.Point(354, 515);
            this.cbRename.Name = "cbRename";
            this.cbRename.Size = new System.Drawing.Size(66, 17);
            this.cbRename.TabIndex = 9;
            this.cbRename.Text = "Rename";
            this.cbRename.ThreeState = true;
            this.cbRename.UseVisualStyleBackColor = true;
            this.cbRename.Click += new System.EventHandler(this.cbActionRename_Click);
            // 
            // cbAll
            // 
            this.cbAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAll.AutoSize = true;
            this.cbAll.Location = new System.Drawing.Point(311, 515);
            this.cbAll.Name = "cbAll";
            this.cbAll.Size = new System.Drawing.Size(37, 17);
            this.cbAll.TabIndex = 9;
            this.cbAll.Text = "All";
            this.cbAll.ThreeState = true;
            this.cbAll.UseVisualStyleBackColor = true;
            this.cbAll.Click += new System.EventHandler(this.cbActionAllNone_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(264, 516);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Check:";
            // 
            // bnRemoveSel
            // 
            this.bnRemoveSel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnRemoveSel.Location = new System.Drawing.Point(297, 512);
            this.bnRemoveSel.Name = "bnRemoveSel";
            this.bnRemoveSel.Size = new System.Drawing.Size(75, 25);
            this.bnRemoveSel.TabIndex = 5;
            this.bnRemoveSel.Text = "&Remove Sel";
            this.bnRemoveSel.UseVisualStyleBackColor = true;
            this.bnRemoveSel.Click += new System.EventHandler(this.bnRemoveSel_Click);
            // 
            // bnActionIgnore
            // 
            this.bnActionIgnore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnActionIgnore.Location = new System.Drawing.Point(216, 512);
            this.bnActionIgnore.Name = "bnActionIgnore";
            this.bnActionIgnore.Size = new System.Drawing.Size(75, 25);
            this.bnActionIgnore.TabIndex = 5;
            this.bnActionIgnore.Text = "&Ignore Sel";
            this.bnActionIgnore.UseVisualStyleBackColor = true;
            this.bnActionIgnore.Click += new System.EventHandler(this.cbActionIgnore_Click);
            // 
            // bnActionOptions
            // 
            this.bnActionOptions.ImageKey = "Options.bmp";
            this.bnActionOptions.ImageList = this.imageList1;
            this.bnActionOptions.Location = new System.Drawing.Point(262, 6);
            this.bnActionOptions.Name = "bnActionOptions";
            this.bnActionOptions.Size = new System.Drawing.Size(100, 25);
            this.bnActionOptions.TabIndex = 8;
            this.bnActionOptions.Text = "&Preferences...";
            this.bnActionOptions.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bnActionOptions.UseVisualStyleBackColor = true;
            this.bnActionOptions.Click += new System.EventHandler(this.bnActionOptions_Click);
            // 
            // bnActionWhichSearch
            // 
            this.bnActionWhichSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnActionWhichSearch.Image = ((System.Drawing.Image)(resources.GetObject("bnActionWhichSearch.Image")));
            this.bnActionWhichSearch.Location = new System.Drawing.Point(189, 512);
            this.bnActionWhichSearch.Name = "bnActionWhichSearch";
            this.bnActionWhichSearch.Size = new System.Drawing.Size(19, 25);
            this.bnActionWhichSearch.TabIndex = 4;
            this.bnActionWhichSearch.UseVisualStyleBackColor = true;
            this.bnActionWhichSearch.Click += new System.EventHandler(this.bnActionWhichSearch_Click);
            // 
            // bnActionBTSearch
            // 
            this.bnActionBTSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnActionBTSearch.ImageKey = "SearchWeb.bmp";
            this.bnActionBTSearch.ImageList = this.imageList1;
            this.bnActionBTSearch.Location = new System.Drawing.Point(105, 512);
            this.bnActionBTSearch.Name = "bnActionBTSearch";
            this.bnActionBTSearch.Size = new System.Drawing.Size(85, 25);
            this.bnActionBTSearch.TabIndex = 3;
            this.bnActionBTSearch.Text = "BT S&earch";
            this.bnActionBTSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bnActionBTSearch.UseVisualStyleBackColor = true;
            this.bnActionBTSearch.Click += new System.EventHandler(this.bnActionBTSearch_Click);
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
            listViewGroup15.Header = "Missing";
            listViewGroup15.Name = "lvgActionMissing";
            listViewGroup16.Header = "Rename";
            listViewGroup16.Name = "lvgActionRename";
            listViewGroup17.Header = "Copy";
            listViewGroup17.Name = "lvgActionCopy";
            listViewGroup18.Header = "Move";
            listViewGroup18.Name = "lvgActionMove";
            listViewGroup19.Header = "Remove";
            listViewGroup19.Name = "lvgActionDelete";
            listViewGroup20.Header = "Download RSS";
            listViewGroup20.Name = "lvgActionDownloadRSS";
            listViewGroup21.Header = "Download";
            listViewGroup21.Name = "lvgActionDownload";
            listViewGroup22.Header = "Media Center Metadata";
            listViewGroup22.Name = "lvgActionMeta";
            listViewGroup23.Header = "Update File Metadata";
            listViewGroup23.Name = "lvgUpdateFileDates";
            listViewGroup24.Header = "Downloading";
            listViewGroup24.Name = "lvgDownloading";
            this.lvAction.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup15,
            listViewGroup16,
            listViewGroup17,
            listViewGroup18,
            listViewGroup19,
            listViewGroup20,
            listViewGroup21,
            listViewGroup22,
            listViewGroup23,
            listViewGroup24});
            this.lvAction.HideSelection = false;
            this.lvAction.Location = new System.Drawing.Point(0, 35);
            this.lvAction.Name = "lvAction";
            this.lvAction.ShowItemToolTips = true;
            this.lvAction.Size = new System.Drawing.Size(964, 470);
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
            // bnActionAction
            // 
            this.bnActionAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnActionAction.ImageKey = "FormRun.bmp";
            this.bnActionAction.ImageList = this.imageList1;
            this.bnActionAction.Location = new System.Drawing.Point(6, 511);
            this.bnActionAction.Name = "bnActionAction";
            this.bnActionAction.Size = new System.Drawing.Size(93, 25);
            this.bnActionAction.TabIndex = 0;
            this.bnActionAction.Text = "&Do Checked";
            this.bnActionAction.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bnActionAction.Click += new System.EventHandler(this.bnActionAction_Click);
            // 
            // bnActionRecentCheck
            // 
            this.bnActionRecentCheck.ImageKey = "Zoom.bmp";
            this.bnActionRecentCheck.ImageList = this.imageList1;
            this.bnActionRecentCheck.Location = new System.Drawing.Point(89, 6);
            this.bnActionRecentCheck.Name = "bnActionRecentCheck";
            this.bnActionRecentCheck.Size = new System.Drawing.Size(75, 25);
            this.bnActionRecentCheck.TabIndex = 0;
            this.bnActionRecentCheck.Text = "R&ecent";
            this.bnActionRecentCheck.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bnActionRecentCheck.UseVisualStyleBackColor = true;
            this.bnActionRecentCheck.Click += new System.EventHandler(this.bnActionRecentCheck_Click);
            // 
            // bnActionCheck
            // 
            this.bnActionCheck.ImageKey = "Zoom.bmp";
            this.bnActionCheck.ImageList = this.imageList1;
            this.bnActionCheck.Location = new System.Drawing.Point(8, 6);
            this.bnActionCheck.Name = "bnActionCheck";
            this.bnActionCheck.Size = new System.Drawing.Size(75, 25);
            this.bnActionCheck.TabIndex = 0;
            this.bnActionCheck.Text = "F&ull";
            this.bnActionCheck.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bnActionCheck.UseVisualStyleBackColor = true;
            this.bnActionCheck.Click += new System.EventHandler(this.bnActionCheck_Click);
            // 
            // tbWTW
            // 
            this.tbWTW.Controls.Add(this.bnWTWChooseSite);
            this.tbWTW.Controls.Add(this.bnWTWBTSearch);
            this.tbWTW.Controls.Add(this.bnWhenToWatchCheck);
            this.tbWTW.Controls.Add(this.txtWhenToWatchSynopsis);
            this.tbWTW.Controls.Add(this.calCalendar);
            this.tbWTW.Controls.Add(this.lvWhenToWatch);
            this.tbWTW.ImageKey = "Calendar_schedule.bmp";
            this.tbWTW.Location = new System.Drawing.Point(4, 23);
            this.tbWTW.Name = "tbWTW";
            this.tbWTW.Size = new System.Drawing.Size(967, 541);
            this.tbWTW.TabIndex = 4;
            this.tbWTW.Text = "When to watch";
            this.tbWTW.UseVisualStyleBackColor = true;
            // 
            // bnWTWChooseSite
            // 
            this.bnWTWChooseSite.Image = ((System.Drawing.Image)(resources.GetObject("bnWTWChooseSite.Image")));
            this.bnWTWChooseSite.Location = new System.Drawing.Point(175, 6);
            this.bnWTWChooseSite.Name = "bnWTWChooseSite";
            this.bnWTWChooseSite.Size = new System.Drawing.Size(19, 25);
            this.bnWTWChooseSite.TabIndex = 2;
            this.bnWTWChooseSite.UseVisualStyleBackColor = true;
            this.bnWTWChooseSite.Click += new System.EventHandler(this.bnWTWChooseSite_Click);
            // 
            // bnWTWBTSearch
            // 
            this.bnWTWBTSearch.ImageKey = "SearchWeb.bmp";
            this.bnWTWBTSearch.ImageList = this.imageList1;
            this.bnWTWBTSearch.Location = new System.Drawing.Point(89, 6);
            this.bnWTWBTSearch.Name = "bnWTWBTSearch";
            this.bnWTWBTSearch.Size = new System.Drawing.Size(87, 25);
            this.bnWTWBTSearch.TabIndex = 1;
            this.bnWTWBTSearch.Text = "BT &Search";
            this.bnWTWBTSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bnWTWBTSearch.UseVisualStyleBackColor = true;
            this.bnWTWBTSearch.Click += new System.EventHandler(this.bnWTWBTSearch_Click);
            // 
            // bnWhenToWatchCheck
            // 
            this.bnWhenToWatchCheck.ImageKey = "Refresh.bmp";
            this.bnWhenToWatchCheck.ImageList = this.imageList1;
            this.bnWhenToWatchCheck.Location = new System.Drawing.Point(8, 6);
            this.bnWhenToWatchCheck.Name = "bnWhenToWatchCheck";
            this.bnWhenToWatchCheck.Size = new System.Drawing.Size(75, 25);
            this.bnWhenToWatchCheck.TabIndex = 0;
            this.bnWhenToWatchCheck.Text = "&Refresh";
            this.bnWhenToWatchCheck.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bnWhenToWatchCheck.UseVisualStyleBackColor = true;
            this.bnWhenToWatchCheck.Click += new System.EventHandler(this.bnWhenToWatchCheck_Click);
            // 
            // txtWhenToWatchSynopsis
            // 
            this.txtWhenToWatchSynopsis.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWhenToWatchSynopsis.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWhenToWatchSynopsis.Location = new System.Drawing.Point(0, 371);
            this.txtWhenToWatchSynopsis.Multiline = true;
            this.txtWhenToWatchSynopsis.Name = "txtWhenToWatchSynopsis";
            this.txtWhenToWatchSynopsis.ReadOnly = true;
            this.txtWhenToWatchSynopsis.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtWhenToWatchSynopsis.Size = new System.Drawing.Size(720, 161);
            this.txtWhenToWatchSynopsis.TabIndex = 4;
            // 
            // calCalendar
            // 
            this.calCalendar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.calCalendar.Location = new System.Drawing.Point(732, 370);
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
            this.lvWhenToWatch.Location = new System.Drawing.Point(0, 35);
            this.lvWhenToWatch.Name = "lvWhenToWatch";
            this.lvWhenToWatch.ShowItemToolTips = true;
            this.lvWhenToWatch.Size = new System.Drawing.Size(959, 324);
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
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 594);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(963, 19);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // pbProgressBarx
            // 
            this.pbProgressBarx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgressBarx.Location = new System.Drawing.Point(821, 3);
            this.pbProgressBarx.Name = "pbProgressBarx";
            this.pbProgressBarx.Size = new System.Drawing.Size(139, 13);
            this.pbProgressBarx.Step = 1;
            this.pbProgressBarx.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbProgressBarx.TabIndex = 0;
            // 
            // txtDLStatusLabel
            // 
            this.txtDLStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDLStatusLabel.Location = new System.Drawing.Point(436, 6);
            this.txtDLStatusLabel.Name = "txtDLStatusLabel";
            this.txtDLStatusLabel.Size = new System.Drawing.Size(379, 13);
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
            this.tsNextShowTxt.Size = new System.Drawing.Size(427, 13);
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
            // menuSearchSites
            // 
            this.menuSearchSites.Name = "menuSearchSites";
            this.menuSearchSites.ShowImageMargin = false;
            this.menuSearchSites.Size = new System.Drawing.Size(36, 4);
            this.menuSearchSites.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuSearchSites_ItemClicked);
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
            this.btnUpdateAvailable.Location = new System.Drawing.Point(852, 0);
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
            this.ClientSize = new System.Drawing.Size(975, 617);
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
            this.tbWTW.ResumeLayout(false);
            this.tbWTW.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Button bnMyShowsCollapse;
        private System.Windows.Forms.TabPage tbAllInOne;
        private System.Windows.Forms.Button bnActionCheck;

        private System.Windows.Forms.Button bnActionAction;
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
        private System.Windows.Forms.Button bnActionWhichSearch;
        private System.Windows.Forms.Button bnActionBTSearch;
        private System.Windows.Forms.Button bnActionIgnore;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bnActionOptions;
        private System.Windows.Forms.Button bnRemoveSel;
        private System.Windows.Forms.ToolStripMenuItem ignoreListToolStripMenuItem;
        private System.Windows.Forms.CheckBox cbWriteMetadata;
        private System.Windows.Forms.CheckBox cbSaveImages;
        private System.Windows.Forms.CheckBox cbDownload;
        private System.Windows.Forms.CheckBox cbCopyMove;
        private System.Windows.Forms.CheckBox cbRename;
        private System.Windows.Forms.CheckBox cbAll;

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
        private System.Windows.Forms.Button bnWhenToWatchCheck;
        private System.Windows.Forms.ContextMenuStrip menuSearchSites;
        private System.Windows.Forms.Timer refreshWTWTimer;
        private System.Windows.Forms.Button bnWTWChooseSite;
        private System.Windows.Forms.Button bnWTWBTSearch;
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
        private System.Windows.Forms.Button bnMyShowsAdd;
        private System.Windows.Forms.TreeView MyShowTree;
        private System.Windows.Forms.Button bnMyShowsRefresh;
        private System.Windows.Forms.Button bnMyShowsDelete;
        private System.Windows.Forms.Button bnMyShowsEdit;
        private System.Windows.Forms.ToolStripMenuItem quickstartGuideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filenameTemplateEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchEnginesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filenameProcessorsToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;

        private System.Windows.Forms.Timer tmrShowUpcomingPopup;
        private System.Windows.Forms.ToolStripMenuItem actorsToolStripMenuItem;
        private System.Windows.Forms.Timer quickTimer;
        private System.Windows.Forms.CheckBox cbModifyMetadata;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button bnHideHTMLPanel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button bnActionRecentCheck;
        private System.Windows.Forms.Button btnActionQuickScan;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnFilter;
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
        private CheckBox cbDeleteFiles;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ColumnHeader columnHeader1;
        private ToolStripMenuItem quickRenameToolStripMenuItem;
        private ToolStripMenuItem accuracyCheckLogToolStripMenuItem;
    }
}
