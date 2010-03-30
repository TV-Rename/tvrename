//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//


using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;


namespace TVRename
{

	/// <summary>
	/// Summary for Preferences
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public partial class Preferences : Form
	{
		private TVDoc mDoc;

        private StringList LangList;

		public Preferences(TVDoc doc, bool goToScanOpts)
		{
			LangList = null;

			InitializeComponent();

			SetupRSSGrid();
			SetupReplacementsGrid();

			mDoc = doc;

			if (goToScanOpts)
				tabControl1.SelectedTab = tpScanOptions;
		}

        private void OKButton_Click(object sender, System.EventArgs e)
         {
	         if (!TVSettings.OKExtensionsString(txtVideoExtensions.Text))
	         {
		         MessageBox.Show("Extensions list must be separated by semicolons, and each extension must start with a dot.","Preferences",MessageBoxButtons.OK, MessageBoxIcon.Warning);
		         tabControl1.SelectedIndex = 1;
		         txtVideoExtensions.Focus();
		         return;
	         }
	         if (!TVSettings.OKExtensionsString(txtOtherExtensions.Text))
	         {
		         MessageBox.Show("Extensions list must be separated by semicolons, and each extension must start with a dot.","Preferences",MessageBoxButtons.OK, MessageBoxIcon.Warning);
		         tabControl1.SelectedIndex = 1;
		         txtOtherExtensions.Focus();
		         return;
	         }
	         TVSettings S = mDoc.Settings;
	         S.Replacements.Clear();
	         for (int i =1;i<ReplacementsGrid.RowsCount;i++)
	         {
		         string from = (string)(ReplacementsGrid[i,0].Value);
		         string to = (string)(ReplacementsGrid[i,1].Value);
		         bool ins = (bool)(ReplacementsGrid[i,2].Value);
		         if (!string.IsNullOrEmpty(from))
			         S.Replacements.Add(new Replacement(from,to,ins));
	         }


	         S.ExportWTWRSS = cbWTWRSS.Checked;
	         S.ExportWTWRSSTo = txtWTWRSS.Text;

	         S.ExportMissingXML= cbMissingXML.Checked;
	         S.ExportMissingXMLTo = txtMissingXML.Text;
	         S.ExportMissingCSV = cbMissingCSV.Checked;
	         S.ExportMissingCSVTo = txtMissingCSV.Text;
	         S.ExportRenamingXML = cbRenamingXML.Checked;
	         S.ExportRenamingXMLTo = txtRenamingXML.Text;
	         S.ExportFOXML = cbFOXML.Checked;
	         S.ExportFOXMLTo = txtFOXML.Text;

	         S.WTWRecentDays = Convert.ToInt32(txtWTWDays.Text);
	         S.StartupTab = cbStartupTab.SelectedIndex;
	         S.NotificationAreaIcon = cbNotificationIcon.Checked;
	         S.SetVideoExtensionsString(txtVideoExtensions.Text);
	         S.SetOtherExtensionsString(txtOtherExtensions.Text);
	         S.ExportRSSMaxDays = Convert.ToInt32(txtExportRSSMaxDays.Text);
	         S.ExportRSSMaxShows = Convert.ToInt32(txtExportRSSMaxShows.Text);
	         S.KeepTogether = cbKeepTogether.Checked;
	         S.LeadingZeroOnSeason = cbLeadingZero.Checked;
	         S.ShowInTaskbar = chkShowInTaskbar.Checked;
	         S.RenameTxtToSub = cbTxtToSub.Checked;
	         S.ShowEpisodePictures = cbShowEpisodePictures.Checked;
	         S.AutoSelectShowInMyShows = cbAutoSelInMyShows.Checked;
	         S.SpecialsFolderName = txtSpecialsFolderName.Text;

	         S.ForceLowercaseFilenames = cbForceLower.Checked;
	         S.IgnoreSamples = cbIgnoreSamples.Checked;

	         S.uTorrentPath = txtRSSuTorrentPath.Text;
	         S.ResumeDatPath = txtUTResumeDatPath.Text;

	         S.SearchRSS = cbSearchRSS.Checked;
	         S.EpImgs = cbEpImgs.Checked;
	         S.NFOs = cbNFOs.Checked;
	         S.FolderJpg = cbFolderJpg.Checked;
	         S.RenameCheck = cbRenameCheck.Checked;
	         S.MissingCheck = cbMissing.Checked;
	         S.SearchLocally = cbSearchLocally.Checked;
	         S.LeaveOriginals = cbLeaveOriginals.Checked;
	         S.CheckuTorrent = cbCheckuTorrent.Checked;

	         if (rbFolderFanArt.Checked)
		         S.FolderJpgIs = TVSettings.FolderJpgIsType.FanArt;
	         else if (rbFolderBanner.Checked)
		         S.FolderJpgIs = TVSettings.FolderJpgIsType.Banner;
	         else
		         S.FolderJpgIs = TVSettings.FolderJpgIsType.Poster;

	         if (LangList != null)
	         {
		         //only set if the language tab was visited

		         TheTVDB db = mDoc.GetTVDB(true, "Preferences-OK");
		         db.LanguagePriorityList = LangList;
		         db.SaveCache();
		         db.Unlock("Preferences-OK");
	         }


	         try
	         {
		         S.SampleFileMaxSizeMB = int.Parse(txtMaxSampleSize.Text);
	         }
	         catch
	         {
		         S.SampleFileMaxSizeMB = 50;
	         }

	         try
	         {
		         S.ParallelDownloads = int.Parse(txtParallelDownloads.Text);
	         }
	         catch
	         {
		         S.ParallelDownloads = 4;
	         }

	         if (S.ParallelDownloads < 1)
		         S.ParallelDownloads = 1;
	         else if (S.ParallelDownloads > 8)
		         S.ParallelDownloads = 8;

	         // RSS URLs
	         S.RSSURLs.Clear();
	         for (int i =1;i<RSSGrid.RowsCount;i++)
	         {
		         string url = (string)(RSSGrid[i,0].Value);
		         if (!string.IsNullOrEmpty(url))
			         S.RSSURLs.Add(url);
	         }

	         mDoc.SetDirty();
	         this.DialogResult =DialogResult.OK;
	         this.Close();
         }

        private void Preferences_Load(object sender, System.EventArgs e)
         {
	         TVSettings S = mDoc.Settings;
	         int r = 1;

	         foreach (Replacement R in S.Replacements)
	         {
		         AddNewReplacementRow(R.This, R.That, R.CaseInsensitive);
		         r++;
	         }

	         txtMaxSampleSize.Text = S.SampleFileMaxSizeMB.ToString();

	         cbWTWRSS.Checked = S.ExportWTWRSS;
	         txtWTWRSS.Text = S.ExportWTWRSSTo;
	         txtWTWDays.Text = S.WTWRecentDays.ToString();
	         txtExportRSSMaxDays.Text = S.ExportRSSMaxDays.ToString();
	         txtExportRSSMaxShows.Text = S.ExportRSSMaxShows.ToString();

	         cbMissingXML.Checked = S.ExportMissingXML;
	         txtMissingXML.Text = S.ExportMissingXMLTo;
	         cbMissingCSV.Checked = S.ExportMissingCSV;
	         txtMissingCSV.Text = S.ExportMissingCSVTo;

	         cbRenamingXML.Checked = S.ExportRenamingXML;
	         txtRenamingXML.Text = S.ExportRenamingXMLTo;

	         cbFOXML.Checked = S.ExportFOXML;
	         txtFOXML.Text = S.ExportFOXMLTo;

	         cbStartupTab.SelectedIndex = S.StartupTab;
	         cbNotificationIcon.Checked = S.NotificationAreaIcon;
	         txtVideoExtensions.Text = S.GetVideoExtensionsString();
	         txtOtherExtensions.Text = S.GetOtherExtensionsString();

	         cbKeepTogether.Checked = S.KeepTogether;
	         cbKeepTogether_CheckedChanged(null, null);

	         cbLeadingZero.Checked = S.LeadingZeroOnSeason;
	         chkShowInTaskbar.Checked = S.ShowInTaskbar;
	         cbTxtToSub.Checked = S.RenameTxtToSub;
	         cbShowEpisodePictures.Checked = S.ShowEpisodePictures;
	         cbAutoSelInMyShows.Checked = S.AutoSelectShowInMyShows;
	         txtSpecialsFolderName.Text = S.SpecialsFolderName;
	         cbForceLower.Checked = S.ForceLowercaseFilenames;
	         cbIgnoreSamples.Checked = S.IgnoreSamples;
	         txtRSSuTorrentPath.Text = S.uTorrentPath;
	         txtUTResumeDatPath.Text = S.ResumeDatPath;

	         txtParallelDownloads.Text = S.ParallelDownloads.ToString();

	         cbSearchRSS.Checked = S.SearchRSS;
	         cbEpImgs.Checked = S.EpImgs;
	         cbNFOs.Checked = S.NFOs;
	         cbFolderJpg.Checked = S.FolderJpg;
	         cbRenameCheck.Checked = S.RenameCheck;
	         cbCheckuTorrent.Checked = S.CheckuTorrent;
	         cbMissing.Checked = S.MissingCheck;
	         cbSearchLocally.Checked = S.SearchLocally;
	         cbLeaveOriginals.Checked = S.LeaveOriginals;

	         EnableDisable(null, null);

	         FillSearchFolderList();

	         foreach (string s in S.RSSURLs)
		         AddNewRSSRow(s);

	         switch (S.FolderJpgIs)
	         {
	         case TVSettings.FolderJpgIsType.FanArt:
		         rbFolderFanArt.Checked = true;
		         break;
	         case TVSettings.FolderJpgIsType.Banner:
		         rbFolderBanner.Checked = true;
		         break;
	         default:
		         rbFolderPoster.Checked = true;
		         break;
	         }

         }
         private void Browse(TextBox txt)
         {
	         saveFile.FileName = txt.Text;
	         if (saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
		         txt.Text = saveFile.FileName;

         }
        private void bnBrowseWTWRSS_Click(object sender, System.EventArgs e)
         {
	         Browse(txtWTWRSS);
         }

        private void txtNumberOnlyKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
         {
	         // digits only
	         if ((e.KeyChar >= 32) && (!Char.IsDigit(e.KeyChar)))
		         e.Handled = true;
         }

        private void CancelButton_Click(object sender, System.EventArgs e)
         {
	         this.Close();
         }

        private void cbNotificationIcon_CheckedChanged(object sender, System.EventArgs e)
         {
	         if (!cbNotificationIcon.Checked)
		         chkShowInTaskbar.Checked = true;
         }

        private void chkShowInTaskbar_CheckedChanged(object sender, System.EventArgs e)
         {
	         if (!chkShowInTaskbar.Checked)
		         cbNotificationIcon.Checked = true;
         }

        private void cbKeepTogether_CheckedChanged(object sender, System.EventArgs e)
         {
	         cbTxtToSub.Enabled = cbKeepTogether.Checked;
         }

        private void bnBrowseMissingCSV_Click(object sender, System.EventArgs e)
         {
	         Browse(txtMissingCSV);
         }

        private void bnBrowseMissingXML_Click(object sender, System.EventArgs e)
         {
	         Browse(txtMissingXML);
         }

        private void bnBrowseRenamingXML_Click(object sender, System.EventArgs e)
         {
	         Browse(txtRenamingXML);
         }

        private void bnBrowseFOXML_Click(object sender, System.EventArgs e)
         {
	         Browse(txtFOXML);
         }

        private void EnableDisable(object sender, System.EventArgs e)
         {
             bool wtw = cbWTWRSS.Checked;
             txtWTWRSS.Enabled = wtw;
             bnBrowseWTWRSS.Enabled = wtw;
             label15.Enabled = wtw;
             label16.Enabled = wtw;
             label17.Enabled = wtw;
             txtExportRSSMaxDays.Enabled = wtw;
             txtExportRSSMaxShows.Enabled = wtw;

             bool fo = cbFOXML.Checked;
             txtFOXML.Enabled = fo;
             bnBrowseFOXML.Enabled = fo;

             bool ren = cbRenamingXML.Checked;
             txtRenamingXML.Enabled = ren;
             bnBrowseRenamingXML.Enabled = ren;

             bool misx = cbMissingXML.Checked;
             txtMissingXML.Enabled = misx;
             bnBrowseMissingXML.Enabled = misx;

             bool misc = cbMissingCSV.Checked;
             txtMissingCSV.Enabled = misc;
             bnBrowseMissingCSV.Enabled = misc;
         }

         private void bnAddSearchFolder_Click(object sender, System.EventArgs e)
         {
             int n = lbSearchFolders.SelectedIndex;
             if (n != -1)
	             folderBrowser.SelectedPath = mDoc.SearchFolders[n];
             else
	             folderBrowser.SelectedPath = "";

             if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
             {
	             mDoc.SearchFolders.Add(folderBrowser.SelectedPath);
	             mDoc.SetDirty();
             }

             FillSearchFolderList();
         }

         private void bnRemoveSearchFolder_Click(object sender, System.EventArgs e)
         {
             int n = lbSearchFolders.SelectedIndex;
             if (n == -1)
	             return;

             mDoc.SearchFolders.RemoveAt(n);
             mDoc.SetDirty();

             FillSearchFolderList();

         }

         private void bnOpenSearchFolder_Click(object sender, System.EventArgs e)
         {
             int n = lbSearchFolders.SelectedIndex;
             if (n == -1)
	             return;
             TVDoc.SysOpen(mDoc.SearchFolders[n]);
         }
         private void FillSearchFolderList()
         {
             lbSearchFolders.Items.Clear();
             mDoc.SearchFolders.Sort();
             foreach (string efi in mDoc.SearchFolders)
	             lbSearchFolders.Items.Add(efi);
         }

         private void lbSearchFolders_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
         {
             if (e.KeyCode == Keys.Delete)
	             bnRemoveSearchFolder_Click(null, null);
         }

         private void lbSearchFolders_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
         {
             if (!e.Data.GetDataPresent(DataFormats.FileDrop))
	             e.Effect = DragDropEffects.None;
             else
	             e.Effect = DragDropEffects.Copy;
         }
         private void lbSearchFolders_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
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
			             mDoc.SearchFolders.Add(path.ToLower());
		             }
	             }
	             catch
	             {
	             }
             }
             mDoc.SetDirty();
             FillSearchFolderList();
         }

		 private void lbSearchFolders_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		 {
			 if (e.Button != System.Windows.Forms.MouseButtons.Right)
				 return;
//                 
//				 TODO ?
//				 lbSearchFolders->ClearSelected();
//				 lbSearchFolders->SelectedIndex = lbSearchFolders->IndexFromPoint(Point(e->X,e->Y));
//
//				 int p;
//				 if ((p = lbSearchFolders->SelectedIndex) == -1)
//				 return;
//
//				 Point^ pt = lbSearchFolders->PointToScreen(Point(e->X, e->Y));
//				 RightClickOnFolder(lbSearchFolders->Items[p]->ToString(),pt);
//				 }
//				 
		 }

        private void bnRSSBrowseuTorrent_Click(object sender, System.EventArgs e)
         {
	         openFile.FileName = txtRSSuTorrentPath.Text;
	         openFile.Filter = "utorrent.exe|utorrent.exe|All Files (*.*)|*.*";
	         if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
		         txtRSSuTorrentPath.Text = openFile.FileName;
         }

        private void bnUTBrowseResumeDat_Click(object sender, System.EventArgs e)
         {
	         openFile.FileName = txtUTResumeDatPath.Text;
	         openFile.Filter = "resume.dat|resume.dat|All Files (*.*)|*.*";
	         if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
		         txtUTResumeDatPath.Text = openFile.FileName;
         }

         private void SetupReplacementsGrid()
         {
	         SourceGrid.Cells.Views.Cell titleModel = new SourceGrid.Cells.Views.Cell();
	         titleModel.BackColor = Color.SteelBlue;
	         titleModel.ForeColor = Color.White;
	         titleModel.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft;

	         ReplacementsGrid.Columns.Clear();
	         ReplacementsGrid.Rows.Clear();

	         ReplacementsGrid.RowsCount = 1;
	         ReplacementsGrid.ColumnsCount = 3;
	         ReplacementsGrid.FixedRows = 1;
	         ReplacementsGrid.FixedColumns = 0;
	         ReplacementsGrid.Selection.EnableMultiSelection = false;

	         ReplacementsGrid.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.EnableStretch | SourceGrid.AutoSizeMode.EnableAutoSize;
	         ReplacementsGrid.Columns[1].AutoSizeMode = SourceGrid.AutoSizeMode.EnableStretch | SourceGrid.AutoSizeMode.EnableAutoSize;
	         ReplacementsGrid.Columns[2].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;

	         ReplacementsGrid.Columns[2].Width = 80;

	         ReplacementsGrid.AutoStretchColumnsToFitWidth = true;
	         ReplacementsGrid.Columns.StretchToFit();

	         ReplacementsGrid.Columns[0].Width = ReplacementsGrid.Columns[0].Width - 8; // allow for scrollbar
	         ReplacementsGrid.Columns[1].Width = ReplacementsGrid.Columns[1].Width - 8;

	         //////////////////////////////////////////////////////////////////////
	         // header row

	         SourceGrid.Cells.ColumnHeader h;
	         h = new SourceGrid.Cells.ColumnHeader("Search");
	         h.AutomaticSortEnabled = false;
	         ReplacementsGrid[0,0] = h;
	         ReplacementsGrid[0,0].View = titleModel;

	         h = new SourceGrid.Cells.ColumnHeader("Replace");
	         h.AutomaticSortEnabled = false;
	         ReplacementsGrid[0,1] = h;
	         ReplacementsGrid[0,1].View = titleModel;

	         h = new SourceGrid.Cells.ColumnHeader("Case Ins.");
	         h.AutomaticSortEnabled = false;
	         ReplacementsGrid[0,2] = h;
	         ReplacementsGrid[0,2].View = titleModel;
         }


         private void AddNewReplacementRow(string from, string to, bool ins)
         {
	         SourceGrid.Cells.Views.Cell roModel = new SourceGrid.Cells.Views.Cell();
	         roModel.ForeColor = Color.Gray;

	         int r = ReplacementsGrid.RowsCount;
	         ReplacementsGrid.RowsCount = r + 1;
	         ReplacementsGrid[r, 0] = new SourceGrid.Cells.Cell(from, typeof(string));
             ReplacementsGrid[r, 1] = new SourceGrid.Cells.Cell(to, typeof(string));
	         ReplacementsGrid[r, 2] = new SourceGrid.Cells.CheckBox(null, ins);
	         if (!string.IsNullOrEmpty(from) && (TVSettings.CompulsoryReplacements().IndexOf(from) != -1))
	         {
		         ReplacementsGrid[r,0].Editor.EnableEdit = false;
		         ReplacementsGrid[r,0].View = roModel;
	         }
         }


         private void SetupRSSGrid()
         {
	         SourceGrid.Cells.Views.Cell titleModel = new SourceGrid.Cells.Views.Cell();
	         titleModel.BackColor = Color.SteelBlue;
	         titleModel.ForeColor = Color.White;
	         titleModel.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft;

	         RSSGrid.Columns.Clear();
	         RSSGrid.Rows.Clear();

	         RSSGrid.RowsCount = 1;
	         RSSGrid.ColumnsCount = 1;
	         RSSGrid.FixedRows = 1;
	         RSSGrid.FixedColumns = 0;
	         RSSGrid.Selection.EnableMultiSelection = false;

	         RSSGrid.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;

	         RSSGrid.AutoStretchColumnsToFitWidth = true;
	         RSSGrid.Columns.StretchToFit();

	         //////////////////////////////////////////////////////////////////////
	         // header row

	         SourceGrid.Cells.ColumnHeader h;
	         h = new SourceGrid.Cells.ColumnHeader("URL");
	         h.AutomaticSortEnabled = false;
	         RSSGrid[0,0] = h;
	         RSSGrid[0,0].View = titleModel;
         }

         private void AddNewRSSRow(string text)
         {
	         int r = RSSGrid.RowsCount;
	         RSSGrid.RowsCount = r + 1;
	         RSSGrid[r, 0] = new SourceGrid.Cells.Cell(text, typeof(string));
         }

        private void bnRSSAdd_Click(object sender, System.EventArgs e)
         {
	         AddNewRSSRow(null);
         }

        private void bnRSSRemove_Click(object sender, System.EventArgs e)
         {
	         // multiselection is off, so we can cheat...
	         int[] rowsIndex = RSSGrid.Selection.GetSelectionRegion().GetRowsIndex();
	         if (rowsIndex.Length > 0)
		         RSSGrid.Rows.Remove(rowsIndex[0]);
         }

        private void bnRSSGo_Click(object sender, System.EventArgs e)
         {
	         // multiselection is off, so we can cheat...
	         int[] rowsIndex = RSSGrid.Selection.GetSelectionRegion().GetRowsIndex();

	         if (rowsIndex.Length > 0)
		         TVDoc.SysOpen((string)(RSSGrid[rowsIndex[0],0].Value));
         }

        private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
         {
	         if (tabControl1.SelectedIndex == 5) // gone to languages tab
		         SetupLanguages();
         }

         private void SetupLanguages()
         {
	         TheTVDB db = mDoc.GetTVDB(true, "Preferences-SL");
	         if (!db.Connected)
	         {
		         lbLangs.Items.Add("Please Wait");
		         lbLangs.Items.Add("Connecting...");
		         lbLangs.Update();
		         db.Connect();
	         }

	         // make our list
	         // add already prioritised items (that still exist)
	         LangList = new StringList();
	         foreach (string s in db.LanguagePriorityList)
		         if (db.LanguageList.ContainsKey(s))
			         LangList.Add(s);

	         // add items that haven't been prioritised
	         foreach (System.Collections.Generic.KeyValuePair<string , string > k in db.LanguageList)
		         if (!LangList.Contains(k.Key))
			         LangList.Add(k.Key);
	         db.Unlock("Preferences-SL");

	         FillLanguageList();
         }

         private void FillLanguageList()
         {
	         TheTVDB db = mDoc.GetTVDB(true, "Preferences-FLL");
	         lbLangs.BeginUpdate();
	         lbLangs.Items.Clear();
	         foreach (string l in LangList)
		         lbLangs.Items.Add(db.LanguageList[l]);
	         lbLangs.EndUpdate();
	         db.Unlock("Preferences-FLL");

         }

        private void bnLangDown_Click(object sender, System.EventArgs e)
         {
	         int n = lbLangs.SelectedIndex;
	         if (n == -1)
		         return;

	         if (n < (LangList.Count - 1))
	         {
		         LangList.Reverse(n,2);
		         FillLanguageList();
		         lbLangs.SelectedIndex = n+1;
	         }
         }

        private void bnLangUp_Click(object sender, System.EventArgs e)
         {
	         int n = lbLangs.SelectedIndex;
	         if (n == -1)
		         return;
	         if (n > 0)
	         {
		         LangList.Reverse(n-1,2);
		         FillLanguageList();
		         lbLangs.SelectedIndex = n-1;
	         }
         }

        private void cbMissing_CheckedChanged(object sender, System.EventArgs e)
         {
	         ScanOptEnableDisable();
         }

         private void ScanOptEnableDisable()
         {
	         bool e = cbMissing.Checked;
	         cbSearchRSS.Enabled = e;
	         cbSearchLocally.Enabled = e;
	         cbEpImgs.Enabled = e;
	         cbNFOs.Enabled = e;
	         cbCheckuTorrent.Enabled = e;

	         bool e2 = cbSearchLocally.Checked;
	         cbLeaveOriginals.Enabled = e && e2;
         }

        private void cbSearchLocally_CheckedChanged(object sender, System.EventArgs e)
         {
	         ScanOptEnableDisable();
         }
        private void bnReplaceAdd_Click(object sender, System.EventArgs e)
         {
	         AddNewReplacementRow(null, null, false);
         }
        private void bnReplaceRemove_Click(object sender, System.EventArgs e)
         {
	         // multiselection is off, so we can cheat...
	         int[] rowsIndex = ReplacementsGrid.Selection.GetSelectionRegion().GetRowsIndex();
	         if (rowsIndex.Length > 0)
	         {
		         // don't delete compulsory items
		         int n = rowsIndex[0];
		         string from = (string)(ReplacementsGrid[n,0].Value);
		         if (string.IsNullOrEmpty(from) || (TVSettings.CompulsoryReplacements().IndexOf(from) == -1))
			         ReplacementsGrid.Rows.Remove(n);
	         }
         }
	}
}