// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
// "Doc" is short for "Document", from the "Document" and "View" model way of thinking of things.
// All the processing and work should be done in here, nothing in UI.cs
// Means we can run TVRename and do useful stuff, without showing any UI. (i.e. text mode / console app)

using System;
using System.Collections.Generic;
using System.Globalization;
using Alphaleonis.Win32.Filesystem;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Xml;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using System.Text;

namespace TVRename
{
    public class TVDoc :IDisposable
    {

        private readonly DownloadIdentifiersController DownloadIdentifiers;
        private readonly List<Finder> Finders;

        public readonly ShowLibrary Library;
        public readonly CommandLineArgs Args;

        private TVRenameStats mStats;
        public ItemList TheActionList;

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly ActionEngine actionManager;
        private readonly CacheUpdater cacheManager;

        private bool ActionCancel;
        public string LoadErr;
        public bool LoadOK;
        private ScanProgress ScanProgDlg;

        private bool mDirty;
   
        public bool CurrentlyBusy = false;  // This is set to true when scanning and indicates to other objects not to commence a scan of their own


        public TVDoc(FileInfo settingsFile, CommandLineArgs args)
        {
            this.Args = args;

            this.Library = new ShowLibrary();
            this.mStats = new TVRenameStats();
            this.actionManager = new ActionEngine(this.mStats);
            this.cacheManager = new CacheUpdater();
            
            this.mDirty = false;
            this.TheActionList = new ItemList();

            this.ActionCancel = false;

            this.ScanProgDlg = null;

            this.Finders = new List<Finder>
            {
                new FileFinder(this),
                new RSSFinder(this),
                new uTorrentFinder(this),
                new SABnzbdFinder(this)
            };

            this.DownloadIdentifiers = new DownloadIdentifiersController();


            this.LoadOK = ((settingsFile == null) || LoadXMLSettings(settingsFile)) && TheTVDB.Instance.LoadOK;

            SetPreferredLanguage();
        }

        public TVRenameStats Stats()
        {
            this.mStats.NS_NumberOfShows = this.Library.Count;
            this.mStats.NS_NumberOfSeasons = 0;
            this.mStats.NS_NumberOfEpisodesExpected = 0;

            foreach (ShowItem si in this.Library.Shows)
            {
                foreach (KeyValuePair<int, List<ProcessedEpisode>> k in si.SeasonEpisodes)
                    this.mStats.NS_NumberOfEpisodesExpected += k.Value.Count;
                this.mStats.NS_NumberOfSeasons += si.SeasonEpisodes.Count;
            }

            return this.mStats;
        }

        public static void SetPreferredLanguage()
        {
            TheTVDB.Instance.RequestLanguage = TVSettings.Instance.PreferredLanguage;
        }

        public void SetDirty() => this.mDirty = true;

        public bool Dirty() => this.mDirty;

        public void DoActions(ItemList theList)
        {
            this.actionManager.DoActions(theList, !this.Args.Hide);
        }
        // ReSharper disable once InconsistentNaming
        public bool DoDownloadsFG()
        {
            ICollection<int> shows = this.Library.Keys;
            bool returnValue = this.cacheManager.DoDownloadsFG((!this.Args.Hide), (!this.Args.Unattended) && (!this.Args.Hide), shows);
            this.Library.GenDict();
            return returnValue;
        }

        // ReSharper disable once InconsistentNaming
        public void DoDownloadsBG()
        {
            ICollection<int> shows = this.Library.Keys;
            this.cacheManager.StartBGDownloadThread(false, shows);
        }

        public int DownloadsRemaining() =>
            this.cacheManager.DownloadDone ? 0 : this.cacheManager.DownloadsRemaining;


        internal static bool FindSeasEp(FileInfo theFile, out int seasF, out int epF, out int maxEp, ShowItem sI)
        {
            return FindSeasEp( theFile, out  seasF, out  epF, out  maxEp,  sI, out FilenameProcessorRE rex);
        }

        public void SetSearcher(int n)
        {
            TVSettings.Instance.TheSearchers.SetToNumber(n);
            SetDirty();
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

            BTFileRenamer btp = new BTFileRenamer(prog);
            ItemList newList = new ItemList();
            bool r = btp.RenameFilesOnDiskToMatchTorrent(torrent, folder, tvTree, newList, copyNotMove, copyDest, args);

            foreach (Item i in newList)
                this.TheActionList.Add(i);

            return r;
        }

        // -----------------------------------------------------------------------------

        public static Searchers GetSearchers() => TVSettings.Instance.TheSearchers;
        
        public void TidyTVDB()
        {
            // remove any shows from thetvdb that aren't in My Shows
            TheTVDB.Instance.GetLock("TidyTVDB");
            List<int> removeList = new List<int>();

            foreach (KeyValuePair<int, SeriesInfo> kvp in TheTVDB.Instance.GetSeriesDict())
            {
                bool found = this.Library.Values.Any(si => si.TVDBCode == kvp.Key);
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
            this.cacheManager.StopBGDownloadThread();
            Stats().Save();
        }

        public static void SearchForEpisode(ProcessedEpisode ep)
        {
            if (ep == null)
                return;
            Helpers.SysOpen(TVSettings.Instance.BTSearchURL(ep));
        }

        public void DoWhenToWatch(bool cachedOnly)
        {
            if (!cachedOnly && !DoDownloadsFG())
                return;
            if (cachedOnly)
                this.Library.GenDict();
        }

        public static List<FileInfo> FindEpOnDisk(DirFilesCache dfc, ProcessedEpisode pe,bool checkDirectoryExist = true)
        {
            return FindEpOnDisk(dfc, pe.SI, pe, checkDirectoryExist);
        }

        private static List<FileInfo> FindEpOnDisk(DirFilesCache dfc, ShowItem si, Episode epi,bool checkDirectoryExist = true)
        {
            if (dfc == null)
                dfc = new DirFilesCache();
            List<FileInfo> ret = new List<FileInfo>();

            int seasWanted = si.DVDOrder ? epi.TheDVDSeason.SeasonNumber: epi.TheAiredSeason.SeasonNumber;
            int epWanted = si.DVDOrder ? epi.DVDEpNum : epi.AiredEpNum;

            int snum = seasWanted;

            if (!si.AllFolderLocationsEpCheck(checkDirectoryExist).ContainsKey(snum))
                return ret;

            foreach (string folder in si.AllFolderLocationsEpCheck(checkDirectoryExist)[snum])
            {
                FileInfo[] files = dfc.Get(folder);
                if (files == null)
                    continue;

                foreach (FileInfo fiTemp in files)
                {
                    if (!TVSettings.Instance.UsefulExtension(fiTemp.Extension, false))
                        continue; // move on

                    if (!FindSeasEp(fiTemp, out int seasFound, out int epFound, out int maxEp, si)) continue;

                    if (seasFound == -1)
                        seasFound = seasWanted;
                    if ((seasFound == seasWanted) && (epFound == epWanted))
                        ret.Add(fiTemp);
                }
            }

            return ret;
        }

        public void WriteXMLSettings()
        {
            // backup old settings before writing new ones

            FileHelper.Rotate(PathManager.TVDocSettingsFile.FullName);
            logger.Info("Saving Settings to {0}", PathManager.TVDocSettingsFile.FullName);

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
                foreach (ShowItem si in this.Library.Values)
                    si.WriteXMLSettings(writer);
                writer.WriteEndElement(); // myshows

                XMLHelper.WriteStringsToXml(TVSettings.Instance.LibraryFolders, writer, "MonitorFolders", "Folder");
                XMLHelper.WriteStringsToXml(TVSettings.Instance.IgnoreFolders, writer, "IgnoreFolders", "Folder");
                XMLHelper.WriteStringsToXml(TVSettings.Instance.DownloadFolders, writer, "FinderSearchFolders", "Folder");

                writer.WriteStartElement("IgnoreItems");
                foreach (IgnoreItem ii in TVSettings.Instance.Ignore)
                    ii.Write(writer);
                writer.WriteEndElement(); // IgnoreItems

                writer.WriteEndElement(); // tvrename
                writer.WriteEndDocument();
            }

            this.mDirty = false;

            Stats().Save();
        }

        private bool LoadXMLSettings(FileInfo from)
        {
            logger.Info("Loading Settings from {0}", from.FullName);
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

                using (XmlReader reader = XmlReader.Create(from.FullName, settings))
                {

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
                            TVSettings.Instance.load(reader.ReadSubtree());
                            reader.Read();
                        }
                        else if (reader.Name == "MyShows")
                        {
                            this.Library.LoadFromXML(reader.ReadSubtree());
                            reader.Read();
                        }
                        else if (reader.Name == "MonitorFolders")
                            TVSettings.Instance.LibraryFolders = XMLHelper.ReadStringsFromXml(reader, "MonitorFolders", "Folder");
                        else if (reader.Name == "IgnoreFolders")
                            TVSettings.Instance.IgnoreFolders  = XMLHelper.ReadStringsFromXml(reader, "IgnoreFolders", "Folder");
                        else if (reader.Name == "FinderSearchFolders")
                            TVSettings.Instance.DownloadFolders = XMLHelper.ReadStringsFromXml(reader, "FinderSearchFolders", "Folder");
                        else if (reader.Name == "IgnoreItems")
                        {
                            XmlReader r2 = reader.ReadSubtree();
                            r2.Read();
                            r2.Read();
                            while (r2.Name == "Ignore")
                                TVSettings.Instance.Ignore.Add(new IgnoreItem(r2));
                            reader.Read();
                        }
                        else
                            reader.ReadOuterXml();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Warn(e, "Problem on Startup loading File");
                this.LoadErr = from.Name + " : " + e.Message;
                return false;
            }

            try
            {
                this.mStats = TVRenameStats.Load();
            }
            catch (Exception)
            {
                // not worried if stats loading fails
            }

            return true;
        }

        private void OutputActionFiles(TVSettings.ScanType st)
        {
            List<ActionListExporter> ALExpoters = new List<ActionListExporter>
            {
                new MissingXML(this.TheActionList),
                new MissingCSV(this.TheActionList),
                new CopyMoveXML(this.TheActionList),
                new RenamingXML(this.TheActionList)
            };

            foreach (ActionListExporter ue in ALExpoters)
            {
                if (ue.Active() && ue.ApplicableFor(st)) { ue.Run(); }
            }
        }

        public void ExportShowInfo()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                new ShowsTXT(this.Library.GetShowItems()).Run();
            }).Start();

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                new ShowsHTML(this.Library.GetShowItems()).Run();
            }).Start();
        }

        public void WriteUpcoming()
        {
            List<UpcomingExporter> lup = new List<UpcomingExporter> {new UpcomingRSS(this), new UpcomingXML(this)};


            foreach (UpcomingExporter ue in lup)
            {
                if (ue.Active()) { ue.Run(); }
            }
        }

        private static bool ListHasMissingItems(ItemList l)
        {
            foreach (Item i in l)
            {
                if (i is ItemMissing)
                    return true;
            }
            return false;
        }

        public void Scan(List< ShowItem> shows, bool unattended, TVSettings.ScanType st)
        {
            try
            {
                this.CurrentlyBusy = true;

                if (shows == null)
                {
                    if (st == TVSettings.ScanType.Full) shows = this.Library.GetShowItems();
                    if (st == TVSettings.ScanType.Quick) shows = GetQuickShowsToScan(true, true);
                    if (st == TVSettings.ScanType.Recent) shows = this.Library.getRecentShows();
                }


                if (TVSettings.Instance.MissingCheck && !CheckAllFoldersExist(shows)
                ) // only check for folders existing for missing check
                    return;

                if (!DoDownloadsFG())
                    return;

                Thread actionWork = new Thread(ScanWorker) {Name = "ActionWork"};

                this.ActionCancel = false;
                foreach (Finder f in this.Finders)
                {
                    f.Reset();
                }

                if (!this.Args.Hide)
                {
                    this.ScanProgDlg = new ScanProgress(
                        TVSettings.Instance.RenameCheck || TVSettings.Instance.MissingCheck,
                        TVSettings.Instance.MissingCheck && TVSettings.Instance.SearchLocally,
                        TVSettings.Instance.MissingCheck &&
                        (TVSettings.Instance.CheckuTorrent || TVSettings.Instance.CheckSABnzbd),
                        TVSettings.Instance.MissingCheck && TVSettings.Instance.SearchRSS);
                }
                else
                    this.ScanProgDlg = null;

                actionWork.Start(shows.ToList());

                if ((this.ScanProgDlg != null) && (this.ScanProgDlg.ShowDialog() == DialogResult.Cancel))
                {
                    this.ActionCancel = true;
                    actionWork.Interrupt();
                    foreach (Finder f in this.Finders)
                    {
                        f.Interrupt();
                    }
                }
                else
                    actionWork.Join();

                this.ScanProgDlg = null;

                this.DownloadIdentifiers.reset();

                this.OutputActionFiles(st); //Save missing shows to XML (and others)

                this.CurrentlyBusy = false;
            }
            catch (Exception e)
            {
                logger.Fatal(e, "Unhandled Exception in ScanWorker");
                
            }
        }

        public void doAllActions()
        {

            ItemList theList = new ItemList();

            foreach (Item action in this.TheActionList)
            {
                if (action is Item)
                {
                    theList.Add((Item)(action));

                }
            }

            DoActions(theList);
        }

        protected internal List<PossibleDuplicateEpisode> FindDoubleEps()
        {
            StringBuilder output = new StringBuilder();
            List<PossibleDuplicateEpisode> returnValue = new List<PossibleDuplicateEpisode>();

            output.AppendLine("");
            output.AppendLine("##################################################");
            output.AppendLine("DUPLICATE FINDER - Start");
            output.AppendLine("##################################################");

            DirFilesCache dfc = new DirFilesCache();
            foreach (ShowItem si in this.Library.Values)
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
                        if (pep.type == ProcessedEpisode.ProcessedEpisodeType.merged)
                        {
                            output.AppendLine(si.ShowName + " - Season: " + kvp.Key + " - " + pep.NumsAsString() +" - "+pep.Name+" is:");
                            foreach (Episode sourceEpisode in pep.sourceEpisodes)
                            {
                                output.AppendLine("                      - " + sourceEpisode.AiredEpNum + " - " + sourceEpisode.Name);
                            }
                        }

                    foreach (ProcessedEpisode comparePep in kvp.Value)
                        {
                            if (pep.FirstAired.HasValue && comparePep.FirstAired.HasValue &&
                                pep.FirstAired == comparePep.FirstAired && pep.EpisodeID < comparePep.EpisodeID)
                            {
                                // Tell user about this possibility
                                output.AppendLine(
                                    $"{si.ShowName} - Season: {kvp.Key} - {pep.FirstAired.ToString()} - {pep.AiredEpNum}({pep.Name}) - {comparePep.AiredEpNum}({comparePep.Name})");

                                //do the 'name' test
                                string root = Helpers.GetCommonStartString(pep.Name, comparePep.Name);
                                bool sameLength = (pep.Name.Length == comparePep.Name.Length);

                                bool sameName = (!root.Trim().Equals("Episode") && sameLength && root.Length > 3 &&
                                                 root.Length > pep.Name.Length / 2);
                                bool oneFound = false;
                                bool largerFileSize = false;

                                if (sameName)
                                {
                                    output.AppendLine("####### POSSIBLE DUPLICATE DUE TO NAME##########");
                                

                                //Do the missing Test (ie is one missing and not the other)
                                    bool pepFound = FindEpOnDisk(dfc, pep).Count > 0;
                                    bool comparePepFound = FindEpOnDisk(dfc, comparePep).Count > 0;

                                oneFound = (pepFound ^ comparePepFound);
                                
                                    if (oneFound)
                                    {
                                        output.AppendLine(
                                            "####### POSSIBLE DUPLICATE DUE TO ONE MISSING AND ONE FOUND ##########");

                                        ProcessedEpisode possibleDupEpisode = pepFound ? pep : comparePep;
                                        //Test the file sizes in the season
                                        //More than 40% longer
                                        FileInfo possibleDupFile = FindEpOnDisk(dfc, possibleDupEpisode)[0];
                                        int dupMovieLength = possibleDupFile.GetFilmLength();
                                        List<int> otherMovieLengths =new List<int>();

                                        foreach (FileInfo file in possibleDupFile.Directory.EnumerateFiles())
                                        {
                                            if (TVSettings.Instance.UsefulExtension(file.Extension, false))
                                            {
                                                otherMovieLengths.Add(file.GetFilmLength());
                                            }
                                        }

                                        int averageMovieLength =
                                            (otherMovieLengths.Sum() - dupMovieLength) / (otherMovieLengths.Count - 1);

                                        largerFileSize = (dupMovieLength > averageMovieLength * 1.4);

                                        if (largerFileSize)
                                        {

                                            {
                                                output.AppendLine(
                                                    "######################################################################");
                                                output.AppendLine(
                                                    "####### SURELY WE HAVE ONE NOW                              ##########");
                                                output.AppendLine(
                                                    $"####### {possibleDupEpisode.AiredEpNum}({possibleDupEpisode.Name}) has length {dupMovieLength} greater than the average in the directory of {averageMovieLength}");
                                                output.AppendLine(
                                                    "######################################################################");
                                            }
                                        }
                                    }
                                }
                                returnValue.Add(new PossibleDuplicateEpisode(pep, comparePep, kvp.Key, true, sameName, oneFound, largerFileSize));
                            }
                        }
                    }
                }

                
            }
            output.AppendLine("##################################################");
            output.AppendLine("DUPLICATE FINDER - End");
            output.AppendLine("##################################################");

            logger.Info(output.ToString());
            return returnValue;
        }

        public void QuickScan() => Scan(null, true,TVSettings.ScanType.Quick);

        private List<ShowItem> GetQuickShowsToScan(bool doMissingRecents, bool doFilesInDownloadDir)
        {

            this.CurrentlyBusy = true;

            List<ShowItem> showsToScan = new List<ShowItem> { };
            if (doFilesInDownloadDir) showsToScan = GetShowsThatHaveDownloads();

            if (doMissingRecents)
            {
                List<ProcessedEpisode> lpe = GetMissingEps();
                if (lpe != null)
                {
                    foreach (ProcessedEpisode pe in lpe)
                    {
                        if (!showsToScan.Contains(pe.SI)) showsToScan.Add(pe.SI);
                    }
                }
            }

            this.CurrentlyBusy = false;

            return showsToScan;
        }

        private bool CheckAllFoldersExist(IEnumerable< ShowItem> showlist)
        {
            // show MissingFolderAction for any folders that are missing
            // return false if user cancels

            if (showlist == null) // nothing specified?
                showlist = this.Library.Values; // everything

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

                    foreach (string folderFE in folders)
                    {
                        string folder = folderFE;

                        // generate new filename info
                        bool goAgain = false;
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
                                    goAgain = false;
                                    break;
                                }
                            }
                            if ((di == null) || (!di.Exists))
                            {
                                string sn = si.ShowName;
                                string text = snum + " of " + si.MaxSeason();
                                string theFolder = folder;
                                string otherFolder = null;

                                FAResult whatToDo = FAResult.kfaNotSet;

                                if (this.Args.MissingFolder == CommandLineArgs.MissingFolderBehavior.Create)
                                    whatToDo = FAResult.kfaCreate;
                                else if (this.Args.MissingFolder == CommandLineArgs.MissingFolderBehavior.Ignore)
                                    whatToDo = FAResult.kfaIgnoreOnce;

                                if (this.Args.Hide && (whatToDo == FAResult.kfaNotSet))
                                    whatToDo = FAResult.kfaIgnoreOnce; // default in /hide mode is to ignore

                                if (TVSettings.Instance.AutoCreateFolders && firstAttempt)
                                {
                                    whatToDo = FAResult.kfaCreate;
                                    firstAttempt = false;
                                }


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
                                    return false;
                                }
                                else if (whatToDo == FAResult.kfaCreate)
                                {
                                    try
                                    {
                                        logger.Info("Creating directory as it is missing: {0}", folder);
                                        Directory.CreateDirectory(folder);
                                    }
                                    catch (Exception ioe)
                                    {
                                        logger.Info("Could not directory: {0}", folder);
                                        logger.Info(ioe);
                                    }
                                    goAgain = true;


                                }
                                else if (whatToDo == FAResult.kfaIgnoreAlways)
                                {
                                    si.IgnoreSeasons.Add(snum);
                                    SetDirty();
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

            return true;
        }

        public void RemoveIgnored()
        {
            ItemList toRemove = new ItemList();
            foreach (Item item in this.TheActionList)
            {
                Item act = (Item) item;
                foreach (IgnoreItem ii in TVSettings.Instance.Ignore)
                {
                    if (!ii.SameFileAs(act.Ignore)) continue;
                    toRemove.Add(item);
                    break;
                }
            }
            foreach (Item action in toRemove)
                this.TheActionList.Remove(action);
        }

        public void ForceUpdateImages(ShowItem si)
        {

            this.TheActionList = new ItemList();

            DirFilesCache dfc = new DirFilesCache();
            logger.Info("*******************************");
            logger.Info("Force Update Images: " + si.ShowName);

            if (!string.IsNullOrEmpty(si.AutoAdd_FolderBase) && (si.AllFolderLocations().Count > 0))
            {
                this.TheActionList.Add(this.DownloadIdentifiers.ForceUpdateShow(DownloadIdentifier.DownloadType.downloadImage, si));
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

                foreach (string folder in folders)
                {
                    //Image series checks here
                    this.TheActionList.Add(this.DownloadIdentifiers.ForceUpdateSeason(DownloadIdentifier.DownloadType.downloadImage, si, folder, snum));
                }

            } // for each season of this show

            RemoveIgnored();

        }

        private void FindUnusedFilesInDLDirectory(ICollection<ShowItem> showList)
        {

            //for each directory in settings directory
            //for each file in directory
            //for each saved show (order by recent)
            //is file aready availabele? 
            //if so add show to list of files to be removed

            DirFilesCache dfc = new DirFilesCache();

            //When doing a fullscan the showlist is null indicating that all shows should be checked
            if (showList is null)
            {
                showList = this.Library.Values ;
            }

            foreach (String dirPath in TVSettings.Instance.DownloadFolders )
            {
                if (!Directory.Exists(dirPath)) continue;

                foreach (String filePath in Directory.GetFiles(dirPath, "*", System.IO.SearchOption.AllDirectories))
                {
                    if (!File.Exists(filePath)) continue;

                    FileInfo fi = new FileInfo(filePath);

                    if (FileHelper.IgnoreFile(fi)) continue;

                    List<ShowItem> matchingShows = new List<ShowItem>();

                    foreach (ShowItem si in showList)
                    {
                        if (si.getSimplifiedPossibleShowNames().Any(name => FileHelper.SimplifyAndCheckFilename(fi.Name, name)))
                            matchingShows.Add(si);
                    }

                    if (matchingShows.Count > 0)
                    {
                        bool fileCanBeRemoved = true;

                        foreach (ShowItem si in matchingShows)
                        {
                            if (fileNeeded(fi, si, dfc)) fileCanBeRemoved = false;
                        }

                        if (fileCanBeRemoved)
                        {
                            ShowItem si = matchingShows[0];//Choose the first series
                            FindSeasEp(fi, out int seasF, out int epF, out int maxEp, si, out FilenameProcessorRE rex);
                            SeriesInfo s = si.TheSeries();
                            Episode ep = s.getEpisode(seasF, epF, si.DVDOrder);
                            ProcessedEpisode pep = new ProcessedEpisode(ep, si);
                            logger.Info($"Removing {fi.FullName } as it matches {matchingShows[0].ShowName} and no episodes are needed");
                            this.TheActionList.Add(new ActionDeleteFile(fi, pep, TVSettings.Instance.Tidyup));
                        }

                    }

                }


                foreach (String subDirPath in Directory.GetDirectories(dirPath, "*", System.IO.SearchOption.AllDirectories))
                {
                    if (!Directory.Exists(subDirPath)) continue;

                    DirectoryInfo di = new DirectoryInfo(subDirPath);

                    List<ShowItem> matchingShows = new List<ShowItem>();

                    foreach (ShowItem si in showList)
                    {
                        if (si.getSimplifiedPossibleShowNames().Any(name => FileHelper.SimplifyAndCheckFilename(di.Name, name)))
                            matchingShows.Add(si);
                    }

                    if (matchingShows.Count > 0)
                    {
                        bool dirCanBeRemoved = true;

                        foreach (ShowItem si in matchingShows)
                        {
                            if (fileNeeded(di, si, dfc)) dirCanBeRemoved = false;
                        }

                        if (dirCanBeRemoved)
                        {
                            ShowItem si = matchingShows[0];//Choose the first series
                            FindSeasEp(di, out int seasF, out int epF, si, out FilenameProcessorRE rex);
                            SeriesInfo s = si.TheSeries();
                            Episode ep = s.getEpisode(seasF, epF, si.DVDOrder);
                            ProcessedEpisode pep = new ProcessedEpisode(ep, si);
                            logger.Info($"Removing {di.FullName } as it matches {matchingShows[0].ShowName} and no episodes are needed");
                            this.TheActionList.Add(new ActionDeleteDirectory(di, pep, TVSettings.Instance.Tidyup));
                        }

                    }

                }

            }


        }

        private static bool fileNeeded(FileInfo fi, ShowItem si, DirFilesCache dfc)
        {
            if (FindSeasEp(fi, out int seasF, out int epF, out int maxEp, si, out FilenameProcessorRE rex))
            {
                SeriesInfo s = si.TheSeries();
                try
                {
                    Episode ep = s.getEpisode(seasF, epF, si.DVDOrder);
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

        private static bool fileNeeded(DirectoryInfo di, ShowItem si, DirFilesCache dfc)
        {
            if (FindSeasEp(di, out int seasF, out int epF, si, out FilenameProcessorRE rex))
            {
                SeriesInfo s = si.TheSeries();
                try
                {
                    Episode ep = s.getEpisode(seasF, epF,si.DVDOrder);
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

        private void RenameAndMissingCheck(SetProgressDelegate prog, ICollection<ShowItem> showList)
        {
            this.TheActionList = new ItemList();

            //int totalEps = 0;


            //foreach (ShowItem si in showlist)
            //  if (si.DoRename)
            //    totalEps += si.SeasonEpisodes.Count;

            if (TVSettings.Instance.RenameCheck)
                Stats().RenameChecksDone++;

            if (TVSettings.Instance.MissingCheck)
                Stats().MissingChecksDone++;

            prog.Invoke(0);

            if (showList == null)
            {
                // only do episode count if we're doing all shows and seasons
                this.mStats.NS_NumberOfEpisodes = 0;
                showList = this.Library.Values;
            }


            DirFilesCache dfc = new DirFilesCache();
            int c = 0;
            foreach (ShowItem si in showList)
            {
                if (this.ActionCancel)
                    return;

                logger.Info("Rename and missing check: " + si.ShowName);
                c++;

                prog.Invoke(100 * c / showList.Count);

                if (si.AllFolderLocations().Count == 0) // no folders defined for this show
                    continue; // so, nothing to do.

                //This is the code that will iterate over the DownloadIdentifiers and ask each to ensure that
                //it has all the required files for that show
                if (!string.IsNullOrEmpty(si.AutoAdd_FolderBase) && (si.AllFolderLocations().Count > 0))
                {
                    this.TheActionList.Add(this.DownloadIdentifiers.ProcessShow(si));
                }

                //MS_TODO Put the bannerrefresh period into the settings file, we'll default to 3 months
                DateTime cutOff = DateTime.Now.AddMonths(-3);
                DateTime lastUpdate = si.BannersLastUpdatedOnDisk ?? DateTime.Now.AddMonths(-4);
                bool timeForBannerUpdate = (cutOff.CompareTo(lastUpdate) == 1);

                if (TVSettings.Instance.NeedToDownloadBannerFile() && timeForBannerUpdate)
                {
                    this.TheActionList.Add(this.DownloadIdentifiers.ForceUpdateShow(DownloadIdentifier.DownloadType.downloadImage, si));
                    si.BannersLastUpdatedOnDisk = DateTime.Now;
                    SetDirty();
                }

                // process each folder for each season...

                int[] numbers = new int[si.SeasonEpisodes.Keys.Count];
                si.SeasonEpisodes.Keys.CopyTo(numbers, 0);
                Dictionary<int, List<string>> allFolders = si.AllFolderLocations();

                int lastSeason = numbers.Concat(new[] {0}).Max();

                foreach (int snum in numbers)
                {
                    if (this.ActionCancel)
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
                        if (episode.AppropriateEpNum > maxEpisodeNumber)
                            maxEpisodeNumber = episode.AppropriateEpNum;
                    }


                    // base folder:
                    if (!string.IsNullOrEmpty(si.AutoAdd_FolderBase) && (si.AllFolderLocations(false).Count > 0))
                    {
                        // main image for the folder itself
                        this.TheActionList.Add(this.DownloadIdentifiers.ProcessShow(si));
                    }


                    foreach (string folder in folders)
                    {
                        if (this.ActionCancel)
                            return;

                        FileInfo[] files = dfc.Get(folder);
                        if (files == null)
                            continue;

                        if (TVSettings.Instance.NeedToDownloadBannerFile() && timeForBannerUpdate)
                        {
                            //Image series checks here
                            this.TheActionList.Add(this.DownloadIdentifiers.ForceUpdateSeason(DownloadIdentifier.DownloadType.downloadImage, si, folder, snum));
                        }

                        bool renCheck = TVSettings.Instance.RenameCheck && si.DoRename && Directory.Exists(folder); // renaming check needs the folder to exist
                        bool missCheck = TVSettings.Instance.MissingCheck && si.DoMissingCheck;

                        //Image series checks here
                        this.TheActionList.Add(this.DownloadIdentifiers.ProcessSeason(si, folder, snum));

                        FileInfo[] localEps = new FileInfo[maxEpisodeNumber + 1];

                        int maxEpNumFound = 0;
                        if (!renCheck && !missCheck)
                            continue;

                        foreach (FileInfo fi in files)
                        {
                            if (this.ActionCancel)
                                return;

                            if (!FindSeasEp(fi, out int seasNum, out int epNum, out int maxEp, si, out FilenameProcessorRE rex))
                                continue; // can't find season & episode, so this file is of no interest to us

                            if (seasNum == -1)
                                seasNum = snum;

#if !NOLAMBDA
                            int epIdx = eps.FindIndex(x => ((x.AppropriateEpNum == epNum) && (x.AppropriateSeasonNumber == seasNum)));
                            if (epIdx == -1)
                                continue; // season+episode number don't correspond to any episode we know of from thetvdb
                            ProcessedEpisode ep = eps[epIdx];
#else
    // equivalent of the 4 lines above, if compiling on MonoDevelop on Windows which, for 
    // some reason, doesn't seem to support lambda functions (the => thing)
							
							ProcessedEpisode ep = null;
							
							foreach (ProcessedEpisode x in eps)
							{
								if (((x.AppropriateEpNum == epNum) && (x.AppropriateSeasonNumber == seasNum)))
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

                                if (TVSettings.Instance.RetainLanguageSpecificSubtitles &&
                                    fi.IsLanguageSpecificSubtitle(out string subtitleExtension))
                                {
                                    newname = TVSettings.Instance.FilenameFriendly(
                                        TVSettings.Instance.NamingStyle.NameForExt(ep, subtitleExtension,
                                            folder.Length));
                                }

                                if (newname != actualFile.Name)
                                {
                                    actualFile = FileHelper.FileInFolder(folder, newname); // rename updates the filename
                                    this.TheActionList.Add(new ActionCopyMoveRename(ActionCopyMoveRename.Op.Rename, fi, actualFile, ep, null,null));

                                    //The following section informs the DownloadIdentifers that we already plan to
                                    //copy a file inthe appropriate place and they do not need to worry about downloading 
                                    //one for that purpse

                                    this.DownloadIdentifiers.notifyComplete(actualFile);

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
                                if ((dbep.AppropriateEpNum  > maxEpNumFound) || (localEps[dbep.AppropriateEpNum] == null)) // not here locally
                                {
                                    DateTime? dt = dbep.GetAirDateDT(true);
                                    bool dtOK = dt != null;

                                    bool notFuture = (dtOK && (dt.Value.CompareTo(today) < 0)); // isn't an episode yet to be aired

                                    bool noAirdatesUntilNow = true;
                                    // for specials "season", see if any season has any airdates
                                    // otherwise, check only up to the season we are considering
                                    for (int i = 1; i <= ((snum == 0) ? lastSeason : snum); i++)
                                    {
                                        if (ShowLibrary.HasAnyAirdates(si, i))
                                        {
                                            noAirdatesUntilNow = false;
                                            break;
                                        }
                                        else
                                        {//If the show is in its first season and no episodes have air dates
                                            if (lastSeason == 1)
                                            {
                                                noAirdatesUntilNow = false;
                                            }
                                        }
                                    }

                                    // only add to the missing list if, either:
                                    // - force check is on
                                    // - there are no airdates at all, for up to and including this season
                                    // - there is an airdate, and it isn't in the future
                                    if (noAirdatesUntilNow ||
                                        ((si.ForceCheckFuture || notFuture) && dtOK) ||
                                        (si.ForceCheckNoAirdate && !dtOK))
                                    {
                                        // then add it as officially missing
                                        this.TheActionList.Add(new ItemMissing(dbep, folder, TVSettings.Instance.FilenameFriendly(TVSettings.Instance.NamingStyle.NameForExt(dbep, null, folder.Length))));
                                    }
                                }
                                else
                                {
                                    // the file is here
                                    if (showList == null)
                                        this.mStats.NS_NumberOfEpisodes++;

                                    // do NFO and thumbnail checks if required
                                    FileInfo filo = localEps[dbep.AppropriateEpNum ]; // filename (or future filename) of the file

                                    this.TheActionList.Add(this.DownloadIdentifiers.ProcessEpisode(dbep, filo));

                                }
                            } // up to date check, for each episode in thetvdb
                            TheTVDB.Instance.Unlock("UpToDateCheck");
                        } // if doing missing check
                    } // for each folder for this season of this show
                } // for each season of this show
            } // for each show

            RemoveIgnored();
        }

        private void NoProgress(int pct)
        {
        }

        private void ScanWorker(object o)
        {
            try
            {
                List<ShowItem> specific = (List<ShowItem>) (o);


                while (!this.Args.Hide && ((this.ScanProgDlg == null) || (!this.ScanProgDlg.Ready)))
                    Thread.Sleep(10); // wait for thread to create the dialog

                this.TheActionList = new ItemList();
                SetProgressDelegate noProgress = NoProgress;

                if (TVSettings.Instance.RenameCheck || TVSettings.Instance.MissingCheck)
                    RenameAndMissingCheck(this.ScanProgDlg == null ? noProgress : this.ScanProgDlg.MediaLibProg,
                        specific);

                if (TVSettings.Instance.RemoveDownloadDirectoriesFiles)
                    FindUnusedFilesInDLDirectory(specific);

                if (TVSettings.Instance.MissingCheck)
                {
                    // have a look around for any missing episodes
                    int activeLocalFinders = 0;
                    int activeRSSFinders = 0;
                    int activeDownloadingFinders = 0;

                    foreach (Finder f in this.Finders)
                    {
                        if (!f.Active()) continue;
                        f.ActionList= this.TheActionList;

                        switch (f.DisplayType())
                        {
                            case Finder.FinderDisplayType.Local:
                                activeLocalFinders++;
                                break;
                            case Finder.FinderDisplayType.Downloading:
                                activeDownloadingFinders++;
                                break;
                            case Finder.FinderDisplayType.RSS:
                                activeRSSFinders++;
                                break;
                        }
                    }

                    int currentLocalFinderId = 0;
                    int currentRSSFinderId = 0;
                    int currentDownloadingFinderId = 0;

                    foreach (Finder f in this.Finders)
                    {
                        if (this.ActionCancel)
                        {
                            return;
                        }

                        if (f.Active() && ListHasMissingItems(this.TheActionList))
                        {

                            int startPos;
                            int endPos;

                            switch (f.DisplayType())
                            {
                                case Finder.FinderDisplayType.Local:
                                    currentLocalFinderId++;
                                    startPos = 100 * (currentLocalFinderId - 1) / activeLocalFinders;
                                    endPos = 100 * (currentLocalFinderId) / activeLocalFinders;
                                    f.Check(this.ScanProgDlg == null ? noProgress : this.ScanProgDlg.LocalSearchProg,
                                        startPos, endPos);
                                    break;
                                case Finder.FinderDisplayType.Downloading:
                                    currentDownloadingFinderId++;
                                    startPos = 100 * (currentDownloadingFinderId - 1) / activeDownloadingFinders;
                                    endPos = 100 * (currentDownloadingFinderId) / activeDownloadingFinders;
                                    f.Check(this.ScanProgDlg == null ? noProgress : this.ScanProgDlg.DownloadingProg,
                                        startPos, endPos);
                                    break;
                                case Finder.FinderDisplayType.RSS:
                                    currentRSSFinderId++;
                                    startPos = 100 * (currentRSSFinderId - 1) / activeRSSFinders;
                                    endPos = 100 * (currentRSSFinderId) / activeRSSFinders;
                                    f.Check(this.ScanProgDlg == null ? noProgress : this.ScanProgDlg.RSSProg, startPos,
                                        endPos);
                                    break;
                            }

                            RemoveIgnored();
                        }
                    }
                }

                if (this.ActionCancel)
                    return;

                // sort Action list by type
                this.TheActionList.Sort(new ActionItemSorter()); // was new ActionSorter()

                if (this.ScanProgDlg != null)
                    this.ScanProgDlg.Done();

                Stats().FindAndOrganisesDone++;
            }
            catch (Exception e)
            {
                logger.Fatal(e,"Unhandled Exception in ScanWorker");
                return;
            }
        }



        public static bool MatchesSequentialNumber(string filename, ref int seas, ref int ep, ProcessedEpisode pe)
        {
            if (pe.OverallNumber == -1)
                return false;

            string num = pe.OverallNumber.ToString();

            bool found = Regex.Match("X" + filename + "X", "[^0-9]0*" + num + "[^0-9]").Success; // need to pad to let it match non-numbers at start and end
            if (found)
            {
                seas = pe.AppropriateSeasonNumber;
                ep = pe.AppropriateEpNum;
            }
            return found;
        }

        private static string SimplifyFilename(string filename, string showNameHint)
        {
            // Look at showNameHint and try to remove the first occurance of it from filename
            // This is very helpful if the showname has a >= 4 digit number in it, as that
            // would trigger the 1302 -> 13,02 matcher
            // Also, shows like "24" can cause confusion


            //TODO: More replacement of non useful characters - MarkSummerville
            filename = filename.Replace(".", " "); // turn dots into spaces

            if (string.IsNullOrEmpty(showNameHint))
                return filename;

            bool nameIsNumber = (Regex.Match(showNameHint, "^[0-9]+$").Success);

            int p = filename.IndexOf(showNameHint, StringComparison.Ordinal);

            if (p == 0)
            {
                filename = filename.Remove(0, showNameHint.Length);
                return filename;
            }

            if (nameIsNumber) // e.g. "24", or easy exact match of show name at start of filename
            {
                filename = filename.Remove(0, showNameHint.Length);
                return filename;
            }
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

        private static bool FindSeasEpDateCheck(FileInfo fi, out int seas, out int ep, out int maxEp, ShowItem si)
        {
            if (fi == null || si == null)
            {
                seas = -1;
                ep = -1;
                maxEp = -1;
                return false;
            }

            // look for a valid airdate in the filename
            // check for YMD, DMY, and MDY
            // only check against airdates we expect for the given show
            SeriesInfo ser = TheTVDB.Instance.GetSeries(si.TVDBCode);
            string[] dateFormats = new[] { "yyyy-MM-dd", "dd-MM-yyyy", "MM-dd-yyyy", "yy-MM-dd", "dd-MM-yy", "MM-dd-yy" };
            string filename = fi.Name;
            // force possible date separators to a dash
            filename = filename.Replace("/", "-");
            filename = filename.Replace(".", "-");
            filename = filename.Replace(",", "-");
            filename = filename.Replace(" ", "-");

            ep = -1;
            seas = -1;
            maxEp = -1;
            Dictionary<int, Season> seasonsToUse = si.DVDOrder ? ser.DVDSeasons : ser.AiredSeasons;

            foreach (KeyValuePair<int, Season> kvp in seasonsToUse)
            {
                if (si.IgnoreSeasons.Contains(kvp.Value.SeasonNumber))
                    continue;

                foreach (Episode epi in kvp.Value.Episodes)
                {
                    DateTime? dt = epi.GetAirDateDT(); // file will have local timezone date, not ours
                    if (dt == null)
                        continue;

                    TimeSpan closestDate = TimeSpan.MaxValue;

                    foreach (string dateFormat in dateFormats)
                    {
                        string datestr = dt.Value.ToString(dateFormat);
                        if (filename.Contains(datestr) && DateTime.TryParseExact(datestr, dateFormat, new CultureInfo("en-GB"), DateTimeStyles.None, out DateTime dtInFilename))
                        {
                            TimeSpan timeAgo = DateTime.Now.Subtract(dtInFilename);
                            if (timeAgo < closestDate)
                            {
                                seas = (si.DVDOrder?epi.DVDSeasonNumber: epi.AiredSeasonNumber);
                                ep = si.DVDOrder?epi.DVDEpNum: epi.AiredEpNum;
                                closestDate = timeAgo;
                            }
                        }
                    }
                }
            }

            return ((ep != -1) && (seas != -1));
        }

        private List<ProcessedEpisode> GetMissingEps()
        {
            int dd = TVSettings.Instance.WTWRecentDays;
            DirFilesCache dfc = new DirFilesCache();
            return GetMissingEps(dfc, this.Library.GetRecentAndFutureEps(dd));
        }

        private List<ProcessedEpisode> GetMissingEps(DirFilesCache dfc, IEnumerable<ProcessedEpisode> lpe)
        {
            List<ProcessedEpisode> missing = new List<ProcessedEpisode>();

            foreach (ProcessedEpisode pe in lpe)
            {
                List<FileInfo> fl = FindEpOnDisk(dfc, pe);

                bool foundOnDisk = ((fl != null) && (fl.Count > 0));
                bool alreadyAired = pe.GetAirDateDT(true).Value.CompareTo(DateTime.Now) < 0;


                if (!foundOnDisk && alreadyAired && (pe.SI.DoMissingCheck))
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

                string[] x = Directory.GetFiles(dirPath, "*", System.IO.SearchOption.AllDirectories);
                logger.Info($"Processing {x.Length} files for shows that need to be scanned");

                foreach (string filePath in x)
                {
                    logger.Info($"Checking to see whether {filePath} is a file that for a show that need scanning");

                    if (!File.Exists(filePath)) continue;

                    FileInfo fi = new FileInfo(filePath);

                    if (FileHelper.IgnoreFile(fi)) continue;

                    foreach (ShowItem si in this.Library.Shows)
                    {
                        if (showsToScan.Contains(si)) continue;

                        if (si.getSimplifiedPossibleShowNames().Any(name => FileHelper.SimplifyAndCheckFilename(fi.Name, name)))
                            showsToScan.Add(si);
                    }
                }

                string[] directories = Directory.GetDirectories(dirPath, "*", System.IO.SearchOption.AllDirectories);
                logger.Info($"Processing {directories.Length} directories for shows that need to be scanned");

                foreach (string subDirPath in directories)
                {
                    logger.Info($"Checking to see whether {subDirPath } has any shows that need scanning");

                    if (!Directory.Exists(subDirPath)) continue;

                    DirectoryInfo di = new DirectoryInfo(subDirPath);

                    foreach (ShowItem si in this.Library.Values)
                    {
                        if (showsToScan.Contains(si)) continue;

                        if (si.getSimplifiedPossibleShowNames().Any(name => FileHelper.SimplifyAndCheckFilename(di.Name, name)))
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
            DoDownloadsFG();
        }

        private static bool FindSeasEp(FileInfo fi, out int seas, out int ep, out int maxEp, ShowItem si, out FilenameProcessorRE re)
        {
            return FindSeasEp(fi, out seas, out ep, out maxEp, si, TVSettings.Instance.FNPRegexs, TVSettings.Instance.LookForDateInFilename, out re);
        }
        public static bool FindSeasEp(FileInfo fi, out int seas, out int ep, out int maxEp, ShowItem si, List<FilenameProcessorRE> rexps, bool doDateCheck,out FilenameProcessorRE re)
        {
            re = null;
            if (fi == null)
            {
                seas = -1;
                ep = -1;
                maxEp = -1;
                return false;
            }

            if (doDateCheck && FindSeasEpDateCheck(fi, out seas, out ep,out maxEp, si))
                return true;

            string filename = fi.Name;
            int l = filename.Length;
            int le = fi.Extension.Length;
            filename = filename.Substring(0, l - le);
            return FindSeasEp(fi.Directory.FullName, filename, out seas, out ep,out  maxEp ,si, rexps, out re);
        }

        private static bool FindSeasEp(DirectoryInfo di, out int seas, out int ep, ShowItem si, out FilenameProcessorRE re)
        {

            List<FilenameProcessorRE> rexps = TVSettings.Instance.FNPRegexs;
            re = null;

            if (di == null)
            {
                seas = -1;
                ep = -1;
                return false;
            }

            return FindSeasEp(di.Parent.FullName, di.Name, out seas, out ep, out int maxEp, si, rexps, out re);
        }
        public static bool FindSeasEp(string directory, string filename, out int seas, out int ep, out int maxEp, ShowItem si, List<FilenameProcessorRE> rexps)
        {
            return FindSeasEp(directory, filename, out seas, out ep,  out maxEp, si,rexps, out FilenameProcessorRE rex);
        }
        public static bool FindSeasEp(string directory, string filename, out int seas, out int ep, out int maxEp, ShowItem si, List<FilenameProcessorRE> rexps, out FilenameProcessorRE rex)
        {
            string showNameHint = (si != null) ? si.ShowName : "";
            maxEp = -1;
            seas = ep = -1;
            rex = null;

            filename = SimplifyFilename(filename, showNameHint);

            string fullPath = directory + System.IO.Path.DirectorySeparatorChar + filename; // construct full path with sanitised filename

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

                        if (!int.TryParse(m.Groups["s"].ToString(), out seas))
                            seas = -1;
                        if (!int.TryParse(m.Groups["e"].ToString(), out ep))
                            ep = -1;
                        if (!int.TryParse(m.Groups["f"].ToString(), out maxEp))
                            maxEp = -1;

                        rex = re;
                        if ((seas != -1) || (ep != -1))  return true;
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
            public readonly Action TheAction;

            public ProcessActionInfo(int n, Action a)
            {
                this.SemaphoreNumber = n;
                this.TheAction = a;
            }
        };

        #endregion

        private void ReleaseUnmanagedResources()
        {
            this.cacheManager.StopBGDownloadThread();
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                // ReSharper disable once UseNullPropagation
                if (this.ScanProgDlg != null) this.ScanProgDlg.Dispose();

                if (this.cacheManager != null) this.cacheManager.Dispose();
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
    }


}



