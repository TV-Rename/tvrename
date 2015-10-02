using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace TVRename
{
    class MissingXML : MissingExporter
    {
        public override bool Active()
        {
            return TVSettings.Instance.ExportMissingXML;
        }
        public override string Location()
        {
            return TVSettings.Instance.ExportMissingXMLTo;
        }
        public override void Run(ItemList TheActionList)
        {
            if (TVSettings.Instance.ExportMissingXML)
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                //XmlWriterSettings settings = gcnew XmlWriterSettings();
                settings.Indent = true;
                settings.NewLineOnAttributes = true;
                using (XmlWriter writer = XmlWriter.Create(TVSettings.Instance.ExportMissingXMLTo, settings))
                {
                    writer.WriteStartDocument();
                    
                    writer.WriteStartElement("TVRename");
                    XMLHelper.WriteAttributeToXML(writer,"Version","2.1");
                    writer.WriteStartElement("MissingItems");

                    foreach (Item Action in TheActionList)
                    {
                        if (Action is ItemMissing)
                        {
                            ItemMissing Missing = (ItemMissing)(Action);
                            writer.WriteStartElement("MissingItem");

                            XMLHelper.WriteElementToXML(writer,"id",Missing.Episode.SI.TVDBCode);
                            XMLHelper.WriteElementToXML(writer, "title",Missing.Episode.TheSeries.Name);
                            XMLHelper.WriteElementToXML(writer, "season", Helpers.pad(Missing.Episode.SeasonNumber));
                            XMLHelper.WriteElementToXML(writer, "episode", Helpers.pad(Missing.Episode.EpNum));
                            XMLHelper.WriteElementToXML(writer, "episodeName",Missing.Episode.Name);
                            XMLHelper.WriteElementToXML(writer, "description",Missing.Episode.Overview);

                            writer.WriteStartElement("pubDate");
                            DateTime? dt = Missing.Episode.GetAirDateDT(true);
                            if (dt != null)
                                writer.WriteValue(dt.Value.ToString("F"));
                            writer.WriteEndElement();
                            
                            writer.WriteEndElement(); // MissingItem

                        }
                    }
                    writer.WriteEndElement(); // MissingItems
                    writer.WriteEndElement(); // tvrename
                    writer.WriteEndDocument();
                    writer.Close();
                }
            }
        }
    }
}
