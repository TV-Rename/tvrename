using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TVRename.db_access;
using TVRename.db_access.documents;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace TVRename.Settings
{
    public class Config : IEntity<ConfigDocument>
    {
        public ConfigDocument innerDocument;

        public Config() { }
        public Config(XmlReader reader)
        {
            if (this.innerDocument == null) innerDocument = new ConfigDocument();
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
                    this.innerDocument.TheSearchers.CurrentSearch = srch;
                }
                else if (reader.Name == "TheSearchers")
                {
                    this.innerDocument.TheSearchers = new Searchers(reader.ReadSubtree());
                    reader.Read();
                }
                else if (reader.Name == "BGDownload")
                    this.innerDocument.BGDownload = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "OfflineMode")
                    this.innerDocument.OfflineMode = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "Replacements")
                {
                    this.innerDocument.Replacements.Clear();
                    reader.Read();
                    while (!reader.EOF)
                    {
                        if ((reader.Name == "Replacements") && (!reader.IsStartElement()))
                            break;
                        if (reader.Name == "Replace")
                        {
                            this.innerDocument.Replacements.Add(new Replacement(reader.GetAttribute("This"), reader.GetAttribute("That"), reader.GetAttribute("CaseInsensitive") == "Y"));
                            reader.Read();
                        }
                        else
                            reader.ReadOuterXml();
                    }
                    reader.Read();
                }
                else if (reader.Name == "ExportWTWRSS")
                    this.innerDocument.ExportWTWRSS = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportWTWRSSTo")
                    this.innerDocument.ExportWTWRSSTo = reader.ReadElementContentAsString();
                else if (reader.Name == "WTWRecentDays")
                    this.innerDocument.WTWRecentDays = reader.ReadElementContentAsInt();
                else if (reader.Name == "StartupTab")
                {
                    int n = reader.ReadElementContentAsInt();
                    if (n == 6)
                        this.innerDocument.StartupTab = 2; // WTW is moved
                    else if ((n >= 1) && (n <= 3)) // any of the three scans
                        this.innerDocument.StartupTab = 1;
                    else
                        this.innerDocument.StartupTab = 0; // otherwise, My Shows
                }
                else if (reader.Name == "StartupTab2")
                    this.innerDocument.StartupTab = TabNumberFromName(reader.ReadElementContentAsString());
                else if (reader.Name == "DefaultNamingStyle") // old naming style
                    this.innerDocument.NamingStyle.StyleString = CustomName.OldNStyle(reader.ReadElementContentAsInt());
                else if (reader.Name == "NamingStyle")
                    this.innerDocument.NamingStyle.StyleString = reader.ReadElementContentAsString();
                else if (reader.Name == "NotificationAreaIcon")
                    this.innerDocument.NotificationAreaIcon = reader.ReadElementContentAsBoolean();
                else if ((reader.Name == "GoodExtensions") || (reader.Name == "VideoExtensions"))
                    this.SetVideoExtensionsString(reader.ReadElementContentAsString());
                else if (reader.Name == "OtherExtensions")
                    this.SetOtherExtensionsString(reader.ReadElementContentAsString());
                else if (reader.Name == "ExportRSSMaxDays")
                    this.innerDocument.ExportRSSMaxDays = reader.ReadElementContentAsInt();
                else if (reader.Name == "ExportRSSMaxShows")
                    this.innerDocument.ExportRSSMaxShows = reader.ReadElementContentAsInt();
                else if (reader.Name == "KeepTogether")
                    this.innerDocument.KeepTogether = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "LeadingZeroOnSeason")
                    this.innerDocument.LeadingZeroOnSeason = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ShowInTaskbar")
                    this.innerDocument.ShowInTaskbar = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "RenameTxtToSub")
                    this.innerDocument.RenameTxtToSub = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ShowEpisodePictures")
                    this.innerDocument.ShowEpisodePictures = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "AutoSelectShowInMyShows")
                    this.innerDocument.AutoSelectShowInMyShows = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SpecialsFolderName")
                    this.innerDocument.SpecialsFolderName = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportMissingXML")
                    this.innerDocument.ExportMissingXML = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportMissingXMLTo")
                    this.innerDocument.ExportMissingXMLTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportMissingCSV")
                    this.innerDocument.ExportMissingCSV = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportMissingCSVTo")
                    this.innerDocument.ExportMissingCSVTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportRenamingXML")
                    this.innerDocument.ExportRenamingXML = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportRenamingXMLTo")
                    this.innerDocument.ExportRenamingXMLTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ExportFOXML")
                    this.innerDocument.ExportFOXML = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportFOXMLTo")
                    this.innerDocument.ExportFOXMLTo = reader.ReadElementContentAsString();
                else if (reader.Name == "ForceLowercaseFilenames")
                    this.innerDocument.ForceLowercaseFilenames = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "IgnoreSamples")
                    this.innerDocument.IgnoreSamples = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SampleFileMaxSizeMB")
                    this.innerDocument.SampleFileMaxSizeMB = reader.ReadElementContentAsInt();
                else if (reader.Name == "ParallelDownloads")
                    this.innerDocument.ParallelDownloads = reader.ReadElementContentAsInt();
                else if (reader.Name == "uTorrentPath")
                    this.innerDocument.uTorrentPath = reader.ReadElementContentAsString();
                else if (reader.Name == "ResumeDatPath")
                    this.innerDocument.ResumeDatPath = reader.ReadElementContentAsString();
                else if (reader.Name == "SearchRSS")
                    this.innerDocument.SearchRSS = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "EpImgs")
                    this.innerDocument.EpImgs = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "NFOs")
                    this.innerDocument.NFOs = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "FolderJpg")
                    this.innerDocument.FolderJpg = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "FolderJpgIs")
                    this.innerDocument.FolderJpgIs = (TVRename.db_access.documents.ConfigDocument.FolderJpgIsType)reader.ReadElementContentAsInt();
                else if (reader.Name == "RenameCheck")
                    this.innerDocument.RenameCheck = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "CheckuTorrent")
                    this.innerDocument.CheckuTorrent = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "MissingCheck")
                    this.innerDocument.MissingCheck = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SearchLocally")
                    this.innerDocument.SearchLocally = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "LeaveOriginals")
                    this.innerDocument.LeaveOriginals = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "LookForDateInFilename")
                    this.innerDocument.LookForDateInFilename = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "MonitorFolders")
                    this.innerDocument.MonitorFolders = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "FNPRegexs")
                {
                    this.innerDocument.FNPRegexs.Clear();
                    reader.Read();
                    while (!reader.EOF)
                    {
                        if ((reader.Name == "FNPRegexs") && (!reader.IsStartElement()))
                            break;
                        if (reader.Name == "Regex")
                        {
                            string s = reader.GetAttribute("Enabled");
                            bool en = s != null ? bool.Parse(s) : true;

                            this.innerDocument.FNPRegexs.Add(new FilenameProcessorRE(en, reader.GetAttribute("RE"), bool.Parse(reader.GetAttribute("UseFullPath")), reader.GetAttribute("Notes")));
                            reader.Read();
                        }
                        else
                            reader.ReadOuterXml();
                    }
                    reader.Read();
                }
                else if (reader.Name == "RSSURLs")
                {
                    this.innerDocument.RSSURLs.Clear();
                    reader.Read();
                    while (!reader.EOF)
                    {
                        if ((reader.Name == "RSSURLs") && (!reader.IsStartElement()))
                            break;
                        if (reader.Name == "URL")
                            this.innerDocument.RSSURLs.Add(reader.ReadElementContentAsString());
                        else
                            reader.ReadOuterXml();
                    }
                    reader.Read();
                }
                else if (reader.Name == "ShowStatusTVWColors")
                {
                    this.innerDocument.ShowStatusColors = new ShowStatusColoringTypeList();
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
                                    this.innerDocument.ShowStatusColors.Add(type, c);
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

            this.innerDocument.uTorrentPath = "";
            foreach (string g in guesses)
            {
                FileInfo f = new FileInfo(g);
                if (f.Exists)
                {
                    this.innerDocument.uTorrentPath = f.FullName;
                    break;
                }
            }

            // ResumeDatPath
            FileInfo f2 = new FileInfo(System.Windows.Forms.Application.UserAppDataPath + "\\..\\..\\..\\uTorrent\\resume.dat");
            if (f2.Exists)
                this.innerDocument.ResumeDatPath = f2.FullName;
            else
                this.innerDocument.ResumeDatPath = "";

            // default settings for the properties
            this.innerDocument.AutoSelectShowInMyShows = true;
            this.innerDocument.BGDownload = false;
            this.innerDocument.CheckuTorrent = false;
            this.innerDocument.EpImgs = false;
            this.innerDocument.ExportFOXML = false;
            this.innerDocument.ExportFOXMLTo = "";
            this.innerDocument.ExportMissingCSV = false;
            this.innerDocument.ExportMissingCSVTo = "";
            this.innerDocument.ExportMissingXML = false;
            this.innerDocument.ExportMissingXMLTo = "";
            this.innerDocument.ExportRSSMaxDays = 7;
            this.innerDocument.ExportRSSMaxShows = 10;
            this.innerDocument.ExportRenamingXML = false;
            this.innerDocument.ExportRenamingXMLTo = "";
            this.innerDocument.ExportWTWRSS = false;
            this.innerDocument.ExportWTWRSSTo = "";
            this.innerDocument.FNPRegexs = DefaultFNPList();
            this.innerDocument.FolderJpg = false;
            this.innerDocument.FolderJpgIs = TVRename.db_access.documents.ConfigDocument.FolderJpgIsType.Poster;
            this.innerDocument.ForceLowercaseFilenames = false;
            this.innerDocument.IgnoreSamples = true;
            this.innerDocument.KeepTogether = true;
            this.innerDocument.LeadingZeroOnSeason = false;
            this.innerDocument.LeaveOriginals = false;
            this.innerDocument.LookForDateInFilename = false;
            this.innerDocument.MissingCheck = true;
            this.innerDocument.NFOs = false;
            this.innerDocument.NamingStyle = new CustomName();
            this.innerDocument.NotificationAreaIcon = false;
            this.innerDocument.OfflineMode = false; // TODO: Make property of thetvdb?
            this.innerDocument.OtherExtensionsArray = new string[0];
            this.innerDocument.OtherExtensionsString = "";
            this.innerDocument.ParallelDownloads = 4;
            this.innerDocument.RSSURLs = DefaultRSSURLList();
            this.innerDocument.RenameCheck = true;
            this.innerDocument.RenameTxtToSub = false;
            this.innerDocument.Replacements = DefaultReplacementList();
            this.innerDocument.ResumeDatPath = "";
            this.innerDocument.SampleFileMaxSizeMB = 50; // sample file must be smaller than this to be ignored
            this.innerDocument.SearchLocally = true;
            this.innerDocument.SearchRSS = false;
            this.innerDocument.ShowEpisodePictures = true;
            this.innerDocument.ShowInTaskbar = true;
            this.innerDocument.SpecialsFolderName = "Specials";
            this.innerDocument.StartupTab = 0;
            this.innerDocument.TheSearchers = new Searchers();
            this.innerDocument.VideoExtensionsArray = new string[0];
            this.innerDocument.VideoExtensionsString = "";
            this.innerDocument.WTWRecentDays = 7;
            this.innerDocument.uTorrentPath = "";
            this.innerDocument.MonitorFolders = false;
            this.innerDocument.ShowStatusColors = new ShowStatusColoringTypeList();
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

        public void WriteXML(XmlWriter writer)
        {
            writer.WriteStartElement("Settings");
            this.innerDocument.TheSearchers.WriteXML(writer);
            writer.WriteStartElement("BGDownload");
            writer.WriteValue(this.innerDocument.BGDownload);
            writer.WriteEndElement();
            writer.WriteStartElement("OfflineMode");
            writer.WriteValue(this.innerDocument.OfflineMode);
            writer.WriteEndElement();
            writer.WriteStartElement("Replacements");
            foreach (Replacement R in this.innerDocument.Replacements)
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
            writer.WriteValue(this.innerDocument.ExportWTWRSS);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportWTWRSSTo");
            writer.WriteValue(this.innerDocument.ExportWTWRSSTo);
            writer.WriteEndElement();
            writer.WriteStartElement("WTWRecentDays");
            writer.WriteValue(this.innerDocument.WTWRecentDays);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportMissingXML");
            writer.WriteValue(this.innerDocument.ExportMissingXML);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportMissingXMLTo");
            writer.WriteValue(this.innerDocument.ExportMissingXMLTo);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportMissingCSV");
            writer.WriteValue(this.innerDocument.ExportMissingCSV);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportMissingCSVTo");
            writer.WriteValue(this.innerDocument.ExportMissingCSVTo);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportRenamingXML");
            writer.WriteValue(this.innerDocument.ExportRenamingXML);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportRenamingXMLTo");
            writer.WriteValue(this.innerDocument.ExportRenamingXMLTo);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportFOXML");
            writer.WriteValue(this.innerDocument.ExportFOXML);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportFOXMLTo");
            writer.WriteValue(this.innerDocument.ExportFOXMLTo);
            writer.WriteEndElement();
            writer.WriteStartElement("StartupTab2");
            writer.WriteValue(TabNameForNumber(this.innerDocument.StartupTab));
            writer.WriteEndElement();
            writer.WriteStartElement("NamingStyle");
            writer.WriteValue(this.innerDocument.NamingStyle.StyleString);
            writer.WriteEndElement();
            writer.WriteStartElement("NotificationAreaIcon");
            writer.WriteValue(this.innerDocument.NotificationAreaIcon);
            writer.WriteEndElement();
            writer.WriteStartElement("VideoExtensions");
            writer.WriteValue(this.innerDocument.VideoExtensionsString);
            writer.WriteEndElement();
            writer.WriteStartElement("OtherExtensions");
            writer.WriteValue(this.innerDocument.OtherExtensionsString);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportRSSMaxDays");
            writer.WriteValue(this.innerDocument.ExportRSSMaxDays);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportRSSMaxShows");
            writer.WriteValue(this.innerDocument.ExportRSSMaxShows);
            writer.WriteEndElement();
            writer.WriteStartElement("KeepTogether");
            writer.WriteValue(this.innerDocument.KeepTogether);
            writer.WriteEndElement();
            writer.WriteStartElement("LeadingZeroOnSeason");
            writer.WriteValue(this.innerDocument.LeadingZeroOnSeason);
            writer.WriteEndElement();
            writer.WriteStartElement("ShowInTaskbar");
            writer.WriteValue(this.innerDocument.ShowInTaskbar);
            writer.WriteEndElement();
            writer.WriteStartElement("IgnoreSamples");
            writer.WriteValue(this.innerDocument.IgnoreSamples);
            writer.WriteEndElement();
            writer.WriteStartElement("ForceLowercaseFilenames");
            writer.WriteValue(this.innerDocument.ForceLowercaseFilenames);
            writer.WriteEndElement();
            writer.WriteStartElement("RenameTxtToSub");
            writer.WriteValue(this.innerDocument.RenameTxtToSub);
            writer.WriteEndElement();
            writer.WriteStartElement("ParallelDownloads");
            writer.WriteValue(this.innerDocument.ParallelDownloads);
            writer.WriteEndElement();
            writer.WriteStartElement("AutoSelectShowInMyShows");
            writer.WriteValue(this.innerDocument.AutoSelectShowInMyShows);
            writer.WriteEndElement();
            writer.WriteStartElement("ShowEpisodePictures");
            writer.WriteValue(this.innerDocument.ShowEpisodePictures);
            writer.WriteEndElement();
            writer.WriteStartElement("SpecialsFolderName");
            writer.WriteValue(this.innerDocument.SpecialsFolderName);
            writer.WriteEndElement();
            writer.WriteStartElement("uTorrentPath");
            writer.WriteValue(this.innerDocument.uTorrentPath);
            writer.WriteEndElement();
            writer.WriteStartElement("ResumeDatPath");
            writer.WriteValue(this.innerDocument.ResumeDatPath);
            writer.WriteEndElement();
            writer.WriteStartElement("SearchRSS");
            writer.WriteValue(this.innerDocument.SearchRSS);
            writer.WriteEndElement();
            writer.WriteStartElement("EpImgs");
            writer.WriteValue(this.innerDocument.EpImgs);
            writer.WriteEndElement();
            writer.WriteStartElement("NFOs");
            writer.WriteValue(this.innerDocument.NFOs);
            writer.WriteEndElement();
            writer.WriteStartElement("FolderJpg");
            writer.WriteValue(this.innerDocument.FolderJpg);
            writer.WriteEndElement();
            writer.WriteStartElement("FolderJpgIs");
            writer.WriteValue((int)this.innerDocument.FolderJpgIs);
            writer.WriteEndElement();
            writer.WriteStartElement("CheckuTorrent");
            writer.WriteValue(this.innerDocument.CheckuTorrent);
            writer.WriteEndElement();
            writer.WriteStartElement("RenameCheck");
            writer.WriteValue(this.innerDocument.RenameCheck);
            writer.WriteEndElement();
            writer.WriteStartElement("MissingCheck");
            writer.WriteValue(this.innerDocument.MissingCheck);
            writer.WriteEndElement();
            writer.WriteStartElement("SearchLocally");
            writer.WriteValue(this.innerDocument.SearchLocally);
            writer.WriteEndElement();
            writer.WriteStartElement("LeaveOriginals");
            writer.WriteValue(this.innerDocument.LeaveOriginals);
            writer.WriteEndElement();
            writer.WriteStartElement("LookForDateInFilename");
            writer.WriteValue(this.innerDocument.LookForDateInFilename);
            writer.WriteEndElement();
            writer.WriteStartElement("MonitorFolders");
            writer.WriteValue(this.innerDocument.MonitorFolders);
            writer.WriteEndElement();

            writer.WriteStartElement("FNPRegexs");
            foreach (FilenameProcessorRE re in this.innerDocument.FNPRegexs)
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
            foreach (string s in this.innerDocument.RSSURLs)
            {
                writer.WriteStartElement("URL");
                writer.WriteValue(s);
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // RSSURL

            if (this.innerDocument.ShowStatusColors != null)
            {
                writer.WriteStartElement("ShowStatusTVWColors");
                foreach (System.Collections.Generic.KeyValuePair<ShowStatusColoringType, System.Drawing.Color> e in this.innerDocument.ShowStatusColors)
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

        ConfigDocument IEntity<ConfigDocument>.GetInnerDocument()
        {
            return this.innerDocument;
        }

        private static StringList DefaultRSSURLList()
        {
            StringList sl = new StringList {
                                               "http://tvrss.net/feed/eztv"
                                           };
            return sl;
        }

        public void SetVideoExtensionsString(string s)
        {
            if (OKExtensionsString(s))
            {
                s = s.ToLower();
                this.innerDocument.VideoExtensionsString = s;
                this.innerDocument.VideoExtensionsArray = s.Split(';');
            }
        }

        public void SetOtherExtensionsString(string s)
        {
            if (OKExtensionsString(s))
            {
                s = s.ToLower();
                this.innerDocument.OtherExtensionsString = s;
                this.innerDocument.OtherExtensionsArray = s.Split(';');
            }
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

        public bool UsefulExtension(string sn, bool otherExtensionsToo)
        {
            foreach (string s in this.innerDocument.VideoExtensionsArray)
            {
                if (sn.ToLower() == s)
                    return true;
            }
            if (otherExtensionsToo)
            {
                foreach (string s in this.innerDocument.OtherExtensionsArray)
                {
                    if (sn.ToLower() == s)
                        return true;
                }
            }

            return false;
        }

        public string FilenameFriendly(string fn)
        {
            foreach (Replacement R in this.innerDocument.Replacements)
            {
                if (R.CaseInsensitive)
                    fn = Regex.Replace(fn, Regex.Escape(R.This), Regex.Escape(R.That), RegexOptions.IgnoreCase);
                else
                    fn = fn.Replace(R.This, R.That);
            }
            if (this.innerDocument.ForceLowercaseFilenames)
                fn = fn.ToLower();
            return fn;
        }
    }
}
