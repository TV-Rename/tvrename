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
	/// Summary for EditRules
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public class EditRules : System.Windows.Forms.Form
	{
		private System.Collections.Generic.List<ShowRule > WorkingRuleSet;
		private ShowItem mSI;
		private int mSeasonNumber;
		private System.Collections.Generic.List<ProcessedEpisode > mOriginalEps;
		private CustomName NameStyle;

		public EditRules(ShowItem si, System.Collections.Generic.List<ProcessedEpisode > originalEpList, int seasonNumber, CustomName style)
		{
			NameStyle = style;
			InitializeComponent();

			mSI = si;
			mOriginalEps = originalEpList;
			mSeasonNumber = seasonNumber;

			if (si.SeasonRules.ContainsKey(seasonNumber))
				WorkingRuleSet = new System.Collections.Generic.List<ShowRule >(si.SeasonRules[seasonNumber]);
			else
				WorkingRuleSet = new System.Collections.Generic.List<ShowRule >();

			txtShowName.Text = si.ShowName();
			txtSeasonNumber.Text = seasonNumber.ToString();

			FillRuleList(false, 0);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
	private System.Windows.Forms.Button bnRuleUp;
	private System.Windows.Forms.Button bnRuleDown;
	private System.Windows.Forms.ListView lvRuleList;
	private System.Windows.Forms.ColumnHeader columnHeader3;
	private System.Windows.Forms.ColumnHeader columnHeader4;
	private System.Windows.Forms.ColumnHeader columnHeader5;
	private System.Windows.Forms.ColumnHeader columnHeader6;
	private System.Windows.Forms.Button bnDelRule;
	private System.Windows.Forms.Button bnEdit;
	private System.Windows.Forms.Button bnAddRule;
	private System.Windows.Forms.Label label1;
	private System.Windows.Forms.Label label2;
	private System.Windows.Forms.Label label3;
	private System.Windows.Forms.ListBox lbEpsPreview;

	private System.Windows.Forms.Label label4;
	private System.Windows.Forms.Button bnCancel;

	private System.Windows.Forms.Button bnOK;

	private System.Windows.Forms.Label txtShowName;
	private System.Windows.Forms.Label txtSeasonNumber;

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
			System.ComponentModel.ComponentResourceManager resources = (new System.ComponentModel.ComponentResourceManager(typeof(EditRules)));
			this.bnRuleUp = (new System.Windows.Forms.Button());
			this.bnRuleDown = (new System.Windows.Forms.Button());
			this.lvRuleList = (new System.Windows.Forms.ListView());
			this.columnHeader3 = (new System.Windows.Forms.ColumnHeader());
			this.columnHeader4 = (new System.Windows.Forms.ColumnHeader());
			this.columnHeader5 = (new System.Windows.Forms.ColumnHeader());
			this.columnHeader6 = (new System.Windows.Forms.ColumnHeader());
			this.bnDelRule = (new System.Windows.Forms.Button());
			this.bnEdit = (new System.Windows.Forms.Button());
			this.bnAddRule = (new System.Windows.Forms.Button());
			this.label1 = (new System.Windows.Forms.Label());
			this.label2 = (new System.Windows.Forms.Label());
			this.label3 = (new System.Windows.Forms.Label());
			this.lbEpsPreview = (new System.Windows.Forms.ListBox());
			this.label4 = (new System.Windows.Forms.Label());
			this.bnCancel = (new System.Windows.Forms.Button());
			this.bnOK = (new System.Windows.Forms.Button());
			this.txtShowName = (new System.Windows.Forms.Label());
			this.txtSeasonNumber = (new System.Windows.Forms.Label());
			this.SuspendLayout();
			// 
			// bnRuleUp
			// 
			this.bnRuleUp.Location = new System.Drawing.Point(255, 226);
			this.bnRuleUp.Name = "bnRuleUp";
			this.bnRuleUp.Size = new System.Drawing.Size(75, 23);
			this.bnRuleUp.TabIndex = 9;
			this.bnRuleUp.Text = "&Up";
			this.bnRuleUp.UseVisualStyleBackColor = true;
			this.bnRuleUp.Click += new System.EventHandler(bnRuleUp_Click);
			// 
			// bnRuleDown
			// 
			this.bnRuleDown.Location = new System.Drawing.Point(336, 226);
			this.bnRuleDown.Name = "bnRuleDown";
			this.bnRuleDown.Size = new System.Drawing.Size(75, 23);
			this.bnRuleDown.TabIndex = 10;
			this.bnRuleDown.Text = "Do&wn";
			this.bnRuleDown.UseVisualStyleBackColor = true;
			this.bnRuleDown.Click += new System.EventHandler(bnRuleDown_Click);
			// 
			// lvRuleList
			// 
			this.lvRuleList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[4] {this.columnHeader3, this.columnHeader4, this.columnHeader5, this.columnHeader6});
			this.lvRuleList.FullRowSelect = true;
			this.lvRuleList.HideSelection = false;
			this.lvRuleList.Location = new System.Drawing.Point(12, 73);
			this.lvRuleList.MultiSelect = false;
			this.lvRuleList.Name = "lvRuleList";
			this.lvRuleList.Size = new System.Drawing.Size(400, 147);
			this.lvRuleList.TabIndex = 5;
			this.lvRuleList.UseCompatibleStateImageBehavior = false;
			this.lvRuleList.View = System.Windows.Forms.View.Details;
			this.lvRuleList.DoubleClick += new System.EventHandler(lvRuleList_DoubleClick);
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Action";
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Episode";
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Episode";
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "Name";
			this.columnHeader6.Width = 205;
			// 
			// bnDelRule
			// 
			this.bnDelRule.Location = new System.Drawing.Point(174, 226);
			this.bnDelRule.Name = "bnDelRule";
			this.bnDelRule.Size = new System.Drawing.Size(75, 23);
			this.bnDelRule.TabIndex = 8;
			this.bnDelRule.Text = "&Delete";
			this.bnDelRule.UseVisualStyleBackColor = true;
			this.bnDelRule.Click += new System.EventHandler(bnDelRule_Click);
			// 
			// bnEdit
			// 
			this.bnEdit.Location = new System.Drawing.Point(93, 226);
			this.bnEdit.Name = "bnEdit";
			this.bnEdit.Size = new System.Drawing.Size(75, 23);
			this.bnEdit.TabIndex = 7;
			this.bnEdit.Text = "&Edit";
			this.bnEdit.UseVisualStyleBackColor = true;
			this.bnEdit.Click += new System.EventHandler(bnEdit_Click);
			// 
			// bnAddRule
			// 
			this.bnAddRule.Location = new System.Drawing.Point(12, 226);
			this.bnAddRule.Name = "bnAddRule";
			this.bnAddRule.Size = new System.Drawing.Size(75, 23);
			this.bnAddRule.TabIndex = 6;
			this.bnAddRule.Text = "&Add";
			this.bnAddRule.UseVisualStyleBackColor = true;
			this.bnAddRule.Click += new System.EventHandler(bnAddRule_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 54);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(37, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "&Rules:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 6);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(37, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Show:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(9, 29);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(46, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Season:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// lbEpsPreview
			// 
			this.lbEpsPreview.FormattingEnabled = true;
			this.lbEpsPreview.Location = new System.Drawing.Point(12, 276);
			this.lbEpsPreview.Name = "lbEpsPreview";
			this.lbEpsPreview.ScrollAlwaysVisible = true;
			this.lbEpsPreview.SelectionMode = System.Windows.Forms.SelectionMode.None;
			this.lbEpsPreview.Size = new System.Drawing.Size(400, 160);
			this.lbEpsPreview.TabIndex = 12;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(9, 257);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(48, 13);
			this.label4.TabIndex = 11;
			this.label4.Text = "Preview:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// bnCancel
			// 
			this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bnCancel.Location = new System.Drawing.Point(337, 442);
			this.bnCancel.Name = "bnCancel";
			this.bnCancel.Size = new System.Drawing.Size(75, 23);
			this.bnCancel.TabIndex = 14;
			this.bnCancel.Text = "Cancel";
			this.bnCancel.UseVisualStyleBackColor = true;
			this.bnCancel.Click += new System.EventHandler(bnCancel_Click);
			// 
			// bnOK
			// 
			this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bnOK.Location = new System.Drawing.Point(256, 442);
			this.bnOK.Name = "bnOK";
			this.bnOK.Size = new System.Drawing.Size(75, 23);
			this.bnOK.TabIndex = 13;
			this.bnOK.Text = "OK";
			this.bnOK.UseVisualStyleBackColor = true;
			this.bnOK.Click += new System.EventHandler(bnOK_Click);
			// 
			// txtShowName
			// 
			this.txtShowName.AutoSize = true;
			this.txtShowName.Location = new System.Drawing.Point(67, 6);
			this.txtShowName.Name = "txtShowName";
			this.txtShowName.Size = new System.Drawing.Size(16, 13);
			this.txtShowName.TabIndex = 1;
			this.txtShowName.Text = "---";
			this.txtShowName.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// txtSeasonNumber
			// 
			this.txtSeasonNumber.AutoSize = true;
			this.txtSeasonNumber.Location = new System.Drawing.Point(67, 29);
			this.txtSeasonNumber.Name = "txtSeasonNumber";
			this.txtSeasonNumber.Size = new System.Drawing.Size(16, 13);
			this.txtSeasonNumber.TabIndex = 3;
			this.txtSeasonNumber.Text = "---";
			this.txtSeasonNumber.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// EditRules
			// 
			this.AcceptButton = this.bnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.bnCancel;
			this.ClientSize = new System.Drawing.Size(424, 478);
			this.Controls.Add(this.lbEpsPreview);
			this.Controls.Add(this.bnOK);
			this.Controls.Add(this.bnRuleUp);
			this.Controls.Add(this.bnCancel);
			this.Controls.Add(this.bnRuleDown);
			this.Controls.Add(this.lvRuleList);
			this.Controls.Add(this.bnDelRule);
			this.Controls.Add(this.bnEdit);
			this.Controls.Add(this.bnAddRule);
			this.Controls.Add(this.txtSeasonNumber);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtShowName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "EditRules";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Season Rules";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
#endregion
	private void bnAddRule_Click(object sender, System.EventArgs e)
			 {
				 ShowRule sr = new ShowRule();
				 AddModifyRule ar = new AddModifyRule(sr);

				 bool res = ar.ShowDialog() == System.Windows.Forms.DialogResult.OK;
				 if (res)
				 WorkingRuleSet.Add(sr);

				 FillRuleList(false, 0);
			 }
			 private void FillRuleList(bool keepSel, int adj)
			 {
				 System.Collections.Generic.List<int> sel = new System.Collections.Generic.List<int>();
				 if (keepSel)
					 foreach (int i in lvRuleList.SelectedIndices)
						 sel.Add(i);

				 lvRuleList.Items.Clear();
				 foreach (ShowRule sr in WorkingRuleSet)
				 {
					 ListViewItem lvi = new ListViewItem();
					 lvi.Text = sr.ActionInWords();
					 lvi.SubItems.Add(sr.First == -1 ? "" : sr.First.ToString());
					 lvi.SubItems.Add(sr.Second == -1 ? "" : sr.Second.ToString());
					 lvi.SubItems.Add(sr.UserSuppliedText);
					 lvi.Tag = sr;
					 lvRuleList.Items.Add(lvi);
				 }

				 if (keepSel)
					 foreach (int i in sel)
						 if (i < lvRuleList.Items.Count)
						 {
							 int n = i+adj;
							 if ((n >= 0) && (n < lvRuleList.Items.Count))
								 lvRuleList.Items[n].Selected = true;
						 }

				 FillPreview();

			 }

	private void bnEdit_Click(object sender, System.EventArgs e)
			 {

				 if (lvRuleList.SelectedItems.Count == 0)
					 return;
				 ShowRule sr = (ShowRule)(lvRuleList.SelectedItems[0].Tag);
				 AddModifyRule ar = new AddModifyRule(sr);
				 ar.ShowDialog(); // modifies rule in-place if OK'd
				 FillRuleList(false, 0);

			 }
	private void bnDelRule_Click(object sender, System.EventArgs e)
			 {

				 if (lvRuleList.SelectedItems.Count == 0)
					 return;
				 ShowRule sr = (ShowRule)(lvRuleList.SelectedItems[0].Tag);

				 WorkingRuleSet.Remove(sr);
				 FillRuleList(false, 0);

			 }

	private void bnRuleUp_Click(object sender, System.EventArgs e)
			 {
				 if (lvRuleList.SelectedIndices.Count != 1)
					 return;
				 int p = lvRuleList.SelectedIndices[0];
				 if (p <= 0)
					 return;

				 ShowRule sr = WorkingRuleSet[p];
				 WorkingRuleSet.RemoveAt(p);
				 WorkingRuleSet.Insert(p-1, sr);

				 FillRuleList(true, -1);

			 }
	private void bnRuleDown_Click(object sender, System.EventArgs e)
			 {
				 if (lvRuleList.SelectedIndices.Count != 1)
					 return;
				 int p = lvRuleList.SelectedIndices[0];
				 if (p >= (lvRuleList.Items.Count - 1))
					 return;

				 ShowRule sr = WorkingRuleSet[p];
				 WorkingRuleSet.RemoveAt(p);
				 WorkingRuleSet.Insert(p+1, sr);
				 FillRuleList(true, +1);
			 }


private void lvRuleList_DoubleClick(object sender, System.EventArgs e)
		 {
				 bnEdit_Click(null, null);
		 }
private void bnOK_Click(object sender, System.EventArgs e)
		 {
			 mSI.SeasonRules[mSeasonNumber] = WorkingRuleSet;
			 this.Close();
		 }
private void bnCancel_Click(object sender, System.EventArgs e)
		 {
			 this.Close();
		 }
		 private void FillPreview()
		 {
			 System.Collections.Generic.List<ProcessedEpisode > pel = new System.Collections.Generic.List<ProcessedEpisode >();

			 if (mOriginalEps != null)
			 {
				 foreach (ProcessedEpisode pe in mOriginalEps)
					 pel.Add(new ProcessedEpisode(pe));

				 TVDoc.ApplyRules(pel, WorkingRuleSet, mSI);
			 }

			 lbEpsPreview.BeginUpdate();
			 lbEpsPreview.Items.Clear();
			 foreach (ProcessedEpisode pe in pel)
				 lbEpsPreview.Items.Add(NameStyle.NameForExt(pe,null));
			 lbEpsPreview.EndUpdate();
		 }
}
}