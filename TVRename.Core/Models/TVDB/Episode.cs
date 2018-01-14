using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace TVRename.Core.Models.TVDB
{
    [PublicAPI]
    public class Episode : PartialEpisode
    {
        public int? AirsAfterSeason { get; set; }

        public int? AirsBeforeEpisode { get; set; }

        public int? AirsBeforeSeason { get; set; }

        [Obsolete("Use List<string> Directors")]
        public string Director { get; set; }

        public List<string> Directors { get; set; } = new List<string>();

        public int? DvdChapter { get; set; }

        public string DvdDiscid { get; set; }

        public string Filename { get; set; }
        
        public List<string> GuestStars { get; set; } = new List<string>();
        
        public string ImdbId { get; set; }

        //public string Language { get; set; } // TODO: Needed?
        
        public string LastUpdatedBy { get; set; }
        
        public string ProductionCode { get; set; }

        public string SeriesId { get; set; }

        public string ShowUrl { get; set; }

        public decimal? SiteRating { get; set; }

        public int? SiteRatingCount { get; set; }

        public string ThumbAdded { get; set; }

        public int? ThumbAuthor { get; set; }

        public string ThumbHeight { get; set; }

        public string ThumbWidth { get; set; }

        public List<string> Writers { get; set; } = new List<string>();

        /// <summary>
        /// Performs an implicit conversion from <see cref="Episode"/> to <see cref="Cache.Episode"/>.
        /// </summary>
        /// <param name="episode">The episode.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Cache.Episode(Episode episode)
        {
            return new Cache.Episode
            {
                Directors = episode.Directors.Select(d => d.Trim()).ToList(),
                FirstAired = episode.FirstAired,
                GuestStars = episode.GuestStars.Select(gs => gs.Trim()).ToList(),
                Id = episode.Id,
                LastUpdated = episode.LastUpdated,
                Name = episode.EpisodeName,
                Number = episode.AiredEpisodeNumber ?? 0, // TODO: Safe?
                Overview = episode.Overview,
                Rating = episode.SiteRating.ToString(),
                Writers = episode.Writers.Select(w => w.Trim()).ToList()
            };
        }
    }
}
