using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace TVRename.Core.Models
{
    public class ShowSettings
    {
        [CanBeNull]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Aliases { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? CheckFuture { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? CheckMissing { get; set; }

        [CanBeNull]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CustomName { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? DvdOrder { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<int> IgnoredSeasons { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? Rename { get; set; }

        [CanBeNull]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SearchUrl { get; set; }
    }
}
