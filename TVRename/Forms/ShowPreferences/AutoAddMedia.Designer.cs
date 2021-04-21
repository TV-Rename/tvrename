namespace TVRename
{
    partial class AutoAddMedia
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoAddMedia));
            this.cbDirectory = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlCF = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblDirectoryName = new System.Windows.Forms.Label();
            this.btnIgnoreFile = new System.Windows.Forms.Button();
            this.btnSkipAutoAdd = new System.Windows.Forms.Button();
            this.lblFileName = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpTV = new System.Windows.Forms.TabPage();
            this.rdoTVTMDB = new System.Windows.Forms.RadioButton();
            this.label13 = new System.Windows.Forms.Label();
            this.rdoTVTVMaze = new System.Windows.Forms.RadioButton();
            this.rdoTVTVDB = new System.Windows.Forms.RadioButton();
            this.rdoTVDefault = new System.Windows.Forms.RadioButton();
            this.tpMovie = new System.Windows.Forms.TabPage();
            this.rdoMovieTMDB = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.rdoMovieTVDB = new System.Windows.Forms.RadioButton();
            this.rdoMovieLibraryDefault = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbMovieDirectory = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tpTV.SuspendLayout();
            this.tpMovie.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbDirectory
            // 
            this.cbDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDirectory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDirectory.FormattingEnabled = true;
            this.cbDirectory.Location = new System.Drawing.Point(70, 376);
            this.cbDirectory.Name = "cbDirectory";
            this.cbDirectory.Size = new System.Drawing.Size(224, 21);
            this.cbDirectory.Sorted = true;
            this.cbDirectory.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(412, 457);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "Quick Add";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(316, 457);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Leave for later";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // pnlCF
            // 
            this.pnlCF.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlCF.Location = new System.Drawing.Point(3, 3);
            this.pnlCF.Name = "pnlCF";
            this.pnlCF.Size = new System.Drawing.Size(463, 342);
            this.pnlCF.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 384);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Location:";
            // 
            // lblDirectoryName
            // 
            this.lblDirectoryName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDirectoryName.AutoSize = true;
            this.lblDirectoryName.Location = new System.Drawing.Point(338, 429);
            this.lblDirectoryName.Name = "lblDirectoryName";
            this.lblDirectoryName.Size = new System.Drawing.Size(0, 13);
            this.lblDirectoryName.TabIndex = 5;
            this.lblDirectoryName.UseMnemonic = false;
            // 
            // btnIgnoreFile
            // 
            this.btnIgnoreFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIgnoreFile.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnIgnoreFile.Location = new System.Drawing.Point(205, 457);
            this.btnIgnoreFile.Name = "btnIgnoreFile";
            this.btnIgnoreFile.Size = new System.Drawing.Size(105, 23);
            this.btnIgnoreFile.TabIndex = 6;
            this.btnIgnoreFile.Text = "Ignore File Forever";
            this.btnIgnoreFile.UseVisualStyleBackColor = true;
            this.btnIgnoreFile.Click += new System.EventHandler(this.btnIgnoreFile_Click);
            // 
            // btnSkipAutoAdd
            // 
            this.btnSkipAutoAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSkipAutoAdd.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSkipAutoAdd.Location = new System.Drawing.Point(109, 457);
            this.btnSkipAutoAdd.Name = "btnSkipAutoAdd";
            this.btnSkipAutoAdd.Size = new System.Drawing.Size(90, 23);
            this.btnSkipAutoAdd.TabIndex = 7;
            this.btnSkipAutoAdd.Text = "Skip Auto Add";
            this.btnSkipAutoAdd.UseVisualStyleBackColor = true;
            this.btnSkipAutoAdd.Click += new System.EventHandler(this.btnSkipAutoAdd_Click);
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Location = new System.Drawing.Point(13, 9);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(0, 13);
            this.lblFileName.TabIndex = 8;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tpTV);
            this.tabControl1.Controls.Add(this.tpMovie);
            this.tabControl1.Location = new System.Drawing.Point(12, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(480, 426);
            this.tabControl1.TabIndex = 9;
            // 
            // tpTV
            // 
            this.tpTV.Controls.Add(this.rdoTVTMDB);
            this.tpTV.Controls.Add(this.label13);
            this.tpTV.Controls.Add(this.rdoTVTVMaze);
            this.tpTV.Controls.Add(this.rdoTVTVDB);
            this.tpTV.Controls.Add(this.rdoTVDefault);
            this.tpTV.Controls.Add(this.pnlCF);
            this.tpTV.Controls.Add(this.cbDirectory);
            this.tpTV.Controls.Add(this.label1);
            this.tpTV.Location = new System.Drawing.Point(4, 22);
            this.tpTV.Name = "tpTV";
            this.tpTV.Padding = new System.Windows.Forms.Padding(3);
            this.tpTV.Size = new System.Drawing.Size(472, 400);
            this.tpTV.TabIndex = 0;
            this.tpTV.Text = "TV";
            this.tpTV.UseVisualStyleBackColor = true;
            // 
            // rdoTVTMDB
            // 
            this.rdoTVTMDB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdoTVTMDB.AutoSize = true;
            this.rdoTVTMDB.Location = new System.Drawing.Point(300, 353);
            this.rdoTVTMDB.Name = "rdoTVTMDB";
            this.rdoTVTMDB.Size = new System.Drawing.Size(56, 17);
            this.rdoTVTMDB.TabIndex = 50;
            this.rdoTVTMDB.TabStop = true;
            this.rdoTVTMDB.Text = "TMDB";
            this.rdoTVTMDB.UseVisualStyleBackColor = true;
            this.rdoTVTMDB.CheckedChanged += new System.EventHandler(this.rdoTVProvider_CheckedChanged);
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 355);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(44, 13);
            this.label13.TabIndex = 49;
            this.label13.Text = "Source:";
            this.label13.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // rdoTVTVMaze
            // 
            this.rdoTVTVMaze.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdoTVTVMaze.AutoSize = true;
            this.rdoTVTVMaze.Location = new System.Drawing.Point(230, 353);
            this.rdoTVTVMaze.Name = "rdoTVTVMaze";
            this.rdoTVTVMaze.Size = new System.Drawing.Size(64, 17);
            this.rdoTVTVMaze.TabIndex = 48;
            this.rdoTVTVMaze.TabStop = true;
            this.rdoTVTVMaze.Text = "TVmaze";
            this.rdoTVTVMaze.UseVisualStyleBackColor = true;
            this.rdoTVTVMaze.CheckedChanged += new System.EventHandler(this.rdoTVProvider_CheckedChanged);
            // 
            // rdoTVTVDB
            // 
            this.rdoTVTVDB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdoTVTVDB.AutoSize = true;
            this.rdoTVTVDB.Location = new System.Drawing.Point(143, 353);
            this.rdoTVTVDB.Name = "rdoTVTVDB";
            this.rdoTVTVDB.Size = new System.Drawing.Size(76, 17);
            this.rdoTVTVDB.TabIndex = 47;
            this.rdoTVTVDB.TabStop = true;
            this.rdoTVTVDB.Text = "The TVDB";
            this.rdoTVTVDB.UseVisualStyleBackColor = true;
            this.rdoTVTVDB.CheckedChanged += new System.EventHandler(this.rdoTVProvider_CheckedChanged);
            // 
            // rdoTVDefault
            // 
            this.rdoTVDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdoTVDefault.AutoSize = true;
            this.rdoTVDefault.Location = new System.Drawing.Point(48, 353);
            this.rdoTVDefault.Name = "rdoTVDefault";
            this.rdoTVDefault.Size = new System.Drawing.Size(93, 17);
            this.rdoTVDefault.TabIndex = 46;
            this.rdoTVDefault.TabStop = true;
            this.rdoTVDefault.Text = "Library Default";
            this.rdoTVDefault.UseVisualStyleBackColor = true;
            this.rdoTVDefault.CheckedChanged += new System.EventHandler(this.rdoTVProvider_CheckedChanged);
            // 
            // tpMovie
            // 
            this.tpMovie.Controls.Add(this.rdoMovieTMDB);
            this.tpMovie.Controls.Add(this.label3);
            this.tpMovie.Controls.Add(this.rdoMovieTVDB);
            this.tpMovie.Controls.Add(this.rdoMovieLibraryDefault);
            this.tpMovie.Controls.Add(this.panel1);
            this.tpMovie.Controls.Add(this.cbMovieDirectory);
            this.tpMovie.Controls.Add(this.label2);
            this.tpMovie.Location = new System.Drawing.Point(4, 22);
            this.tpMovie.Name = "tpMovie";
            this.tpMovie.Padding = new System.Windows.Forms.Padding(3);
            this.tpMovie.Size = new System.Drawing.Size(472, 400);
            this.tpMovie.TabIndex = 1;
            this.tpMovie.Text = "Movie";
            this.tpMovie.UseVisualStyleBackColor = true;
            // 
            // rdoMovieTMDB
            // 
            this.rdoMovieTMDB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdoMovieTMDB.AutoSize = true;
            this.rdoMovieTMDB.Location = new System.Drawing.Point(229, 354);
            this.rdoMovieTMDB.Name = "rdoMovieTMDB";
            this.rdoMovieTMDB.Size = new System.Drawing.Size(56, 17);
            this.rdoMovieTMDB.TabIndex = 49;
            this.rdoMovieTMDB.TabStop = true;
            this.rdoMovieTMDB.Text = "TMDB";
            this.rdoMovieTMDB.UseVisualStyleBackColor = true;
            this.rdoMovieTMDB.CheckedChanged += new System.EventHandler(this.rdoMovieProvider_CheckedChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 356);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 48;
            this.label3.Text = "Source:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // rdoMovieTVDB
            // 
            this.rdoMovieTVDB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdoMovieTVDB.AutoSize = true;
            this.rdoMovieTVDB.Location = new System.Drawing.Point(147, 354);
            this.rdoMovieTVDB.Name = "rdoMovieTVDB";
            this.rdoMovieTVDB.Size = new System.Drawing.Size(76, 17);
            this.rdoMovieTVDB.TabIndex = 47;
            this.rdoMovieTVDB.TabStop = true;
            this.rdoMovieTVDB.Text = "The TVDB";
            this.rdoMovieTVDB.UseVisualStyleBackColor = true;
            this.rdoMovieTVDB.CheckedChanged += new System.EventHandler(this.rdoMovieProvider_CheckedChanged);
            // 
            // rdoMovieLibraryDefault
            // 
            this.rdoMovieLibraryDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdoMovieLibraryDefault.AutoSize = true;
            this.rdoMovieLibraryDefault.Location = new System.Drawing.Point(52, 354);
            this.rdoMovieLibraryDefault.Name = "rdoMovieLibraryDefault";
            this.rdoMovieLibraryDefault.Size = new System.Drawing.Size(93, 17);
            this.rdoMovieLibraryDefault.TabIndex = 46;
            this.rdoMovieLibraryDefault.TabStop = true;
            this.rdoMovieLibraryDefault.Text = "Library Default";
            this.rdoMovieLibraryDefault.UseVisualStyleBackColor = true;
            this.rdoMovieLibraryDefault.CheckedChanged += new System.EventHandler(this.rdoMovieProvider_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(5, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(463, 344);
            this.panel1.TabIndex = 6;
            // 
            // cbMovieDirectory
            // 
            this.cbMovieDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbMovieDirectory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMovieDirectory.FormattingEnabled = true;
            this.cbMovieDirectory.Location = new System.Drawing.Point(72, 376);
            this.cbMovieDirectory.Name = "cbMovieDirectory";
            this.cbMovieDirectory.Size = new System.Drawing.Size(222, 21);
            this.cbMovieDirectory.Sorted = true;
            this.cbMovieDirectory.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 384);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Location:";
            // 
            // AutoAddMedia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(499, 492);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.btnSkipAutoAdd);
            this.Controls.Add(this.btnIgnoreFile);
            this.Controls.Add(this.lblDirectoryName);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AutoAddMedia";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Show Detected...";
            this.Load += new System.EventHandler(this.AutoAddShow_Load);
            this.tabControl1.ResumeLayout(false);
            this.tpTV.ResumeLayout(false);
            this.tpTV.PerformLayout();
            this.tpMovie.ResumeLayout(false);
            this.tpMovie.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbDirectory;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel pnlCF;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblDirectoryName;
        private System.Windows.Forms.Button btnIgnoreFile;
        private System.Windows.Forms.Button btnSkipAutoAdd;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpTV;
        private System.Windows.Forms.TabPage tpMovie;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cbMovieDirectory;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rdoTVTMDB;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.RadioButton rdoTVTVMaze;
        private System.Windows.Forms.RadioButton rdoTVTVDB;
        private System.Windows.Forms.RadioButton rdoTVDefault;
        private System.Windows.Forms.RadioButton rdoMovieTMDB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rdoMovieTVDB;
        private System.Windows.Forms.RadioButton rdoMovieLibraryDefault;
    }
}
