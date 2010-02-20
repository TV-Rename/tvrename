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
using System.Threading;
using System.Text.RegularExpressions;

namespace TVRename
{

	/// <summary>
	/// Summary for FolderMonitor
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public class FolderMonitor : System.Windows.Forms.Form
	{
		private TheTVDBCodeFinder mTCCF;
		private TVDoc mDoc;
		private int mInternalChange;

		public FolderMonitor(TVDoc doc)
		{
			mDoc = doc;
			mInternalChange = 0;

			InitializeComponent();


			mTCCF = new TheTVDBCodeFinder("", mDoc.GetTVDB(false,""));
			mTCCF.Dock = DockStyle.Fill;
			mTCCF.SelectionChanged += new System.EventHandler(lvMatches_ItemSelectionChanged);

			pnlCF.SuspendLayout();
			pnlCF.Controls.Add(mTCCF);
			pnlCF.ResumeLayout();

			FillFolderStringLists();
		}
		public string FMPUpto;
		public int FMPPercent;
		public bool FMPStopNow;
		public FolderMonitorProgress FMP;

		public void FMPShower()
		{
			FMP = new FolderMonitorProgress(this);
			FMP.ShowDialog();
			FMP = null;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
	private System.Windows.Forms.SplitContainer splitContainer3;
	private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	private System.Windows.Forms.Panel panel2;
	private System.Windows.Forms.Button bnFMCheck;
	private System.Windows.Forms.Button bnFMOpenMonFolder;
	private System.Windows.Forms.Button bnFMRemoveMonFolder;
	private System.Windows.Forms.Label label2;
	private System.Windows.Forms.ListBox lstFMMonitorFolders;
	private System.Windows.Forms.Button bnFMAddMonFolder;
	private System.Windows.Forms.Panel panel3;
	private System.Windows.Forms.Label label7;
	private System.Windows.Forms.Button bnFMOpenIgFolder;
	private System.Windows.Forms.Button bnFMAddIgFolder;
	private System.Windows.Forms.Button bnFMRemoveIgFolder;
	private System.Windows.Forms.ListBox lstFMIgnoreFolders;
	private System.Windows.Forms.TextBox txtFMSpecificSeason;
	private System.Windows.Forms.RadioButton rbSpecificSeason;
	private System.Windows.Forms.RadioButton rbFlat;
	private System.Windows.Forms.RadioButton rbFolderPerSeason;
	private System.Windows.Forms.Button bnFMVisitTVcom;
	private System.Windows.Forms.Panel pnlCF;
	private System.Windows.Forms.Button bnFMFullAuto;
	private System.Windows.Forms.ListView lvFMNewShows;
	private System.Windows.Forms.ColumnHeader columnHeader42;
	private System.Windows.Forms.ColumnHeader columnHeader43;
	private System.Windows.Forms.ColumnHeader columnHeader44;
	private System.Windows.Forms.ColumnHeader columnHeader45;
	private System.Windows.Forms.Button bnAddThisOne;
	private System.Windows.Forms.Button bnFolderMonitorDone;

	private System.Windows.Forms.Label label6;
	private System.Windows.Forms.Button bnFMRemoveNewFolder;
	private System.Windows.Forms.Button bnFMNewFolderOpen;
	private System.Windows.Forms.Button bnFMIgnoreAllNewFolders;
	private System.Windows.Forms.Button bnFMIgnoreNewFolder;
	private System.Windows.Forms.Button bnClose;
	private System.Windows.Forms.FolderBrowserDialog folderBrowser;

#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = (new System.ComponentModel.ComponentResourceManager(typeof(FolderMonitor)));
			this.splitContainer3 = (new System.Windows.Forms.SplitContainer());
			this.tableLayoutPanel1 = (new System.Windows.Forms.TableLayoutPanel());
			this.panel2 = (new System.Windows.Forms.Panel());
			this.bnFMCheck = (new System.Windows.Forms.Button());
			this.bnFMOpenMonFolder = (new System.Windows.Forms.Button());
			this.bnFMRemoveMonFolder = (new System.Windows.Forms.Button());
			this.label2 = (new System.Windows.Forms.Label());
			this.lstFMMonitorFolders = (new System.Windows.Forms.ListBox());
			this.bnFMAddMonFolder = (new System.Windows.Forms.Button());
			this.panel3 = (new System.Windows.Forms.Panel());
			this.label7 = (new System.Windows.Forms.Label());
			this.bnFMOpenIgFolder = (new System.Windows.Forms.Button());
			this.bnFMAddIgFolder = (new System.Windows.Forms.Button());
			this.bnFMRemoveIgFolder = (new System.Windows.Forms.Button());
			this.lstFMIgnoreFolders = (new System.Windows.Forms.ListBox());
			this.bnClose = (new System.Windows.Forms.Button());
			this.txtFMSpecificSeason = (new System.Windows.Forms.TextBox());
			this.rbSpecificSeason = (new System.Windows.Forms.RadioButton());
			this.rbFlat = (new System.Windows.Forms.RadioButton());
			this.rbFolderPerSeason = (new System.Windows.Forms.RadioButton());
			this.bnFMVisitTVcom = (new System.Windows.Forms.Button());
			this.pnlCF = (new System.Windows.Forms.Panel());
			this.bnFMFullAuto = (new System.Windows.Forms.Button());
			this.lvFMNewShows = (new System.Windows.Forms.ListView());
			this.columnHeader42 = (new System.Windows.Forms.ColumnHeader());
			this.columnHeader43 = (new System.Windows.Forms.ColumnHeader());
			this.columnHeader44 = (new System.Windows.Forms.ColumnHeader());
			this.columnHeader45 = (new System.Windows.Forms.ColumnHeader());
			this.bnAddThisOne = (new System.Windows.Forms.Button());
			this.bnFolderMonitorDone = (new System.Windows.Forms.Button());
			this.label6 = (new System.Windows.Forms.Label());
			this.bnFMRemoveNewFolder = (new System.Windows.Forms.Button());
			this.bnFMNewFolderOpen = (new System.Windows.Forms.Button());
			this.bnFMIgnoreAllNewFolders = (new System.Windows.Forms.Button());
			this.bnFMIgnoreNewFolder = (new System.Windows.Forms.Button());
			this.folderBrowser = (new System.Windows.Forms.FolderBrowserDialog());
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.tableLayoutPanel1);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.bnClose);
			this.splitContainer3.Panel2.Controls.Add(this.txtFMSpecificSeason);
			this.splitContainer3.Panel2.Controls.Add(this.rbSpecificSeason);
			this.splitContainer3.Panel2.Controls.Add(this.rbFlat);
			this.splitContainer3.Panel2.Controls.Add(this.rbFolderPerSeason);
			this.splitContainer3.Panel2.Controls.Add(this.bnFMVisitTVcom);
			this.splitContainer3.Panel2.Controls.Add(this.pnlCF);
			this.splitContainer3.Panel2.Controls.Add(this.bnFMFullAuto);
			this.splitContainer3.Panel2.Controls.Add(this.lvFMNewShows);
			this.splitContainer3.Panel2.Controls.Add(this.bnAddThisOne);
			this.splitContainer3.Panel2.Controls.Add(this.bnFolderMonitorDone);
			this.splitContainer3.Panel2.Controls.Add(this.label6);
			this.splitContainer3.Panel2.Controls.Add(this.bnFMRemoveNewFolder);
			this.splitContainer3.Panel2.Controls.Add(this.bnFMNewFolderOpen);
			this.splitContainer3.Panel2.Controls.Add(this.bnFMIgnoreAllNewFolders);
			this.splitContainer3.Panel2.Controls.Add(this.bnFMIgnoreNewFolder);
			this.splitContainer3.Size = new System.Drawing.Size(887, 634);
			this.splitContainer3.SplitterDistance = 190;
			this.splitContainer3.SplitterWidth = 5;
			this.splitContainer3.TabIndex = 12;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add((new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50)));
			this.tableLayoutPanel1.ColumnStyles.Add((new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50)));
			this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel3, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add((new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100)));
			this.tableLayoutPanel1.RowStyles.Add((new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 190)));
			this.tableLayoutPanel1.RowStyles.Add((new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 190)));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(887, 190);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.bnFMCheck);
			this.panel2.Controls.Add(this.bnFMOpenMonFolder);
			this.panel2.Controls.Add(this.bnFMRemoveMonFolder);
			this.panel2.Controls.Add(this.label2);
			this.panel2.Controls.Add(this.lstFMMonitorFolders);
			this.panel2.Controls.Add(this.bnFMAddMonFolder);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(3, 3);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(437, 184);
			this.panel2.TabIndex = 0;
			// 
			// bnFMCheck
			// 
			this.bnFMCheck.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
			this.bnFMCheck.Location = new System.Drawing.Point(359, 158);
			this.bnFMCheck.Name = "bnFMCheck";
			this.bnFMCheck.Size = new System.Drawing.Size(75, 23);
			this.bnFMCheck.TabIndex = 10;
			this.bnFMCheck.Text = "&Check";
			this.bnFMCheck.UseVisualStyleBackColor = true;
			this.bnFMCheck.Click += new System.EventHandler(bnFMCheck_Click);
			// 
			// bnFMOpenMonFolder
			// 
			this.bnFMOpenMonFolder.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.bnFMOpenMonFolder.Location = new System.Drawing.Point(165, 158);
			this.bnFMOpenMonFolder.Name = "bnFMOpenMonFolder";
			this.bnFMOpenMonFolder.Size = new System.Drawing.Size(75, 23);
			this.bnFMOpenMonFolder.TabIndex = 9;
			this.bnFMOpenMonFolder.Text = "&Open";
			this.bnFMOpenMonFolder.UseVisualStyleBackColor = true;
			this.bnFMOpenMonFolder.Click += new System.EventHandler(bnFMOpenMonFolder_Click);
			// 
			// bnFMRemoveMonFolder
			// 
			this.bnFMRemoveMonFolder.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.bnFMRemoveMonFolder.Location = new System.Drawing.Point(84, 158);
			this.bnFMRemoveMonFolder.Name = "bnFMRemoveMonFolder";
			this.bnFMRemoveMonFolder.Size = new System.Drawing.Size(75, 23);
			this.bnFMRemoveMonFolder.TabIndex = 8;
			this.bnFMRemoveMonFolder.Text = "&Remove";
			this.bnFMRemoveMonFolder.UseVisualStyleBackColor = true;
			this.bnFMRemoveMonFolder.Click += new System.EventHandler(bnFMRemoveMonFolder_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(0, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(82, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "&Monitor Folders:";
			// 
			// lstFMMonitorFolders
			// 
			this.lstFMMonitorFolders.AllowDrop = true;
			this.lstFMMonitorFolders.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			this.lstFMMonitorFolders.FormattingEnabled = true;
			this.lstFMMonitorFolders.IntegralHeight = false;
			this.lstFMMonitorFolders.Location = new System.Drawing.Point(3, 16);
			this.lstFMMonitorFolders.Name = "lstFMMonitorFolders";
			this.lstFMMonitorFolders.ScrollAlwaysVisible = true;
			this.lstFMMonitorFolders.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lstFMMonitorFolders.Size = new System.Drawing.Size(431, 136);
			this.lstFMMonitorFolders.TabIndex = 6;
			// 
			// bnFMAddMonFolder
			// 
			this.bnFMAddMonFolder.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.bnFMAddMonFolder.Location = new System.Drawing.Point(3, 158);
			this.bnFMAddMonFolder.Name = "bnFMAddMonFolder";
			this.bnFMAddMonFolder.Size = new System.Drawing.Size(75, 23);
			this.bnFMAddMonFolder.TabIndex = 7;
			this.bnFMAddMonFolder.Text = "&Add";
			this.bnFMAddMonFolder.UseVisualStyleBackColor = true;
			this.bnFMAddMonFolder.Click += new System.EventHandler(bnFMAddMonFolder_Click);
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.label7);
			this.panel3.Controls.Add(this.bnFMOpenIgFolder);
			this.panel3.Controls.Add(this.bnFMAddIgFolder);
			this.panel3.Controls.Add(this.bnFMRemoveIgFolder);
			this.panel3.Controls.Add(this.lstFMIgnoreFolders);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(446, 3);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(438, 184);
			this.panel3.TabIndex = 0;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(3, 0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(77, 13);
			this.label7.TabIndex = 5;
			this.label7.Text = "&Ignore Folders:";
			// 
			// bnFMOpenIgFolder
			// 
			this.bnFMOpenIgFolder.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.bnFMOpenIgFolder.Location = new System.Drawing.Point(165, 158);
			this.bnFMOpenIgFolder.Name = "bnFMOpenIgFolder";
			this.bnFMOpenIgFolder.Size = new System.Drawing.Size(75, 23);
			this.bnFMOpenIgFolder.TabIndex = 9;
			this.bnFMOpenIgFolder.Text = "O&pen";
			this.bnFMOpenIgFolder.UseVisualStyleBackColor = true;
			this.bnFMOpenIgFolder.Click += new System.EventHandler(bnFMOpenIgFolder_Click);
			// 
			// bnFMAddIgFolder
			// 
			this.bnFMAddIgFolder.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.bnFMAddIgFolder.Location = new System.Drawing.Point(2, 158);
			this.bnFMAddIgFolder.Name = "bnFMAddIgFolder";
			this.bnFMAddIgFolder.Size = new System.Drawing.Size(75, 23);
			this.bnFMAddIgFolder.TabIndex = 7;
			this.bnFMAddIgFolder.Text = "A&dd";
			this.bnFMAddIgFolder.UseVisualStyleBackColor = true;
			this.bnFMAddIgFolder.Click += new System.EventHandler(bnFMAddIgFolder_Click);
			// 
			// bnFMRemoveIgFolder
			// 
			this.bnFMRemoveIgFolder.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.bnFMRemoveIgFolder.Location = new System.Drawing.Point(84, 158);
			this.bnFMRemoveIgFolder.Name = "bnFMRemoveIgFolder";
			this.bnFMRemoveIgFolder.Size = new System.Drawing.Size(75, 23);
			this.bnFMRemoveIgFolder.TabIndex = 8;
			this.bnFMRemoveIgFolder.Text = "Remo&ve";
			this.bnFMRemoveIgFolder.UseVisualStyleBackColor = true;
			this.bnFMRemoveIgFolder.Click += new System.EventHandler(bnFMRemoveIgFolder_Click);
			// 
			// lstFMIgnoreFolders
			// 
			this.lstFMIgnoreFolders.AllowDrop = true;
			this.lstFMIgnoreFolders.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			this.lstFMIgnoreFolders.FormattingEnabled = true;
			this.lstFMIgnoreFolders.IntegralHeight = false;
			this.lstFMIgnoreFolders.Location = new System.Drawing.Point(0, 16);
			this.lstFMIgnoreFolders.Name = "lstFMIgnoreFolders";
			this.lstFMIgnoreFolders.ScrollAlwaysVisible = true;
			this.lstFMIgnoreFolders.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lstFMIgnoreFolders.Size = new System.Drawing.Size(438, 136);
			this.lstFMIgnoreFolders.TabIndex = 6;
			// 
			// bnClose
			// 
			this.bnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bnClose.Location = new System.Drawing.Point(809, 413);
			this.bnClose.Name = "bnClose";
			this.bnClose.Size = new System.Drawing.Size(75, 23);
			this.bnClose.TabIndex = 30;
			this.bnClose.Text = "Close";
			this.bnClose.UseVisualStyleBackColor = true;
			this.bnClose.Click += new System.EventHandler(bnClose_Click);
			// 
			// txtFMSpecificSeason
			// 
			this.txtFMSpecificSeason.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.txtFMSpecificSeason.Location = new System.Drawing.Point(533, 235);
			this.txtFMSpecificSeason.Name = "txtFMSpecificSeason";
			this.txtFMSpecificSeason.Size = new System.Drawing.Size(53, 20);
			this.txtFMSpecificSeason.TabIndex = 29;
			// 
			// rbSpecificSeason
			// 
			this.rbSpecificSeason.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.rbSpecificSeason.AutoSize = true;
			this.rbSpecificSeason.Location = new System.Drawing.Point(433, 236);
			this.rbSpecificSeason.Name = "rbSpecificSeason";
			this.rbSpecificSeason.Size = new System.Drawing.Size(100, 17);
			this.rbSpecificSeason.TabIndex = 28;
			this.rbSpecificSeason.TabStop = true;
			this.rbSpecificSeason.Text = "Specific season";
			this.rbSpecificSeason.UseVisualStyleBackColor = true;
			this.rbSpecificSeason.CheckedChanged += new System.EventHandler(rbSpecificSeason_CheckedChanged);
			// 
			// rbFlat
			// 
			this.rbFlat.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.rbFlat.AutoSize = true;
			this.rbFlat.Location = new System.Drawing.Point(433, 213);
			this.rbFlat.Name = "rbFlat";
			this.rbFlat.Size = new System.Drawing.Size(120, 17);
			this.rbFlat.TabIndex = 28;
			this.rbFlat.TabStop = true;
			this.rbFlat.Text = "All seasons together";
			this.rbFlat.UseVisualStyleBackColor = true;
			this.rbFlat.CheckedChanged += new System.EventHandler(rbFlat_CheckedChanged);
			// 
			// rbFolderPerSeason
			// 
			this.rbFolderPerSeason.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.rbFolderPerSeason.AutoSize = true;
			this.rbFolderPerSeason.Location = new System.Drawing.Point(433, 190);
			this.rbFolderPerSeason.Name = "rbFolderPerSeason";
			this.rbFolderPerSeason.Size = new System.Drawing.Size(109, 17);
			this.rbFolderPerSeason.TabIndex = 28;
			this.rbFolderPerSeason.TabStop = true;
			this.rbFolderPerSeason.Text = "Folder per season";
			this.rbFolderPerSeason.UseVisualStyleBackColor = true;
			this.rbFolderPerSeason.CheckedChanged += new System.EventHandler(rbFolderPerSeason_CheckedChanged);
			// 
			// bnFMVisitTVcom
			// 
			this.bnFMVisitTVcom.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.bnFMVisitTVcom.Location = new System.Drawing.Point(332, 383);
			this.bnFMVisitTVcom.Name = "bnFMVisitTVcom";
			this.bnFMVisitTVcom.Size = new System.Drawing.Size(75, 23);
			this.bnFMVisitTVcom.TabIndex = 26;
			this.bnFMVisitTVcom.Text = "&Visit TVDB";
			this.bnFMVisitTVcom.UseVisualStyleBackColor = true;
			this.bnFMVisitTVcom.Click += new System.EventHandler(bnFMVisitTVcom_Click);
			// 
			// pnlCF
			// 
			this.pnlCF.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.pnlCF.Location = new System.Drawing.Point(5, 190);
			this.pnlCF.Name = "pnlCF";
			this.pnlCF.Size = new System.Drawing.Size(407, 185);
			this.pnlCF.TabIndex = 25;
			// 
			// bnFMFullAuto
			// 
			this.bnFMFullAuto.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.bnFMFullAuto.Location = new System.Drawing.Point(8, 413);
			this.bnFMFullAuto.Name = "bnFMFullAuto";
			this.bnFMFullAuto.Size = new System.Drawing.Size(75, 23);
			this.bnFMFullAuto.TabIndex = 24;
			this.bnFMFullAuto.Text = "F&ull Auto";
			this.bnFMFullAuto.UseVisualStyleBackColor = true;
			this.bnFMFullAuto.Click += new System.EventHandler(bnFMFullAuto_Click);
			// 
			// lvFMNewShows
			// 
			this.lvFMNewShows.AllowDrop = true;
			this.lvFMNewShows.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			this.lvFMNewShows.Columns.AddRange(new System.Windows.Forms.ColumnHeader[4] {this.columnHeader42, this.columnHeader43, this.columnHeader44, this.columnHeader45});
			this.lvFMNewShows.FullRowSelect = true;
			this.lvFMNewShows.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lvFMNewShows.HideSelection = false;
			this.lvFMNewShows.Location = new System.Drawing.Point(6, 23);
			this.lvFMNewShows.Name = "lvFMNewShows";
			this.lvFMNewShows.Size = new System.Drawing.Size(881, 161);
			this.lvFMNewShows.TabIndex = 11;
			this.lvFMNewShows.UseCompatibleStateImageBehavior = false;
			this.lvFMNewShows.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader42
			// 
			this.columnHeader42.Text = "Folder";
			this.columnHeader42.Width = 240;
			// 
			// columnHeader43
			// 
			this.columnHeader43.Text = "Show";
			this.columnHeader43.Width = 139;
			// 
			// columnHeader44
			// 
			this.columnHeader44.Text = "Season";
			// 
			// columnHeader45
			// 
			this.columnHeader45.Text = "thetvdb code";
			this.columnHeader45.Width = 94;
			// 
			// bnAddThisOne
			// 
			this.bnAddThisOne.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.bnAddThisOne.Location = new System.Drawing.Point(433, 383);
			this.bnAddThisOne.Name = "bnAddThisOne";
			this.bnAddThisOne.Size = new System.Drawing.Size(75, 23);
			this.bnAddThisOne.TabIndex = 10;
			this.bnAddThisOne.Text = "Add &This";
			this.bnAddThisOne.UseVisualStyleBackColor = true;
			this.bnAddThisOne.Click += new System.EventHandler(bnAddThisOne_Click);
			// 
			// bnFolderMonitorDone
			// 
			this.bnFolderMonitorDone.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.bnFolderMonitorDone.Location = new System.Drawing.Point(433, 413);
			this.bnFolderMonitorDone.Name = "bnFolderMonitorDone";
			this.bnFolderMonitorDone.Size = new System.Drawing.Size(75, 23);
			this.bnFolderMonitorDone.TabIndex = 10;
			this.bnFolderMonitorDone.Text = "Do&ne";
			this.bnFolderMonitorDone.UseVisualStyleBackColor = true;
			this.bnFolderMonitorDone.Click += new System.EventHandler(bnFolderMonitorDone_Click);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(3, 5);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(64, 13);
			this.label6.TabIndex = 5;
			this.label6.Text = "&New Shows";
			// 
			// bnFMRemoveNewFolder
			// 
			this.bnFMRemoveNewFolder.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.bnFMRemoveNewFolder.Location = new System.Drawing.Point(89, 414);
			this.bnFMRemoveNewFolder.Name = "bnFMRemoveNewFolder";
			this.bnFMRemoveNewFolder.Size = new System.Drawing.Size(75, 23);
			this.bnFMRemoveNewFolder.TabIndex = 9;
			this.bnFMRemoveNewFolder.Text = "Re&move";
			this.bnFMRemoveNewFolder.UseVisualStyleBackColor = true;
			this.bnFMRemoveNewFolder.Click += new System.EventHandler(bnFMRemoveNewFolder_Click);
			// 
			// bnFMNewFolderOpen
			// 
			this.bnFMNewFolderOpen.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.bnFMNewFolderOpen.Location = new System.Drawing.Point(332, 414);
			this.bnFMNewFolderOpen.Name = "bnFMNewFolderOpen";
			this.bnFMNewFolderOpen.Size = new System.Drawing.Size(75, 23);
			this.bnFMNewFolderOpen.TabIndex = 9;
			this.bnFMNewFolderOpen.Text = "Op&en";
			this.bnFMNewFolderOpen.UseVisualStyleBackColor = true;
			this.bnFMNewFolderOpen.Click += new System.EventHandler(bnFMNewFolderOpen_Click);
			// 
			// bnFMIgnoreAllNewFolders
			// 
			this.bnFMIgnoreAllNewFolders.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.bnFMIgnoreAllNewFolders.Location = new System.Drawing.Point(251, 414);
			this.bnFMIgnoreAllNewFolders.Name = "bnFMIgnoreAllNewFolders";
			this.bnFMIgnoreAllNewFolders.Size = new System.Drawing.Size(75, 23);
			this.bnFMIgnoreAllNewFolders.TabIndex = 9;
			this.bnFMIgnoreAllNewFolders.Text = "Ig&nore All";
			this.bnFMIgnoreAllNewFolders.UseVisualStyleBackColor = true;
			this.bnFMIgnoreAllNewFolders.Click += new System.EventHandler(bnFMIgnoreAllNewFolders_Click);
			// 
			// bnFMIgnoreNewFolder
			// 
			this.bnFMIgnoreNewFolder.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.bnFMIgnoreNewFolder.Location = new System.Drawing.Point(170, 414);
			this.bnFMIgnoreNewFolder.Name = "bnFMIgnoreNewFolder";
			this.bnFMIgnoreNewFolder.Size = new System.Drawing.Size(75, 23);
			this.bnFMIgnoreNewFolder.TabIndex = 9;
			this.bnFMIgnoreNewFolder.Text = "&Ignore";
			this.bnFMIgnoreNewFolder.UseVisualStyleBackColor = true;
			this.bnFMIgnoreNewFolder.Click += new System.EventHandler(bnFMIgnoreNewFolder_Click);
			// 
			// folderBrowser
			// 
			this.folderBrowser.ShowNewFolderButton = false;
			// 
			// FolderMonitor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.bnClose;
			this.ClientSize = new System.Drawing.Size(887, 634);
			this.Controls.Add(this.splitContainer3);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FolderMonitor";
			this.ShowInTaskbar = false;
			this.Text = "TVRename Folder Monitor";
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			this.splitContainer3.Panel2.PerformLayout();
			this.splitContainer3.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.ResumeLayout(false);

		}
#endregion
	private void bnClose_Click(object sender, System.EventArgs e)
			 {
				 this.Close();
			 }



			 private void FillFolderStringLists()
			 {
				 lstFMIgnoreFolders.BeginUpdate();
				 lstFMMonitorFolders.BeginUpdate();

				 lstFMIgnoreFolders.Items.Clear();
				 lstFMMonitorFolders.Items.Clear();

				 mDoc.MonitorFolders.Sort();
				 mDoc.IgnoreFolders.Sort();

				 foreach (string folder in mDoc.MonitorFolders)
					 lstFMMonitorFolders.Items.Add(folder);

				 foreach (string folder in mDoc.IgnoreFolders)
					 lstFMIgnoreFolders.Items.Add(folder);

				 lstFMIgnoreFolders.EndUpdate();
				 lstFMMonitorFolders.EndUpdate();
			 }

			 private void bnFMRemoveMonFolder_Click(object sender, System.EventArgs e)
			 {
				 for (int i =lstFMMonitorFolders.SelectedIndices.Count-1;i>=0;i--)
				 {
					 int n = lstFMMonitorFolders.SelectedIndices[i];
					 mDoc.MonitorFolders.RemoveAt(n);
				 }
				 mDoc.SetDirty();
				 FillFolderStringLists();
			 }

			 private void bnFMRemoveIgFolder_Click(object sender, System.EventArgs e)
			 {
				 for (int i =lstFMIgnoreFolders.SelectedIndices.Count-1;i>=0;i--)
				 {
					 int n = lstFMIgnoreFolders.SelectedIndices[i];
					 mDoc.IgnoreFolders.RemoveAt(n);
				 }
				 mDoc.SetDirty();
				 FillFolderStringLists();
			 }
			 private void bnFMAddMonFolder_Click(object sender, System.EventArgs e)
			 {
				 folderBrowser.SelectedPath = "";
				 if (lstFMMonitorFolders.SelectedIndex != -1)
				 {
					 int n = lstFMMonitorFolders.SelectedIndex;
					 folderBrowser.SelectedPath = mDoc.MonitorFolders[n];
				 }

				 if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				 {
					 mDoc.MonitorFolders.Add(folderBrowser.SelectedPath.ToLower());
					 mDoc.SetDirty();
					 FillFolderStringLists();
				 }
			 }
			 private void bnFMAddIgFolder_Click(object sender, System.EventArgs e)
			 {
				 folderBrowser.SelectedPath = "";
				 if (lstFMIgnoreFolders.SelectedIndex != -1)
					 folderBrowser.SelectedPath = mDoc.IgnoreFolders[lstFMIgnoreFolders.SelectedIndex];

				 if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				 {
					 mDoc.IgnoreFolders.Add(folderBrowser.SelectedPath.ToLower());
					 mDoc.SetDirty();
					 FillFolderStringLists();
				 }
			 }
			 private void bnFMOpenMonFolder_Click(object sender, System.EventArgs e)
			 {
				 if (lstFMMonitorFolders.SelectedIndex != -1)
					 TVDoc.SysOpen(mDoc.MonitorFolders[lstFMMonitorFolders.SelectedIndex]);
			 }
			 private void bnFMOpenIgFolder_Click(object sender, System.EventArgs e)
			 {
				 if (lstFMIgnoreFolders.SelectedIndex != -1)
					 TVDoc.SysOpen(mDoc.MonitorFolders[lstFMIgnoreFolders.SelectedIndex]);
			 }
			 private void lstFMMonitorFolders_DoubleClick(object sender, System.EventArgs e)
			 {
				 bnFMOpenMonFolder_Click(null, null);
			 }
			 private void lstFMIgnoreFolders_DoubleClick(object sender, System.EventArgs e)
			 {
				 bnFMOpenIgFolder_Click(null, null);
			 }
			 private void bnFMCheck_Click(object sender, System.EventArgs e)
			 {
				 mDoc.CheckMonitoredFolders();
				 GuessAll();
				 FillFMNewShowList(false);
				 //FillFMTVcomListCombo();
			 }

			 private void lstFMMonitorFolders_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
			 {
				 if (!e.Data.GetDataPresent(DataFormats.FileDrop))
					 e.Effect = DragDropEffects.None;
				 else
					 e.Effect = DragDropEffects.Copy;
			 }
			 private void lstFMIgnoreFolders_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
			 {
				 if (!e.Data.GetDataPresent(DataFormats.FileDrop))
					 e.Effect = DragDropEffects.None;
				 else
					 e.Effect = DragDropEffects.Copy;
			 }
			 private void lstFMMonitorFolders_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
			 {
				 string[] files = (string[])(e.Data.GetData(DataFormats.FileDrop));
				 for (int i =0;i<files.Length;i++)
				 {
					 string path = files[i];
					 DirectoryInfo di;
					 try
					 {
						 di = new DirectoryInfo(path);
						 if (di.Exists)
							 mDoc.MonitorFolders.Add(path.ToLower());
					 }
					 catch
					 {
					 }
				 }
				 mDoc.SetDirty();
				 FillFolderStringLists();
			 }
			 private void lstFMIgnoreFolders_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
			 {
				 string[] files = (string[])(e.Data.GetData(DataFormats.FileDrop));
				 for (int i =0;i<files.Length;i++)
				 {
					 string path = files[i];
					 DirectoryInfo di;
					 try
					 {
						 di = new DirectoryInfo(path);
						 if (di.Exists)
							 mDoc.IgnoreFolders.Add(path.ToLower());
					 }
					 catch
					 {
					 }
				 }
				 mDoc.SetDirty();
				 FillFolderStringLists();
			 }
			 private void lstFMMonitorFolders_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
			 {
				 if (e.KeyCode == Keys.Delete)
					 bnFMRemoveMonFolder_Click(null, null);
			 }
			 private void lstFMIgnoreFolders_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
			 {
				 if (e.KeyCode == Keys.Delete)
					 bnFMRemoveIgFolder_Click(null, null);
			 }

	private void bnFMFullAuto_Click(object sender, System.EventArgs e)
			 {
				 FMPStopNow = false;
				 FMPUpto = "";
				 FMPPercent = 0;

				 Thread fmpshower = new Thread(new ThreadStart(this.FMPShower));
				 fmpshower.Name = "Folder Monitor Progress";
				 fmpshower.Start();

				 int n = 0;
				 int n2 = mDoc.AddItems.Count;

				 foreach (AddItem ai in mDoc.AddItems)
				 {
					 if (ai.TheSeries == null)
					 {
						 // do search using folder name
						 TVDoc.GuessShowName(ai);
						 if (!string.IsNullOrEmpty(ai.ShowName))
						 {
							 FMPUpto = ai.ShowName;
							 mDoc.GetTVDB(false,"").Search(ai.ShowName);
							 GuessAI(ai);
							 UpdateFMListItem(ai, true);
							 lvFMNewShows.Update();
						 }
					 }
					 FMPPercent = (100*(n+(n2/2)))/n2;
					 n++;
					 if (FMPStopNow)
						 break;
				 }
				 FMPStopNow = true;
			 }

			 private void bnFMRemoveNewFolder_Click(object sender, System.EventArgs e)
			 {
				 if (lvFMNewShows.SelectedItems.Count == 0)
					 return;
				 foreach (ListViewItem lvi in lvFMNewShows.SelectedItems)
				 {
					 AddItem ai = (AddItem)(lvi.Tag);
					 mDoc.AddItems.Remove(ai);
				 }
				 FillFMNewShowList(false);
			 }
			 private void bnFMIgnoreNewFolder_Click(object sender, System.EventArgs e)
			 {
				 if (lvFMNewShows.SelectedItems.Count == 0)
					 return;
				 foreach (ListViewItem lvi in lvFMNewShows.SelectedItems)
				 {
					 AddItem ai = (AddItem)(lvi.Tag);
					 mDoc.IgnoreFolders.Add(ai.Folder.ToLower());
					 mDoc.AddItems.Remove(ai);
				 }
				 mDoc.SetDirty();
				 FillFMNewShowList(false);
				 FillFolderStringLists();
			 }


			 private void bnFMIgnoreAllNewFolders_Click(object sender, System.EventArgs e)
			 {
				 System.Windows.Forms.DialogResult dr = MessageBox.Show("Add everything in this list to the ignore list?", "Ignore All", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);

				 if (dr != System.Windows.Forms.DialogResult.OK)
					 return;

				 foreach (AddItem ai in mDoc.AddItems)
					 mDoc.IgnoreFolders.Add(ai.Folder.ToLower());

				 mDoc.AddItems.Clear();
				 mDoc.SetDirty();
				 FillFolderStringLists();
				 FillFMNewShowList(false);
			 }



			 private void lvFMNewShows_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
			 {
//                 
//				 if (e->Button != System::Windows::Forms::MouseButtons::Right)
//				 return;
//
//				 lstFMNewFolders->ClearSelected();
//				 lstFMNewFolders->SelectedIndex = lstFMNewFolders->IndexFromPoint(Point(e->X,e->Y));
//
//				 int p;
//				 if ((p = lstFMNewFolders->SelectedIndex) == -1)
//				 return;
//
//				 Point^ pt = lstFMNewFolders->PointToScreen(Point(e->X, e->Y));
//				 RightClickOnFolder(lstFMNewFolders->Items[p]->ToString(),pt);
//
//				 
			 }
			 private void lvFMNewShows_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
			 {
				 if (!e.Data.GetDataPresent(DataFormats.FileDrop))
					 e.Effect = DragDropEffects.None;
				 else
					 e.Effect = DragDropEffects.Copy;
			 }
			 private void lvFMNewShows_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
			 {
				 string[] files = (string[])(e.Data.GetData(DataFormats.FileDrop));
				 for (int i =0;i<files.Length;i++)
				 {
					 string path = files[i];
					 DirectoryInfo di;
					 try
					 {
						 di = new DirectoryInfo(path);
						 if (di.Exists)
						 {
							 // keep next line sync'd with ProcessAddItems, etc.
							 bool hasSeasonFolders = Directory.GetDirectories(path, "*Season *").Length > 0; // todo - use non specific word
							 AddItem ai = new AddItem(path, hasSeasonFolders ? FolderModeEnum.kfmFolderPerSeason : FolderModeEnum.kfmFlat, -1);
							 GuessAI(ai);
							 mDoc.AddItems.Add(ai);
						 }
					 }
					 catch
					 {
					 }
				 }
				 FillFMNewShowList(true);
			 }
			 private void lvFMNewShows_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
			 {
				 if (e.KeyCode == Keys.Delete)
					 bnFMRemoveNewFolder_Click(null, null);
			 }
			 private void bnFMNewFolderOpen_Click(object sender, System.EventArgs e)
			 {
				 if (lvFMNewShows.SelectedItems.Count == 0)
					 return;
				 AddItem ai = (AddItem)(lvFMNewShows.SelectedItems[0].Tag);
				 TVDoc.SysOpen(ai.Folder);
			 }
			 private void lvFMNewShows_DoubleClick(object sender, System.EventArgs e)
			 {
				 bnFMNewFolderOpen_Click(null, null);
			 }




			 private void lvMatches_ItemSelectionChanged(object sender, System.EventArgs e)
			 {
				 if (mInternalChange != 0)
					 return;

				 int code = mTCCF.SelectedCode();

				 SeriesInfo ser = mDoc.GetTVDB(false,"").GetSeries(code);
				 if (ser == null)
					 return;

				 foreach (ListViewItem lvi in lvFMNewShows.SelectedItems)
				 {
					 AddItem ai = (AddItem)(lvi.Tag);
					 ai.TheSeries = ser;
					 UpdateFMListItem(ai, false);
				 }
			 }

			 private void lvFMNewShows_SelectedIndexChanged(object sender, System.EventArgs e)
			 {
				 if (lvFMNewShows.SelectedItems.Count == 0)
					 return;
				 AddItem ai = (AddItem)(lvFMNewShows.SelectedItems[0].Tag);
				 //txtTVComCode->Text = ai->TVcomCode == -1 ? "" : ai->TVcomCode.ToString();
				 //txtShowName->Text = ai->Show;
				 mInternalChange++;
				 mTCCF.SetHint(ai.TheSeries != null ? ai.TheSeries.TVDBCode.ToString() : ai.ShowName);

				 if (ai.FolderMode == FolderModeEnum.kfmFlat)
					 rbFlat.Checked = true;
				 else if (ai.FolderMode == FolderModeEnum.kfmSpecificSeason)
				 {
					 rbSpecificSeason.Checked = true;
					 txtFMSpecificSeason.Text = ai.SpecificSeason.ToString();
				 }
				 else
					 rbFolderPerSeason.Checked = true;
				 rbSpecificSeason_CheckedChanged(null, null);

				 mInternalChange--;
			 }
			 private void FillFMNewShowList(bool keepSel)
			 {
                 System.Collections.Generic.List<int> sel = new System.Collections.Generic.List<int>();
				 if (keepSel)
					 foreach (int i in lvFMNewShows.SelectedIndices)
						 sel.Add(i);

				 lvFMNewShows.BeginUpdate();
				 lvFMNewShows.Items.Clear();

				 foreach (AddItem ai in mDoc.AddItems)
				 {
					 ListViewItem lvi = new ListViewItem();
					 FMLVISet(ai, lvi);
					 lvFMNewShows.Items.Add(lvi);
				 }

				 if (keepSel)
					 foreach (int i in sel)
						 if (i < lvFMNewShows.Items.Count)
							 lvFMNewShows.Items[i].Selected = true;

				 lvFMNewShows.EndUpdate();
			 }
			 private static void FMLVISet(AddItem ai, ListViewItem lvi)
			 {
				 lvi.SubItems.Clear();
				 lvi.Text = ai.Folder;
				 lvi.SubItems.Add(ai.TheSeries != null ? ai.TheSeries.Name : "");
				 string fmode = "-";
				 if (ai.FolderMode == FolderModeEnum.kfmFolderPerSeason)
					 fmode = "Per Seas";
				 else if (ai.FolderMode == FolderModeEnum.kfmFlat)
					 fmode = "Flat";
				 else if (ai.FolderMode == FolderModeEnum.kfmSpecificSeason)
					 fmode = ai.SpecificSeason.ToString();
				 lvi.SubItems.Add(fmode);
				 lvi.SubItems.Add(ai.TheSeries != null ? ai.TheSeries.TVDBCode.ToString() : "");
				 lvi.Tag = ai;
			 }
			 private void UpdateFMListItem(AddItem ai, bool makevis)
			 {
				 foreach (ListViewItem lvi in lvFMNewShows.Items)
				 {
					 if (lvi.Tag == ai)
					 {
						 FMLVISet(ai, lvi);
						 if (makevis)
							 lvi.EnsureVisible();
						 break;
					 }
				 }
			 }


			 private void bnAddThisOne_Click(object sender, System.EventArgs e)
			 {
				 if (lvFMNewShows.SelectedItems.Count == 0)
					 return;

				 System.Windows.Forms.DialogResult res = MessageBox.Show("Add the selected folders to My Shows?","Folder Monitor",MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				 if (res != System.Windows.Forms.DialogResult.Yes)
					 return;

				 System.Collections.Generic.List<AddItem > toAdd = new System.Collections.Generic.List<AddItem >();
				 foreach (ListViewItem lvi in lvFMNewShows.SelectedItems)
				 {
					 AddItem ai = (AddItem)(lvi.Tag);
					 toAdd.Add(ai);
					 mDoc.AddItems.Remove(ai);
				 }
				 ProcessAddItems(toAdd);
			 }

			 private void bnFolderMonitorDone_Click(object sender, System.EventArgs e)
			 {
				 System.Windows.Forms.DialogResult res = MessageBox.Show("Add all of these to My Shows?","Folder Monitor",MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				 if (res != System.Windows.Forms.DialogResult.Yes)
					 return;

				 ProcessAddItems(mDoc.AddItems);

				 this.Close();
			 }

			 private void ProcessAddItems(System.Collections.Generic.List<AddItem > toAdd)
			 {
				 foreach (AddItem ai in toAdd)
				 {
					 if (ai.TheSeries == null)
						 continue; // skip

					 // see if there is a matching show item
					 ShowItem found = null;
					 foreach (ShowItem si in mDoc.GetShowItems(true))
					 {
						 if ((ai.TheSeries != null) && (ai.TheSeries.TVDBCode == si.TVDBCode))
						 {
							 found = si;
							 break;
						 }
					 }
					 mDoc.UnlockShowItems();
					 if (found == null)
					 {
						 ShowItem si = new ShowItem(mDoc.GetTVDB(false,""));
						 si.TVDBCode = ai.TheSeries.TVDBCode;
						 //si->ShowName() = ai->TheSeries->Name;
						 mDoc.GetShowItems(true).Add(si);
						 mDoc.UnlockShowItems();
						 mDoc.GenDict();
						 found = si;
					 }

					 if ((ai.FolderMode == FolderModeEnum.kfmFolderPerSeason) || (ai.FolderMode == FolderModeEnum.kfmFlat))
					 {
						 found.AutoAdd_FolderBase = ai.Folder;
						 found.AutoAdd_FolderPerSeason = ai.FolderMode == FolderModeEnum.kfmFolderPerSeason;
						 string foldername = "Season ";

						 foreach (DirectoryInfo di in new DirectoryInfo(ai.Folder).GetDirectories("*Season *"))
						 {
							 string s = di.FullName;
							 string f = ai.Folder;
							 if (!f.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
								 f = f + System.IO.Path.DirectorySeparatorChar.ToString();
							 f = Regex.Escape(f);
							 s = Regex.Replace(s, f+"(.*Season ).*", "$1",RegexOptions.IgnoreCase);
							 if (!string.IsNullOrEmpty(s))
							 {
								 foldername = s;
								 break;
							 }
						 }

						 found.AutoAdd_SeasonFolderName = foldername;
					 }

					 if ((ai.FolderMode == FolderModeEnum.kfmSpecificSeason) && (ai.SpecificSeason != -1))
					 {
						 if (!found.ManualFolderLocations.ContainsKey(ai.SpecificSeason))
							 found.ManualFolderLocations[ai.SpecificSeason] = new StringList();
						 found.ManualFolderLocations[ai.SpecificSeason].Add(ai.Folder);
					 }

					 mDoc.Stats().AutoAddedShows++;
				 }

				 mDoc.Dirty();
				 toAdd.Clear();

				 FillFMNewShowList(true);
			 }

			 private void GuessAI(AddItem ai)
			 {
				 TVDoc.GuessShowName(ai);
				 if (!string.IsNullOrEmpty(ai.ShowName))
				 {
					 TheTVDB db = mDoc.GetTVDB(true,"GuessAI");
					 foreach (System.Collections.Generic.KeyValuePair<int, SeriesInfo > ser in db.GetSeriesDict())
					 {
						 string s;
						 s = ser.Value.Name.ToLower();
						 if (s == ai.ShowName.ToLower())
						 {
							 ai.TheSeries = ser.Value;
							 break;
						 }
					 }
					 db.Unlock("GuessAI");
				 }
			 }

			 private void GuessAll() // not all -> selected only
			 {
				 foreach (AddItem ai in mDoc.AddItems)
					 GuessAI(ai);
				 FillFMNewShowList(false);
			 }

			 private void FMControlLeave(object sender, System.EventArgs e)
			 {
				 if (lvFMNewShows.SelectedItems.Count != 0)
				 {
					 AddItem ai = (AddItem)(lvFMNewShows.SelectedItems[0].Tag);
					 UpdateFMListItem(ai, false);
				 }

			 }
			 private void bnFMVisitTVcom_Click(object sender, System.EventArgs e)
			 {
				 int code = mTCCF.SelectedCode();
				 TVDoc.SysOpen(mDoc.GetTVDB(false,"").WebsiteURL(code, -1, false));
			 }
			 private void SetAllFolderModes(FolderModeEnum fm)
			 {
				 foreach (ListViewItem lvi in lvFMNewShows.SelectedItems)
				 {
					 AddItem ai = (AddItem)(lvi.Tag);

					 ai.FolderMode = fm;
					 UpdateFMListItem(ai, false);
				 }

			 }

	private void rbSpecificSeason_CheckedChanged(object sender, System.EventArgs e)
			 {
				 txtFMSpecificSeason.Enabled = rbSpecificSeason.Checked;

				 if (mInternalChange == 0)
					 SetAllFolderModes(FolderModeEnum.kfmSpecificSeason);
			 }
	private void rbFlat_CheckedChanged(object sender, System.EventArgs e)
			 {
				 if (mInternalChange == 0)
					 SetAllFolderModes(FolderModeEnum.kfmFlat);
			 }
	private void rbFolderPerSeason_CheckedChanged(object sender, System.EventArgs e)
			 {
				 if (mInternalChange == 0)
					 SetAllFolderModes(FolderModeEnum.kfmFolderPerSeason);
			 }

			 private void lstFMMonitorFolders_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
			 {
				 if (e.Button != System.Windows.Forms.MouseButtons.Right)
					 return;
//
//
//				 lstFMMonitorFolders->ClearSelected();
//				 lstFMMonitorFolders->SelectedIndex = lstFMMonitorFolders->IndexFromPoint(Point(e->X,e->Y));
//
//				 int p;
//				 if ((p = lstFMMonitorFolders->SelectedIndex) == -1)
//					 return;
//
//				 Point^ pt = lstFMMonitorFolders->PointToScreen(Point(e->X, e->Y));
//				 RightClickOnFolder(lstFMMonitorFolders->Items[p]->ToString(),pt);
//				 
			 }
			 private void lstFMIgnoreFolders_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
			 {
				 if (e.Button != System.Windows.Forms.MouseButtons.Right)
					 return;
//
//				 lstFMIgnoreFolders->ClearSelected();
//				 lstFMIgnoreFolders->SelectedIndex = lstFMIgnoreFolders->IndexFromPoint(Point(e->X,e->Y));
//
//				 int p;
//				 if ((p = lstFMIgnoreFolders->SelectedIndex) == -1)
//					 return;
//
//				 Point^ pt = lstFMIgnoreFolders->PointToScreen(Point(e->X, e->Y));
//				 RightClickOnFolder(lstFMIgnoreFolders->Items[p]->ToString(),pt);
//				 
			 }

}
}