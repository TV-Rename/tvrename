using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TVRename.Core.Models.Converters
{
    /// <summary>
    /// Provides a class for converting a <see cref="T:TimeZoneInfo" /> to and from JSON.
    /// </summary>
    /// <seealso cref="JsonConverter" />
    /// <inheritdoc />
    public class TimeZoneInfoConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeZoneInfo);
        }

        /// <summary>
        /// Reads the JSON representation of the TimeZoneInfo.
        /// </summary>
        /// <param name="reader">The <see cref="T:JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The TimeZoneInfo value.
        /// </returns>
        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String) return null;

            string value = (string)JToken.Load(reader);

            return TimeZoneInfo.FindSystemTimeZoneById(value); ;
        }

        /// <summary>
        /// Writes the JSON representation of the TimeZoneInfo.
        /// </summary>
        /// <param name="writer">The <see cref="T:JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            TimeZoneInfo tzi = (TimeZoneInfo)value;

            writer.WriteValue(tzi.Id);
        }
    }
}
