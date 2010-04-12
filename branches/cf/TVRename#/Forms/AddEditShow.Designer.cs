//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//


namespace TVRename
{
    partial class AddEditShow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddEditShow));
            this.txtCustomShowName = new System.Windows.Forms.TextBox();
            this.cbTimeZone = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.bnCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.chkSpecialsCount = new System.Windows.Forms.CheckBox();
            this.chkShowNextAirdate = new System.Windows.Forms.CheckBox();
            this.pnlCF = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.chkFolderPerSeason = new System.Windows.Forms.CheckBox();
            this.txtSeasonFolderName = new System.Windows.Forms.TextBox();
            this.txtBaseFolder = new System.Windows.Forms.TextBox();
            this.bnBrowse = new System.Windows.Forms.Button();
            this.chkAutoFolders = new System.Windows.Forms.CheckBox();
            this.cbDoRenaming = new System.Windows.Forms.CheckBox();
            this.cbDoMissingCheck = new System.Windows.Forms.CheckBox();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.label5 = new System.Windows.Forms.Label();
            this.txtIgnoreSeasons = new System.Windows.Forms.TextBox();
            this.chkDVDOrder = new System.Windows.Forms.CheckBox();
            this.chkForceCheckAll = new System.Windows.Forms.CheckBox();
            this.chkPadTwoDigits = new System.Windows.Forms.CheckBox();
            this.lvSeasonFolders = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.txtSeasonNumber = new System.Windows.Forms.TextBox();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.bnBrowseFolder = new System.Windows.Forms.Button();
            this.bnAdd = new System.Windows.Forms.Button();
            this.bnRemove = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbSequentialMatching = new System.Windows.Forms.CheckBox();
            this.chkCustomShowName = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.gbAutoFolders = new System.Windows.Forms.GroupBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.gbAutoFolders.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtCustomShowName
            // 
            this.txtCustomShowName.Location = new System.Drawing.Point(133, 170);
            this.txtCustomShowName.Name = "txtCustomShowName";
            this.txtCustomShowName.Size = new System.Drawing.Size(288, 20);
            this.txtCustomShowName.TabIndex = 2;
            // 
            // cbTimeZone
            // 
            this.cbTimeZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTimeZone.FormattingEnabled = true;
            this.cbTimeZone.Location = new System.Drawing.Point(101, 198);
            this.cbTimeZone.Name = "cbTimeZone";
            this.cbTimeZone.Size = new System.Drawing.Size(200, 21);
            this.cbTimeZone.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 201);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Airs in &Timezone:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(354, 398);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 2;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(273, 398);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // chkSpecialsCount
            // 
            this.chkSpecialsCount.AutoSize = true;
            this.chkSpecialsCount.Location = new System.Drawing.Point(8, 55);
            this.chkSpecialsCount.Name = "chkSpecialsCount";
            this.chkSpecialsCount.Size = new System.Drawing.Size(155, 17);
            this.chkSpecialsCount.TabIndex = 2;
            this.chkSpecialsCount.Text = "S&pecials count as episodes";
            this.chkSpecialsCount.UseVisualStyleBackColor = true;
            // 
            // chkShowNextAirdate
            // 
            this.chkShowNextAirdate.AutoSize = true;
            this.chkShowNextAirdate.Location = new System.Drawing.Point(8, 32);
            this.chkShowNextAirdate.Name = "chkShowNextAirdate";
            this.chkShowNextAirdate.Size = new System.Drawing.Size(201, 17);
            this.chkShowNextAirdate.TabIndex = 1;
            this.chkShowNextAirdate.Text = "Show &next airdate in When to Watch";
            this.chkShowNextAirdate.UseVisualStyleBackColor = true;
            // 
            // pnlCF
            // 
            this.pnlCF.Location = new System.Drawing.Point(3, 4);
            this.pnlCF.Name = "pnlCF";
            this.pnlCF.Size = new System.Drawing.Size(419, 160);
            this.pnlCF.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Base &Folder";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // chkFolderPerSeason
            // 
            this.chkFolderPerSeason.AutoSize = true;
            this.chkFolderPerSeason.Location = new System.Drawing.Point(13, 49);
            this.chkFolderPerSeason.Name = "chkFolderPerSeason";
            this.chkFolderPerSeason.Size = new System.Drawing.Size(171, 17);
            this.chkFolderPerSeason.TabIndex = 3;
            this.chkFolderPerSeason.Text = "&Folder per season, base name:";
            this.chkFolderPerSeason.UseVisualStyleBackColor = true;
            this.chkFolderPerSeason.CheckedChanged += new System.EventHandler(this.chkFolderPerSeason_CheckedChanged);
            // 
            // txtSeasonFolderName
            // 
            this.txtSeasonFolderName.Location = new System.Drawing.Point(190, 47);
            this.txtSeasonFolderName.Name = "txtSeasonFolderName";
            this.txtSeasonFolderName.Size = new System.Drawing.Size(120, 20);
            this.txtSeasonFolderName.TabIndex = 4;
            // 
            // txtBaseFolder
            // 
            this.txtBaseFolder.Location = new System.Drawing.Point(79, 21);
            this.txtBaseFolder.Name = "txtBaseFolder";
            this.txtBaseFolder.Size = new System.Drawing.Size(170, 20);
            this.txtBaseFolder.TabIndex = 1;
            this.txtBaseFolder.Click += new System.EventHandler(this.bnBrowse_Click);
            // 
            // bnBrowse
            // 
            this.bnBrowse.Location = new System.Drawing.Point(255, 18);
            this.bnBrowse.Name = "bnBrowse";
            this.bnBrowse.Size = new System.Drawing.Size(75, 23);
            this.bnBrowse.TabIndex = 2;
            this.bnBrowse.Text = "&Browse...";
            this.bnBrowse.UseVisualStyleBackColor = true;
            this.bnBrowse.Click += new System.EventHandler(this.bnBrowse_Click);
            // 
            // chkAutoFolders
            // 
            this.chkAutoFolders.AutoSize = true;
            this.chkAutoFolders.Checked = true;
            this.chkAutoFolders.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoFolders.Location = new System.Drawing.Point(20, 249);
            this.chkAutoFolders.Name = "chkAutoFolders";
            this.chkAutoFolders.Size = new System.Drawing.Size(110, 17);
            this.chkAutoFolders.TabIndex = 8;
            this.chkAutoFolders.Text = "&Automatic Folders";
            this.chkAutoFolders.UseVisualStyleBackColor = true;
            this.chkAutoFolders.CheckedChanged += new System.EventHandler(this.chkAutoFolders_CheckedChanged);
            // 
            // cbDoRenaming
            // 
            this.cbDoRenaming.AutoSize = true;
            this.cbDoRenaming.Location = new System.Drawing.Point(8, 78);
            this.cbDoRenaming.Name = "cbDoRenaming";
            this.cbDoRenaming.Size = new System.Drawing.Size(86, 17);
            this.cbDoRenaming.TabIndex = 3;
            this.cbDoRenaming.Text = "Do &renaming";
            this.cbDoRenaming.UseVisualStyleBackColor = true;
            // 
            // cbDoMissingCheck
            // 
            this.cbDoMissingCheck.AutoSize = true;
            this.cbDoMissingCheck.Location = new System.Drawing.Point(8, 101);
            this.cbDoMissingCheck.Name = "cbDoMissingCheck";
            this.cbDoMissingCheck.Size = new System.Drawing.Size(110, 17);
            this.cbDoMissingCheck.TabIndex = 4;
            this.cbDoMissingCheck.Text = "Do &missing check";
            this.cbDoMissingCheck.UseVisualStyleBackColor = true;
            this.cbDoMissingCheck.CheckedChanged += new System.EventHandler(this.cbDoMissingCheck_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 228);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Ign&ore Seasons:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtIgnoreSeasons
            // 
            this.txtIgnoreSeasons.Location = new System.Drawing.Point(101, 225);
            this.txtIgnoreSeasons.Name = "txtIgnoreSeasons";
            this.txtIgnoreSeasons.Size = new System.Drawing.Size(156, 20);
            this.txtIgnoreSeasons.TabIndex = 6;
            // 
            // chkDVDOrder
            // 
            this.chkDVDOrder.AutoSize = true;
            this.chkDVDOrder.Location = new System.Drawing.Point(8, 9);
            this.chkDVDOrder.Name = "chkDVDOrder";
            this.chkDVDOrder.Size = new System.Drawing.Size(100, 17);
            this.chkDVDOrder.TabIndex = 0;
            this.chkDVDOrder.Text = "&Use DVD Order";
            this.chkDVDOrder.UseVisualStyleBackColor = true;
            // 
            // chkForceCheckAll
            // 
            this.chkForceCheckAll.AutoSize = true;
            this.chkForceCheckAll.Location = new System.Drawing.Point(21, 124);
            this.chkForceCheckAll.Name = "chkForceCheckAll";
            this.chkForceCheckAll.Size = new System.Drawing.Size(274, 17);
            this.chkForceCheckAll.TabIndex = 5;
            this.chkForceCheckAll.Text = "M&issing check for all episodes (future and no airdate)";
            this.chkForceCheckAll.UseVisualStyleBackColor = true;
            // 
            // chkPadTwoDigits
            // 
            this.chkPadTwoDigits.AutoSize = true;
            this.chkPadTwoDigits.Location = new System.Drawing.Point(190, 73);
            this.chkPadTwoDigits.Name = "chkPadTwoDigits";
            this.chkPadTwoDigits.Size = new System.Drawing.Size(179, 17);
            this.chkPadTwoDigits.TabIndex = 5;
            this.chkPadTwoDigits.Text = "Pad season number to two digits";
            this.chkPadTwoDigits.UseVisualStyleBackColor = true;
            // 
            // lvSeasonFolders
            // 
            this.lvSeasonFolders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvSeasonFolders.FullRowSelect = true;
            this.lvSeasonFolders.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSeasonFolders.Location = new System.Drawing.Point(11, 77);
            this.lvSeasonFolders.Name = "lvSeasonFolders";
            this.lvSeasonFolders.Size = new System.Drawing.Size(311, 97);
            this.lvSeasonFolders.TabIndex = 6;
            this.lvSeasonFolders.UseCompatibleStateImageBehavior = false;
            this.lvSeasonFolders.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Season";
            this.columnHeader1.Width = 52;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Folder";
            this.columnHeader2.Width = 250;
            // 
            // txtSeasonNumber
            // 
            this.txtSeasonNumber.Location = new System.Drawing.Point(61, 19);
            this.txtSeasonNumber.Name = "txtSeasonNumber";
            this.txtSeasonNumber.Size = new System.Drawing.Size(52, 20);
            this.txtSeasonNumber.TabIndex = 1;
            this.txtSeasonNumber.TextChanged += new System.EventHandler(this.txtSeasonNumber_TextChanged);
            // 
            // txtFolder
            // 
            this.txtFolder.Location = new System.Drawing.Point(61, 50);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(180, 20);
            this.txtFolder.TabIndex = 3;
            // 
            // bnBrowseFolder
            // 
            this.bnBrowseFolder.Location = new System.Drawing.Point(247, 48);
            this.bnBrowseFolder.Name = "bnBrowseFolder";
            this.bnBrowseFolder.Size = new System.Drawing.Size(75, 23);
            this.bnBrowseFolder.TabIndex = 4;
            this.bnBrowseFolder.Text = "B&rowse...";
            this.bnBrowseFolder.UseVisualStyleBackColor = true;
            this.bnBrowseFolder.Click += new System.EventHandler(this.bnBrowseFolder_Click);
            // 
            // bnAdd
            // 
            this.bnAdd.Location = new System.Drawing.Point(328, 48);
            this.bnAdd.Name = "bnAdd";
            this.bnAdd.Size = new System.Drawing.Size(75, 23);
            this.bnAdd.TabIndex = 5;
            this.bnAdd.Text = "&Add";
            this.bnAdd.UseVisualStyleBackColor = true;
            // 
            // bnRemove
            // 
            this.bnRemove.Location = new System.Drawing.Point(328, 151);
            this.bnRemove.Name = "bnRemove";
            this.bnRemove.Size = new System.Drawing.Size(75, 23);
            this.bnRemove.TabIndex = 7;
            this.bnRemove.Text = "Remo&ve";
            this.bnRemove.UseVisualStyleBackColor = true;
            this.bnRemove.Click += new System.EventHandler(this.bnRemove_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Season:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 53);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Folder:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.bnRemove);
            this.groupBox1.Controls.Add(this.bnAdd);
            this.groupBox1.Controls.Add(this.bnBrowseFolder);
            this.groupBox1.Controls.Add(this.txtFolder);
            this.groupBox1.Controls.Add(this.txtSeasonNumber);
            this.groupBox1.Controls.Add(this.lvSeasonFolders);
            this.groupBox1.Location = new System.Drawing.Point(7, 173);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(411, 186);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Manual/Additional Folders";
            // 
            // cbSequentialMatching
            // 
            this.cbSequentialMatching.AutoSize = true;
            this.cbSequentialMatching.Location = new System.Drawing.Point(8, 147);
            this.cbSequentialMatching.Name = "cbSequentialMatching";
            this.cbSequentialMatching.Size = new System.Drawing.Size(324, 17);
            this.cbSequentialMatching.TabIndex = 6;
            this.cbSequentialMatching.Text = "Use sequential number matching (finding missing episodes only)";
            this.cbSequentialMatching.UseVisualStyleBackColor = true;
            // 
            // chkCustomShowName
            // 
            this.chkCustomShowName.AutoSize = true;
            this.chkCustomShowName.Location = new System.Drawing.Point(9, 172);
            this.chkCustomShowName.Name = "chkCustomShowName";
            this.chkCustomShowName.Size = new System.Drawing.Size(121, 17);
            this.chkCustomShowName.TabIndex = 1;
            this.chkCustomShowName.Text = "Custom s&how name:";
            this.chkCustomShowName.UseVisualStyleBackColor = true;
            this.chkCustomShowName.CheckedChanged += new System.EventHandler(this.chkCustomShowName_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(433, 390);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.pnlCF);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.chkCustomShowName);
            this.tabPage1.Controls.Add(this.txtCustomShowName);
            this.tabPage1.Controls.Add(this.chkAutoFolders);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.cbTimeZone);
            this.tabPage1.Controls.Add(this.txtIgnoreSeasons);
            this.tabPage1.Controls.Add(this.gbAutoFolders);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(425, 364);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Basics";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(260, 228);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(162, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "e.g. \"1 2 4\". 0 to ignore specials.";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // gbAutoFolders
            // 
            this.gbAutoFolders.Controls.Add(this.chkPadTwoDigits);
            this.gbAutoFolders.Controls.Add(this.txtBaseFolder);
            this.gbAutoFolders.Controls.Add(this.bnBrowse);
            this.gbAutoFolders.Controls.Add(this.txtSeasonFolderName);
            this.gbAutoFolders.Controls.Add(this.label3);
            this.gbAutoFolders.Controls.Add(this.chkFolderPerSeason);
            this.gbAutoFolders.Location = new System.Drawing.Point(9, 251);
            this.gbAutoFolders.Name = "gbAutoFolders";
            this.gbAutoFolders.Size = new System.Drawing.Size(372, 100);
            this.gbAutoFolders.TabIndex = 9;
            this.gbAutoFolders.TabStop = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.chkShowNextAirdate);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.chkDVDOrder);
            this.tabPage2.Controls.Add(this.chkForceCheckAll);
            this.tabPage2.Controls.Add(this.cbDoRenaming);
            this.tabPage2.Controls.Add(this.cbDoMissingCheck);
            this.tabPage2.Controls.Add(this.cbSequentialMatching);
            this.tabPage2.Controls.Add(this.chkSpecialsCount);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(425, 364);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Advanced";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // AddEditShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(433, 433);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddEditShow";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add/Edit Show";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.gbAutoFolders.ResumeLayout(false);
            this.gbAutoFolders.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlCF;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkFolderPerSeason;
        private System.Windows.Forms.TextBox txtSeasonFolderName;
        private System.Windows.Forms.TextBox txtBaseFolder;
        private System.Windows.Forms.Button bnBrowse;
        private System.Windows.Forms.CheckBox chkAutoFolders;
        private System.Windows.Forms.CheckBox cbDoRenaming;
        private System.Windows.Forms.CheckBox cbDoMissingCheck;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkDVDOrder;
        private System.Windows.Forms.CheckBox chkForceCheckAll;
        private System.Windows.Forms.ListView lvSeasonFolders;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TextBox txtSeasonNumber;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Button bnBrowseFolder;
        private System.Windows.Forms.Button bnAdd;
        private System.Windows.Forms.Button bnRemove;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbSequentialMatching;
        private System.Windows.Forms.CheckBox chkCustomShowName;
        private System.Windows.Forms.CheckBox chkPadTwoDigits;
        private System.Windows.Forms.TextBox txtIgnoreSeasons;
        private System.Windows.Forms.TextBox txtCustomShowName;
        private System.Windows.Forms.ComboBox cbTimeZone;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.CheckBox chkSpecialsCount;
        private System.Windows.Forms.CheckBox chkShowNextAirdate;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox gbAutoFolders;
    }
}