namespace TVRename.Windows.Forms
{
    partial class MissingFolderAction
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
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonRetry = new System.Windows.Forms.Button();
            this.buttonCreate = new System.Windows.Forms.Button();
            this.buttonIgnoreAlways = new System.Windows.Forms.Button();
            this.buttonIgnoreOnce = new System.Windows.Forms.Button();
            this.labelFolder = new System.Windows.Forms.Label();
            this.labelTitleFolder = new System.Windows.Forms.Label();
            this.labelSeason = new System.Windows.Forms.Label();
            this.labelTitleSeason = new System.Windows.Forms.Label();
            this.labelShow = new System.Windows.Forms.Label();
            this.labelTitleShow = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowse.Location = new System.Drawing.Point(410, 50);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(92, 23);
            this.buttonBrowse.TabIndex = 10;
            this.buttonBrowse.Text = "&Browse...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(410, 81);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(92, 23);
            this.buttonCancel.TabIndex = 11;
            this.buttonCancel.Text = "Canc&el";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonRetry
            // 
            this.buttonRetry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRetry.Location = new System.Drawing.Point(309, 81);
            this.buttonRetry.Name = "buttonRetry";
            this.buttonRetry.Size = new System.Drawing.Size(92, 23);
            this.buttonRetry.TabIndex = 9;
            this.buttonRetry.Text = "&Retry";
            this.buttonRetry.UseVisualStyleBackColor = true;
            this.buttonRetry.Click += new System.EventHandler(this.buttonRetry_Click);
            // 
            // buttonCreate
            // 
            this.buttonCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCreate.Location = new System.Drawing.Point(208, 81);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Size = new System.Drawing.Size(92, 23);
            this.buttonCreate.TabIndex = 8;
            this.buttonCreate.Text = "&Create";
            this.buttonCreate.UseVisualStyleBackColor = true;
            this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
            // 
            // buttonIgnoreAlways
            // 
            this.buttonIgnoreAlways.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonIgnoreAlways.Location = new System.Drawing.Point(110, 81);
            this.buttonIgnoreAlways.Name = "buttonIgnoreAlways";
            this.buttonIgnoreAlways.Size = new System.Drawing.Size(92, 23);
            this.buttonIgnoreAlways.TabIndex = 7;
            this.buttonIgnoreAlways.Text = "Ignore &Always";
            this.buttonIgnoreAlways.UseVisualStyleBackColor = true;
            this.buttonIgnoreAlways.Click += new System.EventHandler(this.buttonIgnoreAlways_Click);
            // 
            // buttonIgnoreOnce
            // 
            this.buttonIgnoreOnce.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonIgnoreOnce.Location = new System.Drawing.Point(12, 81);
            this.buttonIgnoreOnce.Name = "buttonIgnoreOnce";
            this.buttonIgnoreOnce.Size = new System.Drawing.Size(92, 23);
            this.buttonIgnoreOnce.TabIndex = 6;
            this.buttonIgnoreOnce.Text = "Ignore &Once";
            this.buttonIgnoreOnce.UseVisualStyleBackColor = true;
            this.buttonIgnoreOnce.Click += new System.EventHandler(this.buttonIgnoreOnce_Click);
            // 
            // labelFolder
            // 
            this.labelFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFolder.Location = new System.Drawing.Point(70, 55);
            this.labelFolder.Name = "labelFolder";
            this.labelFolder.Size = new System.Drawing.Size(334, 13);
            this.labelFolder.TabIndex = 5;
            this.labelFolder.Text = "---";
            // 
            // labelTitleFolder
            // 
            this.labelTitleFolder.AutoSize = true;
            this.labelTitleFolder.Location = new System.Drawing.Point(12, 55);
            this.labelTitleFolder.Name = "labelTitleFolder";
            this.labelTitleFolder.Size = new System.Drawing.Size(39, 13);
            this.labelTitleFolder.TabIndex = 4;
            this.labelTitleFolder.Text = "Folder:";
            // 
            // labelSeason
            // 
            this.labelSeason.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSeason.Location = new System.Drawing.Point(70, 31);
            this.labelSeason.Name = "labelSeason";
            this.labelSeason.Size = new System.Drawing.Size(432, 13);
            this.labelSeason.TabIndex = 3;
            this.labelSeason.Text = "---";
            // 
            // labelTitleSeason
            // 
            this.labelTitleSeason.AutoSize = true;
            this.labelTitleSeason.Location = new System.Drawing.Point(12, 31);
            this.labelTitleSeason.Name = "labelTitleSeason";
            this.labelTitleSeason.Size = new System.Drawing.Size(46, 13);
            this.labelTitleSeason.TabIndex = 2;
            this.labelTitleSeason.Text = "Season:";
            // 
            // labelShow
            // 
            this.labelShow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelShow.Location = new System.Drawing.Point(70, 9);
            this.labelShow.Name = "labelShow";
            this.labelShow.Size = new System.Drawing.Size(432, 13);
            this.labelShow.TabIndex = 1;
            this.labelShow.Text = "---";
            // 
            // labelTitleShow
            // 
            this.labelTitleShow.AutoSize = true;
            this.labelTitleShow.Location = new System.Drawing.Point(12, 9);
            this.labelTitleShow.Name = "labelTitleShow";
            this.labelTitleShow.Size = new System.Drawing.Size(37, 13);
            this.labelTitleShow.TabIndex = 0;
            this.labelTitleShow.Text = "Show:";
            // 
            // MissingFolderAction
            // 
            this.AcceptButton = this.buttonIgnoreOnce;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(514, 116);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonRetry);
            this.Controls.Add(this.buttonCreate);
            this.Controls.Add(this.buttonIgnoreAlways);
            this.Controls.Add(this.buttonIgnoreOnce);
            this.Controls.Add(this.labelFolder);
            this.Controls.Add(this.labelTitleFolder);
            this.Controls.Add(this.labelSeason);
            this.Controls.Add(this.labelTitleSeason);
            this.Controls.Add(this.labelShow);
            this.Controls.Add(this.labelTitleShow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MissingFolderAction";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Missing Folder";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonRetry;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.Button buttonIgnoreAlways;
        private System.Windows.Forms.Button buttonIgnoreOnce;
        private System.Windows.Forms.Label labelFolder;
        private System.Windows.Forms.Label labelTitleFolder;
        private System.Windows.Forms.Label labelSeason;
        private System.Windows.Forms.Label labelTitleSeason;
        private System.Windows.Forms.Label labelShow;
        private System.Windows.Forms.Label labelTitleShow;
    }
}
