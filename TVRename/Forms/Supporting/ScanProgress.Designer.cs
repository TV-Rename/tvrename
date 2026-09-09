//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//


namespace TVRename
{
    partial class ScanProgress
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScanProgress));
            bnCancel = new System.Windows.Forms.Button();
            lbMediaLibrary = new System.Windows.Forms.Label();
            pbMediaLib = new System.Windows.Forms.ProgressBar();
            lbSearchLocally = new System.Windows.Forms.Label();
            pbLocalSearch = new System.Windows.Forms.ProgressBar();
            lbSearchRSS = new System.Windows.Forms.Label();
            pbRSS = new System.Windows.Forms.ProgressBar();
            lbCheckDownloading = new System.Windows.Forms.Label();
            pbDownloading = new System.Windows.Forms.ProgressBar();
            timer1 = new System.Windows.Forms.Timer(components);
            pbDownloadFolder = new System.Windows.Forms.ProgressBar();
            lbDownloadFolder = new System.Windows.Forms.Label();
            lblMessage = new System.Windows.Forms.Label();
            pbBulkAutoAdd = new System.Windows.Forms.ProgressBar();
            lbBulkAutoAdd = new System.Windows.Forms.Label();
            lblDetail = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // bnCancel
            // 
            bnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            bnCancel.Location = new System.Drawing.Point(332, 179);
            bnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bnCancel.Name = "bnCancel";
            bnCancel.Size = new System.Drawing.Size(88, 27);
            bnCancel.TabIndex = 0;
            bnCancel.Text = "Cancel";
            bnCancel.UseVisualStyleBackColor = true;
            bnCancel.Click += bnCancel_Click;
            // 
            // lbMediaLibrary
            // 
            lbMediaLibrary.AutoSize = true;
            lbMediaLibrary.Location = new System.Drawing.Point(14, 35);
            lbMediaLibrary.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbMediaLibrary.Name = "lbMediaLibrary";
            lbMediaLibrary.Size = new System.Drawing.Size(115, 15);
            lbMediaLibrary.TabIndex = 1;
            lbMediaLibrary.Text = "Media Library Check";
            // 
            // pbMediaLib
            // 
            pbMediaLib.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pbMediaLib.Location = new System.Drawing.Point(164, 35);
            pbMediaLib.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pbMediaLib.Name = "pbMediaLib";
            pbMediaLib.Size = new System.Drawing.Size(255, 15);
            pbMediaLib.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            pbMediaLib.TabIndex = 2;
            // 
            // lbSearchLocally
            // 
            lbSearchLocally.AutoSize = true;
            lbSearchLocally.Location = new System.Drawing.Point(14, 76);
            lbSearchLocally.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbSearchLocally.Name = "lbSearchLocally";
            lbSearchLocally.Size = new System.Drawing.Size(82, 15);
            lbSearchLocally.TabIndex = 1;
            lbSearchLocally.Text = "Search Locally";
            // 
            // pbLocalSearch
            // 
            pbLocalSearch.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pbLocalSearch.Location = new System.Drawing.Point(164, 76);
            pbLocalSearch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pbLocalSearch.Name = "pbLocalSearch";
            pbLocalSearch.Size = new System.Drawing.Size(255, 15);
            pbLocalSearch.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            pbLocalSearch.TabIndex = 2;
            // 
            // lbSearchRSS
            // 
            lbSearchRSS.AutoSize = true;
            lbSearchRSS.Location = new System.Drawing.Point(14, 120);
            lbSearchRSS.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbSearchRSS.Name = "lbSearchRSS";
            lbSearchRSS.Size = new System.Drawing.Size(69, 15);
            lbSearchRSS.TabIndex = 1;
            lbSearchRSS.Text = "Search Web";
            // 
            // pbRSS
            // 
            pbRSS.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pbRSS.Location = new System.Drawing.Point(164, 120);
            pbRSS.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pbRSS.Name = "pbRSS";
            pbRSS.Size = new System.Drawing.Size(255, 15);
            pbRSS.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            pbRSS.TabIndex = 2;
            // 
            // lbCheckDownloading
            // 
            lbCheckDownloading.AutoSize = true;
            lbCheckDownloading.Location = new System.Drawing.Point(14, 98);
            lbCheckDownloading.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbCheckDownloading.Name = "lbCheckDownloading";
            lbCheckDownloading.Size = new System.Drawing.Size(116, 15);
            lbCheckDownloading.TabIndex = 1;
            lbCheckDownloading.Text = "Search Downloading";
            // 
            // pbDownloading
            // 
            pbDownloading.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pbDownloading.Location = new System.Drawing.Point(164, 98);
            pbDownloading.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pbDownloading.Name = "pbDownloading";
            pbDownloading.Size = new System.Drawing.Size(255, 15);
            pbDownloading.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            pbDownloading.TabIndex = 2;
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // pbDownloadFolder
            // 
            pbDownloadFolder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pbDownloadFolder.Location = new System.Drawing.Point(164, 55);
            pbDownloadFolder.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pbDownloadFolder.Name = "pbDownloadFolder";
            pbDownloadFolder.Size = new System.Drawing.Size(255, 15);
            pbDownloadFolder.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            pbDownloadFolder.TabIndex = 4;
            // 
            // lbDownloadFolder
            // 
            lbDownloadFolder.AutoSize = true;
            lbDownloadFolder.Location = new System.Drawing.Point(14, 55);
            lbDownloadFolder.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbDownloadFolder.Name = "lbDownloadFolder";
            lbDownloadFolder.Size = new System.Drawing.Size(124, 15);
            lbDownloadFolder.TabIndex = 3;
            lbDownloadFolder.Text = "Download Area Check";
            // 
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.ForeColor = System.Drawing.SystemColors.ControlDark;
            lblMessage.Location = new System.Drawing.Point(21, 149);
            lblMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new System.Drawing.Size(0, 15);
            lblMessage.TabIndex = 5;
            // 
            // pbBulkAutoAdd
            // 
            pbBulkAutoAdd.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pbBulkAutoAdd.Location = new System.Drawing.Point(164, 13);
            pbBulkAutoAdd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pbBulkAutoAdd.Name = "pbBulkAutoAdd";
            pbBulkAutoAdd.Size = new System.Drawing.Size(255, 15);
            pbBulkAutoAdd.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            pbBulkAutoAdd.TabIndex = 7;
            // 
            // lbBulkAutoAdd
            // 
            lbBulkAutoAdd.AutoSize = true;
            lbBulkAutoAdd.Location = new System.Drawing.Point(14, 13);
            lbBulkAutoAdd.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbBulkAutoAdd.Name = "lbBulkAutoAdd";
            lbBulkAutoAdd.Size = new System.Drawing.Size(116, 15);
            lbBulkAutoAdd.TabIndex = 6;
            lbBulkAutoAdd.Text = "Scan Library for New";
            // 
            // lblDetail
            // 
            lblDetail.AutoSize = true;
            lblDetail.ForeColor = System.Drawing.SystemColors.ControlDark;
            lblDetail.Location = new System.Drawing.Point(21, 179);
            lblDetail.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDetail.Name = "lblDetail";
            lblDetail.Size = new System.Drawing.Size(0, 15);
            lblDetail.TabIndex = 8;
            // 
            // ScanProgress
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = bnCancel;
            ClientSize = new System.Drawing.Size(434, 219);
            Controls.Add(lblDetail);
            Controls.Add(pbBulkAutoAdd);
            Controls.Add(lbBulkAutoAdd);
            Controls.Add(lblMessage);
            Controls.Add(pbDownloadFolder);
            Controls.Add(lbDownloadFolder);
            Controls.Add(pbDownloading);
            Controls.Add(lbCheckDownloading);
            Controls.Add(pbRSS);
            Controls.Add(lbSearchRSS);
            Controls.Add(pbLocalSearch);
            Controls.Add(lbSearchLocally);
            Controls.Add(pbMediaLib);
            Controls.Add(lbMediaLibrary);
            Controls.Add(bnCancel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ScanProgress";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Scan Progress";
            Load += ScanProgress_Load;
            ResumeLayout(false);
            PerformLayout();

        }


        #endregion

        private System.Windows.Forms.Timer timer1;

        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Label lbMediaLibrary;
        private System.Windows.Forms.ProgressBar pbMediaLib;
        private System.Windows.Forms.Label lbSearchLocally;
        private System.Windows.Forms.ProgressBar pbLocalSearch;
        private System.Windows.Forms.Label lbSearchRSS;
        private System.Windows.Forms.ProgressBar pbRSS;
        private System.Windows.Forms.Label lbCheckDownloading;
        private System.Windows.Forms.ProgressBar pbDownloading;
        private System.Windows.Forms.ProgressBar pbDownloadFolder;
        private System.Windows.Forms.Label lbDownloadFolder;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.ProgressBar pbBulkAutoAdd;
        private System.Windows.Forms.Label lbBulkAutoAdd;
        private System.Windows.Forms.Label lblDetail;
    }
}
