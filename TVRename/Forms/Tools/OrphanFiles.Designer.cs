namespace TVRename.Forms.Tools
{
    partial class OrphanFiles
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
            this.olvFileIssues = new ObjectListViewFlickerFree();
            this.olvShow = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvFileName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvMessage = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvFileDirectory = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvSeason = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvEpisodeNumber = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.showRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.bwRescan = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.olvFileIssues)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // olvFileIssues
            // 
            this.olvFileIssues.AllColumns.Add(this.olvShow);
            this.olvFileIssues.AllColumns.Add(this.olvFileName);
            this.olvFileIssues.AllColumns.Add(this.olvMessage);
            this.olvFileIssues.AllColumns.Add(this.olvFileDirectory);
            this.olvFileIssues.AllColumns.Add(this.olvSeason);
            this.olvFileIssues.AllColumns.Add(this.olvEpisodeNumber);
            this.olvFileIssues.AllowColumnReorder = true;
            this.olvFileIssues.CellEditUseWholeCell = false;
            this.olvFileIssues.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvShow,
            this.olvFileName,
            this.olvMessage,
            this.olvFileDirectory,
            this.olvSeason,
            this.olvEpisodeNumber});
            this.olvFileIssues.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvFileIssues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvFileIssues.FullRowSelect = true;
            this.olvFileIssues.HideSelection = false;
            this.olvFileIssues.Location = new System.Drawing.Point(3, 3);
            this.olvFileIssues.MultiSelect = false;
            this.olvFileIssues.Name = "olvFileIssues";
            this.olvFileIssues.ShowCommandMenuOnRightClick = true;
            this.olvFileIssues.ShowItemCountOnGroups = true;
            this.olvFileIssues.Size = new System.Drawing.Size(844, 580);
            this.olvFileIssues.TabIndex = 0;
            this.olvFileIssues.UseCompatibleStateImageBehavior = false;
            this.olvFileIssues.UseFilterIndicator = true;
            this.olvFileIssues.UseFiltering = true;
            this.olvFileIssues.View = System.Windows.Forms.View.Details;
            this.olvFileIssues.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OlvFileIssues_MouseClick);
            // 
            // olvShow
            // 
            this.olvShow.AspectName = "Showname";
            this.olvShow.Text = "Show";
            this.olvShow.Width = 208;
            // 
            // olvFileName
            // 
            this.olvFileName.AspectName = "Filename";
            this.olvFileName.Text = "File";
            this.olvFileName.Width = 145;
            // 
            // olvMessage
            // 
            this.olvMessage.AspectName = "Message";
            this.olvMessage.Text = "Message";
            this.olvMessage.Width = 104;
            // 
            // olvFileDirectory
            // 
            this.olvFileDirectory.AspectName = "Directory";
            this.olvFileDirectory.Text = "Directory";
            this.olvFileDirectory.Width = 258;
            // 
            // olvSeason
            // 
            this.olvSeason.AspectName = "SeasonNumber";
            this.olvSeason.Text = "Season";
            // 
            // olvEpisodeNumber
            // 
            this.olvEpisodeNumber.AspectName = "EpisodeNumber";
            this.olvEpisodeNumber.Text = "Episode";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.olvFileIssues, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(850, 626);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblStatus);
            this.panel1.Controls.Add(this.pbProgress);
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 589);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(844, 34);
            this.panel1.TabIndex = 6;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(115, 8);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Visible = false;
            // 
            // pbProgress
            // 
            this.pbProgress.Location = new System.Drawing.Point(9, 3);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(100, 23);
            this.pbProgress.TabIndex = 6;
            this.pbProgress.Visible = false;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(9, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(766, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // showRightClickMenu
            // 
            this.showRightClickMenu.Name = "menuSearchSites";
            this.showRightClickMenu.ShowImageMargin = false;
            this.showRightClickMenu.Size = new System.Drawing.Size(36, 4);
            // 
            // bwRescan
            // 
            this.bwRescan.WorkerReportsProgress = true;
            this.bwRescan.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BwRescan_DoWork);
            this.bwRescan.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BwRescan_ProgressChanged);
            this.bwRescan.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BwRescan_RunWorkerCompleted);
            // 
            // OrphanFiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button1;
            this.ClientSize = new System.Drawing.Size(850, 626);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimizeBox = false;
            this.Name = "OrphanFiles";
            this.ShowIcon = false;
            this.Text = "Orphan Media Files";
            ((System.ComponentModel.ISupportInitialize)(this.olvFileIssues)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView olvFileIssues;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private BrightIdeasSoftware.OLVColumn olvShow;
        private BrightIdeasSoftware.OLVColumn olvFileName;
        private BrightIdeasSoftware.OLVColumn olvFileDirectory;
        private BrightIdeasSoftware.OLVColumn olvSeason;
        private BrightIdeasSoftware.OLVColumn olvEpisodeNumber;
        private System.Windows.Forms.ContextMenuStrip showRightClickMenu;
        private BrightIdeasSoftware.OLVColumn olvMessage;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button button1;
        private System.ComponentModel.BackgroundWorker bwRescan;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar pbProgress;
    }
}
