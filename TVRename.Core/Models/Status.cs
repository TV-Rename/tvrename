using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TVRename.Core.Models
{
    /// <summary>
    /// Describes the current status of a show.
    /// </summary>
    [PublicAPI]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Status
    {
        /// <summary>
        /// Default value if no status is specified.
        /// </summary>
        [EnumMember(Value = "")]
        Unknown,

        /// <summary>
        /// No more episodes are being released.
        /// </summary>
        Ended,

        /// <summary>
        /// The show is ongoing.
        /// </summary>
        Continuing
    }
}
