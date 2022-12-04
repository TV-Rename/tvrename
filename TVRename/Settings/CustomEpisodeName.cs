//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using NLog;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// This builds the filenames to rename to, for any given episode (or multi-episode episode)

namespace TVRename;

public class CustomEpisodeName
{
    public string StyleString;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public CustomEpisodeName(string s)
    {
        StyleString = s;
    }

    public CustomEpisodeName()
    {
        StyleString = DefaultStyle();
    }

    private static string DefaultStyle() => PRESETS[1];

    public static string OldNStyle(int n)
    {
        // for now, this maps onto the presets
        if (n is >= 0 and < 9)
        {
            return PRESETS[n];
        }

        return DefaultStyle();
    }

    protected internal static readonly List<string> PRESETS = new()
    {
        "{ShowName} - {Season}x{Episode}[-{Season}x{Episode2}] - {EpisodeName}",
        "{ShowName} - S{Season:2}E{Episode}[-E{Episode2}] - {EpisodeName}",
        "{ShowName} S{Season:2}E{Episode}[-E{Episode2}] - {EpisodeName}",
        "{ShowNameNoYear} ({Year}) S{Season:2}E{Episode}[-E{Episode2}] - {EpisodeName}",
        "{Season}{Episode}[-{Season}{Episode2}] - {EpisodeName}",
        "{Season}x{Episode}[-{Season}x{Episode2}] - {EpisodeName}",
        "S{Season:2}E{Episode}[-E{Episode2}] - {EpisodeName}",
        "E{Episode}[-E{Episode2}] - {EpisodeName}",
        "{Episode}[-{Episode2}] - {ShowName} - 3 - {EpisodeName}",
        "{Episode}[-{Episode2}] - {EpisodeName}",
        "{ShowName} - S{Season:2}{AllEpisodes} - {EpisodeName}"
    };

    protected internal static readonly List<string> TAGS = new()
    {
        "{ShowName}",
        "{ShowNameInitial}",
        "{ShowNameLower}",
        "{ShowNameNoYear}",
        "{Season}",
        "{Season:2}",
        "{SeasonNumber}",
        "{SeasonNumber:2}",
        "{SeasonName}",
        "{Episode}",
        "{Episode2}",
        "{EpisodeName}",
        "{Number}",
        "{Number:2}",
        "{Number:3}",
        "{ShortDate}",
        "{LongDate}",
        "{YMDDate}",
        "{AllEpisodes}",
        "{Year}",
        "{SeasonYear}",
        "{Imdb}",
        "{ShowImdb}"
    };

    public string NameFor(ProcessedEpisode pe) => NameFor(pe, string.Empty, 0);

    public string NameFor(ProcessedEpisode pe, string? extension, int folderNameLength)
    {
        const int MAX_LENGTH = 260;
        int maxFilenameLength = MAX_LENGTH - 1 - folderNameLength - (extension?.Length ?? 5); //Assume a max 5 character extension

        if (maxFilenameLength <= 12)//assume we need space for a 8.3 length filename at least
        {
            throw new System.IO.PathTooLongException(
                $"Cannot create files as path is too long - please review settings for {pe.Show.ShowName}");
        }

        string r = NameForNoExt(pe, StyleString);

        if (string.IsNullOrEmpty(extension))
        {
            return r.Substring(0, Math.Min(maxFilenameLength, r.Length));
        }

        return r.Substring(0, Math.Min(r.Length, maxFilenameLength)) + extension;
    }

    public string GetTargetEpisodeName(ShowConfiguration show, Episode ep)
        => GetTargetEpisodeName(show, ep, false);

    private string GetTargetEpisodeName(ShowConfiguration show, Episode ep, bool urlEncode)
    {
        //note this is for an Episode and not a ProcessedEpisode
        string name = StyleString;

        string epname = ep.Name;

        name = name.ReplaceInsensitive("{ShowName}", show.ShowName);
        name = name.ReplaceInsensitive("{ShowNameLower}", show.ShowName.ToLower().Replace(' ', '-').RemoveCharactersFrom("()[]{}&$:"));
        name = name.ReplaceInsensitive("{ShowNameInitial}", show.ShowName.Initial().ToLower());
        name = name.ReplaceYear(show);

        int seasonNumber;
        int episodeNumber;
        switch (show.Order)
        {
            case ProcessedSeason.SeasonType.dvd:
                seasonNumber = ep.DvdSeasonNumber;
                episodeNumber = ep.DvdEpNum;
                break;

            case ProcessedSeason.SeasonType.aired:
                seasonNumber = ep.AiredSeasonNumber;
                episodeNumber = ep.AiredEpNum;
                break;

            case ProcessedSeason.SeasonType.alternate:
                seasonNumber = ep.AiredSeasonNumber;
                episodeNumber = ep.AiredEpNum;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
        string? seasonName = show.CachedShow?.Season(seasonNumber)?.SeasonName;

        name = name.ReplaceInsensitive("{Season}", seasonNumber.ToString());
        name = name.ReplaceInsensitive("{Season:2}", seasonNumber.ToString("00"));
        name = name.ReplaceInsensitiveLazy("{SeasonNumber}", new Lazy<string?>(() => show.GetSeasonIndex(seasonNumber).ToString()), StringComparison.CurrentCultureIgnoreCase);
        name = name.ReplaceInsensitiveLazy("{SeasonNumber:2}", new Lazy<string?>(() => show.GetSeasonIndex(seasonNumber).ToString("00")), StringComparison.CurrentCultureIgnoreCase);
        name = name.ReplaceInsensitive("{Episode}", episodeNumber.ToString("00"));
        name = name.ReplaceInsensitive("{Episode2}", episodeNumber.ToString("00"));
        name = Regex.Replace(name, "{AllEpisodes}", episodeNumber.ToString("00"));
        name = name.ReplaceInsensitive("{SeasonName}", seasonName ?? string.Empty);
        name = name.ReplaceInsensitive("{EpisodeName}", epname.Trim());
        name = name.ReplaceInsensitive("{Number}", "");
        name = name.ReplaceInsensitive("{Number:2}", "");
        name = name.ReplaceInsensitive("{Number:3}", "");
        name = name.ReplaceInsensitive("{Imdb}", ep.ImdbCode ?? string.Empty);

        CachedSeriesInfo? si = show.CachedShow;
        name = name.ReplaceInsensitive("{ShowImdb}", si?.Imdb ?? string.Empty);
        name = name.ReplaceInsensitiveLazy("{Year}", new Lazy<string?>(() => si?.MinYear.ToString() ?? string.Empty), StringComparison.CurrentCultureIgnoreCase);

        ProcessedSeason? selectedProcessedSeason = show.GetSeason(ep.GetSeasonNumber(show.Order));
        name = name.ReplaceInsensitive("{SeasonYear}", selectedProcessedSeason != null ? selectedProcessedSeason.MinYear().ToString() : string.Empty);

        name = ReplaceDates(urlEncode, name, ep.GetAirDateDt(show.GetTimeZone()));

        name = Regex.Replace(name, "([^\\\\])\\[.*?[^\\\\]\\]", "$1"); // remove optional parts

        name = name.Replace("\\[", "[");
        name = name.Replace("\\]", "]");

        return name.Trim();
    }

    private static string ReplaceDates(bool urlEncode, string name, DateTime? airdt)
    {
        if (!name.Contains("{ShortDate}") && !name.Contains("{LongDate}") && !name.Contains("{YMDDate}"))
        {
            //No date to replace
            return name;
        }

        try
        {
            string ymd;

            if (airdt != null)
            {
                DateTime dt = (DateTime)airdt;
                name = name.ReplaceInsensitive("{ShortDate}", dt.ToString("d"));
                name = name.ReplaceInsensitive("{LongDate}", dt.ToString("D"));
                ymd = dt.ToString("yyyy/MM/dd");
            }
            else
            {
                name = name.ReplaceInsensitive("{ShortDate}", "---");
                name = name.ReplaceInsensitive("{LongDate}", "------");
                ymd = "----/--/--";
            }
            if (urlEncode)
            {
                ymd = Uri.EscapeDataString(ymd);
            }

            name = name.ReplaceInsensitive("{YMDDate}", ymd);

            return name;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Logger.Error($"Parsing date {airdt} into {name}: {ex.Message}");
            throw;
        }
    }

    public static string NameForNoExt(ProcessedEpisode pe, string styleString) => NameForNoExt(pe, styleString, false);

    public static string NameForNoExt(ProcessedEpisode pe, string styleString, bool urlEncode)
    {
        try
        {
            string name = pe.Show.UseCustomNamingFormat && pe.Show.CustomNamingFormat.HasValue()
                ? pe.Show.CustomNamingFormat
                : styleString;

            string showName = pe.Show.ShowName.Replace("[", "\\[").Replace("]", "\\]");

            string episodeName = pe.Name.Trim();
            if (urlEncode)
            {
                showName = Uri.EscapeDataString(showName);
                episodeName = Uri.EscapeDataString(episodeName);
            }

            name = name.ReplaceInsensitive("{ShowName}", showName);
            name = name.ReplaceInsensitive("{ShowNameLower}", pe.Show.ShowName.ToLower().Replace(' ', '-').RemoveCharactersFrom("()[]{}&$:"));
            name = name.ReplaceInsensitive("{ShowNameInitial}", showName.Initial().ToLower());
            name = name.ReplaceYear(pe.Show);
            name = name.ReplaceInsensitive("{Season}", pe.AppropriateSeasonNumber.ToString());
            name = name.ReplaceInsensitive("{Season:2}", pe.AppropriateSeasonNumber.ToString("00"));
            name = name.ReplaceInsensitive("{SeasonNumber}", pe.AppropriateSeasonIndex.ToString());
            name = name.ReplaceInsensitive("{SeasonNumber:2}", pe.AppropriateSeasonIndex.ToString("00"));

            string? seasonName = pe.Show.CachedShow?.Season(pe.AppropriateSeasonNumber)?.SeasonName;
            name = name.ReplaceInsensitive("{SeasonName}", seasonName ?? string.Empty);

            string episodeFormat = pe.AppropriateProcessedSeason.Episodes.Count >= 100 ? "000" : "00";
            name = name.ReplaceInsensitive("{Episode}", pe.AppropriateEpNum.ToString(episodeFormat));
            name = name.ReplaceInsensitive("{Episode2}", pe.EpNum2.ToString(episodeFormat));

            name = name.ReplaceInsensitive("{EpisodeName}", episodeName);
            name = name.ReplaceInsensitiveLazy("{Number}", new Lazy<string?>(() => pe.OverallNumber.ToString()), StringComparison.CurrentCultureIgnoreCase);
            name = name.ReplaceInsensitiveLazy("{Number:2}", new Lazy<string?>(() => pe.OverallNumber.ToString("00")), StringComparison.CurrentCultureIgnoreCase);
            name = name.ReplaceInsensitiveLazy("{Number:3}", new Lazy<string?>(() => pe.OverallNumber.ToString("000")), StringComparison.CurrentCultureIgnoreCase);
            name = name.ReplaceInsensitiveLazy("{Year}", new Lazy<string?>(() => pe.TheCachedSeries.MinYear.ToString()), StringComparison.CurrentCultureIgnoreCase);
            name = name.ReplaceInsensitiveLazy("{SeasonYear}", new Lazy<string?>(() => pe.AppropriateProcessedSeason.MinYear().ToString()), StringComparison.CurrentCultureIgnoreCase);
            name = name.ReplaceInsensitive("{Imdb}", pe.ImdbCode);
            name = name.ReplaceInsensitive("{ShowImdb}", pe.Show.CachedShow?.Imdb ?? string.Empty);

            name = ReplaceDates(urlEncode, name, pe.GetAirDateDt(false));
            name = Regex.Replace(name, "{AllEpisodes}", AllEpsText(pe), RegexOptions.IgnoreCase);

            if (pe.EpNum2 == pe.AppropriateEpNum)
            {
                name = Regex.Replace(name, "([^\\\\])\\[.*?[^\\\\]\\]", "$1"); // remove optional parts
            }
            else
            {
                name = Regex.Replace(name, "([^\\\\])\\[(.*?[^\\\\])\\]", "$1$2"); // remove just the brackets
            }

            name = name.Replace("\\[", "[");
            name = name.Replace("\\]", "]");

            return name.Trim();
        }
        catch (ArgumentNullException)
        {
            Logger.Error($"Asked to update {styleString} with information from {pe.Show.ShowName}, {pe.SeasonNumberAsText}, {pe.EpNumsAsString()}");
        }

        return string.Empty;
    }

    private static string AllEpsText(ProcessedEpisode pe)
    {
        string allEps = string.Empty;
        for (int i = pe.AppropriateEpNum; i <= pe.EpNum2; i++)
        {
            allEps += "E" + i.ToString("00");
        }

        return allEps;
    }
}
