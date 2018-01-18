namespace TVRename.Forms
{
    partial class UpdateNotification
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDownloadNow = new System.Windows.Forms.Button();
            this.bnReleaseNotes = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tbReleaseNotes = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.webReleaseNotes = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(23, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(240, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "A new version of TV Rename is available";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(21, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Release Notes:";
            // 
            // btnDownloadNow
            // 
            this.btnDownloadNow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDownloadNow.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnDownloadNow.Location = new System.Drawing.Point(288, 460);
            this.btnDownloadNow.Name = "btnDownloadNow";
            this.btnDownloadNow.Size = new System.Drawing.Size(120, 23);
            this.btnDownloadNow.TabIndex = 2;
            this.btnDownloadNow.Text = "Download Now";
            this.btnDownloadNow.UseVisualStyleBackColor = true;
            this.btnDownloadNow.Click += new System.EventHandler(this.btnDownloadNow_Click);
            // 
            // bnReleaseNotes
            // 
            this.bnReleaseNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnReleaseNotes.Location = new System.Drawing.Point(288, 431);
            this.bnReleaseNotes.Name = "bnReleaseNotes";
            this.bnReleaseNotes.Size = new System.Drawing.Size(120, 23);
            this.bnReleaseNotes.TabIndex = 3;
            this.bnReleaseNotes.Text = "View Release Notes";
            this.bnReleaseNotes.UseVisualStyleBackColor = true;
            this.bnReleaseNotes.Click += new System.EventHandler(this.bnReleaseNotes_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(26, 460);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // tbReleaseNotes
            // 
            this.tbReleaseNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbReleaseNotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbReleaseNotes.Location = new System.Drawing.Point(26, 92);
            this.tbReleaseNotes.Multiline = true;
            this.tbReleaseNotes.Name = "tbReleaseNotes";
            this.tbReleaseNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbReleaseNotes.Size = new System.Drawing.Size(382, 333);
            this.tbReleaseNotes.TabIndex = 5;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(23, 36);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 6;
            // 
            // webReleaseNotes
            // 
            this.webReleaseNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webReleaseNotes.Location = new System.Drawing.Point(26, 92);
            this.webReleaseNotes.MinimumSize = new System.Drawing.Size(20, 20);
            this.webReleaseNotes.Name = "webReleaseNotes";
            this.webReleaseNotes.Size = new System.Drawing.Size(382, 333);
            this.webReleaseNotes.TabIndex = 7;
            this.webReleaseNotes.Visible = false;
            // 
            // UpdateNotification
            // 
            this.AcceptButton = this.btnDownloadNow;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(420, 496);
            this.Controls.Add(this.webReleaseNotes);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.tbReleaseNotes);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.bnReleaseNotes);
            this.Controls.Add(this.btnDownloadNow);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(350, 350);
            this.Name = "UpdateNotification";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Available Update";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDownloadNow;
        private System.Windows.Forms.Button bnReleaseNotes;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox tbReleaseNotes;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.WebBrowser webReleaseNotes;
    }
}
