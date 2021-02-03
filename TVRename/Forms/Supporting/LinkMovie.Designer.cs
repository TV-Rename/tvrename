namespace TVRename
{
    partial class LinkMovie
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
            this.btnUseSelectedMovie = new System.Windows.Forms.Button();
            this.btnNotAMovie = new System.Windows.Forms.Button();
            this.btnNewMovie = new System.Windows.Forms.Button();
            this.lnkOpenFolder = new System.Windows.Forms.LinkLabel();
            this.lblNameRight = new System.Windows.Forms.Label();
            this.txtNameLeft = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblSourceFileName = new System.Windows.Forms.Label();
            this.cbShows = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnUseSelectedMovie
            // 
            this.btnUseSelectedMovie.Location = new System.Drawing.Point(311, 50);
            this.btnUseSelectedMovie.Name = "btnUseSelectedMovie";
            this.btnUseSelectedMovie.Size = new System.Drawing.Size(75, 46);
            this.btnUseSelectedMovie.TabIndex = 0;
            this.btnUseSelectedMovie.Text = "Use";
            this.btnUseSelectedMovie.UseVisualStyleBackColor = true;
            this.btnUseSelectedMovie.Click += new System.EventHandler(this.btnUseSelectedMovie_Click);
            // 
            // btnNotAMovie
            // 
            this.btnNotAMovie.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnNotAMovie.Location = new System.Drawing.Point(276, 154);
            this.btnNotAMovie.Name = "btnNotAMovie";
            this.btnNotAMovie.Size = new System.Drawing.Size(110, 23);
            this.btnNotAMovie.TabIndex = 2;
            this.btnNotAMovie.Text = "Not A Movie";
            this.btnNotAMovie.UseVisualStyleBackColor = true;
            this.btnNotAMovie.Click += new System.EventHandler(this.Ignore_Click);
            // 
            // btnNewMovie
            // 
            this.btnNewMovie.Location = new System.Drawing.Point(311, 102);
            this.btnNewMovie.Name = "btnNewMovie";
            this.btnNewMovie.Size = new System.Drawing.Size(75, 46);
            this.btnNewMovie.TabIndex = 3;
            this.btnNewMovie.Text = "New Movie";
            this.btnNewMovie.UseVisualStyleBackColor = true;
            this.btnNewMovie.Click += new System.EventHandler(this.btnNewMovie_Click);
            // 
            // lnkOpenFolder
            // 
            this.lnkOpenFolder.AutoSize = true;
            this.lnkOpenFolder.Location = new System.Drawing.Point(17, 162);
            this.lnkOpenFolder.Name = "lnkOpenFolder";
            this.lnkOpenFolder.Size = new System.Drawing.Size(118, 13);
            this.lnkOpenFolder.TabIndex = 5;
            this.lnkOpenFolder.TabStop = true;
            this.lnkOpenFolder.Text = "Open Containing Folder";
            this.lnkOpenFolder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkOpenLeftFolder_LinkClicked);
            // 
            // lblNameRight
            // 
            this.lblNameRight.AutoSize = true;
            this.lblNameRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNameRight.Location = new System.Drawing.Point(11, 162);
            this.lblNameRight.Name = "lblNameRight";
            this.lblNameRight.Size = new System.Drawing.Size(0, 13);
            this.lblNameRight.TabIndex = 14;
            // 
            // txtNameLeft
            // 
            this.txtNameLeft.AutoSize = true;
            this.txtNameLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNameLeft.Location = new System.Drawing.Point(11, 33);
            this.txtNameLeft.Name = "txtNameLeft";
            this.txtNameLeft.Size = new System.Drawing.Size(0, 13);
            this.txtNameLeft.TabIndex = 12;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(11, 9);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(70, 13);
            this.label10.TabIndex = 16;
            this.label10.Text = "Reviewing:";
            // 
            // lblSourceFileName
            // 
            this.lblSourceFileName.AutoSize = true;
            this.lblSourceFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSourceFileName.Location = new System.Drawing.Point(29, 22);
            this.lblSourceFileName.Name = "lblSourceFileName";
            this.lblSourceFileName.Size = new System.Drawing.Size(0, 13);
            this.lblSourceFileName.TabIndex = 17;
            // 
            // cbShows
            // 
            this.cbShows.FormattingEnabled = true;
            this.cbShows.Location = new System.Drawing.Point(32, 59);
            this.cbShows.Name = "cbShows";
            this.cbShows.Size = new System.Drawing.Size(273, 21);
            this.cbShows.TabIndex = 18;
            // 
            // LinkMovie
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnNotAMovie;
            this.ClientSize = new System.Drawing.Size(390, 193);
            this.ControlBox = false;
            this.Controls.Add(this.cbShows);
            this.Controls.Add(this.lblSourceFileName);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.lblNameRight);
            this.Controls.Add(this.txtNameLeft);
            this.Controls.Add(this.lnkOpenFolder);
            this.Controls.Add(this.btnNewMovie);
            this.Controls.Add(this.btnNotAMovie);
            this.Controls.Add(this.btnUseSelectedMovie);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LinkMovie";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose Movie";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnUseSelectedMovie;
        private System.Windows.Forms.Button btnNotAMovie;
        private System.Windows.Forms.Button btnNewMovie;
        private System.Windows.Forms.LinkLabel lnkOpenFolder;
        private System.Windows.Forms.Label lblNameRight;
        private System.Windows.Forms.Label txtNameLeft;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblSourceFileName;
        private System.Windows.Forms.ComboBox cbShows;
    }
}
