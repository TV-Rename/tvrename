//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//


namespace TVRename
{
    partial class TorrentMatch
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
            System.ComponentModel.ComponentResourceManager resources = (new System.ComponentModel.ComponentResourceManager(typeof(TorrentMatch)));
            this.rbBTRenameFiles = (new System.Windows.Forms.RadioButton());
            this.rbBTCopyTo = (new System.Windows.Forms.RadioButton());
            this.bnBTSecondOpen = (new System.Windows.Forms.Button());
            this.bnBTOpenFolder = (new System.Windows.Forms.Button());
            this.tmatchTree = (new System.Windows.Forms.TreeView());
            this.bnGo = (new System.Windows.Forms.Button());
            this.bnBTSecondBrowse = (new System.Windows.Forms.Button());
            this.bnBrowseFolder = (new System.Windows.Forms.Button());
            this.txtBTSecondLocation = (new System.Windows.Forms.TextBox());
            this.txtFolder = (new System.Windows.Forms.TextBox());
            this.bnBrowseTorrent = (new System.Windows.Forms.Button());
            this.label14 = (new System.Windows.Forms.Label());
            this.label3 = (new System.Windows.Forms.Label());
            this.txtTorrentFile = (new System.Windows.Forms.TextBox());
            this.label4 = (new System.Windows.Forms.Label());
            this.bnClose = (new System.Windows.Forms.Button());
            this.folderBrowser = (new System.Windows.Forms.FolderBrowserDialog());
            this.openFile = (new System.Windows.Forms.OpenFileDialog());
            this.SuspendLayout();
            // 
            // rbBTRenameFiles
            // 
            this.rbBTRenameFiles.AutoSize = true;
            this.rbBTRenameFiles.Checked = true;
            this.rbBTRenameFiles.Location = new System.Drawing.Point(113, 67);
            this.rbBTRenameFiles.Name = "rbBTRenameFiles";
            this.rbBTRenameFiles.Size = new System.Drawing.Size(86, 17);
            this.rbBTRenameFiles.TabIndex = 24;
            this.rbBTRenameFiles.TabStop = true;
            this.rbBTRenameFiles.Text = "Rename files";
            this.rbBTRenameFiles.UseVisualStyleBackColor = true;
            this.rbBTRenameFiles.CheckedChanged += new System.EventHandler(rbBTRenameFiles_CheckedChanged);
            // 
            // rbBTCopyTo
            // 
            this.rbBTCopyTo.AutoSize = true;
            this.rbBTCopyTo.Location = new System.Drawing.Point(205, 67);
            this.rbBTCopyTo.Name = "rbBTCopyTo";
            this.rbBTCopyTo.Size = new System.Drawing.Size(65, 17);
            this.rbBTCopyTo.TabIndex = 25;
            this.rbBTCopyTo.Text = "Copy To";
            this.rbBTCopyTo.UseVisualStyleBackColor = true;
            this.rbBTCopyTo.CheckedChanged += new System.EventHandler(rbBTCopyTo_CheckedChanged);
            // 
            // bnBTSecondOpen
            // 
            this.bnBTSecondOpen.Location = new System.Drawing.Point(502, 91);
            this.bnBTSecondOpen.Name = "bnBTSecondOpen";
            this.bnBTSecondOpen.Size = new System.Drawing.Size(75, 23);
            this.bnBTSecondOpen.TabIndex = 21;
            this.bnBTSecondOpen.Text = "&Open";
            this.bnBTSecondOpen.UseVisualStyleBackColor = true;
            this.bnBTSecondOpen.Click += new System.EventHandler(bnBTSecondOpen_Click);
            // 
            // bnBTOpenFolder
            // 
            this.bnBTOpenFolder.Location = new System.Drawing.Point(502, 39);
            this.bnBTOpenFolder.Name = "bnBTOpenFolder";
            this.bnBTOpenFolder.Size = new System.Drawing.Size(75, 23);
            this.bnBTOpenFolder.TabIndex = 20;
            this.bnBTOpenFolder.Text = "&Open";
            this.bnBTOpenFolder.UseVisualStyleBackColor = true;
            this.bnBTOpenFolder.Click += new System.EventHandler(bnBTOpenFolder_Click);
            // 
            // tmatchTree
            // 
            this.tmatchTree.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            this.tmatchTree.Location = new System.Drawing.Point(14, 148);
            this.tmatchTree.Name = "tmatchTree";
            this.tmatchTree.Size = new System.Drawing.Size(565, 220);
            this.tmatchTree.TabIndex = 23;
            // 
            // bnGo
            // 
            this.bnGo.Location = new System.Drawing.Point(113, 119);
            this.bnGo.Name = "bnGo";
            this.bnGo.Size = new System.Drawing.Size(75, 23);
            this.bnGo.TabIndex = 22;
            this.bnGo.Text = "&Go";
            this.bnGo.UseVisualStyleBackColor = true;
            this.bnGo.Click += new System.EventHandler(bnGo_Click);
            // 
            // bnBTSecondBrowse
            // 
            this.bnBTSecondBrowse.Location = new System.Drawing.Point(421, 91);
            this.bnBTSecondBrowse.Name = "bnBTSecondBrowse";
            this.bnBTSecondBrowse.Size = new System.Drawing.Size(75, 23);
            this.bnBTSecondBrowse.TabIndex = 19;
            this.bnBTSecondBrowse.Text = "B&rowse...";
            this.bnBTSecondBrowse.UseVisualStyleBackColor = true;
            this.bnBTSecondBrowse.Click += new System.EventHandler(bnBTSecondBrowse_Click);
            // 
            // bnBrowseFolder
            // 
            this.bnBrowseFolder.Location = new System.Drawing.Point(421, 39);
            this.bnBrowseFolder.Name = "bnBrowseFolder";
            this.bnBrowseFolder.Size = new System.Drawing.Size(75, 23);
            this.bnBrowseFolder.TabIndex = 18;
            this.bnBrowseFolder.Text = "B&rowse...";
            this.bnBrowseFolder.UseVisualStyleBackColor = true;
            this.bnBrowseFolder.Click += new System.EventHandler(bnBrowseFolder_Click);
            // 
            // txtBTSecondLocation
            // 
            this.txtBTSecondLocation.Location = new System.Drawing.Point(113, 93);
            this.txtBTSecondLocation.Name = "txtBTSecondLocation";
            this.txtBTSecondLocation.Size = new System.Drawing.Size(296, 20);
            this.txtBTSecondLocation.TabIndex = 16;
            // 
            // txtFolder
            // 
            this.txtFolder.Location = new System.Drawing.Point(113, 41);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(296, 20);
            this.txtFolder.TabIndex = 17;
            // 
            // bnBrowseTorrent
            // 
            this.bnBrowseTorrent.Location = new System.Drawing.Point(421, 10);
            this.bnBrowseTorrent.Name = "bnBrowseTorrent";
            this.bnBrowseTorrent.Size = new System.Drawing.Size(75, 23);
            this.bnBrowseTorrent.TabIndex = 13;
            this.bnBrowseTorrent.Text = "&Browse...";
            this.bnBrowseTorrent.UseVisualStyleBackColor = true;
            this.bnBrowseTorrent.Click += new System.EventHandler(bnBrowseTorrent_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(63, 69);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(40, 13);
            this.label14.TabIndex = 14;
            this.label14.Text = "Action:";
            this.label14.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(63, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "&Folder:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtTorrentFile
            // 
            this.txtTorrentFile.Location = new System.Drawing.Point(113, 12);
            this.txtTorrentFile.Name = "txtTorrentFile";
            this.txtTorrentFile.Size = new System.Drawing.Size(296, 20);
            this.txtTorrentFile.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(43, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = ".&torrent file:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // bnClose
            // 
            this.bnClose.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.bnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnClose.Location = new System.Drawing.Point(504, 374);
            this.bnClose.Name = "bnClose";
            this.bnClose.Size = new System.Drawing.Size(75, 23);
            this.bnClose.TabIndex = 26;
            this.bnClose.Text = "Close";
            this.bnClose.UseVisualStyleBackColor = true;
            this.bnClose.Click += new System.EventHandler(bnClose_Click);
            // 
            // folderBrowser
            // 
            this.folderBrowser.ShowNewFolderButton = false;
            // 
            // openFile
            // 
            this.openFile.Filter = "Torrent files (*.torrent)|*.torrent|All files (*.*)|*.*";
            // 
            // TorrentMatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnClose;
            this.ClientSize = new System.Drawing.Size(591, 409);
            this.Controls.Add(this.bnClose);
            this.Controls.Add(this.rbBTRenameFiles);
            this.Controls.Add(this.rbBTCopyTo);
            this.Controls.Add(this.bnBTSecondOpen);
            this.Controls.Add(this.bnBTOpenFolder);
            this.Controls.Add(this.tmatchTree);
            this.Controls.Add(this.bnGo);
            this.Controls.Add(this.bnBTSecondBrowse);
            this.Controls.Add(this.bnBrowseFolder);
            this.Controls.Add(this.txtBTSecondLocation);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.bnBrowseTorrent);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtTorrentFile);
            this.Controls.Add(this.label4);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TorrentMatch";
            this.ShowInTaskbar = false;
            this.Text = "Torrent Match";
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.OpenFileDialog openFile;

        private System.Windows.Forms.RadioButton rbBTRenameFiles;
        private System.Windows.Forms.RadioButton rbBTCopyTo;
        private System.Windows.Forms.Button bnBTSecondOpen;
        private System.Windows.Forms.Button bnBTOpenFolder;
        private System.Windows.Forms.TreeView tmatchTree;
        private System.Windows.Forms.Button bnGo;
        private System.Windows.Forms.Button bnBTSecondBrowse;
        private System.Windows.Forms.Button bnBrowseFolder;
        private System.Windows.Forms.TextBox txtBTSecondLocation;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Button bnBrowseTorrent;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTorrentFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bnClose;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
    }
}