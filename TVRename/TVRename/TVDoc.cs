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

using Alphaleonis.Win32.Filesystem;
using NLog;
using NodaTime.Extensions;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using TVRename.Forms;
using TVRename.Settings.AppState;

namespace TVRename;

// ReSharper disable once InconsistentNaming
public class TVDoc : IDisposable
{
    public enum ProviderType
    {
        libraryDefault,

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once IdentifierTypo
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
    internal readonly State CurrentAppState;
    public readonly ItemList TheActionList;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    internal readonly ActionEngine ActionManager;
    private readonly CacheUpdater cacheManager;
    private FindMissingEpisodes? localFinders;
    private FindMissingEpisodes? searchFinders;
    private FindMissingEpisodes? downloadFinders;

    private readonly List<MovieConfiguration> forceMoviesRefresh = new();
    private readonly List<ShowConfiguration> forceShowsRefresh = new();
    private readonly List<MovieConfiguration> forceMoviesScan = new();
    private readonly List<ShowConfiguration> forceShowsScan = new();

    public string? LoadErr;
    public readonly bool LoadOk;
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
        mDirty = false;
        TheActionList = new ItemList();

        downloadIdentifiers = new DownloadIdentifiersController();

        LoadOk = settingsFile is null || LoadXMLSettings(settingsFile);

        try
        {
            CurrentStats = TVRenameStats.Load() ?? new TVRenameStats();
        }
        catch (Exception)
        {
            CurrentStats = new TVRenameStats();
            // not worried if stats loading fails
        }

        try
        {
            CurrentAppState = State.LoadFromDefaultFile();
        }
        catch (Exception ex)
        {
            Logger.Warn(ex, "Could not load app state file.");
            CurrentAppState = new State();
        }
        ActionManager = new ActionEngine(CurrentStats);
    }

    public TVRenameStats Stats()
    {
        CurrentStats.NsNumberOfShows = TvLibrary.Shows.Count();
        CurrentStats.NsNumberOfMovies = FilmLibrary.Movies.Count();
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

    internal bool AlreadyContains(MediaConfiguration show)
    {
        return ContainsMedia(FilmLibrary.Movies.Select(x => (MediaConfiguration)x), show)
               || ContainsMedia(TvLibrary.Shows.Select(x => (MediaConfiguration)x), show);
    }

    public static bool ContainsMedia(IEnumerable<MediaConfiguration> media, MediaConfiguration testMedia)
    {
        return media.Any(testMedium => testMedium.AnyIdsMatch(testMedia));
    }

    #region Denormalisations
    public void UpdateDenormalisations()
    {
        UpdateIdsFromCache();
        UpdateNamesFromCache();
        TvLibrary.GenDict();
        FilmLibrary.UpdateCollectionInformation();
    }

    private void UpdateNamesFromCache()
    {
        foreach (ShowConfiguration show in TvLibrary.Shows)
        {
            CachedSeriesInfo? cachedData = show.CachedShow;
            if (cachedData?.Name.HasValue() == true)
            {
                show.LastName = cachedData.Name;
            }
            if (cachedData?.Imdb.HasValue() == true)
            {
                show.ImdbCode = cachedData.Imdb;
            }
        }

        foreach (MovieConfiguration? show in FilmLibrary.Movies)
        {
            CachedMovieInfo? cachedData = show.CachedMovie;
            if (cachedData?.Name.HasValue() == true)
            {
                show.LastName = cachedData.Name;
            }
            if (cachedData?.Imdb.HasValue() == true)
            {
                show.ImdbCode = cachedData.Imdb;
            }
        }
    }

    private void UpdateIdsFromCache()
    {
        //OK, so we have a flurry of different Ids. There are 2 scenarios:
        // (1) Configuration has a cachedItem that has additional or different information to the main config. (This should be most)
        // (2) There is information in one of the caches that has information that could be useful (most should have been picked up in 1 above)
        // We do them backwards as we perceive the (1) updates are better quality

        CheckForUsefulTVIds(TVmaze.LocalCache.Instance, ProviderType.TheTVDB);
        CheckForUsefulTVIds(TVmaze.LocalCache.Instance, ProviderType.TMDB);

        CheckForUsefulTVIds(TMDB.LocalCache.Instance, ProviderType.TheTVDB);
        CheckForUsefulTVIds(TMDB.LocalCache.Instance, ProviderType.TVmaze);

        CheckForUsefulTVIds(TheTVDB.LocalCache.Instance, ProviderType.TVmaze);
        CheckForUsefulTVIds(TheTVDB.LocalCache.Instance, ProviderType.TMDB);

        //We don't bother checking a non-existent TVMaze movie cache

        CheckForUsefulMovieIds(TMDB.LocalCache.Instance, ProviderType.TheTVDB);
        CheckForUsefulMovieIds(TMDB.LocalCache.Instance, ProviderType.TVmaze);

        CheckForUsefulMovieIds(TheTVDB.LocalCache.Instance, ProviderType.TVmaze);
        CheckForUsefulMovieIds(TheTVDB.LocalCache.Instance, ProviderType.TMDB);

        foreach (ShowConfiguration? show in TvLibrary.Shows)
        {
            UpdateIdsFromCache(show);
        }

        foreach (MovieConfiguration? show in FilmLibrary.Movies)
        {
            CachedMovieInfo? cachedData = show.CachedMovie;
            if (cachedData is null)
            {
                continue;
            }

            show.TmdbCode = GetBestValue(show, cachedData, ProviderType.TMDB, MediaConfiguration.MediaType.movie, $"based on looking up {ProviderType.TMDB.PrettyPrint()} in cache keyed on {show.Provider.PrettyPrint()}");
            show.TvdbCode = GetBestValue(show, cachedData, ProviderType.TheTVDB, MediaConfiguration.MediaType.movie, $"based on looking up {ProviderType.TheTVDB.PrettyPrint()} in cache keyed on {show.Provider.PrettyPrint()}");
            show.TVmazeCode = GetBestValue(show, cachedData, ProviderType.TVmaze, MediaConfiguration.MediaType.movie, $"based on looking up {ProviderType.TVmaze.PrettyPrint()} in cache keyed on {show.Provider.PrettyPrint()}");
        }
    }

    private void UpdateIdsFromCache(ShowConfiguration show)
    {
        CachedSeriesInfo? cachedData = show.CachedShow;
        if (cachedData is null)
        {
            return;
        }

        show.TmdbCode = GetBestValue(show, cachedData, ProviderType.TMDB, MediaConfiguration.MediaType.tv,
            $"based on looking up {ProviderType.TMDB.PrettyPrint()} in cache keyed on {show.Provider.PrettyPrint()}");

        show.TvdbCode = GetBestValue(show, cachedData, ProviderType.TheTVDB, MediaConfiguration.MediaType.tv,
            $"based on looking up {ProviderType.TheTVDB.PrettyPrint()} in cache keyed on {show.Provider.PrettyPrint()}");

        show.TVmazeCode = GetBestValue(show, cachedData, ProviderType.TVmaze, MediaConfiguration.MediaType.tv,
            $"based on looking up {ProviderType.TVmaze.PrettyPrint()} in cache keyed on {show.Provider.PrettyPrint()}");
    }

    // ReSharper disable once InconsistentNaming
    private void CheckForUsefulTVIds(MediaCache cache, ProviderType provider)
    {
        List<CachedSeriesInfo> x;
        lock (cache.SERIES_LOCK)
        {
            x = cache.CachedShowData.Values.Where(show => show.IdCode(provider) > 0).ToList();
        }

        foreach (CachedSeriesInfo cachedData in x)
        {
            ShowConfiguration? showConfiguration = TvLibrary.GetShowItem(cachedData.IdCode(provider), provider);
            if (showConfiguration is null)
            {
                continue;
            }

            string checkName = $"looked in the {cache.Provider().PrettyPrint()} cache based on {provider.PrettyPrint()} [{cachedData.IdCode(provider)}]";
            showConfiguration.TmdbCode = GetBestValue(showConfiguration, cachedData, ProviderType.TMDB, MediaConfiguration.MediaType.tv, checkName);
            showConfiguration.TvdbCode = GetBestValue(showConfiguration, cachedData, ProviderType.TheTVDB, MediaConfiguration.MediaType.tv, checkName);
            showConfiguration.TVmazeCode = GetBestValue(showConfiguration, cachedData, ProviderType.TVmaze, MediaConfiguration.MediaType.tv, checkName);
        }
    }

    private void CheckForUsefulMovieIds(MediaCache cache, ProviderType provider)
    {
        IEnumerable<CachedMovieInfo> x;
        lock (cache.MOVIE_LOCK)
        {
            x = cache.CachedMovieData.Values.Where(show => show.IdCode(provider) > 0).ToList();
        }

        foreach (CachedMovieInfo cachedData in x)
        {
            MovieConfiguration? showConfiguration = FilmLibrary.GetMovie(cachedData.IdCode(provider), provider);
            if (showConfiguration is null)
            {
                continue;
            }
            string checkName = $"looked in the {cache.Provider().PrettyPrint()} cache based on {provider.PrettyPrint()} [{cachedData.IdCode(provider)}]";
            showConfiguration.TmdbCode = GetBestValue(showConfiguration, cachedData, ProviderType.TMDB, MediaConfiguration.MediaType.movie, checkName);
            showConfiguration.TvdbCode = GetBestValue(showConfiguration, cachedData, ProviderType.TheTVDB, MediaConfiguration.MediaType.movie, checkName);
            showConfiguration.TVmazeCode = GetBestValue(showConfiguration, cachedData, ProviderType.TVmaze, MediaConfiguration.MediaType.movie, checkName);
        }
    }

    private int GetBestValue(MediaConfiguration show, CachedMediaInfo cachedData, ProviderType provider, MediaConfiguration.MediaType type, string basedOnInformation)
    {
        int currentValue = show.IdFor(provider);
        int valueFromCache = cachedData.IdCode(provider);

        if (currentValue <= 0 && valueFromCache > 0)
        {
            Logger.Warn($"Updatng media:{type.PrettyPrint()} {show.ShowName} {provider.PrettyPrint()} Id to {valueFromCache}, {basedOnInformation}.");
            return valueFromCache;
        }

        if (currentValue > 0 && valueFromCache > 0 && currentValue != valueFromCache)
        {
            string baseMessage = $"Media:{type.PrettyPrint()}: {show.ShowName} ({show}) has inconsistent {provider.PrettyPrint()} Id: {currentValue} {valueFromCache}, updating to {valueFromCache}, {basedOnInformation} (Cached Value is {cachedData})." + Environment.NewLine
                + $"    Config: {show}" + Environment.NewLine
                + $"    Cache:  {cachedData}";
            switch (show.Media)
            {
                case MediaConfiguration.MediaType.tv:
                    Logger.Error(baseMessage + Environment.NewLine +
                                 $"    TVDB:   {TheTVDB.LocalCache.Instance.GetSeries(show.TvdbId)}" + Environment.NewLine +
                                 $"    TMDB:   {TMDB.LocalCache.Instance.GetSeries(show.TmdbId)}" + Environment.NewLine +
                                 $"    TVMaze: {TVmaze.LocalCache.Instance.GetSeries(show.TvMazeId)}");
                    FullyRefresh((ShowConfiguration)show);
                    break;
                case MediaConfiguration.MediaType.movie:
                    Logger.Error(baseMessage + Environment.NewLine +
                                 $"    TVDB:   {TheTVDB.LocalCache.Instance.GetMovie(show.TvdbId)}" + Environment.NewLine +
                                 $"    TMDB:   {TMDB.LocalCache.Instance.GetMovie(show.TmdbId)}");
                    FullyRefresh((MovieConfiguration)show);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(show),
                        $"GetBestValue: show has invalid Media {show.Media}");
            }
            return valueFromCache;
        }

        return currentValue;
    }

    private void FullyRefresh(ShowConfiguration show)
    {
        forceShowsRefresh.Add(show);
        TheTVDB.LocalCache.Instance.ForgetShow(show.TvdbId);
        TMDB.LocalCache.Instance.ForgetShow(show.TmdbId);
        TVmaze.LocalCache.Instance.ForgetShow(show.TvMazeId);
    }

    private void FullyRefresh(MovieConfiguration show)
    {
        forceMoviesRefresh.Add(show);
        TheTVDB.LocalCache.Instance.ForgetMovie(show.TvdbId);
        TMDB.LocalCache.Instance.ForgetMovie(show.TmdbId);
    }

    #endregion Denormalisations
    public void SetDirty() => mDirty = true;

    public bool Dirty() => mDirty;

    public void SaveSettingsIfNeeded()
    {
        if (Dirty() && TVSettings.Instance.AutoSaveOnExit)
        {
            WriteXMLSettings();
        }
    }

    private void DoActions(ItemList theList, CancellationToken token)
    {
        try
        {
            foreach (Item i in theList)
            {
                if (i is Action a)
                {
                    a.ResetOutcome();
                }
            }

            ActionManager.DoActions(theList, token);
            List<Action> doneActions = TheActionList.Actions.Where(a => a.Outcome.Done && !a.Outcome.Error).ToList();
            List<Item> subsequentItems = doneActions.Select(a => a.Becomes()).OfType<Item>().ToList();

            // remove items from master list, unless it had an error
            TheActionList.Replace(doneActions, subsequentItems);
        }
        catch (Exception e)
        {
            Logger.Fatal(e, "FAILED TO DO ACTIONS");
        }
    }

    private bool DoDownloadsFg(bool unattended, bool tvrMinimised, UI owner)
    {
        List<ISeriesSpecifier> idsToDownload = new(TvLibrary);
        idsToDownload.AddRange(FilmLibrary.Movies);
        return DoDownloadsFGNow(unattended, tvrMinimised, owner, idsToDownload);
    }

    // ReSharper disable once InconsistentNaming
    private bool DoDownloadsFGNow(bool unattended, bool tvrMinimised, UI owner, List<ISeriesSpecifier> passedShows)
    {
        bool showProgress = !Args.Hide && Environment.UserInteractive && !tvrMinimised;
        bool showMsgBox = !unattended && !Args.Unattended && !Args.Hide && Environment.UserInteractive;

        ForceRefreshIdentifiedMedia();
        bool returnValue = cacheManager.DoDownloadsFg(showProgress, showMsgBox, passedShows, owner);
        UpdateDenormalisations();
        return returnValue;
    }

    // ReSharper disable once InconsistentNaming
    public void DoDownloadsBG()
    {
        ForceRefreshIdentifiedMedia();
        List<ISeriesSpecifier> idsToDownload = new(TvLibrary.Shows);
        idsToDownload.AddRange(FilmLibrary.Movies);
        cacheManager.StartBgDownloadThread(false, idsToDownload, false, CancellationToken.None);
    }

    private void ForceRefreshIdentifiedMedia()
    {
        foreach (ShowConfiguration si in forceShowsRefresh)
        {
            ForgetShow(si);
        }
        foreach (MovieConfiguration si in forceMoviesRefresh)
        {
            ForgetMovie(si);
        }
        forceMoviesRefresh.Clear();
        forceShowsRefresh.Clear();
    }

    private static void ForgetMovie(MovieConfiguration si)
    {
        switch (si.Provider)
        {
            case ProviderType.TheTVDB:
                TheTVDB.LocalCache.Instance.ForgetMovie(si);
                break;
            case ProviderType.TMDB:
                TMDB.LocalCache.Instance.ForgetMovie(si);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(si),
                    $"ForgetMovie: si has invalid Provider {si.Provider}");
        }
    }

    private static void ForgetShow(ISeriesSpecifier? si)
    {
        if (si is null)
        {
            return;
        }
        switch (si.Provider)
        {
            case ProviderType.TVmaze:
                TVmaze.LocalCache.Instance.ForgetShow(si);
                break;
            case ProviderType.TheTVDB:
                TheTVDB.LocalCache.Instance.ForgetShow(si);
                break;
            case ProviderType.TMDB:
                TMDB.LocalCache.Instance.ForgetShow(si);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(si),
                    $"ForgetShow: si has invalid Provider {si.Provider}");
        }
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
        TheTVDB.LocalCache.Instance.Tidy(TvLibrary.Shows);
        TheTVDB.LocalCache.Instance.Tidy(FilmLibrary.Movies);
        TVmaze.LocalCache.Instance.Tidy(TvLibrary.Shows);
        TMDB.LocalCache.Instance.Tidy(FilmLibrary.Movies);
        TMDB.LocalCache.Instance.Tidy(TvLibrary.Shows);
    }

    public void Closing()
    {
        cacheManager.StopBgDownloadThread();
        Stats().Save();
    }

    public static void SearchForEpisode(ProcessedEpisode? ep)
    {
        if (ep is null)
        {
            return;
        }

        TVSettings.Instance.BTSearchURL(ep).OpenUrlInBrowser();
    }

    public static void SearchForEpisode(SearchEngine s, ProcessedEpisode? epi)
    {
        if (epi is null)
        {
            return;
        }

        CustomEpisodeName.NameForNoExt(epi, s.Url, true).OpenUrlInBrowser();
    }

    public static void SearchForMovie(MovieConfiguration? ep)
    {
        if (ep is null)
        {
            return;
        }

        TVSettings.Instance.BTMovieSearchURL(ep).OpenUrlInBrowser();
    }

    public static void SearchForMovie(SearchEngine s, MovieConfiguration? epi)
    {
        if (epi is null)
        {
            return;
        }

        CustomMovieName.NameFor(epi, s.Url, true, false).OpenUrlInBrowser();
    }

    // ReSharper disable once InconsistentNaming
    public void WriteXMLSettings()
    {
        Policy retryPolicy = Policy
            .Handle<Exception>()
            .Retry(3, (exception, retryCount) =>
            {
                Logger.Error(exception, $"Retry {retryCount}/3 to save {PathManager.TVDocSettingsFile.FullName}.");
            });

        retryPolicy.Execute(WriteXmlSettingsInternal);
    }
    private void WriteXmlSettingsInternal()
    {
        DirectoryInfo di = PathManager.TVDocSettingsFile.Directory;
        if (!di.Exists)
        {
            di.Create();
        }
        // backup old settings before writing new ones
        FileHelper.Rotate(PathManager.TVDocSettingsFile.FullName);
        Logger.Info($"Saving Settings to {PathManager.TVDocSettingsFile.FullName}");

        XmlWriterSettings settings = new()
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
            foreach (ShowConfiguration si in TvLibrary.Shows)
            {
                si.WriteXmlSettings(writer);
            }

            writer.WriteEndElement(); // MyShows

            writer.WriteStartElement("MyMovies");
            foreach (MovieConfiguration? si in FilmLibrary.Movies)
            {
                si.WriteXmlSettings(writer);
            }

            writer.WriteEndElement(); // MyMovies

            XmlHelper.WriteStringsToXml(TVSettings.Instance.LibraryFolders, writer, "MonitorFolders", "Folder");
            XmlHelper.WriteStringsToXml(TVSettings.Instance.MovieLibraryFolders, writer, "MovieLibraryFolders", "Folder");
            XmlHelper.WriteStringsToXml(TVSettings.Instance.IgnoreFolders, writer, "IgnoreFolders", "Folder");
            XmlHelper.WriteStringsToXml(TVSettings.Instance.DownloadFolders, writer, "FinderSearchFolders", "Folder");
            XmlHelper.WriteStringsToXml(TVSettings.Instance.IgnoredAutoAddHints.Distinct(), writer, "IgnoredAutoAddHints", "Hint");
            writer.WriteStringsToXml(TVSettings.Instance.Ignore, "IgnoreItems", "Ignore");
            writer.WriteStringsToXml(TVSettings.Instance.PreviouslySeenEpisodes, "PreviouslySeenEpisodes", "Episode");
            writer.WriteStringsToXml(TVSettings.Instance.PreviouslySeenMovies, "PreviouslySeenMovies", "Movie");

            writer.WriteEndElement(); // tvrename
            writer.WriteEndDocument();
        }

        mDirty = false;
        Stats().Save();
    }

    // ReSharper disable once InconsistentNaming
    private bool LoadXMLSettings(FileInfo? from)
    {
        Logger.Info($"Loading Settings from {from?.FullName}");
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

            TVSettings.Instance.Load(x.Descendants("Settings").First());
            TvLibrary.LoadFromXml(x.Descendants("MyShows").First());
            FilmLibrary.LoadFromXml(x.Descendants("MyMovies").FirstOrDefault());
            TVSettings.Instance.IgnoreFolders =
                x.Descendants("IgnoreFolders").FirstOrDefault()?.ReadStringsFromXml("Folder").ToSafeList() ?? new SafeList<string>();
            TVSettings.Instance.DownloadFolders =
                x.Descendants("FinderSearchFolders").FirstOrDefault()?.ReadStringsFromXml("Folder").ToSafeList() ?? new SafeList<string>();
            TVSettings.Instance.IgnoredAutoAddHints =
                x.Descendants("IgnoredAutoAddHints").FirstOrDefault()?.ReadStringsFromXml("Hint").ToSafeList() ?? new SafeList<string>();
            TVSettings.Instance.Ignore =
                x.Descendants("IgnoreItems").FirstOrDefault()?.ReadIiFromXml("Ignore").ToSafeList() ?? new SafeList<IgnoreItem>();
            TVSettings.Instance.PreviouslySeenEpisodes = new PreviouslySeenEpisodes(x.Descendants("PreviouslySeenEpisodes").FirstOrDefault());
            TVSettings.Instance.PreviouslySeenMovies = new PreviouslySeenMovies(x.Descendants("PreviouslySeenMovies").FirstOrDefault());

            //MonitorFolders are a little more complex as there is a parameter named the same which we need to ignore
            IEnumerable<XElement> mfs = x.Descendants("MonitorFolders");
            foreach (XElement mf in mfs.Where(mf => mf.Descendants("Folder").Any()))
            {
                TVSettings.Instance.LibraryFolders = mf.ReadStringsFromXml("Folder").ToSafeList();
            }

            TVSettings.Instance.MovieLibraryFolders =
                x.Descendants("MovieLibraryFolders").FirstOrDefault()?.ReadStringsFromXml("Folder").ToSafeList() ?? new SafeList<string>();
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
        List<ActionListExporter> ALExporters = new()
        {
            new MissingXML(TheActionList),
            new MissingCSV(TheActionList),
            new MissingMovieXml(TheActionList),
            new MissingMovieCsv(TheActionList),
            new CopyMoveXml(TheActionList),
            new RenamingXml(TheActionList)
        };

        foreach (ActionListExporter ue in ALExporters.Where(ue => ue.Active() && ue.ApplicableFor(lastScanType)))
        {
            ue.RunAsThread();
        }
    }

    public void ExportMovieInfo()
    {
        List<MovieConfiguration> movieConfigurations = FilmLibrary.GetSortedMovies();
        new MoviesTxt(movieConfigurations).RunAsThread();
        new MoviesHtml(movieConfigurations).RunAsThread();
    }

    public void ExportShowInfo()
    {
        List<ShowConfiguration> sortedShowItems = TvLibrary.GetSortedShowItems();
        new ShowsTXT(sortedShowItems).RunAsThread();
        new ShowsHtml(sortedShowItems).RunAsThread();
    }

    public void WriteUpcoming()
    {
        List<UpcomingExporter> lup = new() { new UpcomingRSS(this), new UpcomingXML(this), new UpcomingiCAL(this), new UpcomingTXT(this) };

        foreach (UpcomingExporter ue in lup)
        {
            ue.RunAsThread();
        }
    }

    public void WriteRecent()
    {
        List<RecentExporter> reps = new() { new RecentASXExporter(this), new RecentM3UExporter(this), new RecentWPLExporter(this), new RecentXSPFExporter(this) };

        foreach (RecentExporter ue in reps)
        {
            ue.RunAsThread();
        }
    }

    internal void Add(List<ShowConfiguration>? newShow, bool showErrors)
    {
        if (newShow is null)
        {
            return;
        }

        foreach (ShowConfiguration show in newShow)
        {
            UpdateIdsFromCache(show);
            TvLibrary.AddShow(show, showErrors);
            if (TvLibrary.Contains(show)) //It might not as it may be a duplicate
            {
                forceShowsRefresh.Add(show);
                forceShowsScan.Add(show);
            }
        }
        SetDirty();
        ExportShowInfo();
    }

    public void Add(List<MovieConfiguration>? newMovie, bool showErrors)
    {
        forceMoviesRefresh.AddNullableRange(newMovie);
        forceMoviesScan.AddNullableRange(newMovie);
        FilmLibrary.AddMovies(newMovie, showErrors);
        SetDirty();
        ExportMovieInfo();
    }

    internal void TvAddedOrEdited(bool download, bool unattended, bool hidden, UI? owner,
        ShowConfiguration show) =>
        TvAddedOrEdited(download, unattended, hidden, owner, show.AsList());

    internal void TvAddedOrEdited(bool download, bool unattended, bool hidden, UI? owner, List<ShowConfiguration> shows)
    {
        SetDirty();

        forceShowsRefresh.AddRange(shows);
        forceShowsScan.AddRange(shows);

        if (download && owner != null)
        {
            if (!DoDownloadsFg(unattended, hidden, owner, shows))
            {
                return;
            }
        }
        else
        {
            UpdateDenormalisations();
        }

        RunExporters();
        SaveSettingsIfNeeded();
    }

    internal void UpdateMedia(bool download, bool unattended, bool hidden, UI owner)
    {
        if (download)
        {
            if (!DoDownloadsFg(unattended, hidden, owner))
            {
                return;
            }
            RunExporters();
        }
        else
        {
            UpdateDenormalisations();
        }
    }

    internal void MoviesAddedOrEdited(bool download, bool unattended, bool hidden, UI? owner,
        MovieConfiguration movie) =>
        MoviesAddedOrEdited(download, unattended, hidden, owner, movie.AsList());

    internal void MoviesAddedOrEdited(bool download, bool unattended, bool hidden, UI? owner, List<MovieConfiguration> movies)
    {
        SetDirty();
        forceMoviesRefresh.AddRange(movies);
        forceMoviesScan.AddRange(movies);

        if (download && owner != null)
        {
            if (!DoDownloadsFg(unattended, hidden, owner, movies))
            {
                return;
            }
        }
        else
        {
            UpdateDenormalisations();
        }
        RunExporters();
        SaveSettingsIfNeeded();
    }

    public IEnumerable<MediaNotFoundException> ShowProblems => cacheManager.Problems.Where(x => x.SourceType == MediaConfiguration.MediaType.tv);
    public IEnumerable<MediaNotFoundException> MovieProblems => cacheManager.Problems.Where(x => x.SourceType == MediaConfiguration.MediaType.movie);

    public bool HasActiveLocalFinders => localFinders?.Active() ?? false;

    public bool HasActiveDownloadFinders => downloadFinders?.Active() ?? false;

    public bool HasActiveSearchFinders => searchFinders?.Active() ?? false;

    public void Scan(ScanSettings settings)
    {
        ScanProgress? scanProgressDlg = settings.UpdateUi;

        try
        {
            Logger.Info("*******************************");
            Logger.Info(settings.GenerateScanMessage());

            PreventAutoScan("Scan " + settings.Type.PrettyPrint());

            UpdateMediaToScan(settings);
            if (settings.Type != TVSettings.ScanType.Incremental)
            {
                TheActionList.Clear();
            }

            if (settings.Type != TVSettings.ScanType.FastSingleShow && settings.AnyMediaToUpdate)
            {
                if (!DoDownloadsFg(settings.Unattended, settings.Hidden, settings.Owner))
                {
                    Logger.Warn("Scan stopped as updates failed");
                    return;
                }
            }

            while (!Args.Hide && Environment.UserInteractive && (scanProgressDlg is null || !scanProgressDlg.Ready))
            {
                Thread.Sleep(10); // wait for thread to create the dialog
            }

            SetProgressDelegate noProgress = NoProgress;

            if (!settings.Unattended && settings.Type != TVSettings.ScanType.SingleShow && settings.Type != TVSettings.ScanType.FastSingleShow && settings.Type != TVSettings.ScanType.Incremental)
            {
                new FindNewItemsInDownloadFolders(this, settings).Check(scanProgressDlg is null ? noProgress : scanProgressDlg.AddNewProg, 0, 50);
                new FindNewShowsInLibrary(this, settings).Check(scanProgressDlg is null ? noProgress : scanProgressDlg.AddNewProg, 50, 100);

                UpdateMediaToScan(settings);
            }

            //If still null then return
            if (!settings.AnyMediaToUpdate)
            {
                Logger.Warn("No Shows/Movies Provided to Scan");
                return;
            }

            new CheckShows(this, settings).Check(scanProgressDlg is null ? noProgress : scanProgressDlg.MediaLibProg);

            if (settings.Type != TVSettings.ScanType.FastSingleShow && settings.Type != TVSettings.ScanType.Incremental)
            {
                new UnArchiveDownloadDirectory(this, settings).Check(scanProgressDlg is null ? noProgress : scanProgressDlg.DownloadFolderProg);
                new CleanDownloadDirectory(this, settings).Check(scanProgressDlg is null ? noProgress : scanProgressDlg.DownloadFolderProg);
            }

            localFinders?.Check(scanProgressDlg is null ? noProgress : scanProgressDlg.LocalSearchProg);
            downloadFinders?.Check(scanProgressDlg is null ? noProgress : scanProgressDlg.DownloadingProg);
            searchFinders?.Check(scanProgressDlg is null ? noProgress : scanProgressDlg.ToBeDownloadedProg);

            if (settings.Type != TVSettings.ScanType.FastSingleShow && settings.Type != TVSettings.ScanType.Incremental)
            {
                new CleanUpTorrents(this, settings).Check(scanProgressDlg is null ? noProgress : scanProgressDlg.ToBeDownloadedProg);
            }

            if (settings.Token.IsCancellationRequested)
            {
                TheActionList.Clear();
                LastScanComplete = false;
                return;
            }

            if (TVSettings.Instance.GroupMissingEpisodesIntoSeasons)
            {
                GroupMissingSeasons();
            }

            // sort Action list by type
            TheActionList.Sort(new ActionItemSorter()); // was new ActionSorter()

            Stats().FindAndOrganisesDone++;

            RemoveDuplicateDownloads(settings.Unattended, settings.Owner);

            downloadIdentifiers.Reset();
            if (settings.Type != TVSettings.ScanType.FastSingleShow)
            {
                forceMoviesScan.Clear();
                forceShowsScan.Clear();
            }

            lastScanType = settings.Type;
            LastScanComplete = true;

            OutputActionFiles(); //Save missing shows to XML (and others)
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
            scanProgressDlg?.Done();
            AllowAutoScan();
        }
    }
    private void GroupMissingSeasons()
    {
        List<IGrouping<(ShowConfiguration? Series, int? SeasonNumberAsInt), ShowItemMissing>> oldActions
            = TheActionList.MissingEpisodes.GroupBy(e => (e.Series, e.SeasonNumberAsInt)).ToList();

        foreach (IGrouping<(ShowConfiguration? Series, int? SeasonNumberAsInt), ShowItemMissing> season in oldActions)
        {
            ShowConfiguration? configuration = season.Key.Series;
            int? seasonNum = season.Key.SeasonNumberAsInt;

            if (configuration != null && seasonNum != null)
            {
                if (configuration.SeasonEpisodes[seasonNum.Value].Count == season.Count() && season.Count() > 1)
                {
                    TheActionList.Replace(season, new ShowSeasonMissing(configuration, seasonNum.Value, season.First().TargetFolder, season.ToList()));
                }
            }
        }
    }

    private void UpdateMediaToScan(ScanSettings settings)
    {
        //Get the default set of shows defined by the specified type
        List<ShowConfiguration> shows = GetShowList(settings.Type, settings.Media, settings.Shows).ToList();
        //Get the default set of shows defined by the specified type
        List<MovieConfiguration> movies = GetMovieList(settings.Type, settings.Media, settings.Movies).ToList();

        if (settings.Type != TVSettings.ScanType.FastSingleShow)
        {
            settings.UpdateShowsAndMovies(shows.Union(forceShowsScan.Where(m => TvLibrary.Contains(m))).ToList(), movies.Union(forceMoviesScan.Where(m => FilmLibrary.Contains(m))).ToList());
        }
    }
    public class ActionSettings
    {
        public readonly bool Unattended;
        public readonly bool DoAll;
        public readonly ItemList Lvr;
        public readonly CancellationTokenSource Token;

        public ActionSettings(bool unattended, bool doAll, ItemList lvr, CancellationTokenSource token)
        {
            Unattended = unattended;
            DoAll = doAll;
            Lvr = lvr;
            Token = token;
        }
    }
    public class ScanSettings
    {
        public readonly bool Unattended;
        public readonly bool Hidden;
        public readonly TVSettings.ScanType Type;
        public List<ShowConfiguration> Shows;
        public List<MovieConfiguration> Movies;
        public readonly CancellationToken Token;
        public readonly UI Owner;
        public readonly MediaConfiguration.MediaType Media;
        public readonly ScanProgress? UpdateUi;

        public ScanSettings(List<ShowConfiguration> shows, List<MovieConfiguration> movies, bool unattended, bool hidden, TVSettings.ScanType st, MediaConfiguration.MediaType media, UI owner, ScanProgress? updateUi, CancellationToken tok)
        {
            Shows = shows;
            Movies = movies;
            Unattended = unattended;
            Hidden = hidden;
            Type = st;
            Token = tok;
            Owner = owner;
            Media = media;
            UpdateUi = updateUi;
        }

        public bool AnyMediaToUpdate => Shows.Any() || Movies.Any();

        public bool Equals(ScanSettings? other) =>
            other != null &&
            Shows == other.Shows &&
            Movies == other.Movies &&
            Unattended == other.Unattended &&
            Hidden == other.Hidden &&
            Type == other.Type &&
            Token == other.Token &&
            Owner == other.Owner &&
            Media == other.Media;

        public void UpdateShowsAndMovies(List<ShowConfiguration> shows, List<MovieConfiguration> movies)
        {
            Shows = shows;
            Movies = movies;
        }

        public string GenerateScanMessage()
        {
            string desc = Unattended ? "unattended " : string.Empty;
            string shows = Shows.Any() ? Shows.Count.ToString() : "all";
            string movies = Movies.Any() ? Movies.Count.ToString() : "all";

            return $"Starting {desc}{Type.PrettyPrint()} {Media.PrettyPrint()} Scan for {shows} shows and {movies} movies...";
        }
    }

    private IEnumerable<ShowConfiguration> GetShowList(TVSettings.ScanType st, MediaConfiguration.MediaType mt, IEnumerable<ShowConfiguration>? passedShows)
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
            TVSettings.ScanType.Incremental => passedShows ?? new List<ShowConfiguration>(),
            TVSettings.ScanType.FastSingleShow => passedShows ?? new List<ShowConfiguration>(),
            _ => passedShows ?? new List<ShowConfiguration>()
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
            TVSettings.ScanType.Quick => GetQuickMoviesToScan(true),
            TVSettings.ScanType.Recent => TVSettings.Instance.IncludeMoviesQuickRecent ? FilmLibrary.GetSortedMovies() : passedShows ?? new List<MovieConfiguration>(),
            TVSettings.ScanType.SingleShow => passedShows ?? new List<MovieConfiguration>(),
            TVSettings.ScanType.FastSingleShow => passedShows ?? new List<MovieConfiguration>(),
            TVSettings.ScanType.Incremental => passedShows ?? new List<MovieConfiguration>(),
            _ => passedShows ?? new List<MovieConfiguration>()
        };
    }

    private IEnumerable<ShowConfiguration> GetQuickShowsToScan(bool doRecentMissing, bool doFilesInDownloadDir)
    {
        List<ShowConfiguration> showsToScan = new();
        if (doFilesInDownloadDir)
        {
            showsToScan = GetShowsThatHaveDownloads();
        }

        if (doRecentMissing)
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
        List<MovieConfiguration> showsToScan = new();
        if (doFilesInDownloadDir)
        {
            showsToScan = GetMoviesThatHaveDownloads();
        }

        return showsToScan;
    }

    public void RemoveIgnored()
    {
        ItemList toRemove = new();
        int numberIgnored = 0;
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

            // TODO Ensure Ignore PreviouslySeen Movies works
            if (TVSettings.Instance.IgnorePreviouslySeenMovies)
            {
                if (TVSettings.Instance.PreviouslySeenMovies.Includes(item) && item is ItemMissing)
                {
                    toRemove.Add(item);
                    numberPreviouslySeenMovies++;
                }
            }
        }

        Logger.Info($"Removing {toRemove.Count} items from the missing items because they are either in the ignore list ({numberIgnored}) or you have ignore previously seen episodes enables ({numberPreviouslySeen}) or you have ignore previously seen movies enables ({numberPreviouslySeenMovies})");

        foreach (Item action in toRemove)
        {
            TheActionList.Remove(action);
        }
    }

    public void ForceUpdateImages(ShowConfiguration si)
    {
        Logger.Info("Force Update Images: " + si.ShowName);

        Dictionary<int, SafeList<string>> allFolders = si.AllExistngFolderLocations();

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
            SafeList<string> folders = allFolders[snum];

            foreach (string folder in folders)
            {
                //Image cachedSeries checks here
                TheActionList.Add(
                    downloadIdentifiers.ForceUpdateSeason(DownloadIdentifier.DownloadType.downloadImage, si, folder,
                        snum));
            }
        } // for each season of this show
    }

    public void ForceUpdateImages(MovieConfiguration si)
    {
        downloadIdentifiers.Reset();

        Logger.Info("Force Update Images: " + si.ShowName);

        // process each folder for each movie...
        foreach (FileInfo? file in si.MovieFiles())
        {
            TheActionList.Add(
                downloadIdentifiers.ForceUpdateMovie(DownloadIdentifier.DownloadType.downloadImage, si, file));

            SetDirty();
        }
    }

    private void RemoveDuplicateDownloads(bool unattended, UI owner)
    {
        bool cancelAllFuture = false;
        foreach (IGrouping<ItemMissing?, ActionTDownload> epGroup in TheActionList.DownloadTorrents
                     .GroupBy(item => item.UndoItemMissing)
                     .Where(items => items.Count() > 1)
                     .Where(items => items.Key != null)
                     .OrderBy(grouping => grouping.Key?.Show.ShowName))
        {
            List<ActionTDownload> actions = epGroup.ToList();

            if (cancelAllFuture)
            {
                TheActionList.Replace(actions, actions.First().UndoItemMissing);
                continue;
            }

            TVSettings.DuplicateActionOutcome duplicateActionOutcome = WhatAction(unattended);
            switch (duplicateActionOutcome)
            {
                case TVSettings.DuplicateActionOutcome.IgnoreAll:
                    TheActionList.Replace(actions, actions.First().UndoItemMissing);
                    break;

                case TVSettings.DuplicateActionOutcome.ChooseFirst:
                    TheActionList.Replace(actions, actions.First());
                    break;

                case TVSettings.DuplicateActionOutcome.Ask:

                    if (epGroup.Key is not null)
                    {
                        (DialogResult dr, ActionTDownload? userChosenAction) = owner.AskAbout(epGroup.Key, actions);
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
                    throw new NotSupportedException($"duplicateActionOutcome = {duplicateActionOutcome} is not supported by {System.Reflection.MethodBase.GetCurrentMethod()}");
            }
        }
    }

    private static TVSettings.DuplicateActionOutcome WhatAction(bool unattended)
    {
        return unattended
            ? TVSettings.Instance.UnattendedMultiActionOutcome
            : TVSettings.Instance.UserMultiActionOutcome;
    }

    private static void NoProgress(int pct, string message, string lastUpdated)
    {
        //Nothing to do - Method is called if we have no UI
    }

    private IEnumerable<ProcessedEpisode> GetMissingEps()
    {
        int dd = TVSettings.Instance.WTWRecentDays;
        DirFilesCache dfc = new();
        return GetMissingEps(dfc, TvLibrary.GetRecentAndFutureEps(dd));
    }

    private static IEnumerable<ProcessedEpisode> GetMissingEps(DirFilesCache dfc, IEnumerable<ProcessedEpisode> lpe)
    {
        List<ProcessedEpisode> missing = new();

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

    private List<ShowConfiguration> GetShowsThatHaveDownloads()
    {
        //for each directory in settings directory
        //for each file in directory
        //for each saved show (order by recent)
        //does show match selected file?
        //if so add cachedSeries to list of cachedSeries scanned

        List<ShowConfiguration> showsToScan = new();

        foreach (string dirPath in TVSettings.Instance.DownloadFolders)
        {
            if (!Directory.Exists(dirPath))
            {
                continue;
            }

            try
            {
                string[] x = Directory.GetFiles(dirPath, "*", System.IO.SearchOption.AllDirectories);
                Logger.Info($"Processing {x.Length} files for shows that need to be scanned");

                foreach (string filePath in x)
                {
                    Logger.Info($"Checking to see whether {filePath} is a file for a show that needs to be scanned");

                    if (!File.Exists(filePath))
                    {
                        continue;
                    }

                    FileInfo fi = new(filePath);

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
            catch (System.IO.DirectoryNotFoundException ex)
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
            try
            {
                string[] directories = Directory.GetDirectories(dirPath, "*", System.IO.SearchOption.AllDirectories);
                Logger.Info($"Processing {directories.Length} directories for shows that need to be scanned");

                foreach (string subDirPath in directories)
                {
                    Logger.Info($"Checking to see whether {subDirPath} has any shows that need scanning");

                    if (!Directory.Exists(subDirPath))
                    {
                        continue;
                    }

                    DirectoryInfo di = new(subDirPath);

                    foreach (ShowConfiguration si in TvLibrary.Shows
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

    private List<MovieConfiguration> GetMoviesThatHaveDownloads()
    {
        //for each directory in settings directory
        //for each file in directory
        //for each saved show (order by recent)
        //does show match selected file?
        //if so add cachedSeries to list of cachedSeries scanned

        List<MovieConfiguration> showsToScan = new();

        foreach (string dirPath in TVSettings.Instance.DownloadFolders)
        {
            if (!Directory.Exists(dirPath))
            {
                continue;
            }

            try
            {
                string[] x = Directory.GetFiles(dirPath, "*", System.IO.SearchOption.AllDirectories);
                Logger.Info($"Processing {x.Length} files for shows that need to be scanned");

                foreach (string filePath in x)
                {
                    Logger.Info($"Checking to see whether {filePath} is a file that for a show that need scanning");

                    if (!File.Exists(filePath))
                    {
                        continue;
                    }

                    FileInfo fi = new(filePath);

                    if (fi.IgnoreFile())
                    {
                        continue;
                    }

                    foreach (MovieConfiguration si in FilmLibrary.Movies
                                 .Where(si => !showsToScan.Contains(si))
                                 .Where(si => si.NameMatch(fi, TVSettings.Instance.UseFullPathNameToMatchSearchFolders))
                                 .ToList())
                    {
                        showsToScan.Add(si);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Warn($"Could not access files in {dirPath} {ex.Message}");
            }
            catch (System.IO.DirectoryNotFoundException ex)
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
            try
            {
                string[] directories = Directory.GetDirectories(dirPath, "*", System.IO.SearchOption.AllDirectories);
                Logger.Info($"Processing {directories.Length} directories for shows that need to be scanned");

                foreach (string subDirPath in directories)
                {
                    Logger.Info($"Checking to see whether {subDirPath} has any shows that need scanning");

                    if (!Directory.Exists(subDirPath))
                    {
                        continue;
                    }

                    DirectoryInfo di = new(subDirPath);

                    foreach (MovieConfiguration si in FilmLibrary.Movies
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

    internal void ForceRefreshShows(IEnumerable<ShowConfiguration>? sis, bool unattended, bool tvrMinimised,
        UI owner) =>
        ForceRefreshShows(sis, unattended, tvrMinimised, owner, true);

    private void ForceRefreshShows(IEnumerable<ShowConfiguration>? sis, bool unattended, bool tvrMinimised, UI owner, bool doDownloads)
    {
        if (sis == null)
        {
            return;
        }

        PreventAutoScan("Force Refresh");
        List<ShowConfiguration> showConfigurations = sis.ToList();

        foreach (ShowConfiguration si in showConfigurations)
        {
            ForgetShow(si);
        }
        if (doDownloads)
        {
            DoDownloadsFg(unattended, tvrMinimised, owner, showConfigurations);
        }
        AllowAutoScan();
    }

    // ReSharper disable once InconsistentNaming
    public static iTVSource GetTVCache(ProviderType p)
    {
        return p switch
        {
            ProviderType.TVmaze => TVmaze.LocalCache.Instance,
            ProviderType.TheTVDB => TheTVDB.LocalCache.Instance,
            ProviderType.TMDB => TMDB.LocalCache.Instance,
            _ => throw new ArgumentOutOfRangeException(nameof(p), p, null)
        };
    }

    public static iMovieSource GetMovieCache(ProviderType p)
    {
        return p switch
        {
            ProviderType.TheTVDB => TheTVDB.LocalCache.Instance,
            ProviderType.TMDB => TMDB.LocalCache.Instance,
            _ => throw new ArgumentOutOfRangeException(nameof(p), p, null)
        };
    }

    public static MediaCache GetMediaCache(ProviderType p)
    {
        return p switch
        {
            ProviderType.TVmaze => TVmaze.LocalCache.Instance,
            ProviderType.TheTVDB => TheTVDB.LocalCache.Instance,
            ProviderType.TMDB => TMDB.LocalCache.Instance,
            _ => throw new ArgumentOutOfRangeException(nameof(p), p, null)
        };
    }

    internal void ForceRefreshMovies(IEnumerable<MovieConfiguration>? sis, bool unattended, bool tvrMinimised,
        UI owner) =>
        ForceRefreshMovies(sis, unattended, tvrMinimised, owner, true);

    private void ForceRefreshMovies(IEnumerable<MovieConfiguration>? sis, bool unattended, bool tvrMinimised, UI owner, bool doDownloads)
    {
        if (sis == null)
        {
            return;
        }

        PreventAutoScan("Force Refresh");
        List<MovieConfiguration> movieConfigurations = sis.ToList();

        foreach (MovieConfiguration si in movieConfigurations)
        {
            ForgetMovie(si);
        }
        if (doDownloads)
        {
            DoDownloadsFg(unattended, tvrMinimised, owner, movieConfigurations);
        }
        AllowAutoScan();
    }

    private bool DoDownloadsFg(bool unattended, bool tvrMinimised, UI owner, IEnumerable<MediaConfiguration> passedShows)
        => DoDownloadsFGNow(unattended, tvrMinimised, owner, new List<ISeriesSpecifier>(passedShows));

    // ReSharper disable once InconsistentNaming
    internal void TVDBServerAccuracyCheck(bool unattended, bool hidden, UI owner)
    {
        PreventAutoScan("TVDB Accuracy Check");
        DoDownloadsFg(unattended, hidden, owner);

        IEnumerable<CachedSeriesInfo> seriesToUpdate = TheTVDB.LocalCache.Instance.ServerTvAccuracyCheck();
        IEnumerable<ShowConfiguration> showsToUpdate = seriesToUpdate.Select(info => TvLibrary.GetShowItem(info.TvdbCode, ProviderType.TheTVDB)).OfType<ShowConfiguration>();
        ForceRefreshShows(showsToUpdate, unattended, hidden, owner);

        IEnumerable<CachedMovieInfo> moviesToUpdate = TheTVDB.LocalCache.Instance.ServerMovieAccuracyCheck();
        IEnumerable<MovieConfiguration> filmsToUpdate = moviesToUpdate.Select(mov => FilmLibrary.GetMovie(mov.TvdbCode, ProviderType.TheTVDB)).OfType<MovieConfiguration>();
        ForceRefreshMovies(filmsToUpdate, unattended, hidden, owner);

        DoDownloadsBG();
        AllowAutoScan();
    }

    // ReSharper disable once UnusedMember.Local
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
            IEnumerable<string>? enumerable = mov.CachedMovie?.GetAliases();
            if (enumerable is null)
            {
                continue;
            }

            foreach (string? al in enumerable)
            {
                Logger.Warn($"::{mov.ShowName},{al}");
            }
        }
    }

    // ReSharper disable once InconsistentNaming
    internal void TMDBServerAccuracyCheck(bool unattended, bool hidden, UI owner)
    {
        PreventAutoScan("TMDB Accuracy Check");
        DoDownloadsFg(unattended, hidden, owner);

        IEnumerable<CachedMovieInfo> moviesToUpdate = TMDB.LocalCache.Instance.ServerMovieAccuracyCheck();
        IEnumerable<MovieConfiguration> filmsToUpdate = moviesToUpdate.Select(mov => FilmLibrary.GetMovie(mov.TmdbCode, ProviderType.TMDB)).OfType<MovieConfiguration>();
        ForceRefreshMovies(filmsToUpdate, unattended, hidden, owner);

        IEnumerable<CachedSeriesInfo> seriesToUpdate = TMDB.LocalCache.Instance.ServerTvAccuracyCheck();
        IEnumerable<ShowConfiguration> showsToUpdate = seriesToUpdate.Select(mov => TvLibrary.GetShowItem(mov.TmdbCode, ProviderType.TMDB)).OfType<ShowConfiguration>();
        ForceRefreshShows(showsToUpdate, unattended, hidden, owner);

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
        currentlyBusy = false;
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

    public void ClearCacheUpdateProblems() => cacheManager.ClearProblems();

    public void UpdateMissingAction(ItemMissing mi, string fileName)
    {
        // make new Item for copying/moving to specified location
        FileInfo from = new(fileName);
        FileInfo to = FinderHelper.GenerateTargetName(mi, from);

        // if we're copying/moving a file across, we might also want to make a thumbnail or NFO for it
        DownloadIdentifiersController di = new();

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
        if (TVSettings.Instance.CopySubsFolders)
        {
            FileFinder.CopySubsFolders(TheActionList, this);
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
        ItemList remove = new();
        foreach (Item action in TheActionList)
        {
            if (action?.Episode?.AppropriateSeasonNumber != snum)
            {
                continue;
            }

            if (action.TargetFolder == er.TargetFolder) //ie if they are for the same cachedSeries
            {
                remove.Add(action);
            }
        }

        foreach (Item action in remove)
        {
            TheActionList.Remove(action);
        }

        if (remove.Any())
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
        if (item is not ActionCopyMoveRename i2)
        {
            return;
        }

        List<Item> toRemove = new();

        foreach (Item a in TheActionList)
        {
            switch (a)
            {
                case ItemMissing:
                    continue;

                case ActionCopyMoveRename i1:
                    {
                        if (i1.From.RemoveExtension(true).StartsWith(i2.From.RemoveExtension(true), StringComparison.Ordinal))
                        {
                            toRemove.Add(i1);
                        }

                        break;
                    }

                case { } ad:
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

    public void RevertSeasonAction(Item item)
    {
        if (item is ShowSeasonMissing ssm)
        {
            TheActionList.Replace(ssm.AsList(), ssm.OriginalItems);
        }
    }
    public void MovieFolderScan(UI ui, string downloadFolder)
    {
        if (!Directory.Exists(downloadFolder))
        {
            Logger.Warn($"Stopping 'Move Movies From' as '{downloadFolder}' does not exist");
            return;
        }
        if (TVSettings.Instance.LibraryFolders.Contains(downloadFolder) || TVSettings.Instance.MovieLibraryFolders.Contains(downloadFolder))
        {
            string msg = $"Stopping 'Move Movies From' from '{downloadFolder}' as it is already a library folder. Either remove the folder from the library in Preferences or use 'Bulk Add' to add TV Shows into the library that are already copied to the library folders.";
            Logger.Warn(msg);
            MessageBox.Show(msg, "Can't scan folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        TheActionList.Clear();

        try
        {
            IEnumerable<string> x = Directory.GetFiles(downloadFolder, "*", System.IO.SearchOption.AllDirectories).OrderBy(s => s).ToList();
            Logger.Info($"Processing {x.Count()} files for movies in '{downloadFolder}' that need to be analysed.");

            foreach (string filePath in x)
            {
                if (!File.Exists(filePath))
                {
                    Logger.Info($"   {filePath} does not exist");
                    continue;
                }

                FileInfo fi = new(filePath);

                if (fi.IgnoreFile())
                {
                    continue;
                }

                Logger.Info($"Checking to see whether {filePath} is a file for a movie that needs to be added or updated");

                List<MovieConfiguration> existingMatchingShows = GetMatchingMovies(fi);

                if (!existingMatchingShows.Any())
                {
                    BonusAutoAdd(fi, ui);
                }
                else
                {
                    //ask user about which show
                    LinkMovie askUser = new(existingMatchingShows, fi);

                    //if user cancelled then move on
                    if (!UiHelpers.ShowDialogAndOK(askUser,ui))
                    {
                        Logger.Info($"User chose to ignore {filePath}");
                        continue;
                    }

                    if (askUser.ChosenShow == null)
                    {
                        //if user selected a new show then
                        BonusAutoAdd(fi, ui);
                    }
                    else
                    {
                        //if user selected a show
                        MergeMovieFileIntoMovieConfig(fi, askUser.ChosenShow, ui);
                    }
                }
            }

            //If keep together is active then we may want to copy over related files too
            if (TVSettings.Instance.KeepTogether)
            {
                FileFinder.KeepTogether(TheActionList, false, true, this);
            }
            if (TVSettings.Instance.CopySubsFolders)
            {
                FileFinder.CopySubsFolders(TheActionList, this);
            }
            MoviesAddedOrEdited(true, false, false, ui, new List<MovieConfiguration>());
        }
        catch (UnauthorizedAccessException ex)
        {
            Logger.Warn($"Could not access files in {downloadFolder} {ex.Message}");
        }
        catch (System.IO.DirectoryNotFoundException ex)
        {
            Logger.Warn($"Could not access files in {downloadFolder} {ex.Message}");
        }
        catch (System.IO.IOException ex)
        {
            Logger.Warn($"Could not access files in {downloadFolder} {ex.Message}");
        }
        catch (NotSupportedException ex)
        {
            Logger.Error($"Please update 'Download Folders' {downloadFolder} is not supported {ex.Message}");
        }
    }

    private List<MovieConfiguration> GetMatchingMovies(FileSystemInfo fi)
    {
        List<MovieConfiguration> matchingMovies = FilmLibrary.GetSortedMovies().Where(mi => mi.NameMatch(fi, TVSettings.Instance.UseFullPathNameToMatchSearchFolders)).ToList();
        return FinderHelper.RemoveShortShows(matchingMovies);
    }

    private void MergeMovieFileIntoMovieConfig(FileInfo fi, MovieConfiguration chosenShow, IDialogParent owner)
    {
        bool fileCanBeDeleted = true;

        foreach (DirectoryInfo folder in chosenShow.Locations.Select(folderName => new DirectoryInfo(folderName)))
        {
            if (HasMissingFiles(chosenShow, folder))
            {
                LinkFileToShow(fi, chosenShow, folder);
                fileCanBeDeleted = false;
            }
            else
            {
                FileInfo incumbent = GetExistingFile(chosenShow, folder);
                if (string.Compare(incumbent.FullName, fi.FullName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    fileCanBeDeleted = false;
                }
                else
                {
                    bool? deleteFile = AskForBetter(fi, incumbent, chosenShow, owner);
                    if (deleteFile is false)
                    {
                        fileCanBeDeleted = false;
                    }
                }
            }
        }

        if (fileCanBeDeleted)
        {
            Logger.Info(
                $"Removing {fi.FullName} as it matches {chosenShow.ShowName} and all existing versions are better quality");

            TheActionList.Add(new ActionDeleteFile(fi, chosenShow, TVSettings.Instance.Tidyup));
        }
    }

    private static FileInfo GetExistingFile(MovieConfiguration chosenShow, DirectoryInfo folder)
    {
        List<FileInfo>? videoFiles = folder.GetFiles().Where(fiTemp => fiTemp.IsMovieFile()).ToList();

        if (videoFiles is null)
        {
            throw new System.IO.FileNotFoundException();
        }

        if (videoFiles.Count != 1)
        {
            Logger.Warn($"{chosenShow.ShowName} has multiple files in {folder.FullName},just considering the first file {videoFiles.First().Name} ");
        }

        return videoFiles.First();
    }

    private void LinkFileToShow(FileInfo fi, MovieConfiguration chosenShow, DirectoryInfo folder)
    {
        string newBase = TVSettings.Instance.FilenameFriendly(chosenShow.ProposedFilename);
        string newName = fi.Name.Replace(fi.MovieFileNameBase(), newBase);
        FileInfo newFile = FileHelper.FileInFolder(folder, newName);

        TheActionList.Add(new ActionCopyMoveRename(fi, newFile, chosenShow, this));

        // if we're copying/moving a file across, we might also want to make a thumbnail or NFO for it
        TheActionList.AddNullableRange(new DownloadIdentifiersController().ProcessMovie(chosenShow, fi));
    }

    /// <summary>Asks user about whether to replace a file.</summary>
    /// <returns>false if the newFile is needed.</returns>
    private bool? AskForBetter(FileInfo newFile, FileInfo existingFile, MovieConfiguration chosenShow, IDialogParent owner)
    {
        FileHelper.VideoComparison result = FileHelper.BetterQualityFile(existingFile, newFile);

        switch (result)
        {
            case FileHelper.VideoComparison.secondFileBetter:
                ScanHelper.UpgradeFile(newFile, chosenShow, existingFile, this, TheActionList);
                return false;

            case FileHelper.VideoComparison.cantTell:
            case FileHelper.VideoComparison.similar:
                {
                    return ScanHelper.AskUserAboutFileReplacement(newFile, existingFile, chosenShow, owner, this, TheActionList);
                }
            //the other cases of the files being the same or the existing file being better are not enough to save the file
            case FileHelper.VideoComparison.firstFileBetter:
            case FileHelper.VideoComparison.same:
                return null;

            default:
                throw new NotSupportedException($"result = {result} is not supported by {System.Reflection.MethodBase.GetCurrentMethod()}");
        }
    }

    private bool HasMissingFiles(MovieConfiguration si, DirectoryInfo folder)
    {
        if (!folder.Exists)
        {
            return true;
        }

        FileInfo[] files = folder.GetFiles();
        FileInfo[] movieFiles = files.Where(f => f.IsMovieFile()).ToArray();

        if (movieFiles.Length == 0)
        {
            return true;
        }

        List<string> bases = movieFiles.Select(fi => fi.MovieFileNameBase()).Distinct().ToList();
        string newBase = TVSettings.Instance.FilenameFriendly(si.ProposedFilename);

        if (bases.Count == 1)
        {
            string baseString = bases[0];

            if (baseString.Equals(newBase, StringComparison.CurrentCultureIgnoreCase))
            {
                //All Seems OK
                return false;
            }

            //The bases do not match
            foreach (FileInfo fi in files)
            {
                if (fi.Name.StartsWith(baseString, StringComparison.CurrentCultureIgnoreCase))
                {
                    string newName = fi.Name.Replace(baseString, newBase);
                    FileInfo newFile = FileHelper.FileInFolder(folder, newName); // rename updates the filename

                    if (newFile.IsMovieFile())
                    {
                        //This is the code that will iterate over the DownloadIdentifiers and ask each to ensure that
                        //it has all the required files for that show
                        TheActionList.Add(downloadIdentifiers.ProcessMovie(si, newFile));
                        return false;
                    }

                    if (newFile.FullName != fi.FullName)
                    {
                        //Check that the file does not already exist
                        //if (FileHelper.FileExistsCaseSensitive(newFile.FullName))
                        if (FileHelper.FileExistsCaseSensitive(files, newFile))
                        {
                            Logger.Warn(
                                $"Identified that {fi.FullName} should be renamed to {newName}, but it already exists.");

                            return false;
                        }
                        else
                        {
                            Logger.Info($"Identified that {fi.FullName} should be renamed to {newName}.");
                            TheActionList.Add(new ActionCopyMoveRename(ActionCopyMoveRename.Op.rename, fi,
                                newFile, si, false, null, this));

                            return false;
                        }
                    }
                    else
                    {
                        if (fi.IsMovieFile())
                        {
                            return true;
                        }
                    }
                }
            } // foreach file in folder

            return true;
        }
        else
        {
            Logger.Warn($"{si.ShowName} in {folder.FullName} has multiple bases: {bases.ToCsv()}");
            return false;
        }
    }

    private void BonusAutoAdd(FileInfo fi, IDialogParent owner)
    {
        //do an auto add
        MovieConfiguration? selectedShow = AutoAddMovieFile(fi, owner);

        if (selectedShow != null && ContainsMedia(FilmLibrary, selectedShow))
        {
            //if user selects existing movie then do a compare for that new file for the show
            MergeMovieFileIntoMovieConfig(fi, selectedShow, owner);
        }
        //if new show then add and link file to it
        else if (selectedShow != null && selectedShow.Locations.Any())
        {
            LinkFileToShow(fi, selectedShow, new DirectoryInfo(selectedShow.Locations.First()));
            Add(selectedShow.AsList(), true);
        }
        else
        {
            Logger.Warn($"User elected todo nothing with {fi.FullName}");
        }
    }

    private MovieConfiguration? AutoAddMovieFile(FileInfo file, IDialogParent owner)
    {
        string hint = file.RemoveExtension(TVSettings.Instance.UseFullPathNameToMatchSearchFolders) + ".";

        //If the hint contains certain terms then we'll ignore it
        if (TVSettings.Instance.IgnoredAutoAddHints.Contains(file.RemoveExtension()))
        {
            Logger.Info(
                $"Ignoring {hint} as it is in the list of ignored terms the user has selected to ignore from prior Auto Adds.");

            return null;
        }
        //remove any search folders  from the hint. They are probably useless at helping specify the show's name
        hint = FinderHelper.RemoveDownloadFolders(hint);

        //Remove any (nnnn) in the hint - probably a year
        string refinedHint = hint.RemoveBracketedYear();

        //Remove anything we can from hint to make it cleaner and hence more likely to match
        refinedHint = FinderHelper.RemoveSeriesEpisodeIndicators(refinedHint, TvLibrary.SeasonWords());

        if (string.IsNullOrWhiteSpace(refinedHint))
        {
            Logger.Info($"Ignoring {hint} as it refines to nothing.");
            return null;
        }

        //If there are no LibraryFolders then we cant use the simplified UI
        if (TVSettings.Instance.LibraryFolders.Count + TVSettings.Instance.MovieLibraryFolders.Count == 0)
        {
            MessageBox.Show(
                "Please add some monitor (library) folders under 'Bulk Add TV Shows' to use the 'Auto Add' functionality (Alternatively you can add them or turn it off in settings).",
                "Can't Auto Add Movie", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return null;
        }

        //popup dialog
        AutoAddMedia askForMatch = new(refinedHint, file, true);

        Logger.Info($"Auto Adding New Show/Movie by asking about for '{refinedHint}'");
        owner.ShowChildDialog(askForMatch);
        DialogResult dr = askForMatch.DialogResult;

        switch (dr)
        {
            case DialogResult.OK when askForMatch.MovieConfiguration.Code > 0:
                return askForMatch.MovieConfiguration;

            //If added, add show to collection
            case DialogResult.OK when askForMatch.ShowConfiguration.Code > 0:
                Logger.Warn($"User requested movie {file.FullName} be added as a TV Show - Ignoring for now");
                break;
            case DialogResult.OK:
                Logger.Warn($"User did not select a movie for {file.FullName}");
                break;
            case DialogResult.Abort:
                Logger.Info("Skippng Auto Add Process");
                break;
            case DialogResult.Ignore:
                Logger.Info($"Permenantly Ignoring 'Auto Add' for: {hint}");
                TVSettings.Instance.IgnoredAutoAddHints.Add(file.RemoveExtension());
                break;
            default:
                Logger.Info($"Cancelled Auto adding new show/movie {hint}");
                break;
        }

        askForMatch.Dispose();
        return null;
    }

    public static void Reconnect()
    {
        TheTVDB.LocalCache.Instance.ReConnect(false);
        TMDB.LocalCache.Instance.ReConnect(false);
        TVmaze.LocalCache.Instance.ReConnect(false);
    }

    public void SetScanSettings(ScanSettings settings)
    {
        localFinders = new FindMissingEpisodesLocally(this, settings);
        downloadFinders = new FindMissingEpisodesDownloading(this, settings);
        searchFinders = new FindMissingEpisodesSearch(this, settings);
    }

    public static void SaveCaches()
    {
        Utility.Helper.TaskHelper.Run(
            () =>
            {
                TheTVDB.LocalCache.Instance.SaveCache();
                TVmaze.LocalCache.Instance.SaveCache();
                TMDB.LocalCache.Instance.SaveCache();
            },
            "Save Cache Files",
            false);
    }

    public void DoActions(ActionSettings set)
    {
        if (set.DoAll)
        {
            PreventAutoScan("Do all actions");
            DoActions(TheActionList, set.Token.Token);
        }
        else
        {
            PreventAutoScan($"Do selected actions ({set.Lvr.Count})");
            DoActions(set.Lvr, set.Token.Token);
        }
        AllowAutoScan();
    }

    public void ForceRefreshBeforeRescan(List<ShowConfiguration> shows, List<MovieConfiguration> movies, bool unattended, bool tvrMinimised, UI owner)
    {
        RemoveActionsFromShows(shows);
        RemoveActionsFromMovies(movies);

        ForceRefreshShows(shows, unattended, tvrMinimised, owner, false);
        ForceRefreshMovies(movies, unattended, tvrMinimised, owner, false);
    }

    private void RemoveActionsFromShows(IReadOnlyCollection<ShowConfiguration> shows)
    {
        List<Item> selectedActions = TheActionList.Where(a => shows.Any(s => a.Series == s)).ToList();
        TheActionList.Remove(selectedActions);
    }

    private void RemoveActionsFromMovies(IReadOnlyCollection<MovieConfiguration> movie)
    {
        List<Item> selectedActions = TheActionList.Where(a => movie.Any(s => a.Movie == s)).ToList();
        TheActionList.Remove(selectedActions);
    }

    public void UpdateImagesScan(IReadOnlyCollection<ShowConfiguration> sis)
    {
        TheActionList.Clear();
        foreach (ShowConfiguration si in sis)
        {
            //update images for the showitem
            ForceUpdateImages(si);
        }

        RemoveIgnored();
    }
    public void UpdateMovieImagesScan(IReadOnlyCollection<MovieConfiguration> sis)
    {
        TheActionList.Clear();
        foreach (MovieConfiguration si in sis)
        {
            //update images for the showitem
            ForceUpdateImages(si);
        }

        RemoveIgnored();
    }
}
