using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace TVRename.Core.Models
{
    public class Show
    {
        public int TVDBId { get; set; }

        public string Location { get; set; }

        [JsonIgnore]
        public Cache.Show Metadata => Core.TVDB.TVDB.Instance.Shows.ContainsKey(this.TVDBId) ? Core.TVDB.TVDB.Instance.Shows[this.TVDBId] : null; // TODO

        [CanBeNull]
        [JsonIgnore]
        public DateTime? NextAirs
        {
            get
            {
                // TODO
                //this.Metadata.Seasons.SelectMany(s => s.Value.Episodes).Where(e => e.Value.FirstAired > DateTime.UtcNow);

                return null;
            }
        }

        public override string ToString()
        {
            return this.Metadata.Name ?? this.TVDBId.ToString();
        }
    }
}
