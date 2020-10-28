// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Linq;
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
                                                            "{StartYear}-{EndYear}",
                                                            "Season {SeasonNumber:2} - {SeasonName}"
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
            "{ShowImdb}",
            "{SeasonName}"
        };

        [NotNull]
        public static List<string> ExamplePresets(ProcessedSeason s)
        {
            return Presets.Select(example => NameFor(s, example)).ToList();
        }

        [NotNull]
        public static string NameFor(ProcessedSeason s, string styleString) => NameFor(s, styleString, false);

        [NotNull]
        private static string NameFor(ProcessedSeason? s, string styleString, bool urlEncode)
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

            string seasonName = s.Show.CachedShow?.Season(s.SeasonNumber)?.SeasonName;

            name = name.ReplaceInsensitive("{ShowName}", showname);
            name = name.ReplaceInsensitive("{ShowNameInitial}", showname.Initial().ToLower());
            name = name.ReplaceInsensitive("{ShowNameLower}", s.Show.ShowName.ToLower().Replace(' ', '-').RemoveCharactersFrom("()[]{}&$:"));
            name = name.ReplaceInsensitive("{Season}", s.SeasonNumber.ToString());
            name = name.ReplaceInsensitive("{Season:2}", s.SeasonNumber.ToString("00"));
            name = name.ReplaceInsensitive("{SeasonNumber}", s.SeasonIndex.ToString());
            name = name.ReplaceInsensitive("{SeasonNumber:2}", s.SeasonIndex.ToString("00"));
            name = name.ReplaceInsensitive("{SeasonName}", seasonName??string.Empty);
            name = name.ReplaceInsensitive("{StartYear}", s.MinYear().ToString());
            name = name.ReplaceInsensitive("{EndYear}", s.MaxYear().ToString());
            name = name.ReplaceInsensitive("{ShowImdb}", s.Show.CachedShow?.Imdb??string.Empty);

            return TVSettings.DirectoryFriendly(name.Trim());
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

    public static class CustomMovieName
    {
        public static string DefaultStyle() => Presets[0];

        private static readonly List<string> Presets = new List<string>
                                                        {
                                                            "{ShowName} ({Year})",
                                                            "{ShowName}"
                                                        };

        internal static readonly List<string> TAGS = new List<string>
        {
            "{ShowName}",
            "{ShowNameInitial}",
            "{ShowNameLower}",
            "{Year}",
            "{ContentRating}",
            "{Imdb}",
        };

        [NotNull]
        public static List<string> ExamplePresets(MovieConfiguration s)
        {
            return Presets.Select(example => NameFor(s, example)).ToList();
        }

        [NotNull]
        public static string NameFor(MovieConfiguration? m, string styleString) => NameFor(m, styleString, false,true);

        public static string NameFor(MovieConfiguration m, string styleString, string? extension)
        {
            string r = NameFor(m, styleString);

            if (string.IsNullOrEmpty(extension))
            {
                return r;
            }

            bool needsSpacer = !extension.StartsWith(".", StringComparison.Ordinal);

            if (needsSpacer)
            {
                return r + "." + extension;
            }

            return r + extension;
        }

        [NotNull]
        public static string NameFor(MovieConfiguration? m, string styleString, bool urlEncode, bool isfilename)
        {
            string name = styleString;

            if (m?.ShowName is null)
            {
                return string.Empty;
            }

            string showname = m.ShowName;
            if (urlEncode)
            {
                showname = Uri.EscapeDataString(showname);
            }

            name = name.ReplaceInsensitive("{ShowName}", showname);
            name = name.ReplaceInsensitive("{ShowNameInitial}", showname.Initial().ToLower());
            name = name.ReplaceInsensitive("{ShowNameLower}", showname.ToLower().Replace(' ', '-').RemoveCharactersFrom("()[]{}&$:"));
            name = name.ReplaceInsensitive("{ContentRating}", m.CachedMovie?.ContentRating) ;
            name = name.ReplaceInsensitive("{Year}", m.CachedMovie?.Year.ToString() );
            name = name.ReplaceInsensitive("{Imdb}", m.CachedMovie?.Imdb );

            return isfilename ? TVSettings.DirectoryFriendly(name.Trim()) : name.Trim();
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
