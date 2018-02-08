namespace TVRename.Windows.Forms
{
    partial class BulkAdd
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
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageFolders = new System.Windows.Forms.TabPage();
            this.listBoxFolders = new System.Windows.Forms.ListBox();
            this.buttonFoldersOpen = new System.Windows.Forms.Button();
            this.labelFolders = new System.Windows.Forms.Label();
            this.buttonFoldersScan = new System.Windows.Forms.Button();
            this.buttonFoldersAdd = new System.Windows.Forms.Button();
            this.buttonFoldersRemove = new System.Windows.Forms.Button();
            this.tabPageResults = new System.Windows.Forms.TabPage();
            this.buttonResultsIgnore = new System.Windows.Forms.Button();
            this.buttonResultsRemove = new System.Windows.Forms.Button();
            this.buttonResultsEdit = new System.Windows.Forms.Button();
            this.buttonResultsId = new System.Windows.Forms.Button();
            this.buttonResultsAdd = new System.Windows.Forms.Button();
            this.listViewResults = new System.Windows.Forms.ListView();
            this.columnHeaderFolder = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderShow = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderFolderStructure = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTvdbCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageListResults = new System.Windows.Forms.ImageList(this.components);
            this.labelResults = new System.Windows.Forms.Label();
            this.tabPageIgnored = new System.Windows.Forms.TabPage();
            this.buttonIgnoredOpen = new System.Windows.Forms.Button();
            this.buttonIgnoredAdd = new System.Windows.Forms.Button();
            this.buttonIgnoredRemove = new System.Windows.Forms.Button();
            this.listBoxIgnored = new System.Windows.Forms.ListBox();
            this.labelIgnore = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.tabControl.SuspendLayout();
            this.tabPageFolders.SuspendLayout();
            this.tabPageResults.SuspendLayout();
            this.tabPageIgnored.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.toolStripContainer.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer.ContentPanel.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPageFolders);
            this.tabControl.Controls.Add(this.tabPageResults);
            this.tabControl.Controls.Add(this.tabPageIgnored);
            this.tabControl.Location = new System.Drawing.Point(-1, 0);
            this.tabControl.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl.Name = "tabControl";
            this.tabControl.Padding = new System.Drawing.Point(0, 0);
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(788, 442);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageFolders
            // 
            this.tabPageFolders.Controls.Add(this.listBoxFolders);
            this.tabPageFolders.Controls.Add(this.buttonFoldersOpen);
            this.tabPageFolders.Controls.Add(this.labelFolders);
            this.tabPageFolders.Controls.Add(this.buttonFoldersScan);
            this.tabPageFolders.Controls.Add(this.buttonFoldersAdd);
            this.tabPageFolders.Controls.Add(this.buttonFoldersRemove);
            this.tabPageFolders.Location = new System.Drawing.Point(4, 22);
            this.tabPageFolders.Name = "tabPageFolders";
            this.tabPageFolders.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFolders.Size = new System.Drawing.Size(780, 416);
            this.tabPageFolders.TabIndex = 0;
            this.tabPageFolders.Text = "Folders";
            this.tabPageFolders.UseVisualStyleBackColor = true;
            // 
            // listBoxFolders
            // 
            this.listBoxFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxFolders.Location = new System.Drawing.Point(9, 39);
            this.listBoxFolders.Name = "listBoxFolders";
            this.listBoxFolders.Size = new System.Drawing.Size(760, 342);
            this.listBoxFolders.TabIndex = 2;
            this.listBoxFolders.SelectedIndexChanged += new System.EventHandler(this.listBoxFolders_SelectedIndexChanged);
            this.listBoxFolders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxFolders_MouseDoubleClick);
            // 
            // buttonFoldersOpen
            // 
            this.buttonFoldersOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonFoldersOpen.Enabled = false;
            this.buttonFoldersOpen.Location = new System.Drawing.Point(170, 387);
            this.buttonFoldersOpen.Name = "buttonFoldersOpen";
            this.buttonFoldersOpen.Size = new System.Drawing.Size(75, 23);
            this.buttonFoldersOpen.TabIndex = 5;
            this.buttonFoldersOpen.Text = "&Open...";
            this.buttonFoldersOpen.UseVisualStyleBackColor = true;
            this.buttonFoldersOpen.Click += new System.EventHandler(this.buttonFoldersOpen_Click);
            // 
            // labelFolders
            // 
            this.labelFolders.AutoSize = true;
            this.labelFolders.Location = new System.Drawing.Point(9, 14);
            this.labelFolders.Name = "labelFolders";
            this.labelFolders.Size = new System.Drawing.Size(455, 13);
            this.labelFolders.TabIndex = 0;
            this.labelFolders.Text = "Select the folders containing your TV shows to scan. Add them below or drag and d" +
    "rop them in.";
            // 
            // buttonFoldersScan
            // 
            this.buttonFoldersScan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFoldersScan.Enabled = false;
            this.buttonFoldersScan.Location = new System.Drawing.Point(694, 387);
            this.buttonFoldersScan.Name = "buttonFoldersScan";
            this.buttonFoldersScan.Size = new System.Drawing.Size(75, 23);
            this.buttonFoldersScan.TabIndex = 1;
            this.buttonFoldersScan.Text = "&Scan";
            this.buttonFoldersScan.UseVisualStyleBackColor = true;
            this.buttonFoldersScan.Click += new System.EventHandler(this.buttonFoldersScan_Click);
            // 
            // buttonFoldersAdd
            // 
            this.buttonFoldersAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonFoldersAdd.Location = new System.Drawing.Point(9, 387);
            this.buttonFoldersAdd.Name = "buttonFoldersAdd";
            this.buttonFoldersAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonFoldersAdd.TabIndex = 3;
            this.buttonFoldersAdd.Text = "&Add...";
            this.buttonFoldersAdd.UseVisualStyleBackColor = true;
            this.buttonFoldersAdd.Click += new System.EventHandler(this.buttonFoldersAdd_Click);
            // 
            // buttonFoldersRemove
            // 
            this.buttonFoldersRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonFoldersRemove.Enabled = false;
            this.buttonFoldersRemove.Location = new System.Drawing.Point(90, 387);
            this.buttonFoldersRemove.Name = "buttonFoldersRemove";
            this.buttonFoldersRemove.Size = new System.Drawing.Size(75, 23);
            this.buttonFoldersRemove.TabIndex = 4;
            this.buttonFoldersRemove.Text = "&Remove";
            this.buttonFoldersRemove.UseVisualStyleBackColor = true;
            this.buttonFoldersRemove.Click += new System.EventHandler(this.buttonFoldersRemove_Click);
            // 
            // tabPageResults
            // 
            this.tabPageResults.Controls.Add(this.buttonResultsIgnore);
            this.tabPageResults.Controls.Add(this.buttonResultsRemove);
            this.tabPageResults.Controls.Add(this.buttonResultsEdit);
            this.tabPageResults.Controls.Add(this.buttonResultsId);
            this.tabPageResults.Controls.Add(this.buttonResultsAdd);
            this.tabPageResults.Controls.Add(this.listViewResults);
            this.tabPageResults.Controls.Add(this.labelResults);
            this.tabPageResults.Location = new System.Drawing.Point(4, 22);
            this.tabPageResults.Name = "tabPageResults";
            this.tabPageResults.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageResults.Size = new System.Drawing.Size(780, 416);
            this.tabPageResults.TabIndex = 1;
            this.tabPageResults.Text = "Results";
            this.tabPageResults.UseVisualStyleBackColor = true;
            // 
            // buttonResultsIgnore
            // 
            this.buttonResultsIgnore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonResultsIgnore.Enabled = false;
            this.buttonResultsIgnore.Location = new System.Drawing.Point(252, 387);
            this.buttonResultsIgnore.Name = "buttonResultsIgnore";
            this.buttonResultsIgnore.Size = new System.Drawing.Size(75, 23);
            this.buttonResultsIgnore.TabIndex = 10;
            this.buttonResultsIgnore.Text = "&Ignore";
            this.buttonResultsIgnore.UseVisualStyleBackColor = true;
            this.buttonResultsIgnore.Click += new System.EventHandler(this.buttonResultsIgnore_Click);
            // 
            // buttonResultsRemove
            // 
            this.buttonResultsRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonResultsRemove.Enabled = false;
            this.buttonResultsRemove.Location = new System.Drawing.Point(171, 387);
            this.buttonResultsRemove.Name = "buttonResultsRemove";
            this.buttonResultsRemove.Size = new System.Drawing.Size(75, 23);
            this.buttonResultsRemove.TabIndex = 9;
            this.buttonResultsRemove.Text = "&Remove";
            this.buttonResultsRemove.UseVisualStyleBackColor = true;
            this.buttonResultsRemove.Click += new System.EventHandler(this.buttonResultsRemove_Click);
            // 
            // buttonResultsEdit
            // 
            this.buttonResultsEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonResultsEdit.Enabled = false;
            this.buttonResultsEdit.Location = new System.Drawing.Point(90, 387);
            this.buttonResultsEdit.Name = "buttonResultsEdit";
            this.buttonResultsEdit.Size = new System.Drawing.Size(75, 23);
            this.buttonResultsEdit.TabIndex = 8;
            this.buttonResultsEdit.Text = "&Edit";
            this.buttonResultsEdit.UseVisualStyleBackColor = true;
            this.buttonResultsEdit.Click += new System.EventHandler(this.buttonResultsEdit_Click);
            // 
            // buttonResultsId
            // 
            this.buttonResultsId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonResultsId.Location = new System.Drawing.Point(9, 387);
            this.buttonResultsId.Name = "buttonResultsId";
            this.buttonResultsId.Size = new System.Drawing.Size(75, 23);
            this.buttonResultsId.TabIndex = 7;
            this.buttonResultsId.Text = "Auto &ID All";
            this.buttonResultsId.UseVisualStyleBackColor = true;
            this.buttonResultsId.Click += new System.EventHandler(this.buttonResultsId_Click);
            // 
            // buttonResultsAdd
            // 
            this.buttonResultsAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonResultsAdd.Enabled = false;
            this.buttonResultsAdd.Location = new System.Drawing.Point(694, 387);
            this.buttonResultsAdd.Name = "buttonResultsAdd";
            this.buttonResultsAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonResultsAdd.TabIndex = 6;
            this.buttonResultsAdd.Text = "&Add Shows";
            this.buttonResultsAdd.UseVisualStyleBackColor = true;
            this.buttonResultsAdd.Click += new System.EventHandler(this.buttonResultsAdd_Click);
            // 
            // listViewResults
            // 
            this.listViewResults.AllowDrop = true;
            this.listViewResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderFolder,
            this.columnHeaderShow,
            this.columnHeaderFolderStructure,
            this.columnHeaderTvdbCode});
            this.listViewResults.FullRowSelect = true;
            this.listViewResults.HideSelection = false;
            this.listViewResults.Location = new System.Drawing.Point(9, 39);
            this.listViewResults.Name = "listViewResults";
            this.listViewResults.Size = new System.Drawing.Size(760, 342);
            this.listViewResults.SmallImageList = this.imageListResults;
            this.listViewResults.TabIndex = 5;
            this.listViewResults.UseCompatibleStateImageBehavior = false;
            this.listViewResults.View = System.Windows.Forms.View.Details;
            this.listViewResults.SelectedIndexChanged += new System.EventHandler(this.listViewResults_SelectedIndexChanged);
            this.listViewResults.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewResults_MouseDoubleClick);
            // 
            // columnHeaderFolder
            // 
            this.columnHeaderFolder.Text = "Folder";
            this.columnHeaderFolder.Width = 262;
            // 
            // columnHeaderShow
            // 
            this.columnHeaderShow.Text = "Show";
            this.columnHeaderShow.Width = 271;
            // 
            // columnHeaderFolderStructure
            // 
            this.columnHeaderFolderStructure.Text = "Folder Structure";
            this.columnHeaderFolderStructure.Width = 105;
            // 
            // columnHeaderTvdbCode
            // 
            this.columnHeaderTvdbCode.Text = "TheTVDB Code";
            this.columnHeaderTvdbCode.Width = 92;
            // 
            // imageListResults
            // 
            this.imageListResults.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListResults.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListResults.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // labelResults
            // 
            this.labelResults.AutoSize = true;
            this.labelResults.Location = new System.Drawing.Point(9, 12);
            this.labelResults.Name = "labelResults";
            this.labelResults.Size = new System.Drawing.Size(428, 13);
            this.labelResults.TabIndex = 3;
            this.labelResults.Text = "These are the detected TV show folders. In order to add a show its ID needs to be" +
    " found.";
            // 
            // tabPageIgnored
            // 
            this.tabPageIgnored.Controls.Add(this.buttonIgnoredOpen);
            this.tabPageIgnored.Controls.Add(this.buttonIgnoredAdd);
            this.tabPageIgnored.Controls.Add(this.buttonIgnoredRemove);
            this.tabPageIgnored.Controls.Add(this.listBoxIgnored);
            this.tabPageIgnored.Controls.Add(this.labelIgnore);
            this.tabPageIgnored.Location = new System.Drawing.Point(4, 22);
            this.tabPageIgnored.Name = "tabPageIgnored";
            this.tabPageIgnored.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageIgnored.Size = new System.Drawing.Size(780, 416);
            this.tabPageIgnored.TabIndex = 2;
            this.tabPageIgnored.Text = "Ignored";
            this.tabPageIgnored.UseVisualStyleBackColor = true;
            // 
            // buttonIgnoredOpen
            // 
            this.buttonIgnoredOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonIgnoredOpen.Enabled = false;
            this.buttonIgnoredOpen.Location = new System.Drawing.Point(171, 387);
            this.buttonIgnoredOpen.Name = "buttonIgnoredOpen";
            this.buttonIgnoredOpen.Size = new System.Drawing.Size(75, 23);
            this.buttonIgnoredOpen.TabIndex = 8;
            this.buttonIgnoredOpen.Text = "&Open...";
            this.buttonIgnoredOpen.UseVisualStyleBackColor = true;
            this.buttonIgnoredOpen.Click += new System.EventHandler(this.buttonIgnoredOpen_Click);
            // 
            // buttonIgnoredAdd
            // 
            this.buttonIgnoredAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonIgnoredAdd.Location = new System.Drawing.Point(9, 387);
            this.buttonIgnoredAdd.Name = "buttonIgnoredAdd";
            this.buttonIgnoredAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonIgnoredAdd.TabIndex = 6;
            this.buttonIgnoredAdd.Text = "&Add...";
            this.buttonIgnoredAdd.UseVisualStyleBackColor = true;
            this.buttonIgnoredAdd.Click += new System.EventHandler(this.buttonIgnoredAdd_Click);
            // 
            // buttonIgnoredRemove
            // 
            this.buttonIgnoredRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonIgnoredRemove.Enabled = false;
            this.buttonIgnoredRemove.Location = new System.Drawing.Point(90, 387);
            this.buttonIgnoredRemove.Name = "buttonIgnoredRemove";
            this.buttonIgnoredRemove.Size = new System.Drawing.Size(75, 23);
            this.buttonIgnoredRemove.TabIndex = 7;
            this.buttonIgnoredRemove.Text = "&Remove";
            this.buttonIgnoredRemove.UseVisualStyleBackColor = true;
            this.buttonIgnoredRemove.Click += new System.EventHandler(this.buttonIgnoredRemove_Click);
            // 
            // listBoxIgnored
            // 
            this.listBoxIgnored.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxIgnored.Location = new System.Drawing.Point(9, 39);
            this.listBoxIgnored.Name = "listBoxIgnored";
            this.listBoxIgnored.Size = new System.Drawing.Size(760, 342);
            this.listBoxIgnored.TabIndex = 4;
            this.listBoxIgnored.SelectedIndexChanged += new System.EventHandler(this.listBoxIgnored_SelectedIndexChanged);
            this.listBoxIgnored.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxIgnored_MouseDoubleClick);
            // 
            // labelIgnore
            // 
            this.labelIgnore.AutoSize = true;
            this.labelIgnore.Location = new System.Drawing.Point(9, 12);
            this.labelIgnore.Name = "labelIgnore";
            this.labelIgnore.Size = new System.Drawing.Size(410, 13);
            this.labelIgnore.TabIndex = 3;
            this.labelIgnore.Text = "These directories will be ignored from the scan results and not checked for TV sh" +
    "ows.";
            // 
            // statusStrip
            // 
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.toolStripProgressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 0);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(784, 22);
            this.statusStrip.TabIndex = 6;
            this.statusStrip.Text = "statusStrip";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(567, 17);
            this.toolStripStatusLabel.Spring = true;
            this.toolStripStatusLabel.Text = "Ready";
            this.toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(200, 16);
            this.toolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // toolStripContainer
            // 
            // 
            // toolStripContainer.BottomToolStripPanel
            // 
            this.toolStripContainer.BottomToolStripPanel.Controls.Add(this.statusStrip);
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.Controls.Add(this.tabControl);
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(784, 439);
            this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer.LeftToolStripPanelVisible = false;
            this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.RightToolStripPanelVisible = false;
            this.toolStripContainer.Size = new System.Drawing.Size(784, 461);
            this.toolStripContainer.TabIndex = 7;
            this.toolStripContainer.Text = "toolStripContainer";
            this.toolStripContainer.TopToolStripPanelVisible = false;
            // 
            // BulkAdd
            // 
            this.AcceptButton = this.buttonFoldersScan;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.toolStripContainer);
            this.MinimizeBox = false;
            this.Name = "BulkAdd";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Bulk Add Shows";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BulkAdd_FormClosing);
            this.Load += new System.EventHandler(this.BulkAdd_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPageFolders.ResumeLayout(false);
            this.tabPageFolders.PerformLayout();
            this.tabPageResults.ResumeLayout(false);
            this.tabPageResults.PerformLayout();
            this.tabPageIgnored.ResumeLayout(false);
            this.tabPageIgnored.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.toolStripContainer.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer.ContentPanel.ResumeLayout(false);
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageFolders;
        private System.Windows.Forms.TabPage tabPageResults;
        private System.Windows.Forms.TabPage tabPageIgnored;
        private System.Windows.Forms.Label labelFolders;
        private System.Windows.Forms.ListBox listBoxFolders;
        private System.Windows.Forms.Button buttonFoldersScan;
        private System.Windows.Forms.Button buttonFoldersAdd;
        private System.Windows.Forms.Button buttonFoldersRemove;
        private System.Windows.Forms.Button buttonFoldersOpen;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.Label labelResults;
        private System.Windows.Forms.ListBox listBoxIgnored;
        private System.Windows.Forms.Label labelIgnore;
        private System.Windows.Forms.ListView listViewResults;
        private System.Windows.Forms.ColumnHeader columnHeaderFolder;
        private System.Windows.Forms.ColumnHeader columnHeaderShow;
        private System.Windows.Forms.ColumnHeader columnHeaderFolderStructure;
        private System.Windows.Forms.ColumnHeader columnHeaderTvdbCode;
        private System.Windows.Forms.ImageList imageListResults;
        private System.Windows.Forms.ToolStripContainer toolStripContainer;
        private System.Windows.Forms.Button buttonResultsAdd;
        private System.Windows.Forms.Button buttonIgnoredOpen;
        private System.Windows.Forms.Button buttonIgnoredAdd;
        private System.Windows.Forms.Button buttonIgnoredRemove;
        private System.Windows.Forms.Button buttonResultsId;
        private System.Windows.Forms.Button buttonResultsIgnore;
        private System.Windows.Forms.Button buttonResultsRemove;
        private System.Windows.Forms.Button buttonResultsEdit;
    }
}
