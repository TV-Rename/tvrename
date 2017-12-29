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
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using Newtonsoft.Json.Linq;
using NLog;

namespace TVRename
{
    public class SeriesInfo
    {
        public DateTime? AirsTime;
        public bool Dirty; // set to true if local info is known to be older than whats on the server
        public DateTime? FirstAired;
        public Dictionary<string, string> Items; // e.g. Overview, Banner, Poster, etc.
        public int LanguageId;
        private string _lastFiguredTz;
        public string Name;
        public bool BannersLoaded;

        public Dictionary<int, Season> Seasons;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //All Banners
        public Dictionary<int, Banner> AllBanners; // All Banners linked by bannerId.

        //Collections of Posters and Banners per season
        private Dictionary<int, Banner> _seasonBanners; // e.g. Dictionary of the best posters per series.
        private Dictionary<int, Banner> _seasonLangBanners; // e.g. Dictionary of the best posters per series in the correct language.
        private Dictionary<int, Banner> _seasonWideBanners; // e.g. Dictionary of the best wide banners per series.
        private Dictionary<int, Banner> _seasonLangWideBanners; // e.g. Dictionary of the best wide banners per series in the correct language.

        //best Banner, Poster and Fanart loaded from the images files (in any language)
        private int _bestSeriesPosterId;
        private int _bestSeriesBannerId;
        private int _bestSeriesFanartId;

        //best Banner, Poster and Fanart loaded from the images files (in our language)
        private int _bestSeriesLangPosterId;
        private int _bestSeriesLangBannerId;
        private int _bestSeriesLangFanartId;

        private TimeZone _seriesTz;

        public string ShowTimeZone; // set for us by ShowItem
        public long SrvLastUpdated;
        public int TVDBCode;

        // note: "SeriesID" in a <Series> is the tv.com code,
        // "seriesid" in an <Episode> is the tvdb code!

        public SeriesInfo(string name, int id)
        {
            SetToDefauts();
            Name = name;
            TVDBCode = id;
        }

        public SeriesInfo(XmlReader r)
        {
            SetToDefauts();
            LoadXml(r);
        }

        public SeriesInfo(JObject json,int langId)
        {
            SetToDefauts();
            LanguageId = langId;
            LoadJson(json);

            if (String.IsNullOrEmpty(Name)            ){
               Logger.Warn("Issue with series " + TVDBCode );
               Logger.Warn(json.ToString());
            }
        }

        public SeriesInfo(JObject json, JObject jsonInDefaultLang, int langId)
        {
            SetToDefauts();
            LanguageId = langId;
            LoadJson(json,jsonInDefaultLang);
            if (String.IsNullOrEmpty(Name)            ){
               Logger.Warn("Issue with series " + TVDBCode );
               Logger.Warn(json.ToString());
               Logger.Info(jsonInDefaultLang .ToString());
            }

        }


        private void FigureOutTimeZone()
        {
            string tzstr = ShowTimeZone;

            if (string.IsNullOrEmpty(tzstr))
                tzstr = TimeZone.DefaultTimeZone();

            _seriesTz = TimeZone.TimeZoneFor(tzstr);

            _lastFiguredTz = tzstr;
        }

        public TimeZone GetTimeZone()
        {
            // we cache the timezone info, as the fetching is a bit slow, and we do this a lot
            if (_lastFiguredTz != ShowTimeZone)
                FigureOutTimeZone();

            return _seriesTz;
        }

        public string[] GetActors()
        {
            String actors = GetValueAcrossVersions("Actors","actors","");
            
            if (!string.IsNullOrEmpty(actors))
            {
                return actors.Split('|');

            }
            return new String[] { };
        }


        private string GetItem(string which) //MS making this private to avoid external classes having to worry about how the items colelction is keyed
        {
            if (Items.ContainsKey(which))
                return Items[which];
            return "";
        }

        public void SetActors(IEnumerable<string> actors)
        {
            Items["Actors"] = String.Join("|", actors);
        }

        public void SetToDefauts()
        {
            ShowTimeZone = TimeZone.DefaultTimeZone(); // default, is correct for most shows
            _lastFiguredTz = "";

            Items = new Dictionary<string, string>();
            Seasons = new Dictionary<int, Season>();

            AllBanners = new Dictionary<int, Banner>();
            _seasonBanners = new Dictionary<int, Banner>();
            _seasonLangBanners = new Dictionary<int, Banner>();
            _seasonWideBanners = new Dictionary<int, Banner>();
            _seasonLangWideBanners = new Dictionary<int, Banner>();

            Dirty = false;
            Name = "";
            AirsTime = null;
            TVDBCode = -1;
            LanguageId = -1;
            BannersLoaded = false;

            _bestSeriesPosterId = -1;
            _bestSeriesBannerId = -1;
            _bestSeriesFanartId = -1;
            _bestSeriesLangPosterId = -1;
            _bestSeriesLangBannerId = -1;
            _bestSeriesLangFanartId = -1;
        }

        public void Merge(SeriesInfo o, int preferredLanguageId)
        {
            if (o.TVDBCode != TVDBCode)
                return; // that's not us!
            if (o.SrvLastUpdated != 0 && o.SrvLastUpdated < SrvLastUpdated)
                return; // older!?

            bool betterLanguage = ((o.LanguageId ==-1)|| (o.LanguageId == preferredLanguageId) && (LanguageId != preferredLanguageId));

            SrvLastUpdated = o.SrvLastUpdated;

            // take the best bits of "o"
            // "o" is always newer/better than us, if there is a choice
            if ((!string.IsNullOrEmpty(o.Name)) && betterLanguage)
                Name = o.Name;
            // this.Items.Clear();
            foreach (KeyValuePair<string, string> kvp in o.Items)
            {
                // on offer is non-empty text, in a better language
                // or text for something we don't have
                if ((!string.IsNullOrEmpty(kvp.Value) && betterLanguage) ||
                     (!Items.ContainsKey(kvp.Key) || string.IsNullOrEmpty(Items[kvp.Key])))
                    Items[kvp.Key] = kvp.Value;
            }
            if (o.AirsTime != null)
                AirsTime = o.AirsTime;

            if ((o.Seasons != null) && (o.Seasons.Count != 0))
                Seasons = o.Seasons;

            if ((o._seasonBanners != null) && (o._seasonBanners.Count != 0))
                _seasonBanners = o._seasonBanners;

            if ((o._seasonLangBanners != null) && (o._seasonLangBanners.Count != 0))
                _seasonLangBanners = o._seasonLangBanners;

            if ((o._seasonLangWideBanners != null) && (o._seasonLangWideBanners.Count != 0))
                _seasonLangWideBanners = o._seasonLangWideBanners;

            if ((o._seasonWideBanners != null) && (o._seasonWideBanners.Count != 0))
                _seasonWideBanners = o._seasonWideBanners;

            if ((o.AllBanners != null) && (o.AllBanners.Count != 0))
                AllBanners = o.AllBanners;

            if ((o._bestSeriesPosterId != -1)) _bestSeriesPosterId = o._bestSeriesPosterId;
            if ((o._bestSeriesBannerId != -1) ) _bestSeriesBannerId = o._bestSeriesBannerId;
            if ((o._bestSeriesFanartId != -1) ) _bestSeriesFanartId = o._bestSeriesFanartId;
            if ((o._bestSeriesLangPosterId != -1)) _bestSeriesLangPosterId = o._bestSeriesLangPosterId;
            if ((o._bestSeriesLangBannerId != -1)) _bestSeriesLangBannerId = o._bestSeriesLangBannerId;
            if ((o._bestSeriesLangFanartId != -1)) _bestSeriesLangFanartId = o._bestSeriesLangFanartId;



            if (betterLanguage)
                LanguageId = o.LanguageId;

            Dirty = o.Dirty;
        }

        public void LoadXml(XmlReader r)
        {
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
                r.Read();
                if (r.Name != "Series")
                    return;

                r.Read();
                while (!r.EOF)
                {
                    if ((r.Name == "Series") && (!r.IsStartElement()))
                        break;
                    if (r.Name == "id")
                        TVDBCode = r.ReadElementContentAsInt();
                    else if (r.Name == "SeriesName")
                        Name = XMLHelper.ReadStringFixQuotesAndSpaces(r);
                    else if (r.Name == "lastupdated")
                        SrvLastUpdated = r.ReadElementContentAsLong();
                    else if ((r.Name == "Language") || (r.Name == "language")) { string ignore = r.ReadElementContentAsString(); }
                    else if ((r.Name == "LanguageId") || (r.Name == "languageId"))
                        LanguageId = r.ReadElementContentAsInt();
                    else if (r.Name == "TimeZone")
                        ShowTimeZone = r.ReadElementContentAsString();
                    else if (r.Name == "Airs_Time")
                    {
                        AirsTime = DateTime.Parse("20:00");

                        string theTime = r.ReadElementContentAsString();
                        try
                        {
                            if (!string.IsNullOrEmpty(theTime))
                            {
                                Items["Airs_Time"] = theTime;
                                DateTime airsTime;
                                if (DateTime.TryParse(theTime, out airsTime) |
                                    DateTime.TryParse(theTime.Replace('.', ':'), out airsTime))
                                    AirsTime = airsTime;
                                else
                                    AirsTime = null;
                            }
                        }
                        catch (FormatException)
                        {
                            Logger.Trace("Failed to parse time: {0} ", theTime);
                        }
                    }
                    else if (r.Name == "FirstAired")
                    {
                        string theDate = r.ReadElementContentAsString();

                        try
                        {
                            FirstAired = DateTime.ParseExact(theDate, "yyyy-MM-dd", new CultureInfo(""));
                            Items["FirstAired"] = FirstAired.Value.ToString("yyyy-MM-dd");
                            Items["Year"] = FirstAired.Value.ToString("yyyy");
                        }
                        catch
                        {
                            Logger.Trace("Failed to parse date: {0} ", theDate);
                            FirstAired = null;
                            Items["FirstAired"] = "";
                            Items["Year"] = "";
                        }
                    }
                    else
                    {
                        string name = r.Name;
                        Items[name] = r.ReadElementContentAsString();
                    }
                    //   r->ReadOuterXml(); // skip
                } // while
            } // try
            catch (XmlException e)
            {
                string message = "Error processing data from TheTVDB for a show.";
                if (TVDBCode != -1)
                    message += "\r\nTheTVDB Code: " + TVDBCode;
                if (!string.IsNullOrEmpty(Name))
                    message += "\r\nName: " + Name;

                message += "\r\nLanguage: \"" + LanguageId + "\"";

                message += "\r\n" + e.Message;

                Logger.Error(e,message);

                throw new TVDBException(e.Message);
            }
        }

        // LoadXml

        public void LoadJson(JObject r)
        {
            //r should be a series of name/value pairs (ie a JArray of JPropertes)
            //save them all into the Items array for safe keeping
            foreach (JProperty seriesItems in r.Children<JProperty>())
            {
                if (seriesItems.Name == "aliases") Items[seriesItems.Name] = JsonHelper.Flatten(seriesItems.Value, "|");
                else if (seriesItems.Name == "genre") Items[seriesItems.Name] = JsonHelper.Flatten(seriesItems.Value, "|");
                else try
                    {
                        if (seriesItems.Value != null) Items[seriesItems.Name] = (string)seriesItems.Value;
                    }
                    catch (ArgumentException ae) {
                       Logger.Warn("Could not parse Json for " + seriesItems.Name + " :" + ae.Message);
                    }
            }


            TVDBCode = (int)r["id"];
            if ((string)r["seriesName"] != null)
            {
                Name = (string)r["seriesName"];
            }


            long updateTime;
            if (long.TryParse((string)r["lastUpdated"], out updateTime) )
                SrvLastUpdated = updateTime;
            else
                SrvLastUpdated = 0;

            string theDate = (string)r["firstAired"];
            try
            {
                if (!String.IsNullOrEmpty(theDate)) {
                    FirstAired = DateTime.ParseExact(theDate, "yyyy-MM-dd", new CultureInfo(""));
                    Items["firstAired"] = FirstAired.Value.ToString("yyyy-MM-dd");
                    Items["Year"] = FirstAired.Value.ToString("yyyy");
                }else
                {
                    FirstAired = null;
                    Items["firstAired"] = "";
                    Items["Year"] = "";
                }
            }
            catch
            {
                FirstAired = null;
                Items["firstAired"] = "";
                Items["Year"] = "";
            }


            AirsTime = DateTime.Parse("20:00");
            string theAirsTime = (string)r["airsTime"];
            try
            {
                if (!string.IsNullOrEmpty(theAirsTime))
                {
                    Items["airsTime"] = theAirsTime;
                    DateTime airsTime;
                    if (DateTime.TryParse(theAirsTime, out airsTime) |
                        DateTime.TryParse(theAirsTime.Replace('.', ':'), out airsTime))
                        AirsTime = airsTime;
                    else
                        AirsTime = null;
                }
            }
            catch (FormatException)
            {
            }

        }

        public void LoadJson(JObject bestLanguageR, JObject backupLanguageR)
        {
            //Here we have two pieces of JSON. One in local language and one in the default language (English). 
            //We will populate with the best language frst and then fill in any gaps with the backup Language
            LoadJson(bestLanguageR);

            //backupLanguageR should be a series of name/value pairs (ie a JArray of JPropertes)
            //TVDB asserts that name and overview are the fields that are localised

            if ((string.IsNullOrWhiteSpace(Name) && ((string)backupLanguageR["seriesName"] != null)) ){
                Name = (string)backupLanguageR["seriesName"];
                Items["seriesName"] = Name;
            }

            if ((string.IsNullOrWhiteSpace(Items["overview"]) && ((string)backupLanguageR["overview"] != null)) ){
                Items["overview"] = (string)backupLanguageR["overview"];
            }

            //Looking at the data then the aliases, banner and runtime are also different by language
            
            if ((string.IsNullOrWhiteSpace(Items["aliases"])))
            {
                Items["aliases"] = JsonHelper.Flatten(backupLanguageR["aliases"], "|");
            }

            if ((string.IsNullOrWhiteSpace(Items["runtime"])))
            {
                Items["runtime"] = (string)backupLanguageR["runtime"];
            }
            if ((string.IsNullOrWhiteSpace(Items["banner"])))
            {
                Items["banner"] = (string)backupLanguageR["banner"];
            }


        }


        public string GetStatus() =>              GetValueAcrossVersions("Status", "status", "Unknown");
        public string GetAirsTime() =>             GetValueAcrossVersions("Airs_Time", "airsTime", "");
        public string GetAirsDay()=> GetValueAcrossVersions("Airs_DayOfWeek", "airsDayOfWeek", "");
        public string GetNetwork() => GetValueAcrossVersions("Network", "network", "");
        public string GetOverview() => GetValueAcrossVersions("Overview", "overview", "");
        public string GetRuntime() => GetValueAcrossVersions("Runtime", "runtime", "");
        public string GetRating() => GetValueAcrossVersions("Rating","rating",""); // check , "ContentRating"
        public string GetImdb() => GetValueAcrossVersions("IMDB_ID", "imdb_id", "");
        public string GetYear() => GetValueAcrossVersions("Year", "year", "");
        public string GetFirstAired() => GetValueAcrossVersions("FirstAired", "firstAired", "");

        public string[] GetGenres()
        {

            String genreString = GetValueAcrossVersions("Genre", "genre", "");

            if (!string.IsNullOrEmpty(genreString))
            {
                return genreString.Split('|');

            }
            return new String[] { };

        }



        public string GetImage(TVSettings.FolderJpgIsType type)
        {
            switch (type)
            {
                case TVSettings.FolderJpgIsType.Banner:
                    return GetSeriesWideBannerPath(); 
                case TVSettings.FolderJpgIsType.FanArt:
                    return GetSeriesFanartPath();
                case TVSettings.FolderJpgIsType.SeasonPoster:
                    return GetSeriesPosterPath(); 
                default:
                    return GetSeriesPosterPath(); 
            }
        }
        string GetValueAcrossVersions(string oldTag, string newTag, string defaultValue)
        {
            //Need to cater for new and old style tags (TVDB interface v1 vs v2)
            if (Items.ContainsKey(oldTag)) return Items[oldTag];
            if (Items.ContainsKey(newTag)) return Items[newTag];
            return defaultValue;
        }



        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Series");

            XMLHelper.WriteElementToXML(writer, "id", TVDBCode);
            XMLHelper.WriteElementToXML(writer, "SeriesName", Name);
            XMLHelper.WriteElementToXML(writer, "lastupdated", SrvLastUpdated);
            XMLHelper.WriteElementToXML(writer, "LanguageId", LanguageId);
            XMLHelper.WriteElementToXML(writer, "Airs_Time", AirsTime );
            XMLHelper.WriteElementToXML(writer, "TimeZone", ShowTimeZone);

            if (FirstAired != null)
            {
                XMLHelper.WriteElementToXML(writer, "FirstAired", FirstAired.Value.ToString("yyyy-MM-dd"));
            }


            List<string> skip = new List<String>
                                  {
                                      "Airs_Time",
                                      "lastupdated","lastUpdated",
                                      "id","seriesName","seriesname","SeriesName",
                                      "lastUpdated","lastupdated",
                                      "FirstAired","firstAired",
                                      "LanguageId","TimeZone"
                                  };

            foreach (KeyValuePair<string, string> kvp in Items)
            {
                if (!skip.Contains(kvp.Key))
                {
                    XMLHelper.WriteElementToXML(writer, kvp.Key, kvp.Value);
                }
            }

            writer.WriteEndElement(); // series
        }

        public Season GetOrAddSeason(int num, int seasonId)
        {
            if (Seasons.ContainsKey(num))
                return Seasons[num];

            Season s = new Season(this, num, seasonId);
            Seasons[num] = s;

            return s;
        }



        public string GetSeasonBannerPath(int snum)
        {
            //We aim to return the season and language specific poster,
            //if not then a season specific one is best
            //if not then the poster is the fallback

            Debug.Assert(BannersLoaded);

            if (_seasonLangBanners.ContainsKey(snum))
                return _seasonLangBanners[snum].BannerPath;

            if (_seasonBanners.ContainsKey(snum))
                return _seasonBanners[snum].BannerPath;

            //if there is a problem then return the non-season specific poster by default
            return GetSeriesPosterPath();
                
        }

        public string GetSeriesWideBannerPath()
        {
            //firstly choose the one the TVDB recommended
            if (!string.IsNullOrEmpty(GetItem("banner"))) return GetItem("banner");

            //then try the best one we've found with the correct language
            if (_bestSeriesLangBannerId != -1) return AllBanners[_bestSeriesLangBannerId].BannerPath;

            //if there are none with the righ tlanguage then try one from another language
            if (_bestSeriesBannerId  != -1) return AllBanners[_bestSeriesBannerId].BannerPath;

            //give up
            return "";
        }

        public string GetSeriesPosterPath()
        {
            //firstly choose the one the TVDB recommended
            if (!string.IsNullOrEmpty(GetItem("poster"))) return GetItem("poster");

            //then try the best one we've found with the correct language
            if (_bestSeriesLangPosterId != -1) return AllBanners[_bestSeriesLangPosterId].BannerPath;

            //if there are none with the righ tlanguage then try one from another language
            if (_bestSeriesPosterId != -1) return AllBanners[_bestSeriesPosterId].BannerPath;

            //give up
            return "";
        }

        public string GetSeriesFanartPath()
        {
            //firstly choose the one the TVDB recommended
            if (!string.IsNullOrEmpty(GetItem("fanart"))) return GetItem("fanart");

            //then try the best one we've found with the correct language
            if (_bestSeriesLangFanartId != -1) return AllBanners[_bestSeriesLangFanartId].BannerPath;

            //if there are none with the righ tlanguage then try one from another language
            if (_bestSeriesFanartId != -1) return AllBanners[_bestSeriesFanartId].BannerPath;

            //give up
            return "";
        }

        public string GetSeasonWideBannerPath(int snum)
        {
            //We aim to return the season and language specific poster,
            //if not then a season specific one is best
            //if not then the poster is the fallback

            Debug.Assert(BannersLoaded);

            if (_seasonLangWideBanners.ContainsKey(snum))
                return _seasonLangWideBanners[snum].BannerPath;

            if (_seasonWideBanners.ContainsKey(snum))
                return _seasonWideBanners[snum].BannerPath;

            //if there is a problem then return the non-season specific poster by default
            return GetSeriesWideBannerPath();
        }



        public void AddOrUpdateBanner(Banner banner)
        {
            if (AllBanners.ContainsKey(banner.BannerId)) {
                AllBanners[banner.BannerId] = banner;
            } else {
                AllBanners.Add(banner.BannerId, banner);
            }

            if (banner.IsSeasonPoster()) AddOrUpdateSeasonPoster(banner);
            if (banner.IsSeasonBanner()) AddOrUpdateWideSeason(banner);

            if (banner.IsSeriesPoster()) _bestSeriesPosterId = GetBestBannerId(banner, _bestSeriesPosterId);
            if (banner.IsSeriesBanner()) _bestSeriesBannerId = GetBestBannerId(banner, _bestSeriesBannerId);
            if (banner.IsFanart()) _bestSeriesFanartId = GetBestBannerId(banner, _bestSeriesFanartId);

            if (banner.LanguageId == LanguageId)
            {
                if (banner.IsSeriesPoster()) _bestSeriesLangPosterId = GetBestBannerId(banner, _bestSeriesLangPosterId);
                if (banner.IsSeriesBanner()) _bestSeriesLangBannerId = GetBestBannerId(banner, _bestSeriesLangBannerId);
                if (banner.IsFanart()) _bestSeriesLangFanartId = GetBestBannerId(banner, _bestSeriesLangFanartId);

            }
        }

        private int GetBestBannerId(Banner selectedBanner, int bestBannerId)
        {
            if (bestBannerId == -1) return selectedBanner.BannerId;

            if (AllBanners[bestBannerId].Rating < selectedBanner.Rating)
            {
                //update banner - we have found a better one
                return selectedBanner.BannerId;
            }

            return bestBannerId;
        }


        public void AddOrUpdateSeasonPoster(Banner banner)
        {
            AddUpdateIntoCollections(banner, _seasonBanners, _seasonLangBanners);
        }

        public void AddOrUpdateWideSeason(Banner banner)
        {
            AddUpdateIntoCollections(banner, _seasonWideBanners, _seasonLangWideBanners);
        }

        private void AddUpdateIntoCollections(Banner banner, Dictionary<int, Banner> coll, Dictionary<int, Banner> langColl)
        {
            //update language specific cache if appropriate
            if (banner.LanguageId == LanguageId)
            {
                AddUpdateIntoCollection(banner,langColl);
            }

            //Now do the same for the all banners dictionary
            AddUpdateIntoCollection(banner, coll);
            
        }

        private void AddUpdateIntoCollection(Banner banner, Dictionary<int, Banner> coll)
        {
            int seasonOfNewBanner = banner.SeasonId;

            if (coll.ContainsKey(seasonOfNewBanner))
            {
                //it already contains a season of the approprite type - see which is better
                if (coll[seasonOfNewBanner].Rating < banner.Rating)
                {
                    //update banner - we have found a better one
                    coll[seasonOfNewBanner] = banner;
                }

            }
            else
                coll.Add(seasonOfNewBanner, banner);
        }

        internal Episode GetEpisode(int seasF, int epF)
        {
           foreach ( Season s in Seasons.Values)
            {
                if (s.SeasonNumber == seasF)
                {
                    foreach (Episode pe in s.Episodes)
                    {
                        if (pe.EpNum == epF) return pe;
                    }
                }
            }
            throw new EpisodeNotFoundException();
        }

        [Serializable]
        public class EpisodeNotFoundException : Exception
        {
            public EpisodeNotFoundException()
            {
            }

            public EpisodeNotFoundException(string message) : base(message)
            {
            }

            public EpisodeNotFoundException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected EpisodeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }
}
