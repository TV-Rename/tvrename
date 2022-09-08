//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using BrightIdeasSoftware;
using CefSharp.WinForms;
using Humanizer;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using TVRename.Forms;
using TVRename.Forms.Supporting;
using TVRename.Forms.Tools;
using TVRename.Forms.Utilities;
using TVRename.Ipc;
using TVRename.Properties;
using TVRename.Utility.Helper;
using Control = System.Windows.Forms.Control;
using DataFormats = System.Windows.Forms.DataFormats;
using Alphaleonis.Win32.Filesystem;
using Microsoft.WindowsAPICodePack.Taskbar;
using DragDropEffects = System.Windows.Forms.DragDropEffects;
using MessageBox = System.Windows.Forms.MessageBox;
using SystemColors = System.Drawing.SystemColors;
using Timer = System.Windows.Forms.Timer;

namespace TVRename;

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
    public const string EXPLORE_PROXY = "https://www.tvrename.com/EXPLOREPROXY";
    public const string WATCH_PROXY = "https://www.tvrename.com/WATCHPROXY";

    #region Delegates

    public delegate void ScanTypeDelegate(TVSettings.ScanType type);

    public delegate void ArgumentDelegate(string[] args);

    public readonly ScanTypeDelegate ScanAndDo;

    #endregion Delegates

    private int busy;
    private readonly TVDoc mDoc;
    private bool internalCheckChange;
    private int lastDlRemaining;
    private int mInternalChange;
    private Point mLastNonMaximizedLocation;
    private Size mLastNonMaximizedSize;
    private readonly AutoFolderMonitor? mAutoFolderMonitor;
    private bool treeExpandCollapseToggle = true;

    private ProcessedEpisode? switchToWhenOpenMyShows;
    private MovieConfiguration? switchToWhenOpenMyMovies;

    private readonly ListViewColumnSorter lvwScheduleColumnSorter;
    //private readonly ListViewColumnSorter lvwActionColumnSorter;

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private bool IsBusy => busy != 0;

    private ScanProgress? scanProgDlg;

    public UI(TVDoc doc, TVRenameSplash splash, bool showUi)
    {
        CefWrapper.Instance.InitialiseBrowserFramework();

        mDoc = doc;
        scanProgDlg = null;

        busy = 0;

        mInternalChange = 0;

        internalCheckChange = false;

        InitializeComponent();
        chrSummary.RequestHandler = new BrowserRequestHandler();
        chrImages.RequestHandler = new BrowserRequestHandler();
        chrInformation.RequestHandler = new BrowserRequestHandler();
        chrMovieImages.RequestHandler = new BrowserRequestHandler();
        chrMovieInformation.RequestHandler = new BrowserRequestHandler();
        chrMovieTrailer.RequestHandler = new BrowserRequestHandler();
        chrTvTrailer.RequestHandler = new BrowserRequestHandler();

        tabControl1.ItemSize = LogicalToDeviceUnits(tabControl1.ItemSize);

        ScanAndDo = ScanAndAction;

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

        lvwScheduleColumnSorter = new ListViewColumnSorter(new DateSorterWtw(3));
        lvWhenToWatch.ListViewItemSorter = lvwScheduleColumnSorter;

        //lvwActionColumnSorter = new ListViewColumnSorter(new NumberAsTextSorter(1));

        if (mDoc.Args.Hide || !showUi)
        {
            WaitForCefInitialised();
            WindowState = FormWindowState.Minimized;
            Visible = false;
            Hide();
        }

        tmrPeriodicScan.Enabled = false;

        UpdateSplashStatus(splash, "Filling Shows",55);
        mDoc.TvLibrary.GenDict();
        FillMyShows(true);
        UpdateSplashStatus(splash, "Filling Movies",65);
        FillMyMovies();
        UpdateSearchButtons();
        SetScan(TVSettings.Instance.UIScanType);
        ClearInfoWindows();
        UpdateSplashStatus(splash, "Updating WTW",75);
        mDoc.UpdateDenormalisations();
        UpdateSplashStatus(splash, "Updating WTW",80);
        FillWhenToWatchList();
        SortSchedule(3);
        UpdateSplashStatus(splash, "Write Upcoming",85);
        mDoc.WriteUpcoming();
        UpdateSplashStatus(splash, "Write Recent",88);
        mDoc.WriteRecent();
        UpdateSplashStatus(splash, "Setting Notifications",90);
        ShowHideNotificationIcon();

        UpdateSplashStatus(splash, "Creating Monitors",92);
        mAutoFolderMonitor = new AutoFolderMonitor(mDoc, this, TVSettings.Instance.FolderMonitorDelaySeconds);
        if (TVSettings.Instance.MonitorFolders)
        {
            UpdateSplashStatus(splash, "Starting Monitor",95);
            mAutoFolderMonitor.Start();
        }

        tmrPeriodicScan.Interval = TVSettings.Instance.PeriodicCheckPeriod();
        tmrPeriodicScan.Enabled = TVSettings.Instance.RunPeriodicCheck();

        SetupObjectListForScanResults();

        if (TVSettings.Instance.RunOnStartUp()) {
            UpdateSplashStatus(splash, "Running Auto-scan",100);
        }

        SetStartUpTab();
        UpdateSplashStatus(splash, "Opening...",100);
    }

    private static void WaitForCefInitialised()
    {
        WaitFor(() => CefSharp.Cef.IsInitialized, 10, "browser to initialise",true);
    }

    private static void WaitFor(Func<bool> func, int maxSeconds, string textMessage, bool doLogging)
    {
        int numberOfWaits = 0;
        while (!func())
        {
            Wait(100);
            numberOfWaits++;
            if (doLogging && WorthLogging(numberOfWaits))
            {
                Logger.Error($"Waiting for {textMessage} {numberOfWaits}/{maxSeconds}");
            }
        }
    }

    private static bool WorthLogging(int numberOfWaits)
    {
        return numberOfWaits switch
        {
            < 20 => true,
            < 200 => numberOfWaits % 10 == 0,
            < 2000 => numberOfWaits % 100 == 0,
            < 20000 => numberOfWaits % 1000 == 0,
            < 200000 => numberOfWaits % 10000 == 0,
            < 2000000 => numberOfWaits % 100000 == 0,
            _ => numberOfWaits % 1000000 == 0
        };
    }

    private static void Wait(int milliseconds)
    {
        Timer timer1 = new();
        if (milliseconds is 0 or < 0)
        {
            return;
        }

        // Console.WriteLine("start wait timer");
        timer1.Interval = milliseconds;
        timer1.Enabled = true;
        timer1.Start();

        timer1.Tick += (_, _) =>
        {
            timer1.Enabled = false;
            timer1.Stop();
            // Console.WriteLine("stop wait timer");
        };

        while (timer1.Enabled)
        {
            Application.DoEvents();
        }
    }

    private void SetStartUpTab()
    {
        try
        {
            WaitForCefInitialised();

            int t = TVSettings.Instance.StartupTab;
            if (t < tabControl1.TabCount)
            {
                tabControl1.SelectedIndex = t;
            }

            tabControl1_SelectedIndexChanged(null, null);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to set startup Tab");
        }
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

    private void ShowChild(Form childForm)
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
        Item? ep = (Item?)rowObject;
        string? destFolder = ep?.DestinationFolder;

        if (destFolder is null)
        {
            return string.Empty;
        }

        foreach (string folder in TVSettings.Instance.LibraryFolders)
        {
            if (destFolder.StartsWith(folder, StringComparison.OrdinalIgnoreCase))
            {
                return folder;
            }
        }
        foreach (string folder in TVSettings.Instance.MovieLibraryFolders)
        {
            if (destFolder.StartsWith(folder, StringComparison.OrdinalIgnoreCase))
            {
                return folder;
            }
        }

        return destFolder;
    }

    private static object GroupSeasonKeyDelegate(object rowObject)
    {
        Item ep = (Item)rowObject;
        if (ep.Movie != null)
        {
            return GenerateShowUiName(ep.Movie);
        }

        if (ep.Series != null)
        {
            return ep.SeasonNumber.HasValue() ? $"{GenerateShowUiName(ep.Series)} - Season {ep.SeasonNumber}" : GenerateShowUiName(ep.Series);
        }

        return string.Empty;
    }

    private static string GroupDateTitleDegate(object? groupKey)
    {
        DateTime? episodeTime = (DateTime?)groupKey;

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
        DateTime? episodeTime = ((Item)rowObject).AirDate;
        DateTime now = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

        if (!episodeTime.HasValue)
        {
            return null;
        }

        if (episodeTime.Value > DateTime.Now)
        {
            return now.AddDays(1);
        }

        TimeSpan timeSince = DateTime.Now - episodeTime.Value;

        if (timeSince < 7.Days())
        {
            return now.AddDays(-1);
        }

        if (DateTime.Now.Year == episodeTime.Value.Year && DateTime.Now.Month == episodeTime.Value.Month)
        {
            return now.AddDays(-8);
        }

        if (DateTime.Now.Year == episodeTime.Value.Year)
        {
            return new DateTime(episodeTime.Value.Year, episodeTime.Value.Month, 1);
        }

        return new DateTime(episodeTime.Value.Year, 1, 1);
    }

    private string GroupItemsTitleDelegate(object groupKey)
    {
        switch ((string)groupKey)
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

            case "K-Unpack":
                return HeaderName("Unpack", mDoc.TheActionList.Count(action => action is ActionUnArchive));

            case "F-DownloadTorrent":
                return HeaderName("Start/Stop Download", mDoc.TheActionList.Count(action => action is ActionTDownload || action is ActionTRemove));

            case "E-Delete":
                return HeaderName("Remove", mDoc.TheActionList.Count(action => action is ActionDeleteFile || action is ActionDeleteDirectory));

            case "B-Rename":
                // ReSharper disable once MergeSequentialPatterns (I think it's clearer this way)
                int renameCount = mDoc.TheActionList.Count(action => action is ActionCopyMoveRename cmr && cmr.Operation == ActionCopyMoveRename.Op.rename) + mDoc.TheActionList.Count(action => action is ActionMoveRenameDirectory);
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

                List<ActionMoveRenameDirectory> x = mDoc.TheActionList.OfType<ActionMoveRenameDirectory>().ToList();

                long moveSize = moveActions.Where(item => item.From.Exists).Sum(copy => copy.From.Length);
                return HeaderName("Move", moveActions.Count + x.Count, moveSize);

            default:
                return "UNKNOWN";
        }
    }

    private static object GroupItemsKeyDelegate(object rowObject)
    {
        Item i = (Item)rowObject;
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
            "lvgActionUnpack" => "K-Unpack",
            _ => "UNKNOWN"
        };
    }

    private static string ConvertShowNameDelegate(object x)
    {
        string oringinalName = (string)x;
        return PostpendTheIfNeeded(oringinalName);
    }

    private void olv1_FormatRow(object sender, FormatRowEventArgs e)
    {
        if (e.Model is Action a)
        {
            if (a.Outcome.Error)
            {
                e.Item.BackColor = Helpers.WarningColor();
            }
        }
    }

    /*private void ReceiveArgs(string[] args)
    {
        // Send command-line arguments to already running instance

        // Parse command line arguments
        CommandLineArgs localArgs = new(new ReadOnlyCollection<string>(args));

        bool previousRenameBehavior = TVSettings.Instance.RenameCheck;
        // Temporarily override behavior for renaming folders
        TVSettings.Instance.RenameCheck = localArgs.RenameCheck;
        // Temporarily override behavior for missing folders
        mDoc.Args.TemporarilyUse(localArgs);

        ProcessArgs(localArgs);

        //Revert the settings
        TVSettings.Instance.RenameCheck = previousRenameBehavior;
        mDoc.Args.RevertFromTempUse();
    }*/

    private void ScanAndAction(TVSettings.ScanType type)
    {
        UiScan(null, null, true, type, MediaConfiguration.MediaType.both);
        ActionAction(true, true, false);
    }

    private static void UpdateSplashStatus(TVRenameSplash splashScreen, string text, int percent)
    {
        if (!splashScreen.IsHandleCreated)
        {
            return;
        }

        Logger.Info($"Splash Screen Updated with: {percent}/100 {text}");
        splashScreen.Invoke((System.Action)delegate { splashScreen.UpdateStatus(text); });
        splashScreen.Invoke((System.Action)delegate { splashScreen.UpdateProgress(percent); });
    }

    private void ClearInfoWindows() => ClearInfoWindows(string.Empty);

    private void ClearMovieInfoWindows() => ClearMovieInfoWindows(string.Empty);

    private void ClearInfoWindows(string defaultText)
    {
        SetHtmlBody(chrImages, ShowHtmlHelper.CreateOldPage(defaultText));
        SetHtmlBody(chrInformation, ShowHtmlHelper.CreateOldPage(defaultText));
        SetHtmlBody(chrSummary, ShowHtmlHelper.CreateOldPage(defaultText));
        SetHtmlBody(chrTvTrailer, ShowHtmlHelper.CreateOldPage(defaultText));
    }

    private void ClearMovieInfoWindows(string defaultText)
    {
        SetHtmlBody(chrMovieImages, ShowHtmlHelper.CreateOldPage(defaultText));
        SetHtmlBody(chrMovieInformation, ShowHtmlHelper.CreateOldPage(defaultText));
        SetHtmlBody(chrMovieTrailer, ShowHtmlHelper.CreateOldPage(defaultText));
    }

    private void MoreBusy() {
        btnScan.Enabled = false;
        tbQuickScan.Enabled = false;
        tpRecentScan.Enabled = false;
        tbFullScan.Enabled = false;

        Interlocked.Increment(ref busy);
    }

    private void LessBusy()
    {
        btnScan.Enabled = true;
        tbQuickScan.Enabled = true;
        tpRecentScan.Enabled = true;
        tbFullScan.Enabled = true;

        Interlocked.Decrement(ref busy);
    }

    private void ProcessArgs(CommandLineArgs a)
    {
        const bool UNATTENDED = true;

        if (a.Hide)
        {
            WindowState = FormWindowState.Minimized;
        }

        if (a.QuickUpdate)
        {
            UiDownload(true, UNATTENDED);
        }

        if (a.ForceUpdate)
        {
            UITVDBAccuracyCheck(UNATTENDED);
            UITMDBAccuracyCheck(UNATTENDED);
        }

        if (a.ForceRefresh)
        {
            ForceRefresh(mDoc.TvLibrary.GetSortedShowItems(), UNATTENDED);
            ForceMovieRefresh(mDoc.FilmLibrary.Movies, UNATTENDED);
        }

        if (a.Scan)
        {
            UiScan(null, null, UNATTENDED, TVSettings.ScanType.Full, MediaConfiguration.MediaType.both);
            WaitForScanToComplete();
        }

        if (a.QuickScan)
        {
            UiScan(null, null, UNATTENDED, TVSettings.ScanType.Quick, MediaConfiguration.MediaType.both);
            WaitForScanToComplete();
        }

        if (a.RecentScan)
        {
            UiScan(null, null, UNATTENDED, TVSettings.ScanType.Recent, MediaConfiguration.MediaType.both);
            WaitForScanToComplete();
        }

        if (a.DoAll)
        {
            ActionAction(true, UNATTENDED, true);
            WaitFor(() => bwAction.IsBusy == false,10000,"actions to execute", false);
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

    private void WaitForScanToComplete()
    {
        WaitFor(()=> bwScan.IsBusy == false, 10000, "scan to complete",false);
    }

    // ReSharper disable once InconsistentNaming
    private void UITVDBAccuracyCheck(bool unattended)
    {
        MoreBusy();
        TaskHelper.Run(
            () => mDoc.TVDBServerAccuracyCheck(unattended, WindowState == FormWindowState.Minimized, this), "TVDB Check"
        );
        LessBusy();
    }

    private void UpdateSearchButtons()
    {
        Searchers searchers = GetUsedSearchers();
        string name = searchers.CurrentSearch.Name;
        bool enabled = name.HasValue() && searchers.Any();

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

    private static MediaConfiguration.MediaType GetSelectedObjectType(ObjectListView list)
    {
        IList listSelectedObjects = list.SelectedObjects;
        if (listSelectedObjects.Count == listSelectedObjects.OfType<MovieItemMissing>().Count())
        {
            return MediaConfiguration.MediaType.movie;
        }

        if (listSelectedObjects.Count == (listSelectedObjects.OfType<ShowItemMissing>().Count() + listSelectedObjects.OfType<ShowSeasonMissing>().Count()))
        {
            return MediaConfiguration.MediaType.tv;
        }

        return MediaConfiguration.MediaType.both;
    }

    private void visitWebsiteToolStripMenuItem_Click(object sender, EventArgs eventArgs) =>
        Helpers.OpenUrl("http://tvrename.com");

    private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Close();

    private static bool UseCustomObject(ObjectListView view)
    {
        return view.SelectedObjects.OfType<Item>().Any(i => i.Episode?.Show.UseCustomSearchUrl == true && i.Episode.Show.CustomSearchUrl.HasValue());
    }

    private static bool UseCustom(ListView view)
    {
        foreach (ListViewItem lvi in view.SelectedItems)
        {
            if (lvi.Tag is not ProcessedEpisode pe)
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

        SetupFilterButton(filterTextBox, filterButton_Click);
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
        TVSettings.UpdateCheckMode updateCheckType = TVSettings.Instance.UpdateCheckType;
        if (updateCheckType != TVSettings.UpdateCheckMode.Off)
        {
            bool checkUpdate = true;
            if (updateCheckType == TVSettings.UpdateCheckMode.Interval)
            {
                TimeSpan lastUpdate = mDoc.CurrentAppState.UpdateCheck.LastUpdate;
                TimeSpan interval = TVSettings.Instance.UpdateCheckInterval;
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
        Button filterButton = new()
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
        NativeMethods.SendMessage(textBox.Handle, 0xd3, (IntPtr)2, (IntPtr)(filterButton.Width << 16));
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
        tsbMyMoviesContextMenu.Visible = TVSettings.Instance.ShowAccessibilityOptions;
        tsbScanContextMenu.Visible = TVSettings.Instance.ShowAccessibilityOptions;
        btnScheduleRightClick.Visible = TVSettings.Instance.ShowAccessibilityOptions;
        toolStripSeparator12.Visible = TVSettings.Instance.ShowAccessibilityOptions;
        toolStripSeparator13.Visible = TVSettings.Instance.ShowAccessibilityOptions;
        toolStripSeparator14.Visible = TVSettings.Instance.ShowAccessibilityOptions;
        toolStripSeparator16.Visible = TVSettings.Instance.ShowAccessibilityOptions;
    }

    // MAH: Added in support of the Filter TextBox Button
    private void filterButton_Click(object? sender, EventArgs e) => filterTextBox.Clear();

    private void filterMovieButton_Click(object? sender, EventArgs e) => filterMoviesTextbox.Clear();

    private ListView ListViewByName(string name)
    {
        return name switch
        {
            "WhenToWatch" => lvWhenToWatch,
            "AllInOne" => olvAction,
            _ => throw new ArgumentException("Inappropriate ListViewParameter " + name)
        };
    }

    private void flushImageCacheToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (IsBusy)
        {
            MessageBox.Show("Can't refresh until background download is complete");
            return;
        }

        UpdateImages(mDoc.TvLibrary.GetSortedShowItems());
        FillMyShows(true);
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
            "the \"Refresh\" button in the \"TV Shows\" tab.",
            "Force Refresh All", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (res == DialogResult.Yes)
        {
            TheTVDB.LocalCache.Instance.ForgetEverything();
            TVmaze.LocalCache.Instance.ForgetEverything();
            TMDB.LocalCache.Instance.ForgetEverything();
            FillMyShows(true);
            FillMyMovies();
            FillEpGuideHtml();
            FillWhenToWatchList();
            BGDownloadTimer_QuickFire();
        }
    }

    private bool LoadWidths(XElement xml)
    {
        string? forwho = xml.Attribute("For")?.Value;

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

        string? actionLayout = x.Descendants("Layout").Descendants("ActionLayout").First().Attribute("State")?.Value;
        if (actionLayout.HasValue())
        {
            olvAction.RestoreState(Convert.FromBase64String(actionLayout));
        }

        foreach (XElement widthXmlElement in x.Descendants("Layout").Descendants("ColumnWidths"))
        {
            ok = LoadWidths(widthXmlElement) && ok;
        }

        SetSplitter(x.Descendants("Layout").Descendants("Splitter").First());

        return ok;
    }

    private void SetSplitter(XElement x)
    {
        splitContainer1.SplitterDistance = int.Parse(x.Attribute("Distance")?.Value ?? "100");
        splitContainer1.Panel2Collapsed = bool.Parse(x.Attribute("HTMLCollapsed")?.Value ?? "false");
        if (splitContainer1.Panel2Collapsed)
        {
            btnHideHTMLPanel.Image = Resources.FillLeft;
        }
    }

    private void SetWindowSize(XElement x)
    {
        SetSize(x.Descendants("Size").First());
        SetLocation(x.Descendants("Location").First());

        WindowState = x.ExtractBool("Maximized", false)
            ? FormWindowState.Maximized
            : FormWindowState.Normal;
    }

    private void SetLocation(XElement x)
    {
        XAttribute? valueX = x.Attribute("X");
        XAttribute? valueY = x.Attribute("Y");

        if (valueX is null)
        {
            Logger.Error($"Missing X from {x}");
        }
        if (valueY == null)
        {
            Logger.Error($"Missing Y from {x}");
        }

        int xloc = valueX == null ? 100 : int.Parse(valueX.Value);
        int yloc = valueY is null ? 100 : int.Parse(valueY.Value);

        Location = new Point(xloc, yloc);
    }

    private void SetSize(XElement x)
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

        XmlWriterSettings settings = new()
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

    private void WriteColWidthsXml(string thingName, XmlWriter writer)
    {
        ListView lv = ListViewByName(thingName);

        writer.WriteStartElement("ColumnWidths");
        writer.WriteAttributeToXml("For", thingName);
        foreach (ColumnHeader lvc in lv.Columns)
        {
            writer.WriteElement("Width", lvc.Width);
        }

        // ReSharper disable once CommentTypo
        writer.WriteEndElement(); // ColumnWidths
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
                mAutoFolderMonitor?.Dispose();
                BGDownloadTimer.Dispose();
                UpdateTimer.Dispose();
                quickTimer.Dispose();
                refreshWTWTimer.Dispose();
                statusTimer.Dispose();
                CefWrapper.Shutdown();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message + "\r\n\r\n" + ex.StackTrace, "Form Closing Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ChooseSiteMenu(ToolStripSplitButton btn)
    {
        btn.DropDownItems.Clear();
        btn.DropDownItems.AddRange(GetUsedSearchers().Where(engine => engine.Name.HasValue()).Select(CreateSearcherMenuItem).ToArray());
    }

    private static ToolStripItem CreateSearcherMenuItem(SearchEngine search)
    {
        ToolStripMenuItem tsi = new(search.Name.ToUiVersion()) { Tag = search };
        return tsi;
    }

    internal void FillMyShows() => FillMyShows(false);
    internal void FillMyShows(bool updateSelectedNode)
    {
        ProcessedSeason? currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
        ShowConfiguration? currentSi = TreeNodeToShowItem(MyShowTree.SelectedNode);

        List<ShowConfiguration?> expanded = MyShowTree.Nodes.Cast<TreeNode>()
            .Where(n => n.IsExpanded)
            .Select(TreeNodeToShowItem).ToList();

        Logger.Info("UI: Updating MyShows");
        MyShowTree.BeginUpdate();

        MyShowTree.Nodes.Clear();
        List<ShowConfiguration> sil = mDoc.TvLibrary.Shows.ToList();
        sil.Sort((a, b) => string.Compare(GenerateShowUIName(a), GenerateShowUIName(b), StringComparison.OrdinalIgnoreCase));

        ShowFilter filter = TVSettings.Instance.Filter;
        foreach (ShowConfiguration si in sil)
        {
            if (filter.Filter(si)
                && (string.IsNullOrEmpty(filterTextBox.Text) || si.NameMatchFilters(filterTextBox.Text)))
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
            if (expanded.Contains(showAtNode))
            {
                n.Expand();
            }
        }

        if (updateSelectedNode)
        {
            if (currentSeas != null)
            {
                SelectSeason(currentSeas);
            }
            else if (currentSi != null)
            {
                SelectShow(currentSi);
            }
        }

        MyShowTree.EndUpdate();
        btnFilterMyShows.BackColor =
            filter.IsEnabled
                ? Color.LightGray
                : Color.Transparent;
    }
    private void FillMyMovies() => FillMyMovies(null);
    private void FillMyMovies(MovieConfiguration? selectedMovie)
    {
        selectedMovie ??= TreeNodeToMovieItem(movieTree.SelectedNode);

        Logger.Info("UI: Updating MyMovies");
        movieTree.BeginUpdate();

        movieTree.Nodes.Clear();
        List<MovieConfiguration> sil = mDoc.FilmLibrary.Movies.ToList();
        sil.Sort((a, b) => string.Compare(GenerateShowUiName(a), GenerateShowUiName(b), StringComparison.OrdinalIgnoreCase));

        MovieFilter filter = TVSettings.Instance.MovieFilter;
        foreach (MovieConfiguration si in sil)
        {
            if (filter.Filter(si) && (string.IsNullOrEmpty(filterMoviesTextbox.Text) || si.NameMatchFilters(filterMoviesTextbox.Text)))
            {
                AddMovieToTree(si);
            }
        }

        if (selectedMovie != null)
        {
            SelectMovie(selectedMovie);
        }

        movieTree.EndUpdate();
        btnMovieFilter.BackColor =
            filter.IsEnabled
                ?Color.LightGray
                : Color.Transparent;
    }

    internal static string GenerateShowUiName(MovieConfiguration show) => PostpendTheIfNeeded(show.ShowName);
    internal static string GenerateShowUiName(ShowConfiguration show) => PostpendTheIfNeeded(show.ShowName);

    public static string QuickStartGuide() => "https://www.tvrename.com/manual/quickstart/";

    private void ShowQuickStartGuide()
    {
        try
        {
            SetHtmlEmbed(chrInformation, QuickStartGuide());
            SetHtmlEmbed(chrImages, QuickStartGuide());
            SetHtmlEmbed(chrSummary, QuickStartGuide());
            SetHtmlEmbed(chrTvTrailer, QuickStartGuide());
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

    private static ProcessedSeason? TreeNodeToSeason(TreeNode? n) => n?.Tag as ProcessedSeason;

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

        ProcessedSeason? seas = TreeNodeToSeason(n);
        if (seas != null)
        {
            // we have a TVDB season, but need to find the equivalent one in our local processed episode collection
            if (!seas.Episodes.IsEmpty)
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
                SetHtmlBody(chrInformation, ShowHtmlHelper.CreateOldPage(si.GetSeasonHtmlOverviewOffline(s)));
                SetHtmlBody(chrImages, ShowHtmlHelper.CreateOldPage(si.GetSeasonImagesHtmlOverview(s)));
            }
            else
            {
                // no epnum specified, just show an overview
                SetHtmlBody(chrInformation, ShowHtmlHelper.CreateOldPage(si.GetShowHtmlOverviewOffline()));
                SetHtmlBody(chrImages, ShowHtmlHelper.CreateOldPage(si.GetShowImagesHtmlOverview()));
            }
            SetHtmlBody(chrSummary, ShowHtmlHelper.CreateOldPage("Not available offline"));
            SetHtmlBody(chrTvTrailer, ShowHtmlHelper.CreateOldPage("Not available offline"));

            return;
        }

        if (snum >= 0 && si.AppropriateSeasons().ContainsKey(snum))
        {
            ProcessedSeason s = si.AppropriateSeasons()[snum];
            SetHtmlBody(chrImages, s.GetSeasonImagesOverview());
            SetHtmlBody(chrInformation, si.GetSeasonHtmlOverview(s, false));
            SetHtmlBody(chrSummary, si.GetSeasonSummaryHtmlOverview(s, false));
            UpdateTvTrailer(si);

            ResetRunBackGroundWorker(bwSeasonHTMLGenerator, s);
            ResetRunBackGroundWorker(bwSeasonSummaryHTMLGenerator, s);
        }
        else
        {
            // no epnum specified, just show an overview
            SetHtmlBody(chrImages, si.GetShowImagesOverview());
            SetHtmlBody(chrInformation, si.GetShowHtmlOverview(false));
            SetHtmlBody(chrSummary, si.GetShowSummaryHtmlOverview(false));
            UpdateTvTrailer(si);

            ResetRunBackGroundWorker(bwShowHTMLGenerator, si);
            ResetRunBackGroundWorker(bwShowSummaryHTMLGenerator, si);
        }
    }

    private static void ResetRunBackGroundWorker(BackgroundWorker worker, object s)
    {
        if (worker.WorkerSupportsCancellation)
        {
            // Cancel the asynchronous operation.
            worker.CancelAsync();
        }

        if (!worker.IsBusy)
        {
            worker.RunWorkerAsync(s);
        }
    }

    private void UpdateTvTrailer(ShowConfiguration? si)
    {
        if (si?.CachedShow?.TrailerUrl?.HasValue() ?? false)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            SetHtmlEmbed(chrTvTrailer, ShowHtmlHelper.YoutubeTrailer(si.CachedShow!));
        }
        else
        {
            SetHtmlBody(chrTvTrailer, ShowHtmlHelper.CreateOldPage("Not available for this TV show"));
        }
    }

    private static void SetWeb(ChromiumWebBrowser web, System.Action a)
    {
        web.Visible = true;
        if (web.IsDisposed)
        {
            return;
        }

        if (!web.IsBrowserInitialized)
        {
            web.IsBrowserInitializedChanged += (_, _) =>
            {
                web.BeginInvoke((MethodInvoker)delegate { SetWeb(web, a); });
            };
        }

        try
        {
            a();
        }
        catch (COMException ex)
        {
            //Fail gracefully - no RHS episode guide is not too big of a problem.
            Logger.Warn(ex, "Could not update UI for the show/cachedSeries/movie information pane");
        }
        catch (NullReferenceException ex)
        {
            //Fail gracefully - no RHS episode guide is not too big of a problem.
            Logger.Warn(ex, "Could not update UI for the show/cachedSeries/movie information pane");
        }
        catch (Exception ex)
        {
            //Fail gracefully - no RHS episode guide is not too big of a problem.
            Logger.Error(ex);
        }
        web.Update();
    }

    public static void SetHtmlBody(ChromiumWebBrowser web, string body)
    {
        SetWeb(web,
            () =>
            {
                web.Load("data:text/html;base64," + Convert.ToBase64String(Encoding.UTF8.GetBytes(body)));
            });
    }

    private static void SetHtmlEmbed(ChromiumWebBrowser web, string? link)
    {
        SetWeb(web,
            () =>
            {
                web.Load(link??string.Empty);
            });
    }

    public static void TvSourceFor(ProcessedEpisode? e)
    {
        if (e?.WebsiteUrl != null && e.WebsiteUrl.HasValue())
        {
            Helpers.OpenUrl(e.WebsiteUrl);
        }
        else
        {
            TvSourceFor(e?.AppropriateProcessedSeason);
        }
    }

    public static void TvSourceFor(ProcessedSeason? seas)
    {
        if (seas?.WebsiteUrl != null && seas.WebsiteUrl.HasValue())
        {
            Helpers.OpenUrl(seas.WebsiteUrl);
        }
        else
        {
            TvSourceFor(seas?.Show);
        }
    }

    public static void TvSourceFor(ShowConfiguration? si)
    {
        if (si != null)
        {
            if (si.WebsiteUrl.HasValue())
            {
                Helpers.OpenUrl(si.WebsiteUrl);
            }
            else if (si.CachedShow?.WebUrl.HasValue() ?? false)
            {
                Helpers.OpenUrl(si.CachedShow?.WebUrl!);
            }
        }
    }

    private void menuSearchSites_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
        if (GetUsedSearchers() == TVDoc.GetMovieSearchers())
        {
            mDoc.SetMovieSearcher((SearchEngine)e.ClickedItem.Tag);
        }
        else
        {
            mDoc.SetSearcher((SearchEngine)e.ClickedItem.Tag);
        }

        UpdateSearchButtons();
    }

    private void bnWhenToWatchCheck_Click(object? sender, EventArgs? e) => RefreshWTW(true, false);

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

    private List<ListViewItem> GenerateNewScheduleItems()
    {
        int dd = TVSettings.Instance.WTWRecentDays;
        DirFilesCache dfc = new();

        IEnumerable<ProcessedEpisode> recentEps = mDoc.TvLibrary.GetRecentAndFutureEps(dd);
        return recentEps.Select(ei => GenerateLvi(dfc, ei)).ToList();
    }

    private ListViewItem GenerateLvi(DirFilesCache dfc, ProcessedEpisode pe)
    {
        ListViewItem lvi = new()
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
        lvi.SubItems.Add(pe.TheCachedSeries.Networks.FirstOrDefault());
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

    private void lvWhenToWatch_ColumnClick(object sender, ColumnClickEventArgs e)
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

        ProcessedEpisode ei = (ProcessedEpisode)lvWhenToWatch.Items[n].Tag;
        switchToWhenOpenMyShows = ei;

        if (TVSettings.Instance.HideWtWSpoilers &&
            (ei.HowLong() != "Aired" || lvWhenToWatch.Items[n].ImageIndex == 1))
        {
            txtWhenToWatchSynopsis.Text = Resources.Spoilers_Hidden_Text;
        }
        else if (ei.Type == ProcessedEpisode.ProcessedEpisodeType.merged)
        {
            txtWhenToWatchSynopsis.Text = string.Join(Environment.NewLine + Environment.NewLine, ei.SourceEpisodes.Select(episode => episode.Overview?.ToUiVersion()));
        }
        else
        {
            txtWhenToWatchSynopsis.Text = ei.Overview?.ToUiVersion();
        }

        mInternalChange++;
        DateTime? dt = ei.GetAirDateDt(true);
        if (dt != null)
        {
            calCalendar.SelectionStart = (DateTime)dt;
            calCalendar.SelectionEnd = (DateTime)dt;
        }

        mInternalChange--;
    }

    private void lvWhenToWatch_DoubleClick(object sender, EventArgs e)
    {
        if (lvWhenToWatch.SelectedItems.Count == 0)
        {
            return;
        }

        ProcessedEpisode ei = (ProcessedEpisode)lvWhenToWatch.SelectedItems[0].Tag;
        List<FileInfo> fl = FinderHelper.FindEpOnDisk(null, ei);
        if (fl.Any())
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
                UiScan(new List<ShowConfiguration> { ei.Show }, null, false, TVSettings.ScanType.SingleShow, MediaConfiguration.MediaType.tv);
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

            ProcessedEpisode ei = (ProcessedEpisode)lvi.Tag;
            DateTime? dt2 = ei.GetAirDateDt(true);
            if (dt2 != null)
            {
                double h = dt2.Value.Subtract(dt).TotalHours;
                if (h is >= 0 and < 24.0)
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
    private void RefreshWTW(bool doDownloads, bool unattended)
    {
        UiDownload(doDownloads, unattended);

        FillMyShows(true);
        FillMyMovies();

        FillWhenToWatchList();
    }

    private void UiDownload(bool doDownloads, bool unattended)
    {
        mDoc.UpdateMedia(doDownloads, unattended, WindowState == FormWindowState.Minimized, this);
    }

    private void refreshWTWTimer_Tick(object sender, EventArgs e)
    {
        if (IsBusy)
        {
            return;
        }

        RefreshWTW(false, true);
    }

    // ReSharper disable once InconsistentNaming
    private void UpdateToolstripWTW()
    {
        // update toolstrip text too
        List<ProcessedEpisode> next1 = mDoc.TvLibrary.NextNShows(1, 0, 36500);

        tsNextShowTxt.Text = "Next airing: ";
        if (next1.Any())
        {
            ProcessedEpisode ei = next1.First();
            tsNextShowTxt.Text += $"{CustomEpisodeName.NameForNoExt(ei, CustomEpisodeName.OldNStyle(1))}, {ei.HowLong()} ({ei.DayOfWeek()}, {ei.TimeOfDay()})".ToUiVersion();
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
            TVDoc.SearchForEpisode((ProcessedEpisode)lvi.Tag);
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
        UpcomingPopup up = new(mDoc);
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
        BuyMeADrink bmad = new();
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

    public void GotoEpguideFor(ProcessedEpisode ep, bool changeTab)
    {
        if (changeTab)
        {
            tabControl1.SelectTab(tbMyShows);
        }

        SelectSeason(ep.AppropriateProcessedSeason);
    }

    public void GotoMovieFor(MovieConfiguration mc, bool changeTab)
    {
        if (changeTab)
        {
            tabControl1.SelectTab(tbMyMovies);
        }

        SelectMovie(mc);
    }

    private void RightClickOnMyShows(ShowConfiguration si, Point pt)
    {
        BuildRightClickMenu(pt, null, new List<ShowConfiguration> { si }, null);
    }

    private void RightClickOnMyShows(ProcessedSeason seas, Point pt)
    {
        BuildRightClickMenu(pt, null, new List<ShowConfiguration> { seas.Show }, seas);
    }

    private void WtwRightClickOnShow(List<ProcessedEpisode> eps, Point pt)
    {
        if (eps.Count == 0)
        {
            return;
        }

        ProcessedEpisode? ep = eps.Count == 1 ? eps[0] : null;

        List<ShowConfiguration> sis = eps.Select(e => e.Show).ToList();

        BuildRightClickMenu(pt, ep, sis, ep?.AppropriateProcessedSeason);
    }

    private void MenuGuideAndTvdb(ProcessedEpisode? ep, ShowConfiguration si, ProcessedSeason? seas)
    {
        if (ep != null)
        {
            AddRcMenuItem("Episode Guide", (_, _) => GotoEpguideFor(ep, true));
            string label = $"Visit {ep.Show.Provider.PrettyPrint()}...";
            AddRcMenuItem(label, (_, _) => TvSourceFor(ep));
        }
        else if (seas != null)
        {
            AddRcMenuItem("Episode Guide", (_, _) => GotoEpguideFor(seas.Show, true));
            string label = $"Visit {seas.Show.Provider.PrettyPrint()}...";
            AddRcMenuItem(label, (_, _) => TvSourceFor(seas));
        }
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        else
        {
            AddRcMenuItem("Episode Guide", (_, _) => GotoEpguideFor(si, true));
            string label = $"Visit {si.Provider.PrettyPrint()}...";
            AddRcMenuItem(label, (_, _) => TvSourceFor(si));
        }
    }

    private void RightMenuScan(List<ShowConfiguration> sil, TVSettings.ScanType x)
    {
        UiScan(sil, null, false, x, MediaConfiguration.MediaType.tv);
        tabControl1.SelectTab(tbAllInOne);
    }
    private void RightMenuMovieScan(MovieConfiguration si, TVSettings.ScanType x)
    {
        UiScan(null, new List<MovieConfiguration> { si }, false, x, MediaConfiguration.MediaType.movie);
        tabControl1.SelectTab(tbAllInOne);
    }

    private void MenuFolders(ItemList? lvr, ShowConfiguration? si, ProcessedSeason? seas, ProcessedEpisode? ep)
    {
        List<string> added = new();

        if (ep != null)
        {
            Dictionary<int, SafeList<string>> afl = ep.Show.AllExistngFolderLocations();
            if (afl.ContainsKey(ep.AppropriateSeasonNumber))
            {
                AddFolders(afl[ep.AppropriateSeasonNumber], added);
            }
        }
        else if (seas != null)
        {
            Dictionary<int, SafeList<string>>? folders = si?.AllExistngFolderLocations();

            if (folders?.ContainsKey(seas.SeasonNumber) == true)
            {
                AddFolders(folders[seas.SeasonNumber], added);
            }
        }

        if (si != null)
        {
            if (si.AutoAddFolderBase.HasValue())
            {
                AddFolders(new[] { si.AutoAddFolderBase }, added);
            }
            AddFoldersSubMenu(si.AllExistngFolderLocations().Values.SelectMany(l => l).ToList(), added);
        }

        if (lvr is null || lvr.Count > 1)
        {
            return;
        }

        {
            AddFoldersSubMenu(lvr.Select(sli => sli.TargetFolder).OfType<string>(), added);
        }
    }

    private void AddFolders(IEnumerable<string> foldersList, ICollection<string> alreadyAdded)
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

            ToolStripMenuItem tsi = new("Open: " + folder.ToUiVersion());
            tsi.Click += (_, _) =>
            {
                Helpers.OpenFolder(folder);
            };

            showRightClickMenu.Items.Add(tsi);
        }
    }

    private void AddFoldersSubMenu(IEnumerable<string> foldersList, ICollection<string> alreadyAdded)
    {
        ToolStripMenuItem tsi = new("Open Other Folders");

        foreach (string folder in foldersList.Where(folder => !string.IsNullOrEmpty(folder) && Directory.Exists(folder) && !alreadyAdded.Contains(folder)))
        {
            alreadyAdded.Add(folder); // don't show the same folder more than once

            ToolStripMenuItem tssi = new("Open: " + folder.ToUiVersion());
            tssi.Click += (_, _) =>
            {
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

        AddRcMenuItem("Force Refresh", (_, _) => ForceMovieRefresh(si, false));
        AddRcMenuItem("Update Images", (_, _) => UpdateImages(new List<MovieConfiguration> { si }));

        showRightClickMenu.Items.Add(new ToolStripSeparator());

        AddRcMenuItem("Edit Movie", (_, _) => EditMovie(si));
        AddRcMenuItem("Delete Movie", (_, _) => DeleteMovie(si));

        showRightClickMenu.Items.Add(new ToolStripSeparator());

        AddRcMenuItem($"Scan \"{si.ShowName}\"", (_, _) => RightMenuMovieScan(si, TVSettings.ScanType.SingleShow));
        AddRcMenuItem($"Quick Scan \"{si.ShowName}\"", (_, _) => RightMenuMovieScan(si, TVSettings.ScanType.FastSingleShow));

        if (si.Locations.Any())
        {
            List<string> added = new();
            AddFolders(si.Locations, added);
        }

        showRightClickMenu.Show(pt);
    }

    private void BuildRightClickMenu(Point pt, ProcessedEpisode? ep, List<ShowConfiguration> sil, ProcessedSeason? seas)
    {
        showRightClickMenu.Items.Clear();

        if (sil.Count == 1)
        {
            MenuGuideAndTvdb(ep, sil[0], seas);
            AddRcMenuItem("Schedule", (_, _) => GotoWtwFor(sil[0]));
        }

        ShowConfiguration? si = sil.Count >= 1 ? sil[0] : null;

        if (sil.Any())
        {
            AddRcMenuItem("Force Refresh", (_, _) => ForceRefresh(sil, false));
            AddRcMenuItem("Update Images", (_, _) => UpdateImages(sil));
            showRightClickMenu.Items.Add(new ToolStripSeparator());

            string scanText = sil.Count > 1 || si is null
                ? "Scan Multiple Shows"
                : "Scan \"" + si.ShowName + "\"";

            AddRcMenuItem(scanText, (_, _) => RightMenuScan(sil, TVSettings.ScanType.SingleShow));
            AddRcMenuItem($"Quick {scanText}", (_, _) => RightMenuScan(sil, TVSettings.ScanType.FastSingleShow));
        }

        if (sil.Count == 1 && si!= null)
        {
            AddRcMenuItem("Edit TV Show", (_, _) => EditShow(si));
            AddRcMenuItem("Delete TV Show", (_, _) => DeleteShow(si));

            if (seas != null)
            {
                string uiFullSeasonWord = ProcessedSeason.UIFullSeasonWord(seas.SeasonNumber);
                AddRcMenuItem("Edit " + uiFullSeasonWord, (_, _) => EditSeason(si, seas.SeasonNumber));
                if (si.IgnoreSeasons.Contains(seas.SeasonNumber))
                {
                    AddRcMenuItem("Include " + uiFullSeasonWord, (_, _) => IncludeSeason(si, seas.SeasonNumber));
                }
                else
                {
                    AddRcMenuItem("Ignore " + uiFullSeasonWord, (_, _) => IgnoreSeason(si, seas.SeasonNumber));
                }
            }

            if (ep != null )
            {
                AddEpisodesMenu(ep);
            }
            else if (seas != null)
            {
                AddSeasonEpisodesMenu(seas, si);
            }

            MenuFolders(null, sil[0], seas, ep);
        }

        if (ep != null)
        {
            showRightClickMenu.Items.Add(new ToolStripSeparator());
            MenuSearchFor(ep);
        }

        showRightClickMenu.Show(pt);
    }

    private void AddEpisodesMenu(ProcessedEpisode ep)
    {
        List<FileInfo> fl = FinderHelper.FindEpOnDisk(null, ep);
        if (fl.Any())
        {
            showRightClickMenu.Items.Add(new ToolStripSeparator());

            foreach (FileInfo fi in fl)
            {
                ToolStripMenuItem tsi = new("Watch: " + fi.FullName.ToUiVersion());
                tsi.Click += (_, _) => { Helpers.OpenFile(fi.FullName); };
                showRightClickMenu.Items.Add(tsi);
            }
        }
    }

    private void AddSeasonEpisodesMenu(ProcessedSeason seas, ShowConfiguration si)
    {
        ToolStripMenuItem tsis = new("Watch Episodes");

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
                    ToolStripMenuItem tsisi = new("Watch: " + fi.FullName.ToUiVersion());
                    tsisi.Click += (_, _) => { Helpers.OpenFile(fi.FullName); };

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

    private void showRightClickMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
        showRightClickMenu.Close();
    }

    private ItemList GetSelectedItems() => new() { olvAction.SelectedObjects.OfType<Item>() };

    private ItemList GetCheckedItems() => new() { mDoc.TheActionList.Checked.Intersect(olvAction.FilteredObjects.OfType<Item>()) };

    private void IncludeSeason(ShowConfiguration si, int seasonNumber)
    {
        si.IgnoreSeasons.Remove(seasonNumber);
        ShowAddedOrEdited(false, false, si,true);
    }

    private void IgnoreSeason(ShowConfiguration si, int seasonNumber)
    {
        si.IgnoreSeasons.Add(seasonNumber);
        ShowAddedOrEdited(false, false, si, true);
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

    private void ManuallyAddFileForItem(ItemMissing mi, string fileName)
    {
        mDoc.UpdateMissingAction(mi, fileName);
        FillActionList();
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

        FillMyShows(true);
        FillActionList();
    }

    private void GotoWtwFor(ShowConfiguration show)
    {
        tabControl1.SelectTab(tbWTW);
        foreach (ListViewItem lvi in lvWhenToWatch.Items)
        {
            lvi.Selected = lvi.Tag is ProcessedEpisode ei && ei.TheCachedSeries.IsCacheFor(show);
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

    private void lvWhenToWatch_MouseClick(object sender, MouseEventArgs e)
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
        List<ProcessedEpisode> eis = lvWhenToWatch.SelectedItems.Cast<ListViewItem>().Select(lvi => lvi.Tag).OfType<ProcessedEpisode>().ToList();

        WtwRightClickOnShow(eis, pt);
    }

    private void preferencesToolStripMenuItem_Click(object sender, EventArgs e) => DoPrefs(false);

    private void DoPrefs(bool scanOptions)
    {
        MoreBusy(); // no background download while preferences are open!
        mDoc.PreventAutoScan("Preferences are open");

        Preferences pref = new(mDoc, scanOptions, CurrentlySelectedSeason());
        if (pref.ShowDialog(this) == DialogResult.OK)
        {
            mDoc.SetDirty();
            TVDoc.Reconnect();
            ShowHideNotificationIcon();
            FillWhenToWatchList();
            ShowInTaskbar = TVSettings.Instance.ShowInTaskbar;
            EnableDisableAccessibilty();
            FillEpGuideHtml();
            mAutoFolderMonitor?.SettingsChanged(TVSettings.Instance.MonitorFolders);
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

            Logger.Error(e2, "Failed to Save Layout Configuration Files");
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
                         TVmaze.LocalCache.Instance.CurrentDLTask ??
                         TMDB.LocalCache.Instance.CurrentDLTask;
        bool somethingDownloading = dlTask.HasValue();

        txtDLStatusLabel.Visible = n != 0 || TVSettings.Instance.BGDownload || somethingDownloading;

        backgroundDownloadNowToolStripMenuItem.Enabled = n == 0;

        if (somethingDownloading)
        {
            txtDLStatusLabel.Text = "Background download: " + dlTask?.ToUiVersion();
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
            RefreshWTW(false, true);

            backgroundDownloadNowToolStripMenuItem.Enabled = true;
        }

        lastDlRemaining = n;
    }

    private static void SaveCaches()
    {
        TVDoc.SaveCaches();
    }

    private void backgroundDownloadToolStripMenuItem_Click(object sender, EventArgs e)
    {
        TVSettings.Instance.BGDownload = !TVSettings.Instance.BGDownload;
        backgroundDownloadToolStripMenuItem.Checked = TVSettings.Instance.BGDownload;
        offlineOperationToolStripMenuItem.Checked = TVSettings.Instance.OfflineMode;
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

        Task<ServerRelease?> tuv = VersionUpdater.CheckForUpdatesAsync();
        ServerRelease? result = await tuv.ConfigureAwait(false);

        mDoc.CurrentAppState.UpdateCheck.LastUpdateCheckUtc = DateTime.UtcNow;
        try
        {
            mDoc.CurrentAppState.SaveToDefaultFile();
        }
        catch (Exception ex)
        {
            Logger.Warn(ex, "Could not save app state file after update check!");
        }

        uiDisp.Invoke(() => NotifyUpdates(result, false, mDoc.Args.Unattended || mDoc.Args.Hide));
    }

    private void BGDownloadTimer_Tick(object sender, EventArgs e)
    {
        if (IsBusy)
        {
            BGDownloadTimer.Interval = 60000; // come back in 60 seconds
            BGDownloadTimer.Start();
            Logger.Info("BG Download is busy - try again in 60 seconds");
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
            if (MessageBox.Show("Are you sure you wish to go offline?", "TV Rename", MessageBoxButtons.YesNo,
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
        BugReport br = new(mDoc);
        br.ShowDialog(this);
    }

    private void ShowHideNotificationIcon()
    {
        notifyIcon1.Visible = TVSettings.Instance.NotificationAreaIcon && !mDoc.Args.Hide;
    }

    private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        StatsWindow sw = new(mDoc.Stats());
        sw.ShowDialog(this);
    }

    private TreeNode AddShowItemToTree(ShowConfiguration si)
    {
        CachedSeriesInfo? ser = si.CachedShow;

        TreeNode n = new(GenerateShowUIName(ser, si)) { Tag = si };

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

                TreeNode n2 = new(nodeTitle);
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

    private void AddMovieToTree(MovieConfiguration si)
    {
        movieTree.Nodes.Add(new TreeNode(GenerateShowUiName(si)) { Tag = si });
    }

    // ReSharper disable once InconsistentNaming
    private static string GenerateShowUIName(ProcessedEpisode? episode) => GenerateShowUIName(episode?.TheCachedSeries, episode?.Show);

    // ReSharper disable once InconsistentNaming
    private static string GenerateShowUIName(ShowConfiguration si)
    {
        CachedSeriesInfo? s = si.CachedShow;
        return GenerateShowUIName(s, si);
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
        string? name = si?.ShowName;

        if (!string.IsNullOrEmpty(name))
        {
            return name;
        }

        if (ser != null)
        {
            return ser.Name;
        }

        return "-- Unknown : " + si?.Code + " --";
    }

    private static (Color, Color) GetWtwColour(ProcessedEpisode ep, DateTime dt)
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
            return (Color.LightGray, Color.Black);
        }

        return (Color.LightBlue, Color.Black);
    }

    private static string GetEpisodeNumber(ProcessedEpisode pe)
    {
        if (pe.AppropriateEpNum > 0 && pe.EpNum2 != pe.AppropriateEpNum && pe.EpNum2 > 0)
        {
            return pe.AppropriateEpNum + "-" + pe.EpNum2;
        }

        return pe.AppropriateEpNum > 0 ? pe.AppropriateEpNum.ToString() : string.Empty;
    }

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

    private static int? ChooseWtwIcon(DirFilesCache dfc, ProcessedEpisode pe, DateTime airdt)
    {
        List<FileInfo> fl = dfc.FindEpOnDisk(pe);
        bool appropriateFileNameFound = !TVSettings.Instance.RenameCheck
                                        || !pe.Show.DoRename
                                        || fl.All(file => file.Name.StartsWith(TVSettings.Instance.FilenameFriendly(TVSettings.Instance.NamingStyle.NameFor(pe)), StringComparison.OrdinalIgnoreCase));

        if (fl.Any() && appropriateFileNameFound)
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
        TreeNode? n2 = MyShowTree.Nodes
                     .Cast<TreeNode>()
                     .Where(n => NodeIsForShow(seas.Show, n))
                     .SelectMany(n => n.Nodes.OfType<TreeNode>())
                     .FirstOrDefault(n2 => TreeNodeToSeason(n2)?.SeasonNumber == seas.SeasonNumber);

        if (n2 == null)
        {
            FillEpGuideHtml(null);
        }
        else
        {
            n2.EnsureVisible();
            MyShowTree.SelectedNode = n2;
        }
    }

    private static bool NodeIsForShow(ShowConfiguration si, TreeNode n)
    {
        return TreeNodeToShowItem(n)?.IdFor(si.Provider) == si.IdFor(si.Provider);
    }

    private void SelectShow(ShowConfiguration si)
    {
        TreeNode? n = MyShowTree.Nodes.Cast<TreeNode>().FirstOrDefault(n => NodeIsForShow(si, n));

        if (n is null)
        {
            FillEpGuideHtml(null);
        }
        else
        {
            n.EnsureVisible();
            MyShowTree.SelectedNode = n;
        }
    }

    private void SelectMovie(MovieConfiguration m)
    {
        TreeNode? n = movieTree.Nodes.Cast<TreeNode>().FirstOrDefault(n => TreeNodeToMovieItem(n) == m);
        if (n is null)
        {
            return;
        }
        n.EnsureVisible();
        movieTree.SelectedNode = n;
    }

    private void AddMovie_Click(object sender, EventArgs e)
    {
        Logger.Info("****************");
        Logger.Info("Adding New Movie");
        MoreBusy();
        mDoc.PreventAutoScan("Add Movie");
        MovieConfiguration mov = new();

        AddEditMovie aem = new(mov, mDoc);
        DialogResult dr = aem.ShowDialog(this);
        if (dr == DialogResult.OK)
        {
            mDoc.Add(mov.AsList(),false);
            FillMyMovies(mov);

            mDoc.MoviesAddedOrEdited(true, false, WindowState == FormWindowState.Minimized, this, mov);
            FillMyMovies(mov);

            Logger.Info($"Added new movie called {mov.ShowName}");
        }
        else
        {
            Logger.Info("Cancelled adding new movie");
        }

        LessBusy();
        mDoc.AllowAutoScan();
    }

    private void bnMyShowsAdd_Click(object sender, EventArgs e)
    {
        Logger.Info("****************");
        Logger.Info("Adding New TV Show");
        MoreBusy();
        mDoc.PreventAutoScan("Add TV Show");
        ShowConfiguration si = new();

        AddEditShow aes = new(si, mDoc);
        DialogResult dr = aes.ShowDialog(this);
        if (dr == DialogResult.OK)
        {
            mDoc.Add(si.AsList(),false);

            ShowAddedOrEdited(false, false, si,false);
            SelectShow(si);
            ShowAddedOrEdited(true, false, si, false);
            Logger.Info($"Added new show called {si.ShowName}" );
        }
        else
        {
            Logger.Info("Cancelled adding new tv show");
        }
        LessBusy();
        mDoc.AllowAutoScan();
    }

    private void ShowAddedOrEdited(bool download, bool unattended, ShowConfiguration si, bool updateSelectedNode)
    {
        mDoc.TvAddedOrEdited(download, unattended, WindowState == FormWindowState.Minimized, this, si);

        FillMyShows(updateSelectedNode);
        FillWhenToWatchList();
    }

    private void bnMyShowsDelete_Click(object sender, EventArgs e)
    {
        TreeNode n = MyShowTree.SelectedNode;
        ShowConfiguration? si = TreeNodeToShowItem(n);
        if (si is null)
        {
            return;
        }

        DeleteShow(si);
    }

    private void DeleteShow(ShowConfiguration si)
    {
        DialogResult res = MessageBox.Show(
            "Remove show \"" + si.ShowName + "\".  Are you sure?", "Confirmation", MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (res != DialogResult.Yes)
        {
            return;
        }

        if (TVSettings.Instance.DeleteShowFromDisk)
        {
            RemoveFromDisk(si.AutoAddFolderBase, si);
            if (si.UsesManualFolders())
            {
                foreach (string directory in si.ManualFolderLocations.Values.SelectMany(x => x))
                {
                    RemoveFromDisk(directory, si);
                }
            }
        }

        Logger.Info($"User asked to remove {si.ShowName} - removing now");
        mDoc.TvLibrary.Remove(si);
        ShowAddedOrEdited(false, false, si, true);
    }

    private void RemoveFromDisk(string folderName,MediaConfiguration si)
    {
        if (!Directory.Exists(folderName))
        {
            return;
        }

        if (FileHelper.IsRootDirectory(folderName))
        {
            Logger.Warn($"Did not remove {folderName} as it is a root folder");
            return;
        }

        if (TVSettings.Instance.LibraryFolders.Any(x=>x.IsSubfolderOf(folderName)))
        {
            Logger.Warn($"Did not remove {folderName} as it is a library folder");
            return;
        }
        if (TVSettings.Instance.MovieLibraryFolders.Any(x => x.IsSubfolderOf(folderName)))
        {
            Logger.Warn($"Did not remove {folderName} as it is a movie library folder");
            return;
        }

        DialogResult res3 = MessageBox.Show(
            $"Remove folder \"{folderName}\" from disk?",
            "Confirmation",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (res3 != DialogResult.Yes)
        {
            return;
        }
        try {
            IEnumerable<string> videofilesThatWouldBeDeleted = Directory
                .GetFiles(folderName, "*", System.IO.SearchOption.AllDirectories)
                .Where(f => f.IsMovieFile())
                .Select(s=>s.TrimStartString(folderName));

            List<(ShowConfiguration,string)> showsthatmatchanyfiles =
                mDoc.TvLibrary.Shows
                    .Where(show => show != si)
                    .SelectMany(_ => videofilesThatWouldBeDeleted, (show, filename) => new { show, filename })
                    .Where(t => t.show.NameMatch(t.filename))
                    .Select(t => (t.show, t.filename))
                    .ToList();

            if (showsthatmatchanyfiles.Any())
            {
                string otherFilesDetails = showsthatmatchanyfiles.Select(m => $"{m.Item1.ShowName}:[{m.Item2}]").ToCsv();

                DialogResult res1 = MessageBox.Show(
                    $"Do you want to remove {folderName}?  It contains files that match other shows {otherFilesDetails}",
                    "Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Hand);

                if (res1 != DialogResult.Yes)
                {
                    Logger.Warn($"Did not remove {folderName} as it contains files that match other shows {otherFilesDetails}");
                    return;
                }
            }

            List<(MovieConfiguration,string)> moviesthatmatchanyfiles = mDoc.FilmLibrary.Movies
                .Where(show => show != si)
                .SelectMany(_ => videofilesThatWouldBeDeleted, (show, filename) => new { show, filename })
                .Where(t => t.show.NameMatch(t.filename))
                .Select(t => (t.show, t.filename))
                .ToList();

            if (moviesthatmatchanyfiles.Any())
            {
                string otherFilesDetails = moviesthatmatchanyfiles.Select(m => $"{m.Item1.ShowName}:[{m.Item2}]").ToCsv();
                DialogResult res2 = MessageBox.Show(
                    $"Do you want to remove {folderName}?  It contains files that match other movies {otherFilesDetails}",
                    "Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Hand);

                if (res2 != DialogResult.Yes)
                {
                    Logger.Warn($"Did not remove {folderName} as contains files that match other movies {otherFilesDetails}");
                    return;
                }
            }

            Logger.Info($"Recycling {folderName} as part of the removal of {si.Name}");
            Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(folderName, //TODO make all one use of the folder removal
                Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
        }
        catch (OperationCanceledException e)
        {
            Logger.Warn($"Failed to remove {folderName} as operation was cancelled: {e.Message}");
        }
        catch (System.IO.DirectoryNotFoundException e)
        {
            Logger.Warn($"Failed to remove {folderName} as it is not found: {e.Message}");
        }
        catch (UnauthorizedAccessException e)
        {
            Logger.Warn($"Failed to remove {folderName} we could not access it (or a subfolder): {e.Message}");
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Failed to remove {folderName} as operation failed");
        }
    }

    private void DeleteMovie(MovieConfiguration si)
    {
        DialogResult res = MessageBox.Show(
            "Remove movie \"" + si.ShowName + "\".  Are you sure?", "Confirmation", MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (res != DialogResult.Yes)
        {
            return;
        }

        if (TVSettings.Instance.DeleteMovieFromDisk && si.Format!= MovieConfiguration.MovieFolderFormat.multiPerDirectory)
        {
            foreach (string directory in si.Locations)
            {
                RemoveFromDisk(directory, si);
            }
        }

        Logger.Info($"User asked to remove {si.ShowName} - removing now");
        mDoc.FilmLibrary.Remove(si);
        mDoc.RunExporters();

        FillMyMovies();
    }

    private void bnMyShowsEdit_Click(object sender, EventArgs e)
    {
        TreeNode n = MyShowTree.SelectedNode;
        if (n is null)
        {
            return;
        }

        ProcessedSeason? seas = TreeNodeToSeason(n);
        if (seas != null)
        {
            ShowConfiguration? si = TreeNodeToShowItem(n);
            if (si != null)
            {
                EditSeason(si, seas.SeasonNumber);
            }

            return;
        }

        ShowConfiguration? si2 = TreeNodeToShowItem(n);
        if (si2 != null)
        {
            EditShow(si2);
        }
    }

    internal void EditSeason(ShowConfiguration si, int seasnum)
    {
        MoreBusy();
        mDoc.PreventAutoScan("Edit Season");

        EditSeason er = new(si, seasnum, TVSettings.Instance.NamingStyle);
        DialogResult dr = er.ShowDialog(this);
        if (dr == DialogResult.OK)
        {
            ShowAddedOrEdited(false, false, si, true);
            SelectSeason(si.AppropriateSeasons()[seasnum]);
        }

        mDoc.AllowAutoScan();
        LessBusy();
    }

    internal void EditShow(ShowConfiguration si)
    {
        MoreBusy();
        mDoc.PreventAutoScan("Edit TV Show");

        AddEditShow aes = new(si, mDoc);
        DialogResult dr = aes.ShowDialog(this);

        if (dr == DialogResult.OK)
        {
            ShowAddedOrEdited(aes.HasChanged, false, si, true);
            SelectShow(si);

            Logger.Info($"Modified show called {si.ShowName}");
        }

        mDoc.AllowAutoScan();
        LessBusy();
    }

    internal void EditMovie(MovieConfiguration si)
    {
        MoreBusy();
        mDoc.PreventAutoScan("Edit Movie");

        AddEditMovie aes = new(si, mDoc);
        DialogResult dr = aes.ShowDialog(this);

        if (dr == DialogResult.OK)
        {
            mDoc.MoviesAddedOrEdited(aes.HasChanged, false, WindowState == FormWindowState.Minimized, this, si);

            FillMyMovies();
            SelectMovie(si);

            Logger.Info($"Modified movie called {si.ShowName}" );
        }

        mDoc.AllowAutoScan();
        LessBusy();
    }

    internal void ForceRefresh(IEnumerable<ShowConfiguration>? sis, bool unattended)
    {
        mDoc.ForceRefreshShows(sis, unattended, WindowState == FormWindowState.Minimized, this);
        FillMyShows(true);
        FillEpGuideHtml();
        RefreshWTW(false, unattended);
    }

    internal void ForceMovieRefresh(IEnumerable<MovieConfiguration>? sis, bool unattended)
    {
        IEnumerable<MovieConfiguration>? movieConfigurations = sis?.ToList();
        mDoc.ForceRefreshMovies(movieConfigurations, unattended, WindowState == FormWindowState.Minimized, this);
        FillMyMovies(movieConfigurations?.FirstOrDefault());
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
        FillActionList();
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
        FillActionList();
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

    private void MyShowTree_AfterSelect(object sender, TreeViewEventArgs e)
    {
        FillEpGuideHtml(e.Node);
        UpdateMyShowsButtonStatus();
    }

    private void MyMoviesTree_AfterSelect(object sender, TreeViewEventArgs e)
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
            SetHtmlBody(chrMovieInformation, ShowHtmlHelper.CreateOldPage(si.GetMovieHtmlOverviewOffline()));
            SetHtmlBody(chrMovieImages, ShowHtmlHelper.CreateOldPage(si.GetMovieImagesHtmlOverview()));
            SetHtmlBody(chrMovieTrailer, ShowHtmlHelper.CreateOldPage("Not available offline"));
            return;
        }
        //SetHtmlBody(chrMovieImages, ShowHtmlHelper.CreateOldPage(si.GetMovieImagesHtmlOverview()));
        SetHtmlBody(chrMovieImages, si.GetMovieImagesOverview());
        SetHtmlBody(chrMovieInformation, si.GetMovieHtmlOverview(false));
        if (si.CachedMovie?.TrailerUrl?.HasValue() ?? false)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            SetHtmlEmbed(chrMovieTrailer, ShowHtmlHelper.YoutubeTrailer(si.CachedMovie));
        }
        else
        {
            SetHtmlBody(chrMovieTrailer, ShowHtmlHelper.CreateOldPage("Not available for this Movie"));
        }

        ResetRunBackGroundWorker(bwMovieHTMLGenerator,si);
    }

    private void MyMoviesTree_MouseClick(object sender, MouseEventArgs e)
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

        MovieConfiguration? si = TreeNodeToMovieItem(n);
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

    private void MyShowTree_MouseClick(object sender, MouseEventArgs e)
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

        ShowConfiguration? si = TreeNodeToShowItem(n);
        ProcessedSeason? seas = TreeNodeToSeason(n);

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
        ProcessedSeason? currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
        if (currentSeas != null)
        {
            return currentSeas;
        }

        ShowConfiguration? currentShow = TreeNodeToShowItem(MyShowTree.SelectedNode);
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
        ProcessedSeason? currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
        ShowConfiguration? currentShow = TreeNodeToShowItem(MyShowTree.SelectedNode);

        int snum = currentSeas?.SeasonNumber ?? 1;

        if (currentShow != null)
        {
            if (currentShow.SeasonEpisodes.TryGetValue(snum, out List<ProcessedEpisode>? returnValue))
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
        MoreBusy();
        mDoc.PreventAutoScan("Filename Templates are open");
        CustomEpisodeName cn = new(TVSettings.Instance.NamingStyle.StyleString);
        CustomNameDesigner cne = new(CurrentlySelectedPel(), cn);
        DialogResult dr = cne.ShowDialog(this);
        if (dr == DialogResult.OK)
        {
            TVSettings.Instance.NamingStyle = cn;
            mDoc.SetDirty();
        }
        mDoc.AllowAutoScan();
        LessBusy();
    }

    private void searchEnginesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        List<ProcessedEpisode>? pel = CurrentlySelectedPel();

        AddEditSearchEngine aese = new(TVDoc.GetSearchers(),
            pel is { Count: > 0 } ? pel[0] : null);

        MoreBusy();
        mDoc.PreventAutoScan("Search Engines are open");

        DialogResult dr = aese.ShowDialog(this);
        if (dr == DialogResult.OK)
        {
            mDoc.SetDirty();
            UpdateSearchButtons();
        }

        mDoc.AllowAutoScan();
        LessBusy();
    }

    private void filenameProcessorsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ShowConfiguration? currentShow = TreeNodeToShowItem(MyShowTree.SelectedNode);
        string theFolder = GetFolderForShow(currentShow);

        if (string.IsNullOrWhiteSpace(theFolder) && TVSettings.Instance.DownloadFolders.Any())
        {
            theFolder = TVSettings.Instance.DownloadFolders.First();
        }

        MoreBusy();
        mDoc.PreventAutoScan("Filename Processors are open");

        AddEditSeasEpFinders d = new(TVSettings.Instance.FNPRegexs, mDoc.TvLibrary.GetSortedShowItems(), currentShow, theFolder);

        DialogResult dr = d.ShowDialog(this);
        if (dr == DialogResult.OK)
        {
            mDoc.SetDirty();
            TVSettings.Instance.FNPRegexs = d.OutputRegularExpressions;
        }

        mDoc.AllowAutoScan();
        LessBusy();
    }

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
        MoreBusy();
        mDoc.PreventAutoScan("Actors Grid is open");
        new ActorsGrid(mDoc).ShowDialog(this);

        mDoc.AllowAutoScan();
        LessBusy();
    }

    private void quickTimer_Tick(object sender, EventArgs e)
    {
        quickTimer.Stop();
        ProcessArgs(mDoc.Args);
    }

    private void UI_KeyDown(object sender, KeyEventArgs e)
    {
        if (!e.Control)
        {
            return;
        }

        switch (e.KeyCode)
        {
            case Keys.F:
                UiScan(null, null, false, TVSettings.ScanType.Full, MediaConfiguration.MediaType.both);
                e.Handled = true;
                break;
            case Keys.Q:
                UiScan(null, null, false, TVSettings.ScanType.Quick, MediaConfiguration.MediaType.both);
                e.Handled = true;
                break;
            case Keys.R:
                UiScan(null, null, false, TVSettings.ScanType.Recent, MediaConfiguration.MediaType.both);
                e.Handled = true;
                break;
            case Keys.D:
                ActionAction(true, false, false);
                e.Handled = true;
                break;
        }

        MoveToTab(e);
    }

    private void MoveToTab(KeyEventArgs e)
    {
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
        if (bwScan.IsBusy)
        {
            Logger.Warn("Can't start scan as it's already running");
            return;
        }
        MoreBusy(); // cancelled in bwScan_RunWorkerCompleted

        CancellationTokenSource cts = new();
        bool hidden = WindowState == FormWindowState.Minimized;

        TVDoc.ScanSettings initialSettings = new(shows ?? new List<ShowConfiguration>(),
            movies ?? new List<MovieConfiguration>(), unattended, hidden, st, media, this, null, cts.Token);
        mDoc.SetScanSettings(initialSettings);
        SetupScanUi(hidden);

        TVDoc.ScanSettings scanSettings = new(shows ?? new List<ShowConfiguration>(),
            movies ?? new List<MovieConfiguration>(), unattended, hidden, st, media, this, scanProgDlg, cts.Token);
        mDoc.SetScanSettings(scanSettings);

        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal,Handle);
        bwScan.RunWorkerAsync(scanSettings);
        ShowDialogAndWait(cts);
    }

    private void ShowDialogAndWait(CancellationTokenSource cts)
    {
        if (scanProgDlg == null)
        {
            return;
        }

        ShowChildDialog(scanProgDlg);

        // ReSharper disable once PossibleNullReferenceException
        if (scanProgDlg.DialogResult == DialogResult.Cancel)
        {
            cts.Cancel();
        }
    }

    private bool lastScanUnattended;
    private void bwScan_DoWork(object sender, DoWorkEventArgs e)
    {
        Thread.CurrentThread.Name ??= "Main Scan Thread"; // Can only set it once
        TVDoc.ScanSettings scanSettings = e.Argument as TVDoc.ScanSettings ?? throw new Exception();
        mDoc.Scan(scanSettings);
        lastScanUnattended = scanSettings.Unattended;
    }

    private void bwScan_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
    }

    private void bwScan_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (!IsDisposed)
        {
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
        }
        AskUserAboutShowProblems(lastScanUnattended);
        LessBusy(); //Note this is set in UiScan()
        scanProgDlg?.Close();
        if (!IsDisposed)
        {
            FillMyShows(true); // scanning can download more info to be displayed in my shows
            FillMyMovies();
            FillActionList();
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress, Handle);
        }
        offlineOperationToolStripMenuItem.Checked = TVSettings.Instance.OfflineMode;
    }

    private void SetupScanUi(bool hidden)
    {
        if (!mDoc.Args.Hide && Environment.UserInteractive)
        {
            scanProgDlg = new ScanProgress(this,
                TVSettings.Instance.DoBulkAddInScan,
                TVSettings.Instance.RenameCheck || TVSettings.Instance.MissingCheck,
                TVSettings.Instance.RemoveDownloadDirectoriesFiles || TVSettings.Instance.RemoveDownloadDirectoriesFilesMatchMovies || TVSettings.Instance.ReplaceWithBetterQuality || TVSettings.Instance.ReplaceMoviesWithBetterQuality,
                mDoc.HasActiveLocalFinders,
                mDoc.HasActiveDownloadFinders,
                mDoc.HasActiveSearchFinders
            );

            if (hidden)
            {
                scanProgDlg.WindowState = FormWindowState.Minimized;
            }
        }
        else
        {
            scanProgDlg = null;
        }
    }

    private void AskUserAboutShowProblems(bool unattended)
    {
        if (unattended)
        {
            return;
        }

        if (mDoc.ShowProblems.Any())
        {
            string message = mDoc.ShowProblems.Count() > 1
                ? $"Shows with Id {string.Join(",", mDoc.ShowProblems.Select(exception => exception.Media.ToString()))} are not found on TVDB, TMDB and TVMaze. Please update them"
                : $"Show with {StringFor(mDoc.ShowProblems.First().ShowIdProvider)} Id {mDoc.ShowProblems.First().Media.IdFor(mDoc.ShowProblems.First().Media.Provider)} is not found on {StringFor(mDoc.ShowProblems.First().ErrorProvider)}. Please Update";

            DialogResult result = MessageBox.Show(message, "Series/Show No Longer Found", MessageBoxButtons.OKCancel,
                MessageBoxIcon.Error);

            if (result == DialogResult.Cancel)
            {
                return;
            }

            foreach (MediaNotFoundException problem in mDoc.ShowProblems)
            {
                if (mDoc.ShowProblems.Count() > 1)
                {
                    MessageBox.Show(problem.Message, "Issue With Series Setup", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                {
                    ShowConfiguration? problemShow = mDoc.TvLibrary.GetShowItem(problem.Media);
                    if (problemShow != null)
                    {
                        EditShow(problemShow);
                    }
                }
            }
        }

        if (mDoc.MovieProblems.Any())
        {
            string message = mDoc.MovieProblems.Count() > 1
                ? $"Movies with Id {string.Join(",", mDoc.MovieProblems.Select(exception => exception.Media.ToString()))} are not found on TVDB, TMDB and TVMaze. Please update them"
                : $"Movie with {StringFor(mDoc.MovieProblems.First().ShowIdProvider)} Id {mDoc.MovieProblems.First().Media} is not found on {StringFor(mDoc.MovieProblems.First().ErrorProvider)}. Please Update";

            DialogResult result = MessageBox.Show(message, "Movie No Longer Found", MessageBoxButtons.OKCancel,
                MessageBoxIcon.Error);

            if (result == DialogResult.Cancel)
            {
                return;
            }

            foreach (MediaNotFoundException problem in mDoc.MovieProblems)
            {
                if (mDoc.MovieProblems.Count() > 1)
                {
                    MessageBox.Show(problem.Message, "Issue With Movie Setup", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                MovieConfiguration? problemMovie = mDoc.FilmLibrary.GetMovie(problem.Media);
                if (problemMovie != null)
                {
                    EditMovie(problemMovie);
                }
            }
        }

        mDoc.ClearCacheUpdateProblems();
    }

    private static string StringFor(TVDoc.ProviderType i)
    {
        return i switch
        {
            TVDoc.ProviderType.TVmaze => "TV Maze",
            TVDoc.ProviderType.TMDB => "TMDB",
            TVDoc.ProviderType.TheTVDB => "The TVDB",
            TVDoc.ProviderType.libraryDefault => throw new ArgumentOutOfRangeException(nameof(i), i, null),
            _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
        };
    }

    private static object ActionImageGetter(object rowObject)
    {
        Item s = (Item)rowObject;
        return s.IconNumber;
    }

    public void FillActionList()
    {
        if (olvAction.IsDisposed)
        {
            return;
        }

        internalCheckChange = true;

        byte[] oldState = olvAction.SaveState();
        olvAction.BeginUpdate();
        Logger.Info("UI: Updating Actions: Adding Data");
        olvAction.SetObjects(mDoc.TheActionList,true);
        Logger.Info("UI: Updating Actions: Rebuilding Columns");
        olvAction.RebuildColumns();
        Logger.Info("UI: Updating Actions: Restoring State");
        olvAction.RestoreState(oldState);
        olvAction.EndUpdate();
        Logger.Info("UI: Updating Actions: Updating CheckBoxes after action list update");

        internalCheckChange = false;

        UpdateActionCheckboxes();
    }

    private static string HeaderName(string name, int number) => $"{name} ({PrettyPrint(number)})";

    private static string PrettyPrint(int number) => number + " " + number.ItemItems();

    private static string HeaderName(string name, int number, long filesize) => $"{name} ({PrettyPrint(number)}, {filesize.GBMB(1)})";

    private void bnActionAction_Click(object sender, EventArgs e) => ActionAction(true, false, false);

    private void ActionAction(bool checkedNotSelected, bool unattended, bool doAll)
    {
        CancellationTokenSource actionCancellationToken = new();
        if (bwAction.IsBusy)
        {
            Logger.Warn("Can't do actions as they are already processing");
            return;
        }

        TVDoc.ActionSettings sett = new(unattended: unattended, doAll: doAll,
            lvr: checkedNotSelected ? GetCheckedItems() : GetSelectedItems(), token: actionCancellationToken);

        bool showUi = WindowState !=FormWindowState.Minimized && !mDoc.Args.Hide && Visible && Environment.UserInteractive;
        // If not /hide, show CopyMoveProgress dialog
        if (showUi)
        {
            CopyMoveProgress cmp = new(mDoc,sett, () => actionCancellationToken.Cancel());
            ShowChild(cmp);
        }

        bwAction.RunWorkerAsync(sett);
    }

    private bool lastActionUnattended;
    private void bwAction_DoWork(object sender, DoWorkEventArgs e)
    {
        Thread.CurrentThread.Name ??= "Main Action Thread"; // Can only set it once

        TVDoc.ActionSettings set = e.Argument as TVDoc.ActionSettings ?? throw new Exception();
        mDoc.DoActions(set);
        lastActionUnattended = set.Unattended;
    }

    private void bwAction_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        FillActionList();
        RefreshWTW(false, lastActionUnattended);
    }

    private void Revert()
    {
        foreach (Item item in GetSelectedItems())
        {
            mDoc.RevertAction(item);
        }

        FillActionList();
        RefreshWTW(false, false);
    }
    private void RevertSeasons()
    {
        foreach (Item item in GetSelectedItems())
        {
            mDoc.RevertSeasonAction(item);
        }

        FillActionList();
    }
    private void folderMonitorToolStripMenuItem_Click(object sender, EventArgs e)
    {
        MoreBusy();
        mDoc.PreventAutoScan("Bulk add shows is open");

        BulkAddSeriesManager bam = new(mDoc);
        BulkAddShow fm = new(mDoc, bam,this);
        fm.ShowDialog(this);
        FillMyShows(true);

        mDoc.AllowAutoScan();
        LessBusy();
    }

    private void lvAction_MouseClick(object sender, MouseEventArgs e)
    {
        Point pt = ((ListView)sender).PointToScreen(new Point(e.X, e.Y));

        ItemList lvr = GetSelectedItems();

        Item? action = olvAction.FocusedObject as Item;

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
            GenerateActionRightClickMenu(pt, lvr, null, null, action.Movie);
        }
        else
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }
            GenerateActionRightClickMenu(pt, lvr, null, null, null);
        }
    }

    private void GenerateActionRightClickMenu(Point pt, ItemList lvr, ShowConfiguration? si, ProcessedEpisode? episode, MovieConfiguration? movie)
    {
        if (lvr.Count == 0)
        {
            return; // nothing selected
        }

        showRightClickMenu.Items.Clear();

        ProcessedSeason? seas = episode?.AppropriateProcessedSeason;

        // Action related items
        if (lvr.Count > lvr.Missing.ToList().Count) // not just missing selected
        {
            AddRcMenuItem("Action Selected", (_, _) => ActionAction(false, false, false));
        }
        if (lvr.CopyMove.Any() || lvr.DownloadTorrents.Any() || lvr.Downloading.Any())
        {
            showRightClickMenu.Items.Add(new ToolStripSeparator());
            AddRcMenuItem("Revert to Missing", (_, _) => Revert());
        }

        if (lvr.MissingSeasons.HasAny())
        {
            AddRcMenuItem("Revert to Missing Episodes", (_, _) => RevertSeasons());
        }

        if (lvr.Count == lvr.MissingEpisodes.ToList().Count + lvr.MissingMovies.ToList().Count) // only missing items selected?
        {
            if (lvr.Count == 1) // only one selected
            {
                AddRcMenuItem("Browse For...", (_, _) => BrowseForMissingItem((ItemMissing)lvr[0]));
            }
            if (episode != null)
            {
                showRightClickMenu.Items.Add(new ToolStripSeparator());
                MenuSearchFor(episode);
            }
            if (movie != null)
            {
                showRightClickMenu.Items.Add(new ToolStripSeparator());
                MenuSearchFor(movie);
            }
        }

        if (si is not null)
        {
            showRightClickMenu.Items.Add(new ToolStripSeparator());
            MenuGuideAndTvdb(episode, si, seas);
        }

        MenuFolders(lvr, si, episode?.AppropriateProcessedSeason, episode);

        showRightClickMenu.Items.Add(new ToolStripSeparator());
        AddRcMenuItem("Ignore Selected", (_, _) => IgnoreSelected());
        if (episode != null)
        {
            AddRcMenuItem("Ignore Entire Season", (_, _) => IgnoreSelectedSeasons(lvr));
        }
        AddRcMenuItem("Remove Selected", (_, _) => ActionDeleteSelected());

        showRightClickMenu.Show(pt);
    }

    private void MenuSearchFor(ProcessedEpisode ep)
    {
        ToolStripMenuItem tsi = new("Search");
        tsi.Click += (_, _) => { TVDoc.SearchForEpisode(ep); };

        foreach (SearchEngine se in TVDoc.GetSearchers())
        {
            if (se.Name.HasValue())
            {
                ToolStripMenuItem tssi = new(se.Name.ToUiVersion());
                tssi.Click += (_, _) => { TVDoc.SearchForEpisode(se, ep); };
                tsi.DropDownItems.Add(tssi);
            }
        }

        if (TVSettings.Instance.SearchJackett || TVSettings.Instance.SearchJackettButton)
        {
            if (TVDoc.GetSearchers().Any())
            {
                tsi.DropDownItems.Add(new ToolStripSeparator());
            }
            ToolStripMenuItem tssi = new("Jackett Search");
            tssi.Click += (_, _) => { JackettFinder.SearchForEpisode(ep); };
            tsi.DropDownItems.Add(tssi);
        }

        showRightClickMenu.Items.Add(tsi);
    }

    private void MenuSearchFor(MovieConfiguration ep)
    {
        ToolStripMenuItem tsi = new("Search");
        tsi.Click += (_, _) => { TVDoc.SearchForMovie(ep); };

        foreach (SearchEngine se in TVDoc.GetMovieSearchers())
        {
            if (se.Name.HasValue())
            {
                ToolStripMenuItem tssi = new(se.Name);
                tssi.Click += (_, _) => { TVDoc.SearchForMovie(se, ep); };
                tsi.DropDownItems.Add(tssi);
            }
        }

        if (TVSettings.Instance.SearchJackett || TVSettings.Instance.SearchJackettButton)
        {
            if (TVDoc.GetMovieSearchers().Any())
            {
                tsi.DropDownItems.Add(new ToolStripSeparator());
            }
            ToolStripMenuItem tssi = new("Jackett Search");
            tssi.Click += (_, _) => { JackettFinder.SearchForMovie(ep); };
            tsi.DropDownItems.Add(tssi);
        }

        showRightClickMenu.Items.Add(tsi);
    }

    private void AddRcMenuItem(string name, EventHandler command)
    {
        ToolStripMenuItem tsi = new(name.ToUiVersion());
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

        bool anyMissing = lvr.Missing.Any();
        btnActionBTSearch.Enabled = anyMissing;
        tbActionJackettSearch.Enabled = anyMissing;

        showRightClickMenu.Items.Clear();
    }

    private void ActionDeleteSelected()
    {
        mDoc.TheActionList.Remove(GetSelectedItems());
        FillActionList(); //TODO - optimise this so we just remove the selected items
    }

    private void lvAction_KeyDown(object sender, KeyEventArgs e)
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
        List<Item> chk = mDoc.TheActionList.Checked;

        SetCheckbox(mcbRename, all.Where(IsRenameAction), chk.Where(IsRenameAction));
        SetCheckbox(mcbCopyMove, all.Where(IsCopyMoveAction), chk.Where(IsCopyMoveAction));
        SetCheckbox(mcbDeleteFiles, all.OfType<ActionDelete>(), chk.OfType<ActionDelete>());
        SetCheckbox(mcbSaveImages, all.OfType<ActionDownloadImage>(), chk.OfType<ActionDownloadImage>());
        SetCheckbox(mcbWriteMetadata, all.OfType<ActionWriteMetadata>(), chk.OfType<ActionWriteMetadata>());
        SetCheckbox(mcbModifyMetadata, all.OfType<ActionFileMetaData>(), chk.OfType<ActionFileMetaData>());
        SetCheckbox(mcbDownload, all.TorrentActions, chk.Where(item => item is ActionTRemove || item is ActionTDownload));

        SetCheckbox(mcbAll, all.Actions, chk.OfType<Action>());
    }

    private static bool IsCopyMoveAction(Item i) =>
        (i is ActionCopyMoveRename a && a.Operation != ActionCopyMoveRename.Op.rename)
        || i is ActionMoveRenameDirectory;

    private static bool IsRenameAction(Item a) => a is ActionCopyMoveRename { Operation: ActionCopyMoveRename.Op.rename };

    private static void SetCheckbox(ToolStripMenuItem box, IEnumerable<Item> all, IEnumerable<Item> chk)
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
            IgnoreItem? ii = action.Ignore;
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
            FillActionList(); //TODO - Optimise this so that we just update selected
        }
    }

    private void ignoreListToolStripMenuItem_Click(object sender, EventArgs e)
    {
        IgnoreEdit ie = new(mDoc, string.Empty);
        ie.ShowDialog(this);
    }

    private void showSummaryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ShowSummary f = new(mDoc, this);
        f.Show(this);
    }

    private void lvAction_ItemChecked(object sender, ItemCheckedEventArgs e) => UpdateActionCheckboxes();

    private void btnFilter_Click(object sender, EventArgs e)
    {
        Filters filters = new(mDoc);
        DialogResult res = filters.ShowDialog(this);
        if (res == DialogResult.OK)
        {
            FillMyShows(true);
        }
    }

    private void filterTextBox_TextChanged(object sender, EventArgs e) => FillMyShows(true);

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
            NativeMethods.SendMessage(tb.Handle, 0xd3, (IntPtr)2, (IntPtr)(filterButton.Width << 16));
        }
    }

    private void visitSupportForumToolStripMenuItem_Click(object sender, EventArgs e)
        => Helpers.OpenUrl("https://groups.google.com/forum/#!forum/tvrename");

    public void Quit() => Close();

    private async void checkForNewVersionToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Dispatcher uiDisp = Dispatcher.CurrentDispatcher;

        Task<ServerRelease?> tuv = VersionUpdater.CheckForUpdatesAsync();
        ServerRelease? result = await tuv.ConfigureAwait(false);

        uiDisp.Invoke(() => NotifyUpdates(result, true,false));
    }

    private void NotifyUpdates(ServerRelease? update, bool forceShowNoUpdateRequiredDialog, bool inSilentMode)
    {
        btnUpdateAvailable.Visible = update is not null;

        if (update is null)
        {
            if (forceShowNoUpdateRequiredDialog || (!inSilentMode && Environment.UserInteractive))
            {
                MessageBox.Show(@"There is no update available please try again later.", @"No update available",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return;
        }

        Logger.Warn(update.LogMessage());

        if (inSilentMode || Helpers.InDebug() || !Environment.UserInteractive)
        {
            return;
        }

        if (TVSettings.Instance.SuppressUpdateAvailablePopup && !forceShowNoUpdateRequiredDialog)
        {
            return;
        }

        UpdateNotification unForm = new(update);
        unForm.ShowDialog(this);
        if (unForm.DialogResult != DialogResult.Abort)
        {
            return;
        }

        //User has decided to download and quit
        Logger.Info("Downloading New Release and Quiting");
        //We need to quit!
        Close();
    }

    private void duplicateFinderLOGToolStripMenuItem_Click(object sender, EventArgs e)
    {
        MergedEpisodeFinder form = new(mDoc, this);
        form.ShowDialog(this);
    }

    private async void btnUpdateAvailable_Click(object sender, EventArgs e)
    {
        btnUpdateAvailable.Visible = false;

        Dispatcher uiDisp = Dispatcher.CurrentDispatcher;

        Task<ServerRelease?> tuv = VersionUpdater.CheckForUpdatesAsync();
        ServerRelease? result = await tuv.ConfigureAwait(false);

        uiDisp.Invoke(() => NotifyUpdates(result, true,false));
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
        logToolStripMenuItem_Click(sender, e);

        TaskHelper.Run(() =>
        {
            TimeZoneTracker results = new();
            foreach (ShowConfiguration si in mDoc.TvLibrary.GetSortedShowItems())
            {
                CachedSeriesInfo? ser = si.CachedShow;
                if (ser != null)
                {
                    results.Add(ser.Networks.FirstOrDefault() ?? string.Empty, si.ShowTimeZone, si.ShowName);
                }
            }
            Logger.Info(results.PrintVersion());
        }, "Timezone Check");
    }

    private class TimeZoneTracker : Dictionary<string, Dictionary<string, List<string>>>
    {
        internal void Add(string network, string timezone, string show)
        {
            if (!ContainsKey(network))
            {
                Add(network, new Dictionary<string, List<string>>());
            }

            Dictionary<string, List<string>> snet = this[network];

            if (!snet.ContainsKey(timezone))
            {
                snet.Add(timezone, new List<string>());
            }

            List<string> snettz = snet[timezone];

            snettz.Add(show);
        }

        internal string PrintVersion()
        {
            StringBuilder sb = new();
            sb.AppendLine("***********************************");
            sb.AppendLine("****Timezone Comparison       *****");
            sb.AppendLine("***********************************");
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> kvp in this)
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
        LogViewer form = new();
        form.Show();
    }

    private void episodeFileQualitySummaryLogToolStripMenuItem_Click(object sender, EventArgs e)
    {
        //Show Log Pane
        logToolStripMenuItem_Click(sender, e);

        TaskHelper.Run(() =>
        {
            Beta.LogShowEpisodeSizes(mDoc);
        }, "Episode File Quality Check");
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        LicenceInfoForm form = new();
        form.ShowDialog(this);
    }

    private void QuickRenameToolStripMenuItem_Click(object sender, EventArgs e)
    {
        QuickRename form = new(mDoc, this);
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

        UITVDBAccuracyCheck(false);

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
        btnScan.Text = st.PrettyPrint() + " Scan";
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

    private void BTSearch_DropDownOpening(object sender, EventArgs e)
    {
        ChooseSiteMenu((ToolStripSplitButton)sender);
    }

    private void BtnSearch_ButtonClick(object? sender, EventArgs? e)
    {
        UiScan(null, null, false, TVSettings.Instance.UIScanType, MediaConfiguration.MediaType.both);
    }

    private void UpdateCheckboxGroup(ToolStripMenuItem menuItem, Func<Item, bool> isValid)
    {
        menuItem.CheckState = menuItem.CheckState switch
        {
            CheckState.Unchecked => CheckState.Checked,
            CheckState.Checked => CheckState.Unchecked,
            CheckState.Indeterminate => CheckState.Unchecked,
            _ => throw new ArgumentOutOfRangeException()
        };

        bool checkedStatus = menuItem.CheckState == CheckState.Checked;

        internalCheckChange = true;

        foreach(Item x in mDoc.TheActionList.Where(isValid))
        {
            x.CheckedItem = checkedStatus;
        }

        internalCheckChange = false;
        UpdateActionCheckboxes();
    }

    private void McbAll_Click(object sender, EventArgs e)
    {
        UpdateCheckboxGroup(mcbAll, item => item is Action);
    }

    private void McbRename_Click(object sender, EventArgs e)
    {
        UpdateCheckboxGroup(mcbRename, IsRenameAction);
    }

    private void McbCopyMove_Click(object sender, EventArgs e)
    {
        UpdateCheckboxGroup(mcbCopyMove, IsCopyMoveAction);
    }

    private void McbDeleteFiles_Click(object sender, EventArgs e)
    {
        UpdateCheckboxGroup(mcbDeleteFiles, i => i is ActionDelete);
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
        ThanksForm form = new();
        form.ShowDialog(this);
    }

    private void TabControl1_DrawItem(object sender, DrawItemEventArgs e)
    {
        //Follow this advice https://docs.microsoft.com/en-us/dotnet/framework/winforms/controls/how-to-display-side-aligned-tabs-with-tabcontrol

        Graphics g = e.Graphics;

        TabControl? tabCtrl = sender as TabControl;

        g.FillRectangle(e.State == DrawItemState.Selected ? Brushes.White : new SolidBrush(BackColor), e.Bounds);

        // Get the item from the collection.
        TabPage? tabPage = tabCtrl?.TabPages[e.Index];

        if (tabPage is null)
        {
            return;
        }

        // Get the real bounds for the tab rectangle.
        Rectangle? tabBounds = tabCtrl?.GetTabRect(e.Index);

        // Draw string. Center the text.
        StringFormat stringFlags = new()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        const int INDENT = 15;

        //GetIcon
        if (tabCtrl?.ImageList?.Images[tabPage.ImageKey] is Bitmap bit && tabBounds != null)
        {
            ScaleBitmapLogicalToDevice(ref bit);

            float xIndent = (tabBounds.Value.Width - bit.Width) / 2.0f;
            float x = tabBounds.Value.X + xIndent;
            float y = tabBounds.Value.Y + INDENT;
            g.DrawImage(bit, x, y);

            Rectangle textarea = new(tabBounds.Value.X, tabBounds.Value.Y + INDENT + bit.Height, tabBounds.Value.Width, tabBounds.Value.Height - (INDENT + bit.Height));
            g.DrawString(tabPage.Text, tabPage.Font, new SolidBrush(ForeColor), textarea, stringFlags);
        }
    }

    private void BwSeasonHTMLGenerator_DoWork(object sender, DoWorkEventArgs e)
    {
        Thread.CurrentThread.Name ??= "Season HTML Creation Thread"; // Can only set it once
        ProcessedSeason? s = e.Argument as ProcessedSeason;
        ShowConfiguration? si = s?.Show;

        string html = string.Empty;
        try
        {
            if (s is not null)
            {
                html = si?.GetSeasonHtmlOverview(s, true) ?? string.Empty;
            }
        }
        catch (Exception exception)
        {
            Logger.Error(exception, $"Error Occurred Creating Show Summary for {si?.ShowName} with order {si?.Order}");
        }
        e.Result = new HtmlUpdateSettings(s, html, chrInformation);
    }

    private void UpdateWeb(object sender, RunWorkerCompletedEventArgs e)
    {
        if (e.Result is HtmlUpdateSettings result)
        {
            if (UiHasContextFor(result.Argument))
            {
                SetHtmlBody(result.Web, result.Html);
            }
        }
    }

    private bool UiHasContextFor(object? context)
    {
        return context switch
        {
            //Check to see whether context is still valid
            MovieConfiguration when tabControl1.SelectedTab != tbMyMovies => false,
            MovieConfiguration mc => TreeNodeToMovieItem(movieTree.SelectedNode) == mc,
            ShowConfiguration when tabControl1.SelectedTab != tbMyShows => false,
            ShowConfiguration sc => TreeNodeToShowItem(MyShowTree.SelectedNode) == sc,
            ProcessedSeason when tabControl1.SelectedTab != tbMyShows => false,
            ProcessedSeason season => TreeNodeToSeason(MyShowTree.SelectedNode) == season,
            _ => false
        };
    }

    private void BwShowHTMLGenerator_DoWork(object sender, DoWorkEventArgs e)
    {
        Thread.CurrentThread.Name ??= "Show HTML Creation Thread"; // Can only set it once
        ShowConfiguration? si = e.Argument as ShowConfiguration;

        string html = string.Empty;
        try
        {
            html = si?.GetShowHtmlOverview(true) ?? string.Empty;
        }
        catch (Exception exception)
        {
            Logger.Error(exception, $"Error Occurred Creating Show Summary for {si?.ShowName} with order {si?.Order}");
        }
        e.Result = new HtmlUpdateSettings(si, html, chrInformation);
    }

    public class HtmlUpdateSettings
    {
        public readonly object? Argument;
        public readonly string Html;
        public readonly ChromiumWebBrowser Web;

        public HtmlUpdateSettings(object? argument, string html, ChromiumWebBrowser web)
        {
            Argument = argument;
            Html = html;
            Web = web;
        }
    }

    private void BwUpdateSchedule_DoWork(object sender, DoWorkEventArgs e)
    {
        Thread.CurrentThread.Name ??= "Update Schedule Thread"; // Can only set it once
        e.Result = GenerateNewScheduleItems();
    }

    private void BwUpdateSchedule_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (e.Result is not List<ListViewItem> newContents)
        {
            return;
        }

        mInternalChange++;
        lvWhenToWatch.BeginUpdate();

        int dd = TVSettings.Instance.WTWRecentDays;

        lvWhenToWatch.Groups["justPassed"].Header =
            "Aired in the last " + dd + " day" + (dd == 1 ? "" : "s");

        // try to maintain selections if we can
        List<ProcessedEpisode> selections = new();
        foreach (ListViewItem lvi in lvWhenToWatch.SelectedItems)
        {
            selections.Add((ProcessedEpisode)lvi.Tag);
        }

        ProcessedSeason? currentSeas = TreeNodeToSeason(MyShowTree.SelectedNode);
        ShowConfiguration? currentShowConfiguration = TreeNodeToShowItem(MyShowTree.SelectedNode);

        lvWhenToWatch.Items.Clear();

        List<DateTime> bolded = new();

        foreach (ListViewItem lvi in newContents)
        {
            if (lvi.Tag is not ProcessedEpisode ei)
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
        if (TVSettings.Instance.ShowAccessibilityOptions)
        {
            targetWidth += 200;
        }

        if (TVSettings.Instance.SearchJackettButton)
        {
            targetWidth += 200;
        }

        bool isWide = Width > targetWidth;
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
        olvAction.ShowGroups = true;
        olvAction.AlwaysGroupByColumn = null;
        olvAction.CustomSorter = delegate { olvAction.ListViewItemSorter = new ListViewActionItemSorter(); };
        olvAction.Sort(olvType, SortOrder.Ascending);
        olvAction.BuildGroups(olvType, SortOrder.Ascending,olvShowColumn,SortOrder.Ascending,olvSeason,SortOrder.Ascending);
        //olvAction.Sort();
        //olvAction.BuildGroups(olvType,SortOrder.Ascending);//(olvType, SortOrder.Ascending,olvShowColumn,SortOrder.Ascending,olvSeason,SortOrder.Ascending);
        olvAction.ResetColumnFiltering();
        olvAction.EndUpdate();
    }

    private void OlvAction_Dropped(object sender, OlvDropEventArgs e)
    {
        // Get a list of filenames being dragged
        string[] files = (string[])((DataObject)e.DataObject).GetData(DataFormats.FileDrop, false);

        // Establish item in list being dragged to, and exit if no item matched
        // Check at least one file was being dragged, and that dragged-to item is a "Missing Item" item.
        if (files.Length <= 0 || e.DropTargetItem.RowObject is not ItemMissing mi)
        {
            return;
        }

        // Only want the first file if multiple files were dragged across.
        ManuallyAddFileForItem(mi, files[0]);
    }

    private void OlvAction_CanDrop(object sender, OlvDropEventArgs e)
    {
        if (e.DropSink?.DropTargetItem?.RowObject is not Item item)
        {
            e.Effect = DragDropEffects.None;
        }
        else
        {
            if (item is ItemMissing)
            {
                if (((DataObject)e.DataObject).GetDataPresent(DataFormats.FileDrop))
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

    private void BwShowSummaryHTMLGenerator_DoWork(object sender, DoWorkEventArgs e)
    {
        Thread.CurrentThread.Name ??= "Show Summary HTML Creation Thread"; // Can only set it once
        ShowConfiguration? si = e.Argument as ShowConfiguration;
        string html = string.Empty;
        try
        {
            html = si?.GetShowSummaryHtmlOverview(true) ?? string.Empty;
        }
        catch (Exception exception)
        {
            Logger.Error(exception, $"Error Occurred Creating Show Summary for {si?.ShowName} with order {si?.Order}");
        }
        e.Result = new HtmlUpdateSettings(si, html, chrSummary);
    }

    private void BwSeasonSummaryHTMLGenerator_DoWork(object sender, DoWorkEventArgs e)
    {
        Thread.CurrentThread.Name ??= "Season Summary Creation Thread"; // Can only set it once
        ProcessedSeason? s = e.Argument as ProcessedSeason;
        ShowConfiguration? si = s?.Show;
        string html = string.Empty;
        try
        {
            if (s is not null)
            {
                html = si?.GetSeasonSummaryHtmlOverview(s, true) ?? string.Empty;
            }
        }
        catch (Exception exception)
        {
            Logger.Error(exception, $"Error Occurred Creating Show Summary for {si?.ShowName} with order {si?.Order}");
        }
        e.Result = new HtmlUpdateSettings(s,html,chrSummary);
    }

    private void ToolStripButton1_Click(object sender, EventArgs e)
    {
        if (lvWhenToWatch.SelectedItems.Count == 0)
        {
            return;
        }

        ToolStripButton button = (ToolStripButton)sender;

        Point pt = button.Owner.PointToScreen(button.Bounds.Location);
        List<ProcessedEpisode> eis = lvWhenToWatch
            .SelectedItems
            .Cast<ListViewItem>()
            .Select(lvi => lvi.Tag as ProcessedEpisode)
            .OfType<ProcessedEpisode>()
            .ToList();

        WtwRightClickOnShow(eis, pt);
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

        ShowConfiguration? si = TreeNodeToShowItem(n);
        ProcessedSeason? seas = TreeNodeToSeason(n);

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

        Item? action = olvAction.FocusedObject as Item;

        if (action?.Episode != null && lvr.Count == 1)
        {
            GenerateActionRightClickMenu(pt, lvr, action.Episode.Show, action.Episode, null);
        }
        else if (action?.Movie != null && lvr.Count == 1)
        {
            GenerateActionRightClickMenu(pt, lvr, null, null, action.Movie);
        }
        else
        {
            GenerateActionRightClickMenu(pt, lvr, null, null, null);
        }
    }

    private void ToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        OrphanFiles ui = new(mDoc, this);
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
                else if (i is ShowSeasonMissing { Series: { }, SeasonNumberAsInt: { } } ssm)
                {
                    JackettFinder.SearchForSeason(ssm.Series, ssm.SeasonNumberAsInt.Value);
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
                JackettFinder.SearchForEpisode((ProcessedEpisode)lvi.Tag);
            }
        }
    }

    private void olvAction_BeforeCreatingGroups(object sender, CreateGroupsEventArgs e)
    {
        e.Parameters.ItemComparer = GetActionComparer(e.Parameters.GroupByColumn);

        if (e.Parameters.PrimarySort == olvDate)
        {
            e.Parameters.PrimarySort = new OLVColumn("RawDate", "AirDate");
        }

        if (e.Parameters.PrimarySort == olvEpisode)
        {
            e.Parameters.PrimarySort = new OLVColumn("RawEp", "EpisodeNumber");
            e.Parameters.GroupComparer = new SeasonGroupComparer(e.Parameters.GroupByOrder);
        }

        if (e.Parameters.PrimarySort == olvSeason || e.Parameters.PrimarySort.AspectName == "SeasonNumberAsInt")
        {
            e.Parameters.PrimarySort = new OLVColumn("RawSe", "SeasonNumberAsInt");
            e.Parameters.GroupComparer = new SeasonGroupComparer(e.Parameters.GroupByOrder);
        }

        e.Parameters.SecondarySort = new OLVColumn("key", "OrderKey");
        e.Parameters.SecondarySortOrder = SortOrder.Ascending;
    }

    private static IComparer<OLVListItem> GetActionComparer(OLVColumn column)
    {
        return column.Index switch
        {
            1 => new NumberAsTextActionComparer(column.Index),
            2 => new NumberAsTextActionComparer(column.Index),
            3 => new DateActionComparer(column.Index),
            _ => new TextActionComparer(column.Index)
        };
    }

    public void ShowFgDownloadProgress(CacheUpdater cu, CancellationTokenSource cts)
    {
        if (!IsDisposed)
        {
            Invoke((MethodInvoker)delegate
            {
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal, Handle);
                new DownloadProgress(cu, cts).Show(this);
            });
        }
    }

    private void toolStripButton2_Click(object sender, EventArgs e)
    {
        MovieConfiguration? si2 = TreeNodeToMovieItem(movieTree?.SelectedNode);
        if (si2 != null)
        {
            EditMovie(si2);
        }
    }

    private void btnMovieDelete_Click(object sender, EventArgs e)
    {
        TreeNode n = movieTree.SelectedNode;
        MovieConfiguration? si = TreeNodeToMovieItem(n);
        if (si is null)
        {
            return;
        }

        DeleteMovie(si);
    }

    private void bwMovieHTMLGenerator_DoWork(object sender, DoWorkEventArgs e)
    {
        MovieConfiguration? si = e.Argument as MovieConfiguration;
        Thread.CurrentThread.Name ??= $"Movie '{si?.Name}' HTML Creation Thread"; // Can only set it once

        string html = string.Empty;
        try
        {
            html = si?.GetMovieHtmlOverview(true) ?? string.Empty;
        }
        catch (Exception exception)
        {
            Logger.Error(exception, $"Error Occurred Creating Movie Summary for {si?.ShowName}");
        }
        e.Result = new HtmlUpdateSettings(si,html,chrMovieInformation);
    }

    private void movieCollectionSummaryLogToolStripMenuItem_Click(object sender, EventArgs e)
    {
        MoreBusy();
        mDoc.PreventAutoScan("Movie Collections are open");

        CollectionsView form = new(mDoc, this);
        form.ShowDialog(this);
        FillMyMovies();

        mDoc.AllowAutoScan();
        LessBusy();
    }

    private void bulkAddMoviesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        MoreBusy();
        mDoc.PreventAutoScan("Bulk Add Shows");
        BulkAddMovieManager bam = new(mDoc);
        BulkAddMovie fm = new(mDoc, bam,this);
        fm.ShowDialog(this);
        FillMyMovies();
        mDoc.AllowAutoScan();
        LessBusy();
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
        TaskHelper.Run(
            () => mDoc.TMDBServerAccuracyCheck(unattended, WindowState == FormWindowState.Minimized, this),
            "TMDB Accuracy Check"
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

        MovieConfiguration? si = TreeNodeToMovieItem(n);
        if (si != null)
        {
            RightClickOnMyMovies(si, pt);
        }
    }

    private void btnMovieFilter_Click(object sender, EventArgs e)
    {
        MovieFilters filters = new(mDoc);
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
            MovieConfiguration? si = TreeNodeToMovieItem(n);
            if (si is not null)
            {
                ForceMovieRefresh(si, false);
            }
        }
        else
        {
            ForceMovieRefresh(false);
        }
    }

    private void ForceMovieRefresh(bool unattended)
    {
        ForceMovieRefresh((List<MovieConfiguration>?)null, unattended);
    }

    internal void ForceMovieRefresh(MovieConfiguration sis, bool unattended)
    {
        ForceMovieRefresh(new List<MovieConfiguration> { sis }, unattended);
    }

    private void recommendationsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        MoreBusy();
        mDoc.PreventAutoScan("Recommendations Open");
        //RecommendationView form = new RecommendationView(mDoc, this, mDoc.TvLibrary.Shows.Take(20));
        RecommendationView form = new(mDoc, this, MediaConfiguration.MediaType.tv);
        form.ShowDialog(this);
        mDoc.AllowAutoScan();
        LessBusy();
        FillMyShows(true);
        FillWhenToWatchList();
    }

    private void duplicateMoviesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        MoreBusy();
        mDoc.PreventAutoScan("Duplicate Movies Open");
        DuplicateMovieFinder form = new(mDoc, this);
        form.ShowDialog(this);
        mDoc.AllowAutoScan();
        LessBusy();
    }

    private void movieRecommendationsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        MoreBusy();
        mDoc.PreventAutoScan("Recommendations Open");
        //RecommendationView form = new RecommendationView(mDoc, this, mDoc.FilmLibrary.Movies.Take(20));
        RecommendationView form = new(mDoc, this, MediaConfiguration.MediaType.movie);
        form.ShowDialog(this);
        FillMyMovies();
        mDoc.AllowAutoScan();
        LessBusy();
    }

    internal void ForceRefresh(ShowConfiguration show, bool unattended)
    {
        ForceRefresh(new List<ShowConfiguration> { show }, unattended);
    }

    private void ForceRefresh(bool unattended)
    {
        ForceRefresh((List<ShowConfiguration>?)null, unattended);
    }

    private void settingsCheckToolStripMenuItem_Click(object sender, EventArgs e)
    {
        MoreBusy();
        mDoc.PreventAutoScan("Settings check is open");

        SettingsReview form = new(mDoc, this);
        form.ShowDialog(this);

        mDoc.AllowAutoScan();
        LessBusy();
    }

    private void toolStripButton1_Click_1(object sender, EventArgs e)
    {
        UiScan(null, null, false, TVSettings.ScanType.Full, MediaConfiguration.MediaType.movie);
        tabControl1.SelectTab(tbAllInOne);
    }

    private void movieSearchEnginesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        MovieConfiguration? m = CurrentlySelectedMovie();

        AddEditSearchEngine aese = new(TVDoc.GetMovieSearchers(), m);
        MoreBusy();
        mDoc.PreventAutoScan("Search Engines are open");

        DialogResult dr = aese.ShowDialog(this);
        if (dr == DialogResult.OK)
        {
            mDoc.SetDirty();
            UpdateSearchButtons();
        }
        mDoc.AllowAutoScan();
        LessBusy();
    }

    private MovieConfiguration? CurrentlySelectedMovie() =>
        TreeNodeToMovieItem(movieTree.SelectedNode)
        ?? mDoc.FilmLibrary.Movies.FirstOrDefault();

    private void scanMovieFolderToolStripMenuItem_Click(object sender, EventArgs e)
    {
        //Show Log Pane
        logToolStripMenuItem_Click(sender, e);

        string downloadFolder = AskUserForFolder();
        if (!downloadFolder.HasValue())
        {
            return;
        }

        mDoc.MovieFolderScan(this, downloadFolder);

        FillActionList();
        FocusOnScanResults();
        FillMyMovies(); //We may have updated movies
        Logger.Info("Finished looking for new movies.");
    }

    private static string AskUserForFolder()
    {
        using FolderBrowserDialog fbd = new();
        fbd.ShowDialog();
        return fbd.SelectedPath;

        /*            CommonOpenFileDialog dialog = new CommonOpenFileDialog {IsFolderPicker = true};
                    ui.ShowChildDialog(dialog);
                    return dialog.FileName;
        */
    }

    public (DialogResult, ActionTDownload?) AskAbout(ItemMissing epGroupKey, List<ActionTDownload> actions)
    {
        DialogResult dr = DialogResult.OK;
        ActionTDownload? userChosenAction = null;

        Invoke((MethodInvoker)delegate
        {
            // Running on the UI thread
            ChooseDownload form = new(epGroupKey, actions);
            ShowChildDialog(form);
            dr = form.DialogResult;
            userChosenAction = form.UserChosenAction;
            form.Dispose();
        });

        return (dr, userChosenAction);
    }

    private void browserTestToolStripMenuItem_Click(object sender, EventArgs e)
    {
        CefWrapper.Instance.CheckForBrowserDependencies(true);
    }

    private void tsbScanTV_Click(object sender, EventArgs e)
    {
        UiScan(null, null, false, TVSettings.ScanType.Full, MediaConfiguration.MediaType.tv);
        tabControl1.SelectTab(tbAllInOne);
    }

    private void cleanLibraryFoldersToolStripMenuItem_Click(object sender, EventArgs e)
    {
        mDoc.TheActionList.Clear();
        DoScanPartNotifier f = new(new CleanUpEmptyLibraryFolders(mDoc));
        f.ShowDialog();
        FillActionList();
        FocusOnScanResults();
    }

    private void olvAction_ColumnClick(object sender, ColumnClickEventArgs e)
    {
        if (!olvAction.AllColumns[e.Column].Groupable && olvAction.AllColumns[e.Column].Sortable)
        {
            //TODO work out how to deal with this
        }
    }

    private void olvAction_BeforeSorting(object sender, BeforeSortingEventArgs e)
    {
        if (e.ColumnToSort == olvDate)
        {
            e.ColumnToSort = new OLVColumn("RawDate", "AirDate");
        }

        if (e.ColumnToSort == olvEpisode)
        {
            e.ColumnToSort = new OLVColumn("RawEp", "EpisodeNumber");
        }

        if (e.ColumnToSort==olvSeason)
        {
            e.ColumnToSort = new OLVColumn("RawSe", "SeasonNumberAsInt");
        }

        e.SecondaryColumnToSort = new OLVColumn("AbsoluteOrder", "OrderKey");
        e.SecondarySortOrder = SortOrder.Ascending;
        olvAction.ShowSortIndicator();
    }

    private void tVDBUPdateCheckerLogToolStripMenuItem_Click(object sender, EventArgs e)
    {
        TvdbUpdateChecker form = new(mDoc);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        //Show Log Pane
        logToolStripMenuItem_Click(sender, e);
        MediaConfiguration? config = form.SelectedMedia;
        long? timeSince = form.TimeSince;

        if (timeSince == null || config == null)
        {
            return;
        }

        Logger.Info($"BETA Update Checker: Testing: {config.Name} ({config.Id()}) since {timeSince.Value.FromUnixTime().ToLocalTime()} ({timeSince}), last update was {config.CachedData?.SrvLastUpdated.FromUnixTime().ToLocalTime()} ({config.CachedData?.SrvLastUpdated})");
        TheTVDB.TvdbAccuracyCheck.InvestigateUpdatesSince(config.Id(), timeSince.Value);
    }

    private void requestANewFeatureToolStripMenuItem_Click(object sender, EventArgs e)
        => Helpers.OpenUrl("https://tvrename.featureupvote.com/");
}

/// <summary>
/// This comparer sort list view specificallly for sesaons so that they appear in the season order
/// OLVGroups have a "SortValue" property,
/// which is used if present. Otherwise, the titles of the groups will be compared.
/// </summary>
public class SeasonGroupComparer : IComparer<OLVGroup>
{
    /// <summary>
    /// Create a group comparer
    /// </summary>
    /// <param name="order">The ordering for column values</param>
    public SeasonGroupComparer(SortOrder order)
    {
        this.sortOrder = order;
    }

    /// <summary>
    /// Compare the two groups. OLVGroups have a "SortValue" property,
    /// which is used if present. Otherwise, the titles of the groups will be compared.
    /// </summary>
    /// <param name="x">group1</param>
    /// <param name="y">group2</param>
    /// <returns>An ordering indication: -1, 0, 1</returns>
    public int Compare(OLVGroup? x, OLVGroup? y)
    {
        if (x is null || y is null)
        {
            return 0;
        }

        // If we can compare the sort values, do that.
        // Otherwise do a case insensitive compare on the group header.
        int result;
        if (x.Items.Any() && y.Items.Any())
        {
            result = CompareValue(x).CompareTo(CompareValue(y));
        }
        else if (x.SortValue != null && y.SortValue != null)
        {
            result = x.SortValue.CompareTo(y.SortValue);
        }
        else
        {
            result = string.Compare(x.Header, y.Header, StringComparison.CurrentCultureIgnoreCase);
        }

        if (this.sortOrder == SortOrder.Descending)
        {
            return 0 - result;
        }

        return result;
    }

    private static int CompareValue(OLVGroup x) => ((Item)x.Items.First().RowObject).SeasonNumberAsInt ?? 0;

    private readonly SortOrder sortOrder;
}
