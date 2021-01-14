using System;
using System.Collections.Concurrent;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    public abstract class MediaCache
    {

        protected FileInfo CacheFile;
        // ReSharper disable once InconsistentNaming
        public string? CurrentDLTask;

        public string LastErrorMessage { get; set; }

        public bool LoadOk;

        
        protected static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();


        // ReSharper disable once InconsistentNaming
        public readonly object LANGUAGE_LOCK = new object();
        public Languages? LanguageList;

        // ReSharper disable once InconsistentNaming
        public readonly object MOVIE_LOCK = new object();
        protected readonly ConcurrentDictionary<int, CachedMovieInfo> Movies = new ConcurrentDictionary<int, CachedMovieInfo>();

        // ReSharper disable once InconsistentNaming
        public readonly object SERIES_LOCK = new object();
        protected readonly ConcurrentDictionary<int, CachedSeriesInfo> Series = new ConcurrentDictionary<int, CachedSeriesInfo>();

        public Language? PreferredLanguage =>
            LanguageList?.GetLanguageFromCode(TVSettings.Instance.PreferredLanguageCode);

        public Language? GetLanguageFromCode(string customLanguageCode) => LanguageList?.GetLanguageFromCode(customLanguageCode);

        public bool IsConnected { get; protected set; }

        protected void SayNothing() => Say(null);
        protected void Say(string? s)
        {
            CurrentDLTask = s;
            if (s.HasValue())
            {
                LOGGER.Info("Status on screen updated: {0}", s);
            }
        }


        public bool HasSeries(int id) => Series.ContainsKey(id);

        public CachedSeriesInfo? GetSeries(int id) => HasSeries(id) ? Series[id] : null;

        public bool HasMovie(int id) => Movies.ContainsKey(id);

        public CachedMovieInfo? GetMovie(int id) => HasMovie(id) ? Movies[id] : null;

        public CachedMediaInfo? GetMedia(int code, MediaConfiguration.MediaType type)
        {
            return type switch
            {
                MediaConfiguration.MediaType.tv => GetSeries(code),
                MediaConfiguration.MediaType.movie => GetMovie(code),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public abstract void Search(string text, bool showErrorMsgBox);
    }
}
