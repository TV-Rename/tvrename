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
    public class Episode
    {
        public bool Dirty;
        public int EpNum;
        public int EpisodeID;
        public DateTime? FirstAired;
        public System.Collections.Generic.Dictionary<string, string> Items; // other fields we don't specifically grab
        public string Overview;

        public int ReadSeasonNum; // only use after loading to attach to the correct season!
        public int SeasonID;
        public int SeriesID;
        public long Srv_LastUpdated;

        public Season TheSeason;
        public SeriesInfo TheSeries;
        private string mName;

        public Episode(Episode O)
        {
            this.EpisodeID = O.EpisodeID;
            this.SeriesID = O.SeriesID;
            this.EpNum = O.EpNum;
            this.FirstAired = O.FirstAired;
            this.Srv_LastUpdated = O.Srv_LastUpdated;
            this.Overview = O.Overview;
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

        public Episode(SeriesInfo ser, Season seas, XmlReader r)
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
                    if (r.Name == "seriesid")
                        this.SeriesID = r.ReadElementContentAsInt(); // thetvdb series id
                    if (r.Name == "seasonid")
                        this.SeasonID = r.ReadElementContentAsInt();
                    else if (r.Name == "EpisodeNumber")
                        this.EpNum = r.ReadElementContentAsInt();
                    else if (r.Name == "SeasonNumber")
                        this.ReadSeasonNum = r.ReadElementContentAsInt();
                    else if (r.Name == "lastupdated")
                        this.Srv_LastUpdated = r.ReadElementContentAsInt();
                    else if (r.Name == "Overview")
                        this.Overview = Helpers.ReadStringFixQuotesAndSpaces(r);
                    else if (r.Name == "EpisodeName")
                        this.Name = Helpers.ReadStringFixQuotesAndSpaces(r);
                    else if (r.Name == "FirstAired")
                    {
                        try
                        {
                            this.FirstAired = DateTime.ParseExact(r.ReadElementContentAsString(), "yyyy-MM-dd", new System.Globalization.CultureInfo(""));
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

                message += "\r\n" + e.Message;

                MessageBox.Show(message, "TVRename", MessageBoxButtons.OK, MessageBoxIcon.Error);

                throw new TVDBException(e.Message);
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

        public string GetItem(string which)
        {
            if (this.Items.ContainsKey(which))
                return this.Items[which];
            return "";
        }

        public bool OK()
        {
            return ((this.SeriesID != -1) && (this.EpisodeID != -1) && (this.EpNum != -1) && ((this.SeasonID != -1) || (this.ReadSeasonNum != -1)));
        }

        public void SetDefaults(SeriesInfo ser, Season seas)
        {
            this.Items = new System.Collections.Generic.Dictionary<string, string>();
            this.TheSeason = seas;
            this.TheSeries = ser;

            this.Overview = "";
            this.Name = "";
            this.EpisodeID = -1;
            this.SeriesID = -1;
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

            writer.WriteStartElement("id");
            writer.WriteValue(this.EpisodeID);
            writer.WriteEndElement();
            writer.WriteStartElement("seriesid");
            writer.WriteValue(this.SeriesID);
            writer.WriteEndElement();
            writer.WriteStartElement("seasonid");
            writer.WriteValue(this.SeasonID);
            writer.WriteEndElement();
            writer.WriteStartElement("EpisodeNumber");
            writer.WriteValue(this.EpNum);
            writer.WriteEndElement();
            writer.WriteStartElement("SeasonNumber");
            writer.WriteValue(this.SeasonNumber);
            writer.WriteEndElement();
            writer.WriteStartElement("lastupdated");
            writer.WriteValue(this.Srv_LastUpdated);
            writer.WriteEndElement();
            writer.WriteStartElement("Overview");
            writer.WriteValue(this.Overview);
            writer.WriteEndElement();
            writer.WriteStartElement("EpisodeName");
            writer.WriteValue(this.Name);
            writer.WriteEndElement();
            if (this.FirstAired != null)
            {
                writer.WriteStartElement("FirstAired");
                writer.WriteValue(this.FirstAired.Value.ToString("yyyy-MM-dd"));
                writer.WriteEndElement();
            }

            foreach (System.Collections.Generic.KeyValuePair<string, string> kvp in this.Items)
            {
                writer.WriteStartElement(kvp.Key);
                writer.WriteValue(kvp.Value);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}