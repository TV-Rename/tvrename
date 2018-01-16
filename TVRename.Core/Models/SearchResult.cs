using System;
using System.Collections.Generic;

namespace TVRename.Core.Models
{
    /// <summary>
    /// Represents a television show search result.
    /// </summary>
    public class SearchResult
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
        public int? Year => this.FirstAired?.Year;
    }
}
