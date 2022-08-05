using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace TVRename.TheTVDB;

internal class TvdbAccuracyCheck
{
    internal readonly List<string> Issues;
    internal readonly List<CachedSeriesInfo> ShowsToUpdate;
    internal readonly List<CachedMovieInfo> MoviesToUpdate;
    private readonly LocalCache lc;

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public TvdbAccuracyCheck(LocalCache localCache)
    {
        lc = localCache;
        Issues = new List<string>();
        ShowsToUpdate = new List<CachedSeriesInfo>();
        MoviesToUpdate = new List<CachedMovieInfo>();
    }

    public void ServerAccuracyCheck(CachedMovieInfo si)
    {
        Logger.Info($"Checking Accuracy of {si.Name} on TVDB");
        try
        {
            CachedMovieInfo? newSi = lc.DownloadMovieNow(si,si.TargetLocale, false);

            if (newSi is null)
            {
                Issues.Add($"Failed to compare {si.Name} as we could not download the cachedSeries details.");
                return;
            }

            if (!Match(newSi, si))
            {
                Issues.Add(
                    $"{si.Name} ({si.Id()}) is not up to date: Local is { si.SrvLastUpdated.FromUnixTime().ToLocalTime()} ({si.SrvLastUpdated}) server is { newSi.SrvLastUpdated.FromUnixTime().ToLocalTime()} ({newSi.SrvLastUpdated})");
                si.Dirty = true;
                if (!MoviesToUpdate.Contains(si))
                {
                    MoviesToUpdate.Add(si);
                }
            }
        }
        catch (SourceConnectivityException)
        {
            Issues.Add($"Failed to compare {si.Name} as we could not download the cachedSeries details.");
        }
    }
    public void ServerAccuracyCheck(CachedSeriesInfo si)
    {
        Logger.Info($"Checking Accuracy of {si.Name} on TVDB");
        if (TVSettings.Instance.TvdbVersion == ApiVersion.v4)
        {
            ServerAccuracyCheckV4(si);
            return;
        }
        try
        {
            CachedSeriesInfo newSi = lc.DownloadSeriesInfo(si, new Locale(), false);
            if (newSi.SrvLastUpdated != si.SrvLastUpdated)
            {
                Issues.Add(
                    $"{si.Name} ({si.Id()}) is not up to date: Local is {si.SrvLastUpdated.FromUnixTime().ToLocalTime()} ({si.SrvLastUpdated}) server is {newSi.SrvLastUpdated.FromUnixTime().ToLocalTime()} ({newSi.SrvLastUpdated})");

                EnsureUpdated(si);
            }

            List<JObject>? eps = lc.GetEpisodes(si.TvdbId, new Locale());
            List<int> serverEpIds = new();

            if (eps != null)
            {
                foreach (JObject epJson in eps)
                {
                    JToken? episodeToUse = epJson["data"];
                    if (episodeToUse != null)
                    {
                        foreach (JToken t in episodeToUse.Children())
                        {
                            int epId = EpisodeAccuracyCheck(si, t);
                            serverEpIds.Add(epId);
                        }
                    }
                    else
                    {
                        throw new SourceConsistencyException($"Could not load 'data' from {epJson}",
                            TVDoc.ProviderType.TheTVDB);
                    }
                }
            }

            //Look for episodes that are local, but not on server
            FindOrphanEpisodes(si, serverEpIds);
        }
        catch (SourceConnectivityException)
        {
            Issues.Add($"Failed to compare {si.Name} as we could not download the cachedSeries details.");
        }
        catch (MediaNotFoundException)
        {
            Issues.Add($"Failed to compare {si.Name} as it no longer exists on TVDB {si.TvdbId}.");
        }
    }

    public void ServerAccuracyCheckV4(CachedSeriesInfo si)
    {
        try
        {
            CachedSeriesInfo newSi = lc.DownloadSeriesInfo(si, new Locale(), false);
            if (newSi.SrvLastUpdated != si.SrvLastUpdated)
            {
                Issues.Add(
                    $"{si.Name} ({si.Id()}) is not up to date: Local is {si.SrvLastUpdated.FromUnixTime().ToLocalTime()} ({si.SrvLastUpdated}) server is {newSi.SrvLastUpdated.FromUnixTime().ToLocalTime()} ({newSi.SrvLastUpdated})");

                EnsureUpdated(si);
            }
            LocalCache.ReloadEpisodesV4(newSi, si.ActualLocale??new Locale(), newSi, si.SeasonOrder);

            foreach (Episode newEpisode in newSi.Episodes)
            {
                EpisodeAccuracyCheck(si, newEpisode);
            }

            //Look for episodes that are local, but not on server
            FindOrphanEpisodes(si, newSi.Episodes.Select(episode => episode.EpisodeId).ToList());
        }
        catch (SourceConnectivityException)
        {
            Issues.Add($"Failed to compare {si.Name} as we could not download the cachedSeries details.");
        }
        catch (MediaNotFoundException)
        {
            Issues.Add($"Failed to compare {si.Name} as it no longer exists on TVDB {si.TvdbId}.");
        }
    }

    private void FindOrphanEpisodes(CachedSeriesInfo si, List<int> serverEpIds)
    {
        foreach (Episode localEp in si.Episodes)
        {
            int localEpId = localEp.EpisodeId;
            if (!serverEpIds.Contains(localEpId))
            {
                Issues.Add($"{si.Name} {localEpId} should be removed: Server is missing.");
                localEp.Dirty = true;
                EnsureUpdated(si);
            }
        }
    }

    private void EnsureUpdated(CachedSeriesInfo si)
    {
        si.Dirty = true;
        if (!ShowsToUpdate.Contains(si))
        {
            ShowsToUpdate.Add(si);
        }
    }

    private int EpisodeAccuracyCheck(CachedSeriesInfo si, JToken t)
    {
        long serverUpdateTime = t.GetMandatoryLong("lastUpdated",TVDoc.ProviderType.TheTVDB);
        int epId = t.GetMandatoryInt("id",TVDoc.ProviderType.TheTVDB);

        try
        {
            Episode ep = si.GetEpisode(epId);

            if (serverUpdateTime != ep.SrvLastUpdated)
            {
                ep.Dirty = true;
                EnsureUpdated(si);
                string diff = serverUpdateTime > ep.SrvLastUpdated ? "not up to date" : "in the future";
                Issues.Add(
                    $"{si.Name} S{ep.AiredSeasonNumber}E{ep.AiredEpNum} ({ep.EpisodeId}) is {diff}: Local is {ep.SrvLastUpdated.FromUnixTime().ToLocalTime()} ({ep.SrvLastUpdated}) server is {serverUpdateTime.FromUnixTime().ToLocalTime()} ({serverUpdateTime})");
            }
        }
        catch (ShowConfiguration.EpisodeNotFoundException)
        {
            Issues.Add(
                $"{si.Name} {epId} is not found: Local is missing; server is {serverUpdateTime.FromUnixTime().ToLocalTime()} ({serverUpdateTime})");

            EnsureUpdated(si);
        }

        return epId;
    }

    private void EpisodeAccuracyCheck(CachedSeriesInfo si, Episode t)
    {
        long serverUpdateTime = t.SrvLastUpdated;
        int epId = t.EpisodeId;

        try
        {
            Episode ep = si.GetEpisode(epId);

            if (serverUpdateTime != ep.SrvLastUpdated)
            {
                ep.Dirty = true;
                EnsureUpdated(si);
                string diff = serverUpdateTime > ep.SrvLastUpdated ? "not up to date" : "in the future";
                Issues.Add(
                    $"{si.Name} S{ep.AiredSeasonNumber}E{ep.AiredEpNum} ({ep.EpisodeId}) is {diff}: Local is {ep.SrvLastUpdated.FromUnixTime().ToLocalTime()} ({ep.SrvLastUpdated}) server is {serverUpdateTime.FromUnixTime().ToLocalTime()} ({serverUpdateTime})");
            }
        }
        catch (ShowConfiguration.EpisodeNotFoundException)
        {
            Issues.Add(
                $"{si.Name} {epId} is not found: Local is missing; server is {serverUpdateTime.FromUnixTime().ToLocalTime()} ({serverUpdateTime})");

            EnsureUpdated(si);
        }
    }

    private static bool Match(CachedMovieInfo newSi, CachedMovieInfo si)
    {
        if (newSi.CollectionName != si.CollectionName)
        {
            return false;
        }

        if (newSi.Overview != si.Overview)
        {
            return false;
        }

        if (newSi.FirstAired != si.FirstAired)
        {
            return false;
        }

        //TODO - Check More fields
        return true;
    }

    public static void InvestigateUpdatesSince(int targetId, long baseTime)
    {
        for (int page = 0; page < 10000; page++)
        {
            Logger.Info($" BETA Update Checker: {page}");
            JObject? currentDownload = LocalCache.Instance.GetUpdatesJson(baseTime, page);
            JToken? jToken = currentDownload?["data"];

            if (jToken?.Children().Any() != true)
            {
                return;
            }

            foreach (JToken seriesResponse in jToken)
            {
                int id = seriesResponse.GetMandatoryInt("recordId", TVDoc.ProviderType.TheTVDB);
                //long time = seriesResponse.GetMandatoryLong("timeStamp", TVDoc.ProviderType.TheTVDB);
                string? entityType = (string?)seriesResponse["entityType"];
                //string method = (string)seriesResponse["method"];

                switch (entityType)
                {
                    case "series":
                    case "translatedseries":
                    case "seriespeople":
                        {
                            if (id != targetId)
                            {
                                continue;
                            }

                            Logger.Error(seriesResponse);
                            continue;
                        }
                    case "movies":
                    case "translatedmovies":
                    case "movie-genres":
                    //todo - make work for movies too
                    case "episodes":
                    case "translatedepisodes":
                        {
                            bool Predicate(KeyValuePair<int, CachedSeriesInfo> x) =>
                                x.Value.Episodes.Any(s => s.EpisodeId == id);

                            KeyValuePair<int, CachedSeriesInfo>? firstOrDefault =
                                LocalCache.Instance.CachedShowData.FirstOrDefault(Predicate);

                            int? episodeId = firstOrDefault?.Value?.TvdbId;

                            if (episodeId is null || episodeId != targetId)
                            {
                                continue;
                            }

                            Logger.Error(seriesResponse);
                            continue;
                        }
                    case "seasons":
                    case "translatedseasons":
                        {
                            bool Predicate(KeyValuePair<int, CachedSeriesInfo> x) => x.Value.Seasons.Any(s => s.SeasonId == id);
                            KeyValuePair<int, CachedSeriesInfo>? firstOrDefault =
                                LocalCache.Instance.CachedShowData.FirstOrDefault(Predicate);

                            int? seriesId = firstOrDefault?.Value?.TvdbId;

                            if (seriesId is null || seriesId != targetId)
                            {
                                continue;
                            }

                            Logger.Error(seriesResponse);
                            continue;
                        }
                    case "artwork":
                    case "artworktypes":
                    case "people":
                    case "characters":
                    case "award-nominees":
                    case "award_categories":
                    case "companies":
                    case "awards":
                    case "company_types":
                    case "movie_status":
                    case "content_ratings":
                    case "countries":
                    case "entity_types":
                    case "genres":
                    case "languages":
                    case "peopletypes":
                    case "seasontypes":
                    case "sourcetypes":
                    case "translatedpeople":
                    case "translatedcharacters":
                    case "lists":
                    case "translatedlists":
                    case "translatedcompanies":
                    case "tags":
                    case "tag-options":
                    case "award-categories":

                        continue;

                    default:
                        Logger.Error($"Found update record for '{entityType}' = {id}");
                        return;
                }
            }
        }
    }
}
