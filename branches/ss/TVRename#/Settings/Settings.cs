// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

// Settings for TVRename.  All of this stuff is through Options->Preferences in the app.

namespace TVRename
{
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

    public class ReplacementList : System.Collections.Generic.List<Replacement>
    {
    }

    public class FNPRegexList : System.Collections.Generic.List<FilenameProcessorRE>
    {
    }

    public class ShowStatusColoringTypeList : System.Collections.Generic.Dictionary<ShowStatusColoringType, System.Drawing.Color>
    {
        public bool IsShowStatusDefined(string showStatus)
        {
            foreach (System.Collections.Generic.KeyValuePair<ShowStatusColoringType, System.Drawing.Color> e in this)
            {
                if (!e.Key.IsMetaType && e.Key.IsShowLevel && e.Key.Status.Equals(showStatus, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public System.Drawing.Color GetEntry(bool meta, bool showLevel, string status)
        {
            foreach (System.Collections.Generic.KeyValuePair<ShowStatusColoringType, System.Drawing.Color> e in this)
            {
                if (e.Key.IsMetaType == meta && e.Key.IsShowLevel == showLevel && e.Key.Status.Equals(status, StringComparison.CurrentCultureIgnoreCase))
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
                else if (!IsShowLevel && IsMetaType)
                {
                    return string.Format("Season Status: {0}", StatusTextForDisplay);
                }
                else if (IsShowLevel && !IsMetaType)
                {
                    return string.Format("Show Status: {0}", StatusTextForDisplay);
                }
                else
                {
                    return "";
                }
            }
        }

        string StatusTextForDisplay
        {
            get
            {
                if (IsMetaType)
                {
                    if (IsShowLevel)
                    {
                        ShowItem.ShowAirStatus status = (ShowItem.ShowAirStatus)Enum.Parse(typeof(ShowItem.ShowAirStatus), Status);
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
                        Season.SeasonStatus status = (Season.SeasonStatus)Enum.Parse(typeof(Season.SeasonStatus), Status);
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
                else
                {
                    return Status;
                }
            }
        }
    }

    public class TVSettings
    {
        #region FolderJpgIsType enum

        public enum FolderJpgIsType
        {
            Banner,
            Poster,
            FanArt
        }

        #endregion

        public bool AutoSelectShowInMyShows = true;
        public bool BGDownload = false;
        public bool CheckuTorrent = false;
        public bool EpImgs = false;
        public bool ExportFOXML = false;
        public string ExportFOXMLTo = "";
        public bool ExportMissingCSV = false;
        public string ExportMissingCSVTo = "";
        public bool ExportMissingXML = false;
        public string ExportMissingXMLTo = "";
        public int ExportRSSMaxDays = 7;
        public int ExportRSSMaxShows = 10;
        public bool ExportRenamingXML = false;
        public string ExportRenamingXMLTo = "";
        public bool ExportWTWRSS = false;
        public string ExportWTWRSSTo = "";
        public FNPRegexList FNPRegexs = DefaultFNPList();
        public bool FolderJpg = false;
        public FolderJpgIsType FolderJpgIs = FolderJpgIsType.Poster;
        public bool ForceLowercaseFilenames = false;
        public bool IgnoreSamples = true;
        public bool KeepTogether = true;
        public bool LeadingZeroOnSeason = false;
        public bool LeaveOriginals = false;
        public bool LookForDateInFilename = false;
        public bool MissingCheck = true;
        public bool NFOs = false;
        public bool pyTivoMeta = false;
        public bool pyTivoMetaSubFolder = false;
        public CustomName NamingStyle = new CustomName();
        public bool NotificationAreaIcon = false;
        public bool OfflineMode = false; // TODO: Make property of thetvdb?
        private string[] OtherExtensionsArray = new string[0];
        private string OtherExtensionsString = "";
        public int ParallelDownloads = 4;
        public StringList RSSURLs = DefaultRSSURLList();
        public bool RenameCheck = true;
        public bool RenameTxtToSub = false;
        public ReplacementList Replacements = DefaultReplacementList();
        public string ResumeDatPath = "";
        public int SampleFileMaxSizeMB = 50; // sample file must be smaller than this to be ignored
        public bool SearchLocally = true;
        public bool SearchRSS = false;
        public bool ShowEpisodePictures = true;
        public bool ShowInTaskbar = true;
        public string SpecialsFolderName = "Specials";
        public int StartupTab = 0;
        public Searchers TheSearchers = new Searchers();
        private string[] VideoExtensionsArray = new string[0];
        private string VideoExtensionsString = "";
        public int WTWRecentDays = 7;
        public string uTorrentPath = "";
        public bool MonitorFolders = false;
        public ShowStatusColoringTypeList ShowStatusColors = new ShowStatusColoringTypeList();

        public TVSettings()
        {
            this.SetToDefaults();
        }

        public TVSettings(XmlReader reader)
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
                            this.Replacements.Add(new Replacement(reader.GetAttribute("This"), reader.GetAttribute("That"), reader.GetAttribute("CaseInsensitive") == "Y"));
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
                    this.SetVideoExtensionsString(reader.ReadElementContentAsString());
                else if (reader.Name == "OtherExtensions")
                    this.SetOtherExtensionsString(reader.ReadElementContentAsString());
                else if (reader.Name == "ExportRSSMaxDays")
                    this.ExportRSSMaxDays = reader.ReadElementContentAsInt();
                else if (reader.Name == "ExportRSSMaxShows")
                    this.ExportRSSMaxShows = reader.ReadElementContentAsInt();
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
                    this.EpImgs = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "NFOs")
                    this.NFOs = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "pyTivoMeta")
                    this.pyTivoMeta = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "pyTivoMetaSubFolder")
                    this.pyTivoMetaSubFolder = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "FolderJpg")
                    this.FolderJpg = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "FolderJpgIs")
                    this.FolderJpgIs = (FolderJpgIsType) reader.ReadElementContentAsInt();
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
                            bool en = s != null ? bool.Parse(s) : true;

                            this.FNPRegexs.Add(new FilenameProcessorRE(en, reader.GetAttribute("RE"), bool.Parse(reader.GetAttribute("UseFullPath")), reader.GetAttribute("Notes")));
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
                            { }

                            string color = reader.GetAttribute("Color");
                            if (type != null && !string.IsNullOrEmpty(color))
                            {
                                try
                                {
                                    System.Drawing.Color c = System.Drawing.ColorTranslator.FromHtml(color);
                                    this.ShowStatusColors.Add(type, c);
                                }
                                catch { }
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

        // ========================================================================================

        public string ItemForFolderJpg()
        {
            switch (this.FolderJpgIs)
            {
                case FolderJpgIsType.Banner:
                    return "banner";
                case FolderJpgIsType.FanArt:
                    return "fanart";
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

        public void SetVideoExtensionsString(string s)
        {
            if (OKExtensionsString(s))
            {
                s = s.ToLower();
                this.VideoExtensionsString = s;
                this.VideoExtensionsArray = s.Split(';');
            }
        }

        public void SetOtherExtensionsString(string s)
        {
            if (OKExtensionsString(s))
            {
                s = s.ToLower();
                this.OtherExtensionsString = s;
                this.OtherExtensionsArray = s.Split(';');
            }
        }

        public static string CompulsoryReplacements()
        {
            return "*?<>:/\\|\""; // invalid filename characters, must be in the list!
        }

        public void SetToDefaults()
        {
            // defaults that aren't handled with default initialisers

            this.SetVideoExtensionsString(".avi;.mpg;.mpeg;.mkv;.mp4;.wmv;.divx;.ogm;.qt;.rm");
            this.SetOtherExtensionsString(".srt;.nfo;.txt;.tbn");

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
            FileInfo f2 = new FileInfo(System.Windows.Forms.Application.UserAppDataPath + "\\..\\..\\..\\uTorrent\\resume.dat");
            if (f2.Exists)
                this.ResumeDatPath = f2.FullName;
            else
                this.ResumeDatPath = "";
        }

        public static FNPRegexList DefaultFNPList()
        {
            // Default list of filename processors

            FNPRegexList l = new FNPRegexList {
                                                  new FilenameProcessorRE(true, "(^|[^a-z])s?(?<s>[0-9]+)[ex](?<e>[0-9]{2,})(e[0-9]{2,})*[^a-z]", false, "3x23 s3x23 3e23 s3e23 s04e01e02e03"),
                                                  new FilenameProcessorRE(false, "(^|[^a-z])s?(?<s>[0-9]+)(?<e>[0-9]{2,})[^a-z]", false, "323 or s323 for season 3, episode 23. 2004 for season 20, episode 4."),
                                                  new FilenameProcessorRE(false, "(^|[^a-z])s(?<s>[0-9]+)--e(?<e>[0-9]{2,})[^a-z]", false, "S02--E03"),
                                                  new FilenameProcessorRE(false, "(^|[^a-z])s(?<s>[0-9]+) e(?<e>[0-9]{2,})[^a-z]", false, "'S02.E04' and 'S02 E04'"),
                                                  new FilenameProcessorRE(false, "^(?<s>[0-9]+) (?<e>[0-9]{2,})", false, "filenames starting with '1.23' for season 1, episode 23"),
                                                  new FilenameProcessorRE(true, "(^|[^a-z])(?<s>[0-9])(?<e>[0-9]{2,})[^a-z]", false, "Show - 323 - Foo"),
                                                  new FilenameProcessorRE(true, "(^|[^a-z])se(?<s>[0-9]+)([ex]|ep|xep)?(?<e>[0-9]+)[^a-z]", false, "se3e23 se323 se1ep1 se01xep01..."),
                                                  new FilenameProcessorRE(true, "(^|[^a-z])(?<s>[0-9]+)-(?<e>[0-9]{2,})[^a-z]", false, "3-23 EpName"),
                                                  new FilenameProcessorRE(true, "(^|[^a-z])s(?<s>[0-9]+) +- +e(?<e>[0-9]{2,})[^a-z]", false, "ShowName - S01 - E01"),
                                                  new FilenameProcessorRE(true, "\\b(?<e>[0-9]{2,}) ?- ?.* ?- ?(?<s>[0-9]+)", false, "like '13 - Showname - 2 - Episode Title.avi'"),
                                                  new FilenameProcessorRE(true, "\\b(episode|ep|e) ?(?<e>[0-9]{2,}) ?- ?(series|season) ?(?<s>[0-9]+)", false, "episode 3 - season 4"),
                                                  new FilenameProcessorRE(true, "season (?<s>[0-9]+)\\\\e?(?<e>[0-9]{1,3}) ?-", true, "Show Season 3\\E23 - Epname"),
                                                  new FilenameProcessorRE(false, "season (?<s>[0-9]+)\\\\episode (?<e>[0-9]{1,3})", true, "Season 3\\Episode 23")
                                              };

            return l;
        }

        private static ReplacementList DefaultReplacementList()
        {
            return new ReplacementList {
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

        private static StringList DefaultRSSURLList()
        {
            StringList sl = new StringList {
                                               "http://tvrss.net/feed/eztv"
                                           };
            return sl;
        }

        public static string[] TabNames()
        {
            return new string[] { "MyShows", "Scan", "WTW" };
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

        public void WriteXML(XmlWriter writer)
        {
            writer.WriteStartElement("Settings");
            this.TheSearchers.WriteXML(writer);
            writer.WriteStartElement("BGDownload");
            writer.WriteValue(this.BGDownload);
            writer.WriteEndElement();
            writer.WriteStartElement("OfflineMode");
            writer.WriteValue(this.OfflineMode);
            writer.WriteEndElement();
            writer.WriteStartElement("Replacements");
            foreach (Replacement R in this.Replacements)
            {
                writer.WriteStartElement("Replace");
                writer.WriteStartAttribute("This");
                writer.WriteValue(R.This);
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("That");
                writer.WriteValue(R.That);
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("CaseInsensitive");
                writer.WriteValue(R.CaseInsensitive ? "Y" : "N");
                writer.WriteEndAttribute();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteStartElement("ExportWTWRSS");
            writer.WriteValue(this.ExportWTWRSS);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportWTWRSSTo");
            writer.WriteValue(this.ExportWTWRSSTo);
            writer.WriteEndElement();
            writer.WriteStartElement("WTWRecentDays");
            writer.WriteValue(this.WTWRecentDays);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportMissingXML");
            writer.WriteValue(this.ExportMissingXML);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportMissingXMLTo");
            writer.WriteValue(this.ExportMissingXMLTo);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportMissingCSV");
            writer.WriteValue(this.ExportMissingCSV);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportMissingCSVTo");
            writer.WriteValue(this.ExportMissingCSVTo);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportRenamingXML");
            writer.WriteValue(this.ExportRenamingXML);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportRenamingXMLTo");
            writer.WriteValue(this.ExportRenamingXMLTo);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportFOXML");
            writer.WriteValue(this.ExportFOXML);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportFOXMLTo");
            writer.WriteValue(this.ExportFOXMLTo);
            writer.WriteEndElement();
            writer.WriteStartElement("StartupTab2");
            writer.WriteValue(TabNameForNumber(this.StartupTab));
            writer.WriteEndElement();
            writer.WriteStartElement("NamingStyle");
            writer.WriteValue(this.NamingStyle.StyleString);
            writer.WriteEndElement();
            writer.WriteStartElement("NotificationAreaIcon");
            writer.WriteValue(this.NotificationAreaIcon);
            writer.WriteEndElement();
            writer.WriteStartElement("VideoExtensions");
            writer.WriteValue(this.VideoExtensionsString);
            writer.WriteEndElement();
            writer.WriteStartElement("OtherExtensions");
            writer.WriteValue(this.OtherExtensionsString);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportRSSMaxDays");
            writer.WriteValue(this.ExportRSSMaxDays);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportRSSMaxShows");
            writer.WriteValue(this.ExportRSSMaxShows);
            writer.WriteEndElement();
            writer.WriteStartElement("KeepTogether");
            writer.WriteValue(this.KeepTogether);
            writer.WriteEndElement();
            writer.WriteStartElement("LeadingZeroOnSeason");
            writer.WriteValue(this.LeadingZeroOnSeason);
            writer.WriteEndElement();
            writer.WriteStartElement("ShowInTaskbar");
            writer.WriteValue(this.ShowInTaskbar);
            writer.WriteEndElement();
            writer.WriteStartElement("IgnoreSamples");
            writer.WriteValue(this.IgnoreSamples);
            writer.WriteEndElement();
            writer.WriteStartElement("ForceLowercaseFilenames");
            writer.WriteValue(this.ForceLowercaseFilenames);
            writer.WriteEndElement();
            writer.WriteStartElement("RenameTxtToSub");
            writer.WriteValue(this.RenameTxtToSub);
            writer.WriteEndElement();
            writer.WriteStartElement("ParallelDownloads");
            writer.WriteValue(this.ParallelDownloads);
            writer.WriteEndElement();
            writer.WriteStartElement("AutoSelectShowInMyShows");
            writer.WriteValue(this.AutoSelectShowInMyShows);
            writer.WriteEndElement();
            writer.WriteStartElement("ShowEpisodePictures");
            writer.WriteValue(this.ShowEpisodePictures);
            writer.WriteEndElement();
            writer.WriteStartElement("SpecialsFolderName");
            writer.WriteValue(this.SpecialsFolderName);
            writer.WriteEndElement();
            writer.WriteStartElement("uTorrentPath");
            writer.WriteValue(this.uTorrentPath);
            writer.WriteEndElement();
            writer.WriteStartElement("ResumeDatPath");
            writer.WriteValue(this.ResumeDatPath);
            writer.WriteEndElement();
            writer.WriteStartElement("SearchRSS");
            writer.WriteValue(this.SearchRSS);
            writer.WriteEndElement();
            writer.WriteStartElement("EpImgs");
            writer.WriteValue(this.EpImgs);
            writer.WriteEndElement();
            writer.WriteStartElement("NFOs");
            writer.WriteValue(this.NFOs);
            writer.WriteEndElement();
            writer.WriteStartElement("pyTivoMeta");
            writer.WriteValue(this.pyTivoMeta);
            writer.WriteEndElement();
            writer.WriteStartElement("pyTivoMetaSubFolder");
            writer.WriteValue(this.pyTivoMetaSubFolder);
            writer.WriteEndElement();
            writer.WriteStartElement("FolderJpg");
            writer.WriteValue(this.FolderJpg);
            writer.WriteEndElement();
            writer.WriteStartElement("FolderJpgIs");
            writer.WriteValue((int) this.FolderJpgIs);
            writer.WriteEndElement();
            writer.WriteStartElement("CheckuTorrent");
            writer.WriteValue(this.CheckuTorrent);
            writer.WriteEndElement();
            writer.WriteStartElement("RenameCheck");
            writer.WriteValue(this.RenameCheck);
            writer.WriteEndElement();
            writer.WriteStartElement("MissingCheck");
            writer.WriteValue(this.MissingCheck);
            writer.WriteEndElement();
            writer.WriteStartElement("SearchLocally");
            writer.WriteValue(this.SearchLocally);
            writer.WriteEndElement();
            writer.WriteStartElement("LeaveOriginals");
            writer.WriteValue(this.LeaveOriginals);
            writer.WriteEndElement();
            writer.WriteStartElement("LookForDateInFilename");
            writer.WriteValue(this.LookForDateInFilename);
            writer.WriteEndElement();
            writer.WriteStartElement("MonitorFolders");
            writer.WriteValue(this.MonitorFolders);
            writer.WriteEndElement();

            writer.WriteStartElement("FNPRegexs");
            foreach (FilenameProcessorRE re in this.FNPRegexs)
            {
                writer.WriteStartElement("Regex");
                writer.WriteStartAttribute("Enabled");
                writer.WriteValue(re.Enabled);
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("RE");
                writer.WriteValue(re.RE);
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("UseFullPath");
                writer.WriteValue(re.UseFullPath);
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("Notes");
                writer.WriteValue(re.Notes);
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // Regex
            }
            writer.WriteEndElement(); // FNPRegexs
            writer.WriteStartElement("RSSURLs");
            foreach (string s in this.RSSURLs)
            {
                writer.WriteStartElement("URL");
                writer.WriteValue(s);
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // RSSURL

            if (ShowStatusColors != null)
            {
                writer.WriteStartElement("ShowStatusTVWColors");
                foreach (System.Collections.Generic.KeyValuePair<ShowStatusColoringType, System.Drawing.Color> e in this.ShowStatusColors)
                {
                    writer.WriteStartElement("ShowStatusTVWColor");
                    // TODO ... Write Meta Flags
                    writer.WriteStartAttribute("IsMeta");
                    writer.WriteValue(e.Key.IsMetaType);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("IsShowLevel");
                    writer.WriteValue(e.Key.IsShowLevel);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("ShowStatus");
                    writer.WriteValue(e.Key.Status);
                    writer.WriteEndAttribute();

                    writer.WriteStartAttribute("Color");
                    writer.WriteValue(Helpers.TranslateColorToHtml(e.Value));
                    writer.WriteEndAttribute();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement(); // ShowStatusTVWColors
            }

            writer.WriteEndElement(); // settings
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

            string url = this.TheSearchers.CurrentSearchURL();
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
    }
}
