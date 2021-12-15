using JetBrains.Annotations;
using NLog;
using Polly;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using File = Alphaleonis.Win32.Filesystem.File;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    internal static class CachePersistor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static string LoadErr;

        private static void RotateCacheFiles([NotNull] FileInfo cacheFile)
        {
            if (cacheFile.Exists)
            {
                double hours = 999.9;
                if (File.Exists(cacheFile.FullName + ".0"))
                {
                    // see when the last rotate was, and only rotate if its been at least an hour since the last save
                    DateTime dt = File.GetLastWriteTime(cacheFile.FullName + ".0");
                    hours = DateTime.Now.Subtract(dt).TotalHours;
                }

                if (hours >= 24.0) // rotate the save file daily
                {
                    for (int i = 8; i >= 0; i--)
                    {
                        string fn = cacheFile.FullName + "." + i;
                        if (File.Exists(fn))
                        {
                            string fn2 = cacheFile.FullName + "." + (i + 1);
                            if (File.Exists(fn2))
                            {
                                File.Delete(fn2);
                            }

                            File.Move(fn, fn2);
                        }
                    }

                    File.Copy(cacheFile.FullName, cacheFile.FullName + ".0");
                }
            }
        }
        public static void SaveCache(ConcurrentDictionary<int, CachedSeriesInfo> series, ConcurrentDictionary<int, CachedMovieInfo> movies, [NotNull] FileInfo cacheFile, long timestamp)
        {
            Policy retryPolicy = Policy
                .Handle<Exception>()
                .Retry(3, (exception, retryCount) =>
                {
                    Logger.Error(exception, $"Retry {retryCount}/3 to save {cacheFile.FullName}.");
                });

            retryPolicy.Execute(() =>
            {
                SaveCacheInternal(series, movies, cacheFile, timestamp);
            });
        }
        private static void SaveCacheInternal([NotNull] ConcurrentDictionary<int, CachedSeriesInfo> series, [NotNull] ConcurrentDictionary<int, CachedMovieInfo> movies, [NotNull] FileInfo cacheFile, long timestamp)
        {
            DirectoryInfo di = cacheFile.Directory;
            if (!di.Exists)
            {
                di.Create();
            }

            Logger.Info($"Saving Cache to: {cacheFile.FullName}" );
            try
            {
                RotateCacheFiles(cacheFile);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to rotate files for Cache to {cacheFile.FullName}");
            }
            try
            {
                SaveCacheFileInternal(series, movies, cacheFile, timestamp);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to save Cache to {cacheFile.FullName}");
                throw;
            }
        }

        private static void SaveCacheFileInternal([NotNull] ConcurrentDictionary<int, CachedSeriesInfo> series, [NotNull] ConcurrentDictionary<int, CachedMovieInfo> movies, [NotNull] FileInfo cacheFile,
            long timestamp)
        {
            // write ourselves to disc for next time.  use same structure as thetvdb.com (limited fields, though)
            // to make loading easy
            XmlWriterSettings settings = new()
            {
                Indent = true,
                NewLineOnAttributes = true
            };

            using (XmlWriter writer = XmlWriter.Create(cacheFile.FullName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Data");
                writer.WriteAttributeToXml("time", timestamp);

                foreach (KeyValuePair<int, CachedSeriesInfo> kvp in series)
                {
                    if (kvp.Value.SrvLastUpdated != 0)
                    {
                        kvp.Value.WriteXml(writer);
                    }
                    else
                    {
                        Logger.Info(
                            $"Cannot save TV {kvp.Key} ({kvp.Value.Name}) to {cacheFile.Name} as it has not been updated at all.");
                    }
                }

                foreach (KeyValuePair<int, CachedMovieInfo> kvp in movies)
                {
                    if (!kvp.Value.IsSearchResultOnly)
                    {
                        kvp.Value.WriteXml(writer);
                    }
                    else
                    {
                        Logger.Info(
                            $"Cannot save Movie {kvp.Key} ({kvp.Value.Name}) to {cacheFile.Name} as it is a search result that has not been used.");
                    }
                }

                writer.WriteEndElement(); // data

                writer.WriteEndDocument();
            }
        }

        public static bool LoadTvCache<T>([NotNull] FileInfo loadFrom, [NotNull] T cache) where T : MediaCache, iTVSource
        {
            Logger.Info($"Loading Cache from: {loadFrom.FullName}" );
            if (!loadFrom.Exists)
            {
                return true; // that's ok
            }

            try
            {
                XElement x = XElement.Load(loadFrom.FullName);
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

        public static bool LoadMovieCache<T>([NotNull] FileInfo loadFrom, T cache) where T : MediaCache, iMovieSource
        {
            Logger.Info($"Loading Cache from: {loadFrom.FullName}" );
            if (!loadFrom.Exists)
            {
                return true; // that's ok
            }

            try
            {
                XElement x = XElement.Load(loadFrom.FullName);
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

        private static bool ProcessMovieXml<T>([NotNull] XElement x, T cache) where T:MediaCache, iMovieSource
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
                message += "\r\n" + e.Message;

                Logger.Error(message);
                Logger.Error(x.ToString());
                throw new CacheLoadException(message);
            }
            return true;
        }

        private static bool ProcessSeriesXml<T>([NotNull] XElement x, [NotNull] T cache) where T:MediaCache,iTVSource
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
                    Episode e = episodeXml.CreateEpisode();
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
                message += "\r\n" + e.Message;

                Logger.Error(message);
                Logger.Error(x.ToString());
                throw new CacheLoadException(message);
            }
            return true;
        }

        private static void ProcessXmlBannerCache([NotNull] XElement r, iTVSource localCache)
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

                localCache.GetSeries(seriesId)?.AddBanners(seriesId, bannersXml.Descendants("Banners").Descendants("Banner")
                    .Select(banner => ShowImage.GenerateFromLegacyBannerXml(seriesId, banner, localCache.SourceProvider())));
            }
        }
    }
}
