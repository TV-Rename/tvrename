// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
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
            return Presets()[1];
        }

        public static string OldNStyle(int n)
        {
            // enum class Style {Name_xxx_EpName = 0, Name_SxxEyy_EpName, xxx_EpName, SxxEyy_EpName, Eyy_EpName, 
            // Exx_Show_Sxx_EpName, yy_EpName, NameSxxEyy_EpName, xXxx_EpName };

            // for now, this maps onto the presets
            if ((n >= 0) && (n < 9))
                return Presets()[n];
            else
                return DefaultStyle();
        }

        public static StringList Presets()
        {
            StringList res = new StringList();

            // if _any_ of these are changed, you'll need to change the OldNStyle function, too.
            res.Add("{ShowName} - {Season}x{Episode}[-{Season}x{Episode2}] - {EpisodeName}");
            res.Add("{ShowName} - S{Season:2}E{Episode}[-E{Episode2}] - {EpisodeName}");
            res.Add("{ShowName} S{Season:2}E{Episode}[-E{Episode2}] - {EpisodeName}");
            res.Add("{Season}{Episode}[-{Season}{Episode2}] - {EpisodeName}");
            res.Add("{Season}x{Episode}[-{Season}x{Episode2}] - {EpisodeName}");
            res.Add("S{Season:2}E{Episode}[-E{Episode2}] - {EpisodeName}");
            res.Add("E{Episode}[-E{Episode2}] - {EpisodeName}");
            res.Add("{Episode}[-{Episode2}] - {ShowName} - 3 - {EpisodeName}");
            res.Add("{Episode}[-{Episode2}] - {EpisodeName}");

            return res;
        }

        public string NameForExt(ProcessedEpisode pe, string extension)
        {
            string r = NameForNoExt(pe, this.StyleString);
            if (!string.IsNullOrEmpty(extension))
            {
                if (!extension.StartsWith("."))
                    r += ".";
                r += extension;
            }
            return r;
        }

        public static StringList Tags()
        {
            StringList res = new StringList();

            res.Add("{ShowName}");
            res.Add("{Season}");
            res.Add("{Season:2}");
            res.Add("{Episode}");
            res.Add("{Episode2}");
            res.Add("{EpisodeName}");
            res.Add("{Number}");
            res.Add("{Number:2}");
            res.Add("{Number:3}");
            res.Add("{ShortDate}");
            res.Add("{LongDate}");
            res.Add("{YMDDate}");

            return res;
        }

        public static string NameForNoExt(ProcessedEpisode pe, string styleString)
        {
            return NameForNoExt(pe, styleString, false);
        }

        public static string NameForNoExt(ProcessedEpisode pe, String styleString, bool urlEncode)
        {
            String name = styleString; // TODO: make copy instaed?

            string showname = pe.SI.ShowName();
            string epname = pe.Name;
            if (urlEncode)
            {
                showname = System.Web.HttpUtility.UrlEncode(showname);
                epname = System.Web.HttpUtility.UrlEncode(epname);
            }

            name = name.Replace("{ShowName}", showname);
            name = name.Replace("{Season}", pe.SeasonNumber.ToString());
            name = name.Replace("{Season:2}", pe.SeasonNumber.ToString("00"));
            name = name.Replace("{Episode}", pe.EpNum.ToString("00"));
            name = name.Replace("{Episode2}", pe.EpNum2.ToString("00"));
            name = name.Replace("{EpisodeName}", epname);
            name = name.Replace("{Number}", pe.OverallNumber.ToString());
            name = name.Replace("{Number:2}", pe.OverallNumber.ToString("00"));
            name = name.Replace("{Number:3}", pe.OverallNumber.ToString("000"));
            DateTime? airdt = pe.GetAirDateDT(false);
            if (airdt != null)
            {
                DateTime dt = (DateTime) airdt;
                name = name.Replace("{ShortDate}", dt.ToString("d"));
                name = name.Replace("{LongDate}", dt.ToString("D"));
                string ymd = dt.ToString("yyyy/MM/dd");
                if (urlEncode)
                    ymd = System.Web.HttpUtility.UrlEncode(ymd);
                name = name.Replace("{YMDDate}", ymd);
            }
            else
            {
                name = name.Replace("{ShortDate}", "---");
                name = name.Replace("{LongDate}", "------");
                string ymd = "----/--/--";
                if (urlEncode)
                    ymd = System.Web.HttpUtility.UrlEncode(ymd);
                name = name.Replace("{YMDDate}", ymd);
            }

            if (pe.EpNum2 == pe.EpNum)
                name = Regex.Replace(name, "([^\\\\])\\[.*?[^\\\\]\\]", "$1"); // remove optional parts
            else
                name = Regex.Replace(name, "([^\\\\])\\[(.*?[^\\\\])\\]", "$1$2"); // remove just the brackets

            name = name.Replace("\\[", "[");
            name = name.Replace("\\]", "]");

            return name;
        }
    }
}
