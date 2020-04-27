// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

// This builds the foldernames to create/find, for any given season

namespace TVRename
{
    public static class CustomSeasonName
    {
        public static string DefaultStyle() => Presets[0];

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

        internal static readonly List<string> TAGS = new List<string>
        {
            "{ShowName}",
            "{ShowNameInitial}",
            "{ShowNameLower}",
            "{Season}",
            "{Season:2}",
            "{SeasonNumber}",
            "{SeasonNumber:2}",
            "{StartYear}",
            "{EndYear}",
            "{ShowImdb}"
        };

        [NotNull]
        public static List<string> ExamplePresets(ProcessedSeason s)
        {
            List<string> possibleExamples = new List<string>();
            foreach (string example in Presets)
            {
                possibleExamples.Add(NameFor(s,example));
            }

            return possibleExamples;
        }

        [NotNull]
        public static string NameFor(ProcessedSeason s, string styleString) => NameFor(s, styleString, false);

        [NotNull]
        private static string NameFor([CanBeNull] ProcessedSeason s, string styleString, bool urlEncode)
        {
            string name = styleString;

            if (s is null)
            {
                return string.Empty;
            }

            string showname = s.Show.ShowName;
            if (urlEncode)
            {
                showname = Uri.EscapeDataString(showname);
            }

            name = name.ReplaceInsensitive("{ShowName}", showname);
            name = name.ReplaceInsensitive("{ShowNameInitial}", showname.Initial().ToLower());
            name = name.ReplaceInsensitive("{ShowNameLower}", s.Show.ShowName.ToLower().Replace(' ', '-').RemoveCharactersFrom("()[]{}&$:"));
            name = name.ReplaceInsensitive("{Season}", s.SeasonNumber.ToString());
            name = name.ReplaceInsensitive("{Season:2}", s.SeasonNumber.ToString("00"));
            name = name.ReplaceInsensitive("{SeasonNumber}", s.SeasonIndex.ToString());
            name = name.ReplaceInsensitive("{SeasonNumber:2}", s.SeasonIndex.ToString("00"));
            name = name.ReplaceInsensitive("{StartYear}", s.MinYear().ToString());
            name = name.ReplaceInsensitive("{EndYear}", s.MaxYear().ToString());
            name = name.ReplaceInsensitive("{ShowImdb}", s.Show.TheSeries()?.Imdb??string.Empty);

            return TVSettings.Instance.DirectoryFriendly(name.Trim());
        }

        [NotNull]
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
