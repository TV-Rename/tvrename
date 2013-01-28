//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//


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
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Download RSS", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("Download", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup7 = new System.Windows.Forms.ListViewGroup("Media Center Metadata", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup8 = new System.Windows.Forms.ListViewGroup("Downloading", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup9 = new System.Windows.Forms.ListViewGroup("Recently Aired", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup10 = new System.Windows.Forms.ListViewGroup("Next 7 Days", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup11 = new System.Windows.Forms.ListViewGroup("Later", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup12 = new System.Windows.Forms.ListViewGroup("Future Episodes", System.Windows.Forms.HorizontalAlignment.Left);
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
            this.backgroundDownloadNowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.folderMonitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.torrentMatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uTorrentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bugReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buyMeADrinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visitWebsiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickstartGuideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statisticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tbMyShows = new System.Windows.Forms.TabPage();
            this.bnHideHTMLPanel = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.MyShowTree = new System.Windows.Forms.TreeView();
            this.epGuideHTML = new System.Windows.Forms.WebBrowser();
            this.bnMyShowsCollapse = new System.Windows.Forms.Button();
            this.bnMyShowsVisitTVDB = new System.Windows.Forms.Button();
            this.bnMyShowsOpenFolder = new System.Windows.Forms.Button();
            this.bnMyShowsRefresh = new System.Windows.Forms.Button();
            this.bnMyShowsDelete = new System.Windows.Forms.Button();
            this.bnMyShowsEdit = new System.Windows.Forms.Button();
            this.bnMyShowsAdd = new System.Windows.Forms.Button();
            this.tbAllInOne = new System.Windows.Forms.TabPage();
            this.cbMeta = new System.Windows.Forms.CheckBox();
            this.cbNFO = new System.Windows.Forms.CheckBox();
            this.cbDownload = new System.Windows.Forms.CheckBox();
            this.cbRSS = new System.Windows.Forms.CheckBox();
            this.cbCopyMove = new System.Windows.Forms.CheckBox();
            this.cbRename = new System.Windows.Forms.CheckBox();
            this.cbAll = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bnRemoveSel = new System.Windows.Forms.Button();
            this.bnActionIgnore = new System.Windows.Forms.Button();
            this.bnActionOptions = new System.Windows.Forms.Button();
            this.bnActionWhichSearch = new System.Windows.Forms.Button();
            this.bnActionBTSearch = new System.Windows.Forms.Button();
            this.lvAction = new TVRename.MyListView();
            this.columnHeader48 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader49 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader51 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader52 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader53 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader54 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader55 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader56 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader58 = new System.Windows.Forms.ColumnHeader();
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
            this.lvWhenToWatch = new TVRename.ListViewFlickerFree();
            this.columnHeader29 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader30 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader31 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader32 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader36 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader33 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader34 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader35 = new System.Windows.Forms.ColumnHeader();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.pbProgressBarx = new System.Windows.Forms.ProgressBar();
            this.txtDLStatusLabel = new System.Windows.Forms.Label();
            this.tsNextShowTxt = new System.Windows.Forms.Label();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader25 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader26 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader27 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader28 = new System.Windows.Forms.ColumnHeader();
            this.openFile = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.menuSearchSites = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshWTWTimer = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.showRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.folderRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.statusTimer = new System.Windows.Forms.Timer(this.components);
            this.BGDownloadTimer = new System.Windows.Forms.Timer(this.components);
            this.saveFile = new System.Windows.Forms.SaveFileDialog();
            this.tmrShowUpcomingPopup = new System.Windows.Forms.Timer(this.components);
            this.quickTimer = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tbMyShows.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
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
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(931, 24);
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
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.exportToolStripMenuItem.Text = "&Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::TVRename.Properties.Resources.saveHS;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(144, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
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
            this.backgroundDownloadToolStripMenuItem.ToolTipText = "Turn this on to let TVRename automatically download thetvdb.com data in the backr" +
                "ground";
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
            this.backgroundDownloadNowToolStripMenuItem,
            this.toolStripSeparator3,
            this.folderMonitorToolStripMenuItem,
            this.actorsToolStripMenuItem,
            this.toolStripSeparator4,
            this.torrentMatchToolStripMenuItem,
            this.uTorrentToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // flushCacheToolStripMenuItem
            // 
            this.flushCacheToolStripMenuItem.Name = "flushCacheToolStripMenuItem";
            this.flushCacheToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.flushCacheToolStripMenuItem.Text = "&Force Refesh All";
            this.flushCacheToolStripMenuItem.Click += new System.EventHandler(this.flushCacheToolStripMenuItem_Click);
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
            this.folderMonitorToolStripMenuItem.Text = "Folder &Monitor";
            this.folderMonitorToolStripMenuItem.Click += new System.EventHandler(this.folderMonitorToolStripMenuItem_Click);
            // 
            // actorsToolStripMenuItem
            // 
            this.actorsToolStripMenuItem.Image = global::TVRename.Properties.Resources.TableHS;
            this.actorsToolStripMenuItem.Name = "actorsToolStripMenuItem";
            this.actorsToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.actorsToolStripMenuItem.Text = "&Actors Grid";
            this.actorsToolStripMenuItem.Click += new System.EventHandler(this.actorsToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(261, 6);
            // 
            // torrentMatchToolStripMenuItem
            // 
            this.torrentMatchToolStripMenuItem.Name = "torrentMatchToolStripMenuItem";
            this.torrentMatchToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.torrentMatchToolStripMenuItem.Text = "&Torrent Match";
            this.torrentMatchToolStripMenuItem.Click += new System.EventHandler(this.torrentMatchToolStripMenuItem_Click);
            // 
            // uTorrentToolStripMenuItem
            // 
            this.uTorrentToolStripMenuItem.Name = "uTorrentToolStripMenuItem";
            this.uTorrentToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.uTorrentToolStripMenuItem.Text = "&uTorrent Save To";
            this.uTorrentToolStripMenuItem.Click += new System.EventHandler(this.uTorrentToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bugReportToolStripMenuItem,
            this.buyMeADrinkToolStripMenuItem,
            this.visitWebsiteToolStripMenuItem,
            this.quickstartGuideToolStripMenuItem,
            this.statisticsToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // bugReportToolStripMenuItem
            // 
            this.bugReportToolStripMenuItem.Name = "bugReportToolStripMenuItem";
            this.bugReportToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.bugReportToolStripMenuItem.Text = "Bug &Report";
            this.bugReportToolStripMenuItem.Click += new System.EventHandler(this.bugReportToolStripMenuItem_Click);
            // 
            // buyMeADrinkToolStripMenuItem
            // 
            this.buyMeADrinkToolStripMenuItem.Name = "buyMeADrinkToolStripMenuItem";
            this.buyMeADrinkToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.buyMeADrinkToolStripMenuItem.Text = "&Buy Me A Drink";
            this.buyMeADrinkToolStripMenuItem.Click += new System.EventHandler(this.buyMeADrinkToolStripMenuItem_Click);
            // 
            // visitWebsiteToolStripMenuItem
            // 
            this.visitWebsiteToolStripMenuItem.Image = global::TVRename.Properties.Resources.Web;
            this.visitWebsiteToolStripMenuItem.Name = "visitWebsiteToolStripMenuItem";
            this.visitWebsiteToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.visitWebsiteToolStripMenuItem.Text = "&Visit Website";
            this.visitWebsiteToolStripMenuItem.Click += new System.EventHandler(this.visitWebsiteToolStripMenuItem_Click);
            // 
            // quickstartGuideToolStripMenuItem
            // 
            this.quickstartGuideToolStripMenuItem.Image = global::TVRename.Properties.Resources.Help;
            this.quickstartGuideToolStripMenuItem.Name = "quickstartGuideToolStripMenuItem";
            this.quickstartGuideToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.quickstartGuideToolStripMenuItem.Text = "&Quickstart Guide";
            this.quickstartGuideToolStripMenuItem.Click += new System.EventHandler(this.quickstartGuideToolStripMenuItem_Click);
            // 
            // statisticsToolStripMenuItem
            // 
            this.statisticsToolStripMenuItem.Image = global::TVRename.Properties.Resources.graphhs;
            this.statisticsToolStripMenuItem.Name = "statisticsToolStripMenuItem";
            this.statisticsToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.statisticsToolStripMenuItem.Text = "&Statistics";
            this.statisticsToolStripMenuItem.Click += new System.EventHandler(this.statisticsToolStripMenuItem_Click);
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
            this.tabControl1.Size = new System.Drawing.Size(931, 533);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.DoubleClick += new System.EventHandler(this.tabControl1_DoubleClick);
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tbMyShows
            // 
            this.tbMyShows.Controls.Add(this.bnHideHTMLPanel);
            this.tbMyShows.Controls.Add(this.splitContainer1);
            this.tbMyShows.Controls.Add(this.bnMyShowsCollapse);
            this.tbMyShows.Controls.Add(this.bnMyShowsVisitTVDB);
            this.tbMyShows.Controls.Add(this.bnMyShowsOpenFolder);
            this.tbMyShows.Controls.Add(this.bnMyShowsRefresh);
            this.tbMyShows.Controls.Add(this.bnMyShowsDelete);
            this.tbMyShows.Controls.Add(this.bnMyShowsEdit);
            this.tbMyShows.Controls.Add(this.bnMyShowsAdd);
            this.tbMyShows.ImageKey = "TVOff.bmp";
            this.tbMyShows.Location = new System.Drawing.Point(4, 23);
            this.tbMyShows.Name = "tbMyShows";
            this.tbMyShows.Padding = new System.Windows.Forms.Padding(3);
            this.tbMyShows.Size = new System.Drawing.Size(923, 506);
            this.tbMyShows.TabIndex = 9;
            this.tbMyShows.Text = "My Shows";
            this.tbMyShows.UseVisualStyleBackColor = true;
            // 
            // bnHideHTMLPanel
            // 
            this.bnHideHTMLPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnHideHTMLPanel.ImageKey = "FillRight.bmp";
            this.bnHideHTMLPanel.ImageList = this.imageList1;
            this.bnHideHTMLPanel.Location = new System.Drawing.Point(282, 479);
            this.bnHideHTMLPanel.Name = "bnHideHTMLPanel";
            this.bnHideHTMLPanel.Size = new System.Drawing.Size(25, 25);
            this.bnHideHTMLPanel.TabIndex = 9;
            this.bnHideHTMLPanel.UseVisualStyleBackColor = true;
            this.bnHideHTMLPanel.Click += new System.EventHandler(this.bnHideHTMLPanel_Click);
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
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.MyShowTree);
            this.splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.epGuideHTML);
            this.splitContainer1.Panel2MinSize = 100;
            this.splitContainer1.Size = new System.Drawing.Size(920, 470);
            this.splitContainer1.SplitterDistance = 280;
            this.splitContainer1.TabIndex = 8;
            // 
            // MyShowTree
            // 
            this.MyShowTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.MyShowTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MyShowTree.HideSelection = false;
            this.MyShowTree.Location = new System.Drawing.Point(0, 0);
            this.MyShowTree.Name = "MyShowTree";
            this.MyShowTree.Size = new System.Drawing.Size(276, 466);
            this.MyShowTree.TabIndex = 0;
            this.MyShowTree.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MyShowTree_MouseClick);
            this.MyShowTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.MyShowTree_AfterSelect);
            // 
            // epGuideHTML
            // 
            this.epGuideHTML.AllowWebBrowserDrop = false;
            this.epGuideHTML.Dock = System.Windows.Forms.DockStyle.Fill;
            this.epGuideHTML.Location = new System.Drawing.Point(0, 0);
            this.epGuideHTML.MinimumSize = new System.Drawing.Size(20, 20);
            this.epGuideHTML.Name = "epGuideHTML";
            this.epGuideHTML.Size = new System.Drawing.Size(632, 466);
            this.epGuideHTML.TabIndex = 6;
            this.epGuideHTML.WebBrowserShortcutsEnabled = false;
            this.epGuideHTML.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.epGuideHTML_Navigating);
            // 
            // bnMyShowsCollapse
            // 
            this.bnMyShowsCollapse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnMyShowsCollapse.ImageKey = "Control_TreeView.bmp";
            this.bnMyShowsCollapse.ImageList = this.imageList1;
            this.bnMyShowsCollapse.Location = new System.Drawing.Point(251, 479);
            this.bnMyShowsCollapse.Name = "bnMyShowsCollapse";
            this.bnMyShowsCollapse.Size = new System.Drawing.Size(25, 25);
            this.bnMyShowsCollapse.TabIndex = 4;
            this.bnMyShowsCollapse.UseVisualStyleBackColor = true;
            this.bnMyShowsCollapse.Click += new System.EventHandler(this.bnMyShowsCollapse_Click);
            // 
            // bnMyShowsVisitTVDB
            // 
            this.bnMyShowsVisitTVDB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnMyShowsVisitTVDB.ImageKey = "PublishToWeb.bmp";
            this.bnMyShowsVisitTVDB.ImageList = this.imageList1;
            this.bnMyShowsVisitTVDB.Location = new System.Drawing.Point(506, 479);
            this.bnMyShowsVisitTVDB.Name = "bnMyShowsVisitTVDB";
            this.bnMyShowsVisitTVDB.Size = new System.Drawing.Size(85, 25);
            this.bnMyShowsVisitTVDB.TabIndex = 7;
            this.bnMyShowsVisitTVDB.Text = "&Visit TVDB";
            this.bnMyShowsVisitTVDB.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bnMyShowsVisitTVDB.UseVisualStyleBackColor = true;
            this.bnMyShowsVisitTVDB.Click += new System.EventHandler(this.bnMyShowsVisitTVDB_Click);
            // 
            // bnMyShowsOpenFolder
            // 
            this.bnMyShowsOpenFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnMyShowsOpenFolder.ImageKey = "OpenFolder.bmp";
            this.bnMyShowsOpenFolder.ImageList = this.imageList1;
            this.bnMyShowsOpenFolder.Location = new System.Drawing.Point(425, 479);
            this.bnMyShowsOpenFolder.Name = "bnMyShowsOpenFolder";
            this.bnMyShowsOpenFolder.Size = new System.Drawing.Size(75, 25);
            this.bnMyShowsOpenFolder.TabIndex = 6;
            this.bnMyShowsOpenFolder.Text = "&Open";
            this.bnMyShowsOpenFolder.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bnMyShowsOpenFolder.UseVisualStyleBackColor = true;
            this.bnMyShowsOpenFolder.Click += new System.EventHandler(this.bnMyShowsOpenFolder_Click);
            // 
            // bnMyShowsRefresh
            // 
            this.bnMyShowsRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnMyShowsRefresh.ImageKey = "Refresh.bmp";
            this.bnMyShowsRefresh.ImageList = this.imageList1;
            this.bnMyShowsRefresh.Location = new System.Drawing.Point(331, 479);
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
            this.bnMyShowsDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnMyShowsDelete.ImageKey = "delete.bmp";
            this.bnMyShowsDelete.ImageList = this.imageList1;
            this.bnMyShowsDelete.Location = new System.Drawing.Point(170, 479);
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
            this.bnMyShowsEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnMyShowsEdit.ImageKey = "EditInformation.bmp";
            this.bnMyShowsEdit.ImageList = this.imageList1;
            this.bnMyShowsEdit.Location = new System.Drawing.Point(89, 479);
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
            this.bnMyShowsAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnMyShowsAdd.ImageKey = "NewCard.bmp";
            this.bnMyShowsAdd.ImageList = this.imageList1;
            this.bnMyShowsAdd.Location = new System.Drawing.Point(8, 479);
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
            this.tbAllInOne.Controls.Add(this.cbMeta);
            this.tbAllInOne.Controls.Add(this.cbNFO);
            this.tbAllInOne.Controls.Add(this.cbDownload);
            this.tbAllInOne.Controls.Add(this.cbRSS);
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
            this.tbAllInOne.Size = new System.Drawing.Size(923, 506);
            this.tbAllInOne.TabIndex = 11;
            this.tbAllInOne.Text = "Scan";
            this.tbAllInOne.UseVisualStyleBackColor = true;
            // 
            // cbMeta
            // 
            this.cbMeta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbMeta.AutoSize = true;
            this.cbMeta.Location = new System.Drawing.Point(832, 480);
            this.cbMeta.Name = "cbMeta";
            this.cbMeta.Size = new System.Drawing.Size(85, 17);
            this.cbMeta.TabIndex = 10;
            this.cbMeta.Text = "pyTivo Meta";
            this.cbMeta.ThreeState = true;
            this.cbMeta.UseVisualStyleBackColor = true;
            this.cbMeta.Click += new System.EventHandler(this.cbActionPyTivoMeta_Click);
            // 
            // cbNFO
            // 
            this.cbNFO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbNFO.AutoSize = true;
            this.cbNFO.Location = new System.Drawing.Point(778, 480);
            this.cbNFO.Name = "cbNFO";
            this.cbNFO.Size = new System.Drawing.Size(48, 17);
            this.cbNFO.TabIndex = 9;
            this.cbNFO.Text = "NFO";
            this.cbNFO.ThreeState = true;
            this.cbNFO.UseVisualStyleBackColor = true;
            this.cbNFO.Click += new System.EventHandler(this.cbActionNFO_Click);
            // 
            // cbDownload
            // 
            this.cbDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDownload.AutoSize = true;
            this.cbDownload.Location = new System.Drawing.Point(698, 480);
            this.cbDownload.Name = "cbDownload";
            this.cbDownload.Size = new System.Drawing.Size(74, 17);
            this.cbDownload.TabIndex = 9;
            this.cbDownload.Text = "Download";
            this.cbDownload.ThreeState = true;
            this.cbDownload.UseVisualStyleBackColor = true;
            this.cbDownload.Click += new System.EventHandler(this.cbActionDownloads_Click);
            // 
            // cbRSS
            // 
            this.cbRSS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbRSS.AutoSize = true;
            this.cbRSS.Location = new System.Drawing.Point(644, 480);
            this.cbRSS.Name = "cbRSS";
            this.cbRSS.Size = new System.Drawing.Size(48, 17);
            this.cbRSS.TabIndex = 9;
            this.cbRSS.Text = "RSS";
            this.cbRSS.ThreeState = true;
            this.cbRSS.UseVisualStyleBackColor = true;
            this.cbRSS.Click += new System.EventHandler(this.cbActionRSS_Click);
            // 
            // cbCopyMove
            // 
            this.cbCopyMove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbCopyMove.AutoSize = true;
            this.cbCopyMove.Location = new System.Drawing.Point(556, 480);
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
            this.cbRename.Location = new System.Drawing.Point(484, 480);
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
            this.cbAll.Location = new System.Drawing.Point(441, 480);
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
            this.label1.Location = new System.Drawing.Point(394, 481);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Check:";
            // 
            // bnRemoveSel
            // 
            this.bnRemoveSel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnRemoveSel.Location = new System.Drawing.Point(297, 477);
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
            this.bnActionIgnore.Location = new System.Drawing.Point(216, 477);
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
            this.bnActionOptions.Location = new System.Drawing.Point(170, 6);
            this.bnActionOptions.Name = "bnActionOptions";
            this.bnActionOptions.Size = new System.Drawing.Size(78, 25);
            this.bnActionOptions.TabIndex = 8;
            this.bnActionOptions.Text = "&Options...";
            this.bnActionOptions.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bnActionOptions.UseVisualStyleBackColor = true;
            this.bnActionOptions.Click += new System.EventHandler(this.bnActionOptions_Click);
            // 
            // bnActionWhichSearch
            // 
            this.bnActionWhichSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnActionWhichSearch.Image = ((System.Drawing.Image)(resources.GetObject("bnActionWhichSearch.Image")));
            this.bnActionWhichSearch.Location = new System.Drawing.Point(189, 477);
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
            this.bnActionBTSearch.Location = new System.Drawing.Point(105, 477);
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
            this.columnHeader56,
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
            listViewGroup5.Header = "Download RSS";
            listViewGroup5.Name = "lvgActionDownloadRSS";
            listViewGroup6.Header = "Download";
            listViewGroup6.Name = "lvgActionDownload";
            listViewGroup7.Header = "Media Center Metadata";
            listViewGroup7.Name = "lvgActionMeta";
            listViewGroup8.Header = "Downloading";
            listViewGroup8.Name = "lvgDownloading";
            this.lvAction.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4,
            listViewGroup5,
            listViewGroup6,
            listViewGroup7,
            listViewGroup8});
            this.lvAction.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvAction.HideSelection = false;
            this.lvAction.Location = new System.Drawing.Point(0, 35);
            this.lvAction.Name = "lvAction";
            this.lvAction.ShowItemToolTips = true;
            this.lvAction.Size = new System.Drawing.Size(920, 435);
            this.lvAction.SmallImageList = this.ilIcons;
            this.lvAction.TabIndex = 2;
            this.lvAction.UseCompatibleStateImageBehavior = false;
            this.lvAction.View = System.Windows.Forms.View.Details;
            this.lvAction.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvAction_MouseDoubleClick);
            this.lvAction.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvAction_ItemChecked);
            this.lvAction.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvAction_MouseClick);
            this.lvAction.SelectedIndexChanged += new System.EventHandler(this.lvAction_SelectedIndexChanged);
            this.lvAction.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lvAction_ItemCheck);
            this.lvAction.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lvAction_RetrieveVirtualItem);
            this.lvAction.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvAction_KeyDown);
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
            this.columnHeader54.Text = "Episode/Filename";
            this.columnHeader54.Width = 180;
            // 
            // columnHeader55
            // 
            this.columnHeader55.Text = "Folder/Filename";
            this.columnHeader55.Width = 180;
            // 
            // columnHeader56
            // 
            this.columnHeader56.Text = "Filename";
            this.columnHeader56.Width = 180;
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
            // 
            // bnActionAction
            // 
            this.bnActionAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnActionAction.ImageKey = "FormRun.bmp";
            this.bnActionAction.ImageList = this.imageList1;
            this.bnActionAction.Location = new System.Drawing.Point(6, 476);
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
            this.bnActionCheck.Text = "&Scan";
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
            this.tbWTW.Size = new System.Drawing.Size(923, 506);
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
            this.txtWhenToWatchSynopsis.Location = new System.Drawing.Point(0, 344);
            this.txtWhenToWatchSynopsis.Multiline = true;
            this.txtWhenToWatchSynopsis.Name = "txtWhenToWatchSynopsis";
            this.txtWhenToWatchSynopsis.ReadOnly = true;
            this.txtWhenToWatchSynopsis.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtWhenToWatchSynopsis.Size = new System.Drawing.Size(684, 161);
            this.txtWhenToWatchSynopsis.TabIndex = 4;
            // 
            // calCalendar
            // 
            this.calCalendar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.calCalendar.FirstDayOfWeek = System.Windows.Forms.Day.Sunday;
            this.calCalendar.Location = new System.Drawing.Point(696, 344);
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
            this.columnHeader35});
            this.lvWhenToWatch.FullRowSelect = true;
            listViewGroup9.Header = "Recently Aired";
            listViewGroup9.Name = "justPassed";
            listViewGroup10.Header = "Next 7 Days";
            listViewGroup10.Name = "next7days";
            listViewGroup10.Tag = "1";
            listViewGroup11.Header = "Later";
            listViewGroup11.Name = "later";
            listViewGroup11.Tag = "2";
            listViewGroup12.Header = "Future Episodes";
            listViewGroup12.Name = "futureEps";
            this.lvWhenToWatch.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup9,
            listViewGroup10,
            listViewGroup11,
            listViewGroup12});
            this.lvWhenToWatch.HideSelection = false;
            this.lvWhenToWatch.Location = new System.Drawing.Point(0, 35);
            this.lvWhenToWatch.Name = "lvWhenToWatch";
            this.lvWhenToWatch.ShowItemToolTips = true;
            this.lvWhenToWatch.Size = new System.Drawing.Size(923, 297);
            this.lvWhenToWatch.SmallImageList = this.ilIcons;
            this.lvWhenToWatch.TabIndex = 3;
            this.lvWhenToWatch.UseCompatibleStateImageBehavior = false;
            this.lvWhenToWatch.View = System.Windows.Forms.View.Details;
            this.lvWhenToWatch.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvWhenToWatch_MouseClick);
            this.lvWhenToWatch.SelectedIndexChanged += new System.EventHandler(this.lvWhenToWatch_Click);
            this.lvWhenToWatch.DoubleClick += new System.EventHandler(this.lvWhenToWatch_DoubleClick);
            this.lvWhenToWatch.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvWhenToWatch_ColumnClick);
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
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 559);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(919, 19);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // pbProgressBarx
            // 
            this.pbProgressBarx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgressBarx.Location = new System.Drawing.Point(783, 3);
            this.pbProgressBarx.Name = "pbProgressBarx";
            this.pbProgressBarx.Size = new System.Drawing.Size(133, 13);
            this.pbProgressBarx.Step = 1;
            this.pbProgressBarx.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbProgressBarx.TabIndex = 0;
            // 
            // txtDLStatusLabel
            // 
            this.txtDLStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDLStatusLabel.Location = new System.Drawing.Point(416, 6);
            this.txtDLStatusLabel.Name = "txtDLStatusLabel";
            this.txtDLStatusLabel.Size = new System.Drawing.Size(361, 13);
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
            this.tsNextShowTxt.Size = new System.Drawing.Size(407, 13);
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
            // folderRightClickMenu
            // 
            this.folderRightClickMenu.Name = "folderRightClickMenu";
            this.folderRightClickMenu.ShowImageMargin = false;
            this.folderRightClickMenu.Size = new System.Drawing.Size(36, 4);
            this.folderRightClickMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.folderRightClickMenu_ItemClicked);
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
            // UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(931, 582);
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
            this.Load += new System.EventHandler(this.UI_Load);
            this.SizeChanged += new System.EventHandler(this.UI_SizeChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UI_FormClosing);
            this.LocationChanged += new System.EventHandler(this.UI_LocationChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UI_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tbMyShows.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
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
        private System.Windows.Forms.ColumnHeader columnHeader56;

        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem folderMonitorToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem torrentMatchToolStripMenuItem;

        private System.Windows.Forms.ColumnHeader columnHeader58;
        private System.Windows.Forms.Button bnActionWhichSearch;
        private System.Windows.Forms.Button bnActionBTSearch;
        private System.Windows.Forms.Button bnActionIgnore;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bnActionOptions;
        private System.Windows.Forms.Button bnRemoveSel;
        private System.Windows.Forms.ToolStripMenuItem ignoreListToolStripMenuItem;
        private System.Windows.Forms.CheckBox cbNFO;
        private System.Windows.Forms.CheckBox cbDownload;
        private System.Windows.Forms.CheckBox cbRSS;
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
        private System.Windows.Forms.ContextMenuStrip folderRightClickMenu;
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
        private System.Windows.Forms.ToolStripMenuItem bugReportToolStripMenuItem;
        private System.Windows.Forms.ImageList ilIcons;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFile;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flushCacheToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backgroundDownloadNowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statisticsToolStripMenuItem;
        private System.Windows.Forms.TabPage tbMyShows;
        private System.Windows.Forms.Button bnMyShowsAdd;
        private System.Windows.Forms.TreeView MyShowTree;
        private System.Windows.Forms.WebBrowser epGuideHTML;
        private System.Windows.Forms.Button bnMyShowsRefresh;
        private System.Windows.Forms.Button bnMyShowsDelete;
        private System.Windows.Forms.Button bnMyShowsEdit;
        private System.Windows.Forms.Button bnMyShowsVisitTVDB;
        private System.Windows.Forms.Button bnMyShowsOpenFolder;
        private System.Windows.Forms.ToolStripMenuItem quickstartGuideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filenameTemplateEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchEnginesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filenameProcessorsToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;

        private System.Windows.Forms.Timer tmrShowUpcomingPopup;
        private System.Windows.Forms.ToolStripMenuItem actorsToolStripMenuItem;
        private System.Windows.Forms.Timer quickTimer;
        private System.Windows.Forms.ToolStripMenuItem uTorrentToolStripMenuItem;
        private System.Windows.Forms.CheckBox cbMeta;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button bnHideHTMLPanel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button bnActionRecentCheck;
    }
}