using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace TVRename
{
    class UpcomingRss :UpcomingExporter
    {
        public UpcomingRss(TVDoc i) : base(i) { }
        public override bool Active() =>TVSettings.Instance.ExportWtwrss;
        public override string Location() => TVSettings.Instance.ExportWtwrssTo;

        protected override bool Generate(System.IO.Stream str, List<ProcessedEpisode> elist)
        {
            if (elist == null)
                return false;

            try
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    NewLineOnAttributes = true,
                    Encoding = Encoding.ASCII
                };
                using (XmlWriter writer = XmlWriter.Create(str, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("rss");
                    XMLHelper.WriteAttributeToXML(writer, "version", "2.0");
                    writer.WriteStartElement("channel");
                    XMLHelper.WriteElementToXML(writer, "title", "Upcoming Shows");
                    XMLHelper.WriteElementToXML(writer, "title", "http://tvrename.com");
                    XMLHelper.WriteElementToXML(writer, "description", "Upcoming shows, exported by TVRename");

                    foreach (ProcessedEpisode ei in elist)
                    {
                        string niceName = TVSettings.Instance.NamingStyle.NameForExt(ei, null, 0);

                        writer.WriteStartElement("item");
                        
                        XMLHelper.WriteElementToXML(writer,"title",ei.HowLong() + " " + ei.DayOfWeek() + " " + ei.TimeOfDay() + " " + niceName);
                        XMLHelper.WriteElementToXML(writer, "link", TheTVDB.Instance.WebsiteUrl(ei.TheSeries.TVDBCode, ei.SeasonId, false));
                        XMLHelper.WriteElementToXML(writer,"description",niceName + "<br/>" + ei.Overview);

                        writer.WriteStartElement("pubDate");
                        DateTime? dt = ei.GetAirDateDt(true);
                        if (dt != null)
                            writer.WriteValue(dt.Value.ToString("r"));
                        writer.WriteEndElement(); //pubDate
                        
                        writer.WriteEndElement(); // item
                    }
                    writer.WriteEndElement(); //channel
                    writer.WriteEndElement(); //rss
                    writer.WriteEndDocument();
                    writer.Close();
                }
                return true;
            } // try
            catch (Exception e)
            {
                Logger.Error(e);
                return false;
            }

        }
    }
}
