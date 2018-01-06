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
    public class Episode
    {
        public bool Dirty;
        public int EpNum;
        public int EpisodeID;
        public DateTime? FirstAired;
        public System.Collections.Generic.Dictionary<string, string> Items; // other fields we don't specifically grab
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
            this.EpisodeID = O.EpisodeID;
            this.SeriesID = O.SeriesID;
            this.EpNum = O.EpNum;
            this.FirstAired = O.FirstAired;
            this.Srv_LastUpdated = O.Srv_LastUpdated;
            this.Overview = O.Overview;
            this.EpisodeRating = O.EpisodeRating;
            this.EpisodeGuestStars = O.EpisodeGuestStars;
            this.EpisodeDirector = O.EpisodeDirector;
            this.Writer = O.Writer;
            this.Name = O.Name;
            this.TheSeason = O.TheSeason;
            this.TheSeries = O.TheSeries;
            this.SeasonID = O.SeasonID;
            this.Dirty = O.Dirty;

            this.Items = new System.Collections.Generic.Dictionary<string, string>();
            foreach (System.Collections.Generic.KeyValuePair<string, string> i in O.Items)
                this.Items.Add(i.Key, i.Value);
        }

        public Episode(SeriesInfo ser, Season seas)
        {
            this.SetDefaults(ser, seas);
        }

        public Episode(SeriesInfo ser, Season seas, XmlReader r, CommandLineArgs args)
        {
            // <Episode>
            //  <id>...</id>
            //  blah blah
            // </Episode>

            try
            {
                this.SetDefaults(ser, seas);

                r.Read();
                if (r.Name != "Episode")
                    return;

                r.Read();
                while (!r.EOF)
                {
                    if ((r.Name == "Episode") && (!r.IsStartElement()))
                        break;
                    if (r.Name == "id")
                        this.EpisodeID = r.ReadElementContentAsInt();
                    else if (r.Name == "seriesid")
                        this.SeriesID = r.ReadElementContentAsInt(); // thetvdb series id
                    else if (r.Name == "seasonid")
                        this.SeasonID = r.ReadElementContentAsInt();
                    else if (r.Name == "EpisodeNumber")
                        this.EpNum = r.ReadElementContentAsInt();
                    else if (r.Name == "SeasonNumber")
                    {
                        String sn = r.ReadElementContentAsString();
                        int.TryParse(sn, out this.ReadSeasonNum);
                    }
                    else if (r.Name == "lastupdated")
                        this.Srv_LastUpdated = r.ReadElementContentAsInt();
                    else if (r.Name == "Overview")
                        this.Overview = XMLHelper.ReadStringFixQuotesAndSpaces(r);
                    else if (r.Name == "Rating")        
                        this.EpisodeRating = XMLHelper.ReadStringFixQuotesAndSpaces(r);  
                    else if (r.Name == "GuestStars")
                        this.EpisodeGuestStars = XMLHelper.ReadStringFixQuotesAndSpaces(r);      
                    else if (r.Name == "EpisodeDirector")
                        this.EpisodeDirector = XMLHelper.ReadStringFixQuotesAndSpaces(r);      
                    else if (r.Name == "Writer")
                        this.Writer = XMLHelper.ReadStringFixQuotesAndSpaces(r);      
                    else if (r.Name == "EpisodeName")
                        this.Name = XMLHelper.ReadStringFixQuotesAndSpaces(r);
                    else if (r.Name == "FirstAired")
                    {
                        try
                        {
                            String contents = r.ReadElementContentAsString();
                            if (contents == "")
                            {
                               logger.Info ("Please confirm, but we are assuming that " + this.Name +"(episode Id =" +this.EpisodeID + ") has no airdate");
                                this.FirstAired = null;
                            } else { 
                                this.FirstAired = DateTime.ParseExact(contents, "yyyy-MM-dd", new System.Globalization.CultureInfo(""));
                            }
                        }
                        catch
                        {
                            this.FirstAired = null;
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
                            this.Items[name] = r2.ReadElementContentAsString();
                            r.Read();
                        }
                    }
                }
            }
            catch (XmlException e)
            {
                string message = "Error processing data from TheTVDB for an episode.";
                if (this.SeriesID != -1)
                    message += "\r\nSeries ID: " + this.SeriesID;
                if (this.EpisodeID != -1)
                    message += "\r\nEpisode ID: " + this.EpisodeID;
                if (this.EpNum != -1)
                    message += "\r\nEpisode Number: " + this.EpNum;
                if (!string.IsNullOrEmpty(this.Name))
                    message += "\r\nName: " + this.Name;


                logger.Error(e, message);

                throw new TVDBException(e.Message);
            }
        }

        public Episode(int seriesId, JObject json, JObject jsonInDefaultLang)
        {
            this.SetDefaults(null,null);
            this.loadJSON(seriesId, json, jsonInDefaultLang);
        }

        public Episode(int seriesId,JObject r)
        {
            // <Episode>
            //  <id>...</id>
            //  blah blah
            // </Episode>

            this.SetDefaults(null, null);

            this.loadJSON(seriesId,r);
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
                this.Name = (string)backupLanguageR["episodeName"];
                this.Items["episodeName"] = this.Name;
            }

            if ((string.IsNullOrWhiteSpace(this.Items["overview"]) && ((string)backupLanguageR["overview"] != null)))
            {
                this.Items["overview"] = (string)backupLanguageR["overview"];
                this.Overview = (string)backupLanguageR["overview"];
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
                    if (currentData.Type == JTokenType.Array) this.Items[episodeItems.Name] = JSONHelper.flatten((JToken)currentData);
                    else if (currentData.Type != JTokenType.Object) //Ignore objects here as it is always the 'language' attribute that we do not need
                    {
                        JValue currentValue = (JValue)episodeItems.Value;
                        this.Items[episodeItems.Name] = currentValue.ToObject<string>();

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

            this.SeriesID = seriesId;

            this.EpisodeID = (int)r["id"];

            if ((string)r["airedSeasonID"] != null) { this.SeasonID = (int)r["airedSeasonID"]; }
            else
            {
               logger.Error("Issue with episode " + EpisodeID + " for series " + seriesId + " no airedSeasonID " );
               logger.Error(r.ToString());
            }

            this.EpNum = (int)r["airedEpisodeNumber"];
            this.Srv_LastUpdated = (int)r["lastUpdated"];
            this.Overview = (string)r["overview"];
            this.EpisodeRating = (string)r["siteRating"];
            this.Name = (string)r["episodeName"];

            String sn = (string)r["airedSeason"];
            if (sn == null) {
               logger.Error("Issue with episode " + EpisodeID + " for series " + seriesId + " airedSeason = null");
               logger.Error(r.ToString());
            }
            else { int.TryParse(sn, out this.ReadSeasonNum); }
            
            this.EpisodeGuestStars = JSONHelper.flatten((JToken)r["guestStars"], "|");
            this.EpisodeDirector = JSONHelper.flatten((JToken)r["directors"], "|");
            this.Writer = JSONHelper.flatten((JToken)r["writers"], "|");

            try
            {
                String contents = (string)r["firstAired"];
                if (String.IsNullOrEmpty(contents))
                {
                    //if (this.ReadSeasonNum > 0)logger.Info("Please confirm, but we are assuming that " + this.Name + "(episode Id =" + this.EpisodeID + ") has no airdate");
                    this.FirstAired = null;
                }
                else
                {
                    this.FirstAired = DateTime.ParseExact(contents, "yyyy-MM-dd", new System.Globalization.CultureInfo(""));
                }
            }
            catch (Exception e)
            {
                logger.Debug(e, "Failed to parse firstAired");
                this.FirstAired = null;

            }
        }

        public string Name
        {
            get
            {
                if ((this.mName == null) || (string.IsNullOrEmpty(this.mName)))
                    return "Episode " + this.EpNum;
                return this.mName;
            }
            set { this.mName = value; }
        }

        public int SeasonNumber
        {
            get
            {
                if (this.TheSeason != null)
                    return this.TheSeason.SeasonNumber;
                return -1;
            }
        }

        public bool SameAs(Episode o)
        {
            return (this.EpisodeID == o.EpisodeID);
        }

        public string GetFilename()
        {
            return getValueAcrossVersions("filename", "Filename","");
        }

        public string[] GetGuestStars()
        {

            String guest = this.EpisodeGuestStars;

            if (!string.IsNullOrEmpty(guest))
            {
                return guest.Split('|');

            }
            return new String[] { };

        }


       string getValueAcrossVersions(string oldTag, string newTag, string defaultValue)
        {
            //Need to cater for new and old style tags (TVDB interface v1 vs v2)
            if (this.Items.ContainsKey(oldTag)) return this.Items[oldTag];
            if (this.Items.ContainsKey(newTag)) return this.Items[newTag];
            return defaultValue;
        }

        public bool OK()
        {
            bool returnVal = (this.SeriesID != -1) && (this.EpisodeID != -1) && (this.EpNum != -1) && (this.SeasonID != -1) && (this.ReadSeasonNum != -1);
            if (!returnVal)
            {
               logger.Warn("Issue with episode " + EpisodeID + " for series " + SeriesID + " for EpNum " + EpNum + " for SeasonID " + SeasonID + " for ReadSeasonNum " + ReadSeasonNum);
            }

            return returnVal;
        }

        public void SetDefaults(SeriesInfo ser, Season seas)
        {
            this.Items = new System.Collections.Generic.Dictionary<string, string>();
            this.TheSeason = seas;
            this.TheSeries = ser;

            this.Overview = "";
            this.EpisodeRating = "";  
            this.EpisodeGuestStars = "";   
            this.EpisodeDirector = ""; 
            this.Writer = ""; 
            this.Name = "";
            this.EpisodeID = -1;
            this.SeriesID = -1;
            this.ReadSeasonNum  = -1;
            this.EpNum = -1;
            this.FirstAired = null;
            this.Srv_LastUpdated = -1;
            this.Dirty = false;
        }

        public DateTime? GetAirDateDT(bool inLocalTime)
        {
            if (this.FirstAired == null)
                return null;

            DateTime fa = (DateTime) this.FirstAired;
            DateTime? airs = this.TheSeries.AirsTime;

            DateTime dt = new DateTime(fa.Year, fa.Month, fa.Day, (airs != null) ? airs.Value.Hour : 20, (airs != null) ? airs.Value.Minute : 0, 0, 0);

            if (!inLocalTime)
                return dt;

            // do timezone adjustment
            return TimeZone.AdjustTZTimeToLocalTime(dt, this.TheSeries.GetTimeZone());
        }

        public string HowLong()
        {
            DateTime? airsdt = this.GetAirDateDT(true);
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
                    return System.Math.Round(ts.TotalMinutes) + "min";
            }
        }

        public string DayOfWeek()
        {
            DateTime? dt = this.GetAirDateDT(true);
            return (dt != null) ? dt.Value.ToString("ddd") : "-";
        }

        public string TimeOfDay()
        {
            DateTime? dt = this.GetAirDateDT(true);
            return (dt != null) ? dt.Value.ToString("t") : "-";
        }

        public void SetSeriesSeason(SeriesInfo ser, Season seas)
        {
            this.TheSeason = seas;
            this.TheSeries = ser;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Episode");

            XMLHelper.WriteElementToXML(writer,"id",this.EpisodeID);
            XMLHelper.WriteElementToXML(writer,"seriesid",this.SeriesID);
            XMLHelper.WriteElementToXML(writer,"seasonid",this.SeasonID);
            XMLHelper.WriteElementToXML(writer,"EpisodeNumber",this.EpNum);
            XMLHelper.WriteElementToXML(writer,"SeasonNumber",this.SeasonNumber);
            XMLHelper.WriteElementToXML(writer,"lastupdated",this.Srv_LastUpdated);
            XMLHelper.WriteElementToXML(writer,"Overview",this.Overview);
            XMLHelper.WriteElementToXML(writer,"Rating",this.EpisodeRating);  
            XMLHelper.WriteElementToXML(writer,"GuestStars",this.EpisodeGuestStars);  
            XMLHelper.WriteElementToXML(writer,"EpisodeDirector",this.EpisodeDirector);  
            XMLHelper.WriteElementToXML(writer,"Writer",this.Writer);  
            XMLHelper.WriteElementToXML(writer,"EpisodeName",this.Name);

            if (this.FirstAired != null)
            {
                XMLHelper.WriteElementToXML(writer,"FirstAired",this.FirstAired.Value.ToString("yyyy-MM-dd"));
            }

            List<string> skip = new List<String>
                                  {
                                      "overview","Overview","seriesId","seriesid","lastupdated","lastUpdated","EpisodeName","episodeName","FirstAired","firstAired",
                                      "GuestStars","guestStars","director","directors","EpisodeDirector","Writer","Writers","id","seasonid","Overview","Rating"
                                  };

            foreach (System.Collections.Generic.KeyValuePair<string, string> kvp in this.Items)
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
