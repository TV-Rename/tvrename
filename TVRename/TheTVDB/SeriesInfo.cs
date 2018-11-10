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
        private Dictionary<string, string> items; // e.g. Overview, Banner, Poster, etc.
        public readonly string TargetLanguageCode; //The Language Code we'd like the Series in ; null if we want to use the system setting
        private int languageId; //The actual language obtained
        public string Name;
        public bool BannersLoaded;

        public Dictionary<int, Season> AiredSeasons;
        public Dictionary<int, Season> DvdSeasons;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        //All Banners
        public Dictionary<int, Banner> AllBanners; // All Banners linked by bannerId.

        //Collections of Posters and Banners per season
        private Dictionary<int, Banner> seasonBanners; // e.g. Dictionary of the best posters per series.
        private Dictionary<int, Banner> seasonLangBanners; // e.g. Dictionary of the best posters per series in the correct language.
        private Dictionary<int, Banner> seasonWideBanners; // e.g. Dictionary of the best wide banners per series.
        private Dictionary<int, Banner> seasonLangWideBanners; // e.g. Dictionary of the best wide banners per series in the correct language.

        //best Banner, Poster and Fanart loaded from the images files (in any language)
        private int bestSeriesPosterId;
        private int bestSeriesBannerId;
        private int bestSeriesFanartId;

        //best Banner, Poster and Fanart loaded from the images files (in our language)
        private int bestSeriesLangPosterId;
        private int bestSeriesLangBannerId;
        private int bestSeriesLangFanartId;

        public long SrvLastUpdated;
        public int TvdbCode;
        public string TempTimeZone;

        public bool UseCustomLanguage => TargetLanguageCode != null;

        public int MinYear()
        {
            int min = 9999;
            foreach (Season s in AiredSeasons.Values)
            {
                min = Math.Min(min,s.MinYear());
            }
            return min;
        }

        public int MaxYear()
        {
            int max = 0;
            foreach (Season s in AiredSeasons.Values)
            {
                max = Math.Max(max, s.MaxYear());
            }
            return max;
        }
        
        public DateTime? LastAiredDate() {
            DateTime? returnValue = null; 
            foreach (Season s in AiredSeasons.Values) //We can use AiredSeasons as it does not matter which order we do this in Aired or DVD
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
            Name = name;
            TvdbCode = id;
        }

        public SeriesInfo(string name, int id, string langCode)
        {
            SetToDefauts();
            TargetLanguageCode = langCode;
            Name = name;
            TvdbCode = id;
        }

        public SeriesInfo(XmlReader r)
        {
            SetToDefauts();
            LoadXml(r);
        }

        public SeriesInfo(JObject json,int langId)
        {
            SetToDefauts();
            languageId = langId;
            LoadJson(json);

            if (string.IsNullOrEmpty(Name))
            {
               Logger.Warn("Issue with series " + TvdbCode );
               Logger.Warn(json.ToString());
            }
        }

        public SeriesInfo(JObject json, JObject jsonInDefaultLang, int langId)
        {
            SetToDefauts();
            languageId = langId;
            LoadJson(json,jsonInDefaultLang);
            if (string.IsNullOrEmpty(Name)            ){
               Logger.Warn("Issue with series " + TvdbCode );
               Logger.Warn(json.ToString());
               Logger.Info(jsonInDefaultLang .ToString());
            }
        }

        private List<Actor> actors;

        public IEnumerable<Actor> GetActors() => actors;

        public IEnumerable<string> GetActorNames() => GetActors().Select(x => x.ActorName);

        private string GetItem(string which) //MS making this private to avoid external classes having to worry about how the items colelction is keyed
        {
            return items.ContainsKey(which) ? items[which] : "";
        }

        private void SetToDefauts()
        {
            items = new Dictionary<string, string>();
            AiredSeasons = new Dictionary<int, Season>();
            DvdSeasons = new Dictionary<int, Season>();
            actors=new List<Actor>();
            Dirty = false;
            Name = "";
            AirsTime = null;
            TvdbCode = -1;
            languageId = -1;
            ResetBanners();
        }

        private void ResetBanners()
        {
            BannersLoaded = false;
            AllBanners = new Dictionary<int, Banner>();
            seasonBanners = new Dictionary<int, Banner>();
            seasonLangBanners = new Dictionary<int, Banner>();
            seasonWideBanners = new Dictionary<int, Banner>();
            seasonLangWideBanners = new Dictionary<int, Banner>();

            bestSeriesPosterId = -1;
            bestSeriesBannerId = -1;
            bestSeriesFanartId = -1;
            bestSeriesLangPosterId = -1;
            bestSeriesLangBannerId = -1;
            bestSeriesLangFanartId = -1;
        }

        public void Merge(SeriesInfo o, int preferredLanguageId)
        {
            if (o.TvdbCode != TvdbCode)
                return; // that's not us!
            if (o.SrvLastUpdated != 0 && o.SrvLastUpdated < SrvLastUpdated)
                return; // older!?

            bool currentLanguageNotSet = o.languageId == -1;
            bool newLanguageBetter = o.languageId == preferredLanguageId && languageId != preferredLanguageId;
            bool betterLanguage = currentLanguageNotSet || newLanguageBetter;

            SrvLastUpdated = o.SrvLastUpdated;

            // take the best bits of "o"
            // "o" is always newer/better than us, if there is a choice
            if ((!string.IsNullOrEmpty(o.Name)) && betterLanguage)
                Name = o.Name;
            // this.Items.Clear();
            foreach (KeyValuePair<string, string> kvp in o.items)
            {
                // on offer is non-empty text, in a better language
                // or text for something we don't have
                if ((!string.IsNullOrEmpty(kvp.Value) && betterLanguage) ||
                     (!items.ContainsKey(kvp.Key) || string.IsNullOrEmpty(items[kvp.Key])))
                    items[kvp.Key] = kvp.Value;
            }
            if (o.AirsTime != null)
                AirsTime = o.AirsTime;

            if ((o.AiredSeasons != null) && (o.AiredSeasons.Count != 0))
                AiredSeasons = o.AiredSeasons;
            if ((o.DvdSeasons != null) && (o.DvdSeasons.Count != 0))
                DvdSeasons = o.DvdSeasons;

            if ((o.seasonBanners != null) && (o.seasonBanners.Count != 0))
                seasonBanners = o.seasonBanners;

            if ((o.seasonLangBanners != null) && (o.seasonLangBanners.Count != 0))
                seasonLangBanners = o.seasonLangBanners;

            if ((o.seasonLangWideBanners != null) && (o.seasonLangWideBanners.Count != 0))
                seasonLangWideBanners = o.seasonLangWideBanners;

            if ((o.seasonWideBanners != null) && (o.seasonWideBanners.Count != 0))
                seasonWideBanners = o.seasonWideBanners;

            if ((o.AllBanners != null) && (o.AllBanners.Count != 0))
                AllBanners = o.AllBanners;

            if ((o.bestSeriesPosterId != -1)) bestSeriesPosterId = o.bestSeriesPosterId;
            if ((o.bestSeriesBannerId != -1) ) bestSeriesBannerId = o.bestSeriesBannerId;
            if ((o.bestSeriesFanartId != -1) ) bestSeriesFanartId = o.bestSeriesFanartId;
            if ((o.bestSeriesLangPosterId != -1)) bestSeriesLangPosterId = o.bestSeriesLangPosterId;
            if ((o.bestSeriesLangBannerId != -1)) bestSeriesLangBannerId = o.bestSeriesLangBannerId;
            if ((o.bestSeriesLangFanartId != -1)) bestSeriesLangFanartId = o.bestSeriesLangFanartId;

            if (betterLanguage)
                languageId = o.languageId;

            Dirty = o.Dirty;
        }

        private void LoadXml(XmlReader r)
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
                        TvdbCode = r.ReadElementContentAsInt();
                    else if (r.Name == "SeriesName")
                        Name = System.Web.HttpUtility.HtmlDecode(XmlHelper.ReadStringFixQuotesAndSpaces(r));
                    else if (r.Name == "lastupdated")
                        SrvLastUpdated = r.ReadElementContentAsLong();
                    else if ((r.Name == "Language") || (r.Name == "language"))
                        r.ReadElementContentAsString();
                    else if ((r.Name == "LanguageId") || (r.Name == "languageId"))
                        languageId = r.ReadElementContentAsInt();
                    else if (r.Name == "TimeZone")
                        TempTimeZone = r.ReadElementContentAsString();
                    else if (r.Name == "Airs_Time")
                    {
                        string theTime = r.ReadElementContentAsString();
                        items["Airs_Time"] = theTime;
                        AirsTime = ParseAirTime(theTime);
                    }
                    else if (r.Name == "FirstAired")
                    {
                        string theDate = r.ReadElementContentAsString();

                        try
                        {
                            FirstAired = DateTime.ParseExact(theDate, "yyyy-MM-dd",
                                new System.Globalization.CultureInfo(""));

                            items["FirstAired"] = FirstAired.Value.ToString("yyyy-MM-dd");
                            items["Year"] = FirstAired.Value.ToString("yyyy");
                        }
                        catch
                        {
                            Logger.Trace("Failed to parse date: {0} ", theDate);
                            FirstAired = null;
                            items["FirstAired"] = "";
                            items["Year"] = "";
                        }
                    }
                    else if (r.Name == "Actors")
                    {
                        if (!r.IsStartElement())
                        {
                            r.Read();
                        }
                        else
                        {
                            ClearActors();
                            r.ReadStartElement();
                            //We may have an old style (single field with pipe delimiters) or new style (structured) format
                            if (r.HasValue)
                            {
                                string actorsNames = r.ReadContentAsString();
                                if (!string.IsNullOrEmpty(actorsNames))
                                    foreach (string aName in actorsNames.Split('|'))
                                        AddActor(new Actor(aName));
                            }
                        }
                    }
                    else if (r.Name == "Actor")
                    {
                        Actor a = new Actor(r.ReadSubtree());
                        AddActor(a);
                        r.Read();
                    }
                    else
                    {
                        string name = r.Name;
                        items[name] = r.ReadElementContentAsString();
                    }
                    //   r->ReadOuterXml(); // skip
                } // while
            } // try
            catch (XmlException e)
            {
                string message = "Error processing data from TheTVDB for a show.";
                if (TvdbCode != -1)
                    message += "\r\nTheTVDB Code: " + TvdbCode;
                if (!string.IsNullOrEmpty(Name))
                    message += "\r\nName: " + Name;

                message += "\r\nLanguage: \"" + languageId + "\"";

                message += "\r\n" + e.Message;

                Logger.Error(e,message);

                throw new TheTVDB.TVDBException(e.Message);
            }
        }

        private static DateTime? ParseAirTime(string theTime)
        {
            try
            {
                if (!string.IsNullOrEmpty(theTime))
                {
                    if (DateTime.TryParse(theTime, out DateTime airsTime) ||
                        DateTime.TryParse(theTime.Replace('.', ':'), out airsTime))
                        return airsTime;
                }
            }
            catch (FormatException)
            {
                Logger.Info("Failed to parse time: {0} ", theTime);
            }
            return DateTime.Parse("20:00");
        }

        private void LoadJson(JObject r)
        {
            //r should be a series of name/value pairs (ie a JArray of JPropertes)
            //save them all into the Items array for safe keeping
            foreach (JProperty seriesItems in r.Children<JProperty>())
            {
                if (seriesItems.Name == "aliases") items[seriesItems.Name] = JsonHelper.Flatten(seriesItems.Value, "|");
                else if (seriesItems.Name == "genre") items[seriesItems.Name] = JsonHelper.Flatten(seriesItems.Value, "|");
                else if (seriesItems.Name == "overview")
                    items[seriesItems.Name] = System.Web.HttpUtility.HtmlDecode((string)seriesItems.Value);
                else try
                    {
                        if (seriesItems.Value != null) items[seriesItems.Name] = (string)seriesItems.Value;
                    }
                    catch (ArgumentException ae) {
                       Logger.Warn("Could not parse Json for " + seriesItems.Name + " :" + ae.Message);
                    }
            }

            TvdbCode = (int)r["id"];
            if ((string)r["seriesName"] != null)
            {
                Name = System.Web.HttpUtility.HtmlDecode((string)r["seriesName"]);
            }

            if (long.TryParse((string)r["lastUpdated"], out long updateTime) )
                SrvLastUpdated = updateTime;
            else
                SrvLastUpdated = 0;

            string theDate = (string)r["firstAired"];
            try
            {
                if (!string.IsNullOrEmpty(theDate)) {
                    FirstAired = DateTime.ParseExact(theDate, "yyyy-MM-dd", new System.Globalization.CultureInfo(""));
                    items["firstAired"] = FirstAired.Value.ToString("yyyy-MM-dd");
                    items["Year"] = FirstAired.Value.ToString("yyyy");
                }
                else
                {
                    FirstAired = null;
                    items["firstAired"] = "";
                    items["Year"] = "";
                }
            }
            catch
            {
                FirstAired = null;
                items["firstAired"] = "";
                items["Year"] = "";
            }

            AirsTime = ParseAirTime((string) r["airsTime"]);
        }

        private void LoadJson(JObject bestLanguageR, JObject backupLanguageR)
        {
            //Here we have two pieces of JSON. One in local language and one in the default language (English). 
            //We will populate with the best language frst and then fill in any gaps with the backup Language
            LoadJson(bestLanguageR);

            //backupLanguageR should be a series of name/value pairs (ie a JArray of JPropertes)
            //TVDB asserts that name and overview are the fields that are localised

            if ((string.IsNullOrWhiteSpace(Name) && ((string)backupLanguageR["seriesName"] != null)) ){
                Name = (string)backupLanguageR["seriesName"];
                items["seriesName"] = System.Web.HttpUtility.HtmlDecode(Name);
            }

            if ((string.IsNullOrWhiteSpace(items["overview"]) && ((string)backupLanguageR["overview"] != null)) ){
                items["overview"] = System.Web.HttpUtility.HtmlDecode((string)backupLanguageR["overview"]);
            }

            //Looking at the data then the aliases, banner and runtime are also different by language

            if ((string.IsNullOrWhiteSpace(items["aliases"])))
            {
                items["aliases"] = JsonHelper.Flatten(backupLanguageR["aliases"], "|");
            }

            if ((string.IsNullOrWhiteSpace(items["runtime"])))
            {
                items["runtime"] = (string)backupLanguageR["runtime"];
            }

            if ((string.IsNullOrWhiteSpace(items["banner"])))
            {
                items["banner"] = (string)backupLanguageR["banner"];
            }
        }

        public string GetStatus() =>              GetValueAcrossVersions("Status", "status", "Unknown");
        public string GetAirsTime() =>             GetValueAcrossVersions("Airs_Time", "airsTime", "");
        public string GetAirsDay()=> GetValueAcrossVersions("Airs_DayOfWeek", "airsDayOfWeek", "");
        public string GetNetwork() => GetValueAcrossVersions("Network", "network", "");
        public string GetOverview() => GetValueAcrossVersions("Overview", "overview", "");
        public string GetRuntime() => GetValueAcrossVersions("Runtime", "runtime", "");
        public string GetContentRating() => GetValueAcrossVersions("Rating","rating",""); 
        public string GetSiteRating() => GetValueAcrossVersions("SiteRating", "siteRating", "");
        public string GetSiteRatingVotes() => GetValueAcrossVersions("SiteRatingCount", "siteRatingCount", "");
        public string GetImdb() => GetValueAcrossVersions("IMDB_ID", "imdbId", "");
        public string GetYear() => GetValueAcrossVersions("Year", "year", "");
        public string GetFirstAired() => GetValueAcrossVersions("FirstAired", "firstAired", "");
        public string GetSeriesId() => GetValueAcrossVersions("SeriesID", "seriesId", "");

        public string[] GetGenres()
        {
            string genreString = GetValueAcrossVersions("Genre", "genre", "");

            if (!string.IsNullOrEmpty(genreString))
            {
                return genreString.Split('|');
            }
            return new string[] { };
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
            if (items.ContainsKey(oldTag)) return items[oldTag];
            if (items.ContainsKey(newTag)) return items[newTag];
            return defaultValue;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Series");

            XmlHelper.WriteElementToXml(writer, "id", TvdbCode);
            XmlHelper.WriteElementToXml(writer, "SeriesName", Name);
            XmlHelper.WriteElementToXml(writer, "lastupdated", SrvLastUpdated);
            XmlHelper.WriteElementToXml(writer, "LanguageId", languageId);
            XmlHelper.WriteElementToXml(writer, "Airs_Time", AirsTime );
            
            if (FirstAired != null)
            {
                XmlHelper.WriteElementToXml(writer, "FirstAired", FirstAired.Value.ToString("yyyy-MM-dd"));
            }

            writer.WriteStartElement("Actors");
            foreach (Actor aa in GetActors())
            {
                aa.WriteXml(writer);
            }
            writer.WriteEndElement(); //Actors

            List<string> skip = new List<string>
                                  {
                                      "Airs_Time",
                                      "lastupdated","lastUpdated",
                                      "id","seriesName","seriesname","SeriesName",
                                      "lastUpdated","lastupdated",
                                      "FirstAired","firstAired",
                                      "LanguageId","TimeZone","Actors"
                                  };

            foreach (KeyValuePair<string, string> kvp in items)
            {
                if (!skip.Contains(kvp.Key))
                {
                    XmlHelper.WriteElementToXml(writer, kvp.Key, kvp.Value);
                }
            }

            writer.WriteEndElement(); // series
        }

        public Season GetOrAddAiredSeason(int num, int seasonId)
        {
            if (AiredSeasons.ContainsKey(num))
                return AiredSeasons[num];

            Season s = new Season(this, num, seasonId,Season.SeasonType.aired);
            AiredSeasons[num] = s;

            return s;
        }

        public Season GetOrAddDvdSeason(int num, int seasonId)
        {
            if (DvdSeasons.ContainsKey(num))
                return DvdSeasons[num];

            Season s = new Season(this, num, seasonId,Season.SeasonType.dvd);
            DvdSeasons[num] = s;

            return s;
        }

        public string GetSeasonBannerPath(int snum)
        {
            //We aim to return the season and language specific poster,
            //if not then a season specific one is best
            //if not then the poster is the fallback

            System.Diagnostics.Debug.Assert(BannersLoaded);

            if (seasonLangBanners.ContainsKey(snum))
                return seasonLangBanners[snum].BannerPath;

            if (seasonBanners.ContainsKey(snum))
                return seasonBanners[snum].BannerPath;

            //if there is a problem then return the non-season specific poster by default
            return GetSeriesPosterPath();
        }

        public string GetSeriesWideBannerPath()
        {
            //ry the best one we've found with the correct language
            if (bestSeriesLangBannerId != -1) return AllBanners[bestSeriesLangBannerId].BannerPath;

            //if there are none with the right language then try one from another language
            if (bestSeriesBannerId  != -1) return AllBanners[bestSeriesBannerId].BannerPath;

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
            if (bestSeriesLangPosterId != -1) return AllBanners[bestSeriesLangPosterId].BannerPath;

            //if there are none with the righ tlanguage then try one from another language
            if (bestSeriesPosterId != -1) return AllBanners[bestSeriesPosterId].BannerPath;

            //give up
            return "";
        }

        public string GetSeriesFanartPath()
        {
            //firstly choose the one the TVDB recommended
            if (!string.IsNullOrEmpty(GetItem("fanart"))) return GetItem("fanart");

            //then try the best one we've found with the correct language
            if (bestSeriesLangFanartId != -1) return AllBanners[bestSeriesLangFanartId].BannerPath;

            //if there are none with the righ tlanguage then try one from another language
            if (bestSeriesFanartId != -1) return AllBanners[bestSeriesFanartId].BannerPath;

            //give up
            return "";
        }

        public string GetSeasonWideBannerPath(int snum)
        {
            //We aim to return the season and language specific poster,
            //if not then a season specific one is best
            //if not then the poster is the fallback

            System.Diagnostics.Debug.Assert(BannersLoaded);

            if (seasonLangWideBanners.ContainsKey(snum))
                return seasonLangWideBanners[snum].BannerPath;

            if (seasonWideBanners.ContainsKey(snum))
                return seasonWideBanners[snum].BannerPath;

            //if there is a problem then return the non-season specific poster by default
            return GetSeriesWideBannerPath();
        }

        public void AddOrUpdateBanner(Banner banner)
        {
            if (AllBanners.ContainsKey(banner.BannerId)) {
                AllBanners[banner.BannerId] = banner;
            }
            else
            {
                AllBanners.Add(banner.BannerId, banner);
            }

            if (banner.IsSeasonPoster()) AddOrUpdateSeasonPoster(banner);
            if (banner.IsSeasonBanner()) AddOrUpdateWideSeason(banner);

            if (banner.IsSeriesPoster()) bestSeriesPosterId = GetBestBannerId(banner, bestSeriesPosterId);
            if (banner.IsSeriesBanner()) bestSeriesBannerId = GetBestBannerId(banner, bestSeriesBannerId);
            if (banner.IsFanart()) bestSeriesFanartId = GetBestBannerId(banner, bestSeriesFanartId);

            if (banner.LanguageId == languageId)
            {
                if (banner.IsSeriesPoster()) bestSeriesLangPosterId = GetBestBannerId(banner, bestSeriesLangPosterId);
                if (banner.IsSeriesBanner()) bestSeriesLangBannerId = GetBestBannerId(banner, bestSeriesLangBannerId);
                if (banner.IsFanart()) bestSeriesLangFanartId = GetBestBannerId(banner, bestSeriesLangFanartId);
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

        private void AddOrUpdateSeasonPoster(Banner banner)
        {
            AddUpdateIntoCollections(banner, seasonBanners, seasonLangBanners);
        }

        private void AddOrUpdateWideSeason(Banner banner)
        {
            AddUpdateIntoCollections(banner, seasonWideBanners, seasonLangWideBanners);
        }

        private void AddUpdateIntoCollections(Banner banner, Dictionary<int, Banner> coll, Dictionary<int, Banner> langColl)
        {
            //update language specific cache if appropriate
            if (banner.LanguageId == languageId)
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

        internal Episode GetEpisode(int seasF, int epF,bool dvdOrder)
        {
            if (dvdOrder)
            {
                foreach (Season s in DvdSeasons.Values)
                {
                    if (s.SeasonNumber == seasF)
                    {
                        foreach (Episode pe in s.Episodes.Values)
                        {
                            if (pe.DvdEpNum == epF) return pe;
                        }
                    }
                }
            }
            else
            {
                foreach (Season s in AiredSeasons.Values)
                {
                    if (s.SeasonNumber == seasF)
                    {
                        foreach (Episode pe in s.Episodes.Values)
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

            protected EpisodeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }

        internal void RemoveEpisode(int episodeId)
        {
            //Remove from Aired and DVD Seasons
            foreach (Season s in DvdSeasons.Values)
            {
                s.RemoveEpisode(episodeId);
            }
            foreach (Season s in AiredSeasons.Values)
            {
                s.RemoveEpisode(episodeId);
            }
        }

        public void ClearActors()
        {
            actors=new List<Actor>();
        }

        public void AddActor(Actor actor)
        {
            actors.Add(actor);
        }

        public string GetImdbNumber()
        {
            return (GetImdb().StartsWith("tt")) ? GetImdb().Substring(2): GetImdb();
        }

        public int GetSeasonIndex(int seasonNumber, Season.SeasonType type)
        {
            Dictionary<int, Season> appropriateSeasons = type == Season.SeasonType.aired ? AiredSeasons : DvdSeasons;

            List<int> seasonNumbers = new List<int>();
            foreach (KeyValuePair<int, Season> sn in appropriateSeasons)
            {
                if (sn.Value.IsSpecial()) continue;

                seasonNumbers.Add(sn.Value.SeasonNumber);
            }

            seasonNumbers.Sort();

            return seasonNumbers.IndexOf(seasonNumber) +1;
        }
    }
}
