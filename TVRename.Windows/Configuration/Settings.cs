using System.Collections.Generic;
using TVRename.Core.Metadata;
using TVRename.Core.Models;
using TVRename.Core.Utility;
using Newtonsoft.Json;

namespace TVRename.Windows.Configuration
{
    /// <summary>
    /// Stores and represents application settings.
    /// See <see cref="JsonSettings{T}"/>.
    /// </summary>
    /// <seealso cref="JsonSettings{Settings}" />
    /// <inheritdoc />
    public class Settings : JsonSettings<Settings>
    {
        [JsonIgnore]
        internal bool Dirty { get; set; }

        public string DefaultLocation { get; set; } = "D:\\TV"; // TODO

        public string SeasonTemplate { get; set; } = "Season {{number | pad}}";

        public string SpecialsTemplate { get; set; } = "Specials";

        public string EpisodeTemplate { get; set; } = "{{show.name}} - S{{season.number | pad}}E{{episode.number | pad}} - {{episode.name}}";

        public Language Language { get; set; } = new Language
        {
            Id = 7,
            Abbreviation = "en"
        };

        public int DownloadThreads { get; set; } = 8;

        public int RecentDays { get; set; } = 7;

        public List<Identifier> Identifiers { get; set; } = new List<Identifier>();

        public List<Show> Shows { get; set; } = new List<Show>();
    }
}
