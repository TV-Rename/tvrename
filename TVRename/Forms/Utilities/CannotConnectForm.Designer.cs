namespace TVRename.Forms.Utilities
{
    partial class CannotConnectForm
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
            this.bnContinue = new System.Windows.Forms.Button();
            this.bnOffline = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.bnTVDB = new System.Windows.Forms.Button();
            this.bnAPICheck = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bnContinue
            // 
            this.bnContinue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnContinue.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnContinue.Location = new System.Drawing.Point(334, 107);
            this.bnContinue.Name = "bnContinue";
            this.bnContinue.Size = new System.Drawing.Size(75, 23);
            this.bnContinue.TabIndex = 0;
            this.bnContinue.Text = "Continue";
            this.bnContinue.UseVisualStyleBackColor = true;
            this.bnContinue.Click += new System.EventHandler(this.bnContinue_Click);
            // 
            // bnOffline
            // 
            this.bnOffline.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnOffline.Location = new System.Drawing.Point(253, 107);
            this.bnOffline.Name = "bnOffline";
            this.bnOffline.Size = new System.Drawing.Size(75, 23);
            this.bnOffline.TabIndex = 1;
            this.bnOffline.Text = "Go Offline";
            this.bnOffline.UseVisualStyleBackColor = true;
            this.bnOffline.Click += new System.EventHandler(this.bnOffline_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            // 
            // bnTVDB
            // 
            this.bnTVDB.Location = new System.Drawing.Point(15, 107);
            this.bnTVDB.Name = "bnTVDB";
            this.bnTVDB.Size = new System.Drawing.Size(75, 23);
            this.bnTVDB.TabIndex = 3;
            this.bnTVDB.Text = "TVDB.com";
            this.bnTVDB.UseVisualStyleBackColor = true;
            this.bnTVDB.Click += new System.EventHandler(this.bnTVDB_Click);
            // 
            // bnAPICheck
            // 
            this.bnAPICheck.Location = new System.Drawing.Point(96, 107);
            this.bnAPICheck.Name = "bnAPICheck";
            this.bnAPICheck.Size = new System.Drawing.Size(75, 23);
            this.bnAPICheck.TabIndex = 4;
            this.bnAPICheck.Text = "API Check";
            this.bnAPICheck.UseVisualStyleBackColor = true;
            this.bnAPICheck.Click += new System.EventHandler(this.bnAPICheck_Click);
            // 
            // CannotConnectForm
            // 
            this.AcceptButton = this.bnContinue;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnContinue;
            this.ClientSize = new System.Drawing.Size(421, 142);
            this.Controls.Add(this.bnAPICheck);
            this.Controls.Add(this.bnTVDB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bnOffline);
            this.Controls.Add(this.bnContinue);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CannotConnectForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cannot Connect";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bnContinue;
        private System.Windows.Forms.Button bnOffline;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bnTVDB;
        private System.Windows.Forms.Button bnAPICheck;
    }
}