//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//


namespace TVRename
{
    partial class FolderMonitor
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
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FolderMonitor));
            this.bnCheck1 = new System.Windows.Forms.Button();
            this.bnOpenMonFolder = new System.Windows.Forms.Button();
            this.bnRemoveMonFolder = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lstFMMonitorFolders = new System.Windows.Forms.ListBox();
            this.bnAddMonFolder = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.bnOpenIgFolder = new System.Windows.Forms.Button();
            this.bnAddIgFolder = new System.Windows.Forms.Button();
            this.bnRemoveIgFolder = new System.Windows.Forms.Button();
            this.lstFMIgnoreFolders = new System.Windows.Forms.ListBox();
            this.bnVisitTVcom = new System.Windows.Forms.Button();
            this.bnFullAuto = new System.Windows.Forms.Button();
            this.bnFolderMonitorDone = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.bnRemoveNewFolder = new System.Windows.Forms.Button();
            this.bnNewFolderOpen = new System.Windows.Forms.Button();
            this.bnIgnoreNewFolder = new System.Windows.Forms.Button();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tbFolders = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.tbIgnore = new System.Windows.Forms.TabPage();
            this.bnCheck2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbResults = new System.Windows.Forms.TabPage();
            this.bnEditEntry = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lvFMNewShows = new TVRename.ListViewFlickerFree();
            this.columnHeader42 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader43 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader44 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader45 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.bnClose = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tbFolders.SuspendLayout();
            this.tbIgnore.SuspendLayout();
            this.tbResults.SuspendLayout();
            this.SuspendLayout();
            // 
            // bnCheck1
            // 
            this.bnCheck1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCheck1.Location = new System.Drawing.Point(770, 384);
            this.bnCheck1.Name = "bnCheck1";
            this.bnCheck1.Size = new System.Drawing.Size(75, 23);
            this.bnCheck1.TabIndex = 10;
            this.bnCheck1.Text = "&Check >>";
            this.bnCheck1.UseVisualStyleBackColor = true;
            this.bnCheck1.Click += new System.EventHandler(this.bnCheck_Click);
            // 
            // bnOpenMonFolder
            // 
            this.bnOpenMonFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnOpenMonFolder.Enabled = false;
            this.bnOpenMonFolder.Location = new System.Drawing.Point(168, 384);
            this.bnOpenMonFolder.Name = "bnOpenMonFolder";
            this.bnOpenMonFolder.Size = new System.Drawing.Size(75, 23);
            this.bnOpenMonFolder.TabIndex = 9;
            this.bnOpenMonFolder.Text = "&Open";
            this.bnOpenMonFolder.UseVisualStyleBackColor = true;
            this.bnOpenMonFolder.Click += new System.EventHandler(this.bnOpenMonFolder_Click);
            // 
            // bnRemoveMonFolder
            // 
            this.bnRemoveMonFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnRemoveMonFolder.Enabled = false;
            this.bnRemoveMonFolder.Location = new System.Drawing.Point(87, 384);
            this.bnRemoveMonFolder.Name = "bnRemoveMonFolder";
            this.bnRemoveMonFolder.Size = new System.Drawing.Size(75, 23);
            this.bnRemoveMonFolder.TabIndex = 8;
            this.bnRemoveMonFolder.Text = "&Remove";
            this.bnRemoveMonFolder.UseVisualStyleBackColor = true;
            this.bnRemoveMonFolder.Click += new System.EventHandler(this.bnRemoveMonFolder_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "&Monitor Folders:";
            // 
            // lstFMMonitorFolders
            // 
            this.lstFMMonitorFolders.AllowDrop = true;
            this.lstFMMonitorFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstFMMonitorFolders.FormattingEnabled = true;
            this.lstFMMonitorFolders.IntegralHeight = false;
            this.lstFMMonitorFolders.Location = new System.Drawing.Point(6, 55);
            this.lstFMMonitorFolders.Name = "lstFMMonitorFolders";
            this.lstFMMonitorFolders.ScrollAlwaysVisible = true;
            this.lstFMMonitorFolders.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstFMMonitorFolders.Size = new System.Drawing.Size(839, 323);
            this.lstFMMonitorFolders.TabIndex = 6;
            this.lstFMMonitorFolders.SelectedIndexChanged += new System.EventHandler(this.lstFMMonitorFolders_SelectedIndexChanged);
            this.lstFMMonitorFolders.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstFMMonitorFolders_DragDrop);
            this.lstFMMonitorFolders.DragOver += new System.Windows.Forms.DragEventHandler(lstFMMonitorFolders_DragOver);
            this.lstFMMonitorFolders.DoubleClick += new System.EventHandler(this.lstFMMonitorFolders_DoubleClick);
            this.lstFMMonitorFolders.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstFMMonitorFolders_KeyDown);
            // 
            // bnAddMonFolder
            // 
            this.bnAddMonFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnAddMonFolder.Location = new System.Drawing.Point(6, 384);
            this.bnAddMonFolder.Name = "bnAddMonFolder";
            this.bnAddMonFolder.Size = new System.Drawing.Size(75, 23);
            this.bnAddMonFolder.TabIndex = 7;
            this.bnAddMonFolder.Text = "&Add";
            this.bnAddMonFolder.UseVisualStyleBackColor = true;
            this.bnAddMonFolder.Click += new System.EventHandler(this.bnAddMonFolder_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 39);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "&Ignore Folders:";
            // 
            // bnOpenIgFolder
            // 
            this.bnOpenIgFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnOpenIgFolder.Enabled = false;
            this.bnOpenIgFolder.Location = new System.Drawing.Point(169, 384);
            this.bnOpenIgFolder.Name = "bnOpenIgFolder";
            this.bnOpenIgFolder.Size = new System.Drawing.Size(75, 23);
            this.bnOpenIgFolder.TabIndex = 9;
            this.bnOpenIgFolder.Text = "O&pen";
            this.bnOpenIgFolder.UseVisualStyleBackColor = true;
            this.bnOpenIgFolder.Click += new System.EventHandler(this.bnOpenIgFolder_Click);
            // 
            // bnAddIgFolder
            // 
            this.bnAddIgFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnAddIgFolder.Location = new System.Drawing.Point(6, 384);
            this.bnAddIgFolder.Name = "bnAddIgFolder";
            this.bnAddIgFolder.Size = new System.Drawing.Size(75, 23);
            this.bnAddIgFolder.TabIndex = 7;
            this.bnAddIgFolder.Text = "A&dd";
            this.bnAddIgFolder.UseVisualStyleBackColor = true;
            this.bnAddIgFolder.Click += new System.EventHandler(this.bnAddIgFolder_Click);
            // 
            // bnRemoveIgFolder
            // 
            this.bnRemoveIgFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnRemoveIgFolder.Enabled = false;
            this.bnRemoveIgFolder.Location = new System.Drawing.Point(88, 384);
            this.bnRemoveIgFolder.Name = "bnRemoveIgFolder";
            this.bnRemoveIgFolder.Size = new System.Drawing.Size(75, 23);
            this.bnRemoveIgFolder.TabIndex = 8;
            this.bnRemoveIgFolder.Text = "Remo&ve";
            this.bnRemoveIgFolder.UseVisualStyleBackColor = true;
            this.bnRemoveIgFolder.Click += new System.EventHandler(this.bnRemoveIgFolder_Click);
            // 
            // lstFMIgnoreFolders
            // 
            this.lstFMIgnoreFolders.AllowDrop = true;
            this.lstFMIgnoreFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstFMIgnoreFolders.FormattingEnabled = true;
            this.lstFMIgnoreFolders.IntegralHeight = false;
            this.lstFMIgnoreFolders.Location = new System.Drawing.Point(6, 55);
            this.lstFMIgnoreFolders.Name = "lstFMIgnoreFolders";
            this.lstFMIgnoreFolders.ScrollAlwaysVisible = true;
            this.lstFMIgnoreFolders.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstFMIgnoreFolders.Size = new System.Drawing.Size(839, 323);
            this.lstFMIgnoreFolders.TabIndex = 6;
            this.lstFMIgnoreFolders.SelectedIndexChanged += new System.EventHandler(this.lstFMIgnoreFolders_SelectedIndexChanged);
            this.lstFMIgnoreFolders.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstFMIgnoreFolders_DragDrop);
            this.lstFMIgnoreFolders.DragOver += new System.Windows.Forms.DragEventHandler(lstFMIgnoreFolders_DragOver);
            this.lstFMIgnoreFolders.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstFMIgnoreFolders_KeyDown);
            // 
            // bnVisitTVcom
            // 
            this.bnVisitTVcom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnVisitTVcom.Enabled = false;
            this.bnVisitTVcom.Location = new System.Drawing.Point(346, 384);
            this.bnVisitTVcom.Name = "bnVisitTVcom";
            this.bnVisitTVcom.Size = new System.Drawing.Size(75, 23);
            this.bnVisitTVcom.TabIndex = 26;
            this.bnVisitTVcom.Text = "&Visit TVDB";
            this.bnVisitTVcom.UseVisualStyleBackColor = true;
            this.bnVisitTVcom.Click += new System.EventHandler(this.bnVisitTVcom_Click);
            // 
            // bnFullAuto
            // 
            this.bnFullAuto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnFullAuto.Location = new System.Drawing.Point(6, 384);
            this.bnFullAuto.Name = "bnFullAuto";
            this.bnFullAuto.Size = new System.Drawing.Size(75, 23);
            this.bnFullAuto.TabIndex = 24;
            this.bnFullAuto.Text = "&Auto ID All";
            this.bnFullAuto.UseVisualStyleBackColor = true;
            this.bnFullAuto.Click += new System.EventHandler(this.bnFullAuto_Click);
            // 
            // bnFolderMonitorDone
            // 
            this.bnFolderMonitorDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnFolderMonitorDone.Location = new System.Drawing.Point(770, 384);
            this.bnFolderMonitorDone.Name = "bnFolderMonitorDone";
            this.bnFolderMonitorDone.Size = new System.Drawing.Size(75, 23);
            this.bnFolderMonitorDone.TabIndex = 10;
            this.bnFolderMonitorDone.Text = "A&dd && Close";
            this.bnFolderMonitorDone.UseVisualStyleBackColor = true;
            this.bnFolderMonitorDone.Click += new System.EventHandler(this.bnFolderMonitorDone_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 39);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "&New Shows";
            // 
            // bnRemoveNewFolder
            // 
            this.bnRemoveNewFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnRemoveNewFolder.Enabled = false;
            this.bnRemoveNewFolder.Location = new System.Drawing.Point(175, 384);
            this.bnRemoveNewFolder.Name = "bnRemoveNewFolder";
            this.bnRemoveNewFolder.Size = new System.Drawing.Size(75, 23);
            this.bnRemoveNewFolder.TabIndex = 9;
            this.bnRemoveNewFolder.Text = "Re&move";
            this.bnRemoveNewFolder.UseVisualStyleBackColor = true;
            this.bnRemoveNewFolder.Click += new System.EventHandler(this.bnRemoveNewFolder_Click);
            // 
            // bnNewFolderOpen
            // 
            this.bnNewFolderOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnNewFolderOpen.Enabled = false;
            this.bnNewFolderOpen.Location = new System.Drawing.Point(427, 384);
            this.bnNewFolderOpen.Name = "bnNewFolderOpen";
            this.bnNewFolderOpen.Size = new System.Drawing.Size(75, 23);
            this.bnNewFolderOpen.TabIndex = 9;
            this.bnNewFolderOpen.Text = "Open &Folder";
            this.bnNewFolderOpen.UseVisualStyleBackColor = true;
            this.bnNewFolderOpen.Click += new System.EventHandler(this.bnNewFolderOpen_Click);
            // 
            // bnIgnoreNewFolder
            // 
            this.bnIgnoreNewFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnIgnoreNewFolder.Enabled = false;
            this.bnIgnoreNewFolder.Location = new System.Drawing.Point(256, 384);
            this.bnIgnoreNewFolder.Name = "bnIgnoreNewFolder";
            this.bnIgnoreNewFolder.Size = new System.Drawing.Size(75, 23);
            this.bnIgnoreNewFolder.TabIndex = 9;
            this.bnIgnoreNewFolder.Text = "&Ignore";
            this.bnIgnoreNewFolder.UseVisualStyleBackColor = true;
            this.bnIgnoreNewFolder.Click += new System.EventHandler(this.bnIgnoreNewFolder_Click);
            // 
            // folderBrowser
            // 
            this.folderBrowser.ShowNewFolderButton = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tbFolders);
            this.tabControl1.Controls.Add(this.tbIgnore);
            this.tabControl1.Controls.Add(this.tbResults);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(859, 436);
            this.tabControl1.TabIndex = 13;
            // 
            // tbFolders
            // 
            this.tbFolders.Controls.Add(this.bnCheck1);
            this.tbFolders.Controls.Add(this.label1);
            this.tbFolders.Controls.Add(this.label2);
            this.tbFolders.Controls.Add(this.bnOpenMonFolder);
            this.tbFolders.Controls.Add(this.bnAddMonFolder);
            this.tbFolders.Controls.Add(this.bnRemoveMonFolder);
            this.tbFolders.Controls.Add(this.lstFMMonitorFolders);
            this.tbFolders.Location = new System.Drawing.Point(4, 22);
            this.tbFolders.Name = "tbFolders";
            this.tbFolders.Padding = new System.Windows.Forms.Padding(3);
            this.tbFolders.Size = new System.Drawing.Size(851, 410);
            this.tbFolders.TabIndex = 0;
            this.tbFolders.Text = "Folders";
            this.tbFolders.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(562, 36);
            this.label1.TabIndex = 5;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // tbIgnore
            // 
            this.tbIgnore.Controls.Add(this.bnCheck2);
            this.tbIgnore.Controls.Add(this.label3);
            this.tbIgnore.Controls.Add(this.label7);
            this.tbIgnore.Controls.Add(this.lstFMIgnoreFolders);
            this.tbIgnore.Controls.Add(this.bnOpenIgFolder);
            this.tbIgnore.Controls.Add(this.bnRemoveIgFolder);
            this.tbIgnore.Controls.Add(this.bnAddIgFolder);
            this.tbIgnore.Location = new System.Drawing.Point(4, 22);
            this.tbIgnore.Name = "tbIgnore";
            this.tbIgnore.Padding = new System.Windows.Forms.Padding(3);
            this.tbIgnore.Size = new System.Drawing.Size(851, 410);
            this.tbIgnore.TabIndex = 1;
            this.tbIgnore.Text = "Ignore";
            this.tbIgnore.UseVisualStyleBackColor = true;
            // 
            // bnCheck2
            // 
            this.bnCheck2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCheck2.Location = new System.Drawing.Point(768, 384);
            this.bnCheck2.Name = "bnCheck2";
            this.bnCheck2.Size = new System.Drawing.Size(75, 23);
            this.bnCheck2.TabIndex = 11;
            this.bnCheck2.Text = "&Check >>";
            this.bnCheck2.UseVisualStyleBackColor = true;
            this.bnCheck2.Click += new System.EventHandler(this.bnCheck2_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(3, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(487, 36);
            this.label3.TabIndex = 10;
            this.label3.Text = "Add folders to this list, to have them ignored when Checking.  Drag and drop, or " +
    "click \"Add...\" below.  Click \"Check >>\" when done.";
            // 
            // tbResults
            // 
            this.tbResults.Controls.Add(this.bnEditEntry);
            this.tbResults.Controls.Add(this.label4);
            this.tbResults.Controls.Add(this.label6);
            this.tbResults.Controls.Add(this.lvFMNewShows);
            this.tbResults.Controls.Add(this.bnFullAuto);
            this.tbResults.Controls.Add(this.bnVisitTVcom);
            this.tbResults.Controls.Add(this.bnIgnoreNewFolder);
            this.tbResults.Controls.Add(this.bnNewFolderOpen);
            this.tbResults.Controls.Add(this.bnFolderMonitorDone);
            this.tbResults.Controls.Add(this.bnRemoveNewFolder);
            this.tbResults.Location = new System.Drawing.Point(4, 22);
            this.tbResults.Name = "tbResults";
            this.tbResults.Size = new System.Drawing.Size(851, 410);
            this.tbResults.TabIndex = 2;
            this.tbResults.Text = "Scan Results";
            this.tbResults.UseVisualStyleBackColor = true;
            // 
            // bnEditEntry
            // 
            this.bnEditEntry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnEditEntry.Enabled = false;
            this.bnEditEntry.Location = new System.Drawing.Point(93, 384);
            this.bnEditEntry.Name = "bnEditEntry";
            this.bnEditEntry.Size = new System.Drawing.Size(75, 23);
            this.bnEditEntry.TabIndex = 28;
            this.bnEditEntry.Text = "&Edit";
            this.bnEditEntry.UseVisualStyleBackColor = true;
            this.bnEditEntry.Click += new System.EventHandler(this.bnEditEntry_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(3, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(645, 32);
            this.label4.TabIndex = 12;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // lvFMNewShows
            // 
            this.lvFMNewShows.AllowDrop = true;
            this.lvFMNewShows.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFMNewShows.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader42,
            this.columnHeader43,
            this.columnHeader44,
            this.columnHeader45});
            this.lvFMNewShows.FullRowSelect = true;
            this.lvFMNewShows.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvFMNewShows.HideSelection = false;
            this.lvFMNewShows.Location = new System.Drawing.Point(6, 55);
            this.lvFMNewShows.Name = "lvFMNewShows";
            this.lvFMNewShows.Size = new System.Drawing.Size(839, 323);
            this.lvFMNewShows.TabIndex = 11;
            this.lvFMNewShows.UseCompatibleStateImageBehavior = false;
            this.lvFMNewShows.View = System.Windows.Forms.View.Details;
            this.lvFMNewShows.SelectedIndexChanged += new System.EventHandler(this.lvFMNewShows_SelectedIndexChanged);
            this.lvFMNewShows.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvFMNewShows_DragDrop);
            this.lvFMNewShows.DragOver += new System.Windows.Forms.DragEventHandler(this.lvFMNewShows_DragOver);
            this.lvFMNewShows.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvFMNewShows_KeyDown);
            this.lvFMNewShows.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvFMNewShows_MouseDoubleClick);
            // 
            // columnHeader42
            // 
            this.columnHeader42.Text = "Folder";
            this.columnHeader42.Width = 240;
            // 
            // columnHeader43
            // 
            this.columnHeader43.Text = "Show";
            this.columnHeader43.Width = 277;
            // 
            // columnHeader44
            // 
            this.columnHeader44.Text = "Folder Structure";
            this.columnHeader44.Width = 103;
            // 
            // columnHeader45
            // 
            this.columnHeader45.Text = "TheTVDB Code";
            this.columnHeader45.Width = 94;
            // 
            // bnClose
            // 
            this.bnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnClose.Location = new System.Drawing.Point(774, 442);
            this.bnClose.Name = "bnClose";
            this.bnClose.Size = new System.Drawing.Size(75, 23);
            this.bnClose.TabIndex = 27;
            this.bnClose.Text = "Close";
            this.bnClose.UseVisualStyleBackColor = true;
            this.bnClose.Click += new System.EventHandler(this.bnClose_Click);
            // 
            // FolderMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(859, 477);
            this.Controls.Add(this.bnClose);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(750, 300);
            this.Name = "FolderMonitor";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Bulk Add Shows";
            this.tabControl1.ResumeLayout(false);
            this.tbFolders.ResumeLayout(false);
            this.tbFolders.PerformLayout();
            this.tbIgnore.ResumeLayout(false);
            this.tbIgnore.PerformLayout();
            this.tbResults.ResumeLayout(false);
            this.tbResults.PerformLayout();
            this.ResumeLayout(false);

        }


        #endregion

        private System.Windows.Forms.Button bnCheck1;
        private System.Windows.Forms.Button bnOpenMonFolder;
        private System.Windows.Forms.Button bnRemoveMonFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lstFMMonitorFolders;
        private System.Windows.Forms.Button bnAddMonFolder;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button bnOpenIgFolder;
        private System.Windows.Forms.Button bnAddIgFolder;
        private System.Windows.Forms.Button bnRemoveIgFolder;
        private System.Windows.Forms.ListBox lstFMIgnoreFolders;
        private System.Windows.Forms.Button bnVisitTVcom;
        private System.Windows.Forms.Button bnFullAuto;
        private ListViewFlickerFree lvFMNewShows;
        private System.Windows.Forms.ColumnHeader columnHeader42;
        private System.Windows.Forms.ColumnHeader columnHeader43;
        private System.Windows.Forms.ColumnHeader columnHeader44;
        private System.Windows.Forms.ColumnHeader columnHeader45;
        private System.Windows.Forms.Button bnFolderMonitorDone;

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button bnRemoveNewFolder;
        private System.Windows.Forms.Button bnNewFolderOpen;
        private System.Windows.Forms.Button bnIgnoreNewFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tbFolders;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tbIgnore;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tbResults;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bnClose;
        private System.Windows.Forms.Button bnEditEntry;
        private System.Windows.Forms.Button bnCheck2;
    }
}
