//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//


using Humanizer;
using SharpCompress.Writers;
using System.Text.RegularExpressions;
using TVRename.Forms;

// ReSharper disable RedundantDefaultMemberInitializer
// ReSharper disable InconsistentNaming

// Settings for TVRename.  All of this stuff is through Options->Preferences in the app.

namespace TVRename;

public sealed class TVSettings
{
    //We are using the singleton design pattern
    //http://msdn.microsoft.com/en-au/library/ff650316.aspx

    private static volatile TVSettings? instance;
    private static readonly Lock syncRoot = new();

    public static TVSettings Instance
    {
        get
        {
            if (instance is null)
            {
                lock (syncRoot)
                {
                    // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
                    if (instance is null)
                    {
                        instance = new TVSettings();
                    }
                }
            }

            return instance;
        }
    }

    private const string VideoExtensionsStringDEFAULT =
        ".avi;.mpg;.mpeg;.mkv;.mp4;.wmv;.divx;.ogm;.qt;.rm;.m4v;.webm;.vob;.ovg;.ogg;.mov;.m4p;.3gp;.wtv;.ts";

    private const string OtherExtensionsStringDEFAULT = ".srt;.nfo;.txt;.tbn";
    private const string keepTogetherExtensionsStringDEFAULT = ".srt;.nfo;.txt;.tbn";
    private const string subtitleExtensionsStringDEFAULT = ".srt;.sub;.sbv;.idx;.ifo;.smi;.ass;.ssa;.smi;.rt;.aqt;.jss;.cvd;.dks;.ttxt;.mpl;.pjs;.psb;.ssf;.svcd;.usf";

    #region FolderJpgIsType enum

    public enum FolderJpgIsType
    {
        Banner,
        Poster,
        FanArt,
        SeasonPoster
    }

    #endregion FolderJpgIsType enum

    #region WTWDoubleClickAction enum

    public enum WTWDoubleClickAction
    {
        Search,
        Scan
    }

    #endregion WTWDoubleClickAction enum

    #region ScanType enum

    public enum ScanType
    {
        Full,
        Recent,
        Quick,
        SingleShow,
        FastSingleShow,
        Incremental
    }

    #endregion ScanType enum

    #region DuplicateActionOutcome enum

    public enum DuplicateActionOutcome
    {
        IgnoreAll,
        ChooseFirst,
        Ask,
        DoAll,
        MostSeeders,
        Largest
    }

    #endregion DuplicateActionOutcome enum

    public enum BetaMode
    {
        BetaToo,
        ProductionOnly
    }

    public enum KeepTogetherModes
    {
        All,
        AllBut,
        Just
    }

    public enum UpdateCheckMode
    {
        Off,
        Everytime,
        Interval
    }

    public SafeList<string> LibraryFolders;
    public SafeList<string> MovieLibraryFolders;
    public SafeList<string> IgnoreFolders;
    public SafeList<string> DownloadFolders;
    public SafeList<string> IgnoredAutoAddHints;
    public SafeList<IgnoreItem> Ignore;
    public bool AutoSelectShowInMyShows = true;
    public bool AutoCreateFolders = false;
    public bool BGDownload = false;
    public bool RestrictMissingExportsToFullScans = true;
    public bool CheckuTorrent = false;
    public bool CheckqBitTorrent = false;
    public bool RemoveCompletedTorrents = false;
    public string qBitTorrentHost = "localhost";
    public string qBitTorrentPort = "8080";
    public bool qBitTorrentUseHTTPS = false;
    public qBitTorrent.qBitTorrentAPIVersion qBitTorrentAPIVersion = qBitTorrent.qBitTorrentAPIVersion.v2;
    public bool EpThumbnails = false;
    public bool EpJPGs = false;
    public bool SeriesJpg = false;
    public bool ShrinkLargeMede8erImages = false;
    public bool FanArtJpg = false;
    public bool GroupMissingEpisodesIntoSeasons = true;
    public bool Mede8erXML = false;
    public bool ExportFOXML = false;
    public string ExportFOXMLTo = string.Empty;
    public bool ExportMissingCSV = false;
    public string ExportMissingCSVTo = string.Empty;
    public bool ExportMissingXML = false;
    public string ExportMissingXMLTo = string.Empty;
    public bool ExportShowsTXT = false;
    public string ExportShowsTXTTo = string.Empty;
    public bool ExportShowsHTML = false;
    public string ExportShowsHTMLTo = string.Empty;
    public bool ExportMissingMoviesCSV = false;
    public string ExportMissingMoviesCSVTo = string.Empty;
    public bool ExportMissingMoviesXML = false;
    public string ExportMissingMoviesXMLTo = string.Empty;
    public bool ExportMoviesTXT = false;
    public string ExportMoviesTXTTo = string.Empty;
    public bool ExportMoviesHTML = false;
    public string ExportMoviesHTMLTo = string.Empty;
    public int ExportRSSMaxDays = 7;
    public int ExportRSSMaxShows = 10;
    public int ExportRSSDaysPast = 0;
    public int? MinRSSSeeders;
    public bool ExportRenamingXML = false;
    public string ExportRenamingXMLTo = string.Empty;
    public bool ExportWTWRSS = false;
    public string ExportWTWRSSTo = string.Empty;
    public bool ExportWTWXML = false;
    public string ExportWTWXMLTo = string.Empty;
    public bool ExportWTWTXT = false;
    public string ExportWTWTXTTo = string.Empty;
    public bool ExportWTWICAL = false;
    public string ExportWTWICALTo = string.Empty;
    public bool ExportRecentXSPF = false;
    public string ExportRecentXSPFTo = string.Empty;
    public bool ExportRecentM3U = false;
    public string ExportRecentM3UTo = string.Empty;
    public bool ExportRecentASX = false;
    public string ExportRecentASXTo = string.Empty;
    public bool ExportRecentWPL = false;
    public string ExportRecentWPLTo = string.Empty;
    public bool UseColoursOnWtw = false;
    public List<FilenameProcessorRE> FNPRegexs = DefaultFNPList();
    public bool FolderJpg = false;
    public FolderJpgIsType FolderJpgIs = FolderJpgIsType.Poster;
    public ScanType MonitoredFoldersScanType = ScanType.Full;
    public bool ForceLowercaseFilenames = false;
    public bool IgnoreSamples = true;
    public bool KeepTogether = true;
    public bool LeadingZeroOnSeason = false;
    public bool LeaveOriginals = false;
    public bool MissingCheck = true;
    public bool MoveLibraryFiles = true;
    public bool CorrectFileDates = false;
    public bool NFOShows = false;
    public bool NFOEpisodes = false;
    public bool NFOMovies = true;
    public bool KODIImages = false;
    public bool pyTivoMeta = false;
    public bool wdLiveTvMeta = false;
    public bool pyTivoMetaSubFolder = false;
    public CustomEpisodeName NamingStyle = new();
    public bool NotificationAreaIcon = false;
    public bool OfflineMode = false;
    public bool DetailedRSSJSONLogging = false;
    public bool ShowBasicShowDetails = false;
    public bool RSSUseCloudflare = true;
    public bool SearchJSONUseCloudflare = true;
    public bool qBitTorrentDownloadFilesFirst = true;
    public TVDoc.ProviderType DefaultProvider = TVDoc.ProviderType.TMDB;

    public Language TMDBLanguage;
    public Region TMDBRegion;
    public float TMDBPercentDirty;
    public bool IncludeMoviesQuickRecent;

    public BetaMode mode = BetaMode.ProductionOnly;
    public float upgradeDirtyPercent = 20;
    public float replaceMargin = 10;
    public bool ReplaceWithBetterQuality = false;
    public bool ReplaceMoviesWithBetterQuality = false;

    public bool ForceSystemToDecideOnUpgradedFiles = false;
    public KeepTogetherModes keepTogetherMode = KeepTogetherModes.All;

    public bool BulkAddIgnoreRecycleBin = false;
    public bool BulkAddCompareNoVideoFolders = false;
    public ScanType UIScanType = ScanType.Full;

    public string AutoAddMovieTerms = "dvdrip;camrip;screener;dvdscr;r5;bluray";

    public IEnumerable<string> AutoAddMovieTermsArray => Convert(AutoAddMovieTerms);

    internal string FilenameFriendly(ShowConfiguration show, Episode sourceEp)
    {
        // ReSharper disable once ArrangeMethodOrOperatorBody
        return FilenameFriendly(NamingStyle.GetTargetEpisodeName(show, sourceEp));
    }

    public string PriorityReplaceTerms = "PROPER;REPACK;RERIP";

    public IEnumerable<string> PriorityReplaceTermsArray => Convert(PriorityReplaceTerms);

    public string AutoAddIgnoreSuffixes = "1080p;720p;2160p";

    public IEnumerable<string> AutoAddIgnoreSuffixesArray => Convert(AutoAddIgnoreSuffixes);

    public string keepTogetherExtensionsString;

    private IEnumerable<string> KeepTogetherExtensionsArray => Convert(keepTogetherExtensionsString);

    public string subtitleExtensionsString;

    public IEnumerable<string> SubtitleExtensionsArray => Convert(subtitleExtensionsString);

    public string searchSeasonWordsString = "Season;Series;Saison;Temporada;Seizoen";

    public IEnumerable<string> SearchSeasonWordsArray => Convert(searchSeasonWordsString);

    public string preferredRSSSearchTermsString = "720p;1080p;2160p";

    public string unwantedRSSSearchTermsString = "hdcam;tscam;CAMRip;HDCAM;HDTS";

    public string[] PreferredRSSSearchTerms() => Convert(preferredRSSSearchTermsString);
    public string[] UnwantedRSSSearchTerms() => Convert(unwantedRSSSearchTermsString);

    public string OtherExtensionsString;

    private IEnumerable<string> OtherExtensionsArray => Convert(OtherExtensionsString);

    private static string[] Convert(string? propertyString) => propertyString.FromScsv().ToArray();

    internal bool IncludeBetaUpdates() => mode == BetaMode.BetaToo;

    public string defaultSeasonWord = "Season";
    public ShowFilter Filter = new();
    public MovieFilter MovieFilter = new();
    public SeasonFilter SeasonFilter = new();
    public int ParallelDownloads = 4;
    public List<string> RSSURLs = DefaultRSSURLList();
    public bool RenameCheck = true;
    public bool PreventMove = false;
    public bool RenameTxtToSub = false;
    public readonly SafeList<Replacement> Replacements = DefaultListRE();
    public string ResumeDatPath;
    public int SampleFileMaxSizeMB = 50; // sample file must be smaller than this to be ignored
    public bool SearchLocally = true;
    public bool IgnorePreviouslySeen = false;
    public bool IgnorePreviouslySeenMovies = false;
    public bool SearchRSS = false;
    public bool SearchRSSManualScanOnly = true;
    public bool SearchJSON = false;
    public bool SearchJSONManualScanOnly = true;
    public bool ShowEpisodePictures = true;
    public bool HideWtWSpoilers = false;
    public bool HideMyShowsSpoilers = false;
    public bool ShowInTaskbar = true;
    public bool ShowAccessibilityOptions = false;
    public bool AutoSearchForDownloadedFiles = false;
    public string SpecialsFolderName = "Specials";
    public string SeasonFolderFormat = CustomSeasonName.DefaultStyle();
    public string MovieFolderFormat = CustomMovieName.DefaultStyle();
    public string MovieFilenameFormat = CustomMovieName.DefaultStyle();
    public int StartupTab;
    public Searchers TheSearchers = new(MediaConfiguration.MediaType.tv);
    public Searchers TheMovieSearchers = new(MediaConfiguration.MediaType.movie);

    public string SearchJSONURL = "https://eztv.ag/api/get-torrents?imdb_id=";
    public string SearchJSONRootNode = "torrents";
    public string SearchJSONFilenameToken = "filename";
    public string SearchJSONURLToken = "torrent_url";
    public string SearchJSONFileSizeToken = "size_bytes";
    public string SearchJSONSeedersToken = "seeds";

    public string[] VideoExtensionsArray => Convert(VideoExtensionsString);

    public static string USER_AGENT =>
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36";

    public static string SpecialsListViewName => "Special";

    public MovieConfiguration.MovieFolderFormat DefMovieFolderFormat =
        MovieConfiguration.MovieFolderFormat.singleDirectorySingleFile;

    public bool AutomateAutoAddWhenOneMovieFound = true;
    public bool AutomateAutoAddWhenOneShowFound = true;

    public StringComparison FileNameComparisonType => FileNameCaseSensitiveMatch
        ? StringComparison.CurrentCulture
        : StringComparison.CurrentCultureIgnoreCase;

    public bool FileNameCaseSensitiveMatch = false;
    public bool CopySubsFolders = true;

    public bool DeleteMovieFromDisk = false;

    public IEnumerable<string> SubsFolderNames => Convert(SubsFolderNamesString);

    public string qBitTorrentProtocol => qBitTorrentUseHTTPS ? "https" : "http";

    public SystemColorMode ColorMode { get; internal set; } = SystemColorMode.Classic; //TODO LInk to settings
    public SystemColorMode ColorModeActual =>
        ColorMode == SystemColorMode.Dark ? SystemColorMode.Dark :
        ColorMode == SystemColorMode.Classic ? SystemColorMode.Classic :
        Helpers.IsSystemDarkMode() ? SystemColorMode.Dark : SystemColorMode.Classic;

    public bool UseGlobalReleaseDate = false;

    public int FolderMonitorDelaySeconds = 1;

    public string SubsFolderNamesString = "subs;subtitle;vobsubs;sub;vobsub;subtitle";

    public bool AutoSaveOnExit = false;

    public DuplicateActionOutcome UnattendedMultiActionOutcome = DuplicateActionOutcome.IgnoreAll;
    public DuplicateActionOutcome UserMultiActionOutcome = DuplicateActionOutcome.MostSeeders;

    public bool SearchJackett = false;
    public bool UseJackettTextSearch = false;
    public bool SearchJackettManualScanOnly = true;
    public bool StopJackettSearchOnFullScan = true;
    public bool SearchJackettButton = false;
    public string JackettServer = "127.0.0.1";
    public string JackettPort = "9117";
    public string JackettIndexer = "/api/v2.0/indexers/all/results/torznab";
    public string JackettAPIKey = string.Empty;

    public bool ChooseWhenMultipleEpisodesMatch = false;
    public bool AutoAddAsPartOfQuickRename = true;
    public bool UseFullPathNameToMatchSearchFolders = false;
    public bool UseFullPathNameToMatchLibraryFolders = false;

    public bool PostpendThe = false;
    public bool ShareLogs = true;
    public bool CopyFutureDatedEpsFromSearchFolders = false;
    public bool ForceBulkAddToUseSettingsOnly = false;
    public bool RetainLanguageSpecificSubtitles = true;
    public bool AutoMergeDownloadEpisodes = false;
    public bool AutoMergeLibraryEpisodes = false;
    public string VideoExtensionsString;
    public int WTWRecentDays = 7;
    public string uTorrentPath;
    public bool MonitorFolders = false;
    public bool RemoveDownloadDirectoriesFiles = false;
    public bool RemoveDownloadDirectoriesFilesMatchMovies = false;
    public bool RemoveDownloadDirectoriesFilesMatchMoviesLengthCheck = false;
    public int RemoveDownloadDirectoriesFilesMatchMoviesLengthCheckLength = 8;
    public bool DeleteShowFromDisk = true;

    public ShowStatusColoringTypeList ShowStatusColors = [];
    public string SABHostPort = string.Empty;
    public string SABAPIKey = string.Empty;
    public bool CheckSABnzbd = false;
    public Language PreferredTVDBLanguage;
    public readonly TheTVDB.ApiVersion TvdbVersion = TheTVDB.ApiVersion.v4;
    public WTWDoubleClickAction WTWDoubleClick = WTWDoubleClickAction.Scan;

    public readonly TidySettings Tidyup = new();
    public bool runPeriodicCheck = false;
    public int periodCheckHours = 1;
    public int periodUpdateCacheHours = 1;
    public bool runStartupCheck = false;
    public bool DoBulkAddInScan = false;
    public PreviouslySeenEpisodes PreviouslySeenEpisodes;
    public PreviouslySeenMovies PreviouslySeenMovies;
    public bool IgnoreAllSpecials = false;

    public bool DefShowIncludeNoAirdate = false;
    public bool DefShowIncludeFuture = false;
    public bool DefShowNextAirdate = true;
    public bool DefShowDVDOrder = false;
    public bool DefShowAlternateOrder = false;
    public bool DefShowDoRenaming = true;
    public bool DefShowDoMissingCheck = true;
    public bool DefShowSequentialMatching = false;
    public bool DefShowAirDateMatching = false;
    public bool DefShowEpNameMatching = false;
    public bool DefShowSpecialsCount = false;
    public bool DefShowAutoFolders = true;
    public bool DefShowUseDefLocation = false;
    public string? DefShowLocation;
    public string? DefaultShowTimezoneName;
    public bool DefShowUseBase = false;
    public bool DefShowUseSubFolders = true;

    public bool DefMovieDoRenaming = true;
    public bool DefMovieDoMissingCheck = true;
    public bool DefMovieUseAutomaticFolders = true;
    public bool DefMovieUseDefaultLocation = true;
    public bool DefMovieCheckFutureDatedMovies = false;
    public bool DefMovieCheckNoDatedMovies = false;

    public bool SuppressUpdateAvailablePopup = false;
    public UpdateCheckMode UpdateCheckType = UpdateCheckMode.Everytime;
    internal TimeSpan UpdateCheckInterval = 1.Days();

    public string? DefMovieDefaultLocation;
    public TVDoc.ProviderType DefaultMovieProvider = TVDoc.ProviderType.TMDB;
    internal bool UnArchiveFilesInDownloadDirectory = true;
    internal string DefaultTvShowFolderFormat = "{ShowName}"; //TODO put in settings and preferences

    private TVSettings()
    {
        // defaults that aren't handled with default initialisers
        Ignore = [];
        PreviouslySeenEpisodes = [];
        PreviouslySeenMovies = [];
        DownloadFolders = [];
        MovieLibraryFolders = [];
        IgnoreFolders = [];
        LibraryFolders = [];
        MovieLibraryFolders = [];
        IgnoredAutoAddHints = [];

        VideoExtensionsString = VideoExtensionsStringDEFAULT;
        OtherExtensionsString = OtherExtensionsStringDEFAULT;
        keepTogetherExtensionsString = keepTogetherExtensionsStringDEFAULT;
        subtitleExtensionsString = subtitleExtensionsStringDEFAULT;

        PreferredTVDBLanguage = Languages.Instance.FallbackLanguage;
        TMDBLanguage = Languages.Instance.FallbackLanguage;
        TMDBRegion = Regions.Instance.FallbackRegion;

        DefShowLocation = string.Empty;

        // have a guess at utorrent's path
        string[] guesses =
        [
            System.Windows.Forms.Application.StartupPath + "\\..\\uTorrent\\uTorrent.exe",
            "c:\\Program Files\\uTorrent\\uTorrent.exe",
            "c:\\Program Files (x86)\\uTorrent\\uTorrent.exe",
        ];
        uTorrentPath = string.Empty;
        foreach (FileInfo f in guesses.Select(g => new FileInfo(g)).Where(f => f.Exists))
        {
            uTorrentPath = f.FullName;
            break;
        }

        // ResumeDatPath
        FileInfo f2 =
            new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"uTorrent\resume.dat"));

        ResumeDatPath = f2.Exists ? f2.FullName : "";
    }
    internal XElement GenerateXElement()
    {
        return new XElement("Settings",
        new XElement("ExportFOXML", ExportFOXML),
        new XElement("ExportFOXMLTo", ExportFOXMLTo),
        new XElement("FolderMonitorDelaySeconds", FolderMonitorDelaySeconds),
        new XElement("NamingStyle", NamingStyle.StyleString),
        new XElement("NotificationAreaIcon", NotificationAreaIcon),
        new XElement("OtherExtensions", OtherExtensionsString),
        new XElement("StartupTab2", TabNameForNumber(StartupTab)),
        new XElement("SubtitleExtensions", subtitleExtensionsString),
        new XElement("VideoExtensions", VideoExtensionsString),
        new XElement("UseColoursOnWtw", UseColoursOnWtw),
        new XElement("BGDownload", BGDownload),
        new XElement("OfflineMode", OfflineMode),
        new XElement("ShowBasicShowDetails", ShowBasicShowDetails),
        new XElement("DetailedRSSJSONLogging", DetailedRSSJSONLogging),
        new XElement("RSSUseCloudflare", RSSUseCloudflare),
        new XElement("SearchJSONUseCloudflare", SearchJSONUseCloudflare),
        new XElement("qBitTorrentDownloadFilesFirst", qBitTorrentDownloadFilesFirst),
        new XElement("ReplaceWithBetterQuality", ReplaceWithBetterQuality),
        new XElement("ReplaceMoviesWithBetterQuality", ReplaceMoviesWithBetterQuality),
        new XElement("ForceSystemToDecideOnUpgradedFiles", ForceSystemToDecideOnUpgradedFiles),
        new XElement("ExportWTWRSS", ExportWTWRSS),
        new XElement("ExportWTWRSSTo", ExportWTWRSSTo),
        new XElement("ExportWTWICAL", ExportWTWICAL),
        new XElement("ExportWTWICALTo", ExportWTWICALTo),
        new XElement("ExportWTWXML", ExportWTWXML),
        new XElement("ExportWTWXMLTo", ExportWTWXMLTo),
        new XElement("ExportWTWTXT", ExportWTWTXT),
        new XElement("ExportWTWTXTTo", ExportWTWTXTTo),
        new XElement("WTWRecentDays", WTWRecentDays),
        new XElement("ExportMissingXML", ExportMissingXML),
        new XElement("ExportMissingXMLTo", ExportMissingXMLTo),
        new XElement("ExportMissingCSV", ExportMissingCSV),
        new XElement("ExportMissingCSVTo", ExportMissingCSVTo),
        new XElement("ExportRenamingXML", ExportRenamingXML),
        new XElement("ExportRenamingXMLTo", ExportRenamingXMLTo),
        new XElement("ExportRecentM3U", ExportRecentM3U),
        new XElement("ExportRecentM3UTo", ExportRecentM3UTo),
        new XElement("ExportRecentASX", ExportRecentASX),
        new XElement("ExportRecentASXTo", ExportRecentASXTo),
        new XElement("ExportRecentWPL", ExportRecentWPL),
        new XElement("ExportRecentWPLTo", ExportRecentWPLTo),
        new XElement("ExportRecentXSPF", ExportRecentXSPF),
        new XElement("ExportRecentXSPFTo", ExportRecentXSPFTo),
        new XElement("ExportShowsTXT", ExportShowsTXT),
        new XElement("ExportShowsTXTTo", ExportShowsTXTTo),
        new XElement("ExportShowsHTML", ExportShowsHTML),
        new XElement("ExportShowsHTMLTo", ExportShowsHTMLTo),
        new XElement("ExportMissingMoviesCSV", ExportMissingMoviesCSV),
        new XElement("ExportMissingMoviesCSVTo", ExportMissingMoviesCSVTo),
        new XElement("ExportMissingMoviesXML", ExportMissingMoviesXML),
        new XElement("ExportMissingMoviesXMLTo", ExportMissingMoviesXMLTo),
        new XElement("ExportMoviesTXT", ExportMoviesTXT),
        new XElement("ExportMoviesTXTTo", ExportMoviesTXTTo),
        new XElement("ExportMoviesHTML", ExportMoviesHTML),
        new XElement("ExportMoviesHTMLTo", ExportMoviesHTMLTo),
        new XElement("ExportRSSMaxDays", ExportRSSMaxDays),
        new XElement("ExportRSSMaxShows", ExportRSSMaxShows),
        new XElement("ExportRSSDaysPast", ExportRSSDaysPast),
        new XElement("MinRSSSeeders", MinRSSSeeders),
        new XElement("KeepTogether", KeepTogether),
        new XElement("KeepTogetherType", (int)keepTogetherMode),
        new XElement("UIScanType", (int)UIScanType),
        new XElement("KeepTogetherExtensions", keepTogetherExtensionsString),
        new XElement("LeadingZeroOnSeason", LeadingZeroOnSeason),
        new XElement("ShowInTaskbar", ShowInTaskbar),
        new XElement("ShowAccessibilityOptions", ShowAccessibilityOptions),
        new XElement("IgnoreSamples", IgnoreSamples),
        new XElement("ForceLowercaseFilenames", ForceLowercaseFilenames),
        new XElement("RenameTxtToSub", RenameTxtToSub),
        new XElement("ParallelDownloads", ParallelDownloads),
        new XElement("AutoSelectShowInMyShows", AutoSelectShowInMyShows),
        new XElement("AutoCreateFolders", AutoCreateFolders),
        new XElement("ShowEpisodePictures", ShowEpisodePictures),
        new XElement("HideWtWSpoilers", HideWtWSpoilers),
        new XElement("HideMyShowsSpoilers", HideMyShowsSpoilers),
        new XElement("SpecialsFolderName", SpecialsFolderName),
        new XElement("SeasonFolderFormat", SeasonFolderFormat),
        new XElement("DefaultTvShowFolderFormat", DefaultTvShowFolderFormat),
        new XElement("MovieFolderFormat", MovieFolderFormat),
        new XElement("MovieFilenameFormat", MovieFilenameFormat),
        new XElement("uTorrentPath", uTorrentPath),
        new XElement("ResumeDatPath", ResumeDatPath),
        new XElement("SearchRSS", SearchRSS),
        new XElement("SearchJSON", SearchJSON),
        new XElement("SearchJSONManualScanOnly", SearchJSONManualScanOnly),
        new XElement("SearchRSSManualScanOnly", SearchRSSManualScanOnly),
        new XElement("EpImgs", EpThumbnails),
        new XElement("NFOShows", NFOShows),
        new XElement("NFOEpisodes", NFOEpisodes),
        new XElement("NFOMovies", NFOMovies),
        new XElement("KODIImages", KODIImages),
        new XElement("pyTivoMeta", pyTivoMeta),
        new XElement("pyTivoMetaSubFolder", pyTivoMetaSubFolder),
        new XElement("wdLiveTvMeta", wdLiveTvMeta),
        new XElement("FolderJpg", FolderJpg),
        new XElement("FolderJpgIs", (int)FolderJpgIs),
        new XElement("MonitoredFoldersScanType", (int)MonitoredFoldersScanType),
        new XElement("DefaultProvider", (int)DefaultProvider),
        new XElement("CheckuTorrent", CheckuTorrent),
        new XElement("RestrictMissingExportsToFullScans", RestrictMissingExportsToFullScans),
        new XElement("RemoveCompletedTorrents", RemoveCompletedTorrents),
        new XElement("CheckqBitTorrent", CheckqBitTorrent),
        new XElement("qBitTorrentHost", qBitTorrentHost),
        new XElement("qBitTorrentPort", qBitTorrentPort),
        new XElement("qBitTorrentUseHTTPS", qBitTorrentUseHTTPS),
        new XElement("qBitTorrentAPIVersion", (int)qBitTorrentAPIVersion),
        new XElement("RenameCheck", RenameCheck),
        new XElement("PreventMove", PreventMove),
        new XElement("MissingCheck", MissingCheck),
        new XElement("MoveLibraryFiles", MoveLibraryFiles),
        new XElement("AutoSearchForDownloadedFiles", AutoSearchForDownloadedFiles),
        new XElement("UpdateFileDates", CorrectFileDates),
        new XElement("SearchLocally", SearchLocally),
        new XElement("IgnorePreviouslySeen", IgnorePreviouslySeen),
        new XElement("IgnorePreviouslySeenMovies", IgnorePreviouslySeenMovies),
        new XElement("LeaveOriginals", LeaveOriginals),
        new XElement("RetainLanguageSpecificSubtitles", RetainLanguageSpecificSubtitles),
        new XElement("ForceBulkAddToUseSettingsOnly", ForceBulkAddToUseSettingsOnly),
        new XElement("AutoMergeEpisodes", AutoMergeDownloadEpisodes),
        new XElement("AutoMergeLibraryEpisodes", AutoMergeLibraryEpisodes),
        new XElement("MonitorFolders", MonitorFolders),
        new XElement("StartupScan", runStartupCheck),
        new XElement("PeriodicScan", runPeriodicCheck),
        new XElement("PeriodicScanHours", periodCheckHours),
        new XElement("PeriodicUpdateCacheHours", periodUpdateCacheHours),
        new XElement("RemoveDownloadDirectoriesFiles", RemoveDownloadDirectoriesFiles),
        new XElement("RemoveDownloadDirectoriesFilesMatchMovies", RemoveDownloadDirectoriesFilesMatchMovies),
        new XElement("RemoveDownloadDirectoriesFilesMatchMoviesLengthCheck", RemoveDownloadDirectoriesFilesMatchMoviesLengthCheck),
        new XElement("RemoveDownloadDirectoriesFilesMatchMoviesLengthCheckLength", RemoveDownloadDirectoriesFilesMatchMoviesLengthCheckLength),
        new XElement("DoBulkAddInScan", DoBulkAddInScan),
        new XElement("DeleteShowFromDisk", DeleteShowFromDisk),
        new XElement("DeleteMovieFromDisk", DeleteMovieFromDisk),
        new XElement("UseGlobalReleaseDate", UseGlobalReleaseDate),
        new XElement("FileNameCaseSensitiveMatch", FileNameCaseSensitiveMatch),
        new XElement("CopySubsFolders", CopySubsFolders),
        new XElement("SABAPIKey", SABAPIKey),
        new XElement("CheckSABnzbd", CheckSABnzbd),
        new XElement("SABHostPort", SABHostPort),
        new XElement("PreferredLanguage", PreferredTVDBLanguage.Abbreviation),
        new XElement("WTWDoubleClick", (int)WTWDoubleClick),
        new XElement("EpJPGs", EpJPGs),
        new XElement("SeriesJpg", SeriesJpg),
        new XElement("GroupMissingEpisodesIntoSeasons", GroupMissingEpisodesIntoSeasons),
        new XElement("Mede8erXML", Mede8erXML),
        new XElement("ShrinkLargeMede8erImages", ShrinkLargeMede8erImages),
        new XElement("FanArtJpg", FanArtJpg),
        new XElement("DeleteEmpty", Tidyup.DeleteEmpty),
        new XElement("DeleteEmptyIsRecycle", Tidyup.DeleteEmptyIsRecycle),
        new XElement("EmptyIgnoreWords", Tidyup.EmptyIgnoreWords),
        new XElement("EmptyIgnoreWordList", Tidyup.EmptyIgnoreWordList),
        new XElement("EmptyIgnoreExtensions", Tidyup.EmptyIgnoreExtensions),
        new XElement("EmptyIgnoreExtensionList", Tidyup.EmptyIgnoreExtensionList),
        new XElement("EmptyMaxSizeCheck", Tidyup.EmptyMaxSizeCheck),
        new XElement("EmptyMaxSizeMB", Tidyup.EmptyMaxSizeMB),
        new XElement("BetaMode", (int)mode),
        new XElement("PercentDirtyUpgrade", upgradeDirtyPercent),
        new XElement("PercentBetter", replaceMargin),
        new XElement("BaseSeasonName", defaultSeasonWord),
        new XElement("SearchSeasonNames", searchSeasonWordsString),
        new XElement("PreferredRSSSearchTerms", preferredRSSSearchTermsString),
        new XElement("UnwantedRSSSearchTerms", unwantedRSSSearchTermsString),
        new XElement("BulkAddIgnoreRecycleBin", BulkAddIgnoreRecycleBin),
        new XElement("BulkAddCompareNoVideoFolders", BulkAddCompareNoVideoFolders),
        new XElement("AutoAddMovieTerms", AutoAddMovieTerms),
        new XElement("AutoAddIgnoreSuffixes", AutoAddIgnoreSuffixes),
        new XElement("SearchJSONURL", SearchJSONURL),
        new XElement("SearchJSONRootNode", SearchJSONRootNode),
        new XElement("SearchJSONFilenameToken", SearchJSONFilenameToken),
        new XElement("SearchJSONURLToken", SearchJSONURLToken),
        new XElement("SearchJSONFileSizeToken", SearchJSONFileSizeToken),
        new XElement("SearchJSONSeedersToken", SearchJSONSeedersToken),
        new XElement("PriorityReplaceTerms", PriorityReplaceTerms),
        new XElement("CopyFutureDatedEpsFromSearchFolders", CopyFutureDatedEpsFromSearchFolders),
        new XElement("ShareLogs", ShareLogs),
        new XElement("PostpendThe", PostpendThe),
        new XElement("IgnoreAllSpecials", IgnoreAllSpecials),
        new XElement("UseFullPathNameToMatchLibraryFolders", UseFullPathNameToMatchLibraryFolders),
        new XElement("UseFullPathNameToMatchSearchFolders", UseFullPathNameToMatchSearchFolders),
        new XElement("AutoAddAsPartOfQuickRename", AutoAddAsPartOfQuickRename),
        new XElement("AutomateAutoAddWhenOneShowFound", AutomateAutoAddWhenOneShowFound),
        new XElement("AutomateAutoAddWhenOneMovieFound", AutomateAutoAddWhenOneMovieFound),
        new XElement("SubsFolderNamesString", SubsFolderNamesString),

        new XElement("ChooseWhenMultipleEpisodesMatch", ChooseWhenMultipleEpisodesMatch),

        new XElement("DefShowIncludeNoAirdate", DefShowIncludeNoAirdate),
        new XElement("DefShowIncludeFuture", DefShowIncludeFuture),
        new XElement("DefShowNextAirdate", DefShowNextAirdate),
        new XElement("DefShowDVDOrder", DefShowDVDOrder),
        new XElement("DefShowAlternateOrder", DefShowAlternateOrder),
        new XElement("DefShowDoRenaming", DefShowDoRenaming),
        new XElement("DefShowDoMissingCheck", DefShowDoMissingCheck),
        new XElement("DefShowSequentialMatching", DefShowSequentialMatching),
        new XElement("DefShowAirDateMatching", DefShowAirDateMatching),
        new XElement("DefShowEpNameMatching", DefShowEpNameMatching),
        new XElement("DefShowSpecialsCount", DefShowSpecialsCount),
        new XElement("DefShowAutoFolders", DefShowAutoFolders),
        new XElement("DefShowUseDefLocation", DefShowUseDefLocation),
        new XElement("DefShowLocation", DefShowLocation),
        new XElement("DefaultShowTimezoneName", DefaultShowTimezoneName),
        new XElement("DefShowUseBase", DefShowUseBase),
        new XElement("DefShowUseSubFolders", DefShowUseSubFolders),
        new XElement("SampleFileMaxSizeMB", SampleFileMaxSizeMB),

        new XElement("DefMovieDoRenaming", DefMovieDoRenaming),
        new XElement("DefMovieDoMissingCheck", DefMovieDoMissingCheck),
        new XElement("DefMovieUseutomaticFolders", DefMovieUseAutomaticFolders),
        new XElement("DefMovieUseDefaultLocation", DefMovieUseDefaultLocation),
        new XElement("DefMovieDefaultLocation", DefMovieDefaultLocation),
        new XElement("DefaultMovieProvider", (int)DefaultMovieProvider),
        new XElement("DefMovieCheckFutureDatedMovies", DefMovieCheckFutureDatedMovies),
        new XElement("DefMovieCheckNoDatedMovies", DefMovieCheckNoDatedMovies),

        new XElement("UnattendedMultiActionOutcome", (int)UnattendedMultiActionOutcome),
        new XElement("UserMultiActionOutcome", (int)UserMultiActionOutcome),
        new XElement("DefMovieFolderFormat", (int)DefMovieFolderFormat),
        new XElement("UnArchiveFilesInDownloadDirectory", UnArchiveFilesInDownloadDirectory),

        new XElement("SearchJackett", SearchJackett),
        new XElement("UseJackettTextSearch", UseJackettTextSearch),
        new XElement("SearchJackettManualScanOnly", SearchJackettManualScanOnly),
        new XElement("JackettServer", JackettServer),
        new XElement("JackettPort", JackettPort),
        new XElement("JackettIndexer", JackettIndexer),
        new XElement("JackettAPIKey", JackettAPIKey),
        new XElement("SearchJackettButton", SearchJackettButton),
        new XElement("StopJackettSearchOnFullScan", StopJackettSearchOnFullScan),
        new XElement("StopJackettSearchOnFullScan", StopJackettSearchOnFullScan),
        new XElement("AutoSaveOnExit", AutoSaveOnExit),

        new XElement("TMDBLanguage", TMDBLanguage.ISODialectAbbreviation),
        new XElement("TMDBRegion", TMDBRegion.Abbreviation),
        new XElement("TMDBPercentDirty", TMDBPercentDirty),
        new XElement("IncludeMoviesQuickRecent", IncludeMoviesQuickRecent),
        GenerateAppUpdateXElement(),
        TheSearchers.GenerateXElement("TheSearchers"),
        TheMovieSearchers.GenerateXElement("TheMovieSearchers"),
        GenerateReplacementsXElement(),
        GenerateRegExsXElement(),
        XmlHelper.GenerateStringsXMLElement(RSSURLs, "RSSURLs", "URL"),
        GenerateShowStatusColoursXElement(),
        GenerateFiltersXElement()
        );
    }

    private XElement GenerateAppUpdateXElement()
    {
        return new XElement("AppUpdate",
            new XElement("Mode", (int)UpdateCheckType),
            new XElement("Interval", UpdateCheckInterval.ToString()),
            new XElement("SuppressPopup", SuppressUpdateAvailablePopup)
            );
    }

    private XElement GenerateReplacementsXElement()
    {
        var replacementsElement = new XElement("Replacements");
        foreach (Replacement R in Replacements)
        {
            replacementsElement.Add(new XElement("Replace",
                new XAttribute("This", R.This),
                new XAttribute("That", R.That),
                new XAttribute("CaseInsensitive", R.CaseInsensitive ? "Y" : "N")
            ));
        }
        return replacementsElement;
    }

    private XElement GenerateRegExsXElement()
    {
        var regExsElement = new XElement("FNPRegexs");
        foreach (FilenameProcessorRE re in FNPRegexs)
        {
            regExsElement.Add(new XElement("Regex",
                new XAttribute("Enabled", re.Enabled),
                new XAttribute("RE", re.RegExpression),
                new XAttribute("UseFullPath", re.UseFullPath),
                new XAttribute("Notes", re.Notes)
            ));
        }
        return regExsElement;
    }

    private XElement GenerateShowStatusColoursXElement()
    {
        return new XElement("ShowStatusTVWColors",
            from KeyValuePair<ColouringRule, System.Drawing.Color> e in ShowStatusColors
            select new XElement("ShowStatusTVWColor",
                new XAttribute("Type", e.Key switch
                {
                    ShowStatusColouringRule => "ShowStatusColouringRule",
                    ShowAirStatusColouringRule => "ShowAirStatusColouringRule",
                    SeasonStatusColouringRule => "SeasonStatusColouringRule",
                    _ => throw new NotImplementedException()
                }),
                new XAttribute("ShowStatus", e.Key switch
                {
                    ShowStatusColouringRule sscr => sscr.status,
                    ShowAirStatusColouringRule sascr => (int)sascr.status,
                    SeasonStatusColouringRule ssascr => (int)ssascr.status,
                    _ => throw new NotImplementedException()
                }),
                new XAttribute("Color", e.Value.TranslateColorToHtml())
            )
        );
    }

    private List<XElement> GenerateFiltersXElement()
    {
        return [
            new XElement("ShowFilters",
                XmlHelper.GenerateInfoXElement("NameFilter", "Name", Filter.ShowName),
                XmlHelper.GenerateInfoXElement("ShowStatusFilter", "ShowStatus", Filter.ShowStatus),
                XmlHelper.GenerateInfoXElement("ShowNetworkFilter", "ShowNetwork", Filter.ShowNetwork),
                XmlHelper.GenerateInfoXElement("ShowRatingFilter", "ShowRating", Filter.ShowRating),
                XmlHelper.GenerateInfoXElement("ShowStatusFilter", "ShowStatusInclude", Filter.ShowStatusInclude),
                XmlHelper.GenerateInfoXElement("ShowNetworkFilter", "ShowNetworkInclude", Filter.ShowNetworkInclude),
                XmlHelper.GenerateInfoXElement("ShowRatingFilter", "ShowRatingInclude", Filter.ShowRatingInclude),
                XmlHelper.GenerateInfoXElement("IncludeBlankFields", "IncludeBlankFields", Filter.IncludeBlankFields),
                from string genre in Filter.Genres
                select XmlHelper.GenerateInfoXElement("GenreFilter", "Genre", genre)
            ),
            new XElement("MovieFilters",
                XmlHelper.GenerateInfoXElement("NameFilter", "Name", MovieFilter.ShowName),
                XmlHelper.GenerateInfoXElement("ShowStatusFilter", "ShowStatus", MovieFilter.ShowStatus),
                XmlHelper.GenerateInfoXElement("ShowNetworkFilter", "ShowNetwork", MovieFilter.ShowNetwork),
                XmlHelper.GenerateInfoXElement("ShowRatingFilter", "ShowRating", MovieFilter.ShowRating),
                XmlHelper.GenerateInfoXElement("ShowYearFilter", "ShowYear", MovieFilter.ShowYear),
                XmlHelper.GenerateInfoXElement("ShowStatusFilter", "ShowStatusInclude", MovieFilter.ShowStatusInclude),
                XmlHelper.GenerateInfoXElement("ShowNetworkFilter", "ShowNetworkInclude", MovieFilter.ShowNetworkInclude),
                XmlHelper.GenerateInfoXElement("ShowRatingFilter", "ShowRatingInclude", MovieFilter.ShowRatingInclude),
                XmlHelper.GenerateInfoXElement("ShowYearFilter", "ShowYearInclude", MovieFilter.ShowYearInclude),
                XmlHelper.GenerateInfoXElement("IncludeBlankFields", "IncludeBlankFields", MovieFilter.IncludeBlankFields),
                from string genre in MovieFilter.Genres
                select XmlHelper.GenerateInfoXElement("GenreFilter", "Genre", genre)
            ),
            new XElement("SeasonFilters",
                new XElement("SeasonIgnoredFilter", SeasonFilter.HideIgnoredSeasons)
            )
            ];
    }

    internal float PercentDirtyUpgrade() => upgradeDirtyPercent;

    public FolderJpgIsType ItemForFolderJpg() => FolderJpgIs;

    public string GetVideoExtensionsString() => VideoExtensionsString;

    public string GetOtherExtensionsString() => OtherExtensionsString;

    public string GetKeepTogetherString() => keepTogetherExtensionsString;

    public bool RunPeriodicCheck() => runPeriodicCheck;

    public int PeriodicCheckPeriod() => (int)periodCheckHours.Hours().TotalMilliseconds;

    public int PeriodicUpdateCachePeriod() => (int)periodUpdateCacheHours.Hours().TotalMilliseconds;

    public bool RunOnStartUp() => runStartupCheck;

    public string GetSeasonSearchTermsString() => searchSeasonWordsString;

    public string GetPreferredRSSSearchTermsString() => preferredRSSSearchTermsString;
    public string GetUnwantedRSSSearchTermsString() => unwantedRSSSearchTermsString;

    public static bool OKExtensionsString(string? s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return true;
        }

        var t = s.FromScsv();
        foreach (string s2 in t)
        {
            if (string.IsNullOrEmpty(s2) || !s2.StartsWith('.') || s2.ContainsAnyCharactersFrom(CompulsoryReplacements()) || s2.ContainsAnyCharactersFrom(Path.GetInvalidFileNameChars()))
            {
                return false;
            }
        }

        return true;
    }

    public static bool OKExtensionsStringNoDotCheck(string? s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return true;
        }

        var t = s.FromScsv();
        foreach (string s2 in t)
        {
            if (string.IsNullOrEmpty(s2) || s2.ContainsAnyCharactersFrom(CompulsoryReplacements()) || s2.ContainsAnyCharactersFrom(Path.GetInvalidFileNameChars()))
            {
                return false;
            }
        }

        return true;
    }

    public static bool OKExporterLocation(string? s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return false;
        }

        if (s.IsWebLink())
        {
            return false;
        }

        return s.IsValidDirectory();
    }

    public static bool OKPath(string? s, bool EmptyOK)
    {
        if (string.IsNullOrEmpty(s))
        {
            return EmptyOK;
        }

        if (s.IsWebLink())
        {
            return false;
        }

        return s.IsValidDirectory();
    }

    public static string CompulsoryReplacements() => "*?<>:/\\|\""; // invalid filename characters, must be in the list!

    public static List<FilenameProcessorRE> DefaultFNPList()
    {
        // Default list of filename processors

        List<FilenameProcessorRE> l =
        [
            new FilenameProcessorRE(true,
                "(^|[^a-z])s?(?<s>[0-9]+).?[ex](?<e>[0-9]{2,})(-?e[0-9]{2,})*-?[ex](?<f>[0-9]{2,})[^a-z]",
                false, "Multipart Rule : s04e01e02e03 S01E01-E02"),
            new FilenameProcessorRE(true,
                "(^|[^a-z])s?(?<s>[0-9]+)[ex](?<e>[0-9]{2,})(e[0-9]{2,})*[^a-z]",
                false, "3x23 s3x23 3e23 s3e23 s04e01e02e03"),
            new FilenameProcessorRE(false,
                "(^|[^a-z])s?(?<s>[0-9]+)(?<e>[0-9]{2,})[^a-z]",
                false,
                "323 or s323 for season 3, episode 23. 2004 for season 20, episode 4."),
            new FilenameProcessorRE(false,
                "(^|[^a-z])s(?<s>[0-9]+)--e(?<e>[0-9]{2,})[^a-z]",
                false, "S02--E03"),
            new FilenameProcessorRE(false,
                "(^|[^a-z])s(?<s>[0-9]+) e(?<e>[0-9]{2,})[^a-z]",
                false, "'S02.E04' and 'S02 E04'"),
            new FilenameProcessorRE(false, "^(?<s>[0-9]+) (?<e>[0-9]{2,})", false,
                "filenames starting with '1.23' for season 1, episode 23"),
            new FilenameProcessorRE(false,
                "(^|[^a-z])(?<s>[0-9])(?<e>[0-9]{2,})[^a-z]",
                false, "Show - 323 - Foo"),
            new FilenameProcessorRE(true,
                "(^|[^a-z])se(?<s>[0-9]+)([ex]|ep|xep)?(?<e>[0-9]+)[^a-z]",
                false, "se3e23 se323 se1ep1 se01xep01..."),
            new FilenameProcessorRE(true,
                "(^|[^a-z])(?<s>[0-9]+)-(?<e>[0-9]{2,})[^a-z]",
                false, "3-23 EpName"),
            new FilenameProcessorRE(true,
                "(^|[^a-z])s(?<s>[0-9]+) +- +e(?<e>[0-9]{2,})[^a-z]",
                false, "ShowName - S01 - E01"),
            new FilenameProcessorRE(true,
                "\\b(?<e>[0-9]{2,}) ?- ?.* ?- ?(?<s>[0-9]+)",
                false,
                "like '13 - Showname - 2 - Episode Title.avi'"),
            new FilenameProcessorRE(true,
                "\\b(episode|ep|e) ?(?<e>[0-9]{2,}) ?- ?(series|season) ?(?<s>[0-9]+)",
                false, "episode 3 - season 4"),
            new FilenameProcessorRE(true,
                "season (?<s>[0-9]+)\\\\e?(?<e>[0-9]{1,3}) ?-",
                true, "Show Season 3\\E23 - Epname"),
            new FilenameProcessorRE(false,
                "season (?<s>[0-9]+)\\\\episode (?<e>[0-9]{1,3})",
                true, "Season 3\\Episode 23")
        ];

        return l;
    }

    private static SafeList<Replacement> DefaultListRE() =>
        [
            new Replacement("*", "#", false),
            new Replacement("?", "", false),
            new Replacement(">", "", false),
            new Replacement("<", "", false),
            new Replacement(":", "-", false),
            new Replacement("/", "-", false),
            new Replacement("\\", "-", false),
            new Replacement("|", "-", false),
            new Replacement("\"", "'", false)
        ];

    private static List<string> DefaultRSSURLList()
    {
        List<string> sl = [];
        return sl;
    }

    private static string[] TabNames() => ["MyMovies", "MyShows", "Scan", "WTW"];

    private static string TabNameForNumber(int n)
    {
        if (n >= 0 && n < TabNames().Length)
        {
            return TabNames()[n];
        }

        return string.Empty;
    }

    private static int TabNumberFromName(string? n)
    {
        if (string.IsNullOrEmpty(n))
        {
            return 0;
        }

        int r = Array.IndexOf(TabNames(), n);
        return r < 0 ? 0 : r;
    }

    public bool FileHasUsefulExtension(FileInfo file, bool otherExtensionsToo) =>
        FileHasUsefulExtension(file.Name, otherExtensionsToo);

    public bool FileHasUsefulExtension(string filename, bool otherExtensionsToo)
    {
        if (VideoExtensionsArray
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Any(s => filename.EndsWith(s, StringComparison.InvariantCultureIgnoreCase)))
        {
            return true;
        }

        return otherExtensionsToo && OtherExtensionsArray
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Any(s => filename.EndsWith(s, StringComparison.InvariantCultureIgnoreCase));
    }

    public string FileHasUsefulExtensionDetails(FileInfo file, bool otherExtensionsToo)
    {
        foreach (string s in VideoExtensionsArray
                     .Where(s => !string.IsNullOrWhiteSpace(s))
                     .Where(s => file.Name.EndsWith(s, StringComparison.InvariantCultureIgnoreCase))
                     .OrderByDescending(s => s.Length))
        {
            return s;
        }

        if (otherExtensionsToo)
        {
            foreach (string s in OtherExtensionsArray
                         .Where(s => !string.IsNullOrWhiteSpace(s))
                         .Where(s => file.Name.EndsWith(s, StringComparison.InvariantCultureIgnoreCase))
                         .OrderByDescending(s => s.Length))
            {
                return s;
            }
        }

        return string.Empty;
    }

    public string BTSearchURL(ProcessedEpisode? epi)
    {
        CachedSeriesInfo? s = epi?.TheCachedSeries;
        if (s is null)
        {
            return string.Empty;
        }

        string url = epi!.Show.UseCustomSearchUrl && epi.Show.CustomSearchUrl.HasValue()
            ? epi.Show.CustomSearchUrl
            : TheSearchers.CurrentSearch.Url;

        return url.HasValue() ? CustomEpisodeName.NameForNoExt(epi, url, true,true) : string.Empty;
    }

    public string BTMovieSearchURL(MovieConfiguration? mov)
    {
        if (mov is null)
        {
            return string.Empty;
        }

        string url = TheMovieSearchers.CurrentSearch.Url;

        return !url.HasValue() ? string.Empty : CustomMovieName.NameFor(mov, url, true, false);
    }

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public string FilenameFriendly(string fn)
    {
        if (string.IsNullOrWhiteSpace(fn))
        {
            return string.Empty;
        }

        foreach (Replacement rep in Replacements)
        {
            fn = rep.DoReplace(fn);
        }

        if (fn.ContainsAnyCharactersFrom(Path.GetInvalidFileNameChars()))
        {
            Logger.Warn($"Need to remove some characters from {fn} as the episode name contains characters that cannot be in the filename.");
            fn = fn.RemoveCharactersFrom(Path.GetInvalidFileNameChars()).RemoveCharactersFrom(Helpers.Tab.ToCharArray());
        }

        return ForceLowercaseFilenames ? fn.ToLower() : fn;
    }

    public static string DirectoryFriendly(string fn)
    {
        if (string.IsNullOrWhiteSpace(fn))
        {
            return string.Empty;
        }

        foreach (Replacement rep in DefaultListRE())
        {
            fn = rep.DoReplace(fn);
        }

        if (fn.ContainsAnyCharactersFrom(Path.GetInvalidPathChars()))
        {
            Logger.Warn($"Need to remove some characters from {fn} as the directory name contains characters that cannot be in the path.");
            fn = fn.RemoveCharactersFrom(Path.GetInvalidPathChars()).RemoveCharactersFrom(Helpers.Tab.ToCharArray());
        }

        return fn;
    }

    public bool NeedToDownloadBannerFile()
    {
        // Return true iff we need to download season specific images
        // There are 4 possible reasons
        return SeasonSpecificFolderJPG() || KODIImages || SeriesJpg || FanArtJpg || !ShowBasicShowDetails;
    }

    // ReSharper disable once InconsistentNaming
    public bool SeasonSpecificFolderJPG()
    {
        return FolderJpgIsType.SeasonPoster == FolderJpgIs;
    }

    public bool KeepTogetherFilesWithType(string fileExtension)
    {
        if (KeepTogether == false)
        {
            return false;
        }

        return keepTogetherMode switch
        {
            KeepTogetherModes.All => true,
            KeepTogetherModes.Just => KeepTogetherExtensionsArray.Contains(fileExtension),
            KeepTogetherModes.AllBut => !KeepTogetherExtensionsArray.Contains(fileExtension),
            _ => throw new NotSupportedException($"keepTogetherMode = {keepTogetherMode} is not supported by {System.Reflection.MethodBase.GetCurrentMethod()}")
        };
    }

    public class TidySettings
    {
        public bool DeleteEmpty = false; // Delete empty folders after move
        public bool DeleteEmptyIsRecycle = true; // Recycle, rather than delete
        public bool EmptyIgnoreWords = false;
        public string EmptyIgnoreWordList = "sample";
        public bool EmptyIgnoreExtensions = false;
        public string EmptyIgnoreExtensionList = ".nzb;.nfo;.par2;.txt;.srt";
        public bool EmptyMaxSizeCheck = true;

        // ReSharper disable once InconsistentNaming
        public int EmptyMaxSizeMB = 100;

        public IEnumerable<string> EmptyIgnoreExtensionsArray => Convert(EmptyIgnoreExtensionList);

        public IEnumerable<string> EmptyIgnoreWordsArray => Convert(EmptyIgnoreWordList);

        public void Load(XElement xmlSettings)
        {
            DeleteEmpty = xmlSettings.ExtractBool("DeleteEmpty", false);
            DeleteEmptyIsRecycle = xmlSettings.ExtractBool("DeleteEmptyIsRecycle", true);
            EmptyIgnoreWords = xmlSettings.ExtractBool("EmptyIgnoreWords", false);
            EmptyIgnoreWordList = xmlSettings.ExtractString("EmptyIgnoreWordList", "sample");
            EmptyIgnoreExtensions = xmlSettings.ExtractBool("EmptyIgnoreExtensions", false);
            EmptyIgnoreExtensionList = xmlSettings.ExtractString("EmptyIgnoreExtensionList", ".nzb;.nfo;.par2;.txt;.srt");
            EmptyMaxSizeCheck = xmlSettings.ExtractBool("EmptyMaxSizeCheck", true);
            EmptyMaxSizeMB = xmlSettings.ExtractInt("EmptyMaxSizeMB", 100);
        }
    }

    public class Replacement(string from, string? to, bool insens)
    {
        // used for invalid (and general) character (and string) replacements in file names

        public readonly bool CaseInsensitive = insens;
        public readonly string That = to ?? string.Empty;
        public readonly string This = from;

        public string DoReplace(string fn)
        {
            try
            {
                return CaseInsensitive ? Regex.Replace(fn, Regex.Escape(This), Regex.Escape(That), RegexOptions.IgnoreCase) : fn.Replace(This, That);
            }
            catch (RegexMatchTimeoutException)
            {
                return fn.Replace(This, That);
            }
        }
    }

    public class FilenameProcessorRE(bool enabled, string re, bool useFullPath, string notes)
    {
        // A regular expression to find the season and episode number in a filename

        public readonly bool Enabled = enabled;
        public readonly string Notes = notes;
        public readonly string RegExpression = re;
        public readonly bool UseFullPath = useFullPath;
    }

    [Serializable]
    public class ShowStatusColoringTypeList : Dictionary<ColouringRule, System.Drawing.Color>
    {
        public ShowStatusColoringTypeList()
        {
        }

        public bool AppliesTo(ShowConfiguration show) => Keys.Any(rule => rule.AppliesTo(show));

        public System.Drawing.Color GetColour(ShowConfiguration show)
        {
            foreach (KeyValuePair<ColouringRule, System.Drawing.Color> e in this)
            {
                if (e.Key.AppliesTo(show))
                {
                    return e.Value;
                }
            }

            return System.Drawing.Color.Empty;
        }

        public System.Drawing.Color GetColour(ProcessedSeason s)
        {
            foreach (KeyValuePair<ColouringRule, System.Drawing.Color> e in this)
            {
                if (e.Key.AppliesTo(s))
                {
                    return e.Value;
                }
            }

            return System.Drawing.Color.Empty;
        }

        public bool AppliesTo(ProcessedSeason s) => Keys.Any(rule => rule.AppliesTo(s));
    }

    public abstract class ColouringRule
    {
        // Text is a read-only property - only a get accessor is needed:
        public abstract string Text
        {
            get;
        }

        public abstract bool AppliesTo(ProcessedSeason s);

        public abstract bool AppliesTo(ShowConfiguration s);
    }

    public class ShowStatusColouringRule(string status) : ColouringRule
    {
        public readonly string status = status;
        public override string Text => "Show Status: " + status;

        public override bool AppliesTo(ProcessedSeason s) => false;

        public override bool AppliesTo(ShowConfiguration s) => status == s.ShowStatus;
    }

    public class ShowAirStatusColouringRule(ShowConfiguration.ShowAirStatus status) : ColouringRule
    {
        public readonly ShowConfiguration.ShowAirStatus status = status;

        public override string ToString()
        {
            return status switch
            {
                ShowConfiguration.ShowAirStatus.aired => "Show Season Status: All aired",
                ShowConfiguration.ShowAirStatus.noEpisodesOrSeasons =>
                    "Show Season Status: No Seasons or Episodes in Seasons",
                ShowConfiguration.ShowAirStatus.noneAired => "Show Season Status: None aired",
                ShowConfiguration.ShowAirStatus.partiallyAired => "Show Season Status: Partially aired",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override string Text => ToString();

        public override bool AppliesTo(ProcessedSeason s) => false;

        public override bool AppliesTo(ShowConfiguration s) => status == s.SeasonsAirStatus;
    }

    public class SeasonStatusColouringRule(ProcessedSeason.SeasonStatus status) : ColouringRule
    {
        public readonly ProcessedSeason.SeasonStatus status = status;

        public override string Text => ToString();

        public override string ToString()
        {
            return status switch
            {
                ProcessedSeason.SeasonStatus.aired => "Season Status: All aired",
                ProcessedSeason.SeasonStatus.noEpisodes => "Season Status: No Episodes",
                ProcessedSeason.SeasonStatus.noneAired => "Season Status: None aired",
                ProcessedSeason.SeasonStatus.partiallyAired => "Season Status: Partially aired",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override bool AppliesTo(ProcessedSeason s) => status == s.Status(s.Show.GetTimeZone());

        public override bool AppliesTo(ShowConfiguration s) => false;
    }

    // ReSharper disable once FunctionComplexityOverflow
    public void Load(XElement xmlSettings)
    {
        FolderMonitorDelaySeconds = xmlSettings.ExtractInt("FolderMonitorDelaySeconds", 1);
        UseColoursOnWtw = xmlSettings.ExtractBool("UseColoursOnWtw", false);
        RSSUseCloudflare = xmlSettings.ExtractBool("RSSUseCloudflare", true);
        SearchJSONUseCloudflare = xmlSettings.ExtractBool("SearchJSONUseCloudflare", true);
        qBitTorrentDownloadFilesFirst = xmlSettings.ExtractBool("qBitTorrentDownloadFilesFirst", true);
        BGDownload = xmlSettings.ExtractBool("BGDownload", false);
        OfflineMode = xmlSettings.ExtractBool("OfflineMode", false);
        ShowBasicShowDetails = xmlSettings.ExtractBool("ShowBasicShowDetails", false);
        DetailedRSSJSONLogging = xmlSettings.ExtractBool("DetailedRSSJSONLogging", false);
        ReplaceWithBetterQuality = xmlSettings.ExtractBool("ReplaceWithBetterQuality", false);
        ReplaceMoviesWithBetterQuality = xmlSettings.ExtractBool("ReplaceMoviesWithBetterQuality", false);
        ForceSystemToDecideOnUpgradedFiles = xmlSettings.ExtractBool("ForceSystemToDecideOnUpgradedFiles", false);
        ExportWTWRSSTo = xmlSettings.ExtractString("ExportWTWRSSTo");
        ExportWTWXML = xmlSettings.ExtractBool("ExportWTWXML", false);
        ExportWTWXMLTo = xmlSettings.ExtractString("ExportWTWXMLTo");
        ExportWTWTXT = xmlSettings.ExtractBool("ExportWTWTXT", false);
        ExportWTWTXTTo = xmlSettings.ExtractString("ExportWTWTXTTo");
        ExportWTWICAL = xmlSettings.ExtractBool("ExportWTWICAL", false);
        ExportWTWICALTo = xmlSettings.ExtractString("ExportWTWICALTo");
        WTWRecentDays = xmlSettings.ExtractInt("WTWRecentDays", 7);
        StartupTab = TabNumberFromName(xmlSettings.ExtractString("StartupTab2"));
        NamingStyle.StyleString = xmlSettings.ExtractStringOrNull("NamingStyle") ??
                                  CustomEpisodeName.OldNStyle(
                                      xmlSettings.ExtractInt("DefaultNamingStyle", 1)); // old naming style
        NotificationAreaIcon = xmlSettings.ExtractBool("NotificationAreaIcon", false);
        VideoExtensionsString = xmlSettings.ExtractString("VideoExtensions",
            xmlSettings.ExtractString("GoodExtensions", VideoExtensionsStringDEFAULT));
        OtherExtensionsString = xmlSettings.ExtractString("OtherExtensions", OtherExtensionsStringDEFAULT);
        subtitleExtensionsString = xmlSettings.ExtractString("SubtitleExtensions", subtitleExtensionsStringDEFAULT);
        ExportRSSMaxDays = xmlSettings.ExtractInt("ExportRSSMaxDays", 7).Between(0, 365);
        ExportRSSMaxShows = xmlSettings.ExtractInt("ExportRSSMaxShows", 10).Between(0, 10000);
        ExportRSSDaysPast = xmlSettings.ExtractInt("ExportRSSDaysPast", 0).Between(0, 365);
        MinRSSSeeders = xmlSettings.ExtractInt("MinRSSSeeders", 10).Between(0, 10000);
        KeepTogether = xmlSettings.ExtractBool("KeepTogether", true);
        LeadingZeroOnSeason = xmlSettings.ExtractBool("LeadingZeroOnSeason", false);
        ShowAccessibilityOptions = xmlSettings.ExtractBool("ShowAccessibilityOptions", false);
        ShowInTaskbar = xmlSettings.ExtractBool("ShowInTaskbar", true);
        RenameTxtToSub = xmlSettings.ExtractBool("RenameTxtToSub", false);
        ShowEpisodePictures = xmlSettings.ExtractBool("ShowEpisodePictures", true);
        HideWtWSpoilers = xmlSettings.ExtractBool("HideWtWSpoilers", false);
        HideMyShowsSpoilers = xmlSettings.ExtractBool("HideMyShowsSpoilers", false);
        AutoCreateFolders = xmlSettings.ExtractBool("AutoCreateFolders", false);
        AutoSelectShowInMyShows = xmlSettings.ExtractBool("AutoSelectShowInMyShows", true);
        SpecialsFolderName = xmlSettings.ExtractString("SpecialsFolderName", "Specials");
        SeasonFolderFormat = xmlSettings.ExtractString("SeasonFolderFormat");
        DefaultTvShowFolderFormat = xmlSettings.ExtractString("DefaultTvShowFolderFormat", "{ShowName}");
        MovieFolderFormat = xmlSettings.ExtractString("MovieFolderFormat", "{ShowName} ({Year})");
        MovieFilenameFormat = xmlSettings.ExtractString("MovieFilenameFormat", "{ShowName} ({Year})");
        SearchJSONURL = xmlSettings.ExtractString("SearchJSONURL", "https://eztv.ag/api/get-torrents?imdb_id=");
        SearchJSONRootNode = xmlSettings.ExtractString("SearchJSONRootNode", "torrents");
        SearchJSONFilenameToken = xmlSettings.ExtractString("SearchJSONFilenameToken", "filename");
        SearchJSONURLToken = xmlSettings.ExtractString("SearchJSONURLToken", "torrent_url");
        SearchJSONFileSizeToken = xmlSettings.ExtractString("SearchJSONFileSizeToken", "size_bytes");
        SearchJSONSeedersToken = xmlSettings.ExtractString("SearchJSONSeedersToken", "seeds");
        SABAPIKey = xmlSettings.ExtractString("SABAPIKey");
        CheckSABnzbd = xmlSettings.ExtractBool("CheckSABnzbd", false);
        SABHostPort = xmlSettings.ExtractString("SABHostPort");
        PreferredTVDBLanguage = Languages.Instance.GetLanguageFromCode(xmlSettings.ExtractString("PreferredLanguage", "en")) ?? Languages.Instance.FallbackLanguage;
        WTWDoubleClick = xmlSettings.ExtractEnum("WTWDoubleClick", WTWDoubleClickAction.Scan);
        ExportMissingXML = xmlSettings.ExtractBool("ExportMissingXML", false);
        ExportMissingXMLTo = xmlSettings.ExtractString("ExportMissingXMLTo");
        ExportRecentXSPF = xmlSettings.ExtractBool("ExportRecentXSPF", false);
        ExportRecentXSPFTo = xmlSettings.ExtractString("ExportRecentXSPFTo");
        ExportRecentM3U = xmlSettings.ExtractBool("ExportRecentM3U", false);
        ExportRecentM3UTo = xmlSettings.ExtractString("ExportRecentM3UTo");
        ExportRecentASX = xmlSettings.ExtractBool("ExportRecentASX", false);
        ExportRecentASXTo = xmlSettings.ExtractString("ExportRecentASXTo");
        ExportRecentWPL = xmlSettings.ExtractBool("ExportRecentWPL", false);
        ExportRecentWPLTo = xmlSettings.ExtractString("ExportRecentWPLTo");
        ExportMissingCSV = xmlSettings.ExtractBool("ExportMissingCSV", false);
        ExportMissingCSVTo = xmlSettings.ExtractString("ExportMissingCSVTo");
        ExportMoviesTXT = xmlSettings.ExtractBool("ExportMoviesTXT", false);
        ExportMoviesTXTTo = xmlSettings.ExtractString("ExportMoviesTXTTo");
        ExportMoviesHTML = xmlSettings.ExtractBool("ExportMoviesHTML", false);
        ExportMoviesHTMLTo = xmlSettings.ExtractString("ExportMoviesHTMLTo");
        ExportMissingMoviesXML = xmlSettings.ExtractBool("ExportMissingMoviesXML", false);
        ExportMissingMoviesXMLTo = xmlSettings.ExtractString("ExportMissingMoviesXMLTo");
        ExportMissingMoviesCSV = xmlSettings.ExtractBool("ExportMissingMoviesCSV", false);
        ExportMissingMoviesCSVTo = xmlSettings.ExtractString("ExportMissingMoviesCSVTo");
        ExportRenamingXML = xmlSettings.ExtractBool("ExportRenamingXML", false);
        ExportRenamingXMLTo = xmlSettings.ExtractString("ExportRenamingXMLTo");
        ExportFOXML = xmlSettings.ExtractBool("ExportFOXML", false);
        ExportFOXMLTo = xmlSettings.ExtractString("ExportFOXMLTo");
        ExportShowsTXT = xmlSettings.ExtractBool("ExportShowsTXT", false);
        ExportShowsTXTTo = xmlSettings.ExtractString("ExportShowsTXTTo");
        ExportShowsHTML = xmlSettings.ExtractBool("ExportShowsHTML", false);
        ExportShowsHTMLTo = xmlSettings.ExtractString("ExportShowsHTMLTo");
        ForceLowercaseFilenames = xmlSettings.ExtractBool("ForceLowercaseFilenames", false);
        IgnoreSamples = xmlSettings.ExtractBool("IgnoreSamples", true);
        SampleFileMaxSizeMB = xmlSettings.ExtractInt("SampleFileMaxSizeMB", 50);
        ParallelDownloads = xmlSettings.ExtractInt("ParallelDownloads", 4);
        uTorrentPath = xmlSettings.ExtractString("uTorrentPath");
        ResumeDatPath = xmlSettings.ExtractString("ResumeDatPath");
        SearchRSS = xmlSettings.ExtractBool("SearchRSS", false);
        SearchJSON = xmlSettings.ExtractBool("SearchJSON", false);
        SearchRSSManualScanOnly = xmlSettings.ExtractBool("SearchRSSManualScanOnly", true);
        SearchJSONManualScanOnly = xmlSettings.ExtractBool("SearchJSONManualScanOnly", true);
        EpThumbnails = xmlSettings.ExtractBool("EpImgs", false);
        NFOMovies = xmlSettings.ExtractBool("NFOMovies", true);
        NFOShows = xmlSettings.ExtractBool("NFOShows") ?? xmlSettings.ExtractBool("NFOs", false);
        NFOEpisodes = xmlSettings.ExtractBool("NFOEpisodes") ?? xmlSettings.ExtractBool("NFOs", false);
        KODIImages = xmlSettings.ExtractBool("KODIImages") ??
                     xmlSettings.ExtractBool("XBMCImages", false); //Backward Compatibility
        pyTivoMeta = xmlSettings.ExtractBool("pyTivoMeta", false);
        wdLiveTvMeta = xmlSettings.ExtractBool("wdLiveTvMeta", false);
        pyTivoMetaSubFolder = xmlSettings.ExtractBool("pyTivoMetaSubFolder", false);
        FolderJpg = xmlSettings.ExtractBool("FolderJpg", false);
        FolderJpgIs = xmlSettings.ExtractEnum("FolderJpgIs", FolderJpgIsType.Poster);
        MonitoredFoldersScanType = xmlSettings.ExtractEnum("MonitoredFoldersScanType", ScanType.Full);
        DefaultProvider = xmlSettings.ExtractEnum("DefaultProvider", TVDoc.ProviderType.TMDB);
        RenameCheck = xmlSettings.ExtractBool("RenameCheck", true);
        PreventMove = xmlSettings.ExtractBool("PreventMove", false);
        CheckuTorrent = xmlSettings.ExtractBool("CheckuTorrent", false);
        RestrictMissingExportsToFullScans = xmlSettings.ExtractBool("RestrictMissingExportsToFullScans", true);
        CheckqBitTorrent = xmlSettings.ExtractBool("CheckqBitTorrent", false);
        RemoveCompletedTorrents = xmlSettings.ExtractBool("RemoveCompletedTorrents", false);
        qBitTorrentHost = xmlSettings.ExtractString("qBitTorrentHost", "localhost");
        qBitTorrentPort = xmlSettings.ExtractString("qBitTorrentPort", "8080");
        qBitTorrentUseHTTPS = xmlSettings.ExtractBool("qBitTorrentUseHTTPS", false);
        qBitTorrentAPIVersion = xmlSettings.ExtractEnum("qBitTorrentAPIVersion", qBitTorrent.qBitTorrentAPIVersion.v2);
        MissingCheck = xmlSettings.ExtractBool("MissingCheck", true);
        MoveLibraryFiles = xmlSettings.ExtractBool("MoveLibraryFiles", true);
        CorrectFileDates = xmlSettings.ExtractBool("UpdateFileDates", false);
        SearchLocally = xmlSettings.ExtractBool("SearchLocally", true);
        IgnorePreviouslySeen = xmlSettings.ExtractBool("IgnorePreviouslySeen", false);
        IgnorePreviouslySeenMovies = xmlSettings.ExtractBool("IgnorePreviouslySeenMovies", false);
        LeaveOriginals = xmlSettings.ExtractBool("LeaveOriginals", false);
        AutoSearchForDownloadedFiles = xmlSettings.ExtractBool("AutoSearchForDownloadedFiles", false);
        AutoMergeDownloadEpisodes = xmlSettings.ExtractBool("AutoMergeEpisodes", false);
        AutoMergeLibraryEpisodes = xmlSettings.ExtractBool("AutoMergeLibraryEpisodes", false);
        RetainLanguageSpecificSubtitles = xmlSettings.ExtractBool("RetainLanguageSpecificSubtitles", true);
        ForceBulkAddToUseSettingsOnly = xmlSettings.ExtractBool("ForceBulkAddToUseSettingsOnly", false);
        MonitorFolders = xmlSettings.ExtractBool("MonitorFolders", false);
        runStartupCheck = xmlSettings.ExtractBool("StartupScan", false);
        runPeriodicCheck = xmlSettings.ExtractBool("PeriodicScan", false);
        periodCheckHours = xmlSettings.ExtractInt("PeriodicScanHours", 1);
        periodUpdateCacheHours = xmlSettings.ExtractInt("PeriodicUpdateCacheHours", 1);
        RemoveDownloadDirectoriesFiles = xmlSettings.ExtractBool("RemoveDownloadDirectoriesFiles", false);
        RemoveDownloadDirectoriesFilesMatchMovies = xmlSettings.ExtractBool("RemoveDownloadDirectoriesFilesMatchMovies", false);
        RemoveDownloadDirectoriesFilesMatchMoviesLengthCheck = xmlSettings.ExtractBool("RemoveDownloadDirectoriesFilesMatchMoviesLengthCheck", false);
        RemoveDownloadDirectoriesFilesMatchMoviesLengthCheckLength = xmlSettings.ExtractInt("RemoveDownloadDirectoriesFilesMatchMoviesLengthCheckLength", 8);
        DoBulkAddInScan = xmlSettings.ExtractBool("DoBulkAddInScan", false);
        DeleteShowFromDisk = xmlSettings.ExtractBool("DeleteShowFromDisk", true);
        FileNameCaseSensitiveMatch = xmlSettings.ExtractBool("FileNameCaseSensitiveMatch", false);
        CopySubsFolders = xmlSettings.ExtractBool("CopySubsFolders", true);
        DeleteMovieFromDisk = xmlSettings.ExtractBool("DeleteMovieFromDisk", false);
        UseGlobalReleaseDate = xmlSettings.ExtractBool("UseGlobalReleaseDate", false);
        EpJPGs = xmlSettings.ExtractBool("EpJPGs", false);
        SeriesJpg = xmlSettings.ExtractBool("SeriesJpg", false);
        Mede8erXML = xmlSettings.ExtractBool("Mede8erXML", false);
        GroupMissingEpisodesIntoSeasons = xmlSettings.ExtractBool("GroupMissingEpisodesIntoSeasons", true);
        ShrinkLargeMede8erImages = xmlSettings.ExtractBool("ShrinkLargeMede8erImages", false);
        FanArtJpg = xmlSettings.ExtractBool("FanArtJpg", false);
        BulkAddIgnoreRecycleBin = xmlSettings.ExtractBool("BulkAddIgnoreRecycleBin", false);
        BulkAddCompareNoVideoFolders = xmlSettings.ExtractBool("BulkAddCompareNoVideoFolders", false);
        AutoAddMovieTerms = xmlSettings.ExtractString("AutoAddMovieTerms", "dvdrip;camrip;screener;dvdscr;r5;bluray");
        AutoAddIgnoreSuffixes = xmlSettings.ExtractString("AutoAddIgnoreSuffixes", "1080p;720p;2160p");
        PriorityReplaceTerms = xmlSettings.ExtractString("PriorityReplaceTerms", "PROPER;REPACK;RERIP");
        mode = xmlSettings.ExtractEnum("BetaMode", BetaMode.ProductionOnly);
        upgradeDirtyPercent = xmlSettings.ExtractFloat("PercentDirtyUpgrade", 20);
        replaceMargin = xmlSettings.ExtractFloat("PercentBetter", 10);
        defaultSeasonWord = xmlSettings.ExtractString("BaseSeasonName", "Season");
        searchSeasonWordsString = xmlSettings.ExtractString("SearchSeasonNames", "Season;Series;Saison;Temporada;Seizoen");
        SubsFolderNamesString =
            xmlSettings.ExtractString("SubsFolderNamesString", "subs;subtitle;vobsubs;sub;vobsub;subtitle");
        preferredRSSSearchTermsString = xmlSettings.ExtractString("PreferredRSSSearchTerms", "720p;1080p;2160p");
        unwantedRSSSearchTermsString = xmlSettings.ExtractString("UnwantedRSSSearchTerms", "hdcam;tscam;CAMRip;HDCAM;HDTS");
        keepTogetherMode = xmlSettings.ExtractEnum("KeepTogetherType", KeepTogetherModes.All);
        UIScanType = xmlSettings.ExtractEnum("UIScanType", ScanType.Full);
        keepTogetherExtensionsString = xmlSettings.ExtractString("KeepTogetherExtensions", keepTogetherExtensionsStringDEFAULT);
        ExportWTWRSS = xmlSettings.ExtractBool("ExportWTWRSS", false);
        CopyFutureDatedEpsFromSearchFolders = xmlSettings.ExtractBool("CopyFutureDatedEpsFromSearchFolders", false);
        ShareLogs = xmlSettings.ExtractBool("ShareLogs", true);
        PostpendThe = xmlSettings.ExtractBool("PostpendThe", false);
        IgnoreAllSpecials = xmlSettings.ExtractBool("IgnoreAllSpecials", false);
        UseFullPathNameToMatchLibraryFolders = xmlSettings.ExtractBool("UseFullPathNameToMatchLibraryFolders", false);
        UseFullPathNameToMatchSearchFolders = xmlSettings.ExtractBool("UseFullPathNameToMatchSearchFolders", false);
        AutoAddAsPartOfQuickRename = xmlSettings.ExtractBool("AutoAddAsPartOfQuickRename", true);
        AutomateAutoAddWhenOneMovieFound = xmlSettings.ExtractBool("AutomateAutoAddWhenOneMovieFound", true);
        AutomateAutoAddWhenOneShowFound = xmlSettings.ExtractBool("AutomateAutoAddWhenOneShowFound", true);
        ChooseWhenMultipleEpisodesMatch = xmlSettings.ExtractBool("ChooseWhenMultipleEpisodesMatch", false);

        DefShowIncludeNoAirdate = xmlSettings.ExtractBool("DefShowIncludeNoAirdate", false);
        DefShowIncludeFuture = xmlSettings.ExtractBool("DefShowIncludeFuture", false);
        DefShowNextAirdate = xmlSettings.ExtractBool("DefShowNextAirdate", true);
        DefShowDVDOrder = xmlSettings.ExtractBool("DefShowDVDOrder", false);
        DefShowAlternateOrder = xmlSettings.ExtractBool("DefShowAlternateOrder", false);
        DefShowDoRenaming = xmlSettings.ExtractBool("DefShowDoRenaming", true);
        DefShowDoMissingCheck = xmlSettings.ExtractBool("DefShowDoMissingCheck", true);

        DefShowSequentialMatching = xmlSettings.ExtractBool("DefShowSequentialMatching", false);
        DefShowAirDateMatching = xmlSettings.ExtractBool("DefShowAirDateMatching", xmlSettings.ExtractBool("LookForDateInFilename", false));
        DefShowEpNameMatching = xmlSettings.ExtractBool("DefShowEpNameMatching", false);

        DefShowSpecialsCount = xmlSettings.ExtractBool("DefShowSpecialsCount", false);
        DefShowAutoFolders = xmlSettings.ExtractBool("DefShowAutoFolders", true);
        DefShowUseDefLocation = xmlSettings.ExtractBool("DefShowUseDefLocation", false);
        DefShowUseBase = xmlSettings.ExtractBool("DefShowUseBase", false);
        DefShowUseSubFolders = xmlSettings.ExtractBool("DefShowUseSubFolders", true);
        DefShowLocation = xmlSettings.ExtractString("DefShowLocation", string.Empty);
        DefaultShowTimezoneName = xmlSettings.ExtractString("DefaultShowTimezoneName");

        DefMovieDoRenaming = xmlSettings.ExtractBool("DefMovieDoRenaming", true);
        DefMovieDoMissingCheck = xmlSettings.ExtractBool("DefMovieDoMissingCheck", true);
        DefMovieUseAutomaticFolders = xmlSettings.ExtractBool("DefMovieUseutomaticFolders", true);
        DefMovieUseDefaultLocation = xmlSettings.ExtractBool("DefMovieUseDefaultLocation", true);
        DefMovieDefaultLocation = xmlSettings.ExtractStringOrNull("DefMovieDefaultLocation");
        DefaultMovieProvider = xmlSettings.ExtractEnum("DefaultMovieProvider", TVDoc.ProviderType.TMDB);
        DefMovieCheckFutureDatedMovies = xmlSettings.ExtractBool("DefMovieCheckFutureDatedMovies", false);
        DefMovieCheckNoDatedMovies = xmlSettings.ExtractBool("DefMovieCheckNoDatedMovies", false);

        UnattendedMultiActionOutcome = xmlSettings.ExtractEnum("UnattendedMultiActionOutcome", DuplicateActionOutcome.IgnoreAll);
        UserMultiActionOutcome = xmlSettings.ExtractEnum("UserMultiActionOutcome", DuplicateActionOutcome.MostSeeders);
        DefMovieFolderFormat = xmlSettings.ExtractEnum("DefMovieFolderFormat", MovieConfiguration.MovieFolderFormat.singleDirectorySingleFile);

        SearchJackett = xmlSettings.ExtractBool("SearchJackett", false);
        UseJackettTextSearch = xmlSettings.ExtractBool("UseJackettTextSearch", false);
        SearchJackettManualScanOnly = xmlSettings.ExtractBool("SearchJackettManualScanOnly", true);
        JackettServer = xmlSettings.ExtractString("JackettServer", "127.0.0.1");
        JackettPort = xmlSettings.ExtractString("JackettPort", "9117");
        JackettIndexer = xmlSettings.ExtractString("JackettIndexer", "/api/v2.0/indexers/all/results/torznab");
        JackettAPIKey = xmlSettings.ExtractString("JackettAPIKey");

        SearchJackettButton = xmlSettings.ExtractBool("SearchJackettButton", true);
        StopJackettSearchOnFullScan = xmlSettings.ExtractBool("StopJackettSearchOnFullScan", true);
        AutoSaveOnExit = xmlSettings.ExtractBool("AutoSaveOnExit", false);
        UnArchiveFilesInDownloadDirectory = xmlSettings.ExtractBool("UnArchiveFilesInDownloadDirectory", false);

        TMDBLanguage = Languages.Instance.GetLanguageFromDialectCode(xmlSettings.ExtractString("TMDBLanguage")) ?? Languages.Instance.FallbackLanguage;
        TMDBRegion = Regions.Instance.RegionFromCode(xmlSettings.ExtractString("TMDBRegion")) ?? Regions.Instance.FallbackRegion;
        TMDBPercentDirty = xmlSettings.ExtractFloat("TMDBPercentDirty", 20);
        IncludeMoviesQuickRecent = xmlSettings.ExtractBool("IncludeMoviesQuickRecent", false);

        Tidyup.Load(xmlSettings);
        RSSURLs = xmlSettings.Descendants("RSSURLs").FirstOrDefault()?.ReadStringsFromXml("URL") ?? [];
        TheSearchers = new Searchers(xmlSettings.Descendants("TheSearchers").FirstOrDefault(), MediaConfiguration.MediaType.tv);
        TheMovieSearchers = new Searchers(xmlSettings.Descendants("TheMovieSearchers").FirstOrDefault(), MediaConfiguration.MediaType.movie);

        UpdateAppUpdateSettings(xmlSettings);

        UpdateReplacements(xmlSettings);
        UpdateRegExs(xmlSettings);
        UpdateShowStatus(xmlSettings);
        UpdateFiters(xmlSettings);
    }

    private void UpdateAppUpdateSettings(XElement xmlSettings)
    {
        XElement? subElement = xmlSettings.Element("AppUpdate");
        if (subElement != null)
        {
            UpdateCheckInterval = TimeSpan.Parse(subElement.ExtractString("Interval", 1.Hours().ToString()));
            UpdateCheckType = Enum.Parse<UpdateCheckMode>(subElement.ExtractString("Mode", ((int)UpdateCheckMode.Everytime).ToString()));
            SuppressUpdateAvailablePopup = subElement.ExtractBool("SuppressPopup", false);
        }
    }

    private void UpdateFiters(XElement xmlSettings)
    {
        SeasonFilter = new SeasonFilter
        {
            HideIgnoredSeasons = XmlConvert.ToBoolean(xmlSettings.Descendants("SeasonFilters")
                                                          .Descendants("SeasonIgnoredFilter").FirstOrDefault()?.Value ??
                                                      "false")
        };

        XElement? ShowFiltersNode = xmlSettings.Descendants("ShowFilters").FirstOrDefault();

        Filter = new ShowFilter
        {
            ShowName = ShowFiltersNode?.Descendants("ShowNameFilter").Attributes("ShowName")
                .FirstOrDefault()?.Value,

            ShowStatus = ShowFiltersNode?.Descendants("ShowStatusFilter").Attributes("ShowStatus")
                .FirstOrDefault()?.Value,
            ShowRating = ShowFiltersNode?.Descendants("ShowRatingFilter").Attributes("ShowRating")
                .FirstOrDefault()?.Value,
            ShowNetwork = ShowFiltersNode?.Descendants("ShowNetworkFilter").Attributes("ShowNetwork")
                .FirstOrDefault()?.Value,

            ShowStatusInclude = (bool?)ShowFiltersNode?.Descendants("ShowStatusFilter").Attributes("ShowStatusInclude")
                .FirstOrDefault() ?? true,
            ShowRatingInclude = (bool?)ShowFiltersNode?.Descendants("ShowRatingFilter").Attributes("ShowRatingInclude")
                .FirstOrDefault() ?? true,
            ShowNetworkInclude = (bool?)ShowFiltersNode?.Descendants("ShowNetworkFilter").Attributes("ShowNetworkInclude")
                .FirstOrDefault() ?? true,

            IncludeBlankFields = (bool?)ShowFiltersNode?.Descendants("IncludeBlankFields").Attributes("IncludeBlankFields")
                .FirstOrDefault() ?? true,
        };

        foreach (XAttribute rep in ShowFiltersNode?.Descendants("GenreFilter").Attributes("Genre") ?? [])
        {
            Filter.Genres.Add(rep.Value);
        }

        XElement? MovieFilterNode = xmlSettings.Descendants("MovieFilter").FirstOrDefault();

        MovieFilter = new MovieFilter
        {
            ShowName = MovieFilterNode?.Descendants("ShowNameFilter").Attributes("ShowName")
                .FirstOrDefault()?.Value,

            ShowStatus = MovieFilterNode?.Descendants("ShowStatusFilter").Attributes("ShowStatus")
                .FirstOrDefault()?.Value,
            ShowRating = MovieFilterNode?.Descendants("ShowRatingFilter").Attributes("ShowRating")
                .FirstOrDefault()?.Value,
            ShowNetwork = MovieFilterNode?.Descendants("ShowNetworkFilter").Attributes("ShowNetwork")
                .FirstOrDefault()?.Value,
            ShowYear = MovieFilterNode?.Descendants("ShowYearFilter").Attributes("ShowYear")
                .FirstOrDefault()?.Value,

            ShowStatusInclude = (bool?)MovieFilterNode?.Descendants("ShowStatusFilter").Attributes("ShowStatusInclude")
                .FirstOrDefault() ?? true,
            ShowRatingInclude = (bool?)MovieFilterNode?.Descendants("ShowRatingFilter").Attributes("ShowRatingInclude")
                .FirstOrDefault() ?? true,
            ShowNetworkInclude = (bool?)MovieFilterNode?.Descendants("ShowNetworkFilter").Attributes("ShowNetworkInclude")
                .FirstOrDefault() ?? true,
            ShowYearInclude = (bool?)MovieFilterNode?.Descendants("ShowYearFilter").Attributes("ShowYearInclude")
                .FirstOrDefault() ?? true,
            IncludeBlankFields = (bool?)MovieFilterNode?.Descendants("IncludeBlankFields").Attributes("IncludeBlankFields")
                .FirstOrDefault() ?? true
        };

        foreach (XAttribute rep in MovieFilterNode?.Descendants("GenreFilter").Attributes("Genre") ?? [])
        {
            MovieFilter.Genres.Add(rep.Value);
        }

        if (SeasonFolderFormat == string.Empty)
        {
            //this has not been set from the XML, so we should give it an appropriate default value
            SeasonFolderFormat = defaultSeasonWord.Trim() + " " + (LeadingZeroOnSeason ? "{Season:2}" : "{Season}");
        }
    }

    private void UpdateShowStatus(XElement xmlSettings)
    {
        ShowStatusColors = [];
        foreach (XElement rep in xmlSettings.Descendants("ShowStatusTVWColors").FirstOrDefault()
                     ?.Descendants("ShowStatusTVWColor") ?? [])
        {
            ColouringRule? newRule = ExtractColouringRule(rep);
            if (newRule is null)
            {
                continue;
            }
            string? color = rep.Attribute("Color")?.Value;
            if (string.IsNullOrEmpty(color))
            {
                continue;
            }

            System.Drawing.Color c = System.Drawing.ColorTranslator.FromHtml(color);
            ShowStatusColors.Add(newRule, c);
        }
    }

    private static ColouringRule? ExtractColouringRule(XElement rep)
    {
        string? showStatus = rep.Attribute("ShowStatus")?.Value;
        if (showStatus is null)
        {
            return null;
        }

        string? type = rep.Attribute("Type")?.Value;
        if (type is null)
        {
            //Old Style Rule; lets convert
            string? isMetaString = rep.Attribute("IsMeta")?.Value;
            string? isShowLevelString = rep.Attribute("IsShowLevel")?.Value;
            bool isMeta = bool.Parse(isMetaString ?? "false");
            bool isShowLevel = bool.Parse(isShowLevelString ?? "true");

            if (!isShowLevel)
            {
                return new SeasonStatusColouringRule(ConvertToSeasonStatus(showStatus));
            }

            if (isMeta)
            {
                return new ShowAirStatusColouringRule(ConvertToShowAirStatus(showStatus));
            }

            return new ShowStatusColouringRule(showStatus);
        }

        return type switch
        {
            "SeasonStatusColouringRule" => new SeasonStatusColouringRule(ExtractEnum<ProcessedSeason.SeasonStatus>(showStatus)),
            "ShowStatusColouringRule" => new ShowStatusColouringRule(showStatus),
            "ShowAirStatusColouringRule" => new ShowAirStatusColouringRule(ExtractEnum<ShowConfiguration.ShowAirStatus>(showStatus)),
            _ => throw new NotSupportedException($"type = {type} is not supported by {System.Reflection.MethodBase.GetCurrentMethod()}")
        };
    }

    private static T ExtractEnum<T>(string value)
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("T must be an enumerated type");
        }

        if (value is null)
        {
            throw new ArgumentNullException(nameof(value), value);
        }

        int val = int.Parse(value);

        if (typeof(T).IsEnumDefined(val))
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        throw new ArgumentException($"'{value}' cannot be mapped to type {typeof(T)}", nameof(value));
    }

    private static ShowConfiguration.ShowAirStatus ConvertToShowAirStatus(string value)
    {
        return value switch
        {
            "All aired" => ShowConfiguration.ShowAirStatus.aired,
            "Aired" => ShowConfiguration.ShowAirStatus.aired,
            "aired" => ShowConfiguration.ShowAirStatus.aired,
            "No Seasons or Episodes in Seasons" => ShowConfiguration.ShowAirStatus.noEpisodesOrSeasons,
            "NoEpisodesOrSeasons" => ShowConfiguration.ShowAirStatus.noEpisodesOrSeasons,
            "noEpisodesOrSeasons" => ShowConfiguration.ShowAirStatus.noEpisodesOrSeasons,
            "None aired" => ShowConfiguration.ShowAirStatus.noneAired,
            "NoneAired" => ShowConfiguration.ShowAirStatus.noneAired,
            "noneAired" => ShowConfiguration.ShowAirStatus.noneAired,
            "Partially aired" => ShowConfiguration.ShowAirStatus.partiallyAired,
            "PartiallyAired" => ShowConfiguration.ShowAirStatus.partiallyAired,
            "partiallyAired" => ShowConfiguration.ShowAirStatus.partiallyAired,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value)
        };
    }

    private static ProcessedSeason.SeasonStatus ConvertToSeasonStatus(string value)
    {
        return value switch
        {
            "All aired" => ProcessedSeason.SeasonStatus.aired,
            "Aired" => ProcessedSeason.SeasonStatus.aired,
            "aired" => ProcessedSeason.SeasonStatus.aired,
            "No Episodes" => ProcessedSeason.SeasonStatus.noEpisodes,
            "NoEpisodes" => ProcessedSeason.SeasonStatus.noEpisodes,
            "noEpisodes" => ProcessedSeason.SeasonStatus.noEpisodes,
            "None aired" => ProcessedSeason.SeasonStatus.noneAired,
            "NoneAired" => ProcessedSeason.SeasonStatus.noneAired,
            "noneAired" => ProcessedSeason.SeasonStatus.noneAired,
            "Partially aired" => ProcessedSeason.SeasonStatus.partiallyAired,
            "PartiallyAired" => ProcessedSeason.SeasonStatus.partiallyAired,
            "partiallyAired" => ProcessedSeason.SeasonStatus.partiallyAired,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value)
        };
    }

    private void UpdateRegExs(XElement xmlSettings)
    {
        FNPRegexs.Clear();
        foreach (XElement rep in xmlSettings.Descendants("FNPRegexs").FirstOrDefault()?.Descendants("Regex") ??
                                 [])
        {
            string? enabledValue = rep.Attribute("Enabled")?.Value;
            string? reValue = rep.Attribute("RE")?.Value;
            string? useFullPathValue = rep.Attribute("UseFullPath")?.Value;
            string? notesValue = rep.Attribute("Notes")?.Value;

            bool enabled = enabledValue is not null && XmlConvert.ToBoolean(enabledValue);
            bool useFullPath = useFullPathValue is not null && XmlConvert.ToBoolean(useFullPathValue);

            if (reValue != null && notesValue != null)
            {
                FNPRegexs.Add(new FilenameProcessorRE(enabled, reValue, useFullPath, notesValue));
            }
        }
    }

    private void UpdateReplacements(XElement xmlSettings)
    {
        Replacements.Clear();
        foreach (XElement rep in xmlSettings.Descendants("Replacements").FirstOrDefault()?.Descendants("Replace") ??
                                 [])
        {
            string? thisValue = rep.Attribute("This")?.Value;
            string? thatValue = rep.Attribute("That")?.Value;
            string? caseInsensitiveValue = rep.Attribute("CaseInsensitive")?.Value;
            bool caseInsensitive = caseInsensitiveValue == "Y";

            if (thisValue != null && thatValue != null)
            {
                Replacements.Add(new Replacement(thisValue, thatValue, caseInsensitive));
            }
        }
    }

    public string DefaultTVShowFolder(ShowConfiguration showConfiguration)
    {
        string style = DefaultTvShowFolderFormat;
        string folderName = CustomTvShowName.DirectoryNameFor(showConfiguration, style);
        return DefaultTVShowFolder(folderName);
    }
    public string DefaultTVShowFolder(string baseHint)
    {
        return FilenameFriendly(FileHelper.MakeValidPath(baseHint));
    }

    public string DefaultTVShowFolder(CachedSeriesInfo showConfiguration)
    {
        string style = DefaultTvShowFolderFormat;
        string folderName = CustomTvShowName.DirectoryNameFor(showConfiguration, style);
        return DefaultTVShowFolder(folderName);
    }

    public static string SeasonNameFor(int seasonNumber) => seasonNumber != 0 ? seasonNumber.ToString() : SpecialsListViewName;
}
