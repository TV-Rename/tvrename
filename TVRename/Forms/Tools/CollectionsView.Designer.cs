namespace TVRename.Forms
{
    partial class CollectionsView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CollectionsView));
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.possibleMergedEpisodeRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.bwScan = new System.ComponentModel.BackgroundWorker();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.olvCollections = new BrightIdeasSoftware.ObjectListView();
            this.olvCollectionName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvMovieName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvInLibrary = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvMovieYear = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.chkRemoveCompleted = new System.Windows.Forms.CheckBox();
            this.possibleMergedEpisodeRightClickMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvCollections)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(925, 664);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Options:";
            // 
            // possibleMergedEpisodeRightClickMenu
            // 
            this.possibleMergedEpisodeRightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.possibleMergedEpisodeRightClickMenu.Name = "menuSearchSites";
            this.possibleMergedEpisodeRightClickMenu.ShowImageMargin = false;
            this.possibleMergedEpisodeRightClickMenu.Size = new System.Drawing.Size(156, 26);
            this.possibleMergedEpisodeRightClickMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.PossibleMergedEpisodeRightClickMenu_ItemClicked);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(155, 22);
            this.toolStripMenuItem1.Text = "toolStripMenuItem1";
            // 
            // bwScan
            // 
            this.bwScan.WorkerReportsProgress = true;
            this.bwScan.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BwScan_DoWork);
            this.bwScan.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BwScan_ProgressChanged);
            this.bwScan.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BwScan_RunWorkerCompleted);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(112, 669);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 11;
            this.lblStatus.Visible = false;
            // 
            // pbProgress
            // 
            this.pbProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pbProgress.Location = new System.Drawing.Point(6, 664);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(100, 23);
            this.pbProgress.TabIndex = 10;
            this.pbProgress.Visible = false;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRefresh.Location = new System.Drawing.Point(6, 664);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 9;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click_1);
            // 
            // olvCollections
            // 
            this.olvCollections.AllColumns.Add(this.olvCollectionName);
            this.olvCollections.AllColumns.Add(this.olvMovieName);
            this.olvCollections.AllColumns.Add(this.olvInLibrary);
            this.olvCollections.AllColumns.Add(this.olvMovieYear);
            this.olvCollections.AllowColumnReorder = true;
            this.olvCollections.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvCollections.CellEditUseWholeCell = false;
            this.olvCollections.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvCollectionName,
            this.olvMovieName,
            this.olvInLibrary,
            this.olvMovieYear});
            this.olvCollections.ContextMenuStrip = this.possibleMergedEpisodeRightClickMenu;
            this.olvCollections.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvCollections.FullRowSelect = true;
            this.olvCollections.HideSelection = false;
            this.olvCollections.IncludeColumnHeadersInCopy = true;
            this.olvCollections.Location = new System.Drawing.Point(6, 29);
            this.olvCollections.MultiSelect = false;
            this.olvCollections.Name = "olvCollections";
            this.olvCollections.ShowCommandMenuOnRightClick = true;
            this.olvCollections.ShowItemCountOnGroups = true;
            this.olvCollections.Size = new System.Drawing.Size(994, 629);
            this.olvCollections.TabIndex = 12;
            this.olvCollections.UseCompatibleStateImageBehavior = false;
            this.olvCollections.UseFilterIndicator = true;
            this.olvCollections.UseFiltering = true;
            this.olvCollections.View = System.Windows.Forms.View.Details;
            this.olvCollections.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.olvDuplicates_CellRightClick);
            // 
            // olvCollectionName
            // 
            this.olvCollectionName.AspectName = "CollectionName";
            this.olvCollectionName.Text = "Collection";
            this.olvCollectionName.Width = 229;
            // 
            // olvMovieName
            // 
            this.olvMovieName.AspectName = "MovieName";
            this.olvMovieName.Groupable = false;
            this.olvMovieName.Text = "Movie Name";
            this.olvMovieName.Width = 204;
            // 
            // olvInLibrary
            // 
            this.olvInLibrary.AspectName = "IsInLibrary";
            this.olvInLibrary.Text = "In Library?";
            this.olvInLibrary.Width = 88;
            // 
            // olvMovieYear
            // 
            this.olvMovieYear.AspectName = "MovieYear";
            this.olvMovieYear.Text = "Year";
            // 
            // chkRemoveCompleted
            // 
            this.chkRemoveCompleted.AutoSize = true;
            this.chkRemoveCompleted.Location = new System.Drawing.Point(78, 9);
            this.chkRemoveCompleted.Name = "chkRemoveCompleted";
            this.chkRemoveCompleted.Size = new System.Drawing.Size(173, 17);
            this.chkRemoveCompleted.TabIndex = 13;
            this.chkRemoveCompleted.Text = "Remove Completed Collections";
            this.chkRemoveCompleted.UseVisualStyleBackColor = true;
            this.chkRemoveCompleted.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // CollectionsView
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1012, 690);
            this.Controls.Add(this.chkRemoveCompleted);
            this.Controls.Add(this.olvCollections);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "CollectionsView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Movie Collections";
            this.possibleMergedEpisodeRightClickMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvCollections)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip possibleMergedEpisodeRightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.ComponentModel.BackgroundWorker bwScan;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Button btnRefresh;
        private BrightIdeasSoftware.ObjectListView olvCollections;
        private BrightIdeasSoftware.OLVColumn olvCollectionName;
        private BrightIdeasSoftware.OLVColumn olvMovieName;
        private BrightIdeasSoftware.OLVColumn olvInLibrary;
        private BrightIdeasSoftware.OLVColumn olvMovieYear;
        private System.Windows.Forms.CheckBox chkRemoveCompleted;
    }
}
