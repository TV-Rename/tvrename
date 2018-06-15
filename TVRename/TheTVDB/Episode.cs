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

namespace TVRename
{
    public class Episode
    {
        public bool Dirty;
        public int AiredEpNum;
        public int DvdEpNum;
        public int EpisodeId;
        public DateTime? FirstAired;
        private Dictionary<string, string> items; // other fields we don't specifically grab
        public string Overview;
        public string EpisodeRating;
        public string EpisodeGuestStars;
        public string EpisodeDirector;
        public string Writer;

        public int ReadAiredSeasonNum; // only use after loading to attach to the correct season!
        public int ReadDvdSeasonNum; // only use after loading to attach to the correct season!
        public int SeasonId;
        public int SeriesId;
        public long SrvLastUpdated;

        public Season TheAiredSeason;
        public Season TheDvdSeason;
        public SeriesInfo TheSeries;
        private string mName;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Episode(Episode o)
        {
            EpisodeId = o.EpisodeId;
            SeriesId = o.SeriesId;
            AiredEpNum = o.AiredEpNum;
            DvdEpNum = o.DvdEpNum;
            FirstAired = o.FirstAired;
            SrvLastUpdated = o.SrvLastUpdated;
            Overview = o.Overview;
            EpisodeRating = o.EpisodeRating;
            EpisodeGuestStars = o.EpisodeGuestStars;
            EpisodeDirector = o.EpisodeDirector;
            Writer = o.Writer;
            Name = o.Name;
            TheAiredSeason = o.TheAiredSeason;
            TheDvdSeason = o.TheDvdSeason;
            TheSeries = o.TheSeries;
            SeasonId = o.SeasonId;
            Dirty = o.Dirty;

            items = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> i in o.items)
                items.Add(i.Key, i.Value);
        }

        public Episode(SeriesInfo ser, Season airSeason, Season dvdSeason)
        {
            SetDefaults(ser, airSeason,dvdSeason);
        }

        public DateTime? GetAirDateDt()
        {
            if (FirstAired == null)
                return null;

            DateTime fa = (DateTime)FirstAired;
            DateTime? airs = TheSeries.AirsTime;

            return new DateTime(fa.Year, fa.Month, fa.Day, airs?.Hour ?? 20, airs?.Minute ?? 0, 0, 0);
        }

        public DateTime? GetAirDateDt(TimeZone tz)
        {
            DateTime? dt = GetAirDateDt();
            if (dt == null) return null;

            return TimeZone.AdjustTZTimeToLocalTime(dt.Value, tz);
        }

        internal IEnumerable<KeyValuePair<string, string>> OtherItems()
        {

            List<string> skip = new List<string>
            {
                "id",
                "airedSeason",
                "airedSeasonID",
                "airedEpisodeNumber",
                "episodeName",
                "overview",
                "lastUpdated",
                "dvdSeason",
                "dvdEpisodeNumber",
                "dvdChapter",
                "absoluteNumber",
                "filename",
                "seriesId",
                "lastUpdatedBy",
                "airsAfterSeason",
                "airsBeforeSeason",
                "airsBeforeEpisode",
                "thumbAuthor",
                "thumbAdded",
                "thumbAdded",
                "thumbWidth",
                "thumbHeight",
                "director",
                "firstAired",
                "Combined_episodenumber",
                "Combined_season",
                "DVD_episodenumber",
                "DVD_season",
                "EpImgFlag",
                "absolute_number",
                "filename",
                "is_movie",
                "thumb_added",
                "thumb_height",
                "thumb_width",
                "EpisodeDirector"
            };
            return items.Where(c => !skip.Contains(c.Key));
            
        }

        public string AirsBeforeSeason => GetValueAcrossVersions("airsBeforeSeason", "airsbefore_season", "");
        public string AirsBeforeEpisode => GetValueAcrossVersions("airsBeforeEpisode", "airsbefore_episode", "");

        public Episode( XmlReader r)
        {
            // <Episode>
            //  <id>...</id>
            //  blah blah
            // </Episode>

            try
            {
                SetDefaults(null, null, null);

                r.Read();
                if (r.Name != "Episode")
                    return;

                r.Read();
                while (!r.EOF)
                {
                    if ((r.Name == "Episode") && (!r.IsStartElement()))
                        break;
                    if (r.Name == "id")
                        EpisodeId = r.ReadElementContentAsInt();
                    else if (r.Name == "seriesid")
                        SeriesId = r.ReadElementContentAsInt(); // thetvdb series id
                    else if (r.Name == "seasonid")
                        SeasonId = r.ReadElementContentAsInt();
                    else if (r.Name == "EpisodeNumber")
                        AiredEpNum = r.ReadElementContentAsInt();
                    else if (r.Name == "dvdEpisodeNumber")
                    {
                        string den = r.ReadElementContentAsString();
                        int.TryParse(den, out DvdEpNum);
                    }
                    else if (r.Name == "SeasonNumber")
                    {
                        string sn = r.ReadElementContentAsString();
                        int.TryParse(sn, out ReadAiredSeasonNum);
                    }
                    else if (r.Name == "dvdSeason")
                    {
                        string dsn = r.ReadElementContentAsString();
                        int.TryParse(dsn, out ReadDvdSeasonNum);
                    }
                    else if (r.Name == "lastupdated")
                        SrvLastUpdated = r.ReadElementContentAsLong();
                    else if (r.Name == "Overview")
                        Overview = XmlHelper.ReadStringFixQuotesAndSpaces(r);
                    else if (r.Name == "Rating")        
                        EpisodeRating = XmlHelper.ReadStringFixQuotesAndSpaces(r);  
                    else if (r.Name == "GuestStars")
                        EpisodeGuestStars = XmlHelper.ReadStringFixQuotesAndSpaces(r);      
                    else if (r.Name == "EpisodeDirector")
                        EpisodeDirector = XmlHelper.ReadStringFixQuotesAndSpaces(r);      
                    else if (r.Name == "Writer")
                        Writer = XmlHelper.ReadStringFixQuotesAndSpaces(r);      
                    else if (r.Name == "EpisodeName")
                        Name = XmlHelper.ReadStringFixQuotesAndSpaces(r);
                    else if (r.Name == "FirstAired")
                    {
                        try
                        {
                            string contents = r.ReadElementContentAsString();
                            if (contents == "")
                            {
                               Logger.Info ("Please confirm, but we are assuming that " + Name +"(episode Id =" +EpisodeId + ") has no airdate");
                                FirstAired = null;
                            } else { 
                                FirstAired = DateTime.ParseExact(contents, "yyyy-MM-dd", new System.Globalization.CultureInfo(""));
                            }
                        }
                        catch
                        {
                            FirstAired = null;
                        }
                    }
                    else
                    {
                        if ((r.IsEmptyElement) || !r.IsStartElement())
                            r.ReadOuterXml();
                        else
                        {
                            XmlReader r2 = r.ReadSubtree();
                            r2.Read();
                            string name = r2.Name;
                            items[name] = r2.ReadElementContentAsString();
                            r.Read();
                        }
                    }
                }
            }
            catch (XmlException e)
            {
                string message = "Error processing data from TheTVDB for an episode.";
                if (SeriesId != -1)
                    message += "\r\nSeries ID: " + SeriesId;
                if (EpisodeId != -1)
                    message += "\r\nEpisode ID: " + EpisodeId;
                if (DvdEpNum != -1)
                    message += "\r\nEpisode (DVD) Number: " + DvdEpNum;
                if (AiredEpNum != -1)
                    message += "\r\nEpisode Aired Number: " + AiredEpNum;
                if (!string.IsNullOrEmpty(Name))
                    message += "\r\nName: " + Name;


                Logger.Error(e, message);

                throw new TheTVDB.TVDBException(e.Message);
            }
        }

        public Episode(int seriesId, JObject json, JObject jsonInDefaultLang)
        {
            SetDefaults(null,null,null);
            LoadJson(seriesId, json, jsonInDefaultLang);
        }

        public Episode(int seriesId,JObject r)
        {
            // <Episode>
            //  <id>...</id>
            //  blah blah
            // </Episode>

            SetDefaults(null, null,null);

            LoadJson(seriesId,r);
        }

        private void LoadJson(int seriesId, JObject bestLanguageR, JObject backupLanguageR)
        {
            //Here we have two pieces of JSON. One in local language and one in the default language (English). 
            //We will populate with the best language frst and then fillin any gaps with the backup Language
            LoadJson(seriesId,bestLanguageR);

            //backupLanguageR should be a series of name/value pairs (ie a JArray of JPropertes)
            //TVDB asserts that name and overview are the fields that are localised

            if ((string.IsNullOrWhiteSpace((string)bestLanguageR["episodeName"]) && ((string)backupLanguageR["episodeName"] != null)))
            {
                Name = (string)backupLanguageR["episodeName"];
                items["episodeName"] = Name;
            }

            if ((string.IsNullOrWhiteSpace(items["overview"]) && ((string)backupLanguageR["overview"] != null)))
            {
                items["overview"] = (string)backupLanguageR["overview"];
                Overview = (string)backupLanguageR["overview"];
            }


        }

        private void LoadJson(int seriesId, JObject r)
        {
            //r should be a series of name/value pairs (ie a JArray of JPropertes)
            //save them all into the Items array for safe keeping
            foreach (JProperty episodeItems in r.Children<JProperty>())
            {
                try
                {
                    JToken currentData = episodeItems.Value;
                    if (currentData.Type == JTokenType.Array) items[episodeItems.Name] = JsonHelper.Flatten((JToken)currentData);
                    else if (currentData.Type != JTokenType.Object) //Ignore objects here as it is always the 'language' attribute that we do not need
                    {
                        JValue currentValue = (JValue)episodeItems.Value;
                        items[episodeItems.Name] = currentValue.ToObject<string>();

                    }

                }
                catch (ArgumentException ae)
                {
                   Logger.Error("Could not parse Json for " + episodeItems.Name + " :" + ae.Message);
                    //ignore as probably a cast exception
                }
                catch (NullReferenceException ae)
                {
                   Logger.Error("Could not parse Json for " + episodeItems.Name + " :" + ae.Message);
                    //ignore as probably a cast exception
                }
                catch (InvalidCastException ae)
                {
                   Logger.Error("Could not parse Json for " + episodeItems.Name + " :" + ae.Message);
                    //ignore as probably a cast exception
                }
            }

            SeriesId = seriesId;
            try{
            EpisodeId = (int)r["id"];

            if ((string)r["airedSeasonID"] != null) { SeasonId = (int)r["airedSeasonID"]; }
            else
            {
               Logger.Error("Issue with episode " + EpisodeId + " for series " + seriesId + " no airedSeasonID " );
               Logger.Error(r.ToString());
            }

            AiredEpNum = (int)r["airedEpisodeNumber"];

            string dvdEpNumString = (string) r["dvdEpisodeNumber"];

            if (string.IsNullOrWhiteSpace(dvdEpNumString)) DvdEpNum = 0;
            else if (!int.TryParse(dvdEpNumString, out DvdEpNum)) DvdEpNum = 0;
            
            SrvLastUpdated = (long)r["lastUpdated"];
            Overview = (string)r["overview"];
            EpisodeRating = (string)r["siteRating"];
            Name = (string)r["episodeName"];

            string sn = (string)r["airedSeason"];
            if (sn == null) {
               Logger.Error("Issue with episode " + EpisodeId + " for series " + seriesId + " airedSeason = null");
               Logger.Error(r.ToString());
            }
            else { int.TryParse(sn, out ReadAiredSeasonNum); }

            string dsn = (string)r["dvdSeason"];
            if (string.IsNullOrWhiteSpace(dsn)) ReadDvdSeasonNum = 0;
            else if (!int.TryParse(dsn, out ReadDvdSeasonNum)) ReadDvdSeasonNum = 0;

            EpisodeGuestStars = JsonHelper.Flatten((JToken)r["guestStars"], "|");
            EpisodeDirector = JsonHelper.Flatten((JToken)r["directors"], "|");
            Writer = JsonHelper.Flatten((JToken)r["writers"], "|");

            try
            {
                string contents = (string)r["firstAired"];
                if (string.IsNullOrEmpty(contents))
                {
                    //if (this.ReadSeasonNum > 0)logger.Info("Please confirm, but we are assuming that " + this.Name + "(episode Id =" + this.EpisodeID + ") has no airdate");
                    FirstAired = null;
                }
                else
                {
                    FirstAired = DateTime.ParseExact(contents, "yyyy-MM-dd", new System.Globalization.CultureInfo(""));
                }
            }
            catch (Exception e)
            {
                Logger.Debug(e, "Failed to parse firstAired");
                FirstAired = null;

            }
                            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to parse : {r.ToString() }");
            }
        }

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(mName))
                    return "Aired Episode " + AiredEpNum;
                return mName;
            }
            set => mName = value;
        }

        public int AiredSeasonNumber
        {
            get
            {
                if (TheAiredSeason != null)
                    return TheAiredSeason.SeasonNumber;
                return -1;
            }
        }

        public int DvdSeasonNumber
        {
            get
            {
                if (TheDvdSeason != null)
                    return TheDvdSeason.SeasonNumber;
                return -1;
            }
        }

        public bool SameAs(Episode o)
        {
            return (EpisodeId == o.EpisodeId);
        }

        public string GetFilename() => GetValueAcrossVersions("filename", "Filename", "");

        public string[] GetGuestStars()
        {

            string guest = EpisodeGuestStars;

            return string.IsNullOrEmpty(guest) ? new string[] { }: guest.Split('|') ;
        }

        private string GetValueAcrossVersions(string oldTag, string newTag, string defaultValue)
        {
            //Need to cater for new and old style tags (TVDB interface v1 vs v2)
            if (items.ContainsKey(oldTag)) return items[oldTag];
            if (items.ContainsKey(newTag)) return items[newTag];
            return defaultValue;
        }

        public bool Ok()
        {
            bool returnVal = (SeriesId != -1) && (EpisodeId != -1) && (AiredEpNum != -1) && (SeasonId != -1) && (ReadAiredSeasonNum != -1) ;
            if (!returnVal)
            {
               Logger.Warn("Issue with episode " + EpisodeId + " for series " + SeriesId + " for EpNum " + AiredEpNum + " for SeasonID " + SeasonId + " for ReadSeasonNum " + ReadAiredSeasonNum + " for DVDSeasonNum " + ReadDvdSeasonNum);
            }

            return returnVal;
        }

        public void SetDefaults(SeriesInfo ser, Season airSeas,Season dvdSeason)
        {
            items = new Dictionary<string, string>();
            TheAiredSeason = airSeas;
            TheDvdSeason = dvdSeason;

            TheSeries = ser;

            Overview = "";
            EpisodeRating = "";  
            EpisodeGuestStars = "";   
            EpisodeDirector = ""; 
            Writer = ""; 
            Name = "";
            EpisodeId = -1;
            SeriesId = -1;
            ReadAiredSeasonNum  = -1;
            ReadDvdSeasonNum = -1;
            AiredEpNum = -1;
            DvdEpNum = -1;
            FirstAired = null;
            SrvLastUpdated = -1;
            Dirty = false;
        }

        public void SetSeriesSeason(SeriesInfo ser, Season airedSeas, Season dvdSeason)
        {
            TheAiredSeason = airedSeas;
            TheDvdSeason = dvdSeason;
            TheSeries = ser;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Episode");

            XmlHelper.WriteElementToXml(writer,"id",EpisodeId);
            XmlHelper.WriteElementToXml(writer,"seriesid",SeriesId);
            XmlHelper.WriteElementToXml(writer,"seasonid",SeasonId);
            XmlHelper.WriteElementToXml(writer,"EpisodeNumber",AiredEpNum);
            XmlHelper.WriteElementToXml(writer,"SeasonNumber",AiredSeasonNumber);
            XmlHelper.WriteElementToXml(writer, "dvdEpisodeNumber", DvdEpNum);
            XmlHelper.WriteElementToXml(writer, "dvdSeason", DvdSeasonNumber);
            XmlHelper.WriteElementToXml(writer,"lastupdated",SrvLastUpdated);
            XmlHelper.WriteElementToXml(writer,"Overview",Overview);
            XmlHelper.WriteElementToXml(writer,"Rating",EpisodeRating);  
            XmlHelper.WriteElementToXml(writer,"GuestStars",EpisodeGuestStars);  
            XmlHelper.WriteElementToXml(writer,"EpisodeDirector",EpisodeDirector);  
            XmlHelper.WriteElementToXml(writer,"Writer",Writer);  
            XmlHelper.WriteElementToXml(writer,"EpisodeName",Name);

            if (FirstAired != null)
            {
                XmlHelper.WriteElementToXml(writer,"FirstAired",FirstAired.Value.ToString("yyyy-MM-dd"));
            }

            List<string> skip = new List<string>
                                  {
                                      "overview","Overview","seriesId","seriesid","lastupdated","lastUpdated","EpisodeName","episodeName","FirstAired","firstAired",
                                      "GuestStars","guestStars","director","directors","EpisodeDirector","Writer","Writers","id","seasonid","Overview","Rating"
                                  };

            foreach (KeyValuePair<string, string> kvp in items)
            {
                if (!skip.Contains(kvp.Key))
                {
                    XmlHelper.WriteElementToXml(writer, kvp.Key, kvp.Value);
                }
            }



            writer.WriteEndElement(); //Episode
        }
    }
}
