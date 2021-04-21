// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Alphaleonis.Win32.Filesystem;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Humanizer;
using JetBrains.Annotations;

// ReSharper disable RedundantDefaultMemberInitializer
// ReSharper disable InconsistentNaming

// Settings for TVRename.  All of this stuff is through Options->Preferences in the app.

namespace TVRename
{
    public sealed class TVSettings
    {
        //We are using the singleton design pattern
        //http://msdn.microsoft.com/en-au/library/ff650316.aspx

        private static volatile TVSettings? instance;
        private static readonly object syncRoot = new object();

        [NotNull]
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
        private const string subtitleExtensionsStringDEFAULT = ".srt;.sub;.sbv;.idx";

        #region FolderJpgIsType enum

        public enum FolderJpgIsType
        {
            Banner,
            Poster,
            FanArt,
            SeasonPoster
        }

        #endregion

        #region WTWDoubleClickAction enum

        public enum WTWDoubleClickAction
        {
            Search,
            Scan
        }

        #endregion

        #region ScanType enum
        public enum ScanType
        {
            Full,
            Recent,
            Quick,
            SingleShow
        }
        #endregion

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
        #endregion
        
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

        public List<string> LibraryFolders;
        public List<string> MovieLibraryFolders;
        public List<string> IgnoreFolders;
        public List<string> DownloadFolders;
        public List<string> IgnoredAutoAddHints;
        public List<IgnoreItem> Ignore;
        public bool AutoSelectShowInMyShows = true;
        public bool AutoCreateFolders = false;
        public bool BGDownload = false;
        public bool CheckuTorrent = false;
        public bool CheckqBitTorrent = false;
        public bool RemoveCompletedTorrents = false;
        public string qBitTorrentHost = "localhost";
        public string qBitTorrentPort = "8080";
        public qBitTorrent.qBitTorrentAPIVersion qBitTorrentAPIVersion = qBitTorrent.qBitTorrentAPIVersion.v2;
        public bool EpTBNs = false;
        public bool EpJPGs = false;
        public bool SeriesJpg = false;
        public bool ShrinkLargeMede8erImages = false;
        public bool FanArtJpg = false;
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
        public CustomEpisodeName NamingStyle = new CustomEpisodeName();
        public bool NotificationAreaIcon = false;
        public bool OfflineMode = false;
        public bool DetailedRSSJSONLogging = false;
        public bool ShowBasicShowDetails = false;
        public bool RSSUseCloudflare = true;
        public bool SearchJSONUseCloudflare = true;
        public bool qBitTorrentDownloadFilesFirst = true;
        public TVDoc.ProviderType DefaultProvider = TVDoc.ProviderType.TheTVDB;

        public string? TMDBLanguage;
        public string? TMDBRegion;
        public float TMDBPercentDirty;
        public bool IncludeMoviesQuickRecent;

        public BetaMode mode = BetaMode.ProductionOnly;
        public float upgradeDirtyPercent = 20;
        public float replaceMargin = 10;
        public bool ReplaceWithBetterQuality = false;
        public bool ForceSystemToDecideOnUpgradedFiles = false;
        public KeepTogetherModes keepTogetherMode = KeepTogetherModes.All;

        public bool BulkAddIgnoreRecycleBin = false;
        public bool BulkAddCompareNoVideoFolders = false;
        public ScanType UIScanType = ScanType.Full;

        public string AutoAddMovieTerms = "dvdrip;camrip;screener;dvdscr;r5;bluray";
        [NotNull]
        public IEnumerable<string> AutoAddMovieTermsArray => Convert(AutoAddMovieTerms);

        internal string FilenameFriendly([NotNull] ShowConfiguration show, [NotNull] Episode sourceEp)
        {
            // ReSharper disable once ArrangeMethodOrOperatorBody
            return FilenameFriendly(NamingStyle.GetTargetEpisodeName(show, sourceEp));
        }

        public string PriorityReplaceTerms = "PROPER;REPACK;RERIP";
        [NotNull]
        public IEnumerable<string> PriorityReplaceTermsArray => Convert(PriorityReplaceTerms);

        public string AutoAddIgnoreSuffixes = "1080p;720p";
        [NotNull]
        public IEnumerable<string> AutoAddIgnoreSuffixesArray => Convert(AutoAddIgnoreSuffixes);

        public string keepTogetherExtensionsString;
        [NotNull]
        public IEnumerable<string> keepTogetherExtensionsArray => Convert(keepTogetherExtensionsString);

        public string subtitleExtensionsString;
        [NotNull]
        public IEnumerable<string> subtitleExtensionsArray => Convert(subtitleExtensionsString);

        public string searchSeasonWordsString = "Season;Series;Saison;Temporada;Seizoen";
        [NotNull]
        public IEnumerable<string> searchSeasonWordsArray => Convert(searchSeasonWordsString);

        public string preferredRSSSearchTermsString = "720p;1080p";
        [NotNull]
        public string[] PreferredRSSSearchTerms() => Convert(preferredRSSSearchTermsString);

        public string OtherExtensionsString;
        [NotNull]
        private IEnumerable<string> OtherExtensionsArray => Convert(OtherExtensionsString);

        [NotNull]
        private static string[] Convert(string? propertyString)
        {
            return string.IsNullOrWhiteSpace(propertyString) ? new string[0] : propertyString.Split(';');
        }

        internal bool IncludeBetaUpdates() => mode == BetaMode.BetaToo;

        public string defaultSeasonWord = "Season";
        public ShowFilter Filter = new ShowFilter();
        public MovieFilter MovieFilter = new MovieFilter();
        public SeasonFilter SeasonFilter = new SeasonFilter();
        public int ParallelDownloads =4;
        public List<string> RSSURLs = DefaultRSSURLList();
        public bool RenameCheck = true;
        public bool PreventMove = false;
        public bool RenameTxtToSub = false;
        public readonly List<Replacement> Replacements = DefaultListRE();
        public string ResumeDatPath;
        public int SampleFileMaxSizeMB=50; // sample file must be smaller than this to be ignored
        public bool SearchLocally = true;
        public bool IgnorePreviouslySeen = false;
        public bool IgnorePreviouslySeenMovies = false; //todo - check this is settable and persists to the XML file
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
        public Searchers TheSearchers = new Searchers(MediaConfiguration.MediaType.tv);
        public Searchers TheMovieSearchers = new Searchers(MediaConfiguration.MediaType.movie);

        public string SearchJSONURL= "https://eztv.ag/api/get-torrents?imdb_id=";
        public string SearchJSONRootNode = "torrents";
        public string SearchJSONFilenameToken = "filename";
        public string SearchJSONURLToken = "torrent_url";
        public string SearchJSONFileSizeToken = "size_bytes";
        public string SearchJSONSeedersToken = "seeds";

        [NotNull]
        public string[] VideoExtensionsArray => Convert(VideoExtensionsString);

        [NotNull]
        public static string USER_AGENT =>
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36";

        public static TheTVDB.LocalCache.PagingMethod TVDBPagingMethod => TheTVDB.LocalCache.PagingMethod.proper;

        [NotNull]
        public static string SpecialsListViewName => "Special";

        public MovieConfiguration.MovieFolderFormat DefMovieFolderFormat =>
            MovieConfiguration.MovieFolderFormat.singleDirectorySingleFile; //TODO fix this  //{ get; internal set; }

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

        public bool CleanLibraryAfterActions = false;
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
        public int WTWRecentDays =7;
        public string uTorrentPath;
        public bool MonitorFolders = false;
        public bool RemoveDownloadDirectoriesFiles = false;
        public bool RemoveDownloadDirectoriesFilesMatchMovies = false;
        public bool RemoveDownloadDirectoriesFilesMatchMoviesLengthCheck = false;
        public int RemoveDownloadDirectoriesFilesMatchMoviesLengthCheckLength = 8;
        public bool DeleteShowFromDisk = true;

        public ShowStatusColoringTypeList ShowStatusColors = new ShowStatusColoringTypeList();
        public string SABHostPort = string.Empty;
        public string SABAPIKey = string.Empty;
        public bool CheckSABnzbd = false;
        public string PreferredLanguageCode = "en";
        public WTWDoubleClickAction WTWDoubleClick = WTWDoubleClickAction.Scan;

        public readonly TidySettings Tidyup = new TidySettings();
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
        public bool DefShowDoRenaming = true;
        public bool DefShowDoMissingCheck = true;
        public bool DefShowSequentialMatching = false;
        public bool DefShowAirDateMatching = false;
        public bool DefShowEpNameMatching = false;
        public bool DefShowSpecialsCount = false;
        public bool DefShowAutoFolders = true;
        public bool DefShowUseDefLocation = false;
        public string DefShowLocation;
        public string? DefaultShowTimezoneName;
        public bool DefShowUseBase = false;
        public bool DefShowUseSubFolders = true;

        public bool DefMovieDoRenaming = true;
        public bool DefMovieDoMissingCheck = true;
        public bool DefMovieUseutomaticFolders = true;
        public bool DefMovieUseDefaultLocation = true;
        public bool SuppressUpdateAvailablePopup = false;
        public UpdateCheckMode UpdateCheckType = UpdateCheckMode.Everytime;
        internal TimeSpan UpdateCheckInterval = TimeSpan.FromDays(1);

        public string? DefMovieDefaultLocation;
        public TVDoc.ProviderType DefaultMovieProvider = TVDoc.ProviderType.TMDB;

        private TVSettings()
        {
            // defaults that aren't handled with default initialisers
            Ignore = new List<IgnoreItem>();
            PreviouslySeenEpisodes = new PreviouslySeenEpisodes();
            PreviouslySeenMovies = new PreviouslySeenMovies();
            DownloadFolders = new List<string>();
            MovieLibraryFolders = new List<string>();
            IgnoreFolders = new List<string>();
            LibraryFolders = new List<string>();
            MovieLibraryFolders = new List<string>();
            IgnoredAutoAddHints = new List<string>();

            VideoExtensionsString = VideoExtensionsStringDEFAULT;
            OtherExtensionsString = OtherExtensionsStringDEFAULT;
            keepTogetherExtensionsString = keepTogetherExtensionsStringDEFAULT;
            subtitleExtensionsString = subtitleExtensionsStringDEFAULT;

            DefShowLocation = string.Empty;

            // have a guess at utorrent's path
            string[] guesses = new string[3];
            guesses[0] = System.Windows.Forms.Application.StartupPath + "\\..\\uTorrent\\uTorrent.exe";
            guesses[1] = "c:\\Program Files\\uTorrent\\uTorrent.exe";
            guesses[2] = "c:\\Program Files (x86)\\uTorrent\\uTorrent.exe";

            uTorrentPath = string.Empty;
            foreach (FileInfo f in guesses.Select(g => new FileInfo(g)).Where(f => f.Exists))
            {
                uTorrentPath = f.FullName;
                break;
            }

            // ResumeDatPath
            FileInfo f2 =
                new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"uTorrent\resume.dat"));

            ResumeDatPath = f2.Exists ? f2.FullName : "";
        }

        // ReSharper disable once FunctionComplexityOverflow
        public void WriteXML([NotNull] XmlWriter writer)
        {
            writer.WriteStartElement("Settings");
            writer.WriteElement("UseColoursOnWtw", UseColoursOnWtw);
            writer.WriteElement("BGDownload", BGDownload);
            writer.WriteElement("OfflineMode", OfflineMode);
            writer.WriteElement("ShowBasicShowDetails", ShowBasicShowDetails);
            writer.WriteElement("DetailedRSSJSONLogging", DetailedRSSJSONLogging);
            writer.WriteElement("RSSUseCloudflare", RSSUseCloudflare);
            writer.WriteElement("SearchJSONUseCloudflare", SearchJSONUseCloudflare);
            writer.WriteElement("qBitTorrentDownloadFilesFirst", qBitTorrentDownloadFilesFirst);
            writer.WriteElement("ReplaceWithBetterQuality", ReplaceWithBetterQuality);
            writer.WriteElement("ForceSystemToDecideOnUpgradedFiles", ForceSystemToDecideOnUpgradedFiles);
            writer.WriteElement("ExportWTWRSS", ExportWTWRSS);
            writer.WriteElement("ExportWTWRSSTo", ExportWTWRSSTo);
            writer.WriteElement("ExportWTWICAL", ExportWTWICAL);
            writer.WriteElement("ExportWTWICALTo", ExportWTWICALTo);
            writer.WriteElement("ExportWTWXML", ExportWTWXML);
            writer.WriteElement("ExportWTWXMLTo", ExportWTWXMLTo);
            writer.WriteElement("ExportWTWTXT", ExportWTWTXT);
            writer.WriteElement("ExportWTWTXTTo", ExportWTWTXTTo);
            writer.WriteElement("WTWRecentDays", WTWRecentDays);
            writer.WriteElement("ExportMissingXML", ExportMissingXML);
            writer.WriteElement("ExportMissingXMLTo", ExportMissingXMLTo);
            writer.WriteElement("ExportMissingCSV", ExportMissingCSV);
            writer.WriteElement("ExportMissingCSVTo", ExportMissingCSVTo);
            writer.WriteElement("ExportRenamingXML", ExportRenamingXML);
            writer.WriteElement("ExportRenamingXMLTo", ExportRenamingXMLTo);
            writer.WriteElement("ExportRecentM3U", ExportRecentM3U);
            writer.WriteElement("ExportRecentM3UTo", ExportRecentM3UTo);
            writer.WriteElement("ExportRecentASX", ExportRecentASX);
            writer.WriteElement("ExportRecentASXTo", ExportRecentASXTo);
            writer.WriteElement("ExportRecentWPL", ExportRecentWPL);
            writer.WriteElement("ExportRecentWPLTo", ExportRecentWPLTo);
            writer.WriteElement("ExportRecentXSPF", ExportRecentXSPF);
            writer.WriteElement("ExportRecentXSPFTo", ExportRecentXSPFTo);
            writer.WriteElement("ExportShowsTXT", ExportShowsTXT);
            writer.WriteElement("ExportShowsTXTTo", ExportShowsTXTTo);
            writer.WriteElement("ExportShowsHTML", ExportShowsHTML);
            writer.WriteElement("ExportShowsHTMLTo", ExportShowsHTMLTo);
            writer.WriteElement("ExportFOXML", ExportFOXML);
            writer.WriteElement("ExportFOXMLTo", ExportFOXMLTo);

            writer.WriteElement("ExportMoviesTXT", ExportMoviesTXT);
            writer.WriteElement("ExportMoviesTXTTo", ExportMoviesTXTTo);
            writer.WriteElement("ExportMoviesHTML", ExportMoviesHTML);
            writer.WriteElement("ExportMoviesHTMLTo", ExportMoviesHTMLTo);
            writer.WriteElement("ExportMissingMoviesXML", ExportMissingMoviesXML);
            writer.WriteElement("ExportMissingMoviesXMLTo", ExportMissingMoviesXMLTo);
            writer.WriteElement("ExportMissingMoviesCSV", ExportMissingMoviesCSV);
            writer.WriteElement("ExportMissingMoviesCSVTo", ExportMissingMoviesCSVTo);

            writer.WriteElement("StartupTab2", TabNameForNumber(StartupTab));
            writer.WriteElement("NamingStyle", NamingStyle.StyleString);
            writer.WriteElement("NotificationAreaIcon", NotificationAreaIcon);
            writer.WriteElement("VideoExtensions", VideoExtensionsString);
            writer.WriteElement("OtherExtensions", OtherExtensionsString);
            writer.WriteElement("SubtitleExtensions", subtitleExtensionsString);
            writer.WriteElement("ExportRSSMaxDays", ExportRSSMaxDays);
            writer.WriteElement("ExportRSSMaxShows", ExportRSSMaxShows);
            writer.WriteElement("ExportRSSDaysPast", ExportRSSDaysPast);
            writer.WriteElement("KeepTogether", KeepTogether);
            writer.WriteElement("KeepTogetherType", (int) keepTogetherMode);
            writer.WriteElement("UIScanType", (int) UIScanType  );
            writer.WriteElement("KeepTogetherExtensions", keepTogetherExtensionsString);
            writer.WriteElement("LeadingZeroOnSeason", LeadingZeroOnSeason);
            writer.WriteElement("ShowInTaskbar", ShowInTaskbar);
            writer.WriteElement("ShowAccessibilityOptions",ShowAccessibilityOptions);
            writer.WriteElement("IgnoreSamples", IgnoreSamples);
            writer.WriteElement("ForceLowercaseFilenames", ForceLowercaseFilenames);
            writer.WriteElement("RenameTxtToSub", RenameTxtToSub);
            writer.WriteElement("ParallelDownloads", ParallelDownloads);
            writer.WriteElement("AutoSelectShowInMyShows", AutoSelectShowInMyShows);
            writer.WriteElement("AutoCreateFolders", AutoCreateFolders);
            writer.WriteElement("ShowEpisodePictures", ShowEpisodePictures);
            writer.WriteElement("HideWtWSpoilers", HideWtWSpoilers);
            writer.WriteElement("HideMyShowsSpoilers", HideMyShowsSpoilers);
            writer.WriteElement("SpecialsFolderName", SpecialsFolderName);
            writer.WriteElement("SeasonFolderFormat", SeasonFolderFormat);
            writer.WriteElement("MovieFolderFormat", MovieFolderFormat);
            writer.WriteElement("MovieFilenameFormat", MovieFilenameFormat);
            writer.WriteElement("uTorrentPath", uTorrentPath);
            writer.WriteElement("ResumeDatPath", ResumeDatPath);
            writer.WriteElement("SearchRSS", SearchRSS);
            writer.WriteElement("SearchJSON", SearchJSON);
            writer.WriteElement("SearchJSONManualScanOnly", SearchJSONManualScanOnly);
            writer.WriteElement("SearchRSSManualScanOnly", SearchRSSManualScanOnly);
            writer.WriteElement("EpImgs", EpTBNs);
            writer.WriteElement("NFOShows", NFOShows);
            writer.WriteElement("NFOEpisodes", NFOEpisodes);
            writer.WriteElement("NFOMovies", NFOMovies); 
            writer.WriteElement("KODIImages", KODIImages);
            writer.WriteElement("pyTivoMeta", pyTivoMeta);
            writer.WriteElement("pyTivoMetaSubFolder", pyTivoMetaSubFolder);
            writer.WriteElement("wdLiveTvMeta", wdLiveTvMeta);
            writer.WriteElement("FolderJpg", FolderJpg);
            writer.WriteElement("FolderJpgIs", (int) FolderJpgIs);
            writer.WriteElement("MonitoredFoldersScanType", (int) MonitoredFoldersScanType);
            writer.WriteElement("DefaultProvider", (int)DefaultProvider);
            writer.WriteElement("CheckuTorrent", CheckuTorrent);
            writer.WriteElement("RemoveCompletedTorrents", RemoveCompletedTorrents);
            writer.WriteElement("CheckqBitTorrent", CheckqBitTorrent);
            writer.WriteElement("qBitTorrentHost", qBitTorrentHost);
            writer.WriteElement("qBitTorrentPort", qBitTorrentPort);
            writer.WriteElement("qBitTorrentAPIVersion", (int)qBitTorrentAPIVersion);
            writer.WriteElement("RenameCheck", RenameCheck);
            writer.WriteElement("PreventMove", PreventMove);
            writer.WriteElement("MissingCheck", MissingCheck);
            writer.WriteElement("MoveLibraryFiles", MoveLibraryFiles);
            writer.WriteElement("AutoSearchForDownloadedFiles", AutoSearchForDownloadedFiles);
            writer.WriteElement("UpdateFileDates", CorrectFileDates);
            writer.WriteElement("SearchLocally", SearchLocally);
            writer.WriteElement("IgnorePreviouslySeen", IgnorePreviouslySeen);
            writer.WriteElement("LeaveOriginals", LeaveOriginals);
            writer.WriteElement("RetainLanguageSpecificSubtitles", RetainLanguageSpecificSubtitles);
            writer.WriteElement("ForceBulkAddToUseSettingsOnly", ForceBulkAddToUseSettingsOnly);
            writer.WriteElement("AutoMergeEpisodes", AutoMergeDownloadEpisodes);
            writer.WriteElement("AutoMergeLibraryEpisodes", AutoMergeLibraryEpisodes);
            writer.WriteElement("MonitorFolders", MonitorFolders);
            writer.WriteElement("StartupScan", runStartupCheck);
            writer.WriteElement("PeriodicScan", runPeriodicCheck);
            writer.WriteElement("PeriodicScanHours", periodCheckHours);
            writer.WriteElement("PeriodicUpdateCacheHours", periodUpdateCacheHours);
            writer.WriteElement("RemoveDownloadDirectoriesFiles", RemoveDownloadDirectoriesFiles);
            writer.WriteElement("RemoveDownloadDirectoriesFilesMatchMovies", RemoveDownloadDirectoriesFilesMatchMovies);
            writer.WriteElement("RemoveDownloadDirectoriesFilesMatchMoviesLengthCheck", RemoveDownloadDirectoriesFilesMatchMoviesLengthCheck);
            writer.WriteElement("RemoveDownloadDirectoriesFilesMatchMoviesLengthCheckLength", RemoveDownloadDirectoriesFilesMatchMoviesLengthCheckLength);
            writer.WriteElement("DoBulkAddInScan", DoBulkAddInScan);
            writer.WriteElement("DeleteShowFromDisk", DeleteShowFromDisk);
            writer.WriteElement("SABAPIKey", SABAPIKey);
            writer.WriteElement("CheckSABnzbd", CheckSABnzbd);
            writer.WriteElement("SABHostPort", SABHostPort);
            writer.WriteElement("PreferredLanguage", PreferredLanguageCode);
            writer.WriteElement("WTWDoubleClick", (int) WTWDoubleClick);
            writer.WriteElement("EpJPGs", EpJPGs);
            writer.WriteElement("SeriesJpg", SeriesJpg);
            writer.WriteElement("Mede8erXML", Mede8erXML);
            writer.WriteElement("ShrinkLargeMede8erImages", ShrinkLargeMede8erImages);
            writer.WriteElement("FanArtJpg", FanArtJpg);
            writer.WriteElement("DeleteEmpty", Tidyup.DeleteEmpty);
            writer.WriteElement("DeleteEmptyIsRecycle", Tidyup.DeleteEmptyIsRecycle);
            writer.WriteElement("EmptyIgnoreWords", Tidyup.EmptyIgnoreWords);
            writer.WriteElement("EmptyIgnoreWordList", Tidyup.EmptyIgnoreWordList);
            writer.WriteElement("EmptyIgnoreExtensions", Tidyup.EmptyIgnoreExtensions);
            writer.WriteElement("EmptyIgnoreExtensionList", Tidyup.EmptyIgnoreExtensionList);
            writer.WriteElement("EmptyMaxSizeCheck", Tidyup.EmptyMaxSizeCheck);
            writer.WriteElement("EmptyMaxSizeMB", Tidyup.EmptyMaxSizeMB);
            writer.WriteElement("BetaMode", (int) mode);
            writer.WriteElement("PercentDirtyUpgrade", upgradeDirtyPercent);
            writer.WriteElement("PercentBetter", replaceMargin);
            writer.WriteElement("BaseSeasonName", defaultSeasonWord);
            writer.WriteElement("SearchSeasonNames", searchSeasonWordsString);
            writer.WriteElement("PreferredRSSSearchTerms", preferredRSSSearchTermsString);
            writer.WriteElement("BulkAddIgnoreRecycleBin", BulkAddIgnoreRecycleBin);
            writer.WriteElement("BulkAddCompareNoVideoFolders", BulkAddCompareNoVideoFolders);
            writer.WriteElement("AutoAddMovieTerms", AutoAddMovieTerms);
            writer.WriteElement("AutoAddIgnoreSuffixes", AutoAddIgnoreSuffixes);
            writer.WriteElement("SearchJSONURL", SearchJSONURL);
            writer.WriteElement("SearchJSONRootNode", SearchJSONRootNode);
            writer.WriteElement("SearchJSONFilenameToken", SearchJSONFilenameToken);
            writer.WriteElement("SearchJSONURLToken", SearchJSONURLToken);
            writer.WriteElement("SearchJSONFileSizeToken", SearchJSONFileSizeToken);
            writer.WriteElement("SearchJSONSeedersToken", SearchJSONSeedersToken);
            writer.WriteElement("PriorityReplaceTerms", PriorityReplaceTerms);
            writer.WriteElement("CopyFutureDatedEpsFromSearchFolders", CopyFutureDatedEpsFromSearchFolders);
            writer.WriteElement("ShareLogs", ShareLogs);
            writer.WriteElement("PostpendThe", PostpendThe);
            writer.WriteElement("IgnoreAllSpecials", IgnoreAllSpecials);
            writer.WriteElement("UseFullPathNameToMatchLibraryFolders", UseFullPathNameToMatchLibraryFolders);
            writer.WriteElement("UseFullPathNameToMatchSearchFolders", UseFullPathNameToMatchSearchFolders);
            writer.WriteElement("AutoAddAsPartOfQuickRename", AutoAddAsPartOfQuickRename);
            writer.WriteElement("CleanLibraryAfterActions", CleanLibraryAfterActions);

            writer.WriteElement("DefShowIncludeNoAirdate", DefShowIncludeNoAirdate);
            writer.WriteElement("DefShowIncludeFuture", DefShowIncludeFuture);
            writer.WriteElement("DefShowNextAirdate", DefShowNextAirdate);
            writer.WriteElement("DefShowDVDOrder", DefShowDVDOrder);
            writer.WriteElement("DefShowDoRenaming", DefShowDoRenaming);
            writer.WriteElement("DefShowDoMissingCheck", DefShowDoMissingCheck);
            writer.WriteElement("DefShowSequentialMatching", DefShowSequentialMatching);
            writer.WriteElement("DefShowAirDateMatching", DefShowAirDateMatching);
            writer.WriteElement("DefShowEpNameMatching", DefShowEpNameMatching);
            writer.WriteElement("DefShowSpecialsCount", DefShowSpecialsCount);
            writer.WriteElement("DefShowAutoFolders", DefShowAutoFolders);
            writer.WriteElement("DefShowUseDefLocation", DefShowUseDefLocation);
            writer.WriteElement("DefShowLocation", DefShowLocation);
            writer.WriteElement("DefaultShowTimezoneName", DefaultShowTimezoneName);
            writer.WriteElement("DefShowUseBase", DefShowUseBase);
            writer.WriteElement("DefShowUseSubFolders", DefShowUseSubFolders);
            writer.WriteElement("SampleFileMaxSizeMB", SampleFileMaxSizeMB);

            writer.WriteElement("DefMovieDoRenaming", DefMovieDoRenaming);
            writer.WriteElement("DefMovieDoMissingCheck", DefMovieDoMissingCheck);
            writer.WriteElement("DefMovieUseutomaticFolders", DefMovieUseutomaticFolders);
            writer.WriteElement("DefMovieUseDefaultLocation", DefMovieUseDefaultLocation);
            writer.WriteElement("DefMovieDefaultLocation", DefMovieDefaultLocation);
            writer.WriteElement("DefaultMovieProvider", (int)DefaultMovieProvider);

            writer.WriteElement("UnattendedMultiActionOutcome",(int)UnattendedMultiActionOutcome );
            writer.WriteElement("UserMultiActionOutcome",(int)UserMultiActionOutcome );

            writer.WriteElement("SearchJackett",SearchJackett );
            writer.WriteElement("UseJackettTextSearch", UseJackettTextSearch);
            writer.WriteElement("SearchJackettManualScanOnly",SearchJackettManualScanOnly);
            writer.WriteElement("JackettServer",JackettServer );
            writer.WriteElement("JackettPort",JackettPort);
            writer.WriteElement("JackettIndexer",JackettIndexer );
            writer.WriteElement("JackettAPIKey",JackettAPIKey );
            writer.WriteElement("SearchJackettButton", SearchJackettButton); 
            writer.WriteElement("StopJackettSearchOnFullScan", StopJackettSearchOnFullScan);
            writer.WriteElement("StopJackettSearchOnFullScan", StopJackettSearchOnFullScan);
            writer.WriteElement("AutoSaveOnExit", AutoSaveOnExit);

            writer.WriteElement("TMDBLanguage", TMDBLanguage);
            writer.WriteElement("TMDBRegion", TMDBRegion);
            writer.WriteElement("TMDBPercentDirty", TMDBPercentDirty);
            writer.WriteElement("IncludeMoviesQuickRecent", IncludeMoviesQuickRecent);

            WriteAppUpdateElement(writer);

            TheSearchers.WriteXml(writer, "TheSearchers");
            TheMovieSearchers.WriteXml(writer, "TheMovieSearchers");
            WriteReplacements(writer);
            WriteRegExs(writer);
            XmlHelper.WriteStringsToXml(RSSURLs,writer,"RSSURLs", "URL");
            WriteShowStatusColours(writer);
            WriteFilters(writer);

            writer.WriteEndElement(); // settings
        }

        private void WriteAppUpdateElement(XmlWriter writer)
        {
            writer.WriteStartElement("AppUpdate");
            writer.WriteElement("Mode", (int)UpdateCheckType);
            writer.WriteElement("Interval", UpdateCheckInterval.ToString());
            writer.WriteElement("SuppressPopup", SuppressUpdateAvailablePopup);
            writer.WriteEndElement();
        }

        private void WriteReplacements([NotNull] XmlWriter writer)
        {
            writer.WriteStartElement("Replacements");
            foreach (Replacement R in Replacements)
            {
                writer.WriteStartElement("Replace");
                writer.WriteAttributeToXml("This", R.This);
                writer.WriteAttributeToXml("That", R.That);
                writer.WriteAttributeToXml("CaseInsensitive", R.CaseInsensitive ? "Y" : "N");
                writer.WriteEndElement(); //Replace
            }

            writer.WriteEndElement(); //Replacements
        }

        private void WriteRegExs([NotNull] XmlWriter writer)
        {
            writer.WriteStartElement("FNPRegexs");
            foreach (FilenameProcessorRE re in FNPRegexs)
            {
                writer.WriteStartElement("Regex");
                writer.WriteAttributeToXml("Enabled", re.Enabled);
                writer.WriteAttributeToXml("RE", re.RegExpression);
                writer.WriteAttributeToXml("UseFullPath", re.UseFullPath);
                writer.WriteAttributeToXml("Notes", re.Notes);
                writer.WriteEndElement(); // Regex
            }

            writer.WriteEndElement(); // FNPRegexs
        }

        private void WriteFilters(XmlWriter writer)
        {
            writer.WriteStartElement("ShowFilters");

            writer.WriteInfo("NameFilter", "Name", Filter.ShowName);
            writer.WriteInfo("ShowStatusFilter", "ShowStatus", Filter.ShowStatus);
            writer.WriteInfo("ShowNetworkFilter", "ShowNetwork", Filter.ShowNetwork);
            writer.WriteInfo("ShowRatingFilter", "ShowRating", Filter.ShowRating);

            writer.WriteInfo("ShowStatusFilter", "ShowStatusInclude", Filter.ShowStatusInclude);
            writer.WriteInfo("ShowNetworkFilter", "ShowNetworkInclude", Filter.ShowNetworkInclude);
            writer.WriteInfo("ShowRatingFilter", "ShowRatingInclude", Filter.ShowRatingInclude);

            foreach (string genre in Filter.Genres)
            {
                writer.WriteInfo("GenreFilter", "Genre", genre);
            }

            writer.WriteEndElement(); //ShowFilters

            writer.WriteStartElement("MovieFilters");

            writer.WriteInfo("NameFilter", "Name", MovieFilter.ShowName);
            writer.WriteInfo("ShowStatusFilter", "ShowStatus", MovieFilter.ShowStatus);
            writer.WriteInfo("ShowNetworkFilter", "ShowNetwork", MovieFilter.ShowNetwork);
            writer.WriteInfo("ShowRatingFilter", "ShowRating", MovieFilter.ShowRating);
            writer.WriteInfo("ShowYearFilter", "ShowYear", MovieFilter.ShowYear);

            writer.WriteInfo("ShowStatusFilter", "ShowStatusInclude", MovieFilter.ShowStatusInclude);
            writer.WriteInfo("ShowNetworkFilter", "ShowNetworkInclude", MovieFilter.ShowNetworkInclude);
            writer.WriteInfo("ShowRatingFilter", "ShowRatingInclude", MovieFilter.ShowRatingInclude);
            writer.WriteInfo("ShowYearFilter", "ShowYearInclude", MovieFilter.ShowYearInclude);

            foreach (string genre in MovieFilter.Genres)
            {
                writer.WriteInfo("GenreFilter", "Genre", genre);
            }

            writer.WriteEndElement(); //MovieFilters

            writer.WriteStartElement("SeasonFilters");
            writer.WriteElement("SeasonIgnoredFilter", SeasonFilter.HideIgnoredSeasons);
            writer.WriteEndElement(); //SeasonFilters
        }

        private void WriteShowStatusColours(XmlWriter writer)
        {
            writer.WriteStartElement("ShowStatusTVWColors");
            foreach (KeyValuePair<ColouringRule, System.Drawing.Color> e in ShowStatusColors)
            {
                writer.WriteStartElement("ShowStatusTVWColor");

                switch (e.Key)
                {
                    case ShowStatusColouringRule sscr:
                        writer.WriteAttributeToXml("Type", "ShowStatusColouringRule");
                        writer.WriteAttributeToXml("ShowStatus", sscr.status);
                        break;
                    case ShowAirStatusColouringRule sascr:
                        writer.WriteAttributeToXml("Type", "ShowAirStatusColouringRule");
                        writer.WriteAttributeToXml("ShowStatus", (int)sascr.status);
                        break;
                    case SeasonStatusColouringRule ssascr:
                        writer.WriteAttributeToXml("Type", "SeasonStatusColouringRule");
                        writer.WriteAttributeToXml("ShowStatus", (int)ssascr.status);
                        break;
                }

                writer.WriteAttributeToXml("Color", Helpers.TranslateColorToHtml(e.Value));

                writer.WriteEndElement(); //ShowStatusTVWColor
            }

            writer.WriteEndElement(); // ShowStatusTVWColors
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

        public static bool OKExtensionsString(string? s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return true;
            }

            string[] t = s.Split(';');
            foreach (string s2 in t)
            {
                if (string.IsNullOrEmpty(s2) || !s2.StartsWith(".", StringComparison.Ordinal) || s2.ContainsAnyCharactersFrom(CompulsoryReplacements()) || s2.ContainsAnyCharactersFrom(Path.GetInvalidFileNameChars()))
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

        public static bool OKPath([NotNull] string s,bool EmptyOK)
        {
            if (!EmptyOK && string.IsNullOrEmpty(s))
            {
                return false;
            }

            if (s.IsWebLink())
            {
                return false;
            }

            return s.IsValidDirectory();
        }

        [NotNull]
        public static string CompulsoryReplacements() => "*?<>:/\\|\""; // invalid filename characters, must be in the list!
        
        [NotNull]
        public static List<FilenameProcessorRE> DefaultFNPList()
        {
            // Default list of filename processors

            List<FilenameProcessorRE> l = new List<FilenameProcessorRE>
            {
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
                    "\\b(episode|ep|e) ?(?<e>[0-9]{2,}) ?- ?(cachedSeries|season) ?(?<s>[0-9]+)",
                    false, "episode 3 - season 4"),
                new FilenameProcessorRE(true,
                    "season (?<s>[0-9]+)\\\\e?(?<e>[0-9]{1,3}) ?-",
                    true, "Show Season 3\\E23 - Epname"),
                new FilenameProcessorRE(false,
                    "season (?<s>[0-9]+)\\\\episode (?<e>[0-9]{1,3})",
                    true, "Season 3\\Episode 23")
            };

            return l;
        }

        [NotNull]
        private static List<Replacement> DefaultListRE()
        {
            return new List<Replacement>
            {
                new Replacement("*", "#", false),
                new Replacement("?", "", false),
                new Replacement(">", "", false),
                new Replacement("<", "", false),
                new Replacement(":", "-", false),
                new Replacement("/", "-", false),
                new Replacement("\\", "-", false),
                new Replacement("|", "-", false),
                new Replacement("\"", "'", false)
            };
        }

        [NotNull]
        private static List<string> DefaultRSSURLList()
        {
            List<string> sl = new List<string>();
            return sl;
        }

        [NotNull]
        private static string[] TabNames() => new[] {"MyShows", "Scan", "WTW"};

        private static string TabNameForNumber(int n)
        {
            if (n >= 0 && n < TabNames().Length)
            {
                return TabNames()[n];
            }

            return "";
        }

        private static int TabNumberFromName(string? n)
        {
            int r = 0;
            if (!string.IsNullOrEmpty(n))
            {
                r = Array.IndexOf(TabNames(), n);
            }

            if (r < 0)
            {
                r = 0;
            }

            return r;
        }

        public bool FileHasUsefulExtension([NotNull] FileInfo file, bool otherExtensionsToo) =>
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

        [NotNull]
        public string FileHasUsefulExtensionDetails(FileInfo file, bool otherExtensionsToo)
        {
            foreach (string s in VideoExtensionsArray
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Where(s => file.Name.EndsWith(s, StringComparison.InvariantCultureIgnoreCase)))
            {
                return s;
            }

            if (otherExtensionsToo)
            {
                foreach (string s in OtherExtensionsArray
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Where(s => file.Name.EndsWith(s, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return s;
                }
            }

            return string.Empty;
        }

        [NotNull]
        public string BTSearchURL(ProcessedEpisode? epi)
        {
            CachedSeriesInfo s = epi?.TheCachedSeries;
            if (s is null)
            {
                return string.Empty;
            }

            string url = epi.Show.UseCustomSearchUrl && epi.Show.CustomSearchUrl.HasValue()
                ? epi.Show.CustomSearchUrl
                : TheSearchers.CurrentSearch.Url;

            return !url.HasValue() ? string.Empty : CustomEpisodeName.NameForNoExt(epi, url, true);
        }

        public string BTMovieSearchURL(MovieConfiguration? mov)
        {
            if (mov is null)
            {
                return string.Empty;
            }

            string url = TheMovieSearchers.CurrentSearch.Url;

            return !url.HasValue() ? string.Empty : CustomMovieName.NameFor(mov, url, true,false);
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
                fn = fn.RemoveCharactersFrom(Path.GetInvalidFileNameChars()).RemoveCharactersFrom("\t".ToCharArray());
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
                fn = fn.RemoveCharactersFrom(Path.GetInvalidPathChars()).RemoveCharactersFrom("\t".ToCharArray());
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

            switch (keepTogetherMode)
            {
                case KeepTogetherModes.All: return true;
                case KeepTogetherModes.Just: return keepTogetherExtensionsArray.Contains(fileExtension);
                case KeepTogetherModes.AllBut: return !keepTogetherExtensionsArray.Contains(fileExtension);
                default:
                    throw new ArgumentOutOfRangeException();
            }
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

            [NotNull]
            public IEnumerable<string> EmptyIgnoreExtensionsArray => Convert(EmptyIgnoreExtensionList);
            [NotNull]
            public IEnumerable<string> EmptyIgnoreWordsArray => Convert(EmptyIgnoreWordList);

            public void load([NotNull] XElement xmlSettings)
            {
                DeleteEmpty = xmlSettings.ExtractBool("DeleteEmpty",false);
                DeleteEmptyIsRecycle = xmlSettings.ExtractBool("DeleteEmptyIsRecycle",true);
                EmptyIgnoreWords = xmlSettings.ExtractBool("EmptyIgnoreWords",false);
                EmptyIgnoreWordList = xmlSettings.ExtractString("EmptyIgnoreWordList","sample");
                EmptyIgnoreExtensions = xmlSettings.ExtractBool("EmptyIgnoreExtensions",false);
                EmptyIgnoreExtensionList = xmlSettings.ExtractString("EmptyIgnoreExtensionList", ".nzb;.nfo;.par2;.txt;.srt");
                EmptyMaxSizeCheck = xmlSettings.ExtractBool("EmptyMaxSizeCheck",true);
                EmptyMaxSizeMB = xmlSettings.ExtractInt("EmptyMaxSizeMB", 100);
            }
        }

        public class Replacement
        {
            // used for invalid (and general) character (and string) replacements in file names

            public readonly bool CaseInsensitive;
            public readonly string That;
            public readonly string This;

            public Replacement(string from, string? to, bool insens)
            {
                This = from;
                That = to ??string.Empty;
                CaseInsensitive = insens;
            }

            [NotNull]
            public string DoReplace([NotNull] string fn)
            {
                return CaseInsensitive ? Regex.Replace(fn, Regex.Escape(This), Regex.Escape(That), RegexOptions.IgnoreCase) : fn.Replace(This, That);
            }
        }

        public class FilenameProcessorRE
        {
            // A regular expression to find the season and episode number in a filename

            public readonly bool Enabled;
            public readonly string Notes;
            public readonly string RegExpression;
            public readonly bool UseFullPath;

            public FilenameProcessorRE(bool enabled, string re, bool useFullPath, string notes)
            {
                Enabled = enabled;
                RegExpression = re;
                UseFullPath = useFullPath;
                Notes = notes;
            }
        }

        [Serializable]
        public class ShowStatusColoringTypeList : Dictionary<ColouringRule, System.Drawing.Color>
        {
            public ShowStatusColoringTypeList()
            {
            }

            protected ShowStatusColoringTypeList(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }

            public bool AppliesTo(ShowConfiguration show) => Keys.Any(rule => rule.appliesTo(show));

            public System.Drawing.Color GetColour(ShowConfiguration show)
            {
                foreach (KeyValuePair<ColouringRule, System.Drawing.Color> e in this)
                {
                    if (e.Key.appliesTo(show))
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
                    if (e.Key.appliesTo(s))
                    {
                        return e.Value;
                    }
                }

                return System.Drawing.Color.Empty;
            }

            public bool AppliesTo(ProcessedSeason s) => Keys.Any(rule => rule.appliesTo(s));
        }

        public abstract class ColouringRule
        {
            // Text is a read-only property - only a get accessor is needed:
            public abstract string Text
            {
                get;
            }
            public abstract bool appliesTo(ProcessedSeason s);
            public abstract bool appliesTo(ShowConfiguration s);
        }

        public class ShowStatusColouringRule : ColouringRule
        {
            public ShowStatusColouringRule(string status)
            {
                this.status = status;
            }

            public readonly string status;
            public override string Text => "Show Status: "+status;

            public override bool appliesTo(ProcessedSeason s) => false;

            public override bool appliesTo(ShowConfiguration s) => status==s.ShowStatus;
        }

        public class ShowAirStatusColouringRule : ColouringRule
        {
            public ShowAirStatusColouringRule(ShowConfiguration.ShowAirStatus status)
            {
                this.status = status;
            }

            public readonly ShowConfiguration.ShowAirStatus status;
            public override string ToString()
            {
                switch (status)
                {
                    case ShowConfiguration.ShowAirStatus.aired:
                        return "Show Season Status: All aired";
                    case ShowConfiguration.ShowAirStatus.noEpisodesOrSeasons:
                        return "Show Season Status: No Seasons or Episodes in Seasons";
                    case ShowConfiguration.ShowAirStatus.noneAired:
                        return "Show Season Status: None aired";
                    case ShowConfiguration.ShowAirStatus.partiallyAired:
                        return "Show Season Status: Partially aired";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public override string Text => ToString();

            public override bool appliesTo(ProcessedSeason s) => false;

            public override bool appliesTo(ShowConfiguration s) => status ==s.SeasonsAirStatus;
        }

        public class SeasonStatusColouringRule : ColouringRule
        {
            public SeasonStatusColouringRule(ProcessedSeason.SeasonStatus status)
            {
                this.status = status;
            }

            public readonly ProcessedSeason.SeasonStatus status;

            public override string Text => ToString();
            public override string ToString()
            {
                switch (status)
                {
                    case ProcessedSeason.SeasonStatus.aired:
                        return "Season Status: All aired";

                    case ProcessedSeason.SeasonStatus.noEpisodes:
                        return "Season Status: No Episodes";

                    case ProcessedSeason.SeasonStatus.noneAired:
                        return "Season Status: None aired";

                    case ProcessedSeason.SeasonStatus.partiallyAired:
                        return "Season Status: Partially aired";

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public override bool appliesTo(ProcessedSeason s) => status == s.Status(s.Show.GetTimeZone());

            public override bool appliesTo(ShowConfiguration s) => false;
        }

        // ReSharper disable once FunctionComplexityOverflow
        public void load([NotNull] XElement xmlSettings)
        {
            UseColoursOnWtw = xmlSettings.ExtractBool("UseColoursOnWtw", false);
            RSSUseCloudflare = xmlSettings.ExtractBool("RSSUseCloudflare", true);
            SearchJSONUseCloudflare = xmlSettings.ExtractBool("SearchJSONUseCloudflare", true);
            qBitTorrentDownloadFilesFirst = xmlSettings.ExtractBool("qBitTorrentDownloadFilesFirst", true);
            BGDownload = xmlSettings.ExtractBool("BGDownload",false);
            OfflineMode = xmlSettings.ExtractBool("OfflineMode",false);
            ShowBasicShowDetails = xmlSettings.ExtractBool("ShowBasicShowDetails",false);
            DetailedRSSJSONLogging = xmlSettings.ExtractBool("DetailedRSSJSONLogging",false);
            ReplaceWithBetterQuality = xmlSettings.ExtractBool("ReplaceWithBetterQuality",false);
            ForceSystemToDecideOnUpgradedFiles = xmlSettings.ExtractBool("ForceSystemToDecideOnUpgradedFiles", false);
            ExportWTWRSSTo = xmlSettings.ExtractString("ExportWTWRSSTo");
            ExportWTWXML = xmlSettings.ExtractBool("ExportWTWXML",false);
            ExportWTWXMLTo = xmlSettings.ExtractString("ExportWTWXMLTo");
            ExportWTWTXT = xmlSettings.ExtractBool("ExportWTWTXT", false);
            ExportWTWTXTTo = xmlSettings.ExtractString("ExportWTWTXTTo");
            ExportWTWICAL = xmlSettings.ExtractBool("ExportWTWICAL",false);
            ExportWTWICALTo = xmlSettings.ExtractString("ExportWTWICALTo");
            WTWRecentDays = xmlSettings.ExtractInt("WTWRecentDays",7);
            StartupTab = TabNumberFromName(xmlSettings.ExtractString("StartupTab2"));
            NamingStyle.StyleString = xmlSettings.ExtractStringOrNull("NamingStyle") ??
                                      CustomEpisodeName.OldNStyle(
                                          xmlSettings.ExtractInt("DefaultNamingStyle",1)); // old naming style
            NotificationAreaIcon = xmlSettings.ExtractBool("NotificationAreaIcon",false);
            VideoExtensionsString = xmlSettings.ExtractString("VideoExtensions",
                                    xmlSettings.ExtractString("GoodExtensions",VideoExtensionsStringDEFAULT));
            OtherExtensionsString = xmlSettings.ExtractString("OtherExtensions",OtherExtensionsStringDEFAULT);
            subtitleExtensionsString = xmlSettings.ExtractString("SubtitleExtensions", subtitleExtensionsStringDEFAULT);
            ExportRSSMaxDays = xmlSettings.ExtractInt("ExportRSSMaxDays",7);
            ExportRSSMaxShows = xmlSettings.ExtractInt("ExportRSSMaxShows",10);
            ExportRSSDaysPast = xmlSettings.ExtractInt("ExportRSSDaysPast",0);
            KeepTogether = xmlSettings.ExtractBool("KeepTogether",true);
            LeadingZeroOnSeason = xmlSettings.ExtractBool("LeadingZeroOnSeason",false);
            ShowAccessibilityOptions = xmlSettings.ExtractBool("ShowAccessibilityOptions", false);
            ShowInTaskbar = xmlSettings.ExtractBool("ShowInTaskbar",true);
            RenameTxtToSub = xmlSettings.ExtractBool("RenameTxtToSub",false);
            ShowEpisodePictures = xmlSettings.ExtractBool("ShowEpisodePictures",true);
            HideWtWSpoilers = xmlSettings.ExtractBool("HideWtWSpoilers",false);
            HideMyShowsSpoilers = xmlSettings.ExtractBool("HideMyShowsSpoilers",false);
            AutoCreateFolders = xmlSettings.ExtractBool("AutoCreateFolders",false);
            AutoSelectShowInMyShows = xmlSettings.ExtractBool("AutoSelectShowInMyShows",true);
            SpecialsFolderName = xmlSettings.ExtractString("SpecialsFolderName", "Specials");
            SeasonFolderFormat = xmlSettings.ExtractString("SeasonFolderFormat");
            MovieFolderFormat = xmlSettings.ExtractString("MovieFolderFormat");
            MovieFilenameFormat = xmlSettings.ExtractString("MovieFilenameFormat");
            SearchJSONURL = xmlSettings.ExtractString("SearchJSONURL", "https://eztv.ag/api/get-torrents?imdb_id=");
            SearchJSONRootNode = xmlSettings.ExtractString("SearchJSONRootNode", "torrents");
            SearchJSONFilenameToken = xmlSettings.ExtractString("SearchJSONFilenameToken", "filename");
            SearchJSONURLToken = xmlSettings.ExtractString("SearchJSONURLToken", "torrent_url");
            SearchJSONFileSizeToken = xmlSettings.ExtractString("SearchJSONFileSizeToken", "size_bytes");
            SearchJSONSeedersToken = xmlSettings.ExtractString("SearchJSONSeedersToken", "seeds");
            SABAPIKey = xmlSettings.ExtractString("SABAPIKey");
            CheckSABnzbd = xmlSettings.ExtractBool("CheckSABnzbd",false);
            SABHostPort = xmlSettings.ExtractString("SABHostPort");
            PreferredLanguageCode = xmlSettings.ExtractString("PreferredLanguage", "en");
            WTWDoubleClick = xmlSettings.ExtractEnum("WTWDoubleClick",WTWDoubleClickAction.Scan);
            ExportMissingXML = xmlSettings.ExtractBool("ExportMissingXML",false);
            ExportMissingXMLTo = xmlSettings.ExtractString("ExportMissingXMLTo");
            ExportRecentXSPF = xmlSettings.ExtractBool("ExportRecentXSPF",false);
            ExportRecentXSPFTo = xmlSettings.ExtractString("ExportRecentXSPFTo");
            ExportRecentM3U = xmlSettings.ExtractBool("ExportRecentM3U",false);
            ExportRecentM3UTo = xmlSettings.ExtractString("ExportRecentM3UTo");
            ExportRecentASX = xmlSettings.ExtractBool("ExportRecentASX",false);
            ExportRecentASXTo = xmlSettings.ExtractString("ExportRecentASXTo");
            ExportRecentWPL = xmlSettings.ExtractBool("ExportRecentWPL",false);
            ExportRecentWPLTo = xmlSettings.ExtractString("ExportRecentWPLTo");
            ExportMissingCSV = xmlSettings.ExtractBool("ExportMissingCSV",false);
            ExportMissingCSVTo = xmlSettings.ExtractString("ExportMissingCSVTo");
            ExportMoviesTXT = xmlSettings.ExtractBool("ExportMoviesTXT", false);
            ExportMoviesTXTTo = xmlSettings.ExtractString("ExportMoviesTXTTo");
            ExportMoviesHTML = xmlSettings.ExtractBool("ExportMoviesHTML", false);
            ExportMoviesHTMLTo = xmlSettings.ExtractString("ExportMoviesHTMLTo");
            ExportMissingMoviesXML = xmlSettings.ExtractBool("ExportMissingMoviesXML", false);
            ExportMissingMoviesXMLTo = xmlSettings.ExtractString("ExportMissingMoviesXMLTo");
            ExportMissingMoviesCSV = xmlSettings.ExtractBool("ExportMissingMoviesCSV", false);
            ExportMissingMoviesCSVTo = xmlSettings.ExtractString("ExportMissingMoviesCSVTo");
            ExportRenamingXML = xmlSettings.ExtractBool("ExportRenamingXML",false);
            ExportRenamingXMLTo = xmlSettings.ExtractString("ExportRenamingXMLTo");
            ExportFOXML = xmlSettings.ExtractBool("ExportFOXML",false);
            ExportFOXMLTo = xmlSettings.ExtractString("ExportFOXMLTo");
            ExportShowsTXT = xmlSettings.ExtractBool("ExportShowsTXT",false);
            ExportShowsTXTTo = xmlSettings.ExtractString("ExportShowsTXTTo");
            ExportShowsHTML = xmlSettings.ExtractBool("ExportShowsHTML",false);
            ExportShowsHTMLTo = xmlSettings.ExtractString("ExportShowsHTMLTo");
            ForceLowercaseFilenames = xmlSettings.ExtractBool("ForceLowercaseFilenames",false);
            IgnoreSamples = xmlSettings.ExtractBool("IgnoreSamples",true);
            SampleFileMaxSizeMB = xmlSettings.ExtractInt("SampleFileMaxSizeMB",50);
            ParallelDownloads = xmlSettings.ExtractInt("ParallelDownloads",4);
            uTorrentPath = xmlSettings.ExtractString("uTorrentPath");
            ResumeDatPath = xmlSettings.ExtractString("ResumeDatPath");
            SearchRSS = xmlSettings.ExtractBool("SearchRSS",false);
            SearchJSON = xmlSettings.ExtractBool("SearchJSON",false);
            SearchRSSManualScanOnly = xmlSettings.ExtractBool("SearchRSSManualScanOnly",true);
            SearchJSONManualScanOnly = xmlSettings.ExtractBool("SearchJSONManualScanOnly",true);
            EpTBNs = xmlSettings.ExtractBool("EpImgs",false);
            NFOMovies = xmlSettings.ExtractBool("NFOMovies",true);
            NFOShows = xmlSettings.ExtractBool("NFOShows") ?? xmlSettings.ExtractBool("NFOs",false);
            NFOEpisodes = xmlSettings.ExtractBool("NFOEpisodes") ?? xmlSettings.ExtractBool("NFOs",false);
            KODIImages = xmlSettings.ExtractBool("KODIImages") ??
                            xmlSettings.ExtractBool("XBMCImages",false); //Backward Compatibility
            pyTivoMeta = xmlSettings.ExtractBool("pyTivoMeta",false);
            wdLiveTvMeta = xmlSettings.ExtractBool("wdLiveTvMeta",false);
            pyTivoMetaSubFolder = xmlSettings.ExtractBool("pyTivoMetaSubFolder",false);
            FolderJpg = xmlSettings.ExtractBool("FolderJpg",false);
            FolderJpgIs = xmlSettings.ExtractEnum("FolderJpgIs", FolderJpgIsType.Poster);
            MonitoredFoldersScanType = xmlSettings.ExtractEnum("MonitoredFoldersScanType",ScanType.Full);
            DefaultProvider = xmlSettings.ExtractEnum("DefaultProvider", TVDoc.ProviderType.TheTVDB);
            RenameCheck = xmlSettings.ExtractBool("RenameCheck",true);
            PreventMove = xmlSettings.ExtractBool("PreventMove",false);
            CheckuTorrent = xmlSettings.ExtractBool("CheckuTorrent",false);
            CheckqBitTorrent = xmlSettings.ExtractBool("CheckqBitTorrent",false);
            RemoveCompletedTorrents = xmlSettings.ExtractBool("RemoveCompletedTorrents", false);
            qBitTorrentHost = xmlSettings.ExtractString("qBitTorrentHost", "localhost");
            qBitTorrentPort = xmlSettings.ExtractString("qBitTorrentPort", "8080");
            qBitTorrentAPIVersion = xmlSettings.ExtractEnum( "qBitTorrentAPIVersion", qBitTorrent.qBitTorrentAPIVersion.v2);
            MissingCheck = xmlSettings.ExtractBool("MissingCheck",true);
            MoveLibraryFiles = xmlSettings.ExtractBool("MoveLibraryFiles",true);
            CorrectFileDates = xmlSettings.ExtractBool("UpdateFileDates",false);
            SearchLocally = xmlSettings.ExtractBool("SearchLocally",true);
            IgnorePreviouslySeen = xmlSettings.ExtractBool("IgnorePreviouslySeen",false);
            LeaveOriginals = xmlSettings.ExtractBool("LeaveOriginals",false);
            AutoSearchForDownloadedFiles = xmlSettings.ExtractBool("AutoSearchForDownloadedFiles",false);
            AutoMergeDownloadEpisodes = xmlSettings.ExtractBool("AutoMergeEpisodes",false);
            AutoMergeLibraryEpisodes = xmlSettings.ExtractBool("AutoMergeLibraryEpisodes",false);
            RetainLanguageSpecificSubtitles = xmlSettings.ExtractBool("RetainLanguageSpecificSubtitles",true);
            ForceBulkAddToUseSettingsOnly = xmlSettings.ExtractBool("ForceBulkAddToUseSettingsOnly",false);
            MonitorFolders = xmlSettings.ExtractBool("MonitorFolders",false);
            runStartupCheck = xmlSettings.ExtractBool("StartupScan",false);
            runPeriodicCheck = xmlSettings.ExtractBool("PeriodicScan",false);
            periodCheckHours = xmlSettings.ExtractInt("PeriodicScanHours",1);
            periodUpdateCacheHours = xmlSettings.ExtractInt("PeriodicUpdateCacheHours", 1);
            RemoveDownloadDirectoriesFiles = xmlSettings.ExtractBool("RemoveDownloadDirectoriesFiles",false);
            RemoveDownloadDirectoriesFilesMatchMovies = xmlSettings.ExtractBool("RemoveDownloadDirectoriesFilesMatchMovies", false);
            RemoveDownloadDirectoriesFilesMatchMoviesLengthCheck = xmlSettings.ExtractBool("RemoveDownloadDirectoriesFilesMatchMoviesLengthCheck", false);
            RemoveDownloadDirectoriesFilesMatchMoviesLengthCheckLength = xmlSettings.ExtractInt("RemoveDownloadDirectoriesFilesMatchMoviesLengthCheckLength", 8);
            DoBulkAddInScan = xmlSettings.ExtractBool("DoBulkAddInScan",false);
            DeleteShowFromDisk = xmlSettings.ExtractBool("DeleteShowFromDisk",true);
            EpJPGs = xmlSettings.ExtractBool("EpJPGs",false);
            SeriesJpg = xmlSettings.ExtractBool("SeriesJpg",false);
            Mede8erXML = xmlSettings.ExtractBool("Mede8erXML",false);
            ShrinkLargeMede8erImages = xmlSettings.ExtractBool("ShrinkLargeMede8erImages",false);
            FanArtJpg = xmlSettings.ExtractBool("FanArtJpg",false);
            BulkAddIgnoreRecycleBin = xmlSettings.ExtractBool("BulkAddIgnoreRecycleBin",false);
            BulkAddCompareNoVideoFolders = xmlSettings.ExtractBool("BulkAddCompareNoVideoFolders",false);
            AutoAddMovieTerms = xmlSettings.ExtractString("AutoAddMovieTerms", "dvdrip;camrip;screener;dvdscr;r5;bluray");
            AutoAddIgnoreSuffixes = xmlSettings.ExtractString("AutoAddIgnoreSuffixes", "1080p;720p");
            PriorityReplaceTerms = xmlSettings.ExtractString("PriorityReplaceTerms", "PROPER;REPACK;RERIP");
            mode= xmlSettings.ExtractEnum("BetaMode", BetaMode.ProductionOnly);
            upgradeDirtyPercent = xmlSettings.ExtractFloat("PercentDirtyUpgrade",20);
            replaceMargin = xmlSettings.ExtractFloat("PercentBetter",10);
            defaultSeasonWord = xmlSettings.ExtractString("BaseSeasonName", "Season");
            searchSeasonWordsString = xmlSettings.ExtractString("SearchSeasonNames", "Season;Series;Saison;Temporada;Seizoen");
            preferredRSSSearchTermsString = xmlSettings.ExtractString("PreferredRSSSearchTerms", "720p;1080p");
            keepTogetherMode = xmlSettings.ExtractEnum("KeepTogetherType", KeepTogetherModes.All);
            UIScanType = xmlSettings.ExtractEnum("UIScanType", ScanType.Full);
            keepTogetherExtensionsString = xmlSettings.ExtractString("KeepTogetherExtensions", keepTogetherExtensionsStringDEFAULT);
            ExportWTWRSS = xmlSettings.ExtractBool("ExportWTWRSS",false);
            CopyFutureDatedEpsFromSearchFolders = xmlSettings.ExtractBool("CopyFutureDatedEpsFromSearchFolders",false);
            ShareLogs = xmlSettings.ExtractBool("ShareLogs",true);
            PostpendThe = xmlSettings.ExtractBool("PostpendThe",false);
            IgnoreAllSpecials = xmlSettings.ExtractBool("IgnoreAllSpecials",false);
            UseFullPathNameToMatchLibraryFolders = xmlSettings.ExtractBool("UseFullPathNameToMatchLibraryFolders",false);
            UseFullPathNameToMatchSearchFolders = xmlSettings.ExtractBool("UseFullPathNameToMatchSearchFolders",false);
            AutoAddAsPartOfQuickRename = xmlSettings.ExtractBool("AutoAddAsPartOfQuickRename", true);
            CleanLibraryAfterActions = xmlSettings.ExtractBool("CleanLibraryAfterActions", false);

            DefShowIncludeNoAirdate = xmlSettings.ExtractBool("DefShowIncludeNoAirdate", false);
            DefShowIncludeFuture = xmlSettings.ExtractBool("DefShowIncludeFuture", false);
            DefShowNextAirdate = xmlSettings.ExtractBool("DefShowNextAirdate", true);
            DefShowDVDOrder = xmlSettings.ExtractBool("DefShowDVDOrder", false);
            DefShowDoRenaming = xmlSettings.ExtractBool("DefShowDoRenaming", true);
            DefShowDoMissingCheck = xmlSettings.ExtractBool("DefShowDoMissingCheck", true);

            DefShowSequentialMatching = xmlSettings.ExtractBool("DefShowSequentialMatching", false);
            DefShowAirDateMatching= xmlSettings.ExtractBool("DefShowAirDateMatching", xmlSettings.ExtractBool("LookForDateInFilename", false));
            DefShowEpNameMatching = xmlSettings.ExtractBool("DefShowEpNameMatching", false);

            DefShowSpecialsCount = xmlSettings.ExtractBool("DefShowSpecialsCount", false);
            DefShowAutoFolders = xmlSettings.ExtractBool("DefShowAutoFolders", true);
            DefShowUseDefLocation = xmlSettings.ExtractBool("DefShowUseDefLocation", false);
            DefShowUseBase = xmlSettings.ExtractBool("DefShowUseBase", false);
            DefShowUseSubFolders = xmlSettings.ExtractBool("DefShowUseSubFolders", true);
            DefShowLocation = xmlSettings.ExtractString("DefShowLocation",string.Empty);
            DefaultShowTimezoneName = xmlSettings.ExtractString("DefaultShowTimezoneName");

            DefMovieDoRenaming = xmlSettings.ExtractBool("DefMovieDoRenaming", true);
            DefMovieDoMissingCheck = xmlSettings.ExtractBool("DefMovieDoMissingCheck", true);
            DefMovieUseutomaticFolders = xmlSettings.ExtractBool("DefMovieUseutomaticFolders", true);
            DefMovieUseDefaultLocation = xmlSettings.ExtractBool("DefMovieUseDefaultLocation",true);
            DefMovieDefaultLocation = xmlSettings.ExtractStringOrNull("DefMovieDefaultLocation");
            DefaultMovieProvider = xmlSettings.ExtractEnum("DefaultMovieProvider", TVDoc.ProviderType.TMDB);

            UnattendedMultiActionOutcome = xmlSettings.ExtractEnum("UnattendedMultiActionOutcome", DuplicateActionOutcome.IgnoreAll);
            UserMultiActionOutcome = xmlSettings.ExtractEnum("UserMultiActionOutcome", DuplicateActionOutcome.MostSeeders);

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

            TMDBLanguage = xmlSettings.ExtractString("TMDBLanguage");
            TMDBRegion = xmlSettings.ExtractString("TMDBRegion");
            TMDBPercentDirty = xmlSettings.ExtractFloat("TMDBPercentDirty",20);
            IncludeMoviesQuickRecent = xmlSettings.ExtractBool("IncludeMoviesQuickRecent",false);

            Tidyup.load(xmlSettings);
            RSSURLs = xmlSettings.Descendants("RSSURLs").FirstOrDefault()?.ReadStringsFromXml("URL")??new List<string>();
            TheSearchers = new Searchers(xmlSettings.Descendants("TheSearchers").FirstOrDefault(),MediaConfiguration.MediaType.tv);
            TheMovieSearchers = new Searchers(xmlSettings.Descendants("TheMovieSearchers").FirstOrDefault(),MediaConfiguration.MediaType.movie);

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
                UpdateCheckInterval = TimeSpan.Parse(subElement.ExtractString("Interval", TimeSpan.FromHours(1).ToString()));
                UpdateCheckType = (UpdateCheckMode)Enum.Parse(typeof(UpdateCheckMode), subElement.ExtractString("Mode", ((int)UpdateCheckMode.Everytime).ToString()));
                SuppressUpdateAvailablePopup = subElement.ExtractBool("SuppressPopup", false);
            }
        }

        private void UpdateFiters([NotNull] XElement xmlSettings)
        {
            SeasonFilter = new SeasonFilter
            {
                HideIgnoredSeasons = XmlConvert.ToBoolean(xmlSettings.Descendants("SeasonFilters")
                                                              .Descendants("SeasonIgnoredFilter").FirstOrDefault()?.Value ??
                                                          "false")
            };

            Filter = new ShowFilter
            {
                ShowName = xmlSettings.Descendants("ShowFilters").Descendants("ShowNameFilter").Attributes("ShowName")
                    .FirstOrDefault()?.Value,

                ShowStatus = xmlSettings.Descendants("ShowFilters").Descendants("ShowStatusFilter").Attributes("ShowStatus")
                    .FirstOrDefault()?.Value,
                ShowRating = xmlSettings.Descendants("ShowFilters").Descendants("ShowRatingFilter").Attributes("ShowRating")
                    .FirstOrDefault()?.Value,
                ShowNetwork = xmlSettings.Descendants("ShowFilters").Descendants("ShowNetworkFilter").Attributes("ShowNetwork")
                    .FirstOrDefault()?.Value,

                ShowStatusInclude = (bool?) xmlSettings.Descendants("ShowFilters").Descendants("ShowStatusFilter").Attributes("ShowStatusInclude")
                                        .FirstOrDefault() ?? true,
                ShowRatingInclude = (bool?)xmlSettings.Descendants("ShowFilters").Descendants("ShowRatingFilter").Attributes("ShowRatingInclude")
                                        .FirstOrDefault() ?? true,
                ShowNetworkInclude = (bool?)xmlSettings.Descendants("ShowFilters").Descendants("ShowNetworkFilter").Attributes("ShowNetworkInclude")
                                         .FirstOrDefault() ?? true
            };

            foreach (XAttribute rep in xmlSettings.Descendants("ShowFilters").Descendants("GenreFilter").Attributes("Genre"))
            {
                Filter.Genres.Add(rep.Value);
            }

            MovieFilter = new MovieFilter
            {
                ShowName = xmlSettings.Descendants("MovieFilter").Descendants("ShowNameFilter").Attributes("ShowName")
                    .FirstOrDefault()?.Value,

                ShowStatus = xmlSettings.Descendants("MovieFilter").Descendants("ShowStatusFilter").Attributes("ShowStatus")
                    .FirstOrDefault()?.Value,
                ShowRating = xmlSettings.Descendants("ShowFilters").Descendants("ShowRatingFilter").Attributes("ShowRating")
                    .FirstOrDefault()?.Value,
                ShowNetwork = xmlSettings.Descendants("MovieFilter").Descendants("ShowNetworkFilter").Attributes("ShowNetwork")
                    .FirstOrDefault()?.Value,
                ShowYear = xmlSettings.Descendants("MovieFilter").Descendants("ShowYearFilter").Attributes("ShowYear")
                    .FirstOrDefault()?.Value,

                ShowStatusInclude = (bool?)xmlSettings.Descendants("MovieFilter").Descendants("ShowStatusFilter").Attributes("ShowStatusInclude")
                    .FirstOrDefault() ?? true,
                ShowRatingInclude = (bool?)xmlSettings.Descendants("MovieFilter").Descendants("ShowRatingFilter").Attributes("ShowRatingInclude")
                    .FirstOrDefault() ?? true,
                ShowNetworkInclude = (bool?)xmlSettings.Descendants("MovieFilter").Descendants("ShowNetworkFilter").Attributes("ShowNetworkInclude")
                    .FirstOrDefault() ?? true,
                ShowYearInclude = (bool?)xmlSettings.Descendants("MovieFilter").Descendants("ShowYearFilter").Attributes("ShowYearInclude")
                .FirstOrDefault() ?? true
            };

            foreach (XAttribute rep in xmlSettings.Descendants("MovieFilter").Descendants("GenreFilter").Attributes("Genre"))
            {
                MovieFilter.Genres.Add(rep.Value);
            }

            if (SeasonFolderFormat == string.Empty)
            {
                //this has not been set from the XML, so we should give it an appropriate default value
                SeasonFolderFormat = defaultSeasonWord.Trim() + " " + (LeadingZeroOnSeason ? "{Season:2}" : "{Season}");
            }
        }

        private void UpdateShowStatus([NotNull] XElement xmlSettings)
        {
            ShowStatusColors = new ShowStatusColoringTypeList();
            foreach (XElement rep in xmlSettings.Descendants("ShowStatusTVWColors").FirstOrDefault()
                                         ?.Descendants("ShowStatusTVWColor") ?? new List<XElement>())
            {
                ColouringRule newRule = ExtractColouringRule(rep);
                if (newRule is null)
                {
                    continue;
                }
                string color = rep.Attribute("Color")?.Value;
                if (string.IsNullOrEmpty(color))
                {
                    continue;
                }

                System.Drawing.Color c = System.Drawing.ColorTranslator.FromHtml(color);
                ShowStatusColors.Add(newRule, c);
            }
        }

        private static ColouringRule? ExtractColouringRule([NotNull] XElement rep)
        {
            string showStatus = rep.Attribute("ShowStatus")?.Value;
            if (showStatus is null)
            {
                return null;
            }

            string type = rep.Attribute("Type")?.Value;
            if (type is null)
            {
                //Old Style Rule; lets convert
                string isMetaString = rep.Attribute("IsMeta")?.Value;
                string isShowLevelString = rep.Attribute("IsShowLevel")?.Value;
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

            switch (type)
            {
                case "SeasonStatusColouringRule":
                    return new SeasonStatusColouringRule(ExtractEnum<ProcessedSeason.SeasonStatus>(showStatus));
                case "ShowStatusColouringRule":
                    return new ShowStatusColouringRule(showStatus);
                case "ShowAirStatusColouringRule":
                    return new ShowAirStatusColouringRule(ExtractEnum<ShowConfiguration.ShowAirStatus>(showStatus));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [NotNull]
        // ReSharper disable once AnnotateNotNullParameter
        private static T ExtractEnum<T>(string value)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            if (value is null)
            {
                throw new ArgumentException();
            }

            int val = int.Parse(value);

            if (typeof(T).IsEnumDefined(val))
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }

            throw new ArgumentException();
        }

        private static ShowConfiguration.ShowAirStatus ConvertToShowAirStatus([NotNull] string value)
        {
            switch (value)
            {
                case "All aired":
                case "Aired":
                case "aired":
                    return ShowConfiguration.ShowAirStatus.aired;
                case "No Seasons or Episodes in Seasons":
                case "NoEpisodesOrSeasons":
                case "noEpisodesOrSeasons":
                    return ShowConfiguration.ShowAirStatus.noEpisodesOrSeasons;
                case "None aired":
                case "NoneAired":
                case "noneAired":
                    return ShowConfiguration.ShowAirStatus.noneAired;
                case "Partially aired":
                case "PartiallyAired":
                case "partiallyAired":
                    return ShowConfiguration.ShowAirStatus.partiallyAired;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static ProcessedSeason.SeasonStatus ConvertToSeasonStatus([NotNull] string value)
        {
            switch (value)
            {
                case "All aired":
                case "Aired":
                case "aired":
                    return ProcessedSeason.SeasonStatus.aired;
                case "No Episodes":
                case "NoEpisodes":
                case "noEpisodes":
                    return ProcessedSeason.SeasonStatus.noEpisodes;
                case "None aired":
                case "NoneAired":
                case "noneAired":
                    return ProcessedSeason.SeasonStatus.noneAired;
                case "Partially aired":
                case "PartiallyAired":
                case "partiallyAired":
                    return ProcessedSeason.SeasonStatus.partiallyAired;
            }
            throw new ArgumentOutOfRangeException();
        }

        private void UpdateRegExs([NotNull] XElement xmlSettings)
        {
            FNPRegexs.Clear();
            foreach (XElement rep in xmlSettings.Descendants("FNPRegexs").FirstOrDefault()?.Descendants("Regex") ??
                                     new List<XElement>())
            {
                string? enabledValue = rep.Attribute("Enabled")?.Value;
                string? reValue = rep.Attribute("RE")?.Value;
                string? useFullPathValue = rep.Attribute("UseFullPath")?.Value;
                string? notesValue = rep.Attribute("Notes")?.Value;

                bool enabled = !(enabledValue is null) && XmlConvert.ToBoolean(enabledValue);
                bool useFullPath = !(useFullPathValue is null) && XmlConvert.ToBoolean(useFullPathValue);

                if (reValue != null && notesValue != null)
                {
                    FNPRegexs.Add(new FilenameProcessorRE(enabled, reValue, useFullPath, notesValue));
                }
            }
        }

        private void UpdateReplacements([NotNull] XElement xmlSettings)
        {
            Replacements.Clear();
            foreach (XElement rep in xmlSettings.Descendants("Replacements").FirstOrDefault()?.Descendants("Replace") ??
                                     new List<XElement>())
            {
                string? thisValue = rep.Attribute("This")?.Value;
                string? thatValue = rep.Attribute("That")?.Value;
                string? caseInsensitiveValue = rep.Attribute("CaseInsensitive")?.Value;
                bool caseInsensitive = caseInsensitiveValue == "Y";

                if (thisValue != null  && thatValue != null)
                {
                    Replacements.Add(new Replacement(thisValue, thatValue, caseInsensitive));
                }
            }
        }
    }
}
