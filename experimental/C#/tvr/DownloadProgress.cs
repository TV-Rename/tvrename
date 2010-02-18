//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//

using System;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Drawing;

namespace TVRename
{

//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//	ref class TVDoc;

	/// <summary>
	/// Summary for DownloadProgress
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public class DownloadProgress : System.Windows.Forms.Form
	{
		private TVDoc mDoc;


        public DownloadProgress(TVDoc doc)
        {
            InitializeComponent();

            mDoc = doc;
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
	private System.Windows.Forms.Button bnCancel;

	private System.Windows.Forms.ProgressBar pbProgressBar;

	private System.Windows.Forms.Label label2;

	private System.Windows.Forms.Label txtCurrent;
	private System.Windows.Forms.Timer tmrUpdate;
	private System.ComponentModel.IContainer components;

		/// <summary>
		/// Required designer variable.
		/// </summary>


#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
					this.components = (new System.ComponentModel.Container());
					this.bnCancel = (new System.Windows.Forms.Button());
					this.pbProgressBar = (new System.Windows.Forms.ProgressBar());
					this.label2 = (new System.Windows.Forms.Label());
					this.txtCurrent = (new System.Windows.Forms.Label());
					this.tmrUpdate = (new System.Windows.Forms.Timer(this.components));
					this.SuspendLayout();
					// 
					// bnCancel
					// 
					this.bnCancel.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
					this.bnCancel.Location = new System.Drawing.Point(159, 62);
					this.bnCancel.Name = "bnCancel";
					this.bnCancel.Size = new System.Drawing.Size(75, 23);
					this.bnCancel.TabIndex = 0;
					this.bnCancel.Text = "Cancel";
					this.bnCancel.UseVisualStyleBackColor = true;
					this.bnCancel.Click += new System.EventHandler(bnCancel_Click);
					// 
					// pbProgressBar
					// 
					this.pbProgressBar.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
					this.pbProgressBar.Location = new System.Drawing.Point(12, 35);
					this.pbProgressBar.Name = "pbProgressBar";
					this.pbProgressBar.Size = new System.Drawing.Size(366, 15);
					this.pbProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
					this.pbProgressBar.TabIndex = 1;
					// 
					// label2
					// 
					this.label2.AutoSize = true;
					this.label2.Location = new System.Drawing.Point(12, 9);
					this.label2.Name = "label2";
					this.label2.Size = new System.Drawing.Size(116, 13);
					this.label2.TabIndex = 2;
					this.label2.Text = "Currently Downloading:";
					// 
					// txtCurrent
					// 
					this.txtCurrent.AutoSize = true;
					this.txtCurrent.Location = new System.Drawing.Point(134, 9);
					this.txtCurrent.Name = "txtCurrent";
					this.txtCurrent.Size = new System.Drawing.Size(16, 13);
					this.txtCurrent.TabIndex = 2;
					this.txtCurrent.Text = "---";
					// 
					// tmrUpdate
					// 
					this.tmrUpdate.Enabled = true;
					this.tmrUpdate.Interval = 100;
					this.tmrUpdate.Tick += new System.EventHandler(tmrUpdate_Tick);
					// 
					// DownloadProgress
					// 
					this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
					this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
					this.ClientSize = new System.Drawing.Size(390, 97);
					this.Controls.Add(this.txtCurrent);
					this.Controls.Add(this.label2);
					this.Controls.Add(this.pbProgressBar);
					this.Controls.Add(this.bnCancel);
					this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
					this.Name = "DownloadProgress";
					this.ShowInTaskbar = false;
					this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
					this.Text = "Download Progress";
					this.Load += new System.EventHandler(DownloadProgress_Load);
					this.ResumeLayout(false);
					this.PerformLayout();

				}
#endregion
        private void bnCancel_Click(object sender, System.EventArgs e)
        {
            tmrUpdate.Stop();
            mDoc.StopBGDownloadThread();
            this.DialogResult = DialogResult.Abort;
        }
        private void tmrUpdate_Tick(object sender, System.EventArgs e)
        {
            if (mDoc.DownloadDone)
                Close();
            else
                UpdateStuff();
        }

private void DownloadProgress_Load(object sender, System.EventArgs e)
		 {
			 //UpdateStuff();
		 }
private void UpdateStuff()
{
    txtCurrent.Text = mDoc.GetTVDB(false, "").CurrentDLTask;
    pbProgressBar.Value = mDoc.DownloadPct;
}

}

}