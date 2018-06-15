//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//


namespace TVRename
{
    partial class TheTvdbCodeFinder
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
            this.txtSearchStatus = new System.Windows.Forms.Label();
            this.bnGoSearch = new System.Windows.Forms.Button();
            this.txtFindThis = new System.Windows.Forms.TextBox();
            this.lvMatches = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtSearchStatus
            // 
            this.txtSearchStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearchStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.txtSearchStatus.Location = new System.Drawing.Point(2, 153);
            this.txtSearchStatus.Name = "txtSearchStatus";
            this.txtSearchStatus.Size = new System.Drawing.Size(397, 15);
            this.txtSearchStatus.TabIndex = 4;
            this.txtSearchStatus.Text = "                    ";
            // 
            // bnGoSearch
            // 
            this.bnGoSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnGoSearch.Location = new System.Drawing.Point(325, 1);
            this.bnGoSearch.Name = "bnGoSearch";
            this.bnGoSearch.Size = new System.Drawing.Size(75, 23);
            this.bnGoSearch.TabIndex = 2;
            this.bnGoSearch.Text = "&Search";
            this.bnGoSearch.UseVisualStyleBackColor = true;
            this.bnGoSearch.Click += new System.EventHandler(this.bnGoSearch_Click);
            // 
            // txtFindThis
            // 
            this.txtFindThis.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFindThis.Location = new System.Drawing.Point(90, 3);
            this.txtFindThis.Name = "txtFindThis";
            this.txtFindThis.Size = new System.Drawing.Size(228, 20);
            this.txtFindThis.TabIndex = 1;
            this.txtFindThis.TextChanged += new System.EventHandler(this.txtFindThis_TextChanged);
            this.txtFindThis.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFindThis_KeyDown);
            // 
            // lvMatches
            // 
            this.lvMatches.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvMatches.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvMatches.FullRowSelect = true;
            this.lvMatches.HideSelection = false;
            this.lvMatches.Location = new System.Drawing.Point(1, 30);
            this.lvMatches.MultiSelect = false;
            this.lvMatches.Name = "lvMatches";
            this.lvMatches.ShowItemToolTips = true;
            this.lvMatches.Size = new System.Drawing.Size(397, 120);
            this.lvMatches.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvMatches.TabIndex = 3;
            this.lvMatches.UseCompatibleStateImageBehavior = false;
            this.lvMatches.View = System.Windows.Forms.View.Details;
            this.lvMatches.SelectedIndexChanged += new System.EventHandler(this.lvMatches_SelectedIndexChanged);
            this.lvMatches.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvMatches_ColumnClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Code";
            this.columnHeader1.Width = 44;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Show Name";
            this.columnHeader2.Width = 268;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Year";
            this.columnHeader3.Width = 49;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(-1, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "TheTVDB &code:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // TheTVDBCodeFinder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtSearchStatus);
            this.Controls.Add(this.bnGoSearch);
            this.Controls.Add(this.txtFindThis);
            this.Controls.Add(this.lvMatches);
            this.Controls.Add(this.label3);
            this.Name = "TheTvdbCodeFinder";
            this.Size = new System.Drawing.Size(403, 170);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColumnHeader columnHeader3;

        private System.Windows.Forms.Label txtSearchStatus;
        private System.Windows.Forms.Button bnGoSearch;
        private System.Windows.Forms.TextBox txtFindThis;
        public System.Windows.Forms.ListView lvMatches;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label label3;
    }
}
