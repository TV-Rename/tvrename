using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TVRename.Core.Models.Converters
{
    /// <summary>
    /// Provides a class for converting a <see cref="T:DateTime" /> to and from a JSON Unix timestamp.
    /// </summary>
    /// <seealso cref="DateTimeConverterBase" />
    /// <inheritdoc />
    public class EpochConverter : DateTimeConverterBase
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The DateTime value.
        /// </returns>
        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return null;

            return Epoch.AddSeconds((long)reader.Value);
        }

        /// <summary>
        /// Writes the JSON representation of the DateTime.
        /// </summary>
        /// <param name="writer">The <see cref="T:JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue((((DateTime)value).ToUniversalTime() - Epoch).TotalSeconds.ToString(CultureInfo.InvariantCulture));
        }
    }
}
