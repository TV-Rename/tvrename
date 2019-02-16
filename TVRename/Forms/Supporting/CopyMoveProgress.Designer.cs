//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename
{
    partial class CopyMoveProgress
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
            this.copyTimer.Stop();
            this.diskSpaceTimer.Stop();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (disposing && (copyTimer != null))
            {
                copyTimer.Dispose();
            }
            if (disposing && (diskSpaceTimer != null))
            {
                diskSpaceTimer.Dispose();
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CopyMoveProgress));
            this.pbFile = new System.Windows.Forms.ProgressBar();
            this.pbGroup = new System.Windows.Forms.ProgressBar();
            this.bnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFilename = new System.Windows.Forms.Label();
            this.pbDiskSpace = new System.Windows.Forms.ProgressBar();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDiskSpace = new System.Windows.Forms.Label();
            this.txtTotal = new System.Windows.Forms.Label();
            this.txtFile = new System.Windows.Forms.Label();
            this.copyTimer = new System.Windows.Forms.Timer(this.components);
            this.cbPause = new System.Windows.Forms.CheckBox();
            this.lvProgress = new TVRename.ListViewFlickerFree();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.diskSpaceTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // pbFile
            // 
            this.pbFile.Location = new System.Drawing.Point(51, 34);
            this.pbFile.Maximum = 1000;
            this.pbFile.Name = "pbFile";
            this.pbFile.Size = new System.Drawing.Size(242, 23);
            this.pbFile.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbFile.TabIndex = 6;
            // 
            // pbGroup
            // 
            this.pbGroup.Location = new System.Drawing.Point(51, 63);
            this.pbGroup.Maximum = 1000;
            this.pbGroup.Name = "pbGroup";
            this.pbGroup.Size = new System.Drawing.Size(242, 23);
            this.pbGroup.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbGroup.TabIndex = 9;
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.Location = new System.Drawing.Point(280, 260);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 2;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "File:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Total:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Filename:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtFilename
            // 
            this.txtFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilename.Location = new System.Drawing.Point(65, 9);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.Size = new System.Drawing.Size(288, 16);
            this.txtFilename.TabIndex = 4;
            this.txtFilename.UseMnemonic = false;
            // 
            // pbDiskSpace
            // 
            this.pbDiskSpace.Location = new System.Drawing.Point(51, 92);
            this.pbDiskSpace.Maximum = 1000;
            this.pbDiskSpace.Name = "pbDiskSpace";
            this.pbDiskSpace.Size = new System.Drawing.Size(243, 23);
            this.pbDiskSpace.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbDiskSpace.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Disk:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtDiskSpace
            // 
            this.txtDiskSpace.AutoSize = true;
            this.txtDiskSpace.Location = new System.Drawing.Point(301, 96);
            this.txtDiskSpace.Name = "txtDiskSpace";
            this.txtDiskSpace.Size = new System.Drawing.Size(55, 13);
            this.txtDiskSpace.TabIndex = 13;
            this.txtDiskSpace.Text = "--- GB free";
            this.txtDiskSpace.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtTotal
            // 
            this.txtTotal.AutoSize = true;
            this.txtTotal.Location = new System.Drawing.Point(300, 67);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.Size = new System.Drawing.Size(53, 13);
            this.txtTotal.TabIndex = 10;
            this.txtTotal.Text = "---% Done";
            this.txtTotal.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtFile
            // 
            this.txtFile.AutoSize = true;
            this.txtFile.Location = new System.Drawing.Point(301, 38);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(53, 13);
            this.txtFile.TabIndex = 7;
            this.txtFile.Text = "---% Done";
            this.txtFile.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // copyTimer
            // 
            this.copyTimer.Interval = 50;
            this.copyTimer.Tick += new System.EventHandler(this.copyTimer_Tick);
            // 
            // cbPause
            // 
            this.cbPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbPause.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbPause.Location = new System.Drawing.Point(199, 260);
            this.cbPause.Name = "cbPause";
            this.cbPause.Size = new System.Drawing.Size(75, 23);
            this.cbPause.TabIndex = 1;
            this.cbPause.Text = "Pause";
            this.cbPause.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbPause.UseVisualStyleBackColor = true;
            this.cbPause.CheckedChanged += new System.EventHandler(this.cbPause_CheckedChanged);
            // 
            // lvProgress
            // 
            this.lvProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvProgress.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvProgress.FullRowSelect = true;
            this.lvProgress.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvProgress.Location = new System.Drawing.Point(13, 125);
            this.lvProgress.MultiSelect = false;
            this.lvProgress.Name = "lvProgress";
            this.lvProgress.ShowItemToolTips = true;
            this.lvProgress.Size = new System.Drawing.Size(343, 123);
            this.lvProgress.TabIndex = 0;
            this.lvProgress.UseCompatibleStateImageBehavior = false;
            this.lvProgress.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Action";
            this.columnHeader1.Width = 78;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Item";
            this.columnHeader2.Width = 256;
            // 
            // diskSpaceTimer
            // 
            this.diskSpaceTimer.Interval = 1000;
            this.diskSpaceTimer.Tick += new System.EventHandler(this.diskSpaceTimer_Tick);
            // 
            // CopyMoveProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 291);
            this.Controls.Add(this.lvProgress);
            this.Controls.Add(this.cbPause);
            this.Controls.Add(this.txtFile);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.txtDiskSpace);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtFilename);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.pbDiskSpace);
            this.Controls.Add(this.pbGroup);
            this.Controls.Add(this.pbFile);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(375, 9999);
            this.MinimumSize = new System.Drawing.Size(375, 318);
            this.Name = "CopyMoveProgress";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Progress";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbDiskSpace;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label txtDiskSpace;
        private System.Windows.Forms.Label txtTotal;
        private System.Windows.Forms.Label txtFile;
        private System.Windows.Forms.Timer copyTimer;
        private System.Windows.Forms.CheckBox cbPause;

        private System.Windows.Forms.ProgressBar pbFile;
        private System.Windows.Forms.ProgressBar pbGroup;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label txtFilename;
        private ListViewFlickerFree lvProgress;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Timer diskSpaceTimer;
    }
}
