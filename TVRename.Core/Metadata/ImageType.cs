using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TVRename.Core.Metadata
{
    /// <summary>
    /// Type of metadata image.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ImageType
    {
        [Description("Show Poster")]
        ShowPoster,

        [Description("Show Banner")]
        ShowBanner,

        [Description("Show Fanart")]
        ShowFanart,

        [Description("Season Poster")]
        SeasonPoster,

        [Description("Season Banner")]
        SeasonBanner,

        [Description("Episode Thumbnail")]
        EpisodeThumbnail
    }
}
