namespace TVRename.Forms
{
    partial class RecommendationView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecommendationView));
            this.btnClose = new System.Windows.Forms.Button();
            this.rightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.bwScan = new System.ComponentModel.BackgroundWorker();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lvRecommendations = new TVRename.ObjectListViewFlickerFree();
            this.olvId = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvScore = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvYear = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvRating = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvTop = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvPopular = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvLanguage = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.label1 = new System.Windows.Forms.Label();
            this.chkRemoveExisting = new System.Windows.Forms.CheckBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.chrRecommendationPreview = new CefSharp.WinForms.ChromiumWebBrowser();
            this.btnPreferences = new System.Windows.Forms.Button();
            this.rightClickMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lvRecommendations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(1069, 711);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // rightClickMenu
            // 
            this.rightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.rightClickMenu.Name = "menuSearchSites";
            this.rightClickMenu.ShowImageMargin = false;
            this.rightClickMenu.Size = new System.Drawing.Size(156, 26);
            this.rightClickMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.rightClickMenu_ItemClicked);
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
            this.lblStatus.Location = new System.Drawing.Point(112, 716);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 11;
            this.lblStatus.Visible = false;
            // 
            // pbProgress
            // 
            this.pbProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pbProgress.Location = new System.Drawing.Point(6, 711);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(100, 23);
            this.pbProgress.TabIndex = 10;
            this.pbProgress.Visible = false;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(6, 394);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 9;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click_1);
            // 
            // lvRecommendations
            // 
            this.lvRecommendations.AllColumns.Add(this.olvId);
            this.lvRecommendations.AllColumns.Add(this.olvName);
            this.lvRecommendations.AllColumns.Add(this.olvScore);
            this.lvRecommendations.AllColumns.Add(this.olvYear);
            this.lvRecommendations.AllColumns.Add(this.olvRating);
            this.lvRecommendations.AllColumns.Add(this.olvTop);
            this.lvRecommendations.AllColumns.Add(this.olvPopular);
            this.lvRecommendations.AllColumns.Add(this.olvLanguage);
            this.lvRecommendations.AllowColumnReorder = true;
            this.lvRecommendations.CellEditUseWholeCell = false;
            this.lvRecommendations.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvId,
            this.olvName,
            this.olvScore,
            this.olvYear,
            this.olvRating,
            this.olvTop,
            this.olvPopular,
            this.olvLanguage});
            this.lvRecommendations.ContextMenuStrip = this.rightClickMenu;
            this.lvRecommendations.Cursor = System.Windows.Forms.Cursors.Default;
            this.lvRecommendations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvRecommendations.FullRowSelect = true;
            this.lvRecommendations.HideSelection = false;
            this.lvRecommendations.IncludeColumnHeadersInCopy = true;
            this.lvRecommendations.Location = new System.Drawing.Point(0, 0);
            this.lvRecommendations.MultiSelect = false;
            this.lvRecommendations.Name = "lvRecommendations";
            this.lvRecommendations.ShowCommandMenuOnRightClick = true;
            this.lvRecommendations.ShowItemCountOnGroups = true;
            this.lvRecommendations.ShowItemToolTips = true;
            this.lvRecommendations.Size = new System.Drawing.Size(663, 676);
            this.lvRecommendations.SortGroupItemsByPrimaryColumn = false;
            this.lvRecommendations.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.lvRecommendations.TabIndex = 12;
            this.lvRecommendations.UseCompatibleStateImageBehavior = false;
            this.lvRecommendations.UseFilterIndicator = true;
            this.lvRecommendations.UseFiltering = true;
            this.lvRecommendations.View = System.Windows.Forms.View.Details;
            this.lvRecommendations.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.lvRecommendations_CellRightClick);
            this.lvRecommendations.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvRecommendations_ItemSelectionChanged);
            // 
            // olvId
            // 
            this.olvId.AspectName = "Key";
            this.olvId.Groupable = false;
            this.olvId.IsEditable = false;
            this.olvId.Text = "Id";
            this.olvId.UseFiltering = false;
            // 
            // olvName
            // 
            this.olvName.AspectName = "Name";
            this.olvName.Text = "Name";
            this.olvName.UseInitialLetterForGroup = true;
            this.olvName.Width = 117;
            // 
            // olvScore
            // 
            this.olvScore.AspectName = "RecommendationScore";
            this.olvScore.AspectToStringFormat = "{0:p1}";
            this.olvScore.Text = "Recommendation %";
            this.olvScore.Width = 112;
            // 
            // olvYear
            // 
            this.olvYear.AspectName = "Year";
            this.olvYear.Text = "Year";
            // 
            // olvRating
            // 
            this.olvRating.AspectName = "StarScore";
            this.olvRating.AspectToStringFormat = "{0:0.00}";
            this.olvRating.Text = "Quality Rating";
            this.olvRating.Width = 88;
            // 
            // olvTop
            // 
            this.olvTop.AspectName = "TopRated";
            this.olvTop.Text = "Top";
            // 
            // olvPopular
            // 
            this.olvPopular.AspectName = "Trending";
            this.olvPopular.Text = "Popular";
            // 
            // olvLanguage
            // 
            this.olvLanguage.AspectName = "Language";
            this.olvLanguage.Text = "Language";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Checks Performed:";
            // 
            // chkRemoveExisting
            // 
            this.chkRemoveExisting.AutoSize = true;
            this.chkRemoveExisting.Checked = true;
            this.chkRemoveExisting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRemoveExisting.Location = new System.Drawing.Point(113, 13);
            this.chkRemoveExisting.Name = "chkRemoveExisting";
            this.chkRemoveExisting.Size = new System.Drawing.Size(144, 17);
            this.chkRemoveExisting.TabIndex = 1;
            this.chkRemoveExisting.Text = "Remove already in library";
            this.chkRemoveExisting.UseVisualStyleBackColor = true;
            this.chkRemoveExisting.CheckedChanged += new System.EventHandler(this.chkAirDateTest_CheckedChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(6, 29);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lvRecommendations);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.chrRecommendationPreview);
            this.splitContainer1.Size = new System.Drawing.Size(1144, 676);
            this.splitContainer1.SplitterDistance = 663;
            this.splitContainer1.TabIndex = 13;
            // 
            // chrRecommendationPreview
            // 
            this.chrRecommendationPreview.ActivateBrowserOnCreation = false;
            this.chrRecommendationPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chrRecommendationPreview.Location = new System.Drawing.Point(0, 0);
            this.chrRecommendationPreview.Name = "chrRecommendationPreview";
            this.chrRecommendationPreview.Size = new System.Drawing.Size(477, 676);
            this.chrRecommendationPreview.TabIndex = 0;
            // 
            // btnPreferences
            // 
            this.btnPreferences.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreferences.Location = new System.Drawing.Point(1034, 1);
            this.btnPreferences.Name = "btnPreferences";
            this.btnPreferences.Size = new System.Drawing.Size(110, 22);
            this.btnPreferences.TabIndex = 14;
            this.btnPreferences.Text = "Preferences";
            this.btnPreferences.UseVisualStyleBackColor = true;
            this.btnPreferences.Click += new System.EventHandler(this.btnPreferences_Click);
            // 
            // RecommendationView
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1156, 737);
            this.Controls.Add(this.btnPreferences);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.chkRemoveExisting);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "RecommendationView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Recommendations";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.this_FormClosing);
            this.rightClickMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lvRecommendations)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ContextMenuStrip rightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.ComponentModel.BackgroundWorker bwScan;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Button btnRefresh;
        private ObjectListViewFlickerFree lvRecommendations;
        private BrightIdeasSoftware.OLVColumn olvName;
        private BrightIdeasSoftware.OLVColumn olvScore;
        private BrightIdeasSoftware.OLVColumn olvYear;
        private BrightIdeasSoftware.OLVColumn olvRating;
        private BrightIdeasSoftware.OLVColumn olvTop;
        private BrightIdeasSoftware.OLVColumn olvPopular;
        private BrightIdeasSoftware.OLVColumn olvLanguage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkRemoveExisting;
        private BrightIdeasSoftware.OLVColumn olvId;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private CefSharp.WinForms.ChromiumWebBrowser chrRecommendationPreview;
        private System.Windows.Forms.Button btnPreferences;
    }
}
