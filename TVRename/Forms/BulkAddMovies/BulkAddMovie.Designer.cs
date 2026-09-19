//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//


using System.Windows.Forms;
using BrightIdeasSoftware;

namespace TVRename
{
    partial class BulkAddMovie
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BulkAddMovie));
            bnCheck1 = new Button();
            bnOpenMonFolder = new Button();
            bnRemoveMonFolder = new Button();
            label2 = new Label();
            lstFMMonitorFolders = new ListBox();
            bnAddMonFolder = new Button();
            label7 = new Label();
            bnOpenIgFolder = new Button();
            bnAddIgFolder = new Button();
            bnRemoveIgFolder = new Button();
            lstFMIgnoreFolders = new ListBox();
            bnVisitTVcom = new Button();
            bnFullAuto = new Button();
            bnFolderMonitorDone = new Button();
            label6 = new Label();
            bnRemoveNewFolder = new Button();
            bnNewFolderOpen = new Button();
            bnIgnoreNewFolder = new Button();
            folderBrowser = new FolderBrowserDialog();
            tabControl1 = new TabControl();
            tbFolders = new TabPage();
            label1 = new Label();
            tbIgnore = new TabPage();
            bnCheck2 = new Button();
            label3 = new Label();
            tbResults = new TabPage();
            olvFMNewShows = new ObjectListViewFlickerFree();
            olvFOlder = new OLVColumn();
            olvMovie = new OLVColumn();
            olvYear = new OLVColumn();
            olvSourceCode = new OLVColumn();
            imagesPassFail = new ImageList(components);
            bnEditEntry = new Button();
            label4 = new Label();
            bnClose = new Button();
            pbProgress = new ProgressBar();
            lblStatusLabel = new Label();
            tabControl1.SuspendLayout();
            tbFolders.SuspendLayout();
            tbIgnore.SuspendLayout();
            tbResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)olvFMNewShows).BeginInit();
            SuspendLayout();
            // 
            // bnCheck1
            // 
            bnCheck1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            bnCheck1.Location = new System.Drawing.Point(898, 443);
            bnCheck1.Margin = new Padding(4, 3, 4, 3);
            bnCheck1.Name = "bnCheck1";
            bnCheck1.Size = new System.Drawing.Size(88, 27);
            bnCheck1.TabIndex = 10;
            bnCheck1.Text = "&Check >>";
            bnCheck1.UseVisualStyleBackColor = true;
            bnCheck1.Click += bnCheck_Click;
            // 
            // bnOpenMonFolder
            // 
            bnOpenMonFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnOpenMonFolder.Enabled = false;
            bnOpenMonFolder.Location = new System.Drawing.Point(196, 443);
            bnOpenMonFolder.Margin = new Padding(4, 3, 4, 3);
            bnOpenMonFolder.Name = "bnOpenMonFolder";
            bnOpenMonFolder.Size = new System.Drawing.Size(88, 27);
            bnOpenMonFolder.TabIndex = 9;
            bnOpenMonFolder.Text = "&Open";
            bnOpenMonFolder.UseVisualStyleBackColor = true;
            bnOpenMonFolder.Click += bnOpenMonFolder_Click;
            // 
            // bnRemoveMonFolder
            // 
            bnRemoveMonFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnRemoveMonFolder.Enabled = false;
            bnRemoveMonFolder.Location = new System.Drawing.Point(102, 443);
            bnRemoveMonFolder.Margin = new Padding(4, 3, 4, 3);
            bnRemoveMonFolder.Name = "bnRemoveMonFolder";
            bnRemoveMonFolder.Size = new System.Drawing.Size(88, 27);
            bnRemoveMonFolder.TabIndex = 8;
            bnRemoveMonFolder.Text = "&Remove";
            bnRemoveMonFolder.UseVisualStyleBackColor = true;
            bnRemoveMonFolder.Click += bnRemoveMonFolder_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(4, 45);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(94, 15);
            label2.TabIndex = 5;
            label2.Text = "&Monitor Folders:";
            // 
            // lstFMMonitorFolders
            // 
            lstFMMonitorFolders.AllowDrop = true;
            lstFMMonitorFolders.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstFMMonitorFolders.FormattingEnabled = true;
            lstFMMonitorFolders.IntegralHeight = false;
            lstFMMonitorFolders.Location = new System.Drawing.Point(7, 63);
            lstFMMonitorFolders.Margin = new Padding(4, 3, 4, 3);
            lstFMMonitorFolders.Name = "lstFMMonitorFolders";
            lstFMMonitorFolders.ScrollAlwaysVisible = true;
            lstFMMonitorFolders.SelectionMode = SelectionMode.MultiExtended;
            lstFMMonitorFolders.Size = new System.Drawing.Size(978, 372);
            lstFMMonitorFolders.TabIndex = 6;
            lstFMMonitorFolders.SelectedIndexChanged += lstFMMonitorFolders_SelectedIndexChanged;
            lstFMMonitorFolders.DragDrop += lstFMMonitorFolders_DragDrop;
            lstFMMonitorFolders.DragOver += lstFMMonitorFolders_DragOver;
            lstFMMonitorFolders.DoubleClick += lstFMMonitorFolders_DoubleClick;
            lstFMMonitorFolders.KeyDown += lstFMMonitorFolders_KeyDown;
            // 
            // bnAddMonFolder
            // 
            bnAddMonFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnAddMonFolder.Location = new System.Drawing.Point(7, 443);
            bnAddMonFolder.Margin = new Padding(4, 3, 4, 3);
            bnAddMonFolder.Name = "bnAddMonFolder";
            bnAddMonFolder.Size = new System.Drawing.Size(88, 27);
            bnAddMonFolder.TabIndex = 7;
            bnAddMonFolder.Text = "&Add";
            bnAddMonFolder.UseVisualStyleBackColor = true;
            bnAddMonFolder.Click += bnAddMonFolder_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(4, 45);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(85, 15);
            label7.TabIndex = 5;
            label7.Text = "&Ignore Folders:";
            // 
            // bnOpenIgFolder
            // 
            bnOpenIgFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnOpenIgFolder.Enabled = false;
            bnOpenIgFolder.Location = new System.Drawing.Point(197, 443);
            bnOpenIgFolder.Margin = new Padding(4, 3, 4, 3);
            bnOpenIgFolder.Name = "bnOpenIgFolder";
            bnOpenIgFolder.Size = new System.Drawing.Size(88, 27);
            bnOpenIgFolder.TabIndex = 9;
            bnOpenIgFolder.Text = "O&pen";
            bnOpenIgFolder.UseVisualStyleBackColor = true;
            bnOpenIgFolder.Click += bnOpenIgFolder_Click;
            // 
            // bnAddIgFolder
            // 
            bnAddIgFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnAddIgFolder.Location = new System.Drawing.Point(7, 443);
            bnAddIgFolder.Margin = new Padding(4, 3, 4, 3);
            bnAddIgFolder.Name = "bnAddIgFolder";
            bnAddIgFolder.Size = new System.Drawing.Size(88, 27);
            bnAddIgFolder.TabIndex = 7;
            bnAddIgFolder.Text = "A&dd";
            bnAddIgFolder.UseVisualStyleBackColor = true;
            bnAddIgFolder.Click += bnAddIgFolder_Click;
            // 
            // bnRemoveIgFolder
            // 
            bnRemoveIgFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnRemoveIgFolder.Enabled = false;
            bnRemoveIgFolder.Location = new System.Drawing.Point(103, 443);
            bnRemoveIgFolder.Margin = new Padding(4, 3, 4, 3);
            bnRemoveIgFolder.Name = "bnRemoveIgFolder";
            bnRemoveIgFolder.Size = new System.Drawing.Size(88, 27);
            bnRemoveIgFolder.TabIndex = 8;
            bnRemoveIgFolder.Text = "Remo&ve";
            bnRemoveIgFolder.UseVisualStyleBackColor = true;
            bnRemoveIgFolder.Click += bnRemoveIgFolder_Click;
            // 
            // lstFMIgnoreFolders
            // 
            lstFMIgnoreFolders.AllowDrop = true;
            lstFMIgnoreFolders.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstFMIgnoreFolders.FormattingEnabled = true;
            lstFMIgnoreFolders.IntegralHeight = false;
            lstFMIgnoreFolders.Location = new System.Drawing.Point(7, 63);
            lstFMIgnoreFolders.Margin = new Padding(4, 3, 4, 3);
            lstFMIgnoreFolders.Name = "lstFMIgnoreFolders";
            lstFMIgnoreFolders.ScrollAlwaysVisible = true;
            lstFMIgnoreFolders.SelectionMode = SelectionMode.MultiExtended;
            lstFMIgnoreFolders.Size = new System.Drawing.Size(978, 372);
            lstFMIgnoreFolders.TabIndex = 6;
            lstFMIgnoreFolders.SelectedIndexChanged += lstFMIgnoreFolders_SelectedIndexChanged;
            lstFMIgnoreFolders.DragDrop += lstFMIgnoreFolders_DragDrop;
            lstFMIgnoreFolders.DragOver += lstFMIgnoreFolders_DragOver;
            lstFMIgnoreFolders.KeyDown += lstFMIgnoreFolders_KeyDown;
            // 
            // bnVisitTVcom
            // 
            bnVisitTVcom.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnVisitTVcom.Enabled = false;
            bnVisitTVcom.Location = new System.Drawing.Point(404, 443);
            bnVisitTVcom.Margin = new Padding(4, 3, 4, 3);
            bnVisitTVcom.Name = "bnVisitTVcom";
            bnVisitTVcom.Size = new System.Drawing.Size(88, 27);
            bnVisitTVcom.TabIndex = 26;
            bnVisitTVcom.Text = "&Visit TVDB";
            bnVisitTVcom.UseVisualStyleBackColor = true;
            bnVisitTVcom.Click += bnVisitTVcom_Click;
            // 
            // bnFullAuto
            // 
            bnFullAuto.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnFullAuto.Location = new System.Drawing.Point(7, 443);
            bnFullAuto.Margin = new Padding(4, 3, 4, 3);
            bnFullAuto.Name = "bnFullAuto";
            bnFullAuto.Size = new System.Drawing.Size(88, 27);
            bnFullAuto.TabIndex = 24;
            bnFullAuto.Text = "&Auto ID All";
            bnFullAuto.UseVisualStyleBackColor = true;
            bnFullAuto.Click += bnFullAuto_Click;
            // 
            // bnFolderMonitorDone
            // 
            bnFolderMonitorDone.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            bnFolderMonitorDone.Location = new System.Drawing.Point(898, 443);
            bnFolderMonitorDone.Margin = new Padding(4, 3, 4, 3);
            bnFolderMonitorDone.Name = "bnFolderMonitorDone";
            bnFolderMonitorDone.Size = new System.Drawing.Size(88, 27);
            bnFolderMonitorDone.TabIndex = 10;
            bnFolderMonitorDone.Text = "A&dd && Close";
            bnFolderMonitorDone.UseVisualStyleBackColor = true;
            bnFolderMonitorDone.Click += bnFolderMonitorDone_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(4, 45);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(72, 15);
            label6.TabIndex = 5;
            label6.Text = "&New Movies";
            // 
            // bnRemoveNewFolder
            // 
            bnRemoveNewFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnRemoveNewFolder.Enabled = false;
            bnRemoveNewFolder.Location = new System.Drawing.Point(204, 443);
            bnRemoveNewFolder.Margin = new Padding(4, 3, 4, 3);
            bnRemoveNewFolder.Name = "bnRemoveNewFolder";
            bnRemoveNewFolder.Size = new System.Drawing.Size(88, 27);
            bnRemoveNewFolder.TabIndex = 9;
            bnRemoveNewFolder.Text = "Re&move";
            bnRemoveNewFolder.UseVisualStyleBackColor = true;
            bnRemoveNewFolder.Click += bnRemoveNewFolder_Click;
            // 
            // bnNewFolderOpen
            // 
            bnNewFolderOpen.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnNewFolderOpen.Enabled = false;
            bnNewFolderOpen.Location = new System.Drawing.Point(498, 443);
            bnNewFolderOpen.Margin = new Padding(4, 3, 4, 3);
            bnNewFolderOpen.Name = "bnNewFolderOpen";
            bnNewFolderOpen.Size = new System.Drawing.Size(88, 27);
            bnNewFolderOpen.TabIndex = 9;
            bnNewFolderOpen.Text = "Open &Folder";
            bnNewFolderOpen.UseVisualStyleBackColor = true;
            bnNewFolderOpen.Click += bnNewFolderOpen_Click;
            // 
            // bnIgnoreNewFolder
            // 
            bnIgnoreNewFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnIgnoreNewFolder.Enabled = false;
            bnIgnoreNewFolder.Location = new System.Drawing.Point(299, 443);
            bnIgnoreNewFolder.Margin = new Padding(4, 3, 4, 3);
            bnIgnoreNewFolder.Name = "bnIgnoreNewFolder";
            bnIgnoreNewFolder.Size = new System.Drawing.Size(88, 27);
            bnIgnoreNewFolder.TabIndex = 9;
            bnIgnoreNewFolder.Text = "&Ignore";
            bnIgnoreNewFolder.UseVisualStyleBackColor = true;
            bnIgnoreNewFolder.Click += bnIgnoreNewFolder_Click;
            // 
            // folderBrowser
            // 
            folderBrowser.ShowNewFolderButton = false;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tbFolders);
            tabControl1.Controls.Add(tbIgnore);
            tabControl1.Controls.Add(tbResults);
            tabControl1.Location = new System.Drawing.Point(0, 0);
            tabControl1.Margin = new Padding(4, 3, 4, 3);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(1002, 503);
            tabControl1.TabIndex = 13;
            // 
            // tbFolders
            // 
            tbFolders.Controls.Add(bnCheck1);
            tbFolders.Controls.Add(label1);
            tbFolders.Controls.Add(label2);
            tbFolders.Controls.Add(bnOpenMonFolder);
            tbFolders.Controls.Add(bnAddMonFolder);
            tbFolders.Controls.Add(bnRemoveMonFolder);
            tbFolders.Controls.Add(lstFMMonitorFolders);
            tbFolders.Location = new System.Drawing.Point(4, 24);
            tbFolders.Margin = new Padding(4, 3, 4, 3);
            tbFolders.Name = "tbFolders";
            tbFolders.Padding = new Padding(4, 3, 4, 3);
            tbFolders.Size = new System.Drawing.Size(994, 475);
            tbFolders.TabIndex = 0;
            tbFolders.Text = "Folders";
            tbFolders.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(4, 3);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(656, 42);
            label1.TabIndex = 5;
            label1.Text = resources.GetString("label1.Text");
            // 
            // tbIgnore
            // 
            tbIgnore.Controls.Add(bnCheck2);
            tbIgnore.Controls.Add(label3);
            tbIgnore.Controls.Add(label7);
            tbIgnore.Controls.Add(lstFMIgnoreFolders);
            tbIgnore.Controls.Add(bnOpenIgFolder);
            tbIgnore.Controls.Add(bnRemoveIgFolder);
            tbIgnore.Controls.Add(bnAddIgFolder);
            tbIgnore.Location = new System.Drawing.Point(4, 24);
            tbIgnore.Margin = new Padding(4, 3, 4, 3);
            tbIgnore.Name = "tbIgnore";
            tbIgnore.Padding = new Padding(4, 3, 4, 3);
            tbIgnore.Size = new System.Drawing.Size(994, 475);
            tbIgnore.TabIndex = 1;
            tbIgnore.Text = "Ignore";
            tbIgnore.UseVisualStyleBackColor = true;
            // 
            // bnCheck2
            // 
            bnCheck2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            bnCheck2.Location = new System.Drawing.Point(896, 443);
            bnCheck2.Margin = new Padding(4, 3, 4, 3);
            bnCheck2.Name = "bnCheck2";
            bnCheck2.Size = new System.Drawing.Size(88, 27);
            bnCheck2.TabIndex = 11;
            bnCheck2.Text = "&Check >>";
            bnCheck2.UseVisualStyleBackColor = true;
            bnCheck2.Click += bnCheck2_Click;
            // 
            // label3
            // 
            label3.Location = new System.Drawing.Point(4, 3);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(568, 42);
            label3.TabIndex = 10;
            label3.Text = "Add folders to this list, to have them ignored when Checking.  Drag and drop, or click \"Add...\" below.  Click \"Check >>\" when done.";
            // 
            // tbResults
            // 
            tbResults.Controls.Add(olvFMNewShows);
            tbResults.Controls.Add(bnEditEntry);
            tbResults.Controls.Add(label4);
            tbResults.Controls.Add(label6);
            tbResults.Controls.Add(bnFullAuto);
            tbResults.Controls.Add(bnVisitTVcom);
            tbResults.Controls.Add(bnIgnoreNewFolder);
            tbResults.Controls.Add(bnNewFolderOpen);
            tbResults.Controls.Add(bnFolderMonitorDone);
            tbResults.Controls.Add(bnRemoveNewFolder);
            tbResults.Location = new System.Drawing.Point(4, 24);
            tbResults.Margin = new Padding(4, 3, 4, 3);
            tbResults.Name = "tbResults";
            tbResults.Size = new System.Drawing.Size(994, 475);
            tbResults.TabIndex = 2;
            tbResults.Text = "Scan Results";
            tbResults.UseVisualStyleBackColor = true;
            // 
            // olvFMNewShows
            // 
            olvFMNewShows.AllColumns.Add(olvFOlder);
            olvFMNewShows.AllColumns.Add(olvMovie);
            olvFMNewShows.AllColumns.Add(olvYear);
            olvFMNewShows.AllColumns.Add(olvSourceCode);
            olvFMNewShows.AllowDrop = true;
            olvFMNewShows.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            olvFMNewShows.Columns.AddRange(new ColumnHeader[] { olvFOlder, olvMovie, olvYear, olvSourceCode });
            olvFMNewShows.FullRowSelect = true;
            olvFMNewShows.Location = new System.Drawing.Point(4, 63);
            olvFMNewShows.Name = "olvFMNewShows";
            olvFMNewShows.ShowCommandMenuOnRightClick = true;
            olvFMNewShows.Size = new System.Drawing.Size(982, 374);
            olvFMNewShows.SmallImageList = imagesPassFail;
            olvFMNewShows.TabIndex = 11;
            olvFMNewShows.UseFilterIndicator = true;
            olvFMNewShows.UseFiltering = true;
            olvFMNewShows.View = View.Details;
            olvFMNewShows.SelectedIndexChanged += lvFMNewShows_SelectedIndexChanged;
            olvFMNewShows.DragDrop += lvFMNewShows_DragDrop;
            olvFMNewShows.DragOver += lvFMNewShows_DragOver;
            olvFMNewShows.KeyDown += lvFMNewShows_KeyDown;
            olvFMNewShows.MouseDoubleClick += lvFMNewShows_MouseDoubleClick;
            // 
            // olvFOlder
            // 
            olvFOlder.AspectName = "Folder";
            olvFOlder.Hideable = false;
            olvFOlder.ImageAspectName = "ImageTypeName";
            olvFOlder.MinimumWidth = 10;
            olvFOlder.Text = "Folder";
            olvFOlder.Width = 240;
            // 
            // olvMovie
            // 
            olvMovie.AspectName = "Movie";
            olvMovie.IsEditable = false;
            olvMovie.MinimumWidth = 10;
            olvMovie.Searchable = false;
            olvMovie.Text = "Movie";
            olvMovie.Width = 277;
            // 
            // olvYear
            // 
            olvYear.AspectName = "Year";
            olvYear.IsEditable = false;
            olvYear.MinimumWidth = 10;
            olvYear.Searchable = false;
            olvYear.Text = "Year";
            olvYear.Width = 100;
            // 
            // olvSourceCode
            // 
            olvSourceCode.AspectName = "SourceCode";
            olvSourceCode.MinimumWidth = 10;
            olvSourceCode.Text = "Source Code";
            olvSourceCode.Width = 94;
            // 
            // imagesPassFail
            // 
            imagesPassFail.ColorDepth = ColorDepth.Depth24Bit;
            imagesPassFail.ImageStream = (ImageListStreamer)resources.GetObject("imagesPassFail.ImageStream");
            imagesPassFail.TransparentColor = System.Drawing.Color.Transparent;
            imagesPassFail.Images.SetKeyName(0, "fail");
            imagesPassFail.Images.SetKeyName(1, "pass");
            // 
            // bnEditEntry
            // 
            bnEditEntry.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnEditEntry.Enabled = false;
            bnEditEntry.Location = new System.Drawing.Point(108, 443);
            bnEditEntry.Margin = new Padding(4, 3, 4, 3);
            bnEditEntry.Name = "bnEditEntry";
            bnEditEntry.Size = new System.Drawing.Size(88, 27);
            bnEditEntry.TabIndex = 28;
            bnEditEntry.Text = "&Edit";
            bnEditEntry.UseVisualStyleBackColor = true;
            bnEditEntry.Click += bnEditEntry_Click;
            // 
            // label4
            // 
            label4.Location = new System.Drawing.Point(4, 3);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(752, 37);
            label4.TabIndex = 12;
            label4.Text = resources.GetString("label4.Text");
            // 
            // bnClose
            // 
            bnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            bnClose.DialogResult = DialogResult.Cancel;
            bnClose.Location = new System.Drawing.Point(903, 510);
            bnClose.Margin = new Padding(4, 3, 4, 3);
            bnClose.Name = "bnClose";
            bnClose.Size = new System.Drawing.Size(88, 27);
            bnClose.TabIndex = 27;
            bnClose.Text = "Close";
            bnClose.UseVisualStyleBackColor = true;
            bnClose.Click += bnClose_Click;
            // 
            // pbProgress
            // 
            pbProgress.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            pbProgress.Location = new System.Drawing.Point(5, 510);
            pbProgress.Margin = new Padding(4, 3, 4, 3);
            pbProgress.Name = "pbProgress";
            pbProgress.Size = new System.Drawing.Size(196, 27);
            pbProgress.TabIndex = 28;
            pbProgress.Visible = false;
            // 
            // lblStatusLabel
            // 
            lblStatusLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblStatusLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            lblStatusLabel.Location = new System.Drawing.Point(208, 510);
            lblStatusLabel.Margin = new Padding(4, 0, 4, 0);
            lblStatusLabel.Name = "lblStatusLabel";
            lblStatusLabel.Size = new System.Drawing.Size(363, 27);
            lblStatusLabel.TabIndex = 29;
            lblStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BulkAddMovie
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1002, 550);
            Controls.Add(lblStatusLabel);
            Controls.Add(pbProgress);
            Controls.Add(bnClose);
            Controls.Add(tabControl1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(872, 340);
            Name = "BulkAddMovie";
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Bulk Add Movies";
            tabControl1.ResumeLayout(false);
            tbFolders.ResumeLayout(false);
            tbFolders.PerformLayout();
            tbIgnore.ResumeLayout(false);
            tbIgnore.PerformLayout();
            tbResults.ResumeLayout(false);
            tbResults.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)olvFMNewShows).EndInit();
            ResumeLayout(false);
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
        private System.Windows.Forms.ImageList imagesPassFail;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Label lblStatusLabel;
        private ObjectListViewFlickerFree olvFMNewShows;
        private OLVColumn olvFOlder;
        private OLVColumn olvMovie;
        private OLVColumn olvYear;
        private OLVColumn olvSourceCode;

    }
}
