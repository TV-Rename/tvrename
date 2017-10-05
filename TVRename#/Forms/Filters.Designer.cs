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
        private void InitializeComponent(TVDoc doc)
        {
            this.cbShowName = new System.Windows.Forms.CheckBox();
            this.cbShowStatus = new System.Windows.Forms.CheckBox();
            this.cbGenre = new System.Windows.Forms.CheckBox();
            this.tbShowName = new System.Windows.Forms.TextBox();
            this.cmbShowStatus = new System.Windows.Forms.ComboBox();
            this.clbGenre = new System.Windows.Forms.CheckedListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbShowName
            // 
            this.cbShowName.AutoSize = true;
            this.cbShowName.Location = new System.Drawing.Point(13, 15);
            this.cbShowName.Name = "cbShowName";
            this.cbShowName.Size = new System.Drawing.Size(84, 17);
            this.cbShowName.TabIndex = 0;
            this.cbShowName.Text = "Show Name";
            this.cbShowName.UseVisualStyleBackColor = true;
            this.cbShowName.CheckedChanged += new System.EventHandler(this.cbShowName_CheckedChanged);
            // 
            // cbShowStatus
            // 
            this.cbShowStatus.AutoSize = true;
            this.cbShowStatus.Location = new System.Drawing.Point(13, 41);
            this.cbShowStatus.Name = "cbShowStatus";
            this.cbShowStatus.Size = new System.Drawing.Size(86, 17);
            this.cbShowStatus.TabIndex = 1;
            this.cbShowStatus.Text = "Show Status";
            this.cbShowStatus.UseVisualStyleBackColor = true;
            this.cbShowStatus.CheckedChanged += new System.EventHandler(this.cbShowStatus_CheckedChanged);
            // 
            // cbGenre
            // 
            this.cbGenre.AutoSize = true;
            this.cbGenre.Location = new System.Drawing.Point(13, 64);
            this.cbGenre.Name = "cbGenre";
            this.cbGenre.Size = new System.Drawing.Size(55, 17);
            this.cbGenre.TabIndex = 2;
            this.cbGenre.Text = "Genre";
            this.cbGenre.UseVisualStyleBackColor = true;
            this.cbGenre.CheckedChanged += new System.EventHandler(this.cbGenre_CheckedChanged);
            // 
            // tbShowName
            // 
            this.tbShowName.Enabled = false;
            this.tbShowName.Location = new System.Drawing.Point(117, 13);
            this.tbShowName.Name = "tbShowName";
            this.tbShowName.Size = new System.Drawing.Size(172, 20);
            this.tbShowName.TabIndex = 3;
            // 
            // cmbShowStatus
            // 
            this.cmbShowStatus.Enabled = false;
            this.cmbShowStatus.FormattingEnabled = true;
            this.cmbShowStatus.Items.AddRange(new object[] {
            "Ended",
            "Continuing"});
            this.cmbShowStatus.Location = new System.Drawing.Point(117, 39);
            this.cmbShowStatus.Name = "cmbShowStatus";
            this.cmbShowStatus.Size = new System.Drawing.Size(172, 21);
            this.cmbShowStatus.TabIndex = 4;
            // 
            // clbGenre
            // 
            this.clbGenre.Enabled = false;
            this.clbGenre.FormattingEnabled = true;
            this.clbGenre.Items.AddRange(doc.getGenres().Cast<object>().ToArray());
            this.clbGenre.Location = new System.Drawing.Point(13, 87);
            this.clbGenre.MultiColumn = true;
            this.clbGenre.Name = "clbGenre";
            this.clbGenre.Size = new System.Drawing.Size(276, 214);
            this.clbGenre.TabIndex = 5;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(214, 308);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(133, 308);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // Filters
            // 
            this.ClientSize = new System.Drawing.Size(306, 343);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.clbGenre);
            this.Controls.Add(this.cmbShowStatus);
            this.Controls.Add(this.tbShowName);
            this.Controls.Add(this.cbGenre);
            this.Controls.Add(this.cbShowStatus);
            this.Controls.Add(this.cbShowName);
            this.Name = "Filters";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbShowName;
        private System.Windows.Forms.CheckBox cbShowStatus;
        private System.Windows.Forms.CheckBox cbGenre;
        private System.Windows.Forms.TextBox tbShowName;
        private System.Windows.Forms.ComboBox cmbShowStatus;
        private System.Windows.Forms.CheckedListBox clbGenre;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
    }
}