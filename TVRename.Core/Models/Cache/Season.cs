using System;
using System.Collections.Concurrent;
using System.Linq;
using Newtonsoft.Json;

namespace TVRename.Core.Models.Cache
{
    /// <summary>
    /// Represents a season of a television show.
    /// </summary>
    public class Season
    {
        /// <summary>
        /// Gets or sets the season number.
        /// </summary>
        /// <value>
        /// The season number.
        /// </value>
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets the episodes.
        /// </summary>
        /// <value>
        /// The episodes.
        /// </value>
        public ConcurrentDictionary<int, Episode> Episodes { get; set; } = new ConcurrentDictionary<int, Episode>();

        /// <summary>
        /// Gets the season status.
        /// </summary>
        /// <value>
        /// The season status.
        /// </value>
        [JsonIgnore]
        public SeasonStatus Status
        {
            get
            {
                if (!this.HasEpisodes) return SeasonStatus.NoEpisodes;

                if (this.HasAiredEpisodes && !this.HasUnairedEpisodes) return SeasonStatus.Aired;

                if (this.HasAiredEpisodes && this.HasUnairedEpisodes) return SeasonStatus.PartiallyAired;

                if (!this.HasAiredEpisodes && this.HasUnairedEpisodes) return SeasonStatus.NoneAired;

                // Can happen if a Season has Episodes WITHOUT FirstAired
                return SeasonStatus.NoEpisodes;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this season has episodes.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this season has episodes; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool HasEpisodes => this.Episodes?.Count > 0;

        /// <summary>
        /// Gets a value indicating whether this season has unaired episodes.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this season has unaired episodes; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool HasUnairedEpisodes => this.Episodes.Any(e => e.Value.FirstAired > DateTime.Now);

        /// <summary>
        /// Gets a value indicating whether this season has aired episodes.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this season has aired episodes; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool HasAiredEpisodes => this.Episodes.Any(e => e.Value.FirstAired < DateTime.Now);

        /// <summary>
        /// Initializes a new instance of the <see cref="Season"/> class.
        /// </summary>
        public Season() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Season"/> class.
        /// </summary>
        /// <param name="number">The season number.</param>
        public Season(int number)
        {
            this.Number = number;
        }
    }
}
