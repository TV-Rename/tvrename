using TVRename.Core.Models.Cache;

namespace TVRename.Core.Models
{
    public class ProcessedSeason : Season
    {
        public string Location { get; set; }

        public string Directory { get; set; }
        
        public ProcessedSeason() { }

        public ProcessedSeason(Season season)
        {
            this.Number = season.Number;
        }
    }
}
