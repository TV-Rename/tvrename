using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using TVRename.Core.Models.Converters;

namespace TVRename.Core.Models.TVDB
{
    [PublicAPI]
    public class Update
    {
        public int Id { get; set; }

        [JsonConverter(typeof(EpochConverter))]
        public DateTime LastUpdated { get; set; }
    }
}
