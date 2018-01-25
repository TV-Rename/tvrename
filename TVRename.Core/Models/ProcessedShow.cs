using System;

namespace TVRename.Core.Models
{
    public class ProcessedShow : Cache.Show
    {
        public bool CustomName { get; set; }

        public string OriginalName { get; set; }

        public string Location { get; set; }

        public ProcessedShow() { }

        public ProcessedShow(Show show)
        {
            this.Name = show.Name ?? show.Metadata.Name;
            this.CustomName = !string.IsNullOrEmpty(show.Name);
            if (this.CustomName) this.OriginalName = show.Metadata.Name;

            this.Location = show.Location;

            this.Id = show.Metadata.Id;
            this.Actors = show.Metadata.Actors;
            this.AirDay = show.Metadata.AirDay;
            this.AirTime = show.Metadata.AirTime;
            this.Aliases = show.Metadata.Aliases;
            this.Banner = show.Metadata.Banner;
            this.Fanart = show.Metadata.Fanart;
            this.FirstAired = show.Metadata.FirstAired;
            this.Genres = show.Metadata.Genres;
            this.ImdbId = show.Metadata.ImdbId;
            this.LanguageId = show.Metadata.LanguageId;
            this.LastUpdated = show.Metadata.LastUpdated;
            this.Network = show.Metadata.Network;
            this.Overview = show.Metadata.Overview;
            this.Poster = show.Metadata.Poster;
            this.ContentRating = show.Metadata.ContentRating;
            this.Runtime = show.Metadata.Runtime;
            this.Rating = show.Metadata.Rating;
            this.Status = show.Metadata.Status;
        }
    }
}
