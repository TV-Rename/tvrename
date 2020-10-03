// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Linq;
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
        public override bool ApplicableFor(TVSettings.ScanType st) => st == TVSettings.ScanType.Full;

        protected override void Do()
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
                writer.WriteAttributeToXml("Version", "2.1");
                writer.WriteStartElement("MissingItems");

                foreach (ItemMissing missing in TheActionList.Missing.ToList())
                {
                    writer.WriteStartElement("MissingItem");

                    writer.WriteElement("id", missing.MissingEpisode.Show.TvdbCode);
                    writer.WriteElement("title", missing.MissingEpisode.TheCachedSeries.Name);
                    writer.WriteElement("season", Helpers.Pad(missing.MissingEpisode.AppropriateSeasonNumber));
                    writer.WriteElement("episode", Helpers.Pad(missing.MissingEpisode.AppropriateEpNum));
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


    // ReSharper disable once InconsistentNaming
    internal class MissingMovieXML : ActionListExporter
    {
        public MissingMovieXML(ItemList theActionList) : base(theActionList)
        {
        }

        public override bool Active() => TVSettings.Instance.ExportMissingMoviesXML;
        protected override string Location() => TVSettings.Instance.ExportMissingMoviesXMLTo;
        public override bool ApplicableFor(TVSettings.ScanType st) => st == TVSettings.ScanType.Full;

        protected override void Do()
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
                writer.WriteAttributeToXml("Version", "2.1");
                writer.WriteStartElement("MissingMovieItems");

                foreach (MovieItemMissing missing in TheActionList.MissingMovies.ToList())
                {
                    writer.WriteStartElement("MissingMovieItem");

                    writer.WriteElement("id", missing.MovieConfig.Code);
                    writer.WriteElement("title", missing.MovieConfig.ShowName);
                    writer.WriteElement("description", missing.MovieConfig.CachedData?.Overview);
                    writer.WriteElement("pubDate",missing.MovieConfig.CachedMovie?.Year);

                    writer.WriteEndElement(); // MissingMovieItem
                }

                writer.WriteEndElement(); // MissingMovieItems
                writer.WriteEndElement(); // tvrename
                writer.WriteEndDocument();
            }
        }
    }
}
