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
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Xml;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using System.Xml.Linq;
using NLog;
using NodaTime.Extensions;
using File = Alphaleonis.Win32.Filesystem.File;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    public class TVDoc : IDisposable
    {
        private readonly DownloadIdentifiersController downloadIdentifiers;
        public readonly ShowLibrary Library;
        public readonly CommandLineArgs Args;
        internal TVRenameStats CurrentStats;
        public ItemList TheActionList;
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

        public TVDoc(FileInfo settingsFile, CommandLineArgs args)
        {
            Args = args;

            Library = new ShowLibrary();
            CurrentStats = new TVRenameStats();
            actionManager = new ActionEngine(CurrentStats);
            cacheManager = new CacheUpdater();
            localFinders = new FindMissingEpisodesLocally(this);
            downloadFinders = new FindMissingEpisodesDownloading(this);
            searchFinders = new FindMissingEpisodesSearch(this);

            mDirty = false;
            TheActionList = new ItemList();

            scanProgDlg = null;

            downloadIdentifiers = new DownloadIdentifiersController();

            LoadOk = (settingsFile == null || LoadXMLSettings(settingsFile)) && TheTVDB.Instance.LoadOk;
        }

        public TVRenameStats Stats()
        {
            CurrentStats.NsNumberOfShows = Library.Count;
            CurrentStats.NsNumberOfSeasons = 0;
            CurrentStats.NsNumberOfEpisodesExpected = 0;

            foreach (ShowItem si in Library.Shows)
            {
                foreach (List<ProcessedEpisode> k in si.SeasonEpisodes.Values)
                    CurrentStats.NsNumberOfEpisodesExpected += k.Count;

                CurrentStats.NsNumberOfSeasons += si.SeasonEpisodes.Count;
            }

            return CurrentStats;
        }

        public void SetDirty() => mDirty = true;

        public bool Dirty() => mDirty;

        public void DoActions(ItemList theList)
        {
            actionManager.DoActions(theList, !Args.Hide && Environment.UserInteractive);
        }

        // ReSharper disable once InconsistentNaming
        public bool DoDownloadsFG(bool unattended)
        {
            ICollection<SeriesSpecifier> shows = Library.SeriesSpecifiers;
            bool showProgress = (!Args.Hide) && Environment.UserInteractive;
            bool showMsgBox = !unattended && (!Args.Unattended) && (!Args.Hide) && Environment.UserInteractive;

            bool returnValue = cacheManager.DoDownloadsFg(showProgress, showMsgBox, shows);
            Library.GenDict();
            return returnValue;
        }

        // ReSharper disable once InconsistentNaming
        public void DoDownloadsBG()
        {
            ICollection<SeriesSpecifier> shows = Library.SeriesSpecifiers;
            cacheManager.StartBgDownloadThread(false, shows,false);
        }

        public int DownloadsRemaining() =>
            cacheManager.DownloadDone ? 0 : cacheManager.DownloadsRemaining;

        public void SetSearcher(int n)
        {
            TVSettings.Instance.TheSearchers.SetToNumber(n);
            SetDirty();
        }

        public static Searchers GetSearchers() => TVSettings.Instance.TheSearchers;

        public void TidyTvdb()
        {
            TheTVDB.Instance.Tidy(Library.Values);
        }

        public void Closing()
        {
            cacheManager.StopBgDownloadThread();
            Stats().Save();
            TheTVDB.Instance.LanguageList.Save();
        }

        public static void SearchForEpisode(ProcessedEpisode ep)
        {
            if (ep == null)
                return;

            Helpers.SysOpen(TVSettings.Instance.BTSearchURL(ep));
        }

        public void DoWhenToWatch(bool cachedOnly,bool unattended)
        {
            if (!cachedOnly && !DoDownloadsFG(unattended))
                return;

            if (cachedOnly)
                Library.GenDict();
        }

        // ReSharper disable once InconsistentNaming
        public void WriteXMLSettings()
        {
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

                XmlHelper.WriteAttributeToXml(writer, "Version", "2.1");

                TVSettings.Instance.WriteXML(writer); // <Settings>

                writer.WriteStartElement("MyShows");
                foreach (ShowItem si in Library.Values)
                    si.WriteXmlSettings(writer);

                writer.WriteEndElement(); // MyShows

                XmlHelper.WriteStringsToXml(TVSettings.Instance.LibraryFolders, writer, "MonitorFolders", "Folder");
                XmlHelper.WriteStringsToXml(TVSettings.Instance.IgnoreFolders, writer, "IgnoreFolders", "Folder");
                XmlHelper.WriteStringsToXml(TVSettings.Instance.DownloadFolders, writer, "FinderSearchFolders","Folder");
                XmlHelper.WriteStringsToXml(TVSettings.Instance.IgnoredAutoAddHints, writer, "IgnoredAutoAddHints","Hint");
                XmlHelper.WriteStringsToXml(TVSettings.Instance.Ignore, writer, "IgnoreItems","Ignore");
                XmlHelper.WriteStringsToXml(TVSettings.Instance.PreviouslySeenEpisodes, writer, "PreviouslySeenEpisodes", "Episode");

                writer.WriteEndElement(); // tvrename
                writer.WriteEndDocument();
            }

            mDirty = false;
            Stats().Save();
            TheTVDB.Instance.LanguageList.Save();
        }

        // ReSharper disable once InconsistentNaming
        private bool LoadXMLSettings(FileInfo from)
        {
            Logger.Info("Loading Settings from {0}", from?.FullName);
            if (from == null)
                return true;

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
                foreach (XElement mf in mfs)
                {
                    if (mf.Descendants("Folder").Any())
                    {
                        TVSettings.Instance.LibraryFolders = mf.ReadStringsFromXml("Folder");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Warn(e, "Problem on Startup loading File");
                LoadErr = from.Name + " : " + e.Message;
                return false;
            }

            try
            {
                CurrentStats = TVRenameStats.Load();
            }
            catch (Exception)
            {
                // not worried if stats loading fails
            }

            try
            {
                TheTVDB.Instance.LanguageList = Languages.Load();
            }
            catch (Exception)
            {
                // not worried if language loading fails as we'll repopulate
            }

            return true;
        }

        private void OutputActionFiles()
        {
            if (!LastScanComplete) return;
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
                new ShowsTXT(Library.GetShowItems()).Run();
            }).Start();

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                new ShowsHTML(Library.GetShowItems()).Run();
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

        internal void ShowAddedOrEdited(bool download, bool unattended)
        {
            SetDirty();
            if (download)
            {
                if (!DoDownloadsFG(unattended))
                    return;
            }

            DoWhenToWatch(true, unattended);

            WriteUpcoming();
            WriteRecent();
           
            ExportShowInfo(); //Save shows list to disk
        }

        public ConcurrentBag<int> ShowProblems => cacheManager.Problems;

        public void Scan(List<ShowItem> shows, bool unattended, TVSettings.ScanType st)
        {
            try
            {
                PreventAutoScan("Scan "+st.PrettyPrint());

                //Get the default set of shows defined by the specified type
                if (shows == null) shows = GetShowList(st);

                //If still null then return
                if (shows == null)
                {
                    Logger.Warn("No Shows Provided to Scan");
                    return;
                }

                if (!DoDownloadsFG(unattended))
                    return;

                Thread actionWork = new Thread(ScanWorker) {Name = "ActionWork"};
                CancellationTokenSource cts = new CancellationTokenSource();
                actionWork.SetApartmentState(ApartmentState.STA); //needed to allow DragDrop on any UI this thread creates

                SetupScanUi();

                actionWork.Start(new ScanSettings(shows.ToList(),unattended,st,cts.Token));

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

        public struct ScanSettings
        {
            public readonly bool Unattended;
            public readonly TVSettings.ScanType Type;
            public readonly List<ShowItem> Shows;
            public readonly CancellationToken Token;

            public ScanSettings(List<ShowItem> list, bool unattended, TVSettings.ScanType st,CancellationToken tok)
            {
                Shows = list;
                Unattended = unattended;
                Type = st;
                Token = tok;
            }
        }

        private void SetupScanUi()
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
            }
            else
                scanProgDlg = null;
        }

        private List<ShowItem> GetShowList(TVSettings.ScanType st)
        {
            if (st == TVSettings.ScanType.Full) return Library.GetShowItems();
            if (st == TVSettings.ScanType.Quick) return GetQuickShowsToScan(true, true);
            if (st == TVSettings.ScanType.Recent) return Library.GetRecentShows();
            return null;
        }

        public void DoAllActions()
        {
            PreventAutoScan("Do all actions");
            ItemList theList = new ItemList();

            theList.AddRange(TheActionList.Actions());

            DoActions(theList);
            AllowAutoScan();
        }

        private List<ShowItem> GetQuickShowsToScan(bool doMissingRecents, bool doFilesInDownloadDir)
        {
            List<ShowItem> showsToScan = new List<ShowItem>();
            if (doFilesInDownloadDir) showsToScan = GetShowsThatHaveDownloads();

            if (doMissingRecents)
            {
                List<ProcessedEpisode> lpe = GetMissingEps();
                if (lpe != null)
                {
                    foreach (ProcessedEpisode pe in lpe)
                    {
                        if (!showsToScan.Contains(pe.Show)) showsToScan.Add(pe.Show);
                    }
                }
            }
            return showsToScan;
        }

        public void RemoveIgnored()
        {
            ItemList toRemove = new ItemList();
            foreach (Item item in TheActionList)
            {
                if (TVSettings.Instance.Ignore.Any(ii => ii.SameFileAs(item.Ignore)))
                {
                    toRemove.Add(item);
                }

                if (TVSettings.Instance.IgnorePreviouslySeen)
                {
                    if (TVSettings.Instance.PreviouslySeenEpisodes.Includes(item))
                    {
                        toRemove.Add(item);
                    }
                }
            }

            foreach (Item action in toRemove)
                TheActionList.Remove(action);
        }

        public void ForceUpdateImages(ShowItem si)
        {
            TheActionList = new ItemList();

            Logger.Info("*******************************");
            Logger.Info("Force Update Images: " + si.ShowName);

            Dictionary<int, List<string>> allFolders = si.AllExistngFolderLocations();

            if (!string.IsNullOrEmpty(si.AutoAddFolderBase) && (allFolders.Any()))
            {
                TheActionList.Add(
                    downloadIdentifiers.ForceUpdateShow(DownloadIdentifier.DownloadType.downloadImage, si));

                si.BannersLastUpdatedOnDisk = DateTime.Now;
                SetDirty();
            }

            // process each folder for each season...
            int[] numbers = new int[si.SeasonEpisodes.Keys.Count];
            si.SeasonEpisodes.Keys.CopyTo(numbers, 0);

            foreach (int snum in numbers)
            {
                if ((si.IgnoreSeasons.Contains(snum)) || (!allFolders.ContainsKey(snum)))
                    continue; // ignore/skip this season

                if ((snum == 0) && (si.CountSpecials))
                    continue; // don't process the specials season, as they're merged into the seasons themselves

                if ((snum == 0) && TVSettings.Instance.IgnoreAllSpecials)
                    continue;

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

                while (!Args.Hide && Environment.UserInteractive && ((scanProgDlg == null) || (!scanProgDlg.Ready)))
                    Thread.Sleep(10); // wait for thread to create the dialog

                TheActionList = new ItemList();
                SetProgressDelegate noProgress = NoProgress;

                if (!settings.Unattended && settings.Type != TVSettings.ScanType.SingleShow)
                {
                    new FindNewShowsInDownloadFolders(this).Check((scanProgDlg == null) ? noProgress : scanProgDlg.AddNewProg, 0, 50, specific, settings);
                    new FindNewShowsInLibrary(this).Check((scanProgDlg == null) ? noProgress : scanProgDlg.AddNewProg, 50, 100, specific, settings);
                }
                
                new CheckShows(this).Check((scanProgDlg == null) ? noProgress : scanProgDlg.MediaLibProg, specific, settings);
                new CleanDownloadDirectory(this).Check((scanProgDlg == null) ? noProgress : scanProgDlg.DownloadFolderProg, specific, settings);
                localFinders.Check((scanProgDlg == null) ? noProgress : scanProgDlg.LocalSearchProg, specific, settings);
                downloadFinders.Check((scanProgDlg == null) ? noProgress : scanProgDlg.DownloadingProg, specific, settings);
                searchFinders.Check((scanProgDlg == null)? noProgress : scanProgDlg.ToBeDownloadedProg, specific, settings);

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
            catch (TVRenameOperationInteruptedException)
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

        private void NoProgress(int pct, string message)
        {
            //Nothing to do - Method is called if we have no UI
        }

        public static bool MatchesSequentialNumber(string filename, ref int seas, ref int ep, ProcessedEpisode pe)
        {
            if (pe.OverallNumber == -1)
                return false;

            string num = pe.OverallNumber.ToString();

            bool found = Regex.Match("X" + filename + "X", "[^0-9]0*" + num + "[^0-9]")
                .Success; // need to pad to let it match non-numbers at start and end

            if (found)
            {
                seas = pe.AppropriateSeasonNumber;
                ep = pe.AppropriateEpNum;
            }

            return found;
        }

        private List<ProcessedEpisode> GetMissingEps()
        {
            int dd = TVSettings.Instance.WTWRecentDays;
            DirFilesCache dfc = new DirFilesCache();
            return GetMissingEps(dfc, Library.GetRecentAndFutureEps(dd));
        }

        private List<ProcessedEpisode> GetMissingEps(DirFilesCache dfc, IEnumerable<ProcessedEpisode> lpe)
        {
            List<ProcessedEpisode> missing = new List<ProcessedEpisode>();

            foreach (ProcessedEpisode pe in lpe)
            {
                List<FileInfo> fl = dfc.FindEpOnDisk(pe);

                bool foundOnDisk = ((fl != null) && (fl.Any()));
                bool alreadyAired;

                DateTime? airDate = pe.GetAirDateDt(true);

                if (airDate.HasValue)
                    alreadyAired = airDate.Value.CompareTo(DateTime.Now) < 0;
                else
                    alreadyAired = true;

                if (!foundOnDisk && alreadyAired && (pe.Show.DoMissingCheck))
                {
                    missing.Add(pe);
                }
            }

            return missing;
        }

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
                if (!Directory.Exists(dirPath)) continue;

                try{ 
                    string[] x = Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories);
                    Logger.Info($"Processing {x.Length} files for shows that need to be scanned");

                    foreach (string filePath in x)
                    {
                        Logger.Info($"Checking to see whether {filePath} is a file that for a show that need scanning");

                        if (!File.Exists(filePath)) continue;

                        FileInfo fi = new FileInfo(filePath);

                        if (fi.IgnoreFile()) continue;

                        foreach (ShowItem si in Library.Shows)
                        {
                            if (showsToScan.Contains(si)) continue;

                            if (si.NameMatch(fi, TVSettings.Instance.UseFullPathNameToMatchSearchFolders))
                            {
                                showsToScan.Add(si);
                            }
                        }
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logger.Warn($"Could not access files in {dirPath} {ex.Message}");
                }
                catch (DirectoryNotFoundException  ex)
                {
                    Logger.Warn($"Could not access files in {dirPath} {ex.Message}");
                }
                catch (IOException ex)
                {
                    Logger.Warn($"Could not access files in {dirPath} {ex.Message}");
                }
                catch (NotSupportedException ex)
                {
                    Logger.Error($"Please update 'Download Folders' {dirPath} is not supported {ex.Message}");
                }
                try { 
                    string[] directories = Directory.GetDirectories(dirPath, "*", SearchOption.AllDirectories);
                    Logger.Info($"Processing {directories.Length} directories for shows that need to be scanned");

                    foreach (string subDirPath in directories)
                    {
                        Logger.Info($"Checking to see whether {subDirPath} has any shows that need scanning");

                        if (!Directory.Exists(subDirPath)) continue;

                        DirectoryInfo di = new DirectoryInfo(subDirPath);

                        foreach (ShowItem si in Library.Values)
                        {
                            if (showsToScan.Contains(si)) continue;

                            if (si.NameMatch(di,TVSettings.Instance.UseFullPathNameToMatchSearchFolders))
                            {
                                showsToScan.Add(si);
                            }
                        }
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logger.Warn($"Could not access sub-directories in {dirPath} {ex.Message}");
                }
                catch (DirectoryNotFoundException ex)
                {
                    Logger.Warn($"Could not access sub-directories in {dirPath} {ex.Message}");
                }
                catch (IOException ex)
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

        internal void ForceRefresh(List<ShowItem> sis, bool unattended)
        {
            PreventAutoScan("Force Refresh");
            if (sis != null)
            {
                foreach (ShowItem si in sis)
                {
                    TheTVDB.Instance.ForgetShow(si.TvdbCode, true,si.UseCustomLanguage,si.CustomLanguageCode);
                }
            }

            DoDownloadsFG(unattended);
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
                if (scanProgDlg != null) scanProgDlg.Dispose();

                // ReSharper disable once UseNullPropagation
                if (cacheManager != null) cacheManager.Dispose();
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
    }
}
