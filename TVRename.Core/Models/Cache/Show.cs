using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json;
using TVRename.Core.Models.Converters;

namespace TVRename.Core.Models.Cache
{
    /// <summary>
    /// Represents a television show.
    /// </summary>
    public class Show
    {
        /// <summary>
        /// Gets or sets TheTVDB show identifier.
        /// </summary>
        /// <value>
        /// The TheTVDB show identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the show name.
        /// </summary>
        /// <value>
        /// The show name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the show actors.
        /// </summary>
        /// <value>
        /// The show actors.
        /// </value>
        public List<string> Actors { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the air day.
        /// </summary>
        /// <value>
        /// The air day.
        /// </value>
        public AirDay AirDay { get; set; }

        /// <summary>
        /// Gets or sets the show air time.
        /// </summary>
        /// <value>
        /// The air time.
        /// </value>
        public TimeSpan? AirTime { get; set; }

        /// <summary>
        /// Gets or sets the show aliases.
        /// </summary>
        /// <value>
        /// The show aliases.
        /// </value>
        public List<string> Aliases { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets TheTVDB show banner path.
        /// </summary>
        /// <value>
        /// TheTVDB show banner path.
        /// </value>
        public string Banner { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Show"/> is dirty and needs updating from TheTVDB.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the show is dirty and needs updating; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool Dirty { get; set; }

        /// <summary>
        /// Gets or sets TheTVDB show fanart path.
        /// </summary>
        /// <value>
        /// TheTVDB show fanart path.
        /// </value>
        public string Fanart { get; set; }

        /// <summary>
        /// Gets or sets when the show first aired.
        /// </summary>
        /// <remarks>
        /// This only uses the date, with no time.
        /// </remarks>
        /// <value>
        /// When the show first aired.
        /// </value>
        public DateTime? FirstAired { get; set; }

        /// <summary>
        /// Gets or sets the genres.
        /// </summary>
        /// <value>
        /// The genres.
        /// </value>
        public List<string> Genres { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the IMDB identifier.
        /// </summary>
        /// <value>
        /// The IMDB identifier.
        /// </value>
        public string ImdbId { get; set; }

        /// <summary>
        /// Gets or sets the show TheTVDB language identifier.
        /// </summary>
        /// <value>
        /// Thw show TheTVDB language identifier.
        /// </value>
        public int LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the time the show was last updated from TheTVDB.
        /// </summary>
        /// <value>
        /// The time the show was last updated from TheTVDB.
        /// </value>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the show television network.
        /// </summary>
        /// <value>
        /// The show television network.
        /// </value>
        public string Network { get; set; }

        /// <summary>
        /// Gets or sets the show overview.
        /// </summary>
        /// <value>
        /// The show overview.
        /// </value>
        public string Overview { get; set; }

        /// <summary>
        /// Gets or sets TheTVDB show poster path.
        /// </summary>
        /// <value>
        /// TheTVDB show poster path.
        /// </value>
        public string Poster { get; set; }

        /// <summary>
        /// Gets or sets the show television content rating.
        /// </summary>
        /// <value>
        /// The show television content rating.
        /// </value>
        public ContentRating ContentRating { get; set; }

        /// <summary>
        /// Gets or sets the show runtime.
        /// </summary>
        /// <value>
        /// The show runtime.
        /// </value>
        public string Runtime { get; set; }

        /// <summary>
        /// Gets or sets the show TheTVDB rating.
        /// </summary>
        /// <value>
        /// The show TheTVDB rating.
        /// </value>
        public Rating Rating { get; set; }

        /// <summary>
        /// Gets or sets the show status.
        /// </summary>
        /// <value>
        /// The show status.
        /// </value>
        public Status Status { get; set; }

        /// <summary>
        /// Gets the show year.
        /// </summary>
        /// <value>
        /// The show year.
        /// </value>
        [JsonIgnore]
        public int? Year => this.FirstAired?.Year;

        /// <summary>
        /// Gets or sets the show air timezone.
        /// </summary>
        /// <value>
        /// The show air timezone.
        /// </value>
        [JsonConverter(typeof(TimeZoneInfoConverter))]
        public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"); // TODO: Move from TVDB to user settings

        /// <summary>
        /// Gets or sets the show seasons.
        /// </summary>
        /// <value>
        /// The show seasons.
        /// </value>
        public ConcurrentDictionary<int, Season> Seasons { get; set; } = new ConcurrentDictionary<int, Season>();
    }
}
