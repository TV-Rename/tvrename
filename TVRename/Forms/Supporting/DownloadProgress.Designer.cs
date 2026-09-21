//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//


namespace TVRename
{
    partial class DownloadProgress
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
            components = new System.ComponentModel.Container();
            bnCancel = new System.Windows.Forms.Button();
            pbProgressBar = new System.Windows.Forms.ProgressBar();
            label2 = new System.Windows.Forms.Label();
            txtCurrent = new System.Windows.Forms.Label();
            tmrUpdate = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // bnCancel
            // 
            bnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            bnCancel.Location = new System.Drawing.Point(186, 71);
            bnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bnCancel.Name = "bnCancel";
            bnCancel.Size = new System.Drawing.Size(88, 27);
            bnCancel.TabIndex = 0;
            bnCancel.Text = "Cancel";
            bnCancel.UseVisualStyleBackColor = true;
            bnCancel.Click += bnCancel_Click;
            // 
            // pbProgressBar
            // 
            pbProgressBar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pbProgressBar.Location = new System.Drawing.Point(14, 39);
            pbProgressBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pbProgressBar.Name = "pbProgressBar";
            pbProgressBar.Size = new System.Drawing.Size(406, 17);
            pbProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            pbProgressBar.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(14, 10);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(133, 15);
            label2.TabIndex = 2;
            label2.Text = "Currently Downloading:";
            // 
            // txtCurrent
            // 
            txtCurrent.AutoSize = true;
            txtCurrent.Location = new System.Drawing.Point(156, 10);
            txtCurrent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            txtCurrent.Name = "txtCurrent";
            txtCurrent.Size = new System.Drawing.Size(22, 15);
            txtCurrent.TabIndex = 2;
            txtCurrent.Text = "---";
            // 
            // tmrUpdate
            // 
            tmrUpdate.Enabled = true;
            tmrUpdate.Tick += tmrUpdate_Tick;
            // 
            // DownloadProgress
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(434, 111);
            Controls.Add(txtCurrent);
            Controls.Add(label2);
            Controls.Add(pbProgressBar);
            Controls.Add(bnCancel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DownloadProgress";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Download Progress";
            Load += DownloadProgress_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.ProgressBar pbProgressBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label txtCurrent;
        private System.Windows.Forms.Timer tmrUpdate;
    }
}
