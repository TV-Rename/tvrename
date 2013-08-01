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

        public System.Collections.Generic.List<Banner> Banners; 

        public System.Collections.Generic.Dictionary<int, Season> Seasons;
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
            this.Dirty = false;
            this.Name = "";
            this.AirsTime = null;
            this.TVDBCode = -1;
            this.Language = "";
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
                        this.Name = Helpers.ReadStringFixQuotesAndSpaces(r);
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
                    else if (r.Name == "Banners")
                    {
                        LoadBannersXml(r);
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

            writer.WriteStartElement("id");
            writer.WriteValue(this.TVDBCode);
            writer.WriteEndElement();

            writer.WriteStartElement("SeriesName");
            writer.WriteValue(this.Name);
            writer.WriteEndElement();

            writer.WriteStartElement("lastupdated");
            writer.WriteValue(this.Srv_LastUpdated);
            writer.WriteEndElement();

            writer.WriteStartElement("Language");
            writer.WriteValue(this.Language);
            writer.WriteEndElement();

            foreach (System.Collections.Generic.KeyValuePair<string, string> kvp in this.Items)
            {
                writer.WriteStartElement(kvp.Key);
                writer.WriteValue(kvp.Value);
                writer.WriteEndElement();
            }
            writer.WriteStartElement("TimeZone");
            writer.WriteValue(this.ShowTimeZone);
            writer.WriteEndElement();

            if (this.FirstAired != null)
            {
                writer.WriteStartElement("FirstAired");
                writer.WriteValue(this.FirstAired.Value.ToString("yyyy-MM-dd"));
                writer.WriteEndElement();
            }

            if (this.Banners != null)
            {
                writer.WriteStartElement("Banners");
                WriteBannersXml(writer);
                writer.WriteEndElement();
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

        public void LoadBannersXml(XmlReader r)
        {
            //<Banners>
            // <Banner>
            //  <id>...</id>
            //  etc.
            // </Banner>
            // <Banner>
            //  <id>...</id>
            //  blah blah
            // </Banner>
            // ...
            //</Banners>

            try
            {
                r.Read();
                while (r.Name.ToLowerInvariant() == "banner" & r.IsStartElement())
                {
                    Banner ban = new Banner();
                    r.Read();
                    while (r.Name.ToLowerInvariant() != "banner")
                    {
                        switch (r.Name.ToLowerInvariant())
                        {
                            case "id":
                                ban.id = r.ReadElementContentAsInt();
                                break;
                            case "bannerpath":
                                ban.BannerPath = r.ReadElementContentAsString();
                                break;
                            case "bannertype":
                                ban.BannerType = r.ReadElementContentAsString();
                                break;
                            case "bannertype2":
                                ban.BannerType2 = r.ReadElementContentAsString();
                                break;
                            case "language":
                                ban.Language = r.ReadElementContentAsString();
                                break;
                            case "season":
                                ban.Season = r.ReadElementContentAsInt();
                                break;
                            case "rating":
                                string x = r.ReadElementContentAsString();
                                if (!String.IsNullOrEmpty(x))
                                    ban.Rating = Convert.ToDecimal(x);
                               break;
                            case "ratingcount":
                                ban.RatingCount = r.ReadElementContentAsInt();
                                break;
                            case "seriesname":
                                ban.SeriesName = (r.ReadElementContentAsString().ToLowerInvariant() == "true");
                                break;
                            default:
                                string name = r.Name;
                                ban.Items[name] = r.ReadElementContentAsString();
                                break;
                        }
                    } // while
                    if (this.Banners == null)
                        this.Banners = new System.Collections.Generic.List<Banner>();
                    this.Banners.Add(ban);
                    r.Read();
                } // while
            } // try
            catch (XmlException e)
            {
                string message = "Error processing banner data from TheTVDB for a show.";
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

        public void WriteBannersXml(XmlWriter writer)
        {
            foreach (var banner in this.Banners)
            {
                writer.WriteStartElement("Banner");

                writer.WriteStartElement("id");
                writer.WriteValue(banner.id);
                writer.WriteEndElement();

                writer.WriteStartElement("BannerPath");
                writer.WriteValue(banner.BannerPath);
                writer.WriteEndElement();

                writer.WriteStartElement("BannerType");
                writer.WriteValue(banner.BannerType);
                writer.WriteEndElement();

                writer.WriteStartElement("BannerType2");
                writer.WriteValue(banner.BannerType2);
                writer.WriteEndElement();

                writer.WriteStartElement("Language");
                writer.WriteValue(banner.Language);
                writer.WriteEndElement();

                writer.WriteStartElement("Season");
                writer.WriteValue(banner.Season);
                writer.WriteEndElement();

                writer.WriteStartElement("Rating");
                writer.WriteValue(banner.Rating);
                writer.WriteEndElement();

                writer.WriteStartElement("RatingCount");
                writer.WriteValue(banner.RatingCount);
                writer.WriteEndElement();

                writer.WriteStartElement("SeriesName");
                writer.WriteValue(banner.SeriesName);
                writer.WriteEndElement();

                foreach (System.Collections.Generic.KeyValuePair<string, string> kvp in banner.Items)
                {
                    writer.WriteStartElement(kvp.Key);
                    writer.WriteValue(kvp.Value);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement(); // Banner
            }
        }

        public string GetBanner(string Language, int Season, TVSettings.FolderJpgIsType JpgType)
        {
            if (this.Banners == null)
                return String.Empty;

            Banner bestban = new Banner();
            string BannerType;
            switch (JpgType)
            {
                case TVSettings.FolderJpgIsType.FanArt:
                    BannerType = "fanart";
                    break;
                case TVSettings.FolderJpgIsType.Poster:
                    BannerType = "poster";
                    break;
                case TVSettings.FolderJpgIsType.Season:
                    BannerType = "season";
                    break;
                case TVSettings.FolderJpgIsType.Banner:
                default:
                    BannerType = "series";
                    break;
            }

            // Go through once, with the language set to our preferred language
            foreach (var banner in this.Banners)
            {
                // Banner must be of the correct type and language
                if (banner.BannerType == BannerType & banner.Language == Language & ((BannerType == "season" & banner.BannerType2 == "season" & banner.Season == Season) | BannerType != "season") )
                {
                    // if this banner has a better rating than what we've found so far, use it
                    if (banner.Rating > bestban.Rating)
                        bestban = banner;
                    else if (banner.Rating == bestban.Rating & banner.RatingCount > bestban.RatingCount)
                        bestban = banner;
                }
            }

            // if a banner was not found in the language we wanted, and that language wasn't english, go through list again and find one using "en" as language
            if (Language != "en" & bestban.id == 0)
            {
                foreach (var banner in this.Banners)
                {
                    // Banner must be of the correct type and language
                    if (banner.BannerType == BannerType & banner.Language == "en" & ((BannerType == "season" & banner.BannerType2 == "season" & banner.Season == Season) | BannerType != "season") )
                    {
                        // if this banner has a better rating than what we've found so far, use it
                        if (banner.Rating > bestban.Rating)
                            bestban = banner;
                        else if (banner.Rating == bestban.Rating & banner.RatingCount > bestban.RatingCount)
                            bestban = banner;
                    }
                }
            }
            return bestban.BannerPath;
        }

        // Using data from http://www.thetvdb.com/wiki/index.php?title=API:banners.xml
        public class Banner
        {
            public long id;
            public string BannerPath;
            public string BannerType;
            public string BannerType2;
            public string Language;
            public int Season;
            public decimal Rating;
            public int RatingCount;
            public bool SeriesName;
            public System.Collections.Generic.Dictionary<string, string> Items = new System.Collections.Generic.Dictionary<string, string>();
            //public string ThumbnailPath;
            //public string VignettePath;
            //public string Colors;
        }
    }
}