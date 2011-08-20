using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TVRename.db_access.documents;
using System.IO;
using TVRename.Utility;
using TVRename.Shows;

namespace TVRename.db_access
{
    class DefaultObjectFactory
    {
        public static ConfigDocument getDefaultConfigDocument()
        {
            ConfigDocument cd = new ConfigDocument();
            // defaults that aren't handled with default initialisers

            cd = SetVideoExtensionsString(cd, ".avi;.mpg;.mpeg;.mkv;.mp4;.wmv;.divx;.ogm;.qt;.rm");
            cd = SetOtherExtensionsString(cd, ".srt;.nfo;.txt;.tbn");

            // have a guess at utorrent's path
            string[] guesses = new string[3];
            guesses[0] = System.Windows.Forms.Application.StartupPath + "\\..\\uTorrent\\uTorrent.exe";
            guesses[1] = "c:\\Program Files\\uTorrent\\uTorrent.exe";
            guesses[2] = "c:\\Program Files (x86)\\uTorrent\\uTorrent.exe";

            cd.uTorrentPath = "";
            foreach (string g in guesses)
            {
                FileInfo f = new FileInfo(g);
                if (f.Exists)
                {
                    cd.uTorrentPath = f.FullName;
                    break;
                }
            }

            // ResumeDatPath
            FileInfo f2 = new FileInfo(System.Windows.Forms.Application.UserAppDataPath + "\\..\\..\\..\\uTorrent\\resume.dat");
            if (f2.Exists)
                cd.ResumeDatPath = f2.FullName;
            else
                cd.ResumeDatPath = "";

            // default settings for the properties
            cd.AutoSelectShowInMyShows = true;
            cd.BGDownload = false;
            cd.CheckuTorrent = false;
            cd.EpImgs = false;
            cd.ExportFOXML = false;
            cd.ExportFOXMLTo = "";
            cd.ExportMissingCSV = false;
            cd.ExportMissingCSVTo = "";
            cd.ExportMissingXML = false;
            cd.ExportMissingXMLTo = "";
            cd.ExportRSSMaxDays = 7;
            cd.ExportRSSMaxShows = 10;
            cd.ExportRenamingXML = false;
            cd.ExportRenamingXMLTo = "";
            cd.ExportWTWRSS = false;
            cd.ExportWTWRSSTo = "";
            cd.FNPRegexs = DefaultFNPList();
            cd.FolderJpg = false;
            cd.FolderJpgIs = TVRename.db_access.documents.ConfigDocument.FolderJpgIsType.Poster;
            cd.ForceLowercaseFilenames = false;
            cd.IgnoreSamples = true;
            cd.KeepTogether = true;
            cd.LeadingZeroOnSeason = false;
            cd.LeaveOriginals = false;
            cd.LookForDateInFilename = false;
            cd.MissingCheck = true;
            cd.NFOs = false;
            cd.NamingStyle = new CustomName();
            cd.NotificationAreaIcon = false;
            cd.OfflineMode = false; // TODO: Make property of thetvdb?
            cd.OtherExtensionsArray = new string[0];
            cd.OtherExtensionsString = "";
            cd.ParallelDownloads = 4;
            cd.RSSURLs = DefaultRSSURLList();
            cd.RenameCheck = true;
            cd.RenameTxtToSub = false;
            cd.Replacements = DefaultReplacementList();
            cd.ResumeDatPath = "";
            cd.SampleFileMaxSizeMB = 50; // sample file must be smaller than this to be ignored
            cd.SearchLocally = true;
            cd.SearchRSS = false;
            cd.ShowEpisodePictures = true;
            cd.ShowInTaskbar = true;
            cd.SpecialsFolderName = "Specials";
            cd.StartupTab = 0;
            cd.TheSearchers = new Searchers();
            cd.VideoExtensionsArray = new string[0];
            cd.VideoExtensionsString = "";
            cd.WTWRecentDays = 7;
            cd.uTorrentPath = "";
            cd.MonitorFolders = false;
            cd.ShowStatusColors = new ShowStatusColoringTypeList();

            cd.Ignore = new System.Collections.Generic.List<IgnoreItem>();
            cd.MonitorFoldersList = new StringList();
            cd.IgnoreFoldersList = new StringList();
            cd.SearchFoldersList = new StringList();

            return cd;
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
        private static StringList DefaultRSSURLList()
        {
            StringList sl = new StringList {
                                               "http://tvrss.net/feed/eztv"
                                           };
            return sl;
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
        public static ConfigDocument SetVideoExtensionsString(ConfigDocument cd, string s)
        {
            if (FileNameUtility.OKExtensionsString(s))
            {
                s = s.ToLower();
                cd.VideoExtensionsString = s;
                cd.VideoExtensionsArray = s.Split(';');
            }
            return cd;
        }
        public static ConfigDocument SetOtherExtensionsString(ConfigDocument cd, string s)
        {
            if (FileNameUtility.OKExtensionsString(s))
            {
                s = s.ToLower();
                cd.OtherExtensionsString = s;
                cd.OtherExtensionsArray = s.Split(';');
            }
            return cd;
        }

        public static ShowItemDocument getShowItemDocument(TheTVDB db)
        {
            ShowItemDocument si = new ShowItemDocument();

            si.TVDB = db;
            si.ManualFolderLocations = new System.Collections.Generic.Dictionary<int, StringList>();
            si.IgnoreSeasons = new System.Collections.Generic.List<int>();
            si.UseCustomShowName = false;
            si.CustomShowName = "";
            si.UseSequentialMatch = false;
            si.SeasonRules = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<ShowRule>>();
            si.SeasonEpisodes = new System.Collections.Generic.Dictionary<int, List<ProcessedEpisode>>();
            si.ShowNextAirdate = true;
            si.TVDBCode = -1;
            //                WhichSeasons = gcnew System.Collections.Generic.List<int>;
            //                NamingStyle = (int)NStyle.DefaultStyle();
            si.AutoAddNewSeasons = true;
            si.PadSeasonToTwoDigits = false;
            si.AutoAdd_FolderBase = "";
            si.AutoAdd_FolderPerSeason = true;
            si.AutoAdd_SeasonFolderName = "Season ";
            si.DoRename = true;
            si.DoMissingCheck = true;
            si.CountSpecials = false;
            si.DVDOrder = false;
            si.ForceCheckNoAirdate = false;
            si.ForceCheckFuture = false;

            return si;
        }
    }
}
