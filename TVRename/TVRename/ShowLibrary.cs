using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TVRename;

/// <summary>
/// Handles a thread-safe implementation of the 'library' this will hold all the ShowItem configuration as well
/// many methods that provide summaries of the data in the library
/// </summary>
public class ShowLibrary : ConcurrentDictionary<ShowConfiguration, int>
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public IEnumerable<ShowConfiguration> Shows => this.Keys;

    public IEnumerable<string> ShowStatuses => Shows.Select(item => item.ShowStatus).Distinct().OrderBy(s => s);

    public IEnumerable<string> SeasonWords()
    {
        //See https://github.com/TV-Rename/tvrename/issues/241 for background
        List<string> results = [.. TVSettings.Instance.SearchSeasonWordsArray];

        if (!TVSettings.Instance.ForceBulkAddToUseSettingsOnly)
        {
            IEnumerable<string> seasonWordsFromShows =
                from si in Shows select CustomSeasonName.GetTextFromPattern(si.AutoAddCustomFolderFormat);

            results.AddRange(seasonWordsFromShows.Distinct());

            results.Add(TVSettings.Instance.defaultSeasonWord);
        }

        return results.Where(t => !string.IsNullOrWhiteSpace(t)).Select(s => s.Trim()).Distinct();
    }

    public void AddShow(ShowConfiguration newShow, bool showErrors)
    {
        if (this.ContainsKey(newShow))
        {
            return;
        }

        List<ShowConfiguration> matchingShows = [.. Shows.Where(configuration => configuration.AnyIdsMatch(newShow))];
        if (matchingShows.Count == 0)
        {
            TryAdd(newShow, 0);
        }
        else
        {
            foreach (ShowConfiguration existingshow in matchingShows)
            {
                if (showErrors)
                {
                    //TODO Merge them in
                    Logger.Error($"Trying to add {newShow}, but we already have {existingshow}");
                    Logger.Error(Environment.StackTrace);
                }
                else
                {
                    Logger.Warn($"Trying to add {newShow}, but we already have {existingshow}");
                }
            }
            return;
        }
    }
    public void AddShows(List<ShowConfiguration>? newShow, bool showErrors)
    {
        if (newShow is null)
        {
            return;
        }

        foreach (ShowConfiguration toAdd in newShow)
        {
            AddShow(toAdd, showErrors);
        }
    }

    public IEnumerable<string> GetSeasonPatterns()
    {
        List<string> results = [TVSettings.Instance.SeasonFolderFormat];

        IEnumerable<string> seasonWordsFromShows = Shows.Select(si => si.AutoAddCustomFolderFormat);

        results.AddRange([.. seasonWordsFromShows.Distinct()]);

        return results;
    }

    public IEnumerable<string> GetGenres()
    {
        List<string> allGenres = [];
        foreach (ShowConfiguration si in Shows)
        {
            allGenres.AddRange(si.Genres);
        }

        List<string> distinctGenres = [.. allGenres.Distinct()];
        distinctGenres.Sort();
        return distinctGenres;
    }

    public IEnumerable<string> GetStatuses()
    {
        return Shows
            .Where(s => !string.IsNullOrWhiteSpace(s.ShowStatus))
            .Select(s => s.ShowStatus)
            .Distinct()
            .OrderBy(s => s);
    }

    public IEnumerable<string> GetTypes()
    {
        return Shows
            .Select(s => s.CachedShow?.SeriesType)
            .Distinct()
            .ValidStrings()
            .OrderBy(s => s);
    }

    public IEnumerable<string> GetNetworks()
    {
        return Shows
            .Select(si => si.CachedShow)
            .Where(seriesInfo => !string.IsNullOrWhiteSpace(seriesInfo?.Network))
            .OfType<CachedSeriesInfo>()
            .SelectMany(seriesInfo => seriesInfo.Networks)
            .Distinct()
            .OrderBy(s => s);
    }

    public IEnumerable<string> GetContentRatings()
    {
        return Shows.Select(si => si.CachedShow)
            .Where(s => !string.IsNullOrWhiteSpace(s?.ContentRating))
            .OfType<CachedSeriesInfo>()
            .Select(s => s.ContentRating)
            .ValidStrings()
            .Distinct()
            .OrderBy(s => s);
    }

    public List<ShowConfiguration> GetSortedShows()
    {
        List<ShowConfiguration> returnList;
        {
            returnList = [.. Shows];
        }
        returnList.Sort(MediaConfiguration.CompareNames);
        return returnList;
    }

    public ShowConfiguration? GetShowItem(int id, TVDoc.ProviderType provider)
    {
        if (id is 0 or -1)
        {
            return null;
        }
        List<ShowConfiguration> matching =
            [.. Shows.Where(configuration => configuration.IdFor(provider) == id)];

        switch (matching.Count)
        {
            case 0:
                return null;
            case 1:
                return matching.First();
        }

        //OK we have multiple!!
        Logger.Error($"Searched for {id} on {provider.PrettyPrint()} TV Show Library has multiple: {matching.Select(x => x.ToString()).ToCsv()}");
        return matching.FirstOrDefault(x => x.Provider == provider) ?? matching.First();
    }

    public void UpdateEpisodeCaches()
    {
        foreach (ShowConfiguration show in Shows)
        {
            show.UpdateEpisodeCaches();
        }
    }

    internal IEnumerable<ShowConfiguration> GetRecentShows() => GetSortedShows().Where(IsRecent);

    private static bool IsRecent(ShowConfiguration si)
    {
        // only scan "recent" shows
        int days = TVSettings.Instance.WTWRecentDays;
        return si.ActiveSeasons.ToList().Select(pair => pair.Value).SelectMany(eis => eis).Any(ei => ei.WithinLastDays(days));
    }

    public List<ProcessedEpisode> NextNShows(int nShows, int nDaysPast, int nDaysFuture)
    {
        DateTime notBefore = TimeHelpers.LocalNow().AddDays(-nDaysPast);
        List<ProcessedEpisode> found = [];

        for (int i = 0; i < nShows; i++)
        {
            ProcessedEpisode? nextAfterThat = GetNextMostRecentProcessedEpisode(nDaysFuture, found, notBefore);

            if (nextAfterThat is null)
            {
                return found;
            }

            DateTime? nextdt = nextAfterThat.GetAirDateDt();
            if (nextdt.HasValue)
            {
                notBefore = nextdt.Value;
                found.Add(nextAfterThat);
            }
        }

        return found;
    }

    private ProcessedEpisode? GetNextMostRecentProcessedEpisode(int nDaysFuture, List<ProcessedEpisode> found, DateTime notBefore)
    {
        ProcessedEpisode? nextAfterThat = null;
        TimeSpan howClose = TimeSpan.MaxValue;
        foreach (ShowConfiguration si in GetSortedShows())
        {
            {
                if (!si.ShowNextAirdate)
                {
                    continue;
                }

                foreach (KeyValuePair<int, List<ProcessedEpisode>> v in si.ActiveSeasons.ToList())
                {
                    if (si.IgnoreSeasons.Contains(v.Key))
                    {
                        continue; // ignore this season
                    }

                    if (v.Key == 0 && TVSettings.Instance.IgnoreAllSpecials)
                    {
                        continue;
                    }

                    foreach (ProcessedEpisode ei in v.Value)
                    {
                        if (found.Contains(ei))
                        {
                            continue;
                        }

                        DateTime? airdt = ei.GetAirDateDt();

                        if (airdt is null || airdt == DateTime.MaxValue)
                        {
                            continue;
                        }

                        DateTime dt = airdt.Value;

                        TimeSpan timeUntil = dt.Subtract(TimeHelpers.LocalNow());
                        if (timeUntil.TotalDays > nDaysFuture)
                        {
                            continue; //episode is too far in the future
                        }

                        TimeSpan ts = dt.Subtract(notBefore);
                        if (ts.TotalSeconds < 0)
                        {
                            continue; //episode is too far in the past
                        }

                        //if we have a closer match
                        if (TimeSpan.Compare(ts, howClose) < 0)
                        {
                            howClose = ts;
                            nextAfterThat = ei;
                        }
                    }
                }
            }
        }

        return nextAfterThat;
    }

    public async Task<List<ProcessedEpisode>> GetRecentAndFutureEpsAsync(int recentDays)
    {
        List<ProcessedEpisode> returnList = [];
        DateTime now = TimeHelpers.LocalNow();
        DateTime limit = now.AddDays(-recentDays);

        foreach (ShowConfiguration si in Shows)
        {
            if (!si.ShowNextAirdate)
            {
                continue;
            }

            foreach (List<ProcessedEpisode> eis in si.ActiveSeasons.Select(p => p.Value))
            {
                bool nextToAirFound = false;

                foreach (ProcessedEpisode ei in eis
                             .Where(ei => ei.HasAiredDate())
                             .Where(ei => ei.GetAirDateDt() >= limit)
                             .OrderBy(ei => ei.GetAirDateDt()))
                {
                    DateTime? dt = ei.GetAirDateDt();

                    if (dt > now && !nextToAirFound)
                    {
                        nextToAirFound = true;
                        ei.NextToAir = true;
                    }
                    else
                    {
                        ei.NextToAir = false;
                    }

                    returnList.Add(ei);
                }
            }
        }

        return returnList;
    }

    public void LoadFromXml(XElement xmlSettings)
    {
        foreach (ShowConfiguration si in xmlSettings.Descendants("ShowItem").Select(showSettings => new ShowConfiguration(showSettings)))
        {
            if (si.UseCustomShowName) // see if custom show name is actually the real show name
            {
                CachedSeriesInfo? ser = si.CachedShow;
                if (ser != null && si.CustomShowName == ser.Name)
                {
                    // then, turn it off
                    si.CustomShowName = string.Empty;
                    si.UseCustomShowName = false;
                }
            }

            AddShow(si, false);
        }
    }

    public IEnumerable<ProcessedEpisode> RecentEpisodes(int days)
    {
        List<ProcessedEpisode> episodes = [];

        // for each show, see if any episodes were aired in "recent" days...
        foreach (ShowConfiguration si in GetRecentShows())
        {
            foreach (KeyValuePair<int, List<ProcessedEpisode>> kvp in si.ActiveSeasons)
            {
                foreach (ProcessedEpisode ei in kvp.Value)
                {
                    if (ei.WithinLastDays(days))
                    {
                        episodes.Add(ei);
                    }
                }
            }
        }

        return episodes;
    }

    public ShowConfiguration? GetShowItem(ISeriesSpecifier ai) => GetShowItem(ai.Id(), ai.Provider)
                                                                  ?? GetShowItem(ai.TmdbId, TVDoc.ProviderType.TMDB)
                                                                  ?? GetShowItem(ai.TvdbId, TVDoc.ProviderType.TheTVDB)
                                                                  ?? GetShowItem(ai.TvMazeId, TVDoc.ProviderType.TVmaze);

    internal void AddAlias(ShowConfiguration sc, string hint)
    {
        if (this.Shows.Contains(sc))
        {
            sc.CheckHintExists(hint);
            return;
        }

        List<ShowConfiguration> matchingShows = [.. Shows.Where(configuration => configuration.AnyIdsMatch(sc))];

        switch (matchingShows.Count)
        {
            case 0:
                return;
            case 1:
                matchingShows.First().CheckHintExists(hint);
                break;
            default:
                {
                    Logger.Warn($"Asked to add {hint} to {sc.Name}, but multple shows match {matchingShows.Select(x => x.Name).ToCsv()}");
                    break;
                }
        }
    }

    internal List<ProcessedEpisode>? GetRandomSeasonEpisodes()
    {
        return GetSortedShows().SelectMany(si => si.GetSortedSeasons()).FirstOrDefault();
    }

    internal void Remove(ShowConfiguration si)
    {
        this.TryRemove(si, out _);
    }
}
