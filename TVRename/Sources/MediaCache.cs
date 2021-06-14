using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Concurrent;

namespace TVRename
{
    public abstract class MediaCache
    {
        protected FileInfo CacheFile;

        // ReSharper disable once InconsistentNaming
        public string? CurrentDLTask;

        public string LastErrorMessage { get; set; }

        public bool LoadOk;

        public abstract TVDoc.ProviderType Provider();

        protected static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();

        // ReSharper disable once InconsistentNaming
        public readonly object MOVIE_LOCK = new object();

        protected readonly ConcurrentDictionary<int, CachedMovieInfo> Movies = new ConcurrentDictionary<int, CachedMovieInfo>();

        // ReSharper disable once InconsistentNaming
        public readonly object SERIES_LOCK = new object();

        protected readonly ConcurrentDictionary<int, CachedSeriesInfo> Series = new ConcurrentDictionary<int, CachedSeriesInfo>();

        private ConcurrentDictionary<int, int> forceReloadOn = new ConcurrentDictionary<int, int>();
        protected bool DoWeForceReloadFor(int code)
        {
            return forceReloadOn.ContainsKey(code) || !Series.ContainsKey(code);
        }

        protected void HaveReloaded(int code)
        {
            forceReloadOn.TryRemove(code, out _);
        }
        public void NeedToReload(int code)
        {
            forceReloadOn.TryAdd(code, code);
        }
        public abstract Language? PreferredLanguage();

        protected bool IsConnected { get; set; }

        public abstract bool EnsureUpdated(ISeriesSpecifier s, bool bannersToo, bool showErrorMsgBox);

        protected void SayNothing() => Say(null);

        protected void Say(string? s)
        {
            CurrentDLTask = s;
            if (s.HasValue())
            {
                LOGGER.Info("Status on screen updated: {0}", s);
            }
        }

        public ConcurrentDictionary<int, CachedMovieInfo> CachedMovieData
        {
            get
            {
                lock (MOVIE_LOCK)
                {
                    return Movies;
                }
            }
        }

        public ConcurrentDictionary<int, CachedSeriesInfo> CachedShowData
        {
            get
            {
                lock (SERIES_LOCK)
                {
                    return Series;
                }
            }
        }

        public CachedSeriesInfo? GetSeries(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }
            lock (SERIES_LOCK)
            {
                return HasSeries(id.Value) ? Series[id.Value] : null;
            }
        }

        public bool HasSeries(int id)
        {
            lock (SERIES_LOCK)
            {
                return Series.ContainsKey(id);
            }
        }

        public bool HasMovie(int id)
        {
            lock (MOVIE_LOCK)
            {
                return Movies.ContainsKey(id);
            }
        }

        public CachedMovieInfo? GetMovie(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            lock (MOVIE_LOCK)
            {
                return HasMovie(id.Value) ? Movies[id.Value] : null;
            }
        }

        public CachedMediaInfo? GetMedia(int code, MediaConfiguration.MediaType type)
        {
            return type switch
            {
                MediaConfiguration.MediaType.tv => GetSeries(code),
                MediaConfiguration.MediaType.movie => GetMovie(code),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public abstract void Search(string text, bool showErrorMsgBox, MediaConfiguration.MediaType type, Locale locale);
        public abstract int PrimaryKey(ISeriesSpecifier ss);
        public abstract string CacheSourceName();
    }
}
