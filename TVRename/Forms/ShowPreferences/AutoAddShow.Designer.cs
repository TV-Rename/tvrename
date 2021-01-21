namespace TVRename
{
    partial class AutoAddShow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoAddShow));
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
            this.tpMovie = new System.Windows.Forms.TabPage();
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
            this.pnlCF.Size = new System.Drawing.Size(463, 367);
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
            // tpMovie
            // 
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
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(5, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(463, 367);
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
            this.cbMovieDirectory.Size = new System.Drawing.Size(224, 21);
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
            // AutoAddShow
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
            this.Name = "AutoAddShow";
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
    }
}
