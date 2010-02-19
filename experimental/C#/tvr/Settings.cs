using System.Collections;
using System.Xml;
//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//

using System.IO;
using System;

namespace TVRename
{

//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//	ref class ProcessedEpisode;

	public class Replacement
	{
		public string This;
		public string That;
		public bool CaseInsensitive;

		public Replacement(string a, string b, bool insens)
		{
			if (b == null)
				b = "";
			This = a;
			That = b;
			CaseInsensitive = insens;
		}
	}


//C++ TO C# CONVERTER NOTE: Enums must be named in C#, so the following enum has been named AnonymousEnum:
	public enum AnonymousEnum4: int
	{
		reSeasonEp,
		reEpSeason,
		reUsingPathSeasonEp
	}

	public class FilenameProcessorRE
	{
		public bool Enabled;
		public string RE;
		public bool UseFullPath;
		public string Notes;

		public FilenameProcessorRE(bool enabled, string re, bool useFullPath, string notes)
		{
			Enabled = enabled;
			RE = re;
			UseFullPath = useFullPath;
			Notes = notes;
		}
	}

	public class  FNPRegexList : System.Collections.Generic.List<FilenameProcessorRE>  
    {
    }

    public class TVSettings
    {
        public enum FolderJpgIsType : int
        {
            Banner,
            Poster,
            FanArt
        }

        private string VideoExtensionsString;
        private string OtherExtensionsString;
        private string[] VideoExtensionsArray;
        private string[] OtherExtensionsArray;

        public int ParallelDownloads;
        public FNPRegexList FNPRegexs;
        public bool BGDownload;
        public Searchers TheSearchers;
        public bool OfflineMode; // TODO: Make property of thetvdb?
        public System.Collections.Generic.List<Replacement> Replacements;
        public bool ExportWTWRSS;
        public string ExportWTWRSSTo;
        public int WTWRecentDays;
        public int StartupTab;
        public FolderJpgIsType FolderJpgIs;
        public CustomName NamingStyle;
        public bool NotificationAreaIcon;
        public int ExportRSSMaxDays;
        public int ExportRSSMaxShows;
        public bool KeepTogether;
        public bool LeadingZeroOnSeason;
        public bool ShowInTaskbar;
        public bool RenameTxtToSub;
        public bool ShowEpisodePictures;
        public bool AutoSelectShowInMyShows;
        public string SpecialsFolderName;
        public StringList RSSURLs;

        public bool ExportMissingXML;
        public string ExportMissingXMLTo;
        public bool ExportMissingCSV;
        public string ExportMissingCSVTo;
        public bool ExportRenamingXML;
        public string ExportRenamingXMLTo;
        public bool ExportFOXML;
        public string ExportFOXMLTo;

        public bool ForceLowercaseFilenames;
        public bool IgnoreSamples;
        public string uTorrentPath;
        public string ResumeDatPath;

        public int SampleFileMaxSizeMB; // sample file must be smaller than this to be ignored

        public bool SearchRSS;
        public bool EpImgs;
        public bool NFOs;
        public bool FolderJpg;
        public bool RenameCheck;
        public bool CheckuTorrent;
        public bool MissingCheck;
        public bool SearchLocally;
        public bool LeaveOriginals;

        // ========================================================================================


        public string ItemForFolderJpg()
        {
            switch (FolderJpgIs)
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
            return VideoExtensionsString;
        }
        public string GetOtherExtensionsString()
        {
            return OtherExtensionsString;
        }

        public static bool OKExtensionsString(string s)
        {
            string[] t = s.Split(';');
            foreach (string s2 in t)
                if ((string.IsNullOrEmpty(s2)) || (!s2.StartsWith(".")))
                    return false;
            return true;
        }
        public void SetVideoExtensionsString(string s)
        {
            if (OKExtensionsString(s))
            {
                s = s.ToLower();
                VideoExtensionsString = s;
                VideoExtensionsArray = s.Split(';');
            }
        }
        public void SetOtherExtensionsString(string s)
        {
            if (OKExtensionsString(s))
            {
                s = s.ToLower();
                OtherExtensionsString = s;
                OtherExtensionsArray = s.Split(';');
            }
        }
        public TVSettings()
        {
            SetToDefaults();
        }
        public static string CompulsoryReplacements()
        {
            return "*?<>:/\\|\""; // invalid filename characters, must be in the list!
        }
        public void SetToDefaults()
        {
            FNPRegexs = new FNPRegexList();
            NamingStyle = new CustomName();
            Replacements = new System.Collections.Generic.List<Replacement>();
            BGDownload = false;
            TheSearchers = new Searchers();
            OfflineMode = false;
            NotificationAreaIcon = false;
            Replacements.Clear();
            Replacements.Add(new Replacement("*", "#", false));
            Replacements.Add(new Replacement("?", "", false));
            Replacements.Add(new Replacement(">", "", false));
            Replacements.Add(new Replacement("<", "", false));
            Replacements.Add(new Replacement(":", "-", false));
            Replacements.Add(new Replacement("/", "-", false));
            Replacements.Add(new Replacement("\\", "-", false));
            Replacements.Add(new Replacement("|", "-", false));
            Replacements.Add(new Replacement("\"", "'", false));

            ExportWTWRSS = false;
            ExportWTWRSSTo = "";
            WTWRecentDays = 7;

            ExportMissingXML = false;
            ExportMissingXMLTo = "";
            ExportMissingCSV = false;
            ExportMissingCSVTo = "";
            ExportRenamingXML = false;
            ExportRenamingXMLTo = "";
            ExportFOXML = false;
            ExportFOXMLTo = "";

            ForceLowercaseFilenames = false;
            IgnoreSamples = true;
            SampleFileMaxSizeMB = 50;

            StartupTab = 0;
            ExportRSSMaxDays = 7;
            ExportRSSMaxShows = 10;
            //DefaultNamingStyle = NStyle::Style::Name_SxxEyy_EpName;
            SetVideoExtensionsString(".avi;.mpg;.mpeg;.mkv;.mp4;.wmv;.divx;.ogm;.qt;.rm");
            SetOtherExtensionsString(".srt;.nfo;.txt");
            KeepTogether = true;
            LeadingZeroOnSeason = false;
            ShowInTaskbar = true;
            RenameTxtToSub = false;
            ShowEpisodePictures = false;
            AutoSelectShowInMyShows = false;
            SpecialsFolderName = "Specials";

            ParallelDownloads = 4;

            FNPRegexs = DefaultFNPList();

            RSSURLs = new StringList();
            RSSURLs.Add("http://tvrss.net/feed/eztv");

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
            FileInfo f2 = new FileInfo(System.Windows.Forms.Application.UserAppDataPath + "\\..\\..\\..\\uTorrent\\resume.dat");
            if (f2.Exists)
                ResumeDatPath = f2.FullName;
            else
                ResumeDatPath = "";

            SearchRSS = false;
            EpImgs = false;
            NFOs = false;
            FolderJpg = false;
            RenameCheck = true;
            CheckuTorrent = false;
            MissingCheck = true;
            SearchLocally = true;
            LeaveOriginals = false;

            FolderJpgIs = FolderJpgIsType.Poster;
        }

        public static FNPRegexList DefaultFNPList()
        {
            FNPRegexList l = new FNPRegexList();

            l.Add(new FilenameProcessorRE(true, "(^|[^a-z])s?(?<s>[0-9]+)[ex](?<e>[0-9]{2,})[^a-z]", false, "3x23 s3x23 3e23 s3e23"));
            l.Add(new FilenameProcessorRE(false, "(^|[^a-z])s?(?<s>[0-9]+)(?<e>[0-9]{2,})[^a-z]", false, "323 or s323 for season 3, episode 23. 2004 for season 20, episode 4."));
            l.Add(new FilenameProcessorRE(false, "(^|[^a-z])s(?<s>[0-9]+)--e(?<e>[0-9]{2,})[^a-z]", false, "S02--E03"));
            l.Add(new FilenameProcessorRE(false, "(^|[^a-z])s(?<s>[0-9]+) e(?<e>[0-9]{2,})[^a-z]", false, "'S02.E04' and 'S02 E04'"));
            l.Add(new FilenameProcessorRE(false, "^(?<s>[0-9]+) (?<e>[0-9]{2,})", false, "filenames starting with '1.23' for season 1, episode 23"));


            l.Add(new FilenameProcessorRE(true, "(^|[^a-z])(?<s>[0-9])(?<e>[0-9]{2,})[^a-z]", false, "Show - 323 - Foo"));
            l.Add(new FilenameProcessorRE(true, "(^|[^a-z])se(?<s>[0-9]+)([ex]|ep|xep)?(?<e>[0-9]+)[^a-z]", false, "se3e23 se323 se1ep1 se01xep01..."));
            l.Add(new FilenameProcessorRE(true, "(^|[^a-z])(?<s>[0-9]+)-(?<e>[0-9]{2,})[^a-z]", false, "3-23 EpName"));
            l.Add(new FilenameProcessorRE(true, "(^|[^a-z])s(?<s>[0-9]+) +- +e(?<e>[0-9]{2,})[^a-z]", false, "ShowName - S01 - E01"));

            l.Add(new FilenameProcessorRE(true, "\\b(?<e>[0-9]{2,}) ?- ?.* ?- ?(?<s>[0-9]+)", false, "like '13 - Showname - 2 - Episode Title.avi'"));
            l.Add(new FilenameProcessorRE(true, "\\b(episode|ep|e) ?(?<e>[0-9]{2,}) ?- ?(series|season) ?(?<s>[0-9]+)", false, "episode 3 - season 4"));

            l.Add(new FilenameProcessorRE(true, "season (?<s>[0-9]+)\\\\e?(?<e>[0-9]{1,3}) ?-", true, "Show Season 3\\E23 - Epname"));
            l.Add(new FilenameProcessorRE(false, "season (?<s>[0-9]+)\\\\episode (?<e>[0-9]{1,3})", true, "Season 3\\Episode 23"));

            return l;
        }

        public static string[] TabNames()
        {
            return new string[3] { "MyShows", "Scan", "WTW" };
        }
        public static string TabNameForNumber(int n)
        {
            if ((n >= 0) && (n < TabNames().Length))
                return TabNames()[n];
            else
                return "";
        }
        public static int TabNumberFromName(string n)
        {
            int r = 0;
            if (!string.IsNullOrEmpty(n))
                r = Array.IndexOf<string>(TabNames(), n);
            if (r < 0)
                r = 0;
            return r;
        }

        public TVSettings(XmlReader reader)
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
                else if (reader.Name == "Replacements")
                {
                    Replacements.Clear();
                    reader.Read();
                    while (!reader.EOF)
                    {
                        if ((reader.Name == "Replacements") && (!reader.IsStartElement()))
                            break;
                        if (reader.Name == "Replace")
                        {
                            Replacements.Add(new Replacement(reader.GetAttribute("This"), reader.GetAttribute("That"), reader.GetAttribute("CaseInsensitive") == "Y"));
                            reader.Read();
                        }
                        else
                            reader.ReadOuterXml();
                    }
                    reader.Read();
                }
                else if (reader.Name == "ExportWTWRSS")
                    ExportWTWRSS = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ExportWTWRSSTo")
                    ExportWTWRSSTo = reader.ReadElementContentAsString();
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
                    SetVideoExtensionsString(reader.ReadElementContentAsString());
                else if (reader.Name == "OtherExtensions")
                    SetOtherExtensionsString(reader.ReadElementContentAsString());
                else if (reader.Name == "ExportRSSMaxDays")
                    ExportRSSMaxDays = reader.ReadElementContentAsInt();
                else if (reader.Name == "ExportRSSMaxShows")
                    ExportRSSMaxShows = reader.ReadElementContentAsInt();
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
                else if (reader.Name == "AutoSelectShowInMyShows")
                    AutoSelectShowInMyShows = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SpecialsFolderName")
                    SpecialsFolderName = reader.ReadElementContentAsString();
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
                    EpImgs = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "NFOs")
                    NFOs = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "FolderJpg")
                    FolderJpg = reader.ReadElementContentAsBoolean();
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
                else if (reader.Name == "FNPRegexs")
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
                            bool en = s != null ? bool.Parse(s) : true;

                            FNPRegexs.Add(new FilenameProcessorRE(en, reader.GetAttribute("RE"), bool.Parse(reader.GetAttribute("UseFullPath")), reader.GetAttribute("Notes")));
                            reader.Read();
                        }
                        else
                            reader.ReadOuterXml();
                    }
                    reader.Read();
                }
                else if (reader.Name == "RSSURLs")
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
                else
                    reader.ReadOuterXml();
            }
        }

        public void WriteXML(XmlWriter writer)
        {
            writer.WriteStartElement("Settings");
            TheSearchers.WriteXML(writer);
            writer.WriteStartElement("BGDownload");
            writer.WriteValue(BGDownload);
            writer.WriteEndElement();
            writer.WriteStartElement("OfflineMode");
            writer.WriteValue(OfflineMode);
            writer.WriteEndElement();
            writer.WriteStartElement("Replacements");
            foreach (Replacement R in Replacements)
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
            writer.WriteValue(ExportWTWRSS);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportWTWRSSTo");
            writer.WriteValue(ExportWTWRSSTo);
            writer.WriteEndElement();
            writer.WriteStartElement("WTWRecentDays");
            writer.WriteValue(WTWRecentDays);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportMissingXML");
            writer.WriteValue(ExportMissingXML);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportMissingXMLTo");
            writer.WriteValue(ExportMissingXMLTo);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportMissingCSV");
            writer.WriteValue(ExportMissingCSV);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportMissingCSVTo");
            writer.WriteValue(ExportMissingCSVTo);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportRenamingXML");
            writer.WriteValue(ExportRenamingXML);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportRenamingXMLTo");
            writer.WriteValue(ExportRenamingXMLTo);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportFOXML");
            writer.WriteValue(ExportFOXML);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportFOXMLTo");
            writer.WriteValue(ExportFOXMLTo);
            writer.WriteEndElement();
            writer.WriteStartElement("StartupTab2");
            writer.WriteValue(TabNameForNumber(StartupTab));
            writer.WriteEndElement();
            writer.WriteStartElement("NamingStyle");
            writer.WriteValue(NamingStyle.StyleString);
            writer.WriteEndElement();
            writer.WriteStartElement("NotificationAreaIcon");
            writer.WriteValue(NotificationAreaIcon);
            writer.WriteEndElement();
            writer.WriteStartElement("VideoExtensions");
            writer.WriteValue(VideoExtensionsString);
            writer.WriteEndElement();
            writer.WriteStartElement("OtherExtensions");
            writer.WriteValue(OtherExtensionsString);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportRSSMaxDays");
            writer.WriteValue(ExportRSSMaxDays);
            writer.WriteEndElement();
            writer.WriteStartElement("ExportRSSMaxShows");
            writer.WriteValue(ExportRSSMaxShows);
            writer.WriteEndElement();
            writer.WriteStartElement("KeepTogether");
            writer.WriteValue(KeepTogether);
            writer.WriteEndElement();
            writer.WriteStartElement("LeadingZeroOnSeason");
            writer.WriteValue(LeadingZeroOnSeason);
            writer.WriteEndElement();
            writer.WriteStartElement("ShowInTaskbar");
            writer.WriteValue(ShowInTaskbar);
            writer.WriteEndElement();
            writer.WriteStartElement("IgnoreSamples");
            writer.WriteValue(IgnoreSamples);
            writer.WriteEndElement();
            writer.WriteStartElement("ForceLowercaseFilenames");
            writer.WriteValue(ForceLowercaseFilenames);
            writer.WriteEndElement();
            writer.WriteStartElement("RenameTxtToSub");
            writer.WriteValue(RenameTxtToSub);
            writer.WriteEndElement();
            writer.WriteStartElement("ParallelDownloads");
            writer.WriteValue(ParallelDownloads);
            writer.WriteEndElement();
            writer.WriteStartElement("AutoSelectShowInMyShows");
            writer.WriteValue(AutoSelectShowInMyShows);
            writer.WriteEndElement();
            writer.WriteStartElement("ShowEpisodePictures");
            writer.WriteValue(ShowEpisodePictures);
            writer.WriteEndElement();
            writer.WriteStartElement("SpecialsFolderName");
            writer.WriteValue(SpecialsFolderName);
            writer.WriteEndElement();
            writer.WriteStartElement("uTorrentPath");
            writer.WriteValue(uTorrentPath);
            writer.WriteEndElement();
            writer.WriteStartElement("ResumeDatPath");
            writer.WriteValue(ResumeDatPath);
            writer.WriteEndElement();
            writer.WriteStartElement("SearchRSS");
            writer.WriteValue(SearchRSS);
            writer.WriteEndElement();
            writer.WriteStartElement("EpImgs");
            writer.WriteValue(EpImgs);
            writer.WriteEndElement();
            writer.WriteStartElement("NFOs");
            writer.WriteValue(NFOs);
            writer.WriteEndElement();
            writer.WriteStartElement("FolderJpg");
            writer.WriteValue(FolderJpg);
            writer.WriteEndElement();
            writer.WriteStartElement("CheckuTorrent");
            writer.WriteValue(CheckuTorrent);
            writer.WriteEndElement();
            writer.WriteStartElement("RenameCheck");
            writer.WriteValue(RenameCheck);
            writer.WriteEndElement();
            writer.WriteStartElement("MissingCheck");
            writer.WriteValue(MissingCheck);
            writer.WriteEndElement();
            writer.WriteStartElement("SearchLocally");
            writer.WriteValue(SearchLocally);
            writer.WriteEndElement();
            writer.WriteStartElement("LeaveOriginals");
            writer.WriteValue(LeaveOriginals);
            writer.WriteEndElement();

            writer.WriteStartElement("FNPRegexs");
            foreach (FilenameProcessorRE re in FNPRegexs)
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
            foreach (string s in RSSURLs)
            {
                writer.WriteStartElement("URL");
                writer.WriteValue(s);
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // RSSURL

            writer.WriteEndElement(); // settings
        }

        public bool UsefulExtension(string sn, bool otherExtensionsToo)
        {
            foreach (string s in VideoExtensionsArray)
                if (sn.ToLower() == s)
                    return true;
            if (otherExtensionsToo)
                foreach (string s in OtherExtensionsArray)
                    if (sn.ToLower() == s)
                        return true;

            return false;
        }
        public string BTSearchURL(ProcessedEpisode epi)
        {
            if (epi == null)
                return "";

            SeriesInfo s = epi.TheSeries;
            if (s == null)
                return "";

            string url = TheSearchers.CurrentSearchURL();
            return CustomName.NameForNoExt(epi, url, true);
        }
    }
} // namespace