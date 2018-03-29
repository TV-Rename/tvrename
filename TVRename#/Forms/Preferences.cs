// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
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

        private TVDoc mDoc;
        private Thread LoadLanguageThread;
        private String EnterPreferredLanguage; // hold here until background language download task is done
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

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
            if (!TVSettings.OKExtensionsString(this.txtEmptyIgnoreExtensions.Text))
            {
                MessageBox.Show(
                    "Extensions list must be separated by semicolons, and each extension must start with a dot.",
                    "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.tabControl1.SelectedTab = tbFolderDeleting;
                this.txtVideoExtensions.Focus();
                return;
            }

            if (!TVSettings.OKExtensionsString(this.txtVideoExtensions.Text))
            {
                MessageBox.Show(
                    "Extensions list must be separated by semicolons, and each extension must start with a dot.",
                    "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.tabControl1.SelectedTab = tbFilesAndFolders;
                this.txtVideoExtensions.Focus();
                return;
            }
            if (!TVSettings.OKExtensionsString(this.txtVideoExtensions.Text))
            {
                MessageBox.Show(
                    "Extensions list must be separated by semicolons, and each extension must start with a dot.",
                    "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.tabControl1.SelectedTab = tbFilesAndFolders;
                this.txtVideoExtensions.Focus();
                return;
            }
            if (!TVSettings.OKExtensionsString(this.txtOtherExtensions.Text))
            {
                MessageBox.Show(
                    "Extensions list must be separated by semicolons, and each extension must start with a dot.",
                    "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.tabControl1.SelectedTab = tbFilesAndFolders;
                this.txtOtherExtensions.Focus();
                return;
            }
            if (!TVSettings.OKExtensionsString(this.txtKeepTogether.Text))
            {
                MessageBox.Show(
                    "Extensions list must be separated by semicolons, and each extension must start with a dot.",
                    "Preferences", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.tabControl1.SelectedTab = tbFilesAndFolders;
                this.txtKeepTogether.Focus();
                return;
            }

            TVSettings S = TVSettings.Instance;

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
            S.ExportShowsTXT = this.cbShowsTXT.Checked;
            S.ExportShowsTXTTo = this.txtShowsTXTTo.Text;

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
            S.HideMyShowsSpoilers = this.chkHideMyShowsSpoilers.Checked;
            S.HideWtWSpoilers = this.chkHideWtWSpoilers.Checked;
            S.AutoSelectShowInMyShows = this.cbAutoSelInMyShows.Checked;
            S.AutoCreateFolders = this.cbAutoCreateFolders.Checked ;  
            S.SpecialsFolderName = this.txtSpecialsFolderName.Text;
            S.searchSeasonWordsString = this.tbSeasonSearchTerms.Text;
            S.defaultSeasonWord = this.txtSeasonFolderName.Text;
            S.keepTogetherExtensionsString = this.txtKeepTogether.Text;

            S.ForceLowercaseFilenames = this.cbForceLower.Checked;
            S.IgnoreSamples = this.cbIgnoreSamples.Checked;

            S.uTorrentPath = this.txtRSSuTorrentPath.Text;
            S.ResumeDatPath = this.txtUTResumeDatPath.Text;
            S.SABHostPort = this.txtSABHostPort.Text;
            S.SABAPIKey = this.txtSABAPIKey.Text;
            S.CheckSABnzbd = this.cbCheckSABnzbd.Checked;

            S.SearchRSS = this.cbSearchRSS.Checked;
            S.EpTBNs = this.cbEpTBNs.Checked;
            S.NFOShows = this.cbNFOShows.Checked;
            S.NFOEpisodes = this.cbNFOEpisodes.Checked;
            S.KODIImages = this.cbKODIImages.Checked;
            S.pyTivoMeta = this.cbMeta.Checked;
            S.pyTivoMetaSubFolder = this.cbMetaSubfolder.Checked;
            S.FolderJpg = this.cbFolderJpg.Checked;
            S.RenameCheck = this.cbRenameCheck.Checked;
            S.PreventMove = this.chkPreventMove.Checked;
            S.MissingCheck = this.cbMissing.Checked;
            S.CorrectFileDates = this.cbxUpdateAirDate.Checked;
            S.SearchLocally = this.cbSearchLocally.Checked;
            S.AutoSearchForDownloadedFiles = this.chkAutoSearchForDownloadedFiles.Checked;
            S.LeaveOriginals = this.cbLeaveOriginals.Checked;
            S.CheckuTorrent = this.cbCheckuTorrent.Checked;
            S.LookForDateInFilename = this.cbLookForAirdate.Checked;
            S.AutoMergeEpisodes = this.chkAutoMergeEpisodes.Checked;

            S.MonitorFolders = this.cbMonitorFolder.Checked;
            S.runStartupCheck = this.chkScanOnStartup.Checked;
            S.runPeriodicCheck = this.chkScheduledScan.Checked;
            S.periodCheckHours = int.Parse(this.domainUpDown1.SelectedItem?.ToString()??"1");
            S.RemoveDownloadDirectoriesFiles = this.cbCleanUpDownloadDir.Checked;

            S.EpJPGs = this.cbEpThumbJpg.Checked;
            S.SeriesJpg = this.cbSeriesJpg.Checked;
            S.Mede8erXML = this.cbXMLFiles.Checked;
            S.ShrinkLargeMede8erImages = this.cbShrinkLarge.Checked;
            S.FanArtJpg = this.cbFantArtJpg.Checked;

            S.Tidyup.DeleteEmpty = this.cbDeleteEmpty.Checked;
            S.Tidyup.DeleteEmptyIsRecycle = this.cbRecycleNotDelete.Checked;
            S.Tidyup.EmptyIgnoreWords = this.cbEmptyIgnoreWords.Checked;
            S.Tidyup.EmptyIgnoreWordList = this.txtEmptyIgnoreWords.Text;
            S.Tidyup.EmptyIgnoreExtensions = this.cbEmptyIgnoreExtensions.Checked;
            S.Tidyup.EmptyIgnoreExtensionList = this.txtEmptyIgnoreExtensions.Text;
            S.Tidyup.EmptyMaxSizeCheck = this.cbEmptyMaxSize.Checked;
            int.TryParse(this.txtEmptyMaxSize.Text, out S.Tidyup.EmptyMaxSizeMB);

            if (this.rbFolderFanArt.Checked)
                S.FolderJpgIs = TVSettings.FolderJpgIsType.FanArt;
            else if (this.rbFolderBanner.Checked)
                S.FolderJpgIs = TVSettings.FolderJpgIsType.Banner;
            else if (this.rbFolderSeasonPoster.Checked)
                S.FolderJpgIs = TVSettings.FolderJpgIsType.SeasonPoster;
            else
                S.FolderJpgIs = TVSettings.FolderJpgIsType.Poster;

            if (this.rdoQuickScan.Checked)
                S.MonitoredFoldersScanType = TVSettings.ScanType.Quick;
            else if (this.rdoRecentScan.Checked)
                S.MonitoredFoldersScanType = TVSettings.ScanType.Recent;
            else
                S.MonitoredFoldersScanType = TVSettings.ScanType.Full;

            if (this.rdEden.Checked)
                S.SelectedKODIType= TVSettings.KODIType.Eden;
            else if (this.rdFrodo.Checked)
                S.SelectedKODIType = TVSettings.KODIType.Frodo;
            else
                S.SelectedKODIType = TVSettings.KODIType.Both;

            if (this.cbMode.Text == "Beta")
            {
                S.mode = TVSettings.BetaMode.BetaToo;
            }
            else
            {
                S.mode = TVSettings.BetaMode.ProductionOnly;
            }

            if (this.cbKeepTogetherMode.Text == "All but these")
            {
                S.keepTogetherMode = TVSettings.KeepTogetherModes.AllBut;
            } else if (this.cbKeepTogetherMode.Text == "Just")

            {
                S.keepTogetherMode = TVSettings.KeepTogetherModes.Just;
            }
            else
                S.keepTogetherMode = TVSettings.KeepTogetherModes.All;


            TheTVDB.Instance.GetLock("Preferences-OK");
            foreach (Language l in TheTVDB.Instance.LanguageList)
            {
                if (l.name == cbLanguages.Text)
                {
                    S.PreferredLanguage = l.abbreviation;
                    break;
                }
            }
            S.WTWDoubleClick = this.rbWTWScan.Checked ? TVSettings.WTWDoubleClickAction.Scan : TVSettings.WTWDoubleClickAction.Search;

            TheTVDB.Instance.SaveCache();
            TheTVDB.Instance.Unlock("Preferences-OK");

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
                S.upgradeDirtyPercent = float.Parse(this.tbPercentDirty.Text);
            }
            catch
            {
                S.upgradeDirtyPercent = 20;
            }
            if (S.upgradeDirtyPercent < 1)
                S.upgradeDirtyPercent = 1;
            else if (S.upgradeDirtyPercent > 100)
                S.upgradeDirtyPercent = 100;


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
                if (item.SubItems.Count > 1 && !string.IsNullOrEmpty(item.SubItems[1].Text) && item.Tag != null &&
                    item.Tag is ShowStatusColoringType)
                {
                    S.ShowStatusColors.Add(item.Tag as ShowStatusColoringType,
                                           System.Drawing.ColorTranslator.FromHtml(item.SubItems[1].Text));
                }
            }

            this.mDoc.SetDirty();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Preferences_Load(object sender, System.EventArgs e)
        {
            this.SetupLanguages();

            TVSettings S = TVSettings.Instance;

            foreach (Replacement R in S.Replacements)
            {
                this.AddNewReplacementRow(R.This, R.That, R.CaseInsensitive);
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

            this.cbShowsTXT.Checked = S.ExportShowsTXT ;
            this.txtShowsTXTTo.Text = S.ExportShowsTXTTo;


            this.cbRenamingXML.Checked = S.ExportRenamingXML;
            this.txtRenamingXML.Text = S.ExportRenamingXMLTo;

            this.cbFOXML.Checked = S.ExportFOXML;
            this.txtFOXML.Text = S.ExportFOXMLTo;

            this.cbStartupTab.SelectedIndex = S.StartupTab;
            this.cbNotificationIcon.Checked = S.NotificationAreaIcon;
            this.txtVideoExtensions.Text = S.GetVideoExtensionsString();
            this.txtOtherExtensions.Text = S.GetOtherExtensionsString();
            this.txtKeepTogether.Text = S.GetKeepTogetherString();
            this.tbSeasonSearchTerms.Text = S.GetSeasonSearchTermsString();

            this.cbKeepTogether.Checked = S.KeepTogether;
            this.cbKeepTogether_CheckedChanged(null, null);

            this.cbLeadingZero.Checked = S.LeadingZeroOnSeason;
            this.chkShowInTaskbar.Checked = S.ShowInTaskbar;
            this.cbTxtToSub.Checked = S.RenameTxtToSub;
            this.cbShowEpisodePictures.Checked = S.ShowEpisodePictures;
            this.chkHideMyShowsSpoilers.Checked = S.HideMyShowsSpoilers;
            this.chkHideWtWSpoilers.Checked = S.HideWtWSpoilers;
            this.cbAutoCreateFolders.Checked = S.AutoCreateFolders; 
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
            this.tbPercentDirty.Text = S.upgradeDirtyPercent.ToString();

            this.cbSearchRSS.Checked = S.SearchRSS;
            this.cbEpTBNs.Checked = S.EpTBNs;
            this.cbNFOShows.Checked = S.NFOShows;
            this.cbNFOEpisodes.Checked = S.NFOEpisodes;
            this.cbKODIImages.Checked = S.KODIImages;
            this.cbMeta.Checked = S.pyTivoMeta;
            this.cbMetaSubfolder.Checked = S.pyTivoMetaSubFolder;
            this.cbFolderJpg.Checked = S.FolderJpg;
            this.cbRenameCheck.Checked = S.RenameCheck;
            this.chkPreventMove.Checked = S.PreventMove;
            this.cbCheckuTorrent.Checked = S.CheckuTorrent;
            this.cbLookForAirdate.Checked = S.LookForDateInFilename;
            this.chkAutoMergeEpisodes.Checked = S.AutoMergeEpisodes;
            this.cbMonitorFolder.Checked = S.MonitorFolders;
            this.chkScheduledScan.Checked = S.RunPeriodicCheck();
            this.chkScanOnStartup.Checked = S.RunOnStartUp();
            this.domainUpDown1.SelectedItem = S.periodCheckHours;
            this.cbCleanUpDownloadDir.Checked = S.RemoveDownloadDirectoriesFiles;
            this.cbMissing.Checked = S.MissingCheck;
            this.cbxUpdateAirDate.Checked = S.CorrectFileDates;
            this.chkAutoSearchForDownloadedFiles.Checked = S.AutoSearchForDownloadedFiles;
            this.cbSearchLocally.Checked = S.SearchLocally;
            this.cbLeaveOriginals.Checked = S.LeaveOriginals;
            this.EnterPreferredLanguage = S.PreferredLanguage;

            this.cbEpThumbJpg.Checked = S.EpJPGs;
            this.cbSeriesJpg.Checked = S.SeriesJpg;
            this.cbXMLFiles.Checked = S.Mede8erXML;
            this.cbShrinkLarge.Checked = S.ShrinkLargeMede8erImages;
            this.cbFantArtJpg.Checked = S.FanArtJpg;


#if DEBUG
            System.Diagnostics.Debug.Assert(S.Tidyup != null);
#endif
            this.cbDeleteEmpty.Checked = S.Tidyup.DeleteEmpty;
            this.cbRecycleNotDelete.Checked = S.Tidyup.DeleteEmptyIsRecycle;
            this.cbEmptyIgnoreWords.Checked = S.Tidyup.EmptyIgnoreWords;
            this.txtEmptyIgnoreWords.Text = S.Tidyup.EmptyIgnoreWordList;
            this.cbEmptyIgnoreExtensions.Checked = S.Tidyup.EmptyIgnoreExtensions;
            this.txtEmptyIgnoreExtensions.Text = S.Tidyup.EmptyIgnoreExtensionList;
            this.cbEmptyMaxSize.Checked = S.Tidyup.EmptyMaxSizeCheck;
            this.txtEmptyMaxSize.Text = S.Tidyup.EmptyMaxSizeMB.ToString();
            this.txtSeasonFolderName.Text = S.defaultSeasonWord;



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
            switch(S.keepTogetherMode)
            {
                case TVSettings.KeepTogetherModes.All:
                default:
                    this.cbKeepTogetherMode.Text = "All";
                    break;
                case TVSettings.KeepTogetherModes.AllBut:
                    this.cbKeepTogetherMode.Text = "All but these";
                    break;
                case TVSettings.KeepTogetherModes.Just:
                    this.cbKeepTogetherMode.Text = "Just";
                    break;
            }

            switch (S.mode)
            {
                case TVSettings.BetaMode.ProductionOnly:
                default:
                    this.cbMode.Text = "Production";
                    break;
                case TVSettings.BetaMode.BetaToo:
                    this.cbMode.Text = "Beta";
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
                case TVSettings.FolderJpgIsType.SeasonPoster:
                    this.rbFolderSeasonPoster.Checked = true;
                    break;
                default:
                    this.rbFolderPoster.Checked = true;
                    break;
            }

            switch (S.MonitoredFoldersScanType)
            {
                case TVSettings.ScanType.Quick:
                    this.rdoQuickScan.Checked = true;
                    break;
                case TVSettings.ScanType.Recent:
                    this.rdoRecentScan.Checked = true;
                    break;
                default:
                    this.rdoFullScan.Checked = true;
                    break;
            }

            switch (S.SelectedKODIType)
            {
                case TVSettings.KODIType.Eden:
                    this.rdEden.Checked = true;
                    break;
                case TVSettings.KODIType.Frodo:
                    this.rdFrodo.Checked = true;
                    break;
                default:
                    this.rdBoth.Checked = true;
                    break;
            }

            if (S.ShowStatusColors != null)
            {
                foreach (
                    System.Collections.Generic.KeyValuePair<ShowStatusColoringType, Color> showStatusColor in
                        S.ShowStatusColors)
                {
                    ListViewItem item = new ListViewItem
                    {
                        Text = showStatusColor.Key.Text,
                        Tag = showStatusColor.Key,
                        ForeColor = showStatusColor.Value
                    };
                    item.SubItems.Add(TranslateColorToHtml(showStatusColor.Value));
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
            foreach (ShowItem show in shows)
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

        private void Browse(TextBox txt, string DefaultExt, int FilterIndex)
        {
            //rss =1, XML = 2, CSV = 3, TXT=4, HTML = 5
            this.saveFile.FileName = txt.Text;
            this.saveFile.DefaultExt = DefaultExt;
            this.saveFile.FilterIndex = FilterIndex;
            if (this.saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txt.Text = this.saveFile.FileName;
        }


        private void bnBrowseWTWXML_Click(object sender, System.EventArgs e)
        {
            this.Browse(this.txtWTWXML,"xml",2);
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
            this.txtKeepTogether.Enabled = (this.cbKeepTogether.Checked && this.cbKeepTogetherMode.Text != "All");
            this.cbKeepTogetherMode.Enabled = this.cbKeepTogether.Checked;
            this.label39.Enabled = this.cbKeepTogether.Checked;
        }

        private void bnBrowseMissingCSV_Click(object sender, System.EventArgs e)
        {
            this.Browse(this.txtMissingCSV,"csv",3);
        }


        private void bnBrowseWTWRSS_Click(object sender, System.EventArgs e)
        {
            this.Browse(this.txtWTWRSS, "rss", 1);
        }

        private void bnBrowseMissingXML_Click(object sender, System.EventArgs e)
        {
            this.Browse(this.txtMissingXML,"xml",2);
        }

        private void bnBrowseShowsTXT_Click(object sender, EventArgs e)
        {
            this.Browse(this.txtShowsTXTTo, "txt", 4);
        }

        private void bnBrowseRenamingXML_Click(object sender, System.EventArgs e)
        {
            this.Browse(this.txtRenamingXML,"xml",2);
        }

        private void bnBrowseFOXML_Click(object sender, System.EventArgs e)
        {
            this.Browse(this.txtFOXML,"xml",2);
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

            bool stxt = this.cbShowsTXT.Checked;
            this.txtShowsTXTTo.Enabled = stxt;
            this.bnBrowseShowsTXT.Enabled = stxt;

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
            Helpers.SysOpen(this.mDoc.SearchFolders[n]);
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
            foreach (string path in files)
            {
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
                Helpers.SysOpen((string) (this.RSSGrid[rowsIndex[0], 0].Value));
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
            catch (Exception e)
            {
                logger.Fatal(e,"Unhandled Exception in LoadLanguages");
                aborted = true;
            }
            TheTVDB.Instance.Unlock("Preferences-LoadLanguages");
            if (!aborted)
                this.BeginInvoke(LoadLanguageDone);
        }

        private void LoadLanguageDoneFunc()
        {
            FillLanguageList();
        }


        private void FillLanguageList()
        {
            TheTVDB.Instance.GetLock( "Preferences-FLL");
            this.cbLanguages.BeginUpdate();
            this.cbLanguages.Items.Clear();

            String pref = "";
            foreach (Language l in TheTVDB.Instance.LanguageList)
            {
                this.cbLanguages.Items.Add(l.name);

                if (EnterPreferredLanguage == l.abbreviation)
                    pref = l.name;
            }
            this.cbLanguages.EndUpdate();
            this.cbLanguages.Text = pref;
            this.cbLanguages.Enabled = true;

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

        private void cbMissing_CheckedChanged(object sender, System.EventArgs e)
        {
            this.ScanOptEnableDisable();
        }

        private void ScanOptEnableDisable()
        {
            bool e = this.cbMissing.Checked;
            this.tbMediaCenter.Enabled = e;

            this.cbSearchRSS.Enabled = e;
            this.cbSearchLocally.Enabled = e;
            this.cbCheckuTorrent.Enabled = e;

            bool e2 = e && this.cbSearchLocally.Checked;
            this.cbLeaveOriginals.Enabled = e2;
            this.cbCheckSABnzbd.Enabled = e2;
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
            Point pt = this.PointToScreen(bnMCPresets.Location);
            cmDefaults.Show(pt);
        }

        private void cbKeepTogetherMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtKeepTogether.Enabled = (this.cbKeepTogether.Checked && this.cbKeepTogetherMode.Text != "All");
        }

        private void domainUpDown1_KeyDown(object sender, KeyEventArgs e)
        {
                e.SuppressKeyPress = true;
            
        }

    }
}
