using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace TVRename.Exporter
{
    /// <summary>
    /// Exports a list of missing items to a XML file.
    /// </summary>
    /// <seealso cref="Exporter" />
    /// <inheritdoc />
    internal class MissingXml : Exporter
    {
        /// <inheritdoc />
        public override bool Active => TVSettings.Instance.ExportMissingXML;

        /// <inheritdoc />
        protected override string Location => TVSettings.Instance.ExportMissingXMLTo;

        /// <summary>
        /// Runs the exporter, checking for missing items, saving a XML file.
        /// </summary>
        /// <param name="actions">The list of items to check.</param>
        public void Run(IEnumerable<Item> actions)
        {
            if (!TVSettings.Instance.ExportMissingXML) return;

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true,
                Encoding = Encoding.UTF8
            };

            using (XmlWriter writer = XmlWriter.Create(this.Location, settings))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("TVRename");
                XMLHelper.WriteAttributeToXML(writer, "Version", "2.1");
                writer.WriteStartElement("MissingItems");

                foreach (Item action in actions)
                {
                    if (!(action is ItemMissing)) continue;

                    ItemMissing missing = (ItemMissing)action;
                    writer.WriteStartElement("MissingItem");

                    XMLHelper.WriteElementToXML(writer, "id", missing.Episode.SI.TVDBCode);
                    XMLHelper.WriteElementToXML(writer, "title", missing.Episode.TheSeries.Name);
                    XMLHelper.WriteElementToXML(writer, "season", Helpers.pad(missing.Episode.SeasonNumber));
                    XMLHelper.WriteElementToXML(writer, "episode", Helpers.pad(missing.Episode.EpNum));
                    XMLHelper.WriteElementToXML(writer, "episodeName", missing.Episode.Name);
                    XMLHelper.WriteElementToXML(writer, "description", missing.Episode.Overview);

                    writer.WriteStartElement("pubDate");
                    DateTime? dt = missing.Episode.GetAirDateDT(true);
                    if (dt != null) writer.WriteValue(dt.Value.ToString("F"));
                    writer.WriteEndElement();
                    writer.WriteEndElement(); // MissingItem
                }

                writer.WriteEndElement(); // MissingItems
                writer.WriteEndElement(); // tvrename
                writer.WriteEndDocument();

                writer.Close();
            }
        }
    }
}
