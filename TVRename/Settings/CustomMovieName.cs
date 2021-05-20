using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename
{
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
        public static string NameFor(MovieConfiguration? m, string styleString) => NameFor(m, styleString, false, true);

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
            name = name.ReplaceInsensitive("{ContentRating}", m.CachedMovie?.ContentRating);
            name = name.ReplaceInsensitive("{Year}", m.CachedMovie?.Year.ToString());
            name = name.ReplaceInsensitive("{Imdb}", m.CachedMovie?.Imdb);

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

        public static string NameFor(MovieConfiguration m, string styleString, int year)
        {
            string styleStringNewYear = styleString.ReplaceInsensitive("{Year}", year.ToString());

            return NameFor(m, styleStringNewYear);
        }
    }
}
