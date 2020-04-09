// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;
using TVRename.Forms;
using TVRename.Forms.Tools;
using TVRename.Forms.Utilities;
using TVRename.Ipc;
using TVRename.Properties;
using DataFormats = System.Windows.Forms.DataFormats;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using DragDropEffects = System.Windows.Forms.DragDropEffects;
using DragEventArgs = System.Windows.Forms.DragEventArgs;
using File = Alphaleonis.Win32.Filesystem.File;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using MessageBox = System.Windows.Forms.MessageBox;
using SystemColors = System.Drawing.SystemColors;

namespace TVRename
{
    // right click commands
    public enum RightClickCommands
    {
        kEpisodeGuideForShow = 1,
        kVisitTvSourceEpisode,
        kVisitTvSourceSeason,
        kVisitTvSourceSeries,
        kScanSpecificSeries,
        kWhenToWatchSeries,
        kForceRefreshSeries,
        kBtSearchFor,
        kActionIgnore,
        kActionBrowseForFile,
        kActionAction,
        kActionDelete,
        kActionIgnoreSeason,
        kEditShow,
        kEditSeason,
        kIgnoreSeason,
        kIncludeSeason,
        kDeleteShow,
        kUpdateImages,
        kActionRevert,
        kWatchBase = 1000,
        kOpenFolderBase = 2000,
        kSearchForBase =3000
    }

    ///  <summary>
    ///  Summary for UI
    ///  WARNING: If you change the name of this class, you will need to change the
    ///           'Resource File Name' property for the managed resource compiler tool
    ///           associated with all .resx files this class depends on.  Otherwise,
    ///           the designers will not be able to interact properly with localized
    ///           resources associated with this form.
    ///  </summary>
    // ReSharper disable once InconsistentNaming
    public partial class UI : Form, IRemoteActions
    {
        internal const string EXPLORE_PROXY = "http://www.tvrename.com/EXPLOREPROXY";
        internal const string WATCH_PROXY = "http://www.tvrename.com/WATCHPROXY";

        #region Delegates

        public delegate void ScanTypeDelegate(TVSettings.ScanType type);
        public delegate void ArgumentDelagate(string[] args);

        public readonly ScanTypeDelegate ScanAndDo;
        public readonly ArgumentDelagate RecieveArgumentDelagate;

        #endregion

        private int busy;
        private readonly TVDoc mDoc;
        private bool internalCheckChange;
        private int lastDlRemaining;

        private MyListView lvAction;
        private List<string> mFoldersToOpen;
        private TVSettings.ScanType uiScanType; 
        private int mInternalChange;
        private List<FileInfo> mLastFl;
        private Point mLastNonMaximizedLocation;
        private Size mLastNonMaximizedSize;
        private readonly AutoFolderMonitor mAutoFolderMonitor;
        private bool treeExpandCollapseToggle = true;

        private ItemList mLastActionsClicked;
        private ProcessedEpisode mLastEpClicked;
        private ProcessedEpisode mLastEpClickedWtw;
        private ProcessedEpisode mLastEpClickedScan;
        private ProcessedSeason mLastProcessedSeasonClicked;
        private List<ShowItem> mLastShowsClicked;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private bool IsBusy => busy!=0;

        public UI(TVDoc doc, [NotNull] TVRenameSplash splash, bool showUi)
        {
            mDoc = doc;

            busy = 0;

            mInternalChange = 0;
            mFoldersToOpen = new List<string>();

            internalCheckChange = false;

            InitializeComponent();

            ScanAndDo = ScanAndAction;
            RecieveArgumentDelagate = RecieveArguments;

            try
            {
                bool layoutLoadSuccess = LoadLayoutXml();
                if (!layoutLoadSuccess)
                {
                    Logger.Info("Error loading layout XML, but no error raised");
                }
            }
            catch (Exception e)
            {
                // silently fail, doesn't matter too much
                Logger.Info(e, "Error loading layout XML");
            }

            lvWhenToWatch.ListViewItemSorter = new DateSorterWtw(0);

            if (mDoc.Args.Hide || !showUi)
            {
                WindowState = FormWindowState.Minimized;
                Visible = false;
                Hide();
            }

            tmrPeriodicScan.Enabled = false;

            UpdateSplashStatus(splash, "Filling Shows");
            mDoc.Library.GenDict();
            FillMyShows();
            UpdateSearchButtons();
            ClearInfoWindows();
            UpdateSplashPercent(splash, 10);
            UpdateSplashStatus(splash, "Updating WTW");
            mDoc.DoWhenToWatch(true,true,WindowState==FormWindowState.Minimized);
            UpdateSplashPercent(splash, 40);
            FillWhenToWatchList();
            UpdateSplashPercent(splash, 60);
            UpdateSplashStatus(splash, "Write Upcoming");
            mDoc.WriteUpcoming();
            UpdateSplashStatus(splash, "Write Recent");
            mDoc.WriteRecent();
            UpdateSplashStatus(splash, "Setting Notifications");
            ShowHideNotificationIcon();

            int t = TVSettings.Instance.StartupTab;
            if (t < tabControl1.TabCount)
            {
                tabControl1.SelectedIndex = TVSettings.Instance.StartupTab;
            }

            tabControl1_SelectedIndexChanged(null, null);
            UpdateSplashStatus(splash, "Creating Monitors");

            mAutoFolderMonitor = new AutoFolderMonitor(mDoc, this);

            tmrPeriodicScan.Interval = TVSettings.Instance.PeriodicCheckPeriod();

            UpdateSplashStatus(splash, "Starting Monitor");
            if (TVSettings.Instance.MonitorFolders)
            {
                mAutoFolderMonitor.Start();
            }

            tmrPeriodicScan.Enabled = TVSettings.Instance.RunPeriodicCheck();

            UpdateSplashStatus(splash, "Running autoscan");
        }

        private void RecieveArguments([NotNull] string[] args)
        {
            // Send command-line arguments to already running instance

            // Parse command line arguments
            CommandLineArgs localArgs = new CommandLineArgs(new ReadOnlyCollection<string>(args));

            bool previousRenameBehavior = TVSettings.Instance.RenameCheck;
            // Temporarily override behavior for renaming folders
            TVSettings.Instance.RenameCheck = localArgs.RenameCheck;
            // Temporarily override behavior for missing folders
            mDoc?.Args.TemporarilyUse(localArgs);

            ProcessArgs(localArgs);

            //Revert the settings
            TVSettings.Instance.RenameCheck = previousRenameBehavior;
            mDoc?.Args.RevertFromTempUse();
        }
        private void ScanAndAction(TVSettings.ScanType type)
        {
            UiScan(null,true,type);
            ActionAction(true,true);
        }

        private static void UpdateSplashStatus([NotNull] TVRenameSplash splashScreen, string text)
        {
            if (splashScreen.IsHandleCreated) {
                Logger.Info($"Splash Screen Updated with: {text}");
                splashScreen.Invoke((System.Action)delegate { splashScreen.UpdateStatus(text); });
            }
        }

        private static void UpdateSplashPercent([NotNull] TVRenameSplash splashScreen, int num)
        {
            if (splashScreen.IsHandleCreated)
            {
                splashScreen.Invoke((System.Action) delegate { splashScreen.UpdateProgress(num); });
            }
        }

        private void ClearInfoWindows() => ClearInfoWindows("");

        private void ClearInfoWindows(string defaultText)
        {
            SetHtmlBody( webImages, ShowHtmlHelper.CreateOldPage(defaultText));
            SetHtmlBody(webInformation, ShowHtmlHelper.CreateOldPage(defaultText));
        }

        private void MoreBusy() => Interlocked.Increment(ref busy);

        private void LessBusy() => Interlocked.Decrement(ref busy);

        private void ProcessArgs([NotNull] CommandLineArgs a)
        {
            const bool UNATTENDED = true;

            if (a.Hide)
            {
                WindowState = FormWindowState.Minimized;
            }

            if (a.QuickUpdate)
            {
                mDoc.DoDownloadsFG(UNATTENDED, WindowState == FormWindowState.Minimized);
            }

            if (a.ForceUpdate)
            {
                txtDLStatusLabel.Text = "Background download: Accuracy Check running";
                mDoc.TVDBServerAccuracyCheck(UNATTENDED,WindowState==FormWindowState.Minimized);
                txtDLStatusLabel.Text = "Background download: Idle";
            }

            if (a.ForceRefresh)
            {
                ForceRefresh(mDoc.Library.GetSortedShowItems(), UNATTENDED);
            }

            if (a.Scan)
            {
                UiScan(null, UNATTENDED, TVSettings.ScanType.Full);
            }

            if (a.QuickScan)
            {
                UiScan(null, UNATTENDED, TVSettings.ScanType.Quick);
            }

            if (a.RecentScan)
            {
                UiScan(null, UNATTENDED, TVSettings.ScanType.Recent);
            }

            if (a.DoAll)
            {
                ActionAction(true, UNATTENDED);
            }

            if (a.Save)
            {
                mDoc.WriteXMLSettings();
                SaveCaches();
            }

            if (a.Quit)
            {
                Close();
            }
        }

        private void UpdateSearchButtons()
        {
            string name = TVDoc.GetSearchers().CurrentSearch.Name;
            bool enabled = name.HasValue();

            btnActionBTSearch.Enabled = enabled;
            btnWTWBTSearch.Enabled = enabled;

            btnWTWBTSearch.Text = UseCustom(lvWhenToWatch) ? "Search" : name;
            btnActionBTSearch.Text = UseCustom(lvAction) ? "Search" : name;

            FillEpGuideHtml();
        }

        private void visitWebsiteToolStripMenuItem_Click(object sender, EventArgs eventArgs) =>
            Helpers.SysOpen("http://tvrename.com");

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Close();

        private static bool UseCustom([NotNull] ListView view)
        {
            foreach (ListViewItem lvi in view.SelectedItems)
            {
                if (!(lvi.Tag is ProcessedEpisode pe))
                {
                    continue;
                }

                if (!pe.Show.UseCustomSearchUrl)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(pe.Show.CustomSearchUrl))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private void UI_Load(object sender, EventArgs e)
        {
            ShowInTaskbar = TVSettings.Instance.ShowInTaskbar && !mDoc.Args.Hide;

            foreach (TabPage tp in tabControl1.TabPages) // grr! why does it go white?
            {
                tp.BackColor = SystemColors.Control;
            }

            // MAH: Create a "Clear" button in the Filter Text Box
            Button filterButton = new Button
            {
                Size = new Size(16, 16),
                Cursor = Cursors.Default,
                Image = Resources.DeleteSmall,
                Name = "Clear"
            };

            int clientSizeHeight = (filterTextBox.ClientSize.Height - 16) / 2;
            filterButton.Location = new Point(filterTextBox.ClientSize.Width - filterButton.Width, clientSizeHeight+1);

            filterButton.Click += filterButton_Click;
            filterTextBox.Controls.Add(filterButton);
            // Send EM_SETMARGINS to prevent text from disappearing underneath the button
            NativeMethods.SendMessage(filterTextBox.Handle, 0xd3, (IntPtr) 2, (IntPtr) (filterButton.Width << 16));

            betaToolsToolStripMenuItem.Visible = TVSettings.Instance.IncludeBetaUpdates();

            Show();
            UI_LocationChanged(null, null);
            UI_SizeChanged(null, null);

            backgroundDownloadToolStripMenuItem.Checked = TVSettings.Instance.BGDownload;
            offlineOperationToolStripMenuItem.Checked = TVSettings.Instance.OfflineMode;
            BGDownloadTimer.Interval = 10000; // first time
            if (TVSettings.Instance.BGDownload)
            {
                BGDownloadTimer.Start();
            }

            UpdateTimer.Start();

            quickTimer.Start();

            if (TVSettings.Instance.RunOnStartUp())
            {
                RunAutoScan("Startup Scan");
            }
        }

        // MAH: Added in support of the Filter TextBox Button
        private void filterButton_Click(object sender, EventArgs e) => filterTextBox.Clear();

        private ListView ListViewByName([NotNull] string name)
        {
            switch (name)
            {
                case "WhenToWatch":
                    return lvWhenToWatch;
                case "AllInOne":
                    return lvAction;
                default:
                    throw new ArgumentException("Inappropriate ListViewParameter " + name);
            }
        }

        private void flushImageCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsBusy)
            {
                MessageBox.Show("Can't refresh until background download is complete");
                return;
            }

            UpdateImages(mDoc.Library.GetSortedShowItems());
            FillMyShows();
            FillEpGuideHtml();
        }

        private void flushCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsBusy)
            {
                MessageBox.Show("Can't refresh until background download is complete");
                return;
            }

            DialogResult res = MessageBox.Show(
                "Are you sure you want to remove all " +
                "locally stored TheTVDB information?  This information will have to be downloaded again.  You " +
                "can force the refresh of a single show by holding down the \"Control\" key while clicking on " +
                "the \"Refresh\" button in the \"My Shows\" tab.",
                "Force Refresh All", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (res == DialogResult.Yes)
            {
                TheTVDB.LocalCache.Instance.ForgetEverything();
                TVmaze.LocalCache.Instance.ForgetEverything();
                FillMyShows();
                FillEpGuideHtml();
                FillWhenToWatchList();
                BGDownloadTimer_QuickFire();
            }
        }

        private bool LoadWidths([NotNull] XElement xml)
        {
            string forwho = xml.Attribute("For")?.Value;

            if (forwho is null)
            {
                return false;
            }

            ListView lv = ListViewByName(forwho);
            if (lv is null)
            {
                return true;
            }

            int c = 0;
            foreach (XElement w in xml.Descendants("Width"))
            {
                if (c >= lv.Columns.Count)
                {
                    return false;
                }

                lv.Columns[c++].Width = XmlConvert.ToInt32(w.Value);
            }

            return true;
        }

        private bool LoadLayoutXml()
        {
            if (mDoc.Args.Hide)
            {
                return true;
            }

            bool ok = true;

            string fn = PathManager.UILayoutFile.FullName;
            if (!File.Exists(fn))
            {
                return true;
            }

            XElement x = XElement.Load(fn);

            if (x.Name.LocalName != "TVRename")
            {
                return false;
            }

            if (x.Attribute("Version")?.Value != "2.1")
            {
                return false;
            }

            if (!x.Descendants("Layout").Any())
            {
                return false;
            }

            SetWindowSize(x.Descendants("Layout").Descendants("Window").First());

            foreach (XElement widthXmlElement in x.Descendants("Layout").Descendants("ColumnWidths"))
            {
                ok = LoadWidths(widthXmlElement) && ok;
            }

            SetSplitter(x.Descendants("Layout").Descendants("Splitter").First());

            return ok;
        }

        private void SetSplitter([NotNull] XElement x)
        {
            splitContainer1.SplitterDistance = int.Parse(x.Attribute("Distance")?.Value??"100");
            splitContainer1.Panel2Collapsed = bool.Parse(x.Attribute("HTMLCollapsed")?.Value ?? "false");
            if (splitContainer1.Panel2Collapsed)
            {
                btnHideHTMLPanel.Image = Resources.FillLeft;
            }
        }

        private void SetWindowSize([NotNull] XElement x)
        {
            SetSize(x.Descendants("Size").First());
            SetLocation(x.Descendants("Location").First());

            WindowState = x.ExtractBool("Maximized",false)
                ? FormWindowState.Maximized
                : FormWindowState.Normal;
        }

        private void SetLocation([NotNull] XElement x)
        {
            XAttribute valueX = x.Attribute("X");
            XAttribute valueY = x.Attribute("Y");

            if (valueX is null)
            {
                Logger.Error($"Missing X from {x}");
            }
            if ( valueY==null)
            {
                Logger.Error($"Missing Y from {x}");
            }

            int xloc = valueX ==null ? 100 : int.Parse(valueX.Value);
            int yloc = valueY is null ? 100 : int.Parse(valueY.Value);

            Location = new Point(xloc, yloc);
        }

        private void SetSize([NotNull] XElement x)
        {
            XAttribute valueX = x.Attribute("Width");
            XAttribute valueY = x.Attribute("Height");

            if (valueX is null)
            {
                Logger.Error($"Missing Width from {x}");
            }
            if (valueY is null)
            {
                Logger.Error($"Missing Height from {x}");
            }

            int xsize = valueX is null ? 100 : int.Parse(valueX.Value);
            int ysize = valueY is null ? 100 : int.Parse(valueY.Value);

            Size = new Size(xsize, ysize);
        }

        private bool SaveLayoutXml()
        {
            if (mDoc.Args.Hide)
            {
                return true;
            }

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true
            };

            using (XmlWriter writer = XmlWriter.Create(PathManager.UILayoutFile.FullName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("TVRename");
                writer.WriteAttributeToXml("Version", "2.1");
                writer.WriteStartElement("Layout");
                writer.WriteStartElement("Window");

                writer.WriteStartElement("Size");
                writer.WriteAttributeToXml("Width", mLastNonMaximizedSize.Width);
                writer.WriteAttributeToXml("Height", mLastNonMaximizedSize.Height);
                writer.WriteEndElement(); // size

                writer.WriteStartElement("Location");
                writer.WriteAttributeToXml("X", mLastNonMaximizedLocation.X);
                writer.WriteAttributeToXml("Y", mLastNonMaximizedLocation.Y);
                writer.WriteEndElement(); // Location

                writer.WriteElement("Maximized", WindowState == FormWindowState.Maximized);

                writer.WriteEndElement(); // window

                WriteColWidthsXml("WhenToWatch", writer);
                WriteColWidthsXml("AllInOne", writer);

                writer.WriteStartElement("Splitter");
                writer.WriteAttributeToXml("Distance", splitContainer1.SplitterDistance);
                writer.WriteAttributeToXml("HTMLCollapsed", splitContainer1.Panel2Collapsed);
                writer.WriteEndElement(); // splitter

                writer.WriteEndElement(); // Layout
                writer.WriteEndElement(); // tvrename
                writer.WriteEndDocument();
            }

            return true;
        }

        private void WriteColWidthsXml([NotNull] string thingName, XmlWriter writer)
        {
            ListView lv = ListViewByName(thingName);
            if (lv is null)
            {
                return;
            }

            writer.WriteStartElement("ColumnWidths");
            writer.WriteAttributeToXml("For", thingName);
            foreach (ColumnHeader lvc in lv.Columns)
            {
                writer.WriteElement("Width", lvc.Width);
            }

            // ReSharper disable once CommentTypo
            writer.WriteEndElement(); // columnwidths
        }

        private void UI_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (mDoc.Dirty() && !mDoc.Args.Unattended && !mDoc.Args.Hide)
                {
                    DialogResult res = MessageBox.Show(
                        "Your changes have not been saved.  Do you wish to save before quitting?", "Unsaved data",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                    switch (res)
                    {
                        case DialogResult.Yes:
                            mDoc.WriteXMLSettings();
                            break;
                        case DialogResult.Cancel:
                            e.Cancel = true;
                            break;
                        case DialogResult.No:
                            break;
                        case DialogResult.None:
                            break;
                        case DialogResult.OK:
                            break;
                        case DialogResult.Abort:
                            break;
                        case DialogResult.Retry:
                            break;
                        case DialogResult.Ignore:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (mDoc.Dirty() && (mDoc.Args.Unattended || mDoc.Args.Hide))
                {
                    //We have to assume that they wanted to save any settings
                    mDoc.WriteXMLSettings();
                }

                if (!e.Cancel)
                {
                    SaveLayoutXml();
                    mDoc.TidyCaches();
                    mDoc.Closing();
                    mAutoFolderMonitor.Dispose();
                    BGDownloadTimer.Dispose();
                    UpdateTimer.Dispose();
                    quickTimer.Dispose();
                    refreshWTWTimer.Dispose();
                    statusTimer.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + "\r\n\r\n" + ex.StackTrace, "Form Closing Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void ChooseSiteMenu([NotNull] ToolStripSplitButton btn)
        {
            btn.DropDownItems.Clear();

            foreach (SearchEngine search in TVDoc.GetSearchers().Where(engine => engine.Name.HasValue()))
            {
                ToolStripMenuItem tsi = new ToolStripMenuItem(search.Name) { Tag = search };
                tsi.Font = new Font(tsi.Font.FontFamily,9,FontStyle.Regular);
                btn.DropDownItems.Add(tsi);
            }
        }

        private void FillMyShows()
        {
            ProcessedSeason currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
            ShowItem currentSi = TreeNodeToShowItem(MyShowTree.SelectedNode);

            List<ShowItem> expanded = new List<ShowItem>();
            foreach (TreeNode n in MyShowTree.Nodes)
            {
                if (n.IsExpanded)
                {
                    expanded.Add(TreeNodeToShowItem(n));
                }
            }

            MyShowTree.BeginUpdate();

            MyShowTree.Nodes.Clear();
            List<KeyValuePair<int, ShowItem>> sil = mDoc.Library.ToList();
            sil.Sort((a, b) => string.Compare(GenerateShowUIName(a.Value), GenerateShowUIName(b.Value), StringComparison.OrdinalIgnoreCase));

            ShowFilter filter = TVSettings.Instance.Filter;
            foreach (KeyValuePair<int, ShowItem> s in sil)
            {
                ShowItem si = s.Value;
                if (filter.Filter(si)
                    & (string.IsNullOrEmpty(filterTextBox.Text) || si.NameMatchFilters(filterTextBox.Text)))
                {
                    TreeNode tvn = AddShowItemToTree(si);
                    if (expanded.Contains(si))
                    {
                        tvn.Expand();
                    }
                }
            }

            foreach (ShowItem si in expanded)
            {
                foreach (TreeNode n in MyShowTree.Nodes)
                {
                    if (TreeNodeToShowItem(n) == si)
                    {
                        n.Expand();
                    }
                }
            }

            if (currentSeas != null)
            {
                SelectSeason(currentSeas);
            }
            else if (currentSi != null)
            {
                SelectShow(currentSi);
            }

            MyShowTree.EndUpdate();
        }

        [NotNull]
        private static string QuickStartGuide() => "https://www.tvrename.com/manual/quickstart/";

        private void ShowQuickStartGuide()
        {
            tabControl1.SelectTab(tbMyShows);
            webInformation.Navigate(QuickStartGuide());
            webImages.Navigate(QuickStartGuide());
        }

        private void FillEpGuideHtml()
        {
            if (MyShowTree.Nodes.Count == 0) 
            {
                ShowQuickStartGuide();
            }
            else
            {
                TreeNode n = MyShowTree.SelectedNode;
                FillEpGuideHtml(n);
            }
        }

        private static ShowItem TreeNodeToShowItem([CanBeNull] TreeNode n)
        {
            if (n is null)
            {
                return null;
            }

            switch (n.Tag)
            {
                case ShowItem si:
                    return si;

                case ProcessedEpisode pe:
                    return pe.Show;

                case ProcessedSeason seas when seas.Episodes.Count == 0:
                    return null;

                case ProcessedSeason seas:
                    return seas.Show;

                default:
                    return null;
            }
        }

        [CanBeNull]
        private static ProcessedSeason TreeNodeToSeason([CanBeNull] TreeNode n)
        {
            ProcessedSeason seas = n?.Tag as ProcessedSeason;
            return seas;
        }

        private void FillEpGuideHtml([CanBeNull] TreeNode n)
        {
            if (n is null)
            {
                FillEpGuideHtml(null, -1);
                return;
            }

            if (n.Tag is ProcessedEpisode pe)
            {
                FillEpGuideHtml(pe.Show, pe.AppropriateSeasonNumber);
                return;
            }

            ProcessedSeason seas = TreeNodeToSeason(n);
            if (seas != null)
            {
                // we have a TVDB season, but need to find the equivalent one in our local processed episode collection
                if (seas.Episodes.Count > 0)
                {
                    FillEpGuideHtml(seas.Show, seas.SeasonNumber);
                    return;
                }

                FillEpGuideHtml(null, -1);
                return;
            }

            FillEpGuideHtml(TreeNodeToShowItem(n), -1);
        }

        private void FillEpGuideHtml(ShowItem si, int snum)
        {
            if (tabControl1.SelectedTab != tbMyShows)
            {
                return;
            }

            if (si is null)
            {
                ClearInfoWindows();
                return;
            }

            if (si.TheSeries() is null)
            {
                ClearInfoWindows("Not downloaded, or not available");
                return;
            }

            if (TVSettings.Instance.OfflineMode || TVSettings.Instance.ShowBasicShowDetails)
            {
                if (snum >= 0 && si.AppropriateSeasons().ContainsKey(snum))
                {
                    ProcessedSeason s = si.AppropriateSeasons()[snum];
                    SetHtmlBody(webInformation, ShowHtmlHelper.CreateOldPage(si.GetSeasonHtmlOverviewOffline(s)));
                    SetHtmlBody(webImages, ShowHtmlHelper.CreateOldPage(si.GetSeasonImagesHtmlOverview(s)));
                }
                else
                {
                    // no epnum specified, just show an overview
                    SetHtmlBody(webInformation, ShowHtmlHelper.CreateOldPage(si.GetShowHtmlOverviewOffline()));
                    SetHtmlBody(webImages, ShowHtmlHelper.CreateOldPage(si.GetShowImagesHtmlOverview()));
                }

                return;
            }

            if (snum >= 0 && si.AppropriateSeasons().ContainsKey(snum))
            {
                ProcessedSeason s = si.AppropriateSeasons()[snum];
                SetHtmlBody(webImages, ShowHtmlHelper.CreateOldPage(si.GetSeasonImagesHtmlOverview(s)));

                SetHtmlBody(webInformation, si.GetSeasonHtmlOverview(s, true));
            }
            else
            {
                // no epnum specified, just show an overview
                SetHtmlBody(webImages, ShowHtmlHelper.CreateOldPage(si.GetShowImagesHtmlOverview()));

                SetHtmlBody(webInformation, si.GetShowHtmlOverview(true));
            }
        }

        private static void SetHtmlBody([NotNull] WebBrowser web, string body)
        {
            try
            {
                web.DocumentText = body; 
            }
            catch (COMException ex)
            {
                //Fail gracefully - no RHS episode guide is not too big of a problem.
                Logger.Warn(ex,"Could not update UI for the show/series information pane");
            }
            catch (Exception ex)
            {
                //Fail gracefully - no RHS episode guide is not too big of a problem.
                Logger.Error(ex);
            }
            web.Update();
        }

        private static void TvSourceFor([CanBeNull] ProcessedEpisode e)
        {
            if (e != null)
            {
                Helpers.SysOpen(e.WebsiteUrl);
            }
        }

        private static void TvSourceFor([CanBeNull] ProcessedSeason seas)
        {
            if (seas != null)
            {
                Helpers.SysOpen(seas.WebsiteUrl);
            }
        }

        private static void TvSourceFor( [CanBeNull] ShowItem si)
        {
            if (si != null)
            {
                Helpers.SysOpen(si.WebsiteUrl);
            }
        }

        private void menuSearchSites_ItemClicked(object sender, [NotNull] ToolStripItemClickedEventArgs e)
        {
            mDoc.SetSearcher((SearchEngine) e.ClickedItem.Tag);
            UpdateSearchButtons();
        }

        private void bnWhenToWatchCheck_Click(object sender, EventArgs e) => RefreshWTW(true,false);

        private void FillWhenToWatchList()
        {
            mInternalChange++;
            lvWhenToWatch.BeginUpdate();

            int dd = TVSettings.Instance.WTWRecentDays;

            lvWhenToWatch.Groups["justPassed"].Header =
                "Aired in the last " + dd + " day" + (dd == 1 ? "" : "s");

            // try to maintain selections if we can
            List<ProcessedEpisode> selections = new List<ProcessedEpisode>();
            foreach (ListViewItem lvi in lvWhenToWatch.SelectedItems)
            {
                selections.Add((ProcessedEpisode) lvi.Tag);
            }

            ProcessedSeason currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
            ShowItem currentShowItem = TreeNodeToShowItem(MyShowTree.SelectedNode);

            lvWhenToWatch.Items.Clear();

            List<DateTime> bolded = new List<DateTime>();
            DirFilesCache dfc = new DirFilesCache();

            IEnumerable<ProcessedEpisode> recentEps = mDoc.Library.GetRecentAndFutureEps(dd);

            foreach (ProcessedEpisode ei in recentEps)
            {
                DateTime? dt = ei.GetAirDateDt(true);
                if (dt != null)
                {
                    bolded.Add(dt.Value);
                }

                ListViewItem lvi = new ListViewItem {Text = ""};
                for (int i = 0; i < 8; i++)
                {
                    lvi.SubItems.Add("");
                }

                UpdateWtw(dfc, ei, lvi);

                lvWhenToWatch.Items.Add(lvi);

                foreach (ProcessedEpisode pe in selections)
                {
                    if (!pe.SameAs(ei))
                    {
                        continue;
                    }

                    lvi.Selected = true;
                    break;
                }
            }

            lvWhenToWatch.Sort();

            lvWhenToWatch.EndUpdate();
            calCalendar.BoldedDates = bolded.ToArray();

            if (currentSeas != null)
            {
                SelectSeason(currentSeas);
            }
            else if (currentShowItem != null)
            {
                SelectShow(currentShowItem);
            }

            UpdateToolstripWTW();
            mInternalChange--;
        }

        private void lvWhenToWatch_ColumnClick(object sender, [NotNull] ColumnClickEventArgs e)
        {
            int col = e.Column;
            // 3 - 6 = do date sort on 3
            // 1 or 2 = number sort
            // all others, text sort

            lvWhenToWatch.ShowGroups = false;

            switch (col)
            {
                case 3:
                case 4:
                case 5:
                case 6:
                    lvWhenToWatch.ShowGroups = true;
                    lvWhenToWatch.ListViewItemSorter = new DateSorterWtw(col);
                    break;
                case 1:
                case 2:
                    lvWhenToWatch.ListViewItemSorter = new NumberAsTextSorter(col);
                    break;
                default:
                    lvWhenToWatch.ListViewItemSorter = new TextSorter(col);
                    break;
            }

            lvWhenToWatch.Sort();
            lvWhenToWatch.Refresh();
        }

        private void lvWhenToWatch_Click(object sender, EventArgs e)
        {
            UpdateSearchButtons();

            if (lvWhenToWatch.SelectedIndices.Count == 0)
            {
                txtWhenToWatchSynopsis.Text = "";
                mLastEpClickedWtw = null;
                return;
            }

            int n = lvWhenToWatch.SelectedIndices[0];

            ProcessedEpisode ei = (ProcessedEpisode) lvWhenToWatch.Items[n].Tag;
            mLastEpClicked = ei;
            mLastEpClickedWtw = ei;

            if (TVSettings.Instance.HideWtWSpoilers &&
                (ei.HowLong() != "Aired" || lvWhenToWatch.Items[n].ImageIndex == 1))
            {
                txtWhenToWatchSynopsis.Text = "[Spoilers Hidden]";
            }
            else if (ei.Type == ProcessedEpisode.ProcessedEpisodeType.merged)
            {
                txtWhenToWatchSynopsis.Text = string.Join(Environment.NewLine+ Environment.NewLine, ei.SourceEpisodes.Select(episode => episode.Overview));
            }
            else
            {
                txtWhenToWatchSynopsis.Text = ei.Overview;
            }

            mInternalChange++;
            DateTime? dt = ei.GetAirDateDt(true);
            if (dt != null)
            {
                calCalendar.SelectionStart = (DateTime) dt;
                calCalendar.SelectionEnd = (DateTime) dt;
            }

            mInternalChange--;
        }

        private void lvWhenToWatch_DoubleClick(object sender, EventArgs e)
        {
            if (lvWhenToWatch.SelectedItems.Count == 0)
            {
                return;
            }

            ProcessedEpisode ei = (ProcessedEpisode) lvWhenToWatch.SelectedItems[0].Tag;
            List<FileInfo> fl = FinderHelper.FindEpOnDisk(null, ei);
            if (fl.Count > 0)
            {
                Helpers.SysOpen(fl[0].FullName);
                return;
            }

            // Don't have the episode.  Scan or search?

            switch (TVSettings.Instance.WTWDoubleClick)
            {
                case TVSettings.WTWDoubleClickAction.Search:
                    bnWTWBTSearch_Click(null, null);
                    break;
                case TVSettings.WTWDoubleClickAction.Scan:
                    UiScan(new List<ShowItem> {ei.Show}, false, TVSettings.ScanType.SingleShow);
                    tabControl1.SelectTab(tbAllInOne);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void calCalendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            if (mInternalChange != 0)
            {
                return;
            }

            DateTime dt = calCalendar.SelectionStart;
            bool first = true;

            foreach (ListViewItem lvi in lvWhenToWatch.Items)
            {
                lvi.Selected = false;

                ProcessedEpisode ei = (ProcessedEpisode) lvi.Tag;
                DateTime? dt2 = ei.GetAirDateDt(true);
                if (dt2 != null)
                {
                    double h = dt2.Value.Subtract(dt).TotalHours;
                    if (h >= 0 && h < 24.0)
                    {
                        lvi.Selected = true;
                        if (first)
                        {
                            lvi.EnsureVisible();
                            first = false;
                        }
                    }
                }
            }

            lvWhenToWatch.Focus();
        }

        // ReSharper disable once InconsistentNaming
        private void RefreshWTW(bool doDownloads,bool unattended)
        {
            if (doDownloads)
            {
                if (!mDoc.DoDownloadsFG(unattended,WindowState==FormWindowState.Minimized))
                {
                    return;
                }
            }

            mInternalChange++;
            mDoc.DoWhenToWatch(true,unattended,WindowState==FormWindowState.Minimized);
            FillMyShows();
            FillWhenToWatchList();
            mInternalChange--;

            mDoc.WriteUpcoming();
            mDoc.WriteRecent();
        }

        private void refreshWTWTimer_Tick(object sender, EventArgs e)
        {
            if (IsBusy)
            {
                return;
            }

            RefreshWTW(false,true);
        }

        // ReSharper disable once InconsistentNaming
        private void UpdateToolstripWTW()
        {
            // update toolstrip text too
            List<ProcessedEpisode> next1 = mDoc.Library.NextNShows(1, 0, 36500);

            tsNextShowTxt.Text = "Next airing: ";
            if (next1.Count >= 1)
            {
                ProcessedEpisode ei = next1[0];
                tsNextShowTxt.Text += CustomEpisodeName.NameForNoExt(ei, CustomEpisodeName.OldNStyle(1)) + ", " + ei.HowLong() +
                                      " (" + ei.DayOfWeek() + ", " + ei.TimeOfDay() + ")";
            }
            else
            {
                tsNextShowTxt.Text += "---";
            }
        }

        private void bnWTWBTSearch_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lvWhenToWatch.SelectedItems)
            {
                TVDoc.SearchForEpisode((ProcessedEpisode) lvi.Tag);
            }
        }

        private void NavigateTo(object sender, [NotNull] WebBrowserNavigatingEventArgs e)
        {
            if (e.Url is null)
            {
                return;
            }

            string url = e.Url.AbsoluteUri;

            if (string.Compare(url, "about:blank", StringComparison.Ordinal) == 0)
            {
                return; // don't intercept about:blank
            }

            if (url == QuickStartGuide())
            {
                return; // let the quick-start guide be shown
            }

            if (url.Contains(@"ieframe.dll"))
            {
                url = e.Url.Fragment.Substring(1);
            }

            if (url.StartsWith(EXPLORE_PROXY, StringComparison.InvariantCultureIgnoreCase))
            {
                e.Cancel = true;
                string path = HttpUtility.UrlDecode(url.Substring(EXPLORE_PROXY.Length).Replace('/', '\\'));
                Helpers.SysOpen("explorer", $"/select, \"{path}\"");
                return;
            }

            if (url.StartsWith(WATCH_PROXY, StringComparison.InvariantCultureIgnoreCase))
            {
                e.Cancel = true;
                string fileName = HttpUtility.UrlDecode(url.Substring(WATCH_PROXY.Length)).Replace('/', '\\');
                Helpers.SysOpen(fileName);
                return;
            }

            if (url.IsHttpLink() || url.IsFileLink())
            {
                e.Cancel = true;
                Helpers.SysOpen(e.Url.AbsoluteUri);
            }
        }

        private void notifyIcon1_Click(object sender, MouseEventArgs e)
        {
            // double-click of notification icon causes a click then double-click event, 
            // so we need to do a timeout before showing the single click's popup
            tmrShowUpcomingPopup.Start();
        }

        private void tmrShowUpcomingPopup_Tick(object sender, EventArgs e)
        {
            tmrShowUpcomingPopup.Stop();
            UpcomingPopup up = new UpcomingPopup(mDoc);
            up.Show();
        }

        public void FocusWindow()
        {
            if (!TVSettings.Instance.ShowInTaskbar)
            {
                Show();
            }

            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }

            Activate();
        }

        private void notifyIcon1_DoubleClick(object sender, MouseEventArgs e)
        {
            tmrShowUpcomingPopup.Stop();
            FocusWindow();
        }

        private void buyMeADrinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BuyMeADrink bmad = new BuyMeADrink();
            bmad.ShowDialog();
        }

        public void GotoEpguideFor(ShowItem si, bool changeTab)
        {
            if (changeTab)
            {
                tabControl1.SelectTab(tbMyShows);
            }

            FillEpGuideHtml(si, -1);
        }

        public void GotoEpguideFor([NotNull] ProcessedEpisode ep, bool changeTab)
        {
            if (changeTab)
            {
                tabControl1.SelectTab(tbMyShows);
            }

            SelectSeason(ep.AppropriateProcessedSeason);
        }

        private void RightClickOnMyShows(ShowItem si, Point pt)
        {
            mLastShowsClicked = new List<ShowItem> {si};
            mLastEpClicked = null;
            mLastProcessedSeasonClicked = null;
            mLastActionsClicked = null;
            BuildRightClickMenu(pt);
        }

        private void RightClickOnMyShows([NotNull] ProcessedSeason seas, Point pt)
        {
            mLastShowsClicked = new List<ShowItem> {seas.Show};
            mLastEpClicked = null;
            mLastProcessedSeasonClicked = seas;
            mLastActionsClicked = null;
            BuildRightClickMenu(pt);
        }

        private void WtwRightClickOnShow([NotNull] List<ProcessedEpisode> eps, Point pt)
        {
            if (eps.Count == 0)
            {
                return;
            }

            ProcessedEpisode ep = eps[0];

            List<ShowItem> sis = new List<ShowItem>();
            foreach (ProcessedEpisode e in eps)
            {
                sis.Add(e.Show);
            }

            mLastEpClicked = ep;
            mLastEpClickedWtw = ep;

            mLastShowsClicked = sis;
            mLastProcessedSeasonClicked = ep?.AppropriateProcessedSeason;
            mLastActionsClicked = null;
            BuildRightClickMenu(pt);
        }

        private void MenuGuideAndTvdb(bool addSep)
        {
            if (mLastShowsClicked is null || mLastShowsClicked.Count != 1)
            {
                return; // nothing or multiple selected
            }

            ShowItem si = mLastShowsClicked[0];
            ProcessedSeason seas = mLastProcessedSeasonClicked;
            ProcessedEpisode ep = mLastEpClicked;

            if (addSep)
            {
                showRightClickMenu.Items.Add(new ToolStripSeparator());
            }

            if (ep != null)
            {
                if (si != null)
                {
                    AddRcMenuItem("Episode Guide", RightClickCommands.kEpisodeGuideForShow);
                }

                string label = ep.Show.Provider == ShowItem.ProviderType.TVmaze
                    ? "Visit Tv Maze..."
                    : "Visit thetvdb.com";
                AddRcMenuItem(label, RightClickCommands.kVisitTvSourceEpisode);
            }
            else if (seas != null)
            {
                if (si != null)
                {
                    AddRcMenuItem("Episode Guide", RightClickCommands.kEpisodeGuideForShow);
                }
                string label = seas.Show.Provider == ShowItem.ProviderType.TVmaze
                    ? "Visit Tv Maze..."
                    : "Visit thetvdb.com";
                AddRcMenuItem(label, RightClickCommands.kVisitTvSourceSeason);
            }
            else if (si != null)
            {
                AddRcMenuItem("Episode Guide", RightClickCommands.kEpisodeGuideForShow);
                string label = si.Provider == ShowItem.ProviderType.TVmaze
                    ? "Visit Tv Maze..."
                    : "Visit thetvdb.com";
                AddRcMenuItem(label, RightClickCommands.kVisitTvSourceSeries);
            }
            else
            {
                //nothing to add
            }
        }

        private void MenuShowAndEpisodes()
        {
            ShowItem si = mLastShowsClicked != null && mLastShowsClicked.Count > 0
                ? mLastShowsClicked[0]
                : null;

            ProcessedSeason seas = mLastProcessedSeasonClicked;
            ProcessedEpisode ep = mLastEpClicked;

            if (si != null)
            {
                AddRcMenuItem("Force Refresh", RightClickCommands.kForceRefreshSeries);
                AddRcMenuItem("Update Images", RightClickCommands.kUpdateImages);
                showRightClickMenu.Items.Add(new ToolStripSeparator());

                string scanText = mLastShowsClicked.Count > 1
                    ? "Scan Multiple Shows"
                    : "Scan \"" + si.ShowName + "\"";
                AddRcMenuItem(scanText, RightClickCommands.kScanSpecificSeries);

                if (mLastShowsClicked != null && mLastShowsClicked.Count == 1)
                {
                    AddRcMenuItem("When to Watch", RightClickCommands.kWhenToWatchSeries);
                    AddRcMenuItem("Edit Show", RightClickCommands.kEditShow);
                    AddRcMenuItem("Delete Show", RightClickCommands.kDeleteShow);
                }
            }

            if (seas != null && mLastShowsClicked != null && mLastShowsClicked.Count == 1)
            {
                AddRcMenuItem("Edit " + ProcessedSeason.UIFullSeasonWord(seas.SeasonNumber), RightClickCommands.kEditSeason);
                if (si != null)
                {
                    if (si.IgnoreSeasons.Contains(seas.SeasonNumber))
                    {
                        AddRcMenuItem("Include " + ProcessedSeason.UIFullSeasonWord(seas.SeasonNumber),
                            RightClickCommands.kIncludeSeason);
                    }
                    else
                    {
                        AddRcMenuItem("Ignore " + ProcessedSeason.UIFullSeasonWord(seas.SeasonNumber),
                            RightClickCommands.kIgnoreSeason);
                    }
                }
            }

            if (ep != null && mLastShowsClicked != null && mLastShowsClicked.Count == 1)
            {
                List<FileInfo> fl = FinderHelper.FindEpOnDisk(null, ep);
                if (fl.Count <= 0)
                {
                    return;
                }

                showRightClickMenu.Items.Add(new ToolStripSeparator());

                int n = mLastFl.Count;
                foreach (FileInfo fi in fl)
                {
                    mLastFl.Add(fi);
                    ToolStripMenuItem tsi = new ToolStripMenuItem("Watch: " + fi.FullName)
                    {
                        Tag = (int) RightClickCommands.kWatchBase + n
                    };

                    showRightClickMenu.Items.Add(tsi);
                }
            }
            else if (seas != null && si != null && mLastShowsClicked != null && mLastShowsClicked.Count == 1)
            {
                ToolStripMenuItem tsis = new ToolStripMenuItem("Watch Epsiodes");

                // for each episode in season, find it on disk
                if (si.SeasonEpisodes.ContainsKey(seas.SeasonNumber))
                {
                    foreach (ProcessedEpisode epds in si.SeasonEpisodes[seas.SeasonNumber])
                    {
                        List<FileInfo> fl = FinderHelper.FindEpOnDisk(null, epds);
                        if (fl.Count <= 0)
                        {
                            continue;
                        }

                        int n = mLastFl.Count;
                        foreach (FileInfo fi in fl)
                        {
                            mLastFl.Add(fi);
                            ToolStripMenuItem tsisi = new ToolStripMenuItem("Watch: " + fi.FullName)
                            {
                                Tag = (int) RightClickCommands.kWatchBase + n
                            };

                            int n1 = n;
                            tsisi.Click += (s, ev) => { WatchEpisode(n1); };

                            n++;
                            tsis.DropDownItems.Add(tsisi);
                        }
                    }
                }

                if (tsis.DropDownItems.Count > 0)
                {
                    showRightClickMenu.Items.Add(new ToolStripSeparator());
                    showRightClickMenu.Items.Add(tsis);
                }
            }
        }

        private void MenuFolders(LvResults lvr)
        {
            if (mLastShowsClicked is null || mLastShowsClicked.Count != 1)
            {
                return;
            }

            ShowItem si = mLastShowsClicked != null && mLastShowsClicked.Count > 0
                ? mLastShowsClicked[0]
                : null;

            ProcessedSeason seas = mLastProcessedSeasonClicked;
            ProcessedEpisode ep = mLastEpClicked;
            List<string> added = new List<string>();

            if (ep != null)
            {
                Dictionary<int, List<string>> afl = ep.Show.AllExistngFolderLocations();
                if (afl.ContainsKey(ep.AppropriateSeasonNumber))
                {
                    AddFolders(afl[ep.AppropriateSeasonNumber], added);
                }
            }
            else if (seas != null && si != null)
            {
                Dictionary<int, List<string>> folders = si.AllExistngFolderLocations();

                if (folders.ContainsKey(seas.SeasonNumber))
                {
                    AddFolders(folders[seas.SeasonNumber], added);
                }
            }

            if (si != null)
            {
                AddFoldersSubMenu(si.AllExistngFolderLocations().Values.SelectMany(l => l).ToList(), added);
            }

            if (lvr is null)
            {
                return;
            }

            {
                AddFoldersSubMenu(lvr.FlatList.Select(sli =>sli.TargetFolder),added);
            }
        }

        private void AddFolders([NotNull] IEnumerable<string> foldersList,[NotNull] ICollection<string> alreadyAdded)
        {
            int n = mFoldersToOpen.Count;
            bool first = true;
            foreach (string folder in foldersList
                .Where(folder => !string.IsNullOrEmpty(folder))
                .Where(Directory.Exists)
                .Where(folder => !alreadyAdded.Contains(folder)))
            {
                alreadyAdded.Add(folder); // don't show the same folder more than once
                if (first)
                {
                    showRightClickMenu.Items.Add(new ToolStripSeparator());
                    first = false;
                }

                ToolStripMenuItem tsi = new ToolStripMenuItem("Open: " + folder);
                mFoldersToOpen.Add(folder);
                tsi.Tag = (int)RightClickCommands.kOpenFolderBase + n;
                n++;
                showRightClickMenu.Items.Add(tsi);
            }
        }

        private void AddFoldersSubMenu([NotNull] IEnumerable<string> foldersList,[NotNull] ICollection<string> alreadyAdded)
        {
            int n = mFoldersToOpen.Count;

            ToolStripMenuItem tsi = new ToolStripMenuItem("Open Other Folders" );

            foreach (string folder in foldersList.Where(folder => !string.IsNullOrEmpty(folder) && Directory.Exists(folder) && !alreadyAdded.Contains(folder)))
            {
                alreadyAdded.Add(folder); // don't show the same folder more than once
                ToolStripMenuItem tssi = new ToolStripMenuItem("Open: " + folder);
                mFoldersToOpen.Add(folder);
                tssi.Tag = (int)RightClickCommands.kOpenFolderBase + n;
                int n1 = n;
                tssi.Click += (s, ev) => {
                    OpenFolderForShow(n1); 
                };
                n++;
                tsi.DropDownItems.Add(tssi);
            }

            if (tsi.DropDownItems.Count > 0)
            {
                showRightClickMenu.Items.Add(new ToolStripSeparator());
                showRightClickMenu.Items.Add(tsi);
            }
        }

        private void BuildRightClickMenu(Point pt)
        {
            showRightClickMenu.Items.Clear();
            mFoldersToOpen = new List<string>();
            mLastFl = new List<FileInfo>();

            MenuGuideAndTvdb(false);
            MenuShowAndEpisodes();
            MenuFolders(null);

            showRightClickMenu.Show(pt);
        }

        private void showRightClickMenu_ItemClicked(object sender, [NotNull] ToolStripItemClickedEventArgs e)
        {
            showRightClickMenu.Close();

            if (e.ClickedItem.Tag is null)
            {
                mLastEpClicked = null;
                return;
            }

            if (e.ClickedItem.Tag is SearchEngine search)
            {
                SearchFor(search);
                return;
            }

            RightClickCommands n = (RightClickCommands) e.ClickedItem.Tag;

            ShowItem si = mLastShowsClicked != null && mLastShowsClicked.Count > 0
                ? mLastShowsClicked[0]
                : null;

            switch (n)
            {
                case RightClickCommands.kEpisodeGuideForShow: // epguide
                    if (mLastEpClicked != null)
                    {
                        GotoEpguideFor(mLastEpClicked, true);
                    }
                    else
                    {
                        if (si != null)
                        {
                            GotoEpguideFor(si, true);
                        }
                    }

                    break;

                case RightClickCommands.kVisitTvSourceEpisode: // thetvdb.com
                    {
                        TvSourceFor(mLastEpClicked);
                        break;
                    }

                case RightClickCommands.kVisitTvSourceSeason:
                    {
                        TvSourceFor(mLastProcessedSeasonClicked);
                        break;
                    }

                case RightClickCommands.kVisitTvSourceSeries:
                    {
                        TvSourceFor(si);
                        break;
                    }
                case RightClickCommands.kScanSpecificSeries:
                    {
                        if (mLastShowsClicked != null)
                        {
                            UiScan(mLastShowsClicked, false, TVSettings.ScanType.SingleShow);
                            tabControl1.SelectTab(tbAllInOne);
                        }
                        break;
                    }

                case RightClickCommands.kWhenToWatchSeries: // when to watch
                    {
                        int code = -1;
                        if (mLastEpClicked != null)
                        {
                            code = mLastEpClicked.TheSeries.TvdbCode;
                        }

                        if (si != null)
                        {
                            code = si.TvdbCode;
                        }

                        GotoWtwFor(code);

                        break;
                    }
                case RightClickCommands.kForceRefreshSeries:
                    if (si != null)
                    {
                        ForceRefresh(mLastShowsClicked,false);
                    }

                    break;
                case RightClickCommands.kUpdateImages:
                    if (si != null)
                    {
                        UpdateImages(mLastShowsClicked);
                        tabControl1.SelectTab(tbAllInOne);
                    }

                    break;
                case RightClickCommands.kEditShow:
                    if (si != null)
                    {
                        EditShow(si);
                    }

                    break;
                case RightClickCommands.kDeleteShow:
                    if (si != null)
                    {
                        DeleteShow(si);
                    }

                    break;
                case RightClickCommands.kEditSeason:
                    if (si != null)
                    {
                        EditSeason(si, mLastProcessedSeasonClicked.SeasonNumber);
                    }

                    break;
                case RightClickCommands.kBtSearchFor:
                    {
                        foreach (ListViewItem lvi in lvAction.SelectedItems)
                        {
                            ItemMissing m = (ItemMissing) lvi.Tag;
                            if (m != null)
                            {
                                TVDoc.SearchForEpisode(m.Episode);
                            }
                        }
                    }

                    break;
                case RightClickCommands.kActionAction:
                    ActionAction(false,false);
                    break;
                case RightClickCommands.kActionRevert:
                    Revert(false);
                    break;
                case RightClickCommands.kActionBrowseForFile:
                    if (mLastActionsClicked != null && mLastActionsClicked.Count > 0)
                    {
                        BrowseForMissingItem((ItemMissing)mLastActionsClicked[0]);
                        mLastActionsClicked = null;
                    }
                    break;
                case RightClickCommands.kActionIgnore:
                    IgnoreSelected();
                    break;
                case RightClickCommands.kActionIgnoreSeason:
                    {
                        // add season to ignore list for each show selected
                        IgnoreSelectedSeasons();

                        mLastActionsClicked = null;
                        FillActionList(true);
                        break;
                    }
                case RightClickCommands.kActionDelete:
                    ActionDeleteSelected();
                    break;

                case RightClickCommands.kIgnoreSeason:
                    if (si != null)
                    {
                        IgnoreSeason(si, mLastProcessedSeasonClicked.SeasonNumber);
                    }
                    break;

                case RightClickCommands.kIncludeSeason:
                    if (si != null)
                    {
                        IncludeSeason(si, mLastProcessedSeasonClicked.SeasonNumber);
                    }
                    break;

                case RightClickCommands.kWatchBase:
                case RightClickCommands.kOpenFolderBase:
                case RightClickCommands.kSearchForBase:
                {
                    //The entries immediately above WatchBase are the Watchxx commands and the paths are stored in mLastFL
                    if (n >= RightClickCommands.kWatchBase && n < RightClickCommands.kOpenFolderBase)
                    {
                        WatchEpisode(n - RightClickCommands.kWatchBase);
                    }
                    else if (n >= RightClickCommands.kOpenFolderBase && n < RightClickCommands.kSearchForBase)
                    {
                        OpenFolderForShow(n - RightClickCommands.kOpenFolderBase);
                        return;
                    }
                    else
                    {
                        Debug.Fail("Unknown right-click action " + n);
                    }

                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
            mLastEpClicked = null;
        }

        private void IncludeSeason([NotNull] ShowItem si, int seasonNumber)
        {
            si.IgnoreSeasons.Remove(seasonNumber);
            ShowAddedOrEdited(false, false);
        }

        private void IgnoreSeason([NotNull] ShowItem si, int seasonNumber)
        {
            si.IgnoreSeasons.Add(seasonNumber);
            ShowAddedOrEdited(false, false);
        }

        private void BrowseForMissingItem([CanBeNull] ItemMissing mi)
        {
            if (mi is null)
            {
                return;
            }

            // browse for mLastActionClicked
            openFile.Filter = "Video Files|" +
                              TVSettings.Instance.GetVideoExtensionsString()
                                  .Replace(".", "*.") +
                              "|All Files (*.*)|*.*";

            if (openFile.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            ManuallyAddFileForItem(mi, openFile.FileName);
        }

        private void ManuallyAddFileForItem([NotNull] ItemMissing mi, string fileName)
        {
            // make new Item for copying/moving to specified location
            FileInfo from = new FileInfo(fileName);
            FileInfo to = new FileInfo(mi.TheFileNoExt + from.Extension);
            mDoc.TheActionList.Add(
                new ActionCopyMoveRename(
                    TVSettings.Instance.LeaveOriginals
                        ? ActionCopyMoveRename.Op.copy
                        : ActionCopyMoveRename.Op.move, from, to
                    , mi.Episode, true, mi,mDoc));

            // and remove old Missing item
            mDoc.TheActionList.Remove(mi);

            // if we're copying/moving a file across, we might also want to make a thumbnail or NFO for it
            DownloadIdentifiersController di = new DownloadIdentifiersController();
            mDoc.TheActionList.Add(di.ProcessEpisode(mi.Episode, to));

            //If keep together is active then we may want to copy over related files too
            if (TVSettings.Instance.KeepTogether)
            {
                FileFinder.KeepTogether(mDoc.TheActionList, false, true,mDoc);
            }

            FillActionList(true);
        }

        private void OpenFolderForShow(int foldernum)
        {
            if (mFoldersToOpen != null && foldernum >= 0 && foldernum < mFoldersToOpen.Count)
            {
                string folder = mFoldersToOpen[foldernum];

                if (Directory.Exists(folder))
                {
                    Helpers.SysOpen(folder);
                }
            }
        }

        private void SearchFor(SearchEngine s)
        {
            string url = s.Url;

            ProcessedEpisode epi = mLastEpClicked;
            if (epi != null)
            {
                Helpers.SysOpen(CustomEpisodeName.NameForNoExt(epi, url, true));
                return;
            }

            LvResults lvr = new LvResults(lvAction, false);

            foreach (Item i in lvr.FlatList)
            {
                if (i is ItemMissing miss)
                {
                    Helpers.SysOpen(CustomEpisodeName.NameForNoExt(miss.Episode, url, true));
                }
            }
        }

        private void WatchEpisode(int wn)
        {
            if (mLastFl != null && wn >= 0 && wn < mLastFl.Count)
            {
                Helpers.SysOpen(mLastFl[wn].FullName);
            }
        }

        private void IgnoreSelectedSeasons()
        {
            if (mLastActionsClicked != null && mLastActionsClicked.Count > 0)
            {
                foreach (Item ai in mLastActionsClicked)
                {
                    Item er = ai;
                    if (er?.Episode is null)
                    {
                        continue;
                    }

                    int snum = er.Episode.AppropriateSeasonNumber;

                    if (!er.Episode.Show.IgnoreSeasons.Contains(snum))
                    {
                        er.Episode.Show.IgnoreSeasons.Add(snum);
                    }

                    // remove all other episodes of this season from the Action list
                    ItemList remove = new ItemList();
                    foreach (Item action in mDoc.TheActionList)
                    {
                        Item er2 = action;
                        if (er2?.Episode is null)
                        {
                            continue;
                        }

                        if (er2.Episode.AppropriateSeasonNumber != snum)
                        {
                            continue;
                        }

                        if (er2.TargetFolder == er.TargetFolder) //ie if they are for the same series
                        {
                            remove.Add(action);
                        }
                    }

                    foreach (Item action in remove)
                    {
                        mDoc.TheActionList.Remove(action);
                    }

                    if (remove.Count > 0)
                    {
                        mDoc.SetDirty();
                    }
                }

                FillMyShows();
            }
        }

        private void GotoWtwFor(int tvdbSeriesCode)
        {
            if (tvdbSeriesCode == -1)
            {
                return;
            }

            tabControl1.SelectTab(tbWTW);
            foreach (ListViewItem lvi in lvWhenToWatch.Items)
            {
                ProcessedEpisode ei = (ProcessedEpisode)lvi.Tag;
                lvi.Selected = ei != null && ei.TheSeries.TvdbCode == tvdbSeriesCode;
            }
            lvWhenToWatch.Focus();
        }

        private void tabControl1_DoubleClick(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tbMyShows)
            {
                bnMyShowsRefresh_Click(null, null);
            }
            else if (tabControl1.SelectedTab == tbWTW)
            {
                bnWhenToWatchCheck_Click(null, null);
            }
            else if (tabControl1.SelectedTab == tbAllInOne)
            {
                BtnSearch_ButtonClick(null, null);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        private void lvWhenToWatch_MouseClick(object sender, [NotNull] MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            if (lvWhenToWatch.SelectedItems.Count == 0)
            {
                return;
            }

            Point pt = lvWhenToWatch.PointToScreen(new Point(e.X, e.Y));
            List<ProcessedEpisode> eis = new List<ProcessedEpisode>();
            foreach (ListViewItem lvi in lvWhenToWatch.SelectedItems)
            {
                eis.Add(lvi.Tag as ProcessedEpisode);
            }

            WtwRightClickOnShow(eis, pt);
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e) => DoPrefs(false);

        private void DoPrefs(bool scanOptions)
        {
            MoreBusy(); // no background download while preferences are open!
            mDoc.PreventAutoScan("Preferences are open");

            Preferences pref = new Preferences(mDoc, scanOptions,CurrentlySelectedSeason());
            if (pref.ShowDialog() == DialogResult.OK)
            {
                mDoc.SetDirty();
                ShowHideNotificationIcon();
                FillWhenToWatchList();
                ShowInTaskbar = TVSettings.Instance.ShowInTaskbar;
                FillEpGuideHtml();
                mAutoFolderMonitor.SettingsChanged(TVSettings.Instance.MonitorFolders);
                betaToolsToolStripMenuItem.Visible = TVSettings.Instance.IncludeBetaUpdates();
                ForceRefresh(null,false);
            }

            mDoc.AllowAutoScan();
            LessBusy();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                mDoc.WriteXMLSettings();
                SaveCaches(); 
                if (!SaveLayoutXml())
                {
                    Logger.Error("Failed to Save Layout Configuration Files");
                }
            }
            catch (Exception ex)
            {
                Exception e2 = ex;
                while (e2.InnerException != null)
                {
                    e2 = e2.InnerException;
                }

                Logger.Error(e2,"Failed to Save Configuration Files");
                string m2 = e2.Message;
                MessageBox.Show(this,
                    ex.Message + "\r\n\r\n" +
                    m2 + "\r\n\r\n" +
                    ex.StackTrace,
                    "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UI_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                mLastNonMaximizedSize = Size;
            }

            if (WindowState == FormWindowState.Minimized && !TVSettings.Instance.ShowInTaskbar)
            {
                Hide();
            }
        }

        private void UI_LocationChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                mLastNonMaximizedLocation = Location;
            }
        }

        private void statusTimer_Tick(object sender, EventArgs e)
        {
            int n = mDoc.DownloadsRemaining();

            txtDLStatusLabel.Visible = n != 0 || TVSettings.Instance.BGDownload;
            if (n != 0)
            {
                txtDLStatusLabel.Text = "Background download: " + TheTVDB.LocalCache.Instance.CurrentDLTask +TVmaze.LocalCache.Instance.CurrentDLTask;
                backgroundDownloadNowToolStripMenuItem.Enabled = false;
            }
            else
            {
                txtDLStatusLabel.Text = "Background download: Idle";
                backgroundDownloadNowToolStripMenuItem.Enabled = true;
            }

            if (IsBusy)
            {
                return;
            }

            if (n == 0 && lastDlRemaining > 0)
            {
                // we've just finished a bunch of background downloads
                SaveCaches();
                RefreshWTW(false,true);

                backgroundDownloadNowToolStripMenuItem.Enabled = true;
            }

            lastDlRemaining = n;
        }

        private static void SaveCaches()
        {
            TheTVDB.LocalCache.Instance.SaveCache();
            TVmaze.LocalCache.Instance.SaveCache();
        }

        private void backgroundDownloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TVSettings.Instance.BGDownload = !TVSettings.Instance.BGDownload;
            backgroundDownloadToolStripMenuItem.Checked = TVSettings.Instance.BGDownload;
            statusTimer_Tick(null, null);
            mDoc.SetDirty();

            if (TVSettings.Instance.BGDownload)
            {
                BGDownloadTimer.Start();
            }
            else
            {
                BGDownloadTimer.Stop();
            }
        }

        private async void UpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdateTimer.Stop();

            Dispatcher uiDisp = Dispatcher.CurrentDispatcher;

            Task<Release> tuv = VersionUpdater.CheckForUpdatesAsync();
            Release result = await tuv.ConfigureAwait(false);

            uiDisp.Invoke(() => NotifyUpdates(result, false,mDoc.Args.Unattended ||mDoc.Args.Hide));
        }
        
        private void BGDownloadTimer_Tick(object sender, EventArgs e)
        {
            if (IsBusy)
            {
                BGDownloadTimer.Interval = 10000; // come back in 10 seconds
                BGDownloadTimer.Start();
                Logger.Info("BG Download is busy - try again in 10 seconds");
                return;
            }

            // after first time (10 seconds), put up to user defined period (by default 60 minutes)
            BGDownloadTimer.Interval = TVSettings.Instance.PeriodicUpdateCachePeriod();
            
            BGDownloadTimer.Start();

            if (TVSettings.Instance.BGDownload && mDoc.DownloadsRemaining() == 0)
                // only do auto-download if don't have stuff to do already
            {
                BackgroundDownloadNow();
            }
        }

        private void BGDownloadTimer_QuickFire()
        {
            BGDownloadTimer.Stop();
            BGDownloadTimer.Interval = 1000;
            BGDownloadTimer.Start();
        }

        private void backgroundDownloadNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TVSettings.Instance.OfflineMode)
            {
                DialogResult res = MessageBox.Show("Ignore offline mode and download anyway?",
                    "Background Download", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (res != DialogResult.Yes)
                {
                    return;
                }
            }

            BackgroundDownloadNow();
        }

        private void BackgroundDownloadNow()
        {
            BGDownloadTimer.Stop();
            BGDownloadTimer.Start();

            mDoc.DoDownloadsBG();

            statusTimer_Tick(null, null);
        }

        private void offlineOperationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!TVSettings.Instance.OfflineMode)
            {
                if (MessageBox.Show("Are you sure you wish to go offline?", "TVRename", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning) == DialogResult.No)
                {
                    return;
                }
            }

            TVSettings.Instance.OfflineMode = !TVSettings.Instance.OfflineMode;
            offlineOperationToolStripMenuItem.Checked = TVSettings.Instance.OfflineMode;
            mDoc.SetDirty();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tbMyShows)
            {
                if (mLastEpClicked != null && TVSettings.Instance.AutoSelectShowInMyShows)
                {
                    GotoEpguideFor(mLastEpClicked, false);
                }
                FillEpGuideHtml();
            }
            else if (tabControl1.SelectedTab == tbWTW)
            {
                mLastEpClicked = mLastEpClickedWtw; 
            }
            else if (tabControl1.SelectedTab == tbAllInOne)
            {
                mLastEpClicked = mLastEpClickedScan;
            }
        }

        private void bugReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BugReport br = new BugReport(mDoc);
            br.ShowDialog();
        }

        private void ShowHideNotificationIcon()
        {
            notifyIcon1.Visible = TVSettings.Instance.NotificationAreaIcon && !mDoc.Args.Hide;
        }

        private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StatsWindow sw = new StatsWindow(mDoc.Stats());
            sw.ShowDialog();
        }

        [NotNull]
        private TreeNode AddShowItemToTree([NotNull] ShowItem si)
        {
            SeriesInfo ser = si.TheSeries();

            TreeNode n = new TreeNode(GenerateShowUIName(ser,si)) {Tag = si};

            if (ser != null)
            {
                if (TVSettings.Instance.ShowStatusColors != null)
                {
                    if (TVSettings.Instance.ShowStatusColors.IsShowStatusDefined(si.ShowStatus))
                    {
                        n.ForeColor = TVSettings.Instance.ShowStatusColors.GetEntry(false, true, si.ShowStatus);
                    }
                    else
                    {
                        Color nodeColor =
                            TVSettings.Instance.ShowStatusColors.GetEntry(true, true, si.SeasonsAirStatus.ToString());

                        if (!nodeColor.IsEmpty)
                        {
                            n.ForeColor = nodeColor;
                        }
                    }
                }

                List<int> theKeys = si.AppropriateSeasons().Keys.ToList();

                theKeys.Sort();

                SeasonFilter sf = TVSettings.Instance.SeasonFilter;
                foreach (int snum in theKeys)
                {
                    ProcessedSeason s = si.AppropriateSeasons()[snum];

                    //Ignore the season if it is filtered out
                    if (!sf.Filter(si, s))
                    {
                        continue;
                    }

                    if (snum == 0 && TVSettings.Instance.IgnoreAllSpecials)
                    {
                        continue;
                    }

                    string nodeTitle = ShowHtmlHelper.SeasonName(si, snum);

                    TreeNode n2 = new TreeNode(nodeTitle);
                    if (si.IgnoreSeasons.Contains(snum))
                    {
                        n2.ForeColor = Color.Gray;
                    }
                    else
                    {
                        if (TVSettings.Instance.ShowStatusColors != null)
                        {
                            Color nodeColor =
                                TVSettings.Instance.ShowStatusColors.GetEntry(true, false,
                                    s.Status(si.GetTimeZone()).ToString());

                            if (!nodeColor.IsEmpty)
                            {
                                n2.ForeColor = nodeColor;
                            }
                        }
                    }

                    n2.Tag = s;
                    n.Nodes.Add(n2);
                }
            }

            MyShowTree.Nodes.Add(n);

            return n;
        }

        // ReSharper disable once InconsistentNaming
        public static string GenerateShowUIName([CanBeNull] ProcessedEpisode episode) => GenerateShowUIName(episode?.TheSeries, episode?.Show);

        // ReSharper disable once InconsistentNaming
        private static string GenerateShowUIName([NotNull] ShowItem si)
        {
            SeriesInfo s = si.TheSeries();
            return GenerateShowUIName(s,si);
        }

        // ReSharper disable once InconsistentNaming
        private static string GenerateShowUIName([CanBeNull] SeriesInfo ser, [CanBeNull] ShowItem si)
        {
            string name = GenerateBestName(ser, si);

            return PostpendTheIfNeeded(name);
        }

        public static string PostpendTheIfNeeded(string name)
        {
            if (TVSettings.Instance.PostpendThe && name.StartsWith("The ", StringComparison.Ordinal))
            {
                return name.RemoveFirst(4) + ", The";
            }

            return name;
        }

        private static string GenerateBestName([CanBeNull] SeriesInfo ser, [CanBeNull] ShowItem si)
        {
            string name = si?.ShowName;

            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }

            if (ser != null)
            {
                return ser.Name;
            }

            return "-- Unknown : " + si?.TvdbCode + " --";
        }

        private void UpdateWtw(DirFilesCache dfc, [NotNull] ProcessedEpisode pe, [NotNull] ListViewItem lvi)
        {
            DateTime? airdt = pe.GetAirDateDt(true);
            if (airdt is null)
            {
                return;
            }
            DateTime dt = airdt.Value;

            lvi.Group = lvWhenToWatch.Groups[CalculateWtwlviGroup(pe, dt)];
            lvi.Tag = pe;
            lvi.Text = GenerateShowUIName(pe);

            int n = 0;
            lvi.SubItems[++n].Text = pe.SeasonNumberAsText;
            lvi.SubItems[++n].Text = GetEpisodeNumber(pe);
            lvi.SubItems[++n].Text = dt.ToShortDateString();
            lvi.SubItems[++n].Text = dt.ToString("t");
            lvi.SubItems[++n].Text = dt.ToString("ddd");
            lvi.SubItems[++n].Text = pe.HowLong();
            lvi.SubItems[++n].Text = pe.TheSeries.Network;
            lvi.SubItems[++n].Text = pe.Name;

            // icon..
            int? iconNumbers = ChooseWtwIcon(dfc, pe, dt);
            if (iconNumbers != null)
            {
                lvi.ImageIndex = iconNumbers.Value;
            }

            if (TVSettings.Instance.UseColoursOnWtw)
            {
                (Color back,Color fore) = GetWtwColour(pe,dt);
                lvi.BackColor = back;
                lvi.ForeColor = fore;
            }
        }

        private static (Color, Color) GetWtwColour([NotNull] ProcessedEpisode ep, DateTime dt)
        {
            //any episodes that match "today's" date be highlighted in light gray so they stand out.
            //Or, really fancy: any future dates are light gray background,
            //past dates are light blue
            //and "today" is yellow..

            if (dt.Date == DateTime.Today)
            {
                return (Color.LightYellow, Color.Black);
            }

            if (ep.IsInFuture(true))
            {
                return (Color.LightGray,Color.Black);
            }

            return (Color.LightBlue, Color.Black);
        }

        [NotNull]
        private static string GetEpisodeNumber([NotNull] ProcessedEpisode pe)
        {
            if (pe.AppropriateEpNum > 0 && pe.EpNum2 != pe.AppropriateEpNum && pe.EpNum2 > 0)
            {
                return pe.AppropriateEpNum + "-" + pe.EpNum2;
            }

            return pe.AppropriateEpNum > 0 ? pe.AppropriateEpNum.ToString() : string.Empty;
        }

        [NotNull]
        private static string CalculateWtwlviGroup(ProcessedEpisode pe, DateTime dt)
        {
            double ttn = dt.Subtract(DateTime.Now).TotalHours;

            if (ttn < 0)
            {
                return "justPassed";
            }
            if (ttn < 7 * 24)
            {
                return "next7days";
            }
            return !pe.NextToAir ? "later" : "futureEps";
        }

        private static int? ChooseWtwIcon(DirFilesCache dfc, [NotNull] ProcessedEpisode pe, DateTime airdt)
        {
            List<FileInfo> fl = dfc.FindEpOnDisk(pe);
            bool appropriateFileNameFound = !TVSettings.Instance.RenameCheck
                                            || !pe.Show.DoRename
                                            || fl.All(file => file.Name.StartsWith( TVSettings.Instance.FilenameFriendly(TVSettings.Instance.NamingStyle.NameFor(pe)), StringComparison.OrdinalIgnoreCase));

            if (fl.Count > 0 && appropriateFileNameFound)
            {
                return 0; //Disk
            }

            if (airdt.CompareTo(DateTime.Now) < 0) // has aired
            {
                if (TVSettings.Instance.IgnorePreviouslySeen && pe.PreviouslySeen)
                {
                    return 9; //tick
                }

                if (pe.Show.DoMissingCheck)
                {
                    return 1; //Search
                }
            }

            return null;
        }

        private void SelectSeason(ProcessedSeason seas)
        {
            foreach (TreeNode n in MyShowTree.Nodes)
            {
                if (NodeIsForShow(seas.Show, n))
                {
                    foreach (TreeNode n2 in n.Nodes)
                    {
                        if (TreeNodeToSeason(n2)?.SeasonId == seas.SeasonId)
                        {
                            n2.EnsureVisible();
                            MyShowTree.SelectedNode = n2;
                            return;
                        }
                    }
                }
            }

            FillEpGuideHtml(null);
        }

        private static bool NodeIsForShow([NotNull] ShowItem si, TreeNode n)
        {
            switch (si.Provider)
            {
                case ShowItem.ProviderType.TVmaze:
                    return TreeNodeToShowItem(n)?.TVmazeCode == si.TVmazeCode;
                case ShowItem.ProviderType.TheTVDB:
                    return TreeNodeToShowItem(n)?.TvdbCode == si.TvdbCode;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SelectShow(ShowItem si)
        {
            foreach (TreeNode n in MyShowTree.Nodes)
            {
                if (TreeNodeToShowItem(n) == si)
                {
                    n.EnsureVisible();
                    MyShowTree.SelectedNode = n;
                    //FillEpGuideHTML();
                    return;
                }
            }

            FillEpGuideHtml(null);
        }

        private void bnMyShowsAdd_Click(object sender, EventArgs e)
        {
            Logger.Info("****************");
            Logger.Info("Adding New Show");
            MoreBusy();
            mDoc.PreventAutoScan("Add Show");
            ShowItem si = new ShowItem();

            AddEditShow aes = new AddEditShow(si);
            DialogResult dr = aes.ShowDialog();
            if (dr == DialogResult.OK)
            {
                mDoc.Library.Add(si);

                ShowAddedOrEdited(false,false);
                SelectShow(si);

                Logger.Info("Added new show called {0}", si.ShowName);
            }
            else
            {
                Logger.Info("Cancelled adding new show");
            }

            ShowAddedOrEdited(true,false);

            LessBusy();
            mDoc.AllowAutoScan();
        }

        private void ShowAddedOrEdited(bool download,bool unattended)
        {
            mDoc.SetDirty();
            RefreshWTW(download,unattended);
            mDoc.ReindexLibrary();
            FillMyShows();

            mDoc.ExportShowInfo(); //Save shows list to disk
        }

        private void bnMyShowsDelete_Click(object sender, EventArgs e)
        {
            TreeNode n = MyShowTree.SelectedNode;
            ShowItem si = TreeNodeToShowItem(n);
            if (si is null)
            {
                return;
            }

            DeleteShow(si);
        }

        private void DeleteShow([NotNull] ShowItem si)
        {
            DialogResult res = MessageBox.Show(
                "Remove show \"" + si.ShowName + "\".  Are you sure?", "Confirmation", MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (res != DialogResult.Yes)
            {
                return;
            }

            if (Directory.Exists(si.AutoAddFolderBase) && TVSettings.Instance.DeleteShowFromDisk)
            {
                DialogResult res3 = MessageBox.Show(
                    $"Remove folder \"{si.AutoAddFolderBase}\" from disk?",
                    "Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (res3 == DialogResult.Yes)
                {
                    Logger.Info($"Recycling {si.AutoAddFolderBase} as part of the removal of {si.ShowName}");
                    Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(si.AutoAddFolderBase,
                        Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                        Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                }
            }

            Logger.Info($"User asked to remove {si.ShowName} - removing now");
            mDoc.Library.Remove(si);
            ShowAddedOrEdited(false,false);
        }

        private void bnMyShowsEdit_Click(object sender, EventArgs e)
        {
            TreeNode n = MyShowTree.SelectedNode;
            if (n is null)
            {
                return;
            }

            ProcessedSeason seas = TreeNodeToSeason(n);
            if (seas != null)
            {
                ShowItem si = TreeNodeToShowItem(n);
                if (si != null)
                {
                    EditSeason(si, seas.SeasonNumber);
                }

                return;
            }

            ShowItem si2 = TreeNodeToShowItem(n);
            if (si2 != null)
            {
                EditShow(si2);
            }
        }

        internal void EditSeason([NotNull] ShowItem si, int seasnum)
        {
            MoreBusy();
            mDoc.PreventAutoScan("Edit Season");

            EditSeason er = new EditSeason(si, seasnum, TVSettings.Instance.NamingStyle);
            DialogResult dr = er.ShowDialog();
            if (dr == DialogResult.OK)
            {
                ShowAddedOrEdited(false, false);
                SelectSeason(si.AppropriateSeasons()[seasnum]);
            }

            mDoc.AllowAutoScan();
            LessBusy();
        }

        internal void EditShow([NotNull] ShowItem si)
        {
            MoreBusy();
            mDoc.PreventAutoScan("Edit Show");

            int oldCode = si.TvdbCode;

            AddEditShow aes = new AddEditShow(si);

            DialogResult dr = aes.ShowDialog();

            if (dr == DialogResult.OK)
            {
                ShowAddedOrEdited(si.TvdbCode != oldCode,false);
                SelectShow(si);

                Logger.Info("Modified show called {0}", si.ShowName);
            }

            mDoc.AllowAutoScan();
            LessBusy();
        }

        internal void ForceRefresh(IEnumerable<ShowItem> sis, bool unattended)
        {
            mDoc.ForceRefresh(sis,unattended,WindowState==FormWindowState.Minimized);
            FillMyShows();
            FillEpGuideHtml();
            RefreshWTW(false, unattended);
        }

        private void UpdateImages([CanBeNull] IReadOnlyCollection<ShowItem> sis)
        {
            if (sis == null)
            {
                return;
            }
            //ForceRefresh(sis,false);

            foreach (ShowItem si in sis)
            {
                //update images for the showitem
                mDoc.ForceUpdateImages(si);
            }

            FillActionList(false);
        }

        private void bnMyShowsRefresh_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                // nuke currently selected show to force getting it fresh
                TreeNode n = MyShowTree.SelectedNode;
                ShowItem si = TreeNodeToShowItem(n);
                ForceRefresh(new List<ShowItem> {si},false);
            }
            else
            {
                ForceRefresh(null,false);
            }
        }

        private void MyShowTree_AfterSelect(object sender, [NotNull] TreeViewEventArgs e)
        {
            FillEpGuideHtml(e.Node);
            bool showSelected = MyShowTree.SelectedNode != null;
            btnEditShow.Enabled = showSelected;
            btnRemoveShow.Enabled = showSelected;
        }

        private void MyShowTree_MouseClick(object sender, [NotNull] MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            MyShowTree.SelectedNode = MyShowTree.GetNodeAt(e.X, e.Y);

            Point pt = MyShowTree.PointToScreen(new Point(e.X, e.Y));
            TreeNode n = MyShowTree.SelectedNode;

            if (n is null)
            {
                return;
            }

            ShowItem si = TreeNodeToShowItem(n);
            ProcessedSeason seas = TreeNodeToSeason(n);

            if (seas != null)
            {
                RightClickOnMyShows(seas, pt);
            }
            else if (si != null)
            {
                RightClickOnMyShows(si, pt);
            }
        }

        private void quickstartGuideToolStripMenuItem_Click(object sender, EventArgs e) => ShowQuickStartGuide();

        private ProcessedSeason CurrentlySelectedSeason()
        {
            ProcessedSeason currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
            if (currentSeas != null)
            {
                return currentSeas;
            }

            ShowItem currentShow = TreeNodeToShowItem(MyShowTree.SelectedNode);
            if (currentShow != null)
            {
                foreach (KeyValuePair<int, ProcessedSeason> s in currentShow.AppropriateSeasons())
                {
                    //Find first season we can
                    return s.Value;
                }
            }

            foreach (ShowItem si in mDoc.Library.GetSortedShowItems())
            {
                foreach (KeyValuePair<int, ProcessedSeason> s in si.AppropriateSeasons())
                {
                    //Find first season we can
                    return s.Value;
                }
            }
            return null;
        }

        [CanBeNull]
        private List<ProcessedEpisode> CurrentlySelectedPel()
        {
            ProcessedSeason currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
            ShowItem currentShow = TreeNodeToShowItem(MyShowTree.SelectedNode);

            int snum = currentSeas?.SeasonNumber ?? 1;
            List<ProcessedEpisode> pel = null;
            if (currentShow != null && currentShow.SeasonEpisodes.ContainsKey(snum))
            {
                pel = currentShow.SeasonEpisodes[snum];
            }
            else if (currentShow?.SeasonEpisodes.First() != null)
            {
                pel = currentShow.SeasonEpisodes.First().Value;
            }
            else
            {
                foreach (ShowItem si in mDoc.Library.GetSortedShowItems())
                {
                    foreach (KeyValuePair<int, List<ProcessedEpisode>> kvp in si.SeasonEpisodes)
                    {
                        pel = kvp.Value;
                        break;
                    }

                    if (pel != null)
                    {
                        break;
                    }
                }
            }

            return pel;
        }

        private void filenameTemplateEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomEpisodeName cn = new CustomEpisodeName(TVSettings.Instance.NamingStyle.StyleString);
            CustomNameDesigner cne = new CustomNameDesigner(CurrentlySelectedPel(), cn);
            DialogResult dr = cne.ShowDialog();
            if (dr == DialogResult.OK)
            {
                TVSettings.Instance.NamingStyle = cn;
                mDoc.SetDirty();
            }
        }

        private void searchEnginesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<ProcessedEpisode> pel = CurrentlySelectedPel();

            AddEditSearchEngine aese = new AddEditSearchEngine(TVDoc.GetSearchers(),
                pel != null && pel.Count > 0 ? pel[0] : null);

            DialogResult dr = aese.ShowDialog();
            if (dr == DialogResult.OK)
            {
                mDoc.SetDirty();
                UpdateSearchButtons();
            }
        }

        private void filenameProcessorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowItem currentShow = TreeNodeToShowItem(MyShowTree.SelectedNode);
            string theFolder = GetFolderForShow(currentShow);

            if (string.IsNullOrWhiteSpace(theFolder) && TVSettings.Instance.DownloadFolders.Count>0)
            {
                theFolder = TVSettings.Instance.DownloadFolders.First();
            }

            AddEditSeasEpFinders d = new AddEditSeasEpFinders(TVSettings.Instance.FNPRegexs, mDoc.Library.GetSortedShowItems(), currentShow, theFolder);

            DialogResult dr = d.ShowDialog();
            if (dr == DialogResult.OK)
            {
                mDoc.SetDirty();
                TVSettings.Instance.FNPRegexs = d.OutputRegularExpressions;
            }
        }

        [NotNull]
        private static string GetFolderForShow([CanBeNull] ShowItem currentShow)
        {
            if (currentShow is null)
            {
                return string.Empty;
            }

            foreach (List<string> folders in currentShow.AllExistngFolderLocations().Values)
            {
                foreach (string folder in folders)
                {
                    if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
                    {
                        return folder;
                    }
                }
            }

            return string.Empty;
        }

        private void actorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ActorsGrid(mDoc).ShowDialog();
        }

        private void quickTimer_Tick(object sender, EventArgs e)
        {
            quickTimer.Stop();
            ProcessArgs(mDoc.Args);
        }

        private void UI_KeyDown(object sender, [NotNull] KeyEventArgs e)
        {
            if (!e.Control)
            {
                return;
            }

            int t = GetIndex(e.KeyCode);

            if (t >= 0 && t < tabControl1.TabCount)
            {
                tabControl1.SelectedIndex = t;
                e.Handled = true;
            }
        }

        private static int GetIndex(Keys e)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (e)
            {
                case Keys.D1:
                    return 0;
                case Keys.D2:
                    return 1;
                case Keys.D3:
                    return 2;
                case Keys.D4:
                    return 3;
                case Keys.D5:
                    return 4;
                case Keys.D6:
                    return 5;
                case Keys.D7:
                    return 6;
                case Keys.D8:
                    return 7;
                case Keys.D9:
                    return 8;
                case Keys.D0:
                    return 9;
                default:
                    return -1;
            }
        }

        private void UiScan([CanBeNull] List<ShowItem> shows, bool unattended, TVSettings.ScanType st)
        {
            Logger.Info("*******************************");
            string desc = unattended ? "unattended " : "";
            string showsdesc = shows?.Count > 0 ? shows.Count.ToString() : "all";
            string scantype = st.PrettyPrint();
            Logger.Info($"Starting {desc}{scantype} Scan for {showsdesc} shows...");

            MoreBusy();
            mDoc.Scan(shows, unattended, st,WindowState==FormWindowState.Minimized);
            LessBusy();

            if (mDoc.ShowProblems.Any() && !unattended)
            {
                string message = mDoc.ShowProblems.Count>1
                    ? $"Shows with Id { string.Join(",",mDoc.ShowProblems.Select(exception => exception.ShowId))} are not found on TVDB and TVMaze. Please update them"
                    : $"Show with {StringFor(mDoc.ShowProblems.First().ShowIdProvider)} Id {mDoc.ShowProblems.First().ShowId} is not found on {StringFor(mDoc.ShowProblems.First().ErrorProvider)}. Please Update";

                DialogResult result = MessageBox.Show(message,"Series No Longer Found", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                if (result != DialogResult.Cancel)
                {
                    foreach (ShowNotFoundException problem in mDoc.ShowProblems)
                    {
                        if (mDoc.ShowProblems.Count > 1)
                        {
                            MessageBox.Show(problem.Message,"Issue With Series Setup",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        }

                        ShowItem problemShow = mDoc.Library.GetShowItem(problem.ShowId);
                        if (problemShow != null)
                        {
                            EditShow(problemShow);
                        }
                    }
                }

                mDoc.ClearShowProblems();
            }

            FillMyShows(); // scanning can download more info to be displayed in my shows
            FillActionList(false);
        }

        [NotNull]
        private string StringFor(ShowItem.ProviderType i)
        {
            switch (i)
            {
                case ShowItem.ProviderType.TVmaze:
                    return "TV Maze";

                case ShowItem.ProviderType.TheTVDB:
                    return "The TVDB";

                case ShowItem.ProviderType.libraryDefault:
                default:
                    throw new ArgumentOutOfRangeException(nameof(i), i, null);
            }
        }

        [NotNull]
        private ListViewItem LviForItem(Item item)
        {
            Item sli = item;
            if (sli is null)
            {
                return new ListViewItem();
            }

            ListViewItem lvi = sli.ScanListViewItem;
            lvi.Group = lvAction.Groups[sli.ScanListViewGroup];

            if (sli.IconNumber != -1)
            {
                lvi.ImageIndex = sli.IconNumber;
            }

            lvi.Checked = true;
            lvi.Tag = sli;

            Debug.Assert(lvi.SubItems.Count <= lvAction.Columns.Count - 1);

            while (lvi.SubItems.Count < lvAction.Columns.Count - 1)
            {
                lvi.SubItems.Add(""); // pad our way to the error column
            }

            if (item is Action act && act.Outcome.Error)
            {
                lvi.BackColor = Helpers.WarningColor();
            }

            lvi.SubItems.Add(item.ErrorText); // error text

            if (!(item is Action))
            {
                lvi.Checked = false;
            }

            Debug.Assert(lvi.SubItems.Count == lvAction.Columns.Count);

            return lvi;
        }

        private void lvAction_RetrieveVirtualItem(object sender, [NotNull] RetrieveVirtualItemEventArgs e)
        {
            Item item = mDoc.TheActionList[e.ItemIndex];
            e.Item = LviForItem(item);
        }

        public void FillActionList(bool preserveExistingCheckboxes)
        {
            internalCheckChange = true;

            // Save where the list is currently scrolled too
            int currentTop = lvAction.GetScrollVerticalPos();

            List<Item> selectedItems = new List<Item>();
            List<Item> deSelectedItems = new List<Item>();
            if (preserveExistingCheckboxes)
            {
                //get checkboxes
                foreach (object index in lvAction.CheckedItems)
                {
                    selectedItems.Add((Item) ((ListViewItem) index).Tag);
                }

                foreach (object index in lvAction.Items)
                {
                    Item chosenItem = (Item)((ListViewItem)index).Tag;
                    if (!selectedItems.Contains(chosenItem))
                    {
                        deSelectedItems.Add(chosenItem);
                    }
                }
            }

            if (lvAction.VirtualMode)
            {
                lvAction.VirtualListSize = mDoc.TheActionList.Count;
            }
            else
            {
                lvAction.BeginUpdate();
                lvAction.Items.Clear();

                foreach (Item item in mDoc.TheActionList.ToList())
                {
                    ListViewItem lvi = LviForItem(item);

                    if (preserveExistingCheckboxes)
                    {
                        if (selectedItems.Contains(item))
                        {
                            lvi.Checked = true;
                        }
                        else if (deSelectedItems.Contains(item))
                        {
                            lvi.Checked = false;
                        }
                        else
                        {
                            //must be a newly added item, so leave with default checked status
                        }
                    }

                    lvAction.Items.Add(lvi);
                }

                lvAction.EndUpdate();
            }

            // Restore the scrolled to position
            lvAction.SetScrollVerticalPos(currentTop);

            // do nice totals for each group
            int missingCount = 0;
            int renameCount = 0;
            int copyCount = 0;
            long copySize = 0;
            int moveCount = 0;
            int removeCount = 0;
            long moveSize = 0;
            int rssCount = 0;
            int downloadCount = 0;
            int metaCount = 0;
            int dlCount = 0;
            int fileMetaCount = 0;

            foreach (Item action in mDoc.TheActionList)
            {
                if (action is ItemMissing)
                {
                    missingCount++;
                }
                else if (action is ActionCopyMoveRename cmr)
                {
                    ActionCopyMoveRename.Op op = cmr.Operation;
                    if (op == ActionCopyMoveRename.Op.copy)
                    {
                        copyCount++;
                        if (cmr.From.Exists)
                        {
                            copySize += cmr.From.Length;
                        }
                    }
                    else if (op == ActionCopyMoveRename.Op.move)
                    {
                        moveCount++;
                        if (cmr.From.Exists)
                        {
                            moveSize += cmr.From.Length;
                        }
                    }
                    else if (op == ActionCopyMoveRename.Op.rename)
                    {
                        renameCount++;
                    }
                }
                else if (action is ActionDownloadImage)
                {
                    downloadCount++;
                }
                else if (action is ActionTDownload)
                {
                    rssCount++;
                }
                else if (action is ActionWriteMetadata) // base interface that all metadata actions are derived from
                {
                    metaCount++;
                }
                else if (action is ActionDateTouch)
                {
                    fileMetaCount++;
                }
                else if (action is ItemDownloading)
                {
                    dlCount++;
                }
                else if (action is ActionDeleteFile || action is ActionDeleteDirectory)
                {
                    removeCount++;
                }
            }

            lvAction.Groups[0].Header = HeaderName("Missing", missingCount);
            lvAction.Groups[1].Header = HeaderName("Rename", renameCount);
            lvAction.Groups[2].Header = HeaderName("Copy", copyCount, copySize);
            lvAction.Groups[3].Header = HeaderName("Move", moveCount, moveSize);
            lvAction.Groups[4].Header = HeaderName("Remove", removeCount);
            lvAction.Groups[5].Header = HeaderName("Download RSS", rssCount);
            lvAction.Groups[6].Header = HeaderName("Download", downloadCount);
            lvAction.Groups[7].Header = HeaderName("Media Center Metadata", metaCount);
            lvAction.Groups[8].Header = HeaderName("Update File/Directory Metadata", fileMetaCount);
            lvAction.Groups[9].Header = HeaderName("Downloading", dlCount);

            internalCheckChange = false;

            UpdateActionCheckboxes();
        }

        [NotNull]
        private static string HeaderName(string name, int number) => $"{name} ({PrettyPrint(number)})";

        [NotNull]
        private static string PrettyPrint(int number) => number + " " + number.ItemItems();

        [NotNull]
        private static string HeaderName(string name, int number, long filesize) => $"{name} ({PrettyPrint(number)}, {filesize.GBMB(1)})";

        private void bnActionAction_Click(object sender, EventArgs e) => ActionAction(true,false);

        private void ActionAction(bool checkedNotSelected, bool unattended)
        {
            mDoc.PreventAutoScan("Action Selected Items");
            ItemList lvr = new LvResults(lvAction, checkedNotSelected).FlatList;
            foreach (Item i in lvr)
            {
                if (i is Action a)
                {
                    a.ResetOutcome();
                }
            }
            mDoc.DoActions(lvr);
            // remove items from master list, unless it had an error
            foreach (Item i2 in new LvResults(lvAction, checkedNotSelected).FlatList)
            {
                if (i2 != null && !lvr.Contains(i2))
                {
                    mDoc.TheActionList.Remove(i2);
                }
            }

            FillActionList(true);
            RefreshWTW(false,unattended);
            mDoc.AllowAutoScan();
        }

        private void Revert(bool checkedNotSelected)
        {
            foreach (Item item in new LvResults(lvAction, checkedNotSelected).FlatList)
            {
                Action revertAction = (Action) item;
                ItemMissing m2 = revertAction.UndoItemMissing;

                if (m2 is null)
                {
                    continue;
                }

                mDoc.TheActionList.Add(m2);
                mDoc.TheActionList.Remove(revertAction);
                
                //We can remove any CopyMoveActions that are closely related too
                if (!(revertAction is ActionCopyMoveRename))
                {
                    continue;
                }

                ActionCopyMoveRename i2 = (ActionCopyMoveRename)item;
                List<Item> toRemove = new List<Item>();

                foreach (Item a in mDoc.TheActionList)
                {
                    switch (a)
                    {
                        case ItemMissing _:
                            continue;

                        case ActionCopyMoveRename i1:
                        {
                            if (i1.From.RemoveExtension(true).StartsWith(i2.From.RemoveExtension(true), StringComparison.Ordinal))
                            {
                                toRemove.Add(i1);
                            }

                            break;
                        }

                        case Item ad:
                        {
                            if (ad.Episode?.AppropriateEpNum == i2.Episode?.AppropriateEpNum &&
                                ad.Episode?.AppropriateSeasonNumber == i2.Episode?.AppropriateSeasonNumber)
                            {
                                toRemove.Add(a);
                            }

                            break;
                        }
                    }
                }

                //Remove all similar items
                mDoc.TheActionList.Remove(toRemove);
            }

            FillActionList(true);
            RefreshWTW(false,false);
        }

        private void folderMonitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BulkAddManager bam = new BulkAddManager(mDoc);
            FolderMonitor fm = new FolderMonitor(mDoc, bam);
            fm.ShowDialog();
            FillMyShows();
        }

        private void lvAction_MouseClick(object sender, [NotNull] MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            // build the right click menu for the _selected_ items, and types of items
            LvResults lvr = new LvResults(lvAction, false);

            if (lvr.Count == 0)
            {
                return; // nothing selected
            }

            Point pt = lvAction.PointToScreen(new Point(e.X, e.Y));

            showRightClickMenu.Items.Clear();

            // Action related items
            if (lvr.Count > lvr.Missing.Count) // not just missing selected
            {
                AddRcMenuItem("Action Selected", RightClickCommands.kActionAction);
            }

            AddRcMenuItem("Ignore Selected", RightClickCommands.kActionIgnore);
            AddRcMenuItem("Ignore Entire Season", RightClickCommands.kActionIgnoreSeason);
            AddRcMenuItem("Remove Selected",RightClickCommands.kActionDelete);

            if (lvr.Count == lvr.Missing.Count) // only missing items selected?
            {
                showRightClickMenu.Items.Add(new ToolStripSeparator());

                ToolStripMenuItem tsi = new ToolStripMenuItem("Search") { Tag = (int)RightClickCommands.kBtSearchFor };

                foreach (SearchEngine se in TVDoc.GetSearchers())
                {
                    if (se.Name.HasValue())
                    {
                        ToolStripMenuItem tssi = new ToolStripMenuItem(se.Name){ Tag = se};
                        tssi.Click += (s, ev) => { SearchFor(se); };
                        tsi.DropDownItems.Add(tssi);
                    }
                }
                showRightClickMenu.Items.Add(tsi);

                if (lvr.Count == 1) // only one selected
                {
                    AddRcMenuItem("Browse For...", RightClickCommands.kActionBrowseForFile);
                }
            }

            if (lvr.CopyMove.Count > 0||lvr.DownloadTorrents.Count>0)
            {
                showRightClickMenu.Items.Add(new ToolStripSeparator());
                AddRcMenuItem("Revert to Missing", RightClickCommands.kActionRevert);
            }

            MenuGuideAndTvdb(true);
            MenuFolders(lvr);

            showRightClickMenu.Show(pt);
        }

        private void AddRcMenuItem(string name,RightClickCommands command)
        {
            ToolStripMenuItem tsi = new ToolStripMenuItem(name) { Tag = (int)command };
            showRightClickMenu.Items.Add(tsi);
        }

        private void lvAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSearchButtons();

            LvResults lvr = new LvResults(lvAction, false);

            if (lvr.Count == 0)
            {
                // disable everything
                btnActionBTSearch.Enabled = false;
                return;
            }

            btnActionBTSearch.Enabled = lvr.SaveImages.Count <= 0;

            mLastShowsClicked = null;
            mLastEpClicked = null;
            mLastProcessedSeasonClicked = null;
            mLastActionsClicked = null;

            showRightClickMenu.Items.Clear();
            mFoldersToOpen = new List<string>();
            mLastFl = new List<FileInfo>();

            mLastActionsClicked = new ItemList();

            foreach (Item ai in lvr.FlatList)
            {
                mLastActionsClicked.Add(ai);
            }

            if (lvr.Count != 1 || lvAction.FocusedItem?.Tag is null)
            {
                return;
            }

            if (!(lvAction.FocusedItem.Tag is Item action))
            {
                return;
            }

            mLastEpClicked = action.Episode;
            mLastEpClickedScan = mLastEpClicked;

            if (action.Episode != null)
            {
                mLastProcessedSeasonClicked = action.Episode.AppropriateProcessedSeason;
                mLastShowsClicked = new List<ShowItem> {action.Episode.Show};
            }
            else
            {
                mLastProcessedSeasonClicked = null;
                mLastShowsClicked = null;
            }
        }

        private void ActionDeleteSelected()
        {
            ListView.SelectedListViewItemCollection sel = lvAction.SelectedItems;
            foreach (ListViewItem lvi in sel)
            {
                mDoc.TheActionList.Remove((Item) lvi.Tag);
            }

            FillActionList(true);
        }

        private void lvAction_KeyDown(object sender, [NotNull] KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                ActionDeleteSelected();
            }
        }

        private void cbActionIgnore_Click(object sender, EventArgs e) => IgnoreSelected();

        private void UpdateActionCheckboxes()
        {
            if (internalCheckChange)
            {
                return;
            }

            LvResults all = new LvResults(lvAction, LvResults.WhichResults.all);
            LvResults chk = new LvResults(lvAction, LvResults.WhichResults.Checked);

            SetCheckbox(mcbRename, all.Rename, chk.Rename);
            SetCheckbox(mcbCopyMove, all.CopyMove, chk.CopyMove);
            SetCheckbox(mcbDeleteFiles, all.Deletes, chk.Deletes);
            SetCheckbox(mcbSaveImages,all.SaveImages,chk.SaveImages);
            SetCheckbox(mcbWriteMetadata, all.WriteMetadatas, chk.WriteMetadatas);
            SetCheckbox(mcbModifyMetadata, all.ModifyMetadatas, chk.ModifyMetadatas);
            SetCheckbox(mcbDownload, all.DownloadTorrents, chk.DownloadTorrents);

            int total1 = all.FlatList.Count-all.Downloading.Count-all.Missing.Count;

            int total2 = chk.FlatList.Count - chk.Downloading.Count - chk.Missing.Count;

            if (total2 == 0)
            {
                mcbAll.CheckState = CheckState.Unchecked;
            }
            else
            {
                mcbAll.CheckState = total2 == total1 ? CheckState.Checked : CheckState.Indeterminate;
            }
        }

        private static void SetCheckbox([NotNull] ToolStripMenuItem box,[NotNull] IEnumerable<Item> all, [NotNull] IEnumerable<Item> chk)
        {
            IEnumerable<Item> enumerable = chk.ToList();
            IEnumerable<Item> btn = all as Item[] ?? all.ToArray();
            if (!enumerable.Any())
            {
                box.CheckState = CheckState.Unchecked;
            }
            else
            {
                box.CheckState = enumerable.Count() == btn.Count()
                    ? CheckState.Checked
                    : CheckState.Indeterminate;
            }

            box.Enabled = btn.Any();
        }

        private void lvAction_ItemCheck(object sender, [NotNull] ItemCheckEventArgs e)
        {
            if (e.Index < 0 || e.Index > lvAction.Items.Count)
            {
                return;
            }

            Item action = (Item) lvAction.Items[e.Index].Tag;
            if (action != null && (action is ItemMissing || action is ItemDownloading))
            {
                e.NewValue = CheckState.Unchecked;
            }
        }

        private void bnActionOptions_Click(object sender, EventArgs e) => DoPrefs(true);

        private void lvAction_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // double-click on an item will search for missing, do nothing (for now) for anything else
            foreach (ItemMissing miss in new LvResults(lvAction, false).Missing)
            {
                if (miss.Episode != null)
                {
                    TVDoc.SearchForEpisode(miss.Episode);
                }
            }
        }

        private void bnActionBTSearch_Click(object sender, EventArgs e)
        {
            LvResults lvr = new LvResults(lvAction, false);

            if (lvr.Count == 0)
            {
                return;
            }

            foreach (Item i in lvr.FlatList)
            {
                if (i?.Episode != null)
                {
                    TVDoc.SearchForEpisode(i.Episode);
                }
            }
        }

        private void bnRemoveSel_Click(object sender, EventArgs e) => ActionDeleteSelected();

        private void IgnoreSelected()
        {
            LvResults lvr = new LvResults(lvAction, false);
            bool added = false;
            foreach (Item action in lvr.FlatList)
            {
                IgnoreItem ii = action.Ignore;
                if (ii != null)
                {
                    TVSettings.Instance.Ignore.Add(ii);
                    added = true;
                }
            }

            if (added)
            {
                mDoc.SetDirty();
                mDoc.RemoveIgnored();
                FillActionList(true);
            }
        }

        private void ignoreListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IgnoreEdit ie = new IgnoreEdit(mDoc);
            ie.ShowDialog();
        }

        private async void showSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UseWaitCursor = true;
            ShowSummary f = new ShowSummary(mDoc);
            await Task.Run(() => f.GenerateData()); //do not use configure await = false here as it causes UI to hang
            f.PopulateGrid();
            UseWaitCursor = false;
            f.Show();
        }

        private void lvAction_ItemChecked(object sender, ItemCheckedEventArgs e) => UpdateActionCheckboxes();

        private void btnFilter_Click(object sender, EventArgs e)
        {
            Filters filters = new Filters(mDoc);
            DialogResult res = filters.ShowDialog();
            if (res == DialogResult.OK)
            {
                FillMyShows();
            }
        }

        private void lvAction_DragDrop(object sender, [NotNull] DragEventArgs e)
        {
            // Get a list of filenames being dragged
            string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop, false);

            // Establish item in list being dragged to, and exit if no item matched
            Point localPoint = lvAction.PointToClient(new Point(e.X, e.Y));
            ListViewItem lvi = lvAction.GetItemAt(localPoint.X, localPoint.Y);
            if (lvi is null)
            {
                return;
            }

            // Check at least one file was being dragged, and that dragged-to item is a "Missing Item" item.
            if (files.Length <= 0 || !(lvi.Tag is ItemMissing mi))
            {
                return;
            }

            // Only want the first file if multiple files were dragged across.
            ManuallyAddFileForItem(mi, files[0]);
        }

        private void lvAction_DragEnter(object sender, [NotNull] DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
            Point localPoint = lvAction.PointToClient(new Point(e.X, e.Y));
            ListViewItem lvi = lvAction.GetItemAt(localPoint.X, localPoint.Y);
            // If we're not dragging over a "ItemMissing" entry, or if we're not dragging a list of files, then change the DragDropEffect
            if (!(lvi?.Tag is ItemMissing) || !e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void filterTextBox_TextChanged(object sender, EventArgs e) => FillMyShows();

        private void filterTextBox_SizeChanged(object sender, EventArgs e)
        {
            // MAH: move the "Clear" button in the Filter Text Box
            if (filterTextBox.Controls.ContainsKey("Clear"))
            {
                Control filterButton = filterTextBox.Controls["Clear"];
                int clientSizeHeight = (filterTextBox.ClientSize.Height - 16) / 2;
                filterButton.Location = new Point(filterTextBox.ClientSize.Width - filterButton.Width,clientSizeHeight + 1);

                // Send EM_SETMARGINS to prevent text from disappearing underneath the button
                NativeMethods.SendMessage(filterTextBox.Handle, 0xd3, (IntPtr) 2, (IntPtr) (filterButton.Width << 16));
            }
        }

        private void visitSupportForumToolStripMenuItem_Click(object sender, EventArgs e)
            => Helpers.SysOpen("https://groups.google.com/forum/#!forum/tvrename");

        public void Quit() => Close();

        private async void checkForNewVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dispatcher uiDisp = Dispatcher.CurrentDispatcher;

            Task<Release> tuv = VersionUpdater.CheckForUpdatesAsync();
            Release result = await tuv.ConfigureAwait(false);

            uiDisp.Invoke(() => NotifyUpdates(result, true));
        }

        private void NotifyUpdates([CanBeNull] Release update, bool showNoUpdateRequiredDialog, bool inSilentMode = false)
        {
            if (update is null)
            {
                btnUpdateAvailable.Visible = false;
                if (showNoUpdateRequiredDialog && !inSilentMode)
                {
                    MessageBox.Show(@"There is no update available please try again later.", @"No update available",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }

            Logger.Warn(update.LogMessage());

            if (inSilentMode || Debugger.IsAttached)
            {
               return;
            }

            UpdateNotification unForm = new UpdateNotification(update);
            unForm.ShowDialog();
            if (unForm.DialogResult == DialogResult.Abort)
            {
                Logger.Info("Downloading New Release and Quiting");
                //We need to quit!
                Close();
            }
            btnUpdateAvailable.Visible = true;
        }

        private void duplicateFinderLOGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<PossibleDuplicateEpisode> x = Beta.FindDoubleEps(mDoc);
            DupEpFinder form = new DupEpFinder(x, mDoc, this);
            form.ShowDialog();
        }

        private async void btnUpdateAvailable_Click(object sender, EventArgs e)
        {
            btnUpdateAvailable.Visible = false;

            Dispatcher uiDisp = Dispatcher.CurrentDispatcher;

            Task<Release> tuv = VersionUpdater.CheckForUpdatesAsync();
            Release result = await tuv.ConfigureAwait(false);

            uiDisp.Invoke(() => NotifyUpdates(result, true));
        }

        private void tmrPeriodicScan_Tick(object sender, EventArgs e) => RunAutoScan("Periodic Scan");

        private void RunAutoScan(string scanType)
        {
            //We only wish to do a scan now if we are not already undertaking one
            if (mDoc.AutoScanCanRun())
            {
                Logger.Info("*******************************");
                Logger.Info(scanType + " fired");
                ScanAndAction(TVSettings.Instance.MonitoredFoldersScanType);
                Logger.Info(scanType + " complete");
            }
            else
            {
                Logger.Info(scanType + " cancelled as the system is already busy");
            }
        }

        private void timezoneInconsistencyLOGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Show Log Pane
            logToolStripMenuItem_Click(sender,e);

            Task.Run(() => {
                TimeZoneTracker results = new TimeZoneTracker();
                foreach (ShowItem si in mDoc.Library.GetSortedShowItems())
                {
                    SeriesInfo ser = si.TheSeries();
                    if (ser != null)
                    {
                        results.Add(ser.Network, si.ShowTimeZone, si.ShowName);
                    }
                }
                Logger.Info(results.PrintVersion());
            });
        }

        private class TimeZoneTracker
        {
            private readonly Dictionary<string, Dictionary<string, List<string>>> tzt =
                new Dictionary<string, Dictionary<string, List<string>>>();

            internal void Add([NotNull] string network, [NotNull] string timezone, string show)
            {
                if (!tzt.ContainsKey(network))
                {
                    tzt.Add(network, new Dictionary<string, List<string>>());
                }

                Dictionary<string, List<string>> snet = tzt[network];

                if (!snet.ContainsKey(timezone))
                {
                    snet.Add(timezone, new List<string>());
                }

                List<string> snettz = snet[timezone];

                snettz.Add(show);
            }

            [NotNull]
            internal string PrintVersion()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("***********************************");
                sb.AppendLine("****Timezone Comparison       *****");
                sb.AppendLine("***********************************");
                foreach (KeyValuePair<string, Dictionary<string, List<string>>> kvp in tzt)
                {
                    foreach (KeyValuePair<string, List<string>> kvp2 in kvp.Value)
                    {
                        sb.AppendLine($"{kvp.Key,-30}{kvp2.Key,-30}{kvp2.Value.ToCsv()}");
                    }
                }

                return sb.ToString();
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mDoc.RunExporters();
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogViewer form = new LogViewer();
            form.Show();
        }

        private void episodeFileQualitySummaryLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Show Log Pane
            logToolStripMenuItem_Click(sender, e);

            Task.Run(() => {
                Beta.LogShowEpisodeSizes(mDoc);
            });
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LicenceInfoForm form = new LicenceInfoForm();
            form.ShowDialog();
        }

        private void LvAction_ColumnClick(object sender, [NotNull] ColumnClickEventArgs e)
        {
            int col = e.Column;

            switch (col)
            {
                case 3:
                    lvAction.ListViewItemSorter = new DateSorterScan(col);
                    break;
                case 1:
                case 2:
                    lvAction.ListViewItemSorter = new NumberAsTextSorter(col);
                    break;
                default:
                    lvAction.ListViewItemSorter = new TextSorter(col);
                    break;
            }

            lvAction.Sort();
            lvAction.Refresh();
        }

        private void QuickRenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuickRename form = new QuickRename(mDoc, this);
            form.Show();
        }

        public void FocusOnScanResults()
        {
            tabControl1.SelectedTab = tbAllInOne;
        }

        private void AccuracyCheckLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Show Log Pane
            logToolStripMenuItem_Click(sender, e);

            Cursor.Current = Cursors.WaitCursor;
            txtDLStatusLabel.Text = "Background download: Accuracy Check running";

            mDoc.TVDBServerAccuracyCheck(false, false);

            txtDLStatusLabel.Text = "Background download: Idle";
            Cursor.Current = Cursors.Default;
        }

        private void ToolStripButton5_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
            btnHideHTMLPanel.Image = splitContainer1.Panel2Collapsed ? Resources.FillRight : Resources.FillLeft;
        }

        private void BtnMyShowsCollapse_Click(object sender, EventArgs e)
        {
            MyShowTree.BeginUpdate();
            treeExpandCollapseToggle = !treeExpandCollapseToggle;
            if (treeExpandCollapseToggle)
            {
                MyShowTree.CollapseAll();
            }
            else
            {
                MyShowTree.ExpandAll();
            }

            MyShowTree.SelectedNode?.EnsureVisible();

            MyShowTree.EndUpdate();
        }

        private void FullToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TODO Persist scan type to settings
            uiScanType = TVSettings.ScanType.Full;
            btnScan.Text = "Full Scan";
        }

        private void RecentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uiScanType = TVSettings.ScanType.Recent;
            btnScan.Text = "Recent Scan";
        }

        private void QuickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uiScanType = TVSettings.ScanType.Quick;
            btnScan.Text = "Quick Scan";
        }

        private void BtnActionBTSearch_DropDownOpening(object sender, EventArgs e)
        {
            ChooseSiteMenu(btnActionBTSearch);
        }

        private void ToolStripSplitButton1_DropDownOpening(object sender, EventArgs e)
        {
            ChooseSiteMenu(btnWTWBTSearch);
        }

        private void BtnSearch_ButtonClick(object sender, EventArgs e)
        {
            UiScan(null, false, uiScanType);
        }

        private void UpdateCheckboxGroup([NotNull] ToolStripMenuItem menuItem, [NotNull] Func<Item, bool> isValid)
        {
            switch (menuItem.CheckState) {
                case CheckState.Unchecked:
                    menuItem.CheckState = CheckState.Checked;
                    break;

                case CheckState.Checked:
                    menuItem.CheckState = CheckState.Unchecked;
                    break;

                case CheckState.Indeterminate:
                    menuItem.CheckState = CheckState.Unchecked;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            CheckState cs = menuItem.CheckState;

            internalCheckChange = true;
            foreach (ListViewItem lvi in lvAction.Items)
            {
                Item i = (Item)lvi.Tag;
                if (isValid(i))
                {
                    lvi.Checked = cs == CheckState.Checked;
                }
            }

            internalCheckChange = false;
            UpdateActionCheckboxes();
        }

        private void McbAll_Click(object sender, EventArgs e)
        {
            UpdateCheckboxGroup(mcbAll, item => true);
        }
        private void McbRename_Click(object sender, EventArgs e)
        {
            UpdateCheckboxGroup(mcbRename, i => i is ActionCopyMoveRename rename && rename.Operation == ActionCopyMoveRename.Op.rename);
        }

        private void McbCopyMove_Click(object sender, EventArgs e)
        {
            UpdateCheckboxGroup(mcbCopyMove, i => i is ActionCopyMoveRename copymove && copymove.Operation != ActionCopyMoveRename.Op.rename);
        }

        private void McbDeleteFiles_Click(object sender, EventArgs e)
        {
            UpdateCheckboxGroup(mcbDeleteFiles,i => i is ActionDelete);
        }

        private void McbSaveImages_Click(object sender, EventArgs e)
        {
            UpdateCheckboxGroup(mcbSaveImages, i => i is ActionDownloadImage);
        }

        private void McbDownload_Click(object sender, EventArgs e)
        {
            UpdateCheckboxGroup(mcbDownload, i => i is ActionTDownload);
        }

        private void McbWriteMetadata_Click(object sender, EventArgs e)
        {
            UpdateCheckboxGroup(mcbWriteMetadata, i => i is ActionWriteMetadata);
        }

        private void McbModifyMetadata_Click(object sender, EventArgs e)
        {
            UpdateCheckboxGroup(mcbModifyMetadata, i => i is ActionFileMetaData);
        }
    }
}
