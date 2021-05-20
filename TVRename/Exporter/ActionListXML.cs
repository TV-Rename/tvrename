using System.Xml;

namespace TVRename
{
    internal abstract class ActionListXml : ActionListExporter
    {
        protected ActionListXml(ItemList theActionList) : base(theActionList)
        {
        }

        protected override void Do()
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
                writer.WriteAttributeToXml("Version", "2.1");
                writer.WriteStartElement(MainXmlElementName());

                foreach (Item action in TheActionList)
                {
                    if (!IsOutput(action))
                    {
                        continue;
                    }

                    ActionCopyMoveRename acmr = (ActionCopyMoveRename)action;
                    writer.WriteStartElement("Item");

                    writer.WriteAttributeToXml("Operation", acmr.Name);
                    writer.WriteAttributeToXml("FromFolder", acmr.From.DirectoryName);
                    writer.WriteAttributeToXml("FromName", acmr.From.Name);
                    writer.WriteAttributeToXml("ToFolder", acmr.To.DirectoryName);
                    writer.WriteAttributeToXml("ToName", acmr.To.Name);
                    writer.WriteAttributeToXml("ShowName", acmr.SeriesName);
                    writer.WriteAttributeToXml("Season", acmr.SourceEpisode.AppropriateSeasonNumber);
                    writer.WriteAttributeToXml("Episode", acmr.SourceEpisode.EpNumsAsString());

                    writer.WriteEndElement(); //Item
                }

                writer.WriteEndElement(); // Name
                writer.WriteEndElement(); // tvrename
                writer.WriteEndDocument();
            }
        }

        protected abstract bool IsOutput(Item a);

        protected abstract string MainXmlElementName();
    }
}
