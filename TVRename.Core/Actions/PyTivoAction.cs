using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TVRename.Core.Models;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename.Core.Actions
{
    public class PyTivoAction : IAction
    {
        private readonly ProcessedShow show;
        private readonly ProcessedSeason season;
        private readonly ProcessedEpisode episode;
        private readonly FileInfo file;

        public string Type => "Metadata";

        public string Produces => this.file.FullName;

        public PyTivoAction(ProcessedShow show, ProcessedSeason season, ProcessedEpisode episode, FileInfo file)
        {
            this.show = show;
            this.season = season;
            this.episode = episode;
            this.file = file;
        }

        // https://pytivo.sourceforge.io/wiki/index.php/Metadata
        public async Task Run(CancellationToken ct)
        {
            await Task.Factory.StartNew(() =>
            {
                StreamWriter writer = new StreamWriter(this.file.FullName, false, Encoding.UTF8);

                writer.WriteLine($"title : {this.show.Name} - {this.episode.Name}");
                writer.WriteLine($"seriesTitle : {this.show.OriginalName ?? this.show.Name}");
                writer.WriteLine($"episodeTitle : {this.episode.Name}");
                writer.WriteLine($"episodeNumber : {this.season.Number}{this.episode.Number:0#}");
                writer.WriteLine("isEpisode : true");
                writer.WriteLine($"description : {this.episode.Overview}");
                if (this.episode.FirstAired != null) writer.WriteLine($"originalAirDate : {this.episode.FirstAired:yyyy-MM-dd}T{this.show.AirTime ?? TimeSpan.Zero:c}Z");
                writer.WriteLine($"callsign : {this.show.Network}");
                writer.WriteLine($"tvRating : {this.show.ContentRating}"); // TODO: Format
                writer.WriteLine($"starRating : {Math.Min(Math.Max(Math.Ceiling(this.show.Rating.Score / (decimal)2.5 * 2) / 2, 1), 4)}"); // 1 to 4, 0.5 increments

                foreach (string actor in this.show.Actors)
                {
                    if (actor.Contains(" "))
                    {
                        string lastName = actor.Substring(actor.LastIndexOf(' ') + 1).Trim();
                        string firstName = actor.Substring(0, actor.Length - lastName.Length).Trim();

                        writer.WriteLine($"vActor : {lastName}|{firstName}");
                    }
                    else
                    {
                        writer.WriteLine($"vActor : {actor}");
                    }
                }

                foreach (string genre in this.show.Genres)
                {
                    writer.WriteLine($"vProgramGenre : {genre}");
                }

                foreach (string genre in this.show.Genres)
                {
                    writer.WriteLine($"vSeriesGenre : {genre}");
                }

                // TODO:
                //seriesId
                //vGuestStar, vDirector, vExecProducer, vProducer, vWriter, vHost, vChoreographer  ???
                //partCount & partIndex

                writer.Close();
            }, ct);
        }
    }
}
