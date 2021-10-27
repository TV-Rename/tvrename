//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using JetBrains.Annotations;
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
        private static readonly object SyncRoot = new();

        [NotNull]
        public static LocalCache Instance
        {
            get
            {
                if (InternalInstance is null)
                {
                    lock (SyncRoot)
                    {
                        if (InternalInstance is null)
                        {
                            InternalInstance = new LocalCache();
                        }
                    }
                }

                return InternalInstance;
            }
        }

        public override int PrimaryKey([NotNull] ISeriesSpecifier ss) => ss.TvMazeId;

        [NotNull]
        public override string CacheSourceName() => "TVMaze";
        public void Setup(FileInfo? loadFrom, [NotNull] FileInfo cache, bool showIssues)
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

        public override bool EnsureUpdated([NotNull] ISeriesSpecifier s, bool bannersToo, bool showErrorMsgBox)
        {
            if (s.Provider != TVDoc.ProviderType.TVmaze)
            {
                throw new SourceConsistencyException(
                    $"Asked to update {s.Name} from TV Maze, but the Id is not for TV maze.",
                    TVDoc.ProviderType.TVmaze);
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

                this.AddSeriesToCache(downloadedSi);
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

        public bool GetUpdates(bool showErrorMsgBox, CancellationToken cts, [NotNull] IEnumerable<ISeriesSpecifier> ss)
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
                            CachedSeriesInfo x = GetSeries(showId);
                            if (!(x is null))
                            {
                                if (x.SrvLastUpdated < showUpdateTime.Value)
                                {
                                    LOGGER.Info(
                                        $"Identified that show with TVMaze Id {showId} {x.Name} should be updated as update time is now {showUpdateTime.Value} and cache has {x.SrvLastUpdated}. ie {Helpers.FromUnixTime(showUpdateTime.Value).ToLocalTime()} to {Helpers.FromUnixTime(x.SrvLastUpdated).ToLocalTime()}.");

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
                return false;
            }
            catch (SourceConsistencyException sce)
            {
                LOGGER.Error(sce.Message);
                LastErrorMessage = sce.Message;
                return false;
            }
        }

        public void UpdatesDoneOk()
        {
            //No Need to do anything as we always refresh from scratch
        }

        public CachedSeriesInfo? GetSeries(string showName, bool showErrorMsgBox, Locale preferredLocale)
        {
            Search(showName, showErrorMsgBox, MediaConfiguration.MediaType.tv, preferredLocale);

            if (string.IsNullOrEmpty(showName))
            {
                return null;
            }

            showName = showName.ToLower();

            List<CachedSeriesInfo> matchingShows = this.GetSeriesDictMatching(showName).Values.ToList();

            return matchingShows.Count switch
            {
                0 => null,
                1 => matchingShows.First(),
                _ => null
            };
        }

        public override void Search(string text, bool showErrorMsgBox, MediaConfiguration.MediaType type,
            Locale preferredLocale)
        {
            if (type == MediaConfiguration.MediaType.tv)
            {
                bool isNumber = System.Text.RegularExpressions.Regex.Match(text, "^[0-9]+$").Success;

                if (isNumber)
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

                List<CachedSeriesInfo> results = API.ShowSearch(text).ToList();
                LOGGER.Info($"Got {results.Count:N0} results searching for {text} on TVMaze");

                foreach (CachedSeriesInfo result in results)
                {
                    LOGGER.Info($"   Movie: {result.Name}:{result.TvMazeCode}   {result.Popularity}");
                    this.AddSeriesToCache(result);
                }
            }
        }

        public void AddOrUpdateEpisode([NotNull] Episode e)
        {
            lock (SERIES_LOCK)
            {
                if (!Series.ContainsKey(e.SeriesId))
                {
                    throw new SourceConsistencyException(
                        $"Can't find the cachedSeries to add the episode to (TVMaze). EpId:{e.EpisodeId} SeriesId:{e.SeriesId} {e.Name}",
                        TVDoc.ProviderType.TVmaze);
                }

                CachedSeriesInfo ser = Series[e.SeriesId];

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

        public void AddOrUpdateEpisode([NotNull] Episode e, CachedSeriesInfo si)
        {
            lock (SERIES_LOCK)
            {
                if (!Series.ContainsKey(e.SeriesId))
                {
                    throw new SourceConsistencyException(
                        $"Can't find the cachedSeries to add the episode to. EpId:{e.EpisodeId} SeriesId:{e.SeriesId} {e.Name}",
                        TVDoc.ProviderType.TVmaze);
                }

                CachedSeriesInfo ser = Series[e.SeriesId];
                ser.AddEpisode(e);
            }
        }

        public void LatestUpdateTimeIs(string time)
        {
            //No Need to do anything aswe always refresh from scratch
        }

        public override TVDoc.ProviderType Provider() => TVDoc.ProviderType.TVmaze;
        TVDoc.ProviderType iTVSource.SourceProvider() => TVDoc.ProviderType.TVmaze;

        public void ReConnect(bool b)
        {
            //nothing to be done here
        }
    }
}
