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
//	ref class FolderMonitor;

	public class FolderMonitorProgress : System.Windows.Forms.Form
	{
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
	private System.Windows.Forms.Button bnCancel;

	private System.Windows.Forms.Label label2;
	private System.Windows.Forms.Timer timer1;


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
			this.label2 = (new System.Windows.Forms.Label());
			this.timer1 = (new System.Windows.Forms.Timer(this.components));
			this.SuspendLayout();
			// 
			// bnCancel
			// 
			this.bnCancel.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.bnCancel.Location = new System.Drawing.Point(60, 33);
			this.bnCancel.Name = "bnCancel";
			this.bnCancel.Size = new System.Drawing.Size(75, 23);
			this.bnCancel.TabIndex = 0;
			this.bnCancel.Text = "Cancel";
			this.bnCancel.UseVisualStyleBackColor = true;
			this.bnCancel.Click += new System.EventHandler(bnCancel_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(39, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(117, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Automatic show lookup";
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Tick += new System.EventHandler(timer1_Tick);
			// 
			// FolderMonitorProgress
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(201, 68);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.bnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "FolderMonitorProgress";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Folder Monitor";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

#endregion
		private FolderMonitor mFM;
			public bool StopNow;

            public FolderMonitorProgress(FolderMonitor thefm)
            {
                mFM = thefm;

                InitializeComponent();
            }
            public void bnCancel_Click(object sender, System.EventArgs e)
            {
                this.DialogResult = DialogResult.Abort;
                mFM.FMPStopNow = true;
            }


            private void timer1_Tick(object sender, System.EventArgs e)
            {
                if (mFM.FMPStopNow)
                    this.Close();
            }

	}

}