using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TvdbLib;
using TvdbLib.Cache;
using TvdbLib.Data;

namespace TVRename.db_access.document
{
    public class ShowItem
    {
        public enum ShowAirStatus
        {
            NoEpisodesOrSeasons,
            Aired,
            PartiallyAired,
            NoneAired
        }

        // fields for the TV Rename program
        public bool UseCustomShowName { get; set; }
        public string CustomShowName { get; set; }
        public bool ShowNextAirdate { get; set; }
        public string tvdbid{ get; set; }
        public bool AutoAddNewSeasons { get; set; }
        public string AutoAdd_FolderBase { get; set; } // TODO: use magical renaming tokens here
        public bool AutoAdd_FolderPerSeason { get; set; }
        public string AutoAdd_SeasonFolderName { get; set; } // TODO: use magical renaming tokens here
        public bool DoRename { get; set; }
        public bool DoMissingCheck { get; set; }
        public bool CountSpecials { get; set; }
        public bool DVDOrder { get; set; } // sort by DVD order, not the default sort we get
        public bool ForceCheckNoAirdate { get; set; }
        public bool ForceCheckFuture { get; set; }
        public bool UseSequentialMatch { get; set; }
        public bool PadSeasonToTwoDigits { get; set; }
        public List<int> IgnoreSeasons { get; set; }
        public Dictionary<int, List<ShowRule>> SeasonRules { get; set; }
        public bool Dirty; // set to true if local info is known to be older than whats on the server


        // unknown
        private string LastFiguredTZ { get; set; }
        private TimeZone SeriesTZ { get; set; }

        // fields from TVDB
        public int TVDBCode { get; set; }
        public List<TvdbActor>TvdbActors { get; set; }
        public Nullable<DayOfWeek> AirsDayOfWeek { get; set; }
        public string AirsTime { get; set; }
        public DateTime FirstAired { get; set; }
        public string GenreString { get; }
        public string ImdbId { get; set; }
        public string Language;

        public string Overview { get; set; }
        public double Rating{get;set;}
        public double Runtime{get;set;}
        public string SeriesName { get; set; }
        public string ShowName { get; set; }
        public ShowAirStatus SeasonsAirStatus { get; set; }
        public long Srv_LastUpdated { get; set; }
        public string ShowTimeZone { get; set; } // set for us by ShowItem
        public string ContentRating { get; set; }
        public DateTime FirstAired {get; set;}
        public int SeriesID { get; set; }
        public string Network { get; set; }
        public int SeriesId { get; set; }
        //<Year>2008</Year>
        public string Zap2itId { get; set; }
        public string Network { get; set; }
        //<RatingCount>183</RatingCount>
        //<added></added>
        //<addedBy></addedBy>
        //<TimeZone>Eastern Standard Time</TimeZone>

        public List<Images> Images { get; set; } //BannerPath, FanartPath, poster
        public Dictionary<int, StringList> ManualFolderLocations { get; set; }
        public Dictionary<int, ProcessedEpisodeList> SeasonEpisodes { get; set; } // built up by applying rules.

        public void SetDefaults()
        {
            this.ManualFolderLocations = new System.Collections.Generic.Dictionary<int, StringList>();
            this.IgnoreSeasons = new System.Collections.Generic.List<int>();
            this.UseCustomShowName = false;
            this.CustomShowName = "";
            this.UseSequentialMatch = false;
            this.SeasonRules = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<ShowRule>>();
            this.SeasonEpisodes = new System.Collections.Generic.Dictionary<int, ProcessedEpisodeList>();
            this.ShowNextAirdate = true;
            this.TVDBCode = -1;
            //                WhichSeasons = gcnew System.Collections.Generic.List<int>;
            //                NamingStyle = (int)NStyle.DefaultStyle();
            this.AutoAddNewSeasons = true;
            this.PadSeasonToTwoDigits = false;
            this.AutoAdd_FolderBase = "";
            this.AutoAdd_FolderPerSeason = true;
            this.AutoAdd_SeasonFolderName = "Season ";
            this.DoRename = true;
            this.DoMissingCheck = true;
            this.CountSpecials = false;
            this.DVDOrder = false;
            ForceCheckNoAirdate = false;
            ForceCheckFuture = false;
        }
    }
}
