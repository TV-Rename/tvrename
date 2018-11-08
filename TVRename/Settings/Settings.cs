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
using System.Xml.Linq;

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

        public BetaMode mode = BetaMode.ProductionOnly;
        public float upgradeDirtyPercent = 20;
        public float replaceMargin = 10;
        public bool ReplaceWithBetterQuality = true;
        public KeepTogetherModes keepTogetherMode = KeepTogetherModes.All;

        public bool BulkAddIgnoreRecycleBin = false;
        public bool BulkAddCompareNoVideoFolders = false;

        public string AutoAddMovieTerms = "dvdrip;camrip;screener;dvdscr;r5;bluray";
        public string[] AutoAddMovieTermsArray => AutoAddMovieTerms.Split(';');

        public string PriorityReplaceTerms = "PROPER;REPACK;RERIP";
        public string[] PriorityReplaceTermsArray => PriorityReplaceTerms.Split(';');

        public string AutoAddIgnoreSuffixes = "1080p;720p";
        public string[] AutoAddIgnoreSuffixesArray => AutoAddIgnoreSuffixes.Split(';');

        public string[] keepTogetherExtensionsArray => keepTogetherExtensionsString.Split(';');
        public string keepTogetherExtensionsString = "";

        public string[] subtitleExtensionsArray => subtitleExtensionsString.Split(';');
        public string subtitleExtensionsString = "";

        public string defaultSeasonWord = "Season";

        public string[] searchSeasonWordsArray => searchSeasonWordsString.Split(';');
        public string[] PreferredRSSSearchTerms() => preferredRSSSearchTermsString.Split(';');

        public string searchSeasonWordsString = "Season;Series;Saison;Temporada;Seizoen";
        public string preferredRSSSearchTermsString = "720p;1080p";

        internal bool IncludeBetaUpdates()
        {
            return (mode == BetaMode.BetaToo);
        }

        public string OtherExtensionsString = "";
        public ShowFilter Filter = new ShowFilter();
        public SeasonFilter SeasonFilter = new SeasonFilter();

        private IEnumerable<string> OtherExtensionsArray => OtherExtensionsString.Split(';');

        public int ParallelDownloads =4;
        public List<string> RSSURLs = DefaultRSSURLList();
        public bool RenameCheck = true;
        public bool PreventMove = false;
        public bool RenameTxtToSub = false;
        public readonly List<Replacement> Replacements = DefaultListRE();
        public string ResumeDatPath;
        public int SampleFileMaxSizeMB=50; // sample file must be smaller than this to be ignored
        public bool SearchLocally = true;
        public bool SearchRSS = false;
        public bool SearchJSON = false;
        public bool ShowEpisodePictures = true;
        public bool HideWtWSpoilers = false;
        public bool HideMyShowsSpoilers = false;
        public bool ShowInTaskbar = true;
        public bool AutoSearchForDownloadedFiles = false;
        public string SpecialsFolderName = "Specials";
        public string SeasonFolderFormat = "Season {Season:2}";
        public int StartupTab;
        public Searchers TheSearchers = new Searchers();

        public string SearchJSONURL= "https://eztv.ag/api/get-torrents?imdb_id=";
        public string SearchJSONRootNode = "torrents";
        public string SearchJSONFilenameToken = "filename";
        public string SearchJSONURLToken = "torrent_url";

        public string[] VideoExtensionsArray => VideoExtensionsString.Split(';');
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

        private TVSettings()
        {
            SetToDefaults();
        }

        private void SetToDefaults()
        {
            // defaults that aren't handled with default initialisers
            Ignore = new List<IgnoreItem>();
            DownloadFolders = new List<string>();
            IgnoreFolders = new List<string>();
            LibraryFolders = new List<string>();
            IgnoredAutoAddHints = new List<string>();

            VideoExtensionsString =
                ".avi;.mpg;.mpeg;.mkv;.mp4;.wmv;.divx;.ogm;.qt;.rm;.m4v;.webm;.vob;.ovg;.ogg;.mov;.m4p;.3gp";

            OtherExtensionsString = ".srt;.nfo;.txt;.tbn";
            keepTogetherExtensionsString = ".srt;.nfo;.txt;.tbn";
            subtitleExtensionsString = ".srt;.sub;.sbv;.idx";

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
                new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"uTorrent\resume.dat"));

            ResumeDatPath = f2.Exists ? f2.FullName : "";
        }

        public void WriteXML(XmlWriter writer)
        {
            writer.WriteStartElement("Settings");
            XmlHelper.WriteElementToXml(writer, "BGDownload", BGDownload);
            XmlHelper.WriteElementToXml(writer, "OfflineMode", OfflineMode);
            XmlHelper.WriteElementToXml(writer, "DetailedRSSJSONLogging", DetailedRSSJSONLogging);
            XmlHelper.WriteElementToXml(writer, "ReplaceWithBetterQuality", ReplaceWithBetterQuality);
            XmlHelper.WriteElementToXml(writer, "ExportWTWRSS", ExportWTWRSS);
            XmlHelper.WriteElementToXml(writer, "ExportWTWRSSTo", ExportWTWRSSTo);
            XmlHelper.WriteElementToXml(writer, "ExportWTWICAL", ExportWTWICAL);
            XmlHelper.WriteElementToXml(writer, "ExportWTWICALTo", ExportWTWICALTo);
            XmlHelper.WriteElementToXml(writer, "ExportWTWXML", ExportWTWXML);
            XmlHelper.WriteElementToXml(writer, "ExportWTWXMLTo", ExportWTWXMLTo);
            XmlHelper.WriteElementToXml(writer, "WTWRecentDays", WTWRecentDays);
            XmlHelper.WriteElementToXml(writer, "ExportMissingXML", ExportMissingXML);
            XmlHelper.WriteElementToXml(writer, "ExportMissingXMLTo", ExportMissingXMLTo);
            XmlHelper.WriteElementToXml(writer, "ExportMissingCSV", ExportMissingCSV);
            XmlHelper.WriteElementToXml(writer, "ExportMissingCSVTo", ExportMissingCSVTo);
            XmlHelper.WriteElementToXml(writer, "ExportRenamingXML", ExportRenamingXML);
            XmlHelper.WriteElementToXml(writer, "ExportRenamingXMLTo", ExportRenamingXMLTo);
            XmlHelper.WriteElementToXml(writer, "ExportRecentM3U", ExportRecentM3U);
            XmlHelper.WriteElementToXml(writer, "ExportRecentM3UTo", ExportRecentM3UTo);
            XmlHelper.WriteElementToXml(writer, "ExportRecentASX", ExportRecentASX);
            XmlHelper.WriteElementToXml(writer, "ExportRecentASXTo", ExportRecentASXTo);
            XmlHelper.WriteElementToXml(writer, "ExportRecentWPL", ExportRecentWPL);
            XmlHelper.WriteElementToXml(writer, "ExportRecentWPLTo", ExportRecentWPLTo);
            XmlHelper.WriteElementToXml(writer, "ExportRecentXSPF", ExportRecentXSPF);
            XmlHelper.WriteElementToXml(writer, "ExportRecentXSPFTo", ExportRecentXSPFTo);
            XmlHelper.WriteElementToXml(writer, "ExportShowsTXT", ExportShowsTXT);
            XmlHelper.WriteElementToXml(writer, "ExportShowsTXTTo", ExportShowsTXTTo);
            XmlHelper.WriteElementToXml(writer, "ExportShowsHTML", ExportShowsHTML);
            XmlHelper.WriteElementToXml(writer, "ExportShowsHTMLTo", ExportShowsHTMLTo);
            XmlHelper.WriteElementToXml(writer, "ExportFOXML", ExportFOXML);
            XmlHelper.WriteElementToXml(writer, "ExportFOXMLTo", ExportFOXMLTo);
            XmlHelper.WriteElementToXml(writer, "StartupTab2", TabNameForNumber(StartupTab));
            XmlHelper.WriteElementToXml(writer, "NamingStyle", NamingStyle.StyleString);
            XmlHelper.WriteElementToXml(writer, "NotificationAreaIcon", NotificationAreaIcon);
            XmlHelper.WriteElementToXml(writer, "VideoExtensions", VideoExtensionsString);
            XmlHelper.WriteElementToXml(writer, "OtherExtensions", OtherExtensionsString);
            XmlHelper.WriteElementToXml(writer, "SubtitleExtensions", subtitleExtensionsString);
            XmlHelper.WriteElementToXml(writer, "ExportRSSMaxDays", ExportRSSMaxDays);
            XmlHelper.WriteElementToXml(writer, "ExportRSSMaxShows", ExportRSSMaxShows);
            XmlHelper.WriteElementToXml(writer, "ExportRSSDaysPast", ExportRSSDaysPast);
            XmlHelper.WriteElementToXml(writer, "KeepTogether", KeepTogether);
            XmlHelper.WriteElementToXml(writer, "KeepTogetherType", (int) keepTogetherMode);
            XmlHelper.WriteElementToXml(writer, "KeepTogetherExtensions", keepTogetherExtensionsString);
            XmlHelper.WriteElementToXml(writer, "LeadingZeroOnSeason", LeadingZeroOnSeason);
            XmlHelper.WriteElementToXml(writer, "ShowInTaskbar", ShowInTaskbar);
            XmlHelper.WriteElementToXml(writer, "IgnoreSamples", IgnoreSamples);
            XmlHelper.WriteElementToXml(writer, "ForceLowercaseFilenames", ForceLowercaseFilenames);
            XmlHelper.WriteElementToXml(writer, "RenameTxtToSub", RenameTxtToSub);
            XmlHelper.WriteElementToXml(writer, "ParallelDownloads", ParallelDownloads);
            XmlHelper.WriteElementToXml(writer, "AutoSelectShowInMyShows", AutoSelectShowInMyShows);
            XmlHelper.WriteElementToXml(writer, "AutoCreateFolders", AutoCreateFolders);
            XmlHelper.WriteElementToXml(writer, "ShowEpisodePictures", ShowEpisodePictures);
            XmlHelper.WriteElementToXml(writer, "HideWtWSpoilers", HideWtWSpoilers);
            XmlHelper.WriteElementToXml(writer, "HideMyShowsSpoilers", HideMyShowsSpoilers);
            XmlHelper.WriteElementToXml(writer, "SpecialsFolderName", SpecialsFolderName);
            XmlHelper.WriteElementToXml(writer, "SeasonFolderFormat", SeasonFolderFormat);
            XmlHelper.WriteElementToXml(writer, "uTorrentPath", uTorrentPath);
            XmlHelper.WriteElementToXml(writer, "ResumeDatPath", ResumeDatPath);
            XmlHelper.WriteElementToXml(writer, "SearchRSS", SearchRSS);
            XmlHelper.WriteElementToXml(writer, "SearchJSON", SearchJSON);
            XmlHelper.WriteElementToXml(writer, "EpImgs", EpTBNs);
            XmlHelper.WriteElementToXml(writer, "NFOShows", NFOShows);
            XmlHelper.WriteElementToXml(writer, "NFOEpisodes", NFOEpisodes);
            XmlHelper.WriteElementToXml(writer, "KODIImages", KODIImages);
            XmlHelper.WriteElementToXml(writer, "pyTivoMeta", pyTivoMeta);
            XmlHelper.WriteElementToXml(writer, "pyTivoMetaSubFolder", pyTivoMetaSubFolder);
            XmlHelper.WriteElementToXml(writer, "wdLiveTvMeta", wdLiveTvMeta);
            XmlHelper.WriteElementToXml(writer, "FolderJpg", FolderJpg);
            XmlHelper.WriteElementToXml(writer, "FolderJpgIs", (int) FolderJpgIs);
            XmlHelper.WriteElementToXml(writer, "MonitoredFoldersScanType", (int) MonitoredFoldersScanType);
            XmlHelper.WriteElementToXml(writer, "CheckuTorrent", CheckuTorrent);
            XmlHelper.WriteElementToXml(writer, "CheckqBitTorrent", CheckqBitTorrent);
            XmlHelper.WriteElementToXml(writer, "qBitTorrentHost", qBitTorrentHost);
            XmlHelper.WriteElementToXml(writer, "qBitTorrentPort", qBitTorrentPort);
            XmlHelper.WriteElementToXml(writer, "RenameCheck", RenameCheck);
            XmlHelper.WriteElementToXml(writer, "PreventMove", PreventMove);
            XmlHelper.WriteElementToXml(writer, "MissingCheck", MissingCheck);
            XmlHelper.WriteElementToXml(writer, "AutoSearchForDownloadedFiles", AutoSearchForDownloadedFiles);
            XmlHelper.WriteElementToXml(writer, "UpdateFileDates", CorrectFileDates);
            XmlHelper.WriteElementToXml(writer, "SearchLocally", SearchLocally);
            XmlHelper.WriteElementToXml(writer, "LeaveOriginals", LeaveOriginals);
            XmlHelper.WriteElementToXml(writer, "RetainLanguageSpecificSubtitles", RetainLanguageSpecificSubtitles);
            XmlHelper.WriteElementToXml(writer, "ForceBulkAddToUseSettingsOnly", ForceBulkAddToUseSettingsOnly);
            XmlHelper.WriteElementToXml(writer, "LookForDateInFilename", LookForDateInFilename);
            XmlHelper.WriteElementToXml(writer, "AutoMergeEpisodes", AutoMergeDownloadEpisodes);
            XmlHelper.WriteElementToXml(writer, "AutoMergeLibraryEpisodes", AutoMergeLibraryEpisodes);
            XmlHelper.WriteElementToXml(writer, "MonitorFolders", MonitorFolders);
            XmlHelper.WriteElementToXml(writer, "StartupScan", runStartupCheck);
            XmlHelper.WriteElementToXml(writer, "PeriodicScan", runPeriodicCheck);
            XmlHelper.WriteElementToXml(writer, "PeriodicScanHours", periodCheckHours);
            XmlHelper.WriteElementToXml(writer, "RemoveDownloadDirectoriesFiles", RemoveDownloadDirectoriesFiles);
            XmlHelper.WriteElementToXml(writer, "DeleteShowFromDisk", DeleteShowFromDisk);
            XmlHelper.WriteElementToXml(writer, "SABAPIKey", SABAPIKey);
            XmlHelper.WriteElementToXml(writer, "CheckSABnzbd", CheckSABnzbd);
            XmlHelper.WriteElementToXml(writer, "SABHostPort", SABHostPort);
            XmlHelper.WriteElementToXml(writer, "PreferredLanguage", PreferredLanguageCode);
            XmlHelper.WriteElementToXml(writer, "WTWDoubleClick", (int) WTWDoubleClick);
            XmlHelper.WriteElementToXml(writer, "EpJPGs", EpJPGs);
            XmlHelper.WriteElementToXml(writer, "SeriesJpg", SeriesJpg);
            XmlHelper.WriteElementToXml(writer, "Mede8erXML", Mede8erXML);
            XmlHelper.WriteElementToXml(writer, "ShrinkLargeMede8erImages", ShrinkLargeMede8erImages);
            XmlHelper.WriteElementToXml(writer, "FanArtJpg", FanArtJpg);
            XmlHelper.WriteElementToXml(writer, "DeleteEmpty", Tidyup.DeleteEmpty);
            XmlHelper.WriteElementToXml(writer, "DeleteEmptyIsRecycle", Tidyup.DeleteEmptyIsRecycle);
            XmlHelper.WriteElementToXml(writer, "EmptyIgnoreWords", Tidyup.EmptyIgnoreWords);
            XmlHelper.WriteElementToXml(writer, "EmptyIgnoreWordList", Tidyup.EmptyIgnoreWordList);
            XmlHelper.WriteElementToXml(writer, "EmptyIgnoreExtensions", Tidyup.EmptyIgnoreExtensions);
            XmlHelper.WriteElementToXml(writer, "EmptyIgnoreExtensionList", Tidyup.EmptyIgnoreExtensionList);
            XmlHelper.WriteElementToXml(writer, "EmptyMaxSizeCheck", Tidyup.EmptyMaxSizeCheck);
            XmlHelper.WriteElementToXml(writer, "EmptyMaxSizeMB", Tidyup.EmptyMaxSizeMB);
            XmlHelper.WriteElementToXml(writer, "BetaMode", (int) mode);
            XmlHelper.WriteElementToXml(writer, "PercentDirtyUpgrade", upgradeDirtyPercent);
            XmlHelper.WriteElementToXml(writer, "PercentBetter", replaceMargin);
            XmlHelper.WriteElementToXml(writer, "BaseSeasonName", defaultSeasonWord);
            XmlHelper.WriteElementToXml(writer, "SearchSeasonNames", searchSeasonWordsString);
            XmlHelper.WriteElementToXml(writer, "PreferredRSSSearchTerms", preferredRSSSearchTermsString);
            XmlHelper.WriteElementToXml(writer, "BulkAddIgnoreRecycleBin", BulkAddIgnoreRecycleBin);
            XmlHelper.WriteElementToXml(writer, "BulkAddCompareNoVideoFolders", BulkAddCompareNoVideoFolders);
            XmlHelper.WriteElementToXml(writer, "AutoAddMovieTerms", AutoAddMovieTerms);
            XmlHelper.WriteElementToXml(writer, "AutoAddIgnoreSuffixes", AutoAddIgnoreSuffixes);
            XmlHelper.WriteElementToXml(writer, "SearchJSONURL", SearchJSONURL);
            XmlHelper.WriteElementToXml(writer, "SearchJSONRootNode", SearchJSONRootNode);
            XmlHelper.WriteElementToXml(writer, "SearchJSONFilenameToken", SearchJSONFilenameToken);
            XmlHelper.WriteElementToXml(writer, "SearchJSONURLToken", SearchJSONURLToken);
            XmlHelper.WriteElementToXml(writer, "PriorityReplaceTerms", PriorityReplaceTerms);

            TheSearchers.WriteXml(writer);
            writer.WriteStartElement("Replacements");
            foreach (Replacement R in Replacements)
            {
                writer.WriteStartElement("Replace");
                XmlHelper.WriteAttributeToXml(writer, "This", R.This);
                XmlHelper.WriteAttributeToXml(writer, "That", R.That);
                XmlHelper.WriteAttributeToXml(writer, "CaseInsensitive", R.CaseInsensitive ? "Y" : "N");
                writer.WriteEndElement(); //Replace
            }

            writer.WriteEndElement(); //Replacements

            writer.WriteStartElement("FNPRegexs");
            foreach (FilenameProcessorRE re in FNPRegexs)
            {
                writer.WriteStartElement("Regex");
                XmlHelper.WriteAttributeToXml(writer, "Enabled", re.Enabled);
                XmlHelper.WriteAttributeToXml(writer, "RE", re.RegExpression);
                XmlHelper.WriteAttributeToXml(writer, "UseFullPath", re.UseFullPath);
                XmlHelper.WriteAttributeToXml(writer, "Notes", re.Notes);
                writer.WriteEndElement(); // Regex
            }

            writer.WriteEndElement(); // FNPRegexs

            XmlHelper.WriteStringsToXml(RSSURLs,writer,"RSSURLs", "URL");

            if (ShowStatusColors != null)
            {
                writer.WriteStartElement("ShowStatusTVWColors");
                foreach (KeyValuePair<ShowStatusColoringType, System.Drawing.Color> e in ShowStatusColors)
                {
                    writer.WriteStartElement("ShowStatusTVWColor");
                    // TODO ... Write Meta Flags
                    XmlHelper.WriteAttributeToXml(writer, "IsMeta", e.Key.IsMetaType);
                    XmlHelper.WriteAttributeToXml(writer, "IsShowLevel", e.Key.IsShowLevel);
                    XmlHelper.WriteAttributeToXml(writer, "ShowStatus", e.Key.Status);
                    XmlHelper.WriteAttributeToXml(writer, "Color", Helpers.TranslateColorToHtml(e.Value));
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

            if (SeasonFilter != null)
            {
                writer.WriteStartElement("SeasonFilters");
                XmlHelper.WriteElementToXml(writer, "SeasonIgnoredFilter", SeasonFilter.HideIgnoredSeasons);
                writer.WriteEndElement(); //SeasonFilters
            }

            writer.WriteEndElement(); // settings
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

        private static string[] TabNames() => new[] {"MyShows", "Scan", "WTW"};

        private static string TabNameForNumber(int n)
        {
            if ((n >= 0) && (n < TabNames().Length))
                return TabNames()[n];

            return "";
        }

        private static int TabNumberFromName(string n)
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
            if (VideoExtensionsArray.Any(s => string.Equals(sn, s, StringComparison.CurrentCultureIgnoreCase)))
            {
                return true;
            }

            if (otherExtensionsToo)
            {
                return OtherExtensionsArray.Any(s => string.Equals(sn, s, StringComparison.CurrentCultureIgnoreCase));
            }

            return false;
        }

        public bool FileHasUsefulExtension(FileInfo file, bool otherExtensionsToo, out string extension)
        {
            foreach (string s in VideoExtensionsArray)
            {
                if (!file.Name.EndsWith(s, StringComparison.InvariantCultureIgnoreCase)) continue;
                extension = s;
                return true;
            }

            if (otherExtensionsToo)
            {
                foreach (string s in OtherExtensionsArray)
                {
                    if (!file.Name.EndsWith(s, StringComparison.InvariantCultureIgnoreCase)) continue;
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

            if (keepTogetherMode == KeepTogetherModes.AllBut) return !keepTogetherExtensionsArray.Contains(extension);

            logger.Error("INVALID USE OF KEEP EXTENSION");
            return false;
        }

        public string BTSearchURL(ProcessedEpisode epi)
        {
            SeriesInfo s = epi?.TheSeries;
            if (s == null)
                return "";

            string url = (epi.Show.UseCustomSearchUrl && !string.IsNullOrWhiteSpace(epi.Show.CustomSearchUrl))
                ? epi.Show.CustomSearchUrl
                : TheSearchers.CurrentSearchUrl();

            return CustomEpisodeName.NameForNoExt(epi, url, true);
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

        public bool NeedToDownloadBannerFile()
        {
            // Return true iff we need to download season specific images
            // There are 4 possible reasons
            return (SeasonSpecificFolderJPG() || KODIImages || SeriesJpg || FanArtJpg);
        }

        // ReSharper disable once InconsistentNaming
        public bool SeasonSpecificFolderJPG()
        {
            return (FolderJpgIsType.SeasonPoster == FolderJpgIs);
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

            public IEnumerable<string> EmptyIgnoreExtensionsArray => EmptyIgnoreExtensionList.Split(';');
            public IEnumerable<string> EmptyIgnoreWordsArray => EmptyIgnoreWordList.Split(';');

            public void load(XElement xmlSettings)
            {
                DeleteEmpty = xmlSettings.ExtractBool("DeleteEmpty")??false;
                DeleteEmptyIsRecycle = xmlSettings.ExtractBool("DeleteEmptyIsRecycle")??true;
                EmptyIgnoreWords = xmlSettings.ExtractBool("EmptyIgnoreWords") ?? false;
                EmptyIgnoreWordList = xmlSettings.ExtractString("EmptyIgnoreWordList")??"sample";
                EmptyIgnoreExtensions = xmlSettings.ExtractBool("EmptyIgnoreExtensions") ?? false;
                EmptyIgnoreExtensionList = xmlSettings.ExtractString("EmptyIgnoreExtensionList")?? ".nzb;.nfo;.par2;.txt;.srt";
                EmptyMaxSizeCheck = xmlSettings.ExtractBool("EmptyMaxSizeCheck") ?? true;
                EmptyMaxSizeMB = xmlSettings.ExtractInt("EmptyMaxSizeMB") ?? 100;
            }
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

            public readonly bool IsMetaType;
            public readonly bool IsShowLevel;
            public readonly string Status;

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
                            (ShowItem.ShowAirStatus) Enum.Parse(typeof(ShowItem.ShowAirStatus), Status, true);

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
                    else
                    {
                        Season.SeasonStatus status =
                            (Season.SeasonStatus) Enum.Parse(typeof(Season.SeasonStatus), Status);

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

        public void load(XElement xmlSettings)
        {
            SetToDefaults();
            BGDownload = xmlSettings.ExtractBool("BGDownload") ?? false;
            OfflineMode = xmlSettings.ExtractBool("OfflineMode")??false;
            DetailedRSSJSONLogging = xmlSettings.ExtractBool("DetailedRSSJSONLogging") ?? false;
            ReplaceWithBetterQuality = xmlSettings.ExtractBool("ReplaceWithBetterQuality")??true;
            ExportWTWRSSTo = xmlSettings.ExtractString("ExportWTWRSSTo");
            ExportWTWXML = xmlSettings.ExtractBool("ExportWTWXML")??false;
            ExportWTWXMLTo = xmlSettings.ExtractString("ExportWTWXMLTo");
            ExportWTWICAL = xmlSettings.ExtractBool("ExportWTWICAL")??false;
            ExportWTWICALTo = xmlSettings.ExtractString("ExportWTWICALTo");
            WTWRecentDays = xmlSettings.ExtractInt("WTWRecentDays")??7;
            StartupTab = TabNumberFromName(xmlSettings.ExtractString("StartupTab2"));
            NamingStyle.StyleString = xmlSettings.ExtractString("NamingStyle") ??
                                      CustomEpisodeName.OldNStyle(
                                          xmlSettings.ExtractInt("DefaultNamingStyle")??1); // old naming style
            NotificationAreaIcon = xmlSettings.ExtractBool("NotificationAreaIcon") ?? false;
            VideoExtensionsString = xmlSettings.ExtractString("VideoExtensions",
                                    xmlSettings.ExtractString("GoodExtensions",
                                        ".avi;.mpg;.mpeg;.mkv;.mp4;.wmv;.divx;.ogm;.qt;.rm;.m4v;.webm;.vob;.ovg;.ogg;.mov;.m4p;.3gp"));
            OtherExtensionsString = xmlSettings.ExtractString("OtherExtensions", ".srt;.nfo;.txt;.tbn");
            subtitleExtensionsString = xmlSettings.ExtractString("SubtitleExtensions", ".srt;.sub;.sbv;.idx");
            ExportRSSMaxDays = xmlSettings.ExtractInt("ExportRSSMaxDays")??7;
            ExportRSSMaxShows = xmlSettings.ExtractInt("ExportRSSMaxShows")??10;
            ExportRSSDaysPast = xmlSettings.ExtractInt("ExportRSSDaysPast")??0;
            KeepTogether = xmlSettings.ExtractBool("KeepTogether")??true;
            LeadingZeroOnSeason = xmlSettings.ExtractBool("LeadingZeroOnSeason")??false;
            ShowInTaskbar = xmlSettings.ExtractBool("ShowInTaskbar")??true;
            RenameTxtToSub = xmlSettings.ExtractBool("RenameTxtToSub")??false;
            ShowEpisodePictures = xmlSettings.ExtractBool("ShowEpisodePictures")??true;
            HideWtWSpoilers = xmlSettings.ExtractBool("HideWtWSpoilers") ?? false;
            HideMyShowsSpoilers = xmlSettings.ExtractBool("HideMyShowsSpoilers") ?? false;
            AutoCreateFolders = xmlSettings.ExtractBool("AutoCreateFolders") ?? false;
            AutoSelectShowInMyShows = xmlSettings.ExtractBool("AutoSelectShowInMyShows")??true;
            SpecialsFolderName = xmlSettings.ExtractString("SpecialsFolderName", "Specials");
            SeasonFolderFormat = xmlSettings.ExtractString("SeasonFolderFormat");
            SearchJSONURL = xmlSettings.ExtractString("SearchJSONURL", "https://eztv.ag/api/get-torrents?imdb_id=");
            SearchJSONRootNode = xmlSettings.ExtractString("SearchJSONRootNode", "torrents");
            SearchJSONFilenameToken = xmlSettings.ExtractString("SearchJSONFilenameToken","filename");
            SearchJSONURLToken = xmlSettings.ExtractString("SearchJSONURLToken", "torrent_url");
            SABAPIKey = xmlSettings.ExtractString("SABAPIKey");
            CheckSABnzbd = xmlSettings.ExtractBool("CheckSABnzbd")??false;
            SABHostPort = xmlSettings.ExtractString("SABHostPort");
            PreferredLanguageCode = xmlSettings.ExtractString("PreferredLanguage","en");
            WTWDoubleClick = xmlSettings.ExtractInt("WTWDoubleClick") ==null
                ? WTWDoubleClickAction.Scan
                : (WTWDoubleClickAction) xmlSettings.ExtractInt("WTWDoubleClick");
            ExportMissingXML = xmlSettings.ExtractBool("ExportMissingXML") ?? false;
            ExportMissingXMLTo = xmlSettings.ExtractString("ExportMissingXMLTo");
            ExportRecentXSPF = xmlSettings.ExtractBool("ExportRecentXSPF") ?? false;
            ExportRecentXSPFTo = xmlSettings.ExtractString("ExportRecentXSPFTo");
            ExportRecentM3U = xmlSettings.ExtractBool("ExportRecentM3U") ?? false;
            ExportRecentM3UTo = xmlSettings.ExtractString("ExportRecentM3UTo");
            ExportRecentASX = xmlSettings.ExtractBool("ExportRecentASX") ?? false;
            ExportRecentASXTo = xmlSettings.ExtractString("ExportRecentASXTo");
            ExportRecentWPL = xmlSettings.ExtractBool("ExportRecentWPL") ?? false;
            ExportRecentWPLTo = xmlSettings.ExtractString("ExportRecentWPLTo");
            ExportMissingCSV = xmlSettings.ExtractBool("ExportMissingCSV") ?? false;
            ExportMissingCSVTo = xmlSettings.ExtractString("ExportMissingCSVTo");
            ExportRenamingXML = xmlSettings.ExtractBool("ExportRenamingXML") ?? false;
            ExportRenamingXMLTo = xmlSettings.ExtractString("ExportRenamingXMLTo");
            ExportFOXML = xmlSettings.ExtractBool("ExportFOXML") ?? false;
            ExportFOXMLTo = xmlSettings.ExtractString("ExportFOXMLTo");
            ExportShowsTXT = xmlSettings.ExtractBool("ExportShowsTXT") ?? false;
            ExportShowsTXTTo = xmlSettings.ExtractString("ExportShowsTXTTo");
            ExportShowsHTML = xmlSettings.ExtractBool("ExportShowsHTML") ?? false;
            ExportShowsHTMLTo = xmlSettings.ExtractString("ExportShowsHTMLTo");
            ForceLowercaseFilenames = xmlSettings.ExtractBool("ForceLowercaseFilenames")??false;
            IgnoreSamples = xmlSettings.ExtractBool("IgnoreSamples") ?? true;
            SampleFileMaxSizeMB = xmlSettings.ExtractInt("SampleFileMaxSizeMB")??50;
            ParallelDownloads = xmlSettings.ExtractInt("ParallelDownloads")??4;
            uTorrentPath = xmlSettings.ExtractString("uTorrentPath");
            ResumeDatPath = xmlSettings.ExtractString("ResumeDatPath");
            SearchRSS = xmlSettings.ExtractBool("SearchRSS") ?? false;
            SearchJSON = xmlSettings.ExtractBool("SearchJSON") ?? false;
            EpTBNs = xmlSettings.ExtractBool("EpImgs")??false;
            NFOShows = xmlSettings.ExtractBool("NFOShows") ?? xmlSettings.ExtractBool("NFOs") ?? false;
            NFOEpisodes = xmlSettings.ExtractBool("NFOEpisodes") ?? xmlSettings.ExtractBool("NFOs") ?? false;
            KODIImages = xmlSettings.ExtractBool("KODIImages") ??
                            xmlSettings.ExtractBool("XBMCImages") ?? false; //Backward Compatibilty
            pyTivoMeta = xmlSettings.ExtractBool("pyTivoMeta") ?? false;
            wdLiveTvMeta = xmlSettings.ExtractBool("wdLiveTvMeta") ?? false;
            pyTivoMetaSubFolder = xmlSettings.ExtractBool("pyTivoMetaSubFolder") ?? false;
            FolderJpg = xmlSettings.ExtractBool("FolderJpg") ?? false;
            FolderJpgIs = xmlSettings.ExtractInt("FolderJpgIs")==null
                ? FolderJpgIsType.Poster
                : (FolderJpgIsType) xmlSettings.ExtractInt("FolderJpgIs");
            MonitoredFoldersScanType = xmlSettings.ExtractInt("MonitoredFoldersScanType")==null
                ? ScanType.Full
                : (ScanType) xmlSettings.ExtractInt("MonitoredFoldersScanType");
            RenameCheck = xmlSettings.ExtractBool("RenameCheck") ?? true;
            PreventMove = xmlSettings.ExtractBool("PreventMove") ?? false;
            CheckuTorrent = xmlSettings.ExtractBool("CheckuTorrent") ?? false;
            CheckqBitTorrent = xmlSettings.ExtractBool("CheckqBitTorrent") ?? false;
            qBitTorrentHost = xmlSettings.ExtractString("qBitTorrentHost", "localhost");
            qBitTorrentPort = xmlSettings.ExtractString("qBitTorrentPort", "8080");
            MissingCheck = xmlSettings.ExtractBool("MissingCheck")??true;
            CorrectFileDates = xmlSettings.ExtractBool("UpdateFileDates")??false;
            SearchLocally = xmlSettings.ExtractBool("SearchLocally")??true;
            LeaveOriginals = xmlSettings.ExtractBool("LeaveOriginals")??false;
            AutoSearchForDownloadedFiles = xmlSettings.ExtractBool("AutoSearchForDownloadedFiles") ?? false;
            LookForDateInFilename = xmlSettings.ExtractBool("LookForDateInFilename")??false;
            AutoMergeDownloadEpisodes = xmlSettings.ExtractBool("AutoMergeEpisodes")??false;
            AutoMergeLibraryEpisodes = xmlSettings.ExtractBool("AutoMergeLibraryEpisodes") ?? false;
            RetainLanguageSpecificSubtitles = xmlSettings.ExtractBool("RetainLanguageSpecificSubtitles")??true;
            ForceBulkAddToUseSettingsOnly = xmlSettings.ExtractBool("ForceBulkAddToUseSettingsOnly")??false;
            MonitorFolders = xmlSettings.ExtractBool("MonitorFolders") ?? false;
            runStartupCheck = xmlSettings.ExtractBool("StartupScan") ?? false;
            runPeriodicCheck = xmlSettings.ExtractBool("PeriodicScan") ?? false;
            periodCheckHours = xmlSettings.ExtractInt("PeriodicScanHours")??1;
            RemoveDownloadDirectoriesFiles = xmlSettings.ExtractBool("RemoveDownloadDirectoriesFiles") ?? false;
            DeleteShowFromDisk = xmlSettings.ExtractBool("DeleteShowFromDisk")??true;
            EpJPGs = xmlSettings.ExtractBool("EpJPGs") ?? false;
            SeriesJpg = xmlSettings.ExtractBool("SeriesJpg") ?? false;
            Mede8erXML = xmlSettings.ExtractBool("Mede8erXML") ?? false;
            ShrinkLargeMede8erImages = xmlSettings.ExtractBool("ShrinkLargeMede8erImages") ?? false;
            FanArtJpg = xmlSettings.ExtractBool("FanArtJpg") ?? false;
            BulkAddIgnoreRecycleBin = xmlSettings.ExtractBool("BulkAddIgnoreRecycleBin") ?? false;
            BulkAddCompareNoVideoFolders = xmlSettings.ExtractBool("BulkAddCompareNoVideoFolders") ?? false;
            AutoAddMovieTerms = xmlSettings.ExtractString("AutoAddMovieTerms", "dvdrip;camrip;screener;dvdscr;r5;bluray");
            AutoAddIgnoreSuffixes = xmlSettings.ExtractString("AutoAddIgnoreSuffixes", "1080p;720p");
            PriorityReplaceTerms = xmlSettings.ExtractString("PriorityReplaceTerms", "PROPER;REPACK;RERIP");
            mode = xmlSettings.ExtractInt("BetaMode")==null
                ? BetaMode.ProductionOnly
                : (BetaMode) xmlSettings.ExtractInt("BetaMode");
            upgradeDirtyPercent = xmlSettings.ExtractFloat("PercentDirtyUpgrade")??20;
            replaceMargin = xmlSettings.ExtractFloat("PercentBetter")??10;
            defaultSeasonWord = xmlSettings.ExtractString("BaseSeasonName", "Season");
            searchSeasonWordsString = xmlSettings.ExtractString("SearchSeasonNames", "Season;Series;Saison;Temporada;Seizoen");
            preferredRSSSearchTermsString = xmlSettings.ExtractString("PreferredRSSSearchTerms", "720p;1080p");
            keepTogetherMode = xmlSettings.ExtractInt("KeepTogetherType") == null
                ? KeepTogetherModes.All
                : (KeepTogetherModes) xmlSettings.ExtractInt("KeepTogetherType");
            keepTogetherExtensionsString = xmlSettings.ExtractString("KeepTogetherExtensions", ".srt;.nfo;.txt;.tbn");
            ExportWTWRSS = xmlSettings.ExtractBool("ExportWTWRSS") ?? false;

            Tidyup.load(xmlSettings);
            RSSURLs = xmlSettings.Descendants("RSSURLs").First().ReadStringsFromXml("URL");
            TheSearchers = new Searchers(xmlSettings.Descendants("TheSearchers").First());

            Replacements.Clear();
            foreach (XElement rep in xmlSettings.Descendants("Replacements").First().Descendants("Replace"))
            {
                Replacements.Add(new Replacement(rep.Attribute("This")?.Value, rep.Attribute("That")?.Value, rep.Attribute("CaseInsensitive")?.Value == "Y"));
            }

            FNPRegexs.Clear();
            foreach (XElement rep in xmlSettings.Descendants("FNPRegexs").First().Descendants("Regex"))
            {
                FNPRegexs.Add(new FilenameProcessorRE(
                    XmlConvert.ToBoolean(rep.Attribute("Enabled")?.Value??"false"),
                    rep.Attribute("RE")?.Value,
                    XmlConvert.ToBoolean(rep.Attribute("UseFullPath")?.Value ?? "false"),
                    rep.Attribute("Notes")?.Value
                    ));
            }

            ShowStatusColors = new ShowStatusColoringTypeList();
            foreach (XElement rep in xmlSettings.Descendants("ShowStatusTVWColors").First().Descendants("ShowStatusTVWColor"))
            {
                string showStatus = rep.Attribute("ShowStatus")?.Value;
                bool isMeta = bool.Parse(rep.Attribute("IsMeta")?.Value??"false");
                bool isShowLevel = bool.Parse(rep.Attribute("IsShowLevel")?.Value??"true");
                ShowStatusColoringType type = new ShowStatusColoringType(isMeta, isShowLevel, showStatus);
                string color = rep.Attribute("Color")?.Value;
                if (string.IsNullOrEmpty(color)) continue;
                System.Drawing.Color c = System.Drawing.ColorTranslator.FromHtml(color);
                ShowStatusColors.Add(type, c);
            }

            SeasonFilter = new SeasonFilter
            {
                HideIgnoredSeasons = XmlConvert.ToBoolean(xmlSettings.Descendants("SeasonFilters")
                    .Descendants("SeasonIgnoredFilter").First().Value)
            };

            Filter = new ShowFilter
            {
                ShowName = xmlSettings.Descendants("ShowFilters").Descendants("ShowNameFilter").Attributes("ShowName").FirstOrDefault()?.Value,
                ShowStatus = xmlSettings.Descendants("ShowFilters").Descendants("ShowStatusFilter").Attributes("ShowStatus").FirstOrDefault()?.Value,
                ShowRating = xmlSettings.Descendants("ShowFilters").Descendants("ShowRatingFilter").Attributes("ShowRating").FirstOrDefault()?.Value,
                ShowNetwork = xmlSettings.Descendants("ShowFilters").Descendants("ShowNetworkFilter").Attributes("ShowNetwork").FirstOrDefault()?.Value,
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
    }
}
