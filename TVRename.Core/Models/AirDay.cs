using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TVRename.Core.Models
{
    /// <summary>
    /// Describes the day of the week a show airs.
    /// </summary>
    [PublicAPI]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AirDay
    {
        /// <summary>
        /// Default value if no day is specified.
        /// </summary>
        [EnumMember(Value = "")]
        Unknown,

        /// <summary>
        /// Airs daily.
        /// </summary>
        Daily,

        /// <summary>
        /// Airs on Monday.
        /// </summary>
        Monday,

        /// <summary>
        /// Airs on Tuesday.
        /// </summary>
        Tuesday,

        /// <summary>
        /// Airs on Wednesday.
        /// </summary>
        Wednesday,

        /// <summary>
        /// Airs on Thursday.
        /// </summary>
        Thursday,

        /// <summary>
        /// Airs on Friday.
        /// </summary>
        Friday,

        /// <summary>
        /// Airs on Saturday.
        /// </summary>
        Saturday,

        /// <summary>
        /// Airs on Sunday.
        /// </summary>
        Sunday
    }
}
