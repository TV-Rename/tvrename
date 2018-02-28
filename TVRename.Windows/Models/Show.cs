using Newtonsoft.Json;
using TVRename.Core.Models;

namespace TVRename.Windows.Models
{
    public class Show : Core.Models.Show
    {
        [JsonProperty("Settings")]
        private ShowSettings JsonSettings { get; set; } = new ShowSettings();

        [JsonIgnore]
        public ShowSettings Settings => this.JsonSettings ?? Configuration.Settings.Instance.DefaultSettings;

        /// <summary>
        /// Determines if <see cref="JsonSettings"/> should be serialized.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeJsonSettings()
        {
            return this.JsonSettings.Aliases?.Count > 0 || this.JsonSettings.Rename.HasValue || this.JsonSettings.CheckMissing.HasValue || this.JsonSettings.CheckFuture.HasValue || !string.IsNullOrEmpty(this.JsonSettings.SearchUrl);
        }

        public override string ToString()
        {
            return this.JsonSettings.CustomName ?? base.ToString();
        }
    }
}
