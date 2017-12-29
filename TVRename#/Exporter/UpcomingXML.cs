using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    class UpcomingXML : UpcomingExporter
    {
        public UpcomingXML(TVDoc i) : base(i) { }

        public override bool Active()=> TVSettings.Instance.ExportWtwxml;
        public override string Location() => TVSettings.Instance.ExportWtwxmlTo;

        protected override bool  Generate(Stream str, List<ProcessedEpisode> elist)
        {
            DirFilesCache dfc = new DirFilesCache();
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.NewLineOnAttributes = true;
                settings.Encoding = Encoding.ASCII;
                using (XmlWriter writer = XmlWriter.Create(str, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("WhenToWatch");

                    foreach (ProcessedEpisode ei in elist)
                    {
                        string niceName = TVSettings.Instance.NamingStyle.NameForExt(ei, null, 0);

                        writer.WriteStartElement("item");
                        XMLHelper.WriteElementToXML(writer,"id",ei.TheSeries.TVDBCode);
                        XMLHelper.WriteElementToXML(writer,"SeriesName",ei.TheSeries.Name);
                        XMLHelper.WriteElementToXML(writer,"SeasonNumber",Helpers.Pad(ei.SeasonNumber));
                        XMLHelper.WriteElementToXML(writer, "EpisodeNumber", Helpers.Pad(ei.EpNum));
                        XMLHelper.WriteElementToXML(writer,"EpisodeName",ei.Name);
  
                        writer.WriteStartElement("available");
                        DateTime? airdt = ei.GetAirDateDt(true);

                        if (airdt.Value.CompareTo(DateTime.Now) < 0) // has aired
                        {
                            List<FileInfo> fl = MDoc.FindEpOnDisk(dfc, ei);
                            if ((fl != null) && (fl.Count > 0))
                                writer.WriteValue("true");
                            else if (ei.Si.DoMissingCheck)
                                writer.WriteValue("false");
                        }

                        writer.WriteEndElement();
                        XMLHelper.WriteElementToXML( writer,"Overview",ei.Overview);
                        
                        writer.WriteStartElement("FirstAired");
                        DateTime? dt = ei.GetAirDateDt(true);
                        if (dt != null)
                            writer.WriteValue(dt.Value.ToString("F"));
                        writer.WriteEndElement();
                        
                        XMLHelper.WriteElementToXML( writer,"Rating",ei.EpisodeRating);
                        XMLHelper.WriteElementToXML( writer,"filename",ei.GetFilename());

                        writer.WriteEndElement(); // item
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Close();
                }
                return true;
            } // try
            catch (Exception e)
            {
                if ((!MDoc.Args.Unattended) && (!MDoc.Args.Hide)) MessageBox.Show(e.Message);
                Logger.Error(e);
                return false;
            }

        }
    }
}
