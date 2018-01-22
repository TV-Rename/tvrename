using Newtonsoft.Json;
using TVRename.Core.Models.Converters;
using TVRename.Core.Utility;

namespace TVRename.Core.Models.Settings
{
    /// <summary>
    /// Stores and represents application statistics.
    /// See <see cref="JsonSettings{T}"/>.
    /// </summary>
    /// <seealso cref="JsonSettings{Statistics}" />
    /// <inheritdoc />
    public class Statistics : JsonSettings<Statistics>
    {
        [JsonConverter(typeof(CounterConverter))]
        public Counter AutoAddedShows { get; set; } = new Counter(0);

        [JsonConverter(typeof(CounterConverter))]
        public Counter FilesCopied { get; set; } = new Counter(0);

        [JsonConverter(typeof(CounterConverter))]
        public Counter FilesMoved { get; set; } = new Counter(0);

        [JsonConverter(typeof(CounterConverter))]
        public Counter FilesRenamed { get; set; } = new Counter(0);

        [JsonConverter(typeof(CounterConverter))]
        public Counter FindAndOrganisesDone { get; set; } = new Counter(0);

        [JsonConverter(typeof(CounterConverter))]
        public Counter MissingChecksDone { get; set; } = new Counter(0);

        [JsonConverter(typeof(CounterConverter))]
        public Counter RenameChecksDone { get; set; } = new Counter(0);

        [JsonConverter(typeof(CounterConverter))]
        public Counter TorrentsMatched { get; set; } = new Counter(0);
    }
}
