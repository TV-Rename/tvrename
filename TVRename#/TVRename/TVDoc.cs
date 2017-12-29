// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
// "Doc" is short for "Document", from the "Document" and "View" model way of thinking of things.
// All the processing and work should be done in here, nothing in UI.cs
// Means we can run TVRename and do useful stuff, without showing any UI. (i.e. text mode / console app)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using NLog;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using File = Alphaleonis.Win32.Filesystem.File;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using Path = System.IO.Path;

namespace TVRename
{
    public class TVDoc
    {
        public List<ShowItem> ShowItems;
        public List<string> MonitorFolders;
        public List<string> IgnoreFolders;
        public List<string> SearchFolders;
        public List<IgnoreItem> Ignore;
        private readonly DownloadIdentifiersController _downloadIdentifiers;

        public bool ActionCancel;
        public bool ActionPause;
        private Thread _actionProcessorThread;
        private Semaphore[] _actionSemaphores;
        private bool _actionStarting;
        private List<Thread> _actionWorkers;
        public FolderMonitorEntryList AddItems;
        public CommandLineArgs Args;

        public bool DownloadDone;
        private bool _downloadOk;
        public int DownloadPct;
        public bool DownloadStopOnError;
        public int DownloadsRemaining;
        public string LoadErr;
        public bool LoadOk;
        public ScanProgress ScanProgDlg;
        public ItemList TheActionList;
        public Semaphore WorkerSemaphore;
        public List<Thread> Workers;
        private bool _mDirty;
        private Thread _mDownloaderThread;
        private TVRenameStats _mStats;
        public bool CurrentlyBusy;  // This is set to true when scanning and indicates to other objects not to commence a scan of their own

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static readonly Logger _threadslogger = LogManager.GetLogger("threads");

        private readonly List<Finder> _finders;

        readonly string[] _seasonWords = { "Season", // EN
                                       "Saison", // FR, DE
                                       "temporada" // ES
                                       }; // TODO: move into settings, and allow user to edit these

        public List<String> GetGenres()
        {
            List<String> allGenres = new List<string>();
            foreach (ShowItem si in ShowItems)
            {
                if (si.Genres != null) allGenres.AddRange(si.Genres);
            }
            List<String> distinctGenres = allGenres.Distinct().ToList();
            distinctGenres.Sort();
            return distinctGenres;
        }

        public List<String> GetStatuses()
        {
            List<String> allStatuses = new List<string>();
            foreach (ShowItem si in ShowItems)
            {
                if (si.ShowStatus != null) allStatuses.Add(si.ShowStatus);
            }
            List<String> distinctStatuses = allStatuses.Distinct().ToList();
            distinctStatuses.Sort();
            return distinctStatuses;
        }

        public List<String> GetNetworks()
        {
            List<String> allValues = new List<string>();
            foreach (ShowItem si in ShowItems)
            {
                if (si.TheSeries()?.GetNetwork() != null) allValues.Add(si.TheSeries().GetNetwork());
            }
            List<String> distinctValues = allValues.Distinct().ToList();
            distinctValues.Sort();
            return distinctValues;
        }

        public List<String> GetRatings()
        {
            List<String> allValues = new List<string>();
            foreach (ShowItem si in ShowItems)
            {
                if (si.TheSeries()?.GetRating() != null) allValues.Add(si.TheSeries().GetRating());
            }
            List<String> distinctValues = allValues.Distinct().ToList();
            distinctValues.Sort();
            return distinctValues;
        }


        public int GetMinYear() => ShowItems.Min(si => Convert.ToInt32(si.TheSeries().GetYear()));

        public int GetMaxYear() => ShowItems.Max(si => Convert.ToInt32(si.TheSeries().GetYear()));


        public TVDoc(FileInfo settingsFile, CommandLineArgs args)
        {
            Args = args;

            Ignore = new List<IgnoreItem>();

            Workers = null;
            WorkerSemaphore = null;

            _mStats = new TVRenameStats();
            _mDirty = false;
            TheActionList = new ItemList();

            MonitorFolders = new List<String>();
            IgnoreFolders = new List<String>();
            SearchFolders = new List<String>();

            ShowItems = new List<ShowItem>();
            AddItems = new FolderMonitorEntryList();

            DownloadDone = true;
            _downloadOk = true;

            ActionCancel = false;
            ScanProgDlg = null;

            _finders = new List<Finder>();
            _finders.Add(new FileFinder(this));
            _finders.Add(new RssFinder(this));
            _finders.Add(new UTorrentFinder(this));
            _finders.Add(new SaBnzbdFinder(this));


            LoadOk = ((settingsFile == null) || LoadXMLSettings(settingsFile)) && TheTVDB.Instance.LoadOk;

            _downloadIdentifiers = new DownloadIdentifiersController();
            UpdateTVDBLanguage();

            //    StartServer();
        }

        public void UpdateTVDBLanguage()
        {
            TheTVDB.Instance.RequestLanguage = TVSettings.Instance.PreferredLanguage;
        }

        ~TVDoc()
        {
            StopBgDownloadThread();
        }

        private void LockShowItems()
        {
            /*#if DEBUG
                             System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1);
                             System.Diagnostics.StackFrame sf = st.GetFrame(0);
                             string msg = sf.GetMethod().DeclaringType.FullName + "::" + sf.GetMethod().Name;
                            logger.Info("LockShowItems " + msg);
            #endif
                             Monitor.Enter(ShowItems);
                    */
        }

        public void UnlockShowItems()
        {
            /*
    #if DEBUG
                    System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1);
                    System.Diagnostics.StackFrame sf = st.GetFrame(0);
                    string msg = sf.GetMethod().DeclaringType.FullName + "::" + sf.GetMethod().Name;
                   logger.Info("UnlockShowItems " + msg);
    #endif

                    Monitor.Exit(ShowItems);
             */
        }

        public TVRenameStats Stats()
        {
            _mStats.NsNumberOfShows = ShowItems.Count;
            _mStats.NsNumberOfSeasons = 0;
            _mStats.NsNumberOfEpisodesExpected = 0;

            LockShowItems();
            foreach (ShowItem si in ShowItems)
            {
                foreach (KeyValuePair<int, List<ProcessedEpisode>> k in si.SeasonEpisodes)
                    _mStats.NsNumberOfEpisodesExpected += k.Value.Count;
                _mStats.NsNumberOfSeasons += si.SeasonEpisodes.Count;
            }
            UnlockShowItems();

            return _mStats;
        }

        public void SetDirty()
        {
            _mDirty = true;
        }

        public bool Dirty()
        {
            return _mDirty;
        }

        public List<ShowItem> GetShowItems(bool lockThem)
        {
            if (lockThem)
                LockShowItems();

            ShowItems.Sort(ShowItem.CompareShowItemNames);
            return ShowItems;
        }

        public ShowItem GetShowItem(int id)
        {
            LockShowItems();
            foreach (ShowItem si in ShowItems)
            {
                if (si.TVDBCode == id)
                {
                    UnlockShowItems();
                    return si;
                }
            }
            UnlockShowItems();
            return null;
        }

        //			void WebServer()
        //			{
        //			TVRenameServer ^serve = gcnew TVRenameServer(this); // all work is done in constructor which never returns
        //			}
        //			void StartServer()
        //			{
        //			mServerThread = gcnew Thread(gcnew ThreadStart(this, &TVDoc::WebServer));
        //			mServerThread->Name = "Web Server";
        //			mServerThread->Start();
        //			}
        //			void StopServer()
        //			{
        //			if (mServerThread != nullptr)
        //			{
        //			mServerThread->Abort();
        //			mServerThread = nullptr;
        //
        //			}
        //			}
        //			

        public void SetSearcher(int n)
        {
            TVSettings.Instance.TheSearchers.SetToNumber(n);
            SetDirty();
        }

        public bool MonitorFolderHasSeasonFolders(DirectoryInfo di, out string folderName)
        {
            try
            {
                // keep in sync with ProcessAddItems, etc.
                foreach (string sw in _seasonWords)
                {
                    DirectoryInfo[] di2 = di.GetDirectories("*" + sw + " *");
                    if (di2.Length == 0)
                        continue;

                    folderName = sw;
                    return true;
                }
            }
            catch (UnauthorizedAccessException uae)
            {
                // e.g. recycle bin, system volume information
                _logger.Warn(uae, "Could not access {0} (or a subdir), may not be an issue as could be expected e.g. recycle bin, system volume information",di.FullName);
            }
 

            folderName = null;
            return false;
        }

        public bool MonitorAddSingleFolder(DirectoryInfo di2, bool andGuess)
        {
            // ..and not already a folder for one of our shows
            string theFolder = di2.FullName.ToLower();
            bool alreadyHaveIt = false;
            foreach (ShowItem si in ShowItems)
            {
                if (si.AutoAddNewSeasons && !string.IsNullOrEmpty(si.AutoAddFolderBase) && FileHelper.FolderIsSubfolderOf(theFolder, si.AutoAddFolderBase))
                {
                    // we're looking at a folder that is a subfolder of an existing show
                    alreadyHaveIt = true;
                    break;
                }

                Dictionary<int, List<string>> afl = si.AllFolderLocations();
                foreach (KeyValuePair<int, List<string>> kvp in afl)
                {
                    foreach (string folder in kvp.Value)
                    {
                        if (theFolder.ToLower() != folder.ToLower())
                            continue;

                        alreadyHaveIt = true;
                        break;
                    }
                }

                if (alreadyHaveIt)
                    break;
            } // for each showitem

            bool hasSeasonFolders = false;
            try
            {
                hasSeasonFolders = MonitorFolderHasSeasonFolders(di2, out string folderName);
                bool hasSubFolders = di2.GetDirectories().Length > 0;
                if (!alreadyHaveIt && (!hasSubFolders || hasSeasonFolders))
                {
                    // ....its good!
                    FolderMonitorEntry ai = new FolderMonitorEntry(di2.FullName, hasSeasonFolders, folderName);
                    AddItems.Add(ai);
                    if (andGuess)
                        MonitorGuessShowItem(ai);
                }

            }
            catch (UnauthorizedAccessException)
            {
                alreadyHaveIt = true;
            }

            return hasSeasonFolders || alreadyHaveIt;
        }

        public void MonitorCheckFolderRecursive(DirectoryInfo di, ref bool stop)
        {
            // is it on the folder monitor ignore list?
            if (IgnoreFolders.Contains(di.FullName.ToLower()))
                return;

            if (MonitorAddSingleFolder(di, false))
                return; // done.

            // recursively check a monitored folder for new shows

            foreach (DirectoryInfo di2 in di.GetDirectories())
            {
                if (stop)
                    return;

                MonitorCheckFolderRecursive(di2, ref stop); // not a season folder.. recurse!
            } // for each directory
        }

        public void MonitorAddAllToMyShows()
        {
            LockShowItems();

            foreach (FolderMonitorEntry ai in AddItems)
            {
                if (ai.CodeUnknown)
                    continue; // skip

                // see if there is a matching show item
                ShowItem found = ShowItemForCode(ai.TVDBCode);
                if (found == null)
                {
                    // need to add a new showitem
                    found = new ShowItem(ai.TVDBCode);
                    ShowItems.Add(found);
                }

                found.AutoAddFolderBase = ai.Folder;
                found.AutoAddFolderPerSeason = ai.HasSeasonFoldersGuess;

                found.AutoAddSeasonFolderName = ai.SeasonFolderName + " ";
                Stats().AutoAddedShows++;
            }

            GenDict();
            Dirty();
            AddItems.Clear();
            UnlockShowItems();
        }

        public void MonitorGuessShowItem(FolderMonitorEntry ai)
        {
            string showName = GuessShowName(ai);

            if (string.IsNullOrEmpty(showName))
                return;

            TheTVDB.Instance.GetLock("MonitorGuessShowItem");

            SeriesInfo ser = TheTVDB.Instance.FindSeriesForName(showName);
            if (ser != null)
                ai.TVDBCode = ser.TVDBCode;

            TheTVDB.Instance.Unlock("MonitorGuessShowItem");
        }

        public void MonitorCheckFolders(ref bool stop, ref int percentDone)
        {
            // Check the monitored folder list, and build up a new "AddItems" list.
            // guessing what the shows actually are isn't done here.  That is done by
            // calls to "MonitorGuessShowItem"

            AddItems = new FolderMonitorEntryList();

            int c = MonitorFolders.Count;

            LockShowItems();
            int c2 = 0;
            foreach (string folder in MonitorFolders)
            {
                percentDone = 100 * c2++ / c;
                DirectoryInfo di = new DirectoryInfo(folder);
                if (!di.Exists)
                    continue;

                MonitorCheckFolderRecursive(di, ref stop);

                if (stop)
                    break;
            }

            UnlockShowItems();
        }

        public bool RenameFilesToMatchTorrent(string torrent, string folder, TreeView tvTree, SetProgressDelegate prog, bool copyNotMove, string copyDest, CommandLineArgs args)
        {
            if (string.IsNullOrEmpty(folder))
                return false;
            if (string.IsNullOrEmpty(torrent))
                return false;

            if (copyNotMove)
            {
                if (string.IsNullOrEmpty(copyDest))
                    return false;
                if (!Directory.Exists(copyDest))
                    return false;
            }

            Stats().TorrentsMatched++;

            BtFileRenamer btp = new BtFileRenamer(prog);
            ItemList newList = new ItemList();
            bool r = btp.RenameFilesOnDiskToMatchTorrent(torrent, folder, tvTree, newList, copyNotMove, copyDest, args);

            foreach (ITem i in newList)
                TheActionList.Add(i);

            return r;
        }

        // -----------------------------------------------------------------------------

        public void GetThread(Object codeIn)
        {
            Debug.Assert(WorkerSemaphore != null);

            WorkerSemaphore.WaitOne(); // don't start until we're allowed to

            int code = (int)(codeIn);

            bool bannersToo = TVSettings.Instance.NeedToDownloadBannerFile();

            _threadslogger.Trace("  Downloading " + code);
            bool r = TheTVDB.Instance.EnsureUpdated(code, bannersToo);
            _threadslogger.Trace("  Finished " + code);
            if (!r)
            {
                _downloadOk = false;
                if (DownloadStopOnError)
                    DownloadDone = true;
            }
            WorkerSemaphore.Release(1);
        }

        public void WaitForAllThreadsAndTidyUp()
        {
            if (Workers != null)
            {
                foreach (Thread t in Workers)
                {
                    if (t.IsAlive)
                        t.Join();
                }
            }

            Workers = null;
            WorkerSemaphore = null;
        }

        public void Downloader()
        {
            // do background downloads of webpages
            _logger.Info("*******************************");
            _logger.Info("Starting Background Download...");
            try
            {
                if (ShowItems.Count == 0)
                {
                    DownloadDone = true;
                    _downloadOk = true;
                    return;
                }

                if (!TheTVDB.Instance.GetUpdates())
                {
                    DownloadDone = true;
                    _downloadOk = false;
                    return;
                }

                // for eachs of the ShowItems, make sure we've got downloaded data for it

                int totalItems = ShowItems.Count;
                int n = 0;
                List<int> codes = new List<int>();
                LockShowItems();
                foreach (ShowItem si in ShowItems)
                    codes.Add(si.TVDBCode);
                UnlockShowItems();

                
                int numWorkers = TVSettings.Instance.ParallelDownloads;
                _logger.Info("Setting up {0} threads to download information from TVDB.com",numWorkers);
                Workers = new List<Thread>();

                WorkerSemaphore = new Semaphore(numWorkers, numWorkers); // allow up to numWorkers working at once

                foreach (int code in codes)
                {
                    DownloadPct = 100 * (n + 1) / (totalItems + 1);
                    DownloadsRemaining = totalItems - n;
                    n++;

                    WorkerSemaphore.WaitOne(); // blocks until there is an available slot
                    Thread t = new Thread(GetThread);
                    Workers.Add(t);
                    t.Name = "GetThread:" + code;
                    t.Start(code); // will grab the semaphore as soon as we make it available
                    int nfr = WorkerSemaphore.Release(1); // release our hold on the semaphore, so that worker can grab it
                    _threadslogger.Trace("Started " + code + " pool has " + nfr + " free");
                    Thread.Sleep(1); // allow the other thread a chance to run and grab

                    // tidy up any finished workers
                    for (int i = Workers.Count - 1; i >= 0; i--)
                    {
                        if (!Workers[i].IsAlive)
                            Workers.RemoveAt(i); // remove dead worker
                    }

                    if (DownloadDone)
                        break;
                }

                WaitForAllThreadsAndTidyUp();

                TheTVDB.Instance.UpdatesDoneOk();
                DownloadDone = true;
                _downloadOk = true;
            }
            catch (ThreadAbortException)
            {
                DownloadDone = true;
                _downloadOk = false;
            }
            finally
            {
                Workers = null;
                WorkerSemaphore = null;
            }
        }

        public void StartBgDownloadThread(bool stopOnError)
        {
            if (!DownloadDone)
                return;
            DownloadStopOnError = stopOnError;
            DownloadPct = 0;
            DownloadDone = false;
            _downloadOk = true;
            _mDownloaderThread = new Thread(Downloader);
            _mDownloaderThread.Name = "Downloader";
            _mDownloaderThread.Start();
        }

        public void WaitForBgDownloadDone()
        {
            if ((_mDownloaderThread != null) && (_mDownloaderThread.IsAlive))
                _mDownloaderThread.Join();
            _mDownloaderThread = null;
        }

        public void StopBgDownloadThread()
        {
            if (_mDownloaderThread != null)
            {
                DownloadDone = true;
                _mDownloaderThread.Join();

                /*if (Workers != null)
                {
                    foreach (Thread t in Workers)
                        t.Interrupt();
                }

                WaitForAllThreadsAndTidyUp();
                if (mDownloaderThread.IsAlive)
                {
                    mDownloaderThread.Interrupt();
                    mDownloaderThread = null;
                }
                */
                _mDownloaderThread = null;
            }
        }

        public bool DoDownloadsFg()
        {
            if (TVSettings.Instance.OfflineMode)
                return true; // don't do internet in offline mode!
            _logger.Info("Doing downloads in the foreground...");

            StartBgDownloadThread(true);

            const int delayStep = 100;
            int count = 1000 / delayStep; // one second
            while ((count-- > 0) && (!DownloadDone))
                Thread.Sleep(delayStep);

            if (!DownloadDone && !Args.Hide) // downloading still going on, so time to show the dialog if we're not in /hide mode
            {
                DownloadProgress dp = new DownloadProgress(this);
                dp.ShowDialog();
                dp.Update();
            }

            WaitForBgDownloadDone();

            TheTVDB.Instance.SaveCache();

            GenDict();

            if (!_downloadOk)
            {
                _logger.Warn(TheTVDB.Instance.LastError);
                if ((!Args.Unattended) && (!Args.Hide))
                    MessageBox.Show(TheTVDB.Instance.LastError, "Error while downloading", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TheTVDB.Instance.LastError = "";
            }

            return _downloadOk;
        }

        public bool GenDict()
        {
            bool res = true;
            LockShowItems();
            foreach (ShowItem si in ShowItems)
            {
                if (!GenerateEpisodeDict(si))
                    res = false;
            }
            UnlockShowItems();
            return res;
        }

        public Searchers GetSearchers()
        {
            return TVSettings.Instance.TheSearchers;
        }

        public void TidyTVDB()
        {
            // remove any shows from thetvdb that aren't in My Shows
            TheTVDB.Instance.GetLock("TidyTVDB");
            List<int> removeList = new List<int>();

            foreach (KeyValuePair<int, SeriesInfo> kvp in TheTVDB.Instance.GetSeriesDict())
            {
                bool found = false;
                foreach (ShowItem si in ShowItems)
                {
                    if (si.TVDBCode == kvp.Key)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    removeList.Add(kvp.Key);
            }

            foreach (int i in removeList)
                TheTVDB.Instance.ForgetShow(i, false);

            TheTVDB.Instance.Unlock("TheTVDB");
            TheTVDB.Instance.SaveCache();
        }

        public void Closing()
        {
            StopBgDownloadThread();
            Stats().Save();
        }

        public void DoBtSearch(ProcessedEpisode ep)
        {
            if (ep == null)
                return;
            Helpers.SysOpen(TVSettings.Instance.BtSearchUrl(ep));
        }

        public void DoWhenToWatch(bool cachedOnly)
        {
            if (!cachedOnly && !DoDownloadsFg())
                return;
            if (cachedOnly)
                GenDict();
        }

        public List<FileInfo> FindEpOnDisk(DirFilesCache dfc, ProcessedEpisode pe)
        {
            return FindEpOnDisk(dfc, pe.Si, pe);
        }

        public List<FileInfo> FindEpOnDisk(DirFilesCache dfc, ShowItem si, Episode epi)
        {
            if (dfc == null)
                dfc = new DirFilesCache();
            List<FileInfo> ret = new List<FileInfo>();

            int seasWanted = epi.TheSeason.SeasonNumber;
            int epWanted = epi.EpNum;

            int snum = epi.TheSeason.SeasonNumber;

            if (!si.AllFolderLocations().ContainsKey(snum))
                return ret;

            foreach (string folder in si.AllFolderLocations()[snum])
            {
                FileInfo[] files = dfc.Get(folder);
                if (files == null)
                    continue;

                foreach (FileInfo fiTemp in files)
                {
                    int seasFound;
                    int epFound;

                    if (!TVSettings.Instance.UsefulExtension(fiTemp.Extension, false))
                        continue; // move on

                    if (FindSeasEp(fiTemp, out seasFound, out epFound, si))
                    {
                        if (seasFound == -1)
                            seasFound = seasWanted;
                        if ((seasFound == seasWanted) && (epFound == epWanted))
                            ret.Add(fiTemp);
                    }
                }
            }

            return ret;
        }

        public bool HasAnyAirdates(ShowItem si, int snum)
        {
            bool r = false;

            SeriesInfo ser = TheTVDB.Instance.GetSeries(si.TVDBCode);
            if ((ser != null) && (ser.Seasons.ContainsKey(snum)))
            {
                foreach (Episode e in ser.Seasons[snum].Episodes)
                {
                    if (e.FirstAired != null)
                    {
                        r = true;
                        break;
                    }
                }
            }
            return r;
        }

        public bool GenerateEpisodeDict(ShowItem si)
        {
            si.SeasonEpisodes.Clear();

            // copy data from tvdb
            // process as per rules
            // done!

            TheTVDB.Instance.GetLock("GenerateEpisodeDict");

            SeriesInfo ser = TheTVDB.Instance.GetSeries(si.TVDBCode);

            if (ser == null)
            {
                TheTVDB.Instance.Unlock("GenerateEpisodeDict");
                return false; // TODO: warn user
            }

            bool r = true;
            foreach (KeyValuePair<int, Season> kvp in ser.Seasons)
            {
                List<ProcessedEpisode> pel = GenerateEpisodes(si, ser, kvp.Key, true);
                si.SeasonEpisodes[kvp.Key] = pel;
                if (pel == null)
                    r = false;
            }

            List<int> theKeys = new List<int>();
            // now, go through and number them all sequentially
            foreach (int snum in ser.Seasons.Keys)
                theKeys.Add(snum);

            theKeys.Sort();

            int overallCount = 1;
            foreach (int snum in theKeys)
            {
                if (snum != 0)
                {
                    foreach (ProcessedEpisode pe in si.SeasonEpisodes[snum])
                    {
                        pe.OverallNumber = overallCount;
                        overallCount += 1 + pe.EpNum2 - pe.EpNum;
                    }
                }
            }

            TheTVDB.Instance.Unlock("GenerateEpisodeDict");

            return r;
        }

        public static List<ProcessedEpisode> GenerateEpisodes(ShowItem si, SeriesInfo ser, int snum, bool applyRules)
        {
            List<ProcessedEpisode> eis = new List<ProcessedEpisode>();

            if ((ser == null) || !ser.Seasons.ContainsKey(snum))
                return null; // todo.. something?

            Season seas = ser.Seasons[snum];

            if (seas == null)
                return null; // TODO: warn user

            foreach (Episode e in seas.Episodes)
                eis.Add(new ProcessedEpisode(e, si)); // add a copy

            if (si.DvdOrder)
            {
                eis.Sort(ProcessedEpisode.DvdOrderSorter);
                Renumber(eis);
            }
            else
                eis.Sort(ProcessedEpisode.EpNumberSorter);

            if (si.CountSpecials && ser.Seasons.ContainsKey(0))
            {
                // merge specials in
                foreach (Episode ep in ser.Seasons[0].Episodes)
                {
                    if (ep.Items.ContainsKey("airsbefore_season") && ep.Items.ContainsKey("airsbefore_episode"))
                    {
                        string seasstr = ep.Items["airsbefore_season"];
                        string epstr = ep.Items["airsbefore_episode"];
                        if ((string.IsNullOrEmpty(seasstr)) || (string.IsNullOrEmpty(epstr)))
                            continue;
                        int sease = int.Parse(seasstr);
                        if (sease != snum)
                            continue;
                        int epnum = int.Parse(epstr);
                        for (int i = 0; i < eis.Count; i++)
                        {
                            if ((eis[i].SeasonNumber == sease) && (eis[i].EpNum == epnum))
                            {
                                ProcessedEpisode pe = new ProcessedEpisode(ep, si)
                                {
                                    TheSeason = eis[i].TheSeason,
                                    SeasonId = eis[i].SeasonId
                                };
                                eis.Insert(i, pe);
                                break;
                            }
                        }
                    }
                }
                // renumber to allow for specials
                int epnumr = 1;
                for (int j = 0; j < eis.Count; j++)
                {
                    eis[j].EpNum2 = epnumr + (eis[j].EpNum2 - eis[j].EpNum);
                    eis[j].EpNum = epnumr;
                    epnumr++;
                }
            }

            if (applyRules)
            {
                List<ShowRule> rules = si.RulesForSeason(snum);
                if (rules != null)
                    ApplyRules(eis, rules, si);
            }

            return eis;
        }

        public static void ApplyRules(List<ProcessedEpisode> eis, List<ShowRule> rules, ShowItem si)
        {
            foreach (ShowRule sr in rules)
            {
                int nn1 = sr.First;
                int nn2 = sr.Second;

                int n1 = -1;
                int n2 = -1;
                // turn nn1 and nn2 from ep number into position in array
                for (int i = 0; i < eis.Count; i++)
                {
                    if (eis[i].EpNum == nn1)
                    {
                        n1 = i;
                        break;
                    }
                }
                for (int i = 0; i < eis.Count; i++)
                {
                    if (eis[i].EpNum == nn2)
                    {
                        n2 = i;
                        break;
                    }
                }

                if (sr.DoWhatNow == RuleAction.KInsert)
                {
                    // this only applies for inserting an episode, at the end of the list
                    if (nn1 == eis[eis.Count - 1].EpNum + 1) // after the last episode
                        n1 = eis.Count;
                }

                string txt = sr.UserSuppliedText;
                int ec = eis.Count;

                switch (sr.DoWhatNow)
                {
                    case RuleAction.KRename:
                        {
                            if ((n1 < ec) && (n1 >= 0))
                                eis[n1].Name = txt;
                            break;
                        }
                    case RuleAction.KRemove:
                        {
                            if ((n1 < ec) && (n1 >= 0) && (n2 < ec) && (n2 >= 0))
                                eis.RemoveRange(n1, 1 + n2 - n1);
                            else if ((n1 < ec) && (n1 >= 0) && (n2 == -1))
                                eis.RemoveAt(n1);
                            break;
                        }
                    case RuleAction.KIgnoreEp:
                        {
                            if (n2 == -1)
                                n2 = n1;
                            for (int i = n1; i <= n2; i++)
                            {
                                if ((i < ec) && (i >= 0))
                                    eis[i].Ignore = true;
                            }
                            break;
                        }
                    case RuleAction.KSplit:
                        {
                            // split one episode into a multi-parter
                            if ((n1 < ec) && (n1 >= 0))
                            {
                                ProcessedEpisode ei = eis[n1];
                                string nameBase = ei.Name;
                                eis.RemoveAt(n1); // remove old one
                                for (int i = 0; i < nn2; i++) // make n2 new parts
                                {
                                    ProcessedEpisode pe2 = new ProcessedEpisode(ei, si, ProcessedEpisode.ProcessedEpisodeType.Split)
                                    {
                                        Name = nameBase + " (Part " + (i + 1) + ")",
                                        EpNum = -2,
                                        EpNum2 = -2
                                    };
                                    eis.Insert(n1 + i, pe2);
                                }
                            }
                            break;
                        }
                    case RuleAction.KMerge:
                    case RuleAction.KCollapse:
                        {
                            if ((n1 != -1) && (n2 != -1) && (n1 < ec) && (n2 < ec) && (n1 < n2))
                            {
                                ProcessedEpisode oldFirstEi = eis[n1];
                                string combinedName = eis[n1].Name + " + ";
                                string combinedSummary = eis[n1].Overview + "<br/><br/>";
                                List<Episode> alleps = new List<Episode>();
                                alleps.Add(eis[n1]);
                                //int firstNum = eis[n1]->TVcomEpCount();
                                for (int i = n1 + 1; i <= n2; i++)
                                {
                                    combinedName += eis[i].Name;
                                    combinedSummary += eis[i].Overview;
                                    alleps.Add(eis[i]);
                                    if (i != n2)
                                    {
                                        combinedName += " + ";
                                        combinedSummary += "<br/><br/>";
                                    }
                                }

                                eis.RemoveRange(n1, n2 - n1);

                                eis.RemoveAt(n1);

                                ProcessedEpisode pe2 = new ProcessedEpisode(oldFirstEi, si, alleps)
                                {
                                    Name = ((string.IsNullOrEmpty(txt)) ? combinedName : txt),
                                    EpNum = -2
                                };
                                if (sr.DoWhatNow == RuleAction.KMerge)
                                    pe2.EpNum2 = -2 + n2 - n1;
                                else
                                    pe2.EpNum2 = -2;

                                pe2.Overview = combinedSummary;
                                eis.Insert(n1, pe2);
                            }
                            break;
                        }
                    case RuleAction.KSwap:
                        {
                            if ((n1 != -1) && (n2 != -1) && (n1 < ec) && (n2 < ec))
                            {
                                ProcessedEpisode t = eis[n1];
                                eis[n1] = eis[n2];
                                eis[n2] = t;
                            }
                            break;
                        }
                    case RuleAction.KInsert:
                        {
                            if ((n1 < ec) && (n1 >= 0))
                            {
                                ProcessedEpisode t = eis[n1];
                                ProcessedEpisode n = new ProcessedEpisode(t.TheSeries, t.TheSeason, si)
                                {
                                    Name = txt,
                                    EpNum = -2,
                                    EpNum2 = -2
                                };
                                eis.Insert(n1, n);
                            }
                            else if (n1 == eis.Count)
                            {
                                ProcessedEpisode t = eis[n1 - 1];
                                ProcessedEpisode n = new ProcessedEpisode(t.TheSeries, t.TheSeason, si)
                                {
                                    Name = txt,
                                    EpNum = -2,
                                    EpNum2 = -2
                                };
                                eis.Add(n);
                            }
                            break;
                        }
                } // switch DoWhatNow

                Renumber(eis);
            } // for each rule
            // now, go through and remove the ignored ones (but don't renumber!!)
            for (int i = eis.Count - 1; i >= 0; i--)
            {
                if (eis[i].Ignore)
                    eis.RemoveAt(i);
            }
        }

        public static void Renumber(List<ProcessedEpisode> eis)
        {
            if (eis.Count == 0)
                return; // nothing to do

            // renumber 
            // pay attention to specials etc.
            int n = (eis[0].EpNum == 0) ? 0 : 1;

            for (int i = 0; i < eis.Count; i++)
            {
                if (eis[i].EpNum != -1) // is -1 if its a special or other ignored ep
                {
                    int num = eis[i].EpNum2 - eis[i].EpNum;
                    eis[i].EpNum = n;
                    eis[i].EpNum2 = n + num;
                    n += num + 1;
                }
            }
        }

        public string GuessShowName(FolderMonitorEntry ai)
        {
            // see if we can guess a season number and show name, too
            // Assume is blah\blah\blah\show\season X
            string showName = ai.Folder;

            foreach (string seasonWord in _seasonWords)
            {
                string seasonFinder = ".*" + seasonWord + "[ _\\.]+([0-9]+).*"; // todo: don't look for just one season word
                if (Regex.Matches(showName, seasonFinder, RegexOptions.IgnoreCase).Count == 0)
                    continue;

                try
                {
                    // remove season folder from end of the path
                    showName = Regex.Replace(showName, "(.*)\\\\" + seasonFinder, "$1", RegexOptions.IgnoreCase);
                    break;
                }
                catch (ArgumentException)
                {
                }
            }
            // assume last folder element is the show name
            showName = showName.Substring(showName.LastIndexOf(Path.DirectorySeparatorChar.ToString()) + 1);

            return showName;
        }

        public void WriteXMLSettings()
        {
            // backup old settings before writing new ones

            FileHelper.Rotate(PathManager.TVDocSettingsFile.FullName);
            _logger.Info("Saving Settings to {0}", PathManager.TVDocSettingsFile.FullName);

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true
            };
            using (XmlWriter writer = XmlWriter.Create(PathManager.TVDocSettingsFile.FullName, settings))
            {

                writer.WriteStartDocument();
                writer.WriteStartElement("TVRename");

                XMLHelper.WriteAttributeToXML(writer, "Version", "2.1");

                TVSettings.Instance.WriteXML(writer); // <Settings>

                writer.WriteStartElement("MyShows");
                foreach (ShowItem si in ShowItems)
                    si.WriteXMLSettings(writer);
                writer.WriteEndElement(); // myshows

                XMLHelper.WriteStringsToXml(MonitorFolders, writer, "MonitorFolders", "Folder");
                XMLHelper.WriteStringsToXml(IgnoreFolders, writer, "IgnoreFolders", "Folder");
                XMLHelper.WriteStringsToXml(SearchFolders, writer, "FinderSearchFolders", "Folder");

                writer.WriteStartElement("IgnoreItems");
                foreach (IgnoreItem ii in Ignore)
                    ii.Write(writer);
                writer.WriteEndElement(); // IgnoreItems

                writer.WriteEndElement(); // tvrename
                writer.WriteEndDocument();
                writer.Close();
            }

            _mDirty = false;

            Stats().Save();
        }

        public bool LoadXMLSettings(FileInfo from)
        {
            _logger.Info("Loading Settings from {0}", from.FullName);
            if (from == null)
                return true;

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    IgnoreComments = true,
                    IgnoreWhitespace = true
                };

                if (!from.Exists)
                {
                    //LoadErr = from->Name + " : File does not exist";
                    //return false;
                    return true; // that's ok
                }

                XmlReader reader = XmlReader.Create(from.FullName, settings);

                reader.Read();
                if (reader.Name != "xml")
                {
                    LoadErr = from.Name + " : Not a valid XML file";
                    return false;
                }

                reader.Read();

                if (reader.Name != "TVRename")
                {
                    LoadErr = from.Name + " : Not a TVRename settings file";
                    return false;
                }

                if (reader.GetAttribute("Version") != "2.1")
                {
                    LoadErr = from.Name + " : Incompatible version";
                    return false;
                }

                reader.Read(); // move forward one

                while (!reader.EOF)
                {
                    if (reader.Name == "TVRename" && !reader.IsStartElement())
                        break; // end of it all

                    if (reader.Name == "Settings")
                    {
                        TVSettings.Instance.Load(reader.ReadSubtree());
                        reader.Read();
                    }
                    else if (reader.Name == "MyShows")
                    {
                        XmlReader r2 = reader.ReadSubtree();
                        r2.Read();
                        r2.Read();
                        while (!r2.EOF)
                        {
                            if ((r2.Name == "MyShows") && (!r2.IsStartElement()))
                                break;
                            if (r2.Name == "ShowItem")
                            {
                                ShowItem si = new ShowItem(r2.ReadSubtree());

                                if (si.UseCustomShowName) // see if custom show name is actually the real show name
                                {
                                    SeriesInfo ser = si.TheSeries();
                                    if ((ser != null) && (si.CustomShowName == ser.Name))
                                    {
                                        // then, turn it off
                                        si.CustomShowName = "";
                                        si.UseCustomShowName = false;
                                    }
                                }
                                ShowItems.Add(si);

                                r2.Read();
                            }
                            else
                                r2.ReadOuterXml();
                        }
                        reader.Read();
                    }
                    else if (reader.Name == "MonitorFolders")
                        MonitorFolders = XMLHelper.ReadStringsFromXml(reader, "MonitorFolders", "Folder");
                    else if (reader.Name == "IgnoreFolders")
                        IgnoreFolders = XMLHelper.ReadStringsFromXml(reader, "IgnoreFolders", "Folder");
                    else if (reader.Name == "FinderSearchFolders")
                        SearchFolders = XMLHelper.ReadStringsFromXml(reader, "FinderSearchFolders", "Folder");
                    else if (reader.Name == "IgnoreItems")
                    {
                        XmlReader r2 = reader.ReadSubtree();
                        r2.Read();
                        r2.Read();
                        while (r2.Name == "Ignore")
                            Ignore.Add(new IgnoreItem(r2));
                        reader.Read();
                    }
                    else
                        reader.ReadOuterXml();
                }

                reader.Close();
            }
            catch (Exception e)
            {
                LoadErr = from.Name + " : " + e.Message;
                return false;
            }

            try
            {
                _mStats = TVRenameStats.Load();
            }
            catch (Exception)
            {
                // not worried if stats loading fails
            }

            //Set these on the settigns object so others can read them too - iealy shuld be refactored into the settings code
            TVSettings.Instance.MonitorFoldersNames = MonitorFolders;
            TVSettings.Instance.IgnoreFoldersNames = IgnoreFolders;
            TVSettings.Instance.SearchFoldersNames = SearchFolders;

            return true;
        }

        public void ExportMissingXML()
        {
            MissingXML mx = new MissingXML();
            mx.Run(TheActionList);
        }

        public void ExportShowInfo()
        {
            ShowsTxt mx = new ShowsTxt();
            mx.Run(ShowItems);
        }

        public List<ProcessedEpisode> NextNShows(int nShows, int nDaysPast, int nDaysFuture)
        {
            DateTime notBefore = DateTime.Now.AddDays(-nDaysPast);
            List<ProcessedEpisode> found = new List<ProcessedEpisode>();

            LockShowItems();
            for (int i = 0; i < nShows; i++)
            {
                ProcessedEpisode nextAfterThat = null;
                TimeSpan howClose = TimeSpan.MaxValue;
                foreach (ShowItem si in GetShowItems(false))
                {
                    if (!si.ShowNextAirdate)
                        continue;
                    foreach (KeyValuePair<int, List<ProcessedEpisode>> v in si.SeasonEpisodes)
                    {
                        if (si.IgnoreSeasons.Contains(v.Key))
                            continue; // ignore this season

                        foreach (ProcessedEpisode ei in v.Value)
                        {
                            if (found.Contains(ei))
                                continue;

                            DateTime? airdt = ei.GetAirDateDt(true);

                            if ((airdt == null) || (airdt == DateTime.MaxValue))
                                continue;
                            DateTime dt = (DateTime)airdt;

                            TimeSpan ts = dt.Subtract(notBefore);
                            TimeSpan timeUntil = dt.Subtract(DateTime.Now);
                            if (((howClose == TimeSpan.MaxValue) || (ts.CompareTo(howClose) <= 0) && (ts.TotalHours >= 0)) && (ts.TotalHours >= 0) && (timeUntil.TotalDays <= nDaysFuture))
                            {
                                howClose = ts;
                                nextAfterThat = ei;
                            }
                        }
                    }
                }
                if (nextAfterThat == null)
                    return found;

                DateTime? nextdt = nextAfterThat.GetAirDateDt(true);
                if (nextdt.HasValue)
                {
                    notBefore = nextdt.Value;
                    found.Add(nextAfterThat);
                }
            }
            UnlockShowItems();

            return found;
        }


        public void WriteUpcoming()
        {
            List<UpcomingExporter> lup = new List<UpcomingExporter>();

            lup.Add(new UpcomingRss(this));
            lup.Add(new UpcomingXML(this));

            foreach (UpcomingExporter ue in lup)
            {
                if (ue.Active()) { ue.Run(); }
            }
        }

        public void ProcessSingleAction(Object infoIn)
        {
            ProcessActionInfo info = infoIn as ProcessActionInfo;
            if (info == null)
                return;

            _actionSemaphores[info.SemaphoreNumber].WaitOne(); // don't start until we're allowed to
            _actionStarting = false; // let our creator know we're started ok

            IAction action = info.TheAction;
            if (action != null)
            {
                _logger.Trace("Triggering Action: {0} - {1} - {2}", action.Name, action.Produces, action);
                action.Go(ref ActionPause, _mStats);
            }
                

            _actionSemaphores[info.SemaphoreNumber].Release(1);
        }

        public ActionQueue[] ActionProcessorMakeQueues(ScanListItemList theList)
        {
            // Take a single list
            // Return an array of "ActionQueue" items.
            // Each individual queue is processed sequentially, but all the queues run in parallel
            // The lists:
            //     - #0 all the cross filesystem moves, and all copies
            //     - #1 all quick "local" moves
            //     - #2 NFO Generator list
            //     - #3 Downloads (rss torrent, thumbnail, folder.jpg) across Settings.ParallelDownloads lists
            // We can discard any non-action items, as there is nothing to do for them

            ActionQueue[] queues = new ActionQueue[4];
            queues[0] = new ActionQueue("Move/Copy", 1); // cross-filesystem moves (slow ones)
            queues[1] = new ActionQueue("Move/Delete", 1); // local rename/moves
            queues[2] = new ActionQueue("Write Metadata", 4); // writing KODI NFO files, etc.
            queues[3] = new ActionQueue("Download", TVSettings.Instance.ParallelDownloads); // downloading torrents, banners, thumbnails

            foreach (IScanListItem sli in theList)
            {
                IAction action = sli as IAction;

                if (action == null)
                    continue; // skip non-actions

                if ((action is IActionWriteMetadata)) // base interface that all metadata actions are derived from
                    queues[2].Actions.Add(action);
                else if ((action is ActionDownload) || (action is ActionRss))
                    queues[3].Actions.Add(action);
                else if (action is ActionCopyMoveRename)
                    queues[(action as ActionCopyMoveRename).QuickOperation() ? 1 : 0].Actions.Add(action);
                else if ((action is ActionDeleteFile) || (action is ActionDeleteDirectory))
                    queues[1].Actions.Add(action);
                else
                {
#if DEBUG
                    Debug.Fail("Unknown action type for making processing queue");
#endif
                    _logger.Error("No action type found for {0}, Please follow up with a developer.", action.GetType());
                    queues[3].Actions.Add(action); // put it in this queue by default
                }
            }
            return queues;
        }

        public void ActionProcessor(Object queuesIn)
        {
#if DEBUG
            Debug.Assert(queuesIn is ActionQueue[]);
#endif
            ActionQueue[] queues = queuesIn as ActionQueue[];
            if (queues == null)
                return;

            int n = queues.Length;

            _actionWorkers = new List<Thread>();
            _actionSemaphores = new Semaphore[n];

            for (int i = 0; i < n; i++)
            {
                _actionSemaphores[i] = new Semaphore(queues[i].ParallelLimit, queues[i].ParallelLimit); // allow up to numWorkers working at once
                _logger.Info("Setting up '{0}' worker, with {1} threads in position {2}.", queues[i].Name, queues[i].ParallelLimit, i);
            }
                

            try
            {
                for (; ; )
                {
                    while (ActionPause)
                        Thread.Sleep(100);

                    // look through the list of semaphores to see if there is one waiting for some work to do
                    bool allDone = true;
                    int which = -1;
                    for (int i = 0; i < n; i++)
                    {
                        // something to do in this queue, and semaphore is available
                        if (queues[i].ActionPosition < queues[i].Actions.Count)
                        {
                            allDone = false;
                            if (_actionSemaphores[i].WaitOne(20, false))
                            {
                                which = i;
                                break;
                            }
                        }
                    }
                    if ((which == -1) && (allDone))
                        break; // all done!

                    if (which == -1)
                        continue; // no semaphores available yet, try again for one

                    ActionQueue q = queues[which];
                    IAction act = q.Actions[q.ActionPosition++];

                    if (act == null)
                        continue;

                    if (!act.Done)
                    {
                        Thread t = new Thread(ProcessSingleAction)
                        {
                            Name = "ProcessSingleAction(" + act.Name + ":" + act.ProgressText + ")"
                        };
                        _actionWorkers.Add(t);
                        _actionStarting = true; // set to false in thread after it has the semaphore
                        t.Start(new ProcessActionInfo(which, act));

                        int nfr = _actionSemaphores[which].Release(1); // release our hold on the semaphore, so that worker can grab it
                        _threadslogger.Trace("ActionProcessor[" + which + "] pool has " + nfr + " free");
                    }

                    while (_actionStarting) // wait for thread to get the semaphore
                        Thread.Sleep(10); // allow the other thread a chance to run and grab

                    // tidy up any finished workers
                    for (int i = _actionWorkers.Count - 1; i >= 0; i--)
                    {
                        if (!_actionWorkers[i].IsAlive)
                            _actionWorkers.RemoveAt(i); // remove dead worker
                    }
                }
                WaitForAllActionThreadsAndTidyUp();
            }
            catch (ThreadAbortException)
            {
                foreach (Thread t in _actionWorkers)
                    t.Abort();
                WaitForAllActionThreadsAndTidyUp();
            }
        }

        private void WaitForAllActionThreadsAndTidyUp()
        {
            if (_actionWorkers != null)
            {
                foreach (Thread t in _actionWorkers)
                {
                    if (t.IsAlive)
                        t.Join();
                }
            }

            _actionWorkers = null;
            _actionSemaphores = null;
        }

        public void DoActions(ScanListItemList theList)
        {
            _logger.Info("**********************");
            _logger.Info("Doing Selected Actions....");
            if (theList == null)
                return;

            // Run tasks in parallel (as much as is sensible)

            ActionQueue[] queues = ActionProcessorMakeQueues(theList);
            ActionPause = false;

            // If not /hide, show CopyMoveProgress dialog

            CopyMoveProgress cmp = null;
            if (!Args.Hide)
                cmp = new CopyMoveProgress(this, queues);

            _actionProcessorThread = new Thread(ActionProcessor)
            {
                Name = "ActionProcessorThread"
            };

            _actionProcessorThread.Start(queues);

            if ((cmp != null) && (cmp.ShowDialog() == DialogResult.Cancel))
                _actionProcessorThread.Abort();

            _actionProcessorThread.Join();

            theList.RemoveAll(x => (x is IAction) && (x as IAction).Done && !(x as IAction).Error);

                foreach (IScanListItem sli in theList)
                {
                    if (sli is IAction) {
                        IAction slia = (IAction)sli;
                        _logger.Warn("Failed to complete the following action: {0}, doing {1}. Error was {2}", slia.Name , slia,slia.ErrorText);
                    }
                }


        }

        public bool ListHasMissingItems(ItemList l)
        {
            foreach (ITem i in l)
            {
                if (i is ItemMissing)
                    return true;
            }
            return false;
        }

        public void ActionGo(List<ShowItem> shows)
        {
            CurrentlyBusy = true;
            if (TVSettings.Instance.MissingCheck && !CheckAllFoldersExist(shows)) // only check for folders existing for missing check
                return;

            if (!DoDownloadsFg())
                return;

            Thread actionWork = new Thread(ScanWorker);
            actionWork.Name = "ActionWork";

            ActionCancel = false;
            foreach (Finder f in _finders) { f.Reset(); }

            if (!Args.Hide)
            {
                ScanProgDlg = new ScanProgress(TVSettings.Instance.RenameCheck || TVSettings.Instance.MissingCheck,
                                                    TVSettings.Instance.MissingCheck && TVSettings.Instance.SearchLocally,
                                                    TVSettings.Instance.MissingCheck && (TVSettings.Instance.CheckuTorrent || TVSettings.Instance.CheckSaBnzbd),
                                                    TVSettings.Instance.MissingCheck && TVSettings.Instance.SearchRss);
            }
            else
                ScanProgDlg = null;

            actionWork.Start(shows);

            if ((ScanProgDlg != null) && (ScanProgDlg.ShowDialog() == DialogResult.Cancel))
            {
                ActionCancel = true;
                actionWork.Interrupt();
                foreach (Finder f in _finders) { f.Interrupt(); }
            }
            else
                actionWork.Join();

            ScanProgDlg = null;

            _downloadIdentifiers.Reset();

            CurrentlyBusy = false;
        }

        public void DoAllActions()
        {

            ScanListItemList theList = new ScanListItemList();

            foreach (ITem action in TheActionList)
            {
                if (action is IScanListItem)
                {
                    theList.Add((IScanListItem)(action));

                }
            }

            DoActions(theList);
        }

        // ReSharper disable once UnusedMember.Local for this method. It is part of some work to try and identify double part episodes automatically
        private void FindDoubleEps()
        {
            StringBuilder output = new StringBuilder();

            foreach (ShowItem si in ShowItems)
            {
                foreach (KeyValuePair<int, List<ProcessedEpisode>> kvp in si.SeasonEpisodes)
                {

                    //Ignore specials seasons
                    if (kvp.Key == 0) continue;

                    //Ignore seasons that all aired on same date
                    DateTime? seasonMinAirDate = (from pep in kvp.Value select pep.FirstAired).Min();
                    DateTime? seasonMaxAirDate = (from pep in kvp.Value select pep.FirstAired).Max();
                    if ((seasonMaxAirDate.HasValue) && seasonMinAirDate.HasValue && seasonMaxAirDate == seasonMinAirDate)
                        continue;

                    //Search through each pair of episodes for the same season
                    foreach (ProcessedEpisode pep in kvp.Value)
                    {
                        foreach (ProcessedEpisode comparePep in kvp.Value)
                        {
                            if (pep.FirstAired.HasValue && comparePep.FirstAired.HasValue && pep.FirstAired == comparePep.FirstAired && pep.EpisodeId < comparePep.EpisodeId)
                            {
                                output.AppendLine(si.ShowName + " - Season: " + kvp.Key + " - " + pep.FirstAired + " - " + pep.EpNum + " - " + comparePep.EpNum);
                            }
                        }
                    }
                }
            }
            
           _logger.Info(output.ToString());
        }
        public void QuickScan() => QuickScan(true, true);

        public void QuickScan(bool doMissingRecents, bool doFilesInDownloadDir)
        {

            CurrentlyBusy = true;

            List<ShowItem> showsToScan = new List<ShowItem>();
            if (doFilesInDownloadDir) showsToScan = GetShowsThatHaveDownloads();

            if (doMissingRecents)
            {
                List<ProcessedEpisode> lpe = GetMissingEps();
                if (lpe != null)
                {
                    foreach (ProcessedEpisode pe in lpe)
                    {
                        if (!showsToScan.Contains(pe.Si)) showsToScan.Add(pe.Si);
                    }
                }
            }


            ActionGo(showsToScan);

            CurrentlyBusy = false;


        }

        public bool CheckAllFoldersExist(List<ShowItem> showlist)
        {
            // show MissingFolderAction for any folders that are missing
            // return false if user cancels

            LockShowItems();

            if (showlist == null) // nothing specified?
                showlist = ShowItems; // everything

            foreach (ShowItem si in showlist)
            {
                if (!si.DoMissingCheck && !si.DoRename)
                    continue; // skip

                Dictionary<int, List<string>> flocs = si.AllFolderLocations();

                int[] numbers = new int[si.SeasonEpisodes.Keys.Count];
                si.SeasonEpisodes.Keys.CopyTo(numbers, 0);
                foreach (int snum in numbers)
                {
                    if (si.IgnoreSeasons.Contains(snum))
                        continue; // ignore this season

                    //int snum = kvp->Key;
                    if ((snum == 0) && (si.CountSpecials))
                        continue; // no specials season, they're merged into the seasons themselves

                    List<string> folders = new List<String>();

                    if (flocs.ContainsKey(snum))
                        folders = flocs[snum];

                    if ((folders.Count == 0) && (!si.AutoAddNewSeasons))
                        continue; // no folders defined or found, autoadd off, so onto the next

                    if (folders.Count == 0)
                    {
                        // no folders defined for this season, and autoadd didn't find any, so suggest the autoadd folder name instead
                        folders.Add(si.AutoFolderNameForSeason(snum));
                    }

                    foreach (string folderFe in folders)
                    {
                        String folder = folderFe;

                        // generate new filename info
                        bool goAgain;
                        DirectoryInfo di = null;
                        bool firstAttempt = true;
                        do
                        {
                            goAgain = false;
                            if (!string.IsNullOrEmpty(folder))
                            {
                                try
                                {
                                    di = new DirectoryInfo(folder);
                                }
                                catch
                                {
                                    break;
                                }
                            }
                            if ((di == null) || (!di.Exists))
                            {
                                string sn = si.ShowName;
                                string text = snum + " of " + si.MaxSeason();
                                string theFolder = folder;
                                string otherFolder = null;

                                FaResult whatToDo = FaResult.KfaNotSet;

                                if (Args.MissingFolder == CommandLineArgs.MissingFolderBehaviour.Create)
                                    whatToDo = FaResult.KfaCreate;
                                else if (Args.MissingFolder == CommandLineArgs.MissingFolderBehaviour.Ignore)
                                    whatToDo = FaResult.KfaIgnoreOnce;

                                if (Args.Hide && (whatToDo == FaResult.KfaNotSet))
                                    whatToDo = FaResult.KfaIgnoreOnce; // default in /hide mode is to ignore

                                if (TVSettings.Instance.AutoCreateFolders && firstAttempt)
                                {
                                    whatToDo = FaResult.KfaCreate;
                                    firstAttempt = false;
                                }


                                if (whatToDo == FaResult.KfaNotSet)
                                {
                                    // no command line guidance, so ask the user
                                    // 									MissingFolderAction ^mfa = gcnew MissingFolderAction(sn, text, theFolder);
                                    // 									mfa->ShowDialog();
                                    // 									whatToDo = mfa->Result;
                                    // 									otherFolder = mfa->FolderName;

                                    MissingFolderAction mfa = new MissingFolderAction(sn, text, theFolder);
                                    mfa.ShowDialog();
                                    whatToDo = mfa.Result;
                                    otherFolder = mfa.FolderName;
                                }

                                if (whatToDo == FaResult.KfaCancel)
                                {
                                    UnlockShowItems();
                                    return false;
                                }

                                if (whatToDo == FaResult.KfaCreate)
                                {
                                    try
                                    {
                                        Directory.CreateDirectory(folder);
                                        _logger.Info("Creating directory as it is missing: {0}",folder);
                                    }
                                    catch (IOException ioe)
                                    {
                                        _logger.Info("Could not directory: {0}", folder);
                                        _logger.Info(ioe);
                                    }
                                    goAgain = true;


                                }
                                else if (whatToDo == FaResult.KfaIgnoreAlways)
                                {
                                    si.IgnoreSeasons.Add(snum);
                                    SetDirty();
                                    break;
                                }
                                else if (whatToDo == FaResult.KfaIgnoreOnce)
                                    break;
                                else if (whatToDo == FaResult.KfaRetry)
                                    goAgain = true;
                                else if (whatToDo == FaResult.KfaDifferentFolder)
                                {
                                    folder = otherFolder;
                                    di = new DirectoryInfo(folder);
                                    goAgain = !di.Exists;
                                    if (di.Exists && (si.AutoFolderNameForSeason(snum).ToLower() != folder.ToLower()))
                                    {
                                        if (!si.ManualFolderLocations.ContainsKey(snum))
                                            si.ManualFolderLocations[snum] = new List<String>();
                                        si.ManualFolderLocations[snum].Add(folder);
                                        SetDirty();
                                    }
                                }
                            }
                        }
                        while (goAgain);
                    } // for each folder
                } // for each snum
            } // for each show
            UnlockShowItems();

            return true;
        }

        public void RemoveIgnored()
        {
            ItemList toRemove = new ItemList();
            foreach (ITem item in TheActionList)
            {
                IScanListItem act = item as IScanListItem;
                foreach (IgnoreItem ii in Ignore)
                {
                    if (ii.SameFileAs(act.Ignore))
                    {
                        toRemove.Add(item);
                        break;
                    }
                }
            }
            foreach (ITem action in toRemove)
                TheActionList.Remove(action);
        }

        public void ForceUpdateImages(ShowItem si)
        {

            TheActionList = new ItemList();
            LockShowItems();

            DirFilesCache dfc = new DirFilesCache();
            _logger.Info("*******************************");
            _logger.Info("Force Update Images: " + si.ShowName);

            if (!string.IsNullOrEmpty(si.AutoAddFolderBase) && (si.AllFolderLocations().Count > 0))
            {
                TheActionList.Add(_downloadIdentifiers.ForceUpdateShow(DownloadIdentifier.DownloadType.DownloadImage, si));
                si.BannersLastUpdatedOnDisk = DateTime.Now;
                SetDirty();
            }

            // process each folder for each season...

            int[] numbers = new int[si.SeasonEpisodes.Keys.Count];
            si.SeasonEpisodes.Keys.CopyTo(numbers, 0);
            Dictionary<int, List<string>> allFolders = si.AllFolderLocations();

            foreach (int snum in numbers)
            {
                if ((si.IgnoreSeasons.Contains(snum)) || (!allFolders.ContainsKey(snum)))
                    continue; // ignore/skip this season

                if ((snum == 0) && (si.CountSpecials))
                    continue; // don't process the specials season, as they're merged into the seasons themselves

                // all the folders for this particular season
                List<string> folders = allFolders[snum];


                List<ProcessedEpisode> eps = si.SeasonEpisodes[snum];

                foreach (string folder in folders)
                {


                    //Image series checks here
                    TheActionList.Add(_downloadIdentifiers.ForceUpdateSeason(DownloadIdentifier.DownloadType.DownloadImage, si, folder, snum));

                }

            } // for each season of this show

            UnlockShowItems();
            RemoveIgnored();

        }

        public void FindUnusedFilesInDlDirectory(List<ShowItem> showList)
        {

            //for each directory in settings directory
            //for each file in directory
            //for each saved show (order by recent)
            //is file aready availabele? 
            //if so add show to list of files to be removed

            DirFilesCache dfc = new DirFilesCache();

            foreach (String dirPath in SearchFolders)
            {
                if (!Directory.Exists(dirPath)) continue;

                foreach (String filePath in Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories))
                {
                    if (!File.Exists(filePath)) continue;

                    FileInfo fi = new FileInfo(filePath);

                    if (!TVSettings.Instance.UsefulExtension(fi.Extension, false))
                        continue; // move on

                    if (TVSettings.Instance.IgnoreSamples && Helpers.Contains(fi.FullName, "sample", StringComparison.OrdinalIgnoreCase) && ((fi.Length / (1024 * 1024)) < TVSettings.Instance.SampleFileMaxSizeMb))
                        continue;

                    List<ShowItem> matchingShows = new List<ShowItem>();

                    foreach (ShowItem si in showList)
                    {
                        if (si.GetSimplifiedPossibleShowNames().Any(name => FileHelper.SimplifyAndCheckFilename(fi.Name, name)))
                            matchingShows.Add(si);
                    }

                    if (matchingShows.Count > 0)
                    {
                        bool fileCanBeRemoved = true;

                        foreach (ShowItem si in matchingShows)
                        {
                            if (FileNeeded(fi, si, dfc)) fileCanBeRemoved = false;
                        }

                        if (fileCanBeRemoved)
                        {
                            int seasF;
                            int epF;

                            ShowItem si = matchingShows[0];//Choose the first series
                            FindSeasEp(fi, out seasF, out epF, si);
                            SeriesInfo s = si.TheSeries();
                            Episode ep = s.GetEpisode(seasF, epF);
                            ProcessedEpisode pep = new ProcessedEpisode(ep, si);
                            TheActionList.Add(new ActionDeleteFile(fi, pep, TVSettings.Instance.Tidyup));
                        }

                    }

                }


                foreach (String subDirPath in Directory.GetDirectories(dirPath, "*", SearchOption.AllDirectories))
                {
                    if (!Directory.Exists(subDirPath)) continue;

                    DirectoryInfo di = new DirectoryInfo(subDirPath);

                    List<ShowItem> matchingShows = new List<ShowItem>();

                    foreach (ShowItem si in showList)
                    {
                        if (si.GetSimplifiedPossibleShowNames().Any(name => FileHelper.SimplifyAndCheckFilename(di.Name, name)))
                            matchingShows.Add(si);
                    }

                    if (matchingShows.Count > 0)
                    {
                        bool dirCanBeRemoved = true;

                        foreach (ShowItem si in matchingShows)
                        {
                            if (FileNeeded(di, si, dfc)) dirCanBeRemoved = false;
                        }

                        if (dirCanBeRemoved)
                        {
                            int seasF;
                            int epF;

                            ShowItem si = matchingShows[0];//Choose the first series
                            FindSeasEp(di, out seasF, out epF, si);
                            SeriesInfo s = si.TheSeries();
                            Episode ep = s.GetEpisode(seasF, epF);
                            ProcessedEpisode pep = new ProcessedEpisode(ep, si);
                            TheActionList.Add(new ActionDeleteDirectory(di, pep, TVSettings.Instance.Tidyup));
                        }

                    }

                }

            }


        }



        public bool FileNeeded(FileInfo fi, ShowItem si, DirFilesCache dfc)
        {
            int seasF;
            int epF;

            if (FindSeasEp(fi, out seasF, out epF, si))
            {
                SeriesInfo s = si.TheSeries();
                try
                {
                    Episode ep = s.GetEpisode(seasF, epF);
                    ProcessedEpisode pep = new ProcessedEpisode(ep, si);

                    if (FindEpOnDisk(dfc, si, pep).Count > 0)
                    {


                        return false;
                    }
                }
                catch (SeriesInfo.EpisodeNotFoundException)
                {
                    //Ignore execption, we may need the file
                    return true;
                }

            }
            //We may need the file
            return true;
        }

        public bool FileNeeded(DirectoryInfo di, ShowItem si, DirFilesCache dfc)
        {
            int seasF;
            int epF;

            if (FindSeasEp(di, out seasF, out epF, si))
            {
                SeriesInfo s = si.TheSeries();
                try
                {
                    Episode ep = s.GetEpisode(seasF, epF);
                    ProcessedEpisode pep = new ProcessedEpisode(ep, si);

                    if (FindEpOnDisk(dfc, si, pep).Count > 0)
                    {


                        return false;
                    }
                }
                catch (SeriesInfo.EpisodeNotFoundException )
                {
                    //Ignore execption, we may need the file
                    return true;
                }

            }
            //We may need the file
            return true;
        }

        public void RenameAndMissingCheck(SetProgressDelegate prog, List<ShowItem> showList)
        {
            TheActionList = new ItemList();

            //int totalEps = 0;

            LockShowItems();

            if (showList == null)
                showList = ShowItems;

            //foreach (ShowItem si in showlist)
            //  if (si.DoRename)
            //    totalEps += si.SeasonEpisodes.Count;

            if (TVSettings.Instance.RenameCheck)
                Stats().RenameChecksDone++;

            if (TVSettings.Instance.MissingCheck)
                Stats().MissingChecksDone++;

            prog.Invoke(0);

            if (showList == null) // only do episode count if we're doing all shows and seasons
                _mStats.NsNumberOfEpisodes = 0;

            DirFilesCache dfc = new DirFilesCache();
            int c = 0;
            foreach (ShowItem si in showList)
            {
                if (ActionCancel)
                    return;

                _logger.Info("Rename and missing check: " + si.ShowName);
                c++;

                prog.Invoke(100 * c / showList.Count);

                if (si.AllFolderLocations().Count == 0) // no folders defined for this show
                    continue; // so, nothing to do.

                //This is the code that will iterate over the DownloadIdentifiers and ask each to ensure that
                //it has all the required files for that show
                if (!string.IsNullOrEmpty(si.AutoAddFolderBase) && (si.AllFolderLocations().Count > 0))
                {
                    TheActionList.Add(_downloadIdentifiers.ProcessShow(si));
                }

                //MS_TODO Put the bannerrefresh period into the settings file, we'll default to 3 months
                DateTime cutOff = DateTime.Now.AddMonths(-3);
                DateTime lastUpdate = si.BannersLastUpdatedOnDisk ?? DateTime.Now.AddMonths(-4);
                bool timeForBannerUpdate = (cutOff.CompareTo(lastUpdate) == 1);

                if (TVSettings.Instance.NeedToDownloadBannerFile() && timeForBannerUpdate)
                {
                    TheActionList.Add(_downloadIdentifiers.ForceUpdateShow(DownloadIdentifier.DownloadType.DownloadImage, si));
                    si.BannersLastUpdatedOnDisk = DateTime.Now;
                    SetDirty();
                }

                // process each folder for each season...

                int[] numbers = new int[si.SeasonEpisodes.Keys.Count];
                si.SeasonEpisodes.Keys.CopyTo(numbers, 0);
                Dictionary<int, List<string>> allFolders = si.AllFolderLocations();

                int lastSeason = 0;
                foreach (int n in numbers)
                    if (n > lastSeason)
                        lastSeason = n;

                foreach (int snum in numbers)
                {
                    if (ActionCancel)
                        return;

                    if ((si.IgnoreSeasons.Contains(snum)) || (!allFolders.ContainsKey(snum)))
                        continue; // ignore/skip this season

                    if ((snum == 0) && (si.CountSpecials))
                        continue; // don't process the specials season, as they're merged into the seasons themselves

                    // all the folders for this particular season
                    List<string> folders = allFolders[snum];

                    bool folderNotDefined = (folders.Count == 0);
                    if (folderNotDefined && (TVSettings.Instance.MissingCheck && !si.AutoAddNewSeasons))
                        continue; // folder for the season is not defined, and we're not auto-adding it

                    List<ProcessedEpisode> eps = si.SeasonEpisodes[snum];
                    int maxEpisodeNumber = 0;
                    foreach (ProcessedEpisode episode in eps)
                    {
                        if (episode.EpNum > maxEpisodeNumber)
                            maxEpisodeNumber = episode.EpNum;
                    }


                    // base folder:
                    if (!string.IsNullOrEmpty(si.AutoAddFolderBase) && (si.AllFolderLocations(false).Count > 0))
                    {
                        // main image for the folder itself
                        TheActionList.Add(_downloadIdentifiers.ProcessShow(si));
                    }


                    foreach (string folder in folders)
                    {
                        if (ActionCancel)
                            return;

                        FileInfo[] files = dfc.Get(folder);
                        if (files == null)
                            continue;

                        if (TVSettings.Instance.NeedToDownloadBannerFile() && timeForBannerUpdate)
                        {
                            //Image series checks here
                            TheActionList.Add(_downloadIdentifiers.ForceUpdateSeason(DownloadIdentifier.DownloadType.DownloadImage, si, folder, snum));
                        }

                        bool renCheck = TVSettings.Instance.RenameCheck && si.DoRename && Directory.Exists(folder); // renaming check needs the folder to exist
                        bool missCheck = TVSettings.Instance.MissingCheck && si.DoMissingCheck;

                        //Image series checks here
                        TheActionList.Add(_downloadIdentifiers.ProcessSeason(si, folder, snum));

                        FileInfo[] localEps = new FileInfo[maxEpisodeNumber + 1];

                        int maxEpNumFound = 0;
                        if (!renCheck && !missCheck)
                            continue;

                        foreach (FileInfo fi in files)
                        {
                            if (ActionCancel)
                                return;

                            int seasNum;
                            int epNum;

                            if (!FindSeasEp(fi, out seasNum, out epNum, si))
                                continue; // can't find season & episode, so this file is of no interest to us

                            if (seasNum == -1)
                                seasNum = snum;

#if !NOLAMBDA
                            int epIdx = eps.FindIndex(x => ((x.EpNum == epNum) && (x.SeasonNumber == seasNum)));
                            if (epIdx == -1)
                                continue; // season+episode number don't correspond to any episode we know of from thetvdb
                            ProcessedEpisode ep = eps[epIdx];
#else
    // equivalent of the 4 lines above, if compiling on MonoDevelop on Windows which, for 
    // some reason, doesn't seem to support lambda functions (the => thing)
							
							ProcessedEpisode ep = null;
							
							foreach (ProcessedEpisode x in eps)
							{
								if (((x.EpNum == epNum) && (x.SeasonNumber == seasNum)))
								{
									ep = x;
									break;
								}
							}
							if (ep == null)
                              continue;
                            // season+episode number don't correspond to any episode we know of from thetvdb
#endif

                            FileInfo actualFile = fi;

                            if (renCheck && TVSettings.Instance.UsefulExtension(fi.Extension, true)) // == RENAMING CHECK ==
                            {
                                string newname = TVSettings.Instance.FilenameFriendly(TVSettings.Instance.NamingStyle.NameForExt(ep, fi.Extension, folder.Length));

                                if (newname != actualFile.Name)
                                {
                                    actualFile = FileHelper.FileInFolder(folder, newname); // rename updates the filename
                                    TheActionList.Add(new ActionCopyMoveRename(ActionCopyMoveRename.Op.Rename, fi, actualFile, ep, null));

                                    //The following section informs the DownloadIdentifers that we already plan to
                                    //copy a file inthe appropriate place and they do not need to worry about downloading 
                                    //one for that purpse

                                    _downloadIdentifiers.NotifyComplete(actualFile);

                                }
                            }
                            if (missCheck && TVSettings.Instance.UsefulExtension(fi.Extension, false)) // == MISSING CHECK part 1/2 ==
                            {
                                // first pass of missing check is to tally up the episodes we do have
                                localEps[epNum] = actualFile;
                                if (epNum > maxEpNumFound)
                                    maxEpNumFound = epNum;
                            }
                        } // foreach file in folder

                        if (missCheck) // == MISSING CHECK part 2/2 (includes NFO and Thumbnails) ==
                        {
                            // second part of missing check is to see what is missing!

                            // look at the offical list of episodes, and look to see if we have any gaps

                            DateTime today = DateTime.Now;
                            TheTVDB.Instance.GetLock("UpToDateCheck");
                            foreach (ProcessedEpisode dbep in eps)
                            {
                                if ((dbep.EpNum > maxEpNumFound) || (localEps[dbep.EpNum] == null)) // not here locally
                                {
                                    DateTime? dt = dbep.GetAirDateDt(true);
                                    bool dtOk = dt != null;

                                    bool notFuture = (dtOk && (dt.Value.CompareTo(today) < 0)); // isn't an episode yet to be aired

                                    bool noAirdatesUntilNow = true;
                                    // for specials "season", see if any season has any airdates
                                    // otherwise, check only up to the season we are considering
                                    for (int i = 1; i <= ((snum == 0) ? lastSeason : snum); i++)
                                    {
                                        if (HasAnyAirdates(si, i))
                                        {
                                            noAirdatesUntilNow = false;
                                            break;
                                        }

                                        //If the show is in its first season and no episodes have air dates
                                        if (lastSeason == 1)
                                        {
                                            noAirdatesUntilNow = false;
                                        }
                                    }

                                    // only add to the missing list if, either:
                                    // - force check is on
                                    // - there are no airdates at all, for up to and including this season
                                    // - there is an airdate, and it isn't in the future
                                    if (noAirdatesUntilNow ||
                                        ((si.ForceCheckFuture || notFuture) && dtOk) ||
                                        (si.ForceCheckNoAirdate && !dtOk))
                                    {
                                        // then add it as officially missing
                                        TheActionList.Add(new ItemMissing(dbep, folder + Path.DirectorySeparatorChar + TVSettings.Instance.FilenameFriendly(TVSettings.Instance.NamingStyle.NameForExt(dbep, null, folder.Length))));
                                    }
                                }
                                else
                                {
                                    // the file is here
                                    if (showList == null)
                                        _mStats.NsNumberOfEpisodes++;

                                    // do NFO and thumbnail checks if required
                                    FileInfo filo = localEps[dbep.EpNum]; // filename (or future filename) of the file

                                    TheActionList.Add(_downloadIdentifiers.ProcessEpisode(dbep, filo));

                                }
                            } // up to date check, for each episode in thetvdb
                            TheTVDB.Instance.Unlock("UpToDateCheck");
                        } // if doing missing check
                    } // for each folder for this season of this show
                } // for each season of this show
            } // for each show

            UnlockShowItems();
            RemoveIgnored();
        }

        public void NoProgress(int pct)
        {
        }

        public void ScanWorker(Object o)
        {
            List<ShowItem> specific = (List<ShowItem>)(o);

            while (!Args.Hide && ((ScanProgDlg == null) || (!ScanProgDlg.Ready)))
                Thread.Sleep(10); // wait for thread to create the dialog

            TheActionList = new ItemList();
            SetProgressDelegate noProgress = NoProgress;

            if (TVSettings.Instance.RenameCheck || TVSettings.Instance.MissingCheck)
                RenameAndMissingCheck(ScanProgDlg == null ? noProgress : ScanProgDlg.MediaLibProg, specific);

            if (TVSettings.Instance.RemoveDownloadDirectoriesFiles)
                FindUnusedFilesInDlDirectory(specific);

            if (TVSettings.Instance.MissingCheck)
            {
                // have a look around for any missing episodes
                int activeLocalFinders = 0;
                int activeRssFinders = 0;
                int activeDownloadingFinders = 0;

                foreach (Finder f in _finders)
                {
                    if (f.Active())
                    {
                        f.SetActionList(TheActionList);

                        switch (f.DisplayType())
                        {
                            case Finder.FinderDisplayType.Local:
                                activeLocalFinders++;
                                break;
                            case Finder.FinderDisplayType.Downloading:
                                activeDownloadingFinders++;
                                break;
                            case Finder.FinderDisplayType.Rss:
                                activeRssFinders++;
                                break;
                        }
                    }
                }

                int currentLocalFinderId = 0;
                int currentRssFinderId = 0;
                int currentDownloadingFinderId = 0;

                foreach (Finder f in _finders)
                {
                    if (ActionCancel)
                    {
                        return;
                    }

                    if (f.Active() && ListHasMissingItems(TheActionList))
                    {

                        int startPos;
                        int endpos = 0;

                        switch (f.DisplayType())
                        {
                            case Finder.FinderDisplayType.Local:
                                currentLocalFinderId++;
                                startPos = 100 * (currentLocalFinderId) / activeLocalFinders;
                                f.Check(ScanProgDlg == null ? noProgress : ScanProgDlg.LocalSearchProg, startPos, endpos);
                                break;
                            case Finder.FinderDisplayType.Downloading:
                                currentDownloadingFinderId++;
                                startPos = 100 * (currentDownloadingFinderId) / activeDownloadingFinders;
                                f.Check(ScanProgDlg == null ? noProgress : ScanProgDlg.DownloadingProg, startPos, endpos);
                                break;
                            case Finder.FinderDisplayType.Rss:
                                currentRssFinderId++;
                                startPos = 100 * (currentRssFinderId) / activeRssFinders;
                                f.Check(ScanProgDlg == null ? noProgress : ScanProgDlg.RssProg, startPos, endpos);
                                break;
                        }

                        RemoveIgnored();
                    }
                }
            }
            if (ActionCancel)
                return;

            // sort Action list by type
            TheActionList.Sort(new ActionItemSorter()); // was new ActionSorter()

            if (ScanProgDlg != null)
                ScanProgDlg.Done();

            Stats().FindAndOrganisesDone++;
        }

        public static bool MatchesSequentialNumber(string filename, ref int seas, ref int ep, ProcessedEpisode pe)
        {
            if (pe.OverallNumber == -1)
                return false;

            string num = pe.OverallNumber.ToString();

            bool found = Regex.Match("X" + filename + "X", "[^0-9]0*" + num + "[^0-9]").Success; // need to pad to let it match non-numbers at start and end
            if (found)
            {
                seas = pe.SeasonNumber;
                ep = pe.EpNum;
            }
            return found;
        }

        public static string SeFinderSimplifyFilename(string filename, string showNameHint)
        {
            // Look at showNameHint and try to remove the first occurance of it from filename
            // This is very helpful if the showname has a >= 4 digit number in it, as that
            // would trigger the 1302 -> 13,02 matcher
            // Also, shows like "24" can cause confusion

            filename = filename.Replace(".", " "); // turn dots into spaces

            if (string.IsNullOrEmpty(showNameHint))
                return filename;

            bool nameIsNumber = (Regex.Match(showNameHint, "^[0-9]+$").Success);

            int p = filename.IndexOf(showNameHint);

            if (p == 0)
            {
                filename = filename.Remove(0, showNameHint.Length);
                return filename;
            }

            if (nameIsNumber) // e.g. "24", or easy exact match of show name at start of filename
                return filename;

            foreach (Match m in Regex.Matches(showNameHint, "(?:^|[^a-z]|\\b)([0-9]{3,})")) // find >= 3 digit numbers in show name
            {
                if (m.Groups.Count > 1) // just in case
                {
                    string number = m.Groups[1].Value;
                    filename = Regex.Replace(filename, "(^|\\W)" + number + "\\b", ""); // remove any occurances of that number in the filename
                }
            }

            return filename;
        }

        private static bool FindSeasEpDateCheck(FileInfo fi, out int seas, out int ep, ShowItem si)
        {
            if (fi == null || si == null)
            {
                seas = -1;
                ep = -1;
                return false;
            }

            // look for a valid airdate in the filename
            // check for YMD, DMY, and MDY
            // only check against airdates we expect for the given show
            SeriesInfo ser = TheTVDB.Instance.GetSeries(si.TVDBCode);
            string[] dateFormats = { "yyyy-MM-dd", "dd-MM-yyyy", "MM-dd-yyyy", "yy-MM-dd", "dd-MM-yy", "MM-dd-yy" };
            string filename = fi.Name;
            // force possible date separators to a dash
            filename = filename.Replace("/", "-");
            filename = filename.Replace(".", "-");
            filename = filename.Replace(",", "-");
            filename = filename.Replace(" ", "-");

            ep = -1;
            seas = -1;

            foreach (KeyValuePair<int, Season> kvp in ser.Seasons)
            {
                if (si.IgnoreSeasons.Contains(kvp.Value.SeasonNumber))
                    continue;

                foreach (Episode epi in kvp.Value.Episodes)
                {
                    DateTime? dt = epi.GetAirDateDt(false); // file will have local timezone date, not ours
                    if (dt == null)
                        continue;

                    TimeSpan closestDate = TimeSpan.MaxValue;

                    foreach (string dateFormat in dateFormats)
                    {
                        string datestr = dt.Value.ToString(dateFormat);
                        DateTime dtInFilename;
                        if (filename.Contains(datestr) && DateTime.TryParseExact(datestr, dateFormat, new CultureInfo("en-GB"), DateTimeStyles.None, out dtInFilename))
                        {
                            TimeSpan timeAgo = DateTime.Now.Subtract(dtInFilename);
                            if (timeAgo < closestDate)
                            {
                                seas = epi.SeasonNumber;
                                ep = epi.EpNum;
                                closestDate = timeAgo;
                            }
                        }
                    }
                }
            }

            return ((ep != -1) && (seas != -1));
        }

        public List<ProcessedEpisode> GetMissingEps()
        {
            int dd = TVSettings.Instance.WtwRecentDays;
            DirFilesCache dfc = new DirFilesCache();
            return GetMissingEps(dfc, GetRecentAndFutureEps(dfc, dd));
        }


        public List<ProcessedEpisode> GetRecentAndFutureEps(DirFilesCache dfc, int days)
        {
            List<ProcessedEpisode> returnList = new List<ProcessedEpisode>();

            foreach (ShowItem si in GetShowItems(true))
            {
                if (!si.ShowNextAirdate)
                    continue;

                foreach (KeyValuePair<int, List<ProcessedEpisode>> kvp in si.SeasonEpisodes)
                {
                    if (si.IgnoreSeasons.Contains(kvp.Key))
                        continue; // ignore this season

                    List<ProcessedEpisode> eis = kvp.Value;

                    bool nextToAirFound = false;

                    foreach (ProcessedEpisode ei in eis)
                    {
                        DateTime? dt = ei.GetAirDateDt(true);
                        if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                        {
                            TimeSpan ts = dt.Value.Subtract(DateTime.Now);
                            if (ts.TotalHours >= (-24 * days)) // in the future (or fairly recent)
                            {
                                if ((ts.TotalHours >= 0) && (!nextToAirFound))
                                {
                                    nextToAirFound = true;
                                    ei.NextToAir = true;
                                }
                                else
                                    ei.NextToAir = false;
                                returnList.Add(ei);
                            }
                        }
                    }
                }

            }
            return returnList;
        }

        public List<ProcessedEpisode> GetMissingEps(DirFilesCache dfc, List<ProcessedEpisode> lpe)
        {
            List<ProcessedEpisode> missing = new List<ProcessedEpisode>();

            foreach (ProcessedEpisode pe in lpe)
            {
                List<FileInfo> fl = FindEpOnDisk(dfc, pe);

                bool foundOnDisk = ((fl != null) && (fl.Count > 0));
                bool alreadyAired = pe.GetAirDateDt(true).Value.CompareTo(DateTime.Now) < 0;


                if (!foundOnDisk && alreadyAired && (pe.Si.DoMissingCheck))
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

            foreach (String dirPath in SearchFolders)
            {
                if (!Directory.Exists(dirPath)) continue;

                foreach (String filePath in Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories))
                {
                    if (!File.Exists(filePath)) continue;

                    FileInfo fi = new FileInfo(filePath);

                    if (!TVSettings.Instance.UsefulExtension(fi.Extension, false))
                        continue; // move on

                    if (TVSettings.Instance.IgnoreSamples && Helpers.Contains(fi.FullName, "sample", StringComparison.OrdinalIgnoreCase) && ((fi.Length / (1024 * 1024)) < TVSettings.Instance.SampleFileMaxSizeMb))
                        continue;

                    foreach (ShowItem si in ShowItems)
                    {
                        if (showsToScan.Contains(si)) continue;

                        if (si.GetSimplifiedPossibleShowNames().Any(name => FileHelper.SimplifyAndCheckFilename(fi.Name, name)))
                            showsToScan.Add(si);
                    }
                }

                foreach (String subDirPath in Directory.GetDirectories(dirPath, "*", SearchOption.AllDirectories))
                {
                    if (!Directory.Exists(subDirPath)) continue;

                    DirectoryInfo di = new DirectoryInfo(subDirPath);

                    List<ShowItem> matchingShows = new List<ShowItem>();

                    foreach (ShowItem si in ShowItems)
                    {
                        if (showsToScan.Contains(si)) continue;

                        if (si.GetSimplifiedPossibleShowNames().Any(name => FileHelper.SimplifyAndCheckFilename(di.Name, name)))
                            showsToScan.Add(si);
                    }

                }
            }

            return showsToScan;
        }

        internal void ForceRefresh(List<ShowItem> sis)
        {
            if (sis != null)
            {
                foreach (ShowItem si in sis)
                {
                    TheTVDB.Instance.ForgetShow(si.TVDBCode, true);
                }
            }
            DoDownloadsFg();
        }

        public static bool FindSeasEp(FileInfo fi, ShowItem si)
        {
            int s;
            int e;
            return FindSeasEp(fi, out s, out e, si);
        }

        public static bool FindSeasEp(FileInfo fi, out int seas, out int ep, ShowItem si)
        {
            return FindSeasEp(fi, out seas, out ep, si, TVSettings.Instance.FnpRegexs, TVSettings.Instance.LookForDateInFilename);
        }

        public static bool FindSeasEp(FileInfo fi, out int seas, out int ep, ShowItem si, List<FilenameProcessorRe> rexps, bool doDateCheck)
        {
            if (fi == null)
            {
                seas = -1;
                ep = -1;
                return false;
            }

            if (doDateCheck && FindSeasEpDateCheck(fi, out seas, out ep, si))
                return true;

            string filename = fi.Name;
            int l = filename.Length;
            int le = fi.Extension.Length;
            filename = filename.Substring(0, l - le);
            return FindSeasEp(fi.Directory.FullName, filename, out seas, out ep, si, rexps);
        }

        public static bool FindSeasEp(DirectoryInfo di, out int seas, out int ep, ShowItem si)
        {

            List<FilenameProcessorRe> rexps = TVSettings.Instance.FnpRegexs;

            if (di == null)
            {
                seas = -1;
                ep = -1;
                return false;
            }

            return FindSeasEp(di.Parent.FullName, di.Name, out seas, out ep, si, rexps);
        }


        public static bool FindSeasEp(string directory, string filename, out int seas, out int ep, ShowItem si, List<FilenameProcessorRe> rexps)
        {
            string showNameHint = (si != null) ? si.ShowName : "";

            seas = ep = -1;

            filename = SeFinderSimplifyFilename(filename, showNameHint);

            string fullPath = directory + Path.DirectorySeparatorChar + filename; // construct full path with sanitised filename

            if ((filename.Length > 256) || (fullPath.Length > 256))
                return false;

            int leftMostPos = filename.Length;

            filename = filename.ToLower() + " ";
            fullPath = fullPath.ToLower() + " ";

            foreach (FilenameProcessorRe re in rexps)
            {
                if (!re.Enabled)
                    continue;
                try
                {
                    Match m = Regex.Match(re.UseFullPath ? fullPath : filename, re.Re, RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        int adj = re.UseFullPath ? (fullPath.Length - filename.Length) : 0;

                        int p = Math.Min(m.Groups["s"].Index, m.Groups["e"].Index) - adj;
                        if (p >= leftMostPos)
                            continue;

                        if (!int.TryParse(m.Groups["s"].ToString(), out seas))
                            seas = -1;
                        if (!int.TryParse(m.Groups["e"].ToString(), out ep))
                            ep = -1;

                        leftMostPos = p;
                    }
                }
                catch (FormatException)
                {
                }
                catch (ArgumentException)
                { }
            }

            return ((seas != -1) || (ep != -1));
        }

        #region Nested type: ProcessActionInfo

        private class ProcessActionInfo
        {
            public readonly int SemaphoreNumber;
            public readonly IAction TheAction;

            public ProcessActionInfo(int n, IAction a)
            {
                SemaphoreNumber = n;
                TheAction = a;
            }
        }

        #endregion

        private ShowItem ShowItemForCode(int code)
        {
            foreach (ShowItem si in ShowItems)
            {
                if (si.TVDBCode == code)
                    return si;
            }
            return null;
        }

        internal List<ShowItem> GetRecentShows()
        {
            // only scan "recent" shows
            List<ShowItem> shows = new List<ShowItem>();
            int dd = TVSettings.Instance.WtwRecentDays;

            // for each show, see if any episodes were aired in "recent" days...
            foreach (ShowItem si in GetShowItems(true))
            {
                bool added = false;

                foreach (KeyValuePair<int, List<ProcessedEpisode>> kvp in si.SeasonEpisodes)
                {
                    if (added)
                        break;

                    if (si.IgnoreSeasons.Contains(kvp.Key))
                        continue; // ignore this season

                    List<ProcessedEpisode> eis = kvp.Value;

                    foreach (ProcessedEpisode ei in eis)
                    {
                        DateTime? dt = ei.GetAirDateDt(true);
                        if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                        {
                            TimeSpan ts = dt.Value.Subtract(DateTime.Now);
                            if ((ts.TotalHours >= (-24 * dd)) && (ts.TotalHours <= 0)) // fairly recent?
                            {
                                shows.Add(si);
                                added = true;
                                break;
                            }
                        }
                    }
                }
            }
            return shows;
        }
    }
}

