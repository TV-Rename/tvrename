namespace TVRename
{
    partial class LogViewer
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
            this.logData = new System.Windows.Forms.RichTextBox();
            this.btnFullLog = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // logData
            // 
            this.logData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logData.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logData.Location = new System.Drawing.Point(0, 3);
            this.logData.Name = "logData";
            this.logData.Size = new System.Drawing.Size(800, 409);
            this.logData.TabIndex = 0;
            this.logData.Text = string.Empty;
            // 
            // btnFullLog
            // 
            this.btnFullLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFullLog.Location = new System.Drawing.Point(695, 418);
            this.btnFullLog.Name = "btnFullLog";
            this.btnFullLog.Size = new System.Drawing.Size(93, 23);
            this.btnFullLog.TabIndex = 1;
            this.btnFullLog.Text = "View Full Log";
            this.btnFullLog.UseVisualStyleBackColor = true;
            this.btnFullLog.Click += new System.EventHandler(this.btnFullLog_Click);
            // 
            // LogViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnFullLog);
            this.Controls.Add(this.logData);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimizeBox = false;
            this.Name = "LogViewer";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "TV Rename Log Viewer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox logData;
        private System.Windows.Forms.Button btnFullLog;
    }
}
