using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace TVRename.TheTVDB
{
    internal class TvdbAccuracyCheck
    {
        [NotNull] internal readonly List<string> Issues;
        [NotNull] internal readonly List<CachedSeriesInfo> ShowsToUpdate;
        [NotNull] private readonly LocalCache lc;

        public TvdbAccuracyCheck(LocalCache localCache)
        {
            lc = localCache;
            Issues = new List<string>();
            ShowsToUpdate = new List<CachedSeriesInfo>();
        }

        public void ServerAccuracyCheck([NotNull] CachedSeriesInfo si)
        {
            int tvdbId = si.TvdbCode;
            try
            {
                CachedSeriesInfo newSi = lc.DownloadSeriesInfo(tvdbId, new Locale(), false);
                if (newSi.SrvLastUpdated != si.SrvLastUpdated)
                {
                    Issues.Add(
                        $"{si.Name} is not up to date: Local is {DateTimeOffset.FromUnixTimeSeconds(si.SrvLastUpdated)} ({si.SrvLastUpdated}) server is {DateTimeOffset.FromUnixTimeSeconds(newSi.SrvLastUpdated)} ({newSi.SrvLastUpdated})");

                    EnsureUpdated(si);
                }

                List<JObject> eps = lc.GetEpisodes(tvdbId, new Locale(Languages.Instance.GetLanguageFromCode("en")));
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
                Issues.Add($"Failed to compare {si.Name} as it no longer exists on TVDB {tvdbId}.");
            }
        }

        private void FindOrphanEpisodes(CachedSeriesInfo si, List<long> serverEpIds)
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

        private int EpisodeAccuracyCheck([NotNull] CachedSeriesInfo si, [NotNull] JToken t)
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
            catch (ShowConfiguration.EpisodeNotFoundException)
            {
                Issues.Add(
                    $"{si.Name} {epId} is not found: Local is missing; server is {serverUpdateTime}");

                EnsureUpdated(si);
            }

            return epId;
        }
    }
}
