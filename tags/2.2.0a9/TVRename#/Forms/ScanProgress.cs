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

	/// <summary>
	/// Summary for ScanProgress
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>



public class ScanProgress : System.Windows.Forms.Form
	{
		public bool Ready;
		public bool Finished;

		public int pctMediaLib;
		public int pctLocalSearch;
		public int pctRSS;
		public int pctuTorrent;

	private System.Windows.Forms.Timer timer1;

		public ScanProgress(bool e1, bool e2, bool e3, bool e4, bool e5, bool e6)
		{
			Ready = false;
			Finished = false;
			InitializeComponent();

			lb1.Enabled = e1;
			lb3.Enabled = e3;
			lb4.Enabled = e4;
			lb5.Enabled = e5;
		}

	  public void UpdateProg()
	  {
          pbMediaLib.Value = ((pctMediaLib < 0) ? 0 : ((pctMediaLib > 100) ? 100 : pctMediaLib));
		  pbMediaLib.Update();
		  pbLocalSearch.Value = ((pctLocalSearch<0) ? 0 : ((pctLocalSearch>100) ? 100 : pctLocalSearch));
		  pbLocalSearch.Update();
		  pbRSS.Value = ((pctRSS<0) ? 0 : ((pctRSS>100) ? 100 : pctRSS));
		  pbRSS.Update();
		  pbuTorrent.Value = ((pctuTorrent<0) ? 0 : ((pctuTorrent>100) ? 100 : pctuTorrent));
		  pbuTorrent.Update();
	  }

	  public void MediaLibProg(int p)
	  {
		  pctMediaLib = p;
	  }

	  public void LocalSearchProg(int p)
	  {
		  pctLocalSearch = p;
	  }

	  public void RSSProg(int p)
	  {
		  pctRSS = p;
	  }
	  public void uTorrentProg(int p)
	  {
		  pctuTorrent = p;
	  }

	private System.Windows.Forms.Button bnCancel;
	private System.Windows.Forms.Label lb1;
    private System.Windows.Forms.ProgressBar pbMediaLib;
	private System.Windows.Forms.Label lb3;
	private System.Windows.Forms.ProgressBar pbLocalSearch;
	private System.Windows.Forms.Label lb5;
	private System.Windows.Forms.ProgressBar pbRSS;
	private System.Windows.Forms.Label lb4;
    private System.Windows.Forms.ProgressBar pbuTorrent;
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScanProgress));
            this.bnCancel = new System.Windows.Forms.Button();
            this.lb1 = new System.Windows.Forms.Label();
            this.pbMediaLib = new System.Windows.Forms.ProgressBar();
            this.lb3 = new System.Windows.Forms.Label();
            this.pbLocalSearch = new System.Windows.Forms.ProgressBar();
            this.lb5 = new System.Windows.Forms.Label();
            this.pbRSS = new System.Windows.Forms.ProgressBar();
            this.lb4 = new System.Windows.Forms.Label();
            this.pbuTorrent = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(289, 90);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 0;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // lb1
            // 
            this.lb1.AutoSize = true;
            this.lb1.Location = new System.Drawing.Point(12, 9);
            this.lb1.Name = "lb1";
            this.lb1.Size = new System.Drawing.Size(104, 13);
            this.lb1.TabIndex = 1;
            this.lb1.Text = "Media Library Check";
            // 
            // pbMediaLib
            // 
            this.pbMediaLib.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbMediaLib.Location = new System.Drawing.Point(141, 9);
            this.pbMediaLib.Name = "pbMediaLib";
            this.pbMediaLib.Size = new System.Drawing.Size(223, 13);
            this.pbMediaLib.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbMediaLib.TabIndex = 2;
            // 
            // lb3
            // 
            this.lb3.AutoSize = true;
            this.lb3.Location = new System.Drawing.Point(12, 28);
            this.lb3.Name = "lb3";
            this.lb3.Size = new System.Drawing.Size(77, 13);
            this.lb3.TabIndex = 1;
            this.lb3.Text = "Search Locally";
            // 
            // pbLocalSearch
            // 
            this.pbLocalSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbLocalSearch.Location = new System.Drawing.Point(141, 28);
            this.pbLocalSearch.Name = "pbLocalSearch";
            this.pbLocalSearch.Size = new System.Drawing.Size(223, 13);
            this.pbLocalSearch.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbLocalSearch.TabIndex = 2;
            // 
            // lb5
            // 
            this.lb5.AutoSize = true;
            this.lb5.Location = new System.Drawing.Point(12, 66);
            this.lb5.Name = "lb5";
            this.lb5.Size = new System.Drawing.Size(66, 13);
            this.lb5.TabIndex = 1;
            this.lb5.Text = "Search RSS";
            // 
            // pbRSS
            // 
            this.pbRSS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbRSS.Location = new System.Drawing.Point(141, 66);
            this.pbRSS.Name = "pbRSS";
            this.pbRSS.Size = new System.Drawing.Size(223, 13);
            this.pbRSS.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbRSS.TabIndex = 2;
            // 
            // lb4
            // 
            this.lb4.AutoSize = true;
            this.lb4.Location = new System.Drawing.Point(12, 47);
            this.lb4.Name = "lb4";
            this.lb4.Size = new System.Drawing.Size(81, 13);
            this.lb4.TabIndex = 1;
            this.lb4.Text = "Check µTorrent";
            // 
            // pbuTorrent
            // 
            this.pbuTorrent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbuTorrent.Location = new System.Drawing.Point(141, 47);
            this.pbuTorrent.Name = "pbuTorrent";
            this.pbuTorrent.Size = new System.Drawing.Size(223, 13);
            this.pbuTorrent.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbuTorrent.TabIndex = 2;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ScanProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(376, 125);
            this.Controls.Add(this.pbuTorrent);
            this.Controls.Add(this.lb4);
            this.Controls.Add(this.pbRSS);
            this.Controls.Add(this.lb5);
            this.Controls.Add(this.pbLocalSearch);
            this.Controls.Add(this.lb3);
            this.Controls.Add(this.pbMediaLib);
            this.Controls.Add(this.lb1);
            this.Controls.Add(this.bnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScanProgress";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Scan Progress";
            this.Load += new System.EventHandler(this.ScanProgress_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

#endregion
private void ScanProgress_Load(object sender, System.EventArgs e)
		 {
			 Ready = true;
			 timer1.Start();
		 }
private void timer1_Tick(object sender, System.EventArgs e)
		 {
			 UpdateProg();
			 timer1.Start();
			 if (Finished)
				 this.Close();
		 }
public void Done()
		 {
			 Finished = true;
		 }
}
}