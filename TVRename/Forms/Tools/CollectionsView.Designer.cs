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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CollectionsView));
            btnClose = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            rightClickMenu = new System.Windows.Forms.ContextMenuStrip(components);
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            lblStatus = new System.Windows.Forms.Label();
            pbProgress = new System.Windows.Forms.ProgressBar();
            btnRefresh = new System.Windows.Forms.Button();
            olvCollections = new BrightIdeasSoftware.ObjectListView();
            olvCollectionName = new BrightIdeasSoftware.OLVColumn();
            olvMovieName = new BrightIdeasSoftware.OLVColumn();
            olvInLibrary = new BrightIdeasSoftware.OLVColumn();
            olvMovieYear = new BrightIdeasSoftware.OLVColumn();
            chkRemoveCompleted = new System.Windows.Forms.CheckBox();
            chkRemoveFuture = new System.Windows.Forms.CheckBox();
            rightClickMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)olvCollections).BeginInit();
            SuspendLayout();
            // 
            // btnClose
            // 
            btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnClose.Location = new System.Drawing.Point(1079, 766);
            btnClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(88, 27);
            btnClose.TabIndex = 6;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(15, 7);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(52, 15);
            label1.TabIndex = 7;
            label1.Text = "Options:";
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
            lblStatus.Location = new System.Drawing.Point(131, 772);
            lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(0, 15);
            lblStatus.TabIndex = 11;
            lblStatus.Visible = false;
            // 
            // pbProgress
            // 
            pbProgress.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            pbProgress.Location = new System.Drawing.Point(7, 766);
            pbProgress.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pbProgress.Name = "pbProgress";
            pbProgress.Size = new System.Drawing.Size(117, 27);
            pbProgress.TabIndex = 10;
            pbProgress.Visible = false;
            // 
            // btnRefresh
            // 
            btnRefresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnRefresh.Location = new System.Drawing.Point(7, 766);
            btnRefresh.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new System.Drawing.Size(88, 27);
            btnRefresh.TabIndex = 9;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += BtnRefresh_Click_1;
            // 
            // olvCollections
            // 
            olvCollections.AllColumns.Add(olvCollectionName);
            olvCollections.AllColumns.Add(olvMovieName);
            olvCollections.AllColumns.Add(olvInLibrary);
            olvCollections.AllColumns.Add(olvMovieYear);
            olvCollections.AllowColumnReorder = true;
            olvCollections.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            olvCollections.CellEditUseWholeCell = false;
            olvCollections.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { olvCollectionName, olvMovieName, olvInLibrary, olvMovieYear });
            olvCollections.ContextMenuStrip = rightClickMenu;
            olvCollections.FullRowSelect = true;
            olvCollections.IncludeColumnHeadersInCopy = true;
            olvCollections.Location = new System.Drawing.Point(7, 33);
            olvCollections.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            olvCollections.MultiSelect = false;
            olvCollections.Name = "olvCollections";
            olvCollections.ShowCommandMenuOnRightClick = true;
            olvCollections.ShowItemCountOnGroups = true;
            olvCollections.Size = new System.Drawing.Size(1159, 725);
            olvCollections.TabIndex = 12;
            olvCollections.UseCompatibleStateImageBehavior = false;
            olvCollections.UseFilterIndicator = true;
            olvCollections.UseFiltering = true;
            olvCollections.View = System.Windows.Forms.View.Details;
            olvCollections.CellRightClick += olvDuplicates_CellRightClick;
            // 
            // olvCollectionName
            // 
            olvCollectionName.AspectName = "CollectionName";
            olvCollectionName.Text = "Collection";
            olvCollectionName.Width = 229;
            // 
            // olvMovieName
            // 
            olvMovieName.AspectName = "MovieName";
            olvMovieName.Groupable = false;
            olvMovieName.Text = "Movie Name";
            olvMovieName.Width = 204;
            // 
            // olvInLibrary
            // 
            olvInLibrary.AspectName = "IsInLibrary";
            olvInLibrary.Text = "In Library?";
            olvInLibrary.Width = 88;
            // 
            // olvMovieYear
            // 
            olvMovieYear.AspectName = "MovieYear";
            olvMovieYear.Text = "Year";
            // 
            // chkRemoveCompleted
            // 
            chkRemoveCompleted.AutoSize = true;
            chkRemoveCompleted.Location = new System.Drawing.Point(91, 7);
            chkRemoveCompleted.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkRemoveCompleted.Name = "chkRemoveCompleted";
            chkRemoveCompleted.Size = new System.Drawing.Size(193, 19);
            chkRemoveCompleted.TabIndex = 13;
            chkRemoveCompleted.Text = "Remove Completed Collections";
            chkRemoveCompleted.UseVisualStyleBackColor = true;
            chkRemoveCompleted.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // chkRemoveFuture
            // 
            chkRemoveFuture.AutoSize = true;
            chkRemoveFuture.Location = new System.Drawing.Point(300, 7);
            chkRemoveFuture.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkRemoveFuture.Name = "chkRemoveFuture";
            chkRemoveFuture.Size = new System.Drawing.Size(147, 19);
            chkRemoveFuture.TabIndex = 14;
            chkRemoveFuture.Text = "Remove Future Movies";
            chkRemoveFuture.UseVisualStyleBackColor = true;
            chkRemoveFuture.CheckedChanged += chkRemoveFuture_CheckedChanged;
            // 
            // CollectionsView
            // 
            AcceptButton = btnClose;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnClose;
            ClientSize = new System.Drawing.Size(1181, 796);
            Controls.Add(chkRemoveFuture);
            Controls.Add(chkRemoveCompleted);
            Controls.Add(olvCollections);
            Controls.Add(lblStatus);
            Controls.Add(pbProgress);
            Controls.Add(btnRefresh);
            Controls.Add(label1);
            Controls.Add(btnClose);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(697, 456);
            Name = "CollectionsView";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Movie Collections";
            FormClosing += CollectionsView_FormClosing;
            rightClickMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)olvCollections).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip rightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Button btnRefresh;
        private BrightIdeasSoftware.ObjectListView olvCollections;
        private BrightIdeasSoftware.OLVColumn olvCollectionName;
        private BrightIdeasSoftware.OLVColumn olvMovieName;
        private BrightIdeasSoftware.OLVColumn olvInLibrary;
        private BrightIdeasSoftware.OLVColumn olvMovieYear;
        private System.Windows.Forms.CheckBox chkRemoveCompleted;
        private System.Windows.Forms.CheckBox chkRemoveFuture;
    }
}
