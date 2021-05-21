using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace TVRename.TMDB
{
    internal class TmdbAccuracyCheck
    {
        [NotNull] internal readonly List<string> Issues;
        [NotNull] internal readonly List<CachedMovieInfo> ShowsToUpdate;
        [NotNull] private readonly LocalCache lc;

        //TODO - make this work for TV Shows too

        public TmdbAccuracyCheck(LocalCache localCache)
        {
            lc = localCache;
            Issues = new List<string>();
            ShowsToUpdate = new List<CachedMovieInfo>();
        }

        public void ServerAccuracyCheck([NotNull] CachedMovieInfo si)
        {
            int tvdbId = si.TmdbCode;
            try
            {
                CachedMovieInfo newSi = lc.DownloadMovieNow(tvdbId, si.ActualLocale, false);

                if (!Match(newSi, si))
                {
                    Issues.Add(
                        $"{si.Name} is not up to date: Local is { DateTimeOffset.FromUnixTimeSeconds(si.SrvLastUpdated)} ({si.SrvLastUpdated}) server is { DateTimeOffset.FromUnixTimeSeconds(newSi.SrvLastUpdated)} ({newSi.SrvLastUpdated})");
                    si.Dirty = true;
                    if (!ShowsToUpdate.Contains(si))
                    {
                        ShowsToUpdate.Add(si);
                    }
                }
            }
            catch (SourceConnectivityException)
            {
                Issues.Add($"Failed to compare {si.Name} as we could not download the cachedSeries details.");
            }
        }

        private bool Match(CachedMovieInfo newSi, CachedMovieInfo si)
        {
            if (newSi.CollectionName != si.CollectionName) return false;
            if (newSi.Overview != si.Overview) return false;
            //TODO - Check More fields
            return true;
        }
    }
}
