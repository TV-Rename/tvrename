// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
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
        public string Language;
        private string LastFiguredTZ;
        public string Name;
        public bool BannersLoaded;

        public System.Collections.Generic.Dictionary<int, Season> Seasons;

        public System.Collections.Generic.Dictionary<int, Banner> AllBanners; // All Banners linked by bannerId.
        private System.Collections.Generic.Dictionary<int, Banner> Banners; // e.g. Dictionary of the best posters per series.
        private System.Collections.Generic.Dictionary<int, Banner> LangBanners; // e.g. Dictionary of the best posters per series in the correct language.
        private System.Collections.Generic.Dictionary<int, Banner> WideBanners; // e.g. Dictionary of the best wide banners per series.
        private System.Collections.Generic.Dictionary<int, Banner> LangWideBanners; // e.g. Dictionary of the best wide banners per series in the correct language.

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


        public void SetToDefauts()
        {
            this.ShowTimeZone = TimeZone.DefaultTimeZone(); // default, is correct for most shows
            this.LastFiguredTZ = "";

            this.Items = new System.Collections.Generic.Dictionary<string, string>();
            this.Seasons = new System.Collections.Generic.Dictionary<int, Season>();

            this.AllBanners = new System.Collections.Generic.Dictionary<int, Banner>();
            this.Banners = new System.Collections.Generic.Dictionary<int, Banner>();
            this.LangBanners = new System.Collections.Generic.Dictionary<int, Banner>();
            this.WideBanners = new System.Collections.Generic.Dictionary<int, Banner>();
            this.LangWideBanners = new System.Collections.Generic.Dictionary<int, Banner>();

            this.Dirty = false;
            this.Name = ""; 
            this.AirsTime = null;
            this.TVDBCode = -1;
            this.Language = "";
            this.BannersLoaded = false;
        }
       
        public void Merge(SeriesInfo o, String preferredLanguage)
        {
            if (o.TVDBCode != this.TVDBCode)
                return; // that's not us!
            if (o.Srv_LastUpdated != 0 && o.Srv_LastUpdated < this.Srv_LastUpdated)
                return; // older!?

            bool betterLanguage = (o.Language == preferredLanguage) && (this.Language != preferredLanguage);

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

            if ((o.Banners != null) && (o.Banners.Count != 0))
                this.Banners = o.Banners;

            if ((o.LangBanners != null) && (o.LangBanners.Count != 0))
                this.LangBanners = o.LangBanners;

            if ((o.LangWideBanners != null) && (o.LangWideBanners.Count != 0))
                this.LangWideBanners = o.LangWideBanners;

            if ((o.WideBanners != null) && (o.WideBanners.Count != 0))
                this.WideBanners = o.WideBanners;
            
            if ((o.AllBanners != null) && (o.AllBanners.Count != 0))
                this.AllBanners = o.AllBanners;

            if (betterLanguage)
                this.Language = o.Language;

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
                    else if ((r.Name == "Language") || (r.Name == "language"))
                        this.Language = r.ReadElementContentAsString();
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
                if (!string.IsNullOrEmpty(this.Language))
                    message += "\r\nLanguage: \"" + this.Language + "\"";

                message += "\r\n" + e.Message;

                MessageBox.Show(message, "TVRename", MessageBoxButtons.OK, MessageBoxIcon.Error);

                throw new TVDBException(e.Message);
            }
        }

        // LoadXml

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Series");

            XMLHelper.WriteElementToXML(writer,"id",this.TVDBCode);
            XMLHelper.WriteElementToXML(writer,"SeriesName",this.Name);
            XMLHelper.WriteElementToXML(writer,"lastupdated",this.Srv_LastUpdated);
            XMLHelper.WriteElementToXML(writer,"Language",this.Language);

            foreach (System.Collections.Generic.KeyValuePair<string, string> kvp in this.Items)
            {
                XMLHelper.WriteElementToXML(writer,kvp.Key,kvp.Value);
            }
            XMLHelper.WriteElementToXML(writer,"TimeZone",this.ShowTimeZone);

            if (this.FirstAired != null)
            {
                XMLHelper.WriteElementToXML(writer,"FirstAired",this.FirstAired.Value.ToString("yyyy-MM-dd"));
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

            if (this.LangBanners.ContainsKey(snum))
                return this.LangBanners[snum].BannerPath;

            if (this.Banners.ContainsKey(snum))
                return this.Banners[snum].BannerPath;

            //if there is a problem then return the non-season specific poster by default
            return GetItem("poster");
        }

        public string GetSeasonWideBannerPath(int snum)
        {
            //We aim to return the season and language specific poster,
            //if not then a season specific one is best
            //if not then the poster is the fallback

            System.Diagnostics.Debug.Assert(BannersLoaded);

            if (this.LangWideBanners.ContainsKey(snum))
                return this.LangWideBanners[snum].BannerPath;

            if (this.WideBanners.ContainsKey(snum))
                return this.WideBanners[snum].BannerPath;

            //if there is a problem then return the non-season specific poster by default
            return GetItem("banner");
        }



        public void AddOrUpdateBanner(Banner banner)
        {
            if (AllBanners.ContainsKey(banner.BannerId)){
                AllBanners[banner.BannerId] = banner;
            } else {
                AllBanners.Add(banner.BannerId, banner);
            }

            if (banner.isSeasonPoster()) AddOrUpdateSeasonPoster(banner);
            if (banner.isWideSeason()) AddOrUpdateWideSeason(banner);

        }

        
        public void AddOrUpdateSeasonPoster(Banner banner)
        {
            AddUpdateIntoCollections(banner, this.Banners, this.LangBanners);
        }

        public void AddOrUpdateWideSeason(Banner banner)
        {
            AddUpdateIntoCollections(banner, this.WideBanners, this.LangWideBanners);
        }

        private void AddUpdateIntoCollections(Banner banner, System.Collections.Generic.Dictionary<int, Banner> coll, System.Collections.Generic.Dictionary<int, Banner> langColl)
        {
            //update language specific cache if appropriate
            if (banner.Language == this.Language)
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