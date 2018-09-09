// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
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

        public class Language
        {
            public Language(int id, string abbreviation, string name, string englishName)
            {
                Id = id;
                Abbreviation = abbreviation;
                Name = name;
                EnglishName = englishName;
            }

            public int Id { get; set; }
            public string Abbreviation { get; set; }
            public string Name { get; set; }
            public string EnglishName { get; set; }
        }

        private static readonly string WebsiteRoot = "http://thetvdb.com";

        private FileInfo cacheFile;
        public bool Connected;

        // ReSharper disable once InconsistentNaming
        public string CurrentDLTask;
        private List<ExtraEp> extraEpisodes; // IDs of extra episodes to grab and merge in on next update
        private List<ExtraEp> removeEpisodeIds; // IDs of episodes that should be removed

        private List<int> forceReloadOn;
        public List<Language> LanguageList;
        public string LastError;
        public string LoadErr;
        public bool LoadOk;
        private long newSrvTime;

        // TODO: make this private or a property. have online/offline state that controls auto downloading of needed info.
        private readonly Dictionary<int, SeriesInfo> series = new Dictionary<int, SeriesInfo>();

        private long srvTime; // only update this after a 100% successful download
        private readonly TvDbTokenProvider tvDbTokenProvider = new TvDbTokenProvider();

        public string RequestLanguage = "en"; // Set and updated by TVDoc
        private static readonly string DefaultLanguage = "en"; //Default backup language

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

        public void Setup(FileInfo loadFrom, FileInfo cache, CommandLineArgs cla)
        {
            args = cla;

            System.Diagnostics.Debug.Assert(cache != null);
            cacheFile = cache;

            LastError = "";
            Connected = false;
            extraEpisodes = new List<ExtraEp>();
            removeEpisodeIds = new List<ExtraEp>();

            LanguageList = new List<Language> {new Language(7, "en", "English", "English"), new Language(17, "fr", "Français", "French")};

            //assume that the data is up to date (this will be overridden by the value in the XML if we have a prior install)
            //If we have no prior install then the app has no shows and is by definition up-to-date
            newSrvTime = DateTime.UtcNow.ToUnixTime();

            srvTime = 0;

            LoadOk = (loadFrom == null) || LoadCache(loadFrom);

            forceReloadOn = new List<int>();
        }

        private void LockExtraEpisodes() => Monitor.Enter(extraEpisodes);

        private void UnlockExtraEpisodes() => Monitor.Exit(extraEpisodes);

        private void LockRemoveEpisodes() => Monitor.Enter(removeEpisodeIds);

        private void UnlockRemoveEpisodes() => Monitor.Exit(removeEpisodeIds);

        public bool HasSeries(int id) => series.ContainsKey(id);

        public SeriesInfo GetSeries(int id) => HasSeries(id) ? series[id] : null;

        public Dictionary<int, SeriesInfo> GetSeriesDict() => series;

        private Dictionary<int, SeriesInfo> GetSeriesDictMatching(string testShowName)
        {
            Dictionary<int, SeriesInfo> matchingSeries = new Dictionary<int, SeriesInfo>();

            testShowName = Helpers.CompareName(testShowName);

            if (string.IsNullOrEmpty(testShowName)) return matchingSeries;

            foreach (KeyValuePair<int, SeriesInfo> kvp in series)
            {
                string show = Helpers.CompareName(kvp.Value.Name);

                if (show.Contains(testShowName, StringComparison.InvariantCultureIgnoreCase))
                {
                    //We have a match
                    matchingSeries.Add(kvp.Key, kvp.Value);
                }
            }

            return matchingSeries;
        }

        public bool GetLock(string whoFor)
        {
            Logger.Trace("Lock Series for " + whoFor);
            bool ok = Monitor.TryEnter(series, 10000);
            System.Diagnostics.Debug.Assert(ok);
            return ok;
        }

        public void Unlock(string whoFor)
        {
            Logger.Trace("Unlock series for (" + whoFor + ")");
            Monitor.Exit(series);
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

            FileStream fs = null;
            try
            {
                fs = loadFrom.Open(FileMode.Open);
                bool r = ProcessXml(fs, null);
                fs.Close();
                fs = null;
                if (r)
                    UpdatesDoneOk();

                return r;
            }
            catch (Exception e)
            {
                Logger.Warn(e, "Problem on Startup loading File");
                LoadErr = loadFrom.Name + " : " + e.Message;

                fs?.Close();
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
            if (!GetLock("SaveCache"))
                return;

            RotateCacheFiles();

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

            Unlock("SaveCache");
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

            foreach (KeyValuePair<int, SeriesInfo> ser in GetSeriesDictMatching(showName))
            {
                return ser.Value;
            }

            return null;
        }

        private Episode FindEpisodeById(int id)
        {
            if (!GetLock("FindEpisodeByID"))
                return null;

            foreach (KeyValuePair<int, SeriesInfo> kvp in series.ToList())
            {
                foreach (KeyValuePair<int, Season> kvp2 in kvp.Value.AiredSeasons)
                    //We can use AiredSeasons as it does not matter which order we do this in Aired or DVD
                {
                    if (kvp2.Value.Episodes.ContainsKey(id))
                    {
                        Episode e = kvp2.Value.Episodes[id];
                        Unlock("FindEpisodeByID");
                        return e;
                    }
                }
            }

            Unlock("FindEpisodeByID");
            return null;
        }

        public bool Connect()
        {
            Connected = GetLanguages();
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
            if (!GetLock("ForgetEverything"))
                return;

            series.Clear();
            Connected = false;
            SaveCache();
            Unlock("ForgetEverything");

            //All series will be forgotten and will be fully refreshed, so we'll only need updates after this point
            newSrvTime = DateTime.UtcNow.ToUnixTime();
        }

        public void ForgetShow(int id, bool makePlaceholder)
        {
            if (!GetLock("ForgetShow"))
                return;

            if (series.ContainsKey(id))
            {
                string name = series[id].Name;
                int langId = series[id].languageId;
                series.Remove(id);
                if (makePlaceholder)
                {
                    AddPlaceholderSeries(id, name, langId);
                    forceReloadOn.Add(id);
                }
            }

            Unlock("ForgetShow");
        }

        private bool GetLanguages()
        {
            Say("TheTVDB Languages");
            try
            {
                JObject jsonResponse =
                    HttpHelper.JsonHttpGetRequest(TvDbTokenProvider.TVDB_API_URL + "/languages", null, tvDbTokenProvider.GetToken());

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
                Say("ERROR OBTAINING LANGUAGES FROM TVDB");
                Logger.Error(ex, "ERROR OBTAINING LANGUAGES FROM TVDB");
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

            long theTime = srvTime;

            if (theTime == 0)
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
            }

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
                JObject jsonUdpateResponse;

                //If this date is in the last week then this needs to be the last call to the update
                DateTime requestedTime = Helpers.FromUnixTime(epochTime).ToUniversalTime();
                DateTime now = DateTime.UtcNow;
                if ((now - requestedTime).TotalDays < 7)
                {
                    moreUpdates = false;
                }

                try
                {
                    jsonUdpateResponse = HttpHelper.JsonHttpGetRequest(uri,
                        new Dictionary<string, string> {{"fromTime", epochTime.ToString()}},
                        tvDbTokenProvider.GetToken(), TVSettings.Instance.PreferredLanguage);
                }
                catch (WebException ex)
                {
                    Logger.Warn("Error obtaining " + uri + ": from lastupdated query -since(local) " +
                                Helpers.FromUnixTime(epochTime).ToLocalTime());

                    Logger.Warn(ex);
                    Say("");
                    LastError = ex.Message;
                    moreUpdates = false;
                    return false;
                }

                int numberOfResponses = 0;
                try
                {
                    JToken dataToken = jsonUdpateResponse["data"];

                    numberOfResponses = !dataToken.HasValues ? 0 : ((JArray) dataToken).Count;
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

                updatesResponses.Add(jsonUdpateResponse);
                numberofCallsMade++;
                long maxUpdateTime;
                try
                {
                    IEnumerable<long> updateTimes = from a in jsonUdpateResponse["data"] select (long) a["lastUpdated"];
                    maxUpdateTime = updateTimes.DefaultIfEmpty(0).Max();
                }
                catch (Exception e)
                {
                    Logger.Error(e,jsonUdpateResponse.ToString() );
                    maxUpdateTime = 0;
                }

                if (maxUpdateTime > 0)
                {
                    newSrvTime =
                        Math.Max(newSrvTime,
                            Math.Max(maxUpdateTime,
                                srvTime)); // just in case the new update time is no better than the prior one

                    Logger.Info("Obtianed " + numberOfResponses + " responses from lastupdated query #" +
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

            Parallel.ForEach(updatesResponses, jsonResponse =>
            {
                // if updatetime > localtime for item, then remove it, so it will be downloaded later
                try
                {
                    foreach (JObject seriesResponse in jsonResponse["data"])
                    {
                        int id = (int) seriesResponse["id"];
                        long time = (long) seriesResponse["lastUpdated"];

                        if (this.series.ContainsKey(id)) // this is a series we have
                        {
                            if (time > this.series[id].SrvLastUpdated) // newer version on the server
                                this.series[id].Dirty = true; // mark as dirty, so it'll be fetched again later
                            else
                                Logger.Info(this.series[id].Name + " has a lastupdated of  " +
                                            Helpers.FromUnixTime(this.series[id].SrvLastUpdated) + " server says " +
                                            Helpers.FromUnixTime(time));

                            //now we wish to see if any episodes from the series have been updated. If so then mark them as dirty too
                            List<JObject> episodeDefaultLangResponses=null;
                            List<JObject> episodeResponses = GetEpisodes(id, GetLanguage(series[id].languageId));
                            if (InForeignLanguage(GetLanguage(series[id].languageId))) episodeDefaultLangResponses = GetEpisodes(id, DefaultLanguage);

                            Dictionary<int, Tuple<JToken, JToken>> episodesResponses =
                                MergeEpisodeResponses(episodeResponses, episodeDefaultLangResponses);

                            int numberOfNewEpisodes = 0;
                            int numberOfUpdatedEpisodes = 0;

                            ICollection<int> oldEpisodeIds = new List<int>();
                            foreach (KeyValuePair<int, Season> kvp2 in GetSeries(id)?.AiredSeasons??new Dictionary<int, Season>())
                            {
                                foreach (Episode ep in kvp2.Value.Episodes.Values)
                                {
                                    oldEpisodeIds.Add(ep.EpisodeId);
                                }
                            }

                            foreach (JObject response in episodeResponses)
                            {
                                try
                                {
                                    foreach (KeyValuePair<int, Tuple<JToken, JToken>> episodeData in episodesResponses)
                                    {
                                        JToken episodeToUse = (episodeData.Value.Item1??episodeData.Value.Item2);
                                        long serverUpdateTime = (long)episodeToUse["lastUpdated"];
                                        int serverEpisodeId = episodeData.Key;

                                        bool found = false;
                                        foreach (KeyValuePair<int, Season> kvp2 in this.series[id].AiredSeasons)
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
                                            LockExtraEpisodes();
                                            extraEpisodes.Add(new ExtraEp(id, serverEpisodeId));
                                            UnlockExtraEpisodes();
                                            numberOfNewEpisodes++;
                                        }
                                    }
                                }
                                catch (InvalidCastException ex)
                                {
                                    Logger.Error("Did not recieve the expected format of episode json from {0}.", uri);
                                    Logger.Error(ex);
                                    Logger.Error(jsonResponse["data"].ToString());
                                }
                                catch (OverflowException ex)
                                {
                                    Logger.Error("Could not parse the episode json from {0}.", uri);
                                    Logger.Error(ex);
                                    Logger.Error(jsonResponse["data"].ToString());
                                }
                            }

                            Logger.Info(this.series[id].Name + " had " + numberOfUpdatedEpisodes +
                                        " episodes updated and " + numberOfNewEpisodes + " new episodes ");

                            if (oldEpisodeIds.Count > 0)
                                Logger.Warn(this.series[id].Name + " had " + oldEpisodeIds.Count +
                                            " episodes deleted: " + string.Join(",", oldEpisodeIds));

                            LockRemoveEpisodes();
                            foreach (int episodeId in oldEpisodeIds)
                                removeEpisodeIds.Add(new ExtraEp(id, episodeId));

                            UnlockRemoveEpisodes();
                        }
                    }
                }
                catch (InvalidCastException ex)
                {
                    Logger.Error("Did not recieve the expected format of json from {0}.", uri);
                    Logger.Error(ex);
                    Logger.Error(jsonResponse["data"].ToString());
                }
                catch (OverflowException ex)
                {
                    Logger.Error("Could not parse the json from {0}.", uri);
                    Logger.Error(ex);
                    Logger.Error(jsonResponse["data"].ToString());
                }
            });

            Say("Upgrading dirty locks");

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
                if (totaldirty > 0 || totaleps > 0) percentDirty = 100 * totaldirty / totaleps;
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

            Say("");

            return true;
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
                        tvDbTokenProvider.GetToken(),lang);

                    episodeResponses.Add(jsonEpisodeResponse);
                    int numberOfResponses = ((JArray)jsonEpisodeResponse["data"]).Count;
                    JToken x = jsonEpisodeResponse["links"]["next"];
                    bool moreResponses = !string.IsNullOrWhiteSpace(x.ToString());
                    Logger.Info(
                        $"Page {pageNumber} of {GetSeries(id)?.Name} had {numberOfResponses} episodes listed in {lang} with {(moreResponses?"":"no ")}more to come");

                    if (numberOfResponses < 100 || !moreResponses)
                    {
                        morePages = false;
                    }
                    else
                    {
                        pageNumber++;
                    }
                }
                catch (WebException ex)
                {
                    Logger.Error(ex, $"Error obtaining page {pageNumber} of {episodeUri} in lang {lang} using url {ex.Response.ResponseUri.AbsoluteUri}");
                    morePages = false;
                }
            }

            return episodeResponses;
        }

        private static string EpisodeUri(int id)
        {
            return TvDbTokenProvider.TVDB_API_URL + "/series/" + id + "/episodes";
        }

        private void ProcessXmlBannerCache(XmlReader r)
        {
            //this is a wrapper that provides the seriesId and the Banners List as provided from the website
            //
            //
            // <BannersCache>
            //      <BannersItem Expiry='xx'>
            //          <SeriesId>123</SeriesId>
            //          <Banners>
            //              <Banner>
            int seriesId = -1;

            while (!r.EOF)
            {
                if ((r.Name == "BannersCache") && !r.IsStartElement())
                    break; // that's it.

                if (r.Name == "BannersItem")
                    r.Read();
                else if ((r.Name == "SeriesId") && r.IsStartElement())
                    seriesId = r.ReadElementContentAsInt();
                else if ((r.Name == "Banners") && r.IsStartElement())
                {
                    ProcessXmlBanner(r.ReadSubtree(), seriesId);
                    r.Read();
                    seriesId = -1;
                }
                else
                    r.Read();
            }
        }

        private bool ProcessXmlBanner(Stream str, int codeHint)
        {
            // Will have a number of banners in the file. Each is linked to series, a type and a rating.
            // all wrapped in <Banners> </Banners>

            // e.g.: 
            //<Banners>
            //  <Banner>
            //      <id>42604</id>
            //      <BannerPath>fanart/original/79488-11.jpg</BannerPath>
            //      <BannerType>fanart</BannerType>
            //      <BannerType2>1920x1080</BannerType2>
            //      <Colors>|235,227,206|38,14,4|150,139,85|</Colors>
            //      <Language>en</Language>
            //      <Rating>7.9333</Rating>
            //      <RatingCount>15</RatingCount>
            //      <SeriesName>false</SeriesName>
            //      <ThumbnailPath>_cache/fanart/original/79488-11.jpg</ThumbnailPath>
            //      <VignettePath>fanart/vignette/79488-11.jpg</VignettePath>
            //  </Banner>
            //  <Banner>
            //      <id>708811</id>
            //      <BannerPath>seasonswide/79488-5.jpg</BannerPath>
            //      <BannerType>season</BannerType>
            //      <BannerType2>seasonwide</BannerType2>
            //      <Language>en</Language>
            //      <Rating/>
            //      <RatingCount>0</RatingCount>
            //      <Season>5</Season>
            //  </Banner>
            //  .......
            //</Banners>

            //We are only interested in ones that are season speicific posters; these have bannerType and BannerType2 of season.
            //There may be many posters per season - we need to get the best one. This is decided by the Rating field.

            if (!GetLock("ProcessTVDBBannerResponse"))
                return false;

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    IgnoreComments = true,
                    IgnoreWhitespace = true
                };

                using (XmlReader r = XmlReader.Create(str, settings))
                    ProcessXmlBanner(r, codeHint);
            }
            catch (XmlException e)
            {
                string message = "Error processing data from TheTVDB (banner file).";

                string name = $"ID #{codeHint} ";
                if (series.ContainsKey(codeHint))
                {
                    name += $"Show ({series[codeHint].Name})";
                }

                Logger.Error(e, name + "-" + message);

                return false;
            }
            finally
            {
                Unlock("ProcessTVDBBannerResponse");
            }

            return true;
        }

        private void ProcessXmlBanner(XmlReader r, int codeHint)
        {
            r.Read();

            while (!r.EOF)
            {
                if ((r.Name == "Banners") && r.IsStartElement())
                {
                    r.Read();
                }
                else if ((r.Name == "SeriesId"))
                {
                    break; //we should never have got here - this is the seriesId that should have been passed in via codehint
                }
                else if (r.Name == "Banner")
                {
                    Banner b = new Banner(codeHint, r.ReadSubtree());

                    if (!series.ContainsKey(b.SeriesId))
                        throw new TVDBException("Can't find the series to add the banner to (TheTVDB).");

                    SeriesInfo ser = series[b.SeriesId];

                    ser.AddOrUpdateBanner(b);

                    r.Read();
                }
                else if ((r.Name == "Banners") && !r.IsStartElement())
                {
                    series[codeHint].BannersLoaded = true;
                    break; // that's it.
                }

                else if (r.Name == "xml")
                    r.Read();
                else
                    r.Read();
            }
        }

        private int GetLanguageId()
        {
            foreach (Language l in LanguageList)
            {
                if (l.Abbreviation == TVSettings.Instance.PreferredLanguage) return l.Id;
            }

            return -1;
        }

        private int GetLanguageId(string lang)
        {
            foreach (Language l in LanguageList)
                if (l.Abbreviation == lang)
                    return l.Id;

            return -1;
        }

        internal string GetLanguage(int langId)
        {
            foreach (Language l in LanguageList)
                if (l.Id == langId)
                    return l.Abbreviation;

            return "";
        }

        private int GetDefaultLanguageId()
        {
            foreach (Language l in LanguageList)
            {
                if (l.Abbreviation == DefaultLanguage) return l.Id;
            }

            return -1;
        }

        private bool ProcessXml(Stream str, int? codeHint)
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

            if (!GetLock("ProcessTVDBResponse"))
                return false;

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    IgnoreComments = true,
                    IgnoreWhitespace = true
                };

                using (XmlReader r = XmlReader.Create(str, settings))
                {
                    r.Read();

                    while (!r.EOF)
                    {
                        if ((r.Name == "Data") && !r.IsStartElement())
                            break; // that's it.

                        if (r.Name == "Series")
                        {
                            // The <series> returned by GetSeries have
                            // less info than other results from
                            // thetvdb.com, so we need to smartly merge
                            // in a <Series> if we already have some/all
                            // info on it (depending on which one came
                            // first).

                            SeriesInfo si = new SeriesInfo(r.ReadSubtree());
                            if (series.ContainsKey(si.TvdbCode))
                                series[si.TvdbCode].Merge(si, series[si.TvdbCode].languageId);
                            else
                                series[si.TvdbCode] = si;

                            r.Read();
                        }
                        else if (r.Name == "Episode")
                        {
                            Episode e = new Episode(r.ReadSubtree());
                            if (e.Ok())
                            {
                                AddOrUpdateEpisode(e);
                            }

                            r.Read();
                        }
                        else if (r.Name == "xml")
                            r.Read();
                        else if (r.Name == "BannersCache")
                        {
                            //this wil not be found in a standard response from the TVDB website
                            //will only be in the response when we are reading from the cache
                            ProcessXmlBannerCache(r);
                            r.Read();
                        }
                        else if (r.Name == "Data")
                        {
                            string time = r.GetAttribute("time");
                            newSrvTime = (time == null) ? 0 : long.Parse(time);
                            r.Read();
                        }
                        else
                            r.ReadOuterXml();
                    }
                }
            }
            catch (XmlException e)
            {
                str.Position = 0;
                string myStr;
                using (StreamReader sr = new StreamReader(str)) myStr = sr.ReadToEnd();

                string message = "Error processing data from TheTVDB (top level).";
                message += "\r\n" + myStr;
                message += "\r\n" + e.Message;
                string name = "";
                if (codeHint.HasValue && series.ContainsKey(codeHint.Value))
                {
                    name += "Show \"" + series[codeHint.Value].Name + "\" ";
                }

                if (codeHint.HasValue)
                {
                    name += "ID #" + codeHint.Value + " ";
                }

                Logger.Error(name + message);
                Logger.Error(str.ToString());
                throw new TVDBException(name + message);
            }
            finally
            {
                Unlock("ProcessTVDBResponse");
            }

            return true;
        }

        private void AddOrUpdateEpisode(Episode e)
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

        private bool DoWeForceReloadFor(int code)
        {
            return forceReloadOn.Contains(code) || !series.ContainsKey(code);
        }

        private SeriesInfo DownloadSeriesNow(int code, bool episodesToo, bool bannersToo)
        {
            bool forceReload = DoWeForceReloadFor(code);

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

            Say(txt);

            string serieLang = series.ContainsKey(code)
                ? GetLanguage(series[code].languageId)
                : TVSettings.Instance.PreferredLanguage;

            string uri = TvDbTokenProvider.TVDB_API_URL + "/series/" + code;
            JObject jsonResponse;
            JObject jsonDefaultLangResponse = new JObject();
            try
            {
                jsonResponse = HttpHelper.JsonHttpGetRequest(uri, null, tvDbTokenProvider.GetToken(),
                    serieLang);

                if (InForeignLanguage(serieLang))
                    jsonDefaultLangResponse =
                        HttpHelper.JsonHttpGetRequest(uri, null, tvDbTokenProvider.GetToken(), DefaultLanguage);
            }
            catch (WebException ex)
            {
                Logger.Error("Error obtaining {0}", uri);
                Logger.Error(ex);
                Say("");
                LastError = ex.Message;
                return null;
            }

            SeriesInfo si;
            JObject seriesData = (JObject) jsonResponse["data"];

            if (InForeignLanguage(serieLang))
            {
                JObject seriesDataDefaultLang = (JObject) jsonDefaultLangResponse["data"];
                si = new SeriesInfo(seriesData, seriesDataDefaultLang, GetLanguageId(serieLang));
            }
            else
            {
                si = new SeriesInfo(seriesData, GetLanguageId(serieLang));
            }

            if (series.ContainsKey(si.TvdbCode))
                series[si.TvdbCode].Merge(si, series[si.TvdbCode].languageId);
            else
                series[si.TvdbCode] = si;

            //Now deal with obtaining any episodes for the series (we then group them into seasons)
            //tvDB only gives us responses in blocks of 100, so we need to iterate over the pages until we get one with <100 rows
            //We push the results into a bag to use later
            //If there is a problem with the while method then we can be proactive by using /series/{id}/episodes/summary to get the total

            if (episodesToo || forceReload)
            {
                ReloadEpisodes(code);
            }

            List<JObject> bannerResponses = new List<JObject>();
            List<JObject> bannerDefaultLangResponses = new List<JObject>();
            if (bannersToo || forceReload)
            {
                // get /series/id/images if the bannersToo is set - may need to make multiple calls to for each image type
                List<string> imageTypes = new List<string>();

                try
                {
                    JObject jsonEpisodeSearchResponse = HttpHelper.JsonHttpGetRequest(
                        TvDbTokenProvider.TVDB_API_URL + "/series/" + code + "/images", null, tvDbTokenProvider.GetToken(),
                        serieLang);

                    JObject a = (JObject) jsonEpisodeSearchResponse["data"];

                    foreach (KeyValuePair<string, JToken> imageType in a)
                    {
                        if ((int) imageType.Value > 0) imageTypes.Add(imageType.Key);
                    }
                }
                catch (WebException ex)
                {
                    //no images for chosen language
                    Logger.Warn(ex,
                        $"No images found for {TvDbTokenProvider.TVDB_API_URL }/series/{code}/images in language {serieLang}");
                }

                foreach (string imageType in imageTypes)
                {
                    try
                    {
                        JObject jsonImageResponse = HttpHelper.JsonHttpGetRequest(
                            TvDbTokenProvider.TVDB_API_URL + "/series/" + code + "/images/query",
                            new Dictionary<string, string> {{"keyType", imageType}}, tvDbTokenProvider.GetToken(),
                            serieLang);

                        bannerResponses.Add(jsonImageResponse);
                    }
                    catch (WebException webEx)
                    {
                        Logger.Info("Looking for " + imageType +
                                    " images (in local language), but none found for seriesId " + code);

                        Logger.Info(webEx);
                    }
                }

                if (InForeignLanguage(serieLang))
                {
                    List<string> imageDefaultLangTypes = new List<string>();

                    try
                    {
                        JObject jsonEpisodeSearchDefaultLangResponse = HttpHelper.JsonHttpGetRequest(
                            TvDbTokenProvider.TVDB_API_URL + "/series/" + code + "/images", null, tvDbTokenProvider.GetToken(),
                            DefaultLanguage);

                        JObject adl = (JObject) jsonEpisodeSearchDefaultLangResponse["data"];

                        foreach (KeyValuePair<string, JToken> imageType in adl)
                        {
                            if ((int) imageType.Value > 0) imageDefaultLangTypes.Add(imageType.Key);
                        }
                    }
                    catch (WebException ex)
                    {
                        Logger.Info("Looking for images, but none found for seriesId {0} in {1}", code,
                            DefaultLanguage);

                        Logger.Info(ex);

                        //no images for chosen language
                    }

                    foreach (string imageType in imageDefaultLangTypes)
                    {
                        try
                        {
                            JObject jsonImageDefaultLangResponse = HttpHelper.JsonHttpGetRequest(
                                TvDbTokenProvider.TVDB_API_URL + "/series/" + code + "/images/query",
                                new Dictionary<string, string> {{"keyType", imageType}}, tvDbTokenProvider.GetToken(),
                                DefaultLanguage);

                            bannerDefaultLangResponses.Add(jsonImageDefaultLangResponse);
                        }
                        catch (WebException webEx)
                        {
                            Logger.Info(webEx,
                                "Looking for " + imageType + " images, but none found for seriesId " + code);
                        }
                    }
                }
            }

            foreach (JObject response in bannerResponses)
            {
                try
                {
                    foreach (JToken jToken in response["data"])
                    {
                        JObject bannerData = (JObject) jToken;
                        Banner b = new Banner(si.TvdbCode, bannerData, GetLanguageId(serieLang));
                        if (!series.ContainsKey(b.SeriesId))
                            throw new TVDBException("Can't find the series to add the banner to (TheTVDB).");

                        SeriesInfo ser = series[b.SeriesId];
                        ser.AddOrUpdateBanner(b);
                    }
                }
                catch (InvalidCastException ex)
                {
                    Logger.Error("Did not recieve the expected format of json from {0}.", uri);
                    Logger.Error(ex);
                    Logger.Error(jsonResponse["data"].ToString());
                }
            }

            foreach (JObject response in bannerDefaultLangResponses)
            {
                try
                {
                    foreach (JObject bannerData in response["data"])
                    {
                        Banner b = new Banner(si.TvdbCode, bannerData, GetDefaultLanguageId());
                        if (!series.ContainsKey(b.SeriesId))
                            throw new TVDBException("Can't find the series to add the banner to (TheTVDB).");

                        SeriesInfo ser = series[b.SeriesId];
                        ser.AddOrUpdateBanner(b);
                    }
                }
                catch (InvalidCastException ex)
                {
                    Logger.Error("Did not recieve the expected format of json from {0}.", uri);
                    Logger.Error(ex);
                    Logger.Error(jsonResponse["data"].ToString());
                }
            }

            series[si.TvdbCode].BannersLoaded = true;

            //Get the actors too then well need another call for that
            try
            {
                JObject jsonActorsResponse = HttpHelper.JsonHttpGetRequest(TvDbTokenProvider.TVDB_API_URL + "/series/" + code + "/actors",
                    null, tvDbTokenProvider.GetToken());

                GetSeries(si.TvdbCode)?.ClearActors();
                foreach (JToken jsonActor in jsonActorsResponse["data"])
                {
                    int actorId = (int)jsonActor["id"];
                    string actorImage = (string) jsonActor["image"];
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
                if (((HttpWebResponse) ex.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    Logger.Info($"No actors found for {series[si.TvdbCode].Name} using url {ex.Response.ResponseUri.AbsoluteUri}");
                }
                else
                {
                    Logger.Error(ex, "Unble to obtain actors for {0}", series[si.TvdbCode].Name);
                }

                LastError = ex.Message;
            }

            forceReloadOn.Remove(code);

            return (series.ContainsKey(code)) ? series[code] : null;
        }

        private void ReloadEpisodes(int code)
        {
            string serieLang = series.ContainsKey(code)
                ? GetLanguage(series[code].languageId)
                : TVSettings.Instance.PreferredLanguage;

            List<JObject> episodePrefLangResponses = GetEpisodes(code, serieLang);
            List<JObject> episodeDefaultLangResponses = null;
            if (InForeignLanguage(serieLang)) episodeDefaultLangResponses = GetEpisodes(code, DefaultLanguage);

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
                        ProcessEpisode(code,episodeId, prefLangEpisode, defltLangEpisode);
                    }
                }
                catch (InvalidCastException ex)
                {
                    Logger.Error(ex,"Did not recieve the expected format of json from {0}.", EpisodeUri(code));
                    Logger.Error(prefLangEpisode?.ToString());
                    Logger.Error(defltLangEpisode?.ToString());
                }
                catch (OverflowException ex)
                {
                    Logger.Error(ex,"Could not parse the episode json from {0}.", EpisodeUri(code));
                    Logger.Error(prefLangEpisode?.ToString());
                    Logger.Error(defltLangEpisode?.ToString());
                }
            });
        }

        private static Dictionary<int, Tuple<JToken, JToken>> MergeEpisodeResponses(List<JObject> episodeResponses, List<JObject> episodeDefaultLangResponses)
        {
            Dictionary<int, Tuple<JToken, JToken>> episodeIds = new Dictionary<int, Tuple<JToken, JToken>>();

            if(episodeResponses!=null) foreach (JObject epResponse in episodeResponses)
            {
                foreach (JToken episodeData in epResponse["data"])
                {
                    int x = (int)episodeData["id"];
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
            if (episodeDefaultLangResponses != null) foreach (JObject epResponse in episodeDefaultLangResponses)
            {
                foreach (JToken episodeData in epResponse["data"])
                {
                    int x = (int)episodeData["id"];
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

            return episodeIds;
        }

        private bool InForeignLanguage() => DefaultLanguage != TVSettings.Instance.PreferredLanguage;
        private bool InForeignLanguage(string lang) => DefaultLanguage != lang;

        private bool DownloadEpisodeNow(int seriesId, int episodeId, bool dvdOrder = false)
        {
            if (series.ContainsKey(seriesId))
            {
                Episode ep = FindEpisodeById(episodeId);
                string eptxt = EpisodeDescription(dvdOrder, episodeId, ep);
                Say(series[seriesId].Name + " (" + eptxt + ")");
            }
            else
                return false; // shouldn't happen

            string serieLang = series.ContainsKey(seriesId)
                ? GetLanguage(series[seriesId].languageId)
                : TVSettings.Instance.PreferredLanguage;

            string uri = TvDbTokenProvider.TVDB_API_URL + "/episodes/" + episodeId;
            JObject jsonEpisodeResponse;
            JObject jsonEpisodeDefaultLangResponse = new JObject();

            try
            {
                jsonEpisodeResponse = HttpHelper.JsonHttpGetRequest(uri, null, tvDbTokenProvider.GetToken(),
                    serieLang);

                if (InForeignLanguage(serieLang))
                    jsonEpisodeDefaultLangResponse =
                        HttpHelper.JsonHttpGetRequest(uri, null, tvDbTokenProvider.GetToken(), DefaultLanguage);
            }
            catch (WebException ex)
            {
                Logger.Error("Error obtaining " + uri + ": " + ex.Message);
                LastError = ex.Message;
                Say("");
                return false;
            }

            if (!GetLock("ProcessTVDBResponse"))
                return false;

            try
            {
                Episode e;
                JObject jsonResponseData = (JObject)jsonEpisodeResponse["data"];

                if (InForeignLanguage())
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
            }
            catch (TVDBException e)
            {
                Logger.Error("Could not parse TVDB Response " + e.Message);
                LastError = e.Message;
                Say("");
                return false;
            }
            finally
            {
                Unlock("ProcessTVDBResponse");
            }

            return true;
        }

        private bool ProcessEpisode(int seriesId,int episodeId, JToken prefLangEpisodeData, JToken defLangEpisodeData,
            bool dvdOrder = false)
        {
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

            if (!GetLock("ProcessTVDBResponse"))
                return false;

            try
            {
                Episode e= new Episode(seriesId, (JObject)prefLangEpisodeData, (JObject)defLangEpisodeData);

                if (e.Ok())
                {
                    AddOrUpdateEpisode(e);
                }
            }
            catch (TVDBException e)
            {
                Logger.Error("Could not parse TVDB Response " + e.Message);
                LastError = e.Message;
                Say("");
                return false;
            }
             finally
            {
                Unlock("ProcessTVDBResponse");
            }
            return true;
        }

        private bool ProcessEpisode(int seriesId, JToken episodeData, bool dvdOrder = false)
        {
            if (series.ContainsKey(seriesId))
            {
                int episodeId = (int)episodeData["id"];

                Episode ep = FindEpisodeById(episodeId);
                string eptxt = EpisodeDescription(dvdOrder, episodeId, ep);

                Say(series[seriesId].Name + " (" + eptxt + ")");
            }
            else
                return false; // shouldn't happen

            if (!GetLock("ProcessTVDBResponse"))
                return false;

            try
            {
                Episode e = new Episode(seriesId, (JObject)episodeData);

                if (e.Ok())
                {
                    AddOrUpdateEpisode(e);
                }
            }
            catch (TVDBException e)
            {
                Logger.Error("Could not parse TVDB Response " + e.Message);
                LastError = e.Message;
                Say("");
                return false;
            }
            finally
            {
                Unlock("ProcessTVDBResponse");
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

        private void AddPlaceholderSeries(int code, string name, int langId)
        {
            series[code] = new SeriesInfo(name ?? "", code) {Dirty = true, languageId = langId};
        }

        public bool EnsureUpdated(int code, bool bannersToo)
        {
            if (DoWeForceReloadFor(code) || (series[code].AiredSeasons.Count == 0))
                return DownloadSeriesNow(code, true, bannersToo) != null; // the whole lot!

            bool ok = true;

            if ((series[code].Dirty) || (bannersToo && !series[code].BannersLoaded))
                ok = (DownloadSeriesNow(code, false, bannersToo) != null);

            foreach (KeyValuePair<int, Season> kvp in GetSeries(code)?.AiredSeasons??new Dictionary<int, Season>())
            {
                Season seas = kvp.Value;
                foreach (Episode e in seas.Episodes.Values)
                {
                    if (!e.Dirty || e.EpisodeId <= 0) continue;
                    LockExtraEpisodes();
                    extraEpisodes.Add(new ExtraEp(e.SeriesId, e.EpisodeId));
                    UnlockExtraEpisodes();
                }
            }

            LockExtraEpisodes();
            Parallel.ForEach(extraEpisodes, ee =>
            {
                if (ee.SeriesId != code || ee.Done) return;
                ok = DownloadEpisodeNow(ee.SeriesId, ee.EpisodeId) && ok;
                ee.Done = true;
            });

            UnlockExtraEpisodes();

            LockRemoveEpisodes();
            foreach (ExtraEp episodetoRemove in removeEpisodeIds)
            {
                series[episodetoRemove.SeriesId].RemoveEpisode(episodetoRemove.EpisodeId);
            }

            removeEpisodeIds.Clear();
            UnlockRemoveEpisodes();

            forceReloadOn.Remove(code);

            return ok;
        }

        public void Search(string text)
        {
            text = Helpers.RemoveDiacritics(text); // API doesn't like accented characters

            bool isNumber = Regex.Match(text, "^[0-9]+$").Success;
            if (isNumber)
                DownloadSeriesNow(int.Parse(text), false, false);

            // but, the number could also be a name, so continue searching as usual
            //text = text.Replace(".", " ");

            if (!Connected && !Connect())
            {
                Say("Failed to Connect");
                return;
            }

            string uri = TvDbTokenProvider.TVDB_API_URL + "/search/series";
            JObject jsonSearchResponse = null;
            JObject jsonSearchDefaultLangResponse = null;
            try
            {
                jsonSearchResponse = HttpHelper.JsonHttpGetRequest(uri, new Dictionary<string, string> {{"name", text}},
                    tvDbTokenProvider.GetToken(), TVSettings.Instance.PreferredLanguage);
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse) ex.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    Logger.Info(
                        $"Could not find any earch results for {text} in {TVSettings.Instance.PreferredLanguage}");
                }
                else
                {
                    Logger.Error("Error obtaining " + uri + ": " + ex.Message);
                    LastError = ex.Message;
                    Say("");
                }
            }

            if (InForeignLanguage())
            { 
                try
                {
                    jsonSearchDefaultLangResponse = HttpHelper.JsonHttpGetRequest(uri,
                        new Dictionary<string, string> {{"name", text}}, tvDbTokenProvider.GetToken(), DefaultLanguage);
                }
                catch (WebException ex)
                {
                    if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
                    {
                        Logger.Info(
                            $"Could not find any earch results for {text} in {DefaultLanguage}");
                    }
                    else
                    {
                        Logger.Error("Error obtaining " + uri + ": " + ex.Message);
                        LastError = ex.Message;
                        Say("");
                    }
                }
            }

            if (GetLock("ProcessTVDBResponse"))
            {
                if (jsonSearchResponse != null) ProcessSearchResult(uri, jsonSearchResponse,GetLanguageId());

                if (jsonSearchDefaultLangResponse != null)
                    //we also want to search for search terms that match in default language
                    ProcessSearchResult(uri, jsonSearchDefaultLangResponse,GetDefaultLanguageId());
            }

            Unlock("ProcessTVDBResponse");
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
                    if (series.ContainsKey(si.TvdbCode))
                        series[si.TvdbCode].Merge(si, series[si.TvdbCode].languageId);
                    else
                        series[si.TvdbCode] = si;
                }
            }
            catch (InvalidCastException ex)
            {
                Logger.Error("Did not recieve the expected format of json from {0}.", uri);
                Logger.Error(ex);
                Logger.Error(jsonResponse["data"].ToString());
            }
        }

        public string WebsiteUrl(int seriesId, int seasonId, bool summaryPage)
        {
            // Summary: http://www.thetvdb.com/?tab=series&id=75340&lid=7
            // Season 3: http://www.thetvdb.com/?tab=season&seriesid=75340&seasonid=28289&lid=7

            if (summaryPage || (seasonId <= 0) || !series.ContainsKey(seriesId))
                return WebsiteRoot + "/?tab=series&id=" + seriesId;
            else
                return WebsiteRoot + "/?tab=season&seriesid=" + seriesId + "&seasonid=" + seasonId;
        }

        public string WebsiteUrl(int seriesId, int seasonId, int episodeId)
        {
            // http://www.thetvdb.com/?tab=episode&seriesid=73141&seasonid=5356&id=108303&lid=7
            return WebsiteRoot + "/?tab=episode&seriesid=" + seriesId + "&seasonid=" + seasonId + "&id=" + episodeId;
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
            GetLock("TidyTVDB");
            List<int> removeList = new List<int>();

            foreach (KeyValuePair<int, SeriesInfo> kvp in GetSeriesDict())
            {
                bool found = libraryValues.Any(si => si.TvdbCode == kvp.Key);
                if (!found)
                    removeList.Add(kvp.Key);
            }

            foreach (int i in removeList)
                ForgetShow(i, false);

            Unlock("TheTVDB");
            SaveCache();
        }
    }
}

