namespace TVRename.Windows.Controls
{
    partial class ShowView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelName = new System.Windows.Forms.Label();
            this.pictureBoxBanner = new System.Windows.Forms.PictureBox();
            this.labelActorsTitle = new System.Windows.Forms.Label();
            this.labelOverview = new System.Windows.Forms.Label();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelActors = new System.Windows.Forms.Label();
            this.labelAirsTitle = new System.Windows.Forms.Label();
            this.labelAirs = new System.Windows.Forms.Label();
            this.labelInformationTitle = new System.Windows.Forms.Label();
            this.labelFirstAiredTitle = new System.Windows.Forms.Label();
            this.labelFirstAired = new System.Windows.Forms.Label();
            this.labelRuntimeTitle = new System.Windows.Forms.Label();
            this.labelRuntime = new System.Windows.Forms.Label();
            this.labelStatusTitle = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelGenresTitle = new System.Windows.Forms.Label();
            this.labelGenres = new System.Windows.Forms.Label();
            this.labelRatingTitle = new System.Windows.Forms.Label();
            this.labelRating = new System.Windows.Forms.Label();
            this.panelVisit = new System.Windows.Forms.Panel();
            this.buttonTvCom = new System.Windows.Forms.Button();
            this.buttonImdb = new System.Windows.Forms.Button();
            this.buttonRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBanner)).BeginInit();
            this.tableLayoutPanel.SuspendLayout();
            this.panelVisit.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.labelName, 2);
            this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelName.Location = new System.Drawing.Point(3, 156);
            this.labelName.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(192, 37);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "Show Name";
            // 
            // pictureBoxBanner
            // 
            this.tableLayoutPanel.SetColumnSpan(this.pictureBoxBanner, 2);
            this.pictureBoxBanner.ErrorImage = null;
            this.pictureBoxBanner.InitialImage = null;
            this.pictureBoxBanner.Location = new System.Drawing.Point(3, 3);
            this.pictureBoxBanner.Name = "pictureBoxBanner";
            this.pictureBoxBanner.Size = new System.Drawing.Size(749, 140);
            this.pictureBoxBanner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxBanner.TabIndex = 0;
            this.pictureBoxBanner.TabStop = false;
            // 
            // labelActorsTitle
            // 
            this.labelActorsTitle.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.labelActorsTitle, 2);
            this.labelActorsTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelActorsTitle.Location = new System.Drawing.Point(3, 240);
            this.labelActorsTitle.Margin = new System.Windows.Forms.Padding(3, 10, 3, 5);
            this.labelActorsTitle.Name = "labelActorsTitle";
            this.labelActorsTitle.Size = new System.Drawing.Size(80, 29);
            this.labelActorsTitle.TabIndex = 2;
            this.labelActorsTitle.Text = "Actors";
            // 
            // labelOverview
            // 
            this.labelOverview.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.labelOverview, 2);
            this.labelOverview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelOverview.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOverview.Location = new System.Drawing.Point(3, 208);
            this.labelOverview.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.labelOverview.Name = "labelOverview";
            this.labelOverview.Size = new System.Drawing.Size(749, 17);
            this.labelOverview.TabIndex = 1;
            this.labelOverview.Text = "Show overview\r\n";
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.pictureBoxBanner, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.labelName, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.labelOverview, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.labelActorsTitle, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.labelActors, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.labelAirsTitle, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.labelAirs, 0, 6);
            this.tableLayoutPanel.Controls.Add(this.labelInformationTitle, 0, 7);
            this.tableLayoutPanel.Controls.Add(this.labelFirstAiredTitle, 0, 9);
            this.tableLayoutPanel.Controls.Add(this.labelFirstAired, 1, 9);
            this.tableLayoutPanel.Controls.Add(this.labelRuntimeTitle, 0, 10);
            this.tableLayoutPanel.Controls.Add(this.labelRuntime, 1, 10);
            this.tableLayoutPanel.Controls.Add(this.labelStatusTitle, 0, 11);
            this.tableLayoutPanel.Controls.Add(this.labelStatus, 1, 11);
            this.tableLayoutPanel.Controls.Add(this.labelGenresTitle, 0, 12);
            this.tableLayoutPanel.Controls.Add(this.labelGenres, 1, 12);
            this.tableLayoutPanel.Controls.Add(this.labelRatingTitle, 0, 13);
            this.tableLayoutPanel.Controls.Add(this.labelRating, 1, 13);
            this.tableLayoutPanel.Controls.Add(this.panelVisit, 0, 14);
            this.tableLayoutPanel.Controls.Add(this.buttonRefresh, 0, 15);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 16;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(755, 626);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // labelActors
            // 
            this.labelActors.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.labelActors, 2);
            this.labelActors.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelActors.Location = new System.Drawing.Point(3, 279);
            this.labelActors.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.labelActors.Name = "labelActors";
            this.labelActors.Size = new System.Drawing.Size(106, 17);
            this.labelActors.TabIndex = 3;
            this.labelActors.Text = "Actor 1, Actor 2";
            // 
            // labelAirsTitle
            // 
            this.labelAirsTitle.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.labelAirsTitle, 2);
            this.labelAirsTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAirsTitle.Location = new System.Drawing.Point(3, 311);
            this.labelAirsTitle.Margin = new System.Windows.Forms.Padding(3, 10, 3, 5);
            this.labelAirsTitle.Name = "labelAirsTitle";
            this.labelAirsTitle.Size = new System.Drawing.Size(54, 29);
            this.labelAirsTitle.TabIndex = 4;
            this.labelAirsTitle.Text = "Airs";
            // 
            // labelAirs
            // 
            this.labelAirs.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.labelAirs, 2);
            this.labelAirs.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAirs.Location = new System.Drawing.Point(3, 350);
            this.labelAirs.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.labelAirs.Name = "labelAirs";
            this.labelAirs.Size = new System.Drawing.Size(164, 17);
            this.labelAirs.TabIndex = 5;
            this.labelAirs.Text = "Date at Time on Network";
            // 
            // labelInformationTitle
            // 
            this.labelInformationTitle.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.labelInformationTitle, 2);
            this.labelInformationTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInformationTitle.Location = new System.Drawing.Point(3, 382);
            this.labelInformationTitle.Margin = new System.Windows.Forms.Padding(3, 10, 3, 5);
            this.labelInformationTitle.Name = "labelInformationTitle";
            this.labelInformationTitle.Size = new System.Drawing.Size(132, 29);
            this.labelInformationTitle.TabIndex = 6;
            this.labelInformationTitle.Text = "Information";
            // 
            // labelFirstAiredTitle
            // 
            this.labelFirstAiredTitle.AutoSize = true;
            this.labelFirstAiredTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFirstAiredTitle.Location = new System.Drawing.Point(3, 421);
            this.labelFirstAiredTitle.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.labelFirstAiredTitle.Name = "labelFirstAiredTitle";
            this.labelFirstAiredTitle.Size = new System.Drawing.Size(72, 17);
            this.labelFirstAiredTitle.TabIndex = 8;
            this.labelFirstAiredTitle.Text = "First Aired";
            // 
            // labelFirstAired
            // 
            this.labelFirstAired.AutoSize = true;
            this.labelFirstAired.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFirstAired.Location = new System.Drawing.Point(203, 421);
            this.labelFirstAired.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.labelFirstAired.Name = "labelFirstAired";
            this.labelFirstAired.Size = new System.Drawing.Size(38, 17);
            this.labelFirstAired.TabIndex = 1;
            this.labelFirstAired.Text = "Date";
            // 
            // labelRuntimeTitle
            // 
            this.labelRuntimeTitle.AutoSize = true;
            this.labelRuntimeTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRuntimeTitle.Location = new System.Drawing.Point(3, 448);
            this.labelRuntimeTitle.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.labelRuntimeTitle.Name = "labelRuntimeTitle";
            this.labelRuntimeTitle.Size = new System.Drawing.Size(60, 17);
            this.labelRuntimeTitle.TabIndex = 2;
            this.labelRuntimeTitle.Text = "Runtime";
            // 
            // labelRuntime
            // 
            this.labelRuntime.AutoSize = true;
            this.labelRuntime.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRuntime.Location = new System.Drawing.Point(203, 448);
            this.labelRuntime.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.labelRuntime.Name = "labelRuntime";
            this.labelRuntime.Size = new System.Drawing.Size(69, 17);
            this.labelRuntime.TabIndex = 3;
            this.labelRuntime.Text = "0 minutes";
            // 
            // labelStatusTitle
            // 
            this.labelStatusTitle.AutoSize = true;
            this.labelStatusTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatusTitle.Location = new System.Drawing.Point(3, 475);
            this.labelStatusTitle.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.labelStatusTitle.Name = "labelStatusTitle";
            this.labelStatusTitle.Size = new System.Drawing.Size(48, 17);
            this.labelStatusTitle.TabIndex = 4;
            this.labelStatusTitle.Text = "Status";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatus.Location = new System.Drawing.Point(203, 475);
            this.labelStatus.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(48, 17);
            this.labelStatus.TabIndex = 5;
            this.labelStatus.Text = "Status";
            // 
            // labelGenresTitle
            // 
            this.labelGenresTitle.AutoSize = true;
            this.labelGenresTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGenresTitle.Location = new System.Drawing.Point(3, 502);
            this.labelGenresTitle.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.labelGenresTitle.Name = "labelGenresTitle";
            this.labelGenresTitle.Size = new System.Drawing.Size(55, 17);
            this.labelGenresTitle.TabIndex = 6;
            this.labelGenresTitle.Text = "Genres";
            // 
            // labelGenres
            // 
            this.labelGenres.AutoSize = true;
            this.labelGenres.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGenres.Location = new System.Drawing.Point(203, 502);
            this.labelGenres.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.labelGenres.Name = "labelGenres";
            this.labelGenres.Size = new System.Drawing.Size(120, 17);
            this.labelGenres.TabIndex = 7;
            this.labelGenres.Text = "Genre 1, Genre 2";
            // 
            // labelRatingTitle
            // 
            this.labelRatingTitle.AutoSize = true;
            this.labelRatingTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRatingTitle.Location = new System.Drawing.Point(3, 529);
            this.labelRatingTitle.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.labelRatingTitle.Name = "labelRatingTitle";
            this.labelRatingTitle.Size = new System.Drawing.Size(49, 17);
            this.labelRatingTitle.TabIndex = 8;
            this.labelRatingTitle.Text = "Rating";
            // 
            // labelRating
            // 
            this.labelRating.AutoSize = true;
            this.labelRating.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRating.Location = new System.Drawing.Point(203, 529);
            this.labelRating.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.labelRating.Name = "labelRating";
            this.labelRating.Size = new System.Drawing.Size(96, 17);
            this.labelRating.TabIndex = 9;
            this.labelRating.Text = "0/10 (0 votes)";
            // 
            // panelVisit
            // 
            this.panelVisit.AutoSize = true;
            this.panelVisit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel.SetColumnSpan(this.panelVisit, 2);
            this.panelVisit.Controls.Add(this.buttonTvCom);
            this.panelVisit.Controls.Add(this.buttonImdb);
            this.panelVisit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelVisit.Location = new System.Drawing.Point(3, 554);
            this.panelVisit.Name = "panelVisit";
            this.panelVisit.Size = new System.Drawing.Size(749, 33);
            this.panelVisit.TabIndex = 11;
            // 
            // buttonTvCom
            // 
            this.buttonTvCom.Location = new System.Drawing.Point(81, 7);
            this.buttonTvCom.Name = "buttonTvCom";
            this.buttonTvCom.Size = new System.Drawing.Size(75, 23);
            this.buttonTvCom.TabIndex = 1;
            this.buttonTvCom.Text = "Visit &TV.com";
            this.buttonTvCom.UseVisualStyleBackColor = true;
            this.buttonTvCom.Click += new System.EventHandler(this.buttonTvCom_Click);
            // 
            // buttonImdb
            // 
            this.buttonImdb.Location = new System.Drawing.Point(0, 7);
            this.buttonImdb.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.buttonImdb.Name = "buttonImdb";
            this.buttonImdb.Size = new System.Drawing.Size(75, 23);
            this.buttonImdb.TabIndex = 0;
            this.buttonImdb.Text = "Visit &IMDB";
            this.buttonImdb.UseVisualStyleBackColor = true;
            this.buttonImdb.Click += new System.EventHandler(this.buttonImdb_Click);
            // 
            // buttonRefresh
            // 
            this.tableLayoutPanel.SetColumnSpan(this.buttonRefresh, 2);
            this.buttonRefresh.Location = new System.Drawing.Point(3, 593);
            this.buttonRefresh.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(75, 23);
            this.buttonRefresh.TabIndex = 11;
            this.buttonRefresh.Text = "&Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // ShowView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.tableLayoutPanel);
            this.DoubleBuffered = true;
            this.Name = "ShowView";
            this.Size = new System.Drawing.Size(755, 627);
            this.Load += new System.EventHandler(this.ShowView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBanner)).EndInit();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.panelVisit.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxBanner;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelActorsTitle;
        private System.Windows.Forms.Label labelOverview;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label labelActors;
        private System.Windows.Forms.Label labelAirs;
        private System.Windows.Forms.Label labelAirsTitle;
        private System.Windows.Forms.Label labelInformationTitle;
        private System.Windows.Forms.Label labelRatingTitle;
        private System.Windows.Forms.Label labelGenresTitle;
        private System.Windows.Forms.Label labelGenres;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label labelRuntimeTitle;
        private System.Windows.Forms.Label labelRuntime;
        private System.Windows.Forms.Label labelStatusTitle;
        private System.Windows.Forms.Label labelFirstAired;
        private System.Windows.Forms.Label labelRating;
        private System.Windows.Forms.Label labelFirstAiredTitle;
        private System.Windows.Forms.Panel panelVisit;
        private System.Windows.Forms.Button buttonTvCom;
        private System.Windows.Forms.Button buttonImdb;
        private System.Windows.Forms.Button buttonRefresh;
    }
}
