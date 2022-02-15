using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    public static class CustomTvShowName
    {
        public static string DefaultStyle() => Presets[0];

        private static readonly List<string> Presets = new()
        {
            "{ShowName} ({Year})",
            "{ShowNameNoYear} ({Year})",
            "{ShowName}"
        };

        internal static readonly List<string> TAGS = new()
        {
            "{ShowName}",
            "{ShowNameInitial}",
            "{ShowNameLower}",
            "{ShowNameNoYear}",
            "{Year}",
            "{ContentRating}",
            "{Imdb}",
        };

        [NotNull]
        public static List<string> ExamplePresets(ShowConfiguration s)
        {
            return Presets.Select(example => NameFor(s, example)).ToList();
        }

        [NotNull]
        public static string NameFor(ShowConfiguration? m, string styleString) => NameFor(m, styleString, false, true);

        [NotNull]
        public static string DirectoryNameFor(ShowConfiguration? m, [NotNull] string styleString)
        {
            return NameFor(m, styleString, false, false);
        }

        [NotNull]
        public static string NameFor(ShowConfiguration m, string styleString, string? extension)
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
        private static string NameFor([CanBeNull] CachedSeriesInfo showConfiguration, string styleString, bool urlEncode, bool isfilename)
        {
            return NameFor(showConfiguration?.Name, showConfiguration, styleString, urlEncode, isfilename);
        }

        public static string NameFor(ShowConfiguration? m, string styleString, bool urlEncode, bool isfilename)
        {
            return NameFor(m?.ShowName, m?.CachedShow, styleString, urlEncode, isfilename);
        }

        public static string NameFor(string? showName,CachedSeriesInfo si, string styleString, bool urlEncode, bool isfilename)
        {
            string name = styleString;

            if (showName is null)
            {
                return string.Empty;
            }

            string showname = showName;
            if (urlEncode)
            {
                showname = Uri.EscapeDataString(showname);
            }

            name = name.ReplaceInsensitive("{ShowName}", showname);
            name = name.ReplaceInsensitive("{ShowNameInitial}", showname.Initial().ToLower());
            name = name.ReplaceInsensitive("{ShowNameLower}", showname.ToLower().Replace(' ', '-').RemoveCharactersFrom("()[]{}&$:"));
            name = name.ReplaceYear(showName);
            name = name.ReplaceInsensitive("{ContentRating}", si?.ContentRating);
            name = name.ReplaceInsensitive("{Year}", si?.Year);
            name = name.ReplaceInsensitive("{Imdb}", si?.Imdb);

            if (urlEncode)
            {
                return name.Trim();
            }
            return isfilename ? TVSettings.Instance.FilenameFriendly(name.Trim()) : TVSettings.DirectoryFriendly(name.Trim());
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

        [NotNull]
        public static string DirectoryNameFor(CachedSeriesInfo showConfiguration, string styleString)
        {
            return NameFor(showConfiguration, styleString, false, false);
        }
    }
}
