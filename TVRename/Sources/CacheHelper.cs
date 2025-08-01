using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename;

public static class CacheHelper
{
    public static void Tidy<T>(this T cache, IEnumerable<ShowConfiguration> libraryValues) where T : MediaCache, iTVSource
    {
        // remove any shows from cache that aren't in My Shows
        List<ShowConfiguration> showConfigurations = libraryValues.ToList();

        lock (cache.SERIES_LOCK)
        {
            List<int> removeList = cache.CachedShowData.Keys.Where(id => showConfigurations.All(si => cache.PrimaryKey(si) != id)).ToList();

            foreach (int i in removeList)
            {
                cache.ForgetShow(i);
            }
        }
    }

    public static void Tidy<T>(this T cache, IEnumerable<MediaConfiguration> libraryValues) where T : MediaCache, iMovieSource
    {
        // remove any shows from cache that aren't in My Movies
        List<MediaConfiguration> movieConfigurations = libraryValues.ToList();

        lock (cache.MOVIE_LOCK)
        {
            List<int> removeList = cache.CachedMovieData.Keys.Where(id => movieConfigurations.All(si => cache.PrimaryKey(si) != id)).ToList();

            foreach (int i in removeList)
            {
                cache.ForgetMovie(i);
            }
        }
    }

    public static void ForgetShow<T>(this T cache, ISeriesSpecifier ss) where T : MediaCache, iTVSource
    {
        cache.ForgetShow(cache.PrimaryKey(ss));
        if (cache.PrimaryKey(ss) <= 0)
        {
            return;
        }
        lock (cache.SERIES_LOCK)
        {
            cache.AddPlaceholderSeries(ss);

            cache.NeedToReload(cache.PrimaryKey(ss));
        }
    }

    public static void ForgetShow<T>(this T cache, int id) where T : MediaCache, iTVSource
    {
        lock (cache.SERIES_LOCK)
        {
            if (cache.CachedShowData.ContainsKey(id))
            {
                cache.CachedShowData.TryRemove(id, out _);
            }
        }
    }

    public static void AddPlaceholderSeries<T>(this T cache, ISeriesSpecifier ss) where T : MediaCache, iTVSource
    {
        lock (cache.SERIES_LOCK)
        {
            cache.CachedShowData[cache.PrimaryKey(ss)] =
                new CachedSeriesInfo(ss.TvdbId, ss.TvMazeId, ss.TmdbId, ss.TargetLocale, cache.SourceProvider())
                { Dirty = true };
        }
    }

    public static void AddPlaceholderMovie<T>(this T cache, ISeriesSpecifier ss) where T : MediaCache, iMovieSource
    {
        lock (cache.MOVIE_LOCK)
        {
            cache.CachedMovieData[cache.PrimaryKey(ss)] =
                new CachedMovieInfo(ss.TvdbId, ss.TvMazeId, ss.TmdbId, ss.TargetLocale, cache.SourceProvider())
                { Dirty = true };
        }
    }

    public static void AddMovieToCache<T>(this T cache, CachedMovieInfo si) where T : MediaCache, iMovieSource
    {
        int id = cache.PrimaryKey(si);
        lock (cache.MOVIE_LOCK)
        {
            if (cache.CachedMovieData.TryGetValue(id, out CachedMovieInfo? oldMovieInfo))
            {
                oldMovieInfo.Merge(si);
            }
            else
            {
                cache.CachedMovieData[id] = si;
            }
        }
    }

    public static void AddSeriesToCache<T>(this T cache, CachedSeriesInfo si) where T : MediaCache, iTVSource
    {
        int id = cache.PrimaryKey(si);
        lock (cache.SERIES_LOCK)
        {
            if (cache.CachedShowData.TryGetValue(id, out CachedSeriesInfo? oldSeriesInfo))
            {
                oldSeriesInfo.Merge(si);
            }
            else
            {
                cache.CachedShowData[id] = si;
            }
        }
    }

    public static void ForgetMovie<T>(this T cache, int id) where T : MediaCache, iMovieSource
    {
        lock (cache.MOVIE_LOCK)
        {
            if (cache.CachedMovieData.ContainsKey(id))
            {
                cache.CachedMovieData.TryRemove(id, out _);
            }
        }
    }

    public static void ForgetMovie<T>(this T cache, ISeriesSpecifier si) where T : MediaCache, iMovieSource
    {
        cache.ForgetMovie(cache.PrimaryKey(si));
        lock (cache.MOVIE_LOCK)
        {
            if (cache.PrimaryKey(si) > 0)
            {
                cache.AddPlaceholderMovie(si);
            }
        }
    }

    public static void MarkPlaceHoldersDirty<T>(this T cache, IEnumerable<ISeriesSpecifier> ss) where T : MediaCache, iMovieSource, iTVSource
    {
        foreach (ISeriesSpecifier downloadShow in ss)
        {
            if (downloadShow.Media == MediaConfiguration.MediaType.tv)
            {
                if (!cache.HasSeries(cache.PrimaryKey(downloadShow)))
                {
                    cache.AddPlaceholderSeries(downloadShow);
                }
                else
                {
                    CachedSeriesInfo? show = cache.GetSeries(cache.PrimaryKey(downloadShow));
                    show?.UpgradeSearchResultToDirty();
                }
            }
            else
            {
                if (!cache.HasMovie(cache.PrimaryKey(downloadShow)))
                {
                    cache.AddPlaceholderMovie(downloadShow);
                }
                else
                {
                    CachedMovieInfo? movie = cache.GetMovie(cache.PrimaryKey(downloadShow));
                    movie?.UpgradeSearchResultToDirty();
                }
            }
        }
    }

    public static void MarkAllDirty(this MediaCache cache)
    {
        lock (cache.MOVIE_LOCK)
        {
            foreach (CachedMovieInfo m in cache.CachedMovieData.Values)
            {
                m.Dirty = true;
            }
        }
        lock (cache.SERIES_LOCK)
        {
            foreach (CachedSeriesInfo m in cache.CachedShowData.Values)
            {
                m.Dirty = true;
            }
        }
    }

    public static void MarkPlaceholdersDirty(this MediaCache cache)
    {
        lock (cache.MOVIE_LOCK)
        {
            // anything with a srv_lastupdated of 0 should be marked as dirty
            // typically, this'll be placeholder cachedSeries
            foreach (CachedMovieInfo ser in cache.CachedMovieData.Values.Where(ser => ser.SrvLastUpdated == 0))
            {
                ser.Dirty = true;
            }
        }
        lock (cache.SERIES_LOCK)
        {
            // anything with a srv_lastupdated of 0 should be marked as dirty
            // typically, this'll be placeholder cachedSeries
            foreach (CachedSeriesInfo? ser in cache.CachedShowData.Values.Where(ser => ser.SrvLastUpdated == 0 || ser.Episodes.Count == 0 || ser.Episodes.Any(e => e.Name.IsPlaceholderName() && e.HasAired())))
            {
                ser.Dirty = true;

                foreach (Episode ep in ser.Episodes.Where(e => e.Name.IsPlaceholderName() && e.HasAired()))
                {
                    ep.Dirty = true;
                }
            }
        }
    }

    public static Dictionary<int, CachedSeriesInfo> GetSeriesDictMatching<T>(this T cache, string testShowName) where T : MediaCache, iTVSource
    {
        Dictionary<int, CachedSeriesInfo> matchingSeries = [];

        testShowName = testShowName.CompareName();

        if (string.IsNullOrEmpty(testShowName))
        {
            return matchingSeries;
        }

        foreach (KeyValuePair<int, CachedSeriesInfo> kvp in cache.CachedShowData)
        {
            string show = kvp.Value.Name.CompareName();

            if (show.Contains(testShowName, StringComparison.InvariantCultureIgnoreCase))
            {
                //We have a match
                matchingSeries.Add(kvp.Key, kvp.Value);
            }
        }

        return matchingSeries;
    }

    public static Dictionary<int, CachedMovieInfo> GetMoviesDictMatching<T>(this T cache, string testShowName) where T : MediaCache, iMovieSource
    {
        Dictionary<int, CachedMovieInfo> matchingSeries = [];

        testShowName = testShowName.CompareName();

        if (string.IsNullOrEmpty(testShowName))
        {
            return matchingSeries;
        }

        lock (cache.MOVIE_LOCK)
        {
            foreach (KeyValuePair<int, CachedMovieInfo> kvp in cache.CachedMovieData)
            {
                string show = kvp.Value.Name.CompareName();

                if (show.Contains(testShowName, StringComparison.InvariantCultureIgnoreCase))
                {
                    //We have a match
                    matchingSeries.Add(kvp.Key, kvp.Value);
                }
            }
        }
        return matchingSeries;
    }

    public static CachedSeriesInfo? GetSeries<T>(this T cache, string showName, bool showErrorMsgBox, Locale preferredLocale) where T : MediaCache, iTVSource
    {
        cache.Search(showName, showErrorMsgBox, MediaConfiguration.MediaType.tv, preferredLocale);

        if (string.IsNullOrEmpty(showName))
        {
            return null;
        }

        showName = showName.ToLower();

        List<CachedSeriesInfo> matchingShows = cache.GetSeriesDictMatching(showName).Values.ToList();

        return matchingShows.Count switch
        {
            0 => null,
            1 => matchingShows.First(),
            _ => null
        };
    }
    public static CachedMovieInfo? GetMovie<T>(this T cache, string hint, int? possibleYear, Locale preferredLocale, bool showErrorMsgBox, bool useMostPopularMatch) where T : MediaCache, iMovieSource
    {
        cache.Search(hint, showErrorMsgBox, MediaConfiguration.MediaType.movie, preferredLocale);

        if (string.IsNullOrEmpty(hint))
        {
            return null;
        }

        string showName = hint.ToLower();

        List<CachedMovieInfo> matchingShows = cache.GetMoviesDictMatching(showName).Values.ToList();

        if (matchingShows.Count == 0)
        {
            return null;
        }

        if (matchingShows.Count == 1)
        {
            return matchingShows.First();
        }

        List<CachedMovieInfo> exactMatchingShows = matchingShows
            .Where(info => info.Name.CompareName().Equals(showName, StringComparison.InvariantCultureIgnoreCase)).ToList();

        if (exactMatchingShows.Count == 0 && !useMostPopularMatch)
        {
            return null;
        }

        if (exactMatchingShows.Count == 1)
        {
            return exactMatchingShows.First();
        }

        if (possibleYear is null && !useMostPopularMatch)
        {
            return null;
        }

        if (possibleYear != null)
        {
            List<CachedMovieInfo> exactMatchingShowsWithYear = exactMatchingShows
                .Where(info => info.Year == possibleYear).ToList();

            if (exactMatchingShowsWithYear.Count == 0 && !useMostPopularMatch)
            {
                return null;
            }

            if (exactMatchingShowsWithYear.Count == 1)
            {
                return exactMatchingShowsWithYear.First();
            }
        }
        if (!useMostPopularMatch)
        {
            return null;
        }

        if (matchingShows.All(s => s.Popularity.HasValue))
        {
            return matchingShows.OrderByDescending(s => s.Popularity).First();
        }
        return null;
    }
}
