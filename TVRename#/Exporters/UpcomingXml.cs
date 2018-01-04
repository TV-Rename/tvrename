using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename.Exporters
{
    /// <summary>
    /// Exporter for saving upcoming episodes to a XML file.
    /// </summary>
    /// <seealso cref="UpcomingExporter" />
    /// <inheritdoc />
    internal class UpcomingXml : UpcomingExporter
    {
        /// <inheritdoc />
        public override bool Active => TVSettings.Instance.ExportWTWXML;

        /// <inheritdoc />
        protected override string Location => TVSettings.Instance.ExportWTWXMLTo;

        /// <inheritdoc />
        public UpcomingXml(TVDoc doc) : base(doc) { }

        /// <inheritdoc />
        protected override bool Generate(Stream stream, IEnumerable<ProcessedEpisode> episodes)
        {
            DirFilesCache dfc = new DirFilesCache();

            try
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    NewLineOnAttributes = true,
                    Encoding = Encoding.UTF8
                };

                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("WhenToWatch");

                    foreach (ProcessedEpisode episode in episodes)
                    {
                        writer.WriteStartElement("item");
                        XMLHelper.WriteElementToXML(writer, "id", episode.TheSeries.TVDBCode);
                        XMLHelper.WriteElementToXML(writer, "SeriesName", episode.TheSeries.Name);
                        XMLHelper.WriteElementToXML(writer, "SeasonNumber", Helpers.pad(episode.SeasonNumber));
                        XMLHelper.WriteElementToXML(writer, "EpisodeNumber", Helpers.pad(episode.EpNum));
                        XMLHelper.WriteElementToXML(writer, "EpisodeName", episode.Name);

                        writer.WriteStartElement("available");
                        DateTime? airdt = episode.GetAirDateDT(true);

                        if (airdt?.CompareTo(DateTime.Now) < 0) // has aired
                        {
                            List<FileInfo> fl = this.Doc.FindEpOnDisk(dfc, episode);

                            if (fl != null && fl.Count > 0)
                            {
                                writer.WriteValue("true");
                            }
                            else if (episode.SI.DoMissingCheck)
                            {
                                writer.WriteValue("false");
                            }
                        }

                        writer.WriteEndElement();
                        XMLHelper.WriteElementToXML(writer, "Overview", episode.Overview);

                        writer.WriteStartElement("FirstAired");
                        DateTime? dt = episode.GetAirDateDT(true);
                        if (dt != null) writer.WriteValue(dt.Value.ToString("F"));
                        writer.WriteEndElement();

                        XMLHelper.WriteElementToXML(writer, "Rating", episode.EpisodeRating);
                        XMLHelper.WriteElementToXML(writer, "filename", episode.GetFilename());

                        writer.WriteEndElement(); // item
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();

                    writer.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                if (!this.Doc.Args.Unattended && !this.Doc.Args.Hide) MessageBox.Show(ex.Message);

                Logger.Error(ex);

                return false;
            }
        }
    }
}
