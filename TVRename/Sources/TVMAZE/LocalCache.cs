// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

// Talk to the TVmaze web API, and get tv series info

// Hierarchy is:
//   TVmaze -> Series (class SeriesInfo) -> Seasons (class Season) -> Episodes (class Episode)

namespace TVRename.TVmaze
{
    // ReSharper disable once InconsistentNaming
    public class LocalCache : iTVSource
    {
        private FileInfo cacheFile;

        public static readonly object SERIES_LOCK = new object();

        private readonly ConcurrentDictionary<int, SeriesInfo> series = new ConcurrentDictionary<int, SeriesInfo>();

        // ReSharper disable once InconsistentNaming
        public string CurrentDLTask;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

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

        public string LastErrorMessage { get; set; }

        public bool LoadOk;

        public void Setup(FileInfo? loadFrom, FileInfo cache, CommandLineArgs cla)
        {
            System.Diagnostics.Debug.Assert(cache != null);
            cacheFile = cache;

            LastErrorMessage = string.Empty;

            LoadOk = loadFrom is null || CachePersistor.LoadCache(loadFrom, this);
        }

        public bool Connect(bool showErrorMsgBox) => true;

        public void SaveCache()
        {
            lock (SERIES_LOCK)
            {
                CachePersistor.SaveCache(series, cacheFile, 0);
            }
        }

        public bool EnsureUpdated(SeriesSpecifier s, bool bannersToo, bool showErrorMsgBox)
        {
            if (s.Provider != ShowItem.ProviderType.TVmaze)
            {
                throw new SourceConsistencyException($"Asked to update {s.Name} from TV Maze, but the Id is not for TV maze.", ShowItem.ProviderType.TVmaze);
            }

            lock (SERIES_LOCK)
            {
                if (series.ContainsKey(s.TvMazeSeriesId) && !series[s.TvMazeSeriesId].Dirty)
                {
                    return true;
                }
            }

            Say($"{s.Name} from TVmaze");
            try
            {
                SeriesInfo downloadedSi = API.GetSeriesDetails(s);

                if (downloadedSi.TvMazeCode != s.TvMazeSeriesId && s.TvMazeSeriesId ==-1)
                {
                    lock (SERIES_LOCK)
                    {
                        series.TryRemove(-1, out _);
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
                Logger.Error(sce.Message);
                LastErrorMessage = sce.Message;
                return true;
            }

            return true;
        }

        private void AddSeriesToCache([NotNull] SeriesInfo si)
        {
            int id = si.TvMazeCode;
            lock (SERIES_LOCK)
            {
                if (series.ContainsKey(id))
                {
                    series[id].Merge(si);
                }
                else
                {
                    series[id] = si;
                }
            }
        }

        public bool GetUpdates(bool showErrorMsgBox, CancellationToken cts, IEnumerable<SeriesSpecifier> ss)
        {
            Say("Validating TVmaze cache");
            foreach (SeriesSpecifier downloadShow in ss.Where(downloadShow => !HasSeries(downloadShow.TvMazeSeriesId)))
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
                            SeriesInfo x = GetSeries(showId);
                            if (!(x is null))
                            {
                                if (x.SrvLastUpdated < showUpdateTime.Value)
                                {
                                    Logger.Info($"Identified that show with TVMaze Id {showId} {x.Name} should be updated as update time is now {showUpdateTime.Value} and cache has {x.SrvLastUpdated}. ie {Helpers.FromUnixTime(showUpdateTime.Value).ToLocalTime()} to {Helpers.FromUnixTime(x.SrvLastUpdated).ToLocalTime()}.");
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
                Logger.Error(sce.Message);
                LastErrorMessage = sce.Message;
                return false;
            }
        }

        private void MarkPlaceholdersDirty()
        {
            lock (SERIES_LOCK)
            {
                // anything with a srv_lastupdated of 0 should be marked as dirty
                // typically, this'll be placeholder series
                foreach (KeyValuePair<int, SeriesInfo> kvp in series)
                {
                    SeriesInfo ser = kvp.Value;
                    if (ser.SrvLastUpdated == 0 || ser.Episodes.Count == 0)
                    {
                        ser.Dirty = true;
                    }
                }
            }
        }

        private void AddPlaceholderSeries([NotNull] SeriesSpecifier ss)
            => AddPlaceholderSeries(ss.TvdbSeriesId, ss.TvMazeSeriesId, ss.CustomLanguageCode);

        private void SayNothing() => Say(string.Empty);
        private void Say(string s)
        {
            CurrentDLTask = s;
            if (s.HasValue())
            {
                Logger.Info("Status on screen updated: {0}", s);
            }
        }

        public void UpdatesDoneOk()
        {
            //No Need to do anything aswe always refresh from scratch
        }

        public SeriesInfo? GetSeries(string showName, bool showErrorMsgBox) => throw new NotImplementedException(); //todo when we can offer sarch for TV Maze

        public SeriesInfo? GetSeries(int id)
        {
            lock (SERIES_LOCK)
            {
                return HasSeries(id) ? series[id] : null;
            }
        }

        public bool HasSeries(int id)
        {
            lock (SERIES_LOCK)
            {
                return series.ContainsKey(id);
            }
        }

        public void AddOrUpdateEpisode(Episode e)
        {
            lock (SERIES_LOCK)
            {
                if (!series.ContainsKey(e.SeriesId))
                {
                    throw new SourceConsistencyException(
                        $"Can't find the series to add the episode to (TVMaze). EpId:{e.EpisodeId} SeriesId:{e.SeriesId} {e.Name}", ShowItem.ProviderType.TVmaze);
                }

                SeriesInfo ser = series[e.SeriesId];

                ser.AddEpisode(e);
            }
        }
        public void Tidy(ICollection<ShowItem> libraryValues)
        {
            // remove any shows from thetvdb that aren't in My Shows
            List<int> removeList = new List<int>();

            lock (SERIES_LOCK)
            {
                foreach (KeyValuePair<int, SeriesInfo> kvp in series)
                {
                    bool found = libraryValues.Any(si => si.TVmazeCode == kvp.Key);
                    if (!found)
                    {
                        removeList.Add(kvp.Key);
                    }
                }

                foreach (int i in removeList)
                {
                    ForgetShow(i);
                }
            }

            SaveCache();
        }

        public void ForgetEverything()
        {
            lock (SERIES_LOCK)
            {
                series.Clear();
            }

            SaveCache();
            Logger.Info("Forgot all TVMaze shows");
        }

        public void ForgetShow(int id)
        {
            lock (SERIES_LOCK)
            {
                if (series.ContainsKey(id))
                {
                    series.TryRemove(id, out _);
                }
            }
        }

        public void ForgetShow(int tvdb,int tvmaze, bool makePlaceholder, bool useCustomLanguage, string? langCode)
        {
            lock (SERIES_LOCK)
            {
                if (series.ContainsKey(tvmaze))
                {
                    series.TryRemove(tvmaze, out SeriesInfo _);
                    if (makePlaceholder)
                    {
                        if (useCustomLanguage && langCode.HasValue())
                        {
                            AddPlaceholderSeries(tvdb,tvmaze, langCode!);
                        }
                        else
                        {
                            AddPlaceholderSeries(tvdb, tvmaze);
                        }
                    }
                }
                else
                {
                    if (tvmaze > 0 && makePlaceholder)
                    {
                        AddPlaceholderSeries(tvdb, tvmaze);
                    }
                }
            }
        }

        private void AddPlaceholderSeries(int tvdb, int tvmaze) 
        {
            lock (SERIES_LOCK)
            {
                series[tvmaze] = new SeriesInfo(tvdb,tvmaze) { Dirty = true };
            }
        }

        private void AddPlaceholderSeries(int tvdb, int tvmaze, string customLanguageCode)
        {
            lock (SERIES_LOCK)
            {
                series[tvmaze] = new SeriesInfo(tvdb, tvmaze, customLanguageCode) {Dirty = true};
            }
        }

        public void UpdateSeries(SeriesInfo si)
        {
            lock (SERIES_LOCK)
            {
                series[si.TvMazeCode] = si;
            }
        }

        public void AddOrUpdateEpisode(Episode e,SeriesInfo si)
        {
            lock (SERIES_LOCK)
            {
                if (!series.ContainsKey(e.SeriesId))
                {
                    throw new SourceConsistencyException(
                        $"Can't find the series to add the episode to. EpId:{e.EpisodeId} SeriesId:{e.SeriesId} {e.Name}", ShowItem.ProviderType.TVmaze);
                }

                SeriesInfo ser = series[e.SeriesId];
                ser.AddEpisode(e);
            }
        }

        public void AddBanners(int seriesId, IEnumerable<Banner> seriesBanners)
        {
            lock (SERIES_LOCK)
            {
                if (series.ContainsKey(seriesId))
                {
                    foreach (Banner b in seriesBanners)
                    {
                        if (!series.ContainsKey(b.SeriesId))
                        {
                            throw new SourceConsistencyException(
                                $"Can't find the series to add the banner {b.BannerId} to. {seriesId},{b.SeriesId}", ShowItem.ProviderType.TVmaze);
                        }

                        SeriesInfo ser = series[b.SeriesId];

                        ser.AddOrUpdateBanner(b);
                    }

                    series[seriesId].BannersLoaded = true;
                }
                else
                {
                    Logger.Warn($"Banners were found for series {seriesId} - Ignoring them.");
                }
            }
        }

        public void LatestUpdateTimeIs(string time)
        {
            //No Need to do anything aswe always refresh from scratch
        }

        public Language PreferredLanguage => throw new NotImplementedException();

        public ConcurrentDictionary<int,SeriesInfo> CachedData
        {
            get {
                lock (SERIES_LOCK)
                {
                    return series;
                }
            }
        }

        public Language GetLanguageFromCode(string customLanguageCode) => throw new NotImplementedException();
    }
}
