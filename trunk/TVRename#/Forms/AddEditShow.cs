//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//


using System.Text.RegularExpressions;
using System.Windows.Forms;

//            this->txtCustomShowName->TextChanged += gcnew System::EventHandler(this, &AddEditShow::txtCustomShowName_TextChanged);

namespace TVRename
{

    /// <summary>
    /// Summary for AddEditShow
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public class AddEditShow : System.Windows.Forms.Form
    {
        private ShowItem mSI;
        private TheTVDB mTVDB;
        private TheTVDBCodeFinder mTCCF;
        public string ShowTimeZone;


        private System.Windows.Forms.Panel pnlCF;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkFolderPerSeason;
        private System.Windows.Forms.TextBox txtSeasonFolderName;
        private System.Windows.Forms.Label txtHash;
        private System.Windows.Forms.TextBox txtBaseFolder;
        private System.Windows.Forms.Button bnBrowse;
        private System.Windows.Forms.CheckBox chkAutoFolders;
        private System.Windows.Forms.CheckBox cbDoRenaming;
        private System.Windows.Forms.CheckBox cbDoMissingCheck;
        private System.Windows.Forms.CheckBox chkThumbnailsAndStuff;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkDVDOrder;
        private System.Windows.Forms.CheckBox chkForceCheckAll;
        private System.Windows.Forms.GroupBox gbAutoFolders;
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
            this.txtHash = new System.Windows.Forms.Label();
            this.txtBaseFolder = new System.Windows.Forms.TextBox();
            this.bnBrowse = new System.Windows.Forms.Button();
            this.chkAutoFolders = new System.Windows.Forms.CheckBox();
            this.cbDoRenaming = new System.Windows.Forms.CheckBox();
            this.cbDoMissingCheck = new System.Windows.Forms.CheckBox();
            this.chkThumbnailsAndStuff = new System.Windows.Forms.CheckBox();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.label5 = new System.Windows.Forms.Label();
            this.txtIgnoreSeasons = new System.Windows.Forms.TextBox();
            this.chkDVDOrder = new System.Windows.Forms.CheckBox();
            this.chkForceCheckAll = new System.Windows.Forms.CheckBox();
            this.gbAutoFolders = new System.Windows.Forms.GroupBox();
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
            this.gbAutoFolders.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtCustomShowName
            // 
            this.txtCustomShowName.Location = new System.Drawing.Point(137, 175);
            this.txtCustomShowName.Name = "txtCustomShowName";
            this.txtCustomShowName.Size = new System.Drawing.Size(283, 20);
            this.txtCustomShowName.TabIndex = 2;
            // 
            // cbTimeZone
            // 
            this.cbTimeZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTimeZone.FormattingEnabled = true;
            this.cbTimeZone.Location = new System.Drawing.Point(103, 201);
            this.cbTimeZone.Name = "cbTimeZone";
            this.cbTimeZone.Size = new System.Drawing.Size(200, 21);
            this.cbTimeZone.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 204);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Airs in &Timezone:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(347, 615);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 21;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(266, 615);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 20;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // chkSpecialsCount
            // 
            this.chkSpecialsCount.AutoSize = true;
            this.chkSpecialsCount.Location = new System.Drawing.Point(12, 251);
            this.chkSpecialsCount.Name = "chkSpecialsCount";
            this.chkSpecialsCount.Size = new System.Drawing.Size(155, 17);
            this.chkSpecialsCount.TabIndex = 11;
            this.chkSpecialsCount.Text = "S&pecials count as episodes";
            this.chkSpecialsCount.UseVisualStyleBackColor = true;
            // 
            // chkShowNextAirdate
            // 
            this.chkShowNextAirdate.AutoSize = true;
            this.chkShowNextAirdate.Location = new System.Drawing.Point(12, 228);
            this.chkShowNextAirdate.Name = "chkShowNextAirdate";
            this.chkShowNextAirdate.Size = new System.Drawing.Size(111, 17);
            this.chkShowNextAirdate.TabIndex = 8;
            this.chkShowNextAirdate.Text = "Show &next airdate";
            this.chkShowNextAirdate.UseVisualStyleBackColor = true;
            // 
            // pnlCF
            // 
            this.pnlCF.Location = new System.Drawing.Point(11, 7);
            this.pnlCF.Name = "pnlCF";
            this.pnlCF.Size = new System.Drawing.Size(409, 160);
            this.pnlCF.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Base &Folder";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // chkFolderPerSeason
            // 
            this.chkFolderPerSeason.AutoSize = true;
            this.chkFolderPerSeason.Location = new System.Drawing.Point(9, 45);
            this.chkFolderPerSeason.Name = "chkFolderPerSeason";
            this.chkFolderPerSeason.Size = new System.Drawing.Size(110, 17);
            this.chkFolderPerSeason.TabIndex = 22;
            this.chkFolderPerSeason.Text = "&Folder per season";
            this.chkFolderPerSeason.UseVisualStyleBackColor = true;
            this.chkFolderPerSeason.CheckedChanged += new System.EventHandler(this.chkFolderPerSeason_CheckedChanged);
            // 
            // txtSeasonFolderName
            // 
            this.txtSeasonFolderName.Location = new System.Drawing.Point(125, 43);
            this.txtSeasonFolderName.Name = "txtSeasonFolderName";
            this.txtSeasonFolderName.Size = new System.Drawing.Size(120, 20);
            this.txtSeasonFolderName.TabIndex = 23;
            // 
            // txtHash
            // 
            this.txtHash.AutoSize = true;
            this.txtHash.Enabled = false;
            this.txtHash.Location = new System.Drawing.Point(251, 46);
            this.txtHash.Name = "txtHash";
            this.txtHash.Size = new System.Drawing.Size(14, 13);
            this.txtHash.TabIndex = 6;
            this.txtHash.Text = "#";
            this.txtHash.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtBaseFolder
            // 
            this.txtBaseFolder.Location = new System.Drawing.Point(75, 17);
            this.txtBaseFolder.Name = "txtBaseFolder";
            this.txtBaseFolder.Size = new System.Drawing.Size(170, 20);
            this.txtBaseFolder.TabIndex = 23;
            this.txtBaseFolder.Click += new System.EventHandler(this.bnBrowse_Click);
            // 
            // bnBrowse
            // 
            this.bnBrowse.Location = new System.Drawing.Point(252, 15);
            this.bnBrowse.Name = "bnBrowse";
            this.bnBrowse.Size = new System.Drawing.Size(75, 23);
            this.bnBrowse.TabIndex = 24;
            this.bnBrowse.Text = "&Browse...";
            this.bnBrowse.UseVisualStyleBackColor = true;
            this.bnBrowse.Click += new System.EventHandler(this.bnBrowse_Click);
            // 
            // chkAutoFolders
            // 
            this.chkAutoFolders.AutoSize = true;
            this.chkAutoFolders.Checked = true;
            this.chkAutoFolders.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoFolders.Location = new System.Drawing.Point(21, 340);
            this.chkAutoFolders.Name = "chkAutoFolders";
            this.chkAutoFolders.Size = new System.Drawing.Size(110, 17);
            this.chkAutoFolders.TabIndex = 22;
            this.chkAutoFolders.Text = "Automatic Folders";
            this.chkAutoFolders.UseVisualStyleBackColor = true;
            this.chkAutoFolders.CheckedChanged += new System.EventHandler(this.chkAutoFolders_CheckedChanged);
            // 
            // cbDoRenaming
            // 
            this.cbDoRenaming.AutoSize = true;
            this.cbDoRenaming.Location = new System.Drawing.Point(12, 274);
            this.cbDoRenaming.Name = "cbDoRenaming";
            this.cbDoRenaming.Size = new System.Drawing.Size(86, 17);
            this.cbDoRenaming.TabIndex = 8;
            this.cbDoRenaming.Text = "Do &renaming";
            this.cbDoRenaming.UseVisualStyleBackColor = true;
            // 
            // cbDoMissingCheck
            // 
            this.cbDoMissingCheck.AutoSize = true;
            this.cbDoMissingCheck.Location = new System.Drawing.Point(107, 274);
            this.cbDoMissingCheck.Name = "cbDoMissingCheck";
            this.cbDoMissingCheck.Size = new System.Drawing.Size(110, 17);
            this.cbDoMissingCheck.TabIndex = 8;
            this.cbDoMissingCheck.Text = "Do &missing check";
            this.cbDoMissingCheck.UseVisualStyleBackColor = true;
            this.cbDoMissingCheck.CheckedChanged += new System.EventHandler(this.cbDoMissingCheck_CheckedChanged);
            // 
            // chkThumbnailsAndStuff
            // 
            this.chkThumbnailsAndStuff.AutoSize = true;
            this.chkThumbnailsAndStuff.Enabled = false;
            this.chkThumbnailsAndStuff.Location = new System.Drawing.Point(256, 251);
            this.chkThumbnailsAndStuff.Name = "chkThumbnailsAndStuff";
            this.chkThumbnailsAndStuff.Size = new System.Drawing.Size(166, 17);
            this.chkThumbnailsAndStuff.TabIndex = 8;
            this.chkThumbnailsAndStuff.Text = "Thumbnail, banners, and stuff";
            this.chkThumbnailsAndStuff.UseVisualStyleBackColor = true;
            this.chkThumbnailsAndStuff.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 319);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Ign&ore Seasons:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtIgnoreSeasons
            // 
            this.txtIgnoreSeasons.Location = new System.Drawing.Point(101, 316);
            this.txtIgnoreSeasons.Name = "txtIgnoreSeasons";
            this.txtIgnoreSeasons.Size = new System.Drawing.Size(156, 20);
            this.txtIgnoreSeasons.TabIndex = 23;
            // 
            // chkDVDOrder
            // 
            this.chkDVDOrder.AutoSize = true;
            this.chkDVDOrder.Location = new System.Drawing.Point(256, 228);
            this.chkDVDOrder.Name = "chkDVDOrder";
            this.chkDVDOrder.Size = new System.Drawing.Size(78, 17);
            this.chkDVDOrder.TabIndex = 25;
            this.chkDVDOrder.Text = "&DVD Order";
            this.chkDVDOrder.UseVisualStyleBackColor = true;
            // 
            // chkForceCheckAll
            // 
            this.chkForceCheckAll.AutoSize = true;
            this.chkForceCheckAll.Location = new System.Drawing.Point(223, 274);
            this.chkForceCheckAll.Name = "chkForceCheckAll";
            this.chkForceCheckAll.Size = new System.Drawing.Size(167, 17);
            this.chkForceCheckAll.TabIndex = 8;
            this.chkForceCheckAll.Text = "M&issing check for all episodes";
            this.chkForceCheckAll.UseVisualStyleBackColor = true;
            // 
            // gbAutoFolders
            // 
            this.gbAutoFolders.Controls.Add(this.chkPadTwoDigits);
            this.gbAutoFolders.Controls.Add(this.txtBaseFolder);
            this.gbAutoFolders.Controls.Add(this.bnBrowse);
            this.gbAutoFolders.Controls.Add(this.txtSeasonFolderName);
            this.gbAutoFolders.Controls.Add(this.label3);
            this.gbAutoFolders.Controls.Add(this.chkFolderPerSeason);
            this.gbAutoFolders.Controls.Add(this.txtHash);
            this.gbAutoFolders.Location = new System.Drawing.Point(12, 341);
            this.gbAutoFolders.Name = "gbAutoFolders";
            this.gbAutoFolders.Size = new System.Drawing.Size(387, 75);
            this.gbAutoFolders.TabIndex = 26;
            this.gbAutoFolders.TabStop = false;
            // 
            // chkPadTwoDigits
            // 
            this.chkPadTwoDigits.AutoSize = true;
            this.chkPadTwoDigits.Location = new System.Drawing.Point(278, 45);
            this.chkPadTwoDigits.Name = "chkPadTwoDigits";
            this.chkPadTwoDigits.Size = new System.Drawing.Size(104, 17);
            this.chkPadTwoDigits.TabIndex = 25;
            this.chkPadTwoDigits.Text = "Pad to two digits";
            this.chkPadTwoDigits.UseVisualStyleBackColor = true;
            this.chkPadTwoDigits.CheckedChanged += new System.EventHandler(this.chkPadTwoDigits_CheckedChanged);
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
            this.lvSeasonFolders.TabIndex = 27;
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
            this.txtSeasonNumber.TabIndex = 28;
            this.txtSeasonNumber.TextChanged += new System.EventHandler(this.txtSeasonNumber_TextChanged);
            // 
            // txtFolder
            // 
            this.txtFolder.Location = new System.Drawing.Point(61, 50);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(180, 20);
            this.txtFolder.TabIndex = 28;
            // 
            // bnBrowseFolder
            // 
            this.bnBrowseFolder.Location = new System.Drawing.Point(247, 48);
            this.bnBrowseFolder.Name = "bnBrowseFolder";
            this.bnBrowseFolder.Size = new System.Drawing.Size(75, 23);
            this.bnBrowseFolder.TabIndex = 29;
            this.bnBrowseFolder.Text = "B&rowse...";
            this.bnBrowseFolder.UseVisualStyleBackColor = true;
            this.bnBrowseFolder.Click += new System.EventHandler(this.bnBrowseFolder_Click);
            // 
            // bnAdd
            // 
            this.bnAdd.Location = new System.Drawing.Point(328, 48);
            this.bnAdd.Name = "bnAdd";
            this.bnAdd.Size = new System.Drawing.Size(75, 23);
            this.bnAdd.TabIndex = 29;
            this.bnAdd.Text = "&Add";
            this.bnAdd.UseVisualStyleBackColor = true;
            // 
            // bnRemove
            // 
            this.bnRemove.Location = new System.Drawing.Point(328, 151);
            this.bnRemove.Name = "bnRemove";
            this.bnRemove.Size = new System.Drawing.Size(75, 23);
            this.bnRemove.TabIndex = 29;
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
            this.label1.TabIndex = 30;
            this.label1.Text = "Season:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 53);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 13);
            this.label7.TabIndex = 30;
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
            this.groupBox1.Location = new System.Drawing.Point(12, 422);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(411, 186);
            this.groupBox1.TabIndex = 31;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Manual/Additional Folders";
            // 
            // cbSequentialMatching
            // 
            this.cbSequentialMatching.AutoSize = true;
            this.cbSequentialMatching.Location = new System.Drawing.Point(12, 297);
            this.cbSequentialMatching.Name = "cbSequentialMatching";
            this.cbSequentialMatching.Size = new System.Drawing.Size(290, 17);
            this.cbSequentialMatching.TabIndex = 8;
            this.cbSequentialMatching.Text = "Use sequential number matching (missing episodes only)";
            this.cbSequentialMatching.UseVisualStyleBackColor = true;
            // 
            // chkCustomShowName
            // 
            this.chkCustomShowName.AutoSize = true;
            this.chkCustomShowName.Location = new System.Drawing.Point(11, 177);
            this.chkCustomShowName.Name = "chkCustomShowName";
            this.chkCustomShowName.Size = new System.Drawing.Size(121, 17);
            this.chkCustomShowName.TabIndex = 32;
            this.chkCustomShowName.Text = "Custom s&how name:";
            this.chkCustomShowName.UseVisualStyleBackColor = true;
            this.chkCustomShowName.CheckedChanged += new System.EventHandler(this.chkCustomShowName_CheckedChanged);
            // 
            // AddEditShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(432, 650);
            this.Controls.Add(this.chkCustomShowName);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkAutoFolders);
            this.Controls.Add(this.gbAutoFolders);
            this.Controls.Add(this.chkDVDOrder);
            this.Controls.Add(this.txtIgnoreSeasons);
            this.Controls.Add(this.pnlCF);
            this.Controls.Add(this.chkSpecialsCount);
            this.Controls.Add(this.chkThumbnailsAndStuff);
            this.Controls.Add(this.chkForceCheckAll);
            this.Controls.Add(this.cbDoMissingCheck);
            this.Controls.Add(this.cbSequentialMatching);
            this.Controls.Add(this.cbDoRenaming);
            this.Controls.Add(this.chkShowNextAirdate);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.txtCustomShowName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cbTimeZone);
            this.Controls.Add(this.label6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddEditShow";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add/Edit Show";
            this.gbAutoFolders.ResumeLayout(false);
            this.gbAutoFolders.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public AddEditShow(ShowItem si, TheTVDB db, string timezone)
        {
            mSI = si;
            mTVDB = db;
            InitializeComponent();

            cbTimeZone.BeginUpdate();
            cbTimeZone.Items.Clear();

            foreach (string s in TimeZone.ZoneNames())
                cbTimeZone.Items.Add(s);

            cbTimeZone.EndUpdate();


            mTCCF = new TheTVDBCodeFinder(si.TVDBCode != -1 ? si.TVDBCode.ToString() : "", mTVDB);
            mTCCF.Dock = DockStyle.Fill;
            //mTCCF->SelectionChanged += gcnew System::EventHandler(this, &AddEditShow::lvMatches_ItemSelectionChanged);

            pnlCF.SuspendLayout();
            pnlCF.Controls.Add(mTCCF);
            pnlCF.ResumeLayout();

            chkCustomShowName.Checked = si.UseCustomShowName;
            if (chkCustomShowName.Checked)
                txtCustomShowName.Text = si.CustomShowName;
            chkCustomShowName_CheckedChanged(null, null);

            cbSequentialMatching.Checked = si.UseSequentialMatch;
            chkShowNextAirdate.Checked = si.ShowNextAirdate;
            chkSpecialsCount.Checked = si.CountSpecials;
            chkFolderPerSeason.Checked = si.AutoAdd_FolderPerSeason;
            txtSeasonFolderName.Text = si.AutoAdd_SeasonFolderName;
            txtBaseFolder.Text = si.AutoAdd_FolderBase;
            chkAutoFolders.Checked = si.AutoAddNewSeasons;
            chkAutoFolders_CheckedChanged(null, null);
            chkFolderPerSeason_CheckedChanged(null, null);

            chkThumbnailsAndStuff.Checked = false; // TODO
            cbDoRenaming.Checked = si.DoRename;
            cbDoMissingCheck.Checked = si.DoMissingCheck;
            cbDoMissingCheck_CheckedChanged(null, null);

            chkPadTwoDigits.Checked = si.PadSeasonToTwoDigits;
            SetRightNumberOfHashes();

            ShowTimeZone = (!string.IsNullOrEmpty(timezone)) ? timezone : TimeZone.DefaultTimeZone();
            cbTimeZone.Text = ShowTimeZone;
            chkDVDOrder.Checked = si.DVDOrder;
            chkForceCheckAll.Checked = si.ForceCheckAll;

            bool first = true;
            si.IgnoreSeasons.Sort();
            foreach (int i in si.IgnoreSeasons)
            {
                if (!first)
                    txtIgnoreSeasons.Text += " ";
                txtIgnoreSeasons.Text += i.ToString();
                first = false;
            }

            foreach (System.Collections.Generic.KeyValuePair<int, StringList> kvp in si.ManualFolderLocations)
            {
                foreach (string s in kvp.Value)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = kvp.Key.ToString();
                    lvi.SubItems.Add(s);

                    lvSeasonFolders.Items.Add(lvi);
                }
            }
            lvSeasonFolders.Sort();

            txtSeasonNumber_TextChanged(null, null);
            txtFolder_TextChanged(null, null);

        }

        private void buttonOK_Click(object sender, System.EventArgs e)
        {
            if (!OKToClose())
            {
                this.DialogResult = DialogResult.None;
                return;
            }

            SetmSI();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }


        private bool OKToClose()
        {
            if (!mTVDB.HasSeries(mTCCF.SelectedCode()))
            {
                DialogResult dr = MessageBox.Show("tvdb code unknown, close anyway?", "TVRename Add/Edit Show", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.No)
                    return false;
            }

            return true;
        }

        private void SetmSI()
        {
            int code = mTCCF.SelectedCode();

            string tz = (cbTimeZone.SelectedItem != null) ? cbTimeZone.SelectedItem.ToString() : "";

            mSI.CustomShowName = txtCustomShowName.Text;
            mSI.UseCustomShowName = chkCustomShowName.Checked;
            ShowTimeZone = tz; //TODO: move to somewhere else. make timezone manager for tvdb?
            mSI.ShowNextAirdate = chkShowNextAirdate.Checked;
            mSI.PadSeasonToTwoDigits = chkPadTwoDigits.Checked;
            mSI.TVDBCode = code;
            //todo mSI->SeasonNumber = seasnum;
            mSI.CountSpecials = chkSpecialsCount.Checked;
            //                                 mSI->Rules = mWorkingRuleSet;  // TODO
            mSI.DoRename = cbDoRenaming.Checked;
            mSI.DoMissingCheck = cbDoMissingCheck.Checked;

            mSI.AutoAddNewSeasons = chkAutoFolders.Checked;
            mSI.AutoAdd_FolderPerSeason = chkFolderPerSeason.Checked;
            mSI.AutoAdd_SeasonFolderName = txtSeasonFolderName.Text;
            mSI.AutoAdd_FolderBase = txtBaseFolder.Text;

            mSI.DVDOrder = chkDVDOrder.Checked;
            mSI.ForceCheckAll = chkForceCheckAll.Checked;
            mSI.UseSequentialMatch = cbSequentialMatching.Checked;

            string slist = txtIgnoreSeasons.Text;
            mSI.IgnoreSeasons.Clear();
            foreach (Match match in Regex.Matches(slist, "\\b[0-9]+\\b"))
                mSI.IgnoreSeasons.Add(int.Parse(match.Value));

            mSI.ManualFolderLocations.Clear();
            foreach (ListViewItem lvi in lvSeasonFolders.Items)
            {
                try
                {
                    int seas = int.Parse(lvi.Text);
                    if (!mSI.ManualFolderLocations.ContainsKey(seas))
                        mSI.ManualFolderLocations.Add(seas, new StringList());

                    mSI.ManualFolderLocations[seas].Add(lvi.SubItems[1].Text);
                }
                catch
                {
                }
            }
        }


        private void bnCancel_Click(object sender, System.EventArgs e)
        {
            Close();
        }
        private void chkFolderPerSeason_CheckedChanged(object sender, System.EventArgs e)
        {
            txtSeasonFolderName.Enabled = chkFolderPerSeason.Checked;
        }
        private void bnBrowse_Click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBaseFolder.Text))
                folderBrowser.SelectedPath = txtBaseFolder.Text;

            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txtBaseFolder.Text = folderBrowser.SelectedPath;
        }
        private void chkAutoFolders_CheckedChanged(object sender, System.EventArgs e)
        {
            gbAutoFolders.Enabled = chkAutoFolders.Checked;
        }
        private void cbDoMissingCheck_CheckedChanged(object sender, System.EventArgs e)
        {
            chkForceCheckAll.Enabled = cbDoMissingCheck.Checked;
        }
        private void bnRemove_Click(object sender, System.EventArgs e)
        {
            if (lvSeasonFolders.SelectedItems.Count > 0)
                foreach (ListViewItem lvi in lvSeasonFolders.SelectedItems)
                    lvSeasonFolders.Items.Remove(lvi);
        }
        private void bnAdd_Click(object sender, System.EventArgs e)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = txtSeasonNumber.Text;
            lvi.SubItems.Add(txtFolder.Text);

            lvSeasonFolders.Items.Add(lvi);

            txtSeasonNumber.Text = "";
            txtFolder.Text = "";

            lvSeasonFolders.Sort();
        }
        private void bnBrowseFolder_Click(object sender, System.EventArgs e)
        {
            folderBrowser.SelectedPath = txtFolder.Text;
            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txtFolder.Text = folderBrowser.SelectedPath;
        }
        private void txtSeasonNumber_TextChanged(object sender, System.EventArgs e)
        {
            bool isNumber = Regex.Match(txtSeasonNumber.Text, "^[0-9]+$").Success;
            bnAdd.Enabled = isNumber && (!string.IsNullOrEmpty(txtSeasonNumber.Text));
        }
        private void txtFolder_TextChanged(object sender, System.EventArgs e)
        {
            bool ok = true;
            if (!string.IsNullOrEmpty(txtFolder.Text))
            {
                try
                {
                    ok = System.IO.Directory.Exists(txtFolder.Text);
                }
                catch
                {
                }
            }
            if (ok)
                txtFolder.BackColor = System.Drawing.SystemColors.Window;
            else
                txtFolder.BackColor = Helpers.WarningColor();

        }
        private void chkCustomShowName_CheckedChanged(object sender, System.EventArgs e)
        {
            txtCustomShowName.Enabled = chkCustomShowName.Checked;
        }
        private void chkPadTwoDigits_CheckedChanged(object sender, System.EventArgs e)
        {
            SetRightNumberOfHashes();
        }

        private void SetRightNumberOfHashes()
        {
            txtHash.Text = chkPadTwoDigits.Checked ? "##" : "#";
        }
    }
}




namespace TVRename
{



} // namespace
