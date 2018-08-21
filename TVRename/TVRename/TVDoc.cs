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
using NodaTime.Extensions;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    public class TVDoc : IDisposable
    {
        private readonly DownloadIdentifiersController downloadIdentifiers;
        private readonly List<Finder> finders;
        public readonly ShowLibrary Library;
        public readonly CommandLineArgs Args;
        private TVRenameStats mStats;
        public ItemList TheActionList;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly ActionEngine actionManager;
        private readonly CacheUpdater cacheManager;

        private bool actionCancel;
        public string LoadErr;
        public readonly bool LoadOk;
        private ScanProgress scanProgDlg;
        private bool mDirty;

        // ReSharper disable once RedundantDefaultMemberInitializer
        private bool currentlyBusy = false; // This is set to true when scanning and indicates to other objects not to commence a scan of their own
        private DateTime busySince;

        public TVDoc(FileInfo settingsFile, CommandLineArgs args)
        {
            Args = args;

            Library = new ShowLibrary();
            mStats = new TVRenameStats();
            actionManager = new ActionEngine(mStats);
            cacheManager = new CacheUpdater();

            mDirty = false;
            TheActionList = new ItemList();

            actionCancel = false;

            scanProgDlg = null;

            finders = new List<Finder> //These should be in order
            {
                new FileFinder(this),
                new uTorrentFinder(this),
                new qBitTorrentFinder(this),
                new SABnzbdFinder(this),
                new RSSFinder(this) //RSS Finder Should Be last as it is the finder if all others fail
            };

            downloadIdentifiers = new DownloadIdentifiersController();

            LoadOk = ((settingsFile == null) || LoadXMLSettings(settingsFile)) && TheTVDB.Instance.LoadOk;
        }

        public TVRenameStats Stats()
        {
            mStats.NS_NumberOfShows = Library.Count;
            mStats.NS_NumberOfSeasons = 0;
            mStats.NS_NumberOfEpisodesExpected = 0;

            foreach (ShowItem si in Library.Shows)
            {
                foreach (List<ProcessedEpisode> k in si.SeasonEpisodes.Values)
                    mStats.NS_NumberOfEpisodesExpected += k.Count;

                mStats.NS_NumberOfSeasons += si.SeasonEpisodes.Count;
            }

            return mStats;
        }

        public void SetDirty() => mDirty = true;

        public bool Dirty() => mDirty;

        public void DoActions(ItemList theList)
        {
            actionManager.DoActions(theList, !Args.Hide);
        }

        // ReSharper disable once InconsistentNaming
        public bool DoDownloadsFG()
        {
            ICollection<int> shows = Library.Keys;
            bool returnValue = cacheManager.DoDownloadsFg((!Args.Hide), (!Args.Unattended) && (!Args.Hide), shows);
            Library.GenDict();
            return returnValue;
        }

        // ReSharper disable once InconsistentNaming
        public void DoDownloadsBG()
        {
            ICollection<int> shows = Library.Keys;
            cacheManager.StartBgDownloadThread(false, shows);
        }

        public int DownloadsRemaining() =>
            cacheManager.DownloadDone ? 0 : cacheManager.DownloadsRemaining;

        internal static bool FindSeasEp(FileInfo theFile, out int seasF, out int epF, out int maxEp, ShowItem sI)
        {
            return FindSeasEp(theFile, out seasF, out epF, out maxEp, sI, out FilenameProcessorRE _);
        }

        public void SetSearcher(int n)
        {
            TVSettings.Instance.TheSearchers.SetToNumber(n);
            SetDirty();
        }

        public bool RenameFilesToMatchTorrent(string torrent, string folder, TreeView tvTree, SetProgressDelegate prog,
            bool copyNotMove, string copyDest, CommandLineArgs args)
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
                TheActionList.Add(i);

            return r;
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
                Library.GenDict();
        }

        public static List<FileInfo> FindEpOnDisk(DirFilesCache dfc, ProcessedEpisode pe,
            bool checkDirectoryExist = true)
        {
            return FindEpOnDisk(dfc, pe.Show, pe, checkDirectoryExist);
        }

        private static List<FileInfo> FindEpOnDisk(DirFilesCache dfc, ShowItem si, Episode epi,
            bool checkDirectoryExist = true)
        {
            if (dfc == null)
                dfc = new DirFilesCache();

            List<FileInfo> ret = new List<FileInfo>();

            int seasWanted = si.DvdOrder ? epi.TheDvdSeason.SeasonNumber : epi.TheAiredSeason.SeasonNumber;
            int epWanted = si.DvdOrder ? epi.DvdEpNum : epi.AiredEpNum;

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

                    if (!FindSeasEp(fiTemp, out int seasFound, out int epFound, out int _, si)) continue;

                    if (seasFound == -1)
                        seasFound = seasWanted;

                    if ((seasFound == seasWanted) && (epFound == epWanted))
                        ret.Add(fiTemp);
                }
            }

            return ret;
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

                writer.WriteEndElement(); // myshows

                XmlHelper.WriteStringsToXml(TVSettings.Instance.LibraryFolders, writer, "MonitorFolders", "Folder");
                XmlHelper.WriteStringsToXml(TVSettings.Instance.IgnoreFolders, writer, "IgnoreFolders", "Folder");
                XmlHelper.WriteStringsToXml(TVSettings.Instance.DownloadFolders, writer, "FinderSearchFolders",
                    "Folder");
                XmlHelper.WriteStringsToXml(TVSettings.Instance.IgnoredAutoAddHints, writer, "IgnoredAutoAddHints",
                    "Hint");

                writer.WriteStartElement("IgnoreItems");
                foreach (IgnoreItem ii in TVSettings.Instance.Ignore)
                    ii.Write(writer);

                writer.WriteEndElement(); // IgnoreItems

                writer.WriteEndElement(); // tvrename
                writer.WriteEndDocument();
            }

            mDirty = false;
            Stats().Save();
        }

        // ReSharper disable once InconsistentNaming
        private bool LoadXMLSettings(FileInfo from)
        {
            Logger.Info("Loading Settings from {0}", from?.FullName);
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
                    return true; // that's ok
                }

                using (XmlReader reader = XmlReader.Create(from.FullName, settings))
                {
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
                            TVSettings.Instance.load(reader.ReadSubtree());
                            reader.Read();
                        }
                        else if (reader.Name == "MyShows")
                        {
                            Library.LoadFromXml(reader.ReadSubtree());
                            reader.Read();
                        }
                        else if (reader.Name == "MonitorFolders")
                            TVSettings.Instance.LibraryFolders =
                                XmlHelper.ReadStringsFromXml(reader, "MonitorFolders", "Folder");
                        else if (reader.Name == "IgnoreFolders")
                            TVSettings.Instance.IgnoreFolders =
                                XmlHelper.ReadStringsFromXml(reader, "IgnoreFolders", "Folder");
                        else if (reader.Name == "FinderSearchFolders")
                            TVSettings.Instance.DownloadFolders =
                                XmlHelper.ReadStringsFromXml(reader, "FinderSearchFolders", "Folder");
                        else if (reader.Name == "IgnoredAutoAddHints")
                            TVSettings.Instance.IgnoredAutoAddHints =
                                XmlHelper.ReadStringsFromXml(reader, "IgnoredAutoAddHints", "Hint");
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
                Logger.Warn(e, "Problem on Startup loading File");
                LoadErr = from.Name + " : " + e.Message;
                return false;
            }

            try
            {
                mStats = TVRenameStats.Load();
            }
            catch (Exception)
            {
                // not worried if stats loading fails
            }

            return true;
        }

        private void OutputActionFiles(TVSettings.ScanType st)
        {
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
                if (ue.Active() && ue.ApplicableFor(st))
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

        private static bool ListHasMissingItems(ItemList l)
        {
            foreach (Item i in l)
            {
                if (i is ItemMissing)
                    return true;
            }

            return false;
        }

        public void Scan(List<ShowItem> shows, bool unattended, TVSettings.ScanType st)
        {
            try
            {
                PreventAutoScan("Scan "+st.PrettyPrint());

                //Get the defult set of shows defined by the specified type
                if (shows == null) shows = GetShowList(st);

                //If still null then return
                if (shows == null)
                {
                    Logger.Error("No Shows Provided to Scan");
                    return;
                }

                // only check for folders existing for missing check
                if (TVSettings.Instance.MissingCheck && !CheckAllFoldersExist(shows))
                    return;

                if (!DoDownloadsFG())
                    return;

                Thread actionWork = new Thread(ScanWorker) {Name = "ActionWork"};

                actionCancel = false;
                ResetFinders();

                SetupScanUi();

                actionWork.Start(shows.ToList());

                AwaitCancellation(actionWork);

                downloadIdentifiers.Reset();

                OutputActionFiles(st); //Save missing shows to XML (and others)
            }
            catch (Exception e)
            {
                Logger.Fatal(e, "Unhandled Exception in ScanWorker");

            }
            finally
            {
                AllowAutoScan();
            }
        }

        private void AwaitCancellation(Thread actionWork)
        {
            if ((scanProgDlg != null) && (scanProgDlg.ShowDialog() == DialogResult.Cancel))
            {
                actionCancel = true;
                actionWork.Interrupt();
                foreach (Finder f in finders)
                {
                    f.Interrupt();
                }
            }
            else
                actionWork.Join();
        }

        private void SetupScanUi()
        {
            if (!Args.Hide)
            {
                bool anySearchDownloading =
                    finders.Any(x => x.Active() && x.DisplayType() == Finder.FinderDisplayType.downloading);
                bool anyActiveFileFinders =
                    finders.Any(x => x.Active() && x.DisplayType() == Finder.FinderDisplayType.local);
                bool anyActiveRssFinders =
                    finders.Any(x => x.Active() && x.DisplayType() == Finder.FinderDisplayType.rss);

                scanProgDlg = new ScanProgress(
                    TVSettings.Instance.RenameCheck || TVSettings.Instance.MissingCheck,
                    TVSettings.Instance.MissingCheck && anyActiveFileFinders,
                    TVSettings.Instance.MissingCheck && anySearchDownloading,
                    TVSettings.Instance.MissingCheck && anyActiveRssFinders);
            }
            else
                scanProgDlg = null;
        }

        private void ResetFinders()
        {
            foreach (Finder f in finders)
            {
                f.Reset();
            }
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

            foreach (Item item in TheActionList)
            {
                if (item is Action action)
                {
                    theList.Add(action);
                }
            }

            DoActions(theList);
            AllowAutoScan();
        }

        protected internal List<PossibleDuplicateEpisode> FindDoubleEps()
        {
            PreventAutoScan("Find Double Episodes");
            StringBuilder output = new StringBuilder();
            List<PossibleDuplicateEpisode> returnValue = new List<PossibleDuplicateEpisode>();

            output.AppendLine("");
            output.AppendLine("##################################################");
            output.AppendLine("DUPLICATE FINDER - Start");
            output.AppendLine("##################################################");

            DirFilesCache dfc = new DirFilesCache();
            foreach (ShowItem si in Library.Values)
            {
                foreach (KeyValuePair<int, List<ProcessedEpisode>> kvp in si.SeasonEpisodes)
                {
                    //Ignore specials seasons
                    if (kvp.Key == 0) continue;

                    //Ignore seasons that all aired on same date
                    DateTime? seasonMinAirDate = (from pep in kvp.Value select pep.FirstAired).Min();
                    DateTime? seasonMaxAirDate = (from pep in kvp.Value select pep.FirstAired).Max();
                    if ((seasonMaxAirDate.HasValue) && seasonMinAirDate.HasValue &&
                        seasonMaxAirDate == seasonMinAirDate)
                        continue;

                    //Search through each pair of episodes for the same season
                    foreach (ProcessedEpisode pep in kvp.Value)
                    {
                        if (pep.Type == ProcessedEpisode.ProcessedEpisodeType.merged)
                        {
                            output.AppendLine(si.ShowName + " - Season: " + kvp.Key + " - " + pep.NumsAsString() +
                                              " - " + pep.Name + " is:");

                            foreach (Episode sourceEpisode in pep.SourceEpisodes)
                            {
                                output.AppendLine("                      - " + sourceEpisode.AiredEpNum + " - " +
                                                  sourceEpisode.Name);
                            }
                        }

                        foreach (ProcessedEpisode comparePep in kvp.Value)
                        {
                            if (pep.FirstAired.HasValue && comparePep.FirstAired.HasValue &&
                                pep.FirstAired == comparePep.FirstAired && pep.EpisodeId < comparePep.EpisodeId)
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
                                        List<int> otherMovieLengths = new List<int>();
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

                                returnValue.Add(new PossibleDuplicateEpisode(pep, comparePep, kvp.Key, true, sameName,
                                    oneFound, largerFileSize));
                            }
                        }
                    }
                }
            }

            output.AppendLine("##################################################");
            output.AppendLine("DUPLICATE FINDER - End");
            output.AppendLine("##################################################");

            Logger.Info(output.ToString());
            AllowAutoScan();
            return returnValue;
        }

        public void QuickScan() => Scan(null, true, TVSettings.ScanType.Quick);

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

        private bool CheckAllFoldersExist(IEnumerable<ShowItem> showlist)
        {
            // show MissingFolderAction for any folders that are missing
            // return false if user cancels

            if (showlist == null) // nothing specified?
                showlist = Library.Values; // everything

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

                    if ((snum == 0) && (si.CountSpecials))
                        continue; // no specials season, they're merged into the seasons themselves

                    List<string> folders = new List<string>();

                    if (flocs.ContainsKey(snum))
                        folders = flocs[snum];

                    if ((folders.Count == 0) && (!si.AutoAddNewSeasons()))
                        continue; // no folders defined or found, autoadd off, so onto the next

                    if (folders.Count == 0)
                    {
                        // no folders defined for this season, and autoadd didn't find any, so suggest the autoadd folder name instead
                        folders.Add(si.AutoFolderNameForSeason(snum));
                    }

                    foreach (string folderExists in folders)
                    {
                        string folder = folderExists;

                        // generate new filename info
                        // ReSharper disable once RedundantAssignment
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
                                    break;
                                }
                            }

                            if ((di != null) && (di.Exists)) continue;

                            string sn = si.ShowName;
                            string text = snum + " of " + si.MaxSeason();
                            string theFolder = folder;
                            string otherFolder = null;

                            FaResult whatToDo = FaResult.kfaNotSet;

                            if (Args.MissingFolder == CommandLineArgs.MissingFolderBehavior.create)
                                whatToDo = FaResult.kfaCreate;
                            else if (Args.MissingFolder == CommandLineArgs.MissingFolderBehavior.ignore)
                                whatToDo = FaResult.kfaIgnoreOnce;

                            if (Args.Hide && (whatToDo == FaResult.kfaNotSet))
                                whatToDo = FaResult.kfaIgnoreOnce; // default in /hide mode is to ignore

                            if (TVSettings.Instance.AutoCreateFolders && firstAttempt)
                            {
                                whatToDo = FaResult.kfaCreate;
                                firstAttempt = false;
                            }

                            if (whatToDo == FaResult.kfaNotSet)
                            {
                                // no command line guidance, so ask the user
                                MissingFolderAction mfa = new MissingFolderAction(sn, text, theFolder);
                                mfa.ShowDialog();
                                whatToDo = mfa.Result;
                                otherFolder = mfa.FolderName;
                            }

                            if (whatToDo == FaResult.kfaCancel)
                            {
                                return false;
                            }
                            else if (whatToDo == FaResult.kfaCreate)
                            {
                                try
                                {
                                    Logger.Info("Creating directory as it is missing: {0}", folder);
                                    Directory.CreateDirectory(folder);
                                }
                                catch (Exception ioe)
                                {
                                    Logger.Info("Could not directory: {0}", folder);
                                    Logger.Info(ioe);
                                }

                                goAgain = true;
                            }
                            else if (whatToDo == FaResult.kfaIgnoreAlways)
                            {
                                si.IgnoreSeasons.Add(snum);
                                SetDirty();
                                break;
                            }
                            else if (whatToDo == FaResult.kfaIgnoreOnce)
                                break;
                            else if (whatToDo == FaResult.kfaRetry)
                                goAgain = true;
                            else if (whatToDo == FaResult.kfaDifferentFolder)
                            {
                                folder = otherFolder;
                                di = new DirectoryInfo(folder);
                                goAgain = !di.Exists;
                                if (di.Exists && (si.AutoFolderNameForSeason(snum).ToLower() != folder.ToLower()))
                                {
                                    if (!si.ManualFolderLocations.ContainsKey(snum))
                                        si.ManualFolderLocations[snum] = new List<string>();

                                    si.ManualFolderLocations[snum].Add(folder);
                                    SetDirty();
                                }
                            }
                        } while (goAgain);
                    } // for each folder
                } // for each snum
            } // for each show

            return true;
        }

        public void RemoveIgnored()
        {
            ItemList toRemove = new ItemList();
            foreach (Item item in TheActionList)
            {
                foreach (IgnoreItem ii in TVSettings.Instance.Ignore)
                {
                    if (!ii.SameFileAs(item.Ignore)) continue;
                    toRemove.Add(item);
                    break;
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

            if (!string.IsNullOrEmpty(si.AutoAddFolderBase) && (si.AllFolderLocations().Count > 0))
            {
                TheActionList.Add(
                    downloadIdentifiers.ForceUpdateShow(DownloadIdentifier.DownloadType.downloadImage, si));

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
                    TheActionList.Add(
                        downloadIdentifiers.ForceUpdateSeason(DownloadIdentifier.DownloadType.downloadImage, si, folder,
                            snum));
                }
            } // for each season of this show

            RemoveIgnored();
        }

        // ReSharper disable once InconsistentNaming
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
                showList = Library.Values;
            }

            foreach (string dirPath in TVSettings.Instance.DownloadFolders)
            {
                if (!Directory.Exists(dirPath)) continue;

                foreach (string filePath in Directory.GetFiles(dirPath, "*", System.IO.SearchOption.AllDirectories))
                {
                    if (!File.Exists(filePath)) continue;

                    FileInfo fi = new FileInfo(filePath);

                    if (FileHelper.IgnoreFile(fi)) continue;

                    List<ShowItem> matchingShows = new List<ShowItem>();

                    foreach (ShowItem si in showList)
                    {
                        if (si.GetSimplifiedPossibleShowNames()
                            .Any(name => FileHelper.SimplifyAndCheckFilename(fi.Name, name)))
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
                            ShowItem si = matchingShows[0]; //Choose the first series
                            FindSeasEp(fi, out int seasF, out int epF, out int _, si, out FilenameProcessorRE _);
                            SeriesInfo s = si.TheSeries();
                            Episode ep = s.GetEpisode(seasF, epF, si.DvdOrder);
                            ProcessedEpisode pep = new ProcessedEpisode(ep, si);
                            Logger.Info(
                                $"Removing {fi.FullName} as it matches {matchingShows[0].ShowName} and no episodes are needed");

                            TheActionList.Add(new ActionDeleteFile(fi, pep, TVSettings.Instance.Tidyup));
                        }
                    }
                }

                foreach (string subDirPath in Directory.GetDirectories(dirPath, "*",
                    System.IO.SearchOption.AllDirectories))
                {
                    if (!Directory.Exists(subDirPath)) continue;

                    DirectoryInfo di = new DirectoryInfo(subDirPath);

                    List<ShowItem> matchingShows = new List<ShowItem>();

                    foreach (ShowItem si in showList)
                    {
                        if (si.GetSimplifiedPossibleShowNames()
                            .Any(name => FileHelper.SimplifyAndCheckFilename(di.Name, name)))
                            matchingShows.Add(si);
                    }

                    if (matchingShows.Count > 0)
                    {
                        bool dirCanBeRemoved = true;

                        foreach (ShowItem si in matchingShows)
                        {
                            if (FileNeeded(di, si, dfc))
                            {
                                Logger.Info($"Not removing {di.FullName} as it may be needed for {si.ShowName}");
                                dirCanBeRemoved = false;
                            }
                        }

                        if (dirCanBeRemoved)
                        {
                            ShowItem si = matchingShows[0]; //Choose the first series
                            FindSeasEp(di, out int seasF, out int epF, si, out FilenameProcessorRE _);
                            SeriesInfo s = si.TheSeries();
                            Episode ep = s.GetEpisode(seasF, epF, si.DvdOrder);
                            ProcessedEpisode pep = new ProcessedEpisode(ep, si);
                            Logger.Info(
                                $"Removing {di.FullName} as it matches {matchingShows[0].ShowName} and no episodes are needed");

                            TheActionList.Add(new ActionDeleteDirectory(di, pep, TVSettings.Instance.Tidyup));
                        }
                    }
                }
            }
        }

        private static bool FileNeeded(FileInfo fi, ShowItem si, DirFilesCache dfc)
        {
            if (FindSeasEp(fi, out int seasF, out int epF, out _, si, out _))
            {
                return EpisodeNeeded(si, dfc, seasF, epF);
            }

            //We may need the file
            return true;
        }

        private static bool FileNeeded(DirectoryInfo di, ShowItem si, DirFilesCache dfc)
        {
            if (FindSeasEp(di, out int seasF, out int epF, si, out _))
            {
                return EpisodeNeeded(si, dfc, seasF, epF);
            }

            //We may need the file
            return true;
        }

        private static bool EpisodeNeeded(ShowItem si, DirFilesCache dfc, int seasF, int epF)
        {
            try
            {
                SeriesInfo s = si.TheSeries();
                Episode ep = s.GetEpisode(seasF, epF, si.DvdOrder);
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
            return true;
        }

        private void MergeLibraryEpisodes(ICollection<ShowItem> showList, DirFilesCache dfc)
        {
            if (!TVSettings.Instance.AutoMergeLibraryEpisodes) return;
            Logger.Info("Scanning for any duplicate episodes in library/monitor folders");
            foreach (ShowItem si in showList)
            {
                if (actionCancel)
                    return;

                if (si.AllFolderLocations().Count == 0) // no folders defined for this show
                    continue; // so, nothing to do.

                // process each folder for each season...

                int[] numbers = new int[si.SeasonEpisodes.Keys.Count];
                si.SeasonEpisodes.Keys.CopyTo(numbers, 0);
                Dictionary<int, List<string>> allFolders = si.AllFolderLocations();

                foreach (int snum in numbers)
                {
                    if (actionCancel)
                        return;

                    if ((si.IgnoreSeasons.Contains(snum)) || (!allFolders.ContainsKey(snum)))
                        continue; // ignore/skip this season

                    if ((snum == 0) && (si.CountSpecials))
                        continue; // don't process the specials season, as they're merged into the seasons themselves

                    // all the folders for this particular season
                    List<string> folders = allFolders[snum];

                    List<ProcessedEpisode> eps = si.SeasonEpisodes[snum];

                    List<ShowRule> rulesToAdd = new List<ShowRule>();

                    foreach (string folder in folders)
                    {
                        if (actionCancel)
                            return;

                        FileInfo[] files = dfc.Get(folder);
                        if (files == null)
                            continue;

                        foreach (FileInfo fi in files)
                        {
                            if (actionCancel)
                                return;

                            if (!TVSettings.Instance.UsefulExtension(fi.Extension, false))
                                continue; //not a video file, so ignore

                            if (!FindSeasEp(fi, out int seasNum, out int epNum, out int maxEp, si,
                                out FilenameProcessorRE _))
                                continue; // can't find season & episode, so this file is of no interest to us

                            if (seasNum == -1)
                                seasNum = snum;

                            int epIdx = eps.FindIndex(x =>
                                ((x.AppropriateEpNum == epNum) && (x.AppropriateSeasonNumber == seasNum)));

                            if (epIdx == -1)
                                continue; // season+episode number don't correspond to any episode we know of from thetvdb

                            ProcessedEpisode ep = eps[epIdx];

                            if (ep.Type != ProcessedEpisode.ProcessedEpisodeType.merged && maxEp != -1)
                            {
                                Logger.Info(
                                    $"Looking at {ep.Show.ShowName} and have identified that episode {epNum} and {maxEp} of season {seasNum} should be merged into one file {fi.FullName}");

                                ShowRule sr = new ShowRule
                                {
                                    DoWhatNow = RuleAction.kMerge,
                                    First = epNum,
                                    Second = maxEp
                                };

                                rulesToAdd.Add(sr);
                            }
                        } // foreach file in folder
                    } // for each folder for this season of this show

                    foreach (ShowRule sr in rulesToAdd)
                    {
                        si.AddSeasonRule(snum, sr);
                        Logger.Info($"Added new rule automatically for {sr}");

                        //Regenerate the episodes with the new rule added
                        ShowLibrary.GenerateEpisodeDict(si);
                    }
                } // for each season of this show
            } // for each show
        }

        private void RenameAndMissingCheck(SetProgressDelegate prog, ICollection<ShowItem> showList)
        {
            TheActionList = new ItemList();

            if (TVSettings.Instance.RenameCheck)
                Stats().RenameChecksDone++;

            if (TVSettings.Instance.MissingCheck)
                Stats().MissingChecksDone++;

            prog.Invoke(0);

            if (showList == null)
            {
                // only do episode count if we're doing all shows and seasons
                mStats.NS_NumberOfEpisodes = 0;
                showList = Library.Values;
            }

            DirFilesCache dfc = new DirFilesCache();

            MergeLibraryEpisodes(showList, dfc);

            int c = 0;
            foreach (ShowItem si in showList)
            {
                if (actionCancel)
                    return;

                Logger.Info("Rename and missing check: " + si.ShowName);
                c++;

                prog.Invoke(100 * c / showList.Count);

                RenameAndMissingCheck(si, dfc);
            } // for each show

            RemoveIgnored();
        }

        private void RenameAndMissingCheck(ShowItem si, DirFilesCache dfc)
        {
            if (si.AllFolderLocations().Count == 0) // no folders defined for this show
                return; // so, nothing to do.

            //This is the code that will iterate over the DownloadIdentifiers and ask each to ensure that
            //it has all the required files for that show
            if (!string.IsNullOrEmpty(si.AutoAddFolderBase) && (si.AllFolderLocations().Count > 0))
            {
                TheActionList.Add(downloadIdentifiers.ProcessShow(si));
            }

            //MS_TODO Put the bannerrefresh period into the settings file, we'll default to 3 months
            DateTime cutOff = DateTime.Now.AddMonths(-3);
            DateTime lastUpdate = si.BannersLastUpdatedOnDisk ?? DateTime.Now.AddMonths(-4);
            bool timeForBannerUpdate = (cutOff.CompareTo(lastUpdate) == 1);

            if (TVSettings.Instance.NeedToDownloadBannerFile() && timeForBannerUpdate)
            {
                TheActionList.Add(
                    downloadIdentifiers.ForceUpdateShow(DownloadIdentifier.DownloadType.downloadImage, si));

                si.BannersLastUpdatedOnDisk = DateTime.Now;
                SetDirty();
            }

            // process each folder for each season...

            int[] numbers = new int[si.SeasonEpisodes.Keys.Count];
            si.SeasonEpisodes.Keys.CopyTo(numbers, 0);
            Dictionary<int, List<string>> allFolders = si.AllFolderLocations();

            int lastSeason = numbers.DefaultIfEmpty(0).Max();

            foreach (int snum in numbers)
            {
                if (actionCancel)
                    return;

                if ((si.IgnoreSeasons.Contains(snum)) || (!allFolders.ContainsKey(snum)))
                    continue; // ignore/skip this season

                if ((snum == 0) && (si.CountSpecials))
                    continue; // don't process the specials season, as they're merged into the seasons themselves

                // all the folders for this particular season
                List<string> folders = allFolders[snum];

                bool folderNotDefined = (folders.Count == 0);
                if (folderNotDefined && (TVSettings.Instance.MissingCheck && !si.AutoAddNewSeasons()))
                    continue; // folder for the season is not defined, and we're not auto-adding it

                List<ProcessedEpisode> eps = si.SeasonEpisodes[snum];
                int maxEpisodeNumber = 0;
                foreach (ProcessedEpisode episode in eps)
                {
                    if (episode.AppropriateEpNum > maxEpisodeNumber)
                        maxEpisodeNumber = episode.AppropriateEpNum;
                }

                // base folder:
                if (!string.IsNullOrEmpty(si.AutoAddFolderBase) && (si.AllFolderLocations(false).Count > 0))
                {
                    // main image for the folder itself
                    TheActionList.Add(downloadIdentifiers.ProcessShow(si));
                }

                foreach (string folder in folders)
                {
                    if (actionCancel)
                        return;

                    FileInfo[] files = dfc.Get(folder);
                    if (files == null)
                        continue;

                    if (TVSettings.Instance.NeedToDownloadBannerFile() && timeForBannerUpdate)
                    {
                        //Image series checks here
                        TheActionList.Add(
                            downloadIdentifiers.ForceUpdateSeason(DownloadIdentifier.DownloadType.downloadImage, si,
                                folder, snum));
                    }

                    bool renCheck =
                        TVSettings.Instance.RenameCheck && si.DoRename &&
                        Directory.Exists(folder); // renaming check needs the folder to exist

                    bool missCheck = TVSettings.Instance.MissingCheck && si.DoMissingCheck;

                    //Image series checks here
                    TheActionList.Add(downloadIdentifiers.ProcessSeason(si, folder, snum));

                    FileInfo[] localEps = new FileInfo[maxEpisodeNumber + 1];

                    int maxEpNumFound = 0;
                    if (!renCheck && !missCheck)
                        continue;

                    foreach (FileInfo fi in files)
                    {
                        if (actionCancel)
                            return;

                        if (!FindSeasEp(fi, out int seasNum, out int epNum, out int _, si,
                            out FilenameProcessorRE _))
                            continue; // can't find season & episode, so this file is of no interest to us

                        if (seasNum == -1)
                            seasNum = snum;

                        int epIdx = eps.FindIndex(x =>
                            ((x.AppropriateEpNum == epNum) && (x.AppropriateSeasonNumber == seasNum)));

                        if (epIdx == -1)
                            continue; // season+episode number don't correspond to any episode we know of from thetvdb

                        ProcessedEpisode ep = eps[epIdx];
                        FileInfo actualFile = fi;

                        if (renCheck && TVSettings.Instance.FileHasUsefulExtension( fi, true, out string otherExtension)) // == RENAMING CHECK ==
                        {
                            string newname = TVSettings.Instance.FilenameFriendly(
                                TVSettings.Instance.NamingStyle.NameFor(ep, otherExtension, folder.Length));

                            if (TVSettings.Instance.RetainLanguageSpecificSubtitles &&
                                fi.IsLanguageSpecificSubtitle(out string subtitleExtension) &&
                                actualFile.Name!=newname)
                            {
                                newname = TVSettings.Instance.FilenameFriendly(
                                    TVSettings.Instance.NamingStyle.NameFor(ep, subtitleExtension,
                                        folder.Length));
                            }

                            if (newname != actualFile.Name)
                            {
                                actualFile = FileHelper.FileInFolder(folder, newname); // rename updates the filename
                                TheActionList.Add(new ActionCopyMoveRename(ActionCopyMoveRename.Op.rename, fi,
                                    actualFile, ep, null, null));

                                //The following section informs the DownloadIdentifers that we already plan to
                                //copy a file inthe appropriate place and they do not need to worry about downloading 
                                //one for that purpse

                                downloadIdentifiers.NotifyComplete(actualFile);
                            }
                        }

                        if (missCheck && TVSettings.Instance.UsefulExtension(fi.Extension, false)
                        ) // == MISSING CHECK part 1/2 ==
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
                            if ((dbep.AppropriateEpNum > maxEpNumFound) || (localEps[dbep.AppropriateEpNum] == null)
                            ) // not here locally
                            {
                                DateTime? dt = dbep.GetAirDateDT(true);
                                bool dtOk = dt != null;

                                bool notFuture =
                                    (dtOk && (dt.Value.CompareTo(today) < 0)); // isn't an episode yet to be aired

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
                                    {
                                        //If the show is in its first season and no episodes have air dates
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
                                    ((si.ForceCheckFuture || notFuture) && dtOk) ||
                                    (si.ForceCheckNoAirdate && !dtOk))
                                {
                                    // then add it as officially missing
                                    TheActionList.Add(new ItemMissing(dbep, folder,
                                        TVSettings.Instance.FilenameFriendly(
                                            TVSettings.Instance.NamingStyle.NameFor(dbep, null, folder.Length))));
                                }
                            }
                            else
                            {
                                // do NFO and thumbnail checks if required
                                FileInfo
                                    filo = localEps[dbep.AppropriateEpNum]; // filename (or future filename) of the file

                                TheActionList.Add(downloadIdentifiers.ProcessEpisode(dbep, filo));
                            }
                        } // up to date check, for each episode in thetvdb

                        TheTVDB.Instance.Unlock("UpToDateCheck");
                    } // if doing missing check
                } // for each folder for this season of this show
            } // for each season of this show
        }

        private static void NoProgress(int pct)
        {
            //Nothign to do - Method is called if we have no UI
        }

        private void ScanWorker(object o)
        {
            try
            {
                List<ShowItem> specific = (List<ShowItem>) (o);

                while (!Args.Hide && ((scanProgDlg == null) || (!scanProgDlg.Ready)))
                    Thread.Sleep(10); // wait for thread to create the dialog

                TheActionList = new ItemList();
                SetProgressDelegate noProgress = NoProgress;

                if (TVSettings.Instance.RenameCheck || TVSettings.Instance.MissingCheck)
                    RenameAndMissingCheck(scanProgDlg == null ? noProgress : scanProgDlg.MediaLibProg,
                        specific);

                if (TVSettings.Instance.RemoveDownloadDirectoriesFiles)
                    FindUnusedFilesInDLDirectory(specific);

                if (TVSettings.Instance.MissingCheck)
                {
                    // have a look around for any missing episodes
                    int activeLocalFinders = 0;
                    int activeRssFinders = 0;
                    int activeDownloadingFinders = 0;

                    foreach (Finder f in finders)
                    {
                        if (!f.Active()) continue;
                        f.ActionList = TheActionList;

                        switch (f.DisplayType())
                        {
                            case Finder.FinderDisplayType.local:
                                activeLocalFinders++;
                                break;
                            case Finder.FinderDisplayType.downloading:
                                activeDownloadingFinders++;
                                break;
                            case Finder.FinderDisplayType.rss:
                                activeRssFinders++;
                                break;
                            default:
                                throw new ArgumentException("Inappropriate displaytype identified " + f.DisplayType());
                        }
                    }

                    int currentLocalFinderId = 0;
                    int currentRssFinderId = 0;
                    int currentDownloadingFinderId = 0;

                    foreach (Finder f in finders)
                    {
                        if (actionCancel)
                        {
                            return;
                        }

                        if (f.Active() && ListHasMissingItems(TheActionList))
                        {
                            int startPos;
                            int endPos;

                            switch (f.DisplayType())
                            {
                                case Finder.FinderDisplayType.local:
                                    currentLocalFinderId++;
                                    startPos = 100 * (currentLocalFinderId - 1) / activeLocalFinders;
                                    endPos = 100 * (currentLocalFinderId) / activeLocalFinders;
                                    f.Check(scanProgDlg == null ? noProgress : scanProgDlg.LocalSearchProg,
                                        startPos, endPos);

                                    break;
                                case Finder.FinderDisplayType.downloading:
                                    currentDownloadingFinderId++;
                                    startPos = 100 * (currentDownloadingFinderId - 1) / activeDownloadingFinders;
                                    endPos = 100 * (currentDownloadingFinderId) / activeDownloadingFinders;
                                    f.Check(scanProgDlg == null ? noProgress : scanProgDlg.DownloadingProg,
                                        startPos, endPos);

                                    break;
                                case Finder.FinderDisplayType.rss:
                                    currentRssFinderId++;
                                    startPos = 100 * (currentRssFinderId - 1) / activeRssFinders;
                                    endPos = 100 * (currentRssFinderId) / activeRssFinders;
                                    f.Check(scanProgDlg == null ? noProgress : scanProgDlg.RSSProg, startPos,
                                        endPos);

                                    break;
                            }

                            RemoveIgnored();
                        }
                    }
                }

                if (actionCancel)
                    return;

                // sort Action list by type
                TheActionList.Sort(new ActionItemSorter()); // was new ActionSorter()

                scanProgDlg?.Done();

                Stats().FindAndOrganisesDone++;
            }
            catch (Exception e)
            {
                Logger.Fatal(e, "Unhandled Exception in ScanWorker");
                scanProgDlg?.Done();
            }
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

            foreach (Match m in Regex.Matches(showNameHint, "(?:^|[^a-z]|\\b)([0-9]{3,})")
            ) // find >= 3 digit numbers in show name
            {
                if (m.Groups.Count > 1) // just in case
                {
                    string number = m.Groups[1].Value;
                    filename = Regex.Replace(filename, "(^|\\W)" + number + "\\b",
                        ""); // remove any occurances of that number in the filename
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
            SeriesInfo ser = TheTVDB.Instance.GetSeries(si.TvdbCode);
            string[] dateFormats = new[] {"yyyy-MM-dd", "dd-MM-yyyy", "MM-dd-yyyy", "yy-MM-dd", "dd-MM-yy", "MM-dd-yy"};
            string filename = fi.Name;
            // force possible date separators to a dash
            filename = filename.Replace("/", "-");
            filename = filename.Replace(".", "-");
            filename = filename.Replace(",", "-");
            filename = filename.Replace(" ", "-");

            ep = -1;
            seas = -1;
            maxEp = -1;
            Dictionary<int, Season> seasonsToUse = si.DvdOrder ? ser.DvdSeasons : ser.AiredSeasons;

            foreach (KeyValuePair<int, Season> kvp in seasonsToUse)
            {
                if (si.IgnoreSeasons.Contains(kvp.Value.SeasonNumber))
                    continue;

                foreach (Episode epi in kvp.Value.Episodes.Values)
                {
                    DateTime? dt = epi.GetAirDateDt(); // file will have local timezone date, not ours
                    if (dt == null)
                        continue;

                    TimeSpan closestDate = TimeSpan.MaxValue;

                    foreach (string dateFormat in dateFormats)
                    {
                        string datestr = dt.Value.ToString(dateFormat);
                        if (filename.Contains(datestr) && DateTime.TryParseExact(datestr, dateFormat,
                                new CultureInfo("en-GB"), DateTimeStyles.None, out DateTime dtInFilename))
                        {
                            TimeSpan timeAgo = DateTime.Now.Subtract(dtInFilename);
                            if (timeAgo < closestDate)
                            {
                                seas = (si.DvdOrder ? epi.DvdSeasonNumber : epi.AiredSeasonNumber);
                                ep = si.DvdOrder ? epi.DvdEpNum : epi.AiredEpNum;
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
            return GetMissingEps(dfc, Library.GetRecentAndFutureEps(dd));
        }

        private List<ProcessedEpisode> GetMissingEps(DirFilesCache dfc, IEnumerable<ProcessedEpisode> lpe)
        {
            List<ProcessedEpisode> missing = new List<ProcessedEpisode>();

            foreach (ProcessedEpisode pe in lpe)
            {
                List<FileInfo> fl = FindEpOnDisk(dfc, pe);

                bool foundOnDisk = ((fl != null) && (fl.Count > 0));
                bool alreadyAired;

                DateTime? airDate = pe.GetAirDateDT(true);

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

                string[] x = Directory.GetFiles(dirPath, "*", System.IO.SearchOption.AllDirectories);
                Logger.Info($"Processing {x.Length} files for shows that need to be scanned");

                foreach (string filePath in x)
                {
                    Logger.Info($"Checking to see whether {filePath} is a file that for a show that need scanning");

                    if (!File.Exists(filePath)) continue;

                    FileInfo fi = new FileInfo(filePath);

                    if (FileHelper.IgnoreFile(fi)) continue;

                    foreach (ShowItem si in Library.Shows)
                    {
                        if (showsToScan.Contains(si)) continue;

                        if (si.GetSimplifiedPossibleShowNames()
                            .Any(name => FileHelper.SimplifyAndCheckFilename(fi.Name, name)))
                            showsToScan.Add(si);
                    }
                }

                string[] directories = Directory.GetDirectories(dirPath, "*", System.IO.SearchOption.AllDirectories);
                Logger.Info($"Processing {directories.Length} directories for shows that need to be scanned");

                foreach (string subDirPath in directories)
                {
                    Logger.Info($"Checking to see whether {subDirPath} has any shows that need scanning");

                    if (!Directory.Exists(subDirPath)) continue;

                    DirectoryInfo di = new DirectoryInfo(subDirPath);

                    foreach (ShowItem si in Library.Values)
                    {
                        if (showsToScan.Contains(si)) continue;

                        if (si.GetSimplifiedPossibleShowNames()
                            .Any(name => FileHelper.SimplifyAndCheckFilename(di.Name, name)))
                            showsToScan.Add(si);
                    }
                }
            }

            return showsToScan;
        }

        internal void ForceRefresh(List<ShowItem> sis)
        {
            PreventAutoScan("Force Refresh");
            if (sis != null)
            {
                foreach (ShowItem si in sis)
                {
                    TheTVDB.Instance.ForgetShow(si.TvdbCode, true);
                }
            }

            DoDownloadsFG();
            AllowAutoScan();
        }

        private static bool FindSeasEp(FileInfo fi, out int seas, out int ep, out int maxEp, ShowItem si,
            out FilenameProcessorRE re)
        {
            return FindSeasEp(fi, out seas, out ep, out maxEp, si, TVSettings.Instance.FNPRegexs,
                TVSettings.Instance.LookForDateInFilename, out re);
        }

        public static bool FindSeasEp(FileInfo fi, out int seas, out int ep, out int maxEp, ShowItem si,
            List<FilenameProcessorRE> rexps, bool doDateCheck, out FilenameProcessorRE re)
        {
            re = null;
            if (fi == null)
            {
                seas = -1;
                ep = -1;
                maxEp = -1;
                return false;
            }

            if (doDateCheck && FindSeasEpDateCheck(fi, out seas, out ep, out maxEp, si))
                return true;

            string filename = fi.Name;
            int l = filename.Length;
            int le = fi.Extension.Length;
            filename = filename.Substring(0, l - le);
            return FindSeasEp(fi.Directory.FullName, filename, out seas, out ep, out maxEp, si, rexps, out re);
        }

        private static bool FindSeasEp(DirectoryInfo di, out int seas, out int ep, ShowItem si,
            out FilenameProcessorRE re)
        {
            List<FilenameProcessorRE> rexps = TVSettings.Instance.FNPRegexs;
            re = null;

            if (di == null)
            {
                seas = -1;
                ep = -1;
                return false;
            }

            return FindSeasEp(di.Parent.FullName, di.Name, out seas, out ep, out int _, si, rexps, out re);
        }

        public static bool FindSeasEp(string directory, string filename, out int seas, out int ep, out int maxEp,
            ShowItem si, List<FilenameProcessorRE> rexps)
        {
            return FindSeasEp(directory, filename, out seas, out ep, out maxEp, si, rexps, out FilenameProcessorRE _);
        }

        public static bool FindSeasEp(string directory, string filename, out int seas, out int ep, out int maxEp,
            ShowItem si, List<FilenameProcessorRE> rexps, out FilenameProcessorRE rex)
        {
            string showNameHint = (si != null) ? si.ShowName : "";
            maxEp = -1;
            seas = ep = -1;
            rex = null;

            filename = SimplifyFilename(filename, showNameHint);

            string fullPath =
                directory + System.IO.Path.DirectorySeparatorChar +
                filename; // construct full path with sanitised filename

            filename = filename.ToLower() + " ";
            fullPath = fullPath.ToLower() + " ";

            foreach (FilenameProcessorRE re in rexps)
            {
                if (!re.Enabled)
                    continue;

                try
                {
                    Match m = Regex.Match(re.UseFullPath ? fullPath : filename, re.RegExpression,
                        RegexOptions.IgnoreCase);

                    if (m.Success)
                    {
                        if (!int.TryParse(m.Groups["s"].ToString(), out seas))
                            seas = -1;

                        if (!int.TryParse(m.Groups["e"].ToString(), out ep))
                            ep = -1;

                        if (!int.TryParse(m.Groups["f"].ToString(), out maxEp))
                            maxEp = -1;

                        rex = re;
                        if ((seas != -1) || (ep != -1)) return true;
                    }
                }
                catch (FormatException)
                {
                }
                catch (ArgumentException)
                {
                }
            }

            return ((seas != -1) || (ep != -1));
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
    }
}
