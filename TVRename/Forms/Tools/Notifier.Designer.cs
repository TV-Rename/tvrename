using System.ComponentModel;

namespace TVRename.Forms.Tools
{
    partial class Notifier
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.btnCancel = new System.Windows.Forms.Button();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.lblMessage = new System.Windows.Forms.Label();
            this.bwDo = new System.ComponentModel.BackgroundWorker();
            this.lblLastUpdate = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(460, 85);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(74, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pbProgress
            // 
            this.pbProgress.Location = new System.Drawing.Point(10, 10);
            this.pbProgress.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(522, 25);
            this.pbProgress.TabIndex = 2;
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(10, 40);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(520, 24);
            this.lblMessage.TabIndex = 3;
            this.lblMessage.Text = "label1";
            // 
            // bwDo
            // 
            this.bwDo.WorkerReportsProgress = true;
            this.bwDo.WorkerSupportsCancellation = true;
            this.bwDo.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwDo_DoWork);
            this.bwDo.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwDo_ProgressChanged);
            this.bwDo.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwDo_RunWorkerCompleted);
            // 
            // lblLastUpdate
            // 
            this.lblLastUpdate.Location = new System.Drawing.Point(10, 58);
            this.lblLastUpdate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLastUpdate.Name = "lblLastUpdate";
            this.lblLastUpdate.Size = new System.Drawing.Size(520, 24);
            this.lblLastUpdate.TabIndex = 4;
            this.lblLastUpdate.Text = "label1";
            // 
            // Notifier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(545, 120);
            this.ControlBox = false;
            this.Controls.Add(this.lblLastUpdate);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Notifier";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "View Progress";
            this.Shown += new System.EventHandler(this.DoScanPartNotifier_Shown);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Label lblMessage;

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ProgressBar pbProgress;

        #endregion

        private BackgroundWorker bwDo;
        private System.Windows.Forms.Label lblLastUpdate;
    }
}

