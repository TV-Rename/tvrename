//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename
{
    partial class EditSeason
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditSeason));
            this.bnRuleUp = new System.Windows.Forms.Button();
            this.bnRuleDown = new System.Windows.Forms.Button();
            this.lvRuleList = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.bnDelRule = new System.Windows.Forms.Button();
            this.bnEdit = new System.Windows.Forms.Button();
            this.bnAddRule = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbEpsPreview = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.txtShowName = new System.Windows.Forms.Label();
            this.txtSeasonNumber = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Rules = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.lvSeenEpisodes = new System.Windows.Forms.ListView();
            this.Episode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.EpisodeName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.bnRemoveSeen = new System.Windows.Forms.Button();
            this.bnAddSeen = new System.Windows.Forms.Button();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabControl1.SuspendLayout();
            this.Rules.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // bnRuleUp
            // 
            this.bnRuleUp.Location = new System.Drawing.Point(250, 198);
            this.bnRuleUp.Name = "bnRuleUp";
            this.bnRuleUp.Size = new System.Drawing.Size(75, 23);
            this.bnRuleUp.TabIndex = 9;
            this.bnRuleUp.Text = "&Up";
            this.bnRuleUp.UseVisualStyleBackColor = true;
            this.bnRuleUp.Click += new System.EventHandler(this.bnRuleUp_Click);
            // 
            // bnRuleDown
            // 
            this.bnRuleDown.Location = new System.Drawing.Point(331, 198);
            this.bnRuleDown.Name = "bnRuleDown";
            this.bnRuleDown.Size = new System.Drawing.Size(75, 23);
            this.bnRuleDown.TabIndex = 10;
            this.bnRuleDown.Text = "Do&wn";
            this.bnRuleDown.UseVisualStyleBackColor = true;
            this.bnRuleDown.Click += new System.EventHandler(this.bnRuleDown_Click);
            // 
            // lvRuleList
            // 
            this.lvRuleList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvRuleList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader1});
            this.lvRuleList.FullRowSelect = true;
            this.lvRuleList.HideSelection = false;
            this.lvRuleList.Location = new System.Drawing.Point(7, 19);
            this.lvRuleList.MultiSelect = false;
            this.lvRuleList.Name = "lvRuleList";
            this.lvRuleList.Size = new System.Drawing.Size(400, 172);
            this.lvRuleList.TabIndex = 5;
            this.lvRuleList.UseCompatibleStateImageBehavior = false;
            this.lvRuleList.View = System.Windows.Forms.View.Details;
            this.lvRuleList.SelectedIndexChanged += new System.EventHandler(this.LvRuleList_SelectedIndexChanged);
            this.lvRuleList.DoubleClick += new System.EventHandler(this.lvRuleList_DoubleClick);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Action";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Episode";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Episode";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Name";
            this.columnHeader6.Width = 150;
            // 
            // bnDelRule
            // 
            this.bnDelRule.Location = new System.Drawing.Point(169, 198);
            this.bnDelRule.Name = "bnDelRule";
            this.bnDelRule.Size = new System.Drawing.Size(75, 23);
            this.bnDelRule.TabIndex = 8;
            this.bnDelRule.Text = "&Delete";
            this.bnDelRule.UseVisualStyleBackColor = true;
            this.bnDelRule.Click += new System.EventHandler(this.bnDelRule_Click);
            // 
            // bnEdit
            // 
            this.bnEdit.Location = new System.Drawing.Point(88, 198);
            this.bnEdit.Name = "bnEdit";
            this.bnEdit.Size = new System.Drawing.Size(75, 23);
            this.bnEdit.TabIndex = 7;
            this.bnEdit.Text = "&Edit";
            this.bnEdit.UseVisualStyleBackColor = true;
            this.bnEdit.Click += new System.EventHandler(this.bnEdit_Click);
            // 
            // bnAddRule
            // 
            this.bnAddRule.Location = new System.Drawing.Point(7, 198);
            this.bnAddRule.Name = "bnAddRule";
            this.bnAddRule.Size = new System.Drawing.Size(75, 23);
            this.bnAddRule.TabIndex = 6;
            this.bnAddRule.Text = "&Add";
            this.bnAddRule.UseVisualStyleBackColor = true;
            this.bnAddRule.Click += new System.EventHandler(this.bnAddRule_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "&Rules:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Show:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Season:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbEpsPreview
            // 
            this.lbEpsPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbEpsPreview.FormattingEnabled = true;
            this.lbEpsPreview.Location = new System.Drawing.Point(6, 238);
            this.lbEpsPreview.Name = "lbEpsPreview";
            this.lbEpsPreview.ScrollAlwaysVisible = true;
            this.lbEpsPreview.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lbEpsPreview.Size = new System.Drawing.Size(405, 238);
            this.lbEpsPreview.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 224);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Preview:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(353, 584);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 14;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // bnOK
            // 
            this.bnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(272, 584);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(75, 23);
            this.bnOK.TabIndex = 13;
            this.bnOK.Text = "OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // txtShowName
            // 
            this.txtShowName.AutoSize = true;
            this.txtShowName.Location = new System.Drawing.Point(56, 9);
            this.txtShowName.Name = "txtShowName";
            this.txtShowName.Size = new System.Drawing.Size(16, 13);
            this.txtShowName.TabIndex = 1;
            this.txtShowName.Text = "---";
            this.txtShowName.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtSeasonNumber
            // 
            this.txtSeasonNumber.AutoSize = true;
            this.txtSeasonNumber.Location = new System.Drawing.Point(56, 33);
            this.txtSeasonNumber.Name = "txtSeasonNumber";
            this.txtSeasonNumber.Size = new System.Drawing.Size(16, 13);
            this.txtSeasonNumber.TabIndex = 3;
            this.txtSeasonNumber.Text = "---";
            this.txtSeasonNumber.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.Rules);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(3, 62);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(425, 516);
            this.tabControl1.TabIndex = 15;
            // 
            // Rules
            // 
            this.Rules.Controls.Add(this.lbEpsPreview);
            this.Rules.Controls.Add(this.label1);
            this.Rules.Controls.Add(this.label4);
            this.Rules.Controls.Add(this.bnRuleUp);
            this.Rules.Controls.Add(this.bnRuleDown);
            this.Rules.Controls.Add(this.lvRuleList);
            this.Rules.Controls.Add(this.bnDelRule);
            this.Rules.Controls.Add(this.bnAddRule);
            this.Rules.Controls.Add(this.bnEdit);
            this.Rules.Location = new System.Drawing.Point(4, 22);
            this.Rules.Name = "Rules";
            this.Rules.Padding = new System.Windows.Forms.Padding(3);
            this.Rules.Size = new System.Drawing.Size(417, 490);
            this.Rules.TabIndex = 0;
            this.Rules.Text = "Rules";
            this.Rules.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.lvSeenEpisodes);
            this.tabPage2.Controls.Add(this.bnRemoveSeen);
            this.tabPage2.Controls.Add(this.bnAddSeen);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(417, 490);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Library Episodes";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(355, 52);
            this.label5.TabIndex = 12;
            this.label5.Text = resources.GetString("label5.Text");
            // 
            // lvSeenEpisodes
            // 
            this.lvSeenEpisodes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvSeenEpisodes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Episode,
            this.EpisodeName});
            this.lvSeenEpisodes.FullRowSelect = true;
            this.lvSeenEpisodes.HideSelection = false;
            this.lvSeenEpisodes.Location = new System.Drawing.Point(6, 78);
            this.lvSeenEpisodes.MultiSelect = false;
            this.lvSeenEpisodes.Name = "lvSeenEpisodes";
            this.lvSeenEpisodes.ShowGroups = false;
            this.lvSeenEpisodes.Size = new System.Drawing.Size(405, 377);
            this.lvSeenEpisodes.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvSeenEpisodes.TabIndex = 11;
            this.lvSeenEpisodes.UseCompatibleStateImageBehavior = false;
            this.lvSeenEpisodes.View = System.Windows.Forms.View.Details;
            this.lvSeenEpisodes.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.LvSeenEpisodes_ColumnClick);
            this.lvSeenEpisodes.SelectedIndexChanged += new System.EventHandler(this.LvSeenEpisodes_SelectedIndexChanged);
            // 
            // Episode
            // 
            this.Episode.Text = "Episode";
            this.Episode.Width = 98;
            // 
            // EpisodeName
            // 
            this.EpisodeName.Text = "Name";
            this.EpisodeName.Width = 260;
            // 
            // bnRemoveSeen
            // 
            this.bnRemoveSeen.Location = new System.Drawing.Point(336, 461);
            this.bnRemoveSeen.Name = "bnRemoveSeen";
            this.bnRemoveSeen.Size = new System.Drawing.Size(75, 23);
            this.bnRemoveSeen.TabIndex = 10;
            this.bnRemoveSeen.Text = "&Remove";
            this.bnRemoveSeen.UseVisualStyleBackColor = true;
            this.bnRemoveSeen.Click += new System.EventHandler(this.Button1_Click);
            // 
            // bnAddSeen
            // 
            this.bnAddSeen.Location = new System.Drawing.Point(255, 461);
            this.bnAddSeen.Name = "bnAddSeen";
            this.bnAddSeen.Size = new System.Drawing.Size(75, 23);
            this.bnAddSeen.TabIndex = 9;
            this.bnAddSeen.Text = "&Add";
            this.bnAddSeen.UseVisualStyleBackColor = true;
            this.bnAddSeen.Click += new System.EventHandler(this.Button2_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Reorder";
            // 
            // EditSeason
            // 
            this.AcceptButton = this.bnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(440, 619);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtShowName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSeasonNumber);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditSeason";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Season";
            this.tabControl1.ResumeLayout(false);
            this.Rules.ResumeLayout(false);
            this.Rules.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bnRuleUp;
        private System.Windows.Forms.Button bnRuleDown;
        private System.Windows.Forms.ListView lvRuleList;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Button bnDelRule;
        private System.Windows.Forms.Button bnEdit;
        private System.Windows.Forms.Button bnAddRule;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lbEpsPreview;

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bnCancel;

        private System.Windows.Forms.Button bnOK;

        private System.Windows.Forms.Label txtShowName;
        private System.Windows.Forms.Label txtSeasonNumber;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage Rules;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView lvSeenEpisodes;
        private System.Windows.Forms.Button bnRemoveSeen;
        private System.Windows.Forms.Button bnAddSeen;
        private System.Windows.Forms.ColumnHeader Episode;
        private System.Windows.Forms.ColumnHeader EpisodeName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}
