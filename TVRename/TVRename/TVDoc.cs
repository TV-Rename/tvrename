// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
// "Doc" is short for "Document", from the "Document" and "View" model way of thinking of things.
// All the processing and work should be done in here, nothing in UI.cs
// Means we can run TVRename and do useful stuff, without showing any UI. (i.e. text mode / console app)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Xml;
using Alphaleonis.Win32.Filesystem;
using System.Xml.Linq;
using JetBrains.Annotations;
using NLog;
using NodaTime.Extensions;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    public class TVDoc : IDisposable
    {
        private readonly DownloadIdentifiersController downloadIdentifiers;
        public readonly ShowLibrary Library;
        public readonly CommandLineArgs Args;
        internal TVRenameStats CurrentStats;
        public readonly ItemList TheActionList;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ActionEngine actionManager;
        private readonly CacheUpdater cacheManager;
        private readonly FindMissingEpisodes localFinders;
        private readonly FindMissingEpisodes searchFinders;
        private readonly FindMissingEpisodes downloadFinders;

        public string LoadErr;
        public readonly bool LoadOk;
        private ScanProgress scanProgDlg;
        private bool mDirty;

        // ReSharper disable once RedundantDefaultMemberInitializer
        private bool currentlyBusy = false; // This is set to true when scanning and indicates to other objects not to commence a scan of their own
        private DateTime busySince;
        private bool LastScanComplete { get; set; }
        private TVSettings.ScanType lastScanType;

        public TVDoc([CanBeNull] FileInfo settingsFile, CommandLineArgs args)
        {
            Args = args;

            Library = new ShowLibrary();
            cacheManager = new CacheUpdater();
            localFinders = new FindMissingEpisodesLocally(this);
            downloadFinders = new FindMissingEpisodesDownloading(this);
            searchFinders = new FindMissingEpisodesSearch(this);

            mDirty = false;
            TheActionList = new ItemList();

            scanProgDlg = null;

            downloadIdentifiers = new DownloadIdentifiersController();

            LoadOk = (settingsFile is null || LoadXMLSettings(settingsFile)) && TheTVDB.LocalCache.Instance.LoadOk && TVmaze.LocalCache.Instance.LoadOk;
            LoadLanguages();
            LoadStats();
            actionManager = new ActionEngine(CurrentStats);
        }

        private void LoadStats()
        {
            try
            {
                CurrentStats = TVRenameStats.Load();
            }
            catch (Exception)
            {
                // not worried if stats loading fails
            }
        }

        private static void LoadLanguages()
        {
            try
            {
                TheTVDB.LocalCache.Instance.LanguageList = Languages.Load();
            }
            catch (Exception)
            {
                // not worried if language loading fails as we'll repopulate
            }
        }

        [NotNull]
        public TVRenameStats Stats()
        {
            CurrentStats.NsNumberOfShows = Library.Count;
            CurrentStats.NsNumberOfSeasons = 0;
            CurrentStats.NsNumberOfEpisodesExpected = 0;

            foreach (ShowItem si in Library.Shows)
            {
                foreach (List<ProcessedEpisode> k in si.SeasonEpisodes.Values)
                {
                    CurrentStats.NsNumberOfEpisodesExpected += k.Count;
                }

                CurrentStats.NsNumberOfSeasons += si.SeasonEpisodes.Count;
            }

            return CurrentStats;
        }

        public void UpdateIdsFromCache()
        {
            lock (TVmaze.LocalCache.SERIES_LOCK)
            {
                foreach (SeriesInfo show in TVmaze.LocalCache.Instance.CachedData.Values)
                {
                    ShowItem showConfiguration = Library.GetShowItem(show.TvdbCode);
                    if (showConfiguration is null)
                    {
                        continue;
                    }
                    if (showConfiguration.TVmazeCode == 0 || showConfiguration.TVmazeCode == -1)         
                    {
                        showConfiguration.TVmazeCode = show.TvMazeCode;
                    }
                    if(showConfiguration.TVmazeCode != show.TvMazeCode)
                    {
                        Logger.Error($"Issue with copy back of ids {show.Name} {show.TvdbCode} {showConfiguration.TVmazeCode} {show.TvMazeCode} ");
                    }
                }
            }
        }

        public void SetDirty() => mDirty = true;

        public bool Dirty() => mDirty;

        public void DoActions([NotNull] ItemList theList)
        {
            foreach (Item i in theList)
            {
                if (i is Action a)
                {
                    a.ResetOutcome();
                }
            }

            actionManager.DoActions(theList, !Args.Hide && Environment.UserInteractive);

            // remove items from master list, unless it had an error
            TheActionList.RemoveAll(x => x is Action action && action.Outcome.Done && !action.Outcome.Error);

            new CleanUpEmptyLibraryFolders(this).Check(null);
        }

        // ReSharper disable once InconsistentNaming
        public bool DoDownloadsFG(bool unattended,bool tvrMinimised)
        {
            ICollection<SeriesSpecifier> shows = Library.SeriesSpecifiers;
            bool showProgress = !Args.Hide && Environment.UserInteractive && !tvrMinimised;
            bool showMsgBox = !unattended && !Args.Unattended && !Args.Hide && Environment.UserInteractive;

            bool returnValue = cacheManager.DoDownloadsFg(showProgress, showMsgBox, shows);
            UpdateIdsFromCache();
            Library.GenDict();
            return returnValue;
        }

        // ReSharper disable once InconsistentNaming
        public void DoDownloadsBG()
        {
            cacheManager.StartBgDownloadThread(false, Library.SeriesSpecifiers,false, CancellationToken.None);
        }

        public int DownloadsRemaining() =>
            cacheManager.DownloadDone ? 0 : cacheManager.DownloadsRemaining;

        public void SetSearcher(SearchEngine s)
        {
            TVSettings.Instance.TheSearchers.SetSearchEngine(s);
            SetDirty();
        }

        public void SetDefaultScanType(TVSettings.ScanType s)
        {
            if (s == TVSettings.Instance.UIScanType)
            {
                return;
            }
            TVSettings.Instance.UIScanType = s;
            SetDirty();
        }

        public static Searchers GetSearchers() => TVSettings.Instance.TheSearchers;

        public void TidyCaches()
        {
            TheTVDB.LocalCache.Instance.Tidy(Library.Values);
            TVmaze.LocalCache.Instance.Tidy(Library.Values);
        }

        public void Closing()
        {
            cacheManager.StopBgDownloadThread();
            Stats().Save();
            TheTVDB.LocalCache.Instance.LanguageList.Save();
        }

        public static void SearchForEpisode([CanBeNull] ProcessedEpisode ep)
        {
            if (ep is null)
            {
                return;
            }

            Helpers.SysOpen(TVSettings.Instance.BTSearchURL(ep));
        }

        public void DoWhenToWatch(bool cachedOnly,bool unattended,bool hidden)
        {
            if (!cachedOnly && !DoDownloadsFG(unattended,hidden))
            {
                return;
            }

            if (cachedOnly)
            {
                UpdateIdsFromCache();
                Library.GenDict();
            }
        }

        // ReSharper disable once InconsistentNaming
        public void WriteXMLSettings()
        {
            DirectoryInfo di = PathManager.TVDocSettingsFile.Directory;
            if (!di.Exists)
            {
                di.Create();
            }
            // backup old settings before writing new ones
            FileHelper.Rotate(PathManager.TVDocSettingsFile.FullName);
            Logger.Info("Saving Settings to {0}", PathManager.TVDocSettingsFile.FullName);

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true
            };

            using (XmlWriter writer = XmlWriter.Create(PathManager.TVDocSettingsFile.FullName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("TVRename");

                writer.WriteAttributeToXml("Version", "2.1");

                TVSettings.Instance.WriteXML(writer); // <Settings>

                writer.WriteStartElement("MyShows");
                foreach (ShowItem si in Library.Values)
                {
                    si.WriteXmlSettings(writer);
                }

                writer.WriteEndElement(); // MyShows

                XmlHelper.WriteStringsToXml(TVSettings.Instance.LibraryFolders, writer, "MonitorFolders", "Folder");
                XmlHelper.WriteStringsToXml(TVSettings.Instance.IgnoreFolders, writer, "IgnoreFolders", "Folder");
                XmlHelper.WriteStringsToXml(TVSettings.Instance.DownloadFolders, writer, "FinderSearchFolders","Folder");
                XmlHelper.WriteStringsToXml(TVSettings.Instance.IgnoredAutoAddHints, writer, "IgnoredAutoAddHints","Hint");
                writer.WriteStringsToXml(TVSettings.Instance.Ignore, "IgnoreItems","Ignore");
                writer.WriteStringsToXml(TVSettings.Instance.PreviouslySeenEpisodes, "PreviouslySeenEpisodes", "Episode");

                writer.WriteEndElement(); // tvrename
                writer.WriteEndDocument();
            }

            mDirty = false;
            Stats().Save();
            TheTVDB.LocalCache.Instance.LanguageList.Save();
        }

        // ReSharper disable once InconsistentNaming
        private bool LoadXMLSettings([CanBeNull] FileInfo from)
        {
            Logger.Info("Loading Settings from {0}", from?.FullName);
            if (from is null)
            {
                return true;
            }

            try
            {
                if (!from.Exists)
                {
                    return true; // that's ok
                }

                XElement x = XElement.Load(from.FullName);

                if (x.Name.LocalName != "TVRename")
                {
                    LoadErr = from.Name + " : Not a TVRename settings file";
                    return false;
                }

                if (x.Attribute("Version")?.Value != "2.1")
                {
                    LoadErr = from.Name + " : Incompatible version";
                    return false;
                }

                TVSettings.Instance.load(x.Descendants("Settings").First());
                Library.LoadFromXml(x.Descendants("MyShows").First());
                TVSettings.Instance.IgnoreFolders =
                    x.Descendants("IgnoreFolders").FirstOrDefault()?.ReadStringsFromXml("Folder") ??new List<string>();
                TVSettings.Instance.DownloadFolders =
                    x.Descendants("FinderSearchFolders").FirstOrDefault()?.ReadStringsFromXml("Folder") ?? new List<string>();
                TVSettings.Instance.IgnoredAutoAddHints =
                    x.Descendants("IgnoredAutoAddHints").FirstOrDefault()?.ReadStringsFromXml("Hint") ?? new List<string>();
                TVSettings.Instance.Ignore =
                    x.Descendants("IgnoreItems").FirstOrDefault()?.ReadIiFromXml("Ignore") ??new List<IgnoreItem>();
                TVSettings.Instance.PreviouslySeenEpisodes = new PreviouslySeenEpisodes(x.Descendants("PreviouslySeenEpisodes").FirstOrDefault());

                //MonitorFolders are a little more complex as there is a parameter named the same which we need to ignore
                IEnumerable<XElement> mfs = x.Descendants("MonitorFolders");
                foreach (XElement mf in mfs.Where(mf => mf.Descendants("Folder").Any()))
                {
                    TVSettings.Instance.LibraryFolders = mf.ReadStringsFromXml("Folder");
                }
            }
            catch (Exception e)
            {
                Logger.Warn(e, "Problem on Startup loading File");
                LoadErr = from.Name + " : " + e.Message;
                return false;
            }

            return true;
        }

        private void OutputActionFiles()
        {
            if (!LastScanComplete)
            {
                return;
            }

            // ReSharper disable once InconsistentNaming
            List<ActionListExporter> ALExpoters = new List<ActionListExporter>
            {
                new MissingXML(TheActionList),
                new MissingCSV(TheActionList),
                new CopyMoveXml(TheActionList),
                new RenamingXml(TheActionList)
            };

            foreach (ActionListExporter ue in ALExpoters)
            {
                if (ue.Active() && ue.ApplicableFor(lastScanType))
                {
                    ue.Run();
                }
            }
        }

        public void ExportShowInfo()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                new ShowsTXT(Library.GetSortedShowItems()).Run();
            }).Start();

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                new ShowsHTML(Library.GetSortedShowItems()).Run();
            }).Start();
        }

        public void WriteUpcoming()
        {
            List<UpcomingExporter> lup = new List<UpcomingExporter> {new UpcomingRSS(this), new UpcomingXML(this),new UpcomingiCAL(this)};

            foreach (UpcomingExporter ue in lup)
            {
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    if (ue.Active())
                    {
                        ue.Run();
                    }
                }).Start();
            }
        }

        public void WriteRecent()
        {
            List<RecentExporter> reps = new List<RecentExporter> { new RecentASXExporter(this), new RecentM3UExporter(this), new RecentWPLExporter(this), new RecentXSPFExporter(this) };

            foreach (RecentExporter ue in reps)
            {
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    if (ue.Active())
                    {
                        ue.Run();
                    }
                }).Start();
            }
        }

        internal void ShowAddedOrEdited(bool download, bool unattended,bool hidden)
        {
            SetDirty();
            if (download)
            {
                if (!DoDownloadsFG(unattended,hidden))
                {
                    return;
                }
            }

            DoWhenToWatch(true, unattended,hidden);

            WriteUpcoming();
            WriteRecent();
           
            ExportShowInfo(); //Save shows list to disk
        }

        public ConcurrentBag<ShowNotFoundException> ShowProblems => cacheManager.Problems;

        public void Scan([CanBeNull] IEnumerable<ShowItem> passedShows, bool unattended, TVSettings.ScanType st, bool hidden)
        {
            try
            {
                PreventAutoScan("Scan "+st.PrettyPrint());

                //Get the default set of shows defined by the specified type
                IEnumerable<ShowItem> shows = GetShowList(st, passedShows);

                //If still null then return
                if (shows is null)
                {
                    Logger.Warn("No Shows Provided to Scan");
                    return;
                }

                if (!DoDownloadsFG(unattended,hidden))
                {
                    return;
                }

                Thread actionWork = new Thread(ScanWorker) {Name = "ActionWork"};
                CancellationTokenSource cts = new CancellationTokenSource();
                actionWork.SetApartmentState(ApartmentState.STA); //needed to allow DragDrop on any UI this thread creates

                SetupScanUi(hidden);

                actionWork.Start(new ScanSettings(shows.ToList(),unattended,hidden,st,cts.Token));

                if (scanProgDlg != null && scanProgDlg.ShowDialog() == DialogResult.Cancel)
                {
                    cts.Cancel();
                }
                else
                {
                    actionWork.Join();
                }
                downloadIdentifiers.Reset();
                OutputActionFiles(); //Save missing shows to XML (and others)
            }
            catch (Exception e)
            {
                Logger.Fatal(e, "Unhandled Exception in Scan");
            }
            finally
            {
                AllowAutoScan();
            }
        }

        public struct ScanSettings : IEquatable<ScanSettings>
        {
            public readonly bool Unattended;
            public readonly bool Hidden;
            public readonly TVSettings.ScanType Type;
            public readonly List<ShowItem> Shows;
            public readonly CancellationToken Token;

            public ScanSettings(List<ShowItem> list, bool unattended, bool hidden, TVSettings.ScanType st,CancellationToken tok)
            {
                Shows = list;
                Unattended = unattended;
                Hidden = hidden;
                Type = st;
                Token = tok;
            }

            public bool Equals(ScanSettings other) => Shows==other.Shows && Unattended==other.Unattended && Hidden==other.Hidden && Type==other.Type && Token==other.Token;
        }

        private void SetupScanUi(bool hidden)
        {
            if (!Args.Hide && Environment.UserInteractive)
            {
                scanProgDlg = new ScanProgress(
                    TVSettings.Instance.DoBulkAddInScan, 
                    TVSettings.Instance.RenameCheck || TVSettings.Instance.MissingCheck,
                    TVSettings.Instance.RemoveDownloadDirectoriesFiles || TVSettings.Instance.ReplaceWithBetterQuality,
                    localFinders.Active(),
                    downloadFinders.Active(),
                    searchFinders.Active()
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

        [CanBeNull]
        private IEnumerable<ShowItem> GetShowList(TVSettings.ScanType st, IEnumerable<ShowItem> passedShows)
        {
            switch (st)
            {
                case TVSettings.ScanType.Full:
                    return Library.GetSortedShowItems();

                case TVSettings.ScanType.Quick:
                    return GetQuickShowsToScan(true, true);

                case TVSettings.ScanType.Recent:
                    return Library.GetRecentShows();

                case TVSettings.ScanType.SingleShow:
                    return passedShows;

                default:
                    return null;
            }
        }

        public void DoAllActions()
        {
            PreventAutoScan("Do all actions");
            ItemList theList = new ItemList();

            theList.AddRange(TheActionList.Actions);

            DoActions(theList);
            AllowAutoScan();
        }

        [NotNull]
        private IEnumerable<ShowItem> GetQuickShowsToScan(bool doMissingRecents, bool doFilesInDownloadDir)
        {
            List<ShowItem> showsToScan = new List<ShowItem>();
            if (doFilesInDownloadDir)
            {
                showsToScan = GetShowsThatHaveDownloads();
            }

            if (doMissingRecents)
            {
                IEnumerable<ProcessedEpisode> lpe = GetMissingEps();
                foreach (ProcessedEpisode pe in lpe.Where(pe => !showsToScan.Contains(pe.Show)))
                {
                    showsToScan.Add(pe.Show);
                }
            }
            return showsToScan;
        }

        public void RemoveIgnored()
        {
            ItemList toRemove = new ItemList();
            int numberIgnored=0;
            int numberPreviouslySeen = 0;
            foreach (Item item in TheActionList)
            {
                if (TVSettings.Instance.Ignore.Any(ii => ii.SameFileAs(item.Ignore)))
                {
                    toRemove.Add(item);
                    numberIgnored++;
                }

                if (TVSettings.Instance.IgnorePreviouslySeen)
                {
                    if (TVSettings.Instance.PreviouslySeenEpisodes.Includes(item))
                    {
                        toRemove.Add(item);
                        numberPreviouslySeen++;
                    }
                }
            }

            Logger.Info($"Removing {toRemove.Count} items from the missing items because they are either in the ignore list ({numberIgnored}) or you have ignore previously seen episodes enables ({numberPreviouslySeen})");

            foreach (Item action in toRemove)
            {
                TheActionList.Remove(action);
            }
        }

        public void ForceUpdateImages([NotNull] ShowItem si)
        {
            TheActionList.Clear();

            Logger.Info("*******************************");
            Logger.Info("Force Update Images: " + si.ShowName);

            Dictionary<int, List<string>> allFolders = si.AllExistngFolderLocations();

            if (!string.IsNullOrEmpty(si.AutoAddFolderBase) && allFolders.Any())
            {
                TheActionList.Add(
                    downloadIdentifiers.ForceUpdateShow(DownloadIdentifier.DownloadType.downloadImage, si));

                si.BannersLastUpdatedOnDisk = DateTime.Now;
                SetDirty();
            }

            // process each folder for each season...
            foreach (int snum in si.GetSeasonKeys())
            {
                if (si.IgnoreSeasons.Contains(snum) || !allFolders.ContainsKey(snum))
                {
                    continue; // ignore/skip this season
                }

                if (snum == 0 && si.CountSpecials)
                {
                    continue; // don't process the specials season, as they're merged into the seasons themselves
                }

                if (snum == 0 && TVSettings.Instance.IgnoreAllSpecials)
                {
                    continue;
                }

                // all the folders for this particular season
                List<string> folders = allFolders[snum];

                foreach (string folder in folders)
                {
                    //Image series checks here
                    TheActionList.Add(
                        downloadIdentifiers.ForceUpdateSeason(DownloadIdentifier.DownloadType.downloadImage, si, folder,
                            snum));
                }
            } // for each season of this show

            RemoveIgnored();
        }

        private void ScanWorker(object o)
        {
            try
            {
                ScanSettings settings = (ScanSettings) o;

                //When doing a full scan the show list is null indicating that all shows should be checked
                List <ShowItem> specific = settings.Shows ?? Library.Values.ToList();

                while (!Args.Hide && Environment.UserInteractive && (scanProgDlg is null || !scanProgDlg.Ready))
                {
                    Thread.Sleep(10); // wait for thread to create the dialog
                }

                TheActionList.Clear();
                SetProgressDelegate noProgress = NoProgress;

                if (!settings.Unattended && settings.Type != TVSettings.ScanType.SingleShow)
                {
                    new FindNewShowsInDownloadFolders(this).Check(scanProgDlg is null ? noProgress : scanProgDlg.AddNewProg, 0, 50, specific, settings);
                    new FindNewShowsInLibrary(this).Check(scanProgDlg is null ? noProgress : scanProgDlg.AddNewProg, 50, 100, specific, settings);
                }
                
                new CheckShows(this).Check(scanProgDlg is null ? noProgress : scanProgDlg.MediaLibProg, specific, settings);
                new CleanDownloadDirectory(this).Check(scanProgDlg is null ? noProgress : scanProgDlg.DownloadFolderProg, specific, settings);
                localFinders.Check(scanProgDlg is null ? noProgress : scanProgDlg.LocalSearchProg, specific, settings);
                downloadFinders.Check(scanProgDlg is null ? noProgress : scanProgDlg.DownloadingProg, specific, settings);
                searchFinders.Check(scanProgDlg is null? noProgress : scanProgDlg.ToBeDownloadedProg, specific, settings);

                if (settings.Token.IsCancellationRequested)
                {
                    TheActionList.Clear();
                    LastScanComplete = false;
                    return;
                }

                // sort Action list by type
                TheActionList.Sort(new ActionItemSorter()); // was new ActionSorter()

                Stats().FindAndOrganisesDone++;
                lastScanType = settings.Type;
                LastScanComplete = true;
            }
            catch (TVRenameOperationInterruptedException)
            {
                Logger.Warn("Scan cancelled by user");
                TheActionList.Clear();
                LastScanComplete = false;
            }
            catch (Exception e)
            {
                Logger.Fatal(e, "Unhandled Exception in ScanWorker");
                LastScanComplete = false;
            }
            finally
            {
                scanProgDlg?.Done(); 
            }
        }

        private static void NoProgress(int pct, string message)
        {
            //Nothing to do - Method is called if we have no UI
        }

        public static bool  MatchesSequentialNumber(string filename, [NotNull] ProcessedEpisode pe)
        {
            if (pe.OverallNumber == -1)
            {
                return false;
            }

            string num = pe.OverallNumber.ToString();
            string matchText = "X" + filename + "X"; // need to pad to let it match non-numbers at start and end

            Match betterMatch = Regex.Match(matchText, @"(E|e|Ep|ep|episode|Episode) ?0*(?<sequencenumber>\d+)\D");

            if (betterMatch.Success)
            {
                int sequenceNUm = int.Parse(betterMatch.Groups["sequencenumber"]?.Value??"-2");
                return sequenceNUm == pe.OverallNumber;
            }

            return Regex.Match(matchText, @"\D0*" + num + @"\D").Success;
        }

        [NotNull]
        private IEnumerable<ProcessedEpisode> GetMissingEps()
        {
            int dd = TVSettings.Instance.WTWRecentDays;
            DirFilesCache dfc = new DirFilesCache();
            return GetMissingEps(dfc, Library.GetRecentAndFutureEps(dd));
        }

        [NotNull]
        private static IEnumerable<ProcessedEpisode> GetMissingEps(DirFilesCache dfc, [NotNull] IEnumerable<ProcessedEpisode> lpe)
        {
            List<ProcessedEpisode> missing = new List<ProcessedEpisode>();

            foreach (ProcessedEpisode pe in lpe)
            {
                List<FileInfo> fl = dfc.FindEpOnDisk(pe);
                bool foundOnDisk = fl.Any(file => file.Name.StartsWith(TVSettings.Instance.FilenameFriendly(TVSettings.Instance.NamingStyle.NameFor(pe)), StringComparison.OrdinalIgnoreCase));
                bool alreadyAired;

                DateTime? airDate = pe.GetAirDateDt(true);

                if (airDate.HasValue)
                {
                    alreadyAired = airDate.Value.CompareTo(DateTime.Now) < 0;
                }
                else
                {
                    alreadyAired = true;
                }

                if (!foundOnDisk && alreadyAired && pe.Show.DoMissingCheck)
                {
                    missing.Add(pe);
                }
            }

            return missing;
        }

        [NotNull]
        private List<ShowItem> GetShowsThatHaveDownloads()
        {
            //for each directory in settings directory
            //for each file in directory
            //for each saved show (order by recent)
            //does show match selected file?
            //if so add series to list of series scanned

            List<ShowItem> showsToScan = new List<ShowItem>();

            foreach (string dirPath in TVSettings.Instance.DownloadFolders)
            {
                if (!Directory.Exists(dirPath))
                {
                    continue;
                }

                try{ 
                    string[] x = Directory.GetFiles(dirPath, "*", System.IO.SearchOption.AllDirectories);
                    Logger.Info($"Processing {x.Length} files for shows that need to be scanned");

                    foreach (string filePath in x)
                    {
                        Logger.Info($"Checking to see whether {filePath} is a file that for a show that need scanning");

                        if (!File.Exists(filePath))
                        {
                            continue;
                        }

                        FileInfo fi = new FileInfo(filePath);

                        if (fi.IgnoreFile())
                        {
                            continue;
                        }

                        foreach (ShowItem si in Library.Shows
                            .Where(si => !showsToScan.Contains(si))
                            .Where(si => si.NameMatch(fi, TVSettings.Instance.UseFullPathNameToMatchSearchFolders)))
                        {
                            showsToScan.Add(si);
                        }
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logger.Warn($"Could not access files in {dirPath} {ex.Message}");
                }
                catch (System.IO.DirectoryNotFoundException  ex)
                {
                    Logger.Warn($"Could not access files in {dirPath} {ex.Message}");
                }
                catch (System.IO.IOException ex)
                {
                    Logger.Warn($"Could not access files in {dirPath} {ex.Message}");
                }
                catch (NotSupportedException ex)
                {
                    Logger.Error($"Please update 'Download Folders' {dirPath} is not supported {ex.Message}");
                }
                try { 
                    string[] directories = Directory.GetDirectories(dirPath, "*", System.IO.SearchOption.AllDirectories);
                    Logger.Info($"Processing {directories.Length} directories for shows that need to be scanned");

                    foreach (string subDirPath in directories)
                    {
                        Logger.Info($"Checking to see whether {subDirPath} has any shows that need scanning");

                        if (!Directory.Exists(subDirPath))
                        {
                            continue;
                        }

                        DirectoryInfo di = new DirectoryInfo(subDirPath);

                        foreach (ShowItem si in Library.Values
                            .Where(si => !showsToScan.Contains(si))
                            .Where(si => si.NameMatch(di,TVSettings.Instance.UseFullPathNameToMatchSearchFolders)))
                        {
                            showsToScan.Add(si);
                        }
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logger.Warn($"Could not access sub-directories in {dirPath} {ex.Message}");
                }
                catch (System.IO.DirectoryNotFoundException ex)
                {
                    Logger.Warn($"Could not access sub-directories in {dirPath} {ex.Message}");
                }
                catch (System.IO.IOException ex)
                {
                    Logger.Warn($"Could not access sub-directories in {dirPath} {ex.Message}");
                }
                catch (NotSupportedException ex)
                {
                    Logger.Error($"Please update 'Download Folders' {dirPath} is not supported {ex.Message}");
                }
            }
            return showsToScan;
        }

        internal void ForceRefresh([CanBeNull] IEnumerable<ShowItem> sis, bool unattended,bool tvrMinimised)
        {
            PreventAutoScan("Force Refresh");
            if (sis != null)
            {
                foreach (ShowItem si in sis)
                {
                    switch (si.Provider)
                    {
                        case ShowItem.ProviderType.TVmaze:
                            TVmaze.LocalCache.Instance.ForgetShow(si.TvdbCode, si.TVmazeCode, true, si.UseCustomLanguage, si.CustomLanguageCode);
                            break;

                        case ShowItem.ProviderType.TheTVDB:
                            TheTVDB.LocalCache.Instance.ForgetShow(si.TvdbCode, si.TVmazeCode,true, si.UseCustomLanguage, si.CustomLanguageCode);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            DoDownloadsFG(unattended, tvrMinimised);
            AllowAutoScan();
        }

        // ReSharper disable once InconsistentNaming
        internal void TVDBServerAccuracyCheck(bool unattended,bool hidden)
        {
            PreventAutoScan("TVDB Accuracy Check");
            IEnumerable<SeriesInfo> seriesToUpdate = TheTVDB.LocalCache.Instance.ServerAccuracyCheck();
            IEnumerable<ShowItem> showsToUpdate = seriesToUpdate.Select(info => Library.GetShowItem(info.TvdbCode));
            ForceRefresh(showsToUpdate, unattended, hidden);
            DoDownloadsBG();
            AllowAutoScan();
        }
        
        private void ReleaseUnmanagedResources()
        {
            cacheManager.StopBgDownloadThread();
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                // ReSharper disable once UseNullPropagation
                if (scanProgDlg != null)
                {
                    scanProgDlg.Dispose();
                }

                // ReSharper disable once UseNullPropagation
                if (cacheManager != null)
                {
                    cacheManager.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TVDoc()
        {
            Dispose(false);
        }

        public void PreventAutoScan(string v)
        {
            Logger.Info($"TV Rename is about to be busy doing {v} since {DateTime.Now.ToLocalDateTime()}");
            currentlyBusy = true;
            busySince = DateTime.Now;
        }

        public void AllowAutoScan()
        {
            Logger.Info($"TV Rename free again (busy since {busySince})");
            currentlyBusy =false;
        }

        public bool AutoScanCanRun() => !currentlyBusy;

        public void RunExporters()
        {
            OutputActionFiles();
            ExportShowInfo();
            WriteUpcoming();
            WriteRecent();
        }

        public void ClearShowProblems() => cacheManager.ClearProblems();

        public void ReindexLibrary()
        {
            Library.ReIndex();
        }

        public void UpdateMissingAction([NotNull] ItemMissing mi, string fileName)
        {
            // make new Item for copying/moving to specified location
            FileInfo from = new FileInfo(fileName);
            FileInfo to = new FileInfo(mi.TheFileNoExt + from.Extension);
            TheActionList.Add(
                new ActionCopyMoveRename(
                    TVSettings.Instance.LeaveOriginals
                        ? ActionCopyMoveRename.Op.copy
                        : ActionCopyMoveRename.Op.move, from, to
                    , mi.Episode, true, mi, this));

            // and remove old Missing item
            TheActionList.Remove(mi);

            // if we're copying/moving a file across, we might also want to make a thumbnail or NFO for it
            DownloadIdentifiersController di = new DownloadIdentifiersController();
            TheActionList.Add(di.ProcessEpisode(mi.Episode, to));

            //If keep together is active then we may want to copy over related files too
            if (TVSettings.Instance.KeepTogether)
            {
                FileFinder.KeepTogether(TheActionList, false, true, this);
            }

        }

        public void IgnoreSeasonForItem([CanBeNull] Item er)
        {
            if (er?.Episode is null)
            {
                return;
            }

            int snum = er.Episode.AppropriateSeasonNumber;

            if (!er.Episode.Show.IgnoreSeasons.Contains(snum))
            {
                er.Episode.Show.IgnoreSeasons.Add(snum);
            }

            // remove all other episodes of this season from the Action list
            ItemList remove = new ItemList();
            foreach (Item action in TheActionList)
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
                TheActionList.Remove(action);
            }

            if (remove.Count > 0)
            {
                SetDirty();
            }
        }

        public void RevertAction(Item item)
        {
            Action revertAction = (Action)item;
            ItemMissing m2 = revertAction.UndoItemMissing;

            if (m2 is null)
            {
                return;
            }

            TheActionList.Add(m2);
            TheActionList.Remove(revertAction);

            //We can remove any CopyMoveActions that are closely related too
            if (!(revertAction is ActionCopyMoveRename))
            {
                return;
            }

            ActionCopyMoveRename i2 = (ActionCopyMoveRename)item;
            List<Item> toRemove = new List<Item>();

            foreach (Item a in TheActionList)
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
            TheActionList.Remove(toRemove);
        }
    }
}
