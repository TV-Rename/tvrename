using System;

namespace TVRename.Core.Models
{
    public class ProcessedShow
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Poster { get; set; }
        public string Banner { get; set; }
        public string Fanart { get; set; }

        public ProcessedShow() { }

        public ProcessedShow(Show show)
        {
            this.Name = show.Metadata.Name;
            this.Location = show.Location;
            this.LastUpdated = show.Metadata.LastUpdated;
            this.Poster = show.Metadata.Poster;
            this.Banner = show.Metadata.Banner;
            this.Fanart = show.Metadata.Fanart;
        }
    }
}
