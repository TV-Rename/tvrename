using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using NLog;

namespace TVRename.TheTVDB;

internal class TvdbAccuracyCheck
{
    internal readonly SafeList<string> Issues;
    internal readonly SafeList<CachedSeriesInfo> ShowsToUpdate;
    internal readonly SafeList<CachedMovieInfo> MoviesToUpdate;

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public TvdbAccuracyCheck()
    {
        Issues = [];
        ShowsToUpdate = [];
        MoviesToUpdate = [];
    }

    public void ServerAccuracyCheck(CachedMovieInfo si)
    {
        Logger.Info($"Checking Accuracy of {si.Name} on TVDB");
        try
        {
            CachedMovieInfo newSi = API.DownloadMovieInfo(si, si.TargetLocale);

            if (Match(newSi, si))
            {
                return;
            }

            Issues.Add(
                $"{si.Name} ({si.Id()}) is not up to date: Local is {si.SrvLastUpdated.FromUnixTime().ToLocalTime()} ({si.SrvLastUpdated}) server is {newSi.SrvLastUpdated.FromUnixTime().ToLocalTime()} ({newSi.SrvLastUpdated})");
            si.Dirty = true;
            if (!MoviesToUpdate.Contains(si))
            {
                MoviesToUpdate.Add(si);
            }
        }
        catch (SourceConnectivityException)
        {
            Issues.Add($"Failed to compare {si.Name} as we could not download the cachedSeries details.");
        }
        catch (SourceConsistencyException ex)
        {
            Logger.Error($"Failed to compare {si.Name} as we could not download the cachedSeries details.", ex);
        }
        catch (MediaNotFoundException)
        {
            Issues.Add($"Failed to compare {si.Name} as it no longer exists on TVDB {si.TvdbId}.");
        }
    }
    
    public void ServerAccuracyCheck(CachedSeriesInfo si)
    {
        Logger.Info($"Checking Accuracy of {si.Name} on TVDB");

        try
        {
            CachedSeriesInfo newSi = API.DownloadSeriesInfo(si, si.TargetLocale);
            TvShowAccuracyCheck(si, newSi);

            API.ReloadEpisodes(newSi, si.ActualLocale ?? new Locale(), newSi, si.SeasonOrder);
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
        catch (SourceConsistencyException ex)
        {
            Logger.Error($"Failed to compare {si.Name} as we could not download the cachedSeries details.",ex);
        }
        catch (MediaNotFoundException)
        {
            Issues.Add($"Failed to compare {si.Name} as it no longer exists on TVDB {si.TvdbId}.");
        }

        void TvShowAccuracyCheck(CachedSeriesInfo testSi, CachedSeriesInfo newSi)
        {
            if (newSi.SrvLastUpdated == testSi.SrvLastUpdated)
            {
                return;
            }

            Issues.Add(
                $"{testSi.Name} ({testSi.Id()}) is not up to date: Local is {testSi.SrvLastUpdated.FromUnixTime().ToLocalTime()} ({testSi.SrvLastUpdated}) server is {newSi.SrvLastUpdated.FromUnixTime().ToLocalTime()} ({newSi.SrvLastUpdated})");

            EnsureUpdated(testSi);
        }

        void FindOrphanEpisodes(CachedSeriesInfo testSi, ICollection<int> serverEpIds)
        {
            foreach (Episode localEp in testSi.Episodes)
            {
                int localEpId = localEp.EpisodeId;
                if (!serverEpIds.Contains(localEpId))
                {
                    Issues.Add($"{testSi.Name} {localEpId} should be removed: Server is missing.");
                    localEp.Dirty = true;
                    EnsureUpdated(testSi);
                }
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

    private void EpisodeAccuracyCheck(CachedSeriesInfo si, Episode t)
    {
        long serverUpdateTime = t.SrvLastUpdated;
        int epId = t.EpisodeId;

        try
        {
            Episode ep = si.GetEpisode(epId);

            if (serverUpdateTime == ep.SrvLastUpdated)
            {
                return;
            }

            ep.Dirty = true;
            EnsureUpdated(si);
            string diff = serverUpdateTime > ep.SrvLastUpdated ? "not up to date" : "in the future";
            Issues.Add(
                $"{si.Name}({si.TvdbId}), S{ep.AiredSeasonNumber}E{ep.AiredEpNum} ({ep.EpisodeId}) is {diff}: Local is {ep.SrvLastUpdated.FromUnixTime().ToLocalTime()} ({ep.SrvLastUpdated}) server is {serverUpdateTime.FromUnixTime().ToLocalTime()} ({serverUpdateTime})");
        }
        catch (EpisodeNotFoundException)
        {
            Issues.Add(
                $"{si.Name}({si.TvdbId}), episode with Id {epId} is not found: Local is missing; server was updated on {serverUpdateTime.FromUnixTime().ToLocalTime()} ({serverUpdateTime})");

            EnsureUpdated(si);
        }
    }

    private static bool Match(CachedMovieInfo newSi, CachedMovieInfo si)
    {
        if (newSi.CollectionName != si.CollectionName && newSi.CollectionName.HasValue() && si.CollectionName.HasValue())
        {
            return false;
        }

        if (newSi.Overview != si.Overview)
        {
            return false;
        }

        if (newSi.FirstAired.HasValue && si.FirstAired.HasValue && !newSi.FirstAired.Value.EqualsUpToSeconds(si.FirstAired.Value))
        {
            return false;
        }

        if (newSi.SrvLastUpdated != si.SrvLastUpdated)
        {
            return false;
        }
        //TODO - Check More fields
        return true;
    }

    /// <exception cref="SourceConsistencyException">If there is a problem with what is returned</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    public static void InvestigateUpdatesSince(int targetId, long baseTime)
    {
        for (int page = 0; page < 10000; page++)
        {
            Logger.Info($" BETA Update Checker: {page}");
            JObject currentDownload = TvdbWebApi.GetUpdates(baseTime, page)
                                      ?? throw new SourceConsistencyException("Could not get updates from TVDB", TVDoc.ProviderType.TheTVDB);
            JToken? jToken = currentDownload["data"];

            if (jToken?.Children().Any() != true)
            {
                return;
            }

            foreach (JToken seriesResponse in jToken)
            {
                int id = seriesResponse.GetMandatoryInt("recordId", TVDoc.ProviderType.TheTVDB);
                long time = seriesResponse.GetMandatoryLong("timeStamp", TVDoc.ProviderType.TheTVDB);
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

                            Logger.Warn($"SERIES: {time.FromUnixTime().ToLocalTime()}:{seriesResponse}");
                            continue;
                        }
                    case "movies":
                    case "translatedmovies":
                    case "movie-genres":
                        {
                            if (id != targetId)
                            {
                                continue;
                            }

                            Logger.Warn($"MOVIE: {time.FromUnixTime().ToLocalTime()}:{seriesResponse}");
                            continue;
                        }
                    case "episodes":
                    case "translatedepisodes":
                        {
                            Episode? targetEpisode = LocalCache.Instance.CachedShowData.Values
                                .Where(s => s.Id() == targetId)
                                .SelectMany(s => s.Episodes)
                                .FirstOrDefault(e => e.EpisodeId == id);

                            if (targetEpisode is null)
                            {
                                continue;
                            }

                            Logger.Warn($"EPISODE: S{targetEpisode.AiredSeasonNumber:D2}E{targetEpisode.AiredEpNum:D2}: {time.FromUnixTime().ToLocalTime()}:{seriesResponse}");
                            continue;
                        }
                    case "seasons":
                    case "translatedseasons":
                        {
                            Season? targetSeason = LocalCache.Instance.CachedShowData.Values
                                .Where(s => s.Id() == targetId)
                                .SelectMany(s => s.Seasons)
                                .FirstOrDefault(e => e.SeasonId == id);

                            if (targetSeason is null)
                            {
                                continue;
                            }

                            Logger.Warn($"SEASON: S{targetSeason.SeasonNumber:D2}:{targetSeason.SeasonName}: {time.FromUnixTime().ToLocalTime()}:{seriesResponse}");
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
