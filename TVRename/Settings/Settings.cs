// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Alphaleonis.Win32.Filesystem;
using System.Text.RegularExpressions;
using System.Xml;
// ReSharper disable RedundantDefaultMemberInitializer

// Settings for TVRename.  All of this stuff is through Options->Preferences in the app.

namespace TVRename
{   
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

        public string[] EmptyIgnoreExtensionsArray => EmptyIgnoreExtensionList.Split(';');
        public string[] EmptyIgnoreWordsArray => EmptyIgnoreWordList.Split(';');
    }

    public class Replacement
    {
        // used for invalid (and general) character (and string) replacements in filenames

        public readonly bool CaseInsensitive;
        public readonly string That;
        public readonly string This;

        public Replacement(string a, string b, bool insens)
        {
            if (b == null)
                b = "";
            This = a;
            That = b;
            CaseInsensitive = insens;
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

    [Serializable()]
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

        public bool IsMetaType;
        public bool IsShowLevel;
        public string Status;

        public string Text
        {
            get
            {
                if (IsShowLevel && IsMetaType)
                {
                    return $"Show Seasons Status: {StatusTextForDisplay}";
                }
                if (!IsShowLevel && IsMetaType)
                {
                    return $"Season Status: {StatusTextForDisplay}";
                }
                if (IsShowLevel && !IsMetaType)
                {
                    return $"Show Status: {StatusTextForDisplay}";
                }
                return "";
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
                if (IsShowLevel)
                {
                    ShowItem.ShowAirStatus status =
                        (ShowItem.ShowAirStatus) Enum.Parse(typeof (ShowItem.ShowAirStatus), Status);
                    switch (status)
                    {
                        case ShowItem.ShowAirStatus.Aired:
                            return "All aired";
                        case ShowItem.ShowAirStatus.NoEpisodesOrSeasons:
                            return "No Seasons or Episodes in Seasons";
                        case ShowItem.ShowAirStatus.NoneAired:
                            return "None aired";
                        case ShowItem.ShowAirStatus.PartiallyAired:
                            return "Partially aired";
                        default:
                            return Status;
                    }
                }
                else
                {
                    Season.SeasonStatus status =
                        (Season.SeasonStatus) Enum.Parse(typeof (Season.SeasonStatus), Status);
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
            }
        }
    }

    public sealed class TVSettings
    {
        //We are using the singleton design pattern
        //http://msdn.microsoft.com/en-au/library/ff650316.aspx

        private static volatile TVSettings instance;
        private static readonly object syncRoot = new object();
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static TVSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new TVSettings();
                    }
                }
                return instance;
            }
        }

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

        public enum KODIType
        {
            Eden,
            Frodo,
            Both
        }

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
        public List<IgnoreItem> Ignore;
        public bool AutoSelectShowInMyShows = true;
        public bool AutoCreateFolders = false;
        public bool BGDownload = false;
        public bool CheckuTorrent = false;
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
        public List<FilenameProcessorRE> FNPRegexs = DefaultFNPList();
        public bool FolderJpg = false;
        public FolderJpgIsType FolderJpgIs = FolderJpgIsType.Poster;
        public ScanType MonitoredFoldersScanType = ScanType.Full;
        public KODIType SelectedKODIType = KODIType.Both;
        public bool ForceLowercaseFilenames = false;
        public bool IgnoreSamples = true;
        public bool KeepTogether = true;
        public bool LeadingZeroOnSeason = false;
        public bool LeaveOriginals = false;
        public bool LookForDateInFilename = false;
        public bool MissingCheck = true;
        public bool CorrectFileDates = false;
        public bool NFOShows = false;
        public bool NFOEpisodes = false;
        public bool KODIImages = false;
        public bool pyTivoMeta = false;
        public bool pyTivoMetaSubFolder = false;
        public CustomName NamingStyle = new CustomName();
        public bool NotificationAreaIcon = false;
        public bool OfflineMode = false;

        public BetaMode mode = BetaMode.ProductionOnly;
        public float upgradeDirtyPercent = 20;
        public  KeepTogetherModes  keepTogetherMode = KeepTogetherModes.All;

        public bool BulkAddIgnoreRecycleBin = false;
        public bool BulkAddCompareNoVideoFolders = false;
        public string AutoAddMovieTerms = "dvdrip;camrip;screener;dvdscr;r5;bluray";
        public string AutoAddIgnoreSuffixes = "1080p;720p";

        public string[] AutoAddMovieTermsArray => AutoAddMovieTerms.Split(';');

        public string[] AutoAddIgnoreSuffixesArray => AutoAddIgnoreSuffixes.Split(';');

        public string[] keepTogetherExtensionsArray => keepTogetherExtensionsString.Split(';');
        public string keepTogetherExtensionsString = "";

        public string defaultSeasonWord = "Season";

        public string[] searchSeasonWordsArray => searchSeasonWordsString.Split(';');
        public string[] PreferredRSSSearchTerms() => preferredRSSSearchTermsString.Split(';');

        public string searchSeasonWordsString = "Season;Series;Saison;Temporada;Seizoen";
        public string preferredRSSSearchTermsString = "720p;1080p";

        internal bool IncludeBetaUpdates()
        {
            return (mode== BetaMode.BetaToo );
        }

        public string OtherExtensionsString = "";
        public ShowFilter Filter = new ShowFilter();

        private string[] OtherExtensionsArray => OtherExtensionsString.Split(';');

        public int ParallelDownloads = 4;
        public List<string> RSSURLs = DefaultRSSURLList();
        public bool RenameCheck = true;
        public bool PreventMove = false;
        public bool RenameTxtToSub = false;
        public List<Replacement> Replacements = DefaultListRE();
        public string ResumeDatPath = "";
        public int SampleFileMaxSizeMB = 50; // sample file must be smaller than this to be ignored
        public bool SearchLocally = true;
        public bool SearchRSS = false;
        public bool ShowEpisodePictures = true;
        public bool HideWtWSpoilers = false;
        public bool HideMyShowsSpoilers = false;
        public bool ShowInTaskbar = true;
        public bool AutoSearchForDownloadedFiles = false;
        public string SpecialsFolderName = "Specials";
        public int StartupTab = 0;
        public Searchers TheSearchers = new Searchers();

        public string[] VideoExtensionsArray => VideoExtensionsString.Split(';');
        public bool ForceBulkAddToUseSettingsOnly = false;
        public bool RetainLanguageSpecificSubtitles = true;
        public bool AutoMergeDownloadEpisodes = false;
        public bool AutoMergeLibraryEpisodes = false;
        public string VideoExtensionsString = "";
        public int WTWRecentDays = 7;
        public string uTorrentPath = "";
        public bool MonitorFolders = false;
        public bool RemoveDownloadDirectoriesFiles =false;
        public ShowStatusColoringTypeList ShowStatusColors = new ShowStatusColoringTypeList();
        public string SABHostPort = "";
        public string SABAPIKey = "";
        public bool CheckSABnzbd = false;
        public string PreferredLanguage = "en";
        public WTWDoubleClickAction WTWDoubleClick;

        public TidySettings Tidyup = new TidySettings();
        public bool runPeriodicCheck = false;
        public int periodCheckHours =1;
        public bool runStartupCheck = false;

        private TVSettings()
        {
            SetToDefaults();
        }

        public void load(XmlReader reader)
        {
            SetToDefaults();

            reader.Read();
            if (reader.Name != "Settings")
                return; // bail out

            reader.Read();
            while (!reader.EOF)
            {
                if ((reader.Name == "Settings") && !reader.IsStartElement())
                    break; // all done

                if (reader.Name == "Searcher")
                {
                    string srch = reader.ReadElementContentAsString(); // and match it based on name...
                    TheSearchers.CurrentSearch = srch;
                }
                else if (reader.Name == "TheSearchers")
                {
                    TheSearchers = new Searchers(reader.ReadSubtree());
                    reader.Read();
                }
                else if (reader.Name == "BGDownload")
                    BGDownload = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "OfflineMode")
                    OfflineMode = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "Replacements" && !reader.IsEmptyElement)
                {
                    Replacements.Clear();
                    reader.Read();
                    while (!reader.EOF)
                    {
                        if ((reader.Name == "Replacements") && (!reader.IsStartElement()))
                            break;
                        if (reader.Name == "Replace")
                        {
                            Replacements.Add(new Replacement(reader.GetAttribute("This"),
                                                                  reader.GetAttribute("That"),
                                                                  reader.GetAttribute("CaseInsensitive") == "Y"));
                            reader.Read();
                        }
                        else
                            reader.ReadOuterXml();
                    }
                    reader.Read();
                }
                else if (reader.Name == "ExportWTWRSS" && !reader.IsEmptyElement)
                    ExportWTWRSS = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportWTWRSSTo")
                    ExportWTWRSSTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportWTWXML")
                    ExportWTWXML = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportWTWXMLTo")
                    ExportWTWXMLTo = reader.ReadElementContentAsString();
                else if (reader.Name == "WTWRecentDays")
                    WTWRecentDays = reader.ReadElementContentAsInt();
                else if (reader.Name == "StartupTab")
                {
                    int n = reader.ReadElementContentAsInt();
                    if (n == 6)
                        StartupTab = 2; // WTW is moved
                    else if ((n >= 1) && (n <= 3)) // any of the three scans
                        StartupTab = 1;
                    else
                        StartupTab = 0; // otherwise, My Shows
                }
                else if (reader.Name == "StartupTab2")
                    StartupTab = TabNumberFromName(reader.ReadElementContentAsString());
                else if (reader.Name == "DefaultNamingStyle") // old naming style
                    NamingStyle.StyleString = CustomName.OldNStyle(reader.ReadElementContentAsInt());
                else if (reader.Name == "NamingStyle")
                    NamingStyle.StyleString = reader.ReadElementContentAsString();
                else if (reader.Name == "NotificationAreaIcon")
                    NotificationAreaIcon = reader.ReadElementContentAsBoolean();
                else if ((reader.Name == "GoodExtensions") || (reader.Name == "VideoExtensions"))
                    VideoExtensionsString = reader.ReadElementContentAsString();
                else if (reader.Name == "OtherExtensions")
                    OtherExtensionsString = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportRSSMaxDays")
                    ExportRSSMaxDays = reader.ReadElementContentAsInt();
                else if (reader.Name == "ExportRSSMaxShows")
                    ExportRSSMaxShows = reader.ReadElementContentAsInt();
                else if (reader.Name == "ExportRSSDaysPast")
                    ExportRSSDaysPast = reader.ReadElementContentAsInt();
                else if (reader.Name == "KeepTogether")
                    KeepTogether = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "LeadingZeroOnSeason")
                    LeadingZeroOnSeason = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ShowInTaskbar")
                    ShowInTaskbar = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "RenameTxtToSub")
                    RenameTxtToSub = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ShowEpisodePictures")
                    ShowEpisodePictures = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "HideWtWSpoilers")
                    HideWtWSpoilers = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "HideMyShowsSpoilers")
                    HideMyShowsSpoilers = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "AutoCreateFolders")
                    AutoCreateFolders = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "AutoSelectShowInMyShows")
                    AutoSelectShowInMyShows = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SpecialsFolderName")
                    SpecialsFolderName = reader.ReadElementContentAsString();
                else if (reader.Name == "SABAPIKey")
                    SABAPIKey = reader.ReadElementContentAsString();
                else if (reader.Name == "CheckSABnzbd")
                    CheckSABnzbd = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SABHostPort")
                    SABHostPort = reader.ReadElementContentAsString();
                else if (reader.Name == "PreferredLanguage")
                    PreferredLanguage = reader.ReadElementContentAsString();
                else if (reader.Name == "WTWDoubleClick")
                    WTWDoubleClick = (WTWDoubleClickAction)reader.ReadElementContentAsInt();
                else if (reader.Name == "ExportMissingXML")
                    ExportMissingXML = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportMissingXMLTo")
                    ExportMissingXMLTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportMissingCSV")
                    ExportMissingCSV = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportMissingCSVTo")
                    ExportMissingCSVTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportRenamingXML")
                    ExportRenamingXML = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportRenamingXMLTo")
                    ExportRenamingXMLTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportFOXML")
                    ExportFOXML = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportFOXMLTo")
                    ExportFOXMLTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportShowsTXT")
                    ExportShowsTXT = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportShowsTXTTo")
                    ExportShowsTXTTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportShowsHTML")
                    ExportShowsHTML = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportShowsHTMLTo")
                    ExportShowsHTMLTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ForceLowercaseFilenames")
                    ForceLowercaseFilenames = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "IgnoreSamples")
                    IgnoreSamples = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SampleFileMaxSizeMB")
                    SampleFileMaxSizeMB = reader.ReadElementContentAsInt();
                else if (reader.Name == "ParallelDownloads")
                    ParallelDownloads = reader.ReadElementContentAsInt();
                else if (reader.Name == "uTorrentPath")
                    uTorrentPath = reader.ReadElementContentAsString();
                else if (reader.Name == "ResumeDatPath")
                    ResumeDatPath = reader.ReadElementContentAsString();
                else if (reader.Name == "SearchRSS")
                    SearchRSS = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "EpImgs")
                    EpTBNs = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "NFOs") //support legacy tag
                {
                    NFOShows = reader.ReadElementContentAsBoolean();
                    NFOEpisodes = NFOShows;
                }
                else if (reader.Name == "NFOShows")
                    NFOShows = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "NFOEpisodes")
                    NFOEpisodes = reader.ReadElementContentAsBoolean();
                else if ((reader.Name == "XBMCImages") || (reader.Name == "KODIImages")) //Backward Compatibilty
                    KODIImages = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "pyTivoMeta")
                    pyTivoMeta = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "pyTivoMetaSubFolder")
                    pyTivoMetaSubFolder = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "FolderJpg")
                    FolderJpg = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "FolderJpgIs")
                    FolderJpgIs = (FolderJpgIsType)reader.ReadElementContentAsInt();
                else if (reader.Name == "MonitoredFoldersScanType")
                    MonitoredFoldersScanType = (ScanType)reader.ReadElementContentAsInt();
                else if ((reader.Name == "SelectedXBMCType") || (reader.Name == "SelectedKODIType"))
                    SelectedKODIType = (KODIType)reader.ReadElementContentAsInt();
                else if (reader.Name == "RenameCheck")
                    RenameCheck = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "PreventMove")
                    PreventMove  = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "CheckuTorrent")
                    CheckuTorrent = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "MissingCheck")
                    MissingCheck = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "UpdateFileDates")
                    CorrectFileDates = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SearchLocally")
                    SearchLocally = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "LeaveOriginals")
                    LeaveOriginals = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "AutoSearchForDownloadedFiles")
                    AutoSearchForDownloadedFiles = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "LookForDateInFilename")
                    LookForDateInFilename = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "AutoMergeEpisodes")
                    AutoMergeDownloadEpisodes = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "AutoMergeLibraryEpisodes")
                    AutoMergeLibraryEpisodes = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "RetainLanguageSpecificSubtitles")
                    RetainLanguageSpecificSubtitles = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ForceBulkAddToUseSettingsOnly")
                    ForceBulkAddToUseSettingsOnly = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "MonitorFolders")
                    MonitorFolders = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "StartupScan")
                    runStartupCheck = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "PeriodicScan")
                    runPeriodicCheck = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "PeriodicScanHours")
                    periodCheckHours = reader.ReadElementContentAsInt();
                else if (reader.Name == "RemoveDownloadDirectoriesFiles")
                    RemoveDownloadDirectoriesFiles = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "EpJPGs")
                    EpJPGs = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SeriesJpg")
                    SeriesJpg = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "Mede8erXML")
                    Mede8erXML = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ShrinkLargeMede8erImages")
                    ShrinkLargeMede8erImages = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "FanArtJpg")
                    FanArtJpg = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "DeleteEmpty")
                    Tidyup.DeleteEmpty = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "DeleteEmptyIsRecycle")
                    Tidyup.DeleteEmptyIsRecycle = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "EmptyIgnoreWords")
                    Tidyup.EmptyIgnoreWords = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "EmptyIgnoreWordList")
                    Tidyup.EmptyIgnoreWordList = reader.ReadElementContentAsString();
                else if (reader.Name == "EmptyIgnoreExtensions")
                    Tidyup.EmptyIgnoreExtensions = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "EmptyIgnoreExtensionList")
                    Tidyup.EmptyIgnoreExtensionList = reader.ReadElementContentAsString();
                else if (reader.Name == "EmptyMaxSizeCheck")
                    Tidyup.EmptyMaxSizeCheck = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "EmptyMaxSizeMB")
                    Tidyup.EmptyMaxSizeMB = reader.ReadElementContentAsInt();
                else if (reader.Name == "BulkAddIgnoreRecycleBin")
                    BulkAddIgnoreRecycleBin = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "BulkAddCompareNoVideoFolders")
                    BulkAddCompareNoVideoFolders = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "AutoAddMovieTerms")
                    AutoAddMovieTerms = reader.ReadElementContentAsString();
                else if (reader.Name == "AutoAddIgnoreSuffixes")
                    AutoAddIgnoreSuffixes = reader.ReadElementContentAsString();
                else if (reader.Name == "BetaMode")
                    mode = (BetaMode)reader.ReadElementContentAsInt();
                else if (reader.Name == "PercentDirtyUpgrade")
                    upgradeDirtyPercent = reader.ReadElementContentAsFloat();
                else if (reader.Name == "BaseSeasonName")
                    defaultSeasonWord = reader.ReadElementContentAsString( );
                else if (reader.Name == "SearchSeasonNames")
                    searchSeasonWordsString = reader.ReadElementContentAsString();
                else if (reader.Name == "PreferredRSSSearchTerms")
                    preferredRSSSearchTermsString = reader.ReadElementContentAsString();
                else if (reader.Name == "KeepTogetherType")
                    keepTogetherMode = (KeepTogetherModes) reader.ReadElementContentAsInt();
                else if (reader.Name == "KeepTogetherExtensions")
                    keepTogetherExtensionsString = reader.ReadElementContentAsString();
                else if (reader.Name == "FNPRegexs" && !reader.IsEmptyElement)
                {
                    FNPRegexs.Clear();
                    reader.Read();
                    while (!reader.EOF)
                    {
                        if ((reader.Name == "FNPRegexs") && (!reader.IsStartElement()))
                            break;
                        if (reader.Name == "Regex")
                        {
                            string s = reader.GetAttribute("Enabled");
                            bool en = s == null || bool.Parse(s);

                            FNPRegexs.Add(new FilenameProcessorRE(en, reader.GetAttribute("RE"),
                                                                       bool.Parse(reader.GetAttribute("UseFullPath")),
                                                                       reader.GetAttribute("Notes")));
                            reader.Read();
                        }
                        else
                            reader.ReadOuterXml();
                    }
                    reader.Read();
                }
                else if (reader.Name == "RSSURLs" && !reader.IsEmptyElement)
                {
                    RSSURLs.Clear();
                    reader.Read();
                    while (!reader.EOF)
                    {
                        if ((reader.Name == "RSSURLs") && (!reader.IsStartElement()))
                            break;
                        if (reader.Name == "URL")
                            RSSURLs.Add(reader.ReadElementContentAsString());
                        else
                            reader.ReadOuterXml();
                    }
                    reader.Read();
                }
                else if (reader.Name == "ShowStatusTVWColors" && !reader.IsEmptyElement)
                {
                    ShowStatusColors = new ShowStatusColoringTypeList();
                    reader.Read();
                    while (!reader.EOF)
                    {
                        if ((reader.Name == "ShowStatusTVWColors") && (!reader.IsStartElement()))
                            break;
                        if (reader.Name == "ShowStatusTVWColor")
                        {
                            ShowStatusColoringType type = null;
                            try
                            {
                                string showStatus = reader.GetAttribute("ShowStatus");
                                bool isMeta = bool.Parse(reader.GetAttribute("IsMeta"));
                                bool isShowLevel = bool.Parse(reader.GetAttribute("IsShowLevel"));

                                type = new ShowStatusColoringType(isMeta, isShowLevel, showStatus);
                            }
                            catch
                            {
                                // ignored
                            }

                            string color = reader.GetAttribute("Color");
                            if (type != null && !string.IsNullOrEmpty(color))
                            {
                                try
                                {
                                    System.Drawing.Color c = System.Drawing.ColorTranslator.FromHtml(color);
                                    ShowStatusColors.Add(type, c);
                                }
                                catch
                                {
                                    // ignored
                                }
                            }
                            reader.Read();
                        }
                        else
                            reader.ReadOuterXml();
                    }
                    reader.Read();
                }
                else if (reader.Name == "ShowFilters" && !reader.IsEmptyElement)
                {
                    Filter = new ShowFilter();
                    reader.Read();
                    while (!reader.EOF)
                    {
                        if ((reader.Name == "ShowFilters") && (!reader.IsStartElement()))
                            break;
                        if (reader.Name == "ShowNameFilter")
                        {
                            Filter.ShowName = reader.GetAttribute("ShowName");
                            reader.Read();
                        }
                        else if (reader.Name == "ShowStatusFilter")
                        {
                            Filter.ShowStatus = reader.GetAttribute("ShowStatus");
                            reader.Read();
                        }
                        else if (reader.Name == "ShowRatingFilter")
                        {
                            Filter.ShowRating = reader.GetAttribute("ShowRating");
                            reader.Read();
                        }
                        else if (reader.Name == "ShowNetworkFilter")
                        {
                            Filter.ShowNetwork = reader.GetAttribute("ShowNetwork");
                            reader.Read();
                        }
                        else if (reader.Name == "GenreFilter")
                        {
                            Filter.Genres.Add(reader.GetAttribute("Genre"));
                            reader.Read();
                        }
                        else
                            reader.ReadOuterXml();
                    }
                    reader.Read();
                }
                else
                    reader.ReadOuterXml();
            }
        }

        public void SetToDefaults()
        {
            // defaults that aren't handled with default initialisers
            Ignore = new List<IgnoreItem>();

            DownloadFolders = new List<string>();
            IgnoreFolders = new List<string>();
            LibraryFolders = new List<string>();

            VideoExtensionsString =
                ".avi;.mpg;.mpeg;.mkv;.mp4;.wmv;.divx;.ogm;.qt;.rm;.m4v;.webm;.vob;.ovg;.ogg;.mov;.m4p;.3gp";
            OtherExtensionsString = ".srt;.nfo;.txt;.tbn";
            keepTogetherExtensionsString = ".srt;.nfo;.txt;.tbn";

            // have a guess at utorrent's path
            string[] guesses = new string[3];
            guesses[0] = System.Windows.Forms.Application.StartupPath + "\\..\\uTorrent\\uTorrent.exe";
            guesses[1] = "c:\\Program Files\\uTorrent\\uTorrent.exe";
            guesses[2] = "c:\\Program Files (x86)\\uTorrent\\uTorrent.exe";

            uTorrentPath = "";
            foreach (string g in guesses)
            {
                FileInfo f = new FileInfo(g);
                if (f.Exists)
                {
                    uTorrentPath = f.FullName;
                    break;
                }
            }

            // ResumeDatPath
            FileInfo f2 =
                new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"uTorrent\resume.dat"));
            ResumeDatPath = f2.Exists ? f2.FullName : "";
        }

        public void WriteXML(XmlWriter writer)
        {
            writer.WriteStartElement("Settings");
            TheSearchers.WriteXML(writer);
            XmlHelper.WriteElementToXml(writer,"BGDownload",BGDownload);
            XmlHelper.WriteElementToXml(writer,"OfflineMode",OfflineMode);
            writer.WriteStartElement("Replacements");
            foreach (Replacement R in Replacements)
            {
                writer.WriteStartElement("Replace");
                XmlHelper.WriteAttributeToXml(writer,"This",R.This);
                XmlHelper.WriteAttributeToXml(writer, "That", R.That);
                XmlHelper.WriteAttributeToXml(writer, "CaseInsensitive", R.CaseInsensitive ? "Y" : "N");
                writer.WriteEndElement(); //Replace
            }
            writer.WriteEndElement(); //Replacements
            
            XmlHelper.WriteElementToXml(writer,"ExportWTWRSS",ExportWTWRSS);
            XmlHelper.WriteElementToXml(writer,"ExportWTWRSSTo",ExportWTWRSSTo);
            XmlHelper.WriteElementToXml(writer,"ExportWTWXML",ExportWTWXML);
            XmlHelper.WriteElementToXml(writer,"ExportWTWXMLTo",ExportWTWXMLTo);
            XmlHelper.WriteElementToXml(writer,"WTWRecentDays",WTWRecentDays);
            XmlHelper.WriteElementToXml(writer,"ExportMissingXML",ExportMissingXML);
            XmlHelper.WriteElementToXml(writer,"ExportMissingXMLTo",ExportMissingXMLTo);
            XmlHelper.WriteElementToXml(writer,"ExportMissingCSV",ExportMissingCSV);
            XmlHelper.WriteElementToXml(writer,"ExportMissingCSVTo",ExportMissingCSVTo);
            XmlHelper.WriteElementToXml(writer,"ExportRenamingXML",ExportRenamingXML);
            XmlHelper.WriteElementToXml(writer,"ExportRenamingXMLTo",ExportRenamingXMLTo);
            XmlHelper.WriteElementToXml(writer,"ExportShowsTXT", ExportShowsTXT);
            XmlHelper.WriteElementToXml(writer, "ExportShowsTXTTo", ExportShowsTXTTo);
            XmlHelper.WriteElementToXml(writer, "ExportShowsHTML", ExportShowsHTML);
            XmlHelper.WriteElementToXml(writer, "ExportShowsHTMLTo", ExportShowsHTMLTo);
            XmlHelper.WriteElementToXml(writer,"ExportFOXML",ExportFOXML);
            XmlHelper.WriteElementToXml(writer,"ExportFOXMLTo",ExportFOXMLTo);
            XmlHelper.WriteElementToXml(writer,"StartupTab2",TabNameForNumber(StartupTab));
            XmlHelper.WriteElementToXml(writer,"NamingStyle",NamingStyle.StyleString);
            XmlHelper.WriteElementToXml(writer,"NotificationAreaIcon",NotificationAreaIcon);
            XmlHelper.WriteElementToXml(writer,"VideoExtensions",VideoExtensionsString);
            XmlHelper.WriteElementToXml(writer,"OtherExtensions",OtherExtensionsString);
            XmlHelper.WriteElementToXml(writer,"ExportRSSMaxDays",ExportRSSMaxDays);
            XmlHelper.WriteElementToXml(writer,"ExportRSSMaxShows",ExportRSSMaxShows);
            XmlHelper.WriteElementToXml(writer,"ExportRSSDaysPast",ExportRSSDaysPast);
            XmlHelper.WriteElementToXml(writer,"KeepTogether",KeepTogether);
            XmlHelper.WriteElementToXml(writer,"KeepTogetherType", (int)keepTogetherMode);
            XmlHelper.WriteElementToXml(writer,"KeepTogetherExtensions", keepTogetherExtensionsString);
            XmlHelper.WriteElementToXml(writer,"LeadingZeroOnSeason",LeadingZeroOnSeason);
            XmlHelper.WriteElementToXml(writer,"ShowInTaskbar",ShowInTaskbar);
            XmlHelper.WriteElementToXml(writer,"IgnoreSamples",IgnoreSamples);
            XmlHelper.WriteElementToXml(writer,"ForceLowercaseFilenames",ForceLowercaseFilenames);
            XmlHelper.WriteElementToXml(writer,"RenameTxtToSub",RenameTxtToSub);
            XmlHelper.WriteElementToXml(writer,"ParallelDownloads",ParallelDownloads);
            XmlHelper.WriteElementToXml(writer,"AutoSelectShowInMyShows",AutoSelectShowInMyShows);
            XmlHelper.WriteElementToXml(writer,"AutoCreateFolders", AutoCreateFolders );
            XmlHelper.WriteElementToXml(writer,"ShowEpisodePictures",ShowEpisodePictures);
            XmlHelper.WriteElementToXml(writer, "HideWtWSpoilers", HideWtWSpoilers);
            XmlHelper.WriteElementToXml(writer, "HideMyShowsSpoilers", HideMyShowsSpoilers);
            XmlHelper.WriteElementToXml(writer,"SpecialsFolderName",SpecialsFolderName);
            XmlHelper.WriteElementToXml(writer,"uTorrentPath",uTorrentPath);
            XmlHelper.WriteElementToXml(writer,"ResumeDatPath",ResumeDatPath);
            XmlHelper.WriteElementToXml(writer,"SearchRSS",SearchRSS);
            XmlHelper.WriteElementToXml(writer,"EpImgs",EpTBNs);
            XmlHelper.WriteElementToXml(writer,"NFOShows",NFOShows);
            XmlHelper.WriteElementToXml(writer,"NFOEpisodes", NFOEpisodes);
            XmlHelper.WriteElementToXml(writer,"KODIImages",KODIImages);
            XmlHelper.WriteElementToXml(writer,"pyTivoMeta",pyTivoMeta);
            XmlHelper.WriteElementToXml(writer,"pyTivoMetaSubFolder",pyTivoMetaSubFolder);
            XmlHelper.WriteElementToXml(writer,"FolderJpg",FolderJpg);
            XmlHelper.WriteElementToXml(writer,"FolderJpgIs",(int) FolderJpgIs);
            XmlHelper.WriteElementToXml(writer,"MonitoredFoldersScanType",(int)MonitoredFoldersScanType);
            XmlHelper.WriteElementToXml(writer,"SelectedKODIType",(int)SelectedKODIType);
            XmlHelper.WriteElementToXml(writer,"CheckuTorrent",CheckuTorrent);
            XmlHelper.WriteElementToXml(writer,"RenameCheck",RenameCheck);
            XmlHelper.WriteElementToXml(writer, "PreventMove", PreventMove);
            XmlHelper.WriteElementToXml(writer,"MissingCheck",MissingCheck);
            XmlHelper.WriteElementToXml(writer, "AutoSearchForDownloadedFiles", AutoSearchForDownloadedFiles);
            XmlHelper.WriteElementToXml(writer, "UpdateFileDates", CorrectFileDates);
            XmlHelper.WriteElementToXml(writer,"SearchLocally",SearchLocally);
            XmlHelper.WriteElementToXml(writer,"LeaveOriginals",LeaveOriginals);
            XmlHelper.WriteElementToXml(writer, "RetainLanguageSpecificSubtitles", RetainLanguageSpecificSubtitles);
            XmlHelper.WriteElementToXml(writer, "ForceBulkAddToUseSettingsOnly", ForceBulkAddToUseSettingsOnly);
            XmlHelper.WriteElementToXml(writer,"LookForDateInFilename",LookForDateInFilename);
            XmlHelper.WriteElementToXml(writer, "AutoMergeEpisodes", AutoMergeDownloadEpisodes);
            XmlHelper.WriteElementToXml(writer, "AutoMergeLibraryEpisodes", AutoMergeLibraryEpisodes);
            XmlHelper.WriteElementToXml(writer,"MonitorFolders",MonitorFolders);
            XmlHelper.WriteElementToXml(writer, "StartupScan", runStartupCheck);
            XmlHelper.WriteElementToXml(writer, "PeriodicScan", runPeriodicCheck);
            XmlHelper.WriteElementToXml(writer, "PeriodicScanHours", periodCheckHours);
            XmlHelper.WriteElementToXml(writer,"RemoveDownloadDirectoriesFiles", RemoveDownloadDirectoriesFiles);
            XmlHelper.WriteElementToXml(writer,"SABAPIKey",SABAPIKey);
            XmlHelper.WriteElementToXml(writer,"CheckSABnzbd",CheckSABnzbd);
            XmlHelper.WriteElementToXml(writer,"SABHostPort",SABHostPort);
            XmlHelper.WriteElementToXml(writer,"PreferredLanguage",PreferredLanguage);
            XmlHelper.WriteElementToXml(writer,"WTWDoubleClick",(int) WTWDoubleClick);
            XmlHelper.WriteElementToXml(writer,"EpJPGs",EpJPGs);
            XmlHelper.WriteElementToXml(writer,"SeriesJpg",SeriesJpg);
            XmlHelper.WriteElementToXml(writer,"Mede8erXML",Mede8erXML);
            XmlHelper.WriteElementToXml(writer,"ShrinkLargeMede8erImages",ShrinkLargeMede8erImages);
            XmlHelper.WriteElementToXml(writer,"FanArtJpg",FanArtJpg);
            XmlHelper.WriteElementToXml(writer,"DeleteEmpty",Tidyup.DeleteEmpty);
            XmlHelper.WriteElementToXml(writer,"DeleteEmptyIsRecycle",Tidyup.DeleteEmptyIsRecycle);
            XmlHelper.WriteElementToXml(writer,"EmptyIgnoreWords",Tidyup.EmptyIgnoreWords);
            XmlHelper.WriteElementToXml(writer,"EmptyIgnoreWordList",Tidyup.EmptyIgnoreWordList);
            XmlHelper.WriteElementToXml(writer,"EmptyIgnoreExtensions",Tidyup.EmptyIgnoreExtensions);
            XmlHelper.WriteElementToXml(writer,"EmptyIgnoreExtensionList",Tidyup.EmptyIgnoreExtensionList);
            XmlHelper.WriteElementToXml(writer,"EmptyMaxSizeCheck",Tidyup.EmptyMaxSizeCheck);
            XmlHelper.WriteElementToXml(writer,"EmptyMaxSizeMB",Tidyup.EmptyMaxSizeMB);
            XmlHelper.WriteElementToXml(writer, "BetaMode", (int)mode);
            XmlHelper.WriteElementToXml(writer, "PercentDirtyUpgrade", upgradeDirtyPercent);
            XmlHelper.WriteElementToXml(writer, "BaseSeasonName", defaultSeasonWord);
            XmlHelper.WriteElementToXml(writer, "SearchSeasonNames", searchSeasonWordsString);
            XmlHelper.WriteElementToXml(writer, "PreferredRSSSearchTerms", preferredRSSSearchTermsString);
            XmlHelper.WriteElementToXml(writer, "BulkAddIgnoreRecycleBin", BulkAddIgnoreRecycleBin);
            XmlHelper.WriteElementToXml(writer, "BulkAddCompareNoVideoFolders", BulkAddCompareNoVideoFolders);
            XmlHelper.WriteElementToXml(writer, "AutoAddMovieTerms", AutoAddMovieTerms);
            XmlHelper.WriteElementToXml(writer, "AutoAddIgnoreSuffixes", AutoAddIgnoreSuffixes);

            writer.WriteStartElement("FNPRegexs");
            foreach (FilenameProcessorRE re in FNPRegexs)
            {
                writer.WriteStartElement("Regex");
                XmlHelper.WriteAttributeToXml(writer,"Enabled",re.Enabled);
                XmlHelper.WriteAttributeToXml(writer,"RE",re.RegExpression);
                XmlHelper.WriteAttributeToXml(writer,"UseFullPath",re.UseFullPath);
                XmlHelper.WriteAttributeToXml(writer,"Notes",re.Notes);
                writer.WriteEndElement(); // Regex
            }
            writer.WriteEndElement(); // FNPRegexs

            writer.WriteStartElement("RSSURLs");
            foreach (string s in RSSURLs) XmlHelper.WriteElementToXml(writer,"URL",s);
            writer.WriteEndElement(); // RSSURLs

            if (ShowStatusColors != null)
            {
                writer.WriteStartElement("ShowStatusTVWColors");
                foreach (KeyValuePair<ShowStatusColoringType, System.Drawing.Color> e in ShowStatusColors)
                {
                    writer.WriteStartElement("ShowStatusTVWColor");
                    // TODO ... Write Meta Flags
                    XmlHelper.WriteAttributeToXml(writer,"IsMeta",e.Key.IsMetaType);
                    XmlHelper.WriteAttributeToXml(writer,"IsShowLevel",e.Key.IsShowLevel);
                    XmlHelper.WriteAttributeToXml(writer,"ShowStatus",e.Key.Status);
                    XmlHelper.WriteAttributeToXml(writer,"Color",Helpers.TranslateColorToHtml(e.Value));
                    writer.WriteEndElement(); //ShowStatusTVWColor
                }
                writer.WriteEndElement(); // ShowStatusTVWColors
            }

            if (Filter != null)
            {
                writer.WriteStartElement("ShowFilters");

                XmlHelper.WriteInfo(writer, "NameFilter", "Name", Filter.ShowName);
                XmlHelper.WriteInfo(writer, "ShowStatusFilter", "ShowStatus", Filter.ShowStatus);
                XmlHelper.WriteInfo(writer, "ShowNetworkFilter", "ShowNetwork", Filter.ShowNetwork);
                XmlHelper.WriteInfo(writer, "ShowRatingFilter", "ShowRating", Filter.ShowRating);

                foreach (string genre in Filter.Genres) XmlHelper.WriteInfo(writer, "GenreFilter", "Genre", genre);
 
                writer.WriteEndElement(); //ShowFilters
            }

            writer.WriteEndElement(); // settings
        }

        internal float PercentDirtyUpgrade()
        {
            return upgradeDirtyPercent;
        }

        public FolderJpgIsType ItemForFolderJpg() => FolderJpgIs;

        public string GetVideoExtensionsString() =>VideoExtensionsString;
        public string GetOtherExtensionsString() => OtherExtensionsString;
        public string GetKeepTogetherString() => keepTogetherExtensionsString;
        
        public bool RunPeriodicCheck() => runPeriodicCheck;
        public int PeriodicCheckPeriod() =>  periodCheckHours * 60* 60 * 1000;
        public bool RunOnStartUp() => runStartupCheck;

        public string GetSeasonSearchTermsString() => searchSeasonWordsString;
        public string GetPreferredRSSSearchTermsString() => preferredRSSSearchTermsString;

        public static bool OKExtensionsString(string s)
        {
            if (string.IsNullOrEmpty(s))
                return true;

            string[] t = s.Split(';');
            foreach (string s2 in t)
            {
                if ((string.IsNullOrEmpty(s2)) || (!s2.StartsWith(".")))
                    return false;
            }
            return true;
        }

        public static string CompulsoryReplacements()
        {
            return "*?<>:/\\|\""; // invalid filename characters, must be in the list!
        }

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

        private static List<string> DefaultRSSURLList()
        {
            List<string> sl = new List<string>();
            return sl;
        }

        public static string[] TabNames()
        {
            return new[] {"MyShows", "Scan", "WTW"};
        }

        public static string TabNameForNumber(int n)
        {
            if ((n >= 0) && (n < TabNames().Length))
                return TabNames()[n];
            return "";
        }

        public static int TabNumberFromName(string n)
        {
            int r = 0;
            if (!string.IsNullOrEmpty(n))
                r = Array.IndexOf(TabNames(), n);
            if (r < 0)
                r = 0;
            return r;
        }

        public bool UsefulExtension(string sn, bool otherExtensionsToo)
        {
            foreach (string s in VideoExtensionsArray)
            {
                if (sn.ToLower() == s)
                    return true;
            }
            if (otherExtensionsToo)
            {
                foreach (string s in OtherExtensionsArray)
                {
                    if (sn.ToLower() == s)
                        return true;
                }
            }

            return false;
        }

        public bool FileHasUsefulExtension(FileInfo file, bool otherExtensionsToo, out string extension)
        {
            foreach (string s in VideoExtensionsArray)
            {
                if (!file.Name.EndsWith(s)) continue;
                extension = s;
                return true;
            }
            if (otherExtensionsToo)
            {
                foreach (string s in OtherExtensionsArray)
                {
                    if (!file.Name.EndsWith(s)) continue;
                    extension = s;
                    return true;
                }
            }

            extension = string.Empty;
            return false;
        }

        public bool KeepExtensionTogether(string extension)
        {
            if (KeepTogether == false) return false;

            if (keepTogetherMode == KeepTogetherModes.All) return true;

            if (keepTogetherMode == KeepTogetherModes.Just) return keepTogetherExtensionsArray.Contains(extension);

            if (keepTogetherMode == KeepTogetherModes.AllBut ) return !keepTogetherExtensionsArray.Contains(extension);

            logger.Error("INVALID USE OF KEEP EXTENSION");
            return false;
        }

        public string BTSearchURL(ProcessedEpisode epi)
        {
            SeriesInfo s = epi?.TheSeries;
            if (s == null)
                return "";

            string url = (epi.SI.UseCustomSearchURL && !string.IsNullOrWhiteSpace(epi.SI.CustomSearchURL))
                ? epi.SI.CustomSearchURL
                : TheSearchers.CurrentSearchURL();
            return CustomName.NameForNoExt(epi, url, true);
        }

        public string FilenameFriendly(string fn)
        {
            if (string.IsNullOrWhiteSpace(fn)) return "";

            foreach (Replacement rep in Replacements)
            {
                if (rep.CaseInsensitive)
                    fn = Regex.Replace(fn, Regex.Escape(rep.This), Regex.Escape(rep.That), RegexOptions.IgnoreCase);
                else
                    fn = fn.Replace(rep.This, rep.That);
            }
            if (ForceLowercaseFilenames)
                fn = fn.ToLower();
            return fn;
        }

        public bool NeedToDownloadBannerFile(){
            // Return true iff we need to download season specific images
            // There are 4 possible reasons
            return (SeasonSpecificFolderJPG() || KODIImages || SeriesJpg || FanArtJpg);
        }

        // ReSharper disable once InconsistentNaming
        public bool SeasonSpecificFolderJPG() {
            return (FolderJpgIsType.SeasonPoster == FolderJpgIs);
        }

        public bool DownloadFrodoImages()
        {
            return (KODIImages && (SelectedKODIType == KODIType.Both || SelectedKODIType == KODIType.Frodo));
        }

        public bool DownloadEdenImages()
        {
            return (KODIImages && (SelectedKODIType == KODIType.Both || SelectedKODIType == KODIType.Eden)); 
        }

        public bool KeepTogetherFilesWithType(string fileExtension)
        {
            if (KeepTogether == false) return false;

            switch (keepTogetherMode)
            {
                case KeepTogetherModes.All: return true;
                case KeepTogetherModes.Just: return keepTogetherExtensionsArray.Contains(fileExtension);
                case KeepTogetherModes.AllBut: return !keepTogetherExtensionsArray.Contains(fileExtension);

            }
            return true;
        }
    }
}
