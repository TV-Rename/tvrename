namespace TVRename.Windows.Forms
{
    partial class Statistics
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
            this.buttonClose = new System.Windows.Forms.Button();
            this.groupBoxFiles = new System.Windows.Forms.GroupBox();
            this.labelFilesRenamed = new System.Windows.Forms.Label();
            this.labelFilesCopied = new System.Windows.Forms.Label();
            this.labelFilesCopiedTitle = new System.Windows.Forms.Label();
            this.labelFilesRenamedTitle = new System.Windows.Forms.Label();
            this.labelFilesMoved = new System.Windows.Forms.Label();
            this.labelFilesMovedTitle = new System.Windows.Forms.Label();
            this.groupBoxChecks = new System.Windows.Forms.GroupBox();
            this.labelChecksMissing = new System.Windows.Forms.Label();
            this.labelChecksMissingTitle = new System.Windows.Forms.Label();
            this.labelChecksRename = new System.Windows.Forms.Label();
            this.labelChecksRenameTitle = new System.Windows.Forms.Label();
            this.groupBoxTotals = new System.Windows.Forms.GroupBox();
            this.labelTotalSeasons = new System.Windows.Forms.Label();
            this.labelTotalEpisodes = new System.Windows.Forms.Label();
            this.labelTotalEpisodesTitle = new System.Windows.Forms.Label();
            this.labelTotalSeasonsTitle = new System.Windows.Forms.Label();
            this.labelTotalShows = new System.Windows.Forms.Label();
            this.labelTotalShowsTitle = new System.Windows.Forms.Label();
            this.labelTotalLocalEpisodes = new System.Windows.Forms.Label();
            this.labelTotalLocalEpisodesTitle = new System.Windows.Forms.Label();
            this.groupBoxFiles.SuspendLayout();
            this.groupBoxChecks.SuspendLayout();
            this.groupBoxTotals.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(117, 279);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Text = "&Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // groupBoxFiles
            // 
            this.groupBoxFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxFiles.Controls.Add(this.labelFilesRenamed);
            this.groupBoxFiles.Controls.Add(this.labelFilesCopied);
            this.groupBoxFiles.Controls.Add(this.labelFilesCopiedTitle);
            this.groupBoxFiles.Controls.Add(this.labelFilesRenamedTitle);
            this.groupBoxFiles.Controls.Add(this.labelFilesMoved);
            this.groupBoxFiles.Controls.Add(this.labelFilesMovedTitle);
            this.groupBoxFiles.Location = new System.Drawing.Point(12, 12);
            this.groupBoxFiles.Name = "groupBoxFiles";
            this.groupBoxFiles.Size = new System.Drawing.Size(180, 82);
            this.groupBoxFiles.TabIndex = 25;
            this.groupBoxFiles.TabStop = false;
            this.groupBoxFiles.Text = "Files";
            // 
            // labelFilesRenamed
            // 
            this.labelFilesRenamed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFilesRenamed.Location = new System.Drawing.Point(120, 39);
            this.labelFilesRenamed.Name = "labelFilesRenamed";
            this.labelFilesRenamed.Size = new System.Drawing.Size(50, 13);
            this.labelFilesRenamed.TabIndex = 30;
            this.labelFilesRenamed.Text = "-";
            this.labelFilesRenamed.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelFilesCopied
            // 
            this.labelFilesCopied.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFilesCopied.Location = new System.Drawing.Point(120, 59);
            this.labelFilesCopied.Name = "labelFilesCopied";
            this.labelFilesCopied.Size = new System.Drawing.Size(50, 13);
            this.labelFilesCopied.TabIndex = 29;
            this.labelFilesCopied.Text = "-";
            this.labelFilesCopied.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelFilesCopiedTitle
            // 
            this.labelFilesCopiedTitle.AutoSize = true;
            this.labelFilesCopiedTitle.Location = new System.Drawing.Point(10, 59);
            this.labelFilesCopiedTitle.Name = "labelFilesCopiedTitle";
            this.labelFilesCopiedTitle.Size = new System.Drawing.Size(66, 13);
            this.labelFilesCopiedTitle.TabIndex = 28;
            this.labelFilesCopiedTitle.Text = "Files copied:";
            // 
            // labelFilesRenamedTitle
            // 
            this.labelFilesRenamedTitle.AutoSize = true;
            this.labelFilesRenamedTitle.Location = new System.Drawing.Point(10, 39);
            this.labelFilesRenamedTitle.Name = "labelFilesRenamedTitle";
            this.labelFilesRenamedTitle.Size = new System.Drawing.Size(75, 13);
            this.labelFilesRenamedTitle.TabIndex = 27;
            this.labelFilesRenamedTitle.Text = "Files renamed:";
            // 
            // labelFilesMoved
            // 
            this.labelFilesMoved.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFilesMoved.Location = new System.Drawing.Point(120, 19);
            this.labelFilesMoved.Name = "labelFilesMoved";
            this.labelFilesMoved.Size = new System.Drawing.Size(50, 13);
            this.labelFilesMoved.TabIndex = 26;
            this.labelFilesMoved.Text = "-";
            this.labelFilesMoved.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelFilesMovedTitle
            // 
            this.labelFilesMovedTitle.AutoSize = true;
            this.labelFilesMovedTitle.Location = new System.Drawing.Point(10, 19);
            this.labelFilesMovedTitle.Name = "labelFilesMovedTitle";
            this.labelFilesMovedTitle.Size = new System.Drawing.Size(66, 13);
            this.labelFilesMovedTitle.TabIndex = 25;
            this.labelFilesMovedTitle.Text = "Files moved:";
            // 
            // groupBoxChecks
            // 
            this.groupBoxChecks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxChecks.Controls.Add(this.labelChecksMissing);
            this.groupBoxChecks.Controls.Add(this.labelChecksMissingTitle);
            this.groupBoxChecks.Controls.Add(this.labelChecksRename);
            this.groupBoxChecks.Controls.Add(this.labelChecksRenameTitle);
            this.groupBoxChecks.Location = new System.Drawing.Point(12, 100);
            this.groupBoxChecks.Name = "groupBoxChecks";
            this.groupBoxChecks.Size = new System.Drawing.Size(180, 65);
            this.groupBoxChecks.TabIndex = 26;
            this.groupBoxChecks.TabStop = false;
            this.groupBoxChecks.Text = "Checks";
            // 
            // labelChecksMissing
            // 
            this.labelChecksMissing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelChecksMissing.Location = new System.Drawing.Point(120, 39);
            this.labelChecksMissing.Name = "labelChecksMissing";
            this.labelChecksMissing.Size = new System.Drawing.Size(50, 13);
            this.labelChecksMissing.TabIndex = 30;
            this.labelChecksMissing.Text = "-";
            this.labelChecksMissing.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelChecksMissingTitle
            // 
            this.labelChecksMissingTitle.AutoSize = true;
            this.labelChecksMissingTitle.Location = new System.Drawing.Point(10, 39);
            this.labelChecksMissingTitle.Name = "labelChecksMissingTitle";
            this.labelChecksMissingTitle.Size = new System.Drawing.Size(110, 13);
            this.labelChecksMissingTitle.TabIndex = 27;
            this.labelChecksMissingTitle.Text = "Missing checks done:";
            // 
            // labelChecksRename
            // 
            this.labelChecksRename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelChecksRename.Location = new System.Drawing.Point(120, 19);
            this.labelChecksRename.Name = "labelChecksRename";
            this.labelChecksRename.Size = new System.Drawing.Size(50, 13);
            this.labelChecksRename.TabIndex = 26;
            this.labelChecksRename.Text = "-";
            this.labelChecksRename.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelChecksRenameTitle
            // 
            this.labelChecksRenameTitle.AutoSize = true;
            this.labelChecksRenameTitle.Location = new System.Drawing.Point(10, 19);
            this.labelChecksRenameTitle.Name = "labelChecksRenameTitle";
            this.labelChecksRenameTitle.Size = new System.Drawing.Size(115, 13);
            this.labelChecksRenameTitle.TabIndex = 25;
            this.labelChecksRenameTitle.Text = "Rename checks done:";
            // 
            // groupBoxTotals
            // 
            this.groupBoxTotals.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxTotals.Controls.Add(this.labelTotalLocalEpisodes);
            this.groupBoxTotals.Controls.Add(this.labelTotalLocalEpisodesTitle);
            this.groupBoxTotals.Controls.Add(this.labelTotalSeasons);
            this.groupBoxTotals.Controls.Add(this.labelTotalEpisodes);
            this.groupBoxTotals.Controls.Add(this.labelTotalEpisodesTitle);
            this.groupBoxTotals.Controls.Add(this.labelTotalSeasonsTitle);
            this.groupBoxTotals.Controls.Add(this.labelTotalShows);
            this.groupBoxTotals.Controls.Add(this.labelTotalShowsTitle);
            this.groupBoxTotals.Location = new System.Drawing.Point(12, 171);
            this.groupBoxTotals.Name = "groupBoxTotals";
            this.groupBoxTotals.Size = new System.Drawing.Size(180, 102);
            this.groupBoxTotals.TabIndex = 27;
            this.groupBoxTotals.TabStop = false;
            this.groupBoxTotals.Text = "Totals";
            // 
            // labelTotalSeasons
            // 
            this.labelTotalSeasons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTotalSeasons.Location = new System.Drawing.Point(120, 39);
            this.labelTotalSeasons.Name = "labelTotalSeasons";
            this.labelTotalSeasons.Size = new System.Drawing.Size(50, 13);
            this.labelTotalSeasons.TabIndex = 30;
            this.labelTotalSeasons.Text = "-";
            this.labelTotalSeasons.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelTotalEpisodes
            // 
            this.labelTotalEpisodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTotalEpisodes.Location = new System.Drawing.Point(120, 59);
            this.labelTotalEpisodes.Name = "labelTotalEpisodes";
            this.labelTotalEpisodes.Size = new System.Drawing.Size(50, 13);
            this.labelTotalEpisodes.TabIndex = 29;
            this.labelTotalEpisodes.Text = "-";
            this.labelTotalEpisodes.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelTotalEpisodesTitle
            // 
            this.labelTotalEpisodesTitle.AutoSize = true;
            this.labelTotalEpisodesTitle.Location = new System.Drawing.Point(10, 59);
            this.labelTotalEpisodesTitle.Name = "labelTotalEpisodesTitle";
            this.labelTotalEpisodesTitle.Size = new System.Drawing.Size(104, 13);
            this.labelTotalEpisodesTitle.TabIndex = 28;
            this.labelTotalEpisodesTitle.Text = "Number of episodes:";
            // 
            // labelTotalSeasonsTitle
            // 
            this.labelTotalSeasonsTitle.AutoSize = true;
            this.labelTotalSeasonsTitle.Location = new System.Drawing.Point(10, 39);
            this.labelTotalSeasonsTitle.Name = "labelTotalSeasonsTitle";
            this.labelTotalSeasonsTitle.Size = new System.Drawing.Size(101, 13);
            this.labelTotalSeasonsTitle.TabIndex = 27;
            this.labelTotalSeasonsTitle.Text = "Number of seasons:";
            // 
            // labelTotalShows
            // 
            this.labelTotalShows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTotalShows.Location = new System.Drawing.Point(120, 19);
            this.labelTotalShows.Name = "labelTotalShows";
            this.labelTotalShows.Size = new System.Drawing.Size(50, 13);
            this.labelTotalShows.TabIndex = 26;
            this.labelTotalShows.Text = "-";
            this.labelTotalShows.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelTotalShowsTitle
            // 
            this.labelTotalShowsTitle.AutoSize = true;
            this.labelTotalShowsTitle.Location = new System.Drawing.Point(10, 19);
            this.labelTotalShowsTitle.Name = "labelTotalShowsTitle";
            this.labelTotalShowsTitle.Size = new System.Drawing.Size(92, 13);
            this.labelTotalShowsTitle.TabIndex = 25;
            this.labelTotalShowsTitle.Text = "Number of shows:";
            // 
            // labelTotalLocalEpisodes
            // 
            this.labelTotalLocalEpisodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTotalLocalEpisodes.Location = new System.Drawing.Point(120, 79);
            this.labelTotalLocalEpisodes.Name = "labelTotalLocalEpisodes";
            this.labelTotalLocalEpisodes.Size = new System.Drawing.Size(50, 13);
            this.labelTotalLocalEpisodes.TabIndex = 32;
            this.labelTotalLocalEpisodes.Text = "-";
            this.labelTotalLocalEpisodes.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelTotalLocalEpisodesTitle
            // 
            this.labelTotalLocalEpisodesTitle.AutoSize = true;
            this.labelTotalLocalEpisodesTitle.Location = new System.Drawing.Point(10, 79);
            this.labelTotalLocalEpisodesTitle.Name = "labelTotalLocalEpisodesTitle";
            this.labelTotalLocalEpisodesTitle.Size = new System.Drawing.Size(90, 13);
            this.labelTotalLocalEpisodesTitle.TabIndex = 31;
            this.labelTotalLocalEpisodesTitle.Text = "Episodes on disk:";
            // 
            // Statistics
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(204, 314);
            this.Controls.Add(this.groupBoxTotals);
            this.Controls.Add(this.groupBoxChecks);
            this.Controls.Add(this.groupBoxFiles);
            this.Controls.Add(this.buttonClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Statistics";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Statistics";
            this.groupBoxFiles.ResumeLayout(false);
            this.groupBoxFiles.PerformLayout();
            this.groupBoxChecks.ResumeLayout(false);
            this.groupBoxChecks.PerformLayout();
            this.groupBoxTotals.ResumeLayout(false);
            this.groupBoxTotals.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.GroupBox groupBoxFiles;
        private System.Windows.Forms.Label labelFilesRenamed;
        private System.Windows.Forms.Label labelFilesCopied;
        private System.Windows.Forms.Label labelFilesCopiedTitle;
        private System.Windows.Forms.Label labelFilesRenamedTitle;
        private System.Windows.Forms.Label labelFilesMoved;
        private System.Windows.Forms.Label labelFilesMovedTitle;
        private System.Windows.Forms.GroupBox groupBoxChecks;
        private System.Windows.Forms.Label labelChecksMissing;
        private System.Windows.Forms.Label labelChecksMissingTitle;
        private System.Windows.Forms.Label labelChecksRename;
        private System.Windows.Forms.Label labelChecksRenameTitle;
        private System.Windows.Forms.GroupBox groupBoxTotals;
        private System.Windows.Forms.Label labelTotalSeasons;
        private System.Windows.Forms.Label labelTotalEpisodes;
        private System.Windows.Forms.Label labelTotalEpisodesTitle;
        private System.Windows.Forms.Label labelTotalSeasonsTitle;
        private System.Windows.Forms.Label labelTotalShows;
        private System.Windows.Forms.Label labelTotalShowsTitle;
        private System.Windows.Forms.Label labelTotalLocalEpisodes;
        private System.Windows.Forms.Label labelTotalLocalEpisodesTitle;
    }
}
