//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//


namespace TVRename
{
    partial class BugReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BugReport));
            this.label7 = new System.Windows.Forms.Label();
            this.cbSettings = new System.Windows.Forms.CheckBox();
            this.cbFOScan = new System.Windows.Forms.CheckBox();
            this.bnCreate = new System.Windows.Forms.Button();
            this.txtEmailText = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.bnClose = new System.Windows.Forms.Button();
            this.cbFolderScan = new System.Windows.Forms.CheckBox();
            this.bnCopy = new System.Windows.Forms.Button();
            this.linkForum = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 13);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Include:";
            // 
            // cbSettings
            // 
            this.cbSettings.AutoSize = true;
            this.cbSettings.Checked = true;
            this.cbSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSettings.Location = new System.Drawing.Point(63, 12);
            this.cbSettings.Name = "cbSettings";
            this.cbSettings.Size = new System.Drawing.Size(88, 17);
            this.cbSettings.TabIndex = 13;
            this.cbSettings.Text = "Settings Files";
            this.cbSettings.UseVisualStyleBackColor = true;
            // 
            // cbFOScan
            // 
            this.cbFOScan.AutoSize = true;
            this.cbFOScan.Checked = true;
            this.cbFOScan.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFOScan.Location = new System.Drawing.Point(157, 12);
            this.cbFOScan.Name = "cbFOScan";
            this.cbFOScan.Size = new System.Drawing.Size(74, 17);
            this.cbFOScan.TabIndex = 14;
            this.cbFOScan.Text = "F&&O Scan";
            this.cbFOScan.UseVisualStyleBackColor = true;
            // 
            // bnCreate
            // 
            this.bnCreate.Location = new System.Drawing.Point(12, 35);
            this.bnCreate.Name = "bnCreate";
            this.bnCreate.Size = new System.Drawing.Size(75, 23);
            this.bnCreate.TabIndex = 16;
            this.bnCreate.Text = "Create";
            this.bnCreate.UseVisualStyleBackColor = true;
            this.bnCreate.Click += new System.EventHandler(this.bnCreate_Click);
            // 
            // txtEmailText
            // 
            this.txtEmailText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmailText.Location = new System.Drawing.Point(15, 64);
            this.txtEmailText.Multiline = true;
            this.txtEmailText.Name = "txtEmailText";
            this.txtEmailText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtEmailText.Size = new System.Drawing.Size(894, 442);
            this.txtEmailText.TabIndex = 18;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.Location = new System.Drawing.Point(12, 509);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(897, 47);
            this.label8.TabIndex = 17;
            this.label8.Text = resources.GetString("label8.Text");
            // 
            // bnClose
            // 
            this.bnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnClose.Location = new System.Drawing.Point(834, 585);
            this.bnClose.Name = "bnClose";
            this.bnClose.Size = new System.Drawing.Size(75, 23);
            this.bnClose.TabIndex = 20;
            this.bnClose.Text = "Close";
            this.bnClose.UseVisualStyleBackColor = true;
            // 
            // cbFolderScan
            // 
            this.cbFolderScan.AutoSize = true;
            this.cbFolderScan.Checked = true;
            this.cbFolderScan.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFolderScan.Location = new System.Drawing.Point(237, 12);
            this.cbFolderScan.Name = "cbFolderScan";
            this.cbFolderScan.Size = new System.Drawing.Size(83, 17);
            this.cbFolderScan.TabIndex = 15;
            this.cbFolderScan.Text = "Folder Scan";
            this.cbFolderScan.UseVisualStyleBackColor = true;
            // 
            // bnCopy
            // 
            this.bnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnCopy.Location = new System.Drawing.Point(12, 585);
            this.bnCopy.Name = "bnCopy";
            this.bnCopy.Size = new System.Drawing.Size(75, 23);
            this.bnCopy.TabIndex = 19;
            this.bnCopy.Text = "Copy";
            this.bnCopy.UseVisualStyleBackColor = true;
            this.bnCopy.Click += new System.EventHandler(this.bnCopy_Click);
            // 
            // linkForum
            // 
            this.linkForum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkForum.AutoSize = true;
            this.linkForum.Location = new System.Drawing.Point(12, 558);
            this.linkForum.Name = "linkForum";
            this.linkForum.Size = new System.Drawing.Size(135, 13);
            this.linkForum.TabIndex = 21;
            this.linkForum.TabStop = true;
            this.linkForum.Text = "TVRename Google Groups";
            this.linkForum.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkForum_LinkClicked);
            // 
            // BugReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnClose;
            this.ClientSize = new System.Drawing.Size(924, 622);
            this.Controls.Add(this.linkForum);
            this.Controls.Add(this.bnCopy);
            this.Controls.Add(this.bnClose);
            this.Controls.Add(this.bnCreate);
            this.Controls.Add(this.cbFolderScan);
            this.Controls.Add(this.cbFOScan);
            this.Controls.Add(this.cbSettings);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtEmailText);
            this.Controls.Add(this.label8);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BugReport";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Bug Report";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox cbSettings;
        private System.Windows.Forms.CheckBox cbFOScan;



        private System.Windows.Forms.Button bnCreate;
        private System.Windows.Forms.TextBox txtEmailText;


        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button bnClose;
        private System.Windows.Forms.CheckBox cbFolderScan;
        private System.Windows.Forms.Button bnCopy;
        private System.Windows.Forms.LinkLabel linkForum;
    }
}