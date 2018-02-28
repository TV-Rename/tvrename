// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// This builds the filenames to rename to, for any given episode (or multi-episode episode)

namespace TVRename
{
    public class CustomName
    {
        public string StyleString;

        public CustomName(CustomName O)
        {
            this.StyleString = O.StyleString;
        }

        public CustomName(string s)
        {
            this.StyleString = s;
        }

        public CustomName()
        {
            this.StyleString = DefaultStyle();
        }

        public static string DefaultStyle()
        {
            return Presets[1];
        }

        public static string OldNStyle(int n)
        {
            // enum class Style {Name_xxx_EpName = 0, Name_SxxEyy_EpName, xxx_EpName, SxxEyy_EpName, Eyy_EpName, 
            // Exx_Show_Sxx_EpName, yy_EpName, NameSxxEyy_EpName, xXxx_EpName };

            // for now, this maps onto the presets
            if ((n >= 0) && (n < 9))
                return Presets[n];

            return DefaultStyle();
        }

        public static readonly List<string> Presets = new List<String>
                                                        {
                                                            "{ShowName} - {Season}x{Episode}[-{Season}x{Episode2}] - {EpisodeName}",
                                                            "{ShowName} - S{Season:2}E{Episode}[-E{Episode2}] - {EpisodeName}",
                                                            "{ShowName} S{Season:2}E{Episode}[-E{Episode2}] - {EpisodeName}",
                                                            "{Season}{Episode}[-{Season}{Episode2}] - {EpisodeName}",
                                                            "{Season}x{Episode}[-{Season}x{Episode2}] - {EpisodeName}",
                                                            "S{Season:2}E{Episode}[-E{Episode2}] - {EpisodeName}",
                                                            "E{Episode}[-E{Episode2}] - {EpisodeName}",
                                                            "{Episode}[-{Episode2}] - {ShowName} - 3 - {EpisodeName}",
                                                            "{Episode}[-{Episode2}] - {EpisodeName}",
                                                            "{ShowName} - S{Season:2}{AllEpisodes} - {EpisodeName}"
                                                        };

        public string NameForExt(ProcessedEpisode pe, string extension = "", int folderNameLength=0)
        {
            // set folderNameLength to have the filename truncated if the total path length is too long

            string r = NameForNoExt(pe, this.StyleString);

            int maxLenOK = 200 - (folderNameLength + (extension?.Length ?? 0));
            if (r.Length > maxLenOK)
                r = r.Substring(0, maxLenOK);

            if (!string.IsNullOrEmpty(extension))
            {
                if (!extension.StartsWith("."))
                    r += ".";
                r += extension;
            }
            return r;
        }

        public string GetTargetEpisodeName(Episode ep, string showname,TimeZone tz, bool urlEncode = false )
        {
            //note this is for an Episode and not a ProcessedEpisode
            String name = this.StyleString;
            
            string epname = ep.Name;

            name = name.ReplaceInsensitive("{ShowName}", showname);
            name = name.ReplaceInsensitive("{Season}", ep.SeasonNumber.ToString());
            name = name.ReplaceInsensitive("{Season:2}", ep.SeasonNumber.ToString("00"));
            name = name.ReplaceInsensitive("{Episode}", ep.EpNum.ToString("00"));
            name = name.ReplaceInsensitive("{Episode2}", ep.EpNum.ToString("00"));
            name = name.ReplaceInsensitive("{EpisodeName}", epname);
            name = name.ReplaceInsensitive("{Number}", "");
            name = name.ReplaceInsensitive("{Number:2}", "");
            name = name.ReplaceInsensitive("{Number:3}", "");
            DateTime? airdt = ep.GetAirDateDT(tz);
            if (airdt != null)
            {
                DateTime dt = (DateTime)airdt;
                name = name.ReplaceInsensitive("{ShortDate}", dt.ToString("d"));
                name = name.ReplaceInsensitive("{LongDate}", dt.ToString("D"));
                string ymd = dt.ToString("yyyy/MM/dd");
                if (urlEncode)
                    ymd = System.Web.HttpUtility.UrlEncode(ymd);
                name = name.ReplaceInsensitive("{YMDDate}", ymd);
            }
            else
            {
                name = name.ReplaceInsensitive("{ShortDate}", "---");
                name = name.ReplaceInsensitive("{LongDate}", "------");
                string ymd = "----/--/--";
                if (urlEncode)
                    ymd = System.Web.HttpUtility.UrlEncode(ymd);
                name = name.ReplaceInsensitive("{YMDDate}", ymd);
            }

            name = Regex.Replace(name, "{AllEpisodes}", ep.EpNum.ToString("00"));

            name = Regex.Replace(name, "([^\\\\])\\[.*?[^\\\\]\\]", "$1"); // remove optional parts

            name = name.Replace("\\[", "[");
            name = name.Replace("\\]", "]");

            return name.Trim();
        }
    

        public static readonly List<string> Tags = new List<String>
                       {
                           "{ShowName}",
                           "{Season}",
                           "{Season:2}",
                           "{Episode}",
                           "{Episode2}",
                           "{EpisodeName}",
                           "{Number}",
                           "{Number:2}",
                           "{Number:3}",
                           "{ShortDate}",
                           "{LongDate}",
                           "{YMDDate}",
                           "{AllEpisodes}"
                       };
        

        public static string NameForNoExt(ProcessedEpisode pe, string styleString)
        {
            return NameForNoExt(pe, styleString, false);
        }

        public static string NameForNoExt(ProcessedEpisode pe, String styleString, bool urlEncode)
        {
            String name = styleString;

            string showname = pe.SI.ShowName;
            string epname = pe.Name;
            if (urlEncode)
            {
                showname = System.Web.HttpUtility.UrlEncode(showname);
                epname = System.Web.HttpUtility.UrlEncode(epname);
            }

            name = name.ReplaceInsensitive("{ShowName}", showname);
            name = name.ReplaceInsensitive("{Season}", pe.SeasonNumber.ToString());
            name = name.ReplaceInsensitive("{Season:2}", pe.SeasonNumber.ToString("00"));
            name = name.ReplaceInsensitive("{Episode}", pe.EpNum.ToString("00"));
            name = name.ReplaceInsensitive("{Episode2}", pe.EpNum2.ToString("00"));
            name = name.ReplaceInsensitive("{EpisodeName}", epname);
            name = name.ReplaceInsensitive("{Number}", pe.OverallNumber.ToString());
            name = name.ReplaceInsensitive("{Number:2}", pe.OverallNumber.ToString("00"));
            name = name.ReplaceInsensitive("{Number:3}", pe.OverallNumber.ToString("000"));
            DateTime? airdt = pe.GetAirDateDT(false);
            if (airdt != null)
            {
                DateTime dt = (DateTime) airdt;
                name = name.ReplaceInsensitive("{ShortDate}", dt.ToString("d"));
                name = name.ReplaceInsensitive("{LongDate}", dt.ToString("D"));
                string ymd = dt.ToString("yyyy/MM/dd");
                if (urlEncode)
                    ymd = System.Web.HttpUtility.UrlEncode(ymd);
                name = name.ReplaceInsensitive("{YMDDate}", ymd);
            }
            else
            {
                name = name.ReplaceInsensitive("{ShortDate}", "---");
                name = name.ReplaceInsensitive("{LongDate}", "------");
                string ymd = "----/--/--";
                if (urlEncode)
                    ymd = System.Web.HttpUtility.UrlEncode(ymd);
                name = name.ReplaceInsensitive("{YMDDate}", ymd);
            }

            String allEps = "";
            for (int i = pe.EpNum; i <= pe.EpNum2; i++)
                allEps += "E" + i.ToString("00");
            name = Regex.Replace(name, "{AllEpisodes}", allEps,RegexOptions.IgnoreCase);

            if (pe.EpNum2 == pe.EpNum)
                name = Regex.Replace(name, "([^\\\\])\\[.*?[^\\\\]\\]", "$1"); // remove optional parts
            else
                name = Regex.Replace(name, "([^\\\\])\\[(.*?[^\\\\])\\]", "$1$2"); // remove just the brackets

            name = name.Replace("\\[", "[");
            name = name.Replace("\\]", "]");

            return name.Trim();
        }
    }
}
