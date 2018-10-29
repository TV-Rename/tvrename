using System;
using System.Xml;

namespace TVRename
{
    abstract class ActionListXml : ActionListExporter
    {
        protected ActionListXml(ItemList theActionList) : base(theActionList)
        {
        }

        public override void Run()
        {
            if (!Active()) return;

            try
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
                    XmlHelper.WriteAttributeToXml(writer, "Version", "2.1");
                    writer.WriteStartElement(MainXmlElementName());

                    foreach (Item action in TheActionList)
                    {
                        if (!IsOutput(action)) continue;

                        ActionCopyMoveRename acmr = (ActionCopyMoveRename)action;
                        writer.WriteStartElement("Item");

                        XmlHelper.WriteAttributeToXml(writer, "Operation", acmr.Name);
                        XmlHelper.WriteAttributeToXml(writer, "FromFolder", acmr.From.DirectoryName);
                        XmlHelper.WriteAttributeToXml(writer, "FromName", acmr.From.Name);
                        XmlHelper.WriteAttributeToXml(writer, "ToFolder", acmr.To.DirectoryName);
                        XmlHelper.WriteAttributeToXml(writer, "ToName", acmr.To.Name);
                        XmlHelper.WriteAttributeToXml(writer, "ShowName", acmr.Episode.TheSeries.Name);
                        XmlHelper.WriteAttributeToXml(writer, "Season", acmr.Episode.AppropriateSeasonNumber);
                        XmlHelper.WriteAttributeToXml(writer, "Episode", acmr.Episode.NumsAsString());

                        writer.WriteEndElement(); //Item                                
                    }
                    writer.WriteEndElement(); // Name
                    writer.WriteEndElement(); // tvrename
                    writer.WriteEndDocument();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
        protected abstract bool IsOutput(Item a);
        protected abstract string MainXmlElementName();
    }
}
