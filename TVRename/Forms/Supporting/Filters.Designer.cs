using System.Linq;

namespace TVRename.Forms
{
    partial class Filters
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Filters));
            this.tbShowName = new System.Windows.Forms.TextBox();
            this.cmbShowStatus = new System.Windows.Forms.ComboBox();
            this.clbGenre = new System.Windows.Forms.CheckedListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.bnReset = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbRating = new System.Windows.Forms.ComboBox();
            this.cmbNetwork = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkHideIgnoredSeasons = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbShowStatusType = new System.Windows.Forms.ComboBox();
            this.cmbNetworkType = new System.Windows.Forms.ComboBox();
            this.cmbRatingType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // tbShowName
            // 
            this.tbShowName.Location = new System.Drawing.Point(213, 13);
            this.tbShowName.Name = "tbShowName";
            this.tbShowName.Size = new System.Drawing.Size(172, 20);
            this.tbShowName.TabIndex = 3;
            // 
            // cmbShowStatus
            // 
            this.cmbShowStatus.FormattingEnabled = true;
            this.cmbShowStatus.Location = new System.Drawing.Point(213, 39);
            this.cmbShowStatus.Name = "cmbShowStatus";
            this.cmbShowStatus.Size = new System.Drawing.Size(172, 21);
            this.cmbShowStatus.TabIndex = 4;
            // 
            // clbGenre
            // 
            this.clbGenre.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clbGenre.CheckOnClick = true;
            this.clbGenre.FormattingEnabled = true;
            this.clbGenre.Location = new System.Drawing.Point(13, 132);
            this.clbGenre.MultiColumn = true;
            this.clbGenre.Name = "clbGenre";
            this.clbGenre.Size = new System.Drawing.Size(374, 214);
            this.clbGenre.TabIndex = 5;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(312, 398);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(231, 398);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // bnReset
            // 
            this.bnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bnReset.Location = new System.Drawing.Point(12, 398);
            this.bnReset.Name = "bnReset";
            this.bnReset.Size = new System.Drawing.Size(75, 23);
            this.bnReset.TabIndex = 8;
            this.bnReset.Text = "Reset";
            this.bnReset.UseVisualStyleBackColor = true;
            this.bnReset.Click += new System.EventHandler(this.bnReset_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Status";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Rating";
            // 
            // cmbRating
            // 
            this.cmbRating.FormattingEnabled = true;
            this.cmbRating.Location = new System.Drawing.Point(213, 68);
            this.cmbRating.Name = "cmbRating";
            this.cmbRating.Size = new System.Drawing.Size(172, 21);
            this.cmbRating.TabIndex = 12;
            // 
            // cmbNetwork
            // 
            this.cmbNetwork.FormattingEnabled = true;
            this.cmbNetwork.Location = new System.Drawing.Point(213, 95);
            this.cmbNetwork.Name = "cmbNetwork";
            this.cmbNetwork.Size = new System.Drawing.Size(172, 21);
            this.cmbNetwork.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Network";
            // 
            // chkHideIgnoredSeasons
            // 
            this.chkHideIgnoredSeasons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkHideIgnoredSeasons.AutoSize = true;
            this.chkHideIgnoredSeasons.Location = new System.Drawing.Point(15, 366);
            this.chkHideIgnoredSeasons.Name = "chkHideIgnoredSeasons";
            this.chkHideIgnoredSeasons.Size = new System.Drawing.Size(151, 17);
            this.chkHideIgnoredSeasons.TabIndex = 15;
            this.chkHideIgnoredSeasons.Text = "Filter Out Ignored Seasons";
            this.chkHideIgnoredSeasons.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(118, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "contains";
            // 
            // cmbShowStatusType
            // 
            this.cmbShowStatusType.FormattingEnabled = true;
            this.cmbShowStatusType.Items.AddRange(new object[] {
            "is",
            "is not"});
            this.cmbShowStatusType.Location = new System.Drawing.Point(121, 39);
            this.cmbShowStatusType.Name = "cmbShowStatusType";
            this.cmbShowStatusType.Size = new System.Drawing.Size(74, 21);
            this.cmbShowStatusType.TabIndex = 17;
            // 
            // cmbNetworkType
            // 
            this.cmbNetworkType.FormattingEnabled = true;
            this.cmbNetworkType.Items.AddRange(new object[] {
            "is",
            "is not"});
            this.cmbNetworkType.Location = new System.Drawing.Point(121, 95);
            this.cmbNetworkType.Name = "cmbNetworkType";
            this.cmbNetworkType.Size = new System.Drawing.Size(74, 21);
            this.cmbNetworkType.TabIndex = 18;
            // 
            // cmbRatingType
            // 
            this.cmbRatingType.FormattingEnabled = true;
            this.cmbRatingType.Items.AddRange(new object[] {
            "is",
            "is not"});
            this.cmbRatingType.Location = new System.Drawing.Point(121, 68);
            this.cmbRatingType.Name = "cmbRatingType";
            this.cmbRatingType.Size = new System.Drawing.Size(74, 21);
            this.cmbRatingType.TabIndex = 19;
            // 
            // Filters
            // 
            this.AcceptButton = this.btnOk;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(404, 433);
            this.Controls.Add(this.cmbRatingType);
            this.Controls.Add(this.cmbNetworkType);
            this.Controls.Add(this.cmbShowStatusType);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.chkHideIgnoredSeasons);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbNetwork);
            this.Controls.Add(this.cmbRating);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bnReset);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.clbGenre);
            this.Controls.Add(this.cmbShowStatus);
            this.Controls.Add(this.tbShowName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Filters";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Filters...";
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.CheckBox chkHideIgnoredSeasons;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbShowStatusType;
        private System.Windows.Forms.ComboBox cmbNetworkType;
        private System.Windows.Forms.ComboBox cmbRatingType;
    }
}
