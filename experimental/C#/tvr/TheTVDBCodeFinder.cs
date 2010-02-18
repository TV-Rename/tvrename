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
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;

namespace TVRename
{

	/// <summary>
	/// Summary for TheTVDBCodeFinder
	/// </summary>
	public class TheTVDBCodeFinder : System.Windows.Forms.UserControl
	{
		private TheTVDB mTVDB;
	private System.Windows.Forms.ColumnHeader columnHeader3;
			 private bool mInternal;

		public event EventHandler SelectionChanged;

		public TheTVDBCodeFinder(string initialHint, TheTVDB db)
		{
			mInternal = false;
			mTVDB = db;

			InitializeComponent();

			txtFindThis.Text = initialHint;
		}

		public void SetHint(string s)
		{
			mInternal = true;
			txtFindThis.Text = s;
			mInternal = false;
			DoFind(true);
		}

		public int SelectedCode()
		{
			try
			{
				if (lvMatches.SelectedItems.Count == 0)
					return int.Parse(txtFindThis.Text);

				return (int)(lvMatches.SelectedItems[0].Tag);
			}
			catch
			{
				return -1;
			}
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
	private System.Windows.Forms.Label txtSearchStatus;
	private System.Windows.Forms.Button bnGoSearch;
	private System.Windows.Forms.TextBox txtFindThis;
	private System.Windows.Forms.ListView lvMatches;
	private System.Windows.Forms.ColumnHeader columnHeader1;
	private System.Windows.Forms.ColumnHeader columnHeader2;
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
			this.txtSearchStatus = (new System.Windows.Forms.Label());
			this.bnGoSearch = (new System.Windows.Forms.Button());
			this.txtFindThis = (new System.Windows.Forms.TextBox());
			this.lvMatches = (new System.Windows.Forms.ListView());
			this.columnHeader1 = (new System.Windows.Forms.ColumnHeader());
			this.columnHeader2 = (new System.Windows.Forms.ColumnHeader());
			this.columnHeader3 = (new System.Windows.Forms.ColumnHeader());
			this.label3 = (new System.Windows.Forms.Label());
			this.SuspendLayout();
			// 
			// txtSearchStatus
			// 
			this.txtSearchStatus.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			this.txtSearchStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.txtSearchStatus.Location = new System.Drawing.Point(2, 153);
			this.txtSearchStatus.Name = "txtSearchStatus";
			this.txtSearchStatus.Size = new System.Drawing.Size(397, 15);
			this.txtSearchStatus.TabIndex = 9;
			this.txtSearchStatus.Text = "                    ";
			// 
			// bnGoSearch
			// 
			this.bnGoSearch.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
			this.bnGoSearch.Location = new System.Drawing.Point(324, -1);
			this.bnGoSearch.Name = "bnGoSearch";
			this.bnGoSearch.Size = new System.Drawing.Size(75, 23);
			this.bnGoSearch.TabIndex = 7;
			this.bnGoSearch.Text = "&Search";
			this.bnGoSearch.UseVisualStyleBackColor = true;
			this.bnGoSearch.Click += new System.EventHandler(bnGoSearch_Click);
			// 
			// txtFindThis
			// 
			this.txtFindThis.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			this.txtFindThis.Location = new System.Drawing.Point(90, 1);
			this.txtFindThis.Name = "txtFindThis";
			this.txtFindThis.Size = new System.Drawing.Size(228, 20);
			this.txtFindThis.TabIndex = 6;
			this.txtFindThis.TextChanged += new System.EventHandler(txtFindThis_TextChanged);
			this.txtFindThis.KeyDown += new System.Windows.Forms.KeyEventHandler(txtFindThis_KeyDown);
			// 
			// lvMatches
			// 
			this.lvMatches.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			this.lvMatches.Columns.AddRange(new System.Windows.Forms.ColumnHeader[3] {this.columnHeader1, this.columnHeader2, this.columnHeader3});
			this.lvMatches.FullRowSelect = true;
			this.lvMatches.HideSelection = false;
			this.lvMatches.Location = new System.Drawing.Point(1, 27);
			this.lvMatches.MultiSelect = false;
			this.lvMatches.Name = "lvMatches";
			this.lvMatches.ShowItemToolTips = true;
			this.lvMatches.Size = new System.Drawing.Size(397, 123);
			this.lvMatches.TabIndex = 8;
			this.lvMatches.UseCompatibleStateImageBehavior = false;
			this.lvMatches.View = System.Windows.Forms.View.Details;
			this.lvMatches.SelectedIndexChanged += new System.EventHandler(lvMatches_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Code";
			this.columnHeader1.Width = 44;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Show Name";
			this.columnHeader2.Width = 268;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Year";
			this.columnHeader3.Width = 49;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(-1, 4);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(85, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "TheTVDB &code:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// TheTVDBCodeFinder
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.txtSearchStatus);
			this.Controls.Add(this.bnGoSearch);
			this.Controls.Add(this.txtFindThis);
			this.Controls.Add(this.lvMatches);
			this.Controls.Add(this.label3);
			this.Name = "TheTVDBCodeFinder";
			this.Size = new System.Drawing.Size(403, 170);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
#endregion
	private void txtFindThis_TextChanged(object sender, System.EventArgs e)
			 {
				 if (!mInternal)
					 DoFind(false);
			 }
			 private void DoFind(bool chooseOnlyMatch)
			 {
				 if (mInternal)
					 return;

				 lvMatches.BeginUpdate();

				 string what = txtFindThis.Text;
				 what = what.Replace("."," ");

				 lvMatches.Items.Clear();
				 if (!string.IsNullOrEmpty(what))
				 {
					 what = what.ToLower();

					 bool numeric = Regex.Match(what,"^[0-9]+$").Success;
					 int matchnum = 0;
					 try
					 {
						 matchnum = numeric ? int.Parse(what) : -1;
					 }
					 catch (OverflowException )
					 {
					 }

					 mTVDB.GetLock("DoFind");
					 foreach (KeyValuePair<int, SeriesInfo> kvp in mTVDB.GetSeriesDict())
					 {
						 int num = kvp.Key;
						 string show = kvp.Value.Name;
						 string s = num.ToString() + " " + show;

						 string simpleS = Regex.Replace(s.ToLower(),"[^\\w ]","");

						 bool numberMatch = numeric && num == matchnum;

						 if (numberMatch || (!numeric && (simpleS.Contains(Regex.Replace(what,"[^\\w ]","")))) || (numeric && show.Contains(what)))
						 {
							 ListViewItem lvi = new ListViewItem();
							 lvi.Text = num.ToString();
							 lvi.SubItems.Add(show);
							 if (kvp.Value.FirstAired != null)
								lvi.SubItems.Add(kvp.Value.FirstAired.Year.ToString());
							 else
								 lvi.SubItems.Add("");

							 lvi.Tag = num;
							 if (numberMatch)
								 lvi.Selected = true;
							 lvMatches.Items.Add(lvi);
						 }
					 }
					 mTVDB.Unlock("DoFind");

					 if ((lvMatches.Items.Count == 1) && numeric)
						 lvMatches.Items[0].Selected = true;

					 int n = lvMatches.Items.Count;
					 txtSearchStatus.Text = "Found " + n + " show" + ((n!=1) ? "s":"");
				 }
				 else
					 txtSearchStatus.Text = "";

				 lvMatches.EndUpdate();

				 if ((lvMatches.Items.Count == 1) && chooseOnlyMatch)
					 lvMatches.Items[0].Selected = true;
			 }
	private void bnGoSearch_Click(object sender, System.EventArgs e)
			 {
				 // search on thetvdb.com site
				 txtSearchStatus.Text = "Searching on TheTVDB.com";
				 txtSearchStatus.Update();

				 //String ^url = "http://www.tv.com/search.php?stype=program&qs="+txtFindThis->Text+"&type=11&stype=search&tag=search%3Bbutton";

				 mTVDB.Search(txtFindThis.Text);

				 DoFind(true);
			 }
	private void lvMatches_SelectedIndexChanged(object sender, System.EventArgs e)
			 {
				 this.SelectionChanged(sender,e);
			 }
	private void txtFindThis_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
			 {
				 if ((e.KeyCode == Keys.Enter)||(e.KeyCode == Keys.Return))
				 {
					 bnGoSearch_Click(null, null);
					 e.Handled = true;
				 }
			 }
}
}