using System;
using System.Text;
using System.Xml;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class MissingXML : ActionListExporter
    {
        public MissingXML(ItemList theActionList) : base(theActionList)
        {
        }

        public override bool Active() =>TVSettings.Instance.ExportMissingXML;
        protected override string Location() =>TVSettings.Instance.ExportMissingXMLTo;

        public override bool ApplicableFor(TVSettings.ScanType st)
        {
            return (st == TVSettings.ScanType.Full);
        }
        public override void Run()
        {
            if (!Active()) return;

            try
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    NewLineOnAttributes = true,
                    Encoding = Encoding.ASCII
                };

            
                using (XmlWriter writer = XmlWriter.Create(Location(), settings))
                {
                    writer.WriteStartDocument();
                    
                    writer.WriteStartElement("TVRename");
                    XMLHelper.WriteAttributeToXML(writer,"Version","2.1");
                    writer.WriteStartElement("MissingItems");

                    foreach (Item action in TheActionList)
                    {
                        if (!(action is ItemMissing)) continue;

                        ItemMissing missing = (ItemMissing)(action);
                        writer.WriteStartElement("MissingItem");

                        XMLHelper.WriteElementToXML(writer,"id",missing.Episode.SI.TVDBCode);
                        XMLHelper.WriteElementToXML(writer, "title",missing.Episode.TheSeries.Name);
                        XMLHelper.WriteElementToXML(writer, "season", Helpers.pad(missing.Episode.AppropriateSeasonNumber));
                        XMLHelper.WriteElementToXML(writer, "episode", Helpers.pad(missing.Episode.AppropriateEpNum));
                        XMLHelper.WriteElementToXML(writer, "episodeName",missing.Episode.Name);
                        XMLHelper.WriteElementToXML(writer, "description",missing.Episode.Overview);

                        writer.WriteStartElement("pubDate");
                        DateTime? dt = missing.Episode.GetAirDateDT(true);
                        if (dt != null)
                            writer.WriteValue(dt.Value.ToString("F"));
                        writer.WriteEndElement();
                            
                        writer.WriteEndElement(); // MissingItem
                    }
                    writer.WriteEndElement(); // MissingItems
                    writer.WriteEndElement(); // tvrename
                    writer.WriteEndDocument();
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
