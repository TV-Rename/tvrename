using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;
using TVRename.Core.Models.Cache;

namespace TVRename.Core.Models
{
    public class ProcessedEpisode : Episode
    {
        //TODO: Implement these methods/populate the values -MarkSummerville
        internal IEnumerable<Episode> SourceEpisodes;
        public ProcessedShow Show { get; private set; }
        public ProcessedEpisodeType Type { get; private set; }
        public enum ProcessedEpisodeType { single, split, merged };


        public string Location { get; set; }

        public string Filename { get; set; }

        public string Extension { get; set; }

        public bool Exists { get; set; }

        public string FullPath => Path.Combine(this.Location, string.IsNullOrEmpty(this.Extension) ? $"{this.Filename}" : $"{this.Filename}.{this.Extension}");

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
