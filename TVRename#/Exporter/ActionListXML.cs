using System;
using System.Xml;

namespace TVRename
{
    abstract class ActionListXML : ActionListExporter
    {
        protected ActionListXML(ItemList theActionList) : base(theActionList)
        {
        }

        public override void Run()
        {
            if (Active())
            {
                try
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.NewLineOnAttributes = true;
                    using (XmlWriter writer = XmlWriter.Create(Location(), settings))
                    {
                        writer.WriteStartDocument();

                        writer.WriteStartElement("TVRename");
                        XMLHelper.WriteAttributeToXML(writer, "Version", "2.1");
                        writer.WriteStartElement(name());

                        foreach (Item action in TheActionList)
                        {
                            if (isOutput(action))
                            {
                                ActionCopyMoveRename acmr = (ActionCopyMoveRename)action;
                                writer.WriteStartElement("Item");

                                XMLHelper.WriteAttributeToXML(writer, "Operation", acmr.Name);
                                XMLHelper.WriteAttributeToXML(writer, "FromFolder", acmr.From.DirectoryName);
                                XMLHelper.WriteAttributeToXML(writer, "FromName", acmr.From.Name);
                                XMLHelper.WriteAttributeToXML(writer, "ToFolder", acmr.To.DirectoryName);
                                XMLHelper.WriteAttributeToXML(writer, "ToName", acmr.To.Name);
                                XMLHelper.WriteAttributeToXML(writer, "ShowName", acmr.Episode.TheSeries.Name);
                                XMLHelper.WriteAttributeToXML(writer, "Season", acmr.Episode.AppropriateSeasonNumber);
                                XMLHelper.WriteAttributeToXML(writer, "Episode", acmr.Episode.NumsAsString());

                                writer.WriteEndElement(); //Item                                
                            }
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
        }
        protected abstract bool isOutput(Item a);
        protected abstract string name();
    }

    class RenamingXML : ActionListXML
    {
        public RenamingXML(ItemList theActionList) : base(theActionList)
        {
        }

        protected override bool isOutput(Item a)
        {
            return (a is ActionCopyMoveRename cmr) && ((cmr.Operation == ActionCopyMoveRename.Op.Rename));
        }
        public override bool ApplicableFor(TVSettings.ScanType st) => true;

        public override bool Active() => TVSettings.Instance.ExportRenamingXML;
        protected override string Location() => TVSettings.Instance.ExportRenamingXMLTo;
        protected override string name() => "Renaming";
    }

    class CopyMoveXML : ActionListXML
    {
        public CopyMoveXML(ItemList theActionList) : base(theActionList)
        {
        }

        public override bool ApplicableFor(TVSettings.ScanType st) => true;

        protected override bool isOutput(Item a)
        {
            return (a is ActionCopyMoveRename cmr) && ((cmr.Operation != ActionCopyMoveRename.Op.Rename));
        }

        public override bool Active() => TVSettings.Instance.ExportFOXML;
        protected override string Location() => TVSettings.Instance.ExportFOXMLTo;
        protected override string name() => "FindingAndOrganising";
    }
}
