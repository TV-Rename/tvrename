// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using File = Alphaleonis.Win32.Filesystem.File;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

// Talk to the TheTVDB web API, and get tv series info

// Hierarchy is:
//   TheTVDB -> Series (class SeriesInfo) -> Seasons (class Season) -> Episodes (class Episode)

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    public class TheTVDB : iTVSource
    {
        [Serializable()]
        // ReSharper disable once InconsistentNaming
        public class TVDBException : Exception
        {
            // Thrown if an error occurs in the XML when reading TheTVDB.xml
            public TVDBException(string message)
                : base(message)
            {
            }
        }

        // ReSharper disable once ConvertToConstant.Local
        private static readonly string WebsiteRoot = "http://thetvdb.com";

        private FileInfo cacheFile;
        public bool Connected;

        // ReSharper disable once InconsistentNaming
        public string CurrentDLTask;

        private ConcurrentDictionary<int,ExtraEp> extraEpisodes; // IDs of extra episodes to grab and merge in on next update
        private ConcurrentDictionary<int,ExtraEp> removeEpisodeIds; // IDs of episodes that should be removed

        private ConcurrentDictionary<int,int> forceReloadOn;
        public Languages LanguageList;
        public string LastError;
        public string LoadErr;
        public bool LoadOk;
        private long newSrvTime;

        public static readonly object SERIES_LOCK = new object();
        // TODO: make this private or a property. have online/offline state that controls auto downloading of needed info.
        private readonly Dictionary<int, SeriesInfo> series = new Dictionary<int, SeriesInfo>();

        public static readonly object LANGUAGE_LOCK = new object();

        private long srvTime; // only update this after a 100% successful download
        private readonly TvDbTokenProvider tvDbTokenProvider = new TvDbTokenProvider();

        // ReSharper disable once ConvertToConstant.Local
        private static readonly string DefaultLanguageCode = "en"; //Default backup language

        private CommandLineArgs args;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        //We are using the singleton design pattern
        //http://msdn.microsoft.com/en-au/library/ff650316.aspx

        private static volatile TheTVDB IntenalInstance;
        private static readonly object SyncRoot = new object();

        public static TheTVDB Instance
        {
            get
            {
                if (IntenalInstance == null)
                {
                    lock (SyncRoot)
                    {
                        if (IntenalInstance == null)
                            IntenalInstance = new TheTVDB();
                    }
                }

                return IntenalInstance;
            }
        }

        public Language PreferredLanguage => LanguageList.GetLanguageFromCode(TVSettings.Instance.PreferredLanguageCode);

        public void Setup(FileInfo loadFrom, FileInfo cache, CommandLineArgs cla)
        {
            args = cla;

            System.Diagnostics.Debug.Assert(cache != null);
            cacheFile = cache;

            LastError = "";
            Connected = false;
            extraEpisodes = new ConcurrentDictionary<int, ExtraEp>(); 
            removeEpisodeIds = new ConcurrentDictionary<int, ExtraEp>();

            LanguageList = new Languages {new Language(7, "en", "English", "English")};

            //assume that the data is up to date (this will be overridden by the value in the XML if we have a prior install)
            //If we have no prior install then the app has no shows and is by definition up-to-date
            newSrvTime = DateTime.UtcNow.ToUnixTime();

            srvTime = 0;

            LoadOk = (loadFrom == null) || LoadCache(loadFrom);

            forceReloadOn = new ConcurrentDictionary<int, int>();
        }

        public bool HasSeries(int id) => series.ContainsKey(id);

        public SeriesInfo GetSeries(int id) => HasSeries(id) ? series[id] : null;

        public SeriesInfo GetSeriesAndDownload(int id) => HasSeries(id) ? series[id] : DownloadSeriesNow(id,false,false,false,TVSettings.Instance.PreferredLanguageCode);

        public Dictionary<int, SeriesInfo> GetSeriesDict() => series;

        private Dictionary<int, SeriesInfo> GetSeriesDictMatching(string testShowName)
        {
            Dictionary<int, SeriesInfo> matchingSeries = new Dictionary<int, SeriesInfo>();

            testShowName = testShowName.CompareName();

            if (string.IsNullOrEmpty(testShowName)) return matchingSeries;

            foreach (KeyValuePair<int, SeriesInfo> kvp in series)
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

        private void Say(string s)
        {
            CurrentDLTask = s;
            Logger.Info("Status on screen updated: {0}", s);
        }

        private bool LoadCache(FileInfo loadFrom)
        {
            Logger.Info("Loading Cache from: {0}", loadFrom.FullName);
            if (!loadFrom.Exists)
                return true; // that's ok

            try
            {
                XElement x = XElement.Load(loadFrom.FullName);
                bool r = ProcessXml(x);
                if (r)
                    UpdatesDoneOk();

                return r;
            }
            catch (Exception e)
            {
                Logger.Warn(e, "Problem on Startup loading File");
                LoadErr = loadFrom.Name + " : " + e.Message;
                return false;
            }
        }

        public void UpdatesDoneOk()
        {
            // call when all downloading and updating is done.  updates local Srv_Time with the tentative
            // new_srv_time value.
            srvTime = newSrvTime;
        }

        public void SaveCache()
        {
            Logger.Info("Saving Cache to: {0}", cacheFile.FullName);
            try
            {
                RotateCacheFiles();

                // write ourselves to disc for next time.  use same structure as thetvdb.com (limited fields, though)
                // to make loading easy
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    NewLineOnAttributes = true
                };

                lock (SERIES_LOCK)
                {
                    using (XmlWriter writer = XmlWriter.Create(cacheFile.FullName, settings))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("Data");
                        XmlHelper.WriteAttributeToXml(writer, "time", srvTime);

                        foreach (KeyValuePair<int, SeriesInfo> kvp in series)
                        {
                            if (kvp.Value.SrvLastUpdated != 0)
                            {
                                kvp.Value.WriteXml(writer);
                                foreach (KeyValuePair<int, Season> kvp2 in kvp.Value.AiredSeasons)
                                    //We can use AiredSeasons as it does not matter which order we do this in Aired or DVD
                                {
                                    Season seas = kvp2.Value;
                                    foreach (Episode e in seas.Episodes.Values)
                                        e.WriteXml(writer);
                                }
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

                            XmlHelper.WriteElementToXml(writer, "SeriesId", kvp.Key);

                            writer.WriteStartElement("Banners");

                            //We need to write out all banners that we have in any of the collections. 

                            foreach (KeyValuePair<int, Banner> kvp3 in kvp.Value.AllBanners)
                            {
                                Banner ban = kvp3.Value;
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
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to save Cache to {cacheFile.FullName}");
            }
        }

        private void RotateCacheFiles()
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
                                File.Delete(fn2);

                            File.Move(fn, fn2);
                        }
                    }

                    File.Copy(cacheFile.FullName, cacheFile.FullName + ".0");
                }
            }
        }

        public SeriesInfo GetSeries(string showName)
        {
            Search(showName);

            if (string.IsNullOrEmpty(showName))
                return null;

            showName = showName.ToLower();

            List<SeriesInfo> matchingShows = GetSeriesDictMatching(showName).Values.ToList();

            if (matchingShows.Count == 0) return null;

            if (matchingShows.Count == 1) return matchingShows.First();

            return null;
        }

        private Episode FindEpisodeById(int id)
        {
            lock(SERIES_LOCK)
            {
                foreach (KeyValuePair<int, SeriesInfo> kvp in series.ToList())
                {
                    foreach (KeyValuePair<int, Season> kvp2 in kvp.Value?.AiredSeasons??new Dictionary<int, Season>())
                        //We can use AiredSeasons as it does not matter which order we do this in Aired or DVD
                    {
                        if (kvp2.Value?.Episodes?.ContainsKey(id)??false)
                        {
                            return kvp2.Value.Episodes[id];
                        }
                    }
                }
            }
            return null;
        }

        public bool Connect()
        {
            Connected = UpdateLanguages();
            return Connected;
        }

        internal static string BuildUrl(int code, string lang)
            //would rather make this private to hide api key from outside world
        {
            return $"{WebsiteRoot}/api/{TvDbTokenProvider.TVDB_API_KEY}/series/{code}/all/{lang}.zip";
        }

        // ReSharper disable once InconsistentNaming
        public static string GetImageURL(string url)
        {
            string mirr = WebsiteRoot;

            if (url.StartsWith("/"))
                url = url.Substring(1);

            if (!mirr.EndsWith("/"))
                mirr += "/";

            return mirr + "banners/" + url;
        }

        public byte[] GetTvdbDownload(string url, bool forceReload = false)
        {
            string theUrl = GetImageURL(url);

            WebClient wc = new WebClient();

            if (forceReload)
                wc.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.Reload);

            try
            {
                byte[] r = wc.DownloadData(theUrl);

                if (!url.EndsWith(".zip"))
                    Logger.Info("Downloaded " + theUrl + ", " + r.Length + " bytes");

                return r;
            }
            catch (WebException e)
            {
                Logger.Warn(CurrentDLTask + " : " + e.Message + " : " + theUrl);
                LastError = CurrentDLTask + " : " + e.Message;
                return null;
            }
        }

        public void ForgetEverything()
        {
            lock (SERIES_LOCK)
            {
                series.Clear();
            }

            Connected = false;
            SaveCache();

            //All series will be forgotten and will be fully refreshed, so we'll only need updates after this point
            newSrvTime = DateTime.UtcNow.ToUnixTime();
        }

        public void ForgetShow(int id, bool makePlaceholder,bool useCustomLanguage, string customLanguageCode)
        {
            lock (SERIES_LOCK)
            {
                if (series.ContainsKey(id))
                {
                    string name = series[id].Name;
                    series.Remove(id);
                    if (makePlaceholder)
                    {
                        if (useCustomLanguage)
                            AddPlaceholderSeries(id, name, customLanguageCode);
                        else
                            AddPlaceholderSeries(id, name);

                        forceReloadOn.TryAdd(id, id);
                    }
                }
            }
        }

        public void ForgetShow(int id)
        {
            lock (SERIES_LOCK)
            {
                if (series.ContainsKey(id))
                {
                    series.Remove(id);
                }
            }
        }
        private bool UpdateLanguages()
        {
            Say("TheTVDB Languages");
            try
            {
                JObject jsonResponse =
                    HttpHelper.JsonHttpGetRequest(TvDbTokenProvider.TVDB_API_URL + "/languages", null, tvDbTokenProvider.GetToken(),true);

                LanguageList.Clear();

                foreach (JToken jToken in jsonResponse["data"])
                {
                    JObject languageJson = (JObject) jToken;
                    int id = (int) languageJson["id"];
                    string name = (string) languageJson["name"];
                    string englishName = (string) languageJson["englishName"];
                    string abbrev = (string) languageJson["abbreviation"];

                    if ((id != -1) && (!string.IsNullOrEmpty(name)) && (!string.IsNullOrEmpty(abbrev)))
                        LanguageList.Add(new Language(id, abbrev, name, englishName));
                }

                Say("");
                return true;
            }
            catch (WebException ex)
            {
                Say("Could not connect to TVDB");
                Logger.Error(ex, "Error obtaining Languages from TVDB");
                LastError = ex.Message;
                return false;
            }
        }

        public bool GetUpdates()
        {
            Say("Updates list");

            if (!Connected && !Connect())
            {
                Say("");
                return false;
            }

            long theTime = GetUpdateTime();

            MarkPlaceholdersDirty();

            if (theTime == 0)
            {
                Say("");
                return true; // that's it for now
            }

            long epochTime = theTime;

            string uri = TvDbTokenProvider.TVDB_API_URL + "/updated/query";

            //We need to ask for updates in blocks of 7 days
            //We'll keep asking until we get to a date within 7 days of today 
            //(up to a maximum of 10 - if you are this far behind then you may need multiple refreshes)

            List<JObject> updatesResponses = new List<JObject>();

            bool moreUpdates = true;
            int numberofCallsMade = 0;

            while (moreUpdates)
            {
                JObject jsonUpdateResponse;

                //If this date is in the last week then this needs to be the last call to the update
                DateTime requestedTime = Helpers.FromUnixTime(epochTime).ToUniversalTime();
                DateTime now = DateTime.UtcNow;
                if ((now - requestedTime).TotalDays < 7)
                {
                    moreUpdates = false;
                }

                try
                {
                    jsonUpdateResponse = HttpHelper.JsonHttpGetRequest(uri,
                        new Dictionary<string, string> { { "fromTime", epochTime.ToString() } },
                        tvDbTokenProvider.GetToken(), TVSettings.Instance.PreferredLanguageCode,true);
                }
                catch (WebException ex)
                {
                    Logger.Warn("Error obtaining " + uri + ": from lastupdated query -since(local) " +
                                Helpers.FromUnixTime(epochTime).ToLocalTime());

                    Logger.Warn(ex);
                    Say("");
                    LastError = ex.Message;
                    return false;
                }

                int numberOfResponses;
                try
                {
                    JToken dataToken = jsonUpdateResponse["data"];

                    numberOfResponses = !dataToken.HasValues ? 0 : ((JArray)dataToken).Count;
                }
                catch (InvalidCastException ex)
                {
                    Say("");
                    LastError = ex.Message;

                    string msg = "Unable to get latest updates from TVDB " + Environment.NewLine +
                                 "Trying to get updates since " + Helpers.FromUnixTime(epochTime).ToLocalTime() +
                                 Environment.NewLine + Environment.NewLine +
                                 "If the date is very old, please consider a full refresh";

                    Logger.Warn("Error obtaining " + uri + ": from lastupdated query -since(local) " +
                                Helpers.FromUnixTime(epochTime).ToLocalTime());

                    Logger.Warn(ex, msg);

                    if ((!args.Unattended) && (!args.Hide))
                        MessageBox.Show(msg, "Error obtaining updates from TVDB", MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                    return true;
                }

                updatesResponses.Add(jsonUpdateResponse);
                numberofCallsMade++;
                long maxUpdateTime = GetUpdateTime(jsonUpdateResponse);

                if (maxUpdateTime > 0)
                {
                    newSrvTime =
                        Math.Max(newSrvTime,
                            Math.Max(maxUpdateTime,
                                srvTime)); // just in case the new update time is no better than the prior one

                    Logger.Info("Obtained " + numberOfResponses + " responses from lastupdated query #" +
                                numberofCallsMade + " - since (local) " +
                                Helpers.FromUnixTime(epochTime).ToLocalTime() + " - to (local) " +
                                Helpers.FromUnixTime(newSrvTime).ToLocalTime());

                    epochTime = newSrvTime;
                }

                //As a safety measure we check that no more than 10 calls are made
                if (numberofCallsMade > 10)
                {
                    moreUpdates = false;
                    string errorMessage =
                        "We have run 10 weeks of updates and we are not up to date.  The system will need to check again once this set of updates have been processed." +
                        Environment.NewLine + "Last Updated time was " + Helpers.FromUnixTime(srvTime).ToLocalTime() +
                        Environment.NewLine + "New Last Updated time is " +
                        Helpers.FromUnixTime(newSrvTime).ToLocalTime() + Environment.NewLine + Environment.NewLine +
                        "If the dates keep getting more recent then let the system keep getting 10 week blocks of updates, otherwise consider a 'Force Refresh All'";

                    Logger.Warn(errorMessage);
                    if ((!args.Unattended) && (!args.Hide))
                        MessageBox.Show(errorMessage, "Long Running Update", MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                }
            }

            Say("Processing Updates from TVDB");

            Parallel.ForEach(updatesResponses, jsonResponse => { ProcessUpdate(jsonResponse, uri); });

            Say("Upgrading dirty locks");

            UpgradeDirtyLocks();

            Say("");

            return true;
        }

        private long GetUpdateTime()
        {
            return srvTime == 0 ? GetUpdateTimeFromShows(0) : srvTime;
        }

        private void UpgradeDirtyLocks()
        {
            // if more than x% of a show's episodes are marked as dirty, just download the entire show again
            foreach (KeyValuePair<int, SeriesInfo> kvp in series)
            {
                int totaleps = 0;
                int totaldirty = 0;
                foreach (KeyValuePair<int, Season> kvp2 in kvp.Value.AiredSeasons)
                {
                    foreach (Episode ep in kvp2.Value.Episodes.Values)
                    {
                        if (ep.Dirty)
                            totaldirty++;

                        totaleps++;
                    }
                }

                float percentDirty = 100;
                if (totaldirty > 0 || totaleps > 0) percentDirty = 100 * totaldirty / (float)totaleps;
                if ((totaleps > 0) && ((percentDirty) >= TVSettings.Instance.PercentDirtyUpgrade())) // 10%
                {
                    kvp.Value.Dirty = true;
                    kvp.Value.AiredSeasons.Clear();
                    kvp.Value.DvdSeasons.Clear();
                    Logger.Info("Planning to download all of {0} as {1}% of the episodes need to be updated",
                        kvp.Value.Name, percentDirty);
                }
                else
                    Logger.Trace(
                        "Not planning to download all of {0} as {1}% of the episodes need to be updated and that's less than the 10% limit to upgrade.",
                        kvp.Value.Name, percentDirty);
            }
        }

        private void ProcessUpdate(JObject jsonResponse, string uri)
        {
            // if updatetime > localtime for item, then remove it, so it will be downloaded later
            try
            {
                foreach (JToken jToken in jsonResponse["data"])
                {
                    JObject seriesResponse = (JObject) jToken;
                    int id = (int)seriesResponse["id"];
                    long time = (long)seriesResponse["lastUpdated"];

                    if (series.ContainsKey(id)) // this is a series we have
                    {
                        if (time > series[id].SrvLastUpdated) // newer version on the server
                            series[id].Dirty = true; // mark as dirty, so it'll be fetched again later
                        else
                            Logger.Info(series[id].Name + " has a lastupdated of  " +
                                        Helpers.FromUnixTime(series[id].SrvLastUpdated) + " server says " +
                                        Helpers.FromUnixTime(time));

                        //now we wish to see if any episodes from the series have been updated. If so then mark them as dirty too
                        List<JObject> episodeDefaultLangResponses = null;
                        string requestedLanguageCode = series[id].UseCustomLanguage ? series[id].TargetLanguageCode: TVSettings.Instance.PreferredLanguageCode;
                        try
                        {
                            List<JObject> episodeResponses = GetEpisodes(id, requestedLanguageCode);
                            if (IsNotDefaultLanguage(requestedLanguageCode)) episodeDefaultLangResponses = GetEpisodes(id, DefaultLanguageCode);

                            Dictionary<int, Tuple<JToken, JToken>> episodesResponses =
                                MergeEpisodeResponses(episodeResponses, episodeDefaultLangResponses);

                            int numberOfNewEpisodes = 0;
                            int numberOfUpdatedEpisodes = 0;

                            ICollection<int> oldEpisodeIds = new List<int>();
                            foreach (KeyValuePair<int, Season> kvp2 in GetSeries(id)?.AiredSeasons ?? new Dictionary<int, Season>())
                            {
                                foreach (Episode ep in kvp2.Value.Episodes.Values)
                                {
                                    oldEpisodeIds.Add(ep.EpisodeId);
                                }
                            }

                            try
                            {
                                foreach (KeyValuePair<int, Tuple<JToken, JToken>> episodeData in episodesResponses)
                                {
                                    JToken episodeToUse = (episodeData.Value.Item1 ?? episodeData.Value.Item2);
                                    long serverUpdateTime = (long) episodeToUse["lastUpdated"];
                                    int serverEpisodeId = episodeData.Key;

                                    bool found = false;
                                    foreach (KeyValuePair<int, Season> kvp2 in series[id].AiredSeasons)
                                    {
                                        Season seas = kvp2.Value;

                                        foreach (Episode ep in seas.Episodes.Values)
                                        {
                                            if (ep.EpisodeId == serverEpisodeId)
                                            {
                                                oldEpisodeIds.Remove(serverEpisodeId);

                                                if (ep.SrvLastUpdated < serverUpdateTime)
                                                {
                                                    ep.Dirty = true; // mark episode as dirty.
                                                    numberOfUpdatedEpisodes++;
                                                }

                                                found = true;
                                                break;
                                            }
                                        }
                                    }

                                    if (!found)
                                    {
                                        // must be a new episode
                                        extraEpisodes.TryAdd(serverEpisodeId, new ExtraEp(id, serverEpisodeId));
                                        numberOfNewEpisodes++;
                                    }
                                }
                            }
                            catch (InvalidCastException ex)
                            {
                                Logger.Error(ex, "Did not receive the expected format of episode json from {0}.", uri);
                                Logger.Error(jsonResponse["data"].ToString());
                            }
                            catch (OverflowException ex)
                            {
                                Logger.Error(ex, "Could not parse the episode json from {0}.", uri);
                                Logger.Error(jsonResponse["data"].ToString());
                            }

                            Logger.Info(series[id].Name + " had " + numberOfUpdatedEpisodes +
                                        " episodes updated and " + numberOfNewEpisodes + " new episodes ");

                            if (oldEpisodeIds.Count > 0)
                                Logger.Warn(series[id].Name + " had " + oldEpisodeIds.Count +
                                            " episodes deleted: " + string.Join(",", oldEpisodeIds));

                            foreach (int episodeId in oldEpisodeIds)
                                removeEpisodeIds.TryAdd(episodeId,new ExtraEp(id, episodeId));
                        }
                        catch (ShowNotFoundException ex)
                        {
                            Logger.Warn($"Episodes were not found for {ex.ShowId}:{series[id].Name} in languange {requestedLanguageCode} or {DefaultLanguageCode}");
                        }
                    }
                }
            }
            catch (InvalidCastException ex)
            {
                Logger.Error("Did not receive the expected format of json from {0}.", uri);
                Logger.Error(ex);
                Logger.Error(jsonResponse["data"].ToString());
            }
            catch (OverflowException ex)
            {
                Logger.Error("Could not parse the json from {0}.", uri);
                Logger.Error(ex);
                Logger.Error(jsonResponse["data"].ToString());
            }
        }

        private static long GetUpdateTime(JObject jsonUdpateResponse)
        {
            long maxUpdateTime;
            try
            {
                IEnumerable<long> updateTimes = from a in jsonUdpateResponse["data"] select (long)a["lastUpdated"];
                maxUpdateTime = updateTimes.DefaultIfEmpty(0).Max();
            }
            catch (Exception e)
            {
                Logger.Error(e, jsonUdpateResponse.ToString());
                maxUpdateTime = 0;
            }

            return maxUpdateTime;
        }

        private long GetUpdateTimeFromShows(long theTime)
        {
            // we can use the oldest thing we have locally.  It isn't safe to use the newest thing.
            // This will only happen the first time we do an update, so a false _all update isn't too bad.
            foreach (KeyValuePair<int, SeriesInfo> kvp in series)
            {
                SeriesInfo ser = kvp.Value;
                if ((theTime == 0) || ((ser.SrvLastUpdated != 0) && (ser.SrvLastUpdated < theTime)))
                    theTime = ser.SrvLastUpdated;

                //We can use AiredSeasons as it does not matter which order we do this in Aired or DVD
                foreach (KeyValuePair<int, Season> kvp2 in kvp.Value.AiredSeasons)
                {
                    Season seas = kvp2.Value;

                    foreach (Episode e in seas.Episodes.Values)
                    {
                        if ((theTime == 0) || ((e.SrvLastUpdated != 0) && (e.SrvLastUpdated < theTime)))
                            theTime = e.SrvLastUpdated;
                    }
                }
            }

            return theTime;
        }

        private void MarkPlaceholdersDirty()
        {
            // anything with a srv_lastupdated of 0 should be marked as dirty
            // typically, this'll be placeholder series
            foreach (KeyValuePair<int, SeriesInfo> kvp in series)
            {
                SeriesInfo ser = kvp.Value;
                if ((ser.SrvLastUpdated == 0) || (ser.AiredSeasons.Count == 0))
                    ser.Dirty = true;

                foreach (KeyValuePair<int, Season> kvp2 in kvp.Value.AiredSeasons)
                {
                    foreach (Episode ep in kvp2.Value.Episodes.Values)
                    {
                        if (ep.SrvLastUpdated == 0)
                            ep.Dirty = true;
                    }
                }
            }
        }

        private List<JObject> GetEpisodes(int id,string lang)
        {
            //Now deal with obtaining any episodes for the series 
            //tvDB only gives us responses in blocks of 100, so we need to iterate over the pages until we get one with <100 rows
            //We push the results into a bag to use later
            //If there is a problem with the while method then we can be proactive by using /series/{id}/episodes/summary to get the total
            string episodeUri = EpisodeUri(id);
            List<JObject> episodeResponses = new List<JObject>();

            int pageNumber = 1;
            bool morePages = true;

            while (morePages)
            {
                try
                {
                    JObject jsonEpisodeResponse = HttpHelper.JsonHttpGetRequest(episodeUri,
                        new Dictionary<string, string> { { "page", pageNumber.ToString() } },
                        tvDbTokenProvider.GetToken(),lang,true);

                    episodeResponses.Add(jsonEpisodeResponse);
                    try
                    {
                        int numberOfResponses = ((JArray) jsonEpisodeResponse["data"]).Count;
                        JToken x = jsonEpisodeResponse["links"]["next"];
                        bool moreResponses = !string.IsNullOrWhiteSpace(x.ToString());
                        Logger.Info(
                            $"Page {pageNumber} of {GetSeries(id)?.Name} had {numberOfResponses} episodes listed in {lang} with {(moreResponses ? "" : "no ")}more to come");

                        if (numberOfResponses < 100 || !moreResponses)
                        {
                            morePages = false;
                        }
                        else
                        {
                            pageNumber++;
                        }
                    }
                    catch (NullReferenceException nre)
                    {
                        Logger.Error(nre,
                            $"Error obtaining page {pageNumber} of {episodeUri} in lang {lang} using url {episodeUri}: Response was {jsonEpisodeResponse}");
                        morePages = false;
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError && !(ex.Response is null) && ex.Response is HttpWebResponse resp &&
                        resp.StatusCode == HttpStatusCode.NotFound)
                    {
                        Logger.Warn($"Show with Id {id} is no longer available from TVDB (got a 404). Error obtaining page { pageNumber} of { episodeUri} in lang {lang} using url { ex.Response.ResponseUri.AbsoluteUri}");

                        if (TvdbIsUp())
                        {
                            throw new ShowNotFoundException(id);
                        }
                    }

                    Logger.Error(ex, $"Error obtaining {episodeUri}");
                    return null;
                }
                catch (IOException ex)
                {
                    Logger.Warn(ex, "Connection to TVDB Failed whilst loading episode with Id {0}.", id);
                    return null;
                }
            }

            return episodeResponses;
        }

        private static string EpisodeUri(int id)
        {
            return TvDbTokenProvider.TVDB_API_URL + "/series/" + id + "/episodes";
        }

        private void ProcessXmlBannerCache(XElement r)
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
                int seriesId = bannersXml.ExtractInt("SeriesId")??-1;

                lock (SERIES_LOCK)
                {
                    if (series.ContainsKey(seriesId))
                    {
                        foreach (XElement banner in bannersXml.Descendants("Banners").Descendants("Banner"))
                        {
                            Banner b = new Banner(seriesId, banner);

                            if (!series.ContainsKey(b.SeriesId))
                                throw new TVDBException(
                                    $"Can't find the series to add the banner {b.BannerId} to (TheTVDB). {seriesId},{b.SeriesId}");

                            SeriesInfo ser = series[b.SeriesId];

                            ser.AddOrUpdateBanner(b);
                        }

                        series[seriesId].BannersLoaded = true;
                    }
                    else
                    {
                        Logger.Warn($"Banners were found for series {seriesId} - Ignoring them {bannersXml}");
                    }
                }
            }
        }

        private int GetLanguageId() => LanguageList.GetLanguageFromCode(TVSettings.Instance.PreferredLanguageCode)?.Id ?? 7;

        private int GetDefaultLanguageId() => LanguageList.GetLanguageFromCode(DefaultLanguageCode).Id;
        
        private bool ProcessXml(XElement x)
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
                    newSrvTime = (time == null) ? 0 : long.Parse(time);

                    foreach (XElement seriesXml in x.Descendants("Series"))
                    {
                        // The <series> returned by GetSeries have
                        // less info than other results from
                        // thetvdb.com, so we need to smartly merge
                        // in a <Series> if we already have some/all
                        // info on it (depending on which one came
                        // first).

                        SeriesInfo si = new SeriesInfo(seriesXml);

                        lock(SERIES_LOCK)
                        {
                            if (series.ContainsKey(si.TvdbCode))
                                series[si.TvdbCode].Merge(si, GetLanguageId());
                            else
                                series[si.TvdbCode] = si;
                        }
                    }

                    foreach (XElement episodeXml in x.Descendants("Episode"))
                    {
                        Episode e = new Episode(episodeXml);
                        if (e.Ok())
                        {
                            AddOrUpdateEpisode(e);
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
                        ProcessXmlBannerCache(banners);
                    }
                }
                catch (XmlException e)
                {
                    string message = "Error processing data from TheTVDB (top level).";
                    message += "\r\n" + x;
                    message += "\r\n" + e.Message;

                    Logger.Error(message);
                    Logger.Error(x.ToString());
                    throw new TVDBException(message);
                }
            return true;
        }

        private void AddOrUpdateEpisode(Episode e)
        {
            lock (SERIES_LOCK)
            {
                if (!series.ContainsKey(e.SeriesId))
                    throw new TVDBException("Can't find the series to add the episode to (TheTVDB).");

                SeriesInfo ser = series[e.SeriesId];

                Season airedSeason = ser.GetOrAddAiredSeason(e.ReadAiredSeasonNum, e.SeasonId);
                airedSeason.AddUpdateEpisode(e);

                Season dvdSeason = ser.GetOrAddDvdSeason(e.ReadDvdSeasonNum, e.SeasonId);
                dvdSeason.AddUpdateEpisode(e);

                e.SetSeriesSeason(ser, airedSeason, dvdSeason);
            }
        }

        private bool DoWeForceReloadFor(int code)
        {
            return forceReloadOn.ContainsKey(code) || !series.ContainsKey(code);
        }

        private SeriesInfo DownloadSeriesNow(int code, bool episodesToo, bool bannersToo, bool useCustomLangCode, string langCode)
        {
            if (code == 0)
            {
                Say("");
                return null;
            }
            bool forceReload = DoWeForceReloadFor(code);

            Say(GenerateMessage(code, episodesToo, bannersToo));

            string uri = TvDbTokenProvider.TVDB_API_URL + "/series/" + code;
            JObject jsonResponse;
            JObject jsonDefaultLangResponse = new JObject();
            string requestedLanguageCode = useCustomLangCode ? langCode : TVSettings.Instance.PreferredLanguageCode;
            try
            {
                jsonResponse = HttpHelper.JsonHttpGetRequest(uri, null, tvDbTokenProvider.GetToken(),requestedLanguageCode,true);

                if (IsNotDefaultLanguage(requestedLanguageCode))
                    jsonDefaultLangResponse =
                        HttpHelper.JsonHttpGetRequest(uri, null, tvDbTokenProvider.GetToken(), DefaultLanguageCode,true);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response !=null && ex.Response is HttpWebResponse resp &&
                    resp.StatusCode == HttpStatusCode.NotFound)
                {
                    Logger.Warn($"Show with Id {code} is no longer available from TVDB (got a 404). {uri}");
                    Say("");

                    if (TvdbIsUp())
                    {
                        LastError = ex.Message;
                        throw new ShowNotFoundException(code);
                    }
                }

                Logger.Error(ex,"Error obtaining {0}", uri);
                Say("");
                LastError = ex.Message;
                return null;
            }

            if (jsonResponse is null)
            {
                Logger.Error("Error obtaining - no response available {0}", uri);
                Say("");
                return null;
            }

            SeriesInfo si = null;
            JObject seriesData = (JObject) jsonResponse["data"];

            if (IsNotDefaultLanguage(requestedLanguageCode))
            {
                if (jsonDefaultLangResponse != null)
                {
                    JObject seriesDataDefaultLang = (JObject) jsonDefaultLangResponse["data"];
                    si = new SeriesInfo(seriesData, seriesDataDefaultLang, LanguageList.GetLanguageFromCode(requestedLanguageCode).Id);
                }
            }
            else
            {
                si = new SeriesInfo(seriesData, GetLanguageId());
            }

            if (si is null)
            {
                Logger.Error("Error obtaining {0} - no cound not generate a series from the responses", uri);
                Say("");
                return null;
            }

            lock (SERIES_LOCK)
            {
                if (series.ContainsKey(si.TvdbCode))
                    series[si.TvdbCode].Merge(si, LanguageList.GetLanguageFromCode(requestedLanguageCode).Id);
                else
                    series[si.TvdbCode] = si;

                si = GetSeries(code);
            }

            //Now deal with obtaining any episodes for the series (we then group them into seasons)
            //tvDB only gives us responses in blocks of 100, so we need to iterate over the pages until we get one with <100 rows
            //We push the results into a bag to use later
            //If there is a problem with the while method then we can be proactive by using /series/{id}/episodes/summary to get the total

            if (episodesToo || forceReload)
            {
                ReloadEpisodes(code,useCustomLangCode,langCode);
            }
            if (bannersToo || forceReload)
            {
                DownloadSeriesBanners(code, si, requestedLanguageCode);
            }

            DownloadSeriesActors(code,si);

            forceReloadOn.TryRemove(code, out _);

            series.TryGetValue(code, out SeriesInfo returnValue);
            return returnValue;
        }

        private void DownloadSeriesActors(int code,SeriesInfo si)
        {
            //Get the actors too then we'll need another call for that
            try
            {
                JObject jsonActorsResponse = HttpHelper.JsonHttpGetRequest(TvDbTokenProvider.TVDB_API_URL + "/series/" + code + "/actors",
                    null, tvDbTokenProvider.GetToken(),true);

                GetSeries(si.TvdbCode)?.ClearActors();
                foreach (JToken jsonActor in jsonActorsResponse["data"])
                {
                    int actorId = (int)jsonActor["id"];
                    string actorImage = (string)jsonActor["image"];
                    string actorName = (string)jsonActor["name"];
                    string actorRole = (string)jsonActor["role"];
                    int actorSeriesId = (int)jsonActor["seriesId"];
                    int actorSortOrder = (int)jsonActor["sortOrder"];

                    GetSeries(si.TvdbCode)?.AddActor(new Actor(actorId, actorImage, actorName, actorRole, actorSeriesId,
                        actorSortOrder));
                }
            }
            catch (WebException ex)
            {
                if (ex.Response is null) //probably a timeout
                {
                    Logger.Error($"Unble to obtain actors for {series[si.TvdbCode].Name} {ex.Message}" );
                }
                else if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    Logger.Info($"No actors found for {series[si.TvdbCode].Name} using url {ex.Response.ResponseUri.AbsoluteUri}");
                }
                else
                {
                    Logger.Error($"Unble to obtain actors for {series[si.TvdbCode].Name} {ex.Message}");
                }

                LastError = ex.Message;
            }
        }

        private void DownloadSeriesBanners(int code, SeriesInfo si,string requestedLanguageCode)
        {
            List<JObject> bannerResponses = new List<JObject>();
            List<JObject> bannerDefaultLangResponses = new List<JObject>();

            // get /series/id/images if the bannersToo is set - may need to make multiple calls to for each image type
            List<string> imageTypes = new List<string>();
            string uriImages = TvDbTokenProvider.TVDB_API_URL + "/series/" + code + "/images";
            string uriImagesQuery = TvDbTokenProvider.TVDB_API_URL + "/series/" + code + "/images/query";

            try
            {
                JObject jsonEpisodeSearchResponse = HttpHelper.JsonHttpGetRequest(
                    uriImages, null, tvDbTokenProvider.GetToken(),
                    requestedLanguageCode,false);

                JObject a = (JObject)jsonEpisodeSearchResponse["data"];

                foreach (KeyValuePair<string, JToken> imageType in a)
                {
                    if ((int)imageType.Value > 0) imageTypes.Add(imageType.Key);
                }
            }
            catch (WebException ex)
            {
                //no images for chosen language
                Logger.Warn(ex,
                    $"No images found for {uriImages} in language {requestedLanguageCode}");
            }

            foreach (string imageType in imageTypes)
            {
                try
                {
                    JObject jsonImageResponse = HttpHelper.JsonHttpGetRequest(
                        uriImagesQuery,
                        new Dictionary<string, string> { { "keyType", imageType } }, tvDbTokenProvider.GetToken(),
                        requestedLanguageCode,false);

                    bannerResponses.Add(jsonImageResponse);
                }
                catch (WebException webEx)
                {
                    Logger.Info(webEx,
                        $"Looking for {imageType} images (in local language), but none found for seriesId {code}");
                }
            }

            if (IsNotDefaultLanguage(requestedLanguageCode))
            {
                List<string> imageDefaultLangTypes = new List<string>();

                try
                {
                    JObject jsonEpisodeSearchDefaultLangResponse = HttpHelper.JsonHttpGetRequest(
                        uriImages, null, tvDbTokenProvider.GetToken(),
                        DefaultLanguageCode,false);

                    JObject adl = (JObject)jsonEpisodeSearchDefaultLangResponse["data"];

                    foreach (KeyValuePair<string, JToken> imageType in adl)
                    {
                        if ((int)imageType.Value > 0) imageDefaultLangTypes.Add(imageType.Key);
                    }
                }
                catch (WebException ex)
                {
                    Logger.Info(ex, $"Looking for images, but none found for seriesId {code} in {DefaultLanguageCode}");
                    //no images for chosen language
                }

                foreach (string imageType in imageDefaultLangTypes)
                {
                    try
                    {
                        JObject jsonImageDefaultLangResponse = HttpHelper.JsonHttpGetRequest(uriImagesQuery,
                            new Dictionary<string, string> { { "keyType", imageType } }, tvDbTokenProvider.GetToken(),
                            DefaultLanguageCode,false);

                        bannerDefaultLangResponses.Add(jsonImageDefaultLangResponse);
                    }
                    catch (WebException webEx)
                    {
                        Logger.Info(webEx,
                            $"Looking for {imageType} images in {DefaultLanguageCode}, but none found for seriesId {code}");
                    }
                }
            }

            foreach (JObject response in bannerResponses)
            {
                try
                {
                    foreach (JToken jToken in response["data"])
                    {
                        JObject bannerData = (JObject)jToken;
                        Banner b = new Banner(si.TvdbCode, bannerData, GetLanguageId());
                         //   if (!series.ContainsKey(b.SeriesId))
                         //       throw new TVDBException("Can't find the series to add the banner to (TheTVDB).");
                         //   SeriesInfo ser = series[b.SeriesId];
                         //   ser.AddOrUpdateBanner(b);
                         si.AddOrUpdateBanner(b);
                    }
                }
                catch (InvalidCastException ex)
                {
                    Logger.Error(ex, "Did not receive the expected format of json from {0}.", uriImagesQuery);
                    Logger.Error(response["data"].ToString());
                }
            }

            foreach (JObject response in bannerDefaultLangResponses)
            {
                try
                {
                    foreach (JToken jToken in response["data"])
                    {
                        JObject bannerData = (JObject)jToken;
                        Banner b = new Banner(si.TvdbCode, bannerData, GetDefaultLanguageId());
                        lock (SERIES_LOCK)
                        {
                            if (!series.ContainsKey(b.SeriesId))
                                throw new TVDBException($"Can't find the series to add the banner to (TheTVDB). Bannner.SeriesId = {b.SeriesId}, series = {si.Name} ({si.SeriesId}), code = {code}");

                            SeriesInfo ser = series[b.SeriesId];
                            ser.AddOrUpdateBanner(b);
                        }
                    }
                }
                catch (InvalidCastException ex)
                {
                    Logger.Error(ex, "Did not receive the expected format of json from {0}.", uriImagesQuery);
                    Logger.Error(response["data"].ToString());
                }
            }

            si.BannersLoaded = true;
        }

        private string GenerateMessage(int code, bool episodesToo, bool bannersToo)
        {
            string txt;
            if (series.ContainsKey(code))
                txt = series[code].Name;
            else
                txt = "Code " + code;

            if (episodesToo)
                txt += " (Everything)";
            else
                txt += " Overview";

            if (bannersToo)
                txt += " plus banners";

            return txt;
        }

        private bool TvdbIsUp()
        {
            JObject jsonResponse;
            try
            {
                jsonResponse = HttpHelper.JsonHttpGetRequest(TvDbTokenProvider.TVDB_API_URL, null,null,false);
            }
            catch (WebException ex)
            {
                //we expect an Unauthorised response - so we know the site is up

                if (ex.Status == WebExceptionStatus.ProtocolError && !(ex.Response is null) && ex.Response is HttpWebResponse resp)
                {
                    switch (resp.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            return true;
                        case HttpStatusCode.Forbidden:
                            return true;
                        case HttpStatusCode.NotFound:
                            return false;
                    }
                }

                return false;
            }

            if (!jsonResponse.HasValues) return false;

            return true;
        }

        private void ReloadEpisodes(int code, bool useCustomLangCode, string langCode)
        {
            string requestLangCode = useCustomLangCode ? langCode : TVSettings.Instance.PreferredLanguageCode;
            List<JObject> episodePrefLangResponses = GetEpisodes(code, requestLangCode);
            List<JObject> episodeDefaultLangResponses = null;
            if (IsNotDefaultLanguage(requestLangCode)) episodeDefaultLangResponses = GetEpisodes(code, DefaultLanguageCode);

            Dictionary<int, Tuple<JToken, JToken>>  episodeResponses = MergeEpisodeResponses(episodePrefLangResponses, episodeDefaultLangResponses);

            Parallel.ForEach(episodeResponses, episodeData =>
            {
                int episodeId = episodeData.Key;
                JToken prefLangEpisode = episodeData.Value.Item1;
                JToken defltLangEpisode = episodeData.Value.Item2;
                try
                {
                    if (string.IsNullOrEmpty(prefLangEpisode?["filename"]?.ToString()) && string.IsNullOrEmpty(defltLangEpisode?["filename"]?.ToString()))
                    {
                        //The episode does not contain enough data (specifically image filename), so we'll get the full version
                        DownloadEpisodeNow(code, episodeId);
                    }
                    else
                    {
                        bool success = ProcessEpisode(code,episodeId, prefLangEpisode, defltLangEpisode);
                        if (!success)
                        {
                            Logger.Error($"Could not process Episode from {EpisodeUri(code)}. {prefLangEpisode?.ToString()} ::: {defltLangEpisode?.ToString()}");
                        }
                    }
                }
                catch (InvalidCastException ex)
                {
                    Logger.Error($"Did not recieve the expected format of json from {EpisodeUri(code)}. {prefLangEpisode?.ToString()} ::: {defltLangEpisode?.ToString()}");
                }
                catch (OverflowException ex)
                {
                    Logger.Error($"Could not parse the episode json from {EpisodeUri(code)}. {prefLangEpisode?.ToString()} ::: {defltLangEpisode?.ToString()}");
                }
            });
        }

        private static Dictionary<int, Tuple<JToken, JToken>> MergeEpisodeResponses(List<JObject> episodeResponses, List<JObject> episodeDefaultLangResponses)
        {
            Dictionary<int, Tuple<JToken, JToken>> episodeIds = new Dictionary<int, Tuple<JToken, JToken>>();

            if(episodeResponses!=null)
            {
                foreach (JObject epResponse in episodeResponses)
                {
                    foreach (JToken episodeData in epResponse["data"])
                    {
                        int x = (int) episodeData["id"];
                        if (x > 0)
                        {
                            if (episodeIds.ContainsKey(x))
                            {
                                Logger.Warn($"Duplicate episode {x} contained in episode data call");
                            }
                            else episodeIds.Add(x, new Tuple<JToken, JToken>(episodeData, null));
                        }
                    }
                }
            }

            if (episodeDefaultLangResponses != null)
            {
                foreach (JObject epResponse in episodeDefaultLangResponses)
                {
                    foreach (JToken episodeData in epResponse["data"])
                    {
                        int x = (int) episodeData["id"];
                        if (x > 0)
                        {
                            if (episodeIds.ContainsKey(x))
                            {
                                JToken old = episodeIds[x].Item1;
                                episodeIds[x] = new Tuple<JToken, JToken>(old, episodeData);
                            }
                            else
                            {
                                episodeIds.Add(x, new Tuple<JToken, JToken>(null, episodeData));
                            }
                        }
                    }
                }
            }
            return episodeIds;
        }

        private bool InForeignLanguage() => DefaultLanguageCode != TVSettings.Instance.PreferredLanguageCode;

        private bool IsNotDefaultLanguage(string languageCode) => DefaultLanguageCode != languageCode;

        private bool DownloadEpisodeNow(int seriesId, int episodeId, bool dvdOrder = false)
        {
            if (episodeId == 0)
            {
                Logger.Warn($"Asked to download episodeId = 0 for series {seriesId}");
                Say("");
                return true;
            }

            string requestLangCode;
            if (series.ContainsKey(seriesId))
            {
                Episode ep = FindEpisodeById(episodeId);
                string eptxt = EpisodeDescription(dvdOrder, episodeId, ep);
                requestLangCode =  (series[seriesId].UseCustomLanguage)? series[seriesId].TargetLanguageCode: TVSettings.Instance.PreferredLanguageCode;
                Say(series[seriesId].Name + " (" + eptxt + ")");
            }
            else
                return false; // shouldn't happen

            string uri = TvDbTokenProvider.TVDB_API_URL + "/episodes/" + episodeId;
            JObject jsonEpisodeResponse;
            JObject jsonEpisodeDefaultLangResponse = new JObject();

            try
            {
                jsonEpisodeResponse = HttpHelper.JsonHttpGetRequest(uri, null, tvDbTokenProvider.GetToken(), requestLangCode,true);

                if (IsNotDefaultLanguage(requestLangCode))
                    jsonEpisodeDefaultLangResponse =
                        HttpHelper.JsonHttpGetRequest(uri, null, tvDbTokenProvider.GetToken(), DefaultLanguageCode,true);
            }
            catch (WebException ex)
            {
                Logger.Error("Error obtaining " + uri + ": " + ex.Message);
                LastError = ex.Message;
                Say("");
                return false;
            }

            try
            {
                Episode e;
                JObject jsonResponseData = (JObject)jsonEpisodeResponse["data"];

                if (IsNotDefaultLanguage(requestLangCode))
                {
                    JObject seriesDataDefaultLang = (JObject) jsonEpisodeDefaultLangResponse["data"];
                    e = new Episode(seriesId, jsonResponseData, seriesDataDefaultLang);
                }
                else
                {
                    e = new Episode(seriesId, jsonResponseData);
                }

                if (e.Ok())
                {
                    AddOrUpdateEpisode(e);
                }
                else
                {
                    Logger.Error($"problem with JSON recieved {jsonResponseData}");
                }
            }
            catch (TVDBException e)
            {
                Logger.Error("Could not parse TVDB Response " + e.Message);
                LastError = e.Message;
                Say("");
                return false;
            }

            return true;
        }

        private bool ProcessEpisode(int seriesId,int episodeId, JToken prefLangEpisodeData, JToken defLangEpisodeData,
            bool dvdOrder = false)
        {
            if (episodeId == 0)
            {
                Logger.Warn($"Asked to download episodeId = 0 for series {seriesId}");
                Say("");
                return true;
            }
            if (prefLangEpisodeData == null) return ProcessEpisode(seriesId, defLangEpisodeData, dvdOrder);
            if (defLangEpisodeData == null) return ProcessEpisode(seriesId, prefLangEpisodeData, dvdOrder);

            if (series.ContainsKey(seriesId))
            {
                Episode ep = FindEpisodeById(episodeId);
                string eptxt = EpisodeDescription(dvdOrder, episodeId, ep);
                Say(series[seriesId].Name + " (" + eptxt + ")");
            }
            else
                return false; // shouldn't happen

            try
            {
                Episode e= new Episode(seriesId, (JObject)prefLangEpisodeData, (JObject)defLangEpisodeData);

                if (e.Ok())
                {
                    AddOrUpdateEpisode(e);
                }
                else
                {
                    Logger.Error($"problem with JSON recieved {prefLangEpisodeData},{defLangEpisodeData}");
                }
            }
            catch (TVDBException e)
            {
                Logger.Error("Could not parse TVDB Response " + e.Message);
                LastError = e.Message;
                Say("");
                return false;
            }

            return true;
        }

        private bool ProcessEpisode(int seriesId, JToken episodeData, bool dvdOrder = false)
        {
            if (series.ContainsKey(seriesId))
            {
                int episodeId = (int)episodeData["id"];

                if (episodeId == 0)
                {
                    Logger.Warn($"Asked to download episodeId = 0 for series {seriesId}");
                    Say("");
                    return true;
                }

                Episode ep = FindEpisodeById(episodeId);
                string eptxt = EpisodeDescription(dvdOrder, episodeId, ep);

                Say(series[seriesId].Name + " (" + eptxt + ")");
            }
            else
                return false; // shouldn't happen

            try
            {
                Episode e = new Episode(seriesId, (JObject)episodeData);

                if (e.Ok())
                {
                    AddOrUpdateEpisode(e);
                }
                else
                {
                    Logger.Error($"Problem Processing episode data: {episodeData}");
                }
            }
            catch (TVDBException e)
            {
                Logger.Error("Could not parse TVDB Response " + e.Message);
                LastError = e.Message;
                Say("");
                return false;
            }

            return true;
        }

        private static string EpisodeDescription(bool dvdOrder, int episodeId, Episode ep)
        {
            string eptxt = "New Episode Id = " + episodeId;

            if (ep != null)
            {
                if ((dvdOrder) && (ep.TheDvdSeason != null))
                    eptxt = $"S{ep.TheDvdSeason.SeasonNumber:00}E{ep.DvdEpNum:00}";

                if ((!dvdOrder) && (ep.TheAiredSeason != null))
                    eptxt = $"S{ep.TheAiredSeason.SeasonNumber:00}E{ep.AiredEpNum:00}";
            }

            return eptxt;
        }

        private void AddPlaceholderSeries(int code, string name)
        {
            series[code] = new SeriesInfo(name ?? "", code) {Dirty = true};
        }

        private void AddPlaceholderSeries(int code, string name,string customLanguageCode)
        {
            series[code] = new SeriesInfo(name ?? "", code, customLanguageCode) { Dirty = true };
        }

        public bool EnsureUpdated(int code, bool bannersToo, bool useCustomLangCode, string langCode)
        {
            if (DoWeForceReloadFor(code) || (series[code].AiredSeasons.Count == 0))
                return DownloadSeriesNow(code, true, bannersToo, useCustomLangCode, langCode) != null; // the whole lot!

            bool ok = true;

            if ((series[code].Dirty) || (bannersToo && !series[code].BannersLoaded))
                ok = (DownloadSeriesNow(code, false, bannersToo, useCustomLangCode, langCode) != null);

            foreach (KeyValuePair<int, Season> kvp in GetSeries(code)?.AiredSeasons ?? new Dictionary<int, Season>())
            {
                Season seas = kvp.Value;
                foreach (Episode e in seas.Episodes.Values)
                {
                    if (!e.Dirty || e.EpisodeId <= 0) continue;
                    extraEpisodes.TryAdd(e.EpisodeId,new ExtraEp(e.SeriesId, e.EpisodeId));
                }
            }

            Parallel.ForEach(extraEpisodes, ee =>
            {
                if (ee.Value.SeriesId != code || ee.Value.Done) return;
                ok = DownloadEpisodeNow(ee.Value.SeriesId, ee.Key) && ok;
                ee.Value.Done = true;
            });

            foreach (ExtraEp episodetoRemove in removeEpisodeIds.Values)
            {
                series[episodetoRemove.SeriesId].RemoveEpisode(episodetoRemove.EpisodeId);
            }
            removeEpisodeIds.Clear();

            forceReloadOn.TryRemove(code, out _);

            return ok;
        }

        public void Search(string text)
        {
            if (!Connected && !Connect())
            {
                Say("Failed to Connect");
                return;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                Say("Please Search for a Show Name");
                return;
            }

            text = text.RemoveDiacritics(); // API doesn't like accented characters

            bool isNumber = Regex.Match(text, "^[0-9]+$").Success;
            try
            {
                if (isNumber)
                {
                    if (int.TryParse(text,out int textAsInt))
                    {
                        DownloadSeriesNow(textAsInt, false, false, false,
                            TVSettings.Instance.PreferredLanguageCode);
                    }
                }
            }
            catch (ShowNotFoundException)
            {
                //not really an issue so we can continue
            }
        
            // but, the number could also be a name, so continue searching as usual
            //text = text.Replace(".", " ");

            string uri = TvDbTokenProvider.TVDB_API_URL + "/search/series";
            JObject jsonSearchResponse = null;
            JObject jsonSearchDefaultLangResponse = null;
            try
            {
                jsonSearchResponse = HttpHelper.JsonHttpGetRequest(uri, new Dictionary<string, string> {{"name", text}},
                    tvDbTokenProvider.GetToken(), TVSettings.Instance.PreferredLanguageCode,false);
            }
            catch (WebException ex)
            {
                if (ex.Response is null) //probably a timeout
                {
                    Logger.Error($"Error obtaining {uri} for search term '{text}': {ex.Message}");
                    LastError = ex.Message;
                    Say("");
                }
                else if(((HttpWebResponse) ex.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    Logger.Info(
                        $"Could not find any search results for {text} in {TVSettings.Instance.PreferredLanguageCode}");
                }
                else
                {
                    Logger.Error($"Error obtaining {ex.Response.ResponseUri} for search term '{text}': {ex.Message}");
                    LastError = ex.Message;
                    Say("");
                }
            }

            if (InForeignLanguage())
            { 
                try
                {
                    jsonSearchDefaultLangResponse = HttpHelper.JsonHttpGetRequest(uri,
                        new Dictionary<string, string> {{"name", text}}, tvDbTokenProvider.GetToken(), DefaultLanguageCode,false);
                }
                catch (WebException ex)
                {
                    if (ex.Response is null) //probably a timeout
                    {
                        Logger.Error("Error obtaining " + uri + ": " + ex.Message);
                        LastError = ex.Message;
                        Say("");
                    }
                    else if(((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
                    {
                        Logger.Info(
                            $"Could not find any search results for {text} in {DefaultLanguageCode}");
                    }
                    else
                    {
                        Logger.Error("Error obtaining " + uri + ": " + ex.Message);
                        LastError = ex.Message;
                        Say("");
                    }
                }
            }

            if (jsonSearchResponse != null) ProcessSearchResult(uri, jsonSearchResponse,GetLanguageId());

            if (jsonSearchDefaultLangResponse != null)
                //we also want to search for search terms that match in default language
                ProcessSearchResult(uri, jsonSearchDefaultLangResponse,GetDefaultLanguageId());
        }

        private void ProcessSearchResult(string uri, JObject jsonResponse, int languageId)
        {
            try
            {
                foreach (JToken jToken in jsonResponse["data"])
                {
                    JObject seriesResponse = (JObject)jToken;
                    // The <series> returned by GetSeries have
                    // less info than other results from
                    // thetvdb.com, so we need to smartly merge
                    // in a <Series> if we already have some/all
                    // info on it (depending on which one came
                    // first).

                    SeriesInfo si = new SeriesInfo(seriesResponse, languageId);
                    lock(SERIES_LOCK)
                    {
                        if (series.ContainsKey(si.TvdbCode))
                            series[si.TvdbCode].Merge(si, languageId);
                        else
                            series[si.TvdbCode] = si;
                    }
                }
            }
            catch (InvalidCastException ex)
            {
                Logger.Error("Did not receive the expected format of json from {0}.", uri);
                Logger.Error(ex);
                Logger.Error(jsonResponse["data"].ToString());
            }
        }

        public string WebsiteUrl(int seriesId, int seasonId, bool summaryPage)
        {
            // Summary: http://www.thetvdb.com/?tab=series&id=75340&lid=7
            // Season 3: http://www.thetvdb.com/?tab=season&seriesid=75340&seasonid=28289&lid=7

            if (summaryPage || (seasonId <= 0) || !series.ContainsKey(seriesId))
                return $"{WebsiteRoot}/?tab=series&id={seriesId}";
            else
                return $"{WebsiteRoot}/?tab=season&seriesid={seriesId}&seasonid={seasonId}";
        }

        public string WebsiteUrl(int seriesId, int seasonId, int episodeId)
        {
            // http://www.thetvdb.com/?tab=episode&seriesid=73141&seasonid=5356&id=108303&lid=7
            return $"{WebsiteRoot}/?tab=episode&seriesid={seriesId}&seasonid={seasonId}&id={episodeId}";
        }

        // Next episode to air of a given show		
        public Episode NextAiring(int code)
        {
            if (!series.ContainsKey(code) || (series[code].AiredSeasons.Count == 0))
                return null; // DownloadSeries(code, true);

            Episode next = null;
            DateTime today = DateTime.Now;
            DateTime mostSoonAfterToday = new DateTime(0);

            SeriesInfo ser = series[code];
            foreach (KeyValuePair<int, Season> kvp2 in ser.AiredSeasons)
            {
                Season s = kvp2.Value;

                foreach (Episode e in s.Episodes.Values)
                {
                    DateTime? adt = e.GetAirDateDt();
                    if (adt == null) continue;

                    DateTime dt = (DateTime) adt;
                    if ((dt.CompareTo(today) > 0) && ((mostSoonAfterToday.CompareTo(new DateTime(0)) == 0) ||
                                                      (dt.CompareTo(mostSoonAfterToday) < 0)))
                    {
                        mostSoonAfterToday = dt;
                        next = e;
                    }
                }
            }

            return next;
        }

        public void Tidy(ICollection<ShowItem> libraryValues)
        {
            // remove any shows from thetvdb that aren't in My Shows
            List<int> removeList = new List<int>();

            lock(SERIES_LOCK)
            {
                foreach (KeyValuePair<int, SeriesInfo> kvp in GetSeriesDict())
                {
                    bool found = libraryValues.Any(si => si.TvdbCode == kvp.Key);
                    if (!found)
                        removeList.Add(kvp.Key);
                }

                foreach (int i in removeList)
                    ForgetShow(i);
            }

            SaveCache();
        }
    }
}
