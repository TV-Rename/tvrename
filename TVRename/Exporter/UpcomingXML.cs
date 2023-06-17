//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace TVRename;

// ReSharper disable once InconsistentNaming
internal class UpcomingXML : UpcomingExporter
{
    public UpcomingXML(TVDoc i) : base(i)
    {
    }

    public override bool Active() => TVSettings.Instance.ExportWTWXML;
    protected override string Name() => "Upcoming XML Exporter";
    protected override string Location() => TVSettings.Instance.ExportWTWXMLTo;

    protected override bool Generate(System.IO.Stream str, IEnumerable<ProcessedEpisode> elist)
    {
        DirFilesCache dfc = new();
        XmlWriterSettings settings = new()
        {
            Indent = true,
            NewLineOnAttributes = true,
            Encoding = System.Text.Encoding.ASCII
        };
        using (XmlWriter writer = XmlWriter.Create(str, settings))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("WhenToWatch");

            foreach (ProcessedEpisode ei in elist)
            {
                writer.WriteStartElement("item");
                writer.WriteElement("ShowTvdbId", ei.Show.TvdbCode);
                writer.WriteElement("ShowTMDBId", ei.Show.TmdbCode);
                writer.WriteElement("ShowTvMazeId", ei.Show.TVmazeCode);
                writer.WriteElement("ShowIMDB", ei.Show.ImdbCode);
                writer.WriteElement("SeriesName", ei.Show.Name);
                writer.WriteElement("SeasonNumber", ei.AppropriateSeasonNumber.Pad());
                writer.WriteElement("EpisodeNumber", ei.AppropriateEpNum.Pad());
                writer.WriteElement("EpisodeName", ei.Name);

                writer.WriteStartElement("available");
                if (ei.HasAired())
                {
                    List<FileInfo> fl = dfc.FindEpOnDisk(ei);
                    if (fl.Any())
                    {
                        writer.WriteValue("true");
                    }
                    else if (ei.Show.DoMissingCheck)
                    {
                        writer.WriteValue("false");
                    }
                    else
                    {
                        writer.WriteValue("no missing check");
                    }
                }
                else
                {
                    writer.WriteValue("future");
                }

                writer.WriteEndElement();
                writer.WriteElement("Overview", ei.Overview);

                writer.WriteStartElement("FirstAired");
                DateTime? dt = ei.GetAirDateDt(true);
                if (dt != null)
                {
                    writer.WriteValue(dt.Value.ToString("F"));
                }

                writer.WriteEndElement();

                writer.WriteElement("Rating", ei.EpisodeRating);
                writer.WriteElement("filename", ei.ThumbnailUrl());

                writer.WriteEndElement(); // item
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }
        return true;
    }
}
