//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
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
            System.ComponentModel.ComponentResourceManager resources = (new System.ComponentModel.ComponentResourceManager(typeof(FolderMonitor)));
            this.splitContainer3 = (new System.Windows.Forms.SplitContainer());
            this.tableLayoutPanel1 = (new System.Windows.Forms.TableLayoutPanel());
            this.panel2 = (new System.Windows.Forms.Panel());
            this.bnFMCheck = (new System.Windows.Forms.Button());
            this.bnFMOpenMonFolder = (new System.Windows.Forms.Button());
            this.bnFMRemoveMonFolder = (new System.Windows.Forms.Button());
            this.label2 = (new System.Windows.Forms.Label());
            this.lstFMMonitorFolders = (new System.Windows.Forms.ListBox());
            this.bnFMAddMonFolder = (new System.Windows.Forms.Button());
            this.panel3 = (new System.Windows.Forms.Panel());
            this.label7 = (new System.Windows.Forms.Label());
            this.bnFMOpenIgFolder = (new System.Windows.Forms.Button());
            this.bnFMAddIgFolder = (new System.Windows.Forms.Button());
            this.bnFMRemoveIgFolder = (new System.Windows.Forms.Button());
            this.lstFMIgnoreFolders = (new System.Windows.Forms.ListBox());
            this.bnClose = (new System.Windows.Forms.Button());
            this.txtFMSpecificSeason = (new System.Windows.Forms.TextBox());
            this.rbSpecificSeason = (new System.Windows.Forms.RadioButton());
            this.rbFlat = (new System.Windows.Forms.RadioButton());
            this.rbFolderPerSeason = (new System.Windows.Forms.RadioButton());
            this.bnFMVisitTVcom = (new System.Windows.Forms.Button());
            this.pnlCF = (new System.Windows.Forms.Panel());
            this.bnFMFullAuto = (new System.Windows.Forms.Button());
            this.lvFMNewShows = (new System.Windows.Forms.ListView());
            this.columnHeader42 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader43 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader44 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader45 = (new System.Windows.Forms.ColumnHeader());
            this.bnAddThisOne = (new System.Windows.Forms.Button());
            this.bnFolderMonitorDone = (new System.Windows.Forms.Button());
            this.label6 = (new System.Windows.Forms.Label());
            this.bnFMRemoveNewFolder = (new System.Windows.Forms.Button());
            this.bnFMNewFolderOpen = (new System.Windows.Forms.Button());
            this.bnFMIgnoreAllNewFolders = (new System.Windows.Forms.Button());
            this.bnFMIgnoreNewFolder = (new System.Windows.Forms.Button());
            this.folderBrowser = (new System.Windows.Forms.FolderBrowserDialog());
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.bnClose);
            this.splitContainer3.Panel2.Controls.Add(this.txtFMSpecificSeason);
            this.splitContainer3.Panel2.Controls.Add(this.rbSpecificSeason);
            this.splitContainer3.Panel2.Controls.Add(this.rbFlat);
            this.splitContainer3.Panel2.Controls.Add(this.rbFolderPerSeason);
            this.splitContainer3.Panel2.Controls.Add(this.bnFMVisitTVcom);
            this.splitContainer3.Panel2.Controls.Add(this.pnlCF);
            this.splitContainer3.Panel2.Controls.Add(this.bnFMFullAuto);
            this.splitContainer3.Panel2.Controls.Add(this.lvFMNewShows);
            this.splitContainer3.Panel2.Controls.Add(this.bnAddThisOne);
            this.splitContainer3.Panel2.Controls.Add(this.bnFolderMonitorDone);
            this.splitContainer3.Panel2.Controls.Add(this.label6);
            this.splitContainer3.Panel2.Controls.Add(this.bnFMRemoveNewFolder);
            this.splitContainer3.Panel2.Controls.Add(this.bnFMNewFolderOpen);
            this.splitContainer3.Panel2.Controls.Add(this.bnFMIgnoreAllNewFolders);
            this.splitContainer3.Panel2.Controls.Add(this.bnFMIgnoreNewFolder);
            this.splitContainer3.Size = new System.Drawing.Size(887, 634);
            this.splitContainer3.SplitterDistance = 190;
            this.splitContainer3.SplitterWidth = 5;
            this.splitContainer3.TabIndex = 12;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add((new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50)));
            this.tableLayoutPanel1.ColumnStyles.Add((new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50)));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add((new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100)));
            this.tableLayoutPanel1.RowStyles.Add((new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 190)));
            this.tableLayoutPanel1.RowStyles.Add((new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 190)));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(887, 190);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.bnFMCheck);
            this.panel2.Controls.Add(this.bnFMOpenMonFolder);
            this.panel2.Controls.Add(this.bnFMRemoveMonFolder);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.lstFMMonitorFolders);
            this.panel2.Controls.Add(this.bnFMAddMonFolder);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(437, 184);
            this.panel2.TabIndex = 0;
            // 
            // bnFMCheck
            // 
            this.bnFMCheck.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.bnFMCheck.Location = new System.Drawing.Point(359, 158);
            this.bnFMCheck.Name = "bnFMCheck";
            this.bnFMCheck.Size = new System.Drawing.Size(75, 23);
            this.bnFMCheck.TabIndex = 10;
            this.bnFMCheck.Text = "&Check";
            this.bnFMCheck.UseVisualStyleBackColor = true;
            this.bnFMCheck.Click += new System.EventHandler(bnFMCheck_Click);
            // 
            // bnFMOpenMonFolder
            // 
            this.bnFMOpenMonFolder.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnFMOpenMonFolder.Location = new System.Drawing.Point(165, 158);
            this.bnFMOpenMonFolder.Name = "bnFMOpenMonFolder";
            this.bnFMOpenMonFolder.Size = new System.Drawing.Size(75, 23);
            this.bnFMOpenMonFolder.TabIndex = 9;
            this.bnFMOpenMonFolder.Text = "&Open";
            this.bnFMOpenMonFolder.UseVisualStyleBackColor = true;
            this.bnFMOpenMonFolder.Click += new System.EventHandler(bnFMOpenMonFolder_Click);
            // 
            // bnFMRemoveMonFolder
            // 
            this.bnFMRemoveMonFolder.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnFMRemoveMonFolder.Location = new System.Drawing.Point(84, 158);
            this.bnFMRemoveMonFolder.Name = "bnFMRemoveMonFolder";
            this.bnFMRemoveMonFolder.Size = new System.Drawing.Size(75, 23);
            this.bnFMRemoveMonFolder.TabIndex = 8;
            this.bnFMRemoveMonFolder.Text = "&Remove";
            this.bnFMRemoveMonFolder.UseVisualStyleBackColor = true;
            this.bnFMRemoveMonFolder.Click += new System.EventHandler(bnFMRemoveMonFolder_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "&Monitor Folders:";
            // 
            // lstFMMonitorFolders
            // 
            this.lstFMMonitorFolders.AllowDrop = true;
            this.lstFMMonitorFolders.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            this.lstFMMonitorFolders.FormattingEnabled = true;
            this.lstFMMonitorFolders.IntegralHeight = false;
            this.lstFMMonitorFolders.Location = new System.Drawing.Point(3, 16);
            this.lstFMMonitorFolders.Name = "lstFMMonitorFolders";
            this.lstFMMonitorFolders.ScrollAlwaysVisible = true;
            this.lstFMMonitorFolders.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstFMMonitorFolders.Size = new System.Drawing.Size(431, 136);
            this.lstFMMonitorFolders.TabIndex = 6;
            // 
            // bnFMAddMonFolder
            // 
            this.bnFMAddMonFolder.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnFMAddMonFolder.Location = new System.Drawing.Point(3, 158);
            this.bnFMAddMonFolder.Name = "bnFMAddMonFolder";
            this.bnFMAddMonFolder.Size = new System.Drawing.Size(75, 23);
            this.bnFMAddMonFolder.TabIndex = 7;
            this.bnFMAddMonFolder.Text = "&Add";
            this.bnFMAddMonFolder.UseVisualStyleBackColor = true;
            this.bnFMAddMonFolder.Click += new System.EventHandler(bnFMAddMonFolder_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.bnFMOpenIgFolder);
            this.panel3.Controls.Add(this.bnFMAddIgFolder);
            this.panel3.Controls.Add(this.bnFMRemoveIgFolder);
            this.panel3.Controls.Add(this.lstFMIgnoreFolders);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(446, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(438, 184);
            this.panel3.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "&Ignore Folders:";
            // 
            // bnFMOpenIgFolder
            // 
            this.bnFMOpenIgFolder.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnFMOpenIgFolder.Location = new System.Drawing.Point(165, 158);
            this.bnFMOpenIgFolder.Name = "bnFMOpenIgFolder";
            this.bnFMOpenIgFolder.Size = new System.Drawing.Size(75, 23);
            this.bnFMOpenIgFolder.TabIndex = 9;
            this.bnFMOpenIgFolder.Text = "O&pen";
            this.bnFMOpenIgFolder.UseVisualStyleBackColor = true;
            this.bnFMOpenIgFolder.Click += new System.EventHandler(bnFMOpenIgFolder_Click);
            // 
            // bnFMAddIgFolder
            // 
            this.bnFMAddIgFolder.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnFMAddIgFolder.Location = new System.Drawing.Point(2, 158);
            this.bnFMAddIgFolder.Name = "bnFMAddIgFolder";
            this.bnFMAddIgFolder.Size = new System.Drawing.Size(75, 23);
            this.bnFMAddIgFolder.TabIndex = 7;
            this.bnFMAddIgFolder.Text = "A&dd";
            this.bnFMAddIgFolder.UseVisualStyleBackColor = true;
            this.bnFMAddIgFolder.Click += new System.EventHandler(bnFMAddIgFolder_Click);
            // 
            // bnFMRemoveIgFolder
            // 
            this.bnFMRemoveIgFolder.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnFMRemoveIgFolder.Location = new System.Drawing.Point(84, 158);
            this.bnFMRemoveIgFolder.Name = "bnFMRemoveIgFolder";
            this.bnFMRemoveIgFolder.Size = new System.Drawing.Size(75, 23);
            this.bnFMRemoveIgFolder.TabIndex = 8;
            this.bnFMRemoveIgFolder.Text = "Remo&ve";
            this.bnFMRemoveIgFolder.UseVisualStyleBackColor = true;
            this.bnFMRemoveIgFolder.Click += new System.EventHandler(bnFMRemoveIgFolder_Click);
            // 
            // lstFMIgnoreFolders
            // 
            this.lstFMIgnoreFolders.AllowDrop = true;
            this.lstFMIgnoreFolders.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            this.lstFMIgnoreFolders.FormattingEnabled = true;
            this.lstFMIgnoreFolders.IntegralHeight = false;
            this.lstFMIgnoreFolders.Location = new System.Drawing.Point(0, 16);
            this.lstFMIgnoreFolders.Name = "lstFMIgnoreFolders";
            this.lstFMIgnoreFolders.ScrollAlwaysVisible = true;
            this.lstFMIgnoreFolders.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstFMIgnoreFolders.Size = new System.Drawing.Size(438, 136);
            this.lstFMIgnoreFolders.TabIndex = 6;
            // 
            // bnClose
            // 
            this.bnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnClose.Location = new System.Drawing.Point(809, 413);
            this.bnClose.Name = "bnClose";
            this.bnClose.Size = new System.Drawing.Size(75, 23);
            this.bnClose.TabIndex = 30;
            this.bnClose.Text = "Close";
            this.bnClose.UseVisualStyleBackColor = true;
            this.bnClose.Click += new System.EventHandler(bnClose_Click);
            // 
            // txtFMSpecificSeason
            // 
            this.txtFMSpecificSeason.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.txtFMSpecificSeason.Location = new System.Drawing.Point(533, 235);
            this.txtFMSpecificSeason.Name = "txtFMSpecificSeason";
            this.txtFMSpecificSeason.Size = new System.Drawing.Size(53, 20);
            this.txtFMSpecificSeason.TabIndex = 29;
            // 
            // rbSpecificSeason
            // 
            this.rbSpecificSeason.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.rbSpecificSeason.AutoSize = true;
            this.rbSpecificSeason.Location = new System.Drawing.Point(433, 236);
            this.rbSpecificSeason.Name = "rbSpecificSeason";
            this.rbSpecificSeason.Size = new System.Drawing.Size(100, 17);
            this.rbSpecificSeason.TabIndex = 28;
            this.rbSpecificSeason.TabStop = true;
            this.rbSpecificSeason.Text = "Specific season";
            this.rbSpecificSeason.UseVisualStyleBackColor = true;
            this.rbSpecificSeason.CheckedChanged += new System.EventHandler(rbSpecificSeason_CheckedChanged);
            // 
            // rbFlat
            // 
            this.rbFlat.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.rbFlat.AutoSize = true;
            this.rbFlat.Location = new System.Drawing.Point(433, 213);
            this.rbFlat.Name = "rbFlat";
            this.rbFlat.Size = new System.Drawing.Size(120, 17);
            this.rbFlat.TabIndex = 28;
            this.rbFlat.TabStop = true;
            this.rbFlat.Text = "All seasons together";
            this.rbFlat.UseVisualStyleBackColor = true;
            this.rbFlat.CheckedChanged += new System.EventHandler(rbFlat_CheckedChanged);
            // 
            // rbFolderPerSeason
            // 
            this.rbFolderPerSeason.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.rbFolderPerSeason.AutoSize = true;
            this.rbFolderPerSeason.Location = new System.Drawing.Point(433, 190);
            this.rbFolderPerSeason.Name = "rbFolderPerSeason";
            this.rbFolderPerSeason.Size = new System.Drawing.Size(109, 17);
            this.rbFolderPerSeason.TabIndex = 28;
            this.rbFolderPerSeason.TabStop = true;
            this.rbFolderPerSeason.Text = "Folder per season";
            this.rbFolderPerSeason.UseVisualStyleBackColor = true;
            this.rbFolderPerSeason.CheckedChanged += new System.EventHandler(rbFolderPerSeason_CheckedChanged);
            // 
            // bnFMVisitTVcom
            // 
            this.bnFMVisitTVcom.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnFMVisitTVcom.Location = new System.Drawing.Point(332, 383);
            this.bnFMVisitTVcom.Name = "bnFMVisitTVcom";
            this.bnFMVisitTVcom.Size = new System.Drawing.Size(75, 23);
            this.bnFMVisitTVcom.TabIndex = 26;
            this.bnFMVisitTVcom.Text = "&Visit TVDB";
            this.bnFMVisitTVcom.UseVisualStyleBackColor = true;
            this.bnFMVisitTVcom.Click += new System.EventHandler(bnFMVisitTVcom_Click);
            // 
            // pnlCF
            // 
            this.pnlCF.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.pnlCF.Location = new System.Drawing.Point(5, 190);
            this.pnlCF.Name = "pnlCF";
            this.pnlCF.Size = new System.Drawing.Size(407, 185);
            this.pnlCF.TabIndex = 25;
            // 
            // bnFMFullAuto
            // 
            this.bnFMFullAuto.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnFMFullAuto.Location = new System.Drawing.Point(8, 413);
            this.bnFMFullAuto.Name = "bnFMFullAuto";
            this.bnFMFullAuto.Size = new System.Drawing.Size(75, 23);
            this.bnFMFullAuto.TabIndex = 24;
            this.bnFMFullAuto.Text = "F&ull Auto";
            this.bnFMFullAuto.UseVisualStyleBackColor = true;
            this.bnFMFullAuto.Click += new System.EventHandler(bnFMFullAuto_Click);
            // 
            // lvFMNewShows
            // 
            this.lvFMNewShows.AllowDrop = true;
            this.lvFMNewShows.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            this.lvFMNewShows.Columns.AddRange(new System.Windows.Forms.ColumnHeader[4] { this.columnHeader42, this.columnHeader43, this.columnHeader44, this.columnHeader45 });
            this.lvFMNewShows.FullRowSelect = true;
            this.lvFMNewShows.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvFMNewShows.HideSelection = false;
            this.lvFMNewShows.Location = new System.Drawing.Point(6, 23);
            this.lvFMNewShows.Name = "lvFMNewShows";
            this.lvFMNewShows.Size = new System.Drawing.Size(881, 161);
            this.lvFMNewShows.TabIndex = 11;
            this.lvFMNewShows.UseCompatibleStateImageBehavior = false;
            this.lvFMNewShows.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader42
            // 
            this.columnHeader42.Text = "Folder";
            this.columnHeader42.Width = 240;
            // 
            // columnHeader43
            // 
            this.columnHeader43.Text = "Show";
            this.columnHeader43.Width = 139;
            // 
            // columnHeader44
            // 
            this.columnHeader44.Text = "Season";
            // 
            // columnHeader45
            // 
            this.columnHeader45.Text = "thetvdb code";
            this.columnHeader45.Width = 94;
            // 
            // bnAddThisOne
            // 
            this.bnAddThisOne.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnAddThisOne.Location = new System.Drawing.Point(433, 383);
            this.bnAddThisOne.Name = "bnAddThisOne";
            this.bnAddThisOne.Size = new System.Drawing.Size(75, 23);
            this.bnAddThisOne.TabIndex = 10;
            this.bnAddThisOne.Text = "Add &This";
            this.bnAddThisOne.UseVisualStyleBackColor = true;
            this.bnAddThisOne.Click += new System.EventHandler(bnAddThisOne_Click);
            // 
            // bnFolderMonitorDone
            // 
            this.bnFolderMonitorDone.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnFolderMonitorDone.Location = new System.Drawing.Point(433, 413);
            this.bnFolderMonitorDone.Name = "bnFolderMonitorDone";
            this.bnFolderMonitorDone.Size = new System.Drawing.Size(75, 23);
            this.bnFolderMonitorDone.TabIndex = 10;
            this.bnFolderMonitorDone.Text = "Do&ne";
            this.bnFolderMonitorDone.UseVisualStyleBackColor = true;
            this.bnFolderMonitorDone.Click += new System.EventHandler(bnFolderMonitorDone_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "&New Shows";
            // 
            // bnFMRemoveNewFolder
            // 
            this.bnFMRemoveNewFolder.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnFMRemoveNewFolder.Location = new System.Drawing.Point(89, 414);
            this.bnFMRemoveNewFolder.Name = "bnFMRemoveNewFolder";
            this.bnFMRemoveNewFolder.Size = new System.Drawing.Size(75, 23);
            this.bnFMRemoveNewFolder.TabIndex = 9;
            this.bnFMRemoveNewFolder.Text = "Re&move";
            this.bnFMRemoveNewFolder.UseVisualStyleBackColor = true;
            this.bnFMRemoveNewFolder.Click += new System.EventHandler(bnFMRemoveNewFolder_Click);
            // 
            // bnFMNewFolderOpen
            // 
            this.bnFMNewFolderOpen.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnFMNewFolderOpen.Location = new System.Drawing.Point(332, 414);
            this.bnFMNewFolderOpen.Name = "bnFMNewFolderOpen";
            this.bnFMNewFolderOpen.Size = new System.Drawing.Size(75, 23);
            this.bnFMNewFolderOpen.TabIndex = 9;
            this.bnFMNewFolderOpen.Text = "Op&en";
            this.bnFMNewFolderOpen.UseVisualStyleBackColor = true;
            this.bnFMNewFolderOpen.Click += new System.EventHandler(bnFMNewFolderOpen_Click);
            // 
            // bnFMIgnoreAllNewFolders
            // 
            this.bnFMIgnoreAllNewFolders.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnFMIgnoreAllNewFolders.Location = new System.Drawing.Point(251, 414);
            this.bnFMIgnoreAllNewFolders.Name = "bnFMIgnoreAllNewFolders";
            this.bnFMIgnoreAllNewFolders.Size = new System.Drawing.Size(75, 23);
            this.bnFMIgnoreAllNewFolders.TabIndex = 9;
            this.bnFMIgnoreAllNewFolders.Text = "Ig&nore All";
            this.bnFMIgnoreAllNewFolders.UseVisualStyleBackColor = true;
            this.bnFMIgnoreAllNewFolders.Click += new System.EventHandler(bnFMIgnoreAllNewFolders_Click);
            // 
            // bnFMIgnoreNewFolder
            // 
            this.bnFMIgnoreNewFolder.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnFMIgnoreNewFolder.Location = new System.Drawing.Point(170, 414);
            this.bnFMIgnoreNewFolder.Name = "bnFMIgnoreNewFolder";
            this.bnFMIgnoreNewFolder.Size = new System.Drawing.Size(75, 23);
            this.bnFMIgnoreNewFolder.TabIndex = 9;
            this.bnFMIgnoreNewFolder.Text = "&Ignore";
            this.bnFMIgnoreNewFolder.UseVisualStyleBackColor = true;
            this.bnFMIgnoreNewFolder.Click += new System.EventHandler(bnFMIgnoreNewFolder_Click);
            // 
            // folderBrowser
            // 
            this.folderBrowser.ShowNewFolderButton = false;
            // 
            // FolderMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnClose;
            this.ClientSize = new System.Drawing.Size(887, 634);
            this.Controls.Add(this.splitContainer3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FolderMonitor";
            this.ShowInTaskbar = false;
            this.Text = "TVRename Folder Monitor";
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            this.splitContainer3.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }


        #endregion

        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button bnFMCheck;
        private System.Windows.Forms.Button bnFMOpenMonFolder;
        private System.Windows.Forms.Button bnFMRemoveMonFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lstFMMonitorFolders;
        private System.Windows.Forms.Button bnFMAddMonFolder;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button bnFMOpenIgFolder;
        private System.Windows.Forms.Button bnFMAddIgFolder;
        private System.Windows.Forms.Button bnFMRemoveIgFolder;
        private System.Windows.Forms.ListBox lstFMIgnoreFolders;
        private System.Windows.Forms.TextBox txtFMSpecificSeason;
        private System.Windows.Forms.RadioButton rbSpecificSeason;
        private System.Windows.Forms.RadioButton rbFlat;
        private System.Windows.Forms.RadioButton rbFolderPerSeason;
        private System.Windows.Forms.Button bnFMVisitTVcom;
        private System.Windows.Forms.Panel pnlCF;
        private System.Windows.Forms.Button bnFMFullAuto;
        private System.Windows.Forms.ListView lvFMNewShows;
        private System.Windows.Forms.ColumnHeader columnHeader42;
        private System.Windows.Forms.ColumnHeader columnHeader43;
        private System.Windows.Forms.ColumnHeader columnHeader44;
        private System.Windows.Forms.ColumnHeader columnHeader45;
        private System.Windows.Forms.Button bnAddThisOne;
        private System.Windows.Forms.Button bnFolderMonitorDone;

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button bnFMRemoveNewFolder;
        private System.Windows.Forms.Button bnFMNewFolderOpen;
        private System.Windows.Forms.Button bnFMIgnoreAllNewFolders;
        private System.Windows.Forms.Button bnFMIgnoreNewFolder;
        private System.Windows.Forms.Button bnClose;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
    }
}