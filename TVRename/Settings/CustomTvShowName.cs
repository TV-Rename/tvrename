//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

public static class CustomTvShowName
{
    public static string DefaultStyle() => Presets[0];

    private static readonly List<string> Presets =
    [
        "{ShowName} ({Year})",
        "{ShowNameNoYear} ({Year})",
        "{ShowName}"
    ];

    internal static readonly List<string> TAGS =
    [
        "{ShowName}",
        "{ShowNamePost}",
        "{ShowNameInitial}",
        "{ShowNameLower}",
        "{ShowNameNoYear}",
        "{Year}",
        "{ContentRating}",
        "{Imdb}",
        "{TVDBId}",
        "{TMDBId}",
        "{TVMazeId}",
        "{Provider}",
        "{ProviderId}",
    ];

    public static List<string> ExamplePresets(ShowConfiguration s)
    {
        return [.. Presets.Select(example => NameFor(s, example))];
    }

    public static string NameFor(ShowConfiguration? m, string styleString) => NameFor(m, styleString, TargetType.Filename);

    public static string DirectoryNameFor(ShowConfiguration? m, string styleString)
    {
        return NameFor(m, styleString, TargetType.Foldername);
    }

    public static string NameFor(ShowConfiguration m, string styleString, string? extension)
    {
        string r = NameFor(m, styleString);

        if (string.IsNullOrEmpty(extension))
        {
            return r;
        }

        bool needsSpacer = !extension.StartsWith('.');

        if (needsSpacer)
        {
            return r + "." + extension;
        }

        return r + extension;
    }

    private static string NameFor(CachedSeriesInfo? showConfiguration, string styleString, TargetType type)
    {
        return NameFor(showConfiguration?.Name, showConfiguration, styleString, type);
    }

    public static string NameFor(ShowConfiguration? m, string styleString, TargetType type)
    {
        return NameFor(m?.ShowName, m?.CachedShow, styleString, type);
    }

    public static string NameFor(string? showName, CachedSeriesInfo? si, string styleString, TargetType type)
    {
        string name = styleString;

        if (showName is null)
        {
            return string.Empty;
        }

        string showname = showName;
        if (type==TargetType.URL)
        {
            showname = Uri.EscapeDataString(showname);
        }

        name = name.ReplaceInsensitive("{ShowName}", showname);
        name = name.ReplaceInsensitive("{ShowNamePost}", TVSettings.Instance.PostpendTerms(showname));
        name = name.ReplaceInsensitive("{ShowNameInitial}", showname.Initial().ToLower());
        name = name.ReplaceInsensitive("{ShowNameLower}", showName.ToLower().Replace(' ', '-').RemoveCharactersFrom("()[]{}&$:"));
        name = name.ReplaceYear(showName);
        name = name.ReplaceInsensitive("{ContentRating}", si?.ContentRating);
        name = name.ReplaceInsensitive("{Year}", si?.Year);
        name = name.ReplaceInsensitive("{Imdb}", si?.Imdb);
        name = name.ReplaceInsensitive("{TVDBId}", si?.TvdbId.ToString());
        name = name.ReplaceInsensitive("{TMDBId}", si?.TmdbId.ToString());
        name = name.ReplaceInsensitive("{TVMazeId}", si?.TvMazeId.ToString());
        name = name.ReplaceInsensitive("{Provider}", si?.ProviderName);
        name = name.ReplaceInsensitive("{ProviderId}", si?.ProviderCode?.ToString());

        if (type == TargetType.URL)
        {
            return name.Trim();
        }
        return (type == TargetType.Filename) ? TVSettings.Instance.FilenameFriendly(name.Trim()) : TVSettings.Instance.DirectoryFriendly(name.Trim());
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

    public static string DirectoryNameFor(CachedSeriesInfo showConfiguration, string styleString)
    {
        return NameFor(showConfiguration, styleString, TargetType.Foldername);
    } 
}
