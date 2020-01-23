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

        private static volatile TVSettings instance;
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

        public List<string> LibraryFolders;
        public List<string> IgnoreFolders;
        public List<string> DownloadFolders;
        public List<string> IgnoredAutoAddHints;
        public List<IgnoreItem> Ignore;
        public bool AutoSelectShowInMyShows = true;
        public bool AutoCreateFolders = false;
        public bool BGDownload = false;
        public bool CheckuTorrent = false;
        public bool CheckqBitTorrent = false;
        public string qBitTorrentHost = "localhost";
        public string qBitTorrentPort = "8080";
        public qBitTorrentAPIVersion qBitTorrentAPIVersion = qBitTorrentAPIVersion.v2;
        public bool EpTBNs = false;
        public bool EpJPGs = false;
        public bool SeriesJpg = false;
        public bool ShrinkLargeMede8erImages = false;
        public bool FanArtJpg = false;
        public bool Mede8erXML = false;
        public bool ExportFOXML = false;
        public string ExportFOXMLTo = "";
        public bool ExportMissingCSV = false;
        public string ExportMissingCSVTo = "";
        public bool ExportMissingXML = false;
        public string ExportMissingXMLTo = "";
        public bool ExportShowsTXT = false;
        public string ExportShowsTXTTo = "";
        public bool ExportShowsHTML = false;
        public string ExportShowsHTMLTo = "";
        public int ExportRSSMaxDays = 7;
        public int ExportRSSMaxShows = 10;
        public int ExportRSSDaysPast = 0;
        public bool ExportRenamingXML = false;
        public string ExportRenamingXMLTo = "";
        public bool ExportWTWRSS = false;
        public string ExportWTWRSSTo = "";
        public bool ExportWTWXML = false;
        public string ExportWTWXMLTo = "";
        public bool ExportWTWICAL = false;
        public string ExportWTWICALTo = "";
        public bool ExportRecentXSPF = false;
        public string ExportRecentXSPFTo = "";
        public bool ExportRecentM3U = false;
        public string ExportRecentM3UTo = "";
        public bool ExportRecentASX = false;
        public string ExportRecentASXTo = "";
        public bool ExportRecentWPL = false;
        public string ExportRecentWPLTo = "";
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
        public bool LookForDateInFilename = false;
        public bool MissingCheck = true;
        public bool MoveLibraryFiles = true;
        public bool CorrectFileDates = false;
        public bool NFOShows = false;
        public bool NFOEpisodes = false;
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

        public BetaMode mode = BetaMode.ProductionOnly;
        public float upgradeDirtyPercent = 20;
        public float replaceMargin = 10;
        public bool ReplaceWithBetterQuality = false;
        public bool ForceSystemToDecideOnUpgradedFiles = false;
        public KeepTogetherModes keepTogetherMode = KeepTogetherModes.All;

        public bool BulkAddIgnoreRecycleBin = false;
        public bool BulkAddCompareNoVideoFolders = false;

        public string AutoAddMovieTerms = "dvdrip;camrip;screener;dvdscr;r5;bluray";
        [NotNull]
        public IEnumerable<string> AutoAddMovieTermsArray => Convert(AutoAddMovieTerms);

        public string PriorityReplaceTerms = "PROPER;REPACK;RERIP";
        [NotNull]
        public IEnumerable<string> PriorityReplaceTermsArray => Convert(PriorityReplaceTerms);

        public string AutoAddIgnoreSuffixes = "1080p;720p";
        [NotNull]
        public IEnumerable<string> AutoAddIgnoreSuffixesArray => Convert(AutoAddIgnoreSuffixes);

        public string keepTogetherExtensionsString = "";
        [NotNull]
        public IEnumerable<string> keepTogetherExtensionsArray => Convert(keepTogetherExtensionsString);

        public string subtitleExtensionsString = "";
        [NotNull]
        public IEnumerable<string> subtitleExtensionsArray => Convert(subtitleExtensionsString);

        public string searchSeasonWordsString = "Season;Series;Saison;Temporada;Seizoen";
        [NotNull]
        public IEnumerable<string> searchSeasonWordsArray => Convert(searchSeasonWordsString);

        public string preferredRSSSearchTermsString = "720p;1080p";
        [NotNull]
        public string[] PreferredRSSSearchTerms() => Convert(preferredRSSSearchTermsString);

        public string OtherExtensionsString = "";
        [NotNull]
        private IEnumerable<string> OtherExtensionsArray => Convert(OtherExtensionsString);

        [NotNull]
        private static string[] Convert([CanBeNull] string propertyString)
        {
            return string.IsNullOrWhiteSpace(propertyString) ? new string[0] : propertyString.Split(';');
        }

        internal bool IncludeBetaUpdates() => mode == BetaMode.BetaToo;

        public string defaultSeasonWord = "Season";
        public ShowFilter Filter = new ShowFilter();
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
        public bool SearchRSS = false;
        public bool SearchRSSManualScanOnly = true;
        public bool SearchJSON = false;
        public bool SearchJSONManualScanOnly = true;
        public bool ShowEpisodePictures = true;
        public bool HideWtWSpoilers = false;
        public bool HideMyShowsSpoilers = false;
        public bool ShowInTaskbar = true;
        public bool AutoSearchForDownloadedFiles = false;
        public string SpecialsFolderName = "Specials";
        public string SeasonFolderFormat = CustomSeasonName.DefaultStyle();
        public int StartupTab;
        public Searchers TheSearchers = new Searchers();

        public string SearchJSONURL= "https://eztv.ag/api/get-torrents?imdb_id=";
        public string SearchJSONRootNode = "torrents";
        public string SearchJSONFilenameToken = "filename";
        public string SearchJSONURLToken = "torrent_url";
        public string SearchJSONFileSizeToken = "size_bytes";

        [NotNull]
        public string[] VideoExtensionsArray => Convert(VideoExtensionsString);

        [NotNull]
        public string USER_AGENT =>
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36";

        public TheTVDB.PagingMethod TVDBPagingMethod => TheTVDB.PagingMethod.proper;

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
        public bool DeleteShowFromDisk = true;

        public ShowStatusColoringTypeList ShowStatusColors = new ShowStatusColoringTypeList();
        public string SABHostPort = "";
        public string SABAPIKey = "";
        public bool CheckSABnzbd = false;
        public string PreferredLanguageCode = "en";
        public WTWDoubleClickAction WTWDoubleClick = WTWDoubleClickAction.Scan;

        public readonly TidySettings Tidyup = new TidySettings();
        public bool runPeriodicCheck = false;
        public int periodCheckHours = 1;
        public bool runStartupCheck = false;
        public bool DoBulkAddInScan = false;
        public PreviouslySeenEpisodes PreviouslySeenEpisodes;
        public bool IgnoreAllSpecials = false;

        public bool DefShowIncludeNoAirdate = false;
        public bool DefShowIncludeFuture = false;
        public bool DefShowNextAirdate = true;
        public bool DefShowDVDOrder = false;
        public bool DefShowDoRenaming = true;
        public bool DefShowDoMissingCheck = true;
        public bool DefShowSequentialMatching = false;
        public bool DefShowSpecialsCount = false;
        public bool DefShowAutoFolders = true;
        public bool DefShowUseDefLocation = false;
        public string DefShowLocation;
        public string DefaultShowTimezoneName;
        public bool DefShowUseBase = false;
        public bool DefShowUseSubFolders = true;

        private TVSettings()
        {
            SetToDefaults();
        }

        private void SetToDefaults()
        {
            // defaults that aren't handled with default initialisers
            Ignore = new List<IgnoreItem>();
            PreviouslySeenEpisodes = new PreviouslySeenEpisodes();
            DownloadFolders = new List<string>();
            IgnoreFolders = new List<string>();
            LibraryFolders = new List<string>();
            IgnoredAutoAddHints = new List<string>();

            VideoExtensionsString = VideoExtensionsStringDEFAULT;
            OtherExtensionsString = OtherExtensionsStringDEFAULT;
            keepTogetherExtensionsString = keepTogetherExtensionsStringDEFAULT;
            subtitleExtensionsString = subtitleExtensionsStringDEFAULT;

            // have a guess at utorrent's path
            string[] guesses = new string[3];
            guesses[0] = System.Windows.Forms.Application.StartupPath + "\\..\\uTorrent\\uTorrent.exe";
            guesses[1] = "c:\\Program Files\\uTorrent\\uTorrent.exe";
            guesses[2] = "c:\\Program Files (x86)\\uTorrent\\uTorrent.exe";

            uTorrentPath = "";
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
            writer.WriteElement("KeepTogetherExtensions", keepTogetherExtensionsString);
            writer.WriteElement("LeadingZeroOnSeason", LeadingZeroOnSeason);
            writer.WriteElement("ShowInTaskbar", ShowInTaskbar);
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
            writer.WriteElement("uTorrentPath", uTorrentPath);
            writer.WriteElement("ResumeDatPath", ResumeDatPath);
            writer.WriteElement("SearchRSS", SearchRSS);
            writer.WriteElement("SearchJSON", SearchJSON);
            writer.WriteElement("SearchJSONManualScanOnly", SearchJSONManualScanOnly);
            writer.WriteElement("SearchRSSManualScanOnly", SearchRSSManualScanOnly);
            writer.WriteElement("EpImgs", EpTBNs);
            writer.WriteElement("NFOShows", NFOShows);
            writer.WriteElement("NFOEpisodes", NFOEpisodes);
            writer.WriteElement("KODIImages", KODIImages);
            writer.WriteElement("pyTivoMeta", pyTivoMeta);
            writer.WriteElement("pyTivoMetaSubFolder", pyTivoMetaSubFolder);
            writer.WriteElement("wdLiveTvMeta", wdLiveTvMeta);
            writer.WriteElement("FolderJpg", FolderJpg);
            writer.WriteElement("FolderJpgIs", (int) FolderJpgIs);
            writer.WriteElement("MonitoredFoldersScanType", (int) MonitoredFoldersScanType);
            writer.WriteElement("CheckuTorrent", CheckuTorrent);
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
            writer.WriteElement("LookForDateInFilename", LookForDateInFilename);
            writer.WriteElement("AutoMergeEpisodes", AutoMergeDownloadEpisodes);
            writer.WriteElement("AutoMergeLibraryEpisodes", AutoMergeLibraryEpisodes);
            writer.WriteElement("MonitorFolders", MonitorFolders);
            writer.WriteElement("StartupScan", runStartupCheck);
            writer.WriteElement("PeriodicScan", runPeriodicCheck);
            writer.WriteElement("PeriodicScanHours", periodCheckHours);
            writer.WriteElement("RemoveDownloadDirectoriesFiles", RemoveDownloadDirectoriesFiles);
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
            writer.WriteElement("DefShowSpecialsCount", DefShowSpecialsCount);
            writer.WriteElement("DefShowAutoFolders", DefShowAutoFolders);
            writer.WriteElement("DefShowUseDefLocation", DefShowUseDefLocation);
            writer.WriteElement("DefShowLocation", DefShowLocation);
            writer.WriteElement("DefaultShowTimezoneName", DefaultShowTimezoneName);
            writer.WriteElement("DefShowUseBase", DefShowUseBase);
            writer.WriteElement("DefShowUseSubFolders", DefShowUseSubFolders);
            writer.WriteElement("SampleFileMaxSizeMB", SampleFileMaxSizeMB); 

            TheSearchers.WriteXml(writer);
            WriteReplacements(writer);
            WriteRegExs(writer);
            XmlHelper.WriteStringsToXml(RSSURLs,writer,"RSSURLs", "URL");
            WriteShowStatusColours(writer);
            WriteFilters(writer);

            writer.WriteEndElement(); // settings
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
            if (Filter != null)
            {
                writer.WriteStartElement("ShowFilters");

                writer.WriteInfo("NameFilter", "Name", Filter.ShowName);
                writer.WriteInfo("ShowStatusFilter", "ShowStatus", Filter.ShowStatus);
                writer.WriteInfo("ShowNetworkFilter", "ShowNetwork", Filter.ShowNetwork);
                writer.WriteInfo("ShowRatingFilter", "ShowRating", Filter.ShowRating);

                foreach (string genre in Filter.Genres)
                {
                    writer.WriteInfo("GenreFilter", "Genre", genre);
                }

                writer.WriteEndElement(); //ShowFilters
            }

            if (SeasonFilter != null)
            {
                writer.WriteStartElement("SeasonFilters");
                writer.WriteElement("SeasonIgnoredFilter", SeasonFilter.HideIgnoredSeasons);
                writer.WriteEndElement(); //SeasonFilters
            }
        }

        private void WriteShowStatusColours(XmlWriter writer)
        {
            if (ShowStatusColors != null)
            {
                writer.WriteStartElement("ShowStatusTVWColors");
                foreach (KeyValuePair<ShowStatusColoringType, System.Drawing.Color> e in ShowStatusColors)
                {
                    writer.WriteStartElement("ShowStatusTVWColor");
                    // TODO ... Write Meta Flags
                    writer.WriteAttributeToXml("IsMeta", e.Key.IsMetaType);
                    writer.WriteAttributeToXml("IsShowLevel", e.Key.IsShowLevel);
                    writer.WriteAttributeToXml("ShowStatus", e.Key.Status);
                    writer.WriteAttributeToXml("Color", Helpers.TranslateColorToHtml(e.Value));
                    writer.WriteEndElement(); //ShowStatusTVWColor
                }

                writer.WriteEndElement(); // ShowStatusTVWColors
            }
        }

        internal float PercentDirtyUpgrade() => upgradeDirtyPercent;

        public FolderJpgIsType ItemForFolderJpg() => FolderJpgIs;

        public string GetVideoExtensionsString() => VideoExtensionsString;
        public string GetOtherExtensionsString() => OtherExtensionsString;
        public string GetKeepTogetherString() => keepTogetherExtensionsString;

        public bool RunPeriodicCheck() => runPeriodicCheck;
        public int PeriodicCheckPeriod() => periodCheckHours * 60 * 60 * 1000;
        public bool RunOnStartUp() => runStartupCheck;

        public string GetSeasonSearchTermsString() => searchSeasonWordsString;
        public string GetPreferredRSSSearchTermsString() => preferredRSSSearchTermsString;

        public static bool OKExtensionsString([CanBeNull] string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return true;
            }

            string[] t = s.Split(';');
            foreach (string s2 in t)
            {
                if (string.IsNullOrEmpty(s2) || !s2.StartsWith(".", StringComparison.Ordinal) || s2.ContainsAnyCharctersFrom(CompulsoryReplacements()) || s2.ContainsAnyCharctersFrom(Path.GetInvalidFileNameChars()))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool OKExporterLocation([CanBeNull] string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return false;
            }

            if (s.IsWebLink())
            {
                return false;
            }

            if (s.ContainsAnyCharctersFrom(Path.GetInvalidPathChars()))
            {
                return false;
            }

            return true;
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

            if (s.ContainsAnyCharctersFrom(Path.GetInvalidPathChars()))
            {
                return false;
            }

            return true;
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
                    "\\b(episode|ep|e) ?(?<e>[0-9]{2,}) ?- ?(series|season) ?(?<s>[0-9]+)",
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

        private static int TabNumberFromName([CanBeNull] string n)
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

        public bool FileHasUsefulExtension(FileInfo file, bool otherExtensionsToo)
        {
            if (VideoExtensionsArray
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Any(s => file.Name.EndsWith(s, StringComparison.InvariantCultureIgnoreCase)))
            {
                return true;
            }

            return otherExtensionsToo && OtherExtensionsArray
                                            .Where(s => !string.IsNullOrWhiteSpace(s))
                                            .Any(s => file.Name.EndsWith(s, StringComparison.InvariantCultureIgnoreCase));
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
        public string BTSearchURL([CanBeNull] ProcessedEpisode epi)
        {
            SeriesInfo s = epi?.TheSeries;
            if (s is null)
            {
                return "";
            }

            string url = epi.Show.UseCustomSearchUrl && !string.IsNullOrWhiteSpace(epi.Show.CustomSearchUrl)
                ? epi.Show.CustomSearchUrl
                : TheSearchers.CurrentSearchUrl();

            return CustomEpisodeName.NameForNoExt(epi, url, true);
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

            if (fn.ContainsAnyCharctersFrom(Path.GetInvalidFileNameChars()))
            {
                Logger.Warn($"Need to remove some characters from {fn} as the episode name contains characters that cannot be in the filename.");
                fn = fn.RemoveCharactersFrom(Path.GetInvalidFileNameChars()).RemoveCharactersFrom("/t".ToCharArray());
            }

            return ForceLowercaseFilenames ? fn.ToLower() : fn;
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

            public Replacement(string a, string b, bool insens)
            {
                if (b is null)
                {
                    b = "";
                }

                This = a;
                That = b;
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
        public class ShowStatusColoringTypeList : Dictionary<ShowStatusColoringType, System.Drawing.Color>
        {
            public ShowStatusColoringTypeList()
            {
            }

            protected ShowStatusColoringTypeList(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }

            public bool IsShowStatusDefined(string showStatus)
            {
                foreach (KeyValuePair<ShowStatusColoringType, System.Drawing.Color> e in this)
                {
                    if (!e.Key.IsMetaType && e.Key.IsShowLevel &&
                        e.Key.Status.Equals(showStatus, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }

            public System.Drawing.Color GetEntry(bool meta, bool showLevel, string status)
            {
                foreach (KeyValuePair<ShowStatusColoringType, System.Drawing.Color> e in this)
                {
                    if (e.Key.IsMetaType == meta && e.Key.IsShowLevel == showLevel &&
                        e.Key.Status.Equals(status, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return e.Value;
                    }
                }

                return System.Drawing.Color.Empty;
            }
        }

        public class ShowStatusColoringType
        {
            public ShowStatusColoringType(bool isMetaType, bool isShowLevel, string status)
            {
                IsMetaType = isMetaType;
                IsShowLevel = isShowLevel;
                Status = status;
            }

            public readonly bool IsMetaType;
            public readonly bool IsShowLevel;
            public readonly string Status;

            [NotNull]
            public string Text
            {
                get
                {
                    if (IsMetaType)
                    {
                        return IsShowLevel ? "Show Seasons" : "Season" +$" Status: { StatusTextForDisplay}";
                    }

                    return IsShowLevel ? $"Show Status: {StatusTextForDisplay}" : string.Empty;
                }
            }

            private string StatusTextForDisplay
            {
                get
                {
                    if (!IsMetaType)
                    {
                        return Status;
                    }
                    string value = ConvertFromOldStyle();

                    return IsShowLevel ? ShowLevelStatusText(value) : SeasonLevelStatusText(value);
                }
            }

            [NotNull]
            private string ConvertFromOldStyle()
            {
                string value = Status.Equals("PartiallyAired") ? "partiallyAired"
                    : Status.Equals("NoneAired") ? "noneAired"
                    : Status.Equals("Aired") ? "aired"
                    : Status.Equals("NoEpisodesOrSeasons") ? "noEpisodesOrSeasons"
                    : Status.Equals("NoEpisodes") ? "noEpisodes"
                    : Status;

                return value;
            }

            private string SeasonLevelStatusText([NotNull] string value)
            {
                Season.SeasonStatus status =
                    (Season.SeasonStatus) Enum.Parse(typeof(Season.SeasonStatus), value);

                switch (status)
                {
                    case Season.SeasonStatus.aired:
                        return "All aired";
                    case Season.SeasonStatus.noEpisodes:
                        return "No Episodes";
                    case Season.SeasonStatus.noneAired:
                        return "None aired";
                    case Season.SeasonStatus.partiallyAired:
                        return "Partially aired";
                    default:
                        return Status;
                }
            }

            private string ShowLevelStatusText([NotNull] string value)
            {
                //Convert from old style values if needed
                ShowItem.ShowAirStatus status =
                    (ShowItem.ShowAirStatus) Enum.Parse(typeof(ShowItem.ShowAirStatus), value, true);

                switch (status)
                {
                    case ShowItem.ShowAirStatus.aired:
                        return "All aired";
                    case ShowItem.ShowAirStatus.noEpisodesOrSeasons:
                        return "No Seasons or Episodes in Seasons";
                    case ShowItem.ShowAirStatus.noneAired:
                        return "None aired";
                    case ShowItem.ShowAirStatus.partiallyAired:
                        return "Partially aired";
                    default:
                        return Status;
                }
            }
        }

        // ReSharper disable once FunctionComplexityOverflow
        public void load([NotNull] XElement xmlSettings)
        {
            SetToDefaults();
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
            ExportWTWICAL = xmlSettings.ExtractBool("ExportWTWICAL",false);
            ExportWTWICALTo = xmlSettings.ExtractString("ExportWTWICALTo");
            WTWRecentDays = xmlSettings.ExtractInt("WTWRecentDays",7);
            StartupTab = TabNumberFromName(xmlSettings.ExtractString("StartupTab2"));
            NamingStyle.StyleString = xmlSettings.ExtractString("NamingStyle") ??
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
            ShowInTaskbar = xmlSettings.ExtractBool("ShowInTaskbar",true);
            RenameTxtToSub = xmlSettings.ExtractBool("RenameTxtToSub",false);
            ShowEpisodePictures = xmlSettings.ExtractBool("ShowEpisodePictures",true);
            HideWtWSpoilers = xmlSettings.ExtractBool("HideWtWSpoilers",false);
            HideMyShowsSpoilers = xmlSettings.ExtractBool("HideMyShowsSpoilers",false);
            AutoCreateFolders = xmlSettings.ExtractBool("AutoCreateFolders",false);
            AutoSelectShowInMyShows = xmlSettings.ExtractBool("AutoSelectShowInMyShows",true);
            SpecialsFolderName = xmlSettings.ExtractString("SpecialsFolderName", "Specials");
            SeasonFolderFormat = xmlSettings.ExtractString("SeasonFolderFormat");
            SearchJSONURL = xmlSettings.ExtractString("SearchJSONURL", "https://eztv.ag/api/get-torrents?imdb_id=");
            SearchJSONRootNode = xmlSettings.ExtractString("SearchJSONRootNode", "torrents");
            SearchJSONFilenameToken = xmlSettings.ExtractString("SearchJSONFilenameToken", "filename");
            SearchJSONURLToken = xmlSettings.ExtractString("SearchJSONURLToken", "torrent_url");
            SearchJSONFileSizeToken = xmlSettings.ExtractString("SearchJSONFileSizeToken", "size_bytes");
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
            RenameCheck = xmlSettings.ExtractBool("RenameCheck",true);
            PreventMove = xmlSettings.ExtractBool("PreventMove",false);
            CheckuTorrent = xmlSettings.ExtractBool("CheckuTorrent",false);
            CheckqBitTorrent = xmlSettings.ExtractBool("CheckqBitTorrent",false);
            qBitTorrentHost = xmlSettings.ExtractString("qBitTorrentHost", "localhost");
            qBitTorrentPort = xmlSettings.ExtractString("qBitTorrentPort", "8080");
            qBitTorrentAPIVersion = xmlSettings.ExtractEnum( "qBitTorrentAPIVersion", qBitTorrentAPIVersion.v2);
            MissingCheck = xmlSettings.ExtractBool("MissingCheck",true);
            MoveLibraryFiles = xmlSettings.ExtractBool("MoveLibraryFiles",true);
            CorrectFileDates = xmlSettings.ExtractBool("UpdateFileDates",false);
            SearchLocally = xmlSettings.ExtractBool("SearchLocally",true);
            IgnorePreviouslySeen = xmlSettings.ExtractBool("IgnorePreviouslySeen",false);
            LeaveOriginals = xmlSettings.ExtractBool("LeaveOriginals",false);
            AutoSearchForDownloadedFiles = xmlSettings.ExtractBool("AutoSearchForDownloadedFiles",false);
            LookForDateInFilename = xmlSettings.ExtractBool("LookForDateInFilename",false);
            AutoMergeDownloadEpisodes = xmlSettings.ExtractBool("AutoMergeEpisodes",false);
            AutoMergeLibraryEpisodes = xmlSettings.ExtractBool("AutoMergeLibraryEpisodes",false);
            RetainLanguageSpecificSubtitles = xmlSettings.ExtractBool("RetainLanguageSpecificSubtitles",true);
            ForceBulkAddToUseSettingsOnly = xmlSettings.ExtractBool("ForceBulkAddToUseSettingsOnly",false);
            MonitorFolders = xmlSettings.ExtractBool("MonitorFolders",false);
            runStartupCheck = xmlSettings.ExtractBool("StartupScan",false);
            runPeriodicCheck = xmlSettings.ExtractBool("PeriodicScan",false);
            periodCheckHours = xmlSettings.ExtractInt("PeriodicScanHours",1);
            RemoveDownloadDirectoriesFiles = xmlSettings.ExtractBool("RemoveDownloadDirectoriesFiles",false);
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
            DefShowSpecialsCount = xmlSettings.ExtractBool("DefShowSpecialsCount", false);
            DefShowAutoFolders = xmlSettings.ExtractBool("DefShowAutoFolders", true);
            DefShowUseDefLocation = xmlSettings.ExtractBool("DefShowUseDefLocation", false);
            DefShowUseBase = xmlSettings.ExtractBool("DefShowUseBase", false);
            DefShowUseSubFolders = xmlSettings.ExtractBool("DefShowUseSubFolders", true);
            DefShowLocation = xmlSettings.ExtractString("DefShowLocation");
            DefaultShowTimezoneName = xmlSettings.ExtractString("DefaultShowTimezoneName");

            Tidyup.load(xmlSettings);
            RSSURLs = xmlSettings.Descendants("RSSURLs").FirstOrDefault()?.ReadStringsFromXml("URL");
            TheSearchers = new Searchers(xmlSettings.Descendants("TheSearchers").FirstOrDefault());

            UpdateReplacements(xmlSettings);
            UpdateRegExs(xmlSettings);
            UpdateShowStatus(xmlSettings);
            UpdateFiters(xmlSettings);
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
                    .FirstOrDefault()?.Value
            };

            foreach (XAttribute rep in xmlSettings.Descendants("ShowFilters").Descendants("GenreFilter").Attributes("Genre"))
            {
                Filter.Genres.Add(rep.Value);
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
                string showStatus = rep.Attribute("ShowStatus")?.Value;
                bool isMeta = bool.Parse(rep.Attribute("IsMeta")?.Value ?? "false");
                bool isShowLevel = bool.Parse(rep.Attribute("IsShowLevel")?.Value ?? "true");
                ShowStatusColoringType type = new ShowStatusColoringType(isMeta, isShowLevel, showStatus);
                string color = rep.Attribute("Color")?.Value;
                if (string.IsNullOrEmpty(color))
                {
                    continue;
                }

                System.Drawing.Color c = System.Drawing.ColorTranslator.FromHtml(color);
                ShowStatusColors.Add(type, c);
            }
        }

        private void UpdateRegExs([NotNull] XElement xmlSettings)
        {
            FNPRegexs.Clear();
            foreach (XElement rep in xmlSettings.Descendants("FNPRegexs").FirstOrDefault()?.Descendants("Regex") ??
                                     new List<XElement>())
            {
                FNPRegexs.Add(new FilenameProcessorRE(
                    XmlConvert.ToBoolean(rep.Attribute("Enabled")?.Value ?? "false"),
                    rep.Attribute("RE")?.Value,
                    XmlConvert.ToBoolean(rep.Attribute("UseFullPath")?.Value ?? "false"),
                    rep.Attribute("Notes")?.Value
                ));
            }
        }

        private void UpdateReplacements([NotNull] XElement xmlSettings)
        {
            Replacements.Clear();
            foreach (XElement rep in xmlSettings.Descendants("Replacements").FirstOrDefault()?.Descendants("Replace") ??
                                     new List<XElement>())
            {
                Replacements.Add(new Replacement(rep.Attribute("This")?.Value, rep.Attribute("That")?.Value,
                    rep.Attribute("CaseInsensitive")?.Value == "Y"));
            }
        }
    }
}
