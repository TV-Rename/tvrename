// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;

// This builds the foldernames to create/find, for any given season

namespace TVRename
{
    public class CustomSeasonName
    {
        private readonly string styleString;

        public CustomSeasonName(string s)
        {
            styleString = s;
        }

        public CustomSeasonName()
        {
            styleString = DefaultStyle();
        }

        private static string DefaultStyle() => Presets[1];

        private static readonly List<string> Presets = new List<string>
                                                        {
                                                            "Season {Season:2}",
                                                            "Season {Season}",
                                                            "S{Season}",
                                                            "S{Season:2}",
                                                            "{ShowName} - Season {Season:2}",
                                                            "Season {SeasonNumber:2}",
                                                            "Season {SeasonNumber}",
                                                            "S{SeasonNumber}",
                                                            "S{SeasonNumber:2}",
                                                            "{ShowName} - Season {SeasonNumber:2}",
                                                            "{StartYear}-{EndYear}"
                                                        };

        protected internal static readonly List<string> TAGS = new List<string>
        {
            "{ShowName}",
            "{Season}",
            "{Season:2}",
            "{SeasonNumber}",
            "{SeasonNumber:2}",
            "{StartYear}",
            "{EndYear}"
        };

        public List<string> ExamplePresets(Season s)
        {
            List<string> possibleExamples = new List<string>();
            foreach (string example in Presets)
            {
                possibleExamples.Add(NameFor(s,example));
            }

            return possibleExamples;
        }

        public string NameFor(Season s) => NameFor(s, styleString);

        public static string NameFor(Season s, string styleString) => NameFor(s, styleString, false);

        private static string NameFor(Season s, string styleString, bool urlEncode)
        {
            string name = styleString;

            if (s == null) return string.Empty;

            string showname = s.TheSeries.Name;
            if (urlEncode)
            {
                showname = System.Web.HttpUtility.UrlEncode(showname);
            }

            name = name.ReplaceInsensitive("{ShowName}", showname);
            name = name.ReplaceInsensitive("{Season}", s.SeasonNumber.ToString());
            name = name.ReplaceInsensitive("{Season:2}", s.SeasonNumber.ToString("00"));
            name = name.ReplaceInsensitive("{SeasonNumber}", s.SeasonIndex.ToString());
            name = name.ReplaceInsensitive("{SeasonNumber:2}", s.SeasonIndex.ToString("00"));
            name = name.ReplaceInsensitive("{StartYear}", s.MinYear().ToString());
            name = name.ReplaceInsensitive("{EndYear}", s.MaxYear().ToString());

            return name.Trim();
        }

        public static string GetTextFromPattern(string styleString)
        {
            string name = styleString;

            foreach (string tag in TAGS)
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
