using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TVRename;

public abstract class MediaCache
{
    protected FileInfo? CacheFile;

    // ReSharper disable once InconsistentNaming
    public string? CurrentDLTask;

    public string? LastErrorMessage { get; set; }

    public bool LoadOk;

    public abstract TVDoc.ProviderType Provider();

    protected static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();

    // ReSharper disable once InconsistentNaming
    public readonly object MOVIE_LOCK = new();

    protected readonly ConcurrentDictionary<int, CachedMovieInfo> Movies = new();

    protected List<CachedMovieInfo> FullMovies()
    {
        lock (MOVIE_LOCK)
        {
            return [.. Movies.Values.Where(info => !info.IsSearchResultOnly).OrderBy(s => s.Name)];
        }
    }

    // ReSharper disable once InconsistentNaming
    public readonly object SERIES_LOCK = new();

    protected readonly ConcurrentDictionary<int, CachedSeriesInfo> Series = new();

    protected List<CachedSeriesInfo> FullShows()
    {
        lock (SERIES_LOCK)
        {
            return [.. Series.Values.Where(info => !info.IsSearchResultOnly).OrderBy(s => s.Name)];
        }
    }

    private readonly ConcurrentDictionary<int, int> forceReloadOn = new();
    protected bool DoWeForceReloadFor(int code) => forceReloadOn.ContainsKey(code) || !HasSeries(code);

    protected void HaveReloaded(int code)
    {
        forceReloadOn.TryRemove(code, out _);
    }
    public void NeedToReload(int code)
    {
        try
        {
            forceReloadOn.TryAdd(code, code);
        }
        catch (OverflowException ex)
        {
            LOGGER.Warn($"Could not add {code} to the list of items to reload, so ignoring for now. If this happens consistenly contact the developer", ex);
        }
    }

    public abstract bool EnsureUpdated(ISeriesSpecifier s, bool bannersToo, bool showErrorMsgBox);

    protected void SayNothing() => Say(null);

    protected void Say(string? s)
    {
        CurrentDLTask = s;
        if (s.HasValue())
        {
            LOGGER.Info($"Status on screen updated: {s}");
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
            return Series.TryGetValue(id.Value, out CachedSeriesInfo? si) ? si : null;
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
            return Movies.TryGetValue(id.Value, out CachedMovieInfo? mi) ? mi : null;
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
    public abstract void ReConnect(bool b);
    public abstract bool GetUpdates(List<ISeriesSpecifier> ss, bool showErrorMsgBox, CancellationToken cts);
}
