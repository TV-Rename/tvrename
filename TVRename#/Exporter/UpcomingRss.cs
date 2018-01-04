using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace TVRename.Exporter
{
    /// <summary>
    /// Exporter for saving upcoming episodes to a RSS file.
    /// </summary>
    /// <seealso cref="UpcomingExporter" />
    /// <inheritdoc />
    internal class UpcomingRss : UpcomingExporter
    {
        /// <inheritdoc />
        public override bool Active => TVSettings.Instance.ExportWTWRSS;

        /// <inheritdoc />
        protected override string Location => TVSettings.Instance.ExportWTWRSSTo;

        /// <inheritdoc />
        public UpcomingRss(TVDoc doc) : base(doc) { }
        
        /// <inheritdoc />
        protected override bool Generate(Stream stream, IEnumerable<ProcessedEpisode> episodes)
        {
            if (episodes == null) return false;

            try
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    NewLineOnAttributes = false,
                    Encoding = Encoding.UTF8
                };

                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    writer.WriteStartDocument();

                    writer.WriteStartElement("rss");
                    XMLHelper.WriteAttributeToXML(writer, "version", "2.0");
                    writer.WriteAttributeString("xmlns", "atom", string.Empty, "http://www.w3.org/2005/Atom");

                    writer.WriteStartElement("channel");
                    XMLHelper.WriteElementToXML(writer, "title", "Upcoming Shows");
                    XMLHelper.WriteElementToXML(writer, "link", "http://www.tvrename.com/");
                    XMLHelper.WriteElementToXML(writer, "description", "Upcoming TV shows exported by TV Rename");

                    foreach (ProcessedEpisode episode in episodes)
                    {
                        string name = TVSettings.Instance.NamingStyle.NameForExt(episode, null, 0);

                        writer.WriteStartElement("item");

                        XMLHelper.WriteElementToXML(writer, "title", $"{episode.HowLong()} {episode.DayOfWeek()} {episode.TimeOfDay()} {name}");
                        XMLHelper.WriteElementToXML(writer, "link", TheTVDB.Instance.WebsiteURL(episode.TheSeries.TVDBCode, episode.SeasonID, false));
                        XMLHelper.WriteElementToXML(writer, "description", $"{name}<br>{episode.Overview}");

                        writer.WriteStartElement("pubDate");
                        DateTime? dt = episode.GetAirDateDT(true);
                        if (dt != null) writer.WriteValue(dt.Value.ToString("r"));
                        writer.WriteEndElement(); // pubDate

                        writer.WriteEndElement(); // item
                    }

                    writer.WriteEndElement(); // channel
                    writer.WriteEndElement(); // rss
                    writer.WriteEndDocument();

                    writer.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                return false;
            }
        }
    }
}
