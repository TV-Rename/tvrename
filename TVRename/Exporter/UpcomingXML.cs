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
                        XMLHelper.WriteElementToXML(writer,"id",ei.TheSeries.TVDBCode);
                        XMLHelper.WriteElementToXML(writer,"SeriesName",ei.TheSeries.Name);
                        XMLHelper.WriteElementToXML(writer,"SeasonNumber",Helpers.pad(ei.AppropriateSeasonNumber));
                        XMLHelper.WriteElementToXML(writer, "EpisodeNumber", Helpers.pad(ei.AppropriateEpNum));
                        XMLHelper.WriteElementToXML(writer,"EpisodeName",ei.Name);
  
                        writer.WriteStartElement("available");
                        DateTime? airdt = ei.GetAirDateDT(true);

                        if (airdt.HasValue && airdt.Value.CompareTo(DateTime.Now) < 0) // has aired
                        {
                            List<FileInfo> fl = TVDoc.FindEpOnDisk(dfc, ei);
                            if ((fl != null) && (fl.Count > 0))
                                writer.WriteValue("true");
                            else if (ei.SI.DoMissingCheck)
                                writer.WriteValue("false");
                        }
                        
                        writer.WriteEndElement();
                        XMLHelper.WriteElementToXML( writer,"Overview",ei.Overview);
                        
                        writer.WriteStartElement("FirstAired");
                        DateTime? dt = ei.GetAirDateDT(true);
                        if (dt != null)
                            writer.WriteValue(dt.Value.ToString("F"));
                        writer.WriteEndElement();
                        
                        XMLHelper.WriteElementToXML( writer,"Rating",ei.EpisodeRating);
                        XMLHelper.WriteElementToXML( writer,"filename",ei.GetFilename());

                        writer.WriteEndElement(); // item
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                return true;
            } // try
            catch (Exception e)
            {
                if ((!this.Doc.Args.Unattended) && (!this.Doc.Args.Hide)) MessageBox.Show(e.Message);
                Logger.Error(e);
                return false;
            }

        }
    }
}
