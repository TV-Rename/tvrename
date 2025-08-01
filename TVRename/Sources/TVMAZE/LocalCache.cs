//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using Alphaleonis.Win32.Filesystem;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// Talk to the TVmaze web API, and get tv cachedSeries info

// Hierarchy is:
//   TVmaze -> Series (class CachedSeriesInfo) -> Seasons (class Season) -> Episodes (class Episode)

namespace TVRename.TVmaze;

// ReSharper disable once InconsistentNaming
public class LocalCache : MediaCache, iTVSource
{
    //We are using the singleton design pattern
    //http://msdn.microsoft.com/en-au/library/ff650316.aspx

    private static volatile LocalCache? InternalInstance;
    private static readonly object SyncRoot = new();

    public static LocalCache Instance
    {
        get
        {
            if (InternalInstance is null)
            {
                lock (SyncRoot)
                {
                    // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
                    if (InternalInstance is null)
                    {
                        InternalInstance = new LocalCache();
                    }
                }
            }

            return InternalInstance;
        }
    }

    public override int PrimaryKey(ISeriesSpecifier ss) => ss.TvMazeId;

    public override string CacheSourceName() => "TVMaze";
    public void Setup(FileInfo? loadFrom, FileInfo cache, bool showIssues)
    {
        System.Diagnostics.Debug.Assert(cache != null);
        CacheFile = cache;

        LastErrorMessage = string.Empty;

        LoadOk = loadFrom is null || CachePersistor.LoadTvCache(loadFrom, this);
    }

    public bool Connect(bool showErrorMsgBox) => true;

    public void SaveCache()
    {
        lock (SERIES_LOCK)
        {
            CachePersistor.SaveCache(Series, Movies, CacheFile!, 0);
        }
    }

    /// <exception cref="SourceConsistencyException">Condition.</exception>
    /// <exception cref="MediaNotFoundException">Condition.</exception>
    public override bool EnsureUpdated(ISeriesSpecifier s, bool bannersToo, bool showErrorMsgBox)
    {
        if (s.Provider != TVDoc.ProviderType.TVmaze)
        {
            throw new SourceConsistencyException(
                $"Asked to update {s.Name} from TV Maze, but the Id is not for TV maze.",
                TVDoc.ProviderType.TVmaze);
        }

        lock (SERIES_LOCK)
        {
            if (Series.TryGetValue(s.TmdbId, out CachedSeriesInfo? si) && !si.Dirty)
            {
                return true;
            }
        }

        Say($"{s.Name} from TVmaze");
        try
        {
            CachedSeriesInfo downloadedSi = API.GetSeriesDetails(s);

            if (downloadedSi.TvMazeCode != s.TvMazeId && s.TvMazeId == -1)
            {
                lock (SERIES_LOCK)
                {
                    Series.TryRemove(-1, out _);
                }
            }

            this.AddSeriesToCache(downloadedSi);
        }
        catch (SourceConnectivityException conex)
        {
            LOGGER.Warn(conex.ErrorText());
            LastErrorMessage = conex.Message;
            return true;
        }
        catch (SourceConsistencyException sce)
        {
            LOGGER.Error(sce.ErrorText());
            LastErrorMessage = sce.Message;
            return true;
        }
        SayNothing();
        return true;
    }

    public override bool GetUpdates(List<ISeriesSpecifier> ss, bool showErrorMsgBox, CancellationToken cts)
    {
        Say("Validating TVmaze cache");
        foreach (ISeriesSpecifier downloadShow in ss.Where(downloadShow => !HasSeries(downloadShow.TvMazeId)))
        {
            this.AddPlaceholderSeries(downloadShow);
        }

        try
        {
            Say("Updates list from TVmaze");
            IEnumerable<KeyValuePair<string, long>> updateTimes = API.GetUpdates();

            Say("Processing updates from TVmaze");
            foreach (KeyValuePair<string, long> showUpdateTime in updateTimes)
            {
                if (!cts.IsCancellationRequested)
                {
                    int showId = int.Parse(showUpdateTime.Key);

                    if (showId > 0 && HasSeries(showId))
                    {
                        CachedSeriesInfo? x = GetSeries(showId);
                        if (x is not null)
                        {
                            if (x.SrvLastUpdated < showUpdateTime.Value)
                            {
                                LOGGER.Info(
                                    $"Identified that show with TVMaze Id {showId} {x.Name} should be updated as update time is now {showUpdateTime.Value} and cache has {x.SrvLastUpdated}. ie {showUpdateTime.Value.FromUnixTime().ToLocalTime()} to {x.SrvLastUpdated.FromUnixTime().ToLocalTime()}.");

                                x.Dirty = true;
                            }
                        }
                    }
                }
                else
                {
                    SayNothing();
                    return true;
                }
            }

            this.MarkPlaceholdersDirty();

            SayNothing();
            return true;
        }
        catch (SourceConnectivityException conex)
        {
            LastErrorMessage = conex.Message;
            LOGGER.Warn(conex.ErrorText());
            return false;
        }
        catch (SourceConsistencyException sce)
        {
            LOGGER.Error(sce.ErrorText());
            LastErrorMessage = sce.Message;
            return false;
        }
        catch (TaskCanceledException)
        {
            LastErrorMessage = "Request to get updates Cancelled";
            LOGGER.Warn(LastErrorMessage);
            return false;
        }
        catch (OverflowException ex)
        {
            LastErrorMessage = $"Error with the format of the TV Maze updates json: {ex.ErrorText()}";
            LOGGER.Warn(LastErrorMessage);
            return false;
        }
    }

    public void UpdatesDoneOk()
    {
        //No Need to do anything as we always refresh from scratch
    }

    /// <exception cref="SourceConsistencyException">Condition.</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    public CachedSeriesInfo? GetSeries(string showName, bool showErrorMsgBox, Locale preferredLocale)
    {
        Search(showName, showErrorMsgBox, MediaConfiguration.MediaType.tv, preferredLocale);

        if (string.IsNullOrEmpty(showName))
        {
            return null;
        }

        showName = showName.ToLower();

        List<CachedSeriesInfo> matchingShows = [.. this.GetSeriesDictMatching(showName).Values];

        return matchingShows.Count switch
        {
            0 => null,
            1 => matchingShows.First(),
            _ => null
        };
    }

    /// <exception cref="SourceConsistencyException">Condition.</exception>
    /// <exception cref="SourceConnectivityException">If there is a problem connecting</exception>
    public override void Search(string text, bool showErrorMsgBox, MediaConfiguration.MediaType type,
        Locale preferredLocale)
    {
        if (type == MediaConfiguration.MediaType.tv)
        {
            if (text.IsNumeric())
            {
                if (int.TryParse(text, out int textAsInt))
                {
                    SearchSpecifier ss = new(textAsInt, preferredLocale,
                        TVDoc.ProviderType.TVmaze, type);
                    try
                    {
                        EnsureUpdated(ss, false, showErrorMsgBox);
                    }
                    catch (MediaNotFoundException)
                    {
                        //not really an issue so we can continue
                    }
                }
            }

            List<CachedSeriesInfo> results = [.. API.ShowSearch(text)];
            LOGGER.Info($"Got {results.Count:N0} results searching for {text} on TVMaze");

            foreach (CachedSeriesInfo result in results)
            {
                LOGGER.Info($"   Movie: {result.Name}:{result.TvMazeCode}   {result.Popularity}");
                this.AddSeriesToCache(result);
            }
        }
    }

    /// <exception cref="SourceConsistencyException">Condition.</exception>
    public void AddOrUpdateEpisode(Episode e)
    {
        lock (SERIES_LOCK)
        {
            if (!Series.TryGetValue(e.SeriesId, out CachedSeriesInfo? ser))
            {
                throw new SourceConsistencyException(
                    $"Can't find the cachedSeries to add the episode to (TVMaze). EpId:{e.EpisodeId} SeriesId:{e.SeriesId} {e.Name}",
                    TVDoc.ProviderType.TVmaze);
            }

            ser.AddEpisode(e);
        }
    }

    public void ForgetEverything()
    {
        lock (SERIES_LOCK)
        {
            Series.Clear();
        }

        SaveCache();
        LOGGER.Info("Forgot all TVMaze shows");
    }

    public void LatestUpdateTimeIs(string time)
    {
        //No Need to do anything aswe always refresh from scratch
    }

    public override TVDoc.ProviderType Provider() => TVDoc.ProviderType.TVmaze;
    TVDoc.ProviderType iTVSource.SourceProvider() => TVDoc.ProviderType.TVmaze;

    public override void ReConnect(bool b)
    {
        //nothing to be done here
    }
}
