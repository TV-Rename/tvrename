using Newtonsoft.Json;

namespace TVRename.Core.Models
{
    public class Show
    {
        public int TVDBId { get; set; }

        public string Location { get; set; }

        [JsonIgnore]
        public Cache.Show Metadata => Core.TVDB.TVDB.Instance.Shows[this.TVDBId];
    }
}
