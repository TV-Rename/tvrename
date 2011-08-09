using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TVRename.db_access.documents
{
    public class ShowItemDocument
    {
        public string Id { get; set; }

        public bool AutoAddNewSeasons { get; set; }
        public string AutoAdd_FolderBase { get; set; }
        public bool AutoAdd_FolderPerSeason { get; set; }
        public string AutoAdd_SeasonFolderName { get; set; }
        public bool CountSpecials { get; set; }
        public string CustomShowName { get; set; }
        public bool DVDOrder { get; set; }
        public bool DoMissingCheck { get; set; }
        public bool DoRename { get; set; }
        public bool ForceCheckFuture { get; set; }
        public bool ForceCheckNoAirdate { get; set; }
        public List<int> IgnoreSeasons { get; set; }
        public Dictionary<int, StringList> ManualFolderLocations { get; set; }
        public bool PadSeasonToTwoDigits { get; set; }
        public Dictionary<int, ProcessedEpisodeList> SeasonEpisodes { get; set; }
        public Dictionary<int, List<ShowRule>> SeasonRules { get; set; }
        public bool ShowNextAirdate { get; set; }
        public TheTVDB TVDB { get; set; }
        public int TVDBCode { get; set; }
        public bool UseCustomShowName { get; set; }
        public bool UseSequentialMatch { get; set; }
    }
}
