using TVRename.Core.Models.Cache;

namespace TVRename.Core.Models
{
    public class ProcessedEpisode : Episode
    {
        public string Location { get; set; }
        public string Filename { get; set; }

        public ProcessedEpisode() { }

        public ProcessedEpisode(Episode episode)
        {
            this.Id = episode.Id;
            this.Number = episode.Number;
            this.Name = episode.Name;
            this.Directors = episode.Directors;
            this.FirstAired = episode.FirstAired;
            this.GuestStars = episode.GuestStars;
            this.LastUpdated = episode.LastUpdated;
            this.Overview = episode.Overview;
            this.Rating = episode.Rating;
            this.Thumbnail = episode.Thumbnail;
            this.Writers = episode.Writers;
        }
    }
}
