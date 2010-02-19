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
	/// Summary for RecoverXML
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public class RecoverXML : System.Windows.Forms.Form
	{
		private FileInfo[] SettingsList;
		private FileInfo[] DBList;

		public FileInfo SettingsFile;
	private System.Windows.Forms.Label lbHint;
	private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
	private System.Windows.Forms.Button bnCancel;
		public FileInfo DBFile;

		public RecoverXML(string hint)
		{
			InitializeComponent();
			SettingsFile = null;
			DBFile = null;
			if (!string.IsNullOrEmpty(hint))
				lbHint.Text = hint+"\r\n ";
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>

	private System.Windows.Forms.Button bnOK;
	private System.Windows.Forms.ListBox lbSettings;


	private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	private System.Windows.Forms.Panel panel2;
	private System.Windows.Forms.Label label2;
	private System.Windows.Forms.ListBox lbDB;

	private System.Windows.Forms.Panel panel1;
	private System.Windows.Forms.Label label1;
	private System.Windows.Forms.Label label3;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components;

#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = (new System.ComponentModel.ComponentResourceManager(typeof(RecoverXML)));
			this.bnOK = (new System.Windows.Forms.Button());
			this.lbSettings = (new System.Windows.Forms.ListBox());
			this.tableLayoutPanel1 = (new System.Windows.Forms.TableLayoutPanel());
			this.panel2 = (new System.Windows.Forms.Panel());
			this.label2 = (new System.Windows.Forms.Label());
			this.lbDB = (new System.Windows.Forms.ListBox());
			this.panel1 = (new System.Windows.Forms.Panel());
			this.label1 = (new System.Windows.Forms.Label());
			this.label3 = (new System.Windows.Forms.Label());
			this.lbHint = (new System.Windows.Forms.Label());
			this.tableLayoutPanel2 = (new System.Windows.Forms.TableLayoutPanel());
			this.bnCancel = (new System.Windows.Forms.Button());
			this.tableLayoutPanel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// bnOK
			// 
			this.bnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.bnOK.Location = new System.Drawing.Point(173, 366);
			this.bnOK.Name = "bnOK";
			this.bnOK.Size = new System.Drawing.Size(75, 23);
			this.bnOK.TabIndex = 0;
			this.bnOK.Text = "OK";
			this.bnOK.UseVisualStyleBackColor = true;
			this.bnOK.Click += new System.EventHandler(bnOK_Click);
			// 
			// lbSettings
			// 
			this.lbSettings.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			this.lbSettings.FormattingEnabled = true;
			this.lbSettings.IntegralHeight = false;
			this.lbSettings.Location = new System.Drawing.Point(0, 16);
			this.lbSettings.Name = "lbSettings";
			this.lbSettings.Size = new System.Drawing.Size(154, 268);
			this.lbSettings.TabIndex = 1;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add((new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50)));
			this.tableLayoutPanel1.ColumnStyles.Add((new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50)));
			this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 55);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add((new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50)));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(321, 290);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.label2);
			this.panel2.Controls.Add(this.lbDB);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(163, 3);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(155, 284);
			this.panel2.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(-3, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(113, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "&TheTVDB local cache";
			// 
			// lbDB
			// 
			this.lbDB.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			this.lbDB.FormattingEnabled = true;
			this.lbDB.IntegralHeight = false;
			this.lbDB.Location = new System.Drawing.Point(0, 16);
			this.lbDB.Name = "lbDB";
			this.lbDB.Size = new System.Drawing.Size(155, 268);
			this.lbDB.TabIndex = 1;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.lbSettings);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(3, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(154, 284);
			this.panel1.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(-3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "&Application Settings";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label3.Location = new System.Drawing.Point(3, 13);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(321, 39);
			this.label3.TabIndex = 1;
			this.label3.Text = "Select the versions of the settings files to use, then click OK.  Changes to your settings are not permanent, until you do a File->Save.";
			// 
			// lbHint
			// 
			this.lbHint.AutoSize = true;
			this.lbHint.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbHint.Font = (new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (System.Byte)(0)));
			this.lbHint.Location = new System.Drawing.Point(3, 0);
			this.lbHint.Name = "lbHint";
			this.lbHint.Size = new System.Drawing.Size(321, 13);
			this.lbHint.TabIndex = 0;
			this.lbHint.Text = "<< Error Description >>";
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add((new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100)));
			this.tableLayoutPanel2.Controls.Add(this.lbHint, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this.label3, 0, 1);
			this.tableLayoutPanel2.Location = new System.Drawing.Point(8, 12);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 3;
			this.tableLayoutPanel2.RowStyles.Add((new System.Windows.Forms.RowStyle()));
			this.tableLayoutPanel2.RowStyles.Add((new System.Windows.Forms.RowStyle()));
			this.tableLayoutPanel2.RowStyles.Add((new System.Windows.Forms.RowStyle()));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(327, 348);
			this.tableLayoutPanel2.TabIndex = 4;
			// 
			// bnCancel
			// 
			this.bnCancel.Location = new System.Drawing.Point(254, 366);
			this.bnCancel.Name = "bnCancel";
			this.bnCancel.Size = new System.Drawing.Size(75, 23);
			this.bnCancel.TabIndex = 5;
			this.bnCancel.Text = "Cancel";
			this.bnCancel.UseVisualStyleBackColor = true;
			this.bnCancel.Click += new System.EventHandler(bnCancel_Click);
			// 
			// RecoverXML
			// 
			this.AcceptButton = this.bnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(350, 396);
			this.Controls.Add(this.bnCancel);
			this.Controls.Add(this.tableLayoutPanel2);
			this.Controls.Add(this.bnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "RecoverXML";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "TVRename Recover";
			this.Load += new System.EventHandler(RecoverXML_Load);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.ResumeLayout(false);

		}
#endregion
	private void RecoverXML_Load(object sender, System.EventArgs e)
			 {
				 SettingsList = new DirectoryInfo(System.Windows.Forms.Application.UserAppDataPath).GetFiles("TVRenameSettings.xml*");
				 DBList = new DirectoryInfo(System.Windows.Forms.Application.UserAppDataPath).GetFiles("TheTVDB.xml*");

				 lbSettings.Items.Add("Default settings");
				 if ((SettingsList != null) && SettingsList.Length > 0)
				 {
					 foreach (FileInfo fi in SettingsList)
						 lbSettings.Items.Add(fi.LastWriteTime.ToString("g"));
					 lbSettings.SelectedIndex = 0;
				 }

				 lbDB.Items.Add("None");
				 if ((DBList != null) && DBList.Length>0)
				 {
					 foreach (FileInfo fi in DBList)
						 lbDB.Items.Add(fi.LastWriteTime.ToString("g"));
					 lbDB.SelectedIndex = 0;
				 }

			 }
private void bnOK_Click(object sender, System.EventArgs e)
		 {
			 // we added a 'none' item at the top of the list, so adjust for that

			 int n = lbDB.SelectedIndex;
			 if (n == -1)
				 n = 0;
			 DBFile = (n == 0) ? null : DBList[n-1];

			 n = lbSettings.SelectedIndex;
			 if (n == -1)
				 n = 0;
			 SettingsFile = (n == 0) ? null : SettingsList[n];

			 this.DialogResult =DialogResult.OK;
			 this.Close();
		 }
private void bnCancel_Click(object sender, System.EventArgs e)
		 {
			 this.DialogResult =DialogResult.Cancel;
			 this.Close();
		 }
}
}