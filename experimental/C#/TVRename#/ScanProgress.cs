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

		public int pctRename;
		public int pctMissing;
		public int pctLocalSearch;
		public int pctRSS;
		public int pctuTorrent;
		public int pctFolderThumbs;

	private System.Windows.Forms.Timer timer1;

		public ScanProgress(bool e1, bool e2, bool e3, bool e4, bool e5, bool e6)
		{
			Ready = false;
			Finished = false;
			InitializeComponent();

			lb1.Enabled = e1;
			lb2.Enabled = e2;
			lb3.Enabled = e3;
			lb4.Enabled = e4;
			lb5.Enabled = e5;
			lb6.Enabled = e6;


		}

	  public void UpdateProg()
	  {
		  pbRename.Value = ((pctRename<0) ? 0 : ((pctRename>100) ? 100 : pctRename));
		  pbRename.Update();
		  pbMissing.Value = ((pctMissing<0) ? 0 : ((pctMissing>100) ? 100 : pctMissing));
		  pbMissing.Update();
		  pbLocalSearch.Value = ((pctLocalSearch<0) ? 0 : ((pctLocalSearch>100) ? 100 : pctLocalSearch));
		  pbLocalSearch.Update();
		  pbRSS.Value = ((pctRSS<0) ? 0 : ((pctRSS>100) ? 100 : pctRSS));
		  pbRSS.Update();
		  pbuTorrent.Value = ((pctuTorrent<0) ? 0 : ((pctuTorrent>100) ? 100 : pctuTorrent));
		  pbuTorrent.Update();
		  pbFolderThumbs.Value = ((pctFolderThumbs<0) ? 0 : ((pctFolderThumbs>100) ? 100 : pctFolderThumbs));
		  pbFolderThumbs.Update();
	  }

	  public void RenameProg(int p)
	  {
		  pctRename = p;
	  }

	  public void MissingProg(int p)
	  {
		  pctMissing = p;
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

	  public void FolderThumbsProg(int p)
	  {
		  pctFolderThumbs = p;
	  }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
	private System.Windows.Forms.Button bnCancel;
	private System.Windows.Forms.Label lb1;

	private System.Windows.Forms.ProgressBar pbRename;
	private System.Windows.Forms.Label lb2;


	private System.Windows.Forms.ProgressBar pbMissing;
	private System.Windows.Forms.Label lb3;


	private System.Windows.Forms.ProgressBar pbLocalSearch;
	private System.Windows.Forms.Label lb5;


	private System.Windows.Forms.ProgressBar pbRSS;
	private System.Windows.Forms.Label lb4;


	private System.Windows.Forms.ProgressBar pbuTorrent;
private System.Windows.Forms.Label lb6;





	private System.Windows.Forms.ProgressBar pbFolderThumbs;
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
			System.ComponentModel.ComponentResourceManager resources = (new System.ComponentModel.ComponentResourceManager(typeof(ScanProgress)));
			this.bnCancel = (new System.Windows.Forms.Button());
			this.lb1 = (new System.Windows.Forms.Label());
			this.pbRename = (new System.Windows.Forms.ProgressBar());
			this.lb2 = (new System.Windows.Forms.Label());
			this.pbMissing = (new System.Windows.Forms.ProgressBar());
			this.lb3 = (new System.Windows.Forms.Label());
			this.pbLocalSearch = (new System.Windows.Forms.ProgressBar());
			this.lb5 = (new System.Windows.Forms.Label());
			this.pbRSS = (new System.Windows.Forms.ProgressBar());
			this.lb4 = (new System.Windows.Forms.Label());
			this.pbuTorrent = (new System.Windows.Forms.ProgressBar());
			this.lb6 = (new System.Windows.Forms.Label());
			this.pbFolderThumbs = (new System.Windows.Forms.ProgressBar());
			this.timer1 = (new System.Windows.Forms.Timer(this.components));
			this.SuspendLayout();
			// 
			// bnCancel
			// 
			this.bnCancel.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
			this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bnCancel.Location = new System.Drawing.Point(289, 127);
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
			this.lb1.Size = new System.Drawing.Size(81, 13);
			this.lb1.TabIndex = 1;
			this.lb1.Text = "Rename Check";
			// 
			// pbRename
			// 
			this.pbRename.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			this.pbRename.Location = new System.Drawing.Point(141, 9);
			this.pbRename.Name = "pbRename";
			this.pbRename.Size = new System.Drawing.Size(223, 13);
			this.pbRename.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.pbRename.TabIndex = 2;
			// 
			// lb2
			// 
			this.lb2.AutoSize = true;
			this.lb2.Location = new System.Drawing.Point(12, 28);
			this.lb2.Name = "lb2";
			this.lb2.Size = new System.Drawing.Size(117, 13);
			this.lb2.TabIndex = 1;
			this.lb2.Text = "Missing Episode Check";
			// 
			// pbMissing
			// 
			this.pbMissing.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			this.pbMissing.Location = new System.Drawing.Point(141, 28);
			this.pbMissing.Name = "pbMissing";
			this.pbMissing.Size = new System.Drawing.Size(223, 13);
			this.pbMissing.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.pbMissing.TabIndex = 2;
			// 
			// lb3
			// 
			this.lb3.AutoSize = true;
			this.lb3.Location = new System.Drawing.Point(12, 47);
			this.lb3.Name = "lb3";
			this.lb3.Size = new System.Drawing.Size(77, 13);
			this.lb3.TabIndex = 1;
			this.lb3.Text = "Search Locally";
			// 
			// pbLocalSearch
			// 
			this.pbLocalSearch.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			this.pbLocalSearch.Location = new System.Drawing.Point(141, 47);
			this.pbLocalSearch.Name = "pbLocalSearch";
			this.pbLocalSearch.Size = new System.Drawing.Size(223, 13);
			this.pbLocalSearch.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.pbLocalSearch.TabIndex = 2;
			// 
			// lb5
			// 
			this.lb5.AutoSize = true;
			this.lb5.Location = new System.Drawing.Point(12, 85);
			this.lb5.Name = "lb5";
			this.lb5.Size = new System.Drawing.Size(66, 13);
			this.lb5.TabIndex = 1;
			this.lb5.Text = "Search RSS";
			// 
			// pbRSS
			// 
			this.pbRSS.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			this.pbRSS.Location = new System.Drawing.Point(141, 85);
			this.pbRSS.Name = "pbRSS";
			this.pbRSS.Size = new System.Drawing.Size(223, 13);
			this.pbRSS.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.pbRSS.TabIndex = 2;
			// 
			// lb4
			// 
			this.lb4.AutoSize = true;
			this.lb4.Location = new System.Drawing.Point(12, 66);
			this.lb4.Name = "lb4";
			this.lb4.Size = new System.Drawing.Size(81, 13);
			this.lb4.TabIndex = 1;
			this.lb4.Text = "Check µTorrent";
			// 
			// pbuTorrent
			// 
			this.pbuTorrent.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			this.pbuTorrent.Location = new System.Drawing.Point(141, 66);
			this.pbuTorrent.Name = "pbuTorrent";
			this.pbuTorrent.Size = new System.Drawing.Size(223, 13);
			this.pbuTorrent.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.pbuTorrent.TabIndex = 2;
			// 
			// lb6
			// 
			this.lb6.AutoSize = true;
			this.lb6.Location = new System.Drawing.Point(12, 104);
			this.lb6.Name = "lb6";
			this.lb6.Size = new System.Drawing.Size(93, 13);
			this.lb6.TabIndex = 1;
			this.lb6.Text = "Folder Thumbnails";
			// 
			// pbFolderThumbs
			// 
			this.pbFolderThumbs.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			this.pbFolderThumbs.Location = new System.Drawing.Point(141, 104);
			this.pbFolderThumbs.Name = "pbFolderThumbs";
			this.pbFolderThumbs.Size = new System.Drawing.Size(223, 13);
			this.pbFolderThumbs.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.pbFolderThumbs.TabIndex = 2;
			// 
			// timer1
			// 
			this.timer1.Interval = 100;
			this.timer1.Tick += new System.EventHandler(timer1_Tick);
			// 
			// ScanProgress
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.bnCancel;
			this.ClientSize = new System.Drawing.Size(376, 162);
			this.Controls.Add(this.pbFolderThumbs);
			this.Controls.Add(this.lb6);
			this.Controls.Add(this.pbuTorrent);
			this.Controls.Add(this.lb4);
			this.Controls.Add(this.pbRSS);
			this.Controls.Add(this.lb5);
			this.Controls.Add(this.pbLocalSearch);
			this.Controls.Add(this.lb3);
			this.Controls.Add(this.pbMissing);
			this.Controls.Add(this.lb2);
			this.Controls.Add(this.pbRename);
			this.Controls.Add(this.lb1);
			this.Controls.Add(this.bnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ScanProgress";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Scan Progress";
			this.Load += new System.EventHandler(ScanProgress_Load);
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