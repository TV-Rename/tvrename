using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace TVRename.TMDB
{
    internal class TmdbAccuracyCheck
    {
        [NotNull] internal readonly List<string> Issues;
        [NotNull] internal readonly List<CachedSeriesInfo> ShowsToUpdate;
        [NotNull] internal readonly List<CachedMovieInfo> MoviesToUpdate;
        [NotNull] private readonly LocalCache lc;

        public TmdbAccuracyCheck(LocalCache localCache)
        {
            lc = localCache;
            Issues = new List<string>();
            ShowsToUpdate = new List<CachedSeriesInfo>();
            MoviesToUpdate = new List<CachedMovieInfo>();
        }

        public void ServerAccuracyCheck([NotNull] CachedMovieInfo si)
        {
            try
            {
                CachedMovieInfo newSi = lc.DownloadMovieNow(si, false,false);

                if (!Match(newSi, si))
                {
                    Issues.Add(
                        $"{si.Name} is not up to date: Local is { DateTimeOffset.FromUnixTimeSeconds(si.SrvLastUpdated)} ({si.SrvLastUpdated}) server is { DateTimeOffset.FromUnixTimeSeconds(newSi.SrvLastUpdated)} ({newSi.SrvLastUpdated})");
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

        public void ServerAccuracyCheck([NotNull] CachedSeriesInfo si)
        {
            try
            {
                CachedSeriesInfo newSi = lc.DownloadSeriesNow(si, false,false);

                if (!Match(newSi, si)) //NB - we use a match method as we can't rely on update time
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
            if (newSi.ContentRating != si.ContentRating) return false;
            if (newSi.Name != si.Name) return false;
            if (newSi.TagLine != si.TagLine) return false;
            if (newSi.Network != si.Network) return false;

            return true;
        }

        private bool Match(CachedSeriesInfo newSi, CachedSeriesInfo si)
        {
            if (newSi.Name != si.Name) return false;
            if (newSi.Overview != si.Overview) return false;
            //TODO - Check More fields
            return true;
        }
    }
}
