//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using DaveChambers.FolderBrowserDialogEx;

namespace TVRename
{
    partial class AddEditMovie
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddEditMovie));
            bnCancel = new Button();
            buttonOK = new Button();
            folderBrowser = new FolderBrowserDialog();
            tabPage2 = new TabPage();
            cbIncludeNoAirdate = new CheckBox();
            cbIncludeFuture = new CheckBox();
            label9 = new Label();
            cbDoRenaming = new CheckBox();
            cbDoMissingCheck = new CheckBox();
            pbAdvanced = new PictureBox();
            tabPage3 = new TabPage();
            label14 = new Label();
            lbSourceAliases = new ListBox();
            label10 = new Label();
            label8 = new Label();
            label4 = new Label();
            bnRemoveAlias = new Button();
            bnAddAlias = new Button();
            tbShowAlias = new TextBox();
            pbAliases = new PictureBox();
            lbShowAlias = new ListBox();
            tabPage5 = new TabPage();
            label2 = new Label();
            cbFolderType = new ComboBox();
            txtIgnoreList = new Label();
            btnIgnoreList = new Button();
            groupBox1 = new GroupBox();
            chkManualFolders = new CheckBox();
            button1 = new Button();
            label7 = new Label();
            bnRemove = new Button();
            bnAdd = new Button();
            bnBrowseFolder = new Button();
            txtFolder = new TextBox();
            lvManualFolders = new ListView();
            columnHeader2 = new ColumnHeader();
            label12 = new Label();
            chkAutoFolders = new CheckBox();
            gbAutoFolders = new GroupBox();
            cbDirectory = new ComboBox();
            label1 = new Label();
            txtFolderNameFormat = new TextBox();
            bnTags = new Button();
            lblSeasonWordPreview = new Label();
            rdoFolderCustom = new RadioButton();
            rdoFolderLibraryDefault = new RadioButton();
            label3 = new Label();
            pbFolders = new PictureBox();
            tabPage1 = new TabPage();
            cbRegion = new ComboBox();
            chkCustomRegion = new CheckBox();
            rdoTMDB = new RadioButton();
            label13 = new Label();
            rdoTVDB = new RadioButton();
            rdoDefault = new RadioButton();
            label60 = new Label();
            pbBasics = new PictureBox();
            cbLanguage = new ComboBox();
            chkCustomLanguage = new CheckBox();
            pnlCF = new Panel();
            chkCustomShowName = new CheckBox();
            txtCustomShowName = new TextBox();
            Folders = new TabControl();
            tabPage6 = new TabPage();
            llFilenameDefaultFormat = new LinkLabel();
            llCustomName = new LinkLabel();
            lbLibraryDefaultNaming = new Label();
            label15 = new Label();
            lbNamingExample = new Label();
            txtCustomMovieFileNamingFormat = new TextBox();
            txtTagList2 = new Label();
            lbAvailableTags = new Label();
            label19 = new Label();
            cbUseCustomNamingFormat = new CheckBox();
            pictureBox1 = new PictureBox();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbAdvanced).BeginInit();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbAliases).BeginInit();
            tabPage5.SuspendLayout();
            groupBox1.SuspendLayout();
            gbAutoFolders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbFolders).BeginInit();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbBasics).BeginInit();
            Folders.SuspendLayout();
            tabPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // bnCancel
            // 
            bnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            bnCancel.DialogResult = DialogResult.Cancel;
            bnCancel.Location = new Point(428, 579);
            bnCancel.Margin = new Padding(4, 3, 4, 3);
            bnCancel.Name = "bnCancel";
            bnCancel.Size = new Size(88, 27);
            bnCancel.TabIndex = 2;
            bnCancel.Text = "Cancel";
            bnCancel.UseVisualStyleBackColor = true;
            bnCancel.Click += bnCancel_Click;
            // 
            // buttonOK
            // 
            buttonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonOK.DialogResult = DialogResult.OK;
            buttonOK.Location = new Point(334, 579);
            buttonOK.Margin = new Padding(4, 3, 4, 3);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(88, 27);
            buttonOK.TabIndex = 1;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(cbIncludeNoAirdate);
            tabPage2.Controls.Add(cbIncludeFuture);
            tabPage2.Controls.Add(label9);
            tabPage2.Controls.Add(cbDoRenaming);
            tabPage2.Controls.Add(cbDoMissingCheck);
            tabPage2.Controls.Add(pbAdvanced);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Margin = new Padding(4, 3, 4, 3);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(4, 3, 4, 3);
            tabPage2.Size = new Size(533, 542);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Advanced";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // cbIncludeNoAirdate
            // 
            cbIncludeNoAirdate.AutoSize = true;
            cbIncludeNoAirdate.Location = new Point(31, 129);
            cbIncludeNoAirdate.Margin = new Padding(4, 3, 4, 3);
            cbIncludeNoAirdate.Name = "cbIncludeNoAirdate";
            cbIncludeNoAirdate.Size = new Size(270, 19);
            cbIncludeNoAirdate.TabIndex = 43;
            cbIncludeNoAirdate.Text = "Include check when movie has no release date";
            cbIncludeNoAirdate.UseVisualStyleBackColor = true;
            // 
            // cbIncludeFuture
            // 
            cbIncludeFuture.AutoSize = true;
            cbIncludeFuture.Location = new Point(31, 103);
            cbIncludeFuture.Margin = new Padding(4, 3, 4, 3);
            cbIncludeFuture.Name = "cbIncludeFuture";
            cbIncludeFuture.Size = new Size(297, 19);
            cbIncludeFuture.TabIndex = 44;
            cbIncludeFuture.Text = "Include check when movie has a future release date";
            cbIncludeFuture.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label9.AutoSize = true;
            label9.Location = new Point(6, 7);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(427, 30);
            label9.TabIndex = 42;
            label9.Text = "Further details of how to setup the actions that TV Rename does for this specific\r\nshow.";
            // 
            // cbDoRenaming
            // 
            cbDoRenaming.AutoSize = true;
            cbDoRenaming.Location = new Point(9, 54);
            cbDoRenaming.Margin = new Padding(4, 3, 4, 3);
            cbDoRenaming.Name = "cbDoRenaming";
            cbDoRenaming.Size = new Size(95, 19);
            cbDoRenaming.TabIndex = 3;
            cbDoRenaming.Text = "Do &renaming";
            cbDoRenaming.UseVisualStyleBackColor = true;
            // 
            // cbDoMissingCheck
            // 
            cbDoMissingCheck.AutoSize = true;
            cbDoMissingCheck.Location = new Point(9, 81);
            cbDoMissingCheck.Margin = new Padding(4, 3, 4, 3);
            cbDoMissingCheck.Name = "cbDoMissingCheck";
            cbDoMissingCheck.Size = new Size(119, 19);
            cbDoMissingCheck.TabIndex = 4;
            cbDoMissingCheck.Text = "Do &missing check";
            cbDoMissingCheck.UseVisualStyleBackColor = true;
            cbDoMissingCheck.CheckedChanged += cbDoMissingCheck_CheckedChanged;
            // 
            // pbAdvanced
            // 
            pbAdvanced.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbAdvanced.Cursor = Cursors.Hand;
            pbAdvanced.Image = (Image)resources.GetObject("pbAdvanced.Image");
            pbAdvanced.InitialImage = (Image)resources.GetObject("pbAdvanced.InitialImage");
            pbAdvanced.Location = new Point(456, 7);
            pbAdvanced.Margin = new Padding(4, 3, 4, 3);
            pbAdvanced.Name = "pbAdvanced";
            pbAdvanced.Size = new Size(50, 46);
            pbAdvanced.SizeMode = PictureBoxSizeMode.CenterImage;
            pbAdvanced.TabIndex = 41;
            pbAdvanced.TabStop = false;
            pbAdvanced.Click += pbAdvanced_Click;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(label14);
            tabPage3.Controls.Add(lbSourceAliases);
            tabPage3.Controls.Add(label10);
            tabPage3.Controls.Add(label8);
            tabPage3.Controls.Add(label4);
            tabPage3.Controls.Add(bnRemoveAlias);
            tabPage3.Controls.Add(bnAddAlias);
            tabPage3.Controls.Add(tbShowAlias);
            tabPage3.Controls.Add(pbAliases);
            tabPage3.Controls.Add(lbShowAlias);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Margin = new Padding(4, 3, 4, 3);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(4, 3, 4, 3);
            tabPage3.Size = new Size(533, 542);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Aliases";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(7, 54);
            label14.Margin = new Padding(4, 0, 4, 0);
            label14.Name = "label14";
            label14.Size = new Size(82, 15);
            label14.TabIndex = 46;
            label14.Text = "Source Aliases";
            label14.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbSourceAliases
            // 
            lbSourceAliases.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lbSourceAliases.FormattingEnabled = true;
            lbSourceAliases.Location = new Point(4, 73);
            lbSourceAliases.Margin = new Padding(4, 3, 4, 3);
            lbSourceAliases.Name = "lbSourceAliases";
            lbSourceAliases.SelectionMode = SelectionMode.MultiExtended;
            lbSourceAliases.Size = new Size(511, 94);
            lbSourceAliases.TabIndex = 45;
            // 
            // label10
            // 
            label10.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label10.AutoSize = true;
            label10.Location = new Point(7, 6);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(420, 45);
            label10.TabIndex = 44;
            label10.Text = "Setup other names that this show is sometimes known as or referred to. \r\nUse this if the files of the show use an abbreviated name and not the full show \r\nname.";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(7, 222);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(88, 15);
            label8.TabIndex = 9;
            label8.Text = "Custom Aliases";
            label8.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(7, 193);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(59, 15);
            label4.TabIndex = 8;
            label4.Text = "Alias Text:";
            label4.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // bnRemoveAlias
            // 
            bnRemoveAlias.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            bnRemoveAlias.Enabled = false;
            bnRemoveAlias.Location = new Point(419, 507);
            bnRemoveAlias.Margin = new Padding(4, 3, 4, 3);
            bnRemoveAlias.Name = "bnRemoveAlias";
            bnRemoveAlias.Size = new Size(97, 27);
            bnRemoveAlias.TabIndex = 3;
            bnRemoveAlias.Text = "&Remove Alias";
            bnRemoveAlias.UseVisualStyleBackColor = true;
            bnRemoveAlias.Click += bnRemoveAlias_Click;
            // 
            // bnAddAlias
            // 
            bnAddAlias.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnAddAlias.Enabled = false;
            bnAddAlias.Location = new Point(419, 187);
            bnAddAlias.Margin = new Padding(4, 3, 4, 3);
            bnAddAlias.Name = "bnAddAlias";
            bnAddAlias.Size = new Size(97, 27);
            bnAddAlias.TabIndex = 2;
            bnAddAlias.Text = "&Add Alias";
            bnAddAlias.UseVisualStyleBackColor = true;
            bnAddAlias.Click += bnAddAlias_Click;
            // 
            // tbShowAlias
            // 
            tbShowAlias.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tbShowAlias.Location = new Point(79, 189);
            tbShowAlias.Margin = new Padding(4, 3, 4, 3);
            tbShowAlias.Name = "tbShowAlias";
            tbShowAlias.Size = new Size(332, 23);
            tbShowAlias.TabIndex = 1;
            tbShowAlias.TextChanged += tbShowAlias_TextChanged;
            tbShowAlias.KeyDown += tbShowAlias_KeyDown;
            // 
            // pbAliases
            // 
            pbAliases.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbAliases.Cursor = Cursors.Hand;
            pbAliases.Image = (Image)resources.GetObject("pbAliases.Image");
            pbAliases.InitialImage = (Image)resources.GetObject("pbAliases.InitialImage");
            pbAliases.Location = new Point(456, 7);
            pbAliases.Margin = new Padding(4, 3, 4, 3);
            pbAliases.Name = "pbAliases";
            pbAliases.Size = new Size(50, 46);
            pbAliases.SizeMode = PictureBoxSizeMode.CenterImage;
            pbAliases.TabIndex = 43;
            pbAliases.TabStop = false;
            pbAliases.Click += pbAliases_Click;
            // 
            // lbShowAlias
            // 
            lbShowAlias.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lbShowAlias.FormattingEnabled = true;
            lbShowAlias.Location = new Point(4, 240);
            lbShowAlias.Margin = new Padding(4, 3, 4, 3);
            lbShowAlias.Name = "lbShowAlias";
            lbShowAlias.SelectionMode = SelectionMode.MultiExtended;
            lbShowAlias.Size = new Size(511, 259);
            lbShowAlias.TabIndex = 0;
            lbShowAlias.SelectedIndexChanged += lbShowAlias_SelectedIndexChanged;
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(label2);
            tabPage5.Controls.Add(cbFolderType);
            tabPage5.Controls.Add(txtIgnoreList);
            tabPage5.Controls.Add(btnIgnoreList);
            tabPage5.Controls.Add(groupBox1);
            tabPage5.Controls.Add(label12);
            tabPage5.Controls.Add(chkAutoFolders);
            tabPage5.Controls.Add(gbAutoFolders);
            tabPage5.Controls.Add(pbFolders);
            tabPage5.Location = new Point(4, 24);
            tabPage5.Margin = new Padding(4, 3, 4, 3);
            tabPage5.Name = "tabPage5";
            tabPage5.Padding = new Padding(4, 3, 4, 3);
            tabPage5.Size = new Size(533, 542);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "Folders";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(7, 63);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(71, 15);
            label2.TabIndex = 31;
            label2.Text = "Folder Type:";
            label2.TextAlign = ContentAlignment.TopRight;
            // 
            // cbFolderType
            // 
            cbFolderType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbFolderType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbFolderType.FormattingEnabled = true;
            cbFolderType.Location = new Point(130, 60);
            cbFolderType.Margin = new Padding(4, 3, 4, 3);
            cbFolderType.Name = "cbFolderType";
            cbFolderType.Size = new Size(386, 23);
            cbFolderType.Sorted = true;
            cbFolderType.TabIndex = 54;
            // 
            // txtIgnoreList
            // 
            txtIgnoreList.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtIgnoreList.AutoSize = true;
            txtIgnoreList.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            txtIgnoreList.Location = new Point(13, 516);
            txtIgnoreList.Margin = new Padding(4, 0, 4, 0);
            txtIgnoreList.Name = "txtIgnoreList";
            txtIgnoreList.Size = new Size(259, 13);
            txtIgnoreList.TabIndex = 53;
            txtIgnoreList.Text = "Note: Some files in these folders are ignored";
            txtIgnoreList.TextAlign = ContentAlignment.TopRight;
            // 
            // btnIgnoreList
            // 
            btnIgnoreList.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnIgnoreList.BackColor = Color.Transparent;
            btnIgnoreList.Location = new Point(334, 510);
            btnIgnoreList.Margin = new Padding(4, 3, 4, 3);
            btnIgnoreList.Name = "btnIgnoreList";
            btnIgnoreList.Size = new Size(178, 27);
            btnIgnoreList.TabIndex = 52;
            btnIgnoreList.Text = "See Ignore List";
            btnIgnoreList.UseVisualStyleBackColor = false;
            btnIgnoreList.Click += btnIgnoreList_Click_1;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(chkManualFolders);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(bnRemove);
            groupBox1.Controls.Add(bnAdd);
            groupBox1.Controls.Add(bnBrowseFolder);
            groupBox1.Controls.Add(txtFolder);
            groupBox1.Controls.Add(lvManualFolders);
            groupBox1.Location = new Point(7, 258);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(510, 200);
            groupBox1.TabIndex = 51;
            groupBox1.TabStop = false;
            // 
            // chkManualFolders
            // 
            chkManualFolders.AutoSize = true;
            chkManualFolders.Checked = true;
            chkManualFolders.CheckState = CheckState.Checked;
            chkManualFolders.Location = new Point(10, 0);
            chkManualFolders.Margin = new Padding(4, 3, 4, 3);
            chkManualFolders.Name = "chkManualFolders";
            chkManualFolders.Size = new Size(167, 19);
            chkManualFolders.TabIndex = 54;
            chkManualFolders.Text = "&Manual/Additional Folders";
            chkManualFolders.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button1.Location = new Point(413, 128);
            button1.Margin = new Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new Size(88, 27);
            button1.TabIndex = 12;
            button1.Text = "Open";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(10, 33);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(43, 15);
            label7.TabIndex = 2;
            label7.Text = "Folder:";
            // 
            // bnRemove
            // 
            bnRemove.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            bnRemove.Location = new Point(413, 162);
            bnRemove.Margin = new Padding(4, 3, 4, 3);
            bnRemove.Name = "bnRemove";
            bnRemove.Size = new Size(88, 27);
            bnRemove.TabIndex = 7;
            bnRemove.Text = "Remo&ve";
            bnRemove.UseVisualStyleBackColor = true;
            bnRemove.Click += bnRemove_Click_1;
            // 
            // bnAdd
            // 
            bnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnAdd.Location = new Point(413, 28);
            bnAdd.Margin = new Padding(4, 3, 4, 3);
            bnAdd.Name = "bnAdd";
            bnAdd.Size = new Size(88, 27);
            bnAdd.TabIndex = 5;
            bnAdd.Text = "&Add";
            bnAdd.UseVisualStyleBackColor = true;
            bnAdd.Click += bnAdd_Click_1;
            // 
            // bnBrowseFolder
            // 
            bnBrowseFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnBrowseFolder.Location = new Point(318, 28);
            bnBrowseFolder.Margin = new Padding(4, 3, 4, 3);
            bnBrowseFolder.Name = "bnBrowseFolder";
            bnBrowseFolder.Size = new Size(88, 27);
            bnBrowseFolder.TabIndex = 4;
            bnBrowseFolder.Text = "B&rowse...";
            bnBrowseFolder.UseVisualStyleBackColor = true;
            bnBrowseFolder.Click += bnBrowseFolder_Click_1;
            // 
            // txtFolder
            // 
            txtFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFolder.Location = new Point(71, 30);
            txtFolder.Margin = new Padding(4, 3, 4, 3);
            txtFolder.Name = "txtFolder";
            txtFolder.Size = new Size(240, 23);
            txtFolder.TabIndex = 3;
            // 
            // lvManualFolders
            // 
            lvManualFolders.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvManualFolders.Columns.AddRange(new ColumnHeader[] { columnHeader2 });
            lvManualFolders.FullRowSelect = true;
            lvManualFolders.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvManualFolders.Location = new Point(13, 61);
            lvManualFolders.Margin = new Padding(4, 3, 4, 3);
            lvManualFolders.Name = "lvManualFolders";
            lvManualFolders.Size = new Size(392, 131);
            lvManualFolders.TabIndex = 6;
            lvManualFolders.UseCompatibleStateImageBehavior = false;
            lvManualFolders.View = View.Details;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Folder";
            columnHeader2.Width = 250;
            // 
            // label12
            // 
            label12.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label12.AutoSize = true;
            label12.Location = new Point(7, 6);
            label12.Margin = new Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new Size(365, 30);
            label12.TabIndex = 48;
            label12.Text = "Setup which folders the episodes for this series should be stored in.  \r\nYou can choose automatic folders or maintain full manual control.";
            // 
            // chkAutoFolders
            // 
            chkAutoFolders.AutoSize = true;
            chkAutoFolders.Checked = true;
            chkAutoFolders.CheckState = CheckState.Checked;
            chkAutoFolders.Location = new Point(16, 96);
            chkAutoFolders.Margin = new Padding(4, 3, 4, 3);
            chkAutoFolders.Name = "chkAutoFolders";
            chkAutoFolders.Size = new Size(123, 19);
            chkAutoFolders.TabIndex = 10;
            chkAutoFolders.Text = "&Automatic Folders";
            chkAutoFolders.UseVisualStyleBackColor = true;
            chkAutoFolders.CheckedChanged += chkAutoFolders_CheckedChanged;
            // 
            // gbAutoFolders
            // 
            gbAutoFolders.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbAutoFolders.Controls.Add(cbDirectory);
            gbAutoFolders.Controls.Add(label1);
            gbAutoFolders.Controls.Add(txtFolderNameFormat);
            gbAutoFolders.Controls.Add(bnTags);
            gbAutoFolders.Controls.Add(lblSeasonWordPreview);
            gbAutoFolders.Controls.Add(rdoFolderCustom);
            gbAutoFolders.Controls.Add(rdoFolderLibraryDefault);
            gbAutoFolders.Controls.Add(label3);
            gbAutoFolders.Location = new Point(7, 97);
            gbAutoFolders.Margin = new Padding(4, 3, 4, 3);
            gbAutoFolders.Name = "gbAutoFolders";
            gbAutoFolders.Padding = new Padding(4, 3, 4, 3);
            gbAutoFolders.Size = new Size(510, 155);
            gbAutoFolders.TabIndex = 11;
            gbAutoFolders.TabStop = false;
            // 
            // cbDirectory
            // 
            cbDirectory.DropDownStyle = ComboBoxStyle.DropDownList;
            cbDirectory.FormattingEnabled = true;
            cbDirectory.Location = new Point(114, 36);
            cbDirectory.Margin = new Padding(4, 3, 4, 3);
            cbDirectory.Name = "cbDirectory";
            cbDirectory.Size = new Size(386, 23);
            cbDirectory.Sorted = true;
            cbDirectory.TabIndex = 30;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 72);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(114, 15);
            label1.TabIndex = 29;
            label1.Text = "Movie Folder Name:";
            label1.TextAlign = ContentAlignment.TopRight;
            // 
            // txtFolderNameFormat
            // 
            txtFolderNameFormat.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFolderNameFormat.Location = new Point(172, 108);
            txtFolderNameFormat.Margin = new Padding(4, 3, 4, 3);
            txtFolderNameFormat.Name = "txtFolderNameFormat";
            txtFolderNameFormat.Size = new Size(234, 23);
            txtFolderNameFormat.TabIndex = 28;
            txtFolderNameFormat.TextChanged += txtFolderNameFormat_TextChanged;
            // 
            // bnTags
            // 
            bnTags.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bnTags.Location = new Point(413, 108);
            bnTags.Margin = new Padding(4, 3, 4, 3);
            bnTags.Name = "bnTags";
            bnTags.Size = new Size(88, 27);
            bnTags.TabIndex = 27;
            bnTags.Text = "Tags...";
            bnTags.UseVisualStyleBackColor = true;
            bnTags.Click += bnTags_Click;
            // 
            // lblSeasonWordPreview
            // 
            lblSeasonWordPreview.AutoSize = true;
            lblSeasonWordPreview.Enabled = false;
            lblSeasonWordPreview.Location = new Point(170, 91);
            lblSeasonWordPreview.Margin = new Padding(4, 0, 4, 0);
            lblSeasonWordPreview.Name = "lblSeasonWordPreview";
            lblSeasonWordPreview.Size = new Size(44, 15);
            lblSeasonWordPreview.TabIndex = 11;
            lblSeasonWordPreview.Text = "label10";
            // 
            // rdoFolderCustom
            // 
            rdoFolderCustom.AutoSize = true;
            rdoFolderCustom.Location = new Point(13, 111);
            rdoFolderCustom.Margin = new Padding(4, 3, 4, 3);
            rdoFolderCustom.Name = "rdoFolderCustom";
            rdoFolderCustom.Size = new Size(145, 19);
            rdoFolderCustom.TabIndex = 7;
            rdoFolderCustom.TabStop = true;
            rdoFolderCustom.Text = "Custom Subdirectories";
            rdoFolderCustom.UseVisualStyleBackColor = true;
            // 
            // rdoFolderLibraryDefault
            // 
            rdoFolderLibraryDefault.AutoSize = true;
            rdoFolderLibraryDefault.Location = new Point(13, 90);
            rdoFolderLibraryDefault.Margin = new Padding(4, 3, 4, 3);
            rdoFolderLibraryDefault.Name = "rdoFolderLibraryDefault";
            rdoFolderLibraryDefault.Size = new Size(148, 19);
            rdoFolderLibraryDefault.TabIndex = 6;
            rdoFolderLibraryDefault.TabStop = true;
            rdoFolderLibraryDefault.Text = "Subdirectories (default)";
            rdoFolderLibraryDefault.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 39);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(67, 15);
            label3.TabIndex = 0;
            label3.Text = "Base &Folder";
            label3.TextAlign = ContentAlignment.TopRight;
            // 
            // pbFolders
            // 
            pbFolders.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbFolders.Cursor = Cursors.Hand;
            pbFolders.Image = (Image)resources.GetObject("pbFolders.Image");
            pbFolders.InitialImage = (Image)resources.GetObject("pbFolders.InitialImage");
            pbFolders.Location = new Point(463, 7);
            pbFolders.Margin = new Padding(4, 3, 4, 3);
            pbFolders.Name = "pbFolders";
            pbFolders.Size = new Size(50, 46);
            pbFolders.SizeMode = PictureBoxSizeMode.CenterImage;
            pbFolders.TabIndex = 47;
            pbFolders.TabStop = false;
            pbFolders.Click += pbFolders_Click;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(cbRegion);
            tabPage1.Controls.Add(chkCustomRegion);
            tabPage1.Controls.Add(rdoTMDB);
            tabPage1.Controls.Add(label13);
            tabPage1.Controls.Add(rdoTVDB);
            tabPage1.Controls.Add(rdoDefault);
            tabPage1.Controls.Add(label60);
            tabPage1.Controls.Add(pbBasics);
            tabPage1.Controls.Add(cbLanguage);
            tabPage1.Controls.Add(chkCustomLanguage);
            tabPage1.Controls.Add(pnlCF);
            tabPage1.Controls.Add(chkCustomShowName);
            tabPage1.Controls.Add(txtCustomShowName);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Margin = new Padding(4, 3, 4, 3);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(4, 3, 4, 3);
            tabPage1.Size = new Size(533, 542);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Basics";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // cbRegion
            // 
            cbRegion.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cbRegion.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRegion.FormattingEnabled = true;
            cbRegion.Location = new Point(156, 445);
            cbRegion.Margin = new Padding(4, 3, 4, 3);
            cbRegion.Name = "cbRegion";
            cbRegion.Size = new Size(233, 23);
            cbRegion.TabIndex = 47;
            cbRegion.SelectedIndexChanged += cbRegion_SelectedIndexChanged;
            // 
            // chkCustomRegion
            // 
            chkCustomRegion.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chkCustomRegion.AutoSize = true;
            chkCustomRegion.Location = new Point(12, 451);
            chkCustomRegion.Margin = new Padding(4, 3, 4, 3);
            chkCustomRegion.Name = "chkCustomRegion";
            chkCustomRegion.Size = new Size(111, 19);
            chkCustomRegion.TabIndex = 46;
            chkCustomRegion.Text = "Custom Region:";
            chkCustomRegion.UseVisualStyleBackColor = true;
            chkCustomRegion.CheckedChanged += chkCustomRegion_CheckedChanged;
            // 
            // rdoTMDB
            // 
            rdoTMDB.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            rdoTMDB.AutoSize = true;
            rdoTMDB.Location = new Point(255, 351);
            rdoTMDB.Margin = new Padding(4, 3, 4, 3);
            rdoTMDB.Name = "rdoTMDB";
            rdoTMDB.Size = new Size(58, 19);
            rdoTMDB.TabIndex = 45;
            rdoTMDB.TabStop = true;
            rdoTMDB.Text = "TMDB";
            rdoTMDB.UseVisualStyleBackColor = true;
            rdoTMDB.CheckedChanged += rdoProvider_CheckedChanged;
            // 
            // label13
            // 
            label13.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label13.AutoSize = true;
            label13.Location = new Point(8, 353);
            label13.Margin = new Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new Size(46, 15);
            label13.TabIndex = 44;
            label13.Text = "Source:";
            label13.TextAlign = ContentAlignment.TopRight;
            // 
            // rdoTVDB
            // 
            rdoTVDB.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            rdoTVDB.AutoSize = true;
            rdoTVDB.Location = new Point(172, 351);
            rdoTVDB.Margin = new Padding(4, 3, 4, 3);
            rdoTVDB.Name = "rdoTVDB";
            rdoTVDB.Size = new Size(77, 19);
            rdoTVDB.TabIndex = 42;
            rdoTVDB.TabStop = true;
            rdoTVDB.Text = "The TVDB";
            rdoTVDB.UseVisualStyleBackColor = true;
            rdoTVDB.CheckedChanged += rdoProvider_CheckedChanged;
            // 
            // rdoDefault
            // 
            rdoDefault.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            rdoDefault.AutoSize = true;
            rdoDefault.Location = new Point(61, 351);
            rdoDefault.Margin = new Padding(4, 3, 4, 3);
            rdoDefault.Name = "rdoDefault";
            rdoDefault.Size = new Size(102, 19);
            rdoDefault.TabIndex = 41;
            rdoDefault.TabStop = true;
            rdoDefault.Text = "Library Default";
            rdoDefault.UseVisualStyleBackColor = true;
            rdoDefault.CheckedChanged += rdoProvider_CheckedChanged;
            // 
            // label60
            // 
            label60.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label60.AutoSize = true;
            label60.Location = new Point(7, 7);
            label60.Margin = new Padding(4, 0, 4, 0);
            label60.Name = "label60";
            label60.Size = new Size(411, 15);
            label60.TabIndex = 40;
            label60.Text = "Use these settings to control the link to the show and what the show is called";
            // 
            // pbBasics
            // 
            pbBasics.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbBasics.Cursor = Cursors.Hand;
            pbBasics.Image = (Image)resources.GetObject("pbBasics.Image");
            pbBasics.InitialImage = (Image)resources.GetObject("pbBasics.InitialImage");
            pbBasics.Location = new Point(468, 7);
            pbBasics.Margin = new Padding(4, 3, 4, 3);
            pbBasics.Name = "pbBasics";
            pbBasics.Size = new Size(50, 46);
            pbBasics.SizeMode = PictureBoxSizeMode.CenterImage;
            pbBasics.TabIndex = 39;
            pbBasics.TabStop = false;
            pbBasics.Click += pbBasics_Click;
            // 
            // cbLanguage
            // 
            cbLanguage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            cbLanguage.FormattingEnabled = true;
            cbLanguage.Location = new Point(156, 414);
            cbLanguage.Margin = new Padding(4, 3, 4, 3);
            cbLanguage.Name = "cbLanguage";
            cbLanguage.Size = new Size(233, 23);
            cbLanguage.TabIndex = 9;
            cbLanguage.SelectedIndexChanged += cbLanguage_SelectedIndexChanged;
            // 
            // chkCustomLanguage
            // 
            chkCustomLanguage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chkCustomLanguage.AutoSize = true;
            chkCustomLanguage.Location = new Point(12, 420);
            chkCustomLanguage.Margin = new Padding(4, 3, 4, 3);
            chkCustomLanguage.Name = "chkCustomLanguage";
            chkCustomLanguage.Size = new Size(126, 19);
            chkCustomLanguage.TabIndex = 8;
            chkCustomLanguage.Text = "Custom Language:";
            chkCustomLanguage.UseVisualStyleBackColor = true;
            chkCustomLanguage.CheckedChanged += chkCustomLanguage_CheckedChanged;
            // 
            // pnlCF
            // 
            pnlCF.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlCF.Location = new Point(4, 60);
            pnlCF.Margin = new Padding(4, 3, 4, 3);
            pnlCF.Name = "pnlCF";
            pnlCF.Size = new Size(518, 282);
            pnlCF.TabIndex = 0;
            // 
            // chkCustomShowName
            // 
            chkCustomShowName.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chkCustomShowName.AutoSize = true;
            chkCustomShowName.Location = new Point(12, 387);
            chkCustomShowName.Margin = new Padding(4, 3, 4, 3);
            chkCustomShowName.Name = "chkCustomShowName";
            chkCustomShowName.Size = new Size(135, 19);
            chkCustomShowName.TabIndex = 1;
            chkCustomShowName.Text = "Custom s&how name:";
            chkCustomShowName.UseVisualStyleBackColor = true;
            chkCustomShowName.CheckedChanged += chkCustomShowName_CheckedChanged;
            // 
            // txtCustomShowName
            // 
            txtCustomShowName.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtCustomShowName.Location = new Point(156, 384);
            txtCustomShowName.Margin = new Padding(4, 3, 4, 3);
            txtCustomShowName.Name = "txtCustomShowName";
            txtCustomShowName.Size = new Size(353, 23);
            txtCustomShowName.TabIndex = 2;
            // 
            // Folders
            // 
            Folders.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Folders.Controls.Add(tabPage1);
            Folders.Controls.Add(tabPage5);
            Folders.Controls.Add(tabPage6);
            Folders.Controls.Add(tabPage3);
            Folders.Controls.Add(tabPage2);
            Folders.Location = new Point(-5, 2);
            Folders.Margin = new Padding(4, 3, 4, 3);
            Folders.Name = "Folders";
            Folders.SelectedIndex = 0;
            Folders.Size = new Size(541, 570);
            Folders.TabIndex = 0;
            // 
            // tabPage6
            // 
            tabPage6.Controls.Add(llFilenameDefaultFormat);
            tabPage6.Controls.Add(llCustomName);
            tabPage6.Controls.Add(lbLibraryDefaultNaming);
            tabPage6.Controls.Add(label15);
            tabPage6.Controls.Add(lbNamingExample);
            tabPage6.Controls.Add(txtCustomMovieFileNamingFormat);
            tabPage6.Controls.Add(txtTagList2);
            tabPage6.Controls.Add(lbAvailableTags);
            tabPage6.Controls.Add(label19);
            tabPage6.Controls.Add(cbUseCustomNamingFormat);
            tabPage6.Controls.Add(pictureBox1);
            tabPage6.Location = new Point(4, 24);
            tabPage6.Margin = new Padding(4, 3, 4, 3);
            tabPage6.Name = "tabPage6";
            tabPage6.Padding = new Padding(4, 3, 4, 3);
            tabPage6.Size = new Size(533, 542);
            tabPage6.TabIndex = 6;
            tabPage6.Text = "Custom File Naming";
            tabPage6.UseVisualStyleBackColor = true;
            // 
            // llFilenameDefaultFormat
            // 
            llFilenameDefaultFormat.AutoSize = true;
            llFilenameDefaultFormat.Location = new Point(121, 73);
            llFilenameDefaultFormat.Margin = new Padding(4, 0, 4, 0);
            llFilenameDefaultFormat.Name = "llFilenameDefaultFormat";
            llFilenameDefaultFormat.Size = new Size(0, 15);
            llFilenameDefaultFormat.TabIndex = 57;
            // 
            // llCustomName
            // 
            llCustomName.AutoSize = true;
            llCustomName.Location = new Point(121, 134);
            llCustomName.Margin = new Padding(4, 0, 4, 0);
            llCustomName.Name = "llCustomName";
            llCustomName.Size = new Size(0, 15);
            llCustomName.TabIndex = 56;
            // 
            // lbLibraryDefaultNaming
            // 
            lbLibraryDefaultNaming.AutoSize = true;
            lbLibraryDefaultNaming.Location = new Point(34, 73);
            lbLibraryDefaultNaming.Margin = new Padding(4, 0, 4, 0);
            lbLibraryDefaultNaming.Name = "lbLibraryDefaultNaming";
            lbLibraryDefaultNaming.Size = new Size(48, 15);
            lbLibraryDefaultNaming.TabIndex = 55;
            lbLibraryDefaultNaming.Text = "Default:";
            // 
            // label15
            // 
            label15.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label15.AutoSize = true;
            label15.Location = new Point(9, 6);
            label15.Margin = new Padding(4, 0, 4, 0);
            label15.Name = "label15";
            label15.Size = new Size(230, 15);
            label15.TabIndex = 54;
            label15.Text = "Setup a naming format just for this movie.";
            // 
            // lbNamingExample
            // 
            lbNamingExample.AutoSize = true;
            lbNamingExample.Location = new Point(34, 134);
            lbNamingExample.Margin = new Padding(4, 0, 4, 0);
            lbNamingExample.Name = "lbNamingExample";
            lbNamingExample.Size = new Size(54, 15);
            lbNamingExample.TabIndex = 52;
            lbNamingExample.Text = "Example:";
            // 
            // txtCustomMovieFileNamingFormat
            // 
            txtCustomMovieFileNamingFormat.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtCustomMovieFileNamingFormat.Location = new Point(125, 100);
            txtCustomMovieFileNamingFormat.Margin = new Padding(4, 3, 4, 3);
            txtCustomMovieFileNamingFormat.Name = "txtCustomMovieFileNamingFormat";
            txtCustomMovieFileNamingFormat.Size = new Size(383, 23);
            txtCustomMovieFileNamingFormat.TabIndex = 51;
            txtCustomMovieFileNamingFormat.TextChanged += txtCustomMovieFileNamingFormat_TextChanged;
            // 
            // txtTagList2
            // 
            txtTagList2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtTagList2.Location = new Point(48, 192);
            txtTagList2.Margin = new Padding(4, 0, 4, 0);
            txtTagList2.Name = "txtTagList2";
            txtTagList2.Size = new Size(421, 323);
            txtTagList2.TabIndex = 48;
            txtTagList2.Text = "<tags>";
            // 
            // lbAvailableTags
            // 
            lbAvailableTags.AutoSize = true;
            lbAvailableTags.Location = new Point(34, 165);
            lbAvailableTags.Margin = new Padding(4, 0, 4, 0);
            lbAvailableTags.Name = "lbAvailableTags";
            lbAvailableTags.Size = new Size(85, 15);
            lbAvailableTags.TabIndex = 49;
            lbAvailableTags.Tag = "";
            lbAvailableTags.Text = "Available Tags:";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(34, 104);
            label19.Margin = new Padding(4, 0, 4, 0);
            label19.Name = "label19";
            label19.Size = new Size(93, 15);
            label19.TabIndex = 50;
            label19.Text = "Custom Format:";
            // 
            // cbUseCustomNamingFormat
            // 
            cbUseCustomNamingFormat.AutoSize = true;
            cbUseCustomNamingFormat.Location = new Point(12, 50);
            cbUseCustomNamingFormat.Margin = new Padding(4, 3, 4, 3);
            cbUseCustomNamingFormat.Name = "cbUseCustomNamingFormat";
            cbUseCustomNamingFormat.Size = new Size(177, 19);
            cbUseCustomNamingFormat.TabIndex = 47;
            cbUseCustomNamingFormat.Text = "Use Custom Naming Format";
            cbUseCustomNamingFormat.UseVisualStyleBackColor = true;
            cbUseCustomNamingFormat.CheckedChanged += cbUseCustomNamingFormat_CheckedChanged;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureBox1.Cursor = Cursors.Hand;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.InitialImage = (Image)resources.GetObject("pictureBox1.InitialImage");
            pictureBox1.Location = new Point(458, 7);
            pictureBox1.Margin = new Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(50, 46);
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox1.TabIndex = 53;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pbCustomFN_Click;
            // 
            // AddEditMovie
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = bnCancel;
            ClientSize = new Size(539, 620);
            ControlBox = false;
            Controls.Add(Folders);
            Controls.Add(bnCancel);
            Controls.Add(buttonOK);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(534, 505);
            Name = "AddEditMovie";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Add/Edit Movie";
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbAdvanced).EndInit();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbAliases).EndInit();
            tabPage5.ResumeLayout(false);
            tabPage5.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            gbAutoFolders.ResumeLayout(false);
            gbAutoFolders.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbFolders).EndInit();
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbBasics).EndInit();
            Folders.ResumeLayout(false);
            tabPage6.ResumeLayout(false);
            tabPage6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox cbDoRenaming;
        private System.Windows.Forms.CheckBox cbDoMissingCheck;
        private System.Windows.Forms.PictureBox pbAdvanced;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ListBox lbSourceAliases;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bnRemoveAlias;
        private System.Windows.Forms.Button bnAddAlias;
        private System.Windows.Forms.TextBox tbShowAlias;
        private System.Windows.Forms.PictureBox pbAliases;
        private System.Windows.Forms.ListBox lbShowAlias;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox chkAutoFolders;
        private System.Windows.Forms.GroupBox gbAutoFolders;
        private System.Windows.Forms.TextBox txtFolderNameFormat;
        private System.Windows.Forms.Button bnTags;
        private System.Windows.Forms.Label lblSeasonWordPreview;
        private System.Windows.Forms.RadioButton rdoFolderCustom;
        private System.Windows.Forms.RadioButton rdoFolderLibraryDefault;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pbFolders;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.RadioButton rdoTVDB;
        private System.Windows.Forms.RadioButton rdoDefault;
        private System.Windows.Forms.Label label60;
        private System.Windows.Forms.PictureBox pbBasics;
        private System.Windows.Forms.ComboBox cbLanguage;
        private System.Windows.Forms.CheckBox chkCustomLanguage;
        private System.Windows.Forms.Panel pnlCF;
        private System.Windows.Forms.CheckBox chkCustomShowName;
        private System.Windows.Forms.TextBox txtCustomShowName;
        private System.Windows.Forms.TabControl Folders;
        private System.Windows.Forms.RadioButton rdoTMDB;
        private System.Windows.Forms.ComboBox cbRegion;
        private System.Windows.Forms.CheckBox chkCustomRegion;
        private System.Windows.Forms.Label txtIgnoreList;
        private System.Windows.Forms.Button btnIgnoreList;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button bnRemove;
        private System.Windows.Forms.Button bnAdd;
        private System.Windows.Forms.Button bnBrowseFolder;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.ListView lvManualFolders;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.CheckBox chkManualFolders;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.LinkLabel llFilenameDefaultFormat;
        private System.Windows.Forms.LinkLabel llCustomName;
        private System.Windows.Forms.Label lbLibraryDefaultNaming;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label lbNamingExample;
        private System.Windows.Forms.TextBox txtCustomMovieFileNamingFormat;
        private System.Windows.Forms.Label txtTagList2;
        private System.Windows.Forms.Label lbAvailableTags;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.CheckBox cbUseCustomNamingFormat;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox cbDirectory;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbFolderType;
        private System.Windows.Forms.CheckBox cbIncludeNoAirdate;
        private System.Windows.Forms.CheckBox cbIncludeFuture;
    }
}
