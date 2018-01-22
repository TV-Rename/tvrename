using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace TVRename.Core.Models
{
    public class Show
    {
        public int TVDBId { get; set; }

        public string Location { get; set; }

        public bool CheckMissing { get; set; } = true;

        public List<int> IgnoredSeasons { get; set; } = new List<int>();

        [JsonIgnore]
        public Cache.Show Metadata => Core.TVDB.TVDB.Instance.Shows.ContainsKey(this.TVDBId) ? Core.TVDB.TVDB.Instance.Shows[this.TVDBId] : null;

        [CanBeNull]
        [JsonIgnore]
        public DateTime? NextAirs {
            get
            {
                // TODO
                //this.Metadata.Seasons.SelectMany(s => s.Value.Episodes).Where(e => e.Value.FirstAired > DateTime.UtcNow);

                return null;
            }
        }
    }
}
