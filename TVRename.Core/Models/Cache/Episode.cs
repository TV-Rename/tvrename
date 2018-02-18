using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TVRename.Core.Models.Cache
{
    /// <summary>
    /// Represents an episode of a television show.
    /// </summary>
    public class Episode
    {
        /// <summary>
        /// Gets or sets TheTVDB episode identifier.
        /// </summary>
        /// <value>
        /// The TheTVDB episode identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the episode number.
        /// </summary>
        /// <value>
        /// The episode number.
        /// </value>
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets the episode name.
        /// </summary>
        /// <value>
        /// The episode name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the episode directors.
        /// </summary>
        /// <value>
        /// The episode directors.
        /// </value>
        public List<string> Directors { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Episode"/> is dirty and needs updating from TheTVDB.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the episode is dirty and needs updating; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool Dirty { get; set; }

        /// <summary>
        /// Gets or sets when the episode first aired.
        /// </summary>
        /// <remarks>
        /// This only uses the date, with no time.
        /// </remarks>
        /// <value>
        /// When the episode first aired.
        /// </value>
        public DateTime? FirstAired { get; set; }

        /// <summary>
        /// Gets or sets the episode guest stars.
        /// </summary>
        /// <value>
        /// The episode guest stars.
        /// </value>
        public List<string> GuestStars { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the time the episode was last updated from TheTVDB.
        /// </summary>
        /// <value>
        /// The time the episode was last updated from TheTVDB.
        /// </value>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the episode overview.
        /// </summary>
        /// <value>
        /// The episode overview.
        /// </value>
        public string Overview { get; set; }

        /// <summary>
        /// Gets or sets the episode rating.
        /// </summary>
        /// <value>
        /// The episode rating.
        /// </value>
        public Rating Rating { get; set; }

        /// <summary>
        /// Gets or sets the episode thumbnail image path.
        /// </summary>
        /// <value>
        /// The episode thumbnail image path.
        /// </value>
        public string Thumbnail { get; set; }

        /// <summary>
        /// Gets or sets the episode writers.
        /// </summary>
        /// <value>
        /// The episode writers.
        /// </value>
        public List<string> Writers { get; set; } = new List<string>();

        //TODO: Populate these properies - MarkSummerville
        public bool SeasonNumber { get; internal set; }
        public string ImdbId { get; set; }
    }
}
