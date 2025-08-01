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

// This builds the foldernames to create/find, for any given season

namespace TVRename;

public static class CustomSeasonName
{
    public static string DefaultStyle() => Presets[0];

    private static readonly List<string> Presets =
    [
        "Season {Season:2}",
        "Season {Season}",
        "S{Season}",
        "S{Season:2}",
        "{ShowName} - Season {Season:2}",
        "{ShowNameNoYear} ({Year}) - Season {Season:2}",
        "Season {SeasonNumber:2}",
        "Season {SeasonNumber}",
        "S{SeasonNumber}",
        "S{SeasonNumber:2}",
        "{ShowName} - Season {SeasonNumber:2}",
        "{StartYear}-{EndYear}",
        "Season {SeasonNumber:2} - {SeasonName}"
    ];

    internal static readonly List<string> TAGS =
    [
        "{ShowName}",
        "{ShowNameInitial}",
        "{ShowNameLower}",
        "{ShowNameNoYear}",
        "{Season}",
        "{Season:2}",
        "{SeasonNumber}",
        "{SeasonNumber:2}",
        "{StartYear}",
        "{EndYear}",
        "{ShowImdb}",
        "{SeasonName}",
        "{TotalNumberOfEpisodes}"
    ];

    public static List<string> ExamplePresets(ProcessedSeason s)
    {
        return [.. Presets.Select(example => NameFor(s, example))];
    }

    public static string NameFor(ProcessedSeason s, string styleString) => NameFor(s, styleString, false);
    public static string NameFor(ShowConfiguration si, int snum, string styleString)
    {
        ProcessedSeason s = si.AppropriateSeasons()[snum];
        return NameFor(s, styleString, false);
    }

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

        string? seasonName = s.Show.CachedShow?.Season(s.SeasonNumber)?.SeasonName;

        name = name.ReplaceInsensitive("{ShowName}", showname);
        name = name.ReplaceInsensitive("{ShowNameInitial}", showname.Initial().ToLower());
        name = name.ReplaceInsensitive("{ShowNameLower}", s.Show.ShowName.ToLower().Replace(' ', '-').RemoveCharactersFrom("()[]{}&$:"));
        name = name.ReplaceYear(s.Show);
        name = name.ReplaceInsensitive("{Season}", s.SeasonNumber.ToString());
        name = name.ReplaceInsensitive("{Season:2}", s.SeasonNumber.ToString("00"));
        name = name.ReplaceInsensitive("{SeasonNumber}", s.SeasonIndex.ToString());
        name = name.ReplaceInsensitive("{SeasonNumber:2}", s.SeasonIndex.ToString("00"));
        name = name.ReplaceInsensitive("{SeasonName}", seasonName ?? string.Empty);
        name = name.ReplaceInsensitive("{StartYear}", s.MinYear().ToString());
        name = name.ReplaceInsensitive("{EndYear}", s.MaxYear().ToString());
        name = name.ReplaceInsensitive("{ShowImdb}", s.Show.CachedShow?.Imdb ?? string.Empty);
        name = name.ReplaceInsensitive("{TotalNumberOfEpisodes}", s.Episodes.Count.ToString());

        return TVSettings.DirectoryFriendly(name.Trim());
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
