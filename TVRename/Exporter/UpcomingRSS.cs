//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Collections.Generic;
using System.Xml;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class UpcomingRSS : UpcomingExporter
    {
        public UpcomingRSS(TVDoc i) : base(i)
        {
        }

        public override bool Active() => TVSettings.Instance.ExportWTWRSS;

        protected override string Location() => TVSettings.Instance.ExportWTWRSSTo;
        protected override string Name() => "Upcoming RSS Exporter";

        protected override bool Generate(System.IO.Stream str, IEnumerable<ProcessedEpisode>? elist)
        {
            if (elist is null)
            {
                return false;
            }

            try
            {
                XmlWriterSettings settings = new()
                {
                    Indent = true,
                    NewLineOnAttributes = true,
                    Encoding = System.Text.Encoding.ASCII
                };
                using (XmlWriter writer = XmlWriter.Create(str, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("rss");
                    writer.WriteAttributeToXml("version", "2.0");
                    writer.WriteStartElement("channel");
                    writer.WriteElement("title", "Upcoming Shows");
                    writer.WriteElement("title", "http://tvrename.com");
                    writer.WriteElement("description", "Upcoming shows, exported by TVRename");

                    foreach (ProcessedEpisode ei in elist)
                    {
                        string niceName = TVSettings.Instance.NamingStyle.NameFor(ei);

                        writer.WriteStartElement("item");

                        writer.WriteElement("title", ei.HowLong() + " " + ei.DayOfWeek() + " " + ei.TimeOfDay() + " " + ei.Show.ShowName + " " + niceName);
                        writer.WriteElement("link", ei.ProviderWebUrl());
                        writer.WriteElement("description", ei.Show.ShowName + "<br/>" + niceName + "<br/>" + ei.Overview);

                        writer.WriteStartElement("pubDate");
                        DateTime? dt = ei.GetAirDateDt(true);
                        if (dt != null)
                        {
                            writer.WriteValue(dt.Value.ToString("r"));
                        }

                        writer.WriteEndElement(); //pubDate

                        writer.WriteEndElement(); // item
                    }
                    writer.WriteEndElement(); //channel
                    writer.WriteEndElement(); //rss
                    writer.WriteEndDocument();
                }
                return true;
            } // try
            catch (Exception e)
            {
                LOGGER.Error(e);
                return false;
            }
        }
    }
}
