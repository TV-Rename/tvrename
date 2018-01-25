using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TVRename.Core.Metadata
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Target
    {
        Show = 0,
        Season = 1,
        Episode = 2
    }
}
