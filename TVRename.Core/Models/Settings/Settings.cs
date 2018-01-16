using System.Collections.Generic;
using TVRename.Core.Utility;

namespace TVRename.Core.Models.Settings
{
    /// <summary>
    /// Stores and represents application settings.
    /// See <see cref="JsonSettings{T}"/>.
    /// </summary>
    /// <seealso cref="JsonSettings{Statistics}" />
    /// <inheritdoc />
    public partial class Settings : JsonSettings<Settings>
    {
        public Language Language { get; set; } = new Language
        {
            Id = 7,
            Abbreviation = "en"
        };

        public int DownloadThreads { get; set; } = 8;

        public List<Show> Shows { get; set; } = new List<Show>();
    }
}
