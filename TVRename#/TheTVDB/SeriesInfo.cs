// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Runtime.Serialization;

namespace TVRename
{
    public class SeriesInfo
    {
        public DateTime? AirsTime;
        public bool Dirty; // set to true if local info is known to be older than whats on the server
        public DateTime? FirstAired;
        public Dictionary<string, string> Items; // e.g. Overview, Banner, Poster, etc.
        private int LanguageId;
        public string Name;
        public bool BannersLoaded;

        public Dictionary<int, Season> AiredSeasons;
        public Dictionary<int, Season> DVDSeasons;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        //All Banners
        public Dictionary<int, Banner> AllBanners; // All Banners linked by bannerId.

        //Collections of Posters and Banners per season
        private Dictionary<int, Banner> SeasonBanners; // e.g. Dictionary of the best posters per series.
        private Dictionary<int, Banner> SeasonLangBanners; // e.g. Dictionary of the best posters per series in the correct language.
        private Dictionary<int, Banner> SeasonWideBanners; // e.g. Dictionary of the best wide banners per series.
        private Dictionary<int, Banner> SeasonLangWideBanners; // e.g. Dictionary of the best wide banners per series in the correct language.

        //best Banner, Poster and Fanart loaded from the images files (in any language)
        private int bestSeriesPosterId;
        private int bestSeriesBannerId;
        private int bestSeriesFanartId;

        //best Banner, Poster and Fanart loaded from the images files (in our language)
        private int bestSeriesLangPosterId;
        private int bestSeriesLangBannerId;
        private int bestSeriesLangFanartId;

        public long Srv_LastUpdated;
        public int TVDBCode;
        public string tempTimeZone;

        public int MinYear()
        {
            int min = 9999;
            foreach (Season s in this.AiredSeasons.Values)
            {
                foreach (Episode e in s.Episodes)
                {
                    if (e.GetAirDateDT().HasValue)
                    {
                        if (e.GetAirDateDT().Value.Year < min) min = e.GetAirDateDT().Value.Year;
                    }
                }
            }

            return min;
        }

        public int MaxYear()
        {
            int max = 0;
            foreach (Season s in this.AiredSeasons.Values)
            {
                foreach (Episode e in s.Episodes)
                {
                    if (e.GetAirDateDT().HasValue)
                    {
                        if (e.GetAirDateDT().Value.Year > max) max = e.GetAirDateDT().Value.Year;
                    }
                }
            }

            return max;
        }

        
        public DateTime? LastAiredDate() {
            DateTime? returnValue = null; 
            foreach (Season s in this.AiredSeasons.Values) //We can use AiredSeasons as it does not matter which order we do this in Aired or DVD
            {
                DateTime? seasonLastAirDate = s.LastAiredDate();

                if (!seasonLastAirDate.HasValue) continue;

                if (!returnValue.HasValue) returnValue = seasonLastAirDate.Value;
                else if (DateTime.Compare(seasonLastAirDate.Value, returnValue.Value) > 0) returnValue = seasonLastAirDate.Value;
            }
            return returnValue;
        }

        // note: "SeriesID" in a <Series> is the tv.com code,
        // "seriesid" in an <Episode> is the tvdb code!

        public SeriesInfo(string name, int id)
        {
            SetToDefauts();
            this.Name = name;
            this.TVDBCode = id;
        }

        public SeriesInfo(XmlReader r)
        {
            SetToDefauts();
            LoadXml(r);
        }

        public SeriesInfo(JObject json,int langId)
        {
            SetToDefauts();
            this.LanguageId = langId;
            LoadJSON(json);

            if (String.IsNullOrEmpty(this.Name)            ){
               logger.Warn("Issue with series " + this.TVDBCode );
               logger.Warn(json.ToString());
            }
        }

        public SeriesInfo(JObject json, JObject jsonInDefaultLang, int langId)
        {
            SetToDefauts();
            this.LanguageId = langId;
            LoadJSON(json,jsonInDefaultLang);
            if (String.IsNullOrEmpty(this.Name)            ){
               logger.Warn("Issue with series " + this.TVDBCode );
               logger.Warn(json.ToString());
               logger.Info(jsonInDefaultLang .ToString());
            }

        }


        public string[] GetActors()
        {
            String actors = getValueAcrossVersions("Actors","actors","");
            
            if (!string.IsNullOrEmpty(actors))
            {
                return actors.Split('|');

            }
            return new String[] { };
        }


        private string GetItem(string which) //MS making this private to avoid external classes having to worry about how the items colelction is keyed
        {
            if (this.Items.ContainsKey(which))
                return this.Items[which];
            return "";
        }

        public void setActors(IEnumerable<string> actors)
        {
            this.Items["Actors"] = String.Join("|", actors);
        }

        public void SetToDefauts()
        {
            this.Items = new Dictionary<string, string>();
            this.AiredSeasons = new Dictionary<int, Season>();
            this.DVDSeasons = new Dictionary<int, Season>();


            this.Dirty = false;
            this.Name = "";
            this.AirsTime = null;
            this.TVDBCode = -1;
            this.LanguageId = -1;
            resetBanners();
        }

        public void resetBanners()
        {
            this.BannersLoaded = false;
            this.AllBanners = new Dictionary<int, Banner>();
            this.SeasonBanners = new Dictionary<int, Banner>();
            this.SeasonLangBanners = new Dictionary<int, Banner>();
            this.SeasonWideBanners = new Dictionary<int, Banner>();
            this.SeasonLangWideBanners = new Dictionary<int, Banner>();

            this.bestSeriesPosterId = -1;
            this.bestSeriesBannerId = -1;
            this.bestSeriesFanartId = -1;
            this.bestSeriesLangPosterId = -1;
            this.bestSeriesLangBannerId = -1;
            this.bestSeriesLangFanartId = -1;

        }

        public void Merge(SeriesInfo o, int preferredLanguageId)
        {
            if (o.TVDBCode != this.TVDBCode)
                return; // that's not us!
            if (o.Srv_LastUpdated != 0 && o.Srv_LastUpdated < this.Srv_LastUpdated)
                return; // older!?

            bool betterLanguage = ((o.LanguageId ==-1)|| (o.LanguageId == preferredLanguageId) && (this.LanguageId != preferredLanguageId));

            this.Srv_LastUpdated = o.Srv_LastUpdated;

            // take the best bits of "o"
            // "o" is always newer/better than us, if there is a choice
            if ((!string.IsNullOrEmpty(o.Name)) && betterLanguage)
                this.Name = o.Name;
            // this.Items.Clear();
            foreach (KeyValuePair<string, string> kvp in o.Items)
            {
                // on offer is non-empty text, in a better language
                // or text for something we don't have
                if ((!string.IsNullOrEmpty(kvp.Value) && betterLanguage) ||
                     (!this.Items.ContainsKey(kvp.Key) || string.IsNullOrEmpty(this.Items[kvp.Key])))
                    this.Items[kvp.Key] = kvp.Value;
            }
            if (o.AirsTime != null)
                this.AirsTime = o.AirsTime;

            if ((o.AiredSeasons != null) && (o.AiredSeasons.Count != 0))
                this.AiredSeasons = o.AiredSeasons;
            if ((o.DVDSeasons != null) && (o.DVDSeasons.Count != 0))
                this.DVDSeasons = o.DVDSeasons;

            if ((o.SeasonBanners != null) && (o.SeasonBanners.Count != 0))
                this.SeasonBanners = o.SeasonBanners;

            if ((o.SeasonLangBanners != null) && (o.SeasonLangBanners.Count != 0))
                this.SeasonLangBanners = o.SeasonLangBanners;

            if ((o.SeasonLangWideBanners != null) && (o.SeasonLangWideBanners.Count != 0))
                this.SeasonLangWideBanners = o.SeasonLangWideBanners;

            if ((o.SeasonWideBanners != null) && (o.SeasonWideBanners.Count != 0))
                this.SeasonWideBanners = o.SeasonWideBanners;

            if ((o.AllBanners != null) && (o.AllBanners.Count != 0))
                this.AllBanners = o.AllBanners;

            if ((o.bestSeriesPosterId != -1)) this.bestSeriesPosterId = o.bestSeriesPosterId;
            if ((o.bestSeriesBannerId != -1) ) this.bestSeriesBannerId = o.bestSeriesBannerId;
            if ((o.bestSeriesFanartId != -1) ) this.bestSeriesFanartId = o.bestSeriesFanartId;
            if ((o.bestSeriesLangPosterId != -1)) this.bestSeriesLangPosterId = o.bestSeriesLangPosterId;
            if ((o.bestSeriesLangBannerId != -1)) this.bestSeriesLangBannerId = o.bestSeriesLangBannerId;
            if ((o.bestSeriesLangFanartId != -1)) this.bestSeriesLangFanartId = o.bestSeriesLangFanartId;



            if (betterLanguage)
                this.LanguageId = o.LanguageId;

            this.Dirty = o.Dirty;
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
                        this.TVDBCode = r.ReadElementContentAsInt();
                    else if (r.Name == "SeriesName")
                        this.Name = XMLHelper.ReadStringFixQuotesAndSpaces(r);
                    else if (r.Name == "lastupdated")
                        this.Srv_LastUpdated = r.ReadElementContentAsLong();
                    else if ((r.Name == "Language") || (r.Name == "language")) { string ignore = r.ReadElementContentAsString(); }
                    else if ((r.Name == "LanguageId") || (r.Name == "languageId"))
                        this.LanguageId = r.ReadElementContentAsInt();
                    else if (r.Name == "TimeZone")
                        this.tempTimeZone = r.ReadElementContentAsString();
                    else if (r.Name == "Airs_Time")
                    {
                        this.AirsTime = DateTime.Parse("20:00");

                        string theTime = r.ReadElementContentAsString();
                        try
                        {
                            if (!string.IsNullOrEmpty(theTime))
                            {
                                this.Items["Airs_Time"] = theTime;
                                DateTime airsTime;
                                if (DateTime.TryParse(theTime, out airsTime) |
                                    DateTime.TryParse(theTime.Replace('.', ':'), out airsTime))
                                    this.AirsTime = airsTime;
                                else
                                    this.AirsTime = null;
                            }
                        }
                        catch (FormatException)
                        {
                            logger.Trace("Failed to parse time: {0} ", theTime);
                        }
                    }
                    else if (r.Name == "FirstAired")
                    {
                        string theDate = r.ReadElementContentAsString();

                        try
                        {
                            this.FirstAired = DateTime.ParseExact(theDate, "yyyy-MM-dd", new System.Globalization.CultureInfo(""));
                            this.Items["FirstAired"] = this.FirstAired.Value.ToString("yyyy-MM-dd");
                            this.Items["Year"] = this.FirstAired.Value.ToString("yyyy");
                        }
                        catch
                        {
                            logger.Trace("Failed to parse date: {0} ", theDate);
                            this.FirstAired = null;
                            this.Items["FirstAired"] = "";
                            this.Items["Year"] = "";
                        }
                    }
                    else
                    {
                        string name = r.Name;
                        this.Items[name] = r.ReadElementContentAsString();
                    }
                    //   r->ReadOuterXml(); // skip
                } // while
            } // try
            catch (XmlException e)
            {
                string message = "Error processing data from TheTVDB for a show.";
                if (this.TVDBCode != -1)
                    message += "\r\nTheTVDB Code: " + this.TVDBCode;
                if (!string.IsNullOrEmpty(this.Name))
                    message += "\r\nName: " + this.Name;

                message += "\r\nLanguage: \"" + this.LanguageId + "\"";

                message += "\r\n" + e.Message;

                logger.Error(e,message);

                throw new TheTVDB.TVDBException(e.Message);
            }
        }

        // LoadXml

        public void LoadJSON(JObject r)
        {
            //r should be a series of name/value pairs (ie a JArray of JPropertes)
            //save them all into the Items array for safe keeping
            foreach (JProperty seriesItems in r.Children<JProperty>())
            {
                if (seriesItems.Name == "aliases") this.Items[seriesItems.Name] = JSONHelper.flatten((JToken)seriesItems.Value, "|");
                else if (seriesItems.Name == "genre") this.Items[seriesItems.Name] = JSONHelper.flatten((JToken)seriesItems.Value, "|");
                else try
                    {
                        if (seriesItems.Value != null) this.Items[seriesItems.Name] = (string)seriesItems.Value;
                    }
                    catch (ArgumentException ae) {
                       logger.Warn("Could not parse Json for " + seriesItems.Name + " :" + ae.Message);
                    }
            }


            this.TVDBCode = (int)r["id"];
            if ((string)r["seriesName"] != null)
            {
                this.Name = (string)r["seriesName"];
            }


            long updateTime;
            if (long.TryParse((string)r["lastUpdated"], out updateTime) )
                this.Srv_LastUpdated = updateTime;
            else
                this.Srv_LastUpdated = 0;

            string theDate = (string)r["firstAired"];
            try
            {
                if (!String.IsNullOrEmpty(theDate)) {
                    this.FirstAired = DateTime.ParseExact(theDate, "yyyy-MM-dd", new System.Globalization.CultureInfo(""));
                    this.Items["firstAired"] = this.FirstAired.Value.ToString("yyyy-MM-dd");
                    this.Items["Year"] = this.FirstAired.Value.ToString("yyyy");
                }else
                {
                    this.FirstAired = null;
                    this.Items["firstAired"] = "";
                    this.Items["Year"] = "";
                }
            }
            catch
            {
                this.FirstAired = null;
                this.Items["firstAired"] = "";
                this.Items["Year"] = "";
            }


            this.AirsTime = DateTime.Parse("20:00");
            string theAirsTime = (string)r["airsTime"];
            try
            {
                if (!string.IsNullOrEmpty(theAirsTime))
                {
                    this.Items["airsTime"] = theAirsTime;
                    DateTime airsTime;
                    if (DateTime.TryParse(theAirsTime, out airsTime) |
                        DateTime.TryParse(theAirsTime.Replace('.', ':'), out airsTime))
                        this.AirsTime = airsTime;
                    else
                        this.AirsTime = null;
                }
            }
            catch (FormatException)
            {
            }

        }

        public void LoadJSON(JObject bestLanguageR, JObject backupLanguageR)
        {
            //Here we have two pieces of JSON. One in local language and one in the default language (English). 
            //We will populate with the best language frst and then fill in any gaps with the backup Language
            LoadJSON(bestLanguageR);

            //backupLanguageR should be a series of name/value pairs (ie a JArray of JPropertes)
            //TVDB asserts that name and overview are the fields that are localised

            if ((string.IsNullOrWhiteSpace(this.Name) && ((string)backupLanguageR["seriesName"] != null)) ){
                this.Name = (string)backupLanguageR["seriesName"];
                this.Items["seriesName"] = this.Name;
            }

            if ((string.IsNullOrWhiteSpace(this.Items["overview"]) && ((string)backupLanguageR["overview"] != null)) ){
                this.Items["overview"] = (string)backupLanguageR["overview"];
            }

            //Looking at the data then the aliases, banner and runtime are also different by language
            
            if ((string.IsNullOrWhiteSpace(this.Items["aliases"])))
            {
                this.Items["aliases"] = JSONHelper.flatten((JToken)backupLanguageR["aliases"], "|");
            }

            if ((string.IsNullOrWhiteSpace(this.Items["runtime"])))
            {
                this.Items["runtime"] = (string)backupLanguageR["runtime"];
            }
            if ((string.IsNullOrWhiteSpace(this.Items["banner"])))
            {
                this.Items["banner"] = (string)backupLanguageR["banner"];
            }


        }


        public string getStatus() =>              getValueAcrossVersions("Status", "status", "Unknown");
        public string getAirsTime() =>             getValueAcrossVersions("Airs_Time", "airsTime", "");
        public string getAirsDay()=> getValueAcrossVersions("Airs_DayOfWeek", "airsDayOfWeek", "");
        public string getNetwork() => getValueAcrossVersions("Network", "network", "");
        public string GetOverview() => getValueAcrossVersions("Overview", "overview", "");
        public string GetRuntime() => getValueAcrossVersions("Runtime", "runtime", "");
        public string GetContentRating() => getValueAcrossVersions("Rating","rating",""); 
        public string GetSiteRating() => getValueAcrossVersions("SiteRating", "siteRating", ""); 
        public string GetIMDB() => getValueAcrossVersions("IMDB_ID", "imdb_id", "");
        public string GetYear() => getValueAcrossVersions("Year", "year", "");
        public string GetFirstAired() => getValueAcrossVersions("FirstAired", "firstAired", "");

        public string[] GetGenres()
        {

            String genreString = getValueAcrossVersions("Genre", "genre", "");

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
        string getValueAcrossVersions(string oldTag, string newTag, string defaultValue)
        {
            //Need to cater for new and old style tags (TVDB interface v1 vs v2)
            if (this.Items.ContainsKey(oldTag)) return this.Items[oldTag];
            if (this.Items.ContainsKey(newTag)) return this.Items[newTag];
            return defaultValue;
        }



        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Series");

            XMLHelper.WriteElementToXML(writer, "id", this.TVDBCode);
            XMLHelper.WriteElementToXML(writer, "SeriesName", this.Name);
            XMLHelper.WriteElementToXML(writer, "lastupdated", this.Srv_LastUpdated);
            XMLHelper.WriteElementToXML(writer, "LanguageId", this.LanguageId);
            XMLHelper.WriteElementToXML(writer, "Airs_Time", this.AirsTime );
            
            if (this.FirstAired != null)
            {
                XMLHelper.WriteElementToXML(writer, "FirstAired", this.FirstAired.Value.ToString("yyyy-MM-dd"));
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

            foreach (KeyValuePair<string, string> kvp in this.Items)
            {
                if (!skip.Contains(kvp.Key))
                {
                    XMLHelper.WriteElementToXML(writer, kvp.Key, kvp.Value);
                }
            }

            writer.WriteEndElement(); // series
        }

        public Season GetOrAddAiredSeason(int num, int seasonID)
        {
            if (this.AiredSeasons.ContainsKey(num))
                return this.AiredSeasons[num];

            Season s = new Season(this, num, seasonID);
            this.AiredSeasons[num] = s;

            return s;
        }


        public Season GetOrAddDVDSeason(int num, int seasonID)
        {
            if (this.DVDSeasons.ContainsKey(num))
                return this.DVDSeasons[num];

            Season s = new Season(this, num, seasonID);
            this.DVDSeasons[num] = s;

            return s;
        }


        public string GetSeasonBannerPath(int snum)
        {
            //We aim to return the season and language specific poster,
            //if not then a season specific one is best
            //if not then the poster is the fallback

            System.Diagnostics.Debug.Assert(this.BannersLoaded);

            if (this.SeasonLangBanners.ContainsKey(snum))
                return this.SeasonLangBanners[snum].BannerPath;

            if (this.SeasonBanners.ContainsKey(snum))
                return this.SeasonBanners[snum].BannerPath;

            //if there is a problem then return the non-season specific poster by default
            return GetSeriesPosterPath();
                
        }

        public string GetSeriesWideBannerPath()
        {

            //ry the best one we've found with the correct language
            if (this.bestSeriesLangBannerId != -1) return this.AllBanners[this.bestSeriesLangBannerId].BannerPath;

            //if there are none with the righ tlanguage then try one from another language
            if (this.bestSeriesBannerId  != -1) return this.AllBanners[this.bestSeriesBannerId].BannerPath;

            //then choose the one the TVDB recommended _LOWERED IN PRIORITY AFTER LEVERAGE issue - https://github.com/TV-Rename/tvrename/issues/285
            if (!string.IsNullOrEmpty(GetItem("banner"))) return GetItem("banner");

            //give up
            return "";
        }

        public string GetSeriesPosterPath()
        {
            //firstly choose the one the TVDB recommended
            if (!string.IsNullOrEmpty(GetItem("poster"))) return GetItem("poster");

            //then try the best one we've found with the correct language
            if (this.bestSeriesLangPosterId != -1) return this.AllBanners[this.bestSeriesLangPosterId].BannerPath;

            //if there are none with the righ tlanguage then try one from another language
            if (this.bestSeriesPosterId != -1) return this.AllBanners[this.bestSeriesPosterId].BannerPath;

            //give up
            return "";
        }

        public string GetSeriesFanartPath()
        {
            //firstly choose the one the TVDB recommended
            if (!string.IsNullOrEmpty(GetItem("fanart"))) return GetItem("fanart");

            //then try the best one we've found with the correct language
            if (this.bestSeriesLangFanartId != -1) return this.AllBanners[this.bestSeriesLangFanartId].BannerPath;

            //if there are none with the righ tlanguage then try one from another language
            if (this.bestSeriesFanartId != -1) return this.AllBanners[this.bestSeriesFanartId].BannerPath;

            //give up
            return "";
        }

        public string GetSeasonWideBannerPath(int snum)
        {
            //We aim to return the season and language specific poster,
            //if not then a season specific one is best
            //if not then the poster is the fallback

            System.Diagnostics.Debug.Assert(this.BannersLoaded);

            if (this.SeasonLangWideBanners.ContainsKey(snum))
                return this.SeasonLangWideBanners[snum].BannerPath;

            if (this.SeasonWideBanners.ContainsKey(snum))
                return this.SeasonWideBanners[snum].BannerPath;

            //if there is a problem then return the non-season specific poster by default
            return GetSeriesWideBannerPath();
        }



        public void AddOrUpdateBanner(Banner banner)
        {
            if (this.AllBanners.ContainsKey(banner.BannerId)) {
                this.AllBanners[banner.BannerId] = banner;
            } else {
                this.AllBanners.Add(banner.BannerId, banner);
            }

            if (banner.isSeasonPoster()) AddOrUpdateSeasonPoster(banner);
            if (banner.isSeasonBanner()) AddOrUpdateWideSeason(banner);

            if (banner.isSeriesPoster()) this.bestSeriesPosterId = GetBestBannerId(banner, this.bestSeriesPosterId);
            if (banner.isSeriesBanner()) this.bestSeriesBannerId = GetBestBannerId(banner, this.bestSeriesBannerId);
            if (banner.isFanart()) this.bestSeriesFanartId = GetBestBannerId(banner, this.bestSeriesFanartId);

            if (banner.LanguageId == this.LanguageId)
            {
                if (banner.isSeriesPoster()) this.bestSeriesLangPosterId = GetBestBannerId(banner, this.bestSeriesLangPosterId);
                if (banner.isSeriesBanner()) this.bestSeriesLangBannerId = GetBestBannerId(banner, this.bestSeriesLangBannerId);
                if (banner.isFanart()) this.bestSeriesLangFanartId = GetBestBannerId(banner, this.bestSeriesLangFanartId);

            }
        }

        private int GetBestBannerId(Banner selectedBanner, int bestBannerId)
        {
            if (bestBannerId == -1) return selectedBanner.BannerId;

            if (this.AllBanners[bestBannerId].Rating < selectedBanner.Rating)
            {
                //update banner - we have found a better one
                return selectedBanner.BannerId;
            }

            return bestBannerId;
        }


        public void AddOrUpdateSeasonPoster(Banner banner)
        {
            AddUpdateIntoCollections(banner, this.SeasonBanners, this.SeasonLangBanners);
        }

        public void AddOrUpdateWideSeason(Banner banner)
        {
            AddUpdateIntoCollections(banner, this.SeasonWideBanners, this.SeasonLangWideBanners);
        }

        private void AddUpdateIntoCollections(Banner banner, Dictionary<int, Banner> coll, Dictionary<int, Banner> langColl)
        {
            //update language specific cache if appropriate
            if (banner.LanguageId == this.LanguageId)
            {
                AddUpdateIntoCollection(banner,langColl);
            }

            //Now do the same for the all banners dictionary
            AddUpdateIntoCollection(banner, coll);
            
        }

        private void AddUpdateIntoCollection(Banner banner, Dictionary<int, Banner> coll)
        {
            int seasonOfNewBanner = banner.SeasonID;

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

        internal Episode getEpisode(int seasF, int epF,bool dvdOrder)
        {
            if (dvdOrder)
            {
                foreach (Season s in this.DVDSeasons.Values)
                {
                    if (s.SeasonNumber == seasF)
                    {
                        foreach (Episode pe in s.Episodes)
                        {
                            if (pe.DVDEpNum == epF) return pe;
                        }
                    }
                }

            }
            else
            {
                foreach (Season s in this.AiredSeasons.Values)
                {
                    if (s.SeasonNumber == seasF)
                    {
                        foreach (Episode pe in s.Episodes)
                        {
                            if (pe.AiredEpNum == epF) return pe;
                        }
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

        internal void RemoveEpisode(int episodeID)
        {
            //Remove from Aired and DVD Seasons
            foreach (Season s in this.DVDSeasons.Values)
            {
                s.RemoveEpisode(episodeID);
            }
            foreach (Season s in this.AiredSeasons.Values)
            {
                s.RemoveEpisode(episodeID);
            }


        }
    }
}
