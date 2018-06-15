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
        None = 0,
        kEpisodeGuideForShow = 1,
        kVisitTVDBEpisode,
        kVisitTVDBSeason,
        kVisitTVDBSeries,
        kScanSpecificSeries,
        kWhenToWatchSeries,
        kForceRefreshSeries,
        kBTSearchFor,
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
        #region Delegates

        public delegate void AutoFolderMonitorDelegate();

        #endregion

        private int Busy;
        private TVDoc mDoc;
        private bool InternalCheckChange;
        private int LastDLRemaining;

        public AutoFolderMonitorDelegate AFMFullScan;
        public AutoFolderMonitorDelegate AFMRecentScan;
        public AutoFolderMonitorDelegate AFMQuickScan;
        public AutoFolderMonitorDelegate AFMDoAll;

        private readonly SetProgressDelegate SetProgress;
        private MyListView lvAction;
        private List<string> mFoldersToOpen;
        private int mInternalChange;
        private List<FileInfo> mLastFL;
        private Point mLastNonMaximizedLocation;
        private Size mLastNonMaximizedSize;
        private readonly AutoFolderMonitor mAutoFolderMonitor;
        private bool treeExpandCollapseToggle = true;

        private ItemList mLastActionsClicked;
        private ProcessedEpisode mLastEpClicked;
        private readonly string mLastFolderClicked;
        private Season mLastSeasonClicked;
        private List<ShowItem> mLastShowsClicked;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public UI(TVDoc doc, TVRenameSplash splash, bool showUI)
        {
            mDoc = doc;

            Busy = 0;
            mLastEpClicked = null;
            mLastFolderClicked = null;
            mLastSeasonClicked = null;
            mLastShowsClicked = null;
            mLastActionsClicked = null;

            mInternalChange = 0;
            mFoldersToOpen = new List<string>();

            InternalCheckChange = false;

            InitializeComponent();

            SetupIPC();

            try
            {
                LoadLayoutXML();
            }
            catch (Exception e)
            {
                // silently fail, doesn't matter too much
                logger.Info(e, "Error loading layout XML");
            }

            SetProgress += SetProgressActual;

            lvWhenToWatch.ListViewItemSorter = new DateSorterWTW();

            if (mDoc.Args.Hide)
            {
                WindowState = FormWindowState.Minimized;
                Visible = false;
                Hide();
            }

            Text = Text + " " + Helpers.DisplayVersion;

            UpdateSplashStatus(splash, "Filling Shows");
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

            int t = TVSettings.Instance.StartupTab;
            if (t < tabControl1.TabCount)
                tabControl1.SelectedIndex = TVSettings.Instance.StartupTab;

            tabControl1_SelectedIndexChanged(null, null);
            UpdateSplashStatus(splash, "Creating Monitors");

            mAutoFolderMonitor = new AutoFolderMonitor(mDoc, this);

            tmrPeriodicScan.Interval = TVSettings.Instance.PeriodicCheckPeriod();

            UpdateSplashStatus(splash, "Update Available?");
            Task task = Task.Run(async () => { await CheckUpdatesOnStartup(showUI); });
            task.Wait();

            UpdateSplashStatus(splash, "Starting Monitor");
            if (TVSettings.Instance.MonitorFolders)
                mAutoFolderMonitor.StartMonitor();

            tmrPeriodicScan.Enabled = TVSettings.Instance.RunPeriodicCheck();

            UpdateSplashStatus(splash, "Running autoscan");
            //splash.Close();
        }

        private async Task CheckUpdatesOnStartup(bool showUI)
        {
            Task<UpdateVersion> tuv = VersionUpdater.CheckForUpdatesAsync();
            NotifyUpdates(await tuv, false, !showUI);
        }

        private static void UpdateSplashStatus(TVRenameSplash splashScreen, string text)
        {
            splashScreen.Invoke((System.Action) delegate { splashScreen.UpdateStatus(text); });
        }

        private static void UpdateSplashPercent(TVRenameSplash splashScreen, int num)
        {
            splashScreen.Invoke((System.Action) delegate { splashScreen.UpdateProgress(num); });
        }

        private void ClearInfoWindows() => ClearInfoWindows("");

        private void ClearInfoWindows(string defaultText)
        {
            SetHTMLbody(defaultText, EpGuidePath(), epGuideHTML);
            SetHTMLbody(defaultText, ImagesGuidePath(), webBrowserImages);
        }

        private static int BGDLLongInterval() => 1000 * 60 * 60; // one hour

        private void MoreBusy() => Interlocked.Increment(ref Busy);

        private void LessBusy() => Interlocked.Decrement(ref Busy);

        private void SetupIPC()
        {
            AFMFullScan += Scan;
            AFMQuickScan += QuickScan;
            AFMRecentScan += RecentScan;
            AFMDoAll += ProcessAll;
        }

        public void SetProgressActual(int p)
        {
            if (p < 0)
                p = 0;
            else if (p > 100)
                p = 100;

            pbProgressBarx.Value = p;
            pbProgressBarx.Update();
        }

        public void ProcessArgs()
        {
            // TODO: Unify command line handling between here and in Program.cs

            if (mDoc.Args.Scan)
                UIScan(null, true, TVSettings.ScanType.Full);

            if (mDoc.Args.QuickScan)
                UIScan(null, true, TVSettings.ScanType.Quick);

            if (mDoc.Args.RecentScan)
                UIScan(null, true, TVSettings.ScanType.Recent);

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

            FillEpGuideHTML();
        }

        private static void visitWebsiteToolStripMenuItem_Click(object sender, EventArgs eventArgs) =>
            Helpers.SysOpen("http://tvrename.com");

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Close();

        private static bool UseCustom(ListView view)
        {
            foreach (ListViewItem lvi in view.SelectedItems)
            {
                if (!(lvi.Tag is ProcessedEpisode pe)) continue;
                if (!pe.SI.UseCustomSearchURL) continue;
                if (string.IsNullOrWhiteSpace(pe.SI.CustomSearchURL)) continue;

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
                ((filterTextBox.ClientSize.Height - 16) / 2) + 1);

            filterButton.Click += filterButton_Click;
            filterTextBox.Controls.Add(filterButton);
            // Send EM_SETMARGINS to prevent text from disappearing underneath the button
            NativeMethods.SendMessage(filterTextBox.Handle, 0xd3, (IntPtr) 2, (IntPtr) (filterButton.Width << 16));

            betaToolsToolStripMenuItem.Visible = TVSettings.Instance.IncludeBetaUpdates();

            Show();
            UI_LocationChanged(null, null);
            UI_SizeChanged(null, null);

            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.SetToolTip(btnActionQuickScan,
                "Scan shows with missing recent aired episodes and and shows that match files in the search folders");

            ToolTip1.SetToolTip(bnActionRecentCheck, "Scan shows with recent aired episodes");
            ToolTip1.SetToolTip(bnActionCheck, "Scan all shows");

            backgroundDownloadToolStripMenuItem.Checked = TVSettings.Instance.BGDownload;
            offlineOperationToolStripMenuItem.Checked = TVSettings.Instance.OfflineMode;
            BGDownloadTimer.Interval = 10000; // first time
            if (TVSettings.Instance.BGDownload)
                BGDownloadTimer.Start();

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
            }

            return null;
        }

        private void flushCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                FillEpGuideHTML();
                FillWhenToWatchList();
                backgroundDownloadToolStripMenuItem_Click(sender, e);
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

        private bool LoadLayoutXML()
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
                                int x = int.Parse(reader.GetAttribute("Width"));
                                int y = int.Parse(reader.GetAttribute("Height"));
                                Size = new Size(x, y);
                                reader.Read();
                            }
                            else if (reader.Name == "Location")
                            {
                                int x = int.Parse(reader.GetAttribute("X"));
                                int y = int.Parse(reader.GetAttribute("Y"));
                                Location = new Point(x, y);
                                reader.Read();
                            }
                            else if (reader.Name == "Maximized")
                                WindowState = (reader.ReadElementContentAsBoolean()
                                    ? FormWindowState.Maximized
                                    : FormWindowState.Normal);
                            else
                                reader.ReadOuterXml();
                        }

                        reader.Read();
                    } // window
                    else if (reader.Name == "ColumnWidths")
                        ok = LoadWidths(reader) && ok;
                    else if (reader.Name == "Splitter")
                    {
                        splitContainer1.SplitterDistance = int.Parse(reader.GetAttribute("Distance"));
                        splitContainer1.Panel2Collapsed = bool.Parse(reader.GetAttribute("HTMLCollapsed"));
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

        private bool SaveLayoutXML()
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

                WriteColWidthsXML("WhenToWatch", writer);
                WriteColWidthsXML("AllInOne", writer);

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

        private void WriteColWidthsXML(string thingName, XmlWriter writer)
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
                        mDoc.WriteXMLSettings();
                    else if (res == DialogResult.Cancel)
                        e.Cancel = true;
                    else if (res == DialogResult.No)
                    {
                    }
                }

                if (!e.Cancel)
                {
                    SaveLayoutXML();
                    mDoc.TidyTVDB();
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
                ToolStripMenuItem tsi = new ToolStripMenuItem(TVDoc.GetSearchers().Name(i));
                tsi.Tag = i;
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
            ShowItem currentSI = TreeNodeToShowItem(MyShowTree.SelectedNode);

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
                if (filter.filter(si)
                    & (string.IsNullOrEmpty(filterTextBox.Text) | si.getSimplifiedPossibleShowNames().Any(name =>
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
            else if (currentSI != null)
                SelectShow(currentSI);

            MyShowTree.EndUpdate();
        }

        private static string QuickStartGuide() => "https://www.tvrename.com/manual/quickstart/";

        private void ShowQuickStartGuide()
        {
            tabControl1.SelectTab(tbMyShows);
            epGuideHTML.Navigate(QuickStartGuide());
            webBrowserImages.Navigate(QuickStartGuide());
        }

        private void FillEpGuideHTML()
        {
            if (MyShowTree.Nodes.Count == 0)
                ShowQuickStartGuide();
            else
            {
                TreeNode n = MyShowTree.SelectedNode;
                FillEpGuideHTML(n);
            }
        }

        private ShowItem TreeNodeToShowItem(TreeNode n)
        {
            if (n == null)
                return null;

            if (n.Tag is ShowItem si)
                return si;

            if (n.Tag is ProcessedEpisode pe)
                return pe.SI;

            if (n.Tag is Season seas)
            {
                if (seas.Episodes.Count == 0) return null;

                return mDoc.Library.ShowItem(seas.TheSeries.TVDBCode);
            }

            return null;
        }

        private static Season TreeNodeToSeason(TreeNode n)
        {
            Season seas = n?.Tag as Season;
            return seas;
        }

        private void FillEpGuideHTML(TreeNode n)
        {
            if (n == null)
            {
                FillEpGuideHTML(null, -1);
                return;
            }

            if (n.Tag is ProcessedEpisode pe)
            {
                FillEpGuideHTML(pe.SI, pe.AppropriateSeasonNumber);
                return;
            }

            Season seas = TreeNodeToSeason(n);
            if (seas != null)
            {
                // we have a TVDB season, but need to find the equiavlent one in our local processed episode collection
                if (seas.Episodes.Count > 0)
                {
                    int tvdbcode = seas.TheSeries.TVDBCode;
                    foreach (ShowItem si in mDoc.Library.Values)
                    {
                        if (si.TVDBCode == tvdbcode)
                        {
                            FillEpGuideHTML(si, seas.SeasonNumber);
                            return;
                        }
                    }
                }

                FillEpGuideHTML(null, -1);
                return;
            }

            FillEpGuideHTML(TreeNodeToShowItem(n), -1);
        }

        private void FillEpGuideHTML(ShowItem si, int snum)
        {
            if (tabControl1.SelectedTab != tbMyShows)
                return;

            if (si == null)
            {
                ClearInfoWindows();
                return;
            }

            TheTVDB.Instance.GetLock("FillEpGuideHTML");

            SeriesInfo ser = TheTVDB.Instance.GetSeries(si.TVDBCode);

            if (ser == null)
            {
                ClearInfoWindows("Not downloaded, or not available");
                TheTVDB.Instance.Unlock("FillEpGuideHTML");
                return;
            }

            string infoPaneBody;
            string imagesPaneBody;

            if (si.DVDOrder && (snum >= 0) && (ser.DVDSeasons.ContainsKey(snum)))
            {
                infoPaneBody = GetSeasonHtmlOverview(si, ser, snum);
                imagesPaneBody = GetSeasonImagesHTMLOverview(si, ser, snum);
            }
            else if (!si.DVDOrder && (snum >= 0) && (ser.AiredSeasons.ContainsKey(snum)))
            {
                infoPaneBody = GetSeasonHtmlOverview(si, ser, snum);
                imagesPaneBody = GetSeasonImagesHTMLOverview(si, ser, snum);
            }
            else
            {
                // no epnum specified, just show an overview
                infoPaneBody = GetShowHTMLOverview(si, ser);
                imagesPaneBody = GetShowImagesHTMLOverview(si, ser);
            }

            TheTVDB.Instance.Unlock("FillEpGuideHTML");
            SetHTMLbody(infoPaneBody, EpGuidePath(), epGuideHTML);
            SetHTMLbody(imagesPaneBody, ImagesGuidePath(), webBrowserImages);
        }

        private static string GetSeasonImagesHTMLOverview(ShowItem si, SeriesInfo ser, int snum)
        {
            string body = "";

            Season s = si.DVDOrder ? ser.DVDSeasons[snum] : ser.AiredSeasons[snum];

            List<ProcessedEpisode> eis = null;
            // int snum = s.SeasonNumber;
            if (si.SeasonEpisodes.ContainsKey(snum))
                eis = si.SeasonEpisodes[snum]; // use processed episodes if they are available
            else
                eis = ShowItem.ProcessedListFromEpisodes(s.Episodes, si);

            string seasText = snum == 0
                ? TVSettings.Instance.SpecialsFolderName
                : (TVSettings.Instance.defaultSeasonWord + " " + snum);

            if ((eis.Count > 0) && (eis[0].SeasonId > 0))
                seasText = " - <A HREF=\"" + TheTVDB.Instance.WebsiteUrl(si.TVDBCode, eis[0].SeasonId, false) + "\">" +
                           seasText + "</a>";
            else
                seasText = " - " + seasText;

            body += "<h1><A HREF=\"" + TheTVDB.Instance.WebsiteUrl(si.TVDBCode, -1, true) + "\">" + si.ShowName +
                    "</A>" + seasText + "</h1>";

            if (TVSettings.Instance.NeedToDownloadBannerFile())
            {
                body += ImageSection("Series Banner", 758, 140, ser.GetSeasonWideBannerPath(snum));
                body += ImageSection("Series Poster", 350, 500, ser.GetSeasonBannerPath(snum));
            }
            else
            {
                body +=
                    "<h2>Images are not being downloaded for this series. Please see Options -> Preferences -> Media Center to reconfigure.</h2>";
            }

            return body;
        }

        private static string GetShowImagesHTMLOverview(ShowItem si, SeriesInfo ser)
        {
            string body =
                $"<h1><A HREF=\"{TheTVDB.Instance.WebsiteUrl(si.TVDBCode, -1, true)}\">{si.ShowName}</A> </h1>";

            body += ImageSection("Show Banner", 758, 140, ser.GetSeriesWideBannerPath());
            body += ImageSection("Show Poster", 350, 500, ser.GetSeriesPosterPath());
            body += ImageSection("Show Fanart", 960, 540, ser.GetSeriesFanartPath());
            return body;
        }

        private static string ImageSection(string title, int width, int height, string bannerPath)
        {
            if (string.IsNullOrEmpty(bannerPath)) return "";

            string url = TheTVDB.GetImageURL(bannerPath);

            if ((string.IsNullOrEmpty(url))) return "";

            return $"<h2>{title}</h2><img width={width} height={height} src=\"{url}\"><br/>";
        }

        private static string GetShowHTMLOverview(ShowItem si, SeriesInfo ser)
        {
            string body = "";

            List<string> skip = new List<string>
            {
                "Actors",
                "banner",
                "Overview",
                "overview",
                "Airs_Time",
                "airsTime",
                "Airs_DayOfWeek",
                "airsDayOfWeek",
                "fanart",
                "poster",
                "zap2it_id",
                "zap2itId",
                "id",
                "seriesName",
                "lastUpdated",
                "updatedBy"
            };

            if ((!string.IsNullOrEmpty(ser.GetSeriesWideBannerPath())) &&
                (!string.IsNullOrEmpty(TheTVDB.GetImageURL(ser.GetSeriesWideBannerPath()))))
                body += "<img width=758 height=140 src=\"" + TheTVDB.GetImageURL(ser.GetSeriesWideBannerPath()) +
                        "\"><br/>";

            body += $"<h1><A HREF=\"{TheTVDB.Instance.WebsiteUrl(si.TVDBCode, -1, true)}\">{si.ShowName}</A> </h1>";

            body += "<h2>Overview</h2>" + ser.GetOverview(); //get overview in either format

            bool first = true;
            foreach (string aa in ser.GetActors())
            {
                if (string.IsNullOrEmpty(aa)) continue;
                if (!first)
                    body += ", ";
                else
                    body += "<h2>Actors</h2>";

                body += "<A HREF=\"http://www.imdb.com/find?s=nm&q=" + aa + "\">" + aa + "</a>";
                first = false;
            }

            string airsTime = ser.GetAirsTime();
            string airsDay = ser.GetAirsDay();
            if ((!string.IsNullOrEmpty(airsTime)) && (!string.IsNullOrEmpty(airsDay)))
            {
                body += "<h2>Airs</h2> " + airsTime + " " + airsDay;
                string net = ser.GetNetwork();
                if (!string.IsNullOrEmpty(net))
                {
                    skip.Add("Network");
                    skip.Add("network");
                    body += ", " + net;
                }
            }

            bool firstInfo = true;
            foreach (KeyValuePair<string, string> kvp in ser.Items)
            {
                if (firstInfo)
                {
                    body += "<h2>Information<table border=0>";
                    firstInfo = false;
                }

                if (skip.Contains(kvp.Key)) continue;

                if (((kvp.Key == "SeriesID") || (kvp.Key == "seriesId")) & (kvp.Value != ""))
                    body += "<tr><td width=120px>tv.com</td><td><A HREF=\"http://www.tv.com/show/" + kvp.Value +
                            "/summary.html\">Visit</a></td></tr>";
                else if ((kvp.Key == "IMDB_ID") || (kvp.Key == "imdbId"))
                    body += "<tr><td width=120px>imdb.com</td><td><A HREF=\"http://www.imdb.com/title/" +
                            kvp.Value + "\">Visit</a></td></tr>";
                else if (kvp.Value != "")
                    body += "<tr><td width=120px>" + kvp.Key + "</td><td>" + kvp.Value + "</td></tr>";
            }

            if (!firstInfo)
                body += "</table>";

            return body;
        }

        private string GetSeasonHtmlOverview(ShowItem si, SeriesInfo ser, int snum)
        {
            string body = "";

            if (!string.IsNullOrEmpty(ser.GetSeriesWideBannerPath()) &&
                !string.IsNullOrEmpty(TheTVDB.GetImageURL(ser.GetSeriesWideBannerPath())))
                body += "<img width=758 height=140 src=\"" + TheTVDB.GetImageURL(ser.GetSeriesWideBannerPath()) +
                        "\"><br/>";

            Season s = si.DVDOrder ? ser.DVDSeasons[snum] : ser.AiredSeasons[snum];

            List<ProcessedEpisode> eis = null;
            // int snum = s.SeasonNumber;
            if (si.SeasonEpisodes.ContainsKey(snum))
                eis = si.SeasonEpisodes[snum]; // use processed episodes if they are available
            else
                eis = ShowItem.ProcessedListFromEpisodes(s.Episodes, si);

            string seasText = SeasonName(si, snum);

            if ((eis.Count > 0) && (eis[0].SeasonId > 0))
                seasText = " - <A HREF=\"" + TheTVDB.Instance.WebsiteUrl(si.TVDBCode, eis[0].SeasonId, false) + "\">" +
                           seasText + "</a>";
            else
                seasText = " - " + seasText;

            body += "<h1><A HREF=\"" + TheTVDB.Instance.WebsiteUrl(si.TVDBCode, -1, true) + "\">" + si.ShowName +
                    "</A>" + seasText + "</h1>";

            DirFilesCache dfc = new DirFilesCache();
            foreach (ProcessedEpisode ei in eis)
            {
                string epl = ei.NumsAsString();

                string episodeURL = TheTVDB.Instance.WebsiteUrl(ei.SeriesId, ei.SeasonId, ei.EpisodeId);

                body += "<A href=\"" + episodeURL + "\" name=\"ep" + epl + "\">"; // anchor
                if (si.DVDOrder && snum == 0)
                {
                    body += "<b>" + ei.Name + "</b>";
                }
                else
                    body += "<b>" + HttpUtility.HtmlEncode(CustomName.NameForNoExt(ei, CustomName.OldNStyle(6))) +
                            "</b>";

                body += "</A>"; // anchor
                if (si.UseSequentialMatch && (ei.OverallNumber != -1))
                    body += " (#" + ei.OverallNumber + ")";

                List<FileInfo> fl = TVDoc.FindEpOnDisk(dfc, ei);
                if (fl != null)
                {
                    foreach (FileInfo fi in fl)
                    {
                        string urlFilename = HttpUtility.UrlEncode(fi.FullName);
                        body += $" <A HREF=\"watch://{urlFilename}\" class=\"search\">Watch</A>";
                        body += $" <A HREF=\"explore://{urlFilename}\" class=\"search\">Show in Explorer</A>";
                    }
                }
                else body += " <A HREF=\"" + TVSettings.Instance.BTSearchURL(ei) + "\" class=\"search\">Search</A>";

                DateTime? dt = ei.GetAirDateDT(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                    body += "<p>" + dt.Value.ToShortDateString() + " (" + ei.HowLong() + ")";

                body += "<p><p>";

                if ((TVSettings.Instance.ShowEpisodePictures) ||
                    (TVSettings.Instance.HideMyShowsSpoilers && ei.HowLong() != "Aired"))
                {
                    body += "<table><tr>";
                    body += "<td width=100% valign=top>" + GetOverview(ei) + "</td><td width=300 height=225>";
                    // 300x168 / 300x225
                    if (!string.IsNullOrEmpty(ei.GetFilename()))
                        body += "<img src=" + TheTVDB.GetImageURL(ei.GetFilename()) + ">";

                    body += "</td></tr></table>";
                }
                else
                    body += GetOverview(ei);

                body += "<p><hr><p>";
            } // for each episode in this season

            return body;
        }

        private static string GetOverview(ProcessedEpisode ei)
        {
            string overviewString = ei.Overview;

            if (TVSettings.Instance.HideMyShowsSpoilers && ei.HowLong() != "Aired")
                overviewString = "[Spoilers Hidden]";

            bool firstInfo = true;
            foreach (KeyValuePair<string, string> kvp in ei.OtherItems())
            {
                if (firstInfo)
                {
                    overviewString += "<table border=0>";
                    firstInfo = false;
                }

                if ((kvp.Value != "") && kvp.Value != "0")
                {
                    if (((kvp.Key == "IMDB_ID") || (kvp.Key == "imdbId")))
                        overviewString += "<tr><td width=120px>imdb.com</td><td><A HREF=\"http://www.imdb.com/title/" +
                                          kvp.Value + "\">Visit</a></td></tr>";
                    else if (((kvp.Key == "showUrl")))
                        overviewString += "<tr><td width=120px>Link</td><td><A HREF=\"" + kvp.Value +
                                          "\">Visit</a></td></tr>";
                    else
                        overviewString += "<tr><td width=120px>" + kvp.Key + "</td><td>" + kvp.Value + "</td></tr>";
                }
            }

            if (!firstInfo)
                overviewString += "</table>";

            return overviewString;
        }

        public static string EpGuidePath() => FileHelper.TempPath("tvrenameepguide.html");
        public static string ImagesGuidePath() => FileHelper.TempPath("tvrenameimagesguide.html");
        public static string LocalFileURLBase(string path) => "file://" + path;

        public static void SetHTMLbody(string body, string path, WebBrowser web)
        {
            Color col = Color.FromName("ButtonFace");

            string css = "* { font-family: Tahoma, Arial; font-size 10pt; } " + "a:link { color: black } " +
                         "a:visited { color:black } " + "a:hover { color:#000080 } " + "a:active { color:black } " +
                         "a.search:link { color: #800000 } " + "a.search:visited { color:#800000 } " +
                         "a.search:hover { color:#000080 } " + "a.search:active { color:#800000 } " +
                         "* {background-color: #" + col.R.ToString("X2") + col.G.ToString("X2") + col.B.ToString("X2") +
                         "}" + "* { color: black }";

            string html = "<html><head><meta charset=\"UTF-8\"><STYLE type=\"text/css\">" + css + "</style>";

            html += "</head><body>";
            html += body;
            html += "</body></html>";

            web.Navigate("about:blank"); // make it close any file it might have open
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    BinaryWriter bw = new BinaryWriter(fs);

                    bw.Write(Encoding.GetEncoding("UTF-8").GetBytes(html));
                }

                web.Navigate(LocalFileURLBase(path));
            }
            catch (Exception ex)
            {
                //Fail gracefully - no RHS episode guide is not too big of a problem.
                //May get errors if TV Rename cannot access the filesystem or disk is full etc
                logger.Error(ex);
            }
        }

        private static void TVDBFor(ProcessedEpisode e)
        {
            if (e == null)
                return;

            Helpers.SysOpen(TheTVDB.Instance.WebsiteUrl(e.SI.TVDBCode, e.SeasonId, false));
        }

        private static void TVDBFor(Season seas)
        {
            if (seas == null)
                return;

            Helpers.SysOpen(TheTVDB.Instance.WebsiteUrl(seas.TheSeries.TVDBCode, -1, false));
        }

        private static void TVDBFor(ShowItem si)
        {
            if (si == null)
                return;

            Helpers.SysOpen(TheTVDB.Instance.WebsiteUrl(si.TVDBCode, -1, false));
        }

        public void menuSearchSites_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            mDoc.SetSearcher((int) (e.ClickedItem.Tag));
            UpdateSearchButtons();
        }

        public void bnWhenToWatchCheck_Click(object sender, EventArgs e) => RefreshWTW(true);

        public void FillWhenToWatchList()
        {
            mInternalChange++;
            lvWhenToWatch.BeginUpdate();

            int dd = TVSettings.Instance.WTWRecentDays;

            lvWhenToWatch.Groups["justPassed"].Header =
                "Aired in the last " + dd + " day" + ((dd == 1) ? "" : "s");

            // try to maintain selections if we can
            List<ProcessedEpisode> selections = new List<ProcessedEpisode>();
            foreach (ListViewItem lvi in lvWhenToWatch.SelectedItems)
                selections.Add((ProcessedEpisode) (lvi.Tag));

            Season currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
            ShowItem currentSI = TreeNodeToShowItem(MyShowTree.SelectedNode);

            lvWhenToWatch.Items.Clear();

            List<DateTime> bolded = new List<DateTime>();
            DirFilesCache dfc = new DirFilesCache();

            IEnumerable<ProcessedEpisode> recentEps = mDoc.Library.GetRecentAndFutureEps(dd);

            foreach (ProcessedEpisode ei in recentEps)
            {
                DateTime? dt = ei.GetAirDateDT(true);
                if ((dt != null)) bolded.Add(dt.Value);

                ListViewItem lvi = new ListViewItem();
                lvi.Text = "";
                for (int i = 0; i < 7; i++) lvi.SubItems.Add("");

                UpdateWTW(dfc, ei, lvi);

                lvWhenToWatch.Items.Add(lvi);

                foreach (ProcessedEpisode pe in selections)
                {
                    if (pe.SameAs(ei))
                    {
                        lvi.Selected = true;
                        break;
                    }
                }
            }

            lvWhenToWatch.Sort();

            lvWhenToWatch.EndUpdate();
            calCalendar.BoldedDates = bolded.ToArray();

            if (currentSeas != null)
                SelectSeason(currentSeas);
            else if (currentSI != null)
                SelectShow(currentSI);

            UpdateToolstripWTW();
            mInternalChange--;
        }

        public void lvWhenToWatch_ColumnClick(object sender, ColumnClickEventArgs e)
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
            else if ((col == 3) || (col == 4))
            {
                lvWhenToWatch.ListViewItemSorter = new DateSorterWTW();
                lvWhenToWatch.ShowGroups = true;
            }
            else
            {
                lvWhenToWatch.ShowGroups = false;
                if ((col == 1) || (col == 2))
                    lvWhenToWatch.ListViewItemSorter = new NumberAsTextSorter(col);
                else if (col == 5)
                    lvWhenToWatch.ListViewItemSorter = new DaySorter(col);
                else
                    lvWhenToWatch.ListViewItemSorter = new TextSorter(col);
            }

            lvWhenToWatch.Sort();
        }

        public void lvWhenToWatch_Click(object sender, EventArgs e)
        {
            UpdateSearchButtons();

            if (lvWhenToWatch.SelectedIndices.Count == 0)
            {
                txtWhenToWatchSynopsis.Text = "";
                return;
            }

            int n = lvWhenToWatch.SelectedIndices[0];

            ProcessedEpisode ei = (ProcessedEpisode) (lvWhenToWatch.Items[n].Tag);

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

        public void lvWhenToWatch_DoubleClick(object sender, EventArgs e)
        {
            if (lvWhenToWatch.SelectedItems.Count == 0)
                return;

            ProcessedEpisode ei = (ProcessedEpisode) (lvWhenToWatch.SelectedItems[0].Tag);
            List<FileInfo> fl = TVDoc.FindEpOnDisk(null, ei);
            if ((fl != null) && (fl.Count > 0))
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
                    UIScan(new List<ShowItem> {ei.SI}, false, TVSettings.ScanType.SingleShow);
                    tabControl1.SelectTab(tbAllInOne);
                    break;
            }
        }

        public void calCalendar_DateSelected(object sender, DateRangeEventArgs e)
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
                ProcessedEpisode ei = (ProcessedEpisode) (lvi.Tag);
                DateTime? dt2 = ei.GetAirDateDT(true);
                if (dt2 != null)
                {
                    double h = dt2.Value.Subtract(dt).TotalHours;
                    if ((h >= 0) && (h < 24.0))
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

        public void bnEpGuideRefresh_Click(object sender, EventArgs e)
        {
            bnWhenToWatchCheck_Click(null, null); // close enough!
            FillMyShows();
        }

        public void RefreshWTW(bool doDownloads)
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

        public void refreshWTWTimer_Tick(object sender, EventArgs e)
        {
            if (Busy == 0)
                RefreshWTW(false);
        }

        public void UpdateToolstripWTW()
        {
            // update toolstrip text too
            List<ProcessedEpisode> next1 = mDoc.Library.NextNShows(1, 0, 36500);

            tsNextShowTxt.Text = "Next airing: ";
            if ((next1 != null) && (next1.Count >= 1))
            {
                ProcessedEpisode ei = next1[0];
                tsNextShowTxt.Text += CustomName.NameForNoExt(ei, CustomName.OldNStyle(1)) + ", " + ei.HowLong() +
                                      " (" + ei.DayOfWeek() + ", " + ei.TimeOfDay() + ")";
            }
            else
                tsNextShowTxt.Text += "---";
        }

        public void bnWTWBTSearch_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lvWhenToWatch.SelectedItems)
                TVDoc.SearchForEpisode((ProcessedEpisode) (lvi.Tag));
        }

        public void NavigateTo(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.AbsoluteUri;
            if (url.Contains("tvrenameepguide.html#ep"))
                return; // don't intercept

            if (url.EndsWith("tvrenameepguide.html"))
                return; // don't intercept

            if (url.EndsWith("tvrenameimagesguide.html"))
                return; // don't intercept

            if (string.Compare(url, "about:blank", StringComparison.Ordinal) == 0)
                return; // don't intercept about:blank

            if (url == QuickStartGuide())
                return; // let the quickstartguide be shown

            if (url.Contains(@"ieframe.dll"))
                url = e.Url.Fragment.Substring(1);

            if (url.StartsWith("explore://", StringComparison.InvariantCultureIgnoreCase))
            {
                e.Cancel = true;
                string path = HttpUtility.UrlDecode(url.Substring("explore://".Length).Replace('/', '\\'));
                Helpers.SysOpen("explorer", $"/select, \"{path}\"");
            }

            if (url.StartsWith("watch://", StringComparison.InvariantCultureIgnoreCase))
            {
                e.Cancel = true;
                string fileName = HttpUtility.UrlDecode(url.Substring("watch://".Length))?.Replace('/', '\\');
                Helpers.SysOpen(fileName);
            }

            if ((string.Compare(url.Substring(0, 7), "http://", StringComparison.Ordinal) == 0) ||
                (string.Compare(url.Substring(0, 7), "file://", StringComparison.Ordinal) == 0))
            {
                e.Cancel = true;
                Helpers.SysOpen(e.Url.AbsoluteUri);
            }
        }

        public void notifyIcon1_Click(object sender, MouseEventArgs e)
        {
            // double-click of notification icon causes a click then doubleclick event, 
            // so we need to do a timeout before showing the single click's popup
            tmrShowUpcomingPopup.Start();
        }

        public void tmrShowUpcomingPopup_Tick(object sender, EventArgs e)
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
            UIScan(null, true, TVSettings.ScanType.Full);
        }

        public void notifyIcon1_DoubleClick(object sender, MouseEventArgs e)
        {
            tmrShowUpcomingPopup.Stop();
            FocusWindow();
        }

        public void buyMeADrinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BuyMeADrink bmad = new BuyMeADrink();
            bmad.ShowDialog();
        }

        public void GotoEpguideFor(ShowItem si, bool changeTab)
        {
            if (changeTab)
                tabControl1.SelectTab(tbMyShows);

            FillEpGuideHTML(si, -1);
        }

        public void GotoEpguideFor(ProcessedEpisode ep, bool changeTab)
        {
            if (changeTab)
                tabControl1.SelectTab(tbMyShows);

            SelectSeason(ep.AppropriateSeason);
        }

        public void RightClickOnMyShows(ShowItem si, Point pt)
        {
            mLastShowsClicked = new List<ShowItem> {si};
            mLastEpClicked = null;
            mLastSeasonClicked = null;
            mLastActionsClicked = null;
            BuildRightClickMenu(pt);
        }

        public void RightClickOnMyShows(Season seas, Point pt)
        {
            mLastShowsClicked = new List<ShowItem> {mDoc.Library.ShowItem(seas.TheSeries.TVDBCode)};
            mLastEpClicked = null;
            mLastSeasonClicked = seas;
            mLastActionsClicked = null;
            BuildRightClickMenu(pt);
        }

        public void WTWRightClickOnShow(List<ProcessedEpisode> eps, Point pt)
        {
            if (eps.Count == 0)
                return;

            ProcessedEpisode ep = eps[0];

            List<ShowItem> sis = new List<ShowItem>();
            foreach (ProcessedEpisode e in eps)
            {
                sis.Add(e.SI);
            }

            mLastEpClicked = ep;
            mLastShowsClicked = sis;
            mLastSeasonClicked = ep?.AppropriateSeason;
            mLastActionsClicked = null;
            BuildRightClickMenu(pt);
        }

        public void MenuGuideAndTVDB(bool addSep)
        {
            if (mLastShowsClicked == null || mLastShowsClicked.Count != 1)
                return; // nothing or multiple selected

            ShowItem si = (mLastShowsClicked != null) && (mLastShowsClicked.Count > 0)
                ? mLastShowsClicked[0]
                : null;

            Season seas = mLastSeasonClicked;
            ProcessedEpisode ep = mLastEpClicked;
            ToolStripMenuItem tsi;

            if (si != null)
            {
                if (addSep)
                {
                    showRightClickMenu.Items.Add(new ToolStripSeparator());
                    addSep = false;
                }

                tsi = new ToolStripMenuItem("Episode Guide");
                tsi.Tag = (int) RightClickCommands.kEpisodeGuideForShow;
                showRightClickMenu.Items.Add(tsi);
            }

            if (ep != null)
            {
                if (addSep)
                {
                    showRightClickMenu.Items.Add(new ToolStripSeparator());
                    addSep = false;
                }

                tsi = new ToolStripMenuItem("Visit thetvdb.com");
                tsi.Tag = (int) RightClickCommands.kVisitTVDBEpisode;
                showRightClickMenu.Items.Add(tsi);
            }
            else if (seas != null)
            {
                if (addSep)
                {
                    showRightClickMenu.Items.Add(new ToolStripSeparator());
                    addSep = false;
                }

                tsi = new ToolStripMenuItem("Visit thetvdb.com");
                tsi.Tag = (int) RightClickCommands.kVisitTVDBSeason;
                showRightClickMenu.Items.Add(tsi);
            }
            else if (si != null)
            {
                if (addSep)
                {
                    showRightClickMenu.Items.Add(new ToolStripSeparator());
                    addSep = false;
                }

                tsi = new ToolStripMenuItem("Visit thetvdb.com");
                tsi.Tag = (int) RightClickCommands.kVisitTVDBSeries;
                showRightClickMenu.Items.Add(tsi);
            }
        }

        public void MenuShowAndEpisodes()
        {
            ShowItem si = (mLastShowsClicked != null) && (mLastShowsClicked.Count > 0)
                ? mLastShowsClicked[0]
                : null;

            Season seas = mLastSeasonClicked;
            ProcessedEpisode ep = mLastEpClicked;
            ToolStripMenuItem tsi;

            if (si != null)
            {
                tsi = new ToolStripMenuItem("Force Refresh") {Tag = (int) RightClickCommands.kForceRefreshSeries};
                showRightClickMenu.Items.Add(tsi);

                tsi = new ToolStripMenuItem("Update Images") {Tag = (int) RightClickCommands.kUpdateImages};
                showRightClickMenu.Items.Add(tsi);

                ToolStripSeparator tss = new ToolStripSeparator();
                showRightClickMenu.Items.Add(tss);

                string scanText = mLastShowsClicked.Count > 1
                    ? "Scan Multiple Shows"
                    : "Scan \"" + si.ShowName + "\"";

                tsi = new ToolStripMenuItem(scanText) {Tag = (int) RightClickCommands.kScanSpecificSeries};
                showRightClickMenu.Items.Add(tsi);

                if (mLastShowsClicked != null && mLastShowsClicked.Count == 1)
                {
                    tsi = new ToolStripMenuItem("When to Watch") {Tag = (int) RightClickCommands.kWhenToWatchSeries};
                    showRightClickMenu.Items.Add(tsi);

                    tsi = new ToolStripMenuItem("Edit Show") {Tag = (int) RightClickCommands.kEditShow};
                    showRightClickMenu.Items.Add(tsi);

                    tsi = new ToolStripMenuItem("Delete Show") {Tag = (int) RightClickCommands.kDeleteShow};
                    showRightClickMenu.Items.Add(tsi);
                }
            }

            if (seas != null && mLastShowsClicked != null && mLastShowsClicked.Count == 1)
            {
                tsi = new ToolStripMenuItem("Edit " + (seas.SeasonNumber == 0
                                                ? TVSettings.Instance.SpecialsFolderName
                                                : TVSettings.Instance.defaultSeasonWord + " " + seas.SeasonNumber));

                tsi.Tag = (int) RightClickCommands.kEditSeason;
                showRightClickMenu.Items.Add(tsi);
            }

            if (ep != null && mLastShowsClicked != null && mLastShowsClicked.Count == 1)
            {
                List<FileInfo> fl = TVDoc.FindEpOnDisk(null, ep);
                if (fl == null) return;

                if (fl.Count <= 0) return;

                ToolStripSeparator tss = new ToolStripSeparator();
                showRightClickMenu.Items.Add(tss);

                int n = mLastFL.Count;
                foreach (FileInfo fi in fl)
                {
                    mLastFL.Add(fi);
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
                    if ((fl != null) && (fl.Count > 0))
                    {
                        if (first)
                        {
                            ToolStripSeparator tss = new ToolStripSeparator();
                            showRightClickMenu.Items.Add(tss);
                            first = false;
                        }

                        int n = mLastFL.Count;
                        foreach (FileInfo fi in fl)
                        {
                            mLastFL.Add(fi);
                            tsi = new ToolStripMenuItem("Watch: " + fi.FullName);
                            tsi.Tag = (int) RightClickCommands.kWatchBase + n;
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

            ShowItem si = (mLastShowsClicked != null) && (mLastShowsClicked.Count > 0)
                ? mLastShowsClicked[0]
                : null;

            Season seas = mLastSeasonClicked;
            ProcessedEpisode ep = mLastEpClicked;
            ToolStripMenuItem tsi;
            List<string> added = new List<string>();

            if (ep != null)
            {
                if (ep.SI.AllFolderLocations().ContainsKey(ep.AppropriateSeasonNumber))
                {
                    int n = mFoldersToOpen.Count;
                    bool first = true;
                    foreach (string folder in ep.SI.AllFolderLocations()[ep.AppropriateSeasonNumber])
                    {
                        if ((!string.IsNullOrEmpty(folder)) && Directory.Exists(folder))
                        {
                            if (first)
                            {
                                ToolStripSeparator tss = new ToolStripSeparator();
                                showRightClickMenu.Items.Add(tss);
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
            else if ((seas != null) && (si != null) && (si.AllFolderLocations().ContainsKey(seas.SeasonNumber)))
            {
                int n = mFoldersToOpen.Count;
                bool first = true;
                foreach (string folder in si.AllFolderLocations()[seas.SeasonNumber])
                {
                    if ((!string.IsNullOrEmpty(folder)) && Directory.Exists(folder) && !added.Contains(folder))
                    {
                        added.Add(folder); // don't show the same folder more than once
                        if (first)
                        {
                            ToolStripSeparator tss = new ToolStripSeparator();
                            showRightClickMenu.Items.Add(tss);
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
                        if ((!string.IsNullOrEmpty(folder)) && Directory.Exists(folder) && !added.Contains(folder))
                        {
                            added.Add(folder); // don't show the same folder more than once
                            if (first)
                            {
                                ToolStripSeparator tss = new ToolStripSeparator();
                                showRightClickMenu.Items.Add(tss);
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

            if (lvr != null) // add folders for selected Scan items
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
                        ToolStripSeparator tss = new ToolStripSeparator();
                        showRightClickMenu.Items.Add(tss);
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

        public void BuildRightClickMenu(Point pt)
        {
            showRightClickMenu.Items.Clear();
            mFoldersToOpen = new List<string>();
            mLastFL = new List<FileInfo>();

            MenuGuideAndTVDB(false);
            MenuShowAndEpisodes();
            MenuFolders(null);

            showRightClickMenu.Show(pt);
        }

        private void showRightClickMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            showRightClickMenu.Close();

            if (e.ClickedItem.Tag != null)
            {
                RightClickCommands n = (RightClickCommands) e.ClickedItem.Tag;

                ShowItem si = (mLastShowsClicked != null) && (mLastShowsClicked.Count > 0)
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

                    case RightClickCommands.kVisitTVDBEpisode: // thetvdb.com
                    {
                        TVDBFor(mLastEpClicked);
                        break;
                    }

                    case RightClickCommands.kVisitTVDBSeason:
                    {
                        TVDBFor(mLastSeasonClicked);
                        break;
                    }

                    case RightClickCommands.kVisitTVDBSeries:
                    {
                        if (si != null)
                            TVDBFor(si);

                        break;
                    }
                    case RightClickCommands.kScanSpecificSeries:
                    {
                        if (mLastShowsClicked != null)
                        {
                            UIScan(mLastShowsClicked, false, TVSettings.ScanType.SingleShow);

                            tabControl1.SelectTab(tbAllInOne);
                        }

                        break;
                    }

                    case RightClickCommands.kWhenToWatchSeries: // when to watch
                    {
                        int code = -1;
                        if (mLastEpClicked != null)
                            code = mLastEpClicked.TheSeries.TVDBCode;

                        if (si != null)
                            code = si.TVDBCode;

                        if (code != -1)
                        {
                            tabControl1.SelectTab(tbWTW);

                            for (int i = 0; i < lvWhenToWatch.Items.Count; i++)
                                lvWhenToWatch.Items[i].Selected = false;

                            for (int i = 0; i < lvWhenToWatch.Items.Count; i++)
                            {
                                ListViewItem lvi = lvWhenToWatch.Items[i];
                                ProcessedEpisode ei = (ProcessedEpisode) (lvi.Tag);
                                if ((ei != null) && (ei.TheSeries.TVDBCode == code))
                                    lvi.Selected = true;
                            }

                            lvWhenToWatch.Focus();
                        }

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
                    case RightClickCommands.kBTSearchFor:
                    {
                        foreach (ListViewItem lvi in lvAction.SelectedItems)
                        {
                            ItemMissing m = (ItemMissing) (lvi.Tag);
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
                    {
                        if ((mLastActionsClicked != null) && (mLastActionsClicked.Count > 0))
                        {
                            ItemMissing mi = (ItemMissing) mLastActionsClicked[0];
                            if (mi != null)
                            {
                                // browse for mLastActionClicked
                                openFile.Filter = "Video Files|" +
                                                  TVSettings.Instance.GetVideoExtensionsString()
                                                      .Replace(".", "*.") +
                                                  "|All Files (*.*)|*.*";

                                if (openFile.ShowDialog() == DialogResult.OK)
                                {
                                    // make new Item for copying/moving to specified location
                                    FileInfo from = new FileInfo(openFile.FileName);
                                    FileInfo to = new FileInfo(mi.TheFileNoExt + from.Extension);
                                    mDoc.TheActionList.Add(
                                        new ActionCopyMoveRename(
                                            TVSettings.Instance.LeaveOriginals
                                                ? ActionCopyMoveRename.Op.Copy
                                                : ActionCopyMoveRename.Op.Move, from, to
                                            , mi.Episode,
                                            TVSettings.Instance.Tidyup, mi));

                                    // and remove old Missing item
                                    mDoc.TheActionList.Remove(mi);

                                    DownloadIdentifiersController di = new DownloadIdentifiersController();

                                    // if we're copying/moving a file across, we might also want to make a thumbnail or NFO for it
                                    mDoc.TheActionList.Add(di.ProcessEpisode(mi.Episode, to));
                                }
                            }

                            mLastActionsClicked = null;
                            FillActionList();
                        }

                        break;
                    }
                    case RightClickCommands.kActionIgnore:
                        IgnoreSelected();
                        break;
                    case RightClickCommands.kActionIgnoreSeason:
                    {
                        // add season to ignore list for each show selected
                        if ((mLastActionsClicked != null) && (mLastActionsClicked.Count > 0))
                        {
                            foreach (Item ai in mLastActionsClicked)
                            {
                                Item er = ai;
                                if (er?.Episode == null)
                                    continue;

                                int snum = er.Episode.AppropriateSeasonNumber;

                                if (!er.Episode.SI.IgnoreSeasons.Contains(snum))
                                    er.Episode.SI.IgnoreSeasons.Add(snum);

                                // remove all other episodes of this season from the Action list
                                ItemList remove = new ItemList();
                                foreach (Item action in mDoc.TheActionList)
                                {
                                    Item er2 = action;

                                    if ((er2 != null) && (er2.Episode != null) &&
                                        (er2.Episode.AppropriateSeasonNumber == snum))
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

                        mLastActionsClicked = null;
                        FillActionList();
                        break;
                    }
                    case RightClickCommands.kActionDelete:
                        ActionDeleteSelected();
                        break;
                    default:
                    {
                        if ((n >= RightClickCommands.kWatchBase) && (n < RightClickCommands.kOpenFolderBase))
                        {
                            int wn = n - RightClickCommands.kWatchBase;
                            if ((mLastFL != null) && (wn >= 0) && (wn < mLastFL.Count))
                                Helpers.SysOpen(mLastFL[wn].FullName);
                        }
                        else if (n >= RightClickCommands.kOpenFolderBase)
                        {
                            int fnum = n - RightClickCommands.kOpenFolderBase;

                            if (fnum < mFoldersToOpen.Count)
                            {
                                string folder = mFoldersToOpen[fnum];

                                if (Directory.Exists(folder))
                                    Helpers.SysOpen(folder);
                            }

                            return;
                        }
                        else
                            Debug.Fail("Unknown right-click action " + n);

                        break;
                    }
                }
            }

            mLastEpClicked = null;
        }

        public void tabControl1_DoubleClick(object sender, EventArgs e)
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
            if ((int) (e.ClickedItem.Tag) == 0) Helpers.SysOpen(mLastFolderClicked);
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

            WTWRightClickOnShow(eis, pt);
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e) => DoPrefs(false);

        private void DoPrefs(bool scanOptions)
        {
            MoreBusy(); // no background download while preferences are open!

            Preferences pref = new Preferences(mDoc, scanOptions);
            if (pref.ShowDialog() == DialogResult.OK)
            {
                mDoc.SetDirty();
                TVDoc.SetPreferredLanguage();
                ShowHideNotificationIcon();
                FillWhenToWatchList();
                ShowInTaskbar = TVSettings.Instance.ShowInTaskbar;
                FillEpGuideHTML();
                mAutoFolderMonitor.SettingsChanged(TVSettings.Instance.MonitorFolders);
                betaToolsToolStripMenuItem.Visible = TVSettings.Instance.IncludeBetaUpdates();
                ForceRefresh(null);
            }

            LessBusy();
        }

        public void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                mDoc.WriteXMLSettings();
                TheTVDB.Instance.SaveCache();
                SaveLayoutXML();
            }
            catch (Exception ex)
            {
                Exception e2 = ex;
                while (e2.InnerException != null)
                    e2 = e2.InnerException;

                string m2 = e2.Message;
                MessageBox.Show(this,
                    ex.Message + "\r\n\r\n" +
                    m2 + "\r\n\r\n" +
                    ex.StackTrace,
                    "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void UI_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
                mLastNonMaximizedSize = Size;

            if ((WindowState == FormWindowState.Minimized) && (!TVSettings.Instance.ShowInTaskbar))
                Hide();
        }

        public void UI_LocationChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
                mLastNonMaximizedLocation = Location;
        }

        public void statusTimer_Tick(object sender, EventArgs e)
        {
            int n = mDoc.DownloadsRemaining();

            txtDLStatusLabel.Visible = (n != 0 || TVSettings.Instance.BGDownload);
            if (n != 0)
            {
                txtDLStatusLabel.Text = "Background download: " + TheTVDB.Instance.CurrentDLTask;
                backgroundDownloadNowToolStripMenuItem.Enabled = false;
            }
            else
                txtDLStatusLabel.Text = "Background download: Idle";

            if (Busy == 0)
            {
                if ((n == 0) && (LastDLRemaining > 0))
                {
                    // we've just finished a bunch of background downloads
                    TheTVDB.Instance.SaveCache();
                    RefreshWTW(false);

                    backgroundDownloadNowToolStripMenuItem.Enabled = true;
                }

                LastDLRemaining = n;
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

        private void BGDownloadTimer_Tick(object sender, EventArgs e)
        {
            if (Busy != 0)
            {
                BGDownloadTimer.Interval = 10000; // come back in 10 seconds
                BGDownloadTimer.Start();
                return;
            }

            BGDownloadTimer.Interval = BGDLLongInterval(); // after first time (10 seconds), put up to 60 minutes
            BGDownloadTimer.Start();

            if (TVSettings.Instance.BGDownload && mDoc.DownloadsRemaining() == 0
            ) // only do auto-download if don't have stuff to do already
            {
                mDoc.DoDownloadsBG();

                statusTimer_Tick(null, null);
            }
        }

        public void backgroundDownloadNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TVSettings.Instance.OfflineMode)
            {
                DialogResult res = MessageBox.Show("Ignore offline mode and download anyway?",
                    "Background Download", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (res != DialogResult.Yes)
                    return;
            }

            BGDownloadTimer.Stop();
            BGDownloadTimer.Start();

            mDoc.DoDownloadsBG();

            statusTimer_Tick(null, null);
        }

        public void offlineOperationToolStripMenuItem_Click(object sender, EventArgs e)
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

        public void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tbMyShows)
                FillEpGuideHTML();

            exportToolStripMenuItem.Enabled = false;
        }

        public void bugReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BugReport br = new BugReport(mDoc);
            br.ShowDialog();
        }

        public void ShowHideNotificationIcon()
        {
            notifyIcon1.Visible = TVSettings.Instance.NotificationAreaIcon && !mDoc.Args.Hide;
        }

        public void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StatsWindow sw = new StatsWindow(mDoc.Stats());
            sw.ShowDialog();
        }

        public TreeNode AddShowItemToTree(ShowItem si)
        {
            TheTVDB.Instance.GetLock("AddShowItemToTree");
            string name = si.ShowName;

            SeriesInfo ser = TheTVDB.Instance.GetSeries(si.TVDBCode);

            if (string.IsNullOrEmpty(name))
            {
                if (ser != null)
                    name = ser.Name;
                else
                    name = "-- Unknown : " + si.TVDBCode + " --";
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

                List<int> theKeys = si.DVDOrder
                    ? new List<int>(ser.DVDSeasons.Keys)
                    : new List<int>(ser.AiredSeasons.Keys);
                // now, go through and number them all sequentially
                //foreach (int snum in ser.Seasons.Keys)
                //    theKeys.Add(snum);

                theKeys.Sort();

                foreach (int snum in theKeys)
                {
                    Season s = si.DVDOrder ? ser.DVDSeasons[snum] : ser.AiredSeasons[snum];

                    string nodeTitle = SeasonName(si, snum);

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

        private static string SeasonName(ShowItem si, int snum)
        {
            string nodeTitle;
            if (si.DVDOrder)
            {
                nodeTitle = snum == 0
                    ? "Not Available on DVD"
                    : "DVD " + TVSettings.Instance.defaultSeasonWord + " " + snum;
            }
            else
            {
                nodeTitle = snum == 0
                    ? TVSettings.Instance.SpecialsFolderName
                    : TVSettings.Instance.defaultSeasonWord + " " + snum;
            }

            return nodeTitle;
        }

        public void UpdateWTW(DirFilesCache dfc, ProcessedEpisode pe, ListViewItem lvi)
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

            double ttn = (dt.Subtract(DateTime.Now)).TotalHours;

            if (ttn < 0)
                lvi.Group = lvWhenToWatch.Groups["justPassed"];
            else if (ttn < (7 * 24))
                lvi.Group = lvWhenToWatch.Groups["next7days"];
            else if (!pe.NextToAir)
                lvi.Group = lvWhenToWatch.Groups["later"];
            else
                lvi.Group = lvWhenToWatch.Groups["futureEps"];

            int n = 1;
            lvi.Text = pe.SI.ShowName;
            lvi.SubItems[n++].Text =
                (pe.AppropriateSeasonNumber != 0) ? pe.AppropriateSeasonNumber.ToString() : "Special";

            string estr = (pe.AppropriateEpNum > 0) ? pe.AppropriateEpNum.ToString() : "";
            if ((pe.AppropriateEpNum > 0) && (pe.EpNum2 != pe.AppropriateEpNum) && (pe.EpNum2 > 0))
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
                if ((fl != null) && (fl.Count > 0))
                    lvi.ImageIndex = 0;
                else if (pe.SI.DoMissingCheck)
                    lvi.ImageIndex = 1;
            }
        }

        public void SelectSeason(Season seas)
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

            FillEpGuideHTML(null);
        }

        public void SelectShow(ShowItem si)
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

            FillEpGuideHTML(null);
        }

        private void bnMyShowsAdd_Click(object sender, EventArgs e)
        {
            logger.Info("****************");
            logger.Info("Adding New Show");
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

                logger.Info("Added new show called {0}", si.ShowName);
            }
            else logger.Info("Cancelled adding new show");

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
            SeriesInfo ser = TheTVDB.Instance.GetSeries(si.TVDBCode);
            List<ProcessedEpisode> pel = ShowLibrary.GenerateEpisodes(si, ser, seasnum, false);

            EditRules er = new EditRules(si, pel, seasnum, TVSettings.Instance.NamingStyle);
            DialogResult dr = er.ShowDialog();
            Dictionary<int, Season> seasonsToUse = si.DVDOrder ? ser.DVDSeasons : ser.AiredSeasons;
            TheTVDB.Instance.Unlock("EditSeason");
            if (dr == DialogResult.OK)
            {
                ShowAddedOrEdited(false);
                SelectSeason(seasonsToUse[seasnum]);
            }

            LessBusy();
        }

        internal void EditShow(ShowItem si)
        {
            MoreBusy();
            TheTVDB.Instance.GetLock("EditShow");
            SeriesInfo ser = TheTVDB.Instance.GetSeries(si.TVDBCode);

            int oldCode = si.TVDBCode;

            AddEditShow aes = new AddEditShow(si);

            DialogResult dr = aes.ShowDialog();

            TheTVDB.Instance.Unlock("EditShow");

            if (dr == DialogResult.OK)
            {
                ShowAddedOrEdited(si.TVDBCode != oldCode);
                SelectShow(si);

                logger.Info("Modified show called {0}", si.ShowName);
            }

            LessBusy();
        }

        internal void ForceRefresh(List<ShowItem> sis)
        {
            mDoc.ForceRefresh(sis);
            FillMyShows();
            FillEpGuideHTML();
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

        private void MyShowTree_AfterSelect(object sender, TreeViewEventArgs e) => FillEpGuideHTML(e.Node);

        private void bnMyShowsVisitTVDB_Click(object sender, EventArgs e)
        {
            TreeNode n = MyShowTree.SelectedNode;
            ShowItem si = TreeNodeToShowItem(n);
            if (si == null)
                return;

            Season seas = TreeNodeToSeason(n);

            int sid = -1;
            if (seas != null)
                sid = seas.SeasonId;

            Helpers.SysOpen(TheTVDB.Instance.WebsiteUrl(si.TVDBCode, sid, false));
        }

        private void bnMyShowsOpenFolder_Click(object sender, EventArgs e)
        {
            TreeNode n = MyShowTree.SelectedNode;
            ShowItem si = TreeNodeToShowItem(n);
            if (si == null)
                return;

            Season seas = TreeNodeToSeason(n);
            Dictionary<int, List<string>> afl = si.AllFolderLocations();
            int[] keys = new int[afl.Count];
            afl.Keys.CopyTo(keys, 0);
            if ((seas == null) && (keys.Length > 0))
            {
                string f = si.AutoAdd_FolderBase;
                if (string.IsNullOrEmpty(f))
                {
                    int n2 = keys[0];
                    if (afl[n2].Count > 0)
                        f = afl[n2][0];
                }

                if (!string.IsNullOrEmpty(f))
                {
                    try
                    {
                        Helpers.SysOpen(f);
                        return;
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            if ((seas != null) && (afl.ContainsKey(seas.SeasonNumber)))
            {
                foreach (string folder in afl[seas.SeasonNumber])
                {
                    if (Directory.Exists(folder))
                    {
                        Helpers.SysOpen(folder);
                        return;
                    }
                }
            }

            try
            {
                if (!string.IsNullOrEmpty(si.AutoAdd_FolderBase) && (Directory.Exists(si.AutoAdd_FolderBase)))
                    Helpers.SysOpen(si.AutoAdd_FolderBase);
            }
            catch
            {
                // ignored
            }
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

        private List<ProcessedEpisode> CurrentlySelectedPEL()
        {
            Season currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
            ShowItem currentSI = TreeNodeToShowItem(MyShowTree.SelectedNode);

            int snum = (currentSeas != null) ? currentSeas.SeasonNumber : 1;
            List<ProcessedEpisode> pel = null;
            if ((currentSI != null) && (currentSI.SeasonEpisodes.ContainsKey(snum)))
                pel = currentSI.SeasonEpisodes[snum];
            else
            {
                foreach (ShowItem si in mDoc.Library.GetShowItems())
                {
                    foreach (KeyValuePair<int, List<ProcessedEpisode>> kvp in si
                        .SeasonEpisodes)
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
            CustomName cn = new CustomName(TVSettings.Instance.NamingStyle.StyleString);
            CustomNameDesigner cne = new CustomNameDesigner(CurrentlySelectedPEL(), cn, mDoc);
            DialogResult dr = cne.ShowDialog();
            if (dr == DialogResult.OK)
            {
                TVSettings.Instance.NamingStyle = cn;
                mDoc.SetDirty();
            }
        }

        private void searchEnginesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<ProcessedEpisode> pel = CurrentlySelectedPEL();

            AddEditSearchEngine aese = new AddEditSearchEngine(TVDoc.GetSearchers(),
                ((pel != null) && (pel.Count > 0)) ? pel[0] : null);

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
            string theFolder = "";

            if (currentShow != null)
            {
                foreach (KeyValuePair<int, List<string>> kvp in
                    currentShow.AllFolderLocations())
                {
                    foreach (string folder in kvp.Value)
                    {
                        if ((!string.IsNullOrEmpty(folder)) && Directory.Exists(folder))
                        {
                            theFolder = folder;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(theFolder))
                        break;
                }
            }

            AddEditSeasEpFinders d = new AddEditSeasEpFinders(TVSettings.Instance.FNPRegexs,
                mDoc.Library.GetShowItems(), currentShow, theFolder);

            DialogResult dr = d.ShowDialog();
            if (dr == DialogResult.OK)
            {
                mDoc.SetDirty();
                TVSettings.Instance.FNPRegexs = d.OutputRegularExpressions;
            }
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
            uTorrent ut = new uTorrent(mDoc, SetProgress);
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

            if (MyShowTree.SelectedNode != null)
                MyShowTree.SelectedNode.EnsureVisible();

            MyShowTree.EndUpdate();
        }

        private void UI_KeyDown(object sender, KeyEventArgs e)
        {
            int t = -1;
            if (e.Control && (e.KeyCode == Keys.D1))
                t = 0;
            else if (e.Control && (e.KeyCode == Keys.D2))
                t = 1;
            else if (e.Control && (e.KeyCode == Keys.D3))
                t = 2;
            else if (e.Control && (e.KeyCode == Keys.D4))
                t = 3;
            else if (e.Control && (e.KeyCode == Keys.D5))
                t = 4;
            else if (e.Control && (e.KeyCode == Keys.D6))
                t = 5;
            else if (e.Control && (e.KeyCode == Keys.D7))
                t = 6;
            else if (e.Control && (e.KeyCode == Keys.D8))
                t = 7;
            else if (e.Control && (e.KeyCode == Keys.D9))
                t = 8;
            else if (e.Control && (e.KeyCode == Keys.D0))
                t = 9;

            if ((t >= 0) && (t < tabControl1.TabCount))
            {
                tabControl1.SelectedIndex = t;
                e.Handled = true;
            }
        }

        private void bnActionCheck_Click(object sender, EventArgs e)
        {
            UIScan(null, false, TVSettings.ScanType.Full);
        }

        public void QuickScan()
        {
            UIScan(null, true, TVSettings.ScanType.Quick);
        }

        public void RecentScan()
        {
            UIScan(mDoc.Library.getRecentShows(), true, TVSettings.ScanType.Recent);
        }

        private void UIScan(List<ShowItem> shows, bool unattended, TVSettings.ScanType st)
        {
            logger.Info("*******************************");
            string desc = unattended ? "unattended " : "";
            string showsdesc = shows?.Count > 0 ? shows.Count.ToString() : "all";
            string scantype = st.PrettyPrint();

            logger.Info($"Starting {desc}{scantype} Scan for {showsdesc} shows...");
            if (st != TVSettings.ScanType.SingleShow) GetNewShows(unattended);
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
                logger.Info("Not looking for new shows as 'Auto-Add' is turned off");
                return;
            }

            //Dont support unattended mode
            if (unattended)
            {
                logger.Info("Not looking for new shows as app is unattended");
                return;
            }

            List<string> possibleShowNames = new List<string>();

            foreach (string dirPath in TVSettings.Instance.DownloadFolders)
            {
                logger.Info("Parsing {0} for new shows", dirPath);
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
                    logger.Info($"Ignoring {hint} as it matches existing shows.");
                    continue;
                }

                //If the hint contains certain terms then we'll ignore it
                if (IgnoreHint(hint))
                {
                    logger.Info($"Ignoring {hint} as it is in the ignore list.");
                    continue;
                }

                //Remove anything we can from hint to make it cleaner and hence more likely to match
                string refinedHint = RemoveSeriesEpisodeIndicators(hint);

                if (string.IsNullOrWhiteSpace(refinedHint))
                {
                    logger.Info($"Ignoring {hint} as it refines to nothing.");
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

                logger.Info("****************");
                logger.Info("Auto Adding New Show");
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
                else logger.Info("Cancelled Auto adding new show");

                LessBusy();
            }

            mDoc.Library.AddRange(addedShows);

            ShowAddedOrEdited(true);

            if (addedShows.Count <= 0) return;

            SelectShow(addedShows.Last());
            logger.Info("Added new shows called: {0}", string.Join(",", addedShows.Select(s => s.ShowName)));
        }

        private static bool IgnoreHint(string hint)
        {
            return TVSettings.Instance.AutoAddMovieTermsArray.Any(term =>
                hint.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        private string RemoveSeriesEpisodeIndicators(string hint)
        {
            string hint2 = Helpers.RemoveDiacritics(hint);
            hint2 = RemoveSE(hint2);
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

        private string RemoveSE(string hint)
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
                        int p2 = Math.Min(p, hint.IndexOf(m.Groups.SyncRoot.ToString()));

                        if ((seas != -1) && (ep != -1)) return hint.Remove(p2 != -1 ? p2 : p);
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

        private bool LookForSeries(string test, IEnumerable<ShowItem> shows)
        {
            foreach (ShowItem si in shows)
            {
                if (si.getSimplifiedPossibleShowNames()
                    .Any(name => FileHelper.SimplifyAndCheckFilename(test, name)))
                    return true;
            }

            return false;
        }

        private ListViewItem LVIForItem(Item item)
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

            Action act = item as Action;
            if ((act != null) && act.Error)
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
            e.Item = LVIForItem(item);
        }

        private void FillActionList()
        {
            InternalCheckChange = true;

            // Save where the list is currently scrolled too
            var currentTop = lvAction.GetScrollVerticalPos();

            if (lvAction.VirtualMode)
                lvAction.VirtualListSize = mDoc.TheActionList.Count;
            else
            {
                lvAction.BeginUpdate();
                lvAction.Items.Clear();

                foreach (Item item in mDoc.TheActionList)
                {
                    ListViewItem lvi = LVIForItem(item);
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

            foreach (Item Action in mDoc.TheActionList)
            {
                if (Action is ItemMissing)
                    missingCount++;
                else if (Action is ActionCopyMoveRename)
                {
                    ActionCopyMoveRename cmr = (ActionCopyMoveRename) (Action);
                    ActionCopyMoveRename.Op op = cmr.Operation;
                    if (op == ActionCopyMoveRename.Op.Copy)
                    {
                        copyCount++;
                        if (cmr.From.Exists)
                            copySize += cmr.From.Length;
                    }
                    else if (op == ActionCopyMoveRename.Op.Move)
                    {
                        moveCount++;
                        if (cmr.From.Exists)
                            moveSize += cmr.From.Length;
                    }
                    else if (op == ActionCopyMoveRename.Op.Rename)
                        renameCount++;
                }
                else if (Action is ActionDownloadImage)
                    downloadCount++;
                else if (Action is ActionRSS)
                    rssCount++;
                else if (Action is ActionWriteMetadata) // base interface that all metadata actions are derived from
                    metaCount++;
                else if (Action is ActionDateTouch)
                    fileMetaCount++;
                else if (Action is ItemuTorrenting || Action is ItemSABnzbd)
                    dlCount++;
                else if (Action is ActionDeleteFile || Action is ActionDeleteDirectory)
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

            InternalCheckChange = false;

            UpdateActionCheckboxes();
        }

        private static string HeaderName(string name, int number)
        {
            return $"{name} ({PrettyPrint(number)})";
        }

        private static string PrettyPrint(int number)
        {
            return number + " " + (number.ItemItems());
        }

        private static string HeaderName(string name, int number, long filesize)
        {
            return $"{name} ({PrettyPrint(number)}, {filesize.GBMB(1)})";
        }

        private void bnActionAction_Click(object sender, EventArgs e) => ProcessAll();

        public void ProcessAll() => ActionAction(true);

        private void ActionAction(bool checkedNotSelected)
        {
            mDoc.CurrentlyBusy = true;
            LVResults lvr = new LVResults(lvAction, checkedNotSelected);
            mDoc.DoActions(lvr.FlatList);
            // remove items from master list, unless it had an error
            foreach (Item i2 in (new LVResults(lvAction, checkedNotSelected)).FlatList)
            {
                if ((i2 != null) && (!lvr.FlatList.Contains(i2)))
                    mDoc.TheActionList.Remove(i2);
            }

            FillActionList();
            RefreshWTW(false);
            mDoc.CurrentlyBusy = false;
        }

        private void Revert(bool checkedNotSelected)
        {
            mDoc.CurrentlyBusy = true;

            foreach (Item item in (new LVResults(lvAction, checkedNotSelected).FlatList))
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

                    if ((a is ActionCopyMoveRename i1))
                    {
                        if (i1.From.RemoveExtension(true).StartsWith(i2.From.RemoveExtension(true)))
                        {
                            toRemove.Add(i1);
                        }
                    }
                    else if (a is Item ad)
                    {
                        if ((ad.Episode?.AppropriateEpNum == i2.Episode?.AppropriateEpNum) &&
                            (ad.Episode?.AppropriateSeasonNumber == i2.Episode?.AppropriateSeasonNumber))
                            toRemove.Add(a);
                    }
                }

                //Remove all similar items
                foreach (Item i in toRemove) mDoc.TheActionList.Remove(i);
            }

            FillActionList();
            RefreshWTW(false);
            mDoc.CurrentlyBusy = false;
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
            TorrentMatch tm = new TorrentMatch(mDoc, SetProgress);
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
            ToolStripMenuItem tsi;
            if (lvr.Count > lvr.Missing.Count) // not just missing selected
            {
                tsi = new ToolStripMenuItem("Action Selected");
                tsi.Tag = (int) RightClickCommands.kActionAction;
                showRightClickMenu.Items.Add(tsi);
            }

            tsi = new ToolStripMenuItem("Ignore Selected");
            tsi.Tag = (int) RightClickCommands.kActionIgnore;
            showRightClickMenu.Items.Add(tsi);

            tsi = new ToolStripMenuItem("Ignore Entire Season");
            tsi.Tag = (int) RightClickCommands.kActionIgnoreSeason;
            showRightClickMenu.Items.Add(tsi);

            tsi = new ToolStripMenuItem("Remove Selected");
            tsi.Tag = (int) RightClickCommands.kActionDelete;
            showRightClickMenu.Items.Add(tsi);

            if (lvr.Count == lvr.Missing.Count) // only missing items selected?
            {
                showRightClickMenu.Items.Add(new ToolStripSeparator());

                tsi = new ToolStripMenuItem("Search");
                tsi.Tag = (int) RightClickCommands.kBTSearchFor;
                showRightClickMenu.Items.Add(tsi);

                if (lvr.Count == 1) // only one selected
                {
                    tsi = new ToolStripMenuItem("Browse For...");
                    tsi.Tag = (int) RightClickCommands.kActionBrowseForFile;
                    showRightClickMenu.Items.Add(tsi);
                }
            }

            if (lvr.CopyMove.Count > 0)
            {
                showRightClickMenu.Items.Add(new ToolStripSeparator());

                tsi = new ToolStripMenuItem("Revert to Missing") {Tag = (int) RightClickCommands.kActionRevert};
                showRightClickMenu.Items.Add(tsi);
            }

            MenuGuideAndTVDB(true);
            MenuFolders(lvr);

            showRightClickMenu.Show(pt);
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
            mLastFL = new List<FileInfo>();

            mLastActionsClicked = new ItemList();

            foreach (Item ai in lvr.FlatList)
                mLastActionsClicked.Add(ai);

            if ((lvr.Count != 1) || lvAction.FocusedItem?.Tag == null) return;

            if (!(lvAction.FocusedItem.Tag is Item action)) return;

            mLastEpClicked = action.Episode;
            if (action.Episode != null)
            {
                mLastSeasonClicked = action.Episode.AppropriateSeason;
                mLastShowsClicked = new List<ShowItem> {action.Episode.SI};
            }
            else
            {
                mLastSeasonClicked = null;
                mLastShowsClicked = null;
            }

            if ((mLastEpClicked != null) && (TVSettings.Instance.AutoSelectShowInMyShows))
                GotoEpguideFor(mLastEpClicked, false);
        }

        private void ActionDeleteSelected()
        {
            ListView.SelectedListViewItemCollection sel = lvAction.SelectedItems;
            foreach (ListViewItem lvi in sel)
                mDoc.TheActionList.Remove((Item) (lvi.Tag));

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
            if (InternalCheckChange)
                return;

            LVResults all = new LVResults(lvAction, LVResults.WhichResults.All);
            LVResults chk = new LVResults(lvAction, LVResults.WhichResults.Checked);

            if (chk.Rename.Count == 0)
                cbRename.CheckState = CheckState.Unchecked;
            else
                cbRename.CheckState = (chk.Rename.Count == all.Rename.Count)
                    ? CheckState.Checked
                    : CheckState.Indeterminate;

            if (chk.CopyMove.Count == 0)
                cbCopyMove.CheckState = CheckState.Unchecked;
            else
                cbCopyMove.CheckState = (chk.CopyMove.Count == all.CopyMove.Count)
                    ? CheckState.Checked
                    : CheckState.Indeterminate;

            if (chk.RSS.Count == 0)
                cbRSS.CheckState = CheckState.Unchecked;
            else
                cbRSS.CheckState =
                    (chk.RSS.Count == all.RSS.Count) ? CheckState.Checked : CheckState.Indeterminate;

            if (chk.Download.Count == 0)
                cbDownload.CheckState = CheckState.Unchecked;
            else
                cbDownload.CheckState = (chk.Download.Count == all.Download.Count)
                    ? CheckState.Checked
                    : CheckState.Indeterminate;

            if (chk.NFO.Count == 0)
                cbNFO.CheckState = CheckState.Unchecked;
            else
                cbNFO.CheckState =
                    (chk.NFO.Count == all.NFO.Count) ? CheckState.Checked : CheckState.Indeterminate;

            if (chk.PyTivoMeta.Count == 0)
                cbMeta.CheckState = CheckState.Unchecked;
            else
                cbMeta.CheckState = (chk.PyTivoMeta.Count == all.PyTivoMeta.Count)
                    ? CheckState.Checked
                    : CheckState.Indeterminate;

            int total1 = all.Rename.Count + all.CopyMove.Count + all.RSS.Count + all.Download.Count + all.NFO.Count +
                         all.PyTivoMeta.Count;

            int total2 = chk.Rename.Count + chk.CopyMove.Count + chk.RSS.Count + chk.Download.Count + chk.NFO.Count +
                         chk.PyTivoMeta.Count;

            if (total2 == 0)
                cbAll.CheckState = CheckState.Unchecked;
            else
                cbAll.CheckState = (total2 == total1) ? CheckState.Checked : CheckState.Indeterminate;
        }

        private void cbActionAllNone_Click(object sender, EventArgs e)
        {
            CheckState cs = cbAll.CheckState;
            if (cs == CheckState.Indeterminate)
            {
                cbAll.CheckState = CheckState.Unchecked;
                cs = CheckState.Unchecked;
            }

            InternalCheckChange = true;
            foreach (ListViewItem lvi in lvAction.Items)
                lvi.Checked = cs == CheckState.Checked;

            InternalCheckChange = false;
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

            InternalCheckChange = true;
            foreach (ListViewItem lvi in lvAction.Items)
            {
                Item i = (Item) (lvi.Tag);
                if (i is ActionCopyMoveRename rename && (rename.Operation == ActionCopyMoveRename.Op.Rename))
                    lvi.Checked = cs == CheckState.Checked;
            }

            InternalCheckChange = false;
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

            InternalCheckChange = true;
            foreach (ListViewItem lvi in lvAction.Items)
            {
                Item i = (Item) (lvi.Tag);
                if (i is ActionCopyMoveRename copymove && (copymove.Operation != ActionCopyMoveRename.Op.Rename))
                    lvi.Checked = cs == CheckState.Checked;
            }

            InternalCheckChange = false;
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

            InternalCheckChange = true;
            foreach (ListViewItem lvi in lvAction.Items)
            {
                Item i = (Item) (lvi.Tag);
                if ((i != null) && (i is ActionNfo))
                    lvi.Checked = cs == CheckState.Checked;
            }

            InternalCheckChange = false;
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

            InternalCheckChange = true;
            foreach (ListViewItem lvi in lvAction.Items)
            {
                Item i = (Item) (lvi.Tag);
                if ((i != null) && (i is ActionPyTivoMeta))
                    lvi.Checked = cs == CheckState.Checked;
            }

            InternalCheckChange = false;
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

            InternalCheckChange = true;
            foreach (ListViewItem lvi in lvAction.Items)
            {
                Item i = (Item) (lvi.Tag);
                if ((i != null) && (i is ActionRSS))
                    lvi.Checked = cs == CheckState.Checked;
            }

            InternalCheckChange = false;
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

            InternalCheckChange = true;
            foreach (ListViewItem lvi in lvAction.Items)
            {
                Item i = (Item) (lvi.Tag);
                if ((i != null) && (i is ActionDownloadImage))
                    lvi.Checked = cs == CheckState.Checked;
            }

            InternalCheckChange = false;
            UpdateActionCheckboxes();
        }

        private void lvAction_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if ((e.Index < 0) || (e.Index > lvAction.Items.Count))
                return;

            Item action = (Item) (lvAction.Items[e.Index].Tag);
            if ((action != null) && ((action is ItemMissing) || (action is ItemuTorrenting) || (action is ItemSABnzbd)))
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
            UIScan(null, false, TVSettings.ScanType.Recent);

        private void btnActionQuickScan_Click(object sender, EventArgs e) =>
            UIScan(null, false, TVSettings.ScanType.Quick);

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
                            ? ActionCopyMoveRename.Op.Copy
                            : ActionCopyMoveRename.Op.Move, from, to
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
                var filterButton = filterTextBox.Controls["Clear"];
                filterButton.Location = new Point(filterTextBox.ClientSize.Width - filterButton.Width,
                    ((filterTextBox.ClientSize.Height - 16) / 2) + 1);

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
                //this.btnUpdateAvailable.Visible = false;
                if (showNoUpdateRequiredDialog && !inSilentMode)
                {
                    MessageBox.Show(@"There is no update available please try again later.", @"No update available",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                return;
            }

            if (inSilentMode)
            {
                logger.Warn(update.LogMessage());
            }
            else
            {
                UpdateNotification unForm = new UpdateNotification(update);
                unForm.ShowDialog();
                //this.btnUpdateAvailable.Visible = true;
            }
        }

        private void duplicateFinderLOGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<PossibleDuplicateEpisode> x = mDoc.FindDoubleEps();
            DupEpFinder form = new DupEpFinder(x, mDoc, this);
            form.ShowDialog();
        }

        private async void btnUpdateAvailable_Click(object sender, EventArgs e)
        {
            Task<UpdateVersion> uv = VersionUpdater.CheckForUpdatesAsync();
            NotifyUpdates(await uv, true);
        }

        private void tmrPeriodicScan_Tick(object sender, EventArgs e) => RunAutoScan("Periodic Scan");

        private void RunAutoScan(string scanType)
        {
            //We only wish to do a scan now if we are not already undertaking one
            if (!mDoc.CurrentlyBusy)
            {
                logger.Info("*******************************");
                logger.Info(scanType + " fired");
                UIScan(null, true, TVSettings.Instance.MonitoredFoldersScanType);
                ProcessAll();
                logger.Info(scanType + " complete");
            }
            else
            {
                logger.Info(scanType + " cancelled as the system is already busy");
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

            logger.Info(results.PrintVersion());
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

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }
}
