//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//

namespace TVRename
{
    partial class EditRules
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditRules));
            this.bnRuleUp = new System.Windows.Forms.Button();
            this.bnRuleDown = new System.Windows.Forms.Button();
            this.lvRuleList = new System.Windows.Forms.ListView();
            this.colRule = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colNum1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colNum2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
            this.lbEpsOriginal = new System.Windows.Forms.ListBox();
            this.txtPreviewLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.bnPreview = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bnRuleUp
            // 
            this.bnRuleUp.Location = new System.Drawing.Point(12, 160);
            this.bnRuleUp.Name = "bnRuleUp";
            this.bnRuleUp.Size = new System.Drawing.Size(75, 23);
            this.bnRuleUp.TabIndex = 11;
            this.bnRuleUp.Text = "&Up";
            this.bnRuleUp.UseVisualStyleBackColor = true;
            this.bnRuleUp.Click += new System.EventHandler(this.bnRuleUp_Click);
            // 
            // bnRuleDown
            // 
            this.bnRuleDown.Location = new System.Drawing.Point(12, 189);
            this.bnRuleDown.Name = "bnRuleDown";
            this.bnRuleDown.Size = new System.Drawing.Size(75, 23);
            this.bnRuleDown.TabIndex = 12;
            this.bnRuleDown.Text = "Do&wn";
            this.bnRuleDown.UseVisualStyleBackColor = true;
            this.bnRuleDown.Click += new System.EventHandler(this.bnRuleDown_Click);
            // 
            // lvRuleList
            // 
            this.lvRuleList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvRuleList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colRule,
            this.colAction,
            this.colNum1,
            this.colNum2,
            this.colDescription});
            this.lvRuleList.FullRowSelect = true;
            this.lvRuleList.HideSelection = false;
            this.lvRuleList.Location = new System.Drawing.Point(93, 73);
            this.lvRuleList.MultiSelect = false;
            this.lvRuleList.Name = "lvRuleList";
            this.lvRuleList.Size = new System.Drawing.Size(614, 168);
            this.lvRuleList.TabIndex = 8;
            this.lvRuleList.UseCompatibleStateImageBehavior = false;
            this.lvRuleList.View = System.Windows.Forms.View.Details;
            this.lvRuleList.DoubleClick += new System.EventHandler(this.lvRuleList_DoubleClick);
            // 
            // colRule
            // 
            this.colRule.Text = "#";
            this.colRule.Width = 25;
            // 
            // colAction
            // 
            this.colAction.Text = "Action";
            // 
            // colNum1
            // 
            this.colNum1.Text = "Episode";
            // 
            // colNum2
            // 
            this.colNum2.Text = "Ep/Num";
            // 
            // colDescription
            // 
            this.colDescription.Text = "Description";
            this.colDescription.Width = 386;
            // 
            // bnDelRule
            // 
            this.bnDelRule.Location = new System.Drawing.Point(12, 131);
            this.bnDelRule.Name = "bnDelRule";
            this.bnDelRule.Size = new System.Drawing.Size(75, 23);
            this.bnDelRule.TabIndex = 10;
            this.bnDelRule.Text = "&Delete";
            this.bnDelRule.UseVisualStyleBackColor = true;
            this.bnDelRule.Click += new System.EventHandler(this.bnDelRule_Click);
            // 
            // bnEdit
            // 
            this.bnEdit.Location = new System.Drawing.Point(12, 102);
            this.bnEdit.Name = "bnEdit";
            this.bnEdit.Size = new System.Drawing.Size(75, 23);
            this.bnEdit.TabIndex = 9;
            this.bnEdit.Text = "&Edit";
            this.bnEdit.UseVisualStyleBackColor = true;
            this.bnEdit.Click += new System.EventHandler(this.bnEdit_Click);
            // 
            // bnAddRule
            // 
            this.bnAddRule.Location = new System.Drawing.Point(12, 73);
            this.bnAddRule.Name = "bnAddRule";
            this.bnAddRule.Size = new System.Drawing.Size(75, 23);
            this.bnAddRule.TabIndex = 7;
            this.bnAddRule.Text = "&Add";
            this.bnAddRule.UseVisualStyleBackColor = true;
            this.bnAddRule.Click += new System.EventHandler(this.bnAddRule_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(90, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "&Rules:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Show:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Season:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbEpsPreview
            // 
            this.lbEpsPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbEpsPreview.FormattingEnabled = true;
            this.lbEpsPreview.HorizontalScrollbar = true;
            this.lbEpsPreview.IntegralHeight = false;
            this.lbEpsPreview.Location = new System.Drawing.Point(350, 16);
            this.lbEpsPreview.Name = "lbEpsPreview";
            this.lbEpsPreview.Size = new System.Drawing.Size(342, 206);
            this.lbEpsPreview.TabIndex = 3;
            this.lbEpsPreview.DoubleClick += new System.EventHandler(this.lbEpsPreview_DoubleClick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Original List:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(632, 478);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 1;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // bnOK
            // 
            this.bnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(551, 478);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(75, 23);
            this.bnOK.TabIndex = 0;
            this.bnOK.Text = "OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // txtShowName
            // 
            this.txtShowName.AutoSize = true;
            this.txtShowName.Location = new System.Drawing.Point(67, 6);
            this.txtShowName.Name = "txtShowName";
            this.txtShowName.Size = new System.Drawing.Size(16, 13);
            this.txtShowName.TabIndex = 3;
            this.txtShowName.Text = "---";
            this.txtShowName.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtSeasonNumber
            // 
            this.txtSeasonNumber.AutoSize = true;
            this.txtSeasonNumber.Location = new System.Drawing.Point(67, 29);
            this.txtSeasonNumber.Name = "txtSeasonNumber";
            this.txtSeasonNumber.Size = new System.Drawing.Size(16, 13);
            this.txtSeasonNumber.TabIndex = 5;
            this.txtSeasonNumber.Text = "---";
            this.txtSeasonNumber.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbEpsOriginal
            // 
            this.lbEpsOriginal.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbEpsOriginal.FormattingEnabled = true;
            this.lbEpsOriginal.HorizontalScrollbar = true;
            this.lbEpsOriginal.IntegralHeight = false;
            this.lbEpsOriginal.Location = new System.Drawing.Point(3, 16);
            this.lbEpsOriginal.Name = "lbEpsOriginal";
            this.lbEpsOriginal.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lbEpsOriginal.Size = new System.Drawing.Size(341, 206);
            this.lbEpsOriginal.TabIndex = 2;
            // 
            // txtPreviewLabel
            // 
            this.txtPreviewLabel.AutoSize = true;
            this.txtPreviewLabel.Location = new System.Drawing.Point(350, 0);
            this.txtPreviewLabel.Name = "txtPreviewLabel";
            this.txtPreviewLabel.Size = new System.Drawing.Size(120, 13);
            this.txtPreviewLabel.TabIndex = 1;
            this.txtPreviewLabel.Text = "Processed List Preview:";
            this.txtPreviewLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbEpsPreview, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lbEpsOriginal, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtPreviewLabel, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 247);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(695, 225);
            this.tableLayoutPanel1.TabIndex = 17;
            // 
            // bnPreview
            // 
            this.bnPreview.Location = new System.Drawing.Point(12, 218);
            this.bnPreview.Name = "bnPreview";
            this.bnPreview.Size = new System.Drawing.Size(75, 23);
            this.bnPreview.TabIndex = 13;
            this.bnPreview.Tag = "";
            this.bnPreview.Text = "&Preview to...";
            this.bnPreview.UseVisualStyleBackColor = true;
            this.bnPreview.Click += new System.EventHandler(this.bnPreview_Click);
            // 
            // EditRules
            // 
            this.AcceptButton = this.bnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(719, 511);
            this.Controls.Add(this.bnPreview);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.bnRuleUp);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnRuleDown);
            this.Controls.Add(this.lvRuleList);
            this.Controls.Add(this.bnDelRule);
            this.Controls.Add(this.bnEdit);
            this.Controls.Add(this.bnAddRule);
            this.Controls.Add(this.txtSeasonNumber);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtShowName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EditRules";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Season Rules";
            this.SizeChanged += new System.EventHandler(this.EditRules_SizeChanged);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bnRuleUp;
        private System.Windows.Forms.Button bnRuleDown;
        private System.Windows.Forms.ListView lvRuleList;
        private System.Windows.Forms.ColumnHeader colAction;
        private System.Windows.Forms.ColumnHeader colNum1;
        private System.Windows.Forms.ColumnHeader colNum2;
        private System.Windows.Forms.ColumnHeader colDescription;
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
        private System.Windows.Forms.ListBox lbEpsOriginal;
        private System.Windows.Forms.Label txtPreviewLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button bnPreview;
        private System.Windows.Forms.ColumnHeader colRule;
    }
}