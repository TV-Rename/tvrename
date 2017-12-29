// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using Newtonsoft.Json.Linq;
using NLog;
using File = Alphaleonis.Win32.Filesystem.File;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using Timer = System.Timers.Timer;

// Talk to the TheTVDB web API, and get tv series info

// Hierarchy is:
//   TheTVDB -> Series (class SeriesInfo) -> Seasons (class Season) -> Episodes (class Episode)

namespace TVRename
{

    public class TVDBException : Exception
    {
        // Thrown if an error occurs in the XML when reading TheTVDB.xml
        public TVDBException(String message)
            : base(message)
        {
        }
    }

    public static class KeepTVDBAliveTimer
    {
        static Timer _timer; // From System.Timers

        static public void Start()
        {
            _timer = new Timer(23 * 60 * 60 * 1000); // Set up the timer for 23 hours 
            _timer.Elapsed += _timer_Elapsed;
            _timer.Enabled = true; // Enable it
        }

        static void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TheTVDB.Instance.RefreshToken();
        }
    }

    public class Language
    {
        public Language(int id, string abbreviation, string name, string englishName) {
            Id = id;
            Abbreviation = abbreviation;
            Name = name;
            EnglishName = englishName;

            
        }
        public int Id { get; set;}
        public string Abbreviation { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
    }

    public class TheTVDB
    {
        public string WebsiteRoot;
        private FileInfo _cacheFile;
        public bool Connected;
        public string CurrentDlTask;
        private List<ExtraEp> _extraEpisodes; // IDs of extra episodes to grab and merge in on next update
        private List<int> _forceReloadOn;
        public List<Language> LanguageList;
        public string LastError;
        public string LoadErr;
        public bool LoadOk;
        private long _newSrvTime;
        private Dictionary<int, SeriesInfo> _series; // TODO: make this private or a property. have online/offline state that controls auto downloading of needed info.
        private long _srvTime; // only update this after a 100% successful download
        // private List<String> WhoHasLock;
        private string _apiRoot;
        private string _authenticationToken; //The JSON Web token issued by TVDB

        public String RequestLanguage = "en"; // Set and updated by TVDoc
        private static readonly String _defaultLanguage = "en"; //Default backup language

        private CommandLineArgs _args;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();


        //We are using the singleton design pattern
        //http://msdn.microsoft.com/en-au/library/ff650316.aspx

        private static volatile TheTVDB _instance;
        private static readonly object _syncRoot = new Object();

        public static TheTVDB Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new TheTVDB();
                    }
                }

                return _instance;
            }
        }

        public void Setup(FileInfo loadFrom, FileInfo cacheFile, CommandLineArgs args)
        {
            _args = args;

            Debug.Assert(cacheFile != null);
            _cacheFile = cacheFile;

            LastError = "";
            // this.WhoHasLock = new List<String>();
            Connected = false;
            _extraEpisodes = new List<ExtraEp>();

            LanguageList = new List<Language> {new Language(7, "en", "English", "English")};

            WebsiteRoot = "http://thetvdb.com";
            _apiRoot = "https://api.thetvdb.com";

            _series = new Dictionary<int, SeriesInfo>();
            _newSrvTime = _srvTime = 0;

            LoadOk = (loadFrom == null) || LoadCache(loadFrom);

            _forceReloadOn = new List<int>();
        }

        private void LockEe()
        {
            Monitor.Enter(_extraEpisodes);
        }

        private void UnlockEe()
        {
            Monitor.Exit(_extraEpisodes);
        }

        public bool HasSeries(int id)
        {
            return _series.ContainsKey(id);
        }

        public SeriesInfo GetSeries(int id)
        {
            if (!HasSeries(id))
                return null;

            return _series[id];
        }

        public Dictionary<int, SeriesInfo> GetSeriesDict()
        {
            return _series;
        }

        public bool GetLock(string whoFor)
        {
            _logger.Trace("Lock Series for " + whoFor);
            bool ok = Monitor.TryEnter(_series, 10000);
            Debug.Assert(ok);
            return ok;
            //            WhoHasLock->Add(whoFor);
        }

        public void Unlock(string whoFor)
        {
            //return;

            //            int n = WhoHasLock->Count - 1;
            //            String ^whoHad = WhoHasLock[n];
            //#if defined(DEBUG)
            //            System.Diagnostics::Debug::Assert(whoFor == whoHad);
            //#endif
            _logger.Trace("Unlock series for (" + whoFor + ")");
            // WhoHasLock->RemoveAt(n);
            //
            Monitor.Exit(_series);
        }

        private void Say(string s)
        {
            CurrentDlTask = s;
            _logger.Info("Status on screen updated: {0}",s);
        }

        public bool LoadCache(FileInfo loadFrom)
        {
            _logger.Info("Loading Cache from: {0}", loadFrom.FullName);
            if ((loadFrom == null) || !loadFrom.Exists)
                return true; // that's ok

            FileStream fs = null;
            try
            {
                fs = loadFrom.Open(FileMode.Open);
                bool r = ProcessXML(fs, null);
                fs.Close();
                fs = null;
                if (r)
                    UpdatesDoneOk();
                return r;
            }
            catch (Exception e)
            {
                LoadErr = loadFrom.Name + " : " + e.Message;

                fs?.Close();

                return false;
            }
        }

        public void UpdatesDoneOk()
        {
            // call when all downloading and updating is done.  updates local Srv_Time with the tentative
            // new_srv_time value.
            _srvTime = _newSrvTime;
        }

        public void SaveCache()
        {
            _logger.Info("Saving Cache to: {0}", _cacheFile.FullName);
            if (!GetLock("SaveCache"))
                return;

            if (_cacheFile.Exists)
            {
                double hours = 999.9;
                if (File.Exists(_cacheFile.FullName + ".0"))
                {
                    // see when the last rotate was, and only rotate if its been at least an hour since the last save
                    DateTime dt = File.GetLastWriteTime(_cacheFile.FullName + ".0");
                    hours = DateTime.Now.Subtract(dt).TotalHours;
                }
                if (hours >= 24.0) // rotate the save file daily
                {
                    for (int i = 8; i >= 0; i--)
                    {
                        string fn = _cacheFile.FullName + "." + i;
                        if (File.Exists(fn))
                        {
                            string fn2 = _cacheFile.FullName + "." + (i + 1);
                            if (File.Exists(fn2))
                                File.Delete(fn2);
                            File.Move(fn, fn2);
                        }
                    }

                    File.Copy(_cacheFile.FullName, _cacheFile.FullName + ".0");
                }
            }

            // write ourselves to disc for next time.  use same structure as thetvdb.com (limited fields, though)
            // to make loading easy
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true
            };
            using (XmlWriter writer = XmlWriter.Create(_cacheFile.FullName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Data");
                XMLHelper.WriteAttributeToXML(writer, "time", _srvTime);

                foreach (KeyValuePair<int, SeriesInfo> kvp in _series)
                {
                    if (kvp.Value.SrvLastUpdated != 0)
                    {
                        kvp.Value.WriteXml(writer);
                        foreach (KeyValuePair<int, Season> kvp2 in kvp.Value.Seasons)
                        {
                            Season seas = kvp2.Value;
                            foreach (Episode e in seas.Episodes)
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

                foreach (KeyValuePair<int, SeriesInfo> kvp in _series)
                {
                    writer.WriteStartElement("BannersItem");

                    XMLHelper.WriteElementToXML(writer, "SeriesId", kvp.Key);

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
                writer.Close();
            }
            Unlock("SaveCache");
        }

        public SeriesInfo FindSeriesForName(string showName)
        {
            Search(showName);

            if (string.IsNullOrEmpty(showName))
                return null;

            showName = showName.ToLower();

            foreach (KeyValuePair<int, SeriesInfo> ser in _series)
            {
                if (ser.Value.Name.ToLower() == showName)
                    return ser.Value;
            }

            return null;
        }

        public Episode FindEpisodeById(int id)
        {
            if (!GetLock("FindEpisodeByID"))
                return null;

            foreach (KeyValuePair<int, SeriesInfo> kvp in _series.ToList())
            {
                foreach (KeyValuePair<int, Season> kvp2 in kvp.Value.Seasons)
                {
                    Season seas = kvp2.Value;
                    foreach (Episode e in seas.Episodes)
                    {
                        if (e.EpisodeId == id)
                        {
                            Unlock("FindEpisodeByID");
                            return e;
                        }
                    }
                }
            }
            Unlock("FindEpisodeByID");
            return null;
        }

        public bool Connect()
        {
            Connected = Login() && GetLanguages();
            return Connected;
        }

        public static string BuildUrl(bool withHttpAndKey, bool episodesToo, int code, string lang)
        //would rather make this private to hide api key from outside world
        {
            string r = withHttpAndKey ? "http://thetvdb.com/api/" + ApiKey() + "/" : "";
            r += episodesToo ? "series/" + code + "/all/" + lang + ".zip" : "series/" + code + "/" + lang + ".xml";
            return r;
        }

        private static string ApiKey()
        {
            return "5FEC454623154441"; // tvrename's API key on thetvdb.com
        }


        public byte[] GetTVDBDownload(string url, bool forceReload = false) {
            string mirr = WebsiteRoot;

            if (url.StartsWith("/"))
                url = url.Substring(1);
            if (!mirr.EndsWith("/"))
                mirr += "/";

            string theUrl = mirr;
            theUrl += "banners/";
            theUrl += url;


            WebClient wc = new WebClient();

            if (forceReload)
                wc.CachePolicy = new RequestCachePolicy(RequestCacheLevel.Reload);

            try
            {
                byte[] r = wc.DownloadData(theUrl);
                //HttpWebResponse ^wres = dynamic_cast<HttpWebResponse ^>(wr->GetResponse());
                //Stream ^str = wres->GetResponseStream();
                //array<unsigned char> ^r = gcnew array<unsigned char>((int)str->Length);
                //str->Read(r, 0, (int)str->Length);

                if (!url.EndsWith(".zip"))
                   _logger.Info("Downloaded " + theUrl + ", " + r.Length + " bytes");

                return r;
            }
            catch (WebException e)
            {
               _logger.Warn(CurrentDlTask + " : " + e.Message + " : " + theUrl);
                LastError = CurrentDlTask + " : " + e.Message;
                return null;
            }
        }

        public void ForgetEverything()
        {
            if (!GetLock("ForgetEverything"))
                return;

            _series.Clear();
            Connected = false;
            SaveCache();
            Unlock("ForgetEverything");

            //All series will be forgotten and will be fully refreshed, so we'll only need updates after this point
            _newSrvTime = DateTime.UtcNow.ToUnixTime();
        }

        public void ForgetShow(int id, bool makePlaceholder)
        {
            if (!GetLock("ForgetShow"))
                return;

            if (_series.ContainsKey(id))
            {
                string name = _series[id].Name;
                _series.Remove(id);
                if (makePlaceholder)
                {
                    MakePlaceholderSeries(id, name);
                    _forceReloadOn.Add(id);
                }
            }
            Unlock("ForgetShow");
        }

        private bool GetLanguages()
        {
            Say("TheTVDB Languages");

            JObject jsonResponse = HttpHelper.JsonHttpgetRequest(_apiRoot + "/languages", null, _authenticationToken);

            LanguageList.Clear();

            foreach (JObject languageJson in jsonResponse["data"]) {
                int id = (int)languageJson["id"];
                string name = (string)languageJson["name"];
                string englishName = (string)languageJson["englishName"];
                string abbrev = (string)languageJson["abbreviation"];

                if ((id != -1) && (!string.IsNullOrEmpty(name)) && (!string.IsNullOrEmpty(abbrev)))
                    LanguageList.Add(new Language(id, abbrev, name, englishName));

            }

            Say("");
            return true;
        }

        private bool Login()
        {
            Say("Connecting to TVDB");

            JObject request = new JObject(new JProperty("apikey", ApiKey()));

            JObject jsonResponse = HttpHelper.JsonHttppostRequest(_apiRoot + "/login", request);

            _authenticationToken = (string)jsonResponse["token"];

            KeepTVDBAliveTimer.Start();

            Say("");
            return true;

        }

        public bool RefreshToken()
        {
            
            Say("Reconnecting to TVDB");


            JObject jsonResponse = HttpHelper.JsonHttpgetRequest(_apiRoot + "/refresh_token", null, _authenticationToken);

            _authenticationToken = (string)jsonResponse["token"];

            _logger.Info("refreshed token at " + DateTime.UtcNow);
            _logger.Info("New Token " + _authenticationToken);
            Say("");
            return true;
        }


        public bool GetUpdates()
        {
            Say("Updates list");

            if (!Connected && !Connect())
            {
                Say("");
                return false;
            }

            long theTime = _srvTime;

            if (theTime == 0)
            {
                // we can use the oldest thing we have locally.  It isn't safe to use the newest thing.
                // This will only happen the first time we do an update, so a false _all update isn't too bad.
                foreach (KeyValuePair<int, SeriesInfo> kvp in _series)
                {
                    SeriesInfo ser = kvp.Value;
                    if ((theTime == 0) || ((ser.SrvLastUpdated != 0) && (ser.SrvLastUpdated < theTime)))
                        theTime = ser.SrvLastUpdated;
                    foreach (KeyValuePair<int, Season> kvp2 in kvp.Value.Seasons)
                    {
                        Season seas = kvp2.Value;

                        foreach (Episode e in seas.Episodes)
                        {
                            if ((theTime == 0) || ((e.SrvLastUpdated != 0) && (e.SrvLastUpdated < theTime)))
                                theTime = e.SrvLastUpdated;
                        }
                    }
                }
            }

            // anything with a srv_lastupdated of 0 should be marked as dirty
            // typically, this'll be placeholder series
            foreach (KeyValuePair<int, SeriesInfo> kvp in _series)
            {
                SeriesInfo ser = kvp.Value;
                if ((ser.SrvLastUpdated == 0) || (ser.Seasons.Count == 0))
                    ser.Dirty = true;
                foreach (KeyValuePair<int, Season> kvp2 in kvp.Value.Seasons)
                {
                    foreach (Episode ep in kvp2.Value.Episodes)
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

            String uri = _apiRoot + "/updated/query";

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
                if ((now-requestedTime).TotalDays  < 7)  { moreUpdates = false; }

                try
                {
                    jsonUdpateResponse = HttpHelper.JsonHttpgetRequest(uri, new Dictionary<string, string> { { "fromTime", epochTime.ToString() } }, _authenticationToken, TVSettings.Instance.PreferredLanguage);

                }
                catch (WebException ex)
                {
                    _logger.Warn("Error obtaining " + uri + ": from lastupdated query -since(local) " + Helpers.FromUnixTime(epochTime).ToLocalTime());
                    _logger.Warn(ex);
                    Say("");
                    LastError = ex.Message;
                    return false;
                }

                int numberOfResponses;
                try
                {
                    numberOfResponses = ((JArray)jsonUdpateResponse["data"]).Count;

                }
                catch (InvalidCastException ex) {
                    
                    Say("");
                    LastError = ex.Message;

                    String msg = "Unable to get latest updates from TVDB " + Environment.NewLine + "Trying to get updates since " + Helpers.FromUnixTime(epochTime).ToLocalTime() + Environment.NewLine + Environment.NewLine + "If the date is very old, please consider a full refresh";
                    _logger.Warn("Error obtaining " + uri + ": from lastupdated query -since(local) " + Helpers.FromUnixTime(epochTime).ToLocalTime());
                    _logger.Warn(ex,msg);

                    if ((!_args.Unattended) && (!_args.Hide))  MessageBox.Show(msg, "Error obtaining updates from TVDB", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return true;
                }


                updatesResponses.Add(jsonUdpateResponse);
                numberofCallsMade++;

                IEnumerable<long> updateTimes = from a in jsonUdpateResponse["data"] select (long)a["lastUpdated"];
                long? maxUpdateTime = updateTimes.Max();
                if (maxUpdateTime != null)
                {
                    _newSrvTime =  Math.Max(_newSrvTime,Math.Max( (long)maxUpdateTime, _srvTime)); // just in case the new update time is no better than the prior one

                    _logger.Info("Obtianed " + numberOfResponses + " responses from lastupdated query #" + numberofCallsMade + " - since (local) " + Helpers.FromUnixTime(epochTime).ToLocalTime() + " - to (local) " + Helpers.FromUnixTime(_newSrvTime).ToLocalTime());
                    epochTime = _newSrvTime;
                }

                //As a safety measure we check that no more than 10 calls are made
                if (numberofCallsMade > 10) {
                    moreUpdates = false;
                    String errorMessage = "We have run 10 weeks of updates but it appears the system may need to check again once this set have been processed." + Environment.NewLine + "Last Updated time was " + Helpers.FromUnixTime(_srvTime).ToLocalTime() + Environment.NewLine + "New Last Updated time is " + Helpers.FromUnixTime(_newSrvTime).ToLocalTime() + Environment.NewLine + Environment.NewLine + "If the dates keep getting closer then keep getting 10 weeks of updates, otherwise consider a full refresh";
                    _logger.Warn(errorMessage);
                    if ((!_args.Unattended) && (!_args.Hide))  MessageBox.Show(errorMessage , "Long Running Update", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }


            }


            Say("Processing Updates from TVDB");

            foreach (JObject jsonResponse in updatesResponses)
            {








                // if updatetime > localtime for item, then remove it, so it will be downloaded later

                foreach (JObject series in jsonResponse["data"])
                {

                    int id = (int)series["id"];
                    int time = (int)series["lastUpdated"];

                    if (_series.ContainsKey(id)) // this is a series we have
                    {
                        if (time > _series[id].SrvLastUpdated) // newer version on the server
                            _series[id].Dirty = true; // mark as dirty, so it'll be fetched again later
                        else
                            _logger.Info(_series[id].Name + " has a lastupdated of  " + Helpers.FromUnixTime(_series[id].SrvLastUpdated) + " server says " + Helpers.FromUnixTime(time));

                        //now we wish to see if any episodes from the series have been updated. If so then mark them as dirty too

                        //Now deal with obtaining any episodes for the series 
                        //tvDB only gives us responses in blocks of 100, so we need to iterate over the pages until we get one with <100 rows
                        //We push the results into a bag to use later
                        //If there is a problem with the while method then we can be proactive by using /series/{id}/episodes/summary to get the total
                        List<JObject> episodeResponses = new List<JObject>();

                        int pageNumber = 1;
                        bool morePages = true;


                        while (morePages)
                        {
                            String episodeUri = _apiRoot + "/series/" + id + "/episodes";
                            JObject jsonEpisodeResponse;
                            try
                            {
                                jsonEpisodeResponse = HttpHelper.JsonHttpgetRequest(episodeUri, new Dictionary<string, string> { { "page", pageNumber.ToString() } }, _authenticationToken);
                                episodeResponses.Add(jsonEpisodeResponse);
                                int numberOfResponses = ((JArray)jsonEpisodeResponse["data"]).Count;

                                _logger.Info("Page " + pageNumber + " of " + _series[id].Name + " had " + numberOfResponses + " episodes listed");
                                if (numberOfResponses < 100) { morePages = false; } else { pageNumber++; }


                            }
                            catch (WebException ex)
                            {
                                _logger.Info("Error obtaining page " + pageNumber + " of " + episodeUri + ": " + ex.Message);
                                //There may be exactly 100 or 200 episodes, may not be a problem
                                morePages = false;
                            }
                        }
                        int numberOfNewEpisodes = 0;
                        int numberOfUpdatedEpisodes = 0;

                        foreach (JObject response in episodeResponses)
                        {
                            foreach (JObject episodeData in response["data"])
                            {
                                long serverUpdateTime = (int)episodeData["lastUpdated"];
                                int serverEpisodeId = (int)episodeData["id"];

                                bool found = false;
                                foreach (KeyValuePair<int, Season> kvp2 in _series[id].Seasons)
                                {
                                    Season seas = kvp2.Value;

                                    foreach (Episode ep in seas.Episodes)
                                    {
                                        if (ep.EpisodeId == serverEpisodeId)
                                        {
                                            if (ep.SrvLastUpdated < serverUpdateTime) {
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
                                    LockEe();
                                    _extraEpisodes.Add(new ExtraEp(id, serverEpisodeId));
                                    UnlockEe();
                                    numberOfNewEpisodes++;
                                }

                            }
                        }
                        _logger.Info(_series[id].Name + " had " + numberOfUpdatedEpisodes + " episodes updated and " + numberOfNewEpisodes + " new episodes ");
                    }
                }
            }

            Say("");

            return true;
        }

        private void ProcessXMLBannerCache(XmlReader r)
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
                    ProcessXMLBanner(r.ReadSubtree(), seriesId);
                    r.Read();
                    seriesId = -1;
                }
                else
                    r.Read();
            }

        }

        private void ProcessXMLBanner(XmlReader r, int? codeHint)
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
                    Banner b = new Banner(null, null, codeHint, r.ReadSubtree(), _args);

                    if (!_series.ContainsKey(b.SeriesId))
                        throw new TVDBException("Can't find the series to add the banner to (TheTVDB).");

                    SeriesInfo ser = _series[b.SeriesId];

                    ser.AddOrUpdateBanner(b);

                    r.Read();

                } else if ((r.Name == "Banners") && !r.IsStartElement()) {
                    _series[(int)codeHint].BannersLoaded = true;
                    break; // that's it.
                }

                else if (r.Name == "xml")
                    r.Read();
                else
                    r.Read();
            }

        }
        private int GetLanguageId() {
            foreach (Language l in LanguageList) { 
                if (l.Abbreviation == RequestLanguage) return l.Id;
            }

            return -1;
        }
        private int GetDefaultLanguageId()
        {
            foreach (Language l in LanguageList)
            {
                if (l.Abbreviation == _defaultLanguage) return l.Id;
            }

            return -1;
        }
        private bool ProcessXML(Stream str, int? codeHint)
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
                XmlReader r = XmlReader.Create(str, settings);

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
                        if (_series.ContainsKey(si.TVDBCode))
                            _series[si.TVDBCode].Merge(si, GetLanguageId());
                        else
                            _series[si.TVDBCode] = si;
                        r.Read();
                    }
                    else if (r.Name == "Episode")
                    {
                        Episode e = new Episode(null, null, r.ReadSubtree(), _args);
                        if (e.Ok())
                        {
                            if (!_series.ContainsKey(e.SeriesId))
                                throw new TVDBException("Can't find the series to add the episode to (TheTVDB).");
                            SeriesInfo ser = _series[e.SeriesId];
                            Season seas = ser.GetOrAddSeason(e.ReadSeasonNum, e.SeasonId);

                            bool added = false;
                            for (int i = 0; i < seas.Episodes.Count; i++)
                            {
                                Episode ep = seas.Episodes[i];
                                if (ep.EpisodeId == e.EpisodeId)
                                {
                                    seas.Episodes[i] = e;
                                    added = true;
                                    break;
                                }
                            }
                            if (!added)
                                seas.Episodes.Add(e);
                            e.SetSeriesSeason(ser, seas);
                        }
                        r.Read();
                    }
                    else if (r.Name == "xml")
                        r.Read();
                    else if (r.Name == "BannersCache")
                    {
                        //this wil not be found in a standard response from the TVDB website
                        //will only be in the response when we are reading from the cache
                        ProcessXMLBannerCache(r);
                        r.Read();
                    }
                    else if (r.Name == "Data")
                    {
                        string time = r.GetAttribute("time");
                        if (time != null)
                            _newSrvTime = long.Parse(time);
                        r.Read();
                    }
                    else
                        r.ReadOuterXml();
                }
            }
            catch (XmlException e)
            {


                str.Position = 0;
                StreamReader sr = new StreamReader(str);
                string myStr = sr.ReadToEnd();

                string message = "Error processing data from TheTVDB (top level).";
                message += "\r\n" + myStr;
                message += "\r\n" + e.Message;
                String name = "";
                if (codeHint.HasValue && _series.ContainsKey(codeHint.Value))
                {
                    name += "Show \"" + _series[codeHint.Value].Name + "\" ";
                }
                if (codeHint.HasValue)
                {
                    name += "ID #" + codeHint.Value + " ";
                }

                _logger.Error(name + message);
                _logger.Error(str.ToString());
                throw new TVDBException(name + message);
                return false;
            }
            finally
            {
                Unlock("ProcessTVDBResponse");
            }
            return true;
        }

        public bool DoWeForceReloadFor(int code)
        {
            return _forceReloadOn.Contains(code) || !_series.ContainsKey(code);
        }

        private SeriesInfo DownloadSeriesNow(int code, bool episodesToo, bool bannersToo)
        {
            bool forceReload = DoWeForceReloadFor(code);

            string txt;
            if (_series.ContainsKey(code))
                txt = _series[code].Name;
            else
                txt = "Code " + code;

            if (episodesToo)
                txt += " (Everything)";
            else
                txt += " Overview";

            if (bannersToo)
                txt += " plus banners";

            Say(txt);

            String uri = _apiRoot + "/series/" + code;
            JObject jsonResponse ;
            JObject jsonDefaultLangResponse = new JObject();
            try
            {
                jsonResponse = HttpHelper.JsonHttpgetRequest(uri, null, _authenticationToken, TVSettings.Instance.PreferredLanguage);

                if (InForeignLanguage()) jsonDefaultLangResponse = HttpHelper.JsonHttpgetRequest(uri, null, _authenticationToken, _defaultLanguage );
            }
            catch (WebException ex)
            {
                _logger.Error("Error obtaining {0}", uri);
                _logger.Error (ex);
                Say("");
                LastError = ex.Message;
                return null;
            }

            SeriesInfo si;
            JObject seriesData = (JObject)jsonResponse["data"];

            if (InForeignLanguage()) {
                JObject seriesDataDefaultLang = (JObject)jsonDefaultLangResponse["data"];
                si = new SeriesInfo(seriesData,seriesDataDefaultLang, GetLanguageId());
            }
            else
            {
                si = new SeriesInfo(seriesData, GetLanguageId());
            }

            if (_series.ContainsKey(si.TVDBCode))
                _series[si.TVDBCode].Merge(si, GetLanguageId());
            else
                _series[si.TVDBCode] = si;


            //Now deal with obtaining any episodes for the series (we then group them into seasons)
            //tvDB only gives us responses in blocks of 100, so we need to iterate over the pages until we get one with <100 rows
            //We push the results into a bag to use later
            //If there is a problem with the while method then we can be proactive by using /series/{id}/episodes/summary to get the total
            List<JObject> episodeResponses = new List<JObject>();

            if (episodesToo  || forceReload) { 
                int pageNumber = 1;
                bool morePages = true;
            

                while (morePages)
                {
                    String episodeUri = _apiRoot + "/series/" + code + "/episodes";
                    JObject jsonEpisodeResponse;
                    try
                    {
                        jsonEpisodeResponse = HttpHelper.JsonHttpgetRequest(episodeUri, new Dictionary<string, string> { { "page", pageNumber.ToString() } }, _authenticationToken);
                        episodeResponses.Add(jsonEpisodeResponse);
                        int numberOfResponses = ((JArray)jsonEpisodeResponse["data"]).Count;
                        //logger.Info(code + "****" + jsonEpisodeResponse.ToString());
                        _logger.Info("Page " + pageNumber + " of " + si.Name + " had " + numberOfResponses + " episodes listed");
                        if (numberOfResponses < 100) { morePages = false; } else { pageNumber++; }
                        

                    }
                    catch (WebException ex)
                    {
                        _logger.Info("Error obtaining page "+pageNumber +" of " + episodeUri + ": " + ex.Message);
                        //There may be exactly 100 or 200 episodes, may not be a problem
                        morePages = false;
                    }
                }
            }

            foreach (JObject response in episodeResponses)
            {
                foreach (JObject episodeData in response["data"])
                {
                    //The episode does not contain enough data (specifically image filename), so we'll get the full version
                    DownloadEpisodeNow(code, (int)episodeData["id"]);

              /* All of this code was used when we tried to create an episode from the series response. Issue was that we ended up doubling up. 
              Creating an imperfect episode and having to do a full refresh anyway.

                COmmenting it out for now

                                        Episode e = new Episode(code,episodeData);
                                        if (e.OK())
                                        {
                                            if (!this.Series.ContainsKey(e.SeriesID))
                                                throw new TVDBException("Can't find the series to add the episode to (TheTVDB).");
                                            SeriesInfo ser = this.Series[e.SeriesID];
                                            Season seas = ser.GetOrAddSeason(e.ReadSeasonNum, e.SeasonID);

                                            bool added = false;
                                            for (int i = 0; i < seas.Episodes.Count; i++)
                                            {
                                                Episode ep = seas.Episodes[i];
                                                if (ep.EpisodeID == e.EpisodeID)
                                                {
                                                    seas.Episodes[i] = e;
                                                    added = true;
                                                    break;
                                                }
                                            }
                                            if (!added)
                                                seas.Episodes.Add(e);
                                            e.SetSeriesSeason(ser, seas);

                                            //The episode does not contain enough data (specifically image filename), so we'll get the full version
                                            this.DownloadEpisodeNow(code, e.EpisodeID);

                                        }
                                           */

                }
            }


            List<JObject> bannerResponses = new List<JObject>();
            List<JObject> bannerDefaultLangResponses = new List<JObject>();
            if (bannersToo || forceReload )            {
                // get /series/id/images if the bannersToo is set - may need to make multiple calls to for each image type
                List<string> imageTypes = new List<string>();

                try
                {
                    JObject jsonEpisodeSearchResponse = HttpHelper.JsonHttpgetRequest(_apiRoot + "/series/" + code + "/images", null, _authenticationToken, TVSettings.Instance.PreferredLanguage);
                    JObject a = (JObject)jsonEpisodeSearchResponse["data"];

                    foreach (KeyValuePair<string, JToken> imageType in a)
                    {
                        if ((int)imageType.Value > 0) imageTypes.Add(imageType.Key);

                    }
                }
                catch (WebException ex) {
                    //no images for chosen language
                    _logger.Trace("No images found for {0} in language {1}", _apiRoot + "/series/" + code + "/images", TVSettings.Instance.PreferredLanguage);
                    _logger.Warn(ex.Message);

                }



                foreach (string imageType in imageTypes)
                {
                    try
                    {
                        JObject jsonImageResponse = HttpHelper.JsonHttpgetRequest(_apiRoot + "/series/" + code + "/images/query", new Dictionary<string, string> { { "keyType", imageType } }, _authenticationToken, TVSettings.Instance.PreferredLanguage);
                        bannerResponses.Add(jsonImageResponse);
                    }
                    catch (WebException webEx)
                    {
                        _logger.Info("Looking for " + imageType + " images (in local language), but none found for seriesId " + code);
                        _logger.Info(webEx);
                    }

                }
                if (InForeignLanguage())
                {
                    List<string> imageDefaultLangTypes = new List<string>();

                    try
                    {
                        JObject jsonEpisodeSearchDefaultLangResponse = HttpHelper.JsonHttpgetRequest(_apiRoot + "/series/" + code + "/images", null, _authenticationToken, _defaultLanguage);

                        JObject adl = (JObject)jsonEpisodeSearchDefaultLangResponse["data"];

                        foreach (KeyValuePair<string, JToken> imageType in adl)
                        {
                            if ((int)imageType.Value > 0) imageDefaultLangTypes.Add(imageType.Key);

                        }
                    }
                    catch (WebException ex)
                    {
                        _logger.Info("Looking for images, but none found for seriesId {0} in {1}", code ,_defaultLanguage );
                        _logger.Info(ex);

                        //no images for chosen language
                    }

                    foreach (string imageType in imageDefaultLangTypes)
                    {

                        try
                        {
                            JObject jsonImageDefaultLangResponse = HttpHelper.JsonHttpgetRequest(_apiRoot + "/series/" + code + "/images/query", new Dictionary<string, string> { { "keyType", imageType } }, _authenticationToken, _defaultLanguage);
                            bannerDefaultLangResponses.Add(jsonImageDefaultLangResponse);
                        }
                        catch (WebException webEx)
                        {
                            _logger.Info("Looking for " + imageType + " images, but none found for seriesId " + code);
                            _logger.Info(webEx);
                        }
                    }


                }


            }



            foreach (JObject response in bannerResponses )
            {
                foreach (JObject bannerData in response["data"])
                {
                    Banner b = new Banner(si.TVDBCode, bannerData,GetLanguageId());
                    if (!_series.ContainsKey(b.SeriesId)) throw new TVDBException("Can't find the series to add the banner to (TheTVDB).");
                    SeriesInfo ser = _series[b.SeriesId];
                    ser.AddOrUpdateBanner(b);
                }
            }

            foreach (JObject response in bannerDefaultLangResponses)
            {
                foreach (JObject bannerData in response["data"])
                {
                    Banner b = new Banner(si.TVDBCode, bannerData, GetDefaultLanguageId());
                    if (!_series.ContainsKey(b.SeriesId)) throw new TVDBException("Can't find the series to add the banner to (TheTVDB).");
                    SeriesInfo ser = _series[b.SeriesId];
                    ser.AddOrUpdateBanner(b);
                }
            }


            _series[si.TVDBCode].BannersLoaded = true;


            //Get the actors too then well need another call for that
            JObject jsonActorsResponse = HttpHelper.JsonHttpgetRequest(_apiRoot + "/series/" + code + "/actors", null, _authenticationToken);
            IEnumerable<string> seriesActors = from a in jsonActorsResponse["data"] select (string)a["name"];
            _series[si.TVDBCode].SetActors(seriesActors);

            _forceReloadOn.Remove(code);

            return (_series.ContainsKey(code)) ? _series[code] : null;


        }

        private bool InForeignLanguage() => !(_defaultLanguage == RequestLanguage);

        private bool DownloadEpisodeNow(int seriesId, int episodeId)
        {
            bool forceReload = _forceReloadOn.Contains(seriesId);

            string txt;
            if (_series.ContainsKey(seriesId))
            {
                Episode ep = FindEpisodeById(episodeId);
                string eptxt =  "New Episode Id = "+ episodeId;
                if (ep?.TheSeason != null)
                    eptxt = string.Format("S{0:00}E{1:00}", ep.TheSeason.SeasonNumber, ep.EpNum);

                txt = _series[seriesId].Name + " (" + eptxt + ")";
            }
            else
                return false; // shouldn't happen
            Say(txt);

            String uri = _apiRoot + "/episodes/" + episodeId;
            JObject jsonResponse;
            JObject jsonDefaultLangResponse = new JObject();

            try {
                jsonResponse = HttpHelper.JsonHttpgetRequest(uri, null, _authenticationToken, TVSettings.Instance.PreferredLanguage);

                if (InForeignLanguage()) jsonDefaultLangResponse = HttpHelper.JsonHttpgetRequest(uri, null, _authenticationToken, _defaultLanguage);
            }
            catch (WebException ex)
            {
                _logger.Error("Error obtaining " + uri + ": " + ex.Message);
                LastError = ex.Message;
                Say ("");
                return false;
            }

            
            if (!GetLock("ProcessTVDBResponse"))
                return false;

            try
            {
                Episode e; 
                JObject jsonResponseData = (JObject)jsonResponse["data"];

                if (InForeignLanguage())
                {
                    JObject seriesDataDefaultLang = (JObject)jsonDefaultLangResponse["data"];
                    e = new Episode(seriesId, jsonResponseData, seriesDataDefaultLang);
                }
                else
                {

                    e = new Episode(seriesId, jsonResponseData);
                }


                if (e.Ok())
                {
                    if (!_series.ContainsKey(e.SeriesId))
                        throw new TVDBException("Can't find the series to add the episode to (TheTVDB).");
                    SeriesInfo ser = _series[e.SeriesId];
                    Season seas = ser.GetOrAddSeason(e.ReadSeasonNum, e.SeasonId);

                    bool added = false;
                    for (int i = 0; i < seas.Episodes.Count; i++)
                    {
                        Episode ep = seas.Episodes[i];
                        if (ep.EpisodeId == e.EpisodeId)
                        {
                            seas.Episodes[i] = e;
                            added = true;
                            break;
                        }
                    }
                    if (!added)
                        seas.Episodes.Add(e);
                    e.SetSeriesSeason(ser, seas);
                }
            }
            catch (TVDBException e)
            {
               _logger.Error("Could not parse TVDB Response " + e.Message);
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

        public SeriesInfo MakePlaceholderSeries(int code, string name)
        {
            if (string.IsNullOrEmpty(name))
                name = "";
            _series[code] = new SeriesInfo(name, code);
            _series[code].Dirty = true;
            return _series[code];
        }

        public bool EnsureUpdated(int code, bool bannersToo)
        {
            if (DoWeForceReloadFor(code) || (_series[code].Seasons.Count == 0))
                return DownloadSeriesNow(code, true, bannersToo) != null; // the whole lot!

            bool ok = true;

            if ((_series[code].Dirty) || (bannersToo && !_series[code].BannersLoaded))
                ok = (DownloadSeriesNow(code, false, bannersToo) != null) && ok;

            foreach (KeyValuePair<int, Season> kvp in _series[code].Seasons)
            {
                Season seas = kvp.Value;
                foreach (Episode e in seas.Episodes)
                {
                    if (e.Dirty && e.EpisodeId >0)
                    {
                        LockEe();
                        _extraEpisodes.Add(new ExtraEp(e.SeriesId, e.EpisodeId));
                        UnlockEe();
                    }
                }
            }

            LockEe();
            foreach (ExtraEp ee in _extraEpisodes)
            {
                if ((ee.SeriesId == code) && (!ee.Done))
                {
                    ok = DownloadEpisodeNow(ee.SeriesId, ee.EpisodeId) && ok;
                    ee.Done = true;
                }
            }
            UnlockEe();

            _forceReloadOn.Remove(code);

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

            String uri = _apiRoot + "/search/series";
            JObject jsonResponse ;
            JObject jsonDefaultLangResponse = new JObject();
            try
            {
                jsonResponse = HttpHelper.JsonHttpgetRequest(uri, new Dictionary<string, string> { { "name", text } }, _authenticationToken, TVSettings.Instance.PreferredLanguage);
                if (InForeignLanguage()) jsonDefaultLangResponse = HttpHelper.JsonHttpgetRequest(uri, new Dictionary<string, string> { { "name", text } }, _authenticationToken,  _defaultLanguage);
            }
            catch (WebException ex)
            {
                _logger.Error("Error obtaining " + uri + ": " + ex.Message);
                LastError = ex.Message;
                Say("");
                return;
            }

            if (GetLock("ProcessTVDBResponse"))
            {


                foreach (JObject series in jsonResponse["data"])
                {

                    // The <series> returned by GetSeries have
                    // less info than other results from
                    // thetvdb.com, so we need to smartly merge
                    // in a <Series> if we already have some/all
                    // info on it (depending on which one came
                    // first).

                    SeriesInfo si = new SeriesInfo(series, GetLanguageId());
                    if (_series.ContainsKey(si.TVDBCode))
                        _series[si.TVDBCode].Merge(si, GetLanguageId());
                    else
                        _series[si.TVDBCode] = si;
                }
                if (InForeignLanguage())
                {
                    //we also want to search for search terms that match in default language
                    foreach (JObject series in jsonDefaultLangResponse["data"])
                    {

                        // The <series> returned by GetSeries have
                        // less info than other results from
                        // thetvdb.com, so we need to smartly merge
                        // in a <Series> if we already have some/all
                        // info on it (depending on which one came
                        // first).

                        SeriesInfo si = new SeriesInfo(series, GetLanguageId());
                        if (_series.ContainsKey(si.TVDBCode))
                            _series[si.TVDBCode].Merge(si, GetLanguageId());
                        else
                            _series[si.TVDBCode] = si;
                    }
                }
            }
            Unlock("ProcessTVDBResponse");


        }

        public string WebsiteUrl(int code, int seasid, bool summaryPage)
        {
            // Summary: http://www.thetvdb.com/?tab=series&id=75340&lid=7
            // Season 3: http://www.thetvdb.com/?tab=season&seriesid=75340&seasonid=28289&lid=7

            if (summaryPage || (seasid <= 0) || !_series.ContainsKey(code))
                return WebsiteRoot + "/?tab=series&id=" + code;
            return WebsiteRoot + "/?tab=season&seriesid=" + code + "&seasonid=" + seasid;
        }

        public string WebsiteUrl(int seriesId, int seasonId, int episodeId)
        {
            // http://www.thetvdb.com/?tab=episode&seriesid=73141&seasonid=5356&id=108303&lid=7
            return WebsiteRoot + "/?tab=episode&seriesid=" + seriesId + "&seasonid=" + seasonId + "&id=" + episodeId;
        }

        


        // Next episode to air of a given show		
        public Episode NextAiring(int code)
        {
            if (!_series.ContainsKey(code) || (_series[code].Seasons.Count == 0))
                return null; // DownloadSeries(code, true);

            Episode next = null;
            DateTime today = DateTime.Now;
            DateTime mostSoonAfterToday = new DateTime(0);

            SeriesInfo ser = _series[code];
            foreach (KeyValuePair<int, Season> kvp2 in ser.Seasons)
            {
                Season s = kvp2.Value;

                foreach (Episode e in s.Episodes)
                {
                    DateTime? adt = e.GetAirDateDt(true);
                    if (adt != null)
                    {
                        DateTime dt = (DateTime) adt;
                        if ((dt.CompareTo(today) > 0) && ((mostSoonAfterToday.CompareTo(new DateTime(0)) == 0) || (dt.CompareTo(mostSoonAfterToday) < 0)))
                        {
                            mostSoonAfterToday = dt;
                            next = e;
                        }
                    }
                }
            }

            return next;
        }
    }
}


