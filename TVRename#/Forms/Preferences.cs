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
using TVRename.db_access.documents;
using TVRename.Settings;
using TVRename.Utility;
using TVRename.db_access;
using TVRename.Shows;
using System.Collections.Generic;

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
        private StringList LangList;
        private TVDoc mDoc;

        public Preferences(TVDoc doc, bool goToScanOpts)
        {
            this.LangList = null;

            this.InitializeComponent();

            this.SetupRSSGrid();
            this.SetupReplacementsGrid();

            this.mDoc = doc;

            if (goToScanOpts)
                this.tabControl1.SelectedTab = this.tpScanOptions;
        }

        private void OKButton_Click(object sender, System.EventArgs e)
        {
            if (!FileNameUtility.OKExtensionsString(this.txtVideoExtensions.Text))
            {
                MessageBox.Show("Extensions list must be separated by semicolons, and each extension must start with a dot.", "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.tabControl1.SelectedIndex = 1;
                this.txtVideoExtensions.Focus();
                return;
            }
            if (!FileNameUtility.OKExtensionsString(this.txtOtherExtensions.Text))
            {
                MessageBox.Show("Extensions list must be separated by semicolons, and each extension must start with a dot.", "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.tabControl1.SelectedIndex = 1;
                this.txtOtherExtensions.Focus();
                return;
            }
            Config S = this.mDoc.SettingsObj;
            S.innerDocument.Replacements.Clear();
            for (int i = 1; i < this.ReplacementsGrid.RowsCount; i++)
            {
                string from = (string) (this.ReplacementsGrid[i, 0].Value);
                string to = (string) (this.ReplacementsGrid[i, 1].Value);
                bool ins = (bool) (this.ReplacementsGrid[i, 2].Value);
                if (!string.IsNullOrEmpty(from))
                    S.innerDocument.Replacements.Add(new Replacement(from, to, ins));
            }

            S.innerDocument.ExportWTWRSS = this.cbWTWRSS.Checked;
            S.innerDocument.ExportWTWRSSTo = this.txtWTWRSS.Text;

            S.innerDocument.ExportMissingXML = this.cbMissingXML.Checked;
            S.innerDocument.ExportMissingXMLTo = this.txtMissingXML.Text;
            S.innerDocument.ExportMissingCSV = this.cbMissingCSV.Checked;
            S.innerDocument.ExportMissingCSVTo = this.txtMissingCSV.Text;
            S.innerDocument.ExportRenamingXML = this.cbRenamingXML.Checked;
            S.innerDocument.ExportRenamingXMLTo = this.txtRenamingXML.Text;
            S.innerDocument.ExportFOXML = this.cbFOXML.Checked;
            S.innerDocument.ExportFOXMLTo = this.txtFOXML.Text;

            S.innerDocument.WTWRecentDays = Convert.ToInt32(this.txtWTWDays.Text);
            S.innerDocument.StartupTab = this.cbStartupTab.SelectedIndex;
            S.innerDocument.NotificationAreaIcon = this.cbNotificationIcon.Checked;
            S.innerDocument = DefaultObjectFactory.SetVideoExtensionsString(S.innerDocument,this.txtVideoExtensions.Text);
            S.innerDocument = DefaultObjectFactory.SetOtherExtensionsString(S.innerDocument, this.txtOtherExtensions.Text);
            S.innerDocument.ExportRSSMaxDays = Convert.ToInt32(this.txtExportRSSMaxDays.Text);
            S.innerDocument.ExportRSSMaxShows = Convert.ToInt32(this.txtExportRSSMaxShows.Text);
            S.innerDocument.KeepTogether = this.cbKeepTogether.Checked;
            S.innerDocument.LeadingZeroOnSeason = this.cbLeadingZero.Checked;
            S.innerDocument.ShowInTaskbar = this.chkShowInTaskbar.Checked;
            S.innerDocument.RenameTxtToSub = this.cbTxtToSub.Checked;
            S.innerDocument.ShowEpisodePictures = this.cbShowEpisodePictures.Checked;
            S.innerDocument.AutoSelectShowInMyShows = this.cbAutoSelInMyShows.Checked;
            S.innerDocument.SpecialsFolderName = this.txtSpecialsFolderName.Text;

            S.innerDocument.ForceLowercaseFilenames = this.cbForceLower.Checked;
            S.innerDocument.IgnoreSamples = this.cbIgnoreSamples.Checked;

            S.innerDocument.uTorrentPath = this.txtRSSuTorrentPath.Text;
            S.innerDocument.ResumeDatPath = this.txtUTResumeDatPath.Text;

            S.innerDocument.SearchRSS = this.cbSearchRSS.Checked;
            S.innerDocument.EpImgs = this.cbEpImgs.Checked;
            S.innerDocument.NFOs = this.cbNFOs.Checked;
            S.innerDocument.FolderJpg = this.cbFolderJpg.Checked;
            S.innerDocument.RenameCheck = this.cbRenameCheck.Checked;
            S.innerDocument.MissingCheck = this.cbMissing.Checked;
            S.innerDocument.SearchLocally = this.cbSearchLocally.Checked;
            S.innerDocument.LeaveOriginals = this.cbLeaveOriginals.Checked;
            S.innerDocument.CheckuTorrent = this.cbCheckuTorrent.Checked;
            S.innerDocument.LookForDateInFilename = this.cbLookForAirdate.Checked;
            S.innerDocument.MonitorFolders = this.cbMonitorFolder.Checked;

            if (this.rbFolderFanArt.Checked)
                S.innerDocument.FolderJpgIs = ConfigDocument.FolderJpgIsType.FanArt;
            else if (this.rbFolderBanner.Checked)
                S.innerDocument.FolderJpgIs = ConfigDocument.FolderJpgIsType.Banner;
            else
                S.innerDocument.FolderJpgIs = ConfigDocument.FolderJpgIsType.Poster;

            if (this.LangList != null)
            {
                //only set if the language tab was visited

                TheTVDB db = this.mDoc.GetTVDB(true, "Preferences-OK");
                db.LanguagePriorityList = this.LangList;
                db.SaveCache();
                db.Unlock("Preferences-OK");
            }

            try
            {
                S.innerDocument.SampleFileMaxSizeMB = int.Parse(this.txtMaxSampleSize.Text);
            }
            catch
            {
                S.innerDocument.SampleFileMaxSizeMB = 50;
            }

            try
            {
                S.innerDocument.ParallelDownloads = int.Parse(this.txtParallelDownloads.Text);
            }
            catch
            {
                S.innerDocument.ParallelDownloads = 4;
            }

            if (S.innerDocument.ParallelDownloads < 1)
                S.innerDocument.ParallelDownloads = 1;
            else if (S.innerDocument.ParallelDownloads > 8)
                S.innerDocument.ParallelDownloads = 8;

            // RSS URLs
            S.innerDocument.RSSURLs.Clear();
            for (int i = 1; i < this.RSSGrid.RowsCount; i++)
            {
                string url = (string) (this.RSSGrid[i, 0].Value);
                if (!string.IsNullOrEmpty(url))
                    S.innerDocument.RSSURLs.Add(url);
            }

            S.innerDocument.ShowStatusColors = new ShowStatusColoringTypeList();
            foreach (ListViewItem item in lvwDefinedColors.Items)
            {
                if (item.SubItems.Count > 1 && !string.IsNullOrEmpty(item.SubItems[1].Text) && item.Tag != null && item.Tag is ShowStatusColoringType)
                {
                    S.innerDocument.ShowStatusColors.Add(item.Tag as ShowStatusColoringType, System.Drawing.ColorTranslator.FromHtml(item.SubItems[1].Text));
                }
            }

            this.mDoc.SetDirty();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Preferences_Load(object sender, System.EventArgs e)
        {
            Config S = this.mDoc.SettingsObj;
            int r = 1;

            foreach (Replacement R in S.innerDocument.Replacements)
            {
                this.AddNewReplacementRow(R.This, R.That, R.CaseInsensitive);
                r++;
            }

            this.txtMaxSampleSize.Text = S.innerDocument.SampleFileMaxSizeMB.ToString();

            this.cbWTWRSS.Checked = S.innerDocument.ExportWTWRSS;
            this.txtWTWRSS.Text = S.innerDocument.ExportWTWRSSTo;
            this.txtWTWDays.Text = S.innerDocument.WTWRecentDays.ToString();
            this.txtExportRSSMaxDays.Text = S.innerDocument.ExportRSSMaxDays.ToString();
            this.txtExportRSSMaxShows.Text = S.innerDocument.ExportRSSMaxShows.ToString();

            this.cbMissingXML.Checked = S.innerDocument.ExportMissingXML;
            this.txtMissingXML.Text = S.innerDocument.ExportMissingXMLTo;
            this.cbMissingCSV.Checked = S.innerDocument.ExportMissingCSV;
            this.txtMissingCSV.Text = S.innerDocument.ExportMissingCSVTo;

            this.cbRenamingXML.Checked = S.innerDocument.ExportRenamingXML;
            this.txtRenamingXML.Text = S.innerDocument.ExportRenamingXMLTo;

            this.cbFOXML.Checked = S.innerDocument.ExportFOXML;
            this.txtFOXML.Text = S.innerDocument.ExportFOXMLTo;

            this.cbStartupTab.SelectedIndex = S.innerDocument.StartupTab;
            this.cbNotificationIcon.Checked = S.innerDocument.NotificationAreaIcon;
            this.txtVideoExtensions.Text = S.innerDocument.GetVideoExtensionsString();
            this.txtOtherExtensions.Text = S.innerDocument.GetOtherExtensionsString();

            this.cbKeepTogether.Checked = S.innerDocument.KeepTogether;
            this.cbKeepTogether_CheckedChanged(null, null);

            this.cbLeadingZero.Checked = S.innerDocument.LeadingZeroOnSeason;
            this.chkShowInTaskbar.Checked = S.innerDocument.ShowInTaskbar;
            this.cbTxtToSub.Checked = S.innerDocument.RenameTxtToSub;
            this.cbShowEpisodePictures.Checked = S.innerDocument.ShowEpisodePictures;
            this.cbAutoSelInMyShows.Checked = S.innerDocument.AutoSelectShowInMyShows;
            this.txtSpecialsFolderName.Text = S.innerDocument.SpecialsFolderName;
            this.cbForceLower.Checked = S.innerDocument.ForceLowercaseFilenames;
            this.cbIgnoreSamples.Checked = S.innerDocument.IgnoreSamples;
            this.txtRSSuTorrentPath.Text = S.innerDocument.uTorrentPath;
            this.txtUTResumeDatPath.Text = S.innerDocument.ResumeDatPath;

            this.txtParallelDownloads.Text = S.innerDocument.ParallelDownloads.ToString();

            this.cbSearchRSS.Checked = S.innerDocument.SearchRSS;
            this.cbEpImgs.Checked = S.innerDocument.EpImgs;
            this.cbNFOs.Checked = S.innerDocument.NFOs;
            this.cbFolderJpg.Checked = S.innerDocument.FolderJpg;
            this.cbRenameCheck.Checked = S.innerDocument.RenameCheck;
            this.cbCheckuTorrent.Checked = S.innerDocument.CheckuTorrent;
            this.cbLookForAirdate.Checked = S.innerDocument.LookForDateInFilename;
            this.cbMonitorFolder.Checked = S.innerDocument.MonitorFolders;
            this.cbMissing.Checked = S.innerDocument.MissingCheck;
            this.cbSearchLocally.Checked = S.innerDocument.SearchLocally;
            this.cbLeaveOriginals.Checked = S.innerDocument.LeaveOriginals;

            this.EnableDisable(null, null);

            this.FillSearchFolderList();

            foreach (string s in S.innerDocument.RSSURLs)
                this.AddNewRSSRow(s);

            switch (S.innerDocument.FolderJpgIs)
            {
                case ConfigDocument.FolderJpgIsType.FanArt:
                    this.rbFolderFanArt.Checked = true;
                    break;
                case ConfigDocument.FolderJpgIsType.Banner:
                    this.rbFolderBanner.Checked = true;
                    break;
                default:
                    this.rbFolderPoster.Checked = true;
                    break;
            }
            if (S.innerDocument.ShowStatusColors != null)
            {
                foreach (System.Collections.Generic.KeyValuePair<ShowStatusColoringType, Color> showStatusColor in S.innerDocument.ShowStatusColors)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = showStatusColor.Key.Text;
                    item.Tag = showStatusColor.Key;
                    item.SubItems.Add(TranslateColorToHtml(showStatusColor.Value));
                    item.ForeColor = showStatusColor.Value;
                    this.lvwDefinedColors.Items.Add(item);
                }
            }

            FillTreeViewColoringShowStatusTypeCombobox();
        }

        

        private void FillTreeViewColoringShowStatusTypeCombobox()
        {
            //System.Collections.Generic.KeyValuePair<string, object> item = new System.Collections.Generic.KeyValuePair<string, object>();
            // Shows
            foreach (string status in Enum.GetNames(typeof(ShowItem.ShowAirStatus)))
            {
                ShowStatusColoringType t = new ShowStatusColoringType(true, true, status);
                //System.Collections.Generic.KeyValuePair<string, object> item = new System.Collections.Generic.KeyValuePair<string, object>("Show Seasons Status: " + status, new ShowStatusColoringType(true, true, status));
                this.cboShowStatus.Items.Add(t);
                //this.cboShowStatus.Items.Add("Show Seasons Status: " + status);
            }
            System.Collections.Generic.List<string> showStatusList = new System.Collections.Generic.List<string>();
            List<ShowItem> shows = this.mDoc.GetShowItems(false);
            foreach (var show in shows)
            {
                if(!showStatusList.Contains(show.ShowStatus))
                    showStatusList.Add(show.ShowStatus);
            }
            foreach (string status in showStatusList)
            {
                ShowStatusColoringType t = new ShowStatusColoringType(false, true, status);
                //System.Collections.Generic.KeyValuePair<string, object> item = new System.Collections.Generic.KeyValuePair<string, object>("Show  Status: " + status, new ShowStatusColoringType(false, true, status));
                this.cboShowStatus.Items.Add(t);
            }
            //this.cboShowStatus.Items.Add(new System.Collections.Generic.KeyValuePair<string, object>("Show Seasons Status: Custom", null));
            // Seasons
            foreach (string status in Enum.GetNames(typeof(Season.SeasonStatus)))
            {
                ShowStatusColoringType t = new ShowStatusColoringType(true, false, status);
                //System.Collections.Generic.KeyValuePair<string, object> item = new System.Collections.Generic.KeyValuePair<string, object>("Seasons Status: " + status, new ShowStatusColoringType(true, false, status));
                this.cboShowStatus.Items.Add(t);
                //this.cboShowStatus.Items.Add("Seasons Status: " + status);
            }
            this.cboShowStatus.DisplayMember = "Text";
            //this.cboShowStatus.ValueMember = ";
        }

        private void Browse(TextBox txt)
        {
            this.saveFile.FileName = txt.Text;
            if (this.saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txt.Text = this.saveFile.FileName;
        }

        private void bnBrowseWTWRSS_Click(object sender, System.EventArgs e)
        {
            this.Browse(this.txtWTWRSS);
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
            if (!this.cbNotificationIcon.Checked)
                this.chkShowInTaskbar.Checked = true;
        }

        private void chkShowInTaskbar_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!this.chkShowInTaskbar.Checked)
                this.cbNotificationIcon.Checked = true;
        }

        private void cbKeepTogether_CheckedChanged(object sender, System.EventArgs e)
        {
            this.cbTxtToSub.Enabled = this.cbKeepTogether.Checked;
        }

        private void bnBrowseMissingCSV_Click(object sender, System.EventArgs e)
        {
            this.Browse(this.txtMissingCSV);
        }

        private void bnBrowseMissingXML_Click(object sender, System.EventArgs e)
        {
            this.Browse(this.txtMissingXML);
        }

        private void bnBrowseRenamingXML_Click(object sender, System.EventArgs e)
        {
            this.Browse(this.txtRenamingXML);
        }

        private void bnBrowseFOXML_Click(object sender, System.EventArgs e)
        {
            this.Browse(this.txtFOXML);
        }

        private void EnableDisable(object sender, System.EventArgs e)
        {
            bool wtw = this.cbWTWRSS.Checked;
            this.txtWTWRSS.Enabled = wtw;
            this.bnBrowseWTWRSS.Enabled = wtw;
            this.label15.Enabled = wtw;
            this.label16.Enabled = wtw;
            this.label17.Enabled = wtw;
            this.txtExportRSSMaxDays.Enabled = wtw;
            this.txtExportRSSMaxShows.Enabled = wtw;

            bool fo = this.cbFOXML.Checked;
            this.txtFOXML.Enabled = fo;
            this.bnBrowseFOXML.Enabled = fo;

            bool ren = this.cbRenamingXML.Checked;
            this.txtRenamingXML.Enabled = ren;
            this.bnBrowseRenamingXML.Enabled = ren;

            bool misx = this.cbMissingXML.Checked;
            this.txtMissingXML.Enabled = misx;
            this.bnBrowseMissingXML.Enabled = misx;

            bool misc = this.cbMissingCSV.Checked;
            this.txtMissingCSV.Enabled = misc;
            this.bnBrowseMissingCSV.Enabled = misc;
        }

        private void bnAddSearchFolder_Click(object sender, System.EventArgs e)
        {
            int n = this.lbSearchFolders.SelectedIndex;
            if (n != -1)
                this.folderBrowser.SelectedPath = this.mDoc.SettingsObj.innerDocument.SearchFoldersList[n];
            else
                this.folderBrowser.SelectedPath = "";

            if (this.folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.mDoc.SettingsObj.innerDocument.SearchFoldersList.Add(this.folderBrowser.SelectedPath);
                this.mDoc.SetDirty();
            }

            this.FillSearchFolderList();
        }

        private void bnRemoveSearchFolder_Click(object sender, System.EventArgs e)
        {
            int n = this.lbSearchFolders.SelectedIndex;
            if (n == -1)
                return;

            this.mDoc.SettingsObj.innerDocument.SearchFoldersList.RemoveAt(n);
            this.mDoc.SetDirty();

            this.FillSearchFolderList();
        }

        private void bnOpenSearchFolder_Click(object sender, System.EventArgs e)
        {
            int n = this.lbSearchFolders.SelectedIndex;
            if (n == -1)
                return;
            TVDoc.SysOpen(this.mDoc.SettingsObj.innerDocument.SearchFoldersList[n]);
        }

        private void FillSearchFolderList()
        {
            this.lbSearchFolders.Items.Clear();
            this.mDoc.SettingsObj.innerDocument.SearchFoldersList.Sort();
            foreach (string efi in this.mDoc.SettingsObj.innerDocument.SearchFoldersList)
                this.lbSearchFolders.Items.Add(efi);
        }

        private void lbSearchFolders_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                this.bnRemoveSearchFolder_Click(null, null);
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
            string[] files = (string[]) (e.Data.GetData(DataFormats.FileDrop));
            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i];
                DirectoryInfo di;
                try
                {
                    di = new DirectoryInfo(path);
                    if (di.Exists)
                        this.mDoc.SettingsObj.innerDocument.SearchFoldersList.Add(path.ToLower());
                }
                catch
                {
                }
            }
            this.mDoc.SetDirty();
            this.FillSearchFolderList();
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
            this.openFile.FileName = this.txtRSSuTorrentPath.Text;
            this.openFile.Filter = "utorrent.exe|utorrent.exe|All Files (*.*)|*.*";
            if (this.openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txtRSSuTorrentPath.Text = this.openFile.FileName;
        }

        private void bnUTBrowseResumeDat_Click(object sender, System.EventArgs e)
        {
            this.openFile.FileName = this.txtUTResumeDatPath.Text;
            this.openFile.Filter = "resume.dat|resume.dat|All Files (*.*)|*.*";
            if (this.openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txtUTResumeDatPath.Text = this.openFile.FileName;
        }

        private void SetupReplacementsGrid()
        {
            SourceGrid.Cells.Views.Cell titleModel = new SourceGrid.Cells.Views.Cell();
            titleModel.BackColor = Color.SteelBlue;
            titleModel.ForeColor = Color.White;
            titleModel.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft;

            this.ReplacementsGrid.Columns.Clear();
            this.ReplacementsGrid.Rows.Clear();

            this.ReplacementsGrid.RowsCount = 1;
            this.ReplacementsGrid.ColumnsCount = 3;
            this.ReplacementsGrid.FixedRows = 1;
            this.ReplacementsGrid.FixedColumns = 0;
            this.ReplacementsGrid.Selection.EnableMultiSelection = false;

            this.ReplacementsGrid.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.EnableStretch | SourceGrid.AutoSizeMode.EnableAutoSize;
            this.ReplacementsGrid.Columns[1].AutoSizeMode = SourceGrid.AutoSizeMode.EnableStretch | SourceGrid.AutoSizeMode.EnableAutoSize;
            this.ReplacementsGrid.Columns[2].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;

            this.ReplacementsGrid.Columns[2].Width = 80;

            this.ReplacementsGrid.AutoStretchColumnsToFitWidth = true;
            this.ReplacementsGrid.Columns.StretchToFit();

            this.ReplacementsGrid.Columns[0].Width = this.ReplacementsGrid.Columns[0].Width - 8; // allow for scrollbar
            this.ReplacementsGrid.Columns[1].Width = this.ReplacementsGrid.Columns[1].Width - 8;

            //////////////////////////////////////////////////////////////////////
            // header row

            SourceGrid.Cells.ColumnHeader h;
            h = new SourceGrid.Cells.ColumnHeader("Search");
            h.AutomaticSortEnabled = false;
            this.ReplacementsGrid[0, 0] = h;
            this.ReplacementsGrid[0, 0].View = titleModel;

            h = new SourceGrid.Cells.ColumnHeader("Replace");
            h.AutomaticSortEnabled = false;
            this.ReplacementsGrid[0, 1] = h;
            this.ReplacementsGrid[0, 1].View = titleModel;

            h = new SourceGrid.Cells.ColumnHeader("Case Ins.");
            h.AutomaticSortEnabled = false;
            this.ReplacementsGrid[0, 2] = h;
            this.ReplacementsGrid[0, 2].View = titleModel;
        }

        private void AddNewReplacementRow(string from, string to, bool ins)
        {
            SourceGrid.Cells.Views.Cell roModel = new SourceGrid.Cells.Views.Cell();
            roModel.ForeColor = Color.Gray;

            int r = this.ReplacementsGrid.RowsCount;
            this.ReplacementsGrid.RowsCount = r + 1;
            this.ReplacementsGrid[r, 0] = new SourceGrid.Cells.Cell(from, typeof(string));
            this.ReplacementsGrid[r, 1] = new SourceGrid.Cells.Cell(to, typeof(string));
            this.ReplacementsGrid[r, 2] = new SourceGrid.Cells.CheckBox(null, ins);
            if (!string.IsNullOrEmpty(from) && (ConfigDocument.CompulsoryReplacements().IndexOf(from) != -1))
            {
                this.ReplacementsGrid[r, 0].Editor.EnableEdit = false;
                this.ReplacementsGrid[r, 0].View = roModel;
            }
        }

        private void SetupRSSGrid()
        {
            SourceGrid.Cells.Views.Cell titleModel = new SourceGrid.Cells.Views.Cell();
            titleModel.BackColor = Color.SteelBlue;
            titleModel.ForeColor = Color.White;
            titleModel.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft;

            this.RSSGrid.Columns.Clear();
            this.RSSGrid.Rows.Clear();

            this.RSSGrid.RowsCount = 1;
            this.RSSGrid.ColumnsCount = 1;
            this.RSSGrid.FixedRows = 1;
            this.RSSGrid.FixedColumns = 0;
            this.RSSGrid.Selection.EnableMultiSelection = false;

            this.RSSGrid.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;

            this.RSSGrid.AutoStretchColumnsToFitWidth = true;
            this.RSSGrid.Columns.StretchToFit();

            //////////////////////////////////////////////////////////////////////
            // header row

            SourceGrid.Cells.ColumnHeader h;
            h = new SourceGrid.Cells.ColumnHeader("URL");
            h.AutomaticSortEnabled = false;
            this.RSSGrid[0, 0] = h;
            this.RSSGrid[0, 0].View = titleModel;
        }

        private void AddNewRSSRow(string text)
        {
            int r = this.RSSGrid.RowsCount;
            this.RSSGrid.RowsCount = r + 1;
            this.RSSGrid[r, 0] = new SourceGrid.Cells.Cell(text, typeof(string));
        }

        private void bnRSSAdd_Click(object sender, System.EventArgs e)
        {
            this.AddNewRSSRow(null);
        }

        private void bnRSSRemove_Click(object sender, System.EventArgs e)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = this.RSSGrid.Selection.GetSelectionRegion().GetRowsIndex();
            if (rowsIndex.Length > 0)
                this.RSSGrid.Rows.Remove(rowsIndex[0]);
        }

        private void bnRSSGo_Click(object sender, System.EventArgs e)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = this.RSSGrid.Selection.GetSelectionRegion().GetRowsIndex();

            if (rowsIndex.Length > 0)
                TVDoc.SysOpen((string) (this.RSSGrid[rowsIndex[0], 0].Value));
        }

        private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 5) // gone to languages tab
                this.SetupLanguages();
        }

        private void SetupLanguages()
        {
            TheTVDB db = this.mDoc.GetTVDB(true, "Preferences-SL");
            if (!db.Connected)
            {
                this.lbLangs.Items.Add("Please Wait");
                this.lbLangs.Items.Add("Connecting...");
                this.lbLangs.Update();
                db.Connect();
            }

            // make our list
            // add already prioritised items (that still exist)
            this.LangList = new StringList();
            foreach (string s in db.LanguagePriorityList)
            {
                if (db.LanguageList.ContainsKey(s))
                    this.LangList.Add(s);
            }

            // add items that haven't been prioritised
            foreach (System.Collections.Generic.KeyValuePair<string, string> k in db.LanguageList)
            {
                if (!this.LangList.Contains(k.Key))
                    this.LangList.Add(k.Key);
            }
            db.Unlock("Preferences-SL");

            this.FillLanguageList();
        }

        private void FillLanguageList()
        {
            TheTVDB db = this.mDoc.GetTVDB(true, "Preferences-FLL");
            this.lbLangs.BeginUpdate();
            this.lbLangs.Items.Clear();
            foreach (string l in this.LangList)
                this.lbLangs.Items.Add(db.LanguageList[l]);
            this.lbLangs.EndUpdate();
            db.Unlock("Preferences-FLL");
        }

        private void bnLangDown_Click(object sender, System.EventArgs e)
        {
            int n = this.lbLangs.SelectedIndex;
            if (n == -1)
                return;

            if (n < (this.LangList.Count - 1))
            {
                this.LangList.Reverse(n, 2);
                this.FillLanguageList();
                this.lbLangs.SelectedIndex = n + 1;
            }
        }

        private void bnLangUp_Click(object sender, System.EventArgs e)
        {
            int n = this.lbLangs.SelectedIndex;
            if (n == -1)
                return;
            if (n > 0)
            {
                this.LangList.Reverse(n - 1, 2);
                this.FillLanguageList();
                this.lbLangs.SelectedIndex = n - 1;
            }
        }

        private void cbMissing_CheckedChanged(object sender, System.EventArgs e)
        {
            this.ScanOptEnableDisable();
        }

        private void ScanOptEnableDisable()
        {
            bool e = this.cbMissing.Checked;
            this.cbSearchRSS.Enabled = e;
            this.cbSearchLocally.Enabled = e;
            this.cbEpImgs.Enabled = e;
            this.cbNFOs.Enabled = e;
            this.cbCheckuTorrent.Enabled = e;

            bool e2 = this.cbSearchLocally.Checked;
            this.cbLeaveOriginals.Enabled = e && e2;
        }

        private void cbSearchLocally_CheckedChanged(object sender, System.EventArgs e)
        {
            this.ScanOptEnableDisable();
        }

        private void bnReplaceAdd_Click(object sender, System.EventArgs e)
        {
            this.AddNewReplacementRow(null, null, false);
        }

        private void bnReplaceRemove_Click(object sender, System.EventArgs e)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = this.ReplacementsGrid.Selection.GetSelectionRegion().GetRowsIndex();
            if (rowsIndex.Length > 0)
            {
                // don't delete compulsory items
                int n = rowsIndex[0];
                string from = (string) (this.ReplacementsGrid[n, 0].Value);
                if (string.IsNullOrEmpty(from) || (ConfigDocument.CompulsoryReplacements().IndexOf(from) == -1))
                    this.ReplacementsGrid.Rows.Remove(n);
            }
        }

        private void btnAddShowStatusColoring_Click(object sender, EventArgs e)
        {
            if (this.cboShowStatus.SelectedItem != null && !string.IsNullOrEmpty(this.txtShowStatusColor.Text))
            {
                try
                {
                    ShowStatusColoringType ssct = this.cboShowStatus.SelectedItem as ShowStatusColoringType;
                    if (!ColorTranslator.FromHtml(this.txtShowStatusColor.Text).IsEmpty)
                    {
                        ListViewItem item = null;
                        item  = this.lvwDefinedColors.FindItemWithText(ssct.Text) ;
                        if(item == null)
                        {
                            item = new ListViewItem();
                            item.SubItems.Add(this.txtShowStatusColor.Text);
                            this.lvwDefinedColors.Items.Add(item);
                        }

                        item.Text = ssct.Text;
                        item.SubItems[1].Text = this.txtShowStatusColor.Text;
                        item.ForeColor = ColorTranslator.FromHtml(this.txtShowStatusColor.Text);
                        item.Tag = ssct;
                        this.txtShowStatusColor.Text = string.Empty;
                        this.txtShowStatusColor.ForeColor = Color.Black;
                    }
                }
                catch { }
            }
        }

        private void btnSelectColor_Click(object sender, EventArgs e)
        {
            try
            {
                colorDialog.Color = System.Drawing.ColorTranslator.FromHtml(this.txtShowStatusColor.Text);
            }
            catch
            {
                colorDialog.Color = Color.Black;
            }
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
            {
                this.txtShowStatusColor.Text =  TranslateColorToHtml(colorDialog.Color);
                this.txtShowStatusColor.ForeColor = colorDialog.Color;
            }
        }

        string TranslateColorToHtml(Color c)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B);
        }

        private void lvwDefinedColors_DoubleClick(object sender, EventArgs e)
        {
            RemoveSelectedDefinedColor();
        }

        private void bnRemoveDefinedColor_Click(object sender, EventArgs e)
        {
            RemoveSelectedDefinedColor();
        }

        private void lvwDefinedColors_SelectedIndexChanged(object sender, EventArgs e)
        {
            bnRemoveDefinedColor.Enabled = this.lvwDefinedColors.SelectedItems.Count == 1;
        }

        private void RemoveSelectedDefinedColor()
        {
            if (this.lvwDefinedColors.SelectedItems.Count == 1)
            {
                this.lvwDefinedColors.Items.Remove(this.lvwDefinedColors.SelectedItems[0]);
            }
        }

        private void txtShowStatusColor_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.txtShowStatusColor.ForeColor = ColorTranslator.FromHtml(this.txtShowStatusColor.Text);
            }
            catch
            {
                this.txtShowStatusColor.ForeColor = Color.Black;
            }
        }

    }
}