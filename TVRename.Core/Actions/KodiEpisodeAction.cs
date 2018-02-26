using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Alphaleonis.Win32.Filesystem;
using TVRename.Core.Extensions;
using TVRename.Core.Models;
using TVRename.Core.Models.Cache;

namespace TVRename.Core.Actions
{
    public class KodiEpisodeAction : IAction
    {
        private readonly ProcessedEpisode episode;
        private readonly FileInfo file;

        public string Type => "Metadata";

        public string Produces => this.file.FullName;

        public KodiEpisodeAction(ProcessedEpisode episode, FileInfo file)
        {
            this.episode = episode;
            this.file = file;
        }
        
        public async Task Run(CancellationToken ct)
        {
            await Task.Factory.StartNew(() =>
            {
                XmlWriter writer = XmlWriter.Create(this.file.FullName, new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "    ",
                    Encoding = Encoding.UTF8,
                    NewLineChars = "\r\n",

                    // Multipart NFO files are not actually valid XML as they have multiple episodeDetails elements
                    ConformanceLevel = ConformanceLevel.Fragment
                });

                //writer.WriteStartDocument(true);
                writer.WriteComment($" created on {DateTime.UtcNow:u} - by TV Rename ");

                if (this.episode.Type == ProcessedEpisodeType.Merged)
                {
                    foreach (Episode ep in this.episode.SourceEpisodes) WriteEpisodeDetailsFor(writer, ep);
                }
                else
                {
                    WriteEpisodeDetailsFor(writer, this.episode);
                }

                writer.Close();
            }, ct);
        }

        // https://kodi.wiki/view/NFO_files/tvshows#TV_Episode_Tags
        private void WriteEpisodeDetailsFor(XmlWriter writer, Episode ep)
        {
            writer.WriteStartElement("episodedetails");

            writer.WriteNode("title", ep.Name);
            writer.WriteNode("showtitle", this.episode.Show?.Name, true);

            if (ep.Rating != null)
            {
                writer.WriteStartElement("ratings");

                writer.WriteStartElement("rating");
                writer.WriteAttributeString("name", "tvdb");
                writer.WriteAttributeString("max", "10");
                writer.WriteAttributeString("default", "true");

                writer.WriteNode("value", ep.Rating.Score);
                writer.WriteNode("votes", ep.Rating.Votes);

                writer.WriteEndElement();

                writer.WriteEndElement();
            }


            writer.WriteNode("season", ep.SeasonNumber);
            writer.WriteNode("episode", ep.Number);
            writer.WriteNode("plot", ep.Overview);

            writer.WriteNode("mpaa", this.episode.Show?.ContentRating.ToString(), true); //TODO: Dash

            writer.WriteNode("id", ep.Id);

            writer.WriteStartElement("uniqueid");
            writer.WriteAttributeString("type", "tvdb");
            writer.WriteAttributeString("default", "true");
            writer.WriteValue(ep.Id);
            writer.WriteEndElement();

            if (!string.IsNullOrWhiteSpace(ep.ImdbId))
            {
                writer.WriteStartElement("uniqueid");
                writer.WriteAttributeString("type", "imdb");
                writer.WriteAttributeString("default", "false");
                writer.WriteValue(ep.ImdbId);
                writer.WriteEndElement();
            }

            writer.WriteNodes("credits", ep.Writers);

            writer.WriteNodes("director", ep.Directors);

            if (ep.FirstAired != null)
            {
                writer.WriteStartElement("aired");
                writer.WriteValue(ep.FirstAired.Value.ToString("yyyy-MM-dd"));
                writer.WriteEndElement();

                writer.WriteStartElement("premiered");
                writer.WriteValue(ep.FirstAired.Value.ToString("yyyy-MM-dd"));
                writer.WriteEndElement();

                writer.WriteStartElement("year");
                writer.WriteValue(ep.FirstAired.Value.ToString("yyyy"));
                writer.WriteEndElement();
            }

            // Find actors in the series or this specific episode
            IEnumerable<string> overallActors = ep.GuestStars;
            overallActors = overallActors.Union(this.episode.Show?.Actors ?? new List<string>());

            int i = 0;
            foreach (string actor in overallActors.Where(a => !string.IsNullOrEmpty(a)))
            {
                writer.WriteStartElement("actor");
                writer.WriteNode("name", actor);
                writer.WriteNode("order", i++);
                writer.WriteEndElement(); // actor
            }
        }
    }
}
