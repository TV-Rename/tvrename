using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TVRename.Core.Models
{
    /// <summary>
    /// US parental guideline content ratings.
    /// <see href="https://en.wikipedia.org/wiki/TV_parental_guidelines_(US)" />
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [PublicAPI]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContentRating
    {
        /// <summary>
        /// Default value if no rating is given.
        /// </summary>
        [EnumMember(Value = "")]
        Unknown,

        /// <summary>
        /// This program is designed to be appropriate for all children.
        /// </summary>
        [EnumMember(Value = "TV-Y")]
        TVY,

        /// <summary>
        /// This program is designed for children age 7 and above.
        /// </summary>
        [EnumMember(Value = "TV-Y7")]
        TVY7,

        /// <summary>
        /// Programs suitable for all ages.
        /// </summary>
        [EnumMember(Value = "TV-G")]
        TVG,

        /// <summary>
        /// This program contains material that parents may find unsuitable for younger children.
        /// </summary>
        [EnumMember(Value = "TV-PG")]
        TVPG,

        /// <summary>
        /// This program contains some material that many parents would find unsuitable for children under 14 years of age.
        /// </summary>
        [EnumMember(Value = "TV-14")]
        TV14,

        /// <summary>
        /// This program is specifically designed to be viewed by adults and therefore may be unsuitable for children under 17.
        /// </summary>
        [EnumMember(Value = "TV-MA")]
        TVMA
    }
}
