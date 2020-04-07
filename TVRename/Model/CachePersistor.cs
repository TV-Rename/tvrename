using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using File = Alphaleonis.Win32.Filesystem.File;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using NLog;

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

        public static void SaveCache(ConcurrentDictionary<int, SeriesInfo> series,[NotNull] FileInfo cacheFile, long timestamp)
        {
            DirectoryInfo di = cacheFile.Directory;
            if (!di.Exists)
            {
                di.Create();
            }

            Logger.Info("Saving Cache to: {0}", cacheFile.FullName);
            try
            {
                RotateCacheFiles(cacheFile);

                // write ourselves to disc for next time.  use same structure as thetvdb.com (limited fields, though)
                // to make loading easy
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    NewLineOnAttributes = true
                };

                using (XmlWriter writer = XmlWriter.Create(cacheFile.FullName, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Data");
                    writer.WriteAttributeToXml("time",timestamp);

                    foreach (KeyValuePair<int, SeriesInfo> kvp in series)
                    {
                        if (kvp.Value.SrvLastUpdated != 0)
                        {
                            kvp.Value.WriteXml(writer);
                            foreach (Episode e in kvp.Value.Episodes)
                            {
                                e.WriteXml(writer);
                            }
                        }
                        else
                        {
                            Logger.Info($"Cannot save {kvp.Value.TvdbCode} ({kvp.Value.Name}) as it has not been updated at all.");
                        }
                    }

                    //
                    // <BannersCache>
                    //      <BannersItem>
                    //          <SeriesId>123</SeriesId>
                    //          <Banners>
                    //              <Banner>

                    writer.WriteStartElement("BannersCache");

                    foreach (KeyValuePair<int, SeriesInfo> kvp in series)
                    {
                        writer.WriteStartElement("BannersItem");

                        writer.WriteElement("SeriesId", kvp.Key);

                        writer.WriteStartElement("Banners");

                        //We need to write out all banners that we have in any of the collections. 

                        foreach (Banner ban in kvp.Value.AllBanners.Select(kvp3 => kvp3.Value))
                        {
                            ban.WriteXml(writer);
                        }

                        writer.WriteEndElement(); //Banners
                        writer.WriteEndElement(); //BannersItem
                    }

                    writer.WriteEndElement(); // BannersCache

                    writer.WriteEndElement(); // data

                    writer.WriteEndDocument();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to save Cache to {cacheFile.FullName}");
            }
        }

        public static bool LoadCache([NotNull] FileInfo loadFrom,iTVSource cache)
        {
            Logger.Info("Loading Cache from: {0}", loadFrom.FullName);
            if (!loadFrom.Exists)
            {
                return true; // that's ok
            }

            try
            {
                XElement x = XElement.Load(loadFrom.FullName);
                bool r = ProcessXml(x,cache);
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

        private static bool ProcessXml([NotNull] XElement x,[NotNull] iTVSource cache)
        {
            // Will have one or more series, and episodes
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
                string time = x.Attribute("time")?.Value;
                cache.LatestUpdateTimeIs(time);

                foreach (SeriesInfo si in x.Descendants("Series").Select(seriesXml => new SeriesInfo(seriesXml)))
                {
                    // The <series> returned by GetSeries have
                    // less info than other results from
                    // thetvdb.com, so we need to smartly merge
                    // in a <Series> if we already have some/all
                    // info on it (depending on which one came
                    // first).
                    cache.UpdateSeries(si);
                }

                foreach (XElement episodeXml in x.Descendants("Episode"))
                {
                    Episode e = new Episode(episodeXml);
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
                    ProcessXmlBannerCache(banners,cache);
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

            foreach (XElement bannersXml in r.Descendants("BannersItem"))
            {
                int seriesId = bannersXml.ExtractInt("SeriesId") ?? -1;

                localCache.AddBanners(seriesId,bannersXml.Descendants("Banners").Descendants("Banner")
                    .Select(banner => new Banner(seriesId, banner)));
            }
        }
    }
}