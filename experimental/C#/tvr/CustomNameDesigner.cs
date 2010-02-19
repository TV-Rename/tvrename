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
	/// Summary for CustomNameDesigner
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public class CustomNameDesigner : System.Windows.Forms.Form
	{
		private ProcessedEpisodeList Eps;
		private CustomName CN;
		private TVDoc mDoc;

		public CustomNameDesigner(ProcessedEpisodeList pel, CustomName cn, TVDoc doc)
		{
			Eps = pel;
			CN = cn;
			mDoc = doc;

			InitializeComponent();

			if (Eps == null)
				lvTest.Enabled = false;
			txtTemplate.Text = CN.StyleString;

			FillExamples();
			FillCombos();
		}

		private void FillCombos()
		{
			cbTags.Items.Clear();
			cbPresets.Items.Clear();
			ProcessedEpisode pe = null;
			if (lvTest.SelectedItems.Count == 0)
				pe = ((Eps != null) && (Eps.Count>0)) ? Eps[0] : null;
			else
				pe = (ProcessedEpisode)(lvTest.SelectedItems[0].Tag);

			foreach (string s in CustomName.Tags())
			{
				string txt = s;
				if (pe != null)
					txt += " - " + CustomName.NameForNoExt(pe, s);
				cbTags.Items.Add(txt);
			}

			foreach (string s in CustomName.Presets())
			{
				if (pe != null)
					cbPresets.Items.Add(CustomName.NameForNoExt(pe, s));
				else
					cbPresets.Items.Add(s);
			}

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		
	private System.Windows.Forms.Label label1;
	private System.Windows.Forms.TextBox txtTemplate;

	private System.Windows.Forms.ComboBox cbPresets;
	private System.Windows.Forms.Label label2;
	private System.Windows.Forms.Label label3;
	private System.Windows.Forms.ListView lvTest;
	private System.Windows.Forms.ColumnHeader columnHeader1;
	private System.Windows.Forms.ColumnHeader columnHeader2;
	private System.Windows.Forms.ColumnHeader columnHeader3;
	private System.Windows.Forms.Button bnOK;

	private System.Windows.Forms.Button bnCancel;

	private System.Windows.Forms.Label label4;
	private System.Windows.Forms.ComboBox cbTags;


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
			System.ComponentModel.ComponentResourceManager resources = (new System.ComponentModel.ComponentResourceManager(typeof(CustomNameDesigner)));
			this.label1 = (new System.Windows.Forms.Label());
			this.txtTemplate = (new System.Windows.Forms.TextBox());
			this.cbPresets = (new System.Windows.Forms.ComboBox());
			this.label2 = (new System.Windows.Forms.Label());
			this.label3 = (new System.Windows.Forms.Label());
			this.lvTest = (new System.Windows.Forms.ListView());
			this.columnHeader1 = (new System.Windows.Forms.ColumnHeader());
			this.columnHeader2 = (new System.Windows.Forms.ColumnHeader());
			this.columnHeader3 = (new System.Windows.Forms.ColumnHeader());
			this.bnOK = (new System.Windows.Forms.Button());
			this.bnCancel = (new System.Windows.Forms.Button());
			this.label4 = (new System.Windows.Forms.Label());
			this.cbTags = (new System.Windows.Forms.ComboBox());
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(89, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Naming template:";
			// 
			// txtTemplate
			// 
			this.txtTemplate.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			this.txtTemplate.Location = new System.Drawing.Point(107, 10);
			this.txtTemplate.Name = "txtTemplate";
			this.txtTemplate.Size = new System.Drawing.Size(590, 20);
			this.txtTemplate.TabIndex = 1;
			this.txtTemplate.TextChanged += new System.EventHandler(txtTemplate_TextChanged);
			// 
			// cbPresets
			// 
			this.cbPresets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbPresets.FormattingEnabled = true;
			this.cbPresets.Location = new System.Drawing.Point(107, 36);
			this.cbPresets.Name = "cbPresets";
			this.cbPresets.Size = new System.Drawing.Size(381, 21);
			this.cbPresets.TabIndex = 2;
			this.cbPresets.SelectedIndexChanged += new System.EventHandler(cbPresets_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(45, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Presets:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 98);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(90, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Sample and Test:";
			// 
			// lvTest
			// 
			this.lvTest.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			this.lvTest.Columns.AddRange(new System.Windows.Forms.ColumnHeader[3] {this.columnHeader1, this.columnHeader2, this.columnHeader3});
			this.lvTest.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lvTest.Location = new System.Drawing.Point(15, 114);
			this.lvTest.Name = "lvTest";
			this.lvTest.Size = new System.Drawing.Size(682, 265);
			this.lvTest.TabIndex = 3;
			this.lvTest.UseCompatibleStateImageBehavior = false;
			this.lvTest.View = System.Windows.Forms.View.Details;
			this.lvTest.SelectedIndexChanged += new System.EventHandler(lvTest_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Example";
			this.columnHeader1.Width = 456;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Season";
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Episode";
			// 
			// bnOK
			// 
			this.bnOK.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
			this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bnOK.Location = new System.Drawing.Point(541, 385);
			this.bnOK.Name = "bnOK";
			this.bnOK.Size = new System.Drawing.Size(75, 23);
			this.bnOK.TabIndex = 4;
			this.bnOK.Text = "OK";
			this.bnOK.UseVisualStyleBackColor = true;
			// 
			// bnCancel
			// 
			this.bnCancel.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
			this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bnCancel.Location = new System.Drawing.Point(622, 385);
			this.bnCancel.Name = "bnCancel";
			this.bnCancel.Size = new System.Drawing.Size(75, 23);
			this.bnCancel.TabIndex = 5;
			this.bnCancel.Text = "Cancel";
			this.bnCancel.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 66);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(34, 13);
			this.label4.TabIndex = 0;
			this.label4.Text = "Tags:";
			// 
			// cbTags
			// 
			this.cbTags.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbTags.FormattingEnabled = true;
			this.cbTags.Location = new System.Drawing.Point(107, 63);
			this.cbTags.Name = "cbTags";
			this.cbTags.Size = new System.Drawing.Size(381, 21);
			this.cbTags.TabIndex = 2;
			this.cbTags.SelectedIndexChanged += new System.EventHandler(cbTags_SelectedIndexChanged);
			// 
			// CustomNameDesigner
			// 
			this.AcceptButton = this.bnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.bnCancel;
			this.ClientSize = new System.Drawing.Size(709, 420);
			this.Controls.Add(this.bnCancel);
			this.Controls.Add(this.bnOK);
			this.Controls.Add(this.lvTest);
			this.Controls.Add(this.cbTags);
			this.Controls.Add(this.cbPresets);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtTemplate);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "CustomNameDesigner";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Filename Template Editor";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

#endregion
		private void FillExamples()
		{
			if (Eps == null)
				return;

			lvTest.Items.Clear();
			foreach (ProcessedEpisode pe in Eps)
			{
				ListViewItem lvi = new ListViewItem();
				string fn = mDoc.FilenameFriendly(CN.NameForExt(pe,null));
				lvi.Text = fn;

				bool ok = false;
				bool ok1 = false;
				bool ok2 = false;
				if (fn.Length < 255)
				{
					int seas;
					int ep;
					ok = mDoc.FindSeasEp(new FileInfo(fn+".avi"), out seas, out ep, pe.SI.ShowName());
					ok1 = ok && (seas == pe.SeasonNumber);
					ok2 = ok && (ep == pe.EpNum);
					string pre1 = ok1 ? "" : "* ";
					string pre2 = ok2 ? "" : "* ";

					lvi.SubItems.Add(pre1 + ((seas != -1) ? seas.ToString() : ""));
					lvi.SubItems.Add(pre2 + ((ep != -1) ? ep.ToString() : ""));
					lvi.Tag = pe;
				}
				if (!ok || !ok1 || !ok2)
					lvi.BackColor = Helpers.WarningColor();
				lvTest.Items.Add(lvi);
			}
		}
	private void cbPresets_SelectedIndexChanged(object sender, System.EventArgs e)
			 {
				 int n = cbPresets.SelectedIndex;
				 if (n == -1)
					 return;

				 txtTemplate.Text = CustomName.Presets()[n];
				 cbPresets.SelectedIndex = -1;
			 }
	private void txtTemplate_TextChanged(object sender, System.EventArgs e)
			 {
				 CN.StyleString = txtTemplate.Text;
				 FillExamples();
			 }
	private void cbTags_SelectedIndexChanged(object sender, System.EventArgs e)
			 {
				 int n = cbTags.SelectedIndex;
				 if (n == -1)
					 return;

				 int p = txtTemplate.SelectionStart;
				 string s = txtTemplate.Text;
				 txtTemplate.Text = s.Substring(0, p) + CustomName.Tags()[cbTags.SelectedIndex] + s.Substring(p);

				 cbTags.SelectedIndex = -1;
			 }
	private void lvTest_SelectedIndexChanged(object sender, System.EventArgs e)
			 {
				 FillCombos();
			 }
	}
}