using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TVRename.Core.Extensions;
using TVRename.Core.Models;
using TVRename.Core.Models.Cache;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

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

        //https://kodi.wiki/view/NFO_files/tvshows#TV_Episode_Tags 
        public async Task Run(CancellationToken ct)
        {
            await Task.Factory.StartNew(() =>
            {
                XmlWriter writer = XmlWriter.Create(this.file.FullName, new XmlWriterSettings
                {
                    Indent = true,
                    Encoding = Encoding.UTF8,
                    NewLineChars = "\r\n",

                    //Multipart NFO files are not actually valid XML as they have multiple episodeDetails elements
                    ConformanceLevel = ConformanceLevel.Fragment
                });

                writer.WriteStartDocument(true);
                writer.WriteComment($" created on {DateTime.UtcNow:u} - by TV Rename ");


                if (this.episode.Type == ProcessedEpisode.ProcessedEpisodeType.merged)
                {
                    foreach (Episode ep in this.episode.SourceEpisodes) WriteEpisodeDetailsFor(writer, ep);
                }
                else WriteEpisodeDetailsFor(writer, this.episode);

                writer.Close();
            }, ct);
        }

        private void WriteEpisodeDetailsFor(XmlWriter writer,Episode singleEpisode )
        {
            // See: https://kodi.wiki/view/NFO_files/tvshows#TV_Episode_Tags
            writer.WriteStartElement("episodedetails");

            writer.WriteNode("title", singleEpisode.Name);

            //writer.WriteNode("rating", singleEpisode.Rating.Score);
            if (singleEpisode.Rating != null)
            {
                writer.WriteStartElement("ratings");

                writer.WriteStartElement("rating");
                writer.WriteAttributeString("name", "tvdb");
                writer.WriteAttributeString("max", "10");
                writer.WriteAttributeString("default", "true");

                writer.WriteNode("value", singleEpisode.Rating.Score);
                writer.WriteNode("votes", singleEpisode.Rating.Votes);

                writer.WriteEndElement();

                writer.WriteEndElement();
            }


            writer.WriteNode("season", singleEpisode.SeasonNumber);
            writer.WriteNode("episode", singleEpisode.Number);
            writer.WriteNode("plot", singleEpisode.Overview);

            if (this.episode.Show != null)
            {
                writer.WriteNode("mpaa", this.episode.Show.ContentRating.ToString());
            }
            writer.WriteNode("id", singleEpisode.Id);

            writer.WriteStartElement("uniqueid");
            writer.WriteAttributeString("type", "tvdb");
            writer.WriteAttributeString("default", "true");
            writer.WriteValue(singleEpisode.Id);
            writer.WriteEndElement();

            /*
            writer.WriteStartElement("uniqueid");
            writer.WriteAttributeString("type", "imdb");
            writer.WriteAttributeString("default", "false");
            writer.WriteValue(singleEpisode.ImdbId);
            writer.WriteEndElement();
            */

            //Writers(s)
            writer.WriteNodes("credits", singleEpisode.Writers);

            //Director(s)
            writer.WriteNodes("director", singleEpisode.Directors);


            if (singleEpisode.FirstAired != null)
            {
                writer.WriteStartElement("aired");
                writer.WriteValue(singleEpisode.FirstAired.Value.ToString("yyyy-MM-dd"));
                writer.WriteEndElement();
            }


            //Find actors in the series or this specific episode
            IEnumerable<string> overallActors = singleEpisode.GuestStars;
            overallActors = overallActors.Union(this.episode.Show?.Actors ?? new List<string>());
            int i = 0;
            foreach (string actor in overallActors)
            {
                if (string.IsNullOrEmpty(actor))
                    continue;

                writer.WriteStartElement("actor");
                writer.WriteNode("name", actor);
                writer.WriteNode("order", i++);
                writer.WriteEndElement(); // actor
            }
        }

    }
}
