// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace TVRename
{
    public class SeriesInfo
    {
        public DateTime? AirsTime;
        public bool Dirty; // set to true if local info is known to be older than whats on the server
        public DateTime? FirstAired;
        public System.Collections.Generic.Dictionary<string, string> Items; // e.g. Overview, Banner, Poster, etc.
        public int LanguageId;
        private string LastFiguredTZ;
        public string Name;
        public bool BannersLoaded;

        public System.Collections.Generic.Dictionary<int, Season> Seasons;

        //All Banners
        public System.Collections.Generic.Dictionary<int, Banner> AllBanners; // All Banners linked by bannerId.

        //Collections of Posters and Banners per season
        private System.Collections.Generic.Dictionary<int, Banner> SeasonBanners; // e.g. Dictionary of the best posters per series.
        private System.Collections.Generic.Dictionary<int, Banner> SeasonLangBanners; // e.g. Dictionary of the best posters per series in the correct language.
        private System.Collections.Generic.Dictionary<int, Banner> SeasonWideBanners; // e.g. Dictionary of the best wide banners per series.
        private System.Collections.Generic.Dictionary<int, Banner> SeasonLangWideBanners; // e.g. Dictionary of the best wide banners per series in the correct language.

        //best Banner, Poster and Fanart loaded from the images files (in any language)
        private int bestSeriesPosterId;
        private int bestSeriesBannerId;
        private int bestSeriesFanartId;

        //best Banner, Poster and Fanart loaded from the images files (in our language)
        private int bestSeriesLangPosterId;
        private int bestSeriesLangBannerId;
        private int bestSeriesLangFanartId;


        private TimeZone SeriesTZ;

        public string ShowTimeZone; // set for us by ShowItem
        public long Srv_LastUpdated;
        public int TVDBCode;

        // note: "SeriesID" in a <Series> is the tv.com code,
        // "seriesid" in an <Episode> is the tvdb code!

        public SeriesInfo(string name, int id)
        {
            this.SetToDefauts();
            this.Name = name;
            this.TVDBCode = id;
        }

        public SeriesInfo(XmlReader r)
        {
            this.SetToDefauts();
            this.LoadXml(r);
        }

        public SeriesInfo(JObject json)
        {
            this.SetToDefauts();
            this.LoadJSON(json);
        }
        private void FigureOutTimeZone()
        {
            string tzstr = this.ShowTimeZone;

            if (string.IsNullOrEmpty(tzstr))
                tzstr = TimeZone.DefaultTimeZone();

            this.SeriesTZ = TimeZone.TimeZoneFor(tzstr);

            this.LastFiguredTZ = tzstr;
        }

        public TimeZone GetTimeZone()
        {
            // we cache the timezone info, as the fetching is a bit slow, and we do this a lot
            if (this.LastFiguredTZ != this.ShowTimeZone)
                this.FigureOutTimeZone();

            return this.SeriesTZ;
        }

        public string GetItem(string which)
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
            this.ShowTimeZone = TimeZone.DefaultTimeZone(); // default, is correct for most shows
            this.LastFiguredTZ = "";

            this.Items = new System.Collections.Generic.Dictionary<string, string>();
            this.Seasons = new System.Collections.Generic.Dictionary<int, Season>();

            this.AllBanners = new System.Collections.Generic.Dictionary<int, Banner>();
            this.SeasonBanners = new System.Collections.Generic.Dictionary<int, Banner>();
            this.SeasonLangBanners = new System.Collections.Generic.Dictionary<int, Banner>();
            this.SeasonWideBanners = new System.Collections.Generic.Dictionary<int, Banner>();
            this.SeasonLangWideBanners = new System.Collections.Generic.Dictionary<int, Banner>();

            this.Dirty = false;
            this.Name = "";
            this.AirsTime = null;
            this.TVDBCode = -1;
            this.LanguageId = -1;
            this.BannersLoaded = false;

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

            bool betterLanguage = (o.LanguageId == preferredLanguageId) && (this.LanguageId != preferredLanguageId);

            this.Srv_LastUpdated = o.Srv_LastUpdated;

            // take the best bits of "o"
            // "o" is always newer/better than us, if there is a choice
            if ((!string.IsNullOrEmpty(o.Name)) && betterLanguage)
                this.Name = o.Name;
            // this.Items.Clear();
            foreach (System.Collections.Generic.KeyValuePair<string, string> kvp in o.Items)
            {
                // on offer is non-empty text, in a better language
                // or text for something we don't have
                if ((!string.IsNullOrEmpty(kvp.Value) && betterLanguage) ||
                     (!this.Items.ContainsKey(kvp.Key) || string.IsNullOrEmpty(this.Items[kvp.Key])))
                    this.Items[kvp.Key] = kvp.Value;
            }
            if (o.AirsTime != null)
                this.AirsTime = o.AirsTime;

            if ((o.Seasons != null) && (o.Seasons.Count != 0))
                this.Seasons = o.Seasons;

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
                        this.ShowTimeZone = r.ReadElementContentAsString();
                    else if (r.Name == "Airs_Time")
                    {
                        this.AirsTime = DateTime.Parse("20:00");

                        try
                        {
                            string theTime = r.ReadElementContentAsString();
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

                MessageBox.Show(message, "TVRename", MessageBoxButtons.OK, MessageBoxIcon.Error);

                throw new TVDBException(e.Message);
            }
        }

        // LoadXml

        public void LoadJSON(JObject r)
        {
            //r should be a series of name/value pairs (ie a JArray of JPropertes)
            //save them all into the Items array for safe keeping
            foreach (JProperty seriesItems in r.Children<JProperty>())
            {
                if (seriesItems.Name == "aliases") this.Items[seriesItems.Name] = JSONHelper.flatten((JArray)seriesItems.Value, "|");
                else if (seriesItems.Name == "genre") this.Items[seriesItems.Name] = JSONHelper.flatten((JArray)seriesItems.Value, "|");
                else try
                    {
                        if (seriesItems.Value != null) this.Items[seriesItems.Name] = (string)seriesItems.Value;
                    }
                    catch (ArgumentException ae) {
                        System.Diagnostics.Debug.Print("Could not parse Json for " + seriesItems.Name + " :" + ae.Message);
                    }
            }


            this.TVDBCode = (int)r["id"];
            if ((string)r["seriesName"] != null) this.Name = (string)r["seriesName"];

            long updateTime;
            if (long.TryParse((string)r["lastUpdated"], out updateTime) )
                this.Srv_LastUpdated = updateTime;
            else
                this.Srv_LastUpdated = 0;

            string theDate = (string)r["firstAired"];
            try
            {
                this.FirstAired = DateTime.ParseExact(theDate, "yyyy-MM-dd", new System.Globalization.CultureInfo(""));
                this.Items["firstAired"] = this.FirstAired.Value.ToString("yyyy-MM-dd");
                this.Items["Year"] = this.FirstAired.Value.ToString("yyyy");
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

        internal string getStatus()
        {
            return getValueAcrossVersions("Status", "status", "Unknown");
        }

        internal string getAirsTime()
        {
            return getValueAcrossVersions("Airs_Time", "airsTime", "");
        }

        internal string getAirsDay()
        {
            return getValueAcrossVersions("Airs_DayOfWeek", "airsDayOfWeek", "");
        }

        internal string getNetwork()
        {
            return getValueAcrossVersions("Network", "network", "");
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
            XMLHelper.WriteElementToXML(writer, "Language", this.LanguageId);

            foreach (System.Collections.Generic.KeyValuePair<string, string> kvp in this.Items)
            {
                XMLHelper.WriteElementToXML(writer, kvp.Key, kvp.Value);
            }
            XMLHelper.WriteElementToXML(writer, "TimeZone", this.ShowTimeZone);

            if (this.FirstAired != null)
            {
                XMLHelper.WriteElementToXML(writer, "FirstAired", this.FirstAired.Value.ToString("yyyy-MM-dd"));
            }

            writer.WriteEndElement(); // series
        }

        public Season GetOrAddSeason(int num, int seasonID)
        {
            if (this.Seasons.ContainsKey(num))
                return this.Seasons[num];

            Season s = new Season(this, num, seasonID);
            this.Seasons[num] = s;

            return s;
        }

        public string GetSeasonBannerPath(int snum)
        {
            //We aim to return the season and language specific poster,
            //if not then a season specific one is best
            //if not then the poster is the fallback

            System.Diagnostics.Debug.Assert(BannersLoaded);

            if (this.SeasonLangBanners.ContainsKey(snum))
                return this.SeasonLangBanners[snum].BannerPath;

            if (this.SeasonBanners.ContainsKey(snum))
                return this.SeasonBanners[snum].BannerPath;

            //if there is a problem then return the non-season specific poster by default
            return GetItem("poster");
        }

        public string GetSeriesWideBannerPath()
        {
            //firstly choose the one the TVDB recommended
            if (!string.IsNullOrEmpty(GetItem("banner"))) return GetItem("banner");

            //then try the best one we've found with the correct language
            if (bestSeriesLangBannerId != -1) return AllBanners[bestSeriesLangBannerId].BannerPath;

            //if there are none with the righ tlanguage then try one from another language
            if (bestSeriesBannerId  != -1) return AllBanners[bestSeriesBannerId].BannerPath;

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

            if (this.SeasonLangWideBanners.ContainsKey(snum))
                return this.SeasonLangWideBanners[snum].BannerPath;

            if (this.SeasonWideBanners.ContainsKey(snum))
                return this.SeasonWideBanners[snum].BannerPath;

            //if there is a problem then return the non-season specific poster by default
            return GetItem("banner");
        }



        public void AddOrUpdateBanner(Banner banner)
        {
            if (AllBanners.ContainsKey(banner.BannerId)) {
                AllBanners[banner.BannerId] = banner;
            } else {
                AllBanners.Add(banner.BannerId, banner);
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

            if (AllBanners[bestBannerId].Rating < selectedBanner.Rating)
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

        private void AddUpdateIntoCollections(Banner banner, System.Collections.Generic.Dictionary<int, Banner> coll, System.Collections.Generic.Dictionary<int, Banner> langColl)
        {
            //update language specific cache if appropriate
            if (banner.LanguageId == this.LanguageId)
            {
                AddUpdateIntoCollection(banner,langColl);
            }

            //Now do the same for the all banners dictionary
            AddUpdateIntoCollection(banner, coll);
            
        }

        private void AddUpdateIntoCollection(Banner banner, System.Collections.Generic.Dictionary<int, Banner> coll)
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

    }
}