//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//


namespace TVRename
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MissingFolderAction));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.bnIgnoreOnce = new System.Windows.Forms.Button();
            this.bnIgnoreAlways = new System.Windows.Forms.Button();
            this.bnCreate = new System.Windows.Forms.Button();
            this.txtShow = new System.Windows.Forms.Label();
            this.txtSeason = new System.Windows.Forms.Label();
            this.txtFolder = new System.Windows.Forms.Label();
            this.bnRetry = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnBrowse = new System.Windows.Forms.Button();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Show:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Season:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Folder:";
            // 
            // bnIgnoreOnce
            // 
            this.bnIgnoreOnce.Location = new System.Drawing.Point(15, 84);
            this.bnIgnoreOnce.Name = "bnIgnoreOnce";
            this.bnIgnoreOnce.Size = new System.Drawing.Size(92, 23);
            this.bnIgnoreOnce.TabIndex = 7;
            this.bnIgnoreOnce.Text = "Ig&nore Once";
            this.bnIgnoreOnce.UseVisualStyleBackColor = true;
            this.bnIgnoreOnce.Click += new System.EventHandler(this.bnIgnoreOnce_Click);
            // 
            // bnIgnoreAlways
            // 
            this.bnIgnoreAlways.Location = new System.Drawing.Point(113, 84);
            this.bnIgnoreAlways.Name = "bnIgnoreAlways";
            this.bnIgnoreAlways.Size = new System.Drawing.Size(92, 23);
            this.bnIgnoreAlways.TabIndex = 8;
            this.bnIgnoreAlways.Text = "Ignore &Always";
            this.bnIgnoreAlways.UseVisualStyleBackColor = true;
            this.bnIgnoreAlways.Click += new System.EventHandler(this.bnIgnoreAlways_Click);
            // 
            // bnCreate
            // 
            this.bnCreate.Location = new System.Drawing.Point(211, 84);
            this.bnCreate.Name = "bnCreate";
            this.bnCreate.Size = new System.Drawing.Size(92, 23);
            this.bnCreate.TabIndex = 9;
            this.bnCreate.Text = "&Create";
            this.bnCreate.UseVisualStyleBackColor = true;
            this.bnCreate.Click += new System.EventHandler(this.bnCreate_Click);
            // 
            // txtShow
            // 
            this.txtShow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtShow.Location = new System.Drawing.Point(70, 9);
            this.txtShow.Name = "txtShow";
            this.txtShow.Size = new System.Drawing.Size(431, 13);
            this.txtShow.TabIndex = 1;
            this.txtShow.Text = "---";
            // 
            // txtSeason
            // 
            this.txtSeason.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSeason.Location = new System.Drawing.Point(70, 31);
            this.txtSeason.Name = "txtSeason";
            this.txtSeason.Size = new System.Drawing.Size(431, 13);
            this.txtSeason.TabIndex = 3;
            this.txtSeason.Text = "---";
            // 
            // txtFolder
            // 
            this.txtFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolder.Location = new System.Drawing.Point(70, 55);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(331, 13);
            this.txtFolder.TabIndex = 5;
            this.txtFolder.Text = "---";
            // 
            // bnRetry
            // 
            this.bnRetry.Location = new System.Drawing.Point(309, 84);
            this.bnRetry.Name = "bnRetry";
            this.bnRetry.Size = new System.Drawing.Size(92, 23);
            this.bnRetry.TabIndex = 10;
            this.bnRetry.Text = "&Retry";
            this.bnRetry.UseVisualStyleBackColor = true;
            this.bnRetry.Click += new System.EventHandler(this.bnRetry_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(407, 84);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(92, 23);
            this.bnCancel.TabIndex = 11;
            this.bnCancel.Text = "Canc&el";
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // bnBrowse
            // 
            this.bnBrowse.Location = new System.Drawing.Point(407, 50);
            this.bnBrowse.Name = "bnBrowse";
            this.bnBrowse.Size = new System.Drawing.Size(92, 23);
            this.bnBrowse.TabIndex = 6;
            this.bnBrowse.Text = "&Browse...";
            this.bnBrowse.UseVisualStyleBackColor = true;
            this.bnBrowse.Click += new System.EventHandler(this.bnBrowse_Click);
            // 
            // MissingFolderAction
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(516, 121);
            this.Controls.Add(this.bnBrowse);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnRetry);
            this.Controls.Add(this.bnCreate);
            this.Controls.Add(this.bnIgnoreAlways);
            this.Controls.Add(this.bnIgnoreOnce);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSeason);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtShow);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MissingFolderAction";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Missing Folder";
            this.TopMost = true;
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MissingFolderAction_DragDrop);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.MissingFolderAction_DragOver);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button bnIgnoreOnce;
        private System.Windows.Forms.Button bnIgnoreAlways;
        private System.Windows.Forms.Button bnCreate;
        private System.Windows.Forms.Label txtShow;
        private System.Windows.Forms.Label txtSeason;
        private System.Windows.Forms.Label txtFolder;
        private System.Windows.Forms.Button bnRetry;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Button bnBrowse;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
    }
}
