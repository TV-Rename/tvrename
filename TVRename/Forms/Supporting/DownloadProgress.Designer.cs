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
            bnCancel = new Button();
            pbProgressBar = new ProgressBar();
            label2 = new Label();
            txtCurrent = new Label();
            tmrUpdate = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // bnCancel
            // 
            bnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bnCancel.Location = new Point(186, 71);
            bnCancel.Margin = new Padding(4, 3, 4, 3);
            bnCancel.Name = "bnCancel";
            bnCancel.Size = new Size(88, 27);
            bnCancel.TabIndex = 0;
            bnCancel.Text = "Cancel";
            bnCancel.UseVisualStyleBackColor = true;
            bnCancel.Click += bnCancel_Click;
            // 
            // pbProgressBar
            // 
            pbProgressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pbProgressBar.Location = new Point(14, 39);
            pbProgressBar.Margin = new Padding(4, 3, 4, 3);
            pbProgressBar.Name = "pbProgressBar";
            pbProgressBar.Size = new Size(406, 17);
            pbProgressBar.Style = ProgressBarStyle.Continuous;
            pbProgressBar.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(14, 10);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(133, 15);
            label2.TabIndex = 2;
            label2.Text = "Currently Downloading:";
            // 
            // txtCurrent
            // 
            txtCurrent.AutoSize = true;
            txtCurrent.Location = new Point(156, 10);
            txtCurrent.Margin = new Padding(4, 0, 4, 0);
            txtCurrent.Name = "txtCurrent";
            txtCurrent.Size = new Size(22, 15);
            txtCurrent.TabIndex = 2;
            txtCurrent.Text = "---";
            txtCurrent.Click += txtCurrent_Click;
            // 
            // tmrUpdate
            // 
            tmrUpdate.Enabled = true;
            tmrUpdate.Tick += tmrUpdate_Tick;
            // 
            // DownloadProgress
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(434, 111);
            Controls.Add(txtCurrent);
            Controls.Add(label2);
            Controls.Add(pbProgressBar);
            Controls.Add(bnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DownloadProgress";
            StartPosition = FormStartPosition.CenterParent;
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
