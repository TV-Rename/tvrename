//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Polly;
using TVRename;

namespace TVRename;

internal static class CachePersistor
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static string? LoadErr;

    private static void RotateCacheFiles(FileInfo cacheFile)
    {
        if (cacheFile.Exists)
        {
            double hours = 999.9;
            if (File.Exists(cacheFile.FullName + ".0"))
            {
                // see when the last rotate was, and only rotate if its been at least an hour since the last save
                DateTime dt = File.GetLastWriteTime(cacheFile.FullName + ".0");
                hours = TimeHelpers.LocalNow().Subtract(dt).TotalHours;
            }

            if (hours >= 24.0) // rotate the save file daily
            {
                FileHelper.Rotate(cacheFile.FullName);
            }
        }
    }
    public static async Task SaveCacheAsync(ConcurrentDictionary<int, CachedSeriesInfo> series, ConcurrentDictionary<int, CachedMovieInfo> movies, FileInfo cacheFile, long timestamp, bool isShuttingDown = false)
    {
        Policy retryPolicy = Policy
            .Handle<Exception>()
            .Retry(3, (exception, retryCount) =>
            {
                Logger.Warn(exception, $"Retry {retryCount}/3 to save {cacheFile.FullName}.");
            });

        try
        {
            await retryPolicy.Execute(async () => { await SaveCacheInternalAsync(series, movies, cacheFile, timestamp); });
        }
        catch (Exception e)
        {
            Logger.Error($"Complete failure to save {cacheFile.FullName}. {e.Message}");

            // Do not show dialog during shutdown to prevent hanging
            if (!isShuttingDown)
            {
                DialogResult dr = MessageBox.Show($"Could not save {cacheFile.Name} due to {e.Message}. Local cache will be lost. Retry? ", "TV Rename",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (dr != DialogResult.No)
                {
                    await SaveCacheAsync(series, movies, cacheFile, timestamp, isShuttingDown);
                }
            }
        }
    }
    private static async Task SaveCacheInternalAsync(ConcurrentDictionary<int, CachedSeriesInfo> series, ConcurrentDictionary<int, CachedMovieInfo> movies, FileInfo cacheFile, long timestamp)
    {
        DirectoryInfo di = cacheFile.Directory;
        if (!di.Exists)
        {
            di.Create();
        }

        Logger.Info($"Saving Cache to: {cacheFile.FullName}");
        try
        {
            RotateCacheFiles(cacheFile);
        }
        catch (System.IO.FileNotFoundException e)
        {
            Logger.Warn(e, $"Failed to rotate files for Cache to {cacheFile.FullName}");
        }
        catch (System.IO.IOException e)
        {
            Logger.Warn(e, $"Failed to rotate files for Cache to {cacheFile.FullName}");
        }
        catch (Exception e)
        {
            Logger.Warn(e, $"Failed to rotate files for Cache to {cacheFile.FullName}");
        }

        try
        {
            await SaveCacheFileInternalAsync(series, movies, cacheFile, timestamp);
        }
        catch (Exception e)
        {
            Logger.Warn(e, $"Failed to save Cache to {cacheFile.FullName}");
            throw;
        }
    }

    private static async Task SaveCacheFileInternalAsync(ConcurrentDictionary<int, CachedSeriesInfo> series, ConcurrentDictionary<int, CachedMovieInfo> movies, FileInfo cacheFile,
        long timestamp)
    {
        var cacheXml = GenerateCacheXML(series,movies,timestamp);
        await XmlHelper.SaveXmlFileAsync(cacheXml, cacheFile.FullName);
        Logger.Info($"Saved Cache to {cacheFile.FullName}");
    }

    private static XDocument GenerateCacheXML(ConcurrentDictionary<int, CachedSeriesInfo> series, ConcurrentDictionary<int, CachedMovieInfo> movies, long timestamp)
    {
        return new XDocument(
           new XDeclaration("1.0", "utf-8", "yes"), // XML Declaration
           new XElement("Data",                     // Root Element
               new XAttribute("time", timestamp),      // Attribute
               GenerateSeriesXElements(series), // <Series> elements
               GenerateMovieXElements(movies)   // <Movie> elements
                )
           );
    }

    private static List<XElement> GenerateMovieXElements(ConcurrentDictionary<int, CachedMovieInfo> movies)
    {
        var elements = new List<XElement>();
        foreach (KeyValuePair<int, CachedMovieInfo> kvp in movies)
        {
            if (!kvp.Value.IsSearchResultOnly)
            {
                elements.Add(kvp.Value.GenerateXElement());
            }
            else
            {
                Logger.Info(
                    $"Cannot save Movie {kvp.Key} ({kvp.Value.Name}) to cache as it is a search result that has not been used.");
            }
        }
        return elements;
    }

    private static List<XElement> GenerateSeriesXElements(ConcurrentDictionary<int, CachedSeriesInfo> series)
    {
        var elements = new List<XElement>();
        foreach (KeyValuePair<int, CachedSeriesInfo> kvp in series)
        {
            if (kvp.Value.SrvLastUpdated != 0)
            {
                elements.Add(kvp.Value.GenerateXElement());
            }
            else
            {
                Logger.Info(
                    $"Cannot save TV {kvp.Key} ({kvp.Value.Name}) to cache as it has not been updated at all.");
            }
        }
        return elements;
    }

    public static async Task<bool> LoadTvCacheAsync<T>(FileInfo loadFrom, T cache, CancellationToken token) where T : MediaCache, iTVSource
    {
        Logger.Info($"Loading Cache from: {loadFrom.FullName}");
        if (!loadFrom.Exists)
        {
            return true; // that's ok
        }

        try
        {
            await using var stream = new System.IO.FileStream(loadFrom.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read, bufferSize: 4096, useAsync: true);

            XElement x = await XElement.LoadAsync(stream, LoadOptions.None, token);
            bool r = ProcessSeriesXml(x, cache);
            if (r)
            {
                cache.UpdatesDoneOk();
            }

            return r;
        }
        catch (Exception e)
        {
            Logger.Warn(e, "Problem on Startup loading File");
            LoadErr = loadFrom.Name + " : " + e.Message;
            return false;
        }
    }

    public static async Task<bool> LoadMovieCacheAsync<T>(FileInfo loadFrom, T cache, CancellationToken token) where T : MediaCache, iMovieSource
    {
        Logger.Info($"Loading Cache from: {loadFrom.FullName}");
        if (!loadFrom.Exists)
        {
            return true; // that's ok
        }

        try
        {
            await using var stream = new System.IO.FileStream(loadFrom.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read, bufferSize: 4096, useAsync: true);

            XElement x = await XElement.LoadAsync(stream, LoadOptions.None, token);
            bool r = ProcessMovieXml(x, cache);
            if (r)
            {
                cache.UpdatesDoneOk();
            }

            return r;
        }
        catch (Exception e)
        {
            Logger.Warn(e, "Problem on Startup loading File");
            LoadErr = loadFrom.Name + " : " + e.Message;
            return false;
        }
    }

    private static bool ProcessMovieXml<T>(XElement x, T cache) where T : MediaCache, iMovieSource
    {
        try
        {
            string? time = x.Attribute("time")?.Value;
            if (time != null)
            {
                cache.LatestUpdateTimeIs(time);
            }
            else
            {
                Logger.Error("Could not obtain update time from XML");
            }

            foreach (CachedMovieInfo si in x.Descendants("Movie").Select(seriesXml => new CachedMovieInfo(seriesXml, cache.SourceProvider())))
            {
                // The <cachedSeries> returned by GetSeries have
                // less info than other results from
                // thetvdb.com, so we need to smartly merge
                // in a <Series> if we already have some/all
                // info on it (depending on which one came
                // first).
                cache.AddMovieToCache(si);
            }
        }
        catch (XmlException e)
        {
            string message = "Error processing data from Cache (top level).";
            message += "\r\n" + x;
            message += "\r\n" + e.ErrorText();

            Logger.Error(message);
            Logger.Error(x.ToString());
            throw new CacheLoadException(message, e);
        }
        return true;
    }

    private static bool ProcessSeriesXml<T>(XElement x, T cache) where T : MediaCache, iTVSource
    {
        // Will have one or more cachedSeries, and episodes
        // all wrapped in <Data> </Data>

        // e.g.:
        //<Data>
        // <Series>
        //  <id>...</id>
        //  etc.
        // </Series>
        // <Episode>
        //  <id>...</id>
        //  blah blah
        // </Episode>
        // <Episode>
        //  <id>...</id>
        //  blah blah
        // </Episode>
        // ...
        //</Data>

        try
        {
            string? time = x.Attribute("time")?.Value;
            if (time != null)
            {
                cache.LatestUpdateTimeIs(time);
            }
            else
            {
                Logger.Error("Could not obtain update time from XML");
            }

            foreach (CachedSeriesInfo si in x.Descendants("Series").Select(seriesXml => new CachedSeriesInfo(seriesXml, TVDoc.ProviderType.libraryDefault)))
            {
                // The <cachedSeries> returned by GetSeries have
                // less info than other results from
                // thetvdb.com, so we need to smartly merge
                // in a <Series> if we already have some/all
                // info on it (depending on which one came
                // first).
                cache.AddSeriesToCache(si);
            }

            foreach (XElement episodeXml in x.Descendants("Episode"))
            {
                Episode e = new(episodeXml);
                if (e.Ok())
                {
                    cache.AddOrUpdateEpisode(e);
                }
                else
                {
                    Logger.Error($"problem with XML recieved {episodeXml}");
                }
            }

            foreach (XElement banners in x.Descendants("BannersCache"))
            {
                //this wil not be found in a standard response from the TVDB website
                //will only be in the response when we are reading from the cache
                ProcessXmlBannerCache(banners, cache);
            }
        }
        catch (XmlException e)
        {
            string message = "Error processing data from Cache (top level).";
            message += "\r\n" + x;
            message += "\r\n" + e.ErrorText();

            Logger.Error(message);
            Logger.Error(x.ToString());
            throw new CacheLoadException(message, e);
        }
        return true;
    }

    private static void ProcessXmlBannerCache(XElement r, iTVSource localCache)
    {
        //this is a wrapper that provides the seriesId and the Banners List as provided from the website
        //
        //
        // <BannersCache>
        //      <BannersItem Expiry='xx'>
        //          <SeriesId>123</SeriesId>
        //          <Banners>
        //              <Banner>
        //NB - this is legacy and will be removed post 2021 - Just to migrate old 'Banner' format to new Image format
        foreach (XElement bannersXml in r.Descendants("BannersItem"))
        {
            int seriesId = bannersXml.ExtractInt("SeriesId") ?? -1;

            localCache.GetSeries(seriesId)?.AddBanners(bannersXml.Descendants("Banners").First().Descendants("Banner")
                .Select(banner => ShowImage.GenerateFromLegacyBannerXml(banner, localCache.SourceProvider())));
        }
    }
}
