using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename;

public static class CustomMovieName
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
        "{CollectionName}",
        "{MovieType}",
        "{CollectionFolder}",
        "{CollectionOrder}",
    };

    public static List<string> ExamplePresets(MovieConfiguration s)
    {
        return Presets.Select(example => NameFor(s, example)).ToList();
    }

    public static string NameFor(MovieConfiguration? m, string styleString) => NameFor(m, styleString, false, true);

    public static string DirectoryNameFor(MovieConfiguration? m, string styleString)
    {
        if (styleString.StartsWith("{collectionFolder}", StringComparison.OrdinalIgnoreCase))
        {
            if (m?.InCollection ?? false)
            {
                return m.CachedMovie?.CollectionName + "\\" + NameFor(m, styleString.RemoveFirst("{collectionFolder}/".Length), false, false);
            }
            return NameFor(m, styleString.RemoveFirst("{collectionFolder}/".Length), false, false);
        }
        return NameFor(m, styleString, false, false);
    }

    public static string NameFor(MovieConfiguration m, string styleString, string? extension)
    {
        string r = NameFor(m, styleString);

        if (string.IsNullOrEmpty(extension))
        {
            return r;
        }

        bool needsSpacer = !extension!.StartsWith(".", StringComparison.Ordinal);

        if (needsSpacer)
        {
            return r + "." + extension;
        }

        return r + extension;
    }

    public static string NameFor(MovieConfiguration? m, string styleString, bool urlEncode, bool isfilename)
    {
        if (m?.ShowName is null)
        {
            return string.Empty;
        }

        string showname = urlEncode
            ? Uri.EscapeDataString(m.ShowName)
            : m.ShowName;
                
        string name = styleString
            .ReplaceInsensitive("{CollectionFolder}", m.InCollection ? "{collectionName}\\" : string.Empty)
            .ReplaceInsensitive("{ShowName}", showname)
            .ReplaceInsensitive("{ShowNameInitial}", showname.Initial().ToLower())
            .ReplaceInsensitive("{ShowNameLower}", showname.ToLower().Replace(' ', '-').RemoveCharactersFrom("()[]{}&$:"))
            .ReplaceYear(m)
            .ReplaceInsensitive("{ContentRating}", m.CachedMovie?.ContentRating)
            .ReplaceInsensitive("{Year}", m.CachedMovie?.Year.ToString())
            .ReplaceInsensitive("{Imdb}", m.CachedMovie?.Imdb)
            .ReplaceInsensitive("{CollectionName}", m.CachedMovie?.CollectionName)
            .ReplaceInsensitive("{CollectionOrder}", m.CollectionOrder?.ToString())
            .ReplaceInsensitive("{MovieType}", m.CachedMovie?.MovieType);

        return urlEncode ? name.Trim()
            : isfilename ? TVSettings.Instance.FilenameFriendly(name.Trim())
            : TVSettings.DirectoryFriendly(name.Trim());
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

    public static string NameFor(MovieConfiguration m, string styleString, int year)
    {
        string styleStringNewYear = styleString.ReplaceInsensitive("{Year}", year.ToString());

        return NameFor(m, styleStringNewYear);
    }
}
