// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;

// This builds the foldernames to create/find, for any given season

namespace TVRename
{
    public class CustomSeasonName
    {
        public string StyleString;

        public CustomSeasonName(CustomSeasonName o)
        {
            StyleString = o.StyleString;
        }

        public CustomSeasonName(string s)
        {
            StyleString = s;
        }

        public CustomSeasonName()
        {
            StyleString = DefaultStyle();
        }

        public static string DefaultStyle() => Presets[1];

        protected internal static readonly List<string> Presets = new List<string>
                                                        {
                                                            "Season {Season:2}",
                                                            "Season {Season}",
                                                            "S{Season}",
                                                            "S{Season:2}",
                                                            "{ShowName} - Season {Season:2}",
                                                            "{StartYear}-{EndYear}"
                                                        };

        protected internal static readonly List<string> Tags = new List<string>
        {
            "{ShowName}",
            "{Season}",
            "{Season:2}",
            "{StartYear}",
            "{EndYear}"
        };

        public string NameFor(Season s) => NameFor(s, StyleString);

        public static string NameFor(Season s, string styleString) => NameFor(s, styleString, false);

        public static string NameFor(Season s, string styleString, bool urlEncode)
        {
            string name = styleString;

            string showname = s.TheSeries.Name;
            if (urlEncode)
            {
                showname = System.Web.HttpUtility.UrlEncode(showname);
            }

            name = name.ReplaceInsensitive("{ShowName}", showname);
            name = name.ReplaceInsensitive("{Season}", s.SeasonNumber.ToString());
            name = name.ReplaceInsensitive("{Season:2}", s.SeasonNumber.ToString("00"));

            return name.Trim();
        }

        public static string GetTextFromPattern(string styleString)
        {
            string name = styleString;

            foreach (string tag in Tags)
            {
                name = name.ReplaceInsensitive(tag, string.Empty);
            }
            name = name.ReplaceInsensitive("-", string.Empty);
            name = name.ReplaceInsensitive("/", string.Empty);
            name = name.ReplaceInsensitive("\\", string.Empty);
            return name.Trim();
        }
    }
}
