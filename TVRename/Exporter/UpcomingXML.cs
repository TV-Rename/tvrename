// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Xml;
using System.Windows.Forms;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class UpcomingXML : UpcomingExporter
    {
        public UpcomingXML(TVDoc i) : base(i) { }

        public override bool Active()=> TVSettings.Instance.ExportWTWXML;
        protected override string Location() => TVSettings.Instance.ExportWTWXMLTo;

        protected override bool  Generate(System.IO.Stream str, List<ProcessedEpisode> elist)
        {
            DirFilesCache dfc = new DirFilesCache();
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings
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
                        XmlHelper.WriteElementToXml(writer,"id",ei.TheSeries.TvdbCode);
                        XmlHelper.WriteElementToXml(writer,"SeriesName",ei.TheSeries.Name);
                        XmlHelper.WriteElementToXml(writer,"SeasonNumber",Helpers.Pad(ei.AppropriateSeasonNumber));
                        XmlHelper.WriteElementToXml(writer, "EpisodeNumber", Helpers.Pad(ei.AppropriateEpNum));
                        XmlHelper.WriteElementToXml(writer,"EpisodeName",ei.Name);
  
                        writer.WriteStartElement("available");
                        DateTime? airdt = ei.GetAirDateDT(true);

                        if (airdt.HasValue && airdt.Value.CompareTo(DateTime.Now) < 0) // has aired
                        {
                            List<FileInfo> fl = dfc.FindEpOnDisk(ei);
                            if ((fl != null) && (fl.Count > 0))
                                writer.WriteValue("true");
                            else if (ei.Show.DoMissingCheck)
                                writer.WriteValue("false");
                        }
                        
                        writer.WriteEndElement();
                        XmlHelper.WriteElementToXml( writer,"Overview",ei.Overview);
                        
                        writer.WriteStartElement("FirstAired");
                        DateTime? dt = ei.GetAirDateDT(true);
                        if (dt != null)
                            writer.WriteValue(dt.Value.ToString("F"));
                        writer.WriteEndElement();
                        
                        XmlHelper.WriteElementToXml( writer,"Rating",ei.EpisodeRating);
                        XmlHelper.WriteElementToXml( writer,"filename",ei.Filename);

                        writer.WriteEndElement(); // item
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                return true;
            } // try
            catch (Exception e)
            {
                if ((!Doc.Args.Unattended) && (!Doc.Args.Hide)) MessageBox.Show(e.Message);
                LOGGER.Error(e);
                return false;
            }
        }
    }
}
