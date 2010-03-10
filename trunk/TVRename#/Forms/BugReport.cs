//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
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
using System.IO;

namespace TVRename
{

	/// <summary>
	/// Summary for BugReport
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public class BugReport : System.Windows.Forms.Form
	{
		private TVDoc mDoc;

		public BugReport(TVDoc doc)
		{
			mDoc = doc;
			InitializeComponent();
			//
			//TODO: Add the constructor code here
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
	private System.Windows.Forms.TextBox txtName;

	private System.Windows.Forms.Label label2;
	private System.Windows.Forms.Label label3;
	private System.Windows.Forms.TextBox txtEmail;

	private System.Windows.Forms.Label label1;
	private System.Windows.Forms.TextBox txtDesc1;

	private System.Windows.Forms.Label label4;
	private System.Windows.Forms.TextBox txtDesc2;

	private System.Windows.Forms.Label label5;
	private System.Windows.Forms.TextBox txtFreq;
	private System.Windows.Forms.TextBox txtComments;


	private System.Windows.Forms.Label label6;
	private System.Windows.Forms.Label label7;
	private System.Windows.Forms.CheckBox cbSettings;
	private System.Windows.Forms.CheckBox cbFOScan;



	private System.Windows.Forms.Button bnCreate;
	private System.Windows.Forms.TextBox txtEmailText;


	private System.Windows.Forms.Label label8;
	private System.Windows.Forms.Button bnClose;
	private System.Windows.Forms.CheckBox cbFolderScan;
	private System.Windows.Forms.Button bnCopy;


#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = (new System.ComponentModel.ComponentResourceManager(typeof(BugReport)));
			this.txtName = (new System.Windows.Forms.TextBox());
			this.label2 = (new System.Windows.Forms.Label());
			this.label3 = (new System.Windows.Forms.Label());
			this.txtEmail = (new System.Windows.Forms.TextBox());
			this.label1 = (new System.Windows.Forms.Label());
			this.txtDesc1 = (new System.Windows.Forms.TextBox());
			this.label4 = (new System.Windows.Forms.Label());
			this.txtDesc2 = (new System.Windows.Forms.TextBox());
			this.label5 = (new System.Windows.Forms.Label());
			this.txtFreq = (new System.Windows.Forms.TextBox());
			this.txtComments = (new System.Windows.Forms.TextBox());
			this.label6 = (new System.Windows.Forms.Label());
			this.label7 = (new System.Windows.Forms.Label());
			this.cbSettings = (new System.Windows.Forms.CheckBox());
			this.cbFOScan = (new System.Windows.Forms.CheckBox());
			this.bnCreate = (new System.Windows.Forms.Button());
			this.txtEmailText = (new System.Windows.Forms.TextBox());
			this.label8 = (new System.Windows.Forms.Label());
			this.bnClose = (new System.Windows.Forms.Button());
			this.cbFolderScan = (new System.Windows.Forms.CheckBox());
			this.bnCopy = (new System.Windows.Forms.Button());
			this.SuspendLayout();
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(56, 12);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(299, 20);
			this.txtName.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 15);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Name:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 41);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(35, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Email:";
			// 
			// txtEmail
			// 
			this.txtEmail.Location = new System.Drawing.Point(56, 38);
			this.txtEmail.Name = "txtEmail";
			this.txtEmail.Size = new System.Drawing.Size(299, 20);
			this.txtEmail.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 67);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(155, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Brief description of the problem:";
			// 
			// txtDesc1
			// 
			this.txtDesc1.Location = new System.Drawing.Point(56, 83);
			this.txtDesc1.Multiline = true;
			this.txtDesc1.Name = "txtDesc1";
			this.txtDesc1.Size = new System.Drawing.Size(299, 40);
			this.txtDesc1.TabIndex = 1;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 129);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(258, 13);
			this.label4.TabIndex = 0;
			this.label4.Text = "Detailed description, and steps to repeat the problem:";
			// 
			// txtDesc2
			// 
			this.txtDesc2.Location = new System.Drawing.Point(56, 145);
			this.txtDesc2.Multiline = true;
			this.txtDesc2.Name = "txtDesc2";
			this.txtDesc2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtDesc2.Size = new System.Drawing.Size(299, 142);
			this.txtDesc2.TabIndex = 1;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 290);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(183, 13);
			this.label5.TabIndex = 0;
			this.label5.Text = "How often does this problem happen:";
			// 
			// txtFreq
			// 
			this.txtFreq.Location = new System.Drawing.Point(56, 310);
			this.txtFreq.Name = "txtFreq";
			this.txtFreq.Size = new System.Drawing.Size(299, 20);
			this.txtFreq.TabIndex = 1;
			// 
			// txtComments
			// 
			this.txtComments.Location = new System.Drawing.Point(56, 356);
			this.txtComments.Multiline = true;
			this.txtComments.Name = "txtComments";
			this.txtComments.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtComments.Size = new System.Drawing.Size(299, 112);
			this.txtComments.TabIndex = 3;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(12, 340);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(147, 13);
			this.label6.TabIndex = 2;
			this.label6.Text = "Any other comments or notes:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(12, 488);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(45, 13);
			this.label7.TabIndex = 2;
			this.label7.Text = "Include:";
			// 
			// cbSettings
			// 
			this.cbSettings.AutoSize = true;
			this.cbSettings.Checked = true;
			this.cbSettings.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbSettings.Location = new System.Drawing.Point(56, 504);
			this.cbSettings.Name = "cbSettings";
			this.cbSettings.Size = new System.Drawing.Size(88, 17);
			this.cbSettings.TabIndex = 4;
			this.cbSettings.Text = "Settings Files";
			this.cbSettings.UseVisualStyleBackColor = true;
			// 
			// cbFOScan
			// 
			this.cbFOScan.AutoSize = true;
			this.cbFOScan.Checked = true;
			this.cbFOScan.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbFOScan.Location = new System.Drawing.Point(150, 504);
			this.cbFOScan.Name = "cbFOScan";
			this.cbFOScan.Size = new System.Drawing.Size(74, 17);
			this.cbFOScan.TabIndex = 4;
			this.cbFOScan.Text = "F&&O Scan";
			this.cbFOScan.UseVisualStyleBackColor = true;
			// 
			// bnCreate
			// 
			this.bnCreate.Location = new System.Drawing.Point(15, 532);
			this.bnCreate.Name = "bnCreate";
			this.bnCreate.Size = new System.Drawing.Size(75, 23);
			this.bnCreate.TabIndex = 5;
			this.bnCreate.Text = "Create";
			this.bnCreate.UseVisualStyleBackColor = true;
			this.bnCreate.Click += new System.EventHandler(bnCreate_Click);
			// 
			// txtEmailText
			// 
			this.txtEmailText.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left));
			this.txtEmailText.Location = new System.Drawing.Point(399, 83);
			this.txtEmailText.Multiline = true;
			this.txtEmailText.Name = "txtEmailText";
			this.txtEmailText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtEmailText.Size = new System.Drawing.Size(461, 441);
			this.txtEmailText.TabIndex = 1;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(396, 12);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(464, 68);
			this.label8.TabIndex = 0;
			this.label8.Text = resources.GetString("label8.Text");
			// 
			// bnClose
			// 
			this.bnClose.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
			this.bnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bnClose.Location = new System.Drawing.Point(785, 530);
			this.bnClose.Name = "bnClose";
			this.bnClose.Size = new System.Drawing.Size(75, 23);
			this.bnClose.TabIndex = 5;
			this.bnClose.Text = "Close";
			this.bnClose.UseVisualStyleBackColor = true;
			// 
			// cbFolderScan
			// 
			this.cbFolderScan.AutoSize = true;
			this.cbFolderScan.Checked = true;
			this.cbFolderScan.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbFolderScan.Location = new System.Drawing.Point(230, 504);
			this.cbFolderScan.Name = "cbFolderScan";
			this.cbFolderScan.Size = new System.Drawing.Size(83, 17);
			this.cbFolderScan.TabIndex = 4;
			this.cbFolderScan.Text = "Folder Scan";
			this.cbFolderScan.UseVisualStyleBackColor = true;
			// 
			// bnCopy
			// 
			this.bnCopy.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.bnCopy.Location = new System.Drawing.Point(399, 530);
			this.bnCopy.Name = "bnCopy";
			this.bnCopy.Size = new System.Drawing.Size(75, 23);
			this.bnCopy.TabIndex = 5;
			this.bnCopy.Text = "Copy";
			this.bnCopy.UseVisualStyleBackColor = true;
			this.bnCopy.Click += new System.EventHandler(bnCopy_Click);
			// 
			// BugReport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.bnClose;
			this.ClientSize = new System.Drawing.Size(875, 567);
			this.Controls.Add(this.bnCopy);
			this.Controls.Add(this.bnClose);
			this.Controls.Add(this.bnCreate);
			this.Controls.Add(this.cbFolderScan);
			this.Controls.Add(this.cbFOScan);
			this.Controls.Add(this.cbSettings);
			this.Controls.Add(this.txtComments);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.txtEmailText);
			this.Controls.Add(this.txtDesc2);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.txtDesc1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtFreq);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.txtEmail);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.label2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "BugReport";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "BugReport";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

#endregion
	private void bnCreate_Click(object sender, System.EventArgs e)
	{
		txtEmailText.Text = "Working... This may take a while.";
		txtEmailText.Update();

		string txt = "";
		txt += "From: " + txtName.Text + " <" + txtEmail.Text + ">" + "\r\n";
		txt += "Subject: TVRename bug report" + "\r\n";
		txt += "\r\n";
		txt += "TVRename version: " + Version.DisplayVersionString() + "\r\n";
		txt += "UserAppDataPath is " + System.Windows.Forms.Application.UserAppDataPath + "\r\n";
		txt += "EpGuidePath is " + UI.EpGuidePath() + "\r\n";
		txt += "\r\n";
		txt += "==== Brief Description ====" + "\r\n";
		txt += txtDesc1.Text + "\r\n";
		txt += "\r\n";
		txt += "==== Description ====" + "\r\n";
		txt += txtDesc2.Text + "\r\n";
		txt += "\r\n";
		txt += "==== Frequency ====" + "\r\n";
		txt += txtFreq.Text + "\r\n";
		txt += "\r\n";
		txt += "==== Notes and Comments ====" + "\r\n";
		txt += txtComments.Text + "\r\n";
		txt += "\r\n";


		if (cbSettings.Checked)
		{
			txt += "==== Settings Files ====" + "\r\n";
			txt += "\r\n";
			txt += "---- TVRenameSettings.xml" + "\r\n";
			txt += "\r\n";
			try
			{
				StreamReader sr = new StreamReader(System.Windows.Forms.Application.UserAppDataPath+System.IO.Path.DirectorySeparatorChar.ToString()+"TVRenameSettings.xml");
				txt += sr.ReadToEnd();
				sr.Close();
				txt += "\r\n";
			}
			catch
			{
				txt += "Error reading TVRenameSettings.xml\r\n";
			}
			txt += "\r\n";
		}

		if (cbFOScan.Checked || cbFolderScan.Checked)
		{
			txt += "==== Filename processors ====\r\n";
			foreach (FilenameProcessorRE s in mDoc.Settings.FNPRegexs)
				txt += (s.Enabled ? "Enabled":"Disabled") + " \""+s.RE+"\" "+(s.UseFullPath?"(FullPath)":"")+"\r\n";
			txt += "\r\n";
		}

		if (cbFOScan.Checked)
		{
			txt += "==== Finding & Organising Directory Scan ====" + "\r\n";
			txt += "\r\n";

            DirCache dirC = new DirCache();
            foreach (string efi in mDoc.SearchFolders)
                dirC.AddFolder(null, 0, 0, efi, true, mDoc.Settings);

			foreach (DirCacheEntry fi in dirC)
			{
				int seas;
				int ep;
				bool r = mDoc.FindSeasEp(fi.TheFile, out seas, out ep, null);
				bool useful = fi.HasUsefulExtension_NotOthersToo;
				txt += fi.TheFile.FullName + " ("+(r?"OK":"No")+" " + seas.ToString()+","+ep.ToString()+" "+(useful?fi.TheFile.Extension:"-")+")" + "\r\n";
			}
			txt += "\r\n";
		}

		if (cbFolderScan.Checked)
		{
			txt += "==== Media Folders Directory Scan ====" + "\r\n";

			foreach (ShowItem si in mDoc.GetShowItems(true))
			{
				foreach (System.Collections.Generic.KeyValuePair<int, ProcessedEpisodeList> kvp in si.SeasonEpisodes)
				{
					int snum = kvp.Key;
					if (((snum == 0) && (si.CountSpecials)) || !si.AllFolderLocations(mDoc.Settings).ContainsKey(snum))
						continue; // skip specials


					foreach (string folder in si.AllFolderLocations(mDoc.Settings)[snum])
					{
						txt += si.TVDBCode + " : " + si.ShowName() + " : S" + snum.ToString() + "\r\n";
						txt += "Folder: " + folder;
						txt += "\r\n";
                        DirCache files = new DirCache();
                        if (Directory.Exists(folder))
                            files.AddFolder(null, 0, 0, folder, true, mDoc.Settings);
						foreach (DirCacheEntry fi in files)
						{
							int seas;
							int ep;
							bool r = mDoc.FindSeasEp(fi.TheFile, out seas, out ep, si.ShowName());
							bool useful = fi.HasUsefulExtension_NotOthersToo;
							txt += fi.TheFile.FullName + " ("+(r?"OK":"No")+" " + seas.ToString()+","+ep.ToString()+" "+(useful?fi.TheFile.Extension:"-")+")" + "\r\n";
						}
						txt += "\r\n";
					}
				}
				txt += "\r\n";
			}
			mDoc.UnlockShowItems();

			txt += "\r\n";
		}

		txtEmailText.Text = txt;
	}
	private void bnCopy_Click(object sender, System.EventArgs e)
			 {
				 Clipboard.SetDataObject(txtEmailText.Text);
			 }
	}
}



namespace TVRename
{


} // namespace