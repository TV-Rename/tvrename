// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using ColumnHeader = SourceGrid.Cells.ColumnHeader;

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
        delegate void LoadLanguageDoneDel();

        private TVDoc mDoc;
        private Thread LoadLanguageThread;
        private String EnterPreferredLanguage; // hold here until background language download task is done

        private LoadLanguageDoneDel LoadLanguageDone;

        public Preferences(TVDoc doc, bool goToScanOpts)
        {
            this.InitializeComponent();
            this.LoadLanguageDone += this.LoadLanguageDoneFunc;

            this.SetupRSSGrid();
            this.SetupReplacementsGrid();

            this.mDoc = doc;

            if (goToScanOpts)
                this.tabControl1.SelectedTab = this.tpScanOptions;
        }

        private void OKButton_Click(object sender, System.EventArgs e)
        {
            if (!TVSettings.OKExtensionsString(this.txtVideoExtensions.Text))
            {
                MessageBox.Show("Extensions list must be separated by semicolons, and each extension must start with a dot.", "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.tabControl1.SelectedIndex = 1;
                this.txtVideoExtensions.Focus();
                return;
            }
            if (!TVSettings.OKExtensionsString(this.txtOtherExtensions.Text))
            {
                MessageBox.Show("Extensions list must be separated by semicolons, and each extension must start with a dot.", "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.tabControl1.SelectedIndex = 1;
                this.txtOtherExtensions.Focus();
                return;
            }
            TVSettings S = this.mDoc.Settings;
            S.Replacements.Clear();
            for (int i = 1; i < this.ReplacementsGrid.RowsCount; i++)
            {
                string from = (string) (this.ReplacementsGrid[i, 0].Value);
                string to = (string) (this.ReplacementsGrid[i, 1].Value);
                bool ins = (bool) (this.ReplacementsGrid[i, 2].Value);
                if (!string.IsNullOrEmpty(from))
                    S.Replacements.Add(new Replacement(from, to, ins));
            }

            S.ExportWTWRSS = this.cbWTWRSS.Checked;
            S.ExportWTWRSSTo = this.txtWTWRSS.Text;
            S.ExportWTWXML = this.cbWTWXML.Checked;
            S.ExportWTWXMLTo = this.txtWTWXML.Text;
            S.ExportMissingXML = this.cbMissingXML.Checked;
            S.ExportMissingXMLTo = this.txtMissingXML.Text;
            S.ExportMissingCSV = this.cbMissingCSV.Checked;
            S.ExportMissingCSVTo = this.txtMissingCSV.Text;
            S.ExportRenamingXML = this.cbRenamingXML.Checked;
            S.ExportRenamingXMLTo = this.txtRenamingXML.Text;
            S.ExportFOXML = this.cbFOXML.Checked;
            S.ExportFOXMLTo = this.txtFOXML.Text;

            S.WTWRecentDays = Convert.ToInt32(this.txtWTWDays.Text);
            S.StartupTab = this.cbStartupTab.SelectedIndex;
            S.NotificationAreaIcon = this.cbNotificationIcon.Checked;
            S.VideoExtensionsString = this.txtVideoExtensions.Text;
            S.OtherExtensionsString = this.txtOtherExtensions.Text;
            S.ExportRSSMaxDays = Convert.ToInt32(this.txtExportRSSMaxDays.Text);
            S.ExportRSSMaxShows = Convert.ToInt32(this.txtExportRSSMaxShows.Text);
            S.ExportRSSDaysPast = Convert.ToInt32(this.txtExportRSSDaysPast.Text); 
            S.KeepTogether = this.cbKeepTogether.Checked;
            S.LeadingZeroOnSeason = this.cbLeadingZero.Checked;
            S.ShowInTaskbar = this.chkShowInTaskbar.Checked;
            S.RenameTxtToSub = this.cbTxtToSub.Checked;
            S.ShowEpisodePictures = this.cbShowEpisodePictures.Checked;
            S.AutoSelectShowInMyShows = this.cbAutoSelInMyShows.Checked;
            S.SpecialsFolderName = this.txtSpecialsFolderName.Text;

            S.ForceLowercaseFilenames = this.cbForceLower.Checked;
            S.IgnoreSamples = this.cbIgnoreSamples.Checked;

            S.uTorrentPath = this.txtRSSuTorrentPath.Text;
            S.ResumeDatPath = this.txtUTResumeDatPath.Text;
            S.SABHostPort = this.txtSABHostPort.Text;
            S.SABAPIKey = this.txtSABAPIKey.Text;
            S.CheckSABnzbd = this.cbCheckSABnzbd.Checked;

            S.SearchRSS = this.cbSearchRSS.Checked;
            S.EpImgs = this.cbEpImgs.Checked;
            S.NFOs = this.cbNFOs.Checked;
            S.pyTivoMeta = this.cbMeta.Checked;
            S.pyTivoMetaSubFolder = this.cbMetaSubfolder.Checked;
            S.FolderJpg = this.cbFolderJpg.Checked;
            S.RenameCheck = this.cbRenameCheck.Checked;
            S.MissingCheck = this.cbMissing.Checked;
            S.SearchLocally = this.cbSearchLocally.Checked;
            S.LeaveOriginals = this.cbLeaveOriginals.Checked;
            S.CheckuTorrent = this.cbCheckuTorrent.Checked;
            S.LookForDateInFilename = this.cbLookForAirdate.Checked;
            S.MonitorFolders = this.cbMonitorFolder.Checked;

            if (this.rbFolderFanArt.Checked)
                S.FolderJpgIs = TVSettings.FolderJpgIsType.FanArt;
            else if (this.rbFolderBanner.Checked)
                S.FolderJpgIs = TVSettings.FolderJpgIsType.Banner;
            else
                S.FolderJpgIs = TVSettings.FolderJpgIsType.Poster;


            TheTVDB db = this.mDoc.GetTVDB(true, "Preferences-OK");
            foreach (var kvp in db.LanguageList)
            {
                if (kvp.Value == cbLanguages.Text)
                {
                    S.PreferredLanguage = kvp.Key;
                    break;
                }
            }
            if (rbWTWScan.Checked)
                S.WTWDoubleClick = TVSettings.WTWDoubleClickAction.Scan;
            else
                S.WTWDoubleClick = TVSettings.WTWDoubleClickAction.Search;

            db.SaveCache();
            db.Unlock("Preferences-OK");
            
            try
            {
                S.SampleFileMaxSizeMB = int.Parse(this.txtMaxSampleSize.Text);
            }
            catch
            {
                S.SampleFileMaxSizeMB = 50;
            }

            try
            {
                S.ParallelDownloads = int.Parse(this.txtParallelDownloads.Text);
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
            for (int i = 1; i < this.RSSGrid.RowsCount; i++)
            {
                string url = (string) (this.RSSGrid[i, 0].Value);
                if (!string.IsNullOrEmpty(url))
                    S.RSSURLs.Add(url);
            }

            S.ShowStatusColors = new ShowStatusColoringTypeList();
            foreach (ListViewItem item in lvwDefinedColors.Items)
            {
                if (item.SubItems.Count > 1 && !string.IsNullOrEmpty(item.SubItems[1].Text) && item.Tag != null && item.Tag is ShowStatusColoringType)
                {
                    S.ShowStatusColors.Add(item.Tag as ShowStatusColoringType, System.Drawing.ColorTranslator.FromHtml(item.SubItems[1].Text));
                }
            }

            this.mDoc.SetDirty();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Preferences_Load(object sender, System.EventArgs e)
        {
            this.SetupLanguages();

            TVSettings S = this.mDoc.Settings;
            int r = 1;

            foreach (Replacement R in S.Replacements)
            {
                this.AddNewReplacementRow(R.This, R.That, R.CaseInsensitive);
                r++;
            }

            this.txtMaxSampleSize.Text = S.SampleFileMaxSizeMB.ToString();

            this.cbWTWRSS.Checked = S.ExportWTWRSS;
            this.txtWTWRSS.Text = S.ExportWTWRSSTo;
            this.txtWTWDays.Text = S.WTWRecentDays.ToString();
            this.cbWTWXML.Checked = S.ExportWTWXML;
            this.txtWTWXML.Text = S.ExportWTWXMLTo;
            this.txtExportRSSMaxDays.Text = S.ExportRSSMaxDays.ToString();
            this.txtExportRSSMaxShows.Text = S.ExportRSSMaxShows.ToString();
            this.txtExportRSSDaysPast.Text = S.ExportRSSDaysPast.ToString();

            this.cbMissingXML.Checked = S.ExportMissingXML;
            this.txtMissingXML.Text = S.ExportMissingXMLTo;
            this.cbMissingCSV.Checked = S.ExportMissingCSV;
            this.txtMissingCSV.Text = S.ExportMissingCSVTo;

            this.cbRenamingXML.Checked = S.ExportRenamingXML;
            this.txtRenamingXML.Text = S.ExportRenamingXMLTo;

            this.cbFOXML.Checked = S.ExportFOXML;
            this.txtFOXML.Text = S.ExportFOXMLTo;

            this.cbStartupTab.SelectedIndex = S.StartupTab;
            this.cbNotificationIcon.Checked = S.NotificationAreaIcon;
            this.txtVideoExtensions.Text = S.GetVideoExtensionsString();
            this.txtOtherExtensions.Text = S.GetOtherExtensionsString();

            this.cbKeepTogether.Checked = S.KeepTogether;
            this.cbKeepTogether_CheckedChanged(null, null);

            this.cbLeadingZero.Checked = S.LeadingZeroOnSeason;
            this.chkShowInTaskbar.Checked = S.ShowInTaskbar;
            this.cbTxtToSub.Checked = S.RenameTxtToSub;
            this.cbShowEpisodePictures.Checked = S.ShowEpisodePictures;
            this.cbAutoSelInMyShows.Checked = S.AutoSelectShowInMyShows;
            this.txtSpecialsFolderName.Text = S.SpecialsFolderName;
            this.cbForceLower.Checked = S.ForceLowercaseFilenames;
            this.cbIgnoreSamples.Checked = S.IgnoreSamples;
            this.txtRSSuTorrentPath.Text = S.uTorrentPath;
            this.txtUTResumeDatPath.Text = S.ResumeDatPath;
            this.txtSABHostPort.Text = S.SABHostPort;
            this.txtSABAPIKey.Text = S.SABAPIKey;
            this.cbCheckSABnzbd.Checked = S.CheckSABnzbd;

            this.txtParallelDownloads.Text = S.ParallelDownloads.ToString();

            this.cbSearchRSS.Checked = S.SearchRSS;
            this.cbEpImgs.Checked = S.EpImgs;
            this.cbNFOs.Checked = S.NFOs;
            this.cbMeta.Checked = S.pyTivoMeta;
            this.cbMetaSubfolder.Checked = S.pyTivoMetaSubFolder;
            this.cbFolderJpg.Checked = S.FolderJpg;
            this.cbRenameCheck.Checked = S.RenameCheck;
            this.cbCheckuTorrent.Checked = S.CheckuTorrent;
            this.cbLookForAirdate.Checked = S.LookForDateInFilename;
            this.cbMonitorFolder.Checked = S.MonitorFolders;
            this.cbMissing.Checked = S.MissingCheck;
            this.cbSearchLocally.Checked = S.SearchLocally;
            this.cbLeaveOriginals.Checked = S.LeaveOriginals;
            EnterPreferredLanguage = S.PreferredLanguage;

            switch (S.WTWDoubleClick)
            {
                case TVSettings.WTWDoubleClickAction.Search:
                default:
                    this.rbWTWSearch.Checked = true;
                    break;
                case TVSettings.WTWDoubleClickAction.Scan:
                    this.rbWTWScan.Checked = true;
                    break;
            }

            this.EnableDisable(null, null);
            this.ScanOptEnableDisable();

            this.FillSearchFolderList();

            foreach (string s in S.RSSURLs)
                this.AddNewRSSRow(s);

            switch (S.FolderJpgIs)
            {
                case TVSettings.FolderJpgIsType.FanArt:
                    this.rbFolderFanArt.Checked = true;
                    break;
                case TVSettings.FolderJpgIsType.Banner:
                    this.rbFolderBanner.Checked = true;
                    break;
                default:
                    this.rbFolderPoster.Checked = true;
                    break;
            }
            if (S.ShowStatusColors != null)
            {
                foreach (System.Collections.Generic.KeyValuePair<ShowStatusColoringType, Color> showStatusColor in S.ShowStatusColors)
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

        private void bnBrowseWTWXML_Click(object sender, System.EventArgs e)
        {
            this.Browse(this.txtWTWXML);
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
            this.txtWTWRSS.Enabled = this.cbWTWRSS.Checked;
            this.bnBrowseWTWRSS.Enabled = this.cbWTWRSS.Checked;

            this.txtWTWXML.Enabled = this.cbWTWXML.Checked;
            this.bnBrowseWTWXML.Enabled = this.cbWTWXML.Checked;

            bool wtw;
            if ((this.cbWTWRSS.Checked) || (this.cbWTWXML.Checked))
                wtw = true;
            else
                wtw = false;

            this.label4.Enabled = wtw;
            this.label15.Enabled = wtw;
            this.label16.Enabled = wtw;
            this.label17.Enabled = wtw;
            this.txtExportRSSMaxDays.Enabled = wtw;
            this.txtExportRSSMaxShows.Enabled = wtw;
            this.txtExportRSSDaysPast.Enabled = wtw;

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
            this.folderBrowser.SelectedPath = n != -1 ? this.mDoc.SearchFolders[n] : "";

            if (this.folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.mDoc.SearchFolders.Add(this.folderBrowser.SelectedPath);
                this.mDoc.SetDirty();
            }

            this.FillSearchFolderList();
        }

        private void bnRemoveSearchFolder_Click(object sender, System.EventArgs e)
        {
            int n = this.lbSearchFolders.SelectedIndex;
            if (n == -1)
                return;

            this.mDoc.SearchFolders.RemoveAt(n);
            this.mDoc.SetDirty();

            this.FillSearchFolderList();
        }

        private void bnOpenSearchFolder_Click(object sender, System.EventArgs e)
        {
            int n = this.lbSearchFolders.SelectedIndex;
            if (n == -1)
                return;
            TVDoc.SysOpen(this.mDoc.SearchFolders[n]);
        }

        private void FillSearchFolderList()
        {
            this.lbSearchFolders.Items.Clear();
            this.mDoc.SearchFolders.Sort();
            foreach (string efi in this.mDoc.SearchFolders)
                this.lbSearchFolders.Items.Add(efi);
        }

        private void lbSearchFolders_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                this.bnRemoveSearchFolder_Click(null, null);
        }

        private void lbSearchFolders_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            e.Effect = !e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.None : DragDropEffects.Copy;
        }

        private void lbSearchFolders_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            string[] files = (string[]) (e.Data.GetData(DataFormats.FileDrop));
            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i];
                try
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    if (di.Exists)
                        this.mDoc.SearchFolders.Add(path.ToLower());
                }
                catch
                {
                }
            }
            this.mDoc.SetDirty();
            this.FillSearchFolderList();
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
            SourceGrid.Cells.Views.Cell titleModel = new SourceGrid.Cells.Views.Cell
                                                         {
                                                             BackColor = Color.SteelBlue,
                                                             ForeColor = Color.White,
                                                             TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft
                                                         };

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
            SourceGrid.Cells.Views.Cell roModel = new SourceGrid.Cells.Views.Cell {ForeColor = Color.Gray};

            int r = this.ReplacementsGrid.RowsCount;
            this.ReplacementsGrid.RowsCount = r + 1;
            this.ReplacementsGrid[r, 0] = new SourceGrid.Cells.Cell(from, typeof(string));
            this.ReplacementsGrid[r, 1] = new SourceGrid.Cells.Cell(to, typeof(string));
            this.ReplacementsGrid[r, 2] = new SourceGrid.Cells.CheckBox(null, ins);
            if (!string.IsNullOrEmpty(from) && (TVSettings.CompulsoryReplacements().IndexOf(from) != -1))
            {
                this.ReplacementsGrid[r, 0].Editor.EnableEdit = false;
                this.ReplacementsGrid[r, 0].View = roModel;
            }
        }

        private void SetupRSSGrid()
        {
            SourceGrid.Cells.Views.Cell titleModel = new SourceGrid.Cells.Views.Cell
                                                         {
                                                             BackColor = Color.SteelBlue,
                                                             ForeColor = Color.White,
                                                             TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft
                                                         };

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

            ColumnHeader h = new SourceGrid.Cells.ColumnHeader("URL");
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

        private void SetupLanguages()
        {
            this.cbLanguages.Items.Clear();
            this.cbLanguages.Items.Add("Please wait...");
            this.cbLanguages.SelectedIndex = 0;
            this.cbLanguages.Update();
            this.cbLanguages.Enabled = false;

            LoadLanguageThread = new Thread(LoadLanguage);
            LoadLanguageThread.Start();
        }

        private void LoadLanguage()
        {
            TheTVDB db = this.mDoc.GetTVDB(true, "Preferences-LoadLanguages");
            bool aborted = false;
            try
            {
                if (!db.Connected)
                {
                    db.Connect();
                }
            }
            catch (ThreadAbortException)
            {
                aborted = true;
            }
            db.Unlock("Preferences-LoadLanguages");
            if (!aborted)
                this.BeginInvoke(LoadLanguageDone);
        }

        private void LoadLanguageDoneFunc()
        {
            FillLanguageList();
        }


        private void FillLanguageList()
        {
            TheTVDB db = this.mDoc.GetTVDB(true, "Preferences-FLL");
            this.cbLanguages.BeginUpdate();
            this.cbLanguages.Items.Clear();

            String pref = "";
            foreach (var kvp in db.LanguageList)
            {
                String name = kvp.Value;
                this.cbLanguages.Items.Add(name);

                if (EnterPreferredLanguage == kvp.Key)
                    pref = kvp.Value;
            }
            this.cbLanguages.EndUpdate();
            this.cbLanguages.Text = pref;
            this.cbLanguages.Enabled = true;

            db.Unlock("Preferences-FLL");
        }
        /*
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
        }*/

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
            this.cbMeta.Enabled = e;
            this.cbCheckuTorrent.Enabled = e;

            bool e2 = this.cbSearchLocally.Checked;
            this.cbLeaveOriginals.Enabled = e && e2;

            bool e3 = this.cbMeta.Checked;
            this.cbMetaSubfolder.Enabled = e && e3;
        }

        private void cbSearchLocally_CheckedChanged(object sender, System.EventArgs e)
        {
            this.ScanOptEnableDisable();
        }

        private void cbMeta_CheckedChanged(object sender, EventArgs e)
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
                if (string.IsNullOrEmpty(from) || (TVSettings.CompulsoryReplacements().IndexOf(from) == -1))
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
                    if (!ColorTranslator.FromHtml(this.txtShowStatusColor.Text).IsEmpty && ssct != null)
                    {
                        ListViewItem item = null;
                        item = this.lvwDefinedColors.FindItemWithText(ssct.Text);
                        if (item == null)
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

        private void Preferences_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (LoadLanguageThread != null && LoadLanguageThread.IsAlive)
            {
                LoadLanguageThread.Abort();
                LoadLanguageThread.Join(500); // milliseconds timeout
            }
        }
    }
}