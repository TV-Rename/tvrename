// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

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
        public override bool ApplicableFor(TVSettings.ScanType st) => (st == TVSettings.ScanType.Full);

        internal override void Do()
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
                XmlHelper.WriteAttributeToXml(writer,"Version","2.1");
                writer.WriteStartElement("MissingItems");

                foreach (ItemMissing missing in TheActionList.MissingItems())
                {
                    writer.WriteStartElement("MissingItem");

                    XmlHelper.WriteElementToXml(writer,"id",missing.Episode.Show.TvdbCode);
                    XmlHelper.WriteElementToXml(writer, "title",missing.Episode.TheSeries.Name);
                    XmlHelper.WriteElementToXml(writer, "season", Helpers.Pad(missing.Episode.AppropriateSeasonNumber));
                    XmlHelper.WriteElementToXml(writer, "episode", Helpers.Pad(missing.Episode.AppropriateEpNum));
                    XmlHelper.WriteElementToXml(writer, "episodeName",missing.Episode.Name);
                    XmlHelper.WriteElementToXml(writer, "description",missing.Episode.Overview);

                    writer.WriteStartElement("pubDate");
                    DateTime? dt = missing.Episode.GetAirDateDt(true);
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
    }
}
