using Newtonsoft.Json;
using TVRename.Core.Models.Converters;
using TVRename.Core.Utility;

namespace TVRename.Windows.Configuration
{
    /// <summary>
    /// Stores and represents application statistics.
    /// See <see cref="JsonSettings{T}"/>.
    /// </summary>
    /// <seealso cref="JsonSettings{Stats}" />
    /// <inheritdoc />
    public class Stats : JsonSettings<Stats>
    {
        [JsonConverter(typeof(CounterConverter))]
        public Counter FilesMoved { get; set; } = new Counter(0);

        [JsonConverter(typeof(CounterConverter))]
        public Counter FilesRenamed { get; set; } = new Counter(0);

        [JsonConverter(typeof(CounterConverter))]
        public Counter FilesCopied { get; set; } = new Counter(0);

        [JsonConverter(typeof(CounterConverter))]
        public Counter RenameChecksDone { get; set; } = new Counter(0);

        [JsonConverter(typeof(CounterConverter))]
        public Counter MissingChecksDone { get; set; } = new Counter(0);
    }
}
