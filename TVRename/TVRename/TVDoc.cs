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
using Alphaleonis.Win32.Filesystem;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Xml;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using System.Xml.Linq;
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

        public string LoadErr;
        public readonly bool LoadOk;
        private ScanProgress scanProgDlg;
        private bool mDirty;

        // ReSharper disable once RedundantDefaultMemberInitializer
        private bool currentlyBusy = false; // This is set to true when scanning and indicates to other objects not to commence a scan of their own
        private DateTime busySince;
        private bool lastScanComplete;
        private TVSettings.ScanType lastScanType;

        public TVDoc(FileInfo settingsFile, CommandLineArgs args)
        {
            Args = args;

            Library = new ShowLibrary();
            mStats = new TVRenameStats();
            actionManager = new ActionEngine(mStats);
            cacheManager = new CacheUpdater();

            mDirty = false;
            TheActionList = new ItemList();

            scanProgDlg = null;

            finders = new List<Finder> //These should be in order
            {
                new FileFinder(this),
                new uTorrentFinder(this),
                new qBitTorrentFinder(this),
                new SABnzbdFinder(this),
                new JSONFinder(this),
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
            ICollection<SeriesSpecifier> shows = Library.SeriesSpecifiers;
            bool returnValue = cacheManager.DoDownloadsFg((!Args.Hide), (!Args.Unattended) && (!Args.Hide), shows);
            Library.GenDict();
            return returnValue;
        }

        // ReSharper disable once InconsistentNaming
        public void DoDownloadsBG()
        {
            ICollection<SeriesSpecifier> shows = Library.SeriesSpecifiers;
            cacheManager.StartBgDownloadThread(false, shows);
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

        public void DoWhenToWatch(bool cachedOnly)
        {
            if (!cachedOnly && !DoDownloadsFG())
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

                writer.WriteEndElement(); // myshows

                XmlHelper.WriteStringsToXml(TVSettings.Instance.LibraryFolders, writer, "MonitorFolders", "Folder");
                XmlHelper.WriteStringsToXml(TVSettings.Instance.IgnoreFolders, writer, "IgnoreFolders", "Folder");
                XmlHelper.WriteStringsToXml(TVSettings.Instance.DownloadFolders, writer, "FinderSearchFolders","Folder");
                XmlHelper.WriteStringsToXml(TVSettings.Instance.IgnoredAutoAddHints, writer, "IgnoredAutoAddHints","Hint");
                XmlHelper.WriteStringsToXml(TVSettings.Instance.Ignore, writer, "IgnoreItems","Ignore");

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
                    x.Descendants("IgnoreFolders").First().ReadStringsFromXml("Folder");
                TVSettings.Instance.DownloadFolders =
                    x.Descendants("FinderSearchFolders").First().ReadStringsFromXml("Folder");
                TVSettings.Instance.IgnoredAutoAddHints =
                    x.Descendants("IgnoredAutoAddHints").First().ReadStringsFromXml("Hint");
                TVSettings.Instance.Ignore =
                    x.Descendants("IgnoreItems").First().ReadIiFromXml("Ignore");

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
                mStats = TVRenameStats.Load();
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
            if (!lastScanComplete) return;
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

                if (!DoDownloadsFG())
                    return;

                Thread actionWork = new Thread(ScanWorker) {Name = "ActionWork"};
                CancellationTokenSource cts = new CancellationTokenSource();

                ResetFinders();

                SetupScanUi();

                actionWork.Start(new ScanSettings(shows.ToList(),unattended,st,cts.Token));

                AwaitCancellation(actionWork,cts);

                downloadIdentifiers.Reset();
                lastScanComplete = true;
                lastScanType = st;
                OutputActionFiles(); //Save missing shows to XML (and others)
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

        private class ScanSettings
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

        private void AwaitCancellation(Thread actionWork, CancellationTokenSource cts)
        {
            if (scanProgDlg != null && scanProgDlg.ShowDialog() == DialogResult.Cancel)
            {
                cts.Cancel();
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
                    finders.Any(x => x.Active() && x.DisplayType() == Finder.FinderDisplayType.search);

                scanProgDlg = new ScanProgress(
                    TVSettings.Instance.DoBulkAddInScan, 
                    TVSettings.Instance.RenameCheck || TVSettings.Instance.MissingCheck,
                    TVSettings.Instance.RemoveDownloadDirectoriesFiles || TVSettings.Instance.ReplaceWithBetterQuality,
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

            foreach (Action action in TheActionList.Actions())
            {
                    theList.Add(action);
            }

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

        private void CheckAllFoldersExist(ShowItem si, DirFilesCache dfc, bool fullscan)
        {
            if (!si.DoMissingCheck && !si.DoRename)
                return; // skip

            Dictionary<int, List<string>> flocs = si.AllFolderLocations();

            int[] numbers = new int[si.SeasonEpisodes.Keys.Count];
            si.SeasonEpisodes.Keys.CopyTo(numbers, 0);
            foreach (int snum in numbers)
            {
                // show MissingFolderAction for any folders that are missing
                // throw Exception if user cancels

                if (si.IgnoreSeasons.Contains(snum))
                    return; // ignore this season

                if ((snum == 0) && (si.CountSpecials))
                    return; // no specials season, they're merged into the seasons themselves

                List<string> folders = new List<string>();

                if (flocs.ContainsKey(snum))
                    folders = flocs[snum];

                if ((folders.Count == 0) && (!si.AutoAddNewSeasons()))
                    return; // no folders defined or found, autoadd off, so onto the next

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
                            throw new TVRenameOperationInteruptedException();
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

            Dictionary<int, List<string>> allFolders = si.AllFolderLocations();

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

        private void MergeLibraryEpisodes(ShowItem si, DirFilesCache dfc, bool fullscan, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                throw new TVRenameOperationInteruptedException();

            if (!TVSettings.Instance.AutoMergeLibraryEpisodes) return;

            Dictionary<int, List<string>> allFolders = si.AllFolderLocations();

            if (allFolders.Count == 0) // no folders defined for this show
                return; // so, nothing to do.

            int[] numbers = new int[si.SeasonEpisodes.Keys.Count];
            si.SeasonEpisodes.Keys.CopyTo(numbers, 0);

            // process each folder for each season...
            foreach (int snum in numbers)
            {
                if (token.IsCancellationRequested)
                    throw new TVRenameOperationInteruptedException();

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
                    if (token.IsCancellationRequested)
                        throw new TVRenameOperationInteruptedException();

                    FileInfo[] files = dfc.Get(folder);
                    if (files == null)
                        continue;

                    foreach (FileInfo fi in files)
                    {
                        if (token.IsCancellationRequested)
                            throw new TVRenameOperationInteruptedException();

                        if (!TVSettings.Instance.UsefulExtension(fi.Extension, false))
                            continue; //not a video file, so ignore

                        if (!FinderHelper.FindSeasEp(fi, out int seasNum, out int epNum, out int maxEp, si,
                            out TVSettings.FilenameProcessorRE _))
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
        }

        private void CheckShows(SetProgressDelegate prog, ICollection<ShowItem> showList,CancellationToken token)
        {
            TheActionList = new ItemList();
            bool fullScan = (showList.Count == Library.Shows.Count());

            if (TVSettings.Instance.RenameCheck)
                Stats().RenameChecksDone++;

            if (TVSettings.Instance.MissingCheck)
                Stats().MissingChecksDone++;

            if (fullScan)
            {
                // only do episode count if we're doing all shows and seasons
                mStats.NS_NumberOfEpisodes = 0;
                showList = Library.Values;
            }

            DirFilesCache dfc = new DirFilesCache();

            int c = 0;
            prog.Invoke(c, "Checking shows");
            foreach (ShowItem si in showList)
            {
                prog.Invoke(100 * c++ / showList.Count,si.ShowName);
                if (token.IsCancellationRequested)
                    return;

                Logger.Info("Rename and missing check: " + si.ShowName);

                // only check for folders existing for missing check
                if (TVSettings.Instance.MissingCheck)
                {
                    CheckAllFoldersExist(si, dfc, fullScan);
                }

                MergeLibraryEpisodes(si, dfc, fullScan,token);
                RenameAndMissingCheck(si, dfc, fullScan,token);
            } // for each show

            RemoveIgnored();
            prog.Invoke(100, string.Empty);
        }

        private void RenameAndMissingCheck(ShowItem si, DirFilesCache dfc,bool fullscan, CancellationToken token)
        {
            Dictionary<int, List<string>> allFolders = si.AllFolderLocations();
            if (allFolders.Count == 0) // no folders defined for this show
                return; // so, nothing to do.

            //This is the code that will iterate over the DownloadIdentifiers and ask each to ensure that
            //it has all the required files for that show
            if (!string.IsNullOrEmpty(si.AutoAddFolderBase) && (allFolders.Any()))
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

            int lastSeason = numbers.DefaultIfEmpty(0).Max();

            foreach (int snum in numbers)
            {
                if (token.IsCancellationRequested)
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
                if (!string.IsNullOrEmpty(si.AutoAddFolderBase) && (si.AutoAddType!= ShowItem.AutomaticFolderType.none))
                {
                    // main image for the folder itself
                    TheActionList.Add(downloadIdentifiers.ProcessShow(si));
                }

                foreach (string folder in folders)
                {
                    if (token.IsCancellationRequested)
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
                        if (token.IsCancellationRequested)
                            return;

                        if (!FinderHelper.FindSeasEp(fi, out int seasNum, out int epNum, out int _, si,
                            out TVSettings.FilenameProcessorRE _))
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
                            string newName = TVSettings.Instance.FilenameFriendly(
                                TVSettings.Instance.NamingStyle.NameFor(ep, otherExtension, folder.Length));

                            if (TVSettings.Instance.RetainLanguageSpecificSubtitles &&
                                fi.IsLanguageSpecificSubtitle(out string subtitleExtension) &&
                                actualFile.Name!= newName)
                            {
                                newName = TVSettings.Instance.FilenameFriendly(
                                    TVSettings.Instance.NamingStyle.NameFor(ep, subtitleExtension,
                                        folder.Length));
                            }

                            FileInfo newFile = FileHelper.FileInFolder(folder, newName); // rename updates the filename

                            if (newName != actualFile.Name) 
                            {
                                //Check that the file does not already exist
                                //if (FileHelper.FileExistsCaseSensitive(newFile.FullName))
                                if (FileHelper.FileExistsCaseSensitive(files,newFile))
                                {
                                    Logger.Warn($"Identified that {actualFile.FullName} should be renamed to {newName}, but it already exists.");
                                }
                                else
                                {
                                    TheActionList.Add(new ActionCopyMoveRename(ActionCopyMoveRename.Op.rename, fi,
                                        newFile, ep, null, null));

                                    //The following section informs the DownloadIdentifers that we already plan to
                                    //copy a file inthe appropriate place and they do not need to worry about downloading 
                                    //one for that purpose
                                    downloadIdentifiers.NotifyComplete(newFile);

                                    localEps[epNum] = newFile;
                                }
                            }
                        }

                        if (missCheck && TVSettings.Instance.UsefulExtension(fi.Extension, false)
                        ) // == MISSING CHECK part 1/2 ==
                        {
                            // first pass of missing check is to tally up the episodes we do have
                            if (localEps[epNum] is null) localEps[epNum] = actualFile;

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
                                if (fullscan) mStats.NS_NumberOfEpisodes++;

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

        private static void NoProgress(int pct,string message)
        {
            //Nothing to do - Method is called if we have no UI
        }

        private void ScanWorker(object o)
        {
            try
            {
                ScanSettings settings = (ScanSettings) o;

                //When doing a fullscan the showlist is null indicating that all shows should be checked
                List <ShowItem> specific = settings.Shows ?? Library.Values.ToList();

                while (!Args.Hide && ((scanProgDlg == null) || (!scanProgDlg.Ready)))
                    Thread.Sleep(10); // wait for thread to create the dialog

                TheActionList = new ItemList();
                SetProgressDelegate noProgress = NoProgress;

                if (TVSettings.Instance.DoBulkAddInScan && !settings.Unattended)
                    LookForNewShowsInLibrary(scanProgDlg == null ? noProgress : scanProgDlg.AddNewProg,
                        settings.Token);

                if (TVSettings.Instance.RenameCheck || TVSettings.Instance.MissingCheck)
                    CheckShows(scanProgDlg == null ? noProgress : scanProgDlg.MediaLibProg,
                        specific,settings.Token);

                if (TVSettings.Instance.RemoveDownloadDirectoriesFiles || TVSettings.Instance.ReplaceWithBetterQuality)
                    TheActionList.AddNullableRange(DownloadDirectoryReview.Go(
                        scanProgDlg == null ? noProgress : scanProgDlg.DownloadFolderProg, specific,
                        settings.Unattended));

                if (TVSettings.Instance.MissingCheck)
                {
                    // have a look around for any missing episodes
                    (int activeLocalFinders, int activeRssFinders, int activeDownloadingFinders) =
                        GetNumbersOfActiveFinders();

                    foreach (Finder f in finders.Where(f => f.Active()))
                    {
                        f.ActionList = TheActionList;
                    }

                    int currentLocalFinderId = 0;
                    int currentSearchFinderId = 0;
                    int currentDownloadingFinderId = 0;

                    foreach (Finder f in finders.Where(f => f.Active()))
                    {
                        if (settings.Token.IsCancellationRequested)
                        {
                            return;
                        }

                        if (!TheActionList.MissingItems().Any())
                        {
                            continue;
                        }

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
                            case Finder.FinderDisplayType.search:
                                currentSearchFinderId++;
                                startPos = 100 * (currentSearchFinderId - 1) / activeRssFinders;
                                endPos = 100 * (currentSearchFinderId) / activeRssFinders;
                                f.Check(scanProgDlg == null ? noProgress : scanProgDlg.ToBeDownloadedProg, startPos,
                                    endPos);

                                break;
                        }

                        RemoveIgnored();
                    }
                }

                if (settings.Token.IsCancellationRequested)
                    return;

                // sort Action list by type
                TheActionList.Sort(new ActionItemSorter()); // was new ActionSorter()

                Stats().FindAndOrganisesDone++;
            }
            catch (TVRenameOperationInteruptedException e)
            {
                Logger.Warn(e, "Scan cancelled by user");
            }
            catch (Exception e)
            {
                Logger.Fatal(e, "Unhandled Exception in ScanWorker");
            }
            finally
            {
                scanProgDlg?.Done(); 
            }
        }

        private void LookForNewShowsInLibrary(SetProgressDelegate prog, CancellationToken token)
        {
            BulkAddManager bam = new BulkAddManager(this);
            bam.CheckFolders(token,prog);
            foreach (FoundFolder folder in bam.AddItems)
            {
                if (token.IsCancellationRequested)
                    break;

                if (folder.CodeKnown)
                    continue;

                BulkAddManager.GuessShowItem(folder, Library);

                if (folder.CodeKnown)
                    continue;

                FolderMonitorEdit ed = new FolderMonitorEdit(folder);
                if ((ed.ShowDialog() != DialogResult.OK) || (ed.Code == -1))
                    continue;

                folder.TVDBCode = ed.Code;
            }

            if (!bam.AddItems.Any(s => s.CodeKnown)) return;

            bam.AddAllToMyShows();
            Logger.Info("Added new shows called: {0}", string.Join(",", bam.AddItems.Where(s => s.CodeKnown).Select(s=>s.Folder)));

            SetDirty();
            DoDownloadsFG();
            DoWhenToWatch(true);

            WriteUpcoming();
            WriteRecent();
        }

        private ( int activeLocalFinders,  int activeRssFinders,  int activeDownloadingFinders) GetNumbersOfActiveFinders()
        {
            int activeLocalFinders = 0;
            int activeRssFinders = 0;
            int activeDownloadingFinders = 0;
            foreach (Finder f in finders)
            {
                if (!f.Active()) continue;

                switch (f.DisplayType())
                {
                    case Finder.FinderDisplayType.local:
                        activeLocalFinders++;
                        break;
                    case Finder.FinderDisplayType.downloading:
                        activeDownloadingFinders++;
                        break;
                    case Finder.FinderDisplayType.search:
                        activeRssFinders++;
                        break;
                    default:
                        throw new ArgumentException("Inappropriate displaytype identified " + f.DisplayType());
                }
            }

            return (activeLocalFinders, activeRssFinders, activeDownloadingFinders);
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

                try{ 
                    string[] x = Directory.GetFiles(dirPath, "*", System.IO.SearchOption.AllDirectories);
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

                            if (si.GetSimplifiedPossibleShowNames()
                                .Any(name => FileHelper.SimplifyAndCheckFilename(fi.Name, name)))
                                showsToScan.Add(si);
                        }
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    Logger.Warn(ex, $"Could not access files in {dirPath}");
                }

                try { 
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
                catch (UnauthorizedAccessException ex)
                {
                    Logger.Warn(ex, $"Could not access sub-directories in {dirPath}");
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
                    TheTVDB.Instance.ForgetShow(si.TvdbCode, true,si.UseCustomLanguage,si.CustomLanguageCode);
                }
            }

            DoDownloadsFG();
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
    }

    // ReSharper disable once InconsistentNaming
    internal class TVRenameOperationInteruptedException : Exception
    {
    }
}
