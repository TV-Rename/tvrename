// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using TVRename.Shows;

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
}
