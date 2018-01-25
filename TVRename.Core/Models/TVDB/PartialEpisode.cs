using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using TVRename.Core.Models.Converters;

namespace TVRename.Core.Models.TVDB
{
    [PublicAPI]
    public class PartialEpisode
    {
        public int? AbsoluteNumber { get; set; }

        public int? AiredEpisodeNumber { get; set; }

        public int AiredSeason { get; set; }
        
        public decimal? DvdEpisodeNumber { get; set; } // TODO: decimal needed?

        public int? DvdSeason { get; set; }

        public string EpisodeName { get; set; }
        
        public DateTime? FirstAired { get; set; } // TODO: Safe?
        
        public int Id { get; set; }
        
        [JsonConverter(typeof(EpochConverter))]
        public DateTime LastUpdated { get; set; }
        
        public string Overview { get; set; }
    }
}
