using System.Threading;
using System.Threading.Tasks;
using TVRename.Core.Models;

namespace TVRename.Core.Actions
{
    public class MissingAction : IAction
    {
        public string Type => "Missing";

        public string Produces => this.Episode.FullPath;

        public ProcessedShow Show { get; }

        public ProcessedSeason Season { get; }

        public ProcessedEpisode Episode { get; }

        public MissingAction(ProcessedShow show, ProcessedSeason season,  ProcessedEpisode episode)
        {
            this.Show = show;
            this.Season = season;
            this.Episode = episode;
        }

        public Task Run(CancellationToken ct) => Task.CompletedTask;
    }
}
