// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using NLog;
using TVRename.Forms;
using TVRename.Ipc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    // right click commands
    public enum RightClickCommands
    {
        none = 0,
        kEpisodeGuideForShow = 1,
        kVisitTvdbEpisode,
        kVisitTvdbSeason,
        kVisitTvdbSeries,
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
        kDeleteShow,
        kUpdateImages,
        kActionRevert,
        kWatchBase = 1000,
        kOpenFolderBase = 2000
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

        public delegate void AutoFolderMonitorDelegate();

        #endregion

        private int busy;
        private TVDoc mDoc;
        private bool internalCheckChange;
        private int lastDlRemaining;

        public AutoFolderMonitorDelegate AfmFullScan;
        public AutoFolderMonitorDelegate AfmRecentScan;
        public AutoFolderMonitorDelegate AfmQuickScan;
        public AutoFolderMonitorDelegate AfmDoAll;

        private readonly SetProgressDelegate setProgress;
        private MyListView lvAction;
        private List<string> mFoldersToOpen;
        private int mInternalChange;
        private List<FileInfo> mLastFl;
        private Point mLastNonMaximizedLocation;
        private Size mLastNonMaximizedSize;
        private readonly AutoFolderMonitor mAutoFolderMonitor;
        private bool treeExpandCollapseToggle = true;

        private ItemList mLastActionsClicked;
        private ProcessedEpisode mLastEpClicked;
        private readonly string mLastFolderClicked;
        private Season mLastSeasonClicked;
        private List<ShowItem> mLastShowsClicked;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public UI(TVDoc doc, TVRenameSplash splash, bool showUi)
        {
            mDoc = doc;

            busy = 0;
            mLastEpClicked = null;
            mLastFolderClicked = null;
            mLastSeasonClicked = null;
            mLastShowsClicked = null;
            mLastActionsClicked = null;

            mInternalChange = 0;
            mFoldersToOpen = new List<string>();

            internalCheckChange = false;

            InitializeComponent();

            SetupIpc();

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

            setProgress += SetProgressActual;

            lvWhenToWatch.ListViewItemSorter = new DateSorterWTW();

            if (mDoc.Args.Hide)
            {
                WindowState = FormWindowState.Minimized;
                Visible = false;
                Hide();
            }

            UpdateSplashStatus(splash, "Filling Shows");

            if (!TVSettings.Instance.ShowCollections)
            {
                collToolStripMenuItem.Visible = false;
                toolStripSeparator0.Visible = false;
            }

            FillMyShows();
            UpdateSearchButtons();
            ClearInfoWindows();
            UpdateSplashPercent(splash, 10);
            UpdateSplashStatus(splash, "Updating WTW");
            mDoc.DoWhenToWatch(true);
            UpdateSplashPercent(splash, 40);
            FillWhenToWatchList();
            UpdateSplashPercent(splash, 60);
            UpdateSplashStatus(splash, "Write Upcoming");
            mDoc.WriteUpcoming();
            UpdateSplashStatus(splash, "Setting Notifications");
            ShowHideNotificationIcon();

            BuildCollectionsMenu();

            int t = TVSettings.Instance.StartupTab;
            if (t < tabControl1.TabCount)
                tabControl1.SelectedIndex = TVSettings.Instance.StartupTab;

            tabControl1_SelectedIndexChanged(null, null);
            UpdateSplashStatus(splash, "Creating Monitors");

            mAutoFolderMonitor = new AutoFolderMonitor(mDoc, this);

            tmrPeriodicScan.Interval = TVSettings.Instance.PeriodicCheckPeriod();

            UpdateSplashStatus(splash, "Starting Monitor");
            if (TVSettings.Instance.MonitorFolders)
                mAutoFolderMonitor.StartMonitor();

            tmrPeriodicScan.Enabled = TVSettings.Instance.RunPeriodicCheck();

            UpdateSplashStatus(splash, "Running autoscan");
        }

        private static void UpdateSplashStatus(TVRenameSplash splashScreen, string text)
        {
            Logger.Info($"Splash Screen Updated with: {text}");
            splashScreen.Invoke((System.Action) delegate { splashScreen.UpdateStatus(text); });
        }

        private static void UpdateSplashPercent(TVRenameSplash splashScreen, int num)
        {
            splashScreen.Invoke((System.Action) delegate { splashScreen.UpdateProgress(num); });
        }

        private void ClearInfoWindows() => ClearInfoWindows("");

        private void ClearInfoWindows(string defaultText)
        {
            SetHtmlBody(ShowHtmlHelper.CreateOldPage(defaultText), webImages);
            SetHtmlBody(ShowHtmlHelper.CreateOldPage(defaultText), webInformation);
        }

        private static int BgdlLongInterval() => 1000 * 60 * 60; // one hour

        private void MoreBusy() => Interlocked.Increment(ref busy);

        private void LessBusy() => Interlocked.Decrement(ref busy);

        private void SetupIpc()
        {
            AfmFullScan += Scan;
            AfmQuickScan += QuickScan;
            AfmRecentScan += RecentScan;
            AfmDoAll += ProcessAll;
        }

        private void SetProgressActual(int p)
        {
            if (p < 0)
                p = 0;
            else if (p > 100)
                p = 100;

            pbProgressBarx.Value = p;
            pbProgressBarx.Update();
        }

        private void ProcessArgs()
        {
            // TODO: Unify command line handling between here and in Program.cs

            if (mDoc.Args.Scan)
                UiScan(null, true, TVSettings.ScanType.Full);

            if (mDoc.Args.QuickScan)
                UiScan(null, true, TVSettings.ScanType.Quick);

            if (mDoc.Args.RecentScan)
                UiScan(null, true, TVSettings.ScanType.Recent);

            if (mDoc.Args.DoAll)
                ProcessAll();

            if (mDoc.Args.Quit || mDoc.Args.Hide)
                Close();
        }

        private void UpdateSearchButtons()
        {
            string name = TVDoc.GetSearchers().Name(TVSettings.Instance.TheSearchers.CurrentSearchNum());

            bnWTWBTSearch.Text = UseCustom(lvWhenToWatch) ? "Search" : name;
            bnActionBTSearch.Text = UseCustom(lvAction) ? "Search" : name;

            FillEpGuideHtml();
        }

        private void visitWebsiteToolStripMenuItem_Click(object sender, EventArgs eventArgs) =>
            Helpers.SysOpen("http://tvrename.com");

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Close();

        private static bool UseCustom(ListView view)
        {
            foreach (ListViewItem lvi in view.SelectedItems)
            {
                if (!(lvi.Tag is ProcessedEpisode pe)) continue;
                if (!pe.Show.UseCustomSearchUrl) continue;
                if (string.IsNullOrWhiteSpace(pe.Show.CustomSearchUrl)) continue;

                return true;
            }

            return false;
        }

        private void UI_Load(object sender, EventArgs e)
        {
            ShowInTaskbar = TVSettings.Instance.ShowInTaskbar && !mDoc.Args.Hide;

            foreach (TabPage tp in tabControl1.TabPages) // grr! TODO: why does it go white?
                tp.BackColor = SystemColors.Control;

            // MAH: Create a "Clear" button in the Filter Text Box
            Button filterButton = new Button
            {
                Size = new Size(16, 16),
                Cursor = Cursors.Default,
                Image = Properties.Resources.DeleteSmall,
                Name = "Clear"
            };

            filterButton.Location = new Point(filterTextBox.ClientSize.Width - filterButton.Width,
                (filterTextBox.ClientSize.Height - 16) / 2 + 1);

            filterButton.Click += filterButton_Click;
            filterTextBox.Controls.Add(filterButton);
            // Send EM_SETMARGINS to prevent text from disappearing underneath the button
            NativeMethods.SendMessage(filterTextBox.Handle, 0xd3, (IntPtr) 2, (IntPtr) (filterButton.Width << 16));

            betaToolsToolStripMenuItem.Visible = TVSettings.Instance.IncludeBetaUpdates();

            Show();
            UI_LocationChanged(null, null);
            UI_SizeChanged(null, null);

            ToolTip tt = new ToolTip();
            tt.SetToolTip(btnActionQuickScan,
                "Scan shows with missing recent aired episodes and and shows that match files in the search folders");

            tt.SetToolTip(bnActionRecentCheck, "Scan shows with recent aired episodes");
            tt.SetToolTip(bnActionCheck, "Scan all shows");

            backgroundDownloadToolStripMenuItem.Checked = TVSettings.Instance.BGDownload;
            offlineOperationToolStripMenuItem.Checked = TVSettings.Instance.OfflineMode;
            BGDownloadTimer.Interval = 10000; // first time
            if (TVSettings.Instance.BGDownload)
                BGDownloadTimer.Start();

            UpdateTimer.Start();

            quickTimer.Start();

            if (TVSettings.Instance.RunOnStartUp())
            {
                RunAutoScan("Startup Scan");
            }
        }

        // MAH: Added in support of the Filter TextBox Button
        private void filterButton_Click(object sender, EventArgs e) => filterTextBox.Clear();

        private ListView ListViewByName(string name)
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

        private void flushCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (busy != 0)
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
                
                TheTVDB.Instance.ForgetEverything();
                FillMyShows();
                FillEpGuideHtml();
                FillWhenToWatchList();
                BGDownloadTimer_QuickFire();
            }
        }

        private bool LoadWidths(XmlReader xml)
        {
            string forwho = xml.GetAttribute("For");

            ListView lv = ListViewByName(forwho);
            if (lv == null)
            {
                xml.ReadOuterXml();
                return true;
            }

            xml.Read();
            int c = 0;
            while (xml.Name == "Width")
            {
                if (c >= lv.Columns.Count)
                    return false;

                lv.Columns[c++].Width = xml.ReadElementContentAsInt();
            }

            xml.Read();
            return true;
        }

        private bool LoadLayoutXml()
        {
            if (mDoc.Args.Hide)
                return true;

            bool ok = true;
            XmlReaderSettings settings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreWhitespace = true
            };

            string fn = PathManager.UILayoutFile.FullName;
            if (!File.Exists(fn))
                return true;

            using (XmlReader reader = XmlReader.Create(fn, settings))
            {
                reader.Read();
                if (reader.Name != "xml")
                    return false;

                reader.Read();
                if (reader.Name != "TVRename")
                    return false;

                if (reader.GetAttribute("Version") != "2.1")
                    return false;

                reader.Read();
                if (reader.Name != "Layout")
                    return false;

                reader.Read();
                while (reader.Name != "Layout")
                {
                    if (reader.Name == "Window")
                    {
                        reader.Read();
                        while (reader.Name != "Window")
                        {
                            if (reader.Name == "Size")
                            {
                                int x = int.Parse(reader.GetAttribute("Width") ?? throw new InvalidOperationException("No Width Specified") );
                                int y = int.Parse(reader.GetAttribute("Height") ?? throw new InvalidOperationException("No Height Specified"));
                                Size = new Size(x, y);
                                reader.Read();
                            }
                            else if (reader.Name == "Location")
                            {
                                int x = int.Parse(reader.GetAttribute("X") ?? throw new InvalidOperationException("No X Specified"));
                                int y = int.Parse(reader.GetAttribute("Y") ?? throw new InvalidOperationException("No Y Specified"));
                                Location = new Point(x, y);
                                reader.Read();
                            }
                            else if (reader.Name == "Maximized")
                                WindowState = reader.ReadElementContentAsBoolean()
                                    ? FormWindowState.Maximized
                                    : FormWindowState.Normal;
                            else
                                reader.ReadOuterXml();
                        }

                        reader.Read();
                    } // window
                    else if (reader.Name == "ColumnWidths")
                        ok = LoadWidths(reader) && ok;
                    else if (reader.Name == "Splitter")
                    {
                        splitContainer1.SplitterDistance = int.Parse(reader.GetAttribute("Distance") ?? throw new InvalidOperationException("No Distance Specified"));
                        splitContainer1.Panel2Collapsed = bool.Parse(reader.GetAttribute("HTMLCollapsed") ?? throw new InvalidOperationException("No HTMLCollapsed Specified"));
                        if (splitContainer1.Panel2Collapsed)
                            bnHideHTMLPanel.ImageKey = "FillLeft.bmp";

                        reader.Read();
                    }
                    else
                        reader.ReadOuterXml();
                } // while
            }

            return ok;
        }

        private bool SaveLayoutXml()
        {
            if (mDoc.Args.Hide)
                return true;

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true
            };

            using (XmlWriter writer = XmlWriter.Create(PathManager.UILayoutFile.FullName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("TVRename");
                XmlHelper.WriteAttributeToXml(writer, "Version", "2.1");
                writer.WriteStartElement("Layout");
                writer.WriteStartElement("Window");

                writer.WriteStartElement("Size");
                XmlHelper.WriteAttributeToXml(writer, "Width", mLastNonMaximizedSize.Width);
                XmlHelper.WriteAttributeToXml(writer, "Height", mLastNonMaximizedSize.Height);
                writer.WriteEndElement(); // size

                writer.WriteStartElement("Location");
                XmlHelper.WriteAttributeToXml(writer, "X", mLastNonMaximizedLocation.X);
                XmlHelper.WriteAttributeToXml(writer, "Y", mLastNonMaximizedLocation.Y);
                writer.WriteEndElement(); // Location

                XmlHelper.WriteElementToXml(writer, "Maximized", WindowState == FormWindowState.Maximized);

                writer.WriteEndElement(); // window

                WriteColWidthsXml("WhenToWatch", writer);
                WriteColWidthsXml("AllInOne", writer);

                writer.WriteStartElement("Splitter");
                XmlHelper.WriteAttributeToXml(writer, "Distance", splitContainer1.SplitterDistance);
                XmlHelper.WriteAttributeToXml(writer, "HTMLCollapsed", splitContainer1.Panel2Collapsed);
                writer.WriteEndElement(); // splitter

                writer.WriteEndElement(); // Layout
                writer.WriteEndElement(); // tvrename
                writer.WriteEndDocument();
            }

            return true;
        }

        private void WriteColWidthsXml(string thingName, XmlWriter writer)
        {
            ListView lv = ListViewByName(thingName);
            if (lv == null)
                return;

            writer.WriteStartElement("ColumnWidths");
            XmlHelper.WriteAttributeToXml(writer, "For", thingName);
            foreach (ColumnHeader lvc in lv.Columns)
            {
                XmlHelper.WriteElementToXml(writer, "Width", lvc.Width);
            }

            writer.WriteEndElement(); // columnwidths
        }

        private void UI_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (mDoc.Dirty())
                {
                    DialogResult res = MessageBox.Show(
                        "Your changes have not been saved.  Do you wish to save before quitting?", "Unsaved data",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                    if (res == DialogResult.Yes)
                    {
                        mDoc.WriteXMLSettings();
                        if (!string.IsNullOrEmpty(PathManager.ShowCollection))
                        {
                            mDoc.WriteXMLShows();
                        }
                    }
                    else if (res == DialogResult.Cancel)
                        e.Cancel = true;
                    else if (res == DialogResult.No)
                    {
                    }
                }

                if (!e.Cancel)
                {
                    SaveLayoutXml();
                    mDoc.TidyTvdb();
                    mDoc.Closing();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + "\r\n\r\n" + ex.StackTrace, "Form Closing Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private ContextMenuStrip BuildSearchMenu()
        {
            menuSearchSites.Items.Clear();
            for (int i = 0; i < TVDoc.GetSearchers().Count(); i++)
            {
                ToolStripMenuItem tsi = new ToolStripMenuItem(TVDoc.GetSearchers().Name(i)) {Tag = i};
                menuSearchSites.Items.Add(tsi);
            }

            return menuSearchSites;
        }

        private void ChooseSiteMenu(int n)
        {
            ContextMenuStrip sm = BuildSearchMenu();
            if (n == 1)
                sm.Show(bnWTWChooseSite, new Point(0, 0));
            else if (n == 0)
                sm.Show(bnActionWhichSearch, new Point(0, 0));
        }

        private void bnWTWChooseSite_Click(object sender, EventArgs e) => ChooseSiteMenu(1);

        private void FillMyShows()
        {
            Season currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
            ShowItem currentSi = TreeNodeToShowItem(MyShowTree.SelectedNode);

            List<ShowItem> expanded = new List<ShowItem>();
            foreach (TreeNode n in MyShowTree.Nodes)
            {
                if (n.IsExpanded)
                    expanded.Add(TreeNodeToShowItem(n));
            }

            MyShowTree.BeginUpdate();

            MyShowTree.Nodes.Clear();
            List<ShowItem> sil = mDoc.Library.GetShowItems();
            ShowFilter filter = TVSettings.Instance.Filter;
            foreach (ShowItem si in sil)
            {
                if (filter.Filter(si)
                    & (string.IsNullOrEmpty(filterTextBox.Text) || si.GetSimplifiedPossibleShowNames().Any(name =>
                           name.Contains(filterTextBox.Text, StringComparison.OrdinalIgnoreCase))
                    ))
                {
                    TreeNode tvn = AddShowItemToTree(si);
                    if (expanded.Contains(si))
                        tvn.Expand();
                }
            }

            foreach (ShowItem si in expanded)
            {
                foreach (TreeNode n in MyShowTree.Nodes)
                {
                    if (TreeNodeToShowItem(n) == si)
                        n.Expand();
                }
            }

            if (currentSeas != null)
                SelectSeason(currentSeas);
            else if (currentSi != null)
                SelectShow(currentSi);

            MyShowTree.EndUpdate();
        }

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
                ShowQuickStartGuide();
            else
            {
                TreeNode n = MyShowTree.SelectedNode;
                FillEpGuideHtml(n);
            }
        }

        private ShowItem TreeNodeToShowItem(TreeNode n)
        {
            if (n == null)
                return null;

            if (n.Tag is ShowItem si)
                return si;

            if (n.Tag is ProcessedEpisode pe)
                return pe.Show;

            if (n.Tag is Season seas)
            {
                if (seas.Episodes.Count == 0) return null;

                return mDoc.Library.ShowItem(seas.TheSeries.TvdbCode);
            }

            return null;
        }

        private static Season TreeNodeToSeason(TreeNode n)
        {
            Season seas = n?.Tag as Season;
            return seas;
        }

        private void FillEpGuideHtml(TreeNode n)
        {
            if (n == null)
            {
                FillEpGuideHtml(null, -1);
                return;
            }

            if (n.Tag is ProcessedEpisode pe)
            {
                FillEpGuideHtml(pe.Show, pe.AppropriateSeasonNumber);
                return;
            }

            Season seas = TreeNodeToSeason(n);
            if (seas != null)
            {
                // we have a TVDB season, but need to find the equiavlent one in our local processed episode collection
                if (seas.Episodes.Count > 0)
                {
                    int tvdbcode = seas.TheSeries.TvdbCode;
                    foreach (ShowItem si in mDoc.Library.Values)
                    {
                        if (si.TvdbCode == tvdbcode)
                        {
                            FillEpGuideHtml(si, seas.SeasonNumber);
                            return;
                        }
                    }
                }

                FillEpGuideHtml(null, -1);
                return;
            }

            FillEpGuideHtml(TreeNodeToShowItem(n), -1);
        }

        private void FillEpGuideHtml(ShowItem si, int snum)
        {
            if (tabControl1.SelectedTab != tbMyShows)
                return;

            if (si == null)
            {
                ClearInfoWindows();
                return;
            }

            TheTVDB.Instance.GetLock("FillEpGuideHTML");

            SeriesInfo ser = TheTVDB.Instance.GetSeries(si.TvdbCode);

            if (ser == null)
            {
                ClearInfoWindows("Not downloaded, or not available");
                TheTVDB.Instance.Unlock("FillEpGuideHTML");
                return;
            }

            string infoPaneBody;
            string imagesPaneBody;


            if (si.DvdOrder && snum >= 0 && ser.DvdSeasons.ContainsKey(snum))
            {
                Season s = ser.DvdSeasons[snum];
                infoPaneBody = si.GetSeasonHtmlOverview(s);
                imagesPaneBody = ShowHtmlHelper.CreateOldPage(si.GetSeasonImagesHtmlOverview(s));
            }
            else if (!si.DvdOrder && snum >= 0 && ser.AiredSeasons.ContainsKey(snum))
            {
                Season s = ser.AiredSeasons[snum];
                infoPaneBody = si.GetSeasonHtmlOverview(s);
                imagesPaneBody = ShowHtmlHelper.CreateOldPage(si.GetSeasonImagesHtmlOverview(s));
            }
            else
            {
                // no epnum specified, just show an overview
                infoPaneBody = si.GetShowHtmlOverview();
                imagesPaneBody = ShowHtmlHelper.CreateOldPage(si.GetShowImagesHtmlOverview());
            }

            TheTVDB.Instance.Unlock("FillEpGuideHTML");
            SetHtmlBody(imagesPaneBody, webImages);
            SetHtmlBody(infoPaneBody, webInformation);
        }

        private static void SetHtmlBody(string body, WebBrowser web)
        {
            try
            {
                web.DocumentText = body;
            }
            catch (Exception ex)
            {
                //Fail gracefully - no RHS episode guide is not too big of a problem.
                Logger.Error(ex);
            }
        }

        private static void TvdbFor(ProcessedEpisode e)
        {
            if (e == null)
                return;

            Helpers.SysOpen(TheTVDB.Instance.WebsiteUrl(e.Show.TvdbCode, e.SeasonId, false));
        }

        private static void TvdbFor(Season seas)
        {
            if (seas == null)
                return;

            Helpers.SysOpen(TheTVDB.Instance.WebsiteUrl(seas.TheSeries.TvdbCode, -1, false));
        }

        private static void TvdbFor(ShowItem si)
        {
            if (si == null)
                return;

            Helpers.SysOpen(TheTVDB.Instance.WebsiteUrl(si.TvdbCode, -1, false));
        }

        private void menuSearchSites_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            mDoc.SetSearcher((int) e.ClickedItem.Tag);
            UpdateSearchButtons();
        }

        private void bnWhenToWatchCheck_Click(object sender, EventArgs e) => RefreshWTW(true);

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
                selections.Add((ProcessedEpisode) lvi.Tag);

            Season currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
            ShowItem currentShowItem = TreeNodeToShowItem(MyShowTree.SelectedNode);

            lvWhenToWatch.Items.Clear();

            List<DateTime> bolded = new List<DateTime>();
            DirFilesCache dfc = new DirFilesCache();

            IEnumerable<ProcessedEpisode> recentEps = mDoc.Library.GetRecentAndFutureEps(dd);

            foreach (ProcessedEpisode ei in recentEps)
            {
                DateTime? dt = ei.GetAirDateDT(true);
                if (dt != null) bolded.Add(dt.Value);

                ListViewItem lvi = new ListViewItem {Text = ""};
                for (int i = 0; i < 7; i++) lvi.SubItems.Add("");

                UpdateWtw(dfc, ei, lvi);

                lvWhenToWatch.Items.Add(lvi);

                foreach (ProcessedEpisode pe in selections)
                {
                    if (!pe.SameAs(ei)) continue;
                    lvi.Selected = true;
                    break;
                }
            }

            lvWhenToWatch.Sort();

            lvWhenToWatch.EndUpdate();
            calCalendar.BoldedDates = bolded.ToArray();

            if (currentSeas != null)
                SelectSeason(currentSeas);
            else if (currentShowItem != null)
                SelectShow(currentShowItem);

            UpdateToolstripWTW();
            mInternalChange--;
        }

        private void lvWhenToWatch_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            int col = e.Column;
            // 3 4, or 6 = do date sort on 3
            // 1 or 2 = number sort
            // 5 = day sort
            // all others, text sort

            if (col == 6) // straight sort by date
            {
                lvWhenToWatch.ListViewItemSorter = new DateSorterWTW();
                lvWhenToWatch.ShowGroups = false;
            }
            else if (col == 3 || col == 4)
            {
                lvWhenToWatch.ListViewItemSorter = new DateSorterWTW();
                lvWhenToWatch.ShowGroups = true;
            }
            else
            {
                lvWhenToWatch.ShowGroups = false;
                if (col == 1 || col == 2)
                    lvWhenToWatch.ListViewItemSorter = new NumberAsTextSorter(col);
                else if (col == 5)
                    lvWhenToWatch.ListViewItemSorter = new DaySorter(col);
                else
                    lvWhenToWatch.ListViewItemSorter = new TextSorter(col);
            }

            lvWhenToWatch.Sort();
        }

        private void lvWhenToWatch_Click(object sender, EventArgs e)
        {
            UpdateSearchButtons();

            if (lvWhenToWatch.SelectedIndices.Count == 0)
            {
                txtWhenToWatchSynopsis.Text = "";
                return;
            }

            int n = lvWhenToWatch.SelectedIndices[0];

            ProcessedEpisode ei = (ProcessedEpisode) lvWhenToWatch.Items[n].Tag;

            if (TVSettings.Instance.HideWtWSpoilers &&
                (ei.HowLong() != "Aired" || lvWhenToWatch.Items[n].ImageIndex == 1))
            {
                txtWhenToWatchSynopsis.Text = "[Spoilers Hidden]";
            }
            else
                txtWhenToWatchSynopsis.Text = ei.Overview;

            mInternalChange++;
            DateTime? dt = ei.GetAirDateDT(true);
            if (dt != null)
            {
                calCalendar.SelectionStart = (DateTime) dt;
                calCalendar.SelectionEnd = (DateTime) dt;
            }

            mInternalChange--;

            if (TVSettings.Instance.AutoSelectShowInMyShows)
                GotoEpguideFor(ei, false);
        }

        private void lvWhenToWatch_DoubleClick(object sender, EventArgs e)
        {
            if (lvWhenToWatch.SelectedItems.Count == 0)
                return;

            ProcessedEpisode ei = (ProcessedEpisode) lvWhenToWatch.SelectedItems[0].Tag;
            List<FileInfo> fl = TVDoc.FindEpOnDisk(null, ei);
            if (fl != null && fl.Count > 0)
            {
                Helpers.SysOpen(fl[0].FullName);
                return;
            }

            // Don't have the episode.  Scan or search?

            switch (TVSettings.Instance.WTWDoubleClick)
            {
                default:
                case TVSettings.WTWDoubleClickAction.Search:
                    bnWTWBTSearch_Click(null, null);
                    break;
                case TVSettings.WTWDoubleClickAction.Scan:
                    UiScan(new List<ShowItem> {ei.Show}, false, TVSettings.ScanType.SingleShow);
                    tabControl1.SelectTab(tbAllInOne);
                    break;
            }
        }

        private void calCalendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            if (mInternalChange != 0)
                return;

            DateTime dt = calCalendar.SelectionStart;
            for (int i = 0; i < lvWhenToWatch.Items.Count; i++)
                lvWhenToWatch.Items[i].Selected = false;

            bool first = true;

            for (int i = 0; i < lvWhenToWatch.Items.Count; i++)
            {
                ListViewItem lvi = lvWhenToWatch.Items[i];
                ProcessedEpisode ei = (ProcessedEpisode) lvi.Tag;
                DateTime? dt2 = ei.GetAirDateDT(true);
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

        public void bnEpGuideRefresh_Click()
        {
            bnWhenToWatchCheck_Click(null, null); // close enough!
            FillMyShows();
        }

        // ReSharper disable once InconsistentNaming
        private void RefreshWTW(bool doDownloads)
        {
            if (doDownloads)
            {
                if (!mDoc.DoDownloadsFG())
                    return;
            }

            mInternalChange++;
            mDoc.DoWhenToWatch(true);
            FillMyShows();
            FillWhenToWatchList();
            mInternalChange--;

            mDoc.WriteUpcoming();
        }

        private void refreshWTWTimer_Tick(object sender, EventArgs e)
        {
            if (busy == 0)
                RefreshWTW(false);
        }

        // ReSharper disable once InconsistentNaming
        private void UpdateToolstripWTW()
        {
            // update toolstrip text too
            List<ProcessedEpisode> next1 = mDoc.Library.NextNShows(1, 0, 36500);

            tsNextShowTxt.Text = "Next airing: ";
            if (next1 != null && next1.Count >= 1)
            {
                ProcessedEpisode ei = next1[0];
                tsNextShowTxt.Text += CustomEpisodeName.NameForNoExt(ei, CustomEpisodeName.OldNStyle(1)) + ", " + ei.HowLong() +
                                      " (" + ei.DayOfWeek() + ", " + ei.TimeOfDay() + ")";
            }
            else
                tsNextShowTxt.Text += "---";
        }

        private void bnWTWBTSearch_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lvWhenToWatch.SelectedItems)
                TVDoc.SearchForEpisode((ProcessedEpisode) lvi.Tag);
        }

        private void NavigateTo(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url == null) return;

            string url = e.Url.AbsoluteUri;

            if (string.Compare(url, "about:blank", StringComparison.Ordinal) == 0)
                return; // don't intercept about:blank

            if (url == QuickStartGuide())
                return; // let the quickstartguide be shown

            if (url.Contains(@"ieframe.dll"))
                url = e.Url.Fragment.Substring(1);

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

            if (string.Compare(url.Substring(0, 7), "http://", StringComparison.Ordinal) == 0 ||
                string.Compare(url.Substring(0, 7), "file://", StringComparison.Ordinal) == 0 ||
                string.Compare(url.Substring(0, 8), "https://", StringComparison.Ordinal) == 0)
            {
                e.Cancel = true;
                Helpers.SysOpen(e.Url.AbsoluteUri);
            }
        }

        private void notifyIcon1_Click(object sender, MouseEventArgs e)
        {
            // double-click of notification icon causes a click then doubleclick event, 
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
                Show();

            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Normal;

            Activate();
        }

        public void Scan()
        {
            UiScan(null, true, TVSettings.ScanType.Full);
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
                tabControl1.SelectTab(tbMyShows);

            FillEpGuideHtml(si, -1);
        }

        public void GotoEpguideFor(ProcessedEpisode ep, bool changeTab)
        {
            if (changeTab)
                tabControl1.SelectTab(tbMyShows);

            SelectSeason(ep.AppropriateSeason);
        }

        private void RightClickOnMyShows(ShowItem si, Point pt)
        {
            mLastShowsClicked = new List<ShowItem> {si};
            mLastEpClicked = null;
            mLastSeasonClicked = null;
            mLastActionsClicked = null;
            BuildRightClickMenu(pt);
        }

        private void RightClickOnMyShows(Season seas, Point pt)
        {
            mLastShowsClicked = new List<ShowItem> {mDoc.Library.ShowItem(seas.TheSeries.TvdbCode)};
            mLastEpClicked = null;
            mLastSeasonClicked = seas;
            mLastActionsClicked = null;
            BuildRightClickMenu(pt);
        }

        private void WtwRightClickOnShow(List<ProcessedEpisode> eps, Point pt)
        {
            if (eps.Count == 0)
                return;

            ProcessedEpisode ep = eps[0];

            List<ShowItem> sis = new List<ShowItem>();
            foreach (ProcessedEpisode e in eps)
            {
                sis.Add(e.Show);
            }

            mLastEpClicked = ep;
            mLastShowsClicked = sis;
            mLastSeasonClicked = ep?.AppropriateSeason;
            mLastActionsClicked = null;
            BuildRightClickMenu(pt);
        }

        private void MenuGuideAndTvdb(bool addSep)
        {
            if (mLastShowsClicked == null || mLastShowsClicked.Count != 1)
                return; // nothing or multiple selected

            ShowItem si = mLastShowsClicked != null && mLastShowsClicked.Count > 0
                ? mLastShowsClicked[0]
                : null;

            Season seas = mLastSeasonClicked;
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
                AddRcMenuItem("Visit thetvdb.com", RightClickCommands.kVisitTvdbEpisode);
            }
            else if (seas != null)
            {
                if (si != null)
                {
                    AddRcMenuItem("Episode Guide", RightClickCommands.kEpisodeGuideForShow);
                }
                AddRcMenuItem("Visit thetvdb.com", RightClickCommands.kVisitTvdbSeason);
            }
            else if (si != null)
            {
                AddRcMenuItem("Episode Guide", RightClickCommands.kEpisodeGuideForShow);
                AddRcMenuItem("Visit thetvdb.com", RightClickCommands.kVisitTvdbSeries);
            }
        }

        private void MenuShowAndEpisodes()
        {
            ShowItem si = mLastShowsClicked != null && mLastShowsClicked.Count > 0
                ? mLastShowsClicked[0]
                : null;

            Season seas = mLastSeasonClicked;
            ProcessedEpisode ep = mLastEpClicked;
            ToolStripMenuItem tsi;

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
                AddRcMenuItem("Edit " + Season.UIFullSeasonWord(seas.SeasonNumber), RightClickCommands.kEditSeason);
            }

            if (ep != null && mLastShowsClicked != null && mLastShowsClicked.Count == 1)
            {
                List<FileInfo> fl = TVDoc.FindEpOnDisk(null, ep);
                if (fl == null) return;

                if (fl.Count <= 0) return;

                showRightClickMenu.Items.Add(new ToolStripSeparator());

                int n = mLastFl.Count;
                foreach (FileInfo fi in fl)
                {
                    mLastFl.Add(fi);
                    tsi = new ToolStripMenuItem("Watch: " + fi.FullName)
                    {
                        Tag = (int) RightClickCommands.kWatchBase + n
                    };

                    showRightClickMenu.Items.Add(tsi);
                }
            }
            else if (seas != null && si != null && mLastShowsClicked != null && mLastShowsClicked.Count == 1)
            {
                // for each episode in season, find it on disk
                bool first = true;
                foreach (ProcessedEpisode epds in si.SeasonEpisodes[seas.SeasonNumber])
                {
                    List<FileInfo> fl = TVDoc.FindEpOnDisk(null, epds);
                    if (fl != null && fl.Count > 0)
                    {
                        if (first)
                        {
                            showRightClickMenu.Items.Add(new ToolStripSeparator());
                            first = false;
                        }

                        int n = mLastFl.Count;
                        foreach (FileInfo fi in fl)
                        {
                            mLastFl.Add(fi);
                            tsi = new ToolStripMenuItem("Watch: " + fi.FullName)
                            {
                                Tag = (int) RightClickCommands.kWatchBase + n
                            };

                            showRightClickMenu.Items.Add(tsi);
                        }
                    }
                }
            }
        }

        private void MenuFolders(LVResults lvr)
        {
            if (mLastShowsClicked == null || mLastShowsClicked.Count != 1)
                return;

            ShowItem si = mLastShowsClicked != null && mLastShowsClicked.Count > 0
                ? mLastShowsClicked[0]
                : null;

            Season seas = mLastSeasonClicked;
            ProcessedEpisode ep = mLastEpClicked;
            ToolStripMenuItem tsi;
            List<string> added = new List<string>();

            if (ep != null)
            {
                if (ep.Show.AllFolderLocations().ContainsKey(ep.AppropriateSeasonNumber))
                {
                    int n = mFoldersToOpen.Count;
                    bool first = true;
                    foreach (string folder in ep.Show.AllFolderLocations()[ep.AppropriateSeasonNumber])
                    {
                        if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
                        {
                            if (first)
                            {
                                showRightClickMenu.Items.Add(new ToolStripSeparator());
                                first = false;
                            }

                            tsi = new ToolStripMenuItem("Open: " + folder);
                            added.Add(folder);
                            mFoldersToOpen.Add(folder);
                            tsi.Tag = (int) RightClickCommands.kOpenFolderBase + n;
                            n++;
                            showRightClickMenu.Items.Add(tsi);
                        }
                    }
                }
            }
            else if (seas != null && si != null && si.AllFolderLocations().ContainsKey(seas.SeasonNumber))
            {
                int n = mFoldersToOpen.Count;
                bool first = true;
                foreach (string folder in si.AllFolderLocations()[seas.SeasonNumber])
                {
                    if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder) && !added.Contains(folder))
                    {
                        added.Add(folder); // don't show the same folder more than once
                        if (first)
                        {
                            showRightClickMenu.Items.Add(new ToolStripSeparator());
                            first = false;
                        }

                        tsi = new ToolStripMenuItem("Open: " + folder);
                        mFoldersToOpen.Add(folder);
                        tsi.Tag = (int) RightClickCommands.kOpenFolderBase + n;
                        n++;
                        showRightClickMenu.Items.Add(tsi);
                    }
                }
            }
            else if (si != null)
            {
                int n = mFoldersToOpen.Count;
                bool first = true;

                foreach (KeyValuePair<int, List<string>> kvp in si.AllFolderLocations())
                {
                    foreach (string folder in kvp.Value)
                    {
                        if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder) && !added.Contains(folder))
                        {
                            added.Add(folder); // don't show the same folder more than once
                            if (first)
                            {
                                showRightClickMenu.Items.Add(new ToolStripSeparator());
                                first = false;
                            }

                            tsi = new ToolStripMenuItem("Open: " + folder);
                            mFoldersToOpen.Add(folder);
                            tsi.Tag = (int) RightClickCommands.kOpenFolderBase + n;
                            n++;
                            showRightClickMenu.Items.Add(tsi);
                        }
                    }
                }
            }

            if (lvr == null) return;
            {
                int n = mFoldersToOpen.Count;
                bool first = true;

                foreach (Item sli in lvr.FlatList)
                {
                    string folder = sli.TargetFolder;

                    if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder) || added.Contains(folder))
                        continue;

                    added.Add(folder); // don't show the same folder more than once
                    if (first)
                    {
                        showRightClickMenu.Items.Add(new ToolStripSeparator());
                        first = false;
                    }

                    tsi = new ToolStripMenuItem("Open: " + folder);
                    mFoldersToOpen.Add(folder);
                    tsi.Tag = (int) RightClickCommands.kOpenFolderBase + n;
                    n++;
                    showRightClickMenu.Items.Add(tsi);
                }
            }
        }

        private void BuildCollectionsMenu ()
        {
            // ActualCollMenuItem
            if (mDoc.ShowCollections.Count == 0)
            {
                // Setup a default collection when we come's from 2.4.x RCy
                ShowCollection Sc = new ShowCollection("2.1");
                Sc.Name = "Default";
                Sc.Description = "Default collection in mono collection use";
                mDoc.ShowCollections.Add(Sc);
                mDoc.WriteXMLCollections();
            }

            ToolStripMenuItem[] CollMenu = new ToolStripMenuItem[mDoc.ShowCollections.Count];
            int i = 0;
            foreach (ShowCollection ShowColl in mDoc.ShowCollections)
            {
                CollMenu[i] = new ToolStripMenuItem();
                CollMenu[i].Name = "CollectionMenuItem" + i.ToString();
                CollMenu[i].Tag = ShowColl;
                CollMenu[i].Text = ShowColl.Name;
                CollMenu[i].ToolTipText = ShowColl.Description;
                if (ShowColl.Path == PathManager.ShowCollection)
                {
                    CollMenu[i].Checked = true;
                }
                CollMenu[i].Click += new EventHandler(SelCollMenuItemClickHandler);
                i++;
            }

            collToolStripMenuItem.DropDownItems.AddRange(CollMenu);
        }

        private void RemoveCollectionsMenu ()
        {
            int iCount = collToolStripMenuItem.DropDownItems.Count -2;
            string MenuItemName = "";
            for (int iCurr = 0; iCurr < iCount; iCurr++)
            {
                MenuItemName = "CollectionMenuItem" + iCurr.ToString();
                collToolStripMenuItem.DropDownItems.RemoveAt(2);
            }
        }

        private void SelCollMenuItemClickHandler(object sender, EventArgs e)
        {
            bool bNeedCancel = false;
            ToolStripMenuItem clickedItem     = (ToolStripMenuItem)sender;
            ShowCollection SelectedCollection = (ShowCollection)clickedItem.Tag;

            try
            {
                if (mDoc.Dirty())
                {
                    DialogResult res = MessageBox.Show(
                        "Your changes have not been saved.  Do you wish to save before changing show collection?", "Unsaved data",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                    if (res == DialogResult.Yes)
                    {
                        mDoc.WriteXMLSettings();
                        if (!string.IsNullOrEmpty(PathManager.ShowCollection))
                        {
                            mDoc.WriteXMLShows();
                        }
                    }
                    else if (res == DialogResult.Cancel)
                        bNeedCancel = true;
                    else if (res == DialogResult.No)
                    {
                        bNeedCancel = false;
                    }
                }

                if (!bNeedCancel)
                {
<<<<<<< HEAD
                    int iMnu = 0;
=======
>>>>>>> 4bc73e173b8f7eb1543dab782078b2d7d54c93d1
                    ToolStripItemCollection TSMIC = collToolStripMenuItem.DropDownItems;
                    ToolStripMenuItem TsM;
                    foreach (ToolStripItem TsI in TSMIC)
                    {
                        try
                        {
                            TsM = (ToolStripMenuItem)TsI;
                            TsM.Checked = false;
                        }
<<<<<<< HEAD
                        catch (Exception ex)
                        {
                            string Message = ex.Message;
=======
                        catch (Exception)
                        {
                            // Just a pass thru for MenuSeparators
>>>>>>> 4bc73e173b8f7eb1543dab782078b2d7d54c93d1
                        }
                    }
                    clickedItem.Checked = true;

                    mDoc.SwitchToCollection(SelectedCollection.Path);
                    mDoc.WriteXMLCollections();
                    FillMyShows();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + "\r\n\r\n" + ex.StackTrace, "Switch Show Collection",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void showRightClickMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            showRightClickMenu.Close();

            if (e.ClickedItem.Tag == null)
            {
                mLastEpClicked = null;
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
                        GotoEpguideFor(mLastEpClicked, true);
                    else
                    {
                        if (si != null)
                            GotoEpguideFor(si, true);
                    }

                    break;

                case RightClickCommands.kVisitTvdbEpisode: // thetvdb.com
                    {
                        TvdbFor(mLastEpClicked);
                        break;
                    }

                case RightClickCommands.kVisitTvdbSeason:
                    {
                        TvdbFor(mLastSeasonClicked);
                        break;
                    }

                case RightClickCommands.kVisitTvdbSeries:
                    {
                        if (si != null)
                            TvdbFor(si);

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
                            code = mLastEpClicked.TheSeries.TvdbCode;

                        if (si != null)
                            code = si.TvdbCode;

                        GotoWtwFor(code);

                        break;
                    }
                case RightClickCommands.kForceRefreshSeries:
                    if (si != null)
                        ForceRefresh(mLastShowsClicked);

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
                        EditShow(si);

                    break;
                case RightClickCommands.kDeleteShow:
                    if (si != null)
                        DeleteShow(si);

                    break;
                case RightClickCommands.kEditSeason:
                    if (si != null)
                        EditSeason(si, mLastSeasonClicked.SeasonNumber);

                    break;
                case RightClickCommands.kBtSearchFor:
                    {
                        foreach (ListViewItem lvi in lvAction.SelectedItems)
                        {
                            ItemMissing m = (ItemMissing) lvi.Tag;
                            if (m != null)
                                TVDoc.SearchForEpisode(m.Episode);
                        }
                    }

                    break;
                case RightClickCommands.kActionAction:
                    ActionAction(false);
                    break;
                case RightClickCommands.kActionRevert:
                    Revert(false);
                    break;
                case RightClickCommands.kActionBrowseForFile:
                    if (mLastActionsClicked != null && mLastActionsClicked.Count > 0)
                    {
                        BrowseForMissingItem((ItemMissing)mLastActionsClicked[0]);
                        mLastActionsClicked = null;
                        FillActionList();
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
                        FillActionList();
                        break;
                    }
                case RightClickCommands.kActionDelete:
                    ActionDeleteSelected();
                    break;
                default:
                {
                    //The entries immedately above WatchBase are the Watchxx commands and the paths are stored in mLastFL
                    if (n >= RightClickCommands.kWatchBase && n < RightClickCommands.kOpenFolderBase)
                    {
                        WatchEpisode(n - RightClickCommands.kWatchBase);
                    }
                    else if (n >= RightClickCommands.kOpenFolderBase)
                    {
                        OpenFolderForShow(n - RightClickCommands.kOpenFolderBase);
                        return;
                    }
                    else
                        Debug.Fail("Unknown right-click action " + n);
                    break;
                }
            }
            mLastEpClicked = null;
        }

        private void BrowseForMissingItem(ItemMissing mi)
        {
            if (mi == null) return;

            // browse for mLastActionClicked
            openFile.Filter = "Video Files|" +
                              TVSettings.Instance.GetVideoExtensionsString()
                                  .Replace(".", "*.") +
                              "|All Files (*.*)|*.*";

            if (openFile.ShowDialog() != DialogResult.OK) return;

            // make new Item for copying/moving to specified location
            FileInfo from = new FileInfo(openFile.FileName);
            FileInfo to = new FileInfo(mi.TheFileNoExt + from.Extension);
            mDoc.TheActionList.Add(
                new ActionCopyMoveRename(
                    TVSettings.Instance.LeaveOriginals
                        ? ActionCopyMoveRename.Op.copy
                        : ActionCopyMoveRename.Op.move, from, to
                    , mi.Episode,
                    TVSettings.Instance.Tidyup, mi));

            // and remove old Missing item
            mDoc.TheActionList.Remove(mi);

            // if we're copying/moving a file across, we might also want to make a thumbnail or NFO for it
            DownloadIdentifiersController di = new DownloadIdentifiersController();
            mDoc.TheActionList.Add(di.ProcessEpisode(mi.Episode, to));
        }

        private void OpenFolderForShow(int fnum)
        {
            if (mFoldersToOpen != null && fnum >= 0 && fnum < mFoldersToOpen.Count)
            {
                string folder = mFoldersToOpen[fnum];

                if (Directory.Exists(folder))
                    Helpers.SysOpen(folder);
            }
        }

        private void WatchEpisode(int wn)
        {
            if (mLastFl != null && wn >= 0 && wn < mLastFl.Count)
                Helpers.SysOpen(mLastFl[wn].FullName);
        }

        private void IgnoreSelectedSeasons()
        {
            if (mLastActionsClicked != null && mLastActionsClicked.Count > 0)
            {
                foreach (Item ai in mLastActionsClicked)
                {
                    Item er = ai;
                    if (er?.Episode == null)
                        continue;

                    int snum = er.Episode.AppropriateSeasonNumber;

                    if (!er.Episode.Show.IgnoreSeasons.Contains(snum))
                        er.Episode.Show.IgnoreSeasons.Add(snum);

                    // remove all other episodes of this season from the Action list
                    ItemList remove = new ItemList();
                    foreach (Item action in mDoc.TheActionList)
                    {
                        Item er2 = action;
                        if (er2?.Episode == null) continue;
                        if (er2.Episode.AppropriateSeasonNumber != snum) continue;

                        if (er2.TargetFolder == er.TargetFolder) //ie if they are for the same series
                            remove.Add(action);
                    }

                    foreach (Item action in remove)
                        mDoc.TheActionList.Remove(action);

                    if (remove.Count > 0)
                        mDoc.SetDirty();
                }

                FillMyShows();
            }
        }

        private void GotoWtwFor(int tvdbSeriesCode)
        {
            if (tvdbSeriesCode == -1) return;

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
                bnMyShowsRefresh_Click(null, null);
            else if (tabControl1.SelectedTab == tbWTW)
                bnWhenToWatchCheck_Click(null, null);
            else if (tabControl1.SelectedTab == tbAllInOne)
                bnActionRecentCheck_Click(null, null);
        }

        private void folderRightClickMenu_ItemClicked(object sender,
            ToolStripItemClickedEventArgs e)
        {
            if ((int) e.ClickedItem.Tag == 0) Helpers.SysOpen(mLastFolderClicked);
        }

        private void lvWhenToWatch_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            if (lvWhenToWatch.SelectedItems.Count == 0)
                return;

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
                collToolStripMenuItem.Visible = (TVSettings.Instance.ShowCollections) ? true : false;
                toolStripSeparator0.Visible = (TVSettings.Instance.ShowCollections) ? true : false;
                FillEpGuideHtml();
                mAutoFolderMonitor.SettingsChanged(TVSettings.Instance.MonitorFolders);
                betaToolsToolStripMenuItem.Visible = TVSettings.Instance.IncludeBetaUpdates();
                ForceRefresh(null);
            }

            mDoc.AllowAutoScan();
            LessBusy();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                mDoc.WriteXMLSettings();
                mDoc.WriteXMLCollections();
                if (!string.IsNullOrEmpty(PathManager.ShowCollection))
                {
                    mDoc.WriteXMLShows();
                }
                TheTVDB.Instance.SaveCache();
                if (!SaveLayoutXml())
                {
                    Logger.Error("Failed to Save Layout Configuration Files");
                }

            }
            catch (Exception ex)
            {
                Exception e2 = ex;
                while (e2.InnerException != null)
                    e2 = e2.InnerException;

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
                mLastNonMaximizedSize = Size;

            if (WindowState == FormWindowState.Minimized && !TVSettings.Instance.ShowInTaskbar)
                Hide();
        }

        private void UI_LocationChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
                mLastNonMaximizedLocation = Location;
        }

        private void statusTimer_Tick(object sender, EventArgs e)
        {
            int n = mDoc.DownloadsRemaining();

            txtDLStatusLabel.Visible = n != 0 || TVSettings.Instance.BGDownload;
            if (n != 0)
            {
                txtDLStatusLabel.Text = "Background download: " + TheTVDB.Instance.CurrentDLTask;
                backgroundDownloadNowToolStripMenuItem.Enabled = false;
            }
            else
                txtDLStatusLabel.Text = "Background download: Idle";

            if (busy == 0)
            {
                if (n == 0 && lastDlRemaining > 0)
                {
                    // we've just finished a bunch of background downloads
                    TheTVDB.Instance.SaveCache();
                    RefreshWTW(false);

                    backgroundDownloadNowToolStripMenuItem.Enabled = true;
                }

                lastDlRemaining = n;
            }
        }

        private void backgroundDownloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TVSettings.Instance.BGDownload = !TVSettings.Instance.BGDownload;
            backgroundDownloadToolStripMenuItem.Checked = TVSettings.Instance.BGDownload;
            statusTimer_Tick(null, null);
            mDoc.SetDirty();

            if (TVSettings.Instance.BGDownload)
                BGDownloadTimer.Start();
            else
                BGDownloadTimer.Stop();
        }

        private async void UpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdateTimer.Stop();

            Task<UpdateVersion> tuv = VersionUpdater.CheckForUpdatesAsync();
            NotifyUpdates(await tuv, false);
        }

        private void BGDownloadTimer_Tick(object sender, EventArgs e)
        {
            if (busy != 0)
            {
                BGDownloadTimer.Interval = 10000; // come back in 10 seconds
                BGDownloadTimer.Start();
                Logger.Info("BG Download is busy - try again in 10 seconds");
                return;
            }

            BGDownloadTimer.Interval = BgdlLongInterval(); // after first time (10 seconds), put up to 60 minutes
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
                    return;
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
                    return;
            }

            TVSettings.Instance.OfflineMode = !TVSettings.Instance.OfflineMode;
            offlineOperationToolStripMenuItem.Checked = TVSettings.Instance.OfflineMode;
            mDoc.SetDirty();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tbMyShows)
                FillEpGuideHtml();

            exportToolStripMenuItem.Enabled = false;
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

        private TreeNode AddShowItemToTree(ShowItem si)
        {
            TheTVDB.Instance.GetLock("AddShowItemToTree");
            string name = si.ShowName;

            SeriesInfo ser = TheTVDB.Instance.GetSeries(si.TvdbCode);

            if (string.IsNullOrEmpty(name))
            {
                if (ser != null)
                    name = ser.Name;
                else
                    name += "-- Unknown : " + si.TvdbCode + " --";
            }

            TreeNode n = new TreeNode(name) {Tag = si};

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
                            n.ForeColor = nodeColor;
                    }
                }

                List<int> theKeys = si.DvdOrder
                    ? new List<int>(ser.DvdSeasons.Keys)
                    : new List<int>(ser.AiredSeasons.Keys);

                theKeys.Sort();

                SeasonFilter sf = TVSettings.Instance.SeasonFilter;
                foreach (int snum in theKeys)
                {
                    Season s = si.DvdOrder ? ser.DvdSeasons[snum] : ser.AiredSeasons[snum];

                    //Ignore the season if it is filtered out
                    if (!sf.Filter(si, s)) continue;

                    string nodeTitle = ShowHtmlHelper.SeasonName(si, snum);

                    TreeNode n2 = new TreeNode(nodeTitle);
                    if (si.IgnoreSeasons.Contains(snum))
                        n2.ForeColor = Color.Gray;
                    else
                    {
                        if (TVSettings.Instance.ShowStatusColors != null)
                        {
                            Color nodeColor =
                                TVSettings.Instance.ShowStatusColors.GetEntry(true, false,
                                    s.Status(si.GetTimeZone()).ToString());

                            if (!nodeColor.IsEmpty)
                                n2.ForeColor = nodeColor;
                        }
                    }

                    n2.Tag = s;
                    n.Nodes.Add(n2);
                }
            }

            MyShowTree.Nodes.Add(n);

            TheTVDB.Instance.Unlock("AddShowItemToTree");

            return n;
        }

        private void UpdateWtw(DirFilesCache dfc, ProcessedEpisode pe, ListViewItem lvi)
        {
            lvi.Tag = pe;

            // group 0 = just missed
            //       1 = this week
            //       2 = future / unknown

            DateTime? airdt = pe.GetAirDateDT(true);
            if (airdt == null)
            {
                // TODO: something!
                return;
            }

            DateTime dt = (DateTime) airdt;

            double ttn = dt.Subtract(DateTime.Now).TotalHours;

            if (ttn < 0)
                lvi.Group = lvWhenToWatch.Groups["justPassed"];
            else if (ttn < 7 * 24)
                lvi.Group = lvWhenToWatch.Groups["next7days"];
            else if (!pe.NextToAir)
                lvi.Group = lvWhenToWatch.Groups["later"];
            else
                lvi.Group = lvWhenToWatch.Groups["futureEps"];

            int n = 1;
            lvi.Text = pe.Show.ShowName;
            lvi.SubItems[n++].Text =
                pe.AppropriateSeasonNumber != 0 ? pe.AppropriateSeasonNumber.ToString() : "Special";

            string estr = pe.AppropriateEpNum > 0 ? pe.AppropriateEpNum.ToString() : "";
            if (pe.AppropriateEpNum > 0 && pe.EpNum2 != pe.AppropriateEpNum && pe.EpNum2 > 0)
                estr += "-" + pe.EpNum2;

            lvi.SubItems[n++].Text = estr;
            lvi.SubItems[n++].Text = dt.ToShortDateString();
            lvi.SubItems[n++].Text = dt.ToString("t");
            lvi.SubItems[n++].Text = dt.ToString("ddd");
            lvi.SubItems[n++].Text = pe.HowLong();
            lvi.SubItems[n++].Text = pe.Name;

            // icon..

            if (airdt.Value.CompareTo(DateTime.Now) < 0) // has aired
            {
                List<FileInfo> fl = TVDoc.FindEpOnDisk(dfc, pe);
                if (fl != null && fl.Count > 0)
                    lvi.ImageIndex = 0;
                else if (pe.Show.DoMissingCheck)
                    lvi.ImageIndex = 1;
            }
        }

        private void SelectSeason(Season seas)
        {
            foreach (TreeNode n in MyShowTree.Nodes)
            {
                foreach (TreeNode n2 in n.Nodes)
                {
                    if (TreeNodeToSeason(n2) == seas)
                    {
                        n2.EnsureVisible();
                        MyShowTree.SelectedNode = n2;
                        return;
                    }
                }
            }

            FillEpGuideHtml(null);
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
            ShowItem si = new ShowItem();
            TheTVDB.Instance.GetLock("AddShow");
            AddEditShow aes = new AddEditShow(si);
            DialogResult dr = aes.ShowDialog();
            TheTVDB.Instance.Unlock("AddShow");
            if (dr == DialogResult.OK)
            {
                mDoc.Library.Add(si);

                ShowAddedOrEdited(true);
                SelectShow(si);

                Logger.Info("Added new show called {0}", si.ShowName);
            }
            else Logger.Info("Cancelled adding new show");

            LessBusy();
        }

        private void ShowAddedOrEdited(bool download)
        {
            mDoc.SetDirty();
            RefreshWTW(download);
            FillMyShows();

            mDoc.ExportShowInfo(); //Save shows list to disk
        }

        private void bnMyShowsDelete_Click(object sender, EventArgs e)
        {
            TreeNode n = MyShowTree.SelectedNode;
            ShowItem si = TreeNodeToShowItem(n);
            if (si == null)
                return;

            DeleteShow(si);
        }

        private void DeleteShow(ShowItem si)
        {
            DialogResult res = MessageBox.Show(
                "Remove show \"" + si.ShowName + "\".  Are you sure?", "Confirmation", MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (res != DialogResult.Yes)
                return;

            mDoc.Library.Remove(si);
            ShowAddedOrEdited(false);
        }

        private void bnMyShowsEdit_Click(object sender, EventArgs e)
        {
            TreeNode n = MyShowTree.SelectedNode;
            if (n == null)
                return;

            Season seas = TreeNodeToSeason(n);
            if (seas != null)
            {
                ShowItem si = TreeNodeToShowItem(n);
                if (si != null)
                    EditSeason(si, seas.SeasonNumber);

                return;
            }

            ShowItem si2 = TreeNodeToShowItem(n);
            if (si2 != null)
            {
                EditShow(si2);
            }
        }

        internal void EditSeason(ShowItem si, int seasnum)
        {
            MoreBusy();

            TheTVDB.Instance.GetLock("EditSeason");
            SeriesInfo ser = TheTVDB.Instance.GetSeries(si.TvdbCode);
            List<ProcessedEpisode> pel = ShowLibrary.GenerateEpisodes(si, ser, seasnum, false);

            EditRules er = new EditRules(si, pel, seasnum, TVSettings.Instance.NamingStyle);
            DialogResult dr = er.ShowDialog();
            TheTVDB.Instance.Unlock("EditSeason");
            if (dr == DialogResult.OK)
            {
                ShowAddedOrEdited(false);
                Dictionary<int, Season> seasonsToUse = si.DvdOrder ? ser.DvdSeasons : ser.AiredSeasons;
                SelectSeason(seasonsToUse[seasnum]);
            }

            LessBusy();
        }

        internal void EditShow(ShowItem si)
        {
            MoreBusy();
            TheTVDB.Instance.GetLock("EditShow");

            int oldCode = si.TvdbCode;

            AddEditShow aes = new AddEditShow(si);

            DialogResult dr = aes.ShowDialog();

            TheTVDB.Instance.Unlock("EditShow");

            if (dr == DialogResult.OK)
            {
                ShowAddedOrEdited(si.TvdbCode != oldCode);
                SelectShow(si);

                Logger.Info("Modified show called {0}", si.ShowName);
            }

            LessBusy();
        }

        internal void ForceRefresh(List<ShowItem> sis)
        {
            mDoc.ForceRefresh(sis);
            FillMyShows();
            FillEpGuideHtml();
            RefreshWTW(false);
        }

        private void UpdateImages(List<ShowItem> sis)
        {
            if (sis != null)
            {
                ForceRefresh(sis);

                foreach (ShowItem si in sis)
                {
                    //update images for the showitem
                    mDoc.ForceUpdateImages(si);
                }

                FillActionList();
            }
        }

        private void bnMyShowsRefresh_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                // nuke currently selected show to force getting it fresh
                TreeNode n = MyShowTree.SelectedNode;
                ShowItem si = TreeNodeToShowItem(n);
                ForceRefresh(new List<ShowItem> {si});
            }
            else
            {
                ForceRefresh(null);
            }
        }

        private void MyShowTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            FillEpGuideHtml(e.Node);
            bool showSelected = MyShowTree.SelectedNode != null;
            bnMyShowsEdit.Enabled = showSelected;
            bnMyShowsDelete.Enabled = showSelected;
        }

        private void MyShowTree_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            MyShowTree.SelectedNode = MyShowTree.GetNodeAt(e.X, e.Y);

            Point pt = MyShowTree.PointToScreen(new Point(e.X, e.Y));
            TreeNode n = MyShowTree.SelectedNode;

            if (n == null)
                return;

            ShowItem si = TreeNodeToShowItem(n);
            Season seas = TreeNodeToSeason(n);

            if (seas != null)
                RightClickOnMyShows(seas, pt);
            else if (si != null)
                RightClickOnMyShows(si, pt);
        }

        private void quickstartGuideToolStripMenuItem_Click(object sender, EventArgs e) => ShowQuickStartGuide();

        private Season CurrentlySelectedSeason()
        {
            Season currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
            if (currentSeas != null) return currentSeas;

            ShowItem currentShow = TreeNodeToShowItem(MyShowTree.SelectedNode);
            if (currentShow != null)
            {
                foreach (KeyValuePair<int, Season> s in currentShow.AppropriateSeasons())
                {
                    //Find first season we can
                    return s.Value;
                }
            }

            foreach (ShowItem si in mDoc.Library.GetShowItems())
            {
                foreach (KeyValuePair<int, Season> s in si.AppropriateSeasons())
                {
                    //Find first season we can
                    return s.Value;
                }
            }
            return null;
        }
        private List<ProcessedEpisode> CurrentlySelectedPel()
        {
            Season currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
            ShowItem currentShow = TreeNodeToShowItem(MyShowTree.SelectedNode);

            int snum = currentSeas?.SeasonNumber ?? 1;
            List<ProcessedEpisode> pel = null;
            if (currentShow != null && currentShow.SeasonEpisodes.ContainsKey(snum))
                pel = currentShow.SeasonEpisodes[snum];
            else
            {
                foreach (ShowItem si in mDoc.Library.GetShowItems())
                {
                    foreach (KeyValuePair<int, List<ProcessedEpisode>> kvp in si.SeasonEpisodes)
                    {
                        pel = kvp.Value;
                        break;
                    }

                    if (pel != null)
                        break;
                }
            }

            return pel;
        }

        private void filenameTemplateEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomEpisodeName cn = new CustomEpisodeName(TVSettings.Instance.NamingStyle.StyleString);
            CustomNameDesigner cne = new CustomNameDesigner(CurrentlySelectedPel(), cn, mDoc);
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

            AddEditSeasEpFinders d = new AddEditSeasEpFinders(TVSettings.Instance.FNPRegexs, mDoc.Library.GetShowItems(), currentShow, theFolder);

            DialogResult dr = d.ShowDialog();
            if (dr == DialogResult.OK)
            {
                mDoc.SetDirty();
                TVSettings.Instance.FNPRegexs = d.OutputRegularExpressions;
            }
        }

        private static string GetFolderForShow(ShowItem currentShow)
        {
            if (currentShow == null) return string.Empty;

            foreach (List<string> folders in currentShow.AllFolderLocations().Values)
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
            ProcessArgs();
        }

        private void uTorrentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uTorrent ut = new uTorrent(mDoc, setProgress);
            ut.ShowDialog();
            tabControl1.SelectedIndex = 1; // go to all-in-one tab
        }

        private void bnMyShowsCollapse_Click(object sender, EventArgs e)
        {
            MyShowTree.BeginUpdate();
            treeExpandCollapseToggle = !treeExpandCollapseToggle;
            if (treeExpandCollapseToggle)
                MyShowTree.CollapseAll();
            else
                MyShowTree.ExpandAll();

            MyShowTree.SelectedNode?.EnsureVisible();

            MyShowTree.EndUpdate();
        }

        private void UI_KeyDown(object sender, KeyEventArgs e)
        {
            int t = -1;
            if (e.Control && e.KeyCode == Keys.D1)
                t = 0;
            else if (e.Control && e.KeyCode == Keys.D2)
                t = 1;
            else if (e.Control && e.KeyCode == Keys.D3)
                t = 2;
            else if (e.Control && e.KeyCode == Keys.D4)
                t = 3;
            else if (e.Control && e.KeyCode == Keys.D5)
                t = 4;
            else if (e.Control && e.KeyCode == Keys.D6)
                t = 5;
            else if (e.Control && e.KeyCode == Keys.D7)
                t = 6;
            else if (e.Control && e.KeyCode == Keys.D8)
                t = 7;
            else if (e.Control && e.KeyCode == Keys.D9)
                t = 8;
            else if (e.Control && e.KeyCode == Keys.D0)
                t = 9;

            if (t >= 0 && t < tabControl1.TabCount)
            {
                tabControl1.SelectedIndex = t;
                e.Handled = true;
            }
        }

        private void bnActionCheck_Click(object sender, EventArgs e) => UiScan(null, false, TVSettings.ScanType.Full);

        public void QuickScan() => UiScan(null, true, TVSettings.ScanType.Quick);

        public void RecentScan() => UiScan(mDoc.Library.GetRecentShows(), true, TVSettings.ScanType.Recent);

        private void UiScan(List<ShowItem> shows, bool unattended, TVSettings.ScanType st)
        {
            Logger.Info("*******************************");
            string desc = unattended ? "unattended " : "";
            string showsdesc = shows?.Count > 0 ? shows.Count.ToString() : "all";
            string scantype = st.PrettyPrint();
            Logger.Info($"Starting {desc}{scantype} Scan for {showsdesc} shows...");

            mDoc.PreventAutoScan("Assessing New Shows");
            if (st != TVSettings.ScanType.SingleShow) GetNewShows(unattended);
            mDoc.AllowAutoScan();

            MoreBusy();
            mDoc.Scan(shows, unattended, st);
            LessBusy();

            FillMyShows(); // scanning can download more info to be displayed in my shows
            FillActionList();
        }

        private void GetNewShows(bool unattended)
        {
            //for each directory in settings directory
            //for each file in directory
            //for each saved show (order by recent)
            //does show match selected file?
            //if so add series to list of series scanned
            if (!TVSettings.Instance.AutoSearchForDownloadedFiles)
            {
                Logger.Info("Not looking for new shows as 'Auto-Add' is turned off");
                return;
            }

            //Dont support unattended mode
            if (unattended)
            {
                Logger.Info("Not looking for new shows as app is unattended");
                return;
            }

            List<string> possibleShowNames = new List<string>();

            foreach (string dirPath in TVSettings.Instance.DownloadFolders)
            {
                Logger.Info("Parsing {0} for new shows", dirPath);
                if (!Directory.Exists(dirPath)) continue;

                foreach (string filePath in Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories))
                {
                    if (!File.Exists(filePath)) continue;

                    FileInfo fi = new FileInfo(filePath);

                    if (FileHelper.IgnoreFile(fi)) continue;

                    if (!LookForSeries(fi.Name)) possibleShowNames.Add(fi.RemoveExtension() + ".");
                }
            }

            List<ShowItem> addedShows = new List<ShowItem>();

            foreach (string hint in possibleShowNames)
            {
                //MessageBox.Show($"Search for {hint}");
                //if hint doesn't match existing added shows
                if (LookForSeries(hint, addedShows))
                {
                    Logger.Info($"Ignoring {hint} as it matches existing shows.");
                    continue;
                }

                //If the hint contains certain terms then we'll ignore it
                if (IgnoreHint(hint))
                {
                    Logger.Info($"Ignoring {hint} as it is in the ignore list (from Settings).");
                    continue;
                }

                //If the hint contains certain terms then we'll ignore it
                if (TVSettings.Instance.IgnoredAutoAddHints.Contains(hint))
                {
                    Logger.Info($"Ignoring {hint} as it is in the list of ignored terms the user has selected to ignore from prior Auto Adds.");
                    continue;
                }

                //Remove any (nnnn) in the hint - probably a year
                string refinedHint = Regex.Replace(hint,@"\(\d{4}\)","");

                //Remove anything we can from hint to make it cleaner and hence more likely to match
                refinedHint = RemoveSeriesEpisodeIndicators(refinedHint);

                if (string.IsNullOrWhiteSpace(refinedHint))
                {
                    Logger.Info($"Ignoring {hint} as it refines to nothing.");
                    continue;
                }

                //If there are no LibraryFolders then we cant use the simplified UI
                if (TVSettings.Instance.LibraryFolders.Count == 0)
                {
                    MessageBox.Show(
                        "Please add some monitor (library) folders under 'Bulk Add Shows'to use the 'Auto Add' functionity (Alternatively you can turn it off in settings).",
                        "Can't Auto Add Show", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                Logger.Info("****************");
                Logger.Info("Auto Adding New Show");
                MoreBusy();

                TheTVDB.Instance.GetLock("AutoAddShow");
                //popup dialog
                AutoAddShow askForMatch = new AutoAddShow(refinedHint);

                DialogResult dr = askForMatch.ShowDialog();
                TheTVDB.Instance.Unlock("AutoAddShow");
                if (dr == DialogResult.OK)
                {
                    //If added add show to collection
                    addedShows.Add(askForMatch.ShowItem);
                }
                else if (dr == DialogResult.Abort)
                {
                    Logger.Info("Skippng Auto Add Process");
                    LessBusy();
                    break;
                }
                else if (dr == DialogResult.Ignore)
                {
                    Logger.Info($"Permenantly Ignoring 'Auto Add' for: {hint}");
                    TVSettings.Instance.IgnoredAutoAddHints.Add(hint);
                }
                else Logger.Info($"Cancelled Auto adding new show {hint}");

                LessBusy();
            }

            mDoc.Library.AddRange(addedShows);

            ShowAddedOrEdited(true);

            if (addedShows.Count <= 0) return;

            SelectShow(addedShows.Last());
            Logger.Info("Added new shows called: {0}", string.Join(",", addedShows.Select(s => s.ShowName)));
        }

        private static bool IgnoreHint(string hint)
        {
            return TVSettings.Instance.AutoAddMovieTermsArray.Any(term =>
                hint.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        private string RemoveSeriesEpisodeIndicators(string hint)
        {
            string hint2 = Helpers.RemoveDiacritics(hint);
            hint2 = RemoveSe(hint2);
            hint2 = hint2.ToLower();
            hint2 = hint2.Replace("'", "");
            hint2 = hint2.Replace("&", "and");
            hint2 = Regex.Replace(hint2, "[_\\W]+", " ");
            foreach (string term in TVSettings.Instance.AutoAddIgnoreSuffixesArray)
            {
                hint2 = hint2.RemoveAfter(term);
            }

            foreach (string seasonWord in mDoc.Library.SeasonWords())
            {
                hint2 = hint2.RemoveAfter(seasonWord);
            }

            hint2 = hint2.Trim();
            return hint2;
        }

        private static string RemoveSe(string hint)
        {
            foreach (FilenameProcessorRE re in TVSettings.Instance.FNPRegexs)
            {
                if (!re.Enabled)
                    continue;

                try
                {
                    Match m = Regex.Match(hint, re.RegExpression, RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        if (!int.TryParse(m.Groups["s"].ToString(), out int seas))
                            seas = -1;

                        if (!int.TryParse(m.Groups["e"].ToString(), out int ep))
                            ep = -1;

                        int p = Math.Min(m.Groups["s"].Index, m.Groups["e"].Index);
                        int p2 = Math.Min(p, hint.IndexOf(m.Groups.SyncRoot.ToString(), StringComparison.Ordinal));

                        if (seas != -1 && ep != -1) return hint.Remove(p2 != -1 ? p2 : p);
                    }
                }
                catch (FormatException)
                {
                }
                catch (ArgumentException)
                {
                }
            }

            return hint;
        }

        private bool LookForSeries(string name) => LookForSeries(name, mDoc.Library.Values);

        private static bool LookForSeries(string test, IEnumerable<ShowItem> shows)
        {
            return shows.Any(si => si.GetSimplifiedPossibleShowNames()
                .Any(name => FileHelper.SimplifyAndCheckFilename(test, name)));
        }

        private ListViewItem LviForItem(Item item)
        {
            Item sli = item;
            if (sli == null)
            {
                return new ListViewItem();
            }

            ListViewItem lvi = sli.ScanListViewItem;
            lvi.Group = lvAction.Groups[sli.ScanListViewGroup];

            if (sli.IconNumber != -1)
                lvi.ImageIndex = sli.IconNumber;

            lvi.Checked = true;
            lvi.Tag = sli;

            Debug.Assert(lvi.SubItems.Count <= lvAction.Columns.Count - 1);

            while (lvi.SubItems.Count < lvAction.Columns.Count - 1)
                lvi.SubItems.Add(""); // pad our way to the error column

            if (item is Action act && act.Error)
            {
                lvi.BackColor = Helpers.WarningColor();
                lvi.SubItems.Add(act.ErrorText); // error text
            }
            else
                lvi.SubItems.Add("");

            if (!(item is Action))
                lvi.Checked = false;

            Debug.Assert(lvi.SubItems.Count == lvAction.Columns.Count);

            return lvi;
        }

        private void lvAction_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            Item item = mDoc.TheActionList[e.ItemIndex];
            e.Item = LviForItem(item);
        }

        private void FillActionList()
        {
            internalCheckChange = true;

            // Save where the list is currently scrolled too
            int currentTop = lvAction.GetScrollVerticalPos();

            if (lvAction.VirtualMode)
                lvAction.VirtualListSize = mDoc.TheActionList.Count;
            else
            {
                lvAction.BeginUpdate();
                lvAction.Items.Clear();

                foreach (Item item in mDoc.TheActionList)
                {
                    ListViewItem lvi = LviForItem(item);
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
                    missingCount++;
                else if (action is ActionCopyMoveRename cmr)
                {
                    ActionCopyMoveRename.Op op = cmr.Operation;
                    if (op == ActionCopyMoveRename.Op.copy)
                    {
                        copyCount++;
                        if (cmr.From.Exists)
                            copySize += cmr.From.Length;
                    }
                    else if (op == ActionCopyMoveRename.Op.move)
                    {
                        moveCount++;
                        if (cmr.From.Exists)
                            moveSize += cmr.From.Length;
                    }
                    else if (op == ActionCopyMoveRename.Op.rename)
                        renameCount++;
                }
                else if (action is ActionDownloadImage)
                    downloadCount++;
                else if (action is ActionRSS)
                    rssCount++;
                else if (action is ActionWriteMetadata) // base interface that all metadata actions are derived from
                    metaCount++;
                else if (action is ActionDateTouch)
                    fileMetaCount++;
                else if (action is ItemDownloading)
                    dlCount++;
                else if (action is ActionDeleteFile || action is ActionDeleteDirectory)
                    removeCount++;
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

        private static string HeaderName(string name, int number) => $"{name} ({PrettyPrint(number)})";

        private static string PrettyPrint(int number) => number + " " + number.ItemItems();

        private static string HeaderName(string name, int number, long filesize)
        {
            return $"{name} ({PrettyPrint(number)}, {filesize.GBMB(1)})";
        }

        private void bnActionAction_Click(object sender, EventArgs e) => ProcessAll();

        public void ProcessAll() => ActionAction(true);

        private void ActionAction(bool checkedNotSelected)
        {
            mDoc.PreventAutoScan("Action Selected Items");
            LVResults lvr = new LVResults(lvAction, checkedNotSelected);
            mDoc.DoActions(lvr.FlatList);
            // remove items from master list, unless it had an error
            foreach (Item i2 in new LVResults(lvAction, checkedNotSelected).FlatList)
            {
                if (i2 != null && !lvr.FlatList.Contains(i2))
                    mDoc.TheActionList.Remove(i2);
            }

            FillActionList();
            RefreshWTW(false);
            mDoc.AllowAutoScan();
        }

        private void Revert(bool checkedNotSelected)
        {
            foreach (Item item in new LVResults(lvAction, checkedNotSelected).FlatList)
            {
                ActionCopyMoveRename i2 = (ActionCopyMoveRename) item;
                ItemMissing m2 = i2.UndoItemMissing;

                if (m2 == null) continue;

                mDoc.TheActionList.Add(m2);
                mDoc.TheActionList.Remove(i2);

                List<Item> toRemove = new List<Item>();
                //We can remove any CopyMoveActions that are closely related too
                foreach (Item a in mDoc.TheActionList)
                {
                    if (a is ItemMissing) continue;

                    if (a is ActionCopyMoveRename i1)
                    {
                        if (i1.From.RemoveExtension(true).StartsWith(i2.From.RemoveExtension(true)))
                        {
                            toRemove.Add(i1);
                        }
                    }
                    else if (a is Item ad)
                    {
                        if (ad.Episode?.AppropriateEpNum == i2.Episode?.AppropriateEpNum &&
                            ad.Episode?.AppropriateSeasonNumber == i2.Episode?.AppropriateSeasonNumber)
                            toRemove.Add(a);
                    }
                }

                //Remove all similar items
                foreach (Item i in toRemove) mDoc.TheActionList.Remove(i);
            }

            FillActionList();
            RefreshWTW(false);
        }

        private void folderMonitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BulkAddManager bam = new BulkAddManager(mDoc);
            FolderMonitor fm = new FolderMonitor(mDoc, bam);
            fm.ShowDialog();
            FillMyShows();
        }

        private void torrentMatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TorrentMatch tm = new TorrentMatch(mDoc, setProgress);
            tm.ShowDialog();
            FillActionList();
        }

        private void bnActionWhichSearch_Click(object sender, EventArgs e) => ChooseSiteMenu(0);

        private void lvAction_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            // build the right click menu for the _selected_ items, and types of items
            LVResults lvr = new LVResults(lvAction, false);

            if (lvr.Count == 0)
                return; // nothing selected

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
                AddRcMenuItem("Search", RightClickCommands.kBtSearchFor);

                if (lvr.Count == 1) // only one selected
                {
                    AddRcMenuItem("Browse For...", RightClickCommands.kActionBrowseForFile);
                }
            }

            if (lvr.CopyMove.Count > 0)
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

            LVResults lvr = new LVResults(lvAction, false);

            if (lvr.Count == 0)
            {
                // disable everything
                bnActionBTSearch.Enabled = false;
                return;
            }

            bnActionBTSearch.Enabled = lvr.Download.Count <= 0;

            mLastShowsClicked = null;
            mLastEpClicked = null;
            mLastSeasonClicked = null;
            mLastActionsClicked = null;

            showRightClickMenu.Items.Clear();
            mFoldersToOpen = new List<string>();
            mLastFl = new List<FileInfo>();

            mLastActionsClicked = new ItemList();

            foreach (Item ai in lvr.FlatList)
                mLastActionsClicked.Add(ai);

            if (lvr.Count != 1 || lvAction.FocusedItem?.Tag == null) return;

            if (!(lvAction.FocusedItem.Tag is Item action)) return;

            mLastEpClicked = action.Episode;
            if (action.Episode != null)
            {
                mLastSeasonClicked = action.Episode.AppropriateSeason;
                mLastShowsClicked = new List<ShowItem> {action.Episode.Show};
            }
            else
            {
                mLastSeasonClicked = null;
                mLastShowsClicked = null;
            }

            if (mLastEpClicked != null && TVSettings.Instance.AutoSelectShowInMyShows)
                GotoEpguideFor(mLastEpClicked, false);
        }

        private void ActionDeleteSelected()
        {
            ListView.SelectedListViewItemCollection sel = lvAction.SelectedItems;
            foreach (ListViewItem lvi in sel)
                mDoc.TheActionList.Remove((Item) lvi.Tag);

            FillActionList();
        }

        private void lvAction_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                ActionDeleteSelected();
        }

        private void cbActionIgnore_Click(object sender, EventArgs e) => IgnoreSelected();

        private void UpdateActionCheckboxes()
        {
            if (internalCheckChange)
                return;

            LVResults all = new LVResults(lvAction, LVResults.WhichResults.All);
            LVResults chk = new LVResults(lvAction, LVResults.WhichResults.Checked);

            if (chk.Rename.Count == 0)
                cbRename.CheckState = CheckState.Unchecked;
            else
                cbRename.CheckState = chk.Rename.Count == all.Rename.Count
                    ? CheckState.Checked
                    : CheckState.Indeterminate;

            if (chk.CopyMove.Count == 0)
                cbCopyMove.CheckState = CheckState.Unchecked;
            else
                cbCopyMove.CheckState = chk.CopyMove.Count == all.CopyMove.Count
                    ? CheckState.Checked
                    : CheckState.Indeterminate;

            if (chk.RSS.Count == 0)
                cbRSS.CheckState = CheckState.Unchecked;
            else
                cbRSS.CheckState =
                    chk.RSS.Count == all.RSS.Count ? CheckState.Checked : CheckState.Indeterminate;

            if (chk.Download.Count == 0)
                cbDownload.CheckState = CheckState.Unchecked;
            else
                cbDownload.CheckState = chk.Download.Count == all.Download.Count
                    ? CheckState.Checked
                    : CheckState.Indeterminate;

            if (chk.NFO.Count == 0)
                cbNFO.CheckState = CheckState.Unchecked;
            else
                cbNFO.CheckState =
                    chk.NFO.Count == all.NFO.Count ? CheckState.Checked : CheckState.Indeterminate;

            if (chk.PyTivoMeta.Count == 0)
                cbMeta.CheckState = CheckState.Unchecked;
            else
                cbMeta.CheckState = chk.PyTivoMeta.Count == all.PyTivoMeta.Count
                    ? CheckState.Checked
                    : CheckState.Indeterminate;

            int total1 = all.Rename.Count + all.CopyMove.Count + all.RSS.Count + all.Download.Count + all.NFO.Count +
                         all.PyTivoMeta.Count;

            int total2 = chk.Rename.Count + chk.CopyMove.Count + chk.RSS.Count + chk.Download.Count + chk.NFO.Count +
                         chk.PyTivoMeta.Count;

            if (total2 == 0)
                cbAll.CheckState = CheckState.Unchecked;
            else
                cbAll.CheckState = total2 == total1 ? CheckState.Checked : CheckState.Indeterminate;
        }

        private void cbActionAllNone_Click(object sender, EventArgs e)
        {
            CheckState cs = cbAll.CheckState;
            if (cs == CheckState.Indeterminate)
            {
                cbAll.CheckState = CheckState.Unchecked;
                cs = CheckState.Unchecked;
            }

            internalCheckChange = true;
            foreach (ListViewItem lvi in lvAction.Items)
                lvi.Checked = cs == CheckState.Checked;

            internalCheckChange = false;
            UpdateActionCheckboxes();
        }

        private void cbActionRename_Click(object sender, EventArgs e)
        {
            CheckState cs = cbRename.CheckState;
            if (cs == CheckState.Indeterminate)
            {
                cbRename.CheckState = CheckState.Unchecked;
                cs = CheckState.Unchecked;
            }

            internalCheckChange = true;
            foreach (ListViewItem lvi in lvAction.Items)
            {
                Item i = (Item) lvi.Tag;
                if (i is ActionCopyMoveRename rename && rename.Operation == ActionCopyMoveRename.Op.rename)
                    lvi.Checked = cs == CheckState.Checked;
            }

            internalCheckChange = false;
            UpdateActionCheckboxes();
        }

        private void cbActionCopyMove_Click(object sender, EventArgs e)
        {
            CheckState cs = cbCopyMove.CheckState;
            if (cs == CheckState.Indeterminate)
            {
                cbCopyMove.CheckState = CheckState.Unchecked;
                cs = CheckState.Unchecked;
            }

            internalCheckChange = true;
            foreach (ListViewItem lvi in lvAction.Items)
            {
                Item i = (Item) lvi.Tag;
                if (i is ActionCopyMoveRename copymove && copymove.Operation != ActionCopyMoveRename.Op.rename)
                    lvi.Checked = cs == CheckState.Checked;
            }

            internalCheckChange = false;
            UpdateActionCheckboxes();
        }

        private void cbActionNFO_Click(object sender, EventArgs e)
        {
            CheckState cs = cbNFO.CheckState;
            if (cs == CheckState.Indeterminate)
            {
                cbNFO.CheckState = CheckState.Unchecked;
                cs = CheckState.Unchecked;
            }

            internalCheckChange = true;
            foreach (ListViewItem lvi in lvAction.Items)
            {
                Item i = (Item) lvi.Tag;
                if (i is ActionNfo)
                    lvi.Checked = cs == CheckState.Checked;
            }

            internalCheckChange = false;
            UpdateActionCheckboxes();
        }

        private void cbActionPyTivoMeta_Click(object sender, EventArgs e)
        {
            CheckState cs = cbMeta.CheckState;
            if (cs == CheckState.Indeterminate)
            {
                cbMeta.CheckState = CheckState.Unchecked;
                cs = CheckState.Unchecked;
            }

            internalCheckChange = true;
            foreach (ListViewItem lvi in lvAction.Items)
            {
                Item i = (Item) lvi.Tag;
                if (i is ActionPyTivoMeta)
                    lvi.Checked = cs == CheckState.Checked;
            }

            internalCheckChange = false;
            UpdateActionCheckboxes();
        }

        private void cbActionRSS_Click(object sender, EventArgs e)
        {
            CheckState cs = cbRSS.CheckState;
            if (cs == CheckState.Indeterminate)
            {
                cbRSS.CheckState = CheckState.Unchecked;
                cs = CheckState.Unchecked;
            }

            internalCheckChange = true;
            foreach (ListViewItem lvi in lvAction.Items)
            {
                Item i = (Item) lvi.Tag;
                if (i is ActionRSS)
                    lvi.Checked = cs == CheckState.Checked;
            }

            internalCheckChange = false;
            UpdateActionCheckboxes();
        }

        private void cbActionDownloads_Click(object sender, EventArgs e)
        {
            CheckState cs = cbDownload.CheckState;
            if (cs == CheckState.Indeterminate)
            {
                cbDownload.CheckState = CheckState.Unchecked;
                cs = CheckState.Unchecked;
            }

            internalCheckChange = true;
            foreach (ListViewItem lvi in lvAction.Items)
            {
                Item i = (Item) lvi.Tag;
                if (i is ActionDownloadImage)
                    lvi.Checked = cs == CheckState.Checked;
            }

            internalCheckChange = false;
            UpdateActionCheckboxes();
        }

        private void lvAction_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.Index < 0 || e.Index > lvAction.Items.Count)
                return;

            Item action = (Item) lvAction.Items[e.Index].Tag;
            if (action != null && (action is ItemMissing || action is ItemDownloading))
                e.NewValue = CheckState.Unchecked;
        }

        private void bnActionOptions_Click(object sender, EventArgs e) => DoPrefs(true);

        private void lvAction_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // double-click on an item will search for missing, do nothing (for now) for anything else
            foreach (ItemMissing miss in new LVResults(lvAction, false).Missing)
            {
                if (miss.Episode != null)
                    TVDoc.SearchForEpisode(miss.Episode);
            }
        }

        private void bnActionBTSearch_Click(object sender, EventArgs e)
        {
            LVResults lvr = new LVResults(lvAction, false);

            if (lvr.Count == 0)
                return;

            foreach (Item i in lvr.FlatList)
            {
                if (i?.Episode != null)
                    TVDoc.SearchForEpisode(i.Episode);
            }
        }

        private void bnRemoveSel_Click(object sender, EventArgs e) => ActionDeleteSelected();

        private void IgnoreSelected()
        {
            LVResults lvr = new LVResults(lvAction, false);
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
                FillActionList();
            }
        }

        private void ignoreListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IgnoreEdit ie = new IgnoreEdit(mDoc);
            ie.ShowDialog();
        }

        private async void showSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSummary f = new ShowSummary(mDoc);
            await Task.Run(() => f.GenerateData());
            f.PopulateGrid();
            f.Show();
        }

        private void lvAction_ItemChecked(object sender, ItemCheckedEventArgs e) => UpdateActionCheckboxes();

        private void bnHideHTMLPanel_Click(object sender, EventArgs e)
        {
            if (splitContainer1.Panel2Collapsed)
            {
                splitContainer1.Panel2Collapsed = false;
                bnHideHTMLPanel.ImageKey = "FillRight.bmp";
            }
            else
            {
                splitContainer1.Panel2Collapsed = true;
                bnHideHTMLPanel.ImageKey = "FillLeft.bmp";
            }
        }

        private void bnActionRecentCheck_Click(object sender, EventArgs e) =>
            UiScan(null, false, TVSettings.ScanType.Recent);

        private void btnActionQuickScan_Click(object sender, EventArgs e) =>
            UiScan(null, false, TVSettings.ScanType.Quick);

        private void btnFilter_Click(object sender, EventArgs e)
        {
            Filters filters = new Filters(mDoc);
            DialogResult res = filters.ShowDialog();
            if (res == DialogResult.OK)
            {
                FillMyShows();
            }
        }

        private void lvAction_DragDrop(object sender, DragEventArgs e)
        {
            // Get a list of filenames being dragged
            string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop, false);

            // Establish item in list being dragged to, and exit if no item matched
            Point localPoint = lvAction.PointToClient(new Point(e.X, e.Y));
            ListViewItem lvi = lvAction.GetItemAt(localPoint.X, localPoint.Y);
            if (lvi == null) return;

            // Check at least one file was being dragged, and that dragged-to item is a "Missing Item" item.
            if (files.Length > 0 & lvi.Tag is ItemMissing)
            {
                // Only want the first file if multiple files were dragged across.
                FileInfo from = new FileInfo(files[0]);
                ItemMissing mi = (ItemMissing) lvi.Tag;
                FileInfo to = new FileInfo(mi.TheFileNoExt + from.Extension);

                mDoc.TheActionList.Add(
                    new ActionCopyMoveRename(
                        TVSettings.Instance.LeaveOriginals
                            ? ActionCopyMoveRename.Op.copy
                            : ActionCopyMoveRename.Op.move, from, to
                        , mi.Episode, TVSettings.Instance.Tidyup, mi));

                // and remove old Missing item
                mDoc.TheActionList.Remove(mi);
                DownloadIdentifiersController di = new DownloadIdentifiersController();

                // if we're copying/moving a file across, we might also want to make a thumbnail or NFO for it
                mDoc.TheActionList.Add(di.ProcessEpisode(mi.Episode, to));
                FillActionList();
            }
        }

        private void lvAction_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
            Point localPoint = lvAction.PointToClient(new Point(e.X, e.Y));
            ListViewItem lvi = lvAction.GetItemAt(localPoint.X, localPoint.Y);
            // If we're not draging over a "ItemMissing" entry, or if we're not dragging a list of files, then change the DragDropEffect
            if (!(lvi?.Tag is ItemMissing) || !e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.None;
        }

        private void filterTextBox_TextChanged(object sender, EventArgs e) => FillMyShows();

        private void filterTextBox_SizeChanged(object sender, EventArgs e)
        {
            // MAH: move the "Clear" button in the Filter Text Box
            if (filterTextBox.Controls.ContainsKey("Clear"))
            {
                Control filterButton = filterTextBox.Controls["Clear"];
                filterButton.Location = new Point(filterTextBox.ClientSize.Width - filterButton.Width,
                    (filterTextBox.ClientSize.Height - 16) / 2 + 1);

                // Send EM_SETMARGINS to prevent text from disappearing underneath the button
                NativeMethods.SendMessage(filterTextBox.Handle, 0xd3, (IntPtr) 2, (IntPtr) (filterButton.Width << 16));
            }
        }

        private void visitSupportForumToolStripMenuItem_Click(object sender, EventArgs e)
            => Helpers.SysOpen("https://groups.google.com/forum/#!forum/tvrename");

        public void Quit() => Close();

        private async void checkForNewVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task<UpdateVersion> uv = VersionUpdater.CheckForUpdatesAsync();
            NotifyUpdates(await uv, true);
        }

        private void NotifyUpdates(UpdateVersion update, bool showNoUpdateRequiredDialog, bool inSilentMode = false)
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

            if (inSilentMode) return;

            UpdateNotification unForm = new UpdateNotification(update);
            unForm.ShowDialog();
            btnUpdateAvailable.Visible = true;
        }

        private void duplicateFinderLOGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<PossibleDuplicateEpisode> x = mDoc.FindDoubleEps();
            DupEpFinder form = new DupEpFinder(x, mDoc, this);
            form.ShowDialog();
        }

        private async void btnUpdateAvailable_Click(object sender, EventArgs e)
        {
            btnUpdateAvailable.Visible = false;
            Task<UpdateVersion> uv = VersionUpdater.CheckForUpdatesAsync();
            NotifyUpdates(await uv, true);
        }

        private void tmrPeriodicScan_Tick(object sender, EventArgs e) => RunAutoScan("Periodic Scan");

        private void RunAutoScan(string scanType)
        {
            //We only wish to do a scan now if we are not already undertaking one
            if (mDoc.AutoScanCanRun())
            {
                Logger.Info("*******************************");
                Logger.Info(scanType + " fired");
                UiScan(null, true, TVSettings.Instance.MonitoredFoldersScanType);
                ProcessAll();
                Logger.Info(scanType + " complete");
            }
            else
            {
                Logger.Info(scanType + " cancelled as the system is already busy");
            }
        }

        private void timezoneInconsistencyLOGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TimeZoneTracker results = new TimeZoneTracker();
            foreach (ShowItem si in mDoc.Library.GetShowItems())
            {
                SeriesInfo ser = si.TheSeries();

                //si.ShowTimeZone = TimeZone.TimeZoneForNetwork(ser.getNetwork());

                results.Add(ser.GetNetwork(), si.ShowTimeZone, si.ShowName);
            }

            Logger.Info(results.PrintVersion());
        }

        private class TimeZoneTracker
        {
            private readonly Dictionary<string, Dictionary<string, List<string>>> tzt =
                new Dictionary<string, Dictionary<string, List<string>>>();

            internal void Add(string network, string timezone, string show)
            {
                if (!tzt.ContainsKey(network)) tzt.Add(network, new Dictionary<string, List<string>>());
                Dictionary<string, List<string>> snet = tzt[network];

                if (!snet.ContainsKey(timezone)) snet.Add(timezone, new List<string>());
                List<string> snettz = snet[timezone];

                snettz.Add(show);
            }

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
                        sb.AppendLine($"{kvp.Key,-30}{kvp2.Key,-30}{string.Join(",", kvp2.Value)}");
                    }
                }

                return sb.ToString();
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aeCollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var Frm = new AddEditCollection(mDoc);
            RemoveCollectionsMenu();
            BuildCollectionsMenu();
        }
    }
}
