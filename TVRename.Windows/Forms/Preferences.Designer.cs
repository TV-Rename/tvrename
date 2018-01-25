namespace TVRename.Windows.Forms
{
    partial class Preferences
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.labelRecentDays = new System.Windows.Forms.Label();
            this.textBoxDefaultLocation = new System.Windows.Forms.TextBox();
            this.groupBoxCalendar = new System.Windows.Forms.GroupBox();
            this.numericUpDownRecentDays = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownDownloadThreads = new System.Windows.Forms.NumericUpDown();
            this.labelDownloadThreads = new System.Windows.Forms.Label();
            this.groupBoxLocation = new System.Windows.Forms.GroupBox();
            this.labelEpisodeTemplate = new System.Windows.Forms.Label();
            this.textBoxEpisodeTemplate = new System.Windows.Forms.TextBox();
            this.labelSeasonTemplate = new System.Windows.Forms.Label();
            this.textBoxSeasonTemplate = new System.Windows.Forms.TextBox();
            this.labelSpecialsTemplate = new System.Windows.Forms.Label();
            this.textBoxSpecialsTemplate = new System.Windows.Forms.TextBox();
            this.buttonDefaultLocation = new System.Windows.Forms.Button();
            this.labelDefaultLocation = new System.Windows.Forms.Label();
            this.groupBoxDownload = new System.Windows.Forms.GroupBox();
            this.groupBoxCalendar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRecentDays)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDownloadThreads)).BeginInit();
            this.groupBoxLocation.SuspendLayout();
            this.groupBoxDownload.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(329, 361);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(248, 361);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "&OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // labelRecentDays
            // 
            this.labelRecentDays.AutoSize = true;
            this.labelRecentDays.Location = new System.Drawing.Point(6, 29);
            this.labelRecentDays.Name = "labelRecentDays";
            this.labelRecentDays.Size = new System.Drawing.Size(120, 13);
            this.labelRecentDays.TabIndex = 0;
            this.labelRecentDays.Text = "Days to count as recent";
            // 
            // textBoxDefaultLocation
            // 
            this.textBoxDefaultLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDefaultLocation.Location = new System.Drawing.Point(150, 19);
            this.textBoxDefaultLocation.Name = "textBoxDefaultLocation";
            this.textBoxDefaultLocation.Size = new System.Drawing.Size(155, 20);
            this.textBoxDefaultLocation.TabIndex = 1;
            // 
            // groupBoxCalendar
            // 
            this.groupBoxCalendar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxCalendar.Controls.Add(this.numericUpDownRecentDays);
            this.groupBoxCalendar.Controls.Add(this.labelRecentDays);
            this.groupBoxCalendar.Location = new System.Drawing.Point(12, 12);
            this.groupBoxCalendar.Name = "groupBoxCalendar";
            this.groupBoxCalendar.Size = new System.Drawing.Size(392, 100);
            this.groupBoxCalendar.TabIndex = 0;
            this.groupBoxCalendar.TabStop = false;
            this.groupBoxCalendar.Text = "Calendar";
            // 
            // numericUpDownRecentDays
            // 
            this.numericUpDownRecentDays.Location = new System.Drawing.Point(150, 27);
            this.numericUpDownRecentDays.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownRecentDays.Name = "numericUpDownRecentDays";
            this.numericUpDownRecentDays.Size = new System.Drawing.Size(63, 20);
            this.numericUpDownRecentDays.TabIndex = 1;
            this.numericUpDownRecentDays.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numericUpDownDownloadThreads
            // 
            this.numericUpDownDownloadThreads.Location = new System.Drawing.Point(150, 25);
            this.numericUpDownDownloadThreads.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownDownloadThreads.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownDownloadThreads.Name = "numericUpDownDownloadThreads";
            this.numericUpDownDownloadThreads.Size = new System.Drawing.Size(63, 20);
            this.numericUpDownDownloadThreads.TabIndex = 1;
            this.numericUpDownDownloadThreads.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // labelDownloadThreads
            // 
            this.labelDownloadThreads.AutoSize = true;
            this.labelDownloadThreads.Location = new System.Drawing.Point(6, 27);
            this.labelDownloadThreads.Name = "labelDownloadThreads";
            this.labelDownloadThreads.Size = new System.Drawing.Size(138, 13);
            this.labelDownloadThreads.TabIndex = 0;
            this.labelDownloadThreads.Text = "Maximum download threads";
            // 
            // groupBoxLocation
            // 
            this.groupBoxLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxLocation.Controls.Add(this.labelEpisodeTemplate);
            this.groupBoxLocation.Controls.Add(this.textBoxEpisodeTemplate);
            this.groupBoxLocation.Controls.Add(this.labelSeasonTemplate);
            this.groupBoxLocation.Controls.Add(this.textBoxSeasonTemplate);
            this.groupBoxLocation.Controls.Add(this.labelSpecialsTemplate);
            this.groupBoxLocation.Controls.Add(this.textBoxSpecialsTemplate);
            this.groupBoxLocation.Controls.Add(this.buttonDefaultLocation);
            this.groupBoxLocation.Controls.Add(this.labelDefaultLocation);
            this.groupBoxLocation.Controls.Add(this.textBoxDefaultLocation);
            this.groupBoxLocation.Location = new System.Drawing.Point(12, 224);
            this.groupBoxLocation.Name = "groupBoxLocation";
            this.groupBoxLocation.Size = new System.Drawing.Size(392, 131);
            this.groupBoxLocation.TabIndex = 2;
            this.groupBoxLocation.TabStop = false;
            this.groupBoxLocation.Text = "Location";
            // 
            // labelEpisodeTemplate
            // 
            this.labelEpisodeTemplate.AutoSize = true;
            this.labelEpisodeTemplate.Location = new System.Drawing.Point(6, 100);
            this.labelEpisodeTemplate.Name = "labelEpisodeTemplate";
            this.labelEpisodeTemplate.Size = new System.Drawing.Size(89, 13);
            this.labelEpisodeTemplate.TabIndex = 7;
            this.labelEpisodeTemplate.Text = "EpisodeTemplate";
            // 
            // textBoxEpisodeTemplate
            // 
            this.textBoxEpisodeTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxEpisodeTemplate.Location = new System.Drawing.Point(150, 97);
            this.textBoxEpisodeTemplate.Name = "textBoxEpisodeTemplate";
            this.textBoxEpisodeTemplate.Size = new System.Drawing.Size(236, 20);
            this.textBoxEpisodeTemplate.TabIndex = 8;
            // 
            // labelSeasonTemplate
            // 
            this.labelSeasonTemplate.AutoSize = true;
            this.labelSeasonTemplate.Location = new System.Drawing.Point(6, 48);
            this.labelSeasonTemplate.Name = "labelSeasonTemplate";
            this.labelSeasonTemplate.Size = new System.Drawing.Size(75, 13);
            this.labelSeasonTemplate.TabIndex = 3;
            this.labelSeasonTemplate.Text = "Season format";
            // 
            // textBoxSeasonTemplate
            // 
            this.textBoxSeasonTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSeasonTemplate.Location = new System.Drawing.Point(150, 45);
            this.textBoxSeasonTemplate.Name = "textBoxSeasonTemplate";
            this.textBoxSeasonTemplate.Size = new System.Drawing.Size(236, 20);
            this.textBoxSeasonTemplate.TabIndex = 4;
            // 
            // labelSpecialsTemplate
            // 
            this.labelSpecialsTemplate.AutoSize = true;
            this.labelSpecialsTemplate.Location = new System.Drawing.Point(6, 74);
            this.labelSpecialsTemplate.Name = "labelSpecialsTemplate";
            this.labelSpecialsTemplate.Size = new System.Drawing.Size(79, 13);
            this.labelSpecialsTemplate.TabIndex = 5;
            this.labelSpecialsTemplate.Text = "Specials format";
            // 
            // textBoxSpecialsTemplate
            // 
            this.textBoxSpecialsTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSpecialsTemplate.Location = new System.Drawing.Point(150, 71);
            this.textBoxSpecialsTemplate.Name = "textBoxSpecialsTemplate";
            this.textBoxSpecialsTemplate.Size = new System.Drawing.Size(236, 20);
            this.textBoxSpecialsTemplate.TabIndex = 6;
            // 
            // buttonDefaultLocation
            // 
            this.buttonDefaultLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDefaultLocation.Location = new System.Drawing.Point(311, 17);
            this.buttonDefaultLocation.Name = "buttonDefaultLocation";
            this.buttonDefaultLocation.Size = new System.Drawing.Size(75, 23);
            this.buttonDefaultLocation.TabIndex = 2;
            this.buttonDefaultLocation.Text = "&Browse...";
            this.buttonDefaultLocation.UseVisualStyleBackColor = true;
            this.buttonDefaultLocation.Click += new System.EventHandler(this.buttonDefaultLocation_Click);
            // 
            // labelDefaultLocation
            // 
            this.labelDefaultLocation.AutoSize = true;
            this.labelDefaultLocation.Location = new System.Drawing.Point(6, 22);
            this.labelDefaultLocation.Name = "labelDefaultLocation";
            this.labelDefaultLocation.Size = new System.Drawing.Size(107, 13);
            this.labelDefaultLocation.TabIndex = 0;
            this.labelDefaultLocation.Text = "Default base location";
            // 
            // groupBoxDownload
            // 
            this.groupBoxDownload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxDownload.Controls.Add(this.labelDownloadThreads);
            this.groupBoxDownload.Controls.Add(this.numericUpDownDownloadThreads);
            this.groupBoxDownload.Location = new System.Drawing.Point(12, 118);
            this.groupBoxDownload.Name = "groupBoxDownload";
            this.groupBoxDownload.Size = new System.Drawing.Size(392, 100);
            this.groupBoxDownload.TabIndex = 1;
            this.groupBoxDownload.TabStop = false;
            this.groupBoxDownload.Text = "Download";
            // 
            // Preferences
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(416, 396);
            this.Controls.Add(this.groupBoxDownload);
            this.Controls.Add(this.groupBoxLocation);
            this.Controls.Add(this.groupBoxCalendar);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Preferences";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Preferences";
            this.Load += new System.EventHandler(this.Preferences_Load);
            this.groupBoxCalendar.ResumeLayout(false);
            this.groupBoxCalendar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRecentDays)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDownloadThreads)).EndInit();
            this.groupBoxLocation.ResumeLayout(false);
            this.groupBoxLocation.PerformLayout();
            this.groupBoxDownload.ResumeLayout(false);
            this.groupBoxDownload.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label labelRecentDays;
        private System.Windows.Forms.TextBox textBoxDefaultLocation;
        private System.Windows.Forms.GroupBox groupBoxCalendar;
        private System.Windows.Forms.NumericUpDown numericUpDownRecentDays;
        private System.Windows.Forms.NumericUpDown numericUpDownDownloadThreads;
        private System.Windows.Forms.Label labelDownloadThreads;
        private System.Windows.Forms.GroupBox groupBoxLocation;
        private System.Windows.Forms.GroupBox groupBoxDownload;
        private System.Windows.Forms.Button buttonDefaultLocation;
        private System.Windows.Forms.Label labelDefaultLocation;
        private System.Windows.Forms.Label labelSeasonTemplate;
        private System.Windows.Forms.TextBox textBoxSeasonTemplate;
        private System.Windows.Forms.Label labelSpecialsTemplate;
        private System.Windows.Forms.TextBox textBoxSpecialsTemplate;
        private System.Windows.Forms.Label labelEpisodeTemplate;
        private System.Windows.Forms.TextBox textBoxEpisodeTemplate;
    }
}