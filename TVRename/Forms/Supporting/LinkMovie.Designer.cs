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
            btnUseSelectedMovie = new System.Windows.Forms.Button();
            btnNotAMovie = new System.Windows.Forms.Button();
            btnNewMovie = new System.Windows.Forms.Button();
            lnkOpenFolder = new System.Windows.Forms.LinkLabel();
            lblNameRight = new System.Windows.Forms.Label();
            txtNameLeft = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            lblSourceFileName = new System.Windows.Forms.Label();
            cbShows = new System.Windows.Forms.ComboBox();
            SuspendLayout();
            // 
            // btnUseSelectedMovie
            // 
            btnUseSelectedMovie.Location = new System.Drawing.Point(363, 58);
            btnUseSelectedMovie.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnUseSelectedMovie.Name = "btnUseSelectedMovie";
            btnUseSelectedMovie.Size = new System.Drawing.Size(88, 53);
            btnUseSelectedMovie.TabIndex = 0;
            btnUseSelectedMovie.Text = "Use";
            btnUseSelectedMovie.UseVisualStyleBackColor = true;
            btnUseSelectedMovie.Click += btnUseSelectedMovie_Click;
            // 
            // btnNotAMovie
            // 
            btnNotAMovie.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnNotAMovie.Location = new System.Drawing.Point(322, 178);
            btnNotAMovie.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnNotAMovie.Name = "btnNotAMovie";
            btnNotAMovie.Size = new System.Drawing.Size(128, 27);
            btnNotAMovie.TabIndex = 2;
            btnNotAMovie.Text = "Not A Movie";
            btnNotAMovie.UseVisualStyleBackColor = true;
            btnNotAMovie.Click += Ignore_Click;
            // 
            // btnNewMovie
            // 
            btnNewMovie.Location = new System.Drawing.Point(363, 118);
            btnNewMovie.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnNewMovie.Name = "btnNewMovie";
            btnNewMovie.Size = new System.Drawing.Size(88, 53);
            btnNewMovie.TabIndex = 3;
            btnNewMovie.Text = "New Movie";
            btnNewMovie.UseVisualStyleBackColor = true;
            btnNewMovie.Click += btnNewMovie_Click;
            // 
            // lnkOpenFolder
            // 
            lnkOpenFolder.AutoSize = true;
            lnkOpenFolder.Location = new System.Drawing.Point(20, 187);
            lnkOpenFolder.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lnkOpenFolder.Name = "lnkOpenFolder";
            lnkOpenFolder.Size = new System.Drawing.Size(134, 15);
            lnkOpenFolder.TabIndex = 5;
            lnkOpenFolder.TabStop = true;
            lnkOpenFolder.Text = "Open Containing Folder";
            lnkOpenFolder.LinkClicked += lnkOpenLeftFolder_LinkClicked;
            // 
            // lblNameRight
            // 
            lblNameRight.AutoSize = true;
            lblNameRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblNameRight.Location = new System.Drawing.Point(13, 187);
            lblNameRight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblNameRight.Name = "lblNameRight";
            lblNameRight.Size = new System.Drawing.Size(0, 13);
            lblNameRight.TabIndex = 14;
            // 
            // txtNameLeft
            // 
            txtNameLeft.AutoSize = true;
            txtNameLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            txtNameLeft.Location = new System.Drawing.Point(13, 38);
            txtNameLeft.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            txtNameLeft.Name = "txtNameLeft";
            txtNameLeft.Size = new System.Drawing.Size(0, 13);
            txtNameLeft.TabIndex = 12;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label10.Location = new System.Drawing.Point(13, 10);
            label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(70, 13);
            label10.TabIndex = 16;
            label10.Text = "Reviewing:";
            // 
            // lblSourceFileName
            // 
            lblSourceFileName.AutoSize = true;
            lblSourceFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblSourceFileName.Location = new System.Drawing.Point(34, 25);
            lblSourceFileName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblSourceFileName.Name = "lblSourceFileName";
            lblSourceFileName.Size = new System.Drawing.Size(0, 13);
            lblSourceFileName.TabIndex = 17;
            // 
            // cbShows
            // 
            cbShows.FormattingEnabled = true;
            cbShows.Location = new System.Drawing.Point(37, 68);
            cbShows.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbShows.Name = "cbShows";
            cbShows.Size = new System.Drawing.Size(318, 23);
            cbShows.TabIndex = 18;
            // 
            // LinkMovie
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnNotAMovie;
            ClientSize = new System.Drawing.Size(455, 223);
            ControlBox = false;
            Controls.Add(cbShows);
            Controls.Add(lblSourceFileName);
            Controls.Add(label10);
            Controls.Add(lblNameRight);
            Controls.Add(txtNameLeft);
            Controls.Add(lnkOpenFolder);
            Controls.Add(btnNewMovie);
            Controls.Add(btnNotAMovie);
            Controls.Add(btnUseSelectedMovie);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LinkMovie";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Choose Movie";
            Load += LinkMovie_Load;
            ResumeLayout(false);
            PerformLayout();
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
