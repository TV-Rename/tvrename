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
	/// Summary for uTorrent
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public class uTorrent : System.Windows.Forms.Form
	{
		private SetProgressDelegate SetProg;

	private System.Windows.Forms.Button bnClose;
	private System.Windows.Forms.Label lbDPMatch;
	private System.Windows.Forms.Label lbDPMissing;
	private System.IO.FileSystemWatcher watcher;
	private System.Windows.Forms.FolderBrowserDialog folderBrowser;


			 private TVDoc mDoc;


		public uTorrent(TVDoc doc, SetProgressDelegate progdel)
		{
			mDoc = doc;
			SetProg = progdel;

			InitializeComponent();

			watcher.Error += new ErrorEventHandler(WatcherError);

			bool en = false;
			foreach (AIOItem i in mDoc.TheAIOList)
				if (i.Type == AIOType.kMissing)
				{
					en = true;
					break;
				}
				cbUTMatchMissing.Enabled = en;
				EnableDisable();

				bnUTRefresh_Click(null, null);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
	private System.Windows.Forms.CheckBox cbUTSetPrio;
	private System.Windows.Forms.CheckBox cbUTMatchMissing;
	private System.Windows.Forms.CheckBox cbUTUseHashing;
	private System.Windows.Forms.CheckBox chkUTTest;
	private System.Windows.Forms.CheckBox chkUTSearchSubfolders;
	private System.Windows.Forms.Button bnUTNone;
	private System.Windows.Forms.Button bnUTGo;
	private System.Windows.Forms.Button bnUTRefresh;
	private System.Windows.Forms.Button bnUTAll;
	private System.Windows.Forms.Button bnUTBrowseSearchFolder;

	private System.Windows.Forms.TextBox txtUTSearchFolder;
	private System.Windows.Forms.Label label15;
	private System.Windows.Forms.Label label13;


	private System.Windows.Forms.Label label11;
	private System.Windows.Forms.CheckedListBox lbUTTorrents;
	private System.Windows.Forms.ListView lvUTResults;
	private System.Windows.Forms.ColumnHeader columnHeader50;
	private System.Windows.Forms.ColumnHeader columnHeader48;
	private System.Windows.Forms.ColumnHeader columnHeader49;
	private System.Windows.Forms.ColumnHeader columnHeader51;
	private System.Windows.Forms.ColumnHeader columnHeader52;


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
			System.ComponentModel.ComponentResourceManager resources = (new System.ComponentModel.ComponentResourceManager(typeof(uTorrent)));
			this.bnUTBrowseSearchFolder = (new System.Windows.Forms.Button());
			this.bnUTAll = (new System.Windows.Forms.Button());
			this.bnUTRefresh = (new System.Windows.Forms.Button());
			this.bnUTGo = (new System.Windows.Forms.Button());
			this.bnUTNone = (new System.Windows.Forms.Button());
			this.chkUTSearchSubfolders = (new System.Windows.Forms.CheckBox());
			this.chkUTTest = (new System.Windows.Forms.CheckBox());
			this.cbUTUseHashing = (new System.Windows.Forms.CheckBox());
			this.cbUTMatchMissing = (new System.Windows.Forms.CheckBox());
			this.cbUTSetPrio = (new System.Windows.Forms.CheckBox());
			this.lbUTTorrents = (new System.Windows.Forms.CheckedListBox());
			this.label11 = (new System.Windows.Forms.Label());
			this.label13 = (new System.Windows.Forms.Label());
			this.label15 = (new System.Windows.Forms.Label());
			this.txtUTSearchFolder = (new System.Windows.Forms.TextBox());
			this.columnHeader50 = (new System.Windows.Forms.ColumnHeader());
			this.columnHeader48 = (new System.Windows.Forms.ColumnHeader());
			this.columnHeader49 = (new System.Windows.Forms.ColumnHeader());
			this.columnHeader51 = (new System.Windows.Forms.ColumnHeader());
			this.columnHeader52 = (new System.Windows.Forms.ColumnHeader());
			this.lvUTResults = (new System.Windows.Forms.ListView());
			this.bnClose = (new System.Windows.Forms.Button());
			this.lbDPMatch = (new System.Windows.Forms.Label());
			this.lbDPMissing = (new System.Windows.Forms.Label());
			this.watcher = (new System.IO.FileSystemWatcher());
			this.folderBrowser = (new System.Windows.Forms.FolderBrowserDialog());
			((System.ComponentModel.ISupportInitialize)(this.watcher)).BeginInit();
			this.SuspendLayout();
			// 
			// bnUTBrowseSearchFolder
			// 
			this.bnUTBrowseSearchFolder.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
			this.bnUTBrowseSearchFolder.Location = new System.Drawing.Point(735, 284);
			this.bnUTBrowseSearchFolder.Name = "bnUTBrowseSearchFolder";
			this.bnUTBrowseSearchFolder.Size = new System.Drawing.Size(75, 23);
			this.bnUTBrowseSearchFolder.TabIndex = 10;
			this.bnUTBrowseSearchFolder.Text = "B&rowse...";
			this.bnUTBrowseSearchFolder.UseVisualStyleBackColor = true;
			this.bnUTBrowseSearchFolder.Click += new System.EventHandler(bnUTBrowseSearchFolder_Click);
			// 
			// bnUTAll
			// 
			this.bnUTAll.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
			this.bnUTAll.Location = new System.Drawing.Point(816, 32);
			this.bnUTAll.Name = "bnUTAll";
			this.bnUTAll.Size = new System.Drawing.Size(75, 23);
			this.bnUTAll.TabIndex = 6;
			this.bnUTAll.Text = "&All";
			this.bnUTAll.UseVisualStyleBackColor = true;
			this.bnUTAll.Click += new System.EventHandler(bnUTAll_Click);
			// 
			// bnUTRefresh
			// 
			this.bnUTRefresh.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
			this.bnUTRefresh.Location = new System.Drawing.Point(816, 3);
			this.bnUTRefresh.Name = "bnUTRefresh";
			this.bnUTRefresh.Size = new System.Drawing.Size(75, 23);
			this.bnUTRefresh.TabIndex = 3;
			this.bnUTRefresh.Text = "Refres&h";
			this.bnUTRefresh.UseVisualStyleBackColor = true;
			this.bnUTRefresh.Click += new System.EventHandler(bnUTRefresh_Click);
			// 
			// bnUTGo
			// 
			this.bnUTGo.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.bnUTGo.Location = new System.Drawing.Point(123, 588);
			this.bnUTGo.Name = "bnUTGo";
			this.bnUTGo.Size = new System.Drawing.Size(75, 23);
			this.bnUTGo.TabIndex = 11;
			this.bnUTGo.Text = "&Go";
			this.bnUTGo.UseVisualStyleBackColor = true;
			this.bnUTGo.Click += new System.EventHandler(bnUTGo_Click);
			// 
			// bnUTNone
			// 
			this.bnUTNone.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
			this.bnUTNone.Location = new System.Drawing.Point(816, 61);
			this.bnUTNone.Name = "bnUTNone";
			this.bnUTNone.Size = new System.Drawing.Size(75, 23);
			this.bnUTNone.TabIndex = 7;
			this.bnUTNone.Text = "&None";
			this.bnUTNone.UseVisualStyleBackColor = true;
			this.bnUTNone.Click += new System.EventHandler(bnUTNone_Click);
			// 
			// chkUTSearchSubfolders
			// 
			this.chkUTSearchSubfolders.AutoSize = true;
			this.chkUTSearchSubfolders.Location = new System.Drawing.Point(142, 359);
			this.chkUTSearchSubfolders.Name = "chkUTSearchSubfolders";
			this.chkUTSearchSubfolders.Size = new System.Drawing.Size(132, 17);
			this.chkUTSearchSubfolders.TabIndex = 14;
			this.chkUTSearchSubfolders.Text = "S&earch subfolders, too";
			this.chkUTSearchSubfolders.UseVisualStyleBackColor = true;
			// 
			// chkUTTest
			// 
			this.chkUTTest.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.chkUTTest.AutoSize = true;
			this.chkUTTest.Checked = true;
			this.chkUTTest.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkUTTest.Location = new System.Drawing.Point(204, 592);
			this.chkUTTest.Name = "chkUTTest";
			this.chkUTTest.Size = new System.Drawing.Size(70, 17);
			this.chkUTTest.TabIndex = 12;
			this.chkUTTest.Text = "Te&st Run";
			this.chkUTTest.UseVisualStyleBackColor = true;
			// 
			// cbUTUseHashing
			// 
			this.cbUTUseHashing.AutoSize = true;
			this.cbUTUseHashing.Location = new System.Drawing.Point(123, 336);
			this.cbUTUseHashing.Name = "cbUTUseHashing";
			this.cbUTUseHashing.Size = new System.Drawing.Size(189, 17);
			this.cbUTUseHashing.TabIndex = 13;
			this.cbUTUseHashing.Text = "Match against files in search folder";
			this.cbUTUseHashing.UseVisualStyleBackColor = true;
			this.cbUTUseHashing.CheckedChanged += new System.EventHandler(cbUTUseHashing_CheckedChanged);
			// 
			// cbUTMatchMissing
			// 
			this.cbUTMatchMissing.AutoSize = true;
			this.cbUTMatchMissing.Enabled = false;
			this.cbUTMatchMissing.Location = new System.Drawing.Point(123, 382);
			this.cbUTMatchMissing.Name = "cbUTMatchMissing";
			this.cbUTMatchMissing.Size = new System.Drawing.Size(185, 17);
			this.cbUTMatchMissing.TabIndex = 15;
			this.cbUTMatchMissing.Text = "Match against missing episode list";
			this.cbUTMatchMissing.UseVisualStyleBackColor = true;
			this.cbUTMatchMissing.CheckedChanged += new System.EventHandler(cbUTMatchMissing_CheckedChanged);
			// 
			// cbUTSetPrio
			// 
			this.cbUTSetPrio.AutoSize = true;
			this.cbUTSetPrio.Location = new System.Drawing.Point(364, 314);
			this.cbUTSetPrio.Name = "cbUTSetPrio";
			this.cbUTSetPrio.Size = new System.Drawing.Size(132, 17);
			this.cbUTSetPrio.TabIndex = 16;
			this.cbUTSetPrio.Text = "Set download priorities";
			this.cbUTSetPrio.UseVisualStyleBackColor = true;
			this.cbUTSetPrio.CheckedChanged += new System.EventHandler(cbUTSetPrio_CheckedChanged);
			// 
			// lbUTTorrents
			// 
			this.lbUTTorrents.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			this.lbUTTorrents.CheckOnClick = true;
			this.lbUTTorrents.FormattingEnabled = true;
			this.lbUTTorrents.IntegralHeight = false;
			this.lbUTTorrents.Location = new System.Drawing.Point(123, 33);
			this.lbUTTorrents.Name = "lbUTTorrents";
			this.lbUTTorrents.ScrollAlwaysVisible = true;
			this.lbUTTorrents.Size = new System.Drawing.Size(687, 244);
			this.lbUTTorrents.TabIndex = 5;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(33, 33);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(84, 13);
			this.label11.TabIndex = 4;
			this.label11.Text = "Choose &torrents:";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(41, 289);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(76, 13);
			this.label13.TabIndex = 8;
			this.label13.Text = "Sear&ch Folder:";
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(77, 410);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(40, 13);
			this.label15.TabIndex = 17;
			this.label15.Text = "Status:";
			// 
			// txtUTSearchFolder
			// 
			this.txtUTSearchFolder.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			this.txtUTSearchFolder.Location = new System.Drawing.Point(123, 286);
			this.txtUTSearchFolder.Name = "txtUTSearchFolder";
			this.txtUTSearchFolder.Size = new System.Drawing.Size(606, 20);
			this.txtUTSearchFolder.TabIndex = 9;
			// 
			// columnHeader50
			// 
			this.columnHeader50.Text = "Reason";
			// 
			// columnHeader48
			// 
			this.columnHeader48.Text = "Torrent";
			this.columnHeader48.Width = 230;
			// 
			// columnHeader49
			// 
			this.columnHeader49.Text = "#";
			this.columnHeader49.Width = 28;
			// 
			// columnHeader51
			// 
			this.columnHeader51.Text = "Priority";
			// 
			// columnHeader52
			// 
			this.columnHeader52.Text = "Location";
			this.columnHeader52.Width = 280;
			// 
			// lvUTResults
			// 
			this.lvUTResults.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			this.lvUTResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[5] {this.columnHeader50, this.columnHeader48, this.columnHeader49, this.columnHeader51, this.columnHeader52});
			this.lvUTResults.FullRowSelect = true;
			this.lvUTResults.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lvUTResults.Location = new System.Drawing.Point(123, 410);
			this.lvUTResults.Name = "lvUTResults";
			this.lvUTResults.ShowItemToolTips = true;
			this.lvUTResults.Size = new System.Drawing.Size(686, 172);
			this.lvUTResults.TabIndex = 18;
			this.lvUTResults.UseCompatibleStateImageBehavior = false;
			this.lvUTResults.View = System.Windows.Forms.View.Details;
			// 
			// bnClose
			// 
			this.bnClose.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
			this.bnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bnClose.Location = new System.Drawing.Point(816, 588);
			this.bnClose.Name = "bnClose";
			this.bnClose.Size = new System.Drawing.Size(75, 23);
			this.bnClose.TabIndex = 20;
			this.bnClose.Text = "Close";
			this.bnClose.UseVisualStyleBackColor = true;
			this.bnClose.Click += new System.EventHandler(bnClose_Click);
			// 
			// lbDPMatch
			// 
			this.lbDPMatch.AutoSize = true;
			this.lbDPMatch.Location = new System.Drawing.Point(361, 337);
			this.lbDPMatch.Name = "lbDPMatch";
			this.lbDPMatch.Size = new System.Drawing.Size(155, 13);
			this.lbDPMatch.TabIndex = 8;
			this.lbDPMatch.Text = "Enable matched, disable others";
			// 
			// lbDPMissing
			// 
			this.lbDPMissing.AutoSize = true;
			this.lbDPMissing.Location = new System.Drawing.Point(361, 383);
			this.lbDPMissing.Name = "lbDPMissing";
			this.lbDPMissing.Size = new System.Drawing.Size(328, 13);
			this.lbDPMissing.TabIndex = 8;
			this.lbDPMissing.Text = "Enable missing episodes, disable others, unless also matched above";
			// 
			// watcher
			// 
			this.watcher.EnableRaisingEvents = true;
			this.watcher.Filter = "resume.dat";
			this.watcher.NotifyFilter = (System.IO.NotifyFilters)(((System.IO.NotifyFilters.LastWrite | System.IO.NotifyFilters.LastAccess) | System.IO.NotifyFilters.CreationTime));
			this.watcher.SynchronizingObject = this;
			this.watcher.Created += new System.IO.FileSystemEventHandler(watcher_Created);
			this.watcher.Changed += new System.IO.FileSystemEventHandler(watcher_Changed);
			// 
			// folderBrowser
			// 
			this.folderBrowser.ShowNewFolderButton = false;
			// 
			// uTorrent
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.bnClose;
			this.ClientSize = new System.Drawing.Size(903, 623);
			this.Controls.Add(this.bnClose);
			this.Controls.Add(this.lvUTResults);
			this.Controls.Add(this.cbUTSetPrio);
			this.Controls.Add(this.cbUTMatchMissing);
			this.Controls.Add(this.cbUTUseHashing);
			this.Controls.Add(this.chkUTTest);
			this.Controls.Add(this.chkUTSearchSubfolders);
			this.Controls.Add(this.bnUTNone);
			this.Controls.Add(this.bnUTGo);
			this.Controls.Add(this.bnUTRefresh);
			this.Controls.Add(this.bnUTAll);
			this.Controls.Add(this.bnUTBrowseSearchFolder);
			this.Controls.Add(this.txtUTSearchFolder);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.lbDPMissing);
			this.Controls.Add(this.lbDPMatch);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.lbUTTorrents);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "uTorrent";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "TVRename - µTorrent";
			((System.ComponentModel.ISupportInitialize)(this.watcher)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

#endregion
	private void bnUTRefresh_Click(object sender, System.EventArgs e)
			 {
				 RefreshResumeDat();
			 }
			 private void UTSelectNone()
			 {
				 for (int i =0;i<lbUTTorrents.Items.Count;i++)
					 lbUTTorrents.SetItemChecked(i, false);
			 }
			 private void UTSelectAll()
			 {
				 for (int i =0;i<lbUTTorrents.Items.Count;i++)
					 lbUTTorrents.SetItemChecked(i, true);
			 }
	private void bnUTAll_Click(object sender, System.EventArgs e)
			 {
				 UTSelectAll();
			 }
	private void bnUTNone_Click(object sender, System.EventArgs e)
			 {
				 UTSelectNone();
			 }
			 private bool CheckResumeDatPath()
			 {
				 if (string.IsNullOrEmpty(mDoc.Settings.ResumeDatPath) || !File.Exists(mDoc.Settings.ResumeDatPath))
				 {
					 MessageBox.Show("Please set the resume.dat path in Preferences before using this feature","µTorrent",MessageBoxButtons.OK);
					 return false;
				 }
				 return true;
			 }

			 private void RefreshResumeDat()
			 {
				 if (!CheckResumeDatPath())
					 return;

                 StringList checkedItems = new StringList();
				 foreach (string torrent in lbUTTorrents.CheckedItems)
					 checkedItems.Add(torrent);

				 lbUTTorrents.Items.Clear();
				 // open resume.dat file, fill checked list box with torrents available to choose from

				 string file = mDoc.Settings.ResumeDatPath;
				 if (!File.Exists(file))
				 {
					 return;
				 }
				 BTFile resumeDat = BEncodeLoader.Load(file);
				 if (resumeDat == null)
				 {
					 return;
				 }
				 BTDictionary dict = resumeDat.GetDict();
				 for (int i =0;i<dict.Items.Count;i++)
				 {
					 BTItem it = dict.Items[i];
					 if (it.Type == AnonymousEnum.kDictionaryItem)
					 {
						 BTDictionaryItem d2 = (BTDictionaryItem)(it);
						 if ((d2.Key != ".fileguard") && (d2.Data.Type == AnonymousEnum.kDictionary))
							 lbUTTorrents.Items.Add(d2.Key);
					 }
				 }

				 foreach (string torrent in checkedItems)
					 for (int i =0;i<lbUTTorrents.Items.Count;i++)
						 if (lbUTTorrents.Items[i].ToString() == torrent)
							 lbUTTorrents.SetItemChecked(i, true);
			 }
	private void bnUTGo_Click(object sender, System.EventArgs e)
			 {
				 if (!CheckResumeDatPath())
					 return;

				 string searchFolder = txtUTSearchFolder.Text;
				 string resumeDatFile = mDoc.Settings.ResumeDatPath;
				 bool testMode = chkUTTest.Checked;

				 if (!File.Exists(resumeDatFile))
					 return;

				 if (!testMode && !CheckUTorrentClosed())
					 return;
//
//				 int action = actNone;
//				 if (chkUTSearchSubfolders->Checked)
//					 action |=  actSearchSubfolders;
//
//
//
//				 if ( (action & (actRename | actCopy | actMatchMissing | actHashSearch)) == 0 )
//					 return;
//

				 lvUTResults.Items.Clear();

				 BTResume btp = new BTResume(SetProg, resumeDatFile);

				 System.Collections.Generic.List<string > sl = new System.Collections.Generic.List<string >();

				 foreach (string torrent in lbUTTorrents.CheckedItems)
					 sl.Add(torrent);

				 btp.DoWork(sl, searchFolder, lvUTResults, cbUTUseHashing.Checked, cbUTMatchMissing.Checked, cbUTSetPrio.Checked, testMode, chkUTSearchSubfolders.Checked, mDoc.TheAIOList, mDoc.Settings.FNPRegexs);

				 if (!testMode)
					 RestartUTorrent();
			 }
	private void cbUTUseHashing_CheckedChanged(object sender, System.EventArgs e)
			 {
				 EnableDisable();
			 }
			 private void EnableDisable()
			 {
				 bool en = cbUTUseHashing.Checked;
				 txtUTSearchFolder.Enabled = en;
				 bnUTBrowseSearchFolder.Enabled = en;
				 chkUTSearchSubfolders.Enabled = en;

				 lbDPMatch.Enabled = cbUTSetPrio.Checked && cbUTUseHashing.Checked;
				 lbDPMissing.Enabled = cbUTSetPrio.Checked && cbUTMatchMissing.Checked;

			 }

			 private static bool CheckUTorrentClosed()
			 {
				 DialogResult dr = MessageBox.Show("Make sure µTorrent is not running, then click OK.","TVRename",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning);
				 return (dr ==DialogResult.OK);
			 }

			 private static bool RestartUTorrent()
			 {
				 MessageBox.Show("You may now restart µTorrent.","TVRename",MessageBoxButtons.OK,MessageBoxIcon.Warning);
				 return true;
			 }


	private void bnClose_Click(object sender, System.EventArgs e)
			 {
				 this.Close();
			 }
	private void cbUTMatchMissing_CheckedChanged(object sender, System.EventArgs e)
			 {
				 EnableDisable();
			 }
	private void cbUTSetPrio_CheckedChanged(object sender, System.EventArgs e)
			 {
				 EnableDisable();
			 }
	private void watcher_Changed(object sender, System.IO.FileSystemEventArgs e)
			 {
				 RefreshResumeDat();
			 }
			 private void WatcherError(object UnnamedParameter1, System.IO.ErrorEventArgs UnnamedParameter2)
			 {
				 while (!watcher.EnableRaisingEvents)
				 {
					 try
					 {
						 StartWatching();
						 RefreshResumeDat();
					 }
					 catch
					 {
						 Threading.Thread.Sleep(500);
					 }
				 }
			 }

	private void txtResumeDatFolder_TextChanged(object sender, System.EventArgs e)
			 {
				 StartWatching();
			 }
			 private void StartWatching()
			 {
				 FileInfo f = new FileInfo(mDoc.Settings.ResumeDatPath);
				 if (f.Exists)
				 {
					 watcher.Path = f.Directory.Name;
					 watcher.Filter = "resume.dat";
					 watcher.EnableRaisingEvents = true;
				 }
				 else
					 watcher.EnableRaisingEvents = false;
			 }
	private void watcher_Created(object sender, System.IO.FileSystemEventArgs e)
			 {
				 RefreshResumeDat();
			 }
	private void bnUTBrowseSearchFolder_Click(object sender, System.EventArgs e)
			 {
				 folderBrowser.SelectedPath = txtUTSearchFolder.Text;

				 if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					 txtUTSearchFolder.Text = folderBrowser.SelectedPath;
			 }
	}
}