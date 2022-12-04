namespace TVRename.Forms
{
    partial class DuplicateMovieFinder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DuplicateMovieFinder));
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.rightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.bwScan = new System.ComponentModel.BackgroundWorker();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.olvDuplicates = new BrightIdeasSoftware.ObjectListView();
            this.olvName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvFiles = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvDoublePart = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvSample = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvDeleted = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvNumber = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.rightClickMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvDuplicates)).BeginInit();
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
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Checks Performed:";
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
            // olvDuplicates
            // 
            this.olvDuplicates.AllColumns.Add(this.olvName);
            this.olvDuplicates.AllColumns.Add(this.olvFiles);
            this.olvDuplicates.AllColumns.Add(this.olvNumber);
            this.olvDuplicates.AllColumns.Add(this.olvDoublePart);
            this.olvDuplicates.AllColumns.Add(this.olvSample);
            this.olvDuplicates.AllColumns.Add(this.olvDeleted);
            this.olvDuplicates.AllowColumnReorder = true;
            this.olvDuplicates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.olvDuplicates.CellEditUseWholeCell = false;
            this.olvDuplicates.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvName,
            this.olvFiles,
            this.olvNumber,
            this.olvDoublePart,
            this.olvSample,
            this.olvDeleted});
            this.olvDuplicates.ContextMenuStrip = this.rightClickMenu;
            this.olvDuplicates.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvDuplicates.FullRowSelect = true;
            this.olvDuplicates.HideSelection = false;
            this.olvDuplicates.IncludeColumnHeadersInCopy = true;
            this.olvDuplicates.Location = new System.Drawing.Point(6, 29);
            this.olvDuplicates.MultiSelect = false;
            this.olvDuplicates.Name = "olvDuplicates";
            this.olvDuplicates.ShowCommandMenuOnRightClick = true;
            this.olvDuplicates.ShowItemCountOnGroups = true;
            this.olvDuplicates.Size = new System.Drawing.Size(994, 629);
            this.olvDuplicates.TabIndex = 12;
            this.olvDuplicates.UseCompatibleStateImageBehavior = false;
            this.olvDuplicates.UseFilterIndicator = true;
            this.olvDuplicates.UseFiltering = true;
            this.olvDuplicates.View = System.Windows.Forms.View.Details;
            this.olvDuplicates.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.olvDuplicates_CellRightClick);
            // 
            // olvName
            // 
            this.olvName.AspectName = "Name";
            this.olvName.Text = "Movie";
            this.olvName.UseInitialLetterForGroup = true;
            // 
            // olvFiles
            // 
            this.olvFiles.AspectName = "Filenames";
            this.olvFiles.Groupable = false;
            this.olvFiles.Text = "Filenames";
            this.olvFiles.Width = 594;
            // 
            // olvDoublePart
            // 
            this.olvDoublePart.AspectName = "IsDoublePart";
            this.olvDoublePart.Text = "MultiPart Movie";
            this.olvDoublePart.Width = 88;
            // 
            // olvSample
            // 
            this.olvSample.AspectName = "IsSample";
            this.olvSample.Text = "Sample?";
            // 
            // olvDeleted
            // 
            this.olvDeleted.AspectName = "IsDeleted";
            this.olvDeleted.Text = "Deleted?";
            // 
            // olvNumber
            // 
            this.olvNumber.AspectName = "NumberOfFiles";
            this.olvNumber.Text = "# files";
            // 
            // DuplicateMovieFinder
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1012, 690);
            this.Controls.Add(this.olvDuplicates);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "DuplicateMovieFinder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Duplicate Movies";
            this.rightClickMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvDuplicates)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip rightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.ComponentModel.BackgroundWorker bwScan;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Button btnRefresh;
        private BrightIdeasSoftware.ObjectListView olvDuplicates;
        private BrightIdeasSoftware.OLVColumn olvName;
        private BrightIdeasSoftware.OLVColumn olvFiles;
        private BrightIdeasSoftware.OLVColumn olvDoublePart;
        private BrightIdeasSoftware.OLVColumn olvSample;
        private BrightIdeasSoftware.OLVColumn olvDeleted;
        private BrightIdeasSoftware.OLVColumn olvNumber;
    }
}
