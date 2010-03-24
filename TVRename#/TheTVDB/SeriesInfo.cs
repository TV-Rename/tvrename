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
        private string LastFiguredTZ;
        private TimeZone SeriesTZ;

        private void FigureOutTimeZone()
        {
            string tzstr = ShowTimeZone;

            if (string.IsNullOrEmpty(tzstr))
                tzstr = TimeZone.DefaultTimeZone();

            SeriesTZ = TimeZone.TimeZoneFor(tzstr);

            LastFiguredTZ = tzstr;
        }

        public TimeZone GetTimeZone()
        {
            // we cache the timezone info, as the fetching is a bit slow, and we do this a lot
            if (LastFiguredTZ != ShowTimeZone) 
                FigureOutTimeZone();

            return SeriesTZ;
        }


        public int TVDBCode;
        public long Srv_LastUpdated;
        public string Name;

        public System.Collections.Generic.Dictionary<string, string> Items; // e.g. Overview, Banner, Poster, etc.
        public DateTime? AirsTime;
        public System.Collections.Generic.Dictionary<int, Season> Seasons;
        public string Language;
        public DateTime? FirstAired;

        public bool Dirty; // set to true if local info is known to be older than whats on the server

        public string ShowTimeZone; // set for us by ShowItem

        // note: "SeriesID" in a <Series> is the tv.com code,
        // "seriesid" in an <Episode> is the tvdb code!

        public string GetItem(string which)
        {
            if (Items.ContainsKey(which))
                return Items[which];
            else
                return "";
        }

        public SeriesInfo(string name, int id)
        {
            SetToDefauts();
            Name = name;
            TVDBCode = id;
        }

        public SeriesInfo(XmlReader r)
        {
            SetToDefauts();
            LoadXml(r);
        }

        public void SetToDefauts()
        {
            ShowTimeZone = TimeZone.DefaultTimeZone(); // default, is correct for most shows
            LastFiguredTZ = "";

            Items = new System.Collections.Generic.Dictionary<string, string>();
            Seasons = new System.Collections.Generic.Dictionary<int, Season>();
            Dirty = false;
            Name = "";
            AirsTime = null;
            TVDBCode = -1;
            Language = "";
        }

        public int LanguagePriority(StringList languages)
        {
            if (string.IsNullOrEmpty(Language))
                return 999999;
            int r = languages.IndexOf(Language); // -1 for not found
            return (r == -1) ? 999999 : r;
        }

        public void Merge(SeriesInfo o, StringList languages)
        {
            if (o.TVDBCode != TVDBCode)
                return; // that's not us!
            if (o.Srv_LastUpdated < Srv_LastUpdated)
                return; // older!?

            bool betterLanguage = o.LanguagePriority(languages) < LanguagePriority(languages); // lower is better

            Srv_LastUpdated = o.Srv_LastUpdated;

            // take the best bits of "o"
            // "o" is always newer/better than us, if there is a choice
            if ((!string.IsNullOrEmpty(o.Name)) && betterLanguage)
                Name = o.Name;
            Items.Clear();
            foreach (System.Collections.Generic.KeyValuePair<string, string> kvp in o.Items)
            {
                if ((!string.IsNullOrEmpty(kvp.Value)) || betterLanguage)
                    Items[kvp.Key] = kvp.Value;
            }
            if (o.AirsTime != null)
                AirsTime = o.AirsTime;
            if ((o.Seasons != null) && (o.Seasons.Count != 0))
                Seasons = o.Seasons;

            if (betterLanguage)
                Language = o.Language;


            Dirty = o.Dirty;
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
                        TVDBCode = r.ReadElementContentAsInt();
                    else if (r.Name == "SeriesName")
                        Name = Helpers.ReadStringFixQuotesAndSpaces(r);
                    else if (r.Name == "lastupdated")
                        Srv_LastUpdated = r.ReadElementContentAsLong();
                    else if ((r.Name == "Language") || (r.Name == "language"))
                        Language = r.ReadElementContentAsString();
                    else if (r.Name == "TimeZone")
                        ShowTimeZone = r.ReadElementContentAsString();
                    else if (r.Name == "Airs_Time")
                    {
                        AirsTime = DateTime.Parse("20:00");

                        try
                        {
                            string theTime = r.ReadElementContentAsString();
                            if (!string.IsNullOrEmpty(theTime))
                            {
                                Items["Airs_Time"] = theTime;
                                AirsTime = DateTime.Parse(theTime);
                            }
                        }
                        catch (FormatException)
                        {
                        }
                    }
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
                        string name = r.Name;
                        Items[name] = r.ReadElementContentAsString();
                    }
                    //   r->ReadOuterXml(); // skip
                } // while
            } // try
            catch (XmlException e)
            {
                string message = "Error processing data from TheTVDB for a show.";
                if (TVDBCode != -1)
                    message += "\r\nTheTVDB Code: " + TVDBCode.ToString();
                if (!string.IsNullOrEmpty(Name))
                    message += "\r\nName: " + Name;
                if (!string.IsNullOrEmpty(Language))
                    message += "\r\nLanguage: \"" + Language + "\"";

                message += "\r\n" + e.Message;

                MessageBox.Show(message, "TVRename", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        } // LoadXml

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Series");

            writer.WriteStartElement("id");
            writer.WriteValue(TVDBCode);
            writer.WriteEndElement();

            writer.WriteStartElement("SeriesName");
            writer.WriteValue(Name);
            writer.WriteEndElement();

            writer.WriteStartElement("lastupdated");
            writer.WriteValue((long)Srv_LastUpdated);
            writer.WriteEndElement();

            writer.WriteStartElement("Language");
            writer.WriteValue(Language);
            writer.WriteEndElement();

            foreach (System.Collections.Generic.KeyValuePair<string, string> kvp in Items)
            {
                writer.WriteStartElement(kvp.Key);
                writer.WriteValue(kvp.Value);
                writer.WriteEndElement();
            }
            writer.WriteStartElement("TimeZone");
            writer.WriteValue(ShowTimeZone);
            writer.WriteEndElement();

            if (FirstAired != null)
            {
                writer.WriteStartElement("FirstAired");
                writer.WriteValue(FirstAired.Value.ToString("yyyy-MM-dd"));
                writer.WriteEndElement();
            }

            writer.WriteEndElement(); // series
        }
        public Season GetOrAddSeason(int num, int seasonID)
        {
            if (Seasons.ContainsKey(num))
                return Seasons[num];

            Season s = new Season(this, num, seasonID);
            Seasons[num] = s;

            return s;
        }
    }
}