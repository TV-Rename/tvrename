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
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
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

        private TVDoc _mDoc;
        private Thread _loadLanguageThread;
        private String _enterPreferredLanguage; // hold here until background language download task is done

        private LoadLanguageDoneDel _loadLanguageDone;

        public Preferences(TVDoc doc, bool goToScanOpts)
        {
            InitializeComponent();
            _loadLanguageDone += LoadLanguageDoneFunc;

            SetupRssGrid();
            SetupReplacementsGrid();

            _mDoc = doc;

            if (goToScanOpts)
                tabControl1.SelectedTab = tpScanOptions;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (!TVSettings.OkExtensionsString(txtEmptyIgnoreExtensions.Text))
            {
                MessageBox.Show(
                    "Extensions list must be separated by semicolons, and each extension must start with a dot.",
                    "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl1.SelectedTab = tbFolderDeleting;
                txtVideoExtensions.Focus();
                return;
            }

            if (!TVSettings.OkExtensionsString(txtVideoExtensions.Text))
            {
                MessageBox.Show(
                    "Extensions list must be separated by semicolons, and each extension must start with a dot.",
                    "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl1.SelectedTab = tbFilesAndFolders;
                txtVideoExtensions.Focus();
                return;
            }
            if (!TVSettings.OkExtensionsString(txtVideoExtensions.Text))
            {
                MessageBox.Show(
                    "Extensions list must be separated by semicolons, and each extension must start with a dot.",
                    "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl1.SelectedTab = tbFilesAndFolders;
                txtVideoExtensions.Focus();
                return;
            }
            if (!TVSettings.OkExtensionsString(txtOtherExtensions.Text))
            {
                MessageBox.Show(
                    "Extensions list must be separated by semicolons, and each extension must start with a dot.",
                    "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl1.SelectedTab = tbFilesAndFolders;
                txtOtherExtensions.Focus();
                return;
            }
            TVSettings s = TVSettings.Instance;

            s.Replacements.Clear();
            for (int i = 1; i < ReplacementsGrid.RowsCount; i++)
            {
                string from = (string) (ReplacementsGrid[i, 0].Value);
                string to = (string) (ReplacementsGrid[i, 1].Value);
                bool ins = (bool) (ReplacementsGrid[i, 2].Value);
                if (!string.IsNullOrEmpty(from))
                    s.Replacements.Add(new Replacement(from, to, ins));
            }

            s.ExportWtwrss = cbWTWRSS.Checked;
            s.ExportWtwrssTo = txtWTWRSS.Text;
            s.ExportWtwxml = cbWTWXML.Checked;
            s.ExportWtwxmlTo = txtWTWXML.Text;
            s.ExportMissingXML = cbMissingXML.Checked;
            s.ExportMissingXMLTo = txtMissingXML.Text;
            s.ExportMissingCsv = cbMissingCSV.Checked;
            s.ExportMissingCsvTo = txtMissingCSV.Text;
            s.ExportRenamingXML = cbRenamingXML.Checked;
            s.ExportRenamingXMLTo = txtRenamingXML.Text;
            s.ExportFoxml = cbFOXML.Checked;
            s.ExportFoxmlTo = txtFOXML.Text;
            s.ExportShowsTxt = cbShowsTXT.Checked;
            s.ExportShowsTxtTo = txtShowsTXTTo.Text;

            s.WtwRecentDays = Convert.ToInt32(txtWTWDays.Text);
            s.StartupTab = cbStartupTab.SelectedIndex;
            s.NotificationAreaIcon = cbNotificationIcon.Checked;
            s.VideoExtensionsString = txtVideoExtensions.Text;
            s.OtherExtensionsString = txtOtherExtensions.Text;
            s.ExportRssMaxDays = Convert.ToInt32(txtExportRSSMaxDays.Text);
            s.ExportRssMaxShows = Convert.ToInt32(txtExportRSSMaxShows.Text);
            s.ExportRssDaysPast = Convert.ToInt32(txtExportRSSDaysPast.Text);
            s.KeepTogether = cbKeepTogether.Checked;
            s.LeadingZeroOnSeason = cbLeadingZero.Checked;
            s.ShowInTaskbar = chkShowInTaskbar.Checked;
            s.RenameTxtToSub = cbTxtToSub.Checked;
            s.ShowEpisodePictures = cbShowEpisodePictures.Checked;
            s.AutoSelectShowInMyShows = cbAutoSelInMyShows.Checked;
            s.AutoCreateFolders = cbAutoCreateFolders.Checked ;  
            s.SpecialsFolderName = txtSpecialsFolderName.Text;

            s.ForceLowercaseFilenames = cbForceLower.Checked;
            s.IgnoreSamples = cbIgnoreSamples.Checked;

            s.UTorrentPath = txtRSSuTorrentPath.Text;
            s.ResumeDatPath = txtUTResumeDatPath.Text;
            s.SabHostPort = txtSABHostPort.Text;
            s.SabapiKey = txtSABAPIKey.Text;
            s.CheckSaBnzbd = cbCheckSABnzbd.Checked;

            s.SearchRss = cbSearchRSS.Checked;
            s.EpTbNs = cbEpTBNs.Checked;
            s.NfoShows = cbNFOShows.Checked;
            s.NfoEpisodes = cbNFOEpisodes.Checked;
            s.KodiImages = cbKODIImages.Checked;
            s.PyTivoMeta = cbMeta.Checked;
            s.PyTivoMetaSubFolder = cbMetaSubfolder.Checked;
            s.FolderJpg = cbFolderJpg.Checked;
            s.RenameCheck = cbRenameCheck.Checked;
            s.MissingCheck = cbMissing.Checked;
            s.SearchLocally = cbSearchLocally.Checked;
            s.LeaveOriginals = cbLeaveOriginals.Checked;
            s.CheckuTorrent = cbCheckuTorrent.Checked;
            s.LookForDateInFilename = cbLookForAirdate.Checked;

            s.MonitorFolders = cbMonitorFolder.Checked;
            s.RemoveDownloadDirectoriesFiles = cbCleanUpDownloadDir.Checked;

            s.EpJpGs = cbEpThumbJpg.Checked;
            s.SeriesJpg = cbSeriesJpg.Checked;
            s.Mede8ErXML = cbXMLFiles.Checked;
            s.ShrinkLargeMede8ErImages = cbShrinkLarge.Checked;
            s.FanArtJpg = cbFantArtJpg.Checked;

            s.Tidyup.DeleteEmpty = cbDeleteEmpty.Checked;
            s.Tidyup.DeleteEmptyIsRecycle = cbRecycleNotDelete.Checked;
            s.Tidyup.EmptyIgnoreWords = cbEmptyIgnoreWords.Checked;
            s.Tidyup.EmptyIgnoreWordList = txtEmptyIgnoreWords.Text;
            s.Tidyup.EmptyIgnoreExtensions = cbEmptyIgnoreExtensions.Checked;
            s.Tidyup.EmptyIgnoreExtensionList = txtEmptyIgnoreExtensions.Text;
            s.Tidyup.EmptyMaxSizeCheck = cbEmptyMaxSize.Checked;
            int.TryParse(txtEmptyMaxSize.Text, out s.Tidyup.EmptyMaxSizeMb);

            if (rbFolderFanArt.Checked)
                s.FolderJpgIs = TVSettings.FolderJpgIsType.FanArt;
            else if (rbFolderBanner.Checked)
                s.FolderJpgIs = TVSettings.FolderJpgIsType.Banner;
            else if (rbFolderSeasonPoster.Checked)
                s.FolderJpgIs = TVSettings.FolderJpgIsType.SeasonPoster;
            else
                s.FolderJpgIs = TVSettings.FolderJpgIsType.Poster;

            if (rdoQuickScan.Checked)
                s.MonitoredFoldersScanType = TVSettings.ScanType.Quick;
            else if (rdoRecentScan.Checked)
                s.MonitoredFoldersScanType = TVSettings.ScanType.Recent;
            else
                s.MonitoredFoldersScanType = TVSettings.ScanType.Full;

            if (rdEden.Checked)
                s.SelectedKodiType= TVSettings.KodiType.Eden;
            else if (rdFrodo.Checked)
                s.SelectedKodiType = TVSettings.KodiType.Frodo;
            else
                s.SelectedKodiType = TVSettings.KodiType.Both;


            TheTVDB.Instance.GetLock("Preferences-OK");
            foreach (Language l in TheTVDB.Instance.LanguageList)
            {
                if (l.Name == cbLanguages.Text)
                {
                    s.PreferredLanguage = l.Abbreviation;
                    break;
                }
            }
            if (rbWTWScan.Checked)
                s.WtwDoubleClick = TVSettings.WtwDoubleClickAction.Scan;
            else
                s.WtwDoubleClick = TVSettings.WtwDoubleClickAction.Search;

            TheTVDB.Instance.SaveCache();
            TheTVDB.Instance.Unlock("Preferences-OK");

            try
            {
                s.SampleFileMaxSizeMb = int.Parse(txtMaxSampleSize.Text);
            }
            catch
            {
                s.SampleFileMaxSizeMb = 50;
            }

            try
            {
                s.ParallelDownloads = int.Parse(txtParallelDownloads.Text);
            }
            catch
            {
                s.ParallelDownloads = 4;
            }

            if (s.ParallelDownloads < 1)
                s.ParallelDownloads = 1;
            else if (s.ParallelDownloads > 8)
                s.ParallelDownloads = 8;

            // RSS URLs
            s.RssurLs.Clear();
            for (int i = 1; i < RSSGrid.RowsCount; i++)
            {
                string url = (string) (RSSGrid[i, 0].Value);
                if (!string.IsNullOrEmpty(url))
                    s.RssurLs.Add(url);
            }

            s.ShowStatusColors = new ShowStatusColoringTypeList();
            foreach (ListViewItem item in lvwDefinedColors.Items)
            {
                if (item.SubItems.Count > 1 && !string.IsNullOrEmpty(item.SubItems[1].Text) && item.Tag != null &&
                    item.Tag is ShowStatusColoringType)
                {
                    s.ShowStatusColors.Add(item.Tag as ShowStatusColoringType,
                                           ColorTranslator.FromHtml(item.SubItems[1].Text));
                }
            }

            _mDoc.SetDirty();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Preferences_Load(object sender, EventArgs e)
        {
            SetupLanguages();

            TVSettings s = TVSettings.Instance;
           
            foreach (Replacement rep in s.Replacements)
            {
                AddNewReplacementRow(rep.This, rep.That, rep.CaseInsensitive);
            }

            txtMaxSampleSize.Text = s.SampleFileMaxSizeMb.ToString();

            cbWTWRSS.Checked = s.ExportWtwrss;
            txtWTWRSS.Text = s.ExportWtwrssTo;
            txtWTWDays.Text = s.WtwRecentDays.ToString();
            cbWTWXML.Checked = s.ExportWtwxml;
            txtWTWXML.Text = s.ExportWtwxmlTo;
            txtExportRSSMaxDays.Text = s.ExportRssMaxDays.ToString();
            txtExportRSSMaxShows.Text = s.ExportRssMaxShows.ToString();
            txtExportRSSDaysPast.Text = s.ExportRssDaysPast.ToString();

            cbMissingXML.Checked = s.ExportMissingXML;
            txtMissingXML.Text = s.ExportMissingXMLTo;
            cbMissingCSV.Checked = s.ExportMissingCsv;
            txtMissingCSV.Text = s.ExportMissingCsvTo;

            cbShowsTXT.Checked = s.ExportShowsTxt ;
            txtShowsTXTTo.Text = s.ExportShowsTxtTo;


            cbRenamingXML.Checked = s.ExportRenamingXML;
            txtRenamingXML.Text = s.ExportRenamingXMLTo;

            cbFOXML.Checked = s.ExportFoxml;
            txtFOXML.Text = s.ExportFoxmlTo;

            cbStartupTab.SelectedIndex = s.StartupTab;
            cbNotificationIcon.Checked = s.NotificationAreaIcon;
            txtVideoExtensions.Text = s.GetVideoExtensionsString();
            txtOtherExtensions.Text = s.GetOtherExtensionsString();

            cbKeepTogether.Checked = s.KeepTogether;
            cbKeepTogether_CheckedChanged(null, null);

            cbLeadingZero.Checked = s.LeadingZeroOnSeason;
            chkShowInTaskbar.Checked = s.ShowInTaskbar;
            cbTxtToSub.Checked = s.RenameTxtToSub;
            cbShowEpisodePictures.Checked = s.ShowEpisodePictures;
            cbAutoCreateFolders.Checked = s.AutoCreateFolders; 
            cbAutoSelInMyShows.Checked = s.AutoSelectShowInMyShows;
            txtSpecialsFolderName.Text = s.SpecialsFolderName;
            cbForceLower.Checked = s.ForceLowercaseFilenames;
            cbIgnoreSamples.Checked = s.IgnoreSamples;
            txtRSSuTorrentPath.Text = s.UTorrentPath;
            txtUTResumeDatPath.Text = s.ResumeDatPath;
            txtSABHostPort.Text = s.SabHostPort;
            txtSABAPIKey.Text = s.SabapiKey;
            cbCheckSABnzbd.Checked = s.CheckSaBnzbd;

            txtParallelDownloads.Text = s.ParallelDownloads.ToString();

            cbSearchRSS.Checked = s.SearchRss;
            cbEpTBNs.Checked = s.EpTbNs;
            cbNFOShows.Checked = s.NfoShows;
            cbNFOEpisodes.Checked = s.NfoEpisodes;
            cbKODIImages.Checked = s.KodiImages;
            cbMeta.Checked = s.PyTivoMeta;
            cbMetaSubfolder.Checked = s.PyTivoMetaSubFolder;
            cbFolderJpg.Checked = s.FolderJpg;
            cbRenameCheck.Checked = s.RenameCheck;
            cbCheckuTorrent.Checked = s.CheckuTorrent;
            cbLookForAirdate.Checked = s.LookForDateInFilename;
            cbMonitorFolder.Checked = s.MonitorFolders;
            cbCleanUpDownloadDir.Checked = s.RemoveDownloadDirectoriesFiles;
            cbMissing.Checked = s.MissingCheck;
            cbSearchLocally.Checked = s.SearchLocally;
            cbLeaveOriginals.Checked = s.LeaveOriginals;
            _enterPreferredLanguage = s.PreferredLanguage;

            cbEpThumbJpg.Checked = s.EpJpGs;
            cbSeriesJpg.Checked = s.SeriesJpg;
            cbXMLFiles.Checked = s.Mede8ErXML;
            cbShrinkLarge.Checked = s.ShrinkLargeMede8ErImages;
            cbFantArtJpg.Checked = s.FanArtJpg;

#if DEBUG
            System.Diagnostics.Debug.Assert(s.Tidyup != null);
#endif
            cbDeleteEmpty.Checked = s.Tidyup.DeleteEmpty;
            cbRecycleNotDelete.Checked = s.Tidyup.DeleteEmptyIsRecycle;
            cbEmptyIgnoreWords.Checked = s.Tidyup.EmptyIgnoreWords;
            txtEmptyIgnoreWords.Text = s.Tidyup.EmptyIgnoreWordList;
            cbEmptyIgnoreExtensions.Checked = s.Tidyup.EmptyIgnoreExtensions;
            txtEmptyIgnoreExtensions.Text = s.Tidyup.EmptyIgnoreExtensionList;
            cbEmptyMaxSize.Checked = s.Tidyup.EmptyMaxSizeCheck;
            txtEmptyMaxSize.Text = s.Tidyup.EmptyMaxSizeMb.ToString();


            switch (s.WtwDoubleClick)
            {
                case TVSettings.WtwDoubleClickAction.Search:
                default:
                    rbWTWSearch.Checked = true;
                    break;
                case TVSettings.WtwDoubleClickAction.Scan:
                    rbWTWScan.Checked = true;
                    break;
            }

            EnableDisable(null, null);
            ScanOptEnableDisable();

            FillSearchFolderList();

            foreach (string url in s.RssurLs)
                AddNewRssRow(url);

            switch (s.FolderJpgIs)
            {
                case TVSettings.FolderJpgIsType.FanArt:
                    rbFolderFanArt.Checked = true;
                    break;
                case TVSettings.FolderJpgIsType.Banner:
                    rbFolderBanner.Checked = true;
                    break;
                case TVSettings.FolderJpgIsType.SeasonPoster:
                    rbFolderSeasonPoster.Checked = true;
                    break;
                default:
                    rbFolderPoster.Checked = true;
                    break;
            }

            switch (s.MonitoredFoldersScanType)
            {
                case TVSettings.ScanType.Quick:
                    rdoQuickScan.Checked = true;
                    break;
                case TVSettings.ScanType.Recent:
                    rdoRecentScan.Checked = true;
                    break;
                default:
                    rdoFullScan.Checked = true;
                    break;
            }

            switch (s.SelectedKodiType)
            {
                case TVSettings.KodiType.Eden:
                    rdEden.Checked = true;
                    break;
                case TVSettings.KodiType.Frodo:
                    rdFrodo.Checked = true;
                    break;
                default:
                    rdBoth.Checked = true;
                    break;
            }

            if (s.ShowStatusColors != null)
            {
                foreach (
                    KeyValuePair<ShowStatusColoringType, Color> showStatusColor in
                        s.ShowStatusColors)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = showStatusColor.Key.Text;
                    item.Tag = showStatusColor.Key;
                    item.SubItems.Add(TranslateColorToHtml(showStatusColor.Value));
                    item.ForeColor = showStatusColor.Value;
                    lvwDefinedColors.Items.Add(item);
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
                cboShowStatus.Items.Add(t);
                //this.cboShowStatus.Items.Add("Show Seasons Status: " + status);
            }
            List<string> showStatusList = new List<string>();
            List<ShowItem> shows = _mDoc.GetShowItems(false);
            foreach (ShowItem show in shows)
            {
                if(!showStatusList.Contains(show.ShowStatus))
                    showStatusList.Add(show.ShowStatus);
            }
            foreach (string status in showStatusList)
            {
                ShowStatusColoringType t = new ShowStatusColoringType(false, true, status);
                //System.Collections.Generic.KeyValuePair<string, object> item = new System.Collections.Generic.KeyValuePair<string, object>("Show  Status: " + status, new ShowStatusColoringType(false, true, status));
                cboShowStatus.Items.Add(t);
            }
            //this.cboShowStatus.Items.Add(new System.Collections.Generic.KeyValuePair<string, object>("Show Seasons Status: Custom", null));
            // Seasons
            foreach (string status in Enum.GetNames(typeof(Season.SeasonStatus)))
            {
                ShowStatusColoringType t = new ShowStatusColoringType(true, false, status);
                //System.Collections.Generic.KeyValuePair<string, object> item = new System.Collections.Generic.KeyValuePair<string, object>("Seasons Status: " + status, new ShowStatusColoringType(true, false, status));
                cboShowStatus.Items.Add(t);
                //this.cboShowStatus.Items.Add("Seasons Status: " + status);
            }
            cboShowStatus.DisplayMember = "Text";
            //this.cboShowStatus.ValueMember = ";
        }

        private void Browse(TextBox txt, string defaultExt, int filterIndex)
        {
            //rss =1, XML = 2, CSV = 3, TXT=4, HTML = 5
            saveFile.FileName = txt.Text;
            saveFile.DefaultExt = defaultExt;
            saveFile.FilterIndex = filterIndex;
            if (saveFile.ShowDialog() == DialogResult.OK)
                txt.Text = saveFile.FileName;
        }


        private void bnBrowseWTWXML_Click(object sender, EventArgs e)
        {
            Browse(txtWTWXML,"xml",2);
        }

        private void TxtNumberOnlyKeyPress(object sender, KeyPressEventArgs e)
        {
            // digits only
            if ((e.KeyChar >= 32) && (!Char.IsDigit(e.KeyChar)))
                e.Handled = true;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cbNotificationIcon_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbNotificationIcon.Checked)
                chkShowInTaskbar.Checked = true;
        }

        private void chkShowInTaskbar_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkShowInTaskbar.Checked)
                cbNotificationIcon.Checked = true;
        }

        private void cbKeepTogether_CheckedChanged(object sender, EventArgs e)
        {
            cbTxtToSub.Enabled = cbKeepTogether.Checked;
        }

        private void bnBrowseMissingCSV_Click(object sender, EventArgs e)
        {
            Browse(txtMissingCSV,"csv",3);
        }


        private void bnBrowseWTWRSS_Click(object sender, EventArgs e)
        {
            Browse(txtWTWRSS, "rss", 1);
        }

        private void bnBrowseMissingXML_Click(object sender, EventArgs e)
        {
            Browse(txtMissingXML,"xml",2);
        }

        private void bnBrowseShowsTXT_Click(object sender, EventArgs e)
        {
            Browse(txtShowsTXTTo, "txt", 4);
        }

        private void bnBrowseRenamingXML_Click(object sender, EventArgs e)
        {
            Browse(txtRenamingXML,"xml",2);
        }

        private void bnBrowseFOXML_Click(object sender, EventArgs e)
        {
            Browse(txtFOXML,"xml",2);
        }

        private void EnableDisable(object sender, EventArgs e)
        {
            txtWTWRSS.Enabled = cbWTWRSS.Checked;
            bnBrowseWTWRSS.Enabled = cbWTWRSS.Checked;

            txtWTWXML.Enabled = cbWTWXML.Checked;
            bnBrowseWTWXML.Enabled = cbWTWXML.Checked;

            bool wtw;
            if ((cbWTWRSS.Checked) || (cbWTWXML.Checked))
                wtw = true;
            else
                wtw = false;

            label4.Enabled = wtw;
            label15.Enabled = wtw;
            label16.Enabled = wtw;
            label17.Enabled = wtw;
            txtExportRSSMaxDays.Enabled = wtw;
            txtExportRSSMaxShows.Enabled = wtw;
            txtExportRSSDaysPast.Enabled = wtw;

            bool fo = cbFOXML.Checked;
            txtFOXML.Enabled = fo;
            bnBrowseFOXML.Enabled = fo;

            bool stxt = cbShowsTXT.Checked;
            txtShowsTXTTo.Enabled = stxt;
            bnBrowseShowsTXT.Enabled = stxt;

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

        private void bnAddSearchFolder_Click(object sender, EventArgs e)
        {
            int n = lbSearchFolders.SelectedIndex;
            folderBrowser.SelectedPath = n != -1 ? _mDoc.SearchFolders[n] : "";

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                _mDoc.SearchFolders.Add(folderBrowser.SelectedPath);
                _mDoc.SetDirty();
            }

            FillSearchFolderList();
        }

        private void bnRemoveSearchFolder_Click(object sender, EventArgs e)
        {
            int n = lbSearchFolders.SelectedIndex;
            if (n == -1)
                return;

            _mDoc.SearchFolders.RemoveAt(n);
            _mDoc.SetDirty();

            FillSearchFolderList();
        }

        private void bnOpenSearchFolder_Click(object sender, EventArgs e)
        {
            int n = lbSearchFolders.SelectedIndex;
            if (n == -1)
                return;
            Helpers.SysOpen(_mDoc.SearchFolders[n]);
        }

        private void FillSearchFolderList()
        {
            lbSearchFolders.Items.Clear();
            _mDoc.SearchFolders.Sort();
            foreach (string efi in _mDoc.SearchFolders)
                lbSearchFolders.Items.Add(efi);
        }

        private void lbSearchFolders_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                bnRemoveSearchFolder_Click(null, null);
        }

        private void lbSearchFolders_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = !e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.None : DragDropEffects.Copy;
        }

        private void lbSearchFolders_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[]) (e.Data.GetData(DataFormats.FileDrop));
            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i];
                try
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    if (di.Exists)
                        _mDoc.SearchFolders.Add(path.ToLower());
                }
                catch
                {
                }
            }
            _mDoc.SetDirty();
            FillSearchFolderList();
        }

        private void bnRSSBrowseuTorrent_Click(object sender, EventArgs e)
        {
            openFile.FileName = txtRSSuTorrentPath.Text;
            openFile.Filter = "utorrent.exe|utorrent.exe|All Files (*.*)|*.*";
            if (openFile.ShowDialog() == DialogResult.OK)
                txtRSSuTorrentPath.Text = openFile.FileName;
        }

        private void bnUTBrowseResumeDat_Click(object sender, EventArgs e)
        {
            openFile.FileName = txtUTResumeDatPath.Text;
            openFile.Filter = "resume.dat|resume.dat|All Files (*.*)|*.*";
            if (openFile.ShowDialog() == DialogResult.OK)
                txtUTResumeDatPath.Text = openFile.FileName;
        }

        private void SetupReplacementsGrid()
        {
            SourceGrid.Cells.Views.Cell titleModel = new SourceGrid.Cells.Views.Cell
                                                         {
                                                             BackColor = Color.SteelBlue,
                                                             ForeColor = Color.White,
                                                             TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft
                                                         };

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

            ColumnHeader h;
            h = new ColumnHeader("Search");
            h.AutomaticSortEnabled = false;
            ReplacementsGrid[0, 0] = h;
            ReplacementsGrid[0, 0].View = titleModel;

            h = new ColumnHeader("Replace");
            h.AutomaticSortEnabled = false;
            ReplacementsGrid[0, 1] = h;
            ReplacementsGrid[0, 1].View = titleModel;

            h = new ColumnHeader("Case Ins.");
            h.AutomaticSortEnabled = false;
            ReplacementsGrid[0, 2] = h;
            ReplacementsGrid[0, 2].View = titleModel;
        }

        private void AddNewReplacementRow(string from, string to, bool ins)
        {
            SourceGrid.Cells.Views.Cell roModel = new SourceGrid.Cells.Views.Cell {ForeColor = Color.Gray};

            int r = ReplacementsGrid.RowsCount;
            ReplacementsGrid.RowsCount = r + 1;
            ReplacementsGrid[r, 0] = new SourceGrid.Cells.Cell(from, typeof(string));
            ReplacementsGrid[r, 1] = new SourceGrid.Cells.Cell(to, typeof(string));
            ReplacementsGrid[r, 2] = new SourceGrid.Cells.CheckBox(null, ins);
            if (!string.IsNullOrEmpty(from) && (TVSettings.CompulsoryReplacements().IndexOf(from) != -1))
            {
                ReplacementsGrid[r, 0].Editor.EnableEdit = false;
                ReplacementsGrid[r, 0].View = roModel;
            }
        }

        private void SetupRssGrid()
        {
            SourceGrid.Cells.Views.Cell titleModel = new SourceGrid.Cells.Views.Cell
                                                         {
                                                             BackColor = Color.SteelBlue,
                                                             ForeColor = Color.White,
                                                             TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft
                                                         };

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

            ColumnHeader h = new ColumnHeader("URL");
            h.AutomaticSortEnabled = false;
            RSSGrid[0, 0] = h;
            RSSGrid[0, 0].View = titleModel;
        }

        private void AddNewRssRow(string text)
        {
            int r = RSSGrid.RowsCount;
            RSSGrid.RowsCount = r + 1;
            RSSGrid[r, 0] = new SourceGrid.Cells.Cell(text, typeof(string));
        }

        private void bnRSSAdd_Click(object sender, EventArgs e)
        {
            AddNewRssRow(null);
        }

        private void bnRSSRemove_Click(object sender, EventArgs e)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = RSSGrid.Selection.GetSelectionRegion().GetRowsIndex();
            if (rowsIndex.Length > 0)
                RSSGrid.Rows.Remove(rowsIndex[0]);
        }

        private void bnRSSGo_Click(object sender, EventArgs e)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = RSSGrid.Selection.GetSelectionRegion().GetRowsIndex();

            if (rowsIndex.Length > 0)
                Helpers.SysOpen((string) (RSSGrid[rowsIndex[0], 0].Value));
        }

        private void SetupLanguages()
        {
            cbLanguages.Items.Clear();
            cbLanguages.Items.Add("Please wait...");
            cbLanguages.SelectedIndex = 0;
            cbLanguages.Update();
            cbLanguages.Enabled = false;

            _loadLanguageThread = new Thread(LoadLanguage);
            _loadLanguageThread.Start();
        }

        private void LoadLanguage()
        {
            TheTVDB.Instance.GetLock ("Preferences-LoadLanguages");
            bool aborted = false;
            try
            {
                if (!TheTVDB.Instance.Connected)
                {
                    TheTVDB.Instance.Connect();
                }
            }
            catch (ThreadAbortException)
            {
                aborted = true;
            }
            TheTVDB.Instance.Unlock("Preferences-LoadLanguages");
            if (!aborted)
                BeginInvoke(_loadLanguageDone);
        }

        private void LoadLanguageDoneFunc()
        {
            FillLanguageList();
        }


        private void FillLanguageList()
        {
            TheTVDB.Instance.GetLock( "Preferences-FLL");
            cbLanguages.BeginUpdate();
            cbLanguages.Items.Clear();

            String pref = "";
            foreach (Language l in TheTVDB.Instance.LanguageList)
            {
                cbLanguages.Items.Add(l.Name);

                if (_enterPreferredLanguage == l.Abbreviation)
                    pref = l.Name;
            }
            cbLanguages.EndUpdate();
            cbLanguages.Text = pref;
            cbLanguages.Enabled = true;

            TheTVDB.Instance.Unlock("Preferences-FLL");
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

        private void cbMissing_CheckedChanged(object sender, EventArgs e)
        {
            ScanOptEnableDisable();
        }

        private void ScanOptEnableDisable()
        {
            bool e = cbMissing.Checked;
            tbMediaCenter.Enabled = e;

            cbSearchRSS.Enabled = e;
            cbSearchLocally.Enabled = e;
            cbCheckuTorrent.Enabled = e;

            bool e2 = e && cbSearchLocally.Checked;
            cbLeaveOriginals.Enabled = e2;
            cbCheckSABnzbd.Enabled = e2;
        }

        private void cbSearchLocally_CheckedChanged(object sender, EventArgs e)
        {
            ScanOptEnableDisable();
        }

        private void cbMeta_CheckedChanged(object sender, EventArgs e)
        {
            ScanOptEnableDisable();
        }

        private void bnReplaceAdd_Click(object sender, EventArgs e)
        {
            AddNewReplacementRow(null, null, false);
        }

        private void bnReplaceRemove_Click(object sender, EventArgs e)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = ReplacementsGrid.Selection.GetSelectionRegion().GetRowsIndex();
            if (rowsIndex.Length > 0)
            {
                // don't delete compulsory items
                int n = rowsIndex[0];
                string from = (string) (ReplacementsGrid[n, 0].Value);
                if (string.IsNullOrEmpty(from) || (TVSettings.CompulsoryReplacements().IndexOf(from) == -1))
                    ReplacementsGrid.Rows.Remove(n);
            }
        }

        private void btnAddShowStatusColoring_Click(object sender, EventArgs e)
        {
            if (cboShowStatus.SelectedItem != null && !string.IsNullOrEmpty(txtShowStatusColor.Text))
            {
                try
                {
                    ShowStatusColoringType ssct = cboShowStatus.SelectedItem as ShowStatusColoringType;
                    if (!ColorTranslator.FromHtml(txtShowStatusColor.Text).IsEmpty && ssct != null)
                    {
                        ListViewItem item = lvwDefinedColors.FindItemWithText(ssct.Text);
                        if (item == null)
                        {
                            item = new ListViewItem();
                            item.SubItems.Add(txtShowStatusColor.Text);
                            lvwDefinedColors.Items.Add(item);
                        }

                        item.Text = ssct.Text;
                        item.SubItems[1].Text = txtShowStatusColor.Text;
                        item.ForeColor = ColorTranslator.FromHtml(txtShowStatusColor.Text);
                        item.Tag = ssct;
                        txtShowStatusColor.Text = string.Empty;
                        txtShowStatusColor.ForeColor = Color.Black;
                    }
                }
                catch { }
            }
        }

        private void btnSelectColor_Click(object sender, EventArgs e)
        {
            try
            {
                colorDialog.Color = ColorTranslator.FromHtml(txtShowStatusColor.Text);
            }
            catch
            {
                colorDialog.Color = Color.Black;
            }
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
            {
                txtShowStatusColor.Text =  TranslateColorToHtml(colorDialog.Color);
                txtShowStatusColor.ForeColor = colorDialog.Color;
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
            bnRemoveDefinedColor.Enabled = lvwDefinedColors.SelectedItems.Count == 1;
        }

        private void RemoveSelectedDefinedColor()
        {
            if (lvwDefinedColors.SelectedItems.Count == 1)
            {
                lvwDefinedColors.Items.Remove(lvwDefinedColors.SelectedItems[0]);
            }
        }

        private void txtShowStatusColor_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtShowStatusColor.ForeColor = ColorTranslator.FromHtml(txtShowStatusColor.Text);
            }
            catch
            {
                txtShowStatusColor.ForeColor = Color.Black;
            }
        }

        private void Preferences_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_loadLanguageThread != null && _loadLanguageThread.IsAlive)
            {
                _loadLanguageThread.Abort();
                _loadLanguageThread.Join(500); // milliseconds timeout
            }
        }

        private void cmDefaults_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int v;
            if (e.ClickedItem == null || !(e.ClickedItem.Tag is String) || !int.TryParse(e.ClickedItem.Tag as String, out v))
                return;

            switch (v)
            {
                case 1: // KODI
                    cbEpTBNs.Checked = true;
                    cbNFOShows.Checked = true;
                    cbNFOEpisodes.Checked = true;
                    cbMeta.Checked = false;
                    cbMetaSubfolder.Checked = false;
                    cbSeriesJpg.Checked = false;
                    cbXMLFiles.Checked = false;
                    cbShrinkLarge.Checked = false;
                    cbFolderJpg.Checked = true;
                    rbFolderSeasonPoster.Checked = true;
                    cbEpThumbJpg.Checked = false;
                    cbFantArtJpg.Checked = false;
                    cbKODIImages.Checked = true;
                    rdBoth.Checked = true;
                    break;
                case 2: // pytivo
                    cbEpTBNs.Checked = false;
                    cbNFOShows.Checked = false;
                    cbNFOEpisodes.Checked = false;
                    cbMeta.Checked = true;
                    cbMetaSubfolder.Checked = true;
                    cbSeriesJpg.Checked = false;
                    cbXMLFiles.Checked = false;
                    cbShrinkLarge.Checked = false;
                    cbFolderJpg.Checked = true;
                    rbFolderPoster.Checked = true;
                    cbEpThumbJpg.Checked = false;
                    cbFantArtJpg.Checked = false;
                    cbKODIImages.Checked = false;
                    break;
                case 3: // mede8er
                    cbEpTBNs.Checked = false;
                    cbNFOShows.Checked = false;
                    cbNFOEpisodes.Checked = false;
                    cbMeta.Checked = false;
                    cbMetaSubfolder.Checked = false;
                    cbSeriesJpg.Checked = true;
                    cbXMLFiles.Checked = true;
                    cbShrinkLarge.Checked = true;
                    cbFolderJpg.Checked = true;
                    rbFolderSeasonPoster.Checked = true;
                    cbEpThumbJpg.Checked = false;
                    cbFantArtJpg.Checked = true;
                    cbKODIImages.Checked = false;
                    break;
                case 4: // none
                    cbEpTBNs.Checked = false;
                    cbNFOShows.Checked = false;
                    cbNFOEpisodes.Checked = false;
                    cbMeta.Checked = false;
                    cbMetaSubfolder.Checked = false;
                    cbSeriesJpg.Checked = false;
                    cbXMLFiles.Checked = false;
                    cbShrinkLarge.Checked = false;
                    cbFolderJpg.Checked = false;
                    rbFolderPoster.Checked = false;
                    cbEpThumbJpg.Checked = false;
                    cbFantArtJpg.Checked = false;
                    cbKODIImages.Checked = false;
                    break;
#if DEBUG
                default:
                    System.Diagnostics.Debug.Fail("Unknown default selected.");
                    break;
#endif
            }
        }

        private void bnMCPresets_Click(object sender, EventArgs e)
        {
            Point pt = PointToScreen(bnMCPresets.Location);
            cmDefaults.Show(pt);
        }

    }
}
