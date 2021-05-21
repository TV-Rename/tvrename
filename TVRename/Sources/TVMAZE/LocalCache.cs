//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using JetBrains.Annotations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

// Talk to the TVmaze web API, and get tv cachedSeries info

// Hierarchy is:
//   TVmaze -> Series (class CachedSeriesInfo) -> Seasons (class Season) -> Episodes (class Episode)

namespace TVRename.TVmaze
{
    // ReSharper disable once InconsistentNaming
    public class LocalCache : MediaCache, iTVSource
    {
        //We are using the singleton design pattern
        //http://msdn.microsoft.com/en-au/library/ff650316.aspx

        private static volatile LocalCache? InternalInstance;
        private static readonly object SyncRoot = new object();

        [NotNull]
        public static LocalCache Instance
        {
            get
            {
                if (InternalInstance is null)
                {
                    lock (SyncRoot)
                    {
                        InternalInstance ??= new LocalCache();
                    }
                }

                return InternalInstance;
            }
        }

        public void Setup(FileInfo? loadFrom, FileInfo cache, CommandLineArgs cla)
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
                CachePersistor.SaveCache(Series, Movies, CacheFile, 0);
            }
        }

        public override Language? PreferredLanguage() => throw new NotImplementedException();

        public override bool EnsureUpdated(ISeriesSpecifier s, bool bannersToo, bool showErrorMsgBox)
        {
            if (s.Provider != TVDoc.ProviderType.TVmaze)
            {
                throw new SourceConsistencyException($"Asked to update {s.Name} from TV Maze, but the Id is not for TV maze.", TVDoc.ProviderType.TVmaze);
            }

            lock (SERIES_LOCK)
            {
                if (Series.ContainsKey(s.TvMazeId) && !Series[s.TvMazeId].Dirty)
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

                lock (SERIES_LOCK)
                {
                    AddSeriesToCache(downloadedSi);
                }
            }
            catch (SourceConnectivityException conex)
            {
                LastErrorMessage = conex.Message;
                return true;
            }
            catch (SourceConsistencyException sce)
            {
                LOGGER.Error(sce.Message);
                LastErrorMessage = sce.Message;
                return true;
            }

            return true;
        }

        private void AddSeriesToCache([NotNull] CachedSeriesInfo si)
        {
            int id = si.TvMazeCode;
            lock (SERIES_LOCK)
            {
                if (Series.ContainsKey(id))
                {
                    Series[id].Merge(si);
                }
                else
                {
                    Series[id] = si;
                }
            }
        }

        public bool GetUpdates(bool showErrorMsgBox, CancellationToken cts, IEnumerable<ISeriesSpecifier> ss)
        {
            Say("Validating TVmaze cache");
            foreach (ISeriesSpecifier downloadShow in ss.Where(downloadShow => !HasSeries(downloadShow.TvMazeId)))
            {
                AddPlaceholderSeries(downloadShow);
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
                            CachedSeriesInfo x = GetSeries(showId);
                            if (!(x is null))
                            {
                                if (x.SrvLastUpdated < showUpdateTime.Value)
                                {
                                    LOGGER.Info($"Identified that show with TVMaze Id {showId} {x.Name} should be updated as update time is now {showUpdateTime.Value} and cache has {x.SrvLastUpdated}. ie {Helpers.FromUnixTime(showUpdateTime.Value).ToLocalTime()} to {Helpers.FromUnixTime(x.SrvLastUpdated).ToLocalTime()}.");
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

                MarkPlaceholdersDirty();

                SayNothing();
                return true;
            }
            catch (SourceConnectivityException conex)
            {
                LastErrorMessage = conex.Message;
                return false;
            }
            catch (SourceConsistencyException sce)
            {
                LOGGER.Error(sce.Message);
                LastErrorMessage = sce.Message;
                return false;
            }
        }

        private void MarkPlaceholdersDirty()
        {
            lock (SERIES_LOCK)
            {
                // anything with a srv_lastupdated of 0 should be marked as dirty
                // typically, this'll be placeholder cachedSeries
                foreach (CachedSeriesInfo? ser in Series.Values.Where(ser => ser.SrvLastUpdated == 0 || ser.Episodes.Count == 0))
                {
                    ser.Dirty = true;
                }
            }
        }

        public void UpdatesDoneOk()
        {
            //No Need to do anything as we always refresh from scratch
        }

        public CachedSeriesInfo GetSeries(string showName, bool showErrorMsgBox, Locale language)
        {
            throw new NotImplementedException(); //todo - (BulkAdd Manager needs to work for new providers)
        }

        public CachedSeriesInfo? GetSeries(int id)
        {
            lock (SERIES_LOCK)
            {
                return HasSeries(id) ? Series[id] : null;
            }
        }

        public override void Search(string text, bool showErrorMsgBox, MediaConfiguration.MediaType type,
            Locale preferredLocale)
        {
            if (type == MediaConfiguration.MediaType.tv)
            {
                List<CachedSeriesInfo>? results = API.ShowSearch(text).ToList();
                LOGGER.Info($"Got {results.Count:N0} results searching for {text} on TVMaze");

                foreach (CachedSeriesInfo result in results)
                {
                    LOGGER.Info($"   Movie: {result.Name}:{result.TvMazeCode}   {result.Popularity}");
                    AddSeriesToCache(result);
                }
            }
        }

        public bool HasSeries(int id)
        {
            lock (SERIES_LOCK)
            {
                return Series.ContainsKey(id);
            }
        }

        public void AddOrUpdateEpisode(Episode e)
        {
            lock (SERIES_LOCK)
            {
                if (!Series.ContainsKey(e.SeriesId))
                {
                    throw new SourceConsistencyException(
                        $"Can't find the cachedSeries to add the episode to (TVMaze). EpId:{e.EpisodeId} SeriesId:{e.SeriesId} {e.Name}", TVDoc.ProviderType.TVmaze);
                }

                CachedSeriesInfo ser = Series[e.SeriesId];

                ser.AddEpisode(e);
            }
        }

        public void Tidy(IEnumerable<ShowConfiguration> libraryValues)
        {
            // remove any shows from thetvdb that aren't in My Shows
            List<int> removeList = new List<int>();

            lock (SERIES_LOCK)
            {
                foreach (KeyValuePair<int, CachedSeriesInfo> kvp in Series)
                {
                    if (libraryValues.All(si => si.TVmazeCode != kvp.Key))
                    {
                        removeList.Add(kvp.Key);
                    }
                }

                foreach (int i in removeList)
                {
                    ForgetShow(i);
                }
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

        public void ForgetShow(int id)
        {
            lock (SERIES_LOCK)
            {
                if (Series.ContainsKey(id))
                {
                    Series.TryRemove(id, out _);
                }
            }
        }

        public void ForgetShow(ISeriesSpecifier ss)
        {
            lock (SERIES_LOCK)
            {
                if (Series.ContainsKey(ss.TvMazeId))
                {
                    Series.TryRemove(ss.TvMazeId, out CachedSeriesInfo _);
                    AddPlaceholderSeries(ss);
                }
                else
                {
                    if (ss.TvMazeId > 0)
                    {
                        AddPlaceholderSeries(ss);
                    }
                }
            }
        }

        private void AddPlaceholderSeries(ISeriesSpecifier ss)
        {
            lock (SERIES_LOCK)
            {
                Series[ss.TvMazeId] = new CachedSeriesInfo(ss.TvdbId, ss.TvMazeId, ss.TmdbId, ss.TargetLocale) { Dirty = true };
            }
        }

        public void UpdateSeries(CachedSeriesInfo si)
        {
            lock (SERIES_LOCK)
            {
                Series[si.TvMazeCode] = si;
            }
        }

        public void AddOrUpdateEpisode(Episode e, CachedSeriesInfo si)
        {
            lock (SERIES_LOCK)
            {
                if (!Series.ContainsKey(e.SeriesId))
                {
                    throw new SourceConsistencyException(
                        $"Can't find the cachedSeries to add the episode to. EpId:{e.EpisodeId} SeriesId:{e.SeriesId} {e.Name}", TVDoc.ProviderType.TVmaze);
                }

                CachedSeriesInfo ser = Series[e.SeriesId];
                ser.AddEpisode(e);
            }
        }

        public void AddBanners(int seriesId, IEnumerable<Banner> seriesBanners)
        {
            lock (SERIES_LOCK)
            {
                if (Series.ContainsKey(seriesId))
                {
                    foreach (Banner b in seriesBanners)
                    {
                        if (!Series.ContainsKey(b.SeriesId))
                        {
                            throw new SourceConsistencyException(
                                $"Can't find the cachedSeries to add the banner {b.BannerId} to. {seriesId},{b.SeriesId}", TVDoc.ProviderType.TVmaze);
                        }

                        CachedSeriesInfo ser = Series[b.SeriesId];

                        ser.AddOrUpdateBanner(b);
                    }

                    Series[seriesId].BannersLoaded = true;
                }
                else
                {
                    LOGGER.Warn($"Banners were found for cachedSeries {seriesId} - Ignoring them.");
                }
            }
        }

        public void LatestUpdateTimeIs(string time)
        {
            //No Need to do anything aswe always refresh from scratch
        }

        public override TVDoc.ProviderType Provider() => TVDoc.ProviderType.TVmaze;

        public ConcurrentDictionary<int, CachedSeriesInfo> CachedData
        {
            get
            {
                lock (SERIES_LOCK)
                {
                    return Series;
                }
            }
        }

        public void ReConnect(bool b)
        {
            //nothing to be done here
        }
    }
}
