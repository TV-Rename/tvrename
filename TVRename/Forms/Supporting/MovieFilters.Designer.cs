using System.Linq;

namespace TVRename.Forms
{
    partial class MovieFilters
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MovieFilters));
            tbShowName = new System.Windows.Forms.TextBox();
            cmbShowStatus = new System.Windows.Forms.ComboBox();
            clbGenre = new System.Windows.Forms.CheckedListBox();
            btnCancel = new System.Windows.Forms.Button();
            btnOk = new System.Windows.Forms.Button();
            bnReset = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            cmbRating = new System.Windows.Forms.ComboBox();
            cmbNetwork = new System.Windows.Forms.ComboBox();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            cmbShowStatusType = new System.Windows.Forms.ComboBox();
            cmbNetworkType = new System.Windows.Forms.ComboBox();
            cmbRatingType = new System.Windows.Forms.ComboBox();
            cmbYearType = new System.Windows.Forms.ComboBox();
            label6 = new System.Windows.Forms.Label();
            cmbYear = new System.Windows.Forms.ComboBox();
            chkIncludeBlanks = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            // 
            // tbShowName
            // 
            tbShowName.Location = new System.Drawing.Point(213, 13);
            tbShowName.Name = "tbShowName";
            tbShowName.Size = new System.Drawing.Size(172, 23);
            tbShowName.TabIndex = 3;
            // 
            // cmbShowStatus
            // 
            cmbShowStatus.FormattingEnabled = true;
            cmbShowStatus.Location = new System.Drawing.Point(213, 39);
            cmbShowStatus.Name = "cmbShowStatus";
            cmbShowStatus.Size = new System.Drawing.Size(172, 23);
            cmbShowStatus.TabIndex = 4;
            // 
            // clbGenre
            // 
            clbGenre.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            clbGenre.CheckOnClick = true;
            clbGenre.FormattingEnabled = true;
            clbGenre.Location = new System.Drawing.Point(13, 149);
            clbGenre.MultiColumn = true;
            clbGenre.Name = "clbGenre";
            clbGenre.Size = new System.Drawing.Size(374, 202);
            clbGenre.TabIndex = 5;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(312, 398);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.TabIndex = 6;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnOk
            // 
            btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnOk.Location = new System.Drawing.Point(231, 398);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(75, 23);
            btnOk.TabIndex = 7;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // bnReset
            // 
            bnReset.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            bnReset.Location = new System.Drawing.Point(12, 398);
            bnReset.Name = "bnReset";
            bnReset.Size = new System.Drawing.Size(75, 23);
            bnReset.TabIndex = 8;
            bnReset.Text = "Reset";
            bnReset.UseVisualStyleBackColor = true;
            bnReset.Click += bnReset_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 16);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(39, 15);
            label1.TabIndex = 9;
            label1.Text = "Name";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 42);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(39, 15);
            label2.TabIndex = 10;
            label2.Text = "Status";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 71);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(41, 15);
            label3.TabIndex = 11;
            label3.Text = "Rating";
            // 
            // cmbRating
            // 
            cmbRating.FormattingEnabled = true;
            cmbRating.Location = new System.Drawing.Point(213, 68);
            cmbRating.Name = "cmbRating";
            cmbRating.Size = new System.Drawing.Size(172, 23);
            cmbRating.TabIndex = 12;
            // 
            // cmbNetwork
            // 
            cmbNetwork.FormattingEnabled = true;
            cmbNetwork.Location = new System.Drawing.Point(213, 95);
            cmbNetwork.Name = "cmbNetwork";
            cmbNetwork.Size = new System.Drawing.Size(172, 23);
            cmbNetwork.TabIndex = 13;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(12, 98);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(52, 15);
            label4.TabIndex = 14;
            label4.Text = "Network";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(118, 16);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(52, 15);
            label5.TabIndex = 16;
            label5.Text = "contains";
            // 
            // cmbShowStatusType
            // 
            cmbShowStatusType.FormattingEnabled = true;
            cmbShowStatusType.Items.AddRange(new object[] { "is", "is not" });
            cmbShowStatusType.Location = new System.Drawing.Point(121, 39);
            cmbShowStatusType.Name = "cmbShowStatusType";
            cmbShowStatusType.Size = new System.Drawing.Size(74, 23);
            cmbShowStatusType.TabIndex = 17;
            // 
            // cmbNetworkType
            // 
            cmbNetworkType.FormattingEnabled = true;
            cmbNetworkType.Items.AddRange(new object[] { "is", "is not" });
            cmbNetworkType.Location = new System.Drawing.Point(121, 95);
            cmbNetworkType.Name = "cmbNetworkType";
            cmbNetworkType.Size = new System.Drawing.Size(74, 23);
            cmbNetworkType.TabIndex = 18;
            // 
            // cmbRatingType
            // 
            cmbRatingType.FormattingEnabled = true;
            cmbRatingType.Items.AddRange(new object[] { "is", "is not" });
            cmbRatingType.Location = new System.Drawing.Point(121, 68);
            cmbRatingType.Name = "cmbRatingType";
            cmbRatingType.Size = new System.Drawing.Size(74, 23);
            cmbRatingType.TabIndex = 19;
            // 
            // cmbYearType
            // 
            cmbYearType.FormattingEnabled = true;
            cmbYearType.Items.AddRange(new object[] { "is", "is not" });
            cmbYearType.Location = new System.Drawing.Point(121, 122);
            cmbYearType.Name = "cmbYearType";
            cmbYearType.Size = new System.Drawing.Size(74, 23);
            cmbYearType.TabIndex = 22;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(12, 125);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(29, 15);
            label6.TabIndex = 21;
            label6.Text = "Year";
            // 
            // cmbYear
            // 
            cmbYear.FormattingEnabled = true;
            cmbYear.Location = new System.Drawing.Point(213, 122);
            cmbYear.Name = "cmbYear";
            cmbYear.Size = new System.Drawing.Size(172, 23);
            cmbYear.TabIndex = 20;
            // 
            // chkIncludeBlanks
            // 
            chkIncludeBlanks.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            chkIncludeBlanks.AutoSize = true;
            chkIncludeBlanks.Location = new System.Drawing.Point(12, 375);
            chkIncludeBlanks.Name = "chkIncludeBlanks";
            chkIncludeBlanks.Size = new System.Drawing.Size(102, 19);
            chkIncludeBlanks.TabIndex = 23;
            chkIncludeBlanks.Text = "Include Blanks";
            chkIncludeBlanks.UseVisualStyleBackColor = true;
            // 
            // MovieFilters
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(404, 433);
            Controls.Add(chkIncludeBlanks);
            Controls.Add(cmbYearType);
            Controls.Add(label6);
            Controls.Add(cmbYear);
            Controls.Add(cmbRatingType);
            Controls.Add(cmbNetworkType);
            Controls.Add(cmbShowStatusType);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(cmbNetwork);
            Controls.Add(cmbRating);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(bnReset);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            Controls.Add(clbGenre);
            Controls.Add(cmbShowStatus);
            Controls.Add(tbShowName);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MovieFilters";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Select Filters...";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox tbShowName;
        private System.Windows.Forms.ComboBox cmbShowStatus;
        private System.Windows.Forms.CheckedListBox clbGenre;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button bnReset;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbRating;
        private System.Windows.Forms.ComboBox cmbNetwork;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbShowStatusType;
        private System.Windows.Forms.ComboBox cmbNetworkType;
        private System.Windows.Forms.ComboBox cmbRatingType;
        private System.Windows.Forms.ComboBox cmbYearType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbYear;
        private System.Windows.Forms.CheckBox chkIncludeBlanks;
    }
}
