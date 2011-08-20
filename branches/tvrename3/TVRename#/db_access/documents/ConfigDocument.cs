using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace TVRename.db_access.documents
{
    public class ConfigDocument
    {
        public string Id { get; set; }

        #region FolderJpgIsType enum

        public enum FolderJpgIsType
        {
            Banner,
            Poster,
            FanArt
        }

        #endregion

        public bool AutoSelectShowInMyShows { get; set; }
        public bool BGDownload { get; set; }
        public bool CheckuTorrent { get; set; }
        public bool EpImgs { get; set; }
        public bool ExportFOXML { get; set; }
        public string ExportFOXMLTo { get; set; }
        public bool ExportMissingCSV { get; set; }
        public string ExportMissingCSVTo { get; set; }
        public bool ExportMissingXML { get; set; }
        public string ExportMissingXMLTo { get; set; }
        public int ExportRSSMaxDays { get; set; }
        public int ExportRSSMaxShows { get; set; }
        public bool ExportRenamingXML { get; set; }
        public string ExportRenamingXMLTo { get; set; }
        public bool ExportWTWRSS { get; set; }
        public string ExportWTWRSSTo { get; set; }
        public FNPRegexList FNPRegexs { get; set; }
        public bool FolderJpg { get; set; }
        public FolderJpgIsType FolderJpgIs { get; set; }
        public bool ForceLowercaseFilenames { get; set; }
        public bool IgnoreSamples { get; set; }
        public bool KeepTogether { get; set; }
        public bool LeadingZeroOnSeason { get; set; }
        public bool LeaveOriginals { get; set; }
        public bool LookForDateInFilename { get; set; }
        public bool MissingCheck { get; set; }
        public bool NFOs { get; set; }
        public CustomName NamingStyle { get; set; }
        public bool NotificationAreaIcon { get; set; }
        public bool OfflineMode { get; set; } // TODO: Make property of thetvdb?
        public string[] OtherExtensionsArray { get; set; }
        public string OtherExtensionsString { get; set; }
        public int ParallelDownloads { get; set; }
        public StringList RSSURLs { get; set; }
        public bool RenameCheck { get; set; }
        public bool RenameTxtToSub { get; set; }
        public ReplacementList Replacements { get; set; }
        public string ResumeDatPath { get; set; }
        public int SampleFileMaxSizeMB { get; set; } // sample file must be smaller than this to be ignored
        public bool SearchLocally { get; set; }
        public bool SearchRSS { get; set; }
        public bool ShowEpisodePictures { get; set; }
        public bool ShowInTaskbar { get; set; }
        public string SpecialsFolderName { get; set; }
        public int StartupTab { get; set; }
        public Searchers TheSearchers { get; set; }
        public string[] VideoExtensionsArray { get; set; }
        public string VideoExtensionsString { get; set; }
        public int WTWRecentDays { get; set; }
        public string uTorrentPath { get; set; }
        public bool MonitorFolders { get; set; }
        public ShowStatusColoringTypeList ShowStatusColors { get; set; }
        public StringList MonitorFoldersList { get; set; }
        public StringList IgnoreFoldersList { get; set; }
        public StringList SearchFoldersList { get; set; }
        public List<IgnoreItem> Ignore { get; set; }

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

        public static string CompulsoryReplacements()
        {
            return "*?<>:/\\|\""; // invalid filename characters, must be in the list!
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
    }
}
