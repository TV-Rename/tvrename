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
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;
using NLog;
using NodaTime.Extensions;
using TVRename.Forms.Supporting;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using File = Alphaleonis.Win32.Filesystem.File;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    public class TVDoc : IDisposable
    {
        public enum ProviderType
        {
            libraryDefault,
            // ReSharper disable once InconsistentNaming
            TVmaze,
            // ReSharper disable once InconsistentNaming
            TheTVDB,
            // ReSharper disable once InconsistentNaming
            TMDB
        }

        private readonly DownloadIdentifiersController downloadIdentifiers;
        public readonly ShowLibrary TvLibrary;
        public readonly MovieLibrary FilmLibrary;
        public readonly CommandLineArgs Args;
        internal readonly TVRenameStats CurrentStats;
        public readonly ItemList TheActionList;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ActionEngine actionManager;
        private readonly CacheUpdater cacheManager;
        private readonly FindMissingEpisodes localFinders;
        private readonly FindMissingEpisodes searchFinders;
        private readonly FindMissingEpisodes downloadFinders;

        public string? LoadErr;
        public readonly bool LoadOk;
        private ScanProgress? scanProgDlg;
        private bool mDirty;

        // ReSharper disable once RedundantDefaultMemberInitializer
        private bool currentlyBusy = false; // This is set to true when scanning and indicates to other objects not to commence a scan of their own
        private DateTime busySince;
        private bool LastScanComplete { get; set; }
        private TVSettings.ScanType lastScanType;

        public TVDoc(FileInfo? settingsFile, CommandLineArgs args)
        {
            Args = args;

            TvLibrary = new ShowLibrary();
            FilmLibrary = new MovieLibrary();
            cacheManager = new CacheUpdater();
            localFinders = new FindMissingEpisodesLocally(this);
            downloadFinders = new FindMissingEpisodesDownloading(this);
            searchFinders = new FindMissingEpisodesSearch(this);

            mDirty = false;
            TheActionList = new ItemList();

            scanProgDlg = null;

            downloadIdentifiers = new DownloadIdentifiersController();

            LoadOk = (settingsFile is null || LoadXMLSettings(settingsFile)) && TheTVDB.LocalCache.Instance.LoadOk && TMDB.LocalCache.Instance.LoadOk && TVmaze.LocalCache.Instance.LoadOk;
            try
            {
                TheTVDB.LocalCache.Instance.LanguageList = Languages.Load();
            }
            catch (Exception)
            {
                // not worried if language loading fails as we'll repopulate
            }
            try
            {
                CurrentStats = TVRenameStats.Load() ?? new TVRenameStats();
            }
            catch (Exception)
            {
                CurrentStats = new TVRenameStats();
                // not worried if stats loading fails
            }
            actionManager = new ActionEngine(CurrentStats);
        }

        [NotNull]
        public TVRenameStats Stats()
        {
            CurrentStats.NsNumberOfShows = TvLibrary.Count;
            CurrentStats.NsNumberOfMovies = FilmLibrary.Count;
            CurrentStats.NsNumberOfSeasons = 0;
            CurrentStats.NsNumberOfEpisodesExpected = 0;

            foreach (ShowConfiguration si in TvLibrary.Shows)
            {
                foreach (List<ProcessedEpisode> k in si.SeasonEpisodes.Values)
                {
                    CurrentStats.NsNumberOfEpisodesExpected += k.Count;
                }

                CurrentStats.NsNumberOfSeasons += si.SeasonEpisodes.Count;
            }

            return CurrentStats;
        }

        internal void Add(ShowConfiguration newShow)
        {
            TvLibrary.Add(newShow);
            SetDirty();
            ExportMovieInfo();
        }

        private void UpdateIdsFromCache()
        {
            lock (TVmaze.LocalCache.Instance.SERIES_LOCK)
            {
                foreach (CachedSeriesInfo show in TVmaze.LocalCache.Instance.CachedData.Values)
                {
                    ShowConfiguration showConfiguration = TvLibrary.GetShowItem(show.TvdbCode);
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

                foreach (CachedSeriesInfo show in TMDB.LocalCache.Instance.CachedShowData.Values)
                {
                    ShowConfiguration showConfiguration = TvLibrary.GetShowItem(show.TvdbCode);
                    if (showConfiguration is null)
                    {
                        continue;
                    }
                    if (showConfiguration.TmdbCode == 0 || showConfiguration.TmdbCode == -1)
                    {
                        showConfiguration.TmdbCode = show.TmdbCode;
                    }
                    if (showConfiguration.TmdbCode != show.TmdbCode)
                    {
                        Logger.Error($"Issue with copy back of ids {show.Name}: {showConfiguration.TmdbCode} {show.TmdbCode} ");
                    }
                }
            }
        }

        public void SetDirty() => mDirty = true;

        public bool Dirty() => mDirty;

        public void DoActions([NotNull] ItemList theList, IDialogParent owner)
        {
            foreach (Item i in theList)
            {
                if (i is Action a)
                {
                    a.ResetOutcome();
                }
            }

            actionManager.DoActions(theList, !Args.Hide && Environment.UserInteractive,owner);


            IEnumerable<Item?> enumerable = TheActionList.Actions.Where(a => a.Outcome.Done && a.Becomes() != null).Select(a => a.Becomes());
            TheActionList.AddNullableRange(enumerable);

            // remove items from master list, unless it had an error
            TheActionList.RemoveAll(x => x is Action action && action.Outcome.Done && !action.Outcome.Error);

            new CleanUpEmptyLibraryFolders(this).Check(null);

        }

        // ReSharper disable once InconsistentNaming
        public bool DoDownloadsFG(bool unattended,bool tvrMinimised, UI owner)
        {
            List<SeriesSpecifier> shows = TvLibrary.SeriesSpecifiers.Union(FilmLibrary.SeriesSpecifiers).ToList();
            bool showProgress = !Args.Hide && Environment.UserInteractive && !tvrMinimised;
            bool showMsgBox = !unattended && !Args.Unattended && !Args.Hide && Environment.UserInteractive;

            bool returnValue = cacheManager.DoDownloadsFg(showProgress, showMsgBox, shows,owner);
            UpdateIdsFromCache();
            TvLibrary.GenDict();
            return returnValue;
        }

        // ReSharper disable once InconsistentNaming
        public void DoDownloadsBG()
        {
            cacheManager.StartBgDownloadThread(false, TvLibrary.SeriesSpecifiers.Union(FilmLibrary.SeriesSpecifiers).ToList(),false, CancellationToken.None);
        }

        public int DownloadsRemaining() =>
            cacheManager.DownloadDone ? 0 : cacheManager.DownloadsRemaining;

        public void SetSearcher(SearchEngine s)
        {
            TVSettings.Instance.TheSearchers.SetSearchEngine(s);
            SetDirty();
        }

        public void SetMovieSearcher(SearchEngine s)
        {
            TVSettings.Instance.TheMovieSearchers.SetSearchEngine(s);
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

        public static Searchers GetMovieSearchers() => TVSettings.Instance.TheMovieSearchers;

        public void TidyCaches()
        {
            TheTVDB.LocalCache.Instance.Tidy(TvLibrary.Values);
            TVmaze.LocalCache.Instance.Tidy(TvLibrary.Values);
            TMDB.LocalCache.Instance.Tidy(FilmLibrary.Values);
        }

        public void Closing()
        {
            cacheManager.StopBgDownloadThread();
            Stats().Save();
            TheTVDB.LocalCache.Instance.LanguageList?.Save();
        }

        public static void SearchForEpisode(ProcessedEpisode? ep)
        {
            if (ep is null)
            {
                return;
            }

            Helpers.OpenUrl(TVSettings.Instance.BTSearchURL(ep));
        }

        public static void SearchForEpisode(SearchEngine s, ProcessedEpisode? epi)
        {
            if (epi is null)
            {
                return;
            }

            Helpers.OpenUrl(CustomEpisodeName.NameForNoExt(epi, s.Url, true));
        }

        public static void SearchForMovie(MovieConfiguration? ep)
        {
            if (ep is null)
            {
                return;
            }

            Helpers.OpenUrl(TVSettings.Instance.BTMovieSearchURL(ep));
        }

        public static void SearchForMovie(SearchEngine s, MovieConfiguration? epi)
        {
            if (epi is null)
            {
                return;
            }

            Helpers.OpenUrl(CustomMovieName.NameFor(epi, s.Url, true,false));
        }

        public void DoWhenToWatch(bool cachedOnly,bool unattended,bool hidden, UI owner)
        {
            if (!cachedOnly && !DoDownloadsFG(unattended,hidden,owner))
            {
                return;
            }

            if (cachedOnly)
            {
                UpdateIdsFromCache();
                TvLibrary.GenDict();
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
                foreach (ShowConfiguration si in TvLibrary.Values)
                {
                    si.WriteXmlSettings(writer);
                }

                writer.WriteEndElement(); // MyShows

                writer.WriteStartElement("MyMovies");
                foreach (MovieConfiguration? si in FilmLibrary.Values)
                {
                    si.WriteXmlSettings(writer);
                }

                writer.WriteEndElement(); // MyMovies

                XmlHelper.WriteStringsToXml(TVSettings.Instance.LibraryFolders, writer, "MonitorFolders", "Folder");
                XmlHelper.WriteStringsToXml(TVSettings.Instance.MovieLibraryFolders, writer, "MovieLibraryFolders", "Folder");
                XmlHelper.WriteStringsToXml(TVSettings.Instance.IgnoreFolders, writer, "IgnoreFolders", "Folder");
                XmlHelper.WriteStringsToXml(TVSettings.Instance.DownloadFolders, writer, "FinderSearchFolders","Folder");
                XmlHelper.WriteStringsToXml(TVSettings.Instance.IgnoredAutoAddHints, writer, "IgnoredAutoAddHints","Hint");
                writer.WriteStringsToXml(TVSettings.Instance.Ignore, "IgnoreItems","Ignore");
                writer.WriteStringsToXml(TVSettings.Instance.PreviouslySeenEpisodes, "PreviouslySeenEpisodes", "Episode");
                writer.WriteStringsToXml(TVSettings.Instance.PreviouslySeenMovies, "PreviouslySeenMovies", "Movie");

                writer.WriteEndElement(); // tvrename
                writer.WriteEndDocument();
            }

            mDirty = false;
            Stats().Save();
            TheTVDB.LocalCache.Instance.LanguageList?.Save();
        }

        // ReSharper disable once InconsistentNaming
        private bool LoadXMLSettings(FileInfo? from)
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
                TvLibrary.LoadFromXml(x.Descendants("MyShows").First());
                FilmLibrary.LoadFromXml(x.Descendants("MyMovies").FirstOrDefault());
                TVSettings.Instance.IgnoreFolders =
                    x.Descendants("IgnoreFolders").FirstOrDefault()?.ReadStringsFromXml("Folder") ??new List<string>();
                TVSettings.Instance.DownloadFolders =
                    x.Descendants("FinderSearchFolders").FirstOrDefault()?.ReadStringsFromXml("Folder") ?? new List<string>();
                TVSettings.Instance.IgnoredAutoAddHints =
                    x.Descendants("IgnoredAutoAddHints").FirstOrDefault()?.ReadStringsFromXml("Hint") ?? new List<string>();
                TVSettings.Instance.Ignore =
                    x.Descendants("IgnoreItems").FirstOrDefault()?.ReadIiFromXml("Ignore") ??new List<IgnoreItem>();
                TVSettings.Instance.PreviouslySeenEpisodes = new PreviouslySeenEpisodes(x.Descendants("PreviouslySeenEpisodes").FirstOrDefault());
                TVSettings.Instance.PreviouslySeenMovies = new PreviouslySeenMovies(x.Descendants("PreviouslySeenMovies").FirstOrDefault());

                //MonitorFolders are a little more complex as there is a parameter named the same which we need to ignore
                IEnumerable<XElement> mfs = x.Descendants("MonitorFolders");
                foreach (XElement mf in mfs.Where(mf => mf.Descendants("Folder").Any()))
                {
                    TVSettings.Instance.LibraryFolders = mf.ReadStringsFromXml("Folder");
                }

                TVSettings.Instance.MovieLibraryFolders =
                    x.Descendants("MovieLibraryFolders").FirstOrDefault()?.ReadStringsFromXml("Folder") ?? new List<string>();

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
                new MissingMovieXml(TheActionList),
                new MissingMovieCsv(TheActionList),
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

        public void ExportMovieInfo()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                new MoviesTxt(FilmLibrary.GetSortedMovies()).Run();
            }).Start();

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                new MoviesHtml(FilmLibrary.GetSortedMovies()).Run();
            }).Start();
        }

        public void ExportShowInfo()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                new ShowsTXT(TvLibrary.GetSortedShowItems()).Run();
            }).Start();

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                new ShowsHTML(TvLibrary.GetSortedShowItems()).Run();
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

        internal void ShowAddedOrEdited(bool download, bool unattended,bool hidden, UI owner)
        {
            SetDirty();
            if (download)
            {
                if (!DoDownloadsFG(unattended,hidden,owner))
                {
                    return;
                }
            }

            DoWhenToWatch(true, unattended,hidden,owner);

            WriteUpcoming();
            WriteRecent();
           
            ExportShowInfo(); //Save shows list to disk
        }

        internal void MovieAddedOrEdited(bool download, bool unattended, bool hidden, UI owner)
        {
            SetDirty();
            if (download)
            {
                if (!DoDownloadsFG(unattended, hidden, owner))
                {
                    return;
                }
            }

            ExportMovieInfo(); //Save movie list to disk
        }

        public ConcurrentBag<ShowNotFoundException> ShowProblems => cacheManager.Problems;

        public void Scan(IEnumerable<ShowConfiguration>? passedShows, List<MovieConfiguration>? passedMovies, bool unattended, TVSettings.ScanType st, MediaConfiguration.MediaType media, bool hidden, UI owner
        )
        {
            try
            {
                PreventAutoScan("Scan "+st.PrettyPrint());

                //Get the default set of shows defined by the specified type
                List<ShowConfiguration> shows = GetShowList(st, media, passedShows).ToList();
                //Get the default set of shows defined by the specified type
                List<MovieConfiguration> movies = GetMovieList(st,media, passedMovies).ToList();

                //If still null then return
                if (!shows.Any() && !movies.Any()) 
                {
                    Logger.Warn("No Shows/Movies Provided to Scan");
                    return;
                }

                if (!DoDownloadsFG(unattended,hidden,owner))
                {
                    Logger.Warn("Scan stopped as updates failed");
                    return;
                }

                Thread actionWork = new Thread(ScanWorker) {Name = "Scan Thread"};
                CancellationTokenSource cts = new CancellationTokenSource();
                actionWork.SetApartmentState(ApartmentState.STA); //needed to allow DragDrop on any UI this thread creates

                SetupScanUi(hidden);

                actionWork.Start(new ScanSettings(shows,movies,unattended,hidden,st,cts.Token,owner));

                ShowDialogAndWait(owner, cts, actionWork);

                RemoveDuplicateDownloads(unattended,owner);

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

        private void ShowDialogAndWait(UI owner, CancellationTokenSource cts, Thread actionWork)
        {
            if (scanProgDlg != null)
            {
                owner.ShowChildDialog(scanProgDlg);
                DialogResult ccresult = scanProgDlg.DialogResult;

                if (ccresult == DialogResult.Cancel)
                {
                    cts.Cancel();
                }
                else
                {
                    actionWork.Join();
                }
            }
            else
            {
                actionWork.Join();
            }
        }

        public readonly struct ScanSettings : IEquatable<ScanSettings>
        {
            public readonly bool Unattended;
            public readonly bool Hidden;
            public readonly TVSettings.ScanType Type;
            public readonly List<ShowConfiguration> Shows;
            public readonly List<MovieConfiguration> Movies;
            public readonly CancellationToken Token;
            public readonly UI Owner;

            public ScanSettings(List<ShowConfiguration> list, List<MovieConfiguration> movies, bool unattended, bool hidden, TVSettings.ScanType st,CancellationToken tok, UI owner)
            {
                Shows = list;
                Movies = movies;
                Unattended = unattended;
                Hidden = hidden;
                Type = st;
                Token = tok;
                Owner = owner;
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
                    TVSettings.Instance.RemoveDownloadDirectoriesFiles || TVSettings.Instance.RemoveDownloadDirectoriesFilesMatchMovies || TVSettings.Instance.ReplaceWithBetterQuality,
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

        private IEnumerable<ShowConfiguration> GetShowList(TVSettings.ScanType st,MediaConfiguration.MediaType mt, IEnumerable<ShowConfiguration>? passedShows)
        {
            if (mt == MediaConfiguration.MediaType.movie)
            {
                return new List<ShowConfiguration>();
            }
            return st switch
            {
                TVSettings.ScanType.Full => TvLibrary.GetSortedShowItems(),
                TVSettings.ScanType.Quick => GetQuickShowsToScan(true, true),
                TVSettings.ScanType.Recent => TvLibrary.GetRecentShows(),
                TVSettings.ScanType.SingleShow => passedShows ?? new List<ShowConfiguration>(),
                _ => new List<ShowConfiguration>()
            };
        }

        private IEnumerable<MovieConfiguration> GetMovieList(TVSettings.ScanType st, MediaConfiguration.MediaType mt, IEnumerable<MovieConfiguration>? passedShows)
        {
            if (mt == MediaConfiguration.MediaType.tv)
            {
                return new List<MovieConfiguration>();
            }
            return st switch
            {
                TVSettings.ScanType.Full => FilmLibrary.GetSortedMovies(),
                TVSettings.ScanType.Quick => GetQuickMoviesToScan( true),
                TVSettings.ScanType.Recent => TVSettings.Instance.IncludeMoviesQuickRecent ? FilmLibrary.GetSortedMovies() : passedShows ?? new List<MovieConfiguration>(),
                TVSettings.ScanType.SingleShow => passedShows ?? new List<MovieConfiguration>(),
                _ => new List<MovieConfiguration>()
            };
        }

        public void DoAllActions(IDialogParent owner)
        {
            PreventAutoScan("Do all actions");
            ItemList theList = new ItemList();

            theList.AddRange(TheActionList.Actions);

            DoActions(theList,owner);
            AllowAutoScan();
        }

        [NotNull]
        private IEnumerable<ShowConfiguration> GetQuickShowsToScan(bool doMissingRecents, bool doFilesInDownloadDir)
        {
            List<ShowConfiguration> showsToScan = new List<ShowConfiguration>();
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

        private IEnumerable<MovieConfiguration> GetQuickMoviesToScan(bool doFilesInDownloadDir)
        {
            List<MovieConfiguration> showsToScan = new List<MovieConfiguration>();
            if (doFilesInDownloadDir)
            {
                showsToScan = GetMoviesThatHaveDownloads();
            }

            return showsToScan;
        }

        public void RemoveIgnored()
        {
            ItemList toRemove = new ItemList();
            int numberIgnored=0;
            int numberPreviouslySeen = 0;
            int numberPreviouslySeenMovies = 0;
            foreach (Item item in TheActionList)
            {
                if (TVSettings.Instance.Ignore.Any(ii => ii.SameFileAs(item.Ignore)))
                {
                    toRemove.Add(item);
                    numberIgnored++;
                }

                if (TVSettings.Instance.IgnorePreviouslySeen)
                {
                    if (TVSettings.Instance.PreviouslySeenEpisodes.Includes(item) && item is ItemMissing)
                    {
                        toRemove.Add(item);
                        numberPreviouslySeen++;
                    }
                }

                // TODO Ensure Ingore PreviouslySeen Movies works
                // if (TVSettings.Instance.IgnorePreviouslySeenMovies)
                // {
                //     if (TVSettings.Instance.PreviouslySeenMovies.Includes(item.co) && item is ItemMissing)
                //     {
                //         toRemove.Add(item);
                //         numberPreviouslySeenMovies++;
                //     }
                // }
            }

            Logger.Info($"Removing {toRemove.Count} items from the missing items because they are either in the ignore list ({numberIgnored}) or you have ignore previously seen episodes enables ({numberPreviouslySeen}) or you have ignore previously seen movies enables ({numberPreviouslySeenMovies})");

            foreach (Item action in toRemove)
            {
                TheActionList.Remove(action);
            }
        }

        public void ForceUpdateImages([NotNull] ShowConfiguration si)
        {
            TheActionList.Clear();

            Logger.Info("*******************************");
            Logger.Info("Force Update Images: " + si.ShowName);

            Dictionary<int, SafeList<string>> allFolders = si.AllExistngFolderLocations();

            if (!String.IsNullOrEmpty(si.AutoAddFolderBase) && allFolders.Any())
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
                SafeList<string> folders = allFolders[snum];

                foreach (string folder in folders)
                {
                    //Image cachedSeries checks here
                    TheActionList.Add(
                        downloadIdentifiers.ForceUpdateSeason(DownloadIdentifier.DownloadType.downloadImage, si, folder,
                            snum));
                }
            } // for each season of this show

            RemoveIgnored();
        }

        public void ForceUpdateImages([NotNull] MovieConfiguration si)
        {
            TheActionList.Clear();
            downloadIdentifiers.Reset();

            Logger.Info("*******************************");
            Logger.Info("Force Update Images: " + si.ShowName);

            // process each folder for each movie...
            foreach (FileInfo? file in  si.Locations
                .Select(s => new DirectoryInfo(s))
                .Where(info => info.Exists)
                .SelectMany(d => d.GetFiles())
                .Where(f => f.IsMovieFile())
                .Distinct()
                .ToList())
            {
                TheActionList.Add(
                    downloadIdentifiers.ForceUpdateMovie(DownloadIdentifier.DownloadType.downloadImage, si,file));

                SetDirty();
            } 

            RemoveIgnored();
        }

        private void ScanWorker(object o)
        {
            try
            {
                ScanSettings settings = (ScanSettings) o;

                while (!Args.Hide && Environment.UserInteractive && (scanProgDlg is null || !scanProgDlg.Ready))
                {
                    Thread.Sleep(10); // wait for thread to create the dialog
                }

                TheActionList.Clear();
                SetProgressDelegate noProgress = NoProgress;
                TheActionList.Clear();


                if (!settings.Unattended && settings.Type != TVSettings.ScanType.SingleShow)
                {
                    new FindNewItemsInDownloadFolders(this).Check(scanProgDlg is null ? noProgress : scanProgDlg.AddNewProg, 0, 50,  settings);
                    new FindNewShowsInLibrary(this).Check(scanProgDlg is null ? noProgress : scanProgDlg.AddNewProg, 50, 100, settings);
                }
                
                new CheckShows(this).Check(scanProgDlg is null ? noProgress : scanProgDlg.MediaLibProg, settings);
                new CleanDownloadDirectory(this).Check(scanProgDlg is null ? noProgress : scanProgDlg.DownloadFolderProg,  settings);
                localFinders.Check(scanProgDlg is null ? noProgress : scanProgDlg.LocalSearchProg,  settings);
                downloadFinders.Check(scanProgDlg is null ? noProgress : scanProgDlg.DownloadingProg,  settings);
                searchFinders.Check(scanProgDlg is null? noProgress : scanProgDlg.ToBeDownloadedProg,  settings);
                new CleanUpTorrents(this).Check(scanProgDlg is null ? noProgress : scanProgDlg.ToBeDownloadedProg, settings);

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

        private void RemoveDuplicateDownloads(bool unattended, IDialogParent owner)
        {
            bool cancelAllFuture = false;
            foreach (IGrouping<ItemMissing, ActionTDownload> epGroup in TheActionList.DownloadTorrents
                .GroupBy(item => item.UndoItemMissing)
                .Where(items => items.Count() > 1)
                .OrderBy(grouping => grouping.Key.Show.ShowName))
            {
                List<ActionTDownload> actions = epGroup.ToList();

                if (cancelAllFuture)
                {
                    TheActionList.Replace(actions, actions.First().UndoItemMissing);
                    continue;
                }
                switch (WhatAction(unattended))
                {
                    case TVSettings.DuplicateActionOutcome.IgnoreAll:
                        TheActionList.Replace(actions, actions.First().UndoItemMissing);
                        break;

                    case TVSettings.DuplicateActionOutcome.ChooseFirst:
                        TheActionList.Replace(actions, actions.First());
                        break;

                    case TVSettings.DuplicateActionOutcome.Ask:
                       
                        ChooseDownload form = new ChooseDownload(epGroup.Key, actions);
                        owner.ShowChildDialog(form);
                        DialogResult dr = form.DialogResult;
                        ActionTDownload userChosenAction = form.UserChosenAction;
                        form.Dispose();

                        if (dr == DialogResult.OK)
                        {
                            TheActionList.Replace(actions, userChosenAction);
                        }
                        else if (dr == DialogResult.Abort)
                        {
                            //Cancel all future
                            cancelAllFuture = true;
                            TheActionList.Replace(actions, actions.First().UndoItemMissing);
                        }
                        else
                        {
                            TheActionList.Replace(actions, actions.First().UndoItemMissing);
                        }

                        break;

                    case TVSettings.DuplicateActionOutcome.DoAll:
                        //Don't need to do anything as it's the default behaviour
                        break;

                    case TVSettings.DuplicateActionOutcome.MostSeeders:
                        ActionTDownload bestAction = actions.OrderByDescending(download => download.Seeders).First();
                        TheActionList.Replace(actions, bestAction);
                        break;

                    case TVSettings.DuplicateActionOutcome.Largest:
                        ActionTDownload largestAction = actions.OrderByDescending(download => download.sizeBytes).First();
                        TheActionList.Replace(actions, largestAction);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static TVSettings.DuplicateActionOutcome WhatAction(bool unattended)
        {
            return unattended
                ? TVSettings.Instance.UnattendedMultiActionOutcome
                : TVSettings.Instance.UserMultiActionOutcome;
        }

        private static void NoProgress(int pct, string message)
        {
            //Nothing to do - Method is called if we have no UI
        }

        [NotNull]
        private IEnumerable<ProcessedEpisode> GetMissingEps()
        {
            int dd = TVSettings.Instance.WTWRecentDays;
            DirFilesCache dfc = new DirFilesCache();
            return GetMissingEps(dfc, TvLibrary.GetRecentAndFutureEps(dd));
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
        private List<ShowConfiguration> GetShowsThatHaveDownloads()
        {
            //for each directory in settings directory
            //for each file in directory
            //for each saved show (order by recent)
            //does show match selected file?
            //if so add cachedSeries to list of cachedSeries scanned

            List<ShowConfiguration> showsToScan = new List<ShowConfiguration>();

            foreach (string dirPath in TVSettings.Instance.DownloadFolders)
            {
                if (!Directory.Exists(dirPath))
                {
                    continue;
                }

                try{ 
                    string[] x = Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories);
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

                        foreach (ShowConfiguration si in TvLibrary.Shows
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

                        if (!Directory.Exists(subDirPath))
                        {
                            continue;
                        }

                        DirectoryInfo di = new DirectoryInfo(subDirPath);

                        foreach (ShowConfiguration si in TvLibrary.Values
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

        [NotNull]
        private List<MovieConfiguration> GetMoviesThatHaveDownloads()
        {
            //for each directory in settings directory
            //for each file in directory
            //for each saved show (order by recent)
            //does show match selected file?
            //if so add cachedSeries to list of cachedSeries scanned

            List<MovieConfiguration> showsToScan = new List<MovieConfiguration>();

            foreach (string dirPath in TVSettings.Instance.DownloadFolders)
            {
                if (!Directory.Exists(dirPath))
                {
                    continue;
                }

                try
                {
                    string[] x = Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories);
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

                        foreach (MovieConfiguration si in FilmLibrary.Movies
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
                catch (DirectoryNotFoundException ex)
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
                try
                {
                    string[] directories = Directory.GetDirectories(dirPath, "*", SearchOption.AllDirectories);
                    Logger.Info($"Processing {directories.Length} directories for shows that need to be scanned");

                    foreach (string subDirPath in directories)
                    {
                        Logger.Info($"Checking to see whether {subDirPath} has any shows that need scanning");

                        if (!Directory.Exists(subDirPath))
                        {
                            continue;
                        }

                        DirectoryInfo di = new DirectoryInfo(subDirPath);

                        foreach (MovieConfiguration si in FilmLibrary.Values
                            .Where(si => !showsToScan.Contains(si))
                            .Where(si => si.NameMatch(di, TVSettings.Instance.UseFullPathNameToMatchSearchFolders)))
                        {
                            showsToScan.Add(si);
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

        internal void ForceRefreshShows(IEnumerable<ShowConfiguration>? sis, bool unattended,bool tvrMinimised, UI owner)
        {
            PreventAutoScan("Force Refresh");
            if (sis != null)
            {
                foreach (ShowConfiguration si in sis)
                {
                    iTVSource cache = GetCache(si);
                    cache.ForgetShow(si.TvdbCode, si.TVmazeCode, si.TmdbCode, true, si.UseCustomLanguage, si.CustomLanguageCode);
                }
            }

            DoDownloadsFG(unattended, tvrMinimised,owner);
            AllowAutoScan();
        }

        private iTVSource GetCache(ShowConfiguration si)
        {
            switch (si.Provider)
            {
                case ProviderType.TVmaze:
                    return TVmaze.LocalCache.Instance;

                case ProviderType.TheTVDB:
                    return TheTVDB.LocalCache.Instance;

                case ProviderType.TMDB:
                    return TMDB.LocalCache.Instance;

                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        internal void ForceRefreshMovies(IEnumerable<MediaConfiguration>? sis, bool unattended, bool tvrMinimised, UI owner)
        {
            PreventAutoScan("Force Refresh");
            if (sis != null)
            {
                foreach (MediaConfiguration si in sis)
                {
                    switch (si.Provider)
                    {
                        case ProviderType.TMDB:
                            TMDB.LocalCache.Instance.ForgetMovie(si.TmdbCode);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            DoDownloadsFG(unattended, tvrMinimised, owner);
            AllowAutoScan();
        }


        // ReSharper disable once InconsistentNaming
        internal void TVDBServerAccuracyCheck(bool unattended,bool hidden, UI owner)
        {
            PreventAutoScan("TVDB Accuracy Check");
            IEnumerable<CachedSeriesInfo> seriesToUpdate = TheTVDB.LocalCache.Instance.ServerAccuracyCheck();
            IEnumerable<ShowConfiguration> showsToUpdate = seriesToUpdate.Select(info => TvLibrary.GetShowItem(info.TvdbCode));
            ForceRefreshShows(showsToUpdate, unattended, hidden,owner);
            DoDownloadsBG();
            AllowAutoScan();
        }

        private void ReviewAliases()
        {
            foreach (MovieConfiguration? mov in FilmLibrary.Movies)
            {
                foreach (string? al in mov.AliasNames)
                {
                    Logger.Warn($"::{mov.ShowName},{al}");
                }
            }

            foreach (MovieConfiguration? mov in FilmLibrary.Movies)
            {
                if (mov.CachedMovie?.GetAliases() is null)
                {
                    continue;
                }

                foreach (string? al in mov.CachedMovie?.GetAliases())
                {
                    Logger.Warn($"::{mov.ShowName},{al}");
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        internal void TMDBServerAccuracyCheck(bool unattended, bool hidden, UI owner)
        {
            PreventAutoScan("TMDB Accuracy Check");
            IEnumerable<CachedMovieInfo> seriesToUpdate = TMDB.LocalCache.Instance.ServerAccuracyCheck();
            IEnumerable<MovieConfiguration> showsToUpdate = seriesToUpdate.Select(mov => FilmLibrary.GetMovie(mov.TmdbCode) );
            ForceRefreshMovies(showsToUpdate, unattended, hidden, owner);
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

                cacheManager.Dispose();
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
            ExportMovieInfo();
            WriteUpcoming();
            WriteRecent();
        }

        public void ClearShowProblems() => cacheManager.ClearProblems();

        public void ReindexLibrary()
        {
            TvLibrary.ReIndex();
        }

        public void UpdateMissingAction([NotNull] ItemMissing mi, string fileName)
        {
            // make new Item for copying/moving to specified location
            FileInfo from = new FileInfo(fileName);
            FileInfo to = FinderHelper.GenerateTargetName(mi, from);

            // if we're copying/moving a file across, we might also want to make a thumbnail or NFO for it
            DownloadIdentifiersController di = new DownloadIdentifiersController();

            switch (mi)
            {
                case ShowItemMissing sim:
                    TheActionList.Add(
                        new ActionCopyMoveRename(
                            TVSettings.Instance.LeaveOriginals
                                ? ActionCopyMoveRename.Op.copy
                                : ActionCopyMoveRename.Op.move, from, to
                            , sim.MissingEpisode, true, mi, this));
                    TheActionList.Add(di.ProcessEpisode(sim.Episode, to));

                    break;
                case MovieItemMissing mim:
                    TheActionList.Add(
                        new ActionCopyMoveRename(
                            TVSettings.Instance.LeaveOriginals
                                ? ActionCopyMoveRename.Op.copy
                                : ActionCopyMoveRename.Op.move, from, to
                            , mim.MovieConfig, true, mi, this));
                    TheActionList.Add(di.ProcessMovie(mim.MovieConfig, to));

                    break;
            }

            // and remove old Missing item
            TheActionList.Remove(mi);

            //If keep together is active then we may want to copy over related files too
            if (TVSettings.Instance.KeepTogether)
            {
                FileFinder.KeepTogether(TheActionList, false, true, this);
            }
        }

        public void IgnoreSeasonForItem(Item? er)
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

                if (er2.TargetFolder == er.TargetFolder) //ie if they are for the same cachedSeries
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
            ItemMissing? m2 = item.UndoItemMissing;

            if (m2 is null)
            {
                return;
            }

            TheActionList.Add(m2);
            TheActionList.Remove(item);

            //We can remove any CopyMoveActions that are closely related too
            if (!(item is ActionCopyMoveRename i2))
            {
                return;
            }

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

        public void Add(MovieConfiguration found)
        {
            FilmLibrary.Add(found);
            SetDirty();
            ExportMovieInfo();
        }
    }
}
