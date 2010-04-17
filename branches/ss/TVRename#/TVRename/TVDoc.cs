// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

// "Doc" is short for "Document", from the "Document" and "View" model way of thinking of things.
// All the processing and work should be done in here, nothing in UI.cs
// Means we can run TVRename and do useful stuff, without showing any UI. (i.e. text mode / console app)

namespace TVRename
{
    public class TVDoc
    {
        private static ShowItemList ShowItems;
        public bool ActionCancel;
        public System.Collections.Generic.List<FolderMonitorItem> AddItems;
        public CommandLineArgs Args;

        public bool DownloadDone;
        private bool DownloadOK;
        public int DownloadPct;
        public bool DownloadStopOnError;
        public int DownloadsRemaining;
        public System.Collections.Generic.List<IgnoreItem> Ignore;
        public StringList IgnoreFolders;
        public string LoadErr;
        public bool LoadOK;
        public StringList MonitorFolders;
        public RSSItemList RSSList;
        public ScanProgress ScanProgDlg;
        public StringList SearchFolders;
        public TVSettings Settings;
        public ItemList TheActionList;
        public Semaphore WorkerSemaphore;
        public System.Collections.Generic.List<Thread> Workers;
        private bool mDirty;
        private Thread mDownloaderThread;
        private TVRenameStats mStats;
        private TheTVDB mTVDB;

        public TVDoc(FileInfo settingsFile, TheTVDB tvdb, CommandLineArgs args)
        {
            this.mTVDB = tvdb;
            this.Args = args;

            this.Ignore = new System.Collections.Generic.List<IgnoreItem>();

            this.Workers = null;
            this.WorkerSemaphore = null;

            this.mStats = new TVRenameStats();
            this.mDirty = false;
            this.TheActionList = new ItemList();

            this.Settings = new TVSettings();

            this.MonitorFolders = new StringList();
            this.IgnoreFolders = new StringList();
            this.SearchFolders = new StringList();
            ShowItems = new ShowItemList();
            this.AddItems = new System.Collections.Generic.List<FolderMonitorItem>();

            this.DownloadDone = true;
            this.DownloadOK = true;

            this.ActionCancel = false;
            this.ScanProgDlg = null;

            this.LoadOK = ((settingsFile == null) || this.LoadXMLSettings(settingsFile)) && this.mTVDB.LoadOK;

            //    StartServer();
        }

        public TheTVDB GetTVDB(bool lockDB, string whoFor)
        {
            if (lockDB)
            {
                if (string.IsNullOrEmpty(whoFor))
                    whoFor = "unknown";

                this.mTVDB.GetLock("GetTVDB : " + whoFor);
            }
            return this.mTVDB;
        }

        public static FileInfo TVDBFile()
        {
            return new FileInfo(System.Windows.Forms.Application.UserAppDataPath + System.IO.Path.DirectorySeparatorChar + "TheTVDB.xml");
        }

        public static FileInfo TVDocSettingsFile()
        {
            return new FileInfo(System.Windows.Forms.Application.UserAppDataPath + System.IO.Path.DirectorySeparatorChar + "TVRenameSettings.xml");
        }

        ~TVDoc()
        {
            this.StopBGDownloadThread();
        }

        private void LockShowItems()
        {
            return;
            /*#if DEBUG
                             System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1);
                             System.Diagnostics.StackFrame sf = st.GetFrame(0);
                             string msg = sf.GetMethod().DeclaringType.FullName + "::" + sf.GetMethod().Name;
                             System.Diagnostics.Debug.Print("LockShowItems " + msg);
            #endif
                             Monitor.Enter(ShowItems);
                    */
        }

        public void UnlockShowItems()
        {
            return;
            /*
    #if DEBUG
                    System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1);
                    System.Diagnostics.StackFrame sf = st.GetFrame(0);
                    string msg = sf.GetMethod().DeclaringType.FullName + "::" + sf.GetMethod().Name;
                    System.Diagnostics.Debug.Print("UnlockShowItems " + msg);
    #endif

                    Monitor.Exit(ShowItems);
             */
        }

        public TVRenameStats Stats()
        {
            this.mStats.NS_NumberOfShows = ShowItems.Count;
            this.mStats.NS_NumberOfSeasons = 0;
            this.mStats.NS_NumberOfEpisodesExpected = 0;

            this.LockShowItems();
            foreach (ShowItem si in ShowItems)
            {
                foreach (System.Collections.Generic.KeyValuePair<int, ProcessedEpisodeList> k in si.SeasonEpisodes)
                    this.mStats.NS_NumberOfEpisodesExpected += k.Value.Count;
                this.mStats.NS_NumberOfSeasons += si.SeasonEpisodes.Count;
            }
            this.UnlockShowItems();

            return this.mStats;
        }

        public void SetDirty()
        {
            this.mDirty = true;
        }

        public bool Dirty()
        {
            return this.mDirty;
        }

        public ShowItemList GetShowItems(bool lockThem)
        {
            if (lockThem)
                this.LockShowItems();

            ShowItems.Sort(new Comparison<ShowItem>(ShowItem.CompareShowItemNames));
            return ShowItems;
        }

        public ShowItem GetShowItem(int id)
        {
            this.LockShowItems();
            foreach (ShowItem si in ShowItems)
            {
                if (si.TVDBCode == id)
                {
                    this.UnlockShowItems();
                    return si;
                }
            }
            this.UnlockShowItems();
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
            this.Settings.TheSearchers.SetToNumber(n);
            this.SetDirty();
        }

        public bool FolderIsSubfolderOf(string thisOne, string ofThat)
        {
            // need terminating slash, otherwise "c:\abc def" will match "c:\abc"
            thisOne += System.IO.Path.DirectorySeparatorChar.ToString();
            ofThat += System.IO.Path.DirectorySeparatorChar.ToString();
            int l = ofThat.Length;
            return ((thisOne.Length >= l) && (thisOne.Substring(0, l).ToLower() == ofThat.ToLower()));
        }

        public void CheckFolder(DirectoryInfo di, System.Collections.Generic.List<FolderMonitorItem> addList) // check a monitored folder for new shows
        {
            foreach (DirectoryInfo di2 in di.GetDirectories())
            {
                string seasonWord = "Season ";
                bool hasSeasonFolders = false;
                try
                {
                    // keep in sync with ProcessAddItems, etc.
                    hasSeasonFolders = di2.GetDirectories("*" + seasonWord + "*").Length > 0; // todo - use non specific word
                }
                catch (UnauthorizedAccessException)
                {
                    // e.g. recycle bin, system volume information
                    continue;
                }

                string theFolder = di2.FullName.ToLower();

                // if its not in the ignore list
                if (this.IgnoreFolders.Contains(theFolder))
                    continue;

                if (!hasSeasonFolders)
                    this.CheckFolder(di2, addList); // not a season folder.. recurse!

                // ..and not already a folder for one of our shows
                bool bzzt = false;
                foreach (ShowItem si in ShowItems)
                {
                    if (si.AutoAddNewSeasons && !string.IsNullOrEmpty(si.AutoAdd_FolderBase) && this.FolderIsSubfolderOf(theFolder, si.AutoAdd_FolderBase))
                    {
                        // we're looking at a folder that is a subfolder of an existing show
                        bzzt = true;
                        break;
                    }

                    if (bzzt)
                        break;

                    System.Collections.Generic.Dictionary<int, StringList> afl = si.AllFolderLocations(this.Settings);
                    foreach (System.Collections.Generic.KeyValuePair<int, StringList> kvp in afl)
                    {
                        foreach (string folder in kvp.Value)
                        {
                            if (theFolder.ToLower() == folder.ToLower())
                            {
                                bzzt = true;
                                break;
                            }
                        }
                    }

                    if (bzzt)
                        break;
                } // for each showitem
                if (!bzzt)
                {
                    // ....its good!
                    addList.Add(new FolderMonitorItem(di2.FullName, hasSeasonFolders ? FolderModeEnum.kfmFolderPerSeason : FolderModeEnum.kfmFlat, -1));
                }
            } // for each directory
        }

        public void CheckMonitoredFolders()
        {
            this.AddItems.Clear();

            this.LockShowItems();
            foreach (string folder in this.MonitorFolders)
            {
                //try {
                DirectoryInfo di = new DirectoryInfo(folder);
                if (!di.Exists)
                    continue;

                this.CheckFolder(di, this.AddItems);
                //                     }
                //					catch (...)
                //					{
                //					}
            }
            this.UnlockShowItems();
        }

        public bool RenameFilesToMatchTorrent(string torrent, string folder, TreeView tvTree, SetProgressDelegate prog, bool copyNotMove, string copyDest)
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

            this.Stats().TorrentsMatched++;

            BTFileRenamer btp = new BTFileRenamer(prog);
            ItemList newList = new ItemList();
            bool r = btp.RenameFilesOnDiskToMatchTorrent(torrent, folder, tvTree, newList, copyNotMove, copyDest);

            foreach (Item i in newList)
                this.TheActionList.Add(i);

            return r;
        }

        // consider each of the files, see if it is suitable for series "ser" and episode "epi"
        // if so, add a rcitem for copy to "fi"
        public bool FindMissingEp(DirCache dirCache, ActionMissing me, ItemList addTo, ActionCopyMoveRename.Op whichOp)
        {
            string showname = me.Episode.SI.ShowName();
            int season = me.Episode.SeasonNumber;

            //String ^toName = FilenameFriendly(Settings->NamingStyle->NameFor(me->PE));
            int epnum = me.Episode.EpNum;

            // TODO: find a 'best match', or use first ?

            showname = Helpers.SimplifyName(showname);

            foreach (DirCacheEntry dce in dirCache)
            {
                if (this.ActionCancel)
                    return true;

                bool matched = false;

                try
                {
                    if (!dce.HasUsefulExtension_NotOthersToo) // not a usefile file extension
                        continue;
                    if (this.Settings.IgnoreSamples && dce.LowerName.Contains("sample") && ((dce.Length / (1024 * 1024)) < this.Settings.SampleFileMaxSizeMB))
                        continue;

                    matched = Regex.Match(dce.SimplifiedFullName, "\\b" + showname + "\\b", RegexOptions.IgnoreCase).Success;

                    if (matched)
                    {
                        int seasF;
                        int epF;
                        // String ^fn = file->Name;

                        if ((this.FindSeasEp(dce.TheFile, out seasF, out epF, me.Episode.TheSeries.Name) && (seasF == season) && (epF == epnum)) || (me.Episode.SI.UseSequentialMatch && this.MatchesSequentialNumber(dce.TheFile.Name, ref seasF, ref epF, me.Episode) && (seasF == season) && (epF == epnum)))
                        {
                            FileInfo fi = new FileInfo(me.TheFileNoExt + dce.TheFile.Extension);
                            addTo.Add(new ActionCopyMoveRename(whichOp, dce.TheFile, fi, me.Episode));

                            // if we're copying/moving a file across, we might also want to make a thumbnail or NFO for it
                            this.ThumbnailAndNFOCheck(me.Episode, fi, addTo);

                            return true;
                        }
                    }
                }
                catch (System.IO.PathTooLongException e)
                {
                    string t = "Path too long. " + dce.TheFile.FullName + ", " + e.Message;
                    t += ".  Try to display more info?";
                    DialogResult dr = MessageBox.Show(t, "Path too long", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (dr == DialogResult.Yes)
                    {
                        t = "DirectoryName " + dce.TheFile.DirectoryName + ", File name: " + dce.TheFile.Name;
                        t += matched ? ", matched.  " : ", no match.  ";
                        if (matched)
                        {
                            t += "Show: " + me.Episode.TheSeries.Name + ", Season " + season + ", Ep " + epnum + ".  ";
                            t += "To: " + me.TheFileNoExt;
                        }

                        MessageBox.Show(t, "Path too long", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }

            return false;
        }

        public void KeepTogether(ItemList Actionlist)
        {
            // for each of the items in rcl, do the same copy/move if for other items with the same
            // base name, but different extensions

            ItemList extras = new ItemList();

            foreach (Item Action1 in Actionlist)
            {
                if (!(Action1 is ActionCopyMoveRename))
                    continue;

                ActionCopyMoveRename Action = (ActionCopyMoveRename) (Action1);

                try
                {
                    DirectoryInfo sfdi = Action.From.Directory;
                    string basename = Action.From.Name;
                    int l = basename.Length;
                    basename = basename.Substring(0, l - Action.From.Extension.Length);

                    string toname = Action.To.Name;
                    int l2 = toname.Length;
                    toname = toname.Substring(0, l2 - Action.To.Extension.Length);

                    FileInfo[] flist = sfdi.GetFiles(basename + ".*");
                    foreach (FileInfo fi in flist)
                    {
                        // do case insensitive replace
                        string n = fi.Name;
                        int p = n.ToUpper().IndexOf(basename.ToUpper());
                        string newName = n.Substring(0, p) + toname + n.Substring(p + basename.Length);
                        if ((this.Settings.RenameTxtToSub) && (newName.EndsWith(".txt")))
                            newName = newName.Substring(0, newName.Length - 4) + ".sub";

                        ActionCopyMoveRename newitem = new ActionCopyMoveRename(Action.Operation, fi, Helpers.FileInFolder(Action.To.Directory, newName), Action.Episode);

                        // check this item isn't already in our to-do list
                        bool doNotAdd = false;
                        foreach (Item ai2 in Actionlist)
                        {
                            if (!(ai2 is ActionCopyMoveRename))
                                continue;

                            if (((ActionCopyMoveRename) (ai2)).SameSource(newitem))
                            {
                                doNotAdd = true;
                                break;
                            }
                        }

                        if (!doNotAdd)
                        {
                            if (!newitem.SameAs(Action)) // don't re-add ourself
                                extras.Add(newitem);
                        }
                    }
                }
                catch (System.IO.PathTooLongException e)
                {
                    string t = "Path or filename too long. " + Action.From.FullName + ", " + e.Message;
                    MessageBox.Show(t, "Path or filename too long", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            foreach (Item action in extras)
            {
                // check we don't already have this in our list and, if we don't add it!
                bool have = false;
                foreach (Item action2 in Actionlist)
                {
                    if (action2.SameAs(action))
                    {
                        have = true;
                        break;
                    }
                }

                if (!have)
                    Actionlist.Add(action);
            }
        }

        public void LookForMissingEps(SetProgressDelegate prog)
        {
            // for each ep we have noticed as being missing
            // look through the monitored folders for it

            this.Stats().FindAndOrganisesDone++;

            prog.Invoke(0);

            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();

            int fileCount = 0;
            foreach (string s in this.SearchFolders)
                fileCount += DirCache.CountFiles(s, true);

            int c = 0;

            DirCache dirCache = new DirCache();
            foreach (String s in this.SearchFolders)
            {
                if (this.ActionCancel)
                    return;

                c = dirCache.AddFolder(prog, c, fileCount, s, true, this.Settings);
            }

            c = 0;
            int totalN = this.TheActionList.Count;
            foreach (Item action1 in this.TheActionList)
            {
                if (this.ActionCancel)
                    return;

                prog.Invoke(50 + 50 * (++c) / (totalN + 1)); // second 50% of progress bar

                if (action1 is ActionMissing)
                {
                    if (this.FindMissingEp(dirCache, (ActionMissing) (action1), newList, ActionCopyMoveRename.Op.Copy))
                        toRemove.Add(action1);
                }
            }

            if (this.Settings.KeepTogether)
                this.KeepTogether(newList);

            prog.Invoke(100);

            if (!this.Settings.LeaveOriginals)
            {
                // go through and change last of each operation on a given source file to a 'Move'
                // ideally do that move within same filesystem

                // sort based on source file, and destination drive, putting last if destdrive == sourcedrive
                newList.Sort(new ActionSorter());

                // sort puts all the CopyMoveRenames together				

                // then set the last of each source file to be a move
                for (int i = 0; i < newList.Count; i++)
                {
                    ActionCopyMoveRename cmr1 = newList[i] as ActionCopyMoveRename;
                    bool ok1 = cmr1 != null;
                    
                    if (!ok1)
                        continue;

                    bool last = i == (newList.Count - 1);
                    ActionCopyMoveRename cmr2 = !last ? newList[i + 1] as ActionCopyMoveRename : null;
                    bool ok2 = cmr2 != null;
                    
                    if (ok2)
                    {
                        ActionCopyMoveRename a1 = cmr1;
                        ActionCopyMoveRename a2 = cmr2;
                        if (!Helpers.Same(a1.From, a2.From))
                            a1.Operation = ActionCopyMoveRename.Op.Move;
                    }
                    else
                    {
                        // last item, or last copymoverename item in the list
                        ActionCopyMoveRename a1 = cmr1;
                        a1.Operation = ActionCopyMoveRename.Op.Move;
                    }
                }
            }

            foreach (Item i in toRemove)
                this.TheActionList.Remove(i);

            foreach (Item i in newList)
                this.TheActionList.Add(i);

            //                 if (Settings->ExportFOXML)
            //				ExportFOXML(Settings->ExportFOXMLTo);
        }

        // -----------------------------------------------------------------------------

        public void GetThread(Object codeIn)
        {
            System.Diagnostics.Debug.Assert(this.WorkerSemaphore != null);

            this.WorkerSemaphore.WaitOne(); // don't start until we're allowed to

            int code = (int) (codeIn);

            System.Diagnostics.Debug.Print("  Downloading " + code);
            bool r = this.GetTVDB(false, "").EnsureUpdated(code);
            System.Diagnostics.Debug.Print("  Finished " + code);
            if (!r)
            {
                this.DownloadOK = false;
                if (this.DownloadStopOnError)
                    this.DownloadDone = true;
            }
            this.WorkerSemaphore.Release(1);
        }

        public void WaitForAllThreadsAndTidyUp()
        {
            if (this.Workers != null)
            {
                foreach (Thread t in this.Workers)
                {
                    if (t.IsAlive)
                        t.Join();
                }
            }

            this.Workers = null;
            this.WorkerSemaphore = null;
        }

        public void Downloader()
        {
            // do background downloads of webpages

            try
            {
                if (ShowItems.Count == 0)
                {
                    this.DownloadDone = true;
                    this.DownloadOK = true;
                    return;
                }

                if (!this.GetTVDB(false, "").GetUpdates())
                {
                    this.DownloadDone = true;
                    this.DownloadOK = false;
                    return;
                }

                // for eachs of the ShowItems, make sure we've got downloaded data for it

                int n2 = ShowItems.Count;
                int n = 0;
                System.Collections.Generic.List<int> codes = new System.Collections.Generic.List<int>();
                this.LockShowItems();
                foreach (ShowItem si in ShowItems)
                    codes.Add(si.TVDBCode);
                this.UnlockShowItems();

                int numWorkers = this.Settings.ParallelDownloads;
                this.Workers = new System.Collections.Generic.List<Thread>();

                this.WorkerSemaphore = new Semaphore(numWorkers, numWorkers); // allow up to numWorkers working at once

                foreach (int code in codes)
                {
                    this.DownloadPct = 100 * (n + 1) / (n2 + 1);
                    this.DownloadsRemaining = n2 - n;
                    n++;

                    this.WorkerSemaphore.WaitOne(); // blocks until there is an available slot
                    Thread t = new Thread(this.GetThread);
                    this.Workers.Add(t);
                    t.Name = "GetThread:" + code;
                    t.Start(code); // will grab the semaphore as soon as we make it available
                    int nfr = this.WorkerSemaphore.Release(1); // release our hold on the semaphore, so that worker can grab it
                    System.Diagnostics.Debug.Print("Started " + code + " pool has " + nfr + " free");
                    Thread.Sleep(1); // allow the other thread a chance to run and grab

                    // tidy up any finished workers
                    for (int i = this.Workers.Count - 1; i >= 0; i--)
                    {
                        if (!this.Workers[i].IsAlive)
                            this.Workers.RemoveAt(i); // remove dead worker
                    }

                    if (this.DownloadDone)
                        break;
                }

                this.WaitForAllThreadsAndTidyUp();

                this.GetTVDB(false, "").UpdatesDoneOK();
                this.DownloadDone = true;
                this.DownloadOK = true;
                return;
            }
            catch (ThreadAbortException)
            {
                this.DownloadDone = true;
                this.DownloadOK = false;
                return;
            }
            finally
            {
                this.Workers = null;
                this.WorkerSemaphore = null;
            }
        }

        public void StartBGDownloadThread(bool stopOnError)
        {
            if (!this.DownloadDone)
                return;
            this.DownloadStopOnError = stopOnError;
            this.DownloadPct = 0;
            this.DownloadDone = false;
            this.DownloadOK = true;
            this.mDownloaderThread = new Thread(this.Downloader);
            this.mDownloaderThread.Name = "Downloader";
            this.mDownloaderThread.Start();
        }

        public void WaitForBGDownloadDone()
        {
            if ((this.mDownloaderThread != null) && (this.mDownloaderThread.IsAlive))
                this.mDownloaderThread.Join();
            this.mDownloaderThread = null;
        }

        public void StopBGDownloadThread()
        {
            if (this.mDownloaderThread != null)
            {
                this.DownloadDone = true;
                this.mDownloaderThread.Join();

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
                this.mDownloaderThread = null;
            }
        }

        public bool DoDownloadsFG()
        {
            if (this.Settings.OfflineMode)
                return true; // don't do internet in offline mode!

            this.StartBGDownloadThread(true);

            const int delayStep = 100;
            int count = 1000 / delayStep; // one second
            while ((count-- > 0) && (!this.DownloadDone))
                System.Threading.Thread.Sleep(delayStep);

            if (!this.DownloadDone && !this.Args.Hide) // downloading still going on, so time to show the dialog if we're not in /hide mode
            {
                DownloadProgress dp = new DownloadProgress(this);
                dp.ShowDialog();
                dp.Update();
            }

            this.WaitForBGDownloadDone();

            this.GetTVDB(false, "").SaveCache();

            this.GenDict();

            if (!this.DownloadOK)
            {
                MessageBox.Show(this.mTVDB.LastError, "Error while downloading", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.mTVDB.LastError = "";
            }

            return this.DownloadOK;
        }

        public bool GenDict()
        {
            bool res = true;
            this.LockShowItems();
            foreach (ShowItem si in ShowItems)
            {
                if (!this.GenerateEpisodeDict(si))
                    res = false;
            }
            this.UnlockShowItems();
            return res;
        }

        public Searchers GetSearchers()
        {
            return this.Settings.TheSearchers;
        }

        public void TidyTVDB()
        {
            // remove any shows from thetvdb that aren't in My Shows
            TheTVDB db = this.GetTVDB(true, "TidyTVDB");
            System.Collections.Generic.List<int> removeList = new System.Collections.Generic.List<int>();

            foreach (System.Collections.Generic.KeyValuePair<int, SeriesInfo> kvp in this.mTVDB.GetSeriesDict())
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
                db.ForgetShow(i, false);

            db.Unlock("TheTVDB");
            db.SaveCache();
        }

        public void Closing()
        {
            this.StopBGDownloadThread();
            this.Stats().Save();
        }

        public void DoBTSearch(ProcessedEpisode ep)
        {
            if (ep == null)
                return;
            TVDoc.SysOpen(this.Settings.BTSearchURL(ep));
        }

        public void DoWhenToWatch(bool cachedOnly)
        {
            if (!cachedOnly && !this.DoDownloadsFG())
                return;
            if (cachedOnly)
                this.GenDict();
        }

        public System.Collections.Generic.List<System.IO.FileInfo> FindEpOnDisk(ProcessedEpisode pe)
        {
            return this.FindEpOnDisk(pe.SI, pe);
        }

        public System.Collections.Generic.List<System.IO.FileInfo> FindEpOnDisk(ShowItem si, Episode epi)
        {
            System.Collections.Generic.List<System.IO.FileInfo> ret = new System.Collections.Generic.List<System.IO.FileInfo>();

            int seasWanted = epi.TheSeason.SeasonNumber;
            int epWanted = epi.EpNum;

            int snum = epi.TheSeason.SeasonNumber;

            if (!si.AllFolderLocations(this.Settings).ContainsKey(snum))
                return ret;

            foreach (string folder in si.AllFolderLocations(this.Settings)[snum])
            {
                DirectoryInfo di;
                try
                {
                    di = new DirectoryInfo(folder);
                }
                catch
                {
                    return ret;
                }
                if (!di.Exists)
                    return ret;

                FileInfo[] files = di.GetFiles();
                foreach (FileInfo fiTemp in files)
                {
                    int seasFound;
                    int epFound;

                    if (!this.Settings.UsefulExtension(fiTemp.Extension, false))
                        continue; // move on

                    if (this.FindSeasEp(fiTemp, out seasFound, out epFound, si.ShowName()))
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
            TheTVDB db = this.GetTVDB(false, "");
            SeriesInfo ser = db.GetSeries(si.TVDBCode);
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

        public void TVShowNFOCheck(ShowItem si)
        {
            // check there is a TVShow.nfo file in the root folder for the show
            if (string.IsNullOrEmpty(si.AutoAdd_FolderBase)) // no base folder defined
                return;

            if (si.AllFolderLocations(this.Settings).Count == 0) // no seasons enabled
                return;

            FileInfo tvshownfo = Helpers.FileInFolder(si.AutoAdd_FolderBase, "tvshow.nfo");

            bool needUpdate = !tvshownfo.Exists || (si.TheSeries().Srv_LastUpdated > TimeZone.Epoch(tvshownfo.LastWriteTime));
            // was it written before we fixed the bug in <episodeguideurl> ?
            needUpdate = needUpdate || (tvshownfo.LastWriteTime.ToUniversalTime().CompareTo(new DateTime(2009, 9, 13, 7, 30, 0, 0, DateTimeKind.Utc)) < 0);
            if (needUpdate)
                this.TheActionList.Add(new ActionNFO(tvshownfo, si));
        }

        public bool GenerateEpisodeDict(ShowItem si)
        {
            si.SeasonEpisodes.Clear();

            // copy data from tvdb
            // process as per rules
            // done!

            TheTVDB db = this.GetTVDB(true, "GenerateEpisodeDict");

            SeriesInfo ser = db.GetSeries(si.TVDBCode);

            if (ser == null)
                return false; // TODO: warn user

            bool r = true;
            foreach (System.Collections.Generic.KeyValuePair<int, Season> kvp in ser.Seasons)
            {
                ProcessedEpisodeList pel = GenerateEpisodes(si, ser, kvp.Key, true);
                si.SeasonEpisodes[kvp.Key] = pel;
                if (pel == null)
                    r = false;
            }

            System.Collections.Generic.List<int> theKeys = new System.Collections.Generic.List<int>();
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

            db.Unlock("GenerateEpisodeDict");

            return r;
        }

        public static ProcessedEpisodeList GenerateEpisodes(ShowItem si, SeriesInfo ser, int snum, bool applyRules)
        {
            ProcessedEpisodeList eis = new ProcessedEpisodeList();

            if ((ser == null) || !ser.Seasons.ContainsKey(snum))
                return null; // todo.. something?

            Season seas = ser.Seasons[snum];

            if (seas == null)
                return null; // TODO: warn user

            foreach (Episode e in seas.Episodes)
                eis.Add(new ProcessedEpisode(e, si)); // add a copy

            if (si.DVDOrder)
            {
                eis.Sort(new System.Comparison<ProcessedEpisode>(ProcessedEpisode.DVDOrderSorter));
                Renumber(eis);
            }
            else
                eis.Sort(new System.Comparison<ProcessedEpisode>(ProcessedEpisode.EPNumberSorter));

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
                                ProcessedEpisode pe = new ProcessedEpisode(ep, si);
                                pe.TheSeason = eis[i].TheSeason;
                                pe.SeasonID = eis[i].SeasonID;
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
                System.Collections.Generic.List<ShowRule> rules = si.RulesForSeason(snum);
                if (rules != null)
                    ApplyRules(eis, rules, si);
            }

            return eis;
        }

        public static void ApplyRules(ProcessedEpisodeList eis, System.Collections.Generic.List<ShowRule> rules, ShowItem si)
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

                string txt = sr.UserSuppliedText;
                int ec = eis.Count;

                switch (sr.DoWhatNow)
                {
                    case RuleAction.kRename:
                        {
                            if ((n1 < ec) && (n1 >= 0))
                                eis[n1].Name = txt;
                            break;
                        }
                    case RuleAction.kRemove:
                        {
                            if ((n1 < ec) && (n1 >= 0) && (n2 < ec) && (n2 >= 0))
                                eis.RemoveRange(n1, 1 + n2 - n1);
                            else if ((n1 < ec) && (n1 >= 0) && (n2 == -1))
                                eis.RemoveAt(n1);
                            break;
                        }
                    case RuleAction.kIgnoreEp:
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
                    case RuleAction.kSplit:
                        {
                            // split one episode into a multi-parter
                            if ((n1 < ec) && (n1 >= 0))
                            {
                                ProcessedEpisode ei = eis[n1];
                                string nameBase = ei.Name;
                                eis.RemoveAt(n1); // remove old one
                                for (int i = 0; i < nn2; i++) // make n2 new parts
                                {
                                    ProcessedEpisode pe2 = new ProcessedEpisode(ei, si);
                                    pe2.Name = nameBase + " (Part " + (i + 1) + ")";
                                    pe2.EpNum = -2;
                                    pe2.EpNum2 = -2;
                                    eis.Insert(n1 + i, pe2);
                                }
                            }
                            break;
                        }
                    case RuleAction.kMerge:
                    case RuleAction.kCollapse:
                        {
                            if ((n1 != -1) && (n2 != -1) && (n1 < ec) && (n2 < ec) && (n1 < n2))
                            {
                                ProcessedEpisode oldFirstEI = eis[n1];
                                string combinedName = eis[n1].Name + " + ";
                                string combinedSummary = eis[n1].Overview + "<br/><br/>";
                                //int firstNum = eis[n1]->TVcomEpCount();
                                for (int i = n1 + 1; i <= n2; i++)
                                {
                                    combinedName += eis[i].Name;
                                    combinedSummary += eis[i].Overview;
                                    if (i != n2)
                                    {
                                        combinedName += " + ";
                                        combinedSummary += "<br/><br/>";
                                    }
                                }

                                eis.RemoveRange(n1, n2 - n1);

                                eis.RemoveAt(n1);

                                ProcessedEpisode pe2 = new ProcessedEpisode(oldFirstEI, si);
                                pe2.Name = ((string.IsNullOrEmpty(txt)) ? combinedName : txt);
                                pe2.EpNum = -2;
                                if (sr.DoWhatNow == RuleAction.kMerge)
                                    pe2.EpNum2 = -2 + n2 - n1;
                                else
                                    pe2.EpNum2 = -2;

                                pe2.Overview = combinedSummary;
                                eis.Insert(n1, pe2);
                            }
                            break;
                        }
                    case RuleAction.kSwap:
                        {
                            if ((n1 != -1) && (n2 != -1) && (n1 < ec) && (n2 < ec))
                            {
                                ProcessedEpisode t = eis[n1];
                                eis[n1] = eis[n2];
                                eis[n2] = t;
                            }
                            break;
                        }
                    case RuleAction.kInsert:
                        {
                            if ((n1 < ec) && (n1 >= 0))
                            {
                                ProcessedEpisode t = eis[n1];
                                ProcessedEpisode n = new ProcessedEpisode(t.TheSeries, t.TheSeason, si);
                                n.Name = txt;
                                n.EpNum = -2;
                                n.EpNum2 = -2;
                                eis.Insert(n1, n);
                                break;
                            }
                        }
                        break;
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

        public static void Renumber(ProcessedEpisodeList eis)
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

        public static void GuessShowName(FolderMonitorItem ai)
        {
            // see if we can guess a season number and show name, too
            // Assume is blah\blah\blah\show\season X
            string showName = "";

            string sp = ai.Folder;
            string seasonFinder = ".*season[ _\\.]+([0-9]+).*";
            if (Regex.Matches(sp, seasonFinder, RegexOptions.IgnoreCase).Count > 0)
            {
                // String ^s = Regex::Replace(sp, seasonFinder, "$1",RegexOptions::IgnoreCase);
                try
                {
                    // seasNum = Convert::ToInt32(s);
                    // remove season folder from end of the path
                    sp = Regex.Replace(sp, "(.*)\\\\" + seasonFinder, "$1", RegexOptions.IgnoreCase);
                }
                catch
                {
                }
            }
            // assume last folder element is the show name
            showName = sp.Substring(sp.LastIndexOf(System.IO.Path.DirectorySeparatorChar.ToString()) + 1);

            ai.ShowName = showName;
        }

        public ProcessedEpisodeList NextNShows(int nshows, int ndays)
        {
            DateTime notBefore = DateTime.Now;
            ProcessedEpisodeList found = new ProcessedEpisodeList();

            this.LockShowItems();
            for (int i = 0; i < nshows; i++)
            {
                ProcessedEpisode nextAfterThat = null;
                TimeSpan howClose = TimeSpan.MaxValue;
                foreach (ShowItem si in this.GetShowItems(false))
                {
                    if (!si.ShowNextAirdate)
                        continue;
                    foreach (System.Collections.Generic.KeyValuePair<int, ProcessedEpisodeList> v in si.SeasonEpisodes)
                    {
                        if (si.IgnoreSeasons.Contains(v.Key))
                            continue; // ignore this season

                        foreach (ProcessedEpisode ei in v.Value)
                        {
                            if (found.Contains(ei))
                                continue;

                            DateTime? airdt = ei.GetAirDateDT(true);

                            if ((airdt == null) || (airdt == DateTime.MaxValue))
                                continue;
                            DateTime dt = (DateTime) airdt;

                            TimeSpan ts = dt.Subtract(notBefore);
                            TimeSpan timeUntil = dt.Subtract(DateTime.Now);
                            if (((howClose == TimeSpan.MaxValue) || (ts.CompareTo(howClose) <= 0) && (ts.TotalHours >= 0)) && (ts.TotalHours >= 0) && (timeUntil.TotalDays <= ndays))
                            {
                                howClose = ts;
                                nextAfterThat = ei;
                            }
                        }
                    }
                }
                if (nextAfterThat == null)
                    return found;

                DateTime? nextdt = nextAfterThat.GetAirDateDT(true);
                notBefore = (DateTime) nextdt;
                found.Add(nextAfterThat);
            }
            this.UnlockShowItems();

            return found;
        }

        public static void WriteStringsToXml(StringList strings, XmlWriter writer, string elementName, string stringName)
        {
            writer.WriteStartElement(elementName);
            foreach (string ss in strings)
            {
                writer.WriteStartElement(stringName);
                writer.WriteValue(ss);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public static void Rotate(string filenameBase)
        {
            if (File.Exists(filenameBase))
            {
                for (int i = 8; i >= 0; i--)
                {
                    string fn = filenameBase + "." + i;
                    if (File.Exists(fn))
                    {
                        string fn2 = filenameBase + "." + (i + 1);
                        if (File.Exists(fn2))
                            File.Delete(fn2);
                        File.Move(fn, fn2);
                    }
                }

                File.Copy(filenameBase, filenameBase + ".0");
            }
        }

        public void WriteXMLSettings()
        {
            // backup old settings before writing new ones

            Rotate(TVDocSettingsFile().FullName);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            XmlWriter writer = XmlWriter.Create(TVDocSettingsFile().FullName, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("TVRename");
            writer.WriteStartAttribute("Version");
            writer.WriteValue("2.1");
            writer.WriteEndAttribute(); // version

            this.Settings.WriteXML(writer); // <Settings>

            writer.WriteStartElement("MyShows");
            foreach (ShowItem si in ShowItems)
                si.WriteXMLSettings(writer);
            writer.WriteEndElement(); // myshows

            WriteStringsToXml(this.MonitorFolders, writer, "MonitorFolders", "Folder");
            WriteStringsToXml(this.IgnoreFolders, writer, "IgnoreFolders", "Folder");
            WriteStringsToXml(this.SearchFolders, writer, "FinderSearchFolders", "Folder");

            writer.WriteStartElement("IgnoreItems");
            foreach (IgnoreItem ii in this.Ignore)
                ii.Write(writer);
            writer.WriteEndElement(); // IgnoreItems

            writer.WriteEndElement(); // tvrename
            writer.WriteEndDocument();
            writer.Close();
            writer = null;

            this.mDirty = false;

            this.Stats().Save();
        }

        public static StringList ReadStringsFromXml(XmlReader reader, string elementName, string stringName)
        {
            StringList r = new StringList();

            if (reader.Name != elementName)
                return r; // uhoh

            if (!reader.IsEmptyElement)
            {
                reader.Read();
                while (!reader.EOF)
                {
                    if ((reader.Name == elementName) && !reader.IsStartElement())
                        break;
                    if (reader.Name == stringName)
                        r.Add(reader.ReadElementContentAsString());
                    else
                        reader.ReadOuterXml();
                }
            }
            reader.Read();
            return r;
        }

        public bool LoadXMLSettings(FileInfo from)
        {
            if (from == null)
                return true;

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;

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
                    this.LoadErr = from.Name + " : Not a valid XML file";
                    return false;
                }

                reader.Read();

                if (reader.Name != "TVRename")
                {
                    this.LoadErr = from.Name + " : Not a TVRename settings file";
                    return false;
                }

                if (reader.GetAttribute("Version") != "2.1")
                {
                    this.LoadErr = from.Name + " : Incompatible version";
                    return false;
                }

                reader.Read(); // move forward one

                while (!reader.EOF)
                {
                    if (reader.Name == "TVRename" && !reader.IsStartElement())
                        break; // end of it all

                    if (reader.Name == "Settings")
                    {
                        this.Settings = new TVSettings(reader.ReadSubtree());
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
                                ShowItem si = new ShowItem(this.mTVDB, r2.ReadSubtree(), this.Settings);

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
                        this.MonitorFolders = ReadStringsFromXml(reader, "MonitorFolders", "Folder");
                    else if (reader.Name == "IgnoreFolders")
                        this.IgnoreFolders = ReadStringsFromXml(reader, "IgnoreFolders", "Folder");
                    else if (reader.Name == "FinderSearchFolders")
                        this.SearchFolders = ReadStringsFromXml(reader, "FinderSearchFolders", "Folder");
                    else if (reader.Name == "IgnoreItems")
                    {
                        XmlReader r2 = reader.ReadSubtree();
                        r2.Read();
                        r2.Read();
                        while (r2.Name == "Ignore")
                            this.Ignore.Add(new IgnoreItem(r2));
                        reader.Read();
                    }
                    else
                        reader.ReadOuterXml();
                }

                reader.Close();
                reader = null;
            }
            catch (Exception e)
            {
                this.LoadErr = from.Name + " : " + e.Message;
                return false;
            }

            try
            {
                this.Stats().Load();
            }
            catch (Exception)
            {
                // not worried if stats loading fails
            }
            return true;
        }

        public static bool SysOpen(string what)
        {
            try
            {
                System.Diagnostics.Process.Start(what);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //            
        //			void ExportMissingCSV(String ^filename)
        //			{
        //			TextWriter ^f = gcnew StreamWriter(filename);
        //			String ^line;
        //			line = "Show Name,Season,Episode,Episode Name,Air Date,Folder,Nice Name,thetvdb.com Code";
        //			f->WriteLine(line);
        //			for each (MissingEpisode ^me in MissingEpisodes)
        //			{
        //			DateTime ^dt = me->GetAirDateDT(true);
        //			String ^line = "\"" + me->TheSeries->Name + "\"" + "," + 
        //			me->SeasonNumber + "," + me->EpNum + 
        //			((me->EpNum != me->EpNum2) ? "-"+me->EpNum2 : "") +
        //			"," +
        //			"\"" + me->Name + "\"" + "," +
        //			((dt != nullptr) ? dt->ToString("G") : "") + "," +
        //			"\"" + me->WhereItBelongs + "\"" + "," +
        //			"\"" + FilenameFriendly(Settings->NamingStyle->NameFor(me)) + "\"" + "," +
        //			me->SeriesID;
        //            //(NStyle::Style)me->SI->NamingStyle
        //
        //			f->WriteLine(line);
        //			}
        //			f->Close();
        //			}
        //			
        //            
        //			void ExportRCXML(String ^name, String ^filename, RCList ^list)
        //			{
        //			XmlWriterSettings ^settings = gcnew XmlWriterSettings();
        //			settings->Indent = true;
        //			settings->NewLineOnAttributes = true;
        //			XmlWriter ^writer = XmlWriter::Create(filename, settings);
        //
        //			writer->WriteStartDocument();
        //			writer->WriteStartElement("TVRename");
        //			writer->WriteStartAttribute("Version");
        //			writer->WriteValue("2.1");
        //			writer->WriteEndAttribute(); // version
        //			writer->WriteStartElement(name);
        //
        //
        //			for each (RCItem ^r in list)
        //			{
        //			writer->WriteStartElement("Item");
        //
        //			writer->WriteStartElement("Operation");
        //			writer->WriteValue(r->GetOperationName());
        //			writer->WriteEndElement();
        //			writer->WriteStartElement("FromFolder");
        //			writer->WriteValue(r->FromFolder);
        //			writer->WriteEndElement();
        //			writer->WriteStartElement("FromName");
        //			writer->WriteValue(r->FromName);
        //			writer->WriteEndElement();
        //			writer->WriteStartElement("ToFolder");
        //			writer->WriteValue(r->ToFolder);
        //			writer->WriteEndElement();
        //			writer->WriteStartElement("ToName");
        //			writer->WriteValue(r->ToName);
        //			writer->WriteEndElement();
        //			writer->WriteStartElement("ShowName");
        //			writer->WriteValue(r->ShowName);
        //			writer->WriteEndElement();	
        //			writer->WriteStartElement("Season");
        //			if (r->TheEpisode != nullptr)
        //			writer->WriteValue(r->TheEpisode->SeasonNumber);
        //			writer->WriteEndElement();
        //			writer->WriteStartElement("EpNum");
        //			if (r->TheEpisode != nullptr)
        //			writer->WriteValue(r->TheEpisode->EpNum);
        //			writer->WriteEndElement();
        //			writer->WriteStartElement("EpNum2");
        //			if ((r->TheEpisode != nullptr) && (r->TheEpisode->EpNum != r->TheEpisode->EpNum2) )
        //			writer->WriteValue(r->TheEpisode->EpNum2);
        //			writer->WriteEndElement();
        //
        //			writer->WriteEndElement(); //Item
        //			}
        //
        //			writer->WriteEndElement(); // "name"
        //			writer->WriteEndElement(); // tvrename
        //			writer->WriteEndDocument();
        //			writer->Close();
        //			}
        //			void ExportRenamingXML(String ^filename)
        //			{
        //			ExportRCXML("Renaming", filename, RenameList);
        //			}
        //			void ExportFOXML(String ^filename)
        //			{
        //			ExportRCXML("FindingAndOrganising", filename, CopyMoveList);
        //			}
        //			void ExportMissingXML(String ^filename)
        //			{
        //			XmlWriterSettings ^settings = gcnew XmlWriterSettings();
        //			settings->Indent = true;
        //			settings->NewLineOnAttributes = true;
        //			XmlWriter ^writer = XmlWriter::Create(filename, settings);
        //
        //			writer->WriteStartDocument();
        //			writer->WriteStartElement("TVRename");
        //			writer->WriteStartAttribute("Version");
        //			writer->WriteValue("2.1");
        //			writer->WriteEndAttribute(); // version
        //			writer->WriteStartElement("MissingItems");
        //
        //			for each (MissingEpisode ^me in MissingEpisodes)
        //			{
        //			writer->WriteStartElement("MissingItem");
        //			writer->WriteStartElement("ShowName");
        //			writer->WriteValue(me->SI->ShowName());
        //			writer->WriteEndElement();
        //			writer->WriteStartElement("Season");
        //			writer->WriteValue(me->Season);
        //			writer->WriteEndElement();
        //			writer->WriteStartElement("Episode");
        //			String ^epl = me->EpNum.ToString();
        //			if (me->EpNum != me->EpNum2)
        //			epl += "-"+me->EpNum2.ToString();
        //			writer->WriteValue(epl);
        //			writer->WriteEndElement();
        //			writer->WriteStartElement("EpisodeName");
        //			writer->WriteValue(me->Name);
        //			writer->WriteEndElement();
        //			writer->WriteStartElement("AirDate");
        //			DateTime ^dt = me->GetAirDateDT(true);
        //			if (dt != nullptr)
        //			writer->WriteValue(dt);
        //			writer->WriteEndElement();
        //			writer->WriteStartElement("Folder");
        //			writer->WriteValue(me->WhereItBelongs);
        //			writer->WriteEndElement();
        //			writer->WriteStartElement("NiceName");
        //			writer->WriteValue(FilenameFriendly(Settings->NamingStyle->NameFor(me)));
        //			writer->WriteEndElement();
        //			writer->WriteStartElement("thetvdbID");
        //			writer->WriteValue( me->SI->TVDBCode);
        //			writer->WriteEndElement();
        //			writer->WriteEndElement(); // MissingItem
        //			}
        //
        //			writer->WriteEndElement(); // MissingItems
        //			writer->WriteEndElement(); // tvrename
        //			writer->WriteEndDocument();
        //			writer->Close();
        //			}
        //			
        public bool GenerateUpcomingXML(Stream str, ProcessedEpisodeList elist)
        {
            if (elist == null)
                return false;

            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.NewLineOnAttributes = true;
                settings.Encoding = System.Text.Encoding.ASCII;
                XmlWriter writer = XmlWriter.Create(str, settings);

                writer.WriteStartDocument();
                writer.WriteStartElement("rss");
                writer.WriteStartAttribute("version");
                writer.WriteValue("2.0");
                writer.WriteEndAttribute();
                writer.WriteStartElement("channel");
                writer.WriteStartElement("title");
                writer.WriteValue("Upcoming Shows");
                writer.WriteEndElement();
                writer.WriteStartElement("title");
                writer.WriteValue("http://tvrename.com");
                writer.WriteEndElement();
                writer.WriteStartElement("description");
                writer.WriteValue("Upcoming shows, exported by TVRename");
                writer.WriteEndElement();

                foreach (ProcessedEpisode ei in elist)
                {
                    string niceName = this.Settings.NamingStyle.NameForExt(ei, null);

                    writer.WriteStartElement("item");
                    writer.WriteStartElement("title");
                    writer.WriteValue(ei.HowLong() + " " + ei.DayOfWeek() + " " + ei.TimeOfDay() + " " + niceName);
                    writer.WriteEndElement();
                    writer.WriteStartElement("link");
                    writer.WriteValue(this.GetTVDB(false, "").WebsiteURL(ei.TheSeries.TVDBCode, ei.SeasonID, false));
                    writer.WriteEndElement();
                    writer.WriteStartElement("description");
                    writer.WriteValue(niceName + "<br/>" + ei.Overview);
                    writer.WriteEndElement();
                    writer.WriteStartElement("pubDate");
                    DateTime? dt = ei.GetAirDateDT(true);
                    if (dt != null)
                        writer.WriteValue(dt.Value.ToString("r"));
                    writer.WriteEndElement();
                    writer.WriteEndElement(); // item
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
                return true;
            } // try
            catch
            {
                return false;
            }
        }

        public void WriteUpcomingRSS()
        {
            if (!this.Settings.ExportWTWRSS)
                return;

            try
            {
                // dirty try/catch to "fix" the problem that a share can disappear during a sleep/resume, and
                // when windows restarts, the share isn't "back" before this timer times out and fires
                // windows explorer tends to lose explorer windows on shares when slept/resumed, too, so its not
                // just me :P
                MemoryStream ms = new MemoryStream();
                if (this.GenerateUpcomingXML(ms, this.NextNShows(this.Settings.ExportRSSMaxShows, this.Settings.ExportRSSMaxDays)))
                {
                    StreamWriter sw = new StreamWriter(this.Settings.ExportWTWRSSTo);
                    sw.Write(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
                    sw.Close();
                }
            }
            catch
            {
            }
        }

        // see if showname is somewhere in filename
        public bool SimplifyAndCheckFilename(string filename, string showname, bool simplifyfilename, bool simplifyshowname)
        {
            return Regex.Match(simplifyfilename ? Helpers.SimplifyName(filename) : filename, "\\b" + (simplifyshowname ? Helpers.SimplifyName(showname) : showname) + "\\b", RegexOptions.IgnoreCase).Success;
        }

        public void CheckAgainstuTorrent(SetProgressDelegate prog)
        {
            // get list of files being downloaded by uTorrent
            string resDatFile = this.Settings.ResumeDatPath;
            if (string.IsNullOrEmpty(resDatFile) || !File.Exists(resDatFile))
                return;

            BTResume btr = new BTResume(prog, resDatFile);
            if (!btr.LoadResumeDat())
                return;

            System.Collections.Generic.List<TorrentEntry> downloading = btr.AllFilesBeingDownloaded(Settings);

            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();
            int c = this.TheActionList.Count + 2;
            int n = 1;
            prog.Invoke(100 * n / c);
            foreach (Item Action1 in this.TheActionList)
            {
                if (this.ActionCancel)
                    return;

                n++;
                prog.Invoke(100 * n / c);

                if (!(Action1 is ActionMissing))
                    continue;

                ActionMissing Action = (ActionMissing) (Action1);

                string showname = Helpers.SimplifyName(Action.Episode.SI.ShowName());

                foreach (TorrentEntry te in downloading)
                {
                    FileInfo file = new FileInfo(te.DownloadingTo);
                    if (!this.Settings.UsefulExtension(file.Extension, false)) // not a usefile file extension
                        continue;

                    // String ^simplifiedfname = Helpers.SimplifyName(file->FullName);

                    if (this.SimplifyAndCheckFilename(file.FullName, showname, true, false)) // if (Regex::Match(simplifiedfname,"\\b"+showname+"\\b",RegexOptions::IgnoreCase)->Success)
                    {
                        int seasF;
                        int epF;
                        if (this.FindSeasEp(file, out seasF, out epF, Action.Episode.TheSeries.Name) && (seasF == Action.Episode.SeasonNumber) && (epF == Action.Episode.EpNum))
                        {
                            toRemove.Add(Action1);
                            newList.Add(new ActionuTorrenting(te, Action.Episode, Action.TheFileNoExt));
                            break;
                        }
                    }
                }
            }

            foreach (Item i in toRemove)
                this.TheActionList.Remove(i);

            foreach (Item Action in newList)
                this.TheActionList.Add(Action);

            prog.Invoke(100);
        }

        public void RSSSearch(SetProgressDelegate prog)
        {
            int c = this.TheActionList.Count + 2;
            int n = 1;
            prog.Invoke(100 * n / c);
            this.RSSList = new RSSItemList();
            foreach (string s in this.Settings.RSSURLs)
                this.RSSList.DownloadRSS(s, this.Settings.FNPRegexs);

            ItemList newItems = new ItemList();
            ItemList toRemove = new ItemList();

            foreach (Item Action1 in this.TheActionList)
            {
                if (this.ActionCancel)
                    return;

                n++;
                prog.Invoke(100 * n / c);

                if (!(Action1 is ActionMissing))
                    continue;

                ActionMissing Action = (ActionMissing) (Action1);

                ProcessedEpisode pe = Action.Episode;
                string simpleShowName = Helpers.SimplifyName(pe.SI.ShowName());
                string simpleSeriesName = Helpers.SimplifyName(pe.TheSeries.Name);

                foreach (RSSItem rss in this.RSSList)
                {
                    if ((this.SimplifyAndCheckFilename(rss.ShowName, simpleShowName, true, false) || (string.IsNullOrEmpty(rss.ShowName) && this.SimplifyAndCheckFilename(rss.Title, simpleSeriesName, true, false))) && (rss.Season == pe.SeasonNumber) && (rss.Episode == pe.EpNum))
                    {
                        newItems.Add(new ActionRSS(rss, Action.TheFileNoExt, pe));
                        toRemove.Add(Action1);
                    }
                }
            }
            foreach (Item i in toRemove)
                this.TheActionList.Remove(i);
            foreach (Item Action in newItems)
                this.TheActionList.Add(Action);

            prog.Invoke(100);
        }

        private Thread ActionProcessorThread;

        public void ProcessSingleAction(Object actionIn)
        {
            System.Diagnostics.Debug.Assert(this.ActionSemaphore != null);
            this.ActionSemaphore.WaitOne(); // don't start until we're allowed to

            Action action = actionIn as Action;
            if (action != null)
                action.Go(this.Settings, ref ActionPause);

            this.ActionSemaphore.Release(1);
        }

        System.Collections.Generic.List<Thread> ActionWorkers;
        Semaphore ActionSemaphore;
        public bool ActionPause;

        public void ActionProcessor(Object theListIn)
        {
            // TODO: Run tasks in parallel (as much as is sensible)

            this.ActionPause = false;
            ScanListItemList theList = theListIn as ScanListItemList;
            if (theList == null)
                return;

            this.ActionWorkers = new System.Collections.Generic.List<Thread>();
            this.ActionSemaphore = new Semaphore(1, 1); // allow up to numWorkers working at once

            try
            {
                foreach (Item item in theList)
                {
                    Action act = item as Action;

                    if (act == null)
                        continue;

                    if (!act.Done)
                    {
                        this.ActionSemaphore.WaitOne();
                        Thread t = new Thread(this.ProcessSingleAction)
                        {
                            Name = "ProcessSingleAction"
                        };
                        this.ActionWorkers.Add(t);
                        t.Start(act);
                        int nfr = this.ActionSemaphore.Release(1); // release our hold on the semaphore, so that worker can grab it
                        System.Diagnostics.Debug.Print("ActionProcessor pool has " + nfr + " free");
                    }

                    Thread.Sleep(1); // allow the other thread a chance to run and grab

                    // tidy up any finished workers
                    for (int i = this.ActionWorkers.Count - 1; i >= 0; i--)
                    {
                        if (!this.ActionWorkers[i].IsAlive)
                            this.ActionWorkers.RemoveAt(i); // remove dead worker
                    }

                }
                this.WaitForAllActionThreadsAndTidyUp();
            }
            catch (ThreadAbortException)
            {
                foreach (Thread t in ActionWorkers)
                    t.Abort();
                this.WaitForAllActionThreadsAndTidyUp();
            }
        }

        private void WaitForAllActionThreadsAndTidyUp()
        {
            if (this.ActionWorkers != null)
            {
                foreach (Thread t in this.ActionWorkers)
                {
                    if (t.IsAlive)
                        t.Join();
                }
            }

            this.ActionWorkers = null;
            this.ActionSemaphore = null;
        }

        //public void AbortActionProcessing()
        //{
        //    if (ActionProcessorThread != null)
        //        ActionProcessorThread.Abort();
        //}

        public void DoActions(ScanListItemList theList)
        {
            // If not /hide, show CopyMoveProgress dialog
            
            CopyMoveProgress cmp = null;
            if (!this.Args.Hide)
                cmp = new CopyMoveProgress(this, theList, this.Stats());

            ActionProcessorThread = new Thread(this.ActionProcessor)
            {
                Name = "ActionProcessorThread"
            };

            ActionProcessorThread.Start(theList);

            if ((cmp != null) && (cmp.ShowDialog() == DialogResult.Cancel))
                ActionProcessorThread.Abort();
            
            ActionProcessorThread.Join();

            theList.RemoveAll(x => (x is Action) && (x as Action).Done && !(x as Action).Error);
        }

        public bool ListHasMissingItems(ItemList l)
        {
            foreach (Item i in l)
            {
                if (i is ActionMissing)
                    return true;
            }
            return false;
        }

        public void ActionGo(ShowItem specific)
        {
            if (this.Settings.MissingCheck && !this.CheckAllFoldersExist(specific)) // only check for folders existing for missing check
                return;

            if (!this.DoDownloadsFG())
                return;

            Thread ActionWork = new Thread(this.ScanWorker);
            ActionWork.Name = "ActionWork";

            this.ActionCancel = false;

            if (!this.Args.Hide)
                this.ScanProgDlg = new ScanProgress(this.Settings.RenameCheck, this.Settings.MissingCheck && this.Settings.SearchLocally, this.Settings.MissingCheck && this.Settings.CheckuTorrent, this.Settings.MissingCheck && this.Settings.SearchRSS);
            else
                this.ScanProgDlg = null;

            ActionWork.Start(specific);

            if ((this.ScanProgDlg != null) && (this.ScanProgDlg.ShowDialog() == DialogResult.Cancel))
            {
                this.ActionCancel = true;
                ActionWork.Interrupt();
            }
            else
                ActionWork.Join();

            this.ScanProgDlg = null;
        }

        public bool CheckAllFoldersExist(ShowItem specific)
        {
            // show MissingFolderAction for any folders that are missing
            // return false if user cancels

            this.LockShowItems();
            System.Collections.Generic.List<ShowItem> showlist;

            if (specific != null)
            {
                showlist = new ShowItemList();
                showlist.Add(specific);
            }
            else
                showlist = ShowItems;

            foreach (ShowItem si in showlist)
            {
                if (!si.DoMissingCheck && !si.DoRename)
                    continue; // skip

                System.Collections.Generic.Dictionary<int, StringList> flocs = si.AllFolderLocations(this.Settings);

                int[] numbers = new int[si.SeasonEpisodes.Keys.Count];
                si.SeasonEpisodes.Keys.CopyTo(numbers, 0);
                foreach (int snum in numbers)
                {
                    if (si.IgnoreSeasons.Contains(snum))
                        continue; // ignore this season

                    //int snum = kvp->Key;
                    if ((snum == 0) && (si.CountSpecials))
                        continue; // no specials season, they're merged into the seasons themselves

                    StringList folders = new StringList();

                    if (flocs.ContainsKey(snum))
                        folders = flocs[snum];

                    if ((folders.Count == 0) && (!si.AutoAddNewSeasons))
                        continue; // no folders defined or found, autoadd off, so onto the next

                    if (folders.Count == 0)
                    {
                        // no folders defined for this season, and autoadd didn't find any, so suggest the autoadd folder name instead
                        folders.Add(si.AutoFolderNameForSeason(snum, this.Settings));
                    }

                    foreach (string folderFE in folders)
                    {
                        String folder = folderFE;

                        // generate new filename info
                        bool goAgain = false;
                        DirectoryInfo di = null;
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
                                    return false;
                                }
                            }
                            if ((di == null) || (!di.Exists))
                            {
                                string sn = si.ShowName();
                                string text = snum + " of " + si.MaxSeason();
                                string theFolder = folder;
                                string otherFolder = null;

                                FAResult whatToDo = FAResult.kfaNotSet;

                                if (this.Args.MissingFolder == CommandLineArgs.MissingFolderBehaviour.Create)
                                    whatToDo = FAResult.kfaCreate;
                                else if (this.Args.MissingFolder == CommandLineArgs.MissingFolderBehaviour.Ignore)
                                    whatToDo = FAResult.kfaIgnoreOnce;

                                if (this.Args.Hide && (whatToDo == FAResult.kfaNotSet))
                                    whatToDo = FAResult.kfaIgnoreOnce; // default in /hide mode is to ignore

                                if (whatToDo == FAResult.kfaNotSet)
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

                                if (whatToDo == FAResult.kfaCancel)
                                {
                                    this.UnlockShowItems();
                                    return false;
                                }
                                else if (whatToDo == FAResult.kfaCreate)
                                {
                                    Directory.CreateDirectory(folder);
                                    goAgain = true;
                                }
                                else if (whatToDo == FAResult.kfaIgnoreAlways)
                                {
                                    si.IgnoreSeasons.Add(snum);
                                    this.SetDirty();
                                    break;
                                }
                                else if (whatToDo == FAResult.kfaIgnoreOnce)
                                    break;
                                else if (whatToDo == FAResult.kfaRetry)
                                    goAgain = true;
                                else if (whatToDo == FAResult.kfaDifferentFolder)
                                {
                                    folder = otherFolder;
                                    di = new DirectoryInfo(folder);
                                    goAgain = !di.Exists;
                                    if (di.Exists && (si.AutoFolderNameForSeason(snum, this.Settings).ToLower() != folder.ToLower()))
                                    {
                                        if (!si.ManualFolderLocations.ContainsKey(snum))
                                            si.ManualFolderLocations[snum] = new StringList();
                                        si.ManualFolderLocations[snum].Add(folder);
                                        this.SetDirty();
                                    }
                                }
                            }
                        }
                        while (goAgain);
                    } // for each folder
                } // for each snum
            } // for each show
            this.UnlockShowItems();

            return true;
        }

        // CheckAllFoldersExist

        public void RemoveIgnored()
        {
            ItemList toRemove = new ItemList();
            foreach (Item item in this.TheActionList)
            {
                ScanListItem act = item as ScanListItem;
                foreach (IgnoreItem ii in this.Ignore)
                {
                    if (ii.SameFileAs(act.Ignore))
                    {
                        toRemove.Add(item);
                        break;
                    }
                }
            }
            foreach (Item Action in toRemove)
                this.TheActionList.Remove(Action);
        }

        public void RenameAndMissingCheck(SetProgressDelegate prog, ShowItem specific)
        {
            this.TheActionList = new ItemList();

            //int totalEps = 0;

            this.LockShowItems();

            System.Collections.Generic.List<ShowItem> showlist;
            if (specific != null)
            {
                showlist = new ShowItemList();
                showlist.Add(specific);
            }
            else
                showlist = ShowItems;

            //foreach (ShowItem si in showlist)
            //  if (si.DoRename)
            //    totalEps += si.SeasonEpisodes.Count;

            if (this.Settings.RenameCheck)
                this.Stats().RenameChecksDone++;

            if (this.Settings.MissingCheck)
                this.Stats().MissingChecksDone++;

            prog.Invoke(0);
            
            if (specific == null) // only do episode count if we're doing all shows and seasons
                this.mStats.NS_NumberOfEpisodes = 0;

            int c = 0;
            foreach (ShowItem si in showlist)
            {
                if (this.ActionCancel)
                    return;
                c++;

                prog.Invoke(100 * c / showlist.Count);

                if (si.AllFolderLocations(this.Settings).Count == 0) // no folders defined for this show
                    continue; // so, nothing to do.

                // for each tv show, optionally write a tvshow.nfo file

                if (this.Settings.NFOs && !string.IsNullOrEmpty(si.AutoAdd_FolderBase) && (si.AllFolderLocations(this.Settings).Count > 0))
                {
                    FileInfo tvshownfo = Helpers.FileInFolder(si.AutoAdd_FolderBase, "tvshow.nfo");

                    bool needUpdate = !tvshownfo.Exists || (si.TheSeries().Srv_LastUpdated > TimeZone.Epoch(tvshownfo.LastWriteTime));
                    // was it written before we fixed the bug in <episodeguideurl> ?
                    needUpdate = needUpdate || (tvshownfo.LastWriteTime.ToUniversalTime().CompareTo(new DateTime(2009, 9, 13, 7, 30, 0, 0, DateTimeKind.Utc)) < 0);
                    if (needUpdate)
                        this.TheActionList.Add(new ActionNFO(tvshownfo, si));
                }

                // process each folder for each season...

                int[] numbers = new int[si.SeasonEpisodes.Keys.Count];
                si.SeasonEpisodes.Keys.CopyTo(numbers, 0);
                System.Collections.Generic.Dictionary<int, StringList> allFolders = si.AllFolderLocations(this.Settings);

                foreach (int snum in numbers)
                {
                    if (this.ActionCancel)
                        return;

                    if ((si.IgnoreSeasons.Contains(snum)) || (!allFolders.ContainsKey(snum)))
                        continue; // ignore/skip this season

                    if ((snum == 0) && (si.CountSpecials))
                        continue; // don't process the specials season, as they're merged into the seasons themselves

                    // all the folders for this particular season
                    StringList folders = allFolders[snum];

                    bool folderNotDefined = (folders.Count == 0);
                    if (folderNotDefined && (this.Settings.MissingCheck && !si.AutoAddNewSeasons) || !this.Settings.MissingCheck)
                        continue; // folder for the season is not defined, and we're not auto-adding it

                    ProcessedEpisodeList eps = si.SeasonEpisodes[snum];
                    int maxEpisodeNumber = 0;
                    foreach (ProcessedEpisode episode in eps)
                    {
                        if (episode.EpNum > maxEpisodeNumber)
                            maxEpisodeNumber = episode.EpNum;
                    }

                    StringList doneFolderJPG = new StringList();
                    if (this.Settings.FolderJpg)
                    {
                        // main image for the folder itself

                        // base folder:
                        if (!string.IsNullOrEmpty(si.AutoAdd_FolderBase) && (si.AllFolderLocations(this.Settings, false).Count > 0))
                        {
                            FileInfo fi = Helpers.FileInFolder(si.AutoAdd_FolderBase, "folder.jpg");
                            if (!fi.Exists)
                            {
                                string bannerPath = si.TheSeries().GetItem(this.Settings.ItemForFolderJpg());
                                if (!string.IsNullOrEmpty(bannerPath))
                                    this.TheActionList.Add(new ActionDownload(si, null, fi, bannerPath));
                            }
                            doneFolderJPG.Add(si.AutoAdd_FolderBase);
                        }
                    }

                    foreach (string folder in folders)
                    {
                        if (this.ActionCancel)
                            return;

                        // generate new filename info
                        DirectoryInfo di;
                        try
                        {
                            di = new DirectoryInfo(folder);
                        }
                        catch
                        {
                            continue; // TODO: show an error?
                        }

                        bool renCheck = this.Settings.RenameCheck && si.DoRename && di.Exists; // renaming check needs the folder to exist
                        bool missCheck = this.Settings.MissingCheck && si.DoMissingCheck;

                        if (this.Settings.FolderJpg)
                        {
                            // season folders JPGs

                            if (!doneFolderJPG.Contains(folder)) // some folders may come up multiple times
                            {
                                doneFolderJPG.Add(folder);

                                FileInfo fi = Helpers.FileInFolder(folder, "folder.jpg");
                                if (!fi.Exists)
                                {
                                    string bannerPath = si.TheSeries().GetItem(this.Settings.ItemForFolderJpg());
                                    if (!string.IsNullOrEmpty(bannerPath))
                                        this.TheActionList.Add(new ActionDownload(si, null, fi, bannerPath));
                                }
                            }
                        }

                        FileInfo[] files = di.GetFiles(); // all the files in the folder
                        FileInfo[] localEps = new FileInfo[maxEpisodeNumber + 1];

                        int maxEpNumFound = 0;

                        foreach (FileInfo fi in files)
                        {
                            if (this.ActionCancel)
                                return;

                            int seasNum;
                            int epNum;

                            if (!this.FindSeasEp(fi, out seasNum, out epNum, si.ShowName()))
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

                            if (renCheck && this.Settings.UsefulExtension(fi.Extension, true)) // == RENAMING CHECK ==
                            {
                                string newname = this.Settings.FilenameFriendly(this.Settings.NamingStyle.NameForExt(ep, fi.Extension));
                                if (newname != actualFile.Name)
                                {
                                    actualFile = Helpers.FileInFolder(folder, newname); // rename updates the filename
                                    this.TheActionList.Add(new ActionCopyMoveRename(ActionCopyMoveRename.Op.Rename, fi, actualFile, ep));
                                }
                            }
                            if (missCheck && this.Settings.UsefulExtension(fi.Extension, false)) // == MISSING CHECK part 1/2 ==
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
                            TheTVDB db = this.GetTVDB(true, "UpToDateCheck");
                            foreach (ProcessedEpisode dbep in eps)
                            {
                                if ((dbep.EpNum > maxEpNumFound) || (localEps[dbep.EpNum] == null)) // not here locally
                                {
                                    DateTime? dt = dbep.GetAirDateDT(true);

                                    bool notFuture = ((dt != null) && (dt.Value.CompareTo(today) < 0)); // isn't an episode yet to be aired
                                    
                                    bool noAirdatesUntilNow = true;
                                    for (int i = 1; i <= snum; i++)
                                        if (this.HasAnyAirdates(si, i))
                                        {
                                            noAirdatesUntilNow = false;
                                            break;
                                        }

                                    // only add to the missing list if, either:
                                    // - force check is on
                                    // - there are no airdates at all, for up to and including this season
                                    // - there is an airdate, and it isn't in the future
                                    if (noAirdatesUntilNow || (si.ForceCheckFuture || notFuture) || (si.ForceCheckNoAirdate && dt == null))
                                    {
                                        // then add it as officially missing
                                        this.TheActionList.Add(new ActionMissing(dbep, folder + System.IO.Path.DirectorySeparatorChar + this.Settings.FilenameFriendly(this.Settings.NamingStyle.NameForExt(dbep, null))));
                                    }
                                }
                                else
                                {
                                    // the file is here
                                    if (specific == null)
                                      this.mStats.NS_NumberOfEpisodes++;

                                    // do NFO and thumbnail checks if required
                                    FileInfo filo = localEps[dbep.EpNum]; // filename (or future filename) of the file

                                    this.ThumbnailAndNFOCheck(dbep, filo, this.TheActionList);
                                }
                            } // up to date check, for each episode in thetvdb
                            db.Unlock("UpToDateCheck");
                        } // if doing missing check
                    } // for each folder for this season of this show
                } // for each season of this show
            } // for each show

            this.UnlockShowItems();
            this.RemoveIgnored();
        }

        private void ThumbnailAndNFOCheck(ProcessedEpisode dbep, FileInfo filo, ItemList addTo)
        {
            if (this.Settings.EpImgs)
            {
                string ban = dbep.GetItem("filename");
                if (!string.IsNullOrEmpty(ban))
                {
                    string fn = filo.Name;
                    fn = fn.Substring(0, fn.Length - filo.Extension.Length);
                    fn += ".tbn";
                    FileInfo img = Helpers.FileInFolder(filo.Directory, fn);
                    if (!img.Exists)
                        addTo.Add(new ActionDownload(dbep.SI, dbep, img, ban));
                }
            }
            if (this.Settings.NFOs)
            {
                string fn = filo.Name;
                fn = fn.Substring(0, fn.Length - filo.Extension.Length);
                fn += ".nfo";
                FileInfo nfo = Helpers.FileInFolder(filo.Directory, fn);

                if (!nfo.Exists || (dbep.Srv_LastUpdated > TimeZone.Epoch(nfo.LastWriteTime)))
                    addTo.Add(new ActionNFO(nfo, dbep));
            }
        }

        public void NoProgress(int pct)
        {
        }

        public void ScanWorker(Object o)
        {
            ShowItem specific = (ShowItem) (o);

            while (!this.Args.Hide && ((this.ScanProgDlg == null) || (!this.ScanProgDlg.Ready)))
                Thread.Sleep(10); // wait for thread to create the dialog

            this.TheActionList = new ItemList();
            SetProgressDelegate noProgress = this.NoProgress;

            if (this.Settings.RenameCheck || this.Settings.MissingCheck)
                this.RenameAndMissingCheck(this.ScanProgDlg == null ? noProgress : this.ScanProgDlg.MediaLibProg, specific);

            if (this.Settings.MissingCheck)
            {
                if (this.ActionCancel)
                    return;

                // have a look around for any missing episodes

                if (this.Settings.SearchLocally && this.ListHasMissingItems(this.TheActionList))
                {
                    this.LookForMissingEps(this.ScanProgDlg == null ? noProgress : this.ScanProgDlg.LocalSearchProg);
                    this.RemoveIgnored();
                }

                if (this.ActionCancel)
                    return;

                if (this.Settings.CheckuTorrent && this.ListHasMissingItems(this.TheActionList))
                {
                    this.CheckAgainstuTorrent(this.ScanProgDlg == null ? noProgress : this.ScanProgDlg.uTorrentProg);
                    this.RemoveIgnored();
                }

                if (this.ActionCancel)
                    return;

                if (this.Settings.SearchRSS && this.ListHasMissingItems(this.TheActionList))
                {
                    this.RSSSearch(this.ScanProgDlg == null ? noProgress : this.ScanProgDlg.RSSProg);
                    this.RemoveIgnored();
                }
            }
            if (this.ActionCancel)
                return;

            // sort Action list by type
            this.TheActionList.Sort(new ActionSorter()); // was new ActionSorter()

            if (this.ScanProgDlg != null)
                this.ScanProgDlg.Done();
        }

        public bool MatchesSequentialNumber(string filename, ref int seas, ref int ep, ProcessedEpisode pe)
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

        public static string SEFinderSimplifyFilename(string filename, string showNameHint)
        {
            // Look at showNameHint and try to remove the first occurance of it from filename
            // This is very helpful if the showname has a >= 4 digit number in it, as that
            // would trigger the 1302 -> 13,02 matcher
            // Also, shows like "24" can cause confusion

            filename = filename.Replace(".", " "); // turn dots into spaces

            if ((showNameHint == null) || (string.IsNullOrEmpty(showNameHint)))
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

        public bool FindSeasEp(FileInfo fi, out int seas, out int ep, string showNameHint)
        {
            return FindSeasEp(fi, out seas, out ep, showNameHint, this.Settings.FNPRegexs);
        }

        public static bool FindSeasEp(FileInfo fi, out int seas, out int ep, string showNameHint, FNPRegexList rexps)
        {
            string filename = fi.Name;
            int l = filename.Length;
            int le = fi.Extension.Length;
            filename = filename.Substring(0, l - le);
            return FindSeasEp(fi.Directory.FullName, filename, out seas, out ep, showNameHint, rexps);
        }

        public static bool FindSeasEp(string directory, string filename, out int seas, out int ep, string showNameHint, FNPRegexList rexps)
        {
            seas = ep = -1;

            filename = SEFinderSimplifyFilename(filename, showNameHint);

            string fullPath = directory + System.IO.Path.DirectorySeparatorChar + filename; // construct full path with sanitised filename

            if ((filename.Length > 256) || (fullPath.Length > 256))
                return false;

            int leftMostPos = filename.Length;

            filename = filename.ToLower() + " ";
            fullPath = fullPath.ToLower() + " ";

            foreach (FilenameProcessorRE re in rexps)
            {
                if (!re.Enabled)
                    continue;
                try
                {
                    Match m = Regex.Match(re.UseFullPath ? fullPath : filename, re.RE, RegexOptions.IgnoreCase);
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
            }

            return ((seas != -1) || (ep != -1));
        }
    }
}
