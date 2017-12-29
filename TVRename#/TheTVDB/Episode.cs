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
using System.Xml;

namespace TVRename
{
    public class Episode
    {
        public bool Dirty;
        public int EpNum;
        public int EpisodeID;
        public DateTime? FirstAired;
        public Dictionary<string, string> Items; // other fields we don't specifically grab
        public string Overview;
        public string EpisodeRating;
        public string EpisodeGuestStars;
        public string EpisodeDirector;
        public string Writer;

        public int ReadSeasonNum; // only use after loading to attach to the correct season!
        public int SeasonID;
        public int SeriesID;
        public long Srv_LastUpdated;

        public Season TheSeason;
        public SeriesInfo TheSeries;
        private string mName;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Episode(Episode O)
        {
            EpisodeID = O.EpisodeID;
            SeriesID = O.SeriesID;
            EpNum = O.EpNum;
            FirstAired = O.FirstAired;
            Srv_LastUpdated = O.Srv_LastUpdated;
            Overview = O.Overview;
            EpisodeRating = O.EpisodeRating;
            EpisodeGuestStars = O.EpisodeGuestStars;
            EpisodeDirector = O.EpisodeDirector;
            Writer = O.Writer;
            Name = O.Name;
            TheSeason = O.TheSeason;
            TheSeries = O.TheSeries;
            SeasonID = O.SeasonID;
            Dirty = O.Dirty;

            Items = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> i in O.Items)
                Items.Add(i.Key, i.Value);
        }

        public Episode(SeriesInfo ser, Season seas)
        {
            SetDefaults(ser, seas);
        }

        public Episode(SeriesInfo ser, Season seas, XmlReader r, CommandLineArgs args)
        {
            // <Episode>
            //  <id>...</id>
            //  blah blah
            // </Episode>

            try
            {
                SetDefaults(ser, seas);

                r.Read();
                if (r.Name != "Episode")
                    return;

                r.Read();
                while (!r.EOF)
                {
                    if ((r.Name == "Episode") && (!r.IsStartElement()))
                        break;
                    if (r.Name == "id")
                        EpisodeID = r.ReadElementContentAsInt();
                    else if (r.Name == "seriesid")
                        SeriesID = r.ReadElementContentAsInt(); // thetvdb series id
                    else if (r.Name == "seasonid")
                        SeasonID = r.ReadElementContentAsInt();
                    else if (r.Name == "EpisodeNumber")
                        EpNum = r.ReadElementContentAsInt();
                    else if (r.Name == "SeasonNumber")
                    {
                        String sn = r.ReadElementContentAsString();
                        int.TryParse(sn, out ReadSeasonNum);
                    }
                    else if (r.Name == "lastupdated")
                        Srv_LastUpdated = r.ReadElementContentAsInt();
                    else if (r.Name == "Overview")
                        Overview = XMLHelper.ReadStringFixQuotesAndSpaces(r);
                    else if (r.Name == "Rating")        
                        EpisodeRating = XMLHelper.ReadStringFixQuotesAndSpaces(r);  
                    else if (r.Name == "GuestStars")
                        EpisodeGuestStars = XMLHelper.ReadStringFixQuotesAndSpaces(r);      
                    else if (r.Name == "EpisodeDirector")
                        EpisodeDirector = XMLHelper.ReadStringFixQuotesAndSpaces(r);      
                    else if (r.Name == "Writer")
                        Writer = XMLHelper.ReadStringFixQuotesAndSpaces(r);      
                    else if (r.Name == "EpisodeName")
                        Name = XMLHelper.ReadStringFixQuotesAndSpaces(r);
                    else if (r.Name == "FirstAired")
                    {
                        try
                        {
                            String contents = r.ReadElementContentAsString();
                            if (contents == "")
                            {
                               logger.Info ("Please confirm, but we are assuming that " + Name +"(episode Id =" +EpisodeID + ") has no airdate");
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
                            Items[name] = r2.ReadElementContentAsString();
                            r.Read();
                        }
                    }
                }
            }
            catch (XmlException e)
            {
                string message = "Error processing data from TheTVDB for an episode.";
                if (SeriesID != -1)
                    message += "\r\nSeries ID: " + SeriesID;
                if (EpisodeID != -1)
                    message += "\r\nEpisode ID: " + EpisodeID;
                if (EpNum != -1)
                    message += "\r\nEpisode Number: " + EpNum;
                if (!string.IsNullOrEmpty(Name))
                    message += "\r\nName: " + Name;


                logger.Error(e, message);

                throw new TVDBException(e.Message);
            }
        }

        public Episode(int seriesId, JObject json, JObject jsonInDefaultLang)
        {
            SetDefaults(null,null);
            loadJSON(seriesId, json, jsonInDefaultLang);
        }

        public Episode(int seriesId,JObject r)
        {
            // <Episode>
            //  <id>...</id>
            //  blah blah
            // </Episode>

            SetDefaults(null, null);

            loadJSON(seriesId,r);
        }
        private void loadJSON(int seriesId, JObject bestLanguageR, JObject backupLanguageR)
        {
            //Here we have two pieces of JSON. One in local language and one in the default language (English). 
            //We will populate with the best language frst and then fillin any gaps with the backup Language
            loadJSON(seriesId,bestLanguageR);

            //backupLanguageR should be a series of name/value pairs (ie a JArray of JPropertes)
            //TVDB asserts that name and overview are the fields that are localised

            if ((string.IsNullOrWhiteSpace((string)bestLanguageR["episodeName"]) && ((string)backupLanguageR["episodeName"] != null)))
            {
                Name = (string)backupLanguageR["episodeName"];
                Items["episodeName"] = Name;
            }

            if ((string.IsNullOrWhiteSpace(Items["overview"]) && ((string)backupLanguageR["overview"] != null)))
            {
                Items["overview"] = (string)backupLanguageR["overview"];
                Overview = (string)backupLanguageR["overview"];
            }


        }


        private void loadJSON(int seriesId, JObject r)
        {
            //r should be a series of name/value pairs (ie a JArray of JPropertes)
            //save them all into the Items array for safe keeping
            foreach (JProperty episodeItems in r.Children<JProperty>())
            {
                try
                {
                    JToken currentData = (JToken)episodeItems.Value;
                    if (currentData.Type == JTokenType.Array) Items[episodeItems.Name] = JSONHelper.flatten((JToken)currentData);
                    else if (currentData.Type != JTokenType.Object) //Ignore objects here as it is always the 'language' attribute that we do not need
                    {
                        JValue currentValue = (JValue)episodeItems.Value;
                        Items[episodeItems.Name] = currentValue.ToObject<string>();

                    }

                }
                catch (ArgumentException ae)
                {
                   logger.Error("Could not parse Json for " + episodeItems.Name + " :" + ae.Message);
                    //ignore as probably a cast exception
                }
                catch (NullReferenceException ae)
                {
                   logger.Error("Could not parse Json for " + episodeItems.Name + " :" + ae.Message);
                    //ignore as probably a cast exception
                }
                catch (InvalidCastException ae)
                {
                   logger.Error("Could not parse Json for " + episodeItems.Name + " :" + ae.Message);
                    //ignore as probably a cast exception
                }
            }

            SeriesID = seriesId;

            EpisodeID = (int)r["id"];

            if ((string)r["airedSeasonID"] != null) { SeasonID = (int)r["airedSeasonID"]; }
            else
            {
               logger.Error("Issue with episode " + EpisodeID + " for series " + seriesId + " no airedSeasonID " );
               logger.Error(r.ToString());
            }

            EpNum = (int)r["airedEpisodeNumber"];
            Srv_LastUpdated = (int)r["lastUpdated"];
            Overview = (string)r["overview"];
            EpisodeRating = (string)r["siteRating"];
            Name = (string)r["episodeName"];

            String sn = (string)r["airedSeason"];
            if (sn == null) {
               logger.Error("Issue with episode " + EpisodeID + " for series " + seriesId + " airedSeason = null");
               logger.Error(r.ToString());
            }
            else { int.TryParse(sn, out ReadSeasonNum); }
            
            EpisodeGuestStars = JSONHelper.flatten((JToken)r["guestStars"], "|");
            EpisodeDirector = JSONHelper.flatten((JToken)r["directors"], "|");
            Writer = JSONHelper.flatten((JToken)r["writers"], "|");

            try
            {
                String contents = (string)r["firstAired"];
                if (String.IsNullOrEmpty(contents))
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
                logger.Debug(e, "Failed to parse firstAired");
                FirstAired = null;

            }
        }

        public string Name
        {
            get
            {
                if ((mName == null) || (string.IsNullOrEmpty(mName)))
                    return "Episode " + EpNum;
                return mName;
            }
            set { mName = value; }
        }

        public int SeasonNumber
        {
            get
            {
                if (TheSeason != null)
                    return TheSeason.SeasonNumber;
                return -1;
            }
        }

        public bool SameAs(Episode o)
        {
            return (EpisodeID == o.EpisodeID);
        }

        public string GetFilename()
        {
            return getValueAcrossVersions("filename", "Filename","");
        }

        public string[] GetGuestStars()
        {

            String guest = EpisodeGuestStars;

            if (!string.IsNullOrEmpty(guest))
            {
                return guest.Split('|');

            }
            return new String[] { };

        }


       string getValueAcrossVersions(string oldTag, string newTag, string defaultValue)
        {
            //Need to cater for new and old style tags (TVDB interface v1 vs v2)
            if (Items.ContainsKey(oldTag)) return Items[oldTag];
            if (Items.ContainsKey(newTag)) return Items[newTag];
            return defaultValue;
        }

        public bool OK()
        {
            bool returnVal = (SeriesID != -1) && (EpisodeID != -1) && (EpNum != -1) && (SeasonID != -1) && (ReadSeasonNum != -1);
            if (!returnVal)
            {
               logger.Warn("Issue with episode " + EpisodeID + " for series " + SeriesID + " for EpNum " + EpNum + " for SeasonID " + SeasonID + " for ReadSeasonNum " + ReadSeasonNum);
            }

            return returnVal;
        }

        public void SetDefaults(SeriesInfo ser, Season seas)
        {
            Items = new Dictionary<string, string>();
            TheSeason = seas;
            TheSeries = ser;

            Overview = "";
            EpisodeRating = "";  
            EpisodeGuestStars = "";   
            EpisodeDirector = ""; 
            Writer = ""; 
            Name = "";
            EpisodeID = -1;
            SeriesID = -1;
            ReadSeasonNum  = -1;
            EpNum = -1;
            FirstAired = null;
            Srv_LastUpdated = -1;
            Dirty = false;
        }

        public DateTime? GetAirDateDT(bool inLocalTime)
        {
            if (FirstAired == null)
                return null;

            DateTime fa = (DateTime) FirstAired;
            DateTime? airs = TheSeries.AirsTime;

            DateTime dt = new DateTime(fa.Year, fa.Month, fa.Day, (airs != null) ? airs.Value.Hour : 20, (airs != null) ? airs.Value.Minute : 0, 0, 0);

            if (!inLocalTime)
                return dt;

            // do timezone adjustment
            return TimeZone.AdjustTZTimeToLocalTime(dt, TheSeries.GetTimeZone());
        }

        public string HowLong()
        {
            DateTime? airsdt = GetAirDateDT(true);
            if (airsdt == null)
                return "";
            DateTime dt = (DateTime) airsdt;

            TimeSpan ts = dt.Subtract(DateTime.Now); // how long...
            if (ts.TotalHours < 0)
                return "Aired";
            else
            {
                int h = ts.Hours;
                if (ts.TotalHours >= 1)
                {
                    if (ts.Minutes >= 30)
                        h += 1;
                    return ts.Days + "d " + h + "h"; // +ts->Minutes+"m "+ts->Seconds+"s";
                }
                else
                    return Math.Round(ts.TotalMinutes) + "min";
            }
        }

        public string DayOfWeek()
        {
            DateTime? dt = GetAirDateDT(true);
            return (dt != null) ? dt.Value.ToString("ddd") : "-";
        }

        public string TimeOfDay()
        {
            DateTime? dt = GetAirDateDT(true);
            return (dt != null) ? dt.Value.ToString("t") : "-";
        }

        public void SetSeriesSeason(SeriesInfo ser, Season seas)
        {
            TheSeason = seas;
            TheSeries = ser;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Episode");

            XMLHelper.WriteElementToXML(writer,"id",EpisodeID);
            XMLHelper.WriteElementToXML(writer,"seriesid",SeriesID);
            XMLHelper.WriteElementToXML(writer,"seasonid",SeasonID);
            XMLHelper.WriteElementToXML(writer,"EpisodeNumber",EpNum);
            XMLHelper.WriteElementToXML(writer,"SeasonNumber",SeasonNumber);
            XMLHelper.WriteElementToXML(writer,"lastupdated",Srv_LastUpdated);
            XMLHelper.WriteElementToXML(writer,"Overview",Overview);
            XMLHelper.WriteElementToXML(writer,"Rating",EpisodeRating);  
            XMLHelper.WriteElementToXML(writer,"GuestStars",EpisodeGuestStars);  
            XMLHelper.WriteElementToXML(writer,"EpisodeDirector",EpisodeDirector);  
            XMLHelper.WriteElementToXML(writer,"Writer",Writer);  
            XMLHelper.WriteElementToXML(writer,"EpisodeName",Name);

            if (FirstAired != null)
            {
                XMLHelper.WriteElementToXML(writer,"FirstAired",FirstAired.Value.ToString("yyyy-MM-dd"));
            }

            List<string> skip = new List<String>
                                  {
                                      "overview","Overview","seriesId","seriesid","lastupdated","lastUpdated","EpisodeName","episodeName","FirstAired","firstAired",
                                      "GuestStars","guestStars","director","directors","EpisodeDirector","Writer","Writers","id","seasonid","Overview","Rating"
                                  };

            foreach (KeyValuePair<string, string> kvp in Items)
            {
                if (!skip.Contains(kvp.Key))
                {
                    XMLHelper.WriteElementToXML(writer, kvp.Key, kvp.Value);
                }
            }



            writer.WriteEndElement(); //Episode
        }
    }
}
