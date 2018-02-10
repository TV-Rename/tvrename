using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TVRename.Core.Extensions;
using TVRename.Core.Models;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename.Core.Actions
{
    public class KodiShowAction : IAction
    {
        private readonly ProcessedShow show;
        private readonly FileInfo file;

        public string Type => "Metadata";

        public string Produces => this.file.FullName;

        public KodiShowAction(ProcessedShow show, FileInfo file)
        {
            this.show = show;
            this.file = file;
        }

        // http://kodi.wiki/view/NFO_files/tvshows
        public async Task Run(CancellationToken ct)
        {
            await Task.Factory.StartNew(() =>
            {
                XmlWriter writer = XmlWriter.Create(this.file.FullName, new XmlWriterSettings
                {
                    Indent = true,
                    Encoding = Encoding.UTF8,
                    NewLineChars = "\r\n"
                });

                writer.WriteStartDocument(true);
                writer.WriteComment($" created on {DateTime.UtcNow:u} - by TV Rename ");

                writer.WriteStartElement("tvshow");

                writer.WriteNode("title", this.show.Name);

                if (this.show.CustomName) writer.WriteNode("showtitle", this.show.OriginalName);

                if (this.show.Rating != null)
                {
                    writer.WriteStartElement("ratings");

                    writer.WriteStartElement("rating");
                    writer.WriteAttributeString("name", "tvdb");
                    writer.WriteAttributeString("max", "10");
                    writer.WriteAttributeString("default", "true");

                    writer.WriteNode("value", this.show.Rating.Score);
                    writer.WriteNode("votes", this.show.Rating.Votes);

                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
                
                writer.WriteNode("plot", this.show.Overview);

                if (!string.IsNullOrEmpty(this.show.Runtime)) writer.WriteNode("runtime", this.show.Runtime);

                writer.WriteNode("mpaa", this.show.ContentRating.ToString()); // TODO: Dash

                writer.WriteNode("id", this.show.Id);

                writer.WriteStartElement("uniqueid");
                writer.WriteAttributeString("type", "tvdb");
                writer.WriteAttributeString("default", "true");
                writer.WriteValue(this.show.Id);
                writer.WriteEndElement();

                writer.WriteStartElement("uniqueid");
                writer.WriteAttributeString("type", "imdb");
                writer.WriteAttributeString("default", "false");
                writer.WriteValue(this.show.ImdbId);
                writer.WriteEndElement();

                writer.WriteNodes("genre", this.show.Genres);

                writer.WriteNode("premiered", this.show.FirstAired?.ToString("yyyy-MM-dd"));

                writer.WriteNode("year", this.show.Year ?? 0); // TODO

                writer.WriteNode("status", this.show.Status.ToString());

                int i = 0;
                foreach (string actor in this.show.Actors)
                {
                    writer.WriteStartElement("actor");

                    writer.WriteNode("name", actor);
                    writer.WriteNode("order", i++);

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();

                writer.Close();
            }, ct);
        }
    }
}
