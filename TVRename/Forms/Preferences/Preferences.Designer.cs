//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//


namespace TVRename
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Preferences));
            this.OKButton = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.ReplacementsGrid = new SourceGrid.Grid();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.bnBrowseWTWICAL = new System.Windows.Forms.Button();
            this.txtWTWICAL = new System.Windows.Forms.TextBox();
            this.cbWTWICAL = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtExportRSSDaysPast = new System.Windows.Forms.TextBox();
            this.bnBrowseWTWXML = new System.Windows.Forms.Button();
            this.txtWTWXML = new System.Windows.Forms.TextBox();
            this.cbWTWXML = new System.Windows.Forms.CheckBox();
            this.bnBrowseWTWRSS = new System.Windows.Forms.Button();
            this.txtWTWRSS = new System.Windows.Forms.TextBox();
            this.cbWTWRSS = new System.Windows.Forms.CheckBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.txtExportRSSMaxDays = new System.Windows.Forms.TextBox();
            this.txtExportRSSMaxShows = new System.Windows.Forms.TextBox();
            this.saveFile = new System.Windows.Forms.SaveFileDialog();
            this.txtSpecialsFolderName = new System.Windows.Forms.TextBox();
            this.txtVideoExtensions = new System.Windows.Forms.TextBox();
            this.cbStartupTab = new System.Windows.Forms.ComboBox();
            this.cbShowEpisodePictures = new System.Windows.Forms.CheckBox();
            this.cbLeadingZero = new System.Windows.Forms.CheckBox();
            this.cbKeepTogether = new System.Windows.Forms.CheckBox();
            this.chkShowInTaskbar = new System.Windows.Forms.CheckBox();
            this.cbNotificationIcon = new System.Windows.Forms.CheckBox();
            this.txtWTWDays = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tbGeneral = new System.Windows.Forms.TabPage();
            this.chkHideWtWSpoilers = new System.Windows.Forms.CheckBox();
            this.chkHideMyShowsSpoilers = new System.Windows.Forms.CheckBox();
            this.label37 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.tbPercentDirty = new System.Windows.Forms.TextBox();
            this.cbMode = new System.Windows.Forms.ComboBox();
            this.label34 = new System.Windows.Forms.Label();
            this.rbWTWScan = new System.Windows.Forms.RadioButton();
            this.rbWTWSearch = new System.Windows.Forms.RadioButton();
            this.label10 = new System.Windows.Forms.Label();
            this.cbLookForAirdate = new System.Windows.Forms.CheckBox();
            this.cbLanguages = new System.Windows.Forms.ComboBox();
            this.label21 = new System.Windows.Forms.Label();
            this.cbAutoSelInMyShows = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.txtParallelDownloads = new System.Windows.Forms.TextBox();
            this.tbFilesAndFolders = new System.Windows.Forms.TabPage();
            this.txtSeasonFormat = new System.Windows.Forms.TextBox();
            this.label47 = new System.Windows.Forms.Label();
            this.bnTags = new System.Windows.Forms.Button();
            this.txtKeepTogether = new System.Windows.Forms.TextBox();
            this.label39 = new System.Windows.Forms.Label();
            this.cbKeepTogetherMode = new System.Windows.Forms.ComboBox();
            this.tbSeasonSearchTerms = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.txtSeasonFolderName = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.bnReplaceRemove = new System.Windows.Forms.Button();
            this.bnReplaceAdd = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.txtMaxSampleSize = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.txtOtherExtensions = new System.Windows.Forms.TextBox();
            this.cbForceLower = new System.Windows.Forms.CheckBox();
            this.cbIgnoreSamples = new System.Windows.Forms.CheckBox();
            this.tbAutoExport = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.bnBrowseShowsHTML = new System.Windows.Forms.Button();
            this.cbShowsHTML = new System.Windows.Forms.CheckBox();
            this.txtShowsHTMLTo = new System.Windows.Forms.TextBox();
            this.bnBrowseShowsTXT = new System.Windows.Forms.Button();
            this.cbShowsTXT = new System.Windows.Forms.CheckBox();
            this.txtShowsTXTTo = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.bnBrowseFOXML = new System.Windows.Forms.Button();
            this.cbFOXML = new System.Windows.Forms.CheckBox();
            this.txtFOXML = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.bnBrowseRenamingXML = new System.Windows.Forms.Button();
            this.cbRenamingXML = new System.Windows.Forms.CheckBox();
            this.txtRenamingXML = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.bnBrowseMissingCSV = new System.Windows.Forms.Button();
            this.bnBrowseMissingXML = new System.Windows.Forms.Button();
            this.txtMissingCSV = new System.Windows.Forms.TextBox();
            this.cbMissingXML = new System.Windows.Forms.CheckBox();
            this.cbMissingCSV = new System.Windows.Forms.CheckBox();
            this.txtMissingXML = new System.Windows.Forms.TextBox();
            this.tpScanOptions = new System.Windows.Forms.TabPage();
            this.cbCheckqBitTorrent = new System.Windows.Forms.CheckBox();
            this.chkAutoMergeLibraryEpisodes = new System.Windows.Forms.CheckBox();
            this.chkAutoMergeDownloadEpisodes = new System.Windows.Forms.CheckBox();
            this.chkPreventMove = new System.Windows.Forms.CheckBox();
            this.label40 = new System.Windows.Forms.Label();
            this.cbxUpdateAirDate = new System.Windows.Forms.CheckBox();
            this.label33 = new System.Windows.Forms.Label();
            this.cbAutoCreateFolders = new System.Windows.Forms.CheckBox();
            this.label28 = new System.Windows.Forms.Label();
            this.cbSearchRSS = new System.Windows.Forms.CheckBox();
            this.cbRenameCheck = new System.Windows.Forms.CheckBox();
            this.cbMissing = new System.Windows.Forms.CheckBox();
            this.cbLeaveOriginals = new System.Windows.Forms.CheckBox();
            this.cbCheckSABnzbd = new System.Windows.Forms.CheckBox();
            this.cbCheckuTorrent = new System.Windows.Forms.CheckBox();
            this.cbSearchLocally = new System.Windows.Forms.CheckBox();
            this.tbFolderDeleting = new System.Windows.Forms.TabPage();
            this.cbCleanUpDownloadDir = new System.Windows.Forms.CheckBox();
            this.label32 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.txtEmptyMaxSize = new System.Windows.Forms.TextBox();
            this.txtEmptyIgnoreWords = new System.Windows.Forms.TextBox();
            this.txtEmptyIgnoreExtensions = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.cbRecycleNotDelete = new System.Windows.Forms.CheckBox();
            this.cbEmptyMaxSize = new System.Windows.Forms.CheckBox();
            this.cbEmptyIgnoreWords = new System.Windows.Forms.CheckBox();
            this.cbEmptyIgnoreExtensions = new System.Windows.Forms.CheckBox();
            this.cbDeleteEmpty = new System.Windows.Forms.CheckBox();
            this.tbMediaCenter = new System.Windows.Forms.TabPage();
            this.cbWDLiveEpisodeFiles = new System.Windows.Forms.CheckBox();
            this.cbNFOEpisodes = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbFolderBanner = new System.Windows.Forms.RadioButton();
            this.rbFolderPoster = new System.Windows.Forms.RadioButton();
            this.rbFolderFanArt = new System.Windows.Forms.RadioButton();
            this.rbFolderSeasonPoster = new System.Windows.Forms.RadioButton();
            this.cbKODIImages = new System.Windows.Forms.CheckBox();
            this.bnMCPresets = new System.Windows.Forms.Button();
            this.cbShrinkLarge = new System.Windows.Forms.CheckBox();
            this.cbEpThumbJpg = new System.Windows.Forms.CheckBox();
            this.label29 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.cbMetaSubfolder = new System.Windows.Forms.CheckBox();
            this.cbMeta = new System.Windows.Forms.CheckBox();
            this.cbEpTBNs = new System.Windows.Forms.CheckBox();
            this.cbSeriesJpg = new System.Windows.Forms.CheckBox();
            this.cbXMLFiles = new System.Windows.Forms.CheckBox();
            this.cbNFOShows = new System.Windows.Forms.CheckBox();
            this.cbFantArtJpg = new System.Windows.Forms.CheckBox();
            this.cbFolderJpg = new System.Windows.Forms.CheckBox();
            this.tbSearchFolders = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.domainUpDown1 = new System.Windows.Forms.DomainUpDown();
            this.chkScheduledScan = new System.Windows.Forms.CheckBox();
            this.chkScanOnStartup = new System.Windows.Forms.CheckBox();
            this.lblScanAction = new System.Windows.Forms.Label();
            this.rdoQuickScan = new System.Windows.Forms.RadioButton();
            this.rdoRecentScan = new System.Windows.Forms.RadioButton();
            this.rdoFullScan = new System.Windows.Forms.RadioButton();
            this.cbMonitorFolder = new System.Windows.Forms.CheckBox();
            this.bnOpenSearchFolder = new System.Windows.Forms.Button();
            this.bnRemoveSearchFolder = new System.Windows.Forms.Button();
            this.bnAddSearchFolder = new System.Windows.Forms.Button();
            this.lbSearchFolders = new System.Windows.Forms.ListBox();
            this.label23 = new System.Windows.Forms.Label();
            this.tbuTorrentNZB = new System.Windows.Forms.TabPage();
            this.qBitTorrent = new System.Windows.Forms.GroupBox();
            this.tbqBitTorrentHost = new System.Windows.Forms.TextBox();
            this.tbqBitTorrentPort = new System.Windows.Forms.TextBox();
            this.label41 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.tbPreferredRSSTerms = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSABHostPort = new System.Windows.Forms.TextBox();
            this.txtSABAPIKey = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.bnUTBrowseResumeDat = new System.Windows.Forms.Button();
            this.txtUTResumeDatPath = new System.Windows.Forms.TextBox();
            this.bnRSSBrowseuTorrent = new System.Windows.Forms.Button();
            this.label27 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.txtRSSuTorrentPath = new System.Windows.Forms.TextBox();
            this.RSSGrid = new SourceGrid.Grid();
            this.bnRSSRemove = new System.Windows.Forms.Button();
            this.bnRSSAdd = new System.Windows.Forms.Button();
            this.bnRSSGo = new System.Windows.Forms.Button();
            this.label25 = new System.Windows.Forms.Label();
            this.tpTreeColoring = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.cboShowStatus = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtShowStatusColor = new System.Windows.Forms.TextBox();
            this.btnSelectColor = new System.Windows.Forms.Button();
            this.bnRemoveDefinedColor = new System.Windows.Forms.Button();
            this.btnAddShowStatusColoring = new System.Windows.Forms.Button();
            this.lvwDefinedColors = new System.Windows.Forms.ListView();
            this.colShowStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colColor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tpBulkAdd = new System.Windows.Forms.TabPage();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.chkForceBulkAddToUseSettingsOnly = new System.Windows.Forms.CheckBox();
            this.cbIgnoreRecycleBin = new System.Windows.Forms.CheckBox();
            this.cbIgnoreNoVideoFolders = new System.Windows.Forms.CheckBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.chkAutoSearchForDownloadedFiles = new System.Windows.Forms.CheckBox();
            this.label43 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.tbIgnoreSuffixes = new System.Windows.Forms.TextBox();
            this.tbMovieTerms = new System.Windows.Forms.TextBox();
            this.tpSubtitles = new System.Windows.Forms.TabPage();
            this.cbTxtToSub = new System.Windows.Forms.CheckBox();
            this.label46 = new System.Windows.Forms.Label();
            this.txtSubtitleExtensions = new System.Windows.Forms.TextBox();
            this.chkRetainLanguageSpecificSubtitles = new System.Windows.Forms.CheckBox();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.openFile = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.cmDefaults = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.KODIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pyTivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mede8erToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tbGeneral.SuspendLayout();
            this.tbFilesAndFolders.SuspendLayout();
            this.tbAutoExport.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tpScanOptions.SuspendLayout();
            this.tbFolderDeleting.SuspendLayout();
            this.tbMediaCenter.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tbSearchFolders.SuspendLayout();
            this.tbuTorrentNZB.SuspendLayout();
            this.qBitTorrent.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tpTreeColoring.SuspendLayout();
            this.tpBulkAdd.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.tpSubtitles.SuspendLayout();
            this.cmDefaults.SuspendLayout();
            this.SuspendLayout();
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.Location = new System.Drawing.Point(369, 551);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 0;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(450, 551);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 1;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // ReplacementsGrid
            // 
            this.ReplacementsGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ReplacementsGrid.BackColor = System.Drawing.SystemColors.Window;
            this.ReplacementsGrid.EnableSort = true;
            this.ReplacementsGrid.Location = new System.Drawing.Point(6, 19);
            this.ReplacementsGrid.Name = "ReplacementsGrid";
            this.ReplacementsGrid.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.ReplacementsGrid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.ReplacementsGrid.Size = new System.Drawing.Size(497, 62);
            this.ReplacementsGrid.TabIndex = 1;
            this.ReplacementsGrid.TabStop = true;
            this.ReplacementsGrid.ToolTipText = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(497, 135);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "When to Watch";
            // 
            // bnBrowseWTWICAL
            // 
            this.bnBrowseWTWICAL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseWTWICAL.Location = new System.Drawing.Point(414, 76);
            this.bnBrowseWTWICAL.Name = "bnBrowseWTWICAL";
            this.bnBrowseWTWICAL.Size = new System.Drawing.Size(75, 23);
            this.bnBrowseWTWICAL.TabIndex = 24;
            this.bnBrowseWTWICAL.Text = "Browse...";
            this.bnBrowseWTWICAL.UseVisualStyleBackColor = true;
            this.bnBrowseWTWICAL.Click += new System.EventHandler(this.bnBrowseWTWICAL_Click);
            // 
            // txtWTWICAL
            // 
            this.txtWTWICAL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWTWICAL.Location = new System.Drawing.Point(65, 78);
            this.txtWTWICAL.Name = "txtWTWICAL";
            this.txtWTWICAL.Size = new System.Drawing.Size(342, 20);
            this.txtWTWICAL.TabIndex = 23;
            // 
            // cbWTWICAL
            // 
            this.cbWTWICAL.AutoSize = true;
            this.cbWTWICAL.Location = new System.Drawing.Point(8, 80);
            this.cbWTWICAL.Name = "cbWTWICAL";
            this.cbWTWICAL.Size = new System.Drawing.Size(43, 17);
            this.cbWTWICAL.TabIndex = 22;
            this.cbWTWICAL.Text = "iCal";
            this.cbWTWICAL.UseVisualStyleBackColor = true;
            this.cbWTWICAL.CheckedChanged += new System.EventHandler(this.EnableDisable);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(330, 109);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "in the past.";
            // 
            // txtExportRSSDaysPast
            // 
            this.txtExportRSSDaysPast.Location = new System.Drawing.Point(294, 106);
            this.txtExportRSSDaysPast.Name = "txtExportRSSDaysPast";
            this.txtExportRSSDaysPast.Size = new System.Drawing.Size(28, 20);
            this.txtExportRSSDaysPast.TabIndex = 20;
            // 
            // bnBrowseWTWXML
            // 
            this.bnBrowseWTWXML.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseWTWXML.Location = new System.Drawing.Point(521, 47);
            this.bnBrowseWTWXML.Name = "bnBrowseWTWXML";
            this.bnBrowseWTWXML.Size = new System.Drawing.Size(75, 23);
            this.bnBrowseWTWXML.TabIndex = 19;
            this.bnBrowseWTWXML.Text = "Browse...";
            this.bnBrowseWTWXML.UseVisualStyleBackColor = true;
            this.bnBrowseWTWXML.Click += new System.EventHandler(this.bnBrowseWTWXML_Click);
            // 
            // txtWTWXML
            // 
            this.txtWTWXML.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWTWXML.Location = new System.Drawing.Point(65, 49);
            this.txtWTWXML.Name = "txtWTWXML";
            this.txtWTWXML.Size = new System.Drawing.Size(423, 20);
            this.txtWTWXML.TabIndex = 18;
            // 
            // cbWTWXML
            // 
            this.cbWTWXML.AutoSize = true;
            this.cbWTWXML.Location = new System.Drawing.Point(8, 51);
            this.cbWTWXML.Name = "cbWTWXML";
            this.cbWTWXML.Size = new System.Drawing.Size(48, 17);
            this.cbWTWXML.TabIndex = 17;
            this.cbWTWXML.Text = "XML";
            this.cbWTWXML.UseVisualStyleBackColor = true;
            this.cbWTWXML.CheckedChanged += new System.EventHandler(this.EnableDisable);
            // 
            // bnBrowseWTWRSS
            // 
            this.bnBrowseWTWRSS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseWTWRSS.Location = new System.Drawing.Point(520, 18);
            this.bnBrowseWTWRSS.Name = "bnBrowseWTWRSS";
            this.bnBrowseWTWRSS.Size = new System.Drawing.Size(75, 23);
            this.bnBrowseWTWRSS.TabIndex = 2;
            this.bnBrowseWTWRSS.Text = "Browse...";
            this.bnBrowseWTWRSS.UseVisualStyleBackColor = true;
            this.bnBrowseWTWRSS.Click += new System.EventHandler(this.bnBrowseWTWRSS_Click);
            // 
            // txtWTWRSS
            // 
            this.txtWTWRSS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWTWRSS.Location = new System.Drawing.Point(64, 20);
            this.txtWTWRSS.Name = "txtWTWRSS";
            this.txtWTWRSS.Size = new System.Drawing.Size(424, 20);
            this.txtWTWRSS.TabIndex = 1;
            // 
            // cbWTWRSS
            // 
            this.cbWTWRSS.AutoSize = true;
            this.cbWTWRSS.Location = new System.Drawing.Point(8, 22);
            this.cbWTWRSS.Name = "cbWTWRSS";
            this.cbWTWRSS.Size = new System.Drawing.Size(48, 17);
            this.cbWTWRSS.TabIndex = 0;
            this.cbWTWRSS.Text = "RSS";
            this.cbWTWRSS.UseVisualStyleBackColor = true;
            this.cbWTWRSS.CheckedChanged += new System.EventHandler(this.EnableDisable);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(212, 109);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(79, 13);
            this.label17.TabIndex = 7;
            this.label17.Text = "days worth and";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(120, 109);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(52, 13);
            this.label16.TabIndex = 5;
            this.label16.Text = "shows, or";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(9, 109);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(71, 13);
            this.label15.TabIndex = 3;
            this.label15.Text = "No more than";
            // 
            // txtExportRSSMaxDays
            // 
            this.txtExportRSSMaxDays.Location = new System.Drawing.Point(178, 106);
            this.txtExportRSSMaxDays.Name = "txtExportRSSMaxDays";
            this.txtExportRSSMaxDays.Size = new System.Drawing.Size(28, 20);
            this.txtExportRSSMaxDays.TabIndex = 6;
            this.txtExportRSSMaxDays.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtNumberOnlyKeyPress);
            // 
            // txtExportRSSMaxShows
            // 
            this.txtExportRSSMaxShows.Location = new System.Drawing.Point(86, 106);
            this.txtExportRSSMaxShows.Name = "txtExportRSSMaxShows";
            this.txtExportRSSMaxShows.Size = new System.Drawing.Size(28, 20);
            this.txtExportRSSMaxShows.TabIndex = 4;
            this.txtExportRSSMaxShows.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtNumberOnlyKeyPress);
            // 
            // saveFile
            // 
            this.saveFile.Filter = "RSS files (*.rss)|*.rss|XML files (*.xml)|*.xml|CSV files (*.csv)|*.csv|TXT files" +
    " (*.txt)|*.txt|HTML files (*.html)|*.html|iCal files (*.ics)|*.ics|All files (*." +
    "*)|*.*";
            // 
            // txtSpecialsFolderName
            // 
            this.txtSpecialsFolderName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSpecialsFolderName.Location = new System.Drawing.Point(113, 281);
            this.txtSpecialsFolderName.Name = "txtSpecialsFolderName";
            this.txtSpecialsFolderName.Size = new System.Drawing.Size(492, 20);
            this.txtSpecialsFolderName.TabIndex = 12;
            // 
            // txtVideoExtensions
            // 
            this.txtVideoExtensions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtVideoExtensions.Location = new System.Drawing.Point(99, 128);
            this.txtVideoExtensions.Name = "txtVideoExtensions";
            this.txtVideoExtensions.Size = new System.Drawing.Size(506, 20);
            this.txtVideoExtensions.TabIndex = 5;
            // 
            // cbStartupTab
            // 
            this.cbStartupTab.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStartupTab.FormattingEnabled = true;
            this.cbStartupTab.Items.AddRange(new object[] {
            "My Shows",
            "Scan",
            "When to Watch"});
            this.cbStartupTab.Location = new System.Drawing.Point(74, 88);
            this.cbStartupTab.Name = "cbStartupTab";
            this.cbStartupTab.Size = new System.Drawing.Size(135, 21);
            this.cbStartupTab.TabIndex = 7;
            // 
            // cbShowEpisodePictures
            // 
            this.cbShowEpisodePictures.AutoSize = true;
            this.cbShowEpisodePictures.Location = new System.Drawing.Point(9, 138);
            this.cbShowEpisodePictures.Name = "cbShowEpisodePictures";
            this.cbShowEpisodePictures.Size = new System.Drawing.Size(218, 17);
            this.cbShowEpisodePictures.TabIndex = 10;
            this.cbShowEpisodePictures.Text = "S&how episode pictures in episode guides";
            this.cbShowEpisodePictures.UseVisualStyleBackColor = true;
            // 
            // cbLeadingZero
            // 
            this.cbLeadingZero.AutoSize = true;
            this.cbLeadingZero.Location = new System.Drawing.Point(6, 257);
            this.cbLeadingZero.Name = "cbLeadingZero";
            this.cbLeadingZero.Size = new System.Drawing.Size(170, 17);
            this.cbLeadingZero.TabIndex = 10;
            this.cbLeadingZero.Text = "&Leading 0 on Season numbers";
            this.cbLeadingZero.UseVisualStyleBackColor = true;
            // 
            // cbKeepTogether
            // 
            this.cbKeepTogether.AutoSize = true;
            this.cbKeepTogether.Location = new System.Drawing.Point(6, 180);
            this.cbKeepTogether.Name = "cbKeepTogether";
            this.cbKeepTogether.Size = new System.Drawing.Size(251, 17);
            this.cbKeepTogether.TabIndex = 8;
            this.cbKeepTogether.Text = "&Copy/Move files with same base name as video";
            this.cbKeepTogether.UseVisualStyleBackColor = true;
            this.cbKeepTogether.CheckedChanged += new System.EventHandler(this.cbKeepTogether_CheckedChanged);
            // 
            // chkShowInTaskbar
            // 
            this.chkShowInTaskbar.AutoSize = true;
            this.chkShowInTaskbar.Location = new System.Drawing.Point(169, 115);
            this.chkShowInTaskbar.Name = "chkShowInTaskbar";
            this.chkShowInTaskbar.Size = new System.Drawing.Size(102, 17);
            this.chkShowInTaskbar.TabIndex = 9;
            this.chkShowInTaskbar.Text = "Show in &taskbar";
            this.chkShowInTaskbar.UseVisualStyleBackColor = true;
            this.chkShowInTaskbar.CheckedChanged += new System.EventHandler(this.chkShowInTaskbar_CheckedChanged);
            // 
            // cbNotificationIcon
            // 
            this.cbNotificationIcon.AutoSize = true;
            this.cbNotificationIcon.Location = new System.Drawing.Point(9, 115);
            this.cbNotificationIcon.Name = "cbNotificationIcon";
            this.cbNotificationIcon.Size = new System.Drawing.Size(154, 17);
            this.cbNotificationIcon.TabIndex = 8;
            this.cbNotificationIcon.Text = "Show &notification area icon";
            this.cbNotificationIcon.UseVisualStyleBackColor = true;
            this.cbNotificationIcon.CheckedChanged += new System.EventHandler(this.cbNotificationIcon_CheckedChanged);
            // 
            // txtWTWDays
            // 
            this.txtWTWDays.Location = new System.Drawing.Point(13, 9);
            this.txtWTWDays.Name = "txtWTWDays";
            this.txtWTWDays.Size = new System.Drawing.Size(28, 20);
            this.txtWTWDays.TabIndex = 1;
            this.txtWTWDays.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtNumberOnlyKeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(47, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "days counts as recent";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 284);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(108, 13);
            this.label13.TabIndex = 11;
            this.label13.Text = "&Specials folder name:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 131);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(90, 13);
            this.label14.TabIndex = 4;
            this.label14.Text = "&Video extensions:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 91);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "&Startup tab:";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tbGeneral);
            this.tabControl1.Controls.Add(this.tbFilesAndFolders);
            this.tabControl1.Controls.Add(this.tbAutoExport);
            this.tabControl1.Controls.Add(this.tpScanOptions);
            this.tabControl1.Controls.Add(this.tbFolderDeleting);
            this.tabControl1.Controls.Add(this.tbMediaCenter);
            this.tabControl1.Controls.Add(this.tbSearchFolders);
            this.tabControl1.Controls.Add(this.tbuTorrentNZB);
            this.tabControl1.Controls.Add(this.tpTreeColoring);
            this.tabControl1.Controls.Add(this.tpBulkAdd);
            this.tabControl1.Controls.Add(this.tpSubtitles);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(517, 533);
            this.tabControl1.TabIndex = 0;
            // 
            // tbGeneral
            // 
            this.tbGeneral.Controls.Add(this.chkHideWtWSpoilers);
            this.tbGeneral.Controls.Add(this.chkHideMyShowsSpoilers);
            this.tbGeneral.Controls.Add(this.label37);
            this.tbGeneral.Controls.Add(this.label38);
            this.tbGeneral.Controls.Add(this.tbPercentDirty);
            this.tbGeneral.Controls.Add(this.cbMode);
            this.tbGeneral.Controls.Add(this.label34);
            this.tbGeneral.Controls.Add(this.rbWTWScan);
            this.tbGeneral.Controls.Add(this.rbWTWSearch);
            this.tbGeneral.Controls.Add(this.label10);
            this.tbGeneral.Controls.Add(this.cbLookForAirdate);
            this.tbGeneral.Controls.Add(this.cbLanguages);
            this.tbGeneral.Controls.Add(this.cbStartupTab);
            this.tbGeneral.Controls.Add(this.label21);
            this.tbGeneral.Controls.Add(this.cbAutoSelInMyShows);
            this.tbGeneral.Controls.Add(this.cbShowEpisodePictures);
            this.tbGeneral.Controls.Add(this.label11);
            this.tbGeneral.Controls.Add(this.label6);
            this.tbGeneral.Controls.Add(this.chkShowInTaskbar);
            this.tbGeneral.Controls.Add(this.label20);
            this.tbGeneral.Controls.Add(this.label2);
            this.tbGeneral.Controls.Add(this.cbNotificationIcon);
            this.tbGeneral.Controls.Add(this.txtParallelDownloads);
            this.tbGeneral.Controls.Add(this.txtWTWDays);
            this.tbGeneral.Location = new System.Drawing.Point(4, 40);
            this.tbGeneral.Name = "tbGeneral";
            this.tbGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tbGeneral.Size = new System.Drawing.Size(509, 489);
            this.tbGeneral.TabIndex = 0;
            this.tbGeneral.Text = "General";
            this.tbGeneral.UseVisualStyleBackColor = true;
            // 
            // chkHideWtWSpoilers
            // 
            this.chkHideWtWSpoilers.AutoSize = true;
            this.chkHideWtWSpoilers.Location = new System.Drawing.Point(9, 184);
            this.chkHideWtWSpoilers.Name = "chkHideWtWSpoilers";
            this.chkHideWtWSpoilers.Size = new System.Drawing.Size(182, 17);
            this.chkHideWtWSpoilers.TabIndex = 24;
            this.chkHideWtWSpoilers.Text = "Hide Spoilers in When To Watch";
            this.chkHideWtWSpoilers.UseVisualStyleBackColor = true;
            // 
            // chkHideMyShowsSpoilers
            // 
            this.chkHideMyShowsSpoilers.AutoSize = true;
            this.chkHideMyShowsSpoilers.Location = new System.Drawing.Point(9, 161);
            this.chkHideMyShowsSpoilers.Name = "chkHideMyShowsSpoilers";
            this.chkHideMyShowsSpoilers.Size = new System.Drawing.Size(151, 17);
            this.chkHideMyShowsSpoilers.TabIndex = 23;
            this.chkHideMyShowsSpoilers.Text = "Hide Spoilers in My Shows";
            this.chkHideMyShowsSpoilers.UseVisualStyleBackColor = true;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(6, 229);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(114, 13);
            this.label37.TabIndex = 20;
            this.label37.Text = "Refresh entire series  if";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(158, 229);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(132, 13);
            this.label38.TabIndex = 22;
            this.label38.Text = "% of episodes are updated";
            // 
            // tbPercentDirty
            // 
            this.tbPercentDirty.Location = new System.Drawing.Point(123, 226);
            this.tbPercentDirty.Name = "tbPercentDirty";
            this.tbPercentDirty.Size = new System.Drawing.Size(28, 20);
            this.tbPercentDirty.TabIndex = 21;
            // 
            // cbMode
            // 
            this.cbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMode.FormattingEnabled = true;
            this.cbMode.Items.AddRange(new object[] {
            "Beta",
            "Production"});
            this.cbMode.Location = new System.Drawing.Point(112, 320);
            this.cbMode.Name = "cbMode";
            this.cbMode.Size = new System.Drawing.Size(146, 21);
            this.cbMode.Sorted = true;
            this.cbMode.TabIndex = 19;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(6, 323);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(37, 13);
            this.label34.TabIndex = 18;
            this.label34.Text = "&Mode:";
            // 
            // rbWTWScan
            // 
            this.rbWTWScan.AutoSize = true;
            this.rbWTWScan.Location = new System.Drawing.Point(27, 66);
            this.rbWTWScan.Name = "rbWTWScan";
            this.rbWTWScan.Size = new System.Drawing.Size(50, 17);
            this.rbWTWScan.TabIndex = 5;
            this.rbWTWScan.TabStop = true;
            this.rbWTWScan.Text = "S&can";
            this.rbWTWScan.UseVisualStyleBackColor = true;
            // 
            // rbWTWSearch
            // 
            this.rbWTWSearch.AutoSize = true;
            this.rbWTWSearch.Location = new System.Drawing.Point(27, 47);
            this.rbWTWSearch.Name = "rbWTWSearch";
            this.rbWTWSearch.Size = new System.Drawing.Size(59, 17);
            this.rbWTWSearch.TabIndex = 4;
            this.rbWTWSearch.TabStop = true;
            this.rbWTWSearch.Text = "S&earch";
            this.rbWTWSearch.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 299);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(100, 13);
            this.label10.TabIndex = 16;
            this.label10.Text = "&Preferred language:";
            // 
            // cbLookForAirdate
            // 
            this.cbLookForAirdate.AutoSize = true;
            this.cbLookForAirdate.Location = new System.Drawing.Point(9, 275);
            this.cbLookForAirdate.Name = "cbLookForAirdate";
            this.cbLookForAirdate.Size = new System.Drawing.Size(158, 17);
            this.cbLookForAirdate.TabIndex = 15;
            this.cbLookForAirdate.Text = "&Look for airdate in filenames";
            this.cbLookForAirdate.UseVisualStyleBackColor = true;
            // 
            // cbLanguages
            // 
            this.cbLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLanguages.FormattingEnabled = true;
            this.cbLanguages.Items.AddRange(new object[] {
            "My Shows",
            "Scan",
            "When to Watch"});
            this.cbLanguages.Location = new System.Drawing.Point(112, 296);
            this.cbLanguages.Name = "cbLanguages";
            this.cbLanguages.Size = new System.Drawing.Size(146, 21);
            this.cbLanguages.Sorted = true;
            this.cbLanguages.TabIndex = 17;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 205);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(82, 13);
            this.label21.TabIndex = 11;
            this.label21.Text = "&Download up to";
            // 
            // cbAutoSelInMyShows
            // 
            this.cbAutoSelInMyShows.AutoSize = true;
            this.cbAutoSelInMyShows.Location = new System.Drawing.Point(9, 252);
            this.cbAutoSelInMyShows.Name = "cbAutoSelInMyShows";
            this.cbAutoSelInMyShows.Size = new System.Drawing.Size(268, 17);
            this.cbAutoSelInMyShows.TabIndex = 14;
            this.cbAutoSelInMyShows.Text = "&Automatically select show and season in My Shows";
            this.cbAutoSelInMyShows.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 32);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(185, 13);
            this.label11.TabIndex = 3;
            this.label11.Text = "Double-click in When to Watch does:";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(126, 205);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(170, 13);
            this.label20.TabIndex = 13;
            this.label20.Text = "shows simultaneously from thetvdb";
            // 
            // txtParallelDownloads
            // 
            this.txtParallelDownloads.Location = new System.Drawing.Point(92, 202);
            this.txtParallelDownloads.Name = "txtParallelDownloads";
            this.txtParallelDownloads.Size = new System.Drawing.Size(28, 20);
            this.txtParallelDownloads.TabIndex = 12;
            this.txtParallelDownloads.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtNumberOnlyKeyPress);
            // 
            // tbFilesAndFolders
            // 
            this.tbFilesAndFolders.Controls.Add(this.txtSeasonFormat);
            this.tbFilesAndFolders.Controls.Add(this.label47);
            this.tbFilesAndFolders.Controls.Add(this.bnTags);
            this.tbFilesAndFolders.Controls.Add(this.txtKeepTogether);
            this.tbFilesAndFolders.Controls.Add(this.label39);
            this.tbFilesAndFolders.Controls.Add(this.cbKeepTogetherMode);
            this.tbFilesAndFolders.Controls.Add(this.tbSeasonSearchTerms);
            this.tbFilesAndFolders.Controls.Add(this.label36);
            this.tbFilesAndFolders.Controls.Add(this.txtSeasonFolderName);
            this.tbFilesAndFolders.Controls.Add(this.label35);
            this.tbFilesAndFolders.Controls.Add(this.bnReplaceRemove);
            this.tbFilesAndFolders.Controls.Add(this.bnReplaceAdd);
            this.tbFilesAndFolders.Controls.Add(this.label3);
            this.tbFilesAndFolders.Controls.Add(this.ReplacementsGrid);
            this.tbFilesAndFolders.Controls.Add(this.label19);
            this.tbFilesAndFolders.Controls.Add(this.txtMaxSampleSize);
            this.tbFilesAndFolders.Controls.Add(this.label22);
            this.tbFilesAndFolders.Controls.Add(this.label14);
            this.tbFilesAndFolders.Controls.Add(this.txtSpecialsFolderName);
            this.tbFilesAndFolders.Controls.Add(this.label13);
            this.tbFilesAndFolders.Controls.Add(this.txtOtherExtensions);
            this.tbFilesAndFolders.Controls.Add(this.txtVideoExtensions);
            this.tbFilesAndFolders.Controls.Add(this.cbKeepTogether);
            this.tbFilesAndFolders.Controls.Add(this.cbForceLower);
            this.tbFilesAndFolders.Controls.Add(this.cbIgnoreSamples);
            this.tbFilesAndFolders.Controls.Add(this.cbLeadingZero);
            this.tbFilesAndFolders.Location = new System.Drawing.Point(4, 40);
            this.tbFilesAndFolders.Name = "tbFilesAndFolders";
            this.tbFilesAndFolders.Padding = new System.Windows.Forms.Padding(3);
            this.tbFilesAndFolders.Size = new System.Drawing.Size(509, 489);
            this.tbFilesAndFolders.TabIndex = 1;
            this.tbFilesAndFolders.Text = "Files and Folders";
            this.tbFilesAndFolders.UseVisualStyleBackColor = true;
            // 
            // txtSeasonFormat
            // 
            this.txtSeasonFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSeasonFormat.Location = new System.Drawing.Point(113, 330);
            this.txtSeasonFormat.Name = "txtSeasonFormat";
            this.txtSeasonFormat.Size = new System.Drawing.Size(390, 20);
            this.txtSeasonFormat.TabIndex = 26;
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(6, 333);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(112, 13);
            this.label47.TabIndex = 25;
            this.label47.Text = "&Seasons folder format:";
            // 
            // bnTags
            // 
            this.bnTags.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnTags.Location = new System.Drawing.Point(548, 328);
            this.bnTags.Name = "bnTags";
            this.bnTags.Size = new System.Drawing.Size(75, 23);
            this.bnTags.TabIndex = 24;
            this.bnTags.Text = "Tags...";
            this.bnTags.UseVisualStyleBackColor = true;
            this.bnTags.Click += new System.EventHandler(this.bnTags_Click);
            // 
            // txtKeepTogether
            // 
            this.txtKeepTogether.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtKeepTogether.Location = new System.Drawing.Point(204, 206);
            this.txtKeepTogether.Name = "txtKeepTogether";
            this.txtKeepTogether.Size = new System.Drawing.Size(299, 20);
            this.txtKeepTogether.TabIndex = 23;
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(25, 210);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(21, 13);
            this.label39.TabIndex = 22;
            this.label39.Text = "Do";
            // 
            // cbKeepTogetherMode
            // 
            this.cbKeepTogetherMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbKeepTogetherMode.FormattingEnabled = true;
            this.cbKeepTogetherMode.Items.AddRange(new object[] {
            "All",
            "All but these",
            "Just"});
            this.cbKeepTogetherMode.Location = new System.Drawing.Point(52, 207);
            this.cbKeepTogetherMode.Name = "cbKeepTogetherMode";
            this.cbKeepTogetherMode.Size = new System.Drawing.Size(146, 21);
            this.cbKeepTogetherMode.Sorted = true;
            this.cbKeepTogetherMode.TabIndex = 21;
            this.cbKeepTogetherMode.SelectedIndexChanged += new System.EventHandler(this.cbKeepTogetherMode_SelectedIndexChanged);
            // 
            // tbSeasonSearchTerms
            // 
            this.tbSeasonSearchTerms.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSeasonSearchTerms.Location = new System.Drawing.Point(113, 353);
            this.tbSeasonSearchTerms.Name = "tbSeasonSearchTerms";
            this.tbSeasonSearchTerms.Size = new System.Drawing.Size(390, 20);
            this.tbSeasonSearchTerms.TabIndex = 20;
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(6, 356);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(109, 13);
            this.label36.TabIndex = 19;
            this.label36.Text = "Season search terms:";
            // 
            // txtSeasonFolderName
            // 
            this.txtSeasonFolderName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSeasonFolderName.Location = new System.Drawing.Point(113, 307);
            this.txtSeasonFolderName.Name = "txtSeasonFolderName";
            this.txtSeasonFolderName.Size = new System.Drawing.Size(390, 20);
            this.txtSeasonFolderName.TabIndex = 18;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(6, 310);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(80, 13);
            this.label35.TabIndex = 17;
            this.label35.Text = "&Seasons name:";
            // 
            // bnReplaceRemove
            // 
            this.bnReplaceRemove.Location = new System.Drawing.Point(90, 91);
            this.bnReplaceRemove.Name = "bnReplaceRemove";
            this.bnReplaceRemove.Size = new System.Drawing.Size(75, 23);
            this.bnReplaceRemove.TabIndex = 3;
            this.bnReplaceRemove.Text = "&Remove";
            this.bnReplaceRemove.UseVisualStyleBackColor = true;
            this.bnReplaceRemove.Click += new System.EventHandler(this.bnReplaceRemove_Click);
            // 
            // bnReplaceAdd
            // 
            this.bnReplaceAdd.Location = new System.Drawing.Point(9, 91);
            this.bnReplaceAdd.Name = "bnReplaceAdd";
            this.bnReplaceAdd.Size = new System.Drawing.Size(75, 23);
            this.bnReplaceAdd.TabIndex = 2;
            this.bnReplaceAdd.Text = "&Add";
            this.bnReplaceAdd.UseVisualStyleBackColor = true;
            this.bnReplaceAdd.Click += new System.EventHandler(this.bnReplaceAdd_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Filename Replacements";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(228, 379);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(55, 13);
            this.label19.TabIndex = 15;
            this.label19.Text = "MB in size";
            // 
            // txtMaxSampleSize
            // 
            this.txtMaxSampleSize.Location = new System.Drawing.Point(172, 376);
            this.txtMaxSampleSize.Name = "txtMaxSampleSize";
            this.txtMaxSampleSize.Size = new System.Drawing.Size(53, 20);
            this.txtMaxSampleSize.TabIndex = 14;
            this.txtMaxSampleSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtNumberOnlyKeyPress);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(3, 157);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(89, 13);
            this.label22.TabIndex = 6;
            this.label22.Text = "&Other extensions:";
            // 
            // txtOtherExtensions
            // 
            this.txtOtherExtensions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOtherExtensions.Location = new System.Drawing.Point(99, 154);
            this.txtOtherExtensions.Name = "txtOtherExtensions";
            this.txtOtherExtensions.Size = new System.Drawing.Size(404, 20);
            this.txtOtherExtensions.TabIndex = 7;
            // 
            // cbForceLower
            // 
            this.cbForceLower.AutoSize = true;
            this.cbForceLower.Location = new System.Drawing.Point(6, 401);
            this.cbForceLower.Name = "cbForceLower";
            this.cbForceLower.Size = new System.Drawing.Size(167, 17);
            this.cbForceLower.TabIndex = 16;
            this.cbForceLower.Text = "&Make all filenames lower case";
            this.cbForceLower.UseVisualStyleBackColor = true;
            // 
            // cbIgnoreSamples
            // 
            this.cbIgnoreSamples.AutoSize = true;
            this.cbIgnoreSamples.Location = new System.Drawing.Point(6, 378);
            this.cbIgnoreSamples.Name = "cbIgnoreSamples";
            this.cbIgnoreSamples.Size = new System.Drawing.Size(166, 17);
            this.cbIgnoreSamples.TabIndex = 13;
            this.cbIgnoreSamples.Text = "&Ignore \"sample\" videos, up to";
            this.cbIgnoreSamples.UseVisualStyleBackColor = true;
            // 
            // tbAutoExport
            // 
            this.tbAutoExport.Controls.Add(this.groupBox7);
            this.tbAutoExport.Controls.Add(this.groupBox5);
            this.tbAutoExport.Controls.Add(this.groupBox4);
            this.tbAutoExport.Controls.Add(this.groupBox3);
            this.tbAutoExport.Controls.Add(this.groupBox2);
            this.tbAutoExport.Location = new System.Drawing.Point(4, 40);
            this.tbAutoExport.Name = "tbAutoExport";
            this.tbAutoExport.Padding = new System.Windows.Forms.Padding(3);
            this.tbAutoExport.Size = new System.Drawing.Size(509, 489);
            this.tbAutoExport.TabIndex = 2;
            this.tbAutoExport.Text = "Automatic Export";
            this.tbAutoExport.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox7.Controls.Add(this.bnBrowseShowsHTML);
            this.groupBox7.Controls.Add(this.cbShowsHTML);
            this.groupBox7.Controls.Add(this.txtShowsHTMLTo);
            this.groupBox7.Controls.Add(this.bnBrowseShowsTXT);
            this.groupBox7.Controls.Add(this.cbShowsTXT);
            this.groupBox7.Controls.Add(this.txtShowsTXTTo);
            this.groupBox7.Location = new System.Drawing.Point(7, 227);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(496, 72);
            this.groupBox7.TabIndex = 4;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "All Shows";
            // 
            // bnBrowseShowsHTML
            // 
            this.bnBrowseShowsHTML.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseShowsHTML.Location = new System.Drawing.Point(413, 45);
            this.bnBrowseShowsHTML.Name = "bnBrowseShowsHTML";
            this.bnBrowseShowsHTML.Size = new System.Drawing.Size(75, 23);
            this.bnBrowseShowsHTML.TabIndex = 8;
            this.bnBrowseShowsHTML.Text = "Browse...";
            this.bnBrowseShowsHTML.UseVisualStyleBackColor = true;
            this.bnBrowseShowsHTML.Click += new System.EventHandler(this.bnBrowseShowsHTML_Click);
            // 
            // cbShowsHTML
            // 
            this.cbShowsHTML.AutoSize = true;
            this.cbShowsHTML.Location = new System.Drawing.Point(8, 49);
            this.cbShowsHTML.Name = "cbShowsHTML";
            this.cbShowsHTML.Size = new System.Drawing.Size(56, 17);
            this.cbShowsHTML.TabIndex = 6;
            this.cbShowsHTML.Text = "HTML";
            this.cbShowsHTML.UseVisualStyleBackColor = true;
            this.cbShowsHTML.CheckedChanged += new System.EventHandler(this.EnableDisable);
            // 
            // txtShowsHTMLTo
            // 
            this.txtShowsHTMLTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtShowsHTMLTo.Location = new System.Drawing.Point(64, 47);
            this.txtShowsHTMLTo.Name = "txtShowsHTMLTo";
            this.txtShowsHTMLTo.Size = new System.Drawing.Size(343, 20);
            this.txtShowsHTMLTo.TabIndex = 7;
            // 
            // bnBrowseShowsTXT
            // 
            this.bnBrowseShowsTXT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseShowsTXT.Location = new System.Drawing.Point(413, 21);
            this.bnBrowseShowsTXT.Name = "bnBrowseShowsTXT";
            this.bnBrowseShowsTXT.Size = new System.Drawing.Size(75, 23);
            this.bnBrowseShowsTXT.TabIndex = 5;
            this.bnBrowseShowsTXT.Text = "Browse...";
            this.bnBrowseShowsTXT.UseVisualStyleBackColor = true;
            this.bnBrowseShowsTXT.Click += new System.EventHandler(this.bnBrowseShowsTXT_Click);
            // 
            // cbShowsTXT
            // 
            this.cbShowsTXT.AutoSize = true;
            this.cbShowsTXT.Location = new System.Drawing.Point(8, 25);
            this.cbShowsTXT.Name = "cbShowsTXT";
            this.cbShowsTXT.Size = new System.Drawing.Size(47, 17);
            this.cbShowsTXT.TabIndex = 3;
            this.cbShowsTXT.Text = "TXT";
            this.cbShowsTXT.UseVisualStyleBackColor = true;
            this.cbShowsTXT.CheckedChanged += new System.EventHandler(this.EnableDisable);
            // 
            // txtShowsTXTTo
            // 
            this.txtShowsTXTTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtShowsTXTTo.Location = new System.Drawing.Point(64, 23);
            this.txtShowsTXTTo.Name = "txtShowsTXTTo";
            this.txtShowsTXTTo.Size = new System.Drawing.Size(342, 20);
            this.txtShowsTXTTo.TabIndex = 4;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.bnBrowseFOXML);
            this.groupBox5.Controls.Add(this.cbFOXML);
            this.groupBox5.Controls.Add(this.txtFOXML);
            this.groupBox5.Location = new System.Drawing.Point(6, 367);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(497, 55);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Finding and Organising";
            // 
            // bnBrowseFOXML
            // 
            this.bnBrowseFOXML.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseFOXML.Location = new System.Drawing.Point(413, 19);
            this.bnBrowseFOXML.Name = "bnBrowseFOXML";
            this.bnBrowseFOXML.Size = new System.Drawing.Size(75, 23);
            this.bnBrowseFOXML.TabIndex = 2;
            this.bnBrowseFOXML.Text = "Browse...";
            this.bnBrowseFOXML.UseVisualStyleBackColor = true;
            this.bnBrowseFOXML.Click += new System.EventHandler(this.bnBrowseFOXML_Click);
            // 
            // cbFOXML
            // 
            this.cbFOXML.AutoSize = true;
            this.cbFOXML.Location = new System.Drawing.Point(8, 23);
            this.cbFOXML.Name = "cbFOXML";
            this.cbFOXML.Size = new System.Drawing.Size(48, 17);
            this.cbFOXML.TabIndex = 0;
            this.cbFOXML.Text = "XML";
            this.cbFOXML.UseVisualStyleBackColor = true;
            this.cbFOXML.CheckedChanged += new System.EventHandler(this.EnableDisable);
            // 
            // txtFOXML
            // 
            this.txtFOXML.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFOXML.Location = new System.Drawing.Point(64, 21);
            this.txtFOXML.Name = "txtFOXML";
            this.txtFOXML.Size = new System.Drawing.Size(343, 20);
            this.txtFOXML.TabIndex = 1;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.bnBrowseRenamingXML);
            this.groupBox4.Controls.Add(this.cbRenamingXML);
            this.groupBox4.Controls.Add(this.txtRenamingXML);
            this.groupBox4.Location = new System.Drawing.Point(6, 305);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(497, 57);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Renaming";
            // 
            // bnBrowseRenamingXML
            // 
            this.bnBrowseRenamingXML.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseRenamingXML.Location = new System.Drawing.Point(413, 19);
            this.bnBrowseRenamingXML.Name = "bnBrowseRenamingXML";
            this.bnBrowseRenamingXML.Size = new System.Drawing.Size(75, 23);
            this.bnBrowseRenamingXML.TabIndex = 2;
            this.bnBrowseRenamingXML.Text = "Browse...";
            this.bnBrowseRenamingXML.UseVisualStyleBackColor = true;
            this.bnBrowseRenamingXML.Click += new System.EventHandler(this.bnBrowseRenamingXML_Click);
            // 
            // cbRenamingXML
            // 
            this.cbRenamingXML.AutoSize = true;
            this.cbRenamingXML.Location = new System.Drawing.Point(8, 23);
            this.cbRenamingXML.Name = "cbRenamingXML";
            this.cbRenamingXML.Size = new System.Drawing.Size(48, 17);
            this.cbRenamingXML.TabIndex = 0;
            this.cbRenamingXML.Text = "XML";
            this.cbRenamingXML.UseVisualStyleBackColor = true;
            this.cbRenamingXML.CheckedChanged += new System.EventHandler(this.EnableDisable);
            // 
            // txtRenamingXML
            // 
            this.txtRenamingXML.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRenamingXML.Location = new System.Drawing.Point(64, 21);
            this.txtRenamingXML.Name = "txtRenamingXML";
            this.txtRenamingXML.Size = new System.Drawing.Size(343, 20);
            this.txtRenamingXML.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.bnBrowseMissingCSV);
            this.groupBox3.Controls.Add(this.bnBrowseMissingXML);
            this.groupBox3.Controls.Add(this.txtMissingCSV);
            this.groupBox3.Controls.Add(this.cbMissingXML);
            this.groupBox3.Controls.Add(this.cbMissingCSV);
            this.groupBox3.Controls.Add(this.txtMissingXML);
            this.groupBox3.Location = new System.Drawing.Point(6, 147);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(497, 79);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Missing";
            // 
            // bnBrowseMissingCSV
            // 
            this.bnBrowseMissingCSV.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseMissingCSV.Location = new System.Drawing.Point(413, 47);
            this.bnBrowseMissingCSV.Name = "bnBrowseMissingCSV";
            this.bnBrowseMissingCSV.Size = new System.Drawing.Size(75, 23);
            this.bnBrowseMissingCSV.TabIndex = 2;
            this.bnBrowseMissingCSV.Text = "Browse...";
            this.bnBrowseMissingCSV.UseVisualStyleBackColor = true;
            this.bnBrowseMissingCSV.Click += new System.EventHandler(this.bnBrowseMissingCSV_Click);
            // 
            // bnBrowseMissingXML
            // 
            this.bnBrowseMissingXML.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseMissingXML.Location = new System.Drawing.Point(414, 19);
            this.bnBrowseMissingXML.Name = "bnBrowseMissingXML";
            this.bnBrowseMissingXML.Size = new System.Drawing.Size(75, 23);
            this.bnBrowseMissingXML.TabIndex = 5;
            this.bnBrowseMissingXML.Text = "Browse...";
            this.bnBrowseMissingXML.UseVisualStyleBackColor = true;
            this.bnBrowseMissingXML.Click += new System.EventHandler(this.bnBrowseMissingXML_Click);
            // 
            // txtMissingCSV
            // 
            this.txtMissingCSV.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMissingCSV.Location = new System.Drawing.Point(64, 48);
            this.txtMissingCSV.Name = "txtMissingCSV";
            this.txtMissingCSV.Size = new System.Drawing.Size(343, 20);
            this.txtMissingCSV.TabIndex = 1;
            // 
            // cbMissingXML
            // 
            this.cbMissingXML.AutoSize = true;
            this.cbMissingXML.Location = new System.Drawing.Point(8, 23);
            this.cbMissingXML.Name = "cbMissingXML";
            this.cbMissingXML.Size = new System.Drawing.Size(48, 17);
            this.cbMissingXML.TabIndex = 3;
            this.cbMissingXML.Text = "XML";
            this.cbMissingXML.UseVisualStyleBackColor = true;
            this.cbMissingXML.CheckedChanged += new System.EventHandler(this.EnableDisable);
            // 
            // cbMissingCSV
            // 
            this.cbMissingCSV.AutoSize = true;
            this.cbMissingCSV.Location = new System.Drawing.Point(9, 48);
            this.cbMissingCSV.Name = "cbMissingCSV";
            this.cbMissingCSV.Size = new System.Drawing.Size(47, 17);
            this.cbMissingCSV.TabIndex = 0;
            this.cbMissingCSV.Text = "CSV";
            this.cbMissingCSV.UseVisualStyleBackColor = true;
            this.cbMissingCSV.CheckedChanged += new System.EventHandler(this.EnableDisable);
            // 
            // txtMissingXML
            // 
            this.txtMissingXML.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMissingXML.Location = new System.Drawing.Point(64, 21);
            this.txtMissingXML.Name = "txtMissingXML";
            this.txtMissingXML.Size = new System.Drawing.Size(343, 20);
            this.txtMissingXML.TabIndex = 4;
            // 
            // tpScanOptions
            // 
            this.tpScanOptions.Controls.Add(this.cbCheckqBitTorrent);
            this.tpScanOptions.Controls.Add(this.chkAutoMergeLibraryEpisodes);
            this.tpScanOptions.Controls.Add(this.chkAutoMergeDownloadEpisodes);
            this.tpScanOptions.Controls.Add(this.chkPreventMove);
            this.tpScanOptions.Controls.Add(this.label40);
            this.tpScanOptions.Controls.Add(this.cbxUpdateAirDate);
            this.tpScanOptions.Controls.Add(this.label33);
            this.tpScanOptions.Controls.Add(this.cbAutoCreateFolders);
            this.tpScanOptions.Controls.Add(this.label28);
            this.tpScanOptions.Controls.Add(this.cbSearchRSS);
            this.tpScanOptions.Controls.Add(this.cbRenameCheck);
            this.tpScanOptions.Controls.Add(this.cbMissing);
            this.tpScanOptions.Controls.Add(this.cbLeaveOriginals);
            this.tpScanOptions.Controls.Add(this.cbCheckSABnzbd);
            this.tpScanOptions.Controls.Add(this.cbCheckuTorrent);
            this.tpScanOptions.Controls.Add(this.cbSearchLocally);
            this.tpScanOptions.Location = new System.Drawing.Point(4, 40);
            this.tpScanOptions.Name = "tpScanOptions";
            this.tpScanOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tpScanOptions.Size = new System.Drawing.Size(509, 489);
            this.tpScanOptions.TabIndex = 6;
            this.tpScanOptions.Text = "Scan Options";
            this.tpScanOptions.UseVisualStyleBackColor = true;
            // 
            // cbCheckqBitTorrent
            // 
            this.cbCheckqBitTorrent.AutoSize = true;
            this.cbCheckqBitTorrent.Location = new System.Drawing.Point(40, 190);
            this.cbCheckqBitTorrent.Name = "cbCheckqBitTorrent";
            this.cbCheckqBitTorrent.Size = new System.Drawing.Size(145, 17);
            this.cbCheckqBitTorrent.TabIndex = 16;
            this.cbCheckqBitTorrent.Text = "Check &qBitTorrent queue";
            this.cbCheckqBitTorrent.UseVisualStyleBackColor = true;
            // 
            // chkAutoMergeLibraryEpisodes
            // 
            this.chkAutoMergeLibraryEpisodes.AutoSize = true;
            this.chkAutoMergeLibraryEpisodes.Location = new System.Drawing.Point(9, 351);
            this.chkAutoMergeLibraryEpisodes.Name = "chkAutoMergeLibraryEpisodes";
            this.chkAutoMergeLibraryEpisodes.Size = new System.Drawing.Size(306, 17);
            this.chkAutoMergeLibraryEpisodes.TabIndex = 15;
            this.chkAutoMergeLibraryEpisodes.Text = "Automatically create merge rules for merged library episodes";
            this.chkAutoMergeLibraryEpisodes.UseVisualStyleBackColor = true;
            // 
            // chkAutoMergeDownloadEpisodes
            // 
            this.chkAutoMergeDownloadEpisodes.AutoSize = true;
            this.chkAutoMergeDownloadEpisodes.Location = new System.Drawing.Point(9, 329);
            this.chkAutoMergeDownloadEpisodes.Name = "chkAutoMergeDownloadEpisodes";
            this.chkAutoMergeDownloadEpisodes.Size = new System.Drawing.Size(337, 17);
            this.chkAutoMergeDownloadEpisodes.TabIndex = 13;
            this.chkAutoMergeDownloadEpisodes.Text = "Automatically create merge rules for merged downloaded episodes";
            this.chkAutoMergeDownloadEpisodes.UseVisualStyleBackColor = true;
            // 
            // chkPreventMove
            // 
            this.chkPreventMove.AutoSize = true;
            this.chkPreventMove.Location = new System.Drawing.Point(40, 56);
            this.chkPreventMove.Name = "chkPreventMove";
            this.chkPreventMove.Size = new System.Drawing.Size(188, 17);
            this.chkPreventMove.TabIndex = 12;
            this.chkPreventMove.Text = "Pre&vent move of files (just rename)";
            this.chkPreventMove.UseVisualStyleBackColor = true;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(6, 290);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(94, 13);
            this.label40.TabIndex = 11;
            this.label40.Text = "Additional Actions:";
            // 
            // cbxUpdateAirDate
            // 
            this.cbxUpdateAirDate.AutoSize = true;
            this.cbxUpdateAirDate.Location = new System.Drawing.Point(9, 306);
            this.cbxUpdateAirDate.Name = "cbxUpdateAirDate";
            this.cbxUpdateAirDate.Size = new System.Drawing.Size(197, 17);
            this.cbxUpdateAirDate.TabIndex = 10;
            this.cbxUpdateAirDate.Text = "Update files and folders with air date";
            this.cbxUpdateAirDate.UseVisualStyleBackColor = true;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(6, 243);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(80, 13);
            this.label33.TabIndex = 9;
            this.label33.Text = "Folder creation:";
            // 
            // cbAutoCreateFolders
            // 
            this.cbAutoCreateFolders.AutoSize = true;
            this.cbAutoCreateFolders.Location = new System.Drawing.Point(9, 259);
            this.cbAutoCreateFolders.Name = "cbAutoCreateFolders";
            this.cbAutoCreateFolders.Size = new System.Drawing.Size(192, 17);
            this.cbAutoCreateFolders.TabIndex = 8;
            this.cbAutoCreateFolders.Text = "&Automatically create missing folders";
            this.cbAutoCreateFolders.UseVisualStyleBackColor = true;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(6, 13);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(141, 13);
            this.label28.TabIndex = 0;
            this.label28.Text = "\"Scan\" checks and actions:";
            // 
            // cbSearchRSS
            // 
            this.cbSearchRSS.AutoSize = true;
            this.cbSearchRSS.Location = new System.Drawing.Point(40, 211);
            this.cbSearchRSS.Name = "cbSearchRSS";
            this.cbSearchRSS.Size = new System.Drawing.Size(158, 17);
            this.cbSearchRSS.TabIndex = 7;
            this.cbSearchRSS.Text = "&Search RSS for missing files";
            this.cbSearchRSS.UseVisualStyleBackColor = true;
            // 
            // cbRenameCheck
            // 
            this.cbRenameCheck.AutoSize = true;
            this.cbRenameCheck.Checked = true;
            this.cbRenameCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRenameCheck.Location = new System.Drawing.Point(20, 33);
            this.cbRenameCheck.Name = "cbRenameCheck";
            this.cbRenameCheck.Size = new System.Drawing.Size(100, 17);
            this.cbRenameCheck.TabIndex = 1;
            this.cbRenameCheck.Text = "&Rename Check";
            this.cbRenameCheck.UseVisualStyleBackColor = true;
            // 
            // cbMissing
            // 
            this.cbMissing.AutoSize = true;
            this.cbMissing.Checked = true;
            this.cbMissing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMissing.Location = new System.Drawing.Point(20, 79);
            this.cbMissing.Name = "cbMissing";
            this.cbMissing.Size = new System.Drawing.Size(95, 17);
            this.cbMissing.TabIndex = 2;
            this.cbMissing.Text = "&Missing Check";
            this.cbMissing.UseVisualStyleBackColor = true;
            this.cbMissing.CheckedChanged += new System.EventHandler(this.cbMissing_CheckedChanged);
            // 
            // cbLeaveOriginals
            // 
            this.cbLeaveOriginals.AutoSize = true;
            this.cbLeaveOriginals.Location = new System.Drawing.Point(60, 125);
            this.cbLeaveOriginals.Name = "cbLeaveOriginals";
            this.cbLeaveOriginals.Size = new System.Drawing.Size(129, 17);
            this.cbLeaveOriginals.TabIndex = 4;
            this.cbLeaveOriginals.Text = "&Copy files, don\'t move";
            this.cbLeaveOriginals.UseVisualStyleBackColor = true;
            // 
            // cbCheckSABnzbd
            // 
            this.cbCheckSABnzbd.AutoSize = true;
            this.cbCheckSABnzbd.Location = new System.Drawing.Point(40, 171);
            this.cbCheckSABnzbd.Name = "cbCheckSABnzbd";
            this.cbCheckSABnzbd.Size = new System.Drawing.Size(137, 17);
            this.cbCheckSABnzbd.TabIndex = 6;
            this.cbCheckSABnzbd.Text = "Check SA&Bnzbd queue";
            this.cbCheckSABnzbd.UseVisualStyleBackColor = true;
            this.cbCheckSABnzbd.CheckedChanged += new System.EventHandler(this.cbSearchLocally_CheckedChanged);
            // 
            // cbCheckuTorrent
            // 
            this.cbCheckuTorrent.AutoSize = true;
            this.cbCheckuTorrent.Location = new System.Drawing.Point(40, 148);
            this.cbCheckuTorrent.Name = "cbCheckuTorrent";
            this.cbCheckuTorrent.Size = new System.Drawing.Size(133, 17);
            this.cbCheckuTorrent.TabIndex = 5;
            this.cbCheckuTorrent.Text = "C&heck µTorrent queue";
            this.cbCheckuTorrent.UseVisualStyleBackColor = true;
            this.cbCheckuTorrent.CheckedChanged += new System.EventHandler(this.cbSearchLocally_CheckedChanged);
            // 
            // cbSearchLocally
            // 
            this.cbSearchLocally.AutoSize = true;
            this.cbSearchLocally.Checked = true;
            this.cbSearchLocally.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSearchLocally.Location = new System.Drawing.Point(40, 102);
            this.cbSearchLocally.Name = "cbSearchLocally";
            this.cbSearchLocally.Size = new System.Drawing.Size(218, 17);
            this.cbSearchLocally.TabIndex = 3;
            this.cbSearchLocally.Text = "&Look in \"Search Folders\" for missing files";
            this.cbSearchLocally.UseVisualStyleBackColor = true;
            this.cbSearchLocally.CheckedChanged += new System.EventHandler(this.cbSearchLocally_CheckedChanged);
            // 
            // tbFolderDeleting
            // 
            this.tbFolderDeleting.Controls.Add(this.cbCleanUpDownloadDir);
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
            this.tbFolderDeleting.Location = new System.Drawing.Point(4, 40);
            this.tbFolderDeleting.Name = "tbFolderDeleting";
            this.tbFolderDeleting.Padding = new System.Windows.Forms.Padding(3);
            this.tbFolderDeleting.Size = new System.Drawing.Size(509, 489);
            this.tbFolderDeleting.TabIndex = 9;
            this.tbFolderDeleting.Text = "Folder Deleting";
            this.tbFolderDeleting.UseVisualStyleBackColor = true;
            // 
            // cbCleanUpDownloadDir
            // 
            this.cbCleanUpDownloadDir.AutoSize = true;
            this.cbCleanUpDownloadDir.Location = new System.Drawing.Point(16, 244);
            this.cbCleanUpDownloadDir.Name = "cbCleanUpDownloadDir";
            this.cbCleanUpDownloadDir.Size = new System.Drawing.Size(276, 17);
            this.cbCleanUpDownloadDir.TabIndex = 11;
            this.cbCleanUpDownloadDir.Text = "Clean up already copied files from download directory";
            this.cbCleanUpDownloadDir.UseVisualStyleBackColor = true;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(13, 168);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(0, 13);
            this.label32.TabIndex = 6;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(13, 43);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(0, 13);
            this.label30.TabIndex = 1;
            // 
            // txtEmptyMaxSize
            // 
            this.txtEmptyMaxSize.Location = new System.Drawing.Point(218, 189);
            this.txtEmptyMaxSize.Name = "txtEmptyMaxSize";
            this.txtEmptyMaxSize.Size = new System.Drawing.Size(55, 20);
            this.txtEmptyMaxSize.TabIndex = 8;
            // 
            // txtEmptyIgnoreWords
            // 
            this.txtEmptyIgnoreWords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmptyIgnoreWords.Location = new System.Drawing.Point(95, 89);
            this.txtEmptyIgnoreWords.Name = "txtEmptyIgnoreWords";
            this.txtEmptyIgnoreWords.Size = new System.Drawing.Size(408, 20);
            this.txtEmptyIgnoreWords.TabIndex = 3;
            this.toolTip1.SetToolTip(this.txtEmptyIgnoreWords, "For example \"sample\"");
            // 
            // txtEmptyIgnoreExtensions
            // 
            this.txtEmptyIgnoreExtensions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmptyIgnoreExtensions.Location = new System.Drawing.Point(95, 139);
            this.txtEmptyIgnoreExtensions.Name = "txtEmptyIgnoreExtensions";
            this.txtEmptyIgnoreExtensions.Size = new System.Drawing.Size(408, 20);
            this.txtEmptyIgnoreExtensions.TabIndex = 5;
            this.toolTip1.SetToolTip(this.txtEmptyIgnoreExtensions, "For example \".par2;.nzb;.nfo\"");
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(281, 193);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(0, 13);
            this.label31.TabIndex = 9;
            // 
            // cbRecycleNotDelete
            // 
            this.cbRecycleNotDelete.AutoSize = true;
            this.cbRecycleNotDelete.Location = new System.Drawing.Point(16, 221);
            this.cbRecycleNotDelete.Name = "cbRecycleNotDelete";
            this.cbRecycleNotDelete.Size = new System.Drawing.Size(299, 17);
            this.cbRecycleNotDelete.TabIndex = 10;
            this.cbRecycleNotDelete.Text = "Folders with files are moved to the &recycle bin, not deleted";
            this.cbRecycleNotDelete.UseVisualStyleBackColor = true;
            // 
            // cbEmptyMaxSize
            // 
            this.cbEmptyMaxSize.AutoSize = true;
            this.cbEmptyMaxSize.Location = new System.Drawing.Point(35, 191);
            this.cbEmptyMaxSize.Name = "cbEmptyMaxSize";
            this.cbEmptyMaxSize.Size = new System.Drawing.Size(177, 17);
            this.cbEmptyMaxSize.TabIndex = 7;
            this.cbEmptyMaxSize.Text = "&Maximum total file size to delete:";
            this.cbEmptyMaxSize.UseVisualStyleBackColor = true;
            // 
            // cbEmptyIgnoreWords
            // 
            this.cbEmptyIgnoreWords.AutoSize = true;
            this.cbEmptyIgnoreWords.Location = new System.Drawing.Point(35, 66);
            this.cbEmptyIgnoreWords.Name = "cbEmptyIgnoreWords";
            this.cbEmptyIgnoreWords.Size = new System.Drawing.Size(366, 17);
            this.cbEmptyIgnoreWords.TabIndex = 2;
            this.cbEmptyIgnoreWords.Text = "Ignore any files with these &words in their name: (semicolon separated list)";
            this.cbEmptyIgnoreWords.UseVisualStyleBackColor = true;
            // 
            // cbEmptyIgnoreExtensions
            // 
            this.cbEmptyIgnoreExtensions.AutoSize = true;
            this.cbEmptyIgnoreExtensions.Location = new System.Drawing.Point(35, 116);
            this.cbEmptyIgnoreExtensions.Name = "cbEmptyIgnoreExtensions";
            this.cbEmptyIgnoreExtensions.Size = new System.Drawing.Size(305, 17);
            this.cbEmptyIgnoreExtensions.TabIndex = 4;
            this.cbEmptyIgnoreExtensions.Text = "&Ignore files with these extensions: (semicolon separated list)";
            this.cbEmptyIgnoreExtensions.UseVisualStyleBackColor = true;
            // 
            // cbDeleteEmpty
            // 
            this.cbDeleteEmpty.AutoSize = true;
            this.cbDeleteEmpty.Location = new System.Drawing.Point(16, 18);
            this.cbDeleteEmpty.Name = "cbDeleteEmpty";
            this.cbDeleteEmpty.Size = new System.Drawing.Size(204, 17);
            this.cbDeleteEmpty.TabIndex = 0;
            this.cbDeleteEmpty.Text = "&Delete empty folders after moving files";
            this.cbDeleteEmpty.UseVisualStyleBackColor = true;
            // 
            // tbMediaCenter
            // 
            this.tbMediaCenter.Controls.Add(this.cbWDLiveEpisodeFiles);
            this.tbMediaCenter.Controls.Add(this.cbNFOEpisodes);
            this.tbMediaCenter.Controls.Add(this.panel1);
            this.tbMediaCenter.Controls.Add(this.cbKODIImages);
            this.tbMediaCenter.Controls.Add(this.bnMCPresets);
            this.tbMediaCenter.Controls.Add(this.cbShrinkLarge);
            this.tbMediaCenter.Controls.Add(this.cbEpThumbJpg);
            this.tbMediaCenter.Controls.Add(this.label29);
            this.tbMediaCenter.Controls.Add(this.label24);
            this.tbMediaCenter.Controls.Add(this.label18);
            this.tbMediaCenter.Controls.Add(this.label12);
            this.tbMediaCenter.Controls.Add(this.cbMetaSubfolder);
            this.tbMediaCenter.Controls.Add(this.cbMeta);
            this.tbMediaCenter.Controls.Add(this.cbEpTBNs);
            this.tbMediaCenter.Controls.Add(this.cbSeriesJpg);
            this.tbMediaCenter.Controls.Add(this.cbXMLFiles);
            this.tbMediaCenter.Controls.Add(this.cbNFOShows);
            this.tbMediaCenter.Controls.Add(this.cbFantArtJpg);
            this.tbMediaCenter.Controls.Add(this.cbFolderJpg);
            this.tbMediaCenter.Location = new System.Drawing.Point(4, 40);
            this.tbMediaCenter.Name = "tbMediaCenter";
            this.tbMediaCenter.Padding = new System.Windows.Forms.Padding(3);
            this.tbMediaCenter.Size = new System.Drawing.Size(509, 489);
            this.tbMediaCenter.TabIndex = 8;
            this.tbMediaCenter.Text = "Media Centres";
            this.tbMediaCenter.UseVisualStyleBackColor = true;
            // 
            // cbWDLiveEpisodeFiles
            // 
            this.cbWDLiveEpisodeFiles.AutoSize = true;
            this.cbWDLiveEpisodeFiles.Location = new System.Drawing.Point(38, 399);
            this.cbWDLiveEpisodeFiles.Name = "cbWDLiveEpisodeFiles";
            this.cbWDLiveEpisodeFiles.Size = new System.Drawing.Size(200, 17);
            this.cbWDLiveEpisodeFiles.TabIndex = 25;
            this.cbWDLiveEpisodeFiles.Text = "WD TV Live Hub Episode Files (.xml)";
            this.cbWDLiveEpisodeFiles.UseVisualStyleBackColor = true;
            // 
            // cbNFOEpisodes
            // 
            this.cbNFOEpisodes.AutoSize = true;
            this.cbNFOEpisodes.Location = new System.Drawing.Point(38, 73);
            this.cbNFOEpisodes.Name = "cbNFOEpisodes";
            this.cbNFOEpisodes.Size = new System.Drawing.Size(129, 17);
            this.cbNFOEpisodes.TabIndex = 24;
            this.cbNFOEpisodes.Text = "&NFO files for episodes";
            this.cbNFOEpisodes.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbFolderBanner);
            this.panel1.Controls.Add(this.rbFolderPoster);
            this.panel1.Controls.Add(this.rbFolderFanArt);
            this.panel1.Controls.Add(this.rbFolderSeasonPoster);
            this.panel1.Location = new System.Drawing.Point(59, 323);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(280, 24);
            this.panel1.TabIndex = 22;
            // 
            // rbFolderBanner
            // 
            this.rbFolderBanner.AutoSize = true;
            this.rbFolderBanner.Location = new System.Drawing.Point(0, 7);
            this.rbFolderBanner.Name = "rbFolderBanner";
            this.rbFolderBanner.Size = new System.Drawing.Size(59, 17);
            this.rbFolderBanner.TabIndex = 12;
            this.rbFolderBanner.TabStop = true;
            this.rbFolderBanner.Text = "&Banner";
            this.rbFolderBanner.UseVisualStyleBackColor = true;
            // 
            // rbFolderPoster
            // 
            this.rbFolderPoster.AutoSize = true;
            this.rbFolderPoster.Location = new System.Drawing.Point(60, 7);
            this.rbFolderPoster.Name = "rbFolderPoster";
            this.rbFolderPoster.Size = new System.Drawing.Size(55, 17);
            this.rbFolderPoster.TabIndex = 13;
            this.rbFolderPoster.TabStop = true;
            this.rbFolderPoster.Text = "&Poster";
            this.rbFolderPoster.UseVisualStyleBackColor = true;
            // 
            // rbFolderFanArt
            // 
            this.rbFolderFanArt.AutoSize = true;
            this.rbFolderFanArt.Location = new System.Drawing.Point(121, 7);
            this.rbFolderFanArt.Name = "rbFolderFanArt";
            this.rbFolderFanArt.Size = new System.Drawing.Size(59, 17);
            this.rbFolderFanArt.TabIndex = 14;
            this.rbFolderFanArt.TabStop = true;
            this.rbFolderFanArt.Text = "Fan A&rt";
            this.rbFolderFanArt.UseVisualStyleBackColor = true;
            // 
            // rbFolderSeasonPoster
            // 
            this.rbFolderSeasonPoster.AutoSize = true;
            this.rbFolderSeasonPoster.Location = new System.Drawing.Point(186, 7);
            this.rbFolderSeasonPoster.Name = "rbFolderSeasonPoster";
            this.rbFolderSeasonPoster.Size = new System.Drawing.Size(94, 17);
            this.rbFolderSeasonPoster.TabIndex = 16;
            this.rbFolderSeasonPoster.TabStop = true;
            this.rbFolderSeasonPoster.Text = "Seaso&n Poster";
            this.rbFolderSeasonPoster.UseVisualStyleBackColor = true;
            // 
            // cbKODIImages
            // 
            this.cbKODIImages.AutoSize = true;
            this.cbKODIImages.Location = new System.Drawing.Point(38, 93);
            this.cbKODIImages.Name = "cbKODIImages";
            this.cbKODIImages.Size = new System.Drawing.Size(271, 17);
            this.cbKODIImages.TabIndex = 17;
            this.cbKODIImages.Text = "Download XMBC &Images (fanart, poster, banner.jpg)";
            this.cbKODIImages.UseVisualStyleBackColor = true;
            // 
            // bnMCPresets
            // 
            this.bnMCPresets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnMCPresets.Location = new System.Drawing.Point(428, 399);
            this.bnMCPresets.Name = "bnMCPresets";
            this.bnMCPresets.Size = new System.Drawing.Size(75, 23);
            this.bnMCPresets.TabIndex = 16;
            this.bnMCPresets.Text = "Pre&sets...";
            this.bnMCPresets.UseVisualStyleBackColor = true;
            this.bnMCPresets.Click += new System.EventHandler(this.bnMCPresets_Click);
            // 
            // cbShrinkLarge
            // 
            this.cbShrinkLarge.AutoSize = true;
            this.cbShrinkLarge.Location = new System.Drawing.Point(38, 260);
            this.cbShrinkLarge.Name = "cbShrinkLarge";
            this.cbShrinkLarge.Size = new System.Drawing.Size(300, 17);
            this.cbShrinkLarge.TabIndex = 9;
            this.cbShrinkLarge.Text = "S&hrink large series and episode images to 156 x 232 pixels";
            this.cbShrinkLarge.UseVisualStyleBackColor = true;
            // 
            // cbEpThumbJpg
            // 
            this.cbEpThumbJpg.AutoSize = true;
            this.cbEpThumbJpg.Location = new System.Drawing.Point(38, 376);
            this.cbEpThumbJpg.Name = "cbEpThumbJpg";
            this.cbEpThumbJpg.Size = new System.Drawing.Size(147, 17);
            this.cbEpThumbJpg.TabIndex = 16;
            this.cbEpThumbJpg.Text = "Episode Thumbnails (.&jpg)";
            this.cbEpThumbJpg.UseVisualStyleBackColor = true;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(17, 194);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(49, 13);
            this.label29.TabIndex = 6;
            this.label29.Text = "Mede8er";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(17, 289);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(44, 13);
            this.label24.TabIndex = 10;
            this.label24.Text = "General";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(17, 123);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(39, 13);
            this.label18.TabIndex = 3;
            this.label18.Text = "pyTivo";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(17, 15);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(33, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "KODI";
            // 
            // cbMetaSubfolder
            // 
            this.cbMetaSubfolder.AutoSize = true;
            this.cbMetaSubfolder.Location = new System.Drawing.Point(38, 165);
            this.cbMetaSubfolder.Name = "cbMetaSubfolder";
            this.cbMetaSubfolder.Size = new System.Drawing.Size(187, 17);
            this.cbMetaSubfolder.TabIndex = 5;
            this.cbMetaSubfolder.Text = "Pl&ace Meta files in .meta subfolder";
            this.cbMetaSubfolder.UseVisualStyleBackColor = true;
            // 
            // cbMeta
            // 
            this.cbMeta.AutoSize = true;
            this.cbMeta.Location = new System.Drawing.Point(38, 142);
            this.cbMeta.Name = "cbMeta";
            this.cbMeta.Size = new System.Drawing.Size(154, 17);
            this.cbMeta.TabIndex = 4;
            this.cbMeta.Text = "&Meta files for episodes (.txt)";
            this.cbMeta.UseVisualStyleBackColor = true;
            this.cbMeta.CheckedChanged += new System.EventHandler(this.cbMeta_CheckedChanged);
            // 
            // cbEpTBNs
            // 
            this.cbEpTBNs.AutoSize = true;
            this.cbEpTBNs.Location = new System.Drawing.Point(38, 34);
            this.cbEpTBNs.Name = "cbEpTBNs";
            this.cbEpTBNs.Size = new System.Drawing.Size(148, 17);
            this.cbEpTBNs.TabIndex = 1;
            this.cbEpTBNs.Text = "&Episode Thumbnails (.tbn)";
            this.cbEpTBNs.UseVisualStyleBackColor = true;
            // 
            // cbSeriesJpg
            // 
            this.cbSeriesJpg.AutoSize = true;
            this.cbSeriesJpg.Location = new System.Drawing.Point(38, 214);
            this.cbSeriesJpg.Name = "cbSeriesJpg";
            this.cbSeriesJpg.Size = new System.Drawing.Size(172, 17);
            this.cbSeriesJpg.TabIndex = 7;
            this.cbSeriesJpg.Text = "&Create series poster (series.jpg)";
            this.cbSeriesJpg.UseVisualStyleBackColor = true;
            // 
            // cbXMLFiles
            // 
            this.cbXMLFiles.AutoSize = true;
            this.cbXMLFiles.Location = new System.Drawing.Point(38, 237);
            this.cbXMLFiles.Name = "cbXMLFiles";
            this.cbXMLFiles.Size = new System.Drawing.Size(183, 17);
            this.cbXMLFiles.TabIndex = 8;
            this.cbXMLFiles.Text = "&XML files for shows and episodes";
            this.cbXMLFiles.UseVisualStyleBackColor = true;
            // 
            // cbNFOShows
            // 
            this.cbNFOShows.AutoSize = true;
            this.cbNFOShows.Location = new System.Drawing.Point(38, 55);
            this.cbNFOShows.Name = "cbNFOShows";
            this.cbNFOShows.Size = new System.Drawing.Size(117, 17);
            this.cbNFOShows.TabIndex = 2;
            this.cbNFOShows.Text = "&NFO files for shows";
            this.cbNFOShows.UseVisualStyleBackColor = true;
            // 
            // cbFantArtJpg
            // 
            this.cbFantArtJpg.AutoSize = true;
            this.cbFantArtJpg.Location = new System.Drawing.Point(38, 353);
            this.cbFantArtJpg.Name = "cbFantArtJpg";
            this.cbFantArtJpg.Size = new System.Drawing.Size(141, 17);
            this.cbFantArtJpg.TabIndex = 15;
            this.cbFantArtJpg.Text = "Fanar&t Image (fanart.jpg)";
            this.cbFantArtJpg.UseVisualStyleBackColor = true;
            // 
            // cbFolderJpg
            // 
            this.cbFolderJpg.AutoSize = true;
            this.cbFolderJpg.Location = new System.Drawing.Point(38, 310);
            this.cbFolderJpg.Name = "cbFolderJpg";
            this.cbFolderJpg.Size = new System.Drawing.Size(138, 17);
            this.cbFolderJpg.TabIndex = 11;
            this.cbFolderJpg.Text = "&Folder image (folder.jpg)";
            this.cbFolderJpg.UseVisualStyleBackColor = true;
            // 
            // tbSearchFolders
            // 
            this.tbSearchFolders.Controls.Add(this.label1);
            this.tbSearchFolders.Controls.Add(this.domainUpDown1);
            this.tbSearchFolders.Controls.Add(this.chkScheduledScan);
            this.tbSearchFolders.Controls.Add(this.chkScanOnStartup);
            this.tbSearchFolders.Controls.Add(this.lblScanAction);
            this.tbSearchFolders.Controls.Add(this.rdoQuickScan);
            this.tbSearchFolders.Controls.Add(this.rdoRecentScan);
            this.tbSearchFolders.Controls.Add(this.rdoFullScan);
            this.tbSearchFolders.Controls.Add(this.cbMonitorFolder);
            this.tbSearchFolders.Controls.Add(this.bnOpenSearchFolder);
            this.tbSearchFolders.Controls.Add(this.bnRemoveSearchFolder);
            this.tbSearchFolders.Controls.Add(this.bnAddSearchFolder);
            this.tbSearchFolders.Controls.Add(this.lbSearchFolders);
            this.tbSearchFolders.Controls.Add(this.label23);
            this.tbSearchFolders.Location = new System.Drawing.Point(4, 40);
            this.tbSearchFolders.Name = "tbSearchFolders";
            this.tbSearchFolders.Size = new System.Drawing.Size(509, 489);
            this.tbSearchFolders.TabIndex = 3;
            this.tbSearchFolders.Text = "Search Folders";
            this.tbSearchFolders.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(178, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "hours";
            // 
            // domainUpDown1
            // 
            this.domainUpDown1.Items.Add("1");
            this.domainUpDown1.Items.Add("2");
            this.domainUpDown1.Items.Add("3");
            this.domainUpDown1.Items.Add("4");
            this.domainUpDown1.Items.Add("5");
            this.domainUpDown1.Items.Add("6");
            this.domainUpDown1.Items.Add("8");
            this.domainUpDown1.Items.Add("12");
            this.domainUpDown1.Items.Add("24");
            this.domainUpDown1.Items.Add("48");
            this.domainUpDown1.Items.Add("96");
            this.domainUpDown1.Location = new System.Drawing.Point(134, 87);
            this.domainUpDown1.Name = "domainUpDown1";
            this.domainUpDown1.Size = new System.Drawing.Size(40, 20);
            this.domainUpDown1.TabIndex = 25;
            this.domainUpDown1.Text = "1";
            this.domainUpDown1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.domainUpDown1_KeyDown);
            // 
            // chkScheduledScan
            // 
            this.chkScheduledScan.AutoSize = true;
            this.chkScheduledScan.Location = new System.Drawing.Point(3, 88);
            this.chkScheduledScan.Name = "chkScheduledScan";
            this.chkScheduledScan.Size = new System.Drawing.Size(135, 17);
            this.chkScheduledScan.TabIndex = 24;
            this.chkScheduledScan.Text = "Sc&heduled scan every ";
            this.toolTip1.SetToolTip(this.chkScheduledScan, "If checked the system will automatically scan and complete actions on startup");
            this.chkScheduledScan.UseVisualStyleBackColor = true;
            // 
            // chkScanOnStartup
            // 
            this.chkScanOnStartup.AutoSize = true;
            this.chkScanOnStartup.Location = new System.Drawing.Point(3, 65);
            this.chkScanOnStartup.Name = "chkScanOnStartup";
            this.chkScanOnStartup.Size = new System.Drawing.Size(103, 17);
            this.chkScanOnStartup.TabIndex = 23;
            this.chkScanOnStartup.Text = "&Scan on Startup";
            this.toolTip1.SetToolTip(this.chkScanOnStartup, "If checked the system will automatically scan and complete actions on startup");
            this.chkScanOnStartup.UseVisualStyleBackColor = true;
            // 
            // lblScanAction
            // 
            this.lblScanAction.AutoSize = true;
            this.lblScanAction.Location = new System.Drawing.Point(2, 12);
            this.lblScanAction.Name = "lblScanAction";
            this.lblScanAction.Size = new System.Drawing.Size(59, 13);
            this.lblScanAction.TabIndex = 22;
            this.lblScanAction.Text = "&Scan Type";
            // 
            // rdoQuickScan
            // 
            this.rdoQuickScan.AutoSize = true;
            this.rdoQuickScan.Location = new System.Drawing.Point(134, 28);
            this.rdoQuickScan.Name = "rdoQuickScan";
            this.rdoQuickScan.Size = new System.Drawing.Size(53, 17);
            this.rdoQuickScan.TabIndex = 20;
            this.rdoQuickScan.TabStop = true;
            this.rdoQuickScan.Text = "&Quick";
            this.rdoQuickScan.UseVisualStyleBackColor = true;
            // 
            // rdoRecentScan
            // 
            this.rdoRecentScan.AutoSize = true;
            this.rdoRecentScan.Location = new System.Drawing.Point(68, 28);
            this.rdoRecentScan.Name = "rdoRecentScan";
            this.rdoRecentScan.Size = new System.Drawing.Size(60, 17);
            this.rdoRecentScan.TabIndex = 19;
            this.rdoRecentScan.TabStop = true;
            this.rdoRecentScan.Text = "&Recent";
            this.rdoRecentScan.UseVisualStyleBackColor = true;
            // 
            // rdoFullScan
            // 
            this.rdoFullScan.AutoSize = true;
            this.rdoFullScan.Location = new System.Drawing.Point(21, 28);
            this.rdoFullScan.Name = "rdoFullScan";
            this.rdoFullScan.Size = new System.Drawing.Size(41, 17);
            this.rdoFullScan.TabIndex = 18;
            this.rdoFullScan.TabStop = true;
            this.rdoFullScan.Text = "&Full";
            this.rdoFullScan.UseVisualStyleBackColor = true;
            // 
            // cbMonitorFolder
            // 
            this.cbMonitorFolder.AutoSize = true;
            this.cbMonitorFolder.Location = new System.Drawing.Point(3, 111);
            this.cbMonitorFolder.Name = "cbMonitorFolder";
            this.cbMonitorFolder.Size = new System.Drawing.Size(154, 17);
            this.cbMonitorFolder.TabIndex = 5;
            this.cbMonitorFolder.Text = "&Monitor folders for changes";
            this.toolTip1.SetToolTip(this.cbMonitorFolder, "If the contents of any of these folder change, then automatically do a \"Scan\" and" +
        " \"Do\".");
            this.cbMonitorFolder.UseVisualStyleBackColor = true;
            // 
            // bnOpenSearchFolder
            // 
            this.bnOpenSearchFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnOpenSearchFolder.Location = new System.Drawing.Point(165, 458);
            this.bnOpenSearchFolder.Name = "bnOpenSearchFolder";
            this.bnOpenSearchFolder.Size = new System.Drawing.Size(75, 23);
            this.bnOpenSearchFolder.TabIndex = 4;
            this.bnOpenSearchFolder.Text = "&Open";
            this.bnOpenSearchFolder.UseVisualStyleBackColor = true;
            this.bnOpenSearchFolder.Click += new System.EventHandler(this.bnOpenSearchFolder_Click);
            // 
            // bnRemoveSearchFolder
            // 
            this.bnRemoveSearchFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnRemoveSearchFolder.Location = new System.Drawing.Point(84, 458);
            this.bnRemoveSearchFolder.Name = "bnRemoveSearchFolder";
            this.bnRemoveSearchFolder.Size = new System.Drawing.Size(75, 23);
            this.bnRemoveSearchFolder.TabIndex = 3;
            this.bnRemoveSearchFolder.Text = "&Remove";
            this.bnRemoveSearchFolder.UseVisualStyleBackColor = true;
            this.bnRemoveSearchFolder.Click += new System.EventHandler(this.bnRemoveSearchFolder_Click);
            // 
            // bnAddSearchFolder
            // 
            this.bnAddSearchFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnAddSearchFolder.Location = new System.Drawing.Point(3, 458);
            this.bnAddSearchFolder.Name = "bnAddSearchFolder";
            this.bnAddSearchFolder.Size = new System.Drawing.Size(75, 23);
            this.bnAddSearchFolder.TabIndex = 2;
            this.bnAddSearchFolder.Text = "&Add";
            this.bnAddSearchFolder.UseVisualStyleBackColor = true;
            this.bnAddSearchFolder.Click += new System.EventHandler(this.bnAddSearchFolder_Click);
            // 
            // lbSearchFolders
            // 
            this.lbSearchFolders.AllowDrop = true;
            this.lbSearchFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSearchFolders.FormattingEnabled = true;
            this.lbSearchFolders.Location = new System.Drawing.Point(3, 153);
            this.lbSearchFolders.Name = "lbSearchFolders";
            this.lbSearchFolders.ScrollAlwaysVisible = true;
            this.lbSearchFolders.Size = new System.Drawing.Size(503, 290);
            this.lbSearchFolders.TabIndex = 1;
            this.lbSearchFolders.DragDrop += new System.Windows.Forms.DragEventHandler(this.lbSearchFolders_DragDrop);
            this.lbSearchFolders.DragOver += new System.Windows.Forms.DragEventHandler(this.lbSearchFolders_DragOver);
            this.lbSearchFolders.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbSearchFolders_KeyDown);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(5, 131);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(78, 13);
            this.label23.TabIndex = 0;
            this.label23.Text = "&Search Folders";
            // 
            // tbuTorrentNZB
            // 
            this.tbuTorrentNZB.Controls.Add(this.qBitTorrent);
            this.tbuTorrentNZB.Controls.Add(this.label45);
            this.tbuTorrentNZB.Controls.Add(this.tbPreferredRSSTerms);
            this.tbuTorrentNZB.Controls.Add(this.groupBox1);
            this.tbuTorrentNZB.Controls.Add(this.groupBox6);
            this.tbuTorrentNZB.Controls.Add(this.RSSGrid);
            this.tbuTorrentNZB.Controls.Add(this.bnRSSRemove);
            this.tbuTorrentNZB.Controls.Add(this.bnRSSAdd);
            this.tbuTorrentNZB.Controls.Add(this.bnRSSGo);
            this.tbuTorrentNZB.Controls.Add(this.label25);
            this.tbuTorrentNZB.Location = new System.Drawing.Point(4, 40);
            this.tbuTorrentNZB.Name = "tbuTorrentNZB";
            this.tbuTorrentNZB.Padding = new System.Windows.Forms.Padding(3);
            this.tbuTorrentNZB.Size = new System.Drawing.Size(509, 489);
            this.tbuTorrentNZB.TabIndex = 4;
            this.tbuTorrentNZB.Text = "Torrents / NZB";
            this.tbuTorrentNZB.UseVisualStyleBackColor = true;
            // 
            // qBitTorrent
            // 
            this.qBitTorrent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.qBitTorrent.Controls.Add(this.tbqBitTorrentHost);
            this.qBitTorrent.Controls.Add(this.tbqBitTorrentPort);
            this.qBitTorrent.Controls.Add(this.label41);
            this.qBitTorrent.Controls.Add(this.label42);
            this.qBitTorrent.Location = new System.Drawing.Point(3, 336);
            this.qBitTorrent.Name = "qBitTorrent";
            this.qBitTorrent.Size = new System.Drawing.Size(500, 81);
            this.qBitTorrent.TabIndex = 7;
            this.qBitTorrent.TabStop = false;
            this.qBitTorrent.Text = "qBitTorrent";
            // 
            // tbqBitTorrentHost
            // 
            this.tbqBitTorrentHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbqBitTorrentHost.Location = new System.Drawing.Point(75, 19);
            this.tbqBitTorrentHost.Name = "tbqBitTorrentHost";
            this.tbqBitTorrentHost.Size = new System.Drawing.Size(419, 20);
            this.tbqBitTorrentHost.TabIndex = 1;
            // 
            // tbqBitTorrentPort
            // 
            this.tbqBitTorrentPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbqBitTorrentPort.Location = new System.Drawing.Point(75, 48);
            this.tbqBitTorrentPort.Name = "tbqBitTorrentPort";
            this.tbqBitTorrentPort.Size = new System.Drawing.Size(419, 20);
            this.tbqBitTorrentPort.TabIndex = 4;
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(11, 51);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(29, 13);
            this.label41.TabIndex = 3;
            this.label41.Text = "Port:";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(11, 22);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(32, 13);
            this.label42.TabIndex = 0;
            this.label42.Text = "Host:";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(150, 9);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(85, 13);
            this.label45.TabIndex = 24;
            this.label45.Text = "Preferred Terms:";
            // 
            // tbPreferredRSSTerms
            // 
            this.tbPreferredRSSTerms.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPreferredRSSTerms.Location = new System.Drawing.Point(241, 6);
            this.tbPreferredRSSTerms.Name = "tbPreferredRSSTerms";
            this.tbPreferredRSSTerms.Size = new System.Drawing.Size(262, 20);
            this.tbPreferredRSSTerms.TabIndex = 23;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtSABHostPort);
            this.groupBox1.Controls.Add(this.txtSABAPIKey);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Location = new System.Drawing.Point(3, 163);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(500, 81);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SABnzbd";
            // 
            // txtSABHostPort
            // 
            this.txtSABHostPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSABHostPort.Location = new System.Drawing.Point(75, 19);
            this.txtSABHostPort.Name = "txtSABHostPort";
            this.txtSABHostPort.Size = new System.Drawing.Size(419, 20);
            this.txtSABHostPort.TabIndex = 1;
            // 
            // txtSABAPIKey
            // 
            this.txtSABAPIKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSABAPIKey.Location = new System.Drawing.Point(75, 48);
            this.txtSABAPIKey.Name = "txtSABAPIKey";
            this.txtSABAPIKey.Size = new System.Drawing.Size(419, 20);
            this.txtSABAPIKey.TabIndex = 4;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 51);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "API Key:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(51, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Host:Port";
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox6.Controls.Add(this.bnUTBrowseResumeDat);
            this.groupBox6.Controls.Add(this.txtUTResumeDatPath);
            this.groupBox6.Controls.Add(this.bnRSSBrowseuTorrent);
            this.groupBox6.Controls.Add(this.label27);
            this.groupBox6.Controls.Add(this.label26);
            this.groupBox6.Controls.Add(this.txtRSSuTorrentPath);
            this.groupBox6.Location = new System.Drawing.Point(3, 250);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(500, 80);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "µTorrent";
            // 
            // bnUTBrowseResumeDat
            // 
            this.bnUTBrowseResumeDat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnUTBrowseResumeDat.Location = new System.Drawing.Point(419, 46);
            this.bnUTBrowseResumeDat.Name = "bnUTBrowseResumeDat";
            this.bnUTBrowseResumeDat.Size = new System.Drawing.Size(75, 23);
            this.bnUTBrowseResumeDat.TabIndex = 5;
            this.bnUTBrowseResumeDat.Text = "Bro&wse...";
            this.bnUTBrowseResumeDat.UseVisualStyleBackColor = true;
            this.bnUTBrowseResumeDat.Click += new System.EventHandler(this.bnUTBrowseResumeDat_Click);
            // 
            // txtUTResumeDatPath
            // 
            this.txtUTResumeDatPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUTResumeDatPath.Location = new System.Drawing.Point(75, 48);
            this.txtUTResumeDatPath.Name = "txtUTResumeDatPath";
            this.txtUTResumeDatPath.Size = new System.Drawing.Size(338, 20);
            this.txtUTResumeDatPath.TabIndex = 4;
            // 
            // bnRSSBrowseuTorrent
            // 
            this.bnRSSBrowseuTorrent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnRSSBrowseuTorrent.Location = new System.Drawing.Point(419, 16);
            this.bnRSSBrowseuTorrent.Name = "bnRSSBrowseuTorrent";
            this.bnRSSBrowseuTorrent.Size = new System.Drawing.Size(75, 23);
            this.bnRSSBrowseuTorrent.TabIndex = 2;
            this.bnRSSBrowseuTorrent.Text = "&Browse...";
            this.bnRSSBrowseuTorrent.UseVisualStyleBackColor = true;
            this.bnRSSBrowseuTorrent.Click += new System.EventHandler(this.bnRSSBrowseuTorrent_Click);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(7, 22);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(62, 13);
            this.label27.TabIndex = 0;
            this.label27.Text = "A&pplication:";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(7, 51);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(62, 13);
            this.label26.TabIndex = 3;
            this.label26.Text = "resume.&dat:";
            // 
            // txtRSSuTorrentPath
            // 
            this.txtRSSuTorrentPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRSSuTorrentPath.Location = new System.Drawing.Point(75, 19);
            this.txtRSSuTorrentPath.Name = "txtRSSuTorrentPath";
            this.txtRSSuTorrentPath.Size = new System.Drawing.Size(338, 20);
            this.txtRSSuTorrentPath.TabIndex = 1;
            // 
            // RSSGrid
            // 
            this.RSSGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RSSGrid.BackColor = System.Drawing.SystemColors.Window;
            this.RSSGrid.EnableSort = true;
            this.RSSGrid.Location = new System.Drawing.Point(3, 36);
            this.RSSGrid.Name = "RSSGrid";
            this.RSSGrid.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.RSSGrid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.RSSGrid.Size = new System.Drawing.Size(500, 91);
            this.RSSGrid.TabIndex = 1;
            this.RSSGrid.TabStop = true;
            this.RSSGrid.ToolTipText = "";
            // 
            // bnRSSRemove
            // 
            this.bnRSSRemove.Location = new System.Drawing.Point(84, 136);
            this.bnRSSRemove.Name = "bnRSSRemove";
            this.bnRSSRemove.Size = new System.Drawing.Size(75, 23);
            this.bnRSSRemove.TabIndex = 3;
            this.bnRSSRemove.Text = "&Remove";
            this.bnRSSRemove.UseVisualStyleBackColor = true;
            this.bnRSSRemove.Click += new System.EventHandler(this.bnRSSRemove_Click);
            // 
            // bnRSSAdd
            // 
            this.bnRSSAdd.Location = new System.Drawing.Point(3, 136);
            this.bnRSSAdd.Name = "bnRSSAdd";
            this.bnRSSAdd.Size = new System.Drawing.Size(75, 23);
            this.bnRSSAdd.TabIndex = 2;
            this.bnRSSAdd.Text = "&Add";
            this.bnRSSAdd.UseVisualStyleBackColor = true;
            this.bnRSSAdd.Click += new System.EventHandler(this.bnRSSAdd_Click);
            // 
            // bnRSSGo
            // 
            this.bnRSSGo.Location = new System.Drawing.Point(165, 136);
            this.bnRSSGo.Name = "bnRSSGo";
            this.bnRSSGo.Size = new System.Drawing.Size(75, 23);
            this.bnRSSGo.TabIndex = 4;
            this.bnRSSGo.Text = "&Open";
            this.bnRSSGo.UseVisualStyleBackColor = true;
            this.bnRSSGo.Click += new System.EventHandler(this.bnRSSGo_Click);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(3, 13);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(99, 13);
            this.label25.TabIndex = 0;
            this.label25.Text = "Torrent RSS URLs:";
            // 
            // tpTreeColoring
            // 
            this.tpTreeColoring.Controls.Add(this.label7);
            this.tpTreeColoring.Controls.Add(this.cboShowStatus);
            this.tpTreeColoring.Controls.Add(this.label5);
            this.tpTreeColoring.Controls.Add(this.txtShowStatusColor);
            this.tpTreeColoring.Controls.Add(this.btnSelectColor);
            this.tpTreeColoring.Controls.Add(this.bnRemoveDefinedColor);
            this.tpTreeColoring.Controls.Add(this.btnAddShowStatusColoring);
            this.tpTreeColoring.Controls.Add(this.lvwDefinedColors);
            this.tpTreeColoring.Location = new System.Drawing.Point(4, 40);
            this.tpTreeColoring.Name = "tpTreeColoring";
            this.tpTreeColoring.Padding = new System.Windows.Forms.Padding(3);
            this.tpTreeColoring.Size = new System.Drawing.Size(509, 489);
            this.tpTreeColoring.TabIndex = 7;
            this.tpTreeColoring.Text = "Tree Colouring";
            this.tpTreeColoring.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 328);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(40, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "&Status:";
            // 
            // cboShowStatus
            // 
            this.cboShowStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboShowStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboShowStatus.FormattingEnabled = true;
            this.cboShowStatus.Location = new System.Drawing.Point(51, 325);
            this.cboShowStatus.Name = "cboShowStatus";
            this.cboShowStatus.Size = new System.Drawing.Size(452, 21);
            this.cboShowStatus.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 360);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "&Text Color:";
            // 
            // txtShowStatusColor
            // 
            this.txtShowStatusColor.Location = new System.Drawing.Point(67, 357);
            this.txtShowStatusColor.Name = "txtShowStatusColor";
            this.txtShowStatusColor.Size = new System.Drawing.Size(100, 20);
            this.txtShowStatusColor.TabIndex = 5;
            this.txtShowStatusColor.TextChanged += new System.EventHandler(this.txtShowStatusColor_TextChanged);
            // 
            // btnSelectColor
            // 
            this.btnSelectColor.Location = new System.Drawing.Point(173, 355);
            this.btnSelectColor.Name = "btnSelectColor";
            this.btnSelectColor.Size = new System.Drawing.Size(75, 23);
            this.btnSelectColor.TabIndex = 4;
            this.btnSelectColor.Text = "Select &Color";
            this.btnSelectColor.UseVisualStyleBackColor = true;
            this.btnSelectColor.Click += new System.EventHandler(this.btnSelectColor_Click);
            // 
            // bnRemoveDefinedColor
            // 
            this.bnRemoveDefinedColor.Enabled = false;
            this.bnRemoveDefinedColor.Location = new System.Drawing.Point(6, 296);
            this.bnRemoveDefinedColor.Name = "bnRemoveDefinedColor";
            this.bnRemoveDefinedColor.Size = new System.Drawing.Size(75, 23);
            this.bnRemoveDefinedColor.TabIndex = 3;
            this.bnRemoveDefinedColor.Text = "&Remove";
            this.bnRemoveDefinedColor.UseVisualStyleBackColor = true;
            this.bnRemoveDefinedColor.Click += new System.EventHandler(this.bnRemoveDefinedColor_Click);
            // 
            // btnAddShowStatusColoring
            // 
            this.btnAddShowStatusColoring.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddShowStatusColoring.Location = new System.Drawing.Point(428, 352);
            this.btnAddShowStatusColoring.Name = "btnAddShowStatusColoring";
            this.btnAddShowStatusColoring.Size = new System.Drawing.Size(75, 23);
            this.btnAddShowStatusColoring.TabIndex = 3;
            this.btnAddShowStatusColoring.Text = "&Add";
            this.btnAddShowStatusColoring.UseVisualStyleBackColor = true;
            this.btnAddShowStatusColoring.Click += new System.EventHandler(this.btnAddShowStatusColoring_Click);
            // 
            // lvwDefinedColors
            // 
            this.lvwDefinedColors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwDefinedColors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colShowStatus,
            this.colColor});
            this.lvwDefinedColors.GridLines = true;
            this.lvwDefinedColors.Location = new System.Drawing.Point(6, 6);
            this.lvwDefinedColors.MultiSelect = false;
            this.lvwDefinedColors.Name = "lvwDefinedColors";
            this.lvwDefinedColors.Size = new System.Drawing.Size(497, 284);
            this.lvwDefinedColors.TabIndex = 0;
            this.lvwDefinedColors.UseCompatibleStateImageBehavior = false;
            this.lvwDefinedColors.View = System.Windows.Forms.View.Details;
            this.lvwDefinedColors.SelectedIndexChanged += new System.EventHandler(this.lvwDefinedColors_SelectedIndexChanged);
            this.lvwDefinedColors.DoubleClick += new System.EventHandler(this.lvwDefinedColors_DoubleClick);
            // 
            // colShowStatus
            // 
            this.colShowStatus.Text = "Show Status";
            this.colShowStatus.Width = 297;
            // 
            // colColor
            // 
            this.colColor.Text = "Color";
            this.colColor.Width = 92;
            // 
            // tpBulkAdd
            // 
            this.tpBulkAdd.Controls.Add(this.groupBox9);
            this.tpBulkAdd.Controls.Add(this.groupBox8);
            this.tpBulkAdd.Location = new System.Drawing.Point(4, 40);
            this.tpBulkAdd.Name = "tpBulkAdd";
            this.tpBulkAdd.Padding = new System.Windows.Forms.Padding(3);
            this.tpBulkAdd.Size = new System.Drawing.Size(509, 489);
            this.tpBulkAdd.TabIndex = 10;
            this.tpBulkAdd.Text = "Bulk/Auto Add";
            this.tpBulkAdd.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox9.Controls.Add(this.chkForceBulkAddToUseSettingsOnly);
            this.groupBox9.Controls.Add(this.cbIgnoreRecycleBin);
            this.groupBox9.Controls.Add(this.cbIgnoreNoVideoFolders);
            this.groupBox9.Location = new System.Drawing.Point(6, 10);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(497, 111);
            this.groupBox9.TabIndex = 16;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Bulk Add";
            // 
            // chkForceBulkAddToUseSettingsOnly
            // 
            this.chkForceBulkAddToUseSettingsOnly.AutoSize = true;
            this.chkForceBulkAddToUseSettingsOnly.Location = new System.Drawing.Point(6, 65);
            this.chkForceBulkAddToUseSettingsOnly.Name = "chkForceBulkAddToUseSettingsOnly";
            this.chkForceBulkAddToUseSettingsOnly.Size = new System.Drawing.Size(248, 17);
            this.chkForceBulkAddToUseSettingsOnly.TabIndex = 15;
            this.chkForceBulkAddToUseSettingsOnly.Text = "Force to Use Season Words from Settings Only";
            this.toolTip1.SetToolTip(this.chkForceBulkAddToUseSettingsOnly, "If set then Bulk Add just uses the season words from settings. If not set (recomm" +
        "ended) then Bulk Add finds addition season words from each show\'s configuration." +
        "");
            this.chkForceBulkAddToUseSettingsOnly.UseVisualStyleBackColor = true;
            // 
            // cbIgnoreRecycleBin
            // 
            this.cbIgnoreRecycleBin.AutoSize = true;
            this.cbIgnoreRecycleBin.Location = new System.Drawing.Point(6, 42);
            this.cbIgnoreRecycleBin.Name = "cbIgnoreRecycleBin";
            this.cbIgnoreRecycleBin.Size = new System.Drawing.Size(116, 17);
            this.cbIgnoreRecycleBin.TabIndex = 14;
            this.cbIgnoreRecycleBin.Text = "Ignore &Recycle Bin";
            this.toolTip1.SetToolTip(this.cbIgnoreRecycleBin, "If set then Bulk Add ignores all files in the Recycle Bin");
            this.cbIgnoreRecycleBin.UseVisualStyleBackColor = true;
            // 
            // cbIgnoreNoVideoFolders
            // 
            this.cbIgnoreNoVideoFolders.AutoSize = true;
            this.cbIgnoreNoVideoFolders.Location = new System.Drawing.Point(6, 19);
            this.cbIgnoreNoVideoFolders.Name = "cbIgnoreNoVideoFolders";
            this.cbIgnoreNoVideoFolders.Size = new System.Drawing.Size(225, 17);
            this.cbIgnoreNoVideoFolders.TabIndex = 13;
            this.cbIgnoreNoVideoFolders.Text = "&Only Include Folders containing Video files";
            this.toolTip1.SetToolTip(this.cbIgnoreNoVideoFolders, "If set then only folders that contain video files are considered for the \'Bulk Ad" +
        "d\' feature");
            this.cbIgnoreNoVideoFolders.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox8.Controls.Add(this.chkAutoSearchForDownloadedFiles);
            this.groupBox8.Controls.Add(this.label43);
            this.groupBox8.Controls.Add(this.label44);
            this.groupBox8.Controls.Add(this.tbIgnoreSuffixes);
            this.groupBox8.Controls.Add(this.tbMovieTerms);
            this.groupBox8.Location = new System.Drawing.Point(6, 127);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(497, 107);
            this.groupBox8.TabIndex = 13;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Auto Add";
            // 
            // chkAutoSearchForDownloadedFiles
            // 
            this.chkAutoSearchForDownloadedFiles.AutoSize = true;
            this.chkAutoSearchForDownloadedFiles.Location = new System.Drawing.Point(6, 19);
            this.chkAutoSearchForDownloadedFiles.Name = "chkAutoSearchForDownloadedFiles";
            this.chkAutoSearchForDownloadedFiles.Size = new System.Drawing.Size(186, 17);
            this.chkAutoSearchForDownloadedFiles.TabIndex = 16;
            this.chkAutoSearchForDownloadedFiles.Text = "Notify when new shows are found";
            this.chkAutoSearchForDownloadedFiles.UseVisualStyleBackColor = true;
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(3, 69);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(78, 13);
            this.label43.TabIndex = 14;
            this.label43.Text = "&Ignore suffixes:";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(3, 43);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(71, 13);
            this.label44.TabIndex = 12;
            this.label44.Text = "&Movie Terms:";
            // 
            // tbIgnoreSuffixes
            // 
            this.tbIgnoreSuffixes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbIgnoreSuffixes.Location = new System.Drawing.Point(99, 66);
            this.tbIgnoreSuffixes.Name = "tbIgnoreSuffixes";
            this.tbIgnoreSuffixes.Size = new System.Drawing.Size(395, 20);
            this.tbIgnoreSuffixes.TabIndex = 15;
            this.toolTip1.SetToolTip(this.tbIgnoreSuffixes, "These terms and any text after them will be ignored when\r\nsearching on TVDB for t" +
        "he show title based on the filename.");
            // 
            // tbMovieTerms
            // 
            this.tbMovieTerms.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMovieTerms.Location = new System.Drawing.Point(99, 40);
            this.tbMovieTerms.Name = "tbMovieTerms";
            this.tbMovieTerms.Size = new System.Drawing.Size(395, 20);
            this.tbMovieTerms.TabIndex = 13;
            this.toolTip1.SetToolTip(this.tbMovieTerms, "If a filename contains any of these terms then it is assumed\r\nthat it is a Film a" +
        "nd not a TV Show. Hence \'Auto Add\' is not\r\ninvoked for this file.");
            // 
            // tpSubtitles
            // 
            this.tpSubtitles.Controls.Add(this.cbTxtToSub);
            this.tpSubtitles.Controls.Add(this.label46);
            this.tpSubtitles.Controls.Add(this.txtSubtitleExtensions);
            this.tpSubtitles.Controls.Add(this.chkRetainLanguageSpecificSubtitles);
            this.tpSubtitles.Location = new System.Drawing.Point(4, 40);
            this.tpSubtitles.Name = "tpSubtitles";
            this.tpSubtitles.Padding = new System.Windows.Forms.Padding(3);
            this.tpSubtitles.Size = new System.Drawing.Size(509, 489);
            this.tpSubtitles.TabIndex = 11;
            this.tpSubtitles.Text = "Subtitles";
            this.tpSubtitles.UseVisualStyleBackColor = true;
            // 
            // cbTxtToSub
            // 
            this.cbTxtToSub.AutoSize = true;
            this.cbTxtToSub.Location = new System.Drawing.Point(6, 38);
            this.cbTxtToSub.Name = "cbTxtToSub";
            this.cbTxtToSub.Size = new System.Drawing.Size(118, 17);
            this.cbTxtToSub.TabIndex = 28;
            this.cbTxtToSub.Text = "&Rename .txt to .sub";
            this.cbTxtToSub.UseVisualStyleBackColor = true;
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(3, 66);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(98, 13);
            this.label46.TabIndex = 26;
            this.label46.Text = "&Subtitle extensions:";
            // 
            // txtSubtitleExtensions
            // 
            this.txtSubtitleExtensions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubtitleExtensions.Location = new System.Drawing.Point(107, 63);
            this.txtSubtitleExtensions.Name = "txtSubtitleExtensions";
            this.txtSubtitleExtensions.Size = new System.Drawing.Size(396, 20);
            this.txtSubtitleExtensions.TabIndex = 27;
            // 
            // chkRetainLanguageSpecificSubtitles
            // 
            this.chkRetainLanguageSpecificSubtitles.AutoSize = true;
            this.chkRetainLanguageSpecificSubtitles.Location = new System.Drawing.Point(6, 15);
            this.chkRetainLanguageSpecificSubtitles.Name = "chkRetainLanguageSpecificSubtitles";
            this.chkRetainLanguageSpecificSubtitles.Size = new System.Drawing.Size(192, 17);
            this.chkRetainLanguageSpecificSubtitles.TabIndex = 25;
            this.chkRetainLanguageSpecificSubtitles.Text = "Retain &Language Specific Subtitles";
            this.chkRetainLanguageSpecificSubtitles.UseVisualStyleBackColor = true;
            // 
            // folderBrowser
            // 
            this.folderBrowser.ShowNewFolderButton = false;
            // 
            // openFile
            // 
            this.openFile.Filter = "Torrent files (*.torrent)|*.torrent|All files (*.*)|*.*";
            // 
            // cmDefaults
            // 
            this.cmDefaults.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.KODIToolStripMenuItem,
            this.pyTivoToolStripMenuItem,
            this.mede8erToolStripMenuItem,
            this.noneToolStripMenuItem});
            this.cmDefaults.Name = "cmDefaults";
            this.cmDefaults.Size = new System.Drawing.Size(121, 92);
            this.cmDefaults.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmDefaults_ItemClicked);
            // 
            // KODIToolStripMenuItem
            // 
            this.KODIToolStripMenuItem.Name = "KODIToolStripMenuItem";
            this.KODIToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.KODIToolStripMenuItem.Tag = "1";
            this.KODIToolStripMenuItem.Text = "&KODI";
            // 
            // pyTivoToolStripMenuItem
            // 
            this.pyTivoToolStripMenuItem.Name = "pyTivoToolStripMenuItem";
            this.pyTivoToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.pyTivoToolStripMenuItem.Tag = "2";
            this.pyTivoToolStripMenuItem.Text = "&pyTivo";
            // 
            // mede8erToolStripMenuItem
            // 
            this.mede8erToolStripMenuItem.Name = "mede8erToolStripMenuItem";
            this.mede8erToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.mede8erToolStripMenuItem.Tag = "3";
            this.mede8erToolStripMenuItem.Text = "&Mede8er";
            // 
            // noneToolStripMenuItem
            // 
            this.noneToolStripMenuItem.Name = "noneToolStripMenuItem";
            this.noneToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.noneToolStripMenuItem.Tag = "4";
            this.noneToolStripMenuItem.Text = "&None";
            // 
            // Preferences
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(541, 576);
            this.ControlBox = false;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.OKButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(520, 550);
            this.Name = "Preferences";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Preferences";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Preferences_FormClosing);
            this.Load += new System.EventHandler(this.Preferences_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tbGeneral.ResumeLayout(false);
            this.tbGeneral.PerformLayout();
            this.tbFilesAndFolders.ResumeLayout(false);
            this.tbFilesAndFolders.PerformLayout();
            this.tbAutoExport.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tpScanOptions.ResumeLayout(false);
            this.tpScanOptions.PerformLayout();
            this.tbFolderDeleting.ResumeLayout(false);
            this.tbFolderDeleting.PerformLayout();
            this.tbMediaCenter.ResumeLayout(false);
            this.tbMediaCenter.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tbSearchFolders.ResumeLayout(false);
            this.tbSearchFolders.PerformLayout();
            this.tbuTorrentNZB.ResumeLayout(false);
            this.tbuTorrentNZB.PerformLayout();
            this.qBitTorrent.ResumeLayout(false);
            this.qBitTorrent.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tpTreeColoring.ResumeLayout(false);
            this.tpTreeColoring.PerformLayout();
            this.tpBulkAdd.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.tpSubtitles.ResumeLayout(false);
            this.tpSubtitles.PerformLayout();
            this.cmDefaults.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #endregion

        private System.Windows.Forms.TabPage tpScanOptions;
        private System.Windows.Forms.CheckBox cbSearchRSS;
        private System.Windows.Forms.CheckBox cbRenameCheck;
        private System.Windows.Forms.CheckBox cbMissing;
        private System.Windows.Forms.CheckBox cbLeaveOriginals;
        private System.Windows.Forms.CheckBox cbSearchLocally;
        private System.Windows.Forms.Label label28;
        private SourceGrid.Grid ReplacementsGrid;
        private System.Windows.Forms.Button bnReplaceRemove;
        private System.Windows.Forms.Button bnReplaceAdd;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbCheckuTorrent;
        private System.Windows.Forms.CheckBox cbAutoSelInMyShows;
        private System.Windows.Forms.RadioButton rbFolderSeasonPoster;

        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button bnCancel;

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button bnBrowseWTWRSS;
        private System.Windows.Forms.TextBox txtWTWRSS;

        private System.Windows.Forms.CheckBox cbWTWRSS;
        private System.Windows.Forms.SaveFileDialog saveFile;

        private System.Windows.Forms.TextBox txtWTWDays;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbStartupTab;
        private System.Windows.Forms.Label label6;

        private System.Windows.Forms.CheckBox cbNotificationIcon;
        private System.Windows.Forms.TextBox txtVideoExtensions;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtExportRSSMaxDays;

        private System.Windows.Forms.TextBox txtExportRSSMaxShows;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.CheckBox cbKeepTogether;
        private System.Windows.Forms.CheckBox cbLeadingZero;
        private System.Windows.Forms.CheckBox chkShowInTaskbar;
        private System.Windows.Forms.CheckBox cbShowEpisodePictures;
        private System.Windows.Forms.TextBox txtSpecialsFolderName;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tbGeneral;
        private System.Windows.Forms.TabPage tbFilesAndFolders;
        private System.Windows.Forms.TabPage tbAutoExport;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button bnBrowseMissingCSV;
        private System.Windows.Forms.CheckBox cbMissingCSV;
        private System.Windows.Forms.TextBox txtMissingCSV;
        private System.Windows.Forms.Button bnBrowseMissingXML;
        private System.Windows.Forms.CheckBox cbMissingXML;
        private System.Windows.Forms.TextBox txtMissingXML;
        private System.Windows.Forms.Button bnBrowseFOXML;

        private System.Windows.Forms.CheckBox cbFOXML;
        private System.Windows.Forms.TextBox txtFOXML;
            
        private System.Windows.Forms.Button bnBrowseRenamingXML;
        private System.Windows.Forms.CheckBox cbRenamingXML;
        private System.Windows.Forms.TextBox txtRenamingXML;
        private System.Windows.Forms.CheckBox cbIgnoreSamples;

        private System.Windows.Forms.CheckBox cbForceLower;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtMaxSampleSize;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtParallelDownloads;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox txtOtherExtensions;
        private System.Windows.Forms.TabPage tbSearchFolders;
        private System.Windows.Forms.Button bnOpenSearchFolder;
        private System.Windows.Forms.Button bnRemoveSearchFolder;
        private System.Windows.Forms.Button bnAddSearchFolder;
        private System.Windows.Forms.ListBox lbSearchFolders;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
        private System.Windows.Forms.TabPage tbuTorrentNZB;
        private System.Windows.Forms.Button bnRSSBrowseuTorrent;
        private System.Windows.Forms.Button bnRSSGo;
        private System.Windows.Forms.TextBox txtRSSuTorrentPath;
              
        private System.Windows.Forms.Label label25;
        private SourceGrid.Grid RSSGrid;

        private System.Windows.Forms.Button bnRSSRemove;
        private System.Windows.Forms.Button bnRSSAdd;
        private System.Windows.Forms.OpenFileDialog openFile;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button bnUTBrowseResumeDat;
        private System.Windows.Forms.TextBox txtUTResumeDatPath;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.CheckBox cbLookForAirdate;
        private System.Windows.Forms.CheckBox cbMonitorFolder;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabPage tpTreeColoring;
        private System.Windows.Forms.ListView lvwDefinedColors;
        private System.Windows.Forms.ColumnHeader colShowStatus;
        private System.Windows.Forms.ColumnHeader colColor;
        private System.Windows.Forms.TextBox txtShowStatusColor;
        private System.Windows.Forms.Button btnSelectColor;
        private System.Windows.Forms.Button btnAddShowStatusColoring;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboShowStatus;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button bnRemoveDefinedColor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtExportRSSDaysPast;
        private System.Windows.Forms.Button bnBrowseWTWXML;
        private System.Windows.Forms.TextBox txtWTWXML;
        private System.Windows.Forms.CheckBox cbWTWXML;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtSABHostPort;
        private System.Windows.Forms.TextBox txtSABAPIKey;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox cbCheckSABnzbd;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cbLanguages;
        private System.Windows.Forms.RadioButton rbWTWScan;
        private System.Windows.Forms.RadioButton rbWTWSearch;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox cbKODIImages;
        private System.Windows.Forms.Label lblScanAction;
        private System.Windows.Forms.RadioButton rdoQuickScan;
        private System.Windows.Forms.RadioButton rdoRecentScan;
        private System.Windows.Forms.RadioButton rdoFullScan;
        private System.Windows.Forms.TabPage tbMediaCenter;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox cbMetaSubfolder;
        private System.Windows.Forms.CheckBox cbMeta;
        private System.Windows.Forms.RadioButton rbFolderFanArt;
        private System.Windows.Forms.RadioButton rbFolderPoster;
        private System.Windows.Forms.RadioButton rbFolderBanner;
        private System.Windows.Forms.CheckBox cbEpTBNs;
        private System.Windows.Forms.CheckBox cbNFOShows;
        private System.Windows.Forms.CheckBox cbFolderJpg;
        private System.Windows.Forms.CheckBox cbEpThumbJpg;
        private System.Windows.Forms.CheckBox cbShrinkLarge;
        private System.Windows.Forms.CheckBox cbSeriesJpg;
        private System.Windows.Forms.CheckBox cbXMLFiles;
        private System.Windows.Forms.Button bnMCPresets;
        private System.Windows.Forms.ContextMenuStrip cmDefaults;
        private System.Windows.Forms.ToolStripMenuItem KODIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pyTivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mede8erToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem;
        private System.Windows.Forms.CheckBox cbFantArtJpg;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox cbNFOEpisodes;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button bnBrowseShowsTXT;
        private System.Windows.Forms.CheckBox cbShowsTXT;
        private System.Windows.Forms.TextBox txtShowsTXTTo;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.CheckBox cbAutoCreateFolders;
        private System.Windows.Forms.ComboBox cbMode;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.TextBox txtSeasonFolderName;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TextBox tbSeasonSearchTerms;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.TextBox tbPercentDirty;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.ComboBox cbKeepTogetherMode;
        private System.Windows.Forms.TextBox txtKeepTogether;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DomainUpDown domainUpDown1;
        private System.Windows.Forms.CheckBox chkScheduledScan;
        private System.Windows.Forms.CheckBox chkScanOnStartup;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.CheckBox cbxUpdateAirDate;
        private System.Windows.Forms.CheckBox chkHideWtWSpoilers;
        private System.Windows.Forms.CheckBox chkHideMyShowsSpoilers;
        private System.Windows.Forms.CheckBox chkPreventMove;
        private System.Windows.Forms.CheckBox chkAutoMergeDownloadEpisodes;
        private System.Windows.Forms.TabPage tpBulkAdd;
        private System.Windows.Forms.Button bnBrowseShowsHTML;
        private System.Windows.Forms.CheckBox cbShowsHTML;
        private System.Windows.Forms.TextBox txtShowsHTMLTo;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.TextBox tbPreferredRSSTerms;
        private System.Windows.Forms.CheckBox chkAutoMergeLibraryEpisodes;
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
        private System.Windows.Forms.TabPage tpSubtitles;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.TextBox txtSubtitleExtensions;
        private System.Windows.Forms.CheckBox chkRetainLanguageSpecificSubtitles;
        private System.Windows.Forms.CheckBox cbTxtToSub;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.CheckBox chkForceBulkAddToUseSettingsOnly;
        private System.Windows.Forms.CheckBox cbIgnoreRecycleBin;
        private System.Windows.Forms.CheckBox cbIgnoreNoVideoFolders;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.CheckBox chkAutoSearchForDownloadedFiles;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.TextBox tbIgnoreSuffixes;
        private System.Windows.Forms.TextBox tbMovieTerms;
        private System.Windows.Forms.Button bnBrowseWTWICAL;
        private System.Windows.Forms.TextBox txtWTWICAL;
        private System.Windows.Forms.CheckBox cbWTWICAL;
        private System.Windows.Forms.GroupBox qBitTorrent;
        private System.Windows.Forms.TextBox tbqBitTorrentHost;
        private System.Windows.Forms.TextBox tbqBitTorrentPort;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.CheckBox cbCheckqBitTorrent;
        private System.Windows.Forms.Button bnTags;
        private System.Windows.Forms.TextBox txtSeasonFormat;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.CheckBox cbWDLiveEpisodeFiles;
    }
}
