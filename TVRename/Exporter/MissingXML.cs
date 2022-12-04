//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Text;
using System.Xml;

namespace TVRename;

// ReSharper disable once InconsistentNaming
internal class MissingXML : MissingActionListExporter
{
    public MissingXML(ItemList theActionList) : base(theActionList)
    {
    }

    public override bool Active() => TVSettings.Instance.ExportMissingXML;
    protected override string Name() => "Missing XML Exporter";

    protected override string Location() => TVSettings.Instance.ExportMissingXMLTo;

    protected override void Do()
    {
        XmlWriterSettings settings = new()
        {
            Indent = true,
            NewLineOnAttributes = true,
            Encoding = Encoding.ASCII
        };

        using (XmlWriter writer = XmlWriter.Create(Location(), settings))
        {
            writer.WriteStartDocument();

            writer.WriteStartElement("TVRename");
            writer.WriteAttributeToXml("Version", "2.1");
            writer.WriteStartElement("MissingItems");

            foreach (ShowSeasonMissing? missing in TheActionList.MissingSeasons)
            {
                writer.WriteStartElement("MissingItem");

                writer.WriteElement("id", missing.Show.TvdbCode);
                writer.WriteElement("title", missing.Show.Name);
                int? missingSeasonNumberAsInt = missing.SeasonNumberAsInt;
                if (missingSeasonNumberAsInt != null)
                {
                    writer.WriteElement("season", missingSeasonNumberAsInt.Value.Pad());
                }

                writer.WriteEndElement(); // MissingItem
            }
            foreach (ShowItemMissing? missing in TheActionList.MissingEpisodes)
            {
                writer.WriteStartElement("MissingItem");

                writer.WriteElement("id", missing.MissingEpisode.Show.TvdbCode);
                writer.WriteElement("title", missing.MissingEpisode.TheCachedSeries.Name);
                writer.WriteElement("season", missing.MissingEpisode.AppropriateSeasonNumber.Pad());
                writer.WriteElement("episode", missing.MissingEpisode.AppropriateEpNum.Pad());
                writer.WriteElement("episodeName", missing.MissingEpisode.Name);
                writer.WriteElement("description", missing.MissingEpisode.Overview);

                writer.WriteStartElement("pubDate");
                DateTime? dt = missing.MissingEpisode.GetAirDateDt(true);
                if (dt != null)
                {
                    writer.WriteValue(dt.Value.ToString("F"));
                }

                writer.WriteEndElement();

                writer.WriteEndElement(); // MissingItem
            }

            writer.WriteEndElement(); // MissingItems
            writer.WriteEndElement(); // tvrename
            writer.WriteEndDocument();
        }
    }
}
