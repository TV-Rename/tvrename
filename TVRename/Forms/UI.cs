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
using System.ComponentModel;
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
using Humanizer;
using JetBrains.Annotations;
using TVRename.Forms;
using TVRename.Forms.Tools;
using TVRename.Forms.Utilities;
using TVRename.Ipc;
using TVRename.Properties;
using DataFormats = System.Windows.Forms.DataFormats;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using DragDropEffects = System.Windows.Forms.DragDropEffects;
using File = Alphaleonis.Win32.Filesystem.File;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using MessageBox = System.Windows.Forms.MessageBox;
using SystemColors = System.Drawing.SystemColors;
using BrightIdeasSoftware;

namespace TVRename
{
    ///  <summary>
    ///  Summary for UI
    ///  WARNING: If you change the name of this class, you will need to change the
    ///           'Resource File Name' property for the managed resource compiler tool
    ///           associated with all .resx files this class depends on.  Otherwise,
    ///           the designers will not be able to interact properly with localized
    ///           resources associated with this form.
    ///  </summary>
    // ReSharper disable once InconsistentNaming
    public partial class UI : Form, IRemoteActions, IDialogParent
    {
        internal const string EXPLORE_PROXY = "http://www.tvrename.com/EXPLOREPROXY";
        internal const string WATCH_PROXY = "http://www.tvrename.com/WATCHPROXY";

        #region Delegates

        public delegate void ScanTypeDelegate(TVSettings.ScanType type);
        public delegate void ArgumentDelegate(string[] args);

        public readonly ScanTypeDelegate ScanAndDo;
        public readonly ArgumentDelegate ReceiveArgumentDelegate;

        #endregion

        private int busy;
        private readonly TVDoc mDoc;
        private bool internalCheckChange;
        private int lastDlRemaining;
        private int mInternalChange;
        private Point mLastNonMaximizedLocation;
        private Size mLastNonMaximizedSize;
        private readonly AutoFolderMonitor mAutoFolderMonitor;
        private bool treeExpandCollapseToggle = true;

        private ProcessedEpisode? switchToWhenOpenMyShows;
        private MovieConfiguration? switchToWhenOpenMyMovies;

        private readonly ListViewColumnSorter lvwScheduleColumnSorter;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private bool IsBusy => busy!=0;

        public UI(TVDoc doc, [NotNull] TVRenameSplash splash, bool showUi)
        {
            mDoc = doc;

            busy = 0;

            mInternalChange = 0;

            internalCheckChange = false;

            InitializeComponent();

            ScanAndDo = ScanAndAction;
            ReceiveArgumentDelegate = RecieveArguments;

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

            lvwScheduleColumnSorter=new ListViewColumnSorter( new DateSorterWtw(3));
            lvWhenToWatch.ListViewItemSorter = lvwScheduleColumnSorter;

            if (mDoc.Args.Hide || !showUi)
            {
                WindowState = FormWindowState.Minimized;
                Visible = false;
                Hide();
            }

            tmrPeriodicScan.Enabled = false;

            UpdateSplashStatus(splash, "Filling Shows");
            mDoc.TvLibrary.GenDict();
            FillMyShows();
            FillMyMovies();
            UpdateSearchButtons();
            SetScan(TVSettings.Instance.UIScanType);
            ClearInfoWindows();
            UpdateSplashPercent(splash, 10);
            UpdateSplashStatus(splash, "Updating WTW");
            mDoc.DoWhenToWatch(true,true,WindowState==FormWindowState.Minimized,this);
            UpdateSplashPercent(splash, 40);
            FillWhenToWatchList();
            SortSchedule(3);
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

            SetupObjectListForScanResults();

            UpdateSplashStatus(splash, "Running autoscan");
        }

        private delegate void ShowChildConsumer(Form childForm);
        public void ShowChildDialog(Form childForm)
        {
            if (InvokeRequired)
            {
                ShowChildConsumer d = ShowChildDialog;
                Invoke(d, childForm);
            }
            else
            {
                if (IsDisposed)
                {
                    childForm.ShowDialog();
                }
                else
                {
                    childForm.ShowDialog(this);
                }
            }
        }

        public void ShowChild(Form childForm)
        {
            if (InvokeRequired)
            {
                ShowChildConsumer d = ShowChild;
                Invoke(d, childForm);
            }
            else
            {
                if (IsDisposed)
                {
                    childForm.Show();
                }
                else
                {
                    childForm.Show(this);
                }
            }
        }

        private void SetupObjectListForScanResults()
        {
            olvAction.SetObjects(mDoc.TheActionList);

            olvShowColumn.AspectToStringConverter = ConvertShowNameDelegate;
            olvShowColumn.ImageGetter = ActionImageGetter;

            olvType.GroupKeyGetter = GroupItemsKeyDelegate;
            olvType.GroupKeyToTitleConverter = GroupItemsTitleDelegate;

            olvDate.GroupKeyGetter = GroupDateKeyDelegate;
            olvDate.GroupKeyToTitleConverter = GroupDateTitleDegate;

            olvSeason.GroupKeyGetter = GroupSeasonKeyDelegate;

            olvFolder.GroupKeyGetter = GroupFolderTitleDelegate;

            SimpleDropSink currActionDropSink = (SimpleDropSink)olvAction.DropSink;
            currActionDropSink.FeedbackColor = Color.LightGray;

            olvDate.DataType = typeof(DateTime);
            olvAction.SortGroupItemsByPrimaryColumn = false;
        }

        private static object GroupFolderTitleDelegate(object rowObject)
        {
            Item ep = (Item) rowObject;
            foreach (string folder in TVSettings.Instance.LibraryFolders)
            {
                if (ep.DestinationFolder.StartsWith(folder, StringComparison.OrdinalIgnoreCase))
                {
                    return folder;
                }
            }

            return ep.DestinationFolder;
        }

        [NotNull]
        private static object GroupSeasonKeyDelegate(object rowObject)
        {
            Item ep = (Item) rowObject;
            if (ep.Movie != null)
            {
                return PostpendTheIfNeeded(ep.Movie.ShowName);
            }

            if (ep.SeasonNumber.HasValue())
            {
                return $"{ep.SeriesName} - Season {ep.SeasonNumber}";
            }

            return ep.SeriesName;
        }

        [NotNull]
        private static string GroupDateTitleDegate(object? groupKey)
        {
            DateTime? episodeTime = (DateTime?) groupKey;

            if (!episodeTime.HasValue)
            {
                return string.Empty;
            }

            if (episodeTime.Value > DateTime.Now)
            {
                return "Future";
            }

            TimeSpan timeSince = DateTime.Now - episodeTime.Value;

            if (timeSince < 7.Days())
            {
                return "This Week";
            }

            if (DateTime.Now.Year == episodeTime.Value.Year && DateTime.Now.Month == episodeTime.Value.Month)
            {
                return "Earlier this Month";
            }

            if (DateTime.Now.Year == episodeTime.Value.Year)
            {
                return episodeTime.Value.ToString("MMMM yyyy");
            }

            return episodeTime.Value.ToString("yyyy");
        }

        private static object? GroupDateKeyDelegate(object rowObject)
        {
            DateTime? episodeTime = ((Item) rowObject).AirDate;

            if (!episodeTime.HasValue)
            {
                return null;
            }

            if (episodeTime.Value > DateTime.Now)
            {
                return DateTime.Now.AddDays(1);
            }

            TimeSpan timeSince = DateTime.Now - episodeTime.Value;

            if (timeSince < 7.Days())
            {
                return DateTime.Now.AddDays(-1);
            }

            if (DateTime.Now.Year == episodeTime.Value.Year && DateTime.Now.Month == episodeTime.Value.Month)
            {
                return DateTime.Now.AddDays(-8);
            }

            if (DateTime.Now.Year == episodeTime.Value.Year)
            {
                return new DateTime(episodeTime.Value.Year, episodeTime.Value.Month, 1);
            }

            return new DateTime(episodeTime.Value.Year, 1, 1);
        }

        [NotNull]
        private string GroupItemsTitleDelegate(object groupKey)
        {
            switch ((string) groupKey)
            {
                case "A-Missing":
                    return HeaderName("Missing", mDoc.TheActionList.Missing.Count);

                case "H-UpdateFiles":
                    return HeaderName("Media Center Metadata", mDoc.TheActionList.Count(item => item is ActionWriteMetadata));

                case "I-UpdateFileDates":
                    return HeaderName("Update File/Directory Metadata", mDoc.TheActionList.Count(item => item is ActionDateTouch));

                case "J-Downloading":
                    return HeaderName("Downloading", mDoc.TheActionList.Count(item => item is ItemDownloading));

                case "G-DownloadImage":
                    return HeaderName("Download", mDoc.TheActionList.SaveImages.Count);

                case "F-DownloadTorrent":
                    return HeaderName("Start/Stop Download", mDoc.TheActionList.Count(action => action is ActionTDownload || action is ActionTRemove));

                case "E-Delete":
                    return HeaderName("Remove", mDoc.TheActionList.Count(action => action is ActionDeleteFile || action is ActionDeleteDirectory));

                case "B-Rename":
                    int renameCount = mDoc.TheActionList.Count(action => action is ActionCopyMoveRename cmr && cmr.Operation == ActionCopyMoveRename.Op.rename);
                    return HeaderName("Rename", renameCount);

                case "C-Copy":
                    List<ActionCopyMoveRename> copyActions = mDoc.TheActionList.CopyMove
                        .Where(cmr => cmr.Operation == ActionCopyMoveRename.Op.copy)
                        .ToList();

                    long copySize = copyActions.Where(item => item.From.Exists).Sum(copy => copy.From.Length);
                    return HeaderName("Copy", copyActions.Count, copySize);

                case "D-Move":
                    List<ActionCopyMoveRename> moveActions = mDoc.TheActionList.CopyMove
                        .Where(cmr => cmr.Operation == ActionCopyMoveRename.Op.move)
                        .ToList();

                    long moveSize = moveActions.Where(item => item.From.Exists).Sum(copy => copy.From.Length);
                    return HeaderName("Move", moveActions.Count, moveSize);
                default:
                    return "UNKNOWN";
            }
        }

        [NotNull]
        private static object GroupItemsKeyDelegate(object rowObject)
        {
            Item i = (Item) rowObject;
            return i.ScanListViewGroup switch
            {
                "lvgActionMissing" => "A-Missing",
                "lvgActionMeta" => "H-UpdateFiles",
                "lvgUpdateFileDates" => "I-UpdateFileDates",
                "lvgDownloading" => "J-Downloading",
                "lvgActionDownload" => "G-DownloadImage",
                "lvgActionDownloadRSS" => "F-DownloadTorrent",
                "lvgActionDelete" => "E-Delete",
                "lvgActionRename" => "B-Rename",
                "lvgActionCopy" => "C-Copy",
                "lvgActionMove" => "D-Move",
                _ => "UNKNOWN"
            };
        }

        private static string ConvertShowNameDelegate(object x)
        {
            string oringinalName = (string) x;
            return PostpendTheIfNeeded(oringinalName);
        }

        private void olv1_FormatRow(object sender, [NotNull] FormatRowEventArgs e)
        {
            if (e.Model is Action a)
            {
                if (a.Outcome.Error)
                {
                    e.Item.BackColor = Helpers.WarningColor();
                }
            }
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
            mDoc.Args.TemporarilyUse(localArgs);

            ProcessArgs(localArgs);

            //Revert the settings
            TVSettings.Instance.RenameCheck = previousRenameBehavior;
            mDoc.Args.RevertFromTempUse();
        }
        private void ScanAndAction(TVSettings.ScanType type)
        {
            UiScan(null,null,true,type,MediaConfiguration.MediaType.both);
            ActionAction(true,true, false);
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
        private void ClearMovieInfoWindows() => ClearMovieInfoWindows("");

        private void ClearInfoWindows(string defaultText)
        {
            SetHtmlBody(webImages, ShowHtmlHelper.CreateOldPage(defaultText));
            SetHtmlBody(webInformation, ShowHtmlHelper.CreateOldPage(defaultText));
            SetHtmlBody(webSummary, ShowHtmlHelper.CreateOldPage(defaultText));
        }

        private void ClearMovieInfoWindows(string defaultText)
        {
            SetHtmlBody(webMovieImages, ShowHtmlHelper.CreateOldPage(defaultText));
            SetHtmlBody(webMovieInformation, ShowHtmlHelper.CreateOldPage(defaultText));
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
                mDoc.DoDownloadsFG(UNATTENDED, WindowState == FormWindowState.Minimized,this);
            }

            if (a.ForceUpdate)
            {
                UIAccuracyCheck(UNATTENDED);
                UITMDBAccuracyCheck(UNATTENDED);
            }

            if (a.ForceRefresh)
            {
                ForceRefresh(mDoc.TvLibrary.GetSortedShowItems(), UNATTENDED);
                ForceMovieRefresh(mDoc.FilmLibrary.Movies, UNATTENDED);
            }

            if (a.Scan)
            {
                UiScan(null,null, UNATTENDED, TVSettings.ScanType.Full,MediaConfiguration.MediaType.both);
            }

            if (a.QuickScan)
            {
                UiScan(null, null, UNATTENDED, TVSettings.ScanType.Quick, MediaConfiguration.MediaType.both);
            }

            if (a.RecentScan)
            {
                UiScan(null, null, UNATTENDED, TVSettings.ScanType.Recent, MediaConfiguration.MediaType.both);
            }

            if (a.DoAll)
            {
                ActionAction(true, UNATTENDED,true);
            }

            if (a.Save)
            {
                mDoc.WriteXMLSettings();
                SaveCaches();
            }

            if (a.Export)
            {
                mDoc.RunExporters();
            }

            if (a.Quit)
            {
                Close();
            }
        }

        // ReSharper disable once InconsistentNaming
        private void UIAccuracyCheck(bool unattended)
        {
            MoreBusy();
            Task.Run(
                () => mDoc.TVDBServerAccuracyCheck(unattended, WindowState == FormWindowState.Minimized,this)
            );
            LessBusy();
        }

        private void UpdateSearchButtons()
        {
            Searchers searchers = GetUsedSearchers();
            string name = searchers.CurrentSearch.Name;
            bool enabled = name.HasValue() && searchers.Count>0;

            btnScheduleBTSearch.Enabled = enabled;
            btnScheduleBTSearch.Text = UseCustom(lvWhenToWatch) ? "Search" : name;

            btnActionBTSearch.Enabled = enabled;
            btnActionBTSearch.Text = UseCustomObject(olvAction) ? "Search" : name;
        }

        private Searchers GetUsedSearchers()
        {
            bool usingScanResults = tabControl1.SelectedTab == tbAllInOne;
            bool haveTvSelected = !usingScanResults || GetSelectedObjectType(olvAction) == MediaConfiguration.MediaType.tv;

            Searchers searchers = haveTvSelected ? TVDoc.GetSearchers() : TVDoc.GetMovieSearchers();
            return searchers;
        }

        private MediaConfiguration.MediaType GetSelectedObjectType(ObjectListView list)
        {
            if (list.SelectedObjects.Count == list.SelectedObjects.OfType<MovieItemMissing>().Count())
                return MediaConfiguration.MediaType.movie;

            if (list.SelectedObjects.Count == list.SelectedObjects.OfType<ShowItemMissing>().Count())
                return MediaConfiguration.MediaType.tv;

            return MediaConfiguration.MediaType.both;
        }

        private void visitWebsiteToolStripMenuItem_Click(object sender, EventArgs eventArgs) =>
            Helpers.OpenUrl("http://tvrename.com");

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Close();

        private static bool UseCustomObject([NotNull] ObjectListView view)
        {
            return view.SelectedObjects.OfType<ProcessedEpisode>().Any(episode =>
                episode.Show.UseCustomSearchUrl && episode.Show.CustomSearchUrl.HasValue());
        }

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

            SetupFilterButton(filterTextBox,filterButton_Click);
            SetupFilterButton(filterMoviesTextbox, filterMovieButton_Click);

            UpdateVisibilityFromSettings();
            EnableDisableAccessibilty();

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

            TriggerAppUpdateCheck();

            quickTimer.Start();

            if (TVSettings.Instance.RunOnStartUp())
            {
                RunAutoScan("Startup Scan");
            }
        }

        private void TriggerAppUpdateCheck()
        {
            var updateCheckType = TVSettings.Instance.UpdateCheckType;
            if (updateCheckType != TVSettings.UpdateCheckMode.Off)
            {
                var checkUpdate = true;
                if (updateCheckType == TVSettings.UpdateCheckMode.Interval)
                {
                    var lastUpdate = mDoc.CurrentAppState.UpdateCheck.LastUpdate;
                    var interval = TVSettings.Instance.UpdateCheckInterval;
                    checkUpdate = lastUpdate >= interval;
                }

                if (checkUpdate)
                {
                    UpdateTimer.Start();
                }
            }

        }

        private static void SetupFilterButton(TextBox textBox, EventHandler handler)
        {
            // MAH: Create a "Clear" button in the Filter Text Box
            Button filterButton = new Button
            {
                Size = new Size(16, 16),
                Cursor = Cursors.Default,
                Image = Resources.DeleteSmall,
                Name = "Clear"
            };

            int clientSizeHeight = (textBox.ClientSize.Height - 16) / 2;
            filterButton.Location = new Point(textBox.ClientSize.Width - filterButton.Width, clientSizeHeight + 1);

            filterButton.Click += handler;
            textBox.Controls.Add(filterButton);
            // Send EM_SETMARGINS to prevent text from disappearing underneath the button
            NativeMethods.SendMessage(textBox.Handle, 0xd3, (IntPtr) 2, (IntPtr) (filterButton.Width << 16));
        }

        private void UpdateVisibilityFromSettings()
        {
            betaToolsToolStripMenuItem.Visible = TVSettings.Instance.IncludeBetaUpdates();
            tbActionJackettSearch.Visible = TVSettings.Instance.SearchJackettButton;
            tsbScheduleJackettSearch.Visible = TVSettings.Instance.SearchJackettButton;
        }

        private void EnableDisableAccessibilty()
        {
            tsbMyShowsContextMenu.Visible = TVSettings.Instance.ShowAccessibilityOptions;
            tsbScanContextMenu.Visible = TVSettings.Instance.ShowAccessibilityOptions;
            btnScheduleRightClick.Visible = TVSettings.Instance.ShowAccessibilityOptions;
            toolStripSeparator12.Visible = TVSettings.Instance.ShowAccessibilityOptions;
            toolStripSeparator13.Visible = TVSettings.Instance.ShowAccessibilityOptions;
            toolStripSeparator14.Visible = TVSettings.Instance.ShowAccessibilityOptions;
        }

        // MAH: Added in support of the Filter TextBox Button
        private void filterButton_Click(object sender, EventArgs e) => filterTextBox.Clear();
        private void filterMovieButton_Click(object sender, EventArgs e) => filterMoviesTextbox.Clear();

        private ListView ListViewByName([NotNull] string name)
        {
            switch (name)
            {
                case "WhenToWatch":
                    return lvWhenToWatch;
                case "AllInOne":
                    return olvAction;
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

            UpdateImages(mDoc.TvLibrary.GetSortedShowItems());
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
                "locally stored TheTVDB, TMDB and TV Maze information?  This information will have to be downloaded again.  You " +
                "can force the refresh of a single show by holding down the \"Control\" key while clicking on " +
                "the \"Refresh\" button in the \"My Shows\" tab.",
                "Force Refresh All", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (res == DialogResult.Yes)
            {
                TheTVDB.LocalCache.Instance.ForgetEverything();
                TVmaze.LocalCache.Instance.ForgetEverything();
                TMDB.LocalCache.Instance.ForgetEverything();
                FillMyShows();
                FillMyMovies();
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

            string actionLayout = x.Descendants("Layout").Descendants("ActionLayout").First().Attribute("State")?.Value;
            if (actionLayout != null)
            {
                if (actionLayout.HasValue())
                {
                    olvAction.RestoreState(Convert.FromBase64String(actionLayout));
                }
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
            XAttribute? valueX = x.Attribute("X");
            XAttribute? valueY = x.Attribute("Y");

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
            XAttribute? valueX = x.Attribute("Width");
            XAttribute? valueY = x.Attribute("Height");

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

                writer.WriteStartElement("ActionLayout");
                writer.WriteAttributeToXml("State", Convert.ToBase64String(olvAction.SaveState()));
                writer.WriteEndElement(); // ActionLayout
                
                writer.WriteEndElement(); // Layout
                writer.WriteEndElement(); // tvrename
                writer.WriteEndDocument();
            }

            return true;
        }

        private void WriteColWidthsXml([NotNull] string thingName, XmlWriter writer)
        {
            ListView lv = ListViewByName(thingName);

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
                if (mDoc.Dirty() && !mDoc.Args.Unattended && !mDoc.Args.Hide && !TVSettings.Instance.AutoSaveOnExit)
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

                if (mDoc.Dirty() && (mDoc.Args.Unattended || mDoc.Args.Hide || TVSettings.Instance.AutoSaveOnExit))
                {
                    //We have to assume that they wanted to save any settings
                    mDoc.WriteXMLSettings();
                }

                if (!e.Cancel)
                {
                    SaveLayoutXml();
                    mDoc.TidyCaches();
                    SaveCaches();
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

        private void ChooseSiteMenu([NotNull] ToolStripSplitButton btn)
        {
            btn.DropDownItems.Clear();
            Searchers searchers = GetUsedSearchers();

            foreach (SearchEngine search in searchers.Where(engine => engine.Name.HasValue()))
            {
                ToolStripMenuItem tsi = new ToolStripMenuItem(search.Name) {Tag = search};
                tsi.Font = new Font(tsi.Font.FontFamily,9,FontStyle.Regular);
                btn.DropDownItems.Add(tsi);
            }
        }

        private void FillMyShows()
        {
            ProcessedSeason currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
            ShowConfiguration currentSi = TreeNodeToShowItem(MyShowTree.SelectedNode);

            List<ShowConfiguration> expanded = new List<ShowConfiguration>();
            foreach (TreeNode n in MyShowTree.Nodes)
            {
                if (n.IsExpanded)
                {
                    expanded.Add(TreeNodeToShowItem(n));
                }
            }

            MyShowTree.BeginUpdate();

            MyShowTree.Nodes.Clear();
            List<ShowConfiguration> sil = mDoc.TvLibrary.Values.ToList();
            sil.Sort((a, b) => string.Compare(GenerateShowUIName(a), GenerateShowUIName(b), StringComparison.OrdinalIgnoreCase));

            ShowFilter filter = TVSettings.Instance.Filter;
            foreach (ShowConfiguration si in sil)
            {
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

            foreach (TreeNode n in MyShowTree.Nodes)
            {
                ShowConfiguration? showAtNode = TreeNodeToShowItem(n);
                if ( expanded.Contains(showAtNode))
                {
                    n.Expand();
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

        private void FillMyMovies()
        {
            MovieConfiguration currentMovie = TreeNodeToMovieItem(movieTree.SelectedNode);

            movieTree.BeginUpdate();

            movieTree.Nodes.Clear();
            List<MovieConfiguration> sil = mDoc.FilmLibrary.Values.ToList();
            sil.Sort((a, b) => string.Compare(GenerateShowUiName(a), GenerateShowUiName(b), StringComparison.OrdinalIgnoreCase));

            MovieFilter filter = TVSettings.Instance.MovieFilter;
            foreach (MovieConfiguration si in sil)
            {
                if (filter.Filter(si) & (string.IsNullOrEmpty(filterMoviesTextbox.Text) || si.NameMatchFilters(filterMoviesTextbox.Text)))
                {
                    AddMovieToTree(si);
                }
            }

            if (currentMovie != null)
            {
                SelectMovie(currentMovie);
            }

            movieTree.EndUpdate();
        }

        public static string GenerateShowUiName(MovieConfiguration show)
        {
            return PostpendTheIfNeeded(show.ShowName);
        }

        [NotNull]
        private static string QuickStartGuide() => "https://www.tvrename.com/manual/quickstart/";

        private void ShowQuickStartGuide()
        {
            tabControl1.SelectTab(tbMyShows);

            if (webInformation.IsDisposed || webImages.IsDisposed || webSummary.IsDisposed)
            {
                return;
            }

            try
            {
                webInformation.Navigate(QuickStartGuide());
                webImages.Navigate(QuickStartGuide());
                webSummary.Navigate(QuickStartGuide());
            }
            catch (COMException ex)
            {
                //Fail gracefully - no RHS episode guide is not too big of a problem.
                Logger.Warn(ex, "Could not update UI for the QuickStart Guide");
            }
            catch (Exception ex)
            {
                //Fail gracefully - no RHS episode guide is not too big of a problem.
                Logger.Error(ex);
            }
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

        private void FillMovieGuideHtml()
        {
            if (movieTree.Nodes.Count == 0)
            {
                ShowQuickStartGuide();
            }
            else
            {
                TreeNode n = movieTree.SelectedNode;
                FillMovieGuideHtml(TreeNodeToMovieItem(n));
            }
        }

        private static MovieConfiguration? TreeNodeToMovieItem(TreeNode? n)
        {
            if (n is null)
            {
                return null;
            }

            return n.Tag switch
            {
                MovieConfiguration i => i,
                _ => null
            };
        }

        private static ShowConfiguration? TreeNodeToShowItem(TreeNode? n)
        {
            if (n is null)
            {
                return null;
            }

            return n.Tag switch
            {
                ShowConfiguration si => si,
                ProcessedEpisode pe => pe.Show,
                ProcessedSeason seas => seas.Show,
                _ => null
            };
        }

        private static ProcessedSeason? TreeNodeToSeason(TreeNode? n)
        {
            ProcessedSeason seas = n?.Tag as ProcessedSeason;
            return seas;
        }

        private void FillEpGuideHtml(TreeNode? n)
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

        private void FillEpGuideHtml(ShowConfiguration? si, int snum)
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

            if (si.CachedShow is null)
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
                    SetHtmlBody(webSummary, ShowHtmlHelper.CreateOldPage("Not available offline"));
                }
                else
                {
                    // no epnum specified, just show an overview
                    SetHtmlBody(webInformation, ShowHtmlHelper.CreateOldPage(si.GetShowHtmlOverviewOffline()));
                    SetHtmlBody(webImages, ShowHtmlHelper.CreateOldPage(si.GetShowImagesHtmlOverview()));
                    SetHtmlBody(webSummary, ShowHtmlHelper.CreateOldPage("Not available offline"));
                }

                return; 
            }

            if (snum >= 0 && si.AppropriateSeasons().ContainsKey(snum))
            {
                ProcessedSeason s = si.AppropriateSeasons()[snum];
                SetHtmlBody(webImages, ShowHtmlHelper.CreateOldPage(si.GetSeasonImagesHtmlOverview(s)));
                SetHtmlBody(webInformation, si.GetSeasonHtmlOverview(s, false));
                SetHtmlBody(webSummary, si.GetSeasonSummaryHtmlOverview(s, false));

                if (bwSeasonHTMLGenerator.WorkerSupportsCancellation)
                {
                    // Cancel the asynchronous operation.
                    bwSeasonHTMLGenerator.CancelAsync();
                }

                if (!bwSeasonHTMLGenerator.IsBusy)
                {
                    bwSeasonHTMLGenerator.RunWorkerAsync(s);
                }

                if (bwSeasonSummaryHTMLGenerator.WorkerSupportsCancellation)
                {
                    // Cancel the asynchronous operation.
                    bwSeasonSummaryHTMLGenerator.CancelAsync();
                }

                if (!bwSeasonSummaryHTMLGenerator.IsBusy)
                {
                    bwSeasonSummaryHTMLGenerator.RunWorkerAsync(s);
                }
            }
            else
            {
                // no epnum specified, just show an overview
                SetHtmlBody(webImages, ShowHtmlHelper.CreateOldPage(si.GetShowImagesHtmlOverview()));
                SetHtmlBody(webInformation, si.GetShowHtmlOverview(false));
                SetHtmlBody(webSummary, si.GetShowSummaryHtmlOverview(false));

                if (bwShowHTMLGenerator.WorkerSupportsCancellation)
                {
                    // Cancel the asynchronous operation.
                    bwShowHTMLGenerator.CancelAsync();
                }

                if (!bwShowHTMLGenerator.IsBusy)
                {
                    bwShowHTMLGenerator.RunWorkerAsync(si);
                }

                if (bwShowSummaryHTMLGenerator.WorkerSupportsCancellation)
                {
                    // Cancel the asynchronous operation.
                    bwShowSummaryHTMLGenerator.CancelAsync();
                }

                if (!bwShowSummaryHTMLGenerator.IsBusy)
                {
                    bwShowSummaryHTMLGenerator.RunWorkerAsync(si);
                }
            }
        }

        private static void SetHtmlBody([NotNull] WebBrowser web, string body)
        {
            if (web.IsDisposed)
            {
                return;
            }

            try
            {
                web.DocumentText = body; 
            }
            catch (COMException ex)
            {
                //Fail gracefully - no RHS episode guide is not too big of a problem.
                Logger.Warn(ex,"Could not update UI for the show/cachedSeries information pane");
            }
            catch (Exception ex)
            {
                //Fail gracefully - no RHS episode guide is not too big of a problem.
                Logger.Error(ex);
            }
            web.Update();
        }

        private static void TvSourceFor(ProcessedEpisode? e)
        {
            if (e?.WebsiteUrl != null)
            {
                Helpers.OpenUrl(e.WebsiteUrl);
            }
        }

        private static void TvSourceFor(ProcessedSeason? seas)
        {
            if (seas?.WebsiteUrl != null)
            {
                Helpers.OpenUrl(seas.WebsiteUrl);
            }
        }

        private static void TvSourceFor( ShowConfiguration? si)
        {
            if (si != null)
            {
                if (si.WebsiteUrl.HasValue())
                {
                    Helpers.OpenUrl(si.WebsiteUrl!);
                }
                else if (si.CachedShow?.WebUrl.HasValue()??false)
                {
                    Helpers.OpenUrl(si.CachedShow?.WebUrl!);
                }
            }
        }

        private void menuSearchSites_ItemClicked(object sender, [NotNull] ToolStripItemClickedEventArgs e)
        {
            if (GetUsedSearchers() == TVDoc.GetMovieSearchers())
            {
                mDoc.SetMovieSearcher((SearchEngine) e.ClickedItem.Tag);
            }
            else
            {
                mDoc.SetSearcher((SearchEngine)e.ClickedItem.Tag);
            }

            UpdateSearchButtons();
        }

        private void bnWhenToWatchCheck_Click(object? sender, EventArgs? e) => RefreshWTW(true,false);

        private void FillWhenToWatchList()
        {
            if (bwUpdateSchedule.WorkerSupportsCancellation)
            {
                // Cancel the asynchronous operation.
                bwUpdateSchedule.CancelAsync();
            }

            if (!bwUpdateSchedule.IsBusy)
            {
                bwUpdateSchedule.RunWorkerAsync();
            }
        }

        [NotNull]
        private List<ListViewItem> GenerateNewScheduleItems()
        {
            int dd = TVSettings.Instance.WTWRecentDays;
            DirFilesCache dfc = new DirFilesCache();

            IEnumerable<ProcessedEpisode> recentEps = mDoc.TvLibrary.GetRecentAndFutureEps(dd);
            return recentEps.Select(ei => GenerateLvi(dfc, ei)).ToList();
        }

        [NotNull]
        private ListViewItem GenerateLvi(DirFilesCache dfc, [NotNull] ProcessedEpisode pe)
        {
            ListViewItem lvi = new ListViewItem
            {
                Text = GenerateShowUIName(pe),
                Tag = pe
            };

            DateTime? airdt = pe.GetAirDateDt(true);
            if (airdt is null)
            {
                return lvi;
            }
            DateTime dt = airdt.Value;

            lvi.Group = lvWhenToWatch.Groups[CalculateWtwlviGroup(pe, dt)];

            lvi.SubItems.Add(pe.SeasonNumberAsText);
            lvi.SubItems.Add(GetEpisodeNumber(pe));
            lvi.SubItems.Add(dt.ToShortDateString());
            lvi.SubItems.Add(dt.ToString("t"));
            lvi.SubItems.Add(dt.ToString("ddd"));
            lvi.SubItems.Add(pe.HowLong());
            lvi.SubItems.Add(pe.TheCachedSeries.Network);
            lvi.SubItems.Add(pe.Name);

            // icon..
            int? iconNumbers = ChooseWtwIcon(dfc, pe, dt);
            if (iconNumbers != null)
            {
                lvi.ImageIndex = iconNumbers.Value;
            }

            if (TVSettings.Instance.UseColoursOnWtw)
            {
                (Color back, Color fore) = GetWtwColour(pe, dt);
                lvi.BackColor = back;
                lvi.ForeColor = fore;
            }
            return lvi;
        }

        private void lvWhenToWatch_ColumnClick(object sender, [NotNull] ColumnClickEventArgs e)
        {
            int col = e.Column;
            SortSchedule(col);
        }

        private void SortSchedule(int col)
        {
            // 3 - 6 = do date sort on 3
            // 1 or 2 = number sort
            // all others, text sort

            lvwScheduleColumnSorter.ClickedOn(col);
            lvWhenToWatch.ShowGroups = false;

            switch (col)
            {
                case 3:
                case 4:
                case 5:
                case 6:
                    lvWhenToWatch.ShowGroups = true;
                    lvwScheduleColumnSorter.ListViewItemSorter = new DateSorterWtw(col);
                    break;

                case 1:
                case 2:
                    lvwScheduleColumnSorter.ListViewItemSorter = new NumberAsTextSorter(col);
                    break;

                default:
                    lvwScheduleColumnSorter.ListViewItemSorter = new TextSorter(col);
                    break;
            }

            lvWhenToWatch.BeginUpdate();
            lvWhenToWatch.Sort();
            lvWhenToWatch.Refresh();
            lvWhenToWatch.EndUpdate();
        }

        private void lvWhenToWatch_Click(object sender, EventArgs e)
        {
            UpdateSearchButtons();

            if (lvWhenToWatch.SelectedIndices.Count == 0)
            {
                txtWhenToWatchSynopsis.Text = string.Empty;
                switchToWhenOpenMyShows = null;
                return;
            }

            int n = lvWhenToWatch.SelectedIndices[0];

            ProcessedEpisode ei = (ProcessedEpisode) lvWhenToWatch.Items[n].Tag;
            switchToWhenOpenMyShows = ei;

            if (TVSettings.Instance.HideWtWSpoilers &&
                (ei.HowLong() != "Aired" || lvWhenToWatch.Items[n].ImageIndex == 1))
            {
                txtWhenToWatchSynopsis.Text = Resources.Spoilers_Hidden_Text;
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
                Helpers.OpenFile(fl[0].FullName);
                return;
            }

            // Don't have the episode.  Scan or search?

            switch (TVSettings.Instance.WTWDoubleClick)
            {
                case TVSettings.WTWDoubleClickAction.Search:
                    bnWTWBTSearch_Click(null, null);
                    break;
                case TVSettings.WTWDoubleClickAction.Scan:
                    UiScan(new List<ShowConfiguration> {ei.Show},null, false, TVSettings.ScanType.SingleShow,MediaConfiguration.MediaType.tv);
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
                if (!mDoc.DoDownloadsFG(unattended,WindowState==FormWindowState.Minimized,this))
                {
                    return;
                }
            }

            mInternalChange++;
            mDoc.DoWhenToWatch(true,unattended,WindowState==FormWindowState.Minimized,this);
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
            List<ProcessedEpisode> next1 = mDoc.TvLibrary.NextNShows(1, 0, 36500);

            tsNextShowTxt.Text = "Next airing: ";
            if (next1.Count >= 1)
            {
                ProcessedEpisode ei = next1[0];
                tsNextShowTxt.Text += $"{CustomEpisodeName.NameForNoExt(ei, CustomEpisodeName.OldNStyle(1))}, {ei.HowLong()} ({ei.DayOfWeek()}, {ei.TimeOfDay()})";
            }
            else
            {
                tsNextShowTxt.Text += "---";
            }
        }

        private void bnWTWBTSearch_Click(object? sender, EventArgs? e)
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
                string openlocation = HttpUtility.UrlDecode(url.Substring(EXPLORE_PROXY.Length));
                if (Helpers.OpenFolder(openlocation))
                {
                    return;
                }
                Helpers.OpenFolderSelectFile(openlocation);
                return;
            }

            if (url.StartsWith(WATCH_PROXY, StringComparison.InvariantCultureIgnoreCase))
            {
                e.Cancel = true;
                string fileName = HttpUtility.UrlDecode(url.Substring(WATCH_PROXY.Length)).Replace('/', '\\');
                Helpers.OpenFile(fileName);
                return;
            }

            if (url.IsHttpLink() || url.IsFileLink())
            {
                e.Cancel = true;
                Helpers.OpenUrl(e.Url.AbsoluteUri);
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
            bmad.ShowDialog(this);
        }

        public void GotoEpguideFor(ShowConfiguration si, bool changeTab)
        {
            if (changeTab)
            {
                tabControl1.SelectTab(tbMyShows);
            }
            SelectShow(si);
            if (changeTab)
            {
                tabControl2.SelectTab(tpSummary);
            }
        }

        public void GotoEpguideFor([NotNull] ProcessedEpisode ep, bool changeTab)
        {
            if (changeTab)
            {
                tabControl1.SelectTab(tbMyShows);
            }

            SelectSeason(ep.AppropriateProcessedSeason);
        }

        public void GotoMovieFor([NotNull] MovieConfiguration mc, bool changeTab)
        {
            if (changeTab)
            {
                tabControl1.SelectTab(tbMyMovies);
            }

            SelectMovie(mc);
        }

        private void RightClickOnMyShows(ShowConfiguration si, Point pt)
        {
            BuildRightClickMenu(pt,null, new List<ShowConfiguration> { si },null);
        }

        private void RightClickOnMyShows([NotNull] ProcessedSeason seas, Point pt)
        {
            BuildRightClickMenu(pt,null, new List<ShowConfiguration> { seas.Show },seas);
        }

        private void WtwRightClickOnShow([NotNull] List<ProcessedEpisode> eps, Point pt)
        {
            if (eps.Count == 0)
            {
                return;
            }

            ProcessedEpisode ep = eps.Count ==1?eps[0]:null;

            List<ShowConfiguration> sis = eps.Select(e => e.Show).ToList();

            BuildRightClickMenu(pt, ep,sis, ep?.AppropriateProcessedSeason);
        }

        private void MenuGuideAndTvdb(bool addSep, ProcessedEpisode? ep, List<ShowConfiguration?>? sis, ProcessedSeason? seas)
        {
            if (sis is null || sis.Count != 1 || sis[0]==null)
            {
                return; // nothing or multiple selected
            }

            ShowConfiguration? si = sis[0];

            if (addSep)
            {
                showRightClickMenu.Items.Add(new ToolStripSeparator());
            }

            if (ep != null)
            {
                AddRcMenuItem("Episode Guide", (sender, args) => GotoEpguideFor(ep,true));

                string label = ep.Show.Provider == TVDoc.ProviderType.TVmaze
                    ? "Visit Tv Maze..."
                    : "Visit thetvdb.com";
                AddRcMenuItem(label, (sender, args) => TvSourceFor(ep));
            }
            else if (seas != null)
            {
                AddRcMenuItem("Episode Guide", (sender, args) => GotoEpguideFor(seas.Show, true));
                string label = seas.Show.Provider == TVDoc.ProviderType.TVmaze
                    ? "Visit Tv Maze..."
                    : "Visit thetvdb.com";
                AddRcMenuItem(label, (sender, args) => TvSourceFor(seas));
            }
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            else if (si!=null)
            {
                AddRcMenuItem("Episode Guide", (sender, args) => GotoEpguideFor(si, true));
                string label = si.Provider == TVDoc.ProviderType.TVmaze
                    ? "Visit Tv Maze..."
                    : "Visit thetvdb.com";
                AddRcMenuItem(label, (sender, args) => TvSourceFor(si));
            }
        }

        private void MenuShowAndEpisodes(List<ShowConfiguration> sil ,ProcessedSeason? seas, ProcessedEpisode? ep)
        {
            ShowConfiguration si = sil.Count >= 1 ?sil[0]:null;

            if (si != null)
            {
                AddRcMenuItem("Force Refresh", (sender, args) => ForceRefresh(sil, false));
                AddRcMenuItem("Update Images", (sender, args) => UpdateImages(sil));
                showRightClickMenu.Items.Add(new ToolStripSeparator());

                string scanText = sil.Count > 1
                    ? "Scan Multiple Shows"
                    : "Scan \"" + si.ShowName + "\"";

                AddRcMenuItem(scanText, (sender, args) => 
                    {
                        UiScan(sil,null, false, TVSettings.ScanType.SingleShow,MediaConfiguration.MediaType.tv);
                        tabControl1.SelectTab(tbAllInOne);
                    }
                );

                if (sil.Count == 1)
                {
                    AddRcMenuItem("Schedule", (sender, args) => GotoWtwFor(si.TvdbCode));
                    AddRcMenuItem("Edit Show", (sender, args) => EditShow(si));
                    AddRcMenuItem("Delete Show", (sender, args) => DeleteShow(si));
                }
            }

            if (seas != null && si != null && sil.Count == 1)
            {
                AddRcMenuItem("Edit " + ProcessedSeason.UIFullSeasonWord(seas.SeasonNumber), (sender, args) => EditSeason(si, seas.SeasonNumber));
                if (si.IgnoreSeasons.Contains(seas.SeasonNumber))
                {
                    AddRcMenuItem("Include " + ProcessedSeason.UIFullSeasonWord(seas.SeasonNumber),(sender, args) => IncludeSeason(si, seas.SeasonNumber));
                }
                else
                {
                    AddRcMenuItem("Ignore " + ProcessedSeason.UIFullSeasonWord(seas.SeasonNumber),(sender, args) => IgnoreSeason(si,seas.SeasonNumber));
                }
            }

            if (ep != null && sil.Count == 1)
            {
                List<FileInfo> fl = FinderHelper.FindEpOnDisk(null, ep);
                if (fl.Count <= 0)
                {
                    return;
                }

                showRightClickMenu.Items.Add(new ToolStripSeparator());

                foreach (FileInfo fi in fl)
                {
                    ToolStripMenuItem tsi = new ToolStripMenuItem("Watch: " + fi.FullName);
                    tsi.Click += (sender, args) => { Helpers.OpenFile(fi.FullName); };
                    showRightClickMenu.Items.Add(tsi);
                }
            }
            else if (seas != null && si != null && sil.Count == 1)
            {
                ToolStripMenuItem tsis = new ToolStripMenuItem("Watch Episodes");

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

                        foreach (FileInfo fi in fl)
                        {
                            ToolStripMenuItem tsisi = new ToolStripMenuItem("Watch: " + fi.FullName);
                            tsisi.Click += (s, ev) => { Helpers.OpenFile(fi.FullName); };

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

        private void MenuFolders(ItemList? lvr, ShowConfiguration? si, ProcessedSeason? seas, ProcessedEpisode? ep)
        {
            List<string> added = new List<string>();

            if (ep != null)
            {
                Dictionary<int, SafeList<string>> afl = ep.Show.AllExistngFolderLocations();
                if (afl.ContainsKey(ep.AppropriateSeasonNumber))
                {
                    AddFolders(afl[ep.AppropriateSeasonNumber], added);
                }
            }
            else if (seas != null && si != null)
            {
                Dictionary<int, SafeList<string>> folders = si.AllExistngFolderLocations();

                if (folders.ContainsKey(seas.SeasonNumber))
                {
                    AddFolders(folders[seas.SeasonNumber], added);
                }
            }

            if (si != null)
            {
                if (si.AutoAddFolderBase.HasValue())
                {
                    AddFolders(new[] {si.AutoAddFolderBase}, added);
                }
                AddFoldersSubMenu(si.AllExistngFolderLocations().Values.SelectMany(l => l).ToList(), added);
            }

            if (lvr is null || lvr.Count>1)
            {
                return;
            }

            {
                AddFoldersSubMenu(lvr.Select(sli =>sli.TargetFolder),added);
            }
        }

        private void AddFolders([NotNull] IEnumerable<string> foldersList,[NotNull] ICollection<string> alreadyAdded)
        {
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
                tsi.Click += (s, ev) => {
                    Helpers.OpenFolder(folder);
                };

                showRightClickMenu.Items.Add(tsi);
            }
        }

        private void AddFoldersSubMenu([NotNull] IEnumerable<string> foldersList,[NotNull] ICollection<string> alreadyAdded)
        {
            ToolStripMenuItem tsi = new ToolStripMenuItem("Open Other Folders" );

            foreach (string folder in foldersList.Where(folder => !string.IsNullOrEmpty(folder) && Directory.Exists(folder) && !alreadyAdded.Contains(folder)))
            {
                alreadyAdded.Add(folder); // don't show the same folder more than once

                ToolStripMenuItem tssi = new ToolStripMenuItem("Open: " + folder);
                tssi.Click += (s, ev) => {
                    Helpers.OpenFolder(folder); 
                };
                tsi.DropDownItems.Add(tssi);
            }

            if (tsi.DropDownItems.Count > 0)
            {
                showRightClickMenu.Items.Add(new ToolStripSeparator());
                showRightClickMenu.Items.Add(tsi);
            }
        }


        private void RightClickOnMyMovies(MovieConfiguration si, Point pt)
        {
            showRightClickMenu.Items.Clear();

            List<string> added = new List<string>();

            AddRcMenuItem("Force Refresh", (sender, args) => ForceMovieRefresh(si, false));
            AddRcMenuItem("Update Images", (sender, args) => UpdateImages(new List<MovieConfiguration>{si}));

            showRightClickMenu.Items.Add(new ToolStripSeparator());

            AddRcMenuItem("Edit Movie", (sender, args) => EditMovie(si));
            AddRcMenuItem("Delete Movie", (sender, args) => DeleteMovie(si));

            showRightClickMenu.Items.Add(new ToolStripSeparator());

            AddRcMenuItem($"Scan \"{si.ShowName}\"", (sender, args) =>
            {
                UiScan(null, new List<MovieConfiguration> {si}, false, TVSettings.ScanType.SingleShow,
                    MediaConfiguration.MediaType.movie);

                tabControl1.SelectTab(tbAllInOne);
            });
            
            if (si.Locations.Any())
            {
                AddFolders(si.Locations, added);
            }

            showRightClickMenu.Show(pt);
        }
        private void BuildRightClickMenu(Point pt, ProcessedEpisode? ep, List<ShowConfiguration> sis, ProcessedSeason? seas)
        {
            showRightClickMenu.Items.Clear();

            MenuGuideAndTvdb(false,  ep,  sis,  seas);
            MenuShowAndEpisodes(sis,seas,ep);
            if (sis.Count == 1)
            {
                MenuFolders(null, sis[0], seas, ep);
            }

            if (ep !=null)
            {
                MenuSearchFor(ep);
            }

            showRightClickMenu.Show(pt);
        }

        private void showRightClickMenu_ItemClicked(object sender, [NotNull] ToolStripItemClickedEventArgs e)
        {
            showRightClickMenu.Close();
        }

        [NotNull]
        private ItemList GetSelectedItems() => new ItemList {olvAction.SelectedObjects.OfType<Item>()};

        [NotNull]
        private ItemList GetCheckedItems() => new ItemList { olvAction.CheckedObjects.OfType<Item>() };

        private void IncludeSeason([NotNull] ShowConfiguration si, int seasonNumber)
        {
            si.IgnoreSeasons.Remove(seasonNumber);
            ShowAddedOrEdited(false, false);
        }

        private void IgnoreSeason([NotNull] ShowConfiguration si, int seasonNumber)
        {
            si.IgnoreSeasons.Add(seasonNumber);
            ShowAddedOrEdited(false, false);
        }

        private void BrowseForMissingItem(ItemMissing? mi)
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

            if (openFile.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            ManuallyAddFileForItem(mi, openFile.FileName);
        }

        private void ManuallyAddFileForItem([NotNull] ItemMissing mi, string fileName)
        {
            mDoc.UpdateMissingAction(mi, fileName);
            FillActionList(true);
        }

        private void IgnoreSelectedSeasons(IEnumerable<Item>? actions)
        {
            if (actions == null)
            {
                return;
            }

            foreach (Item ai in actions)
            {
                mDoc.IgnoreSeasonForItem(ai);
            }

            FillMyShows();
            FillActionList(true);
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
                lvi.Selected = ei != null && ei.TheCachedSeries.TvdbCode == tvdbSeriesCode;
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
            else if (tabControl1.SelectedTab == tbMyMovies)
            {
                ForceMovieRefresh(false);
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
            if (pref.ShowDialog(this) == DialogResult.OK)
            {
                mDoc.SetDirty();
                ShowHideNotificationIcon();
                FillWhenToWatchList();
                ShowInTaskbar = TVSettings.Instance.ShowInTaskbar;
                EnableDisableAccessibilty();
                FillEpGuideHtml();
                mAutoFolderMonitor.SettingsChanged(TVSettings.Instance.MonitorFolders);
                UpdateVisibilityFromSettings();
                ForceRefresh(false);
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

        private void UI_SizeChanged(object? sender, EventArgs? e)
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

        private void UI_LocationChanged(object? sender, EventArgs? e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                mLastNonMaximizedLocation = Location;
            }
        }

        private void statusTimer_Tick(object? sender, EventArgs? e)
        {
            int n = mDoc.DownloadsRemaining();
            string? dlTask = TheTVDB.LocalCache.Instance.CurrentDLTask ??
                             TVmaze.LocalCache.Instance.CurrentDLTask ?? TMDB.LocalCache.Instance.CurrentDLTask;
            bool somethingDownloading = dlTask.HasValue();

            txtDLStatusLabel.Visible = n != 0 || TVSettings.Instance.BGDownload || somethingDownloading;

            backgroundDownloadNowToolStripMenuItem.Enabled = n == 0;

            if (somethingDownloading)
            {
                txtDLStatusLabel.Text = "Background download: " + dlTask;
            }
            else
            {
                txtDLStatusLabel.Text = "Background download: Idle";
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
            TMDB.LocalCache.Instance.SaveCache();
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

            Task<ServerRelease> tuv = VersionUpdater.CheckForUpdatesAsync();
            ServerRelease result = await tuv.ConfigureAwait(false);

            mDoc.CurrentAppState.UpdateCheck.LastUpdateCheckUtc = DateTime.UtcNow;
            try
            {
                mDoc.CurrentAppState.SaveToDefaultFile();
            }
            catch(Exception ex)
            {
                Logger.Warn(ex, "Could not save app state file after update check!");
            }

            uiDisp.Invoke(() => NotifyUpdates(result, false, mDoc.Args.Unattended || mDoc.Args.Hide));
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

        private void tabControl1_SelectedIndexChanged(object? sender, EventArgs? e)
        {
            if (tabControl1.SelectedTab == tbMyShows)
            {
                if (switchToWhenOpenMyShows != null && TVSettings.Instance.AutoSelectShowInMyShows)
                {
                    GotoEpguideFor(switchToWhenOpenMyShows, false);
                    switchToWhenOpenMyShows = null; //disable switching to this episode again
                    FillEpGuideHtml();
                }

                UpdateMyShowsButtonStatus();
            }
            if (tabControl1.SelectedTab == tbMyMovies)
            {
                if (switchToWhenOpenMyMovies != null && TVSettings.Instance.AutoSelectShowInMyShows)
                {
                    GotoMovieFor(switchToWhenOpenMyMovies, false);
                    switchToWhenOpenMyMovies = null; //disable switching to this movie again
                    //FillEpGuideHtml();
                }

                UpdateMyMoviesButtonStatus();
            }
        }

        private void bugReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BugReport br = new BugReport(mDoc);
            br.ShowDialog(this);
        }

        private void ShowHideNotificationIcon()
        {
            notifyIcon1.Visible = TVSettings.Instance.NotificationAreaIcon && !mDoc.Args.Hide;
        }

        private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StatsWindow sw = new StatsWindow(mDoc.Stats());
            sw.ShowDialog(this);
        }

        [NotNull]
        private TreeNode AddShowItemToTree([NotNull] ShowConfiguration si)
        {
            CachedSeriesInfo ser = si.CachedShow;

            TreeNode n = new TreeNode(GenerateShowUIName(ser,si)) {Tag = si};

            if (ser != null)
            {
                if (TVSettings.Instance.ShowStatusColors.AppliesTo(si))
                {
                    n.ForeColor = TVSettings.Instance.ShowStatusColors.GetColour(si);
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
                        if (TVSettings.Instance.ShowStatusColors.AppliesTo(s))
                        {
                            n2.ForeColor = TVSettings.Instance.ShowStatusColors.GetColour(s);
                        }
                    }

                    n2.Tag = s;
                    n.Nodes.Add(n2);
                }
            }

            MyShowTree.Nodes.Add(n);

            return n;
        }

        [NotNull]
        // ReSharper disable once UnusedMethodReturnValue.Local
        private TreeNode AddMovieToTree([NotNull] MovieConfiguration si)
        {
            CachedMovieInfo? ser = si.CachedMovie;

            TreeNode n = new TreeNode(GenerateShowUiName(si)) { Tag = si };

            if (ser != null)
            {
                //if (TVSettings.Instance.ShowStatusColors.AppliesTo(si))
                //{
                    //n.ForeColor = TVSettings.Instance.ShowStatusColors.GetColour(si);
                //}
            }
            movieTree.Nodes.Add(n);

            return n;
        }

        // ReSharper disable once InconsistentNaming
        public static string GenerateShowUIName(ProcessedEpisode? episode) => GenerateShowUIName(episode?.TheCachedSeries, episode?.Show);

        // ReSharper disable once InconsistentNaming
        private static string GenerateShowUIName([NotNull] ShowConfiguration si)
        {
            CachedSeriesInfo s = si.CachedShow;
            return GenerateShowUIName(s,si);
        }

        // ReSharper disable once InconsistentNaming
        private static string GenerateShowUIName(CachedSeriesInfo? ser, ShowConfiguration? si)
        {
            string name = GenerateBestName(ser, si);

            return PostpendTheIfNeeded(name);
        }

        private static string PostpendTheIfNeeded(string name)
        {
            if (TVSettings.Instance.PostpendThe && name.StartsWith("The ", StringComparison.Ordinal))
            {
                return name.RemoveFirst(4) + ", The";
            }

            return name;
        }

        private static string GenerateBestName(CachedSeriesInfo? ser, ShowConfiguration? si)
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

            if (TVSettings.Instance.IgnorePreviouslySeen && pe.PreviouslySeen)
            {
                return 9; //tick
            }

            if (airdt.CompareTo(DateTime.Now) < 0) // has aired
            {
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

        private static bool NodeIsForShow([NotNull] ShowConfiguration si, TreeNode n)
        {
            switch (si.Provider)
            {
                case TVDoc.ProviderType.TVmaze:
                    return TreeNodeToShowItem(n)?.TVmazeCode == si.TVmazeCode;
                case TVDoc.ProviderType.TMDB:
                    return TreeNodeToShowItem(n)?.TmdbCode == si.TmdbCode;
                case TVDoc.ProviderType.TheTVDB:
                    return TreeNodeToShowItem(n)?.TvdbCode == si.TvdbCode;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SelectShow(ShowConfiguration si)
        {
            foreach (TreeNode n in MyShowTree.Nodes.Cast<TreeNode>().Where(n => TreeNodeToShowItem(n) == si))
            {
                n.EnsureVisible();
                MyShowTree.SelectedNode = n;
                return;
            }

            FillEpGuideHtml(null);
        }

        private void SelectMovie(MovieConfiguration m)
        {
            foreach (TreeNode n in movieTree.Nodes.Cast<TreeNode>().Where(n => TreeNodeToMovieItem(n) == m))
            {
                n.EnsureVisible();
                movieTree.SelectedNode = n;
                return;
            }

            //FillEpGuideHtml(null);
        }

        private void AddMovie_Click(object sender, EventArgs e)
        {
            Logger.Info("****************");
            Logger.Info("Adding New Movie");
            MoreBusy();
            mDoc.PreventAutoScan("Add Movie");
            MovieConfiguration mov = new MovieConfiguration();

            AddEditMovie aem = new AddEditMovie(mov, mDoc);
            DialogResult dr = aem.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                mDoc.FilmLibrary.Add(mov);

                MovieAddedOrEdited(false, false);
                SelectMovie(mov);

                Logger.Info($"Added new movie called {mov.ShowName}");
            }
            else
            {
                Logger.Info("Cancelled adding new movie");
            }

            MovieAddedOrEdited(true, false);

            LessBusy();
            mDoc.AllowAutoScan();
        }

        private void bnMyShowsAdd_Click(object sender, EventArgs e)
        {
            Logger.Info("****************");
            Logger.Info("Adding New Show");
            MoreBusy();
            mDoc.PreventAutoScan("Add Show");
            ShowConfiguration si = new ShowConfiguration();

            AddEditShow aes = new AddEditShow(si,mDoc);
            DialogResult dr = aes.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                mDoc.TvLibrary.Add(si);

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

        private void MovieAddedOrEdited(bool download, bool unattended)
        {
            mDoc.SetDirty();
            RefreshWTW(download, unattended);
            mDoc.ReindexLibrary();
            FillMyMovies();

            mDoc.ExportMovieInfo(); //Save Movie list to disk 
        }

        private void bnMyShowsDelete_Click(object sender, EventArgs e)
        {
            TreeNode n = MyShowTree.SelectedNode;
            ShowConfiguration si = TreeNodeToShowItem(n);
            if (si is null)
            {
                return;
            }

            DeleteShow(si);
        }

        private void DeleteShow([NotNull] ShowConfiguration si)
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
                    try
                    {
                        Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(si.AutoAddFolderBase,
                            Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                            Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                    }
                    catch (OperationCanceledException e)
                    {
                        Logger.Warn($"Failed to remove {si.AutoAddFolderBase} as operation was cancelled: {e.Message}");
                    }
                    catch (System.IO.DirectoryNotFoundException e)
                    {
                        Logger.Warn($"Failed to remove {si.AutoAddFolderBase} as it is not found: {e.Message}");
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, $"Failed to remove {si.AutoAddFolderBase} as operation failed");
                    }

                }
            }

            Logger.Info($"User asked to remove {si.ShowName} - removing now");
            mDoc.TvLibrary.Remove(si);
            ShowAddedOrEdited(false,false);
        }


        private void DeleteMovie([NotNull] MovieConfiguration si)
        {
            DialogResult res = MessageBox.Show(
                "Remove movie \"" + si.ShowName + "\".  Are you sure?", "Confirmation", MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (res != DialogResult.Yes)
            {
                return;
            }

            foreach (string directory in si.Locations)
            {

                if (Directory.Exists(directory)
                ) // todo use new setting && TVSettings.Instance.DeleteShowFromDisk)
                {
                    DialogResult res3 = MessageBox.Show(
                        $"Remove folder \"{directory}\" from disk?",
                        "Confirmation",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (res3 == DialogResult.Yes)
                    {
                        Logger.Info($"Recycling {directory} as part of the removal of {si.ShowName}");
                        try
                        {
                            Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(directory,
                                Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                                Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                        }
                        catch (OperationCanceledException e)
                        {
                            Logger.Warn(
                                $"Failed to remove {directory} as operation was cancelled: {e.Message}");
                        }
                        catch (System.IO.DirectoryNotFoundException e)
                        {
                            Logger.Warn($"Failed to remove {directory} as it is not found: {e.Message}");
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e, $"Failed to remove {directory} as operation failed");
                        }

                    }
                }
            }

            Logger.Info($"User asked to remove {si.ShowName} - removing now");
            mDoc.FilmLibrary.Remove(si);
            MovieAddedOrEdited(false, false);
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
                ShowConfiguration si = TreeNodeToShowItem(n);
                if (si != null)
                {
                    EditSeason(si, seas.SeasonNumber);
                }

                return;
            }

            ShowConfiguration si2 = TreeNodeToShowItem(n);
            if (si2 != null)
            {
                EditShow(si2);
            }
        }

        internal void EditSeason([NotNull] ShowConfiguration si, int seasnum)
        {
            MoreBusy();
            mDoc.PreventAutoScan("Edit Season");

            EditSeason er = new EditSeason(si, seasnum, TVSettings.Instance.NamingStyle);
            DialogResult dr = er.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                ShowAddedOrEdited(false, false);
                SelectSeason(si.AppropriateSeasons()[seasnum]);
            }

            mDoc.AllowAutoScan();
            LessBusy();
        }

        internal void EditShow([NotNull] ShowConfiguration si)
        {
            MoreBusy();
            mDoc.PreventAutoScan("Edit Show");

            int oldCode = si.TvdbCode;

            AddEditShow aes = new AddEditShow(si,mDoc);

            DialogResult dr = aes.ShowDialog(this);

            if (dr == DialogResult.OK)
            {
                ShowAddedOrEdited(si.TvdbCode != oldCode,false);
                SelectShow(si);

                Logger.Info("Modified show called {0}", si.ShowName);
            }

            mDoc.AllowAutoScan();
            LessBusy();
        }

        internal void EditMovie([NotNull] MovieConfiguration si)
        {
            MoreBusy();
            mDoc.PreventAutoScan("Edit Movie");

            AddEditMovie aes = new AddEditMovie(si,mDoc);
            DialogResult dr = aes.ShowDialog(this);

            if (dr == DialogResult.OK)
            {
                MovieAddedOrEdited(true, false); //todo optimise so that download is only done if movie config has changed show; aes.HasCHanged ?
                SelectMovie(si);

                Logger.Info("Modified movie called {0}", si.ShowName);
            }

            mDoc.AllowAutoScan();
            LessBusy();
        }

        internal void ForceRefresh(IEnumerable<ShowConfiguration>? sis, bool unattended)
        {
            mDoc.ForceRefreshShows(sis,unattended,WindowState==FormWindowState.Minimized,this);
            FillMyShows();
            FillEpGuideHtml();
            RefreshWTW(false, unattended);
        }

        internal void ForceMovieRefresh(IEnumerable<MovieConfiguration>? sis, bool unattended)
        {
            mDoc.ForceRefreshMovies(sis, unattended, WindowState == FormWindowState.Minimized, this);
            FillMyMovies();
            FillMovieGuideHtml();
            RefreshWTW(false, unattended);
        }

        private void UpdateImages(IReadOnlyCollection<ShowConfiguration>? sis)
        {
            if (sis == null)
            {
                return;
            }

            foreach (ShowConfiguration si in sis)
            {
                //update images for the showitem
                mDoc.ForceUpdateImages(si);
            }

            tabControl1.SelectTab(tbAllInOne);
            FillActionList(false);
        }

        private void UpdateImages(IReadOnlyCollection<MovieConfiguration>? sis)
        {
            if (sis == null)
            {
                return;
            }

            foreach (MovieConfiguration si in sis)
            {
                //update images for the showitem
                mDoc.ForceUpdateImages(si);
            }

            tabControl1.SelectTab(tbAllInOne);
            FillActionList(false);
        }

        private void bnMyShowsRefresh_Click(object? sender, EventArgs? e)
        {
            if (ModifierKeys == Keys.Control)
            {
                // nuke currently selected show to force getting it fresh
                TreeNode n = MyShowTree.SelectedNode;
                ShowConfiguration? si = TreeNodeToShowItem(n);
                if (si != null)
                {
                    ForceRefresh(si, false);
                }
            }
            else
            {
                ForceRefresh(false);
            }
        }

        private void MyShowTree_AfterSelect(object sender, [NotNull] TreeViewEventArgs e)
        {
            FillEpGuideHtml(e.Node);
            UpdateMyShowsButtonStatus();
        }

        private void MyMoviesTree_AfterSelect(object sender, [NotNull] TreeViewEventArgs e)
        {
            FillMovieGuideHtml(TreeNodeToMovieItem(e.Node));
            UpdateMyMoviesButtonStatus();
        }

        private void UpdateMyMoviesButtonStatus()
        {
            bool movieSelected = movieTree.SelectedNode != null;
            btnEditMovie.Enabled = movieSelected;
            btnMovieDelete.Enabled = movieSelected;
            tsbMyMoviesContextMenu.Enabled = movieSelected;
        }

        private void FillMovieGuideHtml(MovieConfiguration? si)
        {
            if (tabControl1.SelectedTab != tbMyMovies)
            {
                return;
            }

            if (si is null)
            {
                ClearMovieInfoWindows();
                return;
            }

            if (si.CachedData is null)
            {
                ClearMovieInfoWindows("Not downloaded, or not available");
                return;
            }

            if (TVSettings.Instance.OfflineMode || TVSettings.Instance.ShowBasicShowDetails)
            {
               SetHtmlBody(webMovieInformation, ShowHtmlHelper.CreateOldPage(si.GetMovieHtmlOverviewOffline()));
               SetHtmlBody(webMovieImages, ShowHtmlHelper.CreateOldPage(si.GetMovieImagesHtmlOverview()));

               return;
            }

            SetHtmlBody(webMovieImages, ShowHtmlHelper.CreateOldPage(si.GetMovieImagesHtmlOverview()));
            SetHtmlBody(webMovieInformation, si.GetMovieHtmlOverview(false));

            if (bwMovieHTMLGenerator.WorkerSupportsCancellation)
            {
                // Cancel the asynchronous operation.
                bwMovieHTMLGenerator.CancelAsync();
            }

            if (!bwMovieHTMLGenerator.IsBusy)
            {
                bwMovieHTMLGenerator.RunWorkerAsync(si);
            }
        }

        private void MyMoviesTree_MouseClick(object sender, [NotNull] MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            movieTree.SelectedNode = movieTree.GetNodeAt(e.X, e.Y);

            Point pt = movieTree.PointToScreen(new Point(e.X, e.Y));
            TreeNode n = movieTree.SelectedNode;

            if (n is null)
            {
                return;
            }

            MovieConfiguration si = TreeNodeToMovieItem(n);
            if (si != null)
            {
                RightClickOnMyMovies(si, pt);
            }
        }


        private void UpdateMyShowsButtonStatus()
        {
            bool showSelected = MyShowTree.SelectedNode != null;
            btnEditShow.Enabled = showSelected;
            btnRemoveShow.Enabled = showSelected;
            tsbMyShowsContextMenu.Enabled = showSelected;
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

            ShowConfiguration si = TreeNodeToShowItem(n);
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

        private ProcessedSeason? CurrentlySelectedSeason()
        {
            ProcessedSeason currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
            if (currentSeas != null)
            {
                return currentSeas;
            }

            ShowConfiguration currentShow = TreeNodeToShowItem(MyShowTree.SelectedNode);
            if (currentShow != null)
            {
                foreach (KeyValuePair<int, ProcessedSeason> s in currentShow.AppropriateSeasons())
                {
                    //Find first season we can
                    return s.Value;
                }
            }

            return mDoc.TvLibrary.GetSortedShowItems().SelectMany(si => si.AppropriateSeasons().Values).FirstOrDefault();
        }

        private List<ProcessedEpisode>? CurrentlySelectedPel()
        {
            ProcessedSeason currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
            ShowConfiguration currentShow = TreeNodeToShowItem(MyShowTree.SelectedNode);

            int snum = currentSeas?.SeasonNumber ?? 1;

            if (currentShow != null)
            {
                if (currentShow.SeasonEpisodes.TryGetValue(snum,out List<ProcessedEpisode> returnValue))
                {
                    return returnValue;
                }
            }

            if (currentShow?.SeasonEpisodes.Any() ?? false)
            {
                return currentShow.SeasonEpisodes.First().Value;
            }

            return mDoc.TvLibrary.GetSortedShowItems().SelectMany(si => si.SeasonEpisodes.Values).FirstOrDefault();
        }

        private void filenameTemplateEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomEpisodeName cn = new CustomEpisodeName(TVSettings.Instance.NamingStyle.StyleString);
            CustomNameDesigner cne = new CustomNameDesigner(CurrentlySelectedPel(), cn);
            DialogResult dr = cne.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                TVSettings.Instance.NamingStyle = cn;
                mDoc.SetDirty();
            }
        }

        private void searchEnginesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<ProcessedEpisode>? pel = CurrentlySelectedPel();

            AddEditSearchEngine aese = new AddEditSearchEngine(TVDoc.GetSearchers(),
                pel != null && pel.Count > 0 ? pel[0] : null);

            DialogResult dr = aese.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                mDoc.SetDirty();
                UpdateSearchButtons();
            }
        }

        private void filenameProcessorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowConfiguration? currentShow = TreeNodeToShowItem(MyShowTree.SelectedNode);
            string theFolder = GetFolderForShow(currentShow);

            if (string.IsNullOrWhiteSpace(theFolder) && TVSettings.Instance.DownloadFolders.Count>0)
            {
                theFolder = TVSettings.Instance.DownloadFolders.First();
            }

            AddEditSeasEpFinders d = new AddEditSeasEpFinders(TVSettings.Instance.FNPRegexs, mDoc.TvLibrary.GetSortedShowItems(), currentShow, theFolder);

            DialogResult dr = d.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                mDoc.SetDirty();
                TVSettings.Instance.FNPRegexs = d.OutputRegularExpressions;
            }
        }

        [NotNull]
        private static string GetFolderForShow(MediaConfiguration? currentShow)
        {
            if (currentShow is null)
            {
                return string.Empty;
            }

            foreach (string folder in currentShow.AllExistngFolderLocations().Values.SelectMany(list => list).Where(folder => !string.IsNullOrEmpty(folder) && Directory.Exists(folder)))
            {
                return folder;
            }

            return string.Empty;
        }

        private void actorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ActorsGrid(mDoc).ShowDialog(this);
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
            return e switch
            {
                Keys.D1 => 0,
                Keys.D2 => 1,
                Keys.D3 => 2,
                Keys.D4 => 3,
                Keys.D5 => 4,
                Keys.D6 => 5,
                Keys.D7 => 6,
                Keys.D8 => 7,
                Keys.D9 => 8,
                Keys.D0 => 9,
                _ => -1
            };
        }

        private void UiScan(List<ShowConfiguration>? shows, List<MovieConfiguration>? movies, bool unattended, TVSettings.ScanType st, MediaConfiguration.MediaType media)
        {
            Logger.Info("*******************************");
            string desc = unattended ? "unattended " : "";
            string showsdesc = shows?.Count > 0 ? shows.Count.ToString() : "all";
            string moviesdesc = movies?.Count > 0 ? movies.Count.ToString() : "all";
            string scantype = st.PrettyPrint();
            string mediatype = media.PrettyPrint();
            Logger.Info($"Starting {desc}{scantype} {mediatype} Scan for {showsdesc} shows and {moviesdesc} movies...");

            MoreBusy();
            mDoc.Scan(shows,movies, unattended, st,media, WindowState==FormWindowState.Minimized,this);
            LessBusy();

            AskUserAboutShowProblems(unattended);

            FillMyShows(); // scanning can download more info to be displayed in my shows
            FillMyMovies();
            FillActionList(false);
        }

        private void AskUserAboutShowProblems(bool unattended)
        {
            if (mDoc.ShowProblems.Any() && !unattended)
            {
                string message = mDoc.ShowProblems.Count > 1
                    ? $"Shows with Id {string.Join(",", mDoc.ShowProblems.Select(exception => exception.ShowId))} are not found on TVDB and TVMaze. Please update them"
                    : $"Show with {StringFor(mDoc.ShowProblems.First().ShowIdProvider)} Id {mDoc.ShowProblems.First().ShowId} is not found on {StringFor(mDoc.ShowProblems.First().ErrorProvider)}. Please Update";

                DialogResult result = MessageBox.Show(message, "Series No Longer Found", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Error);

                if (result != DialogResult.Cancel)
                {
                    foreach (ShowNotFoundException problem in mDoc.ShowProblems)
                    {
                        if (mDoc.ShowProblems.Count > 1)
                        {
                            MessageBox.Show(problem.Message, "Issue With Series Setup", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }

                        ShowConfiguration problemShow = mDoc.TvLibrary.GetShowItem(problem.ShowId);
                        if (problemShow != null)
                        {
                            EditShow(problemShow);
                        }
                    }
                }

                mDoc.ClearShowProblems();
            }
        }

        [NotNull]
        private static string StringFor(TVDoc.ProviderType i)
        {
            switch (i)
            {
                case TVDoc.ProviderType.TVmaze:
                    return "TV Maze";
                case TVDoc.ProviderType.TMDB:
                    return "TMDB";
                case TVDoc.ProviderType.TheTVDB:
                    return "The TVDB";

                case TVDoc.ProviderType.libraryDefault:
                default:
                    throw new ArgumentOutOfRangeException(nameof(i), i, null);
            }
        }

        [NotNull]
        private static object ActionImageGetter(object rowObject)
        {
            Item s = (Item)rowObject;
            return s.IconNumber;
        }

        private void SetCheckboxes()
        {
            internalCheckChange = true;
            olvAction.BeginUpdate();
            olvAction.CheckObjects(mDoc.TheActionList.Actions);
            internalCheckChange = false;
            UpdateActionCheckboxes();
            olvAction.EndUpdate();
        }

        public void FillActionList(bool preserveExistingCheckboxes)
        {
            internalCheckChange = true;
            FillNewActionList(preserveExistingCheckboxes);
            internalCheckChange = false;
            UpdateActionCheckboxes();
        }

        private void FillNewActionList(bool preserveExistingCheckboxes)
        {
            if (olvAction.IsDisposed)
            {
                return;
            }

            byte[] oldState = olvAction.SaveState();
            olvAction.BeginUpdate();

            if (preserveExistingCheckboxes)
            {
                List <Item> oldItems = olvAction.Items.OfType<OLVListItem>().Select(lvi => (Item)lvi.RowObject).ToList();

                mDoc.TheActionList.NotifyUpdated();
                olvAction.RebuildColumns();

                List<Item> newItems = olvAction.Items.OfType<OLVListItem>().Select(lvi => (Item)lvi.RowObject).ToList();
                //We have a new addition - check its checkbox
                olvAction.CheckObjects(newItems.Where(newRow => !oldItems.Contains(newRow)));
            }
            else
            {
                mDoc.TheActionList.NotifyUpdated();
                olvAction.RebuildColumns();
                internalCheckChange = true;
                //olvAction.ItemCheck -= olvAction_ItemCheck;
                olvAction.BeginUpdate();
                if (mDoc.TheActionList.Actions.Count <100)
                {
                    //TODO WE SHOULD DO THIS ALL THE TIME
                    olvAction.CheckObjects(mDoc.TheActionList.Actions);
                }
                internalCheckChange = false;
                //olvAction.ItemCheck += olvAction_ItemCheck;
                UpdateActionCheckboxes();
                olvAction.EndUpdate();
                //SetCheckboxes();
            }
            olvAction.RestoreState(oldState);
            olvAction.EndUpdate();
        }

        [NotNull]
        private static string HeaderName(string name, int number) => $"{name} ({PrettyPrint(number)})";

        [NotNull]
        private static string PrettyPrint(int number) => number + " " + number.ItemItems();

        [NotNull]
        private static string HeaderName(string name, int number, long filesize) => $"{name} ({PrettyPrint(number)}, {filesize.GBMB(1)})";

        private void bnActionAction_Click(object sender, EventArgs e) => ActionAction(true,false, false);

        private void ActionAction(bool checkedNotSelected, bool unattended, bool doAll)
        {
            mDoc.PreventAutoScan("Action Selected Items");
            if (doAll)
            {
                mDoc.DoAllActions(this);
            }
            else
            {
                ItemList lvr = checkedNotSelected
                    ? GetCheckedItems()
                    : GetSelectedItems();

                mDoc.DoActions(lvr, this);
            }

            FillActionList(true);
            RefreshWTW(false,unattended);
            mDoc.AllowAutoScan();
        }

        private void Revert()
        {
            foreach (Item item in GetSelectedItems())
            {
                mDoc.RevertAction(item);
            }

            FillActionList(true);
            RefreshWTW(false,false);
        }

        private void folderMonitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BulkAddSeriesManager bam = new BulkAddSeriesManager(mDoc);
            FolderMonitor fm = new FolderMonitor(mDoc, bam);
            fm.ShowDialog(this);
            FillMyShows();
        }

        private void lvAction_MouseClick([NotNull] object sender, [NotNull] MouseEventArgs e)
        {
            Point pt = ((ListView)sender).PointToScreen(new Point(e.X, e.Y));

            ItemList lvr = GetSelectedItems();

            Item? action = (Item)olvAction.FocusedObject;

            if (action?.Episode != null && lvr.Count == 1)
            {
                switchToWhenOpenMyShows = action.Episode;
                if (e.Button != MouseButtons.Right)
                {
                    return;
                }
                GenerateActionRightClickMenu(pt, lvr, action.Episode.Show, action.Episode, null);
            }
            else if (action?.Movie != null && lvr.Count == 1)
            {
                switchToWhenOpenMyMovies = action.Movie;
                if (e.Button != MouseButtons.Right)
                {
                    return;
                }
                GenerateActionRightClickMenu(pt, lvr, null,null, action.Movie);
            }
            else
            {
                if (e.Button != MouseButtons.Right)
                {
                    return;
                }
                GenerateActionRightClickMenu(pt, lvr, null,null,null);
            }
        }
        private void GenerateActionRightClickMenu(Point pt, [NotNull] ItemList lvr, ShowConfiguration? si, ProcessedEpisode? episode, MovieConfiguration? movie)
        { 
            if (lvr.Count == 0)
            {
                return; // nothing selected
            }

            showRightClickMenu.Items.Clear();

            ProcessedSeason seas = episode?.AppropriateProcessedSeason;

            // Action related items
            if (lvr.Count > lvr.Missing.ToList().Count) // not just missing selected
            {
                AddRcMenuItem("Action Selected", (sender, args) => ActionAction(false, false,false)); 
            }

            AddRcMenuItem("Ignore Selected", (sender, args) => IgnoreSelected());
            if (episode != null)
            {
                AddRcMenuItem("Ignore Entire Season", (sender, args) => IgnoreSelectedSeasons(lvr));
            }
            AddRcMenuItem("Remove Selected", (sender, args) => ActionDeleteSelected());

            if (lvr.Count == lvr.Missing.ToList().Count) // only missing items selected?
            {
                if (episode != null)
                {
                    MenuSearchFor(episode);
                }

                if (movie != null)
                {
                    MenuSearchFor(movie);
                }

                if (lvr.Count == 1) // only one selected
                {
                    AddRcMenuItem("Browse For...", (sender, args) => BrowseForMissingItem((ItemMissing)lvr[0]));
                }
            }

            if (lvr.CopyMove.Count > 0||lvr.DownloadTorrents.Count>0||lvr.Downloading.Count>0)
            {
                showRightClickMenu.Items.Add(new ToolStripSeparator());
                AddRcMenuItem("Revert to Missing", (sender, args) => Revert());
            }

            MenuGuideAndTvdb(true,episode,new List<ShowConfiguration> {si},seas );
            MenuFolders(lvr,si,episode?.AppropriateProcessedSeason,episode);

            showRightClickMenu.Show(pt);
        }

        private void MenuSearchFor(ProcessedEpisode ep)
        {
            showRightClickMenu.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem tsi = new ToolStripMenuItem("Search");
            tsi.Click += (s, ev) => { TVDoc.SearchForEpisode(ep); };

            foreach (SearchEngine se in TVDoc.GetSearchers())
            {
                if (se.Name.HasValue())
                {
                    ToolStripMenuItem tssi = new ToolStripMenuItem(se.Name);
                    tssi.Click += (s, ev) => { TVDoc.SearchForEpisode(se,ep); };
                    tsi.DropDownItems.Add(tssi);
                }
            }

            if (TVSettings.Instance.SearchJackett || TVSettings.Instance.SearchJackettButton)
            {
                if (TVDoc.GetSearchers().Any())
                {
                    tsi.DropDownItems.Add(new ToolStripSeparator());
                }
                ToolStripMenuItem tssi = new ToolStripMenuItem("Jackett Search");
                tssi.Click += (s, ev) => { JackettFinder.SearchForEpisode(ep); };
                tsi.DropDownItems.Add(tssi);
            }

            showRightClickMenu.Items.Add(tsi);
        }

        private void MenuSearchFor(MovieConfiguration ep)
        {
            showRightClickMenu.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem tsi = new ToolStripMenuItem("Search");
            tsi.Click += (s, ev) => { TVDoc.SearchForMovie(ep); };

            foreach (SearchEngine se in TVDoc.GetMovieSearchers())
            {
                if (se.Name.HasValue())
                {
                    ToolStripMenuItem tssi = new ToolStripMenuItem(se.Name);
                    tssi.Click += (s, ev) => { TVDoc.SearchForMovie(se, ep); };
                    tsi.DropDownItems.Add(tssi);
                }
            }

            if (TVSettings.Instance.SearchJackett || TVSettings.Instance.SearchJackettButton)
            {
                if (TVDoc.GetMovieSearchers().Any())
                {
                    tsi.DropDownItems.Add(new ToolStripSeparator());
                }
                ToolStripMenuItem tssi = new ToolStripMenuItem("Jackett Search");
                tssi.Click += (s, ev) => { JackettFinder.SearchForMovie(ep); };
                tsi.DropDownItems.Add(tssi);
            }

            showRightClickMenu.Items.Add(tsi);
        }

        private void AddRcMenuItem(string name,EventHandler command)
        {
            ToolStripMenuItem tsi = new ToolStripMenuItem(name);
            tsi.Click += command;
            showRightClickMenu.Items.Add(tsi);
        }

        private void lvAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSearchButtons();

            ItemList lvr = GetSelectedItems();

            if (lvr.Count == 0)
            {
                // disable everything
                btnActionBTSearch.Enabled = false;
                tbActionJackettSearch.Enabled = false;
                return;
            }

            btnActionBTSearch.Enabled = lvr.Missing.Any();
            tbActionJackettSearch.Enabled = lvr.Missing.Any();

            showRightClickMenu.Items.Clear();
        }

        private void ActionDeleteSelected()
        {
            mDoc.TheActionList.Remove(GetSelectedItems());
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

            ItemList all = mDoc.TheActionList;
            List<Item> chk = olvAction.CheckedObjects.OfType<Item>().ToList();

            SetCheckbox(mcbRename, all.OfType<ActionCopyMoveRename>().Where(a => a.Operation==ActionCopyMoveRename.Op.rename), chk.OfType<ActionCopyMoveRename>().Where(a => a.Operation == ActionCopyMoveRename.Op.rename));
            SetCheckbox(mcbCopyMove, all.OfType<ActionCopyMoveRename>().Where(a => a.Operation != ActionCopyMoveRename.Op.rename), chk.OfType<ActionCopyMoveRename>().Where(a => a.Operation != ActionCopyMoveRename.Op.rename));
            SetCheckbox(mcbDeleteFiles, all.OfType<ActionDelete>(), chk.OfType<ActionDelete>());
            SetCheckbox(mcbSaveImages, all.OfType<ActionDownloadImage>(), chk.OfType<ActionDownloadImage>());
            SetCheckbox(mcbWriteMetadata, all.OfType<ActionWriteMetadata>(), chk.OfType<ActionWriteMetadata>());
            SetCheckbox(mcbModifyMetadata, all.OfType<ActionFileMetaData>(), chk.OfType<ActionFileMetaData>());
            SetCheckbox(mcbDownload, all.TorrentActions, chk.Where(item => item is ActionTRemove || item is ActionTDownload));

            int numberOfActions = all.Actions.Count;
            int numberOfCheckedActions = chk.OfType<Action>().Count();

            if (numberOfCheckedActions == 0)
            {
                mcbAll.CheckState = CheckState.Unchecked;
            }
            else
            {
                mcbAll.CheckState = numberOfCheckedActions == numberOfActions ? CheckState.Checked : CheckState.Indeterminate;
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

        private void olvAction_ItemCheck(object sender, [NotNull] ItemCheckEventArgs e)
        {
            if (internalCheckChange)
            {
                return;
            }

            //Needed to de-selct any un action able items
            Item? action = (Item)olvAction.GetModelObject(e.Index);
            if (action != null && !(action is Action))
            {
                e.NewValue = CheckState.Unchecked;
            }
        }

        private void bnActionOptions_Click(object sender, EventArgs e) => DoPrefs(true);

        private void lvAction_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // double-click on an item will search for missing, do nothing (for now) for anything else
            foreach (Item i in GetSelectedItems())
            {
                if (i is ItemMissing miss)
                {
                    if (miss.Episode != null)
                    {
                        TVDoc.SearchForEpisode(miss.Episode);
                    }
                    if (miss.Movie != null)
                    {
                        TVDoc.SearchForMovie(miss.Movie);
                    }
                }
                else
                {
                    olvAction.ToggleCheckObject(i);
                }
            }
        }

        private void bnActionBTSearch_Click(object sender, EventArgs e)
        {
            foreach (Item i in GetSelectedItems())
            {
                if (i?.Episode != null)
                {
                    TVDoc.SearchForEpisode(i.Episode);
                }
                if (i?.Movie != null)
                {
                    TVDoc.SearchForMovie(i.Movie);
                }
            }
        }

        private void bnRemoveSel_Click(object sender, EventArgs e) => ActionDeleteSelected();

        private void IgnoreSelected()
        {
            bool added = false;
            foreach (Item action in GetSelectedItems())
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
            IgnoreEdit ie = new IgnoreEdit(mDoc,string.Empty);
            ie.ShowDialog(this);
        }

        private async void showSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSummary f = new ShowSummary(mDoc,this);
            f.Show(this);
        }

        private void lvAction_ItemChecked(object sender, ItemCheckedEventArgs e) => UpdateActionCheckboxes();

        private void btnFilter_Click(object sender, EventArgs e)
        {
            Filters filters = new Filters(mDoc);
            DialogResult res = filters.ShowDialog(this);
            if (res == DialogResult.OK)
            {
                FillMyShows();
            }
        }

        private void filterTextBox_TextChanged(object sender, EventArgs e) => FillMyShows();
        private void filterMoviesTextBox_TextChanged(object sender, EventArgs e) => FillMyMovies();

        private void filterTextBox_SizeChanged(object sender, EventArgs e)
        {
            TextBoxSizeChanged(filterTextBox);
        }

        private void filterMoviesTextBox_SizeChanged(object sender, EventArgs e)
        {
            TextBoxSizeChanged(filterMoviesTextbox);
        }

        private static void TextBoxSizeChanged(TextBox tb)
        {
            // MAH: move the "Clear" button in the Filter Text Box
            if (tb.Controls.ContainsKey("Clear"))
            {
                Control filterButton = tb.Controls["Clear"];
                int clientSizeHeight = (tb.ClientSize.Height - 16) / 2;
                filterButton.Location = new Point(tb.ClientSize.Width - filterButton.Width, clientSizeHeight + 1);

                // Send EM_SETMARGINS to prevent text from disappearing underneath the button
                NativeMethods.SendMessage(tb.Handle, 0xd3, (IntPtr) 2, (IntPtr) (filterButton.Width << 16));
            }
        }

        private void visitSupportForumToolStripMenuItem_Click(object sender, EventArgs e)
            => Helpers.OpenUrl("https://groups.google.com/forum/#!forum/tvrename");

        public void Quit() => Close();

        private async void checkForNewVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dispatcher uiDisp = Dispatcher.CurrentDispatcher;

            Task<ServerRelease> tuv = VersionUpdater.CheckForUpdatesAsync();
            ServerRelease result = await tuv.ConfigureAwait(false);

            uiDisp.Invoke(() => NotifyUpdates(result, true));
        }

        private void NotifyUpdates(ServerRelease? update, bool showNoUpdateRequiredDialog, bool inSilentMode = false)
        {
            if (update is null)
            {
                btnUpdateAvailable.Visible = false;
                if (showNoUpdateRequiredDialog && !inSilentMode && Environment.UserInteractive)
                {
                    MessageBox.Show(@"There is no update available please try again later.", @"No update available",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }

            Logger.Warn(update.LogMessage());

            if (inSilentMode || Debugger.IsAttached || !Environment.UserInteractive)
            {
               return;
            }

            UpdateNotification unForm = new UpdateNotification(update);
            if (!TVSettings.Instance.SuppressUpdateAvailablePopup)
            {
                unForm.ShowDialog(this);
                if (unForm.DialogResult == DialogResult.Abort)
                {
                    Logger.Info("Downloading New Release and Quiting");
                    //We need to quit!
                    Close();
                }
            }
            btnUpdateAvailable.Visible = true;
        }

        private void duplicateFinderLOGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DuplicateMovieFinder form = new DuplicateMovieFinder( mDoc, this);
            form.ShowDialog(this);
        }

        private async void btnUpdateAvailable_Click(object sender, EventArgs e)
        {
            btnUpdateAvailable.Visible = false;

            Dispatcher uiDisp = Dispatcher.CurrentDispatcher;

            Task<ServerRelease?> tuv = VersionUpdater.CheckForUpdatesAsync();
            ServerRelease? result = await tuv.ConfigureAwait(false);

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
                foreach (ShowConfiguration si in mDoc.TvLibrary.GetSortedShowItems())
                {
                    CachedSeriesInfo ser = si.CachedShow;
                    if (ser != null)
                    {
                        results.Add(ser.Network??string.Empty, si.ShowTimeZone, si.ShowName);
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
            form.ShowDialog(this);
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

            UIAccuracyCheck(false);

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
            SetScan(TVSettings.ScanType.Full);
        }

        private void SetScan(TVSettings.ScanType st)
        {
            btnScan.Text = st.PrettyPrint()+" Scan";
            mDoc.SetDefaultScanType(st);
        }

        private void RecentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetScan(TVSettings.ScanType.Recent);
        }

        private void QuickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetScan(TVSettings.ScanType.Quick);
        }

        private void BTSearch_DropDownOpening([NotNull] object sender, EventArgs e)
        {
            ChooseSiteMenu((ToolStripSplitButton)sender);
        }

        private void BtnSearch_ButtonClick(object? sender, EventArgs? e)
        {
            UiScan(null, null,false, TVSettings.Instance.UIScanType, MediaConfiguration.MediaType.both);
        }

        private void UpdateCheckboxGroup([NotNull] ToolStripMenuItem menuItem, [NotNull] Func<Item, bool> isValid)
        {
            menuItem.CheckState = menuItem.CheckState switch
            {
                CheckState.Unchecked => CheckState.Checked,
                CheckState.Checked => CheckState.Unchecked,
                CheckState.Indeterminate => CheckState.Unchecked,
                _ => throw new ArgumentOutOfRangeException()
            };

            CheckState cs = menuItem.CheckState;

            internalCheckChange = true;

            if (cs == CheckState.Checked)
            {
                olvAction.CheckObjects(olvAction.Objects.OfType<Item>().Where(isValid).ToList());
            }
            else
            {
                olvAction.UncheckObjects(olvAction.Objects.OfType<Item>().Where(isValid).ToList());
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
            UpdateCheckboxGroup(mcbDownload, i => i is ActionTDownload || i is ActionTRemove);
        }

        private void McbWriteMetadata_Click(object sender, EventArgs e)
        {
            UpdateCheckboxGroup(mcbWriteMetadata, i => i is ActionWriteMetadata);
        }

        private void McbModifyMetadata_Click(object sender, EventArgs e)
        {
            UpdateCheckboxGroup(mcbModifyMetadata, i => i is ActionFileMetaData);
        }

        private void ThanksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ThanksForm form = new ThanksForm();
            form.ShowDialog(this);
        }

        private void TabControl1_DrawItem(object sender, [NotNull] DrawItemEventArgs e)
        {
            //Follow this advice https://docs.microsoft.com/en-us/dotnet/framework/winforms/controls/how-to-display-side-aligned-tabs-with-tabcontrol

            Graphics g = e.Graphics;
            TabControl tabCtrl = (TabControl)sender;

            g.FillRectangle(e.State == DrawItemState.Selected ? Brushes.White : new SolidBrush(BackColor),e.Bounds);

            // Get the item from the collection.
            TabPage tabPage = tabCtrl.TabPages[e.Index];

            // Get the real bounds for the tab rectangle.
            Rectangle tabBounds = tabCtrl.GetTabRect(e.Index);

            // Draw string. Center the text.
            StringFormat stringFlags = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            const int INDENT = 15;

            //GetIcon
            Image? icon = tabCtrl.ImageList.Images[tabPage.ImageKey];
            if (icon is null)
            {
                return;
            }

            float xIndent = (tabBounds.Width - icon.Width) / 2.0f;
            float x = tabBounds.X + xIndent; 
            float y = tabBounds.Y + INDENT;
            e.Graphics.DrawImage(icon, x, y);
            Font labelFont = new Font("Segoe UI Semibold", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);

            Rectangle textarea = new Rectangle(tabBounds.X, tabBounds.Y + INDENT + icon.Height,tabBounds.Width,tabBounds.Height-(INDENT + icon.Height));
            g.DrawString(tabPage.Text, labelFont, Brushes.Black,textarea,stringFlags );
        }

        private void BwSeasonHTMLGenerator_DoWork(object sender, [NotNull] DoWorkEventArgs e)
        {
            ProcessedSeason? s = e.Argument as ProcessedSeason;
            ShowConfiguration si = s?.Show;

            string html = string.Empty;
            try
            {
                html = si?.GetSeasonHtmlOverview(s, true) ?? string.Empty;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Error Occurred Creating Show Summary for {si?.ShowName} with order {si?.Order}");
            }
            e.Result = html;
        }

        private void UpdateWebInformation(object sender, [NotNull] RunWorkerCompletedEventArgs e)
        {
            string html = e.Result as string;
            if (html.HasValue())
            {
                SetHtmlBody(webInformation, html!);
            }
        }

        private void UpdateMovieInformation(object sender, [NotNull] RunWorkerCompletedEventArgs e)
        {
            string html = e.Result as string;
            if (html.HasValue())
            {
                SetHtmlBody(webMovieInformation, html!);
            }
        }

        private void UpdateWebSummary(object sender, [NotNull] RunWorkerCompletedEventArgs e)
        {
            string html = e.Result as string;
            if (html.HasValue())
            {
                SetHtmlBody(webSummary, html!);
            }
        }

        private void BwShowHTMLGenerator_DoWork(object sender, [NotNull] DoWorkEventArgs e)
        {
            ShowConfiguration si = e.Argument as ShowConfiguration;

            string html = string.Empty;
            try
            {
                html = si?.GetShowHtmlOverview(true) ?? string.Empty;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Error Occurred Creating Show Summary for {si?.ShowName} with order {si?.Order}");
            }
            e.Result = html;
        }

        private void BwUpdateSchedule_DoWork(object sender, [NotNull] DoWorkEventArgs e)
        {
            e.Result = GenerateNewScheduleItems();
        }

        private void BwUpdateSchedule_RunWorkerCompleted(object sender, [NotNull] RunWorkerCompletedEventArgs e)
        {
            if(!(e.Result is List<ListViewItem> newContents))
            {
                return;
            }

            mInternalChange++;
            lvWhenToWatch.BeginUpdate();

            int dd = TVSettings.Instance.WTWRecentDays;

            lvWhenToWatch.Groups["justPassed"].Header =
                "Aired in the last " + dd + " day" + (dd == 1 ? "" : "s");

            // try to maintain selections if we can
            List<ProcessedEpisode> selections = new List<ProcessedEpisode>();
            foreach (ListViewItem lvi in lvWhenToWatch.SelectedItems)
            {
                selections.Add((ProcessedEpisode)lvi.Tag);
            }

            ProcessedSeason currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
            ShowConfiguration currentShowConfiguration = TreeNodeToShowItem(MyShowTree.SelectedNode);

            lvWhenToWatch.Items.Clear();

            List<DateTime> bolded = new List<DateTime>();

            foreach (ListViewItem lvi in newContents)
            {
                if (!(lvi.Tag is ProcessedEpisode ei))
                {
                    continue;
                }

                DateTime? dt = ei.GetAirDateDt(true);
                if (dt != null)
                {
                    bolded.Add(dt.Value);
                }

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
            else if (currentShowConfiguration != null)
            {
                SelectShow(currentShowConfiguration);
            }

            UpdateToolstripWTW();
            mInternalChange--;
        }

        private void TbFullScan_Click(object sender, EventArgs e)
        {
            UiScan(null, null, false, TVSettings.ScanType.Full, MediaConfiguration.MediaType.both);
        }

        private void TpRecentScan_Click(object sender, EventArgs e)
        {
            UiScan(null, null, false, TVSettings.ScanType.Recent, MediaConfiguration.MediaType.both);
        }

        private void TbQuickScan_Click(object sender, EventArgs e)
        {
            UiScan(null, null, false, TVSettings.ScanType.Quick, MediaConfiguration.MediaType.both);
        }

        private void UI_Resize(object sender, EventArgs e)
        {
            int targetWidth = 1100;
            if (TVSettings.Instance.ShowAccessibilityOptions) targetWidth += 200;
            if (TVSettings.Instance.SearchJackettButton) targetWidth += 200;

            bool isWide = Width > (targetWidth);
            tpRecentScan.Visible = isWide;
            tbQuickScan.Visible = isWide;
            tbFullScan.Visible = isWide;
            btnScan.Visible = !isWide;
        }

        private void BtnRevertView_Click(object sender, EventArgs e)
        {
            DefaultOlvView();
        }

        private void DefaultOlvView()
        {
            olvAction.BeginUpdate();
            olvAction.ShowGroups=true;
            olvAction.AlwaysGroupByColumn = null;
            olvAction.Sort(olvType,SortOrder.Ascending);
            olvAction.BuildGroups(olvType,SortOrder.Ascending);//,olvShowColumn,SortOrder.Ascending,olvSeason,SortOrder.Ascending);
            olvAction.CustomSorter = delegate { olvAction.ListViewItemSorter = new ListViewActionItemSorter(); };
            olvAction.EndUpdate();
        }

        private void OlvAction_Dropped(object sender, [NotNull] OlvDropEventArgs e)
        {
            // Get a list of filenames being dragged
            string[] files = (string[]) ((DataObject)e.DataObject).GetData(DataFormats.FileDrop, false);

            // Establish item in list being dragged to, and exit if no item matched
            // Check at least one file was being dragged, and that dragged-to item is a "Missing Item" item.
            if (files.Length <= 0 ||  !(e.DropTargetItem.RowObject is ItemMissing mi))
            {
                return;
            }
            
            // Only want the first file if multiple files were dragged across.
            ManuallyAddFileForItem(mi, files[0]);
        }

        private void OlvAction_CanDrop(object sender, [NotNull] OlvDropEventArgs e)
        {
            if (!(e.DropSink?.DropTargetItem?.RowObject is Item item))
            {
                e.Effect = DragDropEffects.None;
            }
            else
            {
                if (item is ItemMissing)
                {
                    if (((DataObject) e.DataObject).GetDataPresent(DataFormats.FileDrop))
                    {
                        e.Effect = DragDropEffects.All;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                        e.InfoMessage = "Can only drag files onto a missing episode";
                    }
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                    e.InfoMessage = "Can only drag onto a missing episode";
                }
            }
        }

        private void BwShowSummaryHTMLGenerator_DoWork(object sender, [NotNull] DoWorkEventArgs e)
        {
            ShowConfiguration si = e.Argument as ShowConfiguration;
            string html = string.Empty;
            try
            {
                html = si?.GetShowSummaryHtmlOverview(true) ??string.Empty;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Error Occurred Creating Show Summary for {si?.ShowName} with order {si?.Order}");
            }
            e.Result = html;
        }
        private void BwSeasonSummaryHTMLGenerator_DoWork(object sender, [NotNull] DoWorkEventArgs e)
        {
            ProcessedSeason s = e.Argument as ProcessedSeason;
            ShowConfiguration si = s?.Show;
            string html = string.Empty;
            try
            {
                html = si?.GetSeasonSummaryHtmlOverview(s, true) ?? string.Empty;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Error Occurred Creating Show Summary for {si?.ShowName} with order {si?.Order}");
            }
            e.Result = html;
        }

        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            if (lvWhenToWatch.SelectedItems.Count == 0)
            {
                return;
            }

            ToolStripButton button = (ToolStripButton) sender;

            Point pt = button.Owner.PointToScreen(button.Bounds.Location);
            List<ProcessedEpisode> eis = new List<ProcessedEpisode>();

            foreach (ListViewItem lvi in lvWhenToWatch.SelectedItems)
            {
                eis.Add(lvi.Tag as ProcessedEpisode);
            }

            WtwRightClickOnShow(eis, pt );
        }

        private void TsbMyShowsContextMenu_Click(object sender, EventArgs e)
        {
            ToolStripButton button = (ToolStripButton)sender;
            Point pt = button.Owner.PointToScreen(button.Bounds.Location);

            TreeNode n = MyShowTree.SelectedNode;

            if (n is null)
            {
                return;
            }

            ShowConfiguration si = TreeNodeToShowItem(n);
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

        private void TsbScanContextMenu_Click(object sender, EventArgs e)
        {
            ToolStripButton button = (ToolStripButton)sender;
            Point pt = button.Owner.PointToScreen(button.Bounds.Location);

            ItemList lvr = GetSelectedItems();

            Item? action = (Item)olvAction.FocusedObject;

            if (action?.Episode != null && lvr.Count == 1)
            {
                GenerateActionRightClickMenu(pt, lvr, action.Episode.Show, action.Episode, null);
            }
            else if (action?.Movie != null && lvr.Count == 1)
            {
                GenerateActionRightClickMenu(pt, lvr, null,null,action.Movie);
            }
            else
            {
                GenerateActionRightClickMenu(pt, lvr, null, null, null);
            }
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OrphanFiles ui = new OrphanFiles(mDoc,this);
            ui.Show(this);
        }

        private void tbJackettSearch_Click(object sender, EventArgs e)
        {
            if (TVSettings.Instance.SearchJackettButton)
            {
                foreach (Item i in GetSelectedItems())
                {
                    if (i?.Episode != null)
                    {
                        JackettFinder.SearchForEpisode(i.Episode);
                    }

                    if (i?.Movie != null)
                    {
                        JackettFinder.SearchForMovie(i.Movie);
                    }
                }
            }
        }

        private void tsbScheduleJackettSearch_Click(object sender, EventArgs e)
        {
            if (TVSettings.Instance.SearchJackettButton)
            {
                foreach (ListViewItem lvi in lvWhenToWatch.SelectedItems)
                {
                    JackettFinder.SearchForEpisode((ProcessedEpisode) lvi.Tag);
                }
            }
        }

        private void olvAction_BeforeCreatingGroups(object sender, CreateGroupsEventArgs e)
        {
            e.Parameters.ItemComparer = new ListViewActionItemSorter();
        }

        public void ShowFgDownloadProgress(CacheUpdater cu, CancellationTokenSource cts)
        {
            Invoke((MethodInvoker) delegate { new DownloadProgress(cu, cts).Show(this); });
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            TreeNode n = movieTree.SelectedNode;
            if (n is null)
            {
                return;
            }

            MovieConfiguration si2 = TreeNodeToMovieItem(n);
            if (si2 != null)
            {
                EditMovie(si2);
            }
        }

        private void btnMovieDelete_Click(object sender, EventArgs e)
        {
            TreeNode n = movieTree.SelectedNode;
            MovieConfiguration si = TreeNodeToMovieItem(n);
            if (si is null)
            {
                return;
            }

            DeleteMovie(si);
        }

        private void bwMovieHTMLGenerator_DoWork(object sender, DoWorkEventArgs e)
        {
            MovieConfiguration si = e.Argument as MovieConfiguration;

            string html = string.Empty;
            try
            {
                html = si?.GetMovieHtmlOverview(true) ?? string.Empty;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Error Occurred Creating Movie Summary for {si?.ShowName}");
            }
            e.Result = html;
        }

        private void movieCollectionSummaryLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CollectionsView form = new CollectionsView(mDoc, this);
            form.ShowDialog(this);
        }

        private void bulkAddMoviesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BulkAddMovieManager bam = new BulkAddMovieManager(mDoc);
            BulkAddMovie fm = new BulkAddMovie(mDoc, bam);
            fm.ShowDialog(this);
            FillMyMovies();
        }

        private void tMDBAccuracyCheckLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Show Log Pane
            logToolStripMenuItem_Click(sender, e);

            Cursor.Current = Cursors.WaitCursor;

            UITMDBAccuracyCheck(false);

            Cursor.Current = Cursors.Default;
        }

        // ReSharper disable once InconsistentNaming
        private void UITMDBAccuracyCheck(bool unattended)
        {
            MoreBusy();
            Task.Run(
                () => mDoc.TMDBServerAccuracyCheck(unattended, WindowState == FormWindowState.Minimized, this)
            );

            LessBusy();
        }

        private void tsbMyMoviesContextMenu_Click(object sender, EventArgs e)
        {
            ToolStripButton button = (ToolStripButton)sender;
            Point pt = button.Owner.PointToScreen(button.Bounds.Location);

            TreeNode n = movieTree.SelectedNode;

            if (n is null)
            {
                return;
            }

            MovieConfiguration si = TreeNodeToMovieItem(n);
                 if (si != null)
            {
                RightClickOnMyMovies(si, pt);
            }

        }

        private void btnMovieFilter_Click(object sender, EventArgs e)
        {
            MovieFilters filters = new MovieFilters(mDoc);
            DialogResult res = filters.ShowDialog(this);
            if (res == DialogResult.OK)
            {
                FillMyMovies();
            }
        }

        private void btnMovieRefresh_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                // nuke currently selected show to force getting it fresh
                TreeNode n = movieTree.SelectedNode;
                MovieConfiguration si = TreeNodeToMovieItem(n);
                ForceMovieRefresh(si, false);
            }
            else
            {
                ForceMovieRefresh(false);
            }
        }

        internal void ForceMovieRefresh(bool unattended)
        {
            ForceMovieRefresh((List<MovieConfiguration>)null, unattended);
        }

        internal void ForceMovieRefresh(MovieConfiguration? sis, bool unattended)
        {
            ForceMovieRefresh(new List<MovieConfiguration> { sis },unattended);
        }

        private void recommendationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //RecommendationView form = new RecommendationView(mDoc, this, mDoc.TvLibrary.Shows.Take(50));
            RecommendationView form = new RecommendationView(mDoc, this, MediaConfiguration.MediaType.tv);
            form.ShowDialog(this);

        }

        private void duplicateMoviesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DuplicateMovieFinder form = new DuplicateMovieFinder(mDoc, this);
            form.ShowDialog(this);
        }

        private void movieRecommendationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //RecommendationView form = new RecommendationView(mDoc, this, mDoc.FilmLibrary.Movies.Take(50));
            RecommendationView form = new RecommendationView(mDoc, this, MediaConfiguration.MediaType.movie); 
            form.ShowDialog(this);
        }

        internal void ForceRefresh(ShowConfiguration show, bool unattended)
        {
            ForceRefresh(new List<ShowConfiguration> { show }, unattended);
        }
        internal void ForceRefresh(bool unattended)
        {
            ForceRefresh((List<ShowConfiguration>) null, unattended);
        }

        private void settingsCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsReview form = new SettingsReview(mDoc, this);
            form.ShowDialog(this);

        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            UiScan(null, null, false, TVSettings.ScanType.Full, MediaConfiguration.MediaType.movie);
            tabControl1.SelectTab(tbAllInOne);
        }

        private void movieSearchEnginesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MovieConfiguration? m = CurrentlySelectedMovie();

            AddEditSearchEngine aese = new AddEditSearchEngine(TVDoc.GetMovieSearchers(), m);

            DialogResult dr = aese.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                mDoc.SetDirty();
                UpdateSearchButtons();
            }
        }

        private MovieConfiguration? CurrentlySelectedMovie()
        {
            MovieConfiguration currentMovie = TreeNodeToMovieItem(movieTree.SelectedNode);

            if (currentMovie != null)
            {
                return currentMovie;
            }

            return mDoc.FilmLibrary.Movies.FirstOrDefault();
        }
    }
}
