namespace TVRename.Forms
{
    partial class YtsViewerView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(YtsViewerView));
            btnClose = new System.Windows.Forms.Button();
            rightClickMenu = new System.Windows.Forms.ContextMenuStrip(components);
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            lblStatus = new System.Windows.Forms.Label();
            pbProgress = new System.Windows.Forms.ProgressBar();
            btnRefresh = new System.Windows.Forms.Button();
            lvRecommendations = new TVRename.ObjectListViewFlickerFree<TVRename.Forms.YtsViewerRow>();
            olvId = new BrightIdeasSoftware.OLVColumn();
            olvName = new BrightIdeasSoftware.OLVColumn();
            olvYear = new BrightIdeasSoftware.OLVColumn();
            olvRating = new BrightIdeasSoftware.OLVColumn();
            olvPopular = new BrightIdeasSoftware.OLVColumn();
            olvGenres = new BrightIdeasSoftware.OLVColumn();
            olvLanguage = new BrightIdeasSoftware.OLVColumn();
            label1 = new System.Windows.Forms.Label();
            chkRemoveExisting = new System.Windows.Forms.CheckBox();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            chrRecommendationPreview = new CefSharp.WinForms.ChromiumWebBrowser();
            btnPreferences = new System.Windows.Forms.Button();
            rightClickMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)lvRecommendations).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // btnClose
            // 
            btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnClose.Location = new System.Drawing.Point(1247, 820);
            btnClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(88, 27);
            btnClose.TabIndex = 6;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            // 
            // rightClickMenu
            // 
            rightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripMenuItem1 });
            rightClickMenu.Name = "menuSearchSites";
            rightClickMenu.ShowImageMargin = false;
            rightClickMenu.Size = new System.Drawing.Size(156, 26);
            rightClickMenu.ItemClicked += rightClickMenu_ItemClicked;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(155, 22);
            toolStripMenuItem1.Text = "toolStripMenuItem1";
            // 
            // lblStatus
            // 
            lblStatus.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblStatus.AutoSize = true;
            lblStatus.Location = new System.Drawing.Point(131, 826);
            lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(0, 15);
            lblStatus.TabIndex = 11;
            lblStatus.Visible = false;
            // 
            // pbProgress
            // 
            pbProgress.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            pbProgress.Location = new System.Drawing.Point(7, 820);
            pbProgress.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pbProgress.Name = "pbProgress";
            pbProgress.Size = new System.Drawing.Size(117, 27);
            pbProgress.TabIndex = 10;
            pbProgress.Visible = false;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new System.Drawing.Point(7, 455);
            btnRefresh.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new System.Drawing.Size(88, 27);
            btnRefresh.TabIndex = 9;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += BtnRefresh_Click_1;
            // 
            // lvRecommendations
            // 
            lvRecommendations.AllColumns.Add(olvId);
            lvRecommendations.AllColumns.Add(olvName);
            lvRecommendations.AllColumns.Add(olvYear);
            lvRecommendations.AllColumns.Add(olvRating);
            lvRecommendations.AllColumns.Add(olvPopular);
            lvRecommendations.AllColumns.Add(olvGenres);
            lvRecommendations.AllColumns.Add(olvLanguage);
            lvRecommendations.AllowColumnReorder = true;
            lvRecommendations.CellEditUseWholeCell = false;
            lvRecommendations.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { olvId, olvName, olvYear, olvRating, olvPopular, olvGenres, olvLanguage });
            lvRecommendations.ContextMenuStrip = rightClickMenu;
            lvRecommendations.Dock = System.Windows.Forms.DockStyle.Fill;
            lvRecommendations.FullRowSelect = true;
            lvRecommendations.IncludeColumnHeadersInCopy = true;
            lvRecommendations.Location = new System.Drawing.Point(0, 0);
            lvRecommendations.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lvRecommendations.MultiSelect = false;
            lvRecommendations.Name = "lvRecommendations";
            lvRecommendations.ShowCommandMenuOnRightClick = true;
            lvRecommendations.ShowItemCountOnGroups = true;
            lvRecommendations.ShowItemToolTips = true;
            lvRecommendations.Size = new System.Drawing.Size(773, 780);
            lvRecommendations.SortGroupItemsByPrimaryColumn = false;
            lvRecommendations.Sorting = System.Windows.Forms.SortOrder.Descending;
            lvRecommendations.TabIndex = 12;
            lvRecommendations.UseCompatibleStateImageBehavior = false;
            lvRecommendations.UseFilterIndicator = true;
            lvRecommendations.UseFiltering = true;
            lvRecommendations.View = System.Windows.Forms.View.Details;
            lvRecommendations.CellRightClick += lvRecommendations_CellRightClick;
            lvRecommendations.ItemSelectionChanged += lvRecommendations_ItemSelectionChanged;
            // 
            // olvId
            // 
            olvId.AspectName = "Id";
            olvId.Groupable = false;
            olvId.IsEditable = false;
            olvId.Text = "Id";
            olvId.UseFiltering = false;
            olvId.Width = 70;
            // 
            // olvName
            // 
            olvName.AspectName = "Name";
            olvName.Text = "Name";
            olvName.UseInitialLetterForGroup = true;
            olvName.Width = 136;
            // 
            // olvYear
            // 
            olvYear.AspectName = "Year";
            olvYear.Text = "Year";
            olvYear.Width = 70;
            // 
            // olvRating
            // 
            olvRating.AspectName = "StarScore";
            olvRating.AspectToStringFormat = "{0:0.00}";
            olvRating.Text = "Quality Rating";
            olvRating.Width = 103;
            // 
            // olvPopular
            // 
            olvPopular.AspectName = "ContentRating";
            olvPopular.Text = "Rating";
            olvPopular.Width = 70;
            // 
            // olvGenres
            // 
            olvGenres.AspectName = "GenresString";
            olvGenres.Text = "Genres";
            olvGenres.Width = 70;
            // 
            // olvLanguage
            // 
            olvLanguage.AspectName = "Language";
            olvLanguage.Text = "Language";
            olvLanguage.Width = 70;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(15, 15);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(107, 15);
            label1.TabIndex = 7;
            label1.Text = "Checks Performed:";
            // 
            // chkRemoveExisting
            // 
            chkRemoveExisting.AutoSize = true;
            chkRemoveExisting.Checked = true;
            chkRemoveExisting.CheckState = System.Windows.Forms.CheckState.Checked;
            chkRemoveExisting.Location = new System.Drawing.Point(132, 15);
            chkRemoveExisting.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkRemoveExisting.Name = "chkRemoveExisting";
            chkRemoveExisting.Size = new System.Drawing.Size(159, 19);
            chkRemoveExisting.TabIndex = 1;
            chkRemoveExisting.Text = "Remove already in library";
            chkRemoveExisting.UseVisualStyleBackColor = true;
            chkRemoveExisting.CheckedChanged += chkAirDateTest_CheckedChanged;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            splitContainer1.Location = new System.Drawing.Point(7, 33);
            splitContainer1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(lvRecommendations);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(chrRecommendationPreview);
            splitContainer1.Size = new System.Drawing.Size(1335, 780);
            splitContainer1.SplitterDistance = 773;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 13;
            // 
            // chrRecommendationPreview
            // 
            chrRecommendationPreview.ActivateBrowserOnCreation = false;
            chrRecommendationPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            chrRecommendationPreview.Location = new System.Drawing.Point(0, 0);
            chrRecommendationPreview.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chrRecommendationPreview.Name = "chrRecommendationPreview";
            chrRecommendationPreview.Size = new System.Drawing.Size(557, 780);
            chrRecommendationPreview.TabIndex = 0;
            // 
            // btnPreferences
            // 
            btnPreferences.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnPreferences.Location = new System.Drawing.Point(1206, 1);
            btnPreferences.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnPreferences.Name = "btnPreferences";
            btnPreferences.Size = new System.Drawing.Size(128, 25);
            btnPreferences.TabIndex = 14;
            btnPreferences.Text = "Preferences";
            btnPreferences.UseVisualStyleBackColor = true;
            btnPreferences.Click += btnPreferences_Click;
            // 
            // YtsViewerView
            // 
            AcceptButton = btnClose;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnClose;
            ClientSize = new System.Drawing.Size(1349, 850);
            Controls.Add(btnPreferences);
            Controls.Add(splitContainer1);
            Controls.Add(lblStatus);
            Controls.Add(pbProgress);
            Controls.Add(btnRefresh);
            Controls.Add(label1);
            Controls.Add(btnClose);
            Controls.Add(chkRemoveExisting);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(697, 456);
            Name = "YtsViewerView";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "YTS Movie Preview";
            FormClosing += this_FormClosing;
            rightClickMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)lvRecommendations).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ContextMenuStrip rightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Button btnRefresh;
        private ObjectListViewFlickerFree<YtsViewerRow> lvRecommendations;
        private BrightIdeasSoftware.OLVColumn olvName;
        private BrightIdeasSoftware.OLVColumn olvYear;
        private BrightIdeasSoftware.OLVColumn olvRating;
        private BrightIdeasSoftware.OLVColumn olvPopular;
        private BrightIdeasSoftware.OLVColumn olvGenres;
        private BrightIdeasSoftware.OLVColumn olvLanguage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkRemoveExisting;
        private BrightIdeasSoftware.OLVColumn olvId;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private CefSharp.WinForms.ChromiumWebBrowser chrRecommendationPreview;
        private System.Windows.Forms.Button btnPreferences;
    }
}
