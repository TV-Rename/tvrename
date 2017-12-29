using System;
using System.Xml;

namespace TVRename
{
    class MissingXML : MissingExporter
    {
        public override bool Active() =>TVSettings.Instance.ExportMissingXML;
        public override string Location() =>TVSettings.Instance.ExportMissingXMLTo;
        
        public override void Run(ItemList theActionList)
        {
            if (TVSettings.Instance.ExportMissingXML)
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    NewLineOnAttributes = true
                };
                
                using (XmlWriter writer = XmlWriter.Create(Location(), settings))
                {
                    writer.WriteStartDocument();
                    
                    writer.WriteStartElement("TVRename");
                    XMLHelper.WriteAttributeToXML(writer,"Version","2.1");
                    writer.WriteStartElement("MissingItems");

                    foreach (ITem action in theActionList)
                    {
                        if (action is ItemMissing)
                        {
                            ItemMissing missing = (ItemMissing)(action);
                            writer.WriteStartElement("MissingItem");

                            XMLHelper.WriteElementToXML(writer,"id",missing.Episode.Si.TVDBCode);
                            XMLHelper.WriteElementToXML(writer, "title",missing.Episode.TheSeries.Name);
                            XMLHelper.WriteElementToXML(writer, "season", Helpers.Pad(missing.Episode.SeasonNumber));
                            XMLHelper.WriteElementToXML(writer, "episode", Helpers.Pad(missing.Episode.EpNum));
                            XMLHelper.WriteElementToXML(writer, "episodeName",missing.Episode.Name);
                            XMLHelper.WriteElementToXML(writer, "description",missing.Episode.Overview);

                            writer.WriteStartElement("pubDate");
                            DateTime? dt = missing.Episode.GetAirDateDt(true);
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
