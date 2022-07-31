using System;
using System.Collections.Generic;

namespace TVRename.TMDB;

internal class TmdbAccuracyCheck
{
    internal readonly List<string> Issues;
    internal readonly List<CachedSeriesInfo> ShowsToUpdate;
    internal readonly List<CachedMovieInfo> MoviesToUpdate;
    private readonly LocalCache lc;

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public TmdbAccuracyCheck(LocalCache localCache)
    {
        lc = localCache;
        Issues = new List<string>();
        ShowsToUpdate = new List<CachedSeriesInfo>();
        MoviesToUpdate = new List<CachedMovieInfo>();
    }

    public void ServerAccuracyCheck(CachedMovieInfo si)
    {
        Logger.Info($"Accuracy Check for {si.Name} on TMDB");
        try
        {
            CachedMovieInfo newSi = lc.DownloadMovieNow(si,false);

            if (!Match(newSi, si))
            {
                Issues.Add(
                    $"{si.Name} ({si.Id()}) is not up to date: Local is {si.SrvLastUpdated.FromUnixTime().ToLocalTime()} ({si.SrvLastUpdated}) server is {newSi.SrvLastUpdated.FromUnixTime().ToLocalTime()} ({newSi.SrvLastUpdated})");
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
        Logger.Info($"Accuracy Check for {si.Name} on TMDB");
        try
        {
            CachedSeriesInfo newSi = lc.DownloadSeriesNow(si,false);

            if (!Match(newSi, si)) //NB - we use a match method as we can't rely on update time
            {
                Issues.Add(
                    $"{si.Name} ({si.Id()}) is not up to date: Local is { si.SrvLastUpdated.FromUnixTime().ToLocalTime()} ({si.SrvLastUpdated}) server is { newSi.SrvLastUpdated.FromUnixTime().ToLocalTime()} ({newSi.SrvLastUpdated})");
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

    private static bool Match(CachedMovieInfo newSi, CachedMovieInfo si) =>
        (newSi.CollectionName ?? string.Empty) == (si.CollectionName ?? string.Empty)
        && (newSi.TagLine ?? string.Empty) == (si.TagLine ?? string.Empty)
        && Match((CachedMediaInfo)newSi, si);

    private static bool Match(CachedMediaInfo newSi, CachedMediaInfo si)
    {
        if (newSi.Name != si.Name)
        {
            return false;
        }

        if (newSi.Overview != si.Overview)
        {
            return false;
        }

        if ((newSi.Country ?? string.Empty) != (si.Country ?? string.Empty))
        {
            return false;
        }

        if ((newSi.Network ?? string.Empty) != (si.Network ?? string.Empty))
        {
            return false;
        }

        if ((newSi.Runtime ?? string.Empty) != (si.Runtime ?? string.Empty))
        {
            return false;
        }
        if (newSi.Status != si.Status)
        {
            return false;
        }
        if (newSi.ContentRating != si.ContentRating)
        {
            return false;
        }

        return true;
    }
}
