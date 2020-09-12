using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace TVRename.TheTVDB
{
    internal class TvdbAccuracyCheck
    {
        [NotNull] internal readonly List<string> Issues;
        [NotNull] internal readonly List<SeriesInfo> ShowsToUpdate;
        [NotNull] private readonly LocalCache lc;

        public TvdbAccuracyCheck(LocalCache localCache)
        {
            lc = localCache;
            Issues = new List<string>();
            ShowsToUpdate = new List<SeriesInfo>();
        }

        public void ServerAccuracyCheck([NotNull] SeriesInfo si)
        {
            int tvdbId = si.TvdbCode;
            try
            {
                SeriesInfo newSi = lc.DownloadSeriesInfo(tvdbId, "en", false);
                if (newSi.SrvLastUpdated != si.SrvLastUpdated)
                {
                    Issues.Add(
                        $"{si.Name} is not up to date: Local is { DateTimeOffset.FromUnixTimeSeconds(si.SrvLastUpdated)} ({si.SrvLastUpdated}) server is { DateTimeOffset.FromUnixTimeSeconds(newSi.SrvLastUpdated)} ({newSi.SrvLastUpdated})");
                    EnsureUpdated(si);
                }

                List<JObject> eps = lc.GetEpisodes(tvdbId, "en");
                List<long> serverEpIds = new List<long>();

                if (eps != null)
                {
                    foreach (JObject epJson in eps)
                    {
                        JToken episodeToUse = epJson["data"];
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
                            throw new SourceConsistencyException($"Could not load 'data' from {epJson}", ShowItem.ProviderType.TheTVDB);
                        }
                    }
                }

                //Look for episodes that are local, but not on server
                FindOrphanEpisodes(si, serverEpIds);
            }
            catch (SourceConnectivityException)
            {
                Issues.Add($"Failed to compare {si.Name} as we could not download the series details.");
            }
        }

        private void FindOrphanEpisodes(SeriesInfo si, List<long> serverEpIds)
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

        private void EnsureUpdated(SeriesInfo si)
        {
            si.Dirty = true;
            if (!ShowsToUpdate.Contains(si))
            {
                ShowsToUpdate.Add(si);
            }
        }

        private int EpisodeAccuracyCheck([NotNull] SeriesInfo si, [NotNull] JToken t)
        {
            long serverUpdateTime = (long)t["lastUpdated"];
            int epId = (int)t["id"];

            try
            {
                Episode ep = si.GetEpisode(epId);

                if (serverUpdateTime != ep.SrvLastUpdated)
                {
                    ep.Dirty = true;
                    EnsureUpdated(si);
                    string diff = serverUpdateTime > ep.SrvLastUpdated ? "not up to date" : "in the future";
                    Issues.Add(
                        $"{si.Name} S{ep.AiredSeasonNumber}E{ep.AiredEpNum} is {diff}: Local is {DateTimeOffset.FromUnixTimeSeconds(ep.SrvLastUpdated)} ({ep.SrvLastUpdated}) server is {DateTimeOffset.FromUnixTimeSeconds(serverUpdateTime)} ({serverUpdateTime})");
                }
            }
            catch (ShowItem.EpisodeNotFoundException)
            {
                Issues.Add(
                    $"{si.Name} {epId} is not found: Local is missing; server is {serverUpdateTime}");

                EnsureUpdated(si);
            }

            return epId;
        }
    }
}
