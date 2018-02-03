using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TVRename.Core.Extensions;
using TVRename.Core.Models;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename.Core.Actions
{
    // ReSharper disable once InconsistentNaming
    public class Mede8erAction : IAction
    {
        private readonly ProcessedShow show;
        private readonly FileInfo file;

        public string Type => "Metadata";

        public string Produces => this.file.FullName;

        public Mede8erAction(ProcessedShow show, FileInfo file)
        {
            this.show = show;
            this.file = file;
        }

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

                writer.WriteStartElement("details");

                writer.WriteStartElement("movie");

                writer.WriteNode("title", this.show.Name);

                writer.WriteStartElement("genres");
                foreach (string genre in this.show.Genres)
                {
                    writer.WriteNode("genre", genre);
                }
                writer.WriteEndElement();

                writer.WriteNode("premiered", this.show.FirstAired?.ToString("yyyy-MM-dd"));

                writer.WriteNode("year", this.show.Year ?? 0); // TODO

                if (this.show.Rating != null) writer.WriteNode("rating", this.show.Rating.Score);

                writer.WriteNode("status", this.show.Status.ToString());

                writer.WriteNode("mpaa", this.show.ContentRating.ToString()); // TODO: Dash

                writer.WriteStartElement("id");
                writer.WriteAttributeString("moviedb", "imdb");
                writer.WriteValue(this.show.ImdbId);
                writer.WriteEndElement();

                writer.WriteNode("tvdbid", this.show.Id);

                if (!string.IsNullOrEmpty(this.show.Runtime)) writer.WriteNode("runtime", this.show.Runtime + " min");

                writer.WriteNode("plot", this.show.Overview);

                writer.WriteStartElement("cast");
                foreach (string actor in this.show.Actors)
                {
                    writer.WriteNode("actor", actor);
                }
                writer.WriteEndElement();

                writer.WriteEndElement();

                writer.WriteEndElement();

                writer.Close();
            }, ct);
        }
    }
}
