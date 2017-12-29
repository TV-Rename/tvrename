// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Alphaleonis.Win32.Filesystem;
using NLog;

// Settings for TVRename.  All of this stuff is through Options->Preferences in the app.

namespace TVRename
{   
    public class TidySettings
    {
        public bool DeleteEmpty; // Delete empty folders after move
        public bool DeleteEmptyIsRecycle = true; // Recycle, rather than delete
        public bool EmptyIgnoreWords;
        public string EmptyIgnoreWordList = "sample";
        public bool EmptyIgnoreExtensions;
        public string EmptyIgnoreExtensionList = ".nzb;.nfo;.par2;.txt;.srt";
        public bool EmptyMaxSizeCheck = true;
        public int EmptyMaxSizeMb = 100;

        public string[] EmptyIgnoreExtensionsArray
        {
            get { return EmptyIgnoreExtensionList.Split(';'); }
        }
        public string[] EmptyIgnoreWordsArray
        {
            get { return EmptyIgnoreWordList.Split(';'); }
        }
    }

    public class Replacement
    {
        // used for invalid (and general) character (and string) replacements in filenames

        public bool CaseInsensitive;
        public string That;
        public string This;

        public Replacement(string a, string b, bool insens)
        {
            if (b == null)
                b = "";
            This = a;
            That = b;
            CaseInsensitive = insens;
        }
    }

    public class FilenameProcessorRe
    {
        // A regular expression to find the season and episode number in a filename

        public bool Enabled;
        public string Notes;
        public string Re;
        public bool UseFullPath;

        public FilenameProcessorRe(bool enabled, string re, bool useFullPath, string notes)
        {
            Enabled = enabled;
            Re = re;
            UseFullPath = useFullPath;
            Notes = notes;
        }
    }

    public class ShowStatusColoringTypeList : Dictionary<ShowStatusColoringType, Color>
    {
        public bool IsShowStatusDefined(string showStatus)
        {
            foreach (KeyValuePair<ShowStatusColoringType, Color> e in this)
            {
                if (!e.Key.IsMetaType && e.Key.IsShowLevel &&
                    e.Key.Status.Equals(showStatus, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public Color GetEntry(bool meta, bool showLevel, string status)
        {
            foreach (KeyValuePair<ShowStatusColoringType, Color> e in this)
            {
                if (e.Key.IsMetaType == meta && e.Key.IsShowLevel == showLevel &&
                    e.Key.Status.Equals(status, StringComparison.CurrentCultureIgnoreCase))
                {
                    return e.Value;
                }
            }
            return Color.Empty;
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
                    return string.Format("Show Seasons Status: {0}", StatusTextForDisplay);
                }
                if (!IsShowLevel && IsMetaType)
                {
                    return string.Format("Season Status: {0}", StatusTextForDisplay);
                }
                if (IsShowLevel && !IsMetaType)
                {
                    return string.Format("Show Status: {0}", StatusTextForDisplay);
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
                        case Season.SeasonStatus.Aired:
                            return "All aired";
                        case Season.SeasonStatus.NoEpisodes:
                            return "No Episodes";
                        case Season.SeasonStatus.NoneAired:
                            return "None aired";
                        case Season.SeasonStatus.PartiallyAired:
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

        private static volatile TVSettings _instance;
        private static readonly object SyncRoot = new Object();
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static TVSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new TVSettings();
                    }
                }

                return _instance;
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

        public enum WtwDoubleClickAction
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
            Quick
        }

        #endregion

        public enum KodiType
        {
            Eden,
            Frodo,
            Both
        }

        public List<String> MonitorFoldersNames = new List<String>();
        public List<String> IgnoreFoldersNames = new List<String>();
        public List<String> SearchFoldersNames = new List<String>();


        public bool AutoSelectShowInMyShows = true;
        public bool AutoCreateFolders;
        public bool BgDownload;
        public bool CheckuTorrent;
        public bool EpTbNs;
        public bool EpJpGs;
        public bool SeriesJpg;
        public bool ShrinkLargeMede8ErImages;
        public bool FanArtJpg;
        public bool Mede8ErXML;
        public bool ExportFoxml;
        public string ExportFoxmlTo = "";
        public bool ExportMissingCsv;
        public string ExportMissingCsvTo = "";
        public bool ExportMissingXML;
        public string ExportMissingXMLTo = "";
        public bool ExportShowsTxt;
        public string ExportShowsTxtTo = "";
        public int ExportRssMaxDays = 7;
        public int ExportRssMaxShows = 10;
        public int ExportRssDaysPast;
        public bool ExportRenamingXML;
        public string ExportRenamingXMLTo = "";
        public bool ExportWtwrss;
        public string ExportWtwrssTo = "";
        public bool ExportWtwxml;
        public string ExportWtwxmlTo = "";
        public List<FilenameProcessorRe> FnpRegexs = DefaultFnpList();
        public bool FolderJpg;
        public FolderJpgIsType FolderJpgIs = FolderJpgIsType.Poster;
        public ScanType MonitoredFoldersScanType = ScanType.Full;
        public KodiType SelectedKodiType = KodiType.Both;
        public bool ForceLowercaseFilenames;
        public bool IgnoreSamples = true;
        public bool KeepTogether = true;
        public bool LeadingZeroOnSeason;
        public bool LeaveOriginals;
        public bool LookForDateInFilename;
        public bool MissingCheck = true;
        public bool NfoShows;
        public bool NfoEpisodes;
        public bool KodiImages;
        public bool PyTivoMeta;
        public bool PyTivoMetaSubFolder;
        public CustomName NamingStyle = new CustomName();
        public bool NotificationAreaIcon;
        public bool OfflineMode;
        public string OtherExtensionsString = "";
        public ShowFilter Filter = new ShowFilter();

        public string[] OtherExtensionsArray
        {
            get { return OtherExtensionsString.Split(';'); }
        }

        public int ParallelDownloads = 4;
        public List<string> RssurLs = DefaultRssurlList();
        public bool RenameCheck = true;
        public bool RenameTxtToSub;
        public List<Replacement> Replacements = DefaultListRe();
        public string ResumeDatPath = "";
        public int SampleFileMaxSizeMb = 50; // sample file must be smaller than this to be ignored
        public bool SearchLocally = true;
        public bool SearchRss;
        public bool ShowEpisodePictures = true;
        public bool ShowInTaskbar = true;
        public string SpecialsFolderName = "Specials";
        public int StartupTab;
        public Searchers TheSearchers = new Searchers();

        public string[] VideoExtensionsArray
        {
            get { return VideoExtensionsString.Split(';'); }
        }
        public string VideoExtensionsString = "";
        public int WtwRecentDays = 7;
        public string UTorrentPath = "";
        public bool MonitorFolders;
        public bool RemoveDownloadDirectoriesFiles;
        public ShowStatusColoringTypeList ShowStatusColors = new ShowStatusColoringTypeList();
        public String SabHostPort = "";
        public String SabapiKey = "";
        public bool CheckSaBnzbd;
        public String PreferredLanguage = "en";
        public WtwDoubleClickAction WtwDoubleClick;

        public TidySettings Tidyup = new TidySettings();

        private TVSettings()
        {
            SetToDefaults();
        }

        public void Load(XmlReader reader)
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
                    BgDownload = reader.ReadElementContentAsBoolean();
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
                    ExportWtwrss = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportWTWRSSTo")
                    ExportWtwrssTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportWTWXML")
                    ExportWtwxml = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportWTWXMLTo")
                    ExportWtwxmlTo = reader.ReadElementContentAsString();
                else if (reader.Name == "WTWRecentDays")
                    WtwRecentDays = reader.ReadElementContentAsInt();
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
                    ExportRssMaxDays = reader.ReadElementContentAsInt();
                else if (reader.Name == "ExportRSSMaxShows")
                    ExportRssMaxShows = reader.ReadElementContentAsInt();
                else if (reader.Name == "ExportRSSDaysPast")
                    ExportRssDaysPast = reader.ReadElementContentAsInt();
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
                else if (reader.Name == "AutoCreateFolders")
                    AutoCreateFolders = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "AutoSelectShowInMyShows")
                    AutoSelectShowInMyShows = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SpecialsFolderName")
                    SpecialsFolderName = reader.ReadElementContentAsString();
                else if (reader.Name == "SABAPIKey")
                    SabapiKey = reader.ReadElementContentAsString();
                else if (reader.Name == "CheckSABnzbd")
                    CheckSaBnzbd = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SABHostPort")
                    SabHostPort = reader.ReadElementContentAsString();
                else if (reader.Name == "PreferredLanguage")
                    PreferredLanguage = reader.ReadElementContentAsString();
                else if (reader.Name == "WTWDoubleClick")
                    WtwDoubleClick = (WtwDoubleClickAction)reader.ReadElementContentAsInt();
                else if (reader.Name == "ExportMissingXML")
                    ExportMissingXML = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportMissingXMLTo")
                    ExportMissingXMLTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportMissingCSV")
                    ExportMissingCsv = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportMissingCSVTo")
                    ExportMissingCsvTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportRenamingXML")
                    ExportRenamingXML = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportRenamingXMLTo")
                    ExportRenamingXMLTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportFOXML")
                    ExportFoxml = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportFOXMLTo")
                    ExportFoxmlTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportShowsTXT")
                    ExportShowsTxt = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportShowsTXTTo")
                    ExportShowsTxtTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ForceLowercaseFilenames")
                    ForceLowercaseFilenames = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "IgnoreSamples")
                    IgnoreSamples = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SampleFileMaxSizeMB")
                    SampleFileMaxSizeMb = reader.ReadElementContentAsInt();
                else if (reader.Name == "ParallelDownloads")
                    ParallelDownloads = reader.ReadElementContentAsInt();
                else if (reader.Name == "uTorrentPath")
                    UTorrentPath = reader.ReadElementContentAsString();
                else if (reader.Name == "ResumeDatPath")
                    ResumeDatPath = reader.ReadElementContentAsString();
                else if (reader.Name == "SearchRSS")
                    SearchRss = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "EpImgs")
                    EpTbNs = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "NFOs") //support legacy tag
                {
                    NfoShows = reader.ReadElementContentAsBoolean();
                    NfoEpisodes = NfoShows;
                }
                else if (reader.Name == "NFOShows")
                    NfoShows = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "NFOEpisodes")
                    NfoEpisodes = reader.ReadElementContentAsBoolean();
                else if ((reader.Name == "XBMCImages") || (reader.Name == "KODIImages")) //Backward Compatibilty
                    KodiImages = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "pyTivoMeta")
                    PyTivoMeta = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "pyTivoMetaSubFolder")
                    PyTivoMetaSubFolder = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "FolderJpg")
                    FolderJpg = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "FolderJpgIs")
                    FolderJpgIs = (FolderJpgIsType)reader.ReadElementContentAsInt();
                else if (reader.Name == "MonitoredFoldersScanType")
                    MonitoredFoldersScanType = (ScanType)reader.ReadElementContentAsInt();
                else if ((reader.Name == "SelectedXBMCType") || (reader.Name == "SelectedKODIType"))
                    SelectedKodiType = (KodiType)reader.ReadElementContentAsInt();
                else if (reader.Name == "RenameCheck")
                    RenameCheck = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "CheckuTorrent")
                    CheckuTorrent = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "MissingCheck")
                    MissingCheck = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SearchLocally")
                    SearchLocally = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "LeaveOriginals")
                    LeaveOriginals = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "LookForDateInFilename")
                    LookForDateInFilename = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "MonitorFolders")
                    MonitorFolders = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "RemoveDownloadDirectoriesFiles")
                    RemoveDownloadDirectoriesFiles = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "EpJPGs")
                    EpJpGs = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SeriesJpg")
                    SeriesJpg = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "Mede8erXML")
                    Mede8ErXML = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ShrinkLargeMede8erImages")
                    ShrinkLargeMede8ErImages = reader.ReadElementContentAsBoolean();
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
                    Tidyup.EmptyMaxSizeMb = reader.ReadElementContentAsInt();

                else if (reader.Name == "FNPRegexs" && !reader.IsEmptyElement)
                {
                    FnpRegexs.Clear();
                    reader.Read();
                    while (!reader.EOF)
                    {
                        if ((reader.Name == "FNPRegexs") && (!reader.IsStartElement()))
                            break;
                        if (reader.Name == "Regex")
                        {
                            string s = reader.GetAttribute("Enabled");
                            bool en = s == null || bool.Parse(s);

                            FnpRegexs.Add(new FilenameProcessorRe(en, reader.GetAttribute("RE"),
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
                    RssurLs.Clear();
                    reader.Read();
                    while (!reader.EOF)
                    {
                        if ((reader.Name == "RSSURLs") && (!reader.IsStartElement()))
                            break;
                        if (reader.Name == "URL")
                            RssurLs.Add(reader.ReadElementContentAsString());
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
                            }

                            string color = reader.GetAttribute("Color");
                            if (type != null && !string.IsNullOrEmpty(color))
                            {
                                try
                                {
                                    Color c = ColorTranslator.FromHtml(color);
                                    ShowStatusColors.Add(type, c);
                                }
                                catch
                                {
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

            VideoExtensionsString = ".avi;.mpg;.mpeg;.mkv;.mp4;.wmv;.divx;.ogm;.qt;.rm";
            OtherExtensionsString = ".srt;.nfo;.txt;.tbn";

            // have a guess at utorrent's path
            string[] guesses = new string[3];
            guesses[0] = Application.StartupPath + "\\..\\uTorrent\\uTorrent.exe";
            guesses[1] = "c:\\Program Files\\uTorrent\\uTorrent.exe";
            guesses[2] = "c:\\Program Files (x86)\\uTorrent\\uTorrent.exe";

            UTorrentPath = "";
            foreach (string g in guesses)
            {
                FileInfo f = new FileInfo(g);
                if (f.Exists)
                {
                    UTorrentPath = f.FullName;
                    break;
                }
            }

            // ResumeDatPath
            FileInfo f2 =
                new FileInfo(Application.UserAppDataPath + "\\..\\..\\..\\uTorrent\\resume.dat");
            ResumeDatPath = f2.Exists ? f2.FullName : "";
        }


        public void WriteXML(XmlWriter writer)
        {
            writer.WriteStartElement("Settings");
            TheSearchers.WriteXML(writer);
            XMLHelper.WriteElementToXML(writer,"BGDownload",BgDownload);
            XMLHelper.WriteElementToXML(writer,"OfflineMode",OfflineMode);
            writer.WriteStartElement("Replacements");
            foreach (Replacement r in Replacements)
            {
                writer.WriteStartElement("Replace");
                XMLHelper.WriteAttributeToXML(writer,"This",r.This);
                XMLHelper.WriteAttributeToXML(writer, "That", r.That);
                XMLHelper.WriteAttributeToXML(writer, "CaseInsensitive", r.CaseInsensitive ? "Y" : "N");
                writer.WriteEndElement(); //Replace
            }
            writer.WriteEndElement(); //Replacements
            
            XMLHelper.WriteElementToXML(writer,"ExportWTWRSS",ExportWtwrss);
            XMLHelper.WriteElementToXML(writer,"ExportWTWRSSTo",ExportWtwrssTo);
            XMLHelper.WriteElementToXML(writer,"ExportWTWXML",ExportWtwxml);
            XMLHelper.WriteElementToXML(writer,"ExportWTWXMLTo",ExportWtwxmlTo);
            XMLHelper.WriteElementToXML(writer,"WTWRecentDays",WtwRecentDays);
            XMLHelper.WriteElementToXML(writer,"ExportMissingXML",ExportMissingXML);
            XMLHelper.WriteElementToXML(writer,"ExportMissingXMLTo",ExportMissingXMLTo);
            XMLHelper.WriteElementToXML(writer,"ExportMissingCSV",ExportMissingCsv);
            XMLHelper.WriteElementToXML(writer,"ExportMissingCSVTo",ExportMissingCsvTo);
            XMLHelper.WriteElementToXML(writer,"ExportRenamingXML",ExportRenamingXML);
            XMLHelper.WriteElementToXML(writer,"ExportRenamingXMLTo",ExportRenamingXMLTo);
            XMLHelper.WriteElementToXML(writer,"ExportShowsTXT", ExportShowsTxt);
            XMLHelper.WriteElementToXML(writer, "ExportShowsTXTTo", ExportShowsTxtTo);
            XMLHelper.WriteElementToXML(writer,"ExportFOXML",ExportFoxml);
            XMLHelper.WriteElementToXML(writer,"ExportFOXMLTo",ExportFoxmlTo);
            XMLHelper.WriteElementToXML(writer,"StartupTab2",TabNameForNumber(StartupTab));
            XMLHelper.WriteElementToXML(writer,"NamingStyle",NamingStyle.StyleString);
            XMLHelper.WriteElementToXML(writer,"NotificationAreaIcon",NotificationAreaIcon);
            XMLHelper.WriteElementToXML(writer,"VideoExtensions",VideoExtensionsString);
            XMLHelper.WriteElementToXML(writer,"OtherExtensions",OtherExtensionsString);
            XMLHelper.WriteElementToXML(writer,"ExportRSSMaxDays",ExportRssMaxDays);
            XMLHelper.WriteElementToXML(writer,"ExportRSSMaxShows",ExportRssMaxShows);
            XMLHelper.WriteElementToXML(writer,"ExportRSSDaysPast",ExportRssDaysPast);
            XMLHelper.WriteElementToXML(writer,"KeepTogether",KeepTogether);
            XMLHelper.WriteElementToXML(writer,"LeadingZeroOnSeason",LeadingZeroOnSeason);
            XMLHelper.WriteElementToXML(writer,"ShowInTaskbar",ShowInTaskbar);
            XMLHelper.WriteElementToXML(writer,"IgnoreSamples",IgnoreSamples);
            XMLHelper.WriteElementToXML(writer,"ForceLowercaseFilenames",ForceLowercaseFilenames);
            XMLHelper.WriteElementToXML(writer,"RenameTxtToSub",RenameTxtToSub);
            XMLHelper.WriteElementToXML(writer,"ParallelDownloads",ParallelDownloads);
            XMLHelper.WriteElementToXML(writer,"AutoSelectShowInMyShows",AutoSelectShowInMyShows);
            XMLHelper.WriteElementToXML(writer, "AutoCreateFolders", AutoCreateFolders );
            XMLHelper.WriteElementToXML(writer,"ShowEpisodePictures",ShowEpisodePictures);
            XMLHelper.WriteElementToXML(writer,"SpecialsFolderName",SpecialsFolderName);
            XMLHelper.WriteElementToXML(writer,"uTorrentPath",UTorrentPath);
            XMLHelper.WriteElementToXML(writer,"ResumeDatPath",ResumeDatPath);
            XMLHelper.WriteElementToXML(writer,"SearchRSS",SearchRss);
            XMLHelper.WriteElementToXML(writer,"EpImgs",EpTbNs);
            XMLHelper.WriteElementToXML(writer,"NFOShows",NfoShows);
            XMLHelper.WriteElementToXML(writer,"NFOEpisodes", NfoEpisodes);
            XMLHelper.WriteElementToXML(writer,"KODIImages",KodiImages);
            XMLHelper.WriteElementToXML(writer,"pyTivoMeta",PyTivoMeta);
            XMLHelper.WriteElementToXML(writer,"pyTivoMetaSubFolder",PyTivoMetaSubFolder);
            XMLHelper.WriteElementToXML(writer,"FolderJpg",FolderJpg);
            XMLHelper.WriteElementToXML(writer,"FolderJpgIs",(int) FolderJpgIs);
            XMLHelper.WriteElementToXML(writer,"MonitoredFoldersScanType",(int)MonitoredFoldersScanType);
            XMLHelper.WriteElementToXML(writer,"SelectedKODIType",(int)SelectedKodiType);
            XMLHelper.WriteElementToXML(writer,"CheckuTorrent",CheckuTorrent);
            XMLHelper.WriteElementToXML(writer,"RenameCheck",RenameCheck);
            XMLHelper.WriteElementToXML(writer,"MissingCheck",MissingCheck);
            XMLHelper.WriteElementToXML(writer,"SearchLocally",SearchLocally);
            XMLHelper.WriteElementToXML(writer,"LeaveOriginals",LeaveOriginals);
            XMLHelper.WriteElementToXML(writer,"LookForDateInFilename",LookForDateInFilename);
            XMLHelper.WriteElementToXML(writer,"MonitorFolders",MonitorFolders);
            XMLHelper.WriteElementToXML(writer, "RemoveDownloadDirectoriesFiles", RemoveDownloadDirectoriesFiles);
            XMLHelper.WriteElementToXML(writer,"SABAPIKey",SabapiKey);
            XMLHelper.WriteElementToXML(writer,"CheckSABnzbd",CheckSaBnzbd);
            XMLHelper.WriteElementToXML(writer,"SABHostPort",SabHostPort);
            XMLHelper.WriteElementToXML(writer,"PreferredLanguage",PreferredLanguage);
            XMLHelper.WriteElementToXML(writer,"WTWDoubleClick",(int) WtwDoubleClick);
            XMLHelper.WriteElementToXML(writer,"EpJPGs",EpJpGs);
            XMLHelper.WriteElementToXML(writer,"SeriesJpg",SeriesJpg);
            XMLHelper.WriteElementToXML(writer,"Mede8erXML",Mede8ErXML);
            XMLHelper.WriteElementToXML(writer,"ShrinkLargeMede8erImages",ShrinkLargeMede8ErImages);
            XMLHelper.WriteElementToXML(writer,"FanArtJpg",FanArtJpg);
            XMLHelper.WriteElementToXML(writer,"DeleteEmpty",Tidyup.DeleteEmpty);
            XMLHelper.WriteElementToXML(writer,"DeleteEmptyIsRecycle",Tidyup.DeleteEmptyIsRecycle);
            XMLHelper.WriteElementToXML(writer,"EmptyIgnoreWords",Tidyup.EmptyIgnoreWords);
            XMLHelper.WriteElementToXML(writer,"EmptyIgnoreWordList",Tidyup.EmptyIgnoreWordList);
            XMLHelper.WriteElementToXML(writer,"EmptyIgnoreExtensions",Tidyup.EmptyIgnoreExtensions);
            XMLHelper.WriteElementToXML(writer,"EmptyIgnoreExtensionList",Tidyup.EmptyIgnoreExtensionList);
            XMLHelper.WriteElementToXML(writer,"EmptyMaxSizeCheck",Tidyup.EmptyMaxSizeCheck);
            XMLHelper.WriteElementToXML(writer,"EmptyMaxSizeMB",Tidyup.EmptyMaxSizeMb);

            writer.WriteStartElement("FNPRegexs");
            foreach (FilenameProcessorRe re in FnpRegexs)
            {
                writer.WriteStartElement("Regex");
                XMLHelper.WriteAttributeToXML(writer,"Enabled",re.Enabled);
                XMLHelper.WriteAttributeToXML(writer,"RE",re.Re);
                XMLHelper.WriteAttributeToXML(writer,"UseFullPath",re.UseFullPath);
                XMLHelper.WriteAttributeToXML(writer,"Notes",re.Notes);
                writer.WriteEndElement(); // Regex
            }
            writer.WriteEndElement(); // FNPRegexs

            writer.WriteStartElement("RSSURLs");
            foreach (string s in RssurLs) XMLHelper.WriteElementToXML(writer,"URL",s);
            writer.WriteEndElement(); // RSSURLs

            if (ShowStatusColors != null)
            {
                writer.WriteStartElement("ShowStatusTVWColors");
                foreach (KeyValuePair<ShowStatusColoringType, Color> e in ShowStatusColors)
                {
                    writer.WriteStartElement("ShowStatusTVWColor");
                    // TODO ... Write Meta Flags
                    XMLHelper.WriteAttributeToXML(writer,"IsMeta",e.Key.IsMetaType);
                    XMLHelper.WriteAttributeToXML(writer,"IsShowLevel",e.Key.IsShowLevel);
                    XMLHelper.WriteAttributeToXML(writer,"ShowStatus",e.Key.Status);
                    XMLHelper.WriteAttributeToXML(writer,"Color",Helpers.TranslateColorToHtml(e.Value));
                    writer.WriteEndElement(); //ShowStatusTVWColor
                }
                writer.WriteEndElement(); // ShowStatusTVWColors
            }

            if (Filter != null)
            {
                writer.WriteStartElement("ShowFilters");

                XMLHelper.WriteInfo(writer, "NameFilter", "Name", Filter.ShowName);
                XMLHelper.WriteInfo(writer, "ShowStatusFilter", "ShowStatus", Filter.ShowStatus);
                XMLHelper.WriteInfo(writer, "ShowNetworkFilter", "ShowNetwork", Filter.ShowNetwork);
                XMLHelper.WriteInfo(writer, "ShowRatingFilter", "ShowRating", Filter.ShowRating);

                foreach (String genre in Filter.Genres) XMLHelper.WriteInfo(writer, "GenreFilter", "Genre", genre);
 
                writer.WriteEndElement(); //ShowFilters
            }

            writer.WriteEndElement(); // settings
        }

        public FolderJpgIsType ItemForFolderJpg() => FolderJpgIs;

        public string GetVideoExtensionsString() =>VideoExtensionsString;
        
        public string GetOtherExtensionsString() => OtherExtensionsString;
        
        public static bool OkExtensionsString(string s)
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

        public static List<FilenameProcessorRe> DefaultFnpList()
        {
            // Default list of filename processors

            List<FilenameProcessorRe> l = new List<FilenameProcessorRe>
                                              {
                                                  new FilenameProcessorRe(true,
                                                                          "(^|[^a-z])s?(?<s>[0-9]+)[ex](?<e>[0-9]{2,})(e[0-9]{2,})*[^a-z]",
                                                                          false, "3x23 s3x23 3e23 s3e23 s04e01e02e03"),
                                                  new FilenameProcessorRe(false,
                                                                          "(^|[^a-z])s?(?<s>[0-9]+)(?<e>[0-9]{2,})[^a-z]",
                                                                          false,
                                                                          "323 or s323 for season 3, episode 23. 2004 for season 20, episode 4."),
                                                  new FilenameProcessorRe(false,
                                                                          "(^|[^a-z])s(?<s>[0-9]+)--e(?<e>[0-9]{2,})[^a-z]",
                                                                          false, "S02--E03"),
                                                  new FilenameProcessorRe(false,
                                                                          "(^|[^a-z])s(?<s>[0-9]+) e(?<e>[0-9]{2,})[^a-z]",
                                                                          false, "'S02.E04' and 'S02 E04'"),
                                                  new FilenameProcessorRe(false, "^(?<s>[0-9]+) (?<e>[0-9]{2,})", false,
                                                                          "filenames starting with '1.23' for season 1, episode 23"),
                                                  new FilenameProcessorRe(true,
                                                                          "(^|[^a-z])(?<s>[0-9])(?<e>[0-9]{2,})[^a-z]",
                                                                          false, "Show - 323 - Foo"),
                                                  new FilenameProcessorRe(true,
                                                                          "(^|[^a-z])se(?<s>[0-9]+)([ex]|ep|xep)?(?<e>[0-9]+)[^a-z]",
                                                                          false, "se3e23 se323 se1ep1 se01xep01..."),
                                                  new FilenameProcessorRe(true,
                                                                          "(^|[^a-z])(?<s>[0-9]+)-(?<e>[0-9]{2,})[^a-z]",
                                                                          false, "3-23 EpName"),
                                                  new FilenameProcessorRe(true,
                                                                          "(^|[^a-z])s(?<s>[0-9]+) +- +e(?<e>[0-9]{2,})[^a-z]",
                                                                          false, "ShowName - S01 - E01"),
                                                  new FilenameProcessorRe(true,
                                                                          "\\b(?<e>[0-9]{2,}) ?- ?.* ?- ?(?<s>[0-9]+)",
                                                                          false,
                                                                          "like '13 - Showname - 2 - Episode Title.avi'"),
                                                  new FilenameProcessorRe(true,
                                                                          "\\b(episode|ep|e) ?(?<e>[0-9]{2,}) ?- ?(series|season) ?(?<s>[0-9]+)",
                                                                          false, "episode 3 - season 4"),
                                                  new FilenameProcessorRe(true,
                                                                          "season (?<s>[0-9]+)\\\\e?(?<e>[0-9]{1,3}) ?-",
                                                                          true, "Show Season 3\\E23 - Epname"),
                                                  new FilenameProcessorRe(false,
                                                                          "season (?<s>[0-9]+)\\\\episode (?<e>[0-9]{1,3})",
                                                                          true, "Season 3\\Episode 23")
                                              };

            return l;
        }

        private static List<Replacement> DefaultListRe()
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

        private static List<string> DefaultRssurlList()
        {
            List<string> sl = new List<String>
                                  {
                                      "http://tvrss.net/feed/eztv"
                                  };
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

        public string BtSearchUrl(ProcessedEpisode epi)
        {
            if (epi == null)
                return "";

            SeriesInfo s = epi.TheSeries;
            if (s == null)
                return "";

            String url = String.IsNullOrEmpty(epi.Si.CustomSearchUrl)
                             ? TheSearchers.CurrentSearchUrl()
                             : epi.Si.CustomSearchUrl;
            return CustomName.NameForNoExt(epi, url, true);
        }

        public string FilenameFriendly(string fn)
        {
            foreach (Replacement r in Replacements)
            {
                if (r.CaseInsensitive)
                    fn = Regex.Replace(fn, Regex.Escape(r.This), Regex.Escape(r.That), RegexOptions.IgnoreCase);
                else
                    fn = fn.Replace(r.This, r.That);
            }
            if (ForceLowercaseFilenames)
                fn = fn.ToLower();
            return fn;
        }

        public bool NeedToDownloadBannerFile(){
            // Return true iff we need to download season specific images
            // There are 4 possible reasons
            return (SeasonSpecificFolderJPG() || KodiImages || SeriesJpg ||FanArtJpg);
        }

        public bool SeasonSpecificFolderJPG() {
            return (FolderJpgIsType.SeasonPoster == FolderJpgIs);
        }

        public bool DownloadFrodoImages()
        {
            return (KodiImages && (SelectedKodiType == KodiType.Both || SelectedKodiType == KodiType.Frodo));
        }

        public bool DownloadEdenImages()
        {
            return (KodiImages && (SelectedKodiType == KodiType.Both || SelectedKodiType == KodiType.Eden)); 
        }
    }
}
