//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//

using System;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.Xml;
using System.Threading;
using System.IO;

namespace TVRename
{
    // right click commands
    public enum RightClickCommands
    {
        kEpisodeGuideForShow = 1,
        kVisitTVDBEpisode,
        kVisitTVDBSeason,
        kVisitTVDBSeries,
        kScanSpecificSeries,
        kWhenToWatchSeries,
        kForceRefreshSeries,
        kBTSearchFor,
        kAIOIgnore,
        kAIOBrowseForFile,
        kAIOAction,
        kAIODelete,
        kAIOIgnoreSeason,
        kWatchBase = 1000,
        kOpenFolderBase = 2000
    }

    /// <summary>
    /// Summary for UI
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public class UI : System.Windows.Forms.Form
    {
        public SetProgressDelegate SetProgress;

        private System.Windows.Forms.Button bnMyShowsCollapse;
        private System.Windows.Forms.TabPage tbAllInOne;
        private System.Windows.Forms.Button bnAIOCheck;


        private MyListView lvAIO;



        private System.Windows.Forms.Button bnAIOAction;
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
        private System.Windows.Forms.Button bnAIOWhichSearch;
        private System.Windows.Forms.Button bnAIOBTSearch;
        private System.Windows.Forms.Button bnAIOIgnore;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bnAIODownloads;

        private System.Windows.Forms.Button bnAIORSS;

        private System.Windows.Forms.Button bnAIOCopyMove;

        private System.Windows.Forms.Button bnAIORename;










        private System.Windows.Forms.Button bnAIOAllNone;
        private System.Windows.Forms.Button bnAIONFO;
        private System.Windows.Forms.Button bnAIOOptions;
        private System.Windows.Forms.Button bnRemoveSel;
        private System.Windows.Forms.ToolStripMenuItem ignoreListToolStripMenuItem;




        private static bool ExperimentalFeatures;


        public static bool IncludeExperimentalStuff()
        {
            return ExperimentalFeatures;
        }

        protected TVDoc mDoc;
        protected System.Drawing.Size mLastNonMaximizedSize;
        protected Point mLastNonMaximizedLocation;
        protected int mInternalChange;
        protected int Busy;

        protected ProcessedEpisode mLastEpClicked;
        protected Season mLastSeasonClicked;
        protected AIOItem mLastAIOClicked;
        protected ShowItem mLastShowClicked;
        protected StringList mFoldersToOpen;
        protected System.Collections.Generic.List<System.IO.FileInfo> mLastFL;
        protected string mLastFolderClicked;

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
        private System.Windows.Forms.ListView lvWhenToWatch;
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
        private System.ComponentModel.IContainer components;
        /// <summary>
        /// Required designer variable.
        /// </summary>


        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        public void InitializeComponent()
        {
            this.components = (new System.ComponentModel.Container());
            System.ComponentModel.ComponentResourceManager resources = (new System.ComponentModel.ComponentResourceManager(typeof(UI)));
            System.Windows.Forms.ListViewGroup listViewGroup13 = (new System.Windows.Forms.ListViewGroup("Missing", System.Windows.Forms.HorizontalAlignment.Left));
            System.Windows.Forms.ListViewGroup listViewGroup14 = (new System.Windows.Forms.ListViewGroup("Rename", System.Windows.Forms.HorizontalAlignment.Left));
            System.Windows.Forms.ListViewGroup listViewGroup15 = (new System.Windows.Forms.ListViewGroup("Copy", System.Windows.Forms.HorizontalAlignment.Left));
            System.Windows.Forms.ListViewGroup listViewGroup16 = (new System.Windows.Forms.ListViewGroup("Move", System.Windows.Forms.HorizontalAlignment.Left));
            System.Windows.Forms.ListViewGroup listViewGroup17 = (new System.Windows.Forms.ListViewGroup("Download RSS", System.Windows.Forms.HorizontalAlignment.Left));
            System.Windows.Forms.ListViewGroup listViewGroup18 = (new System.Windows.Forms.ListViewGroup("Download", System.Windows.Forms.HorizontalAlignment.Left));
            System.Windows.Forms.ListViewGroup listViewGroup19 = (new System.Windows.Forms.ListViewGroup("NFO File", System.Windows.Forms.HorizontalAlignment.Left));
            System.Windows.Forms.ListViewGroup listViewGroup20 = (new System.Windows.Forms.ListViewGroup("Downloading In µTorrent", System.Windows.Forms.HorizontalAlignment.Left));
            System.Windows.Forms.ListViewGroup listViewGroup1 = (new System.Windows.Forms.ListViewGroup("Recently Aired", System.Windows.Forms.HorizontalAlignment.Left));
            System.Windows.Forms.ListViewGroup listViewGroup2 = (new System.Windows.Forms.ListViewGroup("Next 7 Days", System.Windows.Forms.HorizontalAlignment.Left));
            System.Windows.Forms.ListViewGroup listViewGroup3 = (new System.Windows.Forms.ListViewGroup("Later", System.Windows.Forms.HorizontalAlignment.Left));
            System.Windows.Forms.ListViewGroup listViewGroup4 = (new System.Windows.Forms.ListViewGroup("Future Episodes", System.Windows.Forms.HorizontalAlignment.Left));
            this.menuStrip1 = (new System.Windows.Forms.MenuStrip());
            this.fileToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.exportToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.saveToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.toolStripSeparator1 = (new System.Windows.Forms.ToolStripSeparator());
            this.exitToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.optionsToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.offlineOperationToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.backgroundDownloadToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.toolStripSeparator2 = (new System.Windows.Forms.ToolStripSeparator());
            this.preferencesToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.filenameTemplateEditorToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.searchEnginesToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.filenameProcessorsToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.toolsToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.flushCacheToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.backgroundDownloadNowToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.toolStripSeparator3 = (new System.Windows.Forms.ToolStripSeparator());
            this.folderMonitorToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.actorsToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.toolStripSeparator4 = (new System.Windows.Forms.ToolStripSeparator());
            this.torrentMatchToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.uTorrentToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.helpToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.bugReportToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.buyMeADrinkToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.visitWebsiteToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.quickstartGuideToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.statisticsToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.tabControl1 = (new System.Windows.Forms.TabControl());
            this.tbMyShows = (new System.Windows.Forms.TabPage());
            this.bnMyShowsCollapse = (new System.Windows.Forms.Button());
            this.bnMyShowsVisitTVDB = (new System.Windows.Forms.Button());
            this.bnMyShowsOpenFolder = (new System.Windows.Forms.Button());
            this.bnMyShowsRefresh = (new System.Windows.Forms.Button());
            this.epGuideHTML = (new System.Windows.Forms.WebBrowser());
            this.MyShowTree = (new System.Windows.Forms.TreeView());
            this.bnMyShowsDelete = (new System.Windows.Forms.Button());
            this.bnMyShowsEdit = (new System.Windows.Forms.Button());
            this.bnMyShowsAdd = (new System.Windows.Forms.Button());
            this.tbAllInOne = (new System.Windows.Forms.TabPage());
            this.bnAIOOptions = (new System.Windows.Forms.Button());
            this.label1 = (new System.Windows.Forms.Label());
            this.bnAIONFO = (new System.Windows.Forms.Button());
            this.bnAIODownloads = (new System.Windows.Forms.Button());
            this.bnAIORSS = (new System.Windows.Forms.Button());
            this.bnAIOCopyMove = (new System.Windows.Forms.Button());
            this.bnAIORename = (new System.Windows.Forms.Button());
            this.bnAIOAllNone = (new System.Windows.Forms.Button());
            this.bnRemoveSel = (new System.Windows.Forms.Button());
            this.bnAIOIgnore = (new System.Windows.Forms.Button());
            this.bnAIOWhichSearch = (new System.Windows.Forms.Button());
            this.bnAIOBTSearch = (new System.Windows.Forms.Button());
            this.lvAIO = (new TVRename.MyListView());
            this.columnHeader48 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader49 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader51 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader52 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader53 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader54 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader55 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader56 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader58 = (new System.Windows.Forms.ColumnHeader());
            this.ilIcons = (new System.Windows.Forms.ImageList(this.components));
            this.bnAIOAction = (new System.Windows.Forms.Button());
            this.bnAIOCheck = (new System.Windows.Forms.Button());
            this.tbWTW = (new System.Windows.Forms.TabPage());
            this.bnWTWChooseSite = (new System.Windows.Forms.Button());
            this.bnWTWBTSearch = (new System.Windows.Forms.Button());
            this.bnWhenToWatchCheck = (new System.Windows.Forms.Button());
            this.txtWhenToWatchSynopsis = (new System.Windows.Forms.TextBox());
            this.calCalendar = (new System.Windows.Forms.MonthCalendar());
            this.lvWhenToWatch = (new System.Windows.Forms.ListView());
            this.columnHeader29 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader30 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader31 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader32 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader36 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader33 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader34 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader35 = (new System.Windows.Forms.ColumnHeader());
            this.tableLayoutPanel2 = (new System.Windows.Forms.TableLayoutPanel());
            this.pbProgressBarx = (new System.Windows.Forms.ProgressBar());
            this.txtDLStatusLabel = (new System.Windows.Forms.Label());
            this.tsNextShowTxt = (new System.Windows.Forms.Label());
            this.columnHeader5 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader6 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader7 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader8 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader25 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader26 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader27 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader28 = (new System.Windows.Forms.ColumnHeader());
            this.openFile = (new System.Windows.Forms.OpenFileDialog());
            this.folderBrowser = (new System.Windows.Forms.FolderBrowserDialog());
            this.menuSearchSites = (new System.Windows.Forms.ContextMenuStrip(this.components));
            this.refreshWTWTimer = (new System.Windows.Forms.Timer(this.components));
            this.notifyIcon1 = (new System.Windows.Forms.NotifyIcon(this.components));
            this.showRightClickMenu = (new System.Windows.Forms.ContextMenuStrip(this.components));
            this.folderRightClickMenu = (new System.Windows.Forms.ContextMenuStrip(this.components));
            this.statusTimer = (new System.Windows.Forms.Timer(this.components));
            this.BGDownloadTimer = (new System.Windows.Forms.Timer(this.components));
            this.saveFile = (new System.Windows.Forms.SaveFileDialog());
            this.tmrShowUpcomingPopup = (new System.Windows.Forms.Timer(this.components));
            this.quickTimer = (new System.Windows.Forms.Timer(this.components));
            this.ignoreListToolStripMenuItem = (new System.Windows.Forms.ToolStripMenuItem());
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tbMyShows.SuspendLayout();
            this.tbAllInOne.SuspendLayout();
            this.tbWTW.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.fileToolStripMenuItem, this.optionsToolStripMenuItem, this.toolsToolStripMenuItem, this.helpToolStripMenuItem });
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(931, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.exportToolStripMenuItem, this.saveToolStripMenuItem, this.toolStripSeparator1, this.exitToolStripMenuItem });
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.ShortcutKeys = (System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4));
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.ShortcutKeys = (System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E));
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exportToolStripMenuItem.Text = "&Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(exportToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = (System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = (System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.offlineOperationToolStripMenuItem, this.backgroundDownloadToolStripMenuItem, this.toolStripSeparator2, this.preferencesToolStripMenuItem, this.ignoreListToolStripMenuItem, this.filenameTemplateEditorToolStripMenuItem, this.searchEnginesToolStripMenuItem, this.filenameProcessorsToolStripMenuItem });
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.ShortcutKeys = (System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O));
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // offlineOperationToolStripMenuItem
            // 
            this.offlineOperationToolStripMenuItem.Name = "offlineOperationToolStripMenuItem";
            this.offlineOperationToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.offlineOperationToolStripMenuItem.Text = "&Offline Operation";
            this.offlineOperationToolStripMenuItem.ToolTipText = "If you turn this on, TVRename will only use data it has locally, without downloading anything.";
            this.offlineOperationToolStripMenuItem.Click += new System.EventHandler(offlineOperationToolStripMenuItem_Click);
            // 
            // backgroundDownloadToolStripMenuItem
            // 
            this.backgroundDownloadToolStripMenuItem.Name = "backgroundDownloadToolStripMenuItem";
            this.backgroundDownloadToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.backgroundDownloadToolStripMenuItem.Text = "Automatic &Background Download";
            this.backgroundDownloadToolStripMenuItem.ToolTipText = "Turn this on to let TVRename automatically download thetvdb.com data in the backrground";
            this.backgroundDownloadToolStripMenuItem.Click += new System.EventHandler(backgroundDownloadToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(229, 6);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.ShortcutKeys = (System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.preferencesToolStripMenuItem.Text = "&Preferences";
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(preferencesToolStripMenuItem_Click);
            // 
            // filenameTemplateEditorToolStripMenuItem
            // 
            this.filenameTemplateEditorToolStripMenuItem.Name = "filenameTemplateEditorToolStripMenuItem";
            this.filenameTemplateEditorToolStripMenuItem.ShortcutKeys = (System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T));
            this.filenameTemplateEditorToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.filenameTemplateEditorToolStripMenuItem.Text = "&Filename Template Editor";
            this.filenameTemplateEditorToolStripMenuItem.Click += new System.EventHandler(filenameTemplateEditorToolStripMenuItem_Click);
            // 
            // searchEnginesToolStripMenuItem
            // 
            this.searchEnginesToolStripMenuItem.Name = "searchEnginesToolStripMenuItem";
            this.searchEnginesToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.searchEnginesToolStripMenuItem.Text = "&Search Engines";
            this.searchEnginesToolStripMenuItem.Click += new System.EventHandler(searchEnginesToolStripMenuItem_Click);
            // 
            // filenameProcessorsToolStripMenuItem
            // 
            this.filenameProcessorsToolStripMenuItem.Name = "filenameProcessorsToolStripMenuItem";
            this.filenameProcessorsToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.filenameProcessorsToolStripMenuItem.Text = "File&name Processors";
            this.filenameProcessorsToolStripMenuItem.Click += new System.EventHandler(filenameProcessorsToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.flushCacheToolStripMenuItem, this.backgroundDownloadNowToolStripMenuItem, this.toolStripSeparator3, this.folderMonitorToolStripMenuItem, this.actorsToolStripMenuItem, this.toolStripSeparator4, this.torrentMatchToolStripMenuItem, this.uTorrentToolStripMenuItem });
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // flushCacheToolStripMenuItem
            // 
            this.flushCacheToolStripMenuItem.Name = "flushCacheToolStripMenuItem";
            this.flushCacheToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.flushCacheToolStripMenuItem.Text = "&Flush Cache";
            this.flushCacheToolStripMenuItem.Click += new System.EventHandler(flushCacheToolStripMenuItem_Click);
            // 
            // backgroundDownloadNowToolStripMenuItem
            // 
            this.backgroundDownloadNowToolStripMenuItem.Name = "backgroundDownloadNowToolStripMenuItem";
            this.backgroundDownloadNowToolStripMenuItem.ShortcutKeys = (System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B));
            this.backgroundDownloadNowToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.backgroundDownloadNowToolStripMenuItem.Text = "&Background Download Now";
            this.backgroundDownloadNowToolStripMenuItem.Click += new System.EventHandler(backgroundDownloadNowToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(239, 6);
            // 
            // folderMonitorToolStripMenuItem
            // 
            this.folderMonitorToolStripMenuItem.Name = "folderMonitorToolStripMenuItem";
            this.folderMonitorToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.folderMonitorToolStripMenuItem.Text = "Folder &Monitor";
            this.folderMonitorToolStripMenuItem.Click += new System.EventHandler(folderMonitorToolStripMenuItem_Click);
            // 
            // actorsToolStripMenuItem
            // 
            this.actorsToolStripMenuItem.Name = "actorsToolStripMenuItem";
            this.actorsToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.actorsToolStripMenuItem.Text = "&Actors Grid";
            this.actorsToolStripMenuItem.Click += new System.EventHandler(actorsToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(239, 6);
            // 
            // torrentMatchToolStripMenuItem
            // 
            this.torrentMatchToolStripMenuItem.Name = "torrentMatchToolStripMenuItem";
            this.torrentMatchToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.torrentMatchToolStripMenuItem.Text = "&Torrent Match";
            this.torrentMatchToolStripMenuItem.Click += new System.EventHandler(torrentMatchToolStripMenuItem_Click);
            // 
            // uTorrentToolStripMenuItem
            // 
            this.uTorrentToolStripMenuItem.Name = "uTorrentToolStripMenuItem";
            this.uTorrentToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.uTorrentToolStripMenuItem.Text = "&uTorrent Save To";
            this.uTorrentToolStripMenuItem.Click += new System.EventHandler(uTorrentToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.bugReportToolStripMenuItem, this.buyMeADrinkToolStripMenuItem, this.visitWebsiteToolStripMenuItem, this.quickstartGuideToolStripMenuItem, this.statisticsToolStripMenuItem });
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // bugReportToolStripMenuItem
            // 
            this.bugReportToolStripMenuItem.Name = "bugReportToolStripMenuItem";
            this.bugReportToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.bugReportToolStripMenuItem.Text = "Bug &Report";
            this.bugReportToolStripMenuItem.Click += new System.EventHandler(bugReportToolStripMenuItem_Click);
            // 
            // buyMeADrinkToolStripMenuItem
            // 
            this.buyMeADrinkToolStripMenuItem.Name = "buyMeADrinkToolStripMenuItem";
            this.buyMeADrinkToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.buyMeADrinkToolStripMenuItem.Text = "&Buy Me A Drink";
            this.buyMeADrinkToolStripMenuItem.Click += new System.EventHandler(buyMeADrinkToolStripMenuItem_Click);
            // 
            // visitWebsiteToolStripMenuItem
            // 
            this.visitWebsiteToolStripMenuItem.Name = "visitWebsiteToolStripMenuItem";
            this.visitWebsiteToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.visitWebsiteToolStripMenuItem.Text = "&Visit Website";
            this.visitWebsiteToolStripMenuItem.Click += new System.EventHandler(visitWebsiteToolStripMenuItem_Click);
            // 
            // quickstartGuideToolStripMenuItem
            // 
            this.quickstartGuideToolStripMenuItem.Name = "quickstartGuideToolStripMenuItem";
            this.quickstartGuideToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.quickstartGuideToolStripMenuItem.Text = "&Quickstart Guide";
            this.quickstartGuideToolStripMenuItem.Click += new System.EventHandler(quickstartGuideToolStripMenuItem_Click);
            // 
            // statisticsToolStripMenuItem
            // 
            this.statisticsToolStripMenuItem.Name = "statisticsToolStripMenuItem";
            this.statisticsToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.statisticsToolStripMenuItem.Text = "&Statistics";
            this.statisticsToolStripMenuItem.Click += new System.EventHandler(statisticsToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            this.tabControl1.Controls.Add(this.tbMyShows);
            this.tabControl1.Controls.Add(this.tbAllInOne);
            this.tabControl1.Controls.Add(this.tbWTW);
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(931, 533);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.DoubleClick += new System.EventHandler(tabControl1_DoubleClick);
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(tabControl1_SelectedIndexChanged);
            // 
            // tbMyShows
            // 
            this.tbMyShows.Controls.Add(this.bnMyShowsCollapse);
            this.tbMyShows.Controls.Add(this.bnMyShowsVisitTVDB);
            this.tbMyShows.Controls.Add(this.bnMyShowsOpenFolder);
            this.tbMyShows.Controls.Add(this.bnMyShowsRefresh);
            this.tbMyShows.Controls.Add(this.epGuideHTML);
            this.tbMyShows.Controls.Add(this.MyShowTree);
            this.tbMyShows.Controls.Add(this.bnMyShowsDelete);
            this.tbMyShows.Controls.Add(this.bnMyShowsEdit);
            this.tbMyShows.Controls.Add(this.bnMyShowsAdd);
            this.tbMyShows.Location = new System.Drawing.Point(4, 22);
            this.tbMyShows.Name = "tbMyShows";
            this.tbMyShows.Padding = new System.Windows.Forms.Padding(3);
            this.tbMyShows.Size = new System.Drawing.Size(923, 507);
            this.tbMyShows.TabIndex = 9;
            this.tbMyShows.Text = "My Shows";
            this.tbMyShows.UseVisualStyleBackColor = true;
            // 
            // bnMyShowsCollapse
            // 
            this.bnMyShowsCollapse.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnMyShowsCollapse.Location = new System.Drawing.Point(251, 480);
            this.bnMyShowsCollapse.Name = "bnMyShowsCollapse";
            this.bnMyShowsCollapse.Size = new System.Drawing.Size(22, 23);
            this.bnMyShowsCollapse.TabIndex = 4;
            this.bnMyShowsCollapse.Text = "-";
            this.bnMyShowsCollapse.UseVisualStyleBackColor = true;
            this.bnMyShowsCollapse.Click += new System.EventHandler(bnMyShowsCollapse_Click);
            // 
            // bnMyShowsVisitTVDB
            // 
            this.bnMyShowsVisitTVDB.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnMyShowsVisitTVDB.Location = new System.Drawing.Point(506, 480);
            this.bnMyShowsVisitTVDB.Name = "bnMyShowsVisitTVDB";
            this.bnMyShowsVisitTVDB.Size = new System.Drawing.Size(75, 23);
            this.bnMyShowsVisitTVDB.TabIndex = 7;
            this.bnMyShowsVisitTVDB.Text = "&Visit TVDB";
            this.bnMyShowsVisitTVDB.UseVisualStyleBackColor = true;
            this.bnMyShowsVisitTVDB.Click += new System.EventHandler(bnMyShowsVisitTVDB_Click);
            // 
            // bnMyShowsOpenFolder
            // 
            this.bnMyShowsOpenFolder.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnMyShowsOpenFolder.Location = new System.Drawing.Point(425, 480);
            this.bnMyShowsOpenFolder.Name = "bnMyShowsOpenFolder";
            this.bnMyShowsOpenFolder.Size = new System.Drawing.Size(75, 23);
            this.bnMyShowsOpenFolder.TabIndex = 6;
            this.bnMyShowsOpenFolder.Text = "&Open";
            this.bnMyShowsOpenFolder.UseVisualStyleBackColor = true;
            this.bnMyShowsOpenFolder.Click += new System.EventHandler(bnMyShowsOpenFolder_Click);
            // 
            // bnMyShowsRefresh
            // 
            this.bnMyShowsRefresh.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnMyShowsRefresh.Location = new System.Drawing.Point(331, 480);
            this.bnMyShowsRefresh.Name = "bnMyShowsRefresh";
            this.bnMyShowsRefresh.Size = new System.Drawing.Size(75, 23);
            this.bnMyShowsRefresh.TabIndex = 5;
            this.bnMyShowsRefresh.Text = "&Refresh";
            this.bnMyShowsRefresh.UseVisualStyleBackColor = true;
            this.bnMyShowsRefresh.Click += new System.EventHandler(bnMyShowsRefresh_Click);
            // 
            // epGuideHTML
            // 
            this.epGuideHTML.AllowWebBrowserDrop = false;
            this.epGuideHTML.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            this.epGuideHTML.Location = new System.Drawing.Point(279, 6);
            this.epGuideHTML.MinimumSize = new System.Drawing.Size(20, 20);
            this.epGuideHTML.Name = "epGuideHTML";
            this.epGuideHTML.Size = new System.Drawing.Size(644, 468);
            this.epGuideHTML.TabIndex = 6;
            this.epGuideHTML.WebBrowserShortcutsEnabled = false;
            this.epGuideHTML.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(epGuideHTML_Navigating);
            // 
            // MyShowTree
            // 
            this.MyShowTree.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left));
            this.MyShowTree.HideSelection = false;
            this.MyShowTree.Location = new System.Drawing.Point(3, 6);
            this.MyShowTree.Name = "MyShowTree";
            this.MyShowTree.Size = new System.Drawing.Size(270, 468);
            this.MyShowTree.TabIndex = 0;
            this.MyShowTree.MouseClick += new System.Windows.Forms.MouseEventHandler(MyShowTree_MouseClick);
            this.MyShowTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(MyShowTree_AfterSelect);
            // 
            // bnMyShowsDelete
            // 
            this.bnMyShowsDelete.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnMyShowsDelete.Location = new System.Drawing.Point(170, 480);
            this.bnMyShowsDelete.Name = "bnMyShowsDelete";
            this.bnMyShowsDelete.Size = new System.Drawing.Size(75, 23);
            this.bnMyShowsDelete.TabIndex = 3;
            this.bnMyShowsDelete.Text = "&Delete";
            this.bnMyShowsDelete.UseVisualStyleBackColor = true;
            this.bnMyShowsDelete.Click += new System.EventHandler(bnMyShowsDelete_Click);
            // 
            // bnMyShowsEdit
            // 
            this.bnMyShowsEdit.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnMyShowsEdit.Location = new System.Drawing.Point(89, 480);
            this.bnMyShowsEdit.Name = "bnMyShowsEdit";
            this.bnMyShowsEdit.Size = new System.Drawing.Size(75, 23);
            this.bnMyShowsEdit.TabIndex = 2;
            this.bnMyShowsEdit.Text = "&Edit";
            this.bnMyShowsEdit.UseVisualStyleBackColor = true;
            this.bnMyShowsEdit.Click += new System.EventHandler(bnMyShowsEdit_Click);
            // 
            // bnMyShowsAdd
            // 
            this.bnMyShowsAdd.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnMyShowsAdd.Location = new System.Drawing.Point(8, 480);
            this.bnMyShowsAdd.Name = "bnMyShowsAdd";
            this.bnMyShowsAdd.Size = new System.Drawing.Size(75, 23);
            this.bnMyShowsAdd.TabIndex = 1;
            this.bnMyShowsAdd.Text = "&Add";
            this.bnMyShowsAdd.UseVisualStyleBackColor = true;
            this.bnMyShowsAdd.Click += new System.EventHandler(bnMyShowsAdd_Click);
            // 
            // tbAllInOne
            // 
            this.tbAllInOne.Controls.Add(this.bnAIOOptions);
            this.tbAllInOne.Controls.Add(this.label1);
            this.tbAllInOne.Controls.Add(this.bnAIONFO);
            this.tbAllInOne.Controls.Add(this.bnAIODownloads);
            this.tbAllInOne.Controls.Add(this.bnAIORSS);
            this.tbAllInOne.Controls.Add(this.bnAIOCopyMove);
            this.tbAllInOne.Controls.Add(this.bnAIORename);
            this.tbAllInOne.Controls.Add(this.bnAIOAllNone);
            this.tbAllInOne.Controls.Add(this.bnRemoveSel);
            this.tbAllInOne.Controls.Add(this.bnAIOIgnore);
            this.tbAllInOne.Controls.Add(this.bnAIOWhichSearch);
            this.tbAllInOne.Controls.Add(this.bnAIOBTSearch);
            this.tbAllInOne.Controls.Add(this.lvAIO);
            this.tbAllInOne.Controls.Add(this.bnAIOAction);
            this.tbAllInOne.Controls.Add(this.bnAIOCheck);
            this.tbAllInOne.Location = new System.Drawing.Point(4, 22);
            this.tbAllInOne.Name = "tbAllInOne";
            this.tbAllInOne.Padding = new System.Windows.Forms.Padding(3);
            this.tbAllInOne.Size = new System.Drawing.Size(923, 507);
            this.tbAllInOne.TabIndex = 11;
            this.tbAllInOne.Text = "Scan";
            this.tbAllInOne.UseVisualStyleBackColor = true;
            // 
            // bnAIOOptions
            // 
            this.bnAIOOptions.Location = new System.Drawing.Point(89, 6);
            this.bnAIOOptions.Name = "bnAIOOptions";
            this.bnAIOOptions.Size = new System.Drawing.Size(75, 23);
            this.bnAIOOptions.TabIndex = 8;
            this.bnAIOOptions.Text = "&Options...";
            this.bnAIOOptions.UseVisualStyleBackColor = true;
            this.bnAIOOptions.Click += new System.EventHandler(bnAIOOptions_Click);
            // 
            // label1
            // 
            this.label1.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(393, 482);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Check:";
            // 
            // bnAIONFO
            // 
            this.bnAIONFO.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.bnAIONFO.Location = new System.Drawing.Point(845, 477);
            this.bnAIONFO.Name = "bnAIONFO";
            this.bnAIONFO.Size = new System.Drawing.Size(75, 23);
            this.bnAIONFO.TabIndex = 6;
            this.bnAIONFO.Text = "NFO";
            this.bnAIONFO.UseVisualStyleBackColor = true;
            this.bnAIONFO.Click += new System.EventHandler(bnAIONFO_Click);
            // 
            // bnAIODownloads
            // 
            this.bnAIODownloads.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.bnAIODownloads.Location = new System.Drawing.Point(764, 477);
            this.bnAIODownloads.Name = "bnAIODownloads";
            this.bnAIODownloads.Size = new System.Drawing.Size(75, 23);
            this.bnAIODownloads.TabIndex = 6;
            this.bnAIODownloads.Text = "Downloads";
            this.bnAIODownloads.UseVisualStyleBackColor = true;
            this.bnAIODownloads.Click += new System.EventHandler(bnAIODownloads_Click);
            // 
            // bnAIORSS
            // 
            this.bnAIORSS.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.bnAIORSS.Location = new System.Drawing.Point(683, 477);
            this.bnAIORSS.Name = "bnAIORSS";
            this.bnAIORSS.Size = new System.Drawing.Size(75, 23);
            this.bnAIORSS.TabIndex = 6;
            this.bnAIORSS.Text = "RSS";
            this.bnAIORSS.UseVisualStyleBackColor = true;
            this.bnAIORSS.Click += new System.EventHandler(bnAIORSS_Click);
            // 
            // bnAIOCopyMove
            // 
            this.bnAIOCopyMove.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.bnAIOCopyMove.Location = new System.Drawing.Point(602, 477);
            this.bnAIOCopyMove.Name = "bnAIOCopyMove";
            this.bnAIOCopyMove.Size = new System.Drawing.Size(75, 23);
            this.bnAIOCopyMove.TabIndex = 6;
            this.bnAIOCopyMove.Text = "Copy/Move";
            this.bnAIOCopyMove.UseVisualStyleBackColor = true;
            this.bnAIOCopyMove.Click += new System.EventHandler(bnAIOCopyMove_Click);
            // 
            // bnAIORename
            // 
            this.bnAIORename.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.bnAIORename.Location = new System.Drawing.Point(521, 477);
            this.bnAIORename.Name = "bnAIORename";
            this.bnAIORename.Size = new System.Drawing.Size(75, 23);
            this.bnAIORename.TabIndex = 6;
            this.bnAIORename.Text = "Rename";
            this.bnAIORename.UseVisualStyleBackColor = true;
            this.bnAIORename.Click += new System.EventHandler(bnAIORename_Click);
            // 
            // bnAIOAllNone
            // 
            this.bnAIOAllNone.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.bnAIOAllNone.Location = new System.Drawing.Point(440, 477);
            this.bnAIOAllNone.Name = "bnAIOAllNone";
            this.bnAIOAllNone.Size = new System.Drawing.Size(75, 23);
            this.bnAIOAllNone.TabIndex = 6;
            this.bnAIOAllNone.Text = "All/None";
            this.bnAIOAllNone.UseVisualStyleBackColor = true;
            this.bnAIOAllNone.Click += new System.EventHandler(bnAIOAllNone_Click);
            // 
            // bnRemoveSel
            // 
            this.bnRemoveSel.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnRemoveSel.Location = new System.Drawing.Point(269, 477);
            this.bnRemoveSel.Name = "bnRemoveSel";
            this.bnRemoveSel.Size = new System.Drawing.Size(75, 23);
            this.bnRemoveSel.TabIndex = 5;
            this.bnRemoveSel.Text = "&Remove Sel";
            this.bnRemoveSel.UseVisualStyleBackColor = true;
            this.bnRemoveSel.Click += new System.EventHandler(bnRemoveSel_Click);
            // 
            // bnAIOIgnore
            // 
            this.bnAIOIgnore.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnAIOIgnore.Location = new System.Drawing.Point(188, 477);
            this.bnAIOIgnore.Name = "bnAIOIgnore";
            this.bnAIOIgnore.Size = new System.Drawing.Size(75, 23);
            this.bnAIOIgnore.TabIndex = 5;
            this.bnAIOIgnore.Text = "&Ignore Sel";
            this.bnAIOIgnore.UseVisualStyleBackColor = true;
            this.bnAIOIgnore.Click += new System.EventHandler(bnAIOIgnore_Click);
            // 
            // bnAIOWhichSearch
            // 
            this.bnAIOWhichSearch.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnAIOWhichSearch.Image = ((System.Drawing.Image)(resources.GetObject("bnAIOWhichSearch.Image")));
            this.bnAIOWhichSearch.Location = new System.Drawing.Point(161, 477);
            this.bnAIOWhichSearch.Name = "bnAIOWhichSearch";
            this.bnAIOWhichSearch.Size = new System.Drawing.Size(19, 23);
            this.bnAIOWhichSearch.TabIndex = 4;
            this.bnAIOWhichSearch.UseVisualStyleBackColor = true;
            this.bnAIOWhichSearch.Click += new System.EventHandler(bnAIOWhichSearch_Click);
            // 
            // bnAIOBTSearch
            // 
            this.bnAIOBTSearch.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnAIOBTSearch.Location = new System.Drawing.Point(87, 477);
            this.bnAIOBTSearch.Name = "bnAIOBTSearch";
            this.bnAIOBTSearch.Size = new System.Drawing.Size(75, 23);
            this.bnAIOBTSearch.TabIndex = 3;
            this.bnAIOBTSearch.Text = "BT S&earch";
            this.bnAIOBTSearch.UseVisualStyleBackColor = true;
            this.bnAIOBTSearch.Click += new System.EventHandler(bnAIOBTSearch_Click);
            // 
            // lvAIO
            // 
            this.lvAIO.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            this.lvAIO.CheckBoxes = true;
            this.lvAIO.Columns.AddRange(new System.Windows.Forms.ColumnHeader[9] { this.columnHeader48, this.columnHeader49, this.columnHeader51, this.columnHeader52, this.columnHeader53, this.columnHeader54, this.columnHeader55, this.columnHeader56, this.columnHeader58 });
            this.lvAIO.FullRowSelect = true;
            listViewGroup13.Header = "Missing";
            listViewGroup13.Name = "lvgAIOMissing";
            listViewGroup14.Header = "Rename";
            listViewGroup14.Name = "lvgAIORename";
            listViewGroup15.Header = "Copy";
            listViewGroup15.Name = "lvgAIOCopy";
            listViewGroup16.Header = "Move";
            listViewGroup16.Name = "lvgAIOMove";
            listViewGroup17.Header = "Download RSS";
            listViewGroup17.Name = "lvgAIODownloadRSS";
            listViewGroup18.Header = "Download";
            listViewGroup18.Name = "lvgAIODownload";
            listViewGroup19.Header = "NFO File";
            listViewGroup19.Name = "lvgAIONFO";
            listViewGroup20.Header = "Downloading In µTorrent";
            listViewGroup20.Name = "lngInuTorrent";
            this.lvAIO.Groups.AddRange(new System.Windows.Forms.ListViewGroup[8] { listViewGroup13, listViewGroup14, listViewGroup15, listViewGroup16, listViewGroup17, listViewGroup18, listViewGroup19, listViewGroup20 });
            this.lvAIO.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvAIO.HideSelection = false;
            this.lvAIO.Location = new System.Drawing.Point(0, 35);
            this.lvAIO.Name = "lvAIO";
            this.lvAIO.ShowItemToolTips = true;
            this.lvAIO.Size = new System.Drawing.Size(920, 436);
            this.lvAIO.SmallImageList = this.ilIcons;
            this.lvAIO.TabIndex = 2;
            this.lvAIO.UseCompatibleStateImageBehavior = false;
            this.lvAIO.View = System.Windows.Forms.View.Details;
            this.lvAIO.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(lvAIO_MouseDoubleClick);
            this.lvAIO.MouseClick += new System.Windows.Forms.MouseEventHandler(lvAIO_MouseClick);
            this.lvAIO.SelectedIndexChanged += new System.EventHandler(lvAIO_SelectedIndexChanged);
            this.lvAIO.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(lvAIO_ItemCheck);
            this.lvAIO.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(lvAIO_RetrieveVirtualItem);
            this.lvAIO.KeyDown += new System.Windows.Forms.KeyEventHandler(lvAIO_KeyDown);
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
            // 
            // bnAIOAction
            // 
            this.bnAIOAction.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnAIOAction.Location = new System.Drawing.Point(6, 477);
            this.bnAIOAction.Name = "bnAIOAction";
            this.bnAIOAction.Size = new System.Drawing.Size(75, 23);
            this.bnAIOAction.TabIndex = 0;
            this.bnAIOAction.Text = "&Do Checked";
            this.bnAIOAction.Click += new System.EventHandler(bnAIOAction_Click);
            // 
            // bnAIOCheck
            // 
            this.bnAIOCheck.Location = new System.Drawing.Point(8, 6);
            this.bnAIOCheck.Name = "bnAIOCheck";
            this.bnAIOCheck.Size = new System.Drawing.Size(75, 23);
            this.bnAIOCheck.TabIndex = 0;
            this.bnAIOCheck.Text = "&Scan";
            this.bnAIOCheck.UseVisualStyleBackColor = true;
            this.bnAIOCheck.Click += new System.EventHandler(bnAIOCheck_Click);
            // 
            // tbWTW
            // 
            this.tbWTW.Controls.Add(this.bnWTWChooseSite);
            this.tbWTW.Controls.Add(this.bnWTWBTSearch);
            this.tbWTW.Controls.Add(this.bnWhenToWatchCheck);
            this.tbWTW.Controls.Add(this.txtWhenToWatchSynopsis);
            this.tbWTW.Controls.Add(this.calCalendar);
            this.tbWTW.Controls.Add(this.lvWhenToWatch);
            this.tbWTW.Location = new System.Drawing.Point(4, 22);
            this.tbWTW.Name = "tbWTW";
            this.tbWTW.Size = new System.Drawing.Size(923, 507);
            this.tbWTW.TabIndex = 4;
            this.tbWTW.Text = "When to watch";
            this.tbWTW.UseVisualStyleBackColor = true;
            // 
            // bnWTWChooseSite
            // 
            this.bnWTWChooseSite.Image = ((System.Drawing.Image)(resources.GetObject("bnWTWChooseSite.Image")));
            this.bnWTWChooseSite.Location = new System.Drawing.Point(163, 6);
            this.bnWTWChooseSite.Name = "bnWTWChooseSite";
            this.bnWTWChooseSite.Size = new System.Drawing.Size(19, 23);
            this.bnWTWChooseSite.TabIndex = 2;
            this.bnWTWChooseSite.UseVisualStyleBackColor = true;
            this.bnWTWChooseSite.Click += new System.EventHandler(bnWTWChooseSite_Click);
            // 
            // bnWTWBTSearch
            // 
            this.bnWTWBTSearch.Location = new System.Drawing.Point(89, 6);
            this.bnWTWBTSearch.Name = "bnWTWBTSearch";
            this.bnWTWBTSearch.Size = new System.Drawing.Size(75, 23);
            this.bnWTWBTSearch.TabIndex = 1;
            this.bnWTWBTSearch.Text = "BT &Search";
            this.bnWTWBTSearch.UseVisualStyleBackColor = true;
            this.bnWTWBTSearch.Click += new System.EventHandler(bnWTWBTSearch_Click);
            // 
            // bnWhenToWatchCheck
            // 
            this.bnWhenToWatchCheck.Location = new System.Drawing.Point(8, 6);
            this.bnWhenToWatchCheck.Name = "bnWhenToWatchCheck";
            this.bnWhenToWatchCheck.Size = new System.Drawing.Size(75, 23);
            this.bnWhenToWatchCheck.TabIndex = 0;
            this.bnWhenToWatchCheck.Text = "&Refresh";
            this.bnWhenToWatchCheck.UseVisualStyleBackColor = true;
            this.bnWhenToWatchCheck.Click += new System.EventHandler(bnWhenToWatchCheck_Click);
            // 
            // txtWhenToWatchSynopsis
            // 
            this.txtWhenToWatchSynopsis.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            this.txtWhenToWatchSynopsis.Font = (new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)(0)));
            this.txtWhenToWatchSynopsis.Location = new System.Drawing.Point(0, 352);
            this.txtWhenToWatchSynopsis.Multiline = true;
            this.txtWhenToWatchSynopsis.Name = "txtWhenToWatchSynopsis";
            this.txtWhenToWatchSynopsis.ReadOnly = true;
            this.txtWhenToWatchSynopsis.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtWhenToWatchSynopsis.Size = new System.Drawing.Size(733, 154);
            this.txtWhenToWatchSynopsis.TabIndex = 4;
            // 
            // calCalendar
            // 
            this.calCalendar.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.calCalendar.FirstDayOfWeek = System.Windows.Forms.Day.Sunday;
            this.calCalendar.Location = new System.Drawing.Point(745, 352);
            this.calCalendar.MaxSelectionCount = 1;
            this.calCalendar.Name = "calCalendar";
            this.calCalendar.TabIndex = 5;
            this.calCalendar.DateSelected += new System.Windows.Forms.DateRangeEventHandler(calCalendar_DateSelected);
            // 
            // lvWhenToWatch
            // 
            this.lvWhenToWatch.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            this.lvWhenToWatch.Columns.AddRange(new System.Windows.Forms.ColumnHeader[8] { this.columnHeader29, this.columnHeader30, this.columnHeader31, this.columnHeader32, this.columnHeader36, this.columnHeader33, this.columnHeader34, this.columnHeader35 });
            this.lvWhenToWatch.FullRowSelect = true;
            listViewGroup1.Header = "Recently Aired";
            listViewGroup1.Name = "justPassed";
            listViewGroup2.Header = "Next 7 Days";
            listViewGroup2.Name = "next7days";
            listViewGroup2.Tag = "1";
            listViewGroup3.Header = "Later";
            listViewGroup3.Name = "later";
            listViewGroup3.Tag = "2";
            listViewGroup4.Header = "Future Episodes";
            listViewGroup4.Name = "futureEps";
            this.lvWhenToWatch.Groups.AddRange(new System.Windows.Forms.ListViewGroup[4] { listViewGroup1, listViewGroup2, listViewGroup3, listViewGroup4 });
            this.lvWhenToWatch.HideSelection = false;
            this.lvWhenToWatch.Location = new System.Drawing.Point(0, 35);
            this.lvWhenToWatch.Name = "lvWhenToWatch";
            this.lvWhenToWatch.ShowItemToolTips = true;
            this.lvWhenToWatch.Size = new System.Drawing.Size(923, 311);
            this.lvWhenToWatch.SmallImageList = this.ilIcons;
            this.lvWhenToWatch.TabIndex = 3;
            this.lvWhenToWatch.UseCompatibleStateImageBehavior = false;
            this.lvWhenToWatch.View = System.Windows.Forms.View.Details;
            this.lvWhenToWatch.MouseClick += new System.Windows.Forms.MouseEventHandler(lvWhenToWatch_MouseClick);
            this.lvWhenToWatch.SelectedIndexChanged += new System.EventHandler(lvWhenToWatch_Click);
            this.lvWhenToWatch.DoubleClick += new System.EventHandler(lvWhenToWatch_DoubleClick);
            this.lvWhenToWatch.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(lvWhenToWatch_ColumnClick);
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
            this.tableLayoutPanel2.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add((new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45)));
            this.tableLayoutPanel2.ColumnStyles.Add((new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40)));
            this.tableLayoutPanel2.ColumnStyles.Add((new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15)));
            this.tableLayoutPanel2.Controls.Add(this.pbProgressBarx, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtDLStatusLabel, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.tsNextShowTxt, 0, 0);
            this.tableLayoutPanel2.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 559);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add((new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100)));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(919, 19);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // pbProgressBarx
            // 
            this.pbProgressBarx.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            this.pbProgressBarx.Location = new System.Drawing.Point(783, 3);
            this.pbProgressBarx.Name = "pbProgressBarx";
            this.pbProgressBarx.Size = new System.Drawing.Size(133, 13);
            this.pbProgressBarx.Step = 1;
            this.pbProgressBarx.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbProgressBarx.TabIndex = 0;
            // 
            // txtDLStatusLabel
            // 
            this.txtDLStatusLabel.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            this.txtDLStatusLabel.Location = new System.Drawing.Point(416, 6);
            this.txtDLStatusLabel.Name = "txtDLStatusLabel";
            this.txtDLStatusLabel.Size = new System.Drawing.Size(361, 13);
            this.txtDLStatusLabel.TabIndex = 1;
            this.txtDLStatusLabel.Text = "Background Download: ---";
            this.txtDLStatusLabel.Visible = false;
            // 
            // tsNextShowTxt
            // 
            this.tsNextShowTxt.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
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
            this.menuSearchSites.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(menuSearchSites_ItemClicked);
            // 
            // refreshWTWTimer
            // 
            this.refreshWTWTimer.Enabled = true;
            this.refreshWTWTimer.Interval = 600000;
            this.refreshWTWTimer.Tick += new System.EventHandler(refreshWTWTimer_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipText = "TV Rename is t3h r0x0r";
            this.notifyIcon1.BalloonTipTitle = "TV Rename 2.1";
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "TV Rename 2.1";
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(notifyIcon1_Click);
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(notifyIcon1_DoubleClick);
            // 
            // showRightClickMenu
            // 
            this.showRightClickMenu.Name = "menuSearchSites";
            this.showRightClickMenu.ShowImageMargin = false;
            this.showRightClickMenu.Size = new System.Drawing.Size(36, 4);
            this.showRightClickMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(showRightClickMenu_ItemClicked);
            // 
            // folderRightClickMenu
            // 
            this.folderRightClickMenu.Name = "folderRightClickMenu";
            this.folderRightClickMenu.ShowImageMargin = false;
            this.folderRightClickMenu.Size = new System.Drawing.Size(36, 4);
            this.folderRightClickMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(folderRightClickMenu_ItemClicked);
            // 
            // statusTimer
            // 
            this.statusTimer.Enabled = true;
            this.statusTimer.Interval = 250;
            this.statusTimer.Tick += new System.EventHandler(statusTimer_Tick);
            // 
            // BGDownloadTimer
            // 
            this.BGDownloadTimer.Enabled = true;
            this.BGDownloadTimer.Interval = 10000;
            this.BGDownloadTimer.Tick += new System.EventHandler(BGDownloadTimer_Tick);
            // 
            // tmrShowUpcomingPopup
            // 
            this.tmrShowUpcomingPopup.Interval = 250;
            this.tmrShowUpcomingPopup.Tick += new System.EventHandler(tmrShowUpcomingPopup_Tick);
            // 
            // quickTimer
            // 
            this.quickTimer.Interval = 1;
            this.quickTimer.Tick += new System.EventHandler(quickTimer_Tick);
            // 
            // ignoreListToolStripMenuItem
            // 
            this.ignoreListToolStripMenuItem.Name = "ignoreListToolStripMenuItem";
            this.ignoreListToolStripMenuItem.ShortcutKeys = (System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I));
            this.ignoreListToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.ignoreListToolStripMenuItem.Text = "&Ignore List";
            this.ignoreListToolStripMenuItem.Click += new System.EventHandler(ignoreListToolStripMenuItem_Click);
            // 
            // UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
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
            this.Load += new System.EventHandler(UI_Load);
            this.SizeChanged += new System.EventHandler(UI_SizeChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(UI_FormClosing);
            this.LocationChanged += new System.EventHandler(UI_LocationChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(UI_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tbMyShows.ResumeLayout(false);
            this.tbAllInOne.ResumeLayout(false);
            this.tbAllInOne.PerformLayout();
            this.tbWTW.ResumeLayout(false);
            this.tbWTW.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public static int BGDLLongInterval()
        {
            return 1000 * 60 * 60; // one hour
        }

        protected void MoreBusy()
        {
            Interlocked.Increment(ref Busy);
        }
        protected void LessBusy()
        {
            Interlocked.Decrement(ref Busy);
        }

        private static bool IsDebug()
        {
#if DEBUG
            return true;
#else
				 return false;
#endif
        }

        private void HideStuff()
        {
            if (IncludeExperimentalStuff() || IsDebug() || Version.ForceExperimentalOn())
            {
                // debug stuff
                // DirectoryInfo di(System::Windows::Forms::Application::UserAppDataPath+"\\..\\..\\..\\uTorrent");
            }
            else
            {
                // hide developmental things in release build
                actorsToolStripMenuItem.Visible = false;
                uTorrentToolStripMenuItem.Visible = false;

            }
        }

        public UI(string[] args)
        {
            bool ok = true;
            string hint = "";

            if ((args.Length == 1) && (args[0].ToLower() == "/recover"))
            {
                ok = false; // force recover dialog
                hint = "Recover manually requested.";
            }

            FileInfo tvdbFile = TVDoc.TVDBFile();
            FileInfo settingsFile = TVDoc.TVDocSettingsFile();

            do // loop until no problems loading settings & tvdb cache files
            {
                if (!ok) // something went wrong last time around, ask the user what to do
                {
                    RecoverXML rec = new RecoverXML(hint);
                    if (rec.ShowDialog() == DialogResult.OK)
                    {
                        tvdbFile = rec.DBFile;
                        settingsFile = rec.SettingsFile;
                    }
                    else
                        Environment.Exit(1);
                }

                // try loading using current settings files
                mDoc = new TVDoc(args, settingsFile, tvdbFile);

                if (!ok)
                    mDoc.SetDirty();

                ok = mDoc.LoadOK;

                if (!ok)
                {
                    hint = "";
                    if (!string.IsNullOrEmpty(mDoc.LoadErr))
                        hint += mDoc.LoadErr;
                    string h2 = mDoc.GetTVDB(false, "Recover").LoadErr;
                    if (!string.IsNullOrEmpty(h2))
                        hint += "\r\n" + h2;
                }
            } while (!ok);

            ExperimentalFeatures = mDoc.HasArg("/experimental");

            Busy = 0;
            mLastEpClicked = null;
            mLastFolderClicked = null;
            mLastSeasonClicked = null;
            mLastShowClicked = null;
            mLastAIOClicked = null;

            mInternalChange = 0;
            mFoldersToOpen = new StringList();

            InitializeComponent();


            try
            {
                LoadLayoutXML();
            }
            catch
            {
                // silently fail, doesn't matter too much
            }

            SetProgress += SetProgressActual;

            lvWhenToWatch.ListViewItemSorter = new DateSorterWTW();

            if (mDoc.HasArg("/hide"))
            {
                this.WindowState = FormWindowState.Minimized;
                this.Visible = false;
                this.Hide();
            }

            HideStuff();

            this.Text = this.Text + " " + Version.DisplayVersionString();

            FillShowLists();
            UpdateSearchButton();
            SetGuideHTMLbody("");
            mDoc.DoWhenToWatch(true);
            FillWhenToWatchList();
            mDoc.WriteUpcomingRSS();
            ShowHideNotificationIcon();

            int t = mDoc.Settings.StartupTab;
            if (t < tabControl1.TabCount)
                tabControl1.SelectedIndex = mDoc.Settings.StartupTab;
            tabControl1_SelectedIndexChanged(null, null);
        }

        public void SetProgressActual(int p)
        {
            if (p < 0)
                p = 0;
            else if (p > 100)
                p = 100;

            pbProgressBarx.Value = p;
            pbProgressBarx.Update();
        }

        public void ProcessArgs()
        {
            bool quit = mDoc.HasArg("/quit");

            if (mDoc.HasArg("/hide")) // /hide implies /quit
                quit = true;

            // process command line arguments, does not include application path
            //array<String ^> ^args = mDoc->GetArgs();
            //for (int i=0;i<args->Length;i++)
            //{
            //String ^arg = args[i]->ToLower();
            //            
            //			if (arg == "/missingcheck")
            //			{
            //			tabControl1->SelectedIndex = 2;
            //			bnDoMissingCheck_Click(nullptr,nullptr);
            //			}
            //			else if (arg == "/exportmissingxml")
            //			mDoc->ExportMissingXML(args[++i]);
            //			else if (arg == "/exportmissingcsv")
            //			mDoc->ExportMissingCSV(args[++i]);
            //			else if (arg == "/renamingcheck")
            //			{
            //			tabControl1->SelectedIndex = 1;
            //			bnRenameCheck_Click(nullptr,nullptr);
            //			}
            //			else if (arg == "/exportrenamingxml")
            //			mDoc->ExportRenamingXML(args[++i]);
            //			else if (arg == "/renamingdo")
            //			bnRenameDoRenaming_Click(nullptr,nullptr);
            //			else if (arg == "/fnocheck")
            //			{
            //			tabControl1->SelectedIndex = 2;
            //			bnFindMissingStuff_Click(nullptr,nullptr);
            //			}
            //			else if (arg == "exportfnoxml")
            //			mDoc->ExportFOXML(args[++i]);
            //			else if (arg == "/fnodo")
            //			bnDoMovingAndCopying_Click(nullptr,nullptr);
            //			
            //}

            if (quit)
                this.Close();
        }


        ~UI()
        {
            //		mDoc->StopBGDownloadThread();  TODO
            mDoc = null;
        }

        public void UpdateSearchButton()
        {
            string name = mDoc.GetSearchers().Name(mDoc.Settings.TheSearchers.CurrentSearchNum());
            bnWTWBTSearch.Text = name;
            bnAIOBTSearch.Text = name;
            FillEpGuideHTML();
        }

        private void exitToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Close();
        }
        private void visitWebsiteToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            TVDoc.SysOpen("http://tvrename.com");
        }

        private void UI_Load(object sender, System.EventArgs e)
        {
            this.ShowInTaskbar = mDoc.Settings.ShowInTaskbar && !mDoc.HasArg("/hide");

            foreach (TabPage tp in tabControl1.TabPages) // grr! TODO: why does it go white?
                tp.BackColor = System.Drawing.SystemColors.Control;

            this.Show();
            UI_LocationChanged(null, null);
            UI_SizeChanged(null, null);

            backgroundDownloadToolStripMenuItem.Checked = mDoc.Settings.BGDownload;
            offlineOperationToolStripMenuItem.Checked = mDoc.Settings.OfflineMode;
            BGDownloadTimer.Interval = 10000; // first time
            if (mDoc.Settings.BGDownload)
                BGDownloadTimer.Start();

            quickTimer.Start();
            //ProcessArgs();
        }

        private ListView ListViewByName(string name)
        {
            if (name == "WhenToWatch")
                return lvWhenToWatch;
            if (name == "AllInOne")
                return lvAIO;
            return null;
        }
        private void flushCacheToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            System.Windows.Forms.DialogResult res = MessageBox.Show("Are you sure you want to remove all " + "locally stored TheTVDB information?  This information will have to be downloaded again.  You " + "can force the refresh of a single show by holding down the \"Control\" key while clicking on " + "the \"Refresh\" button in the \"My Shows\" tab.", "Flush Web Cache", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res == System.Windows.Forms.DialogResult.Yes)
            {
                mDoc.GetTVDB(false, "").ForgetEverything();
                FillShowLists();
                FillEpGuideHTML();
                FillWhenToWatchList();
            }
        }

        private bool LoadWidths(XmlReader xml)
        {
            string forwho = xml.GetAttribute("For");

            ListView lv = ListViewByName(forwho);
            if (lv == null)
            {
                xml.ReadOuterXml();
                return true;
            }

            xml.Read();
            int c = 0;
            while (xml.Name == "Width")
            {
                if (c >= lv.Columns.Count)
                    return false;
                lv.Columns[c++].Width = xml.ReadElementContentAsInt();
            }
            xml.Read();
            return true;
        }


        private bool LoadLayoutXML()
        {
            if (mDoc.HasArg("/hide"))
                return true;

            bool ok = true;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;

            string fn = System.Windows.Forms.Application.UserAppDataPath + "\\Layout.xml";
            if (!File.Exists(fn))
                return true;

            XmlReader reader = XmlReader.Create(fn, settings);

            reader.Read();
            if (reader.Name != "xml")
                return false;

            reader.Read();
            if (reader.Name != "TVRename")
                return false;

            if (reader.GetAttribute("Version") != "2.1")
                return false;

            reader.Read();
            if (reader.Name != "Layout")
                return false;

            reader.Read();
            while (reader.Name != "Layout")
            {
                if (reader.Name == "Window")
                {
                    reader.Read();
                    while (reader.Name != "Window")
                    {
                        if (reader.Name == "Size")
                        {
                            int x = int.Parse(reader.GetAttribute("Width"));
                            int y = int.Parse(reader.GetAttribute("Height"));
                            this.Size = new System.Drawing.Size(x, y);
                            reader.Read();
                        }
                        else if (reader.Name == "Location")
                        {
                            int x = int.Parse(reader.GetAttribute("X"));
                            int y = int.Parse(reader.GetAttribute("Y"));
                            this.Location = new Point(x, y);
                            reader.Read();
                        }
                        else if (reader.Name == "Maximized")
                            this.WindowState = (reader.ReadElementContentAsBoolean() ? FormWindowState.Maximized : FormWindowState.Normal);
                        else
                            reader.ReadOuterXml();
                    }
                    reader.Read();
                } // window
                else if (reader.Name == "ColumnWidths")
                    ok = LoadWidths(reader) && ok;
                else
                    reader.ReadOuterXml();
            } // while

            reader.Close();
            return ok;
        }

        private bool SaveLayoutXML()
        {
            if (mDoc.HasArg("/hide"))
                return true;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            XmlWriter writer = XmlWriter.Create(System.Windows.Forms.Application.UserAppDataPath + "\\Layout.xml", settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("TVRename");
            writer.WriteStartAttribute("Version");
            writer.WriteValue("2.1");
            writer.WriteEndAttribute(); // version
            writer.WriteStartElement("Layout");
            writer.WriteStartElement("Window");

            writer.WriteStartElement("Size");
            writer.WriteStartAttribute("Width");
            writer.WriteValue(mLastNonMaximizedSize.Width);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("Height");
            writer.WriteValue(mLastNonMaximizedSize.Height);
            writer.WriteEndAttribute();
            writer.WriteEndElement(); // size

            writer.WriteStartElement("Location");
            writer.WriteStartAttribute("X");
            writer.WriteValue(mLastNonMaximizedLocation.X);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("Y");
            writer.WriteValue(mLastNonMaximizedLocation.Y);
            writer.WriteEndAttribute();
            writer.WriteEndElement(); // Location

            writer.WriteStartElement("Maximized");
            writer.WriteValue(this.WindowState == FormWindowState.Maximized);
            writer.WriteEndElement(); // maximized

            writer.WriteEndElement(); // window

            WriteColWidthsXML("WhenToWatch", writer);
            WriteColWidthsXML("AllInOne", writer);

            writer.WriteEndElement(); // Layout
            writer.WriteEndElement(); // tvrename
            writer.WriteEndDocument();

            writer.Close();
            writer = null;

            return true;
        }
        private void WriteColWidthsXML(string thingName, XmlWriter writer)
        {
            ListView lv = ListViewByName(thingName);
            if (lv == null)
                return;
            writer.WriteStartElement("ColumnWidths");
            writer.WriteStartAttribute("For");
            writer.WriteValue(thingName);
            writer.WriteEndAttribute();
            foreach (ColumnHeader lvc in lv.Columns)
            {
                writer.WriteStartElement("Width");
                writer.WriteValue(lvc.Width);
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // columnwidths
        }

        private void UI_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {

            if (mDoc.Dirty())
            {
                System.Windows.Forms.DialogResult res = MessageBox.Show("Your changes have not been saved.  Do you wish to save before quitting?", "Unsaved data", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (res == System.Windows.Forms.DialogResult.Yes)
                {
                    mDoc.WriteXMLSettings();
                }
                else if (res == System.Windows.Forms.DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (res == System.Windows.Forms.DialogResult.No)
                {
                }
            }
            if (!e.Cancel)
            {
                SaveLayoutXML();
                mDoc.TidyTVDB();
                mDoc.Closing();
            }

        }

        private ContextMenuStrip BuildSearchMenu()
        {
            menuSearchSites.Items.Clear();
            for (int i = 0; i < mDoc.GetSearchers().Count(); i++)
            {
                ToolStripMenuItem tsi = new ToolStripMenuItem(mDoc.GetSearchers().Name(i));
                tsi.Tag = i;
                menuSearchSites.Items.Add(tsi);
            }
            return menuSearchSites;
        }

        private void ChooseSiteMenu(int n)
        {
            ContextMenuStrip sm = BuildSearchMenu();
            if (n == 1)
                sm.Show(bnWTWChooseSite, new Point(0, 0));
            else if (n == 0)
                sm.Show(bnAIOWhichSearch, new Point(0, 0));
        }

        private void bnWTWChooseSite_Click(object sender, System.EventArgs e)
        {
            ChooseSiteMenu(1);
        }

        private void bnEpGuideChooseSearch_Click(object sender, System.EventArgs e)
        {
            ChooseSiteMenu(2);
        }

        private void FillShowLists()
        {
            Season currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
            ShowItem currentSI = TreeNodeToShowItem(MyShowTree.SelectedNode);

            ShowItemList expanded = new ShowItemList();
            foreach (TreeNode n in MyShowTree.Nodes)
                if (n.IsExpanded)
                    expanded.Add(TreeNodeToShowItem(n));

            MyShowTree.BeginUpdate();

            MyShowTree.Nodes.Clear();

            System.Collections.Generic.List<ShowItem> sil = mDoc.GetShowItems(true);
            foreach (ShowItem si in sil)
            {
                TreeNode tvn = AddShowItemToTree(si);
                if (expanded.Contains(si))
                    tvn.Expand();
            }
            mDoc.UnlockShowItems();

            foreach (ShowItem si in expanded)
                foreach (TreeNode n in MyShowTree.Nodes)
                    if (TreeNodeToShowItem(n) == si)
                        n.Expand();

            if (currentSeas != null)
                SelectSeason(currentSeas);
            else
                if (currentSI != null)
                    SelectShow(currentSI);

            MyShowTree.EndUpdate();
        }

        private static string QuickStartGuide()
        {
            return "http://tvrename.com/quickstart.html";
        }

        private void ShowQuickStartGuide()
        {
            tabControl1.SelectTab(tbMyShows);
            epGuideHTML.Navigate(QuickStartGuide());
        }

        private void FillEpGuideHTML()
        {
            if (MyShowTree.Nodes.Count == 0)
                ShowQuickStartGuide();
            else
            {
                TreeNode n = MyShowTree.SelectedNode;
                FillEpGuideHTML(n);
            }
        }

        private ShowItem TreeNodeToShowItem(TreeNode n)
        {
            if (n == null)
                return null;

            ShowItem si = n.Tag as ShowItem;
            if (si != null)
                return si;

            ProcessedEpisode pe = n.Tag as ProcessedEpisode;
            if (pe != null)
                return pe.SI;

            Season seas = n.Tag as Season;
            if (seas != null)
            {
                if (seas.Episodes.Count > 0)
                {
                    int tvdbcode = seas.TheSeries.TVDBCode;
                    foreach (ShowItem si2 in mDoc.GetShowItems(true))
                        if (si2.TVDBCode == tvdbcode)
                        {
                            mDoc.UnlockShowItems();
                            return si2;
                        }
                    mDoc.UnlockShowItems();
                }
            }

            return null;
        }

        private static Season TreeNodeToSeason(TreeNode n)
        {
            if (n == null)
                return null;

            Season seas = n.Tag as Season;
            return seas;

        }

        private void FillEpGuideHTML(TreeNode n)
        {
            if (n == null)
            {
                FillEpGuideHTML(null, -1);
                return;
            }

            ProcessedEpisode pe = n.Tag as ProcessedEpisode;
            if (pe != null)
            {
                FillEpGuideHTML(pe.SI, pe.SeasonNumber);
                return;
            }

            Season seas = TreeNodeToSeason(n);
            if (seas != null)
            {
                // we have a TVDB season, but need to find the equiavlent one in our local processed episode collection
                if (seas.Episodes.Count > 0)
                {
                    int tvdbcode = seas.TheSeries.TVDBCode;
                    foreach (ShowItem si in mDoc.GetShowItems(true))
                    {
                        if (si.TVDBCode == tvdbcode)
                        {
                            mDoc.UnlockShowItems();
                            FillEpGuideHTML(si, seas.SeasonNumber);
                            return;
                        }
                    }
                    mDoc.UnlockShowItems();

                    if (pe != null)
                    {
                        FillEpGuideHTML(pe.SI, -1);
                        return;
                    }
                }
                FillEpGuideHTML(null, -1);
                return;
            }

            FillEpGuideHTML(TreeNodeToShowItem(n), -1);
        }


        private void FillEpGuideHTML(ShowItem si, int snum)
        {
            if (tabControl1.SelectedTab != tbMyShows)
                return;

            if (si == null)
            {
                SetGuideHTMLbody("");
                return;
            }
            TheTVDB db = mDoc.GetTVDB(true, "FillEpGuideHTML");
            SeriesInfo ser = db.GetSeries(si.TVDBCode);

            if (ser == null)
            {
                SetGuideHTMLbody("Not downloaded, or not available");
                return;
            }

            string body = "";

            StringList skip = new StringList();
            skip.Add("Actors");
            skip.Add("banner");
            skip.Add("Overview");
            skip.Add("Airs_Time");
            skip.Add("Airs_DayOfWeek");
            skip.Add("fanart");
            skip.Add("poster");
            skip.Add("zap2it_id");

            if ((snum >= 0) && (ser.Seasons.ContainsKey(snum)))
            {
                if (!string.IsNullOrEmpty(ser.GetItem("banner")) && !string.IsNullOrEmpty(db.BannerMirror))
                    body += "<img width=758 height=140 src=\"" + db.BannerMirror + "/banners/" + ser.GetItem("banner") + "\"><br/>";

                Season s = ser.Seasons[snum];

                ProcessedEpisodeList eis = null;
                // int snum = s.SeasonNumber;
                if (si.SeasonEpisodes.ContainsKey(snum))
                    eis = si.SeasonEpisodes[snum]; // use processed episodes if they are available
                else
                    eis = ShowItem.ProcessedListFromEpisodes(s.Episodes, si);


                string seasText = snum == 0 ? "Specials" : ("Season " + snum.ToString());
                if ((eis.Count > 0) && (eis[0].SeasonID > 0))
                    seasText = " - <A HREF=\"" + db.WebsiteURL(si.TVDBCode, eis[0].SeasonID, false) + "\">" + seasText + "</a>";
                else
                    seasText = " - " + seasText;

                body += "<h1><A HREF=\"" + db.WebsiteURL(si.TVDBCode, -1, true) + "\">" + si.ShowName() + "</A>" + seasText + "</h1>";

                foreach (ProcessedEpisode ei in eis)
                {
                    string epl = ei.NumsAsString();

                    // http://www.thetvdb.com/?tab=episode&seriesid=73141&seasonid=5356&id=108303&lid=7
                    string episodeURL = "http://www.thetvdb.com/?tab=episode&seriesid=" + ei.SeriesID + "&seasonid=" + ei.SeasonID + "&id=" + ei.EpisodeID.ToString();

                    body += "<A href=\"" + episodeURL + "\" name=\"ep" + epl + "\">"; // anchor
                    body += "<b>" + CustomName.NameForNoExt(ei, CustomName.OldNStyle(6)) + "</b>";
                    body += "</A>"; // anchor
                    if (si.UseSequentialMatch && (ei.OverallNumber != -1))
                        body += " (#" + ei.OverallNumber.ToString() + ")";

                    body += " <A HREF=\"" + mDoc.Settings.BTSearchURL(ei) + "\" class=\"search\">Search</A>";

                    System.Collections.Generic.List<System.IO.FileInfo> fl = mDoc.FindEpOnDisk(ei);
                    if (fl != null)
                    {
                        foreach (FileInfo fi in fl)
                            body += " <A HREF=\"file://" + fi.FullName + "\" class=\"search\">Watch</A>";
                    }

                    DateTime? dt = ei.GetAirDateDT(true);
                    if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                        body += "<p>" + dt.Value.ToShortDateString() + " (" + ei.HowLong() + ")";

                    body += "<p><p>";

                    if (mDoc.Settings.ShowEpisodePictures)
                    {
                        body += "<table><tr>";
                        body += "<td width=100% valign=top>" + ei.Overview + "</td><td width=300 height=225>";
                        // 300x168 / 300x225
                        if (!string.IsNullOrEmpty(ei.GetItem("filename")))
                            body += "<img src=" + db.BannerMirror + "/banners/_cache/" + ei.GetItem("filename") + ">";
                        body += "</td></tr></table>";
                    }
                    else
                        body += ei.Overview;

                    body += "<p><hr><p>";
                } // for each episode in this season
            }
            else
            {
                // no epnum specified, just show an overview
                if ((!string.IsNullOrEmpty(ser.GetItem("banner"))) && (!string.IsNullOrEmpty(db.BannerMirror)))
                    body += "<img width=758 height=140 src=\"" + db.BannerMirror + "/banners/" + ser.GetItem("banner") + "\"><br/>";

                body += "<h1><A HREF=\"" + db.WebsiteURL(si.TVDBCode, -1, true) + "\">" + si.ShowName() + "</A> " + "</h1>";

                body += "<h2>Overview</h2>" + ser.GetItem("Overview");

                string actors = ser.GetItem("Actors");
                if (!string.IsNullOrEmpty(actors))
                {
                    bool first = true;
                    foreach (string aa in actors.Split('|'))
                    {
                        if (!string.IsNullOrEmpty(aa))
                        {
                            if (!first)
                                body += ", ";
                            else
                                body += "<h2>Actors</h2>";
                            body += "<A HREF=\"http://www.imdb.com/find?s=nm&q=" + aa + "\">" + aa + "</a>";
                            first = false;
                        }
                    }
                }

                string airsTime = ser.GetItem("Airs_Time");
                string airsDay = ser.GetItem("Airs_DayOfWeek");
                if ((!string.IsNullOrEmpty(airsTime)) && (!string.IsNullOrEmpty(airsDay)))
                {
                    body += "<h2>Airs</h2> " + airsTime + " " + airsDay;
                    string net = ser.GetItem("Network");
                    if (!string.IsNullOrEmpty(net))
                    {
                        skip.Add("Network");
                        body += ", " + net;
                    }
                }

                bool firstInfo = true;
                foreach (System.Collections.Generic.KeyValuePair<string, string> kvp in ser.Items)
                {
                    if (firstInfo)
                    {
                        body += "<h2>Information<table border=0>";
                        firstInfo = false;
                    }
                    if (!skip.Contains(kvp.Key))
                    {
                        if (kvp.Key == "SeriesID")
                            body += "<tr><td width=120px>tv.com</td><td><A HREF=\"http://www.tv.com/show/" + kvp.Value + "/summary.html\">Visit</a></td></tr>";
                        else if (kvp.Key == "IMDB_ID")
                            body += "<tr><td width=120px>imdb.com</td><td><A HREF=\"http://www.imdb.com/title/" + kvp.Value + "\">Visit</a></td></tr>";
                        else
                            body += "<tr><td width=120px>" + kvp.Key + "</td><td>" + kvp.Value + "</td></tr>";
                    }
                }
                if (!firstInfo)
                    body += "</table>";

            }
            db.Unlock("FillEpGuideHTML");
            SetGuideHTMLbody(body);
        } // FillEpGuideHTML

        public static string EpGuidePath()
        {
            string tp = Path.GetTempPath();
            return tp + "tvrenameepguide.html";
        }

        public static string EpGuideURLBase()
        {
            return "file://" + EpGuidePath();
        }

        public void SetGuideHTMLbody(string body)
        {
            System.Drawing.Color col = System.Drawing.Color.FromName("ButtonFace");

            string css = "* { font-family: Tahoma, Arial; font-size 10pt; } " + "a:link { color: black } " + "a:visited { color:black } " + "a:hover { color:#000080 } " + "a:active { color:black } " + "a.search:link { color: #800000 } " + "a.search:visited { color:#800000 } " + "a.search:hover { color:#000080 } " + "a.search:active { color:#800000 } " + "* {background-color: #" + col.R.ToString("X2") + col.G.ToString("X2") + col.B.ToString("X2") + "}" + "* { color: black }";

            string html = "<html><head><STYLE type=\"text/css\">" + css + "</style>";

            html += "</head><body>";
            html += body;
            html += "</body></html>";

            epGuideHTML.Navigate("about:blank"); // make it close any file it might have open

            string path = EpGuidePath();

            BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create));
            bw.Write(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(html));
            bw.Close();

            epGuideHTML.Navigate(EpGuideURLBase());
        }

        public void TVDBFor(ProcessedEpisode e)
        {
            if (e == null)
                return;

            TVDoc.SysOpen(mDoc.GetTVDB(false, "").WebsiteURL(e.SI.TVDBCode, e.SeasonID, false));
        }
        public void TVDBFor(Season seas)
        {
            if (seas == null)
                return;

            TVDoc.SysOpen(mDoc.GetTVDB(false, "").WebsiteURL(seas.TheSeries.TVDBCode, -1, false));
        }
        public void TVDBFor(ShowItem si)
        {
            if (si == null)
                return;

            TVDoc.SysOpen(mDoc.GetTVDB(false, "").WebsiteURL(si.TVDBCode, -1, false));
        }
        //            
        //			void RenamingCheckSpecific(ShowItem ^si)
        //			{
        //			MoreBusy();
        //			mDoc->DoRenameCheck(this->SetProgress, si);
        //			FillShowLists();
        //            //FillRenameList();
        //			FillAIOList();
        //			LessBusy();
        //			tabControl1->SelectedIndex = 1;
        //			}
        //			void MissingCheckSpecific(ShowItem ^si)
        //			{
        //			MoreBusy();
        //			mDoc->DoMissingCheck(this->SetProgress, si);
        //			FillShowLists();
        //            //FillMissingList();
        //			FillAIOList();
        //			LessBusy();
        //			if (si != nullptr)
        //			tabControl1->SelectedIndex = 1;
        //			}
        //			
        public void menuSearchSites_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            mDoc.SetSearcher((int)(e.ClickedItem.Tag));
            UpdateSearchButton();

        }
        public void bnWhenToWatchCheck_Click(object sender, System.EventArgs e)
        {
            RefreshWTW(true);
        }

        public void FillWhenToWatchList()
        {
            mInternalChange++;
            lvWhenToWatch.BeginUpdate();

            int dd = mDoc.Settings.WTWRecentDays;

            lvWhenToWatch.Groups[0].Header = "Aired in the last " + dd.ToString() + " day" + ((dd == 1) ? "" : "s");

            // try to maintain selections if we can
            ProcessedEpisodeList selections = new ProcessedEpisodeList();
            foreach (ListViewItem lvi in lvWhenToWatch.SelectedItems)
                selections.Add((ProcessedEpisode)(lvi.Tag));

            Season currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
            ShowItem currentSI = TreeNodeToShowItem(MyShowTree.SelectedNode);


            lvWhenToWatch.Items.Clear();

            System.Collections.Generic.List<DateTime> bolded = new System.Collections.Generic.List<DateTime>();

            foreach (ShowItem si in mDoc.GetShowItems(true))
            {
                if (!si.ShowNextAirdate)
                    continue;

                foreach (System.Collections.Generic.KeyValuePair<int, ProcessedEpisodeList> kvp in si.SeasonEpisodes)
                {
                    if (si.IgnoreSeasons.Contains(kvp.Key))
                        continue; // ignore this season

                    ProcessedEpisodeList eis = kvp.Value;

                    bool nextToAirFound = false;

                    foreach (ProcessedEpisode ei in eis)
                    {
                        DateTime? dt = ei.GetAirDateDT(true);
                        if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                        {
                            TimeSpan ts = dt.Value.Subtract(DateTime.Now);
                            if (ts.TotalHours >= (-24 * dd)) // in the future (or fairly recent)
                            {
                                bolded.Add(dt.Value);
                                if ((ts.TotalHours >= 0) && (!nextToAirFound))
                                {
                                    nextToAirFound = true;
                                    ei.NextToAir = true;
                                }
                                else
                                    ei.NextToAir = false;

                                ListViewItem lvi = new System.Windows.Forms.ListViewItem();
                                lvi.Text = "";
                                for (int i = 0; i < 7; i++)
                                    lvi.SubItems.Add("");

                                UpdateWTW(ei, lvi);

                                lvWhenToWatch.Items.Add(lvi);

                                foreach (ProcessedEpisode pe in selections)
                                    if (pe.SameAs(ei))
                                    {
                                        lvi.Selected = true;
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
            mDoc.UnlockShowItems();
            lvWhenToWatch.Sort();

            lvWhenToWatch.EndUpdate();
            calCalendar.BoldedDates = bolded.ToArray();


            if (currentSeas != null)
                SelectSeason(currentSeas);
            else
                if (currentSI != null)
                    SelectShow(currentSI);

            UpdateToolstripWTW();
            mInternalChange--;
        }

        public void lvWhenToWatch_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            int col = e.Column;
            // 3 4, or 6 = do date sort on 3
            // 1 or 2 = number sort
            // 5 = day sort
            // all others, text sort

            if (col == 6) // straight sort by date
            {
                lvWhenToWatch.ListViewItemSorter = new DateSorterWTW();
                lvWhenToWatch.ShowGroups = false;
            }
            else if ((col == 3) || (col == 4))
            {
                lvWhenToWatch.ListViewItemSorter = new DateSorterWTW();
                lvWhenToWatch.ShowGroups = true;
            }
            else
            {
                lvWhenToWatch.ShowGroups = false;
                if ((col == 1) || (col == 2))
                    lvWhenToWatch.ListViewItemSorter = new NumberAsTextSorter(col);
                else if (col == 5)
                    lvWhenToWatch.ListViewItemSorter = new DaySorter(col);
                else
                    lvWhenToWatch.ListViewItemSorter = new TextSorter(col);
            }
            lvWhenToWatch.Sort();
        }

        public void lvWhenToWatch_Click(object sender, System.EventArgs e)
        {
            if (lvWhenToWatch.SelectedIndices.Count == 0)
            {
                txtWhenToWatchSynopsis.Text = "";
                return;
            }
            int n = lvWhenToWatch.SelectedIndices[0];

            ProcessedEpisode ei = (ProcessedEpisode)(lvWhenToWatch.Items[n].Tag);
            txtWhenToWatchSynopsis.Text = ei.Overview;

            mInternalChange++;
            DateTime? dt = ei.GetAirDateDT(true);
            if (dt != null)
            {
                calCalendar.SelectionStart = (DateTime)dt;
                calCalendar.SelectionEnd = (DateTime)dt;
            }
            mInternalChange--;

            if (mDoc.Settings.AutoSelectShowInMyShows)
                GotoEpguideFor(ei, false);
        }
        public void lvWhenToWatch_DoubleClick(object sender, System.EventArgs e)
        {
            if (lvWhenToWatch.SelectedItems.Count > 0)
            {
                ProcessedEpisode ei = (ProcessedEpisode)(lvWhenToWatch.SelectedItems[0].Tag);
                System.Collections.Generic.List<System.IO.FileInfo> fl = mDoc.FindEpOnDisk(ei);
                if ((fl != null) && (fl.Count > 0))
                {
                    TVDoc.SysOpen(fl[0].FullName);
                    return;
                }
            }

            bnWTWBTSearch_Click(null, null);
        }
        public void calCalendar_DateSelected(object sender, System.Windows.Forms.DateRangeEventArgs e)
        {
            if (mInternalChange != 0)
                return;

            DateTime dt = calCalendar.SelectionStart;
            for (int i = 0; i < lvWhenToWatch.Items.Count; i++)
                lvWhenToWatch.Items[i].Selected = false;

            bool first = true;

            for (int i = 0; i < lvWhenToWatch.Items.Count; i++)
            {
                ListViewItem lvi = lvWhenToWatch.Items[i];
                ProcessedEpisode ei = (ProcessedEpisode)(lvi.Tag);
                DateTime? dt2 = ei.GetAirDateDT(true);
                if (dt2 != null)
                {
                    double h = dt2.Value.Subtract(dt).TotalHours;
                    if ((h >= 0) && (h < 24.0))
                    {
                        lvi.Selected = true;
                        if (first)
                        {
                            lvi.EnsureVisible();
                            first = false;
                        }
                    }
                }
            }
            lvWhenToWatch.Focus();
        }

        public void bnEpGuideRefresh_Click(object sender, System.EventArgs e)
        {
            bnWhenToWatchCheck_Click(null, null); // close enough!
            FillShowLists();
        }

        public void RefreshWTW(bool doDownloads)
        {
            if (doDownloads)
                if (!mDoc.DoDownloadsFG())
                    return;

            mInternalChange++;
            mDoc.DoWhenToWatch(true);
            FillShowLists();
            FillWhenToWatchList();
            mInternalChange--;

            mDoc.WriteUpcomingRSS();
        }

        public void refreshWTWTimer_Tick(object sender, System.EventArgs e)
        {
            if (Busy == 0)
                RefreshWTW(false);
        }
        public void UpdateToolstripWTW()
        {
            // update toolstrip text too
            ProcessedEpisodeList next1 = mDoc.NextNShows(1, 36500);

            tsNextShowTxt.Text = "Next airing: ";
            if ((next1 != null) && (next1.Count >= 1))
            {
                ProcessedEpisode ei = next1[0];
                tsNextShowTxt.Text += CustomName.NameForNoExt(ei, CustomName.OldNStyle(1)) + ", " + ei.HowLong() + " (" + ei.DayOfWeek() + ", " + ei.TimeOfDay() + ")";
            }
            else
                tsNextShowTxt.Text += "---";
        }
        public void bnWTWBTSearch_Click(object sender, System.EventArgs e)
        {
            foreach (ListViewItem lvi in lvWhenToWatch.SelectedItems)
                mDoc.DoBTSearch((ProcessedEpisode)(lvi.Tag));
        }
        public void epGuideHTML_Navigating(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.AbsoluteUri;
            if (url.Contains("tvrenameepguide.html#ep"))
                return; // don't intercept
            if (url.EndsWith("tvrenameepguide.html"))
                return; // don't intercept
            if (url.CompareTo("about:blank") == 0)
                return; // don't intercept about:blank
            if (url == QuickStartGuide())
                return; // let the quickstartguide be shown

            if ((url.Substring(0, 7).CompareTo("http://") == 0) || (url.Substring(0, 7).CompareTo("file://") == 0))
            {
                e.Cancel = true;
                TVDoc.SysOpen(e.Url.AbsoluteUri);
            }

        }
        public void notifyIcon1_Click(object sender, MouseEventArgs e)
        {
            // double-click of notification icon causes a click then doubleclick event, 
            // so we need to do a timeout before showing the single click's popup
            tmrShowUpcomingPopup.Start();
        }
        public void tmrShowUpcomingPopup_Tick(object sender, System.EventArgs e)
        {
            tmrShowUpcomingPopup.Stop();
            UpcomingPopup UP = new UpcomingPopup(mDoc);
            UP.Show();

        }
        public void notifyIcon1_DoubleClick(object sender, MouseEventArgs e)
        {
            tmrShowUpcomingPopup.Stop();
            if (!mDoc.Settings.ShowInTaskbar)
                this.Show();
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;
            this.Activate();
        }
        public void buyMeADrinkToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            BuyMeADrink bmad = new BuyMeADrink();
            bmad.ShowDialog();
        }

        public void GotoEpguideFor(ShowItem si, bool changeTab)
        {
            if (changeTab)
                tabControl1.SelectTab(tbMyShows);
            FillEpGuideHTML(si, -1);
        }

        public void GotoEpguideFor(Episode ep, bool changeTab)
        {
            if (changeTab)
                tabControl1.SelectTab(tbMyShows);

            SelectSeason(ep.TheSeason);
        }

        public void RightClickOnMyShows(ShowItem si, Point pt)
        {
            mLastShowClicked = si;
            mLastEpClicked = null;
            mLastSeasonClicked = null;
            mLastAIOClicked = null;
            BuildRightClickMenu(pt);

        }
        public void RightClickOnMyShows(Season seas, Point pt)
        {
            mLastShowClicked = mDoc.GetShowItem(seas.TheSeries.TVDBCode);
            mLastEpClicked = null;
            mLastSeasonClicked = seas;
            mLastAIOClicked = null;
            BuildRightClickMenu(pt);
        }
        public void RightClickOnShow(ProcessedEpisode ep, Point pt)
        {
            mLastEpClicked = ep;
            mLastShowClicked = ep != null ? ep.SI : null;
            mLastSeasonClicked = ep != null ? ep.TheSeason : null;
            mLastAIOClicked = null;
            BuildRightClickMenu(pt);
        }

        public void MenuGuideAndTVDB(bool addSep)
        {
            ShowItem si = mLastShowClicked;
            Season seas = mLastSeasonClicked;
            ProcessedEpisode ep = mLastEpClicked;
            ToolStripMenuItem tsi;

            if (si != null)
            {
                if (addSep)
                {
                    showRightClickMenu.Items.Add(new ToolStripSeparator());
                    addSep = false;
                }
                tsi = new ToolStripMenuItem("Episode Guide");
                tsi.Tag = (int)RightClickCommands.kEpisodeGuideForShow;
                showRightClickMenu.Items.Add(tsi);
            }

            if (ep != null)
            {
                if (addSep)
                {
                    showRightClickMenu.Items.Add(new ToolStripSeparator());
                    addSep = false;
                }
                tsi = new ToolStripMenuItem("Visit thetvdb.com");
                tsi.Tag = (int)RightClickCommands.kVisitTVDBEpisode;
                showRightClickMenu.Items.Add(tsi);
            }
            else if (seas != null)
            {
                if (addSep)
                {
                    showRightClickMenu.Items.Add(new ToolStripSeparator());
                    addSep = false;
                }
                tsi = new ToolStripMenuItem("Visit thetvdb.com");
                tsi.Tag = (int)RightClickCommands.kVisitTVDBSeason;
                showRightClickMenu.Items.Add(tsi);
            }
            else if (si != null)
            {
                if (addSep)
                {
                    showRightClickMenu.Items.Add(new ToolStripSeparator());
                    addSep = false;
                }
                tsi = new ToolStripMenuItem("Visit thetvdb.com");
                tsi.Tag = (int)RightClickCommands.kVisitTVDBSeries;
                showRightClickMenu.Items.Add(tsi);
            }

        }

        public void MenuShowAndEpisodes()
        {
            ShowItem si = mLastShowClicked;
            Season seas = mLastSeasonClicked;
            ProcessedEpisode ep = mLastEpClicked;
            ToolStripMenuItem tsi;


            if (si != null)
            {
                tsi = new ToolStripMenuItem("Force Refresh");
                tsi.Tag = (int)RightClickCommands.kForceRefreshSeries;
                showRightClickMenu.Items.Add(tsi);
                ToolStripSeparator tss = new ToolStripSeparator();
                showRightClickMenu.Items.Add(tss);
                tsi = new ToolStripMenuItem("Scan");
                tsi.Tag = (int)RightClickCommands.kScanSpecificSeries;
                showRightClickMenu.Items.Add(tsi);
                //tsi = gcnew ToolStripMenuItem("Renaming Check");     tsi->Tag = (int)kRenamingCheckSeries; showRightClickMenu->Items->Add(tsi);
                tsi = new ToolStripMenuItem("When to Watch");
                tsi.Tag = (int)RightClickCommands.kWhenToWatchSeries;
                showRightClickMenu.Items.Add(tsi);
            }

            if (ep != null)
            {
                System.Collections.Generic.List<System.IO.FileInfo> fl = mDoc.FindEpOnDisk(ep);
                if (fl != null)
                {
                    if (fl.Count > 0)
                    {
                        ToolStripSeparator tss = new ToolStripSeparator();
                        showRightClickMenu.Items.Add(tss);

                        int n = mLastFL.Count;
                        foreach (FileInfo fi in fl)
                        {
                            mLastFL.Add(fi);
                            tsi = new ToolStripMenuItem("Watch: " + fi.FullName);
                            tsi.Tag = (int)RightClickCommands.kWatchBase + n;
                            showRightClickMenu.Items.Add(tsi);
                        }
                    }
                }
            }
            else if ((seas != null) && (si != null))
            {
                // for each episode in season, find it on disk
                bool first = true;
                foreach (ProcessedEpisode epds in si.SeasonEpisodes[seas.SeasonNumber])
                {
                    System.Collections.Generic.List<System.IO.FileInfo> fl = mDoc.FindEpOnDisk(epds);
                    if ((fl != null) && (fl.Count > 0))
                    {
                        if (first)
                        {
                            ToolStripSeparator tss = new ToolStripSeparator();
                            showRightClickMenu.Items.Add(tss);
                            first = false;
                        }

                        int n = mLastFL.Count;
                        foreach (FileInfo fi in fl)
                        {
                            mLastFL.Add(fi);
                            tsi = new ToolStripMenuItem("Watch: " + fi.FullName);
                            tsi.Tag = (int)RightClickCommands.kWatchBase + n;
                            showRightClickMenu.Items.Add(tsi);
                        }
                    }
                }
            }
        }

        public void MenuFolders(LVResults lvr)
        {
            ShowItem si = mLastShowClicked;
            Season seas = mLastSeasonClicked;
            ProcessedEpisode ep = mLastEpClicked;
            ToolStripMenuItem tsi;
            StringList added = new StringList();

            if (ep != null)
            {
                if (ep.SI.AllFolderLocations(mDoc.Settings).ContainsKey(ep.SeasonNumber))
                {
                    int n = mFoldersToOpen.Count;
                    bool first = true;
                    foreach (string folder in ep.SI.AllFolderLocations(mDoc.Settings)[ep.SeasonNumber])
                    {
                        if ((!string.IsNullOrEmpty(folder)) && Directory.Exists(folder))
                        {
                            if (first)
                            {
                                ToolStripSeparator tss = new ToolStripSeparator();
                                showRightClickMenu.Items.Add(tss);
                                first = false;
                            }

                            tsi = new ToolStripMenuItem("Open: " + folder);
                            added.Add(folder);
                            mFoldersToOpen.Add(folder);
                            tsi.Tag = (int)RightClickCommands.kOpenFolderBase + n;
                            n++;
                            showRightClickMenu.Items.Add(tsi);
                        }
                    }
                }
            }
            else if ((seas != null) && (si != null) && (si.AllFolderLocations(mDoc.Settings).ContainsKey(seas.SeasonNumber)))
            {
                int n = mFoldersToOpen.Count;
                bool first = true;
                foreach (string folder in si.AllFolderLocations(mDoc.Settings)[seas.SeasonNumber])
                {
                    if ((!string.IsNullOrEmpty(folder)) && Directory.Exists(folder) && !added.Contains(folder))
                    {
                        added.Add(folder); // don't show the same folder more than once
                        if (first)
                        {
                            ToolStripSeparator tss = new ToolStripSeparator();
                            showRightClickMenu.Items.Add(tss);
                            first = false;
                        }

                        tsi = new ToolStripMenuItem("Open: " + folder);
                        mFoldersToOpen.Add(folder);
                        tsi.Tag = (int)RightClickCommands.kOpenFolderBase + n;
                        n++;
                        showRightClickMenu.Items.Add(tsi);
                    }
                }
            }
            else if (si != null)
            {
                int n = mFoldersToOpen.Count;
                bool first = true;

                foreach (System.Collections.Generic.KeyValuePair<int, StringList> kvp in si.AllFolderLocations(mDoc.Settings))
                {
                    foreach (string folder in kvp.Value)
                    {
                        if ((!string.IsNullOrEmpty(folder)) && Directory.Exists(folder) && !added.Contains(folder))
                        {
                            added.Add(folder); // don't show the same folder more than once
                            if (first)
                            {
                                ToolStripSeparator tss = new ToolStripSeparator();
                                showRightClickMenu.Items.Add(tss);
                                first = false;
                            }

                            tsi = new ToolStripMenuItem("Open: " + folder);
                            mFoldersToOpen.Add(folder);
                            tsi.Tag = (int)RightClickCommands.kOpenFolderBase + n;
                            n++;
                            showRightClickMenu.Items.Add(tsi);
                        }
                    }
                }
            }

            if (lvr != null) // add folders for selected Scan items
            {
                int n = mFoldersToOpen.Count;
                bool first = true;

                foreach (AIOItem aio in lvr.FlatList)
                {
                    string folder = aio.TargetFolder();
                    if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder) && !added.Contains(folder))
                    {
                        added.Add(folder); // don't show the same folder more than once
                        if (first)
                        {
                            ToolStripSeparator tss = new ToolStripSeparator();
                            showRightClickMenu.Items.Add(tss);
                            first = false;
                        }

                        tsi = new ToolStripMenuItem("Open: " + folder);
                        mFoldersToOpen.Add(folder);
                        tsi.Tag = (int)RightClickCommands.kOpenFolderBase + n;
                        n++;
                        showRightClickMenu.Items.Add(tsi);
                    }
                }
            }
        }

        public void BuildRightClickMenu(Point pt)
        {
            showRightClickMenu.Items.Clear();
            mFoldersToOpen = new StringList();
            mLastFL = new System.Collections.Generic.List<System.IO.FileInfo>();

            MenuGuideAndTVDB(false);
            MenuShowAndEpisodes();
            MenuFolders(null);

            showRightClickMenu.Show(pt);
        }
        public void showRightClickMenu_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            showRightClickMenu.Close();
            RightClickCommands n = (RightClickCommands)e.ClickedItem.Tag;
            switch (n)
            {
                case RightClickCommands.kEpisodeGuideForShow: // epguide
                    if (mLastEpClicked != null)
                        GotoEpguideFor(mLastEpClicked, true);
                    else
                        GotoEpguideFor(mLastShowClicked, true);
                    break;

                case RightClickCommands.kVisitTVDBEpisode: // thetvdb.com
                    {
                        TVDBFor(mLastEpClicked);
                        break;
                    }

                case RightClickCommands.kVisitTVDBSeason:
                    {
                        TVDBFor(mLastSeasonClicked);
                        break;
                    }

                case RightClickCommands.kVisitTVDBSeries:
                    {
                        TVDBFor(mLastShowClicked);
                        break;
                    }
                case RightClickCommands.kScanSpecificSeries:
                    {
                        if (mLastShowClicked != null)
                        {
                            Scan(mLastShowClicked);
                            tabControl1.SelectTab(tbAllInOne);
                        }
                        break;
                    }
                //                    
                //					case kMissingCheckSeries:
                //					{
                //					if ( mLastShowClicked != nullptr)
                //					MissingCheckSpecific(mLastShowClicked);
                //					break;
                //					}
                //					case kRenamingCheckSeries:
                //					{
                //					if ( mLastShowClicked != nullptr)
                //					RenamingCheckSpecific(mLastShowClicked);
                //					break;
                //					}
                case RightClickCommands.kWhenToWatchSeries: // when to watch
                    {
                        int code = -1;
                        if (mLastEpClicked != null)
                            code = mLastEpClicked.TheSeries.TVDBCode;
                        if (mLastShowClicked != null)
                            code = mLastShowClicked.TVDBCode;

                        if (code != -1)
                        {
                            tabControl1.SelectTab(tbWTW);

                            for (int i = 0; i < lvWhenToWatch.Items.Count; i++)
                                lvWhenToWatch.Items[i].Selected = false;

                            for (int i = 0; i < lvWhenToWatch.Items.Count; i++)
                            {
                                ListViewItem lvi = lvWhenToWatch.Items[i];
                                ProcessedEpisode ei = (ProcessedEpisode)(lvi.Tag);
                                if ((ei != null) && (ei.TheSeries.TVDBCode == code))
                                    lvi.Selected = true;
                            }
                            lvWhenToWatch.Focus();
                        }
                        break;
                    }
                case RightClickCommands.kForceRefreshSeries:
                    ForceRefresh(mLastShowClicked);
                    break;
                case RightClickCommands.kBTSearchFor:
                    {
                        foreach (ListViewItem lvi in lvAIO.SelectedItems)
                        {
                            AIOMissing m = (AIOMissing)(lvi.Tag);
                            if (m != null)
                                mDoc.DoBTSearch(m.PE);
                        }
                    }
                    break;
                case RightClickCommands.kAIOAction:
                    AIOAction(false);
                    break;
                case RightClickCommands.kAIOBrowseForFile:
                    {
                        AIOMissing mi = (AIOMissing)(mLastAIOClicked);
                        if (mi != null)
                        {
                            // browse for mLastAIOClicked
                            openFile.Filter = "Video Files|" + mDoc.Settings.GetVideoExtensionsString().Replace(".", "*.") + "|All Files (*.*)|*.*";
                            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                // make new AIOItem for copying/moving to specified location
                                FileInfo from = new FileInfo(openFile.FileName);
                                mDoc.TheAIOList.Add(new AIOCopyMoveRename(mDoc.Settings.LeaveOriginals ? AIOCopyMoveRename.Op.Copy : AIOCopyMoveRename.Op.Move, from, new FileInfo(mi.TheFileNoExt + from.Extension), mi.PE));
                                // and remove old Missing item
                                mDoc.TheAIOList.Remove(mLastAIOClicked);
                                mLastAIOClicked = null;
                                FillAIOList();
                            }
                        }

                        break;
                    }
                case RightClickCommands.kAIOIgnore:
                    IgnoreSelected();
                    break;
                case RightClickCommands.kAIOIgnoreSeason:
                    {
                        // add this season to ignore list for the show selected
                        int snum = mLastAIOClicked.PE.SeasonNumber;

                        if (!mLastAIOClicked.PE.SI.IgnoreSeasons.Contains(snum))
                            mLastAIOClicked.PE.SI.IgnoreSeasons.Add(snum);

                        // remove all other episodes from this season from aio list
                        System.Collections.Generic.List<AIOItem> remove = new System.Collections.Generic.List<AIOItem>();
                        foreach (AIOItem aio in mDoc.TheAIOList)
                            if ((aio.PE != null) && (aio.PE.SeasonNumber == snum))
                                remove.Add(aio);
                        foreach (AIOItem aio in remove)
                            mDoc.TheAIOList.Remove(aio);

                        mLastAIOClicked = null;
                        FillAIOList();
                        break;
                    }
                case RightClickCommands.kAIODelete:
                    AIODeleteSelected();
                    break;
                default:
                    {
                        if ((n >= RightClickCommands.kWatchBase) && (n < RightClickCommands.kOpenFolderBase))
                        {
                            int wn = n - RightClickCommands.kWatchBase;
                            if ((mLastFL != null) && (wn >= 0) && (wn < mLastFL.Count))
                                TVDoc.SysOpen(mLastFL[wn].FullName);
                        }
                        else if (n >= RightClickCommands.kOpenFolderBase)
                        {
                            int fnum = n - RightClickCommands.kOpenFolderBase;

                            if (fnum < mFoldersToOpen.Count)
                            {
                                string folder = mFoldersToOpen[fnum];

                                if (Directory.Exists(folder))
                                    TVDoc.SysOpen(folder);
                            }
                            return;
                        }
                        else
                        {
                            System.Diagnostics.Debug.Fail("Unknown right-click action " + n.ToString());
                        }
                        break;
                    }

            }

            mLastEpClicked = null;
        }
        public void tabControl1_DoubleClick(object sender, System.EventArgs e)
        {
            if (tabControl1.SelectedTab == tbMyShows)
                bnMyShowsRefresh_Click(null, null);
            else if (tabControl1.SelectedTab == tbWTW)
                bnWhenToWatchCheck_Click(null, null);
            else if (tabControl1.SelectedTab == tbAllInOne)
                bnAIOCheck_Click(null, null);
        }
        public void folderRightClickMenu_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            switch ((int)(e.ClickedItem.Tag))
            {
                case 0: // open folder
                    TVDoc.SysOpen(mLastFolderClicked);
                    break;
                default:
                    break;
            }
        }
        public void RightClickOnFolder(string folderPath, Point pt)
        {
            mLastFolderClicked = folderPath;
            folderRightClickMenu.Items.Clear();

            ToolStripMenuItem tsi;
            int n = 0;

            tsi = new ToolStripMenuItem("Open: " + folderPath);
            tsi.Tag = n++;
            folderRightClickMenu.Items.Add(tsi);

            folderRightClickMenu.Show(pt);
        }

        public void lvWhenToWatch_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Right)
                return;
            if (lvWhenToWatch.SelectedItems.Count == 0)
                return;

            Point pt = lvWhenToWatch.PointToScreen(new Point(e.X, e.Y));
            ProcessedEpisode ei = (ProcessedEpisode)(lvWhenToWatch.SelectedItems[0].Tag);
            RightClickOnShow(ei, pt);
        }


        public void preferencesToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            DoPrefs(false);
        }
        public void DoPrefs(bool scanOptions)
        {
            Preferences pref = new Preferences(mDoc, scanOptions);
            if (pref.ShowDialog() == DialogResult.OK)
            {
                mDoc.SetDirty();
                ShowHideNotificationIcon();
                FillWhenToWatchList();
                this.ShowInTaskbar = mDoc.Settings.ShowInTaskbar;
                FillEpGuideHTML();
            }
        }
        public void saveToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            mDoc.WriteXMLSettings();
            mDoc.GetTVDB(false, "").SaveCache();
            SaveLayoutXML();
        }
        public void UI_SizeChanged(object sender, System.EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
                mLastNonMaximizedSize = this.Size;
            if ((this.WindowState == FormWindowState.Minimized) && (!mDoc.Settings.ShowInTaskbar))
                this.Hide();
        }
        public void UI_LocationChanged(object sender, System.EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
                mLastNonMaximizedLocation = this.Location;
        }

        private int LastDLRemaining = 0;
        public void statusTimer_Tick(object sender, System.EventArgs e)
        {
            int n = mDoc.DownloadDone ? 0 : mDoc.DownloadsRemaining;

            txtDLStatusLabel.Visible = (n != 0 || mDoc.Settings.BGDownload);
            if (n != 0)
            {
                txtDLStatusLabel.Text = "Background download: " + mDoc.GetTVDB(false, "").CurrentDLTask;
                backgroundDownloadNowToolStripMenuItem.Enabled = false;
            }
            else
                txtDLStatusLabel.Text = "Background download: Idle";

            if (Busy == 0)
            {
                if ((n == 0) && (LastDLRemaining > 0))
                {
                    // we've just finished a bunch of background downloads
                    mDoc.GetTVDB(false, "").SaveCache();
                    RefreshWTW(false);

                    backgroundDownloadNowToolStripMenuItem.Enabled = true;
                }
                LastDLRemaining = n;
            }
        }
        public void backgroundDownloadToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            mDoc.Settings.BGDownload = !mDoc.Settings.BGDownload;
            backgroundDownloadToolStripMenuItem.Checked = mDoc.Settings.BGDownload;
            statusTimer_Tick(null, null);
            mDoc.SetDirty();

            if (mDoc.Settings.BGDownload)
                BGDownloadTimer.Start();
            else
                BGDownloadTimer.Stop();
        }
        public void BGDownloadTimer_Tick(object sender, System.EventArgs e)
        {
            if (Busy != 0)
            {
                BGDownloadTimer.Interval = 10000; // come back in 10 seconds
                BGDownloadTimer.Start();
                return;
            }
            BGDownloadTimer.Interval = BGDLLongInterval(); // after first time (10 seconds), put up to 60 minutes
            BGDownloadTimer.Start();

            if (mDoc.Settings.BGDownload && mDoc.DownloadDone) // only do auto-download if don't have stuff to do already
            {
                mDoc.StartBGDownloadThread(false);

                statusTimer_Tick(null, null);
            }
        }
        public void backgroundDownloadNowToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (mDoc.Settings.OfflineMode)
            {
                System.Windows.Forms.DialogResult res = MessageBox.Show("Ignore offline mode and download anyway?", "Background Download", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res != System.Windows.Forms.DialogResult.Yes)
                    return;
            }
            BGDownloadTimer.Stop();
            BGDownloadTimer.Start();

            mDoc.StartBGDownloadThread(false);

            statusTimer_Tick(null, null);
        }
        public void offlineOperationToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (!mDoc.Settings.OfflineMode)
                if (MessageBox.Show("Are you sure you wish to go offline?", "TVRename", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;

            mDoc.Settings.OfflineMode = !mDoc.Settings.OfflineMode;
            offlineOperationToolStripMenuItem.Checked = mDoc.Settings.OfflineMode;
            mDoc.SetDirty();
        }

        public void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (tabControl1.SelectedTab == tbMyShows)
                FillEpGuideHTML();

            exportToolStripMenuItem.Enabled = false; //( (tabControl1->SelectedTab == tbMissing) ||
            //														  (tabControl1->SelectedTab == tbFnO) ||
            //														  (tabControl1->SelectedTab == tbRenaming) );
        }

        public void bugReportToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            BugReport br = new BugReport(mDoc);
            br.ShowDialog();
        }
        public void exportToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            //                
            //				if (tabControl1->SelectedTab == tbMissing)
            //				{
            //				if (!MissingListHasStuff())
            //				return;
            //
            //				saveFile->Filter = "CSV Files (*.csv)|*.csv|XML Files (*.xml)|*.xml";
            //				if (saveFile->ShowDialog() != ::DialogResult::OK)
            //				return;
            //
            //				if (saveFile->FilterIndex == 1) // counts from 1
            //				mDoc->ExportMissingCSV(saveFile->FileName);
            //				else if (saveFile->FilterIndex == 2)
            //				mDoc->ExportMissingXML(saveFile->FileName);
            //				}
            //				else if (tabControl1->SelectedTab == tbFnO)
            //				{
            //				saveFile->Filter = "XML Files|*.xml";
            //				if (saveFile->ShowDialog() != ::DialogResult::OK)
            //				return;
            //				mDoc->ExportFOXML(saveFile->FileName);
            //				}
            //				else if (tabControl1->SelectedTab == tbRenaming)
            //				{
            //				saveFile->Filter = "XML Files|*.xml";
            //				if (saveFile->ShowDialog() != ::DialogResult::OK)
            //				return;
            //				mDoc->ExportRenamingXML(saveFile->FileName);
            //				}
            //				
        }


        public void ShowHideNotificationIcon()
        {
            notifyIcon1.Visible = mDoc.Settings.NotificationAreaIcon && !mDoc.HasArg("/hide");
        }
        public void statisticsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            StatsWindow sw = new StatsWindow(mDoc.Stats());
            sw.ShowDialog();
        }


        ////// //////// ////// //////// ////// //////// ////// //////// ////// //////// ////// //////// ////// //////// 


        public TreeNode AddShowItemToTree(ShowItem si)
        {
            TheTVDB db = mDoc.GetTVDB(true, "AddShowItemToTree");
            string name = si.ShowName();

            SeriesInfo ser = db.GetSeries(si.TVDBCode);

            if (string.IsNullOrEmpty(name))
            {
                if (ser != null)
                    name = ser.Name;
                else
                    name = "-- Unknown : " + si.TVDBCode + " --";
            }

            TreeNode n = new TreeNode(name);
            n.Tag = si;


            if (ser != null)
            {
                System.Collections.Generic.List<int> theKeys = new System.Collections.Generic.List<int>();
                // now, go through and number them all sequentially
                foreach (int snum in ser.Seasons.Keys)
                    theKeys.Add(snum);

                theKeys.Sort();

                foreach (int snum in theKeys)
                {
                    string nodeTitle = snum == 0 ? "Specials" : "Season " + snum.ToString();
                    TreeNode n2 = new TreeNode(nodeTitle);
                    if (si.IgnoreSeasons.Contains(snum))
                        n2.ForeColor = Color.Gray;
                    n2.Tag = ser.Seasons[snum];
                    n.Nodes.Add(n2);
                }
            }

            MyShowTree.Nodes.Add(n);

            db.Unlock("AddShowItemToTree");

            return n;
        }

        public void UpdateWTW(ProcessedEpisode pe, ListViewItem lvi)
        {
            lvi.Tag = pe;

            // group 0 = just missed
            //       1 = this week
            //       2 = future / unknown

            DateTime? airdt = pe.GetAirDateDT(true);
            if (airdt == null)
            {
                // TODO: something!
                return;
            }
            DateTime dt = (DateTime)airdt;

            double ttn = (dt.Subtract(DateTime.Now)).TotalHours;

            if (ttn < 0)
                lvi.Group = lvWhenToWatch.Groups[0];
            else if (ttn < (7 * 24))
                lvi.Group = lvWhenToWatch.Groups[1];
            else if (!pe.NextToAir)
                lvi.Group = lvWhenToWatch.Groups[3];
            else
                lvi.Group = lvWhenToWatch.Groups[2];

            int n = 1;
            lvi.Text = pe.SI.ShowName();
            lvi.SubItems[n++].Text = (pe.SeasonNumber != 0) ? pe.SeasonNumber.ToString() : "Special";
            string estr = (pe.EpNum > 0) ? pe.EpNum.ToString() : "";
            if ((pe.EpNum > 0) && (pe.EpNum2 != pe.EpNum) && (pe.EpNum2 > 0))
                estr += "-" + pe.EpNum2.ToString();
            lvi.SubItems[n++].Text = estr;
            lvi.SubItems[n++].Text = dt.ToShortDateString();
            lvi.SubItems[n++].Text = dt.ToString("t");
            lvi.SubItems[n++].Text = dt.ToString("ddd");
            lvi.SubItems[n++].Text = pe.HowLong();
            lvi.SubItems[n++].Text = pe.Name;

            // icon..

            if (airdt.Value.CompareTo(DateTime.Now) < 0) // has aired
            {
                System.Collections.Generic.List<System.IO.FileInfo> fl = mDoc.FindEpOnDisk(pe);
                if ((fl != null) && (fl.Count > 0))
                    lvi.ImageIndex = 0;
                else
                    if (pe.SI.DoMissingCheck)
                        lvi.ImageIndex = 1;
            }

        }

        public void SelectSeason(Season seas)
        {
            foreach (TreeNode n in MyShowTree.Nodes)
                foreach (TreeNode n2 in n.Nodes)
                    if (TreeNodeToSeason(n2) == seas)
                    {
                        n2.EnsureVisible();
                        MyShowTree.SelectedNode = n2;
                        return;
                    }
            FillEpGuideHTML(null);
        }
        public void SelectShow(ShowItem si)
        {
            foreach (TreeNode n in MyShowTree.Nodes)
                if (TreeNodeToShowItem(n) == si)
                {
                    n.EnsureVisible();
                    MyShowTree.SelectedNode = n;
                    //FillEpGuideHTML();
                    return;
                }
            FillEpGuideHTML(null);
        }

        private void bnMyShowsAdd_Click(object sender, System.EventArgs e)
        {
            MoreBusy();
            ShowItem si = new ShowItem(mDoc.GetTVDB(false, ""));
            TheTVDB db = mDoc.GetTVDB(true, "AddShow");
            AddEditShow aes = new AddEditShow(si, db, TZMagic.DefaultTZ());
            System.Windows.Forms.DialogResult dr = aes.ShowDialog();
            db.Unlock("AddShow");
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                mDoc.GetShowItems(true).Add(si);
                mDoc.UnlockShowItems();
                SeriesInfo ser = db.GetSeries(si.TVDBCode);
                if (ser != null)
                    ser.TimeZone = aes.TimeZone;
                ShowAddedOrEdited(true);
                SelectShow(si);
            }
            LessBusy();
        }

        private void ShowAddedOrEdited(bool download)
        {
            mDoc.SetDirty();
            RefreshWTW(download);
            FillShowLists();
        }

        private void bnMyShowsDelete_Click(object sender, System.EventArgs e)
        {
            TreeNode n = MyShowTree.SelectedNode;
            ShowItem si = TreeNodeToShowItem(n);
            if (si == null)
                return;

            System.Windows.Forms.DialogResult res = MessageBox.Show("Remove show \"" + si.ShowName() + "\".  Are you sure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res != System.Windows.Forms.DialogResult.Yes)
                return;

            mDoc.GetShowItems(true).Remove(si);
            mDoc.UnlockShowItems();
            ShowAddedOrEdited(false);
        }
        private void bnMyShowsEdit_Click(object sender, System.EventArgs e)
        {
            TreeNode n = MyShowTree.SelectedNode;
            if (n == null)
                return;
            Season seas = TreeNodeToSeason(n);
            if (seas != null)
            {
                ShowItem si = TreeNodeToShowItem(n);
                if (si != null)
                    EditSeason(si, seas.SeasonNumber);
                return;
            }

            ShowItem si2 = TreeNodeToShowItem(n);
            if (si2 != null)
            {
                EditShow(si2);
                return;
            }
        }

        private void EditSeason(ShowItem si, int seasnum)
        {
            MoreBusy();

            TheTVDB db = mDoc.GetTVDB(true, "EditSeason");
            SeriesInfo ser = db.GetSeries(si.TVDBCode);
            ProcessedEpisodeList pel = TVDoc.GenerateEpisodes(si, ser, seasnum, false);

            EditRules er = new EditRules(si, pel, seasnum, mDoc.Settings.NamingStyle);
            System.Windows.Forms.DialogResult dr = er.ShowDialog();
            db.Unlock("EditSeason");
            if (dr == DialogResult.OK)
            {
                ShowAddedOrEdited(false);
                if (ser != null)
                    SelectSeason(ser.Seasons[seasnum]);
            }

            LessBusy();
        }

        private void EditShow(ShowItem si)
        {
            MoreBusy();
            TheTVDB db = mDoc.GetTVDB(true, "EditShow");
            SeriesInfo ser = db.GetSeries(si.TVDBCode);

            int oldCode = si.TVDBCode;

            AddEditShow aes = new AddEditShow(si, db, ser != null ? ser.TimeZone : TZMagic.DefaultTZ());

            System.Windows.Forms.DialogResult dr = aes.ShowDialog();

            db.Unlock("EditShow");

            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                if (ser != null)
                    ser.TimeZone = aes.TimeZone; // TODO: move into AddEditShow

                ShowAddedOrEdited(si.TVDBCode != oldCode);
                SelectShow(si);
            }
            LessBusy();
        }

        private void ForceRefresh(ShowItem si)
        {
            if (si != null)
                mDoc.GetTVDB(false, "").ForgetShow(si.TVDBCode, true);
            mDoc.DoDownloadsFG();
            FillShowLists();
            FillEpGuideHTML();
            RefreshWTW(false);
        }
        private void bnMyShowsRefresh_Click(object sender, System.EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                // nuke currently selected show to force getting it fresh
                TreeNode n = MyShowTree.SelectedNode;
                ShowItem si = TreeNodeToShowItem(n);
                ForceRefresh(si);
            }
            else
                ForceRefresh(null);
        }
        private void MyShowTree_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            FillEpGuideHTML(e.Node);
        }
        private void bnMyShowsVisitTVDB_Click(object sender, System.EventArgs e)
        {
            TreeNode n = MyShowTree.SelectedNode;
            ShowItem si = TreeNodeToShowItem(n);
            if (si == null)
                return;
            Season seas = TreeNodeToSeason(n);

            int sid = -1;
            if (seas != null)
                sid = seas.SeasonID;
            TVDoc.SysOpen(mDoc.GetTVDB(false, "").WebsiteURL(si.TVDBCode, sid, false));
        }
        private void bnMyShowsOpenFolder_Click(object sender, System.EventArgs e)
        {
            TreeNode n = MyShowTree.SelectedNode;
            ShowItem si = TreeNodeToShowItem(n);
            if (si == null)
                return;

            Season seas = TreeNodeToSeason(n);
            System.Collections.Generic.Dictionary<int, StringList> afl = si.AllFolderLocations(mDoc.Settings);
            int[] keys = new int[afl.Count];
            afl.Keys.CopyTo(keys, 0);
            if ((seas == null) && (keys.Length > 0))
            {
                string f = si.AutoAdd_FolderBase;
                if (string.IsNullOrEmpty(f))
                {
                    int n2 = keys[0];
                    if (afl[n2].Count > 0)
                        f = afl[n2][0];
                }
                if (!string.IsNullOrEmpty(f))
                {
                    try
                    {
                        TVDoc.SysOpen(f);
                        return;
                    }
                    catch
                    {
                    }
                }
            }

            if ((seas != null) && (afl.ContainsKey(seas.SeasonNumber)))
            {
                foreach (string folder in afl[seas.SeasonNumber])
                    if (Directory.Exists(folder))
                    {
                        TVDoc.SysOpen(folder);
                        return;
                    }
            }
            try
            {
                if (!string.IsNullOrEmpty(si.AutoAdd_FolderBase) && (Directory.Exists(si.AutoAdd_FolderBase)))
                    TVDoc.SysOpen(si.AutoAdd_FolderBase);
            }
            catch
            {
            }

        }
        private void MyShowTree_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Right)
                return;

            MyShowTree.SelectedNode = MyShowTree.GetNodeAt(e.X, e.Y);

            Point pt = MyShowTree.PointToScreen(new Point(e.X, e.Y));
            TreeNode n = MyShowTree.SelectedNode;

            if (n == null)
                return;

            ShowItem si = TreeNodeToShowItem(n);
            Season seas = TreeNodeToSeason(n);

            if (seas != null)
                RightClickOnMyShows(seas, pt);
            else if (si != null)
                RightClickOnMyShows(si, pt);
        }



        private void quickstartGuideToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            ShowQuickStartGuide();
        }
        private ProcessedEpisodeList CurrentlySelectedPEL()
        {
            Season currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
            ShowItem currentSI = TreeNodeToShowItem(MyShowTree.SelectedNode);

            int snum = (currentSeas != null) ? currentSeas.SeasonNumber : 1;
            ProcessedEpisodeList pel = null;
            if ((currentSI != null) && (currentSI.SeasonEpisodes.ContainsKey(snum)))
                pel = currentSI.SeasonEpisodes[snum];
            else
            {
                foreach (ShowItem si in mDoc.GetShowItems(true))
                {
                    foreach (System.Collections.Generic.KeyValuePair<int, ProcessedEpisodeList> kvp in si.SeasonEpisodes)
                    {
                        pel = kvp.Value;
                        break;
                    }
                    if (pel != null)
                        break;
                }
                mDoc.UnlockShowItems();
            }
            return pel;
        }
        private void filenameTemplateEditorToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            CustomName cn = new CustomName(mDoc.Settings.NamingStyle.StyleString);
            CustomNameDesigner cne = new CustomNameDesigner(CurrentlySelectedPEL(), cn, mDoc);
            DialogResult dr = cne.ShowDialog();
            if (dr == DialogResult.OK)
            {
                mDoc.Settings.NamingStyle = cn;
                mDoc.SetDirty();
            }
        }
        private void searchEnginesToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            ProcessedEpisodeList pel = CurrentlySelectedPEL();

            AddEditSearchEngine aese = new AddEditSearchEngine(mDoc.GetSearchers(), ((pel != null) && (pel.Count > 0)) ? pel[0] : null);
            DialogResult dr = aese.ShowDialog();
            if (dr == DialogResult.OK)
            {
                mDoc.SetDirty();
                UpdateSearchButton();
            }
        }
        private void filenameProcessorsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            //Season ^currentSeas = TreeNodeToSeason(MyShowTree->SelectedNode);
            ShowItem currentSI = TreeNodeToShowItem(MyShowTree.SelectedNode);
            string theFolder = "";

            if (currentSI != null)
            {
                foreach (System.Collections.Generic.KeyValuePair<int, StringList> kvp in currentSI.AllFolderLocations(mDoc.Settings))
                {
                    foreach (string folder in kvp.Value)
                    {
                        if ((!string.IsNullOrEmpty(folder)) && Directory.Exists(folder))
                        {
                            theFolder = folder;
                            break;
                        }
                    }
                    if (!string.IsNullOrEmpty(theFolder))
                        break;
                }
            }

            AddEditSeasEpFinders d = new AddEditSeasEpFinders(mDoc.Settings.FNPRegexs, mDoc.GetShowItems(true), currentSI, theFolder, mDoc.Settings);
            mDoc.UnlockShowItems();

            DialogResult dr = d.ShowDialog();
            if (dr == DialogResult.OK)
            {
                mDoc.SetDirty();
            }


        }
        private void actorsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            new ActorsGrid(mDoc).ShowDialog();
        }
        private void quickTimer_Tick(object sender, System.EventArgs e)
        {
            quickTimer.Stop();
            ProcessArgs();
        }
        private void uTorrentToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            uTorrent ut = new uTorrent(mDoc, this.SetProgress);
            ut.ShowDialog();
            tabControl1.SelectedIndex = 1; // go to all-in-one tab
        }
        private void bnMyShowsCollapse_Click(object sender, System.EventArgs e)
        {
            MyShowTree.CollapseAll();
        }
        private void UI_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            int t = -1;
            if (e.Control && (e.KeyCode == Keys.D1))
                t = 0;
            else if (e.Control && (e.KeyCode == Keys.D2))
                t = 1;
            else if (e.Control && (e.KeyCode == Keys.D3))
                t = 2;
            else if (e.Control && (e.KeyCode == Keys.D4))
                t = 3;
            else if (e.Control && (e.KeyCode == Keys.D5))
                t = 4;
            else if (e.Control && (e.KeyCode == Keys.D6))
                t = 5;
            else if (e.Control && (e.KeyCode == Keys.D7))
                t = 6;
            else if (e.Control && (e.KeyCode == Keys.D8))
                t = 7;
            else if (e.Control && (e.KeyCode == Keys.D9))
                t = 8;
            else if (e.Control && (e.KeyCode == Keys.D0))
                t = 9;
            if ((t >= 0) && (t < tabControl1.TabCount))
            {
                tabControl1.SelectedIndex = t;
                e.Handled = true;
            }
        }
        private void bnAIOCheck_Click(object sender, System.EventArgs e)
        {
            Scan(null);
        }

        private void Scan(ShowItem s)
        {
            MoreBusy();
            mDoc.AIOGo(this.SetProgress, s);
            LessBusy();
            FillAIOList();
        }

        private string GBMB(long size)
        {
            long gb1 = (1024 * 1024 * 1024);
            long gb = ((gb1 / 2) + size) / gb1;
            if (gb > 1)
                return gb.ToString() + " GB";
            else
            {
                long mb1 = 1024 * 1024;
                long mb = ((mb1 / 2) + size) / mb1;
                return mb.ToString() + " MB";
            }
        }
        private string itemitems(int n)
        {
            if (n == 1)
                return "Item";
            else
                return "Items";
        }
        private void lvAIO_RetrieveVirtualItem(object sender, System.Windows.Forms.RetrieveVirtualItemEventArgs e)
        {
            AIOItem aio = mDoc.TheAIOList[e.ItemIndex];
            ListViewItem lvi = aio.GetLVI(lvAIO);
            int n = aio.IconNumber();
            if (n != -1)
                lvi.ImageIndex = n;
            lvi.Checked = true;
            lvi.Tag = aio;

            const int kErrCol = 8;
            System.Diagnostics.Debug.Assert(lvi.SubItems.Count <= kErrCol);

            while (lvi.SubItems.Count < kErrCol)
                lvi.SubItems.Add(""); // pad our way to the error column

            if (aio.HasError)
            {

                lvi.SubItems.Add(aio.ErrorText); // error text
                lvi.BackColor = Helpers.WarningColor();
                if ((aio.Type == AIOType.kMissing) || (aio.Type == AIOType.kuTorrenting))
                    lvi.Checked = false;
            }
            else
                lvi.SubItems.Add("");

            System.Diagnostics.Debug.Assert(lvi.SubItems.Count == lvAIO.Columns.Count);

            e.Item = lvi;
        }

        private void FillAIOList()
        {
            if (lvAIO.VirtualMode)
            {
                lvAIO.VirtualListSize = mDoc.TheAIOList.Count;
            }
            else
            {
                lvAIO.BeginUpdate();
                lvAIO.Items.Clear();

                foreach (AIOItem aio in mDoc.TheAIOList)
                {
                    ListViewItem lvi = aio.GetLVI(lvAIO);
                    lvi.Checked = true;
                    lvi.Tag = aio;
                    lvAIO.Items.Add(lvi);

                    int n = aio.IconNumber();
                    if (n != -1)
                        lvi.ImageIndex = n;

                    const int kErrCol = 8;
                    System.Diagnostics.Debug.Assert(lvi.SubItems.Count <= kErrCol);
                    if (aio.HasError)
                    {
                        while (lvi.SubItems.Count < kErrCol)
                            lvi.SubItems.Add(""); // pad our way to the error column
                        lvi.SubItems.Add(aio.ErrorText); // error text
                        lvi.BackColor = Helpers.WarningColor();
                        if ((aio.Type == AIOType.kMissing) || (aio.Type == AIOType.kuTorrenting))
                            lvi.Checked = false;
                    }
                }
                lvAIO.EndUpdate();
            }

            // do nice totals for each group
            int missingCount = 0;
            int renameCount = 0;
            int copyCount = 0;
            long copySize = 0;
            int moveCount = 0;
            long moveSize = 0;
            int rssCount = 0;
            int downloadCount = 0;
            int nfoCount = 0;
            int utCount = 0;

            foreach (AIOItem aio in mDoc.TheAIOList)
            {
                if (aio.Type == AIOType.kMissing)
                    missingCount++;
                else if (aio.Type == AIOType.kCopyMoveRename)
                {
                    AIOCopyMoveRename cmr = (AIOCopyMoveRename)(aio);
                    AIOCopyMoveRename.Op op = cmr.Operation;
                    if (op == AIOCopyMoveRename.Op.Copy)
                    {
                        copyCount++;
                        if (cmr.From.Exists)
                            copySize += cmr.From.Length;
                    }
                    else if (op == AIOCopyMoveRename.Op.Move)
                    {
                        moveCount++;
                        if (cmr.From.Exists)
                            moveSize += cmr.From.Length;
                    }
                    else if (op == AIOCopyMoveRename.Op.Rename)
                        renameCount++;
                }
                else if (aio.Type == AIOType.kDownload)
                    downloadCount++;
                else if (aio.Type == AIOType.kRSS)
                    rssCount++;
                else if (aio.Type == AIOType.kNFO)
                    nfoCount++;
                else if (aio.Type == AIOType.kuTorrenting)
                    utCount++;
            }

            lvAIO.Groups[0].Header = "Missing (" + missingCount.ToString() + " " + itemitems(missingCount) + ")";
            lvAIO.Groups[1].Header = "Rename (" + renameCount.ToString() + " " + itemitems(renameCount) + ")";
            lvAIO.Groups[2].Header = "Copy (" + copyCount.ToString() + " " + itemitems(copyCount) + ", " + GBMB(copySize) + ")";
            lvAIO.Groups[3].Header = "Move (" + moveCount.ToString() + " " + itemitems(moveCount) + ", " + GBMB(moveSize) + ")";
            lvAIO.Groups[4].Header = "Download RSS (" + rssCount.ToString() + " " + itemitems(rssCount) + ")";
            lvAIO.Groups[5].Header = "Download (" + downloadCount.ToString() + " " + itemitems(downloadCount) + ")";
            lvAIO.Groups[6].Header = "NFO File (" + nfoCount.ToString() + " " + itemitems(nfoCount) + ")";
            lvAIO.Groups[7].Header = "Downloading In µTorrent (" + utCount.ToString() + " " + itemitems(utCount) + ")";


        }

        private void bnAIOAction_Click(object sender, System.EventArgs e)
        {
            AIOAction(true);
        }
        private void AIOAction(bool @checked)
        {
            LVResults lvr = new LVResults(lvAIO, @checked);
            mDoc.AIOAction(this.SetProgress, lvr.FlatList);
            // remove items from master list, unless it had an error
            foreach (AIOItem i2 in (new LVResults(lvAIO, @checked)).FlatList)
                if (!lvr.FlatList.Contains(i2))
                    mDoc.TheAIOList.Remove(i2);

            FillAIOList();
            RefreshWTW(false);
        }
        private void folderMonitorToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            FolderMonitor fm = new FolderMonitor(mDoc);
            fm.ShowDialog();
            FillShowLists();
        }
        private void torrentMatchToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            TorrentMatch tm = new TorrentMatch(mDoc, SetProgress);
            tm.ShowDialog();
            FillAIOList();
        }
        private void bnAIOWhichSearch_Click(object sender, System.EventArgs e)
        {
            ChooseSiteMenu(0);
        }
        private void lvAIO_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Right)
                return;

            // build the right click menu for the _selected_ items, and types of items
            LVResults lvr = new LVResults(lvAIO, false);

            if (lvr.Count == 0)
                return; // nothing selected

            Point pt = lvAIO.PointToScreen(new Point(e.X, e.Y));

            showRightClickMenu.Items.Clear();

            // AIO related items
            ToolStripMenuItem tsi;
            if (lvr.Count > lvr.Missing.Count) // not just missing selected
            {
                tsi = new ToolStripMenuItem("Action Selected");
                tsi.Tag = (int)RightClickCommands.kAIOAction;
                showRightClickMenu.Items.Add(tsi);
            }

            tsi = new ToolStripMenuItem("Ignore Selected");
            tsi.Tag = (int)RightClickCommands.kAIOIgnore;
            showRightClickMenu.Items.Add(tsi);

            tsi = new ToolStripMenuItem("Ignore Entire Season");
            tsi.Tag = (int)RightClickCommands.kAIOIgnoreSeason;
            showRightClickMenu.Items.Add(tsi);

            tsi = new ToolStripMenuItem("Remove Selected");
            tsi.Tag = (int)RightClickCommands.kAIODelete;
            showRightClickMenu.Items.Add(tsi);

            if (lvr.Count == lvr.Missing.Count) // only missing items selected?
            {
                showRightClickMenu.Items.Add(new ToolStripSeparator());

                tsi = new ToolStripMenuItem("BT Search");
                tsi.Tag = (int)RightClickCommands.kBTSearchFor;
                showRightClickMenu.Items.Add(tsi);

                if (lvr.Count == 1) // only one selected
                {
                    tsi = new ToolStripMenuItem("Browse For...");
                    tsi.Tag = (int)RightClickCommands.kAIOBrowseForFile;
                    showRightClickMenu.Items.Add(tsi);
                }
            }

            MenuGuideAndTVDB(true);
            MenuFolders(lvr);

            showRightClickMenu.Show(pt);
        }
        private void lvAIO_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            LVResults lvr = new LVResults(lvAIO, false);

            if (lvr.Count == 0)
            {
                // disable everything
                bnAIOBTSearch.Enabled = false;
                return;
            }

            if (lvr.Download.Count > 0)
                bnAIOBTSearch.Enabled = false;
            else
                bnAIOBTSearch.Enabled = true;

            mLastShowClicked = null;
            mLastEpClicked = null;
            mLastSeasonClicked = null;
            mLastAIOClicked = null;

            showRightClickMenu.Items.Clear();
            mFoldersToOpen = new StringList();
            mLastFL = new System.Collections.Generic.List<System.IO.FileInfo>();

            if ((lvr.Count == 1) && (lvAIO.FocusedItem != null) && (lvAIO.FocusedItem.Tag != null))
            {
                AIOItem aio = (AIOItem)(lvAIO.FocusedItem.Tag);
                mLastAIOClicked = aio;

                mLastEpClicked = aio.PE;
                if (aio.PE != null)
                {
                    mLastSeasonClicked = aio.PE.TheSeason;
                    mLastShowClicked = aio.PE.SI;
                }
                else
                {
                    mLastSeasonClicked = null;
                    mLastShowClicked = null;
                }


                if ((mLastEpClicked != null) && (mDoc.Settings.AutoSelectShowInMyShows))
                    GotoEpguideFor(mLastEpClicked, false);
            }
        }
        private void AIODeleteSelected()
        {
            ListView.SelectedListViewItemCollection sel = lvAIO.SelectedItems;
            foreach (ListViewItem lvi in sel)
                mDoc.TheAIOList.Remove((AIOItem)(lvi.Tag));
            FillAIOList();
        }
        private void lvAIO_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                AIODeleteSelected();
        }
        private void bnAIOIgnore_Click(object sender, System.EventArgs e)
        {
            IgnoreSelected();
        }
        private void bnAIOAllNone_Click(object sender, System.EventArgs e)
        {
            LVResults lvr = new LVResults(lvAIO, true);
            bool some = lvr.Count > 0;
            foreach (ListViewItem lvi in lvAIO.Items)
                lvi.Checked = some ? false : true;
        }
        private void bnAIORename_Click(object sender, System.EventArgs e)
        {
            LVResults lvr = new LVResults(lvAIO, true);
            bool some = lvr.Rename.Count > 0;
            foreach (ListViewItem lvi in lvAIO.Items)
            {
                AIOItem i = (AIOItem)(lvi.Tag);
                if ((i != null) && (i.Type == AIOType.kCopyMoveRename) && (((AIOCopyMoveRename)i).Operation == AIOCopyMoveRename.Op.Rename))
                    lvi.Checked = some ? false : true;
            }
        }
        private void bnAIOCopyMove_Click(object sender, System.EventArgs e)
        {
            LVResults lvr = new LVResults(lvAIO, true);
            bool some = lvr.CopyMove.Count > 0;
            foreach (ListViewItem lvi in lvAIO.Items)
            {
                AIOItem i = (AIOItem)(lvi.Tag);
                if ((i != null) && (i.Type == AIOType.kCopyMoveRename) && (((AIOCopyMoveRename)i).Operation != AIOCopyMoveRename.Op.Rename))
                    lvi.Checked = some ? false : true;
            }
        }
        private void bnAIONFO_Click(object sender, System.EventArgs e)
        {
            LVResults lvr = new LVResults(lvAIO, true);
            bool some = lvr.NFO.Count > 0;
            foreach (ListViewItem lvi in lvAIO.Items)
            {
                AIOItem i = (AIOItem)(lvi.Tag);
                if ((i != null) && (i.Type == AIOType.kNFO))
                    lvi.Checked = some ? false : true;
            }
        }

        private void bnAIORSS_Click(object sender, System.EventArgs e)
        {
            LVResults lvr = new LVResults(lvAIO, true);
            bool some = lvr.RSS.Count > 0;
            foreach (ListViewItem lvi in lvAIO.Items)
            {
                AIOItem i = (AIOItem)(lvi.Tag);
                if ((i != null) && (i.Type == AIOType.kRSS))
                    lvi.Checked = some ? false : true;
            }
        }
        private void bnAIODownloads_Click(object sender, System.EventArgs e)
        {
            LVResults lvr = new LVResults(lvAIO, true);
            bool some = lvr.Download.Count > 0;
            foreach (ListViewItem lvi in lvAIO.Items)
            {
                AIOItem i = (AIOItem)(lvi.Tag);
                if ((i != null) && (i.Type == AIOType.kDownload))
                    lvi.Checked = some ? false : true;
            }
        }
        private void lvAIO_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
        {
            if ((e.Index < 0) || (e.Index > lvAIO.Items.Count))
                return;
            AIOItem aio = (AIOItem)(lvAIO.Items[e.Index].Tag);
            if ((aio != null) && ((aio.Type == AIOType.kMissing) || (aio.Type == AIOType.kuTorrenting)))
                e.NewValue = CheckState.Unchecked;
        }
        private void bnAIOOptions_Click(object sender, System.EventArgs e)
        {
            DoPrefs(true);
        }
        private void lvAIO_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // double-click on an item will search for missing, do nothing (for now) for anything else
            foreach (AIOMissing miss in new LVResults(lvAIO, false).Missing)
                if (miss.PE != null)
                    mDoc.DoBTSearch(miss.PE);
        }
        private void bnAIOBTSearch_Click(object sender, System.EventArgs e)
        {
            LVResults lvr = new LVResults(lvAIO, false);

            if (lvr.Count == 0)
                return;

            foreach (AIOItem i in lvr.FlatList)
                if (i.PE != null)
                    mDoc.DoBTSearch(i.PE);
        }

        private void bnRemoveSel_Click(object sender, System.EventArgs e)
        {
            AIODeleteSelected();
        }
        private void IgnoreSelected()
        {
            LVResults lvr = new LVResults(lvAIO, false);
            bool added = false;
            foreach (AIOItem aio in lvr.FlatList)
            {
                IgnoreItem ii = aio.GetIgnore();
                if (ii != null)
                {
                    mDoc.Ignore.Add(ii);
                    added = true;
                }
            }
            if (added)
            {
                mDoc.SetDirty();
                mDoc.RemoveIgnored();
                FillAIOList();
            }
        }
        private void ignoreListToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            IgnoreEdit ie = new IgnoreEdit(mDoc);
            ie.ShowDialog();
        }
    } // UI class
} // namespace