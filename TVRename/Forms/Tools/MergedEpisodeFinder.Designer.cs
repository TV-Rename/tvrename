namespace TVRename.Forms
{
    partial class MergedEpisodeFinder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergedEpisodeFinder));
            this.lvMergedEpisodes = new TVRename.ListViewFlickerFree();
            this.show = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.season = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.episodes = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.airDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.episodeNames = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chkAirDateTest = new System.Windows.Forms.CheckBox();
            this.chkNameTest = new System.Windows.Forms.CheckBox();
            this.chkMIssingTest = new System.Windows.Forms.CheckBox();
            this.chkFilesizeTest = new System.Windows.Forms.CheckBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.rightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.bwScan = new System.ComponentModel.BackgroundWorker();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.rightClickMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvMergedEpisodes
            // 
            this.lvMergedEpisodes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvMergedEpisodes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.show,
            this.season,
            this.episodes,
            this.airDate,
            this.episodeNames,
            this.name});
            this.lvMergedEpisodes.FullRowSelect = true;
            this.lvMergedEpisodes.HideSelection = false;
            this.lvMergedEpisodes.Location = new System.Drawing.Point(6, 36);
            this.lvMergedEpisodes.MultiSelect = false;
            this.lvMergedEpisodes.Name = "lvMergedEpisodes";
            this.lvMergedEpisodes.ShowGroups = false;
            this.lvMergedEpisodes.Size = new System.Drawing.Size(696, 352);
            this.lvMergedEpisodes.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvMergedEpisodes.TabIndex = 0;
            this.lvMergedEpisodes.UseCompatibleStateImageBehavior = false;
            this.lvMergedEpisodes.View = System.Windows.Forms.View.Details;
            this.lvMergedEpisodes.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvDuplicates_MouseClick);
            // 
            // show
            // 
            this.show.Text = "Show Name";
            this.show.Width = 125;
            // 
            // season
            // 
            this.season.Text = "Season";
            // 
            // episodes
            // 
            this.episodes.Text = "Episodes";
            // 
            // airDate
            // 
            this.airDate.Text = "Air Date";
            // 
            // episodeNames
            // 
            this.episodeNames.Text = "Episode Names";
            this.episodeNames.Width = 211;
            // 
            // name
            // 
            this.name.Text = "Combined Name";
            this.name.Width = 168;
            // 
            // chkAirDateTest
            // 
            this.chkAirDateTest.AutoSize = true;
            this.chkAirDateTest.Checked = true;
            this.chkAirDateTest.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAirDateTest.Location = new System.Drawing.Point(113, 13);
            this.chkAirDateTest.Name = "chkAirDateTest";
            this.chkAirDateTest.Size = new System.Drawing.Size(120, 17);
            this.chkAirDateTest.TabIndex = 1;
            this.chkAirDateTest.Text = "Air Date is the same";
            this.chkAirDateTest.UseVisualStyleBackColor = true;
            this.chkAirDateTest.CheckedChanged += new System.EventHandler(this.chkAirDateTest_CheckedChanged);
            // 
            // chkNameTest
            // 
            this.chkNameTest.AutoSize = true;
            this.chkNameTest.Checked = true;
            this.chkNameTest.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNameTest.Location = new System.Drawing.Point(236, 13);
            this.chkNameTest.Name = "chkNameTest";
            this.chkNameTest.Size = new System.Drawing.Size(150, 17);
            this.chkNameTest.TabIndex = 2;
            this.chkNameTest.Text = "Names have common root";
            this.chkNameTest.UseVisualStyleBackColor = true;
            this.chkNameTest.CheckedChanged += new System.EventHandler(this.chkNameTest_CheckedChanged);
            // 
            // chkMIssingTest
            // 
            this.chkMIssingTest.AutoSize = true;
            this.chkMIssingTest.Checked = true;
            this.chkMIssingTest.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMIssingTest.Location = new System.Drawing.Point(392, 13);
            this.chkMIssingTest.Name = "chkMIssingTest";
            this.chkMIssingTest.Size = new System.Drawing.Size(93, 17);
            this.chkMIssingTest.TabIndex = 3;
            this.chkMIssingTest.Text = "One is missing";
            this.chkMIssingTest.UseVisualStyleBackColor = true;
            this.chkMIssingTest.CheckedChanged += new System.EventHandler(this.chkMissingTest_CheckedChanged);
            // 
            // chkFilesizeTest
            // 
            this.chkFilesizeTest.AutoSize = true;
            this.chkFilesizeTest.Checked = true;
            this.chkFilesizeTest.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFilesizeTest.Location = new System.Drawing.Point(489, 13);
            this.chkFilesizeTest.Name = "chkFilesizeTest";
            this.chkFilesizeTest.Size = new System.Drawing.Size(63, 17);
            this.chkFilesizeTest.TabIndex = 4;
            this.chkFilesizeTest.Text = "File size";
            this.chkFilesizeTest.UseVisualStyleBackColor = true;
            this.chkFilesizeTest.CheckedChanged += new System.EventHandler(this.chkFilesizeTest_CheckedChanged);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(633, 394);
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
            this.lblStatus.Location = new System.Drawing.Point(112, 399);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 11;
            this.lblStatus.Visible = false;
            // 
            // pbProgress
            // 
            this.pbProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pbProgress.Location = new System.Drawing.Point(6, 394);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(100, 23);
            this.pbProgress.TabIndex = 10;
            this.pbProgress.Visible = false;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRefresh.Location = new System.Drawing.Point(6, 394);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 9;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click_1);
            // 
            // MergedEpisodeFinder
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(708, 420);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.chkFilesizeTest);
            this.Controls.Add(this.chkMIssingTest);
            this.Controls.Add(this.chkNameTest);
            this.Controls.Add(this.chkAirDateTest);
            this.Controls.Add(this.lvMergedEpisodes);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "MergedEpisodeFinder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Merged Episode Finder";
            this.rightClickMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListViewFlickerFree lvMergedEpisodes;
        private System.Windows.Forms.CheckBox chkAirDateTest;
        private System.Windows.Forms.CheckBox chkNameTest;
        private System.Windows.Forms.CheckBox chkMIssingTest;
        private System.Windows.Forms.CheckBox chkFilesizeTest;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader show;
        private System.Windows.Forms.ColumnHeader season;
        private System.Windows.Forms.ColumnHeader episodes;
        private System.Windows.Forms.ColumnHeader airDate;
        private System.Windows.Forms.ColumnHeader episodeNames;
        private System.Windows.Forms.ColumnHeader name;
        private System.Windows.Forms.ContextMenuStrip rightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.ComponentModel.BackgroundWorker bwScan;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Button btnRefresh;
    }
}
