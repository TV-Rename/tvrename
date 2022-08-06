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
            this.components = new System.ComponentModel.Container();
            this.bnCancel = new System.Windows.Forms.Button();
            this.pbProgressBar = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCurrent = new System.Windows.Forms.Label();
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            //
            // bnCancel
            //
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnCancel.Location = new System.Drawing.Point(186, 71);
            this.bnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(88, 27);
            this.bnCancel.TabIndex = 0;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            //
            // pbProgressBar
            //
            this.pbProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgressBar.Location = new System.Drawing.Point(14, 39);
            this.pbProgressBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pbProgressBar.Name = "pbProgressBar";
            this.pbProgressBar.Size = new System.Drawing.Size(406, 17);
            this.pbProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbProgressBar.TabIndex = 1;
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 10);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Currently Downloading:";
            //
            // txtCurrent
            //
            this.txtCurrent.AutoSize = true;
            this.txtCurrent.Location = new System.Drawing.Point(156, 10);
            this.txtCurrent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtCurrent.Name = "txtCurrent";
            this.txtCurrent.Size = new System.Drawing.Size(22, 15);
            this.txtCurrent.TabIndex = 2;
            this.txtCurrent.Text = "---";
            //
            // tmrUpdate
            //
            this.tmrUpdate.Enabled = true;
            this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);
            //
            // DownloadProgress
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 111);
            this.Controls.Add(this.txtCurrent);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pbProgressBar);
            this.Controls.Add(this.bnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DownloadProgress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Download Progress";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.ProgressBar pbProgressBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label txtCurrent;
        private System.Windows.Forms.Timer tmrUpdate;
    }
}
