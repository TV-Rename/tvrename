using System;
using System.Windows.Forms;
using System.Xml;

namespace TVRename
{
    public class Episode
    {
        private string mName;

        public string Name
        {
            get
            {
                if ((mName == null) || (string.IsNullOrEmpty(mName)))
                    return "Episode " + EpNum.ToString();
                else
                    return mName;
            }
            set
            {
                mName = value;
            }
        }
        public int EpisodeID;
        public int EpNum;
        public int SeriesID;
        public int SeasonID;
        public DateTime? FirstAired;
        public long Srv_LastUpdated;
        public string Overview;
        public System.Collections.Generic.Dictionary<string, string> Items; // other fields we don't specifically grab

        public int ReadSeasonNum; // only use after loading to attach to the correct season!

        public Season TheSeason;
        public SeriesInfo TheSeries;

        public bool Dirty;


        public bool SameAs(Episode o)
        {
            return (EpisodeID == o.EpisodeID);
        }

        public string GetItem(string which)
        {
            if (Items.ContainsKey(which))
                return Items[which];
            else
                return "";
        }

        public bool OK()
        {
            return ((SeriesID != -1) && (EpisodeID != -1) && (EpNum != -1) && ((SeasonID != -1) || (ReadSeasonNum != -1)));
        }

        public void SetDefaults(SeriesInfo ser, Season seas)
        {
            Items = new System.Collections.Generic.Dictionary<string, string>();
            TheSeason = seas;
            TheSeries = ser;

            Overview = "";
            Name = "";
            EpisodeID = -1;
            SeriesID = -1;
            EpNum = -1;
            FirstAired = null;
            Srv_LastUpdated = -1;
            Dirty = false;
        }

        public Episode(Episode O)
        {
            EpisodeID = O.EpisodeID;
            SeriesID = O.SeriesID;
            EpNum = O.EpNum;
            FirstAired = O.FirstAired;
            Srv_LastUpdated = O.Srv_LastUpdated;
            Overview = O.Overview;
            Name = O.Name;
            TheSeason = O.TheSeason;
            TheSeries = O.TheSeries;
            SeasonID = O.SeasonID;
            Dirty = O.Dirty;

            Items = new System.Collections.Generic.Dictionary<string, string>();
            foreach (System.Collections.Generic.KeyValuePair<string, string> i in O.Items)
                Items.Add(i.Key, i.Value);
        }

        public int SeasonNumber
        {
            get
            {
                if (TheSeason != null)
                    return TheSeason.SeasonNumber;
                else
                    return -1;
            }
        }

        public System.DateTime? GetAirDateDT(bool correct)
        {
            if (FirstAired == null)
                return null;

            DateTime fa = (DateTime)FirstAired;
            DateTime? airs = TheSeries.AirsTime;

            DateTime dt = new DateTime(fa.Year, fa.Month, fa.Day, (airs != null) ? airs.Value.Hour : 20, (airs != null) ? airs.Value.Minute : 0, 0, 0);

            if (!correct)
                return dt;

            // do timezone adjustment
            return TimeZone.AdjustTZTimeToLocalTime(dt, TheSeries.GetTimeZone());
        }



        public string HowLong()
        {
            DateTime? airsdt = GetAirDateDT(true);
            if (airsdt == null)
                return "";
            DateTime dt = (DateTime)airsdt;

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
                    return System.Math.Round(ts.TotalMinutes).ToString() + "min";
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

        public Episode(SeriesInfo ser, Season seas)
        {
            SetDefaults(ser, seas);
        }

        public void SetSeriesSeason(SeriesInfo ser, Season seas)
        {
            TheSeason = seas;
            TheSeries = ser;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Episode");

            writer.WriteStartElement("id");
            writer.WriteValue(EpisodeID);
            writer.WriteEndElement();
            writer.WriteStartElement("seriesid");
            writer.WriteValue(SeriesID);
            writer.WriteEndElement();
            writer.WriteStartElement("seasonid");
            writer.WriteValue(SeasonID);
            writer.WriteEndElement();
            writer.WriteStartElement("EpisodeNumber");
            writer.WriteValue(EpNum);
            writer.WriteEndElement();
            writer.WriteStartElement("SeasonNumber");
            writer.WriteValue(SeasonNumber);
            writer.WriteEndElement();
            writer.WriteStartElement("lastupdated");
            writer.WriteValue(Srv_LastUpdated);
            writer.WriteEndElement();
            writer.WriteStartElement("Overview");
            writer.WriteValue(Overview);
            writer.WriteEndElement();
            writer.WriteStartElement("EpisodeName");
            writer.WriteValue(Name);
            writer.WriteEndElement();
            if (FirstAired != null)
            {
                writer.WriteStartElement("FirstAired");
                writer.WriteValue(FirstAired.Value.ToString("yyyy-MM-dd"));
                writer.WriteEndElement();
            }

            foreach (System.Collections.Generic.KeyValuePair<string, string> kvp in Items)
            {
                writer.WriteStartElement(kvp.Key);
                writer.WriteValue(kvp.Value);
                writer.WriteEndElement();
            }


            writer.WriteEndElement();
        }

        public Episode(SeriesInfo ser, Season seas, XmlReader r)
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
                    if (r.Name == "seriesid")
                        SeriesID = r.ReadElementContentAsInt(); // thetvdb series id
                    if (r.Name == "seasonid")
                        SeasonID = r.ReadElementContentAsInt();
                    else if (r.Name == "EpisodeNumber")
                        EpNum = r.ReadElementContentAsInt();
                    else if (r.Name == "SeasonNumber")
                        ReadSeasonNum = r.ReadElementContentAsInt();
                    else if (r.Name == "lastupdated")
                        Srv_LastUpdated = r.ReadElementContentAsInt();
                    else if (r.Name == "Overview")
                        Overview = Helpers.ReadStringFixQuotesAndSpaces(r);
                    else if (r.Name == "EpisodeName")
                        Name = Helpers.ReadStringFixQuotesAndSpaces(r);
                    else if (r.Name == "FirstAired")
                    {
                        try
                        {
                            FirstAired = DateTime.ParseExact(r.ReadElementContentAsString(), "yyyy-MM-dd", new System.Globalization.CultureInfo(""));
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
                    message += "\r\nEpisode ID: " + EpisodeID.ToString();
                if (EpNum != -1)
                    message += "\r\nEpisode Number: " + EpNum.ToString();
                if (!string.IsNullOrEmpty(Name))
                    message += "\r\nName: " + Name;

                message += "\r\n" + e.Message;

                MessageBox.Show(message, "TVRename", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        } // episode constructor
    }
}