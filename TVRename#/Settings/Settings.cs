// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

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
        public int EmptyMaxSizeMB = 100;

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
            this.This = a;
            this.That = b;
            this.CaseInsensitive = insens;
        }
    }

    public class FilenameProcessorRE
    {
        // A regular expression to find the season and episode number in a filename

        public bool Enabled;
        public string Notes;
        public string RE;
        public bool UseFullPath;

        public FilenameProcessorRE(bool enabled, string re, bool useFullPath, string notes)
        {
            this.Enabled = enabled;
            this.RE = re;
            this.UseFullPath = useFullPath;
            this.Notes = notes;
        }
    }

    public class ShowStatusColoringTypeList : Dictionary<ShowStatusColoringType, System.Drawing.Color>
    {
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
            this.IsMetaType = isMetaType;
            this.IsShowLevel = isShowLevel;
            this.Status = status;
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

        private static volatile TVSettings instance;
        private static object syncRoot = new Object();

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
            Quick
        }

        #endregion

        public enum XBMCType
        {
            Eden,
            Frodo,
            Both
        }

        public bool AutoSelectShowInMyShows = true;
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
        public XBMCType SelectedXBMCType = XBMCType.Both;
        public bool ForceLowercaseFilenames = false;
        public bool IgnoreSamples = true;
        public bool KeepTogether = true;
        public bool LeadingZeroOnSeason = false;
        public bool LeaveOriginals = false;
        public bool LookForDateInFilename = false;
        public bool MissingCheck = true;
        public bool NFOs = false;
        public bool XBMCImages = false;
        public bool pyTivoMeta = false;
        public bool pyTivoMetaSubFolder = false;
        public CustomName NamingStyle = new CustomName();
        public bool NotificationAreaIcon = false;
        public bool OfflineMode = false;
        public string OtherExtensionsString = "";

        public string[] OtherExtensionsArray
        {
            get { return OtherExtensionsString.Split(';'); }
        }

        public int ParallelDownloads = 4;
        public List<string> RSSURLs = DefaultRSSURLList();
        public bool RenameCheck = true;
        public bool RenameTxtToSub = false;
        public List<Replacement> Replacements = DefaultListRE();
        public string ResumeDatPath = "";
        public int SampleFileMaxSizeMB = 50; // sample file must be smaller than this to be ignored
        public bool SearchLocally = true;
        public bool SearchRSS = false;
        public bool ShowEpisodePictures = true;
        public bool ShowInTaskbar = true;
        public string SpecialsFolderName = "Specials";
        public int StartupTab = 0;
        public Searchers TheSearchers = new Searchers();

        public string[] VideoExtensionsArray
        {
            get { return VideoExtensionsString.Split(';'); }
        }

        public string VideoExtensionsString = "";
        public int WTWRecentDays = 7;
        public string uTorrentPath = "";
        public bool MonitorFolders = false;
        public ShowStatusColoringTypeList ShowStatusColors = new ShowStatusColoringTypeList();
        public String SABHostPort = "";
        public String SABAPIKey = "";
        public bool CheckSABnzbd = false;
        public String PreferredLanguage = "en";
        public WTWDoubleClickAction WTWDoubleClick;

        public TidySettings Tidyup = new TidySettings();

        private TVSettings()
        {
            this.SetToDefaults();
        }

        public void load(XmlReader reader)
        {
            this.SetToDefaults();

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
                    this.TheSearchers.CurrentSearch = srch;
                }
                else if (reader.Name == "TheSearchers")
                {
                    this.TheSearchers = new Searchers(reader.ReadSubtree());
                    reader.Read();
                }
                else if (reader.Name == "BGDownload")
                    this.BGDownload = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "OfflineMode")
                    this.OfflineMode = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "Replacements")
                {
                    this.Replacements.Clear();
                    reader.Read();
                    while (!reader.EOF)
                    {
                        if ((reader.Name == "Replacements") && (!reader.IsStartElement()))
                            break;
                        if (reader.Name == "Replace")
                        {
                            this.Replacements.Add(new Replacement(reader.GetAttribute("This"),
                                                                  reader.GetAttribute("That"),
                                                                  reader.GetAttribute("CaseInsensitive") == "Y"));
                            reader.Read();
                        }
                        else
                            reader.ReadOuterXml();
                    }
                    reader.Read();
                }
                else if (reader.Name == "ExportWTWRSS")
                    this.ExportWTWRSS = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportWTWRSSTo")
                    this.ExportWTWRSSTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportWTWXML")
                    this.ExportWTWXML = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportWTWXMLTo")
                    this.ExportWTWXMLTo = reader.ReadElementContentAsString();
                else if (reader.Name == "WTWRecentDays")
                    this.WTWRecentDays = reader.ReadElementContentAsInt();
                else if (reader.Name == "StartupTab")
                {
                    int n = reader.ReadElementContentAsInt();
                    if (n == 6)
                        this.StartupTab = 2; // WTW is moved
                    else if ((n >= 1) && (n <= 3)) // any of the three scans
                        this.StartupTab = 1;
                    else
                        this.StartupTab = 0; // otherwise, My Shows
                }
                else if (reader.Name == "StartupTab2")
                    this.StartupTab = TabNumberFromName(reader.ReadElementContentAsString());
                else if (reader.Name == "DefaultNamingStyle") // old naming style
                    this.NamingStyle.StyleString = CustomName.OldNStyle(reader.ReadElementContentAsInt());
                else if (reader.Name == "NamingStyle")
                    this.NamingStyle.StyleString = reader.ReadElementContentAsString();
                else if (reader.Name == "NotificationAreaIcon")
                    this.NotificationAreaIcon = reader.ReadElementContentAsBoolean();
                else if ((reader.Name == "GoodExtensions") || (reader.Name == "VideoExtensions"))
                    this.VideoExtensionsString = reader.ReadElementContentAsString();
                else if (reader.Name == "OtherExtensions")
                    this.OtherExtensionsString = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportRSSMaxDays")
                    this.ExportRSSMaxDays = reader.ReadElementContentAsInt();
                else if (reader.Name == "ExportRSSMaxShows")
                    this.ExportRSSMaxShows = reader.ReadElementContentAsInt();
                else if (reader.Name == "ExportRSSDaysPast")
                    this.ExportRSSDaysPast = reader.ReadElementContentAsInt();
                else if (reader.Name == "KeepTogether")
                    this.KeepTogether = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "LeadingZeroOnSeason")
                    this.LeadingZeroOnSeason = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ShowInTaskbar")
                    this.ShowInTaskbar = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "RenameTxtToSub")
                    this.RenameTxtToSub = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ShowEpisodePictures")
                    this.ShowEpisodePictures = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "AutoSelectShowInMyShows")
                    this.AutoSelectShowInMyShows = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SpecialsFolderName")
                    this.SpecialsFolderName = reader.ReadElementContentAsString();
                else if (reader.Name == "SABAPIKey")
                    this.SABAPIKey = reader.ReadElementContentAsString();
                else if (reader.Name == "CheckSABnzbd")
                    this.CheckSABnzbd = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SABHostPort")
                    this.SABHostPort = reader.ReadElementContentAsString();
                else if (reader.Name == "PreferredLanguage")
                    this.PreferredLanguage = reader.ReadElementContentAsString();
                else if (reader.Name == "WTWDoubleClick")
                    this.WTWDoubleClick = (WTWDoubleClickAction) reader.ReadElementContentAsInt();
                else if (reader.Name == "ExportMissingXML")
                    this.ExportMissingXML = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportMissingXMLTo")
                    this.ExportMissingXMLTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportMissingCSV")
                    this.ExportMissingCSV = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportMissingCSVTo")
                    this.ExportMissingCSVTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportRenamingXML")
                    this.ExportRenamingXML = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportRenamingXMLTo")
                    this.ExportRenamingXMLTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportFOXML")
                    this.ExportFOXML = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportFOXMLTo")
                    this.ExportFOXMLTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ForceLowercaseFilenames")
                    this.ForceLowercaseFilenames = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "IgnoreSamples")
                    this.IgnoreSamples = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SampleFileMaxSizeMB")
                    this.SampleFileMaxSizeMB = reader.ReadElementContentAsInt();
                else if (reader.Name == "ParallelDownloads")
                    this.ParallelDownloads = reader.ReadElementContentAsInt();
                else if (reader.Name == "uTorrentPath")
                    this.uTorrentPath = reader.ReadElementContentAsString();
                else if (reader.Name == "ResumeDatPath")
                    this.ResumeDatPath = reader.ReadElementContentAsString();
                else if (reader.Name == "SearchRSS")
                    this.SearchRSS = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "EpImgs")
                    this.EpTBNs = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "NFOs")
                    this.NFOs = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "XBMCImages")
                    this.XBMCImages = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "pyTivoMeta")
                    this.pyTivoMeta = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "pyTivoMetaSubFolder")
                    this.pyTivoMetaSubFolder = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "FolderJpg")
                    this.FolderJpg = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "FolderJpgIs")
                    this.FolderJpgIs = (FolderJpgIsType)reader.ReadElementContentAsInt();
                else if (reader.Name == "MonitoredFoldersScanType")
                    this.MonitoredFoldersScanType = (ScanType)reader.ReadElementContentAsInt();
                else if (reader.Name == "SelectedXBMCType")
                    this.SelectedXBMCType = (XBMCType)reader.ReadElementContentAsInt();
                else if (reader.Name == "RenameCheck")
                    this.RenameCheck = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "CheckuTorrent")
                    this.CheckuTorrent = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "MissingCheck")
                    this.MissingCheck = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SearchLocally")
                    this.SearchLocally = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "LeaveOriginals")
                    this.LeaveOriginals = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "LookForDateInFilename")
                    LookForDateInFilename = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "MonitorFolders")
                    this.MonitorFolders = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "EpJPGs")
                    this.EpJPGs = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SeriesJpg")
                    this.SeriesJpg = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "Mede8erXML")
                    this.Mede8erXML = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ShrinkLargeMede8erImages")
                    this.ShrinkLargeMede8erImages = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "FanArtJpg")
                    this.FanArtJpg = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "DeleteEmpty")
                    this.Tidyup.DeleteEmpty = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "DeleteEmptyIsRecycle")
                    this.Tidyup.DeleteEmptyIsRecycle = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "EmptyIgnoreWords")
                    this.Tidyup.EmptyIgnoreWords = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "EmptyIgnoreWordList")
                    this.Tidyup.EmptyIgnoreWordList = reader.ReadElementContentAsString();
                else if (reader.Name == "EmptyIgnoreExtensions")
                    this.Tidyup.EmptyIgnoreExtensions = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "EmptyIgnoreExtensionList")
                    this.Tidyup.EmptyIgnoreExtensionList = reader.ReadElementContentAsString();
                else if (reader.Name == "EmptyMaxSizeCheck")
                    this.Tidyup.EmptyMaxSizeCheck = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "EmptyMaxSizeMB")
                    this.Tidyup.EmptyMaxSizeMB = reader.ReadElementContentAsInt();

                else if (reader.Name == "FNPRegexs")
                {
                    this.FNPRegexs.Clear();
                    reader.Read();
                    while (!reader.EOF)
                    {
                        if ((reader.Name == "FNPRegexs") && (!reader.IsStartElement()))
                            break;
                        if (reader.Name == "Regex")
                        {
                            string s = reader.GetAttribute("Enabled");
                            bool en = s == null || bool.Parse(s);

                            this.FNPRegexs.Add(new FilenameProcessorRE(en, reader.GetAttribute("RE"),
                                                                       bool.Parse(reader.GetAttribute("UseFullPath")),
                                                                       reader.GetAttribute("Notes")));
                            reader.Read();
                        }
                        else
                            reader.ReadOuterXml();
                    }
                    reader.Read();
                }
                else if (reader.Name == "RSSURLs")
                {
                    this.RSSURLs.Clear();
                    reader.Read();
                    while (!reader.EOF)
                    {
                        if ((reader.Name == "RSSURLs") && (!reader.IsStartElement()))
                            break;
                        if (reader.Name == "URL")
                            this.RSSURLs.Add(reader.ReadElementContentAsString());
                        else
                            reader.ReadOuterXml();
                    }
                    reader.Read();
                }
                else if (reader.Name == "ShowStatusTVWColors")
                {
                    this.ShowStatusColors = new ShowStatusColoringTypeList();
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
                                    System.Drawing.Color c = System.Drawing.ColorTranslator.FromHtml(color);
                                    this.ShowStatusColors.Add(type, c);
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
                else
                    reader.ReadOuterXml();
            }
        }

        public void SetToDefaults()
        {
            // defaults that aren't handled with default initialisers

            this.VideoExtensionsString = ".avi;.mpg;.mpeg;.mkv;.mp4;.wmv;.divx;.ogm;.qt;.rm";
            this.OtherExtensionsString = ".srt;.nfo;.txt;.tbn";

            // have a guess at utorrent's path
            string[] guesses = new string[3];
            guesses[0] = System.Windows.Forms.Application.StartupPath + "\\..\\uTorrent\\uTorrent.exe";
            guesses[1] = "c:\\Program Files\\uTorrent\\uTorrent.exe";
            guesses[2] = "c:\\Program Files (x86)\\uTorrent\\uTorrent.exe";

            this.uTorrentPath = "";
            foreach (string g in guesses)
            {
                FileInfo f = new FileInfo(g);
                if (f.Exists)
                {
                    this.uTorrentPath = f.FullName;
                    break;
                }
            }

            // ResumeDatPath
            FileInfo f2 =
                new FileInfo(System.Windows.Forms.Application.UserAppDataPath + "\\..\\..\\..\\uTorrent\\resume.dat");
            this.ResumeDatPath = f2.Exists ? f2.FullName : "";
        }


        public void WriteXML(XmlWriter writer)
        {
            writer.WriteStartElement("Settings");
            this.TheSearchers.WriteXML(writer);
            XMLHelper.WriteElementToXML(writer,"BGDownload",this.BGDownload);
            XMLHelper.WriteElementToXML(writer,"OfflineMode",this.OfflineMode);
            writer.WriteStartElement("Replacements");
            foreach (Replacement R in this.Replacements)
            {
                writer.WriteStartElement("Replace");
                XMLHelper.WriteAttributeToXML(writer,"This",R.This);
                XMLHelper.WriteAttributeToXML(writer, "That", R.That);
                XMLHelper.WriteAttributeToXML(writer, "CaseInsensitive", R.CaseInsensitive ? "Y" : "N");
                writer.WriteEndElement(); //Replace
            }
            writer.WriteEndElement(); //Replacements
            
            XMLHelper.WriteElementToXML(writer,"ExportWTWRSS",this.ExportWTWRSS);
            XMLHelper.WriteElementToXML(writer,"ExportWTWRSSTo",this.ExportWTWRSSTo);
            XMLHelper.WriteElementToXML(writer,"ExportWTWXML",this.ExportWTWXML);
            XMLHelper.WriteElementToXML(writer,"ExportWTWXMLTo",this.ExportWTWXMLTo);
            XMLHelper.WriteElementToXML(writer,"WTWRecentDays",this.WTWRecentDays);
            XMLHelper.WriteElementToXML(writer,"ExportMissingXML",this.ExportMissingXML);
            XMLHelper.WriteElementToXML(writer,"ExportMissingXMLTo",this.ExportMissingXMLTo);
            XMLHelper.WriteElementToXML(writer,"ExportMissingCSV",this.ExportMissingCSV);
            XMLHelper.WriteElementToXML(writer,"ExportMissingCSVTo",this.ExportMissingCSVTo);
            XMLHelper.WriteElementToXML(writer,"ExportRenamingXML",this.ExportRenamingXML);
            XMLHelper.WriteElementToXML(writer,"ExportRenamingXMLTo",this.ExportRenamingXMLTo);
            XMLHelper.WriteElementToXML(writer,"ExportFOXML",this.ExportFOXML);
            XMLHelper.WriteElementToXML(writer,"ExportFOXMLTo",this.ExportFOXMLTo);
            XMLHelper.WriteElementToXML(writer,"StartupTab2",TabNameForNumber(this.StartupTab));
            XMLHelper.WriteElementToXML(writer,"NamingStyle",this.NamingStyle.StyleString);
            XMLHelper.WriteElementToXML(writer,"NotificationAreaIcon",this.NotificationAreaIcon);
            XMLHelper.WriteElementToXML(writer,"VideoExtensions",this.VideoExtensionsString);
            XMLHelper.WriteElementToXML(writer,"OtherExtensions",this.OtherExtensionsString);
            XMLHelper.WriteElementToXML(writer,"ExportRSSMaxDays",this.ExportRSSMaxDays);
            XMLHelper.WriteElementToXML(writer,"ExportRSSMaxShows",this.ExportRSSMaxShows);
            XMLHelper.WriteElementToXML(writer,"ExportRSSDaysPast",this.ExportRSSDaysPast);
            XMLHelper.WriteElementToXML(writer,"KeepTogether",this.KeepTogether);
            XMLHelper.WriteElementToXML(writer,"LeadingZeroOnSeason",this.LeadingZeroOnSeason);
            XMLHelper.WriteElementToXML(writer,"ShowInTaskbar",this.ShowInTaskbar);
            XMLHelper.WriteElementToXML(writer,"IgnoreSamples",this.IgnoreSamples);
            XMLHelper.WriteElementToXML(writer,"ForceLowercaseFilenames",this.ForceLowercaseFilenames);
            XMLHelper.WriteElementToXML(writer,"RenameTxtToSub",this.RenameTxtToSub);
            XMLHelper.WriteElementToXML(writer,"ParallelDownloads",this.ParallelDownloads);
            XMLHelper.WriteElementToXML(writer,"AutoSelectShowInMyShows",this.AutoSelectShowInMyShows);
            XMLHelper.WriteElementToXML(writer,"ShowEpisodePictures",this.ShowEpisodePictures);
            XMLHelper.WriteElementToXML(writer,"SpecialsFolderName",this.SpecialsFolderName);
            XMLHelper.WriteElementToXML(writer,"uTorrentPath",this.uTorrentPath);
            XMLHelper.WriteElementToXML(writer,"ResumeDatPath",this.ResumeDatPath);
            XMLHelper.WriteElementToXML(writer,"SearchRSS",this.SearchRSS);
            XMLHelper.WriteElementToXML(writer,"EpImgs",this.EpTBNs);
            XMLHelper.WriteElementToXML(writer,"NFOs",this.NFOs);
            XMLHelper.WriteElementToXML(writer,"XBMCImages",this.XBMCImages);
            XMLHelper.WriteElementToXML(writer,"pyTivoMeta",this.pyTivoMeta);
            XMLHelper.WriteElementToXML(writer,"pyTivoMetaSubFolder",this.pyTivoMetaSubFolder);
            XMLHelper.WriteElementToXML(writer,"FolderJpg",this.FolderJpg);
            XMLHelper.WriteElementToXML(writer,"FolderJpgIs",(int) this.FolderJpgIs);
            XMLHelper.WriteElementToXML(writer,"MonitoredFoldersScanType",(int)this.MonitoredFoldersScanType);
            XMLHelper.WriteElementToXML(writer,"SelectedXBMCType",(int)this.SelectedXBMCType);
            XMLHelper.WriteElementToXML(writer,"CheckuTorrent",this.CheckuTorrent);
            XMLHelper.WriteElementToXML(writer,"RenameCheck",this.RenameCheck);
            XMLHelper.WriteElementToXML(writer,"MissingCheck",this.MissingCheck);
            XMLHelper.WriteElementToXML(writer,"SearchLocally",this.SearchLocally);
            XMLHelper.WriteElementToXML(writer,"LeaveOriginals",this.LeaveOriginals);
            XMLHelper.WriteElementToXML(writer,"LookForDateInFilename",this.LookForDateInFilename);
            XMLHelper.WriteElementToXML(writer,"MonitorFolders",this.MonitorFolders);
            XMLHelper.WriteElementToXML(writer,"SABAPIKey",this.SABAPIKey);
            XMLHelper.WriteElementToXML(writer,"CheckSABnzbd",this.CheckSABnzbd);
            XMLHelper.WriteElementToXML(writer,"SABHostPort",this.SABHostPort);
            XMLHelper.WriteElementToXML(writer,"PreferredLanguage",this.PreferredLanguage);
            XMLHelper.WriteElementToXML(writer,"WTWDoubleClick",(int) this.WTWDoubleClick);
            XMLHelper.WriteElementToXML(writer,"EpJPGs",this.EpJPGs);
            XMLHelper.WriteElementToXML(writer,"SeriesJpg",this.SeriesJpg);
            XMLHelper.WriteElementToXML(writer,"Mede8erXML",this.Mede8erXML);
            XMLHelper.WriteElementToXML(writer,"ShrinkLargeMede8erImages",this.ShrinkLargeMede8erImages);
            XMLHelper.WriteElementToXML(writer,"FanArtJpg",this.FanArtJpg);
            XMLHelper.WriteElementToXML(writer,"DeleteEmpty",this.Tidyup.DeleteEmpty);
            XMLHelper.WriteElementToXML(writer,"DeleteEmptyIsRecycle",this.Tidyup.DeleteEmptyIsRecycle);
            XMLHelper.WriteElementToXML(writer,"EmptyIgnoreWords",this.Tidyup.EmptyIgnoreWords);
            XMLHelper.WriteElementToXML(writer,"EmptyIgnoreWordList",this.Tidyup.EmptyIgnoreWordList);
            XMLHelper.WriteElementToXML(writer,"EmptyIgnoreExtensions",this.Tidyup.EmptyIgnoreExtensions);
            XMLHelper.WriteElementToXML(writer,"EmptyIgnoreExtensionList",this.Tidyup.EmptyIgnoreExtensionList);
            XMLHelper.WriteElementToXML(writer,"EmptyMaxSizeCheck",this.Tidyup.EmptyMaxSizeCheck);
            XMLHelper.WriteElementToXML(writer,"EmptyMaxSizeMB",this.Tidyup.EmptyMaxSizeMB);

            writer.WriteStartElement("FNPRegexs");
            foreach (FilenameProcessorRE re in this.FNPRegexs)
            {
                writer.WriteStartElement("Regex");
                XMLHelper.WriteAttributeToXML(writer,"Enabled",re.Enabled);
                XMLHelper.WriteAttributeToXML(writer,"RE",re.RE);
                XMLHelper.WriteAttributeToXML(writer,"UseFullPath",re.UseFullPath);
                XMLHelper.WriteAttributeToXML(writer,"Notes",re.Notes);
                writer.WriteEndElement(); // Regex
            }
            writer.WriteEndElement(); // FNPRegexs

            writer.WriteStartElement("RSSURLs");
            foreach (string s in this.RSSURLs)
            {
                XMLHelper.WriteElementToXML(writer,"URL",s);
            }
            writer.WriteEndElement(); // RSSURLs

            if (ShowStatusColors != null)
            {
                writer.WriteStartElement("ShowStatusTVWColors");
                foreach (KeyValuePair<ShowStatusColoringType, System.Drawing.Color> e in this.ShowStatusColors)
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

            writer.WriteEndElement(); // settings
        }

        public string ItemForFolderJpg()
        {
            switch (this.FolderJpgIs)
            {
                case FolderJpgIsType.Banner:
                    return "banner";
                case FolderJpgIsType.FanArt:
                    return "fanart";
                case FolderJpgIsType.SeasonPoster:
                    return "seasonPoster";
                default:
                    return "poster";
            }
        }

        public string GetVideoExtensionsString()
        {
            return this.VideoExtensionsString;
        }

        public string GetOtherExtensionsString()
        {
            return this.OtherExtensionsString;
        }

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
                                                  new FilenameProcessorRE(true,
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
            foreach (string s in this.VideoExtensionsArray)
            {
                if (sn.ToLower() == s)
                    return true;
            }
            if (otherExtensionsToo)
            {
                foreach (string s in this.OtherExtensionsArray)
                {
                    if (sn.ToLower() == s)
                        return true;
                }
            }

            return false;
        }

        public string BTSearchURL(ProcessedEpisode epi)
        {
            if (epi == null)
                return "";

            SeriesInfo s = epi.TheSeries;
            if (s == null)
                return "";

            String url = String.IsNullOrEmpty(epi.SI.CustomSearchURL)
                             ? this.TheSearchers.CurrentSearchURL()
                             : epi.SI.CustomSearchURL;
            return CustomName.NameForNoExt(epi, url, true);
        }

        public string FilenameFriendly(string fn)
        {
            foreach (Replacement R in this.Replacements)
            {
                if (R.CaseInsensitive)
                    fn = Regex.Replace(fn, Regex.Escape(R.This), Regex.Escape(R.That), RegexOptions.IgnoreCase);
                else
                    fn = fn.Replace(R.This, R.That);
            }
            if (this.ForceLowercaseFilenames)
                fn = fn.ToLower();
            return fn;
        }

        public bool NeedToDownloadBannerFile(){
            // Return true iff we need to download season specific images
            // There are 3 possible reasons
            return (SeasonSpecificFolderJPG() || XBMCImages || SeriesJpg);
        }

        public bool SeasonSpecificFolderJPG() {
            return (FolderJpgIsType.SeasonPoster == FolderJpgIs);
        }

        public bool DownloadFrodoImages()
        {
            return (XBMCImages && (SelectedXBMCType == XBMCType.Both || SelectedXBMCType == XBMCType.Frodo));
        }

        public bool DownloadEdenImages()
        {
            return (XBMCImages && (SelectedXBMCType == XBMCType.Both || SelectedXBMCType == XBMCType.Eden)); 
        }
    }
}
