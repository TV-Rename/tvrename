// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;

using System.Text;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Linq;
using System.IO;
using System.Linq.Expressions;
using NLog;
using File = Alphaleonis.Win32.Filesystem.File;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

// Talk to the TheTVDB web API, and get tv series info

// Hierarchy is:
//   TheTVDB -> Series (class SeriesInfo) -> Seasons (class Season) -> Episodes (class Episode)

namespace TVRename
{

    public class TVDBException : System.Exception
    {
        // Thrown if an error occurs in the XML when reading TheTVDB.xml
        public TVDBException(String message)
            : base(message)
        {
        }
    }

    public static class KeepTVDBAliveTimer
    {
        static System.Timers.Timer _timer; // From System.Timers

        static public void Start()
        {
            _timer = new System.Timers.Timer(23 * 60 * 60 * 1000); // Set up the timer for 23 hours 
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
            _timer.Enabled = true; // Enable it
        }

        static void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TheTVDB.Instance.RefreshToken();
        }
    }

    public class Language
    {
        public Language(int id, string abbreviation, string name, string englishName) {
            this.id = id;
            this.abbreviation = abbreviation;
            this.name = name;
            this.englishName = englishName;

            
        }
        public int id { get; set;}
        public string abbreviation { get; set; }
        public string name { get; set; }
        public string englishName { get; set; }
    }

    public class TheTVDB
    {
        public string WebsiteRoot;
        private FileInfo CacheFile;
        public bool Connected;
        public string CurrentDLTask;
        private System.Collections.Generic.List<ExtraEp> ExtraEpisodes; // IDs of extra episodes to grab and merge in on next update
        private System.Collections.Generic.List<int> ForceReloadOn;
        public List<Language> LanguageList;
        public string LastError;
        public string LoadErr;
        public bool LoadOK;
        private long New_Srv_Time;
        private System.Collections.Generic.Dictionary<int, SeriesInfo> Series; // TODO: make this private or a property. have online/offline state that controls auto downloading of needed info.
        private long Srv_Time; // only update this after a 100% successful download
        // private List<String> WhoHasLock;
        private string APIRoot;
        private string authenticationToken; //The JSON Web token issued by TVDB

        public String RequestLanguage = "en"; // Set and updated by TVDoc
        private static String DefaultLanguage = "en"; //Default backup language

        private CommandLineArgs Args;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


        //We are using the singleton design pattern
        //http://msdn.microsoft.com/en-au/library/ff650316.aspx

        private static volatile TheTVDB instance;
        private static object syncRoot = new Object();

        public static TheTVDB Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new TheTVDB();
                    }
                }

                return instance;
            }
        }

        public void setup(FileInfo loadFrom, FileInfo cacheFile, CommandLineArgs args)
        {
            Args = args;

            System.Diagnostics.Debug.Assert(cacheFile != null);
            this.CacheFile = cacheFile;

            this.LastError = "";
            // this.WhoHasLock = new List<String>();
            this.Connected = false;
            this.ExtraEpisodes = new System.Collections.Generic.List<ExtraEp>();

            this.LanguageList = new List<Language>();
            this.LanguageList.Add(new Language(7,"en","English","English"));

            this.WebsiteRoot = "http://thetvdb.com";
            this.APIRoot = "https://api.thetvdb.com";

            this.Series = new System.Collections.Generic.Dictionary<int, SeriesInfo>();

            //assume that the data is up to date (this will be overridden by the value in the XML if we have a prior install)
            //If we have no prior install then the app has no shows and is by definition up-to-date
            this.New_Srv_Time = DateTime.UtcNow.ToUnixTime();

            this.Srv_Time = 0;

            this.LoadOK = (loadFrom == null) || this.LoadCache(loadFrom);

            this.ForceReloadOn = new System.Collections.Generic.List<int>();
        }

        private void LockEE()
        {
            Monitor.Enter(this.ExtraEpisodes);
        }

        private void UnlockEE()
        {
            Monitor.Exit(this.ExtraEpisodes);
        }

        public bool HasSeries(int id)
        {
            return this.Series.ContainsKey(id);
        }

        public SeriesInfo GetSeries(int id)
        {
            if (!this.HasSeries(id))
                return null;

            return this.Series[id];
        }

        public System.Collections.Generic.Dictionary<int, SeriesInfo> GetSeriesDict()
        {
            return this.Series;
        }

        public System.Collections.Generic.Dictionary<int, SeriesInfo> GetSeriesDictMatching(string testShowName)
        {
            System.Collections.Generic.Dictionary<int, SeriesInfo> matchingSeries = new System.Collections.Generic.Dictionary<int, SeriesInfo>();

            testShowName = Helpers.CompareName(testShowName);

            if (string.IsNullOrEmpty(testShowName)) return matchingSeries;

            foreach (KeyValuePair<int, SeriesInfo> kvp in this.Series)
            {
                string show = Helpers.CompareName(kvp.Value.Name);

                if (show.Contains(testShowName, StringComparison.InvariantCultureIgnoreCase))
                {
                    //We have a match
                    matchingSeries.Add( kvp.Key, kvp.Value);
                }
            }

            return matchingSeries;
        }


        public bool GetLock(string whoFor)
        {
            logger.Trace("Lock Series for " + whoFor);
            bool ok = Monitor.TryEnter(Series, 10000);
            System.Diagnostics.Debug.Assert(ok);
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
            logger.Trace("Unlock series for (" + whoFor + ")");
            // WhoHasLock->RemoveAt(n);
            //
            Monitor.Exit(Series);
        }

        private void Say(string s)
        {
            this.CurrentDLTask = s;
            logger.Info("Status on screen updated: {0}",s);
        }

        public bool LoadCache(FileInfo loadFrom)
        {
            logger.Info("Loading Cache from: {0}", loadFrom.FullName);
            if ((loadFrom == null) || !loadFrom.Exists)
                return true; // that's ok

            FileStream fs = null;
            try
            {
                fs = loadFrom.Open(FileMode.Open);
                bool r = this.ProcessXML(fs, null);
                fs.Close();
                fs = null;
                if (r)
                    this.UpdatesDoneOK();
                return r;
            }
            catch (Exception e)
            {
                this.LoadErr = loadFrom.Name + " : " + e.Message;

                if (fs != null)
                    fs.Close();

                fs = null;

                return false;
            }
        }

        public void UpdatesDoneOK()
        {
            // call when all downloading and updating is done.  updates local Srv_Time with the tentative
            // new_srv_time value.
            this.Srv_Time = this.New_Srv_Time;
        }

        public void SaveCache()
        {
            logger.Info("Saving Cache to: {0}", this.CacheFile.FullName);
            if (!this.GetLock("SaveCache"))
                return;

            if (this.CacheFile.Exists)
            {
                double hours = 999.9;
                if (File.Exists(this.CacheFile.FullName + ".0"))
                {
                    // see when the last rotate was, and only rotate if its been at least an hour since the last save
                    DateTime dt = File.GetLastWriteTime(this.CacheFile.FullName + ".0");
                    hours = DateTime.Now.Subtract(dt).TotalHours;
                }
                if (hours >= 24.0) // rotate the save file daily
                {
                    for (int i = 8; i >= 0; i--)
                    {
                        string fn = this.CacheFile.FullName + "." + i;
                        if (File.Exists(fn))
                        {
                            string fn2 = this.CacheFile.FullName + "." + (i + 1);
                            if (File.Exists(fn2))
                                File.Delete(fn2);
                            File.Move(fn, fn2);
                        }
                    }

                    File.Copy(this.CacheFile.FullName, this.CacheFile.FullName + ".0");
                }
            }

            // write ourselves to disc for next time.  use same structure as thetvdb.com (limited fields, though)
            // to make loading easy
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true
            };
            using (XmlWriter writer = XmlWriter.Create(this.CacheFile.FullName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Data");
                XMLHelper.WriteAttributeToXML(writer, "time", this.Srv_Time);

                foreach (System.Collections.Generic.KeyValuePair<int, SeriesInfo> kvp in this.Series)
                {
                    if (kvp.Value.Srv_LastUpdated != 0)
                    {
                        kvp.Value.WriteXml(writer);
                        foreach (System.Collections.Generic.KeyValuePair<int, Season> kvp2 in kvp.Value.Seasons)
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

                foreach (System.Collections.Generic.KeyValuePair<int, SeriesInfo> kvp in this.Series)
                {
                    writer.WriteStartElement("BannersItem");

                    XMLHelper.WriteElementToXML(writer, "SeriesId", kvp.Key);

                    writer.WriteStartElement("Banners");

                    //We need to write out all banners that we have in any of the collections. 

                    foreach (System.Collections.Generic.KeyValuePair<int, Banner> kvp3 in kvp.Value.AllBanners)
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
            this.Unlock("SaveCache");
        }

        public SeriesInfo FindSeriesForName(string showName)
        {
            this.Search(showName);

            if (string.IsNullOrEmpty(showName))
                return null;

            showName = showName.ToLower();

            foreach (System.Collections.Generic.KeyValuePair<int, SeriesInfo> ser in this.GetSeriesDictMatching(showName))
            {
                return ser.Value;
            }

            return null;
        }

        public Episode FindEpisodeByID(int id)
        {
            if (!this.GetLock("FindEpisodeByID"))
                return null;

            foreach (System.Collections.Generic.KeyValuePair<int, SeriesInfo> kvp in this.Series.ToList())
            {
                foreach (System.Collections.Generic.KeyValuePair<int, Season> kvp2 in kvp.Value.Seasons)
                {
                    Season seas = kvp2.Value;
                    foreach (Episode e in seas.Episodes)
                    {
                        if (e.EpisodeID == id)
                        {
                            this.Unlock("FindEpisodeByID");
                            return e;
                        }
                    }
                }
            }
            this.Unlock("FindEpisodeByID");
            return null;
        }

        public bool Connect()
        {
            this.Connected = this.Login() && this.GetLanguages();
            return this.Connected;
        }

        public static string BuildURL(bool withHttpAndKey, bool episodesToo, int code, string lang)
        //would rather make this private to hide api key from outside world
        {
            string r = withHttpAndKey ? "http://thetvdb.com/api/" + APIKey() + "/" : "";
            r += episodesToo ? "series/" + code + "/all/" + lang + ".zip" : "series/" + code + "/" + lang + ".xml";
            return r;
        }

        private static string APIKey()
        {
            return "5FEC454623154441"; // tvrename's API key on thetvdb.com
        }


        public byte[] GetTVDBDownload(string url, bool forceReload = false) {
            string mirr = this.WebsiteRoot;

            if (url.StartsWith("/"))
                url = url.Substring(1);
            if (!mirr.EndsWith("/"))
                mirr += "/";

            string theURL = mirr;
            theURL += "banners/";
            theURL += url;


            System.Net.WebClient wc = new System.Net.WebClient();

            if (forceReload)
                wc.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.Reload);

            try
            {
                byte[] r = wc.DownloadData(theURL);
                //HttpWebResponse ^wres = dynamic_cast<HttpWebResponse ^>(wr->GetResponse());
                //Stream ^str = wres->GetResponseStream();
                //array<unsigned char> ^r = gcnew array<unsigned char>((int)str->Length);
                //str->Read(r, 0, (int)str->Length);

                if (!url.EndsWith(".zip"))
                   logger.Info("Downloaded " + theURL + ", " + r.Length + " bytes");

                return r;
            }
            catch (WebException e)
            {
               logger.Warn(this.CurrentDLTask + " : " + e.Message + " : " + theURL);
                this.LastError = this.CurrentDLTask + " : " + e.Message;
                return null;
            }
        }

        public void ForgetEverything()
        {
            if (!this.GetLock("ForgetEverything"))
                return;

            this.Series.Clear();
            this.Connected = false;
            this.SaveCache();
            this.Unlock("ForgetEverything");

            //All series will be forgotten and will be fully refreshed, so we'll only need updates after this point
            this.New_Srv_Time = DateTime.UtcNow.ToUnixTime();
        }

        public void ForgetShow(int id, bool makePlaceholder)
        {
            if (!this.GetLock("ForgetShow"))
                return;

            if (this.Series.ContainsKey(id))
            {
                string name = this.Series[id].Name;
                this.Series.Remove(id);
                if (makePlaceholder)
                {
                    this.MakePlaceholderSeries(id, name);
                    this.ForceReloadOn.Add(id);
                }
            }
            this.Unlock("ForgetShow");
        }

        private bool GetLanguages()
        {
            this.Say("TheTVDB Languages");
            try
            {
                JObject jsonResponse =
                    HTTPHelper.JsonHTTPGETRequest(APIRoot + "/languages", null, this.authenticationToken);
                this.LanguageList.Clear();

                foreach (JObject languageJSON in jsonResponse["data"])
                {
                    int ID = (int)languageJSON["id"];
                    string name = (string)languageJSON["name"];
                    string englishName = (string)languageJSON["englishName"];
                    string abbrev = (string)languageJSON["abbreviation"];

                    if ((ID != -1) && (!string.IsNullOrEmpty(name)) && (!string.IsNullOrEmpty(abbrev)))
                        this.LanguageList.Add(new Language(ID, abbrev, name, englishName));

                }

                this.Say("");
                return true;
            }
            catch (WebException ex)
            {
                this.Say("ERROR OBTAINING LANGUAGES FROM TVDB");
                logger.Error(ex, "ERROR OBTAINING LANGUAGES FROM TVDB");
                this.LastError = ex.Message;
                return false;

            }


        }

        private bool Login()
        {
            this.Say("Connecting to TVDB");

            JObject request = new JObject(new JProperty("apikey", APIKey()));
            try
            {
                JObject jsonResponse = HTTPHelper.JsonHTTPPOSTRequest(APIRoot + "/login", request);
                this.authenticationToken = (string) jsonResponse["token"];
                
            }
            catch (WebException ex)
            {
                logger.Error(ex, request.ToString());
                this.Say("ERROR CONNECTING TO TVDB");
                this.LastError = ex.Message;
                return false;
            }

            KeepTVDBAliveTimer.Start();
            this.Say("");
            return true;

        }

        public bool RefreshToken()
        {
            
            this.Say("Reconnecting to TVDB");

            try
            {
                JObject jsonResponse =
                    HTTPHelper.JsonHTTPGETRequest(APIRoot + "/refresh_token", null, this.authenticationToken);

                this.authenticationToken = (string) jsonResponse["token"];
                logger.Info("refreshed token at " + System.DateTime.UtcNow);
                logger.Info("New Token " + this.authenticationToken);
                this.Say("");
                return true;

            }
            catch (WebException ex)
            {
                logger.Error(ex, "ERROR RECONNECTING TO TVDB");
                this.Say("ERROR RECONNECTING TO TVDB");
                this.LastError = ex.Message;
                return false;

            }

        }


        public bool GetUpdates()
        {
            this.Say("Updates list");

            if (!this.Connected && !this.Connect())
            {
                this.Say("");
                return false;
            }

            long theTime = this.Srv_Time;

            if (theTime == 0)
            {
                // we can use the oldest thing we have locally.  It isn't safe to use the newest thing.
                // This will only happen the first time we do an update, so a false _all update isn't too bad.
                foreach (System.Collections.Generic.KeyValuePair<int, SeriesInfo> kvp in this.Series)
                {
                    SeriesInfo ser = kvp.Value;
                    if ((theTime == 0) || ((ser.Srv_LastUpdated != 0) && (ser.Srv_LastUpdated < theTime)))
                        theTime = ser.Srv_LastUpdated;
                    foreach (System.Collections.Generic.KeyValuePair<int, Season> kvp2 in kvp.Value.Seasons)
                    {
                        Season seas = kvp2.Value;

                        foreach (Episode e in seas.Episodes)
                        {
                            if ((theTime == 0) || ((e.Srv_LastUpdated != 0) && (e.Srv_LastUpdated < theTime)))
                                theTime = e.Srv_LastUpdated;
                        }
                    }
                }
            }

            // anything with a srv_lastupdated of 0 should be marked as dirty
            // typically, this'll be placeholder series
            foreach (System.Collections.Generic.KeyValuePair<int, SeriesInfo> kvp in this.Series)
            {
                SeriesInfo ser = kvp.Value;
                if ((ser.Srv_LastUpdated == 0) || (ser.Seasons.Count == 0))
                    ser.Dirty = true;
                foreach (System.Collections.Generic.KeyValuePair<int, Season> kvp2 in kvp.Value.Seasons)
                {
                    foreach (Episode ep in kvp2.Value.Episodes)
                    {
                        if (ep.Srv_LastUpdated == 0)
                            ep.Dirty = true;
                    }
                }
            }

            if (theTime == 0)
            {
                this.Say("");
                return true; // that's it for now
            }

            long epochTime = theTime;

            String uri = APIRoot + "/updated/query";

            //We need to ask for updates in blocks of 7 days
            //We'll keep asking until we get to a date within 7 days of today 
            //(up to a maximum of 10 - if you are this far behind then you may need multiple refreshes)

            List<JObject> updatesResponses = new List<JObject>();
            
            bool moreUpdates = true;
            int numberofCallsMade = 0;

            while (moreUpdates)
            {
                JObject jsonUdpateResponse = new JObject();

                //If this date is in the last week then this needs to be the last call to the update
                DateTime requestedTime = Helpers.FromUnixTime(epochTime).ToUniversalTime();
                DateTime now = DateTime.UtcNow; 
                if ((now-requestedTime).TotalDays  < 7)  { moreUpdates = false; }

                try
                {
                    jsonUdpateResponse = HTTPHelper.JsonHTTPGETRequest(uri, new Dictionary<string, string> { { "fromTime", epochTime.ToString() } }, this.authenticationToken, TVSettings.Instance.PreferredLanguage);

                }
                catch (WebException ex)
                {
                    logger.Warn("Error obtaining " + uri + ": from lastupdated query -since(local) " + Helpers.FromUnixTime(epochTime).ToLocalTime());
                    logger.Warn(ex);
                    this.Say("");
                    this.LastError = ex.Message;
                    moreUpdates = false;
                    return false;
                }

                int numberOfResponses =0;
                try
                {
                    numberOfResponses = ((JArray)jsonUdpateResponse["data"]).Count;

                }
                catch (InvalidCastException ex) {
                    
                    this.Say("");
                    this.LastError = ex.Message;
                    moreUpdates = false;

                    String msg = "Unable to get latest updates from TVDB " + Environment.NewLine + "Trying to get updates since " + Helpers.FromUnixTime(epochTime).ToLocalTime() + Environment.NewLine + Environment.NewLine + "If the date is very old, please consider a full refresh";
                    logger.Warn("Error obtaining " + uri + ": from lastupdated query -since(local) " + Helpers.FromUnixTime(epochTime).ToLocalTime());
                    logger.Warn(ex,msg);

                    if ((!this.Args.Unattended) && (!this.Args.Hide))  MessageBox.Show(msg, "Error obtaining updates from TVDB", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return true;
                }


                updatesResponses.Add(jsonUdpateResponse);
                numberofCallsMade++;

                IEnumerable<long> updateTimes = from a in jsonUdpateResponse["data"] select (long)a["lastUpdated"];
                long? maxUpdateTime = updateTimes.Max();
                if (maxUpdateTime != null)
                {
                    this.New_Srv_Time =  Math.Max(this.New_Srv_Time,Math.Max( (long)maxUpdateTime, this.Srv_Time)); // just in case the new update time is no better than the prior one

                    logger.Info("Obtianed " + numberOfResponses + " responses from lastupdated query #" + numberofCallsMade + " - since (local) " + Helpers.FromUnixTime(epochTime).ToLocalTime() + " - to (local) " + Helpers.FromUnixTime(this.New_Srv_Time).ToLocalTime());
                    epochTime = this.New_Srv_Time;
                }

                //As a safety measure we check that no more than 10 calls are made
                if (numberofCallsMade > 10) {
                    moreUpdates = false;
                    String errorMessage = "We have run 10 weeks of updates and we are not up to date.  The system will need to check again once this set of updates have been processed." + Environment.NewLine + "Last Updated time was " + Helpers.FromUnixTime(this.Srv_Time).ToLocalTime() + Environment.NewLine + "New Last Updated time is " + Helpers.FromUnixTime(this.New_Srv_Time).ToLocalTime() + Environment.NewLine + Environment.NewLine + "If the dates keep getting more recent then let the system keep getting 10 week blocks of updates, otherwise consider a 'Force Refresh All'";
                    logger.Warn(errorMessage);
                    if ((!this.Args.Unattended) && (!this.Args.Hide))  MessageBox.Show(errorMessage , "Long Running Update", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }


            }


            this.Say("Processing Updates from TVDB");

            foreach (JObject jsonResponse in updatesResponses)
            {
                // if updatetime > localtime for item, then remove it, so it will be downloaded later
                try
                {
                    foreach (JObject series in jsonResponse["data"])
                    {

                        int id = (int) series["id"];
                        int time = (int) series["lastUpdated"];

                        if (this.Series.ContainsKey(id)) // this is a series we have
                        {
                            if (time > this.Series[id].Srv_LastUpdated) // newer version on the server
                                this.Series[id].Dirty = true; // mark as dirty, so it'll be fetched again later
                            else
                                logger.Info(this.Series[id].Name + " has a lastupdated of  " +
                                            Helpers.FromUnixTime(this.Series[id].Srv_LastUpdated) + " server says " +
                                            Helpers.FromUnixTime(time));

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
                                String episodeUri = APIRoot + "/series/" + id + "/episodes";
                                JObject jsonEpisodeResponse;
                                try
                                {
                                    jsonEpisodeResponse = HTTPHelper.JsonHTTPGETRequest(episodeUri,
                                        new Dictionary<string, string> {{"page", pageNumber.ToString()}},
                                        this.authenticationToken);
                                    episodeResponses.Add(jsonEpisodeResponse);
                                    int numberOfResponses = ((JArray) jsonEpisodeResponse["data"]).Count;

                                    logger.Info("Page " + pageNumber + " of " + this.Series[id].Name + " had " +
                                                numberOfResponses + " episodes listed");
                                    if (numberOfResponses < 100)
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
                                    logger.Info("Error obtaining page " + pageNumber + " of " + episodeUri + ": " +
                                                ex.Message);
                                    //There may be exactly 100 or 200 episodes, may not be a problem
                                    morePages = false;
                                }
                            }

                            int numberOfNewEpisodes = 0;
                            int numberOfUpdatedEpisodes = 0;

                            foreach (JObject response in episodeResponses)
                            {
                                try
                                {
                                    foreach (JObject episodeData in response["data"])
                                    {
                                        long serverUpdateTime = (int) episodeData["lastUpdated"];
                                        int serverEpisodeId = (int) episodeData["id"];

                                        bool found = false;
                                        foreach (System.Collections.Generic.KeyValuePair<int, Season> kvp2 in this
                                            .Series[id].Seasons)
                                        {
                                            Season seas = kvp2.Value;

                                            foreach (Episode ep in seas.Episodes)
                                            {
                                                if (ep.EpisodeID == serverEpisodeId)
                                                {
                                                    if (ep.Srv_LastUpdated < serverUpdateTime)
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
                                            this.LockEE();
                                            this.ExtraEpisodes.Add(new ExtraEp(id, serverEpisodeId));
                                            this.UnlockEE();
                                            numberOfNewEpisodes++;
                                        }

                                    }
                                }
                                catch (InvalidCastException ex)
                                {
                                    logger.Error("Did not recieve the expected format of json from {0}.", uri);
                                    logger.Error(ex);
                                    logger.Error(jsonResponse["data"].ToString());

                                }
                            }

                            logger.Info(this.Series[id].Name + " had " + numberOfUpdatedEpisodes +
                                        " episodes updated and " + numberOfNewEpisodes + " new episodes ");
                        }
                    }
                }
                catch (InvalidCastException ex)
                {
                    logger.Error("Did not recieve the expected format of json from {0}.",uri);
                    logger.Error(ex);
                    logger.Error(jsonResponse["data"].ToString());

                }
            }

            this.Say("Upgrading dirty locks");

            // if more than x% of a show's episodes are marked as dirty, just download the entire show again
            foreach (System.Collections.Generic.KeyValuePair<int, SeriesInfo> kvp in this.Series)
            {
                int totaleps = 0;
                int totaldirty = 0;
                foreach (System.Collections.Generic.KeyValuePair<int, Season> kvp2 in kvp.Value.Seasons)
                {
                    foreach (Episode ep in kvp2.Value.Episodes)
                    {
                        if (ep.Dirty)
                            totaldirty++;
                        totaleps++;
                    }
                }

                float percentDirty = 100;
                if (totaldirty > 0 || totaleps > 0) percentDirty = 100 * totaldirty / totaleps;
                if ((totaleps>0) && ((percentDirty) >= TVSettings.Instance.PercentDirtyUpgrade())) // 10%
                {
                    kvp.Value.Dirty = true;
                    kvp.Value.Seasons.Clear();
                    logger.Info("Planning to download all of {0} as {1}% of the episodes need to be updated", kvp.Value.Name, percentDirty);
                }
                else logger.Trace("Not planning to download all of {0} as {1}% of the episodes need to be updated and that's less than the 10% limit to upgrade.", kvp.Value.Name, percentDirty);
            }


            this.Say("");

            return true;
        }
        private bool ProcessXML(Stream str, Stream bannerStr, int? codeHint)
        {
            bool response = ProcessXML(str, codeHint);
            if (response == false) return response;

            //now we can process the bannerStr
            return ProcessXMLBanner(bannerStr, codeHint);
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

        private bool ProcessXMLBanner(Stream str, int? codeHint)
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

            if (!this.GetLock("ProcessTVDBBannerResponse"))
                return false;

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    IgnoreComments = true,
                    IgnoreWhitespace = true
                };
                XmlReader r = XmlReader.Create(str, settings);

                ProcessXMLBanner(r, codeHint);

            }
            catch (XmlException e)
            {
                string message = "Error processing data from TheTVDB (banner file).";
                message += "\r\n" + e.Message;
                String name = "";
                if (codeHint.HasValue && Series.ContainsKey(codeHint.Value))
                {
                    name += "Show \"" + Series[codeHint.Value].Name + "\" ";
                }
                if (codeHint.HasValue)
                {
                    name += "ID #" + codeHint.Value + " ";
                }

                logger.Error(e, name +"-"+ message);

                return false;
            }
            finally
            {
                this.Unlock("ProcessTVDBBannerResponse");
            }
            return true;
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
                    Banner b = new Banner(null, null, codeHint, r.ReadSubtree(), Args);

                    if (!this.Series.ContainsKey(b.SeriesID))
                        throw new TVDBException("Can't find the series to add the banner to (TheTVDB).");

                    SeriesInfo ser = this.Series[b.SeriesID];

                    ser.AddOrUpdateBanner(b);

                    r.Read();

                } else if ((r.Name == "Banners") && !r.IsStartElement()) {
                    this.Series[(int)codeHint].BannersLoaded = true;
                    break; // that's it.
                }

                else if (r.Name == "xml")
                    r.Read();
                else
                    r.Read();
            }

        }
        private int getLanguageId() {
            foreach (Language l in LanguageList) { 
                if (l.abbreviation == this.RequestLanguage) return l.id;
            }

            return -1;
        }
        private int getDefaultLanguageId()
        {
            foreach (Language l in LanguageList)
            {
                if (l.abbreviation == DefaultLanguage) return l.id;
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

            if (!this.GetLock("ProcessTVDBResponse"))
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
                        if (this.Series.ContainsKey(si.TVDBCode))
                            this.Series[si.TVDBCode].Merge(si, this.getLanguageId());
                        else
                            this.Series[si.TVDBCode] = si;
                        r.Read();
                    }
                    else if (r.Name == "Episode")
                    {
                        Episode e = new Episode(null, null, r.ReadSubtree(), Args);
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
                        this.New_Srv_Time = (time == null)?0:long.Parse(time);
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
                if (codeHint.HasValue && Series.ContainsKey(codeHint.Value))
                {
                    name += "Show \"" + Series[codeHint.Value].Name + "\" ";
                }
                if (codeHint.HasValue)
                {
                    name += "ID #" + codeHint.Value + " ";
                }

                logger.Error(name + message);
                logger.Error(str.ToString());
                throw new TVDBException(name + message);
            }
            finally
            {
                this.Unlock("ProcessTVDBResponse");
            }
            return true;
        }

        public bool DoWeForceReloadFor(int code)
        {
            return this.ForceReloadOn.Contains(code) || !this.Series.ContainsKey(code);
        }

        private SeriesInfo DownloadSeriesNow(int code, bool episodesToo, bool bannersToo)
        {
            bool forceReload = DoWeForceReloadFor(code);

            string txt = "";
            if (this.Series.ContainsKey(code))
                txt = this.Series[code].Name;
            else
                txt = "Code " + code;
            if (episodesToo)
                txt += " (Everything)";
            else
                txt += " Overview";

            if (bannersToo)
                txt += " plus banners";

            this.Say(txt);

            String uri = APIRoot + "/series/" + code;
            JObject jsonResponse = new JObject();
            JObject jsonDefaultLangResponse = new JObject();
            try
            {
                jsonResponse = HTTPHelper.JsonHTTPGETRequest(uri, null, this.authenticationToken, TVSettings.Instance.PreferredLanguage);

                if (inForeignLanguage()) jsonDefaultLangResponse = HTTPHelper.JsonHTTPGETRequest(uri, null, this.authenticationToken, DefaultLanguage );
            }
            catch (WebException ex)
            {
                logger.Error("Error obtaining {0}", uri);
                logger.Error (ex);
                this.Say("");
                this.LastError = ex.Message;
                return null;
            }

            SeriesInfo si;
            JObject seriesData = (JObject)jsonResponse["data"];

            if (inForeignLanguage()) {
                JObject seriesDataDefaultLang = (JObject)jsonDefaultLangResponse["data"];
                si = new SeriesInfo(seriesData,seriesDataDefaultLang, getLanguageId());
            }
            else
            {
                si = new SeriesInfo(seriesData, getLanguageId());
            }

            if (this.Series.ContainsKey(si.TVDBCode))
                this.Series[si.TVDBCode].Merge(si, this.getLanguageId());
            else
                this.Series[si.TVDBCode] = si;


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
                    String episodeUri = APIRoot + "/series/" + code + "/episodes";
                    JObject jsonEpisodeResponse = new JObject();
                    try
                    {
                        jsonEpisodeResponse = HTTPHelper.JsonHTTPGETRequest(episodeUri, new Dictionary<string, string> { { "page", pageNumber.ToString() } }, this.authenticationToken);
                        episodeResponses.Add(jsonEpisodeResponse);
                        int numberOfResponses = ((JArray)jsonEpisodeResponse["data"]).Count;
                        //logger.Info(code + "****" + jsonEpisodeResponse.ToString());
                        logger.Info("Page " + pageNumber + " of " + si.Name + " had " + numberOfResponses + " episodes listed");
                        if (numberOfResponses < 100) { morePages = false; } else { pageNumber++; }
                        

                    }
                    catch (WebException ex)
                    {
                        logger.Info("Error obtaining page "+pageNumber +" of " + episodeUri + ": " + ex.Message);
                        //There may be exactly 100 or 200 episodes, may not be a problem
                        morePages = false;
                    }
                }
            }

            foreach (JObject response in episodeResponses)
            {
                try
                {
                    foreach (JObject episodeData in response["data"])
                    {
                    //The episode does not contain enough data (specifically image filename), so we'll get the full version
                    this.DownloadEpisodeNow(code, (int)episodeData["id"]);

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
                catch (InvalidCastException ex)
                {
                    logger.Error("Did not recieve the expected format of json from {0}.", uri);
                    logger.Error(ex);
                    logger.Error(jsonResponse["data"].ToString());

                }
            }


            List<JObject> bannerResponses = new List<JObject>();
            List<JObject> bannerDefaultLangResponses = new List<JObject>();
            if (bannersToo || forceReload )            {
                // get /series/id/images if the bannersToo is set - may need to make multiple calls to for each image type
                List<string> imageTypes = new List<string> { };

                try
                {
                    JObject jsonEpisodeSearchResponse = HTTPHelper.JsonHTTPGETRequest(APIRoot + "/series/" + code + "/images", null, this.authenticationToken, TVSettings.Instance.PreferredLanguage);
                    JObject a = (JObject)jsonEpisodeSearchResponse["data"];

                    foreach (KeyValuePair<string, JToken> imageType in a)
                    {
                        if ((int)imageType.Value > 0) imageTypes.Add(imageType.Key);

                    }
                }
                catch (WebException ex) {
                    //no images for chosen language
                    logger.Trace("No images found for {0} in language {1}", APIRoot + "/series/" + code + "/images", TVSettings.Instance.PreferredLanguage);
                    logger.Warn(ex.Message);

                }



                foreach (string imageType in imageTypes)
                {
                    try
                    {
                        JObject jsonImageResponse = HTTPHelper.JsonHTTPGETRequest(APIRoot + "/series/" + code + "/images/query", new Dictionary<string, string> { { "keyType", imageType } }, this.authenticationToken, TVSettings.Instance.PreferredLanguage);
                        bannerResponses.Add(jsonImageResponse);
                    }
                    catch (WebException WebEx)
                    {
                        logger.Info("Looking for " + imageType + " images (in local language), but none found for seriesId " + code);
                        logger.Info(WebEx);
                    }

                }
                if (inForeignLanguage())
                {
                    List<string> imageDefaultLangTypes = new List<string> { };

                    try
                    {
                        JObject jsonEpisodeSearchDefaultLangResponse = HTTPHelper.JsonHTTPGETRequest(APIRoot + "/series/" + code + "/images", null, this.authenticationToken, DefaultLanguage);

                        JObject adl = (JObject)jsonEpisodeSearchDefaultLangResponse["data"];

                        foreach (KeyValuePair<string, JToken> imageType in adl)
                        {
                            if ((int)imageType.Value > 0) imageDefaultLangTypes.Add(imageType.Key);

                        }
                    }
                    catch (WebException ex)
                    {
                        logger.Info("Looking for images, but none found for seriesId {0} in {1}", code ,DefaultLanguage );
                        logger.Info(ex);

                        //no images for chosen language
                    }

                    foreach (string imageType in imageDefaultLangTypes)
                    {

                        try
                        {
                            JObject jsonImageDefaultLangResponse = HTTPHelper.JsonHTTPGETRequest(APIRoot + "/series/" + code + "/images/query", new Dictionary<string, string> { { "keyType", imageType } }, this.authenticationToken, DefaultLanguage);
                            bannerDefaultLangResponses.Add(jsonImageDefaultLangResponse);
                        }
                        catch (WebException webEx)
                        {
                            logger.Info(webEx,"Looking for " + imageType + " images, but none found for seriesId " + code);
                        }
                    }


                }


            }



            foreach (JObject response in bannerResponses )
            {
                try {
                    foreach (JObject bannerData in response["data"])                    {
                        Banner b = new Banner((int)si.TVDBCode, bannerData,getLanguageId());
                        if (!this.Series.ContainsKey(b.SeriesID)) throw new TVDBException("Can't find the series to add the banner to (TheTVDB).");
                        SeriesInfo ser = this.Series[b.SeriesID];
                        ser.AddOrUpdateBanner(b);
                    }
                }
                catch (InvalidCastException ex)
                {
                    logger.Error("Did not recieve the expected format of json from {0}.", uri);
                    logger.Error(ex);
                    logger.Error(jsonResponse["data"].ToString());

                }
            }

            foreach (JObject response in bannerDefaultLangResponses)
            {
                try
                {
                    foreach (JObject bannerData in response["data"])
                    {
                        Banner b = new Banner((int)si.TVDBCode, bannerData, getDefaultLanguageId());
                        if (!this.Series.ContainsKey(b.SeriesID)) throw new TVDBException("Can't find the series to add the banner to (TheTVDB).");
                        SeriesInfo ser = this.Series[b.SeriesID];
                        ser.AddOrUpdateBanner(b);
                    }
                }
                catch (InvalidCastException ex)
                {
                    logger.Error("Did not recieve the expected format of json from {0}.", uri);
                    logger.Error(ex);
                    logger.Error(jsonResponse["data"].ToString());
                }
            }

            this.Series[(int)si.TVDBCode].BannersLoaded = true;

            //Get the actors too then well need another call for that
            try
            {
                JObject jsonActorsResponse = HTTPHelper.JsonHTTPGETRequest(APIRoot + "/series/" + code + "/actors",
                    null, this.authenticationToken);
                IEnumerable<string> seriesActors = from a in jsonActorsResponse["data"] select (string) a["name"];
                this.Series[(int) si.TVDBCode].setActors(seriesActors);
            }
            catch (WebException ex)
            {
                logger.Error(ex, "Unble to obtain actors for {0}",this.Series[(int)si.TVDBCode].Name);
                this.LastError = ex.Message;
            }
        
            this.ForceReloadOn.Remove(code);

            return (this.Series.ContainsKey(code)) ? this.Series[code] : null;


        }

        private bool inForeignLanguage() => !(DefaultLanguage == this.RequestLanguage);

        private bool DownloadEpisodeNow(int seriesID, int episodeID)
        {
            bool forceReload = this.ForceReloadOn.Contains(seriesID);

            string txt = "";
            if (this.Series.ContainsKey(seriesID))
            {
                Episode ep = this.FindEpisodeByID(episodeID);
                string eptxt =  "New Episode Id = "+ episodeID;
                if ((ep != null) && (ep.TheSeason != null))
                    eptxt = string.Format("S{0:00}E{1:00}", ep.TheSeason.SeasonNumber, ep.EpNum);

                txt = this.Series[seriesID].Name + " (" + eptxt + ")";
            }
            else
                return false; // shouldn't happen
            this.Say(txt);

            String uri = APIRoot + "/episodes/" + episodeID.ToString();
            JObject jsonResponse = new JObject();
            JObject jsonDefaultLangResponse = new JObject();

            try {
                jsonResponse = HTTPHelper.JsonHTTPGETRequest(uri, null, this.authenticationToken, TVSettings.Instance.PreferredLanguage);

                if (inForeignLanguage()) jsonDefaultLangResponse = HTTPHelper.JsonHTTPGETRequest(uri, null, this.authenticationToken, DefaultLanguage);
            }
            catch (WebException ex)
            {
                logger.Error("Error obtaining " + uri + ": " + ex.Message);
                this.LastError = ex.Message;
                this.Say ("");
                return false;
            }

            
            if (!this.GetLock("ProcessTVDBResponse"))
                return false;

            try
            {
                Episode e; 
                JObject jsonResponseData = (JObject)jsonResponse["data"];

                if (inForeignLanguage())
                {
                    JObject seriesDataDefaultLang = (JObject)jsonDefaultLangResponse["data"];
                    e = new Episode(seriesID, jsonResponseData, seriesDataDefaultLang);
                }
                else
                {

                    e = new Episode(seriesID, jsonResponseData);
                }


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
                }
            }
            catch (TVDBException e)
            {
               logger.Error("Could not parse TVDB Response " + e.Message);
                this.LastError = e.Message;
                this.Say("");
                return false;
            }
            finally
            {
                this.Unlock("ProcessTVDBResponse");
            }
            return true;
        }

        public SeriesInfo MakePlaceholderSeries(int code, string name)
        {
            if (string.IsNullOrEmpty(name))
                name = "";
            this.Series[code] = new SeriesInfo(name, code);
            this.Series[code].Dirty = true;
            return this.Series[code];
        }

        public bool EnsureUpdated(int code, bool bannersToo)
        {
            if (DoWeForceReloadFor(code) || (this.Series[code].Seasons.Count == 0))
                return this.DownloadSeriesNow(code, true, bannersToo) != null; // the whole lot!

            bool ok = true;

            if ((this.Series[code].Dirty) || (bannersToo && !this.Series[code].BannersLoaded))
                ok = (this.DownloadSeriesNow(code, false, bannersToo) != null) && ok;

            foreach (System.Collections.Generic.KeyValuePair<int, Season> kvp in this.Series[code].Seasons)
            {
                Season seas = kvp.Value;
                foreach (Episode e in seas.Episodes)
                {
                    if (e.Dirty && e.EpisodeID >0)
                    {
                        this.LockEE();
                        this.ExtraEpisodes.Add(new ExtraEp(e.SeriesID, e.EpisodeID));
                        this.UnlockEE();
                    }
                }
            }

            this.LockEE();
            foreach (ExtraEp ee in this.ExtraEpisodes)
            {
                if ((ee.SeriesID == code) && (!ee.Done))
                {
                    ok = this.DownloadEpisodeNow(ee.SeriesID, ee.EpisodeID) && ok;
                    ee.Done = true;
                }
            }
            this.UnlockEE();

            this.ForceReloadOn.Remove(code);

            return ok;
        }

        public void Search(string text)
        {
            text = Helpers.RemoveDiacritics(text); // API doesn't like accented characters

            bool isNumber = Regex.Match(text, "^[0-9]+$").Success;
            if (isNumber)
                this.DownloadSeriesNow(int.Parse(text), false, false);

            // but, the number could also be a name, so continue searching as usual
            //text = text.Replace(".", " ");

            if (!this.Connected && !this.Connect())
            {
                this.Say("Failed to Connect");
                return;
            }

            String uri = APIRoot + "/search/series";
            JObject jsonResponse = new JObject();
            JObject jsonDefaultLangResponse = new JObject();
            try
            {
                jsonResponse = HTTPHelper.JsonHTTPGETRequest(uri, new Dictionary<string, string> { { "name", text } }, this.authenticationToken, TVSettings.Instance.PreferredLanguage);
                if (inForeignLanguage()) jsonDefaultLangResponse = HTTPHelper.JsonHTTPGETRequest(uri, new Dictionary<string, string> { { "name", text } }, this.authenticationToken,  DefaultLanguage);
            }
            catch (WebException ex)
            {
                logger.Error("Error obtaining " + uri + ": " + ex.Message);
                this.LastError = ex.Message;
                this.Say("");
                return;
            }

            if (this.GetLock("ProcessTVDBResponse"))
            {

                try
                {





                    foreach (JObject series in jsonResponse["data"])
                    {

                        // The <series> returned by GetSeries have
                        // less info than other results from
                        // thetvdb.com, so we need to smartly merge
                        // in a <Series> if we already have some/all
                        // info on it (depending on which one came
                        // first).

                        SeriesInfo si = new SeriesInfo(series, getLanguageId());
                        if (this.Series.ContainsKey(si.TVDBCode))
                            this.Series[si.TVDBCode].Merge(si, this.getLanguageId());
                        else
                            this.Series[si.TVDBCode] = si;
                    }
                }
                catch (InvalidCastException ex)
                {
                    logger.Error("Did not recieve the expected format of json from {0}.", uri);
                    logger.Error(ex);
                    logger.Error(jsonResponse["data"].ToString());

                }

                if (inForeignLanguage())
                {
                    //we also want to search for search terms that match in default language
                    try  {
                        foreach (JObject series in jsonDefaultLangResponse["data"])
                        {

                            // The <series> returned by GetSeries have
                            // less info than other results from
                            // thetvdb.com, so we need to smartly merge
                            // in a <Series> if we already have some/all
                            // info on it (depending on which one came
                            // first).

                            SeriesInfo si = new SeriesInfo(series, getLanguageId());
                            if (this.Series.ContainsKey(si.TVDBCode))
                                this.Series[si.TVDBCode].Merge(si, this.getLanguageId());
                            else
                                this.Series[si.TVDBCode] = si;
                        }

                    }
                    catch (InvalidCastException ex)
                    {
                        logger.Error("Did not recieve the expected format of json from {0}.", uri);
                        logger.Error(ex);
                        logger.Error(jsonResponse["data"].ToString());

                    }
                }
            }
            this.Unlock("ProcessTVDBResponse");


        }

        public string WebsiteURL(int code, int seasid, bool summaryPage)
        {
            // Summary: http://www.thetvdb.com/?tab=series&id=75340&lid=7
            // Season 3: http://www.thetvdb.com/?tab=season&seriesid=75340&seasonid=28289&lid=7

            if (summaryPage || (seasid <= 0) || !this.Series.ContainsKey(code))
                return this.WebsiteRoot + "/?tab=series&id=" + code;
            else
                return this.WebsiteRoot + "/?tab=season&seriesid=" + code + "&seasonid=" + seasid;
        }

        public string WebsiteURL(int SeriesID, int SeasonID, int EpisodeID)
        {
            // http://www.thetvdb.com/?tab=episode&seriesid=73141&seasonid=5356&id=108303&lid=7
            return this.WebsiteRoot + "/?tab=episode&seriesid=" + SeriesID + "&seasonid=" + SeasonID + "&id=" + EpisodeID;
        }

        


        // Next episode to air of a given show		
        public Episode NextAiring(int code)
        {
            if (!this.Series.ContainsKey(code) || (this.Series[code].Seasons.Count == 0))
                return null; // DownloadSeries(code, true);

            Episode next = null;
            DateTime today = DateTime.Now;
            DateTime mostSoonAfterToday = new DateTime(0);

            SeriesInfo ser = this.Series[code];
            foreach (System.Collections.Generic.KeyValuePair<int, Season> kvp2 in ser.Seasons)
            {
                Season s = kvp2.Value;

                foreach (Episode e in s.Episodes)
                {
                    DateTime? adt = e.GetAirDateDT(true);
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


